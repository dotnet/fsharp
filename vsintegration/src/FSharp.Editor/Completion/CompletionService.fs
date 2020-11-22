// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
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

    /// Indicates the text span to be replaced by a committed completion list item.
    override _.GetDefaultCompletionListSpan(sourceText, caretIndex) =

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

            let documentId = workspace.GetDocumentIdInCurrentContext(sourceText.Container)
            let document = workspace.CurrentSolution.GetDocument(documentId)
            let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)
            let classifiedSpans = Tokenizer.getClassifiedSpans(documentId, sourceText, line.Span, Some document.FilePath, defines, CancellationToken.None)

            let isBacktickIdentifier (classifiedSpan: ClassifiedSpan) =
                classifiedSpan.ClassificationType = ClassificationTypeNames.Identifier
                    && sourceText.ToString(classifiedSpan.TextSpan).StartsWith("``")
                    && sourceText.ToString(classifiedSpan.TextSpan).EndsWith("``")
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
                    // Trailing unclosed backtick-pair found before caret, use span from backtick to end of line.
                    let lastSpan = classifiedSpans.[classifiedSpans.Count - 1]
                    TextSpan.FromBounds(unclosedBacktick.TextSpan.Start, lastSpan.TextSpan.End)
                | _ ->
                    // No backticks involved at caret position, fall back to standard identifier chars.
                    getIdentifierChars()

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


