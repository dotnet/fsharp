// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.Text

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ChangeEqualsInFieldTypeToColon)>]
type internal ChangeEqualsInFieldTypeToColonCodeFixProvider() =
    inherit CodeFixProvider()

    let isInRecord (document: Document) (range: range) =
        cancellableTask {
            let! parseResults = document.GetFSharpParseResultsAsync(nameof ChangeEqualsInFieldTypeToColonCodeFixProvider)

            return parseResults.IsPositionWithinRecordDefinition(range.Start)
        }

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0010")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix(this)

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync document span =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()
                let! sourceText = document.GetTextAsync cancellationToken
                let filePath = document.FilePath
                let spanText = sourceText.GetSubText(span).ToString()

                if spanText <> "=" then
                    return None

                else
                    let errorRange = RoslynHelpers.TextSpanToFSharpRange(filePath, span, sourceText)
                    let! isInRecord = errorRange |> isInRecord document

                    if not isInRecord then
                        return None

                    else
                        let codeFix =
                            {
                                Name = CodeFix.ChangeEqualsInFieldTypeToColon
                                Message = SR.ChangeEqualsInFieldTypeToColon()
                                Changes = [ TextChange(TextSpan(span.Start, span.Length), ":") ]
                            }

                        return (Some codeFix)
            }
