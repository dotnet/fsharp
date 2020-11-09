// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions.GeneratedEqualityHashingComparison

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Basic =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic", Includes=[|"EqualOnTuples01.fs"|])>]
    let ``Basic - EqualOnTuples01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic", Includes=[|"Equality01.fs"|])>]
    let ``Basic - Equality01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic", Includes=[|"Hashing01.fs"|])>]
    let ``Basic - Hashing01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic", Includes=[|"Comparison01.fs"|])>]
    let ``Basic - Comparison01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic)
    //<Expects span="(10,9-10,15)" status="error" id="FS0001">The type 'exn' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic", Includes=[|"E_ExceptionsNoComparison.fs"|])>]
    let ``Basic - E_ExceptionsNoComparison.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'exn' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic)
    //<Expects id="FS0344" status="error" span="(21,6-21,7)">The struct, record or union type 'S' has an explicit implementation of 'Object\.GetHashCode' or 'Object\.Equals'\. You must apply the 'CustomEquality' attribute to the type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic", Includes=[|"E_CustomEqualityEquals01.fs"|])>]
    let ``Basic - E_CustomEqualityEquals01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0344
        |> withDiagnosticMessageMatches "The struct, record or union type 'S' has an explicit implementation of 'Object\.GetHashCode' or 'Object\.Equals'\. You must apply the 'CustomEquality' attribute to the type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic)
    //<Expects id="FS0344" status="error" span="(21,6-21,7)">The struct, record or union type 'S' has an explicit implementation of 'Object\.GetHashCode' or 'Object\.Equals'\. You must apply the 'CustomEquality' attribute to the type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/Basic", Includes=[|"E_CustomEqualityGetHashCode01.fs"|])>]
    let ``Basic - E_CustomEqualityGetHashCode01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0344
        |> withDiagnosticMessageMatches "The struct, record or union type 'S' has an explicit implementation of 'Object\.GetHashCode' or 'Object\.Equals'\. You must apply the 'CustomEquality' attribute to the type"

