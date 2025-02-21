// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test

open System.Threading

#nowarn "57"

open System
open System.Globalization
open System.IO
open System.Text
open System.Reflection
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
#if NETCOREAPP
open System.Runtime.Loader
#endif
open FSharp.Test.Utilities
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Xunit
open TestFramework
open System.Collections.Immutable


#if !NETCOREAPP
module AssemblyResolver =

    let probingPaths = [|
        AppDomain.CurrentDomain.BaseDirectory
        Path.GetDirectoryName(typeof<FactForDESKTOPAttribute>.Assembly.Location)
    |]

    let addResolver () =
        AppDomain.CurrentDomain.add_AssemblyResolve(fun h args ->
            let found () =
                (probingPaths ) |> Seq.tryPick(fun p ->
                    try
                        let name = AssemblyName(args.Name)
                        let codebase = Path.GetFullPath(Path.Combine(p, name.Name))
                        if File.Exists(codebase + ".dll") then
                            name.CodeBase <- codebase  + ".dll"
                            name.CultureInfo <- Unchecked.defaultof<CultureInfo>
                            name.Version <- Unchecked.defaultof<Version>
                            Some (name)
                        elif File.Exists(codebase + ".exe") then
                                name.CodeBase <- codebase + ".exe"
                                name.CultureInfo <- Unchecked.defaultof<CultureInfo>
                                name.Version <- Unchecked.defaultof<Version>
                                Some (name)
                        else None
                    with | _ -> None
                    )
            match found() with
            | None -> Unchecked.defaultof<Assembly>
            | Some name -> Assembly.Load(name) )

    do addResolver()
#endif

type ExecutionOutcome = 
    | NoExitCode
    | ExitCode of int
    | Failure of exn

type ExecutionOutput =
    { Outcome:   ExecutionOutcome
      StdOut:   string
      StdErr:   string }

[<Sealed>]
type ILVerifier (dllFilePath: string) =

    member _.VerifyIL (expectedIL: string list) =
        ILChecker.checkIL dllFilePath expectedIL

[<Sealed>]
type PdbDebugInfo(debugInfo: string) =

    member _.InfoText = debugInfo

type CompileOutput =
    | Exe
    | Library
    | Module

type SourceCodeFile =
    {
        FileName: string
        SourceText: string option
    }

/// A source code file
[<RequireQualifiedAccess>]
type SourceCodeFileKind =
    | Fs of SourceCodeFile
    | Fsx of SourceCodeFile
    | Fsi of SourceCodeFile
    | Cs of SourceCodeFile

    static member Create(path:string, ?source: string) =
        match Path.GetExtension(path).ToLowerInvariant() with
        | ".fsi" -> Fsi({FileName=path; SourceText=source})
        | ".fsx" -> Fsx({FileName=path; SourceText=source})
        | ".cs" -> Cs({FileName=path; SourceText=source})
        | ".fs" | _ -> Fs({FileName=path; SourceText=source})

    member this.ChangeExtension =
        match this with
        | Fs s -> Fs({s with FileName=Path.ChangeExtension(s.FileName, ".fs")})
        | Fsx s -> Fsx({s with FileName=Path.ChangeExtension(s.FileName, ".fsx")})
        | Fsi s -> Fsi({s with FileName=Path.ChangeExtension(s.FileName, ".fsi")})
        | Cs s -> Cs({s with FileName=Path.ChangeExtension(s.FileName, ".cs")})

    member this.IsScript =
        match this with
        | Fsx _ -> true
        | _ -> false

    member this.WithFileName (name:string)=
        match this with
        | Fs s -> Fs({s with FileName=name})
        | Fsx s -> Fsx({s with FileName=name})
        | Fsi s -> Fsi({s with FileName=name})
        | Cs s -> Cs({s with FileName=name})

    member this.GetSourceFileName =
        match this with
        | Fs s -> s.FileName
        | Fsx s -> s.FileName
        | Fsi s -> s.FileName
        | Cs s -> s.FileName

    member this.GetSourceText =
        match this with
        | Fs s -> s.SourceText
        | Fsx s -> s.SourceText
        | Fsi s -> s.SourceText
        | Cs s -> s.SourceText

type RoslynLanguageVersion = LanguageVersion

[<Flags>]
type CSharpCompilationFlags =
    | None = 0x0
    | InternalsVisibleTo = 0x1

[<RequireQualifiedAccess>]
type TestCompilation =
    | CSharp of CSharpCompilation
    | IL of ilSource: string * result: Lazy<string * byte []>

    member this.AssertNoErrorsOrWarnings () =
        match this with
            | TestCompilation.CSharp c ->
                let diagnostics = c.GetDiagnostics ()

                if not diagnostics.IsEmpty then
                    Assert.Fail ("CSharp source diagnostics:\n" + (diagnostics |> Seq.map (fun x -> x.GetMessage () + "\n") |> Seq.reduce (+)))

            | TestCompilation.IL (_, result) ->
                let errors, _ = result.Value
                if errors.Length > 0 then
                    Assert.Fail ("IL source errors: " + errors)

    member this.EmitAsFile (outputPath: string) =
        match this with
            | TestCompilation.CSharp c ->
                let c = c.WithAssemblyName(Path.GetFileNameWithoutExtension outputPath)
                let emitResult = c.Emit outputPath
                if not emitResult.Success then
                    failwithf "Unable to emit C# compilation.\n%A" emitResult.Diagnostics

            | TestCompilation.IL (_, result) ->
                let (_, data) = result.Value
                File.WriteAllBytes (outputPath, data)

