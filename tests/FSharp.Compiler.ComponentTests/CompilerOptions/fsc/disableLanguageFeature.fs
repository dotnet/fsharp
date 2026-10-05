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
        |> withOptions ["--disableLanguageFeature:StringInterpolation"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``disableLanguageFeature with invalid feature name should fail``() =
        FSharp """
printfn "Hello, World"
        """
        |> withOptions ["--disableLanguageFeature:InvalidFeatureName"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3881
        |> withDiagnosticMessageMatches "Unrecognized language feature name"
        |> ignore
