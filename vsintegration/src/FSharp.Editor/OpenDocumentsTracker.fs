// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.IO
open System.Text
open System.ComponentModel.Composition
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open System.Collections.Generic

type IOpenDocument =
    abstract Text : Lazy<string>

type IOpenDocumentsTracker<'OpenDoc when 'OpenDoc :> IOpenDocument> =
    abstract MapOpenDocuments: (KeyValuePair<string, 'OpenDoc> -> 'a) -> seq<'a>
    abstract TryFindOpenDocument: string -> 'OpenDoc option
    abstract TryGetDocumentText: string -> string option
    abstract DocumentChanged: IEvent<string>
    abstract DocumentClosed: IEvent<string>

[<NoComparison>]
type OpenDocument =
    { Document: ITextDocument
      Snapshot: ITextSnapshot 
      Encoding: Encoding
      LastChangeTime: DateTime
      ViewCount: int }
    static member Create document snapshot encoding lastChangeTime = 
        { Document = document
          Snapshot = snapshot
          Encoding = encoding
          LastChangeTime = lastChangeTime
          ViewCount = 1 }
    member private x.text = lazy (x.Snapshot.GetText())
    member x.Text = x.text
    interface IOpenDocument with
        member x.Text = x.text

type IVSOpenDocumentsTracker =
    inherit IOpenDocumentsTracker<OpenDocument>
    abstract RegisterView: IWpfTextView -> unit

type IOpenDocumentsTracker = IOpenDocumentsTracker<OpenDocument>

[<Export(typeof<IVSOpenDocumentsTracker>); Export(typeof<IOpenDocumentsTracker<OpenDocument>>)>]
type OpenDocumentsTracker [<ImportingConstructor>](textDocumentFactoryService: ITextDocumentFactoryService) =
    [<VolatileField>]
    let mutable openDocs = Map.empty
    let documentChanged = Event<_>()
    let documentClosed = Event<_>()
    let tryFindDoc path = openDocs |> Map.tryFind path
    let addDoc path doc = openDocs <- openDocs |> Map.add path doc
    
    let updateDoc path (f: OpenDocument -> OpenDocument) =
        match tryFindDoc path with
        | Some doc -> addDoc path (f doc)
        | None -> ()

    let tryGetDocument buffer = 
        match textDocumentFactoryService.TryGetTextDocument buffer with
        | true, doc -> Some doc
        | _ -> None

    interface IVSOpenDocumentsTracker with
        member __.RegisterView (view: IWpfTextView) = 
            match tryGetDocument view.TextBuffer with
            | Some doc ->
                let path = doc.FilePath
                
                let textBufferChanged (args: TextContentChangedEventArgs) =
                    if openDocs |> Map.containsKey path then
                        updateDoc path (fun doc -> { doc with Snapshot = args.After
                                                              LastChangeTime = DateTime.UtcNow })
                        documentChanged.Trigger path
                
                let textBufferChangedSubscription = view.TextBuffer.ChangedHighPriority.Subscribe textBufferChanged
                
                let rec viewClosed _ =
                    match tryFindDoc path with
                    | Some doc when doc.ViewCount = 1 ->
                        textBufferChangedSubscription.Dispose()
                        viewClosedSubscription.Dispose()
                        openDocs <- Map.remove path openDocs
                        documentClosed.Trigger path
                    | Some _ -> updateDoc path (fun doc -> { doc with ViewCount = doc.ViewCount - 1 })
                    | None -> ()
                
                and viewClosedSubscription: IDisposable = view.Closed.Subscribe viewClosed
                try 
                    let lastWriteTime = FileInfo(path).LastWriteTimeUtc
                    let doc = 
                        match tryFindDoc path with
                        | Some doc -> { doc with ViewCount = doc.ViewCount + 1 }
                        | None -> OpenDocument.Create doc view.TextBuffer.CurrentSnapshot doc.Encoding lastWriteTime
                    addDoc path doc
                with _ -> ()
            | None -> ()
        
        member __.MapOpenDocuments f = Seq.map f openDocs
        member __.TryFindOpenDocument path = Map.tryFind path openDocs
        member __.TryGetDocumentText path = 
            let doc = openDocs |> Map.tryFind path 
            doc |> Option.map (fun x -> x.Text.Value)
        member __.DocumentChanged = documentChanged.Publish
        member __.DocumentClosed = documentClosed.Publish

