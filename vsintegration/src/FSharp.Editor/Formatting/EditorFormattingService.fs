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

[<Shared>]
[<ExportLanguageService(typeof<IEditorFormattingService>, FSharpConstants.FSharpLanguageName)>]
type internal FSharpEditorFormattingService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =

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

    member __.GetFormattingChangesAsync (document: Document, position: int, cancellationToken: CancellationToken) =
        async {
            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
            let! options = document.GetOptionsAsync(cancellationToken) |> Async.AwaitTask
            let indentStyle = options.GetOption(FormattingOptions.SmartIndent, FSharpConstants.FSharpLanguageName)
            let projectOptionsOpt = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let! textChange = FSharpEditorFormattingService.GetFormattingChanges(document.Id, sourceText, document.FilePath, checkerProvider.Checker, indentStyle, projectOptionsOpt, position)
                
            return
                match textChange with
                | Some change ->
                    ResizeArray([change]) :> IList<_>
                
                | None ->
                    ResizeArray() :> IList<_>
        }
        
    interface IEditorFormattingService with
        member val SupportsFormatDocument = false
        member val SupportsFormatSelection = false
        member val SupportsFormatOnPaste = false
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

        override __.GetFormattingChangesOnPasteAsync (_document, _span, cancellationToken) =
            async { return ResizeArray() :> IList<_> }
            |> RoslynHelpers.StartAsyncAsTask cancellationToken

        override this.GetFormattingChangesAsync (document, _typedChar, position, cancellationToken) =
            this.GetFormattingChangesAsync (document, position, cancellationToken)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken

        override this.GetFormattingChangesOnReturnAsync (document, position, cancellationToken) =
            this.GetFormattingChangesAsync (document, position, cancellationToken)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
