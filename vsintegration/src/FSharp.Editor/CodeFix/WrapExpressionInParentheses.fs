// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddParentheses); Shared>]
type internal FSharpWrapExpressionInParenthesesFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.WrapExpressionInParentheses()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0597")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix(this)

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIsAppliesAsync _ span =
            let codeFix =
                {
                    Name = CodeFix.AddParentheses
                    Message = title
                    Changes =
                        [
                            TextChange(TextSpan(span.Start, 0), "(")
                            TextChange(TextSpan(span.End, 0), ")")
                        ]
                }

            CancellableTask.singleton (Some codeFix)
