namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module HatDesugaringTests =

    [<Test>]
    let ``Hat operator should be overloadable in infix context``() =
        CompilerAssert.CompileExeAndRun
            """
module X
open System

let (^) (x: int) (y: int) = x + y

if 1 ^ 2 <> 3 then failwith "expected result to be 3"
Console.WriteLine()
            """


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
    let ``Reverse slicing should not work with at symbol``() =
        CompilerAssert.ParseWithErrors
            """
module X
open System

let list = [1;2;3]
Console.WriteLine(list.[@1..])
            """
            [||]

