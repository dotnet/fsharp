module FSharp.Editor.Tests.Refactors.ConvertTupleTests

open Microsoft.VisualStudio.FSharp.Editor

open Xunit

open FSharp.Editor.Tests.Refactors.RefactorTestFramework

let private refactorIn (context: TestContext) (code: string) (marker: string) =
    refactorAt (new FSharpConvertTupleRefactoring()) context code marker

let private refactored (code: string) (marker: string) =
    refactoredBy (new FSharpConvertTupleRefactoring()) code marker

let private actionsAt (code: string) (marker: string) =
    actionsBy (new FSharpConvertTupleRefactoring()) code marker

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
[<InlineData("""
module M

let pair = Some (1, 2)
""",
             "1, 2",
             """
module M

let pair = Some struct (1, 2)
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

[<Theory>]
[<InlineData("""
module M

let add(a, b) c = a + b + c

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

let nothing = ignore<_>(1, 2)
""",
             "1, 2",
             """
module M

let nothing = ignore<_> struct (1, 2)
""")>]
let ``Struct keyword is separated from a name or type arguments the parenthesis follows`` (code: string, marker: string, after: string) =
    Assert.Equal(after, refactored code marker)

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

[<Theory>]
[<InlineData("int * int")>]
[<InlineData("0, 0")>]
[<InlineData("x, y")>]
let ``Return type converts the result and the patterns taking it apart`` (marker: string) =
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

    Assert.Equal(structs, refactored reference marker)
    Assert.Equal(reference, refactored structs marker)

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

let make () =
    try (1, 2)
    finally ()

let (x, y) = make ()
""",
             """
module M

let make () =
    try struct (1, 2)
    finally ()

let struct (x, y) = make ()
""")>]
[<InlineData("""
module M

let make () =
    try (1, 2)
    with _ -> (0, 0)

let (x, y) = make ()
""",
             """
module M

let make () =
    try struct (1, 2)
    with _ -> struct (0, 0)

let struct (x, y) = make ()
""")>]
let ``Result flowing out of try converts every branch of it`` (reference: string, structs: string) =
    Assert.Equal(structs, refactored reference "x, y")
    Assert.Equal(reference, refactored structs "x, y")

[<Theory>]
[<InlineData("""
module M

let pair = (1, 2)
let copy = match pair with | p -> p
let (a, b) = copy
""",
             """
module M

let pair = struct (1, 2)
let copy = match pair with | p -> p
let struct (a, b) = copy
""")>]
[<InlineData("""
module M

let pair = (1, 2)
let alias = pair
let (a, b) = alias
""",
             """
module M

let pair = struct (1, 2)
let alias = pair
let struct (a, b) = alias
""")>]
let ``Value converts through the names it is bound to again`` (reference: string, structs: string) =
    Assert.Equal(structs, refactored reference "1, 2")
    Assert.Equal(reference, refactored structs "1, 2")
    Assert.Equal(structs, refactored reference "a, b")
    Assert.Equal(reference, refactored structs "a, b")

[<Theory>]
[<InlineData("int * int")>]
[<InlineData("a, b")>]
let ``Annotated value in parentheses converts with its initializer`` (marker: string) =
    let reference =
        """
module M

let (pair: int * int) = (1, 2)
let (a, b) = pair
"""

    let structs =
        """
module M

let (pair: struct (int * int)) = struct (1, 2)
let struct (a, b) = pair
"""

    Assert.Equal(structs, refactored reference marker)
    Assert.Equal(reference, refactored structs marker)

[<Theory>]
[<InlineData("""
module M

let ((a, b): int * int) = (1, 2)
""",
             """
module M

let (struct (a, b): struct (int * int)) = struct (1, 2)
""")>]
[<InlineData("""
module M

let pair = (1, 2)
let total = match pair with | ((a, b): int * int) -> a + b
""",
             """
module M

let pair = struct (1, 2)
let total = match pair with | (struct (a, b): struct (int * int)) -> a + b
""")>]
[<InlineData("""
module M

let sum ((a, b): int * int) = a + b

let total = sum (1, 2)
""",
             """
module M

let sum (struct (a, b): struct (int * int)) = a + b

let total = sum struct (1, 2)
""")>]
[<InlineData("""
module M

let sum = fun ((a, b): int * int) -> a + b

let total = sum (1, 2)
""",
             """
module M

let sum = fun (struct (a, b): struct (int * int)) -> a + b

let total = sum struct (1, 2)
""")>]
let ``Annotated tuple pattern converts with its annotation and the value it takes apart`` (reference: string, structs: string) =
    for marker in [ "a, b"; "int * int"; "1, 2" ] do
        Assert.Equal(structs, refactored reference marker)
        Assert.Equal(reference, refactored structs marker)

