// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Coordinating compiler operations - configuration, loading initial context, reporting errors etc.
module internal FSharp.Compiler.CompileOps

open System
open System.Text
open System.Collections.Generic
open Internal.Utilities
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.Range
open FSharp.Compiler.Ast
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Tast
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open Microsoft.FSharp.Core.CompilerServices
#if !NO_EXTENSIONTYPING
open FSharp.Compiler.ExtensionTyping
#endif


#if DEBUG

module internal CompilerService =
    val showAssertForUnexpectedException: bool ref
#endif

//----------------------------------------------------------------------------
// File names and known file suffixes
//--------------------------------------------------------------------------

/// Signature file suffixes
val FSharpSigFileSuffixes: string list

/// Implementation file suffixes
val FSharpImplFileSuffixes: string list

/// Script file suffixes
val FSharpScriptFileSuffixes: string list

val IsScript: string -> bool

/// File suffixes where #light is the default
val FSharpLightSyntaxFileSuffixes: string list


/// Get the name used for FSharp.Core
val GetFSharpCoreLibraryName: unit -> string

//----------------------------------------------------------------------------
// Parsing inputs
//--------------------------------------------------------------------------
  
val ComputeQualifiedNameOfFileFromUniquePath: range * string list -> Ast.QualifiedNameOfFile

val PrependPathToInput: Ast.Ident list -> Ast.ParsedInput -> Ast.ParsedInput

/// State used to de-deuplicate module names along a list of file names
type ModuleNamesDict = Map<string,Map<string,QualifiedNameOfFile>>

/// Checks if a ParsedInput is using a module name that was already given and deduplicates the name if needed.
val DeduplicateParsedInputModuleName: ModuleNamesDict -> Ast.ParsedInput -> Ast.ParsedInput * ModuleNamesDict

/// Parse a single input (A signature file or implementation file)
val ParseInput: (UnicodeLexing.Lexbuf -> Parser.token) * ErrorLogger * UnicodeLexing.Lexbuf * string option * string * isLastCompiland:(bool * bool) -> Ast.ParsedInput

//----------------------------------------------------------------------------
// Error and warnings
//--------------------------------------------------------------------------

/// Get the location associated with an error
val GetRangeOfDiagnostic: PhasedDiagnostic -> range option

/// Get the number associated with an error
val GetDiagnosticNumber: PhasedDiagnostic -> int

/// Split errors into a "main" error and a set of associated errors
val SplitRelatedDiagnostics: PhasedDiagnostic -> PhasedDiagnostic * PhasedDiagnostic list

/// Output an error to a buffer
val OutputPhasedDiagnostic: StringBuilder -> PhasedDiagnostic -> flattenErrors: bool -> suggestNames: bool -> unit

/// Output an error or warning to a buffer
val OutputDiagnostic: implicitIncludeDir:string * showFullPaths: bool * flattenErrors: bool * errorStyle: ErrorStyle *  isError:bool -> StringBuilder -> PhasedDiagnostic -> unit

/// Output extra context information for an error or warning to a buffer
val OutputDiagnosticContext: prefix:string -> fileLineFunction:(string -> int -> string) -> StringBuilder -> PhasedDiagnostic -> unit

/// Part of LegacyHostedCompilerForTesting
[<RequireQualifiedAccess>]
type DiagnosticLocation =
    { Range: range
      File: string
      TextRepresentation: string
      IsEmpty: bool }

/// Part of LegacyHostedCompilerForTesting
[<RequireQualifiedAccess>]
type DiagnosticCanonicalInformation = 
    { ErrorNumber: int
      Subcategory: string
      TextRepresentation: string }

/// Part of LegacyHostedCompilerForTesting
[<RequireQualifiedAccess>]
type DiagnosticDetailedInfo = 
    { Location: DiagnosticLocation option
      Canonical: DiagnosticCanonicalInformation
      Message: string }

/// Part of LegacyHostedCompilerForTesting
[<RequireQualifiedAccess>]
type Diagnostic = 
    | Short of bool * string
    | Long of bool * DiagnosticDetailedInfo

/// Part of LegacyHostedCompilerForTesting
val CollectDiagnostic: implicitIncludeDir:string * showFullPaths: bool * flattenErrors: bool * errorStyle: ErrorStyle *  warning:bool * PhasedDiagnostic * suggestNames: bool -> seq<Diagnostic>

//----------------------------------------------------------------------------
// Resolve assembly references 
//--------------------------------------------------------------------------

exception AssemblyNotResolved of (*originalName*) string * range
exception FileNameNotResolved of (*filename*) string * (*description of searched locations*) string * range
exception DeprecatedCommandLineOptionFull of string * range
exception DeprecatedCommandLineOptionForHtmlDoc of string * range
exception DeprecatedCommandLineOptionSuggestAlternative of string * string * range
exception DeprecatedCommandLineOptionNoDescription of string * range
exception InternalCommandLineOption of string * range
exception HashLoadedSourceHasIssues of (*warnings*) exn list * (*errors*) exn list * range
exception HashLoadedScriptConsideredSource of range  

//----------------------------------------------------------------------------

/// Represents a reference to an F# assembly. May be backed by a real assembly on disk (read by Abstract IL), or a cross-project
/// reference in FSharp.Compiler.Service.
type IRawFSharpAssemblyData = 
    ///  The raw list AutoOpenAttribute attributes in the assembly
    abstract GetAutoOpenAttributes: ILGlobals -> string list
    ///  The raw list InternalsVisibleToAttribute attributes in the assembly
    abstract GetInternalsVisibleToAttributes: ILGlobals  -> string list
    ///  The raw IL module definition in the assembly, if any. This is not present for cross-project references
    /// in the language service
    abstract TryGetILModuleDef: unit -> ILModuleDef option
    abstract HasAnyFSharpSignatureDataAttribute: bool
    abstract HasMatchingFSharpSignatureDataAttribute: ILGlobals -> bool
    ///  The raw F# signature data in the assembly, if any
    abstract GetRawFSharpSignatureData: range * ilShortAssemName: string * fileName: string -> (string * (unit -> byte[])) list
    ///  The raw F# optimization data in the assembly, if any
    abstract GetRawFSharpOptimizationData: range * ilShortAssemName: string * fileName: string -> (string * (unit -> byte[])) list
    ///  The table of type forwarders in the assembly
    abstract GetRawTypeForwarders: unit -> ILExportedTypesAndForwarders
    /// The identity of the module
    abstract ILScopeRef: ILScopeRef
    abstract ILAssemblyRefs: ILAssemblyRef list
    abstract ShortAssemblyName: string

type TimeStampCache = 
    new: defaultTimeStamp: DateTime -> TimeStampCache
    member GetFileTimeStamp: string -> DateTime
    member GetProjectReferenceTimeStamp: IProjectReference * CompilationThreadToken -> DateTime

and IProjectReference = 

    /// The name of the assembly file generated by the project
    abstract FileName: string 

    /// Evaluate raw contents of the assembly file generated by the project
    abstract EvaluateRawContents: CompilationThreadToken -> Cancellable<IRawFSharpAssemblyData option>

    /// Get the logical timestamp that would be the timestamp of the assembly file generated by the project.
    ///
    /// For project references this is maximum of the timestamps of all dependent files.
    /// The project is not actually built, nor are any assemblies read, but the timestamps for each dependent file 
    /// are read via the FileSystem.  If the files don't exist, then a default timestamp is used.
    ///
    /// The operation returns None only if it is not possible to create an IncrementalBuilder for the project at all, e.g. if there
    /// are fatal errors in the options for the project.
    abstract TryGetLogicalTimeStamp: TimeStampCache * CompilationThreadToken -> System.DateTime option

type AssemblyReference = 
    | AssemblyReference of range * string  * IProjectReference option
    member Range: range
    member Text: string
    member ProjectReference: IProjectReference option

type AssemblyResolution = 
      {/// The original reference to the assembly.
       originalReference: AssemblyReference
       /// Path to the resolvedFile
       resolvedPath: string    
       /// Create the tooltip text for the assembly reference
       prepareToolTip: unit -> string
       /// Whether or not this is an installed system assembly (for example, System.dll)
       sysdir: bool
       // Lazily populated ilAssemblyRef for this reference. 
       ilAssemblyRef: ILAssemblyRef option ref  }

type UnresolvedAssemblyReference = UnresolvedAssemblyReference of string * AssemblyReference list

#if !NO_EXTENSIONTYPING
type ResolvedExtensionReference = ResolvedExtensionReference of string * AssemblyReference list * Tainted<ITypeProvider> list
#endif

[<RequireQualifiedAccess>]
type CompilerTarget = 
    | WinExe 
    | ConsoleExe 
    | Dll 
    | Module
    member IsExe: bool
    
