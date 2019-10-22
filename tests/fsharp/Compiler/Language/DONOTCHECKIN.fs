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

let arr = Array3D.create 2 2 2 0
Console.WriteLine(arr.Length - 1 - 2)
let arr2 = arr.[^1.. ,*,*]
Console.WriteLine(arr2)
            """


