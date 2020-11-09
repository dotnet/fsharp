// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Named =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(5,9-5,30)" status="error" id="FS0001">This expression was expected to have type.    'Choice<'a,'b>'    .but here has type.    'string'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_Error_LetRec01.fs"|])>]
    let ``Named - E_Error_LetRec01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .but here has type.    'string'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(4,9-4,34)" status="error" id="FS0827">This is not a valid name for an active pattern</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_Error_LetRec02.fs"|])>]
    let ``Named - E_Error_LetRec02.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0827
        |> withDiagnosticMessageMatches "This is not a valid name for an active pattern"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(4,10-4,43)" status="error" id="FS0827">This is not a valid name for an active pattern</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_Error_LetRec03.fs"|])>]
    let ``Named - E_Error_LetRec03.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0827
        |> withDiagnosticMessageMatches "This is not a valid name for an active pattern"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(4,9-4,29)" status="error" id="FS0001">This expression was expected to have type.    ''a option'    .but here has type.    'string'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_Error_LetRec04.fs"|])>]
    let ``Named - E_Error_LetRec04.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    ''a option'    .but here has type.    'string'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(4,5-4,18)" status="error" id="FS0001">This expression was expected to have type.    'Choice<'a,'b>'    .but here has type.    'string'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_Error_NonParam01.fs"|])>]
    let ``Named - E_Error_NonParam01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .but here has type.    'string'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(4,5-4,22)" status="error" id="FS0827">This is not a valid name for an active pattern</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_Error_NonParam02.fs"|])>]
    let ``Named - E_Error_NonParam02.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0827
        |> withDiagnosticMessageMatches "This is not a valid name for an active pattern"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(4,5-4,30)" status="error" id="FS0827">This is not a valid name for an active pattern</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_Error_NonParam03.fs"|])>]
    let ``Named - E_Error_NonParam03.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0827
        |> withDiagnosticMessageMatches "This is not a valid name for an active pattern"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(4,5-4,17)" status="error" id="FS0001">This expression was expected to have type.    ''a option'    .but here has type.    'string'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_Error_NonParam04.fs"|])>]
    let ``Named - E_Error_NonParam04.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    ''a option'    .but here has type.    'string'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(4,5-4,26)" status="error" id="FS0001">This expression was expected to have type.    'Choice<'a,'b>'    .but here has type.    'string'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_Error_Param01.fs"|])>]
    let ``Named - E_Error_Param01.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .but here has type.    'string'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(4,5-4,31)" status="error" id="FS0827">This is not a valid name for an active pattern</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_Error_Param02.fs"|])>]
    let ``Named - E_Error_Param02.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0827
        |> withDiagnosticMessageMatches "This is not a valid name for an active pattern"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(4,5-4,38)" status="error" id="FS0827">This is not a valid name for an active pattern</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_Error_Param03.fs"|])>]
    let ``Named - E_Error_Param03.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0827
        |> withDiagnosticMessageMatches "This is not a valid name for an active pattern"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(4,5-4,25)" status="error" id="FS0001">This expression was expected to have type.    ''a option'    .but here has type.    'string'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_Error_Param04.fs"|])>]
    let ``Named - E_Error_Param04.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    ''a option'    .but here has type.    'string'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"activePatterns01.fs"|])>]
    let ``Named - activePatterns01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"activePatterns02.fs"|])>]
    let ``Named - activePatterns02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"activePatterns03.fs"|])>]
    let ``Named - activePatterns03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects span="(20,7-20,15)" status="error" id="FS0039">The pattern discriminator 'Sentence' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_MulticasePartialNotAllowed01.fs"|])>]
    let ``Named - E_MulticasePartialNotAllowed01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The pattern discriminator 'Sentence' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"activePatterns05.fs"|])>]
    let ``Named - activePatterns05.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"activePatterns06.fs"|])>]
    let ``Named - activePatterns06.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"activePatterns07.fs"|])>]
    let ``Named - activePatterns07.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"activePatterns08.fs"|])>]
    let ``Named - activePatterns08.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"RecursiveActivePats.fs"|])>]
    let ``Named - RecursiveActivePats.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects id="FS0623" status="error" span="(18,7)">Active pattern case identifiers must begin with an uppercase letter</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_ActivePatterns01.fs"|])>]
    let ``Named - E_ActivePatterns01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0623
        |> withDiagnosticMessageMatches "Active pattern case identifiers must begin with an uppercase letter"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects id="FS3210" status="error" span="(6,15)">A is an active pattern and cannot be treated as a discriminated union case with named fields.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_ActivePatterns02.fs"|])>]
    let ``Named - E_ActivePatterns02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 3210
        |> withDiagnosticMessageMatches "A is an active pattern and cannot be treated as a discriminated union case with named fields."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects status="error" id="FS3174">Active patterns do not have fields. This syntax is invalid\.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_ActivePatternHasNoFields.fs"|])>]
    let ``Named - E_ActivePatternHasNoFields.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 3174
        |> withDiagnosticMessageMatches "Active patterns do not have fields. This syntax is invalid\."

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects id="FS0722" status="error">Only active patterns returning exactly one result may accept arguments</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_ParameterRestrictions01.fs"|])>]
    let ``Named - E_ParameterRestrictions01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0722
        |> withDiagnosticMessageMatches "Only active patterns returning exactly one result may accept arguments"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"MultiActivePatterns01.fs"|])>]
    let ``Named - MultiActivePatterns01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"ActivePatternOutsideMatch01.fs"|])>]
    let ``Named - ActivePatternOutsideMatch01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"ActivePatternOutsideMatch02.fs"|])>]
    let ``Named - ActivePatternOutsideMatch02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"discUnion01.fs"|])>]
    let ``Named - discUnion01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"discUnion02.fs"|])>]
    let ``Named - discUnion02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"NamedLiteral01.fs"|])>]
    let ``Named - NamedLiteral01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"NamedLiteral02.fs"|])>]
    let ``Named - NamedLiteral02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"PatternMatchRegressions01.fs"|])>]
    let ``Named - PatternMatchRegressions01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"PatternMatchRegressions02.fs"|])>]
    let ``Named - PatternMatchRegressions02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects status="error" span="(22,6-22,38)" id="FS1210">Active pattern '\|ClientExternalTypeUse\|WillFail\|' has a result type containing type variables that are not determined by the input\. The common cause is a when a result case is not mentioned, e\.g\. 'let \(\|A\|B\|\) \(x:int\) = A x'\. This can be fixed with a type constraint, e\.g\. 'let \(\|A\|B\|\) \(x:int\) : Choice<int,unit> = A x'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_PatternMatchRegressions02.fs"|])>]
    let ``Named - E_PatternMatchRegressions02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1210
        |> withDiagnosticMessageMatches " = A x'$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"ActivePatternUnconstrained01.fs"|])>]
    let ``Named - ActivePatternUnconstrained01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects status="error" span="(7,6-7,16)" id="FS1210">Active pattern '\|A1\|A2\|A3\|' has a result type containing type variables that are not determined by the input\. The common cause is a when a result case is not mentioned, e\.g\. 'let \(\|A\|B\|\) \(x:int\) = A x'\. This can be fixed with a type constraint, e\.g\. 'let \(\|A\|B\|\) \(x:int\) : Choice<int,unit> = A x'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_ActivePatternUnconstrained01.fs"|])>]
    let ``Named - E_ActivePatternUnconstrained01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1210
        |> withDiagnosticMessageMatches " = A x'$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects status="error" span="(5,6-5,11)" id="FS1209">Active pattern '|A|B|' is not a function$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_ActivePatternNotAFuncion.fs"|])>]
    let ``Named - E_ActivePatternNotAFuncion.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1209
        |> withDiagnosticMessageMatches "Active pattern '|A|B|' is not a function$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Named)
    //<Expects id="FS0265" span="(6,53)" status="error">Active patterns cannot return more than 7 possibilities$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Named", Includes=[|"E_LargeActivePat01.fs"|])>]
    let ``Named - E_LargeActivePat01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0265
        |> withDiagnosticMessageMatches "Active patterns cannot return more than 7 possibilities$"

