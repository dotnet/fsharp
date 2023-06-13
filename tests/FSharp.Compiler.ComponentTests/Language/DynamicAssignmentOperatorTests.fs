// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open System
open Xunit
open FSharp.Test.Compiler

module DynamicAssignmentOperatorTests =

    [<Theory>]
    [<InlineData("6.0")>]
    [<InlineData("7.0")>]
    let ``Implementing dynamic assignment operator does not produce a warning`` version = 
        Fsx """
        type T = T with
            static member inline (?<-) (f, x, y) = f x y
        """
        |> withLangVersion version
        |> compile
        |> shouldSucceed