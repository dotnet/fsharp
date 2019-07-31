// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Coordinating compiler operations - configuration, loading initial context, reporting errors etc.
module internal FSharp.Compiler.Features

/// LanguageFeature enumeration
[<RequireQualifiedAccess>]
type LanguageFeature =
    | PreviewVersion = 0
    | LanguageVersion46 = 1
    | LanguageVersion47 = 2
    | SingleUnderscorePattern = 3
    | WildCardInForLoop = 4
    | RelaxWhitespace = 5
    | NameOf = 6
    | DefaultInterfaceMethodConsumption = 7
    | ImplicitYield = 8
    | OpenStaticClasses = 9


/// LanguageVersion management
type LanguageVersion =

    /// Create a LanguageVersion management object
    new: string -> LanguageVersion

    /// Get the list of valid versions
    member ContainsVersion: string -> bool

    /// Does the specified LanguageVersion support the specified feature
    member SupportsFeature: LanguageFeature -> bool

    /// Get the list of valid versions
    member ValidVersions: string array

    /// Get the list of valid options
    member ValidOptions: string array

    /// Get a string name for the given feature. Returns an empty string if feature is invalid.
    member GetFeatureString: LanguageFeature -> string

    /// Get a version string associated with the given feature. Returns an empty string if feature is invalid.
    member GetFeatureVersionString: LanguageFeature -> string

    /// Current version string.
    member CurrentVersionString: string

    /// Get the specified LanguageVersion   @@@@@@@@
    member SpecifiedVerson: decimal
