// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module langversion =

    [<Fact>]
    let ``fsc langversion is case insensitive - --langversion:pRevIew``() =
        FSharp """
printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--langversion:pRevIew"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsi langversion is case insensitive - --langversion:pRevIew``() =
        FSharp """
printfn "Hello, World"
        """
        |> asFsx
        |> withOptions ["--langversion:pRevIew"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsc langversion is case insensitive - --langversion:laTestmaJor``() =
        FSharp """
printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--langversion:laTestmaJor"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsi langversion is case insensitive - --langversion:laTestmaJor``() =
        FSharp """
printfn "Hello, World"
        """
        |> asFsx
        |> withOptions ["--langversion:laTestmaJor"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsc langversion supports simple version number - --langversion:5``() =
        FSharp """
    printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--langversion:5"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsc langversion supports full version number - --langversion:5.0``() =
        FSharp """
    printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--langversion:5.0"]
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
    let ``fsc langversion fails with invalid version number - --langversion:4.1 which never existed``() =
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
