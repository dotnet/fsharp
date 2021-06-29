// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Functions to map real paths to paths to be written to PDB/IL
namespace Internal.Utilities

type PathMap

[<RequireQualifiedAccess>]
module internal PathMap =

    val empty : PathMap

    /// Add a path mapping to the map.
    val addMapping : string -> string -> PathMap -> PathMap

    /// Map a file path with its replacement.
    /// Prefixes are compared case sensitively.
    val apply : PathMap -> string -> string

    /// Map a directory name with its replacement.
    /// Prefixes are compared case sensitively.
    val applyDir : PathMap -> string -> string
