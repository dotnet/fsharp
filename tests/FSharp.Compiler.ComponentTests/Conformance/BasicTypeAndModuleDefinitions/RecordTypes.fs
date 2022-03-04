// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module RecordTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"Overload_Equals.fs"|])>]
    let ``RecordTypes - Overload_Equals.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"Overload_GetHashCode.fs"|])>]
    let ``RecordTypes - Overload_GetHashCode.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"Overload_ToString.fs"|])>]
    let ``RecordTypes - Overload_ToString.fs - --warnaserror+ --nowarn:988`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/RecordTypes)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/RecordTypes", Includes=[|"UnitType01.fsx"|])>]
    let ``RecordTypes - UnitType01.fsx - -a`` compilation =
        compilation
        |> asFsx
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed
        |> ignore

