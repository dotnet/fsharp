// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ModuleAbbreviations =

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

    // SOURCE=E_AbbreviationOnNamespace01.fs SCFLAGS="--test:ErrorRanges"	# E_AbbreviationOnNamespace01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_AbbreviationOnNamespace01.fs"|])>]
    let ``E_AbbreviationOnNamespace01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 965, Line 11, Col 1, Line 11, Col 22, "The path 'System.IO' is a namespace. A module abbreviation may not abbreviate a namespace.")
            (Error 39, Line 13, Col 15, Line 13, Col 17, "The value, namespace, type or module 'IO' is not defined.")
            (Error 965, Line 16, Col 1, Line 16, Col 43, "The path 'System.Text.RegularExpressions' is a namespace. A module abbreviation may not abbreviate a namespace.")
            (Error 39, Line 18, Col 19, Line 18, Col 21, "The namespace or module 'rx' is not defined.")
            (Error 72, Line 21, Col 4, Line 21, Col 19, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")
        ]

    // SOURCE=E_InvalidAbbrevName01.fs  SCFLAGS="--test:ErrorRanges"		# E_InvalidAbbrevName01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidAbbrevName01.fs"|])>]
    let ``E_InvalidAbbrevName01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 5, Col 11, Line 5, Col 12, "Unexpected symbol '$' in definition. Expected incomplete structured construct at or before this point or other token.")
        ]

    // SOURCE=E_InvalidAbbrevName02.fs						# E_InvalidAbbrevName02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidAbbrevName02.fs"|])>]
    let ``E_InvalidAbbrevName02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 534, Line 5, Col 1, Line 5, Col 18, "A module abbreviation must be a simple name, not a path")
        ]

    // SOURCE=E_NameConflict01.fs						# E_NameConflict01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_NameConflict01.fs"|])>]
    let ``E_NameConflict01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 14, Col 12, Line 14, Col 15, "The field, constructor or member 'sum' is not defined.")
        ]

    // SOURCE=E_UseInsideFunc.fs SCFLAGS="--test:ErrorRanges"			# E_UseInsideFunc.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_UseInsideFunc.fs"|])>]
    let ``E_UseInsideFunc_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 7, Col 5, Line 7, Col 11, "Incomplete structured construct at or before this point in binding")
        ]

    // SOURCE=SanityCheck.fs							# SanityCheck.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck.fs"|])>]
    let ``SanityCheck_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SanityCheck02.fs							# SanityCheck02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck02.fs"|])>]
    let ``SanityCheck02_fs`` compilation =
        compilation
        |> withNoWarn 1104
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=useInsideModuleDef.fs						# useInsideModuleDef.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"useInsideModuleDef.fs"|])>]
    let ``useInsideModuleDef_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=useInsideNamespaceDef.fs COMPILE_ONLY=1				# useInsideNamespaceDef.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"useInsideNamespaceDef.fs"|])>]
    let ``useInsideNamespaceDef_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // NoMT	SOURCE=useInsideNamespaceDefExternal.fs PRECMD="\$FSC_PIPE -a useInsideNamespaceDefExternal_DLL.fs" SCFLAGS="-r:useInsideNamespaceDefExternal_DLL.dll"	# useInsideNamespaceDefExternal.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"useInsideNamespaceDefExternal.fs"|])>]
    let ``useInsideNamespaceDefExternal_fs`` compilation =

        let useInsideNamespaceDefExternal_DLL =
            FsFromPath(__SOURCE_DIRECTORY__ ++ "useInsideNamespaceDefExternal_DLL.fs")
            |> asLibrary
            |> withName "useInsideNamespaceDefExternal_DLL"

        compilation
        |> withReferences [useInsideNamespaceDefExternal_DLL]
        |> verifyCompileAndRun
        |> shouldSucceed
