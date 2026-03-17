// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

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
            (Error 3350, Line 11, Col 16, Line 11, Col 42, "Feature 'Allow let! and use! type annotations without requiring parentheses' is not available in F# 9.0. Please use language version 10.0 or greater.")
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
            (Error 3350, Line 16, Col 17, Line 16, Col 43, "Feature 'Allow let! and use! type annotations without requiring parentheses' is not available in F# 9.0. Please use language version 10.0 or greater.")
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
        |> withLangVersion10
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
        |> withLangVersion10
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
    [<InlineData("10.0","BindReturn")>]
    [<InlineData("10.0","WithoutBindReturn")>]
    [<InlineData("8.0","BindReturn")>]   
    [<InlineData("8.0","WithoutBindReturn")>]  
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
        
    [<Fact>]
    let ``Version 9.0: and! with type annotations requires parentheses`` () =
        FSharp """
module Test

type ParallelBuilder() =
    member _.Return(x) = async { return x }
    member _.ReturnFrom(computation: Async<'T>) = computation
    member _.Bind(computation: Async<'T>, binder: 'T -> Async<'U>) = 
        async {
            let! x = computation
            return! binder x
        }
    member _.Bind2(comp1: Async<'T1>, comp2: Async<'T2>, binder: 'T1 * 'T2 -> Async<'U>) =
        async {
            let! task1 = Async.StartChild comp1
            let! task2 = Async.StartChild comp2
            let! result1 = task1
            let! result2 = task2
            return! binder (result1, result2)
        }
    
    member _.Zero() = async.Zero()
    member _.Combine(comp1, comp2) = async.Combine(comp1, comp2)
    member _.Delay(f) = async.Delay(f)

let parallelCE = ParallelBuilder()

let testParallel() = 
    parallelCE {
        let! x = async { return 1 }
        and! y = async { return 2 }
        return x + y
    }
        
let testParallel2() = 
    parallelCE {
        let! (x: int) = async { return 1 }
        and! (y: int) = async { return 2 }
        return x + y
    }
    
let testParallel3() = 
    parallelCE {
        let! x: int = async { return 1 }
        and! y: int = async { return 2 }
        return x + y
    }
        """
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 43, Col 17, Line 43, Col 20, "Feature 'Allow let! and use! type annotations without requiring parentheses' is not available in F# 9.0. Please use language version 10.0 or greater.");
            (Error 3350, Line 44, Col 17, Line 44, Col 20, "Feature 'Allow let! and use! type annotations without requiring parentheses' is not available in F# 9.0. Please use language version 10.0 or greater.")
        ]
        
    [<Fact>]
    let ``Preview: and! with type annotations works without parentheses`` () =
        FSharp """
module Test

type ParallelBuilder() =
    member _.Return(x) = async { return x }
    member _.ReturnFrom(computation: Async<'T>) = computation
    member _.Bind(computation: Async<'T>, binder: 'T -> Async<'U>) = 
        async {
            let! x = computation
            return! binder x
        }
    member _.Bind2(comp1: Async<'T1>, comp2: Async<'T2>, binder: 'T1 * 'T2 -> Async<'U>) =
        async {
            let! task1 = Async.StartChild comp1
            let! task2 = Async.StartChild comp2
            let! result1 = task1
            let! result2 = task2
            return! binder (result1, result2)
        }
    
    member _.Zero() = async.Zero()
    member _.Combine(comp1, comp2) = async.Combine(comp1, comp2)
    member _.Delay(f) = async.Delay(f)

let parallelCE = ParallelBuilder()

let testParallel() = 
    parallelCE {
        let! x = async { return 1 }
        and! y = async { return 2 }
        return x + y
    }
    
let testParallel2() = 
    parallelCE {
        let! (x: int) = async { return 1 }
        and! (y: int) = async { return 2 }
        return x + y
    }
        
let testParallel3() = 
    parallelCE {
        let! x: int = async { return 1 }
        and! y: int = async { return 2 }
        return x + y
    }

let result = testParallel() |> Async.RunSynchronously
let result2 = testParallel2() |> Async.RunSynchronously
let result3 = testParallel3() |> Async.RunSynchronously
if result <> 3 then failwithf $"Expected 3, but got {result}"
if result2 <> 3 then failwithf $"Expected 3, but got {result2}"
if result3 <> 3 then failwithf $"Expected 3, but got {result3}"
        """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Version 9.0: and! with record patterns and type annotations requires parentheses`` () =
        FSharp """
module Test

type Person = { Name: string; Age: int }
type User = { Id: int; Username: string }

type ParallelBuilder() =
    member _.Return(x) = async { return x }
    member _.ReturnFrom(computation: Async<'T>) = computation
    member _.Bind(computation: Async<'T>, binder: 'T -> Async<'U>) = 
        async {
            let! x = computation
            return! binder x
        }
    member _.Bind2(comp1: Async<'T1>, comp2: Async<'T2>, binder: 'T1 * 'T2 -> Async<'U>) =
        async {
            let! task1 = Async.StartChild comp1
            let! task2 = Async.StartChild comp2
            let! result1 = task1
            let! result2 = task2
            return! binder (result1, result2)
        }
    
    member _.Zero() = async.Zero()
    member _.Combine(comp1, comp2) = async.Combine(comp1, comp2)
    member _.Delay(f) = async.Delay(f)

let parallelCE = ParallelBuilder()

let asyncPerson() = async { return { Name = "John"; Age = 30 } }
let asyncUser() = async { return { Id = 1; Username = "john_doe" } }

let testParallel1() = 
    parallelCE {
        let! ({ Name = name; Age = age }: Person) = asyncPerson()
        and! ({ Id = id; Username = username }: User) = asyncUser()
        return (name, age, id, username)
    }
    
let testParallel2() = 
    parallelCE {
        let! { Name = name; Age = age }: Person = asyncPerson()
        and! { Id = id; Username = username }: User = asyncUser()
        return (name, age, id, username)
    }
            """
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 42, Col 42, Line 42, Col 48, "Feature 'Allow let! and use! type annotations without requiring parentheses' is not available in F# 9.0. Please use language version 10.0 or greater.");
            (Error 3350, Line 43, Col 48, Line 43, Col 52, "Feature 'Allow let! and use! type annotations without requiring parentheses' is not available in F# 9.0. Please use language version 10.0 or greater.")
        ]

    [<Fact>]
    let ``Preview: and! with record patterns and type annotations works without parentheses`` () =
        FSharp """
module Test

type Person = { Name: string; Age: int }
type User = { Id: int; Username: string }

type ParallelBuilder() =
    member _.Return(x) = async { return x }
    member _.ReturnFrom(computation: Async<'T>) = computation
    member _.Bind(computation: Async<'T>, binder: 'T -> Async<'U>) = 
        async {
            let! x = computation
            return! binder x
        }
    member _.Bind2(comp1: Async<'T1>, comp2: Async<'T2>, binder: 'T1 * 'T2 -> Async<'U>) =
        async {
            let! task1 = Async.StartChild comp1
            let! task2 = Async.StartChild comp2
            let! result1 = task1
            let! result2 = task2
            return! binder (result1, result2)
        }
    
    member _.Zero() = async.Zero()
    member _.Combine(comp1, comp2) = async.Combine(comp1, comp2)
    member _.Delay(f) = async.Delay(f)

let parallelCE = ParallelBuilder()

let asyncPerson() = async { return { Name = "John"; Age = 30 } }
let asyncUser() = async { return { Id = 1; Username = "john_doe" } }

let testParallel1() = 
    parallelCE {
        let! ({ Name = name; Age = age }: Person) = asyncPerson()
        and! ({ Id = id; Username = username }: User) = asyncUser()
        return (name, age, id, username)
    }
    
let testParallel2() = 
    parallelCE {
        let! { Name = name; Age = age }: Person = asyncPerson()
        and! { Id = id; Username = username }: User = asyncUser()
        return (name, age, id, username)
    }

let result1 = testParallel1() |> Async.RunSynchronously
let result2 = testParallel2() |> Async.RunSynchronously

if result1 <> ("John", 30, 1, "john_doe") then 
    failwithf $"Expected ('John', 30, 1, 'john_doe'), but got %A{result1}"
if result2 <> ("John", 30, 1, "john_doe") then 
    failwithf $"Expected ('John', 30, 1, 'john_doe'), but got %A{result2}"
            """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Version 9.0: and! with union patterns and type annotations requires parentheses`` () =
        FSharp """
module Test

type MyOption<'T> = Some of 'T | None

type ParallelBuilder() =
    member _.Return(x) = async { return x }
    member _.ReturnFrom(computation: Async<'T>) = computation
    member _.Bind(computation: Async<'T>, binder: 'T -> Async<'U>) = 
        async {
            let! x = computation
            return! binder x
        }
    member _.Bind2(comp1: Async<'T1>, comp2: Async<'T2>, binder: 'T1 * 'T2 -> Async<'U>) =
        async {
            let! task1 = Async.StartChild comp1
            let! task2 = Async.StartChild comp2
            let! result1 = task1
            let! result2 = task2
            return! binder (result1, result2)
        }
    
    member _.Zero() = async.Zero()
    member _.Combine(comp1, comp2) = async.Combine(comp1, comp2)
    member _.Delay(f) = async.Delay(f)

let parallelCE = ParallelBuilder()

let asyncOption1() = async { return MyOption.Some 42 }
let asyncOption2() = async { return MyOption.Some "hello" }

let testParallel() = 
    parallelCE {
        let! (Some value1): MyOption<int> = asyncOption1()
        and! (Some value2): MyOption<string> = asyncOption2()
        return (value1, value2)
    }

let testParallel2() = 
    parallelCE {
        let! Some value1: MyOption<int> = asyncOption1()
        and! Some value2: MyOption<string> = asyncOption2()
        return (value1, value2)
    }
            """
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 34, Col 29, Line 34, Col 42, "Feature 'Allow let! and use! type annotations without requiring parentheses' is not available in F# 9.0. Please use language version 10.0 or greater.");
            (Error 3350, Line 35, Col 29, Line 35, Col 45, "Feature 'Allow let! and use! type annotations without requiring parentheses' is not available in F# 9.0. Please use language version 10.0 or greater.");
            (Error 3350, Line 41, Col 27, Line 41, Col 40, "Feature 'Allow let! and use! type annotations without requiring parentheses' is not available in F# 9.0. Please use language version 10.0 or greater.");
            (Error 3350, Line 42, Col 27, Line 42, Col 43, "Feature 'Allow let! and use! type annotations without requiring parentheses' is not available in F# 9.0. Please use language version 10.0 or greater.")
        ]

    [<Fact>]
    let ``Preview: and! with union patterns and type annotations works without parentheses`` () =
        FSharp """
module Test

type MyOption<'T> = Some of 'T | None

type ParallelBuilder() =
    member _.Return(x) = async { return x }
    member _.ReturnFrom(computation: Async<'T>) = computation
    member _.Bind(computation: Async<'T>, binder: 'T -> Async<'U>) = 
        async {
            let! x = computation
            return! binder x
        }
    member _.Bind2(comp1: Async<'T1>, comp2: Async<'T2>, binder: 'T1 * 'T2 -> Async<'U>) =
        async {
            let! task1 = Async.StartChild comp1
            let! task2 = Async.StartChild comp2
            let! result1 = task1
            let! result2 = task2
            return! binder (result1, result2)
        }
    
    member _.Zero() = async.Zero()
    member _.Combine(comp1, comp2) = async.Combine(comp1, comp2)
    member _.Delay(f) = async.Delay(f)

let parallelCE = ParallelBuilder()

let asyncOption1() = async { return MyOption.Some 42 }
let asyncOption2() = async { return MyOption.Some "hello" }

let testParallel1() = 
    parallelCE {
        let! (Some value1): MyOption<int> = asyncOption1()
        and! (Some value2): MyOption<string> = asyncOption2()
        return (value1, value2)
    }

let testParallel2() = 
    parallelCE {
        let! Some value1: MyOption<int> = asyncOption1()
        and! Some value2: MyOption<string> = asyncOption2()
        return (value1, value2)
    }

let result1 = testParallel1() |> Async.RunSynchronously
let result2 = testParallel2() |> Async.RunSynchronously

if result1 <> (42, "hello") then 
    failwithf $"Expected (42, 'hello'), but got %A{result1}"
if result2 <> (42, "hello") then 
    failwithf $"Expected (42, 'hello'), but got %A{result2}"
            """
        |> withLangVersionPreview
        |> asExe
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Preview: and! with mixed patterns and type annotations works`` () =
        FSharp """
module Test

type Person = { Name: string; Age: int }

type ParallelBuilder() =
    member _.Return(x) = async { return x }
    member _.ReturnFrom(computation: Async<'T>) = computation
    member _.Bind(computation: Async<'T>, binder: 'T -> Async<'U>) = 
        async {
            let! x = computation
            return! binder x
        }
    member _.Bind2(comp1: Async<'T1>, comp2: Async<'T2>, binder: 'T1 * 'T2 -> Async<'U>) =
        async {
            let! task1 = Async.StartChild comp1
            let! task2 = Async.StartChild comp2
            let! result1 = task1
            let! result2 = task2
            return! binder (result1, result2)
        }
    
    member _.Zero() = async.Zero()
    member _.Combine(comp1, comp2) = async.Combine(comp1, comp2)
    member _.Delay(f) = async.Delay(f)

let parallelCE = ParallelBuilder()

let asyncInt() = async { return 42 }
let asyncPerson() = async { return { Name = "Alice"; Age = 25 } }

// Test mixing different pattern types with type annotations
let testMixed() = 
    parallelCE {
        let! x: int = asyncInt()
        and! { Name = name; Age = age }: Person = asyncPerson()
        return (x, name, age)
    }

// Test with parentheses on one and not the other
let testMixed2() = 
    parallelCE {
        let! (y: int) = asyncInt()
        and! { Name = name2; Age = age2 }: Person = asyncPerson()
        return (y, name2, age2)
    }

let result1 = testMixed() |> Async.RunSynchronously
let result2 = testMixed2() |> Async.RunSynchronously

if result1 <> (42, "Alice", 25) then 
    failwithf $"Expected (42, 'Alice', 25), but got %A{result1}"
if result2 <> (42, "Alice", 25) then 
    failwithf $"Expected (42, 'Alice', 25), but got %A{result2}"
            """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Preview: multiple and! with type annotations works`` () =
        FSharp """
module Test

type Builder() =
    member _.Return(x) = async { return x }
    member _.Bind(computation: Async<'T>, binder: 'T -> Async<'U>) = 
        async {
            let! x = computation
            return! binder x
        }
    member _.Bind3(comp1: Async<'T1>, comp2: Async<'T2>, comp3: Async<'T3>, binder: 'T1 * 'T2 * 'T3 -> Async<'U>) =
        async {
            let! task1 = Async.StartChild comp1
            let! task2 = Async.StartChild comp2
            let! task3 = Async.StartChild comp3
            let! result1 = task1
            let! result2 = task2
            let! result3 = task3
            return! binder (result1, result2, result3)
        }
    
    member _.Zero() = async.Zero()
    member _.Combine(comp1, comp2) = async.Combine(comp1, comp2)
    member _.Delay(f) = async.Delay(f)

let builder = Builder()

let asyncInt() = async { return 42 }
let asyncString() = async { return "test" }
let asyncFloat() = async { return 3.14 }

// Test multiple and! with type annotations
let testMultiple() = 
    builder {
        let! x: int = asyncInt()
        and! y: string = asyncString()
        and! z: float = asyncFloat()
        return (x, y, z)
    }

let result = testMultiple() |> Async.RunSynchronously

if result <> (42, "test", 3.14) then 
    failwithf $"Expected (42, 'test', 3.14), but got %A{result}"
            """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Preview: and! with generic type annotations works`` () =
        FSharp """
module Test

type Result<'T, 'E> = Ok of 'T | Error of 'E

type ResultBuilder() =
    member _.Return(x) = Ok x
    member _.Bind(m: Result<'T, 'E>, f: 'T -> Result<'U, 'E>) = 
        match m with
        | Ok x -> f x
        | Error e -> Error e
    member _.Bind2(m1: Result<'T1, 'E>, m2: Result<'T2, 'E>, f: 'T1 * 'T2 -> Result<'U, 'E>) =
        match m1, m2 with
        | Ok x1, Ok x2 -> f (x1, x2)
        | Error e, _ -> Error e
        | _, Error e -> Error e

let result = ResultBuilder()

let getValue1() = Ok 42
let getValue2() = Ok "success"

let test() = 
    result {
        let! x: int = getValue1()
        and! y: string = getValue2()
        return (x, y)
    }

match test() with
| Ok (42, "success") -> printfn "Test passed"
| _ -> failwith "Test failed"
            """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Preview: use! unresolved return type`` () =
        FSharp """
module Test

open System.IO
open System.Threading.Tasks

task {
    use! x: IDisposable = Task.FromResult(new StreamReader(""))
    ()
}
|> ignore
            """
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            Error 39, Line 8, Col 13, Line 8, Col 24, "The type 'IDisposable' is not defined."
        ]

    [<Fact>]
    let ``Preview: use! return type mismatch error 01`` () =
        FSharp """
module Test

open System

task {
    use! (x: int): IDisposable = failwith ""
    ()
}
|> ignore
            """
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            Error 1, Line 7, Col 11, Line 7, Col 17, "This expression was expected to have type
'IDisposable'   
but here has type
'int'   "
        ]

    [<Fact>]
    let ``Preview: let! return type mismatch error 01`` () =
        FSharp """
module Test

open System.Threading.Tasks

task {
    let! x: string = Task.FromResult(1)
    ()
}
|> ignore
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            // x: string
            Error 1, Line 7, Col 10, Line 7, Col 19, "This expression was expected to have type
    'int'   
but here has type
    'string'    "
        ]

    [<Fact>]
    let ``Preview: let! return type mismatch error 02`` () =
        FSharp """
module Test

open System.Threading.Tasks

task {
    let! (x: string): int = Task.FromResult(1)
    ()
}
|> ignore
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            // x: string
            Error 1, Line 7, Col 11, Line 7, Col 20, "This expression was expected to have type
    'int'   
but here has type
    'string'    "
        ]

    [<Fact>]
    let ``Preview: let! return type mismatch error 03`` () =
        FSharp """
module Test

open System.Threading.Tasks

task {
    let! (x: string): int = Task.FromResult("")
    ()
}
|> ignore
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            // (x: string): int
            Error 1, Line 7, Col 10, Line 7, Col 26, "This expression was expected to have type
    'string'   
but here has type
    'int'    "
        ]

    [<Fact>]
    let ``Preview: let!-and! return type mismatch error 01`` () =
        FSharp """
module Test

type MyBuilder() =
    member _.Return(x: int): Result<int, exn> = failwith ""
    member _.Bind(m: Result<int, exn>, f: int -> Result<int, exn>): Result<int, exn> = failwith ""
    member _.Bind2(m1: Result<int, exn>, m2: Result<int, exn>, f: int * int -> Result<int, exn>): Result<int, exn> = failwith ""

let builder = MyBuilder()

builder {
    let! x: int = Ok 1
    and! y: string = Ok 2
    return 0
}
|> ignore
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            // y: string
            Error 1, Line 13, Col 10, Line 13, Col 19, "This expression was expected to have type
