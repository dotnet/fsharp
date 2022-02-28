// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The ILPdbWriter 
module internal FSharp.Compiler.AbstractIL.ILPdbWriter 

open Internal.Utilities
open FSharp.Compiler.AbstractIL.IL
open System.IO
open System.Reflection.Metadata

type PdbDocumentData = ILSourceDocument

type PdbLocalVar = 
    { 
      Name: string
      
      Signature: byte[] 

      /// the local index the name corresponds to
      Index: int32  
    }

/// Defines the set of 'imports' - that is, opened namespaces, types etc. - at each code location
///
/// Note the C# debug evaluation engine used for F# will give C# semantics to these.  That in general
/// is very close to F# semantics, except for things like union type.
type PdbImport =

    /// Represents an 'open type XYZ' opening a type
    | ImportType of targetTypeToken: int32 (* alias: string option *)

    /// Represents an 'open XYZ' opening a namespace
    | ImportNamespace of targetNamespace: string (* assembly: ILAssemblyRef option * alias: string option *) 
    //| ReferenceAlias of string
    //| OpenXmlNamespace of prefix: string * xmlNamespace: string

type PdbImports =
    {
      Parent: PdbImports option
      Imports: PdbImport[]
    }

type PdbMethodScope = 
    {
      Children: PdbMethodScope[]
      StartOffset: int
      EndOffset: int
      Locals: PdbLocalVar[]
      Imports: PdbImports option
    }

type PdbSourceLoc = 
    {
      Document: int
      Line: int
      Column: int
    }
      
type PdbDebugPoint = 
    {
      Document: int
      Offset: int
      Line: int
      Column: int
      EndLine: int
      EndColumn: int
    }

type PdbMethodData = 
    {
      MethToken: int32
      MethName:string
      LocalSignatureToken: int32
      Params: PdbLocalVar[]
      RootScope: PdbMethodScope option
      DebugRange: (PdbSourceLoc * PdbSourceLoc) option
      DebugPoints: PdbDebugPoint[]
    }

[<NoEquality; NoComparison>]
type PdbData = 
    {
      EntryPoint: int32 option
      Timestamp: int32
      /// MVID of the generated .NET module (used by MDB files to identify debug info)
      ModuleID: byte[]
      Documents: PdbDocumentData[]
      Methods: PdbMethodData[] 
      TableRowCounts: int[]
    }

/// Takes the output file name and returns debug file name.
val getDebugFileName: string -> bool -> string

/// 28 is the size of the IMAGE_DEBUG_DIRECTORY in ntimage.h 
val sizeof_IMAGE_DEBUG_DIRECTORY : System.Int32

val logDebugInfo : string -> PdbData -> unit

#if ENABLE_MONO_SUPPORT
val writeMdbInfo<'a> : string -> string -> PdbData -> 'a
#endif

type BinaryChunk = 
    { size: int32 
      addr: int32 }

type idd =
    { iddCharacteristics: int32;
      iddMajorVersion: int32; (* actually u16 in IMAGE_DEBUG_DIRECTORY *)
      iddMinorVersion: int32; (* actually u16 in IMAGE_DEBUG_DIRECTORY *)
      iddType: int32;
      iddTimestamp: int32;
      iddData: byte[];
      iddChunk: BinaryChunk }

type HashAlgorithm =
    | Sha1
    | Sha256

val generatePortablePdb : embedAllSource: bool -> embedSourceList: string list -> sourceLink: string -> checksumAlgorithm: HashAlgorithm -> showTimes: bool -> info: PdbData -> pathMap:PathMap -> int64 * BlobContentId * MemoryStream * string * byte[]

val compressPortablePdbStream: stream:MemoryStream -> MemoryStream

val getInfoForEmbeddedPortablePdb: uncompressedLength: int64 -> contentId: BlobContentId -> compressedStream: MemoryStream -> pdbfile: string -> cvChunk: BinaryChunk -> pdbChunk: BinaryChunk -> deterministicPdbChunk: BinaryChunk -> checksumPdbChunk: BinaryChunk -> algorithmName: string -> checksum: byte[] -> deterministic: bool -> idd[]

val getInfoForPortablePdb: contentId: BlobContentId -> pdbfile: string -> pathMap: PathMap -> cvChunk: BinaryChunk -> deterministicPdbChunk: BinaryChunk -> checksumPdbChunk: BinaryChunk -> algorithmName: string -> checksum: byte[] -> embeddedPdb: bool -> deterministic: bool -> idd[]

#if !FX_NO_PDB_WRITER
val writePdbInfo : showTimes:bool -> outfile:string -> pdbfile:string -> info:PdbData -> cvChunk:BinaryChunk -> idd[]
#endif

/// Check to see if a scope has a local with the same name as any of its children
/// 
/// If so, do not emit 'scope' itself. Instead, 
///  1. Emit a copy of 'scope' in each true gap, with all locals
///  2. Adjust each child scope to also contain the locals from 'scope', 
///     adding the text " (shadowed)" to the names of those with name conflicts.
val unshadowScopes: PdbMethodScope -> PdbMethodScope[]
