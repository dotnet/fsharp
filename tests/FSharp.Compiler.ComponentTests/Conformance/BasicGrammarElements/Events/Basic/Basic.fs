// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

[<Collection(nameof NotThreadSafeResourceCollection)>]
module Events =

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
    [<Theory; FileInlineData("CLIEvent01.fs")>]
    let ``CLIEvent01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=EventWithGenericTypeAsUnit01.fs	# EventWithGenericTypeAsUnit01.fs
    [<Theory; FileInlineData("EventWithGenericTypeAsUnit01.fs")>]
    let ``EventWithGenericTypeAsUnit01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=EventsOnInterface01.fs	# EventsOnInterface01.fs
    [<Theory; FileInlineData("EventsOnInterface01.fs")>]
    let ``EventsOnInterface01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Regression01.fs		# Regression01.fs
    [<Theory; FileInlineData("Regression01.fs")>]
    let ``Regression01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Regression02.fs		# Regression02.fs
    [<Theory; FileInlineData("Regression02.fs")>]
    let ``Regression02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=Regression02b.fs		# Regression02b.fs
    [<Theory; FileInlineData("Regression02b.fs")>]
    let ``Regression02b_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

#if !NETSTANDARD && !NETCOREAPP
    // NoMT	SOURCE=Regression03.fsx COMPILE_ONLY=1 FSIMODE=PIPE SCFLAGS="--nologo"	# Regression03.fsx
    [<Theory; FileInlineData("Regression03.fsx")>]
    let ``Regression03_fsx`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> verifyCompile
        |> shouldSucceed
#endif

    // SOURCE=SanityCheck.fs		# SanityCheck.fs
    [<Theory; FileInlineData("SanityCheck.fs")>]
    let ``SanityCheck_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // NoMT	SOURCE=SanityCheck02.fs				# SanityCheck02.fs
    [<Theory; FileInlineData("SanityCheck02.fs")>]
    let ``SanityCheck02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

#if false && !NETCOREAPP && !NETSTANDARD
    // SOURCE=SanityCheck02.fs PEVER=/MD		# SanityCheck02.fs - /MD
    [<Theory; FileInlineData("SanityCheck02.fs")>]
    let ``SanityCheck02_fs_peverify`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> PEVerifier.verifyPEFile
        |> PEVerifier.shouldSucceed
#endif
