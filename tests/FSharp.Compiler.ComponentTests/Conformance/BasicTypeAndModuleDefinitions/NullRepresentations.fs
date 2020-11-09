// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module NullRepresentations =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/NullRepresentations)
    //<Expects id="FS0043" status="error">The type 'RecType' does not have 'null' as a proper value</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/NullRepresentations", Includes=[|"E_NullInvalidForFSTypes01.fs"|])>]
    let ``NullRepresentations - E_NullInvalidForFSTypes01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0043
        |> withDiagnosticMessageMatches "The type 'RecType' does not have 'null' as a proper value"

