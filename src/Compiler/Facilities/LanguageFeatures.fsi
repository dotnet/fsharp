// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Coordinating compiler operations - configuration, loading initial context, reporting errors etc.
module internal FSharp.Compiler.Features

/// LanguageFeature enumeration
[<RequireQualifiedAccess>]
type LanguageFeature =
    | DotlessFloat32Literal
    | PackageManagement
    | FromEndSlicing
    | ResumableStateMachines
    | DefaultInterfaceMemberConsumption
    | WitnessPassing
    | AdditionalTypeDirectedConversions
    | StringInterpolation
    | OverloadsForCustomOperations
    | ExpandedMeasurables
    | NullnessChecking
    | RefCellNotationInformationals
    | UnionIsPropertiesVisible
    | NonVariablePatternsToRightOfAsPatterns
    | AttributesToRightOfModuleKeyword
    | ReallyLongLists
    | ErrorOnDeprecatedRequireQualifiedAccess
    | RequiredPropertiesSupport
    | InterfacesWithAbstractStaticMembers
    | SelfTypeConstraints
    | MatchNotAllowedForUnionCaseWithNoData
    | CSharpExtensionAttributeNotRequired
    | ErrorForNonVirtualMembersOverrides
    | ArithmeticInLiterals
    | ErrorReportingOnStaticClasses
    | WarningWhenCopyAndUpdateRecordChangesAllFields
    | NonInlineLiteralsAsPrintfFormat
    | ExtendedStringInterpolation
    | WarningWhenMultipleRecdTypeChoice
    | ConstraintIntersectionOnFlexibleTypes
    | StaticLetInRecordsDusEmptyTypes
    | WarningWhenTailRecAttributeButNonTailRecUsage
    | UnmanagedConstraintCsharpInterop
    | ReuseSameFieldsInStructUnions
    | ExtendedFixedBindings
    /// RFC-1137
    | PreferExtensionMethodOverPlainProperty
    | WarningIndexedPropertiesGetSetSameType
    | WarningWhenTailCallAttrOnNonRec
    | BooleanReturningAndReturnTypeDirectedPartialActivePattern
    | EnforceAttributeTargets
    | LowerInterpolatedStringToConcat
    | LowerIntegralRangesToFastLoops
    | AllowAccessModifiersToAutoPropertiesGettersAndSetters
    | LowerSimpleMappingsInComprehensionsToFastLoops
    | ParsedHashDirectiveArgumentNonQuotes
    | EmptyBodiedComputationExpressions
    | AllowObjectExpressionWithoutOverrides
    | DontWarnOnUppercaseIdentifiersInBindingPatterns
    | UseTypeSubsumptionCache
    | DeprecatePlacesWhereSeqCanBeOmitted
    | SupportValueOptionsAsOptionalParameters
    | WarnWhenUnitPassedToObjArg
    | UseBangBindingValueDiscard
    | BetterAnonymousRecordParsing
    | ScopedNowarn
    | ErrorOnInvalidDeclsInTypeDefinitions
    | AllowTypedLetUseAndBang
    | ReturnFromFinal
    | MoreConcreteTiebreaker
    | OverloadResolutionPriority
    | WarnWhenFunctionValueUsedAsInterpolatedStringArg
    | MethodOverloadsCache
    | ImplicitDIMCoverage
    | PreprocessorElif
    | ExtensionConstraintSolutions
    | ExceptionFieldSerializationSupport
    | ErrorOnMissingSignatureAttribute
    | RecordConstructorSyntax
    | NotNullIfNotNull
    | DirectDelegateConstruction
    | AccessProtectedBaseFieldFromClosure
    | ImprovedImpliedArgumentNamesPartTwo
    | RecordSpreads
    | ErrorOnBitwiseOpsOnNonIntegralEnums

/// LanguageVersion management
type LanguageVersion =

    /// Create a LanguageVersion management object
    new: string * ?disabledFeaturesArray: LanguageFeature array -> LanguageVersion

    /// Is the selected LanguageVersion valid
    static member ContainsVersion: string -> bool

    /// Is the selected LanguageVersion currently supported
    static member IsVersionSupported: string -> bool

    /// Has preview been explicitly specified
    member IsPreviewEnabled: bool

    /// Does the selected LanguageVersion support the specified feature
    member SupportsFeature: LanguageFeature -> bool

    /// Get the disabled features
    member DisabledFeatures: LanguageFeature array

    /// Create a new LanguageVersion with updated disabled features
    member WithDisabledFeatures: LanguageFeature array -> LanguageVersion

    /// Get the list of valid versions
    static member ValidVersions: string[]

    /// Get the list of valid options
    static member ValidOptions: string[]

    /// Get the specified LanguageVersion
    member SpecifiedVersion: decimal

    /// Get the text used to specify the version, several of which may map to the same version
    member VersionText: string

    /// Get the specified LanguageVersion as a string
    member SpecifiedVersionString: string

    /// Get a string name for the given feature.
    static member GetFeatureString: feature: LanguageFeature -> string

    /// Get a version string associated with the given feature.
    static member GetFeatureVersionString: feature: LanguageFeature -> string

    /// Try to parse a feature name string to a LanguageFeature option
    static member TryParseFeature: featureName: string -> LanguageFeature option

    static member Default: LanguageVersion
