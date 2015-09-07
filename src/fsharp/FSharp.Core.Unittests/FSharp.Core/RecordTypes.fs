// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Core

open System
open System.Numerics
open FSharp.Core.Unittests.LibraryTestFx
open NUnit.Framework

type Record =
    {
        A: float
        B: float
    }

[<TestFixture>]
type UseRecord() = 
    [<Test>]
    member this.CanCompare() = 
        let r1 = { A = 0.; B = 12. }
        let r2 = { A = 0.; B = 12. }
        Assert.AreEqual(r1, r2)
        Assert.AreNotEqual({ r1 with A = 1.}, r2)
        Assert.IsTrue((r1 = r2))
        Assert.IsTrue(r1.Equals r2)

[<Struct>]
type StructRecord =
    {
        C: float
        D: float
    }

[<TestFixture>]
type UseStructRecord() = 
    [<Test>]
    member this.CanCompare() = 
        let r1 = { C = 0.; D = 12. }
        let r2 = { C = 0.; D = 12. }
        Assert.AreEqual(r1, r2)
        Assert.AreNotEqual({ r1 with C = 1.}, r2)
        Assert.IsTrue((r1 = r2))
        Assert.IsTrue(r1.Equals r2)
