module FSharp.Editor.Tests.Refactors.ConvertDotLambdaTests

open System

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open Xunit

open FSharp.Editor.Tests.Refactors.RefactorTestFramework

let private inModule (binding: string) =
    let definition =
        if binding.StartsWith("let ", StringComparison.Ordinal) then
            binding
        else
            $"let r = {binding}"

    $"""
module M

{definition}
"""

let private caretAt (code: string) (marker: string) =
    code.IndexOf(marker, StringComparison.Ordinal)

let private refactored (code: string) (marker: string) =
    use context = TestContext.CreateWithCode code

    let document =
        tryRefactor code (caretAt code marker) context (new FSharpConvertDotLambdaRefactoring())

    let parseResults =
        document.GetFSharpParseResultsAsync "test"
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.False(parseResults.ParseHadErrors, $"%A{parseResults.Diagnostics}")

    (document.GetTextAsync() |> GetTaskResult).ToString()

[<Theory>]
[<InlineData("xs |> List.map (fun x -> x.Prop)", "xs |> List.map _.Prop")>]
[<InlineData("List.map (fun x -> x.A.B) xs", "List.map _.A.B xs")>]
[<InlineData("Seq.filter (fun x -> x.M(y))", "Seq.filter _.M(y)")>]
[<InlineData("Array.map (fun x -> x.Xs[0]) arr", "Array.map _.Xs[0] arr")>]
[<InlineData("List.map (fun x -> x.M<int>()) xs", "List.map _.M<int>() xs")>]
[<InlineData("List.map (fun x -> x.Xs.[0]) xs", "List.map _.Xs.[0] xs")>]
[<InlineData("List.map (fun x -> x.M().P) xs", "List.map _.M().P xs")>]
[<InlineData("List.map ( fun x -> x.P ) xs", "List.map _.P xs")>]
[<InlineData("List.map (fun x -> x.P) xs", "List.map _.P xs")>]
[<InlineData("List.map (fun ``x`` -> ``x``.P) xs", "List.map _.P xs")>]
[<InlineData("x.M(fun y -> y.P)", "x.M(_.P)")>]
[<InlineData("fun x -> x.P", "_.P")>]
[<InlineData("(fun x -> x.P)", "(_.P)")>]
[<InlineData("fun x -> x .P", "_ .P")>]
[<InlineData("fun x -> x (* why *) .P", "_ (* why *) .P")>]
[<InlineData("List.map (fun x -> x.M(_argument)) xs", "List.map _.M(_argument) xs")>]
[<InlineData("List.map (fun x ->\n    x.Prop) xs", "List.map _.Prop xs")>]
[<InlineData("let r =\n    fun x ->\n        x.P",
             "let r =\n    _.P",
             TestDisplayName = "Lambda spanning multiple lines still converts to shorthand")>]
let ``Lambda reading a member of its parameter converts to shorthand`` (before: string, after: string) =
    Assert.Equal(inModule after, refactored (inModule before) "fun")

[<Theory>]
[<InlineData("_.", "_.P", "fun x -> x.P")>]
[<InlineData("_.", "_.A.B", "fun x -> x.A.B")>]
[<InlineData("_.", "_.Xs[0]", "fun x -> x.Xs[0]")>]
[<InlineData("_.", "List.map _.P xs", "List.map (fun x -> x.P) xs")>]
[<InlineData("_.", "x.M(_.P)", "x.M(fun x -> x.P)")>]
[<InlineData("_.", "xs |> List.map _.M(x)", "xs |> List.map (fun x1 -> x1.M(x))")>]
[<InlineData("_.", "_.M(_.P)", "fun x -> x.M(_.P)")>]
[<InlineData("_.", "_.Replace(\"a\", \"\"\"x\ny\"\"\")", "fun x -> x.Replace(\"a\", \"\"\"x\ny\"\"\")")>]
[<InlineData("_ ", "_ .P", "fun x -> x .P")>]
[<InlineData("_ ", "_ (* why *) .P", "fun x -> x (* why *) .P")>]
[<InlineData("_.P", "_.M(_.P)", "_.M(fun x -> x.P)", TestDisplayName = "Nested shorthand converts on its own")>]
[<InlineData("_.",
             "let f : string -> string =\n    _.PadLeft(\n        3\n    )",
             "let f : string -> string =\n    (fun x -> x.PadLeft(\n        3\n    ))",
             TestDisplayName = "Shorthand whose arguments span multiple lines converts to lambda")>]
let ``Shorthand converts to lambda`` (marker: string, before: string, after: string) =
    Assert.Equal(inModule after, refactored (inModule before) marker)

[<Theory>]
[<InlineData("fun x y -> x.P")>]
[<InlineData("fun (x: T) -> x.P")>]
[<InlineData("fun (x) -> x.P")>]
[<InlineData("fun x -> (x.P)")>]
[<InlineData("fun x -> f x.P")>]
[<InlineData("fun x -> x.M y")>]
[<InlineData("fun x -> x.P + 1")>]
[<InlineData("fun x -> x.M(x)")>]
[<InlineData("fun x -> x.M(fun x -> x)")>]
[<InlineData("fun x -> x.M(x <- 1)")>]
[<InlineData("fun x -> x.M(x.P <- 1)")>]
[<InlineData("fun x -> x.M(x.Item(0) <- 1)")>]
[<InlineData("fun x -> x.M(x.M().P <- 1)")>]
[<InlineData("fun x -> x.M(x.Xs[0] <- 1)")>]
[<InlineData("fun x -> x.")>]
[<InlineData("fun x -> y.P")>]
[<InlineData("fun x -> x")>]
[<InlineData("fun x -> x[0]")>]
[<InlineData("fun _ -> 1")>]
[<InlineData("(fun x -> x.P) y")>]
[<InlineData("((fun x -> x.P)) y")>]
[<InlineData("fun x -> (* why *) x.P")>]
[<InlineData("fun (* why *) x -> x.P")>]
[<InlineData("fun x -> // why\n    x.P")>]
[<InlineData("List.map (fun x -> (* why *) x.P) xs")>]
[<InlineData("fun x -> x.CompareTo(_arg1)")>]
[<InlineData("<@ fun x -> x.P @>")>]
[<InlineData("<@ _.P @>")>]
[<InlineData("_.Trim()\n            .Length")>]
[<InlineData("_.Replace(\"a\",\n        \"b\").Trim()")>]
[<InlineData("1 + 2")>]
let ``No action`` (binding: string) =
    let code = inModule binding
    use context = TestContext.CreateWithCode code

    let marker =
        [ "fun"; "_."; "+" ]
        |> List.find (fun marker -> binding.IndexOf(marker, StringComparison.Ordinal) >= 0)

    Assert.Empty(tryGetRefactoringActions code (caretAt code marker) context (new FSharpConvertDotLambdaRefactoring()))
