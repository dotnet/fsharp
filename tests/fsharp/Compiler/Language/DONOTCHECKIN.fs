namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module DONOTCHECKIN = 
    [<Test>]
    let testReverse3d() =
        CompilerAssert.CompileExeAndRun
            """
open System

let arr = Array3D.create 2 2 2 2 
let arr2 = arr.[1..^1, ..^1, *]
Console.WriteLine(arr2)
            """



    [<Test>]
    let testReverse() =
        CompilerAssert.CompileExeAndRun
            """
open System

let arr = [|1;2;3|]
let arr2 = arr.[..^1]
Console.WriteLine(arr2)
            """


