// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Helper members to integrate DependencyManagers into F# codebase
module internal Microsoft.FSharp.Compiler.DependencyManagerIntegration

open Microsoft.FSharp.Compiler.Range

type IDependencyManagerProvider =
    inherit System.IDisposable
    abstract Name : string
    abstract ToolName: string
    abstract Key: string
    abstract ResolveDependencies : string * string * string * string seq -> string * string list

val RegisteredDependencyManagers : unit -> Map<string,IDependencyManagerProvider>
val tryFindDependencyManagerInPath : range -> string -> IDependencyManagerProvider option
val tryFindDependencyManagerByKey : range -> string -> IDependencyManagerProvider option

val removeDependencyManagerKey : string -> string -> string

val resolve : IDependencyManagerProvider -> string -> string -> range -> string seq -> (string list * string * string) option
