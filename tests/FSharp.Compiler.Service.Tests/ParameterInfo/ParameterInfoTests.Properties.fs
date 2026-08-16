module FSharp.Compiler.Service.Tests.ParameterInfoPropertiesTests

open Xunit

[<Fact>]
let ``Regression.MethodInfo.WithColon.Bug4518_1`` () =
    assertFirstReturnTypeText ": int" """
type T() =
    member this.X
        with set ((a:int), (b:int)) (c:int) = ()
((new T()).X({caret}"""

[<Fact>]
let ``Single.Locations.AfterProperties`` () =
    assertNoParameterInfo "System.DateTime.Today{caret}"

let private propertyGetterSetterSource = """
type Widget(z) =
    member x.P1
        with get() = System.Int32.Parse("")
        and set(z) = System.Int32.Parse("") |> ignore
    member x.P2 with get() = System.Int32.Parse("")
    member x.P2 with set(z) = System.Int32.Parse("") |> ignore"""

[<Fact>]
let ``LocationOfParams.InsidePropertyGettersAndSetters.Case1`` () =
    assertHasParameterInfo (markAtEndOfMarker propertyGetterSetterSource "with get() = System.Int32.Pa")

[<Fact>]
let ``LocationOfParams.InsidePropertyGettersAndSetters.Case2`` () =
    assertHasParameterInfo (markAtEndOfMarker propertyGetterSetterSource "and set(z) = System.Int32.Pa")

[<Fact>]
let ``LocationOfParams.InsidePropertyGettersAndSetters.Case3`` () =
    assertHasParameterInfo (markAtEndOfMarker propertyGetterSetterSource "P2 with get() = System.Int32.Pa")

[<Fact>]
let ``LocationOfParams.InsidePropertyGettersAndSetters.Case4`` () =
    assertHasParameterInfo (markAtEndOfMarker propertyGetterSetterSource "P2 with set(z) = System.Int32.Pa")

[<Fact>]
let ``Multi.NoParameterInfo.OnProperty`` () =
    assertNoParameterInfo """
let s = "Hello"
let _ = s.Length{caret}"""
