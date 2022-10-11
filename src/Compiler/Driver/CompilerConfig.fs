// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// The configuration of the compiler (TcConfig and TcConfigBuilder)
module internal FSharp.Compiler.CompilerConfig

open System
open System.Collections.Concurrent
open System.IO
open Internal.Utilities
open Internal.Utilities.FSharpEnvironment
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.ILPdbWriter
open FSharp.Compiler.DependencyManager
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.BuildGraph

#if !NO_TYPEPROVIDERS
open FSharp.Core.CompilerServices
#endif

let (++) x s = x @ [ s ]

//----------------------------------------------------------------------------
// Some Globals
//--------------------------------------------------------------------------

let FSharpSigFileSuffixes = [ ".mli"; ".fsi" ]

let FSharpMLCompatFileSuffixes = [ ".mli"; ".ml" ]

let FSharpImplFileSuffixes = [ ".ml"; ".fs"; ".fsscript"; ".fsx" ]

let FSharpScriptFileSuffixes = [ ".fsscript"; ".fsx" ]

let FSharpIndentationAwareSyntaxFileSuffixes =
    [ ".fs"; ".fsscript"; ".fsx"; ".fsi" ]

//--------------------------------------------------------------------------
// General file name resolver
//--------------------------------------------------------------------------

exception FileNameNotResolved of searchedLocations: string * fileName: string * range: range

exception LoadedSourceNotFoundIgnoring of fileName: string * range: range

/// Will return None if the fileName is not found.
let TryResolveFileUsingPaths (paths, m, fileName) =
    let () =
        try
            FileSystem.IsPathRootedShim fileName |> ignore
        with :? ArgumentException as e ->
            error (Error(FSComp.SR.buildProblemWithFilename (fileName, e.Message), m))

    if FileSystem.IsPathRootedShim fileName then
        if FileSystem.FileExistsShim fileName then
            Some fileName
        else
            None
    else
        let res =
            paths
            |> Seq.tryPick (fun path ->
                let n = Path.Combine(path, fileName)
                if FileSystem.FileExistsShim n then Some n else None)

        res

/// Will raise FileNameNotResolved if the fileName was not found
let ResolveFileUsingPaths (paths, m, fileName) =
    match TryResolveFileUsingPaths(paths, m, fileName) with
    | Some res -> res
    | None ->
        let searchMessage = String.concat "\n " paths
        raise (FileNameNotResolved(fileName, searchMessage, m))

let GetWarningNumber (m, warningNumber: string) =
    try
        // Okay so ...
        //      #pragma strips FS of the #pragma "FS0004" and validates the warning number
        //      therefore if we have warning id that starts with a numeric digit we convert it to Some (int32)
        //      anything else is ignored None
        if Char.IsDigit(warningNumber[0]) then
            Some(int32 warningNumber)
        elif warningNumber.StartsWithOrdinal("FS") = true then
            raise (ArgumentException())
        else
            None
    with _ ->
        warning (Error(FSComp.SR.buildInvalidWarningNumber warningNumber, m))
        None

let ComputeMakePathAbsolute implicitIncludeDir (path: string) =
    try
        // remove any quotation marks from the path first
        let path = path.Replace("\"", "")

        if not (FileSystem.IsPathRootedShim path) then
            Path.Combine(implicitIncludeDir, path)
        else
            path
    with :? ArgumentException ->
        path

//----------------------------------------------------------------------------
// Configuration
//----------------------------------------------------------------------------

[<RequireQualifiedAccess>]
type CompilerTarget =
    | WinExe
    | ConsoleExe
    | Dll
    | Module

    member x.IsExe =
        (match x with
         | ConsoleExe
         | WinExe -> true
         | _ -> false)

[<RequireQualifiedAccess>]
type ResolveAssemblyReferenceMode =
    | Speculative
    | ReportErrors

[<RequireQualifiedAccess>]
type CopyFSharpCoreFlag =
    | Yes
    | No

/// Represents the file or string used for the --version flag
type VersionFlag =
    | VersionString of string
    | VersionFile of string
    | VersionNone

    member x.GetVersionInfo implicitIncludeDir =
        let vstr = x.GetVersionString implicitIncludeDir

        try
            parseILVersion vstr
        with _ ->
            errorR (Error(FSComp.SR.buildInvalidVersionString vstr, rangeStartup))
            parseILVersion "0.0.0.0"

    member x.GetVersionString implicitIncludeDir =
        match x with
        | VersionString s -> s
        | VersionFile s ->
            let s =
                if FileSystem.IsPathRootedShim s then
                    s
                else
                    Path.Combine(implicitIncludeDir, s)

            if not (FileSystem.FileExistsShim s) then
                errorR (Error(FSComp.SR.buildInvalidVersionFile s, rangeStartup))
                "0.0.0.0"
            else
                use fs = FileSystem.OpenFileForReadShim(s)
                use is = new StreamReader(fs)
                is.ReadLine()
        | VersionNone -> "0.0.0.0"

/// Represents a reference to an assembly. May be backed by a real assembly on disk, or a cross-project
/// reference backed by information generated by the the compiler service.
type IRawFSharpAssemblyData =
    ///  The raw list AutoOpenAttribute attributes in the assembly
    abstract GetAutoOpenAttributes: unit -> string list

    ///  The raw list InternalsVisibleToAttribute attributes in the assembly
    abstract GetInternalsVisibleToAttributes: unit -> string list

    ///  The raw IL module definition in the assembly, if any. This is not present for cross-project references
    /// in the language service
    abstract TryGetILModuleDef: unit -> ILModuleDef option

    ///  The raw F# signature data in the assembly, if any
    abstract GetRawFSharpSignatureData: range * ilShortAssemName: string * fileName: string -> (string * (unit -> ReadOnlyByteMemory)) list

    ///  The raw F# optimization data in the assembly, if any
    abstract GetRawFSharpOptimizationData:
        range * ilShortAssemName: string * fileName: string -> (string * (unit -> ReadOnlyByteMemory)) list

    ///  The table of type forwarders in the assembly
    abstract GetRawTypeForwarders: unit -> ILExportedTypesAndForwarders

    /// The identity of the module
    abstract ILScopeRef: ILScopeRef

    abstract ILAssemblyRefs: ILAssemblyRef list

    abstract ShortAssemblyName: string

    /// Indicates if the assembly has any F# signature data attribute
    abstract HasAnyFSharpSignatureDataAttribute: bool

    /// Indicates if the assembly has an F# signature data attribute auitable for use with this version of F# tooling
    abstract HasMatchingFSharpSignatureDataAttribute: bool

