// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Coordinating compiler operations - configuration, loading initial context, reporting errors etc.
module internal Microsoft.FSharp.Compiler.CompileOps

open System.Text
open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.TypeChecker
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.MSBuildResolver
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Core.CompilerServices
#if EXTENSIONTYPING
open Microsoft.FSharp.Compiler.ExtensionTyping
#endif


#if DEBUG

#if COMPILED_AS_LANGUAGE_SERVICE_DLL
module internal CompilerService =
#else
module internal FullCompiler =
#endif
    val showAssertForUnexpectedException : bool ref

#endif

//----------------------------------------------------------------------------
// File names and known file suffixes
//--------------------------------------------------------------------------

/// Signature file suffixes
val FSharpSigFileSuffixes : string list

/// Implementation file suffixes
val FSharpImplFileSuffixes : string list

/// Script file suffixes
val FSharpScriptFileSuffixes : string list

val IsScript : string -> bool

/// File suffixes where #light is the default
val FSharpLightSyntaxFileSuffixes : string list


/// Get the name used for FSharp.Core
val GetFSharpCoreLibraryName : unit -> string

//----------------------------------------------------------------------------
// Parsing inputs
//--------------------------------------------------------------------------
  
val ComputeQualifiedNameOfFileFromUniquePath : range * string list -> Ast.QualifiedNameOfFile

val PrependPathToInput : Ast.Ident list -> Ast.ParsedInput -> Ast.ParsedInput

val ParseInput : (UnicodeLexing.Lexbuf -> Parser.token) * ErrorLogger * UnicodeLexing.Lexbuf * string option * string * isLastCompiland:(bool * bool) -> Ast.ParsedInput

//----------------------------------------------------------------------------
// Error and warnings
//--------------------------------------------------------------------------

/// Represents the style being used to format errros
type ErrorStyle = 
    | DefaultErrors 
    | EmacsErrors 
    | TestErrors 
    | VSErrors
    | GccErrors

/// Get the location associated with an error
val GetRangeOfError : PhasedError -> range option

/// Get the number associated with an error
val GetErrorNumber : PhasedError -> int

/// Split errors into a "main" error and a set of associated errors
val SplitRelatedErrors : PhasedError -> PhasedError * PhasedError list

/// Output an error to a buffer
val OutputPhasedError : StringBuilder -> PhasedError -> bool -> unit

/// Output an error or warning to a buffer
val OutputErrorOrWarning : implicitIncludeDir:string * showFullPaths: bool * flattenErrors: bool * errorStyle: ErrorStyle *  warning:bool -> StringBuilder -> PhasedError -> unit

/// Output extra context information for an error or warning to a buffer
val OutputErrorOrWarningContext : prefix:string -> fileLineFunction:(string -> int -> string) -> StringBuilder -> PhasedError -> unit

[<RequireQualifiedAccess>]
type ErrorLocation =
    { Range : range
      File : string
      TextRepresentation : string
      IsEmpty : bool }

[<RequireQualifiedAccess>]
type CanonicalInformation = 
    { ErrorNumber : int
      Subcategory : string
      TextRepresentation : string }

[<RequireQualifiedAccess>]
type DetailedIssueInfo = 
    { Location : ErrorLocation option
      Canonical : CanonicalInformation
      Message : string }

[<RequireQualifiedAccess>]
type ErrorOrWarning = 
    | Short of bool * string
    | Long of bool * DetailedIssueInfo

val CollectErrorOrWarning : implicitIncludeDir:string * showFullPaths: bool * flattenErrors: bool * errorStyle: ErrorStyle *  warning:bool * PhasedError -> seq<ErrorOrWarning>

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
    abstract GetAutoOpenAttributes : ILGlobals -> string list
    ///  The raw list InternalsVisibleToAttribute attributes in the assembly
    abstract GetInternalsVisibleToAttributes : ILGlobals  -> string list
    ///  The raw IL module definition in the assembly, if any. This is not present for cross-project references
    /// in the language service
    abstract TryGetRawILModule : unit -> ILModuleDef option
    abstract HasAnyFSharpSignatureDataAttribute : bool
    abstract HasMatchingFSharpSignatureDataAttribute : ILGlobals -> bool
    ///  The raw F# signature data in the assembly, if any
    abstract GetRawFSharpSignatureData : range * ilShortAssemName: string * fileName: string -> (string * byte[]) list
    ///  The raw F# optimization data in the assembly, if any
    abstract GetRawFSharpOptimizationData : range * ilShortAssemName: string * fileName: string -> (string * (unit -> byte[])) list
    ///  The table of type forwarders in the assembly
    abstract GetRawTypeForwarders : unit -> ILExportedTypesAndForwarders
    /// The identity of the module
    abstract ILScopeRef : ILScopeRef
    abstract ILAssemblyRefs : ILAssemblyRef list
    abstract ShortAssemblyName : string

type IProjectReference = 
    /// The name of the assembly file generated by the project
    abstract FileName : string 
    /// Evaluate raw contents of the assembly file generated by the project
    abstract EvaluateRawContents : unit -> IRawFSharpAssemblyData option
    /// Get the logical timestamp that would be the timestamp of the assembly file generated by the project
    abstract GetLogicalTimeStamp : unit -> System.DateTime option

type AssemblyReference = 
    | AssemblyReference of range * string  * IProjectReference option
    member Range : range
    member Text : string
    member ProjectReference : IProjectReference option

type AssemblyResolution = 
      {/// The original reference to the assembly.
       originalReference : AssemblyReference
       /// Path to the resolvedFile
       resolvedPath : string    
       /// Search path used to find this spot.
       resolvedFrom : ResolvedFrom
       /// The qualified name of the assembly
       fusionName : string
       /// Name of the redist, if any, that the assembly was found in.
       redist : string 
       /// Whether or not this is an installed system assembly (for example, System.dll)
       sysdir : bool
       // Lazily populated ilAssemblyRef for this reference. 
       ilAssemblyRef : ILAssemblyRef option ref  }

type UnresolvedAssemblyReference = UnresolvedAssemblyReference of string * AssemblyReference list

#if EXTENSIONTYPING
type ResolvedExtensionReference = ResolvedExtensionReference of string * AssemblyReference list * Tainted<ITypeProvider> list
#endif

type CompilerTarget = 
    | WinExe 
    | ConsoleExe 
    | Dll 
    | Module
    member IsExe : bool
    
type ResolveAssemblyReferenceMode = 
    | Speculative 
    | ReportErrors

//----------------------------------------------------------------------------
// TcConfig
//--------------------------------------------------------------------------

/// Represents the file or string used for the --version flag
type VersionFlag = 
    | VersionString of string
    | VersionFile of string
    | VersionNone
    member GetVersionInfo : implicitIncludeDir:string -> ILVersionInfo
    member GetVersionString : implicitIncludeDir:string -> string

     
type TcConfigBuilder =
    { mutable primaryAssembly : PrimaryAssembly
      mutable autoResolveOpenDirectivesToDlls: bool
      mutable noFeedback: bool
      mutable stackReserveSize: int32 option
      mutable implicitIncludeDir: string
      mutable openBinariesInMemory: bool
      mutable openDebugInformationForLaterStaticLinking: bool
      defaultFSharpBinariesDir: string
      mutable compilingFslib: bool
      mutable compilingFslib20: string option
      mutable compilingFslib40: bool
      mutable useIncrementalBuilder: bool
      mutable includes: string list
      mutable implicitOpens: string list
      mutable useFsiAuxLib: bool
      mutable framework: bool
      mutable resolutionEnvironment : Microsoft.FSharp.Compiler.MSBuildResolver.ResolutionEnvironment
      mutable implicitlyResolveAssemblies : bool
      mutable addVersionSpecificFrameworkReferences : bool
      /// Set if the user has explicitly turned indentation-aware syntax on/off
      mutable light: bool option
      mutable conditionalCompilationDefines: string list
      /// Sources added into the build with #load
      mutable loadedSources: (range * string) list
      
      mutable referencedDLLs: AssemblyReference  list
      mutable projectReferences : IProjectReference list
      mutable knownUnresolvedReferences : UnresolvedAssemblyReference list
      optimizeForMemory: bool
      mutable subsystemVersion : int * int
      mutable useHighEntropyVA : bool
      mutable inputCodePage: int option
      mutable embedResources : string list
      mutable globalWarnAsError: bool
      mutable globalWarnLevel: int
      mutable specificWarnOff: int list 
      mutable specificWarnOn: int list 
      mutable specificWarnAsError: int list 
      mutable specificWarnAsWarn : int list
      mutable mlCompatibility:bool
      mutable checkOverflow:bool
      mutable showReferenceResolutions:bool
      mutable outputFile : string option
      mutable resolutionFrameworkRegistryBase : string
      mutable resolutionAssemblyFoldersSuffix : string 
      mutable resolutionAssemblyFoldersConditions : string          
      mutable platform : ILPlatform option
      mutable prefer32Bit : bool
      mutable useMonoResolution : bool
      mutable target : CompilerTarget
      mutable debuginfo : bool
      mutable testFlagEmitFeeFeeAs100001 : bool
      mutable dumpDebugInfo : bool
      mutable debugSymbolFile : string option
      mutable typeCheckOnly : bool
      mutable parseOnly : bool
      mutable importAllReferencesOnly : bool
      mutable simulateException : string option
      mutable printAst : bool
      mutable tokenizeOnly : bool
      mutable testInteractionParser : bool
      mutable reportNumDecls : bool
      mutable printSignature : bool
      mutable printSignatureFile : string
      mutable xmlDocOutputFile : string option
      mutable stats : bool
      mutable generateFilterBlocks : bool 
      mutable signer : string option
      mutable container : string option
      mutable delaysign : bool
      mutable publicsign : bool
      mutable version : VersionFlag 
      mutable metadataVersion : string option
      mutable standalone : bool
      mutable extraStaticLinkRoots : string list 
      mutable noSignatureData : bool
      mutable onlyEssentialOptimizationData : bool
      mutable useOptimizationDataFile : bool
      mutable useSignatureDataFile : bool
      mutable portablePDB : bool
      mutable ignoreSymbolStoreSequencePoints : bool
      mutable internConstantStrings : bool
      mutable extraOptimizationIterations : int
      mutable win32res : string 
      mutable win32manifest : string
      mutable includewin32manifest : bool
      mutable linkResources : string list
      mutable showFullPaths : bool
      mutable errorStyle : ErrorStyle
      mutable utf8output : bool
      mutable flatErrors : bool
      mutable maxErrors : int
      mutable abortOnError : bool
      mutable baseAddress : int32 option
 #if DEBUG
      mutable writeGeneratedILFiles : bool (* write il files? *)  
      mutable showOptimizationData : bool
#endif
      mutable showTerms     : bool 
      mutable writeTermsToFiles : bool 
      mutable doDetuple     : bool 
      mutable doTLR         : bool 
      mutable doFinalSimplify : bool
      mutable optsOn        : bool 
      mutable optSettings   : Optimizer.OptimizationSettings 
      mutable emitTailcalls : bool
#if PREFERRED_UI_LANG
      mutable preferredUiLang: string option
#else
      mutable lcid         : int option
#endif
      mutable productNameForBannerText : string
      mutable showBanner  : bool
      mutable showTimes : bool
      mutable showLoadedAssemblies : bool
      mutable continueAfterParseFailure : bool
#if EXTENSIONTYPING
      mutable showExtensionTypeMessages : bool
#endif
      mutable pause : bool 
      mutable alwaysCallVirt : bool
      mutable noDebugData : bool

      /// If true, indicates all type checking and code generation is in the context of fsi.exe
      isInteractive : bool 
      isInvalidationSupported : bool 
      mutable sqmSessionGuid : System.Guid option
      mutable sqmNumOfSourceFiles : int
      sqmSessionStartedTime : int64
      mutable emitDebugInfoInQuotations : bool
      mutable exename : string option 
      mutable copyFSharpCore : bool
#if SHADOW_COPY_REFERENCES
      mutable shadowCopyReferences : bool
#endif
    }

    static member CreateNew : 
        defaultFSharpBinariesDir: string * 
        optimizeForMemory: bool * 
        implicitIncludeDir: string * 
        isInteractive: bool * 
        isInvalidationSupported: bool -> TcConfigBuilder

    member DecideNames : string list -> outfile: string * pdbfile: string option * assemblyName: string 
    member TurnWarningOff : range * string -> unit
    member TurnWarningOn : range * string -> unit
    member AddIncludePath : range * string * string -> unit
    member AddReferencedAssemblyByPath : range * string -> unit
    member RemoveReferencedAssemblyByPath : range * string -> unit
    member AddEmbeddedResource : string -> unit
    
    static member SplitCommandLineResourceInfo : string -> string * string * ILResourceAccess


    
