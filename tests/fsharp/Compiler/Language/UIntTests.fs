namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module UIntTests =
    let ``uint type abbreviation works`` () =
        let src = "let x = uint 12"
        let expectedErrors = [||]
        CompilerAssert.TypeCheckWithErrors src expectedErrors