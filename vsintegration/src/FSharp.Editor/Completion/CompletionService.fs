// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Completion
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.Shell

open FSharp.Compiler.PrettyNaming

type internal FSharpCompletionService
    (
        workspace: Workspace,
        serviceProvider: SVsServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager,
        assemblyContentProvider: AssemblyContentProvider,
        settings: EditorOptions
    ) =
    inherit CompletionServiceWithProviders(workspace)

    let builtInProviders = 
        ImmutableArray.Create<CompletionProvider>(
            FSharpCompletionProvider(workspace, serviceProvider, checkerProvider, projectInfoManager, assemblyContentProvider),
            FSharpCommonCompletionProvider.Create(HashDirectiveCompletionProvider.Create(workspace, projectInfoManager)))

    override _.Language = FSharpConstants.FSharpLanguageName
    override _.GetBuiltInProviders() = builtInProviders
    override _.GetRules() =
        let enterKeyRule =
            match settings.IntelliSense.EnterKeySetting with
            | NeverNewline -> EnterKeyRule.Never
            | NewlineOnCompleteWord -> EnterKeyRule.AfterFullyTypedWord
            | AlwaysNewline -> EnterKeyRule.Always

        CompletionRules.Default
            .WithDismissIfEmpty(true)
            .WithDismissIfLastCharacterDeleted(true)
            .WithDefaultEnterKeyRule(enterKeyRule)

    override _.GetDefaultCompletionListSpan( sourceText , caretIndex ) =

        let mutable startIndex = 0
        let mutable endIndex = 0

        // Get single line text and index
        let textLines = sourceText.Lines
        let lineText = textLines.GetLineFromPosition(caretIndex).ToString()
        let lineCaretIndex = textLines.GetLinePosition(caretIndex).Character

        // Check for enclosing backticks or leading backticks, else capture valid identifier characters
        // TODO Replace naive backtick check with safer alternative
        match lineText.IndexOf "``", lineText.LastIndexOf "``" with
        | startTickIndex, endTickIndex when startTickIndex > -1 && endTickIndex > -1 && startTickIndex <> endTickIndex && lineCaretIndex >= startTickIndex && lineCaretIndex <= endTickIndex + 2 ->
            // Cursor is at or between a pair of double ticks, select enclosed range including ticks as identifier
            startIndex <- startTickIndex
            endIndex <- endTickIndex + 2
        | startTickIndex, endTickIndex when startTickIndex > -1 && endTickIndex > -1 && startTickIndex = endTickIndex && lineCaretIndex >= startTickIndex ->
            // Cursor is at or after double ticks with none following, select ticks and all following text as identifier
            startIndex <- startTickIndex
            endIndex <- lineText.Length
        | _ ->
            // No ticks, capture identifier-part chars backward and forward from cursor as identifier
            startIndex <- lineCaretIndex
            while startIndex > 0 && IsIdentifierPartCharacter lineText.[startIndex - 1] do startIndex <- startIndex - 1
            endIndex <- lineCaretIndex
            if startIndex <> lineCaretIndex then
                while endIndex < lineText.Length && IsIdentifierPartCharacter lineText.[endIndex] do endIndex <- endIndex + 1

        // Translate line index back to document index
        startIndex <- caretIndex - (lineCaretIndex - startIndex)
        endIndex <- caretIndex + (endIndex - lineCaretIndex)

        TextSpan.FromBounds(startIndex, endIndex)

[<Shared>]
[<ExportLanguageServiceFactory(typeof<CompletionService>, FSharpConstants.FSharpLanguageName)>]
type internal FSharpCompletionServiceFactory 
    [<ImportingConstructor>] 
    (
        serviceProvider: SVsServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager,
        assemblyContentProvider: AssemblyContentProvider,
        settings: EditorOptions
    ) =
    interface ILanguageServiceFactory with
        member _.CreateLanguageService(hostLanguageServices: HostLanguageServices) : ILanguageService =
            upcast new FSharpCompletionService(hostLanguageServices.WorkspaceServices.Workspace, serviceProvider, checkerProvider, projectInfoManager, assemblyContentProvider, settings)


