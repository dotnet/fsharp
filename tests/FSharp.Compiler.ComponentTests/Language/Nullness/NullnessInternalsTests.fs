module Language.NullnessInternalsTests

open Xunit
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics

[<Fact>]
let ``KnownFromConstructor evaluates to WithoutNull`` () =
    Assert.Equal(NullnessInfo.WithoutNull, Nullness.KnownFromConstructor.Evaluate())

[<Fact>]
let ``KnownFromConstructor TryEvaluate returns ValueSome WithoutNull`` () =
    Assert.Equal(ValueSome NullnessInfo.WithoutNull, Nullness.KnownFromConstructor.TryEvaluate())

[<Fact>]
let ``KnownFromConstructor IsFromConstructor is true`` () =
    Assert.True(Nullness.KnownFromConstructor.IsFromConstructor)

[<Fact>]
let ``Known WithoutNull IsFromConstructor is false`` () =
    Assert.False((Nullness.Known NullnessInfo.WithoutNull).IsFromConstructor)

[<Fact>]
let ``Known WithNull IsFromConstructor is false`` () =
    Assert.False((Nullness.Known NullnessInfo.WithNull).IsFromConstructor)

[<Fact>]
let ``Variable IsFromConstructor is false`` () =
    let nv = NullnessVar()
    Assert.False((Nullness.Variable nv).IsFromConstructor)

[<Fact>]
let ``NullnessVar has no isCtorResult parameter`` () =
    let nv = NullnessVar()
    Assert.NotNull(nv)

[<Fact>]
let ``KnownWithoutNullFromCtor singleton is KnownFromConstructor`` () =
    Assert.True(KnownWithoutNullFromCtor.IsFromConstructor)

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
    Assert.False(nv.Solution.IsFromConstructor)

[<Fact>]
let ``Chained NullnessVar resolution through KnownFromConstructor`` () =
    let inner = NullnessVar()
    inner.Set(Nullness.KnownFromConstructor)
    let outer = NullnessVar()
    outer.Set(Nullness.Variable inner)
    Assert.True(outer.IsFullySolved)
    Assert.Equal(NullnessInfo.WithoutNull, outer.Evaluate())
