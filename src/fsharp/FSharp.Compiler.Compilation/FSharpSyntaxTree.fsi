namespace FSharp.Compiler.Compilation

open System
open System.Threading
open System.Collections.Generic
open System.Collections.Immutable
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.CompileOps
open FSharp.Compiler
open FSharp.Compiler.Ast
open FSharp.Compiler.Range
open Microsoft.CodeAnalysis

[<CustomEquality;NoComparison;RequireQualifiedAccess>]
type FSharpSyntaxNodeKind =
    | ParsedInput of ParsedInput
    | ModuleOrNamespace of SynModuleOrNamespace
    | ModuleDecl of SynModuleDecl
    | LongIdentWithDots of LongIdentWithDots
    | Ident of index: int * Ident
    | ComponentInfo of SynComponentInfo
    | TypeConstraint of SynTypeConstraint
    | MemberSig of SynMemberSig
    | TypeDefnSig of SynTypeDefnSig
    | TypeDefnSigRepr of SynTypeDefnSigRepr
    | ExceptionDefnRepr of SynExceptionDefnRepr
    | UnionCase of SynUnionCase
    | UnionCaseType of SynUnionCaseType
    | ArgInfo of SynArgInfo
    | TypeDefnSimpleRepr of SynTypeDefnSimpleRepr
    | SimplePat of SynSimplePat
    | EnumCase of SynEnumCase
    | Const of SynConst
    | Measure of SynMeasure
    | RationalConst of SynRationalConst
    | TypeDefnKind of SynTypeDefnKind
    | Field of SynField
    | ValSig of SynValSig
    | ValTyparDecls of SynValTyparDecls
    | Type of SynType
    | SimplePats of SynSimplePats
    | Typar of SynTypar
    | TyparDecl of SynTyparDecl
    | Binding of SynBinding
    | ValData of SynValData
    | ValInfo of SynValInfo
    | Pat of SynPat
    | ConstructorArgs of SynConstructorArgs
    | BindingReturnInfo of SynBindingReturnInfo
    | Expr of SynExpr
    | StaticOptimizationConstraint of SynStaticOptimizationConstraint
    | IndexerArg of SynIndexerArg
    | SimplePatAlternativeIdInfo of SynSimplePatAlternativeIdInfo
    | MatchClause of SynMatchClause
    | InterfaceImpl of SynInterfaceImpl
    | TypeDefn of SynTypeDefn
    | TypeDefnRepr of SynTypeDefnRepr
    | MemberDefn of SynMemberDefn
    | ExceptionDefn of SynExceptionDefn
    | ParsedHashDirective of ParsedHashDirective
    | AttributeList of SynAttributeList
    | Attribute of SynAttribute

[<Flags>]
type FSharpSyntaxTokenQueryFlags =
    | None =                0x00
    | IncludeComments =     0x01
    | IncludeWhitespace =   0x10
    | IncludeTrivia =       0x11

[<Struct;NoEquality;NoComparison>]
type FSharpSyntaxToken =

    member ParentNode: FSharpSyntaxNode

    member Span: TextSpan

    member IsKeyword: bool

    member IsIdentifier: bool

    member IsWhitespace: bool

    member IsComment: bool

    member IsComma: bool

    member IsString: bool

    member TryGetText: unit -> string option

    member TryGetNextToken: unit -> FSharpSyntaxToken option

and [<Sealed>] FSharpSyntaxNode =

    member Parent: FSharpSyntaxNode option

    member SyntaxTree: FSharpSyntaxTree

    member Kind: FSharpSyntaxNodeKind

    member Span: TextSpan

    member GetAncestors: unit -> FSharpSyntaxNode seq

    member GetAncestorsAndSelf: unit -> FSharpSyntaxNode seq

    member GetDescendantTokens: ?tokenQueryFlags: FSharpSyntaxTokenQueryFlags -> FSharpSyntaxToken seq

    /// Get tokens whose parent is the current node.
    member GetChildTokens: ?tokenQueryFlags: FSharpSyntaxTokenQueryFlags -> FSharpSyntaxToken seq

    member GetDescendants: ?span: TextSpan -> FSharpSyntaxNode seq

    /// Get nodes whose parent is the current node.
    member GetChildren: ?span: TextSpan -> FSharpSyntaxNode seq

    member GetRoot: unit -> FSharpSyntaxNode

    member TryFindToken: position: int -> FSharpSyntaxToken option

    member TryFindNode: span: TextSpan -> FSharpSyntaxNode option

and [<Sealed>] FSharpSyntaxTree =

    member FilePath: string

    /// TODO: Make this public when we have a better way to handling ParsingInfo, perhaps have a better ParsingOptions?
    member internal ParsingConfig: ParsingConfig

    member internal GetParseResult: CancellationToken -> ParseResult

    member internal ConvertSpanToRange: TextSpan -> range

    /// Gets all the tokens by the given span.
    /// Does not require a full parse, therefore use this when you want lexical information without a full parse.
    member GetTokens: span: TextSpan * ?tokenQueryFlags: FSharpSyntaxTokenQueryFlags * ?ct: CancellationToken -> FSharpSyntaxToken seq

    /// Gets all the tokens.
    /// The same result as getting descendant tokens from the root node.
    /// Does not require a full parse, therefore use this when you want lexical information without a full parse.
    member GetTokens: ?tokenQueryFlags: FSharpSyntaxTokenQueryFlags * ?ct: CancellationToken -> FSharpSyntaxToken seq

    /// Get the root node.
    /// Does a full parse.
    member GetRootNode: CancellationToken -> FSharpSyntaxNode

    /// Get the text associated with the syntax tree.
    member GetText: CancellationToken -> SourceText

    /// Creates a new syntax tree with the given text snapshot.
    member WithChangedTextSnapshot: newTextSnapshot: FSharpSourceSnapshot -> FSharpSyntaxTree

    /// Get diagnostics.
    member GetDiagnostics: ?ct: CancellationToken -> ImmutableArray<Diagnostic>

    static member internal Create: filePath: string * ParsingConfig * FSharpSourceSnapshot -> FSharpSyntaxTree
