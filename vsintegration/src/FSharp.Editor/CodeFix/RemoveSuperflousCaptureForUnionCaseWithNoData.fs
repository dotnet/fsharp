// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.EditorServices

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveSuperfluousCapture); Shared>]
type internal RemoveSuperflousCaptureForUnionCaseWithNoDataProvider [<ImportingConstructor>] () =

    inherit CodeFixProvider()

    static let title = SR.RemoveUnusedBinding()
    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0725", "FS3548")

    member this.GetChanges(document: Document, diagnostics: ImmutableArray<Diagnostic>, ct: CancellationToken) =
        backgroundTask {

            let! sourceText = document.GetTextAsync(ct)
            let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync(CodeFix.RemoveSuperfluousCapture)

            let changes =
                seq {
                    for d in diagnostics do
                        let textSpan = d.Location.SourceSpan
                        let m = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, textSpan, sourceText)
                        let classifications = checkResults.GetSemanticClassification(Some m)

                        let unionCaseItem =
                            classifications
                            |> Array.tryFind (fun c -> c.Type = SemanticClassificationType.UnionCase)

                        match unionCaseItem with
                        | None -> ()
                        | Some unionCaseItem ->
                            // The error/warning captures entire pattern match, like "Ns.Type.DuName bindingName". We want to keep type info when suggesting a replacement, and only remove "bindingName".
                            let typeInfoLength = unionCaseItem.Range.EndColumn - m.StartColumn

                            let reminderSpan =
                                new TextSpan(textSpan.Start + typeInfoLength, textSpan.Length - typeInfoLength)

                            yield TextChange(reminderSpan, "")
                }

            return changes
        }

    override this.RegisterCodeFixesAsync ctx : Task =
        backgroundTask {
            if ctx.Document.Project.IsFSharpCodeFixesUnusedDeclarationsEnabled then
                let! changes = this.GetChanges(ctx.Document, ctx.Diagnostics, ctx.CancellationToken)
                ctx.RegisterFsharpFix(CodeFix.RemoveSuperfluousCapture, title, changes)
        }

    override this.GetFixAllProvider() =
        CodeFixHelpers.createFixAllProvider CodeFix.RemoveSuperfluousCapture this.GetChanges
