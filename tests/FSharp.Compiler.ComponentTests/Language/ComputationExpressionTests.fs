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
    let ``Version 9.0: Allow CE return and type annotations don't play well together needing parentheses``() =
        FSharp """
module ComputationExpressionTests
open System

type MyType() =
    interface IDisposable with
        member this.Dispose () = ()

let f () =
    async {
        return new MyType() : IDisposable
    }

let f1 () =
    async {
        return new MyType() :> IDisposable
    }
        
let f2 () : Async<IDisposable> =
    async {
        return new MyType()
    }
        
let f3 () =
    async {
        return (new MyType() : IDisposable)
    }
        """
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 11, Col 16, Line 11, Col 42, "Feature 'Allow let! and use! type annotations without requiring parentheses' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]
        
    [<Fact>]
    let ``Version 9.0: Allow CE return! and type annotations don't to play well together needing parentheses``() =
        FSharp """
module ComputationExpressionTests

type ResultBuilder() =
    member _.Return(x) = Ok x
    member _.ReturnFrom(x) = x
    member _.Bind(m, f) = 
        match m with
        | Ok a -> f a
        | Error e -> Error e

let result = ResultBuilder()

let f() =
    result {
        return! Ok 1 : Result<int, string>
    }
    
let f1() =
    result {
        return! (Ok 1 : Result<int, string>)
    }
        """
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 16, Col 17, Line 16, Col 43, "Feature 'Allow let! and use! type annotations without requiring parentheses' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]
        
    [<Fact>]
    let ``Preview: Allow CE return and type annotations to play well together without needing parentheses``() =
        FSharp """
module ComputationExpressionTests
open System

type MyType() =
    interface IDisposable with
        member this.Dispose () = ()

let f () =
    async {
        return new MyType() : IDisposable
    }

let f1 () =
    async {
        return new MyType() :> IDisposable
    }
        
let f2 () : Async<IDisposable> =
    async {
        return new MyType()
    }
        
let f3 () =
    async {
        return (new MyType() : IDisposable)
    }
        """
        |> withLangVersionPreview
        |> asExe
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Preview: Allow CE return! and type annotations to play well together without needing parentheses``() =
        FSharp """
module ComputationExpressionTests

type ResultBuilder() =
    member _.Return(x) = Ok x
    member _.ReturnFrom(x) = x
    member _.Bind(m, f) = 
        match m with
        | Ok a -> f a
        | Error e -> Error e

let result = ResultBuilder()

let f() =
    result {
        return! Ok 1 : Result<int, string>
    }
    
let f1() =
    result {
        return! (Ok 1 : Result<int, string>)
    }
        """
        |> withLangVersionPreview
        |> asExe
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

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

    [<Fact>]
    let ``This control construct may only be used if the computation expression builder defines a 'Bind' method(match!)`` () =
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
        match! r2 with
        | Ok x -> return x
        | Error e -> return e
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 23, Col 9, Line 23, Col 15, "This control construct may only be used if the computation expression builder defines a 'Bind' method")
        ]

    [<Fact>]
    let ``This construct may only be used within computation expressions(match!)`` () =
        Fsx """
let run r2 r3 =
    match! r2 with
    | Ok x -> x
    | Error e -> e
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 750, Line 3, Col 5, Line 3, Col 11, "This construct may only be used within computation expressions")
        ]
        
    [<Fact>]
    let ``This control construct may only be used if the computation expression builder defines a 'Yield' method`` () =
        Fsx """
