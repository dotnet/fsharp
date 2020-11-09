// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EntryPoint

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module EntryPoint =

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"behavior001.fs"|])>]
    let ``EntryPoint - behavior001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"noarguments001.fs"|])>]
    let ``EntryPoint - noarguments001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"oneargument001.fs"|])>]
    let ``EntryPoint - oneargument001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"inamodule001.fs"|])>]
    let ``EntryPoint - inamodule001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects id="FS0433" span="(19,9-19,19)" status="error">A function labeled with the 'EntryPointAttribute' attribute must be the last declaration in the last file in the compilation sequence.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"E_twoentrypoints001.fs"|])>]
    let ``EntryPoint - E_twoentrypoints001.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0433
        |> withDiagnosticMessageMatches "A function labeled with the 'EntryPointAttribute' attribute must be the last declaration in the last file in the compilation sequence."

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects id="FS0842" span="(9,3-9,13)" status="error">This attribute is not valid for use on this language element</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"E_oninvalidlanguageelement001.fs"|])>]
    let ``EntryPoint - E_oninvalidlanguageelement001.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0842
        |> withDiagnosticMessageMatches "This attribute is not valid for use on this language element"

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects id="FS0429" span="(12,7-12,17)" status="error">The attribute type 'EntryPointAttribute' has 'AllowMultiple=false'\. Multiple instances of this attribute cannot be attached to a single language element\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"E_twoattributesonsamefunction001.fs"|])>]
    let ``EntryPoint - E_twoattributesonsamefunction001.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0429
        |> withDiagnosticMessageMatches "The attribute type 'EntryPointAttribute' has 'AllowMultiple=false'\. Multiple instances of this attribute cannot be attached to a single language element\.$"

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"entrypointfunctionnotmain001.fs"|])>]
    let ``EntryPoint - entrypointfunctionnotmain001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects id="FS0001" span="(15,23-15,27)" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"E_invalidsignature001.fs"|])>]
    let ``EntryPoint - E_invalidsignature001.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects id="FS0001" span="(15,4-15,6)" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"E_InvalidSignature02.fs"|])>]
    let ``EntryPoint - E_InvalidSignature02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"E_CompilingToALibrary01.fs"|])>]
    let ``EntryPoint - E_CompilingToALibrary01.fs - --test:ErrorRanges --target:library`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--target:library"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"E_CompilingToAModule01.fs"|])>]
    let ``EntryPoint - E_CompilingToAModule01.fs - --test:ErrorRanges --target:module`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--target:module"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"EntryPointAndAssemblyCulture.fs"|])>]
    let ``EntryPoint - EntryPointAndAssemblyCulture.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects id="FS0988" span="(1,1-1,1)" status="warning">Main module of program is empty: nothing will happen when it is run</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"W_NoEntryPointInLastModuleInsideMultipleNamespace.fs"|])>]
    let ``EntryPoint - W_NoEntryPointInLastModuleInsideMultipleNamespace.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "Main module of program is empty: nothing will happen when it is run"

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects id="FS0988" span="(11,24-11,24)" status="warning">Main module of program is empty: nothing will happen when it is run</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"W_NoEntryPointModuleInNamespace.fs"|])>]
    let ``EntryPoint - W_NoEntryPointModuleInNamespace.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "Main module of program is empty: nothing will happen when it is run"

    // This test was automatically generated (moved from FSharpQA suite - EntryPoint)
    //<Expects id="FS0988" span="(13,24-13,24)" status="warning">Main module of program is empty: nothing will happen when it is run</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/EntryPoint", Includes=[|"W_NoEntryPointMultipleModules.fs"|])>]
    let ``EntryPoint - W_NoEntryPointMultipleModules.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "Main module of program is empty: nothing will happen when it is run"

