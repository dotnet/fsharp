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
open System.Reflection.Emit

[<Sealed>]
type ILVerifier (dllFilePath: string) =

    member this.VerifyIL (qualifiedItemName: string, expectedIL: string) =
        ILChecker.checkILItem qualifiedItemName dllFilePath [ expectedIL ]

    member this.VerifyIL (expectedIL: string list) =
        ILChecker.checkIL dllFilePath expectedIL

    member this.VerifyILWithLineNumbers (qualifiedItemName: string, expectedIL: string) =
        ILChecker.checkILItemWithLineNumbers qualifiedItemName dllFilePath [ expectedIL ]

[<RequireQualifiedAccess>]
module CompilerAssert =

    let checker = FSharpChecker.Create()
    let private config = TestFramework.initializeSuite ()


// Do a one time dotnet sdk build to compute the proper set of reference assemblies to pass to the compiler
#if !NETCOREAPP
#else
    let projectFile = """
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.1</TargetFramework>
  </PropertyGroup>

  <ItemGroup><Compile Include="Program.fs" /></ItemGroup>

  <Target Name="WriteFrameworkReferences" AfterTargets="AfterBuild">
    <WriteLinesToFile File="FrameworkReferences.txt" Lines="@(ReferencePath)" Overwrite="true" WriteOnlyWhenDifferent="true" />
  </Target>

</Project>"""

    let programFs = """
open System

[<EntryPoint>]
let main argv = 0"""

    let getNetCoreAppReferences =
        let mutable output = ""
        let mutable errors = ""
        let mutable cleanUp = true
        let projectDirectory = Path.Combine(Path.GetTempPath(), "netcoreapp2.1", Path.GetRandomFileName())
        try
            try
                Directory.CreateDirectory(projectDirectory) |> ignore
                let projectFileName = Path.Combine(projectDirectory, "ProjectFile.fsproj")
                let programFsFileName = Path.Combine(projectDirectory, "Program.fs")
                let frameworkReferencesFileName = Path.Combine(projectDirectory, "FrameworkReferences.txt")

                File.WriteAllText(projectFileName, projectFile)
                File.WriteAllText(programFsFileName, programFs)

                let pInfo = ProcessStartInfo ()

                pInfo.FileName <- config.DotNetExe
                pInfo.Arguments <- "build"
                pInfo.WorkingDirectory <- projectDirectory
                pInfo.RedirectStandardOutput <- true
                pInfo.RedirectStandardError <- true
                pInfo.UseShellExecute <- false

                let p = Process.Start(pInfo)
                p.WaitForExit()

                output <- p.StandardOutput.ReadToEnd ()
                errors <- p.StandardError.ReadToEnd ()
                if not (String.IsNullOrWhiteSpace errors) then Assert.Fail errors

                if p.ExitCode <> 0 then Assert.Fail(sprintf "Program exited with exit code %d" p.ExitCode)

                File.ReadLines(frameworkReferencesFileName) |> Seq.toArray
            with | e ->
                cleanUp <- false
                printfn "%s" output
                printfn "%s" errors
                raise (new Exception (sprintf "An error occured getting netcoreapp references: %A" e))
        finally
            if cleanUp then
                try Directory.Delete(projectDirectory) with | _ -> ()
