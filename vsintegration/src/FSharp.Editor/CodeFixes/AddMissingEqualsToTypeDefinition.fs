// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddMissingEqualsToTypeDefinition); Shared>]
type internal AddMissingEqualsToTypeDefinitionCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.AddMissingEqualsToTypeDefinition()

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0010"

    override this.RegisterCodeFixesAsync context =
        // This is a performance shortcut.
        // Since FS0010 fires all too often, we're just stopping any processing if it's a different error message.
        // The code fix logic itself still has this logic and implements it more reliably.
        if
            context.Diagnostics
            |> Seq.exists (fun d -> d.Descriptor.MessageFormat.ToString().Contains "=")
        then
            context.RegisterFsharpFix this
        else
            Task.CompletedTask

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! range = context.GetErrorRangeAsync()
                let! parseResults = context.Document.GetFSharpParseResultsAsync(nameof AddMissingEqualsToTypeDefinitionCodeFixProvider)

                if not <| parseResults.IsPositionWithinTypeDefinition range.Start then
                    return ValueNone

                else
                    return
                        ValueSome
                            {
                                Name = CodeFix.AddMissingEqualsToTypeDefinition
                                Message = title
                                Changes = [ TextChange(TextSpan(context.Span.Start, 0), "= ") ]
                            }
            }
