// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The IL Binary writer.
module internal FSharp.Compiler.AbstractIL.ILBinaryWriter 

open Internal.Utilities
open FSharp.Compiler.AbstractIL 
open FSharp.Compiler.AbstractIL.Internal 
open FSharp.Compiler.AbstractIL.IL 

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
   embeddedPDB: bool
   embedAllSource: bool
   embedSourceList: string list
   sourceLink: string
   signer : ILStrongNameSigner option
   emitTailcalls: bool
   deterministic: bool
   showTimes : bool
   dumpDebugInfo : bool
   pathMap : PathMap }

/// Write a binary to the file system. Extra configuration parameters can also be specified. 
val WriteILBinary: filename: string * options:  options * input: ILModuleDef * (ILAssemblyRef -> ILAssemblyRef) -> unit
