// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.EditorServices

open System
open System.IO
open System.IO.MemoryMappedFiles
open System.Reflection.Metadata
open FSharp.NativeInterop
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range

#nowarn "9"

[<Sealed>]
type SemanticClassificationView(mmf: MemoryMappedFile, length) =
    member _.ReadRange(reader: byref<BlobReader>) =
        let startLine = reader.ReadInt32()
        let startColumn = reader.ReadInt32()
        let endLine = reader.ReadInt32()
        let endColumn = reader.ReadInt32()
        let fileIndex = reader.ReadInt32()

        let posStart = mkPos startLine startColumn
        let posEnd = mkPos endLine endColumn
        mkFileIndexRange fileIndex posStart posEnd

    member this.ForEach(f: SemanticClassificationItem -> unit) =
        use view = mmf.CreateViewAccessor(0L, length)
        let mutable reader = BlobReader(view.SafeMemoryMappedViewHandle.DangerousGetHandle() |> NativePtr.ofNativeInt, int length)

        reader.Offset <- 0
        while reader.Offset < reader.Length do
            let m = this.ReadRange &reader
            let sct = reader.ReadInt32()
            let item = SemanticClassificationItem((m, (enum<SemanticClassificationType>(sct))))
            f item

[<Sealed>]
type SemanticClassificationKeyStore(mmf: MemoryMappedFile, length) =
    let mutable isDisposed = false
    let checkDispose() =
        if isDisposed then
            raise (ObjectDisposedException("SemanticClassificationKeyStore"))

    member _.GetView() =
        checkDispose()
        SemanticClassificationView(mmf, length)

    interface IDisposable with

            member _.Dispose() =
                isDisposed <- true
                mmf.Dispose()

[<Sealed>] 
type SemanticClassificationKeyStoreBuilder() =

    let b = BlobBuilder()

    member _.WriteAll (semanticClassification: SemanticClassificationItem[]) =
        use ptr = fixed semanticClassification
        b.WriteBytes(NativePtr.ofNativeInt (NativePtr.toNativeInt ptr), semanticClassification.Length * sizeof<SemanticClassificationItem>)

    member _.TryBuildAndReset() =
        if b.Count > 0 then
            let length = int64 b.Count
            let mmf = 
                let mmf =
                    MemoryMappedFile.CreateNew(
                        null, 
                        length, 
                        MemoryMappedFileAccess.ReadWrite, 
                        MemoryMappedFileOptions.None, 
                        HandleInheritability.None)
                use stream = mmf.CreateViewStream(0L, length, MemoryMappedFileAccess.ReadWrite)
                b.WriteContentTo stream
                mmf

            b.Clear()

            Some(new SemanticClassificationKeyStore(mmf, length))       
        else
            b.Clear()
            None