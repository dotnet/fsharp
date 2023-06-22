namespace TypeChecks

module CompilationFromCmdlineArgsTests =

    open System
    open System.IO
    open FSharp.Compiler.CodeAnalysis
    open NUnit.Framework
    open CompilationTests

    // Point to a generated args.txt file.
    // Use scrape.fsx to generate an args.txt from a binary log file.
    // The path needs to be absolute.
    let localProjects: string list =
        [
            @"C:\Projects\fantomas\src\Fantomas.Core\Fantomas.Core.args.txt"
            @"C:\Projects\FsAutoComplete\src\FsAutoComplete\FsAutoComplete.args.txt"
            @"C:\Projects\fsharp\src\Compiler\FSharp.Compiler.Service.args.txt"
            @"C:\Projects\fsharp\tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.args.txt"
        ]

    let checker = FSharpChecker.Create()

    let testCompilerFromArgs (method: Method) (projectArgumentsFilePath: string) : unit =
        let oldWorkDir = Environment.CurrentDirectory

        try
            Environment.CurrentDirectory <- FileInfo(projectArgumentsFilePath).Directory.FullName

            let args =
                let argsFromFile = File.ReadAllLines(projectArgumentsFilePath)

                [|
                    yield "fsc.exe"
                    yield! argsFromFile
                    if not (Array.contains "--times" argsFromFile) then
                        yield "--times"
                    yield! methodOptions method
                |]

            let diagnostics, exitCode = checker.Compile(args) |> Async.RunSynchronously

            for diag in diagnostics do
                printfn "%A" diag

            Assert.That(exitCode, Is.Zero)
        finally
            Environment.CurrentDirectory <- oldWorkDir

    [<TestCaseSource(nameof localProjects)>]
    [<Explicit("Slow, only useful as a sanity check that the test codebase is sound and type-checks using the old method")>]
    let ``Test sequential type-checking`` (projectArgumentsFilePath: string) =
        testCompilerFromArgs Method.Sequential projectArgumentsFilePath

    [<TestCaseSource(nameof localProjects)>]
    [<Explicit("This only runs with the explicitly mentioned projects above")>]
    let ``Test graph-based type-checking`` (projectArgumentsFilePath: string) =
        testCompilerFromArgs Method.Graph projectArgumentsFilePath
