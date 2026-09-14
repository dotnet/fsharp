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

[<Theory>]
[<InlineData("module M\n\nlet (|Even|_|) x = if x % 2 = 0 then Some () else None\n",
             "(|Even",
             "module M\n\n[<return: Struct>]\nlet (|Even|_|) x = if x % 2 = 0 then ValueSome () else ValueNone\n")>]
[<InlineData("module M\n\nlet (|Positive|_|) x =\n    match x with\n    | v when v > 0 -> Some v\n    | _ -> None\n",
             "(|Positive",
             "module M\n\n[<return: Struct>]\nlet (|Positive|_|) x =\n    match x with\n    | v when v > 0 -> ValueSome v\n    | _ -> ValueNone\n")>]
[<InlineData("module M\n\nlet (|Int|_|) (s: string) =\n    if s.Length = 0 then failwith \"empty\"\n    else\n        try Some(int s) with _ -> None\n",
             "(|Int",
             "module M\n\n[<return: Struct>]\nlet (|Int|_|) (s: string) =\n    if s.Length = 0 then failwith \"empty\"\n    else\n        try ValueSome(int s) with _ -> ValueNone\n")>]
[<InlineData("module M\n\nlet (|Even|_|) (x: int) : unit option = if x % 2 = 0 then Some () else None\n",
             "(|Even",
             "module M\n\n[<return: Struct>]\nlet (|Even|_|) (x: int) : unit voption = if x % 2 = 0 then ValueSome () else ValueNone\n")>]
[<InlineData("module M\n\nlet f v =\n    let (|Even|_|) x = if x % 2 = 0 then Some () else None\n    match v with\n    | Even -> 1\n    | _ -> 0\n",
             "(|Even",
             "module M\n\nlet f v =\n    let (|Even|_|) x = if x % 2 = 0 then ValueSome () else ValueNone\n    match v with\n    | Even -> 1\n    | _ -> 0\n")>]
[<InlineData("module M\n\n[<CompiledName(\"EvenPattern\")>]\nlet (|Even|_|) x = if x % 2 = 0 then Some () else None\n",
             "(|Even",
             "module M\n\n[<CompiledName(\"EvenPattern\")>]\n[<return: Struct>]\nlet (|Even|_|) x = if x % 2 = 0 then ValueSome () else ValueNone\n")>]
[<InlineData("module M\n\nlet rec (|A|_|) x = if x > 0 then Some () else None\nand (|B|_|) x = if x < 0 then Some () else None\n",
             "(|B",
             "module M\n\nlet rec (|A|_|) x = if x > 0 then Some () else None\nand [<return: Struct>] (|B|_|) x = if x < 0 then ValueSome () else ValueNone\n")>]
let ``Option-returning active pattern converts to a struct one`` (before: string, marker: string, after: string) =
    Assert.Equal(after, refactored before marker)

[<Theory>]
[<InlineData("module M\n\nlet (|Even|_|) (x: int) : unit voption = if x % 2 = 0 then ValueSome () else ValueNone\n",
             "module M\n\nlet (|Even|_|) (x: int) : unit option = if x % 2 = 0 then Some () else None\n")>]
[<InlineData("module M\n\n[<return: Struct>] let (|Even|_|) x = if x % 2 = 0 then ValueSome () else ValueNone\n",
             "module M\n\nlet (|Even|_|) x = if x % 2 = 0 then Some () else None\n")>]
[<InlineData("module M\n\n[<CompiledName(\"EvenPattern\"); return: Struct>]\nlet (|Even|_|) x = if x % 2 = 0 then ValueSome () else ValueNone\n",
             "module M\n\n[<CompiledName(\"EvenPattern\")>]\nlet (|Even|_|) x = if x % 2 = 0 then Some () else None\n")>]
let ``Struct active pattern converts to an option-returning one`` (before: string, after: string) =
    Assert.Equal(after, refactored before "(|Even")

