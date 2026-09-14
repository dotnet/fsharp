module FSharp.Editor.Tests.Refactors.ExtractLetBindingTests

open System

open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open Xunit

open FSharp.Editor.Tests.Refactors.RefactorTestFramework

let private extractToLetBinding = "Extract to let binding"
let private extractToLiteral = "Extract to literal"

let private selectionOf (code: string) (selected: string) =
    TextSpan(code.IndexOf(selected, StringComparison.Ordinal), selected.Length)

let private caretAt (code: string) (marker: string) =
    TextSpan(code.IndexOf(marker, StringComparison.Ordinal), 0)

let private extractedAt (title: string) (code: string) (span: TextSpan) =
    use context = TestContext.CreateWithCode code

    let document =
        refactorSpan code span title context (new FSharpExtractLetBindingRefactoring())

    let parseResults =
        document.GetFSharpParseResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty(parseResults.Diagnostics)
    (document.GetTextAsync() |> GetTaskResult).ToString()

let private extracted (title: string) (code: string) (selected: string) =
    extractedAt title code (selectionOf code selected)

let private titlesAt (code: string) (span: TextSpan) =
    use context = TestContext.CreateWithCode code

    tryGetRefactoringActionsForSpan code span context (new FSharpExtractLetBindingRefactoring())
    |> Seq.map _.Title
    |> List.ofSeq

let private titlesFor (code: string) (selected: string) =
    titlesAt code (selectionOf code selected)

[<Theory>]
[<InlineData("w * h + 1", "    let extracted = w * h + 1\n    printfn \"%d\" (extracted)\n")>]
[<InlineData("(w * h + 1)", "    let extracted = w * h + 1\n    printfn \"%d\" extracted\n")>]
let ``Expression in a statement is bound in front of it`` (selected: string, body: string) =
    let code = "module M\n\nlet area w h =\n    printfn \"%d\" (w * h + 1)\n"
    Assert.Equal($"module M\n\nlet area w h =\n{body}", extracted extractToLetBinding code selected)

[<Fact>]
let ``Parentheses of a method call stay`` () =
    let code =
        "module M\n\nlet append (sb: System.Text.StringBuilder) =\n    sb.Append(1 + 2)\n"

    let expected =
        "module M\n\nlet append (sb: System.Text.StringBuilder) =\n    let extracted = 1 + 2\n    sb.Append(extracted)\n"

    Assert.Equal(expected, extracted extractToLetBinding code "(1 + 2)")

[<Fact>]
let ``Multi-line right-hand side is bound in front of its let`` () =
    let code =
        "module M\n\nlet run items =\n    let mutable acc = 0\n    let total =\n        items\n        |> List.filter (fun i -> i > acc)\n        |> List.sum\n    total\n"

    let expected =
        "module M\n\nlet run items =\n    let mutable acc = 0\n    let extracted =\n        items\n        |> List.filter (fun i -> i > acc)\n        |> List.sum\n    let total =\n        extracted\n    total\n"

    Assert.Equal(expected, extracted extractToLetBinding code "items\n        |> List.filter (fun i -> i > acc)\n        |> List.sum")

[<Fact>]
let ``Whole lines selected with their indentation and line break are extracted`` () =
    let code =
        "module M\n\nlet run items =\n    let mutable acc = 0\n\n    let total =\n        items\n        |> List.filter (fun i -> i > acc)\n        |> List.sum\n\n    total\n"

    let expected =
        "module M\n\nlet run items =\n    let mutable acc = 0\n\n    let extracted =\n        items\n        |> List.filter (fun i -> i > acc)\n        |> List.sum\n    let total =\n        extracted\n\n    total\n"

    Assert.Equal(
        expected,
        extracted extractToLetBinding code "        items\n        |> List.filter (fun i -> i > acc)\n        |> List.sum\n"
    )

