namespace FSharp.Compiler.UnitTests

open NUnit.Framework
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
            """
            [|
                FSharpErrorSeverity.Error, 39, (5,1,5,15), "The field, constructor or member 'GetSlice' is not defined."
            |]

