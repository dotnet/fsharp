module Language.NullnessInternalsTests

open Xunit
open FSharp.Compiler.TypedTree

[<Fact>]
let ``KnownFromConstructor evaluates to WithoutNull`` () =
    Assert.Equal(NullnessInfo.WithoutNull, Nullness.KnownFromConstructor.Evaluate())

[<Fact>]
let ``KnownFromConstructor TryEvaluate returns ValueSome WithoutNull`` () =
    Assert.Equal(ValueSome NullnessInfo.WithoutNull, Nullness.KnownFromConstructor.TryEvaluate())

[<Fact>]
let ``KnownFromConstructor is distinct from Known WithoutNull`` () =
    match Nullness.KnownFromConstructor with
    | Nullness.Known _ -> Assert.Fail("KnownFromConstructor should be distinct from Known WithoutNull")
    | Nullness.KnownFromConstructor -> ()
    | _ -> Assert.Fail("Unexpected case")

[<Fact>]
let ``NullnessVar IsFullySolved with KnownFromConstructor`` () =
    let nv = NullnessVar()
    nv.Set(Nullness.KnownFromConstructor)
    Assert.True(nv.IsFullySolved)

[<Fact>]
let ``NullnessVar Set normalizes KnownFromConstructor to Known WithoutNull`` () =
    let nv = NullnessVar()
    nv.Set(Nullness.KnownFromConstructor)
    Assert.Equal(NullnessInfo.WithoutNull, nv.Evaluate())
    // After Set normalizes, Solution should not be KnownFromConstructor
    match nv.Solution with
    | Nullness.KnownFromConstructor -> Assert.Fail("Expected normalization away from KnownFromConstructor")
    | Nullness.Known NullnessInfo.WithoutNull -> ()
    | other -> Assert.Fail($"Unexpected solution: %A{other}")

[<Fact>]
let ``Chained NullnessVar resolution through KnownFromConstructor`` () =
    let inner = NullnessVar()
    inner.Set(Nullness.KnownFromConstructor)
    let outer = NullnessVar()
    outer.Set(Nullness.Variable inner)
    Assert.True(outer.IsFullySolved)
    Assert.Equal(NullnessInfo.WithoutNull, outer.Evaluate())
