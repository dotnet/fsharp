module FSharp.Compiler.Service.Tests.ParameterInfoTypeProvidersTests

open Xunit

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.StaticMethodWithOneParam`` () =
    assertParameterInfoOverloads [["arg1"]] "let foo = N1.T1.M1({caret}"

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.StaticMethodWithMoreParam`` () =
    assertParameterInfoOverloads [["arg1"; "arg2"]] "let foo = N1.T1.M2({caret}"

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.StaticMethodColonContent`` () =
    assertFirstReturnTypeText ": int" "let foo = N1.T1.M2({caret}"

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.ConstructorWithNoParam`` () =
    assertParameterInfoOverloadIndex 0 [] "let foo = new N1.T1({caret}"

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.ConstructorWithOneParam`` () =
    assertParameterInfoOverloadIndex 1 ["arg1"] "let foo = new N1.T1({caret}"

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.ConstructorWithMoreParam`` () =
    assertParameterInfoOverloadIndex 2 ["arg1"; "arg2"] "let foo = new N1.T1({caret}"

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.Type.WhenOpeningBracket`` () =
    assertParameterInfoOverloads [["Param1"; "ParamIgnored"]] "type foo = N1.T<{caret}"

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.Type.AfterCloseBracket`` () =
    assertNoParameterInfo "type foo = N1.T< \"Hello\", 2>{caret}"

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.Type.AfterDelimiter`` () =
    assertParameterInfoContains ["Param1"; "ParamIgnored"] "type foo = N1.T<\"Hello\",{caret}"

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``TypeProvider.Type.ParameterInfoLocation.WithNamespace`` () =
    assertHasParameterInfo "type boo = N1.T<{caret}"

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``TypeProvider.Type.ParameterInfoLocation.WithOutNamespace`` () =
    assertHasParameterInfo "open N1 \ntype boo = T<{caret}"

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``TypeProvider.Type.Negative.InString`` () =
    assertNoParameterInfo "type boo = \"N1.T<{caret}\""

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``TypeProvider.Type.Negative.InComment`` () =
    assertNoParameterInfo "// type boo = N1.T<{caret}"

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``LocationOfParams.TypeProviders.Basic`` () =
    assertHasParameterInfo """
            type U = N1.T< "fo{caret}o", 42 >"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``LocationOfParams.TypeProviders.BasicNamed`` () =
    assertHasParameterInfo """
            type U = N1.T< "fo{caret}o", ParamIgnored=42 >"""

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``LocationOfParams.TypeProviders.Prefix0`` () =
    assertHasParameterInfo """
            type U = N1.T< {caret} """

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``LocationOfParams.TypeProviders.Prefix1`` () =
    assertHasParameterInfo """
            type U = N1.T< "fo{caret}o", 42 """

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``LocationOfParams.TypeProviders.Prefix1Named`` () =
    assertHasParameterInfo """
            type U = N1.T< "fo{caret}o", ParamIgnored=42 """

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``LocationOfParams.TypeProviders.Prefix2`` () =
    assertHasParameterInfo """
            type U = N1.T< "fo{caret}o", """

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``LocationOfParams.TypeProviders.Prefix2Named1`` () =
    assertHasParameterInfo """
            type U = N1.T< "fo{caret}o", ParamIgnored= """

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``LocationOfParams.TypeProviders.Prefix2Named2`` () =
    assertHasParameterInfo """
            type U = N1.T< "fo{caret}o", ParamIgnored """

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``LocationOfParams.TypeProviders.Negative1`` () =
    assertNoParameterInfo """
                type D = System.Collections.Generic.Dictionary< in{caret}t, int >"""

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``LocationOfParams.TypeProviders.Negative2`` () =
    assertNoParameterInfo """
                type D = System.Collections.Generic.List< in{caret}t >"""

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``LocationOfParams.TypeProviders.Negative3`` () =
    assertNoParameterInfo """
                let i = 42
                let b = i< 4{caret}2"""

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``LocationOfParams.TypeProviders.Negative4.Bug181000`` () =
    assertNoParameterInfo """
                type U = N1.T< "foo", 42 >{caret}  """

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``LocationOfParams.TypeProviders.BasicWithinExpr`` () =
    assertNoParameterInfo """
                let f() =
                    let r = id( N1.T< "fo{caret}o", ParamIgnored=42 > )
                    r    """

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``LocationOfParams.TypeProviders.BasicWithinExpr.DoesNotInterfereWithOuterFunction`` () =
    assertHasParameterInfo """
            let f() =
                let r = id( N1.{caret}T< "foo", ParamIgnored=42 > )
                r    """

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``LocationOfParams.TypeProviders.Bug199744.ExcessCommasShouldNotAssertAndShouldGiveInfo.Case1`` () =
    assertHasParameterInfo """
            type U = N1.T< "fo{caret}o", 42, , >"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``LocationOfParams.TypeProviders.Bug199744.ExcessCommasShouldNotAssertAndShouldGiveInfo.Case2`` () =
    assertHasParameterInfo """
            type U = N1.T< "fo{caret}o", , >"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``LocationOfParams.TypeProviders.Bug199744.ExcessCommasShouldNotAssertAndShouldGiveInfo.Case3`` () =
    assertHasParameterInfo """
            type U = N1.T< ,{caret} >"""

[<Fact(Skip = "Re-enable this test --- https://github.com/dotnet/fsharp/issues/5238")>]
let ``LocationOfParams.TypeProviders.StaticParametersAtConstructorCallSite`` () =
    assertHasParameterInfo """
            let x = new N1.T< "fo{caret}o", 42 >()"""

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.FormatOfNamesOfSystemTypes`` () =
    assertParameterInfoOverloads [["Param1: string"; "ParamIgnored: int"]] """type TTT = N1.T< "fo{caret}o", ParamIgnored=42 > """
