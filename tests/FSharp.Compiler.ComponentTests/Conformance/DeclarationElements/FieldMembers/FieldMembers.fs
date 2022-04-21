// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module FieldMembers =

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

    // SOURCE=DefaultValue01.fs	# DefaultValue01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"DefaultValue01.fs"|])>]
    let ``DefaultValue01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_StaticField01.fs	# E_StaticField01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_StaticField01.fs"|])>]
    let ``E_StaticField01_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 880, Line 11, Col 20, Line 11, Col 33, "Uninitialized 'val' fields must be mutable and marked with the '[<DefaultValue>]' attribute. Consider using a 'let' binding instead of a 'val' field.")
            (Error 881, Line 11, Col 20, Line 11, Col 33, "Static 'val' fields in types must be mutable, private and marked with the '[<DefaultValue>]' attribute. They are initialized to the 'null' or 'zero' value for their type. Consider also using a 'static let mutable' binding in a class type.")
        ]

    // SOURCE=E_StaticField02a.fs	# E_StaticField02a.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_StaticField02a.fs"|])>]
    let ``E_StaticField02a_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 881, Line 7, Col 32, Line 7, Col 49, "Static 'val' fields in types must be mutable, private and marked with the '[<DefaultValue>]' attribute. They are initialized to the 'null' or 'zero' value for their type. Consider also using a 'static let mutable' binding in a class type.")
            (Error 881, Line 16, Col 32, Line 16, Col 49, "Static 'val' fields in types must be mutable, private and marked with the '[<DefaultValue>]' attribute. They are initialized to the 'null' or 'zero' value for their type. Consider also using a 'static let mutable' binding in a class type.")
        ]

    // SOURCE=StaticField01.fs		# StaticField01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StaticField01.fs"|])>]
    let ``StaticField01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=StaticField02.fs		# StaticField02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StaticField02.fs"|])>]
    let ``StaticField02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Staticfield03.fs		# Staticfield03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Staticfield03.fs"|])>]
    let ``StaticField03_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed


