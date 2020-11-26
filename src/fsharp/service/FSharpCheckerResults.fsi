// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices


open System.Threading
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Range
open FSharp.Compiler.ScriptClosure
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.CheckDeclarations

/// Represents the reason why the GetDeclarationLocation operation failed.
[<RequireQualifiedAccess>]
type public FSharpFindDeclFailureReason = 

    /// Generic reason: no particular information about error apart from a message
    | Unknown of message: string

    /// Source code file is not available
    | NoSourceCode

    /// Trying to find declaration of ProvidedType without TypeProviderDefinitionLocationAttribute
    | ProvidedType of string

    /// Trying to find declaration of ProvidedMember without TypeProviderDefinitionLocationAttribute
    | ProvidedMember of string

/// Represents the result of the GetDeclarationLocation operation.
[<RequireQualifiedAccess>]
type public FSharpFindDeclResult = 

    /// Indicates a declaration location was not found, with an additional reason
    | DeclNotFound of FSharpFindDeclFailureReason

    /// Indicates a declaration location was found
    | DeclFound    of range

    /// Indicates an external declaration was found
    | ExternalDecl of assembly : string * externalSym : ExternalSymbol
     
/// Represents the checking context implied by the ProjectOptions 
[<Sealed>]
type public FSharpProjectContext =

    /// Get the resolution and full contents of the assemblies referenced by the project options
    member GetReferencedAssemblies : unit -> FSharpAssembly list

    /// Get the accessibility rights for this project context w.r.t. InternalsVisibleTo attributes granting access to other assemblies
    member AccessibilityRights : FSharpAccessibilityRights

/// Options used to determine active --define conditionals and other options relevant to parsing files in a project
type public FSharpParsingOptions =
    { 
      SourceFiles: string[]
      ConditionalCompilationDefines: string list
      ErrorSeverityOptions: FSharpErrorSeverityOptions
      IsInteractive: bool
      LightSyntax: bool option
      CompilingFsLib: bool
      IsExe: bool
    }
    static member Default: FSharpParsingOptions

    static member internal FromTcConfig: tcConfig: TcConfig * sourceFiles: string[]  * isInteractive: bool -> FSharpParsingOptions

    static member internal FromTcConfigBuilder: tcConfigB: TcConfigBuilder * sourceFiles: string[] * isInteractive: bool -> FSharpParsingOptions