/// Cache of time stamps as we traverse a project description
type TimeStampCache(defaultTimeStamp: DateTime) =
    let files = ConcurrentDictionary<string, DateTime>()

    let projects =
        ConcurrentDictionary<IProjectReference, DateTime>(HashIdentity.Reference)

    member _.GetFileTimeStamp fileName =
        let ok, v = files.TryGetValue fileName

        if ok then
            v
        else
            let v =
                try
                    FileSystem.GetLastWriteTimeShim fileName
                with :? FileNotFoundException ->  // TODO: .NET API never throws this exception, is it needed?
                    defaultTimeStamp

            files[fileName] <- v
            v

    member cache.GetProjectReferenceTimeStamp(projectReference: IProjectReference) =
        let ok, v = projects.TryGetValue projectReference

        if ok then
            v
        else
            let v = defaultArg (projectReference.TryGetLogicalTimeStamp cache) defaultTimeStamp
            projects[projectReference] <- v
            v

and [<RequireQualifiedAccess>] ProjectAssemblyDataResult =
    | Available of IRawFSharpAssemblyData
    | Unavailable of useOnDiskInstead: bool

and IProjectReference =
    /// The name of the assembly file generated by the project
    abstract FileName: string

    /// Evaluate raw contents of the assembly file generated by the project
    abstract EvaluateRawContents: unit -> NodeCode<ProjectAssemblyDataResult>

    /// Get the logical timestamp that would be the timestamp of the assembly file generated by the project
    ///
    /// For project references this is maximum of the timestamps of all dependent files.
    /// The project is not actually built, nor are any assemblies read, but the timestamps for each dependent file
    /// are read via the FileSystem. If the files don't exist, then a default timestamp is used.
    ///
    /// The operation returns None only if it is not possible to create an IncrementalBuilder for the project at all, e.g. if there
    /// are fatal errors in the options for the project.
    abstract TryGetLogicalTimeStamp: cache: TimeStampCache -> DateTime option

