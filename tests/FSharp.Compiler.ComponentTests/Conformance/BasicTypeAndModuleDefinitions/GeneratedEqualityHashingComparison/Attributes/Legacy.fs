// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions.GeneratedEqualityHashingComparison.Attributes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Legacy =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(9,5-9,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test01.fs"|])>]
    let ``Legacy - Test01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(10,5-10,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test02.fs"|])>]
    let ``Legacy - Test02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(8,5-8,25)" id="FS0501">The object constructor 'StructuralComparisonAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralComparisonAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test03.fs"|])>]
    let ``Legacy - Test03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralComparisonAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(9,5-9,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test04.fs"|])>]
    let ``Legacy - Test04.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(9,5-9,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test05.fs"|])>]
    let ``Legacy - Test05.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(8,5-8,25)" id="FS0501">The object constructor 'StructuralComparisonAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralComparisonAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test06.fs"|])>]
    let ``Legacy - Test06.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralComparisonAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(9,5-9,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test07.fs"|])>]
    let ``Legacy - Test07.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(9,5-9,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test08.fs"|])>]
    let ``Legacy - Test08.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(6,5-6,22)" id="FS0501">The object constructor 'ReferenceEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> ReferenceEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test09.fs"|])>]
    let ``Legacy - Test09.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " ReferenceEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(10,5-10,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test10.fs"|])>]
    let ``Legacy - Test10.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(10,5-10,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test11.fs"|])>]
    let ``Legacy - Test11.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(8,5-8,25)" id="FS0501">The object constructor 'StructuralComparisonAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralComparisonAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test12.fs"|])>]
    let ``Legacy - Test12.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralComparisonAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(9,5-9,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test13.fs"|])>]
    let ``Legacy - Test13.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(11,5-11,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test14.fs"|])>]
    let ``Legacy - Test14.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(8,5-8,25)" id="FS0501">The object constructor 'StructuralComparisonAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralComparisonAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test15.fs"|])>]
    let ``Legacy - Test15.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralComparisonAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(9,5-9,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test16.fs"|])>]
    let ``Legacy - Test16.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(8,5-8,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test17.fs"|])>]
    let ``Legacy - Test17.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(6,5-6,22)" id="FS0501">The object constructor 'ReferenceEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> ReferenceEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test18.fs"|])>]
    let ``Legacy - Test18.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " ReferenceEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(8,5-8,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test19.fs"|])>]
    let ``Legacy - Test19.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(8,5-8,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test20.fs"|])>]
    let ``Legacy - Test20.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(6,5-6,25)" id="FS0501">The object constructor 'StructuralComparisonAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralComparisonAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test21.fs"|])>]
    let ``Legacy - Test21.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralComparisonAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(8,5-8,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test22.fs"|])>]
    let ``Legacy - Test22.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(10,5-10,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test23.fs"|])>]
    let ``Legacy - Test23.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(6,5-6,25)" id="FS0501">The object constructor 'StructuralComparisonAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralComparisonAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test24.fs"|])>]
    let ``Legacy - Test24.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralComparisonAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(7,5-7,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test25.fs"|])>]
    let ``Legacy - Test25.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(7,5-7,23)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test26.fs"|])>]
    let ``Legacy - Test26.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test27.fs"|])>]
    let ``Legacy - Test27.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy)
    //<Expects status="error" span="(7,3-7,21)" id="FS0501">The object constructor 'StructuralEqualityAttribute' takes 0 argument\(s\) but is here given 1\. The required signature is 'new : unit -> StructuralEqualityAttribute'\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/Legacy", Includes=[|"Test28.fs"|])>]
    let ``Legacy - Test28.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0501
        |> withDiagnosticMessageMatches " StructuralEqualityAttribute'\.$"

