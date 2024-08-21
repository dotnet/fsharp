// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Telemetry

open Microsoft.VisualStudio.Telemetry
open System
open System.Diagnostics
open System.Collections.Concurrent
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.FSharp.Editor

#nowarn "3220" // Ignore warning about direct tuple items access.

[<RequireQualifiedAccess>]
module TelemetryEvents =
    [<Literal>]
    let CodefixActivated = "codefixactivated"

    [<Literal>]
    let Hints = "hints"

    [<Literal>]
    let LanguageServiceStarted = "languageservicestarted"

    [<Literal>]
    let GetSymbolUsesInProjectsStarted = "getsymbolusesinprojectsstarted"

    [<Literal>]
    let GetSymbolUsesInProjectsFinished = "getsymbolusesinprojectsfinished"

    [<Literal>]
    let AddSyntacticClassifications = "addsyntacticclassifications"

    [<Literal>]
    let AddSemanticClassifications = "addsemanticclassifications"

    [<Literal>]
    let GetDiagnosticsForDocument = "getdiagnosticsfordocument"

    [<Literal>]
    let ProvideCompletions = "providecompletions"

    [<Literal>]
    let GoToDefinition = "gotodefinition"

    [<Literal>]
    let GoToDefinitionGetSymbol = "gotodefinition/getsymbol"

    [<Literal>]
    let AnalysisSaveFileHandler = "analysis/savefilehandler"

// TODO: needs to be something more sophisticated in future
[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type TelemetryThrottlingStrategy =
    | NoThrottling
    | Throttle of {| Timeout: TimeSpan |}
    // At most, send one event per 5 seconds.
    static member Default =
        Throttle
            {|
                Timeout = TimeSpan.FromSeconds(5.0)
            |}

[<RequireQualifiedAccess>]
module TelemetryReporter =
    let internal noopDisposable =
        { new IDisposable with
            member _.Dispose() = ()
        }

    let internal lastTriggeredEvents = ConcurrentDictionary<string, DateTime>()

    [<Literal>]
    let eventPrefix = "dotnet/fsharp/"

    [<Literal>]
    let propPrefix = "dotnet.fsharp."

    // This should always be inlined.
    let inline createEvent name (props: (string * obj) array) =
        let eventName = eventPrefix + name
        let event = TelemetryEvent(eventName, TelemetrySeverity.Normal)

        // TODO:
        // We need to utilize TelemetryEvent's Correlation id, so we can track (for example) events on one document in the project.

        // TODO: need to carefully review the code, since it will be a hot path when we are sending telemetry
        // This particular approach is here to avoid allocations for properties, which is likely the case if we destructing them.
        for prop in props do
            event.Properties.Add(propPrefix + prop.Item1, prop.Item2)

        event

[<Struct; NoComparison; NoEquality>]
type TelemetryReporter private (name: string, props: (string * obj) array, stopwatch: Stopwatch) =

    static member val private SendAdditionalTelemetry =
        lazy
            (let componentModel =
                Package.GetGlobalService(typeof<ComponentModelHost.SComponentModel>) :?> ComponentModelHost.IComponentModel

             if isNull componentModel then
                 TelemetryService.DefaultSession.IsUserMicrosoftInternal
             else
                 let workspace = componentModel.GetService<VisualStudioWorkspace>()
                 let options = workspace.Services.GetService<EditorOptions>()

                 TelemetryService.DefaultSession.IsUserMicrosoftInternal
                 || options.Advanced.SendAdditionalTelemetry
                 || options.Advanced.UseTransparentCompiler)

    static member ReportFault(name, ?severity: FaultSeverity, ?e: exn) =
        if TelemetryReporter.SendAdditionalTelemetry.Value then
            let faultName = String.Concat(TelemetryReporter.eventPrefix, name, "/fault")

            match severity, e with
            | Some s, Some e -> TelemetryService.DefaultSession.PostFault(faultName, name, s, e)
            | None, Some e -> TelemetryService.DefaultSession.PostFault(faultName, name, e)
            | Some s, None -> TelemetryService.DefaultSession.PostFault(faultName, name, s)
            | None, None -> TelemetryService.DefaultSession.PostFault(faultName, name)
            |> ignore

    static member ReportCustomFailure(name, ?props) =
        if TelemetryReporter.SendAdditionalTelemetry.Value then
            let props = defaultArg props [||]
            let name = String.Concat(TelemetryReporter.eventPrefix, name, "/failure")
            let event = TelemetryReporter.createEvent name props
            TelemetryService.DefaultSession.PostEvent event

    static member ReportSingleEvent(name, props) =
        if TelemetryReporter.SendAdditionalTelemetry.Value then
            let event = TelemetryReporter.createEvent name props
            TelemetryService.DefaultSession.PostEvent event

    // A naïve implementation using stopwatch and returning an IDisposable
    // TODO: needs a careful review, since it will be a hot path when we are sending telemetry
    static member ReportSingleEventWithDuration(name, props, ?throttlingStrategy) : IDisposable =

        let additionalTelemetryEnabled = TelemetryReporter.SendAdditionalTelemetry.Value

        if additionalTelemetryEnabled then
            let throttlingStrategy =
                defaultArg throttlingStrategy TelemetryThrottlingStrategy.Default

            match throttlingStrategy with
            | TelemetryThrottlingStrategy.NoThrottling ->
                let stopwatch = Stopwatch()
                stopwatch.Start()
                new TelemetryReporter(name, props, stopwatch)
            | TelemetryThrottlingStrategy.Throttle s ->
                // This is not "atomic" for now, theoretically multiple threads can send the event as for now.
                match TelemetryReporter.lastTriggeredEvents.TryGetValue(name) with
                | false, lastTriggered
                | true, lastTriggered when lastTriggered + s.Timeout < DateTime.UtcNow ->
                    let stopwatch = Stopwatch()
                    stopwatch.Start()
                    // Whenever we create an event, we update the time.
                    TelemetryReporter.lastTriggeredEvents.AddOrUpdate(name, DateTime.UtcNow, (fun _ _ -> DateTime.UtcNow))
                    |> ignore

                    new TelemetryReporter(name, props, stopwatch)
                | _ -> TelemetryReporter.noopDisposable
        else
            TelemetryReporter.noopDisposable

    interface IDisposable with
        member _.Dispose() =
            stopwatch.Stop()

            let event =
                TelemetryReporter.createEvent name (Array.concat [ props; [| "vs_event_duration_ms", stopwatch.ElapsedMilliseconds |] ])

            TelemetryService.DefaultSession.PostEvent event
