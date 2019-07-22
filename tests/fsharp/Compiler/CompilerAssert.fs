// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open System.Diagnostics
open System.IO
open System.Text

open FSharp.Compiler.Text
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Interactive.Shell

open NUnit.Framework
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open System.Reflection.Emit

type TestError =
    {
        Number: int
        StartLine: int
        StartColumn: int
        EndLine: int
        EndColumn: int
        Message: string
    }

[<Sealed>]
type ILVerifier (dllFilePath: string) =

    member this.VerifyIL (qualifiedItemName: string, expectedIL: string) =
        ILChecker.checkILItem qualifiedItemName dllFilePath [ expectedIL ]

    member this.VerifyIL (expectedIL: string list) =
        ILChecker.checkIL dllFilePath expectedIL

    member this.VerifyILWithLineNumbers (qualifiedItemName: string, expectedIL: string) =
        ILChecker.checkILItemWithLineNumbers qualifiedItemName dllFilePath [ expectedIL ]

[<AbstractClass;Sealed>]
type CompilerAssert private () =

    static let checker = FSharpChecker.Create()
    static let config = TestFramework.initializeSuite ()

    static let defaultProjectOptions =
        {
            ProjectFileName = "Z:\\test.fsproj"
            ProjectId = None
            SourceFiles = [|"test.fs"|]
#if !NETCOREAPP
            OtherOptions = [|"--preferreduilang:en-US";|]
#else
            OtherOptions = 
                // Hack: Currently a hack to get the runtime assemblies for netcore in order to compile.
                let assemblies =
                    typeof<obj>.Assembly.Location
                    |> Path.GetDirectoryName
                    |> Directory.EnumerateFiles
                    |> Seq.toArray
                    |> Array.filter (fun x -> 
                        x.ToLowerInvariant().Contains("system.") || x.ToLowerInvariant().EndsWith("netstandard.dll") || x.ToLowerInvariant().EndsWith("mscorlib.dll"))
                    |> Array.map (fun x -> sprintf "-r:%s" x)
                Array.append [|"--preferreduilang:en-US"; "--targetprofile:netcore_private"; "--noframework"|] assemblies
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

    static let gate = obj ()

    static let compile extraArgs isExe source f =
        lock gate <| fun () ->
            let tmp1 = Path.GetTempFileName()
            let tmp2 = Path.GetTempFileName()

            let inputFilePath = Path.ChangeExtension(tmp1, ".fs")
            let outputFilePath = Path.ChangeExtension (tmp2, if isExe then ".exe" else ".dll")
            let runtimeConfigFilePath = Path.ChangeExtension (outputFilePath, ".runtimeconfig.json")
            let fsCoreDllPath = config.FSCOREDLLPATH
            let tmpFsCoreFilePath = Path.Combine (Path.GetDirectoryName(outputFilePath), Path.GetFileName(fsCoreDllPath))
            try
                File.Copy (fsCoreDllPath , tmpFsCoreFilePath, true)
                File.WriteAllText (inputFilePath, source)
                File.WriteAllText (runtimeConfigFilePath, """
{
  "runtimeOptions": {
    "tfm": "netcoreapp3.0",
    "framework": {
      "name": "Microsoft.NETCore.App",
      "version": "3.0.0-preview6-27804-01"
    }
  }
}
                """)

                let args =
                    defaultProjectOptions.OtherOptions
                    |> Array.append [| "fsc.exe"; inputFilePath; "-o:" + outputFilePath; (if isExe then "--target:exe" else "--target:library"); "--nowin32manifest" |]
                let errors, _ = checker.Compile (Array.append args extraArgs) |> Async.RunSynchronously

                f (errors, outputFilePath)

            finally
                try File.Delete tmp1 with | _ -> ()
                try File.Delete tmp2 with | _ -> ()

                try File.Delete inputFilePath with | _ -> ()
                try File.Delete outputFilePath with | _ -> ()
                try File.Delete runtimeConfigFilePath with | _ -> ()
                try File.Delete tmpFsCoreFilePath with | _ -> ()

    static let run outputExe =
        let pInfo = ProcessStartInfo ()
#if NETCOREAPP
        pInfo.FileName <- config.DotNetExe
        pInfo.Arguments <- outputExe
#else
        pInfo.FileName <- outputExe
#endif

        pInfo.RedirectStandardError <- true
        pInfo.RedirectStandardOutput <- true
        pInfo.UseShellExecute <- false
        
        let p = Process.Start(pInfo)

        p.WaitForExit()
        let errors = p.StandardError.ReadToEnd ()
        if not (String.IsNullOrWhiteSpace errors) then
            Assert.Fail errors

        p.StandardOutput.ReadToEnd ()

    static let compileWithOptions (source: string) (fsharpLanguageVersion: string) (compilation: CSharpCompilation) extraArgs isExe f =
        let tmp1 = Path.GetTempPath ()
        let fileName = compilation.AssemblyName + (if compilation.Options.OutputKind = OutputKind.DynamicallyLinkedLibrary then ".dll" else ".exe")
        let compilationOutputPath = Path.Combine (tmp1, fileName)
        try
            let csharpDiagnostics = compilation.GetDiagnostics ()

            if not csharpDiagnostics.IsEmpty then                  
                Assert.Fail ("CSharp Source Diagnostics:\n" + (csharpDiagnostics |> Seq.map (fun x -> x.GetMessage () + "\n") |> Seq.reduce (+)))

            let emitResult = compilation.Emit compilationOutputPath
                    
            Assert.IsTrue (emitResult.Success, "Unable to emit compilation.")

            compile (Array.append [|"--langversion:" + fsharpLanguageVersion; "-r:" + compilationOutputPath|] extraArgs) isExe source f
        finally
            try File.Delete tmp1 with | _ -> ()

            try File.Delete compilationOutputPath with | _ -> ()

    static member Compile (source: string, fsharpLanguageVersion: string, compilation: CSharpCompilation) =
        compileWithOptions source fsharpLanguageVersion compilation [||] false (fun (errors, _) -> 
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors)
        )

    static member Pass (source: string) =
        lock gate <| fun () ->
            let parseResults, fileAnswer = checker.ParseAndCheckFileInProject("test.fs", 0, SourceText.ofString source, defaultProjectOptions) |> Async.RunSynchronously

            Assert.IsEmpty(parseResults.Errors, sprintf "Parse errors: %A" parseResults.Errors)

            match fileAnswer with
            | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
            | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

            Assert.IsEmpty(typeCheckResults.Errors, sprintf "Type Check errors: %A" typeCheckResults.Errors)


    static member TypeCheckSingleError (source: string) (expectedErrorNumber: int) (expectedErrorRange: int * int * int * int) (expectedErrorMsg: string) =
        lock gate <| fun () ->
            let parseResults, fileAnswer = checker.ParseAndCheckFileInProject("test.fs", 0, SourceText.ofString source, defaultProjectOptions) |> Async.RunSynchronously

            Assert.IsEmpty(parseResults.Errors, sprintf "Parse errors: %A" parseResults.Errors)

            match fileAnswer with
            | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
            | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

            Assert.AreEqual(1, typeCheckResults.Errors.Length, sprintf "Expected one type check error: %A" typeCheckResults.Errors)
            typeCheckResults.Errors
            |> Array.iter (fun info ->
                Assert.AreEqual(FSharpErrorSeverity.Error, info.Severity)
                Assert.AreEqual(expectedErrorNumber, info.ErrorNumber, "expectedErrorNumber")
                Assert.AreEqual(expectedErrorRange, (info.StartLineAlternate, info.StartColumn, info.EndLineAlternate, info.EndColumn), "expectedErrorRange")
                Assert.AreEqual(expectedErrorMsg, info.Message, "expectedErrorMsg")
            )

    static member HasTypeCheckErrors (source: string, compilation, expectedErrors: TestError list, ?fsharpLanguageVersion) =
        let fsharpLanguageVersion = defaultArg fsharpLanguageVersion "default"
        compileWithOptions source fsharpLanguageVersion compilation [||] false (fun (errors, _) -> 
            let errors =
                errors 
                |> Array.map (fun err ->
                    let terr =
                        { 
                            Number = err.ErrorNumber
                            StartLine = err.StartLineAlternate
                            StartColumn = err.StartColumn
                            EndLine = err.EndLineAlternate
                            EndColumn = err.EndColumn
                            Message = err.Message
                        }
                    Assert.AreEqual(FSharpErrorSeverity.Error, err.Severity, sprintf "Expected error severity as Error. Actual error: %A" terr)
                    terr
                )

            Assert.Greater (errors.Length, 0, "Was expecting errors on type checking but there were none.")
            Assert.AreEqual (expectedErrors.Length, errors.Length, sprintf "The number of expected errors does not equal the number of actual errors. Actual errors: %A" errors)
            
            (expectedErrors, errors)
            ||> Seq.iter2 (fun expectedError actualError ->
                Assert.AreEqual(expectedError.Number, actualError.Number, sprintf "Expected error number does not equal the actual error number. Expected error: %A - Actual error: %A" expectedError actualError)
                Assert.AreEqual(expectedError.Message, actualError.Message, sprintf "Expected error message does not equal the actual error message. Expected error: %A - Actual error: %A" expectedError actualError)
                Assert.True ((expectedError = actualError), sprintf "Expected error ranges do not equal the actual error ranges. Expected error: %A - Actual error: %A" expectedError actualError)
            )
        )

    static member CompileExe (source: string) =
        compile [||] true source (fun (errors, _) -> 
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors))

    static member CompileExeAndRun (source: string) =
        compile [||] true source (fun (errors, outputExe) ->
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors)

            run outputExe |> ignore
        )

    static member CompileExeAndRun (source: string, compilation, expectedOutput: string, ?fsharpLanguageVersion) =
        let fsharpLanguageVersion = defaultArg fsharpLanguageVersion "default"
        compileWithOptions source fsharpLanguageVersion compilation [||] true (fun (errors, outputExe) ->
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors)

            Assert.AreEqual (expectedOutput, run outputExe)
        )

    static member CompileAndVerifyIL (source: string) (f: ILVerifier -> unit) =
        compile [||] false source (fun (errors, outputFilePath) -> 
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors)

            f (ILVerifier outputFilePath)
        )
 
    static member RunScript (source: string) (expectedErrorMessages: string list) =
        lock gate <| fun () ->
            // Intialize output and input streams
            use inStream = new StringReader("")
            use outStream = new StringWriter()
            use errStream = new StringWriter()

            // Build command line arguments & start FSI session
            let argv = [| "C:\\fsi.exe" |]
    #if !NETCOREAPP
            let allArgs = Array.append argv [|"--noninteractive"|]
    #else
            let allArgs = Array.append argv [|"--noninteractive"; "--targetprofile:netcore"|]
    #endif

            let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
            use fsiSession = FsiEvaluationSession.Create(fsiConfig, allArgs, inStream, outStream, errStream, collectible = true)
            
            let ch, errors = fsiSession.EvalInteractionNonThrowing source

            let errorMessages = ResizeArray()
            errors
            |> Seq.iter (fun error -> errorMessages.Add(error.Message))

            match ch with
            | Choice2Of2 ex -> errorMessages.Add(ex.Message)
            | _ -> ()

            if expectedErrorMessages.Length <> errorMessages.Count then
                Assert.Fail(sprintf "Expected error messages: %A \n\n Actual error messages: %A" expectedErrorMessages errorMessages)
            else
                (expectedErrorMessages, errorMessages)
                ||> Seq.iter2 (fun expectedErrorMessage errorMessage ->
                    Assert.AreEqual(expectedErrorMessage, errorMessage)
                )
