// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 

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
                    let! projectCheckResults = checker.ParseAndCheckProject(projectOptions, userOpName=userOpName)
                    let! uses = projectCheckResults.GetAllUsesOfAllSymbols()
                    return uses |> Array.distinctBy (fun symbolUse -> symbolUse.RangeAlternate)
                | None -> return [||]
            })
        |> Async.Parallel
        |> Async.map Array.concat
        // FCS may return several `FSharpSymbolUse`s for same range, which have different `ItemOccurrence`s (Use, UseInAttribute, UseInType, etc.)
        // We don't care about the occurrence type here, so we distinct by range.
        |> Async.map (Array.distinctBy (fun x -> x.RangeAlternate))
        |> liftAsync

    let getSymbolUse (context: CodeFixContext) =
        asyncMaybe {
            let document = context.Document
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, context.CancellationToken)
            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
            let! _, _, checkResults = checker.ParseAndCheckDocument(document, projectOptions, sourceText = sourceText, userOpName = userOpName)
            let line = sourceText.Lines.GetLineFromPosition(context.Span.End)
            let linePos = sourceText.Lines.GetLinePosition(context.Span.End)
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
            let! lexerSymbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, context.Span.End, document.FilePath, defines, SymbolLookupKind.Greedy, false)
            return! checkResults.GetSymbolUseAtLocation(Line.fromZ linePos.Line, lexerSymbol.Ident.idRange.EndColumn, line.ToString(), lexerSymbol.FullIsland, userOpName=userOpName)
        } |> liftAsync
        
    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override __.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let document = context.Document
            
            // Attempt to get a symbol from the identifier that triggered the code fix. If we got one, bail since it's not unresolved.
            let! symbolUse = getSymbolUse context
            do! Option.guard symbolUse.IsNone
            
            let! allSymbolUses = getSymbolUsesInProjects document.Project.Solution.Projects
            let! sourceText = document.GetTextAsync(context.CancellationToken)
            let unresolvedIdentifierText = sourceText.GetSubText(context.Span).ToString()
                
            let suggestedNames = ErrorResolutionHints.getSuggestedNames allSymbolUses unresolvedIdentifierText

            match suggestedNames with
            | None -> ()
            | Some suggestions ->
                let diagnostics =
                    context.Diagnostics
                    |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                    |> Seq.toImmutableArray

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
