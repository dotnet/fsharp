namespace FSharp.Compiler.ComponentTests

open Xunit
open FSharp.Test.Compiler

[<TestFixture>]
type AttributePrivateSetterTests() =

    [<Fact>]
    let ``Compiler disallows setting a value of a private property on attribute``() =
        // Arrange
        let code = """
using System;

namespace AttributeTest
{
    [AttributeUsage(AttributeTargets.All)]
    public class TestAttribute : Attribute
    {
        public TestAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
        
        public bool IsDefault { get; private set; }
    }
}
"""
        let mainCode = """
module AttributeTest

[<AttributeTest.Test("TestName", IsDefault = true)>]
type TestClass() = class end
"""
        // Act & Assert
        FSharp mainCode
        |> withAdditionalSources [(code, "cs")]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (DiagnosticLevel.Error, 3248, "Property 'IsDefault' on attribute cannot be set because the setter is private", (4, 21, 4, 43))
        ]
        |> ignore