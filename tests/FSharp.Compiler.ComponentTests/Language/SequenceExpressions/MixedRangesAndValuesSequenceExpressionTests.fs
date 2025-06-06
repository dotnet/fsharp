// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open FSharp.Test
open Xunit
open FSharp.Test.Compiler

module  MixedRangesAndValuesSequenceExpressionTests =
    
    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compileAndRun

    [<Fact>]
    let ``Version 90: Mixed ranges and values require preview language version``() =
        FSharp """
module MixedRangeVersionTest

let test = [1; 2..5; 10]
        """
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 4, Col 12, Line 4, Col 25, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 4, Col 16, Line 4, Col 20, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    [<Fact>]
    let ``Preview: Mixed ranges and values require preview language version``() =
        FSharp """
module MixedRangeVersionTest

let test = [1; 2..5; 10]
        """
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldSucceed
        
    [<Fact>]
    let ``Version 90: Mixed ranges require preview language version``() =
        FSharp """
module MixedRangeVersionTest

let test = [ 2..5; 10..20 ]
        """
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 4, Col 12, Line 4, Col 28, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 4, Col 14, Line 4, Col 18, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 4, Col 20, Line 4, Col 26, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    [<Fact>]
    let ``Preview: Mixed ranges require preview language version``() =
        FSharp """
module MixedRangeVersionTest

let test = [ 2..5; 10..20 ]
        """
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldSucceed
    
    [<Fact>]
    let ``Mixed ranges and values in lists``() =
        FSharp """
module MixedRangeListTests

let test1 = [-3; 1..10]
let expected1 = [-3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10]
if test1 <> expected1 then failwith $"test1 failed: got {test1}"

let test2 = [-3; 1..10; 19]
let expected2 = [-3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 19]
if test2 <> expected2 then failwith $"test2 failed: got {test2}"

let test3 = [1..3; 5..7; 10]
let expected3 = [1; 2; 3; 5; 6; 7; 10]
if test3 <> expected3 then failwith $"test3 failed: got {test3}"

let test4 = [0; 2..2..10; 15]
let expected4 = [0; 2; 4; 6; 8; 10; 15]
if test4 <> expected4 then failwith $"test4 failed: got {test4}"

printfn "All list tests passed!"
        """
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Mixed ranges and values in arrays``() =
        FSharp """
module MixedRangeArrayTests

let test1 = [|-3; 1..10|]
let expected1 = [|-3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10|]
if test1 <> expected1 then failwith $"test1 failed: got {test1}"

let test2 = [|-3; 1..10; 19|]
let expected2 = [|-3; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 19|]
if test2 <> expected2 then failwith $"test2 failed: got {test2}"

let test3 = [|1..3; 5..7; 10|]
let expected3 = [|1; 2; 3; 5; 6; 7; 10|]
if test3 <> expected3 then failwith $"test3 failed: got {test3}"

let test4 = [|0; 2..2..10; 15|]
let expected4 = [|0; 2; 4; 6; 8; 10; 15|]
if test4 <> expected4 then failwith $"test4 failed: got {test4}"

let test5 = [|1; 5..4; 10|]
let expected5 = [|1; 10|]
if test5 <> expected5 then failwith $"test5 failed: got {test5}"

let test6 = [|0; 5..5; 10|]
let expected6 = [|0; 5; 10|]
if test6 <> expected6 then failwith $"test6 failed: got {test6}"

printfn "All array tests passed!"
        """
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Mixed ranges and values in sequences``() =
        FSharp """
module MixedRangeSeqTests

let test1 = seq { 1..10; 19 }
let expected1 = [1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 19]
if List.ofSeq test1 <> expected1 then failwith $"test1 failed"

let test2 = seq { -3; 1..5; 10..12 }
let expected2 = [-3; 1; 2; 3; 4; 5; 10; 11; 12]
if List.ofSeq test2 <> expected2 then failwith $"test2 failed"

let test3 = seq { 0; 2..2..10; 15 }
let expected3 = [0; 2; 4; 6; 8; 10; 15]
if List.ofSeq test3 <> expected3 then failwith $"test3 failed"

let test4 = seq { 
    yield 1
    yield! [2..5]
    yield 10
    yield! [15..2..20]
}
let expected4 = [1; 2; 3; 4; 5; 10; 15; 17; 19]
if List.ofSeq test4 <> expected4 then failwith $"test4 failed"

printfn "All sequence tests passed!"
        """
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Type inference with mixed ranges and values``() =
        FSharp """
module TypeInferenceTests

let test1: int list = [1..5; 10]
let test2: int array = [|1..5; 10|]
let test3: seq<int> = seq { 1..5; 10 }

let inline mixedRange start finish extra =
    [start..finish; extra]

let test4 = mixedRange 1 5 10
let test5 = mixedRange 1.0 5.0 10.0
let test6 = mixedRange 'a' 'e' 'z'

if test4 <> [1; 2; 3; 4; 5; 10] then failwith "int range failed"
if test5 <> [1.0; 2.0; 3.0; 4.0; 5.0; 10.0] then failwith "float range failed"
if test6 <> ['a'; 'b'; 'c'; 'd'; 'e'; 'z'] then failwith "char range failed"

printfn "Type inference tests passed!"
        """
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Complex expressions with mixed ranges and values``() =
        FSharp """
module ComplexExpressionTests

let x = 5
let test1 = [1; x..x+5; 20]
let expected1 = [1; 5; 6; 7; 8; 9; 10; 20]
if test1 <> expected1 then failwith $"test1 failed: got {test1}"

// Conditional with ranges
let includeRange = true
let test2 = if includeRange then [1; 2..5; 10] else [1; 10]
let expected2 = [1; 2; 3; 4; 5; 10]
if test2 <> expected2 then failwith $"test2 failed: got {test2}"

let getStart() = 1
let getEnd() = 5
let test3 = [0; getStart()..getEnd(); 10]
let expected3 = [0; 1; 2; 3; 4; 5; 10]
if test3 <> expected3 then failwith $"test3 failed: got {test3}"

let test4 = [
    yield 1
    yield! [2..4]
    yield 5
    yield! [6; 7..9; 10]
]
let expected4 = [1; 2; 3; 4; 5; 6; 7; 8; 9; 10]
if test4 <> expected4 then failwith $"test4 failed: got {test4}"

printfn "Complex expression tests passed!"
        """
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Edge cases for mixed ranges and values``() =
        FSharp """
module EdgeCaseTests

let test1 = [1..3; 5..7; 9..10]
let expected1 = [1; 2; 3; 5; 6; 7; 9; 10]
if test1 <> expected1 then failwith $"test1 failed: got {test1}"

let test2 = [1..5; 3..7]
let expected2 = [1; 2; 3; 4; 5; 3; 4; 5; 6; 7]
if test2 <> expected2 then failwith $"test2 failed: got {test2}"

let test3 = [20; 10..-1..5; 0]
let expected3 = [20; 10; 9; 8; 7; 6; 5; 0]
if test3 <> expected3 then failwith $"test3 failed: got {test3}"

let test4 = [0; 1..1000; 2000]
if test4.Length <> 1002 then failwith $"test4 failed: expected length 1002, got {test4.Length}"
if test4.[0] <> 0 then failwith "test4: first element should be 0"
if test4.[1001] <> 2000 then failwith "test4: last element should be 2000"

// Mixed types should fail compilation
// This is a compile-time test, not runtime
// let shouldFail = [1; 2.0..5.0] // This should not compile

printfn "Edge case tests passed!"
        """
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed