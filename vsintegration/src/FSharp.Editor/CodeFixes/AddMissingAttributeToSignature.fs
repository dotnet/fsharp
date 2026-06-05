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

/// Code-fix for FS3888 (attribute present in the .fs implementation but
/// missing from the .fsi signature). Inserts the attribute text into the
/// .fsi above the corresponding declaration. Cross-document: the diagnostic
/// is in the .fs but the edit lands in the .fsi, so the fix returns a
/// `ChangedSolution` rather than a `TextChange` against the current document.
[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddMissingAttributeToSignature); Shared>]
type internal AddMissingAttributeToSignatureCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    /// Look up the .fsi document in the solution by file path; uses Roslyn's
    /// path index instead of walking projects/documents linearly. Prefer the
    /// current document's project first (typical case - same project), then
    /// fall back to any project that contains a document with the path.
    let tryFindSigDocument (document: Document) (sigFilePath: string) =
        let solution = document.Project.Solution
        let docIds = solution.GetDocumentIdsWithFilePath(sigFilePath)
        if docIds.IsEmpty then None
        else
            // Prefer same project for cross-project independence.
            let preferred =
                docIds
                |> Seq.tryFind (fun id -> id.ProjectId = document.Project.Id)
            (preferred |> Option.defaultValue docIds.[0])
            |> solution.GetDocument
            |> Option.ofObj

    /// Leading whitespace of the .fsi sig line, so the inserted attribute lines
    /// up with the declaration it attaches to.
    let indentOfLine (sigSourceText: SourceText) (lineStart: int) =
        let line = sigSourceText.Lines.GetLineFromPosition(lineStart).ToString()
        let mutable i = 0
        while i < line.Length && (line.[i] = ' ' || line.[i] = '\t') do
            i <- i + 1
        line.Substring(0, i)

    /// Line break that matches the .fsi file's existing convention - avoids
    /// inserting CRLF into an LF-only file (or vice versa). If the target line
    /// has no trailing newline (last line of file with no trailing newline),
    /// walks PREVIOUS lines to find one and reuse it; only falls back to
    /// `Environment.NewLine` if the file contains no line breaks at all.
    let lineBreakAt (sigSourceText: SourceText) (lineStart: int) =
        let inline lineBreakOf (line: TextLine) =
            let lbLen = line.EndIncludingLineBreak - line.End
            if lbLen > 0 then
                Some (sigSourceText.ToString(TextSpan(line.End, lbLen)))
            else
                None
        let lines = sigSourceText.Lines
        let startLineNo = lines.GetLineFromPosition(lineStart).LineNumber
        let mutable result : string option = None
        let mutable i = startLineNo
        while result.IsNone && i >= 0 do
            result <- lineBreakOf lines.[i]
            i <- i - 1
        result |> Option.defaultValue Environment.NewLine

    /// Canonicalize attribute names whose `[<X>]` form in the .fs requires an
    /// `open` of a namespace that the .fsi may not have. For these names we
    /// emit the fully-qualified form so the inserted .fsi compiles regardless.
    /// Only applied to bare (un-dotted) attribute names - if the user already
    /// wrote a qualified name we leave it alone.
    let canonicalizeAttribName (attribText: string) : string =
        if attribText.Contains(".") then attribText
        else
            let knownFqns =
                [
                    "Conditional",                  "System.Diagnostics.Conditional"
                    "EditorBrowsable",              "System.ComponentModel.EditorBrowsable"
                    "NoEagerConstraintApplication", "Microsoft.FSharp.Core.CompilerServices.NoEagerConstraintApplication"
                ]
            let matches (simple: string) =
                attribText = simple
                || attribText = simple + "Attribute"
                || attribText.StartsWith(simple + "(", StringComparison.Ordinal)
                || attribText.StartsWith(simple + "Attribute(", StringComparison.Ordinal)
            knownFqns
            |> List.tryPick (fun (simple, full) ->
                if matches simple then Some (full + attribText.Substring(simple.Length))
                else None)
            |> Option.defaultValue attribText

    /// Safe wrapper around `FSharpRangeToTextSpan` that returns None when the
    /// range is out of bounds (e.g. .fsi was truncated between registration
    /// and apply). OperationCanceledException must not be swallowed.
    let tryFSharpRangeToTextSpan (text: SourceText) (range: FSharp.Compiler.Text.range) =
        try Some (RoslynHelpers.FSharpRangeToTextSpan(text, range))
        with
        | :? ArgumentOutOfRangeException
        | :? IndexOutOfRangeException -> None

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS3888"

    override _.RegisterCodeFixesAsync context =
        cancellableTask {
            let document = context.Document
            let! sourceText = document.GetTextAsync(context.CancellationToken)

            // SynAttribute.Range covers ONE attribute body (e.g.
            // `NoDynamicInvocation(false)`) WITHOUT the surrounding `[< >]`
            // brackets and WITHOUT sibling attributes in a `[<A; B>]` list,
            // so wrap before inserting. Canonicalize known non-default names
            // so the inserted .fsi compiles without extra opens.
            let attribSpan = context.Span
            let rawAttribText = sourceText.GetSubText(attribSpan).ToString()

            if String.IsNullOrWhiteSpace rawAttribText then () else

            let attribText = canonicalizeAttribName rawAttribText
            let bracketed = $"[<{attribText}>]"

            // Find the declaration symbol the attribute is attached to.
            // Position-based lookup is unreliable: skipping `>]`/`;`/whitespace
            // lands on `let`/`type`/`module` (keywords, no symbol use) or, in
            // multi-attribute cases like `[<A; B>]` / `[<A>]\n[<B>]`, on a
            // sibling attribute. Enumerate all symbol uses in the file and pick
            // the FIRST definition by source position (single O(N) min, no
            // sort) whose range starts AFTER the diagnostic attribute and
            // which has a `SignatureLocation`.
            let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync "AddMissingAttributeToSignature"
            let diagFsRange = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, attribSpan, sourceText)

            let symbolUse =
                let candidates =
                    checkResults.GetAllUsesOfAllSymbolsInFile(context.CancellationToken)
                    |> Seq.filter (fun (u: FSharp.Compiler.Symbols.FSharpSymbolUse) ->
                        u.IsFromDefinition
                        && u.Symbol.SignatureLocation.IsSome
                        && (u.Range.StartLine > diagFsRange.EndLine
                            || (u.Range.StartLine = diagFsRange.EndLine
                                && u.Range.StartColumn >= diagFsRange.EndColumn)))
                if Seq.isEmpty candidates then None
                else Some (candidates |> Seq.minBy (fun u -> u.Range.StartLine, u.Range.StartColumn))

            match symbolUse |> Option.bind (fun u -> u.Symbol.SignatureLocation) with
            | Some sigRange ->
                match tryFindSigDocument document sigRange.FileName with
                | Some sigDoc ->
                    // The actual offset / indent / line-break are recomputed
                    // inside the CodeAction so a .fsi edit happening between
                    // lightbulb registration and application does not
                    // desynchronise the insertion. The .fsi span lookup is
                    // wrapped to bail out gracefully if the file was truncated.
                    let action =
                        CodeAction.Create(
                            $"Add {bracketed} to signature",
                            (fun cancellationToken ->
                                cancellableTask {
                                    let! current = sigDoc.GetTextAsync(cancellationToken)
                                    match tryFSharpRangeToTextSpan current sigRange with
                                    | None -> return sigDoc.Project.Solution
                                    | Some currentSigSpan ->
                                        let currentLineStart = current.Lines.GetLineFromPosition(currentSigSpan.Start).Start
                                        let currentIndent = indentOfLine current currentLineStart
                                        let currentLineBreak = lineBreakAt current currentLineStart
                                        let currentInsertion = $"{currentIndent}{bracketed}{currentLineBreak}"
                                        let updated = current.WithChanges(TextChange(TextSpan(currentLineStart, 0), currentInsertion))
                                        return sigDoc.WithText(updated).Project.Solution
                                }
                                |> CancellableTask.start cancellationToken),
                            equivalenceKey =
                                $"{CodeFix.AddMissingAttributeToSignature}:{bracketed}:{sigRange.FileName}:{sigRange.StartLine}:{sigRange.StartColumn}:{sigRange.EndLine}:{sigRange.EndColumn}"
                        )

                    context.RegisterCodeFix(action, context.Diagnostics)
                | None -> ()
            | None -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
