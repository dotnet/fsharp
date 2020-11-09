// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Expression =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    //<Expects id="FS0025" span="(7,11-7,12)" status="warning">Incomplete pattern matches on this expression\. For example, the value '0' may indicate a case not covered by the pattern\(s\)</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Expression", Includes=[|"E_CounterExample01.fs"|])>]
    let ``Expression - E_CounterExample01.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "Incomplete pattern matches on this expression\. For example, the value '0' may indicate a case not covered by the pattern\(s\)"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    //<Expects id="FS0025" span="(11,9-11,10)" status="warning">Incomplete pattern matches on this expression\. For example, the value '1' may indicate a case not covered by the pattern\(s\)\. However, a pattern rule with a 'when' clause might successfully match this value</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Expression", Includes=[|"W_whenGuards01.fs"|])>]
    let ``Expression - W_whenGuards01.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "Incomplete pattern matches on this expression\. For example, the value '1' may indicate a case not covered by the pattern\(s\)\. However, a pattern rule with a 'when' clause might successfully match this value"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Expression", Includes=[|"NoCounterExampleTryWith01.fs"|])>]
    let ``Expression - NoCounterExampleTryWith01.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    //<Expects status="warning" span="(34,10-34,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value '\[|_; 1|\]' may indicate a case not covered by the pattern\(s\)\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Expression", Includes=[|"W_CounterExampleWithEnum01.fs"|])>]
    let ``Expression - W_CounterExampleWithEnum01.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "Incomplete pattern matches on this expression\. For example, the value '\[|_; 1|\]' may indicate a case not covered by the pattern\(s\)\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    //<Expects status="warning" span="(30,10-30,18)" id="FS0025">Incomplete pattern matches on this expression\. For example, the value 'T.Y' may indicate a case not covered by the pattern\(s\)\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Expression", Includes=[|"W_CounterExampleWithEnum02.fs"|])>]
    let ``Expression - W_CounterExampleWithEnum02.fs - --test:ErrorRanges -a`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "-a"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "Incomplete pattern matches on this expression\. For example, the value 'T.Y' may indicate a case not covered by the pattern\(s\)\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Expression", Includes=[|"patterns01.fs"|])>]
    let ``Expression - patterns01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Expression", Includes=[|"patterns02.fs"|])>]
    let ``Expression - patterns02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Expression", Includes=[|"whenGuards01.fs"|])>]
    let ``Expression - whenGuards01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Expression", Includes=[|"whenGuards02.fs"|])>]
    let ``Expression - whenGuards02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Expression", Includes=[|"whenGuardss01.fs"|])>]
    let ``Expression - whenGuardss01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Expression", Includes=[|"whenGuardss02.fs"|])>]
    let ``Expression - whenGuardss02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

