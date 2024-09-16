namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test


module UIntTests =
    let ``uint type abbreviation works`` () =
        let src = "let x = uint 12"
        let expectedErrors = [||]
        CompilerAssert.TypeCheckWithErrors src expectedErrors