[<RequireQualifiedAccess>]
type ResolveAssemblyReferenceMode = 
    | Speculative 
    | ReportErrors

[<RequireQualifiedAccess>]
type CopyFSharpCoreFlag = Yes | No

//----------------------------------------------------------------------------
// TcConfig
//--------------------------------------------------------------------------

/// Represents the file or string used for the --version flag
type VersionFlag = 
    | VersionString of string
    | VersionFile of string
    | VersionNone
    member GetVersionInfo: implicitIncludeDir:string -> ILVersionInfo
    member GetVersionString: implicitIncludeDir:string -> string

[<NoEquality; NoComparison>]
type TcConfigBuilder =
    { mutable primaryAssembly: PrimaryAssembly
      mutable autoResolveOpenDirectivesToDlls: bool
      mutable noFeedback: bool
      mutable stackReserveSize: int32 option
      mutable implicitIncludeDir: string
      mutable openDebugInformationForLaterStaticLinking: bool
      defaultFSharpBinariesDir: string
      mutable compilingFslib: bool
      mutable compilingFslib20: string option
      mutable compilingFslib40: bool
      mutable compilingFslibNoBigInt: bool
      mutable useIncrementalBuilder: bool
      mutable includes: string list
      mutable implicitOpens: string list
      mutable useFsiAuxLib: bool
      mutable framework: bool
      mutable resolutionEnvironment: ReferenceResolver.ResolutionEnvironment
      mutable implicitlyResolveAssemblies: bool
      /// Set if the user has explicitly turned indentation-aware syntax on/off
      mutable light: bool option
      mutable conditionalCompilationDefines: string list
      /// Sources added into the build with #load
      mutable loadedSources: (range * string) list
      
      mutable referencedDLLs: AssemblyReference  list
      mutable projectReferences: IProjectReference list
      mutable knownUnresolvedReferences: UnresolvedAssemblyReference list
      reduceMemoryUsage: ReduceMemoryFlag
      mutable subsystemVersion: int * int
      mutable useHighEntropyVA: bool
      mutable inputCodePage: int option
      mutable embedResources: string list
      mutable errorSeverityOptions: FSharpErrorSeverityOptions
      mutable mlCompatibility:bool
      mutable checkOverflow:bool
      mutable showReferenceResolutions:bool
      mutable outputFile: string option
      mutable platform: ILPlatform option
      mutable prefer32Bit: bool
      mutable useSimpleResolution: bool
      mutable target: CompilerTarget
      mutable debuginfo: bool
      mutable testFlagEmitFeeFeeAs100001: bool
      mutable dumpDebugInfo: bool
      mutable debugSymbolFile: string option
      mutable typeCheckOnly: bool
      mutable parseOnly: bool
      mutable importAllReferencesOnly: bool
      mutable simulateException: string option
      mutable printAst: bool
      mutable tokenizeOnly: bool
      mutable testInteractionParser: bool
      mutable reportNumDecls: bool
      mutable printSignature: bool
      mutable printSignatureFile: string
      mutable xmlDocOutputFile: string option
      mutable stats: bool
      mutable generateFilterBlocks: bool 
      mutable signer: string option
      mutable container: string option
      mutable delaysign: bool
      mutable publicsign: bool
      mutable version: VersionFlag 
      mutable metadataVersion: string option
      mutable standalone: bool
      mutable extraStaticLinkRoots: string list 
      mutable noSignatureData: bool
      mutable onlyEssentialOptimizationData: bool
      mutable useOptimizationDataFile: bool
      mutable jitTracking: bool
      mutable portablePDB: bool
      mutable embeddedPDB: bool
      mutable embedAllSource: bool
      mutable embedSourceList: string list
      mutable sourceLink: string
      mutable ignoreSymbolStoreSequencePoints: bool
      mutable internConstantStrings: bool
      mutable extraOptimizationIterations: int
      mutable win32res: string 
      mutable win32manifest: string
      mutable includewin32manifest: bool
      mutable linkResources: string list
      mutable legacyReferenceResolver: ReferenceResolver.Resolver 
      mutable showFullPaths: bool
      mutable errorStyle: ErrorStyle
      mutable utf8output: bool
      mutable flatErrors: bool
      mutable maxErrors: int
      mutable abortOnError: bool
      mutable baseAddress: int32 option
 #if DEBUG
      mutable showOptimizationData: bool
#endif
      mutable showTerms    : bool 
      mutable writeTermsToFiles: bool 
      mutable doDetuple    : bool 
      mutable doTLR        : bool 
      mutable doFinalSimplify: bool
      mutable optsOn       : bool 
      mutable optSettings  : Optimizer.OptimizationSettings 
      mutable emitTailcalls: bool
      mutable deterministic: bool
      mutable preferredUiLang: string option
      mutable lcid        : int option
      mutable productNameForBannerText: string
      mutable showBanner : bool
      mutable showTimes: bool
      mutable showLoadedAssemblies: bool
      mutable continueAfterParseFailure: bool
#if !NO_EXTENSIONTYPING
      mutable showExtensionTypeMessages: bool
#endif
      mutable pause: bool 
      mutable alwaysCallVirt: bool
      mutable noDebugData: bool

      /// If true, indicates all type checking and code generation is in the context of fsi.exe
      isInteractive: bool 
      isInvalidationSupported: bool 
      mutable emitDebugInfoInQuotations: bool
      mutable exename: string option 
      mutable copyFSharpCore: CopyFSharpCoreFlag
      mutable shadowCopyReferences: bool
      mutable useSdkRefs: bool

      /// A function to call to try to get an object that acts as a snapshot of the metadata section of a .NET binary,
      /// and from which we can read the metadata. Only used when metadataOnly=true.
      mutable tryGetMetadataSnapshot : ILReaderTryGetMetadataSnapshot

      /// if true - 'let mutable x = Span.Empty', the value 'x' is a stack referring span. Used for internal testing purposes only until we get true stack spans.
      mutable internalTestSpanStackReferring : bool

      /// Prevent erasure of conditional attributes and methods so tooling is able analyse them.
      mutable noConditionalErasure: bool

      mutable pathMap : PathMap
    }

    static member Initial: TcConfigBuilder

    static member CreateNew: 
        legacyReferenceResolver: ReferenceResolver.Resolver *
        defaultFSharpBinariesDir: string * 
        reduceMemoryUsage: ReduceMemoryFlag * 
        implicitIncludeDir: string * 
        isInteractive: bool * 
        isInvalidationSupported: bool *
        defaultCopyFSharpCore: CopyFSharpCoreFlag *
        tryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot 
          -> TcConfigBuilder

    member DecideNames: string list -> outfile: string * pdbfile: string option * assemblyName: string 
    member TurnWarningOff: range * string -> unit
    member TurnWarningOn: range * string -> unit
    member AddIncludePath: range * string * string -> unit
    member AddReferencedAssemblyByPath: range * string -> unit
    member RemoveReferencedAssemblyByPath: range * string -> unit
    member AddEmbeddedSourceFile: string -> unit
    member AddEmbeddedResource: string -> unit
    member AddPathMapping: oldPrefix: string * newPrefix: string -> unit
    
    static member SplitCommandLineResourceInfo: string -> string * string * ILResourceAccess

