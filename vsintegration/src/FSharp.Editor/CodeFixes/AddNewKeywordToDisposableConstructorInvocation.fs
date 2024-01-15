// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddNewKeyword); Shared>]
type internal AddNewKeywordCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.AddNewKeyword()

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0760"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            CancellableTask.singleton (
                ValueSome
                    {
                        Name = CodeFix.AddNewKeyword
                        Message = title
                        Changes = [ TextChange(TextSpan(context.Span.Start, 0), "new ") ]
                    }
            )
