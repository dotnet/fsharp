// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.MemberDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NamedArguments =

    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compileAndRun

    // SOURCE=E_MisspeltParam01.fs		# E_MisspeltParam01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_MisspeltParam01.fs"|])>]
    let ``E_MisspeltParam01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 508, Line 5, Col 9, Line 5, Col 89, "No accessible member or object constructor named 'ProcessStartInfo' takes 0 arguments. The named argument 'Argument' doesn't correspond to any argument or settable return property for any overload.")
        ]

    // SOURCE=E_MustBePrefix.fs		# E_MustBePrefix.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_MustBePrefix.fs"|])>]
    let ``E_MustBePrefix_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 35, Line 16, Col 1, Line 16, Col 35, "This construct is deprecated: The unnamed arguments do not form a prefix of the arguments of the method called")
        ]

    // SOURCE=E_NonNamedAfterNamed.fs		# E_NonNamedAfterNamed.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_NonNamedAfterNamed.fs"|])>]
    let ``E_NonNamedAfterNamed_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 691, Line 9, Col 33, Line 9, Col 34, "Named arguments must appear after all other arguments")
        ]

    // SOURCE=E_NumParamMismatch01.fs		# E_NumParamMismatch01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_NumParamMismatch01.fs"|])>]
    let ``E_NumParamMismatch01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 941, Line 13, Col 28, Line 13, Col 38, "Accessibility modifiers are not permitted on overrides or interface implementations")
            (Error 497, Line 18, Col 1, Line 18, Col 32, "The member or object constructor 'NamedMeth1' requires 1 additional argument(s). The required signature is 'abstract IFoo.NamedMeth1: arg1: int * arg2: int * arg3: int * arg4: int -> float'.")
            (Error 500, Line 19, Col 1, Line 19, Col 43, "The member or object constructor 'NamedMeth1' requires 4 argument(s) but is here given 2 unnamed and 3 named argument(s). The required signature is 'abstract IFoo.NamedMeth1: arg1: int * arg2: int * arg3: int * arg4: int -> float'.")
        ]

    // SOURCE=E_ReusedParam.fs			# E_ReusedParam.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ReusedParam.fs"|])>]
    let ``E_ReusedParam_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 364, Line 7, Col 1, Line 7, Col 42, "The named argument 'param1' has been assigned more than one value")
            (Warning 20, Line 7, Col 1, Line 7, Col 42, "The result of this expression has type 'int' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // SOURCE=E_SyntaxErrors01.fs               SCFLAGS="--test:ErrorRanges"		# E_SyntaxErrors01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SyntaxErrors01.fs"|])>]
    let ``E_SyntaxErrors01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 8, Col 47, Line 8, Col 51, "The value or constructor 'arg1' is not defined.")
            (Error 39, Line 8, Col 54, Line 8, Col 58, "The value or constructor 'arg2' is not defined.")
            (Error 3, Line 12, Col 1, Line 12, Col 8, "This value is not a function and cannot be applied.")
        ]

    // SOURCE=genericNamedParams.fs		# genericNamedParams.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"genericNamedParams.fs"|])>]
    let ``genericNamedParams_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=mixNamedNonNamed.fs		# mixNamedNonNamed.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"mixNamedNonNamed.fs"|])>]
    let ``mixNamedNonNamed_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=PropSetAfterConstrn01.fs	# PropSetAfterConstrn01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PropSetAfterConstrn01.fs"|])>]
    let ``PropertySetterAfterConstruction01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=PropertySetterAfterConstruction01NamedExtensions.fs	# PropertySetterAfterConstruction01NamedExtensions.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PropSetAfterConstrn01NamedExt.fs"|])>]
    let ``PropertySetterAfterConstruction01NamedExtensions_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=PropSetAfterConstrn01NamedExtInherit.fs	# PropSetAfterConstrn01NamedExtInherit.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PropSetAfterConstrn01NamedExtInherit.fs"|])>]
    let ``PropertySetterAfterConstruction01NamedExtensionsInheritance_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=PropertySetterAfterConstruction01NamedExtensionsOptional.fs	# PropSetAfterConstrn01NamedExtOpt.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PropSetAfterConstrn01NamedExtOpt.fs"|])>]
    let ``PropertySetterAfterConstruction01NamedExtensionsOptional_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCEPropSetAfterConstrn02.fs	# PropSetAfterConstrn02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PropSetAfterConstrn02.fs"|])>]
    let ``PropertySetterAfterConstruction02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=PropSetAfterConstrn02NamedExt.fs	# PropSetAfterConstrn02NamedExt.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PropSetAfterConstrn02NamedExt.fs"|])>]
    let ``PropertySetterAfterConstruction02NamedExtensions_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=refLibsHaveNamedParams.fs	# refLibsHaveNamedParams.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"refLibsHaveNamedParams.fs"|])>]
    let ``refLibsHaveNamedParams_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SanityCheck.fs			# SanityCheck.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck.fs"|])>]
    let ``SanityCheck_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed
