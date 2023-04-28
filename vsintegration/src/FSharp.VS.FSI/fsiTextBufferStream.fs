// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Utilities

// This type wraps the IVsTextLines which contains the FSI session (output and input).
// It provides the API for writing directly to the read-only part of the buffer.
// It extends the read-only marker on the buffer (making the written text read-only).
//
type internal TextBufferStream(textLines:ITextBuffer, contentTypeRegistry: IContentTypeRegistryService) = 
    do if null = textLines then raise (new ArgumentNullException("textLines"))
    // The following line causes unhandled excepiton on a background thread, see https://github.com/dotnet/fsharp/issues/2318#issuecomment-279340343
    // It seems we should provide a Quick Info Provider at the same time as uncommenting it.
    
    //do textLines.ChangeContentType(contentTypeRegistry.GetContentType Guids.fsiContentTypeName, Guid Guids.guidFsiLanguageService)
    
    let mutable readonlyRegion  = null : IReadOnlyRegion

    let extendReadOnlyRegion position =
            use readonlyEdit = textLines.CreateReadOnlyRegionEdit()
            match readonlyRegion with
            |   null -> ()
            |   _ -> readonlyEdit.RemoveReadOnlyRegion(readonlyRegion)
            readonlyRegion <- readonlyEdit.CreateReadOnlyRegion(Span(0, position))
            readonlyEdit.Apply() |> ignore
        
            
    let appendReadOnlyText (text:string) = 
        let snapshot = textLines.CurrentSnapshot
        let insertionPosition =
            match readonlyRegion with
            |   null -> 0
            |   _ -> readonlyRegion.Span.GetEndPoint(snapshot).Position

        do 
            use edit = textLines.CreateEdit()
            edit.Insert(insertionPosition, text) |> ignore
            edit.Apply() |> ignore
        extendReadOnlyRegion (insertionPosition + text.Length)
        ()

    member this.ReadOnlyMarkerSpan 
        with get() = 
            match readonlyRegion with
            |   null -> TextSpan()
            |   _ ->
                    let snapshot = textLines.CurrentSnapshot
                    let endpoint = readonlyRegion.Span.GetEndPoint(snapshot)
                    let line = endpoint.GetContainingLine()
                    TextSpan(iStartLine = 0, iStartIndex = 0, iEndLine = line.LineNumber, iEndIndex = line.Start.Difference(endpoint))
        
    member this.ExtendReadOnlyMarker() = 
        extendReadOnlyRegion textLines.CurrentSnapshot.Length

    member this.ResetReadOnlyMarker() = 
        match readonlyRegion with
        |   null -> ()
        |   _ ->
                use edit = textLines.CreateReadOnlyRegionEdit()
                edit.RemoveReadOnlyRegion(readonlyRegion)
                readonlyRegion <- null
                edit.Apply() |> ignore

    member this.DirectWrite(text:string)     = appendReadOnlyText text
    member this.DirectWriteLine(text:string) = appendReadOnlyText (text + Environment.NewLine)
    member this.DirectWriteLine()            = appendReadOnlyText (Environment.NewLine)
