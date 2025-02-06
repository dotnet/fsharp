// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test

open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.IO
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open FSharp.Test.Assert
open FSharp.Test.Utilities
open FSharp.Test.ScriptHelpers
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Xunit
open Xunit.Abstractions
open System
open System.Collections.Immutable
open System.IO
open System.Text
open System.Text.RegularExpressions
open System.Reflection
open System.Reflection.Metadata
open System.Reflection.PortableExecutable

open FSharp.Test.CompilerAssertHelpers
open TestFramework

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open FSharp.Compiler.CodeAnalysis

module rec Compiler =

    [<AutoOpen>]
    type SourceUtilities () =
        static member getCurrentMethodName([<CallerMemberName; Optional; DefaultParameterValue("")>] memberName: string) = memberName

    type BaselineFile =
        {
            FilePath: string
            BslSource: string
            Content: string option
        }

    type Baseline =
        {
            SourceFilename: string option
            FSBaseline: BaselineFile
            ILBaseline: BaselineFile
        }

    type TestType =
        | Text of string
        | Path of string

    type CompilationUnit =
        | FS of FSharpCompilationSource
        | CS of CSharpCompilationSource
        | IL of ILCompilationSource

        override this.ToString() = match this with | FS fs -> fs.ToString() | _ -> (sprintf "%A" this)

        member this.OutputDirectory =
            let toString diOpt =
                match diOpt: DirectoryInfo option with
                | Some di -> di.FullName
                | None -> ""
            match this with
            | FS fs -> fs.OutputDirectory |> toString
            | CS cs -> cs.OutputDirectory |> toString
            | _ -> raise (Exception "Not supported for this compilation type")

        member this.WithStaticLink(staticLink: bool) = match this with | FS fs -> FS { fs with StaticLink = staticLink } | cu -> cu

    type FSharpCompilationSource =
        { Source:           SourceCodeFileKind
          AdditionalSources:SourceCodeFileKind list
          Baseline:         Baseline option
          Options:          string list
          OutputType:       CompileOutput
          OutputDirectory:  DirectoryInfo option
          Name:             string option
          IgnoreWarnings:   bool
          References:       CompilationUnit list
          TargetFramework:  TargetFramework
          StaticLink:       bool
        }

        member this.CreateOutputDirectory() =
            match this.OutputDirectory with
            | Some d -> d.Create()
            | None -> ()

        member this.FullName =
            match this.OutputDirectory, this.Name with
            | Some directory, Some name -> Some(Path.Combine(directory.FullName, name))
            | None, _ -> this.Name
            | _ -> None

        member this.OutputFileName =
            match this.FullName, this.OutputType with
            | Some fullName, CompileOutput.Library -> Some (Path.ChangeExtension(fullName, ".dll"))
            | Some fullName, CompileOutput.Exe -> Some (Path.ChangeExtension(fullName, ".exe"))
            | _ -> None

        override this.ToString() = match this.Name with | Some n -> n | _ -> (sprintf "%A" this)

    type CSharpCompilationSource =
        {
            Source:          SourceCodeFileKind
            LangVersion:     CSharpLanguageVersion
            TargetFramework: TargetFramework
            OutputType:      CompileOutput
            OutputDirectory: DirectoryInfo option
            Name:            string option
            References:      CompilationUnit list
        }

    type ILCompilationSource =
        {
            Source:     TestType
            References: CompilationUnit list
        }

    type ErrorType = Error of int | Warning of int | Information of int | Hidden of int

    type SymbolType =
        | MemberOrFunctionOrValue of string
        | Entity of string
        | GenericParameter of string
        | Parameter of string
        | StaticParameter of string
        | ActivePatternCase of string
        | UnionCase of string
        | Field of string

        member this.FullName () =
            match this with
            | MemberOrFunctionOrValue fullname
            | Entity fullname
            | GenericParameter fullname
            | Parameter fullname
            | StaticParameter fullname
            | ActivePatternCase fullname
            | UnionCase fullname
            | Field fullname -> fullname

    let mapDiagnosticSeverity severity errorNumber =
        match severity with
        | FSharpDiagnosticSeverity.Hidden -> Hidden errorNumber
        | FSharpDiagnosticSeverity.Info -> Information errorNumber
        | FSharpDiagnosticSeverity.Warning -> Warning errorNumber
        | FSharpDiagnosticSeverity.Error -> Error errorNumber

    type Line = Line of int
    type Col = Col of int

    type Range =
        { StartLine:   int
          StartColumn: int
          EndLine:     int
          EndColumn:   int }

    type Disposable (dispose : unit -> unit) =
        interface IDisposable with
            member this.Dispose() =
                dispose()

    type ErrorInfo =
        { Error:   ErrorType
          Range:   Range
          NativeRange : FSharp.Compiler.Text.range
          Message: string
          SubCategory: string }

