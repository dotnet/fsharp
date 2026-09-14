module FSharp.Editor.Tests.Refactors.ConvertOptionalParameterDefaultValueTests

open System
open System.Threading

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
        tryRefactor code (caretAt code marker) context (new FSharpConvertOptionalParameterDefaultValueRefactoring())

    let _, checkResults =
        document.GetFSharpParseAndCheckResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty(
        checkResults.Diagnostics
        |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)
    )

    (document.GetTextAsync() |> GetTaskResult).ToString()

let private actionsAt (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code
    tryGetRefactoringActions code (caretAt code marker) context (new FSharpConvertOptionalParameterDefaultValueRefactoring())

[<Fact>]
let ``Optional parameter with a default value becomes a .NET optional parameter and the open is added`` () =
    let before =
        "module M\n\ntype Greeter() =\n    member _.Greet(name: string, ?greeting: string) =\n        let greeting = defaultArg greeting \"Hello\"\n        $\"{greeting}, {name}\"\n\nlet a = Greeter().Greet(\"Ada\")\nlet b = Greeter().Greet(\"Ada\", greeting = \"Hi\")\n"

    let after =
        "module M\n\nopen System.Runtime.InteropServices\n\ntype Greeter() =\n    member _.Greet(name: string, [<Optional; DefaultParameterValue(\"Hello\")>] greeting: string) =\n        $\"{greeting}, {name}\"\n\nlet a = Greeter().Greet(\"Ada\")\nlet b = Greeter().Greet(\"Ada\", greeting = \"Hi\")\n"

    Assert.Equal(after, refactored before "?greeting")

[<Theory>]
[<InlineData("module M\n\nopen System.Runtime.InteropServices\n\ntype Counter() =\n    static member Next(?step: int) =\n        let step = defaultArg step 1\n        step + 1\n\nlet n = Counter.Next()\n",
             "module M\n\nopen System.Runtime.InteropServices\n\ntype Counter() =\n    static member Next([<Optional; DefaultParameterValue(1)>] step: int) =\n        step + 1\n\nlet n = Counter.Next()\n")>]
[<InlineData("module M\n\nopen System.Runtime.InteropServices\n\ntype Counter() =\n    static member Next(value: int, ?step: float) =\n        let step = defaultArg step 0.5\n        float value + step\n",
             "module M\n\nopen System.Runtime.InteropServices\n\ntype Counter() =\n    static member Next(value: int, [<Optional; DefaultParameterValue(0.5)>] step: float) =\n        float value + step\n")>]
let ``Shadowing default converts both ways`` (fsharpForm: string, dotNetForm: string) =
    Assert.Equal(dotNetForm, refactored fsharpForm "?step")
    Assert.Equal(fsharpForm, refactored dotNetForm "step:")

[<Fact>]
let ``Inline defaults are replaced by the parameter`` () =
    let before =
        "module M\n\nopen System.Runtime.InteropServices\n\ntype Counter() =\n    static member Next(value: int, ?step: int) = value + defaultArg step 1\n"

    let after =
        "module M\n\nopen System.Runtime.InteropServices\n\ntype Counter() =\n    static member Next(value: int, [<Optional; DefaultParameterValue(1)>] step: int) = value + step\n"

    Assert.Equal(after, refactored before "?step")

[<Fact>]
let ``Body on the member line moves below it when converting back`` () =
    let before =
        "module M\n\nopen System.Runtime.InteropServices\n\ntype Counter() =\n    static member Next(value: int, [<Optional; DefaultParameterValue(1)>] step: int) = value + step\n"

    let after =
        "module M\n\nopen System.Runtime.InteropServices\n\ntype Counter() =\n    static member Next(value: int, ?step: int) =\n        let step = defaultArg step 1\n        value + step\n"

    Assert.Equal(after, refactored before "step:")

[<Theory>]
[<InlineData("module M\n\nopen System.Runtime.InteropServices\n\ntype C() =\n    static member M(?flag: bool) = 1\n",
             "?flag",
             "module M\n\nopen System.Runtime.InteropServices\n\ntype C() =\n    static member M([<Optional>] flag: bool) = 1\n")>]
[<InlineData("module M\n\nopen System.Runtime.InteropServices\n\ntype C() =\n    static member M([<Optional>] step: int) =\n        step + 1\n",
             "step:",
             "module M\n\nopen System.Runtime.InteropServices\n\ntype C() =\n    static member M(?step: int) =\n        let step = defaultArg step Unchecked.defaultof<_>\n        step + 1\n")>]
let ``Optional without a default value`` (before: string, marker: string, after: string) =
    Assert.Equal(after, refactored before marker)

[<Fact>]
let ``Title names the target form`` () =
    let fsharpForm =
        "module M\n\ntype C() =\n    static member M(?x: int) = defaultArg x 0\n"

    let dotNetForm =
        "module M\n\nopen System.Runtime.InteropServices\n\ntype C() =\n    static member M([<Optional; DefaultParameterValue(0)>] x: int) = x\n"

    Assert.Equal("Use [<Optional; DefaultParameterValue>] for optional parameter", (actionsAt fsharpForm "?x" |> Seq.exactlyOne).Title)
    Assert.Equal("Use F# '?' optional parameter", (actionsAt dotNetForm "x:" |> Seq.exactlyOne).Title)

[<Theory>]
[<InlineData("module M\n\ntype C() =\n    static member M(?x: int) = defaultArg x 0 + defaultArg x 1\n", "?x")>]
[<InlineData("module M\n\ntype C() =\n    static member M(?x: int) = x.IsSome\n", "?x")>]
[<InlineData("module M\n\ntype C() =\n    static member M(?x: obj) = defaultArg x (box 1)\n", "?x")>]
[<InlineData("module M\n\ntype C() =\n    static member M(?x: int) = defaultArg x 1.0\n", "?x")>]
[<InlineData("module M\n\ntype C() =\n    static member M(?x) = defaultArg x 0\n", "?x")>]
[<InlineData("module M\n\ntype C() =\n    static member M([<Struct>] ?x: int) = defaultValueArg x 0\n", "?x")>]
[<InlineData("module M\n\nopen System.Runtime.InteropServices\n\ntype C() =\n    static member M([<Optional; In>] x: int) = x\n", "x:")>]
[<InlineData("module M\n\ntype B() =\n    abstract M: ?x: int -> int\n    default _.M(?x: int) = defaultArg x 0\n", "?x: int)")>]
[<InlineData("module M\n\ntype C() =\n    static member M(x: int) = x\n", "x:")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)

[<Fact>]
let ``No action when the file has a signature`` () =
    let code = "module M\n\ntype C() =\n    static member M(?x: int) = defaultArg x 0\n"

    let signature =
        "module M\n\ntype C =\n    new: unit -> C\n    static member M: ?x: int -> int\n"

    let document = RoslynTestHelpers.GetFsiAndFsDocuments signature code |> Seq.last
    let actions = ResizeArray<CodeAction>()

    let context =
        CodeRefactoringContext(document, TextSpan(caretAt code "?x", 1), (fun action -> actions.Add action), CancellationToken.None)

    (new FSharpConvertOptionalParameterDefaultValueRefactoring()).ComputeRefactoringsAsync(context).GetAwaiter().GetResult()

    Assert.False(document.IsFSharpSignatureFile)
    Assert.Empty(actions)