'int'   
but here has type
'string'    "
            // x: int
            Warning 25, Line 12, Col 10, Line 12, Col 16, "Incomplete pattern matches on this expression."
        ]

    [<Fact>]
    let ``Preview: let!-and! return type mismatch error 02`` () =
        FSharp """
module Test

type MyBuilder() =
    member _.Return(x: int): Result<int, exn> = failwith ""
    member _.Bind(m: Result<int, exn>, f: int -> Result<int, exn>): Result<int, exn> = failwith ""
    member _.Bind2(m1: Result<int, exn>, m2: Result<int, exn>, f: int * int -> Result<int, exn>): Result<int, exn> = failwith ""

let builder = MyBuilder()

builder {
    let! x: int = Ok 1
    and! (y: string): int = Ok 2
    return 0
}
|> ignore
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            // y: string
            Error 1, Line 13, Col 11, Line 13, Col 20, "This expression was expected to have type
'int'   
but here has type
'string'    "
        ]

    [<Fact>]
    let ``Preview: let!-and! return type mismatch error 03`` () =
        FSharp """
module Test

type MyBuilder() =
    member _.Return(x: int): Result<int, exn> = failwith ""
    member _.Bind(m: Result<int, exn>, f: int -> Result<int, exn>): Result<int, exn> = failwith ""
    member _.Bind2(m1: Result<int, exn>, m2: Result<int, exn>, f: int * int -> Result<int, exn>): Result<int, exn> = failwith ""

