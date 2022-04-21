// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module MemberDefinitionsModule =

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

    // SOURCE="BasicMembers.fs"	# BasicMembers
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"BasicMembers.fs"|])>]
    let ``BasicMembers_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed


