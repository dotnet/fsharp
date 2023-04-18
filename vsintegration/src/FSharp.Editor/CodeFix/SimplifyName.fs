// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

open FSharp.Compiler.Text

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.SimplifyName); Shared>]
type internal FSharpSimplifyNameCodeFixProvider() =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds =
        ImmutableArray.Create(FSharpIDEDiagnosticIds.SimplifyNamesDiagnosticId)

    member this.GetChangedDocument(document: Document, diagnostics: ImmutableArray<Diagnostic>, ct: CancellationToken) =
        backgroundTask {
            let! sourceText = document.GetTextAsync(ct)

            let changes =
                diagnostics |> Seq.map (fun d -> TextChange(d.Location.SourceSpan, ""))

            return document.WithText(sourceText.WithChanges(changes))
        }

    override this.RegisterCodeFixesAsync ctx : Task =
        backgroundTask {
            let diag = ctx.Diagnostics |> Seq.head

            let title =
                match diag.Properties.TryGetValue(SimplifyNameDiagnosticAnalyzer.LongIdentPropertyKey) with
                | true, longIdent -> sprintf "%s '%s'" (SR.SimplifyName()) longIdent
                | _ -> SR.SimplifyName()

            let codeAction =
                CodeAction.Create(title, (fun ct -> this.GetChangedDocument(ctx.Document, ctx.Diagnostics, ct)), title)

            ctx.RegisterCodeFix(codeAction, this.GetPrunedDiagnostics(ctx))
        }

    override this.GetFixAllProvider() =
        CodeFixHelpers.createFixAllProvider CodeFix.SimplifyName this.GetChangedDocument
