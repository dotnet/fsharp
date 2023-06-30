// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.MakeOuterBindingRecursive); Shared>]
type internal MakeOuterBindingRecursiveCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0039"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! parseResults = context.Document.GetFSharpParseResultsAsync(nameof MakeOuterBindingRecursiveCodeFixProvider)
                let! sourceText = context.GetSourceTextAsync()
                let! diagnosticRange = context.GetErrorRangeAsync()

                if not <| parseResults.IsPosContainedInApplication diagnosticRange.Start then
                    return None
                else
                    return
                        parseResults.TryRangeOfNameOfNearestOuterBindingContainingPos diagnosticRange.Start
                        |> Option.bind (fun bindingRange -> RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, bindingRange))
                        |> Option.filter (fun bindingSpan ->
                            sourceText
                                .GetSubText(bindingSpan)
                                .ContentEquals(sourceText.GetSubText context.Span))
                        |> Option.map (fun bindingSpan ->
                            let title =
                                String.Format(SR.MakeOuterBindingRecursive(), sourceText.GetSubText(bindingSpan).ToString())

                            {
                                Name = CodeFix.MakeOuterBindingRecursive
                                Message = title
                                Changes = [ TextChange(TextSpan(bindingSpan.Start, 0), "rec ") ]
                            })
            }