[<Theory>]
[<InlineData("""
module M

let sum (a, b) = a + b
let value = (sum) (1, 2)
""",
             """
module M

let sum struct (a, b) = a + b
let value = (sum) struct (1, 2)
""")>]
[<InlineData("""
module M

let make () = (1, 2)
let (a, b) = (make) ()
""",
             """
module M

let make () = struct (1, 2)
let struct (a, b) = (make) ()
""")>]
[<InlineData("""
module M

let sum = fun (a, b) -> a + b
let value = sum (1, 2)
""",
             """
module M

let sum = fun struct (a, b) -> a + b
let value = sum struct (1, 2)
""")>]
[<InlineData("""
module M

let make = fun () -> (1, 2)
let (a, b) = make ()
""",
             """
module M

let make = fun () -> struct (1, 2)
let struct (a, b) = make ()
""")>]
[<InlineData("""
module M

let make () = (1, 2)
let alias = make
let (a, b) = alias ()
""",
             """
module M

let make () = struct (1, 2)
let alias = make
let struct (a, b) = alias ()
""")>]
[<InlineData("""
module M

let scale (factor: int) (a, b) = a * factor + b
let double = scale 2
let scaled = double (1, 2)
""",
             """
module M

let scale (factor: int) struct (a, b) = a * factor + b
let double = scale 2
let scaled = double struct (1, 2)
""")>]
let ``Function in parentheses, bound as a lambda, aliased or partly applied converts with its calls`` (reference: string, structs: string) =
    for marker in [ "1, 2"; "a, b" ] do
        Assert.Equal(structs, refactored reference marker)
        Assert.Equal(reference, refactored structs marker)

[<Theory>]
[<InlineData("""
module M

type C() =
    member _.Sum(p: int * int, n: int) =
        let (a, b) = p
        a + b + n

let pair = (1, 2)
let value = C().Sum(p = pair, n = 0)
""",
             """
module M

type C() =
    member _.Sum(p: struct (int * int), n: int) =
        let struct (a, b) = p
        a + b + n

let pair = struct (1, 2)
let value = C().Sum(p = pair, n = 0)
""",
             "1, 2|int * int")>]
[<InlineData("""
module M

type C() =
    member _.Sum(n: int, p: int * int) =
        let (a, b) = p
        a + b + n

let value = C().Sum(p = (1, 2), n = 0)
""",
             """
module M

type C() =
    member _.Sum(n: int, p: struct (int * int)) =
        let struct (a, b) = p
        a + b + n

let value = C().Sum(p = struct (1, 2), n = 0)
""",
             "1, 2|int * int")>]
[<InlineData("""
module M

type C() =
    member _.Twice(p: int * int) =
        let (a, b) = p
        (a + b) * 2

let pair = (1, 2)
let value = C().Twice(p = pair)
""",
             """
module M

type C() =
    member _.Twice(p: struct (int * int)) =
        let struct (a, b) = p
        (a + b) * 2

let pair = struct (1, 2)
let value = C().Twice(p = pair)
""",
             "1, 2")>]
let ``Named argument converts with the parameter it names`` (reference: string, structs: string, markers: string) =
    for marker in markers.Split '|' do
        Assert.Equal(structs, refactored reference marker)
        Assert.Equal(reference, refactored structs marker)

[<Theory>]
[<InlineData("""
module M

let pair a b = (a, b)
let (x, y) = pair 1 2
""",
             "x, y",
             """
module M

let pair a b = struct (a, b)
let struct (x, y) = pair 1 2
""")>]
[<InlineData("""
module M

let first (a, b) = a
let one = first (1, 2)
""",
             "1, 2",
             """
module M

let first struct (a, b) = a
let one = first struct (1, 2)
""")>]
let ``Tuple of generic elements converts with the function it goes through`` (reference: string, useMarker: string, structs: string) =
    for marker in [ "a, b"; useMarker ] do
        Assert.Equal(structs, refactored reference marker)
        Assert.Equal(reference, refactored structs marker)

[<Fact>]
let ``Comment between struct and its parenthesis is kept`` () =
    let code =
        """
module M

type R = { Pair: struct (* note *) (int * int) }
"""

    Assert.Equal(
        """
module M

type R = { Pair: (* note *) int * int }
""",
        refactored code "int * int"
    )

[<Fact>]
let ``Struct keyword is separated from a name ending in a combining mark`` () =
    let code = "\nmodule M\n\nlet f\u0301(a,b) = a+b\nlet x = f\u0301 (1,2)\n"

    Assert.Equal("\nmodule M\n\nlet f\u0301 struct (a,b) = a+b\nlet x = f\u0301 struct (1,2)\n", refactored code "a,b")

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

let biggest = System.Math.Max (1, 2)
""",
             "1, 2")>]
[<InlineData("""
module M

type Calc() =
    member _.Add(a: int, b: int) = a + b

let total = Calc().Add (1, 2)
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
[<InlineData("""
module M

type Point(x: int, y: int) =
    member _.X = x
""",
             "x: int")>]
[<InlineData("""
module M

type Calc() =
    member _.Sum(p: int * int) = fst p
""",
             "int * int")>]
[<InlineData("""
module M

type Calc() =
    member _.Add(a: int, b: int) = a + b

let total = (Calc().Add) (1, 2)
""",
             "1, 2")>]
[<InlineData("""
module M

type U = U of (int * int)

let value = U(1, 2)
""",
             "1, 2")>]
[<InlineData("""
module M

let pair =
    1,
    2
""",
             "1,")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)
