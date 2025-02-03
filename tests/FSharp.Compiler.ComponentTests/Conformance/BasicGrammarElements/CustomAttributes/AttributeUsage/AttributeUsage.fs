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

    [<Collection(nameof NotThreadSafeResourceCollection)>]
    module TimeCritical =
        // SOURCE=AssemblyVersion03.fs                          # AssemblyVersion03.fs
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
    let ``AttributeTargetsIsMethod01_fs 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=AttributeTargetsIsMethod01.fs				# AttributeTargetsIsMethod01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsMethod01.fs"|])>]
    let ``AttributeTargetsIsMethod01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=AttributeTargetsIsProperty.fs	# AttributeTargetsIsProperty.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsProperty.fs"|])>]
    let ``AttributeTargetsIsProperty_fs 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=AttributeTargetsIsProperty.fs	# AttributeTargetsIsProperty.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsProperty.fs"|])>]
    let ``AttributeTargetsIsProperty_fs`` compilation =
        compilation
        |> verifyCompile
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
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 21, Col 21, Line 21, Col 22, "This attribute is not valid for use on this language element")
            (Error 842, Line 24, Col 21, Line 24, Col 29, "This attribute is not valid for use on this language element")
            (Error 842, Line 27, Col 7, Line 27, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 18, Col 7, Line 18, Col 8, "This attribute is not valid for use on this language element")
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
        
    // SOURCE=E_AttributeTargetIsField01.fs					# E_AttributeTargetIsField01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsField01.fs"|])>]
    let ``E_AttributeTargetIsField01_fs 8_0`` compilation =
        compilation
        |> withLangVersion80
        |> withOptions ["--nowarn:25"]
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargetIsField01.fs					# E_AttributeTargetIsField01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsField01.fs"|])>]
    let ``E_AttributeTargetIsField01_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:25"]
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 9, Col 3, Line 9, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 12, Col 3, Line 12, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 15, Col 3, Line 15, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 18, Col 3, Line 18, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 21, Col 3, Line 21, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 24, Col 3, Line 24, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 27, Col 3, Line 27, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 30, Col 3, Line 30, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 33, Col 3, Line 33, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 36, Col 3, Line 36, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 39, Col 3, Line 39, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 42, Col 3, Line 42, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 45, Col 3, Line 45, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 49, Col 3, Line 49, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 56, Col 3, Line 56, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 64, Col 3, Line 64, Col 12, "This attribute is not valid for use on this language element")
            (Error 842, Line 66, Col 7, Line 66, Col 16, "This attribute is not valid for use on this language element")
        ]
        
    // SOURCE=E_AttributeTargetIsField02.fs					# E_AttributeTargetIsField02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsField02.fs"|])>]
    let ``E_AttributeTargetIsField02_fs 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> withOptions ["--nowarn:25"]
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargetIsField02.fs					# E_AttributeTargetIsField02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsField02.fs"|])>]
    let ``E_AttributeTargetIsField02_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:25"]
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargetIsMethod02.fs					# E_AttributeTargetIsMethod02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsMethod02.fs"|])>]
    let ``E_AttributeTargetIsMethod02_fs 8_0`` compilation =
        compilation
        |> withLangVersion80
        |> withOptions ["--nowarn:25"]
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargetIsMethod02.fs					# E_AttributeTargetIsMethod02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsMethod02.fs"|])>]
    let ``E_AttributeTargetIsMethod02_fs`` compilation =
        compilation
        |> withLangVersion90
        |> withOptions ["--nowarn:25"]
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 9, Col 3, Line 9, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 12, Col 3, Line 12, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 15, Col 3, Line 15, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 18, Col 3, Line 18, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 21, Col 3, Line 21, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 24, Col 3, Line 24, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 26, Col 7, Line 26, Col 17, "This attribute is not valid for use on this language element")
            (Error 842, Line 28, Col 3, Line 28, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 31, Col 3, Line 31, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 34, Col 3, Line 34, Col 13, "This attribute is not valid for use on this language element")
            (Error 842, Line 39, Col 3, Line 39, Col 13, "This attribute is not valid for use on this language element")
        ]
        
    // SOURCE=E_AttributeTargetIsMethod03.fs					# E_AttributeTargetIsMethod03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsMethod03.fs"|])>]
    let ``E_AttributeTargetIsMethod03_fs 8_0`` compilation =
        compilation
        |> withLangVersion80
        |> withOptions ["--nowarn:25"]
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargetIsMethod03.fs					# E_AttributeTargetIsMethod03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsMethod03.fs"|])>]
    let ``E_AttributeTargetIsMethod03_fs`` compilation =
        compilation
        |> withLangVersion90
        |> withOptions ["--nowarn:25"]
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 12, Col 6, Line 12, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 15, Col 6, Line 15, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 18, Col 6, Line 18, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 20, Col 10, Line 20, Col 20, "This attribute is not valid for use on this language element")
            (Error 842, Line 22, Col 6, Line 22, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 25, Col 6, Line 25, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 28, Col 6, Line 28, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 31, Col 6, Line 31, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 34, Col 6, Line 34, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 37, Col 6, Line 37, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 39, Col 10, Line 39, Col 20, "This attribute is not valid for use on this language element")
            (Error 842, Line 41, Col 6, Line 41, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 44, Col 6, Line 44, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 47, Col 6, Line 47, Col 16, "This attribute is not valid for use on this language element")
            (Error 842, Line 52, Col 6, Line 52, Col 16, "This attribute is not valid for use on this language element")
        ]

    // SOURCE=E_AttributeTargetIsMethod04.fs					# E_AttributeTargetIsMethod04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsMethod04.fs"|])>]
    let ``E_AttributeTargetIsMethod04_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:25"]
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 10, Col 3, Line 10, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 13, Col 3, Line 13, Col 15, "This attribute is not valid for use on this language element")
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
        
    // SOURCE=AttributeTargetIsStruct.fs 	# AttributeTargetIsStruct.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsStruct.fs"|])>]
    let ``AttributeTargetIsStruct_fs 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=AttributeTargetIsStruct.fs 	# AttributeTargetIsStruct.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsStruct.fs"|])>]
    let ``AttributeTargetIsStruct_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=AttributeTargetIsClass.fs 	# AttributeTargetIsClass.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsClass.fs"|])>]
    let ``AttributeTargetIsClass_fs 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=AttributeTargetIsClass.fs 	# AttributeTargetIsClass.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsClass.fs"|])>]
    let ``AttributeTargetIsClass_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargetIsStruct.fs 	# E_AttributeTargetIsStruct.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsStruct.fs"|])>]
    let ``E_AttributeTargetIsStruct_fs 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargetIsStruct.fs 	# E_AttributeTargetIsStruct.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsStruct.fs"|])>]
    let ``E_AttributeTargetIsStruct_fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 13, Col 3, Line 13, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 19, Col 3, Line 19, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 22, Col 11, Line 22, Col 22, "This attribute is not valid for use on this language element")
            (Error 842, Line 25, Col 3, Line 25, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 34, Col 3, Line 34, Col 18, "This attribute is not valid for use on this language element")
            (Error 842, Line 35, Col 3, Line 35, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 40, Col 3, Line 40, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 41, Col 3, Line 41, Col 18, "This attribute is not valid for use on this language element")
            (Error 842, Line 49, Col 3, Line 49, Col 18, "This attribute is not valid for use on this language element")
            (Error 842, Line 50, Col 3, Line 50, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 53, Col 3, Line 53, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 54, Col 3, Line 54, Col 18, "This attribute is not valid for use on this language element")
        ]

    // SOURCE=E_AttributeTargetIsClass.fs 	# E_AttributeTargetIsClass.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsClass.fs"|])>]
    let ``E_AttributeTargetIsClass_fs 8_0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargetIsClass.fs 	# E_AttributeTargetIsClass.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsClass.fs"|])>]
    let ``E_AttributeTargetIsClass_fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 13, Col 3, Line 13, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 19, Col 3, Line 19, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 22, Col 10, Line 22, Col 22, "This attribute is not valid for use on this language element")
        ]
        
    // SOURCE=E_AttributeTargetIsClass01.fs 	# E_AttributeTargetIsClass01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsClass01.fs"|])>]
    let ``E_AttributeTargetIsClass01_fs 8_0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargetIsClass01.fs 	# E_AttributeTargetIsClass01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsClass01.fs"|])>]
    let ``E_AttributeTargetIsClass01_fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 7, Col 3, Line 7, Col 18, "This attribute is not valid for use on this language element")
            (Error 842, Line 10, Col 10, Line 10, Col 25, "This attribute is not valid for use on this language element")
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
        
    // SOURCE=E_AttributeTargetIsField03.fs	# E_AttributeTargetIsField03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsField03.fs"|])>]
    let ``E_AttributeTargetIsField03_fs 8_0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 14, Col 5, Line 14, Col 15, "This attribute is not valid for use on this language element")
        ]
    
    // SOURCE=E_AttributeTargetIsField03.fs	# E_AttributeTargetIsField03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsField03.fs"|])>]
    let ``E_AttributeTargetIsField03_fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 14, Col 5, Line 14, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 15, Col 5, Line 15, Col 25, "This attribute is not valid for use on this language element")
        ]
        
    // SOURCE=E_AttributeTargetIsProperty01.fs	# E_AttributeTargetIsField03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsProperty01.fs"|])>]
    let ``E_AttributeTargetIsProperty01_fs 8_0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
    
    // SOURCE=E_AttributeTargetIsProperty01.fs	# E_AttributeTargetIsField03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsProperty01.fs"|])>]
    let ``E_AttributeTargetIsProperty01_fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 14, Col 5, Line 14, Col 18, "This attribute is not valid for use on this language element")
            (Error 842, Line 15, Col 5, Line 15, Col 25, "This attribute is not valid for use on this language element")
        ]
        
    // SOURCE=E_AttributeTargetIsCtor01.fs	# E_AttributeTargetIsCtor01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsCtor01.fs"|])>]
    let ``E_AttributeTargetIsCtor01_fs 8_0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
    
    // SOURCE=E_AttributeTargetIsCtor01.fs	# E_AttributeTargetIsCtor01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsCtor01.fs"|])>]
    let ``E_AttributeTargetIsCtor01_fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 9, Col 15, Line 9, Col 27, "This attribute is not valid for use on this language element")
            (Error 842, Line 11, Col 16, Line 11, Col 28, "This attribute is not valid for use on this language element")
            (Error 842, Line 14, Col 15, Line 14, Col 27, "This attribute is not valid for use on this language element")
            (Error 842, Line 17, Col 16, Line 17, Col 28, "This attribute is not valid for use on this language element")
        ]
        
    // SOURCE=AttributeTargetsIsEnum01.fs	# AttributeTargetsIsEnum01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsEnum01.fs"|])>]
    let ``AttributeTargetsIsEnum01_fs 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
    
    // SOURCE=AttributeTargetsIsEnum01.fs	# AttributeTargetsIsEnum01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsEnum01.fs"|])>]
    let ``AttributeTargetsIsEnum01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargetIsEnum01.fs	# E_AttributeTargetIsEnum01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsEnum01.fs"|])>]
    let ``E_AttributeTargetIsEnum01_fs 8_0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
    
    // SOURCE=E_AttributeTargetIsEnum01.fs	# E_AttributeTargetIsEnum01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsEnum01.fs"|])>]
    let ``E_AttributeTargetIsEnum01_fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 19, Col 3, Line 19, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 20, Col 3, Line 20, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 21, Col 3, Line 21, Col 18, "This attribute is not valid for use on this language element")
            (Error 842, Line 22, Col 3, Line 22, Col 17, "This attribute is not valid for use on this language element")
        ]        

     // SOURCE=AttributeTargetsIsDelegate01.fs	# AttributeTargetsIsDelegate01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsDelegate01.fs"|])>]
    let ``AttributeTargetsIsDelegate01_fs 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
    
    // SOURCE=AttributeTargetsIsDelegate01.fs	# AttributeTargetsIsDelegate01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsDelegate01.fs"|])>]
    let ``AttributeTargetsIsDelegate01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargetIsDelegate01.fs	# E_AttributeTargetIsDelegate01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsDelegate01.fs"|])>]
    let ``E_AttributeTargetIsDelegate01_fs 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
    
    // SOURCE=E_AttributeTargetIsDelegate01.fs	# E_AttributeTargetIsDelegate01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsDelegate01.fs"|])>]
    let ``E_AttributeTargetsIsDelegate01_fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 19, Col 3, Line 19, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 20, Col 3, Line 20, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 21, Col 3, Line 21, Col 18, "This attribute is not valid for use on this language element")
            (Error 842, Line 22, Col 3, Line 22, Col 13, "This attribute is not valid for use on this language element")
        ]

    [<Fact>]
    let ``Type-level let bindings allowed to use attribute with Field target`` () =
        FSharp"""
module Foo
type InterruptibleLazy<'T> private (valueFactory: unit -> 'T) =
    [<VolatileField>]
    let mutable valueFactory = valueFactory
        """
        |> compile
        |> shouldSucceed
        
     // SOURCE=AttributeTargetsIsInterface.fs	# AttributeTargetsIsInterface.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsInterface.fs"|])>]
    let ``AttributeTargetsIsInterface_fs 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
    
    // SOURCE=AttributeTargetsIsInterface.fs	# AttributeTargetsIsInterface.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AttributeTargetsIsInterface.fs"|])>]
    let ``AttributeTargetsIsInterface_fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=E_AttributeTargetIsInterface.fs	# E_AttributeTargetIsInterface.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsInterface.fs"|])>]
    let ``E_AttributeTargetIsInterface_fs 8_0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
    
    // SOURCE=E_AttributeTargetIsInterface.fs	# E_AttributeTargetIsInterface.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsInterface.fs"|])>]
    let ``E_AttributeTargetIsInterface_fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 11, Col 3, Line 11, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 14, Col 3, Line 14, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 18, Col 3, Line 18, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 19, Col 3, Line 19, Col 15, "This attribute is not valid for use on this language element")
        ]

    // SOURCE= E_AttributeTargetIsClass02.fs	# E_AttributeTargetIsClass02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsClass02.fs"|])>]
    let ``E_AttributeTargetIsClass02_fs 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
    
    // SOURCE=E_AttributeTargetIsClass02.fs	# E_AttributeTargetIsClass02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AttributeTargetIsClass02.fs"|])>]
    let ``E_AttributeTargetIsClass02_fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 842, Line 15, Col 3, Line 15, Col 18, "This attribute is not valid for use on this language element");
            (Error 842, Line 16, Col 3, Line 16, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 20, Col 3, Line 20, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 21, Col 3, Line 21, Col 18, "This attribute is not valid for use on this language element")
            (Error 842, Line 26, Col 3, Line 26, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 27, Col 3, Line 27, Col 18, "This attribute is not valid for use on this language element")
            (Error 842, Line 34, Col 3, Line 34, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 35, Col 3, Line 35, Col 18, "This attribute is not valid for use on this language element")
            (Error 842, Line 43, Col 3, Line 43, Col 18, "This attribute is not valid for use on this language element")
            (Error 842, Line 44, Col 3, Line 44, Col 15, "This attribute is not valid for use on this language element")
            (Error 842, Line 47, Col 3, Line 47, Col 14, "This attribute is not valid for use on this language element")
            (Error 842, Line 48, Col 3, Line 48, Col 18, "This attribute is not valid for use on this language element")
        ]
        
    // SOURCE= CLIMutableAttribute01.fs	# CLIMutableAttribute01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CLIMutableAttribute01.fs"|])>]
    let ``CLIMutableAttribute01 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
    
    // SOURCE=CLIMutableAttribute01.fs	# CLIMutableAttribute01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CLIMutableAttribute01.fs"|])>]
    let ``CLIMutableAttribute01 90`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE= E_CLIMutableAttribute.fs	# E_CLIMutableAttribute.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_CLIMutableAttribute.fs"|])>]
    let ``E_CLIMutableAttribute 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3132, Line 4, Col 8, Line 4, Col 16, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 7, Col 8, Line 7, Col 16, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 10, Col 8, Line 10, Col 20, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 13, Col 8, Line 13, Col 17, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 16, Col 8, Line 16, Col 15, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 19, Col 8, Line 19, Col 19, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 22, Col 8, Line 22, Col 17, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 25, Col 8, Line 25, Col 18, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
        ]
    
    // SOURCE=E_CLIMutableAttribute.fs	# E_CLIMutableAttribute.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_CLIMutableAttribute.fs"|])>]
    let ``E_CLIMutableAttribute 90`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3132, Line 4, Col 8, Line 4, Col 16, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 7, Col 8, Line 7, Col 16, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 10, Col 8, Line 10, Col 20, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 13, Col 8, Line 13, Col 17, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 16, Col 8, Line 16, Col 15, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 19, Col 8, Line 19, Col 19, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 22, Col 8, Line 22, Col 17, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
            (Error 3132, Line 25, Col 8, Line 25, Col 18, "This type definition may not have the 'CLIMutable' attribute. Only record types may have this attribute.")
        ]

    // SOURCE= E_AllowNullLiteral.fs	# E_AllowNullLiteral.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AllowNullLiteral.fs"|])>]
    let ``E_AllowNullLiteral 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 935, Line 15, Col 10, Line 15, Col 11, "Types with the 'AllowNullLiteral' attribute may only inherit from or implement types which also allow the use of the null literal")
            (Error 934, Line 27, Col 10, Line 27, Col 11, "Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute")
            (Error 934, Line 30, Col 10, Line 30, Col 11, "Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute")
            (Error 934, Line 33, Col 10, Line 33, Col 11, "Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute")
            (Error 934, Line 36, Col 10, Line 36, Col 11, "Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute")
            (Error 934, Line 39, Col 10, Line 39, Col 13, "Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute")
            (Error 842, Line 41, Col 7, Line 41, Col 23, "This attribute is not valid for use on this language element")
            (Error 842, Line 44, Col 7, Line 44, Col 23, "This attribute is not valid for use on this language element")
            (Error 935, Line 54, Col 10, Line 54, Col 11, "Types with the 'AllowNullLiteral' attribute may only inherit from or implement types which also allow the use of the null literal")
        ]
    
    // SOURCE=E_AllowNullLiteral.fs	# E_AllowNullLiteral.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AllowNullLiteral.fs"|])>]
    let ``E_AllowNullLiteral 90`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 935, Line 15, Col 10, Line 15, Col 11, "Types with the 'AllowNullLiteral' attribute may only inherit from or implement types which also allow the use of the null literal")
            (Error 934, Line 27, Col 10, Line 27, Col 11, "Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute")
            (Error 934, Line 30, Col 10, Line 30, Col 11, "Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute")
            (Error 934, Line 33, Col 10, Line 33, Col 11, "Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute")
            (Error 934, Line 36, Col 10, Line 36, Col 11, "Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute")
            (Error 934, Line 39, Col 10, Line 39, Col 13, "Records, union, abbreviations and struct types cannot have the 'AllowNullLiteral' attribute")
            (Error 842, Line 41, Col 7, Line 41, Col 23, "This attribute is not valid for use on this language element")
            (Error 842, Line 44, Col 7, Line 44, Col 23, "This attribute is not valid for use on this language element")
            (Error 935, Line 54, Col 10, Line 54, Col 11, "Types with the 'AllowNullLiteral' attribute may only inherit from or implement types which also allow the use of the null literal")
        ]
        
    // SOURCE= AllowNullLiteral01.fs	# AllowNullLiteral01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AllowNullLiteral01.fs"|])>]
    let ``AllowNullLiteral01 8.0`` compilation =
        compilation
        |> withLangVersion80
        |> verifyCompile
        |> shouldSucceed
    
    // SOURCE=AllowNullLiteral01.fs	# AllowNullLiteral01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AllowNullLiteral01.fs"|])>]
    let ``AllowNullLiteral01 90`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE= E_VolatileField.fs	# E_VolatileField.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_VolatileField.fs"|])>]
    let ``E_VolatileField 9.0`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 823, Line 4, Col 15, Line 4, Col 16, "The 'VolatileField' attribute may only be used on 'let' bindings in classes")
            (Error 823, Line 7, Col 11, Line 7, Col 12, "The 'VolatileField' attribute may only be used on 'let' bindings in classes")
            (Error 879, Line 7, Col 11, Line 7, Col 12, "Volatile fields must be marked 'mutable' and cannot be thread-static")
            (Error 823, Line 10, Col 7, Line 10, Col 9, "The 'VolatileField' attribute may only be used on 'let' bindings in classes")
            (Error 879, Line 10, Col 7, Line 10, Col 9, "Volatile fields must be marked 'mutable' and cannot be thread-static")
            (Error 823, Line 26, Col 17, Line 26, Col 18, "The 'VolatileField' attribute may only be used on 'let' bindings in classes")
            (Error 879, Line 14, Col 16, Line 14, Col 19, "Volatile fields must be marked 'mutable' and cannot be thread-static")
            (Error 879, Line 17, Col 16, Line 17, Col 17, "Volatile fields must be marked 'mutable' and cannot be thread-static")
            (Error 879, Line 20, Col 9, Line 20, Col 11, "Volatile fields must be marked 'mutable' and cannot be thread-static")
            (Error 879, Line 23, Col 9, Line 23, Col 10, "Volatile fields must be marked 'mutable' and cannot be thread-static")
        ]
    
    // SOURCE=E_VolatileField.fs	# E_VolatileField.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_VolatileField.fs"|])>]
    let ``E_VolatileField 90`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 823, Line 4, Col 15, Line 4, Col 16, "The 'VolatileField' attribute may only be used on 'let' bindings in classes")
            (Error 823, Line 7, Col 11, Line 7, Col 12, "The 'VolatileField' attribute may only be used on 'let' bindings in classes")
            (Error 879, Line 7, Col 11, Line 7, Col 12, "Volatile fields must be marked 'mutable' and cannot be thread-static")
            (Error 823, Line 10, Col 7, Line 10, Col 9, "The 'VolatileField' attribute may only be used on 'let' bindings in classes")
            (Error 879, Line 10, Col 7, Line 10, Col 9, "Volatile fields must be marked 'mutable' and cannot be thread-static")
            (Error 823, Line 26, Col 17, Line 26, Col 18, "The 'VolatileField' attribute may only be used on 'let' bindings in classes")
            (Error 879, Line 14, Col 16, Line 14, Col 19, "Volatile fields must be marked 'mutable' and cannot be thread-static")
            (Error 879, Line 17, Col 16, Line 17, Col 17, "Volatile fields must be marked 'mutable' and cannot be thread-static")
            (Error 879, Line 20, Col 9, Line 20, Col 11, "Volatile fields must be marked 'mutable' and cannot be thread-static")
            (Error 879, Line 23, Col 9, Line 23, Col 10, "Volatile fields must be marked 'mutable' and cannot be thread-static")
        ]
        
    // SOURCE= VolatileField01.fs	# VolatileField01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"VolatileField01.fs"|])>]
    let ``VolatileField01 9.0`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldSucceed
    
    // SOURCE=VolatileField01.fs	# VolatileField01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"VolatileField01.fs"|])>]
    let ``VolatileField01 90`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE= E_SealedAttribute01.fs	# E_SealedAttribute01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SealedAttribute01.fs"|])>]
    let ``E_SealedAttribute01 9.0`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 942, Line 2, Col 6, Line 2, Col 31, "Struct types are always sealed")
            (Error 948, Line 8, Col 6, Line 8, Col 24, "Interface types cannot be sealed")
            (Error 942, Line 14, Col 6, Line 14, Col 33, "Delegate types are always sealed")
        ]
    
    // SOURCE=E_SealedAttribute01.fs	# E_SealedAttribute01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SealedAttribute01.fs"|])>]
    let ``E_SealedAttribute01 90`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 942, Line 2, Col 6, Line 2, Col 31, "Struct types are always sealed")
            (Error 948, Line 8, Col 6, Line 8, Col 24, "Interface types cannot be sealed")
            (Error 942, Line 14, Col 6, Line 14, Col 33, "Delegate types are always sealed")
        ]

    // SOURCE= E_StructLayout01.fs	# E_StructLayout01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_StructLayout01.fs"|])>]
    let ``E_StructLayout01 9.0`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 937, Line 2, Col 6, Line 2, Col 8, "Only structs and classes without primary constructors may be given the 'StructLayout' attribute")
            (Error 937, Line 7, Col 6, Line 7, Col 8, "Only structs and classes without primary constructors may be given the 'StructLayout' attribute")
            (Error 937, Line 11, Col 6, Line 11, Col 8, "Only structs and classes without primary constructors may be given the 'StructLayout' attribute")
            (Error 937, Line 14, Col 6, Line 14, Col 8, "Only structs and classes without primary constructors may be given the 'StructLayout' attribute")
            (Error 937, Line 17, Col 6, Line 17, Col 8, "Only structs and classes without primary constructors may be given the 'StructLayout' attribute")
        ]

    // SOURCE=E_StructLayout01.fs	# E_StructLayout01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_StructLayout01.fs"|])>]
    let ``E_StructLayout01 90`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 937, Line 2, Col 6, Line 2, Col 8, "Only structs and classes without primary constructors may be given the 'StructLayout' attribute")
            (Error 937, Line 7, Col 6, Line 7, Col 8, "Only structs and classes without primary constructors may be given the 'StructLayout' attribute")
            (Error 937, Line 11, Col 6, Line 11, Col 8, "Only structs and classes without primary constructors may be given the 'StructLayout' attribute")
            (Error 937, Line 14, Col 6, Line 14, Col 8, "Only structs and classes without primary constructors may be given the 'StructLayout' attribute")
            (Error 937, Line 17, Col 6, Line 17, Col 8, "Only structs and classes without primary constructors may be given the 'StructLayout' attribute")
        ]

