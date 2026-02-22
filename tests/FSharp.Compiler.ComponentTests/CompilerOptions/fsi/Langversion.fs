// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsi

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

// Migrated from FSharpQA suite - CompilerOptions/fsi/langversion
// Tests for invalid langversion arguments that produce errors in FSI.

module Langversion =

    // Test: --langversion:4.5 - unrecognized version
    // Original: SOURCE=badlangversion.fsx SCFLAGS=" --langversion:4.5"
    [<Fact>]
    let ``fsi langversion - unrecognized version 4.5`` () =
        FSharp """exit 0"""
        |> asFsx
        |> withOptions ["--langversion:4.5"]
        |> compile
        |> shouldFail
        |> withErrorCode 246
        |> withDiagnosticMessageMatches "Unrecognized value '4\\.5' for --langversion"
        |> ignore

    // Test: --langversion:4,7 - comma instead of dot (culture issue)
    // Original: SOURCE=badlangversion-culture.fsx SCFLAGS=" --langversion:4,7"
    [<Fact>]
    let ``fsi langversion - comma instead of dot`` () =
        FSharp """exit 0"""
        |> asFsx
        |> withOptions ["--langversion:4,7"]
        |> compile
        |> shouldFail
        |> withErrorCode 246
        |> withDiagnosticMessageMatches "Unrecognized value '4,7' for --langversion"
        |> ignore

    // Test: --langversion:4.70000000000 - too many decimal places
    // Original: SOURCE=badlangversion-decimal.fsx SCFLAGS=" --langversion:4.70000000000"
    [<Fact>]
    let ``fsi langversion - too many decimal places`` () =
        FSharp """exit 0"""
        |> asFsx
        |> withOptions ["--langversion:4.70000000000"]
        |> compile
        |> shouldFail
        |> withErrorCode 246
        |> withDiagnosticMessageMatches "Unrecognized value '4\\.70000000000' for --langversion"
        |> ignore
