#if INTERACTIVE
#r @"../test/ComboProvider.dll"
#endif

module ComboProvider.Tests

open ComboProvider.Provided
open Xunit

[<Fact>]
let ``Default constructor should create instance`` () =
    Assert.Equal("My internal state", MyType().InnerState)

[<Fact>]
let ``Constructor with parameter should create instance`` () =
    Assert.Equal("override", MyType("override").InnerState)

[<Fact>]
let ``Static method returns an object of right type`` () =
    Assert.Equal("SomeRuntimeHelper", MyType.StaticMethod().GetType().Name)

[<Fact>]
let ``StaticMethod2 returns a null`` () =
    Assert.True(MyType.StaticMethod2() = null)

let ``MyType supports null`` () =
    Assert.True(MyType("test it") <> null)
                