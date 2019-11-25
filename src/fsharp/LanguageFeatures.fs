// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Coordinating compiler operations - configuration, loading initial context, reporting errors etc.
module internal FSharp.Compiler.Features

open System

//------------------------------------------------------------------------------------------------------------------
// Language version command line switch
//------------------------------------------------------------------------------------------------------------------
// Add your features to this List - in code use languageVersion.SupportsFeature(LanguageFeatures.yourFeature) 
// a return value of false means your feature is not supported by the user's language selection
// All new language features added from now on must be protected by this.
// Note:
//   *  The fslang design process will require a decision about feature name and whether it is required.
//   *  When a feature is assigned a release language, we will scrub the code of feature references and apply
//      the Release Language version.

/// LanguageFeature enumeration
[<RequireQualifiedAccess>]
type LanguageFeature =
    | PreviewVersion = 0
    | SingleUnderscorePattern = 3
    | WildCardInForLoop = 4
    | RelaxWhitespace = 5
    | NameOf = 6
    | ImplicitYield = 7
    | OpenStaticClasses = 8
    | PackageManagement = 9
    | FixedIndexSlice3d4d = 11
    | LanguageVersion46 = 0x10046
    | LanguageVersion47 = 0x10047
    | LanguageVersion50 = 0x10050

/// LanguageVersion management
type LanguageVersion (specifiedVersionAsString) =

    // When we increment language versions here preview is higher than current RTM version
    static let languageVersion46 = 4.6m
    static let languageVersion47 = 4.7m
    static let languageVersion50 = 5.0m
    static let previewVersion = 9999m                   // Language version when preview specified
    static let defaultVersion = languageVersion50       // Language version when default specified
    static let latestVersion = defaultVersion           // Language version when latest specified
    static let latestMajorVersion = languageVersion47   // Language version when latestmajor specified

    static let validOptions = [| "preview"; "default"; "latest"; "latestmajor" |]
    static let languageVersions = set [| languageVersion46; languageVersion47; languageVersion50 |]

    static let features =
        dict [
            // Add new LanguageVersions here ...
            LanguageFeature.LanguageVersion46, languageVersion46
            LanguageFeature.LanguageVersion47, languageVersion47
            LanguageFeature.LanguageVersion50, languageVersion50
            LanguageFeature.PreviewVersion, previewVersion

            // F# 4.7
            LanguageFeature.SingleUnderscorePattern, languageVersion47
            LanguageFeature.WildCardInForLoop, languageVersion47
            LanguageFeature.RelaxWhitespace, languageVersion47
            LanguageFeature.ImplicitYield, languageVersion47

            // Add new Language Features here...
            LanguageFeature.NameOf, previewVersion
            LanguageFeature.OpenStaticClasses, previewVersion
            LanguageFeature.PackageManagement, previewVersion
            LanguageFeature.FixedIndexSlice3d4d, previewVersion
        ]

    let specified =
        match specifiedVersionAsString with
        | "?" -> 0m
        | "preview" -> previewVersion
        | "default" -> defaultVersion
        | "latest" -> latestVersion
        | "latestmajor" -> latestMajorVersion
        | "4.6" -> languageVersion46
        | "4.7" -> languageVersion47
        | "5.0" -> languageVersion50
        | _ -> 0m

    /// Check if this feature is supported by the selected langversion
    member __.SupportsFeature featureId =
        match features.TryGetValue featureId with
        | true, v -> v <= specified
        | false, _ -> false

    /// Does the languageVersion support this version string
    member __.ContainsVersion version =
        match version with
        | "?" | "preview" | "default" | "latest" | "latestmajor" -> true
        | _ -> languageVersions.Contains specified

    /// Get a list of valid strings for help text
    member __.ValidOptions = validOptions

    /// Get a list of valid versions for help text
    member __.ValidVersions = [|
        for v in languageVersions |> Seq.sort do
            let label = if v = defaultVersion then " (Default)" else ""
            yield sprintf "%M%s" v label
            |]

    /// Get the specified LanguageVersion
    member __.SpecifiedVersion = specified
