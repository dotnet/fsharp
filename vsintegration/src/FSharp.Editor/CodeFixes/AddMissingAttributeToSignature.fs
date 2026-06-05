// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.Editor.SymbolHelpers

open CancellableTasks

/// Code-fix for FS3888 (attribute present in the .fs implementation but
/// missing from the .fsi signature). Inserts the attribute text into the
/// .fsi above the corresponding declaration. Cross-document: the diagnostic
/// is in the .fs but the edit lands in the .fsi, so the fix returns a
/// `ChangedSolution` rather than a `TextChange` against the current document.
[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddMissingAttributeToSignature); Shared>]
type internal AddMissingAttributeToSignatureCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let br = Environment.NewLine

    /// Look up the .fsi document in the solution by file path; the path comes
    /// from the F# symbol's `SignatureLocation` which is an absolute file name.
    let tryFindSigDocument (solution: Solution) (sigFilePath: string) =
        solution.Projects
        |> Seq.collect (fun p -> p.Documents)
        |> Seq.tryFind (fun d ->
            not (isNull d.FilePath)
            && String.Equals(d.FilePath, sigFilePath, StringComparison.OrdinalIgnoreCase))

    /// Indentation = leading whitespace of the target line in the .fsi, so the
    /// inserted attribute lines up with the declaration it attaches to.
    let indentOfLine (sigSourceText: SourceText) (lineStart: int) =
        let line = sigSourceText.Lines.GetLineFromPosition(lineStart).ToString()
        let mutable i = 0
        while i < line.Length && (line.[i] = ' ' || line.[i] = '\t') do
            i <- i + 1
        line.Substring(0, i)

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS3888"

    override _.RegisterCodeFixesAsync context =
        cancellableTask {
            let document = context.Document
            let! sourceText = document.GetTextAsync(context.CancellationToken)

            // The diagnostic span is the attribute text in the .fs.
            let attribSpan = context.Span
            let attribText = sourceText.GetSubText(attribSpan).ToString()

            // The symbol the attribute is attached to sits at the next
            // non-whitespace position after the attribute closing `>]`.
            let mutable pos = attribSpan.End
            while pos < sourceText.Length && Char.IsWhiteSpace(sourceText.[pos]) do
                pos <- pos + 1

            let! symbolUseOpt = getSymbolUsesOfSymbolAtLocationInDocument (document, pos)

            let sigLocation =
                symbolUseOpt
                |> Option.bind (fun uses -> uses |> Array.tryHead)
                |> Option.bind (fun u -> u.Symbol.SignatureLocation)

            match sigLocation with
            | Some sigRange ->
                match tryFindSigDocument document.Project.Solution sigRange.FileName with
                | Some sigDoc ->
                    let! sigSourceText = sigDoc.GetTextAsync(context.CancellationToken)
                    let sigSpan = RoslynHelpers.FSharpRangeToTextSpan(sigSourceText, sigRange)
                    let sigLineStart = sigSourceText.Lines.GetLineFromPosition(sigSpan.Start).Start
                    let indent = indentOfLine sigSourceText sigLineStart
                    let insertion = $"{indent}{attribText}{br}"

                    let action =
                        CodeAction.Create(
                            $"Add {attribText} to signature",
                            (fun cancellationToken ->
                                cancellableTask {
                                    let! current = sigDoc.GetTextAsync(cancellationToken)
                                    let updated = current.WithChanges(TextChange(TextSpan(sigLineStart, 0), insertion))
                                    return sigDoc.WithText(updated).Project.Solution
                                }
                                |> CancellableTask.start cancellationToken),
                            equivalenceKey = $"{CodeFix.AddMissingAttributeToSignature}:{attribText}"
                        )

                    context.RegisterCodeFix(action, context.Diagnostics)
                | None -> ()
            | None -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
