module FSharp.Editor.Tests.Refactors.AddExplicitReturnTypeTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit
open System.Linq
open FSharp.Editor.Tests.Refactors.RefactorTestFramework
open FSharp.Test.ProjectGeneration
open FSharp.Editor.Tests.Helpers
open System.Threading

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

    let actions =
        tryGetRefactoringActions code spanStart context (new AddExplicitReturnType())

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
    
    let actions =
        tryGetRefactoringActions code spanStart context (new AddExplicitReturnType())
    
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
    
    let actions =
        tryGetRefactoringActions code spanStart context (new AddExplicitReturnType())
    
    do Assert.Empty(actions)

[<Fact>]
let ``Correctly infer int as explicit return type`` () =
    let symbolName = "sum"

    let code =
        """
let sum a b = a + b
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddExplicitReturnType())

    let expectedCode =
        $"""
let sum a b : int= a + b
        """

    let resultText = newDoc.GetTextAsync context.CT |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Handle Parantheses on the arguments`` () =
    let symbolName = "sum"

    let code =
        """
let sum (a:float) (b:float) = a + b
        """

    use context = TestContext.CreateWithCode code

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddExplicitReturnType())

    let expectedCode =
        """
let sum (a:float) (b:float) : float= a + b
        """

    let resultText = newDoc.GetTextAsync context.CT |> GetTaskResult
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

    let newDoc = tryRefactor code spanStart context (new AddExplicitReturnType())

    let expectedCode =
        $"""
let rec fib n : int=
    if n < 2 then 1
    else fib (n - 1) + fib (n - 2)
        """

    let resultText = newDoc.GetTextAsync context.CT |> GetTaskResult
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

    let newDoc = tryRefactor code spanStart context (new AddExplicitReturnType())

    let expectedCode =
        $"""
let apply1 (transform: int -> int) y : int= transform y
        """

    let resultText = newDoc.GetTextAsync context.CT |> GetTaskResult
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

    let newDoc = tryRefactor code spanStart context (new AddExplicitReturnType())

    let expectedCode =
        $"""
type SomeType(factor0: int) =
    let factor = factor0
    member this.SomeMethod(a, b, c) : int= (a + b + c) * factor
        """

    let resultText = newDoc.GetTextAsync context.CT |> GetTaskResult
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

    let newDoc = tryRefactor code spanStart context (new AddExplicitReturnType())

    let expectedCode =
        """
type MyType = { Value: int }
let sum a b : MyType= {Value=a+b}
        """

    let resultText = newDoc.GetTextAsync context.CT |> GetTaskResult
    Assert.Equal(expectedCode, resultText.ToString())

[<Fact>]
let ``Correctly infer custom type that is declared earlier in project`` () =
    let symbolName = "sum"

    let myModule =
        """
type MyType = { Value: int }
        """

    let code =
        """
open ModuleFirst

let sum a b = {Value=a+b}
        """

    let project =
           SyntheticProject.Create(
               { sourceFile "First" [] with
                   Source = myModule 
                   SignatureFile = AutoGenerated
               },
               { sourceFile "Second" ["First"] with
                   Source = code
               }
           )

    project.SaveAndCheck() |> ignore
    let solution, _ = RoslynTestHelpers.CreateSolution project
    let ct = CancellationToken false
    let context = new TestContext(solution, ct)

    let spanStart = code.IndexOf symbolName

    let newDoc = tryRefactor code spanStart context (new AddExplicitReturnType())

    let expectedCode =
        """
open ModuleFirst

let sum a b : MyType= {Value=a+b}
        """

    let resultText = newDoc.GetTextAsync context.CT |> GetTaskResult
    Assert.Equal(expectedCode.Trim(' ','\r','\n'), resultText.ToString().Trim(' ','\r','\n'))
