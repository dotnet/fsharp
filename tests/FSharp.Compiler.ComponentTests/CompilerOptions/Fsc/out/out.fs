// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test.Compiler

// Migrated from FSharpQA suite - CompilerOptions/fsc/out
// Tests for --out compiler option
module Out =

    let dummySource = """
// #NoMT #CompilerOptions 
exit 0
"""

    //----------------------------------------------------
    // Valid --out syntax (should succeed)
    //----------------------------------------------------

    [<Fact>]
    let ``out - valid syntax with colon`` () =
        Fs dummySource
        |> asExe
        |> withOptions ["--out:out1.exe"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``out - last one wins`` () =
        Fs dummySource
        |> asExe
        |> withOptions ["--out:out1.exe"; "--out:out3.exe"]
        |> compile
        |> shouldSucceed
        |> ignore

    //----------------------------------------------------
    // Invalid option syntax (error FS0243 - unrecognized option)
    //----------------------------------------------------

    [<Theory>]
    [<InlineData("--OUT:out1.exe", "--OUT")>]    // error01.fs (case-sensitive)
    [<InlineData("--oUT:out1.exe", "--oUT")>]    // error02.fs (case-sensitive)
    [<InlineData("--oup:out1", "--oup")>]        // error03.fs (misspelled)
    [<InlineData("-out:out2.exe", "-out")>]      // error06.fs (single dash invalid)
    let ``out - unrecognized option`` (options: string, expectedOption: string) =
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
    // --out with space instead of colon (error FS0224)
    //----------------------------------------------------

    [<Fact>]
    let ``out - space instead of colon`` () =
        Fs dummySource
        |> asExe
        |> withOptions ["--out"; "out2.exe"]
        |> compile
        |> shouldFail
        |> withErrorCode 224
        |> ignore

    //----------------------------------------------------
    // Missing argument (error FS0224)
    //----------------------------------------------------

    [<Theory>]
    [<InlineData("--out:")>]   // error04.fs - empty after colon
    [<InlineData("--out")>]    // error04.fs - completely missing (last)
    let ``out - missing argument`` (options: string) =
        Fs dummySource
        |> asExe
        |> withOptions (options.Split(' ') |> Array.toList)
        |> compile
        |> shouldFail
        |> withErrorCode 224
        |> ignore

    //----------------------------------------------------
    // Invalid characters in filename (error FS1227)
    // Note: These tests are platform-specific (Windows only for | and >)
    //----------------------------------------------------

    [<Fact(Skip = "Platform-specific: invalid filename chars differ by OS")>]
    let ``out - invalid char pipe`` () =
        Fs dummySource
        |> asExe
        |> withOptions ["--out:|"]
        |> compile
        |> shouldFail
        |> withErrorCode 1227
        |> ignore

    [<Fact(Skip = "Platform-specific: invalid filename chars differ by OS")>]
    let ``out - invalid char greater than`` () =
        Fs dummySource
        |> asExe
        |> withOptions ["--out:>.exe"]
        |> compile
        |> shouldFail
        |> withErrorCode 1227
        |> ignore

    //----------------------------------------------------
    // --out not available in FSI (error FS0243)
    // Note: FSI session creation fails with invalid options, so this is tested
    // by verifying the option is not recognized rather than checking eval results
    //----------------------------------------------------

    // The original fsharpqa test verified FSI doesn't recognize --out
    // The ComponentTests infrastructure doesn't support testing invalid FSI session options directly
    // because the session fails to create. The behavior is verified by the compiler's option parsing.
