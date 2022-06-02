// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test

open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.IO
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Symbols
open FSharp.Test.Assert
open FSharp.Test.Utilities
open FSharp.Test.ScriptHelpers
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open NUnit.Framework
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
open System.Reflection.Metadata

module rec Compiler =
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
        override this.ToString() = match this with | FS fs -> fs.ToString() | _ -> (sprintf "%A" this   )

    type FSharpCompilationSource =
        { Source:           SourceCodeFileKind
          AdditionalSources:SourceCodeFileKind list
          Baseline:         Baseline option
          Options:          string list
          OutputType:       CompileOutput
          OutputDirectory:  DirectoryInfo option
          Name:             string option
          IgnoreWarnings:   bool
          References:       CompilationUnit list }

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
          Message: string }

    type EvalOutput = Result<FsiValue option, exn>

    type ExecutionOutput =
        { ExitCode: int
          StdOut:   string
          StdErr:   string }

    type RunOutput =
        | EvalOutput of EvalOutput
        | ExecutionOutput of ExecutionOutput

    type CompilationOutput =
        { OutputPath:   string option
          Dependencies: string list
          Adjust:       int
          Diagnostics:  ErrorInfo list
          Output:       RunOutput option
          Compilation:  CompilationUnit }

    [<RequireQualifiedAccess>]
    type CompilationResult =
        | Success of CompilationOutput
        | Failure of CompilationOutput

    type ExecutionPlatform =
        | Anycpu = 0
        | AnyCpu32bitPreferred = 1
        | X86 = 2
        | Itanium = 3
        | X64 = 4
        | Arm = 5
        | Arm64 = 6

    let private defaultOptions : string list = []

    // Not very safe version of reading stuff from file, but we want to fail fast for now if anything goes wrong.
    let private getSource (src: TestType) : string =
        match src with
        | Text t -> t
        | Path p ->
            use stream = FileSystem.OpenFileForReadShim(p)
            stream.ReadAllText()

    // Load the source file from the path
    let loadSourceFromFile path = getSource(TestType.Path path)

    let private fsFromString (source: SourceCodeFileKind): FSharpCompilationSource =
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
        }

    let private csFromString (source: SourceCodeFileKind) : CSharpCompilationSource =
        {
            Source          = source
            LangVersion     = CSharpLanguageVersion.CSharp9
            TargetFramework = TargetFramework.Current
            OutputDirectory= None
            Name            = None
            References      = []
        }

    let private fromFSharpDiagnostic (errors: FSharpDiagnostic[]) : ErrorInfo list =
        let toErrorInfo (e: FSharpDiagnostic) : ErrorInfo =
            let errorNumber = e.ErrorNumber
            let severity = e.Severity

            let error = if severity = FSharpDiagnosticSeverity.Warning then Warning errorNumber else Error errorNumber

            { Error   = error
              Range   =
                  { StartLine   = e.StartLine
                    StartColumn = e.StartColumn
                    EndLine     = e.EndLine
                    EndColumn   = e.EndColumn }
              Message = e.Message }

        errors
        |> List.ofArray
        |> List.distinctBy (fun e -> e.Severity, e.ErrorNumber, e.StartLine, e.StartColumn, e.EndLine, e.EndColumn, e.Message)
        |> List.map toErrorInfo

    let private partitionErrors diagnostics = diagnostics |> List.partition (fun e -> match e.Error with Error _ -> true | _ -> false)

    let private getErrors diagnostics = diagnostics |> List.filter (fun e -> match e.Error with Error _ -> true | _ -> false)

    let private getWarnings diagnostics = diagnostics |> List.filter (fun e -> match e.Error with Warning _ -> true | _ -> false)

    let private adjustRange (range: Range) (adjust: int) : Range =
        { range with
                StartLine   = range.StartLine   - adjust
                StartColumn = range.StartColumn + 1
                EndLine     = range.EndLine     - adjust
                EndColumn   = range.EndColumn   + 1 }

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

    let Fsx (source: string) : CompilationUnit =
        fsFromString (FsxSourceCode source) |> FS

    let FsxFromPath (path: string) : CompilationUnit =
        fsFromString (SourceFromPath path) |> FS

    let Fs (source: string) : CompilationUnit =
        fsFromString (SourceCodeFileKind.Fs({FileName="test.fs"; SourceText=Some source })) |> FS

    let FSharp (source: string) : CompilationUnit =
        fsFromString (SourceCodeFileKind.Fs({FileName="test.fs"; SourceText=Some source })) |> FS

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
        } |> FS

    let CSharp (source: string) : CompilationUnit =
        csFromString (SourceCodeFileKind.Fs({FileName="test.cs"; SourceText=Some source })) |> CS

    let CSharpFromPath (path: string) : CompilationUnit =
        csFromString (SourceFromPath path) |> CS

    let asFsx (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS src -> FS {src with Source=SourceCodeFileKind.Fsx({FileName=src.Source.GetSourceFileName; SourceText=src.Source.GetSourceText})}
        | _ -> failwith "Only F# compilation can be of type Fsx."

    let asFs (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS src -> FS {src with Source=SourceCodeFileKind.Fs({FileName=src.Source.GetSourceFileName; SourceText=src.Source.GetSourceText})}
        | _ -> failwith "Only F# compilation can be of type Fs."

    let withName (name: string) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS src -> FS { src with Name = Some name }
        | CS src -> CS { src with Name = Some name }
        | IL _ -> failwith "IL Compilation cannot be named."

    let withReferences (references: CompilationUnit list) (cUnit: CompilationUnit) : CompilationUnit =
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

    let withDebug (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--debug+" ] "debug+ is only supported on F#" cUnit

    let withNoDebug (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--debug-" ] "debug- is only supported on F#" cUnit

    let withOcamlCompat (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--mlcompatibility" ] "withOcamlCompat is only supported on F#" cUnit

    let withOptions (options: string list) (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper options "withOptions is only supported for F#" cUnit

    let withOutputDirectory (path: string) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with OutputDirectory = Some (DirectoryInfo(path)) }
        | _ -> failwith "withOutputDirectory is only supported on F#"

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

    let withLangVersionPreview (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--langversion:preview" ] "withLangVersionPreview is only supported on F#" cUnit

    let withAssemblyVersion (version:string) (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ $"--version:{version}" ] "withAssemblyVersion is only supported on F#" cUnit

    let withWarnOn  (cUnit: CompilationUnit) warning : CompilationUnit =
        withOptionsHelper [ $"--warnon:{warning}" ] "withWarnOn is only supported for F#" cUnit

    let withNoWarn warning (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ $"--nowarn:{warning}" ] "withNoWarn is only supported for F#" cUnit

    let withNoOptimize (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--optimize-" ] "withNoOptimize is only supported for F#" cUnit

    let withOptimize (cUnit: CompilationUnit) : CompilationUnit =
        withOptionsHelper [ "--optimize+" ] "withOptimize is only supported for F#" cUnit

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

    let asLibrary (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with OutputType = CompileOutput.Library }
        | _ -> failwith "TODO: Implement asLibrary where applicable."

    let asExe (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> FS { fs with OutputType = CompileOutput.Exe }
        | _ -> failwith "TODO: Implement where applicable."

    let withPlatform (platform:ExecutionPlatform) (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS _ -> 
            let p =
                match platform with
                | ExecutionPlatform.Anycpu -> "anycpu"
                | ExecutionPlatform.AnyCpu32bitPreferred -> "anycpu32bitpreferred"
                | ExecutionPlatform.Itanium -> "itanium"
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
                        Compilation.CreateFromSources([fs.Source] @ fs.AdditionalSources, fs.OutputType, options, refs, name, outDir) |> CompilationReference.CreateFSharp
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

        let ((err: FSharpDiagnostic[], outputFilePath: string), deps) = CompilerAssert.CompileRaw(compilation, ignoreWarnings)

        let diagnostics = err |> fromFSharpDiagnostic

        let result =
            { OutputPath   = None
              Dependencies = deps
              Adjust       = 0
              Diagnostics  = diagnostics
              Output       = None
              Compilation  = cUnit }

        let (errors, warnings) = partitionErrors diagnostics

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
            | None -> DirectoryInfo(tryCreateTemporaryDirectory())
        let references = processReferences fs.References outputDirectory
        let compilation = Compilation.CreateFromSources([fs.Source] @ fs.AdditionalSources, output, options, references, name, outputDirectory)
        compileFSharpCompilation compilation fs.IgnoreWarnings (FS fs)

    let private compileCSharpCompilation (compilation: CSharpCompilation) csSource : CompilationResult =
        let outputPath = tryCreateTemporaryDirectory()
        Directory.CreateDirectory(outputPath) |> ignore
        let fileName = compilation.AssemblyName
        let output = Path.Combine(outputPath, Path.ChangeExtension(fileName, ".dll"))
        let cmplResult = compilation.Emit (output)
        let result =
            { OutputPath   = None
              Dependencies = []
              Adjust       = 0
              Diagnostics  = []
              Output       = None 
              Compilation  = CS csSource }

        if cmplResult.Success then
            CompilationResult.Success { result with OutputPath  = Some output }
        else
            CompilationResult.Failure result

    let private compileCSharp (csSource: CSharpCompilationSource) : CompilationResult =

        let source = csSource.Source.GetSourceText |> Option.defaultValue ""
        let name = defaultArg csSource.Name (tryCreateTemporaryFileName())

        let outputDirectory =
            match csSource.OutputDirectory with
            | Some di -> di
            | None -> DirectoryInfo(tryCreateTemporaryDirectory())

        let additionalReferences =
            match processReferences csSource.References outputDirectory with
            | [] -> ImmutableArray.Empty
            | r  -> (List.map (asMetadataReference (CS csSource)) r).ToImmutableArray().As<MetadataReference>()

        let references = TargetFrameworkUtil.getReferences csSource.TargetFramework

        let lv =
          match csSource.LangVersion with
            | CSharpLanguageVersion.CSharp8 -> LanguageVersion.CSharp8
            | CSharpLanguageVersion.CSharp9 -> LanguageVersion.CSharp9
            | CSharpLanguageVersion.Preview -> LanguageVersion.Preview
            | _ -> LanguageVersion.Default

        let cmpl =
          CSharpCompilation.Create(
            name,
            [ CSharpSyntaxTree.ParseText (source, CSharpParseOptions lv) ],
            references.As<MetadataReference>().AddRange additionalReferences,
            CSharpCompilationOptions (OutputKind.DynamicallyLinkedLibrary))

        compileCSharpCompilation cmpl csSource

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

    let compileGuid (cUnit: CompilationUnit) : Guid =
        let bytes =
            compile cUnit
            |> shouldSucceed
            |> getAssemblyInBytes

        use reader1 = new PEReader(bytes.ToImmutableArray())
        let reader1 = reader1.GetMetadataReader()

        reader1.GetModuleDefinition().Mvid |> reader1.GetGuid

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
              Diagnostics  = diagnostics
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
              Diagnostics  = diagnostics
              Output       = None 
              Compilation  = FS fsSource }
        let (errors, warnings) = partitionErrors diagnostics

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
            let options = fsSource.Options |> Array.ofList
            CompilerAssert.TypeCheck(options, fileName, source)
        | _ -> failwith "Typecheck only supports F#"

    let run (result: CompilationResult) : CompilationResult =
        match result with
        | CompilationResult.Failure f -> failwith (sprintf "Compilation should be successful in order to run.\n Errors: %A" (f.Diagnostics))
        | CompilationResult.Success s ->
            match s.OutputPath with
            | None -> failwith "Compilation didn't produce any output. Unable to run. (Did you forget to set output type to Exe?)"
            | Some p ->
                let (exitCode, output, errors) = CompilerAssert.ExecuteAndReturnResult (p, s.Dependencies, false)
                printfn "---------output-------\n%s\n-------"  output
                printfn "---------errors-------\n%s\n-------"  errors
                let executionResult = { s with Output = Some (ExecutionOutput { ExitCode = exitCode; StdOut = output; StdErr = errors }) }
                if exitCode = 0 then
                    CompilationResult.Success executionResult
                else
                    CompilationResult.Failure executionResult

    let compileAndRun = compile >> run

    let compileExeAndRun = asExe >> compileAndRun

    let private evalFSharp (fs: FSharpCompilationSource) : CompilationResult =
        let source = fs.Source.GetSourceText |> Option.defaultValue ""
        let options = fs.Options |> Array.ofList

        use script = new FSharpScript(additionalArgs=options)
        let ((evalresult: Result<FsiValue option, exn>), (err: FSharpDiagnostic[])) = script.Eval(source)
        let diagnostics = err |> fromFSharpDiagnostic
        let result =
            { OutputPath   = None
              Dependencies = []
              Adjust       = 0
              Diagnostics  = diagnostics
              Output       = Some(EvalOutput evalresult) 
              Compilation  = FS fs }

        let (errors, warnings) = partitionErrors diagnostics
        let evalError = match evalresult with Ok _ -> false | _ -> true
        if evalError || errors.Length > 0 || (warnings.Length > 0 && not fs.IgnoreWarnings) then
            CompilationResult.Failure result
        else
            CompilationResult.Success result

    let eval (cUnit: CompilationUnit) : CompilationResult =
        match cUnit with
        | FS fs -> evalFSharp fs
        | _ -> failwith "Script evaluation is only supported for F#."

    let runFsi (cUnit: CompilationUnit) : CompilationResult =
        match cUnit with
        | FS fs ->
            let disposals = ResizeArray<IDisposable>()
            try
                let source = fs.Source.GetSourceText |> Option.defaultValue ""
                let name = fs.Name |> Option.defaultValue "unnamed"
                let options = fs.Options |> Array.ofList
                let outputDirectory =
                    match fs.OutputDirectory with
                    | Some di -> di
                    | None -> DirectoryInfo(tryCreateTemporaryDirectory())
                outputDirectory.Create()
                disposals.Add({ new IDisposable with member _.Dispose() = outputDirectory.Delete(true) })

                let references = processReferences fs.References outputDirectory
                let cmpl = Compilation.Create(fs.Source, fs.OutputType, options, references, name, outputDirectory)
                let _compilationRefs, _deps = evaluateReferences outputDirectory disposals fs.IgnoreWarnings cmpl
                let options =
                    let opts = new ResizeArray<string>(fs.Options)

                    // For every built reference add a -I path so that fsi can find it easily
                    for reference in references do
                        match reference with
                        | CompilationReference( cmpl, _) ->
                            match cmpl with
                            | Compilation(_sources, _outputType, _options, _references, _name, outputDirectory) ->
                                if outputDirectory.IsSome then
                                    opts.Add($"-I:\"{(outputDirectory.Value.FullName)}\"")
                        | _ -> ()
                    opts.ToArray()
                let errors = CompilerAssert.RunScriptWithOptionsAndReturnResult options source

                let result =
                    { OutputPath   = None
                      Dependencies = []
                      Adjust       = 0
                      Diagnostics  = []
                      Output       = None 
                      Compilation  = cUnit }

                if errors.Count > 0 then
                    let output = ExecutionOutput {
                        ExitCode = -1
                        StdOut   = String.Empty
                        StdErr   = ((errors |> String.concat "\n").Replace("\r\n","\n")) }
                    CompilationResult.Failure { result with Output = Some output }
                else
                    CompilationResult.Success result

            finally
                disposals
                |> Seq.iter (fun x -> x.Dispose())

        | _ -> failwith "FSI running only supports F#."


    let private createBaselineErrors (baselineFile: BaselineFile) (actualErrors: string) : unit =
        FileSystem.OpenFileForWriteShim(baselineFile.FilePath + ".err").Write(actualErrors)

    let private verifyFSBaseline (fs) : unit =
        match fs.Baseline with
        | None -> failwith "Baseline was not provided."
        | Some bsl ->
            let errorsExpectedBaseLine =
                match bsl.FSBaseline.Content with
                | Some b -> b.Replace("\r\n","\n")
                | None ->  String.Empty

            let typecheckDiagnostics = fs |> typecheckFSharpSourceAndReturnErrors

            let errorsActual = (typecheckDiagnostics |> Array.map (sprintf "%A") |> String.concat "\n").Replace("\r\n","\n")

            if errorsExpectedBaseLine <> errorsActual then
                fs.CreateOutputDirectory()
                createBaselineErrors bsl.FSBaseline errorsActual
            elif FileSystem.FileExistsShim(bsl.FSBaseline.FilePath) then
                FileSystem.FileDeleteShim(bsl.FSBaseline.FilePath)

            Assert.AreEqual(errorsExpectedBaseLine, errorsActual, $"\nExpected:\n{errorsExpectedBaseLine}\nActual:\n{errorsActual}")

    /// Check the typechecker output against the baseline, if invoked with empty baseline, will expect no error/warnings output.
    let verifyBaseline (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs -> (verifyFSBaseline fs) |> ignore
        | _ -> failwith "Baseline tests are only supported for F#."

        cUnit

    let verifyIL (il: string list) (result: CompilationResult) : unit =
        match result with
        | CompilationResult.Success s ->
            match s.OutputPath with
            | None -> failwith "Operation didn't produce any output!"
            | Some p -> ILChecker.checkIL p il
        | CompilationResult.Failure _ -> failwith "Result should be \"Success\" in order to get IL."

    let verifyILBinary (il: string list) (dll: string)= ILChecker.checkIL dll il

    let private verifyFSILBaseline (baseline: Baseline option) (result: CompilationOutput) : unit =
        match baseline with
        | None -> failwith "Baseline was not provided."
        | Some bsl ->
            match result.OutputPath with
                | None -> failwith "Operation didn't produce any output!"
                | Some p ->
                    let expectedIL =
                        match bsl.ILBaseline.Content with
                        | Some b -> b
                        | None ->  String.Empty
                    let (success, errorMsg, actualIL) = ILChecker.verifyILAndReturnActual p expectedIL

                    if not success then
                        // Failed try update baselines if required
                        // If we are here then the il file has been produced we can write it back to the baseline location
                        // if the environment variable TEST_UPDATE_BSL has been set
                        if snd (Int32.TryParse(Environment.GetEnvironmentVariable("TEST_UPDATE_BSL"))) <> 0 then
                            match baseline with
                            | Some baseline -> System.IO.File.Copy(baseline.ILBaseline.FilePath, baseline.ILBaseline.BslSource, true)
                            | None -> ()

                        createBaselineErrors bsl.ILBaseline actualIL
                        Assert.Fail(errorMsg)

    let verifyILBaseline (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS fs ->
            match fs |> compileFSharp  with
            | CompilationResult.Failure a -> failwith $"Build failure: {a}"
            | CompilationResult.Success s -> verifyFSILBaseline fs.Baseline s
        | _ -> failwith "Baseline tests are only supported for F#."

        cUnit

    let verifyBaselines = verifyBaseline >> verifyILBaseline

    type ImportScope = { Kind: ImportDefinitionKind; Name: string }

    type PdbVerificationOption =
    | VerifyImportScopes of ImportScope list list
    | VerifySequencePoints of (Line * Col * Line * Col) list
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

        // Sanity check: explicitly test that first import scope is indeed an apty one (i.e. there are no imports).
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


    let private verifyPdbOptions reader options =
        for option in options do
            match option with
            | VerifyImportScopes scopes -> verifyPdbImportTables reader scopes
            | VerifySequencePoints sp -> verifySequencePoints reader sp
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
            verifyPdbOptions reader options
        | _ -> failwith "Output path is not set, please make sure compilation was successfull."

        ()

    let verifyPdb (options: PdbVerificationOption list) (result: CompilationResult) : CompilationResult =
        match result with
        | CompilationResult.Success r -> verifyPortablePdb r options
        | _ -> failwith "Result should be \"Success\" in order to verify PDB."

        result

    [<AutoOpen>]
    module Assertions =
        let private getErrorNumber (error: ErrorType) : int =
            match error with
            | Error e | Warning e | Information e | Hidden e -> e

        let private getErrorInfo (info: ErrorInfo) : string =
            sprintf "%A %A" info.Error info.Message

        let inline private assertErrorsLength (source: ErrorInfo list) (expected: 'a list) : unit =
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

        let private assertErrors (what: string) libAdjust (source: ErrorInfo list) (expected: ErrorInfo list) : unit =

            // (Error 67, Line 14, Col 3, Line 14, Col 24, "This type test or downcast will always hold")
            let errorMessage error =
                let { Error = err; Range = range; Message = message } = error
                let errorType =
                    match err with
                    | ErrorType.Error n -> $"Error {n}"
                    | ErrorType.Warning n-> $"Warning {n}"
                    | ErrorType.Hidden n-> $"Hidden {n}"
                    | ErrorType.Information n-> $"Information {n}"
                $"""({errorType}, Line {range.StartLine}, Col {range.StartColumn}, Line {range.EndLine}, Col {range.EndColumn}, "{message}")""".Replace("\r\n", "\n")

            let expectedErrors = expected |> List.map (fun error -> errorMessage error)
            let sourceErrors = source |> List.map (fun error -> errorMessage { error with Range = adjustRange error.Range libAdjust })

            let inline checkEqual k a b =
             if a <> b then
                 Assert.AreEqual(a, b, sprintf "%s: Mismatch in %s, expected '%A', got '%A'.\nAll errors:\n%A\nExpected errors:\n%A" what k a b sourceErrors expectedErrors)

            // For lists longer than 100 errors:
            expectedErrors |> List.iter System.Diagnostics.Debug.WriteLine

            // TODO: Check all "categories", collect all results and print alltogether.
            checkEqual "Errors count" expectedErrors.Length sourceErrors.Length

            (sourceErrors, expectedErrors)
            ||> List.iter2 (fun actual expected ->

                Assert.AreEqual(actual, expected, $"Mismatched error message:\nExpecting: {expected}\nActual:    {actual}\n"))

        let adjust (adjust: int) (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success s -> CompilationResult.Success { s with Adjust = adjust }
            | CompilationResult.Failure f -> CompilationResult.Failure { f with Adjust = adjust }

        let shouldSucceed (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success _ -> result
            | CompilationResult.Failure r ->
                let message = 
                    [ sprintf "Operation failed (expected to succeed).\n All errors:\n%A\n" r.Diagnostics
                      match r.Output with
                      | Some (ExecutionOutput output) ->
                          sprintf "----output-----\n%s\n----error-------\n%s\n----------" output.StdOut output.StdErr
                      | _ -> () ]
                    |> String.concat "\n"
                failwith message

        let shouldFail (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success _ -> failwith "Operation was succeeded (expected to fail)."
            | CompilationResult.Failure _ -> result

        let private assertResultsCategory (what: string) (selector: CompilationOutput -> ErrorInfo list) (expected: ErrorInfo list) (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success r 
            | CompilationResult.Failure r ->
                assertErrors what r.Adjust (selector r) expected
            result

        let withResults (expectedResults: ErrorInfo list) result : CompilationResult =
            assertResultsCategory "Results" (fun r -> r.Diagnostics) expectedResults result

        let withResult (expectedResult: ErrorInfo ) (result: CompilationResult) : CompilationResult =
            withResults [expectedResult] result

        let withDiagnostics (expected: (ErrorType * Line * Col * Line * Col * string) list) (result: CompilationResult) : CompilationResult =
            let (expectedResults: ErrorInfo list) =
                expected |>
                List.map(
                    fun e ->
                      let (error, (Line startLine), (Col startCol), (Line endLine), (Col endCol), message) = e
                      { Error = error
                        Range =
                            { StartLine   = startLine
                              StartColumn = startCol
                              EndLine     = endLine
                              EndColumn   = endCol }
                        Message     = message })
            withResults expectedResults result

        let withSingleDiagnostic (expected: (ErrorType * Line * Col * Line * Col * string)) (result: CompilationResult) : CompilationResult =
            withDiagnostics [expected] result

        let withErrors (expectedErrors: ErrorInfo list) (result: CompilationResult) : CompilationResult =
            assertResultsCategory "Errors" (fun r -> getErrors r.Diagnostics) expectedErrors result

        let withError (expectedError: ErrorInfo) (result: CompilationResult) : CompilationResult =
            withErrors [expectedError] result

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
            match result with
            | CompilationResult.Success r
            | CompilationResult.Failure r ->
                match r.Output with
                | None -> failwith "Execution output is missing, cannot check exit code."
                | Some o ->
                    match o with
                    | ExecutionOutput e -> Assert.AreEqual(e.ExitCode, expectedExitCode, sprintf "Exit code was expected to be: %A, but got %A." expectedExitCode e.ExitCode)
                    | _ -> failwith "Cannot check exit code on this run result."
            result

        let private checkOutput (category: string) (substring: string) (selector: ExecutionOutput -> string) (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success r
            | CompilationResult.Failure r ->
                match r.Output with
                | None -> failwith (sprintf "Execution output is missing cannot check \"%A\"" category)
                | Some o ->
                    match o with
                    | ExecutionOutput e ->
                        let where = selector e
                        if not (where.Contains(substring)) then
                            failwith (sprintf "\nThe following substring:\n    %A\nwas not found in the %A\nOutput:\n    %A" substring category where)
                    | _ -> failwith "Cannot check output on this run result."
            result

        let withOutputContains (substring: string) (result: CompilationResult) : CompilationResult =
            checkOutput "STDERR/STDOUT" substring (fun o -> o.StdOut + "\n" + o.StdErr) result

        let withStdOutContains (substring: string) (result: CompilationResult) : CompilationResult =
            checkOutput "STDOUT" substring (fun o -> o.StdOut) result

        let withStdErrContains (substring: string) (result: CompilationResult) : CompilationResult =
            checkOutput "STDERR" substring (fun o -> o.StdErr) result

        // TODO: probably needs a bit of simplification, + need to remove that pyramid of doom.
        let private assertEvalOutput (selector: FsiValue -> 'T) (value: 'T) (result: CompilationResult) : CompilationResult =
            match result with
            | CompilationResult.Success r
            | CompilationResult.Failure r ->
                match r.Output with
                | None -> failwith "Execution output is missing cannot check value."
                | Some o ->
                    match o with
                    | EvalOutput e ->
                        match e with
                        | Ok v ->
                            match v with
                            | None -> failwith "Cannot assert value of evaluation, since it is None."
                            | Some e -> Assert.AreEqual(value, (selector e))
                        | Result.Error ex -> raise ex
                    | _ -> failwith "Only 'eval' output is supported."
            result

        // TODO: Need to support for:
        // STDIN, to test completions
        // Contains
        // Cancellation
        let withEvalValueEquals (value: 'T) (result: CompilationResult) : CompilationResult =
            assertEvalOutput (fun (x: FsiValue) -> x.ReflectionValue :?> 'T) value result

        let withEvalTypeEquals t (result: CompilationResult) : CompilationResult =
            assertEvalOutput (fun (x: FsiValue) -> x.ReflectionType) t result
