// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.Completion.FileSystem
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.Text

open Microsoft.FSharp.Compiler.Range

type internal AbstractReferenceDirectiveCompletionProvider(projectInfoManager: ProjectInfoManager) =
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
 
    //let getPathThroughLastSlash(stringLiteral : SyntaxToken, position : int) =
    //    return PathCompletionUtilities.GetPathThroughLastSlash(
    //        quotedPath: stringLiteral.ToString(),
    //        quotedPathStart: stringLiteral.SpanStart,
    //        position: position);

    override __.IsInsertionTrigger(text, characterPosition, _options) =
        PathCompletionUtilities.IsTriggerCharacter(text, characterPosition)

    override __.ProvideCompletionsAsync(context : CompletionContext) =
        asyncMaybe {
            let document = context.Document
            let position = context.Position
            let cancellationToken = context.CancellationToken
            let! sourceText = document.GetTextAsync(cancellationToken)
            let textLines = sourceText.Lines
            let caretLinePos = textLines.GetLinePosition(position)
            
            //let caretLine = textLines.GetLineFromPosition(position)
            //let fcsCaretLineNumber = Line.fromZ caretLinePos.Line  // Roslyn line numbers are zero-based, FSharp.Compiler.Service line numbers are 1-based
            let caretLineColumn = caretLinePos.Character
            let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)  
            // first try to get the #r string literal token.  If we couldn't, then we're not in a #r reference directive and we immediately bail.
            let tokens = CommonHelpers.tokenizeLine (document.Id, sourceText, position, document.FilePath, defines)
            let! token = tokens |> List.tryFind (fun x -> caretLineColumn >= x.LeftColumn && caretLineColumn <= x.RightColumn)
            
            //var textChangeSpan = this.GetTextChangeSpan(stringLiteral, position);
            
            //var gacHelper = new GlobalAssemblyCacheCompletionHelper(this, textChangeSpan, itemRules: s_rules);
            //var text = await document.GetTextAsync(context.CancellationToken).ConfigureAwait(false);
            //var snapshot = text.FindCorrespondingEditorTextSnapshot();
            //if (snapshot == null)
            //{
            //    // Passing null to GetFileSystemDiscoveryService raises an exception.
            //    // Instead, return here since there is no longer snapshot for this document.
            //    return;
            //}
            
            //var referenceResolver = document.Project.CompilationOptions.MetadataReferenceResolver;
            
            //// TODO: https://github.com/dotnet/roslyn/issues/5263
            //// Avoid dependency on a specific resolvers.
            //// The search paths should be provided by specialized workspaces:
            //// - InteractiveWorkspace for interactive window 
            //// - ScriptWorkspace for loose .csx files (we don't have such workspace today)
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
            
            //var fileSystemHelper = new FileSystemCompletionHelper(
            //    this, textChangeSpan,
            //    GetFileSystemDiscoveryService(snapshot),
            //    Glyph.OpenFolder,
            //    Glyph.Assembly,
            //    searchPaths: searchPaths,
            //    allowableExtensions: new[] { ".dll", ".exe" },
            //    exclude: path => path.Contains(","),
            //    itemRules: s_rules);
            
            //var pathThroughLastSlash = GetPathThroughLastSlash(stringLiteral, position);
            
            //var documentPath = document.Project.IsSubmission ? null : document.FilePath;
            //context.AddItems(gacHelper.GetItems(pathThroughLastSlash, documentPath));
            //context.AddItems(fileSystemHelper.GetItems(pathThroughLastSlash, documentPath));
            return null
        } 
        |> Async.Ignore
        |> CommonRoslynHelpers.StartAsyncUnitAsTask context.CancellationToken
    