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

    /// Look up the .fsi document in the solution by file path; the path comes
    /// from the F# symbol's `SignatureLocation` which is an absolute file name.
    /// On Windows file paths are case-insensitive; elsewhere they are case-sensitive.
    let tryFindSigDocument (solution: Solution) (sigFilePath: string) =
        let pathComparison =
            if Environment.OSVersion.Platform = PlatformID.Win32NT then
                StringComparison.OrdinalIgnoreCase
            else
                StringComparison.Ordinal
        solution.Projects
        |> Seq.collect (fun p -> p.Documents)
        |> Seq.tryFind (fun d ->
            not (isNull d.FilePath)
            && String.Equals(d.FilePath, sigFilePath, pathComparison))

    /// Leading whitespace of the .fsi sig line, so the inserted attribute lines
    /// up with the declaration it attaches to.
    let indentOfLine (sigSourceText: SourceText) (lineStart: int) =
        let line = sigSourceText.Lines.GetLineFromPosition(lineStart).ToString()
        let mutable i = 0
        while i < line.Length && (line.[i] = ' ' || line.[i] = '\t') do
            i <- i + 1
        line.Substring(0, i)

    /// Line break that matches the .fsi file's existing convention - avoids
    /// inserting CRLF into an LF-only file (or vice versa).
    let lineBreakAt (sigSourceText: SourceText) (lineStart: int) =
        let line = sigSourceText.Lines.GetLineFromPosition(lineStart)
        let lbLen = line.EndIncludingLineBreak - line.End
        if lbLen > 0 then
            sigSourceText.ToString(TextSpan(line.End, lbLen))
        else
            Environment.NewLine

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS3888"

    override _.RegisterCodeFixesAsync context =
        cancellableTask {
            let document = context.Document
            let! sourceText = document.GetTextAsync(context.CancellationToken)

            // SynAttribute.Range covers ONE attribute body (e.g.
            // `NoDynamicInvocation(false)`) WITHOUT the surrounding `[< >]`
            // brackets and WITHOUT sibling attributes in a `[<A; B>]` list,
            // so wrap before inserting.
            let attribSpan = context.Span
            let attribText = sourceText.GetSubText(attribSpan).ToString()
            let bracketed = $"[<{attribText}>]"

            // Find the declaration symbol the attribute is attached to.
            // Position-based lookup is unreliable: skipping `>]`/`;`/whitespace
            // lands on `let`/`type`/`module` (keywords, no symbol use) or, in
            // multi-attribute cases like `[<A; B>]` / `[<A>]\n[<B>]`, on a
            // sibling attribute. Enumerate all symbol uses in the file and pick
            // the first definition whose range starts AFTER the diagnostic
            // attribute and which has a `SignatureLocation`.
            let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync "AddMissingAttributeToSignature"
            let diagFsRange = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, attribSpan, sourceText)

            let symbolUse =
                checkResults.GetAllUsesOfAllSymbolsInFile(context.CancellationToken)
                |> Seq.filter (fun (u: FSharp.Compiler.Symbols.FSharpSymbolUse) ->
                    u.IsFromDefinition
                    && u.Symbol.SignatureLocation.IsSome
                    && (u.Range.StartLine > diagFsRange.EndLine
                        || (u.Range.StartLine = diagFsRange.EndLine
                            && u.Range.StartColumn >= diagFsRange.EndColumn)))
                |> Seq.sortBy (fun u -> u.Range.StartLine, u.Range.StartColumn)
                |> Seq.tryHead

            match symbolUse |> Option.bind (fun u -> u.Symbol.SignatureLocation) with
            | Some sigRange ->
                match tryFindSigDocument document.Project.Solution sigRange.FileName with
                | Some sigDoc ->
                    // Title can be computed up-front; the actual offset / indent
                    // / line-break are recomputed inside the CodeAction so a
                    // .fsi edit happening between lightbulb registration and
                    // application does not desynchronise the insertion.
                    let action =
                        CodeAction.Create(
                            $"Add {bracketed} to signature",
                            (fun cancellationToken ->
                                cancellableTask {
                                    let! current = sigDoc.GetTextAsync(cancellationToken)
                                    let currentSigSpan = RoslynHelpers.FSharpRangeToTextSpan(current, sigRange)
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
