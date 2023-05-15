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
    let GetSymbolUsesInProjectsStarted = "getSymbolUsesInProjectsStarted"

    [<Literal>]
    let GetSymbolUsesInProjectsFinished = "getSymbolUsesInProjectsFinished"

    [<Literal>]
    let AddSyntacticCalssifications = "addsyntacticclassifications"

    [<Literal>]
    let AddSemanticCalssifications = "addsemanticclassifications"

    [<Literal>]
    let GetDiagnosticsForDocument = "getdiagnosticsfordocument"

    [<Literal>]
    let ProvideCompletions = "providecompletions"

// TODO: needs to be something more sophisticated in future
[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type TelemetryThrottlingStrategy =
    | NoThrottling
    | Throttle of {| Timeout: TimeSpan |}
    // At most, send one event per 3 seconds.
    static member Default =
        Throttle
            {|
                Timeout = TimeSpan.FromSeconds(3.0)
            |}

[<RequireQualifiedAccess>]
module TelemetryReporter =
    let internal noopDisposable =
        { new IDisposable with
            member _.Dispose() = ()
        }

    let internal lastSentEvents = ConcurrentDictionary<string, DateTime>()

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
        // This particular approach is here to avoid alocations for properties, which is likely the case if we destructing them.
        for prop in props do
            event.Properties.Add(propPrefix + prop.Item1, prop.Item2)

        event

[<Struct; NoComparison; NoEquality>]
type TelemetryReporter private (name: string, props: (string * obj) array, stopwatch: Stopwatch) =

    static let settings =
        lazy
            (let componentModel =
                Package.GetGlobalService(typeof<ComponentModelHost.SComponentModel>) :?> ComponentModelHost.IComponentModel

             let workspace = componentModel.GetService<VisualStudioWorkspace>()
             workspace.Services.GetService<EditorOptions>())

    static member ReportSingleEvent(name, props) =
        let event = TelemetryReporter.createEvent name props
        TelemetryService.DefaultSession.PostEvent event

    // A naïve implementation using stopwatch and returning an IDisposable
    // TODO: needs a careful review, since it will be a hot path when we are sending telemetry
    static member ReportSingleEventWithDuration(name, props, ?throttlingStrategy) : IDisposable =

        if not settings.Value.Advanced.SendAdditionalTelemetry then
            TelemetryReporter.noopDisposable
        else
            let throttlingStrategy =
                defaultArg throttlingStrategy TelemetryThrottlingStrategy.Default

            match throttlingStrategy with
            | TelemetryThrottlingStrategy.NoThrottling ->
                let stopwatch = Stopwatch()
                stopwatch.Start()
                new TelemetryReporter(name, props, stopwatch)
            | TelemetryThrottlingStrategy.Throttle s ->
                // This is not "atomic" for now, theoretically multiple threads can send the event as for now.
                match TelemetryReporter.lastSentEvents.TryGetValue(name) with
                | false, lastSent
                | true, lastSent when lastSent + s.Timeout < DateTime.UtcNow ->
                    let stopwatch = Stopwatch()
                    stopwatch.Start()
                    // Whenever we create an event, we update the last sent time.
                    TelemetryReporter.lastSentEvents.AddOrUpdate(name, DateTime.UtcNow, (fun _ _ -> DateTime.UtcNow))
                    |> ignore

                    new TelemetryReporter(name, props, stopwatch)
                | _ -> TelemetryReporter.noopDisposable

    interface IDisposable with
        member _.Dispose() =
            stopwatch.Stop()

            let event =
                TelemetryReporter.createEvent name (Array.concat [ props; [| "vs_event_duration_ms", stopwatch.ElapsedMilliseconds |] ])

            TelemetryService.DefaultSession.PostEvent event
