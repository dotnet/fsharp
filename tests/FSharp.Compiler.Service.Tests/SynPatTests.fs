module FSharp.Compiler.Service.Tests.SynPatTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open Xunit

type Parenthesization = Needed | Unneeded

module Parenthesization =
    let ofBool shouldParenthesize =
        if shouldParenthesize then Needed
        else Unneeded

let pats: obj array list =
    [
        [|[Needed]; "match () with () -> ()"|]
        [|[Needed]; "let (Lazy x) = lazy 1"|]
        [|[Unneeded; Unneeded]; "let ((Lazy x)) = lazy 1"|]
        [|[Needed; Unneeded]; "let (()) = ()"|]
        [|[Needed; Unneeded; Unneeded]; "let ((())) = ()"|]
    ]

// `expected` represents whether each parenthesized pattern, from the inside outward, requires its parentheses.
[<Theory; MemberData(nameof pats)>]
let shouldBeParenthesizedInContext (expected: Parenthesization list) src =
    let ast = getParseResults src

    let actual =
        ([], ast)
        ||> ParsedInput.fold (fun actual path node ->
            match node, path with
            | SyntaxNode.SynPat pat, SyntaxNode.SynPat(SynPat.Paren _) :: path ->
                Parenthesization.ofBool (SynPat.shouldBeParenthesizedInContext path pat) :: actual
            | _ -> actual)

    Assert.Equal<Parenthesization list>(expected, actual)