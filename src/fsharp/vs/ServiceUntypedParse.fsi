// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// API to the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//----------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.ErrorLogger
open System.Collections.Generic

// implementation details used by other code in the compiler    
[<NoEquality; NoComparison>]
type internal UntypedParseResults = 
  { // Error infos
    Errors : ErrorInfo[]
    // Untyped AST
    Input : Ast.ParsedInput option
    // Do not report errors from the type checker
    ParseHadErrors : bool
    // When these files change then the build is invalid
    DependencyFiles : string list
    }

[<Sealed>]
type internal UntypedParseInfo = 
    member internal ParseTree : Ast.ParsedInput option
    /// Notable parse info for ParameterInfo at a given location
    member internal FindNoteworthyParamInfoLocations : line:int * col:int -> NoteworthyParamInfoLocations option
    /// Name of the file for which this information were created
    member internal FileName                       : string
    /// Get declared items and the selected item at the specified location
    member internal GetNavigationItems             : unit -> NavigationItems
    /// Return the inner-most range associated with a possible breakpoint location
    member internal ValidateBreakpointLocation : Position -> Range option
    /// When these files change then the build is invalid
    member internal DependencyFiles : unit -> string list
    internal new : parsed:UntypedParseResults -> UntypedParseInfo

/// Information about F# source file names
module internal SourceFile =
   /// Whether or not this file is compilable
   val IsCompilable : string -> bool
   /// Whether or not this file should be a single-file project
   val MustBeSingleFileProject : string -> bool

type internal CompletionPath = string list * string option // plid * residue

type internal InheritanceContext = 
    | Class
    | Interface
    | Unknown

type internal RecordContext =
    | CopyOnUpdate of range * CompletionPath // range
    | Constructor of string // typename
    | New of CompletionPath

type internal CompletionContext = 
    // completion context cannot be determined due to errors
    | Invalid
    // completing something after the inherit keyword
    | Inherit of InheritanceContext * CompletionPath
    // completing records field
    | RecordField of RecordContext
    | RangeOperator
    // completing property setters in constructor call
    // end of constructor ast node * list of properties that were already set
    | NewObject of pos * HashSet<string>

// implementation details used by other code in the compiler    
module internal UntypedParseInfoImpl =
    open Microsoft.FSharp.Compiler.Ast
    val GetUntypedParseResults : UntypedParseInfo -> UntypedParseResults
    val TryFindExpressionASTLeftOfDotLeftOfCursor : int * int * ParsedInput option -> (pos * bool) option
    val GetRangeOfExprLeftOfDot : int * int * ParsedInput option -> ((int*int) * (int*int)) option
    val TryFindExpressionIslandInPosition : int * int * ParsedInput option -> string option
    val TryGetCompletionContext : int * int * UntypedParseInfo option -> CompletionContext option

// implementation details used by other code in the compiler    
module internal SourceFileImpl =
    val IsInterfaceFile : string -> bool 
    val AdditionalDefinesForUseInEditor : string -> string list
