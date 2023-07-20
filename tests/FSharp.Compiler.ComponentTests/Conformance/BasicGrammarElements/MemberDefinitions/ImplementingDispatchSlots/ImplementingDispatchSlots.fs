// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module MemberDefinitions_ImplementingDispatchSlots =

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

    // SOURCE=SanityCheck.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SanityCheck.fs"|])>]
    let ``SanityCheck_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed


