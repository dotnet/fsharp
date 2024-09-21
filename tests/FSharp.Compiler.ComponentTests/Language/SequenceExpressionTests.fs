// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Language.SequenceExpressionTests

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.ScriptHelpers


let fsiSession = getSessionForEval [||] LangVersion.Preview

let runCode = evalInSharedSession fsiSession

[<Fact>]
let ``Basic recursive case uses tail recursion``() =
    Fsx """
let rec f () = seq {
    try 
        yield 123    
        yield (456/0)
    with pat ->
        yield 789
        yield! f()
}
    """
    |> withLangVersion80
    |> compile
    |> verifyIL ["
      .class auto ansi serializable sealed nested assembly beforefieldinit 'f@3-1'
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.Collections.Generic.IEnumerable`1<int32>>
  {
    .field static assembly initonly class Test/'f@3-1' @_instance
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.Collections.Generic.IEnumerable`1<int32>>::.ctor()
      IL_0006:  ret
    } 

    .method public strict virtual instance class [runtime]System.Collections.Generic.IEnumerable`1<int32> 
            Invoke(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar) cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.s   123
      IL_0002:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Singleton<int32>(!!0)
      IL_0007:  ldsfld     class Test/'f@5-2' Test/'f@5-2'::@_instance
      IL_000c:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Delay<int32>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.Collections.Generic.IEnumerable`1<!!0>>)
      IL_0011:  tail.
      IL_0013:  call       class [runtime]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::Append<int32>(class [runtime]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                            class [runtime]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0018:  ret
    } "]

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
l <- "Before sum" :: l
let totalSum = s() |> Seq.sum
l <- "After sum" :: l
if totalSum <> 1 then
    failwith $"Sum was {{totalSum}} instead"

l <- List.rev l
let expectedList = 
    [ "Before sum"   // Seq is lazy, so we do not expect anything until iteration starts
      "Before try"
      "Inside finally"
      "Inside with pattern"
      "Inside with pattern"   // Yes indeed, the exn matching pattern is executed twice
      "Inside with body"
      "End of with body"
      "After sum"]
if l<> expectedList then
    failwith $" List is %A{l}"
    """
    |> runCode
    |> shouldSucceed

[<Theory>]
[<InlineData(10)>]
[<InlineData(100)>]
[<InlineData(1000)>]
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
if mySum <> (6/3 + 6/2 + 6/1 + 100) then
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
let mySum = (mySeq () |> Seq.truncate 10) |> Seq.sum 
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
    |> withLangVersion80
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
    |> withLangVersion80
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
    |> withLangVersion80
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
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withErrorCode 1
    |> withDiagnosticMessageMatches "This expression was expected to have type"


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
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withErrorCode 30
    |> withDiagnosticMessageMatches "Value restriction: The value 'typedSeq' has an inferred generic type"
    |> withDiagnosticMessageMatches "val typedSeq: '_a seq"

[<Fact>]
let ``Sequence(SynExpr.Sequential) expressions should be of the form 'seq { ... } lang version 9``() =
    Fsx """
{ 1;10 }
[| { 1;10 } |]
let a = { 1;10 }
let b = [| { 1;10 } |]
let c = [ { 1;10 } ]
    """
    |> withOptions [ "--nowarn:0020" ]
    |> withLangVersion90
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
      (Error 740, Line 2, Col 1, Line 2, Col 9, "Sequence expressions should be of the form 'seq { ... }'")
      (Error 740, Line 3, Col 4, Line 3, Col 12, "Sequence expressions should be of the form 'seq { ... }'")
      (Error 740, Line 4, Col 9, Line 4, Col 17, "Sequence expressions should be of the form 'seq { ... }'")
      (Error 740, Line 5, Col 12, Line 5, Col 20, "Sequence expressions should be of the form 'seq { ... }'")
      (Error 740, Line 6, Col 11, Line 6, Col 19, "Sequence expressions should be of the form 'seq { ... }'")
    ]
    
[<Fact>]
let ``Sequence(SynExpr.Sequential) expressions should be of the form 'seq { ... } lang version preview``() =
    Fsx """
{ 1;10 }
[| { 1;10 } |]
let a = { 1;10 }
let b = [| { 1;10 } |]
let c = [ { 1;10 } ]
    """
    |> withOptions [ "--nowarn:0020" ]
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
      (Error 740, Line 2, Col 1, Line 2, Col 9, "Sequence expressions should be of the form 'seq { ... }'")
      (Error 740, Line 3, Col 4, Line 3, Col 12, "Sequence expressions should be of the form 'seq { ... }'")
      (Error 740, Line 4, Col 9, Line 4, Col 17, "Sequence expressions should be of the form 'seq { ... }'")
      (Error 740, Line 5, Col 12, Line 5, Col 20, "Sequence expressions should be of the form 'seq { ... }'")
      (Error 740, Line 6, Col 11, Line 6, Col 19, "Sequence expressions should be of the form 'seq { ... }'")
    ]

[<Fact>]
let ``Sequence(SynExpr.IndexRange) expressions should be of the form 'seq { ... } lang version 9``() =
    Fsx """
{ 1..10 }

{ 1..5..10 }

[| { 1..10 } |]

[| { 1..5..10 } |]

let a = { 1..10 }

let a3 = { 1..10..20 }

let b = [| { 1..10 } |]

let b3 = [| { 1..10..20 } |]

let c = [ { 1..10 } ]

[| { 1..10 } |]

[| yield { 1..10 } |]

[ { 1..10 } ]

[ { 1..10..10 } ]

[ yield { 1..10 } ]

[ yield { 1..10..20 } ]

ResizeArray({ 1..10 })

ResizeArray({ 1..10..20 })

let fw start finish = [ for x in { start..finish } -> x ]

let fe start finish = [| for x in { start..finish } -> x |]

for x in { 1..10 }  do ()

for x in { 1..5..10 } do ()
    
let f = Seq.head

let a2 = f { 1..6 }

let a23 = f { 1..6..10 }

let b2 = set { 1..6 }

let f10 start finish = for x in { start..finish } do ignore (float x ** float x)

let (..) _ _ = "lol"

let lol1 = { 1..10 }

{ 1..5..10 }
    """
    |> withOptions [ "--nowarn:0020" ]
    |> withLangVersion90
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Sequence(SynExpr.IndexRange) expressions should be of the form 'seq { ... }``() =
    Fsx """
{ 1..10 }

{ 1..5..10 }

[| { 1..10 } |]

[| { 1..5..10 } |]

let a = { 1..10 }

let a3 = { 1..10..20 }

let b = [| { 1..10 } |]

let b3 = [| { 1..10..20 } |]

let c = [ { 1..10 } ]

[| { 1..10 } |]

[| yield { 1..10 } |]

[ { 1..10 } ]

[ { 1..10..10 } ]

[ yield { 1..10 } ]

[ yield { 1..10..20 } ]

ResizeArray({ 1..10 })

ResizeArray({ 1..10..20 })

let fw start finish = [ for x in { start..finish } -> x ]

let fe start finish = [| for x in { start..finish } -> x |]

for x in { 1..10 }  do ()

for x in { 1..5..10 } do ()
    
let f = Seq.head

let a2 = f { 1..6 }

let a23 = f { 1..6..10 }

let b2 = set { 1..6 }

let f10 start finish = for x in { start..finish } do ignore (float x ** float x)

let (..) _ _ = "lol"

let lol1 = { 1..10 }

{ 1..5..10 }
    """
    |> withOptions [ "--nowarn:0020" ]
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Warning 740, Line 2, Col 1, Line 2, Col 10, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 4, Col 1, Line 4, Col 13, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 6, Col 4, Line 6, Col 13, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 8, Col 4, Line 8, Col 16, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 10, Col 9, Line 10, Col 18, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 12, Col 10, Line 12, Col 23, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 14, Col 12, Line 14, Col 21, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 16, Col 13, Line 16, Col 26, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 18, Col 11, Line 18, Col 20, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 20, Col 4, Line 20, Col 13, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 22, Col 10, Line 22, Col 19, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 24, Col 3, Line 24, Col 12, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 26, Col 3, Line 26, Col 16, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 28, Col 9, Line 28, Col 18, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 30, Col 9, Line 30, Col 22, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 32, Col 13, Line 32, Col 22, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 34, Col 13, Line 34, Col 26, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 36, Col 34, Line 36, Col 51, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 38, Col 35, Line 38, Col 52, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 40, Col 10, Line 40, Col 19, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 42, Col 10, Line 42, Col 22, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 46, Col 12, Line 46, Col 20, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 48, Col 13, Line 48, Col 25, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 50, Col 14, Line 50, Col 22, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 52, Col 33, Line 52, Col 50, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 56, Col 12, Line 56, Col 21, "Sequence expressions should be of the form 'seq { ... }'");
        (Warning 740, Line 58, Col 1, Line 58, Col 13, "Sequence expressions should be of the form 'seq { ... }'")
    ]