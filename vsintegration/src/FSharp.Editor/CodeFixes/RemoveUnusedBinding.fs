// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveUnusedBinding); Shared>]
type internal RemoveUnusedBindingCodeFixProvider [<ImportingConstructor>] () =

    inherit CodeFixProvider()

    static let title = SR.RemoveUnusedBinding()
    override _.FixableDiagnosticIds = ImmutableArray.Create("FS1182")

    member this.GetChanges(document: Document, diagnostics: ImmutableArray<Diagnostic>, ct: CancellationToken) =
        backgroundTask {

            let! sourceText = document.GetTextAsync(ct)
            let! parseResults = document.GetFSharpParseResultsAsync(nameof (RemoveUnusedBindingCodeFixProvider))

            let changes =
                seq {
                    for d in diagnostics do
                        let textSpan = d.Location.SourceSpan

                        let symbolRange =
                            RoslynHelpers.TextSpanToFSharpRange(document.FilePath, textSpan, sourceText)

                        let spanOfBindingOpt =
                            parseResults.TryRangeOfBindingWithHeadPatternWithPos(symbolRange.Start)
                            |> Option.bind (fun rangeOfBinding -> RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, rangeOfBinding))

                        match spanOfBindingOpt with
                        | Some spanOfBinding ->
                            let keywordEndColumn =
                                let rec loop ch pos =
                                    if not (Char.IsWhiteSpace(ch)) then
                                        pos
                                    else
                                        loop sourceText.[pos - 1] (pos - 1)

                                loop sourceText.[spanOfBinding.Start - 1] (spanOfBinding.Start - 1)

                            // This is safe, since we could never have gotten here unless there was a `let` or `use`
                            let keywordStartColumn = keywordEndColumn - 2
                            let fullSpan = TextSpan(keywordStartColumn, spanOfBinding.End - keywordStartColumn)

                            yield TextChange(fullSpan, "")
                        | None -> ()
                }

            return changes
        }

    override this.RegisterCodeFixesAsync ctx : Task =
        backgroundTask {
            if ctx.Document.Project.IsFSharpCodeFixesUnusedDeclarationsEnabled then
                let! changes = this.GetChanges(ctx.Document, ctx.Diagnostics, ctx.CancellationToken)
                ctx.RegisterFsharpFix(CodeFix.RemoveUnusedBinding, title, changes)
        }

    override this.GetFixAllProvider() =
        CodeFixHelpers.createFixAllProvider CodeFix.RemoveUnusedBinding this.GetChanges
