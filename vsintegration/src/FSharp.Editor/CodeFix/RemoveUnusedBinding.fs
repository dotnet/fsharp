// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

[<AutoOpen>]
module FSharpParseFileResultsExtensions =
    type FSharpParseFileResults with
        member this.TryRangeOfBindingWithHeadPatternWithPos pos =
            let input = this.ParseTree
            SyntaxTraversal.Traverse(pos, input, { new SyntaxVisitorBase<_>() with 
                member _.VisitExpr(_, _, defaultTraverse, expr) =
                    defaultTraverse expr

                override _.VisitBinding(_path, defaultTraverse, binding) =
                    match binding with
                    | SynBinding(_, SynBindingKind.Normal, _, _, _, _, _, pat, _, _, _, _) as binding ->
                        if Position.posEq binding.RangeOfHeadPattern.Start pos then
                            Some binding.RangeOfBindingWithRhs
                        else
                            // Check if it's an operator
                            match pat with
                            | SynPat.LongIdent(LongIdentWithDots([id], _), _, _, _, _, _) when id.idText.StartsWith("op_") ->
                                if Position.posEq id.idRange.Start pos then
                                    Some binding.RangeOfBindingWithRhs
                                else
                                    defaultTraverse binding
                            | _ -> defaultTraverse binding

                    | _ -> defaultTraverse binding })

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "RemoveUnusedBinding"); Shared>]
type internal FSharpRemoveUnusedBindingCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    
    inherit CodeFixProvider()
    static let userOpName = "RemoveUnusedBinding"
    let fixableDiagnosticIds = set ["FS1182"]

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            // Don't show code fixes for unused values, even if they are compiler-generated.
            do! Option.guard context.Document.FSharpOptions.CodeFixes.UnusedDeclarations

            let document = context.Document
            let! sourceText = document.GetTextAsync(context.CancellationToken)

            let! parsingOptions, _ = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, context.CancellationToken, userOpName)
            let! parseResults = checkerProvider.Checker.ParseDocument(document, parsingOptions, userOpName=userOpName)

            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray

            let symbolRange = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, context.Span, sourceText)
            let! rangeOfBinding = parseResults.TryRangeOfBindingWithHeadPatternWithPos(symbolRange.Start)
            let! spanOfBinding = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, rangeOfBinding)

            let keywordEndColumn =
                let rec loop ch pos =
                    if not (Char.IsWhiteSpace(ch)) then
                        pos
                    else
                        loop sourceText.[pos - 1] (pos - 1)
                loop sourceText.[spanOfBinding.Start - 1] (spanOfBinding.Start - 1)

            // This is safe, since we could never have gotten here unless there was a `let` or `use`
            let keywordStartColumn = keywordEndColumn - 2
            let fullSpan = TextSpan(keywordStartColumn, spanOfBinding.End - keywordStartColumn)

            let prefixTitle = SR.RemoveUnusedBinding()
            let removalCodeFix =
                CodeFixHelpers.createTextChangeCodeFix(
                    prefixTitle,
                    context,
                    (fun () -> asyncMaybe.Return [| TextChange(fullSpan, "") |]))
            context.RegisterCodeFix(removalCodeFix, diagnostics)
        } 
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
