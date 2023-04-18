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

    member this.GetChanges(_document: Document, diagnostics: ImmutableArray<Diagnostic>, _ct: CancellationToken) =
        backgroundTask {

            let changes =
                diagnostics
                |> Seq.map (fun d -> TextChange(TextSpan(d.Location.SourceSpan.Start, 0), "new "))

            return changes
        }

    override this.RegisterCodeFixesAsync ctx : Task =
        backgroundTask {
            let! changes = this.GetChanges(ctx.Document, ctx.Diagnostics, ctx.CancellationToken)
            ctx.RegisterFsharpFix(CodeFix.AddNewKeyword, title, changes)
        }

    override this.GetFixAllProvider() =
        CodeFixHelpers.createFixAllProvider CodeFix.AddNewKeyword this.GetChanges
