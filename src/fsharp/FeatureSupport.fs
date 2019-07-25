// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.FeatureSupport

open System
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Range
open FSharp.Compiler.InfoReader
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.Features
open FSharp.Compiler.FeatureLanguageSupport

[<Sealed>]
type internal FeatureSupport private (runtimeError: (range -> exn) option, langSupport: LanguageFeatureSupport) =

    static let runtimeSupportsFeature (infoReader: InfoReader) m featureId =
        let g = infoReader.g
        
        let runtimeFeature =
            match featureId with
            | LanguageFeature.DefaultInterfaceMethodConsumption -> "DefaultImplementationsOfInterfaces"
            | _ -> String.Empty
        
        if String.IsNullOrWhiteSpace runtimeFeature then
            true
        else
            match g.System_Runtime_CompilerServices_RuntimeFeature_ty with
            | Some runtimeFeatureTy ->
                infoReader.GetILFieldInfosOfType (Some runtimeFeature, AccessorDomain.AccessibleFromEverywhere, m, runtimeFeatureTy)
                |> List.exists (fun ilFieldInfo -> ilFieldInfo.FieldName = runtimeFeature)
            | _ ->
                false

    member __.IsSupported =
        runtimeError.IsNone && langSupport.IsSupported

    member __.IsRuntimeSupported =
        runtimeError.IsNone

    member __.IsLanguageSupported =
        langSupport.IsSupported

    member __.TryRaiseRuntimeError m =
        runtimeError |> Option.iter (fun f -> error (f m))

    /// Returns true if there was an error which means not supported.
    member __.TryRaiseRuntimeErrorRecover m =
        runtimeError |> Option.iter (fun f -> errorR (f m))
        runtimeError.IsSome

    member __.TryRaiseLanguageError m =
        langSupport.TryRaiseLanguageError m

    /// Returns true if there was an error.
    member __.TryRaiseLanguageErrorRecover m =
        langSupport.TryRaiseLanguageErrorRecover m

    /// Get feature support for the given feature.
    static member From (infoReader: InfoReader, m, featureId) =
        let g = infoReader.g
    
        let runtimeError =
            if not (runtimeSupportsFeature infoReader m featureId) then
                let featureStr = g.langVersion.GetFeatureString featureId
                Some(fun m -> Error(FSComp.SR.chkFeatureNotRuntimeSupported featureStr, m))
            else
                None
    
        let langSupport = LanguageFeatureSupport.From (g.langVersion, featureId)    
        FeatureSupport (runtimeError, langSupport)