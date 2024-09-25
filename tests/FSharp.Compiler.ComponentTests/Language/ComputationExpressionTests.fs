﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

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
    let ``A CE returned from type member succeeds``() =
        FSharp """
module ComputationExpressionTests
type Builder () =
    member _.Bind(x, f) = f x
    member _.Return(x) = x

type A =
    static member Prop = Builder ()

let x = A.Prop { return 0 }
        """
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``use! may not be combined with and!`` () =
        Fsx """
module Result =
    let zip x1 x2 =
        match x1,x2 with
        | Ok x1res, Ok x2res -> Ok (x1res, x2res)
        | Error e, _ -> Error e
        | _, Error e -> Error e

type ResultBuilder() =
    member _.MergeSources(t1: Result<'T,'U>, t2: Result<'T1,'U>) = Result.zip t1 t2
    member _.BindReturn(x: Result<'T,'U>, f) = Result.map f x

let result = ResultBuilder()

let run r2 r3 =
    result {
        use! b = r2
        and! c = r3
        return b - c
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3345, Line 18, Col 9, Line 18, Col 13, "use! may not be combined with and!")
        ]
        
    [<Fact>]
    let ``multiple use! may not be combined with and!`` () =
        Fsx """
module Result =
    let zip x1 x2 =
        match x1,x2 with
        | Ok x1res, Ok x2res -> Ok (x1res, x2res)
        | Error e, _ -> Error e
        | _, Error e -> Error e

type ResultBuilder() =
    member _.MergeSources(t1: Result<'T,'U>, t2: Result<'T1,'U>) = Result.zip t1 t2
    member _.BindReturn(x: Result<'T,'U>, f) = Result.map f x

let result = ResultBuilder()

let run r2 r3 =
    result {
        use! b = r2
        and! c = r3
        use! d = r2
        return b - c
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3345, Line 18, Col 9, Line 18, Col 13, "use! may not be combined with and!")
        ]
        
    [<Fact>]
    let ``multiple use! may not be combined with multiple and!`` () =
        Fsx """
module Result =
    let zip x1 x2 =
        match x1,x2 with
        | Ok x1res, Ok x2res -> Ok (x1res, x2res)
        | Error e, _ -> Error e
        | _, Error e -> Error e

type ResultBuilder() =
    member _.MergeSources(t1: Result<'T,'U>, t2: Result<'T1,'U>) = Result.zip t1 t2
    member _.BindReturn(x: Result<'T,'U>, f) = Result.map f x
    member _.BindReturn(x: Result<'T,'U>, f) = Result.map f x
    member _.Bind(x: Result<'T,'U>, f) = Result.bind f x

let result = ResultBuilder()

let run r2 r3 =
    result {
        let! c = r3
        and! c = r3
        use! b = r2
        and! c = r3
        return b - c
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3345, Line 22, Col 9, Line 22, Col 13, "use! may not be combined with and!")
        ]

    [<Fact>]
    let ``This control construct may only be used if the computation expression builder defines a 'Bind' method`` () =
        Fsx """
module Result =
    let zip x1 x2 =
        match x1,x2 with
        | Ok x1res, Ok x2res -> Ok (x1res, x2res)
        | Error e, _ -> Error e
        | _, Error e -> Error e

type ResultBuilder() =
    member _.MergeSources(t1: Result<'T,'U>, t2: Result<'T1,'U>) = Result.zip t1 t2
    member _.BindReturn(x: Result<'T,'U>, f) = Result.map f x
    member _.Delay(f) = f()
    
    member _.TryWith(r: Result<'T,'U>, f) =
        match r with
        | Ok x -> Ok x
        | Error e -> f e

let result = ResultBuilder()

let run r2 r3 =
    result {
        let! a = r2
        return! a
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 23, Col 9, Line 23, Col 13, "This control construct may only be used if the computation expression builder defines a 'Bind' method")
        ]
    
    [<Fact>]
    let ``This control construct may only be used if the computation expression builder defines a 'Using' method`` () =
        Fsx """
module Result =
    let zip x1 x2 =
        match x1,x2 with
        | Ok x1res, Ok x2res -> Ok (x1res, x2res)
        | Error e, _ -> Error e
        | _, Error e -> Error e

type ResultBuilder() =
    member _.MergeSources(t1: Result<'T,'U>, t2: Result<'T1,'U>) = Result.zip t1 t2
    member _.BindReturn(x: Result<'T,'U>, f) = Result.map f x
    member _.Delay(f) = f()
    
    member _.TryWith(r: Result<'T,'U>, f) =
        match r with
        | Ok x -> Ok x
        | Error e -> f e

let result = ResultBuilder()

let run r2 r3 =
    result {
        use! a = r2
        return! a
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 23, Col 9, Line 23, Col 13, "This control construct may only be used if the computation expression builder defines a 'Using' method")
        ]
    
    [<Fact>]
    let ``do! expressions may not be used in queries`` () =
        Fsx """
query {
    do! failwith ""
    yield 1
}
    """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3143, Line 3, Col 5, Line 3, Col 8, "'let!', 'use!' and 'do!' expressions may not be used in queries")
        ]
        
    [<Fact>]
    let ``let! expressions may not be used in queries`` () =
        Fsx """
query {
    let! x = failwith ""
    yield 1
}
    """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3143, Line 3, Col 5, Line 3, Col 9, "'let!', 'use!' and 'do!' expressions may not be used in queries")
        ]
            
    [<Fact>]
    let ``let!, and! expressions may not be used in queries`` () =
        Fsx """
query {
    let! x = failwith ""
    and! y = failwith ""
    yield 1
}
    """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3143, Line 3, Col 5, Line 3, Col 9, "'let!', 'use!' and 'do!' expressions may not be used in queries")
        ]
        
    [<Fact>]
    let ``use! expressions may not be used in queries`` () =
        Fsx """
query {
    use! x = failwith ""
    yield 1
}
    """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3143, Line 3, Col 5, Line 3, Col 9, "'let!', 'use!' and 'do!' expressions may not be used in queries")
        ]

    [<Fact>]
    let ``do! expressions may not be used in queries(SynExpr.Sequential)`` () =
        Fsx """
query {
    for c in [1..10] do
    do! failwith ""
    yield 1
}
    """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3143, Line 4, Col 5, Line 4, Col 8, "'let!', 'use!' and 'do!' expressions may not be used in queries")  
        ]
        
    [<Fact>]
    let ``let! expressions may not be used in queries(SynExpr.Sequential)`` () =
        Fsx """
query {
    for c in [1..10] do
    let! x = failwith ""
    yield 1
}
    """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3143, Line 4, Col 5, Line 4, Col 9, "'let!', 'use!' and 'do!' expressions may not be used in queries")
        ]
        
    [<Fact>]
    let ``let!, and! expressions may not be used in queries(SynExpr.Sequential)`` () =
        Fsx """
query {
    for c in [1..10] do
    let! x = failwith ""
    and! y = failwith ""
    yield 1
}
    """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3143, Line 4, Col 5, Line 4, Col 9, "'let!', 'use!' and 'do!' expressions may not be used in queries")
        ]
        
    [<Fact>]
    let ``use! expressions may not be used in queries(SynExpr.Sequential)`` () =
        Fsx """
query {
    for c in [1..10] do
    use! x = failwith ""
    yield 1
}
    """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3143, Line 4, Col 5, Line 4, Col 9, "'let!', 'use!' and 'do!' expressions may not be used in queries")
        ]