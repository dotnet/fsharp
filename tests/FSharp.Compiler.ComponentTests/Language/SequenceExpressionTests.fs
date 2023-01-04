// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ComponentTests.Language.SequenceExpressionTests

open Xunit
open FSharp.Test.Compiler



let fsiSession = getSessionForEval()

let runCode = evalInSharedSession fsiSession

[<Fact>]
let ``A seq{try/with} happy path with multiple language elements``() =
    Fsx """
let rec mySeq inputEnumerable =
    seq {
        for x in inputEnumerable do       
            try
                match x with                                     
                | 0 -> yield 1                                      // - Single value
                | 1 -> yield! (mySeq [0;3;6])                       // - Recursion
                | 2 -> ()                                           // - Empty
                | 3 -> failwith "This should get caught!"           // - Generic exn throw
                | 4 -> yield (4/0)                                  // - Specific exn throw
                | 5 ->                                            
                    yield 5                                         // - Two yields, will be a state machine
                    yield 5
                | _ -> failwith "This should get caught!"
            with                                               
                | :? System.DivideByZeroException -> yield 4          // - Specific exn
                | anyOther when x = 3 -> yield 3                     // - Generic exn using 'x', no yield
                | anyOther when x = 6 -> ()                          // - Empty yield from 'with' clause
    }
 
if (mySeq [0..5] |> Seq.sum) <> (1+(1+3)+3+4+5+5) then
    failwith $"Sum was {(mySeq [0..5] |> Seq.sum)} instead"
    """
    |> runCode
    |> shouldSucceed

[<Fact>]
let ``Inner try-finally's Dispose is executed before yielding from outer try-with``() =
    Fsx """
let mutable l = []
let s() = seq {
    try
        try
            l <- "Before try" :: l
            yield (1/0)
            l <- "After crash should never happen" :: l
        finally
            l <- "Inside finally" :: l
    with ex when (l <- "Inside with pattern" :: l;true) ->
        l <- "Inside with body" :: l
        yield 1
        l <- "End of with body" :: l
}
let totalSum = s() |> Seq.sum
if totalSum <> 1 then
    failwith $"Sum was {{totalSum}} instead"

failwith $"List is %A{l}"
    """
    |> runCode
    |> shouldSucceed

[<Theory>]
[<InlineData(2)>]
[<InlineData(5000)>]
let ``A sequence expression can recurse itself from with clause``(recLevel:int) =
    Fsx $"""
let rec f () = seq {{
    try
        yield 1
        yield (1/0)
    with pat ->
        yield! f()
}}
let topNsum = f() |> Seq.take {recLevel} |> Seq.sum
if topNsum <> {recLevel} then
    failwith $"Sum was {{topNsum}} instead"
    """
    |> runCode
    |> shouldSucceed

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
    |> runCode
    |> shouldSucceed

[<Fact>]
let ``A sequence expression can have try-with around foreach``() =
    Fsx """
let mySeq (providedInput: seq<int>) =
    seq {
        try
            for x in providedInput do
                yield (6 /  x)
        with _ ->
                yield 100
    } 
let mySum = (mySeq [3;2;1;0]) |> Seq.sum 
if mySum <> 100 then
    failwith $"Sum was {mySum} instead"
    """
    |> runCode
    |> shouldSucceed

[<Fact>]
let ``A sequence expression can have try-with around while``() =
    Fsx """
let mySeq () =
    seq {
        let mutable x = 3
        try
            while true do                
                yield (6/x)
                x <- x-1
        with _ ->
                yield 100
    } 
let mySum = (mySeq () |> Seq.take 10) |> Seq.sum 
if mySum <> (6/3 + 6/2 + 6/1 + 100) then
    failwith $"Sum was {mySum} instead"
    """
    |> runCode
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
    |> runCode
    |> shouldSucceed

[<Fact>]
let ``A sequence expression can fail later in try/with and still get caught``() =
    Fsx """
let sum =
    seq {
        try    
            yield 1   
            yield 2
            yield 3
            yield (10/0)  // This will crash
            yield 4       // This will never be reached
        with _ -> ()     
    }
    |> Seq.sum
if sum <> (1+2+3) then
    failwith $"Sum was {sum} instead"
    """
    |> runCode
    |> shouldSucceed

[<Fact>]
let ``A sequence expression can have inner seq{try/with} in an outer try/with``() =
    Fsx """
let sum =
    seq {     
        try           
            yield 1              
            yield! seq{ try (10 /  0) with _ -> 1}         
            yield 1
        with _ -> yield 100000  // will not get hit, covered by inner 'with'      
    }
    |> Seq.sum
if sum <> (1+1+1) then
    failwith $"Sum was {sum} instead"
    """
    |> runCode
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
    |> runCode
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
    |> runCode
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
    |> runCode
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
    |> runCode
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
let ``seq{try/with} mismatch implicit vs yield``(valInTry,valInWith1,valInWith2) =
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
 