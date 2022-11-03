module ParallelTypeCheckingTests.Utils

open System
open FSharp.Compiler
open FSharp.Compiler.Service.Tests
open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open OpenTelemetry
open OpenTelemetry.Resources
open OpenTelemetry.Trace

let CodeRoot =
    System.IO.Path.Combine(
        __SOURCE_DIRECTORY__,
        "../../../"
    )
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
    | ParallelFs
    | Graph
    | Nojaf

let methods =
    [
        Method.Sequential
        Method.ParallelFs
        Method.Nojaf
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

type Args =
    {
        Path : string
        Mode : Method
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

/// Includes mutation of static config
let setupCompilationMethod (method: Method) (x: CompilationUnit): CompilationUnit =
    printfn $"Method: {method}"
    match method with
        | Method.Sequential ->
            x
        | Method.ParallelFs ->
            x
            |> withOptions [ "--test:ParallelCheckingWithSignatureFilesOn" ]
        | Method.Graph ->
            ParseAndCheckInputs.CheckMultipleInputsInParallel2 <- ParallelTypeChecking.Real.CheckMultipleInputsInParallel
            x
            |> withOptions [ "--test:ParallelCheckingWithSignatureFilesOn" ]
        | Method.Nojaf ->
            ParseAndCheckInputs.CheckMultipleInputsInParallel2 <- ParallelTypeChecking.Nojaf.CheckMultipleInputsInParallel
            x
            |> withOptions [ "--test:ParallelCheckingWithSignatureFilesOn" ]
