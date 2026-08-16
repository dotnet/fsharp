module FSharp.Compiler.Service.Tests.ParameterInfoLetBindingsTests

open Xunit

[<Fact(Skip = "non-FCS: GetMethods has no string/comment lexical context; suppression inside strings is editor-layer")>]
let ``Single.InString`` () =
    assertNoParameterInfo """let s = "System.Console.WriteLine({caret})" """

[<Fact>]
let ``Multi.NoParameterInfo.WithinString`` () =
    assertNoParameterInfo """let s = "new System.DateTime(2000,12{caret}" """
