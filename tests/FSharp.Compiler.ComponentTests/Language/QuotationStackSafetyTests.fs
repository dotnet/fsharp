// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module Language.QuotationStackSafetyTests

open System
open System.IO
open System.Reflection
open System.Security.Cryptography
open FSharp.Test.Compiler
open FSharp.Test.Utilities
open Xunit

let private timeout = 120000
let private config = TestFramework.initialConfig

let private elements count render =
    [ for i in 1..count -> render i ] |> String.concat "; "

let private listLiteral count = $"[ {elements count string} ]"

let private hash path =
    use stream = File.OpenRead path
    use algorithm = SHA256.Create()
    BitConverter.ToString(algorithm.ComputeHash stream).Replace("-", "")

let private assertProcess (result: ProcessResult) =
    Assert.True(result.ExitCode = 0, $"Exit code {result.ExitCode}\n{result.StdOut}\n{result.StdErr}")
    Assert.Contains("done", result.StdOut.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries))
    printfn "%s" result.StdOut

let private sourceCore =
    let tfm =
        match typeof<list<int>>.Assembly.GetCustomAttribute<Runtime.Versioning.TargetFrameworkAttribute>() with
        | null -> "netstandard2.0"
        | attr when attr.FrameworkName.StartsWith(".NETStandard,Version=v2.1", StringComparison.Ordinal) ->
            "netstandard2.1"
        | _ -> "netstandard2.0"

    Path.Combine(
        TestFramework.repoRoot,
        "artifacts",
        "bin",
        "FSharp.Core",
        config.BUILD_CONFIG,
        tfm,
        "FSharp.Core.dll"
    )

let private assertIdentity (result: ProcessResult) =
    let toolDirectory = Path.GetDirectoryName(Path.GetFullPath config.FSI)

    let sourceCompiler =
        Path.Combine(
            TestFramework.repoRoot,
            "artifacts",
            "bin",
            "FSharp.Compiler.Service",
            config.BUILD_CONFIG,
#if NETCOREAPP
            Path.GetFileName toolDirectory,
#else
            "netstandard2.0",
#endif
            "FSharp.Compiler.Service.dll"
        )

    Assert.True(File.Exists config.FSCOREDLLPATH, $"Missing Core reference assembly: {config.FSCOREDLLPATH}")

    printfn
        "configuration=%s fsi=%s fsc=%s core-reference=%s core-runtime=%s dotnet=%s"
        config.BUILD_CONFIG
        config.FSI
        config.FSC
        config.FSCOREDLLPATH
        sourceCore
        config.DotNetExe

    for name, assembly, sourcePath in
        [ "compiler", typeof<FSharp.Compiler.CodeAnalysis.FSharpChecker>.Assembly, sourceCompiler
          "core", typeof<list<int>>.Assembly, sourceCore ] do
        let path = Path.GetFullPath assembly.Location

        let configuration =
            assembly.GetCustomAttribute<AssemblyConfigurationAttribute>().Configuration

        let sourceHash = hash sourcePath

        printfn
            "host-%s=%s\nhost-%s-mvid=%O\nhost-%s-configuration=%s\nhost-%s-sha256=%s\nsource-%s=%s\nsource-%s-sha256=%s"
            name
            path
            name
            assembly.ManifestModule.ModuleVersionId
            name
            configuration
            name
            (hash path)
            name
            (Path.GetFullPath sourcePath)
            name
            sourceHash

        Assert.Equal(Path.Combine(AppContext.BaseDirectory, Path.GetFileName sourcePath), path, ignoreCase = true)
        Assert.Equal(config.BUILD_CONFIG, configuration)
        Assert.Equal(sourceHash, hash path)

        for tool in [ config.FSI; config.FSC ] do
            Assert.Equal(sourceHash, hash (Path.Combine(Path.GetDirectoryName tool, Path.GetFileName sourcePath)))

        Assert.Contains(
            $"{name}={Path.Combine(toolDirectory, Path.GetFileName sourcePath)}",
            result.StdOut,
            StringComparison.OrdinalIgnoreCase
        )

        Assert.Contains($"{name}-mvid={assembly.ManifestModule.ModuleVersionId}", result.StdOut)
        Assert.Contains($"{name}-configuration={configuration}", result.StdOut)
        Assert.Contains($"{name}-sha256={sourceHash}", result.StdOut)

