// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveUnusedBinding); Shared>]
type internal RemoveUnusedBindingCodeFixProvider [<ImportingConstructor>] () =

    inherit CodeFixProvider()

    static let title = SR.RemoveUnusedBinding()

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS1182"

    override this.RegisterCodeFixesAsync context =
        if context.Document.Project.IsFSharpCodeFixesUnusedDeclarationsEnabled then
            context.RegisterFsharpFix this
        else
            Task.CompletedTask

    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! token = CancellableTask.getCancellationToken ()

                let! sourceText = context.Document.GetTextAsync token
                let! parseResults = context.Document.GetFSharpParseResultsAsync(nameof RemoveUnusedBindingCodeFixProvider)

                let change =
                    let bindingRangeOpt =
                        RoslynHelpers.TextSpanToFSharpRange(context.Document.FilePath, context.Span, sourceText)
                        |> fun r -> parseResults.TryRangeOfBindingWithHeadPatternWithPos(r.Start)

                    match bindingRangeOpt with
                    | Some(Expression range) ->
                        let span = RoslynHelpers.FSharpRangeToTextSpan(sourceText, range)

                        let keywordEndColumn =
                            let rec loop ch pos =
                                if not (Char.IsWhiteSpace(ch)) then
                                    pos
                                else
                                    loop sourceText.[pos - 1] (pos - 1)

                            loop sourceText.[span.Start - 1] (span.Start - 1)

                        let keywordStartColumn = keywordEndColumn - 2 // removes 'let' or 'use'
                        let fullSpan = TextSpan(keywordStartColumn, span.End - keywordStartColumn)

                        ValueSome(TextChange(fullSpan, ""))

                    | Some(SelfId range) ->
                        let span = RoslynHelpers.FSharpRangeToTextSpan(sourceText, range)

                        let rec findAs index (str: SourceText) =
                            if str[index] <> ' ' then index else findAs (index - 1) str

                        let rec findEqual index (str: SourceText) =
                            if str[index] <> ' ' then
                                index
                            else
                                findEqual (index + 1) str

                        let asStart = findAs (span.Start - 1) sourceText - 1
                        let equalStart = findEqual span.End sourceText

                        let fullSpan = TextSpan(asStart, equalStart - asStart)

                        ValueSome(TextChange(fullSpan, ""))

                    | Some Member -> ValueNone

                    | None -> ValueNone

                return
                    change
                    |> ValueOption.map (fun change ->
                        {
                            Name = CodeFix.RemoveUnusedBinding
                            Message = title
                            Changes = [ change ]
                        })
            }
