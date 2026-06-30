// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module ``UncheckedDefaultofOptimization`` =

    [<Fact>]
    let ``Unused Unchecked.defaultof bindings of concrete types are eliminated`` () =
        FSharp """
module Test
open System
let f (n: float32) =
    Console.WriteLine n
    let _ = Unchecked.defaultof<decimal>
    let _ = Unchecked.defaultof<decimal>
    let _ = Unchecked.defaultof<decimal>
    let n' = n * 2.f
    Console.WriteLine n'
        """
        |> withOptimize
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent ["initobj [runtime]System.Decimal"]

    [<Fact>]
    let ``Unused Unchecked.defaultof bindings of unsolved generic types do not cause FS0073`` () =
        // Regression for SRTP witness/dummy-argument patterns (e.g. FSharpPlus) where
        // eliminating an ilzero binding referencing unsolved typars trips FS0073 in IlxGen.
        FSharp """
module Test
let inline witness () = Unchecked.defaultof<'T>
let inline run< ^T when ^T: (static member Zero: ^T)> () =
    let _ = (witness () : ^T)
    LanguagePrimitives.GenericZero< ^T>
let v : int = run< int> ()
        """
        |> withOptimize
        |> asLibrary
        |> compile
        |> shouldSucceed