// This type is used either for the output of the compiler (typically in CompilationResult coming from 'compile')
// or for the output of the code generated by the compiler (in CompilationResult coming from 'run')

    type EvalOutput =
        { Result: Result<FsiValue option, exn>
          StdOut: string
          StdErr: string }

    type RunOutput =
        | EvalOutput of EvalOutput
        | ExecutionOutput of ExecutionOutput

    type SourceCodeFileName = string

    type CompilationOutput =
        { OutputPath:    string option
          Dependencies:  string list
          Adjust:        int
          Diagnostics:   ErrorInfo list
          PerFileErrors: (SourceCodeFileName * ErrorInfo) list
          Output:        RunOutput option
          Compilation:   CompilationUnit }

    [<RequireQualifiedAccess>]
    type CompilationResult =
        | Success of CompilationOutput
        | Failure of CompilationOutput
        with
            member this.Output = match this with Success o | Failure o -> o
            member this.RunOutput = this.Output.Output
            member this.Compilation = this.Output.Compilation
            member this.OutputPath = this.Output.OutputPath

    type ExecutionPlatform =
        | Anycpu = 0
        | AnyCpu32bitPreferred = 1
        | X86 = 2
        | Itanium = 3
        | X64 = 4
        | Arm = 5
        | Arm64 = 6

    let public defaultOptions : string list = ["--realsig+"]

    let normalizePathSeparator (text:string) = text.Replace(@"\", "/")

    let normalizeName name =
        let invalidPathChars = Array.concat [Path.GetInvalidPathChars(); [| ':'; '\\'; '/'; ' '; '.' |]]
        let result = invalidPathChars |> Array.fold(fun (acc:string) (c:char) -> acc.Replace(string(c), "_")) name
        result

    let readFileOrDefault (path: string): string option =
        match FileSystem.FileExistsShim(path) with
        | true -> Some (File.ReadAllText path)
        | _ -> None

    let createCompilationUnit sourceBaselineSuffix ilBaselineSuffixes directoryPath filename =

        let outputDirectoryPath = createTemporaryDirectory().FullName
        let sourceFilePath = normalizePathSeparator (directoryPath ++ filename)
        let fsBslFilePath = sourceFilePath + sourceBaselineSuffix + ".err.bsl"
        let ilBslFilePath =
            let ilBslPaths = [|
                for baselineSuffix in ilBaselineSuffixes do
#if DEBUG
    #if NETCOREAPP
                    yield sourceFilePath + baselineSuffix + ".il.netcore.debug.bsl"
                    yield sourceFilePath + baselineSuffix + ".il.netcore.bsl"
    #else
                    yield sourceFilePath + baselineSuffix + ".il.net472.debug.bsl"
                    yield sourceFilePath + baselineSuffix + ".il.net472.bsl"
    #endif
                    yield sourceFilePath + baselineSuffix + ".il.debug.bsl"
                    yield sourceFilePath + baselineSuffix + ".il.bsl"
#else
    #if NETCOREAPP
                    yield sourceFilePath + baselineSuffix + ".il.netcore.release.bsl"
                    yield sourceFilePath + baselineSuffix + ".il.netcore.bsl"
    #else
                    yield sourceFilePath + baselineSuffix + ".il.net472.release.bsl"
                    yield sourceFilePath + baselineSuffix + ".il.net472.bsl"
    #endif
                    yield sourceFilePath + baselineSuffix + ".il.release.bsl"
                    yield sourceFilePath + baselineSuffix + ".il.bsl"
#endif
                |]

            let findBaseline =
                ilBslPaths
                |> Array.tryPick(fun p -> if File.Exists(p) then Some p else None)
            match findBaseline with
            | Some s -> s
            | None -> sourceFilePath + sourceBaselineSuffix + ".il.bsl"

        let fsOutFilePath = normalizePathSeparator (Path.ChangeExtension(outputDirectoryPath ++ filename, ".err"))
        let ilOutFilePath = normalizePathSeparator (Path.ChangeExtension(outputDirectoryPath ++ filename, ".il"))
        let fsBslSource = readFileOrDefault fsBslFilePath
        let ilBslSource = readFileOrDefault ilBslFilePath

        {   Source            = SourceCodeFileKind.Create(sourceFilePath)
            AdditionalSources = []
            Baseline          =
                Some
                    {
                        SourceFilename = Some sourceFilePath
                        FSBaseline = { FilePath = fsOutFilePath; BslSource = fsBslFilePath; Content = fsBslSource }
                        ILBaseline = { FilePath = ilOutFilePath; BslSource = ilBslFilePath; Content = ilBslSource }
                    }
            Options           = Compiler.defaultOptions
            OutputType        = Library
            Name              = Some filename
            IgnoreWarnings    = false
            References        = []
            OutputDirectory   = Some (DirectoryInfo(outputDirectoryPath))
            TargetFramework   = TargetFramework.Current
            StaticLink        = false
            } |> FS

    /// For all files specified in the specified directory, whose name can be found in includedFiles
    /// create a compilation with all baselines correctly when set
    let createCompilationUnitForFiles baselineSuffix directoryPath includedFiles =

        if not (Directory.Exists(directoryPath)) then
            failwith (sprintf "Directory does not exist: \"%s\"." directoryPath)

        let allFiles : string[] = Directory.GetFiles(directoryPath, "*.fs")

        let filteredFiles =
            match includedFiles |> Array.map (fun f -> normalizePathSeparator (directoryPath ++ f)) with
                | [||] -> allFiles
                | incl -> incl

        let fsFiles = filteredFiles |> Array.map Path.GetFileName

        if fsFiles |> Array.length < 1 then
            failwith (sprintf "No required files found in \"%s\".\nAll files: %A.\nIncludes:%A." directoryPath allFiles includedFiles)

        for f in filteredFiles do
            if not <| FileSystem.FileExistsShim(f) then
                failwithf "Requested file \"%s\" not found.\nAll files: %A.\nIncludes:%A." f allFiles includedFiles

        let results =
            fsFiles
            |> Array.map (fun fs -> (createCompilationUnit baselineSuffix [baselineSuffix] directoryPath fs) :> obj)
            |> Seq.map (fun c -> [| c |])

        results

    let getTestOutputDirectory dir testCaseName extraDirectory =
        // If the executing assembly has 'artifacts\bin' in it's path then we are operating normally in the CI or dev tests
        // Thus the output directory will be in a subdirectory below where we are executing.
        // The subdirectory will be relative to the source directory containing the test source file,
        // E.g
        //    When the source code is in:
        //        $(repo-root)\tests\FSharp.Compiler.ComponentTests\Conformance\PseudoCustomAttributes
        //    and the test is running in the FSharp.Compiler.ComponentTeststest library
        //    The output directory will be:
        //        artifacts\bin\FSharp.Compiler.ComponentTests\$(Flavour)\$(TargetFramework)\tests\FSharp.Compiler.ComponentTests\Conformance\PseudoCustomAttributes
        //
        //    If we can't find anything then we execute in the directory containing the source
        //
        try
            let testlibraryLocation = normalizePathSeparator (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            let pos = testlibraryLocation.IndexOf("artifacts/bin",StringComparison.OrdinalIgnoreCase)
            if pos > 0 then
                // Running under CI or dev build
                let testRoot = Path.Combine(testlibraryLocation.Substring(0, pos), @"tests/")
                let testSourceDirectory =
                    let dirInfo = normalizePathSeparator (Path.GetFullPath(dir))
                    let testPaths = dirInfo.Replace(testRoot, "").Split('/')
                    testPaths[0] <- "tests"
                    Path.Combine(testPaths)
                let n = Path.Combine(testlibraryLocation, testSourceDirectory.Trim('/'), normalizeName testCaseName, extraDirectory)
                let outputDirectory = new DirectoryInfo(n)
                Some outputDirectory
            else
                raise (new InvalidOperationException($"Failed to find the test output directory:\nTest Library Location: '{testlibraryLocation}'\n Pos: {pos}"))
                None

        with | e ->
            raise (new InvalidOperationException($" '{e.Message}'.  Can't get the location of the executing assembly"))

    // Not very safe version of reading stuff from file, but we want to fail fast for now if anything goes wrong.
    let private getSource (src: TestType) : string =
        match src with
        | Text t -> t
        | Path p ->
            use stream = FileSystem.OpenFileForReadShim(p)
            stream.ReadAllText()

    // Load the source file from the path
    let loadSourceFromFile path = getSource(TestType.Path path)

    let fsFromString (source: SourceCodeFileKind): FSharpCompilationSource =
        {
            Source            = source
            AdditionalSources = []
            Baseline          = None
            Options           = defaultOptions
            OutputType        = Library
            OutputDirectory   = None
            Name              = None
            IgnoreWarnings    = false
            References        = []
            TargetFramework   = TargetFramework.Current
            StaticLink        = false
        }

    let private csFromString (source: SourceCodeFileKind) : CSharpCompilationSource =
        {
            Source          = source
            LangVersion     = CSharpLanguageVersion.CSharp9
            TargetFramework = TargetFramework.Current
            OutputType      = Library
            OutputDirectory = None
            Name            = None
            References      = []
        }

    let private fromFSharpDiagnostic (errors: FSharpDiagnostic[]) : (SourceCodeFileName * ErrorInfo) list =
        let toErrorInfo (e: FSharpDiagnostic) : SourceCodeFileName * ErrorInfo =
            let errorNumber = e.ErrorNumber
            let severity = e.Severity
            let error =
                match severity with
                | FSharpDiagnosticSeverity.Warning -> Warning errorNumber
                | FSharpDiagnosticSeverity.Error -> Error errorNumber
                | FSharpDiagnosticSeverity.Info -> Information errorNumber
                | FSharpDiagnosticSeverity.Hidden -> Hidden errorNumber

            e.FileName |> Path.GetFileName,
            { Error   = error
              NativeRange = e.Range
              SubCategory = e.Subcategory
              Range   =
                  { StartLine   = e.StartLine
                    StartColumn = e.StartColumn
                    EndLine     = e.EndLine
                    EndColumn   = e.EndColumn }
              Message = e.Message }

        errors
        |> List.ofArray
        |> List.distinctBy (fun e -> e.FileName,e.Severity, e.ErrorNumber, e.StartLine, e.StartColumn, e.EndLine, e.EndColumn, e.Message)
        |> List.map toErrorInfo

    let private partitionErrors diagnostics = diagnostics |> List.partition (fun e -> match e.Error with Error _ -> true | _ -> false)

    let private getErrors diagnostics = diagnostics |> List.filter (fun e -> match e.Error with Error _ -> true | _ -> false)

    let private getWarnings diagnostics = diagnostics |> List.filter (fun e -> match e.Error with Warning _ -> true | _ -> false)

    let private adjustRange (range: Range) (adjust: int) : Range =
        {
            StartLine   = range.StartLine   - adjust
            StartColumn = range.StartColumn + 1
            EndLine     = range.EndLine     - adjust
            EndColumn   = range.EndColumn   + 1
        }

    let FsxSourceCode source =
        SourceCodeFileKind.Fsx({FileName="test.fsx"; SourceText=Some source})

    let Source source =
        SourceCodeFileKind.Create("test.fs", source)

    let SourceFromPath path =
        SourceCodeFileKind.Create(path)

    let FsiSource source =
        SourceCodeFileKind.Fsi({FileName="test.fsi"; SourceText=Some source })

    let FsSource source =
        SourceCodeFileKind.Fs({FileName="test.fs"; SourceText=Some source })

    let CsSource source =
        SourceCodeFileKind.Cs({FileName="test.cs"; SourceText=Some source })

    let CsFromPath (path: string) : CompilationUnit =
        csFromString (SourceFromPath path)
        |> CS
        |> withName (Path.GetFileNameWithoutExtension(path))

    let Fsx (source: string) : CompilationUnit =
        fsFromString (FsxSourceCode source) |> FS

    let FsxFromPath (path: string) : CompilationUnit =
        fsFromString (SourceFromPath path) |> FS
    
    let Fs (source: string) : CompilationUnit =
        fsFromString (FsSource source) |> FS

    let Fsi (source: string) : CompilationUnit =
        fsFromString (FsiSource source) |> FS

    let FSharp (source: string) : CompilationUnit =
        Fs source

    let FSharpWithFileName name (source: string) : CompilationUnit =
        fsFromString (SourceCodeFileKind.Fs({FileName=name; SourceText=Some source }))
        |> FS

    let FsFromPath (path: string) : CompilationUnit =
        fsFromString (SourceFromPath path)
        |> FS
        |> withName (Path.GetFileNameWithoutExtension(path))

    let FSharpWithInputAndOutputPath (src: string) (inputFilePath: string) (outputFilePath: string) : CompilationUnit =
        let compileDirectory = Path.GetDirectoryName(outputFilePath)
        let name = Path.GetFileName(outputFilePath)
        {
            Source            = SourceCodeFileKind.Create(inputFilePath, src)
            AdditionalSources = []
            Baseline          = None
            Options           = defaultOptions
            OutputType        = Library
            OutputDirectory   = Some(DirectoryInfo(compileDirectory))
            Name              = Some name
            IgnoreWarnings    = false
            References        = []
            TargetFramework   = TargetFramework.Current
            StaticLink        = false
        } |> FS

    let CSharp (source: string) : CompilationUnit =
        csFromString (SourceCodeFileKind.Fs({FileName="test.cs"; SourceText=Some source })) |> CS

    let CSharpFromPath (path: string) : CompilationUnit =
        csFromString (SourceFromPath path) |> CS

    let asFs (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS { Source = SourceCodeFileKind.Fsi _} -> cUnit
        | FS src -> FS {src with Source=SourceCodeFileKind.Fs({FileName=src.Source.GetSourceFileName; SourceText=src.Source.GetSourceText})}
        | _ -> failwith "Only F# compilation can be of type Fs."

    let asFsi (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS src -> FS {src with Source=SourceCodeFileKind.Fsi({FileName=src.Source.GetSourceFileName; SourceText=src.Source.GetSourceText})}
        | _ -> failwith "Only F# compilation can be of type Fsi."

    let asFsx (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS src -> FS {src with Source=SourceCodeFileKind.Fsx({FileName=src.Source.GetSourceFileName; SourceText=src.Source.GetSourceText})}
        | _ -> failwith "Only F# compilation can be of type Fsx."

    let withName (name: string) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS src -> FS { src with Name = Some name }
        | CS src -> CS { src with Name = Some name }
        | IL _ -> failwith "IL Compilation cannot be named."

    let withFileName (name: string) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS compilationSource -> FS { compilationSource with Source = compilationSource.Source.WithFileName(name) }
        | CS cSharpCompilationSource -> CS { cSharpCompilationSource with Source = cSharpCompilationSource.Source.WithFileName(name) }
        | IL _ -> failwith "IL Compilation cannot be named."

    let withReferenceFSharpCompilerService (cUnit: CompilationUnit) : CompilationUnit =
        // Compute the location of the FSharp.Compiler.Service dll that matches the target framework used to build this test assembly
        let compilerServiceAssemblyLocation =
            typeof<FSharp.Compiler.Text.Range>.Assembly.Location
        withOptionsHelper [ $"-r:{compilerServiceAssemblyLocation}" ] "withReferenceFSharpCompilerService is only supported for F#" cUnit

    let withReferences (references: CompilationUnit list) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with References = fs.References @ references }
        | CS cs -> CS { cs with References = cs.References @ references }
        | IL _ -> failwith "References are not supported in IL"

    let withStaticLink (references: CompilationUnit list) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with References = fs.References @ references }
        | CS cs -> CS { cs with References = cs.References @ references }
        | IL _ -> failwith "References are not supported in IL"

    let withAdditionalSourceFiles (additionalSources: SourceCodeFileKind list) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with AdditionalSources = fs.AdditionalSources @ additionalSources }
        | CS _ ->  failwith "References are not supported in C#"
        | IL _ ->  failwith "References are not supported in IL"

    let withAdditionalSourceFile (additionalSource: SourceCodeFileKind) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with AdditionalSources = fs.AdditionalSources @ [additionalSource]}
        | CS _ ->  failwith "References are not supported in C#"
        | IL _ ->  failwith "References are not supported in IL"

    let private withOptionsHelper (options: string list) (message:string) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with Options = fs.Options @ options }
        | _ -> failwith message

    let withCodepage (codepage:string) (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ $"--codepage:{codepage}" ] "codepage is only supported on F#" cUnit

    let withDebug (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--debug+" ] "debug+ is only supported on F#" cUnit

    let withNoDebug (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--debug-" ] "debug- is only supported on F#" cUnit

    let withOcamlCompat (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--mlcompatibility" ] "withOcamlCompat is only supported on F#" cUnit

    let withOptions (options: string list) (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper options "withOptions is only supported for F#" cUnit

    let withOptionsString (options: string) (cUnit: CompilationUnit) : CompilationUnit =
        let options = if String.IsNullOrWhiteSpace options then [] else (options.Split([|';'|])) |> Array.toList
        withOptionsHelper options "withOptionsString is only supported for F#" cUnit

    let withOutputDirectory (path: DirectoryInfo option) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with OutputDirectory = path }
        | _ -> failwith "withOutputDirectory is only supported on F#"

    let withCheckNulls (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper ["--checknulls+"] "checknulls is only supported in F#" cUnit

    let withBufferWidth (width: int)(cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ $"--bufferwidth:{width}" ] "withBufferWidth is only supported on F#" cUnit

    let withDefines (defines: string list) (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper (defines |> List.map(fun define -> $"--define:{define}")) "withDefines is only supported on F#" cUnit

    let withErrorRanges (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--test:ErrorRanges" ] "withErrorRanges is only supported on F#" cUnit

    let withLangVersion46 (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--langversion:4.6" ] "withLangVersion46 is only supported on F#" cUnit

    let withLangVersion47 (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--langversion:4.7" ] "withLangVersion47 is only supported on F#" cUnit

    let withLangVersion50 (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--langversion:5.0" ] "withLangVersion50 is only supported on F#" cUnit

    let withLangVersion60 (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--langversion:6.0" ] "withLangVersion60 is only supported on F#" cUnit

    let withLangVersion70 (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--langversion:7.0" ] "withLangVersion70 is only supported on F#" cUnit

    let withLangVersion80 (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--langversion:8.0" ] "withLangVersion80 is only supported on F#" cUnit

    let withLangVersion90 (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--langversion:9.0" ] "withLangVersion90 is only supported on F#" cUnit

    let withLangVersionPreview (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--langversion:preview" ] "withLangVersionPreview is only supported on F#" cUnit

    let withLangVersion (version: string) (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ $"--langversion:{version}" ] "withLangVersion is only supported on F#" cUnit

    let withAssemblyVersion (version:string) (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ $"--version:{version}" ] "withAssemblyVersion is only supported on F#" cUnit

    let withWarnOn warning (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ $"--warnon:{warning}" ] "withWarnOn is only supported for F#" cUnit

    let withNoWarn warning (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ $"--nowarn:{warning}" ] "withNoWarn is only supported for F#" cUnit

    let withNoOptimize (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--optimize-" ] "withNoOptimize is only supported for F#" cUnit

    let withOptimize (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--optimize+" ] "withOptimize is only supported for F#" cUnit

    let withOptimization (optimization: bool) (cUnit: CompilationUnit) : CompilationUnit =
        let option = if optimization then "--optimize+" else "--optimize-"
        withOptionsHelper [ option ] "withOptimization is only supported for F#" cUnit

    let withFullPdb(cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--debug:full" ] "withFullPdb is only supported for F#" cUnit

    let withPdbOnly(cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--debug:pdbonly" ] "withPdbOnly is only supported for F#" cUnit

    let withPortablePdb(cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--debug:portable" ] "withPortablePdb is only supported for F#" cUnit

    let withEmbeddedPdb(cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--debug:embedded" ] "withEmbeddedPdb is only supported for F#" cUnit

    let withEmbedAllSource(cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--embed+" ] "withEmbedAllSource is only supported for F#" cUnit

    let withEmbedNoSource(cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--embed-" ] "withEmbedNoSource is only supported for F#" cUnit

    let withEmbedSourceFiles(cUnit: CompilationUnit) files : CompilationUnit =
        withOptionsHelper [ $"--embed:{files}" ] "withEmbedSourceFiles is only supported for F#" cUnit

    /// Turns on checks that check integrity of XML doc comments
    let withXmlCommentChecking (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--warnon:3390" ] "withXmlCommentChecking is only supported for F#" cUnit

    /// Turns on checks that force the documentation of all parameters
    let withXmlCommentStrictParamChecking (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--warnon:3391" ] "withXmlCommentChecking is only supported for F#" cUnit

    /// Only include optimization information essential for implementing inlined constructs. Inhibits cross-module inlining but improves binary compatibility.
    let withNoOptimizationData (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--nooptimizationdata" ] "withNoOptimizationData is only supported for F#" cUnit

    /// Don't add a resource to the generated assembly containing F#-specific metadata
    let withNoInterfaceData (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--nointerfacedata" ] "withNoInterfaceData is only supported for F#" cUnit

    //--refonly[+|-]
    let withRefOnly (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ $"--refonly+" ] "withRefOnly is only supported for F#" cUnit

    //--refonly[+|-]
    let withNoRefOnly (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ $"--refonly-" ] "withRefOnly is only supported for F#" cUnit

    //--refout:<file>                          Produce a reference assembly with the specified file path.
    let withRefOut (name:string) (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ $"--refout:{name}" ] "withNoInterfaceData is only supported for F#" cUnit

    let withCSharpLanguageVersion (ver: CSharpLanguageVersion) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | CS cs -> CS { cs with LangVersion = ver }
        | _ -> failwith "Only supported in C#"

    let withCSharpLanguageVersionPreview =
        withCSharpLanguageVersion CSharpLanguageVersion.Preview

    let withOutputType (outputType : CompileOutput) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS x -> FS { x with OutputType = outputType }
        | CS x -> CS { x with OutputType = outputType }
        | _ -> failwith "TODO: Implement where applicable."

    let withRealInternalSignatureOff (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with Options = fs.Options @ ["--realsig-"] }
        | _ -> failwith "withRealInternalSignatureOff only supported by f#"

    let withRealInternalSignatureOn (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with Options = fs.Options @ ["--realsig+"] }
        | _ -> failwith "withRealInternalSignatureOn only supported by f#"

    let withRealInternalSignature (realSig: bool) (cUnit: CompilationUnit) : CompilationUnit  =
        if realSig then
            cUnit |>  withRealInternalSignatureOn
        else
            cUnit |>  withRealInternalSignatureOff

    let asExe (cUnit: CompilationUnit) : CompilationUnit =
        withOutputType CompileOutput.Exe cUnit

    let asLibrary (cUnit: CompilationUnit) : CompilationUnit =
        withOutputType CompileOutput.Library cUnit

    let asModule (cUnit: CompilationUnit) : CompilationUnit =
        withOutputType CompileOutput.Module cUnit

    let asNetStandard20 (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with TargetFramework = TargetFramework.NetStandard20 }
        | CS _ -> failwith "References are not supported in CS"
        | IL _ ->  failwith "References are not supported in IL"

    let withPlatform (platform:ExecutionPlatform) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS _ ->
            let p =
                match platform with
                | ExecutionPlatform.Anycpu -> "anycpu"
                | ExecutionPlatform.AnyCpu32bitPreferred -> "anycpu32bitpreferred"
                | ExecutionPlatform.Itanium -> "Itanium"
                | ExecutionPlatform.X64 -> "x64"
                | ExecutionPlatform.X86 -> "x86"
                | ExecutionPlatform.Arm -> "arm"
                | ExecutionPlatform.Arm64 -> "arm64"
                | _ -> failwith $"Unknown value for ExecutionPlatform: {platform}"

            withOptionsHelper [ $"--platform:{p}" ] "withPlatform is only supported for F#" cUnit
        | _ -> failwith "TODO: Implement ignorewarnings for the rest."

    let ignoreWarnings (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with IgnoreWarnings = true }
        | _ -> failwith "TODO: Implement ignorewarnings for the rest."

    let withCulture culture (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ $"--preferreduilang:%s{culture}" ] "preferreduilang is only supported for F#" cUnit

    let rec private asMetadataReference (cUnit: CompilationUnit) reference =
        match reference with
        | CompilationReference (cmpl, _) ->
            let result = compileFSharpCompilation cmpl false cUnit
            match result with
            | CompilationResult.Failure f ->
                let message = sprintf "Operation failed (expected to succeed).\n All errors:\n%A" (f.Diagnostics)
                failwith message
            | CompilationResult.Success s ->
                match s.OutputPath with
                    | None -> failwith "Operation didn't produce any output!"
                    | Some p -> p |> MetadataReference.CreateFromFile
        | _ -> failwith "Conversion isn't possible"

    let private processReferences (references: CompilationUnit list) defaultOutputDirectory =
        let rec loop acc = function
            | [] -> List.rev acc
            | x::xs ->
                match x with
                | FS fs ->
                    let refs = loop [] fs.References
                    let options = fs.Options |> List.toArray
                    let name = defaultArg fs.Name null
                    let outDir =
                        match fs.OutputDirectory with
                        | Some outputDirectory -> outputDirectory
                        | _ -> defaultOutputDirectory
                    let cmpl =
                        CompilationReference.CreateFSharp(Compilation.CreateFromSources([fs.Source] @ fs.AdditionalSources, fs.OutputType, options, fs.TargetFramework, refs, name, outDir), fs.StaticLink)
                    loop (cmpl::acc) xs

                | CS cs ->
                    let refs = loop [] cs.References
                    let name = defaultArg cs.Name null
                    let metadataReferences = List.map (asMetadataReference x) refs
                    let cmpl =
                        CompilationUtil.CreateCSharpCompilation(cs.Source, cs.LangVersion, cs.TargetFramework, additionalReferences = metadataReferences.ToImmutableArray().As<PortableExecutableReference>(), name = name)
                        |> CompilationReference.Create
                    loop (cmpl::acc) xs

                | IL _ -> failwith "TODO: Process references for IL"
        loop [] references

    let private compileFSharpCompilation compilation ignoreWarnings (cUnit: CompilationUnit) : CompilationResult =

        use capture = new TestConsole.ExecutionCapture()

        let ((err: FSharpDiagnostic[], exn, outputFilePath: string), deps) =
            CompilerAssert.CompileRaw(compilation, ignoreWarnings)

        // Create and stash the console output
        let diagnostics = err |> fromFSharpDiagnostic

        let outcome = exn |> Option.map Failure |> Option.defaultValue NoExitCode

        let result = {
            OutputPath    = None
            Dependencies  = deps
            Adjust        = 0
            PerFileErrors = diagnostics
            Diagnostics   = diagnostics |> List.map snd
            Output        = Some (RunOutput.ExecutionOutput { Outcome = outcome; StdOut = capture.OutText; StdErr = capture.ErrorText })
            Compilation   = cUnit
        }

        let (errors, warnings) = partitionErrors result.Diagnostics

        // Treat warnings as errors if "IgnoreWarnings" is false
        if errors.Length > 0 || (warnings.Length > 0 && not ignoreWarnings) then
            CompilationResult.Failure result
        else
            CompilationResult.Success { result with OutputPath = Some outputFilePath }

    let private compileFSharp (fs: FSharpCompilationSource) : CompilationResult =
        let output = fs.OutputType
        let options = fs.Options |> Array.ofList
        let name = defaultArg fs.Name null
        let outputDirectory =
            match fs.OutputDirectory with
            | Some di -> di
            | None -> createTemporaryDirectory()
        let references = processReferences fs.References outputDirectory
        let compilation = Compilation.CreateFromSources([fs.Source] @ fs.AdditionalSources, output, options, fs.TargetFramework, references, name, outputDirectory)
        compileFSharpCompilation compilation fs.IgnoreWarnings (FS fs)

    let toErrorInfo (d: Diagnostic) =
        let span = d.Location.GetMappedLineSpan().Span
        let number = d.Id |> Seq.where Char.IsDigit |> String.Concat |> int

        { Error =
            match d.Severity with
            | DiagnosticSeverity.Error -> Error
            | DiagnosticSeverity.Warning -> Warning
            | DiagnosticSeverity.Info -> Information
            | DiagnosticSeverity.Hidden -> Hidden
            | x -> failwith $"Unknown severity {x}"
            |> (|>) number
          Range =
            { StartLine = span.Start.Line
              StartColumn = span.Start.Character
              EndLine = span.End.Line
              EndColumn = span.End.Character }
          NativeRange = Unchecked.defaultof<_>
          SubCategory = ""
          Message = d.GetMessage() }

    let private compileCSharpCompilation (compilation: CSharpCompilation) csSource (filePath : string) dependencies : CompilationResult =
        let cmplResult = compilation.Emit filePath
        let result =
            { OutputPath   = None
              Dependencies = dependencies
              Adjust       = 0
              Diagnostics  = cmplResult.Diagnostics |> Seq.map toErrorInfo |> Seq.toList
              PerFileErrors= List.empty // Not needed for C# testing for now. Implement when needed
              Output       = None
              Compilation  = CS csSource }

        if cmplResult.Success then
            CompilationResult.Success { result with OutputPath  = Some filePath }
        else
            CompilationResult.Failure result

    let private compileCSharp (csSource: CSharpCompilationSource) : CompilationResult =

        let source = csSource.Source.GetSourceText |> Option.defaultValue ""
        let name = defaultArg csSource.Name (getTemporaryFileName())

        let outputDirectory =
            match csSource.OutputDirectory with
            | Some di -> di
            | None -> createTemporaryDirectory()

        let additionalReferences =
            processReferences csSource.References outputDirectory
            |> List.map (asMetadataReference (CS csSource))

        let additionalMetadataReferences = additionalReferences.ToImmutableArray().As<MetadataReference>()

        let additionalReferencePaths = [for r in additionalReferences -> r.FilePath]

        let references = TargetFrameworkUtil.getReferences csSource.TargetFramework

        let lv = CSharpLanguageVersion.toLanguageVersion csSource.LangVersion

        let outputKind, extension =
            match csSource.OutputType with
            | Exe -> OutputKind.ConsoleApplication, "exe"
            | Library -> OutputKind.DynamicallyLinkedLibrary, "dll"
            | Module -> OutputKind.NetModule, "mod"

        let cmpl =
          CSharpCompilation.Create(
            name,
            [ CSharpSyntaxTree.ParseText (source, CSharpParseOptions lv) ],
            references.As<MetadataReference>().AddRange additionalMetadataReferences,
            CSharpCompilationOptions outputKind)

        let filename = Path.ChangeExtension(cmpl.AssemblyName, extension)
        let filePath = Path.Combine(outputDirectory.FullName, filename)

        compileCSharpCompilation cmpl csSource filePath additionalReferencePaths

    let compile (cUnit: CompilationUnit) : CompilationResult =
        match cUnit with
        | FS fs -> compileFSharp fs
        | CS cs -> compileCSharp cs
        | _ -> failwith "TODO"

    let private getAssemblyInBytes (result: CompilationResult) =
        match result with
        | CompilationResult.Success output ->
            match output.OutputPath with
            | Some filePath -> File.ReadAllBytes(filePath)
            | _ -> failwith "Output path not found."
        | _ ->
            failwith "Compilation has errors."

    let getAssembly = getAssemblyInBytes >> Assembly.Load

    let withPeReader func compilationResult =
        let bytes = getAssemblyInBytes compilationResult
        use reader = new PEReader(bytes.ToImmutableArray())
        func reader

    let withMetadataReader func =
        withPeReader (fun reader -> reader.GetMetadataReader() |> func)

    let compileGuid cUnit =
        cUnit
        |> compile
        |> shouldSucceed
        |> withMetadataReader (fun reader -> reader.GetModuleDefinition().Mvid |> reader.GetGuid)

    let compileAssembly cUnit =
        cUnit
        |> compile
        |> shouldSucceed
        |> getAssembly

    let private parseFSharp (fsSource: FSharpCompilationSource) : CompilationResult =
        let source = fsSource.Source.GetSourceText |> Option.defaultValue ""
        let fileName = fsSource.Source.ChangeExtension.GetSourceFileName
        let parseResults = CompilerAssert.Parse(source, fileName = fileName)
        let failed = parseResults.ParseHadErrors
        let diagnostics =  parseResults.Diagnostics |> fromFSharpDiagnostic
        let result =
            { OutputPath   = None
              Dependencies = []
              Adjust       = 0
              Diagnostics  = diagnostics |> List.map snd
              PerFileErrors= diagnostics
              Output       = None
              Compilation  = FS fsSource }

        if failed then
            CompilationResult.Failure result
        else
            CompilationResult.Success result

    let parse (cUnit: CompilationUnit) : CompilationResult =
        match cUnit with
        | FS fs -> parseFSharp fs
        | _ -> failwith "Parsing only supported for F#."

    let private typecheckFSharpSourceAndReturnErrors (fsSource: FSharpCompilationSource) : FSharpDiagnostic [] =
        let source =
            match fsSource.Source.GetSourceText with
            | None -> File.ReadAllText(fsSource.Source.GetSourceFileName)
            | Some text -> text
        let options = fsSource.Options |> Array.ofList
        let (err: FSharpDiagnostic []) = CompilerAssert.TypeCheckWithOptionsAndName options (fsSource.Name |> Option.defaultValue "test.fs") source
        err

    let private typecheckFSharpSource (fsSource: FSharpCompilationSource) : CompilationResult =
        let (err: FSharpDiagnostic []) = typecheckFSharpSourceAndReturnErrors fsSource
        let diagnostics = err |> fromFSharpDiagnostic
        let result =
            { OutputPath   = None
              Dependencies = []
              Adjust       = 0
              Diagnostics  = diagnostics |> List.map snd
              PerFileErrors= diagnostics
              Output       = None
              Compilation  = FS fsSource }
        let (errors, warnings) = partitionErrors result.Diagnostics

        // Treat warnings as errors if "IgnoreWarnings" is false;
        if errors.Length > 0 || (warnings.Length > 0 && not fsSource.IgnoreWarnings) then
            CompilationResult.Failure result
        else
            CompilationResult.Success result

    let private typecheckFSharp (fsSource: FSharpCompilationSource) : CompilationResult =
        match fsSource.Source with
        | _ -> typecheckFSharpSource fsSource

    let typecheck (cUnit: CompilationUnit) : CompilationResult =
        match cUnit with
        | FS fs -> typecheckFSharp fs
        | _ -> failwith "Typecheck only supports F#"

    let typecheckResults (cUnit: CompilationUnit) : FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults =
        match cUnit with
        | FS fsSource ->
            let source = fsSource.Source.GetSourceText |> Option.defaultValue ""
            let fileName = fsSource.Source.ChangeExtension.GetSourceFileName

            let references =
                let outputDirectory =
                    match fsSource.OutputDirectory with
                    | Some di -> di
                    | None -> createTemporaryDirectory()
                let references = processReferences fsSource.References outputDirectory
                if references.IsEmpty then
                    Array.empty
                else
                    outputDirectory.Create()
                    // Note that only the references are relevant here
                    let compilation = Compilation.Compilation([], CompileOutput.Exe,Array.empty, TargetFramework.Current, references, None, None)
                    evaluateReferences outputDirectory fsSource.IgnoreWarnings compilation
                    |> fst
            
            let options =
                [|
                    yield! fsSource.Options |> Array.ofList
                    yield! references
                |]
            CompilerAssert.TypeCheck(options, fileName, source)
        | _ -> failwith "Typecheck only supports F#"

    let typecheckProject enablePartialTypeChecking useTransparentCompiler (cUnit: CompilationUnit) : FSharp.Compiler.CodeAnalysis.FSharpCheckProjectResults =
        match cUnit with
        | FS fsSource ->
            let options = fsSource.Options |> Array.ofList
            let sourceFiles =
                [| yield (fsSource.Source.GetSourceFileName, fsSource.Source.GetSourceText)
                   yield!
                    fsSource.AdditionalSources
                    |> List.map (fun source -> source.GetSourceFileName, source.GetSourceText) |]

            let getSourceText =
                let project = Map.ofArray sourceFiles
                fun (name: string) ->
                    Map.tryFind name project
                    |> Option.bind (Option.map SourceText.ofString)
                    |> async.Return

            let sourceFiles = Array.map fst sourceFiles

            CompilerAssert.TypeCheckProject(options, sourceFiles, getSourceText, enablePartialTypeChecking, useTransparentCompiler)
        | _ -> failwith "Typecheck only supports F#"

    let run (result: CompilationResult) : CompilationResult =
        match result with
        | CompilationResult.Failure f -> failwith (sprintf "Compilation should be successful in order to run.\n Errors: %A" (f.Diagnostics))
        | CompilationResult.Success s ->
            match s.OutputPath with
            | None -> failwith "Compilation didn't produce any output. Unable to run. (Did you forget to set output type to Exe?)"
            | Some p ->
                let isFsx =
                    match s.Compilation with
                    | FS fs ->
                        match fs.Source with
                        | SourceCodeFileKind.Fsx _ -> true
                        | _ -> false
                    | _ -> false
                let output = CompilerAssert.ExecuteAndReturnResult (p, isFsx, s.Dependencies, false)
                let executionResult = { s with Output = Some (ExecutionOutput output) }
                match output.Outcome with
                | Failure _ -> CompilationResult.Failure executionResult
                | _  -> CompilationResult.Success executionResult

    let compileAndRun = compile >> run

    let compileExeAndRun = asExe >> compileAndRun

    let private processScriptResults fs (evalResult: Result<FsiValue option, exn>, err: FSharpDiagnostic[]) outputWritten errorsWritten =
        let perFileDiagnostics = err |> fromFSharpDiagnostic
        let diagnostics = perFileDiagnostics |> List.map snd
        let (errors, warnings) = partitionErrors diagnostics
        let result =
            { OutputPath   = None
              Dependencies = []
              Adjust       = 0
              Diagnostics  = if fs.IgnoreWarnings then errors else diagnostics
              PerFileErrors = perFileDiagnostics
              Output       = Some (EvalOutput ({Result = evalResult; StdOut = outputWritten; StdErr = errorsWritten}))
              Compilation  = FS fs }

        let evalError = match evalResult with Ok _ -> false | _ -> true
        if evalError || errors.Length > 0 || (warnings.Length > 0 && not fs.IgnoreWarnings) then
            CompilationResult.Failure result
        else
            CompilationResult.Success result


    let private evalFSharp (fs: FSharpCompilationSource) (script:FSharpScript) : CompilationResult =
        let source = fs.Source.GetSourceText |> Option.defaultValue ""
        use capture = new TestConsole.ExecutionCapture()
        let result = script.Eval(source)
        let outputWritten, errorsWritten = capture.OutText, capture.ErrorText
        processScriptResults fs result outputWritten errorsWritten

    let scriptingShim = Path.Combine(__SOURCE_DIRECTORY__,"ScriptingShims.fsx")
    let private evalScriptFromDisk (fs: FSharpCompilationSource) (script:FSharpScript) : CompilationResult =

        let fileNames =
            (fs.Source :: fs.AdditionalSources)
            |> List.map (fun x -> x.GetSourceFileName)
            |> List.insertAt 0 scriptingShim
            |> List.map (sprintf " @\"%s\"")
            |> String.Concat

        use capture = new TestConsole.ExecutionCapture()
        let result = script.Eval("#load " + fileNames)
        let outputWritten, errorsWritten = capture.OutText, capture.ErrorText
        processScriptResults fs result outputWritten errorsWritten

    let eval (cUnit: CompilationUnit) : CompilationResult =
        match cUnit with
        | FS fs ->
            let options = fs.Options |> Array.ofList
            use script = new FSharpScript(additionalArgs=options)
            evalFSharp fs script
        | _ -> failwith "Script evaluation is only supported for F#."

    let getSessionForEval args version = new FSharpScript(additionalArgs=args,quiet=true,langVersion=version)

    let evalInSharedSession (script:FSharpScript) (cUnit: CompilationUnit)  : CompilationResult =
        match cUnit with
        | FS fs -> evalFSharp fs script
        | _ -> failwith "Script evaluation is only supported for F#."

    let evalScriptFromDiskInSharedSession (script:FSharpScript) (cUnit: CompilationUnit) : CompilationResult =
        match cUnit with
        | FS fs -> evalScriptFromDisk fs script
        | _ -> failwith "Script evaluation is only supported for F#."

    let runFsi (cUnit: CompilationUnit) : CompilationResult =
        match cUnit with
        | FS fs ->
            let source = fs.Source.GetSourceText |> Option.defaultValue ""
            let name = fs.Name |> Option.defaultValue "unnamed"
            let options = fs.Options |> Array.ofList
            let outputDirectory =
                match fs.OutputDirectory with
                | Some di -> di
                | None -> createTemporaryDirectory()
            outputDirectory.Create()

            let references = processReferences fs.References outputDirectory
            let cmpl = Compilation.Create(fs.Source, fs.OutputType, options, fs.TargetFramework, references, name, outputDirectory)
            let _compilationRefs, _deps = evaluateReferences outputDirectory fs.IgnoreWarnings cmpl
            let options =
                let opts = new ResizeArray<string>(fs.Options)

                // For every built reference add a -I path so that fsi can find it easily
                for reference in references do
                    match reference with
                    | CompilationReference( cmpl, _) ->
                        match cmpl with
                        | Compilation(_sources, _outputType, _options, _targetFramework, _references, _name, outputDirectory) ->
                            if outputDirectory.IsSome then
                                opts.Add($"-I:\"{(outputDirectory.Value.FullName)}\"")
                    | _ -> ()
                opts.ToArray()
            let errors, stdOut, stdErr = CompilerAssert.RunScriptWithOptionsAndReturnResult options source

            let mkResult output =
              { OutputPath   = None
                Dependencies = []
                Adjust       = 0
                Diagnostics  = []
                PerFileErrors= []
                Output       = Some output
                Compilation  = cUnit }

            if errors.Count = 0 then
                let output =
                    ExecutionOutput { Outcome = NoExitCode; StdOut = stdOut; StdErr = stdErr }
                CompilationResult.Success (mkResult output)
            else
                let err = (errors |> String.concat "\n").Replace("\r\n","\n")
                let output = 
                    ExecutionOutput {Outcome = NoExitCode; StdOut = String.Empty; StdErr = err }
                CompilationResult.Failure (mkResult output)

        | _ -> failwith "FSI running only supports F#."


    let convenienceBaselineInstructions baseline expected actual =
        $"""to update baseline:
$ cp {baseline.FilePath} {baseline.BslSource}
to compare baseline:
$ code --diff {baseline.FilePath} {baseline.BslSource}
Expected:
{expected}
Actual:
{actual}"""
    let updateBaseline () =
        snd (Int32.TryParse(Environment.GetEnvironmentVariable("TEST_UPDATE_BSL"))) <> 0
    let updateBaseLineIfEnvironmentSaysSo baseline =
        if updateBaseline () then
            if FileSystem.FileExistsShim baseline.FilePath then
                FileSystem.CopyShim(baseline.FilePath, baseline.BslSource, true)

    let assertBaseline expected actual baseline fOnFail =
        if expected <> actual then
            fOnFail()
            updateBaseLineIfEnvironmentSaysSo baseline
            createBaselineErrors baseline actual
            Assert.True((expected = actual), convenienceBaselineInstructions baseline expected actual)
        elif FileSystem.FileExistsShim baseline.FilePath then
            FileSystem.FileDeleteShim baseline.FilePath

    
    let private createBaselineErrors (baselineFile: BaselineFile) (actualErrors: string) : unit =
        printfn $"creating baseline error file for convenience: {baselineFile.FilePath}, expected: {baselineFile.BslSource}"
        let file = FileSystem.OpenFileForWriteShim(baselineFile.FilePath)
        file.SetLength(0)
        file.WriteAllText(actualErrors)

    let private verifyFSBaseline fs : unit =
        match fs.Baseline with
        | None -> failwith "Baseline was not provided."
        | Some bsl ->
            let errorsExpectedBaseLine =
                match bsl.FSBaseline.Content with
                | Some b -> b.Replace("\r\n","\n")
                | None ->  String.Empty

            let typecheckDiagnostics = fs |> typecheckFSharpSourceAndReturnErrors

            let errorsActual =
                (typecheckDiagnostics
                |> Array.map (sprintf "%A")
                |> String.concat "\n"
                ).Replace("\r\n","\n")

            if errorsExpectedBaseLine <> errorsActual then
                fs.CreateOutputDirectory()
                createBaselineErrors bsl.FSBaseline errorsActual
                updateBaseLineIfEnvironmentSaysSo bsl.FSBaseline
                let errorMsg = (convenienceBaselineInstructions bsl.FSBaseline errorsExpectedBaseLine errorsActual)
                Assert.True((errorsExpectedBaseLine = errorsActual), errorMsg)
            elif FileSystem.FileExistsShim(bsl.FSBaseline.FilePath) then
                FileSystem.FileDeleteShim(bsl.FSBaseline.FilePath)

    /// Check the typechecker output against the baseline, if invoked with empty baseline, will expect no error/warnings output.
    let verifyBaseline (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> (verifyFSBaseline fs) |> ignore
        | _ -> failwith "Baseline tests are only supported for F#."

        cUnit

    let private doILCheck func (il: string list) result =
        match result with
        | CompilationResult.Success s ->
            match s.OutputPath with
            | None -> failwith "Operation didn't produce any output!"
            | Some p -> func p il
        | CompilationResult.Failure f -> failwith $"Result should be \"Success\" in order to get IL. Failure: {Environment.NewLine}{f}"

    let withILContains expected result : CompilationResult =
        match result with
        | CompilationResult.Success s ->
            match s.OutputPath with
            | None -> failwith "Operation didn't produce any output!"
            | Some p ->
                match ILChecker.verifyILAndReturnActual [] p expected with
                | true, _, _ -> result
                | false, errorMsg, _actualIL -> 
                    CompilationResult.Failure( {s with Output = Some (ExecutionOutput {Outcome = NoExitCode; StdOut = errorMsg; StdErr = ""})} )
        | CompilationResult.Failure f ->
            printfn "Failure:"
            printfn $"{f}"
            failwith $"Result should be \"Success\" in order to get IL."

    let verifyIL = doILCheck ILChecker.checkIL

    let verifyILNotPresent = doILCheck ILChecker.checkILNotPresent

    let verifyILBinary (il: string list) (dll: string)= ILChecker.checkIL dll il

    let private verifyFSILBaseline (baseline: Baseline) (result: CompilationOutput) : unit =
        match result.OutputPath with
        | None -> failwith "Operation didn't produce any output!"
        | Some p ->
            let expectedIL =
                match baseline.ILBaseline.Content with
                | Some b -> b
                | None ->  String.Empty
            let success, errorMsg, actualIL = ILChecker.verifyILAndReturnActual [] p [expectedIL]

            if not success then
                // Failed try update baselines if required
                // If we are here then the il file has been produced we can write it back to the baseline location
                // if the environment variable TEST_UPDATE_BSL has been set
                updateBaseLineIfEnvironmentSaysSo baseline.ILBaseline
                createBaselineErrors baseline.ILBaseline actualIL
                let errorMsg = (convenienceBaselineInstructions baseline.ILBaseline expectedIL actualIL) + errorMsg
                Assert.Fail(errorMsg)

    let verifyILBaseline (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs ->
            match fs |> compileFSharp, fs.Baseline with
            | CompilationResult.Failure a, Some baseline ->
                match baseline.ILBaseline.Content with
                | Some "" -> ()
                | Some il ->
                    failwith $"Build failure: {a} while expected il\n{il}"
                | None ->
                    if not (FileSystem.FileExistsShim baseline.ILBaseline.BslSource) && updateBaseline () then
                        File.WriteAllText(baseline.ILBaseline.BslSource, "")
                    else
                        failwith $"Build failure empty baseline at {baseline.ILBaseline.BslSource}: {a}"
            | CompilationResult.Success s, Some baseline -> verifyFSILBaseline baseline s
            | _, None ->
                failwithf $"Baseline was not provided."
        | _ -> failwith "Baseline tests are only supported for F#."

        cUnit

    let verifyBaselines = verifyBaseline >> verifyILBaseline

    let normalizeNewlines output =
        let regex = new Regex("(\r\n|\r|\n)", RegexOptions.Singleline ||| RegexOptions.ExplicitCapture)
        let result = regex.Replace(output, System.Environment.NewLine)
        result

    let regexStrip output pattern flags =
        let regex = new Regex(pattern, flags)
        let result = regex.Replace(output, "")
        result

    let stripEnvironment output =
        let pattern = @"(---------------------------------------------------------------(\r\n|\r|\n)).*(\n---------------------------------------------------------------(\r\n|\r|\n))"
        let result = regexStrip output pattern (RegexOptions.Singleline ||| RegexOptions.ExplicitCapture)
        result

    let stripVersion output =
        let pattern = @"(Microsoft \(R\) (.*) version (.*) F# (.*))"
        let result = regexStrip output pattern (RegexOptions.Multiline ||| RegexOptions.ExplicitCapture)
        result

    let getOutput (cResult: CompilationResult) : string option =
        let result =
            match cResult  with
            | CompilationResult.Failure f -> failwith $"Build failure: {f}"
            | CompilationResult.Success output ->
                match output.Output with
                | Some (EvalOutput _) -> None
                | Some (ExecutionOutput eo) ->
                    match eo.StdOut with
                    | null -> None
                    | output -> Some (stripVersion (stripEnvironment (normalizeNewlines output)))
                | None -> None
        result

    let verifyOutput (expected: string) (cResult: CompilationResult) : CompilationResult =
        match getOutput cResult with
        | None -> cResult
        | Some actual ->
            let expected = stripVersion (normalizeNewlines expected)
            if expected <> actual then
                failwith $"""Output does not match expected:{Environment.NewLine}{expected}{Environment.NewLine}Actual:{Environment.NewLine}{actual}{Environment.NewLine}"""
            else
                cResult

    let verifyOutputWithBaseline path =
        verifyOutput (File.ReadAllText(path).Replace(@"\r\n", Environment.NewLine))

    let verifyOutputContains (expected: string array) (cResult: CompilationResult) : CompilationResult =
        match getOutput cResult with
        | None -> cResult
        | Some actual ->
            for item in expected do
                if not(actual.Contains(item)) then
                    failwith $"""Output does not match expected:{Environment.NewLine}{item}{Environment.NewLine}Actual:{Environment.NewLine}{actual}{Environment.NewLine}"""
            cResult

    type ImportScope = { Kind: ImportDefinitionKind; Name: string }

    type PdbVerificationOption =
    | VerifyImportScopes of ImportScope list list
    | VerifySequencePoints of (Line * Col * Line * Col) list
    | VerifyDocuments of string list
    | Dummy of unit

    let private verifyPdbFormat (reader: MetadataReader) compilationType =
        if reader.MetadataVersion <> "PDB v1.0" then
            failwith $"Invalid PDB file version. Expected: \"PDB v1.0\"; Got {reader.MetadataVersion}"

        if reader.MetadataKind <> MetadataKind.Ecma335 then
            failwith $"Invalid metadata kind detected. Expected {MetadataKind.Ecma335}; Got {reader.MetadataKind}"

        // This should not happen, just a sanity check:
        if reader.IsAssembly then
            failwith $"Unexpected PDB type, expected `IsAssembly` to be `false`."

        let shouldHaveEntryPoint = (compilationType = CompileOutput.Exe)

        // Sanity check, we want to verify, that Entrypoint is non-nil, if we are building "Exe" target.
        if reader.DebugMetadataHeader.EntryPoint.IsNil && shouldHaveEntryPoint then
            failwith $"EntryPoint expected to be {shouldHaveEntryPoint}, but was {reader.DebugMetadataHeader.EntryPoint.IsNil}"

    let private verifyPdbImportTables (reader: MetadataReader) (scopes: ImportScope list list) =
        // There always should be 2 import scopes - 1 empty "root" one, and one flattened table of imports for current scope.
        if reader.ImportScopes.Count < 2 then
            failwith $"Expected to have at least 2 import scopes, but found {reader.ImportScopes.Count}."

        // Sanity check: explicitly test that first import scope is indeed an empty one (i.e. there are no imports).
        let rootScope = reader.ImportScopes.ToImmutableArray().Item(0) |> reader.GetImportScope

        let rootScopeImportsLength = rootScope.GetImports().ToImmutableArray().Length

        if rootScopeImportsLength <> 0 then
            failwith $"Expected root scope to have 0 imports, but got {rootScopeImportsLength}."

        let pdbScopes = [ for import in reader.ImportScopes -> reader.GetImportScope import ] |> List.skip 1 |> List.rev

        if pdbScopes.Length <> scopes.Length then
            failwith $"Expected import scopes amount is {scopes.Length}, but got {pdbScopes.Length}."

        for (pdbScope, expectedScope) in List.zip pdbScopes scopes do
            let imports = [ for import in pdbScope.GetImports() ->
                            match import.Kind with
                            | ImportDefinitionKind.ImportNamespace ->
                                let targetNamespaceBlob = import.TargetNamespace
                                let targetNamespaceBytes = reader.GetBlobBytes(targetNamespaceBlob)
                                let name = Encoding.UTF8.GetString(targetNamespaceBytes, 0, targetNamespaceBytes.Length)
                                Some { Kind = import.Kind; Name = name }
                            | _ -> None ] |> List.filter Option.isSome |> List.map Option.get

            if expectedScope.Length <> imports.Length then
                failwith $"Expected imports amount is {expectedScope.Length}, but got {imports.Length}\nExpected:\n%A{expectedScope}\nActual:%A{imports}"

            if expectedScope <> imports then
                failwith $"Expected imports are different from PDB.\nExpected:\n%A{expectedScope}\nActual:%A{imports}"

    let private verifySequencePoints (reader: MetadataReader) expectedSequencePoints =
        let sequencePoints =
            [ for sp in reader.MethodDebugInformation do
                let mdi = reader.GetMethodDebugInformation sp
                yield! mdi.GetSequencePoints() ]
            |> List.sortBy (fun sp -> sp.StartLine)
            |> List.map (fun sp -> (Line sp.StartLine, Col sp.StartColumn, Line sp.EndLine, Col sp.EndColumn) )

        if sequencePoints <> expectedSequencePoints then
            failwith $"Expected sequence points are different from PDB.\nExpected: %A{expectedSequencePoints}\nActual: %A{sequencePoints}"

    let private verifyDocuments (reader: MetadataReader) expectedDocuments =
        let documents =
            [ for doc in reader.Documents do
                if not doc.IsNil then
                    let di = reader.GetDocument doc
                    let nmh = di.Name
                    if not nmh.IsNil then
                        let name = reader.GetString nmh
                        name ]
            |> List.sort

        let expectedDocuments = expectedDocuments |> List.sort

        if documents <> expectedDocuments then
            failwith $"Expected documents are different from PDB.\nExpected: %A{expectedDocuments}\nActual: %A{documents}"

    let private verifyPdbOptions optOutputPath reader options =
        let outputPath = Path.GetDirectoryName(optOutputPath |> Option.defaultValue ".")
        for option in options do
            match option with
            | VerifyImportScopes scopes -> verifyPdbImportTables reader scopes
            | VerifySequencePoints sp -> verifySequencePoints reader sp
            | VerifyDocuments docs -> verifyDocuments reader (docs |> List.map(fun doc -> Path.Combine(outputPath, doc)))
            | _ -> failwith $"Unknown verification option: {option.ToString()}"

    let private verifyPortablePdb (result: CompilationOutput) options : unit =
        match result.OutputPath with
        | Some assemblyPath ->
            let pdbPath = Path.ChangeExtension(assemblyPath, ".pdb")
            if not (FileSystem.FileExistsShim pdbPath) then
                failwith $"PDB file does not exists: {pdbPath}"

            use fileStream = File.OpenRead pdbPath;
            use provider = MetadataReaderProvider.FromPortablePdbStream fileStream
            let reader = provider.GetMetadataReader()
            let compilationType =
                match result.Compilation with
                | FS r -> r.OutputType
                | _ -> failwith "Only F# compilations are supported when verifying PDBs."

            verifyPdbFormat reader compilationType
            verifyPdbOptions result.OutputPath reader options
        | _ -> failwith "Output path is not set, please make sure compilation was successful."

        ()

    let verifyPdb (options: PdbVerificationOption list) (result: CompilationResult) : CompilationResult =
        match result with
        | CompilationResult.Success r -> verifyPortablePdb r options
        | _ -> failwith "Result should be \"Success\" in order to verify PDB."

        result

    let verifyHasPdb (result: CompilationResult): unit =
        let verifyPdbExists r =
            match r.OutputPath with
            | Some assemblyPath ->
                let pdbPath = Path.ChangeExtension(assemblyPath, ".pdb")
                if not (FileSystem.FileExistsShim pdbPath) then
                    failwith $"PDB file does not exists: {pdbPath}"
            | _ -> failwith "Output path is not set, please make sure compilation was successful."
        match result with
        | CompilationResult.Success r -> verifyPdbExists r
        | _ -> failwith "Result should be \"Success\" in order to verify PDB."

    let verifyNoPdb (result: CompilationResult): unit =
        let verifyPdbNotExists r =
            match r.OutputPath with
            | Some assemblyPath ->
                let pdbPath = Path.ChangeExtension(assemblyPath, ".pdb")
                if FileSystem.FileExistsShim pdbPath then
                    failwith $"PDB file exists: {pdbPath}"
            | _ -> failwith "Output path is not set, please make sure compilation was successful."
        match result with
        | CompilationResult.Success r -> verifyPdbNotExists r
        | _ -> failwith "Result should be \"Success\" in order to verify PDB."

    [<AutoOpen>]
    module Assertions =
        let private getErrorNumber (error: ErrorType) : int =
            match error with
            | Error e | Warning e | Information e | Hidden e -> e

        let private getErrorInfo (info: ErrorInfo) : string =
            sprintf "%A %A" info.Error info.Message

        let private assertErrorsLength (source: ErrorInfo list) (expected: 'a list) : unit =
            if (List.length source) <> (List.length expected) then
                failwith (sprintf "Expected list of issues differ from compilation result:\nExpected:\n %A\nActual:\n %A" expected (List.map getErrorInfo source))
            ()

        let private assertErrorMessages (source: ErrorInfo list) (expected: string list) : unit =
            for exp in expected do
                if not (List.exists (fun (el: ErrorInfo) ->
                    let msg = el.Message
                    msg = exp) source) then
                    failwith (sprintf "Mismatch in error message, expected '%A' was not found during compilation.\nAll errors:\n%A" exp (List.map getErrorInfo source))
            assertErrorsLength source expected

        let private assertErrorNumbers (source: ErrorInfo list) (expected: int list) : unit =
            for exp in expected do
                if not (List.exists (fun (el: ErrorInfo) -> (getErrorNumber el.Error) = exp) source) then
                    failwith (sprintf "Mismatch in ErrorNumber, expected '%A' was not found during compilation.\nAll errors:\n%A" exp (List.map getErrorInfo source))

        let consequtiveWhiteSpaceTrimmer = new Regex(@"(\r\n|\n|\ |\t)(\ )+")
        let trimExtraSpaces s = consequtiveWhiteSpaceTrimmer.Replace(s,"$1")

        let private assertErrors (what: string) libAdjust (source: ErrorInfo list) (expected: ErrorInfo list) : unit =

            // (Error 67, Line 14, Col 3, Line 14, Col 24, "This type test or downcast will always hold")
            let errorMessage error =
                let { Error = err; Range = range; Message = message } = error
                let message = trimExtraSpaces message
                let errorType =
                    match err with
                    | ErrorType.Error n -> $"Error {n}"
                    | ErrorType.Warning n-> $"Warning {n}"
                    | ErrorType.Hidden n-> $"Hidden {n}"
                    | ErrorType.Information n-> $"Information {n}"
                $"""({errorType}, Line {range.StartLine}, Col {range.StartColumn}, Line {range.EndLine}, Col {range.EndColumn}, "{message}")""".Replace("\r\n", "\n")

            let expectedErrors = expected |> List.map (fun error -> errorMessage error)
            let expectedErrorsAsStr = expectedErrors |> String.concat ";\n" |> sprintf "[%s]"
            let sourceErrors = source |> List.map (fun error -> errorMessage { error with Range = adjustRange error.Range libAdjust })
            let sourceErrorsAsStr = sourceErrors |> String.concat ";\n" |> sprintf "[%s]"

            let inline checkEqual k a b =
             if a <> b then
                 failwith $"%s{what}: Mismatch in %s{k}, expected '%A{a}', got '%A{b}'.\nAll errors:\n%s{sourceErrorsAsStr}\nExpected errors:\n%s{expectedErrorsAsStr}"

            // For lists longer than 100 errors:
            expectedErrors |> List.iter System.Diagnostics.Debug.WriteLine

            // TODO: Check all "categories", collect all results and print alltogether.
            checkEqual "Errors count" expectedErrors.Length sourceErrors.Length

            (sourceErrors, expectedErrors)
            ||> List.iter2 (fun actual expected ->
                Assert.Equal(expected, actual))

        let adjust (adjust: int) (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success s -> CompilationResult.Success { s with Adjust = adjust }
            | CompilationResult.Failure f -> CompilationResult.Failure { f with Adjust = adjust }

        let shouldSucceed (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success _ -> result
            | CompilationResult.Failure r ->
                let messages = r.Diagnostics |> List.map (fun e -> $"%A{e}") |> String.concat ";\n"
                let diagnostics = $"All errors:\n{messages}"

                eprintfn $"\n{diagnostics}"

                match r.Output with
                | Some (EvalOutput { Result = Result.Error ex })
                | Some (ExecutionOutput {Outcome = Failure ex }) ->
                    failwithf $"Eval or Execution has failed (expected to succeed): %A{ex}\n{diagnostics}"
                | _ ->
                    failwithf $"Operation failed (expected to succeed).\n{diagnostics}"

        let shouldFail (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success _ -> failwith "Operation succeeded (expected to fail)."
            | CompilationResult.Failure _ -> result

        let private assertResultsCategory (what: string) (selector: CompilationOutput -> ErrorInfo list) (expected: ErrorInfo list) (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success r
            | CompilationResult.Failure r ->
                assertErrors what r.Adjust (selector r) expected
            result

        let private withResultsIgnoreNativeRange (expectedResults: ErrorInfo list) result : CompilationResult =
            assertResultsCategory "Results" (fun r -> r.Diagnostics) expectedResults result

        let private withResultIgnoreNativeRange (expectedResult: ErrorInfo ) (result: CompilationResult) : CompilationResult =
            withResultsIgnoreNativeRange [expectedResult] result

        let withDiagnostics (expected: (ErrorType * Line * Col * Line * Col * string) list) (result: CompilationResult) : CompilationResult =
            let expectedResults: ErrorInfo list =
                [ for e in expected do
                      let (error, Line startLine, Col startCol, Line endLine, Col endCol, message) = e
                      { Error = error
                        Range =
                            { StartLine   = startLine
                              StartColumn = startCol
                              EndLine     = endLine
                              EndColumn   = endCol }
                        NativeRange = Unchecked.defaultof<_>
                        SubCategory = ""
                        Message     = message } ]
            withResultsIgnoreNativeRange expectedResults result

        let withSingleDiagnostic (expected: (ErrorType * Line * Col * Line * Col * string)) (result: CompilationResult) : CompilationResult =
            withDiagnostics [expected] result

        let withErrors (expectedErrors: ErrorInfo list) (result: CompilationResult) : CompilationResult =
            assertResultsCategory "Errors" (fun r -> getErrors r.Diagnostics) expectedErrors result

        let withError (expectedError: ErrorInfo) (result: CompilationResult) : CompilationResult =
            withErrors [expectedError] result

        module StructuredResultsAsserts =
            type SimpleErrorInfo =
                { Error:   ErrorType
                  Range:   Range
                  Message: string }

            let withResults (expectedResults: SimpleErrorInfo list) result : CompilationResult =
                let mappedResults = expectedResults |> List.map (fun s -> { Error = s.Error;Range = s.Range;  Message = s.Message; NativeRange = Unchecked.defaultof<_>; SubCategory = ""})
                Compiler.Assertions.withResultsIgnoreNativeRange mappedResults result

            let withResult (expectedResult: SimpleErrorInfo ) (result: CompilationResult) : CompilationResult =
                withResults [expectedResult] result

        module TextBasedDiagnosticAsserts =
            open FSharp.Compiler.Text.Range

            let private messageAndNumber errorType=
                match errorType with
                | ErrorType.Error n -> "error",n
                | ErrorType.Warning n-> "warning",n
                | ErrorType.Hidden n
                | ErrorType.Information n-> "info",n

            let normalizeNewLines (s:string) = s.Replace("\r\n","\n").Replace("\n",Environment.NewLine)

            let private renderToString (cr:CompilationResult) =
                [ for (file,err) in cr.Output.PerFileErrors do
                    let m = err.NativeRange
                    let file = file.Replace("/", "\\")
                    let severity,no = messageAndNumber err.Error
                    let adjustedMessage = err.Message |> normalizeNewLines
                    let location =
                        if  (equals m range0) || (equals m rangeStartup) || (equals m rangeCmdArgs) then
                            ""
                        else
                            // The baseline .bsl files use 1-based notation for columns, hence the +1's
                            sprintf "%s(%d,%d,%d,%d):" file m.StartLine (m.StartColumn+1) m.EndLine (m.EndColumn+1)
                    Environment.NewLine + $"{location} {err.SubCategory} {severity} FS%04d{no}: {adjustedMessage}" + Environment.NewLine
                ]
                |> String.Concat

            let withResultsMatchingFile (path:string) (result:CompilationResult) =
                let expectedContent = File.ReadAllText(path) |> normalizeNewLines
                let actualErrors = renderToString result

                match Environment.GetEnvironmentVariable("TEST_UPDATE_BSL") with
                | null -> ()
                | _ when expectedContent = actualErrors -> ()
                | _ -> File.WriteAllText(path, actualErrors)
                //File.WriteAllText(path, actualErrors)

                match Assert.shouldBeSameMultilineStringSets expectedContent actualErrors with
                | None -> ()
                | Some diff -> Assert.True(String.IsNullOrEmpty(diff), path)

                result

        let checkCodes (expected: int list) (selector: CompilationOutput -> ErrorInfo list) (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success r
            | CompilationResult.Failure r ->
                assertErrorNumbers (selector r) expected
            result

        let withErrorCodes (expectedCodes: int list) (result: CompilationResult) : CompilationResult =
            checkCodes expectedCodes (fun r -> getErrors r.Diagnostics) result

        let withErrorCode (expectedCode: int) (result: CompilationResult) : CompilationResult =
            withErrorCodes [expectedCode] result

        let withWarnings (expectedWarnings: ErrorInfo list) (result: CompilationResult) : CompilationResult =
            assertResultsCategory "Warnings" (fun r -> getWarnings r.Diagnostics) expectedWarnings result

        let withWarning (expectedWarning: ErrorInfo) (result: CompilationResult) : CompilationResult =
            withWarnings [expectedWarning] result

        let withWarningCodes (expectedCodes: int list) (result: CompilationResult) : CompilationResult =
            checkCodes expectedCodes (fun r -> getWarnings r.Diagnostics) result

        let withWarningCode (expectedCode: int) (result: CompilationResult) : CompilationResult =
            withWarningCodes [expectedCode] result

        let private checkErrorMessages (messages: string list) (selector: CompilationOutput -> ErrorInfo list) (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success r
            | CompilationResult.Failure r -> assertErrorMessages (selector r) messages
            result

        let private diagnosticMatches (pattern: string) (diagnostics: ErrorInfo list) : bool =
            diagnostics |> List.exists (fun d -> Regex.IsMatch(d.Message, pattern))

        let withDiagnosticMessage (message: string) (result: CompilationResult) : CompilationResult =
            let messages = [for d in result.Output.Diagnostics -> d.Message]
            if not (messages |> List.exists ((=) message)) then
                failwith $"Message:\n{message}\n\nwas not found. All diagnostic messages:\n{messages}"
            result

        let withDiagnosticMessageMatches (pattern: string) (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success r
            | CompilationResult.Failure r ->
                if not <| diagnosticMatches pattern r.Diagnostics then
                    failwithf "Expected diagnostic message pattern was not found in compilation diagnostics.\nDiagnostics:\n%A" r.Diagnostics
            result

        let withDiagnosticMessageDoesntMatch (pattern: string) (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success r
            | CompilationResult.Failure r ->
                if diagnosticMatches pattern r.Diagnostics then
                    failwith "Diagnostic message pattern was not expected, but was present."
            result

        let withMessages (messages: string list) (result: CompilationResult) : CompilationResult =
            checkErrorMessages messages (fun r -> r.Diagnostics) result

        let withMessage (message: string) (result: CompilationResult) : CompilationResult =
            withMessages [message] result

        let withErrorMessages (messages: string list) (result: CompilationResult) : CompilationResult =
            checkErrorMessages messages (fun r -> getErrors r.Diagnostics) result

        let withErrorMessage (message: string) (result: CompilationResult) : CompilationResult =
            withErrorMessages [message] result

        let withWarningMessages (messages: string list) (result: CompilationResult) : CompilationResult =
            checkErrorMessages messages (fun r -> getWarnings r.Diagnostics) result

        let withWarningMessage (message: string) (result: CompilationResult) : CompilationResult =
            withWarningMessages [message] result

        let withExitCode (expectedExitCode: int) (result: CompilationResult) : CompilationResult =
            match result.RunOutput with
            | None -> failwith "Execution output is missing, cannot check exit code."
            | Some o ->
                match o with
                | ExecutionOutput {Outcome = ExitCode exitCode} -> Assert.Equal(expectedExitCode, exitCode)
                | _ -> failwith "Cannot check exit code on this run result."
            result

        let private getMatch (input: string) (pattern: string) useWildcards=
            // Escape special characters and replace wildcards with regex equivalents
            if useWildcards then
                let input = input.Replace("\r\n", "\n")
                let pattern = $"""^{Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".")}$"""
                let m = Regex(pattern, RegexOptions.Multiline).Match(input)
                if m.Success then
                    m.Index
                else 
                    -1
            else
                input.IndexOf(pattern) 

        let private checkOutputInOrderCore useWildcards (category: string) (substrings: string list) (selector: ExecutionOutput -> string) (result: CompilationResult) : CompilationResult =
            match result.RunOutput with
            | None ->
                printfn "Execution output is missing cannot check \"%A\"" category
                failwith "Execution output is missing."
            | Some o ->
                match o with
                | ExecutionOutput e ->
                    let input = selector e
                    let mutable searchPos = 0
                    for substring in substrings do
                        match getMatch (input.Substring(searchPos)) substring useWildcards with
                        | -1 -> failwith (sprintf "\nThe following substring:\n    %A\nwas not found in the %A\nOutput:\n    %A" substring category input)
                        | pos -> searchPos <- pos + substring.Length
                | _ -> failwith "Cannot check output on this run result."
            result

        let private checkOutputInOrder category substrings selector result =
            checkOutputInOrderCore false category substrings selector result

        let withOutputContainsAllInOrder (substrings: string list) (result: CompilationResult) : CompilationResult =
            checkOutputInOrder "STDERR/STDOUT" substrings (fun o -> o.StdOut + "\n" + o.StdErr) result

        let withStdOutContains (substring: string) (result: CompilationResult) : CompilationResult =
            checkOutputInOrder "STDOUT" [substring] (fun o -> o.StdOut)  result

        let withStdOutContainsAllInOrder (substrings: string list) (result: CompilationResult) : CompilationResult =
            checkOutputInOrder "STDOUT" substrings (fun o -> o.StdOut) result

        let withStdErrContainsAllInOrder (substrings: string list) (result: CompilationResult) : CompilationResult =
            checkOutputInOrder "STDERR" substrings (fun o -> o.StdErr) result

        let withStdErrContains (substring: string) (result: CompilationResult) : CompilationResult =
            checkOutputInOrder "STDERR" [substring] (fun o -> o.StdErr) result

        let private checkOutputInOrderWithWildcards category substrings selector result =
            checkOutputInOrderCore true category substrings selector result

        let withOutputContainsAllInOrderWithWildcards (substrings: string list) (result: CompilationResult) : CompilationResult =
            checkOutputInOrderWithWildcards "STDERR/STDOUT" substrings (fun o -> o.StdOut + "\n" + o.StdErr) result

        let withStdOutContainsWithWildcards (substring: string) (result: CompilationResult) : CompilationResult =
            checkOutputInOrderWithWildcards "STDOUT" [substring] (fun o -> o.StdOut)  result

        let withStdOutContainsAllInOrderWithWildcards (substrings: string list) (result: CompilationResult) : CompilationResult =
            checkOutputInOrderWithWildcards "STDOUT" substrings (fun o -> o.StdOut) result

        let withStdErrContainsAllInOrderWithWildcards (substrings: string list) (result: CompilationResult) : CompilationResult =
            checkOutputInOrderWithWildcards "STDERR" substrings (fun o -> o.StdErr) result

        let withStdErrContainsWithWildcards (substring: string) (result: CompilationResult) : CompilationResult =
            checkOutputInOrderWithWildcards "STDERR" [substring] (fun o -> o.StdErr) result

        let private assertEvalOutput (selector: FsiValue -> 'T) (value: 'T) (result: CompilationResult) : CompilationResult =
            match result.RunOutput with
            | None -> failwith "Execution output is missing cannot check value."
            | Some (EvalOutput output) ->
                match output.Result with
                | Ok (Some e) -> Assert.Equal<'T>(value, (selector e))
                | Ok None -> failwith "Cannot assert value of evaluation, since it is None."
                | Result.Error ex -> raise ex
            | Some _ -> failwith "Only 'eval' output is supported."
            result

        // TODO: Need to support for:
        // STDIN, to test completions
        // Contains
        // Cancellation
        let withEvalValueEquals (value: 'T) (result: CompilationResult) : CompilationResult =
            assertEvalOutput (fun (x: FsiValue) -> x.ReflectionValue :?> 'T) value result

        let withEvalTypeEquals t (result: CompilationResult) : CompilationResult =
            assertEvalOutput (fun (x: FsiValue) -> x.ReflectionType) t result

    let signatureText (pageWidth: int option) (checkResults: FSharp.Compiler.CodeAnalysis.FSharpCheckFileResults) =
        checkResults.GenerateSignature(?pageWidth = pageWidth)
        |> Option.defaultWith (fun _ -> failwith "Unable to generate signature text.")

    let signaturesShouldContain (expected: string) cUnit =
        let text =
            cUnit
            |> typecheckResults
            |> signatureText None

        let actual =
            text.ToString().Split('\n')
            |> Array.map (fun s -> s.TrimEnd(' ', '\r'))
            |> Array.filter (fun s -> s.Length > 0)

        if not (actual |> Array.contains expected) then
            printfn $"The following signature:\n%s{expected}\n\nwas not found in:"
            actual |> Array.iter (printfn "%s")
            failwith "Expected signature was not found."

    let private printSignaturesImpl pageWidth cUnit  =
        cUnit
        |> typecheckResults
        |> signatureText pageWidth
        |> string
        |> fun s ->
            s.Replace("\r", "").Split('\n')
            |> Array.map (fun line -> line.TrimEnd())
            |> String.concat "\n"

    let printSignatures cUnit = printSignaturesImpl None cUnit
    let printSignaturesWith pageWidth cUnit = printSignaturesImpl (Some pageWidth) cUnit

    let getImpliedSignatureHash cUnit = 
        let tcResults = cUnit |> typecheckResults
        let hash = tcResults.CalculateSignatureHash()
        match hash with
        | Some h -> h
        | None -> failwith "Implied signature hash returned 'None' which should not happen"

