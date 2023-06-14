// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ChangeToUpcast); Shared>]
type internal FSharpChangeToUpcastCodeFixProvider() =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS3198")

    interface IFSharpCodeFix with
        member _.GetChangesAsync document span =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()

                let! sourceText = document.GetTextAsync(cancellationToken)
                let text = sourceText.GetSubText(span).ToString()

                // Only works if it's one or the other
                let isDowncastOperator = text.Contains(":?>")
                let isDowncastKeyword = text.Contains("downcast")

                let changes =
                    [
                        if
                            (isDowncastOperator || isDowncastKeyword)
                            && not (isDowncastOperator && isDowncastKeyword)
                        then
                            let replacement =
                                if isDowncastOperator then
                                    text.Replace(":?>", ":>")
                                else
                                    text.Replace("downcast", "upcast")

                            TextChange(span, replacement)
                    ]

                let title =
                    if isDowncastOperator then
                        SR.UseUpcastOperator()
                    else
                        SR.UseUpcastKeyword()

                return title, changes
            }

    override this.RegisterCodeFixesAsync context : Task =
        cancellableTask {
            let! title, changes = (this :> IFSharpCodeFix).GetChangesAsync context.Document context.Span
            context.RegisterFsharpFix(CodeFix.ChangeToUpcast, title, changes)
        }
        |> CancellableTask.startAsTask context.CancellationToken
