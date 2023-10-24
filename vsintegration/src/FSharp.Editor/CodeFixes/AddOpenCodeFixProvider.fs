// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddOpen); Shared>]
type internal AddOpenCodeFixProvider [<ImportingConstructor>] (assemblyContentProvider: AssemblyContentProvider) =
    inherit CodeFixProvider()

    static let br = Environment.NewLine

    let fixUnderscoresInMenuText (text: string) = text.Replace("_", "__")

    let qualifySymbolFix (context: CodeFixContext) (fullName, qualifier) =
        {
            Name = CodeFix.AddOpen
            Message = fixUnderscoresInMenuText fullName
            Changes = [ TextChange(context.Span, qualifier) ]
        }

    // Hey, I know what you're thinking: this is a horrible hack.
    // Indeed it is, this is a better (but still bad) version of the OpenDeclarationHelper.
    // The things should be actually fixed in the InsertionContext, it's bugged.
    // But currently CompletionProvider also depends on InsertionContext and it's not tested enough.
    // So fixing InsertionContext or OpenDeclarationHelper might break completion which would be bad.
    // The hack below is at least heavily tested.
    // And at least it shows what should be fixed down the line.
    let getOpenDeclaration (sourceText: SourceText) (ctx: InsertionContext) (ns: string) =
        // insertion context counts from 2, make the world sane
        let insertionLineNumber = ctx.Pos.Line - 2
        let margin = String(' ', ctx.Pos.Column)

        let startLineNumber, openDeclaration =
            match ctx.ScopeKind with
            | ScopeKind.TopModule ->
                match sourceText.Lines[insertionLineNumber].ToString().Trim() with

                // explicit top level module
                | line when line.StartsWith "module" && not (line.EndsWith "=") -> insertionLineNumber + 2, $"{margin}open {ns}{br}{br}"

                // nested module, shouldn't be here
                | line when line.StartsWith "module" -> insertionLineNumber, $"{margin}open {ns}{br}{br}"

                // attribute, shouldn't be here
                | line when line.StartsWith "[<" && line.EndsWith ">]" ->
                    let moduleDeclLineNumberOpt =
                        sourceText.Lines
                        |> Seq.skip insertionLineNumber
                        |> Seq.tryFindIndex (fun line -> line.ToString().Contains "module")

                    match moduleDeclLineNumberOpt with
                    // implicit top level module
                    | None -> insertionLineNumber, $"{margin}open {ns}{br}{br}"
                    // explicit top level module
                    | Some number ->
                        // add back the skipped lines
                        let moduleDeclLineNumber = insertionLineNumber + number
                        let moduleDeclLineText = sourceText.Lines[moduleDeclLineNumber].ToString().Trim()

                        if moduleDeclLineText.EndsWith "=" then
                            insertionLineNumber, $"{margin}open {ns}{br}{br}"
                        else
                            moduleDeclLineNumber + 2, $"{margin}open {ns}{br}{br}"

                // implicit top level module
                | _ -> insertionLineNumber, $"{margin}open {ns}{br}{br}"

            | ScopeKind.Namespace -> insertionLineNumber + 3, $"{margin}open {ns}{br}{br}"
            | ScopeKind.NestedModule -> insertionLineNumber + 2, $"{margin}open {ns}{br}{br}"
            | ScopeKind.OpenDeclaration -> insertionLineNumber + 1, $"{margin}open {ns}{br}"

            // So far I don't know how to get here
            | ScopeKind.HashDirective -> insertionLineNumber + 1, $"open {ns}{br}{br}"

        let start = sourceText.Lines[startLineNumber].Start
        TextChange(TextSpan(start, 0), openDeclaration)

    let openNamespaceFix ctx name ns multipleNames sourceText =
        let displayText = $"open {ns}" + (if multipleNames then " (" + name + ")" else "")
        let change = getOpenDeclaration sourceText ctx ns

        {
            Name = CodeFix.AddOpen
            Message = fixUnderscoresInMenuText displayText
            Changes = [ change ]
        }

    let getSuggestionsAsCodeFixes
        (context: CodeFixContext)
        (sourceText: SourceText)
        (candidates: (InsertionContextEntity * InsertionContext) list)
        =
        seq {
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
            |> Seq.map (fun (ns, name, ctx, multipleNames) -> openNamespaceFix ctx name ns multipleNames sourceText)

            candidates
            |> Seq.filter (fun (entity, _) -> not (entity.LastIdent.StartsWith "op_")) // Don't include qualified operator names. The resultant codefix won't compile because it won't be an infix operator anymore.
            |> Seq.map (fun (entity, _) -> entity.FullRelativeName, entity.Qualifier)
            |> Seq.distinct
            |> Seq.sort
            |> Seq.map (qualifySymbolFix context)

        }
        |> Seq.concat

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0039", "FS0043")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let document = context.Document

                let! sourceText = context.GetSourceTextAsync()

                let! parseResults, checkResults = document.GetFSharpParseAndCheckResultsAsync(nameof AddOpenCodeFixProvider)

                let line = sourceText.Lines.GetLineFromPosition(context.Span.End)
                let linePos = sourceText.Lines.GetLinePosition(context.Span.End)

                let! defines, langVersion, strictIndentation = document.GetFsharpParsingOptionsAsync(nameof AddOpenCodeFixProvider)

                return
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
                    |> Option.filter (fun lexerSymbol ->
                        let symbolOpt =
                            checkResults.GetSymbolUseAtLocation(
                                Line.fromZ linePos.Line,
                                lexerSymbol.Ident.idRange.EndColumn,
                                line.ToString(),
                                lexerSymbol.FullIsland
                            )

                        match symbolOpt with
                        | None -> true
                        // this is for operators for FS0043
                        | Some symbol when PrettyNaming.IsLogicalOpName symbol.Symbol.DisplayName -> true
                        | _ -> false)
                    |> Option.bind (fun _ ->
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
                            |> Array.collect (fun s ->
                                [|
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
                                                |> Array.replace
                                                    (s.CleanedIdents.Length - 1)
                                                    (lastIdent.Substring(0, lastIdent.Length - 9))
                                |])

                        ParsedInput.GetLongIdentAt parseResults.ParseTree unresolvedIdentRange.End
                        |> Option.bind (fun longIdent ->
                            let maybeUnresolvedIdents =
                                longIdent
                                |> List.map (fun ident ->
                                    {
                                        Ident = ident.idText
                                        Resolved = not (ident.idRange = unresolvedIdentRange)
                                    })
                                |> List.toArray

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

                            entities
                            |> Seq.map createEntity
                            |> Seq.concat
                            |> Seq.toList
                            |> getSuggestionsAsCodeFixes context sourceText
                            |> Seq.tryHead))

                    |> ValueOption.ofOption
            }
