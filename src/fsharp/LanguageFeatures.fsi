// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Coordinating compiler operations - configuration, loading initial context, reporting errors etc.
module internal FSharp.Compiler.Features

/// LanguageFeature enumeration
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

/// LanguageVersion management
type LanguageVersion =

    /// Create a LanguageVersion management object
    new: string -> LanguageVersion

    /// Get the list of valid versions
    member ContainsVersion: string -> bool

    /// Has preview been explicitly specified
    member IsPreviewEnabled: bool

    /// Has been explicitly specified as 4.6, 4.7 or 5.0
    member IsExplicitlySpecifiedAs50OrBefore: unit -> bool

    /// Does the selected LanguageVersion support the specified feature
    member SupportsFeature: LanguageFeature -> bool

    /// Get the list of valid versions
    member ValidVersions: string array

    /// Get the list of valid options
    member ValidOptions: string array

    /// Get the specified LanguageVersion
    member SpecifiedVersion: decimal

    /// Get the text used to specify the version, several of which may map to the same version
    member VersionText: string

    /// Get the specified LanguageVersion as a string
    member SpecifiedVersionString: string

    /// Get a string name for the given feature.
    member GetFeatureString: feature: LanguageFeature -> string

    /// Get a version string associated with the given feature.
    member GetFeatureVersionString: feature: LanguageFeature -> string

    static member Default: LanguageVersion
