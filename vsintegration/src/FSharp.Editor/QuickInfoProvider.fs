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
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

// FSROSLYNTODO: with the merge of the below PR, the QuickInfo API should be changed
// to allow for a more flexible syntax for defining the content of the tooltip.
// The below interface should be discarded then or updated accourdingly.
// https://github.com/dotnet/roslyn/pull/13623
type internal FSharpDeferredQuickInfoContent(content: string) =
    interface IDeferredQuickInfoContent with
        override this.Create() : FrameworkElement =
            upcast new Label(Content = content, Foreground = SolidColorBrush(Colors.Black))

[<Shared>]
[<ExportQuickInfoProvider(PredefinedQuickInfoProviderNames.Semantic, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpQuickInfoProvider [<System.ComponentModel.Composition.ImportingConstructor>] 
    ([<System.ComponentModel.Composition.Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider) =

    let xmlMemberIndexService = serviceProvider.GetService(typeof<SVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)

    static member ProvideQuickInfo(document: Document, sourceText: SourceText, position: int, options: FSharpProjectOptions, textVersionHash: int, cancellationToken: CancellationToken) =
        async {
            let! _parseResults, checkResultsAnswer = FSharpChecker.Instance.ParseAndCheckFileInProject(document.FilePath, textVersionHash, sourceText.ToString(), options)
            let checkFileResults = 
                match checkResultsAnswer with
                | FSharpCheckFileAnswer.Aborted -> failwith "Compilation isn't complete yet"
                | FSharpCheckFileAnswer.Succeeded(results) -> results
          
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let textLineColumn = textLinePos.Character
            //let qualifyingNames, partialName = QuickParse.GetPartialLongNameEx(textLine.ToString(), textLineColumn - 1)
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
            let tryClassifyAtPosition position = 
                CommonHelpers.tryClassifyAtPosition(document.Id, sourceText, document.FilePath, defines, position, cancellationToken)
            
            let quickParseInfo = 
                match tryClassifyAtPosition position with 
                | None when textLineColumn > 0 -> tryClassifyAtPosition (position - 1) 
                | res -> res

            match quickParseInfo with 
            | Some (islandColumn, qualifiers) -> 
                let! res = checkFileResults.GetToolTipTextAlternate(textLineNumber, islandColumn, textLine.ToString(), qualifiers, FSharpTokenTag.IDENT)
                return Some(res)
            | None -> return None
        }
    
    interface IQuickInfoProvider with
        override this.GetItemAsync(document: Document, position: int, cancellationToken: CancellationToken): Task<QuickInfoItem> =
            async {
                match FSharpLanguageService.GetOptions(document.Project.Id) with
                | Some(options) ->
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let textSpan = TextSpan(0, sourceText.Length)
                    let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                    let classification = CommonHelpers.tryClassifyAtPosition(document.Id, sourceText, document.FilePath, defines, position, cancellationToken)

                    match classification with
                    | Some _ ->
                        let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                        let! toolTipElement = FSharpQuickInfoProvider.ProvideQuickInfo(document, sourceText, position, options, textVersion.GetHashCode(), cancellationToken)
                        match toolTipElement with
                        | Some toolTipElement ->
                            let dataTipText = XmlDocumentation.BuildDataTipText(documentationBuilder, toolTipElement) 
                            return QuickInfoItem(textSpan, FSharpDeferredQuickInfoContent(dataTipText))
                        | None -> return null
                    | None -> return null
                | None -> return null
            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)