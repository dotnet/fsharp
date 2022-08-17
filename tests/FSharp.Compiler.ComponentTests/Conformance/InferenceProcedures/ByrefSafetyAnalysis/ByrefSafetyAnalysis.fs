// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ByrefSafetyAnalysis =

    // SOURCE=E_ByrefAsArrayElement.fs SCFLAGS="--test:ErrorRanges"                       # E_ByrefAsArrayElement.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ByrefAsArrayElement.fs"|])>]
    let``E_ByrefAsArrayElement_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3300, Line 5, Col 18, Line 5, Col 19, "The parameter 'f' has an invalid type '(byref<int> -> 'a)'. This is not permitted by the rules of Common IL.")
            (Error 424, Line 7, Col 6, Line 7, Col 13, "The address of an array element cannot be used at this point")
            (Error 412, Line 9, Col 19, Line 9, Col 20, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 11, Col 19, Line 11, Col 20, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_ByrefAsGenericArgument01.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefAsGenericArgument01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ByrefAsGenericArgument01.fs"|])>]
    let``E_ByrefAsGenericArgument01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 431, Line 9, Col 5, Line 9, Col 9, "A byref typed value would be stored here. Top-level let-bound byref values are not permitted.")
            (Error 412, Line 9, Col 5, Line 9, Col 9, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 9, Col 30, Line 9, Col 32, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // NoMT SOURCE=ByrefInFSI1.fsx FSIMODE=PIPE COMPILE_ONLY=1                            # ByrefInFSI1.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ByrefInFSI1.fsx"|])>]
    let``ByrefInFSI1_fsx`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=E_ByrefUsedInInnerLambda01.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefUsedInInnerLambda01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ByrefUsedInInnerLambda01.fs"|])>]
    let``E_ByrefUsedInInnerLambda01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 12, Col 34, Line 12, Col 48, "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

    // SOURCE=E_ByrefUsedInInnerLambda02.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefUsedInInnerLambda02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ByrefUsedInInnerLambda02.fs"|])>]
    let``E_ByrefUsedInInnerLambda02_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 11, Col 24, Line 11, Col 55, "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

    // SOURCE=E_ByrefUsedInInnerLambda03.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefUsedInInnerLambda03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ByrefUsedInInnerLambda03.fs"|])>]
    let``E_ByrefUsedInInnerLambda03_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 11, Col 24, Line 11, Col 60, "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

    // SOURCE=E_SetFieldToByref01.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SetFieldToByref01.fs"|])>]
    let``E_SetFieldToByref01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 412, Line 11, Col 17, Line 11, Col 27, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 437, Line 10, Col 6, Line 10, Col 9, "A type would store a byref typed value. This is not permitted by Common IL.")
            (Error 412, Line 11, Col 50, Line 11, Col 54, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_SetFieldToByref02.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SetFieldToByref02.fs"|])>]
    let``E_SetFieldToByref02_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 431, Line 8, Col 9, Line 8, Col 17, "A byref typed value would be stored here. Top-level let-bound byref values are not permitted.")
        ]
    // SOURCE=E_SetFieldToByref03.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SetFieldToByref03.fs"|])>]
    let``E_SetFieldToByref03_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1178, Line 8, Col 6, Line 8, Col 21, "The struct, record or union type 'RecordWithByref' is not structurally comparable because the type 'byref<int>' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'RecordWithByref' to clarify that the type is not comparable")
            (Error 412, Line 8, Col 25, Line 8, Col 26, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 437, Line 8, Col 6, Line 8, Col 21, "A type would store a byref typed value. This is not permitted by Common IL.")
        ]

    // SOURCE=E_SetFieldToByref04.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SetFieldToByref04.fs"|])>]
    let``E_SetFieldToByref04_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 412, Line 14, Col 28, Line 14, Col 37, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 421, Line 14, Col 29, Line 14, Col 30, "The address of the variable 'x' cannot be used at this point")
            (Error 412, Line 19, Col 20, Line 19, Col 53, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_SetFieldToByref05.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SetFieldToByref05.fs"|])>]
    let``E_SetFieldToByref05_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1178, Line 8, Col 6, Line 8, Col 17, "The struct, record or union type 'DUWithByref' is not structurally comparable because the type 'byref<int>' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'DUWithByref' to clarify that the type is not comparable")
            (Error 412, Line 9, Col 18, Line 9, Col 18, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 437, Line 8, Col 6, Line 8, Col 17, "A type would store a byref typed value. This is not permitted by Common IL.")
            (Error 412, Line 9, Col 7, Line 9, Col 8, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_FirstClassFuncTakesByref.fs SCFLAGS="--test:ErrorRanges --flaterrors"     # E_FirstClassFuncTakesByref.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_FirstClassFuncTakesByref.fs"|])>]
    let``E_FirstClassFuncTakesByref_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 8, Col 12, Line 8, Col 16, "This expression was expected to have type\n    'byref<'a>'    \nbut here has type\n    'int ref'    ")
        ]

    // SOURCE=E_StaticallyResolvedByRef01.fs SCFLAGS="--test:ErrorRanges"                 # E_StaticallyResolvedByRef01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_StaticallyResolvedByRef01.fs"|])>]
    let``E_StaticallyResolvedByRef01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
#if NETCOREAPP
            (Error 43, Line 11, Col 11, Line 11, Col 12, "The member or object constructor 'TryParse' does not take 1 argument(s). An overload was found taking 4 arguments.")
#else
            (Error 43, Line 11, Col 11, Line 11, Col 12, "The member or object constructor 'TryParse' does not take 1 argument(s). An overload was found taking 2 arguments.")
#endif
        ]

    // SOURCE=UseByrefInLambda01.fs                                                       # UseByrefInLambda01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"UseByrefInLambda01.fs"|])>]
    let``UseByrefInLambda01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

