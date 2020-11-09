// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Constants =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Constants)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Constants", Includes=[|"decimal.fs"|])>]
    let ``Constants - decimal.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Constants)
    //<Expects id="FS0636" status="error">Units-of-measure supported only on float, float32, decimal and signed integer types</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Constants", Includes=[|"E_UnsupportedTypes01.fs"|])>]
    let ``Constants - E_UnsupportedTypes01.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0636
        |> withDiagnosticMessageMatches "Units-of-measure supported only on float, float32, decimal and signed integer types"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Constants)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Constants", Includes=[|"ieee32.fs"|])>]
    let ``Constants - ieee32.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Constants)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Constants", Includes=[|"ieee64.fs"|])>]
    let ``Constants - ieee64.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Constants)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Constants", Includes=[|"SpecialSyntax_.fs"|])>]
    let ``Constants - SpecialSyntax_.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

