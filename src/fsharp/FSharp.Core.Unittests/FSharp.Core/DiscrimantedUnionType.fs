// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Core

open System
open System.Numerics 
open FSharp.Core.Unittests.LibraryTestFx
open NUnit.Framework

type EnumUnion = 
| A
| B

[<TestFixture>]
type UseUnionsAsEnums() =
    [<Test>]
    member this.CanCompare() =        
        Assert.AreEqual(EnumUnion.B, EnumUnion.B)
        Assert.AreNotEqual(EnumUnion.A, EnumUnion.B)

[<Flags>]
type FlagsUnion = 
| One = 1
| Two = 2
| Four = 4

[<TestFixture>]
type UseUnionsAsFlags() =
    [<Test>]
    member this.CanCompareWithInts() =
        Assert.AreEqual(int FlagsUnion.One, 1)
        Assert.AreEqual(int FlagsUnion.Two, 2)
        Assert.AreEqual(int FlagsUnion.Four, 4)

    [<Test>]
    member this.CanUseBinaryOr() =
        Assert.AreEqual(int (FlagsUnion.One ||| FlagsUnion.Two), 3)

    [<Test>]
    member this.CanCompareWithFlags() =
        Assert.AreEqual(FlagsUnion.Two, FlagsUnion.Two)
        Assert.AreNotEqual(FlagsUnion.Two, FlagsUnion.One)

type UnionsWithData = 
| Alpha of int
| Beta of string * float

[<TestFixture>]
type UseUnionsWithData() =
    let a1 = Alpha 1
    let a2 = Alpha 2
    let b1 = Beta("win",8.1)

    [<Test>]
    member this.CanAccessTheData() =
        match a1 with
        | Alpha 1 -> ()
        | _ -> Assert.Fail()

        match a2 with
        | Alpha 2 -> ()
        | _ -> Assert.Fail()

        match a2 with
        | Alpha x -> Assert.AreEqual(x,2)
        | _ -> Assert.Fail()

        match b1 with
        | Beta ("win",8.1) -> ()
        | _ -> Assert.Fail()
        
        match b1 with
        | Beta (x,y) -> 
            Assert.AreEqual(x,"win")
            Assert.AreEqual(y,8.1)
        | _ -> Assert.Fail()

    [<Test>]
    member this.CanAccessTheDataInGuards() =
        match a1 with
        | Alpha x when x = 1 -> ()
        | _ -> Assert.Fail()

        match a2 with
        | Alpha  x when x = 2 -> ()
        | _ -> Assert.Fail()