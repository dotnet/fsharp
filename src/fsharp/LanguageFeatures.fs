// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Coordinating compiler operations - configuration, loading initial context, reporting errors etc.
module internal FSharp.Compiler.Features

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

[<RequireQualifiedAccess>]
type LanguageFeature =
    | SingleUnderscorePattern
    | WildCardInForLoop
    | RelaxWhitespace
    | RelaxWhitespace2
    | NameOf
    | ImplicitYield
    | OpenTypeDeclaration
    | DotlessFloat32Literal
    | PackageManagement
    | FromEndSlicing
    | FixedIndexSlice3d4d
    | AndBang
    | ResumableStateMachines
    | NullableOptionalInterop
    | DefaultInterfaceMemberConsumption
    | WitnessPassing
    | AdditionalTypeDirectedConversions
    | InterfacesWithMultipleGenericInstantiation
    | StringInterpolation
    | OverloadsForCustomOperations
    | ExpandedMeasurables
    | StructActivePattern
    | PrintfBinaryFormat
    | IndexerNotationWithoutDot
    | RefCellNotationInformationals
    | UseBindingValueDiscard
    | NonVariablePatternsToRightOfAsPatterns
    | AttributesToRightOfModuleKeyword
    | MLCompatRevisions

/// LanguageVersion management
type LanguageVersion (versionText) =

    // When we increment language versions here preview is higher than current RTM version
    static let languageVersion46 = 4.6m
    static let languageVersion47 = 4.7m
    static let languageVersion50 = 5.0m
    static let previewVersion = 9999m                   // Language version when preview specified
    static let defaultVersion = languageVersion50       // Language version when default specified
    static let latestVersion = defaultVersion           // Language version when latest specified
    static let latestMajorVersion = languageVersion50   // Language version when latestmajor specified

    static let validOptions = [| "preview"; "default"; "latest"; "latestmajor" |]
    static let languageVersions = set [| languageVersion46; languageVersion47 ; languageVersion50 |]

    static let features =
        dict [
            // F# 4.7
            LanguageFeature.SingleUnderscorePattern, languageVersion47
            LanguageFeature.WildCardInForLoop, languageVersion47
            LanguageFeature.RelaxWhitespace, languageVersion47
            LanguageFeature.ImplicitYield, languageVersion47

            // F# 5.0
            LanguageFeature.FixedIndexSlice3d4d, languageVersion50
            LanguageFeature.DotlessFloat32Literal, languageVersion50
            LanguageFeature.AndBang, languageVersion50
            LanguageFeature.NullableOptionalInterop, languageVersion50
            LanguageFeature.DefaultInterfaceMemberConsumption, languageVersion50
            LanguageFeature.OpenTypeDeclaration, languageVersion50
            LanguageFeature.PackageManagement, languageVersion50
            LanguageFeature.WitnessPassing, languageVersion50
            LanguageFeature.InterfacesWithMultipleGenericInstantiation, languageVersion50
            LanguageFeature.NameOf, languageVersion50
            LanguageFeature.StringInterpolation, languageVersion50

            // F# preview
            LanguageFeature.AdditionalTypeDirectedConversions, previewVersion
            LanguageFeature.RelaxWhitespace2, previewVersion
            LanguageFeature.OverloadsForCustomOperations, previewVersion
            LanguageFeature.ExpandedMeasurables, previewVersion
            LanguageFeature.FromEndSlicing, previewVersion
            LanguageFeature.ResumableStateMachines, previewVersion
            LanguageFeature.StructActivePattern, previewVersion
            LanguageFeature.PrintfBinaryFormat, previewVersion
            LanguageFeature.IndexerNotationWithoutDot, previewVersion
            LanguageFeature.RefCellNotationInformationals, previewVersion
            LanguageFeature.UseBindingValueDiscard, previewVersion
            LanguageFeature.NonVariablePatternsToRightOfAsPatterns, previewVersion
            LanguageFeature.AttributesToRightOfModuleKeyword, previewVersion
            LanguageFeature.MLCompatRevisions,previewVersion
        ]

    static let defaultLanguageVersion = LanguageVersion("default")

    let specified =
        match versionText with
        | "?" -> 0m
        | "preview" -> previewVersion
        | "default" -> defaultVersion
        | "latest" -> latestVersion
        | "latestmajor" -> latestMajorVersion
        | "4.6" -> languageVersion46
        | "4.7" -> languageVersion47
        | "5.0" -> languageVersion50
        | _ -> 0m

    let versionToString v =
        if v = previewVersion then "'preview'"
        else string v

    let specifiedString = versionToString specified

    /// Check if this feature is supported by the selected langversion
    member _.SupportsFeature featureId =
        match features.TryGetValue featureId with
        | true, v -> v <= specified
        | false, _ -> false

    /// Has preview been explicitly specified
    member _.IsExplicitlySpecifiedAs50OrBefore() =
        match versionText with
        | "4.6" -> true
        | "4.7" -> true
        | "5.0" -> true
        | _ -> false

    /// Has preview been explicitly specified
    member _.IsPreviewEnabled =
        specified = previewVersion

    /// Does the languageVersion support this version string
    member _.ContainsVersion version =
        match version with
        | "?" | "preview" | "default" | "latest" | "latestmajor" -> true
        | _ -> languageVersions.Contains specified

    /// Get a list of valid strings for help text
    member _.ValidOptions = validOptions

    /// Get a list of valid versions for help text
    member _.ValidVersions =
        [|
            for v in languageVersions |> Seq.sort ->
                sprintf "%M%s" v (if v = defaultVersion then " (Default)" else "")
        |]

    /// Get the text used to specify the version
    member _.VersionText = versionText

    /// Get the specified LanguageVersion
    member _.SpecifiedVersion = specified

    /// Get the specified LanguageVersion as a string
    member _.SpecifiedVersionString = specifiedString

    /// Get a string name for the given feature.
    member _.GetFeatureString feature =
        match feature with
        | LanguageFeature.SingleUnderscorePattern -> FSComp.SR.featureSingleUnderscorePattern()
        | LanguageFeature.WildCardInForLoop -> FSComp.SR.featureWildCardInForLoop()
        | LanguageFeature.RelaxWhitespace -> FSComp.SR.featureRelaxWhitespace()
        | LanguageFeature.RelaxWhitespace2 -> FSComp.SR.featureRelaxWhitespace2()
        | LanguageFeature.NameOf -> FSComp.SR.featureNameOf()
        | LanguageFeature.ImplicitYield -> FSComp.SR.featureImplicitYield()
        | LanguageFeature.OpenTypeDeclaration -> FSComp.SR.featureOpenTypeDeclaration()
        | LanguageFeature.DotlessFloat32Literal -> FSComp.SR.featureDotlessFloat32Literal()
        | LanguageFeature.PackageManagement -> FSComp.SR.featurePackageManagement()
        | LanguageFeature.FromEndSlicing -> FSComp.SR.featureFromEndSlicing()
        | LanguageFeature.FixedIndexSlice3d4d -> FSComp.SR.featureFixedIndexSlice3d4d()
        | LanguageFeature.AndBang -> FSComp.SR.featureAndBang()
        | LanguageFeature.ResumableStateMachines -> FSComp.SR.featureResumableStateMachines()
        | LanguageFeature.NullableOptionalInterop -> FSComp.SR.featureNullableOptionalInterop()
        | LanguageFeature.DefaultInterfaceMemberConsumption -> FSComp.SR.featureDefaultInterfaceMemberConsumption()
        | LanguageFeature.WitnessPassing -> FSComp.SR.featureWitnessPassing()
        | LanguageFeature.AdditionalTypeDirectedConversions -> FSComp.SR.featureAdditionalImplicitConversions()
        | LanguageFeature.InterfacesWithMultipleGenericInstantiation -> FSComp.SR.featureInterfacesWithMultipleGenericInstantiation()
        | LanguageFeature.StringInterpolation -> FSComp.SR.featureStringInterpolation()
        | LanguageFeature.OverloadsForCustomOperations -> FSComp.SR.featureOverloadsForCustomOperations()
        | LanguageFeature.ExpandedMeasurables -> FSComp.SR.featureExpandedMeasurables()
        | LanguageFeature.StructActivePattern -> FSComp.SR.featureStructActivePattern()
        | LanguageFeature.PrintfBinaryFormat -> FSComp.SR.featurePrintfBinaryFormat()
        | LanguageFeature.IndexerNotationWithoutDot -> FSComp.SR.featureIndexerNotationWithoutDot()
        | LanguageFeature.RefCellNotationInformationals -> FSComp.SR.featureRefCellNotationInformationals()
        | LanguageFeature.UseBindingValueDiscard -> FSComp.SR.featureDiscardUseValue()
        | LanguageFeature.NonVariablePatternsToRightOfAsPatterns -> FSComp.SR.featureNonVariablePatternsToRightOfAsPatterns()
        | LanguageFeature.AttributesToRightOfModuleKeyword -> FSComp.SR.featureAttributesToRightOfModuleKeyword()
        | LanguageFeature.MLCompatRevisions -> FSComp.SR.featureMLCompatRevisions()

    /// Get a version string associated with the given feature.
    member _.GetFeatureVersionString feature =
        match features.TryGetValue feature with
        | true, v -> versionToString v
        | _ -> invalidArg "feature" "Internal error: Unable to find feature."

    override x.Equals(yobj: obj) =
        match yobj with 
        | :? LanguageVersion as y -> x.SpecifiedVersion = y.SpecifiedVersion
        | _ -> false

    override x.GetHashCode() = hash x.SpecifiedVersion

    static member Default = defaultLanguageVersion
