// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Interactive.Server.Tests.FsiJsonRpcServerTests

open System
open System.IO
open System.Runtime.InteropServices
open System.Threading
open System.Xml.Linq
open Xunit

open FSharp.Compiler.Interactive.Protocol
open FSharp.Compiler.Interactive.Server.Tests.FsiServerHarness

/// Start a session, hand it to the test, and shut it down afterwards.
let private withSession (test: FsiServerHarness -> unit) =
    use session = new FsiServerHarness()
    test session

/// macOS resolves `/tmp`, `/var` and `/etc` through their `/private` targets when a path is
/// canonicalised by the kernel — which is what happens to the session's own working directory
/// after it changes there — but not when `Path.GetTempPath()`/`GetFullPath` builds one in this
/// process. The two would otherwise disagree on the very directory both sides just agreed on.
let private stripMacPrivatePrefix (path: string) =
    if RuntimeInformation.IsOSPlatform OSPlatform.OSX && path.StartsWith("/private/", StringComparison.Ordinal) then
        path.Substring "/private".Length
    else
        path

/// Start a session that has already completed the handshake.
let private withInitializedSession (test: FsiServerHarness -> unit) =
    withSession (fun session ->
        session.Initialize() |> ignore
        test session)

/// Include the result and the session's output in a failure, since a protocol result alone rarely
/// explains what the session actually did.
let private describe (session: FsiServerHarness) (result: ExecutionResult) =
    $"""result: {describeResult result}
standard output:
{session.StandardOutput}
standard error:
{session.StandardError}"""

let private temporaryPath (suffix: string) =
    Path.Combine(Path.GetTempPath(), $"fsiServerTest_{Guid.NewGuid():N}{suffix}")

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

        Assert.StartsWith(".NET", result.frameworkDescription, StringComparison.Ordinal)
        Assert.True result.supportsInterrupt

        Assert.True(
            Directory.Exists result.workingDirectory,
            $"'{result.workingDirectory}' is not a directory"
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

[<Fact>]
let ``only the protocol's own methods are reachable`` () =
    withInitializedSession (fun session ->
        // The handlers are registered one by one, so an implementation member — here the one that
        // would close the execution queue and stop the session from ever running another
        // interaction — is not a method a host can call.
        match session.RequestExpectingError("Complete", obj ()) with
        | None -> failwith "the session accepted a method that is not part of the protocol"
        | Some code -> Assert.Equal(-32601, code)

        let result = session.Execute "1 + 1"
        Assert.True(succeeded result, describe session result))

[<Fact>]
let ``request DTOs reject missing required fields as invalid params`` () =
    withInitializedSession (fun session ->
        let malformedRequests =
            [ Methods.Execute, box {| sourcePath = null; startLine = Nullable<int>() |}
              Methods.ExecuteFile, obj ()
              Methods.SetPaths, box {| workingDirectory = "" |}
              Methods.Execute, box {| code = 42; sourcePath = null; startLine = Nullable<int>() |}
              Methods.ExecuteFile, box {| path = 42 |}
              Methods.Execute, box {| code = "1"; sourcePath = null; startLine = "1" |}
              Methods.SetPaths, box {| includePaths = 42; workingDirectory = "" |}
              Methods.SetPaths, box {| includePaths = Array.empty<string>; workingDirectory = 42 |}
              Methods.Execute, box {| code = "1"; sourcePath = null; startLine = 3000000000L |}
              Methods.SetPaths, box {| includePaths = [| box 42 |]; workingDirectory = "wd" |} ]

        for methodName, parameters in malformedRequests do
            Assert.Equal(Some -32602, session.RequestExpectingError(methodName, parameters)))

[<Fact>]
let ``fails when the command-line owner process cannot be watched`` () =
    let error =
        Assert.ThrowsAny<Exception>(fun () ->
            new FsiServerHarness(clientProcessId = Int32.MaxValue) |> ignore)

    Assert.Contains("exited with code 1", error.Message)

//-------------------------------------------------------------------------
// The command line
//-------------------------------------------------------------------------

[<Fact>]
let ``accepts the switch in its slash spelling`` () =
    // fsi's own option parser recognises the switch, so the other spelling it takes for a long option
    // turns the server on too.
    use session =
        new FsiServerHarness(serverSwitches = fun pipeName -> [ $"/fsi-server-jsonrpc:{pipeName}" ])

    Assert.Equal(session.ProcessId, session.Initialize().processId)

[<Fact>]
let ``accepts the switch from a response file`` () =
    let responseFile = temporaryPath ".rsp"

    try
        use session =
            new FsiServerHarness(
                serverSwitches =
                    fun pipeName ->
                        File.WriteAllText(responseFile, $"--fsi-server-jsonrpc:{pipeName}{Environment.NewLine}")
                        [ $"@{responseFile}" ]
            )

        Assert.Equal(session.ProcessId, session.Initialize().processId)
    finally
        try
            File.Delete responseFile
        with _ ->
            ()

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

[<Theory>]
[<InlineData("let answer = 42", "answer")>]
[<InlineData("42;; open System;;", "it")>]
let ``returns evaluated values`` code name =
    withInitializedSession (fun session ->
        let result = session.Execute code
        Assert.True(succeeded result, describe session result)
        Assert.Contains(result.values, fun value -> value.name = name && value.value = "42"))

[<Fact>]
let ``does not report the helper bindings the compiler introduces`` () =
    withInitializedSession (fun session ->
        // `let a, b = …` compiles through a `patternInput` binding of its own, which must not be
        // taken for the user's.
        let result = session.Execute """let patternInput = "user";; let a, b = (1, 2)"""
        Assert.True(succeeded result, describe session result)

        let names = result.values |> Array.map _.name |> Array.sort
        Assert.Equal<string[]>([| "a"; "b"; "patternInput" |], names))

[<Fact>]
let ``runs requests only after the startup scripts are done`` () =
    let startup = temporaryPath ".fsx"

    // Long enough for the request to arrive while the script is still running, and it replaces the
    // event loop the way a script that drives its own UI toolkit does.
    File.WriteAllText(
        startup,
        """
System.Threading.Thread.Sleep 3000
fsi.EventLoop <- System.Activator.CreateInstance(fsi.EventLoop.GetType(), true) :?> FSharp.Compiler.Interactive.IEventLoop
let startupOnly = 123
"""
    )

    try
        use session = new FsiServerHarness(extraArguments = [ $"--use:{startup}" ])
        session.Initialize() |> ignore

        let result = session.Execute("let requested = 42", timeout = TimeSpan.FromSeconds 60.0)
        Assert.True(succeeded result, describe session result)

        // The script's own binding is not this request's.
        Assert.Equal<string[]>([| "requested" |], result.values |> Array.map _.name)
    finally
        try
            File.Delete startup
        with _ ->
            ()

[<Fact>]
let ``formats values with the session's own printer`` () =
    withInitializedSession (fun session ->
        let result = session.Execute "let many = [ 1 .. 200 ]"
        Assert.True(succeeded result, describe session result)

        // The session's print length applies, so the text is bounded the way the console's is.
        let many = result.values |> Array.find (fun value -> value.name = "many")
        Assert.Contains("...", many.value))

[<Fact>]
let ``a value whose ToString throws does not fail the interaction`` () =
    withInitializedSession (fun session ->
        let result =
            session.Execute "type Loud() = override _.ToString() = failwith \"boom\";; let loud = Loud()"

        Assert.True(succeeded result, describe session result)
        Assert.Contains(result.values, fun value -> value.name = "loud"))

[<Fact>]
let ``keeps bindings across interactions`` () =
    withInitializedSession (fun session ->
        let bound = session.Execute "let x = 40"
        Assert.True(succeeded bound, describe session bound)

        let result = session.Execute "x + 2"
        Assert.True(succeeded result, describe session result)
        Assert.True(session.WaitForOutput "val it: int = 42", describe session result))

[<Fact>]
let ``evaluates every interaction in a request`` () =
    withInitializedSession (fun session ->
        let result = session.Execute "let first = 11;; let second = 22;;"
        Assert.True(succeeded result, describe session result)

        let next = session.Execute "second"
        Assert.True(succeeded next, describe session next)
        Assert.True(session.WaitForOutput "val it: int = 22", describe session next))

[<Fact>]
let ``keeps apostrophe-terminated identifiers intact`` () =
    withInitializedSession (fun session ->
        let result = session.Execute "let value' = 42;; value' + 1"
        Assert.True(succeeded result, describe session result)
        Assert.True(session.WaitForOutput "val it: int = 43", describe session result))

[<Fact>]
let ``splits interactions the way the lexer does`` () =
    withInitializedSession (fun session ->
        // A verbatim string ending in a backslash, and `(*)` — the operator, not a comment. Each would
        // fool a splitter that only looks for `;;` outside strings and comments.
        let result =
            session.Execute """let path = @"C:\";; let times = (*);; let after = path.Length + times 2 3"""

        Assert.True(succeeded result, describe session result)

        let next = session.Execute "after"
        Assert.True(succeeded next, describe session next)
        Assert.True(session.WaitForOutput "val it: int = 9", describe session next))

[<Fact>]
let ``keeps the bindings that ran before a failing interaction`` () =
    withInitializedSession (fun session ->
        let result = session.Execute "let kept = 1;; let broken: int = \"text\";; let never = 2"
        Assert.False(succeeded result, describe session result)
        Assert.NotEmpty(errors result)

        let kept = session.Execute "kept"
        Assert.True(succeeded kept, describe session kept)
        Assert.True(session.WaitForOutput "val it: int = 1", describe session kept)

        // Nothing after the failure ran.
        Assert.False(succeeded (session.Execute "never")))

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
        Assert.True(error.startLine >= 1, $"unexpected start line {error.startLine}")
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
        Assert.EndsWith("Library.fs", reported[0].fileName, StringComparison.Ordinal))

