// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.Configuration
open System.Diagnostics

module FSRoslynCommonConstants =
    [<Literal>]
    let packageGuid = "871D2A70-12A2-4e42-9440-425DD92A4116"
    [<Literal>]
    let languageServiceGuid = "BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B"
    [<Literal>]
    let editorFactoryGuid = "4EB7CCB7-4336-4FFD-B12B-396E9FD079A9"
    [<Literal>]
    let FSharpLanguageName = "F#"
    [<Literal>]
    let FSharpContentType = "F#"

module LanguageServiceUtils = 

    // This key can have 'off', 'vs', and 'roslyn' states. Will default to 'vs'.
    let private languageServiceTypeKey = "fsharp-language-service"

    let private getConfigValue(key: string) =
        try
            ConfigurationManager.AppSettings.[key].ToLower()
        with ex -> 
            Debug.Assert(false, sprintf "Error loading 'devenv.exe.config' configuration[%s]: %A" key ex)
            String.Empty

    let shouldEnableVSLanguageService =
        match getConfigValue(languageServiceTypeKey) with
        | "off" -> false
        | "roslyn" -> false
        | _ -> true

    let shouldEnableRoslynLanguageService =
        match getConfigValue(languageServiceTypeKey) with
        | "roslyn" -> true
        | _ -> false
