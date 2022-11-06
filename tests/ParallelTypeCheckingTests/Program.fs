module ParallelTypeCheckingTests.Program
#nowarn "1182"
open FSharp.Test
open ParallelTypeCheckingTests.TestUtils
open ParallelTypeCheckingTests.Utils

let _parse (argv: string[]): Args =
    let parseMode (mode : string) =
        match mode.ToLower() with
        | "sequential" -> Method.Sequential
        | "parallelfs" -> Method.ParallelFs
        | "nojaf" -> Method.Nojaf
        | "graph" -> Method.Graph
        | _ -> failwith $"Unrecognised method: {mode}"
    
    let path, mode, workingDir =
        match argv with
        | [|path|] ->
            path, Method.Sequential, None
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
    let c = TestCompilation.Codebases.fsFsi
    let m = Method.Graph
    let c =
        {
            Method = m
            Files = c
            OutputType = CompileOutput.Library
        } : TestCompilation.Case
    
    TestCompilation.``Compile all codebase examples with all methods`` c
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