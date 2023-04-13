// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddNewKeyword); Shared>]
type internal FSharpAddNewKeywordCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.AddNewKeyword()
    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0760"

    member this.GetChangedDocument(document: Document, diagnostics: ImmutableArray<Diagnostic>, ct: CancellationToken) =
        backgroundTask {
            let! sourceText = document.GetTextAsync(ct)

            let changes =
                diagnostics
                |> Seq.map (fun d -> TextChange(TextSpan(d.Location.SourceSpan.Start, 0), "new "))

            CodeFixHelpers.reportCodeFixRecommendation diagnostics document CodeFix.AddNewKeyword
            return document.WithText(sourceText.WithChanges(changes))
        }

    override this.RegisterCodeFixesAsync ctx : Task =
        backgroundTask {

            let codeAction =
                CodeAction.Create(title, (fun ct -> this.GetChangedDocument(ctx.Document, ctx.Diagnostics, ct)), title)

            ctx.RegisterCodeFix(codeAction, this.GetPrunedDiagnostics(ctx))
        }

    override this.GetFixAllProvider() =
        FixAllProvider.Create(fun fixAllCtx doc allDiagnostics -> this.GetChangedDocument(doc, allDiagnostics, fixAllCtx.CancellationToken))
