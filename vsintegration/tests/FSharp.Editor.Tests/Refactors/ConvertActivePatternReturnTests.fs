module FSharp.Editor.Tests.Refactors.ConvertActivePatternReturnTests

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
        tryRefactor code (caretAt code marker) context (new FSharpConvertActivePatternReturnRefactoring())

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
    tryGetRefactoringActions code (caretAt code marker) context (new FSharpConvertActivePatternReturnRefactoring())

let private evenOption =
    """
module M

let (|Even|_|) x = if x % 2 = 0 then Some () else None
"""

let private evenStruct =
    """
module M

[<return: Struct>]
let (|Even|_|) x = if x % 2 = 0 then ValueSome () else ValueNone
"""

let private localEven =
    """
module M

let f v =
    let (|Even|_|) x = if x % 2 = 0 then Some () else None
    match v with
    | Even -> 1
    | _ -> 0
"""

[<Theory>]
[<InlineData("""
module M

let (|Positive|_|) x =
    match x with
    | v when v > 0 -> Some v
    | _ -> None
""",
             "(|Positive",
             """
module M

[<return: Struct>]
let (|Positive|_|) x =
    match x with
    | v when v > 0 -> ValueSome v
    | _ -> ValueNone
""")>]
[<InlineData("""
module M

let (|Int|_|) (s: string) =
    if s.Length = 0 then failwith "empty"
    else
        try Some(int s) with _ -> None
""",
             "(|Int",
             """
module M

[<return: Struct>]
let (|Int|_|) (s: string) =
    if s.Length = 0 then failwith "empty"
    else
        try ValueSome(int s) with _ -> ValueNone
""")>]
[<InlineData("""
module M

let (|Even|_|) (x: int) : unit option = if x % 2 = 0 then Some () else None
""",
             "(|Even",
             """
module M

[<return: Struct>]
let (|Even|_|) (x: int) : unit voption = if x % 2 = 0 then ValueSome () else ValueNone
""")>]
[<InlineData("""
module M

let f v =
    let (|Even|_|) x = if x % 2 = 0 then Some () else None
    match v with
    | Even -> 1
    | _ -> 0
""",
             "(|Even",
             """
module M

let f v =
    let (|Even|_|) x = if x % 2 = 0 then ValueSome () else ValueNone
    match v with
    | Even -> 1
    | _ -> 0
""")>]
[<InlineData("""
module M

[<CompiledName("EvenPattern")>]
let (|Even|_|) x = if x % 2 = 0 then Some () else None
""",
             "(|Even",
             """
module M

[<CompiledName("EvenPattern")>]
[<return: Struct>]
let (|Even|_|) x = if x % 2 = 0 then ValueSome () else ValueNone
""")>]
[<InlineData("""
module M

let rec (|A|_|) x = if x > 0 then Some () else None
and (|B|_|) x = if x < 0 then Some () else None
""",
             "(|B",
             """
module M

let rec (|A|_|) x = if x > 0 then Some () else None
and [<return: Struct>] (|B|_|) x = if x < 0 then ValueSome () else ValueNone
""")>]
let ``Option-returning active pattern converts to a struct one`` (before: string, marker: string, after: string) =
    Assert.Equal(after, refactored before marker)

[<Fact>]
let ``Option-returning active pattern gets the attribute on its own line`` () =
    Assert.Equal(evenStruct, refactored evenOption "(|Even")

[<Theory>]
[<InlineData("""
module M

let (|Even|_|) (x: int) : unit voption = if x % 2 = 0 then ValueSome () else ValueNone
""",
             """
module M

let (|Even|_|) (x: int) : unit option = if x % 2 = 0 then Some () else None
""")>]
[<InlineData("""
module M

[<return: Struct>] let (|Even|_|) x = if x % 2 = 0 then ValueSome () else ValueNone
""",
             """
module M

let (|Even|_|) x = if x % 2 = 0 then Some () else None
""")>]
[<InlineData("""
module M

[<CompiledName("EvenPattern"); return: Struct>]
let (|Even|_|) x = if x % 2 = 0 then ValueSome () else ValueNone
""",
             """
module M

[<CompiledName("EvenPattern")>]
let (|Even|_|) x = if x % 2 = 0 then Some () else None
""")>]
let ``Struct active pattern converts to an option-returning one`` (before: string, after: string) =
    Assert.Equal(after, refactored before "(|Even")

