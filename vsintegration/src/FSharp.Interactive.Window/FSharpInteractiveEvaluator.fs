// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.Threading.Tasks

open Microsoft.VisualStudio.InteractiveWindow
open Microsoft.VisualStudio.Text

/// The windows currently evaluating F#.
///
/// The interactive window package records itself in the properties of the buffers it creates for
/// input, but not in those it creates for output, and it replaces the output buffer on every reset.
/// So an output buffer can only be recognised by asking the windows we know about which buffer is
/// theirs right now.
module internal FSharpInteractiveWindows =

    let private windows = ResizeArray<IInteractiveWindow>()

    let add (window: IInteractiveWindow) =
        lock windows (fun () ->
            if not (windows.Contains window) then
                windows.Add window)

    let remove (window: IInteractiveWindow) =
        lock windows (fun () -> windows.Remove window |> ignore)

    let ownsOutputBuffer (buffer: ITextBuffer) =
        lock windows (fun () -> windows |> Seq.exists (fun window -> obj.ReferenceEquals(window.OutputBuffer, buffer)))

module internal ResultRendering =

    let formatDiagnostic (diagnostic: FSharp.Compiler.Interactive.Protocol.DiagnosticInfo) =
        $"{diagnostic.fileName}({diagnostic.startLine},{diagnostic.startColumn + 1}): {diagnostic.severity} FS%04d{diagnostic.errorNumber}: {diagnostic.message}"

    /// The line the window shows when a session comes up: which fsi answered, on what, and where.
    let formatSessionStart (session: FSharp.Compiler.Interactive.Protocol.InitializeResult) =
        $"F# Interactive {session.fsiVersion} on {session.frameworkDescription}, in {session.workingDirectory}"

/// Connects the interactive window to an F# Interactive session.
[<Sealed>]
type internal FSharpInteractiveEvaluator
    (
        host: InteractiveHostClient,
        getOptions: unit -> InteractiveHostOptions,
        scanners: ILexicalScannerFactory
    ) =

    let mutable currentWindow: IInteractiveWindow | null = null
    let mutable outputSubscription: IDisposable | null = null
    let mutable errorSubscription: IDisposable | null = null
    let mutable exitedSubscription: IDisposable | null = null
    let mutable startedSubscription: IDisposable | null = null
    let mutable disposed = false

    // The window submits text without saying where it came from, so an editor command records the
    // origin here for the submission it is about to make. Both run on the UI thread.
    let mutable nextSubmissionOrigin: struct (string * int) voption = ValueNone

    // Output arrives on the threads pumping the session's console streams, so it goes through the
    // window's writers rather than its editing operations, which belong to the UI thread.
    let write (text: string) =
        match currentWindow with
        | null -> ()
        | window -> window.OutputWriter.Write text

    let writeLine (text: string) =
        match currentWindow with
        | null -> ()
        | window -> window.OutputWriter.WriteLine text

    let writeError (text: string) =
        match currentWindow with
        | null -> ()
        | window -> window.ErrorOutputWriter.Write text

    let writeErrorLine (text: string) =
        match currentWindow with
        | null -> ()
        | window -> window.ErrorOutputWriter.WriteLine text

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

    let ensureSessionAsync () =
        task {
            match! host.EnsureStartedAsync(getOptions ()) with
            | Result.Ok _ -> return true
            | Result.Error message ->
                writeErrorLine message
                return false
        }

    let unsubscribe (subscription: IDisposable | null) =
        match subscription with
        | null -> ()
        | subscription -> subscription.Dispose()

    let reportSessionExit exitCode =
        writeErrorLine $"{VFSIstrings.SR.sessionTerminationDetected()} (exit code {exitCode})"

    let reportSessionStart session =
        writeLine (ResultRendering.formatSessionStart session)

    /// Attribute the next submission to a file and line, so that its diagnostics land on the user's
    /// own source rather than on the submission.
    member _.SetNextSubmissionOrigin(sourcePath: string, startLine: int) =
        nextSubmissionOrigin <-
            if String.IsNullOrEmpty sourcePath then
                ValueNone
            else
                ValueSome(struct (sourcePath, startLine))

    member _.EvaluatingProcessId = host.EvaluatingProcessId

    member _.Host = host

    interface IInteractiveEvaluator with

        member _.CurrentWindow
            with get () = currentWindow
            and set window =
                currentWindow <- window

                unsubscribe outputSubscription
                unsubscribe errorSubscription
                unsubscribe exitedSubscription
                unsubscribe startedSubscription

                match window with
                | null -> ()
                | window ->
                    FSharpInteractiveWindows.add window
                    outputSubscription <- host.OutputReceived.Subscribe write
                    errorSubscription <- host.ErrorOutputReceived.Subscribe writeError
                    exitedSubscription <- host.ProcessExited.Subscribe reportSessionExit
                    startedSubscription <- host.SessionStarted.Subscribe reportSessionStart

        member _.InitializeAsync() =
            task {
                let! started = ensureSessionAsync ()
                return ExecutionResult started
            }

        // `initialize` distinguishes a reset that runs start-up work from one that does not. An F#
        // session has none to vary, and the flag never means "do not start a replacement".
        member _.ResetAsync(_initialize) =
            task {
                match! host.ResetAsync(getOptions ()) with
                | Result.Ok _ -> return ExecutionResult true
                | Result.Error message ->
                    writeErrorLine message
                    return ExecutionResult false
            }

        member _.CanExecuteCode(text) = SubmissionAnalysis.isComplete scanners text

        member _.ExecuteCodeAsync(text) =
            task {
                let! started = ensureSessionAsync ()

                match started, String.IsNullOrWhiteSpace text with
                | false, _ -> return ExecutionResult false
                | true, true -> return ExecutionResult true
                | true, false ->
                    let origin = nextSubmissionOrigin
                    nextSubmissionOrigin <- ValueNone

                    let submit code =
                        match origin with
                        | ValueSome(struct (sourcePath, startLine)) -> host.ExecuteAsync(code, sourcePath, startLine)
                        | ValueNone -> host.ExecuteAsync code

                    match! submit (SubmissionAnalysis.withTerminator scanners text) with
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

                unsubscribe outputSubscription
                unsubscribe errorSubscription
                unsubscribe exitedSubscription
                unsubscribe startedSubscription
                (host :> IDisposable).Dispose()