type CSharpLanguageVersion =
    | CSharp8 = 0
    | CSharp9 = 1
    | CSharp11 = 11
    | CSharp12 = 12
    | Preview = 99

module CSharpLanguageVersion =
    /// Converts the given C# language version to a Roslyn language version value.
    let toLanguageVersion lv =
        match lv with
        | CSharpLanguageVersion.CSharp8 -> LanguageVersion.CSharp8
        | CSharpLanguageVersion.CSharp9 -> LanguageVersion.CSharp9
        | CSharpLanguageVersion.CSharp11 -> LanguageVersion.CSharp11
        | CSharpLanguageVersion.CSharp12 -> LanguageVersion.CSharp12
        | CSharpLanguageVersion.Preview -> LanguageVersion.Preview
        | _ -> LanguageVersion.Default

[<AbstractClass; Sealed>]
type CompilationUtil private () =

    static let createCSharpCompilation (source: SourceCodeFileKind, lv, tf, additionalReferences, name) =
        let lv = CSharpLanguageVersion.toLanguageVersion lv
        let tf = defaultArg tf TargetFramework.NetStandard20
        let source =
            match source.GetSourceText with
            | Some text ->
                // In memory source file copy it to the build directory
                text
            | None ->
                // On Disk file
                File.ReadAllText(source.GetSourceFileName)
        let name = defaultArg name (Guid.NewGuid().ToString ())
        let additionalReferences = defaultArg additionalReferences ImmutableArray<PortableExecutableReference>.Empty
        let references = TargetFrameworkUtil.getReferences tf
        let c =
            CSharpCompilation.Create(
                name,
                [ CSharpSyntaxTree.ParseText (source, CSharpParseOptions lv) ],
                references.AddRange(additionalReferences).As<MetadataReference>(),
                CSharpCompilationOptions (OutputKind.DynamicallyLinkedLibrary))
        TestCompilation.CSharp c

    static member CreateCSharpCompilation (source:SourceCodeFileKind, lv, ?tf, ?additionalReferences, ?name) =
        createCSharpCompilation (source, lv, tf, additionalReferences, name)

    static member CreateCSharpCompilation (source:string, lv, ?tf, ?additionalReferences, ?name) =
        createCSharpCompilation (SourceCodeFileKind.Create("test.cs", source), lv, tf, additionalReferences, name)

    static member CreateILCompilation (source: string) =
        let compute =
            lazy
                let ilFilePath = getTemporaryFileName() + ".il"
                let dllFilePath = Path.ChangeExtension (ilFilePath, ".dll")
                try
                    File.WriteAllText (ilFilePath, source)
                    let errors = ILChecker.reassembleIL ilFilePath dllFilePath
                    try
                        (errors, File.ReadAllBytes dllFilePath)
                    with
                        | _ -> (errors, [||])
                finally
                    try Directory.Delete(Path.GetDirectoryName ilFilePath, true) with _ -> ()
        TestCompilation.IL (source, compute)

and CompilationReference =
    private
    | CompilationReference of Compilation * staticLink: bool
    | TestCompilationReference of TestCompilation

    static member CreateFSharp(cmpl: Compilation, ?staticLink) =
        let staticLink = defaultArg staticLink false
        CompilationReference(cmpl, staticLink)

    static member Create(cmpl: TestCompilation) =
        TestCompilationReference cmpl

and Compilation =
    private
    | Compilation of
        sources: SourceCodeFileKind list *
        outputType: CompileOutput *
        options: string[] *
        targetFramework: TargetFramework *
        CompilationReference list *
        name: string option *
        outputDirectory: DirectoryInfo option with

        static member Create(source:SourceCodeFileKind, output:CompileOutput, ?options:string array, ?targetFramework:TargetFramework, ?cmplRefs:CompilationReference list, ?name:string, ?outputDirectory: DirectoryInfo) =
            let options = defaultArg options [||]
            let targetFramework = defaultArg targetFramework TargetFramework.Current
            let cmplRefs = defaultArg cmplRefs []
            let name =
                match defaultArg name null with
                | null -> None
                | n -> Some n
            Compilation([source], output, options, targetFramework, cmplRefs, name, outputDirectory)

        static member Create(source:string, output:CompileOutput, ?options:string array, ?targetFramework:TargetFramework, ?cmplRefs:CompilationReference list, ?name:string, ?outputDirectory: DirectoryInfo) =
            let options = defaultArg options [||]
            let targetFramework = defaultArg targetFramework TargetFramework.Current
            let cmplRefs = defaultArg cmplRefs []
            let name =
                match defaultArg name null with
                | null -> None
                | n -> Some n
            Compilation([SourceCodeFileKind.Create("test.fs", source)], output, options, targetFramework, cmplRefs, name, outputDirectory)

        static member Create(fileName:string, source:string, output, ?options, ?targetFramework:TargetFramework, ?cmplRefs, ?name, ?outputDirectory: DirectoryInfo) =
            let source = SourceCodeFileKind.Create(fileName, source)
            let options = defaultArg options [||]
            let targetFramework = defaultArg targetFramework TargetFramework.Current
            let cmplRefs = defaultArg cmplRefs []
            let name = defaultArg name null
            let outputDirectory = defaultArg outputDirectory null
            Compilation.Create(source, output, options, targetFramework, cmplRefs, name, outputDirectory)

        static member CreateFromSources(sources, output, ?options, ?targetFramework, ?cmplRefs, ?name, ?outputDirectory: DirectoryInfo) =
            let options = defaultArg options [||]
            let targetFramework = defaultArg targetFramework TargetFramework.Current
            let cmplRefs = defaultArg cmplRefs []
            let name =
                match defaultArg name null with
                | null -> None
                | n -> Some n
            Compilation(sources, output, options, targetFramework, cmplRefs, name, outputDirectory)

module CompilerAssertHelpers =

    let UseTransparentCompiler =
        FSharp.Compiler.CompilerConfig.FSharpExperimentalFeaturesEnabledAutomatically ||
        not (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEST_TRANSPARENT_COMPILER")))

    let checker = FSharpChecker.Create(suggestNamesForErrors=true, useTransparentCompiler = UseTransparentCompiler)

    // Unlike C# whose entrypoint is always string[] F# can make an entrypoint with 0 args, or with an array of string[]
    let mkDefaultArgs (entryPoint:MethodBase) : obj[] = [|
        if entryPoint.GetParameters().Length = 1 then
            yield Array.empty<string>
    |]

    let executeAssemblyEntryPoint (asm: Assembly) isFsx =
        let entryPoint : MethodBase = asm.EntryPoint
        let entryPoint =
            if isNull entryPoint && isFsx then
                // lookup the last static constructor
                // of the assembly types, which should match
                // the equivalent of a .fsx entry point
                let moduleInitType = asm.GetTypes() |> Array.last
                moduleInitType.GetConstructors(BindingFlags.Static ||| BindingFlags.NonPublic).[0] :> MethodBase
            else
                entryPoint
        let args = mkDefaultArgs entryPoint

        use capture = new TestConsole.ExecutionCapture()
        let outcome =
            try 
                match entryPoint.Invoke(Unchecked.defaultof<obj>, args) with 
                | :? int as rc -> ExitCode rc
                | _ -> NoExitCode
            with
            | exn -> Failure exn
        outcome, capture.OutText, capture.ErrorText

#if NETCOREAPP
    let executeBuiltApp assemblyPath deps isFsx =
        let ctxt = AssemblyLoadContext("ContextName", true)
        try
            ctxt.add_Resolving(fun ctxt name ->
                deps
                |> List.tryFind (fun (x: string) -> Path.GetFileNameWithoutExtension x = name.Name)
                |> Option.map ctxt.LoadFromAssemblyPath
                |> Option.toObj)

            executeAssemblyEntryPoint (ctxt.LoadFromAssemblyPath assemblyPath) isFsx
        finally
            ctxt.Unload()
#else
    type Worker () =
        inherit MarshalByRefObject()

        member x.ExecuteTestCase assemblyPath isFsx =
            // Set console streams for the AppDomain.
            TestConsole.install()
            let assembly = Assembly.LoadFrom assemblyPath
            executeAssemblyEntryPoint assembly isFsx

    let executeBuiltApp assembly dependecies isFsx =
        let thisAssemblyDirectory = Path.GetDirectoryName(typeof<Worker>.Assembly.Location)
        let setup = AppDomainSetup(ApplicationBase = thisAssemblyDirectory)
        let testCaseDomain = AppDomain.CreateDomain($"built app {assembly}", null, setup)

        testCaseDomain.add_AssemblyResolve(fun _ args ->
            dependecies
            |> List.tryFind (fun path -> Path.GetFileNameWithoutExtension path = AssemblyName(args.Name).Name)
            |> Option.filter FileSystem.FileExistsShim
            |> Option.map Assembly.LoadFile
            |> Option.toObj
        )

        let worker =
            (testCaseDomain.CreateInstanceFromAndUnwrap(typeof<Worker>.Assembly.CodeBase, typeof<Worker>.FullName)) :?> Worker

        let outcome, output, errors = worker.ExecuteTestCase assembly isFsx
        // Replay streams captured in appdomain.
        printf $"{output}"
        eprintf $"{errors}"
        
        AppDomain.Unload testCaseDomain
        
        outcome, output, errors

#endif

    let defaultProjectOptions (targetFramework: TargetFramework) =
        let assemblies = TargetFrameworkUtil.getFileReferences targetFramework |> Array.map (fun x -> sprintf "-r:%s" x)
        let testDefaults = [|
            "--preferreduilang:en-US"
            "--noframework"
            "--warn:5"
#if NETCOREAPP
            "--targetprofile:netcore"
#else
            "--targetprofile:mscorlib"
#endif
        |]
        {
            ProjectFileName = "Z:\\test.fsproj"
            ProjectId = None
            SourceFiles = [|"test.fs"|]
            OtherOptions = Array.append testDefaults assemblies
            ReferencedProjects = [||]
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = false
            LoadTime = DateTime()
            UnresolvedReferences = None
            OriginalLoadReferences = []
            Stamp = None
        }

    let defaultProjectOptionsForFilePath path (targetFramework: TargetFramework) =
        { defaultProjectOptions targetFramework with SourceFiles = [| path |] }

    let rawCompile outputFilePath isExe options (targetFramework: TargetFramework) (sources: SourceCodeFileKind list) =
        let args =
            [|
                yield "fsc.dll"
                for item in sources do
                    yield item.GetSourceFileName
                yield "-o:" + outputFilePath
                yield (if isExe then "--target:exe" else "--target:library")
                yield! (defaultProjectOptions targetFramework).OtherOptions
                yield! options
             |]

        // Generate a response file, purely for diagnostic reasons.
        File.WriteAllLines(Path.ChangeExtension(outputFilePath, ".rsp"), args)
        let errors, ex = checker.Compile args |> Async.RunImmediate
        errors, ex, outputFilePath

    let compileDisposable (outputDirectory:DirectoryInfo) isExe options targetFramework nameOpt (sources:SourceCodeFileKind list) =
        let name =
            match nameOpt with
            | Some name -> name
            | _ -> getTemporaryFileNameInDirectory outputDirectory

        let outputFilePath = Path.ChangeExtension (Path.Combine(outputDirectory.FullName, name), if isExe then ".exe" else ".dll")
        let sources =
            [
                for item in sources do
                    match item.GetSourceText with
                    | Some text ->
                        // In memory source file copy it to the build directory
                        let source = item.ChangeExtension
                        let destFileName = Path.Combine(outputDirectory.FullName, Path.GetFileName(source.GetSourceFileName))
                        File.WriteAllText (destFileName, text)
                        yield source.WithFileName(destFileName)
                    | None ->
                        // On Disk file
                        let sourceFileName = item.GetSourceFileName
                        let source = item.ChangeExtension
                        let destFileName = Path.Combine(outputDirectory.FullName, Path.GetFileName(source.GetSourceFileName))
                        File.Copy(sourceFileName, destFileName, true)
                        yield source.WithFileName(destFileName)
            ]
        rawCompile outputFilePath isExe options targetFramework sources
    
    let assertErrors libAdjust ignoreWarnings (errors: FSharpDiagnostic []) expectedErrors =
        let errorMessage (error: FSharpDiagnostic) =
            let errN, range, message = error.ErrorNumber, error.Range, error.Message
            let errorType =
                match error.Severity with
                | FSharpDiagnosticSeverity.Error -> $"Error {errN}"
                | FSharpDiagnosticSeverity.Warning-> $"Warning {errN}"
                | FSharpDiagnosticSeverity.Hidden-> $"Hidden {errN}"
                | FSharpDiagnosticSeverity.Info -> $"Information {errN}"
            $"""({errorType}, Line {range.StartLine}, Col {range.StartColumn}, Line {range.EndLine}, Col {range.EndColumn}, "{message}")""".Replace("\r\n", "\n")
        
        let errors =
            errors
            |> Array.filter (fun error -> if ignoreWarnings then error.Severity <> FSharpDiagnosticSeverity.Warning && error.Severity <> FSharpDiagnosticSeverity.Info else true)
            |> Array.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLine, e.StartColumn, e.EndLine, e.EndColumn, e.Message)
        let errorsAsStr = errors |> Array.map errorMessage |> String.concat ";\n" |> sprintf "[%s]"
        let errors =
            errors
            |> Array.map (fun info ->
                (info.Severity, info.ErrorNumber, (info.StartLine - libAdjust, info.StartColumn + 1, info.EndLine - libAdjust, info.EndColumn + 1), info.Message))
        
        let checkEqual k a b =
           if a <> b then
               failwithf $"Mismatch in %s{k}, expected '%A{a}', got '%A{b}'.\nAll errors:\n%s{errorsAsStr}"

        checkEqual "Errors"  (Array.length expectedErrors) errors.Length

        Array.zip errors expectedErrors
        |> Array.iter (fun (actualError, expectedError) ->
            let (expectedSeverity, expectedErrorNumber, expectedErrorRange, expectedErrorMsg: string) = expectedError
            let (actualSeverity, actualErrorNumber, actualErrorRange, actualErrorMsg: string) = actualError
            let expectedErrorMsg = expectedErrorMsg.Replace("\r\n", "\n")
            let actualErrorMsg = actualErrorMsg.Replace("\r\n", "\n")
            checkEqual "Severity" expectedSeverity actualSeverity
            checkEqual "ErrorNumber" expectedErrorNumber actualErrorNumber
            checkEqual "ErrorRange" expectedErrorRange actualErrorRange
            checkEqual "Message" expectedErrorMsg actualErrorMsg)


    let compile isExe options (source:SourceCodeFileKind) f =
        let outputFilePath = Path.ChangeExtension (getTemporaryFileName (), if isExe then ".exe" else ".dll")
        let tempDir = Directory.GetParent outputFilePath

        let sourceFile =
            match source.GetSourceText with
            | Some text ->
                // In memory source file copy it to the build directory
                let sourceWithTempFileName = source.WithFileName(getTemporaryFileNameInDirectory tempDir).ChangeExtension
                File.WriteAllText(sourceWithTempFileName.GetSourceFileName, text)
                sourceWithTempFileName
            | None ->
                // On Disk file
                source

        f (rawCompile outputFilePath isExe options TargetFramework.Current [sourceFile])

    let rec compileCompilationAux outputDirectory ignoreWarnings (cmpl: Compilation) : (FSharpDiagnostic[] * exn option * string) * string list =

        let compilationRefs, deps = evaluateReferences outputDirectory ignoreWarnings cmpl
        let isExe, sources, options, targetFramework, name =
            match cmpl with
            | Compilation(sources, output, options, targetFramework, _, name, _) ->
                (match output with | Module -> false | Library -> false | Exe -> true),           // isExe
                sources,
                options,
                targetFramework,
                name

        let res = compileDisposable outputDirectory isExe (Array.append options compilationRefs) targetFramework name sources

        let deps2 =
            compilationRefs
            |> Array.filter (fun x -> not (x.Contains("--staticlink")))
            |> Array.map (fun x -> x.Replace("-r:", String.Empty))
            |> List.ofArray

        res, (deps @ deps2)

    and evaluateReferences (outputDir:DirectoryInfo) ignoreWarnings (cmpl: Compilation) : string[] * string list =
        match cmpl with
        | Compilation(_, _, _, _, cmpls, _, _) ->
            let compiledRefs =
                cmpls
                |> List.map (fun cmpl ->
                        match cmpl with
                        | CompilationReference (cmpl, staticLink) ->
                            compileCompilationAux outputDir ignoreWarnings cmpl, staticLink
                        | TestCompilationReference (cmpl) ->
                            let fileName =
                                match cmpl with
                                | TestCompilation.CSharp c when not (String.IsNullOrWhiteSpace c.AssemblyName) -> c.AssemblyName
                                | _ -> getTemporaryFileNameInDirectory outputDir
                            let tmp = Path.Combine(outputDir.FullName, Path.ChangeExtension(fileName, ".dll"))
                            cmpl.EmitAsFile tmp
                            (([||], None, tmp), []), false)

            let compilationRefs =
                compiledRefs
                |> List.map (fun (((errors, _, outputFilePath), _), staticLink) ->
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

    let compileCompilation ignoreWarnings (cmpl: Compilation) f =
        let outputDirectory = createTemporaryDirectory()
        f (compileCompilationAux outputDirectory ignoreWarnings cmpl)

    // NOTE: This function will not clean up all the compiled projects after itself.
    // The reason behind is so we can compose verification of test runs easier.
    // TODO: We must not rely on the filesystem when compiling
    let rec returnCompilation (cmpl: Compilation) ignoreWarnings =
        let outputDirectory =
            match cmpl with
            | Compilation(outputDirectory = Some outputDirectory) -> DirectoryInfo(outputDirectory.FullName)
            | Compilation _ -> createTemporaryDirectory()

        outputDirectory.Create()
        compileCompilationAux outputDirectory ignoreWarnings cmpl

    let unwrapException (ex: exn) = ex.InnerException |> Option.ofObj |> Option.map _.Message |> Option.defaultValue ex.Message

    let executeBuiltAppNewProcess (outputFilePath: string) =
#if !NETCOREAPP
        let fileName = outputFilePath
        let arguments = ""
#else
        let fileName = "dotnet"
        let arguments = outputFilePath

        let runtimeconfig = """
{
    "runtimeOptions": {
        "tfm": "net9.0",
        "framework": {
            "name": "Microsoft.NETCore.App",
            "version": "7.0"
        }
    }
}"""
        let runtimeconfigPath = Path.ChangeExtension(outputFilePath, ".runtimeconfig.json")
        File.WriteAllText(runtimeconfigPath, runtimeconfig)
#endif
        let rc, output, errors = Commands.executeProcess fileName arguments (Path.GetDirectoryName(outputFilePath))
        let output = String.Join(Environment.NewLine, output)
        let errors = String.Join(Environment.NewLine, errors)
        ExitCode rc, output, errors

open CompilerAssertHelpers

[<Sealed;AbstractClass>]
type CompilerAssert private () =

    static let compileExeAndRunWithOptions options (source: SourceCodeFileKind) =
        compile true options source (fun (errors, _, outputExe) ->

            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors)

            executeBuiltApp outputExe [] false
        )

    static let compileLibraryAndVerifyILWithOptions options (source: SourceCodeFileKind) (f: ILVerifier -> unit) =
        compile false options source (fun (errors, _, outputFilePath) ->
            let errors =
                errors |> Array.filter (fun x -> x.Severity = FSharpDiagnosticSeverity.Error)
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had errors: %A" errors)

            f (ILVerifier outputFilePath)
        )

    static let compileLibraryAndVerifyDebugInfoWithOptions options (expectedFile: string) (source: SourceCodeFileKind) =
        let options = [| yield! options; yield"--test:DumpDebugInfo" |]
        compile false options source (fun (errors, _, outputFilePath) ->
            let errors =
                errors |> Array.filter (fun x -> x.Severity = FSharpDiagnosticSeverity.Error)
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had errors: %A" errors)
            let debugInfoFile = outputFilePath + ".debuginfo"
            if not (File.Exists expectedFile) then 
                File.Copy(debugInfoFile, expectedFile)
                failwith $"debug info expected file {expectedFile} didn't exist, now copied over"
            let debugInfo = File.ReadAllLines(debugInfoFile)
            let expected = File.ReadAllLines(expectedFile)
            if debugInfo <> expected then 
                File.Copy(debugInfoFile, expectedFile, overwrite=true)
                failwith $"""debug info mismatch
Expected is in {expectedFile}
Actual is in {debugInfoFile}
Updated automatically, please check diffs in your pull request, changes must be scrutinized
"""
        )

    static member UseTransparentCompiler = UseTransparentCompiler

    static member Checker = checker

    static member DefaultProjectOptions = defaultProjectOptions

    static member GenerateFsInputPath() =
        let path = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), ".fs"))
        printfn $"input path = {path}"
        path

    static member GenerateDllOutputPath() =
        let path = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), ".dll"))
        printfn $"output path = {path}"
        path

    static member CompileWithErrors(cmpl: Compilation, expectedErrors, ?ignoreWarnings) =
        let ignoreWarnings = defaultArg ignoreWarnings false
        compileCompilation ignoreWarnings cmpl (fun ((errors, _, _), _) ->
            assertErrors 0 ignoreWarnings errors expectedErrors)

    static member Compile(cmpl: Compilation, ?ignoreWarnings) =
        CompilerAssert.CompileWithErrors(cmpl, [||], defaultArg ignoreWarnings false)

    static member CompileRaw(cmpl: Compilation, ?ignoreWarnings) =
        returnCompilation cmpl (defaultArg ignoreWarnings false)

    static member ExecuteAndReturnResult (outputFilePath: string, isFsx: bool, deps: string list, newProcess: bool) =
        let outcome, output, errors = 
            if not newProcess then
                executeBuiltApp outputFilePath deps isFsx
            else
                executeBuiltAppNewProcess outputFilePath
        { Outcome = outcome; StdOut = output; StdErr = errors}

    static member ExecuteAux(cmpl: Compilation, ?ignoreWarnings, ?beforeExecute, ?newProcess) =

        let copyDependenciesToOutputDir (outputFilePath:string) (deps: string list) =
            let outputDirectory = Path.GetDirectoryName(outputFilePath)
            for dep in deps do
                let outputFilePath = Path.Combine(outputDirectory, Path.GetFileName(dep))
                if not (File.Exists(outputFilePath)) then
                    File.Copy(dep, outputFilePath)

        let ignoreWarnings = defaultArg ignoreWarnings false
        let beforeExecute = defaultArg beforeExecute copyDependenciesToOutputDir
        let newProcess = defaultArg newProcess false
        compileCompilation ignoreWarnings cmpl (fun ((errors, _, outputFilePath), deps) ->
            assertErrors 0 ignoreWarnings errors [||]
            beforeExecute outputFilePath deps
            if newProcess then 
                executeBuiltAppNewProcess outputFilePath
            else
                executeBuiltApp outputFilePath deps false
        )

    static member Execute(cmpl: Compilation, ?ignoreWarnings, ?beforeExecute, ?newProcess) =
        let outcome, _, _ = CompilerAssert.ExecuteAux(cmpl, ?ignoreWarnings = ignoreWarnings, ?beforeExecute = beforeExecute, ?newProcess = newProcess)
        match outcome with
        | ExitCode n when n <> 0 -> failwith $"Process exited with code {n}."
        | Failure exn -> raise exn
        | _ -> ()


    static member ExecutionHasOutput(cmpl: Compilation, expectedOutput: string) =
        let _, output, _ = CompilerAssert.ExecuteAux(cmpl, newProcess = true)
        Assert.Equal(expectedOutput, output)  

    static member Pass (source: string) =
        let parseResults, fileAnswer = checker.ParseAndCheckFileInProject("test.fs", 0, SourceText.ofString source, defaultProjectOptions TargetFramework.Current) |> Async.RunImmediate

        Assert.Empty(parseResults.Diagnostics)

        match fileAnswer with
        | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Type Checker Aborted")
        | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

        Assert.Empty(typeCheckResults.Diagnostics)

    static member PassWithOptions options (source: string) =
        let defaultOptions = defaultProjectOptions TargetFramework.Current
        let options = { defaultOptions with OtherOptions = Array.append options defaultOptions.OtherOptions}

        let parseResults, fileAnswer = checker.ParseAndCheckFileInProject("test.fs", 0, SourceText.ofString source, options) |> Async.RunImmediate

        Assert.Empty(parseResults.Diagnostics)

        match fileAnswer with
        | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Type Checker Aborted")
        | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

        Assert.Empty(typeCheckResults.Diagnostics)

    static member TypeCheckWithErrorsAndOptionsAgainstBaseLine options (sourceDirectory:string) (sourceFile: string) =
        let absoluteSourceFile = System.IO.Path.Combine(sourceDirectory, sourceFile)
        let parseResults, fileAnswer =
            let defaultOptions = defaultProjectOptions TargetFramework.Current
            checker.ParseAndCheckFileInProject(
                sourceFile,
                0,
                SourceText.ofString (File.ReadAllText absoluteSourceFile),
                { defaultOptions with OtherOptions = Array.append options defaultOptions.OtherOptions; SourceFiles = [|sourceFile|] })
            |> Async.RunImmediate

        Assert.Empty(parseResults.Diagnostics)

        match fileAnswer with
        | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Type Checker Aborted")
        | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

        let errorsExpectedBaseLine =
            let bslFile = Path.ChangeExtension(absoluteSourceFile, "bsl")
            if not (FileSystem.FileExistsShim bslFile) then
                // new test likely initialized, create empty baseline file
                File.WriteAllText(bslFile, "")
            File.ReadAllText(Path.ChangeExtension(absoluteSourceFile, "bsl"))
        let errorsActual =
            typeCheckResults.Diagnostics
            |> Array.map (sprintf "%A")
            |> String.concat "\n"
        File.WriteAllText(Path.ChangeExtension(absoluteSourceFile,"err"), errorsActual)

        Assert.Equal(errorsExpectedBaseLine.Replace("\r\n","\n"), errorsActual.Replace("\r\n","\n"))

    static member TypeCheckWithOptionsAndName options name (source: string) =
        let errors =
            let parseResults, fileAnswer =
                let defaultOptions = defaultProjectOptions TargetFramework.Current
                checker.ParseAndCheckFileInProject(
                    name,
                    0,
                    SourceText.ofString source,
                    { defaultOptions with OtherOptions = Array.append options defaultOptions.OtherOptions; SourceFiles = [|name|] })
                |> Async.RunImmediate

            if parseResults.Diagnostics.Length > 0 then
                if options |> Array.contains "--test:ContinueAfterParseFailure" then
                    [| yield! parseResults.Diagnostics
                       match fileAnswer with
                       | FSharpCheckFileAnswer.Succeeded(tcResults) -> yield! tcResults.Diagnostics 
                       | _ -> () |]
                else parseResults.Diagnostics
            else

                match fileAnswer with
                | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Type Checker Aborted"); [| |]
                | FSharpCheckFileAnswer.Succeeded(typeCheckResults) -> typeCheckResults.Diagnostics

        errors

    static member TypeCheckWithOptions options (source: string) =
        let errors =
            let parseResults, fileAnswer =
                let defaultOptions = defaultProjectOptions TargetFramework.Current
                checker.ParseAndCheckFileInProject(
                    "test.fs",
                    0,
                    SourceText.ofString source,
                    { defaultOptions with OtherOptions = Array.append options defaultOptions.OtherOptions})
                |> Async.RunImmediate

            if parseResults.Diagnostics.Length > 0 then
                parseResults.Diagnostics
            else

                match fileAnswer with
                | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Type Checker Aborted"); [| |]
                | FSharpCheckFileAnswer.Succeeded(typeCheckResults) -> typeCheckResults.Diagnostics

        errors

    /// Parses and type checks the given source. Fails if type checker is aborted.
    static member ParseAndTypeCheck(options, name, source: string) =
        let parseResults, fileAnswer =
            let defaultOptions = defaultProjectOptionsForFilePath name TargetFramework.Current
            checker.ParseAndCheckFileInProject(
                name,
                0,
                SourceText.ofString source,
                { defaultOptions with OtherOptions = Array.append options defaultOptions.OtherOptions})
            |> Async.RunImmediate

        match fileAnswer with
        | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Type Checker Aborted"); failwith "Type Checker Aborted"
        | FSharpCheckFileAnswer.Succeeded(typeCheckResults) -> parseResults, typeCheckResults

    /// Parses and type checks the given source. Fails if the type checker is aborted or the parser returns any diagnostics.
    static member TypeCheck(options, name, source: string) =
        let parseResults, checkResults = CompilerAssert.ParseAndTypeCheck(options, name, source)

        Assert.Empty(parseResults.Diagnostics)

        checkResults

    static member TypeCheckWithErrorsAndOptionsAndAdjust options libAdjust (source: string) expectedTypeErrors =
        let errors =
            let parseResults, fileAnswer =
                let defaultOptions = defaultProjectOptions TargetFramework.Current
                checker.ParseAndCheckFileInProject(
                    "test.fs",
                    0,
                    SourceText.ofString source,
                    { defaultOptions with OtherOptions = Array.append options defaultOptions.OtherOptions})
                |> Async.RunImmediate

            if parseResults.Diagnostics.Length > 0 then
                parseResults.Diagnostics
            else

                match fileAnswer with
                | FSharpCheckFileAnswer.Aborted -> Assert.Fail("Type Checker Aborted"); [| |]
                | FSharpCheckFileAnswer.Succeeded(typeCheckResults) -> typeCheckResults.Diagnostics

        assertErrors libAdjust false errors expectedTypeErrors


    static member TypeCheckWithErrorsAndOptions options (source: string) expectedTypeErrors =
        CompilerAssert.TypeCheckWithErrorsAndOptionsAndAdjust options 0 (source: string) expectedTypeErrors

    static member TypeCheckWithErrors (source: string) expectedTypeErrors =
        CompilerAssert.TypeCheckWithErrorsAndOptions [||] source expectedTypeErrors

    static member TypeCheckSingleErrorWithOptions options (source: string) (expectedSeverity: FSharpDiagnosticSeverity) (expectedErrorNumber: int) (expectedErrorRange: int * int * int * int) (expectedErrorMsg: string) =
        CompilerAssert.TypeCheckWithErrorsAndOptions options source [| expectedSeverity, expectedErrorNumber, expectedErrorRange, expectedErrorMsg |]

    static member TypeCheckSingleError (source: string) (expectedSeverity: FSharpDiagnosticSeverity) (expectedErrorNumber: int) (expectedErrorRange: int * int * int * int) (expectedErrorMsg: string) =
        CompilerAssert.TypeCheckWithErrors source [| expectedSeverity, expectedErrorNumber, expectedErrorRange, expectedErrorMsg |]

    static member TypeCheckProject(options: string array, sourceFiles: string array, getSourceText, enablePartialTypeChecking, useTransparentCompiler) : FSharpCheckProjectResults =
        let checker = FSharpChecker.Create(documentSource = DocumentSource.Custom getSourceText, enablePartialTypeChecking = enablePartialTypeChecking, useTransparentCompiler = useTransparentCompiler)
        let defaultOptions = defaultProjectOptions TargetFramework.Current
        let projectOptions = { defaultOptions with OtherOptions = Array.append options defaultOptions.OtherOptions; SourceFiles = sourceFiles }

        if useTransparentCompiler then
            let getFileSnapshot _ fileName =
                async.Return
                    (FSharpFileSnapshot(
                        FileName = fileName,
                        Version = "1",
                        GetSource = fun () -> task {
                            match! getSourceText fileName with
                            | Some source -> return SourceTextNew.ofISourceText source
                            | None -> return failwith $"couldn't get source for {fileName}"
                        }
                    ))

            let snapshot = FSharpProjectSnapshot.FromOptions(projectOptions, getFileSnapshot) |> Async.RunImmediate

            checker.ParseAndCheckProject(snapshot)
        else
            checker.ParseAndCheckProject(projectOptions)
        |> Async.RunImmediate

    static member CompileExeWithOptions(options, (source: SourceCodeFileKind)) =
        compile true options source (fun (errors, _, _) ->
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors))

    static member CompileExeWithOptions(options, (source: string)) =
        compile true options (SourceCodeFileKind.Create("test.fs", source)) (fun (errors, _, _) ->
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors))

    static member CompileExe (source: SourceCodeFileKind) =
        CompilerAssert.CompileExeWithOptions([||], source)

    static member CompileExe (source: string) =
        CompilerAssert.CompileExeWithOptions([||], (SourceCodeFileKind.Create("test.fs", source)))

    static member CompileExeAndRunWithOptions(options, (source: SourceCodeFileKind)) =
        compileExeAndRunWithOptions options source

    static member CompileExeAndRunWithOptions(options, (source: string)) =
        compileExeAndRunWithOptions options (SourceCodeFileKind.Create("test.fs", source))

    static member CompileExeAndRun (source: SourceCodeFileKind) =
        compileExeAndRunWithOptions [||] source

    static member CompileExeAndRun (source: string) =
        compileExeAndRunWithOptions [||] (SourceCodeFileKind.Create("test.fs", source))

    static member CompileLibraryAndVerifyILWithOptions(options, (source: SourceCodeFileKind), (f: ILVerifier -> unit)) =
        compileLibraryAndVerifyILWithOptions options source f 

    static member CompileLibraryAndVerifyILWithOptions(options, (source: string), (f: ILVerifier -> unit)) =
        compileLibraryAndVerifyILWithOptions options (SourceCodeFileKind.Create("test.fs", source)) f 

    static member CompileLibraryAndVerifyDebugInfoWithOptions(options, (expectedFile: string), (source: SourceCodeFileKind)) =
        compileLibraryAndVerifyDebugInfoWithOptions options expectedFile source

    static member CompileLibraryAndVerifyDebugInfoWithOptions(options, (expectedFile: string), (source: string)) =
        compileLibraryAndVerifyDebugInfoWithOptions options expectedFile (SourceCodeFileKind.Create("test.fs", source))

    static member CompileLibraryAndVerifyIL((source: SourceCodeFileKind), (f: ILVerifier -> unit)) =
        compileLibraryAndVerifyILWithOptions [||] source f

    static member CompileLibraryAndVerifyIL((source: string), (f: ILVerifier -> unit)) =
        compileLibraryAndVerifyILWithOptions [||] (SourceCodeFileKind.Create("test.fs", source)) f

    static member CompileLibraryAndVerifyILRealSig((source: string), (f: ILVerifier -> unit)) =
        compileLibraryAndVerifyILWithOptions [|"--realsig+"|] (SourceCodeFileKind.Create("test.fs", source)) f

    static member RunScriptWithOptionsAndReturnResult options (source: string) =
        // Initialize output and input streams
        use inStream = new StringReader("")
        use outStream = new StringWriter()
        use errStream = new StringWriter()

        // Build command line arguments & start FSI session
        let argv = [| "C:\\fsi.exe" |]
#if NETCOREAPP
        let args = Array.append argv [|"--noninteractive"; "--targetprofile:netcore"|]
#else
        let args = Array.append argv [|"--noninteractive"; "--targetprofile:mscorlib"|]
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

        errorMessages, string outStream, string errStream

    static member RunScriptWithOptions options (source: string) (expectedErrorMessages: string list) =
        let errorMessages, _, _ = CompilerAssert.RunScriptWithOptionsAndReturnResult options source
        if expectedErrorMessages.Length <> errorMessages.Count then
            Assert.Fail(sprintf "Expected error messages: %A \n\n Actual error messages: %A" expectedErrorMessages errorMessages)
        else
            (expectedErrorMessages, errorMessages)
            ||> Seq.iter2 (fun expectedErrorMessage errorMessage ->
                Assert.Equal(expectedErrorMessage, errorMessage)
        )

    static member RunScript source expectedErrorMessages =
        CompilerAssert.RunScriptWithOptions [||] source expectedErrorMessages

    static member Parse (source: string, ?langVersion: string, ?fileName: string) =
        let langVersion = defaultArg langVersion "default"
        let sourceFileName = defaultArg fileName "test.fsx"
        let parsingOptions =
            { FSharpParsingOptions.Default with
                SourceFiles = [| sourceFileName |]
                LangVersionText = langVersion }
        checker.ParseFile(sourceFileName, SourceText.ofString source, parsingOptions) |> Async.RunImmediate

    static member ParseWithErrors (source: string, ?langVersion: string) = fun expectedParseErrors ->
        let parseResults = CompilerAssert.Parse (source, ?langVersion=langVersion)

        Assert.True(parseResults.ParseHadErrors)

        let errors =
            parseResults.Diagnostics
            |> Array.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLine, e.StartColumn, e.EndLine, e.EndColumn, e.Message)

        printfn $"diagnostics: %A{[| for e in errors -> e.Severity, e.ErrorNumber, e.StartLine, e.StartColumn, e.EndLine, e.EndColumn, e.Message |]}"
        Assert.True((Array.length expectedParseErrors = errors.Length), sprintf "Parse errors: %A" parseResults.Diagnostics)

        Array.zip errors expectedParseErrors
        |> Array.iter (fun (info, expectedError) ->
            let (expectedSeverity: FSharpDiagnosticSeverity, expectedErrorNumber: int, expectedErrorRange: int * int * int * int, expectedErrorMsg: string) = expectedError
            Assert.Equal(expectedSeverity, info.Severity)
            Assert.Equal(expectedErrorNumber, info.ErrorNumber)
            Assert.Equal(expectedErrorRange, (info.StartLine, info.StartColumn + 1, info.EndLine, info.EndColumn + 1))
            Assert.Equal(expectedErrorMsg, info.Message)
        )
