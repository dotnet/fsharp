// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Helper members to integrate DependencyManagers into F# codebase
namespace Interactive.DependencyManager

/// Todo describe this API
[<AllowNullLiteralAttribute >]
type IDependencyManagerProvider =
    /// Todo describe this API
    abstract Name: string

    /// Todo describe this API
    abstract Key: string

    /// Todo describe this API
    abstract ResolveDependencies: scriptDir: string * mainScriptName: string * scriptName: string * scriptExt: string * packageManagerTextLines: string seq * tfm: string -> bool * string seq * string seq * string seq

/// Todo describe this API
[<RequireQualifiedAccess>]
type ErrorReportType =
| Warning
| Error


/// Todo describe this API
type DependencyProvider =
    interface IDisposable

    /// Construct a new DependencyProvider
    new : unit -> DependencyProvider

    /// Returns a formatted error message for the host to present
    member CreatePackageManagerUnknownError: compilerTools: string seq * outputDir: string * packageManagerKey: string * reportError: ErrorReportType -> int * string -> unit -> string

    /// TryFindDependencyManagerInPath - given a #r "key:sometext" go and find a DependencyManager that satisfies the key
    member TryFindDependencyManagerInPath: compilerTools: string seq * outputDir: string * reportError: ErrorReportType -> int * string -> unit * path: string -> string * IDependencyManagerProvider

    /// Remove the dependency mager with the specified key
    member RemoveDependencyManagerKey: packageManagerKey: string * path: string -> string

    /// Go fetch a dependencymanager that supports a specific key
    member TryFindDependencyManagerByKey: compilerTools: string seq * outputDir: string * reportError: ErrorReportType -> int * string -> unit * key:string -> IDependencyManagerProvider

    /// Resolve reference for a list of package manager lines
    member Resolve: packageManager:IDependencyManagerProvider * implicitIncludeDir:string * mainScriptName:string * fileName:string * scriptExt:string * packageManagerTextLines: string seq * reportError: ErrorReportType -> int * string -> unit * executionTfm: string -> bool * string seq * string seq * string seq
