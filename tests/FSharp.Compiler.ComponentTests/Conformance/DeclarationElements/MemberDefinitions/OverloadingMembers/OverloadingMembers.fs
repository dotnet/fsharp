// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.MemberDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module OverloadingMembers =

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

    // NOMONO,NoMT	SOURCE=ConsumeOverloadGenericMethods.fs SCFLAGS="-r:lib.dll" PRECMD="\$CSC_PIPE /t:library /reference:System.Core.dll lib.cs"	# ConsumeOverloadGenericMethods.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ConsumeOverloadGenericMethods.fs"|])>]
    let ``ConsumeOverloadGenericMethods_fs`` compilation =

        let lib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "lib.cs")
            |> withName "Library"

        compilation
        |> withReferences [lib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=diffNumArguments.fs							# diffNumArguments.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"diffNumArguments.fs"|])>]
    let ``diffNumArguments_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_InferredTypeNotUnique01.fs SCFLAGS="--test:ErrorRanges"			# E_InferredTypeNotUnique01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InferredTypeNotUnique01.fs"|])>]
    let ``E_InferredTypeNotUnique01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 438, Line 11, Col 17, Line 11, Col 20, "Duplicate method. The method 'Foo' has the same name and signature as another method in type 'SomeClass'.")
            (Error 438, Line 7, Col 17, Line 7, Col 20, "Duplicate method. The method 'Foo' has the same name and signature as another method in type 'SomeClass'.")
        ]

    // SOURCE=E_OperatorOverloading01.fs SCFLAGS="--test:ErrorRanges"		# E_OperatorOverloading01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OperatorOverloading01.fs"|])>]
    let ``E_OperatorOverloading01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1173, Line 17, Col 40, Line 17, Col 41, "Infix operator member '+' has 1 initial argument(s). Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...")
            (Warning 1174, Line 17, Col 40, Line 17, Col 41, "Infix operator member '+' has extra curried arguments. Expected a tuple of 2 arguments, e.g. static member (+) (x,y) = ...")
            (Warning 52, Line 17, Col 83, Line 17, Col 87, "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed")
            (Warning 52, Line 17, Col 90, Line 17, Col 94, "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed")
            (Warning 52, Line 17, Col 96, Line 17, Col 100, "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed")
            (Warning 52, Line 17, Col 103, Line 17, Col 107, "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed")
            (Error 43, Line 21, Col 13, Line 21, Col 14, "Method or object constructor 'op_Addition' not found")
        ]

    // SOURCE=E_OverloadCurriedFunc.fs		# E_OverloadCurriedFunc.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OverloadCurriedFunc.fs"|])>]
    let ``E_OverloadCurriedFunc_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 6, Col 7, Line 6, Col 17, "The type 'OverloadID' is not defined.")
            (Error 39, Line 8, Col 7, Line 8, Col 17, "The type 'OverloadID' is not defined.")
            (Error 39, Line 10, Col 7, Line 10, Col 17, "The type 'OverloadID' is not defined.")
            (Error 39, Line 12, Col 7, Line 12, Col 17, "The type 'OverloadID' is not defined.")
            (Error 816, Line 18, Col 1, Line 18, Col 33, "One or more of the overloads of this method has curried arguments. Consider redesigning these members to take arguments in tupled form.")
        ]

    // SOURCE=E_OverloadMismatch.fs		# E_OverloadMismatch.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OverloadMismatch.fs"|])>]
    let ``E_OverloadMismatch_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 366, Line 11, Col 13, Line 11, Col 17, "No implementation was given for 'abstract IFoo.Foo: TextReader -> 't'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        ]

    // SOURCE=E_ReturnGenericUnit01.fs							# E_ReturnGenericUnit01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ReturnGenericUnit01.fs"|])>]
    let ``E_ReturnGenericUnit01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 17, Line 21, Col 17, Line 21, Col 18, "The member 'M: unit -> unit' does not have the correct type to override the corresponding abstract method.")
        ]

    //// SOURCE=E_UnsolvableConstraints01.fs     SCFLAGS="--test:ErrorRanges"		# E_UnsolvableConstraints01.fs
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_UnsolvableConstraints01.fs"|])>]
    //let ``E_UnsolvableConstraints01_fs`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldFail
    //    |> withDiagnostics [
    //    ]

    // SOURCE=InferenceForLambdaArgs.fs						# InferenceForLambdaArgs.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InferenceForLambdaArgs.fs"|])>]
    let ``InferenceForLambdaArgs_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=NoOverloadIDSpecified.fs							# NoOverloadIDSpecified.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoOverloadIDSpecified.fs"|])>]
    let ``NoOverloadIDSpecified_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=NoWarningWhenDoingDispatchSlotInference01.fs SCFLAGS="--warnaserror+" COMPILE_ONLY=1	# NoWarningWhenDoingDispatchSlotInference01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoWarningWhenDoingDispatchSlotInference01.fs"|])>]
    let ``NoWarningWhenDoingDispatchSlotInference01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=NoWarningWhenOverloadingInSubClass01.fs SCFLAGS="--warnaserror"		# NoWarningWhenOverloadingInSubClass01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoWarningWhenOverloadingInSubClass01.fs"|])>]
    let ``NoWarningWhenOverloadingInSubClass01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=OnAllOverloads01.fs							# OnAllOverloads01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OnAllOverloads01.fs"|])>]
    let ``OnAllOverloads01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=OperatorOverloading01.fs						# OperatorOverloading01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OperatorOverloading01.fs"|])>]
    let ``OperatorOverloading01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=OperatorOverloading02.fs						# OperatorOverloading02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OperatorOverloading02.fs"|])>]
    let ``OperatorOverloading02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=OperatorOverloading03.fs						# OperatorOverloading03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OperatorOverloading03.fs"|])>]
    let ``OperatorOverloading03_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=OperatorOverloading04.fs						# OperatorOverloading04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OperatorOverloading04.fs"|])>]
    let ``OperatorOverloading04_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // NoMT	SOURCE=OverloadingAndExtensionMethodsForGenericTypes.fs				# OverloadingAndExtensionMethodsForGenericTypes.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OverloadingAndExtensionMethodsForGenericTypes.fs"|])>]
    let ``OverloadingAndExtensionMethodsForGenericTypes_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=reuseOverloadIDs.fs							# reuseOverloadIDs.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"reuseOverloadIDs.fs"|])>]
    let ``reuseOverloadIDs_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SanityCheck.fs								# SanityCheck.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck.fs"|])>]
    let ``SanityCheck_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=TieBreakerRule01a.fs	# TieBreakerRule01a.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TieBreakerRule01a.fs"|])>]
    let ``TieBreakerRule01a_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=TieBreakerRule01b.fs	# TieBreakerRule01b.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TieBreakerRule01b.fs"|])>]
    let ``TieBreakerRule01b_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=TieBreakerRule02.fs	# TieBreakerRule02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TieBreakerRule02.fs"|])>]
    let ``TieBreakerRule02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=TieBreakerRule03.fs	# TieBreakerRule03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TieBreakerRule03.fs"|])>]
    let ``TieBreakerRule03_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=TooGeneric.fs SCFLAGS="--define:TOO_GENERIC"	# TooGeneric.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TooGeneric.fs"|])>]
    let ``TooGeneric_fs`` compilation =
        compilation
        |> withDefines ["TOO_GENERIC"]
        |> verifyCompileAndRun
        |> shouldSucceed
