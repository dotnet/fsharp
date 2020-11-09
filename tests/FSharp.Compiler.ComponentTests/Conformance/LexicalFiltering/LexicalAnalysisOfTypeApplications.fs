// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalFiltering

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module LexicalAnalysisOfTypeApplications =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/LexicalAnalysisOfTypeApplications)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/LexicalAnalysisOfTypeApplications", Includes=[|"ComplexTypeApp01.fs"|])>]
    let ``LexicalAnalysisOfTypeApplications - ComplexTypeApp01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

