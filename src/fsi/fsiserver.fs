// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// <summary>
/// The JSON-RPC server mode of F# Interactive, activated by <c>--fsi-server-jsonrpc:&lt;pipe name&gt;</c>.
/// </summary>
/// <remarks>
/// <para>
/// An editor hosting F# Interactive needs two things from the process: a control channel to submit
/// interactions and receive structured results, and the program's own console output. This server
/// keeps those apart. Control traffic is JSON-RPC over a named pipe; everything the script itself
/// prints continues to flow through the redirected standard output and error streams, exactly as it
/// does for a console session. That separation is what removes the need for a host to recognise
/// prompts in the output text in order to tell one interaction's results from the next.
/// </para>
/// <para>
/// The transport is StreamJsonRpc over a header-delimited stream, the same combination Roslyn's
/// interactive host uses, so a client built on that library talks to this one with its stock
/// message handler.
/// </para>
/// <para>
/// Threading mirrors the standard input path of a console session. Interactions are evaluated on
/// the event loop thread by way of <c>EventLoopInvoke</c>, so scripts that create user interface
/// objects behave as they do at the console. Interactions are queued onto a single worker so that
/// they run in the order they arrived, while requests that must not wait behind them — an interrupt
/// above all — are served as they arrive.
/// </para>
/// <para>
/// Not <c>module internal</c>: <c>FsiRpcTarget</c> needs its members to be genuinely public IL, and
/// under <c>--realsig-</c> a member's own accessibility is capped by its enclosing module's, so an
/// internal module would take that away no matter what the type itself declares. Everything else
/// here goes back to <c>private</c>/<c>internal</c> explicitly instead of inheriting it from the
/// module.
/// </para>
/// </remarks>
module FSharp.Compiler.Interactive.Server

open System
open System.Collections.Concurrent
open System.Diagnostics
open System.IO
open System.IO.Pipes
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open StreamJsonRpc

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Interactive.Protocol
open FSharp.Compiler.Interactive.Shell

/// The name of the command line option that turns on this server.
[<Literal>]
let internal JsonRpcServerOption = "--fsi-server-jsonrpc:"

/// File name reported for interactions that the host did not attribute to a source file.
[<Literal>]
let private DefaultInteractionName = "stdin.fsx"

//-------------------------------------------------------------------------
// Shaping results for the wire
//-------------------------------------------------------------------------

let private severityText (severity: FSharpDiagnosticSeverity) =
    match severity with
    | FSharpDiagnosticSeverity.Error -> "error"
    | FSharpDiagnosticSeverity.Warning -> "warning"
    | FSharpDiagnosticSeverity.Info -> "info"
    | FSharpDiagnosticSeverity.Hidden -> "hidden"

let private toDiagnosticInfo (diagnostic: FSharpDiagnostic) =
    {
        severity = severityText diagnostic.Severity
        message = diagnostic.Message
        errorNumber = diagnostic.ErrorNumber
        subcategory = diagnostic.Subcategory
        fileName = diagnostic.FileName
        startLine = diagnostic.StartLine
        startColumn = diagnostic.StartColumn
        endLine = diagnostic.EndLine
        endColumn = diagnostic.EndColumn
    }

let private toExecutionResult (outcome: Choice<FsiValue option, exn>) (diagnostics: FSharpDiagnostic[]) (cancelled: bool) =
    let hasErrors =
        diagnostics
        |> Array.exists (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)

    let failure =
        match outcome with
        | Choice1Of2 _ -> None
        // When the interaction failed to compile, the diagnostics already say everything there is
        // to say. The exception raised to stop processing carries no more information, and a host
        // that reported it alongside them would be saying the same thing twice.
        | Choice2Of2 _ when hasErrors -> None
        | Choice2Of2 e -> Some e

    {
        success = not hasErrors && failure.IsNone && not cancelled
        cancelled = cancelled
        diagnostics = diagnostics |> Array.map toDiagnosticInfo
        ``exception`` =
            match failure with
            | Some e ->
                {
                    ``type`` = e.GetType().FullName
                    message = e.Message
                    stackTrace =
                        match e.StackTrace with
                        | null -> ""
                        | trace -> trace
                }
            | None -> Unchecked.defaultof<ExceptionInfo>
        workingDirectory = Directory.GetCurrentDirectory()
    }

//-------------------------------------------------------------------------
// The server
//-------------------------------------------------------------------------

