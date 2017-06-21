// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.FSharp.Compiler.SourceCodeServices

[<Shared>]
[<ExportLanguageService(typeof<ISynchronousIndentationService>, FSharpConstants.FSharpLanguageName)>]
type internal FSharpIndentationService
    [<ImportingConstructor>]
    (projectInfoManager: ProjectInfoManager) =

    static member GetDesiredIndentation(documentId: DocumentId, sourceText: SourceText, filePath: string, lineNumber: int, tabSize: int, optionsOpt: FSharpProjectOptions option): Option<int> =
        // Match indentation with previous line
        let rec tryFindPreviousNonEmptyLine l =
            if l <= 0 then None
            else
                let previousLine = sourceText.Lines.[l - 1]
                if not (String.IsNullOrEmpty(previousLine.ToString())) then
                    Some previousLine
                else
                    tryFindPreviousNonEmptyLine (l - 1)

        let rec tryFindLastNoneWhitespaceOrCommentToken (line: TextLine) = maybe {
           let! options = optionsOpt
           let defines = CompilerEnvironment.GetCompilationDefinesForEditing(filePath, options.OtherOptions |> Seq.toList)
           let tokens = Tokenizer.tokenizeLine(documentId, sourceText, line.Start, filePath, defines)

           return!
               tokens
               |> List.rev
               |> List.tryFind (fun x ->
                   x.Tag <> FSharpTokenTag.WHITESPACE && x.Tag <> FSharpTokenTag.COMMENT && x.Tag <> FSharpTokenTag.LINE_COMMENT)
        }

        let (|Eq|_|) y x =
            if x = y then Some()
            else None

        let (|NeedIndent|_|) (token: FSharpTokenInfo) =
            match token.Tag with
            | Eq FSharpTokenTag.EQUALS
            | Eq FSharpTokenTag.LARROW
            | Eq FSharpTokenTag.RARROW
            | Eq FSharpTokenTag.LPAREN
            | Eq FSharpTokenTag.LBRACK
            | Eq FSharpTokenTag.LBRACK_BAR
            | Eq FSharpTokenTag.LBRACK_LESS
            | Eq FSharpTokenTag.LBRACE
            | Eq FSharpTokenTag.BEGIN
            | Eq FSharpTokenTag.DO
            | Eq FSharpTokenTag.FUNCTION
            | Eq FSharpTokenTag.THEN
            | Eq FSharpTokenTag.ELSE
            | Eq FSharpTokenTag.STRUCT
            | Eq FSharpTokenTag.CLASS
            | Eq FSharpTokenTag.TRY -> Some ()
            | _ -> None

        maybe {
            let! previousLine = tryFindPreviousNonEmptyLine lineNumber

            let rec loop column spaces =
                if previousLine.Start + column >= previousLine.End then
                    spaces
                else
                    match previousLine.Text.[previousLine.Start + column] with
                    | ' ' -> loop (column + 1) (spaces + 1)
                    | '\t' -> loop (column + 1) (((spaces / tabSize) + 1) * tabSize)
                    | _ -> spaces

            let lastIndent = loop 0 0

            let lastToken = tryFindLastNoneWhitespaceOrCommentToken previousLine
            return
                match lastToken with
                | Some(NeedIndent) -> (lastIndent/tabSize + 1) * tabSize
                | _ -> lastIndent
        }

    interface ISynchronousIndentationService with
        member this.GetDesiredIndentation(document: Document, lineNumber: int, cancellationToken: CancellationToken): Nullable<IndentationResult> =
            async {
                let! cancellationToken = Async.CancellationToken
                let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                let! options = document.GetOptionsAsync(cancellationToken) |> Async.AwaitTask
                let tabSize = options.GetOption(FormattingOptions.TabSize, FSharpConstants.FSharpLanguageName)
                let projectOptionsOpt = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
                let indent = FSharpIndentationService.GetDesiredIndentation(document.Id, sourceText, document.FilePath, lineNumber, tabSize, projectOptionsOpt)
                return
                    match indent with
                    | None -> Nullable()
                    | Some(indentation) -> Nullable<IndentationResult>(IndentationResult(sourceText.Lines.[lineNumber].Start, indentation))
            } |> (fun c -> Async.RunSynchronously(c,cancellationToken=cancellationToken))
