namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module HatDesugaringTests =

    [<Test>]
    let ``Hat operator should be overloadable in infix context``() =
        CompilerAssert.CompileExeAndRun
            """
open System

let (^) (x: int) (y: int) = x + y

if 1 ^ 2 <> 3 then failwith "expected result to be 3"
            """


    [<Test>]
    let ``Hat operator should not be overloadable in prefix context``() = 
        CompilerAssert.ParseWithErrors
            """
open System

let (^) (x: int) (y:int) = x + y

Console.WriteLine(^1)
            """
            [|
            |]

