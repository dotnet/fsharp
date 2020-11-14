// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "AddIgnoreToExpression"); Shared>]
type internal FSharpAddIgnoreKeywordCodeFixProvider() =
    inherit CodeFixProvider()

    static let fixableDiagnosticIds = set ["FS0020"; "FS0001" ]

    override __.FixableDiagnosticIds = fixableDiagnosticIds.ToImmutableArray()

    override this.RegisterCodeFixesAsync context : Task =
        async {
            let title = SR.IgnoreExpression()

            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray

            let codeFix =
                CodeFixHelpers.createTextChangeCodeFix(
                    title,
                    context,
                    (fun () -> asyncMaybe.Return [| TextChange(TextSpan(context.Span.End, 0), " |> ignore") |]))
            
            context.RegisterCodeFix(codeFix, diagnostics)
        } |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
 