[<Fact>]
let ``positions every interaction of a selection against the host's lines`` () =
    withInitializedSession (fun session ->
        // The line directive covers the whole selection, not just the text before its first `;;`.
        let path = Path.Combine(Path.GetTempPath(), "Library.fs")

        let result =
            session.Execute("let ok = 1;;\nlet bad: int = \"text\"", sourcePath = path, startLine = 120)

        let reported = errors result
        Assert.NotEmpty reported
        Assert.Equal(121, reported[0].startLine))

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
        let script = temporaryPath ".fsx"
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
let ``loads a script file whose path contains quotes`` () =
    if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
        ()
    else
        withInitializedSession (fun session ->
            let directory = temporaryPath "\"quoted"
            Directory.CreateDirectory directory |> ignore
            let script = Path.Combine(directory, "script.fsx")
            File.WriteAllText(script, "printfn \"quoted path loaded\"\n")

            try
                let loaded = session.Request<ExecutionResult>(Methods.ExecuteFile, { path = script })
                Assert.True(succeeded loaded, describe session loaded)
                Assert.True(session.WaitForOutput "quoted path loaded", describe session loaded)
            finally
                try
                    Directory.Delete(directory, true)
                with _ ->
                    ())

[<Fact>]
let ``setPaths changes the working directory`` () =
    withInitializedSession (fun session ->
        let directory = temporaryPath ""
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
            let expected =
                Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar) |> stripMacPrivatePrefix

            let actual =
                Path.GetFullPath(result.workingDirectory).TrimEnd(Path.DirectorySeparatorChar)
                |> stripMacPrivatePrefix

            Assert.Equal(expected, actual)
        finally
            try
                Directory.Delete(directory, true)
            with _ ->
                ())

[<Fact>]
let ``setPaths rejects an unrepresentable path without changing the working directory`` () =
    withInitializedSession (fun session ->
        let before = session.Execute "1"
        let path = temporaryPath "\"quoted"

        let error =
            session.RequestExpectingError(
                Methods.SetPaths,
                {
                    includePaths = [| path |]
                    workingDirectory = path
                }
            )

        Assert.Equal(Some -32602, error)
        Assert.Equal(before.workingDirectory, (session.Execute "2").workingDirectory))

[<Fact>]
let ``setPaths rejects a missing working directory`` () =
    withInitializedSession (fun session ->
        let error =
            session.RequestExpectingError(
                Methods.SetPaths,
                {
                    includePaths = [||]
                    workingDirectory = temporaryPath ""
                }
            )

        Assert.Equal(Some -32002, error))

