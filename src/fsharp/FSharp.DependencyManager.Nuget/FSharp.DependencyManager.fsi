// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager.Nuget

module internal FSharpDependencyManager =
    val formatPackageReference: PackageReference -> seq<string>
    val parsePackageReference: string list -> PackageReference list * string option option


/// The results of ResolveDependencies
[<Class>]
type ResolveDependenciesResult =

    /// Succeded?
    member Success: bool

    /// The resolution output log
    member StdOut: string array

    /// The resolution error log (* process stderror *)
    member StdError: string array

    /// The resolution paths
    member Resolutions: string seq

    /// The source code file paths
    member SourceFiles: string seq

    /// The roots to package directories
    member Roots: string seq


type [<DependencyManagerAttribute>] FSharpDependencyManager =
    new: outputDir:string option -> FSharpDependencyManager
    member Name: string
    member Key:string
    member ResolveDependencies: scriptExt:string * packageManagerTextLines:string seq * tfm: string -> obj
