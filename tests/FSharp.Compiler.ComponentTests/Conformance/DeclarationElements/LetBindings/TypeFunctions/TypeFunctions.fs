// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.LetBindings

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TypeFunctions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeofInCustomAttributes001.fs"|])>]
    let ``typeofInCustomAttributes001_fs`` compilation =
        compilation
        |> asFsx
        |> compile
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    //<Expects id="FS0704" span="(9,16-9,17)" status="error">Expected type, not unit-of-measure</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_typeof_measure_01.fs"|])>]
    let ``E_typeof_measure_01_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0704
        |> withDiagnosticMessageMatches "Expected type, not unit-of-measure"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    //<Expects id="FS0929" span="(7,6-7,7)" status="error">This type requires a definition</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_typeof_undefined_01.fs"|])>]
    let ``E_typeof_undefined_01_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0929
        |> withDiagnosticMessageMatches "This type requires a definition"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeof_anonymous_01.fs"|])>]
    let ``typeof_anonymous_01_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeof_class_01.fs"|])>]
    let ``typeof_class_01_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeof_interface_01.fs"|])>]
    let ``typeof_interface_01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/LetBindings/TypeFunctions)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"typeof_struct_01.fs"|])>]
    let ``typeof_struct_01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        |> ignore

