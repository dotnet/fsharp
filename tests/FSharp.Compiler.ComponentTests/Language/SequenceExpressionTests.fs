// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ComponentTests.Language.SequenceExpressionTests

open Xunit
open FSharp.Test.Compiler


[<Fact>]
let ``A sequence expression can yield from with clause``() =
    Fsx """
let sum =
    seq {
        for x in [0;1] do       
            try
                yield (10 /  x)
            with _ ->
                yield 100
    }
    |> Seq.sum
if sum <> 110 then
    failwith $"Sum was {sum} instead"
    """
    |> asExe
    |> compileAndRun
    |> shouldSucceed

[<Fact>]
let ``A sequence expression can yield! from with clause``() =
    Fsx """
let sum =
    seq {
        for x in [0;1] do       
            try
                yield (10 /  x)
            with _ ->
                yield! seq{1;2;3}
    }
    |> Seq.sum
if sum <> 16 then
    failwith $"Sum was {sum} instead"
    """
    |> asExe
    |> compileAndRun
    |> shouldSucceed

[<Fact>]
let ``A sequence expression can do multiple yields from try/with clause``() =
    Fsx """
let sum =
    seq {
        for x in [0;1] do       
            try
                yield 1   // Should work both times before failure
                yield! seq{ (10 /  x);2}  // Will crash for 0        
            with _ ->
                yield 100
                yield 100
    }
    |> Seq.sum
if sum <> (1+100+100+1+10+2) then
    failwith $"Sum was {sum} instead"
    """
    |> asExe
    |> compileAndRun
    |> shouldSucceed

[<Fact>]
let ``A sequence expression can yield from try and have empty with``() =
    Fsx """
let sum =
    seq {
        for x in [1;0] do       
            try
                yield (10 /  x)
            with _ ->
                printfn "Crash"
    }
    |> Seq.sum
if sum <> 10 then
    failwith $"Sum was {sum} instead"
    """
    |> asExe
    |> compileAndRun
    |> shouldSucceed

[<Fact>]
let ``A sequence expression can yield from with and have empty try``() =
    Fsx """
let sum =
    seq {
        for x in [1;0] do       
            try
                let result = (10 /  x)
                printfn "%A" result
            with _ ->
                yield 100
    }
    |> Seq.sum
if sum <> 100 then
    failwith $"Sum was {sum} instead"
    """
    |> asExe
    |> compileAndRun
    |> shouldSucceed


[<Fact>]
let ``A sequence expression can have implicit yields in try-with``() =
    Fsx """
let sum =
    seq {
        for x in [0;1] do       
            try
                (10 /  x)
            with _ ->
                100
    }
    |> Seq.sum
if sum <> 110 then
    failwith $"Sum was {sum} instead"
    """
    |> asExe
    |> compileAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData("41","42","43")>]
[<InlineData("()","42","43")>]
[<InlineData("41","()","43")>]
[<InlineData("41","42","()")>]
let ``Propper type matching in seq{try/with}``(valInTry,valInWith1,valInWith2) =
    Fsx $"""
let typedSeq =
    seq {{
        for x in [0;1] do       
            try
                %s{valInTry}
            with
            |_ when x = 0 -> %s{valInWith1}
            |_ when x = 0 -> %s{valInWith2}
    }}
    """
    |> typecheck
    |> shouldSucceed

[<Theory>]
[<InlineData("42","false","false")>]
[<InlineData("42","43","false")>]
[<InlineData("42","false","43")>]
let ``Type mismatch error in seq{try/with}``(valInTry,valInWith1,valInWith2) =
    Fsx $"""
let typedSeq =
    seq {{
        for x in [0;1] do       
            try
                %s{valInTry}
            with
            |_ when x = 0 -> %s{valInWith1}
            |_ when x = 0 -> %s{valInWith2}
    }}
    """
    |> typecheck
    |> shouldFail
    |> withErrorCode 193
    |> withDiagnosticMessageMatches "Type constraint mismatch"


[<Literal>]
let printCode = """ printfn "Hello there" """

[<Theory>]
[<InlineData("()","()","()")>]
[<InlineData(printCode,printCode,printCode)>]
let ``Missing result type in seq{try/with}``(valInTry,valInWith1,valInWith2) =
    Fsx $"""
let typedSeq =
    seq {{
        for x in [0;1] do       
            try
                %s{valInTry}
            with
            |_ when x = 0 -> %s{valInWith1}
            |_ when x = 0 -> %s{valInWith2}
    }}
    """
    |> typecheck
    |> shouldFail
    |> withErrorCode 30
    |> withDiagnosticMessageMatches "Value restriction. The value 'typedSeq' has been inferred to have generic type"
 