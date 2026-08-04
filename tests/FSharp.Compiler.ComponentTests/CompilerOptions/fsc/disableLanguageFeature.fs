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

    // Guard test: %B (binary integer formatting) parses, compiles and runs correctly now that
    // the PrintfBinaryFormat feature flag is removed. The old flag gated %B at langversion 6.0,
    // but the minimum supported langversion is 8.0 (anything lower errors with FS3880), so every
    // supported langversion is already above the old gate - the removal is a pure no-op for all
    // supported versions and there is no sub-6.0 langversion at which to observe the difference.
    // This asserts the %B code path itself is intact after deleting the guard.
    [<Fact>]
    let ``PrintfBinaryFormat is unconditional - percent B compiles and runs``() =
        FSharp """
module Test
let s = sprintf "%B" 19
if s <> "10011" then failwithf "expected 10011 got %s" s
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    // RED driver: after the flag is removed, PrintfBinaryFormat is no longer a recognized
    // language-feature name, so --disableLanguageFeature:PrintfBinaryFormat must be rejected
    // with error 3881 "Unrecognized language feature name".
    // On the CURRENT (pre-removal) compiler this FAILS, because the name is still recognized
    // (the flag exists), so no 3881 is produced. That failure is the intended RED state.
    [<Fact>]
    let ``disableLanguageFeature PrintfBinaryFormat is not a recognized feature``() =
        FSharp """
printfn "Hello, World"
        """
        |> withOptions ["--disableLanguageFeature:PrintfBinaryFormat"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3881
        |> withDiagnosticMessageMatches "Unrecognized language feature name"
        |> ignore
