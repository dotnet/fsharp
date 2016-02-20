// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
module FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Core.RecordTypes

open System
open System.Numerics
open FSharp.Core.Unittests.LibraryTestFx
open NUnit.Framework
open FsCheck
open FsCheck.PropOperators

type Record =
    {   A: int
        B: int
    }


let [<Test>] ``can compare records`` () = 
    Check.QuickThrowOnFailure <|
    fun (i1:int) (i2:int) ->
        i1 <> i2 ==>
            let r1 = { A = i1; B = i2 }
            let r2 = { A = i1; B = i2 }
            (r1 = r2)                   |@ "r1 = r2" .&.
            ({ r1 with A = r1.B} <> r2) |@ "{r1 with A = r1.B} <> r2" .&.
            (r1.Equals r2)              |@ "r1.Equals r2"

[<Struct>]
type StructRecord =
    {   C: int
        D: int
    }

let [<Test>] ``can compare struct records`` () =
    Check.QuickThrowOnFailure <|
    fun (i1:int) (i2:int) ->
        i1 <> i2 ==>
            let sr1 = { C = i1; D = i2 }
            let sr2 = { C = i1; D = i2 }
            (sr1 = sr2)                    |@ "sr1 = sr2" .&.
            ({ sr1 with C = sr1.D} <> sr2) |@ "{sr1 with C = sr1.D} <> sr2" .&.
            (sr1.Equals sr2)               |@ "sr1.Equals sr2"

