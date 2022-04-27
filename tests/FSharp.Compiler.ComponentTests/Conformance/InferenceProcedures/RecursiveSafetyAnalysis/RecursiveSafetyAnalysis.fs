// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module RecursiveSafetyAnalysis =

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun

    // SOURCE=E_CyclicReference01.fs SCFLAGS="--mlcompatibility --test:ErrorRanges --flaterrors"  # E_CyclicReference01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_CyclicReference01.fs"|])>]
    let ``E_CyclicReference01_fs`` compilation =
        compilation
        |> withOptions ["--mlcompatibility"; "--flaterrors"]
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 953, Line 6, Col 6, Line 6, Col 15, "This type definition involves an immediate cyclic reference through an abbreviation")
            (Error 1, Line 8, Col 25, Line 8, Col 34, "This expression was expected to have type\n    'bogusType'    \nbut here has type\n    'Map<'a,'b>'    ")
        ]

    // SOURCE=E_DuplicateRecursiveRecords.fs SCFLAGS="--test:ErrorRanges"          # E_DuplicateRecursiveRecords.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_DuplicateRecursiveRecords.fs"|])>]
    let``E_DuplicateRecursiveRecords_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 37, Line 10, Col 5, Line 10, Col 8, "Duplicate definition of type, exception or module 'Foo'")
        ]

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_RecursiveInline.fs"|])>]
    let ``E_RecursiveInline_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1114, Line 8, Col 15, Line 8, Col 25, "The value 'E_RecursiveInline.test' was marked inline but was not bound in the optimization environment")
            (Error 1113, Line 7, Col 16, Line 7, Col 20, "The value 'test' was marked inline but its implementation makes use of an internal or private function which is not sufficiently accessible")
            (Warning 1116, Line 8, Col 15, Line 8, Col 25, "A value marked as 'inline' has an unexpected value")
            (Error 1118, Line 8, Col 15, Line 8, Col 25, "Failed to inline the value 'test' marked 'inline', perhaps because a recursive value was marked 'inline'")
        ]

    // SOURCE=E_TypeDeclaration01.fs SCFLAGS="--langversion:5.0 --test:ErrorRanges" COMPILE_ONLY=1	# E_TypeDeclaration01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_TypeDeclaration01.fs"|])>]
    let ``E_TypeDeclaration01`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 193, Line 15, Col 14, Line 15, Col 20, "Type constraint mismatch. The type \n    'int * 'a'    \nis not compatible with type\n    'string'    \n")
        ]

    //<Expects status="error" id="FS0193" span="(21,27-21,28)">Type constraint mismatch</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_TypeDeclaration02.fs"|])>]
    let ``E_TypeDeclaration02_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 20, Col 48, Line 20, Col 49, "This expression was expected to have type\n    'myint<'a>'    \nbut here has type\n    'SfsIntTerm<'c>'    ")
            (Error 1, Line 21, Col 27, Line 21, Col 28, "The type 'myint<'d>' is not compatible with the type 'SfsIntTerm<'a>'")
            (Error 1, Line 21, Col 29, Line 21, Col 30, "The type 'myint<'d>' is not compatible with the type 'SfsIntTerm<'b>'")
            (Error 193, Line 21, Col 27, Line 21, Col 28, "Type constraint mismatch. The type \n    'myint<'d>'    \nis not compatible with type\n    'SfsIntTerm<'a>'    \n")
        ]

    // SOURCE=E_VariationsOnRecursiveStruct.fs SCFLAGS="--test:ErrorRanges"     # E_VariationsOnRecursiveStruct.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_VariationsOnRecursiveStruct.fs"|])>]
    let ``E_VariationsOnRecursiveStruct_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 954, Line 6, Col 6, Line 6, Col 8, "This type definition involves an immediate cyclic reference through a struct field or inheritance relation")
            (Error 912, Line 6, Col 6, Line 6, Col 8, "This declaration element is not permitted in an augmentation")
        ]

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InfiniteRecursiveExplicitConstructor.fs"|])>]
    let ``InfiniteRecursiveExplicitConstructor_fs`` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldSucceed

    // SOURCE=RecursiveTypeDeclarations01.fs                        # RecursiveTypeDeclarations01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecursiveTypeDeclarations01.fs"|])>]
    let ``RecursiveTypeDeclarations01_fs`` compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE=RecursiveValueDeclarations01.fs                        # RecursiveValueDeclarations01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecursiveValueDeclarations01.fs"|])>]
    let ``RecursiveValueDeclarations01_fs`` compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE=RecursiveTypeDeclarations02.fs                        # RecursiveTypeDeclarations02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RecursiveTypeDeclarations02.fs"|])>]
    let ``RecursiveTypeDeclarations02_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:3370"]
        |> verifyCompileAndRun
        |> shouldSucceed

