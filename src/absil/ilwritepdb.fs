// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.AbstractIL.ILPdbWriter

open System
open System.Collections.Generic 
open System.Collections.Immutable
open System.IO
open System.Reflection
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection.PortableExecutable
open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Support 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Range

// -------------------------------------------------------------------- 
// PDB types
// --------------------------------------------------------------------  
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
    override x.ToString() = sprintf "(%d,%d)-(%d,%d)" x.Line x.Column x.EndLine x.EndColumn

type PdbMethodData = 
    { MethToken: int32
      MethName:string
      LocalSignatureToken: int32
      Params: PdbLocalVar array
      RootScope: PdbMethodScope
      Range: (PdbSourceLoc * PdbSourceLoc) option
      SequencePoints: PdbSequencePoint array }

module SequencePoint = 
    let orderBySource sp1 sp2 = 
        let c1 = compare sp1.Document sp2.Document
        if c1 <> 0 then 
            c1 
        else 
            let c1 = compare sp1.Line sp2.Line
            if c1 <> 0 then 
                c1 
            else 
                compare sp1.Column sp2.Column 
        
    let orderByOffset sp1 sp2 = 
        compare sp1.Offset sp2.Offset 

/// 28 is the size of the IMAGE_DEBUG_DIRECTORY in ntimage.h 
let sizeof_IMAGE_DEBUG_DIRECTORY = 28 

[<NoEquality; NoComparison>]
type PdbData = 
    { EntryPoint: int32 option
      Timestamp: int32
      ModuleID: byte[]
      Documents: PdbDocumentData[]
      Methods: PdbMethodData[] 
      TableRowCounts: int[] }

//---------------------------------------------------------------------
// Portable PDB Writer
//---------------------------------------------------------------------

type idd =
    { iddCharacteristics: int32;
      iddMajorVersion: int32; (* actually u16 in IMAGE_DEBUG_DIRECTORY *)
      iddMinorVersion: int32; (* actually u16 in IMAGE_DEBUG_DIRECTORY *)
      iddType: int32;
      iddData: byte[];}

let magicNumber = 0x53445352L
let pdbGetDebugInfo (mvid:byte[]) (filepath:string) = 
    let iddDataBuffer = 
        let path = (System.Text.Encoding.UTF8.GetBytes filepath)
        let buffer = Array.zeroCreate (sizeof<int32> + mvid.Length + sizeof<int32> + path.Length + 1)

        let offset, size = 0, sizeof<int32>                                     // Magic Number RSDS dword: 0x53445352L
        Buffer.BlockCopy(BitConverter.GetBytes(magicNumber), 0, buffer, offset, size)

        let offset, size = offset + size, mvid.Length                           // mvid Guid
        Buffer.BlockCopy(mvid, 0, buffer, offset, size)

        let offset, size = offset + size, sizeof<int32>                         // # of pdb files generated (1)
        Buffer.BlockCopy(BitConverter.GetBytes(1), 0, buffer, offset, size)

        let offset = offset + size                                              // Path to pdb string
        Buffer.BlockCopy(path, 0, buffer, offset, path.Length)
        buffer

    { iddCharacteristics = 0x0;                                                 // Reserved
      iddMajorVersion = 0x0;                                                    // VersionMajor should be 0
      iddMinorVersion = 0x0;                                                    // VersionMinor should be 0
      iddType = 0x2;                                                            // IMAGE_DEBUG_TYPE_CODEVIEW
      iddData = iddDataBuffer }                                                 // Path name to the pdb file when built

// Document checksum algorithms
let guidSourceHashMD5 = System.Guid(0x406ea660u, 0x64cfus, 0x4c82us, 0xb6uy, 0xf0uy, 0x42uy, 0xd4uy, 0x81uy, 0x72uy, 0xa7uy, 0x99uy) //406ea660-64cf-4c82-b6f0-42d48172a799
let hashSizeOfMD5 = 16

