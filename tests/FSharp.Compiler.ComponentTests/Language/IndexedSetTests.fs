// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Language.IndexedSetTests

open FSharp.Test.Compiler
open Xunit

module Array =
    [<Fact>]
    let ``Dotless indexed set of parenthesized tuple compiles`` () =
        FSharp "
        let xs = Array.zeroCreate<int * int * int> 1
        xs[0] <- (1, 2, 3)
        "
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Dotless indexed set of unparenthesized tuple compiles`` () =
        FSharp "
        let xs = Array.zeroCreate<int * int * int> 1
        xs[0] <- 1, 2, 3
        "
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Dotless indexed set of double-parenthesized tuple compiles`` () =
        FSharp "
        let xs = Array.zeroCreate<int * int * int> 1
        xs[0] <- ((1, 2, 3))
        "
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Dot-indexed set of parenthesized tuple compiles`` () =
        FSharp "
        let xs = Array.zeroCreate<int * int * int> 1
        xs.[0] <- (1, 2, 3)
        "
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Dot-indexed set of unparenthesized tuple compiles`` () =
        FSharp "
        let xs = Array.zeroCreate<int * int * int> 1
        xs.[0] <- 1, 2, 3
        "
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Dot-indexed set of double-parenthesized tuple compiles`` () =
        FSharp "
        let xs = Array.zeroCreate<int * int * int> 1
        xs.[0] <- ((1, 2, 3))
        "
        |> typecheck
        |> shouldSucceed

module Dictionary =
    // Parsed as SynExpr.Set.
    [<Fact>]
    let ``Dotless indexed set of parenthesized tuple compiles`` () =
        FSharp "
        let xs = System.Collections.Generic.Dictionary<int, int * int * int> ()
        xs[0] <- (1, 2, 3)
        "
        |> typecheck
        |> shouldSucceed

    // Parsed as SynExpr.Set.
    [<Fact>]
    let ``Dotless indexed set of unparenthesized tuple compiles`` () =
        FSharp "
        let xs = System.Collections.Generic.Dictionary<int, int * int * int> ()
        xs[0] <- 1, 2, 3
        "
        |> typecheck
        |> shouldSucceed

    // Parsed as SynExpr.Set.
    [<Fact>]
    let ``Dotless indexed set of double-parenthesized tuple compiles`` () =
        FSharp "
        let xs = System.Collections.Generic.Dictionary<int, int * int * int> ()
        xs[0] <- ((1, 2, 3))
        "
        |> typecheck
        |> shouldSucceed

    // Parsed as SynExpr.DotIndexedSet.
    [<Fact>]
    let ``Dot-indexed set of parenthesized tuple compiles`` () =
        FSharp "
        let xs = System.Collections.Generic.Dictionary<int, int * int * int> ()
        xs.[0] <- (1, 2, 3)
        "
        |> typecheck
        |> shouldSucceed

    // Parsed as SynExpr.DotIndexedSet.
    [<Fact>]
    let ``Dot-indexed set of unparenthesized tuple compiles`` () =
        FSharp "
        let xs = System.Collections.Generic.Dictionary<int, int * int * int> ()
        xs.[0] <- 1, 2, 3
        "
        |> typecheck
        |> shouldSucceed

    // Parsed as SynExpr.DotIndexedSet.
    [<Fact>]
    let ``Dot-indexed set of double-parenthesized tuple compiles`` () =
        FSharp "
        let xs = System.Collections.Generic.Dictionary<int, int * int * int> ()
        xs.[0] <- ((1, 2, 3))
        "
        |> typecheck
        |> shouldSucceed

    // Parsed as SynExpr.NamedIndexedPropertySet.
    [<Fact>]
    let ``Named indexed property set of parenthesized tuple compiles`` () =
        FSharp "
        let xs = System.Collections.Generic.Dictionary<int, int * int * int> ()
        xs.Item 0 <- (1, 2, 3)
        "
        |> typecheck
        |> shouldSucceed

    // Parsed as SynExpr.NamedIndexedPropertySet.
    [<Fact>]
    let ``Named indexed property set of unparenthesized tuple compiles`` () =
        FSharp "
        let xs = System.Collections.Generic.Dictionary<int, int * int * int> ()
        xs.Item 0 <- 1, 2, 3
        "
        |> typecheck
        |> shouldSucceed

    // Parsed as SynExpr.NamedIndexedPropertySet.
    [<Fact>]
    let ``Named indexed property set of double-parenthesized tuple compiles`` () =
        FSharp "
        let xs = System.Collections.Generic.Dictionary<int, int * int * int> ()
        xs.Item 0 <- ((1, 2, 3))
        "
        |> typecheck
        |> shouldSucceed

    // Parsed as SynExpr.DotNamedIndexedPropertySet.
    [<Fact>]
    let ``Dot-named indexed property set of parenthesized tuple compiles`` () =
        FSharp "
        let xs = System.Collections.Generic.Dictionary<int, int * int * int> ()
        (xs).Item 0 <- (1, 2, 3)
        "
        |> typecheck
        |> shouldSucceed

    // Parsed as SynExpr.DotNamedIndexedPropertySet.
    [<Fact>]
    let ``Dot-named indexed property set of unparenthesized tuple compiles`` () =
        FSharp "
        let xs = System.Collections.Generic.Dictionary<int, int * int * int> ()
        (xs).Item 0 <- 1, 2, 3
        "
        |> typecheck
        |> shouldSucceed

    // Parsed as SynExpr.DotNamedIndexedPropertySet.
    [<Fact>]
    let ``Dot-named indexed property set of double-parenthesized tuple compiles`` () =
        FSharp "
        let xs = System.Collections.Generic.Dictionary<int, int * int * int> ()
        (xs).Item 0 <- ((1, 2, 3))
        "
        |> typecheck
        |> shouldSucceed
