// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveUnusedOpens); Shared>]
type internal RemoveUnusedOpensCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.RemoveUnusedOpens()

    override _.FixableDiagnosticIds =
        ImmutableArray.Create FSharpIDEDiagnosticIds.RemoveUnnecessaryImportsDiagnosticId

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! sourceText = context.GetSourceTextAsync()

                let span =
                    sourceText.Lines.GetLineFromPosition(context.Span.Start).SpanIncludingLineBreak

                return
                    ValueSome
                        {
                            Name = CodeFix.RemoveUnusedOpens
                            Message = title
                            Changes = [ TextChange(span, "") ]
                        }
            }
