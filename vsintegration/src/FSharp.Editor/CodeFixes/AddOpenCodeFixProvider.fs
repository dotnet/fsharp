// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddOpen); Shared>]
type internal AddOpenCodeFixProvider [<ImportingConstructor>] (assemblyContentProvider: AssemblyContentProvider) =
    inherit CodeFixProvider()

    let maxOpenSuggestions = 5
    let maxQualifySuggestions = 3

    let systemFirst (name: ReadOnlySpan<char>) (sortText: string) =
        let isInSystem =
            name.Equals("System".AsSpan(), StringComparison.Ordinal)
            || name.StartsWith("System.".AsSpan(), StringComparison.Ordinal)

        (if isInSystem then 0 else 1), sortText

    let openOrder (declaration: string) =
        systemFirst (declaration.AsSpan(declaration.LastIndexOf ' ' + 1)) declaration

    let qualificationOrder (fullName: string) =
        systemFirst (fullName.AsSpan()) fullName

    let fixUnderscoresInMenuText (text: string) = text.Replace("_", "__")

    let qualifySymbolFix (firstIdentSpan: TextSpan) (fullName, qualifier) =
        {
            Name = CodeFix.AddOpen
            Message = fixUnderscoresInMenuText fullName
            Changes = [ TextChange(firstIdentSpan, qualifier) ]
        }

    let openNamespaceFix ctx name declaration multipleNames sourceText =
        let displayText = declaration + (if multipleNames then " (" + name + ")" else "")

        let change =
            OpenDeclarationHelper.getOpenDeclarationChange sourceText ctx declaration

        {
            Name = CodeFix.AddOpen
            Message = fixUnderscoresInMenuText displayText
            Changes = [ change ]
        }

    let declaringEntity (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpEntity as entity -> entity.DeclaringEntity
        | :? FSharpMemberOrFunctionOrValue as mfv -> mfv.DeclaringEntity
        | _ -> None

    let rec tryFindModuleOrTypeNamedByLeading (leadingIdentCount: int) (nameIdentCount: int) (symbol: FSharpSymbol) =
        match declaringEntity symbol with
        | Some parent when not parent.IsNamespace ->
            if nameIdentCount - 1 = leadingIdentCount then
                ValueSome parent
            else
                tryFindModuleOrTypeNamedByLeading leadingIdentCount (nameIdentCount - 1) parent
        | _ -> ValueNone

    let tryGetOpenDeclaration (entity: InsertionContextEntity) (symbol: AssemblySymbol) ns =
        match tryFindModuleOrTypeNamedByLeading entity.NamespaceIdentCount symbol.CleanedIdents.Length symbol.Symbol with
        | ValueSome opened when opened.IsFSharpModule -> ValueSome $"open {ns}"
        | ValueSome opened when opened.GenericParameters.Count > 0 -> ValueNone
        | ValueSome _ -> ValueSome $"open type {ns}"
        | ValueNone -> ValueSome $"open {ns}"

    let isQualifiable (entity: InsertionContextEntity) = not (entity.LastIdent.StartsWith "op_")

    let getSuggestionsAsCodeFixes
        (firstIdentSpan: TextSpan)
        (sourceText: SourceText)
        (candidates: (InsertionContextEntity * InsertionContext * AssemblySymbol) list)
        =
        seq {
            candidates
            |> Seq.chooseV (fun (entity, ctx, symbol) ->
                match entity.Namespace with
                | Some ns ->
                    tryGetOpenDeclaration entity symbol ns
                    |> ValueOption.map (fun declaration -> declaration, entity.FullDisplayName, ctx)
                | None -> ValueNone)
            |> Seq.groupBy (fun (declaration, _, _) -> declaration)
            |> Seq.map (fun (declaration, xs) ->
                declaration,
                xs
                |> Seq.map (fun (_, name, ctx) -> name, ctx)
                |> Seq.distinctBy (fun (name, _) -> name)
                |> Seq.sortBy fst
                |> Seq.toArray)
            |> Seq.sortBy (fst >> openOrder)
            |> Seq.map (fun (declaration, names) ->
                let multipleNames = names |> Array.length > 1
                names |> Seq.map (fun (name, ctx) -> declaration, name, ctx, multipleNames))
            |> Seq.concat
            |> Seq.truncate maxOpenSuggestions
            |> Seq.map (fun (declaration, name, ctx, multipleNames) -> openNamespaceFix ctx name declaration multipleNames sourceText)

            candidates
            |> Seq.filter (fun (entity, _, _) -> isQualifiable entity)
            |> Seq.map (fun (entity, _, _) -> entity.FullRelativeName, entity.Qualifier)
            |> Seq.distinct
            |> Seq.sortBy (fst >> qualificationOrder)
            |> Seq.truncate maxQualifySuggestions
            |> Seq.map (qualifySymbolFix firstIdentSpan)

        }
        |> Seq.concat

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0039", "FS0043")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFixes this

    interface IFSharpMultiCodeFixProvider with
        member _.GetCodeFixesAsync context =
            cancellableTask {
                let document = context.Document

                let! sourceText = context.GetSourceTextAsync()

                let! parseResults, checkResults = document.GetFSharpParseAndCheckResultsAsync(nameof AddOpenCodeFixProvider)

                let line = sourceText.Lines.GetLineFromPosition(context.Span.End)
                let linePos = sourceText.Lines.GetLinePosition(context.Span.End)

                let! defines, langVersion = document.GetFsharpParsingOptionsAsync(nameof AddOpenCodeFixProvider)

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

                        let catalogue =
                            assemblyContentProvider.GetAllEntitiesInProjectAndReferencedAssemblies checkResults

                        let casesInScope = HashSet<string>(StringComparer.Ordinal)
                        let sourceLine = line.ToString()

                        match ParsedInput.TryGetCompletionContext(unresolvedIdentRange.End, parseResults.ParseTree, sourceLine) with
                        | Some(CompletionContext.Pattern _) ->
                            context.CancellationToken.ThrowIfCancellationRequested()

                            let cases =
                                [
                                    for symbol in catalogue do
                                        if symbol.Symbol :? FSharpActivePatternCase then
                                            yield symbol
                                ]

                            let partialName =
                                { QuickParse.GetPartialLongNameEx(sourceLine, linePos.Character - 1) with
                                    QualifyingIdents = []
                                    PartialIdent = ""
                                    LastDotPos = None
                                }

                            let symbols =
                                checkResults.GetDeclarationListSymbols(
                                    Some parseResults,
                                    unresolvedIdentRange.EndLine,
                                    sourceLine,
                                    partialName,
                                    getAllEntities = (fun () -> cases)
                                )

                            for group in symbols do
                                for symbol in group do
                                    if symbol.Symbol :? FSharpActivePatternCase then
                                        casesInScope.Add symbol.Symbol.FullName |> ignore
                        | _ -> ()

                        let entities =
                            catalogue
                            |> Array.collect (fun s ->
                                [|
                                    if not (s.Symbol :? FSharpActivePatternCase) || casesInScope.Contains s.FullName then
                                        yield s, s.CleanedIdents

                                        if isAttribute then
                                            let lastIdent = s.CleanedIdents.[s.CleanedIdents.Length - 1]

                                            if
                                                lastIdent.EndsWith "Attribute"
                                                && s.Kind LookupType.Precise = EntityKind.Attribute
                                            then
                                                yield
                                                    s,
                                                    s.CleanedIdents
                                                    |> Array.replace
                                                        (s.CleanedIdents.Length - 1)
                                                        (lastIdent.Substring(0, lastIdent.Length - 9))
                                |])

                        ParsedInput.GetLongIdentAt parseResults.ParseTree unresolvedIdentRange.End
                        |> Option.map (fun longIdent ->
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
                                ParsedInput.TryFindInsertionContextAt
                                    unresolvedIdentRange.Start
                                    parseResults.ParseTree
                                    maybeUnresolvedIdents
                                    insertionPoint

                            let patternName =
                                longIdent
                                |> List.map (fun ident -> PrettyNaming.NormalizeIdentifierBackticks ident.idText)
                                |> String.concat "."

                            entities
                            |> Seq.collect (fun (symbol, idents) ->
                                createEntity (symbol.TopRequireQualifiedAccessParent, symbol.AutoOpenParent, symbol.Namespace, idents)
                                |> Seq.map (fun (entity, insertionContext) ->
                                    let entity =
                                        if symbol.Symbol :? FSharpActivePatternCase then
                                            if entity.FullDisplayName <> "" && entity.FullDisplayName <> patternName then
                                                { entity with Namespace = None }
                                            else
                                                { entity with FullDisplayName = "" }
                                        else
                                            entity

                                    entity, insertionContext, symbol))
                            |> Seq.toList
                            |> getSuggestionsAsCodeFixes
                                (RoslynHelpers.FSharpRangeToTextSpan(sourceText, longIdent.Head.idRange))
                                sourceText))

                    |> Option.defaultValue Seq.empty
            }
