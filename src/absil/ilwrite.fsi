// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// The IL Binary writer.
module internal Microsoft.FSharp.Compiler.AbstractIL.ILBinaryWriter 

open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.IL 

[<Sealed>]
type ILStrongNameSigner =
    member PublicKey: byte[]
    static member OpenPublicKeyOptions: string -> bool -> ILStrongNameSigner
    static member OpenPublicKey: byte[] -> ILStrongNameSigner
    static member OpenKeyPairFile: string -> ILStrongNameSigner
    static member OpenKeyContainer: string -> ILStrongNameSigner

type options =
 { ilg: ILGlobals
   pdbfile: string option
   portablePDB: bool
   signer : ILStrongNameSigner option
   fixupOverlappingSequencePoints : bool
   emitTailcalls: bool
   showTimes : bool
   dumpDebugInfo : bool }

/// Write a binary to the file system. Extra configuration parameters can also be specified. 
val WriteILBinary: filename: string * options:  options * input: ILModuleDef * noDebugData: bool -> unit
