// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks
open System.Threading
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddOpen); Shared>]
type internal AddOpenCodeFixProvider [<ImportingConstructor>] (assemblyContentProvider: AssemblyContentProvider) =
    inherit CodeFixProvider()

    let fixUnderscoresInMenuText (text: string) = text.Replace("_", "__")

    let qualifySymbolFix (context: CodeFixContext) (fullName, qualifier) =
        context.RegisterFsharpFix(CodeFix.AddOpen, fixUnderscoresInMenuText fullName, [| TextChange(context.Span, qualifier) |])

    let openNamespaceFix (context: CodeFixContext) ctx name ns multipleNames sourceText =
        let displayText = "open " + ns + (if multipleNames then " (" + name + ")" else "")
        let newText, _ = OpenDeclarationHelper.insertOpenDeclaration sourceText ctx ns
        let changes = newText.GetTextChanges(sourceText)

        context.RegisterFsharpFix(CodeFix.AddOpen, fixUnderscoresInMenuText displayText, changes)

    let addSuggestionsAsCodeFixes
        (context: CodeFixContext)
        (sourceText: SourceText)
        (candidates: (InsertionContextEntity * InsertionContext) list)
        =
        do
            candidates
            |> Seq.choose (fun (entity, ctx) -> entity.Namespace |> Option.map (fun ns -> ns, entity.FullDisplayName, ctx))
            |> Seq.groupBy (fun (ns, _, _) -> ns)
            |> Seq.map (fun (ns, xs) ->
                ns,
                xs
                |> Seq.map (fun (_, name, ctx) -> name, ctx)
                |> Seq.distinctBy (fun (name, _) -> name)
                |> Seq.sortBy fst
                |> Seq.toArray)
            |> Seq.map (fun (ns, names) ->
                let multipleNames = names |> Array.length > 1
                names |> Seq.map (fun (name, ctx) -> ns, name, ctx, multipleNames))
            |> Seq.concat
            |> Seq.iter (fun (ns, name, ctx, multipleNames) -> openNamespaceFix context ctx name ns multipleNames sourceText)

        do
            candidates
            |> Seq.filter (fun (entity, _) -> not (entity.LastIdent.StartsWith "op_")) // Don't include qualified operator names. The resultant codefix won't compile because it won't be an infix operator anymore.
            |> Seq.map (fun (entity, _) -> entity.FullRelativeName, entity.Qualifier)
            |> Seq.distinct
            |> Seq.sort
            |> Seq.iter (qualifySymbolFix context)

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0039", "FS0043")

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let document = context.Document

            let! sourceText = document.GetTextAsync(context.CancellationToken)

            let! parseResults, checkResults =
                document.GetFSharpParseAndCheckResultsAsync(nameof (AddOpenCodeFixProvider))
                |> CancellableTask.start context.CancellationToken
                |> Async.AwaitTask
                |> liftAsync

            let line = sourceText.Lines.GetLineFromPosition(context.Span.End)
            let linePos = sourceText.Lines.GetLinePosition(context.Span.End)

            let! defines, langVersion, strictIndentation =
                document.GetFsharpParsingOptionsAsync(nameof (AddOpenCodeFixProvider))
                |> liftAsync

            let! symbol =
                maybe {
                    let! lexerSymbol =
                        Tokenizer.getSymbolAtPosition (
                            document.Id,
                            sourceText,
                            context.Span.End,
                            document.FilePath,
                            defines,
                            SymbolLookupKind.Greedy,
                            false,
                            false,
                            Some langVersion,
                            strictIndentation,
                            context.CancellationToken
                        )

                    return
                        checkResults.GetSymbolUseAtLocation(
                            Line.fromZ linePos.Line,
                            lexerSymbol.Ident.idRange.EndColumn,
                            line.ToString(),
                            lexerSymbol.FullIsland
                        )
                }

            do! Option.guard symbol.IsNone

            let unresolvedIdentRange =
                let startLinePos = sourceText.Lines.GetLinePosition context.Span.Start
                let startPos = Position.fromZ startLinePos.Line startLinePos.Character
                let endLinePos = sourceText.Lines.GetLinePosition context.Span.End
                let endPos = Position.fromZ endLinePos.Line endLinePos.Character
                Range.mkRange context.Document.FilePath startPos endPos

            let isAttribute =
                ParsedInput.GetEntityKind(unresolvedIdentRange.Start, parseResults.ParseTree) = Some EntityKind.Attribute

            let entities =
                assemblyContentProvider.GetAllEntitiesInProjectAndReferencedAssemblies checkResults
                |> List.collect (fun s ->
                    [
                        yield s.TopRequireQualifiedAccessParent, s.AutoOpenParent, s.Namespace, s.CleanedIdents
                        if isAttribute then
                            let lastIdent = s.CleanedIdents.[s.CleanedIdents.Length - 1]

                            if
                                lastIdent.EndsWith "Attribute"
                                && s.Kind LookupType.Precise = EntityKind.Attribute
                            then
                                yield
                                    s.TopRequireQualifiedAccessParent,
                                    s.AutoOpenParent,
                                    s.Namespace,
                                    s.CleanedIdents
                                    |> Array.replace (s.CleanedIdents.Length - 1) (lastIdent.Substring(0, lastIdent.Length - 9))
                    ])

            let longIdent =
                ParsedInput.GetLongIdentAt parseResults.ParseTree unresolvedIdentRange.End

            let! maybeUnresolvedIdents =
                longIdent
                |> Option.map (fun longIdent ->
                    longIdent
                    |> List.map (fun ident ->
                        {
                            Ident = ident.idText
                            Resolved = not (ident.idRange = unresolvedIdentRange)
                        })
                    |> List.toArray)

            let insertionPoint =
                if document.Project.IsFSharpCodeFixesAlwaysPlaceOpensAtTopLevelEnabled then
                    OpenStatementInsertionPoint.TopLevel
                else
                    OpenStatementInsertionPoint.Nearest

            let createEntity =
                ParsedInput.TryFindInsertionContext
                    unresolvedIdentRange.StartLine
                    parseResults.ParseTree
                    maybeUnresolvedIdents
                    insertionPoint

            return
                entities
                |> Seq.map createEntity
                |> Seq.concat
                |> Seq.toList
                |> addSuggestionsAsCodeFixes context sourceText
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
