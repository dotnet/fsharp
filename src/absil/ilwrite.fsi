// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// The IL Binary writer 
module internal Microsoft.FSharp.Compiler.AbstractIL.ILBinaryWriter 

open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.IL 

#if FX_NO_KEY_SIGNING
type ILStrongNameSigner = unit
#else
[<Sealed>]
type ILStrongNameSigner =
    member PublicKey: byte[]
    static member OpenPublicKeyFile: string -> ILStrongNameSigner
    static member OpenPublicKey: byte[] -> ILStrongNameSigner
    static member OpenKeyPairFile: string -> ILStrongNameSigner
    static member OpenKeyContainer: string -> ILStrongNameSigner
#endif

type options =
 { ilg: ILGlobals
   pdbfile: string option;
#if FX_NO_KEY_SIGNING
#else
   signer : ILStrongNameSigner option;
#endif
   fixupOverlappingSequencePoints : bool;
   emitTailcalls: bool;
   showTimes : bool;
   dumpDebugInfo : bool }

/// Write a binary to the file system. Extra configuration parameters can also be specified. 
val WriteILBinary: 
    filename: string ->
    options:  options ->
    input:    ILModuleDef -> 
    noDebugData: bool ->
    unit



