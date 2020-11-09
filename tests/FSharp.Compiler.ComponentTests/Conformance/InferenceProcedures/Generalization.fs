// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Generalization =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/Generalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/Generalization", Includes=[|"GenGroup01.fs"|])>]
    let ``Generalization - GenGroup01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/Generalization)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/Generalization", Includes=[|"NoMoreValueRestriction01.fs"|])>]
    let ``Generalization - NoMoreValueRestriction01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/Generalization)
    //<Expects span="(8,11-8,14)" status="error" id="FS0001">This expression was expected to have type    'char'    but here has type    'float32'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/Generalization", Includes=[|"E_NoMoreValueRestriction01.fs"|])>]
    let ``Generalization - E_NoMoreValueRestriction01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type    'char'    but here has type    'float32'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/Generalization)
    //<Expects id="FS0064" span="(8,14-8,15)" status="warning">This construct causes code to be less generic than indicated by its type annotations\. The type variable implied by the use of a '#', '_' or other type annotation at or near '.+\.fs\(8,13\)-\(8,14\)' has been constrained to be type 'obj'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/Generalization", Includes=[|"E_DynamicTypeTestOverFreeArg01.fs"|])>]
    let ``Generalization - E_DynamicTypeTestOverFreeArg01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by its type annotations\. The type variable implied by the use of a '#', '_' or other type annotation at or near '.+\.fs\(8,13\)-\(8,14\)' has been constrained to be type 'obj'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/Generalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/Generalization", Includes=[|"LessRestrictive01.fs"|])>]
    let ``Generalization - LessRestrictive01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/Generalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/Generalization", Includes=[|"LessRestrictive02.fs"|])>]
    let ``Generalization - LessRestrictive02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/Generalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/Generalization", Includes=[|"LessRestrictive03.fs"|])>]
    let ``Generalization - LessRestrictive03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/Generalization)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/Generalization", Includes=[|"TypeAnnotation01.fs"|])>]
    let ``Generalization - TypeAnnotation01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/Generalization)
    //<Expects status="error" span="(23,9-23,19)" id="FS3068">The function or member 'Foo' is used in a way that requires further type annotations at its definition to ensure consistency of inferred types\. The inferred signature is 'static member private Qux\.Foo : \('T list -> 'T list\)'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/Generalization", Includes=[|"E_GeneralizeMemberInGeneric01.fs"|])>]
    let ``Generalization - E_GeneralizeMemberInGeneric01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3068
        |> withDiagnosticMessageMatches " 'T list\)'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/Generalization)
    //<Expects status="error" span="(11,16-11,29)" id="FS3068">The function or member 'Empty' is used in a way that requires further type annotations at its definition to ensure consistency of inferred types\. The inferred signature is 'static member Rope\.Empty : Rope<'T>'\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/Generalization", Includes=[|"RecordProperty01.fs"|])>]
    let ``Generalization - RecordProperty01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3068
        |> withDiagnosticMessageMatches "'\."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/Generalization)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/Generalization", Includes=[|"PropertyConstraint01.fs"|])>]
    let ``Generalization - PropertyConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

