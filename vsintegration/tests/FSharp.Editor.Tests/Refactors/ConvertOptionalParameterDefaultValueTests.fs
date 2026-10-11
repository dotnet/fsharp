module FSharp.Editor.Tests.Refactors.ConvertOptionalParameterDefaultValueTests

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
open FSharp.Test.ProjectGeneration

let private caretAt (code: string) (marker: string) =
    code.IndexOf(marker, StringComparison.Ordinal)

let private fsharpTitle = "Use F# '?' optional parameter"
let private structTitle = "Use F# '[<Struct>] ?' optional parameter"

let private hasTitle (title: string) (action: CodeAction) =
    String.Equals(action.Title, title, StringComparison.Ordinal)

let private textOf (document: Document) =
    (document.GetTextAsync() |> GetTaskResult).ToString()

let private assertNoErrors (document: Document) =
    let _, checkResults =
        document.GetFSharpParseAndCheckResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty(
        checkResults.Diagnostics
        |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)
    )

let private refactoredBy (pick: CodeAction seq -> CodeAction) (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code

    let action =
        tryGetRefactoringActions code (caretAt code marker) context (new FSharpConvertOptionalParameterDefaultValueRefactoring())
        |> pick

    for operation in action.GetOperationsAsync CancellationToken.None |> GetTaskResult do
        let applyChanges = operation :?> ApplyChangesOperation
        applyChanges.Apply(context.Solution.Workspace, CancellationToken.None)
        context.Solution <- applyChanges.ChangedSolution

    let document = RoslynTestHelpers.GetLastDocument context.Solution
    assertNoErrors document
    textOf document

let private refactored (code: string) (marker: string) = refactoredBy Seq.head code marker

let private refactoredWith (title: string) (code: string) (marker: string) =
    refactoredBy (Seq.find (hasTitle title)) code marker

let private actionsAt (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code
    tryGetRefactoringActions code (caretAt code marker) context (new FSharpConvertOptionalParameterDefaultValueRefactoring())

let private actionsIn (document: Document) (marker: string) =
    let actions = ResizeArray<CodeAction>()

    let context =
        CodeRefactoringContext(
            document,
            TextSpan(caretAt (textOf document) marker, 1),
            (fun action -> actions.Add action),
            CancellationToken.None
        )

    (new FSharpConvertOptionalParameterDefaultValueRefactoring()).ComputeRefactoringsAsync(context).GetAwaiter().GetResult()

    actions

let private documentOf (solution: Solution) (project: SyntheticProject) (fileId: string) =
    solution.GetDocumentIdsWithFilePath(project.GetFilePath fileId)
    |> Seq.exactlyOne
    |> solution.GetDocument

let private fileOf (fileId: string) (source: string) =
    { sourceFile fileId [] with
        Source = source
    }

[<Theory>]
[<InlineData("""
module M

type Greeter() =
    member _.Greet(name: string, ?greeting: string) =
        let greeting = defaultArg greeting "Hello"
        $"{greeting}, {name}"

let a = Greeter().Greet("Ada")
let b = Greeter().Greet("Ada", greeting = "Hi")
""",
             """
module M

type Greeter() =
    member _.Greet(name: string, [<System.Runtime.InteropServices.Optional; System.Runtime.InteropServices.DefaultParameterValue("Hello")>] greeting: string) =
        $"{greeting}, {name}"

let a = Greeter().Greet("Ada")
let b = Greeter().Greet("Ada", greeting = "Hi")
""")>]
[<InlineData("""
module M

type OptionalAttribute() =
    inherit System.Attribute()

type Greeter() =
    member _.Greet(?greeting: string) = defaultArg greeting "Hello"

let a = Greeter().Greet()
""",
             """
module M

type OptionalAttribute() =
    inherit System.Attribute()

type Greeter() =
    member _.Greet([<System.Runtime.InteropServices.Optional; System.Runtime.InteropServices.DefaultParameterValue("Hello")>] greeting: string) = greeting

let a = Greeter().Greet()
""")>]
let ``Attributes are fully qualified where System.Runtime.InteropServices is not open`` (before: string, after: string) =
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
[<InlineData("""
module M

open System.Runtime.InteropServices

type Step = System.Int32

type Counter() =
    static member Next(?step: Step) =
        let step = defaultArg step 1
        step + 1
""",
             """
module M

open System.Runtime.InteropServices

type Step = System.Int32

type Counter() =
    static member Next([<Optional; DefaultParameterValue(1)>] step: Step) =
        step + 1
""")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

let defaultArg (_: int option) (fallback: int) = fallback + 1

type Counter() =
    static member Next(?step: int) =
        let step = Microsoft.FSharp.Core.Operators.defaultArg step 1
        step + 1
""",
             """
module M

open System.Runtime.InteropServices

let defaultArg (_: int option) (fallback: int) = fallback + 1

type Counter() =
    static member Next([<Optional; DefaultParameterValue(1)>] step: int) =
        step + 1
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
[<InlineData("""
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional>] step: int) = (* keep this *) step + 1
""",
             "step:",
             """
module M

open System.Runtime.InteropServices

type C() =
    static member M(?step: int) = (* keep this *)
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

let private dotNetForm =
    """
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional; DefaultParameterValue(0)>] x: int) = x
"""

let private titlesOf (actions: CodeAction seq) =
    actions |> Seq.map _.Title |> List.ofSeq

[<Fact>]
let ``Title names the target form`` () =
    Assert.Equal("Use [<Optional; DefaultParameterValue>] for optional parameter", (actionsAt fsharpForm "?x" |> Seq.exactlyOne).Title)
    Assert.Equal<string list>([ fsharpTitle; structTitle ], titlesOf (actionsAt dotNetForm "x:"))

[<Theory>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next(value: int, [<Optional; DefaultParameterValue(1)>] step: int) =
        value + step
""",
             """
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next(value: int, [<Struct>] ?step: int) =
        let step = defaultValueArg step 1
        value + step
""")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional>] step: int) =
        step + 1
