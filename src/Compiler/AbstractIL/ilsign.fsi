// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Functions associated with signing il assemblies which
/// vary between supported implementations of the CLI Common Language
/// Runtime, e.g. between the SSCLI, Mono and the Microsoft CLR.
///

module internal FSharp.Compiler.AbstractIL.StrongNameSign

open System.IO

//---------------------------------------------------------------------
// Strong name signing
//---------------------------------------------------------------------
[<Sealed>]
type ILStrongNameSigner =
    static member OpenPublicKeyOptions: pubkey: byte array -> usePublicSign: bool -> ILStrongNameSigner
    static member OpenPublicKey: pubkey: byte array -> ILStrongNameSigner
    static member OpenKeyPairFile: keyPair: byte array -> usePublicSign: bool -> ILStrongNameSigner
    static member OpenKeyContainer: keyContainerName: string -> usePublicSign: bool -> ILStrongNameSigner
    member IsFullySigned: bool
    member PublicKey: byte array
    member SignatureSize: int
    member SignStream: stream: System.IO.Stream -> unit
