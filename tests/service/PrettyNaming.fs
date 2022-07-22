module FSharp.Compiler.Service.Tests.PrettyNaming

open FSharp.Compiler.Syntax.PrettyNaming
open FsUnit
open NUnit.Framework

[<Test>]
let ``words in operator name should not be considered as mangled infix operator`` () =
    IsLogicalInfixOpName "$foo hoo"
    |> should equal false

    IsLogicalInfixOpName "@foo hoo"
    |> should equal false

[<Test>]
let ``typo in mangled operator name should not be considered as mangled infix operator`` () =
    IsLogicalInfixOpName "op_Nagation"
    |> should equal false

[<Test>]
let ``valid mangled operator name should be considered as mangled infix operator`` () =
    IsLogicalInfixOpName "op_Addition"
    |> should equal true
    
    IsLogicalInfixOpName "op_Append"
    |> should equal true

[<Test>]
let ``invalid mangled operator name should not be considered as mangled infix operator`` () =
    IsLogicalInfixOpName "op_Dollarfoo"
    |> should equal false
    
    IsLogicalInfixOpName "foo"
    |> should equal false

[<Test>]
let ``symbols in mangled operator name should not be considered as mangled infix operator`` () =
    IsLogicalInfixOpName "$foo"
    |> should equal false
    
    IsLogicalInfixOpName "$"
    |> should equal false

    IsLogicalInfixOpName "+"
    |> should equal false