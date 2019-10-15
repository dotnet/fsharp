namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module DONOTCHECKIN = 

    [<Test>]
    let testReverse() =
        CompilerAssert.CompileExeAndRun
            """
open System

let arr = [1;2;3]
//Console.WriteLine(arr.Length - 1)
Console.WriteLine(arr.[^1..])
            """


