module FSharp.Compiler.Service.Tests.SyntaxTreeOpsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open Xunit

let private exprOf source =
    match getParseResults source with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [ SynModuleOrNamespace(decls = [ SynModuleDecl.Expr(expr = expr) ]) ])) -> expr
    | tree -> failwith $"Expected a single expression, got %A{tree}"

[<Theory>]
[<InlineData("x.P")>]
[<InlineData("x.A.B")>]
[<InlineData("x.M(y)")>]
[<InlineData("x.Xs[0]")>]
[<InlineData("x.Xs.[0]")>]
[<InlineData("x.M<int>()")>]
[<InlineData("x.M().P")>]
let ``tryPopUnaryArg splits the root off a chain that pushUnaryArg restores`` (source: string) =
    let body = exprOf source

    match tryPopUnaryArg body with
    | ValueSome(struct (root, popped)) ->
        Assert.Equal("x", root.idText)
        Assert.Equal(sprintf "%A" body, sprintf "%A" (pushUnaryArg popped root))
    | ValueNone -> Assert.Fail $"Expected {source} to have a root"

[<Theory>]
[<InlineData("f x.P")>]
[<InlineData("x.M y")>]
[<InlineData("x.P + 1")>]
[<InlineData("x")>]
[<InlineData("x[0]")>]
[<InlineData("(x.P)")>]
[<InlineData("x.")>]
let ``tryPopUnaryArg rejects what _. cannot express`` (source: string) =
    Assert.True((tryPopUnaryArg (exprOf source)).IsNone)
