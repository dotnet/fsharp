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

let private area =
    """
module M

let area w h =
    printfn "%d" (w * h + 1)
"""

[<Theory>]
[<InlineData("w * h + 1",
             """
module M

let area w h =
    let extracted = w * h + 1
    printfn "%d" (extracted)
""")>]
[<InlineData("(w * h + 1)",
             """
module M

let area w h =
    let extracted = w * h + 1
    printfn "%d" extracted
""")>]
let ``Expression in a statement is bound in front of it`` (selected: string, expected: string) =
    Assert.Equal(expected, extracted extractToLetBinding area selected)

[<Fact>]
let ``Parentheses of a method call stay`` () =
    let code =
        """
module M

let append (sb: System.Text.StringBuilder) =
    sb.Append(1 + 2)
"""

    let expected =
        """
module M

let append (sb: System.Text.StringBuilder) =
    let extracted = 1 + 2
    sb.Append(extracted)
"""

    Assert.Equal(expected, extracted extractToLetBinding code "(1 + 2)")

let private run =
    """
module M

let run items =
    let mutable acc = 0
    let total =
        items
        |> List.filter (fun i -> i > acc)
        |> List.sum
    total
"""

[<Fact>]
let ``Multi-line right-hand side is bound in front of its let`` () =
    let expected =
        """
module M

let run items =
    let mutable acc = 0
    let extracted =
        items
        |> List.filter (fun i -> i > acc)
        |> List.sum
    let total =
        extracted
    total
"""

    let start = run.LastIndexOf("items", StringComparison.Ordinal)

    let finish =
        run.IndexOf("|> List.sum", start, StringComparison.Ordinal)
        + "|> List.sum".Length

    Assert.Equal(expected, extractedAt extractToLetBinding run (TextSpan.FromBounds(start, finish)))

[<Fact>]
let ``Whole lines selected with their indentation and line break are extracted`` () =
    let code =
        """
module M

let run items =
    let mutable acc = 0

    let total =
        items
        |> List.filter (fun i -> i > acc)
        |> List.sum

    total
"""

    let expected =
        """
module M

let run items =
    let mutable acc = 0

    let extracted =
        items
        |> List.filter (fun i -> i > acc)
        |> List.sum
    let total =
        extracted

    total
"""

    let lineStart = code.LastIndexOf("        items", StringComparison.Ordinal)

    let afterLineBreak =
        code.IndexOf('\n', code.IndexOf("|> List.sum", lineStart, StringComparison.Ordinal))
        + 1

    Assert.Equal(expected, extractedAt extractToLetBinding code (TextSpan.FromBounds(lineStart, afterLineBreak)))

[<Fact>]
let ``Match clause body on the arrow line moves to its own lines`` () =
    let code =
        """
module M

let f x offset =
    match x with
    | Some v -> v * 2 + offset
    | None -> 0
"""

    let expected =
        """
module M

let f x offset =
    match x with
    | Some v ->
        let extracted = v * 2 + offset
        extracted
    | None -> 0
"""

    Assert.Equal(expected, extracted extractToLetBinding code "v * 2 + offset")

[<Fact>]
let ``Right-hand side on the equals line keeps its trailing comment`` () =
    let code =
        """
module M

let r = compute a b // slow
"""

    let expected =
        """
module M

let r =
    let extracted = compute a b
    extracted // slow
"""

    Assert.Equal(expected, extracted extractToLetBinding code "compute a b")

[<Fact>]
let ``Expression in a computation expression is bound in front of its statement`` () =
    let code =
        """
module M

let load id = async {
    let! raw = fetch id
    return parse raw |> List.length }
"""

    let expected =
        """
module M

let load id = async {
    let! raw = fetch id
    let extracted = parse raw |> List.length
    return extracted }
"""

    Assert.Equal(expected, extracted extractToLetBinding code "parse raw |> List.length")

[<Fact>]
let ``Lambda body on the arrow line moves to its own lines`` () =
    let code =
        """
module M

let r = xs |> List.map (fun x -> x + 1)
"""

    let expected =
        """
module M

let r = xs |> List.map (fun x ->
    let extracted = x + 1
    extracted)
"""

    Assert.Equal(expected, extracted extractToLetBinding code "x + 1")

[<Fact>]
let ``Name does not collide with an existing identifier`` () =
    let code =
        """
module M

let extracted = 0

let f x =
    printfn "%d" (x + 1)
"""

    let expected =
        """
module M

let extracted = 0

let f x =
    let extracted1 = x + 1
    printfn "%d" (extracted1)
"""

    Assert.Equal(expected, extracted extractToLetBinding code "x + 1")

[<Fact>]
let ``Line breaks of the file are kept`` () =
    let code = "module M\r\n\r\nlet f x =\r\n    printfn \"%d\" (x + 1)\r\n"

    let expected =
        "module M\r\n\r\nlet f x =\r\n    let extracted = x + 1\r\n    printfn \"%d\" (extracted)\r\n"

    Assert.Equal(expected, extracted extractToLetBinding code "x + 1")

[<Fact>]
let ``Lines inside a multi-line string are not re-indented`` () =
    // The code contains a triple-quoted string, which a triple-quoted literal cannot hold.
    let code =
        "module M\n\nlet f name =\n    printfn \"%s\" (String.Format(\"\"\"Hello\n{0}\"\"\", name))\n"

    let expected =
        "module M\n\nlet f name =\n    let extracted =\n        String.Format(\"\"\"Hello\n{0}\"\"\", name)\n    printfn \"%s\" (extracted)\n"

    Assert.Equal(expected, extracted extractToLetBinding code "String.Format(\"\"\"Hello\n{0}\"\"\", name)")

