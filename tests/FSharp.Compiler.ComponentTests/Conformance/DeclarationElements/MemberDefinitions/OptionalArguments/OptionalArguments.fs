// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.MemberDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module OptionalArguments =

    let csTestLib =
        CSharpFromPath (__SOURCE_DIRECTORY__ ++ "TestLib.cs")
        |> withName "CSLibraryWithAttributes"

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

    // SOURCE=E_OptionalNamedArgs.fs SCFLAGS="-r:TestLib.dll" PRECMD="\$CSC_PIPE /t:library TestLib.cs"	# E_OptionalNamedArgs.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_OptionalNamedArgs.fs"|])>]
    let ``E_OptionalNamedArgs_fs`` compilation =
        compilation
        |> withReferences [csTestLib]
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 7, Col 42, Line 7, Col 48, "This expression was expected to have type\n    'string option'    \nbut here has type\n    'string'    ")
        ]

    // SOURCE=NullOptArgsFromCS.fs   SCFLAGS="-r:TestLib.dll" PRECMD="\$CSC_PIPE /t:library TestLib.cs"	# NullOptArgsFromCS.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NullOptArgsFromCS.fs"|])>]
    let ``NullOptArgsFromCS_fs`` compilation =
        compilation
        |> withReferences [csTestLib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=NullOptArgsFromVB.fs   SCFLAGS="-r:TestLibVB.dll" PRECMD="\$VBC_PIPE /t:library TestLibVB.vb"	# NullOptArgsFromVB.fs
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NullOptArgsFromVB.fs"|])>]
    //let ``NullOptArgsFromVB_fs`` compilation =
    //    compilation
    //    |> verifyCompileAndRun
    //    |> shouldSucceed

    // SOURCE=OptionalArgOnAbstract01.fs			# OptionalArgOnAbstract01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OptionalArgOnAbstract01.fs"|])>]
    let ``OptionalArgOnAbstract01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SanityOptArgsFromCS.fs SCFLAGS="-r:TestLib.dll" PRECMD="\$CSC_PIPE /t:library TestLib.cs"	# SanityOptArgsFromCS.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityOptArgsFromCS.fs"|])>]
    let ``SanityOptArgsFromCS_fs`` compilation =
        compilation
        |> withReferences [csTestLib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_SanityCheck.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SanityCheck.fs"|])>]
    let ``E_SanityCheck_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1212, Line 7, Col 22, Line 7, Col 32, "Optional arguments must come at the end of the argument list, after any non-optional arguments")
            (Error 39, Line 8, Col 5, Line 8, Col 8, "The type 'Foo' does not define the field, constructor or member 'Bar'.")
        ]

    // SOURCE=E_SanityCheck02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SanityCheck02.fs"|])>]
    let ``E_SanityCheck02_fs`` compilation =
        compilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1212, Line 7, Col 22, Line 7, Col 29, "Optional arguments must come at the end of the argument list, after any non-optional arguments")
            (Error 39, Line 8, Col 5, Line 8, Col 8, "The type 'Foo' does not define the field, constructor or member 'Bar'.")
        ]

    // SOURCE=optionalOfOptOptA.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"optionalOfOptOptA.fs"|])>]
    let ``optionalOfOptOptA_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SanityCheck.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck.fs"|])>]
    let ``SanityCheck_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SanityCheck02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck02.fs"|])>]
    let ``SanityCheck02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SanityCheck03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck03.fs"|])>]
    let ``SanityCheck03_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed


