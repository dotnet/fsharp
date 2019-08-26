namespace rec FSharp.Compiler.Compilation

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

[<Struct;NoEquality;NoComparison>]
type FSharpSyntaxToken =

    member IsNone: bool

    /// Gets the parent node that best owns the token.
    /// Does a full parse of the syntax tree if neccessary.
    /// This means having a token does not necessarily mean that the syntax tree has actually been parsed.
    /// Throws an exception if the token is None.
    member GetParentNode: ?ct: CancellationToken -> FSharpSyntaxNode

    member Span: TextSpan

    member IsKeyword: bool

    member IsIdentifier: bool

    member IsString: bool

    member Value: obj voption

    member ValueText: string voption

    static member None: FSharpSyntaxToken

[<Class>]
type FSharpSyntaxNode =

    member Parent: FSharpSyntaxNode option

    member SyntaxTree: FSharpSyntaxTree

    member Span: TextSpan

    member GetAncestors: unit -> FSharpSyntaxNode seq

    member GetAncestorsAndSelf: unit -> FSharpSyntaxNode seq

    member TryFirstAncestorOrSelf: ('TNode -> bool) -> 'TNode option when 'TNode :> FSharpSyntaxNode

    member TryFirstAncestorOrSelf: unit -> 'TNode option when 'TNode :> FSharpSyntaxNode

    member GetDescendantTokens: unit -> FSharpSyntaxToken seq

    /// Get tokens whose parent is the current node.
    member GetChildTokens: unit -> FSharpSyntaxToken seq

    member GetDescendants: span: TextSpan -> FSharpSyntaxNode seq

    member GetDescendants: unit -> FSharpSyntaxNode seq

    /// Get nodes whose parent is the current node.
    member GetChildren: span: TextSpan -> FSharpSyntaxNode seq

    member GetChildren: unit -> FSharpSyntaxNode seq

    member GetRoot: unit -> FSharpSyntaxNode

    member FindToken: position: int -> FSharpSyntaxToken

    member TryFindNode: span: TextSpan -> FSharpSyntaxNode option

type [<Sealed>] FSharpSyntaxTree =

    /// The file that was parsed to form a syntax tree.
    /// Will be empty if there is no file associated with the syntax tree.
    /// Will never be null.
    member FilePath: string

    member internal Source: FSharpSource

    /// TODO: Make this public when we have a better way to handling ParsingInfo, perhaps have a better ParsingOptions?
    member internal ParsingConfig: ParsingConfig

    member internal GetParseResult: CancellationToken -> ParseResult

    member internal ConvertSpanToRange: TextSpan -> range

    /// Gets all the tokens by the given span.
    /// Does not require a full parse, therefore use this when you want lexical information without a full parse.
    member GetTokens: span: TextSpan * ?ct: CancellationToken -> FSharpSyntaxToken seq

    /// Gets all the tokens.
    /// The same result as getting descendant tokens from the root node.
    /// Does not require a full parse, therefore use this when you want lexical information without a full parse.
    member GetTokens: ?ct: CancellationToken -> FSharpSyntaxToken seq

    /// Get the root node.
    /// Does a full parse.
    member GetRootNode: ?ct: CancellationToken -> FSharpSyntaxNode

    /// Get the text associated with the syntax tree.
    member GetText: ?ct: CancellationToken -> SourceText

    /// Creates a new syntax tree with the given source.
    member WithChangedSource: newSrc: FSharpSource -> FSharpSyntaxTree

    /// Get diagnostics.
    member GetDiagnostics: ?ct: CancellationToken -> ImmutableArray<Diagnostic>

    static member internal Create: ParsingConfig * FSharpSource -> FSharpSyntaxTree

// ------------------------------------------------------------------------
//
// Syntax Nodes
//
// ------------------------------------------------------------------------

[<Class>]
type ExpressionSyntax =
    inherit FSharpSyntaxNode

    member internal Green: SynExpr