let builder = MyBuilder()

builder {
    let! x: int = Ok 1
    and! (y: int): string = Ok 1
    return 0
}
|> ignore
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            // (y: int): string
            Error 1, Line 13, Col 10, Line 13, Col 26, "This expression was expected to have type
'int'   
but here has type
'string'    "
            // y: int
            Error 1, Line 13, Col 11, Line 13, Col 17, "This expression was expected to have type
'string'    
but here has type
'int'   "
        ]

    [<Theory; FileInlineData("tailcalls.fsx")>]
    let ``tail call methods work`` compilation =
        compilation
         |> getCompilation 
         |> asFsx
         |> runFsi
         |> shouldSucceed

    [<Theory; FileInlineData("coroutines.fsx")>]
    let ``YieldFromFinal works in coroutines`` compilation =
        compilation
         |> getCompilation 
         |> asFsx
         |> runFsi
         |> shouldSucceed

    let queryFS1182NoWarnCases =
        [
            "let result = query { for x in [1;2;3] do where (x > 0); select 1 }"
            "let result = query { for x in [1;2;3] do where (x > 2); select x }"
            "let result = query { for x in [1;2;3] do let y = x * 2 in select y }"
            """let data1 = [1;2;3]
let data2 = [(1, "one"); (2, "two"); (3, "three")]
let result = query { for x in data1 do join (y, name) in data2 on (x = y); select name }"""
            "let result = query { for a in [1;2;3] do for b in [4;5;6] do where (a < b); select (a + b) }"
            "let result = query { for x in [3;1;2] do sortBy x; select x }"
        ]
        |> List.map (fun s -> [| box s |])

    [<Theory>]
    [<MemberData(nameof queryFS1182NoWarnCases)>]
    let ``Query variable used in expression does not trigger FS1182`` (code: string) =
        FSharp $"module Test\n{code}"
        |> withWarnOn 1182
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    let queryFS1182WarnCases =
        [
            "let result = query { for x in [1;2;3] do select 1 }",
                2, 26, 2, 27, "The value 'x' is unused"
            """let result = 
    query { for x in [1;2;3] do
            select (
                let unused = 42
                x) }""",
                5, 21, 5, 27, "The value 'unused' is unused"
            """let f () =
    let unused = 42
    query { for x in [1;2;3] do select x }""",
                3, 9, 3, 15, "The value 'unused' is unused"
            """let result =
    query { for x in [1;2;3] do
            select (
                let x = 42
                x) }""",
                3, 17, 3, 18, "The value 'x' is unused"
            """let result =
    query { for x in [1;2;3] do
            select (
                let x = x
                1) }""",
                5, 21, 5, 22, "The value 'x' is unused"
        ]
        |> List.map (fun (str, line1, col1, line2, col2, msg) ->
            [| box str; box line1; box col1; box line2; box col2; box msg |])

    [<Theory>]
    [<MemberData(nameof queryFS1182WarnCases)>]
    let ``Unused variable in query warns FS1182`` (code: string, line1: int, col1: int, line2: int, col2: int, msg: string) =
        FSharp $"module Test\n{code}"
        |> withWarnOn 1182
        |> asLibrary
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 1182, Line line1, Col col1, Line line2, Col col2, msg)
        |> ignore
