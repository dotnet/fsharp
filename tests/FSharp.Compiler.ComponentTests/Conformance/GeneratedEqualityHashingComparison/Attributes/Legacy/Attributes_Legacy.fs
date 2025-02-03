// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.GeneratedEqualityHashingComparison

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AttributesLegacy =

    // SOURCE=Test01.fs SCFLAGS="--test:ErrorRanges"        # Test01.fs
    [<Theory; FileInlineData("Test01.fs")>]
    let``Test01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 28, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 8, Col 5, Line 8, Col 31, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
            (Error 501, Line 9, Col 5, Line 9, Col 29, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test02.fs SCFLAGS="--test:ErrorRanges"        # Test02.fs
    [<Theory; FileInlineData("Test02.fs")>]
    let``Test02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 8, Col 5, Line 8, Col 28, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 9, Col 5, Line 9, Col 31, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
            (Error 501, Line 10, Col 5, Line 10, Col 30, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test03.fs SCFLAGS="--test:ErrorRanges"        # Test03.fs
    [<Theory; FileInlineData("Test03.fs")>]
    let``Test03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 28, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 8, Col 5, Line 8, Col 31, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
        ]

    // SOURCE=Test04.fs SCFLAGS="--test:ErrorRanges"        # Test04.fs
    [<Theory; FileInlineData("Test04.fs")>]
    let``Test04_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 28, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 8, Col 5, Line 8, Col 32, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
            (Error 501, Line 9, Col 5, Line 9, Col 29, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test05.fs SCFLAGS="--test:ErrorRanges"        # Test05.fs
    [<Theory; FileInlineData("Test05.fs")>]
    let``Test05_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 28, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 8, Col 5, Line 8, Col 32, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
            (Error 501, Line 9, Col 5, Line 9, Col 30, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test06.fs SCFLAGS="--test:ErrorRanges"        # Test06.fs
    [<Theory; FileInlineData("Test06.fs")>]
    let``Test06_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 28, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 8, Col 5, Line 8, Col 32, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
        ]

    // SOURCE=Test07.fs SCFLAGS="--test:ErrorRanges"        # Test07.fs
    [<Theory; FileInlineData("Test07.fs")>]
    let``Test07_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 28, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 9, Col 5, Line 9, Col 29, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test08.fs SCFLAGS="--test:ErrorRanges"        # Test08.fs
    [<Theory; FileInlineData("Test08.fs")>]
    let``Test08_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 28, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 9, Col 5, Line 9, Col 30, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test09.fs SCFLAGS="--test:ErrorRanges"        # Test09.fs
    [<Theory; FileInlineData("Test09.fs")>]
    let``Test09_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 6, Col 5, Line 6, Col 28, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
        ]

    // SOURCE=Test10.fs SCFLAGS="--test:ErrorRanges"        # Test10.fs
    [<Theory; FileInlineData("Test10.fs")>]
    let``Test10_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 8, Col 5, Line 8, Col 29, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 9, Col 5, Line 9, Col 31, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
            (Error 501, Line 10, Col 5, Line 10, Col 29, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test11.fs SCFLAGS="--test:ErrorRanges"        # Test11.fs
    [<Theory; FileInlineData("Test11.fs")>]
    let``Test11_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 8, Col 5, Line 8, Col 29, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 9, Col 5, Line 9, Col 31, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
            (Error 501, Line 10, Col 5, Line 10, Col 30, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test12.fs SCFLAGS="--test:ErrorRanges"        # Test12.fs
    [<Theory; FileInlineData("Test12.fs")>]
    let``Test12_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 29, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 8, Col 5, Line 8, Col 31, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
        ]

    // SOURCE=Test13.fs SCFLAGS="--test:ErrorRanges"        # Test13.fs
    [<Theory; FileInlineData("Test13.fs")>]
    let``Test13_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 29, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 8, Col 5, Line 8, Col 32, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
            (Error 501, Line 9, Col 5, Line 9, Col 29, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test14.fs SCFLAGS="--test:ErrorRanges"        # Test14.fs
    [<Theory; FileInlineData("Test14.fs")>]
    let``Test14_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 9, Col 5, Line 9, Col 29, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 10, Col 5, Line 10, Col 32, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
            (Error 501, Line 11, Col 5, Line 11, Col 30, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test15.fs SCFLAGS="--test:ErrorRanges"        # Test15.fs
    [<Theory; FileInlineData("Test15.fs")>]
    let``Test15_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 29, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 8, Col 5, Line 8, Col 32, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
        ]

    // SOURCE=Test16.fs SCFLAGS="--test:ErrorRanges"        # Test16.fs
    [<Theory; FileInlineData("Test16.fs")>]
    let``Test16_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 29, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 9, Col 5, Line 9, Col 29, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test17.fs SCFLAGS="--test:ErrorRanges"        # Test17.fs
    [<Theory; FileInlineData("Test17.fs")>]
    let``Test17_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 6, Col 5, Line 6, Col 29, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
            (Error 501, Line 8, Col 5, Line 8, Col 30, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test18.fs SCFLAGS="--test:ErrorRanges"        # Test18.fs
    [<Theory; FileInlineData("Test18.fs")>]
    let``Test18_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 6, Col 5, Line 6, Col 29, "The object constructor 'ReferenceEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> ReferenceEqualityAttribute'.")
        ]

    // SOURCE=Test19.fs SCFLAGS="--test:ErrorRanges"        # Test19.fs
    [<Theory; FileInlineData("Test19.fs")>]
    let``Test19_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 31, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
            (Error 501, Line 8, Col 5, Line 8, Col 29, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test20.fs SCFLAGS="--test:ErrorRanges"        # Test20.fs
    [<Theory; FileInlineData("Test20.fs")>]
    let``Test20_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 31, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
            (Error 501, Line 8, Col 5, Line 8, Col 30, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test21.fs SCFLAGS="--test:ErrorRanges"        # Test21.fs
    [<Theory; FileInlineData("Test21.fs")>]
    let``Test21_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 6, Col 5, Line 6, Col 31, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
        ]

    // SOURCE=Test22.fs SCFLAGS="--test:ErrorRanges"        # Test22.fs
    [<Theory; FileInlineData("Test22.fs")>]
    let``Test22_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 32, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
            (Error 501, Line 8, Col 5, Line 8, Col 29, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test23.fs SCFLAGS="--test:ErrorRanges"        # Test23.fs
    [<Theory; FileInlineData("Test23.fs")>]
    let``Test23_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 9, Col 5, Line 9, Col 32, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
            (Error 501, Line 10, Col 5, Line 10, Col 30, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test24.fs SCFLAGS="--test:ErrorRanges"        # Test24.fs
    [<Theory; FileInlineData("Test24.fs")>]
    let``Test24_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 6, Col 5, Line 6, Col 32, "The object constructor 'StructuralComparisonAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralComparisonAttribute'.")
        ]

    // SOURCE=Test25.fs SCFLAGS="--test:ErrorRanges"        # Test25.fs
    [<Theory; FileInlineData("Test25.fs")>]
    let``Test25_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 29, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test26.fs SCFLAGS="--test:ErrorRanges"        # Test26.fs
    [<Theory; FileInlineData("Test26.fs")>]
    let``Test26_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 5, Line 7, Col 30, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]

    // SOURCE=Test27.fs SCFLAGS="--test:ErrorRanges"        # Test27.fs
    [<Theory; FileInlineData("Test27.fs")>]
    let``Test27_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=Test28.fs SCFLAGS="--test:ErrorRanges"        # Test28.fs
    [<Theory; FileInlineData("Test28.fs")>]
    let``Test28_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 501, Line 7, Col 3, Line 7, Col 28, "The object constructor 'StructuralEqualityAttribute' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> StructuralEqualityAttribute'.")
        ]


