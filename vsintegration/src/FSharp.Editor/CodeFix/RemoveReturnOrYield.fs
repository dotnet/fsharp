// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.Range
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.SourceCodeServices

[<AutoOpen>]
module Traversal =
    type FSharpParseFileResults with
        member scope.TryRangeOfExprInYieldOrReturn pos =
            match scope.ParseTree with
            | Some parseTree ->
                AstTraversal.Traverse(pos, parseTree, { new AstTraversal.AstVisitorBase<_>() with 
                    member __.VisitExpr(_path, _, defaultTraverse, expr) =
                        match expr with
                        | SynExpr.YieldOrReturn(_, expr, range)
                        | SynExpr.YieldOrReturnFrom(_, expr, range) when rangeContainsPos range pos ->
                            Some expr.Range
                        | _ -> defaultTraverse expr })
            | None -> None

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "RemoveReturnOrYield"); Shared>]
type internal FSharpRemoveReturnOrYieldCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    inherit CodeFixProvider()

    static let userOpName = "RemoveReturnOrYield"
    let fixableDiagnosticIds = set ["FS0748"; "FS0747"]

    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override this.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
            let! parsingOptions, _ = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(context.Document, context.CancellationToken, userOpName)
            let! parseResults = checkerProvider.Checker.ParseFile(context.Document.FilePath, sourceText.ToFSharpSourceText(), parsingOptions, userOpName) |> liftAsync

            let errorRange = RoslynHelpers.TextSpanToFSharpRange(context.Document.FilePath, context.Span, sourceText)
            let! exprRange = parseResults.TryRangeOfExprInYieldOrReturn errorRange.Start
            let! exprSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, exprRange)

            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray

            let title =
                let text = sourceText.GetSubText(context.Span).ToString()
                if text.StartsWith("return!") then
                    SR.RemoveReturnBang()
                elif text.StartsWith("return") then
                    SR.RemoveReturn()
                elif text.StartsWith("yield!") then
                    SR.RemoveYieldBang()
                else
                    SR.RemoveYield()

            let codeFix =
                CodeFixHelpers.createTextChangeCodeFix(
                    title,
                    context,
                    (fun () -> asyncMaybe.Return [| TextChange(context.Span, sourceText.GetSubText(exprSpan).ToString()) |]))

            context.RegisterCodeFix(codeFix, diagnostics)
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken) 
