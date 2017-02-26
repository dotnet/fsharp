// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.Completion.FileSystem
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.Text

open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

type internal ReferenceDirectiveCompletionProvider(projectInfoManager: ProjectInfoManager) =
    inherit CommonCompletionProvider()
    
    //let getTextChangeSpan (sringSyntaxToken stringLiteral, int position) =
    //    return PathCompletionUtilities.GetTextChangeSpan(
    //        quotedPath: stringLiteral.ToString(),
    //        quotedPathStart: stringLiteral.SpanStart,
    //        position: position);
 
    let getFileSystemDiscoveryService (textSnapshot : ITextSnapshot) : ICurrentWorkingDirectoryDiscoveryService =
        CurrentWorkingDirectoryDiscoveryService.GetService(textSnapshot)
 
    let commitRules = ImmutableArray.Create(CharacterSetModificationRule.Create(CharacterSetModificationKind.Replace, '"', '\\', ','))
    let filterRules = ImmutableArray<CharacterSetModificationRule>.Empty
    let rules = CompletionItemRules.Create(filterCharacterRules = filterRules, commitCharacterRules = commitRules, enterKeyRule = EnterKeyRule.Never)
 
    override __.IsInsertionTrigger(text, characterPosition, _options) =
        PathCompletionUtilities.IsTriggerCharacter(text, characterPosition)

    override this.ProvideCompletionsAsync(context : Microsoft.CodeAnalysis.Completion.CompletionContext) =
        asyncMaybe {
            let document = context.Document
            let position = context.Position
            let cancellationToken = context.CancellationToken
            let! sourceText = document.GetTextAsync(cancellationToken)
            let textLines = sourceText.Lines
            let caretLinePos = textLines.GetLinePosition(position)
            let fcsCaretLineNumber = Line.fromZ caretLinePos.Line
            let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)  
            // first try to get the #r string literal token.  If we couldn't, then we're not in a #r reference directive and we immediately bail.
            let tokens = CommonHelpers.tokenizeLine (document.Id, sourceText, position, document.FilePath, defines) 

            Logging.Logging.logInfof "tokens = %A" (tokens |> List.map (fun x -> sprintf "(%d, %d, %A)" x.LeftColumn x.RightColumn x.CharClass))

            let tks = tokens
            let _x = tks

            let stripWs (tokens: FSharpTokenInfo list) = tokens |> List.skipWhile (fun x -> x.CharClass = FSharpTokenCharKind.WhiteSpace)

            let! stringLiteralSpan =
                match stripWs tokens with
                | token :: rest when token.ColorClass = FSharpTokenColorKind.PreprocessorKeyword ->
                    let range = 
                        match rest |> stripWs |> List.takeWhile (fun x -> x.ColorClass = FSharpTokenColorKind.String) with
                        | [] -> None
                        | stringTokens ->
                            let leftQuote = List.head stringTokens
                            let rightQuote = List.last stringTokens
                            let r = mkRange document.FilePath (mkPos fcsCaretLineNumber leftQuote.RightColumn) (mkPos fcsCaretLineNumber (rightQuote.RightColumn + 1))
                            let span = CommonRoslynHelpers.FSharpRangeToTextSpan (sourceText, r)
                            Logging.Logging.logInfof "%A, text = %s" r (sourceText.ToString span)
                            Some r
                    range |> Option.map (fun x -> CommonRoslynHelpers.FSharpRangeToTextSpan (sourceText, x))
                | _ -> None

            let textChangeSpan = PathCompletionUtilities.GetTextChangeSpan(sourceText.ToString stringLiteralSpan, stringLiteralSpan.Start, position)
            
            //let gacHelper = new GlobalAssemblyCacheCompletionHelper(this, textChangeSpan, itemRules: s_rules);
            
            // Passing null to GetFileSystemDiscoveryService raises an exception.
            // Instead, return here since there is no longer snapshot for this document.
            let! snapshot = sourceText.FindCorrespondingEditorTextSnapshot() |> Option.ofObj
            
            //let referenceResolver = document.Project.CompilationOptions.MetadataReferenceResolver
            
            //ImmutableArray<string> searchPaths;
            //RuntimeMetadataReferenceResolver rtResolver;
            //WorkspaceMetadataFileReferenceResolver workspaceResolver;
            
            //if ((rtResolver = referenceResolver as RuntimeMetadataReferenceResolver) != null)
            //{
            //    searchPaths = rtResolver.PathResolver.SearchPaths;
            //}
            //else if ((workspaceResolver = referenceResolver as WorkspaceMetadataFileReferenceResolver) != null)
            //{
            //    searchPaths = workspaceResolver.PathResolver.SearchPaths;
            //}
            //else
            //{
            //    return;
            //}
            
            let fileSystemHelper = 
                FileSystemCompletionHelper(
                    this, 
                    textChangeSpan,
                    getFileSystemDiscoveryService snapshot,
                    Glyph.OpenFolder,
                    Glyph.Assembly,
                    searchPaths = ImmutableArray.Create ".",
                    allowableExtensions = [|".dll"; ".exe" |],
                    exclude = (fun path -> path.Contains(",")),
                    itemRules = rules)
            
            let pathThroughLastSlash = 
                PathCompletionUtilities.GetPathThroughLastSlash(sourceText.ToString stringLiteralSpan, stringLiteralSpan.Start, position)
            
            let documentPath = if document.Project.IsSubmission then null else document.FilePath
            //context.AddItems(gacHelper.GetItems(pathThroughLastSlash, documentPath));
            context.AddItems(fileSystemHelper.GetItems(pathThroughLastSlash, documentPath))
        } 
        |> Async.Ignore
        |> CommonRoslynHelpers.StartAsyncUnitAsTask context.CancellationToken
    