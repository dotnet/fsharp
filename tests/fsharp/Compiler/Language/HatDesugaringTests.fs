namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module HatDesugaringTests =

    [<Test>]
    let ``Hat operator should be overloadable in infix context``() =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
module X
open System

let (^) (x: int) (y: int) = x + y

if 1 ^ 2 <> 3 then failwithf "expected result to be 3 but got %i" (1 ^ 2)
Console.WriteLine()
            """
    
    [<Test>]
    let ``Reverse slicing should work with overloaded infix hat``() =
        CompilerAssert.CompileExeAndRunWithOptions [| "--langversion:preview" |]
            """
module X
open System

let (^) (x: int) (y: int) = x + y
let result = [1;2].[^1..]
if result <> [1;2] then failwithf "expected result to be [1;2] but got %A" result
Console.WriteLine()
            """

    [<Test>]
    let ``At operator should not be usable in prefix context``() =
        CompilerAssert.ParseWithErrors
            """
module X

let x = @1
            """
            [|
                FSharpErrorSeverity.Error, 10, (4,9,4,10), "Unexpected infix operator in binding"
            |]

    [<Test>]
    let ``Hat operator should not be overloadable in prefix context``() = 
        CompilerAssert.ParseWithErrors
            """
module X
open System

let (~^) (x: int) (y:int) = x + y

Console.WriteLine(^1)
            """
            [|
                FSharpErrorSeverity.Error, 1208, (5,6,5,8), "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.";
                FSharpErrorSeverity.Error, 10, (7,20,7,21), "Unexpected integer literal in expression. Expected ')' or other token.";
                FSharpErrorSeverity.Error, 583, (7,18,7,19), "Unmatched '('";
            |]

    [<Test>]
    let ``Reverse slicing should not work with at symbol in 1st slice index``() =
        CompilerAssert.ParseWithErrors
            """
module X
open System

let list = [1;2;3]
Console.WriteLine(list.[@1..])
            """
            [|
                FSharpErrorSeverity.Error, 1208, (6,25,6,26), "Invalid prefix operator"
            |]

    [<Test>]
    let ``Reverse slicing should not work with at symbol in 2nd slice index``() =
        CompilerAssert.ParseWithErrors
            """
module X
open System

let list = [1;2;3]
Console.WriteLine(list.[..@1])
            """
            [|
                FSharpErrorSeverity.Error, 1208, (6,25,6,28), "Invalid prefix operator"
            |]

    [<Test>]
    let ``Reverse slicing should not work with at symbol in both slice index``() =
        CompilerAssert.ParseWithErrors
            """
module X
open System

let list = [1;2;3]
Console.WriteLine(list.[@1..@1])
            """
            [|
                FSharpErrorSeverity.Error, 1208, (6,25,6,26), "Invalid prefix operator";
                FSharpErrorSeverity.Error, 1208, (6,29,6,30), "Invalid prefix operator"
            |]

    [<Test>]
    let ``Reverse indexing should not work with at symbol``() =
        CompilerAssert.ParseWithErrors
            """
module X
open System

let list = [1;2;3]
Console.WriteLine(list.[@11])
            """
            [|
                FSharpErrorSeverity.Error, 1208, (6,25,6,26), "Invalid prefix operator"
            |]