/// <summary>
/// Serialises the interactions submitted by the host onto a single worker, so that they are
/// evaluated strictly in the order they were received.
/// </summary>
/// <remarks>
/// Owned by the server loop rather than by the target the host calls into: closing the queue ends
/// the session's willingness to run anything, and must not be reachable from the wire.
/// </remarks>
[<Sealed>]
type internal ExecutionQueue() =
    let queue = new BlockingCollection<unit -> unit>()

    let worker =
        Thread(
            (fun () ->
                for job in queue.GetConsumingEnumerable() do
                    // A job reports its own failures to the host; nothing here may escape and kill
                    // the worker, or the session would stop responding to every later request.
                    try
                        job ()
                    with _ ->
                        ()),
            Name = "FSI-JsonRpc-Execute",
            IsBackground = true
        )

    do worker.Start()

    /// <summary>
    /// False once the queue is closed, when the job will never run. Checking
    /// <c>IsAddingCompleted</c> first would still race with the close, and a job silently dropped
    /// leaves the host waiting on a task nothing completes.
    /// </summary>
    member _.TryEnqueue(job: unit -> unit) =
        try
            queue.Add job
            true
        with :? InvalidOperationException ->
            false

    member _.Complete() = queue.CompleteAdding()

/// <summary>The object the host calls into.</summary>
/// <remarks>
/// <para>
/// Everything that evaluates code goes onto the execution queue and completes its task when the
/// interaction finishes, which leaves StreamJsonRpc free to dispatch an interrupt in the meantime.
/// </para>
/// <para>
/// Public, not <c>internal</c>: <c>AddLocalRpcTarget</c> discovers <c>JsonRpcMethod</c> members by
/// reflecting over the instance it is handed, and under <c>--realsig-</c> a member's own IL
/// visibility is capped by its enclosing scope's, so an internal type (or an internal module around
/// a public one) would take away the public visibility that reflection needs regardless of what the
/// members declare.
/// </para>
/// <para>
/// StreamJsonRpc offers every public member, not only the attributed ones, so the public members
/// here are exactly the handlers the protocol defines. The construction the server loop needs goes
/// through the internal constructor instead.
/// </para>
/// </remarks>
[<Sealed>]
type FsiRpcTarget
    internal
    (
        fsiSession: FsiEvaluationSession,
        fsiConfig: FsiEvaluationSessionHostConfig,
        outWriter: TextWriter,
        errorWriter: TextWriter,
        shutdownRequested: TaskCompletionSource<unit>,
        executionQueue: ExecutionQueue
    ) =

    let interruptLock = obj ()
    let mutable currentCancellation: CancellationTokenSource = null
    let mutable initialized = false

    /// <summary>
    /// Evaluate on the event loop thread, the same thread a console session evaluates on.
    /// </summary>
    /// <remarks>
    /// <c>EvalInteractionNonThrowing</c> reports diagnostics and execution failures through its
    /// result, but a failure inside the event loop machinery itself would still escape, so it is
    /// caught here and reported as an ordinary failed interaction.
    /// </remarks>
    let evaluateOnEventLoop (evaluate: unit -> Choice<FsiValue option, exn> * FSharpDiagnostic[]) =
        try
            fsiConfig.EventLoopInvoke evaluate
        with e ->
            Choice2Of2 e, [||]

    /// Flush everything the interaction printed before answering, so that a host which shows
    /// standard output and RPC results side by side sees them in the order they were produced.
    let flushConsole () =
        try
            outWriter.Flush()
            errorWriter.Flush()
        with _ ->
            ()

    let runInteraction (code: string) (scriptPath: string) =
        let cancellation = new CancellationTokenSource()

        lock interruptLock (fun () -> currentCancellation <- cancellation)

        try
            let outcome, diagnostics =
                evaluateOnEventLoop (fun () -> fsiSession.EvalInteractionNonThrowing(code, scriptPath, cancellation.Token))

            flushConsole ()
            toExecutionResult outcome diagnostics cancellation.IsCancellationRequested
        finally
            lock interruptLock (fun () -> currentCancellation <- null)
            cancellation.Dispose()

    /// <summary>
    /// Queue an interaction and hand back the task the host is waiting on. A request that arrives
    /// once the session has stopped accepting work fails, rather than waiting for a turn that will
    /// never come.
    /// </summary>
    let queueInteraction (run: unit -> ExecutionResult) =
        let completion =
            TaskCompletionSource<ExecutionResult>(TaskCreationOptions.RunContinuationsAsynchronously)

        let queued =
            executionQueue.TryEnqueue(fun () ->
                try
                    completion.TrySetResult(run ()) |> ignore
                with e ->
                    completion.TrySetException e |> ignore)

        if not queued then
            completion.TrySetException(LocalRpcException("The F# Interactive session is shutting down", ErrorCode = -32001))
            |> ignore

        completion.Task

    /// Prefix the submitted text with a line directive so that diagnostics point back at the
    /// editor's own file and line rather than at the position within the submission.
    let positionInteraction (code: string) (sourcePath: string) (startLine: Nullable<int>) =
        if String.IsNullOrEmpty sourcePath || not startLine.HasValue then
            code
        else
            $"# %d{startLine.Value} @\"%s{sourcePath}\"\n%s{code}"

    /// Refuse anything that arrives before the handshake, so that a mis-sequenced host gets a clear
    /// answer rather than an obscure failure later on.
    let requireInitialized () =
        if not initialized then
            raise (LocalRpcException("'fsi/initialize' must be called first", ErrorCode = -32000))

    /// Watch the process that owns this session, so that an F# Interactive left behind by a
    /// crashed host does not survive as an orphan.
    let attachToClientProcess (clientProcessId: int) =
        try
            let client = Process.GetProcessById clientProcessId
            client.EnableRaisingEvents <- true
            client.Exited.Add(fun _ -> exit 0)

            // The host may already have gone by the time the handler was attached.
            if client.HasExited then
                exit 0
        with _ ->
            // An unknown process id is not fatal: the session simply loses orphan protection.
            ()

    [<JsonRpcMethod(Methods.Initialize, UseSingleObjectParameterDeserialization = true)>]
    member _.Initialize(request: InitializeRequest) : InitializeResult =
        if request.clientProcessId > 0 then
            attachToClientProcess request.clientProcessId

        initialized <- true

        {
            processId = Process.GetCurrentProcess().Id
            frameworkDescription = RuntimeInformation.FrameworkDescription
            processArchitecture = string RuntimeInformation.ProcessArchitecture
            fsiVersion =
                match typeof<FsiEvaluationSession>.Assembly.GetName().Version with
                | null -> ""
                | version -> string version
            workingDirectory = Directory.GetCurrentDirectory()
            supportsInterrupt = true
        }

    [<JsonRpcMethod(Methods.Execute, UseSingleObjectParameterDeserialization = true)>]
    member _.Execute(request: ExecuteRequest) : Task<ExecutionResult> =
        requireInitialized ()

        let text = positionInteraction request.code request.sourcePath request.startLine

        let scriptPath =
            if String.IsNullOrEmpty request.sourcePath then
                DefaultInteractionName
            else
                request.sourcePath

        queueInteraction (fun () -> runInteraction text scriptPath)

    [<JsonRpcMethod(Methods.ExecuteFile, UseSingleObjectParameterDeserialization = true)>]
    member _.ExecuteFile(request: ExecuteFileRequest) : Task<ExecutionResult> =
        requireInitialized ()

        // Routed through #load so that the file joins the session the same way it would from a
        // script, rather than being replayed as anonymous text.
        queueInteraction (fun () -> runInteraction $"#load @\"%s{request.path}\"" request.path)

    /// Apply the host's notion of where to look for sources and references, expressed as the
    /// directives a script would use.
    [<JsonRpcMethod(Methods.SetPaths, UseSingleObjectParameterDeserialization = true)>]
    member _.SetPaths(request: SetPathsRequest) : Task<ExecutionResult> =
        requireInitialized ()

        // The process directory moves on the queue, alongside the directive that moves the
        // compiler's: doing it as the request arrives would move it under an earlier interaction
        // that is still running.
        queueInteraction (fun () ->
            let directives = ResizeArray()

            if
                not (String.IsNullOrWhiteSpace request.workingDirectory)
                && Directory.Exists request.workingDirectory
            then
                // Two different notions of "current directory" have to agree here. The directive
                // moves the compiler's, which is what relative #load and #r resolve against; the
                // process one is what the running script sees when it opens a file by relative path.
                try
                    Directory.SetCurrentDirectory request.workingDirectory
                with _ ->
                    ()

                directives.Add $"#silentCd @\"%s{request.workingDirectory}\""

            match request.includePaths with
            | null -> ()
            | paths ->
                for path in paths do
                    if not (String.IsNullOrWhiteSpace path) then
                        directives.Add $"#I @\"%s{path}\""

            if directives.Count = 0 then
                toExecutionResult (Choice1Of2 None) [||] false
            else
                runInteraction (String.Join("\n", directives)) DefaultInteractionName)

    /// <summary>Interrupt the interaction in flight.</summary>
    /// <remarks>
    /// Served straight away rather than queued, which is the point: an interrupt that waited its
    /// turn behind the interaction it is meant to stop would never arrive.
    /// </remarks>
    [<JsonRpcMethod(Methods.Interrupt)>]
    member _.Interrupt() : InterruptResult =
        requireInitialized ()

        let cancellation = lock interruptLock (fun () -> currentCancellation)

        match cancellation with
        | null -> { interrupted = false }
        | cts ->
            // Cancel the token the interaction is running under, then ask the session to interrupt
            // the evaluation thread, which is what stops code already inside a long-running call.
            try
                cts.Cancel()
            with _ ->
                ()

            try
                fsiSession.Interrupt()
            with _ ->
                ()

            { interrupted = true }

    [<JsonRpcMethod(Methods.Shutdown)>]
    member _.Shutdown() : unit =
        requireInitialized ()
        shutdownRequested.TrySetResult() |> ignore

