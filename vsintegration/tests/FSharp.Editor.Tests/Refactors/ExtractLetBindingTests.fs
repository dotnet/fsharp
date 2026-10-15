module FSharp.Editor.Tests.Refactors.ExtractLetBindingTests

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open Xunit

open FSharp.Editor.Tests.Helpers
open FSharp.Editor.Tests.Refactors.RefactorTestFramework

[<Literal>]
let private extractToLetBinding = "Extract to let binding"

[<Literal>]
let private extractToLiteral = "Extract to literal"

[<Literal>]
let private bothTitles = extractToLetBinding + ";" + extractToLiteral

[<Literal>]
let private area =
    """
let area w h =
    printfn "%d" (w * h + 1)
"""

[<Literal>]
let private greet =
    """
let greet name =
    printfn "Hello %s" name
"""

[<Literal>]
let private greetWithLiteral =
    """
[<Literal>]
let ExtractedConstant = "Hello %s"

let greet name =
    printfn ExtractedConstant name
"""

[<Literal>]
let private moduleConstant =
    """
let f () = g 42
"""

[<Literal>]
let private mapIncrement =
    """
let f xs =
    xs |> List.map (fun x -> x + 1)
"""

[<Literal>]
let private mapIncrementExtracted =
    """
let f xs =
    let extracted: int -> int = fun x -> x + 1
    xs |> List.map extracted
"""

let private extractedAt title code span =
    use context = TestContext.CreateWithCode code
    refactoredText (new FSharpExtractLetBindingRefactoring()) context title code span

let private titlesAt code span =
    use context = TestContext.CreateWithCode code
    offeredTitles (new FSharpExtractLetBindingRefactoring()) context code span

let private assertTypeChecks (code: string) =
    use context = TestContext.CreateWithCode code

    let _, checkResults =
        (RoslynTestHelpers.GetLastDocument context.Solution).GetFSharpParseAndCheckResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty(checkResults.Diagnostics)

[<Theory>]
[<InlineData("Expression in a statement is bound in front of it",
             extractToLetBinding,
             area,
             "w * h + 1",
             """
let area w h =
    let extracted = w * h + 1
    printfn "%d" (extracted)
""")>]
[<InlineData("Parenthesized expression in a statement is bound in front of it",
             extractToLetBinding,
             area,
             "(w * h + 1)",
             """
let area w h =
    let extracted = w * h + 1
    printfn "%d" extracted
""")>]
[<InlineData("Parentheses of a method call stay",
             extractToLetBinding,
             """
let append (sb: System.Text.StringBuilder) =
    sb.Append(1 + 2)
""",
             "(1 + 2)",
             """
let append (sb: System.Text.StringBuilder) =
    let extracted = 1 + 2
    sb.Append(extracted)
""")>]
[<InlineData("Multi-line right-hand side is bound in front of its let",
             extractToLetBinding,
             """
let run items =
    let mutable acc = 0
    let total =
        items
        |> List.filter (fun i -> i > acc)
        |> List.sum
    total
""",
             """items
        |> List.filter (fun i -> i > acc)
        |> List.sum""",
             """
let run items =
    let mutable acc = 0
    let extracted =
        items
        |> List.filter (fun i -> i > acc)
        |> List.sum
    let total =
        extracted
    total
""")>]
[<InlineData("Whole lines selected with their indentation and line break are extracted",
             extractToLetBinding,
             """
let run items =
    let mutable acc = 0

    let total =
        items
        |> List.filter (fun i -> i > acc)
        |> List.sum

    total
""",
             """        items
        |> List.filter (fun i -> i > acc)
        |> List.sum
""",
             """
let run items =
    let mutable acc = 0

    let extracted =
        items
        |> List.filter (fun i -> i > acc)
        |> List.sum
    let total =
        extracted

    total
""")>]
[<InlineData("Match clause body on the arrow line moves to its own lines",
             extractToLetBinding,
             """
let f x offset =
    match x with
    | Some v -> v * 2 + offset
    | None -> 0
""",
             "v * 2 + offset",
             """
let f x offset =
    match x with
    | Some v ->
        let extracted = v * 2 + offset
        extracted
    | None -> 0
""")>]
[<InlineData("Right-hand side on the equals line keeps its trailing comment",
             extractToLetBinding,
             """
let r = compute a b // slow
""",
             "compute a b",
             """
let r =
    let extracted = compute a b
    extracted // slow
""")>]
[<InlineData("Expression in a computation expression is bound in front of its statement",
             extractToLetBinding,
             """
let load id = async {
    let! raw = fetch id
    return parse raw |> List.length }
""",
             "parse raw |> List.length",
             """
let load id = async {
    let! raw = fetch id
    let extracted = parse raw |> List.length
    return extracted }
""")>]
[<InlineData("Lambda body on the arrow line moves to its own lines",
             extractToLetBinding,
             """
let r = xs |> List.map (fun x -> x + 1)
""",
             "x + 1",
             """
let r = xs |> List.map (fun x ->
    let extracted = x + 1
    extracted)
""")>]
[<InlineData("Body of a match clause on the line of its let moves under the arrow",
             extractToLetBinding,
             """
let f x = match x with _ -> 1 + 2
""",
             "1 + 2",
             """
let f x = match x with _ ->
              let extracted = 1 + 2
              extracted
""")>]
[<InlineData("Body of an if on the line of its let moves under the then",
             extractToLetBinding,
             """
let g c = if c then printfn "%d" (1 + 2)
""",
             "(1 + 2)",
             """
let g c = if c then
              let extracted = 1 + 2
              printfn "%d" extracted
""")>]
[<InlineData("Body of a let in a module on the line of its module moves under the equals",
             extractToLetBinding,
             """
namespace N
module M = let f () = 42
""",
             "42",
             """
namespace N
module M = let f () =
               let extracted = 42
               extracted
""")>]
[<InlineData("Expression in a finally block is bound inside the block",
             extractToLetBinding,
             """
let f (stream: System.IO.Stream) =
    try
        stream.WriteByte(1uy)
    finally
        stream.Dispose()
""",
             "stream.Dispose()",
             """
let f (stream: System.IO.Stream) =
    try
        stream.WriteByte(1uy)
    finally
        let extracted = stream.Dispose()
        extracted
""")>]
[<InlineData("Name does not collide with an existing identifier",
             extractToLetBinding,
             """
let extracted = 0

let f x =
    printfn "%d" (x + 1)
""",
             "x + 1",
             """
let extracted = 0

let f x =
    let extracted1 = x + 1
    printfn "%d" (extracted1)
""")>]
[<InlineData("Line breaks of the file are kept",
             extractToLetBinding,
             "module M\r\n\r\nlet f x =\r\n    printfn \"%d\" (x + 1)\r\n",
             "x + 1",
             "module M\r\n\r\nlet f x =\r\n    let extracted = x + 1\r\n    printfn \"%d\" (extracted)\r\n")>]
[<InlineData("Lines inside a multi-line string are not re-indented",
             extractToLetBinding,
             "module M\n\nlet f name =\n    printfn \"%s\" (String.Format(\"\"\"Hello\n{0}\"\"\", name))\n",
             "String.Format(\"\"\"Hello\n{0}\"\"\", name)",
             "module M\n\nlet f name =\n    let extracted =\n        String.Format(\"\"\"Hello\n{0}\"\"\", name)\n    printfn \"%s\" (extracted)\n")>]
[<InlineData("Lines inside a multi-line string in an inactive branch are not re-indented",
             extractToLetBinding,
             "module M\n\nlet r = combine 1 [\n            #if ALT\n            \"\"\"first\n    preserved\n            last\"\"\"\n            #else\n            \"\"\n            #endif\n            ]\n",
             "1",
             "module M\n\nlet r =\n    let extracted = 1\n    combine extracted [\n        #if ALT\n        \"\"\"first\n    preserved\n            last\"\"\"\n        #else\n        \"\"\n        #endif\n        ]\n")>]
[<InlineData("Constant in a module-level declaration becomes a literal in front of it",
             extractToLiteral,
             greet,
             "\"Hello %s\"",
             greetWithLiteral)>]
[<InlineData("Negative number becomes a literal",
             extractToLiteral,
             """
let f () = g -1
""",
             "-1",
             """
[<Literal>]
let ExtractedConstant = -1

let f () = g ExtractedConstant
""")>]
let ``Selection is extracted`` (_scenario: string, title: string, code: string, selected: string, expected: string) =
    let code = fileOf code
    Assert.Equal(fileOf expected, extractedAt title code (selectionOf code selected))

