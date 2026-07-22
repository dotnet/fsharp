module FSharp.Compiler.Service.Tests.CheckerExtensionsTests

open Xunit
open FSharp.Test.Assert

[<Fact>]
let ``Extract ordered marked sources`` () =
    let markedSources = SourceContext.extractOrderedMarkedSources "let a{caret1}, b{caret2} = 1, 2"

    markedSources
    |> shouldBe [ "let a{caret}, b = 1, 2"
                  "let a, b{caret} = 1, 2" ]
