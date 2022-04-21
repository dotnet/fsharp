// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module InterfaceSpecificationsAndImplementations =

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

    // SOURCE=ConcreteUnitOnInterface01.fs		# ConcreteUnitOnInterface01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ConcreteUnitOnInterface01.fs"|])>]
    let ``ConcreteUnitOnInterface01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=GenericMethodsOnInterface01.fs		# GenericMethodsOnInterface01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GenericMethodsOnInterface01.fs"|])>]
    let ``GenericMethodsOnInterface01_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=GenericMethodsOnInterface02.fs		# GenericMethodsOnInterface02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GenericMethodsOnInterface02.fs"|])>]
    let ``GenericMethodsOnInterface02_fs`` compilation =
        compilation
        |> verifyCompileAndRun
        |> shouldSucceed