/// A handle to the results of CheckFileInProject.
[<Sealed>]
type public FSharpCheckFileResults =
    /// The errors returned by parsing a source file.
    member Errors : FSharpErrorInfo[]

    /// Get a view of the contents of the assembly up to and including the file just checked
    member PartialAssemblySignature : FSharpAssemblySignature

    /// Get the resolution of the ProjectOptions 
    member ProjectContext : FSharpProjectContext

    /// Indicates whether type checking successfully occurred with some results returned. If false, indicates that 
    /// an unrecoverable error in earlier checking/parsing/resolution steps.
    member HasFullTypeCheckInfo: bool

    /// Tries to get the current successful TcImports. This is only used in testing. Do not use it for other stuff.
    member internal TryGetCurrentTcImports: unit -> TcImports option

    /// Indicates the set of files which must be watched to accurately track changes that affect these results,
    /// Clients interested in reacting to updates to these files should watch these files and take actions as described
    /// in the documentation for compiler service.
    member DependencyFiles : string[]

    /// <summary>Get the items for a declaration list</summary>
    ///
    /// <param name="parsedFileResults">
    ///    If this is present, it is used to filter declarations based on location in the
    ///    parse tree, specifically at 'open' declarations, 'inherit' of class or interface
    ///    'record field' locations and r.h.s. of 'range' operator a..b
    /// </param>
    /// <param name="line">The line number where the completion is happening</param>
    /// <param name="partialName">
    ///    Partial long name. QuickParse.GetPartialLongNameEx can be used to get it.
    /// </param>
    /// <param name="lineText">
    ///    The text of the line where the completion is happening. This is only used to make a couple
    ///    of adhoc corrections to completion accuracy (e.g. checking for "..")
    /// </param>
    /// <param name="getAllEntities">
    ///    Function that returns all entities from current and referenced assemblies.
    /// </param>
    member GetDeclarationListInfo: parsedFileResults:FSharpParseFileResults option * line: int * lineText:string * partialName: PartialLongName * ?getAllEntities: (unit -> AssemblySymbol list) -> FSharpDeclarationListInfo

    /// <summary>Get the items for a declaration list in FSharpSymbol format</summary>
    ///
    /// <param name="parsedFileResults">
    ///    If this is present, it is used to filter declarations based on location in the
    ///    parse tree, specifically at 'open' declarations, 'inherit' of class or interface
    ///    'record field' locations and r.h.s. of 'range' operator a..b
    /// </param>
    /// <param name="line">The line number where the completion is happening</param>
    /// <param name="partialName">
    ///    Partial long name. QuickParse.GetPartialLongNameEx can be used to get it.
    /// </param>
    /// <param name="lineText">
    ///    The text of the line where the completion is happening. This is only used to make a couple
    ///    of adhoc corrections to completion accuracy (e.g. checking for "..")
    /// </param>
    /// <param name="getAllEntities">
    ///    Function that returns all entities from current and referenced assemblies.
    /// </param>
    member GetDeclarationListSymbols: parsedFileResults:FSharpParseFileResults option * line: int * lineText:string * partialName: PartialLongName * ?getAllEntities: (unit -> AssemblySymbol list) -> FSharpSymbolUse list list

    /// <summary>Compute a formatted tooltip for the given location</summary>
    ///
    /// <param name="line">The line number where the information is being requested.</param>
    /// <param name="colAtEndOfNames">The column number at the end of the identifiers where the information is being requested.</param>
    /// <param name="lineText">The text of the line where the information is being requested.</param>
    /// <param name="names">The identifiers at the location where the information is being requested.</param>
    /// <param name="tokenTag">Used to discriminate between 'identifiers', 'strings' and others. For strings, an attempt is made to give a tooltip for a #r "..." location. Use a value from FSharpTokenInfo.Tag, or FSharpTokenTag.Identifier, unless you have other information available.</param>
    member GetStructuredToolTipText : line:int * colAtEndOfNames:int * lineText:string * names:string list * tokenTag:int -> FSharpStructuredToolTipText

    /// <summary>Compute a formatted tooltip for the given location</summary>
    ///
    /// <param name="line">The line number where the information is being requested.</param>
    /// <param name="colAtEndOfNames">The column number at the end of the identifiers where the information is being requested.</param>
    /// <param name="lineText">The text of the line where the information is being requested.</param>
    /// <param name="names">The identifiers at the location where the information is being requested.</param>
    /// <param name="tokenTag">Used to discriminate between 'identifiers', 'strings' and others. For strings, an attempt is made to give a tooltip for a #r "..." location. Use a value from FSharpTokenInfo.Tag, or FSharpTokenTag.Identifier, unless you have other information available.</param>
    member GetToolTipText : line:int * colAtEndOfNames:int * lineText:string * names:string list * tokenTag:int -> FSharpToolTipText

    /// <summary>Compute the Visual Studio F1-help key identifier for the given location, based on name resolution results</summary>
    ///
    /// <param name="line">The line number where the information is being requested.</param>
    /// <param name="colAtEndOfNames">The column number at the end of the identifiers where the information is being requested.</param>
    /// <param name="lineText">The text of the line where the information is being requested.</param>
    /// <param name="names">The identifiers at the location where the information is being requested.</param>
    member GetF1Keyword : line:int * colAtEndOfNames:int * lineText:string * names:string list -> string option

    /// <summary>Compute a set of method overloads to show in a dialog relevant to the given code location.</summary>
    ///
    /// <param name="line">The line number where the information is being requested.</param>
    /// <param name="colAtEndOfNames">The column number at the end of the identifiers where the information is being requested.</param>
    /// <param name="lineText">The text of the line where the information is being requested.</param>
    /// <param name="names">The identifiers at the location where the information is being requested.</param>
    member GetMethods : line:int * colAtEndOfNames:int * lineText:string * names:string list option -> FSharpMethodGroup

    /// <summary>Compute a set of method overloads to show in a dialog relevant to the given code location.  The resulting method overloads are returned as symbols.</summary>
    /// <param name="line">The line number where the information is being requested.</param>
    /// <param name="colAtEndOfNames">The column number at the end of the identifiers where the information is being requested.</param>
    /// <param name="lineText">The text of the line where the information is being requested.</param>
    /// <param name="names">The identifiers at the location where the information is being requested.</param>
    member GetMethodsAsSymbols : line:int * colAtEndOfNames:int * lineText:string * names:string list -> FSharpSymbolUse list option

    /// <summary>Resolve the names at the given location to the declaration location of the corresponding construct.</summary>
    ///
    /// <param name="line">The line number where the information is being requested.</param>
    /// <param name="colAtEndOfNames">The column number at the end of the identifiers where the information is being requested.</param>
    /// <param name="lineText">The text of the line where the information is being requested.</param>
    /// <param name="names">The identifiers at the location where the information is being requested.</param>
    /// <param name="preferFlag">If not given, then get the location of the symbol. If false, then prefer the location of the corresponding symbol in the implementation of the file (rather than the signature if present). If true, prefer the location of the corresponding symbol in the signature of the file (rather than the implementation).</param>
    member GetDeclarationLocation : line:int * colAtEndOfNames:int * lineText:string * names:string list * ?preferFlag:bool -> FSharpFindDeclResult

    /// <summary>Resolve the names at the given location to a use of symbol.</summary>
    ///
    /// <param name="line">The line number where the information is being requested.</param>
    /// <param name="colAtEndOfNames">The column number at the end of the identifiers where the information is being requested.</param>
    /// <param name="lineText">The text of the line where the information is being requested.</param>
    /// <param name="names">The identifiers at the location where the information is being requested.</param>
    member GetSymbolUseAtLocation  : line:int * colAtEndOfNames:int * lineText:string * names:string list -> FSharpSymbolUse option

    /// <summary>Get any extra colorization info that is available after the typecheck</summary>
    member GetSemanticClassification : range option -> struct (range * SemanticClassificationType)[]

    /// <summary>Get the locations of format specifiers</summary>
    [<System.Obsolete("This member has been replaced by GetFormatSpecifierLocationsAndArity, which returns both range and arity of specifiers")>]
    member GetFormatSpecifierLocations : unit -> range[]

    /// <summary>Get the locations of and number of arguments associated with format specifiers</summary>
    member GetFormatSpecifierLocationsAndArity : unit -> (range*int)[]

    /// Get all textual usages of all symbols throughout the file
    member GetAllUsesOfAllSymbolsInFile : ?cancellationToken: CancellationToken -> seq<FSharpSymbolUse>

    /// Get the textual usages that resolved to the given symbol throughout the file
    member GetUsesOfSymbolInFile : symbol:FSharpSymbol * ?cancellationToken: CancellationToken -> FSharpSymbolUse[]

    member internal GetVisibleNamespacesAndModulesAtPoint : pos -> ModuleOrNamespaceRef[]

    /// Find the most precise display environment for the given line and column.
    member GetDisplayContextForPos : cursorPos : pos -> FSharpDisplayContext option

    /// Determines if a long ident is resolvable at a specific point.
    member internal IsRelativeNameResolvable: cursorPos : pos * plid : string list * item: Item -> bool

    /// Determines if a long ident is resolvable at a specific point.
    member IsRelativeNameResolvableFromSymbol: cursorPos : pos * plid : string list * symbol: FSharpSymbol -> bool

    /// Represents complete typechecked implementation file, including its typechecked signatures if any.
    member ImplementationFile: FSharpImplementationFileContents option

    /// Open declarations in the file, including auto open modules.
    member OpenDeclarations: FSharpOpenDeclaration[]

    /// Internal constructor
    static member internal MakeEmpty : 
        filename: string *
        creationErrors: FSharpErrorInfo[] *
        keepAssemblyContents: bool 
          -> FSharpCheckFileResults
        
    /// Internal constructor
    static member internal Make: 
        mainInputFileName: string * 
        projectFileName: string *
        tcConfig: TcConfig *
        tcGlobals: TcGlobals *
        isIncompleteTypeCheckEnvironment: bool *
        builder: IncrementalBuilder * 
        dependencyFiles: string[] * 
        creationErrors: FSharpErrorInfo[] *
        parseErrors: FSharpErrorInfo[] *
        tcErrors: FSharpErrorInfo[] *
        keepAssemblyContents: bool *
        ccuSigForFile: ModuleOrNamespaceType *
        thisCcu: CcuThunk *
        tcImports: TcImports *
        tcAccessRights: AccessorDomain *
        sResolutions: TcResolutions *
        sSymbolUses: TcSymbolUses *
        sFallback: NameResolutionEnv *
        loadClosure : LoadClosure option *
        implFileOpt: TypedImplFile option *
        openDeclarations: OpenDeclaration[]
          -> FSharpCheckFileResults

    /// Internal constructor - check a file and collect errors
    static member internal CheckOneFile: 
         parseResults: FSharpParseFileResults *
         sourceText: ISourceText *
         mainInputFileName: string *
         projectFileName: string *
         tcConfig: TcConfig *
         tcGlobals: TcGlobals *
         tcImports: TcImports *
         tcState: TcState *
         moduleNamesDict: ModuleNamesDict *
         loadClosure: LoadClosure option *
         backgroundDiagnostics: (PhasedDiagnostic * FSharpErrorSeverity)[] *    
         reactorOps: IReactorOperations *
         userOpName: string *
         isIncompleteTypeCheckEnvironment: bool * 
         builder: IncrementalBuilder * 
         dependencyFiles: string[] * 
         creationErrors:FSharpErrorInfo[] * 
         parseErrors:FSharpErrorInfo[] * 
         keepAssemblyContents: bool *
         suggestNamesForErrors: bool
          ->  Async<FSharpCheckFileAnswer>

