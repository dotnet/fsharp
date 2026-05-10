// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open FSharp.Test.Compiler
open Xunit

module StaticMethodResolution =
    
    // So that the compiler doesn't treat an extension method as intrinsic
    // we place a method to another module.
    [<Fact>]
    let ``Extension static method is resolved correctly when one or many intrinsic candidates are found``() =
        Fsx """
module Extensions =

    type StaticGeneric<'T>() =
        static member Bar() = ()

    [<AutoOpen>]
    module StaticGenericExtensions =
        type StaticGeneric<'T> with
            static member Bar(_: int) = ()

module Program =
    open Extensions
    
    StaticGeneric.Bar(42) // StaticGeneric is just an ident
    StaticGeneric<int>.Bar(42) // StaticGeneric<int> is an expression
        """
        |> withOptions ["--nowarn:1125"]
        |> typecheck
        |> shouldSucceed