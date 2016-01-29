// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.BraceCompletion
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Utilities

/// F# brace completion context implementation
type internal CompletionContext(tokenContext : TokenContext, textManager : IVsTextManager) =
    interface IBraceCompletionContext with
        member this.Start(_) =
            ()

        /// Called when user hits Return while inside of a brace completion session.
        member this.OnReturn(session) =
            
            let lp = [| LANGPREFERENCES(guidLang = Guid(FSharpCommonConstants.languageServiceGuidString)) |]
            ErrorHandler.ThrowOnFailure(textManager.GetUserPreferences(null, null, lp, null)) |> ignore

            // if smart indent is not enabled, or we are in a string, don't do any special formatting
            if (lp.[0].IndentStyle <> vsIndentStyle.vsIndentStyleSmart || session.OpeningBrace = '"') then
                ()
            else
                // within other braces:
                //   leave the opening brace where it is
                //   put the caret one line down, indented once from the previous line
                //   put the closing brace one line below that, justified with the original line
                //   let q = query {
                //        $caret$
                //   }

                let openingBraceLine = session.OpeningPoint.GetPoint(session.SubjectBuffer.CurrentSnapshot).GetContainingLine()
                let existingIndent = TextHelper.physicalIndentStringOfLine session.TextView openingBraceLine
                let caretPosition = session.TextView.Caret.Position.BufferPosition.Position
                let singleIndent = TextHelper.singleIndentString session.TextView

                // we will already have "existing" indentation applied, justifying the caret with the previous line.
                // insert an additional indent, a newline, then another buffer matching "existing" indentation 
                use edit = session.SubjectBuffer.CreateEdit()
                if edit.Insert(caretPosition, String.Format("{0}{1}{2}", singleIndent, Environment.NewLine, existingIndent)) then
                    let newSnapshot = edit.Apply()
                    
                    // if edit failed to apply, snapshots will be the same
                    if newSnapshot = edit.Snapshot then
                        ()
                    else
                        session.TextView.Caret.MoveTo(
                                SnapshotPoint(newSnapshot, caretPosition + singleIndent.Length).TranslateTo(session.TextView.TextSnapshot, PointTrackingMode.Positive)
                            ) |> ignore

        /// Called when user types the (potential) closing brace for a session.
        /// Return false if we don't think user is really completing the session. E.g. escaped \" should not close a string session
        member this.AllowOverType(session) =
            let caretPosition = session.TextView.Caret.Position.BufferPosition.Position
            let line = session.TextView.TextSnapshot.GetLineFromPosition(caretPosition)
            let lineNum = line.LineNumber
            let braceColumn = caretPosition - line.Start.Position

            // test what state we'd be in if the closing brace and a space were typed
            let tokenInfo = tokenContext.GetContextAt(session.TextView, lineNum, braceColumn + 1, (string session.ClosingBrace + " "), braceColumn)

            match tokenInfo.Type with
            | TokenType.Comment
            | TokenType.Unknown
            | TokenType.String -> false
            | _ -> true

        member this.Finish(_) =
            ()

[<Export(typeof<IBraceCompletionContextProvider>)>]
[<BracePair('"', '"')>]
[<BracePair('[', ']')>]
[<BracePair('{', '}')>]
[<BracePair('(', ')')>]
[<ContentType("F#")>]
type BraceCompletionContextProvider [<ImportingConstructor>] (serviceProvider : SVsServiceProvider, adapterService : IVsEditorAdaptersFactoryService) =

    let tokenContext = TokenContext(serviceProvider, adapterService)
    let textManager = serviceProvider.GetService(typeof<SVsTextManager>) :?> IVsTextManager

    interface IBraceCompletionContextProvider with

        /// Called when user types a character matching a supported opening brace
        member this.TryCreateContext(textView : ITextView , snap : SnapshotPoint, openingBrace : char, _ : char,  completionContext : byref<IBraceCompletionContext>) =
            if textView = null then raise (ArgumentException("textView")) else

            let line = snap.GetContainingLine()
            let lineNum = line.LineNumber
            let braceColumn = snap.Position - line.Start.Position

            // check what token context the user is at to decide if we should do brace completion
            let info = tokenContext.GetContextAt(textView, lineNum, braceColumn, " ", braceColumn)
            match info.Type with
            | TokenType.String
            | TokenType.Comment 
            | TokenType.Unknown ->
                // don't initiate brace completion inside of strings, comments, or inactive code
                completionContext <- null
                false
            | _ ->
                // check if we might be in a char literal
                let prevChar =
                    match snap.Position - 1 with
                    | p when p < 0 -> None
                    | p -> Some(snap.Snapshot.[p])
                
                match prevChar with
                | Some('\'') ->
                    // previous character was a ', but could this be a char literal?
                    // test the token context in the case that we added a closing ' after the brace character
                    let (insideCharLit, outsideCharLit) =
                        (tokenContext.GetContextAt(textView, lineNum, braceColumn,     (string openingBrace) + "' ", braceColumn).Type,
                         tokenContext.GetContextAt(textView, lineNum, braceColumn + 2, (string openingBrace) + "' ", braceColumn).Type)

                    match (insideCharLit, outsideCharLit) with
                    | (TokenType.String, t) when t <> TokenType.String ->
                        completionContext <- null
                        false
                    | _ ->
                        completionContext <- CompletionContext(tokenContext, textManager)
                        true
                | _ ->
                    completionContext <- CompletionContext(tokenContext, textManager)
                    true