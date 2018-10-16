// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Generic

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.FSharp.Compiler.SourceCodeServices
open System.Threading
open System.Windows.Forms

[<Shared>]
[<ExportLanguageService(typeof<IEditorFormattingService>, FSharpConstants.FSharpLanguageName)>]
type internal FSharpEditorFormattingService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager,
        settings: EditorOptions
    ) =
    
    static let toIList (xs : 'a seq) = ResizeArray(xs) :> IList<'a>
    
    static let getIndentation (line : string) = line |> Seq.takeWhile ((=) ' ') |> Seq.length

    static member GetFormattingChanges(documentId: DocumentId, sourceText: SourceText, filePath: string, checker: FSharpChecker, indentStyle: FormattingOptions.IndentStyle, options: (FSharpParsingOptions * FSharpProjectOptions) option, position: int) =
        // Logic for determining formatting changes:
        // If first token on the current line is a closing brace,
        // match the indent with the indent on the line that opened it

        asyncMaybe {
            
            // Gate formatting on whether smart indentation is enabled
            // (this is what C# does)
            do! Option.guard (indentStyle = FormattingOptions.IndentStyle.Smart)

            let! parsingOptions, _projectOptions = options
            
            let line = sourceText.Lines.[sourceText.Lines.IndexOf position]
                
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions

            let tokens = Tokenizer.tokenizeLine(documentId, sourceText, line.Start, filePath, defines)

            let! firstMeaningfulToken = 
                tokens
                |> Array.tryFind (fun x ->
                    x.Tag <> FSharpTokenTag.WHITESPACE &&
                    x.Tag <> FSharpTokenTag.COMMENT &&
                    x.Tag <> FSharpTokenTag.LINE_COMMENT)

            let! (left, right) =
                FSharpBraceMatchingService.GetBraceMatchingResult(checker, sourceText, filePath, parsingOptions, position, "FormattingService", forFormatting=true)

            if right.StartColumn = firstMeaningfulToken.LeftColumn then
                // Replace the indentation on this line with the indentation of the left bracket
                let! leftSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, left)

                let indentChars (line : TextLine) =
                    line.ToString()
                    |> Seq.takeWhile ((=) ' ')
                    |> Seq.length
                    
                let startIndent = indentChars sourceText.Lines.[sourceText.Lines.IndexOf leftSpan.Start]
                let currentIndent = indentChars line

                return TextChange(TextSpan(line.Start, currentIndent), String.replicate startIndent " ")
            else
                return! None
        }
        
    static member GetPasteChanges(documentId: DocumentId, sourceText: SourceText, filePath: string, formattingOptions: Microsoft.VisualStudio.FSharp.Editor.FormattingOptions, tabSize: int, parsingOptions: FSharpParsingOptions, currentClipboard: string, span: TextSpan) =
        asyncMaybe {
            
            do! Option.guard formattingOptions.FormatOnPaste

            let startLineIdx = sourceText.Lines.IndexOf span.Start
            
            // If we're starting and ending on the same line, we've got nothing to format
            do! Option.guard (startLineIdx <> sourceText.Lines.IndexOf span.End)
            
            let startLine = sourceText.Lines.[startLineIdx]

            // VS quirk: if we're pasting on an empty line which has automatically been
            // indented (i.e. by ISynchronousIndentationService), then the pasted span
            // includes this automatic indentation. When pasting, we only care about what
            // was actually in the clipboard.
            let fixedSpan =
                let pasteText = sourceText.GetSubText(span)
                let pasteTextString = pasteText.ToString()

                if currentClipboard.Length > 0 && pasteTextString.EndsWith currentClipboard then
                    let prepended = pasteTextString.[0..pasteTextString.Length-currentClipboard.Length-1]
                    
                    // Only strip off leading indentation if the pasted span is otherwise
                    // identical to the clipboard (ignoring leading spaces).
                    if prepended |> Seq.forall ((=) ' ') then
                        TextSpan(span.Start + prepended.Length, span.Length - prepended.Length)
                    else
                        span
                else
                    span

            // Calculate the indentation of the line we pasted onto
            let currentIndent =
                let priorStartSpan = TextSpan(startLine.Span.Start, startLine.Span.Length - (startLine.Span.End - fixedSpan.Start))
                
                sourceText.GetSubText(priorStartSpan).ToString()
                |> Seq.takeWhile ((=) ' ')
                |> Seq.length

            let fixedPasteText = sourceText.GetSubText(fixedSpan)
            let leadingIndentation = fixedPasteText.ToString() |> getIndentation
                
            let stripIndentation charsToRemove =
                let searchIndent = String.replicate charsToRemove " "
                let newText = String.replicate currentIndent " "
            
                fixedPasteText.Lines
                |> Seq.indexed
                |> Seq.choose (fun (i, line) ->
                    if line.ToString().StartsWith searchIndent then
                        TextChange(TextSpan(line.Start + fixedSpan.Start, charsToRemove), if i = 0 then "" else newText)
                        |> Some
                    else
                        None
                )
            
            if leadingIndentation > 0 then
                return stripIndentation leadingIndentation
            else
                let nextLineShouldBeIndented = FSharpIndentationService.IndentShouldFollow(documentId, sourceText, filePath, span.Start, parsingOptions)
                
                let removeIndentation =
                    let nextLineIndent = fixedPasteText.Lines.[1].ToString() |> getIndentation

                    if nextLineShouldBeIndented then nextLineIndent - tabSize
                    else nextLineIndent
                
                return stripIndentation removeIndentation
        }

    member __.GetFormattingChangesAsync (document: Document, position: int, cancellationToken: CancellationToken) =
        async {
            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
            let! options = document.GetOptionsAsync(cancellationToken) |> Async.AwaitTask
            let indentStyle = options.GetOption(FormattingOptions.SmartIndent, FSharpConstants.FSharpLanguageName)
            let projectOptionsOpt = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let! textChange = FSharpEditorFormattingService.GetFormattingChanges(document.Id, sourceText, document.FilePath, checkerProvider.Checker, indentStyle, projectOptionsOpt, position)
            return textChange |> Option.toList |> toIList
        }
        
    member __.OnPasteAsync (document: Document, span: TextSpan, currentClipboard: string, cancellationToken: CancellationToken) =
        async {
            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
            let! options = document.GetOptionsAsync(cancellationToken) |> Async.AwaitTask
            let tabSize = options.GetOption<int>(FormattingOptions.TabSize, FSharpConstants.FSharpLanguageName)
            
            match projectInfoManager.TryGetOptionsForEditingDocumentOrProject document with
            | Some (parsingOptions, _) ->
                let! textChanges = FSharpEditorFormattingService.GetPasteChanges(document.Id, sourceText, document.FilePath, settings.Formatting, tabSize, parsingOptions, currentClipboard, span)
                return textChanges |> Option.defaultValue Seq.empty |> toIList
            | None ->
                return toIList Seq.empty
        }
        
    interface IEditorFormattingService with
        member val SupportsFormatDocument = false
        member val SupportsFormatSelection = false
        member val SupportsFormatOnPaste = true
        member val SupportsFormatOnReturn = true

        override __.SupportsFormattingOnTypedCharacter (document, ch) =
            if FSharpIndentationService.IsSmartIndentEnabled document.Project.Solution.Workspace.Options then
                match ch with
                | ')' | ']' | '}' -> true
                | _ -> false
            else
                false

        override __.GetFormattingChangesAsync (_document, _span, cancellationToken) =
            async { return ResizeArray() :> IList<_> }
            |> RoslynHelpers.StartAsyncAsTask cancellationToken

        override this.GetFormattingChangesOnPasteAsync (document, span, cancellationToken) =
            let currentClipboard = Clipboard.GetText()

            this.OnPasteAsync (document, span, currentClipboard, cancellationToken)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken

        override this.GetFormattingChangesAsync (document, _typedChar, position, cancellationToken) =
            this.GetFormattingChangesAsync (document, position, cancellationToken)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken

        override this.GetFormattingChangesOnReturnAsync (document, position, cancellationToken) =
            this.GetFormattingChangesAsync (document, position, cancellationToken)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
