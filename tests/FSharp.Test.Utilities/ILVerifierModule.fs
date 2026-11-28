// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Test

open FSharp.Test.Compiler
open System
open System.IO
open TestFramework

[<AutoOpen>]
module ILVerifierModule =
    let config = initialConfig

    let fsharpCoreReference = $"--reference \"{typeof<unit>.Assembly.Location}\""

    let private systemDllReferences =
        // Get the path containing mecorlib.dll or System.Core.Private.dll
        let refs =
            let systemPath = Path.GetDirectoryName(typeof<obj>.Assembly.Location)
            DirectoryInfo(systemPath).GetFiles("*.dll")
            |> Array.map (fun dll -> $"--reference \"{Path.Combine(systemPath, dll.FullName)}\"")
            |> Array.toList
        (fsharpCoreReference :: refs)

    let private exec (dotnetExe: string) args workingDirectory =
        let arguments = args |> String.concat " "
        Commands.executeProcess dotnetExe arguments workingDirectory

    let private verifyPEFileCore peverifierArgs (dllFilePath: string) =
        // Resolve ilverify through the local tool manifest (.config/dotnet-tools.json) rather than a
        // hard-coded NuGet cache path. `dotnet tool run` locates the tool wherever it was restored, so
        // verification does not depend on the NuGet cache layout, tool version, or target framework.
        let peverifyFullArgs = [ yield "tool"; yield "run"; yield "ilverify"; yield "--"; yield "--verbose"; yield dllFilePath; yield! peverifierArgs ]
        let workingDirectory = Path.GetDirectoryName dllFilePath
        let exitCode, outputText, errorText =
            let peverifierCommandPath = Path.ChangeExtension(dllFilePath, ".peverifierCommandPath.cmd")
            let args = peverifyFullArgs |> Seq.fold(fun a acc -> $"{a} " + acc) ""
            File.WriteAllLines(peverifierCommandPath, [| $"{args}" |] )
            File.Copy(typeof<RequireQualifiedAccessAttribute>.Assembly.Location, Path.GetDirectoryName(dllFilePath) ++ "FSharp.Core.dll", true)
            exec config.DotNetExe peverifyFullArgs workingDirectory

        match exitCode with
        | 0 -> {Outcome = NoExitCode; StdOut = outputText; StdErr = errorText }
        | _ -> {Outcome = ExitCode exitCode; StdOut = outputText; StdErr = errorText }

    let private referenceDllReferences (compilationResult: CompilationResult) =
        // The compiled F# references (from withReferences) are recorded as absolute
        // paths in CompilationOutput.Dependencies. Surface them to the verifier so
        // cross-assembly references (e.g. a realsig library) resolve at verification time.
        compilationResult.Output.Dependencies
        |> List.filter (fun p -> not (String.IsNullOrWhiteSpace p))
        |> List.map (fun p -> $"--reference \"{p}\"")

    let private verifyPEFileAux (compilationResult: CompilationResult) args =
        let result =
            match compilationResult.Compilation with
            | FS _ ->
                match compilationResult, compilationResult.OutputPath with
                | CompilationResult.Success result, Some name ->
                    let verifyResult = verifyPEFileCore args name
                    match verifyResult.Outcome with
                    | NoExitCode -> CompilationResult.Success {result with Output = Some (ExecutionOutput verifyResult)}
                    | ExitCode _ ->  CompilationResult.Failure {result with Output = Some (ExecutionOutput verifyResult)}
                    | failed -> failwith $"Compilation must succeed in order to verify IL.{failed}"
                | failed ->
                    failwith $"""Compilation must succeed in order to verify IL.{failed}"""
            | _ ->
                failwith "PEVerify is only supported for F#."
        result

    let verifyPEFile compilationResult =
        verifyPEFileAux compilationResult [| fsharpCoreReference |]

    let verifyPEFileWithArgs compilationResult args =
        verifyPEFileAux compilationResult (fsharpCoreReference :: args)

    let verifyPEFileWithSystemDlls compilationResult =
        // Include the compilation's own F# references (e.g. a library built via
        // withReferences) so cross-assembly resolution succeeds during verification.
        verifyPEFileAux compilationResult (referenceDllReferences compilationResult @ systemDllReferences)

    let verifyPEFileWithSystemDllsOn dllPath =
        // Verify a specific PE file (e.g. a referenced library) with the system dlls + FSharp.Core on the reference path.
        verifyPEFileCore (fsharpCoreReference :: systemDllReferences) dllPath