/// The result of calling TypeCheckResult including the possibility of abort and background compiler not caught up.
and [<RequireQualifiedAccess>] public FSharpCheckFileAnswer =
    /// Aborted because cancellation caused an abandonment of the operation
    | Aborted 
    
    /// Success 
    | Succeeded of FSharpCheckFileResults    

/// A handle to the results of CheckFileInProject.
[<Sealed>]
type public FSharpCheckProjectResults =

    /// The errors returned by processing the project
    member Errors: FSharpErrorInfo[]

    /// Get a view of the overall signature of the assembly. Only valid to use if HasCriticalErrors is false.
    member AssemblySignature: FSharpAssemblySignature

    /// Get a view of the overall contents of the assembly. Only valid to use if HasCriticalErrors is false.
    member AssemblyContents: FSharpAssemblyContents

    /// Get an optimized view of the overall contents of the assembly. Only valid to use if HasCriticalErrors is false.
    member GetOptimizedAssemblyContents: unit -> FSharpAssemblyContents

    /// Get the resolution of the ProjectOptions 
    member ProjectContext: FSharpProjectContext

    /// Get the textual usages that resolved to the given symbol throughout the project
    member GetUsesOfSymbol: symbol:FSharpSymbol * ?cancellationToken: CancellationToken -> FSharpSymbolUse[]

    /// Get all textual usages of all symbols throughout the project
    member GetAllUsesOfAllSymbols: ?cancellationToken: CancellationToken  -> FSharpSymbolUse[]

    /// Indicates if critical errors existed in the project options
    member HasCriticalErrors: bool 

    /// Indicates the set of files which must be watched to accurately track changes that affect these results,
    /// Clients interested in reacting to updates to these files should watch these files and take actions as described
    /// in the documentation for compiler service.
    member DependencyFiles: string[]

    member internal RawFSharpAssemblyData : IRawFSharpAssemblyData option

    // Internal constructor.
    internal new : 
        projectFileName:string *
        tcConfigOption: TcConfig option *
        keepAssemblyContents: bool *
        errors: FSharpErrorInfo[] * 
        details:(TcGlobals * TcImports * CcuThunk * ModuleOrNamespaceType * TcSymbolUses list * TopAttribs option * IRawFSharpAssemblyData option * ILAssemblyRef * AccessorDomain * TypedImplFile list option * string[]) option 
           -> FSharpCheckProjectResults

