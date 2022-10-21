// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System

[<RequireQualifiedAccess>]
module internal FSharpConstants =
    
    [<Literal>]
    /// "871D2A70-12A2-4e42-9440-425DD92A4116"
    let packageGuidString = "871D2A70-12A2-4e42-9440-425DD92A4116"

    [<Literal>]
    /// "871D2A70-12A2-4e42-9440-425DD92A4116" - FSharp Package
    let fsiPackageGuidString = "871D2A70-12A2-4e42-9440-425DD92A4116"
    
    [<Literal>]
    /// "BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B"
    let languageServiceGuidString = "BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B"

    [<Literal>]
    /// "91a04a73-4f2c-4e7c-ad38-c1a68e7da05c"
    let projectPackageGuidString = "91a04a73-4f2c-4e7c-ad38-c1a68e7da05c"

    [<Literal>]
    /// "F#"
    let FSharpLanguageName = "F#"
    
    [<Literal>]
    /// "F#"
    let FSharpContentTypeName = "F#"

    [<Literal>]
    /// ".fs"
    let FSharpFileExtension = ".fs"

    [<Literal>]
    /// "F# Signature Help"
    let FSharpSignatureHelpContentTypeName = "F# Signature Help"
    
    [<Literal>]
    /// "F# Language Service"
    let FSharpLanguageServiceCallbackName = "F# Language Service"
    
    [<Literal>]
    /// "FSharp"
    let FSharpLanguageLongName = "FSharp"

    [<Literal>]
    /// "F# Miscellaneous Files"
    let FSharpMiscellaneousFilesName = "F# Miscellaneous Files"

    [<Literal>]
    /// "F# Metadata"
    let FSharpMetadataName = "F# Metadata"

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
    /// "00BE7FD9-8145-4A2E-A1BF-3BAF0F4F47DD"
    let lensOptionPageIdString = "00BE7FD9-8145-4A2E-A1BF-3BAF0F4F47DD"

    [<Literal>]
    /// "8FDA964A-263D-4B4E-9560-29897535217C"
    let languageServicePerformanceOptionPageIdString = "8FDA964A-263D-4B4E-9560-29897535217C"

    [<Literal>]
    /// "9007718C-357A-4327-A193-AB3EC38D7EE8"
    let advancedSettingsPageIdSring = "9007718C-357A-4327-A193-AB3EC38D7EE8"

    [<Literal>]
    /// "9EBEBCE8-A79B-46B0-A8C5-A9818AEED17D"
    let formattingOptionPageIdString = "9EBEBCE8-A79B-46B0-A8C5-A9818AEED17D"

    let blueHighContrastThemeId = Guid "{ce94d289-8481-498b-8ca9-9b6191a315b9}"