""",
             """
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Struct>] ?step: int) =
        let step = defaultValueArg step Unchecked.defaultof<_>
        step + 1
""")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

let defaultValueArg (_: int voption) (fallback: int) = fallback + 1

type C() =
    static member M([<Optional; DefaultParameterValue(1)>] step: int) =
        step + 1
""",
             """
module M

open System.Runtime.InteropServices

let defaultValueArg (_: int voption) (fallback: int) = fallback + 1

type C() =
    static member M([<Struct>] ?step: int) =
        let step = Microsoft.FSharp.Core.Operators.defaultValueArg step 1
        step + 1
""")>]
let ``Converting back to a struct optional parameter uses defaultValueArg`` (dotNetForm: string, structForm: string) =
    Assert.Equal(structForm, refactoredWith structTitle dotNetForm "step:")

[<Fact>]
let ``Struct optional parameter is not offered before F# 10`` () =
    use context =
        new TestContext(RoslynTestHelpers.CreateSolution(dotNetForm, extraFSharpProjectOtherOptions = [| "--langversion:9.0" |]))

    let actions =
        tryGetRefactoringActions dotNetForm (caretAt dotNetForm "x:") context (new FSharpConvertOptionalParameterDefaultValueRefactoring())

    Assert.Equal<string list>([ fsharpTitle ], titlesOf actions)

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
[<InlineData("""
module M

let defaultArg (x: int option) (fallback: int) = fallback

type C() =
    static member M(?x: int) = defaultArg x 0
""",
             "?x")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional>] x: obj) = x
""",
             "x:")>]
[<InlineData("""
module M

type C() =
    static member M(?x: int, ?y: int) = defaultArg x 0 + defaultArg y 1
""",
             "?y")>]
[<InlineData("""
module M

type OptionalAttribute() =
    inherit System.Attribute()

type C() =
    static member M([<Optional>] x: int) = x
""",
             "x:")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type DefaultParameterValueAttribute(value: int) =
    inherit System.Attribute()

type C() =
    static member M([<Optional; DefaultParameterValue(1)>] x: int) = x
""",
             "x:")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional; DefaultParameterValue(1)>] x: string) = x
""",
             "x:")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional>] x: int, y: int) = x + y
""",
             "x:")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional>] x: outref<bool>) = x <- true