[<Theory>]
[<InlineData("module M\n\nlet (|Even|_|) x = if x % 2 = 0 then Some () else None\n", "(|Even")>]
[<InlineData("module M\n\nlet (|Positive|_|) x =\n    match x with\n    | v when v > 0 -> Some v\n    | _ -> None\n", "(|Positive")>]
[<InlineData("module M\n\nlet f v =\n    let (|Even|_|) x = if x % 2 = 0 then Some () else None\n    match v with\n    | Even -> 1\n    | _ -> 0\n",
             "(|Even")>]
[<InlineData("module M\n\n[<CompiledName(\"EvenPattern\")>]\nlet (|Even|_|) x = if x % 2 = 0 then Some () else None\n", "(|Even")>]
[<InlineData("module M\n\nlet rec (|A|_|) x = if x > 0 then Some () else None\nand (|B|_|) x = if x < 0 then Some () else None\n", "(|B")>]
let ``Converting to struct and back restores the active pattern`` (original: string, marker: string) =
    Assert.Equal(original, refactored (refactored original marker) marker)

[<Fact>]
let ``Title names the target return kind`` () =
    let optionPattern =
        "module M\n\nlet (|Even|_|) x = if x % 2 = 0 then Some () else None\n"

    let structPattern =
        "module M\n\n[<return: Struct>]\nlet (|Even|_|) x = if x % 2 = 0 then ValueSome () else ValueNone\n"

    Assert.Equal("Use struct return for active pattern", (actionsAt optionPattern "(|Even" |> Seq.exactlyOne).Title)
    Assert.Equal("Use option return for active pattern", (actionsAt structPattern "(|Even" |> Seq.exactlyOne).Title)

[<Theory>]
[<InlineData("module M\n\nlet (|Even|Odd|) x = if x % 2 = 0 then Even else Odd\n", "(|Even")>]
[<InlineData("module M\n\nlet (|A|B|_|) x = if x > 0 then Some () else None\n", "(|A")>]
[<InlineData("module M\n\nlet (|Positive|_|) x = Some x |> Option.filter (fun v -> v > 0)\n", "(|Positive")>]
[<InlineData("module M\n\nlet (|Even|_|) x = x % 2 = 0\n", "(|Even")>]
[<InlineData("module M\n\nlet (|Never|_|) (x: int) : unit option = failwith \"never\"\n", "(|Never")>]
[<InlineData("module M\n\nlet (|Even|_|) x = if x % 2 = 0 then Some () else ValueNone\n", "(|Even")>]
[<InlineData("module M\n\nlet (|Even|_|) x = if x % 2 = 0 then Some () else None\n", "Some")>]
[<InlineData("module M\n\nlet even x = if x % 2 = 0 then Some () else None\n", "even")>]
[<InlineData("module M\n\nlet (|Even|_|) x : int list = if x % 2 = 0 then Some () else None\n", "(|Even")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)

[<Fact>]
let ``Local active pattern is not converted to struct before F# 9`` () =
    let code =
        "module M\n\nlet f v =\n    let (|Even|_|) x = if x % 2 = 0 then Some () else None\n    match v with\n    | Even -> 1\n    | _ -> 0\n"

    use context =
        new TestContext(RoslynTestHelpers.CreateSolution(code, extraFSharpProjectOtherOptions = [| "--langversion:8.0" |]))

    Assert.Empty(tryGetRefactoringActions code (caretAt code "(|Even") context (new FSharpConvertActivePatternReturnRefactoring()))

[<Fact>]
let ``No action when the file has a signature`` () =
    let code = "module M\n\nlet (|Even|_|) x = if x % 2 = 0 then Some () else None\n"
    let signature = "module M\n\nval (|Even|_|): int -> unit option\n"
    let document = RoslynTestHelpers.GetFsiAndFsDocuments signature code |> Seq.last
    let actions = ResizeArray<CodeAction>()

    let context =
        CodeRefactoringContext(document, TextSpan(caretAt code "(|Even", 1), (fun action -> actions.Add action), CancellationToken.None)

    (new FSharpConvertActivePatternReturnRefactoring()).ComputeRefactoringsAsync(context).GetAwaiter().GetResult()

    Assert.False(document.IsFSharpSignatureFile)
    Assert.Empty(actions)
