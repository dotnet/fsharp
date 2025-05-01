// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open Xunit
open System
open FSharp.Test


module ``String Tests`` =

    type CalcSum(x : int, y: int) = 
        let mutable x = x
        let mutable y = y
    
        member _.Sum () = x + y
   
        interface IFormattable with
            member x.ToString (format: string, _ : IFormatProvider) = 
                match format with
                | null | ""
                | "g" | "G" -> String.Format("X + Y = {0}", x.Sum())
                | "s" | "S" -> x.Sum().ToString() // Short form
                | _ -> invalidArg format "Format is wrong!"

        override x.ToString() = (x :> IFormattable).ToString(null, null)

    [<Fact>]
    let ``String of custom type``() =
        let calc = CalcSum(10, 20)
        Assert.areEqual (string calc) "X + Y = 30"

        let testDelegate = fun () ->
            printfn "%s" (calc.ToString())
            Console.WriteLine("{0:S}", calc)
            Console.Write("{0} {1} {2:D}", 10, 20, calc)
        let e = Assert.Throws<ArgumentException> testDelegate
        Assert.areEqual e.ParamName "D"

    // int32
    type Foo =
      |  A  =  1
      |  B  =  2

    [<Fact>]
    let ``String of int32 based enum``() =
        let a = Foo.A
        let r = a :> System.IFormattable

        Assert.areEqual (string a) (string r)

    // uint32
    type Foo2 =
        | A = 3u
        | B = 4u

    [<Fact>]
    let ``String of uint32 based enum``() =
        let a = Foo2.A
        let r = a :> System.IFormattable
        Assert.areEqual (string a) (string r)

    // char
    type Foo3 =
        | A = 'a'
        | B = 'b'

    [<Fact>]
    let ``String of char based enum``() =
        let a = Foo3.A
        let r = a :> System.IFormattable
        Assert.areEqual (string a) (string r)

    // int16
    type Foo4 =
        | A = 1s
        | B = 2s
        
    [<Fact>]
    let ``String of int16 based enum``() =
        let a = Foo4.A
        let r = a :> System.IFormattable
        Assert.areEqual (string a) (string r)

    // uint16
    type Foo5 =
        | A = 1us
        | B = 2us

    [<Fact>]
    let ``String of uint16 based enum``() =
        let a = Foo5.A
        let r = a :> System.IFormattable
        Assert.areEqual (string a) (string r)

    // sbyte
    type Foo6 =
        | A = 1y
        | B = 2y

    [<Fact>]
    let ``String of sbyte based enum``() =
        let a = Foo6.A
        let r = a :> System.IFormattable
        Assert.areEqual (string a) (string r)

    // byte
    type Foo7 =
        | A = 1uy
        | B = 2uy

    [<Fact>]
    let ``String of byte based enum``() =
        let a = Foo7.A
        let r = a :> System.IFormattable
        Assert.areEqual (string a) (string r)