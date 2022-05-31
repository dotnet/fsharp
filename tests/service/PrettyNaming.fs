module FSharp.Compiler.Service.Tests.PrettyNaming

open FSharp.Compiler.Syntax.PrettyNaming
open FsUnit
open NUnit.Framework

[<Test>]
let ``words in operator name should not be considered as mangled infix operator`` () =
    IsMangledInfixOperator "$foo hoo"
    |> should equal false

    IsMangledInfixOperator "@foo hoo"
    |> should equal false

[<Test>]
let ``typo in mangled operator name should not be considered as mangled infix operator`` () =
    IsMangledInfixOperator "op_Nagation"
    |> should equal false

[<Test>]
let ``valid mangled operator name should be considered as mangled infix operator`` () =
    IsMangledInfixOperator "op_Addition"
    |> should equal true
    
    IsMangledInfixOperator "op_Append"
    |> should equal true

[<Test>]
let ``invalid mangled operator name should not be considered as mangled infix operator`` () =
    IsMangledInfixOperator "op_Dollarfoo"
    |> should equal false
    
    IsMangledInfixOperator "foo"
    |> should equal false

[<Test>]
let ``symbols in mangled operator name should not be considered as mangled infix operator`` () =
    IsMangledInfixOperator "$foo"
    |> should equal false
    
    IsMangledInfixOperator "$"
    |> should equal false

    IsMangledInfixOperator "+"
    |> should equal false