// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module UnionTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"Overload_Equals.fs"|])>]
    let ``UnionTypes - Overload_Equals.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"Overload_GetHashCode.fs"|])>]
    let ``UnionTypes - Overload_GetHashCode.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/UnionTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/UnionTypes", Includes=[|"Overload_ToString.fs"|])>]
    let ``UnionTypes - Overload_ToString.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

