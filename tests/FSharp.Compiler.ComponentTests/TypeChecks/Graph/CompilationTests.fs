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
    | Method.Sequential -> ["--parallelcompilation-"]
    | Method.Graph -> ["--test:DumpCheckingGraph"]

let withMethod (method: Method) (cu: CompilationUnit) : CompilationUnit =
    match cu with
    | CompilationUnit.FS cs ->
        FS
            { cs with
                Options = cs.Options @ (methodOptions method)
            }
    | cu -> cu

let compileScenario (scenario: Scenario) (method: Method) =
    let cUnit =
        let files =
            scenario.Files
            |> List.map (fun (f: FileInScenario) -> SourceCodeFileKind.Create(f.FileName, f.Content))

        match files with
        | [] -> failwith "empty files"
        | first :: rest ->
            let f = fsFromString first |> FS
            f |> withAdditionalSourceFiles rest

    let dir = TestFramework.createTemporaryDirectory()

    printfn "Compiling scenario '%s' \nin directory %s" scenario.Name dir.FullName

    cUnit
    |> withName scenario.Name
    |> withOutputDirectory (Some dir)
    |> ignoreWarnings
    |> withOutputType CompileOutput.Library
    |> withMethod method
    |> compile

let compileAValidScenario (scenario: Scenario) (method: Method) =
    compileScenario scenario method
    |> shouldSucceed
    |> ignore

let scenarios = compilingScenarios |> List.map (fun c -> [| box c |])

[<Theory>]
[<MemberData(nameof scenarios)>]
let ``Compile a valid scenario using graph-based type-checking`` (scenario) =
    compileAValidScenario scenario Method.Graph

[<Theory>]
[<MemberData(nameof scenarios)>]
let ``Compile a valid scenario using sequential type-checking`` (scenario) =
    compileAValidScenario scenario Method.Sequential

[<Fact>]
let ``Compile misordered scenario using graph-based type-checking fails`` () =
    compileScenario misorderedScenario Method.Graph
    |> shouldFail
    |> withErrorCodes [238; 248]
    |> ignore

[<Fact>]
let ``Compile misordered scenario using sequential type-checking fails`` () =
    compileScenario misorderedScenario Method.Sequential
    |> shouldFail
    |> withErrorCodes [238; 248]
    |> ignore