let private identity =
    $"""
for name, assembly in [ "compiler", AppDomain.CurrentDomain.GetAssemblies() |> Array.find (fun a -> a.GetName().Name = "FSharp.Compiler.Service"); "core", typeof<list<int>>.Assembly ] do
    printfn "%%s=%%s" name (System.IO.Path.GetFullPath assembly.Location)
    printfn "%%s-mvid=%%O" name assembly.ManifestModule.ModuleVersionId
    printfn "%%s-configuration=%%s" name ((assembly.GetCustomAttributes(typeof<System.Reflection.AssemblyConfigurationAttribute>, false).[0] :?> System.Reflection.AssemblyConfigurationAttribute).Configuration)
    use stream = System.IO.File.OpenRead assembly.Location
    use algorithm = System.Security.Cryptography.SHA256.Create()
    printfn "%%s-sha256=%%s" name (BitConverter.ToString(algorithm.ComputeHash stream).Replace("-", ""))
"""

let private inDirectory action =
    let directory = TestFramework.createTemporaryDirectory ()

    try
        let result = action directory.FullName
        directory.Delete(true)
        result
    with _ ->
        printfn "failed-test-artifacts=%s" directory.FullName
        reraise ()

let private runtime =
    """
open System
open System.Threading
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

let checkList elementType count checkHead (quotation: Expr) =
    let listType = typedefof<list<_>>.MakeGenericType [| elementType |]
    if quotation.Type <> listType then failwith "Wrong quotation type"
    let mutable tail = quotation
    for i in 1 .. count do
        match tail with
        | NewUnionCase(case, [head; rest]) when case.DeclaringType = listType && case.Name = "Cons" ->
            if case.GetFields().Length <> 2 then failwith "Wrong cons fields"
            checkHead i head
            tail <- rest
        | _ -> failwithf "Expected cons at index %d" i
    match tail with
    | NewUnionCase(case, []) when case.DeclaringType = listType && case.Name = "Empty" && case.GetFields().Length = 0 -> ()
    | _ -> failwith "Expected exact terminal nil"

let checkInt expected = function
    | Value(:? int as actual, ty) when ty = typeof<int> && actual = expected -> ()
    | _ -> failwithf "Wrong integer head at %d" expected

let run action =
    let mutable error = ValueNone
    let worker = Thread(ThreadStart(fun () ->
        try
            printfn "runtime core=%s thread=%d stack=262144" typeof<list<int>>.Assembly.Location Environment.CurrentManagedThreadId
            action()
        with ex -> error <- ValueSome ex), 256 * 1024)
    worker.IsBackground <- true
    worker.Start()
    if not (worker.Join(90000)) then failwith "Runtime worker timed out"
    error |> ValueOption.iter raise
    printfn "done"
"""

let private compilerDriver (directory: string) =
    let source =
        """
open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics

let compile (responseFile: string) stackSize =
    let mutable exitCode = 1
    let start = ThreadStart(fun () ->
        try
            let workerThread = Environment.CurrentManagedThreadId
            printfn "compiler=%s core=%s thread=%d pool=%b stack=%d"
                typeof<FSharpChecker>.Assembly.Location typeof<list<int>>.Assembly.Location
                workerThread Thread.CurrentThread.IsThreadPoolThread stackSize
            let checker = FSharpChecker.Create()
            let arguments = Array.append [| "fsc.exe" |] (File.ReadAllLines responseFile)
#if NETCOREAPP
            let allocated = GC.GetTotalAllocatedBytes(true)
#endif
            let stopwatch = Diagnostics.Stopwatch.StartNew()
            let compilation =
                async {
                    if Environment.CurrentManagedThreadId <> workerThread then
                        failwith "Compilation left the configured worker"
                    printfn "compile-start thread=%d" Environment.CurrentManagedThreadId
                    return! checker.Compile arguments
                }
                |> Async.StartImmediateAsTask
            if not compilation.IsCompleted || Environment.CurrentManagedThreadId <> workerThread then
                failwith "Compilation did not complete synchronously on the configured worker"
            let mutable awaiter = compilation.GetAwaiter()
            let diagnostics, error = awaiter.GetResult()
            printfn "compile-end synchronous=true thread=%d elapsed-ms=%d"
                Environment.CurrentManagedThreadId stopwatch.ElapsedMilliseconds
#if NETCOREAPP
            printfn "allocated-bytes=%d" (GC.GetTotalAllocatedBytes(true) - allocated)
#endif
            for diagnostic in diagnostics do
                eprintfn "diagnostic severity=%O code=%d FS%04d: %s"
                    diagnostic.Severity diagnostic.ErrorNumber diagnostic.ErrorNumber diagnostic.Message
            let errors = diagnostics |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
            printfn "error-diagnostics=%d" errors.Length
            if errors.Length <> 0 then
                failwith "Compilation reported errors"
            match error with
            | Some ex when
                Array.contains "--typecheckonly" arguments &&
                ex.GetType() = typeof<FSharpChecker>.Assembly.GetType("FSharp.Compiler.DiagnosticsLogger+StopProcessingExn", true) &&
                Microsoft.FSharp.Reflection.FSharpValue.GetExceptionFields(ex, Reflection.BindingFlags.Public ||| Reflection.BindingFlags.NonPublic) = [| null |] &&
                (Diagnostics.StackTrace(ex).GetFrames() |> Array.exists (fun frame ->
                    let method = frame.GetMethod()
                    method.DeclaringType.FullName = "FSharp.Compiler.Driver" && method.Name = "main2")) ->
                // fsc.main2 exits successfully via this internal control exception.
                printfn "typecheck-only completed: %O\n%s" ex ex.StackTrace
            | Some ex -> raise ex
            | None -> ()
            exitCode <- 0
        with ex ->
            eprintfn "%O" ex
            exitCode <- 1)
    let worker = if stackSize = 0 then Thread(start) else Thread(start, stackSize)
    worker.IsBackground <- true
    worker.Start()
    if not (worker.Join(90000)) then failwith "Compilation worker timed out"
    if exitCode <> 0 then failwithf "Compiler exited with %d" exitCode
    printfn "done"
"""

    let source = "module QuotationCompilerDriver\n" + runtime + source
    File.WriteAllText(Path.Combine(directory, "QuotationCompilerDriver.fs"), source)

    FSharp source
    |> withName "QuotationCompilerDriver"
    |> withOutputDirectory (Some(DirectoryInfo directory))
    |> withReferenceFSharpCompilerService
