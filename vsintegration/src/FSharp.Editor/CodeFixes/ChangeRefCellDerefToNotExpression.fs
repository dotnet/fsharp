// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ChangeRefCellDerefToNotExpression); Shared>]
type internal ChangeRefCellDerefToNotExpressionCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.UseNotForNegation()

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0001"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! parseResults = context.Document.GetFSharpParseResultsAsync(nameof ChangeRefCellDerefToNotExpressionCodeFixProvider)

                let! sourceText = context.GetSourceTextAsync()
                let! errorRange = context.GetErrorRangeAsync()

                return
                    parseResults.TryRangeOfRefCellDereferenceContainingPos errorRange.Start
                    |> ValueOption.ofOption
                    |> ValueOption.map (fun range -> RoslynHelpers.FSharpRangeToTextSpan(sourceText, range))
                    |> ValueOption.map (fun span ->
                        {
                            Name = CodeFix.ChangeRefCellDerefToNotExpression
                            Message = title
                            Changes = [ TextChange(span, "not ") ]
                        })
            }
