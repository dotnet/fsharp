// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Helper members to integrate DependencyManagers into F# codebase
namespace Interactive.DependencyManager

open System


/// The results of ResolveDependencies
type ResolveDependenciesResult =

    // Make new ResolveDependenciesResult
    new: success: bool * stdOut: string array * stdError: string array * resolutions: string seq * sourceFiles: string seq * roots: string seq -> ResolveDependenciesResult

    /// Succeded?
    member public Success: bool

    /// The resolution output log
    member public StdOut: string array

    /// The resolution error log (* process stderror *)
    member public StdError: string array
    
    /// The resolution paths
    member public Resolutions: string seq

    /// The source code file paths
    member public SourceFiles: string seq

    /// The roots to package directories
    member public Roots: string seq


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

    /// Resolve the dependencies, for the given set of arguments, go find the .dll references, scripts and additional include values.
    abstract ResolveDependencies: scriptDir: string * mainScriptName: string * scriptName: string * scriptExt: string * packageManagerTextLines: string seq * tfm: string ->  bool * string array * string array  *string seq * string seq * string seq
    

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
    new: nativeProbingRoots: NativeResolutionProbe -> DependencyProvider

    /// Returns a formatted error message for the host to present
    member CreatePackageManagerUnknownError: string seq * string * string * ResolvingErrorReport -> int * string

    /// Remove the dependency mager with the specified key
    member RemoveDependencyManagerKey: packageManagerKey: string * path: string -> string

    /// Resolve reference for a list of package manager lines
    member Resolve : packageManager: IDependencyManagerProvider * implicitIncludeDir: string * mainScriptName: string * fileName: string * scriptExt: string * packageManagerTextLines: string seq * reportError: ResolvingErrorReport * executionTfm: string -> ResolveDependenciesResult

    /// Fetch a dependencymanager that supports a specific key
    member TryFindDependencyManagerByKey: compilerTools: string seq * outputDir: string * reportError: ResolvingErrorReport * key: string -> IDependencyManagerProvider

    /// TryFindDependencyManagerInPath - given a #r "key:sometext" go and find a DependencyManager that satisfies the key
    member TryFindDependencyManagerInPath: compilerTools: string seq * outputDir: string * reportError: ResolvingErrorReport * path: string -> string * IDependencyManagerProvider
