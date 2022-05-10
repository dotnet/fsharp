// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Functions associated with signing il assemblies which
/// vary between supported implementations of the CLI Common Language
/// Runtime, e.g. between the SSCLI, Mono and the Microsoft CLR.
///

module internal FSharp.Compiler.AbstractIL.StrongNameSign

//---------------------------------------------------------------------
// Strong name signing
//---------------------------------------------------------------------
[<Sealed>]
type ILStrongNameSigner =
    member PublicKey: byte []
    static member OpenPublicKeyOptions: string -> bool -> ILStrongNameSigner
    static member OpenPublicKey: byte [] -> ILStrongNameSigner
    static member OpenKeyPairFile: string -> ILStrongNameSigner
    static member OpenKeyContainer: string -> ILStrongNameSigner
    member Close: unit -> unit
    member IsFullySigned: bool
    member PublicKey: byte []
    member SignatureSize: int
    member SignFile: string -> unit