[<Theory>]
[<InlineData("Decimal constant becomes a literal",
             extractToLiteral,
             """
let f () = id 1.5m
""",
             "1.5m",
             """
[<Literal>]
let ExtractedConstant = 1.5m

let f () = id ExtractedConstant
""")>]
[<InlineData("Literal attribute is qualified where another LiteralAttribute hides it",
             extractToLiteral,
             """
type LiteralAttribute(required: int) =
    inherit System.Attribute()

let x = 42
""",
             "42",
             """
type LiteralAttribute(required: int) =
    inherit System.Attribute()

[<global.Microsoft.FSharp.Core.Literal>]
let ExtractedConstant = 42

let x = ExtractedConstant
""")>]
[<InlineData("Lambda with a function type annotation of its let",
             extractToLetBinding,
             """
let f: string -> int = (fun x -> x.Length)
""",
             "(fun x -> x.Length)",
             """
let f: string -> int =
    let extracted: string -> int = fun x -> x.Length
    extracted
""")>]
[<InlineData("Lambda over a type parameter of its function",
             extractToLetBinding,
             """
let f (xs: 'T list) = xs |> List.map (fun x -> [ x ])
""",
             "(fun x -> [ x ])",
             """
let f (xs: 'T list) =
    let extracted: 'T -> 'T list = fun x -> [ x ]
    xs |> List.map extracted
""")>]
[<InlineData("Lambda with a type variable inference introduced is not annotated",
             extractToLetBinding,
             """
let f xs = xs |> List.map (fun x -> [ x ])
""",
             "(fun x -> [ x ])",
             """
let f xs =
    let extracted = fun x -> [ x ]
    xs |> List.map extracted
""")>]
[<InlineData("Function expression with a function type annotation of its let",
             extractToLetBinding,
             """
let f: string option -> int = (function Some x -> x.Length | None -> 0)
""",
             "(function Some x -> x.Length | None -> 0)",
             """
let f: string option -> int =
    let extracted: string option -> int = function Some x -> x.Length | None -> 0
    extracted
""")>]
[<InlineData("Lambda selected without its parentheses",
             extractToLetBinding,
             """
let f (xs: string list) = xs |> List.map (fun x -> x.Length)
""",
             "fun x -> x.Length",
             """
let f (xs: string list) =
    let extracted: string -> int = fun x -> x.Length
    xs |> List.map (extracted)
""")>]
[<InlineData("Lambda argument of a method",
             extractToLetBinding,
             """
let f (xs: System.Collections.Generic.List<string>) = xs.ConvertAll(fun x -> x.Length)
""",
             "(fun x -> x.Length)",
             """
let f (xs: System.Collections.Generic.List<string>) =
    let extracted: string -> int = fun x -> x.Length
    xs.ConvertAll(extracted)
""")>]
let ``Extracted code type checks`` (_scenario: string, title: string, code: string, selected: string, expected: string) =
    let code = fileOf code
    let result = extractedAt title code (selectionOf code selected)

    Assert.Equal(fileOf expected, result)
    assertTypeChecks result

[<Theory>]
[<InlineData("Caret on the fun keyword", extractToLetBinding, mapIncrement, "fun x", mapIncrementExtracted)>]
[<InlineData("Caret on the parameter", extractToLetBinding, mapIncrement, "x ->", mapIncrementExtracted)>]
[<InlineData("Caret on the arrow", extractToLetBinding, mapIncrement, "-> x", mapIncrementExtracted)>]
[<InlineData("Caret after the arrow", extractToLetBinding, mapIncrement, " x + 1)", mapIncrementExtracted)>]
[<InlineData("Caret at the start of a string constant", extractToLiteral, greet, "\"Hello %s\" name", greetWithLiteral)>]
[<InlineData("Caret inside a string constant", extractToLiteral, greet, "llo %s", greetWithLiteral)>]
[<InlineData("Caret right after a string constant", extractToLiteral, greet, "\"| name", greetWithLiteral)>]
let ``Caret extracts the lambda or constant around it`` (_scenario: string, title: string, code: string, marker: string, expected: string) =
    let code = fileOf code
    Assert.Equal(fileOf expected, extractedAt title code (caretAt code marker))

[<Theory>]
[<InlineData("Record field", "let extracted = { Value = 0 }", "extracted.Value <- ")>]
[<InlineData("Array element", "let extracted = [| 0 |]", "extracted[0] <- ")>]
[<InlineData("Mutable local", "let mutable extracted = 0", "extracted <- ")>]
let ``Name does not collide with an assignment target from another file`` (_scenario: string, declaration: string, assignment: string) =
    let dependency =
        """
module Dependency

type Box = { mutable Value: int }

DECLARATION
"""
            .Replace("DECLARATION", declaration)

    let code =
        """
module M

open Dependency

let f () =
    ASSIGNMENT({ Value = 1 }).Value
"""
            .Replace("ASSIGNMENT", assignment)

    let expected =
        """
module M

open Dependency

let f () =
    let extracted1 = { Value = 1 }
    ASSIGNMENTextracted1.Value
"""
            .Replace("ASSIGNMENT", assignment)

    use context = TestContext.CreateWithCodeAndDependency code dependency

    Assert.Equal(
        expected,
        refactoredText (new FSharpExtractLetBindingRefactoring()) context extractToLetBinding code (selectionOf code "({ Value = 1 })")
    )

[<Theory>]
[<InlineData("Constant in a module-level declaration offers a literal too", moduleConstant, "42", bothTitles)>]
[<InlineData("Constant in a member offers no literal",
             """
type T() =
    member _.M() = 42
""",
             "42",
             extractToLetBinding)>]
