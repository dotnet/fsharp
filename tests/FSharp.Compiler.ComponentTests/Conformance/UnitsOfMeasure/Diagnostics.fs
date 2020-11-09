// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Diagnostics =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects status="error" span="(11,2-11,3)" id="FS0001">The type 'int<foo>' does not match the type 'int'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"E_RangeExpression01.fs"|])>]
    let ``Diagnostics - E_RangeExpression01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "' does not match the type 'int'$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"RangeExpression01.fs"|])>]
    let ``Diagnostics - RangeExpression01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects status="error" span="(7,5-7,6)" id="FS0030">.+val y : '_a list ref</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"W_NonGenVarInValueRestrictionWarning.fs"|])>]
    let ``Diagnostics - W_NonGenVarInValueRestrictionWarning.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0030
        |> withDiagnosticMessageMatches ".+val y : '_a list ref"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects status="error" id="FS0634" span="(9,16-9,18)">Non-zero constants cannot have generic units\. For generic zero, write 0\.0<_></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"E_UnexpectedTypeParameter01.fs"|])>]
    let ``Diagnostics - E_UnexpectedTypeParameter01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0634

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects id="FS0625" span="(9,21-9,22)" status="error">Denominator must not be 0 in unit-of-measure exponent</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"E_ZeroDenominator.fs"|])>]
    let ``Diagnostics - E_ZeroDenominator.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0625
        |> withDiagnosticMessageMatches "Denominator must not be 0 in unit-of-measure exponent"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects id="FS0010" span="(13,18-13,19)" status="error">Unexpected infix operator in binding\. Expected integer literal, '-' or other token</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"E_ParsingRationalExponents.fs"|])>]
    let ``Diagnostics - E_ParsingRationalExponents.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> withDiagnosticMessageMatches "Unexpected infix operator in binding\. Expected integer literal, '-' or other token"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects id="FS0064" span="(5,30-5,31)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The unit-of-measure variable 'a has been constrained to be measure '1'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"W_UnitOfMeasureCodeLessGeneric01.fs"|])>]
    let ``Diagnostics - W_UnitOfMeasureCodeLessGeneric01.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by the type annotations\. The unit-of-measure variable 'a has been constrained to be measure '1'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects status="error" span="(6,41-6,43)" id="FS0702">Expected unit-of-measure parameter, not type parameter\. Explicit unit-of-measure parameters must be marked with the \[<Measure>\] attribute</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"E_ExplicitUnitOfMeasureParameters01.fs"|])>]
    let ``Diagnostics - E_ExplicitUnitOfMeasureParameters01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0702
        |> withDiagnosticMessageMatches "\] attribute"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects status="error" id="FS0702" span="(6,45-6,47)">Expected unit-of-measure parameter, not type parameter\. Explicit unit-of-measure parameters must be marked with the \[<Measure>\] attribute</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"E_ExplicitUnitOfMeasureParameters02.fs"|])>]
    let ``Diagnostics - E_ExplicitUnitOfMeasureParameters02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0702
        |> withDiagnosticMessageMatches "\] attribute"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects status="error" id="FS0702" span="(6,33-6,35)">Expected unit-of-measure parameter, not type parameter\. Explicit unit-of-measure parameters must be marked with the \[<Measure>\] attribute</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"E_ExplicitUnitOfMeasureParameters03.fs"|])>]
    let ``Diagnostics - E_ExplicitUnitOfMeasureParameters03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0702
        |> withDiagnosticMessageMatches "\] attribute"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects status="error" id="FS0702" span="(6,27-6,29)">Expected unit-of-measure parameter, not type parameter\. Explicit unit-of-measure parameters must be marked with the \[<Measure>\] attribute</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"E_ExplicitUnitOfMeasureParameters04.fs"|])>]
    let ``Diagnostics - E_ExplicitUnitOfMeasureParameters04.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0702
        |> withDiagnosticMessageMatches "\] attribute"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects id="FS0001" span="(9,22-9,28)" status="error">The unit of measure 's' does not match the unit of measure 'Kg'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"E_RangeOfDimensioned03.fs"|])>]
    let ``Diagnostics - E_RangeOfDimensioned03.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The unit of measure 's' does not match the unit of measure 'Kg'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"RangeOfDimensioned01.fs"|])>]
    let ``Diagnostics - RangeOfDimensioned01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"RangeOfDimensioned02.fs"|])>]
    let ``Diagnostics - RangeOfDimensioned02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects id="FS0704" span="(14,49-14,50)" status="error">Expected type, not unit-of-measure</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"E_ExpectedTypeNotUnitOfMeasure01.fs"|])>]
    let ``Diagnostics - E_ExpectedTypeNotUnitOfMeasure01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0704
        |> withDiagnosticMessageMatches "Expected type, not unit-of-measure"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects id="FS0636" span="(15,9-15,16)" status="error">Units-of-measure supported only on float, float32, decimal and signed integer types</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"E_UnsupportedType01.fs"|])>]
    let ``Diagnostics - E_UnsupportedType01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0636
        |> withDiagnosticMessageMatches "Units-of-measure supported only on float, float32, decimal and signed integer types"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/Diagnostics)
    //<Expects status="error" id="FS0001" span="(8,16-8,23)">The type 'decimal<Kg>' does not match the type 'decimal'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/Diagnostics", Includes=[|"E_RangeOfDecimals01.fs"|])>]
    let ``Diagnostics - E_RangeOfDecimals01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "' does not match the type 'decimal'"

