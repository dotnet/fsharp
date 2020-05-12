module FSharp.Compiler.UnitTests.FsiTests

open System.IO
open FSharp.Compiler.Interactive.Shell
open NUnit.Framework

type FSharpErrorSeverity = FSharp.Compiler.SourceCodeServices.FSharpErrorSeverity

type CustomType = { X: int }

let createFsiSession () =
    // Intialize output and input streams
    let inStream = new StringReader("")
    let outStream = new CompilerOutputStream()
    let errStream = new CompilerOutputStream()

    // Build command line arguments & start FSI session
    let argv = [| "C:\\fsi.exe" |]
    let allArgs = Array.append argv [|"--noninteractive"|]

    let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
    FsiEvaluationSession.Create(fsiConfig, allArgs, inStream, new StreamWriter(outStream), new StreamWriter(errStream), collectible = true)

[<Test>]
let ``No bound values at the start of FSI session`` () =
    use fsiSession = createFsiSession ()
    let values = fsiSession.GetBoundValues()
    Assert.IsEmpty values

[<Test>]
let ``Bound value has correct name`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("let x = 1")

    let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

    Assert.AreEqual("x", boundValue.Name)

[<Test>]
let ``Bound value has correct value`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("let y = 2")

    let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

    Assert.AreEqual(2, boundValue.Value.ReflectionValue)

[<Test>]
let ``Bound value has correct type`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("let z = 3")

    let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

    Assert.AreEqual(typeof<int>, boundValue.Value.ReflectionType)

[<Test>]
let ``Seven bound values are ordered and have their correct name`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("let x = 1")
    fsiSession.EvalInteraction("let y = 2")
    fsiSession.EvalInteraction("let z = 3")
    fsiSession.EvalInteraction("let a = 4")
    fsiSession.EvalInteraction("let ccc = 5")
    fsiSession.EvalInteraction("let b = 6")
    fsiSession.EvalInteraction("let aa = 7")

    let names = fsiSession.GetBoundValues() |> List.map (fun x -> x.Name)

    Assert.AreEqual(["a";"aa";"b";"ccc";"x";"y";"z"], names)

[<Test>]
let ``Seven bound values are ordered and have their correct value`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("let x = 1")
    fsiSession.EvalInteraction("let y = 2")
    fsiSession.EvalInteraction("let z = 3")
    fsiSession.EvalInteraction("let a = 4")
    fsiSession.EvalInteraction("let ccc = 5")
    fsiSession.EvalInteraction("let b = 6")
    fsiSession.EvalInteraction("let aa = 7")

    let values = fsiSession.GetBoundValues() |> List.map (fun x -> x.Value.ReflectionValue)

    Assert.AreEqual([4;7;6;5;1;2;3], values)

[<Test>]
let ``Seven bound values are ordered and have their correct type`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("let x = 1")
    fsiSession.EvalInteraction("let y = 2")
    fsiSession.EvalInteraction("let z = 3")
    fsiSession.EvalInteraction("let a = 4.")
    fsiSession.EvalInteraction("let ccc = 5")
    fsiSession.EvalInteraction("let b = 6.f")
    fsiSession.EvalInteraction("let aa = 7")

    let types = fsiSession.GetBoundValues() |> List.map (fun x -> x.Value.ReflectionType)

    Assert.AreEqual([typeof<float>;typeof<int>;typeof<float32>;typeof<int>;typeof<int>;typeof<int>;typeof<int>], types)

[<Test>]
let ``Able to find a bound value by the identifier`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("let x = 1")
    fsiSession.EvalInteraction("let y = 2")
    fsiSession.EvalInteraction("let z = 3")
    fsiSession.EvalInteraction("let a = 4")
    fsiSession.EvalInteraction("let ccc = 5")
    fsiSession.EvalInteraction("let b = 6")
    fsiSession.EvalInteraction("let aa = 7")

    let boundValueOpt = fsiSession.TryFindBoundValue "ccc"

    Assert.IsTrue boundValueOpt.IsSome

[<Test>]
let ``Able to find a bound value by the identifier and has valid info`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("let x = 1.")
    fsiSession.EvalInteraction("let y = 2.")
    fsiSession.EvalInteraction("let z = 3")
    fsiSession.EvalInteraction("let a = 4.")
    fsiSession.EvalInteraction("let ccc = 5.")
    fsiSession.EvalInteraction("let b = 6.")
    fsiSession.EvalInteraction("let aa = 7.")

    let boundValue = (fsiSession.TryFindBoundValue "z").Value
    
    Assert.AreEqual("z", boundValue.Name)
    Assert.AreEqual(3, boundValue.Value.ReflectionValue)
    Assert.AreEqual(typeof<int>, boundValue.Value.ReflectionType)

[<Test>]
let ``Not Able to find a bound value by the identifier`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("let x = 1")
    fsiSession.EvalInteraction("let y = 2")
    fsiSession.EvalInteraction("let z = 3")
    fsiSession.EvalInteraction("let a = 4")
    fsiSession.EvalInteraction("let ccc = 5")
    fsiSession.EvalInteraction("let b = 6")
    fsiSession.EvalInteraction("let aa = 7")

    let boundValueOpt = fsiSession.TryFindBoundValue "aaa"

    Assert.IsTrue boundValueOpt.IsNone

[<Test>]
let ``The 'it' value does not exist at the start of a FSI session`` () =
    use fsiSession = createFsiSession ()

    let boundValueOpt = fsiSession.TryFindBoundValue "it"

    Assert.IsTrue boundValueOpt.IsNone

[<Test>]
let ``The 'it' bound value does exists after a value is not explicitly bound`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("456")

    let boundValueOpt = fsiSession.TryFindBoundValue "it"

    Assert.IsTrue boundValueOpt.IsSome

[<Test>]
let ``The 'it' value does exists after a value is not explicitly bound and has valid info`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("456")

    let boundValue = (fsiSession.TryFindBoundValue "it").Value

    Assert.AreEqual("it", boundValue.Name)
    Assert.AreEqual(456, boundValue.Value.ReflectionValue)
    Assert.AreEqual(typeof<int>, boundValue.Value.ReflectionType)

[<Test>]
let ``The latest shadowed value is only available`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("let x = 1")
    let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

    Assert.AreEqual("x", boundValue.Name)
    Assert.AreEqual(1, boundValue.Value.ReflectionValue)
    Assert.AreEqual(typeof<int>, boundValue.Value.ReflectionType)

    fsiSession.EvalInteraction("let x = (1, 2)")
    let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

    Assert.AreEqual("x", boundValue.Name)
    Assert.AreEqual((1, 2), boundValue.Value.ReflectionValue)
    Assert.AreEqual(typeof<int * int>, boundValue.Value.ReflectionType)

[<Test>]
let ``The latest shadowed value is only available and can be found`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("let x = 1")
    let boundValue = (fsiSession.TryFindBoundValue "x").Value

    Assert.AreEqual("x", boundValue.Name)
    Assert.AreEqual(1, boundValue.Value.ReflectionValue)
    Assert.AreEqual(typeof<int>, boundValue.Value.ReflectionType)

    fsiSession.EvalInteraction("let x = (1, 2)")
    let boundValue = (fsiSession.TryFindBoundValue "x").Value

    Assert.AreEqual("x", boundValue.Name)
    Assert.AreEqual((1, 2), boundValue.Value.ReflectionValue)
    Assert.AreEqual(typeof<int * int>, boundValue.Value.ReflectionType)

[<Test>]
let ``Values are successfully shadowed even with intermediate interactions`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("let x = 1")
    fsiSession.EvalInteraction("let z = 100")
    fsiSession.EvalInteraction("let x = (1, 2)")
    fsiSession.EvalInteraction("let w = obj ()")

    let boundValues = fsiSession.GetBoundValues()

    Assert.AreEqual(3, boundValues.Length)

    let boundValue = boundValues |> List.find (fun x -> x.Name = "x")

    Assert.AreEqual("x", boundValue.Name)
    Assert.AreEqual((1, 2), boundValue.Value.ReflectionValue)
    Assert.AreEqual(typeof<int * int>, boundValue.Value.ReflectionType)

    let boundValue = (fsiSession.TryFindBoundValue "x").Value

    Assert.AreEqual("x", boundValue.Name)
    Assert.AreEqual((1, 2), boundValue.Value.ReflectionValue)
    Assert.AreEqual(typeof<int * int>, boundValue.Value.ReflectionType)

[<Test>]
let ``Creation of a simple bound value succeeds`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("x", 1)
    Assert.IsEmpty errors

    let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

    Assert.AreEqual("x", boundValue.Name)
    Assert.AreEqual(typeof<int>, boundValue.Value.ReflectionType)
    Assert.AreEqual(1, boundValue.Value.ReflectionValue)

[<Test>]
let ``Creation of a bound value succeeds with underscores in the identifier`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("x_y_z", 1)
    Assert.IsEmpty errors

    let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

    Assert.AreEqual("x_y_z", boundValue.Name)

