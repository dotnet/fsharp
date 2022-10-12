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
    | BetterExceptionPrinting
    | DelegateTypeNameResolutionFix
    | ReallyLongLists
    | ErrorOnDeprecatedRequireQualifiedAccess
    | RequiredPropertiesSupport
    | InitPropertiesSupport
    | LowercaseDUWhenRequireQualifiedAccess
    | InterfacesWithAbstractStaticMembers
    | SelfTypeConstraints
    | MatchNotAllowedForUnionCaseWithNoData

/// LanguageVersion management
type LanguageVersion(versionText) =

    // When we increment language versions here preview is higher than current RTM version
    static let languageVersion46 = 4.6m
    static let languageVersion47 = 4.7m
    static let languageVersion50 = 5.0m
    static let languageVersion60 = 6.0m
    static let languageVersion70 = 7.0m
    static let previewVersion = 9999m // Language version when preview specified
    static let defaultVersion = languageVersion70 // Language version when default specified
    static let latestVersion = defaultVersion // Language version when latest specified
    static let latestMajorVersion = languageVersion70 // Language version when latestmajor specified

    static let validOptions = [| "preview"; "default"; "latest"; "latestmajor" |]

    static let languageVersions =
        set [| languageVersion46; languageVersion47; languageVersion50; languageVersion60; languageVersion70 |]

    static let features =
        dict
            [
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

                // F# 6.0
                LanguageFeature.AdditionalTypeDirectedConversions, languageVersion60
                LanguageFeature.RelaxWhitespace2, languageVersion60
                LanguageFeature.OverloadsForCustomOperations, languageVersion60
                LanguageFeature.ExpandedMeasurables, languageVersion60
                LanguageFeature.ResumableStateMachines, languageVersion60
                LanguageFeature.StructActivePattern, languageVersion60
                LanguageFeature.PrintfBinaryFormat, languageVersion60
                LanguageFeature.IndexerNotationWithoutDot, languageVersion60
                LanguageFeature.RefCellNotationInformationals, languageVersion60
                LanguageFeature.UseBindingValueDiscard, languageVersion60
                LanguageFeature.NonVariablePatternsToRightOfAsPatterns, languageVersion60
                LanguageFeature.AttributesToRightOfModuleKeyword, languageVersion60
                LanguageFeature.DelegateTypeNameResolutionFix, languageVersion60

                // F# 7.0
                LanguageFeature.MLCompatRevisions, languageVersion70
                LanguageFeature.BetterExceptionPrinting, languageVersion70
                LanguageFeature.ReallyLongLists, languageVersion70
                LanguageFeature.ErrorOnDeprecatedRequireQualifiedAccess, languageVersion70
                LanguageFeature.RequiredPropertiesSupport, languageVersion70
                LanguageFeature.InitPropertiesSupport, languageVersion70
                LanguageFeature.LowercaseDUWhenRequireQualifiedAccess, languageVersion70
                LanguageFeature.InterfacesWithAbstractStaticMembers, languageVersion70
                LanguageFeature.SelfTypeConstraints, languageVersion70

                // F# preview
                LanguageFeature.FromEndSlicing, previewVersion
                LanguageFeature.MatchNotAllowedForUnionCaseWithNoData, previewVersion
            ]

    static let defaultLanguageVersion = LanguageVersion("default")

    static let getVersionFromString (version: string) =
        match version.ToUpperInvariant() with
        | "?" -> 0m
        | "PREVIEW" -> previewVersion
        | "DEFAULT" -> defaultVersion
        | "LATEST" -> latestVersion
        | "LATESTMAJOR" -> latestMajorVersion
        | "4.6" -> languageVersion46
        | "4.7" -> languageVersion47
        | "5.0"
        | "5" -> languageVersion50
        | "6.0"
        | "6" -> languageVersion60
        | "7.0"
        | "7" -> languageVersion70
        | _ -> 0m

    let specified = getVersionFromString versionText

    static let versionToString v =
        if v = previewVersion then "'PREVIEW'" else string v

    let specifiedString = versionToString specified

    /// Check if this feature is supported by the selected langversion
    member _.SupportsFeature featureId =
        match features.TryGetValue featureId with
        | true, v -> v <= specified
        | false, _ -> false

    /// Has preview been explicitly specified
    member _.IsExplicitlySpecifiedAs50OrBefore() =
        let v = getVersionFromString versionText
        v <> 0.0m && v <= 5.0m

    /// Has preview been explicitly specified
    member _.IsPreviewEnabled = specified = previewVersion

    /// Does the languageVersion support this version string
    static member ContainsVersion version =
        let langVersion = getVersionFromString version
        langVersion <> 0m && languageVersions.Contains langVersion

    /// Get a list of valid strings for help text
    static member ValidOptions = validOptions

    /// Get a list of valid versions for help text
    static member ValidVersions =
        [|
            for v in languageVersions |> Seq.sort -> sprintf "%M%s" v (if v = defaultVersion then " (Default)" else "")
        |]

    /// Get the text used to specify the version
    member _.VersionText = versionText

    /// Get the specified LanguageVersion
    member _.SpecifiedVersion = specified

    /// Get the specified LanguageVersion as a string
    member _.SpecifiedVersionString = specifiedString

    /// Get a string name for the given feature.
    static member GetFeatureString feature =
        match feature with
        | LanguageFeature.SingleUnderscorePattern -> FSComp.SR.featureSingleUnderscorePattern ()
        | LanguageFeature.WildCardInForLoop -> FSComp.SR.featureWildCardInForLoop ()
        | LanguageFeature.RelaxWhitespace -> FSComp.SR.featureRelaxWhitespace ()
        | LanguageFeature.RelaxWhitespace2 -> FSComp.SR.featureRelaxWhitespace2 ()
        | LanguageFeature.NameOf -> FSComp.SR.featureNameOf ()
        | LanguageFeature.ImplicitYield -> FSComp.SR.featureImplicitYield ()
        | LanguageFeature.OpenTypeDeclaration -> FSComp.SR.featureOpenTypeDeclaration ()
        | LanguageFeature.DotlessFloat32Literal -> FSComp.SR.featureDotlessFloat32Literal ()
        | LanguageFeature.PackageManagement -> FSComp.SR.featurePackageManagement ()
        | LanguageFeature.FromEndSlicing -> FSComp.SR.featureFromEndSlicing ()
        | LanguageFeature.FixedIndexSlice3d4d -> FSComp.SR.featureFixedIndexSlice3d4d ()
        | LanguageFeature.AndBang -> FSComp.SR.featureAndBang ()
        | LanguageFeature.ResumableStateMachines -> FSComp.SR.featureResumableStateMachines ()
        | LanguageFeature.NullableOptionalInterop -> FSComp.SR.featureNullableOptionalInterop ()
        | LanguageFeature.DefaultInterfaceMemberConsumption -> FSComp.SR.featureDefaultInterfaceMemberConsumption ()
        | LanguageFeature.WitnessPassing -> FSComp.SR.featureWitnessPassing ()
        | LanguageFeature.AdditionalTypeDirectedConversions -> FSComp.SR.featureAdditionalImplicitConversions ()
        | LanguageFeature.InterfacesWithMultipleGenericInstantiation -> FSComp.SR.featureInterfacesWithMultipleGenericInstantiation ()
        | LanguageFeature.StringInterpolation -> FSComp.SR.featureStringInterpolation ()
        | LanguageFeature.OverloadsForCustomOperations -> FSComp.SR.featureOverloadsForCustomOperations ()
        | LanguageFeature.ExpandedMeasurables -> FSComp.SR.featureExpandedMeasurables ()
        | LanguageFeature.StructActivePattern -> FSComp.SR.featureStructActivePattern ()
        | LanguageFeature.PrintfBinaryFormat -> FSComp.SR.featurePrintfBinaryFormat ()
        | LanguageFeature.IndexerNotationWithoutDot -> FSComp.SR.featureIndexerNotationWithoutDot ()
        | LanguageFeature.RefCellNotationInformationals -> FSComp.SR.featureRefCellNotationInformationals ()
        | LanguageFeature.UseBindingValueDiscard -> FSComp.SR.featureDiscardUseValue ()
        | LanguageFeature.NonVariablePatternsToRightOfAsPatterns -> FSComp.SR.featureNonVariablePatternsToRightOfAsPatterns ()
        | LanguageFeature.AttributesToRightOfModuleKeyword -> FSComp.SR.featureAttributesToRightOfModuleKeyword ()
        | LanguageFeature.MLCompatRevisions -> FSComp.SR.featureMLCompatRevisions ()
        | LanguageFeature.BetterExceptionPrinting -> FSComp.SR.featureBetterExceptionPrinting ()
        | LanguageFeature.DelegateTypeNameResolutionFix -> FSComp.SR.featureDelegateTypeNameResolutionFix ()
        | LanguageFeature.ReallyLongLists -> FSComp.SR.featureReallyLongList ()
        | LanguageFeature.ErrorOnDeprecatedRequireQualifiedAccess -> FSComp.SR.featureErrorOnDeprecatedRequireQualifiedAccess ()
        | LanguageFeature.RequiredPropertiesSupport -> FSComp.SR.featureRequiredProperties ()
        | LanguageFeature.InitPropertiesSupport -> FSComp.SR.featureInitProperties ()
        | LanguageFeature.LowercaseDUWhenRequireQualifiedAccess -> FSComp.SR.featureLowercaseDUWhenRequireQualifiedAccess ()
        | LanguageFeature.InterfacesWithAbstractStaticMembers -> FSComp.SR.featureInterfacesWithAbstractStaticMembers ()
        | LanguageFeature.SelfTypeConstraints -> FSComp.SR.featureSelfTypeConstraints ()
        | LanguageFeature.MatchNotAllowedForUnionCaseWithNoData -> FSComp.SR.featureMatchNotAllowedForUnionCaseWithNoData ()

    /// Get a version string associated with the given feature.
    static member GetFeatureVersionString feature =
        match features.TryGetValue feature with
        | true, v -> versionToString v
        | _ -> invalidArg "feature" "Internal error: Unable to find feature."

    override x.Equals(yobj: obj) =
        match yobj with
        | :? LanguageVersion as y -> x.SpecifiedVersion = y.SpecifiedVersion
        | _ -> false

    override x.GetHashCode() = hash x.SpecifiedVersion

    static member Default = defaultLanguageVersion
