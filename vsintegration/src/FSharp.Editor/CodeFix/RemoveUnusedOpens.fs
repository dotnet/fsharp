// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

open FSharp.Compiler.Range

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "RemoveUnusedOpens"); Shared>]
type internal FSharpRemoveUnusedOpensCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = [IDEDiagnosticIds.RemoveUnnecessaryImportsDiagnosticId]
        
    let createCodeFix (title: string, context: CodeFixContext) =
        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                asyncMaybe {
                    let document = context.Document
                    let! sourceText = document.GetTextAsync()
                    let checker = checkerProvider.Checker
                    let! _parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, context.CancellationToken)
                    let! unusedOpens = UnusedOpensDiagnosticAnalyzer.GetUnusedOpenRanges(document, projectOptions, checker)
                    let changes =
                        unusedOpens
                        |> List.map (fun m ->
                            let span = sourceText.Lines.[Line.toZ m.StartLine].SpanIncludingLineBreak
                            TextChange(span, ""))
                        |> List.toArray

                    return document.WithText(sourceText.WithChanges(changes))
                }
                |> Async.map (Option.defaultValue context.Document)
                |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
            title)

    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override __.RegisterCodeFixesAsync context : Task =
        async {
            let diagnostics = context.Diagnostics |> Seq.filter (fun x -> fixableDiagnosticIds |> List.contains x.Id) |> Seq.toImmutableArray
            context.RegisterCodeFix(createCodeFix(SR.RemoveUnusedOpens(), context), diagnostics)
        } |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)

    override __.GetFixAllProvider() = WellKnownFixAllProviders.BatchFixer
 