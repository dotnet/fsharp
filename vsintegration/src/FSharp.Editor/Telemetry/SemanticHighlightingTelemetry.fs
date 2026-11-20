namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Microsoft.Internal.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.Editor.Telemetry
open Microsoft.VisualStudio.Settings
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Telemetry
open Microsoft.VisualStudio.Threading
open StreamJsonRpc

type internal HighlightingTelemetryConfiguration =
    {|
        RolloutSessionId: string
        Generation: int64
        Enabled: bool
    |}

[<CLIMutable>]
type internal HighlightingTelemetryConfigurationResult =
    {
        Generation: int64
        SemanticTokensEnabled: bool
    }

type internal SemanticHighlightingTelemetry
    (postEvent: TelemetryEvent -> unit, ?now: unit -> DateTime, ?delay: int * CancellationToken -> Task, ?isMicrosoftInternal: unit -> bool)
    =
    let sessionId = Guid.NewGuid().ToString("D")
    let now = defaultArg now (fun () -> DateTime.UtcNow)

    let delay =
        defaultArg delay (fun (milliseconds, ct) -> Task.Delay(milliseconds, ct))

    let isMicrosoftInternal =
        defaultArg isMicrosoftInternal (fun () -> TelemetryService.DefaultSession.IsUserMicrosoftInternal)

    let mutable exposed = 0
    let mutable generation = 0L
    let gate = obj ()
    let states = HashSet<string>(StringComparer.Ordinal)
    let mutable lastDetail = DateTime.MinValue

    let canReport enabled =
        try
            isMicrosoftInternal () || enabled ()
        with _ ->
            false

    let post name props enabled =
        try
            if canReport enabled then
                TelemetryReporter.createEvent name [| "schema_version", box 1; "rollout_session_id", box sessionId; yield! props |]
                |> postEvent
        with _ ->
            Trace.TraceWarning("F# highlighting telemetry could not be posted.")

    member _.ReportExposure(settingValue, provider, enabled) =
        if canReport enabled && Interlocked.CompareExchange(&exposed, 1, 0) = 0 then
            post "semantichighlighting/exposure" [| "setting_value", box settingValue; "resolved_provider", box provider |] enabled

    member _.ReportState(provider: string, state: string, props, enabled) =
        if canReport enabled && lock gate (fun () -> states.Add(provider + "/" + state)) then
            post "semantichighlighting/providerstate" [| "provider", box provider; "state", box state; yield! props |] enabled

    member _.IsEnabled(options: EditorOptions) =
        canReport (fun () -> options.Advanced.SendAdditionalTelemetry)

    member this.Configure(options: EditorOptions) =
        lock gate (fun () ->
            {|
                RolloutSessionId = sessionId
                Generation = Interlocked.Increment(&generation)
                Enabled = this.IsEnabled options
            |})

    member this.ConfigureServer
        (
            options: EditorOptions,
            send: HighlightingTelemetryConfiguration -> Task<HighlightingTelemetryConfigurationResult>,
            retry,
            cancellationToken: CancellationToken
        ) =
        task {
            let enabled () =
                options.Advanced.SendAdditionalTelemetry

            try
                let mutable configured = false
                // Loaded clients need not be initialized yet. Retry only this startup handshake.
                let delays =
                    if retry then
                        [| 0; 250; 1000; 4000; 16000; 64000; 128000 |]
                    else
                        [| 0 |]

                for milliseconds in delays do
                    if not configured then
                        do! delay (milliseconds, cancellationToken)
                        let parameters = this.Configure options

                        let! result =
                            task {
                                try
                                    return! send parameters
                                with
                                | :? TimeoutException
                                | :? ConnectionLostException
                                | :? ObjectDisposedException
                                | :? OperationCanceledException when retry && not cancellationToken.IsCancellationRequested ->
                                    return Unchecked.defaultof<_>
                            }

                        configured <- not (isNull (box result)) && result.Generation >= parameters.Generation

                        if configured then
                            this.ReportState("lsp", (if result.SemanticTokensEnabled then "ready" else "disabled"), [||], enabled)

                if retry && not configured then
                    this.ReportState("lsp", "unavailable", [||], enabled)
            with
            | :? OperationCanceledException when cancellationToken.IsCancellationRequested -> ()
            | _ -> this.ReportState("lsp", "unavailable", [||], enabled)
        }

    member _.ReportDetails(enabled: unit -> bool, props) =
        try
            if canReport enabled then
                let sampled =
                    lock gate (fun () ->
                        let current = now ()

                        if current - lastDetail >= TimeSpan.FromSeconds 5. then
                            lastDetail <- current
                            true
                        else
                            false)

                if sampled && canReport enabled then
                    post
                        TelemetryEvents.AddSemanticClassifications
                        [| "provider", box "old"; "request_kind", box "span"; yield! props |]
                        enabled
        with _ ->
            Trace.TraceWarning("F# highlighting telemetry could not be collected.")

