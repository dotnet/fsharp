// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.ComponentModel.Composition
open System.IO
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation
open Microsoft.CodeAnalysis.Text
open Microsoft.ServiceHub.Framework
open Microsoft.VisualStudio.Copilot
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.ServiceBroker

open FSharp.Compiler.EditorServices
open CancellableTasks

/// Solution-wide lookup of F# declarations behind the Copilot chat "#" mention picker.
/// Kept apart from the brokered service so it can be exercised without a Visual Studio workspace.
module internal CopilotSymbolQuery =

    [<Literal>]
    let private MaxMentions = 20

    /// Overloads and partial definitions share one fully qualified name; a handful of them is plenty of context.
    [<Literal>]
    let private MaxDeclarations = 4

    [<Literal>]
    let private UserOpName = "CopilotSymbolContext"

    let private fsharpDocuments (solution: Solution) =
        solution.Projects
        |> Seq.where (fun project -> project.Language = FSharpConstants.FSharpLanguageName)
        |> Seq.collect _.Documents

    let describe (item: NavigableItem) (document: Document) =
        let container =
            match item.Container.FullName with
            | "" -> Path.GetFileName document.FilePath
            | name -> name

        if document.IsFSharpSignatureFile then
            $"signature, {container} - {document.Project.Name}"
        else
            $"{container} - {document.Project.Name}"

    /// Declarations whose fully qualified name matches `searchText`, best match first, one entry per name.
    let search (cache: FSharpNavigableItemsCache) (solution: Solution) (searchText: string) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let tryMatch = cache.CreateMatcherFor searchText

            let matchesIn (document: Document) =
                cancellableTask {
                    ct.ThrowIfCancellationRequested()
                    let! items = cache.GetNavigableItems document

                    return
                        items
                        |> Seq.chooseV (fun item ->
                            tryMatch item
                            |> ValueOption.map (fun patternMatch -> struct (patternMatch.Kind, item, document)))
                }

            let! hits =
                fsharpDocuments solution
                |> Seq.map matchesIn
                // Throttle to avoid launching a parse per document in the solution all at once.
                |> CancellableTask.whenAllThrottled (max 1 Environment.ProcessorCount)

            return
                hits
                |> Seq.collect id
                |> Seq.sortBy (fun (struct (kind, item: NavigableItem, document: Document)) ->
                    document.IsFSharpSignatureFile, kind, item.Name.Length)
                |> Seq.distinctBy (fun (struct (_, item, _)) -> CopilotSymbolMapping.fullyQualifiedName item)
                |> Seq.truncate MaxMentions
                |> Seq.map (fun (struct (_, item, document)) -> struct (item, document))
                |> Seq.toArray
        }

    /// Declarations carrying exactly this fully qualified name. Signature files answer only when no
    /// implementation declares the name.
    let declarationsOf (cache: FSharpNavigableItemsCache) (solution: Solution) (fullyQualifiedName: string) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()

            let matchesIn (document: Document) =
                cancellableTask {
                    ct.ThrowIfCancellationRequested()
                    let! items = cache.GetNavigableItems document

                    return
                        items
                        |> Seq.chooseV (fun item ->
                            if CopilotSymbolMapping.hasFullyQualifiedName fullyQualifiedName item then
                                ValueSome struct (item, document)
                            else
                                ValueNone)
                }

            let! hits =
                fsharpDocuments solution
                |> Seq.map matchesIn
                // Throttle to avoid launching a parse per document in the solution all at once.
                |> CancellableTask.whenAllThrottled (max 1 Environment.ProcessorCount)
                |> CancellableTask.map (Seq.collect id)

            let implementations =
                hits
                |> Seq.filter (fun (struct (_, document: Document)) -> not document.IsFSharpSignatureFile)

            let preferred =
                if Seq.isEmpty implementations then
                    hits :> _ seq
                else
                    implementations

            return preferred |> Seq.truncate MaxDeclarations |> Seq.toArray
        }

    /// The source of the whole declaration `item` names, together with the span it occupies.
    let snippetOf (item: NavigableItem) (document: Document) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let! sourceText = document.GetTextAsync ct
            let! parseResults = document.GetFSharpParseResultsAsync UserOpName

            let sourceLines =
                Array.init sourceText.Lines.Count (fun line -> sourceText.Lines[line].ToString())

            let scopes = Structure.getOutliningRanges sourceLines parseResults.ParseTree

            let struct (firstLine, lastLine) =
                CopilotSymbolSnippets.definitionLines sourceLines scopes item

            let firstLine = max 1 firstLine
            let lastLine = min sourceText.Lines.Count lastLine

            let span =
                TextSpan.FromBounds(sourceText.Lines[firstLine - 1].Start, sourceText.Lines[lastLine - 1].End)

            return struct (sourceText.GetSubText(span).ToString(), span)
        }

    let symbolContext (cache: FSharpNavigableItemsCache) (solution: Solution) (fullyQualifiedName: string) =
        cancellableTask {
            let! declarations = declarationsOf cache solution fullyQualifiedName

            match Array.tryHeadV declarations with
            | ValueNone -> return ValueNone
            | ValueSome(struct (first, _)) ->
                let snippets = ResizeArray()
                let locations = ResizeArray()

                for struct (item, document) in declarations do
                    let! struct (text, span) = snippetOf item document
                    snippets.Add text
                    locations.Add(SnippetLocation(document.FilePath, CopilotSpan(span.Start, span.Length)))

                return
                    ValueSome(
                        CopilotSymbolContext(
                            fullyQualifiedName,
                            first.Name,
                            String.Join(Environment.NewLine + Environment.NewLine, snippets),
                            CopilotSymbolMapping.symbolContextType first.Kind,
                            locations.ToArray()
                        )
                    )
        }

