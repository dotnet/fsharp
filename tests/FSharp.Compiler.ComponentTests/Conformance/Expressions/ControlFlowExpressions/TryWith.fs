// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.ControlFlowExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TryWith =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/TryWith)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ControlFlowExpressions/TryWith", Includes=[|"TryWith01.fs"|])>]
    let ``TryWith - TryWith01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/TryWith)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ControlFlowExpressions/TryWith", Includes=[|"TryWith02.fs"|])>]
    let ``TryWith - TryWith02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/TryWith)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ControlFlowExpressions/TryWith", Includes=[|"TryWith03.fs"|])>]
    let ``TryWith - TryWith03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/TryWith)
    //<Expects id="FS0413" status="error" span="(4,1)">Calls to 'reraise' may only occur directly in a handler of a try-with$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ControlFlowExpressions/TryWith", Includes=[|"E_RethrowOutsideWith01.fs"|])>]
    let ``TryWith - E_RethrowOutsideWith01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0413
        |> withDiagnosticMessageMatches "Calls to 'reraise' may only occur directly in a handler of a try-with$"

