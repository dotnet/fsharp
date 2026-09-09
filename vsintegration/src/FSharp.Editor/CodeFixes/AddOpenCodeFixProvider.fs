// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

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

    let fixUnderscoresInMenuText (text: string) = text.Replace("_", "__")

    let qualifySymbolFix (context: CodeFixContext) (fullName, qualifier) =
        {
            Name = CodeFix.AddOpen
            Message = fixUnderscoresInMenuText fullName
            Changes = [ TextChange(context.Span, qualifier) ]
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

    // A plain `open` reaches namespaces and F# modules. When the entity sits deeper than that - a type
    // nested in a type, or a static member of one - the type itself has to be opened.
    let openDeclaration (entity: InsertionContextEntity) openableIdentCount ns =
        if entity.NamespaceIdentCount > openableIdentCount then
            $"open type {ns}"
        else
            $"open {ns}"

    let getSuggestionsAsCodeFixes
        (context: CodeFixContext)
        (sourceText: SourceText)
        (candidates: (InsertionContextEntity * InsertionContext * int) list)
        =
        seq {
            candidates
            |> Seq.choose (fun (entity, ctx, openableIdentCount) ->
                entity.Namespace
                |> Option.map (fun ns -> openDeclaration entity openableIdentCount ns, entity.FullDisplayName, ctx))
            |> Seq.groupBy (fun (declaration, _, _) -> declaration)
            |> Seq.map (fun (declaration, xs) ->
                declaration,
                xs
                |> Seq.map (fun (_, name, ctx) -> name, ctx)
                |> Seq.distinctBy (fun (name, _) -> name)
                |> Seq.sortBy fst
                |> Seq.toArray)
            |> Seq.map (fun (declaration, names) ->
                let multipleNames = names |> Array.length > 1
                names |> Seq.map (fun (name, ctx) -> declaration, name, ctx, multipleNames))
            |> Seq.concat
            |> Seq.map (fun (declaration, name, ctx, multipleNames) -> openNamespaceFix ctx name declaration multipleNames sourceText)

            candidates
            |> Seq.filter (fun (entity, _, _) -> not (entity.LastIdent.StartsWith "op_")) // Don't include qualified operator names. The resultant codefix won't compile because it won't be an infix operator anymore.
            |> Seq.map (fun (entity, _, _) -> entity.FullRelativeName, entity.Qualifier)
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

                        let entities =
                            assemblyContentProvider.GetAllEntitiesInProjectAndReferencedAssemblies checkResults
                            |> Array.collect (fun s ->
                                [|
                                    yield
                                        s.OpenableIdentCount,
                                        (s.TopRequireQualifiedAccessParent, s.AutoOpenParent, s.Namespace, s.CleanedIdents)
                                    if isAttribute then
                                        let lastIdent = s.CleanedIdents.[s.CleanedIdents.Length - 1]

                                        if
                                            lastIdent.EndsWith "Attribute"
                                            && s.Kind LookupType.Precise = EntityKind.Attribute
                                        then
                                            yield
                                                s.OpenableIdentCount,
                                                (s.TopRequireQualifiedAccessParent,
                                                 s.AutoOpenParent,
                                                 s.Namespace,
                                                 s.CleanedIdents
                                                 |> Array.replace
                                                     (s.CleanedIdents.Length - 1)
                                                     (lastIdent.Substring(0, lastIdent.Length - 9)))
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
                            |> Seq.collect (fun (openableIdentCount, symbol) ->
                                createEntity symbol
                                |> Seq.map (fun (entity, ctx) -> entity, ctx, openableIdentCount))
                            |> Seq.toList
                            |> getSuggestionsAsCodeFixes context sourceText
                            |> Seq.tryHead))

                    |> ValueOption.ofOption
            }
