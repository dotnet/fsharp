// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Core.Unittests
open System
open NUnit.Framework

[<TestFixture>]
type BasicNameOfTests() =    
    let localConstant = 23
    member this.MemberMethod() = 0 
    member this.MemberProperty = this.MemberMethod()
    static member StaticMethod() = 0 
    static member StaticProperty = BasicNameOfTests.StaticMethod()
        
    [<Test>]
    member this.``local variable name lookup`` () =
        let a = 0
        let result = nameof a
        Assert.AreEqual("a",result)
        Assert.AreEqual("result",nameof result)

    [<Test>]
    member this.``local int function name`` () =
        let myFunction x = 0 * x
        let b = nameof myFunction
        Assert.AreEqual("myFunction",b)

    [<Test>]
    member this.``local curried function name`` () =
        let curriedFunction x y = x * y
        let b = nameof curriedFunction
        Assert.AreEqual("curriedFunction",b)

    [<Test>]
    member this.``local tupled function name`` () =
        let tupledFunction(x,y) = x * y
        let b = nameof tupledFunction
        Assert.AreEqual("tupledFunction",b)

    [<Test>]
    member this.``local unit function name`` () =
        let myFunction() = 1
        let b = nameof(myFunction)
        Assert.AreEqual("myFunction",b)

    [<Test>]
    member this.``local function paarameter name`` () =
        let myFunction parameter1 = nameof parameter1

        Assert.AreEqual("parameter1",myFunction "x")
        Assert.AreEqual("parameter1",myFunction "x")

    [<Test>]
    member this.``can get name from inside a local function (needs to be let rec)`` () =
        let rec myLocalFunction x = 
            let z = 2 * x
            nameof myLocalFunction + " " + z.ToString()
            
        Assert.AreEqual("myLocalFunction 46",myLocalFunction 23)
        Assert.AreEqual("myLocalFunction 50",myLocalFunction 25)

    [<Test>]
    member this.CanGetNameFromInsideAMember () =
        let b = nameof(this.CanGetNameFromInsideAMember)
        Assert.AreEqual("CanGetNameFromInsideAMember",b)

    [<Test>]
    member this.``member function name`` () =
        let b = nameof(this.MemberMethod)
        Assert.AreEqual("MemberMethod",b)

    [<Test>]
    member this.``static member function name`` () =
        let b = nameof(BasicNameOfTests.StaticMethod)
        Assert.AreEqual("StaticMethod",b)

    [<Test>]
    member this.``library function name`` () =
        let b = nameof(List.map)
        Assert.AreEqual("Map",b)

    [<Test>]
    member this.``static class function name`` () =
        let b = nameof(Tuple.Create)
        Assert.AreEqual("Create",b)

    [<Test>]
    member this.``class member lookup`` () =
        let b = nameof(localConstant)
        Assert.AreEqual("localConstant",b)

    [<Test>]
    member this.``member property name`` () =
        let b = nameof(this.MemberProperty)
        Assert.AreEqual("get_MemberProperty",b)

    [<Test>]
    member this.``static property name`` () =
        let b = nameof(BasicNameOfTests.StaticProperty)
        Assert.AreEqual("get_StaticProperty",b)

    [<Test>]
    member this.``nameof local property with encapsulated name`` () =
        let ``local property with encapsulated name and %.f`` = 0
        let b = nameof(``local property with encapsulated name and %.f``)
        Assert.AreEqual("local property with encapsulated name and %.f",b)

    member this.MethodGroup() = ()    
    member this.MethodGroup(i:int) = ()

    [<Test>]
    member this.``method group name lookup`` () =
        let b = nameof(this.MethodGroup)
        Assert.AreEqual("MethodGroup",b)