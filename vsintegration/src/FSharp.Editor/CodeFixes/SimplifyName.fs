// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.SimplifyName); Shared>]
type internal SimplifyNameCodeFixProvider() =
    inherit CodeFixProvider()

    override _.FixableDiagnosticIds =
        ImmutableArray.Create(FSharpIDEDiagnosticIds.SimplifyNamesDiagnosticId)

    member this.GetChanges(_document: Document, diagnostics: ImmutableArray<Diagnostic>, _ct: CancellationToken) =
        backgroundTask {
            let changes =
                diagnostics |> Seq.map (fun d -> TextChange(d.Location.SourceSpan, ""))

            return changes
        }

    override this.RegisterCodeFixesAsync ctx : Task =
        backgroundTask {
            let diag = ctx.Diagnostics |> Seq.head

            let title =
                match diag.Properties.TryGetValue(SimplifyNameDiagnosticAnalyzer.LongIdentPropertyKey) with
                | true, longIdent -> sprintf "%s '%s'" (SR.SimplifyName()) longIdent
                | _ -> SR.SimplifyName()

            let! changes = this.GetChanges(ctx.Document, ctx.Diagnostics, ctx.CancellationToken)
            ctx.RegisterFsharpFix(CodeFix.SimplifyName, title, changes)
        }

    override this.GetFixAllProvider() =
        CodeFixHelpers.createFixAllProvider CodeFix.SimplifyName this.GetChanges