let f3 =
    async {
        if true then
            yield "a"
        else
            yield "b"
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 5, Col 13, Line 5, Col 18, "This control construct may only be used if the computation expression builder defines a 'Yield' method")
        ]
        
    [<Fact>]
    let ``This control construct may only be used if the computation expression builder defines a 'YieldFrom' method`` () =
        Fsx """
let f3 =
    async {
        if true then
            yield! "a"
        else
            yield "b"
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 5, Col 13, Line 5, Col 19, "This control construct may only be used if the computation expression builder defines a 'YieldFrom' method")
        ]
    

    [<Fact>]
    let ``This control construct may only be used if the computation expression builder defines a 'Return' method`` () =
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
        match r2 with
        | Ok x -> return x
        | Error e -> return e
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 24, Col 19, Line 24, Col 25, "This control construct may only be used if the computation expression builder defines a 'Return' method")
        ]
        

    [<Fact>]
    let ``This control construct may only be used if the computation expression builder defines a 'ReturnFrom' method`` () =
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
        match r2 with
        | Ok x -> return! x
        | Error e -> return e
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 24, Col 19, Line 24, Col 26, "This control construct may only be used if the computation expression builder defines a 'ReturnFrom' method")
        ]        

    [<Fact>]
    let ``This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
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
    member _.Bind(x: Result<'T,'U>, f) =
        match x with
        | Ok x -> f x
        | Error e -> Error e
        
    member _.Zero() = Ok ()

let result = ResultBuilder()

let run r2 r3 =
    result {
        let! r2 = r2
        let! r3 = r3
        if r2 = Ok 1 then
            do! r2
        return r3
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 27, Col 9, Line 27, Col 15, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]
        
    [<Fact>]
    let ``Sequence2 This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
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
    member _.Bind(x: Result<'T,'U>, f) =
        match x with
        | Ok x -> f x
        | Error e -> Error e
        
    member _.Zero() = Ok ()

    member _.For(sequence: #seq<'T>, binder: 'T -> Result<_, _>) =
        sequence
        |> Seq.map binder
        |> Seq.fold (fun acc x -> Result.bind (fun () -> x) acc) (Ok ())

let result = ResultBuilder()

let run (r2: Result<int, string>) (r3: Result<int, string>) =
    result {
        let! r2 = r2
        let! r3 = r3
        for i in [ 1] do
            do! r2
        return r3
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 32, Col 9, Line 32, Col 15, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]
        
    [<Fact>]
    let ``Sequence 5 This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
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
    member _.Bind(x: Result<'T,'U>, f) =
        match x with
        | Ok x -> f x
        | Error e -> Error e
        
    member _.Zero() = Ok ()

    member _.For(sequence: #seq<'T>, binder: 'T -> Result<_, _>) =
        sequence
        |> Seq.map binder
        |> Seq.fold (fun acc x -> Result.bind (fun () -> x) acc) (Ok ())

let result = ResultBuilder()

let run (r2: Result<int, string>) (r3: Result<int, string>) =
    result {
        let! r2 = r2
        let! r3 = r3
        for i in [ 1] do
            do! r2
        return! r3
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 32, Col 9, Line 32, Col 16, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]
    
    [<Fact>]
    let ``Sequence 7 This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
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
    member _.Bind(x: Result<'T,'U>, f) =
        match x with
        | Ok x -> f x
        | Error e -> Error e
        
    member _.Zero() = Ok ()

    member _.For(sequence: #seq<'T>, binder: 'T -> Result<_, _>) =
        sequence
        |> Seq.map binder
        |> Seq.fold (fun acc x -> Result.bind (fun () -> x) acc) (Ok ())

let result = ResultBuilder()

let run (r2: Result<int, string>) (r3: Result<int, string>) =
    result {
        let! r2 = r2
        let! r3 = r3
        match r2 with
        | 0 -> do! r2
        | _ -> do! r2
        return! r3
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 33, Col 9, Line 33, Col 16, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]    
    
    [<Fact>]
    let ``Sequence 8 This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
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
    member _.Bind(x: Result<'T,'U>, f) =
        match x with
        | Ok x -> f x
        | Error e -> Error e
        
    member _.Zero() = Ok ()

    member _.For(sequence: #seq<'T>, binder: 'T -> Result<_, _>) =
        sequence
        |> Seq.map binder
        |> Seq.fold (fun acc x -> Result.bind (fun () -> x) acc) (Ok ())

let result = ResultBuilder()

let run (r2: Result<int, string>) (r3: Result<int, string>) =
    result {
        let! r2 = r2
        let! r3 = r3
        match! r2 with
        | 0 -> do! r2
        | _ -> do! r2
        return! r3
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 33, Col 9, Line 33, Col 16, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]
    
    [<Fact>]
    let ``Sequence 9 This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
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
    member _.Bind(x: Result<'T,'U>, f) =
        match x with
        | Ok x -> f x
        | Error e -> Error e
        
    member _.Zero() = Ok ()

    member _.For(sequence: #seq<'T>, binder: 'T -> Result<_, _>) =
        sequence
        |> Seq.map binder
        |> Seq.fold (fun acc x -> Result.bind (fun () -> x) acc) (Ok ())

let result = ResultBuilder()

let run (r2: Result<int, string>) (r3: Result<int, string>) =
    result {
        let! r2 = r2
        let! r3 = r3
        match! r2 with
        | 0 -> do! r2
        | _ -> do! r2
        return r3
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 33, Col 9, Line 33, Col 15, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]

    [<Fact>]
    let ``Sequence3 This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
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
    member _.Bind(x: Result<'T,'U>, f) =
        match x with
        | Ok x -> f x
        | Error e -> Error e
        
    member _.Zero() = Ok ()
    
    member _.Yield(x: 'T) = Ok x

    member _.For(sequence: #seq<'T>, binder: 'T -> Result<_, _>) =
        sequence
        |> Seq.map binder
        |> Seq.fold (fun acc x -> Result.bind (fun () -> x) acc) (Ok ())

let result = ResultBuilder()

let run (r2: Result<int, string>) (r3: Result<int, string>) =
    result {
        let! r2 = r2
        let! r3 = r3
        for i in [ 1] do
            yield r2
        return r3
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 34, Col 9, Line 34, Col 15, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]
        
    [<Fact>]
    let ``Sequence 4 This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
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
    member _.Bind(x: Result<'T,'U>, f) =
        match x with
        | Ok x -> f x
        | Error e -> Error e
        
    member _.Zero() = Ok ()
    
    member _.Yield(x: 'T) = Ok x

    member _.For(sequence: #seq<'T>, binder: 'T -> Result<_, _>) =
        sequence
        |> Seq.map binder
        |> Seq.fold (fun acc x -> Result.bind (fun () -> x) acc) (Ok ())

let result = ResultBuilder()

let run (r2: Result<int, string>) (r3: Result<int, string>) =
    result {
        let! r2 = r2
        let! r3 = r3
        for i in [ 1] do
            yield r2
        yield r3
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 34, Col 9, Line 34, Col 14, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]        
    [<Fact>]
    let ``Sequence 6 This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
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
    member _.Bind(x: Result<'T,'U>, f) =
        match x with
        | Ok x -> f x
        | Error e -> Error e
        
    member _.Zero() = Ok ()
    
    member _.Yield(x: 'T) = Ok x
    
    member _.YieldFrom(x: Result<'T,'U>) = x

    member _.For(sequence: #seq<'T>, binder: 'T -> Result<_, _>) =
        sequence
        |> Seq.map binder
        |> Seq.fold (fun acc x -> Result.bind (fun () -> x) acc) (Ok ())

let result = ResultBuilder()

let run (r2: Result<int, string>) (r3: Result<int, string>) =
    result {
        let! r2 = r2
        let! r3 = r3
        for i in [ 1] do
            yield! r2
        yield! r3
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 36, Col 9, Line 36, Col 15, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]

    [<Fact>]
    let ``Sequence 10 This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
        Fsx """
module Test =
    type R = S of string 
     
    type T() = 
      member x.Bind(p: R, rest: (string -> R)) =  
        match p with 
        | S(s) -> rest s 
      member x.Zero() = S("l")
      member x.For(s : seq<int>, rest: (int -> unit)) = S("")

    let t = new T()

    let t' = t { 
      let a = 10
      for x in [1] do ()
      0 |> ignore
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 17, Col 7, Line 17, Col 18, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]

    [<Fact>]
    let ``Sequence 11 This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
        Fsx """
module Test =
    type R = S of string 
     
    type T() = 
      member x.Bind(p: R, rest: string -> R) =  
        match p with 
        | S(s) -> rest s 
      member x.Zero() = S("l")
      member x.For(s: seq<int>, rest: (int -> R)) = 
        let folder state item =
            match state with
            | S(str) ->
                match rest item with
                | S(itemStr) -> S(str + itemStr)
        Seq.fold folder (S("")) s

    let t = new T()

    let t' = t { 
      let a = 10
      for x in [1] do
          ()
      for x in [1] do
          ()
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 24, Col 7, Line 25, Col 13, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]

    [<Fact>]
    let ``Sequence 12 This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
        Fsx """
module Test =
    type R = S of string 
     
    type T() = 
      member x.Bind(p: R, rest: string -> R) =  
        match p with 
        | S(s) -> rest s 
      member x.Zero() = S("l")
      member x.Yield(value: 'a) = S(string value)
      member x.For(s: seq<int>, rest: (int -> R)) = 
        let folder state item =
            match state with
            | S(str) ->
                match rest item with
                | S(itemStr) -> S(str + itemStr)
        Seq.fold folder (S("")) s

    let t = new T()

    let t' = t { 
      let a = 10
      for x in [1] ->
          ()
      for x in [1] ->
          ()
    }
    
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 25, Col 7, Line 26, Col 13, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]
        
    [<Fact>]
    let ``Sequence 13 This control construct may only be used if the computation expression builder defines a 'Combine' method`` () =
        Fsx """
module Test =
    type R = S of string 
     
    type T() = 
      member x.Bind(p: R, rest: string -> R) =  
        match p with 
        | S(s) -> rest s 
      member x.Zero() = S("l")
      member x.For(s: seq<int>, rest: (int -> R)) = 
        let folder state item =
            match state with
            | S(str) ->
                match rest item with
                | S(itemStr) -> S(str + itemStr)
        Seq.fold folder (S("")) s

    let t = new T()

    let t' = t { 
      let a = 10
      for x in [1] do ()
      if true then
          ()
      else
          ()
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 23, Col 7, Line 26, Col 13, "This control construct may only be used if the computation expression builder defines a 'Combine' method")
        ]


    [<Fact>]
    let ``Type constraint mismatch when using return!`` () =
        Fsx """
