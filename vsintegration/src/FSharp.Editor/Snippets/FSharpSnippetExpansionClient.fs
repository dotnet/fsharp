// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Runtime.InteropServices
open System.Threading
open System.Xml.Linq

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.LanguageServices.Implementation.Snippets
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.TextManager.Interop
open MSXML

type FSharpSnippetExpansionClient
    (
        textView: ITextView,
        editorAdaptersFactoryService: IVsEditorAdaptersFactoryService
    ) =
    inherit AbstractSnippetExpansionClient(Guid(FSharpConstants.languageServiceGuidString), textView, textView.TextBuffer, editorAdaptersFactoryService)
        override __.GetExpansionFunction(_xmlFunctionNode: IXMLDOMNode, _bstrFieldName: string, pfunc: byref<IVsExpansionFunction>): int =
            pfunc <- null
            VSConstants.E_INVALIDARG
        override __.InsertEmptyCommentAndGetEndPositionTrackingSpan(): ITrackingSpan =
            // this method is only called through `AbstractSnippetExpansionClient.FormatSpan` which is manually overridden below
            raise (NotSupportedException())
        override __.AddImports(_document: Document, _position: int, _snippetNode: XElement, _placeSystemNamespaceFirst: bool, _cancellationToken: CancellationToken): Document =
            // this method is only called through `AbstractSnippetExpansionClient.FormatSpan` which is manually overridden below
            raise (NotSupportedException())
    interface IVsExpansionClient with
        // this method is implemented by `AbstractSnippetExpansionClient` and is dependent on Roslyn syntax nodes which F# doesn't expose so we handle it ourselves here
        member __.FormatSpan(pBuffer: IVsTextLines, ts: TextSpan []): int =
            if ts.Length = 1 then
                let ts = ts.[0]
                match pBuffer.GetLineText(ts.iStartLine, ts.iStartIndex, ts.iEndLine, ts.iEndIndex) with
                | (result, text) when result = VSConstants.S_OK ->
                    // split the text at the newlines and ensure each line after the first has the same indention level as the first
                    let lines = text.Split('\n')
                    let indent = String(' ', ts.iStartIndex) // TODO: this doesn't handle virtual spaces
                    for i in 1..lines.Length - 1 do
                        lines.[i] <- indent + lines.[i]
                    let newText = String.Join("\n", lines)
                    let changedSpan: TextSpan array = Array.zeroCreate 1
                    let mutable textPointer = Unchecked.defaultof<nativeint>
                    try
                        // the new text has to traverse through unmanaged memory
                        textPointer <- Marshal.StringToHGlobalUni(newText)
                        match pBuffer.ReplaceLines(ts.iStartLine, ts.iStartIndex, ts.iEndLine, ts.iEndIndex, textPointer, newText.Length, changedSpan) with
                        | result when result = VSConstants.S_OK ->
                            // success replacing text
                            VSConstants.S_OK
                        | _ ->
                            // error replacing text
                            VSConstants.S_OK
                    finally
                        Marshal.FreeHGlobal(textPointer)
                | _ -> VSConstants.S_OK
            else
                VSConstants.E_INVALIDARG
