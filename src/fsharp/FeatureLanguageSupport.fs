// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.FeatureLanguageSupport

open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Range
open FSharp.Compiler.Features

[<Sealed>]
type internal LanguageFeatureSupport private (langError: (range -> exn) option) =

    member __.IsSupported =
        langError.IsNone

    member __.TryRaiseLanguageError m =
        langError |> Option.iter (fun f -> error (f m))

    /// Returns true if there was an error which means not supported.
    member __.TryRaiseLanguageErrorRecover m =
        langError |> Option.iter (fun f -> errorR (f m))
        langError.IsSome

    static member From (langVersion: LanguageVersion, featureId) =
        if not (langVersion.SupportsFeature featureId) then
            let featureStr = langVersion.GetFeatureString featureId
            let currentVersionStr = langVersion.CurrentVersionString
            let suggestedVersionStr = langVersion.GetFeatureVersionString featureId
            Some(fun m -> Error(FSComp.SR.chkFeatureNotLanguageSupported(featureStr, currentVersionStr, suggestedVersionStr), m))
        else
            None
        |> LanguageFeatureSupport