[<Fact>]
let ``setPaths waits its turn behind a running interaction`` () =
    withInitializedSession (fun session ->
        let directory = temporaryPath ""
        Directory.CreateDirectory directory |> ignore

        try
            // Warm the session up, so that the interaction below is genuinely running by the time
            // the request to move the directory arrives.
            let warmUp = session.Execute "1"
            Assert.True(succeeded warmUp, describe session warmUp)

            let running =
                session.BeginRequest<ExecutionResult>(
                    Methods.Execute,
                    FsiServerHarness.ExecuteParams
                        """
System.Threading.Thread.Sleep 5000
printfn "interaction saw [%s]" (System.IO.Directory.GetCurrentDirectory())
"""
                )

            Thread.Sleep 2000

            let moved =
                session.Request<ExecutionResult>(
                    Methods.SetPaths,
                    {
                        includePaths = [||]
                        workingDirectory = directory
                    }
                )

            Assert.True(succeeded moved, describe session moved)

            let result = session.EndRequest(running, TimeSpan.FromSeconds 60.0)
            Assert.True(succeeded result, describe session result)

            // The process directory moves on the queue like everything else, so an interaction that
            // was already running keeps the directory it started in.
            Assert.True(
                session.WaitForOutput $"interaction saw [{warmUp.workingDirectory}]",
                describe session result
            )
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
        let running =
            session.BeginRequest<ExecutionResult>(
                Methods.Execute,
                FsiServerHarness.ExecuteParams
                    """
printfn "interrupt target started"
while true do System.Threading.Thread.Sleep 10
"""
            )

        Assert.True(session.WaitForOutput "interrupt target started")

        let next =
            session.BeginRequest<ExecutionResult>(Methods.Execute, FsiServerHarness.ExecuteParams "40 + 2")

        let interrupts =
            Array.init 8 (fun _ -> session.BeginRequest<InterruptResult> Methods.Interrupt)

        let interruptResults =
            interrupts
            |> Array.map (fun request -> session.EndRequest(request, TimeSpan.FromSeconds 30.0))

        Assert.Equal(1, interruptResults |> Array.filter _.interrupted |> Array.length)

        let interrupted = session.EndRequest(running, TimeSpan.FromSeconds 60.0)
        Assert.True(interrupted.cancelled, describe session interrupted)
        Assert.False(interrupted.success, describe session interrupted)

        let subsequent = session.EndRequest(next, TimeSpan.FromSeconds 60.0)
        Assert.True(subsequent.success, describe session subsequent)
        Assert.True(session.WaitForOutput "val it: int = 42", describe session subsequent))

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

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``the session exits when the control channel closes`` corrupt =
    use session = new FsiServerHarness()
    session.Initialize() |> ignore

    session.CloseControlChannel corrupt
    Assert.True(session.WaitForExit 30_000, "the session did not exit after the control channel closed")
    Assert.Equal(corrupt, session.ExitCode <> 0)

    if corrupt then
        Assert.Contains("F# Interactive server terminated:", session.StandardError)

[<Fact>]
let ``the session exits when its host process exits`` () =
    // A second session stands in for the editor: it is a real, live process to attach to, and
    // killing it must bring down the session that named it as its host.
    use host = new FsiServerHarness()
    use session = new FsiServerHarness(clientProcessId = host.ProcessId)

    session.Initialize() |> ignore
    Assert.False session.HasExited

    (host :> IDisposable).Dispose()

    Assert.True(session.WaitForExit 30_000, "the session outlived its host process")

[<Fact>]
let ``the last owner named on the command line is the one that counts`` () =
    // A host puts its own switches after the user's arguments to keep them from naming another owner,
    // which holds only if the last occurrence wins.
    use earlier = new FsiServerHarness()

    use session =
        new FsiServerHarness(
            serverSwitches = fun pipeName -> [ $"--fsi-server-client-pid:{earlier.ProcessId}"; $"--fsi-server-jsonrpc:{pipeName}" ]
        )

    session.Initialize() |> ignore

    (earlier :> IDisposable).Dispose()

    Assert.False(session.WaitForExit 5_000, "the session followed the owner named first")
    Assert.True(succeeded (session.Execute "1 + 1"))

//-------------------------------------------------------------------------
// What ships
//-------------------------------------------------------------------------

