// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open FSharp.Test
open Xunit
open FSharp.Test.Compiler
open FSharp.Test.ScriptHelpers

// Run sequentially because of shared fsiSession.
[<FSharp.Test.RunTestCasesInSequence>]
module SequenceExpression =

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

    [<Fact>]
    let ``With clause in seq expression can bind specific exn type``() =

        Fsx """
open System
let whatIsIt =
    seq {
        try
            yield 1
        with
        | :? AggregateException as exc when (exc.InnerException :? OperationCanceledException) -> ()
    }
    |> Seq.head
        """
        |> compile
        |> verifyIL [
        """IL_0000:  ldarg.1
           IL_0001:  isinst     [runtime]System.AggregateException
           IL_0006:  stloc.0
           IL_0007:  ldloc.0""";

        """IL_000a:  ldloc.0
           IL_000b:  callvirt   instance class [runtime]System.Exception [runtime]System.Exception::get_InnerException()
           IL_0010:  stloc.1
           IL_0011:  ldloc.1
           IL_0012:  isinst     [runtime]System.OperationCanceledException"""]

    [<Fact>]
    let ``With clause in seq expression can bind many exn subtypes``() =

        Fsx """
open System
let whatIsIt =
    seq {
        try
            yield (10/0)
        with
        | :? AggregateException as exc when (exc.InnerException :? OperationCanceledException) -> ()
        | :? AggregateException as exagg when (exagg.InnerExceptions.GetHashCode()) = 15 -> yield (exagg.InnerExceptions.GetHashCode())
        | :? AggregateException as exagg -> yield (exagg.InnerExceptions.GetHashCode())
        | :? DivideByZeroException as exn when exn.Message = "abc" -> yield 0
        | _ -> yield 1
    }
    |> Seq.head
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
    let ``yield may only be used within list, array, and sequence expressions``() =
        Fsx """
