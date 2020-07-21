// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Helper members to integrate DependencyManagers into F# codebase
namespace Microsoft.DotNet.DependencyManager

open System
open System.Runtime.InteropServices


/// The results of ResolveDependencies
type IResolveDependenciesResult =

    /// Succeded?
    abstract Success: bool

    /// The resolution output log
    abstract StdOut: string array

    /// The resolution error log (process stderr)
    abstract StdError: string array

    /// The resolution paths
    abstract Resolutions: seq<string>

    /// The source code file paths
    abstract SourceFiles: seq<string>

    /// The roots to package directories
    abstract Roots: seq<string>

/// Wraps access to a DependencyManager implementation
[<AllowNullLiteralAttribute >]
type IDependencyManagerProvider =

    /// Name of the dependency manager
    abstract Name: string

    /// Key that identifies the types of dependencies that this DependencyManager operates on
    /// E.g
    ///     nuget: indicates that this DM is for nuget packages
    ///     paket: indicates that this DM is for paket scripts, which manage nuget packages, github source dependencies etc ...
    abstract Key: string

    /// The help messages for this dependency manager inster
    abstract HelpMessages: string[]

    /// Resolve the dependencies, for the given set of arguments, go find the .dll references, scripts and additional include values.
    abstract ResolveDependencies: scriptDir: string * mainScriptName: string * scriptName: string * scriptExt: string * packageManagerTextLines: string seq * tfm: string * rid: string -> IResolveDependenciesResult

/// Todo describe this API
[<RequireQualifiedAccess>]
type ErrorReportType =
    | Warning
    | Error

type ResolvingErrorReport = delegate of ErrorReportType * int * string -> unit

/// Provides DependencyManagement functions.
/// Class is IDisposable
type DependencyProvider =
    interface System.IDisposable

    /// Construct a new DependencyProvider
    new: assemblyProbingPaths: AssemblyResolutionProbe * nativeProbingRoots: NativeResolutionProbe -> DependencyProvider

    /// Construct a new DependencyProvider
    new: nativeProbingRoots: NativeResolutionProbe -> DependencyProvider

    /// Returns a formatted help messages for registered dependencymanagers for the host to present
    member GetRegisteredDependencyManagerHelpText: string seq * string * ResolvingErrorReport -> string[]

    /// Returns a formatted error message for the host to present
    member CreatePackageManagerUnknownError: string seq * string * string * ResolvingErrorReport -> int * string

    /// Remove the dependency manager with the specified key
    member RemoveDependencyManagerKey: packageManagerKey: string * path: string -> string

    /// Resolve reference for a list of package manager lines
    member Resolve : packageManager: IDependencyManagerProvider * scriptExt: string * packageManagerTextLines: string seq * reportError: ResolvingErrorReport * executionTfm: string * [<Optional;DefaultParameterValue(null:string)>]executionRid: string  * [<Optional;DefaultParameterValue("")>]implicitIncludeDir: string * [<Optional;DefaultParameterValue("")>]mainScriptName: string * [<Optional;DefaultParameterValue("")>]fileName: string -> IResolveDependenciesResult

    /// Fetch a dependencymanager that supports a specific key
    member TryFindDependencyManagerByKey: compilerTools: string seq * outputDir: string * reportError: ResolvingErrorReport * key: string -> IDependencyManagerProvider

    /// TryFindDependencyManagerInPath - given a #r "key:sometext" go and find a DependencyManager that satisfies the key
    member TryFindDependencyManagerInPath: compilerTools: string seq * outputDir: string * reportError: ResolvingErrorReport * path: string -> string * IDependencyManagerProvider
