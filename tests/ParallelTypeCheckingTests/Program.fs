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
    OptimizeInputs.UseParallelOptimizer <- true
    // let args = _parse _argv
    // let args = { args with LineLimit = None }
    let componentTests = codebases[1]
    let config = codebaseToConfig componentTests Method.ParallelCheckingOfBackedImplFiles
    TestCompilerFromArgs config
    compileAValidProject
        {
            Method = Method.Sequential
            Project = Codebases.dependentSignatures
        }
    0