""",
             "x:")>]
[<InlineData("""
module M

open System.Runtime.CompilerServices
open System.Runtime.InteropServices

[<Struct; IsByRefLike>]
type S =
    val Value: int

type C() =
    static member M([<Optional>] x: S) = x.Value
""",
             "x:")>]
[<InlineData("""
module M

open System
open System.Runtime.InteropServices

type C() =
    static member M([<ParamArray>] [<Optional>] x: int[]) = x.Length
""",
             "x:")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)

[<Fact>]
let ``Mutable shadowing keeps its binding`` () =
    let before =
        """
module M

open System.Runtime.InteropServices

type C() =
    static member M(?x: int) =
        let mutable x = defaultArg x 0
        x <- x + 1
        x
"""

    let after =
        """
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional; DefaultParameterValue(0)>] x: int) =
        let mutable x = x
        x <- x + 1
        x
"""

    Assert.Equal(after, refactored before "?x")

[<Fact>]
let ``Open in another module does not count as in scope`` () =
    let before =
        """
module M

module Other =
    open System.Runtime.InteropServices

    let o = 1

type C() =
    static member M(?x: int) = defaultArg x 0
"""

    let after =
        """
module M

module Other =
    open System.Runtime.InteropServices

    let o = 1

type C() =
    static member M([<System.Runtime.InteropServices.Optional; System.Runtime.InteropServices.DefaultParameterValue(0)>] x: int) = x
"""

    Assert.Equal(after, refactored before "?x")

[<Fact>]
let ``Open in an enclosing module counts as in scope`` () =
    let before =
        """
module M

module Inner =
    open System.Runtime.InteropServices

    type C() =
        static member M(?x: int) = defaultArg x 0
"""

    let after =
        """
module M

module Inner =
    open System.Runtime.InteropServices

    type C() =
        static member M([<Optional; DefaultParameterValue(0)>] x: int) = x
"""

    Assert.Equal(after, refactored before "?x")

[<Theory>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next([<Optional; DefaultParameterValue(1)>] step: int) =
        step + 1

let a = Counter.Next()
let b = Counter.Next(?step = Some 2)
let c = Counter.Next(?step = None)
let d (s: int option) = Counter.Next(?step = s)
let e (s: int list) = Counter.Next(?step = List.tryHead s)
""",
             """
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next([<Struct>] ?step: int) =
        let step = defaultValueArg step 1
        step + 1

let a = Counter.Next()
let b = Counter.Next(?step = ValueSome 2)
let c = Counter.Next(?step = ValueNone)
let d (s: int option) = Counter.Next(?step = ValueOption.ofOption s)
let e (s: int list) = Counter.Next(?step = ValueOption.ofOption (List.tryHead s))
""")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next<'T>([<Optional; DefaultParameterValue(1)>] step: int) =
        step + sizeof<'T>

let a = Counter.Next<string>(?step = Some 2)
""",
             """
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next<'T>([<Struct>] ?step: int) =
        let step = defaultValueArg step 1
        step + sizeof<'T>

let a = Counter.Next<string>(?step = ValueSome 2)
""")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next([<Optional; DefaultParameterValue(1)>] step: int) =
        step + 1

let a = Counter.Next(?step = ``Some`` 2)
let b = Counter.Next(?step = ``None``)
""",
             """
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next([<Struct>] ?step: int) =
        let step = defaultValueArg step 1
        step + 1

let a = Counter.Next(?step = ValueSome 2)
let b = Counter.Next(?step = ValueNone)
""")>]
[<InlineData("""
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next([<Optional; DefaultParameterValue(1)>] step: int) =
        step + 1

type Shadow =
    | ValueSome of int
    | ValueNone

let a = Counter.Next(?step = Some 2)
let b = Counter.Next(?step = None)
""",
             """
module M

open System.Runtime.InteropServices

type Counter() =
    static member Next([<Struct>] ?step: int) =
        let step = defaultValueArg step 1
        step + 1

type Shadow =
    | ValueSome of int
    | ValueNone

let a = Counter.Next(?step = Microsoft.FSharp.Core.ValueOption.ValueSome 2)
let b = Counter.Next(?step = Microsoft.FSharp.Core.ValueOption.ValueNone)
""")>]
let ``Converting to a struct optional parameter converts optional arguments to value options`` (before: string, after: string) =
    Assert.Equal(after, refactoredWith structTitle before "step:")

