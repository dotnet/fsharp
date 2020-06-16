namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities

[<TestFixture>]
module UIntTests =
    let ``uint type abbreviation works`` () =
        let src = "let x = uint 12"
        let expectedErrors = [||]
        CompilerAssert.TypeCheckWithErrors src expectedErrors
