// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading.Tasks

open Microsoft.VisualStudio.FSharp.Editor.Logging

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "AddMissingEqualsToTypeDefinition"); Shared>]
type internal FSharpAddMissingEqualsToTypeDefinitionCodeFixProvider() =
    inherit CodeFixProvider()

    let fixableDiagnosticIds = set ["FS3360"]

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray

            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)

            let mutable pos = context.Span.Start - 1
            
            // This won't ever actually happen, but eh why not
            do! Option.guard (pos > 0)

            let mutable ch = sourceText.[pos]
            while pos > 0 && Char.IsWhiteSpace(ch) do
                pos <- pos - 1
                ch <- sourceText.[pos]

            let title = SR.AddMissingEqualsToTypeDefinition()

            let codeFix =
                CodeFixHelpers.createTextChangeCodeFix(
                    title,
                    context,
                    // 'pos + 1' is here because 'pos' is now the position of the first non-whitespace character.
                    // Using just 'pos' will creat uncompilable code.
                    (fun () -> asyncMaybe.Return [| TextChange(TextSpan(pos + 1, 0), " =") |]))

            context.RegisterCodeFix(codeFix, diagnostics)
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken) 