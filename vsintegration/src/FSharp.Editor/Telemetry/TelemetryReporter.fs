// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Telemetry

open Microsoft.VisualStudio.Telemetry
open System
open System.Diagnostics

#nowarn "3220" // Ignore warning about direct tuple items access.

[<RequireQualifiedAccess>]
module TelemetryReporter =
    let [<Literal>] eventPrefix = "dotnet/fsharp/"
    let [<Literal>] propPrefix = "dotnet.fsharp."

    // This should always be inlined.
    let inline createEvent name (props: (string * obj) array) =
        let eventName = eventPrefix + name
        let event = TelemetryEvent(eventName)
        
        // TODO: need to carefully review the code, since it will be a hot path when we are sending telemetry
        // This particular approach is here to avoid alocations for properties, which is likely the case if we destructing them.
        for prop in props do
            event.Properties.Add(propPrefix + prop.Item1, prop.Item2)

        event

[<Struct; NoComparison; NoEquality>]
type TelemetryReporter private (name: string, props: (string * obj) array, stopwatch: Stopwatch) = 

    static member ReportSingleEvent (name, props) =
        let session = TelemetryService.DefaultSession
        let event = TelemetryReporter.createEvent name props
        session.PostEvent event

    // A naïve implementation using stopwatch and returning an IDisposable
    // TODO: needs a careful review, since it will be a hot path when we are sending telemetry
    static member ReportSingleEventWithDuration (name, props) : IDisposable =
        let stopwatch = Stopwatch()
        stopwatch.Start()
        new TelemetryReporter(name, props, stopwatch)

    interface IDisposable with
        member _.Dispose() =
            let session = TelemetryService.DefaultSession
            stopwatch.Stop()
            let event = TelemetryReporter.createEvent name (Array.concat [props; [| "vs_event_duration_ms", stopwatch.ElapsedMilliseconds |]])
            session.PostEvent event 

    