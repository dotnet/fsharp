module FSharp.Compiler.Service.Tests.CompletionLambdasTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``DotCompletionInBrokenLambda`` () =
    let info =
        Checker.getCompletionInfo
            """1 |> id (fun x .{caret}> x)"""

    Assert.Equal(0, info.Items.Length)

[<Theory>]
[<InlineData("""1 |> id (fun)
let x = 1
x.{caret}""")>] // error prepended: 1 |> id (fun)
[<InlineData("""let x = 1
x.{caret}
1 |> id (fun)""")>] // error appended: 1 |> id (fun)
[<InlineData("""1 |> id (fun x > x)
let x = 1
x.{caret}""")>] // error prepended: 1 |> id (fun x > x)
[<InlineData("""let x = 1
x.{caret}
1 |> id (fun x > x)""")>] // error appended: 1 |> id (fun x > x)
[<InlineData("""1 |> id (fun x > )
let x = 1
x.{caret}""")>] // error prepended: 1 |> id (fun x > )
[<InlineData("""let x = 1
x.{caret}
1 |> id (fun x > )""")>] // error appended: 1 |> id (fun x > )
[<InlineData("""1 |> id (fun x -> )
let x = 1
x.{caret}""")>] // error prepended: 1 |> id (fun x -> )
[<InlineData("""let x = 1
x.{caret}
1 |> id (fun x -> )""")>] // error appended: 1 |> id (fun x -> )
let ``DotCompletionWithBrokenLambda`` (markedSource: string) =
    let info = Checker.getCompletionInfo markedSource

    assertHasItemWithNames [ "CompareTo" ] info
    assertHasNoItemsWithNames [ "Array" ] info

[<Fact>]
let ``LambdaExpression.WithoutClosing.Bug1346c`` () =
    let info =
        Checker.getCompletionInfo
            """let p4 = 
   let isPalindrome x = 
       let chars = (string_of_int x).ToCharArray()
       let len = chars.{caret}
       chars 
       |> Array.mapi (fun i c -> )"""

    assertHasItemWithNames [ "Length" ] info

[<Fact>]
let ``Identifier.InLambdaExpression`` () =
    let info =
        Checker.getCompletionInfo
            """let funcLambdaExp = fun (x:int)-> x.{caret}"""

    assertHasItemWithNames [ "ToString"; "Equals" ] info

[<Fact>]
let ``Identifier.AsFunctionName.UsingFunKeyword`` () =
    let info =
        Checker.getCompletionInfo
            """fun f4.{caret}  x -> x+1"""

    Assert.Equal(0, info.Items.Length)

[<Fact(Skip = "VS bug 69654 (aspirational): Ctrl-space scoping inside parens not yet matched by FCS; assertion preserved (b in scope, i out)")>]
let ``AutoComplete.Bug69654_0`` () =
    let info =
        Checker.getCompletionInfo
            """
let q =
    let a = 42
    let b = (fun i -> i) 43
    // i shows up in Ctrl-space list here, b does not
    ({caret}) // but in the parens, things are correct again
"""

    assertHasItemWithNames [ "b" ] info
    assertHasNoItemsWithNames [ "i" ] info
