// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.DataExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module SequenceExpressions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects id="FS0035" span="(10,9-10,34)" status="error">.+'if ... then ... else'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"version46/W_IfThenElse01.fs"|])>]
    let ``SequenceExpressions - version46/W_IfThenElse01.fs - --langversion:4.6 --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--langversion:4.6"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0035
        |> withDiagnosticMessageMatches ".+'if ... then ... else'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects id="FS0035" span="(10,9-10,51)" status="error">.+'if ... then ... else'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"version46/W_IfThenElse02.fs"|])>]
    let ``SequenceExpressions - version46/W_IfThenElse02.fs - --langversion:4.6 --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--langversion:4.6"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0035
        |> withDiagnosticMessageMatches ".+'if ... then ... else'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects id="FS0035" span="(10,9-10,45)" status="error">.+'if ... then ... else'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"version46/W_IfThenElse03.fs"|])>]
    let ``SequenceExpressions - version46/W_IfThenElse03.fs - --langversion:4.6 --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--langversion:4.6"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0035
        |> withDiagnosticMessageMatches ".+'if ... then ... else'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"version47/W_IfThenElse01.fs"|])>]
    let ``SequenceExpressions - version47/W_IfThenElse01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"version47/W_IfThenElse02.fs"|])>]
    let ``SequenceExpressions - version47/W_IfThenElse02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"version47/W_IfThenElse03.fs"|])>]
    let ``SequenceExpressions - version47/W_IfThenElse03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"IfThenElse04.fs"|])>]
    let ``SequenceExpressions - IfThenElse04.fs - --test:ErrorRanges --warnaserror`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"IfThenElse05.fs"|])>]
    let ``SequenceExpressions - IfThenElse05.fs - --test:ErrorRanges --warnaserror`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"IfThenElse06.fs"|])>]
    let ``SequenceExpressions - IfThenElse06.fs - --test:ErrorRanges --warnaserror`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"IfThenElse07.fs"|])>]
    let ``SequenceExpressions - IfThenElse07.fs - --test:ErrorRanges --warnaserror`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"tailcalls01.fs"|])>]
    let ``SequenceExpressions - tailcalls01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"tailcalls02.fs"|])>]
    let ``SequenceExpressions - tailcalls02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"CodeDisposalInMatch01.fs"|])>]
    let ``SequenceExpressions - CodeDisposalInMatch01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"final_yield_bang_keyword_01.fs"|])>]
    let ``SequenceExpressions - final_yield_bang_keyword_01.fs - --test:ErrorRanges --warnaserror`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"final_yield_dash_gt_01.fs"|])>]
    let ``SequenceExpressions - final_yield_dash_gt_01.fs - --test:ErrorRanges --warnaserror`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"final_yield_keyword_01.fs"|])>]
    let ``SequenceExpressions - final_yield_keyword_01.fs - --test:ErrorRanges --warnaserror`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--warnaserror"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects id="FS0596" span="(8,16-8,23)" status="error">The use of '->' in sequence and computation expressions is limited to the form 'for pat in expr -> expr'\. Use the syntax 'for \.\.\. in \.\.\. do \.\.\. yield\.\.\.' to generate elements in more complex sequence expressions</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"E_final_yield_dash_gt_01.fs"|])>]
    let ``SequenceExpressions - E_final_yield_dash_gt_01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0596
        |> withDiagnosticMessageMatches " expr'\. Use the syntax 'for \.\.\. in \.\.\. do \.\.\. yield\.\.\.' to generate elements in more complex sequence expressions"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    //<Expects id="FS0742" status="error">This list expression exceeds the maximum size for list literals\. Use an array for larger literals and call Array\.ToList</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"E_ReallyLongList01.fs"|])>]
    let ``SequenceExpressions - E_ReallyLongList01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0742
        |> withDiagnosticMessageMatches "This list expression exceeds the maximum size for list literals\. Use an array for larger literals and call Array\.ToList"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/DataExpressions/SequenceExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/DataExpressions/SequenceExpressions", Includes=[|"ReallyLongArray01.fs"|])>]
    let ``SequenceExpressions - ReallyLongArray01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

