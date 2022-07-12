// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module UnionTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Overload_Equals.fs"|])>]
    let ``Overload_Equals.fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Overload_GetHashCode.fs"|])>]
    let ``Overload_GetHashCode.fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Overload_ToString.fs"|])>]
    let ``Overload_ToString_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore
