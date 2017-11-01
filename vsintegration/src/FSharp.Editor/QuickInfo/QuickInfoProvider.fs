// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading
open System.Threading.Tasks
open System.ComponentModel.Composition
open System.Text

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.FindSymbols

open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler

open Internal.Utilities.StructuredFormat

type private TooltipInfo =
    { StructuredText: FSharpStructuredToolTipText 
      Span: TextSpan
      Symbol: FSharpSymbol
      SymbolKind: LexerSymbolKind }

module private FSharpQuickInfo =

    let userOpName = "QuickInfo"

    // when a construct has been declared in a signature file the documentation comments that are
    // written in that file are the ones that go into the generated xml when the project is compiled
    // therefore we should include these doccoms in our design time tooltips
    let getTooltipFromRange
        (
            checker: FSharpChecker, 
            projectInfoManager: FSharpProjectOptionsManager, 
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
            let! extSpan = RoslynHelpers.TryFSharpRangeToTextSpan (extSourceText, declRange)
            let extLineText = (extSourceText.Lines.GetLineFromPosition extSpan.Start).ToString()
            
            // project options need to be retrieved because the signature file could be in another project 
            let! extParsingOptions, _extSite, extProjectOptions = projectInfoManager.TryGetOptionsForProject extDocId.ProjectId
            let extDefines = CompilerEnvironment.GetCompilationDefinesForEditing (extDocument.FilePath, extParsingOptions)
            let! extLexerSymbol = Tokenizer.getSymbolAtPosition(extDocId, extSourceText, extSpan.Start, declRange.FileName, extDefines, SymbolLookupKind.Greedy, true)
            let! _, _, extCheckFileResults = checker.ParseAndCheckDocument(extDocument, extProjectOptions, allowStaleResults=true, sourceText=extSourceText, userOpName = userOpName)
            
            let! extTooltipText = 
                extCheckFileResults.GetStructuredToolTipText
                    (declRange.StartLine, extLexerSymbol.Ident.idRange.EndColumn, extLineText, extLexerSymbol.FullIsland, FSharpTokenTag.IDENT, userOpName=userOpName) |> liftAsync
            
            match extTooltipText with
            | FSharpToolTipText [] 
            | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
            | extTooltipText  -> 
                let! extSymbolUse =
                    extCheckFileResults.GetSymbolUseAtLocation(declRange.StartLine, extLexerSymbol.Ident.idRange.EndColumn, extLineText, extLexerSymbol.FullIsland, userOpName=userOpName)
                let! span = RoslynHelpers.TryFSharpRangeToTextSpan (extSourceText, extLexerSymbol.Range)
                
                return { StructuredText = extTooltipText
                         Span = span
                         Symbol = extSymbolUse.Symbol
                         SymbolKind = extLexerSymbol.Kind }
        }

    /// Get tooltip combined from doccom of Signature and definition
    let getTooltipInfo 
        (
            checker: FSharpChecker, 
            projectInfoManager: FSharpProjectOptionsManager, 
            document: Document, 
            position: int, 
            cancellationToken: CancellationToken
        ) 
        : Async<(FSharpSymbolUse * TooltipInfo option * TooltipInfo option) option> = 

        asyncMaybe {
            let! sourceText = document.GetTextAsync cancellationToken
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.FilePath, parsingOptions)
            let! lexerSymbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy, true)
            let idRange = lexerSymbol.Ident.idRange  
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, projectOptions, allowStaleResults = true, sourceText=sourceText, userOpName = userOpName)
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()        
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland, userOpName=userOpName)
            
            /// Gets the tooltip information for the orignal target
            let getTargetSymbolTooltip () = 
                asyncMaybe {
                    let! targetTooltip = 
                        checkFileResults.GetStructuredToolTipText
                            (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland, FSharpTokenTag.IDENT, userOpName=userOpName) |> liftAsync
                    
                    match targetTooltip with
                    | FSharpToolTipText [] 
                    | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
                    | _ -> 
                        let! targetTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sourceText, lexerSymbol.Range)
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
                let! findSigDeclarationResult = checkFileResults.GetDeclarationLocation (idRange.StartLine, idRange.EndColumn, lineText, lexerSymbol.FullIsland, preferFlag=true, userOpName=userOpName) |> liftAsync
                    
                // it is necessary to retrieve the backup tooltip info because this acquires
                // the textSpan designating where we want the tooltip to appear.
                let! targetTooltipInfo = getTargetSymbolTooltip()
                
                let! result =
                    match findSigDeclarationResult with 
                    | FSharpFindDeclResult.DeclFound declRange when isSignatureFile declRange.FileName ->
                        asyncMaybe {
                            let! sigTooltipInfo = getTooltipFromRange(checker, projectInfoManager, document, declRange, cancellationToken)
                            
                            // if the target was declared in a signature file, and the current file
                            // is not the corresponding module implementation file for that signature,
                            // the doccoms from the signature will overwrite any doccoms that might be 
                            // present on the definition/implementation
                            let! findImplDefinitionResult = checkFileResults.GetDeclarationLocation (idRange.StartLine, idRange.EndColumn, lineText, lexerSymbol.FullIsland, preferFlag=false, userOpName=userOpName) |> liftAsync   
                            
                            match findImplDefinitionResult  with 
                            | FSharpFindDeclResult.DeclNotFound _ 
                            | FSharpFindDeclResult.ExternalDecl _ -> return symbolUse, Some sigTooltipInfo, None
                            | FSharpFindDeclResult.DeclFound declRange -> 
                                let! implTooltipInfo = getTooltipFromRange(checker, projectInfoManager, document, declRange, cancellationToken)
                                return symbolUse, Some sigTooltipInfo, Some { implTooltipInfo with Span = targetTooltipInfo.Span }
                        }
                    | _ -> async.Return None
                    |> liftAsync
                
                return result |> Option.defaultValue (symbolUse, None, Some targetTooltipInfo)
        }

[<ExportQuickInfoProvider(PredefinedQuickInfoProviderNames.Semantic, FSharpConstants.FSharpLanguageName)>]
type internal FSharpQuickInfoProvider 
    [<ImportingConstructor>] 
    (
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager,
        gotoDefinitionService: FSharpGoToDefinitionService,
        viewProvider: QuickInfoViewProvider
    ) =

    let xmlMemberIndexService = serviceProvider.GetService(typeof<SVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)

    static member ProvideQuickInfo(checker: FSharpChecker, documentId: DocumentId, sourceText: SourceText, filePath: string, position: int, parsingOptions: FSharpParsingOptions, options: FSharpProjectOptions, textVersionHash: int) =
        asyncMaybe {
            let! _, _, checkFileResults = checker.ParseAndCheckDocument (filePath, textVersionHash, sourceText.ToString(), options, allowStaleResults = true, userOpName = FSharpQuickInfo.userOpName)
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing (filePath, parsingOptions)
            let! symbol = Tokenizer.getSymbolAtPosition (documentId, sourceText, position, filePath, defines, SymbolLookupKind.Precise, true)
            let! res = checkFileResults.GetStructuredToolTipText (textLineNumber, symbol.Ident.idRange.EndColumn, textLine.ToString(), symbol.FullIsland, FSharpTokenTag.IDENT, userOpName=FSharpQuickInfo.userOpName) |> liftAsync
            match res with
            | FSharpToolTipText [] 
            | FSharpToolTipText [FSharpStructuredToolTipElement.None] -> return! None
            | _ -> 
                let! symbolUse = checkFileResults.GetSymbolUseAtLocation (textLineNumber, symbol.Ident.idRange.EndColumn, textLine.ToString(), symbol.FullIsland, userOpName=FSharpQuickInfo.userOpName)
                let! symbolSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sourceText, symbol.Range)
                return res, symbolSpan, symbolUse.Symbol, symbol.Kind
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
                    let mainDescription, documentation, typeParameterMap, usage, exceptions = ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray()
                    XmlDocumentation.BuildDataTipText(documentationBuilder, mainDescription.Add, documentation.Add, typeParameterMap.Add, usage.Add, exceptions.Add, tooltip.StructuredText)
                    let glyph = Tokenizer.GetGlyphForSymbol(tooltip.Symbol, tooltip.SymbolKind)
                    let navigation = QuickInfoNavigation(gotoDefinitionService, document, symbolUse.RangeAlternate)
                    let content = viewProvider.ProvideContent(glyph, mainDescription, documentation=documentation, typeParameterMap=typeParameterMap, usage=usage, exceptions=exceptions, navigation=navigation)
                    return QuickInfoItem (tooltip.Span, content)

                | Some sigTooltip, Some targetTooltip ->
                    let mainDescription, targetDocumentation, sigDocumentation, typeParameterMap, exceptions, usage = ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray()
                    XmlDocumentation.BuildDataTipText(documentationBuilder, ignore, sigDocumentation.Add, ignore, ignore, ignore, sigTooltip.StructuredText)
                    XmlDocumentation.BuildDataTipText(documentationBuilder, mainDescription.Add, targetDocumentation.Add, typeParameterMap.Add, exceptions.Add, usage.Add, targetTooltip.StructuredText)

                    let width = 
                        mainDescription
                        |> Seq.append targetDocumentation
                        |> Seq.append exceptions
                        |> Seq.append usage
                        |> Seq.append sigDocumentation
                        |> Seq.append typeParameterMap
                        |> Seq.map (fun x -> x.Text.Length)
                        |> Seq.max

                    // eyeballed formula returning separator width in chars such as to prevent it from wrapping
                    // will not be needed once we replace the ascii-art divider with a XAML element
                    let width = if width / 2 > 85 then 85 else width / 2

                    let seperator = TaggedTextOps.tag Text (String.replicate width "⎯")  
                    let lineBreak = TaggedTextOps.tag LineBreak "\n"

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
                    let glyph = Tokenizer.GetGlyphForSymbol(targetTooltip.Symbol, targetTooltip.SymbolKind)
                    let navigation = QuickInfoNavigation(gotoDefinitionService, document, symbolUse.RangeAlternate)
                    let content = viewProvider.ProvideContent(glyph, mainDescription, documentation=documentation, typeParameterMap=typeParameterMap, usage=usage, exceptions=exceptions, navigation=navigation)
                    return QuickInfoItem (targetTooltip.Span, content)
            }   |> Async.map Option.toObj
                |> RoslynHelpers.StartAsyncAsTask cancellationToken 
