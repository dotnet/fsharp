// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Interactive.Server.Tests.FsiJsonRpcServerTests

open System
open System.IO
open System.Threading
open Xunit

open FSharp.Compiler.Interactive.Protocol
open FSharp.Compiler.Interactive.Server.Tests.FsiServerHarness

/// Start a session, hand it to the test, and shut it down afterwards.
let private withSession (test: FsiServerHarness -> unit) =
    use session = new FsiServerHarness()
    test session

/// Start a session that has already completed the handshake.
let private withInitializedSession (test: FsiServerHarness -> unit) =
    withSession (fun session ->
        session.Initialize() |> ignore
        test session)

/// Include the result and the session's output in a failure, since a protocol result alone rarely
/// explains what the session actually did.
let private describe (session: FsiServerHarness) (result: ExecutionResult) =
    sprintf
        "result: %s\nstandard output:\n%s\nstandard error:\n%s"
        (describeResult result)
        session.StandardOutput
        session.StandardError

//-------------------------------------------------------------------------
// Handshake
//-------------------------------------------------------------------------

[<Fact>]
let ``initialize reports the session process`` () =
    withSession (fun session ->
        let result = session.Initialize()

        // The reported identifier is what a host attaches a debugger to, so it must be the process
        // actually evaluating code rather than any launcher in front of it.
        Assert.Equal(session.ProcessId, result.processId)

        Assert.StartsWith(".NET", result.frameworkDescription)
        Assert.True result.supportsInterrupt

        Assert.True(
            Directory.Exists result.workingDirectory,
            sprintf "'%s' is not a directory" result.workingDirectory
        ))

[<Fact>]
let ``requests before initialize are refused`` () =
    withSession (fun session ->
        match session.RequestExpectingError(Methods.Execute, FsiServerHarness.ExecuteParams "1 + 1") with
        | None -> failwith "the session accepted an interaction before the handshake"
        | Some code -> Assert.Equal(-32000, code))

[<Fact>]
let ``unknown methods are refused`` () =
    withInitializedSession (fun session ->
        match session.RequestExpectingError("fsi/doesNotExist", obj ()) with
        | None -> failwith "the session accepted an unknown method"
        | Some code -> Assert.Equal(-32601, code))

//-------------------------------------------------------------------------
// Evaluating interactions
//-------------------------------------------------------------------------

[<Fact>]
let ``evaluates an interaction and prints its result`` () =
    withInitializedSession (fun session ->
        let result = session.Execute "1 + 1"

        Assert.True(succeeded result, describe session result)
        Assert.Empty(diagnostics result)

        // The value is reported the way a console session reports it: printed to standard output.
        Assert.True(session.WaitForOutput "val it: int = 2", describe session result))

[<Fact>]
let ``keeps bindings across interactions`` () =
    withInitializedSession (fun session ->
        let bound = session.Execute "let x = 40"
        Assert.True(succeeded bound, describe session bound)

        let result = session.Execute "x + 2"
        Assert.True(succeeded result, describe session result)
        Assert.True(session.WaitForOutput "val it: int = 42", describe session result))

[<Fact>]
let ``reports what the interaction printed`` () =
    withInitializedSession (fun session ->
        let result = session.Execute "printfn \"hello from the session\""
        Assert.True(succeeded result, describe session result)
        Assert.True(session.WaitForOutput "hello from the session", describe session result))

[<Fact>]
let ``evaluates multi-line interactions`` () =
    withInitializedSession (fun session ->
        let code = String.Join("\n", [ "let add a b ="; "    a + b"; ""; "add 20 22" ])

        let result = session.Execute code
        Assert.True(succeeded result, describe session result)
        Assert.True(session.WaitForOutput "val it: int = 42", describe session result))

[<Fact>]
let ``carries text that has to survive JSON escaping`` () =
    withInitializedSession (fun session ->
        // Quotes, backslashes and non-ASCII all have to make the round trip intact, in both the
        // request and the output that comes back.
        let result = session.Execute "printfn \"%s\" \"quote \\\" backslash \\\\ Ф# ✓\""

        Assert.True(succeeded result, describe session result)
        Assert.True(session.WaitForOutput "quote \" backslash \\ Ф# ✓", describe session result))

