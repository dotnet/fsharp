// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager.Nuget

open System.Collections.Generic

module internal FSharpDependencyManager =
    val formatPackageReference: PackageReference -> seq<string>
    val parsePackageReference: string list -> PackageReference list * string option option

type [<DependencyManagerAttribute>] FSharpDependencyManager =
    new: outputDir:string option -> FSharpDependencyManager
    member Name: string
    member Key:string
    member ResolveDependencies: scriptExt:string * packageManagerTextLines:string seq * tfm: string -> bool * string seq * string seq * string seq
