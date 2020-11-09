// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures.NameResolution

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Misc =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/Misc)
    //<Expects id="FS0037" status="error" span="(9,6)">Duplicate definition of type, exception or module 'Foo`1'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/Misc", Includes=[|"E-DuplicateTypes01.fs"|])>]
    let ``Misc - E-DuplicateTypes01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0037
        |> withDiagnosticMessageMatches "Duplicate definition of type, exception or module 'Foo`1'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/Misc)
    //<Expects id="FS0812" span="(17,13-17,34)" status="notin">The syntax 'expr\.id' may only be used with record labels, properties and fields</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/Misc", Includes=[|"E_ClashingIdentifiersDU01.fs"|])>]
    let ``Misc - E_ClashingIdentifiersDU01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> withDiagnosticMessageMatches "The syntax 'expr\.id' may only be used with record labels, properties and fields"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/Misc)
    //<Expects id="FS0812" span="(17,13-17,34)" status="error">The syntax 'expr\.id' may only be used with record labels, properties and fields</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/Misc", Includes=[|"E_ClashingIdentifiersDU02.fs"|])>]
    let ``Misc - E_ClashingIdentifiersDU02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0812
        |> withDiagnosticMessageMatches "The syntax 'expr\.id' may only be used with record labels, properties and fields"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/NameResolution/Misc)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/InferenceProcedures/NameResolution/Misc", Includes=[|"recordlabels.fs"|])>]
    let ``Misc - recordlabels.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

