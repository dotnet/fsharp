// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading
open System.Threading.Tasks
open System.Windows
open System.Windows.Controls
open System.Windows.Data
open System.Windows.Media
open System.ComponentModel.Composition
open System.Text

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Editor.Shared.Extensions
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.QuickInfo
open Microsoft.CodeAnalysis.Text


open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.Language.Intellisense

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler

open Internal.Utilities.StructuredFormat

module private SessionHandling =
    let mutable currentSession = None
    
    [<Export (typeof<IQuickInfoSourceProvider>)>]
    [<Name (FSharpProviderConstants.SessionCapturingProvider)>]
    [<Order (After = PredefinedQuickInfoProviderNames.Semantic)>]
    [<ContentType (FSharpConstants.FSharpContentTypeName)>]
    type SourceProviderForCapturingSession () =
        interface IQuickInfoSourceProvider with 
            member x.TryCreateQuickInfoSource _ =
              { new IQuickInfoSource with
                  member __.AugmentQuickInfoSession(session,_,_) = currentSession <- Some session
                  member __.Dispose() = () }

type internal SourceLink(run) as this = 
    inherit Documents.Hyperlink(run)
    
    let lessOpacity =
        { new IValueConverter with
              member this.Convert(value, targetType, _, _) =
                  match value with 
                  | :? Color as c when targetType = typeof<Color> ->
                      // return same color but slightly transparent
                      Color.FromArgb(70uy, c.R, c.G, c.B) :> _
                  | _ -> DependencyProperty.UnsetValue
              member this.ConvertBack(_,_,_,_) = DependencyProperty.UnsetValue }
    
    let underlineBrush = Media.SolidColorBrush()
    do BindingOperations.SetBinding(underlineBrush, SolidColorBrush.ColorProperty, Binding("Foreground.Color", Source = this, Converter = lessOpacity)) |> ignore
    let normalUnderline = TextDecorationCollection [TextDecoration(Location = TextDecorationLocation.Underline, PenOffset = 1.0)]
    let slightUnderline = TextDecorationCollection [TextDecoration(Location = TextDecorationLocation.Underline, PenOffset = 1.0, Pen = Pen(Brush = underlineBrush))]
    do this.TextDecorations <- slightUnderline

    override this.OnMouseEnter(e) = 
        base.OnMouseEnter(e)
        this.TextDecorations <- normalUnderline

    override this.OnMouseLeave(e) = 
        base.OnMouseLeave(e)
        this.TextDecorations <- slightUnderline

type private TooltipInfo =
    { StructuredText: FSharpStructuredToolTipText 
      Span: TextSpan
      Symbol: FSharpSymbol
      SymbolKind: LexerSymbolKind }

module private FSharpQuickInfo =
    
    let private empty = 
        { new IDeferredQuickInfoContent with 
            member x.Create() = TextBlock(Visibility = Visibility.Collapsed) :> FrameworkElement }

    let createDeferredContent (symbolGlyph, mainDescription, documentation) =
        QuickInfoDisplayDeferredContent(symbolGlyph, null, mainDescription, documentation, empty, empty, empty, empty)

    // when a construct has been declared in a signature file the documentation comments that are
    // written in that file are the ones that go into the generated xml when the project is compiled
    // therefore we should include these doccoms in our design time tooltips
    let getTooltipFromRange
        (
            checker: FSharpChecker, 
            projectInfoManager: ProjectInfoManager, 
            document: Document, 
            declRange: range, 
            cancellationToken: CancellationToken
        ) 
        : Async<TooltipInfo option> = 
        
        asyncMaybe {
            let solution = document.Project.Solution
            // ascertain the location of the target declaration in the signature file
            let! extDocId = solution.GetDocumentIdsWithFilePath declRange.FileName |> Seq.tryHead
            let extDocument = solution.GetProject(extDocId.ProjectId).GetDocument extDocId
            let! extSourceText = extDocument.GetTextAsync cancellationToken
            let extSpan = RoslynHelpers.FSharpRangeToTextSpan (extSourceText, declRange)
            let extLineText = (extSourceText.Lines.GetLineFromPosition extSpan.Start).ToString()
            
            // project options need to be retrieved because the signature file could be in another project 
            let! extProjectOptions = projectInfoManager.TryGetOptionsForProject extDocId.ProjectId
            let extDefines = CompilerEnvironment.GetCompilationDefinesForEditing (extDocument.FilePath, List.ofSeq extProjectOptions.OtherOptions)
            let! extLexerSymbol = Tokenizer.getSymbolAtPosition(extDocId, extSourceText, extSpan.Start, declRange.FileName, extDefines, SymbolLookupKind.Greedy)
            let! _, _, extCheckFileResults = checker.ParseAndCheckDocument(extDocument,extProjectOptions,allowStaleResults=true,sourceText=extSourceText)
            
            let! extTooltipText = 
                extCheckFileResults.GetStructuredToolTipTextAlternate
                    (declRange.StartLine, extLexerSymbol.Ident.idRange.EndColumn, extLineText, extLexerSymbol.FullIsland, FSharpTokenTag.IDENT) |> liftAsync
            
            match extTooltipText with
            | FSharpToolTipText [] 
            | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
            | extTooltipText  -> 
                let! extSymbolUse =
                    extCheckFileResults.GetSymbolUseAtLocation(declRange.StartLine, extLexerSymbol.Ident.idRange.EndColumn, extLineText, extLexerSymbol.FullIsland)
                
                return { StructuredText = extTooltipText
                         Span = RoslynHelpers.FSharpRangeToTextSpan (extSourceText, extLexerSymbol.Range)
                         Symbol = extSymbolUse.Symbol
                         SymbolKind = extLexerSymbol.Kind }
        }

    /// Get tooltip combined from doccom of Signature and definition
    let getTooltipInfo 
        (
            checker: FSharpChecker, 
            projectInfoManager: ProjectInfoManager, 
            document: Document, 
            position: int, 
            cancellationToken: CancellationToken
        ) 
        : Async<(FSharpSymbolUse * TooltipInfo option * TooltipInfo option) option> = 

        asyncMaybe {
            let! sourceText = document.GetTextAsync cancellationToken
            let! projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.FilePath, projectOptions.OtherOptions |> Seq.toList)
            let! lexerSymbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy)
            let idRange = lexerSymbol.Ident.idRange  
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, projectOptions, allowStaleResults = true, sourceText=sourceText)
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()        
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland)
            
            /// Gets the tooltip information for the orignal target
            let getTargetSymbolTooltip () = 
                asyncMaybe {
                    let! targetTooltip = 
                        checkFileResults.GetStructuredToolTipTextAlternate
                            (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland, FSharpTokenTag.IDENT) |> liftAsync
                    
                    match targetTooltip with
                    | FSharpToolTipText [] 
                    | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
                    | _ -> 
                        let targetTextSpan = RoslynHelpers.FSharpRangeToTextSpan (sourceText, lexerSymbol.Range)
                        return { StructuredText = targetTooltip
                                 Span = targetTextSpan
                                 Symbol = symbolUse.Symbol
                                 SymbolKind = lexerSymbol.Kind }
                } 

            // if the target is in a signature file, adjusting the tooltip info is unnecessary
            if isSignatureFile document.FilePath then
                let! targetTooltipInfo = getTargetSymbolTooltip()
                return symbolUse, None, Some targetTooltipInfo
            else
                // find the declaration location of the target symbol, with a preference for signature files
                let! findSigDeclarationResult = 
                    checkFileResults.GetDeclarationLocationAlternate
                        (idRange.StartLine, idRange.EndColumn, lineText, lexerSymbol.FullIsland, preferFlag=true) |> liftAsync
                    
                // it is necessary to retrieve the backup tooltip info because this acquires
                // the textSpan designating where we want the tooltip to appear.
                let! targetTooltipInfo = getTargetSymbolTooltip()
                
                match findSigDeclarationResult with 
                | FSharpFindDeclResult.DeclNotFound _ -> return symbolUse, None, Some targetTooltipInfo
                | FSharpFindDeclResult.DeclFound declRange -> 
                    if isSignatureFile declRange.FileName then 
                        let! sigTooltipInfo = getTooltipFromRange(checker, projectInfoManager, document, declRange, cancellationToken)
                        // if the target was declared in a signature file, and the current file
                        // is not the corresponding module implementation file for that signature,
                        // the doccoms from the signature will overwrite any doccoms that might be 
                        // present on the definition/implementation
                
                        let! findImplDefinitionResult = 
                            checkFileResults.GetDeclarationLocationAlternate
                                (idRange.StartLine, idRange.EndColumn, lineText, lexerSymbol.FullIsland, preferFlag=false) |> liftAsync   
                
                        match findImplDefinitionResult  with 
                        | FSharpFindDeclResult.DeclNotFound _ -> return symbolUse, Some sigTooltipInfo, None
                        | FSharpFindDeclResult.DeclFound declRange -> 
                            let! implTooltipInfo = getTooltipFromRange(checker, projectInfoManager, document, declRange, cancellationToken)
                            return symbolUse, Some sigTooltipInfo, Some { implTooltipInfo with Span = targetTooltipInfo.Span }
                    else 
                        return symbolUse, None, Some targetTooltipInfo
        }