// If the FIPS algorithm policy is enabled on the computer (e.g., for US government employees and contractors)
// then obtaining the MD5 implementation in BCL will throw. 
// In this case, catch the failure, and not set a checksum. 
let checkSum (url:string) =
    try
        use file = FileSystem.FileStreamReadShim(url)
        use md5 = System.Security.Cryptography.MD5.Create()
        let checkSum = md5.ComputeHash(file)
        Some (guidSourceHashMD5, checkSum)
    with _ -> None

//------------------------------------------------------------------------------
// PDB Writer.  The function [WritePdbInfo] abstracts the 
// imperative calls to the Symbol Writer API.
//------------------------------------------------------------------------------

// This function takes output file name and returns debug file name.
let getDebugFileName outfile = 
#if ENABLE_MONO_SUPPORT
  if IL.runningOnMono then 
      outfile + ".mdb"
  else 
#endif
      (Filename.chopExtension outfile) + ".pdb" 

let sortMethods showTimes info =
    reportTime showTimes (sprintf "PDB: Defined %d documents" info.Documents.Length)
    Array.sortInPlaceBy (fun x -> x.MethToken) info.Methods
    reportTime showTimes (sprintf "PDB: Sorted %d methods" info.Methods.Length)
    ()

let getRowCounts tableRowCounts =
    let builder = ImmutableArray.CreateBuilder<int>(tableRowCounts |> Array.length)
    tableRowCounts |> Seq.iter(fun x -> builder.Add(x))
    builder.MoveToImmutable()

let fixupOverlappingSequencePoints fixupSPs showTimes methods =
    // This next bit is a workaround.  The sequence points we get 
    // from F# (which has nothing to do with this module) are actually expression 
    // marks, i.e. the source ranges they denote are typically 
    // nested, and each point indicates where the  
    // code for an expression with a particular range begins.  
    // This is in many ways a much more convenient form to emit. 
    // However, it is not the form that debug tools accept nicely. 
    // However, sequence points are really a non-overlapping, non-nested 
    // partition of the source code of a method.  So here we shorten the 
    // length of all sequence point marks so they do not go further than 
    // the next sequence point in the source. 
    let spCounts =  methods |> Array.map (fun x -> x.SequencePoints.Length)
    let allSps = methods |> Array.map (fun x -> x.SequencePoints)
                         |> Array.concat 
                         |> Array.mapi (fun i sp -> i, sp)
    if fixupSPs then 
        // sort the sequence points into source order 
        Array.sortInPlaceWith (fun (_,sp1) (_,sp2) -> SequencePoint.orderBySource sp1 sp2) allSps
        // shorten the ranges of any that overlap with following sequence points 
        // sort the sequence points back into offset order 
        for i = 0 to Array.length allSps - 2 do
            let n,sp1 = allSps.[i]
            let _,sp2 = allSps.[i+1]
            if (sp1.Document = sp2.Document) && 
               (sp1.EndLine > sp2.Line || 
                (sp1.EndLine = sp2.Line &&
                 sp1.EndColumn >= sp2.Column)) then
              let adjustToPrevLine = (sp1.Line < sp2.Line)
              allSps.[i] <-  n,{sp1 with EndLine = (if adjustToPrevLine then sp2.Line-1 else sp2.Line)
                                         EndColumn = (if adjustToPrevLine then 80 else sp2.Column) }
        reportTime showTimes (sprintf "PDB: fixupOverlappingSequencePoints %d" (allSps |> Array.length) )
        Array.sortInPlaceBy fst allSps
    spCounts, allSps

