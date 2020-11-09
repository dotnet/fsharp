// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.ControlFlowExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module SimpleFor =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/SimpleFor)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ControlFlowExpressions/SimpleFor", Includes=[|"Downto01.fs"|])>]
    let ``SimpleFor - Downto01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/SimpleFor)
    //<Expects id="FS0001" span="(9,27-9,41)" status="error">This expression was expected to have type.    'int'    .but here has type.    'float'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ControlFlowExpressions/SimpleFor", Includes=[|"E_ForRequiresInt01.fs"|])>]
    let ``SimpleFor - E_ForRequiresInt01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'int'    .but here has type.    'float'"

