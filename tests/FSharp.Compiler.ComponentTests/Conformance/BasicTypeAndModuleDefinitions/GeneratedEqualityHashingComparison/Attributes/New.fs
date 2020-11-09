// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions.GeneratedEqualityHashingComparison.Attributes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module New =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(8,8-8,9)" id="FS0381">A type cannot have both the 'ReferenceEquality' and 'StructuralEquality' or 'StructuralComparison' attributes$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test01.fs"|])>]
    let ``New - Test01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0381
        |> withDiagnosticMessageMatches "A type cannot have both the 'ReferenceEquality' and 'StructuralEquality' or 'StructuralComparison' attributes$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(16,12-16,14)" id="FS0001">The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test02.fs"|])>]
    let ``New - Test02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(14,12-14,14)" id="FS0001">The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test03.fs"|])>]
    let ``New - Test03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(9,8-9,9)" id="FS0385">A type with attribute 'CustomComparison' must have an explicit implementation of at least one of 'System\.IComparable' or 'System\.Collections\.IStructuralComparable'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test04.fs"|])>]
    let ``New - Test04.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0385
        |> withDiagnosticMessageMatches "A type with attribute 'CustomComparison' must have an explicit implementation of at least one of 'System\.IComparable' or 'System\.Collections\.IStructuralComparable'$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(14,12-14,14)" id="FS0001">The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test05.fs"|])>]
    let ``New - Test05.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(9,8-9,9)" id="FS0385">A type with attribute 'CustomComparison' must have an explicit implementation of at least one of 'System\.IComparable' or 'System\.Collections\.IStructuralComparable'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test06.fs"|])>]
    let ``New - Test06.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0385
        |> withDiagnosticMessageMatches "A type with attribute 'CustomComparison' must have an explicit implementation of at least one of 'System\.IComparable' or 'System\.Collections\.IStructuralComparable'$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(7,8-7,9)" id="FS0380">The 'StructuralEquality' attribute must be used in conjunction with the 'NoComparison' or 'StructuralComparison' attributes$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test07.fs"|])>]
    let ``New - Test07.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0380
        |> withDiagnosticMessageMatches "The 'StructuralEquality' attribute must be used in conjunction with the 'NoComparison' or 'StructuralComparison' attributes$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(14,12-14,14)" id="FS0001">The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test08.fs"|])>]
    let ``New - Test08.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'equality' constraint because it has the 'NoEquality' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test09.fs"|])>]
    let ``New - Test09.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(8,8-8,9)" id="FS0381">A type cannot have both the 'ReferenceEquality' and 'StructuralEquality' or 'StructuralComparison' attributes$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test10.fs"|])>]
    let ``New - Test10.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0381
        |> withDiagnosticMessageMatches "A type cannot have both the 'ReferenceEquality' and 'StructuralEquality' or 'StructuralComparison' attributes$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(16,12-16,14)" id="FS0001">The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test11.fs"|])>]
    let ``New - Test11.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(8,8-8,9)" id="FS0379">The 'StructuralComparison' attribute must be used in conjunction with the 'StructuralEquality' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test12.fs"|])>]
    let ``New - Test12.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0379
        |> withDiagnosticMessageMatches "The 'StructuralComparison' attribute must be used in conjunction with the 'StructuralEquality' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(8,8-8,9)" id="FS0385">A type with attribute 'CustomComparison' must have an explicit implementation of at least one of 'System\.IComparable' or 'System\.Collections\.IStructuralComparable'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test13.fs"|])>]
    let ``New - Test13.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0385
        |> withDiagnosticMessageMatches "A type with attribute 'CustomComparison' must have an explicit implementation of at least one of 'System\.IComparable' or 'System\.Collections\.IStructuralComparable'$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(16,12-16,14)" id="FS0001">The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test14.fs"|])>]
    let ``New - Test14.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test15.fs"|])>]
    let ``New - Test15.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(8,8-8,9)" id="FS0380">The 'StructuralEquality' attribute must be used in conjunction with the 'NoComparison' or 'StructuralComparison' attributes$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test16.fs"|])>]
    let ``New - Test16.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0380
        |> withDiagnosticMessageMatches "The 'StructuralEquality' attribute must be used in conjunction with the 'NoComparison' or 'StructuralComparison' attributes$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(16,12-16,14)" id="FS0001">The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test17.fs"|])>]
    let ``New - Test17.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test18.fs"|])>]
    let ``New - Test18.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test19.fs"|])>]
    let ``New - Test19.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(16,12-16,14)" id="FS0001">The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test20.fs"|])>]
    let ``New - Test20.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(8,8-8,9)" id="FS0379">The 'StructuralComparison' attribute must be used in conjunction with the 'StructuralEquality' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test21.fs"|])>]
    let ``New - Test21.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0379
        |> withDiagnosticMessageMatches "The 'StructuralComparison' attribute must be used in conjunction with the 'StructuralEquality' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(15,13-15,15)" id="FS0001">The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test22.fs"|])>]
    let ``New - Test22.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(17,13-17,15)" id="FS0001">The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test23.fs"|])>]
    let ``New - Test23.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(14,12-14,14)" id="FS0001">The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test24.fs"|])>]
    let ``New - Test24.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'comparison' constraint because it has the 'NoComparison' attribute$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(14,12-14,14)" id="FS0001">The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test25.fs"|])>]
    let ``New - Test25.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="error" span="(16,12-16,14)" id="FS0001">The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test26.fs"|])>]
    let ``New - Test26.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'R' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test27.fs"|])>]
    let ``New - Test27.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New)
    //<Expects status="warning" span="(10,6-10,12)" id="FS0386">A type with attribute 'NoEquality' should not usually have an explicit implementation of 'Object\.Equals\(obj\)'\. Disable this warning if this is intentional for interoperability purposes$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Attributes/New", Includes=[|"Test28.fs"|])>]
    let ``New - Test28.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0386
        |> withDiagnosticMessageMatches "A type with attribute 'NoEquality' should not usually have an explicit implementation of 'Object\.Equals\(obj\)'\. Disable this warning if this is intentional for interoperability purposes$"

