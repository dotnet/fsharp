module FSharp.Compiler.Service.Tests.SynExprTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open Xunit

type Parenthesization = Needed | Unneeded

module Parenthesization =
    let ofBool shouldParenthesize =
        if shouldParenthesize then Needed
        else Unneeded

let exprs: obj array list =
    [
        [|([] : Parenthesization list); "()"|]
        [|[Needed]; "(1 + 2) * 3"|]
        [|[Unneeded]; "1 + (2 * 3)"|]
        [|[Unneeded]; "1 * (2 * 3)"|]
        [|[Unneeded]; "(1 * 2) * 3"|]
        [|[Needed]; "1 / (2 / 3)"|]
        [|[Unneeded]; "(1 / 2) / 3"|]
        [|[Unneeded]; "(printfn \"Hello, world.\")"|]
        [|[Needed]; "let (~-) x = x in id -(<@ 3 @>)"|]
        [|[Unneeded; Unneeded]; "let (~-) x = x in id (-(<@ 3 @>))"|]
        [|[Unneeded]; "(())"|]
        [|[Unneeded]; "(3)"|]
        [|[Needed];
          "
          let x = (x
                + y)
          in x
          "
        |]
        [|[Unneeded];
          "
          let x = (x
                 + y)
          in x
          "
        |]
        [|[Needed];
          "
          async {
              return (
              1
              )
          }
          "
        |]
        [|[Unneeded];
          "
          async {
              return (
               1
              )
          }
          "
        |]
    ]

#if !NET6_0_OR_GREATER
open System

type String with
    // This is not a true polyfill, but it suffices for the .NET Framework target.
    member this.ReplaceLineEndings() = this.Replace("\r", "")
#endif

// `expected` represents whether each parenthesized expression, from the inside outward, requires its parentheses.
[<Theory; MemberData(nameof exprs)>]
let shouldBeParenthesizedInContext (expected: Parenthesization list) src =
    let ast = getParseResults src

    let getSourceLineStr =
        let lines = src.ReplaceLineEndings().Split '\n'
        Line.toZ >> Array.get lines

    let actual =
        ([], ast)
        ||> ParsedInput.fold (fun actual path node ->
            match node, path with
            | SyntaxNode.SynExpr expr, SyntaxNode.SynExpr(SynExpr.Paren _) :: path ->
                Parenthesization.ofBool (SynExpr.shouldBeParenthesizedInContext getSourceLineStr path expr) :: actual
            | _ -> actual)

    Assert.Equal<Parenthesization list>(expected, actual)

[<Theory>]
[<InlineData("9")>]
[<InlineData("9 |> ignore")>]
[<InlineData("
let x =
    do ()
    9
")>]
[<InlineData("
let x =
    do ()
    9 |> ignore
")>]
[<InlineData("
for x in 1..10 do
    9 |> ignore
")>]
let ``shouldBeParenthesizedInContext handles an unparenthesized hypothetical`` src =
    let ast = getParseResults src

    let getSourceLineStr =
        let lines = src.ReplaceLineEndings().Split '\n'
        Line.toZ >> Array.get lines

    let expr, path =
        (None, ast)
        ||> ParsedInput.foldWhile (fun acc path node ->
            match node with
            | SyntaxNode.SynExpr (SynExpr.Const(SynConst.Int32 9, _)  as expr) -> Some(Some(expr, path))
            | _ -> Some acc)
        |> Option.defaultWith (fun () -> invalidOp "Expected a 9 but did not find one.")

    Assert.False(SynExpr.shouldBeParenthesizedInContext getSourceLineStr path expr)
