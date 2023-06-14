namespace FSharp.Compiler.ComponentTests.ErrorMessages

open FSharp.Test.Compiler
open FSharp.Test.Compiler.Assertions.StructuredResultsAsserts

module ``TailCall Attribute`` =

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully in if-else`` () =
        """
module M

let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    if n = 0
    then acc
    else (fact (n - 1) (mul n acc)) + 23
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 10
                        StartColumn = 11
                        EndLine = 10
                        EndColumn = 35 }
              Message =
               "The member or function 'fact' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully in match clause`` () =
        """
module M

let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    match n with
    | 0 -> acc
    | _ -> (fact (n - 1) (mul n acc)) + 23
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 10
                        StartColumn = 13
                        EndLine = 10
                        EndColumn = 37 }
              Message =
               "The member or function 'fact' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for rec call in binding`` () =
        """
module M

let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    match n with
    | 0 -> acc
    | _ ->
        let r = fact (n - 1) (mul n acc)
        r + 23
        """
        |> FSharp
        |> withLangVersionPreview        
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 11
                        StartColumn = 17
                        EndLine = 11
                        EndColumn = 41 }
              Message =
               "The member or function 'fact' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcall and bind from toplevel`` () =
        """
module M

let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    if n = 0
    then acc
    else
        printfn "%A" n
        fact (n - 1) (mul n acc)
    
let r = fact 100000 1
r |> ignore
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for mutually recursive functions`` () =
        """
module M

let foo x =
    printfn "Foo: %x" x

[<TailCall>]
let rec bar x =
    match x with
    | 0 ->
        foo x           // OK: non-tail-recursive call to a function which doesn't share the current stack frame (i.e., 'bar' or 'baz').
        printfn "Zero"
        
    | 1 ->
        bar (x - 1)     // Warning: this call is not tail-recursive
        printfn "Uno"
        baz x           // OK: tail-recursive call.

    | x ->
        printfn "0x%08x" x
        bar (x - 1)     // OK: tail-recursive call.
        
and [<TailCall>] baz x =
    printfn "Baz!"
    bar (x - 1)         // OK: tail-recursive call.
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 15
                        StartColumn = 9
                        EndLine = 15
                        EndColumn = 20 }
              Message =
               "The member or function 'bar' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for invalid tailcall in type method`` () =
        """
module M

type C () =
    [<TailCall>]
    member this.M1() = this.M1() + 1
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 6
                        StartColumn = 24
                        EndLine = 6
                        EndColumn = 33 }
              Message =
               "The member or function 'M1' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcall in type method`` () =
        """
module M

type C () =
    [<TailCall>]
    member this.M1() =
        printfn "M1 called"
        this.M1()

let c = C()
c.M1()
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in type methods`` () =
        """
module M

type C () =
    [<TailCall>]
    member this.M1() =
        printfn "M1 called"
        this.M2()    // ok

    [<TailCall>]
    member this.M2() =
        printfn "M2 called"
        this.M1()     // ok
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for invalid tailcalls in type methods`` () =
        """
module M

type F () =
    [<TailCall>]
    member this.M1() =
        printfn "M1 called"
        this.M2() + 1   // should warn

    [<TailCall>]
    member this.M2() =
        printfn "M2 called"
        this.M1() + 2    // should warn
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 8
                        StartColumn = 9
                        EndLine = 8
                        EndColumn = 18 }
              Message =
               "The member or function 'M2' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3567
              Range = { StartLine = 13
                        StartColumn = 9
                        EndLine = 13
                        EndColumn = 18 }
              Message =
               "The member or function 'M2' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcall and bind from nested bind`` () =
        """
module M

let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    if n = 0
    then acc
    else
        printfn "%A" n
        fact (n - 1) (mul n acc)
    
let f () =
    let r = fact 100000 1
    r |> ignore
    
fact 100000 1 |> ignore
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid tailcalls in seq expression because of bind`` () =
        """
module M

[<TailCall>]
let rec f x : seq<int> =
    seq {
        let r = f (x - 1)
        let r2 = Seq.map (fun x -> x + 1) r
        yield! r2
}
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 7
                        StartColumn = 17
                        EndLine = 7
                        EndColumn = 26 }
              Message =
               "The member or function 'f' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid tailcalls in seq expression because of pipe`` () =
        """
module M

[<TailCall>]
let rec f x : seq<int> =
    seq {
        yield! f (x - 1) |> Seq.map (fun x -> x + 1)
}
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 7
                        StartColumn = 16
                        EndLine = 7
                        EndColumn = 25 }
              Message =
               "The member or function 'f' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in seq expression`` () =
        """
module M

[<TailCall>]
let rec f x = seq {
    let y = x - 1
    let z = y - 1
    yield! f (z - 1)
}

let a: seq<int> = f 10
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in async expression`` () =
        """
module M

[<TailCall>] 
let rec f x = async {
    let y = x - 1
    let z = y - 1
    return! f (z - 1)
}

let a: Async<int> = f 10
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid tailcalls in async expression`` () =
        """
module M

[<TailCall>] 
let rec f x = async { 
    let! r = f (x - 1)
    return r
}
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 6
                        StartColumn = 14
                        EndLine = 6
                        EndColumn = 23 }
              Message =
               "The member or function 'f' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]
        
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in rec module`` () =
        """
module M

module rec M =

    module M1 =
        [<TailCall>]
        let m1func() = M2.m2func()

    module M2 =
        [<TailCall>]
        let m2func() = M1.m1func()
        
    let f () =
        M1.m1func() |> ignore

M.M1.m1func() |> ignore
M.M2.m2func()
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for invalid tailcalls in rec module`` () =
        """
module M

module rec M =

    module M1 =
        [<TailCall>]
        let m1func() = 1 + M2.m2func()

    module M2 =
        [<TailCall>]
        let m2func() = 2 + M1.m1func()
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 8
                        StartColumn = 28
                        EndLine = 8
                        EndColumn = 39 }
              Message =
               "The member or function 'm2func' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3567
              Range = { StartLine = 12
                        StartColumn = 28
                        EndLine = 12
                        EndColumn = 39 }
              Message =
               "The member or function 'm2func' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]
        
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn for byref parameters`` () =
        """
module M

[<TailCall>]
let rec foo(x: int byref) = foo(&x)
let run() = let mutable x = 0 in foo(&x)
        """
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 6
                        StartColumn = 34
                        EndLine = 6
                        EndColumn = 41 }
              Message =
               "The member or function 'foo' has the 'TailCall' attribute, but is not being used in a tail recursive way." }
        ]
