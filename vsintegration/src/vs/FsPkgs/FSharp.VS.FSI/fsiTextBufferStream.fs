// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.IO
open System.Diagnostics
open System.Globalization
open System.Windows.Forms
open System.Runtime.InteropServices
open System.ComponentModel.Design
open Microsoft.Win32
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.TextManager.Interop
open Util
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor

// This type wraps the IVsTextLines which contains the FSI session (output and input).
// It provides the API for writing directly to the read-only part of the buffer.
// It extends the read-only marker on the buffer (making the written text read-only).
//
type internal TextBufferStream(textLines:ITextBuffer) = 
    do if null = textLines then raise (new ArgumentNullException("textLines"))

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
