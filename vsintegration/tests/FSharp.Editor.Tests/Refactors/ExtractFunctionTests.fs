module FSharp.Editor.Tests.Refactors.ExtractFunctionTests

open System

open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open Xunit

open FSharp.Editor.Tests.Helpers
open FSharp.Editor.Tests.Refactors.RefactorTestFramework

let private extractToLocalFunction = "Extract to local function"
let private extractToModuleFunction = "Extract to module function"
let private extractToPrivateMember = "Extract to private member"

let private selectionOf (code: string) (selected: string) =
    TextSpan(code.IndexOf(selected, StringComparison.Ordinal), selected.Length)

let private contextFor (setting: ParameterAnnotationSetting) (code: string) =
    let options =
        { CodeFixesOptions.Default with
            ExtractFunctionParameterAnnotations = setting
        }

    new TestContext(RoslynTestHelpers.CreateSolution(code, editorOptions = options))

let private caretAt (code: string) (marker: string) =
    TextSpan(code.IndexOf(marker, StringComparison.Ordinal), 0)

let private extractedAtWith (setting: ParameterAnnotationSetting) (title: string) (code: string) (span: TextSpan) =
    use context = contextFor setting code

    let document =
        refactorSpan code span title context (new FSharpExtractFunctionRefactoring())

    let parseResults =
        document.GetFSharpParseResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty(parseResults.Diagnostics)
    (document.GetTextAsync() |> GetTaskResult).ToString()

let private extractedWith (setting: ParameterAnnotationSetting) (title: string) (code: string) (selected: string) =
    extractedAtWith setting title code (selectionOf code selected)

let private extracted = extractedWith ParameterAnnotationSetting.Always

let private titlesAt (code: string) (span: TextSpan) =
    use context = contextFor ParameterAnnotationSetting.Always code

    tryGetRefactoringActionsForSpan code span context (new FSharpExtractFunctionRefactoring())
    |> Seq.map _.Title
    |> List.ofSeq

let private titlesFor (code: string) (selected: string) =
    titlesAt code (selectionOf code selected)

[<Fact>]
let ``Caret in the header of a parenthesized lambda extracts it as if it were selected`` () =
    let code =
        """
module M

let f (xs: int list) (n: int) =
    xs |> List.map (fun x -> x + n)
"""

    let lambda = "(fun x -> x + n)"
    let titles = titlesFor code lambda

    Assert.NotEmpty(titles)
    Assert.Equal<string list>(titles, titlesAt code (caretAt code "x ->"))

    for title in titles do
        Assert.Equal(extracted title code lambda, extractedAtWith ParameterAnnotationSetting.Always title code (caretAt code "x ->"))

    Assert.Empty(titlesAt code (caretAt code "x + n"))

let private area =
    """
module M

let area (w: int) (h: int) =
    printfn "%d" (w * h + 1)
"""

let private areaWithLocalFunction =
    """
module M

let area (w: int) (h: int) =
    let extractedFunction (w: int) (h: int) = w * h + 1
    printfn "%d" (extractedFunction w h)
"""

[<Fact>]
let ``Captured parameters become parameters of a local function`` () =
    Assert.Equal(areaWithLocalFunction, extracted extractToLocalFunction area "w * h + 1")

[<Fact>]
let ``Module function is declared in front of the declaration using it`` () =
    let expected =
        """
module M

let private extractedFunction (w: int) (h: int) = w * h + 1

let area (w: int) (h: int) =
    printfn "%d" (extractedFunction w h)
"""

    Assert.Equal(expected, extracted extractToModuleFunction area "w * h + 1")

[<Fact>]
let ``Call keeps its parentheses where the selection lost them`` () =
    Assert.Equal(areaWithLocalFunction, extracted extractToLocalFunction area "(w * h + 1)")

[<Fact>]
let ``Selection without captures becomes a function of unit`` () =
    let code =
        """
module M

let f () =
    printfn "%d" (1 + 2)
"""

    let expected =
        """
module M

let f () =
    let extractedFunction () = 1 + 2
    printfn "%d" (extractedFunction ())
"""

    Assert.Equal(expected, extracted extractToLocalFunction code "1 + 2")

[<Fact>]
let ``Selection using this becomes a private member`` () =
    let code =
        """
module M

type Order(lines: int list) =
    member this.Rate = 3
    member this.Total = lines |> List.sumBy (fun l -> l * this.Rate)
"""

    let expected =
        """
module M

type Order(lines: int list) =
    member this.Rate = 3
    member this.Total = lines |> List.sumBy (fun l -> this.ExtractedMethod(l))
    member private this.ExtractedMethod(l: int) = l * this.Rate
"""

    Assert.Equal(expected, extracted extractToPrivateMember code "l * this.Rate")

[<Theory>]
[<InlineData("Always", "let extractedFunction (s: string) (n: int) =")>]
[<InlineData("WhenNeeded", "let extractedFunction (s: string) n =")>]
[<InlineData("Never", "let extractedFunction s n =")>]
let ``Parameter annotations follow the option`` (setting: string, header: string) =
    let setting =
        match setting with
        | "Always" -> ParameterAnnotationSetting.Always
        | "WhenNeeded" -> ParameterAnnotationSetting.WhenNeeded
        | _ -> ParameterAnnotationSetting.Never

    let code =
        """
module M

let shout (s: string) (n: int) =
    printfn "%s" (s.ToUpper() + string n)
"""

    let expected =
        $"""
module M

let shout (s: string) (n: int) =
    {header} s.ToUpper() + string n
    printfn "%%s" (extractedFunction s n)
"""

    Assert.Equal(expected, extractedWith setting extractToLocalFunction code "s.ToUpper() + string n")

[<Fact>]
let ``Variants follow what the selection uses`` () =
    Assert.Equal<string list>([ extractToLocalFunction; extractToModuleFunction ], titlesFor area "w * h + 1")

    let derived =
        """
module M

type Derived() =
    inherit System.Object()
    override this.ToString() = base.ToString() + "!"
"""

    Assert.Equal<string list>([ extractToPrivateMember ], titlesFor derived "base.ToString() + \"!\"")

[<Fact>]
let ``No action when the selection assigns to a captured mutable local`` () =
    let code =
        """
module M

let counter () =
    let mutable n = 0
    for i in 1 .. 3 do
        n <- n + i
    n
"""

    let start = code.IndexOf("for i in", StringComparison.Ordinal)

    let finish =
        code.IndexOf("n <- n + i", start, StringComparison.Ordinal)
        + "n <- n + i".Length

    Assert.Empty(titlesAt code (TextSpan.FromBounds(start, finish)))

[<Fact>]
let ``No action when the selection reads a byref parameter`` () =
    let code =
        """
module M

let incr (x: byref<int>) =
    x <- x + 1
"""

    Assert.Empty(titlesFor code "x + 1")
