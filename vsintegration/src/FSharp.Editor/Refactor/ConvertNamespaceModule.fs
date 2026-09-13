// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.CodeAnalysis.CodeRefactorings
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Text

open CancellableTasks

[<RequireQualifiedAccess>]
module private NamespaceModuleConversion =

    [<NoComparison; NoEquality>]
    type Shape =
        | Nested of
            namespacePath: LongIdent *
            namespaceIsRecursive: bool *
            namespaceKeyword: range *
            opens: range list *
            moduleIdent: Ident *
            moduleIsRecursive: bool *
            moduleKeyword: range *
            equals: range *
            bodyColumn: int
        | Root of path: LongIdent * keyword: range * moduleRange: range

    let spanOf (sourceText: SourceText) (m: range) =
        RoslynHelpers.FSharpRangeToTextSpan(sourceText, m)

    let textBetween (sourceText: SourceText) (first: Ident) (last: Ident) =
        sourceText.ToString(TextSpan.FromBounds((spanOf sourceText first.idRange).Start, (spanOf sourceText last.idRange).End))

    let isBlank (sourceText: SourceText) start finish =
        String.IsNullOrWhiteSpace(sourceText.ToString(TextSpan.FromBounds(start, finish)))

    let restOfLineIsBlank (sourceText: SourceText) (m: range) =
        let position = (spanOf sourceText m).End
        isBlank sourceText position (sourceText.Lines.GetLineFromPosition position).End

    /// The declarations of a namespace are a single nested module, optionally preceded by `open`s that would
    /// apply equally to a root module: moving them into it changes nothing they resolve against.
    let private tryOpensBeforeNestedModule (decls: SynModuleDecl list) =
        match List.rev decls with
        | nestedModule :: reversedOpens when
            reversedOpens
            |> List.forall (function
                | SynModuleDecl.Open _ -> true
                | _ -> false)
            ->
            match nestedModule with
            | SynModuleDecl.NestedModule _ -> Some(reversedOpens |> List.rev |> List.map _.Range, nestedModule)
            | _ -> None
        | _ -> None

    let tryShape (sourceText: SourceText) (parseTree: ParsedInput) =
        match parseTree with
        | ParsedInput.ImplFile(ParsedImplFileInput(
            contents = [ SynModuleOrNamespace(
                             longId = namespacePath
                             isRecursive = namespaceIsRecursive
                             kind = SynModuleOrNamespaceKind.DeclaredNamespace
                             decls = decls
                             trivia = namespaceTrivia) ])) ->
            match tryOpensBeforeNestedModule decls with
            | Some(opens,
                   SynModuleDecl.NestedModule(
                       moduleInfo = moduleInfo
                       isRecursive = moduleIsRecursive
                       decls = (firstDeclaration :: _ as declarations)
                       range = moduleRange
                       trivia = moduleTrivia)) ->
                match namespaceTrivia.LeadingKeyword, moduleInfo.LongIdent, moduleTrivia.ModuleKeyword, moduleTrivia.EqualsRange with
                | SynModuleOrNamespaceLeadingKeyword.Namespace namespaceKeyword, [ moduleIdent ], Some moduleKeyword, Some equals when
                    namespaceKeyword.StartColumn = 0
                    && moduleKeyword.StartColumn = 0
                    && moduleKeyword.StartLine = equals.EndLine
                    && firstDeclaration.Range.StartLine > equals.EndLine
                    && Position.posEq moduleRange.End (List.last declarations).Range.End
                    && restOfLineIsBlank sourceText (List.last namespacePath).idRange
                    ->
                    ValueSome(
                        Nested(
                            namespacePath,
                            namespaceIsRecursive,
                            namespaceKeyword,
                            opens,
                            moduleIdent,
                            moduleIsRecursive,
                            moduleKeyword,
                            equals,
                            firstDeclaration.Range.StartColumn
                        )
                    )
                | _ -> ValueNone
            | _ -> ValueNone

        | ParsedInput.ImplFile(ParsedImplFileInput(
            contents = [ SynModuleOrNamespace(
                             longId = (_ :: _ :: _ as path)
                             kind = SynModuleOrNamespaceKind.NamedModule
                             decls = firstDeclaration :: _
                             range = moduleRange
                             trivia = moduleTrivia) ])) ->
            match moduleTrivia.LeadingKeyword with
            | SynModuleOrNamespaceLeadingKeyword.Module keyword when
                keyword.StartColumn = 0
                && (List.last path).idRange.EndLine = keyword.StartLine
                && firstDeclaration.Range.StartLine > keyword.StartLine
                ->
                ValueSome(Root(path, keyword, moduleRange))
            | _ -> ValueNone

        | _ -> ValueNone

    let isOnHeader (caretLine: int) shape =
        match shape with
        | Nested(namespaceKeyword = namespaceKeyword; moduleKeyword = moduleKeyword) ->
            caretLine = namespaceKeyword.StartLine || caretLine = moduleKeyword.StartLine
        | Root(keyword = keyword) -> caretLine = keyword.StartLine

    let dotted (idents: LongIdent) =
        idents |> List.map _.idText |> String.concat "."

    let title shape =
        match shape with
        | Nested(namespacePath = namespacePath; moduleIdent = moduleIdent) ->
            String.Format(SR.ConvertToRootModule(), $"{dotted namespacePath}.{moduleIdent.idText}")
        | Root(path = path) ->
            String.Format(SR.ConvertToNamespaceWithNestedModule(), dotted (List.take (path.Length - 1) path), (List.last path).idText)

    let linesInsideLiterals (parseTree: ParsedInput) =
        (HashSet<int>(), parseTree)
        ||> ParsedInput.fold (fun lines _ node ->
            match node with
            | SyntaxNode.SynExpr(SynExpr.Const(range = m))
            | SyntaxNode.SynExpr(SynExpr.InterpolatedString(range = m))
            | SyntaxNode.SynPat(SynPat.Const(range = m)) ->
                for line in m.StartLine + 1 .. m.EndLine do
                    lines.Add(Line.toZ line) |> ignore
            | _ -> ()

            lines)

    let leadingSpaces (sourceText: SourceText) (line: TextLine) =
        let mutable position = line.Start

        while position < line.End && sourceText[position] = ' ' do
            position <- position + 1

        position - line.Start

    let lineBreakOf (sourceText: SourceText) (line: TextLine) =
        match line.EndIncludingLineBreak - line.End with
        | 0 -> Environment.NewLine
        | length -> sourceText.ToString(TextSpan(line.End, length))

    let changes (sourceText: SourceText) (parseTree: ParsedInput) (indentSize: int) shape =
        let lines = sourceText.Lines
        let literalLines = linesInsideLiterals parseTree

        match shape with
        | Nested(
            namespacePath = namespacePath
            namespaceIsRecursive = namespaceIsRecursive
            namespaceKeyword = namespaceKeyword
            opens = opens
            moduleIdent = moduleIdent
            moduleIsRecursive = moduleIsRecursive
            equals = equals
            bodyColumn = bodyColumn) ->
            let namespaceLine = Line.toZ namespaceKeyword.StartLine
            let headerLine = Line.toZ equals.EndLine
            let lineBreak = lineBreakOf sourceText lines[headerLine]

            let openLines =
                opens
                |> List.collect (fun m -> [ Line.toZ m.StartLine .. Line.toZ m.EndLine ])
                |> Set.ofList

            // The first line after the namespace that is neither blank nor part of a moved `open`: a plain
            // comment, an XML doc comment, an attribute, or (absent all of those) the module keyword itself.
            // Everything before it — the namespace line, the opens, the blank lines around them — is deleted;
            // everything from it onward stays exactly where it is.
            let firstKeptLine =
                seq { namespaceLine + 1 .. headerLine }
                |> Seq.find (fun i ->
                    not (openLines.Contains i)
                    && not (String.IsNullOrWhiteSpace(lines[i].ToString())))

            let moduleSpan = spanOf sourceText moduleIdent.idRange
            let equalsEnd = (spanOf sourceText equals).End

            let headerEnd =
                if isBlank sourceText equalsEnd lines[headerLine].End then
                    lines[headerLine].End
                else
                    equalsEnd

            let recursive =
                if namespaceIsRecursive && not moduleIsRecursive then
                    "rec "
                else
                    ""

            let rootPath =
                $"{recursive}{textBetween sourceText namespacePath.Head (List.last namespacePath)}.{sourceText.ToString moduleSpan}"

            [
                // The namespace line and every `open` before the module's own doc/attributes move to right after
                // the header: a root-style `module A.B.C` must be the file's first declaration, so nothing — not
                // even a leading `open` — can precede it.
                TextChange(TextSpan.FromBounds(lines[namespaceLine].Start, lines[firstKeptLine].Start), "")
                TextChange(TextSpan.FromBounds(moduleSpan.Start, headerEnd), rootPath)

                if not (List.isEmpty opens) then
                    let openText =
                        opens
                        |> List.map (fun m -> sourceText.ToString(spanOf sourceText m))
                        |> String.concat lineBreak

                    TextChange(TextSpan(lines[headerLine].EndIncludingLineBreak, 0), $"{lineBreak}{openText}{lineBreak}{lineBreak}")

                for i in headerLine + 1 .. lines.Count - 1 do
                    let removed = min bodyColumn (leadingSpaces sourceText lines[i])

                    if removed > 0 && not (literalLines.Contains i) then
                        TextChange(TextSpan(lines[i].Start, removed), "")
            ]

        | Root(path, keyword, moduleRange) ->
            let lastIdent = List.last path
            let namespaceIdents = List.take (path.Length - 1) path
            let lineBreak = lineBreakOf sourceText lines[Line.toZ keyword.StartLine]
            let lastSpan = spanOf sourceText lastIdent.idRange
            let padding = String(' ', indentSize)

            [
                TextChange(
                    TextSpan(lines[Line.toZ moduleRange.StartLine].Start, 0),
                    $"namespace {textBetween sourceText path.Head (List.last namespaceIdents)}{lineBreak}{lineBreak}"
                )
                TextChange(
                    TextSpan.FromBounds((spanOf sourceText path.Head.idRange).Start, lastSpan.End),
                    $"{sourceText.ToString lastSpan} ="
                )

                for i in Line.toZ keyword.StartLine + 1 .. lines.Count - 1 do
                    if not (String.IsNullOrWhiteSpace(lines[i].ToString()) || literalLines.Contains i) then
                        TextChange(TextSpan(lines[i].Start, 0), padding)
            ]

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "ConvertNamespaceModule"); Shared>]
type internal FSharpConvertNamespaceModuleRefactoring [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    static let hasSignatureFile (document: Document) =
        let signaturePath = document.FilePath + "i"

        document.Project.Documents
        |> Seq.exists (fun d -> String.Equals(d.FilePath, signaturePath, StringComparison.OrdinalIgnoreCase))

    override _.ComputeRefactoringsAsync context =
        cancellableTask {
            let document = context.Document

            if not (document.IsFSharpSignatureFile || document.IsFSharpScript) then
                let! cancellationToken = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync cancellationToken
                let! parseResults = document.GetFSharpParseResultsAsync(nameof FSharpConvertNamespaceModuleRefactoring)

                let caretLine =
                    Line.fromZ (sourceText.Lines.GetLineFromPosition context.Span.Start).LineNumber

                match NamespaceModuleConversion.tryShape sourceText parseResults.ParseTree with
                | ValueSome shape when
                    NamespaceModuleConversion.isOnHeader caretLine shape
                    && not (hasSignatureFile document)
                    ->
                    let title = NamespaceModuleConversion.title shape

                    let changedDocument =
                        cancellableTask {
                            let! cancellationToken = CancellableTask.getCancellationToken ()
                            let! options = document.GetOptionsAsync cancellationToken

                            let indentSize =
                                options.GetOption(FormattingOptions.IndentationSize, FSharpConstants.FSharpLanguageName)

                            TelemetryReporter.ReportSingleEvent(
                                TelemetryEvents.RefactoringActivated,
                                [| "name", box (nameof FSharpConvertNamespaceModuleRefactoring) |]
                            )

                            let changes =
                                NamespaceModuleConversion.changes sourceText parseResults.ParseTree indentSize shape

                            return document.WithText(sourceText.WithChanges changes)
                        }

                    context.RegisterRefactoring(CodeAction.Create(title, changedDocument, title))
                | _ -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
