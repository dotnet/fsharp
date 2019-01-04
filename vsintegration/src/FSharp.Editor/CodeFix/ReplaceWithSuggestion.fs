// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open Microsoft.FSharp.Compiler.SourceCodeServices

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "ReplaceWithSuggestion"); Shared>]
type internal FSharpReplaceWithSuggestionCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    inherit CodeFixProvider()
    static let userOpName = "ReplaceWithSuggestionCodeFix"
    let fixableDiagnosticIds = set ["FS0039"; "FS1129"; "FS0495"]
    let checker = checkerProvider.Checker

    let getSymbolUsesInProjects (projects: seq<Project>) =
        projects
        |> Seq.map (fun project ->
            async {
                match! projectInfoManager.TryGetOptionsByProject(project, CancellationToken.None) with
                | Some (_, projectOptions) ->
                    let! projectCheckResults = checker.ParseAndCheckProject(projectOptions, userOpName = userOpName)
                    let! uses = projectCheckResults.GetAllUsesOfAllSymbols()
                    return uses |> Array.distinctBy (fun symbolUse -> symbolUse.RangeAlternate)
                | None -> return [||]
            })
        |> Async.Parallel
        |> Async.map Array.concat
        // FCS may return several `FSharpSymbolUse`s for same range, which have different `ItemOccurrence`s (Use, UseInAttribute, UseInType, etc.)
        // We don't care about the occurrence type here, so we distinct by range.
        |> Async.map (Array.distinctBy (fun x -> x.RangeAlternate))
        
    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override __.RegisterCodeFixesAsync context : Task =
        async {
            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray

            let document = context.Document
            let! sourceText = document.GetTextAsync(context.CancellationToken) |> Async.AwaitTask
            let unresolvedIdentifierText = sourceText.GetSubText(context.Span).ToString() // Tiny string, so the allocation here is fine

            let projects =
                document.Project.Solution.GetDocumentIdsWithFilePath(document.FilePath)
                |> Seq.map (fun x -> x.ProjectId)
                |> Seq.distinct
                |> Seq.map document.Project.Solution.GetProject

            let! allSymbolUses = getSymbolUsesInProjects projects
            let suggestedNames = ErrorResolutionHints.getSuggestedNames allSymbolUses unresolvedIdentifierText

            match suggestedNames with
            | None -> ()
            | Some suggestions ->
                for suggestion in suggestions do
                    let replacement = Keywords.QuoteIdentifierIfNeeded suggestion
                    let codeFix = 
                        CodeFixHelpers.createTextChangeCodeFix(
                            FSComp.SR.replaceWithSuggestion suggestion,
                            context,
                            (fun () -> asyncMaybe.Return [| TextChange(context.Span, replacement) |]))
                
                    context.RegisterCodeFix(codeFix, diagnostics)
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
