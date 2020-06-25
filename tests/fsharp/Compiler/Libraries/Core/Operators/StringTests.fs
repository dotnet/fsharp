// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module ``String Tests`` =

    // int32
    type Foo =
      |  A  =  1
      |  B  =  2

    [<Test>]
    let ``String of int32 based enum``() =
        let a = Foo.A
        let r = a :> System.IFormattable

        Assert.areEqual (string a) (string r)

    // uint32
    type Foo2 =
        | A = 3u
        | B = 4u

    [<Test>]
    let ``String of uint32 based enum``() =
        let a = Foo2.A
        let r = a :> System.IFormattable
        Assert.areEqual (string a) (string r)

    // char
    type Foo3 =
        | A = 'a'
        | B = 'b'

    [<Test>]
    let ``String of char based enum``() =
        let a = Foo3.A
        let r = a :> System.IFormattable
        Assert.areEqual (string a) (string r)

    // int16
    type Foo4 =
        | A = 1s
        | B = 2s
        
    [<Test>]
    let ``String of int16 based enum``() =
        let a = Foo4.A
        let r = a :> System.IFormattable
        Assert.areEqual (string a) (string r)

    // uint16
    type Foo5 =
        | A = 1us
        | B = 2us

    [<Test>]
    let ``String of uint16 based enum``() =
        let a = Foo5.A
        let r = a :> System.IFormattable
        Assert.areEqual (string a) (string r)

    // sbyte
    type Foo6 =
        | A = 1y
        | B = 2y

    [<Test>]
    let ``String of sbyte based enum``() =
        let a = Foo6.A
        let r = a :> System.IFormattable
        Assert.areEqual (string a) (string r)

    // byte
    type Foo7 =
        | A = 1uy
        | B = 2uy

    [<Test>]
    let ``String of byte based enum``() =
        let a = Foo7.A
        let r = a :> System.IFormattable
        Assert.areEqual (string a) (string r)