/// The `Microsoft.FSharp.Compiler` package is what the .NET SDK lays out as `dotnet fsi`: a file its
/// manifest does not list is a file no installed SDK has.
module private CompilerPackage =

    let private repositoryRoot () =
        let rec search (directory: DirectoryInfo) =
            match directory with
            | null -> failwith "the repository root was not found above the test output"
            | directory ->
                if File.Exists(Path.Combine(directory.FullName, "src", "Microsoft.FSharp.Compiler", "Microsoft.FSharp.Compiler.nuspec")) then
                    directory.FullName
                else
                    search directory.Parent

        search (DirectoryInfo AppContext.BaseDirectory)

    let private projectDirectory () =
        Path.Combine(repositoryRoot (), "src", "Microsoft.FSharp.Compiler")

    let private localName (name: string) (element: XElement) = element.Name.LocalName = name

    /// The assemblies the project file adds next to fsi.dll for the JSON-RPC server.
    let serverAssemblies () =
        XDocument.Load(Path.Combine(projectDirectory (), "Microsoft.FSharp.Compiler.fsproj")).Descendants()
        |> Seq.filter (localName "FsiJsonRpcServerAssemblies")
        |> Seq.collect (fun element -> element.Value.Split([| ';' |], StringSplitOptions.RemoveEmptyEntries))
        |> Seq.toArray

    /// The assemblies the manifest puts into the package's lib folder beside fsi.dll — by name, since
    /// fsi's own output holds the same builds of them that the pack step picks up.
    let libraryAssemblies () =
        XDocument.Load(Path.Combine(projectDirectory (), "Microsoft.FSharp.Compiler.nuspec")).Descendants()
        |> Seq.filter (localName "file")
        |> Seq.choose (fun element ->
            let source = element.Attribute(XName.Get "src").Value
            let target = element.Attribute(XName.Get "target").Value

            // Resource satellites are globbed. The compiler driver and the MSBuild tasks share the
            // folder but are not fsi's to load, and fsi's own build does not produce them.
            // The nuspec spells paths with backslashes, which Path.GetFileName only splits on Windows.
            let name = source.Substring(source.LastIndexOf '\\' + 1)

            if
                target.StartsWith("lib", StringComparison.Ordinal)
                && source.IndexOf("**", StringComparison.Ordinal) < 0
                && name.EndsWith(".dll", StringComparison.Ordinal)
                && name <> "fsc.dll"
                && name <> "FSharp.Build.dll"
            then
                Some name
            else
                None)
        |> Seq.toArray

    /// Assemblies the SDK provides beside fsi from its own build, so the package leaves them out.
    let providedBySdk (fileName: string) =
        fileName.StartsWith("Microsoft.Build.", StringComparison.Ordinal)
        || fileName.StartsWith("Microsoft.NET.StringTools", StringComparison.Ordinal)
        || fileName.StartsWith("System.", StringComparison.Ordinal)

[<Fact>]
let ``the compiler package lists every assembly the server loads`` () =
    // Whatever fsi's build restored beyond what this repository builds and what the SDK provides is
    // there for the server, and has to be in the package or the shipped fsi cannot start the server.
    let restoredForServer =
        Directory.EnumerateFiles(fsiOutputDirectory (), "*.dll")
        |> Seq.map Path.GetFileName
        |> Seq.filter (fun name ->
            not (name.StartsWith("FSharp.", StringComparison.Ordinal))
            && name <> "fsi.dll"
            && not (CompilerPackage.providedBySdk name))
        |> Seq.sort
        |> Seq.toArray

    Assert.Equal<string[]>(restoredForServer, CompilerPackage.serverAssemblies () |> Array.sort)

[<Fact>]
let ``the shipped files are enough to start the server`` () =
    // Stage exactly what an SDK has beside fsi.dll — the package's lib folder plus the assemblies the
    // SDK adds from its own build — and start a session from there.
    let staged = temporaryPath ""
    Directory.CreateDirectory staged |> ignore

    let fsiDirectory = fsiOutputDirectory ()

    let stage (fileName: string) =
        File.Copy(Path.Combine(fsiDirectory, fileName), Path.Combine(staged, fileName), true)

    try
        CompilerPackage.libraryAssemblies () |> Array.iter stage
        CompilerPackage.serverAssemblies () |> Array.iter stage

        Directory.EnumerateFiles(fsiDirectory, "*.dll")
        |> Seq.map Path.GetFileName
        |> Seq.filter CompilerPackage.providedBySdk
        |> Seq.iter stage

        // No fsi.deps.json: the SDK generates its own, and without one the host probes the directory,
        // so the files themselves are what is under test.
        stage "fsi.runtimeconfig.json"

        use session = new FsiServerHarness(fsiDirectory = staged)
        session.Initialize() |> ignore

        let result = session.Execute "1 + 1"
        Assert.True(succeeded result, describe session result)
    finally
        try
            Directory.Delete(staged, true)
        with _ ->
            ()
