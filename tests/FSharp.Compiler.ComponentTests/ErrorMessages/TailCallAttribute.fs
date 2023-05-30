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
        let r = (fact (n-1) (mul n acc))
        r + 23
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
    let ``Don't warn for valid tailcall`` () =
        """
let mul x y = x * y

[<TailCall>]
let rec fact n acc =
    if n = 0
    then acc
    else (fact (n-1) (mul n acc))
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
