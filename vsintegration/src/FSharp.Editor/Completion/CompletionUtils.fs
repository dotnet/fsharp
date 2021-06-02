// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Completion
open System.Globalization
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax.PrettyNaming

module internal CompletionUtils =

    let private isLetterChar (cat: UnicodeCategory) =
        // letter-character:
        //   A Unicode character of classes Lu, Ll, Lt, Lm, Lo, or Nl 
        //   A Unicode-escape-sequence representing a character of classes Lu, Ll, Lt, Lm, Lo, or Nl

        match cat with
        | UnicodeCategory.UppercaseLetter
        | UnicodeCategory.LowercaseLetter
        | UnicodeCategory.TitlecaseLetter
        | UnicodeCategory.ModifierLetter
        | UnicodeCategory.OtherLetter
        | UnicodeCategory.LetterNumber -> true
        | _ -> false

    /// Defines a set of helper methods to classify Unicode characters.
    let private isIdentifierStartCharacter(ch: char) =
        // identifier-start-character:
        //   letter-character
        //   _ (the underscore character U+005F)

        if ch < 'a' then // '\u0061'
            if ch < 'A' then // '\u0041'
                false
            else ch <= 'Z'   // '\u005A'
                || ch = '_' // '\u005F'

        elif ch <= 'z' then // '\u007A'
            true
        elif ch <= '\u007F' then // max ASCII
            false
        
        else isLetterChar(CharUnicodeInfo.GetUnicodeCategory(ch))
 
        /// Returns true if the Unicode character can be a part of an identifier.
    let private isIdentifierPartCharacter(ch: char) =
        // identifier-part-character:
        //   letter-character
        //   decimal-digit-character
        //   connecting-character
        //   combining-character
        //   formatting-character

        if ch < 'a' then // '\u0061'
            if ch < 'A' then // '\u0041'
                ch >= '0'  // '\u0030'
                && ch <= '9' // '\u0039'
            else
                ch <= 'Z'  // '\u005A'
                || ch = '_' // '\u005F'
        elif ch <= 'z' then // '\u007A'
            true
        elif ch <= '\u007F' then // max ASCII
            false

        else
            let cat = CharUnicodeInfo.GetUnicodeCategory(ch)
            isLetterChar(cat)
            ||
            match cat with
            | UnicodeCategory.DecimalDigitNumber
            | UnicodeCategory.ConnectorPunctuation
            | UnicodeCategory.NonSpacingMark
            | UnicodeCategory.SpacingCombiningMark -> true
            | _ when int ch > 127 ->
                CharUnicodeInfo.GetUnicodeCategory(ch) = UnicodeCategory.Format
            | _ -> false

    let isStartingNewWord (sourceText, position) =
        FSharpCommonCompletionUtilities.IsStartingNewWord(sourceText, position, (fun ch -> isIdentifierStartCharacter ch), (fun ch -> isIdentifierPartCharacter ch))

    let shouldProvideCompletion (documentId: DocumentId, filePath: string, defines: string list, sourceText: SourceText, triggerPosition: int) : bool =
        let textLines = sourceText.Lines
        let triggerLine = textLines.GetLineFromPosition triggerPosition
        let classifiedSpans = Tokenizer.getClassifiedSpans(documentId, sourceText, triggerLine.Span, Some filePath, defines, CancellationToken.None)
        classifiedSpans.Count = 0 || // we should provide completion at the start of empty line, where there are no tokens at all
        let result =
          classifiedSpans.Exists (fun classifiedSpan -> 
            classifiedSpan.TextSpan.IntersectsWith triggerPosition &&
            (
                match classifiedSpan.ClassificationType with
                | ClassificationTypeNames.Comment
                | ClassificationTypeNames.StringLiteral
                | ClassificationTypeNames.ExcludedCode
                | ClassificationTypeNames.Operator
                | ClassificationTypeNames.NumericLiteral -> false
                | _ -> true // anything else is a valid classification type
            ))
        result

    let inline getKindPriority kind =
        match kind with
        | CompletionItemKind.CustomOperation -> 0
        | CompletionItemKind.Property -> 1
        | CompletionItemKind.Field -> 2
        | CompletionItemKind.Method (isExtension = false) -> 3
        | CompletionItemKind.Event -> 4
        | CompletionItemKind.Argument -> 5
        | CompletionItemKind.Other -> 6
        | CompletionItemKind.Method (isExtension = true) -> 7

    /// Indicates the text span to be replaced by a committed completion list item.
    let getDefaultCompletionListSpan(sourceText: SourceText, caretIndex, documentId, filePath, defines) =

        // Gets connected identifier-part characters backward and forward from caret.
        let getIdentifierChars() =
            let mutable startIndex = caretIndex
            let mutable endIndex = caretIndex
            while startIndex > 0 && IsIdentifierPartCharacter sourceText.[startIndex - 1] do startIndex <- startIndex - 1
            if startIndex <> caretIndex then
                while endIndex < sourceText.Length && IsIdentifierPartCharacter sourceText.[endIndex] do endIndex <- endIndex + 1
            TextSpan.FromBounds(startIndex, endIndex)

        let line = sourceText.Lines.GetLineFromPosition(caretIndex)
        if line.ToString().IndexOf "``" < 0 then
            // No backticks on the line, capture standard identifier chars.
            getIdentifierChars()
        else
            // Line contains backticks.
            // Use tokenizer to check for identifier, in order to correctly handle extraneous backticks in comments, strings, etc.

            // If caret is at a backtick-identifier, then that is our span.

            // Else, check if we are after an unclosed ``, to support the common case of a manually typed leading ``.
            // Tokenizer will not consider this an identifier, it will consider the bare `` a Keyword, followed by
            // arbitrary tokens (Identifier, Operator, Text, etc.) depending on the trailing text.

            // Else, backticks are not involved in caret location, fall back to standard identifier character scan.

            // There may still be edge cases where backtick related spans are incorrectly captured, such as unclosed
            // backticks before later valid backticks on a line, this is an acceptable compromise in order to support
            // the majority of common cases.

            let classifiedSpans = Tokenizer.getClassifiedSpans(documentId, sourceText, line.Span, Some filePath, defines, CancellationToken.None)

            let isBacktickIdentifier (classifiedSpan: ClassifiedSpan) =
                classifiedSpan.ClassificationType = ClassificationTypeNames.Identifier
                    && Tokenizer.isDoubleBacktickIdent (sourceText.ToString(classifiedSpan.TextSpan))
            let isUnclosedBacktick (classifiedSpan: ClassifiedSpan) =
                classifiedSpan.ClassificationType = ClassificationTypeNames.Keyword
                    && sourceText.ToString(classifiedSpan.TextSpan) = "``"

            match classifiedSpans |> Seq.tryFind (fun cs -> isBacktickIdentifier cs && cs.TextSpan.IntersectsWith caretIndex) with
            | Some backtickIdentifier ->
                // Backtick enclosed identifier found intersecting with caret, use its span.
                backtickIdentifier.TextSpan
            | _ ->
                match classifiedSpans |> Seq.tryFindBack (fun cs -> isUnclosedBacktick cs && caretIndex >= cs.TextSpan.Start) with
                | Some unclosedBacktick ->
                    // Unclosed backtick found before caret, use span from backtick to end of line.
                    let lastSpan = classifiedSpans.[classifiedSpans.Count - 1]
                    TextSpan.FromBounds(unclosedBacktick.TextSpan.Start, lastSpan.TextSpan.End)
                | _ ->
                    // No backticks involved at caret position, fall back to standard identifier chars.
                    getIdentifierChars()
