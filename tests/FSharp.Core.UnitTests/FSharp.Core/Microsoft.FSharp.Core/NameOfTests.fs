// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Core.Unittests
open System
open NUnit.Framework

exception ABC

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
    member this.``local function parameter name`` () =
        let myFunction parameter1 = nameof parameter1

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
    member this.``namespace name`` () =
        let b = nameof(FSharp.Core)
        Assert.AreEqual("Core",b)

    [<Test>]
    member this.``module name`` () =
        let b = nameof(FSharp.Core.Operators)
        Assert.AreEqual("Operators",b)

    [<Test>]
    member this.``exception name`` () =
        let b = nameof(ABC)
        Assert.AreEqual("ABC",b)

    [<Test>]
    member this.``nested type name 1`` () =
        let b = nameof(System.Collections.Generic.List.Enumerator<_>)
        Assert.AreEqual("Enumerator",b)

    [<Test>]
    member this.``type name 2`` () =
        let b = nameof(System.Action<_>)
        Assert.AreEqual("Action",b)

    [<Test>]
    member this.``member function which is defined below`` () =
        let b = nameof(this.MemberMethodDefinedBelow)
        Assert.AreEqual("MemberMethodDefinedBelow",b)

    member this.MemberMethodDefinedBelow(x,y) = x * y

    [<Test>]
    member this.``static member function name`` () =
        let b = nameof(BasicNameOfTests.StaticMethod)
        Assert.AreEqual("StaticMethod",b)

    [<Test>]
    member this.``class member lookup`` () =
        let b = nameof(localConstant)
        Assert.AreEqual("localConstant",b)

    [<Test>]
    member this.``member property name`` () =
        let b = nameof(this.MemberProperty)
        Assert.AreEqual("MemberProperty",b)

    [<Test>]
    member this.``static property name`` () =
        let b = nameof(BasicNameOfTests.StaticProperty)
        Assert.AreEqual("StaticProperty",b)

    member this.get_XYZ() = 1

    [<Test>]
    member this.``member method starting with get_`` () =
        let b = nameof(this.get_XYZ)
        Assert.AreEqual("get_XYZ",b)

    static member get_SXYZ() = 1

    [<Test>]
    member this.``static method starting with get_`` () =
        let b = nameof(BasicNameOfTests.get_SXYZ)
        Assert.AreEqual("get_SXYZ",b)

    [<Test>]
    member this.``nameof local property with encapsulated name`` () =
        let ``local property with encapsulated name and %.f`` = 0
        let b = nameof(``local property with encapsulated name and %.f``)
        Assert.AreEqual("local property with encapsulated name and %.f",b)

[<TestFixture>]
type MethodGroupTests() =
    member this.MethodGroup() = ()    
    member this.MethodGroup(i:int) = ()

    member this.MethodGroup1(i:int, f:float, s:string) = 0
    member this.MethodGroup1(f:float, l:int64) = "foo"
    member this.MethodGroup1(u:unit -> unit -> int, h: unit) : unit = ()

    [<Test>]
    member this.``single argument method group name lookup`` () =
        let b = nameof(this.MethodGroup)
        Assert.AreEqual("MethodGroup",b)

    [<Test>]
    member this.``multiple argument method group name lookup`` () =
        let b = nameof(this.MethodGroup1 : (float * int64 -> _))
        Assert.AreEqual("MethodGroup1",b)

[<TestFixture>]
type FrameworkMethodTests() =
    [<Test>]
    member this.``library function name`` () =
        let b = nameof(List.map)
        Assert.AreEqual("map",b)

    [<Test>]
    member this.``static class function name`` () =
        let b = nameof(System.Tuple.Create)
        Assert.AreEqual("Create",b)


type CustomUnionType =
| OptionA of string
| OptionB of int * string

[<TestFixture>]
type OperatorNameTests() =    

    [<Test>]
    member this.``lookup name of typeof operator`` () =
        let b = nameof(typeof<int>)
        Assert.AreEqual("typeof",b)

    [<Test>]
    member this.``lookup name of + operator`` () =
        let b = nameof(+)
        Assert.AreEqual("op_Addition",b)

    [<Test>]
    member this.``lookup name of |> operator`` () =
        let a = nameof(|>)
        Assert.AreEqual("op_PipeRight",a)
        let b = nameof(op_PipeRight)
        Assert.AreEqual("op_PipeRight",b)

    [<Test>]
    member this.``lookup name of nameof operator`` () =
        let b = nameof(nameof)
        Assert.AreEqual("nameof",b)

[<TestFixture>]
type PatternMatchingOfOperatorNameTests() =    
    member this.Method1(i:int) = ()

    [<Test>]
    member this.``use it as a match case guard`` () =
        match "Method1" with
        | x when x = nameof(this.Method1) -> ()
        | _ ->  Assert.Fail("not expected")

[<TestFixture>]
type NameOfOperatorInQuotations() =        
    [<Test>]
    member this.``use it in a quotation`` () =
        let q =
            <@ 
                let f(x:int) = nameof x
                f 20
            @>
        ()

[<TestFixture>]
type NameOfOperatorForGenerics() =
    [<Test>]
    member this.``use it in a generic function`` () =
        let fullyGeneric x = x
        let b = nameof(fullyGeneric)
        Assert.AreEqual("fullyGeneric",b)

    [<Test>]
    member this.``lookup name of a generic class`` () =
        let b = nameof System.Collections.Generic.List<int>
        Assert.AreEqual("List",b)

[<TestFixture>]
type UserDefinedNameOfTests() =
    [<Test>]
    member this.``userdefined nameof should shadow the operator`` () =        
        let nameof x = "test" + x.ToString()

        let y = nameof 1
        Assert.AreEqual("test1",y)

type Person = 
    { Name : string
      Age : int }
    member __.Update(fld : string, value : obj) = 
        match fld with
        | x when x = nameof __.Name -> { __ with Name = string value }
        | x when x = nameof __.Age -> { __ with Age = value :?> int }
        | _ -> __

