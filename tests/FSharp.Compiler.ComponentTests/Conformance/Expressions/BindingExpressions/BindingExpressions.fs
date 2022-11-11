// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module BindingExpressions =

    let verifyCompile compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> compile

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ApplicationExpressions/BasicApplication)
    // SOURCE=AmbigLetBinding.fs	# AmbigLetBinding
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AmbigLetBinding.fs"|])>]
    let ``AmbigLetBinding_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 222, Line 6, Col 1, Line 7, Col 1, "Files in libraries or multiple-file applications must begin with a namespace or module declaration, e.g. 'namespace SomeNamespace.SubNamespace' or 'module SomeNamespace.SomeModule'. Only the last source file of an application may omit such a declaration.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ApplicationExpressions/BasicApplication)
    // NoMT SOURCE=in01.fs SCFLAGS="--warnaserror+ --test:ErrorRanges"              COMPILE_ONLY=1 # in01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"in01.fs"|])>]
    let ``in01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 11, Col 5, Line 11, Col 6, "The value or constructor 'a' is not defined.")
            (Warning 20, Line 11, Col 5, Line 11, Col 10, "The result of this expression has type 'bool' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // NoMT SOURCE=in01.fsx FSIMODE=PIPE SCFLAGS="--warnaserror+ --test:ErrorRanges"              COMPILE_ONLY=1 # in01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"in01.fsx"|])>]
    let ``in01_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 10, Col 1, Line 10, Col 2, "The value or constructor 'a' is not defined.")
        ]

    // NoMT SOURCE=in02.fs SCFLAGS="--warnaserror+ --test:ErrorRanges"              COMPILE_ONLY=1 # in02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"in02.fs"|])>]
    let ``in02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 20, Line 12, Col 5, Line 12, Col 10, "The result of this expression has type 'bool' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // NoMT SOURCE=in02.fsx FSIMODE=PIPE SCFLAGS="--warnaserror+ --test:ErrorRanges"              COMPILE_ONLY=1 # in02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"in02.fsx"|])>]
    let ``in02_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldSucceed

    // NoMT SOURCE=in03.fs SCFLAGS="--warnaserror+ --test:ErrorRanges"              COMPILE_ONLY=1 # in03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"in03.fs"|])>]
    let ``in03_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 20, Line 12, Col 5, Line 12, Col 10, "The result of this expression has type 'bool' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // NoMT SOURCE=in03.fsx FSIMODE=PIPE SCFLAGS="--warnaserror+ --test:ErrorRanges"              COMPILE_ONLY=1 # in03.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"in03.fsx"|])>]
    let ``in03_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldSucceed

    // NoMT SOURCE=in04.fs SCFLAGS="--warnaserror+ --test:ErrorRanges"              COMPILE_ONLY=1 # in04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"in04.fs"|])>]
    let ``in04_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 20, Line 13, Col 5, Line 13, Col 10, "The result of this expression has type 'bool' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // NoMT SOURCE=in04.fsx FSIMODE=PIPE SCFLAGS="--warnaserror+ --test:ErrorRanges"              COMPILE_ONLY=1 # in04.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"in04.fsx"|])>]
    let ``in04_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldSucceed

    // NoMT SOURCE=in05.fs SCFLAGS="--warnaserror+ --test:ErrorRanges --flaterrors" COMPILE_ONLY=1 # in05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"in05.fs"|])>]
    let ``in05_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 12, Col 13, Line 12, Col 14, "The types 'unit, int' do not support the operator '+'")
            (Error 1, Line 12, Col 18, Line 12, Col 24, "The types 'unit, int' do not support the operator '+'")
            (Warning 20, Line 13, Col 5, Line 13, Col 10, "The result of this expression has type 'bool' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // NoMT SOURCE=in05.fsx FSIMODE=PIPE SCFLAGS="--warnaserror+ --test:ErrorRanges --flaterrors" COMPILE_ONLY=1 # in05.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"in05.fsx"|])>]
    let ``in05_fsx`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 10, Col 9, Line 10, Col 10, "The types 'unit, int' do not support the operator '+'")
            (Error 1, Line 10, Col 14, Line 10, Col 20, "The types 'unit, int' do not support the operator '+'")
        ]

    // SOURCE=MutableLocals01.fs SCFLAGS="--warnon:3180 --optimize+ --test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MutableLocals01.fs"|])>]
    let ``MutableLocals01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:3370"; "--test:ErrorRanges"]
        |> compileAndRun
        |> shouldSucceed

    // SOURCE=W_TypeInferforGenericType.fs SCFLAGS="--test:ErrorRanges"	# W_TypeInferforGenericType.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_TypeInferforGenericType.fs"|])>]
    let ``W_TypeInferforGenericType_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 64, Line 10, Col 32, Line 10, Col 33, "This construct causes code to be less generic than indicated by the type annotations. The type variable 'b has been constrained to be type ''a'.")
        ]

