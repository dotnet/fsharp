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

/// Where this build put its outputs: `<artifacts>/bin`, and the configuration and framework this
/// test assembly was built for, which fsi shares.
let buildOutput () =
    let baseDirectory =
        DirectoryInfo(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))

    struct {|
        BinDirectory = baseDirectory.Parent.Parent.Parent.FullName
        Configuration = baseDirectory.Parent.Name
        Framework = baseDirectory.Name
    |}

/// The fsi built by this repository: a sibling of the test output at
/// `<artifacts>/bin/fsi/<configuration>/<framework>`.
let fsiOutputDirectory () =
    let output = buildOutput ()
    Path.Combine(output.BinDirectory, "fsi", output.Configuration, output.Framework)

/// fsi is a managed dll run under the dotnet host — the same way `InteractiveHost.fs` launches it
/// for the window.
let private locateFsi (fsiDirectory: string) =
    let fsi = Path.Combine(fsiDirectory, "fsi.dll")

    if not (File.Exists fsi) then
        failwith $"Could not find the fsi under test at '{fsi}'. Build src/fsi first."

    locateDotnetHost (), [ fsi ]

/// A running session, plus everything needed to talk to it and to explain a failure.
///
/// `serverSwitches` spells the switch that turns the server on, for tests of the forms fsi accepts;
/// `fsiDirectory` points at an fsi other than the build's own, for tests of what ships;
/// `clientProcessId` names the process whose exit ends the session, this one by default.
[<Sealed>]
type FsiServerHarness
    (
        ?extraArguments: string list,
        ?workingDirectory: string,
        ?serverSwitches: string -> string list,
        ?fsiDirectory: string,
        ?clientProcessId: int
    ) =
    // On Unix the pipe is a socket under $TMPDIR, and macOS caps socket paths at 104 characters.
    let pipeName = $"fsi{Guid.NewGuid():N}".Substring(0, 15)
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
            $"\"{argument}\""
        else
            argument

    let startInfo =
        let fsiHost, leadingArguments =
            locateFsi (defaultArg fsiDirectory (fsiOutputDirectory ()))

        let serverSwitches =
            defaultArg serverSwitches (fun pipeName -> [ $"{CommandLine.ServerOption}{pipeName}" ])

        let arguments =
            [
                yield! leadingArguments
                "--nologo"
                yield! serverSwitches pipeName
                $"--fsi-server-client-pid:{defaultArg clientProcessId (Process.GetCurrentProcess().Id)}"
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
        // The server admits its own user only, and a client that says so too is turned away from
        // a pipe somebody else opened under the same name.
        let pipe =
            new NamedPipeClientStream(
                ".",
                pipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous ||| PipeOptions.CurrentUserOnly
            )

        try
            pipe.Connect 60_000
        with e ->
            let detail =
                if session.HasExited then
                    $"The session exited with code {session.ExitCode}."
                else
                    "The session is still running."

            let error = lock outputLock (fun () -> standardError.ToString())
            failwith $"Could not connect to the session on pipe '{pipeName}'. {detail}\n{e.Message}\n-- stderr --\n{error}"

        pipe

    let rpc =
        let rpc = new JsonRpc(new HeaderDelimitedMessageHandler(pipe, new JsonMessageFormatter()))
        rpc.StartListening()
        rpc

    /// On failure, fold in what the session itself printed: a protocol error alone rarely says what
    /// the session actually did.
    let await (work: Task<'T>) (timeout: TimeSpan) =
        try
            if not (work.Wait timeout) then
                failwith "The session did not answer in time."

            work.Result
        with e ->
            let output, error = lock outputLock (fun () -> standardOutput.ToString(), standardError.ToString())
            raise (Exception($"{e.Message}\n-- stdout --\n{output}-- stderr --\n{error}", e))

    member _.StandardOutput = lock outputLock (fun () -> standardOutput.ToString())

    member _.StandardError = lock outputLock (fun () -> standardError.ToString())

    member _.HasExited = session.HasExited

    member _.ExitCode = session.ExitCode

    member _.ProcessId = session.Id

    member _.WaitForExit(milliseconds: int) = session.WaitForExit milliseconds

    member _.CloseControlChannel(corrupt: bool) =
        if corrupt then
            let bytes = Encoding.ASCII.GetBytes "Content-Length: invalid\r\n\r\n"

            // The session hangs up on the malformed header, and the client's own JsonRpc, listening on
            // the same pipe, disposes it in turn, possibly before this write or flush is done.
            try
                pipe.Write(bytes, 0, bytes.Length)
                pipe.Flush()
            with
            | :? ObjectDisposedException
            | :? IOException -> ()
        else
            rpc.Dispose()

        pipe.Dispose()

    /// Wait until the session's own output contains the given text, which is how a test observes
    /// what a script printed rather than what the protocol returned.
    member this.WaitForOutput(text: string, ?timeout: TimeSpan) =
        let deadline = DateTime.UtcNow + defaultArg timeout (TimeSpan.FromSeconds 30.0)

        let rec wait () =
            if this.StandardOutput.Contains(text, StringComparison.Ordinal) then true
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
        // `await` folds diagnostic text into a wrapping exception on any failure (see above), so
        // the type this classifies on is found by descending through causes, not just one level.
        let rec classify (e: exn) =
            match e with
            | :? RemoteInvocationException as remote -> Some remote.ErrorCode
            | :? RemoteMethodNotFoundException as remote -> Some(int remote.ErrorCode)
            | _ ->
                match e.InnerException with
                | null -> None
                | inner -> classify inner

        try
            this.Request<ExecutionResult>(method, parameters) |> ignore
            None
        with e ->
            match classify e with
            | Some code -> Some code
            | None -> raise e

    /// Perform the handshake every host makes before submitting anything.
    member this.Initialize() =
        this.Request<InitializeResult> Methods.Initialize

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
    diagnostics result |> Array.filter (fun d -> String.Equals(d.severity, "error", StringComparison.Ordinal))

let warnings result =
    diagnostics result |> Array.filter (fun d -> String.Equals(d.severity, "warning", StringComparison.Ordinal))

let succeeded (result: ExecutionResult) = result.success

let exceptionMessage (result: ExecutionResult) =
    match box result.``exception`` with
    | null -> None
    | _ -> Some result.``exception``.message

/// Render a result for a failure message.
let describeResult (result: ExecutionResult) =
    let diagnosticText =
        diagnostics result
        |> Array.map (fun d ->
            $"{d.fileName}({d.startLine},{d.startColumn}): {d.severity} FS{d.errorNumber:D4}: {d.message}")
        |> String.concat "\n    "

    let exceptionText =
        match exceptionMessage result with
        | Some message -> message
        | None -> "<none>"

    let outcome =
        $"success={result.success} cancelled={result.cancelled} workingDirectory={result.workingDirectory}"

    $"{outcome} exception={exceptionText}\n    {diagnosticText}"
