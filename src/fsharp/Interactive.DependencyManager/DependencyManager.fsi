
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Helper members to integrate DependencyManagers into F# codebase
namespace Interactive.DependencyManager

open System

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
    abstract ResolveDependencies: scriptDir: string * mainScriptName: string * scriptName: string * scriptExt: string * packageManagerTextLines: string seq * tfm: string -> bool * string seq * string seq * string seq

/// Indicates to the Error reporting callbacks, severity of the error
[<RequireQualifiedAccess>]
type ErrorReportType =
    | Warning
    | Error


/// Provides DependencyManagement functions.
/// Class is IDisposable
type DependencyProvider =
    interface System.IDisposable

    /// Construct a new DependencyProvider
    new : unit -> DependencyProvider

    /// Returns a formatted error message for the host to present
    member CreatePackageManagerUnknownError : compilerTools:seq<string> * outputDir:string * packageManagerKey:string * reportError:(ErrorReportType ->int * string -> unit) -> int * string

    /// Remove the dependency mager with the specified key
    member RemoveDependencyManagerKey : packageManagerKey:string * path:string -> string

    /// Resolve reference for a list of package manager lines
    member Resolve : packageManager:IDependencyManagerProvider * implicitIncludeDir:string * mainScriptName:string * fileName:string * scriptExt:string * packageManagerTextLines:seq<string> * reportError:(ErrorReportType -> int * string -> unit) * executionTfm:string -> bool * seq<string> * seq<string> * seq<string>

    /// Fetch a dependencymanager that supports a specific key
    member TryFindDependencyManagerByKey : compilerTools:seq<string> * outputDir:string * reportError:(ErrorReportType -> int * string -> unit) * key:string -> IDependencyManagerProvider

    /// TryFindDependencyManagerInPath - given a #r "key:sometext" go and find a DependencyManager that satisfies the key
    member TryFindDependencyManagerInPath : compilerTools:seq<string> * outputDir:string * reportError:(ErrorReportType -> int * string -> unit) * path:string -> string * IDependencyManagerProvider

