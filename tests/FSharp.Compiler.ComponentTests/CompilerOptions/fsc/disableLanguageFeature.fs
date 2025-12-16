// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module disableLanguageFeature =

    [<Fact>]
    let ``disableLanguageFeature with valid feature name should typecheck successfully``() =
        FSharp """
printfn "Hello, World"
        """
        |> withOptions ["--disableLanguageFeature:NameOf"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``disableLanguageFeature should disable NameOf feature``() =
        FSharp """
let x = 5
let name = nameof(x)
        """
        |> withOptions ["--langversion:latest"; "--disableLanguageFeature:NameOf"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39
        |> withDiagnosticMessageMatches "The value or constructor 'nameof' is not defined"
        |> ignore

    [<Fact>]
    let ``disableLanguageFeature with invalid feature name should fail``() =
        FSharp """
printfn "Hello, World"
        """
        |> withOptions ["--disableLanguageFeature:InvalidFeatureName"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3879
        |> withDiagnosticMessageMatches "Unrecognized language feature name"
        |> ignore

    [<Fact>]
    let ``disableLanguageFeature can be used multiple times``() =
        FSharp """
let x = 5
let name = nameof(x)
        """
        |> withOptions ["--langversion:latest"; "--disableLanguageFeature:NameOf"; "--disableLanguageFeature:StringInterpolation"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39
        |> ignore

    [<Fact>]
    let ``disableLanguageFeature is case insensitive``() =
        FSharp """
let x = 5
let name = nameof(x)
        """
        |> withOptions ["--langversion:latest"; "--disableLanguageFeature:nameof"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39
        |> ignore
