// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddNewKeyword); Shared>]
type internal FSharpAddNewKeywordCodeFixProvider() =
    inherit CodeFixProvider()

    static let fixableDiagnosticIds = set [ "FS0760" ]

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override _.RegisterCodeFixesAsync context : Task =
        async {
            let title = SR.AddNewKeyword()

            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray

            let codeFix =
                CodeFixHelpers.createTextChangeCodeFix (
                    CodeFix.AddNewKeyword,
                    title,
                    context,
                    (fun () -> asyncMaybe.Return [| TextChange(TextSpan(context.Span.Start, 0), "new ") |])
                )

            context.RegisterCodeFix(codeFix, diagnostics)
        }
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