type AssemblyReference =
    | AssemblyReference of range: range * text: string * projectReference: IProjectReference option

    member x.Range = (let (AssemblyReference (m, _, _)) = x in m)

    member x.Text = (let (AssemblyReference (_, text, _)) = x in text)

    member x.ProjectReference = (let (AssemblyReference (_, _, contents)) = x in contents)

    member x.SimpleAssemblyNameIs name =
        (String.Compare(FileSystemUtils.fileNameWithoutExtensionWithValidate false x.Text, name, StringComparison.OrdinalIgnoreCase) = 0)
        || not (x.Text.Contains "/")
           && not (x.Text.Contains "\\")
           && not (x.Text.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
           && not (x.Text.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
           && (try
                   let aname = System.Reflection.AssemblyName x.Text in aname.Name = name
               with _ ->
                   false)

    override x.ToString() = sprintf "AssemblyReference(%s)" x.Text

type UnresolvedAssemblyReference = UnresolvedAssemblyReference of string * AssemblyReference list
#if !NO_TYPEPROVIDERS
type ResolvedExtensionReference = ResolvedExtensionReference of string * AssemblyReference list * Tainted<ITypeProvider> list
#endif

type ImportedAssembly =
    {
        ILScopeRef: ILScopeRef
        FSharpViewOfMetadata: CcuThunk
        AssemblyAutoOpenAttributes: string list
        AssemblyInternalsVisibleToAttributes: string list
#if !NO_TYPEPROVIDERS
        IsProviderGenerated: bool
        mutable TypeProviders: Tainted<ITypeProvider> list
#endif
        FSharpOptimizationData: Microsoft.FSharp.Control.Lazy<Option<Optimizer.LazyModuleInfo>>
    }

type AvailableImportedAssembly =
    | ResolvedImportedAssembly of ImportedAssembly
    | UnresolvedImportedAssembly of string

type CcuLoadFailureAction =
    | RaiseError
    | ReturnNone

type Directive =
    | Resolution
    | Include

type LStatus =
    | Unprocessed
    | Processed

type TokenizeOption =
    | AndCompile
    | Only
    | Unfiltered

type PackageManagerLine =
    {
        Directive: Directive
        LineStatus: LStatus
        Line: string
        Range: range
    }

    static member AddLineWithKey
        (packageKey: string)
        (directive: Directive)
        (line: string)
        (m: range)
        (packageManagerLines: Map<string, PackageManagerLine list>)
        : Map<string, PackageManagerLine list> =
        let path = PackageManagerLine.StripDependencyManagerKey packageKey line

        let newLine =
            {
                Directive = directive
                LineStatus = LStatus.Unprocessed
                Line = path
                Range = m
            }

        let oldLines = MultiMap.find packageKey packageManagerLines
        let newLines = oldLines @ [ newLine ]
        packageManagerLines.Add(packageKey, newLines)

    static member RemoveUnprocessedLines
        (packageKey: string)
        (packageManagerLines: Map<string, PackageManagerLine list>)
        : Map<string, PackageManagerLine list> =
        let oldLines = MultiMap.find packageKey packageManagerLines

        let newLines =
            oldLines |> List.filter (fun line -> line.LineStatus = LStatus.Processed)

        packageManagerLines.Add(packageKey, newLines)

    static member SetLinesAsProcessed
        (packageKey: string)
        (packageManagerLines: Map<string, PackageManagerLine list>)
        : Map<string, PackageManagerLine list> =
        let oldLines = MultiMap.find packageKey packageManagerLines

        let newLines =
            oldLines
            |> List.map (fun line ->
                { line with
                    LineStatus = LStatus.Processed
                })

        packageManagerLines.Add(packageKey, newLines)

    static member StripDependencyManagerKey (packageKey: string) (line: string) : string =
        line.Substring(packageKey.Length + 1).Trim()

[<RequireQualifiedAccess>]
type MetadataAssemblyGeneration =
    | None
    | ReferenceOut of outputPath: string
    | ReferenceOnly

[<RequireQualifiedAccess>]
type ParallelReferenceResolution =
    | On
    | Off

[<NoEquality; NoComparison>]
type TcConfigBuilder =
    {
        mutable primaryAssembly: PrimaryAssembly
        mutable noFeedback: bool
        mutable stackReserveSize: int32 option
        mutable implicitIncludeDir: string (* normally "." *)
        mutable openDebugInformationForLaterStaticLinking: bool (* only for --standalone *)
        defaultFSharpBinariesDir: string
        mutable compilingFSharpCore: bool
        mutable useIncrementalBuilder: bool
        mutable includes: string list
        mutable implicitOpens: string list
        mutable useFsiAuxLib: bool
        mutable implicitlyReferenceDotNetAssemblies: bool
        mutable resolutionEnvironment: LegacyResolutionEnvironment
        mutable implicitlyResolveAssemblies: bool
        mutable indentationAwareSyntax: bool option
        mutable conditionalDefines: string list
        mutable loadedSources: (range * string * string) list
        mutable compilerToolPaths: string list
        mutable referencedDLLs: AssemblyReference list
        mutable packageManagerLines: Map<string, PackageManagerLine list>
        mutable projectReferences: IProjectReference list
        mutable knownUnresolvedReferences: UnresolvedAssemblyReference list
        reduceMemoryUsage: ReduceMemoryFlag
        mutable subsystemVersion: int * int
        mutable useHighEntropyVA: bool
        mutable inputCodePage: int option
        mutable clearResultsCache: bool
        mutable embedResources: string list
        mutable diagnosticsOptions: FSharpDiagnosticOptions
        mutable mlCompatibility: bool
        mutable checkOverflow: bool
        mutable showReferenceResolutions: bool
        mutable outputDir: string option
        mutable outputFile: string option
        mutable platform: ILPlatform option
        mutable prefer32Bit: bool
        mutable useSimpleResolution: bool
        mutable target: CompilerTarget
        mutable debuginfo: bool
        mutable testFlagEmitFeeFeeAs100001: bool
        mutable dumpDebugInfo: bool
        mutable debugSymbolFile: string option
        (* Backend configuration *)
        mutable typeCheckOnly: bool
        mutable parseOnly: bool
        mutable importAllReferencesOnly: bool
        mutable simulateException: string option
        mutable printAst: bool
        mutable tokenize: TokenizeOption
        mutable testInteractionParser: bool
        mutable reportNumDecls: bool
        mutable printSignature: bool
        mutable printSignatureFile: string
        mutable printAllSignatureFiles: bool
        mutable xmlDocOutputFile: string option
        mutable stats: bool
        mutable generateFilterBlocks: bool (* don't generate filter blocks due to bugs on Mono *)

        mutable signer: string option
        mutable container: string option

        mutable delaysign: bool
        mutable publicsign: bool
        mutable version: VersionFlag
        mutable metadataVersion: string option
        mutable standalone: bool
        mutable extraStaticLinkRoots: string list
        mutable compressMetadata: bool
        mutable noSignatureData: bool
        mutable onlyEssentialOptimizationData: bool
        mutable useOptimizationDataFile: bool
        mutable jitTracking: bool
        mutable portablePDB: bool
        mutable embeddedPDB: bool
        mutable embedAllSource: bool
        mutable embedSourceList: string list
        mutable sourceLink: string

        mutable internConstantStrings: bool
        mutable extraOptimizationIterations: int

        mutable win32icon: string
        mutable win32res: string
        mutable win32manifest: string
        mutable includewin32manifest: bool
        mutable linkResources: string list

        mutable legacyReferenceResolver: LegacyReferenceResolver

        mutable showFullPaths: bool
        mutable diagnosticStyle: DiagnosticStyle
        mutable utf8output: bool
        mutable flatErrors: bool

        mutable maxErrors: int
        mutable abortOnError: bool (* intended for fsi scripts that should exit on first error *)
        mutable baseAddress: int32 option
        mutable checksumAlgorithm: HashAlgorithm
#if DEBUG
        mutable showOptimizationData: bool
#endif
        mutable showTerms: bool (* show terms between passes? *)
        mutable writeTermsToFiles: bool (* show terms to files? *)
        mutable doDetuple: bool (* run detuple pass? *)
        mutable doTLR: bool (* run TLR pass? *)
        mutable doFinalSimplify: bool (* do final simplification pass *)
        mutable optsOn: bool (* optimizations are turned on *)
        mutable optSettings: Optimizer.OptimizationSettings
        mutable emitTailcalls: bool
        mutable deterministic: bool
        mutable concurrentBuild: bool
        mutable parallelCheckingWithSignatureFiles: bool
        mutable emitMetadataAssembly: MetadataAssemblyGeneration
        mutable preferredUiLang: string option
        mutable lcid: int option
        mutable productNameForBannerText: string
        /// show the MS (c) notice, e.g. with help or fsi?
        mutable showBanner: bool

        /// show times between passes?
        mutable showTimes: bool
        mutable showLoadedAssemblies: bool
        mutable continueAfterParseFailure: bool

#if !NO_TYPEPROVIDERS
        /// show messages about extension type resolution?
        mutable showExtensionTypeMessages: bool
#endif

        /// Pause between passes?
        mutable pause: bool

        /// Whenever possible, emit callvirt instead of call
        mutable alwaysCallVirt: bool

        /// If true, strip away data that would not be of use to end users, but is useful to us for debugging
        mutable noDebugAttributes: bool

        /// If true, do not emit ToString implementations for unions, records, structs, exceptions
        mutable useReflectionFreeCodeGen: bool

        /// If true, indicates all type checking and code generation is in the context of fsi.exe
        isInteractive: bool

        isInvalidationSupported: bool

        /// If true - every expression in quotations will be augmented with full debug info (fileName, location in file)
        mutable emitDebugInfoInQuotations: bool

        mutable exename: string option

        // If true - the compiler will copy FSharp.Core.dll along the produced binaries
        mutable copyFSharpCore: CopyFSharpCoreFlag

        /// When false FSI will lock referenced assemblies requiring process restart, false = disable Shadow Copy false, the default
        mutable shadowCopyReferences: bool

        mutable useSdkRefs: bool

        mutable fxResolver: FxResolver option

        mutable bufferWidth: int option

        // Is F# Interactive using multi-assembly emit?
        mutable fsiMultiAssemblyEmit: bool

        /// specify the error range for FxResolver
        rangeForErrors: range

        /// Override the SDK directory used by FxResolver, used for FCS only
        sdkDirOverride: string option

        /// A function to call to try to get an object that acts as a snapshot of the metadata section of a .NET binary,
        /// and from which we can read the metadata. Only used when metadataOnly=true.
        mutable tryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot

        mutable internalTestSpanStackReferring: bool

        mutable noConditionalErasure: bool

        mutable applyLineDirectives: bool

        mutable pathMap: PathMap

        mutable langVersion: LanguageVersion

        mutable xmlDocInfoLoader: IXmlDocumentationInfoLoader option

        mutable exiter: Exiter

        mutable parallelReferenceResolution: ParallelReferenceResolution
    }

    // Directories to start probing in
    // Algorithm:
    //  Search for native libraries using:
    //  1. Include directories
    //  2. compilerToolPath directories
    //  3. reference dll's
    //  4. The implicit include directory
    //
    // NOTE: it is important this is a delayed IEnumerable sequence. It is recomputed
    // each time a resolution happens and additional paths may be added as a result.
    member tcConfigB.GetNativeProbingRoots() =
        seq {
            yield! tcConfigB.includes
            yield! tcConfigB.compilerToolPaths
            yield! (tcConfigB.referencedDLLs |> Seq.map (fun ref -> Path.GetDirectoryName(ref.Text)))
            tcConfigB.implicitIncludeDir
        }
        |> Seq.distinct

    static member CreateNew
        (
            legacyReferenceResolver,
            defaultFSharpBinariesDir,
            reduceMemoryUsage,
            implicitIncludeDir,
            isInteractive,
            isInvalidationSupported,
            defaultCopyFSharpCore,
            tryGetMetadataSnapshot,
            sdkDirOverride,
            rangeForErrors
        ) =

        if (String.IsNullOrEmpty defaultFSharpBinariesDir) then
            failwith "Expected a valid defaultFSharpBinariesDir"

        // These are all default values, many can be overridden using the command line switch
        {
            primaryAssembly = PrimaryAssembly.Mscorlib
            indentationAwareSyntax = None
            noFeedback = false
            stackReserveSize = None
            conditionalDefines = []
            openDebugInformationForLaterStaticLinking = false
            compilingFSharpCore = false
            useIncrementalBuilder = false
            implicitOpens = []
            includes = []
            resolutionEnvironment = LegacyResolutionEnvironment.EditingOrCompilation false
            implicitlyReferenceDotNetAssemblies = true
            implicitlyResolveAssemblies = true
            compilerToolPaths = []
            referencedDLLs = []
            packageManagerLines = Map.empty
            projectReferences = []
            knownUnresolvedReferences = []
            loadedSources = []
            diagnosticsOptions = FSharpDiagnosticOptions.Default
            embedResources = []
            inputCodePage = None
            clearResultsCache = false
            subsystemVersion = 4, 0 // per spec for 357994
            useHighEntropyVA = false
            mlCompatibility = false
            checkOverflow = false
            showReferenceResolutions = false
            outputDir = None
            outputFile = None
            platform = None
            prefer32Bit = false
            useSimpleResolution = false
            target = CompilerTarget.ConsoleExe
            debuginfo = false
            testFlagEmitFeeFeeAs100001 = false
            dumpDebugInfo = false
            debugSymbolFile = None

            (* Backend configuration *)
            typeCheckOnly = false
            parseOnly = false
            importAllReferencesOnly = false
            simulateException = None
            printAst = false
            tokenize = TokenizeOption.AndCompile
            testInteractionParser = false
            reportNumDecls = false
            printSignature = false
            printSignatureFile = ""
            printAllSignatureFiles = false
            xmlDocOutputFile = None
            stats = false
            generateFilterBlocks = false (* don't generate filter blocks *)

            signer = None
            container = None
            maxErrors = 100
            abortOnError = false
            baseAddress = None
            checksumAlgorithm = HashAlgorithm.Sha256

            delaysign = false
            publicsign = false
            version = VersionNone
            metadataVersion = None
            standalone = false
            extraStaticLinkRoots = []
            compressMetadata = false
            noSignatureData = false
            onlyEssentialOptimizationData = false
            useOptimizationDataFile = false
            jitTracking = true
            portablePDB = true
            embeddedPDB = false
            embedAllSource = false
            embedSourceList = []
            sourceLink = ""
            internConstantStrings = true
            extraOptimizationIterations = 0

            win32icon = ""
            win32res = ""
            win32manifest = ""
            includewin32manifest = true
            linkResources = []
            showFullPaths = false
            diagnosticStyle = DiagnosticStyle.Default

            utf8output = false
            flatErrors = false

#if DEBUG
            showOptimizationData = false
#endif
            showTerms = false
            writeTermsToFiles = false

            doDetuple = false
            doTLR = false
            doFinalSimplify = false
            optsOn = false
            optSettings = Optimizer.OptimizationSettings.Defaults
            emitTailcalls = true
            deterministic = false
            concurrentBuild = true
            parallelCheckingWithSignatureFiles = false
            emitMetadataAssembly = MetadataAssemblyGeneration.None
            preferredUiLang = None
            lcid = None
            productNameForBannerText = FSharpProductName
            showBanner = true
            showTimes = false
            showLoadedAssemblies = false
            continueAfterParseFailure = false
#if !NO_TYPEPROVIDERS
            showExtensionTypeMessages = false
#endif
            pause = false
            alwaysCallVirt = true
            noDebugAttributes = false
            useReflectionFreeCodeGen = false
            emitDebugInfoInQuotations = false
            exename = None
            shadowCopyReferences = false
            useSdkRefs = true
            fxResolver = None
            bufferWidth = None
            fsiMultiAssemblyEmit = true
            internalTestSpanStackReferring = false
            noConditionalErasure = false
            pathMap = PathMap.empty
            applyLineDirectives = true
            langVersion = LanguageVersion.Default
            implicitIncludeDir = implicitIncludeDir
            defaultFSharpBinariesDir = defaultFSharpBinariesDir
            reduceMemoryUsage = reduceMemoryUsage
            legacyReferenceResolver = legacyReferenceResolver
            isInteractive = isInteractive
            isInvalidationSupported = isInvalidationSupported
            copyFSharpCore = defaultCopyFSharpCore
            tryGetMetadataSnapshot = tryGetMetadataSnapshot
            useFsiAuxLib = isInteractive
            rangeForErrors = rangeForErrors
            sdkDirOverride = sdkDirOverride
            xmlDocInfoLoader = None
            exiter = QuitProcessExiter
            parallelReferenceResolution = ParallelReferenceResolution.Off
        }

    member tcConfigB.FxResolver =
        // We compute the FxResolver on-demand.  It depends on some configuration parameters
        // which may be later adjusted.
        match tcConfigB.fxResolver with
        | None ->
            let useDotNetFramework = (tcConfigB.primaryAssembly = PrimaryAssembly.Mscorlib)

            let fxResolver =
                FxResolver(
                    useDotNetFramework,
                    tcConfigB.implicitIncludeDir,
                    rangeForErrors = tcConfigB.rangeForErrors,
                    useSdkRefs = tcConfigB.useSdkRefs,
                    isInteractive = tcConfigB.isInteractive,
                    sdkDirOverride = tcConfigB.sdkDirOverride
                )

            tcConfigB.fxResolver <- Some fxResolver
            fxResolver
        | Some fxResolver -> fxResolver

    member tcConfigB.SetPrimaryAssembly primaryAssembly =
        tcConfigB.primaryAssembly <- primaryAssembly
        tcConfigB.fxResolver <- None // this needs to be recreated when the primary assembly changes

    member tcConfigB.SetUseSdkRefs useSdkRefs =
        tcConfigB.useSdkRefs <- useSdkRefs
        tcConfigB.fxResolver <- None // this needs to be recreated when the primary assembly changes

    member tcConfigB.ResolveSourceFile(m, nm, pathLoadedFrom) =
        use _ = UseBuildPhase BuildPhase.Parameter

        let paths =
            seq {
                yield! tcConfigB.includes
                yield pathLoadedFrom
            }

        ResolveFileUsingPaths(paths, m, nm)

    /// Decide names of output file, pdb and assembly
    member tcConfigB.DecideNames sourceFiles =
        use _ = UseBuildPhase BuildPhase.Parameter

        if sourceFiles = [] then
            errorR (Error(FSComp.SR.buildNoInputsSpecified (), rangeCmdArgs))

        let ext () =
            match tcConfigB.target with
            | CompilerTarget.Dll -> ".dll"
            | CompilerTarget.Module -> ".netmodule"
            | CompilerTarget.ConsoleExe
            | CompilerTarget.WinExe -> ".exe"

        let implFiles =
            sourceFiles
            |> List.filter (fun fileName -> List.exists (FileSystemUtils.checkSuffix fileName) FSharpImplFileSuffixes)

        let outfile =
            match tcConfigB.outputFile, List.rev implFiles with
            | None, [] -> "out" + ext ()
            | None, h :: _ ->
                let basic = FileSystemUtils.fileNameOfPath h

                let modname =
                    try
                        FileSystemUtils.chopExtension basic
                    with _ ->
                        basic

                modname + (ext ())
            | Some f, _ -> f

        let assemblyName =
            let baseName = FileSystemUtils.fileNameOfPath outfile
            (FileSystemUtils.fileNameWithoutExtension baseName)

        let pdbfile =
            if tcConfigB.debuginfo then
                Some(
                    match tcConfigB.debugSymbolFile with
                    | None -> getDebugFileName outfile
                    | Some f -> f
                )
            elif (tcConfigB.debugSymbolFile <> None) && (not tcConfigB.debuginfo) then
                error (Error(FSComp.SR.buildPdbRequiresDebug (), rangeStartup))
            else
                None

        tcConfigB.outputFile <- Some outfile
        outfile, pdbfile, assemblyName

    member tcConfigB.TurnWarningOff(m, s: string) =
        use _ = UseBuildPhase BuildPhase.Parameter

        match GetWarningNumber(m, s) with
        | None -> ()
        | Some n ->
            // nowarn:62 turns on mlCompatibility, e.g. shows ML compat items in intellisense menus
            if n = 62 then
                tcConfigB.mlCompatibility <- true

            tcConfigB.diagnosticsOptions <-
                { tcConfigB.diagnosticsOptions with
                    WarnOff = ListSet.insert (=) n tcConfigB.diagnosticsOptions.WarnOff
                }

    member tcConfigB.TurnWarningOn(m, s: string) =
        use _ = UseBuildPhase BuildPhase.Parameter

        match GetWarningNumber(m, s) with
        | None -> ()
        | Some n ->
            // warnon 62 turns on mlCompatibility, e.g. shows ML compat items in intellisense menus
            if n = 62 then
                tcConfigB.mlCompatibility <- false

            tcConfigB.diagnosticsOptions <-
                { tcConfigB.diagnosticsOptions with
                    WarnOn = ListSet.insert (=) n tcConfigB.diagnosticsOptions.WarnOn
                }

    member tcConfigB.AddIncludePath(m, path, pathIncludedFrom) =
        let absolutePath = ComputeMakePathAbsolute pathIncludedFrom path

        let ok =
            let existsOpt =
                try
                    Some(FileSystem.DirectoryExistsShim absolutePath)
                with _ ->
                    warning (Error(FSComp.SR.buildInvalidSearchDirectory path, m))
                    None

            match existsOpt with
            | Some exists ->
                if not exists then
                    warning (Error(FSComp.SR.buildSearchDirectoryNotFound absolutePath, m))

                exists
            | None -> false

        if ok && not (List.contains absolutePath tcConfigB.includes) then
            tcConfigB.includes <- tcConfigB.includes ++ absolutePath

    member tcConfigB.AddLoadedSource(m, originalPath, pathLoadedFrom) =
        if FileSystem.IsInvalidPathShim originalPath then
            warning (Error(FSComp.SR.buildInvalidFilename originalPath, m))
        else
            let path =
                let paths =
                    seq {
                        yield! tcConfigB.includes
                        yield pathLoadedFrom
                    }

                match TryResolveFileUsingPaths(paths, m, originalPath) with
                | Some path -> path
                | None ->
                    // File doesn't exist in the paths. Assume it will be in the load-ed from directory.
                    ComputeMakePathAbsolute pathLoadedFrom originalPath

            if not (List.contains path (List.map (fun (_, _, path) -> path) tcConfigB.loadedSources)) then
                tcConfigB.loadedSources <- tcConfigB.loadedSources ++ (m, originalPath, path)

    member tcConfigB.AddEmbeddedSourceFile fileName =
        tcConfigB.embedSourceList <- tcConfigB.embedSourceList ++ fileName

    member tcConfigB.AddEmbeddedResource fileName =
        tcConfigB.embedResources <- tcConfigB.embedResources ++ fileName

    member tcConfigB.AddCompilerToolsByPath path =
        if not (tcConfigB.compilerToolPaths |> List.exists (fun text -> path = text)) then // NOTE: We keep same paths if range is different.
            let compilerToolPath =
                tcConfigB.compilerToolPaths
                |> List.tryPick (fun text -> if text = path then Some text else None)

            if compilerToolPath.IsNone then
                tcConfigB.compilerToolPaths <- tcConfigB.compilerToolPaths ++ path

    member tcConfigB.AddReferencedAssemblyByPath(m, path) =
        if FileSystem.IsInvalidPathShim path then
            warning (Error(FSComp.SR.buildInvalidAssemblyName (path), m))
        elif
            not (
                tcConfigB.referencedDLLs
                |> List.exists (fun ar2 -> equals m ar2.Range && path = ar2.Text)
            )
        then // NOTE: We keep same paths if range is different.
            let projectReference =
                tcConfigB.projectReferences
                |> List.tryPick (fun pr -> if pr.FileName = path then Some pr else None)

            tcConfigB.referencedDLLs <- tcConfigB.referencedDLLs ++ AssemblyReference(m, path, projectReference)

    member tcConfigB.AddDependencyManagerText(packageManager: IDependencyManagerProvider, lt, m, path: string) =
        tcConfigB.packageManagerLines <- PackageManagerLine.AddLineWithKey packageManager.Key lt path m tcConfigB.packageManagerLines

    member tcConfigB.AddReferenceDirective(dependencyProvider: DependencyProvider, m, path: string, directive) =
        let output = tcConfigB.outputDir |> Option.defaultValue ""

        let reportError =
            ResolvingErrorReport(fun errorType err msg ->
                let error = err, msg

                match errorType with
                | ErrorReportType.Warning -> warning (Error(error, m))
                | ErrorReportType.Error -> errorR (Error(error, m)))

        let dm =
            dependencyProvider.TryFindDependencyManagerInPath(tcConfigB.compilerToolPaths, output, reportError, path)

        match dm with
        // #r "Assembly"
        | NonNull path, Null -> tcConfigB.AddReferencedAssemblyByPath(m, path)

        | _, NonNull dependencyManager ->
            if tcConfigB.langVersion.SupportsFeature(LanguageFeature.PackageManagement) then
                tcConfigB.AddDependencyManagerText(dependencyManager, directive, m, path)
            else
                errorR (Error(FSComp.SR.packageManagementRequiresVFive (), m))

        | Null, Null when directive = Directive.Include -> errorR (Error(FSComp.SR.poundiNotSupportedByRegisteredDependencyManagers (), m))

        | Null, Null -> errorR (Error(FSComp.SR.buildInvalidHashrDirective (), m))

    member tcConfigB.RemoveReferencedAssemblyByPath(m, path) =
        tcConfigB.referencedDLLs <-
            tcConfigB.referencedDLLs
            |> List.filter (fun ar -> not (equals ar.Range m) || ar.Text <> path)

    member tcConfigB.AddPathMapping(oldPrefix, newPrefix) =
        tcConfigB.pathMap <- tcConfigB.pathMap |> PathMap.addMapping oldPrefix newPrefix

    static member SplitCommandLineResourceInfo(ri: string) =
        let p = ri.IndexOf ','

        if p <> -1 then
            let file = String.sub ri 0 p
            let rest = String.sub ri (p + 1) (String.length ri - p - 1)
            let p = rest.IndexOf ','

            if p <> -1 then
                let name = String.sub rest 0 p + ".resources"
                let pubpri = String.sub rest (p + 1) (rest.Length - p - 1)

                if pubpri = "public" then
                    file, name, ILResourceAccess.Public
                elif pubpri = "private" then
                    file, name, ILResourceAccess.Private
                else
                    error (Error(FSComp.SR.buildInvalidPrivacy pubpri, rangeStartup))
            else
                file, rest, ILResourceAccess.Public
        else
            ri, FileSystemUtils.fileNameOfPath ri, ILResourceAccess.Public

//----------------------------------------------------------------------------
// TcConfig
//--------------------------------------------------------------------------

/// This type is immutable and must be kept as such. Do not extract or mutate the underlying data except by cloning it.
[<Sealed>]
type TcConfig private (data: TcConfigBuilder, validate: bool) =

    // Validate the inputs - this helps ensure errors in options are shown in visual studio rather than only when built
    // However we only validate a minimal number of options at the moment
    do
        if validate then
            try
                data.version.GetVersionInfo(data.implicitIncludeDir) |> ignore
            with e ->
                errorR e

    // clone the input builder to ensure nobody messes with it.
    let data = { data with pause = data.pause }

    let computeKnownDllReference libraryName =
        let defaultCoreLibraryReference =
            AssemblyReference(range0, libraryName + ".dll", None)

        let nameOfDll (assemRef: AssemblyReference) =
            let fileName = ComputeMakePathAbsolute data.implicitIncludeDir assemRef.Text

            if FileSystem.FileExistsShim fileName then
                assemRef, Some fileName
            else
                // If the file doesn't exist, let reference resolution logic report the error later...
                defaultCoreLibraryReference,
                if equals assemRef.Range rangeStartup then
                    Some fileName
                else
                    None

        match
            data.referencedDLLs
            |> List.filter (fun assemblyReference -> assemblyReference.SimpleAssemblyNameIs libraryName)
        with
        | [] -> defaultCoreLibraryReference, None
        | [ r ]
        | r :: _ -> nameOfDll r

    // Look for an explicit reference to mscorlib/netstandard.dll or System.Runtime.dll and use that to compute clrRoot and targetFrameworkVersion
    let primaryAssemblyReference, primaryAssemblyExplicitFilenameOpt =
        computeKnownDllReference (data.primaryAssembly.Name)

    let fslibReference =
        // Look for explicit FSharp.Core reference otherwise use version that was referenced by compiler
        let dllReference, fileNameOpt = computeKnownDllReference getFSharpCoreLibraryName

        match fileNameOpt with
        | Some _ -> dllReference
        | None -> AssemblyReference(range0, getDefaultFSharpCoreLocation (), None)

    // clrRoot: the location of the primary assembly (mscorlib.dll or netstandard.dll or System.Runtime.dll)
    //
    // targetFrameworkVersionValue: Normally just HighestInstalledNetFrameworkVersion()
    //
    // Note, when mscorlib.dll has been given explicitly the actual value of
    // targetFrameworkVersion shouldn't matter since resolution has already happened.
    // In those cases where it does matter (e.g. --noframework is not being used or we are processing further
    // resolutions for a script) then it is correct to just use HighestInstalledNetFrameworkVersion().
    let clrRootValue, targetFrameworkVersionValue =
        match primaryAssemblyExplicitFilenameOpt with
        | Some primaryAssemblyFilename ->
            let fileName =
                ComputeMakePathAbsolute data.implicitIncludeDir primaryAssemblyFilename

            try
                let clrRoot = Some(Path.GetDirectoryName(FileSystem.GetFullPathShim fileName))
                clrRoot, data.legacyReferenceResolver.Impl.HighestInstalledNetFrameworkVersion()
            with e ->
                // We no longer expect the above to fail but leaving this just in case
                error (Error(FSComp.SR.buildErrorOpeningBinaryFile (fileName, e.Message), rangeStartup))
        | None -> None, data.legacyReferenceResolver.Impl.HighestInstalledNetFrameworkVersion()

    let makePathAbsolute path =
        ComputeMakePathAbsolute data.implicitIncludeDir path

    let targetFrameworkDirectories =
        try
            [
                // Check if we are given an explicit framework root - if so, use that
                match clrRootValue with
                | Some x ->
                    let clrRoot = makePathAbsolute x
                    yield clrRoot
                    let clrFacades = Path.Combine(clrRoot, "Facades")

                    if FileSystem.DirectoryExistsShim(clrFacades) then
                        yield clrFacades

                | None ->
                    // "there is no really good notion of runtime directory on .NETCore"
#if NETSTANDARD
                    let runtimeRoot = Path.GetDirectoryName(typeof<Object>.Assembly.Location)
#else
                    let runtimeRoot =
                        System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
#endif
                    let runtimeRootWithoutSlash = runtimeRoot.TrimEnd('/', '\\')
                    let runtimeRootFacades = Path.Combine(runtimeRootWithoutSlash, "Facades")
                    let runtimeRootWPF = Path.Combine(runtimeRootWithoutSlash, "WPF")

                    match data.resolutionEnvironment with
                    | LegacyResolutionEnvironment.CompilationAndEvaluation ->
                        // Default compilation-and-execution-time references on .NET Framework and Mono, e.g. for F# Interactive
                        //
                        // In the current way of doing things, F# Interactive refers to implementation assemblies.
                        yield runtimeRoot

                        if FileSystem.DirectoryExistsShim runtimeRootFacades then
                            yield runtimeRootFacades // System.Runtime.dll is in /usr/lib/mono/4.5/Facades

                        if FileSystem.DirectoryExistsShim runtimeRootWPF then
                            yield runtimeRootWPF // PresentationCore.dll is in C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF

                        match data.FxResolver.GetFrameworkRefsPackDirectory() with
                        | Some path when FileSystem.DirectoryExistsShim(path) -> yield path
                        | _ -> ()

                    | LegacyResolutionEnvironment.EditingOrCompilation _ ->
                        // Default compilation-time references on .NET Framework
                        //
                        // This is the normal case for "fsc.exe a.fs". We refer to the reference assemblies folder.
                        let frameworkRoot =
                            data.legacyReferenceResolver.Impl.DotNetFrameworkReferenceAssembliesRootDirectory

                        let frameworkRootVersion = Path.Combine(frameworkRoot, targetFrameworkVersionValue)
                        yield frameworkRootVersion
                        let facades = Path.Combine(frameworkRootVersion, "Facades")

                        if FileSystem.DirectoryExistsShim facades then
                            yield facades

                        match data.FxResolver.GetFrameworkRefsPackDirectory() with
                        | Some path when FileSystem.DirectoryExistsShim(path) -> yield path
                        | _ -> ()
            ]
        with e ->
            errorRecovery e range0
            []

    member _.bufferWidth = data.bufferWidth
    member _.fsiMultiAssemblyEmit = data.fsiMultiAssemblyEmit
    member _.FxResolver = data.FxResolver
    member _.primaryAssembly = data.primaryAssembly
    member _.noFeedback = data.noFeedback
    member _.stackReserveSize = data.stackReserveSize
    member _.implicitIncludeDir = data.implicitIncludeDir

    member _.openDebugInformationForLaterStaticLinking =
        data.openDebugInformationForLaterStaticLinking

    member _.fsharpBinariesDir = data.defaultFSharpBinariesDir
    member _.compilingFSharpCore = data.compilingFSharpCore
    member _.useIncrementalBuilder = data.useIncrementalBuilder
    member _.includes = data.includes
    member _.implicitOpens = data.implicitOpens
    member _.useFsiAuxLib = data.useFsiAuxLib
    member _.implicitlyReferenceDotNetAssemblies = data.implicitlyReferenceDotNetAssemblies
    member _.implicitlyResolveAssemblies = data.implicitlyResolveAssemblies
    member _.resolutionEnvironment = data.resolutionEnvironment
    member _.indentationAwareSyntax = data.indentationAwareSyntax
    member _.conditionalDefines = data.conditionalDefines
    member _.loadedSources = data.loadedSources
    member _.compilerToolPaths = data.compilerToolPaths
    member _.referencedDLLs = data.referencedDLLs
    member _.knownUnresolvedReferences = data.knownUnresolvedReferences
    member _.clrRoot = clrRootValue
    member _.reduceMemoryUsage = data.reduceMemoryUsage
    member _.subsystemVersion = data.subsystemVersion
    member _.useHighEntropyVA = data.useHighEntropyVA
    member _.inputCodePage = data.inputCodePage
    member _.clearResultsCache = data.clearResultsCache
    member _.embedResources = data.embedResources
    member _.diagnosticsOptions = data.diagnosticsOptions
    member _.mlCompatibility = data.mlCompatibility
    member _.checkOverflow = data.checkOverflow
    member _.showReferenceResolutions = data.showReferenceResolutions
    member _.outputDir = data.outputDir
    member _.outputFile = data.outputFile
    member _.platform = data.platform
    member _.prefer32Bit = data.prefer32Bit
    member _.useSimpleResolution = data.useSimpleResolution
    member _.target = data.target
    member _.debuginfo = data.debuginfo
    member _.testFlagEmitFeeFeeAs100001 = data.testFlagEmitFeeFeeAs100001
    member _.dumpDebugInfo = data.dumpDebugInfo
    member _.debugSymbolFile = data.debugSymbolFile
    member _.typeCheckOnly = data.typeCheckOnly
    member _.parseOnly = data.parseOnly
    member _.importAllReferencesOnly = data.importAllReferencesOnly
    member _.simulateException = data.simulateException
    member _.printAst = data.printAst
    member _.targetFrameworkVersion = targetFrameworkVersionValue
    member _.tokenize = data.tokenize
    member _.testInteractionParser = data.testInteractionParser
    member _.reportNumDecls = data.reportNumDecls
    member _.printSignature = data.printSignature
    member _.printSignatureFile = data.printSignatureFile
    member _.printAllSignatureFiles = data.printAllSignatureFiles
    member _.xmlDocOutputFile = data.xmlDocOutputFile
    member _.stats = data.stats
    member _.generateFilterBlocks = data.generateFilterBlocks
    member _.signer = data.signer
    member _.container = data.container
    member _.delaysign = data.delaysign
    member _.publicsign = data.publicsign
    member _.version = data.version
    member _.metadataVersion = data.metadataVersion
    member _.standalone = data.standalone
    member _.extraStaticLinkRoots = data.extraStaticLinkRoots
    member _.compressMetadata = data.compressMetadata
    member _.noSignatureData = data.noSignatureData
    member _.onlyEssentialOptimizationData = data.onlyEssentialOptimizationData
    member _.useOptimizationDataFile = data.useOptimizationDataFile
    member _.jitTracking = data.jitTracking
    member _.portablePDB = data.portablePDB
    member _.embeddedPDB = data.embeddedPDB
    member _.embedAllSource = data.embedAllSource
    member _.embedSourceList = data.embedSourceList
    member _.sourceLink = data.sourceLink
    member _.packageManagerLines = data.packageManagerLines
    member _.internConstantStrings = data.internConstantStrings
    member _.extraOptimizationIterations = data.extraOptimizationIterations
    member _.win32icon = data.win32icon
    member _.win32res = data.win32res
    member _.win32manifest = data.win32manifest
    member _.includewin32manifest = data.includewin32manifest
    member _.linkResources = data.linkResources
    member _.showFullPaths = data.showFullPaths
    member _.diagnosticStyle = data.diagnosticStyle
    member _.utf8output = data.utf8output
    member _.flatErrors = data.flatErrors
    member _.maxErrors = data.maxErrors
    member _.baseAddress = data.baseAddress
    member _.checksumAlgorithm = data.checksumAlgorithm
#if DEBUG
    member _.showOptimizationData = data.showOptimizationData
#endif
    member _.showTerms = data.showTerms
    member _.writeTermsToFiles = data.writeTermsToFiles
    member _.doDetuple = data.doDetuple
    member _.doTLR = data.doTLR
    member _.doFinalSimplify = data.doFinalSimplify
    member _.optSettings = data.optSettings
    member _.emitTailcalls = data.emitTailcalls
    member _.deterministic = data.deterministic
    member _.concurrentBuild = data.concurrentBuild
    member _.parallelCheckingWithSignatureFiles = data.parallelCheckingWithSignatureFiles
    member _.emitMetadataAssembly = data.emitMetadataAssembly
    member _.pathMap = data.pathMap
    member _.langVersion = data.langVersion
    member _.preferredUiLang = data.preferredUiLang
    member _.lcid = data.lcid
    member _.optsOn = data.optsOn
    member _.productNameForBannerText = data.productNameForBannerText
    member _.showBanner = data.showBanner
    member _.showTimes = data.showTimes
    member _.showLoadedAssemblies = data.showLoadedAssemblies
    member _.continueAfterParseFailure = data.continueAfterParseFailure
#if !NO_TYPEPROVIDERS
    member _.showExtensionTypeMessages = data.showExtensionTypeMessages
#endif
    member _.pause = data.pause
    member _.alwaysCallVirt = data.alwaysCallVirt
    member _.noDebugAttributes = data.noDebugAttributes
    member _.useReflectionFreeCodeGen = data.useReflectionFreeCodeGen
    member _.isInteractive = data.isInteractive
    member _.isInvalidationSupported = data.isInvalidationSupported
    member _.emitDebugInfoInQuotations = data.emitDebugInfoInQuotations
    member _.copyFSharpCore = data.copyFSharpCore
    member _.shadowCopyReferences = data.shadowCopyReferences
    member _.useSdkRefs = data.useSdkRefs
    member _.sdkDirOverride = data.sdkDirOverride
    member _.tryGetMetadataSnapshot = data.tryGetMetadataSnapshot
    member _.internalTestSpanStackReferring = data.internalTestSpanStackReferring
    member _.noConditionalErasure = data.noConditionalErasure
    member _.applyLineDirectives = data.applyLineDirectives
    member _.xmlDocInfoLoader = data.xmlDocInfoLoader
    member _.exiter = data.exiter
    member _.parallelReferenceResolution = data.parallelReferenceResolution

    static member Create(builder, validate) =
        use _ = UseBuildPhase BuildPhase.Parameter
        TcConfig(builder, validate)

    member _.legacyReferenceResolver = data.legacyReferenceResolver

    member _.CloneToBuilder() =
        { data with
            conditionalDefines = data.conditionalDefines
        }

    member tcConfig.ComputeCanContainEntryPoint(sourceFiles: string list) =
        let n = sourceFiles.Length in (sourceFiles |> List.mapi (fun i _ -> (i = n - 1)), tcConfig.target.IsExe)

    // This call can fail if no CLR is found (this is the path to mscorlib)
    member _.GetTargetFrameworkDirectories() = targetFrameworkDirectories

    member tcConfig.ComputeIndentationAwareSyntaxInitialStatus fileName =
        use _unwindBuildPhase = UseBuildPhase BuildPhase.Parameter

        let indentationAwareSyntaxOnByDefault =
            List.exists (FileSystemUtils.checkSuffix fileName) FSharpIndentationAwareSyntaxFileSuffixes

        if indentationAwareSyntaxOnByDefault then
            (tcConfig.indentationAwareSyntax <> Some false)
        else
            (tcConfig.indentationAwareSyntax = Some true)

    member tcConfig.GetAvailableLoadedSources() =
        use _unwindBuildPhase = UseBuildPhase BuildPhase.Parameter

        let resolveLoadedSource (m, originalPath, path) =
            try
                if not (FileSystem.FileExistsShim(path)) then
                    let secondTrial =
                        tcConfig.includes
                        |> List.tryPick (fun root ->
                            let path = ComputeMakePathAbsolute root originalPath
                            if FileSystem.FileExistsShim(path) then Some path else None)

                    match secondTrial with
                    | Some path -> Some(m, path)
                    | None ->
                        error (LoadedSourceNotFoundIgnoring(path, m))
                        None
                else
                    Some(m, path)
            with e ->
                errorRecovery e m
                None

        tcConfig.loadedSources |> List.choose resolveLoadedSource |> List.distinct

    // This is not the complete set of search paths, it is just the set
    // that is special to F# (as compared to MSBuild resolution)
    member tcConfig.GetSearchPathsForLibraryFiles() =
        [
            yield! tcConfig.GetTargetFrameworkDirectories()
            yield! List.map tcConfig.MakePathAbsolute tcConfig.includes
            tcConfig.implicitIncludeDir
            tcConfig.fsharpBinariesDir
        ]

    member _.MakePathAbsolute path = makePathAbsolute path

    member _.ResolveSourceFile(m, fileName, pathLoadedFrom) =
        data.ResolveSourceFile(m, fileName, pathLoadedFrom)

    member _.PrimaryAssemblyDllReference() = primaryAssemblyReference

    member _.CoreLibraryDllReference() = fslibReference

    member _.GetNativeProbingRoots() = data.GetNativeProbingRoots()

    /// A closed set of assemblies where, for any subset S:
    ///    - the TcImports object built for S (and thus the F# Compiler CCUs for the assemblies in S)
    ///       is a resource that can be shared between any two IncrementalBuild objects that reference
    ///       precisely S
    ///
    /// Determined by looking at the set of assemblies referenced by f# .
    ///
    /// Returning true may mean that the file is locked and/or placed into the
    /// 'framework' reference set that is potentially shared across multiple compilations.
    member tcConfig.IsSystemAssembly(fileName: string) =
        try
            FileSystem.FileExistsShim fileName
            && ((tcConfig.GetTargetFrameworkDirectories()
                 |> List.exists (fun clrRoot -> clrRoot = Path.GetDirectoryName fileName))
                || (tcConfig
                       .FxResolver
                       .GetSystemAssemblies()
                       .Contains(FileSystemUtils.fileNameWithoutExtension fileName))
                || tcConfig.FxResolver.IsInReferenceAssemblyPackDirectory fileName)
        with _ ->
            false

    member tcConfig.GenerateSignatureData =
        not tcConfig.standalone && not tcConfig.noSignatureData

    member tcConfig.GenerateOptimizationData = tcConfig.GenerateSignatureData

    member tcConfig.assumeDotNetFramework =
        tcConfig.primaryAssembly = PrimaryAssembly.Mscorlib

/// Represents a computation to return a TcConfig. Normally this is just a constant immutable TcConfig,
/// but for F# Interactive it may be based on an underlying mutable TcConfigBuilder.
type TcConfigProvider =
    | TcConfigProvider of (CompilationThreadToken -> TcConfig)

    member x.Get ctok =
        (let (TcConfigProvider f) = x in f ctok)

    /// Get a TcConfigProvider which will return only the exact TcConfig.
    static member Constant tcConfig = TcConfigProvider(fun _ctok -> tcConfig)

    /// Get a TcConfigProvider which will continue to respect changes in the underlying
    /// TcConfigBuilder rather than delivering snapshots.
    static member BasedOnMutableBuilder tcConfigB =
        TcConfigProvider(fun _ctok -> TcConfig.Create(tcConfigB, validate = false))

let GetFSharpCoreLibraryName () = getFSharpCoreLibraryName
