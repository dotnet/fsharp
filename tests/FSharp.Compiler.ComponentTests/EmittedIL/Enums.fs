// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Compiler.Assertions.StructuredResultsAsserts

module Enums =

    [<Fact>]
    let ``Arithmetic in enum definition works``() =
        FSharp """
module Enums

let [<Literal>] one = 1

type Flags =
    | A = 1
    | B = (one <<< 1)
    | C = (one <<< (one * 2))
        """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.field public static literal valuetype Enums/Flags A = int32(0x00000001)"""
            """.field public static literal valuetype Enums/Flags B = int32(0x00000002)"""
            """.field public static literal valuetype Enums/Flags C = int32(0x00000004)"""
        ]

    [<Fact>]
    let ``Enum with inconsistent case types errors with the right message``() =
        FSharp """
module Enums

type E =
    | A = (1L <<< 0)
    | B = (1 <<< 1)
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withResult {
            Error = Error 1
            Range = { StartLine = 6
                      StartColumn = 11
                      EndLine = 6
                      EndColumn = 20 }
            Message = "This expression was expected to have type
    'int64'    
but here has type
    'int'    "
        }