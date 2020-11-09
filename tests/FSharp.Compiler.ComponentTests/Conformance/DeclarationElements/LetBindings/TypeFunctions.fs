// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.LetBindings

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TypeFunctions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/TypeFunctions", Includes=[|"typeofBasic001.fs"|])>]
    let ``TypeFunctions - typeofBasic001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/TypeFunctions", Includes=[|"typeofInCustomAttributes001.fs"|])>]
    let ``TypeFunctions - typeofInCustomAttributes001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    //<Expects id="FS0704" span="(9,16-9,17)" status="error">Expected type, not unit-of-measure</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/TypeFunctions", Includes=[|"E_typeof_measure_01.fs"|])>]
    let ``TypeFunctions - E_typeof_measure_01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0704
        |> withDiagnosticMessageMatches "Expected type, not unit-of-measure"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    //<Expects id="FS0929" span="(7,6-7,7)" status="error">This type requires a definition</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/TypeFunctions", Includes=[|"E_typeof_undefined_01.fs"|])>]
    let ``TypeFunctions - E_typeof_undefined_01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0929
        |> withDiagnosticMessageMatches "This type requires a definition"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/TypeFunctions", Includes=[|"typeof_anonymous_01.fs"|])>]
    let ``TypeFunctions - typeof_anonymous_01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/TypeFunctions", Includes=[|"typeof_class_01.fs"|])>]
    let ``TypeFunctions - typeof_class_01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/TypeFunctions", Includes=[|"typeof_interface_01.fs"|])>]
    let ``TypeFunctions - typeof_interface_01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/TypeFunctions", Includes=[|"typeof_struct_01.fs"|])>]
    let ``TypeFunctions - typeof_struct_01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    //<Expects id="FS0671" status="error" span="(6,12)">A property cannot have explicit type parameters\. Consider using a method instead\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/TypeFunctions", Includes=[|"E_NoTypeFuncsInTypes.fs"|])>]
    let ``TypeFunctions - E_NoTypeFuncsInTypes.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0671
        |> withDiagnosticMessageMatches "A property cannot have explicit type parameters\. Consider using a method instead\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/DeclarationElements/LetBindings/TypeFunctions", Includes=[|"SizeOf01.fs"|])>]
    let ``TypeFunctions - SizeOf01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

