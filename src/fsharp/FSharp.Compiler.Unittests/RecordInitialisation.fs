// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace FSharp.Compiler.Unittests

open System
open System.Text
open NUnit.Framework
open Microsoft.FSharp.Compiler

type RecordWithInts = 
    { A : int
      B : int
      C : int }

[<TestFixture>]
type OrderOfRecordInitialisation() = 

    let expected =  
        { A = 1
          B = 2
          C = 3 }

    [<Test>]
    member this.ShouldInitialzeInGivenOrder1() = 
        let order = ref ""
        let actual =
          { A = let _ = order := !order + "1" in 1
            B = let _ = order := !order + "2" in 2
            C = let _ = order := !order + "3" in 3 }

        Assert.AreEqual(expected, actual)
        Assert.AreEqual("123", !order)

    [<Test>]
    member this.ShouldInitialzeInGivenOrder2() = 
        let order = ref ""
        let actual =
          { A = let _ = order := !order + "1" in 1
            C = let _ = order := !order + "2" in 3
            B = let _ = order := !order + "3" in 2 }

        Assert.AreEqual(expected, actual)
        Assert.AreEqual("123", !order)

    [<Test>]
    member this.ShouldInitialzeInGivenOrder3() = 
        let order = ref ""
        let actual =
          { B = let _ = order := !order + "1" in 2
            A = let _ = order := !order + "2" in 1
            C = let _ = order := !order + "3" in 3 }

        Assert.AreEqual(expected, actual)
        Assert.AreEqual("123", !order)

    [<Test>]
    member this.ShouldInitialzeInGivenOrder4() = 
        let order = ref ""
        let actual =
          { B = let _ = order := !order + "1" in 2
            C = let _ = order := !order + "2" in 3
            A = let _ = order := !order + "3" in 1 }

        Assert.AreEqual(expected, actual)
        Assert.AreEqual("123", !order)

    [<Test>]
    member this.ShouldInitialzeInGivenOrder5() = 
        let order = ref ""
        let actual =
          { C = let _ = order := !order + "1" in 3
            A = let _ = order := !order + "2" in 1
            B = let _ = order := !order + "3" in 2 }

        Assert.AreEqual(expected, actual)
        Assert.AreEqual("123", !order)

    [<Test>]
    member this.ShouldInitialzeInGivenOrder6() = 
        let order = ref ""
        let actual =
          { C = let _ = order := !order + "1" in 3
            B = let _ = order := !order + "2" in 2
            A = let _ = order := !order + "3" in 1 }

        Assert.AreEqual(expected, actual)
        Assert.AreEqual("123", !order)


type RecordWithDifferentTypes = 
    { A : int
      B : string
      C : float
      D : RecordWithInts }

[<TestFixture>]
type RecordInitialisationWithDifferentTxpes() = 

    let expected =  
        { A = 1
          B = "2"
          C = 3.0
          D = 
            { A = 4
              B = 5
              C = 6 }}

    [<Test>]
    member this.ShouldInitialzeInGivenOrder1() = 
        let order = ref ""
        let actual =
          { A = let _ = order := !order + "1" in 1
            B = let _ = order := !order + "2" in "2"
            C = let _ = order := !order + "3" in 3.0
            D = let _ = order := !order + "4" in 
                              { A = let _ = order := !order + "5" in 4
                                B = let _ = order := !order + "6" in 5
                                C = let _ = order := !order + "7" in 6 } }

        Assert.AreEqual(expected, actual)
        Assert.AreEqual("1234567", !order)

    [<Test>]
    member this.ShouldInitialzeInGivenOrder2() = 
        let order = ref ""
        let actual =
          { A = let _ = order := !order + "1" in 1            
            C = let _ = order := !order + "2" in 3.0
            D = let _ = order := !order + "3" in 
                              { A = let _ = order := !order + "4" in 4
                                B = let _ = order := !order + "5" in 5
                                C = let _ = order := !order + "6" in 6 }
                                
            B = let _ = order := !order + "7" in "2" }

        Assert.AreEqual(expected, actual)
        Assert.AreEqual("1234567", !order)

    [<Test>]
    member this.ShouldInitialzeInGivenOrder3() = 
        let order = ref ""
        let actual =
          { A = let _ = order := !order + "1" in 1            
            C = let _ = order := !order + "2" in 3.0
            B = let _ = order := !order + "3" in "2"
            D = let _ = order := !order + "4" in 
                              { A = let _ = order := !order + "5" in 4
                                B = let _ = order := !order + "6" in 5
                                C = let _ = order := !order + "7" in 6 } }

        Assert.AreEqual(expected, actual)
        Assert.AreEqual("1234567", !order)


    [<Test>]
    member this.ShouldInitialzeInGivenOrder4() = 
        let order = ref ""
        let actual =
          { B = let _ = order := !order + "1" in "2"
            A = let _ = order := !order + "2" in 1            
            C = let _ = order := !order + "3" in 3.0            
            D = let _ = order := !order + "4" in 
                              { A = let _ = order := !order + "5" in 4
                                B = let _ = order := !order + "6" in 5
                                C = let _ = order := !order + "7" in 6 } }

        Assert.AreEqual(expected, actual)
        Assert.AreEqual("1234567", !order)

    [<Test>]
    member this.ShouldInitialzeInGivenOrder5() = 
        let order = ref ""
        let actual =
          { D = let _ = order := !order + "1" in 
                              { A = let _ = order := !order + "2" in 4
                                B = let _ = order := !order + "3" in 5
                                C = let _ = order := !order + "4" in 6 } 
            B = let _ = order := !order + "5" in "2"
            C = let _ = order := !order + "6" in 3.0
            A = let _ = order := !order + "7" in 1 }

        Assert.AreEqual(expected, actual)
        Assert.AreEqual("1234567", !order)

    [<Test>]
    member this.ShouldInitialzeInGivenOrder6() = 
        let order = ref ""
        let actual =
          { D = let _ = order := !order + "1" in 
                              { A = let _ = order := !order + "2" in 4
                                B = let _ = order := !order + "3" in 5
                                C = let _ = order := !order + "4" in 6 } 
            A = let _ = order := !order + "5" in 1
            B = let _ = order := !order + "6" in "2"
            C = let _ = order := !order + "7" in 3.0 }

        Assert.AreEqual(expected, actual)
        Assert.AreEqual("1234567", !order)