[<InlineData("Expression offers no literal", "let f x = g (x + 1)", "x + 1", extractToLetBinding)>]
[<InlineData("Native integer does not become a literal", "let f () = g 1n", "1n", extractToLetBinding)>]
[<InlineData("Unsigned native integer does not become a literal", "let f () = g 1un", "1un", extractToLetBinding)>]
[<InlineData("Literal is not offered where the declaration does not start its line",
             """
namespace N
module M = let f () = 42
""",
             "42",
             extractToLetBinding)>]
[<InlineData("Operand of nameof is not extracted on its own", "let name = nameof System.String.Empty", "System.String.Empty", "")>]
[<InlineData("The nameof expression is extracted",
             "let name = nameof System.String.Empty",
             "nameof System.String.Empty",
             extractToLetBinding)>]
[<InlineData("Selection that is not a whole expression", "let f w h = w * h + 1", "w * h +", "")>]
[<InlineData("Tail of a chain of operators", "let f () = 1 + 2 + 3", "2 + 3", "")>]
[<InlineData("Then branch of an if with an else",
             """
let f c x y =
    if c then x + 1 else y
""",
             "x + 1",
             "")>]
[<InlineData("Condition of a while loop",
             """
let f n =
    let mutable i = 0
    while i < n do
        i <- i + 1
""",
             "i < n",
             "")>]
[<InlineData("Guard of a match clause",
             """
let f x =
    match x with
    | v when v > 0 -> v
    | _ -> 0
""",
             "v > 0",
             "")>]
[<InlineData("Hole of an interpolated string", "let f a b = $\"{a + b}\"", "a + b", "")>]
[<InlineData("Return in a computation expression",
             """
let load id = async {
    return id }
""",
             "return id",
             "")>]
[<InlineData("Arguments of a method call", "let f (sb: System.Text.StringBuilder) a b = sb.Insert(a, b)", "a, b", "")>]
[<InlineData("Identifier", "let f x = x.ToString() + \"\"", "x", "")>]
[<InlineData("Right operand of &&", "let f (a: bool) (s: string) = a && s.Length > 0", "s.Length > 0", "")>]
[<InlineData("Operand of assert",
             """
let f check x =
    assert (check (x + 1))
""",
             "check (x + 1)",
             "")>]
[<InlineData("Parenthesized receiver of a call on a struct",
             """
[<Struct>]
type S =
    val mutable X: int
    new(x) = { X = x }
    member s.Inc() = s.X <- s.X + 1

let f () =
    let mutable s = S(1)
    (s).Inc()
    s.X
""",
             "(s)",
             "")>]
[<InlineData("Address of a local",
             """
let f () =
    let mutable x = 0
    System.Threading.Interlocked.Increment(&x)
""",
             "(&x)",
             "")>]
[<InlineData("Empty selection", "let f x = x + 1", "", "")>]
let ``Actions offered for a selection`` (_scenario: string, code: string, selected: string, titles: string) =
    let code = fileOf code
    Assert.Equal(titles, titlesAt code (selectionOf code selected))

[<Theory>]
[<InlineData("Caret on the fun keyword of a parenthesized lambda", mapIncrement, "fun x", extractToLetBinding)>]
[<InlineData("Caret on the parameter of a parenthesized lambda", mapIncrement, "x ->", extractToLetBinding)>]
[<InlineData("Caret on the arrow of a parenthesized lambda", mapIncrement, "-> x", extractToLetBinding)>]
[<InlineData("Caret after the arrow of a parenthesized lambda", mapIncrement, " x + 1)", extractToLetBinding)>]
[<InlineData("Caret inside a constant offers only the literal", moduleConstant, "2", extractToLiteral)>]
[<InlineData("Caret in the body of a lambda", mapIncrement, "x + 1)", "")>]
[<InlineData("Caret on the operator of a lambda body", mapIncrement, "+ 1)", "")>]
[<InlineData("Caret before the parenthesis of a lambda", mapIncrement, "(fun", "")>]
[<InlineData("Caret on a lambda without parentheses", "let f = fun x -> x + 1", "fun", "")>]
[<InlineData("Caret in an application", "let f x = g x", "g x", "")>]
[<InlineData("Caret in a constant of a member",
             """
type T() =
    member _.M() = 42
""",
             "42",
             "")>]
[<InlineData("Caret in a constant of an interpolated string hole", "let f a = $\"{a + 1}\"", "1}", "")>]
[<InlineData("Caret in a native integer", "let f () = g 1n", "1n", "")>]
[<InlineData("Caret in an unsigned native integer", "let f () = g 1un", "1un", "")>]
[<InlineData("Caret in a constant of a declaration that does not start its line",
             """
namespace N
module M = let f () = 42
""",
             "42",
             "")>]
let ``Actions offered at a caret`` (_scenario: string, code: string, marker: string, titles: string) =
    let code = fileOf code
    Assert.Equal(titles, titlesAt code (caretAt code marker))
