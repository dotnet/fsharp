// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions.GeneratedEqualityHashingComparison.Attributes

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module New =

    // SOURCE=Test01.fs SCFLAGS="--test:ErrorRanges"        # Test01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test01.fs"|])>]
    let``Test01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 381, Line 8, Col 8, Line 8, Col 9, "A type cannot have both the 'ReferenceEquality' and 'StructuralEquality' or 'StructuralComparison' attributes")
        ]

    // SOURCE=Test02.fs SCFLAGS="--test:ErrorRanges"        # Test02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test02.fs"|])>]
    let``Test02_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 14, Col 17, Line 14, Col 19, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 15, Col 12, Line 15, Col 14, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 16, Col 12, Line 16, Col 14, "The type 'R' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface")
        ]

    // SOURCE=Test03.fs SCFLAGS="--test:ErrorRanges"        # Test03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test03.fs"|])>]
    let``Test03_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 14, Col 12, Line 14, Col 14, "The type 'R' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface")
        ]

    // SOURCE=Test04.fs SCFLAGS="--test:ErrorRanges"        # Test04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test04.fs"|])>]
    let``Test04_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 380, Line 9, Col 8, Line 9, Col 9, "The 'StructuralEquality' attribute must be used in conjunction with the 'NoComparison' or 'StructuralComparison' attributes")
            (Error 385, Line 9, Col 8, Line 9, Col 9, "A type with attribute 'CustomComparison' must have an explicit implementation of at least one of 'System.IComparable' or 'System.Collections.IStructuralComparable'")
        ]

    // SOURCE=Test05.fs SCFLAGS="--test:ErrorRanges"        # Test05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test05.fs"|])>]
    let``Test05_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 12, Col 17, Line 12, Col 19, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 13, Col 12, Line 13, Col 14, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 14, Col 12, Line 14, Col 14, "The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute")
        ]

    // SOURCE=Test06.fs SCFLAGS="--test:ErrorRanges"        # Test06.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test06.fs"|])>]
    let``Test06_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 377, Line 9, Col 8, Line 9, Col 9, "This type uses an invalid mix of the attributes 'NoEquality', 'ReferenceEquality', 'StructuralEquality', 'NoComparison' and 'StructuralComparison'")
            (Error 385, Line 9, Col 8, Line 9, Col 9, "A type with attribute 'CustomComparison' must have an explicit implementation of at least one of 'System.IComparable' or 'System.Collections.IStructuralComparable'")
        ]

    // SOURCE=Test07.fs SCFLAGS="--test:ErrorRanges"        # Test07.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test07.fs"|])>]
    let``Test07_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 380, Line 7, Col 8, Line 7, Col 9, "The 'StructuralEquality' attribute must be used in conjunction with the 'NoComparison' or 'StructuralComparison' attributes")
        ]

    // SOURCE=Test08.fs SCFLAGS="--test:ErrorRanges"        # Test08.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test08.fs"|])>]
    let``Test08_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 13, Col 17, Line 13, Col 19, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 14, Col 12, Line 14, Col 14, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
        ]

    // SOURCE=Test09.fs SCFLAGS="--test:ErrorRanges"        # Test09.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test09.fs"|])>]
    let``Test09_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Test10.fs SCFLAGS="--test:ErrorRanges"        # Test10.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test10.fs"|])>]
    let``Test10_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 381, Line 8, Col 8, Line 8, Col 9, "A type cannot have both the 'ReferenceEquality' and 'StructuralEquality' or 'StructuralComparison' attributes")
        ]

    // SOURCE=Test11.fs SCFLAGS="--test:ErrorRanges"        # Test11.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test11.fs"|])>]
    let``Test11_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 14, Col 17, Line 14, Col 19, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 15, Col 12, Line 15, Col 14, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 16, Col 12, Line 16, Col 14, "The type 'R' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface")
        ]

    // SOURCE=Test12.fs SCFLAGS="--test:ErrorRanges"        # Test12.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test12.fs"|])>]
    let``Test12_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 379, Line 8, Col 8, Line 8, Col 9, "The 'StructuralComparison' attribute must be used in conjunction with the 'StructuralEquality' attribute")
        ]

    // SOURCE=Test13.fs SCFLAGS="--test:ErrorRanges"        # Test13.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test13.fs"|])>]
    let``Test13_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 385, Line 8, Col 8, Line 8, Col 9, "A type with attribute 'CustomComparison' must have an explicit implementation of at least one of 'System.IComparable' or 'System.Collections.IStructuralComparable'")
        ]

    // SOURCE=Test14.fs SCFLAGS="--test:ErrorRanges"        # Test14.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test14.fs"|])>]
    let``Test14_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 14, Col 17, Line 14, Col 19, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 15, Col 12, Line 15, Col 14, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 16, Col 12, Line 16, Col 14, "The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute")
        ]

    // SOURCE=Test15.fs SCFLAGS="--test:ErrorRanges"        # Test15.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test15.fs"|])>]
    let``Test15_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 18, Col 23, Line 18, Col 25, "The value or constructor 'v3' is not defined.")
        ]

    // SOURCE=Test16.fs SCFLAGS="--test:ErrorRanges"        # Test16.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test16.fs"|])>]
    let``Test16_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 380, Line 8, Col 8, Line 8, Col 9, "The 'StructuralEquality' attribute must be used in conjunction with the 'NoComparison' or 'StructuralComparison' attributes")
        ]

    // SOURCE=Test17.fs SCFLAGS="--test:ErrorRanges"        # Test17.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test17.fs"|])>]
    let``Test17_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 14, Col 17, Line 14, Col 19, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 15, Col 12, Line 15, Col 14, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 16, Col 12, Line 16, Col 14, "The type 'R' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface")
            (Error 39, Line 20, Col 1, Line 20, Col 2, "The value or constructor 'v' is not defined.")
        ]

    // SOURCE=Test18.fs SCFLAGS="--test:ErrorRanges"        # Test18.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test18.fs"|])>]
    let``Test18_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Test19.fs SCFLAGS="--test:ErrorRanges"        # Test19.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test19.fs"|])>]
    let``Test19_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Test20.fs SCFLAGS="--test:ErrorRanges"        # Test20.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test20.fs"|])>]
    let``Test20_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 14, Col 17, Line 14, Col 19, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 15, Col 12, Line 15, Col 14, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 16, Col 12, Line 16, Col 14, "The type 'R' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface")
        ]

    // SOURCE=Test21.fs SCFLAGS="--test:ErrorRanges"        # Test21.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test21.fs"|])>]
    let``Test21_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 379, Line 8, Col 8, Line 8, Col 9, "The 'StructuralComparison' attribute must be used in conjunction with the 'StructuralEquality' attribute")
        ]

    // SOURCE=Test22.fs SCFLAGS="--test:ErrorRanges"        # Test22.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test22.fs"|])>]
    let``Test22_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 15, Col 13, Line 15, Col 15, "The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute")
        ]

    // SOURCE=Test23.fs SCFLAGS="--test:ErrorRanges"        # Test23.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test23.fs"|])>]
    let``Test23_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 14, Col 17, Line 14, Col 19, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 15, Col 17, Line 15, Col 19, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 17, Col 13, Line 17, Col 15, "The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute")
        ]

    // SOURCE=Test24.fs SCFLAGS="--test:ErrorRanges"        # Test24.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test24.fs"|])>]
    let``Test24_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 14, Col 12, Line 14, Col 14, "The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute")
        ]

    // SOURCE=Test25.fs SCFLAGS="--test:ErrorRanges"        # Test25.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test25.fs"|])>]
    let``Test25_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 14, Col 12, Line 14, Col 14, "The type 'R' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface")
        ]

    // SOURCE=Test26.fs SCFLAGS="--test:ErrorRanges"        # Test26.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test26.fs"|])>]
    let``Test26_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 14, Col 17, Line 14, Col 19, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 15, Col 12, Line 15, Col 14, "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute")
            (Error 1, Line 16, Col 12, Line 16, Col 14, "The type 'R' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface")
        ]

    // SOURCE=Test27.fs SCFLAGS="--test:ErrorRanges"        # Test27.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test27.fs"|])>]
    let``Test27_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=Test28.fs SCFLAGS="--test:ErrorRanges"        # Test28.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Test28.fs"|])>]
    let``Test28_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 378, Line 10, Col 6, Line 10, Col 12, "The 'NoEquality' attribute must be used in conjunction with the 'NoComparison' attribute")
            (Warning 386, Line 10, Col 6, Line 10, Col 12, "A type with attribute 'NoEquality' should not usually have an explicit implementation of 'Object.Equals(obj)'. Disable this warning if this is intentional for interoperability purposes")
            (Warning 346, Line 10, Col 6, Line 10, Col 12, "The struct, record or union type 'MyType' has an explicit implementation of 'Object.Equals'. Consider implementing a matching override for 'Object.GetHashCode()'")
        ]


