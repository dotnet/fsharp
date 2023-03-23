// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Telemetry

open Microsoft.VisualStudio.Telemetry

module TelemetryReporter =

    let eventPrefix = "dotnet/fsharp/"
    let propPrefix = "dotnet.fsharp."

    let getFullEventName name = eventPrefix + name
    let getFullPropName name = propPrefix + name

    let createEvent name (props: (string * obj) list) =
        let event = TelemetryEvent(getFullEventName name)

        props
        |> List.map (fun (k, v) -> getFullPropName k, v)
        |> List.iter event.Properties.Add

        event

    let reportEvent name props =
        let session = TelemetryService.DefaultSession
        let event = createEvent name props
        session.PostEvent event