[<ExportQuickInfoProvider(PredefinedQuickInfoProviderNames.Semantic, FSharpConstants.FSharpLanguageName)>]
type internal FSharpQuickInfoProvider 
    [<System.ComponentModel.Composition.ImportingConstructor>] 
    (
        [<System.ComponentModel.Composition.Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager,
        typeMap: Shared.Utilities.ClassificationTypeMap,
        gotoDefinitionService:FSharpGoToDefinitionService,
        glyphService: IGlyphService
    ) =

    let fragment (content: Layout.TaggedText seq, typemap: ClassificationTypeMap, initialDoc: Document, thisSymbolUseRange: range) : IDeferredQuickInfoContent =

        let workspace = initialDoc.Project.Solution.Workspace
        let solution = workspace.CurrentSolution

        let isTargetValid range =
            range <> rangeStartup && solution.TryGetDocumentIdFromFSharpRange (range,initialDoc.Project.Id) |> Option.isSome

        let navigateTo (range:range) = 
            asyncMaybe { 
                let targetPath = range.FileName 
                let! targetDoc = solution.TryGetDocumentFromFSharpRange (range,initialDoc.Project.Id)
                let! targetSource = targetDoc.GetTextAsync() 
                let! targetTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (targetSource, range)
                // to ensure proper navigation decsions we need to check the type of document the navigation call
                // is originating from and the target we're provided by default
                //  - signature files (.fsi) should navigate to other signature files 
                //  - implementation files (.fs) should navigate to other implementation files
                let (|Signature|Implementation|) filepath =
                    if isSignatureFile filepath then Signature else Implementation

                match initialDoc.FilePath, targetPath with 
                | Signature, Signature 
                | Implementation, Implementation ->
                    return (gotoDefinitionService.TryNavigateToTextSpan (targetDoc, targetTextSpan))
                // adjust the target from signature to implementation
                | Implementation, Signature  ->
                    return! gotoDefinitionService.NavigateToSymbolDefinitionAsync (targetDoc, targetSource, range)|>liftAsync
                // adjust the target from implmentation to signature
                | Signature, Implementation -> 
                    return! gotoDefinitionService.NavigateToSymbolDeclarationAsync (targetDoc, targetSource, range)|>liftAsync
            } 
            |> Async.map(
                function
                | Some true -> SessionHandling.currentSession |> Option.iter (fun session -> session.Dismiss())
                | _ -> ()) 
            |> Async.Ignore 
            |> Async.StartImmediate 

        let formatMap = typemap.ClassificationFormatMapService.GetClassificationFormatMap "tooltip"

        let layoutTagToFormatting (layoutTag: LayoutTag) =
            layoutTag
            |> RoslynHelpers.roslynTag
            |> ClassificationTags.GetClassificationTypeName
            |> typemap.GetClassificationType
            |> formatMap.GetTextProperties

        let inlines = 
            seq { 
                for taggedText in content do
                    let run = Documents.Run taggedText.Text
                    let inl =
                        match taggedText with
                        | :? Layout.NavigableTaggedText as nav when thisSymbolUseRange <> nav.Range && isTargetValid nav.Range ->                        
                            let h = SourceLink (run, ToolTip = nav.Range.FileName)
                            h.Click.Add (fun _ -> navigateTo nav.Range)
                            h :> Documents.Inline
                        | _ -> run :> _
                    DependencyObjectExtensions.SetTextProperties (inl, layoutTagToFormatting taggedText.Tag)
                    yield inl
            }

        let createTextLinks () =
            let tb = TextBlock(TextWrapping = TextWrapping.Wrap, TextTrimming = TextTrimming.None)
            DependencyObjectExtensions.SetDefaultTextProperties(tb, formatMap)
            tb.Inlines.AddRange inlines
            if tb.Inlines.Count = 0 then tb.Visibility <- Visibility.Collapsed
            tb :> FrameworkElement
            
        { new IDeferredQuickInfoContent with member x.Create() = createTextLinks() }

    let xmlMemberIndexService = serviceProvider.GetService(typeof<SVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)

    static member ProvideQuickInfo(checker: FSharpChecker, documentId: DocumentId, sourceText: SourceText, filePath: string, position: int, options: FSharpProjectOptions, textVersionHash: int) =
        asyncMaybe {
            let! _, _, checkFileResults = checker.ParseAndCheckDocument (filePath, textVersionHash, sourceText.ToString(), options, allowStaleResults = true)
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing (filePath, options.OtherOptions |> Seq.toList)
            let! symbol = Tokenizer.getSymbolAtPosition (documentId, sourceText, position, filePath, defines, SymbolLookupKind.Precise)
            let! res = checkFileResults.GetStructuredToolTipTextAlternate (textLineNumber, symbol.Ident.idRange.EndColumn, textLine.ToString(), symbol.FullIsland, FSharpTokenTag.IDENT) |> liftAsync
            match res with
            | FSharpToolTipText [] 
            | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
            | _ -> 
                let! symbolUse = checkFileResults.GetSymbolUseAtLocation (textLineNumber, symbol.Ident.idRange.EndColumn, textLine.ToString(), symbol.FullIsland)
                return res, RoslynHelpers.FSharpRangeToTextSpan (sourceText, symbol.Range), symbolUse.Symbol, symbol.Kind
        }
    
    interface IQuickInfoProvider with
        override this.GetItemAsync(document: Document, position: int, cancellationToken: CancellationToken): Task<QuickInfoItem> =
            asyncMaybe {
                let! symbolUse, sigTooltipInfo, targetTooltipInfo = 
                    FSharpQuickInfo.getTooltipInfo(checkerProvider.Checker, projectInfoManager, document, position, cancellationToken)

                match sigTooltipInfo, targetTooltipInfo with 
                | None, None -> return null
                | Some tooltip, None  
                | None, Some tooltip -> 
                    let mainDescription = ResizeArray()
                    let documentation = ResizeArray()
                    XmlDocumentation.BuildDataTipText(documentationBuilder, mainDescription.Add, documentation.Add, tooltip.StructuredText)
                    let content = 
                        FSharpQuickInfo.createDeferredContent
                            (SymbolGlyphDeferredContent(Tokenizer.GetGlyphForSymbol(tooltip.Symbol, tooltip.SymbolKind), glyphService),
                             fragment (mainDescription, typeMap, document, symbolUse.RangeAlternate),
                             fragment (documentation, typeMap, document, symbolUse.RangeAlternate))
                    return QuickInfoItem (tooltip.Span, content)

                | Some sigTooltip, Some targetTooltip ->
                    let description, targetDocumentation, sigDocumentation = ResizeArray(), ResizeArray(), ResizeArray()
                    XmlDocumentation.BuildDataTipText(documentationBuilder, ignore, sigDocumentation.Add, sigTooltip.StructuredText)
                    XmlDocumentation.BuildDataTipText(documentationBuilder, description.Add, targetDocumentation.Add, targetTooltip.StructuredText)

                    let width = 
                        description
                        |> Seq.append targetDocumentation
                        |> Seq.append sigDocumentation
                        |> Seq.map (fun x -> x.Text.Length)
                        |> Seq.max

                    let seperator = TaggedTextOps.tag LayoutTag.Text (String.replicate width "-")  
                    let lineBreak = TaggedTextOps.tag LayoutTag.LineBreak  "\n"

                    // get whitespace nomalized documentation text
                    let getText (tts: seq<Layout.TaggedText>) = 
                        let text = 
                            (StringBuilder(), tts) 
                            ||> Seq.fold (fun sb tt -> 
                                if String.IsNullOrWhiteSpace tt.Text then sb else sb.Append tt.Text)
                            |> string
                        if String.IsNullOrWhiteSpace text then None else Some text
                    
                    let documentation = 
                        [ match getText targetDocumentation, getText sigDocumentation with
                          | None, None -> ()
                          | None, Some _ -> yield! sigDocumentation
                          | Some _, None -> yield! targetDocumentation
                          | Some implText, Some sigText when implText.Equals (sigText, StringComparison.OrdinalIgnoreCase) -> 
                              yield! sigDocumentation
                          | Some _, Some _ -> 
                              yield! sigDocumentation
                              yield lineBreak
                              yield seperator
                              yield lineBreak
                              yield! targetDocumentation ]

                    let content = 
                        FSharpQuickInfo.createDeferredContent
                            (SymbolGlyphDeferredContent (Tokenizer.GetGlyphForSymbol (targetTooltip.Symbol, targetTooltip.SymbolKind), glyphService),
                            fragment (description, typeMap, document, symbolUse.RangeAlternate),
                            fragment (documentation, typeMap, document, symbolUse.RangeAlternate))

                    return QuickInfoItem (targetTooltip.Span, content)
            }   |> Async.map Option.toObj
                |> RoslynHelpers.StartAsyncAsTask cancellationToken 
