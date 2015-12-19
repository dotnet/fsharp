// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Core

open System
open System.Numerics
open FSharp.Core.Unittests.LibraryTestFx
open NUnit.Framework

type EnumUnion = 
    | A
    | B

[<Parallelizable(ParallelScope.Self)>][<TestFixture>]
type UseUnionsAsEnums() = 
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.CanCompare() = 
        Assert.AreEqual(EnumUnion.B, EnumUnion.B)
        Assert.AreNotEqual(EnumUnion.A, EnumUnion.B)

[<Flags>]
type FlagsUnion = 
    | One = 1
    | Two = 2
    | Four = 4

[<Parallelizable(ParallelScope.Self)>][<TestFixture>]
type UseUnionsAsFlags() = 
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.CanCompareWithInts() = 
        Assert.AreEqual(int FlagsUnion.One, 1)
        Assert.AreEqual(int FlagsUnion.Two, 2)
        Assert.AreEqual(int FlagsUnion.Four, 4)
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.CanCastFromInts() = 
        let four : FlagsUnion = enum 4
        Assert.AreEqual(four, FlagsUnion.Four)
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.CanCreateValuesWithoutName() = 
        let unknown : FlagsUnion = enum 99 // strange, but valid
        Assert.AreEqual(int unknown, 99)
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.CanParseViaBCL() = 
        let values = System.Enum.GetValues(typeof<FlagsUnion>)
        let fourFromString = System.Enum.Parse(typeof<FlagsUnion>, "Four", false) :?> FlagsUnion // downcast needed
        Assert.AreEqual(fourFromString, FlagsUnion.Four)
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.CanUseBinaryOr() = 
        Assert.AreEqual(int (FlagsUnion.One ||| FlagsUnion.Two), 3)
        Assert.AreEqual(int (FlagsUnion.One ||| FlagsUnion.One), 1)
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.CanCompareWithFlags() = 
        Assert.AreEqual(FlagsUnion.Two, FlagsUnion.Two)
        Assert.AreNotEqual(FlagsUnion.Two, FlagsUnion.One)

type UnionsWithData = 
    | Alpha of int
    | Beta of string * float

[<Parallelizable(ParallelScope.Self)>][<TestFixture>]
type UseUnionsWithData() = 
    let a1 = Alpha 1
    let a2 = Alpha 2
    let b1 = Beta("win", 8.1)
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.CanAccessTheData() = 
        match a1 with
        | Alpha 1 -> ()
        | _ -> Assert.Fail()
        match a2 with
        | Alpha 2 -> ()
        | _ -> Assert.Fail()
        match a2 with
        | Alpha x -> Assert.AreEqual(x, 2)
        | _ -> Assert.Fail()
        match b1 with
        | Beta("win", 8.1) -> ()
        | _ -> Assert.Fail()
        match b1 with
        | Beta(x, y) -> 
            Assert.AreEqual(x, "win")
            Assert.AreEqual(y, 8.1)
        | _ -> Assert.Fail()
    
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member this.CanAccessTheDataInGuards() = 
        match a1 with
        | Alpha x when x = 1 -> ()
        | _ -> Assert.Fail()
        match a2 with
        | Alpha x when x = 2 -> ()
        | _ -> Assert.Fail()