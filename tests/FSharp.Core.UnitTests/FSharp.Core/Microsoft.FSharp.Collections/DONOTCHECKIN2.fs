namespace FSharp.Core.UnitTests.FSharp_Core.Microsoft_FSharp_Collections
open NUnit.Framework
open System
module DONOTCHECKIN2

[<TestFixture>]
type DONOTCHECKIN2 = 
    [<Test>]
    member this.reverse() = 
        let arr = [1;2;3]
        let a = ^1
        Console.WriteLine(arr.[^1..])