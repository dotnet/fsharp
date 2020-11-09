// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.TypesAndTypeConstraints

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TypeParameterDefinitions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/TypeParameterDefinitions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes=[|"BasicTypeParam01.fs"|])>]
    let ``TypeParameterDefinitions - BasicTypeParam01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/TypeParameterDefinitions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes=[|"HashConstraint01.fs"|])>]
    let ``TypeParameterDefinitions - HashConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/TypeParameterDefinitions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes=[|"HashConstraint02.fs"|])>]
    let ``TypeParameterDefinitions - HashConstraint02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/TypeParameterDefinitions)
    //<Expects id="FS0001" span="(6,5-6,60)" status="error">This expression was expected to have type.    ''a -> unit'    .but here has type.    'unit'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes=[|"E_GenericTypeConstraint.fs"|])>]
    let ``TypeParameterDefinitions - E_GenericTypeConstraint.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches " unit'    .but here has type.    'unit'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/TypeParameterDefinitions)
    //<Expects status="error" span="(7,12-7,13)" id="FS0583">Unmatched '\('$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes=[|"E_LazyInType02.fs"|])>]
    let ``TypeParameterDefinitions - E_LazyInType02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0583
        |> withDiagnosticMessageMatches "Unmatched '\('$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/TypeParameterDefinitions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes=[|"MultipleConstraints01.fs"|])>]
    let ``TypeParameterDefinitions - MultipleConstraints01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/TypeParameterDefinitions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes=[|"ValueTypesWithConstraints01.fs"|])>]
    let ``TypeParameterDefinitions - ValueTypesWithConstraints01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/TypeParameterDefinitions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes=[|"UnitSpecialization.fs"|])>]
    let ``TypeParameterDefinitions - UnitSpecialization.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