#endif

    let private defaultProjectOptions =
        {
            ProjectFileName = "Z:\\test.fsproj"
            ProjectId = None
            SourceFiles = [|"test.fs"|]
#if !NETCOREAPP
            OtherOptions = [|"--preferreduilang:en-US";|]
#else
            OtherOptions = 
                let assemblies = getNetCoreAppReferences |> Array.map (fun x -> sprintf "-r:%s" x)
                Array.append [|"--preferreduilang:en-US"; "--targetprofile:netcore"; "--noframework"|] assemblies
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

    let private gate = obj ()

    let private compile isExe source f =
        lock gate <| fun () ->
            let inputFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".fs")
            let outputFilePath = Path.ChangeExtension (Path.GetTempFileName(), if isExe then ".exe" else ".dll")
            let runtimeConfigFilePath = Path.ChangeExtension (outputFilePath, ".runtimeconfig.json")
            let fsCoreDllPath = config.FSCOREDLLPATH
            let tmpFsCoreFilePath = Path.Combine (Path.GetDirectoryName(outputFilePath), Path.GetFileName(fsCoreDllPath))
            try
                File.Copy (fsCoreDllPath , tmpFsCoreFilePath, true)
                File.WriteAllText (inputFilePath, source)
                File.WriteAllText (runtimeConfigFilePath, """
{
  "runtimeOptions": {
    "tfm": "netcoreapp2.1",
    "framework": {
      "name": "Microsoft.NETCore.App",
      "version": "2.1.0"
    }
  }
}
                """)

                let args =
                    defaultProjectOptions.OtherOptions
                    |> Array.append [| "fsc.exe"; inputFilePath; "-o:" + outputFilePath; (if isExe then "--target:exe" else "--target:library"); "--nowin32manifest" |]
                let errors, _ = checker.Compile args |> Async.RunSynchronously

                f (errors, outputFilePath)

            finally
                try File.Delete inputFilePath with | _ -> ()
                try File.Delete outputFilePath with | _ -> ()
                try File.Delete runtimeConfigFilePath with | _ -> ()
                try File.Delete tmpFsCoreFilePath with | _ -> ()

    let Pass (source: string) =
        lock gate <| fun () ->
            let parseResults, fileAnswer = checker.ParseAndCheckFileInProject("test.fs", 0, SourceText.ofString source, defaultProjectOptions) |> Async.RunSynchronously

            Assert.IsEmpty(parseResults.Errors, sprintf "Parse errors: %A" parseResults.Errors)

            match fileAnswer with
            | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
            | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

            Assert.IsEmpty(typeCheckResults.Errors, sprintf "Type Check errors: %A" typeCheckResults.Errors)

    let TypeCheckWithErrorsAndOptions options (source: string) expectedTypeErrors =
        lock gate <| fun () ->
            let parseResults, fileAnswer = 
                checker.ParseAndCheckFileInProject(
                    "test.fs",
                    0,
                    SourceText.ofString source,
                    { defaultProjectOptions with OtherOptions = Array.append options defaultProjectOptions.OtherOptions}) 
                |> Async.RunSynchronously

            Assert.IsEmpty(parseResults.Errors, sprintf "Parse errors: %A" parseResults.Errors)

            match fileAnswer with
            | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
            | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

            let errors = 
                typeCheckResults.Errors
                |> Array.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLineAlternate, e.StartColumn, e.EndLineAlternate, e.EndColumn, e.Message)

            Assert.AreEqual(Array.length expectedTypeErrors, errors.Length, sprintf "Type check errors: %A" typeCheckResults.Errors)

            Array.zip errors expectedTypeErrors
            |> Array.iter (fun (info, expectedError) ->
                let (expectedServerity: FSharpErrorSeverity, expectedErrorNumber: int, expectedErrorRange: int * int * int * int, expectedErrorMsg: string) = expectedError
                Assert.AreEqual(expectedServerity, info.Severity)
                Assert.AreEqual(expectedErrorNumber, info.ErrorNumber, "expectedErrorNumber")
                Assert.AreEqual(expectedErrorRange, (info.StartLineAlternate, info.StartColumn + 1, info.EndLineAlternate, info.EndColumn + 1), "expectedErrorRange")
                Assert.AreEqual(expectedErrorMsg, info.Message, "expectedErrorMsg")
            )

    let TypeCheckWithErrors (source: string) expectedTypeErrors =
        TypeCheckWithErrorsAndOptions [||] source expectedTypeErrors

    let TypeCheckSingleErrorWithOptions options (source: string) (expectedServerity: FSharpErrorSeverity) (expectedErrorNumber: int) (expectedErrorRange: int * int * int * int) (expectedErrorMsg: string) =
        TypeCheckWithErrorsAndOptions options source [| expectedServerity, expectedErrorNumber, expectedErrorRange, expectedErrorMsg |]

    let TypeCheckSingleError (source: string) (expectedServerity: FSharpErrorSeverity) (expectedErrorNumber: int) (expectedErrorRange: int * int * int * int) (expectedErrorMsg: string) =
        TypeCheckWithErrors source [| expectedServerity, expectedErrorNumber, expectedErrorRange, expectedErrorMsg |]

    let CompileExe (source: string) =
        compile true source (fun (errors, _) ->
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors))

    let CompileExeAndRun (source: string) =
        compile true source (fun (errors, outputExe) ->

            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors)

            let pInfo = ProcessStartInfo ()
#if NETCOREAPP
            pInfo.FileName <- config.DotNetExe
            pInfo.Arguments <- outputExe
#else
            pInfo.FileName <- outputExe
#endif

            pInfo.RedirectStandardError <- true
            pInfo.UseShellExecute <- false
            
            let p = Process.Start(pInfo)

            p.WaitForExit()
            let errors = p.StandardError.ReadToEnd ()
            if not (String.IsNullOrWhiteSpace errors) then
                Assert.Fail errors

            if p.ExitCode <> 0 then
                Assert.Fail(sprintf "Program exited with exit code %d" p.ExitCode)
        )

    let CompileLibraryAndVerifyIL (source: string) (f: ILVerifier -> unit) =
        compile false source (fun (errors, outputFilePath) -> 
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors)

            f (ILVerifier outputFilePath)
        )
 
    let RunScript (source: string) (expectedErrorMessages: string list) =
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

    let ParseWithErrors (source: string) expectedParseErrors =
        let sourceFileName = "test.fs"
        let parsingOptions = { FSharpParsingOptions.Default with SourceFiles = [| sourceFileName |] }
        let parseResults = checker.ParseFile(sourceFileName, SourceText.ofString source, parsingOptions) |> Async.RunSynchronously

        Assert.True(parseResults.ParseHadErrors)

        let errors = 
            parseResults.Errors
            |> Array.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLineAlternate, e.StartColumn, e.EndLineAlternate, e.EndColumn, e.Message)

        Assert.AreEqual(Array.length expectedParseErrors, errors.Length, sprintf "Type check errors: %A" parseResults.Errors)

        Array.zip errors expectedParseErrors
        |> Array.iter (fun (info, expectedError) ->
            let (expectedServerity: FSharpErrorSeverity, expectedErrorNumber: int, expectedErrorRange: int * int * int * int, expectedErrorMsg: string) = expectedError
            Assert.AreEqual(expectedServerity, info.Severity)
            Assert.AreEqual(expectedErrorNumber, info.ErrorNumber, "expectedErrorNumber")
            Assert.AreEqual(expectedErrorRange, (info.StartLineAlternate, info.StartColumn + 1, info.EndLineAlternate, info.EndColumn + 1), "expectedErrorRange")
            Assert.AreEqual(expectedErrorMsg, info.Message, "expectedErrorMsg")
        )
