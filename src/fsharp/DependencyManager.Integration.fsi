// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Helper members to integrate DependencyManagers into F# codebase
module internal Microsoft.FSharp.Compiler.DependencyManagerIntegration

open Microsoft.FSharp.Compiler.Range

/// Contract for dependency anager provider.  This is a loose contract for now, just to define the shape, 
/// it is resolved through reflection (ReflectionDependencyManagerProvider)
type IDependencyManagerProvider =
    inherit System.IDisposable
    abstract Name : string
    abstract ToolName: string
    abstract Key: string
    abstract ResolveDependencies : string * string * string * string seq -> string option * string list

/// Reference
[<RequireQualifiedAccess>]
type ReferenceType =
| RegisteredDependencyManager of IDependencyManagerProvider
| Library of string
| UnknownType

val registeredDependencyManagers : string list -> Map<string,IDependencyManagerProvider>
val tryFindDependencyManagerInPath : range -> string -> string list -> ReferenceType
val tryFindDependencyManagerByKey : range -> string -> string list -> IDependencyManagerProvider option
val removeDependencyManagerKey : string -> string -> string
val createPackageManagerUnknownError : string -> range -> string list -> exn
val resolve : IDependencyManagerProvider -> string -> string -> range -> string seq -> (string option * string list) option
