// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ChangeEqualsInFieldTypeToColon)>]
type internal ChangeEqualsInFieldTypeToColonCodeFixProvider() =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0010")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix(this)

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync _ span =
            cancellableTask {
                let codeFix = 
                    {
                        Name = CodeFix.ChangeEqualsInFieldTypeToColon
                        Message = SR.ChangeEqualsInFieldTypeToColon()
                        Changes = [ TextChange(TextSpan(span.Start, 1), ":")]
                    }
                return (Some codeFix)
            }