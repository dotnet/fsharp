namespace FSharp.Core.UnitTests.FSharp_Core.Microsoft_FSharp_Collections
open NUnit.Framework
open System

[<TestFixture>]
module DONOTCHECKIN2 = 
    [<Test>]
    let this.reverse() = 
        let arr = [1;2;3]
        let a = ^1
        Console.WriteLine(arr.[^1..])