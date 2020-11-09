// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ByrefSafetyAnalysis =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"UseByrefInLambda01.fs"|])>]
    let ``ByrefSafetyAnalysis - UseByrefInLambda01.fs - -a`` compilation =
        compilation
        |> withOptions ["-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    //<Expects id="FS0412" span="(9,30-9,32)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"E_ByrefAsGenericArgument01.fs"|])>]
    let ``ByrefSafetyAnalysis - E_ByrefAsGenericArgument01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0412
        |> withDiagnosticMessageMatches "A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    //<Expects id="FS0406" span="(12,34-12,48)" status="error">The byref-typed variable 'byrefValue' is used in an invalid way\. Byrefs cannot be captured by closures or passed to inner functions\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"E_ByrefUsedInInnerLambda01.fs"|])>]
    let ``ByrefSafetyAnalysis - E_ByrefUsedInInnerLambda01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0406
        |> withDiagnosticMessageMatches "The byref-typed variable 'byrefValue' is used in an invalid way\. Byrefs cannot be captured by closures or passed to inner functions\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    //<Expects id="FS0406" span="(11,24-11,55)" status="error">The byref-typed variable 'byrefValue' is used in an invalid way\. Byrefs cannot be captured by closures or passed to inner functions\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"E_ByrefUsedInInnerLambda02.fs"|])>]
    let ``ByrefSafetyAnalysis - E_ByrefUsedInInnerLambda02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0406
        |> withDiagnosticMessageMatches "The byref-typed variable 'byrefValue' is used in an invalid way\. Byrefs cannot be captured by closures or passed to inner functions\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    //<Expects id="FS0406" span="(11,24-11,60)" status="error">The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"E_ByrefUsedInInnerLambda03.fs"|])>]
    let ``ByrefSafetyAnalysis - E_ByrefUsedInInnerLambda03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0406
        |> withDiagnosticMessageMatches "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    //<Expects id="FS0412" span="(11,6-11,14)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"E_ByrefUsedInQuotation01.fs"|])>]
    let ``ByrefSafetyAnalysis - E_ByrefUsedInQuotation01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0412
        |> withDiagnosticMessageMatches "A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    //<Expects id="FS0412" span="(11,50-11,54)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"E_SetFieldToByref01.fs"|])>]
    let ``ByrefSafetyAnalysis - E_SetFieldToByref01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0412
        |> withDiagnosticMessageMatches "A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    //<Expects id="FS0431" span="(8,9-8,17)" status="error">A byref typed value would be stored here\. Top-level let-bound byref values are not permitted</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"E_SetFieldToByref02.fs"|])>]
    let ``ByrefSafetyAnalysis - E_SetFieldToByref02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0431
        |> withDiagnosticMessageMatches "A byref typed value would be stored here\. Top-level let-bound byref values are not permitted"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    //<Expects id="FS0412" span="(8,25-8,26)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"E_SetFieldToByref03.fs"|])>]
    let ``ByrefSafetyAnalysis - E_SetFieldToByref03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0412
        |> withDiagnosticMessageMatches "A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    //<Expects id="FS0412" span="(19,20-19,53)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"E_SetFieldToByref04.fs"|])>]
    let ``ByrefSafetyAnalysis - E_SetFieldToByref04.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0412
        |> withDiagnosticMessageMatches "A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    //<Expects id="FS0412" span="(9,7-9,8)" status="error">A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"E_SetFieldToByref05.fs"|])>]
    let ``ByrefSafetyAnalysis - E_SetFieldToByref05.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0412
        |> withDiagnosticMessageMatches "A type instantiation involves a byref type\. This is not permitted by the rules of Common IL\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    //<Expects id="FS0001" span="(8,12-8,16)" status="error">This expression was expected to have type.    'byref<'a>'    .but here has type.    'int ref'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"E_FirstClassFuncTakesByref.fs"|])>]
    let ``ByrefSafetyAnalysis - E_FirstClassFuncTakesByref.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .but here has type.    'int ref'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ByrefSafetyAnalysis)
    //<Expects status="error" id="FS0043" span="(11,11-11,12)">The member or object constructor 'TryParse' does not take 1 argument\(s\)\. An overload was found taking 2 arguments\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ByrefSafetyAnalysis", Includes=[|"E_StaticallyResolvedByRef01.fs"|])>]
    let ``ByrefSafetyAnalysis - E_StaticallyResolvedByRef01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0043
        |> withDiagnosticMessageMatches "The member or object constructor 'TryParse' does not take 1 argument\(s\)\. An overload was found taking 2 arguments\."