[<Sealed>]
// Immutable TcConfig
type TcConfig =
    member primaryAssembly: PrimaryAssembly
    member autoResolveOpenDirectivesToDlls: bool
    member noFeedback: bool
    member stackReserveSize: int32 option
    member implicitIncludeDir: string
    member openDebugInformationForLaterStaticLinking: bool
    member fsharpBinariesDir: string
    member compilingFslib: bool
    member compilingFslib20: string option
    member compilingFslib40: bool
    member compilingFslibNoBigInt: bool
    member useIncrementalBuilder: bool
    member includes: string list
    member implicitOpens: string list
    member useFsiAuxLib: bool
    member framework: bool
    member implicitlyResolveAssemblies: bool
    /// Set if the user has explicitly turned indentation-aware syntax on/off
    member light: bool option
    member conditionalCompilationDefines: string list
    member subsystemVersion: int * int
    member useHighEntropyVA: bool
    member referencedDLLs: AssemblyReference list
    member reduceMemoryUsage: ReduceMemoryFlag
    member inputCodePage: int option
    member embedResources: string list
    member errorSeverityOptions: FSharpErrorSeverityOptions
    member mlCompatibility:bool
    member checkOverflow:bool
    member showReferenceResolutions:bool
    member outputFile: string option
    member platform: ILPlatform option
    member prefer32Bit: bool
    member useSimpleResolution: bool
    member target: CompilerTarget
    member debuginfo: bool
    member testFlagEmitFeeFeeAs100001: bool
    member dumpDebugInfo: bool
    member debugSymbolFile: string option
    member typeCheckOnly: bool
    member parseOnly: bool
    member importAllReferencesOnly: bool
    member simulateException: string option
    member printAst: bool
    member tokenizeOnly: bool
    member testInteractionParser: bool
    member reportNumDecls: bool
    member printSignature: bool
    member printSignatureFile: string
    member xmlDocOutputFile: string option
    member stats: bool
    member generateFilterBlocks: bool 
    member signer: string option
    member container: string option
    member delaysign: bool
    member publicsign: bool
    member version: VersionFlag 
    member metadataVersion: string option
    member standalone: bool
    member extraStaticLinkRoots: string list 
    member noSignatureData: bool
    member onlyEssentialOptimizationData: bool
    member useOptimizationDataFile: bool
    member jitTracking: bool
    member portablePDB: bool
    member embeddedPDB: bool
    member embedAllSource: bool
    member embedSourceList: string list
    member sourceLink: string
    member ignoreSymbolStoreSequencePoints: bool
    member internConstantStrings: bool
    member extraOptimizationIterations: int
    member win32res: string 
    member win32manifest: string
    member includewin32manifest: bool
    member linkResources: string list
    member showFullPaths: bool
    member errorStyle: ErrorStyle
    member utf8output: bool
    member flatErrors: bool

    member maxErrors: int
    member baseAddress: int32 option