open System.Threading.Tasks

let maybeTask = task { return false }

let indexHandler (): Task<string> = 
    task {
        return! maybeTask
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 193, Line 8, Col 17, Line 8, Col 26, "Type constraint mismatch. The type \n    'TaskCode<bool,bool>'    \nis not compatible with type\n    'TaskCode<string,string>'    \n")
        ]

    [<Fact>]
    let ``Type constraint mismatch when using return`` () =
        Fsx """
open System.Threading.Tasks

let maybeTask = task { return false }

let indexHandler (): Task<string> = 
    task {
        return maybeTask
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 8, Col 16, Line 8, Col 25, "This expression was expected to have type
    'string'    
but here has type
    'Task<bool>'    ")
        ]

    [<Fact>]
    let ``use expressions may not be used in queries(SynExpr.Sequential)`` () =
        Fsx """
let x11 = 
    query { for c in [1..10] do
            use x = { new System.IDisposable with __.Dispose() = () }
            yield 1  }   
    """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3142, Line 4, Col 13, Line 4, Col 16, "'use' expressions may not be used in queries")
        ]

    [<Fact>]
    let ``use, This control construct may only be used if the computation expression builder defines a 'Using' method`` () =
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
    
    member _.YieldReturn(x: Result<'T,'U>) = x
    member _.Return(x: 'T) = Ok x

let result = ResultBuilder()

let run r2 r3 =
    result {
        use b = r2
        return Ok 0
    }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 708, Line 20, Col 9, Line 20, Col 12, "This control construct may only be used if the computation expression builder defines a 'Using' method")
        ]

    [<Fact>]
    let ``This 'let' definition may not be used in a query. Only simple value definitions may be used in queries.`` () =
        Fsx """
