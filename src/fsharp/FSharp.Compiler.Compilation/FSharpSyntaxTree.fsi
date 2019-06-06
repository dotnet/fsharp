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

[<NoEquality; NoComparison>]
type internal ParsingConfig =
    {
        tcConfig: TcConfig
        isLastFileOrScript: bool
        isExecutable: bool
        conditionalCompilationDefines: string list
        filePath: string
    }

[<Sealed>]
type FSharpSyntaxToken =

    member internal Token: Parser.token

    member Range: Range.range

[<Sealed>]
type FSharpSyntaxTree =

    internal new: filePath: string * ParsingConfig * FSharpSourceSnapshot -> FSharpSyntaxTree

    member FilePath: string

    /// TODO: Make this public when we have a better way to handling ParsingInfo, perhaps have a better ParsingOptions?
    member internal ParsingConfig: ParsingConfig

    member GetParseResultAsync: unit -> Async<ParseResult>

    member GetSourceTextAsync: unit -> Async<SourceText>

    member TryFindTokenAsync: line: int * column: int -> Async<FSharpSyntaxToken option>

   // member TryFindNodeAsync: line: int * column: int -> Async<SyntaxNode option>

    //member GetTokensAsync: line: int -> Async<ImmutableArray<FSharpTokenInfo>>

    //member TryGetTokenAsync: line: int * column: int -> Async<FSharpTokenInfo option>