/// Wait for the host to connect, then serve requests until it disconnects or asks to shut down.
let private runServer
    (fsiSession: FsiEvaluationSession)
    (fsiConfig: FsiEvaluationSessionHostConfig)
    (pipeName: string)
    (outWriter: TextWriter)
    (errorWriter: TextWriter)
    =
    use pipe =
        new NamedPipeServerStream(
            pipeName,
            PipeDirection.InOut,
            maxNumberOfServerInstances = 1,
            transmissionMode = PipeTransmissionMode.Byte,
            options = PipeOptions.Asynchronous
        )

    pipe.WaitForConnection()

    let shutdownRequested =
        TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

    let executionQueue = ExecutionQueue()

    let target =
        FsiRpcTarget(fsiSession, fsiConfig, outWriter, errorWriter, shutdownRequested, executionQueue)

    use rpc =
        new JsonRpc(new HeaderDelimitedMessageHandler(pipe, new JsonMessageFormatter()))

    rpc.AddLocalRpcTarget(target, JsonRpcTargetOptions(NotifyClientOfEvents = false, AllowNonPublicInvocation = false))

    // Diagnostic breadcrumb: a host that gets "method not found" against a target that plainly
    // declares the method has almost certainly loaded a second, different copy of this library, so
    // its identity here is worth more than the rest of the trace.
    let streamJsonRpc = typeof<JsonRpc>.Assembly
    errorWriter.WriteLine $"FSI-SERVER: StreamJsonRpc %O{streamJsonRpc.GetName().Version} from %s{streamJsonRpc.Location}"

    errorWriter.Flush()
    rpc.StartListening()

    // Either the host goes away or it asks to stop. Both end the session.
    Task.WaitAny(rpc.Completion, shutdownRequested.Task) |> ignore

    if shutdownRequested.Task.IsCompleted then
        // Give the reply to the shutdown request its moment to reach the host before the process
        // disappears from under it.
        Task.Delay(250).Wait()

    executionQueue.Complete()

