// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.UseTripleQuotedInterpolation); Shared>]
type internal UseTripleQuotedInterpolationCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.UseTripleQuotedInterpolation()

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS3373"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! parseResults = context.Document.GetFSharpParseResultsAsync(nameof UseTripleQuotedInterpolationCodeFixProvider)

                let! sourceText = context.GetSourceTextAsync()
                let! errorRange = context.GetErrorRangeAsync()

                return
                    parseResults.TryRangeOfStringInterpolationContainingPos errorRange.Start
                    |> ValueOption.ofOption
                    |> ValueOption.map (fun range -> RoslynHelpers.FSharpRangeToTextSpan(sourceText, range))
                    |> ValueOption.map (fun span ->
                        let interpolation = sourceText.GetSubText(span).ToString()

                        {
                            Name = CodeFix.UseTripleQuotedInterpolation
                            Message = title
                            Changes = [ TextChange(span, "$\"\"" + interpolation[1..] + "\"\"") ]
                        })
            }
