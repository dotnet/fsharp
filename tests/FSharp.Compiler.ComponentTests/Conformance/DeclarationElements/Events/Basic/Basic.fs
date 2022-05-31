// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.Events

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

    // SOURCE=CLIEvent01.fs		# CLIEvent01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CLIEvent01.fs"|])>]
    let ``CLIEvent01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=EventWithGenericTypeAsUnit01.fs	# EventWithGenericTypeAsUnit01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"EventWithGenericTypeAsUnit01.fs"|])>]
    let ``EventWithGenericTypeAsUnit01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=EventsOnInterface01.fs	# EventsOnInterface01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"EventsOnInterface01.fs"|])>]
    let ``EventsOnInterface01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Regression01.fs		# Regression01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Regression01.fs"|])>]
    let ``Regression01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Regression02.fs		# Regression02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Regression02.fs"|])>]
    let ``Regression02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Regression02b.fs		# Regression02b.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Regression02b.fs"|])>]
    let ``Regression02b_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

#if !NETSTANDARD && !NETCOREAPP
    // NoMT	SOURCE=Regression03.fsx COMPILE_ONLY=1 FSIMODE=PIPE SCFLAGS="--nologo"	# Regression03.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Regression03.fsx"|])>]
    let ``Regression03_fsx`` compilation =
        compilation
        |> asFsx
        |> verifyCompile
        |> shouldSucceed
#endif

    // SOURCE=SanityCheck.fs		# SanityCheck.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck.fs"|])>]
    let ``SanityCheck_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // NoMT	SOURCE=SanityCheck02.fs				# SanityCheck02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck02.fs"|])>]
    let ``SanityCheck02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

#if !NETCOREAPP && !NETSTANDARD
    // SOURCE=SanityCheck02.fs PEVER=/MD		# SanityCheck02.fs - /MD
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck02.fs"|])>]
    let ``SanityCheck02_fs_peverify`` compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> PEVerifier.verifyPEFile
        |> PEVerifier.shouldSucceed
#endif
