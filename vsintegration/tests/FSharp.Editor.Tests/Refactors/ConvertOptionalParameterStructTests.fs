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
    "module M\n\ntype Greeter() =\n    member _.Greet(name: string, ?greeting: string) =\n        let greeting = defaultArg greeting \"Hello\"\n        $\"{greeting}, {name}\"\n\nlet a = Greeter().Greet(\"Ada\")\nlet b = Greeter().Greet(\"Ada\", greeting = \"Hi\")\nlet c = Greeter().Greet(\"Ada\", ?greeting = Some \"Hey\")\nlet d (g: string option) = Greeter().Greet(\"Ada\", ?greeting = g)\n"

let private structGreeter =
    "module M\n\ntype Greeter() =\n    member _.Greet(name: string, [<Struct>] ?greeting: string) =\n        let greeting = defaultValueArg greeting \"Hello\"\n        $\"{greeting}, {name}\"\n\nlet a = Greeter().Greet(\"Ada\")\nlet b = Greeter().Greet(\"Ada\", greeting = \"Hi\")\nlet c = Greeter().Greet(\"Ada\", ?greeting = ValueSome \"Hey\")\nlet d (g: string option) = Greeter().Greet(\"Ada\", ?greeting = ValueOption.ofOption g)\n"

[<Fact>]
let ``Optional parameter and its optional arguments convert to value options`` () =
    Assert.Equal(structGreeter, refactored greeter "?greeting")

[<Fact>]
let ``Struct optional parameter and its optional arguments convert back to options`` () =
    Assert.Equal(greeter, refactored structGreeter "?greeting")

[<Theory>]
[<InlineData("module M\n\ntype Counter() =\n    member _.Next(?step: int) =\n        match step with\n        | Some s -> s\n        | None -> 1\n",
             "module M\n\ntype Counter() =\n    member _.Next([<Struct>] ?step: int) =\n        match step with\n        | ValueSome s -> s\n        | ValueNone -> 1\n")>]
[<InlineData("module M\n\ntype Counter() =\n    static member Describe(?step: int) =\n        if Option.isSome step && step.IsSome then step |> Option.defaultValue 0 else 1\n",
             "module M\n\ntype Counter() =\n    static member Describe([<Struct>] ?step: int) =\n        if ValueOption.isSome step && step.IsSome then step |> ValueOption.defaultValue 0 else 1\n")>]
[<InlineData("module M\n\ntype Counter() =\n    static member Next(?step: int) = defaultArg step 1\n\nlet next (step: int option) = Counter.Next(?step = (if true then step else None))\n",
             "module M\n\ntype Counter() =\n    static member Next([<Struct>] ?step: int) = defaultValueArg step 1\n\nlet next (step: int option) = Counter.Next(?step = ValueOption.ofOption (if true then step else None))\n")>]
let ``Uses of the parameter in the member body follow the conversion`` (before: string, after: string) =
    Assert.Equal(after, refactored before "?step")
    Assert.Equal(before, refactored after "?step")

[<Fact>]
let ``Value option passed to a struct optional parameter is converted to an option`` () =
    let before =
        "module M\n\ntype Counter() =\n    static member Next([<Struct>] ?step: int) = defaultValueArg step 1\n\nlet next (step: int voption) = Counter.Next(?step = step)\n"

    let after =
        "module M\n\ntype Counter() =\n    static member Next(?step: int) = defaultArg step 1\n\nlet next (step: int voption) = Counter.Next(?step = ValueOption.toOption step)\n"

    Assert.Equal(after, refactored before "?step")

[<Fact>]
let ``Title names the target option kind`` () =
    Assert.Equal("Use 'voption' for optional parameter", (actionsAt greeter "?greeting" |> Seq.exactlyOne).Title)
    Assert.Equal("Use 'option' for optional parameter", (actionsAt structGreeter "?greeting" |> Seq.exactlyOne).Title)

[<Theory>]
[<InlineData("module M\n\ntype C() =\n    static member M(?x: int) = printfn \"%A\" x\n", "?x")>]
[<InlineData("module M\n\ntype C() =\n    static member M(?x: int) = Option.map string x\n", "?x")>]
[<InlineData("module M\n\ntype B() =\n    abstract M: ?x: int -> int\n    default _.M(?x) = defaultArg x 0\n", "?x)")>]
[<InlineData("module M\n\ntype C() =\n    static member M(?x: int) = defaultArg x 0\n", "M(")>]
[<InlineData("module M\n\ntype C() =\n    static member M(x: int option) = defaultArg x 0\n", "x:")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)

[<Fact>]
let ``Value option is not offered before F# 10`` () =
    let code = "module M\n\ntype C() =\n    static member M(?x: int) = defaultArg x 0\n"

    use context =
        new TestContext(RoslynTestHelpers.CreateSolution(code, extraFSharpProjectOtherOptions = [| "--langversion:9.0" |]))

    Assert.Empty(actionsIn context code "?x")

[<Fact>]
let ``No action when the file has a signature`` () =
    let code = "module M\n\ntype C() =\n    static member M(?x: int) = defaultArg x 0\n"

    let signature =
        "module M\n\ntype C =\n    new: unit -> C\n    static member M: ?x: int -> int\n"

    let document = RoslynTestHelpers.GetFsiAndFsDocuments signature code |> Seq.last
    let actions = ResizeArray<CodeAction>()

    let context =
        CodeRefactoringContext(document, TextSpan(caretAt code "?x", 1), (fun action -> actions.Add action), CancellationToken.None)

    (new FSharpConvertOptionalParameterStructRefactoring()).ComputeRefactoringsAsync(context).GetAwaiter().GetResult()

    Assert.False(document.IsFSharpSignatureFile)
    Assert.Empty(actions)
