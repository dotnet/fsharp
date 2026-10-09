module FSharp.Compiler.Service.Tests.ParameterInfoModulesTests

open Xunit

[<Fact>]
let ``Regression.MethodSortedByArgumentCount.Bug4495.Case1`` () =
    assertParameterInfoOverloadIndex 0 ["System.Type array"] """
module ParameterInfo

let a1 = System.Reflection.Assembly.Load("mscorlib")
let m = a1.GetType("System.Decimal").GetConstructor({caret}null)"""

[<Fact>]
let ``Regression.MethodSortedByArgumentCount.Bug4495.Case2`` () =
    assertParameterInfoContains
        [ "System.Reflection.BindingFlags"
          "System.Reflection.Binder"
          "System.Type array"
          "System.Reflection.ParameterModifier array" ] """
module ParameterInfo

let a1 = System.Reflection.Assembly.Load("mscorlib")
let m = a1.GetType("System.Decimal").GetConstructor({caret}null)"""
