// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Completion
open System.Globalization
open Microsoft.FSharp.Compiler.SourceCodeServices

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
        CommonCompletionUtilities.IsStartingNewWord(sourceText, position, (fun ch -> isIdentifierStartCharacter ch), (fun ch -> isIdentifierPartCharacter ch))

    let shouldProvideCompletion (documentId: DocumentId, filePath: string, defines: string list, sourceText: SourceText, triggerPosition: int) : bool =
        let textLines = sourceText.Lines
        let triggerLine = textLines.GetLineFromPosition triggerPosition
        let classifiedSpans = Tokenizer.getClassifiedSpans(documentId, sourceText, triggerLine.Span, Some filePath, defines, CancellationToken.None)
        classifiedSpans.Count = 0 || // we should provide completion at the start of empty line, where there are no tokens at all
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