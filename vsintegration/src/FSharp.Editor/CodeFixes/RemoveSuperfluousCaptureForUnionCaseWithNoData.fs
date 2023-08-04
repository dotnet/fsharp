// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.EditorServices

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveSuperfluousCapture); Shared>]
type internal RemoveSuperfluousCaptureForUnionCaseWithNoDataCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.RemoveUnusedBinding()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0725", "FS3548") // cannot happen at once

    override this.RegisterCodeFixesAsync ctx =
        if ctx.Document.Project.IsFSharpCodeFixesUnusedDeclarationsEnabled then
            ctx.RegisterFsharpFix this
        else
            Task.CompletedTask

    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! sourceText = context.GetSourceTextAsync()
                let! _, checkResults = context.Document.GetFSharpParseAndCheckResultsAsync CodeFix.RemoveSuperfluousCapture

                let m =
                    RoslynHelpers.TextSpanToFSharpRange(context.Document.FilePath, context.Span, sourceText)

                let classifications = checkResults.GetSemanticClassification(Some m)

                return
                    classifications
                    |> Array.tryFindV (fun c -> c.Type = SemanticClassificationType.UnionCase)
                    |> ValueOption.map (fun unionCaseItem ->
                        // The error/warning captures entire pattern match, like "Ns.Type.DuName bindingName". We want to keep type info when suggesting a replacement, and only remove "bindingName".
                        let typeInfoLength = unionCaseItem.Range.EndColumn - m.StartColumn

                        let reminderSpan =
                            TextSpan(context.Span.Start + typeInfoLength, context.Span.Length - typeInfoLength)

                        {
                            Name = CodeFix.RemoveSuperfluousCapture
                            Message = title
                            Changes = [ TextChange(reminderSpan, "") ]
                        })
            }
