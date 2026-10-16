module FSharp.Compiler.Service.Tests.ParameterInfoFunctionsTests

open Xunit

[<Fact>]
let ``Regression.MethodInfo.WithColon.Bug4518_5`` () =
    assertFirstReturnTypeText ": (int -> int) " """
           let f x y = x + y
           f({caret}"""

[<Fact>]
let ``Single.BasicFSharpFunction`` () =
    assertParameterInfoOverloads [["x: 'a"]] """
            let foo(x) = 1
            foo({caret}"""

[<Fact(Skip = "non-FCS: paraminfo popup LOCATION is editor-layer (MethodListForAMethodTip.GetColumnOfStartOfLongId); GetMethods returns no group for the curried `sin 0.0` application.")>]
let ``Single.Locations.FunctionWithSpace`` () =
    assertHasParameterInfo "let a = sin 0{caret}.0"

[<Fact>]
let ``LocationOfParams.ThisOnceAssertedToo`` () =
    assertNoParameterInfo """
                let readString() =
                    let x = 42
                    while ('"' = '""' then
                            ()
                        else
                            let sb = new System.Text.StringBuilder()
                            while true do
                                ({caret})  """