let private greet =
    """
module M

let greet name =
    printfn "Hello %s" name
"""

let private greetWithLiteral =
    """
module M

[<Literal>]
let ExtractedConstant = "Hello %s"

let greet name =
    printfn ExtractedConstant name
"""

[<Fact>]
let ``Constant in a module-level declaration becomes a literal in front of it`` () =
    Assert.Equal(greetWithLiteral, extracted extractToLiteral greet "\"Hello %s\"")

[<Fact>]
let ``Negative number becomes a literal`` () =
    let code =
        """
module M

let f () = g -1
"""

    let expected =
        """
module M

[<Literal>]
let ExtractedConstant = -1

let f () = g ExtractedConstant
"""

    Assert.Equal(expected, extracted extractToLiteral code "-1")

let private moduleConstant =
    """
module M

let f () = g 42
"""

[<Fact>]
let ``Literal is offered only for constants outside types`` () =
    let memberConstant =
        """
module M

type T() =
    member _.M() = 42
"""

    let expression =
        """
module M

let f x = g (x + 1)
"""

    Assert.Equal<string list>([ extractToLetBinding; extractToLiteral ], titlesFor moduleConstant "42")
    Assert.Equal<string list>([ extractToLetBinding ], titlesFor memberConstant "42")
    Assert.Equal<string list>([ extractToLetBinding ], titlesFor expression "x + 1")

let private mapIncrement =
    """
module M

let f xs =
    xs |> List.map (fun x -> x + 1)
"""

[<Theory>]
[<InlineData("fun x")>]
[<InlineData("x ->")>]
[<InlineData("-> x")>]
[<InlineData(" x + 1)")>]
let ``Caret in the header of a parenthesized lambda extracts the lambda`` (marker: string) =
    let expected =
        """
module M

let f xs =
    let extracted = fun x -> x + 1
    xs |> List.map extracted
"""

    Assert.Equal<string list>([ extractToLetBinding ], titlesAt mapIncrement (caretAt mapIncrement marker))
    Assert.Equal(expected, extractedAt extractToLetBinding mapIncrement (caretAt mapIncrement marker))

[<Theory>]
[<InlineData("x + 1)")>]
[<InlineData("+ 1)")>]
[<InlineData("(fun")>]
let ``No action without a selection outside a parenthesized lambda header`` (marker: string) =
    Assert.Empty(titlesAt mapIncrement (caretAt mapIncrement marker))

[<Fact>]
let ``No action without a selection on a lambda without parentheses`` () =
    let code =
        """
module M

let f = fun x -> x + 1
"""

    Assert.Empty(titlesAt code (caretAt code "fun"))

[<Theory>]
[<InlineData("\"Hello %s\" name", 0)>]
[<InlineData("llo %s", 0)>]
[<InlineData("\" name", 1)>]
let ``Constant at the caret becomes a literal without a selection`` (marker: string, offset: int) =
    let caret = TextSpan(greet.IndexOf(marker, StringComparison.Ordinal) + offset, 0)
    Assert.Equal(greetWithLiteral, extractedAt extractToLiteral greet caret)

[<Fact>]
let ``Only the literal is offered without a selection`` () =
    let caret = TextSpan(moduleConstant.IndexOf("42", StringComparison.Ordinal) + 1, 0)

    Assert.Equal<string list>([ extractToLiteral ], titlesAt moduleConstant caret)

[<Theory>]
[<InlineData("""
module M

let f x = g x
""",
             "g x")>]
[<InlineData("""
module M

type T() =
    member _.M() = 42
""",
             "42")>]
[<InlineData("""
module M

let f a = $"{a + 1}"
""",
             "1}")>]
let ``No action without a selection`` (code: string, marker: string) =
    Assert.Empty(titlesAt code (caretAt code marker))

[<Theory>]
[<InlineData("""
module M

let f w h = w * h + 1
""",
             "w * h +")>]
[<InlineData("""
module M

let f () = 1 + 2 + 3
""",
             "2 + 3")>]
[<InlineData("""
module M

let f c x y =
    if c then x + 1 else y
""",
             "x + 1")>]
[<InlineData("""
module M

let f n =
    let mutable i = 0
    while i < n do
        i <- i + 1
""",
             "i < n")>]
[<InlineData("""
module M

let f x =
    match x with
    | v when v > 0 -> v
    | _ -> 0
""",
             "v > 0")>]
[<InlineData("""
module M

let f a b = $"{a + b}"
""",
             "a + b")>]
[<InlineData("""
module M

let load id = async {
    return id }
""",
             "return id")>]
[<InlineData("""
module M

let f (sb: System.Text.StringBuilder) a b = sb.Insert(a, b)
""",
             "a, b")>]
[<InlineData("""
module M

let f x = x.ToString() + ""
""",
             "x")>]
[<InlineData("""
module M

let f (a: bool) (s: string) = a && s.Length > 0
""",
             "s.Length > 0")>]
[<InlineData("""
module M

let f x = x + 1
""",
             "")>]
let ``No action`` (code: string, selected: string) = Assert.Empty(titlesFor code selected)
