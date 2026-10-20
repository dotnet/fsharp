// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsi

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

// Migrated from FSharpQA suite - CompilerOptions/fsi/langversion
// Tests for invalid langversion arguments that produce errors in FSI.

module Langversion =

    [<Theory>]
    [<InlineData("4.6")>]
    [<InlineData("4.7")>]
    [<InlineData("5")>]
    [<InlineData("5.0")>]
    [<InlineData("6")>]
    [<InlineData("6.0")>]
    [<InlineData("7")>]
    [<InlineData("7.0")>]
    let ``Issue 20666 - fsi unsupported langversion identifies the last compatible SDK feature band`` (version: string) =
        let expected =
            $"Language version '{version}' is out of support. The last compatible SDK feature band is .NET SDK 10.0.1xx. Download it at https://dotnet.microsoft.com/en-us/download/dotnet/10.0."

        let result = runFsiProcess ["--nologo"; "--exec"; $"--langversion:{version}"]
        Assert.NotEqual(0, result.ExitCode)
        Assert.Contains("error FS3880:", result.StdErr)
        Assert.Contains(expected, result.StdErr)
        Assert.DoesNotContain("10.0.2", result.StdErr)
        Assert.DoesNotContain("10.0.4", result.StdErr)

    [<Theory>]
    [<InlineData("8")>]
    [<InlineData("8.0")>]
    [<InlineData("9.0")>]
    [<InlineData("10.0")>]
    [<InlineData("11.0")>]
    [<InlineData("11.2")>]
    [<InlineData("default")>]
    [<InlineData("latest")>]
    [<InlineData("latestmajor")>]
    [<InlineData("preview")>]
    [<InlineData("pRevIew")>]
    [<InlineData("laTestmaJor")>]
    let ``fsi langversion supports numeric versions and named selectors`` (version: string) =
        let result = runFsiProcess ["--nologo"; "--exec"; $"--langversion:{version}"]
        Assert.Equal(0, result.ExitCode)
        Assert.DoesNotContain("FS3880", result.StdErr)

    // Test: --langversion:4.5 - unrecognized version
    // Original: SOURCE=badlangversion.fsx SCFLAGS=" --langversion:4.5"
    [<Fact>]
    let ``fsi langversion - unrecognized version 4.5`` () =
        FSharp """exit 0"""
        |> asFsx
        |> withOptions ["--langversion:4.5"]
        |> compile
        |> shouldFail
        |> withErrorCode 246
        |> withDiagnosticMessageMatches "Unrecognized value '4\\.5' for --langversion"
        |> ignore

    // Test: --langversion:4,7 - comma instead of dot (culture issue)
    // Original: SOURCE=badlangversion-culture.fsx SCFLAGS=" --langversion:4,7"
    [<Fact>]
    let ``fsi langversion - comma instead of dot`` () =
        FSharp """exit 0"""
        |> asFsx
        |> withOptions ["--langversion:4,7"]
        |> compile
        |> shouldFail
        |> withErrorCode 246
        |> withDiagnosticMessageMatches "Unrecognized value '4,7' for --langversion"
        |> ignore

    // Test: --langversion:4.70000000000 - too many decimal places
    // Original: SOURCE=badlangversion-decimal.fsx SCFLAGS=" --langversion:4.70000000000"
    [<Fact>]
    let ``fsi langversion - too many decimal places`` () =
        FSharp """exit 0"""
        |> asFsx
        |> withOptions ["--langversion:4.70000000000"]
        |> compile
        |> shouldFail
        |> withErrorCode 246
        |> withDiagnosticMessageMatches "Unrecognized value '4\\.70000000000' for --langversion"
        |> ignore