[<Sealed>]
// Immutable TcConfig
type TcConfig =
    member primaryAssembly: PrimaryAssembly
    member autoResolveOpenDirectivesToDlls: bool
    member noFeedback: bool
    member stackReserveSize: int32 option
    member implicitIncludeDir: string
    member openBinariesInMemory: bool
    member openDebugInformationForLaterStaticLinking: bool
    member fsharpBinariesDir: string
    member compilingFslib: bool
    member compilingFslib20: string option
    member compilingFslib40: bool
    member useIncrementalBuilder: bool
    member includes: string list
    member implicitOpens: string list
    member useFsiAuxLib: bool
    member framework: bool
    member implicitlyResolveAssemblies : bool
    /// Set if the user has explicitly turned indentation-aware syntax on/off
    member light: bool option
    member conditionalCompilationDefines: string list
    member subsystemVersion : int * int
    member useHighEntropyVA : bool
    member referencedDLLs: AssemblyReference list
    member optimizeForMemory: bool
    member inputCodePage: int option
    member embedResources : string list
    member globalWarnAsError: bool
    member globalWarnLevel: int
    member specificWarnOn: int list 
    member specificWarnOff: int list 
    member specificWarnAsError: int list 
    member specificWarnAsWarn : int list
    member mlCompatibility:bool
    member checkOverflow:bool
    member showReferenceResolutions:bool
    member outputFile : string option
    member resolutionFrameworkRegistryBase : string
    member resolutionAssemblyFoldersSuffix : string 
    member resolutionAssemblyFoldersConditions : string          
    member platform : ILPlatform option
    member prefer32Bit : bool
    member useMonoResolution : bool
    member target : CompilerTarget
    member debuginfo : bool
    member testFlagEmitFeeFeeAs100001 : bool
    member dumpDebugInfo : bool
    member debugSymbolFile : string option
    member typeCheckOnly : bool
    member parseOnly : bool
    member importAllReferencesOnly : bool
    member simulateException : string option
    member printAst : bool
    member tokenizeOnly : bool
    member testInteractionParser : bool
    member reportNumDecls : bool
    member printSignature : bool
    member printSignatureFile : string
    member xmlDocOutputFile : string option
    member stats : bool
    member generateFilterBlocks : bool 
    member signer : string option
    member container : string option
    member delaysign : bool
    member publicsign : bool
    member version : VersionFlag 
    member metadataVersion : string option
    member standalone : bool
    member extraStaticLinkRoots : string list 
    member noSignatureData : bool
    member onlyEssentialOptimizationData : bool
    member useOptimizationDataFile : bool
    member useSignatureDataFile : bool
    member portablePDB : bool
    member ignoreSymbolStoreSequencePoints : bool
    member internConstantStrings : bool
    member extraOptimizationIterations : int
    member win32res : string 
    member win32manifest : string
    member includewin32manifest : bool
    member linkResources : string list
    member showFullPaths : bool
    member errorStyle : ErrorStyle
    member utf8output : bool
    member flatErrors : bool

    member maxErrors : int
    member baseAddress : int32 option
#if DEBUG
    member writeGeneratedILFiles : bool (* write il files? *)  
    member showOptimizationData : bool
#endif
    member showTerms     : bool 
    member writeTermsToFiles : bool 
    member doDetuple     : bool 
    member doTLR         : bool 
    member doFinalSimplify : bool
    member optSettings   : Optimizer.OptimizationSettings 
    member emitTailcalls : bool
#if PREFERRED_UI_LANG
    member preferredUiLang: string option
#else
    member lcid         : int option
#endif
    member optsOn        : bool 
    member productNameForBannerText : string
    member showBanner  : bool
    member showTimes : bool
    member showLoadedAssemblies : bool
    member continueAfterParseFailure : bool
#if EXTENSIONTYPING
    member showExtensionTypeMessages : bool
#endif
    member pause : bool 
    member alwaysCallVirt : bool
    member noDebugData : bool

    /// If true, indicates all type checking and code generation is in the context of fsi.exe
    member isInteractive : bool
    member isInvalidationSupported : bool 


    member ComputeLightSyntaxInitialStatus : string -> bool
    member ClrRoot : string list
    
    /// Get the loaded sources that exist and issue a warning for the ones that don't
    member GetAvailableLoadedSources : unit -> (range*string) list
    
    member ComputeCanContainEntryPoint : sourceFiles:string list -> bool list *bool 

    /// File system query based on TcConfig settings
    member ResolveSourceFile : range * string * string -> string
    /// File system query based on TcConfig settings
    member MakePathAbsolute : string -> string

    member sqmSessionGuid : System.Guid option
    member sqmNumOfSourceFiles : int
    member sqmSessionStartedTime : int64
    member copyFSharpCore : bool
