// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open System
open System.Diagnostics

/// Event name constants for LSP telemetry.
[<RequireQualifiedAccess>]
module TelemetryEvents =
    [<Literal>]
    let LspServerInitialized = "lsp/initialized"

    [<Literal>]
    let GetDiagnostics = "lsp/getdiagnostics"

    [<Literal>]
    let GetCodeActions = "lsp/getcodeactions"

    [<Literal>]
    let DocumentOpened = "lsp/documentopened"

    [<Literal>]
    let DocumentChanged = "lsp/documentchanged"

    [<Literal>]
    let DocumentClosed = "lsp/documentclosed"

    [<Literal>]
    let GetProjectContexts = "lsp/getprojectcontexts"

    [<Literal>]
    let ServerFault = "lsp/fault"

/// Abstraction for telemetry reporting in the F# LSP server.
/// Implementations can route events to VS telemetry, OpenTelemetry, or a no-op sink.
type ILspTelemetry =

    /// Report a simple telemetry event with optional properties.
    abstract ReportEvent: name: string * properties: (string * obj) array -> unit

    /// Report an event that measures duration. Dispose the returned handle to complete the measurement.
    abstract ReportEventWithDuration: name: string * properties: (string * obj) array -> IDisposable

    /// Report a fault/error event.
    abstract ReportFault: name: string * description: string * exn: exn option -> unit

/// No-op telemetry implementation for standalone/testing use.
type NullTelemetry() =
    static let noopDisposable =
        { new IDisposable with
            member _.Dispose() = ()
        }

    static member val Instance = NullTelemetry() :> ILspTelemetry

    interface ILspTelemetry with
        member _.ReportEvent(_name, _properties) = ()

        member _.ReportEventWithDuration(_name, _properties) = noopDisposable

        member _.ReportFault(_name, _description, _exn) = ()

/// Telemetry implementation that writes to System.Diagnostics.Trace (useful for debugging).
type TraceTelemetry() =

    interface ILspTelemetry with
        member _.ReportEvent(name, properties) =
            let propsStr =
                properties
                |> Array.map (fun (k, v) -> $"{k}={v}")
                |> String.concat ", "

            Trace.TraceInformation($"[Telemetry] {name} ({propsStr})")

        member _.ReportEventWithDuration(name, properties) =
            let sw = Stopwatch.StartNew()
            Trace.TraceInformation($"[Telemetry] {name} started")

            { new IDisposable with
                member _.Dispose() =
                    sw.Stop()

                    let propsStr =
                        properties
                        |> Array.map (fun (k, v) -> $"{k}={v}")
                        |> String.concat ", "

                    Trace.TraceInformation($"[Telemetry] {name} completed in {sw.ElapsedMilliseconds}ms ({propsStr})")
            }

        member _.ReportFault(name, description, exn) =
            match exn with
            | Some e -> Trace.TraceError($"[Telemetry] FAULT {name}: {description} - {e}")
            | None -> Trace.TraceError($"[Telemetry] FAULT {name}: {description}")
