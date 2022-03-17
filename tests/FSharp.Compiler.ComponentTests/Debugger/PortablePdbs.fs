// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Debugger

open Xunit
open FSharp.Test.Compiler

module PortablePdbs =

    [<Fact>]
    let ``Valid Portable PDBs are produced by compiler`` () =
        FSharp """
open System

module Foo =
    let getcwd () = Environment.CurrentDirectory

[<EntryPoint>]
let main _ =
    printfn "%s" Environment.CurrentDirectory
    0
        """
        |> asExe
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb

    
