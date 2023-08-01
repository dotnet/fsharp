// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.Diagnostics

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.ProposeUppercaseLabel); Shared>]
type internal ProposeUppercaseLabelCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0053"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! errorText = context.GetSquigglyTextAsync()

                // probably not the 100% robust way to do that
                // but actually we could also just implement the code fix for this case as well
                if errorText.StartsWith "exception " then
                    return ValueNone
                else
                    let upperCased = string (Char.ToUpper errorText[0]) + errorText.Substring(1)

                    let title =
                        CompilerDiagnostics.GetErrorMessage(FSharpDiagnosticKind.ReplaceWithSuggestion upperCased)

                    return
                        (ValueSome
                            {
                                Name = CodeFix.ProposeUppercaseLabel
                                Message = title
                                Changes = [ TextChange(TextSpan(context.Span.Start, context.Span.Length), upperCased) ]
                            })
            }
