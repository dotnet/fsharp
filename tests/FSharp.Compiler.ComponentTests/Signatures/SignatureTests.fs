module FSharp.Compiler.ComponentTests.Signatures.SignatureTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Very basic signature test`` () =
    FSharp "let a = 0"
    |> signaturesShouldContain "val a: int"

[<Theory>]
[<InlineData("let a : int[] = [| 1 |]",
             "val a: int array")>]
[<InlineData("let b: int array = [| 2 |]",
             "val b: int array")>]
[<InlineData("let c: array<int> = [| 3 |]",
             "val c: int array")>]
let ``Value with int array return type`` implementation expectedSignature =
    FSharp implementation
    |> signaturesShouldContain expectedSignature