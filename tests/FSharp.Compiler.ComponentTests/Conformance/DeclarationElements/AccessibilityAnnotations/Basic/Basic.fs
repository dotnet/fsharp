// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.AccessibilityAnnotations

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Basic =

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

    //SOURCE=E_ExposeLessVisible01.fs                                                 # E_ExposeLessVisible01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ExposeLessVisible01.fs"|])>]
    let ``E_ExposeLessVisible01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 410, Line 5, Col 19, Line 5, Col 20, "The type 'A' is less accessible than the value, member or type 'x' it is used in.")
        ]

    //SOURCE=E_BaseIFaceLessAccessible01.fs SCFLAGS="-a --test:ErrorRanges"           # E_BaseIFaceLessAccessible01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_BaseIFaceLessAccessible01.fs"|])>]
    let ``E_BaseIFaceLessAccessible01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 410, Line 11, Col 8, Line 11, Col 23, "The type 'I1' is less accessible than the value, member or type 'IAmAnInterface1' it is used in.")
        ]

    //SOURCE=E_LocalLetBinding02.fs SCFLAGS="--test:ErrorRanges"                      # E_LocalLetBinding02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_LocalLetBinding02.fs"|])>]
    let ``E_LocalLetBinding02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 646, Line 7, Col 13, Line 7, Col 16, "Multiple visibility attributes have been specified for this identifier. 'let' bindings in classes are always private, as are any 'let' bindings inside expressions.")
        ]

    //SOURCE=E_privateThingsInaccessible.fs                                           # E_privateThingsInaccessible.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_PrivateThingsInaccessible.fs"|])>]
    let ``E_privateThingsInaccessible_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1094, Line 18, Col 17, Line 18, Col 41, "The value 'somePrivateField' is not accessible from this code location")
            (Error 1094, Line 19, Col 17, Line 19, Col 42, "The value 'somePrivateMethod' is not accessible from this code location")
            (Error 491, Line 23, Col 17, Line 23, Col 34, "The member or object constructor 'PrivateMethod' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
        ]

    //SOURCE=E_privateThingsInaccessible02.fs SCFLAGS="--test:ErrorRanges"            # E_privateThingsInaccessible02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_privateThingsInaccessible02.fs"|])>]
    let ``E_PrivateThingsInaccessible02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1092, Line 26, Col 19, Line 26, Col 32, "The type 'PrivateModule' is not accessible from this code location")
            (Error 1094, Line 26, Col 17, Line 26, Col 34, "The value 'x' is not accessible from this code location")
            (Error 1092, Line 27, Col 19, Line 27, Col 32, "The type 'PrivateModule' is not accessible from this code location")
            (Error 1094, Line 27, Col 17, Line 27, Col 34, "The value 'f' is not accessible from this code location")
            (Error 1094, Line 29, Col 17, Line 29, Col 20, "The value 'y' is not accessible from this code location")
            (Error 1094, Line 30, Col 17, Line 30, Col 20, "The value 'g' is not accessible from this code location")
        ]

    //SOURCE=E_privateThingsInaccessible03.fs SCFLAGS="--test:ErrorRanges"            # E_privateThingsInaccessible03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_privateThingsInaccessible03.fs"|])>]
    let ``E_PrivateThingsInaccessible03_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1092, Line 11, Col 15, Line 11, Col 28, "The type 'PrivateModule' is not accessible from this code location")
            (Error 1094, Line 11, Col 13, Line 11, Col 30, "The value 'x' is not accessible from this code location")
            (Error 39, Line 15, Col 13, Line 15, Col 26, "The value, namespace, type or module 'PrivateModule' is not defined.")
        ]

    //SOURCE=E_privateThingsInaccessible04.fs SCFLAGS="--test:ErrorRanges"            # E_privateThingsInaccessible04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_privateThingsInaccessible04.fs"|])>]
    let ``E_PrivateThingsInaccessible04_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 25, Col 17, Line 25, Col 30, "The value, namespace, type or module 'PrivateModule' is not defined.")
            (Error 39, Line 26, Col 17, Line 26, Col 30, "The value, namespace, type or module 'PrivateModule' is not defined.")
            (Error 39, Line 28, Col 17, Line 28, Col 18, "The value or constructor 'y' is not defined.")
            (Error 39, Line 29, Col 17, Line 29, Col 18, "The value or constructor 'g' is not defined.")
        ]

    //SOURCE=E_privateThingsInaccessible05.fs SCFLAGS="--test:ErrorRanges"            # E_privateThingsInaccessible05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_privateThingsInaccessible05.fs"|])>]
    let ``E_PrivateThingsInaccessible05_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1096, Line 11, Col 9, Line 11, Col 24, "The record, struct or class field 'foo' is not accessible from this code location")
        ]

    //SOURCE=E_PrivateImplicitCtor01.fs SCFLAGS="--test:ErrorRanges"                  # E_PrivateImplicitCtor01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_PrivateImplicitCtor01.fs"|])>]
    let ``E_PrivateImplicitCtor01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 509, Line 24, Col 10, Line 24, Col 18, "Method or object constructor 'C' not found")
        ]

    //SOURCE=E_ProtectedThingsInaccessible01.fs SCFLAGS="--test:ErrorRanges"          # E_ProtectedThingsInaccessible01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_ProtectedThingsInaccessible01.fs"|])>]
    let ``E_ProtectedThingsInaccessible01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 629, Line 11, Col 24, Line 11, Col 41, "Method 'MemberwiseClone' is not accessible from this code location")
        ]

    //SOURCE=E_MoreAccessibleBaseClass01.fs                                           # E_MoreAccessibleBaseClass01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_MoreAccessibleBaseClass01.fs"|])>]
    let ``E_MoreAccessibleBaseClass01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 410, Line 10, Col 6, Line 10, Col 8, "The type 'C1' is less accessible than the value, member or type 'C2' it is used in.")
        ]

    //SOURCE=E_MoreAccessibleBaseClass02.fs                                           # E_MoreAccessibleBaseClass02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_MoreAccessibleBaseClass02.fs"|])>]
    let ``E_MoreAccessibleBaseClass02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 410, Line 12, Col 6, Line 12, Col 7, "The type 'I2' is less accessible than the value, member or type 'D' it is used in.")
            (Error 410, Line 18, Col 6, Line 18, Col 25, "The type 'I3' is less accessible than the value, member or type 'IAmAnotherInterface' it is used in.")
        ]

    //SOURCE=InterfaceImplementationVisibility.fs                                     # InterfaceImplementationVisibility.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InterfaceImplementationVisibility.fs"|])>]
    let ``InterfaceImplementationVisibility_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=InternalizedIFaces02.fs SCFLAGS="-a --warnaserror"                       # InternalizedIFaces02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InternalizedIFaces02.fs"|])>]
    let ``InternalizedIFaces02_fs`` compilation =
        compilation
        |> withOptions ["--nowarn:1178";]
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=internalMethodsWorkCorrectly.fs                                          # InternalMethodsWorkCorrectly.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InternalMethodsWorkCorrectly.fs"|])>]
    let ``InternalMethodsWorkCorrectly_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=LessOrMoreAccessibleCode01.fs                                            # LessOrMoreAccessibleCode01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"LessOrMoreAccessibleCode01.fs"|])>]
    let ``LessOrMoreAccessibleCode01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    //SOURCE=LocalLetBinding01.fs SCFLAGS="-a --test:ErrorRanges"                     # LocalLetBinding01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"LocalLetBinding01.fs"|])>]
    let ``LocalLetBinding01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed
