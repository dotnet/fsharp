// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test

open System
open System.IO
open System.Text
open System.Reflection
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open System.Runtime.Loader
open FSharp.Test.Utilities
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open NUnit.Framework
open TestFramework
open System.Collections.Immutable

[<Sealed>]
type ILVerifier (dllFilePath: string) =

    member _.VerifyIL (expectedIL: string list) =
        ILChecker.checkIL dllFilePath expectedIL

[<Sealed>]
type PdbDebugInfo(debugInfo: string) =

    member _.InfoText = debugInfo

type CompileOutput =
    | Library
    | Exe

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
                    NUnit.Framework.Assert.Fail ("CSharp source diagnostics:\n" + (diagnostics |> Seq.map (fun x -> x.GetMessage () + "\n") |> Seq.reduce (+)))

            | TestCompilation.IL (_, result) ->
                let errors, _ = result.Value
                if errors.Length > 0 then
                    NUnit.Framework.Assert.Fail ("IL source errors: " + errors)

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
    | Preview = 99

[<AbstractClass; Sealed>]
type CompilationUtil private () =

    static let createCSharpCompilation (source: SourceCodeFileKind, lv, tf, additionalReferences, name) =
        let lv =
            match lv with
                | CSharpLanguageVersion.CSharp8 -> LanguageVersion.CSharp8
                | CSharpLanguageVersion.CSharp9 -> LanguageVersion.CSharp9
                | CSharpLanguageVersion.Preview -> LanguageVersion.Preview
                | _ -> LanguageVersion.Default

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
                let ilFilePath = tryCreateTemporaryFileName ()
                let tmp = tryCreateTemporaryFileName ()
                let dllFilePath = Path.ChangeExtension (tmp, ".dll")
                try
                    File.WriteAllText (ilFilePath, source)
                    let errors = ILChecker.reassembleIL ilFilePath dllFilePath
                    try
                        (errors, File.ReadAllBytes dllFilePath)
                    with
                        | _ -> (errors, [||])
                finally
                    try File.Delete ilFilePath with | _ -> ()
                    try File.Delete tmp with | _ -> ()
                    try File.Delete dllFilePath with | _ -> ()
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
        CompilationReference list *
        name: string option *
        outputDirectory: DirectoryInfo option with

        static member Create(source:SourceCodeFileKind, output:CompileOutput, ?options:string array, ?cmplRefs:CompilationReference list, ?name:string, ?outputDirectory: DirectoryInfo) =
            let options = defaultArg options [||]
            let cmplRefs = defaultArg cmplRefs []
            let name =
                match defaultArg name null with
                | null -> None
                | n -> Some n
            Compilation([source], output, options, cmplRefs, name, outputDirectory)

        static member Create(source:string, output:CompileOutput, ?options:string array, ?cmplRefs:CompilationReference list, ?name:string, ?outputDirectory: DirectoryInfo) =
            let options = defaultArg options [||]
            let cmplRefs = defaultArg cmplRefs []
            let name =
                match defaultArg name null with
                | null -> None
                | n -> Some n
            Compilation([SourceCodeFileKind.Create("test.fs", source)], output, options, cmplRefs, name, outputDirectory)

        static member Create(fileName:string, source:string, output, ?options, ?cmplRefs, ?name, ?outputDirectory: DirectoryInfo) =
            let source = SourceCodeFileKind.Create(fileName, source)
            let options = defaultArg options [||]
            let cmplRefs = defaultArg cmplRefs []
            let name = defaultArg name null
            let outputDirectory = defaultArg outputDirectory null
            Compilation.Create(source, output, options, cmplRefs, name, outputDirectory)

        static member CreateFromSources(sources, output, ?options, ?cmplRefs, ?name, ?outputDirectory: DirectoryInfo) =
            let options = defaultArg options [||]
            let cmplRefs = defaultArg cmplRefs []
            let name =
                match defaultArg name null with
                | null -> None
                | n -> Some n
            Compilation(sources, output, options, cmplRefs, name, outputDirectory)