#if NETCOREAPP
    |> withOptions [ "--define:NETCOREAPP" ]
#endif
    |> asLibrary
    |> compile
    |> shouldSucceed
    |> ignore

    printfn "driver-generation succeeded"
    let script = Path.Combine(directory, "compile.fsx")

    let driverScript =
        "#r \"QuotationCompilerDriver.dll\"\nopen System\n"
        + identity
        + "QuotationCompilerDriver.compile fsi.CommandLineArgs.[1] (int fsi.CommandLineArgs.[2])\n"

    File.WriteAllText(script, driverScript)
    script

let private compileInWorker (directory: string) name options (source: string) =
    let driverScript = compilerDriver directory
    let sourcePath = Path.Combine(directory, name + ".fs")
    let outputPath =
#if NETCOREAPP
        Path.Combine(directory, name + ".dll")
#else
        Path.Combine(directory, name + (if List.contains "--target:library" options then ".dll" else ".exe"))
#endif
    let responseFile = Path.Combine(directory, name + ".args")
    File.WriteAllText(sourcePath, source)

    File.WriteAllLines(
        responseFile,
        [| "--nologo"
#if NETCOREAPP
           "--targetprofile:netcore"
#else
           "--targetprofile:mscorlib"
#endif
           "--noframework"
           "--simpleresolution"
           $"""-r:{Path.Combine(directory, "QuotationCompilerDriver.dll")}"""
           yield!
               TargetFrameworkUtil.currentReferences
               |> Array.map (fun path ->
                   let path =
                       if Path.GetFileName path = "FSharp.Core.dll" then
                           config.FSCOREDLLPATH
                       else
                           path

                   $"-r:{Path.GetFullPath path}")
           yield! options
           $"-o:{outputPath}"
           sourcePath |]
    )

    printfn
        "configuration=%s fsi=%s fsc=%s core-reference=%s dotnet=%s"
        config.BUILD_CONFIG
        config.FSI
        config.FSC
        config.FSCOREDLLPATH
        config.DotNetExe

    printfn "tested-source=%s source-sha256=%s response-file=%s" sourcePath (hash sourcePath) responseFile

    printfn
        "child-fsi=\"%s\" --nologo --exec \"%s\" \"%s\" 262144"
        config.FSI
        driverScript
        responseFile

    let result =
        runFsiProcessWithTimeout timeout [ "--nologo"; "--exec"; driverScript; responseFile; "262144" ]

    printfn "child-exit=%d\n%s\n%s" result.ExitCode result.StdOut result.StdErr
    assertProcess result
    assertIdentity result
    Assert.Contains("error-diagnostics=0", result.StdOut)
    Assert.Contains("pool=false stack=262144", result.StdOut)
    Assert.Contains("compile-end synchronous=true", result.StdOut)

    if not (List.contains "--typecheckonly" options) then
        Assert.True(File.Exists outputPath, $"Missing compiled assembly: {outputPath}")

    outputPath

let private runExecutable (directory: string) (outputPath: string) =
#if NETCOREAPP
    File.Copy(
        Path.ChangeExtension(TestFramework.initialConfig.FSI, ".runtimeconfig.json"),
        Path.ChangeExtension(outputPath, ".runtimeconfig.json")
    )
#endif

    File.Copy(sourceCore, Path.Combine(directory, "FSharp.Core.dll"), true)

    let exitCode, stdout, stderr =
        TestFramework.Commands.executeProcessWithTimeout
            timeout
#if NETCOREAPP
            TestFramework.initialConfig.DotNetExe
            $"\"{outputPath}\""
#else
            outputPath
            ""
#endif
            directory

    assertProcess
        { ExitCode = exitCode
          StdOut = stdout
          StdErr = stderr }

let private executable directory options declarations body =
    compileInWorker
        directory
        "QuotationProgram"
        options
        ("open Microsoft.FSharp.Quotations.Patterns\nopen QuotationCompilerDriver\n"
         + declarations
         + "\n[<EntryPoint>]\nlet main _ =\n    run (fun () ->\n"
         + body
         + "\n    )\n    0")
    |> runExecutable directory

[<Fact>]
let ``Issue 20788 exact FSI literal`` () =
    inDirectory (fun directory ->
        let script = Path.Combine(directory, "repro.fsx")
        let source = $"let q = <@ {listLiteral 1500} @>\nprintfn \"done\""
        let identityScript = Path.Combine(directory, "identity.fsx")
        File.WriteAllText(identityScript, "open System\n" + identity)
        File.WriteAllText(script, source)
        printfn "literal-count=1500 source=%s source-sha256=%s" script (hash script)

        printfn
            "child-fsi=\"%s\" --nologo --use:\"%s\" --exec \"%s\""
            config.FSI
            identityScript
            script

        let result =
            runFsiProcessWithTimeout timeout [ "--nologo"; $"--use:{identityScript}"; "--exec"; script ]

        printfn "child-exit=%d\n%s\n%s" result.ExitCode result.StdOut result.StdErr
        assertProcess result
        assertIdentity result)

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(32)>]
[<InlineData(1000)>]
[<InlineData(1500)>]
[<InlineData(5000)>]
[<InlineData(10000)>]
let ``Issue 20788 translation only on small stacks`` count =
    inDirectory (fun directory ->
        compileInWorker
            directory
            "QuotationTranslation"
            [ "--typecheckonly"; "--target:library" ]
            $"module QuotationTranslation\nlet q = <@ ({listLiteral count} : int list) @>\nprintfn \"done\""
        |> ignore)

