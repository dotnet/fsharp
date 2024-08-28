module TypeChecks.CompilationTests

open FSharp.Test
open FSharp.Test.Compiler
open Xunit;
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
            |> List.map (fun (f: FileInScenario) -> SourceCodeFileKind.Create(f.FileName, f.Content))

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

let scenarios = scenarios |> List.map (fun c -> [| box c |])

[<Theory>]
[<MemberData(nameof scenarios)>]
let ``Compile a valid scenario using graph-based type-checking`` (scenario) =
    compileAValidScenario scenario Method.Graph

[<Theory>]
[<MemberData(nameof scenarios)>]
let ``Compile a valid scenario using sequential type-checking`` (scenario) =
    compileAValidScenario scenario Method.Sequential
