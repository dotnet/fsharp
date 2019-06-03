namespace FSharp.Compiler.Compilation

open System.Collections.Immutable
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.CompileOps
open FSharp.Compiler
open FSharp.Compiler.Ast

[<Sealed>]
type SourceSnapshot =

    member FilePath: string

[<NoEquality; NoComparison>]
type internal ParsingConfig =
    {
        tcConfig: TcConfig
        isLastFileOrScript: bool
        isExecutable: bool
        conditionalCompilationDefines: string list
        filePath: string
    }

[<Sealed;AbstractClass;Extension>]
type internal ITemporaryStorageServiceExtensions =

    [<Extension>]
    static member CreateSourceSnapshot: ITemporaryStorageService * filePath: string * SourceText -> Cancellable<SourceSnapshot>

    [<Extension>]
    static member CreateSourceSnapshot: ITemporaryStorageService * filePath: string -> Cancellable<SourceSnapshot>

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type SyntaxNodeKind =
    | Expr of SynExpr
    | ModuleDecl of SynModuleDecl
    | Binding of SynBinding
    | ComponentInfo of SynComponentInfo
    | HashDirective of Range.range
    | ImplicitInherit of SynType * SynExpr * Range.range
    | InheritSynMemberDefn of SynComponentInfo * SynTypeDefnKind * SynType * SynMemberDefns * Range.range
    | InterfaceSynMemberDefnType of SynType
    | LetOrUse of SynBinding list * Range.range
    | MatchClause of SynMatchClause
    | ModuleOrNamespace of SynModuleOrNamespace
    | Pat of SynPat
    | RecordField of SynExpr option * LongIdentWithDots option
    | SimplePats of SynSimplePat list
    | Type of SynType
    | TypeAbbrev of SynType * Range.range

[<Sealed>]
type SyntaxNode =
    
    member Kind : SyntaxNodeKind

[<Sealed>]
type SyntaxTree =

    internal new: filePath: string * ParsingConfig * SourceSnapshot -> SyntaxTree

    member FilePath: string

    /// TODO: Make this public when we have a better way to handling ParsingInfo, perhaps have a better ParsingOptions?
    member internal ParsingConfig: ParsingConfig

    member GetParseResultAsync: unit -> Async<ParseResult>

    member GetSourceTextAsync: unit -> Async<SourceText>

    member TryFindNodeAsync: line: int * column: int -> Async<SyntaxNode option>

    //member GetTokensAsync: line: int -> Async<ImmutableArray<FSharpTokenInfo>>

    //member TryGetTokenAsync: line: int * column: int -> Async<FSharpTokenInfo option>