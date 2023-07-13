// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Text.RegularExpressions

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ChangePrefixNegationToInfixSubtraction); Shared>]
type internal ChangePrefixNegationToInfixSubtractionCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.ChangePrefixNegationToInfixSubtraction()

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0003"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! sourceText = context.GetSourceTextAsync()

                // in a line like "... x  -1 ...",
                // squiggly goes for "x", not for "-", hence we search for "-"
                let remainingText = $"{sourceText.GetSubText(context.Span.End)}"
                let pattern = @"^\s+(-)"

                match Regex.Match(remainingText, pattern) with
                | m when m.Success ->
                    let spacePlace = context.Span.End + m.Groups[1].Index + 1

                    return
                        Some
                            {
                                Name = CodeFix.ChangePrefixNegationToInfixSubtraction
                                Message = title
                                Changes = [ TextChange(TextSpan(spacePlace, 0), " ") ]
                            }
                | _ -> return None
            }