#if DEBUG
    member showOptimizationData: bool
#endif
    member showTerms    : bool 
    member writeTermsToFiles: bool 
    member doDetuple    : bool 
    member doTLR        : bool 
    member doFinalSimplify: bool
    member optSettings  : Optimizer.OptimizationSettings 
    member emitTailcalls: bool
    member deterministic: bool
    member pathMap: PathMap
    member preferredUiLang: string option
    member optsOn       : bool 
    member productNameForBannerText: string
    member showBanner : bool
    member showTimes: bool
    member showLoadedAssemblies: bool
    member continueAfterParseFailure: bool
#if !NO_EXTENSIONTYPING
    member showExtensionTypeMessages: bool
#endif
    member pause: bool 
    member alwaysCallVirt: bool
    member noDebugData: bool

    /// If true, indicates all type checking and code generation is in the context of fsi.exe
    member isInteractive: bool
    member isInvalidationSupported: bool 


    member ComputeLightSyntaxInitialStatus: string -> bool
    member GetTargetFrameworkDirectories: unit -> string list
    
    /// Get the loaded sources that exist and issue a warning for the ones that don't
    member GetAvailableLoadedSources: unit -> (range*string) list
    
    member ComputeCanContainEntryPoint: sourceFiles:string list -> bool list *bool 

    /// File system query based on TcConfig settings
    member ResolveSourceFile: range * filename: string * pathLoadedFrom: string -> string

    /// File system query based on TcConfig settings
    member MakePathAbsolute: string -> string

    member copyFSharpCore: CopyFSharpCoreFlag
    member shadowCopyReferences: bool
    member useSdkRefs: bool

    static member Create: TcConfigBuilder * validate: bool -> TcConfig

/// Represents a computation to return a TcConfig. Normally this is just a constant immutable TcConfig,
/// but for F# Interactive it may be based on an underlying mutable TcConfigBuilder.
[<Sealed>]
type TcConfigProvider = 

    member Get: CompilationThreadToken -> TcConfig

    /// Get a TcConfigProvider which will return only the exact TcConfig.
    static member Constant: TcConfig -> TcConfigProvider

    /// Get a TcConfigProvider which will continue to respect changes in the underlying
    /// TcConfigBuilder rather than delivering snapshots.
    static member BasedOnMutableBuilder: TcConfigBuilder -> TcConfigProvider

//----------------------------------------------------------------------------
// Tables of referenced DLLs 
//--------------------------------------------------------------------------

/// Represents a resolved imported binary
[<RequireQualifiedAccess>]
type ImportedBinary = 
    { FileName: string
      RawMetadata: IRawFSharpAssemblyData
#if !NO_EXTENSIONTYPING
      ProviderGeneratedAssembly: System.Reflection.Assembly option
      IsProviderGenerated: bool
      ProviderGeneratedStaticLinkMap: ProvidedAssemblyStaticLinkingMap  option
#endif
      ILAssemblyRefs: ILAssemblyRef list
      ILScopeRef: ILScopeRef}

