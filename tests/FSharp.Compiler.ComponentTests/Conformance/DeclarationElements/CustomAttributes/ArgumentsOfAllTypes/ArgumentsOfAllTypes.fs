// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.CustomAttributes

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ArgumentsOfAllTypes =

    let csLibraryWithAttributes =
        CSharpFromPath (__SOURCE_DIRECTORY__ ++ "CSLibraryWithAttributes.cs")
        |> withName "CSLibraryWithAttributes"

    let verifyCompile compilation =
        compilation
        |> asLibrary
        |> withReferences [csLibraryWithAttributes]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> withReferences [csLibraryWithAttributes]
        |> compileAndRun

    // SOURCE=System_Int16_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Int16.dll"               # System_Int16_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Int16_Consumer.fsx"|])>]
    let ``System_Int16_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_Int16.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_Int32_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Int32.dll"               # System_Int32_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Int32_Consumer.fsx"|])>]
    let ``System_Int32_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_Int32.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_Int64_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll   -r System_Int64.dll"             # System_Int64_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Int64_Consumer.fsx"|])>]
    let ``System_Int64_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_Int64.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_UInt16_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_UInt16.dll"             # System_UInt16_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_UInt16_Consumer.fsx"|])>]
    let ``System_UInt16_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_UInt16.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_UInt32_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_UInt32.dll"             # System_UInt32_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_UInt32_Consumer.fsx"|])>]
    let ``System_UInt32_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_UInt32.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_UInt64_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_UInt64.dll"             # System_UInt64_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_UInt64_Consumer.fsx"|])>]
    let ``System_UInt64_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_UInt64.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_Char_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Char.dll"                 # System_Char_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Char_Consumer.fsx"|])>]
    let ``System_Char_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_Char.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_Byte_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Byte.dll"                 # System_Byte_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Byte_Consumer.fsx"|])>]
    let ``System_Byte_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_Byte.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_SByte_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_SByte.dll"               # System_SByte_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_SByte_Consumer.fsx"|])>]
    let ``System_SByte_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_SByte.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_Single_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Single.dll"             # System_Single_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Single_Consumer.fsx"|])>]
    let ``System_Single_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_Single.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_Double_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Double.dll"             # System_Double_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Double_Consumer.fsx"|])>]
    let ``System_Double_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_Double.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_String_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_String.dll"             # System_String_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_String_Consumer.fsx"|])>]
    let ``System_String_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_String.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_DateTimeKind_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_DateTimeKind.dll"	# System_DateTimeKind_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_DateTimeKind_Consumer.fsx"|])>]
    let ``System_DateTimeKind_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_DateTimeKind.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_Type_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Type.dll"                 # System_Type_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Type_Consumer.fsx"|])>]
    let ``System_Type_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_Type.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_Object_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Object.dll"             # System_Object_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Object_Consumer.fsx"|])>]
    let ``System_Object_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_Object.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=System_TypeArray_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_TypeArray.dll"       # System_TypeArray_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_TypeArray_Consumer.fsx"|])>]
    let ``System_TypeArray_Consumer_fsx`` compilation =
        let fslib =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_TypeArray.fsx")
            |> asLibrary
            |> withReferences [csLibraryWithAttributes]

        compilation
        |> withReferences [fslib]
        |> verifyCompileAndRun
        |> shouldSucceed
