// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CustomAttributes_ImportedAttributes =

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

    // SOURCE=FieldOffset01.fs SCFLAGS="" PEVER=/MD 	# FieldOffset01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"FieldOffset01.fs"|])>]
    let ``FieldOffset01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed


