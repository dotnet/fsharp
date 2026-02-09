module Language.NullnessInternalsTests

open Xunit
open FSharp.Compiler.TypedTree

[<Fact>]
let ``NullnessVar IsFullySolved with Known WithoutNull`` () =
    let nv = NullnessVar()
    nv.Set(Nullness.Known NullnessInfo.WithoutNull)
    Assert.True(nv.IsFullySolved)

[<Fact>]
let ``NullnessVar Set stores Known WithoutNull`` () =
    let nv = NullnessVar()
    nv.Set(Nullness.Known NullnessInfo.WithoutNull)
    Assert.Equal(NullnessInfo.WithoutNull, nv.Evaluate())
    match nv.Solution with
    | Nullness.Known NullnessInfo.WithoutNull -> ()
    | other -> Assert.Fail($"Unexpected solution: %A{other}")

[<Fact>]
let ``Chained NullnessVar resolution`` () =
    let inner = NullnessVar()
    inner.Set(Nullness.Known NullnessInfo.WithoutNull)
    let outer = NullnessVar()
    outer.Set(Nullness.Variable inner)
    Assert.True(outer.IsFullySolved)
    Assert.Equal(NullnessInfo.WithoutNull, outer.Evaluate())
