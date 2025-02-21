module Signatures.SeqTests

open Xunit
open Signatures.TestHelpers
open FSharp.Test
open FSharp.Test.Compiler

[<Fact>]
let ``int seq`` () =
    assertSingleSignatureBinding
        "let s = seq { yield 1 }"
        "val s: int seq"

[<Fact>]
let ``tuple seq`` () =
    assertSingleSignatureBinding
        "let s = seq { yield (1, 'b', 2.) }"
        "val s: (int * char * float) seq"

[<Fact>]
let ``seq transpose`` () =
    let encodeFs =
        FsSource """module Program
let transpose (source: seq<#seq<'T>>) =
    source |> Seq.collect Seq.indexed |> Seq.groupBy fst |> Seq.map (snd >> (Seq.map snd))"""

    Fsi """module Program
val transpose: source: seq<'Collection> -> seq<seq<'T>> when 'Collection :> seq<'T>""" 
    |> withAdditionalSourceFile encodeFs
    |> withLangVersionPreview
    |> compile
    |> shouldSucceed
