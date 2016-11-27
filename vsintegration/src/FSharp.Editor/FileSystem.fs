// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open System.IO
open System.ComponentModel.Composition

[<Export>]
type FileSystem [<ImportingConstructor>] (openDocumentsTracker: IOpenDocumentsTracker) =
    static let defaultFileSystem = Shim.FileSystem

    let getOpenDocContent (fileName: string) =
        openDocumentsTracker.TryFindOpenDocument fileName
        |> Option.map (fun doc -> 
            let content = doc.Text.Value 
            content |> doc.Encoding.GetBytes)

    interface IFileSystem with
        member __.FileStreamReadShim fileName = 
            getOpenDocContent fileName
            |> Option.map (fun bytes -> new MemoryStream (bytes) :> Stream)
            |> function Some x -> x | None -> defaultFileSystem.FileStreamReadShim fileName
        
        member __.ReadAllBytesShim fileName =
            getOpenDocContent fileName 
            |> function Some x -> x | None -> defaultFileSystem.ReadAllBytesShim fileName
        
        member __.GetLastWriteTimeShim fileName =
            openDocumentsTracker.TryFindOpenDocument fileName
            |> Option.bind (fun doc ->
                if doc.Document.IsDirty then
                    Some doc.LastChangeTime
                else None)
            |> function Some x -> x | None -> defaultFileSystem.GetLastWriteTimeShim fileName
        
        member __.GetTempPathShim() = defaultFileSystem.GetTempPathShim()
        member __.FileStreamCreateShim fileName = defaultFileSystem.FileStreamCreateShim fileName
        member __.FileStreamWriteExistingShim fileName = defaultFileSystem.FileStreamWriteExistingShim fileName
        member __.GetFullPathShim fileName = defaultFileSystem.GetFullPathShim fileName
        member __.IsInvalidPathShim fileName = defaultFileSystem.IsInvalidPathShim fileName
        member __.IsPathRootedShim fileName = defaultFileSystem.IsPathRootedShim fileName
        member __.SafeExists fileName = defaultFileSystem.SafeExists fileName
        member __.FileDelete fileName = defaultFileSystem.FileDelete fileName
        member __.AssemblyLoadFrom fileName = defaultFileSystem.AssemblyLoadFrom fileName
        member __.AssemblyLoad assemblyName = defaultFileSystem.AssemblyLoad assemblyName