module rec CompilerAssertHelpers =

    let checker = FSharpChecker.Create(suggestNamesForErrors=true)

    // Unlike C# whose entrypoint is always string[] F# can make an entrypoint with 0 args, or with an array of string[]
    let mkDefaultArgs (entryPoint:MethodInfo) : obj[] = [|
        if entryPoint.GetParameters().Length = 1 then
            yield Array.empty<string>
    |]

#if NETCOREAPP
    let executeBuiltApp assembly deps =
        let ctxt = AssemblyLoadContext("ContextName", true)
        try
            ctxt.add_Resolving(fun ctxt name ->
                deps
                |> List.tryFind (fun (x: string) -> Path.GetFileNameWithoutExtension x = name.Name)
                |> Option.map ctxt.LoadFromAssemblyPath
                |> Option.defaultValue null)

            let asm = ctxt.LoadFromAssemblyPath(assembly)
            let entryPoint = asm.EntryPoint
            let args = mkDefaultArgs entryPoint
            (entryPoint.Invoke(Unchecked.defaultof<obj>, args)) |> ignore
        finally
            ctxt.Unload()
#else
    type Worker () =
        inherit MarshalByRefObject()

        member x.ExecuteTestCase assemblyPath (deps: string[]) =
            AppDomain.CurrentDomain.add_AssemblyResolve(ResolveEventHandler(fun _ args ->
                deps
                |> Array.tryFind (fun (x: string) -> Path.GetFileNameWithoutExtension x = args.Name)
                |> Option.bind (fun x -> if FileSystem.FileExistsShim x then Some x else None)
                |> Option.map Assembly.LoadFile
                |> Option.defaultValue null))
            let asm = Assembly.LoadFrom(assemblyPath)
            let entryPoint = asm.EntryPoint
            let args = mkDefaultArgs entryPoint
            (entryPoint.Invoke(Unchecked.defaultof<obj>, args)) |> ignore

    let adSetup =
        let setup = new System.AppDomainSetup ()
        let directory = Path.GetDirectoryName(typeof<Worker>.Assembly.Location)
        setup.ApplicationBase <- directory
        setup

    let executeBuiltApp assembly deps =
        let ad = AppDomain.CreateDomain((Guid()).ToString(), null, adSetup)
        let worker =
            use _ = new AlreadyLoadedAppDomainResolver()
            (ad.CreateInstanceFromAndUnwrap(typeof<Worker>.Assembly.CodeBase, typeof<Worker>.FullName)) :?> Worker
        worker.ExecuteTestCase assembly (deps |> Array.ofList) |>ignore
