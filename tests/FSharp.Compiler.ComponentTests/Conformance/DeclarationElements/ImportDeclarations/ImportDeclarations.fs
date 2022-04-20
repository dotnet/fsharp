// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ImportDeclarations =

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

        // SOURCE=E_OpenTwice.fs        SCFLAGS="--warnaserror+ --test:ErrorRanges"		# E_OpenTwice.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OpenTwice.fs"|])>]
    let ``E_OpenTwice_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 892, Line 11, Col 6, Line 11, Col 39, "This declaration opens the module 'Microsoft.FSharp.Collections.List', which is marked as 'RequireQualifiedAccess'. Adjust your code to use qualified references to the elements of the module instead, e.g. 'List.map' instead of 'map'. This change will ensure that your code is robust as new constructs are added to libraries.")
            (Error 892, Line 12, Col 6, Line 12, Col 39, "This declaration opens the module 'Microsoft.FSharp.Collections.List', which is marked as 'RequireQualifiedAccess'. Adjust your code to use qualified references to the elements of the module instead, e.g. 'List.map' instead of 'map'. This change will ensure that your code is robust as new constructs are added to libraries.")
        ]

    // SOURCE=E_OpenUnknownNS.fs					# E_OpenUnknownNS.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OpenUnknownNS.fs"|])>]
    let ``E_OpenUnknownNS_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 7, Col 6, Line 7, Col 26, "The namespace or module 'SomeUnknownNamespace' is not defined.")
        ]

    // SOURCE=E_openEnum.fs       SCFLAGS="--test:ErrorRanges"		# E_openEnum.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_openEnum.fs"|])>]
    let ``E_openEnum_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 20, Col 9, Line 20, Col 10, "The value or constructor 'A' is not defined.")
            (Error 39, Line 27, Col 9, Line 27, Col 10, "The value or constructor 'B' is not defined.")
            (Error 39, Line 34, Col 9, Line 34, Col 10, "The value or constructor 'C' is not defined.")
        ]

    // SOURCE=E_openInTypeDecl.fs SCFLAGS="--test:ErrorRanges"		# E_openInTypeDecl.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_openInTypeDecl.fs"|])>]
    let ``E_openInTypeDecl_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 7, Col 5, Line 7, Col 9, "Unexpected keyword 'open' in member definition")
        ]

    // SOURCE=E_openModInFun.fs   SCFLAGS="--test:ErrorRanges"		# E_openModInFun.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_openModInFun.fs"|])>]
    let ``E_openModInFun_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 9, Col 5, Line 9, Col 9, "Unexpected keyword 'open' in binding. Expected incomplete structured construct at or before this point or other token.")
            (Error 10, Line 17, Col 9, Line 17, Col 13, "Unexpected keyword 'open' in binding")
            (Error 10, Line 23, Col 9, Line 23, Col 13, "Unexpected keyword 'open' in expression")
        ]

    // SOURCE=OpenNestedModule01.fs					# OpenNestedModule01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OpenNestedModule01.fs"|])>]
    let ``OpenNestedModule01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=W_OpenUnqualifiedNamespace01.fs				# W_OpenUnqualifiedNamespace01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_OpenUnqualifiedNamespace01.fs"|])>]
    let ``W_OpenUnqualifiedNamespace01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 893, Line 7, Col 6, Line 7, Col 13, "This declaration opens the namespace or module 'System.Collections.Generic' through a partially qualified path. Adjust this code to use the full path of the namespace. This change will make your code more robust as new constructs are added to the F# and CLI libraries.")
        ]

    // SOURCE=openDU.fs						# openDU.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"openDU.fs"|])>]
    let ``openDU_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=openSystem01.fs							# openSystem01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"openSystem01.fs"|])>]
    let ``openSystem01_fs`` compilation =
        compilation
        |> asFsx
        |> verifyCompileAndRun
        |> shouldSucceed


