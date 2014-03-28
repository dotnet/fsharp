// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// API to the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//----------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range
open System.Collections.Generic

type internal Param = 
    { Name: string
      CanonicalTypeTextForSorting: string
      Display: string
      Description: string }

[<NoEquality; NoComparison>]
// Note: this type does not hold any handles to compiler data structure.
type internal Method = 
    { Description : DataTipText
      Type: string
      Parameters: Param[]
      /// Indicates that this not really a method, but actually a static arguments list, like TP<42,"foo"> ?
      IsStaticArguments: bool }

[<Sealed>]
// Note: this type does not hold any handles to compiler data structure. All data has been pre-formatted.
type internal MethodOverloads = 
    member Name: string
    member Methods: Method[] 

[<RequireQualifiedAccess>]
type internal FindDeclFailureReason = 
    // generic reason: no particular information about error
    | Unknown
    // source code file is not available
    | NoSourceCode
    // trying to find declaration of ProvidedType without TypeProviderDefinitionLocationAttribute
    | ProvidedType of string
    // trying to find declaration of ProvidedMember without TypeProviderDefinitionLocationAttribute
    | ProvidedMember of string

[<NoEquality; NoComparison>]
type internal FindDeclResult = 
    /// declaration not found + reason
    | DeclNotFound of FindDeclFailureReason
    /// found declaration; return (position-in-file, name-of-file)
    | DeclFound      of Position * string
     
type internal Names = string list 
type internal NamesWithResidue = Names * string 

[<Sealed>]
/// A handle to the results of TypeCheckSource.  
/// A live object of this type keeps the background corresponding background builder (and type providers) alive (through reference-counting)
type internal TypeCheckResults =
    /// The errors returned by parsing a source file
    member Errors : ErrorInfo[]

    member HasFullTypeCheckInfo: bool

    /// Intellisense autocompletions
    member GetDeclarations                : untypedParseInfoOpt:UntypedParseInfo option * position:Position * lineText:string * names:NamesWithResidue * hasTextChangedSinceLastTypecheck: (obj * Range -> bool) -> Async<DeclarationSet>
    /// Resolve the names at the given location to give a data tip 
    member GetDataTipText                 : position:Position * lineText:string * names:Names * tokenTag:int -> DataTipText
    /// Resolve the names at the given location to give F1 keyword
    member GetF1Keyword                   : position:Position * lineText:string * names:Names -> string option
    /// Resolve the names at the given location to a set of methods
    member GetMethods                     : position:Position * lineText:string * names:Names option -> MethodOverloads
    /// Resolve the names at the given location to the declaration location of the corresponding construct
    member GetDeclarationLocation         : position:Position * lineText:string * names:Names * tokenTag:int * isDeclaration:bool -> FindDeclResult

    /// Get any extra colorization info that is available after the typecheck
    member GetExtraColorizations : unit -> (Range * TokenColorKind)[]

/// wraps the set of unresolved references providing implementations of Equals\GetHashCode
/// of this objects of this type can be used as parts of types with generated Equals\GetHashCode
/// i.e. records or DUs
type internal UnresolvedReferencesSet = class end

/// A set of key information for the language service's internal caches of project/script build information for a particular source file
type internal CheckOptions = 
    { 
      // Note that this may not reduce to just the project directory, because there may be two projects in the same directory.
      ProjectFileName: string
      ProjectFileNames: string[]
      ProjectOptions: string[]
      /// When true, the typechecking environment is known a priori to be incomplete. 
      /// This can happen, for example, when a .fs file is opened outside of a project.
      /// It may be appropriate, then, to not show error messages related to type checking
      /// since they will just be noise.
      IsIncompleteTypeCheckEnvironment : bool
      /// When true, use the reference resolution rules for scripts rather than the rules for compiler.
      UseScriptResolutionRules : bool
      /// Timestamp of project/script load
      LoadTime : System.DateTime
      UnresolvedReferences : UnresolvedReferencesSet option
    }
         
          
/// Information about the compilation environment    
module internal CompilerEnvironment =
    /// These are the names of assemblies that should be referenced for .fs, .ml, .fsi, .mli files that
    /// are not asscociated with a project.
    val DefaultReferencesForOrphanSources : string list
    /// Return the compilation defines that should be used when editing the given file.
    val GetCompilationDefinesForEditing : filename : string * compilerFlags : string list -> string list
    /// Return true if this is a subcategory of error or warning message that the language service can emit
    val IsCheckerSupportedSubcategory : string -> bool

/// Information about the debugging environment
module internal DebuggerEnvironment =
    /// Return the language ID, which is the expression evaluator id that the
    /// debugger will use.
    val GetLanguageID : unit -> System.Guid
    
/// This file has become eligible to be re-typechecked.
/// This notifies the language service that it needs to set the dirty flag on files whose typecheck antecedents have changed.
type internal NotifyFileTypeCheckStateIsDirty = NotifyFileTypeCheckStateIsDirty of (string -> unit)
        
