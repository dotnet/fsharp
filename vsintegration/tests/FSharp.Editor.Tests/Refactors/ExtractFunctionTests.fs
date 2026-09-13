module FSharp.Editor.Tests.Refactors.ExtractFunctionTests

open Microsoft.VisualStudio.FSharp.Editor

open Xunit

open FSharp.Editor.Tests.Helpers
open FSharp.Editor.Tests.Refactors.RefactorTestFramework

[<Literal>]
let private extractToLocalFunction = "Extract to local function"

[<Literal>]
let private extractToModuleFunction = "Extract to module function"

[<Literal>]
let private extractToPrivateMember = "Extract to private member"

[<Literal>]
let private localAndModule = extractToLocalFunction + ";" + extractToModuleFunction

[<Literal>]
let private localAndMember = extractToLocalFunction + ";" + extractToPrivateMember

[<Literal>]
let private area =
    """
let area (w: int) (h: int) =
    printfn "%d" (w * h + 1)
"""

[<Literal>]
let private areaWithLocalFunction =
    """
let area (w: int) (h: int) =
    let extractedFunction (w: int) (h: int) = w * h + 1
    printfn "%d" (extractedFunction w h)
"""

let private settingOf =
    function
    | "Always" -> ParameterAnnotationSetting.Always
    | "WhenNeeded" -> ParameterAnnotationSetting.WhenNeeded
    | _ -> ParameterAnnotationSetting.Never

let private contextFor (setting: ParameterAnnotationSetting) (code: string) =
    let options =
        { CodeFixesOptions.Default with
            ExtractFunctionParameterAnnotations = setting
        }

    new TestContext(RoslynTestHelpers.CreateSolution(code, editorOptions = options))

let private extractedAtWith setting title code span =
    use context = contextFor setting code
    refactoredText (new FSharpExtractFunctionRefactoring()) context title code span

let private titlesAt code span =
    use context = contextFor ParameterAnnotationSetting.Always code
    offeredTitles (new FSharpExtractFunctionRefactoring()) context code span

[<Fact>]
let ``Caret in the header of a parenthesized lambda extracts it as if it were selected`` () =
    let code =
        fileOf
            """
let f (xs: int list) (n: int) =
    xs |> List.map (fun x -> x + n)
"""

    let lambda = selectionOf code "(fun x -> x + n)"
    let caret = caretAt code "x ->"
    let titles = titlesAt code lambda

    Assert.True(titles.Length > 0)
    Assert.Equal(titles, titlesAt code caret)

    for title in titles.Split ';' do
        Assert.Equal(
            extractedAtWith ParameterAnnotationSetting.Always title code lambda,
            extractedAtWith ParameterAnnotationSetting.Always title code caret
        )

    Assert.Empty(titlesAt code (caretAt code "x + n"))

[<Theory>]
[<InlineData("Captured parameters become parameters of a local function", extractToLocalFunction, area, "w * h + 1", areaWithLocalFunction)>]
[<InlineData("Module function is declared in front of the declaration using it",
             extractToModuleFunction,
             area,
             "w * h + 1",
             """
let private extractedFunction (w: int) (h: int) = w * h + 1

let area (w: int) (h: int) =
    printfn "%d" (extractedFunction w h)
""")>]
[<InlineData("Call keeps its parentheses where the selection lost them", extractToLocalFunction, area, "(w * h + 1)", areaWithLocalFunction)>]
[<InlineData("Selection without captures becomes a function of unit",
             extractToLocalFunction,
             """
let f () =
    printfn "%d" (1 + 2)
""",
             "1 + 2",
             """
let f () =
    let extractedFunction () = 1 + 2
    printfn "%d" (extractedFunction ())
""")>]
[<InlineData("Selection using this becomes a private member",
             extractToPrivateMember,
             """
type Order(lines: int list) =
    member this.Rate = 3
    member this.Total = lines |> List.sumBy (fun l -> l * this.Rate)
""",
             "l * this.Rate",
             """
type Order(lines: int list) =
    member this.Rate = 3
    member this.Total = lines |> List.sumBy (fun l -> this.ExtractedMethod(l))
    member private this.ExtractedMethod(l: int) = l * this.Rate
""")>]
[<InlineData("Private member of a type with a quoted name",
             extractToPrivateMember,
             """
type ``My Type``() =
    static member M() = 1 + 2
""",
             "1 + 2",
             """
type ``My Type``() =
    static member M() = ``My Type``.ExtractedMethod()
    static member private ExtractedMethod() = 1 + 2
""")>]
[<InlineData("Private member called through a quoted self identifier",
             extractToPrivateMember,
             """
type T() =
    member ``the self``.M() = 1 + 2
""",
             "1 + 2",
             """
type T() =
    member ``the self``.M() = ``the self``.ExtractedMethod()
    member private ``the self``.ExtractedMethod() = 1 + 2
""")>]
[<InlineData("Private member is named so that it does not collide with a member",
             extractToPrivateMember,
             """
type T() =
    member this.ExtractedMethod() = 0
    member this.M() = 1 + 2
""",
             "1 + 2",
             """
type T() =
    member this.ExtractedMethod() = 0
    member this.M() = this.ExtractedMethod1()
    member private this.ExtractedMethod1() = 1 + 2
""")>]
[<InlineData("Call that is the receiver of an assignment is parenthesized",
             extractToLocalFunction,
             """
let f () =
    System.Text.StringBuilder("abc").Length <- 0
""",
             "System.Text.StringBuilder(\"abc\")",
             """
let f () =
    let extractedFunction () = System.Text.StringBuilder("abc")
    (extractedFunction ()).Length <- 0
""")>]
[<InlineData("Lines inside a multi-line string in an inactive branch are not re-indented",
             extractToLocalFunction,
             "module M\n\nlet combine (n: int) (xs: string list) = n\n\nlet r = combine 1 [\n            #if ALT\n            \"\"\"first\n    preserved\n            last\"\"\"\n            #else\n            \"\"\n            #endif\n            ]\n",
             "1",
             "module M\n\nlet combine (n: int) (xs: string list) = n\n\nlet r =\n    let extractedFunction () = 1\n    combine (extractedFunction ()) [\n        #if ALT\n        \"\"\"first\n    preserved\n            last\"\"\"\n        #else\n        \"\"\n        #endif\n        ]\n")>]
let ``Selection is extracted`` (_scenario: string, title: string, code: string, selected: string, expected: string) =
    let code = fileOf code

    Assert.Equal(fileOf expected, extractedAtWith ParameterAnnotationSetting.Always title code (selectionOf code selected))

[<Theory>]
[<InlineData("Always",
             """
let shout (s: string) (n: int) =
    printfn "%s" (s.ToUpper() + string n)
""",
             "s.ToUpper() + string n",
             """
let shout (s: string) (n: int) =
    let extractedFunction (s: string) (n: int) = s.ToUpper() + string n
    printfn "%s" (extractedFunction s n)
""")>]
[<InlineData("WhenNeeded",
             """
let shout (s: string) (n: int) =
    printfn "%s" (s.ToUpper() + string n)
""",
             "s.ToUpper() + string n",
             """
let shout (s: string) (n: int) =
    let extractedFunction (s: string) n = s.ToUpper() + string n
    printfn "%s" (extractedFunction s n)
""")>]
[<InlineData("Never",
             """
let shout (s: string) (n: int) =
    printfn "%s" (s.ToUpper() + string n)
""",
             "s.ToUpper() + string n",
             """
let shout (s: string) (n: int) =
    let extractedFunction s n = s.ToUpper() + string n
    printfn "%s" (extractedFunction s n)
""")>]
[<InlineData("WhenNeeded",
             """
let f (x: int64) = System.Math.Abs x
""",
             "System.Math.Abs x",
             """
let f (x: int64) =
    let extractedFunction (x: int64) = System.Math.Abs x
    extractedFunction x
""")>]
let ``Parameter annotations follow the option`` (setting: string, code: string, selected: string, expected: string) =
    let code = fileOf code

    Assert.Equal(fileOf expected, extractedAtWith (settingOf setting) extractToLocalFunction code (selectionOf code selected))

[<Theory>]
[<InlineData("Variants follow what the selection uses", area, "w * h + 1", localAndModule)>]
[<InlineData("A call of base offers only a private member",
             """
type Derived() =
    inherit System.Object()
    override this.ToString() = base.ToString() + "!"
""",
             "base.ToString() + \"!\"",
             extractToPrivateMember)>]
[<InlineData("Selection that assigns to a captured mutable local",
             """
let counter () =
    let mutable n = 0
    for i in 1 .. 3 do
        n <- n + i
    n
""",
             """for i in 1 .. 3 do
        n <- n + i""",
             "")>]
[<InlineData("Byref parameter",
             """
let incr (x: byref<int>) =
    x <- x + 1
""",
             "x + 1",
             "")>]
[<InlineData("Byref-like type through an abbreviation",
             """
[<Struct; System.Runtime.CompilerServices.IsByRefLike>]
type Window =
    val Length: int

type Buffer = Window

let size (buffer: Buffer) =
    buffer.Length + 1
""",
             "buffer.Length + 1",
             "")>]
[<InlineData("Generic local function",
             """
let f () =
    let identity x = x
    (identity 1, identity "s")
""",
             "(identity 1, identity \"s\")",
             "")>]
[<InlineData("Mutable local read by a closure",
             """
let f () =
    let mutable n = 0
    let read = (fun () -> n)
    n <- 1
    read ()
""",
             "(fun () -> n)",
             "")>]
[<InlineData("Mutable local read by a lambda shorthand",
             """
let f (xs: string list) =
    let mutable n = 0
    let tails = xs |> Seq.map _.Substring(n)
    n <- 1
    tails
""",
             "xs |> Seq.map _.Substring(n)",
             "")>]
[<InlineData("Mutable local read at once becomes a parameter",
             """
let f () =
    let mutable n = 0
    n <- 1
    printfn "%d" (n + 1)
""",
             "n + 1",
             localAndModule)>]
[<InlineData("Type parameter of a function stays in its scope", "let f<'T> () = typeof<'T>", "typeof<'T>", extractToLocalFunction)>]
[<InlineData("Type parameter of a type can be used by a member",
             """
type C<'T>() =
    member this.M<'U>() = (typeof<'T>, typeof<'U>)
""",
             "typeof<'T>",
             localAndMember)>]
[<InlineData("Type parameter of a method stays in its scope",
             """
type C<'T>() =
    member this.M<'U>() = (typeof<'T>, typeof<'U>)
""",
             "typeof<'U>",
             extractToLocalFunction)>]
[<InlineData("No local function for a selection using this of a struct",
             """
[<Struct>]
type S =
    val X: int
    new(x) = { X = x }
    member this.M() = this.X + 1
""",
             "this.X + 1",
             extractToPrivateMember)>]
[<InlineData("No private member where a local hides the self identifier",
             """
type C() =
    member this.M() =
        let this = "abc"
        this.Length + 1
""",
             "this.Length + 1",
             extractToLocalFunction)>]
let ``Actions offered for a selection`` (_scenario: string, code: string, selected: string, titles: string) =
    let code = fileOf code
    Assert.Equal(titles, titlesAt code (selectionOf code selected))

[<Theory>]
[<InlineData("let inline f (x: int) = x + 1", "Extract to module function", false)>]
[<InlineData("let inline internal f (x: int) = x + 1", "Extract to module function", false)>]
[<InlineData("let inline private f (x: int) = x + 1", "Extract to module function", true)>]
[<InlineData("let f (x: int) = x + 1", "Extract to module function", true)>]
[<InlineData("type T() =\n    member inline this.M(x: int) = x + 1", "Extract to private member", false)>]
[<InlineData("type T() =\n    member inline private this.M(x: int) = x + 1", "Extract to private member", true)>]
[<InlineData("type T() =\n    member this.M(x: int) = x + 1", "Extract to private member", true)>]
let ``Private function or member is offered unless a more accessible inline one would call it``
    (declaration: string, title: string, offered: bool)
    =
    let code = $"module M\n\n{declaration}\n"
    let titles = (titlesAt code (selectionOf code "x + 1")).Split ';'

    Assert.Equal(offered, Array.contains title titles)
    Assert.Contains(extractToLocalFunction, titles)

[<Literal>]
let private assignedDependency =
    """
module Dependency

type Box = { mutable Value: int }

let extractedFunction = { Value = 0 }
let ExtractedMethod = { Value = 0 }
"""

[<Theory>]
[<InlineData("Local function name does not collide with an assignment target from another file",
             extractToLocalFunction,
             """
module M

open Dependency

let f () =
    extractedFunction.Value <- ({ Value = 1 }).Value
""",
             """
module M

open Dependency

let f () =
    let extractedFunction1 () = { Value = 1 }
    extractedFunction.Value <- (extractedFunction1 ()).Value
""")>]
[<InlineData("Member name does not collide with an assignment target from another file",
             extractToPrivateMember,
             """
module M

open Dependency

type T() =
    member this.Reset() =
        ExtractedMethod.Value <- ({ Value = 1 }).Value
""",
             """
module M

open Dependency

type T() =
    member this.Reset() =
        ExtractedMethod.Value <- this.ExtractedMethod1().Value
    member private this.ExtractedMethod1() = { Value = 1 }
""")>]
let ``Name does not collide with an assignment target from another file``
    (_scenario: string, title: string, code: string, expected: string)
    =
    use context = TestContext.CreateWithCodeAndDependency code assignedDependency

    Assert.Equal(expected, refactoredText (new FSharpExtractFunctionRefactoring()) context title code (selectionOf code "({ Value = 1 })"))
