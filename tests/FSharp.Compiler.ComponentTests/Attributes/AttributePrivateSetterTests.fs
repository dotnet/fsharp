namespace FSharp.Compiler.ComponentTests

open Xunit
open FSharp.Test.Compiler

[<TestFixture>]
type AttributePrivateSetterTests() =

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
            (DiagnosticLevel.Error, 3248, "Property 'ConditionString' on attribute cannot be set because the setter is private", (5, 13, 5, 41))
        ]
        |> ignore