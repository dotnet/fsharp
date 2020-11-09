// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TyperelatedExpressions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/Type-relatedExpressions", Includes=[|"rigidtypeannotation01.fs"|])>]
    let ``TyperelatedExpressions - rigidtypeannotation01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    //<Expects id="FS0001" span="(9,2-9,15)" status="error">This expression was expected to have type.    'seq<'a>'    .but here has type.    ''b list'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/Type-relatedExpressions", Includes=[|"E_rigidtypeannotation02.fs"|])>]
    let ``TyperelatedExpressions - E_rigidtypeannotation02.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .but here has type.    ''b list'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    //<Expects id="FS0001" span="(8,2-8,24)" status="error">This expression was expected to have type.    'seq<'a>'    .but here has type.    ''b list'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/Type-relatedExpressions", Includes=[|"E_rigidtypeannotation02b.fs"|])>]
    let ``TyperelatedExpressions - E_rigidtypeannotation02b.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .but here has type.    ''b list'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    //<Expects id="FS0001" span="(6,29-6,32)" status="error">This expression was expected to have type.    'int'    .but here has type.    'string'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/Type-relatedExpressions", Includes=[|"E_RigidTypeAnnotation03.fs"|])>]
    let ``TyperelatedExpressions - E_RigidTypeAnnotation03.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'int'    .but here has type.    'string'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/Type-relatedExpressions", Includes=[|"staticcoercion01.fs"|])>]
    let ``TyperelatedExpressions - staticcoercion01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/Type-relatedExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/Type-relatedExpressions", Includes=[|"staticcoercion01b.fs"|])>]
    let ``TyperelatedExpressions - staticcoercion01b.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