[<Test>]
let ``Creation of a bound value succeeds with tildes in the identifier`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("``hello world``", 1)
    Assert.IsEmpty errors

    let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

    Assert.AreEqual("``hello world``", boundValue.Name)

[<Test>]
let ``Creation of a bound value succeeds with 'it' as the indentifier`` () =
    use fsiSession = createFsiSession ()

    fsiSession.EvalInteraction("\"test\"")

    let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

    Assert.AreEqual("it", boundValue.Name)
    Assert.AreEqual(typeof<string>, boundValue.Value.ReflectionType)

    let errors = fsiSession.AddBoundValue("it", 1)
    Assert.IsEmpty errors

    let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

    Assert.AreEqual("it", boundValue.Name)
    Assert.AreEqual(typeof<int>, boundValue.Value.ReflectionType)

[<Test>]
let ``Creation of a bound value succeeds with tildes in the identifier and with 'at' but has warning`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("``hello @ world``", 1)
    let error = errors |> Array.exactlyOne

    Assert.AreEqual(FSharpErrorSeverity.Warning, error.Severity)
    Assert.AreEqual("Identifiers containing '@' are reserved for use in F# code generation", error.Message)

    let boundValue = fsiSession.GetBoundValues() |> List.exactlyOne

    Assert.AreEqual("``hello @ world``", boundValue.Name)

[<Test>]
let ``Creation of a bound value fails if the name is not a valid identifier with 'at' in front`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("@x", 1)
    let error = errors |> Array.exactlyOne

    Assert.AreEqual(FSharpErrorSeverity.Error, error.Severity)
    Assert.AreEqual("Identifier expected", error.Message)

[<Test>]
let ``Creation of a bound value fails if the name is not a valid identifier with 'at' in back`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("x@", 1)
    let error = errors |> Array.exactlyOne

    Assert.AreEqual(FSharpErrorSeverity.Error, error.Severity)
    Assert.AreEqual("Identifier expected", error.Message)

[<Test>]
let ``Creation of a bound value fails if the name is null`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue(null, 1)
    let error = errors |> Array.exactlyOne

    Assert.AreEqual(FSharpErrorSeverity.Error, error.Severity)
    Assert.AreEqual("Identifier expected", error.Message)

[<Test>]
let ``Creation of a bound value fails if the name is empty`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("", 1)
    let error = errors |> Array.exactlyOne

    Assert.AreEqual(FSharpErrorSeverity.Error, error.Severity)
    Assert.AreEqual("Identifier expected", error.Message)

[<Test>]
let ``Creation of a bound value fails if the name is whitespace`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue(" ", 1)
    let error = errors |> Array.exactlyOne

    Assert.AreEqual(FSharpErrorSeverity.Error, error.Severity)
    Assert.AreEqual("Identifier expected", error.Message)

[<Test>]
let ``Creation of a bound value fails if the name contains spaces`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("x x", 1)
    let error = errors |> Array.exactlyOne

    Assert.AreEqual(FSharpErrorSeverity.Error, error.Severity)
    Assert.AreEqual("Identifier expected", error.Message)

[<Test>]
let ``Creation of a bound value fails if the name contains an operator at the end`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("x+", 1)
    let error = errors |> Array.exactlyOne

    Assert.AreEqual(FSharpErrorSeverity.Error, error.Severity)
    Assert.AreEqual("Identifier expected", error.Message)

[<Test>]
let ``Creation of a bound value fails if the name contains an operator at the front`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("+x", 1)
    let error = errors |> Array.exactlyOne

    Assert.AreEqual(FSharpErrorSeverity.Error, error.Severity)
    Assert.AreEqual("Identifier expected", error.Message)

[<Test>]
let ``Creation of a bound value fails if the name contains dots`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("x.x", 1)
    let error = errors |> Array.exactlyOne

    Assert.AreEqual(FSharpErrorSeverity.Error, error.Severity)
    Assert.AreEqual("Identifier expected", error.Message)

[<Test>]
let ``Creation of a bound value succeeds if the value contains types from assemblies that are not referenced in the session, due to implicit resolution`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("x", { X = 1 })
    Assert.IsEmpty errors

[<Test>]
let ``Creation of a bound value succeeds if the value contains types from assemblies that are not referenced in the session, due to implicit resolution, and then doing some evaluation`` () =
    use fsiSession = createFsiSession ()

    let errors = fsiSession.AddBoundValue("x", { X = 1 })
    Assert.IsEmpty errors

    fsiSession.EvalInteraction("let y = { x with X = 5 }")

    let boundValues = fsiSession.GetBoundValues()
    Assert.AreEqual(2, boundValues.Length)

    let v1 = boundValues.[0]
    let v2 = boundValues.[1]

    Assert.AreEqual("x", v1.Name)
    Assert.AreEqual({ X = 1 }, v1.Value.ReflectionValue)
    Assert.AreEqual(typeof<CustomType>, v1.Value.ReflectionType)

    Assert.AreEqual("y", v2.Name)
    Assert.AreEqual({ X = 5 }, v2.Value.ReflectionValue)
    Assert.AreEqual(typeof<CustomType>, v2.Value.ReflectionType)