// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module And =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/And)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/And", Includes=[|"andPattern01.fs"|])>]
    let ``And - andPattern01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/And)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/And", Includes=[|"andPattern02.fs"|])>]
    let ``And - andPattern02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/And)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/And", Includes=[|"andPattern03.fs"|])>]
    let ``And - andPattern03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/And)
    //<Expects id="FS0038" status="error">'x' is bound twice in this pattern</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/And", Includes=[|"E_IdentBoundTwice.fs"|])>]
    let ``And - E_IdentBoundTwice.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0038
        |> withDiagnosticMessageMatches "'x' is bound twice in this pattern"

