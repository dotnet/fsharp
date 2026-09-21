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
/// </remarks>
module FSharp.Compiler.Interactive.Server

open System
open System.Collections.Concurrent
open System.Diagnostics
open System.IO
open System.IO.Pipes
open System.Reflection
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Newtonsoft.Json
open Newtonsoft.Json.Linq
open StreamJsonRpc

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Interactive.Protocol
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.Symbols

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

/// A path spliced into a directive as a verbatim string literal, in which only a quote needs escaping.
let private verbatimString (text: string) =
    "@\"" + text.Replace("\"", "\"\"") + "\""

/// Watch the process that owns this session, so that an F# Interactive left behind by a crashed
/// host does not survive as an orphan.
let private watchClientProcess (clientProcessId: int) =
    let client = Process.GetProcessById clientProcessId
    client.EnableRaisingEvents <- true
    client.Exited.Add(fun _ -> exit 0)

    // The host may already have gone by the time the handler was attached.
    if client.HasExited then
        exit 0

let private invalidParams message =
    LocalRpcException(message, ErrorCode = -32602)

[<Sealed>]
type private StrictStringJsonConverter() =
    inherit JsonConverter()

    override _.CanConvert(objectType) = objectType = typeof<string>
    override _.CanWrite = false

    override _.ReadJson(reader, _, _, _) =
        match reader.TokenType with
        | JsonToken.String -> reader.Value
        | JsonToken.Null -> null
        | token -> raise (invalidParams $"Expected a string or null, but found {token}.")

    override _.WriteJson(_, _, _) = raise (NotSupportedException())

[<Sealed>]
type private StrictStringArrayJsonConverter() =
    inherit JsonConverter()

    override _.CanConvert(objectType) = objectType = typeof<string[]>
    override _.CanWrite = false

    override _.ReadJson(reader, _, _, _) =
        let token = JToken.ReadFrom reader

        match token.Type with
        | JTokenType.Null -> null
        | JTokenType.Array ->
            token.Children()
            |> Seq.map (fun item ->
                match item.Type with
                | JTokenType.String -> item.Value<string>()
                | JTokenType.Null -> null
                | itemType -> raise (invalidParams $"Expected a string or null, but found {itemType}."))
            |> Seq.toArray
            |> box
        | tokenType -> raise (invalidParams $"Expected an array or null, but found {tokenType}.")

    override _.WriteJson(_, _, _) = raise (NotSupportedException())

[<Sealed>]
type private StrictNullableInt32JsonConverter() =
    inherit JsonConverter()

    override _.CanConvert(objectType) = objectType = typeof<Nullable<int>>
    override _.CanWrite = false

    override _.ReadJson(reader, _, _, _) =
        match reader.TokenType with
        | JsonToken.Null -> null
        | JsonToken.Integer ->
            try
                box (Nullable(Convert.ToInt32 reader.Value))
            with :? OverflowException ->
                raise (invalidParams "The integer is outside the supported range.")
        | token -> raise (invalidParams $"Expected an integer or null, but found {token}.")

    override _.WriteJson(_, _, _) = raise (NotSupportedException())

let private requireRequest methodName request =
    if obj.ReferenceEquals(request, null) then
        raise (invalidParams $"'{methodName}' requires a request object.")

let private requireText fieldName (value: string) =
    if isNull value then
        raise (invalidParams $"'{fieldName}' is required.")

let private requireDirectivePath fieldName (value: string) =
    requireText fieldName value

    if value.IndexOf('"') >= 0 || value.IndexOf('\r') >= 0 || value.IndexOf('\n') >= 0 then
        raise (invalidParams $"'{fieldName}' contains characters that F# Interactive directives cannot represent.")

