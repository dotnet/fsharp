// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ComponentTests.Miscellaneous.FindUnsolvedTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``fallbackRange being set in FindUnsolved`` () =    
    FSharp
        """let f<'b> () : 'b = (let a = failwith "" in unbox a)"""
    |> typecheckResults
    |> ignore
