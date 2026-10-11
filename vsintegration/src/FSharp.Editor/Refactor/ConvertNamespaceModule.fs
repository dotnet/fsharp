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

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization

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
            moduleAttributes: SynAttributes *
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

    /// Whether what follows on the line is at most a line comment, which can move together with what stands before it.
    let restOfLineIsBlankOrLineComment (sourceText: SourceText) (m: range) =
        let position = (spanOf sourceText m).End

        let rest =
            sourceText.ToString(TextSpan.FromBounds(position, (sourceText.Lines.GetLineFromPosition position).End)).AsSpan().TrimStart()

        rest.IsEmpty || rest.StartsWith("//".AsSpan(), StringComparison.Ordinal)

    /// Whether an `#if` stands on or above the line. The tree shows only what the active branches hold, so a
    /// declaration or an `open` in another branch there would go unseen and stay above the root module.
    let private hasConditionalDirectiveThrough (line: int) (directives: ConditionalDirectiveTrivia list) =
        directives
        |> List.exists (function
            | ConditionalDirectiveTrivia.If(range = m) -> m.StartLine <= line
            | _ -> false)

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
                             trivia = namespaceTrivia) ]
            trivia = fileTrivia)) ->
            match tryOpensBeforeNestedModule decls with
            | Some(opens,
                   SynModuleDecl.NestedModule(
                       moduleInfo = (SynComponentInfo(attributes = moduleAttributes) as moduleInfo)
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
                    && List.forall (restOfLineIsBlankOrLineComment sourceText) opens
                    && not (hasConditionalDirectiveThrough equals.EndLine fileTrivia.ConditionalDirectives)
                    ->
                    ValueSome(
                        Nested(
                            namespacePath,
                            namespaceIsRecursive,
                            namespaceKeyword,
                            opens,
                            moduleAttributes,
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

    /// The first identifier of each dotted name in the attributes: what they look up in the scope they stand in.
    let private headIdentifiers (attributes: SynAttributes) =
        [
            for list in attributes do
                for attribute in list.Attributes do
                    yield! List.truncate 1 attribute.TypeName.LongIdent

                    yield!
                        ([], [ SyntaxNode.SynExpr attribute.ArgExpr ])
                        ||> SyntaxNodes.fold (fun heads _ node ->
                            match node with
                            | SyntaxNode.SynExpr(SynExpr.Ident head)
                            | SyntaxNode.SynExpr(SynExpr.LongIdent(longDotId = SynLongIdent(id = head :: _)))
                            | SyntaxNode.SynType(SynType.LongIdent(SynLongIdent(id = head :: _))) -> head :: heads
                            | _ -> heads)
        ]

    /// Whether the attributes mean the same at the position as where they stand. A root module's attributes are
    /// checked above the `open`s that move below its header, so a name one of those brings into scope is lost to them.
    let attributesResolveAt (checkResults: FSharpCheckFileResults) (sourceText: SourceText) (position: pos) (attributes: SynAttributes) =
        headIdentifiers attributes
        |> List.forall (fun head ->
            // Only the operators of FSharp.Core make a constant, and those need no `open`.
            PrettyNaming.IsLogicalOpName head.idText
            || (let line = sourceText.Lines[Line.toZ head.idRange.EndLine].ToString()

                match checkResults.GetSymbolUseAtLocation(head.idRange.EndLine, head.idRange.EndColumn, line, [ head.idText ]) with
                | Some symbolUse ->
                    match symbolUse.Symbol with
                    // A named argument sets a property or a field of the attribute.
                    | :? FSharpField -> true
                    | :? FSharpMemberOrFunctionOrValue as value when value.IsMember && not value.IsConstructor -> true
                    | symbol -> checkResults.IsRelativeNameResolvableFromSymbol(position, [], symbol)
                | None -> false))

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

    /// Whether a line lexed from this state starts inside a string literal.
    let private continuesString (state: FSharpTokenizerColorState) =
        match state with
        | FSharpTokenizerColorState.InitialState
        | FSharpTokenizerColorState.Token
        | FSharpTokenizerColorState.IfDefSkip
        | FSharpTokenizerColorState.EndLineThenSkip
        | FSharpTokenizerColorState.EndLineThenToken
        | FSharpTokenizerColorState.Comment
        | FSharpTokenizerColorState.SingleLineComment
        | FSharpTokenizerColorState.StringInComment
        | FSharpTokenizerColorState.VerbatimStringInComment
        | FSharpTokenizerColorState.TripleQuoteStringInComment -> false
        // String, VerbatimString, TripleQuoteString and the extended interpolated string state, which the public enum does not name.
        | _ -> true

    let private isSkipped (state: FSharpTokenizerLexState) =
        match FSharpLineTokenizer.ColorStateOfLexState state with
        | FSharpTokenizerColorState.IfDefSkip
        | FSharpTokenizerColorState.EndLineThenSkip -> true
        | _ -> false

    let private startsWithDirective (code: ReadOnlySpan<char>) (keyword: string) =
        code.StartsWith(keyword.AsSpan(), StringComparison.Ordinal)
        && (code.Length = keyword.Length
            || Char.IsWhiteSpace code[keyword.Length]
            || code[keyword.Length] = '/')

    /// Whether the line is a directive the lexer recognizes while it skips an inactive branch.
    let private isSkippedDirective (line: string) =
        let code = line.AsSpan().TrimStart()

        startsWithDirective code "#if"
        || startsWithDirective code "#elif"
        || startsWithDirective code "#else"
        || startsWithDirective code "#endif"

    let private scanLine (tokenizer: FSharpSourceTokenizer) (line: string) (state: FSharpTokenizerLexState) =
        let lineTokenizer = tokenizer.CreateLineTokenizer line

        let rec loop state =
            match lineTokenizer.ScanToken state with
            | Some _, next -> loop next
            | None, next -> next

        loop state

    /// Lines inside multi-line string literals of the branches of `#if` the defines leave inactive. Each run of
    /// inactive lines between two directives is lexed as the code it is when its branch is taken, where a line
    /// that reads like a directive inside a string is part of that string.
    let private linesInsideInactiveLiterals (sourceText: SourceText) (defines: string list) (langVersion: string) (lines: HashSet<int>) =
        let fileTokenizer = FSharpSourceTokenizer(defines, None, Some langVersion)
        let branchTokenizer = FSharpSourceTokenizer([], None, Some langVersion)
        let mutable fileState = FSharpTokenizerLexState.Initial
        let mutable branchState = FSharpTokenizerLexState.Initial

        for line in sourceText.Lines do
            let text = line.ToString()

            let isInString =
                continuesString (FSharpLineTokenizer.ColorStateOfLexState branchState)

            if isSkipped fileState && (isInString || not (isSkippedDirective text)) then
                if isInString then
                    lines.Add line.LineNumber |> ignore

                branchState <- scanLine branchTokenizer text branchState
            else
                branchState <- FSharpTokenizerLexState.Initial

            fileState <- scanLine fileTokenizer text fileState

    /// Lines inside multi-line string literals, including those in branches of `#if` the defines leave inactive.
    let linesInsideLiterals (sourceText: SourceText) (parseTree: ParsedInput) (defines: string list) (langVersion: string) =
        let lines =
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

        let directives =
            match parseTree with
            | ParsedInput.ImplFile file -> file.Trivia.ConditionalDirectives
            | ParsedInput.SigFile file -> file.Trivia.ConditionalDirectives

        if not directives.IsEmpty then
            linesInsideInactiveLiterals sourceText defines langVersion lines

        lines

    let leadingSpaces (sourceText: SourceText) (line: TextLine) =
        let mutable position = line.Start

        while position < line.End && sourceText[position] = ' ' do
            position <- position + 1

        position - line.Start

    let lineBreakOf (sourceText: SourceText) (line: TextLine) =
        match line.EndIncludingLineBreak - line.End with
        | 0 -> Environment.NewLine
        | length -> sourceText.ToString(TextSpan(line.End, length))

    let changes (sourceText: SourceText) (literalLines: HashSet<int>) (indentSize: int) shape =
        let lines = sourceText.Lines

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

            // The namespace line and the moved `open`s are deleted, each with the blank lines right after it;
            // comments, XML doc comments and attributes among them stay in front of the new header.
            let deletedRanges =
                let removedLines = openLines.Add namespaceLine

                let deletedLines =
                    ((false, []), [ namespaceLine .. headerLine - 1 ])
                    ||> List.fold (fun (afterDeleted, deleted) i ->
                        if
                            removedLines.Contains i
                            || (afterDeleted && String.IsNullOrWhiteSpace(lines[i].ToString()))
                        then
                            true, i :: deleted
                        else
                            false, deleted)
                    |> snd

                ([], deletedLines)
                ||> List.fold (fun ranges i ->
                    match ranges with
                    | struct (first, last) :: rest when first = i + 1 -> struct (i, last) :: rest
                    | _ -> struct (i, i) :: ranges)

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
                // A root-style `module A.B.C` must be the file's first declaration, so the `open`s move to right
                // after the header.
                for struct (first, last) in deletedRanges do
                    TextChange(TextSpan.FromBounds(lines[first].Start, lines[last].EndIncludingLineBreak), "")

                TextChange(TextSpan.FromBounds(moduleSpan.Start, headerEnd), rootPath)

                if not (List.isEmpty opens) then
                    let openText =
                        opens
                        |> List.map (fun m ->
                            let span = spanOf sourceText m

                            sourceText.ToString(TextSpan.FromBounds(span.Start, lines.GetLineFromPosition(span.End).End)).TrimEnd())
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
                    let! attributesKeepTheirMeaning =
                        match shape with
                        | NamespaceModuleConversion.Nested(
                            namespaceKeyword = namespaceKeyword; opens = _ :: _; moduleAttributes = (_ :: _ as moduleAttributes)) ->
                            cancellableTask {
                                let! _, checkResults =
                                    document.GetFSharpParseAndCheckResultsAsync(nameof FSharpConvertNamespaceModuleRefactoring)

                                return
                                    NamespaceModuleConversion.attributesResolveAt
                                        checkResults
                                        sourceText
                                        namespaceKeyword.End
                                        moduleAttributes
                            }
                        | _ -> CancellableTask.singleton true

                    if attributesKeepTheirMeaning then
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

                                let! defines, langVersion =
                                    document.GetFsharpParsingOptionsAsync(nameof FSharpConvertNamespaceModuleRefactoring)

                                let literalLines =
                                    NamespaceModuleConversion.linesInsideLiterals sourceText parseResults.ParseTree defines langVersion

                                let changes =
                                    NamespaceModuleConversion.changes sourceText literalLines indentSize shape

                                return document.WithText(sourceText.WithChanges changes)
                            }

                        context.RegisterRefactoring(CodeAction.Create(title, changedDocument, title))
                | _ -> ()
        }
        |> CancellableTask.startAsTask context.CancellationToken
