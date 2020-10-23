//<Expects id="FS0039" status="error" span="(100,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(102,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(107,22)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(112,22)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(117,22)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(122,22)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(126,37)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(133,13)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(142,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(146,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(150,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(154,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(158,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(162,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(166,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(170,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(174,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(179,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(185,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(189,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(193,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(200,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(206,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(211,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(223,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(227,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(232,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(236,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(251,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(256,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(258,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(262,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(266,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0267" status="error" span="(271,33)">This is not a valid constant expression or custom attribute value</Expects>
//<Expects id="FS0267" status="error" span="(271,23)">This is not a valid constant expression or custom attribute value</Expects>
//<Expects id="FS0039" status="error" span="(271,33)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0267" status="error" span="(271,33)">This is not a valid constant expression or custom attribute value</Expects>
//<Expects id="FS0267" status="error" span="(271,23)">This is not a valid constant expression or custom attribute value</Expects>
//<Expects id="FS0039" status="error" span="(281,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(285,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(289,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(291,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(295,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(303,22)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(310,32)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(318,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(322,17)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(336,22)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(337,22)">The value or constructor 'nameof' is not defined.</Expects>
//<Expects id="FS0039" status="error" span="(342,13)">The value or constructor 'name' is not defined. Maybe you want one of the following:</Expects>


#if TESTS_AS_APP
module TestSuite_FSharpCore_nameof_46
#endif

#nowarn "44"

open System

exception ABC

type Assert =
    static member AreEqual (a, b) = a=b
    static member Fail = raise (Exception("Fail"))

let failures = ref []

let report_failure (s : string) = 
    failures := !failures @ [s]

let test (s : string) b =
    if b then stderr.Write " Success "
    else
        stderr.Write " Failed  "
        report_failure (s)
    stderr.WriteLine(s)

let check s b1 b2 = test s (b1 = b2)

type BasicNameOfTests() =

    static let staticConstant = 23
    let localConstant = 23

    member this.MemberMethod() = 0
    member this.MemberProperty = this.MemberMethod()

    member this.``member method``() = 0
    member this.``member property`` = this.``member method``()

    static member StaticMethod() = 0
    static member StaticProperty = BasicNameOfTests.StaticMethod()

    static member ``static method``() = 0
    static member ``static property`` = BasicNameOfTests.``static method``()

    static member ``local variable name lookup`` () =
        let a = 0
        let b = nameof a
        let result = Assert.AreEqual("a", b)
        let c = nameof b
        result || Assert.AreEqual("b", c)

    static member ``local int function name`` () =
        let myFunction x = 0 * x
        let result = nameof myFunction
        Assert.AreEqual("myFunction", result)

    static member ``local curried function name`` () =
        let curriedFunction x y = x * y
        let result = nameof curriedFunction
        Assert.AreEqual("curriedFunction", result)

    static member ``local tupled function name`` () =
        let tupledFunction(x,y) = x * y
        let result = nameof tupledFunction
        Assert.AreEqual("tupledFunction", result)

    static member ``local unit function name`` () =
        let myFunction() = 1
        let result = nameof(myFunction)
        Assert.AreEqual("myFunction", result)

    static member ``local function parameter name`` () =
        let myFunction parameter1 = nameof parameter1
        let result = myFunction "x"
        Assert.AreEqual("parameter1", result)

    static member ``can get name from inside a local function (needs to be let rec)`` () =
        let rec myLocalFunction x = 
            let z = 2 * x
            nameof myLocalFunction + " " + z.ToString()

        let a = myLocalFunction 23
        let result = Assert.AreEqual("myLocalFunction 46", a)

        let b = myLocalFunction 25
        result || Assert.AreEqual("myLocalFunction 50", b)

    static member ``can get name from inside static member`` () =
        let b = nameof(BasicNameOfTests.``can get name from inside static member``)
        Assert.AreEqual("can get name from inside static member", b)

    member this.``can get name from inside instance member`` () =
        let b = nameof(this.``can get name from inside instance member``)
        Assert.AreEqual("can get name from inside instance member", b)

    static member ``can get name of static member`` () =
        let b = nameof(BasicNameOfTests.``can get name of static member``)
        Assert.AreEqual("can get name of static member", b)

    member this.``can get name of instance member`` () =
        let b = nameof(this.MemberMethod)
        Assert.AreEqual("MemberMethod", b)

    static member ``namespace name`` () =
        let b = nameof(FSharp.Core)
        Assert.AreEqual("Core",b)

    static member ``module name`` () =
        let b = nameof(FSharp.Core.Operators)
        Assert.AreEqual("Operators",b)

    static member ``exception name`` () =
        let b = nameof(ABC)
        Assert.AreEqual("ABC",b)

    static member ``nested type name 1`` () =
        let b = nameof(System.Collections.Generic.List.Enumerator<_>)
        Assert.AreEqual("Enumerator",b)

    static member ``type name 2`` () =
        let b = nameof(System.Action<_>)
        Assert.AreEqual("Action",b)

    static member ``member function which is defined below`` () =
        let x = BasicNameOfTests()
        let b = nameof(x.MemberMethodDefinedBelow)
        Assert.AreEqual("MemberMethodDefinedBelow",b)

    member this.MemberMethodDefinedBelow(x,y) = x * y

    static member ``static member function name`` () =
        let b = nameof(BasicNameOfTests.StaticMethod)
        Assert.AreEqual("StaticMethod",b)

    member this.``class member lookup`` () =
        let b = nameof(localConstant)
        Assert.AreEqual("localConstant",b)

    static member ``static property name`` () =
        let b = nameof(BasicNameOfTests.StaticProperty)
        Assert.AreEqual("StaticProperty",b)

    member this.get_XYZ() = 1

    static member ``member method starting with get_`` () =
        let x = BasicNameOfTests()
        let b = nameof(x.get_XYZ)
        Assert.AreEqual("get_XYZ",b)

    static member get_SXYZ() = 1

    static member ``static method starting with get_`` () =
        let b = nameof(BasicNameOfTests.get_SXYZ)
        Assert.AreEqual("get_SXYZ",b)

    static member ``nameof local property with encapsulated name`` () =
        let ``local property with encapsulated name and %.f`` = 0
        let b = nameof(``local property with encapsulated name and %.f``)
        Assert.AreEqual("local property with encapsulated name and %.f",b)

