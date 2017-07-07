// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Generic

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.FSharp.Compiler.SourceCodeServices
open System.Threading

[<Shared>]
[<ExportLanguageService(typeof<IEditorFormattingService>, FSharpConstants.FSharpLanguageName)>]
type internal FSharpEditorFormattingService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =

    static member GetFormattingChanges(document: Document, sourceText: SourceText, checker: FSharpChecker, optionsOpt: FSharpProjectOptions option, position: int) =
        // Logic should be:
        // If first token on the current line is a closing brace,
        // match the indent with the indent on the line that opened it

        asyncMaybe {
            let! options = optionsOpt
            
            let line = sourceText.Lines.[sourceText.Lines.IndexOf position]
                
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.FilePath, options.OtherOptions |> List.ofArray)

            let tokens = Tokenizer.tokenizeLine(document.Id, sourceText, line.Start, document.FilePath, defines)

            let! firstMeaningfulToken = 
                tokens
                |> List.tryFind (fun x ->
                    x.Tag <> FSharpTokenTag.WHITESPACE &&
                    x.Tag <> FSharpTokenTag.COMMENT &&
                    x.Tag <> FSharpTokenTag.LINE_COMMENT)

            let! (left, right) =
                FSharpBraceMatchingService.GetBraceMatchingResult(checker, sourceText, document.FilePath, options, position, "FormattingService")

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

    member __.GetFormattingChangesAsync (document: Document, position: int, cancellationToken: CancellationToken) =
        async {
            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
            let optionsOpt = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let! textChange = FSharpEditorFormattingService.GetFormattingChanges(document, sourceText, checkerProvider.Checker, optionsOpt, position)
                
            return
                match textChange with
                | Some change ->
                    List([change]) :> IList<_>
                | None ->
                    List() :> IList<_>
        }
        
    interface IEditorFormattingService with
        member val SupportsFormatDocument = false
        member val SupportsFormatSelection = false
        member val SupportsFormatOnPaste = false
        member val SupportsFormatOnReturn = true

        override __.SupportsFormattingOnTypedCharacter (_document, ch) =
            match ch with
            | ')' | ']' | '}' -> true
            | _ -> false

        override __.GetFormattingChangesAsync (_document, _span, _cancellationToken) = null

        override __.GetFormattingChangesOnPasteAsync (_document, _span, _cancellationToken) = null

        override this.GetFormattingChangesAsync (document, _typedChar, position, cancellationToken) =
            this.GetFormattingChangesAsync (document, position, cancellationToken)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken

        override this.GetFormattingChangesOnReturnAsync (document, position, cancellationToken) =
            this.GetFormattingChangesAsync (document, position, cancellationToken)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
