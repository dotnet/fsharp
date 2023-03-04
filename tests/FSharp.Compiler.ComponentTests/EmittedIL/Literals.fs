// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Compiler.Assertions.StructuredResultsAsserts

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
    let ``Arithmetic in integer literals is evaluated at compile time``() =
        FSharp """
module LiteralArithmetic

let [<Literal>] bytesInMegabyte = 1024L * 1024L

let [<Literal>] bytesInKilobyte = bytesInMegabyte >>> 10

let [<Literal>] bytesInKilobyte2 = bytesInMegabyte / 1024L

let [<Literal>] secondsInDayPlusThree = 3 + (60 * 60 * 24)

let [<Literal>] bitwise = 1us &&& (3us ||| 4us)
        """
        |> withLangVersionPreview
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
    let ``Arithmetic in char and floating point literals is evaluated at compile time``() =
        // on Linux and Mac floats with no decimal parts are printed without the decimal point (unlike Windows)
        // let's add some fractions so that the tests are consistent
        FSharp """
module LiteralArithmetic

let [<Literal>] bytesInMegabyte = 1024. * 1024. + 0.1

let [<Literal>] bytesInKilobyte = bytesInMegabyte / 1024. + 0.1

let [<Literal>] secondsInDayPlusThree = 3.1f + (60f * 60f * 24f)

let [<Literal>] chars = 'a' + 'b' - 'a'
        """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.field public static literal float64 bytesInMegabyte = float64(1048576.1000000001)"""
            """.field public static literal float64 bytesInKilobyte = float64(1024.10009765625)"""
            """.field public static literal float32 secondsInDayPlusThree = float32(86403.102)"""
            """.field public static literal char chars = char(0x0062)"""
        ]

    [<Fact>]
    let ``Logical operations on booleans are evaluated at compile time``() =
        FSharp """
module LiteralArithmetic

let [<Literal>] flag = true

let [<Literal>] flippedFlag = not flag

let [<Literal>] simple1 = flippedFlag || false

let [<Literal>] simple2 = true && not true

let [<Literal>] complex1 = false || (flag && not flippedFlag)

let [<Literal>] complex2 = false || (flag && flippedFlag)

let [<Literal>] complex3 = true || (flag && not flippedFlag)
        """
        |> withLangVersionPreview
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
    let ``Arithmetic can be used for constructing enum literals``() =
        FSharp """
module LiteralArithmetic

type E =
    | A = 1
    | B = 2
    
let [<Literal>] x = enum<E> (1 + 1)
        """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.field public static literal valuetype LiteralArithmetic/E x = int32(0x00000002)"""
        ]

    [<Fact>]
    let ``Arithmetic can be used for constructing literals in attributes``() =
        FSharp """
module LiteralArithmetic

open System.Runtime.CompilerServices

// 256 = AggressiveInlining
[<MethodImpl(enum -(-1 <<< 8))>]
let x () =
    3
        """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.method public static int32  x() cil managed aggressiveinlining"""
        ]

    [<Fact>]
    let ``Compilation fails when addition in literal overflows``() =
        FSharp """
module LiteralArithmetic

let [<Literal>] x = System.Int32.MaxValue + 1
        """
        |> withLangVersionPreview
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
    let ``Compilation fails when using decimal arithmetic in literal``() =
        FSharp """
module LiteralArithmetic

let [<Literal>] x = 1m + 1m
        """
        |> withLangVersionPreview
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
    let ``Compilation fails when using arithmetic with a non-literal in literal``() =
        FSharp """
module LiteralArithmetic

let [<Literal>] x = 1 + System.DateTime.Now.Hour
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResults [
#if !NETCOREAPP
            { Error = Warning 52
              Range = { StartLine = 4
                        StartColumn = 45
                        EndLine = 4
                        EndColumn = 49 }
              Message = "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed" }
#endif
            { Error = Error 267
              Range = { StartLine = 4
                        StartColumn = 45
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

    [<Fact>]
    let ``Arithmetic cannot be used in enums, literals and attributes in lang version70``() =
        FSharp """
module LiteralArithmetic

open System.Runtime.CompilerServices

[<MethodImpl(enum -(-1 <<< 8))>]
let x () = 3

let [<Literal>] lit = 1 <<< (7 * 10)

type E =
    | A = (1 <<< 2)
    | B = 1
    | C = (5 / 3 * 4)
        """
        |> withLangVersion70
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 6, Col 19, Line 6, Col 30, "Feature 'Arithmetic and logical operations in literals, enum definitions and attributes' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 9, Col 23, Line 9, Col 37, "Feature 'Arithmetic and logical operations in literals, enum definitions and attributes' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 12, Col 12, Line 12, Col 19, "Feature 'Arithmetic and logical operations in literals, enum definitions and attributes' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 14, Col 12, Line 14, Col 21, "Feature 'Arithmetic and logical operations in literals, enum definitions and attributes' is not available in F# 7.0. Please use language version 'PREVIEW' or greater.")
        ]
