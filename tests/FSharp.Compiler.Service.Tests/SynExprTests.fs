module FSharp.Compiler.EditorServices.Tests.SynExprTests

open System
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open NUnit.Framework

type Parenthesization = Needed | Unneeded

module Parenthesization =
    let ofBool shouldParenthesize =
        if shouldParenthesize then Needed
        else Unneeded

let exprs : obj array list =
    [
        [|Needed; "(1 + 2) * 3"|]
        [|Unneeded; "1 + (2 * 3)"|]
        [|Unneeded; "1 * (2 * 3)"|]
        [|Unneeded; "(1 * 2) * 3"|]
        [|Needed; "1 / (2 / 3)"|]
        [|Unneeded; "(1 / 2) / 3"|]
        [|Unneeded; "(printfn \"Hello, world.\")"|]
        [|Needed; "let (~-) x = x in id -(<@ 3 @>)"|]
        [|Unneeded; "let (~-) x = x in id (-(<@ 3 @>))"|]
        [|Unneeded; "(())"|]
        [|Unneeded; "(3)"|]
        [|Needed;
          "
          let x = (x
                + y)
          in x
          "
        |]
        [|Unneeded;
          "
          let x = (x
                 + y)
          in x
          "
        |]
        [|Needed;
          "
          async {
              return (
              1
              )
          }
          "
        |]
        [|Unneeded;
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
type String with
    // This is not a true polyfill, but it suffices for the .NET Framework target.
    member this.ReplaceLineEndings() = this.Replace("\r", "")
#endif

[<Theory; TestCaseSource(nameof exprs)>]
let ``SynExpr.shouldBeParenthesizedInContext`` (needsParens: Parenthesization) src =
    let ast = getParseResults src

    let getSourceLineStr =
        let lines = src.ReplaceLineEndings().Split '\n'
        Line.toZ >> Array.get lines

    let exprs =
        let exprs = ResizeArray()

        SyntaxTraversal.TraverseAll(
            ast,
            { new SyntaxVisitorBase<unit>() with
                member _.VisitExpr(path, _, defaultTraverse, expr) =
                    match path with
                    | SyntaxNode.SynExpr(SynExpr.Paren _) :: path ->
                        exprs.Add(Parenthesization.ofBool (SynExpr.shouldBeParenthesizedInContext getSourceLineStr path expr))
                    | _ -> ()

                    defaultTraverse expr
            })

        List.ofSeq exprs

    CollectionAssert.AreEqual(needsParens |> List.replicate exprs.Length, exprs)
