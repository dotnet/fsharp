// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.MultiTargeting

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module MultiTargetingTests =

    [<FactForDESKTOP>]
    let ``E_BadPathToFSharpCore - Invalid FSharp.Core path produces FS0084`` () =
        FSharp "exit 0"
            |> withOptions [
                "-o:test.exe"
                "--target:exe"
                "--noframework"
                "-r:mscorlib.dll"
                "-r:I_DO_NOT_EXIST/FSharp.Core.dll"
               ]
            |> compile
            |> shouldFail
            |> withErrorCode 0084
            |> withDiagnosticMessageMatches "I_DO_NOT_EXIST"

    [<FactForDESKTOP>]
    let ``E_BadPathToFSharpCore fsx - FSI variant with invalid path`` () =
        Fsx "exit 0"
            |> withOptions [
                "-o:test.exe"
                "--target:exe"
                "--noframework"
                "-r:mscorlib.dll"
                "-r:I_DO_NOT_EXIST/FSharp.Core.dll"
               ]
            |> compile
            |> shouldFail
            |> withErrorCode 0084
            |> withDiagnosticMessageMatches "I_DO_NOT_EXIST"

    [<FactForDESKTOP>]
    let ``E_MissingReferenceToFSharpCore - Compiles without FSharp.Core (no ICE)`` () =
        // Regression test for FSHARP1.0:4800
        // Verifies compiler doesn't ICE when compiling without FSharp.Core reference
        // (mixing .NET Framework versions: mscorlib 4.0 but no FSharp.Core)
        FSharp "exit 0"
            |> withOptions [
                "-o:test.exe"
                "--target:exe"
                "--noframework"
                "-r:mscorlib.dll"
               ]
            |> compile
            |> shouldSucceed  // Original test expected success - just verifying no ICE
