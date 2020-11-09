// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions.GeneratedEqualityHashingComparison.Attributes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Diags =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Diags)
    //<Expects status="error" span="(7,3-7,23)" id="FS0501">The object constructor 'StructuralComparisonAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralComparisonAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Diags", Includes=[|"E_AdjustUses01a.fs"|])>]
    let ``Diags - E_AdjustUses01a.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralComparisonAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Diags)
    //<Expects status="error" span="(7,3-7,23)" id="FS0501">The object constructor 'StructuralComparisonAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralComparisonAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Diags", Includes=[|"E_AdjustUses01b.fs"|])>]
    let ``Diags - E_AdjustUses01b.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralComparisonAttribute'\.$"

