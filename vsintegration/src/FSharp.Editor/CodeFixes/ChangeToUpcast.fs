// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ChangeToUpcast); Shared>]
type internal ChangeToUpcastCodeFixProvider() =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS3198")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix(this)

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync document span =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()

                let! sourceText = document.GetTextAsync(cancellationToken)
                let text = sourceText.GetSubText(span).ToString()

                // Only works if it's one or the other
                let isDowncastOperator = text.Contains(":?>")
                let isDowncastKeyword = text.Contains("downcast")

                if
                    (isDowncastOperator || isDowncastKeyword)
                    && not (isDowncastOperator && isDowncastKeyword)
                then
                    let replacement =
                        if isDowncastOperator then
                            text.Replace(":?>", ":>")
                        else
                            text.Replace("downcast", "upcast")

                    let changes = [ TextChange(span, replacement) ]

                    let title =
                        if isDowncastOperator then
                            SR.UseUpcastOperator()
                        else
                            SR.UseUpcastKeyword()

                    let codeFix =
                        {
                            Name = CodeFix.ChangeToUpcast
                            Message = title
                            Changes = changes
                        }

                    return Some codeFix
                else
                    return None
            }
