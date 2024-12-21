// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

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
    [<Theory; FileInlineData("ConcreteUnitOnInterface01.fs")>]
    let ``ConcreteUnitOnInterface01_fs`` compilation =
        compilation
        |> getCompilation 
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=GenericMethodsOnInterface01.fs		# GenericMethodsOnInterface01.fs
    [<Theory; FileInlineData("GenericMethodsOnInterface01.fs")>]
    let ``GenericMethodsOnInterface01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=GenericMethodsOnInterface02.fs		# GenericMethodsOnInterface02.fs
    [<Theory; FileInlineData("GenericMethodsOnInterface02.fs")>]
    let ``GenericMethodsOnInterface02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed


