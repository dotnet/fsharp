namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test
open FSharp.Compiler.Diagnostics


module FixedIndexSliceTests =

    // These tests verify that 3D/4D fixed-index slicing works correctly (feature added in F# 5.0)
    
    [<Fact>]
    let ``Fixed index 3d slicing works in langversion 8``() =
        CompilerAssert.PassWithOptions [| "--langversion:8.0"|]
            """
open System

let arr3 = Array3D.create 2 2 2 2
let _ = arr3.[1, *, *]
let _ = arr3.[*, 1, *]
let _ = arr3.[*, *, 1]
let _ = arr3.[1, 1, *]
let _ = arr3.[*, 1, *]
let _ = arr3.[*, 1, 1]
            """

    [<Fact>]
    let ``Fixed index 4d slicing works in langversion 8``() =
        CompilerAssert.PassWithOptions [| "--langversion:8.0"|]
            """
open System

let arr4 = Array4D.create 2 2 2 2 2
let _ = arr4.[1, *, *, *]
let _ = arr4.[*, 1, *, *]
let _ = arr4.[*, *, 1, *]
let _ = arr4.[*, *, *, 1]
let _ = arr4.[1, 1, *, *]
let _ = arr4.[1, *, 1, *]
let _ = arr4.[1, *, *, 1]
let _ = arr4.[*, 1, 1, *]
let _ = arr4.[*, *, 1, 1]
let _ = arr4.[1, *, 1, 1]
let _ = arr4.[1, 1, *, 1]
let _ = arr4.[1, 1, 1, *]
let _ = arr4.[*, 1, 1, 1]
            """
            
    [<Fact>]
    let ``Fixed index 3d set slicing works in langversion 8``() =
        CompilerAssert.PassWithOptions [| "--langversion:8.0"|]
            """
open System

let arr3 = Array3D.create 2 2 2 2
let arr2 = array2D [ [1;2]; [3;4] ]
let arr1 = [|1;2|]
arr3.[1, *, *] <- arr2
arr3.[*, 1, *] <- arr2
arr3.[*, *, 1] <- arr2
arr3.[1, 1, *] <- arr1
arr3.[1, *, 1] <- arr1
arr3.[*, 1, 1] <- arr1
            """

    [<Fact>]
    let ``Fixed index 4d set slicing works in langversion 8``() =
        CompilerAssert.PassWithOptions [| "--langversion:8.0"|]
            """
open System

let arr4 = Array4D.create 2 2 2 2 2
let arr3 = Array3D.create 2 2 2 2
let arr2 = array2D [ [1;2]; [3;4] ]
let arr1 = [|1;2|]
arr4.[1, *, *, *] <- arr3
arr4.[*, 1, *, *] <- arr3
arr4.[*, *, 1, *] <- arr3
arr4.[*, *, *, 1] <- arr3
arr4.[1, 1, *, *] <- arr2
arr4.[1, *, 1, *] <- arr2
arr4.[1, *, *, 1] <- arr2
arr4.[*, 1, *, 1] <- arr2
arr4.[*, 1, 1, *] <- arr2
arr4.[*, *, 1, 1] <- arr2
arr4.[1, *, 1, 1] <- arr1
arr4.[1, 1, *, 1] <- arr1
arr4.[1, 1, 1, *] <- arr1
arr4.[*, 1, 1, 1] <- arr1
            """
