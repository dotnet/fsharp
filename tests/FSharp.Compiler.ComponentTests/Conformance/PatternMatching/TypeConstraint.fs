// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TypeConstraint =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/TypeConstraint)
    //<Expects id="FS0583" span="(20,5-20,6)" status="error">Unmatched '\('$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/TypeConstraint", Includes=[|"E_typecontraint01.fs"|])>]
    let ``TypeConstraint - E_typecontraint01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0583
        |> withDiagnosticMessageMatches "Unmatched '\('$"