#if NETCOREAPP
    let missingConstructorRepro =
        """
open System.Text.Json.Serialization

type internal ApplicationTenantJsonDerivedTypeAttribute () =
    inherit JsonDerivedTypeAttribute (typeof<ApplicationTenant>, "a")

// --------------------------------------------------------------------------
// IMPORTANT: Read ReadMe before modifying this сlass and any referenced types
// --------------------------------------------------------------------------
and [<ApplicationTenantJsonDerivedType>]
    ApplicationTenant
    [<JsonConstructor>] (id, name, loginProvider, allowedDomains, authorizedTenants, properties) =

    member _.Id = ""
        """

    [<InlineData("8.0")>]
    [<InlineData("9.0")>]
    [<Theory>]
    let ``Regression for - F# 9 compiler cannot find constructor for attribute`` langVersion =
        FSharp missingConstructorRepro
        |> withLangVersion langVersion
        |> verifyCompile
        |> shouldSucceed
#endif

    [<Fact>] // Regression for https://github.com/dotnet/fsharp/issues/14304
    let ``Construct an object with default and params parameters using parameterless constructor`` () =
        Fsx """
open System
open System.Runtime.InteropServices

type DefaultAndParams([<Optional; DefaultParameterValue(1)>]x: int, [<ParamArray>] value: string[]) =
    inherit Attribute()

type ParamsOnly([<ParamArray>] value: string[]) =
    inherit Attribute()

type DefaultOnly([<Optional; DefaultParameterValue(1)>]x: int) =
    inherit Attribute()

[<DefaultAndParams>]
type Q1 = struct end

[<DefaultAndParams(x = 1)>] // ok
type Q11 = struct end

[<DefaultAndParams(value = [||])>] // ok
type Q12 = struct end

[<ParamsOnly>]
type Q2 = struct end

[<DefaultOnly>]
type Q3 = struct end
        """
        |> typecheck
        |> shouldSucceed