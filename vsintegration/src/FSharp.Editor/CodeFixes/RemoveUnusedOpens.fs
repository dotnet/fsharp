// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

open FSharp.Compiler.Text

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

                let! unusedOpensOpt = UnusedOpensDiagnosticAnalyzer.GetUnusedOpenRanges context.Document

                return
                    unusedOpensOpt
                    |> ValueOption.map (
                        List.map (fun m ->
                            let span = sourceText.Lines[Line.toZ m.StartLine].SpanIncludingLineBreak
                            TextChange(span, ""))
                    )
                    |> ValueOption.map (fun changes ->
                        {
                            Name = CodeFix.RemoveUnusedOpens
                            Message = title
                            Changes = changes
                        })
            }
