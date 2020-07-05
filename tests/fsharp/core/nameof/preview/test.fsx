//<Expects status="success" />
#if TESTS_AS_APP
module TestSuite_FSharpCore_nameof_47
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

    member this.``can get name of instance member via unchecked`` () =
        let b = nameof(Unchecked.defaultof<BasicNameOfTests>.MemberMethod)
        Assert.AreEqual("MemberMethod", b)

    member this.``can get name of method type parameter``<'TTT> () =
        let b = nameof<'TTT>
        Assert.AreEqual("TTT", b)

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

do test "local variable name lookup"                    (BasicNameOfTests.``local variable name lookup`` ())
do test "local int function name"                       (BasicNameOfTests.``local int function name`` ())
do test "local curried function name"                   (BasicNameOfTests.``local curried function name`` ())
do test "local tupled function name"                    (BasicNameOfTests.``local tupled function name`` ())
do test "local unit function name"                      (BasicNameOfTests.``local unit function name`` ())
do test "local function parameter name"                 (BasicNameOfTests.``local function parameter name`` ())
do test "can get name from inside a local function (needs to be let rec)"
                                                        (BasicNameOfTests.``can get name from inside a local function (needs to be let rec)`` ())
do test "can get name from inside static member"        (BasicNameOfTests.``can get name from inside static member`` ())
do test "can get name from inside instance member"      ((BasicNameOfTests()).``can get name from inside instance member`` ())
do test "can get name of instance member"               ((BasicNameOfTests()).``can get name of instance member`` ())
do test "can get name of instance member via unchecked" ((BasicNameOfTests()).``can get name of instance member via unchecked`` ())
do test "namespace name"                                (BasicNameOfTests.``namespace name`` ())
do test "module name"                                   (BasicNameOfTests.``module name`` ())
do test "exception name"                                (BasicNameOfTests.``exception name`` ())
do test "nested type name 1"                            (BasicNameOfTests.``nested type name 1`` ())
do test "type name 2"                                   (BasicNameOfTests.``type name 2`` ())
do test "member function which is defined below"        (BasicNameOfTests.``member function which is defined below`` ())
do test "class member lookup"                           ((BasicNameOfTests()).``class member lookup`` ())
do test "static member function name"                   (BasicNameOfTests.``static member function name`` ())
do test "static property name"                          (BasicNameOfTests.``static property name`` ())
do test "member method starting with get_"              (BasicNameOfTests.``member method starting with get_`` ())
do test "static method starting with get_"              (BasicNameOfTests.``static method starting with get_`` ())
do test "nameof local property with encapsulated name"  (BasicNameOfTests.``nameof local property with encapsulated name`` ())

do test "single argument method group name lookup"      ((MethodGroupNameOfTests()).``single argument method group name lookup`` ())
do test "multiple argument method group name lookup"    ((MethodGroupNameOfTests()).``multiple argument method group name lookup`` ())

do test "measure 1"                                     ((UnionAndRecordNameOfTests()).``measure 1`` ())
do test "record case 1"                                 ((UnionAndRecordNameOfTests()).``record case 1`` ())
do test "union case 1"                                  ((UnionAndRecordNameOfTests()).``union case 1`` ())
do test "union case 2"                                  ((UnionAndRecordNameOfTests()).``union case 2`` ())

do test "ok in attribute"                               ((AttributeNameOfTests()).``ok in attribute`` ())

do test "lookup name of typeof operator"                ((OperatorNameOfTests()).``lookup name of typeof operator`` ())
do test "lookup name of + operator"                     ((OperatorNameOfTests()).``lookup name of + operator`` ())
do test "lookup name of |> operator"                    ((OperatorNameOfTests()).``lookup name of |> operator`` ())
do test "lookup name of nameof operator"                ((OperatorNameOfTests()).``lookup name of nameof operator`` ())

do test "use it as a match case guard"                  ((PatternMatchingOfOperatorNameTests()).``use it as a match case guard`` ())

do test "se it in a quotation"                          ((NameOfOperatorInQuotations()).``use it in a quotation`` ())

do test "use it in a generic function"                  ((NameOfOperatorForGenerics()).``use it in a generic function`` ())
do test "lookup name of a generic class"                ((NameOfOperatorForGenerics()).``lookup name of a generic class`` ())

do test "user defined nameof should shadow the operator"(UserDefinedNameOfTests.``user defined nameof should shadow the operator`` ())

#if TESTS_AS_APP
let RUN() = 
  match !failures with 
  | [] -> stdout.WriteLine "Test Passed"
  | _ ->  stdout.WriteLine "Test Failed"
#else
let aa =
  match !failures with 
  | [] ->
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

