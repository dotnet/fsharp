// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// The ILPdbWriter 
module internal Microsoft.FSharp.Compiler.AbstractIL.ILPdbWriter 

open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.ILAsciiWriter 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.Types  
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.BinaryConstants 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Support 

open Microsoft.FSharp.Compiler.DiagnosticMessage
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Range

open System.Collections.Generic 
open System.IO

type PdbDocumentData = ILSourceDocument

type PdbLocalVar = 
    { Name: string
      Signature: byte[] 
      /// the local index the name corresponds to
      Index: int32  }

type PdbMethodScope = 
    { Children: PdbMethodScope array
      StartOffset: int
      EndOffset: int
      Locals: PdbLocalVar array
      (* REVIEW open_namespaces: pdb_namespace array *) }

type PdbSourceLoc = 
    { Document: int
      Line: int
      Column: int }
      
type PdbSequencePoint = 
    { Document: int
      Offset: int
      Line: int
      Column: int
      EndLine: int
      EndColumn: int }
     override ToString: unit -> string

type PdbMethodData = 
    { MethToken: int32
      MethName:string
      LocalSignatureToken: int32
      Params: PdbLocalVar array
      RootScope: PdbMethodScope
      Range: (PdbSourceLoc * PdbSourceLoc) option
      SequencePoints: PdbSequencePoint array }

[<NoEquality; NoComparison>]
type PdbData = 
    { EntryPoint: int32 option
      Timestamp: int32
      ModuleID: byte[]                                              // MVID of the generated .NET module (used by MDB files to identify debug info)
      Documents: PdbDocumentData[]
      Methods: PdbMethodData[] 
      TableRowCounts: int[] }

/// 28 is the size of the IMAGE_DEBUG_DIRECTORY in ntimage.h 
val sizeof_IMAGE_DEBUG_DIRECTORY : System.Int32
val DumpDebugInfo : string -> PdbData -> unit

#if ENABLE_MONO_SUPPORT
val WriteMdbInfo<'a> : string -> string -> PdbData -> 'a
#endif

//#if FX_PORTABLEPDB_WRITER
type idd =
    { iddCharacteristics: int32;
      iddMajorVersion: int32; (* actually u16 in IMAGE_DEBUG_DIRECTORY *)
      iddMinorVersion: int32; (* actually u16 in IMAGE_DEBUG_DIRECTORY *)
      iddType: int32;
      iddData: byte[]; }

val WritePortablePdbInfo : fixupOverlappingSequencePoints:bool -> showTimes:bool -> fpdb:string -> info:PdbData -> idd
//#endif

#if !FX_NO_PDB_WRITER
val WritePdbInfo : fixupOverlappingSequencePoints:bool -> showTimes:bool -> f:string -> fpdb:string -> info:PdbData -> idd
#endif