let writePortablePdbInfo (fixupSPs:bool) showTimes fpdb (info:PdbData) = 

    try FileSystem.FileDelete fpdb with _ -> ()

    sortMethods showTimes info
    let _spCounts, _allSps = fixupOverlappingSequencePoints fixupSPs showTimes info.Methods
    let externalRowCounts = getRowCounts info.TableRowCounts
    let docs = 
        if info.Documents = null then 
            Array.empty<PdbDocumentData>
        else
            info.Documents

    let metadata = MetadataBuilder()
    let serializeDocumentName (name:string) =
        let count s c = s |> Seq.filter(fun ch -> if c = ch then true else false) |> Seq.length

        let s1, s2 = '/', '\\'
        let separator = if (count name s1) >= (count name s2) then s1 else s2
 
        let writer = new BlobBuilder()
        writer.WriteByte(byte(separator))

        for part in name.Split( [| separator |] ) do
            let partIndex = MetadataTokens.GetHeapOffset(BlobHandle.op_Implicit(metadata.GetOrAddBlobUTF8(part)))
            writer.WriteCompressedInteger(int(partIndex))

        metadata.GetOrAddBlob(writer);

    let corSymLanguageTypeFSharp = System.Guid(0xAB4F38C9u, 0xB6E6us, 0x43baus, 0xBEuy, 0x3Buy, 0x58uy, 0x08uy, 0x0Buy, 0x2Cuy, 0xCCuy, 0xE3uy)
    let documentIndex =
        let mutable index = new Dictionary<string, DocumentHandle>(docs.Length)
        metadata.SetCapacity(TableIndex.Document, docs.Length)
        for doc in docs do
            let handle =
                match checkSum doc.File with
                | Some (hashAlg, checkSum) ->
                    serializeDocumentName doc.File,
                    metadata.GetOrAddGuid(hashAlg),
                    metadata.GetOrAddBlob(checkSum.ToImmutableArray()),
                    metadata.GetOrAddGuid(corSymLanguageTypeFSharp)
                | None ->
                    serializeDocumentName doc.File,
                    metadata.GetOrAddGuid(System.Guid.Empty),
                    metadata.GetOrAddBlob(ImmutableArray<byte>.Empty),
                    metadata.GetOrAddGuid(corSymLanguageTypeFSharp)
                |> metadata.AddDocument
            index.Add(doc.File, handle)
        index

    metadata.SetCapacity(TableIndex.MethodDebugInformation, info.Methods.Length)
    info.Methods |> Array.iteri (fun _i minfo ->
        let docHandle, sequencePointBlob =
            let sps =
                if minfo.SequencePoints = null then
                    Array.empty<PdbSequencePoint>
                else 
                    match minfo.Range with
                    | None -> Array.empty<PdbSequencePoint>
                    | Some (_,_) -> minfo.SequencePoints
 
            let getDocumentHandle d = 
                if docs.Length = 0 || d < 0 || d > docs.Length then 
                    Unchecked.defaultof<DocumentHandle>
                else 
                    match documentIndex.TryGetValue(docs.[d].File) with
                    | false, _ -> Unchecked.defaultof<DocumentHandle>
                    | true, f  -> f

            // Return a document that the entire method body is declared within. 
            // If part of the method body is in another document returns nil handle.
            let tryGetSingleDocumentIndex =
                let mutable singleDocumentIndex = 0
                for i in 1 .. sps.Length - 1 do
                    let index = sps.[i].Document
                    if index <> singleDocumentIndex then 
                        singleDocumentIndex <- index
                singleDocumentIndex

            if sps.Length = 0 then 
                Unchecked.defaultof<DocumentHandle>, Unchecked.defaultof<BlobHandle>
            else
                let builder = new BlobBuilder()
                builder.WriteCompressedInteger(minfo.LocalSignatureToken)

                let mutable previousNonHiddenStartLine = -1
                let mutable previousNonHiddenStartColumn = -1
                let mutable previousDocumentIndex = -1
                let mutable singleDocumentIndex = tryGetSingleDocumentIndex
                let mutable currentDocumentIndex = previousDocumentIndex

                for i in 0 .. (sps.Length - 1) do

                    if previousDocumentIndex <> currentDocumentIndex then
                        // optional document in header or document record:
                        if previousDocumentIndex <> -1   then
                            // optional document in header or document record
                            builder.WriteCompressedInteger(0)
                        builder.WriteCompressedInteger(currentDocumentIndex)
                        previousDocumentIndex <- currentDocumentIndex

                    // delta IL offset:
                    if i > 0 then 
                        builder.WriteCompressedInteger(sps.[i].Offset - sps.[i - 1].Offset)
                    else
                        builder.WriteCompressedInteger(sps.[i].Offset)

                        // F# does not support hidden sequence points yet !!!
                        // if (sequencePoints[i].IsHidden)
                        // {
                        //     builder.WriteInt16(0);
                        //     continue;
                        // }

                    let deltaLines = sps.[i].EndLine - sps.[i].Line;
                    let deltaColumns = sps.[i].EndColumn - sps.[i].Column;
                    builder.WriteCompressedInteger(deltaLines);

                    if deltaLines = 0 then 
                        builder.WriteCompressedInteger(deltaColumns)
                    else
                        builder.WriteCompressedSignedInteger(deltaColumns)

                    // delta Start Lines & Columns:
                    if previousNonHiddenStartLine < 0 then
                        builder.WriteCompressedInteger(sps.[i].Line)
                        builder.WriteCompressedInteger(sps.[i].Column)
                    else
                        builder.WriteCompressedSignedInteger(sps.[i].Line - previousNonHiddenStartLine)
                        builder.WriteCompressedSignedInteger(sps.[i].Column - previousNonHiddenStartColumn)

                    previousNonHiddenStartLine <- sps.[i].Line
                    previousNonHiddenStartColumn <- sps.[i].Column

                getDocumentHandle singleDocumentIndex, metadata.GetOrAddBlob(builder)

        // Write the scopes 
        let mutable lastLocalVariableHandle = Unchecked.defaultof<LocalVariableHandle>
        let nextHandle handle = MetadataTokens.LocalVariableHandle(MetadataTokens.GetRowNumber(LocalVariableHandle.op_Implicit(handle)) + 1)
        let rec writePdbScope top scope =
            if top || scope.Locals.Length <> 0 || scope.Children.Length <> 0 then 
                lastLocalVariableHandle <- nextHandle lastLocalVariableHandle
                metadata.AddLocalScope(MetadataTokens.MethodDefinitionHandle(minfo.MethToken), 
                                       Unchecked.defaultof<ImportScopeHandle>, 
                                       lastLocalVariableHandle, 
                                       Unchecked.defaultof<LocalConstantHandle>, 
                                       scope.StartOffset, 
                                       scope.EndOffset - scope.StartOffset) |>ignore
                for localVariable in scope.Locals do
                    lastLocalVariableHandle <- metadata.AddLocalVariable(LocalVariableAttributes.None, localVariable.Index, metadata.GetOrAddString(localVariable.Name))
                scope.Children |> Array.iter (writePdbScope false)

        writePdbScope true minfo.RootScope
        metadata.AddMethodDebugInformation(docHandle, sequencePointBlob) |> ignore)

    let entryPoint =
        match info.EntryPoint with 
        | None -> MetadataTokens.MethodDefinitionHandle(0)
        | Some x -> MetadataTokens.MethodDefinitionHandle(x) 

    let serializer = PortablePdbBuilder(metadata, externalRowCounts, entryPoint, null )
    let blobBuilder = new BlobBuilder()
    serializer.Serialize(blobBuilder) |> ignore

    reportTime showTimes "PDB: Created"
    use portablePdbStream = new FileStream(fpdb, FileMode.Create, FileAccess.ReadWrite)
    blobBuilder.WriteContentTo(portablePdbStream)
    reportTime showTimes "PDB: Closed"
    pdbGetDebugInfo info.ModuleID fpdb

