// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module disableLanguageFeature =

    [<Fact>]
    let ``disableLanguageFeature with valid feature name should compile successfully``() =
        FSharp """
printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--disableLanguageFeature:NameOf"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``disableLanguageFeature should disable NameOf feature``() =
        FSharp """
let x = 5
let name = nameof(x)
        """
        |> asExe
        |> withOptions ["--langversion:latest"; "--disableLanguageFeature:NameOf"]
        |> compile
        |> shouldFail
        |> ignore

    [<Fact>]
    let ``disableLanguageFeature with invalid feature name should fail``() =
        FSharp """
printfn "Hello, World"
        """
        |> asExe
        |> withOptions ["--disableLanguageFeature:InvalidFeatureName"]
        |> compile
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
        |> asExe
        |> withOptions ["--langversion:latest"; "--disableLanguageFeature:NameOf"; "--disableLanguageFeature:StringInterpolation"]
        |> compile
        |> shouldFail
        |> ignore

    [<Fact>]
    let ``disableLanguageFeature is case insensitive``() =
        FSharp """
let x = 5
let name = nameof(x)
        """
        |> asExe
        |> withOptions ["--langversion:latest"; "--disableLanguageFeature:nameof"]
        |> compile
        |> shouldFail
        |> ignore
