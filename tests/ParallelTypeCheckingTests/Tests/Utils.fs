module ParallelTypeCheckingTests.TestUtils

open System
open FSharp.Compiler
open FSharp.Compiler.CompilerConfig
open ParallelTypeCheckingTests
open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open OpenTelemetry
open OpenTelemetry.Resources
open OpenTelemetry.Trace

let CodeRoot =
    @$"{__SOURCE_DIRECTORY__}\.checkouts\fcs"
let replaceCodeRoot (s : string) = s.Replace("$CODE_ROOT$", CodeRoot)
let packages =
    let pathWithEnv = @"%USERPROFILE%\.nuget\packages"
    Environment.ExpandEnvironmentVariables(pathWithEnv);
let replacePaths (s : string) =
    s
    |> replaceCodeRoot
    |> fun s -> s.Replace("$PACKAGES$", packages)

[<Struct>]
type Method =
    | Sequential
    | ParallelCheckingOfBackedImplFiles
    | Graph

let methods =
    [
        Method.Sequential
        Method.Graph
    ]

let setupOtel () =
    Sdk
        .CreateTracerProviderBuilder()
        .AddSource("fsc")
        .SetResourceBuilder(
            ResourceBuilder
                .CreateDefault()
                .AddService (serviceName = "program", serviceVersion = "42.42.42.44")
        )
        .AddJaegerExporter(fun c ->
            c.BatchExportProcessorOptions.MaxQueueSize <- 10000000
            c.BatchExportProcessorOptions.MaxExportBatchSize <- 10000000
            c.ExportProcessorType <- ExportProcessorType.Simple
            c.MaxPayloadSizeInBytes <- Nullable (1000000000)
        )
        .Build ()

type internal Args =
    {
        Path : string
        LineLimit : int option
        Method : Method
        WorkingDir : string option
    }

let makeCompilationUnit (files : (string * string) list) : CompilationUnit =
    let files = files |> List.map (fun (name, code) -> SourceCodeFileKind.Create(name, code))
    match files with
    | [] -> failwith "empty files"
    | first :: rest ->
        let f = fsFromString first |> FS
        f
        |> withAdditionalSourceFiles rest

let internal mapMethod (method : Method) =
    match method with
    | Method.Sequential -> TypeCheckingMode.Sequential
    | Method.ParallelCheckingOfBackedImplFiles -> TypeCheckingMode.ParallelCheckingOfBackedImplFiles
    | Method.Graph -> TypeCheckingMode.Graph

/// Includes mutation of static config
/// A very hacky way to setup the given type-checking method - mutates static state and returns new args
/// TODO Make the method configurable via proper config passed top-down
let setupCompilationMethod (method: Method) =
    printfn $"Method: {method}"
    let mode = mapMethod method
    ParseAndCheckInputs.CheckMultipleInputsUsingGraphMode <- ParallelTypeChecking.CheckMultipleInputsInParallel
    ParseAndCheckInputs.typeCheckingMode <- mode