/// Represents a resolved imported assembly
[<RequireQualifiedAccess>]
type ImportedAssembly = 
    { ILScopeRef: ILScopeRef
      FSharpViewOfMetadata: CcuThunk
      AssemblyAutoOpenAttributes: string list
      AssemblyInternalsVisibleToAttributes: string list
#if !NO_EXTENSIONTYPING
      IsProviderGenerated: bool
      mutable TypeProviders: Tainted<Microsoft.FSharp.Core.CompilerServices.ITypeProvider> list
#endif
      FSharpOptimizationData: Lazy<Option<Optimizer.LazyModuleInfo>> }


[<Sealed>] 
type TcAssemblyResolutions = 
    member GetAssemblyResolutions: unit -> AssemblyResolution list
    static member SplitNonFoundationalResolutions : CompilationThreadToken * TcConfig -> AssemblyResolution list * AssemblyResolution list * UnresolvedAssemblyReference list
    static member BuildFromPriorResolutions    : CompilationThreadToken * TcConfig * AssemblyResolution list * UnresolvedAssemblyReference list -> TcAssemblyResolutions 

/// Represents a table of imported assemblies with their resolutions.
[<Sealed>] 
type TcImports =
    interface System.IDisposable
    //new: TcImports option -> TcImports
    member DllTable: NameMap<ImportedBinary> with get
    member GetImportedAssemblies: unit -> ImportedAssembly list
    member GetCcusInDeclOrder: unit -> CcuThunk list
    /// This excludes any framework imports (which may be shared between multiple builds)
    member GetCcusExcludingBase: unit -> CcuThunk list 
    member FindDllInfo: CompilationThreadToken * range * string -> ImportedBinary
    member TryFindDllInfo: CompilationThreadToken * range * string * lookupOnly: bool -> option<ImportedBinary>
    member FindCcuFromAssemblyRef: CompilationThreadToken * range * ILAssemblyRef -> CcuResolutionResult
#if !NO_EXTENSIONTYPING
    member ProviderGeneratedTypeRoots: ProviderGeneratedType list
#endif
    member GetImportMap: unit -> Import.ImportMap

    /// Try to resolve a referenced assembly based on TcConfig settings.
    member TryResolveAssemblyReference: CompilationThreadToken * AssemblyReference * ResolveAssemblyReferenceMode -> OperationResult<AssemblyResolution list>

    /// Resolve a referenced assembly and report an error if the resolution fails.
    member ResolveAssemblyReference: CompilationThreadToken * AssemblyReference * ResolveAssemblyReferenceMode -> AssemblyResolution list

    /// Try to find the given assembly reference by simple name.  Used in magic assembly resolution.  Effectively does implicit
    /// unification of assemblies by simple assembly name.
    member TryFindExistingFullyQualifiedPathBySimpleAssemblyName: CompilationThreadToken * string -> string option

    /// Try to find the given assembly reference.
    member TryFindExistingFullyQualifiedPathByExactAssemblyRef: CompilationThreadToken * ILAssemblyRef -> string option

#if !NO_EXTENSIONTYPING
    /// Try to find a provider-generated assembly
    member TryFindProviderGeneratedAssemblyByName: CompilationThreadToken * assemblyName:string -> System.Reflection.Assembly option
#endif
    /// Report unresolved references that also weren't consumed by any type providers.
    member ReportUnresolvedAssemblyReferences: UnresolvedAssemblyReference list -> unit
    member SystemRuntimeContainsType: string -> bool

    static member BuildFrameworkTcImports     : CompilationThreadToken * TcConfigProvider * AssemblyResolution list * AssemblyResolution list -> Cancellable<TcGlobals * TcImports>
    static member BuildNonFrameworkTcImports  : CompilationThreadToken * TcConfigProvider * TcGlobals * TcImports * AssemblyResolution list * UnresolvedAssemblyReference list -> Cancellable<TcImports>
    static member BuildTcImports              : CompilationThreadToken * TcConfigProvider -> Cancellable<TcGlobals * TcImports>

//----------------------------------------------------------------------------
// Special resources in DLLs
//--------------------------------------------------------------------------

/// Determine if an IL resource attached to an F# assembly is an F# signature data resource
val IsSignatureDataResource: ILResource -> bool

/// Determine if an IL resource attached to an F# assembly is an F# optimization data resource
val IsOptimizationDataResource: ILResource -> bool