#if SHADOW_COPY_REFERENCES
    member shadowCopyReferences : bool
#endif
    static member Create : TcConfigBuilder * validate: bool -> TcConfig

/// Represents a computation to return a TcConfig. Normally this is just a constant immutable TcConfig,
/// but for F# Interactive it may be based on an underlying mutable TcConfigBuilder.
[<Sealed>]
type TcConfigProvider = 

    member Get : unit -> TcConfig

    /// Get a TcConfigProvider which will return only the exact TcConfig.
    static member Constant : TcConfig -> TcConfigProvider

    /// Get a TcConfigProvider which will continue to respect changes in the underlying
    /// TcConfigBuilder rather than delivering snapshots.
    static member BasedOnMutableBuilder : TcConfigBuilder -> TcConfigProvider

//----------------------------------------------------------------------------
// Tables of referenced DLLs 
//--------------------------------------------------------------------------

/// Represents a resolved imported binary
[<RequireQualifiedAccess>]
type ImportedBinary = 
    { FileName: string
      RawMetadata: IRawFSharpAssemblyData
#if EXTENSIONTYPING
      ProviderGeneratedAssembly: System.Reflection.Assembly option
      IsProviderGenerated: bool
      ProviderGeneratedStaticLinkMap : ProvidedAssemblyStaticLinkingMap  option
#endif
      ILAssemblyRefs : ILAssemblyRef list
      ILScopeRef: ILScopeRef}

/// Represents a resolved imported assembly
[<RequireQualifiedAccess>]
type ImportedAssembly = 
    { ILScopeRef: ILScopeRef
      FSharpViewOfMetadata: CcuThunk
      AssemblyAutoOpenAttributes: string list
      AssemblyInternalsVisibleToAttributes: string list
#if EXTENSIONTYPING
      IsProviderGenerated: bool
      mutable TypeProviders: Tainted<Microsoft.FSharp.Core.CompilerServices.ITypeProvider> list
#endif
      FSharpOptimizationData : Lazy<Option<Optimizer.LazyModuleInfo>> }


[<Sealed>] 
type TcAssemblyResolutions = 
    member GetAssemblyResolutions : unit -> AssemblyResolution list

    static member SplitNonFoundationalResolutions  : TcConfig -> AssemblyResolution list * AssemblyResolution list * UnresolvedAssemblyReference list
    static member BuildFromPriorResolutions     : TcConfig * AssemblyResolution list * UnresolvedAssemblyReference list -> TcAssemblyResolutions 
    


/// Repreesnts a table of imported assemblies with their resolutions.
[<Sealed>] 
type TcImports =
    interface System.IDisposable
    //new : TcImports option -> TcImports
    member SetBase : TcImports -> unit
    member DllTable : NameMap<ImportedBinary> with get
    member GetImportedAssemblies : unit -> ImportedAssembly list
    member GetCcusInDeclOrder : unit -> CcuThunk list
    /// This excludes any framework imports (which may be shared between multiple builds)
    member GetCcusExcludingBase : unit -> CcuThunk list 
    member FindDllInfo : range * string -> ImportedBinary
    member TryFindDllInfo : range * string * lookupOnly: bool -> option<ImportedBinary>
    member FindCcuFromAssemblyRef : range * ILAssemblyRef -> Tast.CcuResolutionResult
#if EXTENSIONTYPING
    member ProviderGeneratedTypeRoots : ProviderGeneratedType list
#endif
    member GetImportMap : unit -> Import.ImportMap

    /// Try to resolve a referenced assembly based on TcConfig settings.
    member TryResolveAssemblyReference : AssemblyReference * ResolveAssemblyReferenceMode -> OperationResult<AssemblyResolution list>

    /// Resolve a referenced assembly and report an error if the resolution fails.
    member ResolveAssemblyReference : AssemblyReference * ResolveAssemblyReferenceMode -> AssemblyResolution list
    /// Try to find the given assembly reference.
    member TryFindExistingFullyQualifiedPathFromAssemblyRef : ILAssemblyRef -> string option
