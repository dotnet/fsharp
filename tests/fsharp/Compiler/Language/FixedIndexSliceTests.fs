namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module FixedIndexSliceTests =
    
    [<Test>]
    let ``Fixed index 3d slicing should not be available in 47``() =
        CompilerAssert.TypeCheckWithErrorsAndOptions [| "--langversion:4.7"|]
            """
open System

let arr3 = Array3D.create 2 2 2 2
arr3.[1, *, *]
arr3.[*, 1, *]
arr3.[*, *, 1]
arr3.[1, 1, *]
arr3.[*, 1, *]
arr3.[*, 1, 1]
            """
            [|
                FSharpErrorSeverity.Error, 39, (5,1,5,15), "The type '[,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (6,1,6,15), "The type '[,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (7,1,7,15), "The type '[,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (8,1,8,15), "The type '[,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (9,1,9,15), "The type '[,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (10,1,10,15), "The type '[,,]<T>' does not define the field, constructor or member 'GetSlice'."
            |]

    [<Test>]
    let ``Fixed index 4d slicing should not be available in 47``() =
        CompilerAssert.TypeCheckWithErrorsAndOptions [| "--langversion:4.7"|]
            """
open System

let arr4 = Array4D.create 2 2 2 2 2
arr4.[1, *, *, *]
arr4.[*, 1, *, *]
arr4.[*, *, 1, *]
arr4.[*, *, *, 1]
arr4.[1, 1, *, *]
arr4.[1, *, 1, *]
arr4.[1, *, *, 1]
arr4.[*, 1, 1, *]
arr4.[*, *, 1, 1]
arr4.[1, *, 1, 1]
arr4.[1, 1, *, 1]
arr4.[1, 1, 1, *]
arr4.[*, 1, 1, 1]
            """
            [|
                FSharpErrorSeverity.Error, 39, (5,1,5,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (6,1,6,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (7,1,7,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (8,1,8,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (9,1,9,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (10,1,10,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (11,1,11,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (12,1,12,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (13,1,13,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (14,1,14,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (15,1,15,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (16,1,16,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
                FSharpErrorSeverity.Error, 39, (17,1,17,18), "The type '[,,,]<T>' does not define the field, constructor or member 'GetSlice'."
            |]
            
    [<Test>]
    let ``Fixed index 3d set slicing should not be available in 47``() =
        CompilerAssert.TypeCheckWithErrorsAndOptions [| "--langversion:4.7"|]
            """
open System

let arr3 = Array3D.create 2 2 2 2
let arr2 = array2D [ [1;2]; [3;4] ]
let arr1 = [|1;2|]
arr3.[1, *, *] <- arr2
arr3.[*, 1, *] <- arr2
arr3.[*, *, 1] <- arr2
arr3.[1, 1, *] <- arr1
arr3.[*, 1, *] <- arr1
arr3.[*, 1, 1] <- arr1
            """
            [|
                FSharpErrorSeverity.Error, 39, (7,1,7,15), "The type '[,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (8,1,8,15), "The type '[,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (9,1,9,15), "The type '[,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (10,1,10,15), "The type '[,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (11,1,11,15), "The type '[,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (12,1,12,15), "The type '[,,]<T>' does not define the field, constructor or member 'SetSlice'."
            |]

    [<Test>]
    let ``Fixed index 4d set slicing should not be available in 47``() =
        CompilerAssert.TypeCheckWithErrorsAndOptions [| "--langversion:4.7"|]
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
            [|
                FSharpErrorSeverity.Error, 39, (8,1,8,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (9,1,9,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (10,1,10,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (11,1,11,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (12,1,12,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (13,1,13,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (14,1,14,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (15,1,15,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (16,1,16,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (17,1,17,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (18,1,18,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (19,1,19,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (20,1,20,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
                FSharpErrorSeverity.Error, 39, (21,1,21,18), "The type '[,,,]<T>' does not define the field, constructor or member 'SetSlice'."
            |]