module internal ParseAndCheckFile = 

    val parseFile: 
        sourceText: ISourceText * 
        fileName: string * 
        options: FSharpParsingOptions * 
        userOpName: string *
        suggestNamesForErrors: bool
          -> FSharpErrorInfo[] * ParsedInput option * bool

    val matchBraces: 
        sourceText: ISourceText *
        fileName: string *
        options: FSharpParsingOptions *
        userOpName: string *
        suggestNamesForErrors: bool
          -> (range * range)[]

// An object to typecheck source in a given typechecking environment.
// Used internally to provide intellisense over F# Interactive.
type internal FsiInteractiveChecker =
    internal new: 
        ReferenceResolver.Resolver *
        ops: IReactorOperations *
        tcConfig: TcConfig * 
        tcGlobals: TcGlobals * 
        tcImports: TcImports * 
        tcState: TcState 
          ->  FsiInteractiveChecker 

    member internal ParseAndCheckInteraction : 
        ctok: CompilationThreadToken * 
        sourceText:ISourceText * 
        ?userOpName: string 
          -> Async<FSharpParseFileResults * FSharpCheckFileResults * FSharpCheckProjectResults>

module internal FSharpCheckerResultsSettings =
    val defaultFSharpBinariesDir: string

    val maxTimeShareMilliseconds : int64