#if EXTENSIONTYPING
    /// Try to find a provider-generated assembly
    member TryFindProviderGeneratedAssemblyByName : assemblyName:string -> System.Reflection.Assembly option
#endif
    /// Report unresolved references that also weren't consumed by any type providers.
    member ReportUnresolvedAssemblyReferences : UnresolvedAssemblyReference list -> unit
    member SystemRuntimeContainsType : string -> bool

    static member BuildFrameworkTcImports      : TcConfigProvider * AssemblyResolution list * AssemblyResolution list -> TcGlobals * TcImports
    static member BuildNonFrameworkTcImports   : TcConfigProvider * TcGlobals * TcImports * AssemblyResolution list * UnresolvedAssemblyReference list -> TcImports
    static member BuildTcImports               : TcConfigProvider -> TcGlobals * TcImports

//----------------------------------------------------------------------------
// Special resources in DLLs
//--------------------------------------------------------------------------

/// Determine if an IL resource attached to an F# assemnly is an F# signature data resource
val IsSignatureDataResource : ILResource -> bool

/// Determine if an IL resource attached to an F# assemnly is an F# optimization data resource
val IsOptimizationDataResource : ILResource -> bool

/// Determine if an IL resource attached to an F# assemnly is an F# quotation data resource for reflected definitions
val IsReflectedDefinitionsResource : ILResource -> bool
val GetSignatureDataResourceName : ILResource -> string

#if NO_COMPILER_BACKEND
#else
/// Write F# signature data as an IL resource
val WriteSignatureData : TcConfig * TcGlobals * Tastops.Remap * CcuThunk * string -> ILResource

/// Write F# optimization data as an IL resource
val WriteOptimizationData :  TcGlobals * string * CcuThunk * Optimizer.LazyModuleInfo -> ILResource
#endif


//----------------------------------------------------------------------------
// #r and other directives
//--------------------------------------------------------------------------

/// Process #r in F# Interactive.
/// Adds the reference to the tcImports and add the ccu to the type checking environment.
val RequireDLL : TcImports * TcEnv * thisAssemblyName: string * referenceRange: range * file: string -> TcEnv * (ImportedBinary list * ImportedAssembly list)

