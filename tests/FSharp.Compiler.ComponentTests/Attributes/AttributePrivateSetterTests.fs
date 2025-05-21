module FSharp.Compiler.ComponentTests.AttributePrivateSetter

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Compiler disallows setting a value of a private property on attribute``() =
    // Arrange
    let mainCode = """
module AttributeTest

open System.Diagnostics

[<Conditional("DEBUG", ConditionString = "RELEASE")>]
type TestClass() = class end
"""
    // Act & Assert
    FSharp mainCode
    |> compile
    |> shouldFail
    |> withDiagnostics [
        (Error, 3248, Line 5, Col 13, Line 5, Col 41, "Property 'ConditionString' on attribute cannot be set because the setter is private")
    ]
    |> ignore