let f1 = yield [ 3; 4 ] 
let f2 = yield! [ 3; 4 ]
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 747, Line 2, Col 10, Line 2, Col 15, "This construct may only be used within list, array and sequence expressions, e.g. expressions of the form 'seq { ... }', '[ ... ]' or '[| ... |]'. These use the syntax 'for ... in ... do ... yield...' to generate elements");
            (Error 747, Line 3, Col 10, Line 3, Col 16, "This construct may only be used within list, array and sequence expressions, e.g. expressions of the form 'seq { ... }', '[ ... ]' or '[| ... |]'. These use the syntax 'for ... in ... do ... yield...' to generate elements")
        ]
    
    [<Fact>]
    let ``return may only be used within list, array, and sequence expressions``() =
        Fsx """
let f1 = return [ 3; 4 ] 
let f2 = return! [ 3; 4 ] 
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 748, Line 2, Col 10, Line 2, Col 16, "This construct may only be used within computation expressions. To return a value from an ordinary function simply write the expression without 'return'.");
            (Error 748, Line 3, Col 10, Line 3, Col 17, "This construct may only be used within computation expressions. To return a value from an ordinary function simply write the expression without 'return'.")
        ]

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
            (Error 740, Line 2, Col 1, Line 2, Col 9, "Invalid record, sequence or computation expression. Sequence expressions should be of the form 'seq { ... }'")
            (Error 740, Line 3, Col 4, Line 3, Col 12, "Invalid record, sequence or computation expression. Sequence expressions should be of the form 'seq { ... }'")
            (Error 740, Line 4, Col 9, Line 4, Col 17, "Invalid record, sequence or computation expression. Sequence expressions should be of the form 'seq { ... }'")
            (Error 740, Line 5, Col 12, Line 5, Col 20, "Invalid record, sequence or computation expression. Sequence expressions should be of the form 'seq { ... }'")
            (Error 740, Line 6, Col 11, Line 6, Col 19, "Invalid record, sequence or computation expression. Sequence expressions should be of the form 'seq { ... }'")
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
            (Error 740, Line 2, Col 1, Line 2, Col 9, "Invalid record, sequence or computation expression. Sequence expressions should be of the form 'seq { ... }'")
            (Error 740, Line 3, Col 4, Line 3, Col 12, "Invalid record, sequence or computation expression. Sequence expressions should be of the form 'seq { ... }'")
            (Error 740, Line 4, Col 9, Line 4, Col 17, "Invalid record, sequence or computation expression. Sequence expressions should be of the form 'seq { ... }'")
            (Error 740, Line 5, Col 12, Line 5, Col 20, "Invalid record, sequence or computation expression. Sequence expressions should be of the form 'seq { ... }'")
            (Error 740, Line 6, Col 11, Line 6, Col 19, "Invalid record, sequence or computation expression. Sequence expressions should be of the form 'seq { ... }'")
        ]

    // SOURCE=E_SequenceExpressions01.fs 	# E_SequenceExpressions01.fs
    [<Theory; FileInlineData("E_SequenceExpressions01.fs")>]
    let ``E_SequenceExpressions01 lang version 9`` compilation =
        compilation
        |> getCompilation
        |> withOptions [ "--nowarn:0020" ]
        |> withLangVersion90
        |> typecheck
        |> shouldSucceed

    // SOURCE=E_SequenceExpressions01.fs 	# E_SequenceExpressions01.fs
    [<Theory; FileInlineData("E_SequenceExpressions01.fs")>]
    let ``E_SequenceExpressions01 lang version preview`` compilation =
        compilation
        |> getCompilation
        |> withOptions [ "--nowarn:0020" ]
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 3873, Line 1, Col 1, Line 1, Col 10, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 3, Col 1, Line 3, Col 13, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 5, Col 4, Line 5, Col 13, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 7, Col 4, Line 7, Col 16, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 9, Col 9, Line 9, Col 18, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 11, Col 10, Line 11, Col 23, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 13, Col 12, Line 13, Col 21, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 15, Col 13, Line 15, Col 26, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 17, Col 11, Line 17, Col 20, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 19, Col 4, Line 19, Col 13, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 21, Col 10, Line 21, Col 19, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 23, Col 3, Line 23, Col 12, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 25, Col 3, Line 25, Col 16, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 27, Col 9, Line 27, Col 18, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 29, Col 9, Line 29, Col 22, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 31, Col 13, Line 31, Col 22, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 33, Col 13, Line 33, Col 26, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 35, Col 34, Line 35, Col 51, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 37, Col 35, Line 37, Col 52, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 39, Col 10, Line 39, Col 19, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 41, Col 10, Line 41, Col 22, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 45, Col 12, Line 45, Col 20, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 47, Col 13, Line 47, Col 25, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 49, Col 14, Line 49, Col 22, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 51, Col 33, Line 51, Col 50, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 55, Col 12, Line 55, Col 21, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 57, Col 1, Line 57, Col 13, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 59, Col 28, Line 59, Col 34, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 61, Col 44, Line 61, Col 52, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 61, Col 53, Line 61, Col 61, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 61, Col 62, Line 61, Col 71, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 65, Col 17, Line 65, Col 23, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 65, Col 34, Line 65, Col 44, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 67, Col 7, Line 67, Col 13, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 67, Col 15, Line 67, Col 21, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 67, Col 23, Line 67, Col 30, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 69, Col 14, Line 69, Col 22, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 71, Col 24, Line 71, Col 32, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 73, Col 25, Line 73, Col 33, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 74, Col 25, Line 74, Col 33, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
            (Warning 3873, Line 76, Col 13, Line 76, Col 20, "This construct is deprecated. Sequence expressions should be of the form 'seq { ... }'")
        ]

    // SOURCE=SequenceExpressions01.fs 	# SequenceExpressions01.fs
    [<Theory; FileInlineData("SequenceExpressions01.fs")>]
    let ``SequenceExpressions01 lang version 9`` compilation =
        compilation
        |> getCompilation
        |> withOptions [ "--nowarn:0020" ]
        |> withLangVersion90
        |> typecheck
        |> shouldSucceed
    
    // SOURCE=SequenceExpressions01.fs 	# SequenceExpressions01.fs
    [<Theory; FileInlineData("SequenceExpressions01.fs")>]
    let ``SequenceExpressions01 lang version preview`` compilation =
        compilation
        |> getCompilation
        |> withOptions [ "--nowarn:0020" ]
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed