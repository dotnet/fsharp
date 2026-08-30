// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.Globalization
open System.Threading.Tasks

open Microsoft.VisualStudio.InteractiveWindow

module internal ResultRendering =

    let formatDiagnostic (diagnostic: FSharp.Compiler.Interactive.Protocol.DiagnosticInfo) =
        $"{diagnostic.fileName}({diagnostic.startLine},{diagnostic.startColumn + 1}): "
        + $"{diagnostic.severity} FS%04d{diagnostic.errorNumber}: {diagnostic.message}"

/// Connects the interactive window to an F# Interactive session.
[<Sealed>]
type internal FSharpInteractiveEvaluator
    (
        host: InteractiveHostClient,
        getOptions: unit -> InteractiveHostOptions,
        onPlatformChanged: InteractiveHostPlatform -> unit
    ) =

    let mutable currentWindow: IInteractiveWindow = null
    let mutable outputSubscription: IDisposable = null
    let mutable errorSubscription: IDisposable = null
    let mutable exitedSubscription: IDisposable = null
    let mutable disposed = false
    let mutable requestedPlatform: InteractiveHostPlatform option = None

    // The window submits text without saying where it came from, so an editor command records the
    // origin here for the submission it is about to make. Both run on the UI thread.
    let mutable nextSubmissionOrigin: (string * int) option = None

    // Output arrives on the threads pumping the session's console streams, so it goes through the
    // window's writers rather than its editing operations, which belong to the UI thread.
    let write (text: string) =
        match currentWindow with
        | null -> ()
        | window -> window.OutputWriter.Write text

    let writeError (text: string) =
        match currentWindow with
        | null -> ()
        | window -> window.ErrorOutputWriter.Write text

    let writeErrorLine (text: string) =
        match currentWindow with
        | null -> ()
        | window -> window.ErrorOutputWriter.WriteLine text

    let optionsForNextSession () =
        let options = getOptions ()

        match requestedPlatform with
        | Some platform -> { options with Platform = platform }
        | None -> options

    let reportDiagnostics (result: FSharp.Compiler.Interactive.Protocol.ExecutionResult) =
        match result.diagnostics with
        | null -> ()
        | diagnostics ->
            for diagnostic in diagnostics do
                writeErrorLine (ResultRendering.formatDiagnostic diagnostic)

        match box result.``exception`` with
        | null -> ()
        | _ ->
            writeErrorLine result.``exception``.message

            if not (String.IsNullOrWhiteSpace result.``exception``.stackTrace) then
                writeErrorLine result.``exception``.stackTrace

    let ensureSessionAsync () =
        task {
            match! host.EnsureStartedAsync(optionsForNextSession ()) with
            | Result.Ok _ -> return true
            | Result.Error message ->
                writeErrorLine message
                return false
        }

    let unsubscribe (subscription: IDisposable) =
        match subscription with
        | null -> ()
        | subscription -> subscription.Dispose()

    let reportSessionExit exitCode =
        writeErrorLine (
            String.Format(
                CultureInfo.CurrentCulture,
                "{0} (exit code {1})",
                VFSIstrings.SR.sessionTerminationDetected (),
                exitCode
            )
        )

    member _.CurrentPlatform =
        match requestedPlatform with
        | Some platform -> platform
        | None -> (getOptions ()).Platform

    member _.RequestPlatform platform = requestedPlatform <- Some platform

    /// Attribute the next submission to a file and line, so that its diagnostics land on the user's
    /// own source rather than on the submission.
    member _.SetNextSubmissionOrigin(sourcePath: string, startLine: int) =
        nextSubmissionOrigin <-
            if String.IsNullOrEmpty sourcePath then
                None
            else
                Some(sourcePath, startLine)

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

                if not (isNull window) then
                    outputSubscription <- host.OutputReceived.Subscribe write
                    errorSubscription <- host.ErrorOutputReceived.Subscribe writeError
                    exitedSubscription <- host.ProcessExited.Subscribe reportSessionExit

        member _.InitializeAsync() =
            task {
                let! started = ensureSessionAsync ()
                return ExecutionResult started
            }

        // `initialize` distinguishes a reset that runs start-up work from one that does not. An F#
        // session has none to vary, and the flag never means "do not start a replacement".
        member _.ResetAsync(_initialize) =
            task {
                let options = optionsForNextSession ()
                onPlatformChanged options.Platform

                match! host.ResetAsync options with
                | Result.Ok _ -> return ExecutionResult true
                | Result.Error message ->
                    writeErrorLine message
                    return ExecutionResult false
            }

        member _.CanExecuteCode(text) = SubmissionAnalysis.isComplete text

        member _.ExecuteCodeAsync(text) =
            task {
                let! started = ensureSessionAsync ()

                if not started then
                    return ExecutionResult false
                elif String.IsNullOrWhiteSpace text then
                    return ExecutionResult true
                else
                    let origin = nextSubmissionOrigin
                    nextSubmissionOrigin <- None

                    let submit code =
                        match origin with
                        | Some(sourcePath, startLine) -> host.ExecuteAsync(code, sourcePath, startLine)
                        | None -> host.ExecuteAsync code

                    match! submit (SubmissionAnalysis.withTerminator text) with
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
            if
                not (isNull currentWindow)
                && not (isNull currentWindow.CurrentLanguageBuffer)
                && currentWindow.CurrentLanguageBuffer.CurrentSnapshot.LineCount > 1
            then
                "- "
            else
                "> "

    interface IDisposable with
        member _.Dispose() =
            if not disposed then
                disposed <- true
                unsubscribe outputSubscription
                unsubscribe errorSubscription
                unsubscribe exitedSubscription
                (host :> IDisposable).Dispose()