/// Identical to _VSFILECHANGEFLAGS in vsshell.idl
type internal DependencyChangeCode =
    | NoChange = 0x0
    | FileChanged = 0x00000001
    | TimeChanged = 0x00000002
    | Deleted = 0x00000008
    | Added = 0x00000010   
    
/// Callback that indicates whether a requested result has become obsolete.    
[<NoComparison;NoEquality>]
type internal IsResultObsolete = 
    | IsResultObsolete of (unit->bool)

/// The result of calling TypeCheckResult including the possibility of abort and background compiler not caught up.
[<NoComparison>]
type internal TypeCheckAnswer =
    | NoAntecedant
    | Aborted // because result was obsolete
    | TypeCheckSucceeded of TypeCheckResults    

[<Sealed>]
[<AutoSerializable(false)>]      
type internal InteractiveChecker =
    /// Create an instance of an InteractiveChecker.  Currently resources are not reclaimed.
    static member Create : NotifyFileTypeCheckStateIsDirty -> InteractiveChecker

    /// Parse a source code file, returning information about brace matching in the file
    /// Return an enumeration of the matching parethetical tokens in the file
    member MatchBraces : filename : string * source: string * options: CheckOptions -> (Range * Range)[]

    /// Parse a source code file, returning a handle that can be used for obtaining navigation bar information
    /// To get the full information, call 'TypeCheckSource' method on the result
    member UntypedParse : filename: string * source: string * options: CheckOptions -> UntypedParseInfo        

    /// Typecheck a source code file, returning a handle to the results of the parse including
    /// the reconstructed types in the file.
    ///
    /// Return None if the background builder is not yet done prepring the type check results for the antecedent to the 
    /// file.
    member TypeCheckSource : parsed: UntypedParseInfo * filename: string * fileversion: int * source: string * options: CheckOptions * isResultObsolete: IsResultObsolete * textSnapshotInfo: obj -> TypeCheckAnswer
    
    /// For a given script file, get the CheckOptions implied by the #load closure
    member GetCheckOptionsFromScriptRoot : filename : string * source : string * loadedTimestamp : System.DateTime -> CheckOptions
        
#if NO_QUICK_SEARCH_HELPERS // only used in QuickSearch prototype
#else
    /// For QuickSearch index - not used by VS2008/VS2010/VS11
    member GetSlotsCount : options : CheckOptions -> int
    /// For QuickSearch index - not used by VS2008/VS2010/VS11
    member UntypedParseForSlot : slot:int * options : CheckOptions -> UntypedParseInfo
#endif // QUICK_SEARCH

    /// Try to get recent type check results for a file. This may arbitrarily refuse to return any
    /// results if the InteractiveChecker would like a chance to recheck the file, in which case
    /// UntypedParse and TypeCheckSource should be called. If the source of the file
    /// has changed the results returned by this function may be out of date, though may
    /// still be usable for generating intellsense menus and information.
    member TryGetRecentTypeCheckResultsForFile : filename: string * options:CheckOptions -> (UntypedParseInfo * TypeCheckResults * (*version*)int) option

    /// This function is called when the entire environment is known to have changed for reasons not encoded in the CheckOptions of any project/compilation.
    /// For example, the type provider approvals file may have changed.
    member InvalidateAll : unit -> unit    
        
    /// This function is called when the configuration is known to have changed for reasons not encoded in the CheckOptions.
    /// For example, dependent references may have been deleted or created.
    member InvalidateConfiguration: options: CheckOptions -> unit    

    /// Begin background parsing the given project.
    member StartBackgroundCompile: options: CheckOptions -> unit

    /// Stop the background compile.
    member StopBackgroundCompile : unit -> unit

    /// Block until the background compile finishes.
    member WaitForBackgroundCompile : unit -> unit
    
    /// Report a statistic for testability
    static member GlobalForegroundParseCountStatistic : int

    /// Report a statistic for testability
    static member GlobalForegroundTypeCheckCountStatistic : int

    /// Flush all caches and garbage collect
    member ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients : unit -> unit

    /// This function is called when a project has been cleaned/rebuilt, and thus any live type providers should be refreshed.
    member NotifyProjectCleaned: options: CheckOptions -> unit    
    



#if FSI_SERVER_INTELLISENSE
// These functions determine all declarations, called by fsi.fs for fsi-server requests.
module internal FsiIntelisense =
    val getDeclarations : Build.TcConfig * Env.TcGlobals * Build.TcImports * Build.TcState -> string -> string[] -> (string * string * string * int)[]
#endif

module internal PrettyNaming =
    val IsIdentifierPartCharacter     : (char -> bool)
    val IsLongIdentifierPartCharacter : (char -> bool)
    val GetLongNameFromString         : (string -> Names)
    // Temporary workaround for no localized resources in FSharp.LanguageService.dll
    val FormatAndOtherOverloadsString : (int -> string)
