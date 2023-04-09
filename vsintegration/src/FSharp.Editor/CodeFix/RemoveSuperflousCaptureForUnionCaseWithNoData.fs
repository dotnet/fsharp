// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.EditorServices

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveSuperfluousCapture); Shared>]
type internal RemoveSuperflousCaptureForUnionCaseWithNoDataProvider [<ImportingConstructor>] () =

    inherit CodeFixProvider()

    override _.FixableDiagnosticIds = Seq.toImmutableArray [ "FS0725"; "FS3548" ]

    override this.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            do! Option.guard context.Document.Project.IsFSharpCodeFixesUnusedDeclarationsEnabled

            let document = context.Document
            let! sourceText = document.GetTextAsync(context.CancellationToken)

            let! _, checkResults =
                document.GetFSharpParseAndCheckResultsAsync(nameof (RemoveSuperflousCaptureForUnionCaseWithNoDataProvider))
                |> liftAsync

            let m =
                RoslynHelpers.TextSpanToFSharpRange(document.FilePath, context.Span, sourceText)

            let classifications = checkResults.GetSemanticClassification(Some m)

            let unionCaseItem =
                classifications
                |> Array.tryFind (fun c -> c.Type = SemanticClassificationType.UnionCase)

            match unionCaseItem with
            | None -> ()
            | Some unionCaseItem ->
                // The error/warning captures entire pattern match, like "Ns.Type.DuName bindingName". We want to keep type info when suggesting a replacement, and only remove "bindingName".
                let typeInfoLength = unionCaseItem.Range.EndColumn - m.StartColumn

                let reminderSpan =
                    new TextSpan(context.Span.Start + typeInfoLength, context.Span.Length - typeInfoLength)

                this.RegisterFix(CodeFix.RemoveSuperfluousCapture, SR.RemoveUnusedBinding(), context, TextChange(reminderSpan, ""))
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
