// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module PinvokeDeclarations =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/P-invokeDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"SanityCheck01.fs"|])>]
    let ``PinvokeDeclarations - SanityCheck01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/P-invokeDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"MarshalStruct01.fs"|])>]
    let ``PinvokeDeclarations - MarshalStruct01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/P-invokeDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"MarshalStruct01_Records.fs"|])>]
    let ``PinvokeDeclarations - MarshalStruct01_Records.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/P-invokeDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"EntryPoint.fs"|])>]
    let ``PinvokeDeclarations - EntryPoint.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/P-invokeDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"ComVisible01.fs"|])>]
    let ``PinvokeDeclarations - ComVisible01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/P-invokeDeclarations)
    //<Expects id="FS1133" status="error" span="(8,12)">No constructors are available for the type 'r'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"ComVisible02.fs"|])>]
    let ``PinvokeDeclarations - ComVisible02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 1133
        |> withDiagnosticMessageMatches "No constructors are available for the type 'r'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/P-invokeDeclarations)
    // <Expects status="error" id="FS1221" span="(14,9-14,26)">DLLImport bindings must be static members in a class or function definitions in a module</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"E_DLLImportInTypeDef01.fs"|])>]
    let ``PinvokeDeclarations - E_DLLImportInTypeDef01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1221
        |> withDiagnosticMessageMatches "DLLImport bindings must be static members in a class or function definitions in a module"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/P-invokeDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"CallingConventions01.fs"|])>]
    let ``PinvokeDeclarations - CallingConventions01.fs - --platform:x86`` compilation =
        compilation
        |> withOptions ["--platform:x86"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/P-invokeDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"CallingConventions01_Records.fs"|])>]
    let ``PinvokeDeclarations - CallingConventions01_Records.fs - --platform:x86`` compilation =
        compilation
        |> withOptions ["--platform:x86"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/P-invokeDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"CallingConventions01.fs"|])>]
    let ``PinvokeDeclarations - CallingConventions01.fs - --platform:x64 --define:AMD64`` compilation =
        compilation
        |> withOptions ["--platform:x64"; "--define:AMD64"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/DeclarationElements/P-invokeDeclarations)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"CallingConventions01_Records.fs"|])>]
    let ``PinvokeDeclarations - CallingConventions01_Records.fs - --platform:x64 --define:AMD64`` compilation =
        compilation
        |> withOptions ["--platform:x64"; "--define:AMD64"]
        |> typecheck
        |> shouldSucceed

