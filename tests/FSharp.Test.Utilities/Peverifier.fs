// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Test

open FSharp.Test.Compiler
open System
open System.IO
open TestFramework

type PEVerifyOutput ={
        ExitCode: int
        OutputLines: string array
        ErrorLines: string array
    }

[<RequireQualifiedAccess>]
type PEVerifyResult =
    | Success of PEVerifyOutput
    | Failure of PEVerifyOutput

[<RequireQualifiedAccess>]
module PEVerifier =
    let config = initializeSuite ()

    let private exec (dotnetExe: string) args workingDirectory =
        let arguments = args |> String.concat " "
        let exitCode, _output, errors = Commands.executeProcess dotnetExe arguments workingDirectory
        let errors = errors |> String.concat Environment.NewLine
        errors, exitCode

    let private verifyPEFileCore peverifierArgs (dllFilePath: string) =
        let mutable errors = ResizeArray ()
        let peverifyFullArgs = [ yield "--verbose"; yield dllFilePath; yield! peverifierArgs  ]
        let workingDirectory = Path.GetDirectoryName dllFilePath
        let _, exitCode =
            let peverifierCommandPath = Path.ChangeExtension(dllFilePath, ".peverifierCommandPath.cmd")
            let args = peverifyFullArgs |> Seq.fold(fun a acc -> $"{a} " + acc) ""
            File.WriteAllLines(peverifierCommandPath, [| $"{args}" |] )
            File.Copy(typeof<RequireQualifiedAccessAttribute>.Assembly.Location, Path.GetDirectoryName(dllFilePath) ++ "FSharp.Core.dll", true)
            let profile = Environment.GetEnvironmentVariable("USERPROFILE")
            exec $"{profile}/.dotnet/tools/ilverify.exe" peverifyFullArgs workingDirectory

        // Grab output
        let outputLines = File.ReadAllLines(Path.Combine(workingDirectory, "StandardOutput.txt"))
        let errorLines = File.ReadAllLines(Path.Combine(workingDirectory, "StandardError.txt"))

        let result = {
            ExitCode = exitCode
            OutputLines = outputLines
            ErrorLines = errorLines
        }

        if result.ExitCode <> 0 then
            errors.Add ($"PEVERIFIER failed with error code: {exitCode}")

        if result.ErrorLines |> Array.length <= 0 then
            errors.Add ($"PEVERIFIER stderr is not empty:\n {result.ErrorLines}")

        match errors.Count with
        | 0 -> PEVerifyResult.Success result
        | _ -> PEVerifyResult.Failure result

    let private verifyPEFileAux cUnit args =
        let result =
            match cUnit with
            | FS fs ->
                match compile cUnit, fs.OutputFileName  with
                | CompilationResult.Success _, Some name ->
                    verifyPEFileCore args name
                | failed ->
                    failwith $"""Compilation must succeed in order to verify IL.{failed}"""
            | _ ->
                failwith "PEVerify is only supported for F#."
        result

    let verifyPEFile cUnit =
        verifyPEFileAux cUnit [||]

    let verifyPEFileWithArgs cUnit args =
        verifyPEFileCore cUnit args

    let verifyPEFileWithSystemDlls cUnit =
        // Get the path containing mecorlib.dll or System.Core.Private.dll
        let fsharpCorePath = typeof<unit>.Assembly.Location
        let systemPath = Path.GetDirectoryName(typeof<obj>.Assembly.Location)
        let systemDllPaths =
            DirectoryInfo(systemPath).GetFiles("*.dll")
            |> Array.map (fun dll -> $"--reference \"{Path.Combine(systemPath, dll.FullName)}\"")
            |> Array.toList
        verifyPEFileAux cUnit ($"--reference \"{fsharpCorePath}\"" :: systemDllPaths)

    let shouldFail result =
        match result with
        | PEVerifyResult.Success _ -> failwith $"Expected to Fail - {result}"
        | PEVerifyResult.Failure _ -> ()

    let shouldSucceed result =
        match result with
        | PEVerifyResult.Success _ -> ()
        | PEVerifyResult.Failure _ -> failwith $"Expected to Succeed - {result}"