/// Start the server on a background thread and return, leaving the caller's thread free to drive
/// the event loop. Mirrors how a console session spawns its standard input reader.
let internal startOnBackgroundThread
    (fsiSession: FsiEvaluationSession)
    (fsiConfig: FsiEvaluationSessionHostConfig)
    (pipeName: string)
    (outWriter: TextWriter)
    (errorWriter: TextWriter)
    =
    let thread =
        Thread(
            (fun () ->
                try
                    runServer fsiSession fsiConfig pipeName outWriter errorWriter
                with e ->
                    errorWriter.WriteLine $"F# Interactive server terminated: %O{e}"
                    errorWriter.Flush()

                // The session exists only to serve this host. Once the connection is gone there is
                // nothing left to do, and lingering would leak a process.
                exit 0),
            Name = "FSI-JsonRpc-Dispatch",
            IsBackground = true
        )

    thread.Start()

/// <summary>
/// Recognise <c>--fsi-server-jsonrpc:&lt;pipe name&gt;</c> in a command line, returning the pipe name.
/// </summary>
let internal tryGetPipeName (argv: string[]) =
    argv
    |> Array.tryPick (fun arg ->
        if arg.StartsWith(JsonRpcServerOption, StringComparison.Ordinal) then
            let name = arg.Substring(JsonRpcServerOption.Length).Trim('"')

            if String.IsNullOrWhiteSpace name then None else Some name
        else
            None)