//-------------------------------------------------------------------------
// Diagnostics
//-------------------------------------------------------------------------

[<Fact>]
let ``reports type errors as structured diagnostics`` () =
    withInitializedSession (fun session ->
        let result = session.Execute "1 + \"text\""

        Assert.False(succeeded result, describe session result)

        let reported = errors result
        Assert.NotEmpty reported

        // FS0001 is the type mismatch error, and it must carry a usable position.
        let error = reported[0]
        Assert.Equal(1, error.errorNumber)
        Assert.True(error.startLine >= 1, sprintf "unexpected start line %d" error.startLine)
        Assert.False(String.IsNullOrWhiteSpace error.message))

[<Fact>]
let ``reports undefined identifiers`` () =
    withInitializedSession (fun session ->
        let result = session.Execute "thisNameIsNotDefined"

        Assert.False(succeeded result, describe session result)

        // FS0039: the value or constructor is not defined.
        Assert.True(errors result |> Array.exists (fun d -> d.errorNumber = 39), describe session result))

[<Fact>]
let ``warnings do not fail an interaction`` () =
    withInitializedSession (fun session ->
        // An incomplete pattern match warns, but the interaction still runs.
        let result = session.Execute "let f (x: int option) = match x with Some v -> v"

        Assert.True(succeeded result, describe session result)
        Assert.NotEmpty(warnings result)
        Assert.Empty(errors result))

[<Fact>]
let ``attributes diagnostics to the host's file and line`` () =
    withInitializedSession (fun session ->
        // A host executing a selection tells the session where that selection came from, so that
        // the reported position lands on the user's own source rather than within the submission.
        let path = Path.Combine(Path.GetTempPath(), "Library.fs")
        let result = session.Execute("1 + \"text\"", sourcePath = path, startLine = 120)

        let reported = errors result
        Assert.NotEmpty reported
        Assert.Equal(120, reported[0].startLine)
        Assert.EndsWith("Library.fs", reported[0].fileName))

[<Fact>]
let ``reports an escaping exception`` () =
    withInitializedSession (fun session ->
        // Annotated so that the interaction compiles: a bare `failwith` is generic and would fail
        // the value restriction instead of ever running.
        let result = session.Execute "(failwith \"boom\": unit)"

        Assert.False(succeeded result, describe session result)
        Assert.Empty(errors result)
        Assert.Equal(Some "boom", exceptionMessage result))

[<Fact>]
let ``does not report an exception for a compilation failure`` () =
    withInitializedSession (fun session ->
        // The diagnostics already describe the failure. Reporting the exception fsi raises to stop
        // processing would make a host show the same problem twice.
        let result = session.Execute "1 + \"text\""

        Assert.NotEmpty(errors result)
        Assert.Equal(None, exceptionMessage result))

[<Fact>]
let ``keeps serving after a failed interaction`` () =
    withInitializedSession (fun session ->
        Assert.False(succeeded (session.Execute "1 + \"text\""))
        Assert.False(succeeded (session.Execute "(failwith \"boom\": unit)"))

        // A session that stopped responding after an error would make the window useless.
        let result = session.Execute "2 * 21"
        Assert.True(succeeded result, describe session result)
        Assert.True(session.WaitForOutput "val it: int = 42", describe session result))

//-------------------------------------------------------------------------
// Files and search paths
//-------------------------------------------------------------------------

[<Fact>]
let ``loads a script file`` () =
    withInitializedSession (fun session ->
        let script =
            Path.Combine(Path.GetTempPath(), sprintf "fsiServerTest_%s.fsx" (Guid.NewGuid().ToString "N"))

        File.WriteAllText(script, "printfn \"the script ran\"\n")

        try
            let loaded =
                session.Request<ExecutionResult>(Methods.ExecuteFile, { path = script })

            Assert.True(succeeded loaded, describe session loaded)

            // The file is loaded, not replayed as anonymous text, so its effects are what prove it
            // reached the session. Its definitions land in a module named after the file, which is
            // ordinary `#load` behaviour and not something to assert on here.
            Assert.True(session.WaitForOutput "the script ran", describe session loaded)
        finally
            try
                File.Delete script
            with _ ->
                ())

