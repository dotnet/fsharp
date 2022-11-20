module internal ParallelTypeCheckingTests.Program

#nowarn "1182"

open FSharp.Compiler
open FSharp.Compiler.CompilerConfig
open ParallelTypeCheckingTests.TestCompilation
open ParallelTypeCheckingTests.TestUtils

let _parse (argv: string[]) : Args =
    let parseMode (mode: string) =
        match mode.ToLower() with
        | "sequential" -> Method.Sequential
        | "parallelfs" -> Method.ParallelCheckingOfBackedImplFiles
        | "graph" -> Method.Graph
        | _ -> failwith $"Unrecognised mode: {mode}"

    let path, mode, workingDir =
        match argv with
        | [| path |] -> path, Method.Sequential, None
        | [| path; method |] -> path, parseMode method, None
        | [| path; method; workingDir |] -> path, parseMode method, Some workingDir
        | _ -> failwith "Invalid args - use 'args_path [method [fs-parallel]]'"

    {
        Path = path
        LineLimit = None
        Method = mode
        WorkingDir = workingDir
    }

open ParallelTypeCheckingTests.TestCompilationFromCmdlineArgs
[<EntryPoint>]
let main _argv =
    ParseAndCheckInputs.CheckMultipleInputsUsingGraphMode <-
            ParallelTypeChecking.CheckMultipleInputsInParallel
    OptimizeInputs.UseParallelOptimizer <- bool.Parse(_argv[0])
    // let args = _parse _argv
    // let args = { args with LineLimit = None }
    let componentTests = codebases[System.Int32.Parse(_argv[1])]
    let config = codebaseToConfig componentTests Method.Graph
    TestCompilerFromArgs config
    0

