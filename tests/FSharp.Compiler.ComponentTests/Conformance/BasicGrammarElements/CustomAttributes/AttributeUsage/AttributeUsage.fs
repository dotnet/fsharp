// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CustomAttributes_AttributeUsage =

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
            (Error 842, Line 24, Col 21, Line 24, Col 29, "This attribute is not valid for use on this language element")
            (Error 842, Line 27, Col 7, Line 27, Col 16, "This attribute is not valid for use on this language element")
        ]

    // SOURCE=E_AttributeTargets02.fs					# E_AttributeTargets02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargets02.fs"|])>]
    let ``E_AttributeTargets02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 14, Col 7, Line 14, Col 34, "This attribute is not valid for use on this language element")
            (Error 842, Line 24, Col 7, Line 24, Col 36, "This attribute is not valid for use on this language element")
            (Error 842, Line 29, Col 15, Line 29, Col 47, "This attribute is not valid for use on this language element")
        ]
        
    // SOURCE=E_AttributeTargets03.fs					# E_AttributeTargets03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargets03.fs"|])>]
    let ``E_AttributeTargets03_fs`` compilation =
        compilation
        |> withLangVersion80
        |> withOptions ["--nowarn:25"]
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargets03.fs					# E_AttributeTargets03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargets03.fs"|])>]
    let ``E_AttributeTargets03_fs preview`` compilation =
        compilation
        |> withLangVersionPreview
        |> withOptions ["--nowarn:25"]
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 8, Col 3, Line 8, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 11, Col 3, Line 11, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 14, Col 3, Line 14, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 17, Col 3, Line 17, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 20, Col 3, Line 20, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 102, Col 3, Line 102, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 106, Col 3, Line 106, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 110, Col 3, Line 110, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 113, Col 3, Line 113, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 116, Col 3, Line 116, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 121, Col 3, Line 121, Col 13, "This attribute is not valid for use on this language element")
        ]
        
    // SOURCE=E_AttributeTargets04.fs					# E_AttributeTargets04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargets04.fs"|])>]
    let ``E_AttributeTargets04_fs`` compilation =
        compilation
        |> withLangVersion80
        |> withOptions ["--nowarn:25"]
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargets04.fs					# E_AttributeTargets04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargets04.fs"|])>]
    let ``E_AttributeTargets04_fs preview`` compilation =
        compilation
        |> withLangVersionPreview
        |> withOptions ["--nowarn:25"]
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 23, Col 3, Line 23, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 26, Col 3, Line 26, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 29, Col 3, Line 29, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 32, Col 3, Line 32, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 35, Col 3, Line 35, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 38, Col 3, Line 38, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 41, Col 3, Line 41, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 44, Col 3, Line 44, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 47, Col 3, Line 47, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 50, Col 3, Line 50, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 53, Col 3, Line 53, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 64, Col 3, Line 64, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 70, Col 3, Line 70, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 77, Col 3, Line 77, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 83, Col 3, Line 83, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 95, Col 3, Line 95, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 104, Col 7, Line 104, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 108, Col 7, Line 108, Col 16, "This attribute is not valid for use on this language element")
        ]
        
        // SOURCE=E_AttributeTargets05.fs					# E_AttributeTargets05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargets05.fs"|])>]
    let ``E_AttributeTargets05_fs`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargets05.fs					# E_AttributeTargets05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargets05.fs"|])>]
    let ``E_AttributeTargets05_fs preview`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 20, Col 6, Line 20, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 23, Col 6, Line 23, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 32, Col 6, Line 32, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 35, Col 6, Line 35, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 38, Col 6, Line 38, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 40, Col 10, Line 40, Col 19, "This attribute is not valid for use on this language element")
            (Error 842, Line 52, Col 6, Line 52, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 55, Col 6, Line 55, Col 16, "This attribute is not valid for use on this language element")
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


