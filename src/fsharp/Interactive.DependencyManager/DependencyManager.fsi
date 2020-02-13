// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Helper members to integrate DependencyManagers into F# codebase
namespace Interactive.DependencyManager

type IDependencyManagerProvider =
    abstract Name: string
    abstract Key: string
    abstract ResolveDependencies: scriptDir: string * mainScriptName: string * scriptName: string * scriptExt: string * packageManagerTextLines: string seq * tfm: string -> bool * string seq * string seq

[<RequireQualifiedAccess>]
type ReferenceType =
| RegisteredDependencyManager of IDependencyManagerProvider
| Library of string
| UnknownType

[<RequireQualifiedAccess>]
type ErrorReportType =
| Warning
| Error

type DependencyProvider =

    new : unit -> DependencyProvider

    member CreatePackageManagerUnknownError: compilerTools: string seq * outputDir: string option * packageManagerKey: string reportError: ErrorReportType -> int * string -> unit -> string

    // Lose the Reference type Discrimiated Union, C# won't like it
    member TryFindDependencyManagerInPath: compilerTools: string seq * outputDir: string option * reportError: ErrorReportType -> int * string -> unit * path: string -> ReferenceType

    member RemoveDependencyManagerKey: packageManagerKey: string * path: string -> string

    member TryFindDependencyManagerByKey: compilerTools: string seq * outputDir: string option * reportError: ErrorReportType -> int * string -> unit * key:string -> IDependencyManagerProvider option

    member Resolve: packageManager:IDependencyManagerProvider * implicitIncludeDir:string  * mainScriptName:string * fileName:string * scriptExt:string * packageManagerTextLines: string seq * reportError: ErrorReportType -> int * string -> unit * executionTfm: string
