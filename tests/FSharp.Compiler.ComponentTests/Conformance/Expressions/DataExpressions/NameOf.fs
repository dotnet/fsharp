// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.DataExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module NameOf =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/NameOf)
    //<Expects id="FS3250" span="(5,16)" status="error">Expression does not have a name.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/NameOf", Includes=[|"E_NameOfIntConst.fs"|])>]
    let ``NameOf - E_NameOfIntConst.fs - --langversion:preview`` compilation =
        compilation
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/NameOf)
    //<Expects id="FS3250" span="(5,16)" status="error">Expression does not have a name.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/NameOf", Includes=[|"E_NameOfStringConst.fs"|])>]
    let ``NameOf - E_NameOfStringConst.fs - --langversion:preview`` compilation =
        compilation
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/NameOf)
    //<Expects id="FS3250" span="(6,16)" status="error">Expression does not have a name.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/NameOf", Includes=[|"E_NameOfAppliedFunction.fs"|])>]
    let ``NameOf - E_NameOfAppliedFunction.fs - --langversion:preview`` compilation =
        compilation
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/NameOf)
    //<Expects id="FS3250" span="(6,16)" status="error">Expression does not have a name.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/NameOf", Includes=[|"E_NameOfIntegerAppliedFunction.fs"|])>]
    let ``NameOf - E_NameOfIntegerAppliedFunction.fs - --langversion:preview`` compilation =
        compilation
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/NameOf)
    //<Expects id="FS3250" span="(6,16)" status="error">Expression does not have a name.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/NameOf", Includes=[|"E_NameOfPartiallyAppliedFunction.fs"|])>]
    let ``NameOf - E_NameOfPartiallyAppliedFunction.fs - --langversion:preview`` compilation =
        compilation
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/NameOf)
    //<Expects id="FS3250" span="(6,16)" status="error">Expression does not have a name.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/NameOf", Includes=[|"E_NameOfDictLookup.fs"|])>]
    let ``NameOf - E_NameOfDictLookup.fs - --langversion:preview`` compilation =
        compilation
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/NameOf)
    //<Expects id="FS3250" span="(7,16)" status="error">Expression does not have a name.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/NameOf", Includes=[|"E_NameOfParameterAppliedFunction.fs"|])>]
    let ``NameOf - E_NameOfParameterAppliedFunction.fs - --langversion:preview`` compilation =
        compilation
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3250
        |> withDiagnosticMessageMatches "Expression does not have a name."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/NameOf)
    //<Expects id="FS3251" span="(5,9)" status="error">Using the 'nameof' operator as a first-class function value is not permitted</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/NameOf", Includes=[|"E_NameOfAsAFunction.fs"|])>]
    let ``NameOf - E_NameOfAsAFunction.fs - --langversion:preview`` compilation =
        compilation
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3251
        |> withDiagnosticMessageMatches "Using the 'nameof' operator as a first-class function value is not permitted"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/NameOf)
    //<Expects id="FS3251" span="(6,28)" status="error">Using the 'nameof' operator as a first-class function value is not permitted.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/NameOf", Includes=[|"E_NameOfWithPipe.fs"|])>]
    let ``NameOf - E_NameOfWithPipe.fs - --langversion:preview`` compilation =
        compilation
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3251
        |> withDiagnosticMessageMatches "Using the 'nameof' operator as a first-class function value is not permitted."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/NameOf)
    //<Expects id="FS0039" span="(5,43)" status="error">The value, constructor, namespace or type 'Unknown' is not defined.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/NameOf", Includes=[|"E_NameOfUnresolvableName.fs"|])>]
    let ``NameOf - E_NameOfUnresolvableName.fs - --langversion:preview`` compilation =
        compilation
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The value, constructor, namespace or type 'Unknown' is not defined."

