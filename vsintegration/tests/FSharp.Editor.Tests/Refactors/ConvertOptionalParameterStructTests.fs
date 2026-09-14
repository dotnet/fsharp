module FSharp.Editor.Tests.Refactors.ConvertOptionalParameterStructTests

open System
open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open Xunit

open FSharp.Compiler.Diagnostics

open FSharp.Editor.Tests.Helpers
open FSharp.Editor.Tests.Refactors.RefactorTestFramework

let private caretAt (code: string) (marker: string) =
    code.IndexOf(marker, StringComparison.Ordinal)

let private refactored (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code

    let document =
        tryRefactor code (caretAt code marker) context (new FSharpConvertOptionalParameterStructRefactoring())

    let _, checkResults =
        document.GetFSharpParseAndCheckResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty(
        checkResults.Diagnostics
        |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)
    )

    (document.GetTextAsync() |> GetTaskResult).ToString()

let private actionsIn (context: TestContext) (code: string) (marker: string) =
    tryGetRefactoringActions code (caretAt code marker) context (new FSharpConvertOptionalParameterStructRefactoring())

let private actionsAt (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code
    actionsIn context code marker

let private greeter =
    """
module M

type Greeter() =
    member _.Greet(name: string, ?greeting: string) =
        let greeting = defaultArg greeting "Hello"
        $"{greeting}, {name}"

let a = Greeter().Greet("Ada")
let b = Greeter().Greet("Ada", greeting = "Hi")
let c = Greeter().Greet("Ada", ?greeting = Some "Hey")
let d (g: string option) = Greeter().Greet("Ada", ?greeting = g)
"""

let private structGreeter =
    """
module M

type Greeter() =
    member _.Greet(name: string, [<Struct>] ?greeting: string) =
        let greeting = defaultValueArg greeting "Hello"
        $"{greeting}, {name}"

let a = Greeter().Greet("Ada")
let b = Greeter().Greet("Ada", greeting = "Hi")
let c = Greeter().Greet("Ada", ?greeting = ValueSome "Hey")
let d (g: string option) = Greeter().Greet("Ada", ?greeting = ValueOption.ofOption g)
"""

[<Fact>]
let ``Optional parameter and its optional arguments convert to value options`` () =
    Assert.Equal(structGreeter, refactored greeter "?greeting")

[<Fact>]
let ``Struct optional parameter and its optional arguments convert back to options`` () =
    Assert.Equal(greeter, refactored structGreeter "?greeting")

[<Theory>]
[<InlineData("""
module M

type Counter() =
    member _.Next(?step: int) =
        match step with
        | Some s -> s
        | None -> 1
""",
             """
module M

type Counter() =
    member _.Next([<Struct>] ?step: int) =
        match step with
        | ValueSome s -> s
        | ValueNone -> 1
""")>]
[<InlineData("""
module M

type Counter() =
    static member Describe(?step: int) =
        if Option.isSome step && step.IsSome then step |> Option.defaultValue 0 else 1
""",
             """
module M

type Counter() =
    static member Describe([<Struct>] ?step: int) =
        if ValueOption.isSome step && step.IsSome then step |> ValueOption.defaultValue 0 else 1
""")>]
[<InlineData("""
module M

type Counter() =
    static member Next(?step: int) = defaultArg step 1

let next (step: int option) = Counter.Next(?step = (if true then step else None))
""",
             """
module M

type Counter() =
    static member Next([<Struct>] ?step: int) = defaultValueArg step 1

let next (step: int option) = Counter.Next(?step = ValueOption.ofOption (if true then step else None))
""")>]
let ``Uses of the parameter in the member body follow the conversion`` (before: string, after: string) =
    Assert.Equal(after, refactored before "?step")
    Assert.Equal(before, refactored after "?step")

[<Fact>]
let ``Value option passed to a struct optional parameter is converted to an option`` () =
    let before =
        """
module M

type Counter() =
    static member Next([<Struct>] ?step: int) = defaultValueArg step 1

let next (step: int voption) = Counter.Next(?step = step)
"""

    let after =
        """
module M

type Counter() =
    static member Next(?step: int) = defaultArg step 1

let next (step: int voption) = Counter.Next(?step = ValueOption.toOption step)
"""

    Assert.Equal(after, refactored before "?step")

[<Fact>]
let ``Title names the target option kind`` () =
    Assert.Equal("Use 'voption' for optional parameter", (actionsAt greeter "?greeting" |> Seq.exactlyOne).Title)
    Assert.Equal("Use 'option' for optional parameter", (actionsAt structGreeter "?greeting" |> Seq.exactlyOne).Title)

let private counter =
    """
module M

type C() =
    static member M(?x: int) = defaultArg x 0
"""

[<Theory>]
[<InlineData("""
module M

type C() =
    static member M(?x: int) = printfn "%A" x
""",
             "?x")>]
[<InlineData("""
module M

type C() =
    static member M(?x: int) = Option.map string x
""",
             "?x")>]
[<InlineData("""
module M

type B() =
    abstract M: ?x: int -> int
    default _.M(?x) = defaultArg x 0
""",
             "?x)")>]
[<InlineData("""
module M

type C() =
    static member M(?x: int) = defaultArg x 0
""",
             "M(")>]
[<InlineData("""
module M

type C() =
    static member M(x: int option) = defaultArg x 0
""",
             "x:")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)

[<Fact>]
let ``Value option is not offered before F# 10`` () =
    use context =
        new TestContext(RoslynTestHelpers.CreateSolution(counter, extraFSharpProjectOtherOptions = [| "--langversion:9.0" |]))

    Assert.Empty(actionsIn context counter "?x")

[<Fact>]
let ``No action when the file has a signature`` () =
    let signature =
        """
module M

type C =
    new: unit -> C
    static member M: ?x: int -> int
"""

    let document = RoslynTestHelpers.GetFsiAndFsDocuments signature counter |> Seq.last
    let actions = ResizeArray<CodeAction>()

    let context =
        CodeRefactoringContext(document, TextSpan(caretAt counter "?x", 1), (fun action -> actions.Add action), CancellationToken.None)

    (new FSharpConvertOptionalParameterStructRefactoring()).ComputeRefactoringsAsync(context).GetAwaiter().GetResult()

    Assert.False(document.IsFSharpSignatureFile)
    Assert.Empty(actions)
