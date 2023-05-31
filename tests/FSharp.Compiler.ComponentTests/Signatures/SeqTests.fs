module FSharp.Compiler.ComponentTests.Signatures.SeqTests

open Xunit
open FSharp.Compiler.ComponentTests.Signatures.TestHelpers

[<Fact>]
let ``int seq`` () =
    assertSingleSignatureBinding
        "let s = seq { yield 1 }"
        "val s: int seq"
