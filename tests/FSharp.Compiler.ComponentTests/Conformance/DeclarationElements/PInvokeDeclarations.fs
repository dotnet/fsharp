// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.DeclarationElements

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module PInvokeDeclarations =

    let private resourcePath = __SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations"

    // SOURCE=SanityCheck01.fs
    // P/Invoke to msvcrt.dll - typecheck only (runtime requires Windows)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"SanityCheck01.fs"|])>]
    let ``SanityCheck01_fs`` compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE=MarshalStruct01.fs
    // P/Invoke to User32.dll - typecheck only (runtime requires Windows)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"MarshalStruct01.fs"|])>]
    let ``MarshalStruct01_fs`` compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE=MarshalStruct01_Records.fs
    // P/Invoke to User32.dll - typecheck only (runtime requires Windows)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"MarshalStruct01_Records.fs"|])>]
    let ``MarshalStruct01_Records_fs`` compilation =
        compilation
        |> asExe
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE=EntryPoint.fs
    // P/Invoke to kernel32.dll - typecheck only (runtime requires Windows)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"EntryPoint.fs"|])>]
    let ``EntryPoint_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE=ComVisible01.fs
    // ComVisible attribute on records - should compile on all platforms
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"ComVisible01.fs"|])>]
    let ``ComVisible01_fs`` compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // SOURCE=ComVisible02.fs
    // <Expects id="FS1133" status="error" span="(8,12)">No constructors are available for the type 'r'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"ComVisible02.fs"|])>]
    let ``ComVisible02_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1133
        |> withDiagnosticMessageMatches "No constructors are available for the type 'r'"
        |> ignore

    // SOURCE=E_DLLImportInTypeDef01.fs SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" id="FS1221" span="(14,9-14,26)">DLLImport bindings must be static members in a class or function definitions in a module</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"E_DLLImportInTypeDef01.fs"|])>]
    let ``E_DLLImportInTypeDef01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1221
        |> withDiagnosticMessageMatches "DLLImport bindings must be static members in a class or function definitions in a module"
        |> ignore

    // SOURCE=CallingConventions01.fs SCFLAGS="--platform:x86" PLATFORM=x86
    // Typecheck only (native DLL required for runtime - Windows only)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"CallingConventions01.fs"|])>]
    let ``CallingConventions01_fs_x86`` compilation =
        compilation
        |> asExe
        |> withPlatform ExecutionPlatform.X86
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE=CallingConventions01_Records.fs SCFLAGS="--platform:x86" PLATFORM=x86
    // Typecheck only (native DLL required for runtime - Windows only)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"CallingConventions01_Records.fs"|])>]
    let ``CallingConventions01_Records_fs_x86`` compilation =
        compilation
        |> asExe
        |> withPlatform ExecutionPlatform.X86
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE=CallingConventions01.fs SCFLAGS="--platform:x64 --define:AMD64" PLATFORM=AMD64
    // Typecheck only (native DLL required for runtime - Windows only)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"CallingConventions01.fs"|])>]
    let ``CallingConventions01_fs_x64`` compilation =
        compilation
        |> asExe
        |> withPlatform ExecutionPlatform.X64
        |> withDefines ["AMD64"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE=CallingConventions01_Records.fs SCFLAGS="--platform:x64 --define:AMD64" PLATFORM=AMD64
    // Typecheck only (native DLL required for runtime - Windows only)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/DeclarationElements/P-invokeDeclarations", Includes=[|"CallingConventions01_Records.fs"|])>]
    let ``CallingConventions01_Records_fs_x64`` compilation =
        compilation
        |> asExe
        |> withPlatform ExecutionPlatform.X64
        |> withDefines ["AMD64"]
        |> typecheck
        |> shouldSucceed
        |> ignore
