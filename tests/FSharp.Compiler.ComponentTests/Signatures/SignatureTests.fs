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

[<Fact>]
let ``2 dimensional array`` () =
    FSharp "let a : int[,] = failwith \"todo\""
    |> signaturesShouldContain "val a: int array2d"

[<Fact>]
let ``3 dimensional array`` () =
    FSharp "let a : int[,,] = failwith \"todo\""
    |> signaturesShouldContain "val a: int array3d"

[<Fact>]
let ``4 dimensional array`` () =
    FSharp "let a : int[,,,] = failwith \"todo\""
    |> signaturesShouldContain "val a: int array4d"

[<Fact>]
let ``5 till 32 dimensional array`` () =
    [ 5 .. 32 ]
    |> List.iter (fun idx ->
        let arrayType =
            [ 1 .. idx ]
            |> List.fold (fun acc _ -> $"array<{acc}>") "int"

        FSharp $"let a : {arrayType} = failwith \"todo\""
        |> signaturesShouldContain $"val a: int array{idx}d"
    )

[<Fact>]
let ``Use array2d syntax in implementation`` () =
    FSharp "let y : int array2d = Array2D.init 0 0 (fun _ _ -> 0)"
    |> signaturesShouldContain "val y: int array2d"