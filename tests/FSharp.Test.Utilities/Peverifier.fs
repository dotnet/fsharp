// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Test

open FSharp.Test.Compiler
open System
open System.IO
open TestFramework

type PEVerifyOutput =
    {
        ExitCode: int
        Lines:   string array
    }

[<RequireQualifiedAccess>]
type PEVerifyResult =
    | Success of PEVerifyOutput
    | Failure of PEVerifyOutput

[<RequireQualifiedAccess>]
module PEVerifier =

    let config = initializeSuite ()

    let private exec exe args =
        let arguments = args |> String.concat " "
        let timeout = 30000
        let exitCode, _output, errors = Commands.executeProcess (Some exe) arguments "" timeout
        let errors = errors |> String.concat Environment.NewLine
        errors, exitCode

    let private verifyPEFileCore peverifierArgs dllFilePath =
        let mutable errors = ResizeArray ()
        let peverifierPath = config.PEVERIFY
        let peverifyFullArgs = [ yield dllFilePath; yield "/NOLOGO"; yield! peverifierArgs  ]
        let stdErr, exitCode =
            let peverifierCommandPath = Path.ChangeExtension(dllFilePath, ".peverifierCommandPath")
            File.WriteAllLines(peverifierCommandPath, [| $"{peverifierPath} {peverifyFullArgs}" |] )
            File.Copy(typeof<RequireQualifiedAccessAttribute>.Assembly.Location, Path.GetDirectoryName(dllFilePath) ++ "FSharp.Core.dll", true)
            exec peverifierPath peverifyFullArgs

        if exitCode <> 0 then
            errors.Add (sprintf "PEVERIFIER failed with error code: %d" exitCode)

        if not (String.IsNullOrWhiteSpace stdErr) then
            errors.Add (sprintf "PEVERIFIER stderr is not empty:\n %s" stdErr)

        let error = { ExitCode=exitCode; Lines = errors.ToArray() }

        match errors.Count with
        | 0 -> PEVerifyResult.Success error
        | _ -> PEVerifyResult.Failure error

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

    let shouldFail result =
        match result with
        | PEVerifyResult.Success _ -> failwith $"Expected to Fail - {result}"
        | PEVerifyResult.Failure _ -> ()

    let shouldSucceed result =
        match result with
        | PEVerifyResult.Success _ -> ()
        | PEVerifyResult.Failure _ -> failwith $"Expected to Succeed - {result}"
