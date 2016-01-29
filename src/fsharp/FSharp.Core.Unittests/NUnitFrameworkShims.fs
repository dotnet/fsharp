// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace NUnit.Framework
open System

type TestFixtureAttribute() =
    inherit System.Attribute()

type TestAttribute() =
    inherit System.Attribute()

type SetUpAttribute() =
    inherit System.Attribute()

type TearDownAttribute() =
    inherit System.Attribute()

exception AssertionException of string

module private Impl =
    let rec equals (expected:obj) (actual:obj) = 
        match expected, actual with 
        |   (:? Array as a1), (:? Array as a2) ->
                if a1.Rank > 1 then failwith "Rank > 1 not supported"                
                if a2.Rank > 1 then false
                else
                    let lb = a1.GetLowerBound(0)
                    let ub = a1.GetUpperBound(0)
                    if lb <> a2.GetLowerBound(0) || ub <> a2.GetUpperBound(0) then false
                    else
                        {lb..ub} |> Seq.forall(fun i -> equals (a1.GetValue(i)) (a2.GetValue(i)))    
        |   _ ->
                Object.Equals(expected, actual)

type Assert = 
    

    static member AreEqual(expected : obj, actual : obj, message : string) =
        if not (Impl.equals expected actual) then
            let message = sprintf "%s: Expected %A but got %A" message expected actual
            AssertionException message |> raise

    static member AreNotEqual(expected : obj, actual : obj, message : string) =
        if Impl.equals expected actual then
            let message = sprintf "%s: Expected not %A but got %A" message expected actual
            AssertionException message |> raise


    static member AreEqual(expected : obj, actual : obj) = Assert.AreEqual(expected, actual, "Assertion")

    static member AreNotEqual(expected : obj, actual : obj) = Assert.AreNotEqual(expected, actual, "Assertion")

    static member IsNull(o : obj) = Assert.AreEqual(null, o)

    static member IsTrue(x : bool, message : string) =
        if not x then
            AssertionException(message) |> raise

    static member IsTrue(x : bool) = Assert.IsTrue(x, "")

    static member IsFalse(x : bool, message : string) =
        if x then
            AssertionException(message) |> raise

    static member IsFalse(x : bool) = Assert.IsFalse(x, "")

    static member Fail(message : string) = AssertionException(message) |> raise
    
    static member Fail() = Assert.Fail("") 

    static member Fail(message : string, args : obj[]) = Assert.Fail(String.Format(message,args))