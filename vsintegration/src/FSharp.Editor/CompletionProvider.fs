// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Linq
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.Debugging
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionIcons

type internal FSharpCompletionProvider(workspace: Workspace, serviceProvider: SVsServiceProvider) =
    inherit CompletionProvider()

    static let completionTriggers = [| '.' |]
    static let declarationItemsCache = ConditionalWeakTable<string, FSharpDeclarationListItem>()
    
    let xmlMemberIndexService = serviceProvider.GetService(typeof<IVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)

    static member ShouldTriggerCompletionAux(sourceText: SourceText, caretPosition: int, trigger: CompletionTriggerKind, getInfo: (unit -> DocumentId * string * string list)) =
        // Skip if we are at the start of a document
        if caretPosition = 0 then
            false

        // Skip if it was triggered by an operation other than insertion
        elif not (trigger = CompletionTriggerKind.Insertion) then
            false

        // Skip if we are not on a completion trigger
        else
          let triggerPosition = caretPosition - 1
          let c = sourceText.[triggerPosition]
          if not (completionTriggers |> Array.contains c) then
            false

          // Trigger completion if we are on a valid classification type
          else
            let documentId, filePath,  defines = getInfo()
            let textLines = sourceText.Lines
            let triggerLine = textLines.GetLineFromPosition(triggerPosition)
            let classifiedSpanOption =
                CommonHelpers.getColorizationData(documentId, sourceText, triggerLine.Span, Some(filePath), defines, CancellationToken.None)
                |> Seq.tryFind(fun classifiedSpan -> classifiedSpan.TextSpan.Contains(triggerPosition))

            match classifiedSpanOption with
            | None -> false
            | Some(classifiedSpan) ->
                match classifiedSpan.ClassificationType with
                | ClassificationTypeNames.Comment -> false
                | ClassificationTypeNames.StringLiteral -> false
                | ClassificationTypeNames.ExcludedCode -> false
                | _ -> true // anything else is a valid classification type

    static member ProvideCompletionsAsyncAux(sourceText: SourceText, caretPosition: int, options: FSharpProjectOptions, filePath: string, textVersionHash: int) = async {
        let! parseResults, checkFileAnswer = FSharpLanguageService.Checker.ParseAndCheckFileInProject(filePath, textVersionHash, sourceText.ToString(), options)
        match checkFileAnswer with
        | FSharpCheckFileAnswer.Aborted -> return List()
        | FSharpCheckFileAnswer.Succeeded(checkFileResults) -> 

        let textLines = sourceText.Lines
        let caretLine = textLines.GetLineFromPosition(caretPosition)
        let caretLinePos = textLines.GetLinePosition(caretPosition)
        let fcsCaretLineNumber = Line.fromZ caretLinePos.Line  // Roslyn line numbers are zero-based, FSharp.Compiler.Service line numbers are 1-based
        let caretLineColumn = caretLinePos.Character

        let qualifyingNames, partialName = QuickParse.GetPartialLongNameEx(caretLine.ToString(), caretLineColumn - 1) 
        let! declarations = checkFileResults.GetDeclarationListInfo(Some(parseResults), fcsCaretLineNumber, caretLineColumn, caretLine.ToString(), qualifyingNames, partialName)

        let results = List<CompletionItem>()

        for declarationItem in declarations.Items do
            // FSROSLYNTODO: This doesn't yet reflect pulbic/private/internal into the glyph
            // FSROSLYNTODO: We should really use FSharpSymbol information here.  But GetDeclarationListInfo doesn't provide it, and switch to GetDeclarationListSymbols is a bit large at the moment
            let glyph = 
                match declarationItem.GlyphMajor with 
                | GlyphMajor.Class -> Glyph.ClassPublic
                | GlyphMajor.Constant -> Glyph.ConstantPublic
                | GlyphMajor.Delegate -> Glyph.DelegatePublic
                | GlyphMajor.Enum -> Glyph.EnumPublic
                | GlyphMajor.EnumMember -> Glyph.EnumMember
                | GlyphMajor.Event -> Glyph.EventPublic
                | GlyphMajor.Exception -> Glyph.ClassPublic
                | GlyphMajor.FieldBlue -> Glyph.FieldPublic
                | GlyphMajor.Interface -> Glyph.InterfacePublic
                | GlyphMajor.Method -> Glyph.MethodPublic
                | GlyphMajor.Method2 -> Glyph.ExtensionMethodPublic
                | GlyphMajor.Module -> Glyph.ModulePublic
                | GlyphMajor.NameSpace -> Glyph.Namespace
                | GlyphMajor.Property -> Glyph.PropertyPublic
                | GlyphMajor.Struct -> Glyph.StructurePublic
                | GlyphMajor.Typedef -> Glyph.ClassPublic
                | GlyphMajor.Type -> Glyph.ClassPublic
                | GlyphMajor.Union -> Glyph.EnumPublic
                | GlyphMajor.Variable -> Glyph.Local
                | GlyphMajor.ValueType -> Glyph.StructurePublic
                | GlyphMajor.Error -> Glyph.Error
                | _ -> Glyph.ClassPublic

            let completionItem = CommonCompletionItem.Create(declarationItem.Name, glyph=Nullable(glyph))
            declarationItemsCache.Remove(completionItem.DisplayText) |> ignore // clear out stale entries if they exist
            declarationItemsCache.Add(completionItem.DisplayText, declarationItem)
            results.Add(completionItem)

        return results
    }


    override this.ShouldTriggerCompletion(sourceText: SourceText, caretPosition: int, trigger: CompletionTrigger, _: OptionSet) =
        let getInfo() = 
            let documentId = workspace.GetDocumentIdInCurrentContext(sourceText.Container)
            let document = workspace.CurrentSolution.GetDocument(documentId)
            let defines = FSharpLanguageService.GetCompilationDefinesForEditingDocument(document)  
            (documentId, document.FilePath, defines)

        FSharpCompletionProvider.ShouldTriggerCompletionAux(sourceText, caretPosition, trigger.Kind, getInfo)
    
    override this.ProvideCompletionsAsync(context: Microsoft.CodeAnalysis.Completion.CompletionContext) =
        async {
            match FSharpLanguageService.TryGetOptionsForEditingDocumentOrProject(context.Document)  with 
            | Some options ->
                let! sourceText = context.Document.GetTextAsync(context.CancellationToken) |> Async.AwaitTask
                let! textVersion = context.Document.GetTextVersionAsync(context.CancellationToken) |> Async.AwaitTask
                let! results = FSharpCompletionProvider.ProvideCompletionsAsyncAux(sourceText, context.Position, options, context.Document.FilePath, textVersion.GetHashCode())
                context.AddItems(results)
            | None -> ()
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask context.CancellationToken
        

    override this.GetDescriptionAsync(_: Document, completionItem: CompletionItem, cancellationToken: CancellationToken): Task<CompletionDescription> =
        async {
            let exists, declarationItem = declarationItemsCache.TryGetValue(completionItem.DisplayText)
            if exists then
                let! description = declarationItem.DescriptionTextAsync
                let datatipText = XmlDocumentation.BuildDataTipText(documentationBuilder, description) 
                return CompletionDescription.FromText(datatipText)
            else
                return CompletionDescription.Empty
        } |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

type internal FSharpCompletionService(workspace: Workspace, serviceProvider: SVsServiceProvider) =
    inherit CompletionServiceWithProviders(workspace)

    let builtInProviders = ImmutableArray.Create<CompletionProvider>(FSharpCompletionProvider(workspace, serviceProvider))
    let completionRules = CompletionRules.Default.WithDismissIfEmpty(true).WithDismissIfLastCharacterDeleted(true).WithDefaultEnterKeyRule(EnterKeyRule.Never)

    override this.Language = FSharpCommonConstants.FSharpLanguageName
    override this.GetBuiltInProviders() = builtInProviders
    override this.GetRules() = completionRules



[<Shared>]
[<ExportLanguageServiceFactory(typeof<CompletionService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpCompletionServiceFactory [<ImportingConstructor>] (serviceProvider: SVsServiceProvider) =
    interface ILanguageServiceFactory with
        member this.CreateLanguageService(hostLanguageServices: HostLanguageServices) : ILanguageService =
            upcast new FSharpCompletionService(hostLanguageServices.WorkspaceServices.Workspace, serviceProvider)


