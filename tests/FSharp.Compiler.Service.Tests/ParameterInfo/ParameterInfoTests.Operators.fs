module FSharp.Compiler.Service.Tests.ParameterInfoOperatorsTests

open Xunit

[<Fact(Skip = "FCS GetMethods returns the (>) operator group; the negative case was editor-layer only")>]
let ``Single.Negative.OperatorTrick1`` () =
    assertNoParameterInfo "let fooo = 0\n             >({caret} 1 )"

[<Fact(Skip = "FCS GetMethods returns the (<) operator group; the negative case was editor-layer only")>]
let ``Single.Negative.OperatorTrick2`` () =
    assertNoParameterInfo "let fooo = 0\n             <({caret} 1 )"

[<Fact>]
let ``LocationOfParams.InfixOperators.Case1`` () =
    assertHasParameterInfo """System.Console.Write{caret}Line("" + "")"""

[<Fact>]
let ``LocationOfParams.InfixOperators.Case2`` () =
    assertHasParameterInfo """System.Console.Write{caret}Line((+)(3)(4))"""

[<Fact>]
let ``Regression.ParameterWithOperators.Bug90832`` () =
    assertParameterInfoContains ["value: string"] """System.Console.Write{caret}Line("This is a" + " bug.")"""
