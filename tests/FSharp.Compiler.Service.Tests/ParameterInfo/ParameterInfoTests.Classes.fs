module FSharp.Compiler.Service.Tests.ParameterInfoClassesTests

open Xunit

[<Fact>]
let ``Regression.OnConstructor.881644`` () =
    assertParameterInfoContains ["path: string"] "new System.IO.StreamReader({caret}"

[<Fact>]
let ``Regression.MethodInfo.WithColon.Bug4518_3`` () =
    assertFirstReturnTypeText ": unit" """
type M() =
    member this.f x = ()
let m = new M()
m.f({caret}"""

[<Fact>]
let ``Single.Constructor1`` () =
    assertHasParameterInfo "new System.DateTime({caret}"

[<Fact>]
let ``LocationOfParams.InsideAMemberOfAType`` () =
    assertHasParameterInfo """
type Widget(z) =
    member x.a = (1 <> System.Int32.Pa{caret}rse("")) """

[<Fact>]
let ``Multi.DotNet.StaticMethod.WithinClassMember`` () =
    assertParameterInfoContains ["string"; "System.Globalization.NumberStyles"] """
type Widget(z) =
    member x.a = (1 <> System.Int32.Pa{caret}rse("",

let widget = Widget(1)
45"""

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Multi.DotNet.Constructor`` () =
    assertParameterInfoContains ["int"; "int"; "int"] "let _ = new System.Date{caret}Time(2010,12,"

[<Fact>]
let ``Regression.OptionalArguments.Bug4042`` () =
    assertParameterInfoOverloads [ ["x: int"; "?y int"] ] """
module ParameterInfo
type TT(x : int, ?y : int) =
    let z = y
    do printfn "%A" z
    member this.Foo(?z : int) = z

type TT2(x : int, y : int option) =
    let z  = y
    do printfn "%A" z
let tt = TT({caret}"""
