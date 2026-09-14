module FSharp.Editor.Tests.Refactors.ConvertTupleTests

open System

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open Xunit

open FSharp.Compiler.Diagnostics

open FSharp.Editor.Tests.Refactors.RefactorTestFramework

let private caretAt (code: string) (marker: string) =
    code.IndexOf(marker, StringComparison.Ordinal)

let private textOf (document: Document) =
    (document.GetTextAsync() |> GetTaskResult).ToString()

let private errorsOf (document: Document) =
    let _, checkResults =
        document.GetFSharpParseAndCheckResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    checkResults.Diagnostics
    |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

let private refactorIn (context: TestContext) (code: string) (marker: string) =
    tryRefactor code (caretAt code marker) context (new FSharpConvertTupleRefactoring())

let private refactored (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code
    let document = refactorIn context code marker
    Assert.Empty(errorsOf document)
    textOf document

let private actionsAt (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code
    tryGetRefactoringActions code (caretAt code marker) context (new FSharpConvertTupleRefactoring())

[<Theory>]
[<InlineData("""
module M

let show () = printfn "%A" (1, 2)
""",
             "1, 2",
             """
module M

let show () = printfn "%A" struct (1, 2)
""")>]
[<InlineData("""
module M

let pairs: list<int * int> = []
""",
             "int * int",
             """
module M

let pairs: list<struct (int * int)> = []
""")>]
let ``Tuple that flows nowhere else converts on its own`` (before: string, marker: string, after: string) =
    Assert.Equal(after, refactored before marker)
    Assert.Equal(before, refactored after marker)

[<Fact>]
let ``Value, its tuple patterns and the annotations it flows into convert together`` () =
    let reference =
        """
module M

let pair = (1, 2)
let (a, b) = pair
let copy: int * int = pair
"""

    let structs =
        """
module M

let pair = struct (1, 2)
let struct (a, b) = pair
let copy: struct (int * int) = pair
"""

    Assert.Equal(structs, refactored reference "1, 2")
    Assert.Equal(reference, refactored structs "1, 2")

[<Fact>]
let ``Parameter annotation converts its arguments and the patterns on it`` () =
    let reference =
        """
module M

let sum (p: int * int) =
    let (a, b) = p
    a + b

let pair = (3, 4)
let total = sum pair + sum (1, 2)
"""

    let structs =
        """
module M

let sum (p: struct (int * int)) =
    let struct (a, b) = p
    a + b

let pair = struct (3, 4)
let total = sum pair + sum struct (1, 2)
"""

    Assert.Equal(structs, refactored reference "int * int")
    Assert.Equal(reference, refactored structs "int * int")

[<Theory>]
[<InlineData("""
module M

let scale (factor: int) (p: int * int) =
    let (x, y) = p
    x * factor + y

let scaled = scale 2 (1, 2)
""",
             """
module M

let scale (factor: int) (p: struct (int * int)) =
    let struct (x, y) = p
    x * factor + y

let scaled = scale 2 struct (1, 2)
""")>]
[<InlineData("""
module M

let area (p: int * int, factor: int) =
    let (w, h) = p
    w * h * factor

let total = area ((2, 3), 4)
""",
             """
module M

let area (p: struct (int * int), factor: int) =
    let struct (w, h) = p
    w * h * factor

let total = area (struct (2, 3), 4)
""")>]
let ``Parameter converts the matching argument of every call`` (reference: string, structs: string) =
    Assert.Equal(structs, refactored reference "int * int")
    Assert.Equal(reference, refactored structs "int * int")

[<Theory>]
[<InlineData("""
module M

let add (a, b) c = a + b + c

let total = add (1, 2) 3
""",
             "a, b",
             """
module M

let add struct (a, b) c = a + b + c

let total = add struct (1, 2) 3
""")>]
[<InlineData("""
module M

type Calc() =
    member _.Add (a: int, b: int) (c: int) = a + b + c

let total = Calc().Add (1, 2) 3
""",
             "a: int",
             """
module M

type Calc() =
    member _.Add struct (a: int, b: int) (c: int) = a + b + c

let total = Calc().Add struct (1, 2) 3
""")>]
let ``Tuple argument of a curried function or member converts with its calls`` (reference: string, marker: string, structs: string) =
    Assert.Equal(structs, refactored reference marker)
    Assert.Equal(reference, refactored structs marker)

[<Fact>]
let ``Struct keyword is separated from a name the parenthesis follows`` () =
    let code =
        """
module M

let add(a, b) c = a + b + c

let total = add (1, 2) 3
"""

    Assert.Equal(
        """
module M

let add struct (a, b) c = a + b + c

let total = add struct (1, 2) 3
""",
        refactored code "a, b"
    )

[<Fact>]
let ``Value declared in another file converts there`` () =
    let definition =
        """
module A

let pair = (1, 2)
"""

    let code =
        """
module B

let (a, b) = A.pair
"""

    use context = TestContext.CreateWithCodeAndDependency code definition
    let document = refactorIn context code "a, b"

    Assert.Equal(
        """
module B

let struct (a, b) = A.pair
""",
        textOf document
    )

    Assert.Equal(
        """
module A

let pair = struct (1, 2)
""",
        (context.Solution.Projects |> Seq.head).Documents |> Seq.head |> textOf
    )

    // The checker reads the other file of a synthetic project from disk, so the result is checked as a new project.
    let definitionAfter =
        (context.Solution.Projects |> Seq.head).Documents |> Seq.head |> textOf

    use checkContext =
        TestContext.CreateWithCodeAndDependency (textOf document) definitionAfter

    Assert.Empty((checkContext.Solution.Projects |> Seq.head).Documents |> Seq.last |> errorsOf)

[<Fact>]
let ``Use that cannot be followed is left for the compiler to report`` () =
    let code =
        """
module M

let pair = (1, 2)
let first = fst pair
"""

    use context = TestContext.CreateWithCode code
    let document = refactorIn context code "1, 2"

    Assert.Equal(
        """
module M

let pair = struct (1, 2)
let first = fst pair
""",
        textOf document
    )

    Assert.Equal(5, (errorsOf document |> Array.exactlyOne).StartLine)

[<Fact>]
let ``Return type converts the result and the patterns taking it apart`` () =
    let reference =
        """
module M

let origin () : int * int = (0, 0)
let (x, y) = origin ()
"""

    let structs =
        """
module M

let origin () : struct (int * int) = struct (0, 0)
let struct (x, y) = origin ()
"""

    Assert.Equal(structs, refactored reference "int * int")
    Assert.Equal(reference, refactored structs "int * int")

[<Fact>]
let ``Record field converts its values and the patterns on it`` () =
    let reference =
        """
module M

type Line = { Start: int * int; Finish: int * int }

let line = { Start = (0, 0); Finish = (1, 1) }
let (sx, sy) = line.Start
"""

    let structs =
        """
module M

type Line = { Start: struct (int * int); Finish: int * int }

let line = { Start = struct (0, 0); Finish = (1, 1) }
let struct (sx, sy) = line.Start
"""

    Assert.Equal(structs, refactored reference "int * int; Finish")
    Assert.Equal(reference, refactored structs "int * int); Finish")

[<Fact>]
let ``Title names the target kind`` () =
    let reference =
        """
module M

let pair = (1, 2)
"""

    let structs =
        """
module M

let pair = struct (1, 2)
"""

    Assert.Equal("Convert to struct tuple", (actionsAt reference "1, 2" |> Seq.exactlyOne).Title)
    Assert.Equal("Convert to reference tuple", (actionsAt structs "1, 2" |> Seq.exactlyOne).Title)

[<Theory>]
[<InlineData("""
module M

let x = 1
""",
             "1")>]
[<InlineData("""
module M

let biggest = System.Math.Max(1, 2)
""",
             "1, 2")>]
[<InlineData("""
module M

let quoted = <@ (1, 2) @>
""",
             "1, 2")>]
[<InlineData("""
module M

type Greeter() =
    member _.Greet(name: string, ?greeting: string) = name
""",
             "?greeting")>]
[<InlineData("""
module M

type Calc() =
    member _.Add struct (a: int, b: int) = a + b
""",
             "a: int")>]
[<InlineData("""
module M

type Point(x: int, y: int) =
    new(x: int, y: int, z: int) = Point(x + z, y)
""",
             "z: int")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)
