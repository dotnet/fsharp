module FSharp.Editor.Tests.Refactors.AddReturnTypeTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit
open FSharp.Editor.Tests.Refactors.RefactorTestFramework
open FSharp.Test.ProjectGeneration
open FSharp.Editor.Tests.Helpers

[<Theory>]
[<InlineData(":int")>]
[<InlineData(" :int")>]
[<InlineData(" : int")>]
[<InlineData(" :    int")>]
let ``Refactor should not trigger`` (shouldNotTrigger: string) =
    let symbolName = "sum"

    let code =
        $"""
let sum a b {shouldNotTrigger}= a + b
            """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let actions = tryGetRefactoringActions code spanStart context (new AddReturnType())

    do Assert.Empty(actions)

[<Fact>]
let ``Refactor should not trigger on values`` () =
    let symbolName = "example2"

    let code =
        """
let example2 = 42 // value
            """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let actions = tryGetRefactoringActions code spanStart context (new AddReturnType())

    do Assert.Empty(actions)

[<Fact>]
let ``Refactor should not trigger on member values`` () =
    let symbolName = "SomeProp"

    let code =
        """
type Example3() =
    member _.SomeProp = 42 // property
            """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let actions = tryGetRefactoringActions code spanStart context (new AddReturnType())

    do Assert.Empty(actions)

[<Fact>]
let ``Correctly infer int as return type`` () =
    let symbolName = "sum"

    let code =
        """
let sum a b = a + b
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        $"""
let sum a b : int = a + b
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Correctly infer on next line arguments`` () =
    let symbolName = "sum"

    let code =
        """
let sum
    x y =
    x + y
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        $"""
let sum
    x y : int =
    x + y
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Should not throw exception when binding another method`` () =
    let symbolName = "addThings"

    let code =
        """
let add (x:int) (y:int) = (float(x + y)) + 0.1
let addThings = add
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        $"""
let add (x:int) (y:int) = (float(x + y)) + 0.1
let addThings : (int->int->float) = add
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Handle parentheses on the arguments`` () =
    let symbolName = "sum"

    let code =
        """
let sum (a:float) (b:float) = a + b
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        """
let sum (a:float) (b:float) : float = a + b
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Infer on rec method`` () =
    let symbolName = "fib"

    let code =
        $"""
let rec fib n =
    if n < 2 then 1
    else fib (n - 1) + fib (n - 2)
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        $"""
let rec fib n : int =
    if n < 2 then 1
    else fib (n - 1) + fib (n - 2)
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Infer with function parameter method`` () =
    let symbolName = "apply1"

    let code =
        $"""
let apply1 (transform: int -> int) y = transform y
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        $"""
let apply1 (transform: int -> int) y : int = transform y
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Infer on member function`` () =
    let symbolName = "SomeMethod"

    let code =
        $"""
type SomeType(factor0: int) =
    let factor = factor0
    member this.SomeMethod(a, b, c) = (a + b + c) * factor
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        $"""
type SomeType(factor0: int) =
    let factor = factor0
    member this.SomeMethod(a, b, c) : int = (a + b + c) * factor
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Binding another function doesnt crash`` () =
    let symbolName = "getNow"

    let code =
        $"""
let getNow() = 
    System.DateTime.Now
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        $"""
let getNow() : System.DateTime = 
    System.DateTime.Now
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Handle already existing opens for DateTime`` () =
    let symbolName = "getNow"

    let code =
        $"""
open System

let getNow() = 
    DateTime.Now
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        $"""
open System

let getNow() : DateTime = 
    DateTime.Now
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Binding linq function doesnt crash`` () =
    let symbolName = "skip1"

    let code =
        $"""
let skip1 elements = 
    System.Linq.Enumerable.Skip(elements, 1)
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        $"""
let skip1 elements : System.Collections.Generic.IEnumerable<'a> = 
    System.Linq.Enumerable.Skip(elements, 1)
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Handle already existing opens on Linq`` () =
    let symbolName = "skip1"

    let code =
        $"""
open System

let skip1 elements = 
    Linq.Enumerable.Skip(elements, 1)
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        $"""
open System

let skip1 elements : Collections.Generic.IEnumerable<'a> = 
    Linq.Enumerable.Skip(elements, 1)
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Handle already existing opens on Enumerable`` () =
    let symbolName = "skip1"

    let code =
        $"""
open System
open System.Linq

let skip1 elements = 
    Enumerable.Skip(elements, 1)
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        $"""
open System
open System.Linq

let skip1 elements : Collections.Generic.IEnumerable<'a> = 
    Enumerable.Skip(elements, 1)
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Correctly infer custom type that is declared earlier in file`` () =
    let symbolName = "sum"

    let code =
        """
type MyType = { Value: int }
let sum a b = {Value=a+b}
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        """
type MyType = { Value: int }
let sum a b : MyType = {Value=a+b}
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Correctly infer custom type that is declared earlier in project`` () =
    let symbolName = "sum"

    let myModule =
        """
module ModuleFirst
type MyType = { Value: int }
        """

    let code =
        """
module ModuleSecond

open ModuleFirst

let sum a b = {Value=a+b}
        """

    let project =
        { SyntheticProject.Create(
              { sourceFile "First" [] with
                  Source = myModule
              },
              { sourceFile "Second" [ "First" ] with
                  Source = code
              }
          ) with
            AutoAddModules = false
        }

    let solution, _ = RoslynTestHelpers.CreateSolution project
    let context = new TestContext(solution)

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddReturnType())

    let expectedCode =
        """
module ModuleSecond

open ModuleFirst

let sum a b : MyType = {Value=a+b}
        """

    let resultText = newDoc.GetTextAsync() |> GetTaskResult
    Assert.Equal(expectedCode.Trim(' ', '\r', '\n'), resultText.ToString().Trim(' ', '\r', '\n'))
