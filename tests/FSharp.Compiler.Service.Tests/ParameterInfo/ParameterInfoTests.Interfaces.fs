module FSharp.Compiler.Service.Tests.ParameterInfoInterfacesTests

open Xunit

[<Fact>]
let ``Regression.MethodInfo.WithColon.Bug4518_2`` () =
    assertFirstReturnTypeText ": int" """
type IFoo = interface
     abstract f : int -> int
         end
let i : IFoo = null
i.f({caret}"""
