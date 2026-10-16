module FSharp.Compiler.Service.Tests.ParameterInfoAttributesTests

open Xunit

[<Fact(Skip = "FSharp1.0:5242")>]
let ``Single.OnAttributes`` () =
    assertParameterInfoOverloads [ []; [ "check: bool" ] ] """
type Emp =
    [<DefaultValue({caret}true)>]
    static val mutable private m_ID : int"""

[<Fact>]
let ``LocationOfParams.Attributes.Bug230393`` () =
    assertHasParameterInfo """
let paramTest((strA : string),(strB : string)) =
    strA + strB
param{caret}Test(

[<Measure>]
type RMB"""

[<Fact>]
let ``ParameterInfo.ArgumentsWithParamsArrayAttribute`` () =
    assertParameterInfoContains [ "format"; "[<System.ParamArray>] args" ] """let _ = System.String.Form{caret}at("",)"""

[<Fact(Skip = "93188 - No param info shown in the Attribute method")>]
let ``Regression.Multi.ExplicitAnnotate.Bug93188`` () =
    assertParameterInfoOverloads [ ["int"; "string"] ] """
type LiveAnimalAttribute(a : int, b: string) =
    inherit System.Attribute()

[<LiveAnimal(1,{caret}"Bat")>]
type Wombat() = class end"""
