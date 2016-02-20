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


let [<Test>] ``pattern matching on struct records`` () =
    Check.QuickThrowOnFailure <|
    fun (i1:int) (i2:int) ->
        let sr1 = { C = i1; D = i2 }
        (match sr1 with
        | { C = c; D = d } when c = i1 && d = i2 -> true
        | _ -> false) 
        |@ "with pattern match on struct record" .&.
        (sr1 |> function 
        | { C = c; D = d } when c = i1 && d = i2 -> true
        | _ -> false)
        |@ "function pattern match on struct record"


let [<Test>] ``let binds using struct records`` () =
    Check.QuickThrowOnFailure <|
    fun (i1:int) (i2:int) ->
        let sr1 = { C = i1; D = i2 }
        let { C = c1; D = d2 } as sr2 = sr1
        (sr1 = sr2)          |@ "sr1 = sr2" .&.
        (c1 = i1 && d2 = i2) |@ "c1 = i1 && d2 = i2"


let [<Test>] ``function argument bindings using struct records`` () =
    Check.QuickThrowOnFailure <|
    fun (i1:int) (i2:int) ->
        let sr1 = { C = i1; D = i2 }
        let test sr1 ({ C = c1; D = d2 } as sr2) =
            sr1 = sr2 && c1 = i1 && d2 = i2
        test sr1 sr1      
        
        
[<Struct>]
type MutableStructRecord =
    {   mutable M1: int
        mutable M2: int
    }                  
    
let [<Test>] ``can mutate struct record fields`` () =
    Check.QuickThrowOnFailure <|
    fun (i1:int) (i2:int) (m1:int) (m2:int) ->
        (i1 <> m1 && i2 <> m2) ==>
            let mutable sr1 = { M1 = i1; M2 = i2}
            sr1.M1 <- m1
            sr1.M2 <- m2
            sr1.M1 = m1 && sr1.M2 = m2


[<Struct>]
[<CustomComparison; CustomEquality>]
type ComparisonStructRecord =
    {   C1 :int
        C2: int
    }
    override self.Equals other =
        match other with
        | :? ComparisonStructRecord as o ->  (self.C1 + self.C2) = (o.C1 + o.C2)
        | _ -> false

    override self.GetHashCode() = hash self
    interface IComparable with
        member self.CompareTo other =
            match other with
            | :? ComparisonStructRecord as o -> compare (self.C1 + self.C2) (o.C1 + o.C2)
            | _ -> invalidArg "other" "cannot compare values of different types"


let [<Test>] ``struct records support [CustomEquality>]`` () =
    Check.QuickThrowOnFailure <|
    fun (i1:int) (i2:int) ->
        let sr1 = { C1 = i1; C2 = i2 }
        let sr2 = { C1 = i1; C2 = i2 }
        (sr1.Equals sr2)      


let [<Test>] ``struct records support [<CustomComparison>]`` () =
    Check.QuickThrowOnFailure <|
    fun (i1:int) (i2:int) (k1:int) (k2:int) ->        
        let sr1 = { C1 = i1; C2 = i2 }
        let sr2 = { C1 = k1; C2 = k2 }
        if   sr1 > sr2 then compare sr1 sr2 = 1
        elif sr1 < sr2 then compare sr1 sr2 = -1
        elif sr1 = sr2 then compare sr1 sr2 = 0
        else false
   