let x18rec2 = 
    query {
        for d in [1..10] do
        let rec f x = x + 1 // error expected here - no recursive functions
        and g x = f x + 2
        select (f d)
    }
    
let x18inline = 
    query {
        for d in [1..10] do
        let inline f x = x + 1 // error expected here - no inline functions
        select (f d)
    }
    
let x18mutable = 
    query {
        for d in [1..10] do
        let mutable v = 1 // error expected here - no mutable values
        select (f d)
    }

    """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3147, Line 5, Col 17, Line 5, Col 20, "This 'let' definition may not be used in a query. Only simple value definitions may be used in queries.")
            (Error 3147, Line 13, Col 20, Line 13, Col 23, "This 'let' definition may not be used in a query. Only simple value definitions may be used in queries.")
            (Error 3147, Line 20, Col 21, Line 20, Col 22, "This 'let' definition may not be used in a query. Only simple value definitions may be used in queries.")
        ]
        
    [<Fact>]
    let ``Fix resumable and non-resumable CE error ranges`` () =
        FSharp """
module Test
        
open System.Threading.Tasks
let minimum () : Async<int> =
    async {
        let! batch = async { return 1 }
        return "1"
    }
    
let minimum2 () : Task<int> =
    task {
        let! batch = task { return 1 }
        return "1"
    }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 8, Col 16, Line 8, Col 19, "This expression was expected to have type
'int' 
but here has type
'string' ");
            (Error 193, Line 14, Col 16, Line 14, Col 19, "Type constraint mismatch. The type 
'TaskCode<string,string>' 
is not compatible with type
'TaskCode<int,int>' 
")
        ]
        
    [<Fact>]
    let ``Fix resumable and non-resumable CE error ranges 2`` () =
        FSharp """
module Test
        
open System.Threading.Tasks
let minimum () : Async<int> =
    async {
        let batch: Async<int> = async { return "" }
        return "1"
    }
    
let minimum2 () : Task<int> =
    task {
        let batch: Task<int> = task { return "" }
        return "1"
    }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 7, Col 48, Line 7, Col 50, "This expression was expected to have type
'int' 
but here has type
'string' ");
            (Error 1, Line 8, Col 16, Line 8, Col 19, "This expression was expected to have type
'int' 
but here has type
'string' ");
            (Error 1, Line 13, Col 46, Line 13, Col 48, "This expression was expected to have type
'int' 
but here has type
'string' ");
            (Error 1, Line 14, Col 16, Line 14, Col 19, "This expression was expected to have type
'int' 
but here has type
'string' ")
        ]        
    
    [<Fact>]
    let ``Fix resumable and non-resumable CE error ranges 3`` () =
        FSharp """
module Test
        
open System.Threading.Tasks
open System.Collections.Generic
open System.Linq

let f () : Task<IList<string>> = task {
    let! x = task { return 42 }

    let! y = task { return 43 }

    return Seq.empty.ToList()
}
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 193, Line 13, Col 12, Line 13, Col 30, "Type constraint mismatch. The type 
'TaskCode<List<'a>,List<'a>>' 
is not compatible with type
'TaskCode<IList<string>,IList<string>>' 
")
        ]
        
    [<Fact>]
    let ``Fix resumable and non-resumable CE error ranges 4`` () =
        FSharp """
module Test
        
open System.Threading.Tasks

let foo () : int64 = 6
let otherAsync () = async { return "lol"}
let fooAsync () : Async<int64> = async { 
    let! _ = otherAsync ()
    return 6 
}

let otherTask() = task { return "lol"}
let fooTask () : Task<int64> = task { 
        let! _ = otherTask()
        return 6 
    }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 193, Line 16, Col 16, Line 16, Col 17, "Type constraint mismatch. The type 
'TaskCode<int,int>' 
is not compatible with type
'TaskCode<int64,int64>' 
")
        ]