[<Fact>]
let ``Match clause body on the arrow line moves to its own lines`` () =
    let code =
        "module M\n\nlet f x offset =\n    match x with\n    | Some v -> v * 2 + offset\n    | None -> 0\n"

    let expected =
        "module M\n\nlet f x offset =\n    match x with\n    | Some v ->\n        let extracted = v * 2 + offset\n        extracted\n    | None -> 0\n"

    Assert.Equal(expected, extracted extractToLetBinding code "v * 2 + offset")

[<Fact>]
let ``Right-hand side on the equals line keeps its trailing comment`` () =
    let code = "module M\n\nlet r = compute a b // slow\n"

    let expected =
        "module M\n\nlet r =\n    let extracted = compute a b\n    extracted // slow\n"

    Assert.Equal(expected, extracted extractToLetBinding code "compute a b")

[<Fact>]
let ``Expression in a computation expression is bound in front of its statement`` () =
    let code =
        "module M\n\nlet load id = async {\n    let! raw = fetch id\n    return parse raw |> List.length }\n"

    let expected =
        "module M\n\nlet load id = async {\n    let! raw = fetch id\n    let extracted = parse raw |> List.length\n    return extracted }\n"

    Assert.Equal(expected, extracted extractToLetBinding code "parse raw |> List.length")

[<Fact>]
let ``Lambda body on the arrow line moves to its own lines`` () =
    let code = "module M\n\nlet r = xs |> List.map (fun x -> x + 1)\n"

    let expected =
        "module M\n\nlet r = xs |> List.map (fun x ->\n    let extracted = x + 1\n    extracted)\n"

    Assert.Equal(expected, extracted extractToLetBinding code "x + 1")

[<Fact>]
let ``Name does not collide with an existing identifier`` () =
    let code =
        "module M\n\nlet extracted = 0\n\nlet f x =\n    printfn \"%d\" (x + 1)\n"

    let expected =
        "module M\n\nlet extracted = 0\n\nlet f x =\n    let extracted1 = x + 1\n    printfn \"%d\" (extracted1)\n"

    Assert.Equal(expected, extracted extractToLetBinding code "x + 1")

[<Fact>]
let ``Line breaks of the file are kept`` () =
    let code = "module M\r\n\r\nlet f x =\r\n    printfn \"%d\" (x + 1)\r\n"

    let expected =
        "module M\r\n\r\nlet f x =\r\n    let extracted = x + 1\r\n    printfn \"%d\" (extracted)\r\n"

    Assert.Equal(expected, extracted extractToLetBinding code "x + 1")

[<Fact>]
let ``Lines inside a multi-line string are not re-indented`` () =
    let code =
        "module M\n\nlet f name =\n    printfn \"%s\" (String.Format(\"\"\"Hello\n{0}\"\"\", name))\n"

    let expected =
        "module M\n\nlet f name =\n    let extracted =\n        String.Format(\"\"\"Hello\n{0}\"\"\", name)\n    printfn \"%s\" (extracted)\n"

    Assert.Equal(expected, extracted extractToLetBinding code "String.Format(\"\"\"Hello\n{0}\"\"\", name)")

[<Fact>]
let ``Constant in a module-level declaration becomes a literal in front of it`` () =
    let code = "module M\n\nlet greet name =\n    printfn \"Hello %s\" name\n"

    let expected =
        "module M\n\n[<Literal>]\nlet ExtractedConstant = \"Hello %s\"\n\nlet greet name =\n    printfn ExtractedConstant name\n"

    Assert.Equal(expected, extracted extractToLiteral code "\"Hello %s\"")

[<Fact>]
let ``Negative number becomes a literal`` () =
    let code = "module M\n\nlet f () = g -1\n"

    let expected =
        "module M\n\n[<Literal>]\nlet ExtractedConstant = -1\n\nlet f () = g ExtractedConstant\n"

    Assert.Equal(expected, extracted extractToLiteral code "-1")

[<Fact>]
let ``Literal is offered only for constants outside types`` () =
    Assert.Equal<string list>([ extractToLetBinding; extractToLiteral ], titlesFor "module M\n\nlet f () = g 42\n" "42")
    Assert.Equal<string list>([ extractToLetBinding ], titlesFor "module M\n\ntype T() =\n    member _.M() = 42\n" "42")
    Assert.Equal<string list>([ extractToLetBinding ], titlesFor "module M\n\nlet f x = g (x + 1)\n" "x + 1")

