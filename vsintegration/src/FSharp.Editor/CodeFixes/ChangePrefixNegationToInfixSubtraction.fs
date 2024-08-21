// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ChangePrefixNegationToInfixSubtraction); Shared>]
type internal ChangePrefixNegationToInfixSubtractionCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.ChangePrefixNegationToInfixSubtraction()

    static let rec findNextNonWhitespacePos (sourceText: SourceText) pos =
        if pos < sourceText.Length - 1 then
            if Char.IsWhiteSpace sourceText[pos] then
                findNextNonWhitespacePos sourceText (pos + 1)
            else
                ValueSome pos
        else
            ValueNone

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0003"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! sourceText = context.GetSourceTextAsync()

                // in a line like "... x  -1 ...",
                // squiggly goes for "x", not for "-", hence we search for "-"
                let fix =
                    findNextNonWhitespacePos sourceText (context.Span.End + 1)
                    |> ValueOption.filter (fun pos -> sourceText[pos] = '-')
                    |> ValueOption.map (fun pos ->
                        {
                            Name = CodeFix.ChangePrefixNegationToInfixSubtraction
                            Message = title
                            Changes = [ TextChange(TextSpan(pos + 1, 0), " ") ]
                        })

                return fix
            }
