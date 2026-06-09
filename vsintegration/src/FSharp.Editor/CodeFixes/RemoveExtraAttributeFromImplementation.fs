// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text

open CancellableTasks

/// Reverse code-fix for FS3888: removes the offending attribute from the .fs.
[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveExtraAttributeFromImplementation); Shared>]
type internal RemoveExtraAttributeFromImplementationCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    // Expand the SynAttribute.Range body span to the smallest enclosing chunk we can delete:
    //   [<A>]\n            -> bracket + trailing newline + indent
    //   [<A; B>]  drop A   -> "A; "
    //   [<A; B>]  drop B   -> "; B"
    // Returns None on unrecognized layouts (e.g. multi-line `[<\nA\n>]`).
    let computeDeletionSpan (text: SourceText) (attribSpan: TextSpan) : TextSpan option =
        let s = text.ToString()

        let nextNonWhitespaceForward pos =
            let mutable i = pos

            while i < s.Length && (s.[i] = ' ' || s.[i] = '\t') do
                i <- i + 1

            i

        let nextNonWhitespaceBackward pos =
            let mutable i = pos

            while i > 0 && (s.[i - 1] = ' ' || s.[i - 1] = '\t') do
                i <- i - 1

            i

        let leftTrim = nextNonWhitespaceBackward attribSpan.Start
        let rightTrim = nextNonWhitespaceForward attribSpan.End

        let hasLeftSemi = leftTrim > 0 && s.[leftTrim - 1] = ';'
        let hasRightSemi = rightTrim < s.Length && s.[rightTrim] = ';'

        let lookingAtBracketOpen pos =
            pos >= 2 && s.[pos - 2] = '[' && s.[pos - 1] = '<'

        let lookingAtBracketClose pos =
            pos + 1 < s.Length && s.[pos] = '>' && s.[pos + 1] = ']'

        if hasLeftSemi then
            Some(TextSpan.FromBounds(leftTrim - 1, attribSpan.End))
        elif hasRightSemi then
            let mutable rs = rightTrim + 1

            while rs < s.Length && (s.[rs] = ' ' || s.[rs] = '\t') do
                rs <- rs + 1

            Some(TextSpan.FromBounds(attribSpan.Start, rs))
        elif lookingAtBracketOpen leftTrim && lookingAtBracketClose rightTrim then
            let mutable deletionStart = leftTrim - 2
            let mutable deletionEnd = rightTrim + 2

            if deletionEnd < s.Length && s.[deletionEnd] = '\r' then
                deletionEnd <- deletionEnd + 1

            if deletionEnd < s.Length && s.[deletionEnd] = '\n' then
                deletionEnd <- deletionEnd + 1

            // Only absorb indentation if the bracket is on its own line, otherwise it could be inline with other code.
            let mutable indentStart = deletionStart

            while indentStart > 0 && (s.[indentStart - 1] = ' ' || s.[indentStart - 1] = '\t') do
                indentStart <- indentStart - 1

            if indentStart = 0 || s.[indentStart - 1] = '\n' || s.[indentStart - 1] = '\r' then
                deletionStart <- indentStart

            Some(TextSpan.FromBounds(deletionStart, deletionEnd))
        else
            None

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS3888"

    override _.RegisterCodeFixesAsync context =
        cancellableTask {
            let document = context.Document
            let! sourceText = document.GetTextAsync(context.CancellationToken)

            let attribSpan = context.Span

            if attribSpan.IsEmpty then
                ()
            else
                let attribText = sourceText.GetSubText(attribSpan).ToString()

                if String.IsNullOrWhiteSpace attribText then
                    ()
                else
                    match computeDeletionSpan sourceText attribSpan with
                    | None -> ()
                    | Some deletion ->
                        let title = $"Remove [<{attribText}>] from implementation"

                        let action =
                            CodeAction.Create(
                                title,
                                System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<Document>>(fun ct ->
                                    task {
                                        let! current = document.GetTextAsync(ct)
                                        let updated = current.WithChanges(TextChange(deletion, ""))
                                        return document.WithText(updated)
                                    }),
                                equivalenceKey =
                                    $"{CodeFix.RemoveExtraAttributeFromImplementation}:{attribText}:{deletion.Start}:{deletion.End}"
                            )

                        context.RegisterCodeFix(action, context.Diagnostics)
        }
        |> CancellableTask.startAsTask context.CancellationToken
