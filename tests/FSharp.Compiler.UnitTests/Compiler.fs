// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open Microsoft.FSharp.Compiler.Text
open Microsoft.FSharp.Compiler.SourceCodeServices

open NUnit.Framework

[<RequireQualifiedAccess>]
module Compiler =

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

    let AssertPass (source: string) =
        let parseResults, fileAnswer = checker.ParseAndCheckFileInProject("test.fs", 0, SourceText.ofString source, defaultProjectOptions) |> Async.RunSynchronously

        Assert.True(parseResults.Errors.Length = 0, sprintf "Parse errors: %A" parseResults.Errors)

        match fileAnswer with
        | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
        | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

        Assert.True(typeCheckResults.Errors.Length = 0, sprintf "Type Check errors: %A" typeCheckResults.Errors)

    let AssertSingleErrorTypeCheck (source: string) (expectedErrorNumber: int) (expectedErrorRange: int * int * int * int) (expectedErrorMsg: string) =
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