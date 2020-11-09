// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.BasicTypeAndModuleDefinitions.GeneratedEqualityHashingComparison

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module IComparison =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/IComparison)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/IComparison", Includes=[|"DU.fs"|])>]
    let ``IComparison - DU.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/IComparison)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/IComparison", Includes=[|"Record.fs"|])>]
    let ``IComparison - Record.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/IComparison)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/IComparison", Includes=[|"Struct.fs"|])>]
    let ``IComparison - Struct.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/IComparison)
    //<Expects id="FS0343" span="(15,6)" status="warning">The type 'C' implements 'System\.IComparable' explicitly but provides no corresponding override for 'Object\.Equals'\. An implementation of 'Object\.Equals' has been automatically provided, implemented via 'System\.IComparable'\. Consider implementing the override 'Object\.Equals' explicitly</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/BasicTypeAndModuleDefinitions/GeneratedEqualityHashingComparison/IComparison", Includes=[|"W_ImplIComparable.fs"|])>]
    let ``IComparison - W_ImplIComparable.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0343
        |> withDiagnosticMessageMatches "The type 'C' implements 'System\.IComparable' explicitly but provides no corresponding override for 'Object\.Equals'\. An implementation of 'Object\.Equals' has been automatically provided, implemented via 'System\.IComparable'\. Consider implementing the override 'Object\.Equals' explicitly"

