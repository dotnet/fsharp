// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module WithOOP =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/WithOOP)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/WithOOP", Includes=[|"InInterface01.fs"|])>]
    let ``WithOOP - InInterface01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/WithOOP)
    //<Expects id="FS0897" span="(9,5)" status="error">Measure declarations may have only static members$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/WithOOP", Includes=[|"E_NoInstanceOnMeasure01.fs"|])>]
    let ``WithOOP - E_NoInstanceOnMeasure01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0897
        |> withDiagnosticMessageMatches "Measure declarations may have only static members$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/WithOOP)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/WithOOP", Includes=[|"StaticsOnMeasure01.fs"|])>]
    let ``WithOOP - StaticsOnMeasure01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/WithOOP)
    //<Expects id="FS0438" span="(9,5)" status="error">Duplicate method\. The method '\.ctor' has the same name and signature as another method in type 'Foo<'a>' once tuples, functions, units of measure and/or provided types are erased\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/WithOOP", Includes=[|"E_OverloadsDifferByUOMAttr.fs"|])>]
    let ``WithOOP - E_OverloadsDifferByUOMAttr.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0438
        |> withDiagnosticMessageMatches "' once tuples, functions, units of measure and/or provided types are erased\."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/WithOOP)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/WithOOP", Includes=[|"GenericUOM01.fs"|])>]
    let ``WithOOP - GenericUOM01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/WithOOP)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/WithOOP", Includes=[|"Polymorphism02.fs"|])>]
    let ``WithOOP - Polymorphism02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/WithOOP)
    //<Expects id="FS0904" span="(8,5-8,18)" status="error">Measure declarations may have only static members: constructors are not available</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/WithOOP", Includes=[|"E_NoConstructorOnMeasure01.fs"|])>]
    let ``WithOOP - E_NoConstructorOnMeasure01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0904
        |> withDiagnosticMessageMatches "Measure declarations may have only static members: constructors are not available"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/UnitsOfMeasure/WithOOP)
    //<Expects id="FS0842" status="error" span="(8,36)">This attribute is not valid for use on this language element</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/UnitsOfMeasure/WithOOP", Includes=[|"E_GenericUOM01.fs"|])>]
    let ``WithOOP - E_GenericUOM01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0842
        |> withDiagnosticMessageMatches "This attribute is not valid for use on this language element"