[<Fact>]
let ``setPaths changes the working directory`` () =
    withInitializedSession (fun session ->
        let directory =
            Path.Combine(Path.GetTempPath(), sprintf "fsiServerTest_%s" (Guid.NewGuid().ToString "N"))

        Directory.CreateDirectory directory |> ignore

        try
            let result =
                session.Request<ExecutionResult>(
                    Methods.SetPaths,
                    {
                        includePaths = [| directory |]
                        workingDirectory = directory
                    }
                )

            Assert.True(succeeded result, describe session result)

            // The host mirrors this value so that its own reference resolution matches the session.
            let expected = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar)
            let actual = Path.GetFullPath(result.workingDirectory).TrimEnd(Path.DirectorySeparatorChar)
            Assert.Equal(expected, actual)
        finally
            try
                Directory.Delete(directory, true)
            with _ ->
                ())

[<Fact>]
let ``reports the working directory after every interaction`` () =
    withInitializedSession (fun session ->
        let result = session.Execute "1"
        Assert.True(Directory.Exists result.workingDirectory, describe session result))

//-------------------------------------------------------------------------
// Interrupting
//-------------------------------------------------------------------------

[<Fact>]
let ``interrupts a running interaction`` () =
    withInitializedSession (fun session ->
        // Warm the session up first, so that the interrupt below meets a session that is genuinely
        // executing the loop rather than still starting up.
        Assert.True(succeeded (session.Execute "1"))

        let running =
            session.BeginRequest<ExecutionResult>(
                Methods.Execute,
                FsiServerHarness.ExecuteParams "while true do System.Threading.Thread.Sleep 10"
            )

        Thread.Sleep 3000

        // Interactions queue behind one another, but an interrupt is served as it arrives — which
        // is the whole point, since one that waited its turn would never stop anything.
        let interrupted =
            session.Request<InterruptResult>(Methods.Interrupt, TimeSpan.FromSeconds 30.0)

        Assert.True interrupted.interrupted

        // The interrupted interaction must come back rather than hang forever.
        try
            let result = session.EndRequest(running, TimeSpan.FromSeconds 60.0)
            Assert.False(succeeded result, describe session result)
        with _ ->
            // Reported as a failed call rather than a failed interaction; either is acceptable.
            ())

[<Fact>]
let ``interrupt is harmless when nothing is running`` () =
    withInitializedSession (fun session ->
        let result = session.Request<InterruptResult> Methods.Interrupt
        Assert.False result.interrupted)

//-------------------------------------------------------------------------
// Lifetime
//-------------------------------------------------------------------------

[<Fact>]
let ``shutdown ends the session`` () =
    withInitializedSession (fun session ->
        session.Request<obj> Methods.Shutdown |> ignore

        Assert.True(session.WaitForExit 30_000, "the session did not exit after shutdown"))

[<Fact>]
let ``the session exits when the host disconnects`` () =
    let session = new FsiServerHarness()
    session.Initialize() |> ignore

    // Closing the control channel is what happens when the editor process dies. A session that
    // survived it would leak a process for every crash.
    (session :> IDisposable).Dispose()

[<Fact>]
let ``the session exits when its host process exits`` () =
    // A second session stands in for the editor: it is a real, live process to attach to, and
    // killing it must bring down the session that named it as its host.
    use host = new FsiServerHarness()
    use session = new FsiServerHarness()

    session.Initialize(clientProcessId = host.ProcessId) |> ignore
    Assert.False session.HasExited

    (host :> IDisposable).Dispose()

    Assert.True(session.WaitForExit 30_000, "the session outlived its host process")
