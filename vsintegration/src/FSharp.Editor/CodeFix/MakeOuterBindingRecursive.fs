// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.Range
open FSharp.Compiler.SourceCodeServices

[<AutoOpen>]
module TreeExt =
    type FSharpParseFileResults with
        member scope.TryRangeOfNearestOuterBindingContainingPos pos =
            let toot binding =
                match binding with
                | SynBinding.Binding (_, _, _, _, _, _, _, headPat, _, _, _, _) ->
                    match headPat with
                    | SynPat.LongIdent (longIdentWithDots, _, _, _, _, _) ->
                        Some longIdentWithDots.Range
                    | SynPat.Named(_, ident, false, _, _) ->
                        Some ident.idRange
                    | _ ->
                        None

            let rec yeet expr workingRange =
                match expr with
                | SynExpr.LetOrUse(false, false, bindings, bodyExpr, range) when rangeContainsPos range pos ->
                    let potentialNestedRange =
                        bindings
                        |> List.tryFind (fun binding -> rangeContainsPos binding.RangeOfBindingAndRhs pos)
                        |> Option.bind toot
                    match potentialNestedRange with
                    | Some range ->
                        yeet bodyExpr range
                    | None ->
                        Some workingRange
                | _ ->
                    Some workingRange

            match scope.ParseTree with
            | Some input ->
                AstTraversal.Traverse(pos, input, { new AstTraversal.AstVisitorBase<_>() with
                    member _.VisitExpr(_, _, defaultTraverse, expr) =                        
                        defaultTraverse expr

                    override _.VisitBinding(defaultTraverse, binding) =
                        match binding with
                        | SynBinding.Binding (_, _, _, _, _, _, _, _, _, expr, _range, _) as b when rangeContainsPos b.RangeOfBindingAndRhs pos ->
                            match toot b with
                            | Some range -> yeet expr range
                            | None -> None
                        | _ -> defaultTraverse binding
                })
            | None -> None

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "MakeOuterBindingRecursive"); Shared>]
type internal FSharpMakeOuterBindingRecursiveCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    inherit CodeFixProvider()

    static let userOpName = "MakeOuterBindingRecursive"
    let fixableDiagnosticIds = set ["FS0039"]

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override _.RegisterCodeFixesAsync context =
        asyncMaybe {
            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
            let! parsingOptions, _ = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(context.Document, context.CancellationToken, userOpName)
            let! parseResults = checkerProvider.Checker.ParseFile(context.Document.FilePath, sourceText.ToFSharpSourceText(), parsingOptions, userOpName) |> liftAsync

            let diagnosticRange = RoslynHelpers.TextSpanToFSharpRange(context.Document.FilePath, context.Span, sourceText)
            do! Option.guard (parseResults.IsPosContainedInApplication diagnosticRange.Start)

            let! outerBindingRange = parseResults.TryRangeOfNearestOuterBindingContainingPos diagnosticRange.Start
            let! outerBindingNameSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, outerBindingRange)

            // One last check to verify the names are the same
            do! Option.guard (sourceText.GetSubText(outerBindingNameSpan).ContentEquals(sourceText.GetSubText(context.Span)))

            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray

            let title = SR.MakeOuterBindingRecursive()

            let codeFix =
                CodeFixHelpers.createTextChangeCodeFix(
                    title,
                    context,
                    (fun () -> asyncMaybe.Return [| TextChange(TextSpan(outerBindingNameSpan.Start, 0), "rec ") |]))

            context.RegisterCodeFix(codeFix, diagnostics)
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken) 
