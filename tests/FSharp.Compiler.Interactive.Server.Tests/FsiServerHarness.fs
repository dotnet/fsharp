// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Drives a real F# Interactive process in its JSON-RPC server mode, the way an editor would.
///
/// The tests exercise the shipped protocol end to end rather than an in-process stand-in, because
/// the parts most likely to break are the ones that only exist across a process boundary: the
/// handshake, the lifetime of the session, and the interaction between the control channel and the
/// output streams. The client here is StreamJsonRpc, the same library the window uses.
module FSharp.Compiler.Interactive.Server.Tests.FsiServerHarness

open System
open System.Diagnostics
open System.IO
open System.IO.Pipes
open System.Runtime.InteropServices
open System.Text
open System.Threading
open System.Threading.Tasks

open StreamJsonRpc

open FSharp.Compiler.Interactive.Protocol

/// How long to wait for the session to answer a request. Generous, because the first interaction
/// of a session pays for the type checker warming up.
let private defaultTimeout = TimeSpan.FromSeconds 120.0

/// Prefer the .NET host this repository provisions, so that the session runs on the same runtime
/// as the rest of the build.
let private locateDotnetHost () =
    let executable =
        if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
            "dotnet.exe"
        else
            "dotnet"

    let rec search (directory: DirectoryInfo) =
        match directory with
        | null -> executable
        | directory ->
            let candidate = Path.Combine(directory.FullName, ".dotnet", executable)

            if File.Exists candidate then
                candidate
            else
                search directory.Parent

    search (DirectoryInfo(AppContext.BaseDirectory))

/// Locate the fsi built by this repository, alongside the test assembly's own output, and how to
/// launch it.
///
/// Test output lives at `<artifacts>/bin/<project>/<configuration>/<framework>`, and fsi is its
/// sibling at `<artifacts>/bin/fsi/<configuration>/<framework>`. net472's fsi is a native
/// executable that runs directly; every other framework's is a managed dll run under the dotnet
/// host — the same split `InteractiveHost.fs` makes for the window.
let private locateFsi () =
    let baseDirectory =
        DirectoryInfo(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))

    let framework = baseDirectory.Name
    let configuration = baseDirectory.Parent.Name
    let binDirectory = baseDirectory.Parent.Parent.Parent
    let fsiDirectory = Path.Combine(binDirectory.FullName, "fsi", configuration, framework)

    if framework = "net472" then
        let fsi = Path.Combine(fsiDirectory, "fsi.exe")

        if not (File.Exists fsi) then
            failwithf "Could not find the fsi under test at '%s'. Build src/fsi first." fsi

        fsi, []
    else
        let fsi = Path.Combine(fsiDirectory, "fsi.dll")

        if not (File.Exists fsi) then
            failwithf "Could not find the fsi under test at '%s'. Build src/fsi first." fsi

        locateDotnetHost (), [ fsi ]

