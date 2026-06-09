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

/// Reverse code-fix for FS3888 (attribute present in the .fs implementation but
/// missing from the .fsi signature). Same-document edit: removes the offending
/// attribute from the .fs so the impl matches the .fsi contract.
///
/// Complements AddMissingAttributeToSignatureCodeFixProvider. The user gets two
/// lightbulbs: "Add to signature" (preserve the impl's intent) and "Remove from
/// implementation" (treat the .fsi as the source of truth - the right choice
/// when the .fs attribute was sloppily added and has no consumer effect anyway).
[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveExtraAttributeFromImplementation); Shared>]
type internal RemoveExtraAttributeFromImplementationCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    /// The diagnostic span covers ONE attribute body
    /// (e.g. `NoDynamicInvocation(false)`) WITHOUT the surrounding `[< >]`
    /// brackets and WITHOUT sibling separators.
    /// Expand it textually to the smallest enclosing chunk we can delete:
    ///   - `[<A>]\n`              -> delete the whole bracket and the trailing newline + indent
    ///   - `[<A; B>]` (delete A)  -> delete `A; ` keep `[<B>]`
    ///   - `[<A; B>]` (delete B)  -> delete `; B` keep `[<A>]`
    let computeDeletionSpan (text: SourceText) (attribSpan: TextSpan) : TextSpan option =
        let s = text.ToString()

        // Scan immediately around the body (without walking through whitespace
        // first - the cases we recognize all have `;` or `<`/`>` directly
        // adjacent to the body or separated by spaces only on one side).
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
            // `[<A; B>]` deleting B: delete from the `;` (inclusive) through
            // the end of the body. Keeps the trailing `>]`. Pre-`;` text is
            // untouched so siblings before are intact.
            Some(TextSpan.FromBounds(leftTrim - 1, attribSpan.End))
        elif hasRightSemi then
            // `[<A; B>]` deleting A: delete from the start of the body through
            // the `;` (inclusive) and any following whitespace before the next
            // sibling. Keeps the leading `[<`.
            let mutable rs = rightTrim + 1 // past the `;`

            while rs < s.Length && (s.[rs] = ' ' || s.[rs] = '\t') do
                rs <- rs + 1

            Some(TextSpan.FromBounds(attribSpan.Start, rs))
        elif lookingAtBracketOpen leftTrim && lookingAtBracketClose rightTrim then
            // Lone `[<X>]`: delete `[<X>]` and one trailing line break so we
            // don't leave a blank line behind. Also absorb any indentation
            // on the same line so the result has no dangling whitespace.
            let mutable deletionStart = leftTrim - 2 // include `[<`
            let mutable deletionEnd = rightTrim + 2 // include `>]`

            if deletionEnd < s.Length && s.[deletionEnd] = '\r' then
                deletionEnd <- deletionEnd + 1

            if deletionEnd < s.Length && s.[deletionEnd] = '\n' then
                deletionEnd <- deletionEnd + 1
            // Look back through leading spaces/tabs only if the bracket is at
            // line start (preceded by a newline). Otherwise keep them - the
            // bracket might be inline with other code.
            let mutable indentStart = deletionStart

            while indentStart > 0 && (s.[indentStart - 1] = ' ' || s.[indentStart - 1] = '\t') do
                indentStart <- indentStart - 1

            if indentStart = 0 || s.[indentStart - 1] = '\n' || s.[indentStart - 1] = '\r' then
                deletionStart <- indentStart

            Some(TextSpan.FromBounds(deletionStart, deletionEnd))
        else
            // Unrecognized bracket layout (e.g. multi-line `[<\nA\n>]`).
            // Decline rather than risk a corrupt edit.
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
