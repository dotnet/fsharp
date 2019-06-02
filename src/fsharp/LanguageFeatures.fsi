// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Coordinating compiler operations - configuration, loading initial context, reporting errors etc.
module internal FSharp.Compiler.Features

/// LanguageFeature enumeration
[<RequireQualifiedAccess>]
type LanguageFeature =
    | LanguageVersion46 = 0
    | LanguageVersion47 = 1
    | SingleUnderscorePattern = 2
    | Nullness = 1000
    | ScriptingPackageManagement = 1001

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
