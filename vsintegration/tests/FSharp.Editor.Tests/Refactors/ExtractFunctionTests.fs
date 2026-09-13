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

let private extractedWith (setting: ParameterAnnotationSetting) (title: string) (code: string) (selected: string) =
    use context = contextFor setting code

    let document =
        refactorSpan code (selectionOf code selected) title context (new FSharpExtractFunctionRefactoring())

    let parseResults =
        document.GetFSharpParseResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty(parseResults.Diagnostics)
    (document.GetTextAsync() |> GetTaskResult).ToString()

let private extracted = extractedWith ParameterAnnotationSetting.Always

let private titlesFor (code: string) (selected: string) =
    use context = contextFor ParameterAnnotationSetting.Always code

    tryGetRefactoringActionsForSpan code (selectionOf code selected) context (new FSharpExtractFunctionRefactoring())
    |> Seq.map _.Title
    |> List.ofSeq

let private area =
    "module M\n\nlet area (w: int) (h: int) =\n    printfn \"%d\" (w * h + 1)\n"

[<Fact>]
let ``Captured parameters become parameters of a local function`` () =
    let expected =
        "module M\n\nlet area (w: int) (h: int) =\n    let extractedFunction (w: int) (h: int) = w * h + 1\n    printfn \"%d\" (extractedFunction w h)\n"

    Assert.Equal(expected, extracted extractToLocalFunction area "w * h + 1")

[<Fact>]
let ``Module function is declared in front of the declaration using it`` () =
    let expected =
        "module M\n\nlet private extractedFunction (w: int) (h: int) = w * h + 1\n\nlet area (w: int) (h: int) =\n    printfn \"%d\" (extractedFunction w h)\n"

    Assert.Equal(expected, extracted extractToModuleFunction area "w * h + 1")

[<Fact>]
let ``Call keeps its parentheses where the selection lost them`` () =
    let expected =
        "module M\n\nlet area (w: int) (h: int) =\n    let extractedFunction (w: int) (h: int) = w * h + 1\n    printfn \"%d\" (extractedFunction w h)\n"

    Assert.Equal(expected, extracted extractToLocalFunction area "(w * h + 1)")

[<Fact>]
let ``Selection without captures becomes a function of unit`` () =
    let code = "module M\n\nlet f () =\n    printfn \"%d\" (1 + 2)\n"

    let expected =
        "module M\n\nlet f () =\n    let extractedFunction () = 1 + 2\n    printfn \"%d\" (extractedFunction ())\n"

    Assert.Equal(expected, extracted extractToLocalFunction code "1 + 2")

[<Fact>]
let ``Selection using this becomes a private member`` () =
    let code =
        "module M\n\ntype Order(lines: int list) =\n    member this.Rate = 3\n    member this.Total = lines |> List.sumBy (fun l -> l * this.Rate)\n"

    let expected =
        "module M\n\ntype Order(lines: int list) =\n    member this.Rate = 3\n    member this.Total = lines |> List.sumBy (fun l -> this.ExtractedMethod(l))\n    member private this.ExtractedMethod(l: int) = l * this.Rate\n"

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
        "module M\n\nlet shout (s: string) (n: int) =\n    printfn \"%s\" (s.ToUpper() + string n)\n"

    let expected =
        $"module M\n\nlet shout (s: string) (n: int) =\n    {header} s.ToUpper() + string n\n    printfn \"%%s\" (extractedFunction s n)\n"

    Assert.Equal(expected, extractedWith setting extractToLocalFunction code "s.ToUpper() + string n")

[<Fact>]
let ``Variants follow what the selection uses`` () =
    Assert.Equal<string list>([ extractToLocalFunction; extractToModuleFunction ], titlesFor area "w * h + 1")

    let derived =
        "module M\n\ntype Derived() =\n    inherit System.Object()\n    override this.ToString() = base.ToString() + \"!\"\n"

    Assert.Equal<string list>([ extractToPrivateMember ], titlesFor derived "base.ToString() + \"!\"")

[<Theory>]
[<InlineData("module M\n\nlet counter () =\n    let mutable n = 0\n    for i in 1 .. 3 do\n        n <- n + i\n    n\n",
             "for i in 1 .. 3 do\n        n <- n + i")>]
[<InlineData("module M\n\nlet incr (x: byref<int>) =\n    x <- x + 1\n", "x + 1")>]
let ``No action`` (code: string, selected: string) = Assert.Empty(titlesFor code selected)
