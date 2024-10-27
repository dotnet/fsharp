// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module BindingExpressions =

    let verifyCompile compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> compile

    let verifyCompileAsExe compilation =
        compilation
        |> asExe
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
        |> verifyCompileAsExe
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 12, Col 13, Line 12, Col 14, "The type 'int' does not match the type 'unit'")
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
            (Error 1, Line 10, Col 9, Line 10, Col 10, "The type 'int' does not match the type 'unit'")
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
    
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UpperBindingPattern.fs"|])>]
    let ``UpperBindingPattern_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 3874, Line 11, Col 8, Line 11, Col 11, "Variable patterns should be lowercase.")
            (Warning 3874, Line 11, Col 12, Line 11, Col 15, "Variable patterns should be lowercase.")
            (Warning 3874, Line 13, Col 8, Line 13, Col 11, "Variable patterns should be lowercase.")
            (Warning 3874, Line 13, Col 12, Line 13, Col 15, "Variable patterns should be lowercase.")
            (Warning 3874, Line 22, Col 22, Line 22, Col 25, "Variable patterns should be lowercase.")
            (Warning 3874, Line 22, Col 27, Line 22, Col 30, "Variable patterns should be lowercase.")
            (Warning 3874, Line 24, Col 22, Line 24, Col 25, "Variable patterns should be lowercase.")
            (Warning 3874, Line 24, Col 26, Line 24, Col 29, "Variable patterns should be lowercase.")
            (Warning 3874, Line 32, Col 20, Line 32, Col 23, "Variable patterns should be lowercase.")
            (Warning 3874, Line 32, Col 25, Line 32, Col 28, "Variable patterns should be lowercase.")
            (Warning 3874, Line 34, Col 21, Line 34, Col 24, "Variable patterns should be lowercase.")
            (Warning 3874, Line 34, Col 25, Line 34, Col 28, "Variable patterns should be lowercase.")
            (Warning 3874, Line 42, Col 31, Line 42, Col 34, "Variable patterns should be lowercase.")
            (Warning 3874, Line 46, Col 5, Line 46, Col 8, "Variable patterns should be lowercase.")
            (Warning 3874, Line 53, Col 18, Line 53, Col 21, "Variable patterns should be lowercase.")
            (Warning 3874, Line 67, Col 9, Line 67, Col 14, "Variable patterns should be lowercase.")
            (Warning 3874, Line 73, Col 9, Line 73, Col 18, "Variable patterns should be lowercase.")
            (Warning 3874, Line 80, Col 9, Line 80, Col 18, "Variable patterns should be lowercase.")
            (Warning 3874, Line 87, Col 9, Line 87, Col 18, "Variable patterns should be lowercase.")
            (Warning 3874, Line 117, Col 37, Line 117, Col 40, "Variable patterns should be lowercase.")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UpperBindingPattern.fs"|])>]
    let ``UpperBindingPattern_fs preview`` compilation =
        compilation
        |> asExe
        |> withLangVersionPreview
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 3874, Line 3, Col 8, Line 3, Col 10, "Variable patterns should be lowercase.")
            (Warning 3874, Line 3, Col 11, Line 3, Col 13, "Variable patterns should be lowercase.")
            (Warning 3874, Line 5, Col 8, Line 5, Col 10, "Variable patterns should be lowercase.")
            (Warning 3874, Line 5, Col 11, Line 5, Col 13, "Variable patterns should be lowercase.")
            (Warning 3874, Line 7, Col 8, Line 7, Col 9, "Variable patterns should be lowercase.")
            (Warning 3874, Line 7, Col 10, Line 7, Col 11, "Variable patterns should be lowercase.")
            (Warning 3874, Line 11, Col 8, Line 11, Col 11, "Variable patterns should be lowercase.")
            (Warning 3874, Line 11, Col 12, Line 11, Col 15, "Variable patterns should be lowercase.")
            (Warning 3874, Line 13, Col 8, Line 13, Col 11, "Variable patterns should be lowercase.")
            (Warning 3874, Line 13, Col 12, Line 13, Col 15, "Variable patterns should be lowercase.")
            (Warning 3874, Line 16, Col 22, Line 16, Col 24, "Variable patterns should be lowercase.")
            (Warning 3874, Line 16, Col 26, Line 16, Col 28, "Variable patterns should be lowercase.")
            (Warning 3874, Line 18, Col 22, Line 18, Col 24, "Variable patterns should be lowercase.")
            (Warning 3874, Line 18, Col 25, Line 18, Col 27, "Variable patterns should be lowercase.")
            (Warning 3874, Line 20, Col 22, Line 20, Col 23, "Variable patterns should be lowercase.")
            (Warning 3874, Line 20, Col 24, Line 20, Col 25, "Variable patterns should be lowercase.")
            (Warning 3874, Line 22, Col 22, Line 22, Col 25, "Variable patterns should be lowercase.")
            (Warning 3874, Line 22, Col 27, Line 22, Col 30, "Variable patterns should be lowercase.")
            (Warning 3874, Line 24, Col 22, Line 24, Col 25, "Variable patterns should be lowercase.")
            (Warning 3874, Line 24, Col 26, Line 24, Col 29, "Variable patterns should be lowercase.")
            (Warning 3874, Line 26, Col 20, Line 26, Col 22, "Variable patterns should be lowercase.")
            (Warning 3874, Line 26, Col 24, Line 26, Col 26, "Variable patterns should be lowercase.")
            (Warning 3874, Line 28, Col 20, Line 28, Col 22, "Variable patterns should be lowercase.")
            (Warning 3874, Line 28, Col 23, Line 28, Col 25, "Variable patterns should be lowercase.")
            (Warning 3874, Line 30, Col 20, Line 30, Col 21, "Variable patterns should be lowercase.")
            (Warning 3874, Line 30, Col 22, Line 30, Col 23, "Variable patterns should be lowercase.")
            (Warning 3874, Line 32, Col 20, Line 32, Col 23, "Variable patterns should be lowercase.")
            (Warning 3874, Line 32, Col 25, Line 32, Col 28, "Variable patterns should be lowercase.")
            (Warning 3874, Line 34, Col 21, Line 34, Col 24, "Variable patterns should be lowercase.")
            (Warning 3874, Line 34, Col 25, Line 34, Col 28, "Variable patterns should be lowercase.")
            (Warning 49, Line 40, Col 17, Line 40, Col 20, "Match cases labels must be lowercase identifiers")
            (Warning 49, Line 42, Col 31, Line 42, Col 34, "Match cases labels must be lowercase identifiers")
            (Warning 49, Line 44, Col 32, Line 44, Col 34, "Match cases labels must be lowercase identifiers")
            (Warning 3874, Line 46, Col 5, Line 46, Col 8, "Variable patterns should be lowercase.")
            (Warning 3874, Line 49, Col 5, Line 49, Col 7, "Variable patterns should be lowercase.")
            (Warning 3874, Line 53, Col 18, Line 53, Col 21, "Variable patterns should be lowercase.")
            (Warning 3874, Line 57, Col 18, Line 57, Col 20, "Variable patterns should be lowercase.")
            (Warning 3874, Line 61, Col 6, Line 61, Col 8, "Variable patterns should be lowercase.")
            (Warning 3874, Line 67, Col 9, Line 67, Col 14, "Variable patterns should be lowercase.")
            (Warning 3874, Line 69, Col 19, Line 69, Col 24, "Variable patterns should be lowercase.")
            (Warning 3874, Line 69, Col 34, Line 69, Col 44, "Variable patterns should be lowercase.")
            (Warning 3874, Line 73, Col 9, Line 73, Col 18, "Variable patterns should be lowercase.")
            (Warning 3874, Line 80, Col 9, Line 80, Col 18, "Variable patterns should be lowercase.")
            (Warning 3874, Line 87, Col 9, Line 87, Col 18, "Variable patterns should be lowercase.")
            (Warning 3874, Line 95, Col 9, Line 95, Col 11, "Variable patterns should be lowercase.")
            (Warning 3874, Line 102, Col 9, Line 102, Col 11, "Variable patterns should be lowercase.")
            (Warning 3874, Line 109, Col 9, Line 109, Col 11, "Variable patterns should be lowercase.")
            (Warning 49, Line 117, Col 33, Line 117, Col 35, "Match cases labels must be lowercase identifiers")
            (Warning 49, Line 117, Col 37, Line 117, Col 40, "Match cases labels must be lowercase identifiers")
            (Warning 49, Line 119, Col 18, Line 119, Col 20, "Match cases labels must be lowercase identifiers");
            (Warning 49, Line 119, Col 22, Line 119, Col 24, "Match cases labels must be lowercase identifiers")
        ]
