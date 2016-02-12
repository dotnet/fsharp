// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.TextManager
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Utilities

module TextHelper =
    /// Returns the column of the first non-whitespace character on the specified line.
    /// If line is all whitespace, returns the column matching the end of the line.
    let physicalIndentColumnOfLine (textView : ITextView) (line : ITextSnapshotLine) =
        let tabSize = textView.Options.GetOptionValue<int>(DefaultOptions.TabSizeOptionId)
        let rec loop column i =
            if i > line.End.Position then
                column
            else
                match line.Snapshot.[i] with
                | ' '  -> loop (column + 1) (i + 1)
                | '\t' -> loop (((column / tabSize) + 1) * tabSize) (i + 1)
                | _    -> column

        loop 0 line.Start.Position

    /// Returns a string of whitespace matching the physical indentation of the specified line.
    let physicalIndentStringOfLine (textView : ITextView) (line : ITextSnapshotLine) =        
        let indentColumn = physicalIndentColumnOfLine textView line
        String(Array.create indentColumn ' ')

    /// Returns a string of whitespace matching a single indentation.
    let singleIndentString (textView : ITextView) =  
        let tabSize = textView.Options.GetOptionValue<int>(DefaultOptions.TabSizeOptionId)
        String(Array.create tabSize ' ')

/// F# smart indent implementation
type SmartIndent (textView : ITextView, textManager : IVsTextManager) =
    let mutable _textView = textView
    
    interface ISmartIndent with

        /// returns the desired indentation for the new line
        /// or null when no indentation is desired or unable to determine indentation
        member this.GetDesiredIndentation(line : ITextSnapshotLine) =

            let lp = [| LANGPREFERENCES(guidLang = Guid(FSharpCommonConstants.languageServiceGuidString)) |]
            let indentStyle = if ErrorHandler.Succeeded(textManager.GetUserPreferences(null, null, lp, null)) then lp.[0].IndentStyle else vsIndentStyle.vsIndentStyleDefault

            if (indentStyle = vsIndentStyle.vsIndentStyleNone || _textView = null || _textView.IsClosed) then
                Nullable<int>()
            else

            match line.LineNumber - 1 with
            | lineNum when lineNum < 0 ->
                Nullable<int>()
            | lineNum ->
                let previousLine = line.Snapshot.GetLineFromLineNumber(lineNum)        
                let caretIsOnCurrentLine = _textView.Caret.ContainingTextViewLine.ContainsBufferPosition(line.Start)

                match (caretIsOnCurrentLine, previousLine.Length = 0) with                
                | (true, false) ->
                    // if user hit 'return' from non-empty line || clicked on same line as caret, which is preceded by non-empty line
                    //     match physical indentation of non-empty line
                    Nullable<int>(TextHelper.physicalIndentColumnOfLine _textView previousLine)
                | (true, true) ->
                    // if user hit 'return' from empty line || clicked on same line as caret, which is preceded by empty line
                    //     keep existing virtual indentation
                    Nullable<int>(_textView.Caret.Position.VirtualSpaces)
                | (false, _) ->
                    // if user has clicked on an empty line, NOT the same line as the caret is current on
                    //     scan backward until we find a non-empty line, and match its physical indentation
                    let rec loop lineNumber =
                        if lineNumber < 0 then
                            Nullable<int>()
                        else
                            let earlierLine = line.Snapshot.GetLineFromLineNumber(lineNumber)
                            if earlierLine.Length > 0 then                            
                                Nullable<int>(TextHelper.physicalIndentColumnOfLine _textView earlierLine)
                            else
                                loop (lineNumber - 1)

                    loop lineNum
          
    interface IDisposable with
        member this.Dispose() =
            _textView <- null

[<Export(typeof<ISmartIndentProvider>)>]
[<ContentType("F#")>]
type SmartIndentProvider [<ImportingConstructor>](serviceProvider : SVsServiceProvider) =
    interface ISmartIndentProvider with

        member this.CreateSmartIndent(textView : ITextView) =            
            if textView = null then raise (ArgumentNullException("textView")) else

            let textManager = serviceProvider.GetService(typeof<SVsTextManager>) :?> IVsTextManager
            (new SmartIndent(textView, textManager)) :> ISmartIndent