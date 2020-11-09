// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TypeChecker =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"GenericSubType01.fs"|])>]
    let ``TypeChecker - GenericSubType01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"Slash_InFunction01.fs"|])>]
    let ``TypeChecker - Slash_InFunction01.fs - --warnaserror+`` compilation =
        compilation
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"Slash_InMethod01.fs"|])>]
    let ``TypeChecker - Slash_InMethod01.fs - --warnaserror+`` compilation =
        compilation
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"Generalization01.fs"|])>]
    let ``TypeChecker - Generalization01.fs - --warnaserror`` compilation =
        compilation
        |> withOptions ["--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    //<Expects status="error" id="FS3360" span="(13,6-13,8)">'IB<'b>' cannot implement the interface 'IA<_>' with the two instantiations 'IA<kg>' and 'IA<'b>' because they may unify.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"E_GenInterfaceWithDifferentGenInstantiations.fs"|])>]
    let ``TypeChecker - E_GenInterfaceWithDifferentGenInstantiations.fs - --test:ErrorRanges --langversion:preview`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--langversion:preview"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3360
        |> withDiagnosticMessageMatches "' because they may unify."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"TypeAbbreviation_decimal_01.fs"|])>]
    let ``TypeChecker - TypeAbbreviation_decimal_01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"TypeAbbreviation_float32_01.fs"|])>]
    let ``TypeChecker - TypeAbbreviation_float32_01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"TypeAbbreviation_float_01.fs"|])>]
    let ``TypeChecker - TypeAbbreviation_float_01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    //<Expects id="FS0687" span="(8,1-8,2)" status="error">This value, type or method expects 2 type parameter\(s\) but was given 1</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"typechecker01.fs"|])>]
    let ``TypeChecker - typechecker01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0687
        |> withDiagnosticMessageMatches "This value, type or method expects 2 type parameter\(s\) but was given 1"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    //<Expects status="warning" span="(11,19-11,26)" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The unit-of-measure variable 'v has been constrained to be measure 'kg'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"W_TypeContraint01.fs"|])>]
    let ``TypeChecker - W_TypeContraint01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by the type annotations\. The unit-of-measure variable 'v has been constrained to be measure 'kg'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"TypeConstraint02.fs"|])>]
    let ``TypeChecker - TypeConstraint02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"TypeConstraint03.fs"|])>]
    let ``TypeChecker - TypeConstraint03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    //<Expects id="FS0464" status="warning" span="(9,18)">This code is less generic than indicated by its annotations\. A unit-of-measure specified using '_' has been determined to be '1', i\.e\. dimensionless</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"LessGeneric02.fs"|])>]
    let ``TypeChecker - LessGeneric02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0464
        |> withDiagnosticMessageMatches "This code is less generic than indicated by its annotations\. A unit-of-measure specified using '_' has been determined to be '1', i\.e\. dimensionless"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/TypeChecker)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/TypeChecker", Includes=[|"ValueRestriction01.fs"|])>]
    let ``TypeChecker - ValueRestriction01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