type MethodGroupNameOfTests() =
    member this.MethodGroup() = ()    
    member this.MethodGroup(i:int) = ()

    member this.MethodGroup1(i:int, f:float, s:string) = 0
    member this.MethodGroup1(f:float, l:int64) = "foo"
    member this.MethodGroup1(u:unit -> unit -> int, h: unit) : unit = ()

    member this.``single argument method group name lookup`` () =
        let b = nameof(this.MethodGroup)
        Assert.AreEqual("MethodGroup",b)

    member this.``multiple argument method group name lookup`` () =
        let b = nameof(this.MethodGroup1 : (float * int64 -> _))
        Assert.AreEqual("MethodGroup1",b)

type FrameworkMethodTests() =
    member this.``library function name`` () =
        let b = nameof(List.map)
        Assert.AreEqual("map",b)

    member this.``static class function name`` () =
        let b = nameof(System.Tuple.Create)
        Assert.AreEqual("Create",b)

type CustomUnionType =
    | OptionA 
    | OptionB of int * string

type CustomRecordType =
    { X: int; Y: int }

[<Measure>] type Milliquacks

type UnionAndRecordNameOfTests() =

    member this.``measure 1`` () =
        let b = nameof(Milliquacks)
        Assert.AreEqual("Milliquacks",b)

    member this.``record case 1`` () =
        let sample = Unchecked.defaultof<CustomRecordType>
        let b = nameof(sample.X)
        let result = Assert.AreEqual("X",b)
        let b = nameof(sample.Y)
        result || Assert.AreEqual("Y",b)

    member this.``union case 1`` () =
        let b = nameof(OptionA)
        Assert.AreEqual("OptionA",b)

    member this.``union case 2`` () =
        let b = nameof(OptionB)
        Assert.AreEqual("OptionB",b)

type AttributeNameOfTests() =

    [<System.Obsolete("test " + nameof(string))>]
    member this.``ok in attribute`` () =
        let t = typeof<AttributeNameOfTests>.GetMethod("ok in attribute")
        let attrs = t.GetCustomAttributes(typeof<ObsoleteAttribute>, false)
        let attr = attrs.[0] :?> ObsoleteAttribute
        Assert.AreEqual(attr.Message, "test string")

type OperatorNameOfTests() =    

    member this.``lookup name of typeof operator`` () =
        let b = nameof(typeof<int>)
        Assert.AreEqual("typeof",b)

    member this.``lookup name of + operator`` () =
        let b = nameof(+)
        Assert.AreEqual("op_Addition",b)

    member this.``lookup name of |> operator`` () =
        let a = nameof(|>)
        let result = Assert.AreEqual("op_PipeRight",a)
        let b = nameof(op_PipeRight)
        result || Assert.AreEqual("op_PipeRight",b)

    member this.``lookup name of nameof operator`` () =
        let b = nameof(nameof)
        Assert.AreEqual("nameof",b)

type PatternMatchingOfOperatorNameTests() =
    member this.Method1(i:int) = ()

    member this.``use it as a match case guard`` () =
        match "Method1" with
        | x when x = nameof(this.Method1) -> true
        | _ ->  false

type NameOfOperatorInQuotations() =
    member this.``use it in a quotation`` () =
        let q =
            <@ 
                let f(x:int) = nameof x
                f 20
            @>
        true

type NameOfOperatorForGenerics() =
    member this.``use it in a generic function`` () =
        let fullyGeneric x = x
        let b = nameof(fullyGeneric)
        Assert.AreEqual("fullyGeneric",b)

    member this.``lookup name of a generic class`` () =
        let b = nameof System.Collections.Generic.List<int>
        Assert.AreEqual("List",b)

type UserDefinedNameOfTests() =
    static member ``user defined nameof should shadow the operator`` () =
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

module Foo =
    let nameof = ()
    let x = name ()

