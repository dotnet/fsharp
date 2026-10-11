module FSharp.Editor.Tests.Refactors.ConvertDotLambdaTests

open System

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open Xunit

open FSharp.Editor.Tests.Refactors.RefactorTestFramework

let private inModule (binding: string) =
    $"""
module M

let r = {binding}
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
[<InlineData("List.map (fun ``x`` -> ``x``.P) xs", "List.map _.P xs")>]
[<InlineData("x.M(fun y -> y.P)", "x.M(_.P)")>]
[<InlineData("fun x -> x.P", "_.P")>]
[<InlineData("(fun x -> x.P)", "(_.P)")>]
[<InlineData("fun x -> x .P", "_ .P")>]
[<InlineData("fun x -> x (* why *) .P", "_ (* why *) .P")>]
[<InlineData("List.map (fun x -> x.M(_argument)) xs", "List.map _.M(_argument) xs")>]
[<InlineData("""List.map (fun x ->
    x.Prop) xs""",
             "List.map _.Prop xs")>]
let ``Lambda reading a member of its parameter converts to shorthand`` (before: string, after: string) =
    Assert.Equal(inModule after, refactored (inModule before) "fun")

[<Theory>]
[<InlineData("_.P", "fun x -> x.P")>]
[<InlineData("_.A.B", "fun x -> x.A.B")>]
[<InlineData("_.Xs[0]", "fun x -> x.Xs[0]")>]
[<InlineData("List.map _.P xs", "List.map (fun x -> x.P) xs")>]
[<InlineData("x.M(_.P)", "x.M(fun x -> x.P)")>]
[<InlineData("xs |> List.map _.M(x)", "xs |> List.map (fun x1 -> x1.M(x))")>]
[<InlineData("_.M(_.P)", "fun x -> x.M(_.P)")>]
[<InlineData("_.Replace(\"a\", \"\"\"x\ny\"\"\")", "fun x -> x.Replace(\"a\", \"\"\"x\ny\"\"\")")>]
let ``Shorthand converts to lambda`` (before: string, after: string) =
    Assert.Equal(inModule after, refactored (inModule before) "_.")

[<Theory>]
[<InlineData("_ .P", "fun x -> x .P")>]
[<InlineData("_ (* why *) .P", "fun x -> x (* why *) .P")>]
let ``Shorthand keeps what stands between the underscore and the dot`` (before: string, after: string) =
    Assert.Equal(inModule after, refactored (inModule before) "_ ")

[<Fact>]
let ``Shorthand whose arguments span multiple lines converts to lambda`` () =
    let before =
        """
module M

let f : string -> string =
    _.PadLeft(
        3
    )
"""

    let after =
        """
module M

let f : string -> string =
    (fun x -> x.PadLeft(
        3
    ))
"""

    Assert.Equal(after, refactored before "_.")

[<Fact>]
let ``Nested shorthand converts on its own`` () =
    Assert.Equal(inModule "_.M(fun x -> x.P)", refactored (inModule "_.M(_.P)") "_.P")

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

[<Fact>]
let ``Lambda spanning multiple lines still converts to shorthand`` () =
    let before =
        """
module M

let r =
    fun x ->
        x.P
"""

    let after =
        """
module M

let r =
    _.P
"""

    Assert.Equal(after, refactored before "fun")

[<Fact>]
let ``Converting to shorthand and back restores the lambda`` () =
    let original = inModule "List.map (fun x -> x.P) xs"
    let shorthand = refactored original "fun"
    Assert.Equal(original, refactored shorthand "_.")
