// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test.Utilities

open System
open System.Diagnostics
open System.IO
open System.Text
open System.Diagnostics
open System.Collections.Generic
open System.Reflection
open FSharp.Compiler.Text
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Interactive.Shell
#if FX_NO_APP_DOMAINS
open System.Runtime.Loader
#endif
open NUnit.Framework
open System.Reflection.Emit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open FSharp.Test.Utilities.Utilities

[<Sealed>]
type ILVerifier (dllFilePath: string) =

    member this.VerifyIL (qualifiedItemName: string, expectedIL: string) =
        ILChecker.checkILItem qualifiedItemName dllFilePath [ expectedIL ]

    member this.VerifyIL (expectedIL: string list) =
        ILChecker.checkIL dllFilePath expectedIL

    member this.VerifyILWithLineNumbers (qualifiedItemName: string, expectedIL: string) =
        ILChecker.checkILItemWithLineNumbers qualifiedItemName dllFilePath [ expectedIL ]

type Worker () =
    inherit MarshalByRefObject()

    member x.ExecuteTestCase assemblyPath (deps: string[]) =
        AppDomain.CurrentDomain.add_AssemblyResolve(ResolveEventHandler(fun _ args ->
            deps
            |> Array.tryFind (fun (x: string) -> Path.GetFileNameWithoutExtension x = args.Name)
            |> Option.bind (fun x -> if File.Exists x then Some x else None)
            |> Option.map Assembly.LoadFile
            |> Option.defaultValue null))
        let asm = Assembly.LoadFrom(assemblyPath)
        let entryPoint = asm.EntryPoint
        (entryPoint.Invoke(Unchecked.defaultof<obj>, [||])) |> ignore

type SourceKind =
    | Fs
    | Fsx

type CompileOutput =
    | Library
    | Exe

type CompilationReference =
    private
    | CompilationReference of Compilation * staticLink: bool
    | TestCompilationReference of TestCompilation

    static member CreateFSharp(cmpl: Compilation, ?staticLink) =
        let staticLink = defaultArg staticLink false
        CompilationReference(cmpl, staticLink)

    static member Create(cmpl: TestCompilation) =
        TestCompilationReference cmpl

and Compilation = private Compilation of source: string * SourceKind * CompileOutput * options: string[] * CompilationReference list * name: string option with

    static member Create(source, sourceKind, output, ?options, ?cmplRefs, ?name) =
        let options = defaultArg options [||]
        let cmplRefs = defaultArg cmplRefs []
        Compilation(source, sourceKind, output, options, cmplRefs, name)

[<Sealed;AbstractClass>]
type CompilerAssert private () =

    static let checker = FSharpChecker.Create(suggestNamesForErrors=true)

    static let config = TestFramework.initializeSuite ()

    static let _ = config |> ignore

// Do a one time dotnet sdk build to compute the proper set of reference assemblies to pass to the compiler
#if NETCOREAPP
    static let projectFile = """
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <UseFSharpPreview>true</UseFSharpPreview>
    <DisableImplicitFSharpCoreReference>true</DisableImplicitFSharpCoreReference>
  </PropertyGroup>

  <ItemGroup><Compile Include="Program.fs" /></ItemGroup>

  <Target Name="WriteFrameworkReferences" AfterTargets="AfterBuild">
    <WriteLinesToFile File="FrameworkReferences.txt" Lines="@(ReferencePath)" Overwrite="true" WriteOnlyWhenDifferent="true" />
  </Target>

</Project>"""

    static let programFs = """
open System

[<EntryPoint>]
let main argv = 0"""

    static let getNetCoreAppReferences =
        let mutable output = ""
        let mutable errors = ""
        let mutable cleanUp = true
        let projectDirectory = Path.Combine(Path.GetTempPath(), "CompilerAssert", Path.GetRandomFileName())
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
                raise (new Exception (sprintf "An error occurred getting netcoreapp references: %A" e))
        finally
            if cleanUp then
                try Directory.Delete(projectDirectory) with | _ -> ()
#endif

#if FX_NO_APP_DOMAINS
    static let executeBuiltApp assembly deps =
        let ctxt = AssemblyLoadContext("ContextName", true)
        try
            let asm = ctxt.LoadFromAssemblyPath(assembly)
            let entryPoint = asm.EntryPoint
            ctxt.add_Resolving(fun ctxt name ->
                deps
                |> List.tryFind (fun (x: string) -> Path.GetFileNameWithoutExtension x = name.Name)
                |> Option.map ctxt.LoadFromAssemblyPath
                |> Option.defaultValue null)
            (entryPoint.Invoke(Unchecked.defaultof<obj>, [||])) |> ignore
        finally
            ctxt.Unload()
#else

    static let pathToThisDll = Assembly.GetExecutingAssembly().CodeBase

    static let adSetup =
        let setup = new System.AppDomainSetup ()
        setup.PrivateBinPath <- pathToThisDll
        setup

    static let executeBuiltApp assembly deps =
        let ad = AppDomain.CreateDomain((Guid()).ToString(), null, adSetup)
        let worker = (ad.CreateInstanceFromAndUnwrap(pathToThisDll, typeof<Worker>.FullName)) :?> Worker
        worker.ExecuteTestCase assembly (deps |> Array.ofList) |>ignore
#endif

    static let defaultProjectOptions =
        {
            ProjectFileName = "Z:\\test.fsproj"
            ProjectId = None
            SourceFiles = [|"test.fs"|]
#if NETCOREAPP
            OtherOptions =
                let assemblies = getNetCoreAppReferences |> Array.map (fun x -> sprintf "-r:%s" x)
                Array.append [|"--preferreduilang:en-US"; "--targetprofile:netcore"; "--noframework";"--warn:5"|] assemblies
#else
            OtherOptions = [|"--preferreduilang:en-US";"--warn:5"|]
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

    static let rawCompile inputFilePath outputFilePath isExe options source =
        File.WriteAllText (inputFilePath, source)
        let args =
            options
            |> Array.append defaultProjectOptions.OtherOptions
            |> Array.append [| "fsc.exe"; inputFilePath; "-o:" + outputFilePath; (if isExe then "--target:exe" else "--target:library"); "--nowin32manifest" |]
        let errors, _ = checker.Compile args |> Async.RunSynchronously

        errors, outputFilePath

    static let compileAux isExe options source f : unit =
        let inputFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".fs")
        let outputFilePath = Path.ChangeExtension (Path.GetTempFileName(), if isExe then ".exe" else ".dll")
        try
            f (rawCompile inputFilePath outputFilePath isExe options source)
        finally
            try File.Delete inputFilePath with | _ -> ()
            try File.Delete outputFilePath with | _ -> ()

    static let compileDisposable outputPath isScript isExe options nameOpt source =
        let ext =
            if isScript then ".fsx"
            else ".fs"
        let inputFilePath = Path.ChangeExtension(Path.Combine(outputPath, Path.GetRandomFileName()), ext)
        let name =
            match nameOpt with
            | Some name -> name
            | _ -> Path.GetRandomFileName()
        let outputFilePath = Path.ChangeExtension (Path.Combine(outputPath, name), if isExe then ".exe" else ".dll")
        let o =
            { new IDisposable with
                member _.Dispose() =
                    try File.Delete inputFilePath with | _ -> ()
                    try File.Delete outputFilePath with | _ -> () }
        try
            o, rawCompile inputFilePath outputFilePath isExe options source
        with
        | _ ->
            o.Dispose()
            reraise()

    static let assertErrors libAdjust ignoreWarnings (errors: FSharpErrorInfo []) expectedErrors =
        let errors =
            errors
            |> Array.filter (fun error -> if ignoreWarnings then error.Severity <> FSharpErrorSeverity.Warning else true)
            |> Array.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLineAlternate, e.StartColumn, e.EndLineAlternate, e.EndColumn, e.Message)
            |> Array.map (fun info ->
                (info.Severity, info.ErrorNumber, (info.StartLineAlternate - libAdjust, info.StartColumn + 1, info.EndLineAlternate - libAdjust, info.EndColumn + 1), info.Message))

        let checkEqual k a b =
            if a <> b then
                Assert.AreEqual(a, b, sprintf "Mismatch in %s, expected '%A', got '%A'.\nAll errors:\n%A" k a b errors)

        checkEqual "Errors"  (Array.length expectedErrors) errors.Length

        Array.zip errors expectedErrors
        |> Array.iter (fun (actualError, expectedError) ->
            let (expectedSeverity, expectedErrorNumber, expectedErrorRange, expectedErrorMsg) = expectedError
            let (actualSeverity, actualErrorNumber, actualErrorRange, actualErrorMsg) = actualError
            checkEqual "Severity" expectedSeverity actualSeverity
            checkEqual "ErrorNumber" expectedErrorNumber actualErrorNumber
            checkEqual "ErrorRange" expectedErrorRange actualErrorRange
            checkEqual "Message" expectedErrorMsg actualErrorMsg)

    static let gate = obj ()

    static let compile isExe options source f =
        lock gate (fun _ -> compileAux isExe options source f)

    static let rec compileCompilationAux outputPath (disposals: ResizeArray<IDisposable>) ignoreWarnings (cmpl: Compilation) : (FSharpErrorInfo[] * string) * string list =
        let compilationRefs, deps =
            match cmpl with
            | Compilation(_, _, _, _, cmpls, _) ->
                let compiledRefs =
                    cmpls
                    |> List.map (fun cmpl ->
                            match cmpl with
                            | CompilationReference (cmpl, staticLink) ->
                                compileCompilationAux outputPath disposals ignoreWarnings cmpl, staticLink
                            | TestCompilationReference (cmpl) ->
                                let filename =
                                 match cmpl with
                                 | TestCompilation.CSharp c -> c.AssemblyName
                                 | _ -> Path.GetRandomFileName()
                                let tmp = Path.Combine(outputPath, Path.ChangeExtension(filename, ".dll"))
                                disposals.Add({ new IDisposable with
                                                    member _.Dispose() =
                                                        try File.Delete tmp with | _ -> () })
                                cmpl.EmitAsFile tmp
                                (([||], tmp), []), false)

                let compilationRefs =
                    compiledRefs
                    |> List.map (fun (((errors, outputFilePath), _), staticLink) ->
                        assertErrors 0 ignoreWarnings errors [||]
                        let rOption = "-r:" + outputFilePath
                        if staticLink then
                            [rOption;"--staticlink:" + Path.GetFileNameWithoutExtension outputFilePath]
                        else
                            [rOption])
                    |> List.concat
                    |> Array.ofList

                let deps =
                    compiledRefs
                    |> List.map (fun ((_, deps), _) -> deps)
                    |> List.concat
                    |> List.distinct

                compilationRefs, deps

        let isScript =
            match cmpl with
            | Compilation(_, kind, _, _, _, _) ->
                match kind with
                | Fs -> false
                | Fsx -> true

        let isExe =
            match cmpl with
            | Compilation(_, _, output, _, _, _) ->
                match output with
                | Library -> false
                | Exe -> true

        let source =
            match cmpl with
            | Compilation(source, _, _, _, _, _) -> source

        let options =
            match cmpl with
            | Compilation(_, _, _, options, _, _) -> options

        let nameOpt =
            match cmpl with
            | Compilation(_, _, _, _, _, nameOpt) -> nameOpt

        let disposal, res = compileDisposable outputPath isScript isExe (Array.append options compilationRefs) nameOpt source
        disposals.Add disposal

        let deps2 =
            compilationRefs
            |> Array.filter (fun x -> not (x.Contains("--staticlink")))
            |> Array.map (fun x -> x.Replace("-r:", String.Empty))
            |> List.ofArray

        res, (deps @ deps2)

    static let rec compileCompilation ignoreWarnings (cmpl: Compilation) f =
        let compileDirectory = Path.Combine(Path.GetTempPath(), "CompilerAssert", Path.GetRandomFileName())
        let disposals = ResizeArray()
        try
            Directory.CreateDirectory(compileDirectory) |> ignore
            f (compileCompilationAux compileDirectory disposals ignoreWarnings cmpl)
        finally
            try Directory.Delete compileDirectory with | _ -> ()
            disposals
            |> Seq.iter (fun x -> x.Dispose())

    static member CompileWithErrors(cmpl: Compilation, expectedErrors, ?ignoreWarnings) =
        let ignoreWarnings = defaultArg ignoreWarnings false
        lock gate (fun () ->
            compileCompilation ignoreWarnings cmpl (fun ((errors, _), _) ->
                assertErrors 0 ignoreWarnings errors expectedErrors))

    static member Compile(cmpl: Compilation, ?ignoreWarnings) =
        CompilerAssert.CompileWithErrors(cmpl, [||], defaultArg ignoreWarnings false)

    static member Execute(cmpl: Compilation, ?ignoreWarnings, ?beforeExecute, ?newProcess, ?onOutput) =
        let ignoreWarnings = defaultArg ignoreWarnings false
        let beforeExecute = defaultArg beforeExecute (fun _ _ -> ())
        let newProcess = defaultArg newProcess false
        let onOutput = defaultArg onOutput (fun _ -> ())
        lock gate (fun () ->
            compileCompilation ignoreWarnings cmpl (fun ((errors, outputFilePath), deps) ->
                assertErrors 0 ignoreWarnings errors [||]
                beforeExecute outputFilePath deps
                if newProcess then
                    let mutable pinfo = ProcessStartInfo()
                    pinfo.RedirectStandardError <- true
                    pinfo.RedirectStandardOutput <- true
#if !NETCOREAPP
                    pinfo.FileName <- outputFilePath
#else
                    pinfo.FileName <- "dotnet"
                    pinfo.Arguments <- outputFilePath

                    let runtimeconfig =
                        """
{
    "runtimeOptions": {
        "tfm": "netcoreapp3.1",
        "framework": {
            "name": "Microsoft.NETCore.App",
            "version": "3.1.0"
        }
    }
}
                        """

                    let runtimeconfigPath = Path.ChangeExtension(outputFilePath, ".runtimeconfig.json")
                    File.WriteAllText(runtimeconfigPath, runtimeconfig)
                    use _disposal =
                        { new IDisposable with
                            member _.Dispose() = try File.Delete runtimeconfigPath with | _ -> () }
#endif
                    pinfo.UseShellExecute <- false
                    let p = Process.Start pinfo
                    let errors = p.StandardError.ReadToEnd()
                    let output = p.StandardOutput.ReadToEnd()
                    Assert.True(p.WaitForExit(120000))
                    if p.ExitCode <> 0 then
                        Assert.Fail errors
                    onOutput output
                else
                    executeBuiltApp outputFilePath deps))

    static member ExecutionHasOutput(cmpl: Compilation, expectedOutput: string) =
        CompilerAssert.Execute(cmpl, newProcess = true, onOutput = (fun output -> Assert.AreEqual(expectedOutput, output)))

    /// Assert that the given source code compiles with the `defaultProjectOptions`, with no errors or warnings
    static member Pass (source: string) =
        lock gate <| fun () ->
            let parseResults, fileAnswer = checker.ParseAndCheckFileInProject("test.fs", 0, SourceText.ofString source, defaultProjectOptions) |> Async.RunSynchronously

            Assert.IsEmpty(parseResults.Errors, sprintf "Parse errors: %A" parseResults.Errors)

            match fileAnswer with
            | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
            | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

            Assert.IsEmpty(typeCheckResults.Errors, sprintf "Type Check errors: %A" typeCheckResults.Errors)

    static member PassWithOptions options (source: string) =
        lock gate <| fun () ->
            let options = { defaultProjectOptions with OtherOptions = Array.append options defaultProjectOptions.OtherOptions}

            let parseResults, fileAnswer = checker.ParseAndCheckFileInProject("test.fs", 0, SourceText.ofString source, options) |> Async.RunSynchronously

            Assert.IsEmpty(parseResults.Errors, sprintf "Parse errors: %A" parseResults.Errors)

            match fileAnswer with
            | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
            | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

            Assert.IsEmpty(typeCheckResults.Errors, sprintf "Type Check errors: %A" typeCheckResults.Errors)

    static member TypeCheckWithErrorsAndOptionsAgainstBaseLine options (sourceDirectory:string) (sourceFile: string) =
        lock gate <| fun () ->
            let absoluteSourceFile = System.IO.Path.Combine(sourceDirectory, sourceFile)
            let parseResults, fileAnswer =
                checker.ParseAndCheckFileInProject(
                    sourceFile,
                    0,
                    SourceText.ofString (File.ReadAllText absoluteSourceFile),
                    { defaultProjectOptions with OtherOptions = Array.append options defaultProjectOptions.OtherOptions; SourceFiles = [|sourceFile|] })
                |> Async.RunSynchronously

            Assert.IsEmpty(parseResults.Errors, sprintf "Parse errors: %A" parseResults.Errors)

            match fileAnswer with
            | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
            | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

            let errorsExpectedBaseLine =
                let bslFile = Path.ChangeExtension(absoluteSourceFile, "bsl")
                if not (File.Exists bslFile) then
                    // new test likely initialized, create empty baseline file
                    File.WriteAllText(bslFile, "")
                File.ReadAllText(Path.ChangeExtension(absoluteSourceFile, "bsl"))
            let errorsActual =
                typeCheckResults.Errors
                |> Array.map (sprintf "%A")
                |> String.concat "\n"
            File.WriteAllText(Path.ChangeExtension(absoluteSourceFile,"err"), errorsActual)

            Assert.AreEqual(errorsExpectedBaseLine.Replace("\r\n","\n"), errorsActual.Replace("\r\n","\n"))

    static member TypeCheckWithErrorsAndOptionsAndAdjust options libAdjust (source: string) expectedTypeErrors =
        lock gate <| fun () ->
            let errors =
                let parseResults, fileAnswer =
                    checker.ParseAndCheckFileInProject(
                        "test.fs",
                        0,
                        SourceText.ofString source,
                        { defaultProjectOptions with OtherOptions = Array.append options defaultProjectOptions.OtherOptions})
                    |> Async.RunSynchronously

                if parseResults.Errors.Length > 0 then
                    parseResults.Errors
                else

                    match fileAnswer with
                    | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted"); [| |]
                    | FSharpCheckFileAnswer.Succeeded(typeCheckResults) -> typeCheckResults.Errors

            assertErrors libAdjust false errors expectedTypeErrors

    static member TypeCheckWithErrorsAndOptions options (source: string) expectedTypeErrors =
        CompilerAssert.TypeCheckWithErrorsAndOptionsAndAdjust options 0 (source: string) expectedTypeErrors

    static member TypeCheckWithErrors (source: string) expectedTypeErrors =
        CompilerAssert.TypeCheckWithErrorsAndOptions [||] source expectedTypeErrors

    static member TypeCheckSingleErrorWithOptions options (source: string) (expectedSeverity: FSharpErrorSeverity) (expectedErrorNumber: int) (expectedErrorRange: int * int * int * int) (expectedErrorMsg: string) =
        CompilerAssert.TypeCheckWithErrorsAndOptions options source [| expectedSeverity, expectedErrorNumber, expectedErrorRange, expectedErrorMsg |]

    static member TypeCheckSingleError (source: string) (expectedSeverity: FSharpErrorSeverity) (expectedErrorNumber: int) (expectedErrorRange: int * int * int * int) (expectedErrorMsg: string) =
        CompilerAssert.TypeCheckWithErrors source [| expectedSeverity, expectedErrorNumber, expectedErrorRange, expectedErrorMsg |]

    static member CompileExeWithOptions options (source: string) =
        compile true options source (fun (errors, _) ->
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors))

    static member CompileExe (source: string) =
        CompilerAssert.CompileExeWithOptions [||] source

    static member CompileExeAndRunWithOptions options (source: string) =
        compile true options source (fun (errors, outputExe) ->

            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors)

            executeBuiltApp outputExe []
        )

    static member CompileExeAndRun (source: string) =
        CompilerAssert.CompileExeAndRunWithOptions [||] source

    static member CompileLibraryAndVerifyILWithOptions options (source: string) (f: ILVerifier -> unit) =
        compile false options source (fun (errors, outputFilePath) ->
            let errors =
                errors |> Array.filter (fun x -> x.Severity = FSharpErrorSeverity.Error)
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had errors: %A" errors)

            f (ILVerifier outputFilePath)
        )

    static member CompileLibraryAndVerifyIL (source: string) (f: ILVerifier -> unit) =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [||] source f

    static member RunScriptWithOptions options (source: string) (expectedErrorMessages: string list) =
        lock gate <| fun () ->
            // Intialize output and input streams
            use inStream = new StringReader("")
            use outStream = new StringWriter()
            use errStream = new StringWriter()

            // Build command line arguments & start FSI session
            let argv = [| "C:\\fsi.exe" |]
    #if NETCOREAPP
            let args = Array.append argv [|"--noninteractive"; "--targetprofile:netcore"|]
    #else
            let args = Array.append argv [|"--noninteractive"|]
    #endif
            let allArgs = Array.append args options

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

    static member RunScript source expectedErrorMessages =
        CompilerAssert.RunScriptWithOptions [||] source expectedErrorMessages

    static member ParseWithErrors (source: string) expectedParseErrors =
        let sourceFileName = "test.fs"
        let parsingOptions = { FSharpParsingOptions.Default with SourceFiles = [| sourceFileName |] }
        let parseResults = checker.ParseFile(sourceFileName, SourceText.ofString source, parsingOptions) |> Async.RunSynchronously

        Assert.True(parseResults.ParseHadErrors)

        let errors =
            parseResults.Errors
            |> Array.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLineAlternate, e.StartColumn, e.EndLineAlternate, e.EndColumn, e.Message)

        Assert.AreEqual(Array.length expectedParseErrors, errors.Length, sprintf "Parse errors: %A" parseResults.Errors)

        Array.zip errors expectedParseErrors
        |> Array.iter (fun (info, expectedError) ->
            let (expectedSeverity: FSharpErrorSeverity, expectedErrorNumber: int, expectedErrorRange: int * int * int * int, expectedErrorMsg: string) = expectedError
            Assert.AreEqual(expectedSeverity, info.Severity)
            Assert.AreEqual(expectedErrorNumber, info.ErrorNumber, "expectedErrorNumber")
            Assert.AreEqual(expectedErrorRange, (info.StartLineAlternate, info.StartColumn + 1, info.EndLineAlternate, info.EndColumn + 1), "expectedErrorRange")
            Assert.AreEqual(expectedErrorMsg, info.Message, "expectedErrorMsg")
        )
