// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Binary reader.  Read a .NET binary and concert it to Abstract IL data
/// structures.
/// 
/// NOTE:
///   - The metadata in the loaded modules will be relative to 
///     those modules, e.g. ILScopeRef.Local will mean "local to 
///     that module".  You must use [rescopeILType] etc. if you want to include 
///     (i.e. copy) the metadata into your own module. 
///
///   - PDB (debug info) reading/folding:
///     The PDB reader is invoked if you give a PDB path 
///     This indicates if you want to search for PDB files and have the 
///     reader fold them in.  You cannot currently name the pdb file 
///     directly - you can only name the path.  Giving "None" says 
///     "do not read the PDB file even if one exists". 
/// 
///     The debug info appears primarily as I_seqpoint annotations in 
///     the instruction streams.  Unfortunately the PDB information does
///     not, for example, tell you how to map back from a class definition
///     to a source code line number - you will need to explicitly search
///     for a sequence point in the code for one of the methods of the 
///     class.  That is not particularly satisfactory, and it may be
///     a good idea to build a small library which extracts the information
///     you need.  
module internal Microsoft.FSharp.Compiler.AbstractIL.ILBinaryReader 

open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open System.IO


type ILReaderOptions =
   { pdbPath: string option;
     ilGlobals: ILGlobals;
     optimizeForMemory: bool  (* normally off, i.e. optimize for startup-path speed *) }

val mkDefault :  ILGlobals -> ILReaderOptions

// The non-memory resources (i.e. the file handle) associated with 
// the read can be recovered by calling Dispose.  Any remaining 
// lazily-computed items in the metadata graph returned by MetadataOfILModuleReader 
// will no longer be valid. 
[<Sealed>]
type ILModuleReader =
    member ILModuleDef : ILModuleDef
    member ILAssemblyRefs : ILAssemblyRef list
    interface System.IDisposable
    
val OpenILModuleReader: string -> ILReaderOptions -> ILModuleReader

/// Open a binary reader, except first copy the entire contents of the binary into 
/// memory, close the file and ensure any subsequent reads happen from the in-memory store. 
/// PDB files may not be read with this option. 
val OpenILModuleReaderAfterReadingAllBytes: string -> ILReaderOptions -> ILModuleReader

/// Open a binary reader based on the given bytes. 
val OpenILModuleReaderFromBytes: fileNameForDebugOutput:string -> assemblyContents: byte[] -> options: ILReaderOptions -> ILModuleReader

#if STATISTICS
(* report statistics from all reads *)
val report: TextWriter -> unit
#endif
