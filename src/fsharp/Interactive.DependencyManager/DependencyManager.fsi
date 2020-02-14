// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Helper members to integrate DependencyManagers into F# codebase
namespace Interactive.DependencyManager

open System

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

    /// Go fetch a dependencymanager that supports a specific key
    member TryFindDependencyManagerByKey : compilerTools:seq<string> * outputDir:string * reportError:(ErrorReportType -> int * string -> unit) * key:string -> IDependencyManagerProvider

    /// TryFindDependencyManagerInPath - given a #r "key:sometext" go and find a DependencyManager that satisfies the key
    member TryFindDependencyManagerInPath : compilerTools:seq<string> * outputDir:string * reportError:(ErrorReportType -> int * string -> unit) * path:string -> string * IDependencyManagerProvider

