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
open System.Windows
open System.Windows.Controls
open System.Windows.Media

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.QuickInfo
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Text.Formatting
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open System.Windows.Documents

// FSROSLYNTODO: with the merge of the below PR, the QuickInfo API should be changed
// to allow for a more flexible syntax for defining the content of the tooltip.
// The below interface should be discarded then or updated accourdingly.
// https://github.com/dotnet/roslyn/pull/13623
type internal FSharpDeferredQuickInfoContent(content: string, textProperties: TextFormattingRunProperties) =
    interface IDeferredQuickInfoContent with
        override this.Create() : FrameworkElement =
            let textBlock = TextBlock(Run(content), TextWrapping = TextWrapping.Wrap, TextTrimming = TextTrimming.None)
            textBlock.SetValue(TextElement.BackgroundProperty, textProperties.BackgroundBrush)
            textBlock.SetValue(TextElement.ForegroundProperty, textProperties.ForegroundBrush)
            textBlock.SetValue(TextElement.FontFamilyProperty, textProperties.Typeface.FontFamily)
            textBlock.SetValue(TextElement.FontSizeProperty, textProperties.FontRenderingEmSize)
            textBlock.SetValue(TextElement.FontStyleProperty, if textProperties.Italic then FontStyles.Italic else FontStyles.Normal)
            textBlock.SetValue(TextElement.FontWeightProperty, if textProperties.Bold then FontWeights.Bold else FontWeights.Normal)
            upcast textBlock

[<Shared>]
[<ExportQuickInfoProvider(PredefinedQuickInfoProviderNames.Semantic, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpQuickInfoProvider 
    [<System.ComponentModel.Composition.ImportingConstructor>] 
    (
        [<System.ComponentModel.Composition.Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider,
        _classificationFormatMapService: IClassificationFormatMapService,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager,
        typeMap: Shared.Utilities.ClassificationTypeMap
    ) =

    let xmlMemberIndexService = serviceProvider.GetService(typeof<SVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)
    
    static member ProvideQuickInfo(checker: FSharpChecker, documentId: DocumentId, sourceText: SourceText, filePath: string, position: int, options: FSharpProjectOptions, textVersionHash: int) =
        asyncMaybe {
            let! _, checkFileResults = checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText.ToString(), options)
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
            //let qualifyingNames, partialName = QuickParse.GetPartialLongNameEx(textLine.ToString(), textLineColumn - 1)
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(filePath, options.OtherOptions |> Seq.toList)
            let! symbol = CommonHelpers.getSymbolAtPosition(documentId, sourceText, position, filePath, defines, SymbolLookupKind.Fuzzy)
            let! res = checkFileResults.GetStructuredToolTipTextAlternate(textLineNumber, symbol.RightColumn, textLine.ToString(), [symbol.Text], FSharpTokenTag.IDENT) |> liftAsync
            return! 
                match res with
                | FSharpToolTipText [] 
                | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> None
                | _ -> Some(res, CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbol.Range))
        }
    
    interface IQuickInfoProvider with
        override this.GetItemAsync(document: Document, position: int, cancellationToken: CancellationToken): Task<QuickInfoItem> =
            asyncMaybe {
                let! sourceText = document.GetTextAsync(cancellationToken)
                let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)  
                let! _ = CommonHelpers.getSymbolAtPosition(document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Fuzzy)
                let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                let! textVersion = document.GetTextVersionAsync(cancellationToken)
                let! toolTipElement, textSpan = FSharpQuickInfoProvider.ProvideQuickInfo(checkerProvider.Checker, document.Id, sourceText, document.FilePath, position, options, textVersion.GetHashCode())
                let mainDescription = Collections.Generic.List()
                let documentation = Collections.Generic.List()
                XmlDocumentation.BuildDataTipText(
                    documentationBuilder, 
                    CommonRoslynHelpers.CollectTaggedText mainDescription, 
                    CommonRoslynHelpers.CollectTaggedText documentation, 
                    toolTipElement)
                let empty = ClassifiableDeferredContent(Array.Empty<TaggedText>(), typeMap);
                let content = 
                    QuickInfoDisplayDeferredContent
                        (
                            symbolGlyph = null,//SymbolGlyphDeferredContent(),
                            warningGlyph = null,
                            mainDescription = ClassifiableDeferredContent(mainDescription, typeMap),
                            documentation = ClassifiableDeferredContent(documentation, typeMap),
                            typeParameterMap = empty,
                            anonymousTypes = empty,
                            usageText = empty,
                            exceptionText = empty
                        )
                return QuickInfoItem(textSpan, content)
            } 
            |> Async.map Option.toObj
            |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
