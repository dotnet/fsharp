// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions.GeneratedEqualityHashingComparison.Attributes

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Diags =

    // SOURCE=E_AdjustUses01a.fs SCFLAGS=--test:ErrorRanges                 # E_AdjustUses01a.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AdjustUses01a.fs"|])>]
    let``E_AdjustUses01a_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 3, Line 7, Col 30, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
        ]

    // SOURCE=E_AdjustUses01b.fs SCFLAGS=--test:ErrorRanges                 # E_AdjustUses01b.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AdjustUses01b.fs"|])>]
    let``E_AdjustUses01b_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 3, Line 7, Col 30, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
        ]

