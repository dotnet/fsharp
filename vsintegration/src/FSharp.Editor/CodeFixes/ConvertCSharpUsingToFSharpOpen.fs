// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ConvertCSharpUsingToFSharpOpen); Shared>]
type internal ConvertCSharpUsingToFSharpOpenCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.ConvertCSharpUsingToFSharpOpen()
    let usingLength = "using".Length

    let isCSharpUsingShapeWithPos (errorSpan: TextSpan) (sourceText: SourceText) =
        // Walk back until whitespace
        let mutable pos = errorSpan.Start
        let mutable ch = sourceText[pos]

        while pos > 0 && not (Char.IsWhiteSpace(ch)) do
            pos <- pos - 1
            ch <- sourceText[pos]

        // Walk back whitespace
        ch <- sourceText[pos]

        while pos > 0 && Char.IsWhiteSpace(ch) do
            pos <- pos - 1
            ch <- sourceText[pos]

        // Take 'using' slice and don't forget that offset because computer math is annoying
        let start = pos - usingLength + 1

        if start < 0 then
            false
        else
            let span = TextSpan(start, usingLength)
            let slice = sourceText.GetSubText(span).ToString()
            slice = "using"

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0039", "FS0201")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let diagnostic = context.Diagnostics[0]
                let! errorText = context.GetSquigglyTextAsync()
                let! sourceText = context.GetSourceTextAsync()

                let isValidCase =
                    match diagnostic.Id with
                    // using is included in the squiggly
                    | "FS0201" when errorText.Contains("using ") -> true
                    // using is not included in the squiggly
                    | "FS0039" when isCSharpUsingShapeWithPos context.Span sourceText -> true
                    | _ -> false

                if isValidCase then
                    let lineNumber = sourceText.Lines.GetLinePositionSpan(context.Span).Start.Line
                    let line = sourceText.Lines[lineNumber]

                    let change =
                        TextChange(line.Span, line.ToString().Replace("using", "open").Replace(";", ""))

                    return
                        ValueSome
                            {
                                Name = CodeFix.ConvertCSharpUsingToFSharpOpen
                                Message = title
                                Changes = [ change ]
                            }
                else
                    return ValueNone
            }
