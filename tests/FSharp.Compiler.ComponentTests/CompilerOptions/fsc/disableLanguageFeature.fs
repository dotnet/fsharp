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
        // nameof with type parameter requires LanguageFeature.NameOf
        FSharp """
let f<'T>() = nameof<'T>
        """
        |> withOptions ["--langversion:latest"; "--disableLanguageFeature:NameOf"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39
        |> withDiagnosticMessageMatches "The value or constructor 'nameof' is not defined"
        |> ignore

    [<Fact>]
    let ``disableLanguageFeature should disable NestedCopyAndUpdate feature``() =
        // Nested copy and update requires LanguageFeature.NestedCopyAndUpdate  
        FSharp """
type Inner = { X: int }
type Outer = { Inner: Inner }
let o = { Inner = { X = 1 } }
let o2 = { o with Inner.X = 2 }
        """
        |> withOptions ["--langversion:latest"; "--disableLanguageFeature:NestedCopyAndUpdate"]
        |> typecheck
        |> shouldFail
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

    [<Fact>]
    let ``disableLanguageFeature can be used multiple times``() =
        // nameof with type parameter requires LanguageFeature.NameOf
        FSharp """
let f<'T>() = nameof<'T>
        """
        |> withOptions ["--langversion:latest"; "--disableLanguageFeature:NameOf"; "--disableLanguageFeature:StringInterpolation"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39
        |> ignore

    [<Fact>]
    let ``disableLanguageFeature is case insensitive``() =
        // nameof with type parameter requires LanguageFeature.NameOf
        FSharp """
let f<'T>() = nameof<'T>
        """
        |> withOptions ["--langversion:latest"; "--disableLanguageFeature:nameof"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39
        |> ignore
