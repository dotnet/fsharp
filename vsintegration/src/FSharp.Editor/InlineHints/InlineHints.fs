// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.VisualStudio.Shell

open System
open System.Collections.Immutable
open System.Threading
open System.ComponentModel.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.InlineHints

open FSharp.Compiler
open FSharp.Compiler.SourceCodeServices

[<Export(typeof<IFSharpInlineHintsService>)>]
type internal FSharpInlineHintsService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        [<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =

    static let userOpName = "FSharpInlineHints"

    interface IFSharpInlineHintsService with
        member _.GetInlineHintsAsync(document: Document, textSpan: TextSpan, cancellationToken: CancellationToken) =
            asyncMaybe {
                let! textVersion = document.GetTextVersionAsync(cancellationToken)
                let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, cancellationToken, userOpName)
                let! sourceText = document.GetTextAsync(cancellationToken)
                let! _parseFileResults, _, checkFileResults = checkerProvider.Checker.ParseAndCheckDocument(document, projectOptions, userOpName)
                let range = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, textSpan, sourceText)
                let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFileWithinRange(range) |> liftAsync

                let typeHints = ImmutableArray.CreateBuilder()
                
                // todo - get these at some point I guess
                // most likely need to work with the parse tree API
                // since there is no good way to tell if a symbol is a parameter (declared or used)
                let _parameterHints = ImmutableArray.CreateBuilder()

                for symbolUse in symbolUses do
                    if symbolUse.IsFromDefinition then
                        match symbolUse.Symbol with
                        | :? FSharpMemberOrFunctionOrValue as x when x.IsValue && not x.IsMemberThisValue && not x.IsConstructorThisValue ->
                            let symbolSpan = RoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate)

                            // TODO: look for if 'x' is SynExpr.Typed perhaps?

                            // This does not correctly "classify" non-F# types
                            // TODO deal with that, probably via a new API?
                            let typeLayout = x.FormatLayout symbolUse.DisplayContext
                            
                            let taggedText = ResizeArray()        
                            
                            Layout.renderL (Layout.taggedTextListR taggedText.Add) typeLayout |> ignore
                            
                            let displayParts = ImmutableArray.CreateBuilder()
                            displayParts.Add(TaggedText(TextTags.Text, ": "))
                            
                            taggedText
                            |> Seq.map (fun tt -> RoslynHelpers.roslynTag tt.Tag, tt.Text)
                            |> Seq.map (fun (tag, text) -> TaggedText(tag, text))
                            |> Seq.iter (fun tt -> displayParts.Add(tt))

                            // TODO - this is not actually correct
                            // We need to get QuickInfo for the actual type we pull out, not the value
                            // This code is correct for parameter name hints though, if that gets done!
                            let callBack position =
                                fun _ _ ->
                                    asyncMaybe {
                                        let! quickInfo =
                                            FSharpAsyncQuickInfoSource.ProvideQuickInfo(
                                                checkerProvider.Checker,
                                                document.Id,
                                                sourceText,
                                                document.FilePath,
                                                position,
                                                parsingOptions,
                                                projectOptions,
                                                textVersion.GetHashCode(),
                                                document.FSharpOptions.LanguageServicePerformance)

                                        let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(serviceProvider.XMLMemberIndexService)
                                        let mainDesc, docs  = FSharpAsyncQuickInfoSource.BuildSingleQuickInfoItem documentationBuilder quickInfo

                                        let descriptionParts = ImmutableArray.CreateBuilder()

                                        mainDesc
                                        |> Seq.map (fun tt -> RoslynHelpers.roslynTag tt.Tag, tt.Text)
                                        |> Seq.map (fun (tag, text) -> TaggedText(tag, text))
                                        |> Seq.iter (fun tt -> descriptionParts.Add(tt))

                                        docs
                                        |> Seq.map (fun tt -> RoslynHelpers.roslynTag tt.Tag, tt.Text)
                                        |> Seq.map (fun (tag, text) -> TaggedText(tag, text))
                                        |> Seq.iter (fun tt -> descriptionParts.Add(tt))

                                        return (descriptionParts.ToImmutableArray())
                                    }
                                    |> Async.map (Option.defaultValue ImmutableArray<_>.Empty)
                                    |> RoslynHelpers.StartAsyncAsTask(cancellationToken)

                            let getDescriptionAsync position = Func<Document, CancellationToken, _>(callBack position)

                            let hint = FSharpInlineHint(TextSpan(symbolSpan.End, 0), displayParts.ToImmutableArray(), getDescriptionAsync symbolSpan.Start)
                            typeHints.Add(hint)
                        | _ -> ()

                return typeHints.ToImmutableArray()
            }
            |> Async.map (Option.defaultValue ImmutableArray<_>.Empty)
            |> RoslynHelpers.StartAsyncAsTask(cancellationToken)
