// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddInstanceMemberParameter); Shared>]
type internal FSharpAddInstanceMemberParameterCodeFixProvider() =
    inherit CodeFixProvider()

    static let title = SR.AddMissingInstanceMemberParameter()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0673")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix(this)

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync _ span =
            let codeFix =
                {
                    Name = CodeFix.AddInstanceMemberParameter
                    Message = title
                    Changes = [ TextChange(TextSpan(span.Start, 0), "x.") ]
                }

            CancellableTask.singleton (Some codeFix)
