// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Compiler

module ComputationExpressionTests =
    [<Fact>]
    let ``A CE not using Zero does not require Zero``() =
        FSharp """
module ComputationExpressionTests
type ListBuilder () =
    member _.Combine (a: List<'T>, b) = a @ b
    member _.Yield  x = List.singleton x
    member _.Delay expr = expr () :  List<'T>

let lb = ListBuilder ()

let x = lb {1; 2;}
        """
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``A CE explicitly using Zero fails without a defined Zero``() =
        FSharp """
module ComputationExpressionTests
type ListBuilder () =
    member _.Combine (a: List<'T>, b) = a @ b
    member _.Yield  x = List.singleton x
    member _.Delay expr = expr () :  List<'T>

let lb = ListBuilder ()

let x = lb {1; 2;()}
        """
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 39, Line 10, Col 18, Line 10, Col 20, "The type 'ListBuilder' does not define the field, constructor or member 'Zero'.")
        |> ignore

    [<Fact>]
    let ``A CE explicitly using Zero succeeds with a defined Zero``() =
        FSharp """
module ComputationExpressionTests
type ListBuilder () =
    member _.Zero () = []
    member _.Combine (a: List<'T>, b) = a @ b
    member _.Yield  x = List.singleton x
    member _.Delay expr = expr () :  List<'T>

let lb = ListBuilder ()

let x = lb {1; 2;()}
        """
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``A CE with a complete if-then expression does not require Zero``() =
        FSharp """
module ComputationExpressionTests
type ListBuilder () =
    member _.Combine (a: List<'T>, b) = a @ b
    member _.Yield  x = List.singleton x
    member _.Delay expr = expr () :  List<'T>

let lb = ListBuilder ()

let x = lb {1; 2; if true then 3 else 4;}
        """
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``A CE with a missing/empty else branch implicitly requires Zero``() =
        FSharp """
module ComputationExpressionTests
type ListBuilder () =
    member _.Combine (a: List<'T>, b) = a @ b
    member _.Yield  x = List.singleton x
    member _.Delay expr = expr () :  List<'T>

let lb = ListBuilder ()

let x = lb {1; 2; if true then 3;}
        """
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 708, Line 10, Col 19, Line 10, Col 31, "This control construct may only be used if the computation expression builder defines a 'Zero' method")
        |> ignore

    [<Theory>]
    [<InlineData("preview","BindReturn")>]
    [<InlineData("preview","BindReturn")>]
    [<InlineData("preview","WithoutBindReturn")>]
    [<InlineData("4.7","BindReturn")>]   
    [<InlineData("4.7","WithoutBindReturn")>]  
    let ``A CE with BindReturn and Zero can omit else in an if-then return`` (langVersion, bindReturnName) = 
        let code = $"""
type Builder () =
    member inline __.Return (x: 'T) = Seq.singleton x
    member inline __.Bind (p: seq<'T>, rest: 'T->seq<'U>) = Seq.collect rest p
    member inline __.Zero () = Seq.empty
    member inline __.%s{bindReturnName}  (x : seq<'T>, f: 'T -> 'U)  = Seq.map f x

let seqbuilder= new Builder ()

let _pythags = seqbuilder {{
  let! z = seq [5;10]  
  if (z > 6) then return (z,z) }} """
        code
        |> FSharp
        |> withLangVersion langVersion
        |> typecheck
        |> shouldSucceed

    [<Fact>] 
    let ``A CE with BindReturn and Zero can work without Return if flow control is not used`` () = 
        let code = $"""
type Builder () =
    member inline __.Bind (p: seq<'T>, rest: 'T->seq<'U>) = Seq.collect rest p
    //member inline __.Zero () = Seq.empty
    member inline __.BindReturn  (x : seq<'T>, f: 'T -> 'U)  = Seq.map f x

let seqbuilder= new Builder ()

let _pythags = seqbuilder {{
  let! z = seq [5;10]  
  return (z,z) }} """
        code
        |> FSharp     
        |> typecheck
        |> shouldSucceed
        
    [<Fact>]
    let ``Better CE error range 1`` () =
        Fsx """
let test = task {
    let! x = 1
    return x
}
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 41, Line 3, Col 14, Line 3, Col 15, "No overloads match for method 'Bind'.

Known types of arguments: int * ('a -> TaskCode<'a,'a>)

Available overloads:
 - member TaskBuilderBase.Bind: computation: Async<'TResult1> * continuation: ('TResult1 -> TaskCode<'TOverall,'TResult2>) -> TaskCode<'TOverall,'TResult2> // Argument 'computation' doesn't match
 - member TaskBuilderBase.Bind: task: System.Threading.Tasks.Task<'TResult1> * continuation: ('TResult1 -> TaskCode<'TOverall,'TResult2>) -> TaskCode<'TOverall,'TResult2> // Argument 'task' doesn't match
 - member TaskBuilderBase.Bind: task: ^TaskLike * continuation: ('TResult1 -> TaskCode<'TOverall,'TResult2>) -> TaskCode<'TOverall,'TResult2> when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter) and ^Awaiter :> System.Runtime.CompilerServices.ICriticalNotifyCompletion and ^Awaiter: (member get_IsCompleted: unit -> bool) and ^Awaiter: (member GetResult: unit -> 'TResult1) // Argument 'task' doesn't match")
        ]
        
    [<Fact>]
    let ``Better CE error range 2`` () =
        Fsx """
let test = task {
    let! x = (1, 2)
    return x
}
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 41, Line 3, Col 14, Line 3, Col 20, "No overloads match for method 'Bind'.

Known types of arguments: (int * int) * ('a -> TaskCode<'a,'a>)

Available overloads:
 - member TaskBuilderBase.Bind: computation: Async<'TResult1> * continuation: ('TResult1 -> TaskCode<'TOverall,'TResult2>) -> TaskCode<'TOverall,'TResult2> // Argument 'computation' doesn't match
 - member TaskBuilderBase.Bind: task: System.Threading.Tasks.Task<'TResult1> * continuation: ('TResult1 -> TaskCode<'TOverall,'TResult2>) -> TaskCode<'TOverall,'TResult2> // Argument 'task' doesn't match
 - member TaskBuilderBase.Bind: task: ^TaskLike * continuation: ('TResult1 -> TaskCode<'TOverall,'TResult2>) -> TaskCode<'TOverall,'TResult2> when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter) and ^Awaiter :> System.Runtime.CompilerServices.ICriticalNotifyCompletion and ^Awaiter: (member get_IsCompleted: unit -> bool) and ^Awaiter: (member GetResult: unit -> 'TResult1) // Argument 'task' doesn't match")
        ]
        
    [<Fact>]
    let ``Better error range 3`` () =
        Fsx """
let test = task {
    return! 1
}
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 41, Line 3, Col 5, Line 3, Col 14, "No overloads match for method 'ReturnFrom'.

Known type of argument: int

Available overloads:
 - member TaskBuilderBase.ReturnFrom: computation: Async<'T> -> TaskCode<'T,'T> // Argument 'computation' doesn't match
 - member TaskBuilderBase.ReturnFrom: task: System.Threading.Tasks.Task<'T> -> TaskCode<'T,'T> // Argument 'task' doesn't match
 - member TaskBuilderBase.ReturnFrom: task: ^TaskLike -> TaskCode<'T,'T> when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter) and ^Awaiter :> System.Runtime.CompilerServices.ICriticalNotifyCompletion and ^Awaiter: (member get_IsCompleted: unit -> bool) and ^Awaiter: (member GetResult: unit -> 'T) // Argument 'task' doesn't match")
        ]
        
    [<Fact>]
    let ``Better error range 4`` () =
        Fsx """
    type C() =
        static member M1([<System.ParamArray>] x:int64[]) = Array.sum x
    let x1 = C.M1(2)
        """
        |> withLangVersion50
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 41, Line 4, Col 19, Line 4, Col 20, "No overloads match for method 'M1'.

Known type of argument: int

Available overloads:
 - static member C.M1: [<System.ParamArray>] x: int64 array -> int64 // Argument 'x' doesn't match
 - static member C.M1: [<System.ParamArray>] x: int64 array -> int64 // Argument at index 1 doesn't match")
            ]
        
    [<Fact>]
    let ``Better error range 5`` () =
        Fsx """
    type C() =
        static member M1([<System.ParamArray>] x:int64[]) = Array.sum x
    let x1 = C.M1(x=2)
        """
        |> withLangVersion50
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 4, Col 21, Line 4, Col 22, "This expression was expected to have type
    'int64 array'    
but here has type
    'int'    ")
            ]