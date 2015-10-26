// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// The IL Binary writer 
module internal Microsoft.FSharp.Compiler.AbstractIL.ILBinaryWriter 

open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.IL 

[<Sealed>]
type ILStrongNameSigner =
    member PublicKey: byte[]
    static member OpenPublicKeyFile: string -> ILStrongNameSigner
    static member OpenPublicKey: byte[] -> ILStrongNameSigner
    static member OpenKeyPairFile: string -> ILStrongNameSigner
    static member OpenKeyContainer: string -> ILStrongNameSigner

type EmitStreamProvider = Lazy<Choice<EmitTo, Diagnostic>>
and EmitTo =
    | EmittedFile of string
    | EmittedStream of System.IO.Stream
and Diagnostic = string

type options =
    { ilg: ILGlobals
      pdbfile: EmitStreamProvider option
      mdbfile: EmitStreamProvider option
      signer : ILStrongNameSigner option
      fixupOverlappingSequencePoints : bool
      emitTailcalls: bool
      showTimes : bool
      dumpDebugInfo : EmitStreamProvider option }

/// Write a binary. Extra configuration parameters can also be specified. 
val WriteILBinary: outfile: EmitStreamProvider * options:  options * input: ILModuleDef * noDebugData: bool -> unit

