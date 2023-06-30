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

    override this.RegisterCodeFixesAsync context =
        // This is a performance shortcut.
        // Since FS0010 fires all too often, we're just stopping any handling of it
        // in case when there is the message doesn't have a notion of symbols in question.
        // The code fix logic itself still has this logic and implements it more reliably.
        if
            context.Diagnostics
            |> Seq.map (fun d -> $"{d.Descriptor.MessageFormat}")
            |> Seq.exists (fun d -> d.Contains "=" && d.Contains ":")
        then
            context.RegisterFsharpFix(this)
        else
            task { return () }

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! spanText = context.GetSquigglyTextAsync()
                if spanText <> "=" then
                    return None

                else
                    let! errorRange = context.GetErrorRangeAsync()
                    let! isInRecord = errorRange |> isInRecord context.Document

                    if not isInRecord then
                        return None

                    else
                        let codeFix =
                            {
                                Name = CodeFix.ChangeEqualsInFieldTypeToColon
                                Message = SR.ChangeEqualsInFieldTypeToColon()
                                Changes = [ TextChange(TextSpan(context.Span.Start, context.Span.Length), ":") ]
                            }

                        return (Some codeFix)
            }
