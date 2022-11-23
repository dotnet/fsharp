// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Compiler

module ``Literals`` =

    [<Fact>]
    let ``Literal attribute generates literal static field``() =
        FSharp """
module LiteralValue

[<Literal>]
let x = 7

[<EntryPoint>]
let main _ =
    0
         """
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
.field public static literal int32 x = int32(0x00000007)
.custom instance void [FSharp.Core]Microsoft.FSharp.Core.LiteralAttribute::.ctor() = ( 01 00 00 00 )"""]


    [<Fact>]
    let ``Arithmetics in integer literals is evaluated at compile-time``() =
        FSharp """
module LiteralArithmetics

let [<Literal>] bytesInMegabyte = 1024L * 1024L

let [<Literal>] bytesInKilobyte = bytesInMegabyte >>> 10

let [<Literal>] bytesInKilobyte2 = bytesInMegabyte / 1024L

let [<Literal>] secondsInDayPlusThree = 3 + (60 * 60 * 24)

let [<Literal>] bitwise = 1us &&& (3us ||| 4us)
        """
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.field public static literal int64 bytesInMegabyte = int64(0x100000)"""
            """.field public static literal int64 bytesInKilobyte = int64(0x400)"""
            """.field public static literal int64 bytesInKilobyte2 = int64(0x400)"""
            """.field public static literal int32 secondsInDayPlusThree = int32(0x00015183)"""
            """.field public static literal uint16 bitwise = uint16(0x0001)"""
        ]

    [<Fact>]
    let ``Logical operations on booleans are evaluated at compile-time``() =
        FSharp """
module LiteralArithmetics

let [<Literal>] flag = true

let [<Literal>] flippedFlag = not flag

let [<Literal>] simple1 = flippedFlag || false

let [<Literal>] simple2 = true && not true

let [<Literal>] complex1 = false || (flag && not flippedFlag)

let [<Literal>] complex2 = false || (flag && flippedFlag)

let [<Literal>] complex3 = true || (flag && not flippedFlag)
        """
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.field public static literal bool flag = bool(true)"""
            """.field public static literal bool flippedFlag = bool(false)"""
            """.field public static literal bool simple1 = bool(false)"""
            """.field public static literal bool simple2 = bool(false)"""
            """.field public static literal bool complex1 = bool(true)"""
            """.field public static literal bool complex2 = bool(false)"""
            """.field public static literal bool complex3 = bool(true)"""
        ]

    [<Fact>]
    let ``Arithmetics can be used for constructing enum literals``() =
        FSharp """
module LiteralArithmetics

type E =
    | A = 1
    | B = 2
    
let [<Literal>] x = enum<E> (1 + 1)
        """
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.field public static literal valuetype LiteralArithmetics/E x = int32(0x00000002)"""
        ]

    [<Fact>]
    let ``Arithmetics can be used for constructing literals in attributes``() =
        FSharp """
module LiteralArithmetics

open System.Runtime.CompilerServices

// 256 = AggressiveInlining
[<MethodImpl(enum -(-1 <<< 8))>]
let x () =
    3
        """
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.method public static int32  x() cil managed aggressiveinlining"""
        ]

    [<Fact>]
    let ``Compilation fails when addition in literal overflows``() =
        FSharp """
module LiteralArithmetics

let [<Literal>] x = System.Int32.MaxValue + 1
        """
        |> compile
        |> shouldFail
        |> withResult {
            Error = Error 3177
            Range = { StartLine = 4
                      StartColumn = 21
                      EndLine = 4
                      EndColumn = 46 }
            Message = "This literal expression or attribute argument results in an arithmetic overflow."
        }

    [<Fact>]
    let ``Compilation fails when using decimal arithmetics in literal``() =
        FSharp """
module LiteralArithmetics

let [<Literal>] x = 1m + 1m
        """
        |> compile
        |> shouldFail
        |> withResults [
            { Error = Error 267
              Range = { StartLine = 4
                        StartColumn = 21
                        EndLine = 4
                        EndColumn = 23 }
              Message = "This is not a valid constant expression or custom attribute value" }
            { Error = Error 267
              Range = { StartLine = 4
                        StartColumn = 26
                        EndLine = 4
                        EndColumn = 28 }
              Message = "This is not a valid constant expression or custom attribute value" }
            { Error = Error 267
              Range = { StartLine = 4
                        StartColumn = 21
                        EndLine = 4
                        EndColumn = 28 }
              Message = "This is not a valid constant expression or custom attribute value" }
        ]

    [<Fact>]
    let ``Compilation fails when using arithmetics with a non-literal in literal``() =
        FSharp """
module LiteralArithmetics

let [<Literal>] x = 1 + System.DateTime.Now.Hour
        """
        |> compile
        |> shouldFail
        |> withResults [
#if !NETCOREAPP
            { Error = Warning 52
              Range = { StartLine = 4
                        StartColumn = 25
                        EndLine = 4
                        EndColumn = 49 }
              Message = "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed" }
#endif
            { Error = Error 267
              Range = { StartLine = 4
                        StartColumn = 25
                        EndLine = 4
                        EndColumn = 49 }
              Message = "This is not a valid constant expression or custom attribute value" }
            { Error = Error 267
              Range = { StartLine = 4
                        StartColumn = 21
                        EndLine = 4
                        EndColumn = 49 }
              Message = "This is not a valid constant expression or custom attribute value" }
        ]