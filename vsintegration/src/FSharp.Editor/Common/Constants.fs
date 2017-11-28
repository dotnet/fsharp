// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Configuration
open System.Diagnostics
open Microsoft.CodeAnalysis.Classification

[<RequireQualifiedAccess>]
module internal FSharpConstants =
    
    [<Literal>]
    /// "871D2A70-12A2-4e42-9440-425DD92A4116"
    let packageGuidString = "871D2A70-12A2-4e42-9440-425DD92A4116"
    
    [<Literal>]
    /// "BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B"
    let languageServiceGuidString = "BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B"
    
    [<Literal>]
    /// "F#"
    let FSharpLanguageName = "F#"
    
    [<Literal>]
    /// "F#"
    let FSharpContentTypeName = "F#"

    [<Literal>]
    /// "F# Signature Help"
    let FSharpSignatureHelpContentTypeName = "F# Signature Help"
    
    [<Literal>]
    /// "F# Language Service"
    let FSharpLanguageServiceCallbackName = "F# Language Service"
    
    [<Literal>]
    /// "FSharp"
    let FSharpLanguageLongName = "FSharp"

[<RequireQualifiedAccess>]
module internal FSharpProviderConstants =

    [<Literal>]
    /// "Session Capturing Quick Info Source Provider"
    let SessionCapturingProvider = "Session Capturing Quick Info Source Provider"


[<RequireQualifiedAccess>]
module internal Guids =
    
    [<Literal>]
    /// "9B164E40-C3A2-4363-9BC5-EB4039DEF653"
    let svsSettingsPersistenceManagerIdString = "9B164E40-C3A2-4363-9BC5-EB4039DEF653"

    [<Literal>]
    /// "9b3c6b8a-754a-461d-9ebe-de1a682d57c1"
    let intelliSenseOptionPageIdString = "9b3c6b8a-754a-461d-9ebe-de1a682d57c1"

    [<Literal>]
    /// "1e2b3290-4d67-41ff-a876-6f41f868e28f"
    let quickInfoOptionPageIdString = "1e2b3290-4d67-41ff-a876-6f41f868e28f"

    [<Literal>]
    /// "9A66EB6A-DE52-4169-BC26-36FBD4312FD7"
    let codeFixesOptionPageIdString = "9A66EB6A-DE52-4169-BC26-36FBD4312FD7"

    [<Literal>]
    /// "8FDA964A-263D-4B4E-9560-29897535217C"
    let languageServicePerformanceOptionPageIdString = "8FDA964A-263D-4B4E-9560-29897535217C"

    let blueHighContrastThemeId = Guid "{ce94d289-8481-498b-8ca9-9b6191a315b9}"
