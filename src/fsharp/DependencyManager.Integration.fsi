// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Helper members to integrate DependencyManagers into F# codebase
module internal FSharp.Compiler.DependencyManagerIntegration

open FSharp.Compiler.Range

type IDependencyManagerProvider =
    abstract Name: string
    abstract Key: string
    abstract ResolveDependencies: scriptDir: string * mainScriptName: string * scriptName: string * packageManagerTextLines: string seq * tfm: string -> bool * string list * string list
    abstract DependencyAdding: IEvent<string * string>
    abstract DependencyAdded: IEvent<string * string>
    abstract DependencyFailed: IEvent<string * string>

[<RequireQualifiedAccess>]
type ReferenceType =
| RegisteredDependencyManager of IDependencyManagerProvider
| Library of string
| UnknownType

val tryFindDependencyManagerInPath: string list -> string option -> range -> string -> ReferenceType
val tryFindDependencyManagerByKey: string list -> string option -> range -> string -> IDependencyManagerProvider option
val removeDependencyManagerKey: string -> string -> string
val createPackageManagerUnknownError: string list -> string option -> string -> range -> exn
val resolve: IDependencyManagerProvider -> string -> string -> string -> range -> string seq -> (bool * string list * string list) option
