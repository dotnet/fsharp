module TypeChecks.CompilationTests

open System
open System.IO
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
    |> withOptions scenario.CompilerOptions
    |> withMethod method
    |> compile

let compileAValidScenario (scenario: Scenario) (method: Method) =
    compileScenario scenario method
    |> shouldSucceed
    |> ignore

let scenarios = compilingScenarios |> List.map (fun c -> [| box c |])

/// The dependencies in the graph the compiler writes for `--test:DumpCheckingGraph`.
let dumpedDependencies (result: CompilationResult) =
    Path.ChangeExtension(result.OutputPath.Value, ".graph.md")
    |> File.ReadAllLines
    |> Array.choose (fun line ->
        match line.Trim().Split([| " --> " |], StringSplitOptions.None) with
        | [| file; dependency |] -> Some(int file, int dependency)
        | _ -> None)

[<Theory>]
[<MemberData(nameof scenarios)>]
let ``Compile a valid scenario using graph-based type-checking`` (scenario) =
    let dependencies =
        compileScenario scenario Method.Graph |> shouldSucceed |> dumpedDependencies

    for file in scenario.Files do
        let actualDependencies =
            dependencies
            |> Array.filter (fun (idx, _) -> idx = file.Index)
            |> Array.map snd
            |> set

        Assert.True(
            (file.ExpectedDependencies = actualDependencies),
            $"Dependencies don't match for {Path.GetFileName file.FileName}: {actualDependencies}"
        )

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
