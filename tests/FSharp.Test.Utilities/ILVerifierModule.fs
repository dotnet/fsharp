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
        let exitCode, _output, errors = Commands.executeProcess dotnetExe arguments workingDirectory
        let errors = errors |> String.concat Environment.NewLine
        errors, exitCode

    let private verifyPEFileCore peverifierArgs (dllFilePath: string) =
        let nuget_packages =
            match Environment.GetEnvironmentVariable("NUGET_PACKAGES") with
            | null ->
                let profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                $"""{profile}/.nuget/packages"""
            | path -> path
        let peverifyFullArgs = [ yield "exec"; yield $"""{nuget_packages}/dotnet-ilverify/9.0.0/tools/net9.0/any/ILVerify.dll"""; yield "--verbose"; yield dllFilePath; yield! peverifierArgs ]
        let workingDirectory = Path.GetDirectoryName dllFilePath
        let _, exitCode =
            let peverifierCommandPath = Path.ChangeExtension(dllFilePath, ".peverifierCommandPath.cmd")
            let args = peverifyFullArgs |> Seq.fold(fun a acc -> $"{a} " + acc) ""
            File.WriteAllLines(peverifierCommandPath, [| $"{args}" |] )
            File.Copy(typeof<RequireQualifiedAccessAttribute>.Assembly.Location, Path.GetDirectoryName(dllFilePath) ++ "FSharp.Core.dll", true)
            exec config.DotNetExe peverifyFullArgs workingDirectory

        // Grab output
        let outputText = File.ReadAllText(Path.Combine(workingDirectory, "StandardOutput.txt"))
        let errorText = File.ReadAllText(Path.Combine(workingDirectory, "StandardError.txt"))

        match exitCode with
        | 0 -> {Outcome = NoExitCode; StdOut = outputText; StdErr = errorText } 
        | _ -> {Outcome = ExitCode exitCode; StdOut = outputText; StdErr = errorText } 

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
        verifyPEFileAux compilationResult systemDllReferences
