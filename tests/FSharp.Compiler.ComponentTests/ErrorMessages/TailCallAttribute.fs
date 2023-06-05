namespace FSharp.Compiler.ComponentTests.ErrorMessages

open FSharp.Test.Compiler
open FSharp.Test.Compiler.Assertions.StructuredResultsAsserts
open Xunit

module ``TailCall Attribute`` =

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully in if-else`` () =
        """
let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    if n = 0
    then acc
    else (fact (n-1) (mul n acc)) + 23
        """
        |> FSharp
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 8
                        StartColumn = 11
                        EndLine = 8
                        EndColumn = 33 }
              Message =
               "The member or function 'fact' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3567
              Range = { StartLine = 8
                        StartColumn = 11
                        EndLine = 8
                        EndColumn = 15 }
              Message =
               "The member or function 'fact' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully in match clause`` () =
        """
let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    match n with
    | 0 -> acc
    | _ -> (fact (n-1) (mul n acc)) + 23
        """
        |> FSharp
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 8
                        StartColumn = 13
                        EndLine = 8
                        EndColumn = 35 }
              Message =
               "The member or function 'fact' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3567
              Range = { StartLine = 8
                        StartColumn = 13
                        EndLine = 8
                        EndColumn = 17 }
              Message =
               "The member or function 'fact' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]
    
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for rec call in binding`` () =
        """
let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    match n with
    | 0 -> acc
    | _ ->
        let r = fact (n-1) (mul n acc)
        r + 23
        """
        |> FSharp
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 9
                        StartColumn = 17
                        EndLine = 9
                        EndColumn = 21 }
              Message =
               "The member or function 'fact' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcall and bind from toplevel`` () =
        """
let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    if n = 0
    then acc
    else fact (n-1) (mul n acc)
    
let r = fact 100000 1
r |> ignore
        """
        |> FSharp
        |> typecheck
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for mutually recursive functions`` () =
        """
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
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 13
                        StartColumn = 9
                        EndLine = 13
                        EndColumn = 20 }
              Message =
               "The member or function 'bar' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3567
              Range = { StartLine = 13
                        StartColumn = 9
                        EndLine = 13
                        EndColumn = 12 }
              Message =
               "The member or function 'bar' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for invalid tailcall in type method`` () =
        """
type C () =
    [<TailCall>]
    member this.M1() = this.M1() + 1
        """
        |> FSharp
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 4
                        StartColumn = 24
                        EndLine = 4
                        EndColumn = 33 }
              Message =
               "The member or function 'M1' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcall in type method`` () =
        """
type C () =
    [<TailCall>]
    member this.M1() = this.M1()
        """
        |> FSharp
        |> typecheck
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Don't warn for valid tailcalls in type methods`` () =
        """
type C () =
    [<TailCall>]
    member this.M1() =
        this.M2()    // ok

    [<TailCall>]
    member this.M2() =
        this.M1()     // ok
        """
        |> FSharp
        |> typecheck
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully for invalid tailcalls in type methods`` () =
        """
type F () =
    [<TailCall>]
    member this.M1() =
        this.M2() + 1   // should warn

    [<TailCall>]
    member this.M2() =
        this.M1() + 2    // should warn
        """
        |> FSharp
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 3567
              Range = { StartLine = 5
                        StartColumn = 9
                        EndLine = 5
                        EndColumn = 18 }
              Message =
               "The member or function 'M2' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
            { Error = Warning 3567
              Range = { StartLine = 9
                        StartColumn = 9
                        EndLine = 9
                        EndColumn = 18 }
              Message =
               "The member or function 'M1' has the 'TailCallAttribute' attribute, but is not being used in a tail recursive way." }
        ]
