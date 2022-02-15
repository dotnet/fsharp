namespace FSharp.Compiler.UnitTests

open System
open System.IO
open FluentAssertions
open FSharp.Compiler.Interactive.Shell
open FSharp.Test
open Xunit

[<Collection("SingleThreaded")>]
module FsiTests =

    let createFsiSession (useOneDynamicAssembly: bool) =
        // Intialize output and input streams
        let inStream = new StringReader("")
        let outStream = new CompilerOutputStream()
        let errStream = new CompilerOutputStream()

        // Build command line arguments & start FSI session
        let argv = [| "C:\\fsi.exe" |]
        let allArgs = Array.append argv [|"--noninteractive"; if useOneDynamicAssembly then "--refemit" else "--refemit-" |]

        let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
        FsiEvaluationSession.Create(fsiConfig, allArgs, inStream, new StreamWriter(outStream), new StreamWriter(errStream), collectible = true)

    [<Fact>]
    let ``No bound values at the start of FSI session`` () =
        use fsiSession = createFsiSession false
        let values = fsiSession.GetBoundValues()
        Assert.shouldBeEmpty values

    [<Fact>]
    let ``Bound value has correct name`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("let x = 1")

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "x" boundValue.Name

    [<Fact>]
    let ``Bound value has correct value`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("let y = 2")

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe 2 boundValue.Value.ReflectionValue

    [<Fact>]
    let ``Bound value has correct type`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("let z = 3")

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe typeof<int> boundValue.Value.ReflectionType

    [<Fact>]
    let ``Seven bound values are ordered and have their correct name`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("let x = 1")
        fsiSession.EvalInteraction("let y = 2")
        fsiSession.EvalInteraction("let z = 3")
        fsiSession.EvalInteraction("let a = 4")
        fsiSession.EvalInteraction("let ccc = 5")
        fsiSession.EvalInteraction("let b = 6")
        fsiSession.EvalInteraction("let aa = 7")

        let names = fsiSession.GetBoundValues() |> List.map (fun x -> x.Name)

        Assert.shouldBe ["a";"aa";"b";"ccc";"x";"y";"z"] names

    [<Fact>]
    let ``Seven bound values are ordered and have their correct value`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("let x = 1")
        fsiSession.EvalInteraction("let y = 2")
        fsiSession.EvalInteraction("let z = 3")
        fsiSession.EvalInteraction("let a = 4")
        fsiSession.EvalInteraction("let ccc = 5")
        fsiSession.EvalInteraction("let b = 6")
        fsiSession.EvalInteraction("let aa = 7")

        let values = fsiSession.GetBoundValues() |> List.map (fun x -> x.Value.ReflectionValue)

        Assert.shouldBeEquivalentTo [4;7;6;5;1;2;3] values

    [<Fact>]
    let ``Seven bound values are ordered and have their correct type`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("let x = 1")
        fsiSession.EvalInteraction("let y = 2")
        fsiSession.EvalInteraction("let z = 3")
        fsiSession.EvalInteraction("let a = 4.")
        fsiSession.EvalInteraction("let ccc = 5")
        fsiSession.EvalInteraction("let b = 6.f")
        fsiSession.EvalInteraction("let aa = 7")

        let types = fsiSession.GetBoundValues() |> List.map (fun x -> x.Value.ReflectionType)

        Assert.shouldBe [typeof<float>;typeof<int>;typeof<float32>;typeof<int>;typeof<int>;typeof<int>;typeof<int>] types

    [<Fact>]
    let ``Able to find a bound value by the identifier`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("let x = 1")
        fsiSession.EvalInteraction("let y = 2")
        fsiSession.EvalInteraction("let z = 3")
        fsiSession.EvalInteraction("let a = 4")
        fsiSession.EvalInteraction("let ccc = 5")
        fsiSession.EvalInteraction("let b = 6")
        fsiSession.EvalInteraction("let aa = 7")

        let boundValueOpt = fsiSession.TryFindBoundValue "ccc"

        Assert.shouldBeTrue boundValueOpt.IsSome

    [<Fact>]
    let ``Able to find a bound value by the identifier and has valid info`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("let x = 1.")
        fsiSession.EvalInteraction("let y = 2.")
        fsiSession.EvalInteraction("let z = 3")
        fsiSession.EvalInteraction("let a = 4.")
        fsiSession.EvalInteraction("let ccc = 5.")
        fsiSession.EvalInteraction("let b = 6.")
        fsiSession.EvalInteraction("let aa = 7.")

        let boundValue = (fsiSession.TryFindBoundValue "z").Value

        Assert.shouldBe "z" boundValue.Name
        Assert.shouldBe 3 boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<int> boundValue.Value.ReflectionType

    [<Fact>]
    let ``Not Able to find a bound value by the identifier`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("let x = 1")
        fsiSession.EvalInteraction("let y = 2")
        fsiSession.EvalInteraction("let z = 3")
        fsiSession.EvalInteraction("let a = 4")
        fsiSession.EvalInteraction("let ccc = 5")
        fsiSession.EvalInteraction("let b = 6")
        fsiSession.EvalInteraction("let aa = 7")

        let boundValueOpt = fsiSession.TryFindBoundValue "aaa"

        Assert.shouldBeTrue boundValueOpt.IsNone

    [<Fact>]
    let ``The 'it' value does not exist at the start of a FSI session`` () =
        use fsiSession = createFsiSession false

        let boundValueOpt = fsiSession.TryFindBoundValue "it"

        Assert.shouldBeTrue boundValueOpt.IsNone

    [<Fact>]
    let ``The 'it' bound value does exists after a value is not explicitly bound`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("456")

        let boundValueOpt = fsiSession.TryFindBoundValue "it"

        Assert.shouldBeTrue boundValueOpt.IsSome

    [<Fact>]
    let ``The 'it' value does exists after a value is not explicitly bound and has valid info`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("456")

        let boundValue = (fsiSession.TryFindBoundValue "it").Value

        Assert.shouldBe "it" boundValue.Name
        Assert.shouldBe 456 boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<int> boundValue.Value.ReflectionType

    [<Fact>]
    let ``The latest shadowed value is only available`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("let x = 1")
        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "x" boundValue.Name
        Assert.shouldBe 1 boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<int> boundValue.Value.ReflectionType

        fsiSession.EvalInteraction("let x = (1, 2)")
        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "x" boundValue.Name
        Assert.shouldBe (1, 2) boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<int * int> boundValue.Value.ReflectionType

    [<Fact>]
    let ``The latest shadowed value is only available and can be found`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("let x = 1")
        let boundValue = (fsiSession.TryFindBoundValue "x").Value

        Assert.shouldBe "x" boundValue.Name
        Assert.shouldBe 1 boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<int> boundValue.Value.ReflectionType

        fsiSession.EvalInteraction("let x = (1, 2)")
        let boundValue = (fsiSession.TryFindBoundValue "x").Value

        Assert.shouldBe "x" boundValue.Name
        Assert.shouldBe (1, 2) boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<int * int> boundValue.Value.ReflectionType

    [<Fact>]
    let ``Values are successfully shadowed even with intermediate interactions`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("let x = 1")
        fsiSession.EvalInteraction("let z = 100")
        fsiSession.EvalInteraction("let x = (1, 2)")
        fsiSession.EvalInteraction("let w = obj ()")

        let boundValues = fsiSession.GetBoundValues()

        Assert.shouldBe 3 boundValues.Length

        let boundValue = boundValues |> List.find (fun x -> x.Name = "x")

        Assert.shouldBe "x" boundValue.Name
        Assert.shouldBe (1, 2) boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<int * int> boundValue.Value.ReflectionType

        let boundValue = (fsiSession.TryFindBoundValue "x").Value

        Assert.shouldBe "x" boundValue.Name
        Assert.shouldBe (1, 2) boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<int * int> boundValue.Value.ReflectionType

    [<Fact>]
    let ``Creation of a simple bound value succeeds`` () =
        use fsiSession = createFsiSession false

        fsiSession.AddBoundValue("x", 1)

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "x" boundValue.Name
        Assert.shouldBe typeof<int> boundValue.Value.ReflectionType
        Assert.shouldBe 1 boundValue.Value.ReflectionValue

    [<Fact>]
    let ``Creation of a bound value succeeds with underscores in the identifier`` () =
        use fsiSession = createFsiSession false

        fsiSession.AddBoundValue("x_y_z", 1)

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "x_y_z" boundValue.Name

    [<Fact>]
    let ``Creation of a bound value succeeds with tildes in the identifier`` () =
        use fsiSession = createFsiSession false

        fsiSession.AddBoundValue("``hello world``", 1)

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "``hello world``" boundValue.Name

    [<Fact>]
    let ``Creation of a bound value succeeds with 'it' as the indentifier`` () =
        use fsiSession = createFsiSession false

        fsiSession.EvalInteraction("\"test\"")

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "it" boundValue.Name
        Assert.shouldBe typeof<string> boundValue.Value.ReflectionType

        fsiSession.AddBoundValue("it", 1)

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "it" boundValue.Name
        Assert.shouldBe typeof<int> boundValue.Value.ReflectionType

    [<Fact>]
    let ``Creation of a bound value fails with tildes in the identifier and with 'at' but has warning`` () =
        use fsiSession = createFsiSession false

        Assert.Throws<ArgumentException>(fun () -> fsiSession.AddBoundValue("``hello @ world``", 1)) |> ignore

    [<Fact>]
    let ``Creation of a bound value fails if the name is not a valid identifier with 'at' in front`` () =
        use fsiSession = createFsiSession false

        Assert.Throws<ArgumentException>(fun () -> fsiSession.AddBoundValue("@x", 1)) |> ignore

    [<Fact>]
    let ``Creation of a bound value fails if the name is not a valid identifier with 'at' in back`` () =
        use fsiSession = createFsiSession false

        Assert.Throws<ArgumentException>(fun () -> fsiSession.AddBoundValue("x@", 1)) |> ignore

    [<Fact>]
    let ``Creation of a bound value fails if the name is null`` () =
        use fsiSession = createFsiSession false

        Assert.Throws<ArgumentException>(fun () -> fsiSession.AddBoundValue(null, 1)) |> ignore

    [<Fact>]
    let ``Creation of a bound value fails if the name is empty`` () =
        use fsiSession = createFsiSession false

        Assert.Throws<ArgumentException>(fun () -> fsiSession.AddBoundValue("", 1)) |> ignore

    [<Fact>]
    let ``Creation of a bound value fails if the name is whitespace`` () =
        use fsiSession = createFsiSession false

        Assert.Throws<ArgumentException>(fun () -> fsiSession.AddBoundValue(" ", 1)) |> ignore

    [<Fact>]
    let ``Creation of a bound value fails if the name contains spaces`` () =
        use fsiSession = createFsiSession false

        Assert.Throws<ArgumentException>(fun () -> fsiSession.AddBoundValue("x x", 1)) |> ignore

    [<Fact>]
    let ``Creation of a bound value fails if the name contains an operator at the end`` () =
        use fsiSession = createFsiSession false

        Assert.Throws<ArgumentException>(fun () -> fsiSession.AddBoundValue("x+", 1)) |> ignore

    [<Fact>]
    let ``Creation of a bound value fails if the name contains an operator at the front`` () =
        use fsiSession = createFsiSession false

        Assert.Throws<ArgumentException>(fun () -> fsiSession.AddBoundValue("+x", 1)) |> ignore

    [<Fact>]
    let ``Creation of a bound value fails if the name contains dots`` () =
        use fsiSession = createFsiSession false

        Assert.Throws<ArgumentException>(fun () -> fsiSession.AddBoundValue("x.x", 1)) |> ignore

    [<Fact>]
    let ``Creation of a bound value fails if the value passed is null`` () =
        use fsiSession = createFsiSession false

        Assert.Throws<ArgumentNullException>(fun () -> fsiSession.AddBoundValue("x", null) |> ignore) |> ignore

    type CustomType = { X: int }

    [<Fact>]
    let ``Creation of a bound value succeeds if the value contains types from assemblies that are not referenced in the session, due to implicit resolution`` () =
        use fsiSession = createFsiSession false

        fsiSession.AddBoundValue("x", { X = 1 })

    [<Fact>]
    let ``Creation of a bound value succeeds if the value contains types from assemblies that are not referenced in the session, due to implicit resolution, and then doing some evaluation`` () =
        use fsiSession = createFsiSession false

        fsiSession.AddBoundValue("x", { X = 1 })
        fsiSession.EvalInteraction("let y = { x with X = 5 }")

        let boundValues = fsiSession.GetBoundValues()
        Assert.shouldBe 2 boundValues.Length

        let v1 = boundValues.[0]
        let v2 = boundValues.[1]

        Assert.shouldBe "x" v1.Name
        Assert.shouldBe { X = 1 } v1.Value.ReflectionValue
        Assert.shouldBe typeof<CustomType> v1.Value.ReflectionType

        Assert.shouldBe "y" v2.Name
        Assert.shouldBe { X = 5 } v2.Value.ReflectionValue
        Assert.shouldBe typeof<CustomType> v2.Value.ReflectionType

    [<Fact>]
    let ``Creation of a bound value, of type ResizeArray<string>, succeeds`` () =
        use fsiSession = createFsiSession false

        let xs = ResizeArray()
        xs.Add("banana")
        xs.Add("apple")

        fsiSession.AddBoundValue("xs", xs)

        let boundValues = fsiSession.GetBoundValues()
        Assert.shouldBe 1 boundValues.Length

        let v1 = boundValues.[0]

        Assert.shouldBe "xs" v1.Name
        Assert.shouldBe xs v1.Value.ReflectionValue
        Assert.shouldBe typeof<ResizeArray<string>> v1.Value.ReflectionType

    [<Struct>]
    type CustomStruct(x: int) =
        member _.X = x

    type CustomType2() =

        member _.Message = "hello"

    [<Fact>]
    let ``Creation of a bound value succeeds if the value contains types from assemblies that are not referenced in the session, due to implicit resolution, and then use a member from it`` () =
        use fsiSession = createFsiSession false

        let value = CustomType2()
        fsiSession.AddBoundValue("x", value)

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "x" boundValue.Name
        Assert.shouldBe value boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<CustomType2> boundValue.Value.ReflectionType

        fsiSession.EvalInteraction("let x = x.Message")

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "x" boundValue.Name
        Assert.shouldBe "hello" boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<string> boundValue.Value.ReflectionType

    [<Fact>]
    let ``Creation of a bound value succeeds if the value contains generic types from assemblies that are not referenced in the session, due to implicit resolution, and then use a member from it`` () =
        use fsiSession = createFsiSession false

        let value = ResizeArray<CustomType2>()
        value.Add(CustomType2())

        fsiSession.AddBoundValue("x", value)

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "x" boundValue.Name
        Assert.shouldBe value boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<ResizeArray<CustomType2>> boundValue.Value.ReflectionType

        fsiSession.EvalInteraction("let x = x.[0].Message")

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "x" boundValue.Name
        Assert.shouldBe "hello" boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<string> boundValue.Value.ReflectionType

    [<Fact>]
    let ``Creation of a bound value succeeds if the value contains two generic types from assemblies that are not referenced in the session, due to implicit resolution`` () =
        use fsiSession = createFsiSession false

        let value = ({ X = 1 }, CustomType2())

        fsiSession.AddBoundValue("x", value)

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe "x" boundValue.Name
        Assert.shouldBe value boundValue.Value.ReflectionValue
        Assert.shouldBe typeof<CustomType * CustomType2> boundValue.Value.ReflectionType

    [<Fact>]
    let ``Creation of a bound value fails if the value contains types from a dynamic assembly using refemit`` () =
        use fsiSession = createFsiSession true

        fsiSession.AddBoundValue("fsiSession", fsiSession)

        let res, _ = fsiSession.EvalInteractionNonThrowing("""
    type TypeInDynamicAssembly() = class end
    fsiSession.AddBoundValue("x", TypeInDynamicAssembly())""")

        match res with
        | Choice2Of2 ex -> Assert.shouldBe typeof<NotSupportedException> (ex.GetType())
        | _ -> failwith "Expected an exception"

    [<Fact>]
    let ``Creation of a bound value fails if the value contains types from a dynamic assembly using ilwrite`` () =
        use fsiSession = createFsiSession false

        fsiSession.AddBoundValue("fsiSession", fsiSession)

        let res, _ = fsiSession.EvalInteractionNonThrowing("""
    type TypeInDynamicAssembly() = class end
    fsiSession.AddBoundValue("x", TypeInDynamicAssembly())""")

        match res with
        | Choice2Of2 ex -> Assert.shouldBe typeof<InvalidOperationException> (ex.GetType())
        | _ -> failwith "Expected an exception"

    type internal NonPublicCustomType() = class end

    [<Fact>]
    let ``Creation of a bound value fails if the value's type is not public`` () =
        use fsiSession = createFsiSession false

        Assert.Throws<InvalidOperationException>(fun () -> fsiSession.AddBoundValue("x", NonPublicCustomType())) |> ignore

    [<Fact>]
    let ``Creation of a bound value succeeds if the value is a partial application function type`` () =
        use fsiSession = createFsiSession false

        fsiSession.AddBoundValue("createFsiSession", createFsiSession)

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe typeof<bool -> FsiEvaluationSession> boundValue.Value.ReflectionType

    [<Fact>]
    let ``Creation of a bound value succeeds if the value is a partial application function type with four arguments`` () =
        use fsiSession = createFsiSession false

        let addXYZW x y z w = x + y + z + w
        let addYZW = addXYZW 1
        let addZW = addYZW 2

        fsiSession.AddBoundValue("addZW", addZW)

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe typeof<int -> int -> int> boundValue.Value.ReflectionType

    [<Fact>]
    let ``Creation of a bound value succeeds if the value is a lambda`` () =
        use fsiSession = createFsiSession false

        fsiSession.AddBoundValue("addXYZ", fun x y z -> x + y + z)

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe typeof<int -> int -> int -> int> boundValue.Value.ReflectionType

    type TestFSharpFunc() =
        inherit FSharpFunc<int, int>()
        override _.Invoke x = x

    type Test2FSharpInheritFunc() =
        inherit TestFSharpFunc()

    [<Fact>]
    let ``Creation of a bound value succeeds if the value is a type that inherits FSharpFunc`` () =
        use fsiSession = createFsiSession false

        fsiSession.AddBoundValue("test", Test2FSharpInheritFunc())

        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

        Assert.shouldBe typeof<Test2FSharpInheritFunc> boundValue.Value.ReflectionType

    [<Fact>]
    let ``Creation of a bound value succeeds if the value is an array of a built-in value type``() =
        use fsiSession = createFsiSession false
        let arr = [|0; 1|]
        fsiSession.AddBoundValue("boundMdArray", arr)
        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne
        Assert.shouldBe typeof<int[]> boundValue.Value.ReflectionType
        boundValue.Value.ReflectionValue.Should().Be(arr, "") |> ignore

    [<Fact>]
    let ``Creation of a bound value succeeds if the value is an array of a built-in reference type``() =
        use fsiSession = createFsiSession false
        let arr = [|"zero"; "one"|]
        fsiSession.AddBoundValue("boundMdArray", arr)
        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne
        Assert.shouldBe typeof<string[]> boundValue.Value.ReflectionType
        boundValue.Value.ReflectionValue.Should().Be(arr, "") |> ignore

    [<Fact>]
    let ``Creation of a bound value succeeds if the value is an array of a custom value type``() =
        use fsiSession = createFsiSession false
        let arr = [|CustomStruct(1)|]
        fsiSession.AddBoundValue("boundMdArray", arr)
        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne
        Assert.shouldBe typeof<CustomStruct[]> boundValue.Value.ReflectionType
        boundValue.Value.ReflectionValue.Should().Be(arr, "") |> ignore

    [<Fact>]
    let ``Creation of a bound value succeeds if the value is an array of a custom reference type``() =
        use fsiSession = createFsiSession false
        let arr = [|CustomType2()|]
        fsiSession.AddBoundValue("boundMdArray", arr)
        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne
        Assert.shouldBe typeof<CustomType2[]> boundValue.Value.ReflectionType
        boundValue.Value.ReflectionValue.Should().Be(arr, "") |> ignore

    [<Fact>]
    let ``Creation of a bound value succeeds if the value is a multidimensional array of a built-in value type``() =
        use fsiSession = createFsiSession false
        let mdArr = array2D [[1; 0]; [0; 1]]
        fsiSession.AddBoundValue("boundMdArray", mdArr)
        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne
        Assert.shouldBe typeof<int[,]> boundValue.Value.ReflectionType
        boundValue.Value.ReflectionValue.Should().Be(mdArr, "") |> ignore

    [<Fact>]
    let ``Creation of a bound value succeeds if the value is a multidimensional array of a built-in reference type``() =
        use fsiSession = createFsiSession false
        let mdArr = array2D [["one"; "zero"]; ["zero"; "one"]]
        fsiSession.AddBoundValue("boundMdArray", mdArr)
        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne
        Assert.shouldBe typeof<string[,]> boundValue.Value.ReflectionType
        boundValue.Value.ReflectionValue.Should().Be(mdArr, "") |> ignore

    [<Fact>]
    let ``Creation of a bound value succeeds if the value is a multidimensional array of a custom value type``() =
        use fsiSession = createFsiSession false
        let mdArr = array2D [[CustomStruct(1); CustomStruct(1)]; [CustomStruct(1); CustomStruct(1)]]
        fsiSession.AddBoundValue("boundMdArray", mdArr)
        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne
        Assert.shouldBe typeof<CustomStruct[,]> boundValue.Value.ReflectionType
        boundValue.Value.ReflectionValue.Should().Be(mdArr, "") |> ignore

    [<Fact>]
    let ``Creation of a bound value succeeds if the value is a multidimensional array of a custom reference type``() =
        use fsiSession = createFsiSession false
        let mdArr = array2D [[CustomType2(); CustomType2()]; [CustomType2(); CustomType2()]]
        fsiSession.AddBoundValue("boundMdArray", mdArr)
        let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne
        Assert.shouldBe typeof<CustomType2[,]> boundValue.Value.ReflectionType
        boundValue.Value.ReflectionValue.Should().Be(mdArr, "") |> ignore
