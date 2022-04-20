// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.CustomAttributes

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AttributeUsage =

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

    // SOURCE=AssemblyVersion01.fs							# AssemblyVersion01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyVersion01.fs"|])>]
    let ``AssemblyVersion01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=AssemblyVersion02.fs							# AssemblyVersion02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyVersion02.fs"|])>]
    let ``AssemblyVersion02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=AssemblyVersion03.fs							# AssemblyVersion03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyVersion03.fs"|])>]
    let ``AssemblyVersion03_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:52"]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=AssemblyVersion04.fs							# AssemblyVersion04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyVersion04.fs"|])>]
    let ``AssemblyVersion04_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:52"]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=AttributeTargetsIsCtor01.fs				# AttributeTargetsIsCtor01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsCtor01.fs"|])>]
    let ``AttributeTargetsIsCtor01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=AttributeTargetsIsMethod01.fs				# AttributeTargetsIsMethod01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsMethod01.fs"|])>]
    let ``AttributeTargetsIsMethod01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=ConditionalAttribute.fs					# ConditionalAttribute.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ConditionalAttribute.fs"|])>]
    let ``ConditionalAttribute_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_AttributeTargets01.fs					# E_AttributeTargets01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargets01.fs"|])>]
    let ``E_AttributeTargets01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 21, Col 21, Line 21, Col 22, "This attribute is not valid for use on this language element")
            (Error 842, Line 24, Col 28, Line 24, Col 29, "This attribute is not valid for use on this language element")
            (Error 842, Line 27, Col 15, Line 27, Col 16, "This attribute is not valid for use on this language element")
        ]

    // SOURCE=E_AttributeTargets02.fs					# E_AttributeTargets02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargets02.fs"|])>]
    let ``E_AttributeTargets02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 14, Col 17, Line 14, Col 34, "This attribute is not valid for use on this language element")
            (Error 842, Line 24, Col 14, Line 24, Col 29, "This attribute is not valid for use on this language element")
            (Error 842, Line 29, Col 25, Line 29, Col 40, "This attribute is not valid for use on this language element")
        ]

    // SOURCE=E_ConditionalAttribute.fs SCFLAGS="--test:ErrorRanges"	# E_ConditionalAttribute.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ConditionalAttribute.fs"|])>]
    let ``E_ConditionalAttribute_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1213, Line 10, Col 6, Line 10, Col 9, "Attribute 'System.Diagnostics.ConditionalAttribute' is only valid on methods or attribute classes")
        ]

    // SOURCE=E_RequiresExplicitTypeArguments01.fs SCFLAGS="--test:ErrorRanges"	# E_RequiresExplicitTypeArguments01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_RequiresExplicitTypeArguments01.fs"|])>]
    let ``E_RequiresExplicitTypeArguments01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 685, Line 14, Col 1, Line 14, Col 6, "The generic function 'Foo' must be given explicit type argument(s)")
            (Error 685, Line 26, Col 1, Line 26, Col 6, "The generic function 'Foo' must be given explicit type argument(s)")
            (Error 685, Line 28, Col 1, Line 28, Col 6, "The generic function 'Foo' must be given explicit type argument(s)")
        ]

    // SOURCE=E_RequiresExplicitTypeArguments02.fs SCFLAGS="--test:ErrorRanges"	# E_RequiresExplicitTypeArguments02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_RequiresExplicitTypeArguments02.fs"|])>]
    let ``E_RequiresExplicitTypeArguments02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 685, Line 20, Col 5, Line 20, Col 10, "The generic function 'Foo' must be given explicit type argument(s)")
        ]

    // #	SOURCE=E_WithBitwiseAnd01.fsx SCFLAGS="--test:ErrorRanges -a"	# E_WithBitwiseAnd01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_WithBitwiseAnd01.fsx"|])>]
    let ``E_WithBitwiseAnd01_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 267, Line 7, Col 25, Line 7, Col 91, "This is not a valid constant expression or custom attribute value")
            (Warning 839, Line 12, Col 3, Line 12, Col 6, "Unexpected condition in imported assembly: failed to decode AttributeUsage attribute")
        ]

    // SOURCE=E_WithBitwiseOr01.fsx  SCFLAGS="--test:ErrorRanges -a"	# E_WithBitwiseOr01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_WithBitwiseOr01.fsx"|])>]
    let ``E_WithBitwiseOr01_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 12, Col 3, Line 12, Col 6, "This attribute is not valid for use on this language element")
        ]

    // SOURCE=MarshalAsAttribute.fs					# MarshalAsAttribute.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MarshalAsAttribute.fs"|])>]
    let ``MarshalAsAttribute_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=RequiresExplicitTypeArguments01.fs					# RequiresExplicitTypeArguments01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RequiresExplicitTypeArguments01.fs"|])>]
    let ``RequiresExplicitTypeArguments01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=RequiresExplicitTypeArguments02.fs					# RequiresExplicitTypeArguments02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RequiresExplicitTypeArguments02.fs"|])>]
    let ``RequiresExplicitTypeArguments02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=W_AssemblyVersion01.fs SCFLAGS="--test:ErrorRanges"	# W_AssemblyVersion01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_AssemblyVersion01.fs"|])>]
    let ``W_AssemblyVersion01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 2003, Line 4, Col 46, Line 4, Col 59, "The attribute System.Reflection.AssemblyVersionAttribute specified version '1.2.3.4.5.6', but this value is invalid and has been ignored")
        ]

    // SOURCE=W_AssemblyVersion02.fs SCFLAGS="--test:ErrorRanges"	# W_AssemblyVersion02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_AssemblyVersion02.fs"|])>]
    let ``W_AssemblyVersion02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 2003, Line 4, Col 46, Line 4, Col 55, "The attribute System.Reflection.AssemblyVersionAttribute specified version '1.2.*.4', but this value is invalid and has been ignored")
        ]

    // SOURCE=WithBitwiseOr02a.fsx   SCFLAGS=-a			# WithBitwiseOr02a.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"WithBitwiseOr02a.fsx"|])>]
    let ``WithBitwiseOr02a_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=WithBitwiseOr02b.fsx   SCFLAGS=-a			# WithBitwiseOr02b.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"WithBitwiseOr02b.fsx"|])>]
    let ``WithBitwiseOr02b_fsx`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=X_AssemblyVersion01.fs SCFLAGS="--test:ErrorRanges"	# X_AssemblyVersion01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"X_AssemblyVersion01.fs"|])>]
    let ``X_AssemblyVersion01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 2003, Line 5, Col 50, Line 5, Col 61, "The attribute System.Reflection.AssemblyFileVersionAttribute specified version '9.8.7.6.5', but this value is invalid and has been ignored")
        ]

    // SOURCE=X_AssemblyVersion02.fs SCFLAGS="--test:ErrorRanges"	# X_AssemblyVersion02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"X_AssemblyVersion02.fs"|])>]
    let ``X_AssemblyVersion02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 2003, Line 5, Col 59, Line 5, Col 68, "The attribute System.Reflection.AssemblyFileVersionAttribute specified version '9.8.*.6', but this value is invalid and has been ignored")
        ]


