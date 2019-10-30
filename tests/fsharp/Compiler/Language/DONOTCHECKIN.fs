namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module DONOTCHECKIN = 
    [<Test>]
    let testReverseList() =
        CompilerAssert.CompileExeAndRun
            """
open System

let lst = [1;2;3]
let arr2 = lst.[^1..]
Console.WriteLine(arr2)
            """

    [<Test>]
    let testReverse2d() =
        CompilerAssert.CompileExeAndRun
            """
open System

let arr = array2D [ [1;2;3]; [3;2;1]]
let arr2 = arr.[1, ^1]
Console.WriteLine(arr2)
            """
 
    [<Test>]
    let testReverse3d() =
        CompilerAssert.CompileExeAndRun
            """
open System

let arr = Array3D.create 2 2 2 2 
let arr2 = arr.[1..^1, ..^1, ^0]
Console.WriteLine(arr2)
            """
    [<Test>]
    let testSyn() =
        CompilerAssert.CompileExeAndRun
            """
open System

let arr = [|1;2|]
Console.WriteLine(Array.get arr 1)
            """


    [<Test>]
    let testReverse() =
        CompilerAssert.CompileExeAndRun
            """
open System

let arr = [|1;2;3|]
let arr2 = arr.[1..^1]
Console.WriteLine(arr2)
            """


