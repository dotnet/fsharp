// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.EditorServices

open System.Collections.Generic
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

type public CompletionPath = string list * string option // plid * residue

[<RequireQualifiedAccess>]
type public InheritanceContext = 
    | Class
    | Interface
    | Unknown

[<RequireQualifiedAccess>]
type public FSharpRecordContext =
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
    | RecordField of context: FSharpRecordContext

    | RangeOperator

    /// Completing named parameters\setters in parameter list of constructor\method calls
    /// end of name ast node * list of properties\parameters that were already set
    | ParameterList of pos * HashSet<string>

    | AttributeApplication

    | OpenDeclaration of isOpenType: bool

    /// Completing pattern type (e.g. foo (x: |))
    | PatternType

type public FSharpModuleKind =
    { IsAutoOpen: bool
      HasModuleSuffix: bool }

[<RequireQualifiedAccess>]
type public FSharpEntityKind =
    | Attribute
    | Type
    | FunctionOrValue of isActivePattern:bool
    | Module of FSharpModuleKind

/// Kind of lexical scope.
[<RequireQualifiedAccess>]
type public FSharpScopeKind =
    | Namespace
    | TopModule
    | NestedModule
    | OpenDeclaration
    | HashDirective

/// Insert open namespace context.
[<RequireQualifiedAccess>]
type public FSharpInsertionContext =
    {
      /// Current scope kind.
      ScopeKind: FSharpScopeKind

      /// Current position (F# compiler line number).
      Pos: pos
    }

/// Where open statements should be added.
[<RequireQualifiedAccess>]
type public FSharpOpenStatementInsertionPoint =
    | TopLevel
    | Nearest

/// Short identifier, i.e. an identifier that contains no dots.
type public FSharpShortIdent = string

/// An array of `ShortIdent`.
type public FSharpShortIdents = FSharpShortIdent[]

/// `ShortIdent` with a flag indicating if it's resolved in some scope.
type public FSharpMaybeUnresolvedIdent = 
    { Ident: FSharpShortIdent; Resolved: bool }

/// Long identifier (i.e. it may contain dots).
type public FSharpLongIdent = string

/// Helper data structure representing a symbol, suitable for implementing unresolved identifiers resolution code fixes.
type public FSharpParsedEntity =
    {
      /// Full name, relative to the current scope.
      FullRelativeName: FSharpLongIdent

      /// Ident parts needed to append to the current ident to make it resolvable in current scope.
      Qualifier: FSharpLongIdent

      /// Namespace that is needed to open to make the entity resolvable in the current scope.
      Namespace: FSharpLongIdent option

      /// Full display name (i.e. last ident plus modules with `RequireQualifiedAccess` attribute prefixed).
      FullDisplayName: FSharpLongIdent

      /// Last part of the entity's full name.
      LastIdent: FSharpShortIdent
    }

/// Operations querying the entire syntax tree
module public ParsedInput =
    val TryFindExpressionASTLeftOfDotLeftOfCursor: pos * ParsedInput option -> (pos * bool) option

    val GetRangeOfExprLeftOfDot: pos  * ParsedInput option -> range option

    val TryFindExpressionIslandInPosition: pos * ParsedInput option -> string option

    val TryGetCompletionContext: pos * ParsedInput * lineStr: string -> CompletionContext option

    val GetEntityKind: pos * ParsedInput -> FSharpEntityKind option

    val GetFullNameOfSmallestModuleOrNamespaceAtPoint: ParsedInput * pos -> string[]

    /// Returns `InsertContext` based on current position and symbol idents.
    val TryFindInsertionContext: 
        currentLine: int -> 
        ast: ParsedInput -> 
        partiallyQualifiedName: FSharpMaybeUnresolvedIdent[] -> 
        insertionPoint: FSharpOpenStatementInsertionPoint ->
        (( (* requiresQualifiedAccessParent: *) FSharpShortIdents option * (* autoOpenParent: *) FSharpShortIdents option * (*  entityNamespace *) FSharpShortIdents option * (* entity: *) FSharpShortIdents) -> (FSharpParsedEntity * FSharpInsertionContext)[])
    
    /// Returns `InsertContext` based on current position and symbol idents.
    val FindNearestPointToInsertOpenDeclaration: currentLine: int -> ast: ParsedInput -> entity: FSharpShortIdents -> insertionPoint: FSharpOpenStatementInsertionPoint -> FSharpInsertionContext

    /// Returns long identifier at position.
    val GetLongIdentAt: ast: ParsedInput -> pos: pos -> LongIdent option

    /// Corrects insertion line number based on kind of scope and text surrounding the insertion point.
    val AdjustInsertionPoint: getLineStr: (int -> string) -> ctx: FSharpInsertionContext -> pos

// implementation details used by other code in the compiler    
module internal SourceFileImpl =

    val IsInterfaceFile: string -> bool 

    val AdditionalDefinesForUseInEditor: isInteractive: bool -> string list

