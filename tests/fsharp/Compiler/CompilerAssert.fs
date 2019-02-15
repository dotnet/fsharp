// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open System.IO
open System.Text
open System.Diagnostics
open FSharp.Compiler.Text
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Interactive.Shell

open NUnit.Framework

[<RequireQualifiedAccess>]
module CompilerAssert =

    let checker = FSharpChecker.Create()

    let private config = TestFramework.initializeSuite ()

    let private defaultProjectOptions =
        {
            ProjectFileName = "Z:\\test.fsproj"
            ProjectId = None
            SourceFiles = [|"test.fs"|]
            OtherOptions = [||]
            ReferencedProjects = [||]
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = false
            LoadTime = DateTime()
            UnresolvedReferences = None
            OriginalLoadReferences = []
            ExtraProjectInfo = None
            Stamp = None
        }

    let private exec exe args =
        let startInfo = ProcessStartInfo(exe, String.concat " " args)
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false
        use p = Process.Start(startInfo)
        p.WaitForExit()
        p.StandardOutput.ReadToEnd(), p.StandardError.ReadToEnd(), p.ExitCode

    let Pass (source: string) =
        let parseResults, fileAnswer = checker.ParseAndCheckFileInProject("test.fs", 0, SourceText.ofString source, defaultProjectOptions) |> Async.RunSynchronously

        Assert.True(parseResults.Errors.Length = 0, sprintf "Parse errors: %A" parseResults.Errors)

        match fileAnswer with
        | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
        | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

        Assert.True(typeCheckResults.Errors.Length = 0, sprintf "Type Check errors: %A" typeCheckResults.Errors)

    let TypeCheckSingleError (source: string) (expectedErrorNumber: int) (expectedErrorRange: int * int * int * int) (expectedErrorMsg: string) =
        let parseResults, fileAnswer = checker.ParseAndCheckFileInProject("test.fs", 0, SourceText.ofString source, defaultProjectOptions) |> Async.RunSynchronously

        Assert.True(parseResults.Errors.Length = 0, sprintf "Parse errors: %A" parseResults.Errors)

        match fileAnswer with
        | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
        | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

        Assert.True(typeCheckResults.Errors.Length = 1, sprintf "Expected one type check error: %A" typeCheckResults.Errors)
        typeCheckResults.Errors
        |> Array.iter (fun info ->
            Assert.AreEqual(FSharpErrorSeverity.Error, info.Severity)
            Assert.AreEqual(expectedErrorNumber, info.ErrorNumber, "expectedErrorNumber")
            Assert.AreEqual(expectedErrorRange, (info.StartLineAlternate, info.StartColumn, info.EndLineAlternate, info.EndColumn), "expectedErrorRange")
            Assert.AreEqual(expectedErrorMsg, info.Message, "expectedErrorMsg")
        )

    let RunScript (source: string) (expectedOutput: string) =
        let tmp = Path.GetTempFileName()
        let tmpFsx = Path.ChangeExtension(tmp, ".fsx")

        try
            File.WriteAllText(tmpFsx, source)
            let output, errors, exitCode = exec config.FSI [tmpFsx]

            if errors.Length > 0 || exitCode <> 0 then
                Assert.Fail(sprintf "Exit Code: %i" exitCode)
                Assert.Fail(sprintf "%A" errors)
                
            Assert.AreEqual(expectedOutput, output)

        finally
            try File.Delete(tmp) with | _ -> ()
            try File.Delete(tmpFsx) with | _ -> ()