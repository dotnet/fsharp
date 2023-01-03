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
let ``A sequence expression can fail later in try/with and still get caught``() =
    Fsx """
let sum =
    seq {
        try
            yield 1   
            yield (10 /  0)
        with _ -> ()
    }
    |> Seq.sum
if sum <> (1) then
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
                yield (10/x)  // Will crash for 0     
                yield 2  // Will only get there for 1
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
let ``Propper type matching in seq{try/with} with implicit yield``(valInTry,valInWith1,valInWith2) =
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
[<InlineData("41","42","43")>]
[<InlineData("yield 41","yield 42","yield 43")>]
let ``seq{try/with} using yield or implicit must be consistent``(valInTry,valInWith1,valInWith2) =
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
[<InlineData("yield 41","42","43")>]
[<InlineData("yield 41","42","yield 43")>]
let ``seq{try/with} mismatch implicit vs. yield``(valInTry,valInWith1,valInWith2) =
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
    |> withErrorCode 3221
    |> withDiagnosticMessageMatches "This expression returns a value of type 'int' but is implicitly discarded."
    |> withDiagnosticMessageMatches "If you intended to use the expression as a value in the sequence then use an explicit 'yield'."

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
 