[<Theory>]
[<InlineData("success")>]
[<InlineData("failure")>]
[<InlineData("timeout")>]
[<InlineData("invalid-source")>]
let ``Issue 20788 isolated harness probes`` probe =
    inDirectory (fun directory ->
        match probe with
        | "success" ->
            for expression in [ "42"; "<@ [1] @>" ] do
                compileInWorker
                    directory
                    "TinyTranslation"
                    [ "--typecheckonly"; "--target:library" ]
                    $"module TinyTranslation\nlet value = {expression}"
                |> ignore
        | "failure" ->
            let script = Path.Combine(directory, "failure.fsx")

            File.WriteAllText(
                script,
                "open System\n"
                + identity
                + "\nprintfn \"intentional child stdout\"\neprintfn \"intentional child failure\"\nexit 17"
            )

            let result = runFsiProcessWithTimeout timeout [ "--nologo"; "--exec"; script ]
            Assert.Equal(17, result.ExitCode)
            assertIdentity result
            let error = Assert.ThrowsAny<Exception>(fun () -> assertProcess result)
            Assert.Contains("intentional child stdout", error.Message)
            Assert.Contains("intentional child failure", error.Message)
            printfn "%s" error.Message
        | "invalid-source" ->
            let error =
                Assert.ThrowsAny<Exception>(fun () ->
                    compileInWorker
                        directory
                        "InvalidTranslation"
                        [ "--typecheckonly"; "--target:library" ]
                        "module InvalidTranslation\nlet value: int = \"not an integer\""
                    |> ignore)

            Assert.Contains("Exit code 1", error.Message)
            Assert.Contains("error-diagnostics=1", error.Message)
            Assert.Contains("diagnostic severity=Error code=1 FS0001:", error.Message)

            Assert.Single(
                error.Message.Split('\n')
                |> Array.filter (fun line -> line.StartsWith("diagnostic severity=Error ", StringComparison.Ordinal))
            )
            |> ignore

            Assert.DoesNotContain("typecheck-only completed:", error.Message)

            assertIdentity
                { ExitCode = 1
                  StdOut = error.Message
                  StdErr = "" }

            printfn "%s" error.Message
        | "timeout" ->
            let script = Path.Combine(directory, "timeout.fsx")
            let pidFile = Path.Combine(directory, "child.pid")

            File.WriteAllText(
                script,
                "open System\n"
                + identity
                + $"""
use child = System.Diagnostics.Process.GetCurrentProcess()
System.IO.File.WriteAllLines(@"{pidFile}", [| string child.Id; string (child.StartTime.ToUniversalTime().Ticks) |])
printfn "timeout child started"
eprintfn "timeout child stderr"
System.Threading.Thread.Sleep System.Threading.Timeout.Infinite"""
            )

            let stopwatch = System.Diagnostics.Stopwatch.StartNew()

            let error =
                Assert.Throws<TimeoutException>(fun () ->
                    runFsiProcessWithTimeout timeout [ "--nologo"; "--exec"; script ] |> ignore)

            printfn "%s" error.Message
            Assert.True(
                stopwatch.ElapsedMilliseconds < int64 timeout + 15000L,
                $"Timeout cleanup took {stopwatch.Elapsed}"
            )
            Assert.Contains($"Process timed out after {timeout}ms", error.Message)
            Assert.Contains("timeout child started", error.Message)
            Assert.Contains("timeout child stderr", error.Message)
            let childIdentity = File.ReadAllLines pidFile
            let pid = int childIdentity.[0]
            let startTicks = int64 childIdentity.[1]

            try
                use child = System.Diagnostics.Process.GetProcessById pid

                if child.StartTime.ToUniversalTime().Ticks = startTicks then
                    Assert.True(child.HasExited, $"Timed-out child {pid} ({startTicks}) is still alive")
            with :? ArgumentException ->
                () // An exited process may already have been removed from the process table.

            assertIdentity
                { ExitCode = 0
                  StdOut = error.Message
                  StdErr = "" }

            printfn
                "timeout-child-pid=%d start-ticks=%d exited=true elapsed-ms=%d"
                pid
                startTicks
                stopwatch.ElapsedMilliseconds

            printfn "%s" error.Message
        | _ -> failwithf "Unknown probe: %s" probe)

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Issue 20788 tiny full compilation controls`` quoted =
    inDirectory (fun directory ->
        let expression = if quoted then "<@ [1] @>" else "42"

        compileInWorker
            directory
            "TinyCompilation"
            [ "--target:library" ]
            $"module TinyCompilation\nlet value = {expression}"
        |> ignore)

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Issue 20788 translation controls`` quotedArray =
    inDirectory (fun directory ->
        let expression =
            if quotedArray then
                $"<@ [| {elements 10000 string} |] @>"
            else
                listLiteral 5000

        compileInWorker
            directory
            "TranslationControl"
            [ "--typecheckonly"; "--target:library" ]
            $"module TranslationControl\nlet value = {expression}"
        |> ignore)

[<Theory>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(32)>]
[<InlineData(1000)>]
[<InlineData(1500)>]
[<InlineData(5000)>]
[<InlineData(10000)>]
let ``Issue 20788 literal compilation and runtime on small stacks`` count =
    inDirectory (fun directory ->
        executable
            directory
            []
            ""
            $"        checkList typeof<int> {count} checkInt <@ ({listLiteral count} : int list) @>")

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Issue 20788 ordinary list and quoted array controls`` quotedArray =
    inDirectory (fun directory ->
        let declarations, body =
            if quotedArray then
                "",
                $"""        match <@ [| {elements 10000 string} |] @> with
        | NewArray(ty, heads) when ty = typeof<int> && heads.Length = 10000 -> List.iteri (fun i head -> checkInt (i + 1) head) heads
        | _ -> failwith "Wrong array shape"
"""
            else
                // The desktop JIT reserves a large frame for this unquoted literal.
                $"let values = {listLiteral 5000}",
                """        if values <> [1 .. 5000] then failwith "Wrong ordinary list"
"""

        executable directory [] declarations body)
