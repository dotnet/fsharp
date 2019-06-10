namespace FSharp.Compiler.Compilation

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

[<NoEquality;NoComparison;RequireQualifiedAccess>]
type FSharpSyntaxNodeKind =
    | ParsedInput of ParsedInput
    | ModuleOrNamespace of SynModuleOrNamespace
    | ModuleDecl of SynModuleDecl
    | LongIdentWithDots of LongIdentWithDots
    | Ident of Ident
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

[<Sealed>]
type FSharpSyntaxToken =

    member ParentNode: FSharpSyntaxNode

    member internal Range: Range.range

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

    member internal Range: range

    member Span: TextSpan

    member TryFindToken: position: int -> FSharpSyntaxToken option

    member TryFindNode: span: TextSpan -> FSharpSyntaxNode option

and [<Sealed>] FSharpSyntaxTree =

    internal new: filePath: string * ParsingConfig * FSharpSourceSnapshot * changes: IReadOnlyList<TextChangeRange> -> FSharpSyntaxTree

    member FilePath: string

    /// TODO: Make this public when we have a better way to handling ParsingInfo, perhaps have a better ParsingOptions?
    member internal ParsingConfig: ParsingConfig

    member internal GetParseResult: CancellationToken -> ParseResult

    member GetRootNode: CancellationToken -> FSharpSyntaxNode

    member GetText: CancellationToken -> SourceText

    member WithChangedTextSnapshot: newTextSnapshot: FSharpSourceSnapshot -> FSharpSyntaxTree
