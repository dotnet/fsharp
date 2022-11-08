module internal ParallelTypeCheckingTests.Program
#nowarn "1182"
open FSharp.Compiler.CompilerConfig
open ParallelTypeCheckingTests.TestUtils

let _parse (argv: string[]): Args =
    let parseMode (mode : string) =
        match mode.ToLower() with
        | "sequential" -> Method.Sequential
        | "parallelfs" -> Method.ParallelCheckingOfBackedImplFiles
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
    let args = _parse _argv
    let args = {args with LineLimit = None}
    TestCompilationFromCmdlineArgs.TestCompilerFromArgs args
    0