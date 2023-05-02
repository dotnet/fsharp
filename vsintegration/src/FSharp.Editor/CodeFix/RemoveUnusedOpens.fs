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
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

open FSharp.Compiler.Text

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveUnusedOpens); Shared>]
type internal FSharpRemoveUnusedOpensCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.RemoveUnusedOpens()

    override _.FixableDiagnosticIds =
        ImmutableArray.Create FSharpIDEDiagnosticIds.RemoveUnnecessaryImportsDiagnosticId

    member this.GetChanges(document: Document, diagnostics: ImmutableArray<Diagnostic>, ct: CancellationToken) =
        backgroundTask {
            let! sourceText = document.GetTextAsync(ct)

            let changes =
                diagnostics
                |> Seq.map (fun d ->
                    sourceText
                        .Lines
                        .GetLineFromPosition(
                            d.Location.SourceSpan.Start
                        )
                        .SpanIncludingLineBreak)
                |> Seq.map (fun span -> TextChange(span, ""))

            return changes
        }

    override this.RegisterCodeFixesAsync ctx : Task =
        backgroundTask {
            if ctx.Document.Project.IsFSharpCodeFixesUnusedOpensEnabled then
                let! changes = this.GetChanges(ctx.Document, ctx.Diagnostics, ctx.CancellationToken)
                ctx.RegisterFsharpFix(CodeFix.RemoveUnusedOpens, title, changes)
        }

    override this.GetFixAllProvider() =
        CodeFixHelpers.createFixAllProvider CodeFix.RemoveUnusedOpens this.GetChanges
