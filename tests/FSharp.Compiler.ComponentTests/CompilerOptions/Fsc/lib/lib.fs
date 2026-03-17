// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test.Compiler

// Migrated from FSharpQA suite - CompilerOptions/fsc/lib
// Tests for --lib (-I) compiler option
module Lib =

    // Dummy source for testing lib option variants
    let dummySource = """
// #NoMT #CompilerOptions 
exit 0
"""

    //----------------------------------------------------
    // Valid --lib / -I syntax variants (all should succeed)
    //----------------------------------------------------

    [<Theory>]
    [<InlineData("-I .")>]
    [<InlineData("-I:.")>]
    [<InlineData("--lib:.")>]
    let ``lib - valid syntax variants`` (options: string) =
        Fs dummySource
        |> asExe
        |> withOptions (options.Split(' ') |> Array.toList)
        |> compile
        |> shouldSucceed
        |> ignore

    //----------------------------------------------------
    // Invalid option syntax (error FS0243 - unrecognized option)
    //----------------------------------------------------

    [<Theory>]
    [<InlineData("--I:Folder", "--I")>]          // error01.fs
    [<InlineData("--I Folder", "--I")>]          // error02.fs  
    [<InlineData("-lib:Folder1", "-lib")>]       // error03.fs
    [<InlineData("--LIB:Folder1", "--LIB")>]     // error05.fs (case-sensitive)
    [<InlineData("-i:Folder1", "-i")>]           // error06.fs (case-sensitive)
    [<InlineData("--libb:Folder1", "--libb")>]   // error07.fs (misspelled)
    let ``lib - unrecognized option`` (options: string, expectedOption: string) =
        Fs dummySource
        |> asExe
        |> withOptions (options.Split(' ') |> Array.toList)
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 243, Line 0, Col 1, Line 0, Col 1, $"Unrecognized option: '{expectedOption}'. Use '--help' to learn about recognized command line options.")
        ]
        |> ignore

    //----------------------------------------------------
    // Missing argument (error FS0224)
    //----------------------------------------------------

    [<Theory>]
    [<InlineData("--lib Folder1")>]  // error04.fs - --lib requires colon
    [<InlineData("--lib:")>]         // error09.fs - empty after colon
    [<InlineData("--lib")>]          // error10.fs - completely missing
    let ``lib - missing argument`` (options: string) =
        Fs dummySource
        |> asExe
        |> withOptions (options.Split(' ') |> Array.toList)
        |> compile
        |> shouldFail
        |> withErrorCode 224
        |> ignore

    //----------------------------------------------------
    // Warning for non-existent folder (warning FS0211)
    //----------------------------------------------------

    [<Fact>]
    let ``lib - folder does not exist produces warning`` () =
        Fs dummySource
        |> asExe
        |> withOptions ["--lib:FolderThatDoesNotExist"]
        |> compile
        |> withWarningCode 211
        |> withDiagnosticMessageMatches "FolderThatDoesNotExist"
        |> ignore

    //----------------------------------------------------
    // Comma-separated paths
    //----------------------------------------------------

    [<Fact>]
    let ``lib - comma separated paths`` () =
        Fs dummySource
        |> asExe
        |> withOptions ["--lib:.,.."]
        |> compile
        |> shouldSucceed
        |> ignore

    //----------------------------------------------------
    // Multiple --lib options
    //----------------------------------------------------

    [<Fact>]
    let ``lib - multiple lib options`` () =
        Fs dummySource
        |> asExe
        |> withOptions ["--lib:."; "--lib:.."]
        |> compile
        |> shouldSucceed
        |> ignore