type internal LspTelemetryConsentForwarder
    (
        telemetry: SemanticHighlightingTelemetry,
        options: EditorOptions,
        context: JoinableTaskContext,
        send: HighlightingTelemetryConfiguration -> CancellationToken -> Task<HighlightingTelemetryConfigurationResult>
    ) =
    let cancellation = new CancellationTokenSource()
    let cancellationToken = cancellation.Token
    let requests = context.CreateCollection()
    let factory = context.CreateFactory requests
    let subscriptions = HashSet<IDisposable>()
    let gate = obj ()
    let mutable configuring = 0
    let mutable disposed = 0

    let configure retry =
        if
            not cancellationToken.IsCancellationRequested
            && (not retry || Interlocked.CompareExchange(&configuring, 1, 0) = 0)
        then
            factory
                .RunAsync(fun () ->
                    task {
                        try
                            do!
                                telemetry.ConfigureServer(
                                    options,
                                    (fun parameters -> send parameters cancellationToken),
                                    retry,
                                    cancellationToken
                                )
                        finally
                            if retry then
                                Interlocked.Exchange(&configuring, 0) |> ignore
                    }
                    :> Task)
                .FileAndForget("vs/fsharp/lsp/telemetry", fileOnlyIf = Func<Exception, bool>(fun _ -> telemetry.IsEnabled options))

    let settingsSubscription =
        options.SettingsChanged.Subscribe(fun () -> configure false)

    member _.Configure(retry) = configure retry

    member this.RegisterView(settingValue, provider, closed: IEvent<EventHandler, EventArgs>) =
        let subscription =
            options.SettingsChanged.Subscribe(fun () ->
                telemetry.ReportExposure(settingValue, provider, (fun () -> options.Advanced.SendAdditionalTelemetry)))

        lock gate (fun () ->
            if Volatile.Read(&disposed) = 0 then
                subscriptions.Add subscription |> ignore
            else
                subscription.Dispose())

        closed.Add(fun _ ->
            lock gate (fun () ->
                subscriptions.Remove subscription |> ignore
                subscription.Dispose()))

        this.Configure(retry = (provider = "lsp"))

    member _.WaitForPendingRequestsAsync() = requests.JoinTillEmptyAsync()

    interface IDisposable with
        member this.Dispose() =
            if Interlocked.Exchange(&disposed, 1) = 0 then
                settingsSubscription.Dispose()
                cancellation.Cancel()

                lock gate (fun () ->
                    for subscription in subscriptions do
                        subscription.Dispose()

                    subscriptions.Clear())

                context.Factory
                    .RunAsync(fun () ->
                        task {
                            try
                                do! this.WaitForPendingRequestsAsync()
                            finally
                                cancellation.Dispose()
                        }
                        :> Task)
                    .FileAndForget("vs/fsharp/lsp/telemetry", fileOnlyIf = Func<Exception, bool>(fun _ -> telemetry.IsEnabled options))

[<RequireQualifiedAccess>]
module internal HighlightingTelemetry =
    let instance =
        SemanticHighlightingTelemetry(TelemetryService.DefaultSession.PostEvent)

    let normalizeProvider value =
        let settingValue =
            match value with
            | "old"
            | "lsp"
            | "unset" -> value
            | _ -> "unrecognized"

        struct (settingValue, if value = "lsp" then "lsp" else "old")

    let readProvider (serviceProvider: IServiceProvider) =
        try
            let settings =
                serviceProvider.GetService(typeof<SVsUnifiedSettingsManager>)
                :?> Microsoft.VisualStudio.Utilities.UnifiedSettings.ISettingsManager

            settings.GetReader().GetValueOrThrow<string>("fsharp.semanticClassificationProvider")
            |> normalizeProvider
        with _ ->
            struct ("unavailable", "old")
