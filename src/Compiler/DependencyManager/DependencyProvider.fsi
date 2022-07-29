// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Helper members to integrate DependencyManagers into F# codebase
namespace FSharp.Compiler.DependencyManager

open System
open System.Runtime.InteropServices
open Internal.Utilities.Library

/// The results of ResolveDependencies
type IResolveDependenciesResult =

    /// Succeded?
    abstract Success: bool

    /// The resolution output log
    abstract StdOut: string[]

    /// The resolution error log (process stderr)
    abstract StdError: string[]

    /// The resolution paths - the full paths to selected resolved dll's.
    /// In scripts this is equivalent to #r @"c:\somepath\to\packages\ResolvedPackage\1.1.1\lib\netstandard2.0\ResolvedAssembly.dll"
    abstract Resolutions: seq<string>

    /// The source code file paths
    abstract SourceFiles: seq<string>

    /// The roots to package directories
    ///     This points to the root of each located package.
    ///     The layout of the package manager will be package manager specific.
    ///     however, the dependency manager dll understands the nuget package layout
    ///     and so if the package contains folders similar to the nuget layout then
    ///     the dependency manager will be able to probe and resolve any native dependencies
    ///     required by the nuget package.
    ///
    /// This path is also equivalent to
    ///     #I @"c:\somepath\to\packages\1.1.1\ResolvedPackage"
    abstract Roots: seq<string>

/// Wraps access to a DependencyManager implementation
[<AllowNullLiteral>]
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

    /// Clear the results cache
    abstract ClearResultsCache: unit -> unit

    /// Resolve the dependencies, for the given set of arguments, go find the .dll references, scripts and additional include values.
    abstract ResolveDependencies:
        scriptDir: string *
        mainScriptName: string *
        scriptName: string *
        scriptExt: string *
        packageManagerTextLines: (string * string) seq *
        tfm: string *
        rid: string *
        timeout: int ->
            IResolveDependenciesResult

/// Todo describe this API
[<RequireQualifiedAccess>]
type ErrorReportType =
    | Warning
    | Error

type ResolvingErrorReport = delegate of ErrorReportType * int * string -> unit

/// Provides DependencyManagement functions.
///
/// The class incrementally collects IDependencyManagerProvider, indexed by key, and
/// queries them.  These are found and instantiated with respect to the compilerTools and outputDir
/// provided each time the TryFindDependencyManagerByKey and TryFindDependencyManagerInPath are
/// executed, which are assumed to be invariant over the lifetime of the DependencyProvider.
type DependencyProvider =
    interface IDisposable

    /// Construct a new DependencyProvider with no dynamic load handlers (only for compilation/analysis)
    new: unit -> DependencyProvider

    /// Construct a new DependencyProvider with only native resolution
    new: nativeProbingRoots: NativeResolutionProbe -> DependencyProvider

    /// Construct a new DependencyProvider with only native resolution and specify caching
    new: nativeProbingRoots: NativeResolutionProbe * useResultsCache: bool -> DependencyProvider

    /// Construct a new DependencyProvider with managed and native resolution
    new: assemblyProbingPaths: AssemblyResolutionProbe * nativeProbingRoots: NativeResolutionProbe -> DependencyProvider

    /// Construct a new DependencyProvider with managed and native resolution and specify caching
    new:
        assemblyProbingPaths: AssemblyResolutionProbe *
        nativeProbingRoots: NativeResolutionProbe *
        useResultsCache: bool ->
            DependencyProvider

    /// Returns a formatted help messages for registered dependencymanagers for the host to present
    member GetRegisteredDependencyManagerHelpText: string seq * string * ResolvingErrorReport -> string[]

    /// Clear the DependencyManager results caches
    member ClearResultsCache: string seq * string * ResolvingErrorReport -> unit

    /// Returns a formatted error message for the host to present
    member CreatePackageManagerUnknownError: string seq * string * string * ResolvingErrorReport -> int * string

    /// Resolve reference for a list of package manager lines
    member Resolve:
        packageManager: IDependencyManagerProvider *
        scriptExt: string *
        packageManagerTextLines: (string * string) seq *
        reportError: ResolvingErrorReport *
        executionTfm: string *
        [<Optional; DefaultParameterValue(null: string MaybeNull)>] executionRid: string *
        [<Optional; DefaultParameterValue("")>] implicitIncludeDir: string *
        [<Optional; DefaultParameterValue("")>] mainScriptName: string *
        [<Optional; DefaultParameterValue("")>] fileName: string *
        [<Optional; DefaultParameterValue(-1)>] timeout: int ->
            IResolveDependenciesResult

    /// Fetch a dependencymanager that supports a specific key
    member TryFindDependencyManagerByKey:
        compilerTools: string seq * outputDir: string * reportError: ResolvingErrorReport * key: string ->
            IDependencyManagerProvider MaybeNull

    /// TryFindDependencyManagerInPath - given a #r "key:sometext" go and find a DependencyManager that satisfies the key
    member TryFindDependencyManagerInPath:
        compilerTools: string seq * outputDir: string * reportError: ResolvingErrorReport * path: string ->
            string MaybeNull * IDependencyManagerProvider MaybeNull
