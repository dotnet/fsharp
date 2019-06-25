// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions
open FSharp.Compiler.SourceCodeServices

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "ProposeUpperCaseLabel"); Shared>]
type internal FSharpProposeUpperCaseLabelCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = ["FS0053"]
    static let userOpName = "ProposeUpperCaseLabel"
        
    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override __.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let textChanger (originalText: string) = originalText.[0].ToString().ToUpper() + originalText.Substring(1)
            let! solutionChanger, originalText = SymbolHelpers.changeAllSymbolReferences(context.Document, context.Span, textChanger, projectInfoManager, checkerProvider.Checker, userOpName)
            let title = CompilerDiagnostics.getErrorMessage (ReplaceWithSuggestion <| textChanger originalText)
            context.RegisterCodeFix(
                CodeAction.Create(title, solutionChanger, title),
                context.Diagnostics |> Seq.filter (fun x -> fixableDiagnosticIds |> List.contains x.Id) |> Seq.toImmutableArray)
        } |> Async.Ignore |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)