// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open System.IO
open System.Text
open FSharp.Compiler.Text
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Interactive.Shell

open NUnit.Framework

[<RequireQualifiedAccess>]
module CompilerAssert =

    let checker = FSharpChecker.Create()

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

    let private createFsiSession () =
        // Intialize output and input streams
        let sbOut = new StringBuilder()
        let sbErr = new StringBuilder()
        let inStream = new StringReader("")
        let outStream = new StringWriter(sbOut)
        let errStream = new StringWriter(sbErr)

        // Bmand line arguments & start FSI session
        let argv = [| "C:\\fsi.exe" |]
        let allArgs = Array.append argv [|"--noninteractive"|]

        let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
        FsiEvaluationSession.Create(fsiConfig, allArgs, inStream, outStream, errStream)

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

    let RunScript (source: string) (expectedOutput: 'T option) =
        let tmp = Path.GetTempFileName()
        let tmpFsx = Path.ChangeExtension(tmp, ".fsx")

        try
            File.WriteAllText(tmpFsx, source)

            use fsiSession = createFsiSession ()

            let _, errors = fsiSession.EvalScriptNonThrowing(tmpFsx)

            if errors.Length > 0 then
                Assert.Fail(sprintf "%A" errors)

            //match fsiSession.EvalExpressionNonThrowing(source) with
            //| _, errors when errors.Length > 0 -> Assert.Fail(errors |> List.ofArray |> string)
            //| Choice.Choice1Of2(result), _ ->
            //    match result, expectedOutput with
            //    | Some fsiValue, Some expectedOutput -> Assert.AreEqual(fsiValue.ReflectionValue, expectedOutput)
            //    | Some fsiValue, None ->                Assert.AreEqual(null, fsiValue.ReflectionValue)
            //    | None, Some expectedOutput ->          Assert.AreEqual(expectedOutput, null)
            //    | _ -> ()
            //| _ -> ()

        finally
            try File.Delete(tmp) with | _ -> ()
            try File.Delete(tmpFsx) with | _ -> ()