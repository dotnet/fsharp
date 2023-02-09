// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Functions associated with writing binaries which
/// vary between supported implementations of the CLI Common Language
/// Runtime, e.g. between the SSCLI, Mono and the Microsoft CLR.
///
/// The implementation of the functions can be found in ilsupp-*.fs
module internal FSharp.Compiler.AbstractIL.Support

val absilWriteGetTimeStamp: unit -> int32

type IStream = System.Runtime.InteropServices.ComTypes.IStream

/// Unmanaged resource file linker - for native resources (not managed ones).
/// The function may be called twice, once with a zero-RVA and
/// arbitrary buffer, and once with the real buffer.  The size of the
/// required buffer is returned.
val linkNativeResources: unlinkedResources: byte[] list -> rva: int32 -> byte[]

val unlinkResource: int32 -> byte[] -> byte[]
