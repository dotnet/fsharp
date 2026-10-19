// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Abstractions for metadata heap indexing.
/// Used by full assembly emission (ilwrite.fs) and intended to also back the delta
/// emitter tracked in F# hot-reload work (dotnet/fsharp#19941), providing a unified
/// interface for string, blob, GUID, and user-string heap access.
module internal FSharp.Compiler.AbstractIL.ILMetadataHeaps

/// Abstraction for metadata heap indexing operations.
/// This interface allows both full assembly and delta emission to share
/// the same heap access patterns while using different underlying storage.
type IMetadataHeaps =
    /// Get or add a string to the #Strings heap, returning the heap index.
    /// Empty/null strings return 0.
    abstract GetStringHeapIdx: string -> int

    /// Get or add a byte array to the #Blob heap, returning the heap index.
    /// Empty arrays return 0.
    abstract GetBlobHeapIdx: byte[] -> int

    /// Get or add a GUID to the #GUID heap, returning the 1-based index.
    abstract GetGuidIdx: byte[] -> int

    /// Get or add a string to the #US (User Strings) heap, returning the heap index.
    abstract GetUserStringHeapIdx: string -> int

/// Extension functions for IMetadataHeaps
[<AutoOpen>]
module MetadataHeapsExtensions =
    type IMetadataHeaps with
        /// Get string heap index for an optional string, returning 0 for None.
        member this.GetStringHeapIdxOption(sopt: string option) =
            match sopt with
            | Some s -> this.GetStringHeapIdx s
            | None -> 0

/// <summary>
/// Records the uncompressed heap sizes produced during metadata emission so that later delta passes
/// can reason about stream growth.
/// </summary>
/// <remarks>
/// This type is delta-owned: the full-assembly IL writer (ilwrite.fs) does not currently expose an
/// equivalent snapshot type on main. Keeping the definition here (rather than growing ilwrite.fsi's
/// public surface) lets the delta writer stay self-contained; a future PR that wires a baseline
/// producer into this writer can either reuse this type directly or convert into it at the boundary.
/// </remarks>
[<NoEquality; NoComparison>]
type MetadataHeapSizes =
    {
        StringHeapSize: int
        UserStringHeapSize: int
        BlobHeapSize: int
        GuidHeapSize: int
    }
