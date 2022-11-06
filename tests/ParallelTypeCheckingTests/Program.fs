module internal ParallelTypeCheckingTests.Program
#nowarn "1182"
open FSharp.Compiler.CompilerConfig
open ParallelTypeCheckingTests.TestUtils

let _parse (argv: string[]): Args =
    let parseMode (mode : string) =
        match mode.ToLower() with
        | "sequential" -> TypeCheckingMode.Sequential
        | "parallelfs" -> TypeCheckingMode.ParallelCheckingOfBackedImplFiles
        | "graph" -> TypeCheckingMode.Graph
        | _ -> failwith $"Unrecognised method: {mode}"
    
    let path, mode, workingDir =
        match argv with
        | [|path|] ->
            path, TypeCheckingMode.Sequential, None
        | [|path; mode|] ->
            path, parseMode mode, None
        | [|path; mode; workingDir|] ->
            path, parseMode mode, Some workingDir
        | _ -> failwith "Invalid args - use 'args_path [fs-parallel]"
    
    {
        Path = path
        LineLimit = None
        Method = mode
        WorkingDir = workingDir
    }

[<EntryPoint>]
let main _argv =
    let c =
        {
            Method = Method.Graph
            Project = TestCompilation.Codebases.fsFsi
        } : TestCompilation.Case
    
    TestCompilation.compile c
    // let workDir, path, lineLimit = TestCompilationFromCmdlineArgs.codebases[2]
    // let stuff =
    //     {
    //         Path = path
    //         LineLimit = lineLimit
    //         WorkingDir = Some workDir
    //         Mode = Method.Nojaf
    //     }
    // TestCompilationFromCmdlineArgs.TestCompilerFromArgs stuff
    0