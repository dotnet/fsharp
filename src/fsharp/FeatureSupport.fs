// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.FeatureSupport

open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Range
open FSharp.Compiler.Features

[<Sealed>]
type LanguageFeatureSupport private (langError: (range -> exn) option) =

    member __.IsSupported =
        langError.IsNone

    member __.TryRaiseLanguageError m =
        langError |> Option.iter (fun f -> error (f m))

    /// Returns true if there was an error which means not supported.
    member __.TryRaiseLanguageErrorRecover m =
        langError |> Option.iter (fun f -> errorR (f m))
        langError.IsSome

    static member Create (langVersion: LanguageVersion, featureId) =
        if not (langVersion.SupportsFeature featureId) then
            let featureStr = langVersion.GetFeatureString featureId
            let currentVersionStr = langVersion.CurrentVersionString
            let suggestedVersionStr = langVersion.GetFeatureVersionString featureId
            Some(fun m -> Error(FSComp.SR.chkFeatureNotLanguageSupported(featureStr, currentVersionStr, suggestedVersionStr), m))
        else
            None
        |> LanguageFeatureSupport

[<Sealed>]
type FeatureSupport private (langSupport: LanguageFeatureSupport, runtimeError: (range -> exn) option) =

    member __.IsSupported =
        runtimeError.IsNone && langSupport.IsSupported

    member __.IsRuntimeSupported =
        runtimeError.IsNone

    member __.IsLanguageSupported =
        langSupport.IsSupported

    member __.TryRaiseRuntimeError m =
        runtimeError |> Option.iter (fun f -> error (f m))

    /// Returns true if there was an error which means not runtime supported.
    member __.TryRaiseRuntimeErrorRecover m =
        runtimeError |> Option.iter (fun f -> errorR (f m))
        runtimeError.IsSome

    member __.TryRaiseLanguageError m =
        langSupport.TryRaiseLanguageError m

    /// Returns true if there was an error which means not language supported
    member __.TryRaiseLanguageErrorRecover m =
        langSupport.TryRaiseLanguageErrorRecover m

    static member Create (langSupport, runtimeError) =
        FeatureSupport (langSupport, runtimeError)