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
            let label = new Label()
            label.Content <- content
            label.Foreground <- SolidColorBrush(Colors.White)
            upcast label

[<Shared>]
[<ExportQuickInfoProvider(PredefinedQuickInfoProviderNames.Semantic, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpQuickInfoProvider [<ImportingConstructor>] (serviceProvider: SVsServiceProvider) =

    let xmlMemberIndexService = serviceProvider.GetService(typeof<SVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)

    static member ProvideQuickInfo(sourceText: SourceText, position: int, options: FSharpProjectOptions, filePath: string, textVersionHash: int) = 
        async {
            let! parseResults = FSharpChecker.Instance.ParseFileInProject(filePath, sourceText.ToString(), options)
            let! checkFileAnswer = FSharpChecker.Instance.CheckFileInProject(parseResults, filePath, textVersionHash, sourceText.ToString(), options)
            let checkFileResults = 
                match checkFileAnswer with
                | FSharpCheckFileAnswer.Aborted -> failwith "Compilation isn't complete yet"
                | FSharpCheckFileAnswer.Succeeded(results) -> results
            
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
            let qualifyingNames, partialName = QuickParse.GetPartialLongNameEx(textLine.ToString(), position - textLine.Start - 1)
            return! checkFileResults.GetToolTipTextAlternate(textLineNumber, position, textLine.ToString(), qualifyingNames, tagOfToken(token.IDENT(partialName)))
        }
    
    interface IQuickInfoProvider with
        override this.GetItemAsync(document: Document, position: int, cancellationToken: CancellationToken): Task<QuickInfoItem> =
            async {
                match FSharpLanguageService.GetOptions(document.Project.Id) with
                | Some(options) ->
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let textSpan = TextSpan(0, sourceText.Length)
                    let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                    let tokens = FSharpColorizationService.GetColorizationData(document.Id, sourceText, textSpan, Some(document.FilePath), defines, cancellationToken)

                    match tokens |> Seq.tryFind(fun t -> t.TextSpan.Contains(position)) with
                    | Some(token) ->
                        match token.ClassificationType with
                        | ClassificationTypeNames.Identifier ->
                            let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                            let! toolTipElement = FSharpQuickInfoProvider.ProvideQuickInfo(sourceText, position, options, document.FilePath, textVersion.GetHashCode())
                            let dataTipText = XmlDocumentation.BuildDataTipText(documentationBuilder, toolTipElement) 
                            return QuickInfoItem(token.TextSpan, FSharpDeferredQuickInfoContent(dataTipText))
                        | _ -> return Unchecked.defaultof<_>
                    | None -> return Unchecked.defaultof<_>
                | None -> return Unchecked.defaultof<_>
            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)