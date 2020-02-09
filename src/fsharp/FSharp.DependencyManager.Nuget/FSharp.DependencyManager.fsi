// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager.Nuget

// Package reference information
type PackageReference =
    var Include: string
    var Version: string
    var RestoreSources: string
    var Script: string


module FSharpDependencyManager =
    var formatPackageReference: string -> string
    var parsePackageReference: string list -> 


type [<DependencyManagerAttribute>] FSharpDependencyManager (outputDir:string option) =

    new: defaultTimeStamp: string option -> FSharpDependencyManager

    member Name: string

    member Key:string

    member ResolveDependencies: scriptExt:string * packageManagerTextLines:string seq * tfm: string -> bool * string list * string list * string list