/// Processing # commands
val ProcessMetaCommandsFromInput : 
              ('T -> range * string -> 'T) * 
              ('T -> range * string -> 'T) * 
              ('T -> range * string -> unit) -> TcConfigBuilder -> Ast.ParsedInput -> string -> 'T -> 'T

/// Process all the #r, #I etc. in an input
val ApplyMetaCommandsFromInputToTcConfig : TcConfig -> (Ast.ParsedInput * string) -> TcConfig

/// Process the #nowarn in an input
val ApplyNoWarnsToTcConfig : TcConfig -> (Ast.ParsedInput*string) -> TcConfig


//----------------------------------------------------------------------------
// Scoped pragmas
//--------------------------------------------------------------------------

/// Find the scoped #nowarn pragmas with their range information
val GetScopedPragmasForInput : Ast.ParsedInput -> ScopedPragma list

/// Get an error logger that filters the reporting of warnings based on scoped pragma information
val GetErrorLoggerFilteringByScopedPragmas : checkFile:bool * ScopedPragma list * ErrorLogger  -> ErrorLogger

/// This list is the default set of references for "non-project" files. 
val DefaultBasicReferencesForOutOfProjectSources : string list

//----------------------------------------------------------------------------
// Parsing
//--------------------------------------------------------------------------

/// Parse one input file
val ParseOneInputFile : TcConfig * Lexhelp.LexResourceManager * string list * string * isLastCompiland: (bool * bool) * ErrorLogger * (*retryLocked*) bool -> ParsedInput option

//----------------------------------------------------------------------------
// Type checking and querying the type checking state
//--------------------------------------------------------------------------

/// Get the initial type checking environment including the loading of mscorlib/System.Core, FSharp.Core
/// applying the InternalsVisibleTo in referenced assemblies and opening 'Checked' if requested.
val GetInitialTcEnv : assemblyName: string * range * TcConfig * TcImports * TcGlobals -> TcEnv
                
[<Sealed>]
/// Represents the incremental type checking state for a set of inputs
type TcState =
    member NiceNameGenerator : Ast.NiceNameGenerator

    /// The CcuThunk for the current assembly being checked
    member Ccu : CcuThunk
    
    /// Get the typing environment implied by the set of signature files and/or inferred signatures of implementation files checked so far
    member TcEnvFromSignatures : TcEnv

    /// Get the typing environment implied by the set of implemetation files checked so far
    member TcEnvFromImpls : TcEnv
    /// The inferred contents of the assembly, containing the signatures of all implemented files.
    member PartialAssemblySignature : ModuleOrNamespaceType

    member NextStateAfterIncrementalFragment : TcEnv -> TcState

/// Get the initial type checking state for a set of inputs
val GetInitialTcState : 
    range * string * TcConfig * TcGlobals * TcImports * Ast.NiceNameGenerator * TcEnv -> TcState

/// Check one input, returned as an Eventually computation
val TypeCheckOneInputEventually :
    (unit -> bool) * TcConfig * TcImports * TcGlobals * Ast.LongIdent option * NameResolution.TcResultsSink * TcState * Ast.ParsedInput  
           -> Eventually<(TcEnv * TopAttribs * Tast.TypedImplFile list) * TcState>

/// Finish the checking of multiple inputs 
val TypeCheckMultipleInputsFinish : (TcEnv * TopAttribs * 'T list) list * TcState -> (TcEnv * TopAttribs * 'T list) * TcState
    
/// Finish the checking of a closed set of inputs 
val TypeCheckClosedInputSetFinish : TypedImplFile list * TcState -> TcState * TypedAssembly

/// Check a closed set of inputs 
val TypeCheckClosedInputSet :
    (unit -> bool) * TcConfig * TcImports * TcGlobals * Ast.LongIdent option * TcState * Ast.ParsedInput  list 
        -> TcState * TopAttribs * Tast.TypedAssembly * TcEnv

/// Check a single input and finish the checking
val TypeCheckSingleInputAndFinishEventually :
    (unit -> bool) * TcConfig * TcImports * TcGlobals * Ast.LongIdent option * NameResolution.TcResultsSink * TcState * Ast.ParsedInput 
        -> Eventually<(TcEnv * TopAttribs * Tast.TypedImplFile list) * TcState>

/// Indicates if we should report a warning
val ReportWarning : globalWarnLevel: int * specificWarnOff: int list * specificWarnOn: int list -> PhasedError -> bool

/// Indicates if we should report a warning as an error
val ReportWarningAsError : globalWarnLevel: int * specificWarnOff: int list * specificWarnOn: int list * specificWarnAsError: int list * specificWarnAsWarn: int list * globalWarnAsError: bool -> PhasedError -> bool

//----------------------------------------------------------------------------
// #load closure
//--------------------------------------------------------------------------

[<RequireQualifiedAccess>]
type CodeContext =
    | Evaluation
    | Compilation
    | Editing

[<RequireQualifiedAccess>]
type LoadClosure = 
    { /// The source files along with the ranges of the #load positions in each file.
      SourceFiles: (string * range list) list

      /// The resolved references along with the ranges of the #r positions in each file.
      References: (string * AssemblyResolution list) list

      /// The list of references that were not resolved during load closure. These may still be extension references.
      UnresolvedReferences : UnresolvedAssemblyReference list

      /// The list of all sources in the closure with inputs when available
      Inputs: (string * ParsedInput option) list

      /// The #nowarns
      NoWarns: (string * range list) list

      /// *Parse* errors seen while parsing root of closure
      RootErrors : PhasedError list

      /// *Parse* warnings seen while parsing root of closure
      RootWarnings : PhasedError list }

    // Used from service.fs, when editing a script file
    static member ComputeClosureOfSourceText : filename: string * source: string * implicitDefines:CodeContext * useMonoResolution: bool * useFsiAuxLib: bool * lexResourceManager: Lexhelp.LexResourceManager * applyCompilerOptions: (TcConfigBuilder -> unit) -> LoadClosure

    /// Used from fsi.fs and fsc.fs, for #load and command line. The resulting references are then added to a TcConfig.
    static member ComputeClosureOfSourceFiles : tcConfig:TcConfig * (string * range) list * implicitDefines:CodeContext * useDefaultScriptingReferences : bool * lexResourceManager : Lexhelp.LexResourceManager -> LoadClosure
