module FSharp.Compiler.Service.Tests.ParameterInfoObjectExpressionsTests

open Xunit

[<Fact>]
let ``Multi.Constructor.WithinObjectExpression`` () =
    assertParameterInfoOverloads [[]] "let _ = { new System.Object({caret}) with member _.GetHashCode() = 2}"
