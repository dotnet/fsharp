module FSharp.Editor.Tests.Refactors.ConvertOptionalParameterStructTests

open System
open System.Threading
open System.Threading.Tasks

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

let private newLineOf (code: string) =
    if code.IndexOf('\r') >= 0 then "\r\n" else "\n"

let private moduleM (code: string) =
    let newLine = newLineOf code
    $"{newLine}module M{newLine}{newLine}{code}{newLine}"

let private caretAt (code: string) (marker: string) =
    code.IndexOf(marker, StringComparison.Ordinal)

let private assertRefactored (marker: string) (before: string) (after: string) =
    let code = moduleM before
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

    Assert.Equal(moduleM after, (document.GetTextAsync() |> GetTaskResult).ToString())

let private actionsIn (context: TestContext) (code: string) (marker: string) =
    tryGetRefactoringActions code (caretAt code marker) context (new FSharpConvertOptionalParameterStructRefactoring())

let private actionsAt (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code
    actionsIn context code marker

let private actionsOf (document: Document) (position: int) =
    let actions = ResizeArray<CodeAction>()

    let context =
        CodeRefactoringContext(document, TextSpan(position, 1), (fun action -> actions.Add action), CancellationToken.None)

    (new FSharpConvertOptionalParameterStructRefactoring()).ComputeRefactoringsAsync(context).GetAwaiter().GetResult()

    actions

[<Literal>]
let private greeter =
    """type Greeter() =
    member _.Greet(name: string, ?greeting: string) =
        let greeting = defaultArg greeting "Hello"
        $"{greeting}, {name}"

let a = Greeter().Greet("Ada")
let b = Greeter().Greet("Ada", greeting = "Hi")
let c = Greeter().Greet("Ada", ?greeting = Some "Hey")
let d (g: string option) = Greeter().Greet("Ada", ?greeting = g)"""

[<Literal>]
let private structGreeter =
    """type Greeter() =
    member _.Greet(name: string, [<Struct>] ?greeting: string) =
        let greeting = defaultValueArg greeting "Hello"
        $"{greeting}, {name}"

let a = Greeter().Greet("Ada")
let b = Greeter().Greet("Ada", greeting = "Hi")
let c = Greeter().Greet("Ada", ?greeting = ValueSome "Hey")
let d (g: string option) = Greeter().Greet("Ada", ?greeting = ValueOption.ofOption g)"""

let private counter =
    moduleM
        """type C() =
    static member M(?x: int) = defaultArg x 0"""

[<Theory>]
[<InlineData("match cases",
             """type Counter() =
    member _.Next(?step: int) =
        match step with
        | Some s -> s
        | None -> 1""",
             """type Counter() =
    member _.Next([<Struct>] ?step: int) =
        match step with
        | ValueSome s -> s
        | ValueNone -> 1""")>]
[<InlineData("module functions and properties",
             """type Counter() =
    static member Describe(?step: int) =
        if Option.isSome step && step.IsSome then step |> Option.defaultValue 0 else 1""",
             """type Counter() =
    static member Describe([<Struct>] ?step: int) =
        if ValueOption.isSome step && step.IsSome then step |> ValueOption.defaultValue 0 else 1""")>]
[<InlineData("expression argument",
             """type Counter() =
    static member Next(?step: int) = defaultArg step 1

let next (step: int option) = Counter.Next(?step = (if true then step else None))""",
             """type Counter() =
    static member Next([<Struct>] ?step: int) = defaultValueArg step 1

let next (step: int option) = Counter.Next(?step = ValueOption.ofOption (if true then step else None))""")>]
let ``Uses of the parameter in the member body follow the conversion`` (_scenario: string, before: string, after: string) =
    assertRefactored "?" before after
    assertRefactored "?" after before

[<Theory>]
[<InlineData("optional parameter to value option", greeter, structGreeter)>]
[<InlineData("value option parameter to option", structGreeter, greeter)>]
[<InlineData("value option argument of a struct parameter",
             """type Counter() =
    static member Next([<Struct>] ?step: int) = defaultValueArg step 1

let next (step: int voption) = Counter.Next(?step = step)""",
             """type Counter() =
    static member Next(?step: int) = defaultArg step 1

let next (step: int voption) = Counter.Next(?step = ValueOption.toOption step)""")>]
[<InlineData("struct attribute with a target",
             """type Counter() =
    static member Next([<param: Struct>] ?step: int) = defaultValueArg step 1""",
             """type Counter() =
    static member Next(?step: int) = defaultArg step 1""")>]
[<InlineData("generic static member",
             """type Counter() =
    static member Next<'T>(value: 'T, ?step: int) = value, defaultArg step 1

let next = Counter.Next<string>("a", ?step = Some 2)""",
             """type Counter() =
    static member Next<'T>(value: 'T, [<Struct>] ?step: int) = value, defaultValueArg step 1

let next = Counter.Next<string>("a", ?step = ValueSome 2)""")>]
[<InlineData("generic instance member",
             """type Counter() =
    member _.Next<'T>(value: 'T, ?step: int) = value, defaultArg step 1

let next = Counter().Next<string>("a", ?step = Some 2)""",
             """type Counter() =
    member _.Next<'T>(value: 'T, [<Struct>] ?step: int) = value, defaultValueArg step 1

let next = Counter().Next<string>("a", ?step = ValueSome 2)""")>]
[<InlineData("comment between function and argument",
             """type Counter() =
    static member Next([<Struct>] ?step: int) = defaultValueArg step 1

let next (step: int option) = Counter.Next(?step = ValueOption.ofOption (* keep *) step)""",
             """type Counter() =
    static member Next(?step: int) = defaultArg step 1

let next (step: int option) = Counter.Next(?step = (* keep *) step)""")>]
[<InlineData("nested conversions",
             """type Counter() =
    static member Next(?step: int) = defaultArg step 1

let next = Counter.Next(?step = ValueOption.toOption (ValueSome (Counter.Next(?step = Some 2))))""",
             """type Counter() =
    static member Next([<Struct>] ?step: int) = defaultValueArg step 1

let next = Counter.Next(?step = (ValueSome (Counter.Next(?step = ValueSome 2))))""")>]
[<InlineData("shadowed ValueSome",
             """type Counter() =
    static member Next(?step: int) = defaultArg step 1

let ValueSome x = ValueSome (x + 1)

let next = Counter.Next(?step = Some 2)""",
             """type Counter() =
    static member Next([<Struct>] ?step: int) = defaultValueArg step 1

let ValueSome x = ValueSome (x + 1)

let next = Counter.Next(?step = ValueOption.ofOption (Some 2))""")>]
[<InlineData("shadowed ValueOption module",
             """type Counter() =
    static member Next(?step: int) = defaultArg step 1

module ValueOption =
    let ofOption (x: int option) = ValueSome 0

let next (step: int option) = Counter.Next(?step = step)""",
             """type Counter() =
    static member Next([<Struct>] ?step: int) = defaultValueArg step 1

module ValueOption =
    let ofOption (x: int option) = ValueSome 0

let next (step: int option) = Counter.Next(?step = Microsoft.FSharp.Core.ValueOption.ofOption step)""")>]
[<InlineData("shadowed Some",
             """let Some x = Some (x + 1)

type Counter() =
    static member Next(?step: int) = defaultArg step 1

let next = Counter.Next(?step = Some 1)""",
             """let Some x = Some (x + 1)

type Counter() =
    static member Next([<Struct>] ?step: int) = defaultValueArg step 1

let next = Counter.Next(?step = ValueOption.ofOption (Some 1))""")>]
let ``Optional arguments at call sites follow the conversion`` (_scenario: string, before: string, after: string) =
    assertRefactored "?" before after

[<Fact>]
let ``Overload that differs in another parameter does not block the conversion`` () =
    let before =
        """type Counter() =
    static member Next(start: int, [<Struct>] ?step: int) = start + defaultValueArg step 1
    static member Next(start: string, ?step: int) = start.Length + defaultArg step 1"""

    let after =
        """type Counter() =
    static member Next(start: int, [<Struct>] ?step: int) = start + defaultValueArg step 1
    static member Next(start: string, [<Struct>] ?step: int) = start.Length + defaultValueArg step 1"""

    assertRefactored "?step: int) = start.Length" before after

[<Fact>]
let ``Title names the target option kind`` () =
    Assert.Equal("Use 'voption' for optional parameter", (actionsAt (moduleM greeter) "?greeting" |> Seq.exactlyOne).Title)
    Assert.Equal("Use 'option' for optional parameter", (actionsAt (moduleM structGreeter) "?greeting" |> Seq.exactlyOne).Title)

[<Theory>]
[<InlineData("parameter passed to another function",
             """type C() =
    static member M(?x: int) = printfn "%A" x""",
             "?x")>]
[<InlineData("unsupported module function",
             """type C() =
    static member M(?x: int) = Option.map string x""",
             "?x")>]
[<InlineData("override of an abstract slot",
             """type B() =
    abstract M: ?x: int -> int
    default _.M(?x) = defaultArg x 0""",
             "?x)")>]
[<InlineData("caret on the member name",
             """type C() =
    static member M(?x: int) = defaultArg x 0""",
             "M(")>]
[<InlineData("parameter that is not optional",
             """type C() =
    static member M(x: int option) = defaultArg x 0""",
             "x:")>]
[<InlineData("shadowed defaultArg",
             """let defaultArg (x: int option) (fallback: int) = fallback

type C() =
    static member M(?x: int) = defaultArg x 0""",
             "?x")>]
[<InlineData("shadowed Option module",
             """module Option =
    let isSome (x: int option) = true

type C() =
    static member M(?x: int) = Option.isSome x""",
             "?x")>]
[<InlineData("shadowed ValueOption module",
             """module ValueOption =
    let isSome (x: int voption) = false

type C() =
    static member M(?x: int) = Option.isSome x""",
             "?x")>]
[<InlineData("shadowed defaultValueArg",
             """let defaultValueArg (x: int voption) (fallback: int) = fallback

type C() =
    static member M(?x: int) = defaultArg x 0""",
             "?x")>]
[<InlineData("parameter is the default value of Option.defaultValue",
             """type C() =
    static member M(?x: int) = Option.defaultValue x (Some None)""",
             "?x")>]
[<InlineData("overload with the converted signature, option side",
             """type C() =
    static member M(?x: int) = defaultArg x 0
    static member M([<Struct>] ?x: int) = defaultValueArg x 1""",
             "?x: int) = defaultArg")>]
[<InlineData("overload with the converted signature, struct side",
             """type C() =
    static member M(?x: int) = defaultArg x 0
    static member M([<Struct>] ?x: int) = defaultValueArg x 1""",
             "?x: int) = defaultValueArg")>]
[<InlineData("generic overload with the converted signature",
             """type Box<'T>() =
    static member M<'U>(a: 'T, b: 'U list, ?x: int) = defaultArg x 0
    static member M<'V>(a: 'T, b: 'V list, [<Struct>] ?x: int) = defaultValueArg x 1""",
             "?x: int) = defaultArg")>]
[<InlineData("parameter rebound by a match",
             """type C() =
    static member M(?x: int) =
        match x with
        | y -> Option.defaultValue 0 y""",
             "?x")>]
[<InlineData("shadowed pipe",
             """let (|>) (x: int option) f = f x

type C() =
    static member M(?x: int) = x |> Option.defaultValue 0""",
             "?x")>]
let ``No action`` (_scenario: string, code: string, marker: string) =
    Assert.Empty(actionsAt (moduleM code) marker)

[<Fact>]
let ``Value option is not offered before F# 10`` () =
    use context =
        new TestContext(RoslynTestHelpers.CreateSolution(counter, extraFSharpProjectOtherOptions = [| "--langversion:9.0" |]))

    Assert.Empty(actionsIn context counter "?x")

[<Theory>]
[<InlineData("--langversion:9.0", 0)>]
[<InlineData("--langversion:10.0", 1)>]
let ``Value option is offered when the dependent projects have F# 10`` (dependentLangVersion: string, expectedActions: int) =
    let declaringId = ProjectId.CreateNewId()
    let dependentId = ProjectId.CreateNewId()

    let declaring =
        RoslynTestHelpers.CreateProjectInfo
            declaringId
            "C:\\declaring.fsproj"
            [ RoslynTestHelpers.CreateDocumentInfo declaringId "C:\\declaring.fs" counter ]

    let dependent =
        (RoslynTestHelpers.CreateProjectInfo
            dependentId
            "C:\\dependent.fsproj"
            [
                RoslynTestHelpers.CreateDocumentInfo dependentId "C:\\dependent.fs" "module Dependent"
            ])
            .WithProjectReferences([ ProjectReference declaringId ])

    let solution = RoslynTestHelpers.CreateSolution [ declaring; dependent ]
    use _ = solution.Workspace

    for projectId, path, otherOptions in
        [
            declaringId, "C:\\declaring.fs", [||]
            dependentId, "C:\\dependent.fs", [| dependentLangVersion |]
        ] do
        RoslynTestHelpers.SetProjectOptions
            projectId
            solution
            { RoslynTestHelpers.DefaultProjectOptions with
                SourceFiles = [| path |]
                OtherOptions = otherOptions
            }

    let document = solution.GetProject(declaringId).Documents |> Seq.exactlyOne

    Assert.Equal(expectedActions, (actionsOf document (caretAt counter "?x")).Count)

[<Fact>]
let ``No action when the file has a signature`` () =
    let signature =
        moduleM
            """type C =
    new: unit -> C
    static member M: ?x: int -> int"""

    let document = RoslynTestHelpers.GetFsiAndFsDocuments signature counter |> Seq.last

    Assert.False(document.IsFSharpSignatureFile)
    Assert.Empty(actionsOf document (caretAt counter "?x"))

[<Fact>]
let ``Action applies after an invocation that was cancelled`` () =
    let code = moduleM greeter
    use context = TestContext.CreateWithCode code
    let action = actionsIn context code "?greeting" |> Seq.exactlyOne

    use cancellation = new CancellationTokenSource()
    let cancelled = action.GetOperationsAsync cancellation.Token
    cancellation.Cancel()
    Task.WhenAny(cancelled).GetAwaiter().GetResult() |> ignore

    Assert.NotEmpty(action.GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult())
