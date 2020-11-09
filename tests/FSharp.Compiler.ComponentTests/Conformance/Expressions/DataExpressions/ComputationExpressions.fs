// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.DataExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ComputationExpressions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"CombineResults01.fs"|])>]
    let ``ComputationExpressions - CombineResults01.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"ForLoop01.fs"|])>]
    let ``ComputationExpressions - ForLoop01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"Regressions01.fs"|])>]
    let ``ComputationExpressions - Regressions01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"MinMaxValuesInLoop01.fs"|])>]
    let ``ComputationExpressions - MinMaxValuesInLoop01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"MinMaxValuesInLoop02.fs"|])>]
    let ``ComputationExpressions - MinMaxValuesInLoop02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"CompExprMethods01.fs"|])>]
    let ``ComputationExpressions - CompExprMethods01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"CompExprMethods02.fs"|])>]
    let ``ComputationExpressions - CompExprMethods02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"CompExprMethods03.fs"|])>]
    let ``ComputationExpressions - CompExprMethods03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"CompExprMethods04.fs"|])>]
    let ``ComputationExpressions - CompExprMethods04.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"DifferentGenericBuilders.fs"|])>]
    let ``ComputationExpressions - DifferentGenericBuilders.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"BuilderViaExtMethods.fs"|])>]
    let ``ComputationExpressions - BuilderViaExtMethods.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"NonClassWorkflow01.fs"|])>]
    let ``ComputationExpressions - NonClassWorkflow01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"MutateBuilders.fs"|])>]
    let ``ComputationExpressions - MutateBuilders.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"RunAndDelay01.fs"|])>]
    let ``ComputationExpressions - RunAndDelay01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"Capacity01.fs"|])>]
    let ``ComputationExpressions - Capacity01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    //<Expects status="error" span="(18,3-19,14)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'Combine' method$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"E_MissingCombine.fs"|])>]
    let ``ComputationExpressions - E_MissingCombine.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0708
        |> withDiagnosticMessageMatches "This control construct may only be used if the computation expression builder defines a 'Combine' method$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    //<Expects status="error" span="(17,3-17,23)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'For' method$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"E_MissingFor.fs"|])>]
    let ``ComputationExpressions - E_MissingFor.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0708
        |> withDiagnosticMessageMatches "This control construct may only be used if the computation expression builder defines a 'For' method$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    //<Expects status="error" span="(16,3-16,16)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'Return' method$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"E_MissingReturn.fs"|])>]
    let ``ComputationExpressions - E_MissingReturn.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0708
        |> withDiagnosticMessageMatches "This control construct may only be used if the computation expression builder defines a 'Return' method$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    //<Expects status="error" span="(17,3-17,17)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'ReturnFrom' method$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"E_MissingReturnFrom.fs"|])>]
    let ``ComputationExpressions - E_MissingReturnFrom.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0708
        |> withDiagnosticMessageMatches "This control construct may only be used if the computation expression builder defines a 'ReturnFrom' method$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    //<Expects status="error" span="(18,3-18,6)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'TryFinally' method$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"E_MissingTryFinally.fs"|])>]
    let ``ComputationExpressions - E_MissingTryFinally.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0708
        |> withDiagnosticMessageMatches "This control construct may only be used if the computation expression builder defines a 'TryFinally' method$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    //<Expects status="error" span="(18,3-18,6)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'TryWith' method$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"E_MissingTryWith.fs"|])>]
    let ``ComputationExpressions - E_MissingTryWith.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0708
        |> withDiagnosticMessageMatches "This control construct may only be used if the computation expression builder defines a 'TryWith' method$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    //<Expects status="error" span="(17,3-17,18)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'Using' method$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"E_MissingUsing.fs"|])>]
    let ``ComputationExpressions - E_MissingUsing.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0708
        |> withDiagnosticMessageMatches "This control construct may only be used if the computation expression builder defines a 'Using' method$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    //<Expects status="error" span="(17,3-17,13)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'While' method$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"E_MissingWhile.fs"|])>]
    let ``ComputationExpressions - E_MissingWhile.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0708
        |> withDiagnosticMessageMatches "This control construct may only be used if the computation expression builder defines a 'While' method$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    //<Expects status="error" span="(16,3-16,15)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'Yield' method$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"E_MissingYield.fs"|])>]
    let ``ComputationExpressions - E_MissingYield.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0708
        |> withDiagnosticMessageMatches "This control construct may only be used if the computation expression builder defines a 'Yield' method$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    //<Expects status="error" span="(16,3-16,16)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'YieldFrom' method$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"E_MissingYieldFrom.fs"|])>]
    let ``ComputationExpressions - E_MissingYieldFrom.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0708
        |> withDiagnosticMessageMatches "This control construct may only be used if the computation expression builder defines a 'YieldFrom' method$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/ComputationExpressions)
    //<Expects status="error" span="(16,3-16,9)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'Zero' method$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/ComputationExpressions", Includes=[|"E_MissingZero.fs"|])>]
    let ``ComputationExpressions - E_MissingZero.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0708
        |> withDiagnosticMessageMatches "This control construct may only be used if the computation expression builder defines a 'Zero' method$"

