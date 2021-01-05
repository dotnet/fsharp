// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Symbols

[<AutoOpen>]
module Ass =
    type FSharpParseFileResults with
        member this.Fart(pos) =
            let ranges = ResizeArray()
            SyntaxTraversal.Traverse(pos, this.ParseTree, { new SyntaxVisitorBase<_>() with 
                member _.VisitExpr(_path, _traverseSynExpr, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.New(_, SynType.LongIdent(identifier), _, _range) ->
                        ranges.Add(identifier.Range)
                        defaultTraverse expr
                    | _ -> defaultTraverse expr })
            |> ignore
            ranges

[<Export(typeof<IFSharpDocumentDiagnosticAnalyzer>)>]
type internal RedundantNewDiagnosticAnalyzer
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =

    static let userOpName = "RedundantNewDiagnosticAnalyzer"
    static let descriptorId = "FSIDE0001"
    static let descriptor =
        DiagnosticDescriptor(
            descriptorId,
            "This ain't necessary",
            "This ain't necessary",
            "Style",
            DiagnosticSeverity.Info,
            true,
            null,
            null,
            FSharpDiagnosticCustomTags.Unnecessary)

    interface IFSharpDocumentDiagnosticAnalyzer with

        member this.AnalyzeSyntaxAsync(_document: Document, _cancellationToken: CancellationToken): Task<ImmutableArray<Diagnostic>> =
            Task.FromResult(ImmutableArray<Diagnostic>.Empty)

        member this.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken): Task<ImmutableArray<Diagnostic>> =
            asyncMaybe {
                match! projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, cancellationToken, userOpName) with
                | (_parsingOptions, projectOptions) ->
                    let! sourceText = document.GetTextAsync()
                    let checker = checkerProvider.Checker
                    let! parseResults, _, checkResults = checker.ParseAndCheckDocument(document, projectOptions, sourceText = sourceText, userOpName = userOpName)
                    let symbolUses = checkResults.GetAllUsesOfAllSymbolsInFile(cancellationToken)
                    let ranges = parseResults.Fart(parseResults.ParseTree.Range.End)
                    let toot = 
                        symbolUses
                        |> Seq.distinctBy (fun su -> su.Range) // Account for "hidden" uses, like a val in a member val definition. These aren't relevant
                        |> Seq.filter(fun su -> su.IsFromDefinition)
                        |> Seq.filter (fun su -> ranges.Contains(su.Range))
                        |> Seq.filter (fun su -> match su.Symbol with :? FSharpEntity as entity -> not entity.IsDisposableType | _ -> false) 
                        |> Seq.map (fun su -> su.Range)
                        |> Seq.map (fun m -> Diagnostic.Create(descriptor, RoslynHelpers.RangeToLocation(m, sourceText, document.FilePath)))
                        |> Seq.toImmutableArray

                    return toot
            }
            |> Async.map (Option.defaultValue ImmutableArray<Diagnostic>.Empty)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
