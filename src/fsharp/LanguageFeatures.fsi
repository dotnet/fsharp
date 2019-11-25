// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Coordinating compiler operations - configuration, loading initial context, reporting errors etc.
module internal FSharp.Compiler.Features

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

    /// Get the specified LanguageVersion
    member SpecifiedVersion: decimal
