module FSharp.Compiler.Service.Tests.CompletionLiteralsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``Literal.809979`` () =
    let info =
        Checker.getCompletionInfo """let value=uint64.{caret}"""

    assertHasNoItemsWithNames [ "Parse" ] info

[<Fact>]
let ``CharLiteral`` () =
    let info =
        Checker.getCompletionInfo
            """let x = "foo"
let x' = "bar"
x'.{caret}"""

    assertHasItemWithNames [ "CompareTo"; "GetHashCode" ] info

[<Fact>]
let ``Literal.Float`` () =
    let info =
        Checker.getCompletionInfo """let myfloat = (42.0).{caret}"""

    assertHasItemWithNames [ "GetType"; "ToString" ] info

[<Fact>]
let ``Literal.String`` () =
    let info =
        Checker.getCompletionInfo """let name = "foo".{caret}"""

    assertHasItemWithNames [ "Chars"; "Clone" ] info

[<Fact>]
let ``Literal.Int`` () =
    let info =
        Checker.getCompletionInfo """let typeint = (10).{caret}"""

    assertHasItemWithNames [ "GetType"; "ToString" ] info
