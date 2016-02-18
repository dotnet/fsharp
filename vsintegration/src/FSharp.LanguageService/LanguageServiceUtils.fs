// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.Configuration
open System.Diagnostics

module FSharpCommonConstants =
    [<Literal>]
    let packageGuid = "871D2A70-12A2-4e42-9440-425DD92A4116"
    [<Literal>]
    let languageServiceGuid = "BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B"
    [<Literal>]
    let editorFactoryGuid = "4EB7CCB7-4336-4FFD-B12B-396E9FD079A9"
    [<Literal>]
    let FSharpLanguageName = "F#"
    [<Literal>]
    let FSharpContentTypeName = "F#"
    [<Literal>]
    let FSharpLanguageServiceCallbackName = "F# Language Service"

module LanguageServiceUtils = 

    // This key can have 'true' and 'false' values. Will default to 'true'.
    let private shouldEnableLanguageServiceKey = "enable-fsharp-language-service"

    let private getConfigValue(key: string) =
        try
            ConfigurationManager.AppSettings.[key]
        with ex -> 
            Debug.Assert(false, sprintf "Error loading 'devenv.exe.config' configuration[%s]: %A" key ex)
            String.Empty

    let shouldEnableLanguageService =
        match getConfigValue(shouldEnableLanguageServiceKey).ToLower() with
        | "false" -> false
        | _ -> true