/// A running session, plus everything needed to talk to it and to explain a failure.
[<Sealed>]
type FsiServerHarness(?extraArguments: string list, ?workingDirectory: string) =
    let pipeName = "FsiServerTests_" + Guid.NewGuid().ToString("N")
    let standardOutput = StringBuilder()
    let standardError = StringBuilder()
    let outputLock = obj ()

    /// .NET Framework has no `ProcessStartInfo.ArgumentList`, so the command line is built by hand
    /// on every target — one fewer thing that differs between fsi's two hosting flavors.
    let quoteIfNeeded (argument: string) =
        if
            argument.IndexOf(" ", StringComparison.Ordinal) >= 0
            && not (argument.StartsWith("\"", StringComparison.Ordinal))
        then
            "\"" + argument + "\""
        else
            argument

    let startInfo =
        let fsiHost, leadingArguments = locateFsi ()

        let arguments =
            [
                yield! leadingArguments
                "--nologo"
                "--fsi-server-jsonrpc:" + pipeName
                yield! defaultArg extraArguments []
            ]

        ProcessStartInfo(
            FileName = fsiHost,
            Arguments = String.Join(" ", arguments |> List.map quoteIfNeeded),
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            WorkingDirectory = defaultArg workingDirectory (Path.GetTempPath())
        )

    let session = new Process(StartInfo = startInfo)

    do
        session.OutputDataReceived.Add(fun e ->
            match e.Data with
            | null -> ()
            | line -> lock outputLock (fun () -> standardOutput.AppendLine line |> ignore))

        session.ErrorDataReceived.Add(fun e ->
            match e.Data with
            | null -> ()
            | line -> lock outputLock (fun () -> standardError.AppendLine line |> ignore))

        session.Start() |> ignore
        session.BeginOutputReadLine()
        session.BeginErrorReadLine()

    let pipe =
        let pipe =
            new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous)

        try
            pipe.Connect 60_000
        with e ->
            let detail =
                if session.HasExited then
                    sprintf "The session exited with code %d." session.ExitCode
                else
                    "The session is still running."

            failwithf "Could not connect to the session on pipe '%s'. %s\n%s" pipeName detail e.Message

        pipe

    let rpc =
        let rpc = new JsonRpc(new HeaderDelimitedMessageHandler(pipe, new JsonMessageFormatter()))
        rpc.StartListening()
        rpc

    let await (work: Task<'T>) (timeout: TimeSpan) =
        if not (work.Wait timeout) then
            failwith "The session did not answer in time."

        work.Result

    member _.StandardOutput = lock outputLock (fun () -> standardOutput.ToString())

    member _.StandardError = lock outputLock (fun () -> standardError.ToString())

    member _.HasExited = session.HasExited

    member _.ProcessId = session.Id

    member _.WaitForExit(milliseconds: int) = session.WaitForExit milliseconds

    /// Wait until the session's own output contains the given text, which is how a test observes
    /// what a script printed rather than what the protocol returned.
    member this.WaitForOutput(text: string, ?timeout: TimeSpan) =
        let deadline = DateTime.UtcNow + defaultArg timeout (TimeSpan.FromSeconds 30.0)

        let rec wait () =
            if this.StandardOutput.Contains text then true
            elif DateTime.UtcNow > deadline then false
            else
                Thread.Sleep 50
                wait ()

        wait ()

    /// Send a request whose parameters are a single object, as every method of this protocol but
    /// the argument-less ones expects.
    member _.BeginRequest<'T>(method: string, parameters: obj) : Task<'T> =
        rpc.InvokeWithParameterObjectAsync<'T>(method, parameters)

    member _.BeginRequest<'T>(method: string) : Task<'T> = rpc.InvokeAsync<'T>(method)

    member _.EndRequest(work: Task<'T>, ?timeout: TimeSpan) =
        await work (defaultArg timeout defaultTimeout)

    member this.Request<'T>(method: string, parameters: obj, ?timeout: TimeSpan) : 'T =
        await (this.BeginRequest<'T>(method, parameters)) (defaultArg timeout defaultTimeout)

    member this.Request<'T>(method: string, ?timeout: TimeSpan) : 'T =
        await (this.BeginRequest<'T> method) (defaultArg timeout defaultTimeout)

    /// Issue a request expected to fail, returning the JSON-RPC error code the session reported.
    ///
    /// An unknown method surfaces as its own exception type rather than as a reported error, so it
    /// is mapped back to the code the specification gives it.
    member this.RequestExpectingError(method: string, parameters: obj) =
        let classify (e: exn) =
            match e with
            | :? RemoteMethodNotFoundException -> Some -32601
            | :? RemoteInvocationException as remote -> Some remote.ErrorCode
            | _ -> None

        try
            this.Request<ExecutionResult>(method, parameters) |> ignore
            None
        with e ->
            let reported =
                match e with
                | :? AggregateException as aggregate -> classify aggregate.InnerException
                | e -> classify e

            match reported with
            | Some code -> Some code
            | None -> raise e

    /// Perform the handshake every host makes before submitting anything.
    member this.Initialize(?clientProcessId: int) =
        let clientProcessId =
            defaultArg clientProcessId (Process.GetCurrentProcess().Id)

        this.Request<InitializeResult>(Methods.Initialize, { clientProcessId = clientProcessId })

    static member ExecuteParams(code: string, ?sourcePath: string, ?startLine: int) : ExecuteRequest =
        {
            code = code
            sourcePath = Option.toObj sourcePath
            startLine =
                match startLine with
                | Some line -> Nullable line
                | None -> Nullable()
        }

    /// Submit one interaction and return the structured result.
    member this.Execute(code: string, ?sourcePath: string, ?startLine: int, ?timeout: TimeSpan) =
        this.Request<ExecutionResult>(
            Methods.Execute,
            FsiServerHarness.ExecuteParams(code, ?sourcePath = sourcePath, ?startLine = startLine),
            ?timeout = timeout
        )

    interface IDisposable with
        member _.Dispose() =
            try
                rpc.Dispose()
            with _ ->
                ()

            try
                pipe.Dispose()
            with _ ->
                ()

            try
                if not session.HasExited then
                    session.Kill()

                session.WaitForExit 10_000 |> ignore
            with _ ->
                ()

            session.Dispose()

//-------------------------------------------------------------------------
// Reading the pieces of a result
//-------------------------------------------------------------------------

let diagnostics (result: ExecutionResult) =
    match result.diagnostics with
    | null -> [||]
    | items -> items

let errors result =
    diagnostics result |> Array.filter (fun d -> d.severity = "error")

let warnings result =
    diagnostics result |> Array.filter (fun d -> d.severity = "warning")

let succeeded (result: ExecutionResult) = result.success

let exceptionMessage (result: ExecutionResult) =
    match box result.``exception`` with
    | null -> None
    | _ -> Some result.``exception``.message

/// Render a result for a failure message.
let describeResult (result: ExecutionResult) =
    let diagnosticText =
        diagnostics result
        |> Array.map (fun d -> sprintf "%s(%d,%d): %s FS%04d: %s" d.fileName d.startLine d.startColumn d.severity d.errorNumber d.message)
        |> String.concat "\n    "

    sprintf
        "success=%b cancelled=%b workingDirectory=%s exception=%s\n    %s"
        result.success
        result.cancelled
        result.workingDirectory
        (match exceptionMessage result with
         | Some m -> m
         | None -> "<none>")
        diagnosticText