/// Offers F# declarations to Copilot chat, which merges them into the picker shown for "#".
/// Copilot's own symbol provider reads the Roslyn compilation, which F# projects do not have.
[<ExportBrokeredService(FSharpConstants.copilotSymbolProviderName,
                        CopilotDescriptors.CurrentContextProviderVersion,
                        [| typeof<ICopilotMentionQueryable>; typeof<ICopilotMentionBatchQueryable> |],
                        Audience = (ServiceAudience.PublicSdk ||| ServiceAudience.Local))>]
type internal FSharpCopilotContextProvider
    [<ImportingConstructor>]
    (cache: FSharpNavigableItemsCache, [<Import(AllowDefault = true)>] workspace: VisualStudioWorkspace | null) =

    static let moniker =
        ServiceMoniker(FSharpConstants.copilotSymbolProviderName, Version CopilotDescriptors.CurrentContextProviderVersion)

    static let descriptor =
        CopilotContextDescriptor(
            CopilotSymbolMapping.SymbolMember,
            "An F# type, module, member or value declared in the current solution.",
            CopilotDefaultTypes.SymbolContextName,
            [|
                CopilotInputDescriptor(
                    CopilotSymbolMapping.FullyQualifiedNameInput,
                    "Fully qualified name of the F# declaration.",
                    CopilotDefaultTypes.StringName,
                    IsRequired = true
                )
            |]
        )

    static let members = [| descriptor |] :> IReadOnlyList<CopilotContextDescriptor>

    static let memberNames = [| CopilotSymbolMapping.SymbolMember |] :> IReadOnlyList<string>

    static let noMentions =
        Array.empty<CopilotQueriedMention> :> IReadOnlyCollection<CopilotQueriedMention>

    let mentionFor (item: NavigableItem) (document: Document) =
        let inputs = Dictionary<string, CopilotValue>(StringComparer.Ordinal)

        inputs[CopilotSymbolMapping.FullyQualifiedNameInput] <-
            CopilotValue(CopilotDefaultTypes.StringName, CopilotSymbolMapping.fullyQualifiedName item)

        let description = CopilotSymbolQuery.describe item document

        CopilotQueriedContextMention(
            moniker,
            descriptor,
            inputs,
            item.Name,
            Description = description,
            Tooltip = description,
            Icon = Nullable(CopilotSymbolMapping.icon item.Kind),
            IsNavigable = true
        )
        :> CopilotQueriedMention

    /// The user is still typing, so the trailing input is the search text. It is preceded by the member
    /// name once the mention has been committed, as in "#fsharpSymbol:Namespace.Type".
    let searchTextOf (query: CopilotMentionQuery) =
        match query.Type, query.Inputs with
        | CopilotMentionType.Context, null -> ValueNone
        | CopilotMentionType.Context, inputs when inputs.Count > 0 ->
            match inputs[inputs.Count - 1] with
            | text when String.IsNullOrWhiteSpace text -> ValueNone
            | text when String.Equals(text, CopilotSymbolMapping.SymbolMember, StringComparison.Ordinal) -> ValueNone
            | text -> ValueSome text
        | _ -> ValueNone

    let mentionsFor (searchText: string voption) =
        cancellableTask {
            match workspace, searchText with
            | null, _
            | _, ValueNone -> return noMentions
            | workspace, ValueSome searchText ->
                let! hits = CopilotSymbolQuery.search cache workspace.CurrentSolution searchText

                return
                    hits |> Array.map (fun (struct (item, document)) -> mentionFor item document)
                    :> IReadOnlyCollection<CopilotQueriedMention>
        }

    let fullyQualifiedNameOf (inputs: IReadOnlyDictionary<string, CopilotValue> | null) =
        match inputs with
        | null -> ValueNone
        | inputs ->
            match inputs.TryGetValue CopilotSymbolMapping.FullyQualifiedNameInput with
            | true, value ->
                match value.TryGetValue<string>() with
                | true, name when not (String.IsNullOrWhiteSpace name) -> ValueSome name
                | _ -> ValueNone
            | _ -> ValueNone

    interface IExportedBrokeredService with
        member _.Descriptor = CopilotDescriptors.CreateContextProviderDescriptor moniker

        member _.InitializeAsync _cancellationToken = Task.CompletedTask

    interface ICopilotContextReducer with
        member _.ReduceAsync(context, _reduction, _counter, _cancellationToken) = Task.FromResult context

    interface ICopilotContextProvider with
        member _.GetMembersAsync _cancellationToken =
            ValueTask<IReadOnlyList<CopilotContextDescriptor>> members

        member _.GetMembersAsync(_requestId, _cancellationToken) = Task.FromResult memberNames

        member _.StoreAsync(_requestId, _cancellationToken) = ValueTask()

        member _.ReleaseAsync(_requestId, _cancellationToken) = ValueTask()

        member _.GetContextAsync(requestId, memberName, inputs, cancellationToken) : Task<CopilotContext> =
            match workspace, fullyQualifiedNameOf inputs with
            | null, _
            | _, ValueNone -> Task.FromResult null
            | workspace, ValueSome fullyQualifiedName when
                String.Equals(memberName, CopilotSymbolMapping.SymbolMember, StringComparison.Ordinal)
                ->
                cancellableTask {
                    let! symbol = CopilotSymbolQuery.symbolContext cache workspace.CurrentSolution fullyQualifiedName

                    match symbol with
                    | ValueNone -> return null
                    | ValueSome symbol -> return CopilotContext(moniker, descriptor, requestId, symbol, CanReduce = false)
                }
                |> CancellableTask.start cancellationToken
            | _ -> Task.FromResult null

    interface ICopilotMentionQueryable with
        member _.QueryMentionAsync(query, cancellationToken) : Task<IReadOnlyCollection<CopilotQueriedMention>> =
            mentionsFor (searchTextOf query) |> CancellableTask.start cancellationToken

        member _.NavigateToMentionableAsync(mention, cancellationToken) : Task<bool> =
            match workspace, fullyQualifiedNameOf mention.Inputs with
            | null, _
            | _, ValueNone -> Task.FromResult false
            | workspace, ValueSome fullyQualifiedName ->
                cancellableTask {
                    let! ct = CancellableTask.getCancellationToken ()
                    let solution = workspace.CurrentSolution
                    let! declarations = CopilotSymbolQuery.declarationsOf cache solution fullyQualifiedName

                    match Array.tryHeadV declarations with
                    | ValueNone -> return false
                    | ValueSome(struct (item, document)) ->
                        let! sourceText = document.GetTextAsync ct

                        match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, item.Range) with
                        | ValueNone -> return false
                        | ValueSome span ->
                            do! ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync ct

                            let navigation =
                                solution.Workspace.Services.GetService<IFSharpDocumentNavigationService>()

                            return navigation.TryNavigateToSpan(solution.Workspace, document.Id, span, ct)
                }
                |> CancellableTask.start cancellationToken

    // Copilot's own picker providers answer through the batch interface, one result collection per query.
    // Each distinct search text scans the solution once, and the scans run side by side.
    interface ICopilotMentionBatchQueryable with
        member _.QueryMentionBatchAsync(queries, cancellationToken) : Task<IReadOnlyList<IReadOnlyCollection<CopilotQueriedMention>>> =
            cancellableTask {
                let searchTexts = queries |> Seq.map searchTextOf |> Seq.toArray
                let distinct = Array.distinct searchTexts
                let! mentions = distinct |> Array.map mentionsFor |> CancellableTask.whenAll
                let byText = Array.zip distinct mentions |> dict

                return searchTexts |> Array.map (fun text -> byText[text]) :> IReadOnlyList<IReadOnlyCollection<CopilotQueriedMention>>
            }
            |> CancellableTask.start cancellationToken
