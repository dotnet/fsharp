// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager.Nuget

module internal FSharpDependencyManager =
    val formatPackageReference: PackageReference -> seq<string>
    val parsePackageReference: scriptExt: string -> string list -> PackageReference list * string option option

/// The results of ResolveDependencies
[<Class>]
type ResolveDependenciesResult =

    /// Succeded?
    member Success: bool

    /// The resolution output log
    member StdOut: string[]

    /// The resolution error log (process stderr)
    member StdError: string[]

    /// The resolution paths
    member Resolutions: seq<string>

    /// The source code file paths
    member SourceFiles: seq<string>

    /// The roots to package directories
    member Roots: seq<string>

[<DependencyManagerAttribute>] 
type FSharpDependencyManager =
    new: outputDir:string option -> FSharpDependencyManager

    member Name: string

    member Key:string

    member HelpMessages:string[]

    member ResolveDependencies: scriptExt:string * packageManagerTextLines:string seq * tfm: string * rid: string -> obj
