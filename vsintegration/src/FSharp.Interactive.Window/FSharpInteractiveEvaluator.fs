// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.IO
open System.Threading.Tasks

open Microsoft.VisualStudio.InteractiveWindow
open Microsoft.VisualStudio.Text

/// The window package records the window in the properties of its input buffers but not of its
/// output buffers, and it replaces the output buffer on every reset. So an output buffer is
/// recognised by asking the windows evaluating F# which buffer is theirs right now.
module internal FSharpInteractiveWindows =

    let private windows = ResizeArray<IInteractiveWindow>()

    let add (window: IInteractiveWindow) =
        lock windows (fun () ->
            if not (windows.Contains window) then
                windows.Add window)

    let remove (window: IInteractiveWindow) =
        lock windows (fun () -> windows.Remove window |> ignore)

    let ownsOutputBuffer (buffer: ITextBuffer) =
        lock windows (fun () ->
            windows
            |> Seq.exists (fun window -> obj.ReferenceEquals(window.OutputBuffer, buffer)))

module internal ResultRendering =

    let formatDiagnostic (diagnostic: FSharp.Compiler.Interactive.Protocol.DiagnosticInfo) =
        $"{diagnostic.fileName}({diagnostic.startLine},{diagnostic.startColumn + 1}): {diagnostic.severity} FS%04d{diagnostic.errorNumber}: {diagnostic.message}"

    let formatSessionStart (session: RemoteSession) =
        let handshake = session.Initialization
        $"F# Interactive {handshake.fsiVersion} from {session.Origin.Description} on {handshake.frameworkDescription}, in {handshake.workingDirectory}"

[<Sealed>]
type internal FSharpInteractiveEvaluator
    (host: InteractiveHostClient, getOptions: unit -> InteractiveHostOptions, scanners: ILexicalScannerFactory) =

    let mutable currentWindow: IInteractiveWindow | null = null
    let mutable subscriptions: IDisposable list = []
    let mutable disposed = false

    let origins = SubmissionOrigins()

    // Output arrives on the threads pumping the session's console streams, hence the window's
    // writers: its editing operations belong to the UI thread.
    let output () =
        match currentWindow with
        | null -> TextWriter.Null
        | window -> window.OutputWriter

    let errorOutput () =
        match currentWindow with
        | null -> TextWriter.Null
        | window -> window.ErrorOutputWriter

    let writeErrorLine (text: string) = errorOutput().WriteLine text

    let reportDiagnostics (result: FSharp.Compiler.Interactive.Protocol.ExecutionResult) =
        match result.diagnostics with
        | null -> ()
        | diagnostics ->
            for diagnostic in diagnostics do
                writeErrorLine (ResultRendering.formatDiagnostic diagnostic)

        match result.``exception`` with
        | null -> ()
        | failure ->
            writeErrorLine failure.message

            if not (String.IsNullOrWhiteSpace failure.stackTrace) then
                writeErrorLine failure.stackTrace

    let startedAsync (start: InteractiveHostOptions -> Task<Result<RemoteSession, string>>) =
        task {
            match! start (getOptions ()) with
            | Result.Ok _ -> return ExecutionResult true
            | Result.Error message ->
                writeErrorLine message
                return ExecutionResult false
        }

    let unsubscribe () =
        for subscription in subscriptions do
            subscription.Dispose()

        subscriptions <- []

    member _.AddSubmissionOrigin(text: string, sourcePath: string, startLine: int) =
        if not (String.IsNullOrEmpty sourcePath) then
            origins.Add(text, sourcePath, startLine)

    interface IInteractiveEvaluator with

        member _.CurrentWindow
            with get () = currentWindow
            and set window =
                currentWindow <- window
                unsubscribe ()

                match window with
                | null -> ()
                | window ->
                    FSharpInteractiveWindows.add window

                    subscriptions <-
                        [
                            host.OutputReceived.Subscribe(fun text -> output().Write text)
                            host.ErrorOutputReceived.Subscribe(fun text -> errorOutput().Write text)
                            host.ProcessExited.Subscribe(fun exitCode ->
                                writeErrorLine $"{VFSIstrings.SR.sessionTerminationDetected ()} (exit code {exitCode})")
                            host.SessionStarted.Subscribe(fun session -> output().WriteLine(ResultRendering.formatSessionStart session))
                        ]

        member _.InitializeAsync() =
            startedAsync (fun options -> host.EnsureStartedAsync options)

        // `initialize` selects start-up work, of which an F# session has none to vary; it never
        // means "do not start a replacement".
        member _.ResetAsync(_initialize) =
            startedAsync (fun options -> host.ResetAsync options)

        member _.CanExecuteCode(text) =
            SubmissionAnalysis.isComplete scanners text

        member _.ExecuteCodeAsync(text) =
            task {
                let origin = origins.Take text

                let! started = startedAsync (fun options -> host.EnsureStartedAsync options)

                if not started.IsSuccessful || String.IsNullOrWhiteSpace text then
                    return started
                else
                    match! host.ExecuteAsync(SubmissionAnalysis.withTerminator scanners text, origin) with
                    | Result.Error message ->
                        writeErrorLine message
                        return ExecutionResult false
                    | Result.Ok result ->
                        reportDiagnostics result
                        return ExecutionResult result.success
            }

        member _.AbortExecution() = host.InterruptAsync() |> ignore

        member _.FormatClipboard() = null

        member _.GetPrompt() =
            match currentWindow with
            | null -> "> "
            | window ->
                match window.CurrentLanguageBuffer with
                | null -> "> "
                | buffer when buffer.CurrentSnapshot.LineCount > 1 -> "- "
                | _ -> "> "

    interface IDisposable with
        member _.Dispose() =
            if not disposed then
                disposed <- true

                match currentWindow with
                | null -> ()
                | window -> FSharpInteractiveWindows.remove window

                unsubscribe ()
                (host :> IDisposable).Dispose()
