// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "AddOpen"); Shared>]
type internal FSharpAddOpenCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager,
        assemblyContentProvider: AssemblyContentProvider
    ) =
    inherit CodeFixProvider()

    static let userOpName = "FSharpAddOpenCodeFixProvider"
    let fixableDiagnosticIds = ["FS0039"; "FS0043"]

    let checker = checkerProvider.Checker
    let fixUnderscoresInMenuText (text: string) = text.Replace("_", "__")

    let qualifySymbolFix (context: CodeFixContext) (fullName, qualifier) = 
        CodeAction.Create(
            fixUnderscoresInMenuText fullName,
            fun (cancellationToken: CancellationToken) -> 
                async {
                    let! cancellationToken = Async.CancellationToken
                    let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    return context.Document.WithText(sourceText.Replace(context.Span, qualifier))
                } |> RoslynHelpers.StartAsyncAsTask(cancellationToken))

    let openNamespaceFix (context: CodeFixContext) ctx name ns multipleNames = 
        let displayText = "open " + ns + if multipleNames then " (" + name + ")" else ""
        // TODO when fresh Roslyn NuGet packages are published, assign "Namespace" Tag to this CodeAction to show proper glyph.
        CodeAction.Create(
            fixUnderscoresInMenuText displayText,
            (fun (cancellationToken: CancellationToken) -> 
                async {
                    let! cancellationToken = Async.CancellationToken
                    let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let changedText, _ = OpenDeclarationHelper.insertOpenDeclaration sourceText ctx ns
                    return context.Document.WithText(changedText)
                } |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
            displayText)

    let getSuggestions (context: CodeFixContext) (candidates: (Entity * InsertContext) list) : unit =
        //Logging.Logging.logInfof "Candidates: %+A" candidates

        let openNamespaceFixes =
            candidates
            |> Seq.choose (fun (entity, ctx) -> entity.Namespace |> Option.map (fun ns -> ns, entity.Name, ctx))
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
            |> Seq.map (fun (ns, name, ctx, multipleNames) -> 
                openNamespaceFix context ctx name ns multipleNames)
            |> Seq.toList
            
        let qualifiedSymbolFixes =
            candidates
            |> Seq.filter (fun (entity,_) -> not(entity.LastIdent.StartsWith "op_")) // Don't include qualified operator names. The resultant codefix won't compile because it won't be an infix operator anymore.
            |> Seq.map (fun (entity, _) -> entity.FullRelativeName, entity.Qualifier)
            |> Seq.distinct
            |> Seq.sort
            |> Seq.map (qualifySymbolFix context)
            |> Seq.toList

        for codeFix in openNamespaceFixes @ qualifiedSymbolFixes do
            context.RegisterCodeFix(codeFix, context.Diagnostics |> Seq.filter (fun x -> fixableDiagnosticIds |> List.contains x.Id) |> Seq.toImmutableArray)

    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override __.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let document = context.Document
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
            let! _, parsedInput, checkResults = checker.ParseAndCheckDocument(document, projectOptions, sourceText = sourceText, userOpName = userOpName)
            let line = sourceText.Lines.GetLineFromPosition(context.Span.End)
            let linePos = sourceText.Lines.GetLinePosition(context.Span.End)
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
            
            let! symbol = 
                asyncMaybe {
                    let! lexerSymbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, context.Span.End, document.FilePath, defines, SymbolLookupKind.Greedy, false)
                    return! checkResults.GetSymbolUseAtLocation(Line.fromZ linePos.Line, lexerSymbol.Ident.idRange.EndColumn, line.ToString(), lexerSymbol.FullIsland, userOpName=userOpName)
                } |> liftAsync

            do! Option.guard symbol.IsNone

            let unresolvedIdentRange =
                let startLinePos = sourceText.Lines.GetLinePosition context.Span.Start
                let startPos = Pos.fromZ startLinePos.Line startLinePos.Character
                let endLinePos = sourceText.Lines.GetLinePosition context.Span.End
                let endPos = Pos.fromZ endLinePos.Line endLinePos.Character
                Range.mkRange context.Document.FilePath startPos endPos
            
            let isAttribute = UntypedParseImpl.GetEntityKind(unresolvedIdentRange.Start, parsedInput) = Some EntityKind.Attribute
            
            let entities =
                assemblyContentProvider.GetAllEntitiesInProjectAndReferencedAssemblies checkResults
                |> List.collect (fun s -> 
                     [ yield s.TopRequireQualifiedAccessParent, s.AutoOpenParent, s.Namespace, s.CleanedIdents
                       if isAttribute then
                          let lastIdent = s.CleanedIdents.[s.CleanedIdents.Length - 1]
                          if lastIdent.EndsWith "Attribute" && s.Kind LookupType.Precise = EntityKind.Attribute then
                              yield 
                                  s.TopRequireQualifiedAccessParent, 
                                  s.AutoOpenParent,
                                  s.Namespace,
                                  s.CleanedIdents 
                                  |> Array.replace (s.CleanedIdents.Length - 1) (lastIdent.Substring(0, lastIdent.Length - 9)) ])

            let longIdent = ParsedInput.getLongIdentAt parsedInput unresolvedIdentRange.End

            let! maybeUnresolvedIdents =
                longIdent 
                |> Option.map (fun longIdent ->
                    longIdent
                    |> List.map (fun ident ->
                        { Ident = ident.idText
                          Resolved = not (ident.idRange = unresolvedIdentRange)})
                    |> List.toArray)
                                                    
            let insertionPoint = 
                if document.FSharpOptions.CodeFixes.AlwaysPlaceOpensAtTopLevel then OpenStatementInsertionPoint.TopLevel
                else OpenStatementInsertionPoint.Nearest

            let createEntity = ParsedInput.tryFindInsertionContext unresolvedIdentRange.StartLine parsedInput maybeUnresolvedIdents insertionPoint
            return entities |> Seq.map createEntity |> Seq.concat |> Seq.toList |> getSuggestions context
        } 
        |> Async.Ignore 
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
 