let private toExecutionResult
    (outcome: Choice<FsiValue option, exn>)
    (diagnostics: FSharpDiagnostic[])
    (values: ValueInfo[])
    (cancelled: bool)
    =
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
        values = values
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
type internal ExecutionQueue(ready: WaitHandle) =
    let queue = new BlockingCollection<unit -> unit>()

    let worker =
        Thread(
            (fun () ->
                // Requests are accepted from the moment the host connects but run only once the session
                // has finished its startup scripts: their bindings would otherwise be reported as the
                // first request's, and one posted to an event loop a script then replaces is never run.
                ready.WaitOne() |> ignore

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
/// The server loop registers the six handlers one by one, so this type stays internal and nothing
/// beyond the protocol is callable from the wire.
/// </para>
/// </remarks>
[<Sealed>]
type internal FsiRpcTarget
    internal
    (
        fsiSession: FsiEvaluationSession,
        fsiConfig: FsiEvaluationSessionHostConfig,
        outWriter: TextWriter,
        errorWriter: TextWriter,
        shutdownRequested: TaskCompletionSource<unit>,
        executionQueue: ExecutionQueue
    ) =

    let interactionLock = obj ()
    let mutable currentInteraction = ValueNone
    let mutable initialized = false
    let mutable currentValues: ResizeArray<ValueInfo> = null

    /// Formatted by the session's own printer, so that the text matches the console's and obeys the
    /// session's print settings. Formatting runs user code — a <c>ToString</c> override, a lazy
    /// value — so a failure there becomes the value's text rather than a failed interaction.
    let toValueInfo (name: string) (value: FsiValue) =
        {
            name = name
            typeName =
                match value.ReflectionType with
                | null -> ""
                | reflectionType -> reflectionType.FullName
            value =
                try
                    fsiSession.FormatValue(value.ReflectionValue, value.ReflectionType)
                with e ->
                    $"<{e.GetType().Name}: {e.Message}>"
        }

    /// The console prints what the user bound, not the helper bindings the compiler introduces
    /// around it, such as the `patternInput` of `let a, b = …`.
    let isUserBinding (evaluation: EvaluationEventArgs) =
        match evaluation.Symbol with
        | :? FSharpMemberOrFunctionOrValue as value -> not value.IsCompilerGenerated
        | _ -> true

    do
        fsiConfig.OnEvaluation.Add(fun evaluation ->
            match evaluation.FsiValue with
            | Some value when isUserBinding evaluation -> values.Add(toValueInfo evaluation.Name value)
            | _ -> ())

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

    /// Flush buffered console writers before answering. The output and RPC streams are independent,
    /// so their reader-visible ordering is deliberately unspecified.
    let flushConsole () =
        try
            outWriter.Flush()
            errorWriter.Flush()
        with _ ->
            ()

    let runInteraction (code: string) (scriptPath: string) =
        let cancellation = new CancellationTokenSource()
        let values = ResizeArray<ValueInfo>()

        lock interactionLock (fun () -> currentInteraction <- ValueSome(cancellation, false))

        try
            lock interactionLock (fun () -> currentValues <- values)

            let outcome, diagnostics =
                evaluateOnEventLoop (fun () -> fsiSession.EvalInteractionNonThrowing(code, scriptPath, cancellation.Token))

            flushConsole ()
            toExecutionResult outcome diagnostics (values.ToArray()) cancellation.IsCancellationRequested
        finally
            lock interactionLock (fun () ->
                currentValues <- null
                currentInteraction <- ValueNone)

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
            $"# {startLine.Value} {verbatimString sourcePath}\n{code}"

    /// Refuse anything that arrives before the handshake, so that a mis-sequenced host gets a clear
    /// answer rather than an obscure failure later on.
    let requireInitialized () =
        if not initialized then
            raise (LocalRpcException("'fsi/initialize' must be called first", ErrorCode = -32000))

    member _.Initialize() : InitializeResult =
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

    member _.Execute(request: ExecuteRequest) : Task<ExecutionResult> =
        requireInitialized ()
        requireRequest Methods.Execute request
        requireText "code" request.code

        if request.startLine.HasValue && request.startLine.Value < 1 then
            raise (invalidParams "'startLine' must be at least one.")

        let text = positionInteraction request.code request.sourcePath request.startLine

        let scriptPath =
            if String.IsNullOrEmpty request.sourcePath then
                DefaultInteractionName
            else
                request.sourcePath

        queueInteraction (fun () -> runInteraction text scriptPath)

    member _.ExecuteFile(request: ExecuteFileRequest) : Task<ExecutionResult> =
        requireInitialized ()
        requireRequest Methods.ExecuteFile request
        requireText "path" request.path

        if String.IsNullOrWhiteSpace request.path then
            raise (invalidParams "'path' must not be empty.")

        // Routed through #load so that the file joins the session the same way it would from a
        // script, rather than being replayed as anonymous text.
        queueInteraction (fun () -> runInteraction $"#load {verbatimString request.path}" request.path)

    /// Apply the host's notion of where to look for sources and references, expressed as the
    /// directives a script would use.
    member _.SetPaths(request: SetPathsRequest) : Task<ExecutionResult> =
        requireInitialized ()
        requireRequest Methods.SetPaths request
        requireText "workingDirectory" request.workingDirectory

        match request.includePaths with
        | null -> raise (invalidParams "'includePaths' is required.")
        | paths ->
            paths
            |> Array.iteri (fun index path -> requireDirectivePath $"includePaths[{index}]" path)

        if not (String.IsNullOrWhiteSpace request.workingDirectory) then
            requireDirectivePath "workingDirectory" request.workingDirectory

        if
            not (String.IsNullOrWhiteSpace request.workingDirectory)
            && not (Directory.Exists request.workingDirectory)
        then
            raise (LocalRpcException($"The working directory '{request.workingDirectory}' does not exist.", ErrorCode = -32002))

        queueInteraction (fun () ->
            let directives = ResizeArray()

            if not (String.IsNullOrWhiteSpace request.workingDirectory) then
                directives.Add $"#silentCd {verbatimString request.workingDirectory}"

            for path in request.includePaths do
                if not (String.IsNullOrWhiteSpace path) then
                    directives.Add $"#I {verbatimString path}"

            if directives.Count = 0 then
                toExecutionResult (Choice1Of2 None) [||] [||] false
            else
                let previousDirectory = Directory.GetCurrentDirectory()
                let result = runInteraction (String.Join("\n", directives)) DefaultInteractionName

                if result.success && not (String.IsNullOrWhiteSpace request.workingDirectory) then
                    try
                        Directory.SetCurrentDirectory request.workingDirectory

                        { result with
                            workingDirectory = Directory.GetCurrentDirectory()
                        }
                    with e ->
                        runInteraction $"#silentCd {verbatimString previousDirectory}" DefaultInteractionName
                        |> ignore

                        toExecutionResult (Choice2Of2 e) [||] [||] false
                else
                    result)

    /// <summary>Interrupt the interaction in flight.</summary>
    /// <remarks>
    /// Served straight away rather than queued, which is the point: an interrupt that waited its
    /// turn behind the interaction it is meant to stop would never arrive.
    /// </remarks>
    member _.Interrupt() : InterruptResult =
        requireInitialized ()

        lock interactionLock (fun () ->
            match currentInteraction with
            | ValueNone
            | ValueSome(_, true) -> { interrupted = false }
            | ValueSome(cancellation, false) ->
                currentInteraction <- ValueSome(cancellation, true)

                try
                    cancellation.Cancel()
                with _ ->
                    ()

                try
                    fsiSession.Interrupt()
                with _ ->
                    ()

                { interrupted = true })

    member _.Shutdown() : unit =
        requireInitialized ()
        shutdownRequested.TrySetResult() |> ignore

/// Wait for the host to connect, then serve requests until it disconnects or asks to shut down.
let private runServer
    (fsiSession: FsiEvaluationSession)
    (fsiConfig: FsiEvaluationSessionHostConfig)
    (pipeName: string)
    (clientProcessId: int option)
    (eventLoopStarted: WaitHandle)
    (outWriter: TextWriter)
    (errorWriter: TextWriter)
    =
    // Watched before the host has connected: a host that dies while starting up must not leave a
    // session waiting on the pipe forever.
    clientProcessId |> Option.iter watchClientProcess

    // Any local process could otherwise open the pipe, and whoever connects first runs code as this
    // user. CurrentUserOnly limits the pipe's access list to the current user and rejects a client
    // running as anyone else.
    use pipe =
        new NamedPipeServerStream(
            pipeName,
            PipeDirection.InOut,
            maxNumberOfServerInstances = 1,
            transmissionMode = PipeTransmissionMode.Byte,
            options = (PipeOptions.Asynchronous ||| PipeOptions.CurrentUserOnly)
        )

    pipe.WaitForConnection()

    let shutdownRequested =
        TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

    let executionQueue = ExecutionQueue eventLoopStarted

    let target =
        FsiRpcTarget(fsiSession, fsiConfig, outWriter, errorWriter, shutdownRequested, executionQueue)

    let formatter = new JsonMessageFormatter()
    formatter.JsonSerializer.Converters.Add(new StrictStringJsonConverter())
    formatter.JsonSerializer.Converters.Add(new StrictStringArrayJsonConverter())
    formatter.JsonSerializer.Converters.Add(new StrictNullableInt32JsonConverter())

    use rpc = new JsonRpc(new HeaderDelimitedMessageHandler(pipe, formatter))

    // Registered one by one rather than by reflecting over the target, so that exactly the
    // protocol's methods are callable. A request carrying its parameters as one object — the shape
    // the protocol documents — lands in the handler's single parameter.
    let register (rpcMethod: string) (takesRequestObject: bool) (handlerName: string) =
        let handler =
            typeof<FsiRpcTarget>.GetMethod(handlerName, BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic)

        rpc.AddLocalRpcMethod(
            handler,
            target,
            JsonRpcMethodAttribute(rpcMethod, UseSingleObjectParameterDeserialization = takesRequestObject)
        )

    register Methods.Initialize false (nameof target.Initialize)
    register Methods.Execute true (nameof target.Execute)
    register Methods.ExecuteFile true (nameof target.ExecuteFile)
    register Methods.SetPaths true (nameof target.SetPaths)
    register Methods.Interrupt false (nameof target.Interrupt)
    register Methods.Shutdown false (nameof target.Shutdown)

    rpc.StartListening()

    // Either the host goes away or it asks to stop. A faulted transport is not an orderly
    // disconnect: observe it so the process reports failure instead of a successful shutdown.
    let completed = Task.WaitAny(rpc.Completion, shutdownRequested.Task)

    if completed = 0 then
        rpc.Completion.GetAwaiter().GetResult()

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
    (clientProcessId: int option)
    (eventLoopStarted: WaitHandle)
    (outWriter: TextWriter)
    (errorWriter: TextWriter)
    =
    let thread =
        Thread(
            (fun () ->
                try
                    runServer fsiSession fsiConfig pipeName clientProcessId eventLoopStarted outWriter errorWriter
                    exit 0
                with e ->
                    errorWriter.WriteLine $"F# Interactive server terminated: {e}"
                    errorWriter.Flush()
                    exit 1),
            Name = "FSI-JsonRpc-Dispatch",
            IsBackground = true
        )

    thread.Start()
