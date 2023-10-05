// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

//open System.Composition
//open System.Collections.Immutable
//open System.Threading
//open System.Threading.Tasks

//open FSharp.Compiler.EditorServices

//open Microsoft.CodeAnalysis
//open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

////type IFSharpUnnecessaryParenthesesDiagnosticAnalyzer = inherit IFSharpDocumentDiagnosticAnalyzer

//[<Export(typeof<IFSharpUnnecessaryParenthesesDiagnosticAnalyzer>)>]
//type internal UnnecessaryParenthesesDiagnosticAnalyzer [<ImportingConstructor>] () =
//    static let descriptor =
//        let title = "Parentheses can be removed."
//        DiagnosticDescriptor(
//            "IDE0047",
//            title,
//            title,
//            "Style",
//            DiagnosticSeverity.Hidden,
//            isEnabledByDefault=true,
//            description=null,
//            helpLinkUri=null)

//    interface IFSharpUnnecessaryParenthesesDiagnosticAnalyzer with
//        member _.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken) =
//            ignore (document, cancellationToken)
//            Task.FromResult ImmutableArray.Empty

//        member _.AnalyzeSyntaxAsync(document: Document, cancellationToken: CancellationToken) =
//            asyncMaybe {
//                let! parseResults = document.GetFSharpParseResultsAsync(nameof UnnecessaryParenthesesDiagnosticAnalyzer) |> liftAsync
//                let! unnecessaryParentheses = UnnecessaryParentheses.getUnnecessaryParentheses parseResults.ParseTree |> liftAsync
//                let! ct = Async.CancellationToken |> liftAsync
//                let! sourceText = document.GetTextAsync ct
//                return
//                    unnecessaryParentheses
//                    |> Seq.map (fun range -> Diagnostic.Create(descriptor, RoslynHelpers.RangeToLocation(range, sourceText, document.FilePath)))
//                    |> Seq.toImmutableArray
//            }
//            |> Async.map (Option.defaultValue ImmutableArray.Empty)
//            |> RoslynHelpers.StartAsyncAsTask cancellationToken
