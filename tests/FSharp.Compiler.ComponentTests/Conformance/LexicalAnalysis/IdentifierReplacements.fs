// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module IdentifierReplacements =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifierReplacements)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifierReplacements", Includes=[|"Line01.fs"|])>]
    let ``IdentifierReplacements - Line01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifierReplacements)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifierReplacements", Includes=[|"Line02.fs"|])>]
    let ``IdentifierReplacements - Line02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/IdentifierReplacements)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/IdentifierReplacements", Includes=[|"SourceFile01.fs"|])>]
    let ``IdentifierReplacements - SourceFile01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