#endif

    let defaultProjectOptions =
        {
            ProjectFileName = "Z:\\test.fsproj"
            ProjectId = None
            SourceFiles = [|"test.fs"|]
            OtherOptions =
                let assemblies = TargetFrameworkUtil.currentReferences |> Array.map (fun x -> sprintf "-r:%s" x)
                let defaultOptions =  Array.append [|"--preferreduilang:en-US"; "--noframework"; "--warn:5"|] assemblies
#if NETCOREAPP
                Array.append [|"--targetprofile:netcore"|] defaultOptions
#else
                Array.append [|"--targetprofile:mscorlib"|] defaultOptions
#endif
            ReferencedProjects = [||]
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = false
            LoadTime = DateTime()
            UnresolvedReferences = None
            OriginalLoadReferences = []
            Stamp = None
        }

    let rawCompile outputFilePath isExe options (sources: SourceCodeFileKind list) =
        let args =
            [|
                yield "fsc.dll"
                for item in sources do
                    yield item.GetSourceFileName
                yield "-o:" + outputFilePath
                yield (if isExe then "--target:exe" else "--target:library")
                yield! defaultProjectOptions.OtherOptions
                yield! options
             |]

        // Generate a response file, purely for diagnostic reasons.
        File.WriteAllLines(Path.ChangeExtension(outputFilePath, ".rsp"), args)
        let errors, _ = checker.Compile args |> Async.RunImmediate
        errors, outputFilePath

    let compileDisposable (outputDirectory:DirectoryInfo) isExe options nameOpt (sources:SourceCodeFileKind list) =
        let disposeFile path =
            {
                new IDisposable with
                    member _.Dispose() =
                        try File.Delete path with | _ -> ()
            }
        let disposals = ResizeArray<IDisposable>()
        let disposeList =
            {
                new IDisposable with
                    member _.Dispose() =
                        for item in disposals do
                            item.Dispose()
            }
        let name =
            match nameOpt with
            | Some name -> name
            | _ -> tryCreateTemporaryFileName()

        let outputFilePath = Path.ChangeExtension (Path.Combine(outputDirectory.FullName, name), if isExe then ".exe" else ".dll")
        disposals.Add(disposeFile outputFilePath)
        let sources =
            [
                for item in sources do
                    match item.GetSourceText with
                    | Some text ->
                        // In memory source file copy it to the build directory
                        let source = item.ChangeExtension
                        File.WriteAllText (source.GetSourceFileName, text)
                        disposals.Add(disposeFile source.GetSourceFileName)
                        yield source
                    | None ->
                        // On Disk file
                        let sourceFileName = item.GetSourceFileName
                        let source = item.ChangeExtension
                        let destFileName = Path.Combine(outputDirectory.FullName, Path.GetFileName(source.GetSourceFileName))
                        File.Copy(sourceFileName, destFileName, true)
                        disposals.Add(disposeFile destFileName)
                        yield source.WithFileName(destFileName)
            ]
        try
            disposeList, rawCompile outputFilePath isExe options sources
        with
        | _ ->
            disposeList.Dispose()
            reraise()

    let assertErrors libAdjust ignoreWarnings (errors: FSharpDiagnostic []) expectedErrors =
        let errors =
            errors
            |> Array.filter (fun error -> if ignoreWarnings then error.Severity <> FSharpDiagnosticSeverity.Warning && error.Severity <> FSharpDiagnosticSeverity.Info else true)
            |> Array.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLine, e.StartColumn, e.EndLine, e.EndColumn, e.Message)
            |> Array.map (fun info ->
                (info.Severity, info.ErrorNumber, (info.StartLine - libAdjust, info.StartColumn + 1, info.EndLine - libAdjust, info.EndColumn + 1), info.Message))

        let checkEqual k a b =
           if a <> b then
               Assert.AreEqual(a, b, sprintf "Mismatch in %s, expected '%A', got '%A'.\nAll errors:\n%A" k a b errors)

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
        let sourceFile =
            match source.GetSourceText with
            | Some text ->
                // In memory source file copy it to the build directory
                let s = source.WithFileName(tryCreateTemporaryFileName ()).ChangeExtension
                File.WriteAllText (source.GetSourceFileName, text)
                s
            | None ->
                // On Disk file
                source

        let outputFilePath = Path.ChangeExtension (tryCreateTemporaryFileName (), if isExe then ".exe" else ".dll")
        try
            f (rawCompile outputFilePath isExe options [source])
        finally
            try File.Delete sourceFile.GetSourceFileName with | _ -> ()
            try File.Delete outputFilePath with | _ -> ()

    let rec evaluateReferences (outputPath:DirectoryInfo) (disposals: ResizeArray<IDisposable>) ignoreWarnings (cmpl: Compilation) : string[] * string list =
        match cmpl with
        | Compilation(_, _, _, cmpls, _, _) ->
            let compiledRefs =
                cmpls
                |> List.map (fun cmpl ->
                        match cmpl with
                        | CompilationReference (cmpl, staticLink) ->
                            compileCompilationAux outputPath disposals ignoreWarnings cmpl, staticLink
                        | TestCompilationReference (cmpl) ->
                            let fileName =
                                match cmpl with
                                | TestCompilation.CSharp c when not (String.IsNullOrWhiteSpace c.AssemblyName) -> c.AssemblyName
                                | _ -> tryCreateTemporaryFileName()
                            let tmp = Path.Combine(outputPath.FullName, Path.ChangeExtension(fileName, ".dll"))
                            disposals.Add({ new IDisposable with member _.Dispose() = File.Delete tmp })
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

    let rec compileCompilationAux outputDirectory (disposals: ResizeArray<IDisposable>) ignoreWarnings (cmpl: Compilation) : (FSharpDiagnostic[] * string) * string list =

        let compilationRefs, deps = evaluateReferences outputDirectory disposals ignoreWarnings cmpl
        let isExe, sources, options, name =
            match cmpl with
            | Compilation(sources, output, options, _, name, _) ->
                (match output with | Library -> false | Exe -> true),           // isExe
                sources,
                options,
                name

        let disposal, res = compileDisposable outputDirectory isExe (Array.append options compilationRefs) name sources
        disposals.Add(disposal)

        let deps2 =
            compilationRefs
            |> Array.filter (fun x -> not (x.Contains("--staticlink")))
            |> Array.map (fun x -> x.Replace("-r:", String.Empty))
            |> List.ofArray

        res, (deps @ deps2)

    let rec compileCompilation ignoreWarnings (cmpl: Compilation) f =
        let disposals = ResizeArray()
        try
            let outputDirectory = DirectoryInfo(tryCreateTemporaryDirectory())
            disposals.Add({ new IDisposable with member _.Dispose() = try File.Delete (outputDirectory.FullName) with | _ -> () })
            f (compileCompilationAux outputDirectory disposals ignoreWarnings cmpl)
        finally
            disposals
            |> Seq.iter (fun x -> x.Dispose())

    // NOTE: This function will not clean up all the compiled projects after itself.
    // The reason behind is so we can compose verification of test runs easier.
    // TODO: We must not rely on the filesystem when compiling
    let rec returnCompilation (cmpl: Compilation) ignoreWarnings =
        let outputDirectory =
            match cmpl with
            | Compilation(_, _, _, _, _, Some outputDirectory) -> DirectoryInfo(outputDirectory.FullName)
            | Compilation(_, _, _, _, _, _) -> DirectoryInfo(tryCreateTemporaryDirectory())

        outputDirectory.Create() |> ignore
        compileCompilationAux outputDirectory (ResizeArray()) ignoreWarnings cmpl

    let executeBuiltAppAndReturnResult (outputFilePath: string) (deps: string list) : (int * string * string) =
        let out = Console.Out
        let err = Console.Error

        let stdout = StringBuilder ()
        let stderr = StringBuilder ()

        let outWriter = new StringWriter (stdout)
        let errWriter = new StringWriter (stderr)

        let mutable exitCode = 0

        try
            try
                Console.SetOut(outWriter)
                Console.SetError(errWriter)
                (executeBuiltApp outputFilePath deps) |> ignore
            with e ->
                let errorMessage = if e.InnerException <> null then (e.InnerException.ToString()) else (e.ToString())
                stderr.Append (errorMessage) |> ignore
                exitCode <- -1
        finally
            Console.SetOut(out)
            Console.SetError(err)
            outWriter.Close()
            errWriter.Close()

        (exitCode, stdout.ToString(), stderr.ToString())

    let executeBuiltAppNewProcessAndReturnResult (outputFilePath: string) : (int * string * string) =
#if !NETCOREAPP
        let fileName = outputFilePath
        let arguments = ""
#else
        let fileName = "dotnet"
        let arguments = outputFilePath

        let runtimeconfig = """
{
    "runtimeOptions": {
        "tfm": "net6.0",
        "framework": {
            "name": "Microsoft.NETCore.App",
            "version": "6.0"
        }
    }
}"""
        let runtimeconfigPath = Path.ChangeExtension(outputFilePath, ".runtimeconfig.json")
        File.WriteAllText(runtimeconfigPath, runtimeconfig)
        use _disposal =
            { new IDisposable with
              member _.Dispose() = try File.Delete runtimeconfigPath with | _ -> () }
#endif
        let timeout = 30000
        let exitCode, output, errors = Commands.executeProcess (Some fileName) arguments (Path.GetDirectoryName(outputFilePath)) timeout
        (exitCode, output |> String.concat "\n", errors |> String.concat "\n")
open CompilerAssertHelpers

[<Sealed;AbstractClass>]
type CompilerAssert private () =

    static let compileExeAndRunWithOptions options (source: SourceCodeFileKind) =
        compile true options source (fun (errors, outputExe) ->

            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors)

            executeBuiltApp outputExe []
        )

    static let compileLibraryAndVerifyILWithOptions options (source: SourceCodeFileKind) (f: ILVerifier -> unit) =
        compile false options source (fun (errors, outputFilePath) ->
            let errors =
                errors |> Array.filter (fun x -> x.Severity = FSharpDiagnosticSeverity.Error)
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had errors: %A" errors)

            f (ILVerifier outputFilePath)
        )


    static let compileLibraryAndVerifyDebugInfoWithOptions options (expectedFile: string) (source: SourceCodeFileKind) =
        let options = [| yield! options; yield"--test:DumpDebugInfo" |]
        compile false options source (fun (errors, outputFilePath) ->
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

    static member Checker = checker

    static member DefaultProjectOptions = defaultProjectOptions

    static member GenerateFsInputPath() =
        Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), ".fs"))

    static member GenerateDllOutputPath() =
        Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), ".dll"))

    static member CompileWithErrors(cmpl: Compilation, expectedErrors, ?ignoreWarnings) =
        let ignoreWarnings = defaultArg ignoreWarnings false
        compileCompilation ignoreWarnings cmpl (fun ((errors, _), _) ->
            assertErrors 0 ignoreWarnings errors expectedErrors)

    static member Compile(cmpl: Compilation, ?ignoreWarnings) =
        CompilerAssert.CompileWithErrors(cmpl, [||], defaultArg ignoreWarnings false)

    static member CompileRaw(cmpl: Compilation, ?ignoreWarnings) =
        returnCompilation cmpl (defaultArg ignoreWarnings false)

    static member ExecuteAndReturnResult (outputFilePath: string, deps: string list, newProcess: bool) =
        // If we execute in-process (true by default), then the only way of getting STDOUT is to redirect it to SB, and STDERR is from catching an exception.
       if not newProcess then
           executeBuiltAppAndReturnResult outputFilePath deps
       else
           executeBuiltAppNewProcessAndReturnResult outputFilePath

    static member Execute(cmpl: Compilation, ?ignoreWarnings, ?beforeExecute, ?newProcess, ?onOutput) =

        let copyDependenciesToOutputDir (outputFilePath:string) (deps: string list) =
            let outputDirectory = Path.GetDirectoryName(outputFilePath)
            for dep in deps do
                let outputFilePath = Path.Combine(outputDirectory, Path.GetFileName(dep))
                if not (File.Exists(outputFilePath)) then
                    File.Copy(dep, outputFilePath)

        let ignoreWarnings = defaultArg ignoreWarnings false
        let beforeExecute = defaultArg beforeExecute copyDependenciesToOutputDir
        let newProcess = defaultArg newProcess false
        let onOutput = defaultArg onOutput (fun _ -> ())
        compileCompilation ignoreWarnings cmpl (fun ((errors, outputFilePath), deps) ->
            assertErrors 0 ignoreWarnings errors [||]
            beforeExecute outputFilePath deps
            if newProcess then
                let (exitCode, output, errors) = executeBuiltAppNewProcessAndReturnResult outputFilePath
                if exitCode <> 0 then
                    Assert.Fail errors
                onOutput output
            else
                executeBuiltApp outputFilePath deps)

    static member ExecutionHasOutput(cmpl: Compilation, expectedOutput: string) =
        CompilerAssert.Execute(cmpl, newProcess = true, onOutput = (fun output -> Assert.AreEqual(expectedOutput, output, sprintf "'%s' = '%s'" expectedOutput output)))

    /// Assert that the given source code compiles with the `defaultProjectOptions`, with no errors or warnings
    static member CompileOfAst isExe source =
        let outputFilePath = Path.ChangeExtension (tryCreateTemporaryFileName (), if isExe then "exe" else ".dll")
        let parseOptions = { FSharpParsingOptions.Default with SourceFiles = [|"test.fs"|] }

        let parseResults =
            checker.ParseFile("test.fs", SourceText.ofString source, parseOptions)
            |> Async.RunImmediate

        Assert.IsEmpty(parseResults.Diagnostics, sprintf "Parse errors: %A" parseResults.Diagnostics)

        let dependencies =
        #if NETCOREAPP
            Array.toList TargetFrameworkUtil.currentReferences
        #else
            []
        #endif

        let compileErrors, statusCode =
            checker.Compile([parseResults.ParseTree], "test", outputFilePath, dependencies, executable = isExe, noframework = true)
            |> Async.RunImmediate

        Assert.IsEmpty(compileErrors, sprintf "Compile errors: %A" compileErrors)
        Assert.AreEqual(0, statusCode, sprintf "Nonzero status code: %d" statusCode)
        outputFilePath

    static member CompileOfAstToDynamicAssembly source =
        let assemblyName = sprintf "test-%O" (Guid.NewGuid())
        let parseOptions = { FSharpParsingOptions.Default with SourceFiles = [|"test.fs"|] }
        let parseResults =
            checker.ParseFile("test.fs", SourceText.ofString source, parseOptions)
            |> Async.RunImmediate

        Assert.IsEmpty(parseResults.Diagnostics, sprintf "Parse errors: %A" parseResults.Diagnostics)

        let dependencies =
            #if NETCOREAPP
                Array.toList TargetFrameworkUtil.currentReferences
            #else
                []
            #endif

        let compileErrors, statusCode, assembly =
            checker.CompileToDynamicAssembly([parseResults.ParseTree], assemblyName, dependencies, None, noframework = true)
            |> Async.RunImmediate

        Assert.IsEmpty(compileErrors, sprintf "Compile errors: %A" compileErrors)
        Assert.AreEqual(0, statusCode, sprintf "Nonzero status code: %d" statusCode)
        Assert.IsTrue(assembly.IsSome, "no assembly returned")
        Option.get assembly

    static member Pass (source: string) =
        let parseResults, fileAnswer = checker.ParseAndCheckFileInProject("test.fs", 0, SourceText.ofString source, defaultProjectOptions) |> Async.RunImmediate

        Assert.IsEmpty(parseResults.Diagnostics, sprintf "Parse errors: %A" parseResults.Diagnostics)

        match fileAnswer with
        | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
        | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

        Assert.IsEmpty(typeCheckResults.Diagnostics, sprintf "Type Check errors: %A" typeCheckResults.Diagnostics)

    static member PassWithOptions options (source: string) =
        let options = { defaultProjectOptions with OtherOptions = Array.append options defaultProjectOptions.OtherOptions}

        let parseResults, fileAnswer = checker.ParseAndCheckFileInProject("test.fs", 0, SourceText.ofString source, options) |> Async.RunImmediate

        Assert.IsEmpty(parseResults.Diagnostics, sprintf "Parse errors: %A" parseResults.Diagnostics)

        match fileAnswer with
        | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
        | FSharpCheckFileAnswer.Succeeded(typeCheckResults) ->

        Assert.IsEmpty(typeCheckResults.Diagnostics, sprintf "Type Check errors: %A" typeCheckResults.Diagnostics)

    static member TypeCheckWithErrorsAndOptionsAgainstBaseLine options (sourceDirectory:string) (sourceFile: string) =
        let absoluteSourceFile = System.IO.Path.Combine(sourceDirectory, sourceFile)
        let parseResults, fileAnswer =
            checker.ParseAndCheckFileInProject(
                sourceFile,
                0,
                SourceText.ofString (File.ReadAllText absoluteSourceFile),
                { defaultProjectOptions with OtherOptions = Array.append options defaultProjectOptions.OtherOptions; SourceFiles = [|sourceFile|] })
            |> Async.RunImmediate

        Assert.IsEmpty(parseResults.Diagnostics, sprintf "Parse errors: %A" parseResults.Diagnostics)

        match fileAnswer with
        | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted")
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

        Assert.AreEqual(errorsExpectedBaseLine.Replace("\r\n","\n"), errorsActual.Replace("\r\n","\n"))

    static member TypeCheckWithOptionsAndName options name (source: string) =
        let errors =
            let parseResults, fileAnswer =
                checker.ParseAndCheckFileInProject(
                    name,
                    0,
                    SourceText.ofString source,
                    { defaultProjectOptions with OtherOptions = Array.append options defaultProjectOptions.OtherOptions; SourceFiles = [|name|] })
                |> Async.RunImmediate

            if parseResults.Diagnostics.Length > 0 then
                parseResults.Diagnostics
            else

                match fileAnswer with
                | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted"); [| |]
                | FSharpCheckFileAnswer.Succeeded(typeCheckResults) -> typeCheckResults.Diagnostics

        errors

    static member TypeCheckWithOptions options (source: string) =
        let errors =
            let parseResults, fileAnswer =
                checker.ParseAndCheckFileInProject(
                    "test.fs",
                    0,
                    SourceText.ofString source,
                    { defaultProjectOptions with OtherOptions = Array.append options defaultProjectOptions.OtherOptions})
                |> Async.RunImmediate

            if parseResults.Diagnostics.Length > 0 then
                parseResults.Diagnostics
            else

                match fileAnswer with
                | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted"); [| |]
                | FSharpCheckFileAnswer.Succeeded(typeCheckResults) -> typeCheckResults.Diagnostics

        errors

    /// Parses and type checks the given source. Fails if type checker is aborted.
    static member ParseAndTypeCheck(options, name, source: string) =
        let parseResults, fileAnswer =
            checker.ParseAndCheckFileInProject(
                name,
                0,
                SourceText.ofString source,
                { defaultProjectOptions with OtherOptions = Array.append options defaultProjectOptions.OtherOptions})
            |> Async.RunImmediate

        match fileAnswer with
        | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted"); failwith "Type Checker Aborted"
        | FSharpCheckFileAnswer.Succeeded(typeCheckResults) -> parseResults, typeCheckResults

    /// Parses and type checks the given source. Fails if the type checker is aborted or the parser returns any diagnostics.
    static member TypeCheck(options, name, source: string) =
        let parseResults, checkResults = CompilerAssert.ParseAndTypeCheck(options, name, source)

        Assert.IsEmpty(parseResults.Diagnostics, sprintf "Parse errors: %A" parseResults.Diagnostics)

        checkResults

    static member TypeCheckWithErrorsAndOptionsAndAdjust options libAdjust (source: string) expectedTypeErrors =
        let errors =
            let parseResults, fileAnswer =
                checker.ParseAndCheckFileInProject(
                    "test.fs",
                    0,
                    SourceText.ofString source,
                    { defaultProjectOptions with OtherOptions = Array.append options defaultProjectOptions.OtherOptions})
                |> Async.RunImmediate

            if parseResults.Diagnostics.Length > 0 then
                parseResults.Diagnostics
            else

                match fileAnswer with
                | FSharpCheckFileAnswer.Aborted _ -> Assert.Fail("Type Checker Aborted"); [| |]
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

    static member CompileExeWithOptions(options, (source: SourceCodeFileKind)) =
        compile true options source (fun (errors, _) ->
            if errors.Length > 0 then
                Assert.Fail (sprintf "Compile had warnings and/or errors: %A" errors))

    static member CompileExeWithOptions(options, (source: string)) =
        compile true options (SourceCodeFileKind.Create("test.fs", source)) (fun (errors, _) ->
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

    static member RunScriptWithOptionsAndReturnResult options (source: string) =
        // Intialize output and input streams
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

        errorMessages

    static member RunScriptWithOptions options (source: string) (expectedErrorMessages: string list) =
        let errorMessages = CompilerAssert.RunScriptWithOptionsAndReturnResult options source
        if expectedErrorMessages.Length <> errorMessages.Count then
            Assert.Fail(sprintf "Expected error messages: %A \n\n Actual error messages: %A" expectedErrorMessages errorMessages)
        else
            (expectedErrorMessages, errorMessages)
            ||> Seq.iter2 (fun expectedErrorMessage errorMessage ->
                Assert.AreEqual(expectedErrorMessage, errorMessage)
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
        Assert.AreEqual(Array.length expectedParseErrors, errors.Length, sprintf "Parse errors: %A" parseResults.Diagnostics)

        Array.zip errors expectedParseErrors
        |> Array.iter (fun (info, expectedError) ->
            let (expectedSeverity: FSharpDiagnosticSeverity, expectedErrorNumber: int, expectedErrorRange: int * int * int * int, expectedErrorMsg: string) = expectedError
            Assert.AreEqual(expectedSeverity, info.Severity)
            Assert.AreEqual(expectedErrorNumber, info.ErrorNumber, "expectedErrorNumber")
            Assert.AreEqual(expectedErrorRange, (info.StartLine, info.StartColumn + 1, info.EndLine, info.EndColumn + 1), "expectedErrorRange")
            Assert.AreEqual(expectedErrorMsg, info.Message, "expectedErrorMsg")
        )