[<Theory>]
[<InlineData("fun x")>]
[<InlineData("x ->")>]
[<InlineData("-> x")>]
[<InlineData(" x + 1)")>]
let ``Caret in the header of a parenthesized lambda extracts the lambda`` (marker: string) =
    let code = "module M\n\nlet f xs =\n    xs |> List.map (fun x -> x + 1)\n"

    let expected =
        "module M\n\nlet f xs =\n    let extracted = fun x -> x + 1\n    xs |> List.map extracted\n"

    Assert.Equal<string list>([ extractToLetBinding ], titlesAt code (caretAt code marker))
    Assert.Equal(expected, extractedAt extractToLetBinding code (caretAt code marker))

[<Theory>]
[<InlineData("module M\n\nlet f xs =\n    xs |> List.map (fun x -> x + 1)\n", "x + 1)")>]
[<InlineData("module M\n\nlet f xs =\n    xs |> List.map (fun x -> x + 1)\n", "+ 1)")>]
[<InlineData("module M\n\nlet f xs =\n    xs |> List.map (fun x -> x + 1)\n", "(fun")>]
[<InlineData("module M\n\nlet f = fun x -> x + 1\n", "fun")>]
let ``No action without a selection outside a parenthesized lambda header`` (code: string, marker: string) =
    Assert.Empty(titlesAt code (caretAt code marker))

[<Theory>]
[<InlineData("\"Hello %s\" name")>]
[<InlineData("llo %s")>]
[<InlineData(" name\n")>]
let ``Constant at the caret becomes a literal without a selection`` (marker: string) =
    let code = "module M\n\nlet greet name =\n    printfn \"Hello %s\" name\n"

    let expected =
        "module M\n\n[<Literal>]\nlet ExtractedConstant = \"Hello %s\"\n\nlet greet name =\n    printfn ExtractedConstant name\n"

    Assert.Equal(expected, extractedAt extractToLiteral code (caretAt code marker))

[<Fact>]
let ``Only the literal is offered without a selection`` () =
    let code = "module M\n\nlet f () = g 42\n"
    Assert.Equal<string list>([ extractToLiteral ], titlesAt code (caretAt code "2\n"))

[<Theory>]
[<InlineData("module M\n\nlet f x = g x\n", "g x")>]
[<InlineData("module M\n\ntype T() =\n    member _.M() = 42\n", "42")>]
[<InlineData("module M\n\nlet f a = $\"{a + 1}\"\n", "1}")>]
let ``No action without a selection`` (code: string, marker: string) =
    Assert.Empty(titlesAt code (caretAt code marker))

[<Theory>]
[<InlineData("module M\n\nlet f w h = w * h + 1\n", "w * h +")>]
[<InlineData("module M\n\nlet f () = 1 + 2 + 3\n", "2 + 3")>]
[<InlineData("module M\n\nlet f c x y =\n    if c then x + 1 else y\n", "x + 1")>]
[<InlineData("module M\n\nlet f n =\n    let mutable i = 0\n    while i < n do\n        i <- i + 1\n", "i < n")>]
[<InlineData("module M\n\nlet f x =\n    match x with\n    | v when v > 0 -> v\n    | _ -> 0\n", "v > 0")>]
[<InlineData("module M\n\nlet f a b = $\"{a + b}\"\n", "a + b")>]
[<InlineData("module M\n\nlet load id = async {\n    return id }\n", "return id")>]
[<InlineData("module M\n\nlet f (sb: System.Text.StringBuilder) a b = sb.Insert(a, b)\n", "a, b")>]
[<InlineData("module M\n\nlet f x = x.ToString() + \"\"\n", "x")>]
[<InlineData("module M\n\nlet f (a: bool) (s: string) = a && s.Length > 0\n", "s.Length > 0")>]
[<InlineData("module M\n\nlet f x = x + 1\n", "")>]
let ``No action`` (code: string, selected: string) = Assert.Empty(titlesFor code selected)
