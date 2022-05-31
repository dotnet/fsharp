// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.BinaryResourceFormats

open FSharp.Compiler.AbstractIL.IL

module VersionResourceFormat =

    val VS_VERSION_INFO_RESOURCE:
        (ILVersionInfo * ILVersionInfo * int32 * int32 * int32 * int32 * int32 * int64) *
        seq<string * #seq<string * string>> *
        seq<int32 * int32> ->
            byte[]

module ManifestResourceFormat =

    val VS_MANIFEST_RESOURCE: data: byte[] * isLibrary: bool -> byte[]

module ResFileFormat =

    val ResFileHeader: unit -> byte[]
