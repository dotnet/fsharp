// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ExceptionDefinition =

    // SOURCE=Abbreviation01.fsx                                                               # Abbreviation01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Abbreviation01.fsx"|])>]
    let``Abbreviation01_fsx`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=AbbreviationForSystemException.fsx                                               # AbbreviationForSystemException.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AbbreviationForSystemException.fsx"|])>]
    let``AbbreviationForSystemException_fsx`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=Abbreviation_SampleCodeFromSpec01.fsx                                            # Abbreviation_SampleCodeFromSpec01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Abbreviation_SampleCodeFromSpec01.fsx"|])>]
    let``Abbreviation_SampleCodeFromSpec01_fsx`` compilation =
        compilation
        |> asFsx
        |> asExe
        |> withOptions ["--nowarn:52"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=ActiveRecognizer.fsx                                                             # ActiveRecognizer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ActiveRecognizer.fsx"|])>]
    let``ActiveRecognizer_fsx`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=AddMethsProps01.fs                                                               # AddMethsProps01
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AddMethsProps01.fs"|])>]
    let``AddMethsProps01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=CatchWOTypecheck01.fs                                                            # CatchWOTypeCheck01
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CatchWOTypecheck01.fs"|])>]
    let``CatchWOTypecheck01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=EqualAndBoxing01.fs                                                              # EqualAndBoxing01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"EqualAndBoxing01.fs"|])>]
    let``EqualAndBoxing01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=ExceptionAsDerivedFromSystemException01.fsx                                      # ExceptionAsDerivedFromSystemException01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ExceptionAsDerivedFromSystemException01.fsx"|])>]
    let``ExceptionAsDerivedFromSystemException01_fsx`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=ExnAsDiscriminatedUnion01.fsx                                                    # ExnAsDiscriminatedUnion01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ExnAsDiscriminatedUnion01.fsx"|])>]
    let``ExnAsDiscriminatedUnion01_fsx`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=E_Abbreviation_NonMatchingObjConstructor.fsx SCFLAGS="--test:ErrorRanges"        # E_Abbreviation_NonMatchingObjConstructor.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Abbreviation_NonMatchingObjConstructor.fsx"|])>]
    let``E_Abbreviation_NonMatchingObjConstructor_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 920, Line 8, Col 1, Line 8, Col 28, "Abbreviations for Common IL exception types must have a matching object constructor")        ]

    // SOURCE=E_AssertionFailureExn.fs                     SCFLAGS="--test:ErrorRanges"        # E_AssertionFailureExn.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AssertionFailureExn.fs"|])>]
    let``E_AssertionFailureExn_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 11, Col 36, Line 11, Col 61, "The type 'AssertionFailureException' is not defined in 'Microsoft.FSharp.Core'.")
        ]

    // SOURCE=E_BeginWithUppercase01.fsx                   SCFLAGS="--test:ErrorRanges"        # E_BeginWithUppercase01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_BeginWithUppercase01.fsx"|])>]
    let``E_BeginWithUppercase01_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 53, Line 9, Col 1, Line 9, Col 19, "Discriminated union cases and exception labels must be uppercase identifiers")
            (Error 53, Line 10, Col 1, Line 10, Col 19, "Discriminated union cases and exception labels must be uppercase identifiers")
        ]

    // SOURCE=E_BeginWithUppercase02.fsx                   SCFLAGS="--test:ErrorRanges"        # E_BeginWithUppercase02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_BeginWithUppercase02.fsx"|])>]
    let``E_BeginWithUppercase02_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 8, Col 11, Line 8, Col 12, "Unexpected integer literal in exception definition. Expected identifier or other token.")
        ]

    // SOURCE=E_BeginWithUppercase03.fsx                   SCFLAGS="--test:ErrorRanges"        # E_BeginWithUppercase03.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_BeginWithUppercase03.fsx"|])>]
    let``E_BeginWithUppercase03_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 9, Col 11, Line 9, Col 14, "Unexpected string literal in exception definition. Expected identifier or other token.")
        ]

    // SOURCE=E_BeginWithUppercase04.fsx                   SCFLAGS="--test:ErrorRanges"        # E_BeginWithUppercase04.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_BeginWithUppercase04.fsx"|])>]
    let``E_BeginWithUppercase04_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 7, Col 11, Line 7, Col 12, "Unexpected reserved keyword in exception definition. Expected identifier or other token.")
        ]

    // SOURCE=E_DynamicInvocationNotSupported.fsx SCFLAGS=--test:ErrorRanges                   # E_DynamicInvocationNotSupported.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_DynamicInvocationNotSupported.fsx"|])>]
    let``E_DynamicInvocationNotSupported_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 6, Col 9, Line 6, Col 38, "The value or constructor 'DynamicInvocationNotSupported' is not defined.")
        ]

    // SOURCE=E_ExnAsDiscriminatedUnion01.fsx              SCFLAGS="--test:ErrorRanges"        # E_ExnAsDiscriminatedUnion01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ExnAsDiscriminatedUnion01.fsx"|])>]
    let``E_ExnAsDiscriminatedUnion01_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 11, Col 14, Line 11, Col 15, "Unexpected symbol ':' in implementation file")
        ]

    // SOURCE=E_ExnConstructorBadFieldName.fs              SCFLAGS="--test:ErrorRanges"        # E_ExnConstructorBadFieldName.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ExnConstructorBadFieldName.fs"|])>]
    let``E_ExnConstructorBadFieldName_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3174, Line 8, Col 17, Line 8, Col 19, "The exception 'AAA' does not have a field named 'V3'.")
            (Error 3174, Line 13, Col 7, Line 13, Col 9, "The exception 'AAA' does not have a field named 'V3'.")
        ]

    // SOURCE=E_ExnFieldConflictingName.fs                 SCFLAGS="--test:ErrorRanges"        # E_ExnFieldConflictingName.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ExnFieldConflictingName.fs"|])>]
    let``E_ExnFieldConflictingName_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3176, Line 6, Col 18, Line 6, Col 23, "Named field 'Data1' conflicts with autogenerated name for anonymous field.")
            (Error 3176, Line 8, Col 28, Line 8, Col 29, "Named field 'A' is used more than once.")
        ]

    // SOURCE=E_FieldNameUsedMulti.fs                      SCFLAGS="--test:ErrorRanges"        # E_FieldNameUsedMulti.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_FieldNameUsedMulti.fs"|])>]
    let``E_FieldNameUsedMulti_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3175, Line 8, Col 22, Line 8, Col 24, "Union case/exception field 'V1' cannot be used more than once.")
            (Error 3175, Line 13, Col 16, Line 13, Col 18, "Union case/exception field 'V2' cannot be used more than once.")
        ]

    // SOURCE=E_FieldMemberClash.fs                        SCFLAGS="--test:ErrorRanges"        # E_FieldMemberClash.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_FieldMemberClash.fs"|])>]
    let``E_FieldMemberClash_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 23, Line 9, Col 17, Line 9, Col 22, "The member 'Data0' can not be defined because the name 'Data0' clashes with the field 'Data0' in this type or module")
            (Error 23, Line 10, Col 17, Line 10, Col 22, "The member 'Data1' can not be defined because the name 'Data1' clashes with the field 'Data1' in this type or module")
            (Error 23, Line 11, Col 17, Line 11, Col 19, "The member 'V3' can not be defined because the name 'V3' clashes with the field 'V3' in this type or module")
        ]

    // SOURCE=E_GeneratedTypeName01.fsx                    SCFLAGS="--test:ErrorRanges"        # E_GeneratedTypeName01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_GeneratedTypeName01.fsx"|])>]
    let``E_GeneratedTypeName01_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1104, Line 8, Col 11, Line 8, Col 27, "Identifiers containing '@' are reserved for use in F# code generation")
            (Warning 1104, Line 10, Col 16, Line 10, Col 41, "Identifiers containing '@' are reserved for use in F# code generation")
            (Error 39, Line 10, Col 16, Line 10, Col 41, "The type 'Crazy@name.pException' is not defined. Maybe you want one of the following:\r\n   Crazy@name.p")
        ]

    // SOURCE=E_GeneratedTypeNameClash02.fsx               SCFLAGS="--test:ErrorRanges"        # E_GeneratedTypeNameClash02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_GeneratedTypeNameClash02.fsx"|])>]
    let``E_GeneratedTypeNameClash02_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 37, Line 8, Col 11, Line 8, Col 12, "Duplicate definition of type, exception or module 'EException'")
        ]

    // SOURCE=E_InheritException.fs                        SCFLAGS="--test:ErrorRanges"        # E_InheritException.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InheritException.fs"|])>]
    let``E_InheritException_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 945, Line 9, Col 5, Line 9, Col 24, "Cannot inherit a sealed type")
            (Error 1133, Line 9, Col 5, Line 9, Col 24, "No constructors are available for the type 'FSharpExn'")
        ]

    // SOURCE=E_MatchFailure.fsx                  SCFLAGS=--test:ErrorRanges                   # E_MatchFailure.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_MatchFailure.fsx"|])>]
    let``E_MatchFailure_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 6, Col 9, Line 6, Col 21, "The value or constructor 'MatchFailure' is not defined. Maybe you want one of the following:\r\n   MatchFailureException")
        ]

    // SOURCE=E_MustStartWithCap01.fs                                                          # E_MustStartWithCap01
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_MustStartWithCap01.fs"|])>]
    let``E_MustStartWithCap01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 53, Line 8, Col 1, Line 8, Col 39, "Discriminated union cases and exception labels must be uppercase identifiers")
        ]

    // SOURCE=E_Undefined.fsx                     SCFLAGS=--test:ErrorRanges                   # E_Undefined.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_Undefined.fsx"|])>]
    let``E_Undefined_fsx`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 6, Col 9, Line 6, Col 18, "The value or constructor 'Undefined' is not defined.")
        ]

    // SOURCE=GeneratedTypeNameNoClash01.fsx               SCFLAGS="--test:ErrorRanges"        # GeneratedTypeNameNoClash01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GeneratedTypeNameNoClash01.fsx"|])>]
    let``GeneratedTypeNameNoClash01_fsx`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=ImportCSharpException01.fsx                                                      # ImportCSharpException01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ImportCSharpException01.fsx"|])>]
    let``ImportCSharpException01_fsx`` compilation =

        let cSharpException =
            CSharpFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "CSharpException.cs"))
            |> withName "CSharpException"

        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> withReferences [cSharpException]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=LowercaseIdentifier01.fsx                                                        # LowercaseIdentifier01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"LowercaseIdentifier01.fsx"|])>]
    let``LowercaseIdentifier01_fsx`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=NamedFields01.fsx                                                                # NamedFields01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedFields01.fsx"|])>]
    let``NamedFields01_fsx`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"; "--nowarn:25"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=PatternMatch_SampleCodeFromSpec01.fsx                                            # PatternMatch_SampleCodeFromSpec01.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PatternMatch_SampleCodeFromSpec01.fsx"|])>]
    let``PatternMatch_SampleCodeFromSpec01_fsx`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=ReflectionAPI.fsx                                                                # ReflectionAPI.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ReflectionAPI.fsx"|])>]
    let``ReflectionAPI_fsx`` compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed
