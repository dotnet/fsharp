// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.Diagnostics

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveIndexerDotBeforeBracket); Shared>]
type internal RemoveDotFromIndexerAccessOptInCodeFixProvider() =
    inherit CodeFixProvider()

    static let title =
        CompilerDiagnostics.GetErrorMessage FSharpDiagnosticKind.RemoveIndexerDot

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS3366"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            CancellableTask.singleton (
                ValueSome
                    {
                        Name = CodeFix.RemoveIndexerDotBeforeBracket
                        Message = title
                        Changes = [ TextChange(context.Span, "") ]
                    }
            )