[<Theory>]
[<InlineData("""
module M

let (|Positive|_|) x =
    match x with
    | v when v > 0 -> Some v
    | _ -> None
""",
             "(|Positive")>]
[<InlineData("""
module M

[<CompiledName("EvenPattern")>]
let (|Even|_|) x = if x % 2 = 0 then Some () else None
""",
             "(|Even")>]
[<InlineData("""
module M

let rec (|A|_|) x = if x > 0 then Some () else None
and (|B|_|) x = if x < 0 then Some () else None
""",
             "(|B")>]
let ``Converting to struct and back restores the active pattern`` (original: string, marker: string) =
    Assert.Equal(original, refactored (refactored original marker) marker)

[<Fact>]
let ``Converting to struct and back restores module-level and local active patterns`` () =
    Assert.Equal(evenOption, refactored (refactored evenOption "(|Even") "(|Even")
    Assert.Equal(localEven, refactored (refactored localEven "(|Even") "(|Even")

[<Fact>]
let ``Title names the target return kind`` () =
    Assert.Equal("Use struct return for active pattern", (actionsAt evenOption "(|Even" |> Seq.exactlyOne).Title)
    Assert.Equal("Use option return for active pattern", (actionsAt evenStruct "(|Even" |> Seq.exactlyOne).Title)

[<Theory>]
[<InlineData("""
module M

let (|Even|Odd|) x = if x % 2 = 0 then Even else Odd
""",
             "(|Even")>]
[<InlineData("""
module M

let (|A|B|_|) x = if x > 0 then Some () else None
""",
             "(|A")>]
[<InlineData("""
module M

let (|Positive|_|) x = Some x |> Option.filter (fun v -> v > 0)
""",
             "(|Positive")>]
[<InlineData("""
module M

let (|Even|_|) x = x % 2 = 0
""",
             "(|Even")>]
[<InlineData("""
module M

let (|Never|_|) (x: int) : unit option = failwith "never"
""",
             "(|Never")>]
[<InlineData("""
module M

let (|Even|_|) x = if x % 2 = 0 then Some () else ValueNone
""",
             "(|Even")>]
[<InlineData("""
module M

let (|Even|_|) x = if x % 2 = 0 then Some () else None
""",
             "Some")>]
[<InlineData("""
module M

let even x = if x % 2 = 0 then Some () else None
""",
             "even")>]
[<InlineData("""
module M

let (|Even|_|) x : int list = if x % 2 = 0 then Some () else None
""",
             "(|Even")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)

[<Fact>]
let ``Local active pattern is not converted to struct before F# 9`` () =
    use context =
        new TestContext(RoslynTestHelpers.CreateSolution(localEven, extraFSharpProjectOtherOptions = [| "--langversion:8.0" |]))

    Assert.Empty(
        tryGetRefactoringActions localEven (caretAt localEven "(|Even") context (new FSharpConvertActivePatternReturnRefactoring())
    )

[<Fact>]
let ``No action when the file has a signature`` () =
    let signature =
        """
module M

val (|Even|_|): int -> unit option
"""

    let document =
        RoslynTestHelpers.GetFsiAndFsDocuments signature evenOption |> Seq.last

    let actions = ResizeArray<CodeAction>()

    let context =
        CodeRefactoringContext(
            document,
            TextSpan(caretAt evenOption "(|Even", 1),
            (fun action -> actions.Add action),
            CancellationToken.None
        )

    (new FSharpConvertActivePatternReturnRefactoring()).ComputeRefactoringsAsync(context).GetAwaiter().GetResult()

    Assert.False(document.IsFSharpSignatureFile)
    Assert.Empty(actions)