#if FX_NO_PDB_WRITER
#else
//---------------------------------------------------------------------
// PDB Writer.  The function [WritePdbInfo] abstracts the 
// imperative calls to the Symbol Writer API.
//---------------------------------------------------------------------
let writePdbInfo fixupOverlappingSequencePoints showTimes f fpdb info =

    try FileSystem.FileDelete fpdb with _ -> ()

    let pdbw = ref Unchecked.defaultof<PdbWriter>

    try
        pdbw := pdbInitialize f fpdb
    with _ -> error(Error(FSComp.SR.ilwriteErrorCreatingPdb(fpdb), rangeCmdArgs))

    match info.EntryPoint with 
    | None -> () 
    | Some x -> pdbSetUserEntryPoint !pdbw x 

    let docs = info.Documents |> Array.map (fun doc -> pdbDefineDocument !pdbw doc.File)
    let getDocument i = 
      if i < 0 || i > docs.Length then failwith "getDocument: bad doc number"
      docs.[i]
    reportTime showTimes (sprintf "PDB: Defined %d documents" info.Documents.Length)
    Array.sortInPlaceBy (fun x -> x.MethToken) info.Methods
    reportTime showTimes (sprintf "PDB: Sorted %d methods" info.Methods.Length)

    // This next bit is a workaround.  The sequence points we get 
    // from F# (which has nothing to do with this module) are actually expression 
    // marks, i.e. the source ranges they denote are typically 
    // nested, and each point indicates where the  
    // code for an expression with a particular range begins.  
    // This is in many ways a much more convenient form to emit. 
    // However, it is not the form that debug tools accept nicely. 
    // However, sequence points are really a non-overlapping, non-nested 
    // partition of the source code of a method.  So here we shorten the 
    // length of all sequence point marks so they do not go further than 
    // the next sequence point in the source. 
    let spCounts =  info.Methods |> Array.map (fun x -> x.SequencePoints.Length)
    let allSps = Array.concat (Array.map (fun x -> x.SequencePoints) info.Methods |> Array.toList)
    let allSps = Array.mapi (fun i sp -> (i,sp)) allSps
    if fixupOverlappingSequencePoints then 
        // sort the sequence points into source order 
        Array.sortInPlaceWith (fun (_,sp1) (_,sp2) -> SequencePoint.orderBySource sp1 sp2) allSps
        // shorten the ranges of any that overlap with following sequence points 
        // sort the sequence points back into offset order 
        for i = 0 to Array.length allSps - 2 do
            let n,sp1 = allSps.[i]
            let _,sp2 = allSps.[i+1]
            if (sp1.Document = sp2.Document) && 
               (sp1.EndLine > sp2.Line || 
                (sp1.EndLine = sp2.Line &&
                 sp1.EndColumn >= sp2.Column)) then
              let adjustToPrevLine = (sp1.Line < sp2.Line)
              allSps.[i] <-  n,{sp1 with EndLine = (if adjustToPrevLine then sp2.Line-1 else sp2.Line)
                                         EndColumn = (if adjustToPrevLine then 80 else sp2.Column) }
        Array.sortInPlaceBy fst allSps

    let spOffset = ref 0
    info.Methods |> Array.iteri (fun i minfo ->

          let sps = Array.sub allSps !spOffset spCounts.[i]
          spOffset := !spOffset + spCounts.[i]
          begin match minfo.Range with 
          | None -> () 
          | Some (a,b) ->
              pdbOpenMethod !pdbw minfo.MethToken

              pdbSetMethodRange !pdbw 
                (getDocument a.Document) a.Line a.Column
                (getDocument b.Document) b.Line b.Column

              // Partition the sequence points by document 
              let spsets =
                let res = (Map.empty : Map<int,PdbSequencePoint list ref>)
                let add res (_,sp) = 
                  let k = sp.Document
                  match Map.tryFind k res with
                    | Some xsR -> xsR := sp :: !xsR; res
                    | None     -> Map.add k (ref [sp]) res
               
                let res = Array.fold add res sps
                let res = Map.toList res  // ordering may not be stable 
                List.map (fun (_,x) -> Array.ofList !x) res

              spsets |> List.iter (fun spset -> 
                  if spset.Length > 0 then 
                    Array.sortInPlaceWith SequencePoint.orderByOffset spset
                    let sps = 
                      spset |> Array.map (fun sp -> 
                           // Ildiag.dprintf "token 0x%08lx has an sp at offset 0x%08x\n" minfo.MethToken sp.Offset 
                           (sp.Offset, sp.Line, sp.Column,sp.EndLine, sp.EndColumn)) 
                  // Use of alloca in implementation of pdbDefineSequencePoints can give stack overflow here 
                    if sps.Length < 5000 then 
                      pdbDefineSequencePoints !pdbw (getDocument spset.[0].Document) sps)

              // Write the scopes 
              let rec writePdbScope top sco = 
                  if top || sco.Locals.Length <> 0 || sco.Children.Length <> 0 then 
                      pdbOpenScope !pdbw sco.StartOffset
                      sco.Locals |> Array.iter (fun v -> pdbDefineLocalVariable !pdbw v.Name v.Signature v.Index)
                      sco.Children |> Array.iter (writePdbScope false)
                      pdbCloseScope !pdbw sco.EndOffset
              writePdbScope true minfo.RootScope 

              pdbCloseMethod !pdbw
          end)
    reportTime showTimes "PDB: Wrote methods"

    let res = pdbWriteDebugInfo !pdbw
    for pdbDoc in docs do pdbCloseDocument pdbDoc
    pdbClose !pdbw f fpdb;

    reportTime showTimes "PDB: Closed"
    { iddCharacteristics = res.iddCharacteristics;
      iddMajorVersion = res.iddMajorVersion;
      iddMinorVersion = res.iddMinorVersion;
      iddType = res.iddType;
      iddData = res.iddData}
