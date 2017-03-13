// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

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

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module internal InsertContext =
    /// Corrects insertion line number based on kind of scope and text surrounding the insertion point.
    let adjustInsertionPoint (sourceText: SourceText) ctx  =
        let getLineStr line = sourceText.Lines.[line].ToString().Trim()
        let line =
            match ctx.ScopeKind with
            | ScopeKind.TopModule ->
                if ctx.Pos.Line > 1 then
                    // it's an implicit module without any open declarations    
                    let line = getLineStr (ctx.Pos.Line - 2)
                    let isImpliciteTopLevelModule = not (line.StartsWith "module" && not (line.EndsWith "="))
                    if isImpliciteTopLevelModule then 1 else ctx.Pos.Line
                else 1
            | ScopeKind.Namespace ->
                // for namespaces the start line is start line of the first nested entity
                if ctx.Pos.Line > 1 then
                    [0..ctx.Pos.Line - 1]
                    |> List.mapi (fun i line -> i, getLineStr line)
                    |> List.tryPick (fun (i, lineStr) -> 
                        if lineStr.StartsWith "namespace" then Some i
                        else None)
                    |> function
                        // move to the next line below "namespace" and convert it to F# 1-based line number
                        | Some line -> line + 2 
                        | None -> ctx.Pos.Line
                else 1  
            | _ -> ctx.Pos.Line

        { ctx.Pos with Line = line }

    /// <summary>
    /// Inserts open declaration into `SourceText`. 
    /// </summary>
    /// <param name="sourceText">SourceText.</param>
    /// <param name="ctx">Insertion context. Typically returned from tryGetInsertionContext</param>
    /// <param name="ns">Namespace to open.</param>
    let insertOpenDeclaration (sourceText: SourceText) (ctx: InsertContext) (ns: string) : SourceText =
        let insert line lineStr (sourceText: SourceText) : SourceText =
            let pos = sourceText.Lines.[line].Start
            sourceText.WithChanges(TextChange(TextSpan(pos, 0), lineStr + Environment.NewLine))

        let pos = adjustInsertionPoint sourceText ctx
        let docLine = pos.Line - 1
        let lineStr = (String.replicate pos.Column " ") + "open " + ns
        let sourceText = sourceText |> insert docLine lineStr
        // if there's no a blank line between open declaration block and the rest of the code, we add one
        let sourceText = 
            if sourceText.Lines.[docLine + 1].ToString().Trim() <> "" then 
                sourceText |> insert (docLine + 1) ""
            else sourceText
        // for top level module we add a blank line between the module declaration and first open statement
        if (pos.Column = 0 || ctx.ScopeKind = ScopeKind.Namespace) && docLine > 0
            && not (sourceText.Lines.[docLine - 1].ToString().Trim().StartsWith "open") then
                sourceText |> insert docLine ""
        else sourceText

[<ExportCodeFixProvider(FSharpCommonConstants.FSharpLanguageName, Name = "AddOpen"); Shared>]
type internal FSharpAddOpenCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: ProjectInfoManager,
        assemblyContentProvider: AssemblyContentProvider
    ) =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = ["FS0039"; "FS0043"]

    let checker = checkerProvider.Checker
    let fixUnderscoresInMenuText (text: string) = text.Replace("_", "__")

    let qualifySymbolFix (context: CodeFixContext) (fullName, qualifier) = 
        CodeAction.Create(
            fixUnderscoresInMenuText fullName,
            fun (cancellationToken: CancellationToken) -> 
                async {
                    let! sourceText = context.Document.GetTextAsync()
                    return context.Document.WithText(sourceText.Replace(context.Span, qualifier))
                } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken))

    let openNamespaceFix (context: CodeFixContext) ctx name ns multipleNames = 
        let displayText = "open " + ns + if multipleNames then " (" + name + ")" else ""
        // TODO when fresh Roslyn NuGet packages are published, assign "Namespace" Tag to this CodeAction to show proper glyph.
        CodeAction.Create(
            fixUnderscoresInMenuText displayText,
            (fun (cancellationToken: CancellationToken) -> 
                async {
                    let! sourceText = context.Document.GetTextAsync()
                    return context.Document.WithText(InsertContext.insertOpenDeclaration sourceText ctx ns)
                } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)),
            displayText)

    let getSuggestions (context: CodeFixContext) (candidates: (Entity * InsertContext) list) : unit =
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
            let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
            let! _, parsedInput, checkResults = checker.ParseAndCheckDocument(document, options, allowStaleResults = true, sourceText = sourceText)
            let line = sourceText.Lines.GetLineFromPosition(context.Span.End)
            let linePos = sourceText.Lines.GetLinePosition(context.Span.End)
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
            
            let! symbol = 
                asyncMaybe {
                    let! lexerSymbol = CommonHelpers.getSymbolAtPosition(document.Id, sourceText, context.Span.End, document.FilePath, defines, SymbolLookupKind.Greedy)
                    return! checkResults.GetSymbolUseAtLocation(Line.fromZ linePos.Line, lexerSymbol.Ident.idRange.EndColumn, line.ToString(), lexerSymbol.FullIsland)
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
                |> List.collect (fun e -> 
                     [ yield e.TopRequireQualifiedAccessParent, e.AutoOpenParent, e.Namespace, e.CleanedIdents
                       if isAttribute then
                          let lastIdent = e.CleanedIdents.[e.CleanedIdents.Length - 1]
                          if lastIdent.EndsWith "Attribute" && e.Kind LookupType.Precise = EntityKind.Attribute then
                              yield 
                                  e.TopRequireQualifiedAccessParent, 
                                  e.AutoOpenParent,
                                  e.Namespace,
                                  e.CleanedIdents 
                                  |> Array.replace (e.CleanedIdents.Length - 1) (lastIdent.Substring(0, lastIdent.Length - 9)) ])

            let longIdent = ParsedInput.getLongIdentAt parsedInput unresolvedIdentRange.End

            let! maybeUnresolvedIdents =
                longIdent 
                |> Option.map (fun longIdent ->
                    longIdent
                    |> List.map (fun ident ->
                        { Ident = ident.idText
                          Resolved = not (ident.idRange = unresolvedIdentRange)})
                    |> List.toArray)
                                                    
            let createEntity = ParsedInput.tryFindInsertionContext unresolvedIdentRange.StartLine parsedInput maybeUnresolvedIdents
            return entities |> Seq.map createEntity |> Seq.concat |> Seq.toList |> getSuggestions context
        } 
        |> Async.Ignore 
        |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
 