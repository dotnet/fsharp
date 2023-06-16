module TypeChecks.CompilationTests

open FSharp.Test
open FSharp.Test.Compiler
open NUnit.Framework
open Scenarios

[<Struct>]
type Method =
    | Sequential
    | Graph

let methodOptions (method: Method) =
    match method with
    | Method.Sequential -> []
    | Method.Graph -> [ "--test:GraphBasedChecking"; "--test:DumpCheckingGraph" ]

let withMethod (method: Method) (cu: CompilationUnit) : CompilationUnit =
    match cu with
    | CompilationUnit.FS cs ->
        FS
            { cs with
                Options = cs.Options @ (methodOptions method)
            }
    | cu -> cu

let compileAValidScenario (scenario: Scenario) (method: Method) =
    let cUnit =
        let files =
            scenario.Files
            |> Array.map (fun (f: FileInScenario) -> SourceCodeFileKind.Create(f.FileWithAST.File, f.Content))
            |> Array.toList

        match files with
        | [] -> failwith "empty files"
        | first :: rest ->
            let f = fsFromString first |> FS
            f |> withAdditionalSourceFiles rest

    cUnit
    |> withOutputType CompileOutput.Library
    |> withMethod method
    |> compile
    |> shouldSucceed
    |> ignore

let scenarios = codebases

[<TestCaseSource(nameof scenarios)>]
let ``Compile a valid scenario using graph-based type-checking`` (scenario: Scenario) =
    compileAValidScenario scenario Method.Graph

[<TestCaseSource(nameof scenarios)>]
let ``Compile a valid scenario using sequential type-checking`` (scenario: Scenario) =
    compileAValidScenario scenario Method.Sequential
