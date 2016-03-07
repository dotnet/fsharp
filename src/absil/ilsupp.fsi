// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Functions associated with writing binaries which 
/// vary between supported implementations of the CLI Common Language 
/// Runtime, e.g. between the SSCLI, Mono and the Microsoft CLR.
///
/// The implementation of the functions can be found in ilsupp-*.fs
module internal Microsoft.FSharp.Compiler.AbstractIL.Internal.Support

#if FX_NO_PDB_WRITER
#else
type PdbWriter
val pdbInitialize : string -> string -> PdbWriter
#endif
#if FX_NO_PDB_READER
#else
type PdbReader
val pdbReadClose: PdbReader -> unit
#endif

val absilWriteGetTimeStamp: unit -> int32

open System
open System.Runtime.InteropServices
#if FX_NO_SYMBOLSTORE
#else
open System.Diagnostics.SymbolStore
#endif
open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.Internal
open Microsoft.FSharp.Compiler.AbstractIL.IL 

#if FX_NO_LINKEDRESOURCES
#else
type IStream = System.Runtime.InteropServices.ComTypes.IStream
#endif

/// Unmanaged resource file linker - for native resources (not managed ones).
/// The function may be called twice, once with a zero-RVA and
/// arbitrary buffer, and once with the real buffer.  The size of the
/// required buffer is returned.
type PEFileType = X86 | X64

#if FX_NO_LINKEDRESOURCES
#else
val linkNativeResources: unlinkedResources:byte[] list ->  rva:int32 -> PEFileType -> tempFilePath:string -> byte[]
val unlinkResource: int32 -> byte[] -> byte[]
#endif

#if FX_NO_PDB_WRITER
#else
/// PDB reader and associated types
type PdbDocument
type PdbMethod
type PdbVariable
type PdbMethodScope

type PdbSequencePoint = 
    { pdbSeqPointOffset: int;
      pdbSeqPointDocument: PdbDocument;
      pdbSeqPointLine: int;
      pdbSeqPointColumn: int;
      pdbSeqPointEndLine: int;
      pdbSeqPointEndColumn: int; }

val pdbReadOpen: string (* module *) -> string (* path *) -> PdbReader
val pdbReadClose: PdbReader -> unit
val pdbReaderGetMethod: PdbReader -> int32 (* token *) -> PdbMethod
val pdbReaderGetMethodFromDocumentPosition: PdbReader -> PdbDocument -> int (* line *) -> int (* col *) -> PdbMethod
val pdbReaderGetDocuments: PdbReader -> PdbDocument array
val pdbReaderGetDocument: PdbReader -> string (* url *) -> byte[] (* guid *) -> byte[] (* guid *) -> byte[] (* guid *) -> PdbDocument

val pdbDocumentGetURL: PdbDocument -> string
val pdbDocumentGetType: PdbDocument -> byte[] (* guid *)
val pdbDocumentGetLanguage: PdbDocument -> byte[] (* guid *)
val pdbDocumentGetLanguageVendor: PdbDocument -> byte[] (* guid *)
val pdbDocumentFindClosestLine: PdbDocument -> int -> int

val pdbMethodGetToken: PdbMethod -> int32
val pdbMethodGetRootScope: PdbMethod ->  PdbMethodScope
val pdbMethodGetSequencePoints: PdbMethod -> PdbSequencePoint array

val pdbScopeGetChildren: PdbMethodScope -> PdbMethodScope array
val pdbScopeGetOffsets: PdbMethodScope -> int * int
val pdbScopeGetLocals: PdbMethodScope -> PdbVariable array

val pdbVariableGetName: PdbVariable -> string
val pdbVariableGetSignature: PdbVariable -> byte[]
val pdbVariableGetAddressAttributes: PdbVariable -> int32 (* kind *) * int32 (* addrField1 *)
#endif

#if FX_NO_PDB_WRITER
#else
//---------------------------------------------------------------------
// PDB writer.
//---------------------------------------------------------------------

type PdbDocumentWriter

type idd =
    { iddCharacteristics: int32;
      iddMajorVersion: int32; (* actually u16 in IMAGE_DEBUG_DIRECTORY *)
      iddMinorVersion: int32; (* actually u16 in IMAGE_DEBUG_DIRECTORY *)
      iddType: int32;
      iddData: byte[];}

val pdbInitialize: 
    string (* .exe/.dll already written and closed *) -> 
    string  (* .pdb to write *) ->
    PdbWriter
val pdbClose: PdbWriter -> string -> string -> unit
val pdbCloseDocument : PdbDocumentWriter -> unit
val pdbSetUserEntryPoint: PdbWriter -> int32 -> unit
val pdbDefineDocument: PdbWriter -> string -> PdbDocumentWriter
val pdbOpenMethod: PdbWriter -> int32 -> unit
val pdbCloseMethod: PdbWriter -> unit
val pdbOpenScope: PdbWriter -> int -> unit
val pdbCloseScope: PdbWriter -> int -> unit
val pdbDefineLocalVariable: PdbWriter -> string -> byte[] -> int32 -> unit
val pdbSetMethodRange: PdbWriter -> PdbDocumentWriter -> int -> int -> PdbDocumentWriter -> int -> int -> unit
val pdbDefineSequencePoints: PdbWriter -> PdbDocumentWriter -> (int * int * int * int * int) array -> unit
val pdbWriteDebugInfo: PdbWriter -> idd
#endif

//---------------------------------------------------------------------
// Strong name signing
//---------------------------------------------------------------------

type keyContainerName = string
type keyPair = byte[]
type pubkey = byte[]
type pubkeyOptions = byte[] * bool

val signerOpenPublicKeyFile: string -> pubkey 
val signerOpenKeyPairFile: string -> keyPair 
val signerSignatureSize: pubkey -> int 
val signerGetPublicKeyForKeyPair: keyPair -> pubkey 
val signerGetPublicKeyForKeyContainer: string -> pubkey 
val signerCloseKeyContainer: keyContainerName -> unit 
val signerSignFileWithKeyPair: string -> keyPair -> unit 
val signerSignFileWithKeyContainer: string -> keyContainerName -> unit
