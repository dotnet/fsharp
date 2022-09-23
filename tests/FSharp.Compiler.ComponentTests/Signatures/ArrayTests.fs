module FSharp.Compiler.ComponentTests.Signatures.SignatureTests

open Xunit
open FSharp.Compiler.ComponentTests.Signatures.TestHelpers

[<Theory>]
[<InlineData("let a : int[] = [| 1 |]",
             "val a: int array")>]
[<InlineData("let b: int array = [| 2 |]",
             "val b: int array")>]
[<InlineData("let c: array<int> = [| 3 |]",
             "val c: int array")>]
let ``Value with int array return type`` implementation expectedSignature =
    assertSingleSignatureBinding implementation expectedSignature

[<Fact>]
let ``2 dimensional array`` () =
    assertSingleSignatureBinding
        "let a : int[,] = failwith \"todo\""
        "val a: int array2d"

[<Fact>]
let ``3 dimensional array`` () =
    assertSingleSignatureBinding
        "let a : int[,,] = failwith \"todo\""
        "val a: int array3d"

[<Fact>]
let ``4 dimensional array`` () =
    assertSingleSignatureBinding
        "let a : int[,,,] = failwith \"todo\""
        "val a: int array4d"

[<Fact>]
let ``5 till 32 dimensional array`` () =
    [ 5 .. 32 ]
    |> List.iter (fun idx ->
        let arrayType =
            [ 1 .. idx ]
            |> List.fold (fun acc _ -> $"array<{acc}>") "int"

        assertSingleSignatureBinding
            $"let a : {arrayType} = failwith \"todo\""
            $"val a: int array{idx}d"
    )

[<Fact>]
let ``Use array2d syntax in implementation`` () =
    assertSingleSignatureBinding
        "let y : int array2d = Array2D.init 0 0 (fun _ _ -> 0)"
        "val y: int array2d"
