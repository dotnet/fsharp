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
        """
module M

type Greeter() =
    member _.Greet(name: string, ?greeting: string) =
        let greeting = defaultArg greeting "Hello"
        $"{greeting}, {name}"

let a = Greeter().Greet("Ada")
let b = Greeter().Greet("Ada", greeting = "Hi")
"""

    let after =
        """
module M

open System.Runtime.InteropServices

type Greeter() =
    member _.Greet(name: string, [<Optional; DefaultParameterValue("Hello")>] greeting: string) =
        $"{greeting}, {name}"

let a = Greeter().Greet("Ada")
let b = Greeter().Greet("Ada", greeting = "Hi")
"""

    Assert.Equal(after, refactored before "?greeting")

[<Theory>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next(?step: int) =
        let step = defaultArg step 1
        step + 1

let n = Counter.Next()
""",
             """
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next([<Optional; DefaultParameterValue(1)>] step: int) =
        step + 1

let n = Counter.Next()
""")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next(value: int, ?step: float) =
        let step = defaultArg step 0.5
        float value + step
""",
             """
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next(value: int, [<Optional; DefaultParameterValue(0.5)>] step: float) =
        float value + step
""")>]
let ``Shadowing default converts both ways`` (fsharpForm: string, dotNetForm: string) =
    Assert.Equal(dotNetForm, refactored fsharpForm "?step")
    Assert.Equal(fsharpForm, refactored dotNetForm "step:")

[<Fact>]
let ``Inline defaults are replaced by the parameter`` () =
    let before =
        """
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next(value: int, ?step: int) = value + defaultArg step 1
"""

    let after =
        """
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next(value: int, [<Optional; DefaultParameterValue(1)>] step: int) = value + step
"""

    Assert.Equal(after, refactored before "?step")

[<Fact>]
let ``Body on the member line moves below it when converting back`` () =
    let before =
        """
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next(value: int, [<Optional; DefaultParameterValue(1)>] step: int) = value + step
"""

    let after =
        """
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next(value: int, ?step: int) =
        let step = defaultArg step 1
        value + step
"""

    Assert.Equal(after, refactored before "step:")

[<Theory>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type C() =
    static member M(?flag: bool) = 1
""",
             "?flag",
             """
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional>] flag: bool) = 1
""")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional>] step: int) =
        step + 1
""",
             "step:",
             """
module M

open System.Runtime.InteropServices

type C() =
    static member M(?step: int) =
        let step = defaultArg step Unchecked.defaultof<_>
        step + 1
""")>]
let ``Optional without a default value`` (before: string, marker: string, after: string) =
    Assert.Equal(after, refactored before marker)

let private fsharpForm =
    """
module M

type C() =
    static member M(?x: int) = defaultArg x 0
"""

[<Fact>]
let ``Title names the target form`` () =
    let dotNetForm =
        """
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional; DefaultParameterValue(0)>] x: int) = x
"""

    Assert.Equal("Use [<Optional; DefaultParameterValue>] for optional parameter", (actionsAt fsharpForm "?x" |> Seq.exactlyOne).Title)
    Assert.Equal("Use F# '?' optional parameter", (actionsAt dotNetForm "x:" |> Seq.exactlyOne).Title)

[<Theory>]
[<InlineData("""
module M

type C() =
    static member M(?x: int) = defaultArg x 0 + defaultArg x 1
""",
             "?x")>]
[<InlineData("""
module M

type C() =
    static member M(?x: int) = x.IsSome
""",
             "?x")>]
[<InlineData("""
module M

type C() =
    static member M(?x: obj) = defaultArg x (box 1)
""",
             "?x")>]
[<InlineData("""
module M

type C() =
    static member M(?x: int) = defaultArg x 1.0
""",
             "?x")>]
[<InlineData("""
module M

type C() =
    static member M(?x) = defaultArg x 0
""",
             "?x")>]
[<InlineData("""
module M

type C() =
    static member M([<Struct>] ?x: int) = defaultValueArg x 0
""",
             "?x")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional; In>] x: int) = x
""",
             "x:")>]
[<InlineData("""
module M

type B() =
    abstract M: ?x: int -> int
    default _.M(?x: int) = defaultArg x 0
""",
             "?x: int)")>]
[<InlineData("""
module M

type C() =
    static member M(x: int) = x
""",
             "x:")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)

[<Fact>]
let ``No action when the file has a signature`` () =
    let signature =
        """
module M

type C =
    new: unit -> C
    static member M: ?x: int -> int
"""

    let document =
        RoslynTestHelpers.GetFsiAndFsDocuments signature fsharpForm |> Seq.last

    let actions = ResizeArray<CodeAction>()

    let context =
        CodeRefactoringContext(document, TextSpan(caretAt fsharpForm "?x", 1), (fun action -> actions.Add action), CancellationToken.None)

    (new FSharpConvertOptionalParameterDefaultValueRefactoring()).ComputeRefactoringsAsync(context).GetAwaiter().GetResult()

    Assert.False(document.IsFSharpSignatureFile)
    Assert.Empty(actions)