#endif

#if ENABLE_MONO_SUPPORT
//---------------------------------------------------------------------
// Support functions for calling 'Mono.CompilerServices.SymbolWriter'
// assembly dynamically if it is available to the compiler
//---------------------------------------------------------------------
open Microsoft.FSharp.Reflection

// Dynamic invoke operator. Implements simple overload resolution based 
// on the name and number of parameters only.
// Supports the following cases:
//   obj?Foo()        // call with no arguments
//   obj?Foo(1, "a")  // call with two arguments (extracted from tuple)
// NOTE: This doesn�t actually handle all overloads.  It just picks first entry with right 
// number of arguments.
let (?) this memb (args:'Args) : 'R = 
    // Get array of 'obj' arguments for the reflection call
    let args = 
        if typeof<'Args> = typeof<unit> then [| |]
        elif FSharpType.IsTuple typeof<'Args> then Microsoft.FSharp.Reflection.FSharpValue.GetTupleFields(args)
        else [| box args |]
    
    // Get methods and perform overload resolution
    let methods = this.GetType().GetMethods()
    let bestMatch = methods |> Array.tryFind (fun mi -> mi.Name = memb && mi.GetParameters().Length = args.Length)
    match bestMatch with
    | Some(mi) -> unbox(mi.Invoke(this, args))        
    | None -> error(Error(FSComp.SR.ilwriteMDBMemberMissing(memb), rangeCmdArgs))

// Creating instances of needed classes from 'Mono.CompilerServices.SymbolWriter' assembly

let monoCompilerSvc = new AssemblyName("Mono.CompilerServices.SymbolWriter, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756")
let ctor (asmName:AssemblyName) clsName (args:obj[]) = 
    let asm = Assembly.Load(asmName)
    let ty = asm.GetType(clsName)
    System.Activator.CreateInstance(ty, args)

let createSourceMethodImpl (name:string) (token:int) (namespaceID:int) = 
    ctor monoCompilerSvc "Mono.CompilerServices.SymbolWriter.SourceMethodImpl" [| box name; box token; box namespaceID |]

let createWriter (f:string) = 
    ctor monoCompilerSvc "Mono.CompilerServices.SymbolWriter.MonoSymbolWriter" [| box f |]

//---------------------------------------------------------------------
// MDB Writer.  Generate debug symbols using the MDB format
//---------------------------------------------------------------------
let writeMdbInfo fmdb f info = 
    // Note, if we can�t delete it code will fail later
    try FileSystem.FileDelete fmdb with _ -> ()

    // Try loading the MDB symbol writer from an assembly available on Mono dynamically
    // Report an error if the assembly is not available.    
    let wr = 
        try createWriter f
        with e -> error(Error(FSComp.SR.ilwriteErrorCreatingMdb(), rangeCmdArgs))

    // NOTE: MonoSymbolWriter doesn't need information about entrypoints, so 'info.EntryPoint' is unused here.
    // Write information about Documents. Returns '(SourceFileEntry*CompileUnitEntry)[]'
    let docs =
        [| for doc in info.Documents do
             let doc = wr?DefineDocument(doc.File)
             let unit = wr?DefineCompilationUnit(doc)
             yield doc, unit |]

    let getDocument i = 
        if i < 0 || i >= Array.length docs then failwith "getDocument: bad doc number" else docs.[i]

    // Sort methods and write them to the MDB file
    Array.sortInPlaceBy (fun x -> x.MethToken) info.Methods
    for meth in info.Methods do
        // Creates an instance of 'SourceMethodImpl' which is a private class that implements 'IMethodDef' interface
        // We need this as an argument to 'OpenMethod' below. Using private class is ugly, but since we don't reference
        // the assembly, the only way to implement 'IMethodDef' interface would be dynamically using Reflection.Emit...
        let sm = createSourceMethodImpl meth.MethName meth.MethToken 0
        match meth.Range with
        | Some(mstart, _) ->
            // NOTE: 'meth.Params' is not needed, Mono debugger apparently reads this from meta-data
            let _, cue = getDocument mstart.Document
            wr?OpenMethod(cue, 0, sm) |> ignore

            // Write sequence points
            for sp in meth.SequencePoints do
                wr?MarkSequencePoint(sp.Offset, cue?get_SourceFile(), sp.Line, sp.Column, false)

            // Walk through the tree of scopes and write all variables
            let rec writeScope (scope:PdbMethodScope) = 
                wr?OpenScope(scope.StartOffset) |> ignore
                for local in scope.Locals do
                    wr?DefineLocalVariable(local.Index, local.Name)
                for child in scope.Children do 
                    writeScope(child)
                wr?CloseScope(scope.EndOffset)          
            writeScope(meth.RootScope)

            // Finished generating debug information for the curretn method
            wr?CloseMethod()
        | _ -> ()

    // Finalize - MDB requires the MVID of the generated .NET module
    let moduleGuid = new System.Guid(info.ModuleID |> Array.map byte)
    wr?WriteSymbolFile(moduleGuid)
#endif

//---------------------------------------------------------------------
// Dumps debug info into a text file for testing purposes
//---------------------------------------------------------------------
open Printf

let logDebugInfo (outfile:string) (info:PdbData) = 
    use sw = new StreamWriter(new FileStream(outfile + ".debuginfo", FileMode.Create))

    fprintfn sw "ENTRYPOINT\r\n  %b\r\n" info.EntryPoint.IsSome
    fprintfn sw "DOCUMENTS"
    for i, doc in Seq.zip [0 .. info.Documents.Length-1] info.Documents do
      fprintfn sw " [%d] %s" i doc.File
      fprintfn sw "     Type: %A" doc.DocumentType
      fprintfn sw "     Language: %A" doc.Language
      fprintfn sw "     Vendor: %A" doc.Vendor

    // Sort methods (because they are sorted in PDBs/MDBs too)
    fprintfn sw "\r\nMETHODS"
    Array.sortInPlaceBy (fun x -> x.MethToken) info.Methods
    for meth in info.Methods do
      fprintfn sw " %s" meth.MethName
      fprintfn sw "     Params: %A" [ for p in meth.Params -> sprintf "%d: %s" p.Index p.Name ]
      fprintfn sw "     Range: %A" (meth.Range |> Option.map (fun (f, t) -> 
                                      sprintf "[%d,%d:%d] - [%d,%d:%d]" f.Document f.Line f.Column t.Document t.Line t.Column))
      fprintfn sw "     Points:"

      for sp in meth.SequencePoints do
        fprintfn sw "      - Doc: %d Offset:%d [%d:%d]-[%d-%d]" sp.Document sp.Offset sp.Line sp.Column sp.EndLine sp.EndColumn

      // Walk through the tree of scopes and write all variables
      fprintfn sw "     Scopes:"
      let rec writeScope offs (scope:PdbMethodScope) = 
        fprintfn sw "      %s- [%d-%d]" offs scope.StartOffset scope.EndOffset
        if scope.Locals.Length > 0 then
          fprintfn sw "      %s  Locals: %A" offs [ for p in scope.Locals -> sprintf "%d: %s" p.Index p.Name ]
        for child in scope.Children do writeScope (offs + "  ") child
      writeScope "" meth.RootScope
      fprintfn sw ""
