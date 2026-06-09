// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.Symbols

open CancellableTasks

/// Code-fix for FS3888: inserts the missing attribute into the .fsi above the
/// matching declaration. Cross-document fix (diagnostic in .fs, edit in .fsi).
[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddMissingAttributeToSignature); Shared>]
type internal AddMissingAttributeToSignatureCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    // Path normalized to handle slash/case/relative differences between FCS and Roslyn.
    let tryFindSigDocument (document: Document) (sigFilePath: string) =
        let solution = document.Project.Solution

        let normalizedPath =
            try
                System.IO.Path.GetFullPath(sigFilePath)
            with _ ->
                sigFilePath

        let docIds = solution.GetDocumentIdsWithFilePath(normalizedPath)

        if docIds.IsEmpty then
            None
        else
            let preferred = docIds |> Seq.tryFind (fun id -> id.ProjectId = document.Project.Id)

            (preferred |> Option.defaultValue docIds.[0])
            |> solution.GetDocument
            |> Option.ofObj

    // `Conditional("DEBUG")` -> `("Conditional", "(\"DEBUG\")")`.
    let splitAttribHead (text: string) : struct (string * string) =
        let mutable i = 0

        while i < text.Length && text.[i] <> '(' && not (Char.IsWhiteSpace(text.[i])) do
            i <- i + 1

        struct (text.Substring(0, i), text.Substring(i))

    // Negative-lookbehind guards against re-qualifying already-qualified or substring-of-longer-identifier tokens.
    let qualifyEnumToken (simple: string) (qualified: string) (text: string) : string =
        let pattern = $@"(?<![\w\.]){System.Text.RegularExpressions.Regex.Escape(simple)}\."
        let replacement = qualified + "."
        System.Text.RegularExpressions.Regex.Replace(text, pattern, replacement)

    // Auto-qualify well-known attribute names and enum args so the inserted .fsi
    // text compiles without requiring extra opens.
    let canonicalizeAttribName (attribText: string) : string =
        let struct (head, rest) = splitAttribHead attribText

        let qualifiedHead =
            if head.Contains(".") then
                head
            else
                match head with
                | "Conditional"
                | "ConditionalAttribute" -> "System.Diagnostics." + head
                | "EditorBrowsable"
                | "EditorBrowsableAttribute" -> "System.ComponentModel." + head
                | "NoEagerConstraintApplication"
                | "NoEagerConstraintApplicationAttribute" -> "Microsoft.FSharp.Core.CompilerServices." + head
                | "Obsolete"
                | "ObsoleteAttribute" -> "System." + head
                | "AttributeUsage"
                | "AttributeUsageAttribute" -> "System." + head
                | "Unverifiable"
                | "UnverifiableAttribute" -> "Microsoft.FSharp.Core.CompilerServices." + head
                | _ -> head

        let qualifiedRest =
            rest
            |> qualifyEnumToken "EditorBrowsableState" "System.ComponentModel.EditorBrowsableState"
            |> qualifyEnumToken "AttributeTargets" "System.AttributeTargets"

        qualifiedHead + qualifiedRest

    let indentOfLine (sigSourceText: SourceText) (lineStart: int) =
        let line = sigSourceText.Lines.GetLineFromPosition(lineStart).ToString()
        let mutable i = 0

        while i < line.Length && (line.[i] = ' ' || line.[i] = '\t') do
            i <- i + 1

        line.Substring(0, i)

    // Reuse the .fsi's existing newline convention; if the target line has no
    // terminator, walk backward to find one before falling back to Environment.NewLine.
    let lineBreakAt (sigSourceText: SourceText) (lineStart: int) =
        let inline lineBreakOf (line: TextLine) =
            let lbLen = line.EndIncludingLineBreak - line.End

            if lbLen > 0 then
                Some(sigSourceText.ToString(TextSpan(line.End, lbLen)))
            else
                None

        let lines = sigSourceText.Lines
        let startLineNo = lines.GetLineFromPosition(lineStart).LineNumber
        let mutable result: string option = None
        let mutable i = startLineNo

        while result.IsNone && i >= 0 do
            result <- lineBreakOf lines.[i]
            i <- i - 1

        result |> Option.defaultValue Environment.NewLine

    // Bails out if the .fsi was truncated between registration and apply.
    let tryFSharpRangeToTextSpan (text: SourceText) (range: FSharp.Compiler.Text.range) =
        try
            Some(RoslynHelpers.FSharpRangeToTextSpan(text, range))
        with
        | :? ArgumentOutOfRangeException
        | :? IndexOutOfRangeException -> None

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS3888"

    override _.RegisterCodeFixesAsync context =
        cancellableTask {
            let document = context.Document
            let! sourceText = document.GetTextAsync(context.CancellationToken)

            // SynAttribute.Range covers one attribute body without `[<` `>]` or sibling separators.
            let attribSpan = context.Span
            let rawAttribText = sourceText.GetSubText(attribSpan).ToString()

            if String.IsNullOrWhiteSpace rawAttribText then
                ()
            else

                let attribText = canonicalizeAttribName rawAttribText
                let bracketed = $"[<{attribText}>]"

                // Position-based lookup is unreliable in `[<A; B>]` / `[<A>]\n[<B>]`
                // (lands on sibling attribute or on `let`/`type`/`module`). Enumerate
                // symbol uses and pick the first definition starting after the diagnostic
                // whose SignatureLocation points into a different file (the .fsi).
                let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync "AddMissingAttributeToSignature"

                let diagFsRange =
                    RoslynHelpers.TextSpanToFSharpRange(document.FilePath, attribSpan, sourceText)

                let candidates =
                    checkResults.GetAllUsesOfAllSymbolsInFile(context.CancellationToken)
                    |> Seq.filter (fun (u: FSharp.Compiler.CodeAnalysis.FSharpSymbolUse) ->
                        u.IsFromDefinition
                        && u.Symbol.SignatureLocation.IsSome
                        // SignatureLocation must point into the .fsi, not back at the .fs.
                        // The wildcard self-identifier `_` in `member _.F = ...` is reported
                        // as a definition whose SignatureLocation is its own .fs position.
                        && (match u.Symbol.SignatureLocation with
                            | Some sigLoc -> not (String.Equals(sigLoc.FileName, document.FilePath, StringComparison.OrdinalIgnoreCase))
                            | None -> false)
                        // Skip the implicit constructor of `type T() = ...`: its
                        // SignatureLocation points at `new: ...`, not at the member.
                        && (match u.Symbol with
                            | :? FSharpMemberOrFunctionOrValue as mfv -> not mfv.IsConstructor
                            | _ -> true)
                        && (u.Range.StartLine > diagFsRange.EndLine
                            || (u.Range.StartLine = diagFsRange.EndLine
                                && u.Range.StartColumn >= diagFsRange.EndColumn)))
                    |> Seq.toArray

                let symbolUse =
                    if candidates.Length = 0 then
                        None
                    else
                        // Tie-break by symbol name for determinism across overloads / type+ctor pairs.
                        candidates
                        |> Array.minBy (fun u ->
                            u.Range.StartLine, u.Range.StartColumn, u.Range.EndLine, u.Range.EndColumn, u.Symbol.FullName)
                        |> Some

                match symbolUse |> Option.bind (fun u -> u.Symbol.SignatureLocation) with
                | Some sigRange when not (String.Equals(sigRange.FileName, document.FilePath, StringComparison.OrdinalIgnoreCase)) ->
                    match tryFindSigDocument document sigRange.FileName with
                    | Some sigDoc ->
                        // Keep the DocumentId, not the Document: re-resolve at apply
                        // time so intervening .fsi edits are observed.
                        let sigDocId = sigDoc.Id

                        let normalizedSigPath =
                            try
                                System.IO.Path.GetFullPath(sigRange.FileName)
                            with _ ->
                                sigRange.FileName

                        let createChangedSolution
                            (cancellationToken: System.Threading.CancellationToken)
                            : System.Threading.Tasks.Task<Solution> =
                            task {
                                let currentSolution = document.Project.Solution

                                match currentSolution.GetDocument(sigDocId) |> Option.ofObj with
                                | None -> return currentSolution
                                | Some liveSigDoc ->
                                    let! current = liveSigDoc.GetTextAsync(cancellationToken)

                                    match tryFSharpRangeToTextSpan current sigRange with
                                    | None -> return currentSolution
                                    | Some currentSigSpan ->
                                        let currentLineStart = current.Lines.GetLineFromPosition(currentSigSpan.Start).Start
                                        let currentIndent = indentOfLine current currentLineStart
                                        let currentLineBreak = lineBreakAt current currentLineStart
                                        let currentInsertion = $"{currentIndent}{bracketed}{currentLineBreak}"

                                        let updated =
                                            current.WithChanges(TextChange(TextSpan(currentLineStart, 0), currentInsertion))

                                        return liveSigDoc.WithText(updated).Project.Solution
                            }

                        let action =
                            CodeAction.Create(
                                $"Add {bracketed} to signature",
                                System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<Solution>>(
                                    createChangedSolution
                                ),
                                equivalenceKey =
                                    $"{CodeFix.AddMissingAttributeToSignature}:{bracketed}:{normalizedSigPath}:{sigRange.StartLine}:{sigRange.StartColumn}:{sigRange.EndLine}:{sigRange.EndColumn}"
                            )

                        context.RegisterCodeFix(action, context.Diagnostics)
                    | None -> ()
                | Some _
                | None -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
