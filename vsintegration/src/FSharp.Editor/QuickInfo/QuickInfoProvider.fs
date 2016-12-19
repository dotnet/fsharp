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
open Microsoft.CodeAnalysis.Editor.Shared.Extensions

open Microsoft.VisualStudio.Language.Intellisense
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Text.Formatting
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Imaging
open Microsoft.VisualStudio.Imaging.Interop
open Microsoft.VisualStudio.PlatformUI

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

open System.Windows.Documents
open System.Windows.Controls
open System.Windows.Data
open System.Windows.Media

// FSROSLYNTODO: with the merge of the below PR, the QuickInfo API should be changed
// to allow for a more flexible syntax for defining the content of the tooltip.
// The below interface should be discarded then or updated accourdingly.
// https://github.com/dotnet/roslyn/pull/13623
type internal FSharpDeferredQuickInfoContent(content: string, textProperties: TextFormattingRunProperties, glyph: Glyph) =
    interface IDeferredQuickInfoContent with
        override this.Create() : FrameworkElement =
            let moniker = GlyphExtensions.GetImageMoniker(glyph)
            let image = new CrispImage()
            image.Moniker <- (box moniker) :?> _
 
            // Inform the ImageService of the background color so that images have the correct background.
            let binding = 
                Binding(
                    "Background", 
                    Converter = new BrushToColorConverter(), 
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof<StackPanel>, 1))
 
            image.SetBinding(ImageThemingUtilities.ImageBackgroundColorProperty, binding) |> ignore

            image.Margin <- new Thickness(1., 1., 3., 1.)
            let symbolGlyphBorder =
                new Border(
                    BorderThickness = new Thickness(0.),
                    BorderBrush = Brushes.Transparent,
                    VerticalAlignment = VerticalAlignment.Top,
                    Child = image)

            let textBlock = TextBlock(Run(content), TextWrapping = TextWrapping.Wrap, TextTrimming = TextTrimming.None)
            textBlock.SetValue(TextElement.BackgroundProperty, textProperties.BackgroundBrush)
            textBlock.SetValue(TextElement.ForegroundProperty, textProperties.ForegroundBrush)
            textBlock.SetValue(TextElement.FontFamilyProperty, textProperties.Typeface.FontFamily)
            textBlock.SetValue(TextElement.FontSizeProperty, textProperties.FontRenderingEmSize)
            textBlock.SetValue(TextElement.FontStyleProperty, if textProperties.Italic then FontStyles.Italic else FontStyles.Normal)
            textBlock.SetValue(TextElement.FontWeightProperty, if textProperties.Bold then FontWeights.Bold else FontWeights.Normal)

            let symbolGlyphAndMainDescriptionDock = 
                new DockPanel(
                    LastChildFill = true,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = Brushes.Transparent)
 
            symbolGlyphAndMainDescriptionDock.Children.Add(symbolGlyphBorder) |> ignore
            symbolGlyphAndMainDescriptionDock.Children.Add(textBlock) |> ignore

            let panel = StackPanel(Orientation = Orientation.Vertical)
            panel.Children.Add(symbolGlyphAndMainDescriptionDock) |> ignore
            upcast panel

[<Shared>]
[<ExportQuickInfoProvider(PredefinedQuickInfoProviderNames.Semantic, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpQuickInfoProvider 
    [<System.ComponentModel.Composition.ImportingConstructor>] 
    (
        [<System.ComponentModel.Composition.Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider,
        classificationFormatMapService: IClassificationFormatMapService,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =

    let xmlMemberIndexService = serviceProvider.GetService(typeof<SVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)
    
    static member ProvideQuickInfo(checker: FSharpChecker, documentId: DocumentId, sourceText: SourceText, filePath: string, position: int, 
                                   options: FSharpProjectOptions, textVersionHash: int, cancellationToken: CancellationToken) =
        async {
            let! _parseResults, checkResultsAnswer = checker.ParseAndCheckFileInProject(filePath, textVersionHash, sourceText.ToString(), options)
            let checkFileResults = 
                match checkResultsAnswer with
                | FSharpCheckFileAnswer.Aborted -> failwith "Compilation isn't complete yet"
                | FSharpCheckFileAnswer.Succeeded(results) -> results
          
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let textLineColumn = textLinePos.Character
            //let qualifyingNames, partialName = QuickParse.GetPartialLongNameEx(textLine.ToString(), textLineColumn - 1)
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(filePath, options.OtherOptions |> Seq.toList)
            
            let tryClassifyAtPosition position = 
                CommonHelpers.tryClassifyAtPosition(documentId, sourceText, filePath, defines, position, SymbolSearchKind.DoesNotIncludeRightColumn, cancellationToken)
            
            let quickParseInfo = 
                match tryClassifyAtPosition position with 
                | None when textLineColumn > 0 -> tryClassifyAtPosition (position - 1) 
                | res -> res

            match quickParseInfo with 
            | Some (islandColumn, qualifiers, textSpan) -> 
                let! res = checkFileResults.GetToolTipTextAlternate(textLineNumber, islandColumn, textLine.ToString(), qualifiers, FSharpTokenTag.IDENT)
                match res with
                | FSharpToolTipText [] -> return None
                | _ -> 
                    let! symbolUse = checkFileResults.GetSymbolUseAtLocation(textLineNumber, islandColumn, textLine.ToString(), qualifiers)
                    match symbolUse with
                    | Some symbolUse ->
                        return Some(res, textSpan, symbolUse.Symbol)
                    | None -> return None
            | None -> return None
        }
    
    interface IQuickInfoProvider with
        override this.GetItemAsync(document: Document, position: int, cancellationToken: CancellationToken): Task<QuickInfoItem> =
            async {
                let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)  
                let classification = 
                    CommonHelpers.tryClassifyAtPosition(document.Id, sourceText, document.FilePath, defines, position, SymbolSearchKind.DoesNotIncludeRightColumn, cancellationToken)

                match classification with
                | Some _ ->
                    match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)  with 
                    | Some options ->
                        let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                        let! quickInfoResult = FSharpQuickInfoProvider.ProvideQuickInfo(checkerProvider.Checker, document.Id, sourceText, document.FilePath, position, options, textVersion.GetHashCode(), cancellationToken)
                        match quickInfoResult with
                        | Some(toolTipElement, textSpan, symbol) ->
                            let dataTipText = XmlDocumentation.BuildDataTipText(documentationBuilder, toolTipElement)
                            let textProperties = classificationFormatMapService.GetClassificationFormatMap("tooltip").DefaultTextProperties
                            return QuickInfoItem(textSpan, FSharpDeferredQuickInfoContent(dataTipText, textProperties, Glyph.forSymbol symbol))
                        | None -> return null
                    | None -> return null
                | None -> return null
            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)