/// Determine if an IL resource attached to an F# assembly is an F# quotation data resource for reflected definitions
val IsReflectedDefinitionsResource: ILResource -> bool
val GetSignatureDataResourceName: ILResource -> string

/// Write F# signature data as an IL resource
val WriteSignatureData: TcConfig * TcGlobals * Tastops.Remap * CcuThunk * filename: string * inMem: bool -> ILResource

/// Write F# optimization data as an IL resource
val WriteOptimizationData: TcGlobals * filename: string * inMem: bool * CcuThunk * Optimizer.LazyModuleInfo -> ILResource

//----------------------------------------------------------------------------
// #r and other directives
//--------------------------------------------------------------------------

//----------------------------------------------------------------------------
// #r and other directives
//--------------------------------------------------------------------------

/// Process #r in F# Interactive.
/// Adds the reference to the tcImports and add the ccu to the type checking environment.
val RequireDLL: CompilationThreadToken * TcImports * TcEnv * thisAssemblyName: string * referenceRange: range * file: string -> TcEnv * (ImportedBinary list * ImportedAssembly list)

/// Processing # commands
val ProcessMetaCommandsFromInput: 
    (('T -> range * string -> 'T) * ('T -> range * string -> 'T) * ('T -> range * string -> unit)) 
    -> TcConfigBuilder * Ast.ParsedInput * string * 'T 
    -> 'T

/// Process all the #r, #I etc. in an input
val ApplyMetaCommandsFromInputToTcConfig: TcConfig * Ast.ParsedInput * string -> TcConfig

/// Process the #nowarn in an input
val ApplyNoWarnsToTcConfig: TcConfig * Ast.ParsedInput * string -> TcConfig

//----------------------------------------------------------------------------
// Scoped pragmas
//--------------------------------------------------------------------------

/// Find the scoped #nowarn pragmas with their range information
val GetScopedPragmasForInput: Ast.ParsedInput -> ScopedPragma list

/// Get an error logger that filters the reporting of warnings based on scoped pragma information
val GetErrorLoggerFilteringByScopedPragmas: checkFile:bool * ScopedPragma list * ErrorLogger  -> ErrorLogger

/// This list is the default set of references for "non-project" files. 
val DefaultReferencesForScriptsAndOutOfProjectSources: bool -> string list

//----------------------------------------------------------------------------
// Parsing
//--------------------------------------------------------------------------

/// Parse one input file
val ParseOneInputFile: TcConfig * Lexhelp.LexResourceManager * string list * string * isLastCompiland: (bool * bool) * ErrorLogger * (*retryLocked*) bool -> ParsedInput option

//----------------------------------------------------------------------------
// Type checking and querying the type checking state
//--------------------------------------------------------------------------

/// Get the initial type checking environment including the loading of mscorlib/System.Core, FSharp.Core
/// applying the InternalsVisibleTo in referenced assemblies and opening 'Checked' if requested.
val GetInitialTcEnv: assemblyName: string * range * TcConfig * TcImports * TcGlobals -> TcEnv
                
[<Sealed>]
/// Represents the incremental type checking state for a set of inputs
type TcState =
    member NiceNameGenerator: Ast.NiceNameGenerator

    /// The CcuThunk for the current assembly being checked
    member Ccu: CcuThunk
    
    /// Get the typing environment implied by the set of signature files and/or inferred signatures of implementation files checked so far
    member TcEnvFromSignatures: TcEnv

    /// Get the typing environment implied by the set of implementation files checked so far
    member TcEnvFromImpls: TcEnv

    /// The inferred contents of the assembly, containing the signatures of all files.
    // a.fsi + b.fsi + c.fsi (after checking implementation file for c.fs)
    member CcuSig: ModuleOrNamespaceType

    member NextStateAfterIncrementalFragment: TcEnv -> TcState

    member CreatesGeneratedProvidedTypes: bool

/// Get the initial type checking state for a set of inputs
val GetInitialTcState: 
    range * string * TcConfig * TcGlobals * TcImports * Ast.NiceNameGenerator * TcEnv -> TcState

/// Check one input, returned as an Eventually computation
val TypeCheckOneInputEventually :
    checkForErrors:(unit -> bool) * TcConfig * TcImports * TcGlobals * Ast.LongIdent option * NameResolution.TcResultsSink * TcState * Ast.ParsedInput  
           -> Eventually<(TcEnv * TopAttribs * TypedImplFile option * ModuleOrNamespaceType) * TcState>

/// Finish the checking of multiple inputs 
val TypeCheckMultipleInputsFinish: (TcEnv * TopAttribs * 'T option * 'U) list * TcState -> (TcEnv * TopAttribs * 'T list * 'U list) * TcState
    
/// Finish the checking of a closed set of inputs 
val TypeCheckClosedInputSetFinish: TypedImplFile list * TcState -> TcState * TypedImplFile list

/// Check a closed set of inputs 
val TypeCheckClosedInputSet: CompilationThreadToken * checkForErrors: (unit -> bool) * TcConfig * TcImports * TcGlobals * Ast.LongIdent option * TcState * Ast.ParsedInput  list  -> TcState * TopAttribs * TypedImplFile list * TcEnv

/// Check a single input and finish the checking
val TypeCheckOneInputAndFinishEventually :
    checkForErrors: (unit -> bool) * TcConfig * TcImports * TcGlobals * Ast.LongIdent option * NameResolution.TcResultsSink * TcState * Ast.ParsedInput 
        -> Eventually<(TcEnv * TopAttribs * TypedImplFile list * ModuleOrNamespaceType list) * TcState>

/// Indicates if we should report a warning
val ReportWarning: FSharpErrorSeverityOptions -> PhasedDiagnostic -> bool

/// Indicates if we should report a warning as an error
val ReportWarningAsError: FSharpErrorSeverityOptions -> PhasedDiagnostic -> bool

//----------------------------------------------------------------------------
// #load closure
//--------------------------------------------------------------------------

[<RequireQualifiedAccess>]
type CodeContext =
    | CompilationAndEvaluation
    | Compilation
    | Editing

[<RequireQualifiedAccess>]
type LoadClosureInput = 
    { FileName: string
      SyntaxTree: ParsedInput option
      ParseDiagnostics: (PhasedDiagnostic * bool) list 
      MetaCommandDiagnostics: (PhasedDiagnostic * bool) list  }

[<RequireQualifiedAccess>]
type LoadClosure = 
    { /// The source files along with the ranges of the #load positions in each file.
      SourceFiles: (string * range list) list

      /// The resolved references along with the ranges of the #r positions in each file.
      References: (string * AssemblyResolution list) list

      /// The list of references that were not resolved during load closure.
      UnresolvedReferences: UnresolvedAssemblyReference list

      /// The list of all sources in the closure with inputs when available, with associated parse errors and warnings
      Inputs: LoadClosureInput list

      /// The original #load references, including those that didn't resolve
      OriginalLoadReferences: (range * string) list

      /// The #nowarns
      NoWarns: (string * range list) list

      /// Diagnostics seen while processing resolutions
      ResolutionDiagnostics: (PhasedDiagnostic * bool)  list

      /// Diagnostics to show for root of closure (used by fsc.fs)
      AllRootFileDiagnostics: (PhasedDiagnostic * bool) list

      /// Diagnostics seen while processing the compiler options implied root of closure
      LoadClosureRootFileDiagnostics: (PhasedDiagnostic * bool) list }   

    /// Analyze a script text and find the closure of its references. 
    /// Used from FCS, when editing a script file.  
    //
    /// A temporary TcConfig is created along the way, is why this routine takes so many arguments. We want to be sure to use exactly the
    /// same arguments as the rest of the application.
    static member ComputeClosureOfScriptText: CompilationThreadToken * legacyReferenceResolver: ReferenceResolver.Resolver * defaultFSharpBinariesDir: string * filename: string * sourceText: ISourceText * implicitDefines:CodeContext * useSimpleResolution: bool * useFsiAuxLib: bool * useSdkRefs: bool * lexResourceManager: Lexhelp.LexResourceManager * applyCompilerOptions: (TcConfigBuilder -> unit) * assumeDotNetFramework: bool * tryGetMetadataSnapshot: ILReaderTryGetMetadataSnapshot * reduceMemoryUsage: ReduceMemoryFlag -> LoadClosure

    /// Analyze a set of script files and find the closure of their references. The resulting references are then added to the given TcConfig.
    /// Used from fsi.fs and fsc.fs, for #load and command line. 
    static member ComputeClosureOfScriptFiles: CompilationThreadToken * tcConfig:TcConfig * (string * range) list * implicitDefines:CodeContext * lexResourceManager: Lexhelp.LexResourceManager -> LoadClosure
