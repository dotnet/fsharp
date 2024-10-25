namespace TypeChecks

module CompilationFromCmdlineArgsTests =

    open System
    open System.IO
    open FSharp.Compiler.CodeAnalysis
    open Xunit
    open CompilationTests

    // Point to a generated args.txt file.
    // Use scrape.fsx to generate an args.txt from a binary log file.
    // The path needs to be absolute.
    let localProjects =
        [
            @"C:\Projects\fantomas\src\Fantomas.Core\Fantomas.Core.args.txt"
            @"C:\Projects\FsAutoComplete\src\FsAutoComplete\FsAutoComplete.args.txt"
            @"C:\Projects\fsharp\src\Compiler\FSharp.Compiler.Service.args.txt"
            @"C:\Projects\fsharp\tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.args.txt"
        ] |> Seq.map (fun p -> [| box p |])

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

            let diagnostics, exn = checker.Compile(args) |> Async.RunSynchronously

            for diag in diagnostics do
                printfn "%A" diag

            Assert.Equal(exn, None)
        finally
            Environment.CurrentDirectory <- oldWorkDir

    [<MemberData(nameof localProjects)>]
    [<Theory(Skip = "Slow, only useful as a sanity check that the test codebase is sound and type-checks using the old method")>]
    let ``Test sequential type-checking`` (projectArgumentsFilePath: string) =
        testCompilerFromArgs Method.Sequential projectArgumentsFilePath

    [<MemberData(nameof localProjects)>]
    [<Theory(Skip = "This should only run with the explicitly mentioned projects above")>]
    let ``Test graph-based type-checking`` (projectArgumentsFilePath: string) =
        testCompilerFromArgs Method.Graph projectArgumentsFilePath
