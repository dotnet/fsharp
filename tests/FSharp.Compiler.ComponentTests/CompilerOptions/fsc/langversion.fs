// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module langversion =

    [<Theory>]
    [<InlineData("4.6")>]
    [<InlineData("4.7")>]
    [<InlineData("5")>]
    [<InlineData("5.0")>]
    [<InlineData("6")>]
    [<InlineData("6.0")>]
    [<InlineData("7")>]
    [<InlineData("7.0")>]
    let ``Issue 20666 - fsc unsupported langversion identifies the last compatible SDK feature band`` (version: string) =
        let expected =
            $"Language version '{version}' is out of support. The last compatible SDK feature band is .NET SDK 10.0.1xx. Download it at https://dotnet.microsoft.com/en-us/download/dotnet/10.0."

        FSharp """printfn "Hello, World" """
        |> asExe
        |> withOptions [$"--langversion:{version}"]
        |> compile
        |> shouldFail
        |> withErrorCode 3880
        |> withDiagnosticMessage expected
        |> ignore

    [<Theory>]
    [<InlineData("default")>]
    [<InlineData("latest")>]
    let ``fsc langversion supports named selectors`` (version: string) =
        FSharp """printfn "Hello, World" """
        |> asExe
        |> withOptions [$"--langversion:{version}"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsc langversion is case-insensitive - --langversion:pRevIew``() =
        FSharp """
printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--langversion:pRevIew"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsi langversion is case-insensitive - --langversion:pRevIew``() =
        FSharp """
printfn "Hello, World"
        """
        |> asFsx
        |> withOptions ["--langversion:pRevIew"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsc langversion is case-insensitive - --langversion:laTestmaJor``() =
        FSharp """
printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--langversion:laTestmaJor"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsi langversion is case-insensitive - --langversion:laTestmaJor``() =
        FSharp """
printfn "Hello, World"
        """
        |> asFsx
        |> withOptions ["--langversion:laTestmaJor"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsc langversion supports simple version number - --langversion:8``() =
        FSharp """
    printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--langversion:8"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsc langversion supports full version number - --langversion:8_0``() =
        FSharp """
    printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--langversion:8.0"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsc langversion supports full version number - --langversion:4``() =
        FSharp """
    printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--langversion:4"]
        |> compile
        |> shouldFail
        |> withErrorCode 246
        |> withDiagnosticMessageMatches "Unrecognized value '4' for --langversion use --langversion:\? for complete list"
        |> ignore

    [<Fact>]
    let ``fsc langversion fails with invalid version number - --langversion:4_1 which never existed``() =
        FSharp """
    printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--langversion:4.1"]
        |> compile
        |> shouldFail
        |> withErrorCode 246
        |> withDiagnosticMessageMatches "Unrecognized value '4.1' for --langversion use --langversion:\? for complete list"
        |> ignore
