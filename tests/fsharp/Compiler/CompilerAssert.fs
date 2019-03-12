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
#if NET472
            OtherOptions = [||]
#else
            OtherOptions = 
                // Hack: Currently a hack to get the runtime assemblies for netcore in order to compile.
                let assemblies =
                    typeof<obj>.Assembly.Location
                    |> Path.GetDirectoryName
                    |> Directory.EnumerateFiles
                    |> Seq.toArray
                    |> Array.filter (fun x -> x.ToLowerInvariant().Contains("system."))
                    |> Array.map (fun x -> sprintf "-r:%s" x)
                Array.append [|"--targetprofile:netcore"; "--noframework"|] assemblies
#endif
            ReferencedProjects = [||]
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = false
            LoadTime = DateTime()
            UnresolvedReferences = None
            OriginalLoadReferences = []
            ExtraProjectInfo = None
            Stamp = None
        }

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

    let RunScript (source: string) =
        // Intialize output and input streams
        let sbOut = new StringBuilder()
        let sbErr = new StringBuilder()
        use inStream = new StringReader("")
        use outStream = new StringWriter(sbOut)
        use errStream = new StringWriter(sbErr)

        // Build command line arguments & start FSI session
        let argv = [| "C:\\fsi.exe" |]
#if NET472
        let allArgs = Array.append argv [|"--noninteractive"|]
#else
        let allArgs = Array.append argv [|"--noninteractive"; "--targetprofile:netcore"|]
#endif

        let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
        use fsiSession = FsiEvaluationSession.Create(fsiConfig, allArgs, inStream, outStream, errStream)
        
        let ch, errors = fsiSession.EvalInteractionNonThrowing source

        if errors.Length > 0 then
            Assert.Fail(sprintf "%A" errors)

        match ch with
        | Choice2Of2 ex -> Assert.Fail(ex.Message)
        | _ -> ()