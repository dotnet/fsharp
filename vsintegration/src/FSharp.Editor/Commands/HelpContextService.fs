// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading.Tasks
open System.Collections.Generic
open System.Composition
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Classification
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.LanguageServices.Implementation.F1Help
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.FSharp.Compiler.Range

[<Shared>]
[<ExportLanguageService(typeof<IHelpContextService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpHelpContextService 
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =

    static member GetHelpTerm(checker: FSharpChecker, sourceText : SourceText, fileName, options, span: TextSpan, tokens: List<ClassifiedSpan>, textVersion) = async {
        let! _parse,check = checker.ParseAndCheckFileInProject(fileName, textVersion, sourceText.ToString(), options)
        match check with
        | FSharpCheckFileAnswer.Aborted -> return None
        | FSharpCheckFileAnswer.Succeeded(check) ->
            let textLines = sourceText.Lines
            let lineInfo = textLines.GetLineFromPosition(span.Start)
            let line = lineInfo.LineNumber
            let lineText = lineInfo.ToString()

            let caretColumn = textLines.GetLinePosition(span.Start).Character

            let shouldTryToFindSurroundingIdent (token : ClassifiedSpan) =
                let span = token.TextSpan
                let content = sourceText.ToString().Substring(span.Start, span.End - span.Start)
                match token.ClassificationType with
                | ClassificationTypeNames.Text
                | ClassificationTypeNames.WhiteSpace -> true
                | ClassificationTypeNames.Operator when
                       content = "." -> true
                | _ -> false
          
            let tokenInformation, col =
                let col = 
                    if caretColumn = lineText.Length && caretColumn > 0 then
                        // if we are at the end of the line, we always step back one character
                        caretColumn - 1
                    else
                        caretColumn

                let getTokenAt line col = 
                    if col < 0 || line < 0 then None else
                    let start = textLines.[line].Start + col
                    let span = TextSpan.FromBounds(start, start + 1)
                    tokens 
                    |> Seq.tryFindIndex(fun t -> t.TextSpan.Contains(span))
                    |> Option.map (fun i -> tokens.[i])
          
                match getTokenAt line col with
                | Some t as original -> // when col > 0 && shouldTryToFindSurroundingIdent t ->
                    if shouldTryToFindSurroundingIdent t then
                        match getTokenAt line (col - 1) with
                        | Some t as newInfo when not (shouldTryToFindSurroundingIdent t) -> newInfo, col - 1
                        | _ -> 
                            match getTokenAt line (col + 1) with
                            | Some t as newInfo when not (shouldTryToFindSurroundingIdent t) -> newInfo, col + 1
                            | _ -> original, col
                    else original, col
                | otherwise -> otherwise, col

            match tokenInformation with
            |   None -> return None
            |   Some token ->
                    match token.ClassificationType with
                    |   ClassificationTypeNames.Keyword
                    |   ClassificationTypeNames.Operator
                    |   ClassificationTypeNames.PreprocessorKeyword ->
                            return Some (sourceText.GetSubText(token.TextSpan).ToString() + "_FS")
                    |   ClassificationTypeNames.Comment -> return Some "comment_FS"
                    |   ClassificationTypeNames.Identifier ->
                            try
                                let possibleIdentifier = QuickParse.GetCompleteIdentifierIsland false lineText col
                                match possibleIdentifier with
                                |   None -> return None
                                |   Some(s,colAtEndOfNames, _) ->
                                        if check.HasFullTypeCheckInfo then 
                                            let qualId = PrettyNaming.GetLongNameFromString s
                                            return! check.GetF1KeywordAlternate(Line.fromZ line, colAtEndOfNames, lineText, qualId)
                                        else 
                                            return None
                            with e ->
                                Assert.Exception e
                                return None
                    | _ -> return None
    }

    interface IHelpContextService with
        member this.Language = "fsharp"
        member this.Product = "fsharp"

        member this.GetHelpTermAsync(document, textSpan, cancellationToken) = 
            async {
                match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document) with 
                | Some options ->
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                    let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)  
                    let textLine = sourceText.Lines.GetLineFromPosition(textSpan.Start)
                    let tokens = CommonHelpers.getColorizationData(document.Id, sourceText, textLine.Span, Some document.Name, defines, cancellationToken)
                    let! keyword = FSharpHelpContextService.GetHelpTerm(checkerProvider.Checker, sourceText, document.FilePath, options, textSpan, tokens, textVersion.GetHashCode())
                    return defaultArg keyword String.Empty
                | None -> return String.Empty
            } |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

        member this.FormatSymbol(_symbol) = Unchecked.defaultof<_>
        
