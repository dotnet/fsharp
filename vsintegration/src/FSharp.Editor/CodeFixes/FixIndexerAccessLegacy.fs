// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open FSharp.Compiler.Diagnostics

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.FixIndexerAccess); Shared>]
type internal LegacyFixAddDotToIndexerAccessCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = CompilerDiagnostics.GetErrorMessage FSharpDiagnosticKind.AddIndexerDot

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS3217"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! sourceText = context.GetSourceTextAsync()

                let span, replacement =
                    try
                        let mutable span = context.Span

                        let notStartOfBracket (span: TextSpan) =
                            let t = sourceText.GetSubText(TextSpan(span.Start, span.Length + 1))
                            t[t.Length - 1] <> '['

                        // skip all braces and blanks until we find [
                        while span.End < sourceText.Length && notStartOfBracket span do
                            span <- TextSpan(span.Start, span.Length + 1)

                        span, sourceText.GetSubText(span).ToString()
                    with _ ->
                        context.Span, sourceText.GetSubText(context.Span).ToString()

                return
                    ValueSome
                        {
                            Name = CodeFix.FixIndexerAccess
                            Message = title
                            Changes = [ TextChange(span, replacement.TrimEnd() + ".") ]
                        }
            }
