// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// API to the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//----------------------------------------------------------------------------

namespace FSharp.Compiler.SourceCodeServices

open System.Collections.Generic

open FSharp.Compiler
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.Text

[<Sealed>]
/// Represents the results of parsing an F# file
type public FSharpParseFileResults = 

    /// The syntax tree resulting from the parse
    member ParseTree : ParsedInput option

    /// Attempts to find the range of the name of the nearest outer binding that contains a given position.
    member TryRangeOfNameOfNearestOuterBindingContainingPos: pos: pos -> range option

    /// Attempts to find the range of an attempted lambda expression or pattern, the argument range, and the expr range when writing a C#-style "lambda" (which is actually an operator application)
    member TryRangeOfParenEnclosingOpEqualsGreaterUsage: opGreaterEqualPos: pos -> (range * range * range) option

    /// Attempts to find the range of an expression `expr` contained in a `yield expr`  or `return expr` expression (and bang-variants).
    member TryRangeOfExprInYieldOrReturn: pos: pos -> range option

    /// Attempts to find the range of a record expression containing the given position.
    member TryRangeOfRecordExpressionContainingPos: pos: pos -> range option

    /// Attempts to find an Ident of a pipeline containing the given position, and the number of args already applied in that pipeline.
    /// For example, '[1..10] |> List.map ' would give back the ident of '|>' and 1, because it applied 1 arg (the list) to 'List.map'.
    member TryIdentOfPipelineContainingPosAndNumArgsApplied: pos: pos -> (Ident * int) option

    /// Determines if the given position is inside a function or method application.
    member IsPosContainedInApplication: pos: pos -> bool

    /// Attempts to find the range of a function or method that is being applied. Also accounts for functions in pipelines.
    member TryRangeOfFunctionOrMethodBeingApplied: pos: pos -> range option

    /// Gets the ranges of all arguments, if they can be found, for a function application at the given position.
    member GetAllArgumentsForFunctionApplicationAtPostion: pos: pos -> range list option

    /// <summary>
    /// Given the position of an expression, attempts to find the range of the
    /// '!' in a derefence operation of that expression, like:
    /// '!expr', '!(expr)', etc.
    /// </summary>
    member TryRangeOfRefCellDereferenceContainingPos: expressionPos: pos -> range option

    /// Notable parse info for ParameterInfo at a given location
    member FindNoteworthyParamInfoLocations : pos:pos -> FSharpNoteworthyParamInfoLocations option

    /// Determines if the given position is contained within a curried parameter in a binding.
    member IsPositionContainedInACurriedParameter: pos: pos -> bool

    /// Name of the file for which this information were created
    member FileName                       : string

    /// Get declared items and the selected item at the specified location
    member GetNavigationItems             : unit -> FSharpNavigationItems

    /// Return the inner-most range associated with a possible breakpoint location
    member ValidateBreakpointLocation : pos:pos -> range option

    /// When these files change then the build is invalid
    member DependencyFiles : string[]

    /// Get the errors and warnings for the parse
    member Errors : FSharpDiagnostic[]

    /// Indicates if any errors occurred during the parse
    member ParseHadErrors : bool

    internal new: errors: FSharpDiagnostic[] * input: ParsedInput option * parseHadErrors: bool * dependencyFiles: string[] -> FSharpParseFileResults

/// Information about F# source file names
module public SourceFile =

   /// Whether or not this file is compilable
   val IsCompilable : string -> bool

   /// Whether or not this file should be a single-file project
   val MustBeSingleFileProject : string -> bool

type public CompletionPath = string list * string option // plid * residue

[<RequireQualifiedAccess>]
type public InheritanceContext = 
    | Class
    | Interface
    | Unknown

[<RequireQualifiedAccess>]
type public RecordContext =
    | CopyOnUpdate of range: range * path: CompletionPath
    | Constructor of typeName: string
    | New of path: CompletionPath

[<RequireQualifiedAccess>]
type public CompletionContext = 
    /// Completion context cannot be determined due to errors
    | Invalid

    /// Completing something after the inherit keyword
    | Inherit of context: InheritanceContext * path: CompletionPath

    /// Completing records field
    | RecordField of context: RecordContext

    | RangeOperator

    /// Completing named parameters\setters in parameter list of constructor\method calls
    /// end of name ast node * list of properties\parameters that were already set
    | ParameterList of pos * HashSet<string>

    | AttributeApplication

    | OpenDeclaration of isOpenType: bool

    /// Completing pattern type (e.g. foo (x: |))
    | PatternType


type public ModuleKind = { IsAutoOpen: bool; HasModuleSuffix: bool }

[<RequireQualifiedAccess>]
type public EntityKind =
    | Attribute
    | Type
    | FunctionOrValue of isActivePattern:bool
    | Module of ModuleKind

// implementation details used by other code in the compiler    
module public UntypedParseImpl =
    val TryFindExpressionASTLeftOfDotLeftOfCursor : pos * ParsedInput option -> (pos * bool) option

    val GetRangeOfExprLeftOfDot : pos  * ParsedInput option -> range option

    val TryFindExpressionIslandInPosition : pos * ParsedInput option -> string option

    val TryGetCompletionContext : pos * ParsedInput * lineStr: string -> CompletionContext option

    val GetEntityKind: pos * ParsedInput -> EntityKind option

    val GetFullNameOfSmallestModuleOrNamespaceAtPoint : ParsedInput * pos -> string[]

// implementation details used by other code in the compiler    
module internal SourceFileImpl =

    val IsInterfaceFile : string -> bool 

    val AdditionalDefinesForUseInEditor: isInteractive: bool -> string list

