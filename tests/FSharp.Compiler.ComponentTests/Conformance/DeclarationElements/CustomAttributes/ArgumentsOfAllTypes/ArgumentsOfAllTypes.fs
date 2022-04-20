// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.DeclarationElements.CustomAttributes

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ArgumentsOfAllTypes =

    let verifyCompile compilation =
        let csLibraryWithAttributes =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "CSLibraryWithAttributes.cs")

        compilation
        |> asLibrary
        |> withReferences [csLibraryWithAttributes]
        |> compile

    let verifyCompileAndRun compilation =
        let csLibraryWithAttributes =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "CSLibraryWithAttributes.cs")

        compilation
        |> asExe
        |> withReferences [csLibraryWithAttributes]
        |> withOptions ["--nowarn:988"]
        |> compileAndRun

    //# This test builds an F# library with attributes..
    //# ... and then it consumes them from C# code
    //SOURCE=FSharpAttrLibrary.fs SCFLAGS=-a                                                                      # FSharpAttrLibrary.fs
    //SOURCE=dummy.fs PRECMD="\$CSC_PIPE CSharpConsumer.cs /reference:System.Core.dll /r:FSharpAttrLibrary.dll" POSTCMD=CSharpConsumer.exe    # CSharpConsumer
    //SOURCE=dummy.fs PRECMD="\$CSC_PIPE /target:library /reference:System.dll CSLibraryWithAttributes.cs"        # CSLibraryWithAttributes.cs
    //
    //#
    //#
    //#

    //// SOURCE=System_Int16.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                         # System_Int16.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Int16.fsx"|])>]
    //let ``System_Int16_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Int16_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Int16.dll"               # System_Int16_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Int16_Consumer.fsx"|])>]
    //let ``System_Int16_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Int32.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                         # System_Int32.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Int32.fsx"|])>]
    //let ``System_Int32_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Int32_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Int32.dll"               # System_Int32_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Int32_Consumer.fsx"|])>]
    //let ``System_Int32_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Int64.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                         # System_Int64.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Int64.fsx"|])>]
    //let ``System_Int64_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Int64_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll   -r System_Int64.dll"             # System_Int64_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Int64_Consumer.fsx"|])>]
    //let ``System_Int64_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_UInt16.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                        # System_UInt16.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_UInt16.fsx"|])>]
    //let ``System_UInt16_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_UInt16_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_UInt16.dll"             # System_UInt16_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_UInt16_Consumer.fsx"|])>]
    //let ``System_UInt16_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_UInt32.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                        # System_UInt32.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_UInt32.fsx"|])>]
    //let ``System_UInt32_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_UInt32_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_UInt32.dll"             # System_UInt32_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_UInt32_Consumer.fsx"|])>]
    //let ``System_UInt32_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_UInt64.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                        # System_UInt64.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_UInt64.fsx"|])>]
    //let ``System_UInt64_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_UInt64_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_UInt64.dll"             # System_UInt64_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_UInt64_Consumer.fsx"|])>]
    //let ``System_UInt64_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Char.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                          # System_Char.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Char.fsx"|])>]
    //let ``System_Char_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Char_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Char.dll"                 # System_Char_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Char_Consumer.fsx"|])>]
    //let ``System_Char_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Byte.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                          # System_Byte.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Byte.fsx"|])>]
    //let ``System_Byte_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Byte_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Byte.dll"                 # System_Byte_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Byte_Consumer.fsx"|])>]
    //let ``System_Byte_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_SByte.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                         # System_SByte.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_SByte.fsx"|])>]
    //let ``System_SByte_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_SByte_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_SByte.dll"               # System_SByte_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_SByte_Consumer.fsx"|])>]
    //let ``System_SByte_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Single.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                        # System_Single.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Single.fsx"|])>]
    //let ``System_Single_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Single_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Single.dll"             # System_Single_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Single_Consumer.fsx"|])>]
    //let ``System_Single_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Double.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                        # System_Double.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Double.fsx"|])>]
    //let ``System_Double_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Double_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Double.dll"             # System_Double_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Double_Consumer.fsx"|])>]
    //let ``System_Double_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_String.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                        # System_String.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_String.fsx"|])>]
    //let ``System_String_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_String_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_String.dll"             # System_String_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_String_Consumer.fsx"|])>]
    //let ``System_String_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_DateTimeKind.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                  # System_DateTimeKind.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_DateTimeKind.fsx"|])>]
    //let ``System_DateTimeKind_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_DateTimeKind_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_DateTimeKind.dll"	# System_DateTimeKind_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_DateTimeKind_Consumer.fsx"|])>]
    //let ``System_DateTimeKind_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Type.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                          # System_Type.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Type.fsx"|])>]
    //let ``System_Type_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Type_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Type.dll"                 # System_Type_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Type_Consumer.fsx"|])>]
    //let ``System_Type_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Object.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                        # System_Object.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Object.fsx"|])>]
    //let ``System_Object_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    //// SOURCE=System_Object_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_Object.dll"             # System_Object_Consumer.fsx
    //[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_Object_Consumer.fsx"|])>]
    //let ``System_Object_Consumer_fsx`` compilation =
    //    compilation
    //    |> verifyCompile
    //    |> shouldSucceed

    // SOURCE=System_TypeArray.fsx SCFLAGS="-a -r CSLibraryWithAttributes.dll"                                     # System_TypeArray.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_TypeArray.fsx"|])>]
    let ``System_TypeArray_fsx`` compilation =
        compilation
        |> verifyCompile
        |> shouldSucceed

    // SOURCE=System_TypeArray_Consumer.fsx SCFLAGS="-r CSLibraryWithAttributes.dll -r System_TypeArray.dll"       # System_TypeArray_Consumer.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"System_TypeArray_Consumer.fsx"|])>]
    let ``System_TypeArray_Consumer_fsx`` compilation =
        let system_TypeArray =
            FsFromPath (__SOURCE_DIRECTORY__ ++ "System_TypeArray.fsx")
            |> asLibrary

        compilation
        |> withReferences [system_TypeArray]
        |> verifyCompileAndRun
        |> shouldSucceed