[<Fact>]
let ``Escaped parameter name keeps its backticks`` () =
    let fsharpForm =
        """
module M

open System.Runtime.InteropServices

type C() =
    static member M(?``type``: int) = defaultArg ``type`` 7
"""

    let dotNetForm =
        """
module M

open System.Runtime.InteropServices

type C() =
    static member M([<Optional; DefaultParameterValue(7)>] ``type``: int) = ``type``
"""

    let shadowingForm =
        """
module M

open System.Runtime.InteropServices

type C() =
    static member M(?``type``: int) =
        let ``type`` = defaultArg ``type`` 7
        ``type``
"""

    Assert.Equal(dotNetForm, refactored fsharpForm "?``type``")
    Assert.Equal(shadowingForm, refactored dotNetForm "``type``:")

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

    Assert.False(document.IsFSharpSignatureFile)
    Assert.Empty(actionsIn document "?x")

let private counterDefinition =
    """
module Definition

open System.Runtime.InteropServices

type Counter() =
    static member Next([<Optional; DefaultParameterValue(1)>] step: int) =
        step + 1
"""

let private counterCallers =
    """
module Callers

let a = Definition.Counter.Next()
let b = Definition.Counter.Next(?step = Some 2)
"""

let private inOneProject (definition: string) (callers: string) =
    { SyntheticProject.Create(fileOf "Definition" definition, fileOf "Callers" callers) with
        AutoAddModules = false
    }

[<Fact>]
let ``Converting to a struct optional parameter converts optional arguments in another file`` () =
    let project = inOneProject counterDefinition counterCallers
    let solution, _ = RoslynTestHelpers.CreateSolution project
    use _ = new TestContext(solution)

    let action =
        actionsIn (documentOf solution project "Definition") "step:"
        |> Seq.find (hasTitle structTitle)

    let operation =
        action.GetOperationsAsync CancellationToken.None
        |> GetTaskResult
        |> Seq.exactlyOne

    let changed = (operation :?> ApplyChangesOperation).ChangedSolution

    let definition = textOf (documentOf changed project "Definition")
    let callers = textOf (documentOf changed project "Callers")

    Assert.Equal(
        """
module Definition

open System.Runtime.InteropServices

type Counter() =
    static member Next([<Struct>] ?step: int) =
        let step = defaultValueArg step 1
        step + 1
""",
        definition
    )

    Assert.Equal(
        """
module Callers

let a = Definition.Counter.Next()
let b = Definition.Counter.Next(?step = ValueSome 2)
""",
        callers
    )

    // The checker reads the other file of a synthetic project from disk, so the result is checked as a project of its own.
    let checkedSolution, _ =
        RoslynTestHelpers.CreateSolution(inOneProject definition callers)

    use _ = new TestContext(checkedSolution)

    for document in (Seq.exactlyOne checkedSolution.Projects).Documents do
        assertNoErrors document

[<Theory>]
[<InlineData("--langversion:9.0", false)>]
[<InlineData("--langversion:10.0", true)>]
let ``Struct optional parameter is offered only when every dependent F# project supports it``
    (dependentLangVersion: string, isOffered: bool)
    =
    let library =
        { SyntheticProject.Create("Library", fileOf "Definition" counterDefinition) with
            AutoAddModules = false
        }

    let app =
        { SyntheticProject.Create("App", fileOf "Callers" counterCallers) with
            AutoAddModules = false
            DependsOn = [ library ]
            OtherOptions = [ dependentLangVersion ]
        }

    let struct (solution, _) = RoslynTestHelpers.CreateMultiProjectSolution app
    use _ = new TestContext(solution)

    let titles =
        actionsIn (documentOf solution library "Definition") "step:" |> titlesOf

    Assert.Equal<string list>(
        [
            fsharpTitle
            if isOffered then
                structTitle
        ],
        titles
    )
