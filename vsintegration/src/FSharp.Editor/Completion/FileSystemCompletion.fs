// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.Completion.FileSystem
open Microsoft.CodeAnalysis.Text

open System.Text.RegularExpressions
open System.IO

module internal FileSystemCompletion =
    let [<Literal>] private NetworkPath = "\\\\"
    let private commitRules = ImmutableArray.Create(CharacterSetModificationRule.Create(CharacterSetModificationKind.Replace, '"', '\\', ',', '/'))
    let private rules = CompletionItemRules.Create(commitCharacterRules = commitRules)

    let private getQuotedPathStart(text: SourceText, position: int, quotedPathGroup: Group) =
        text.Lines.GetLineFromPosition(position).Start + quotedPathGroup.Index

    let private getPathThroughLastSlash(text: SourceText, position: int, quotedPathGroup: Group) =
        PathCompletionUtilities.GetPathThroughLastSlash(
            quotedPath = quotedPathGroup.Value,
            quotedPathStart = getQuotedPathStart(text, position, quotedPathGroup),
            position = position)
 
    let private getTextChangeSpan(text: SourceText, position: int, quotedPathGroup: Group) =
        PathCompletionUtilities.GetTextChangeSpan(
            quotedPath = quotedPathGroup.Value,
            quotedPathStart = getQuotedPathStart(text, position, quotedPathGroup),
            position = position)

    let private getFileGlyph (extention: string) =
        match extention with
        | ".exe" | ".dll" -> Some Glyph.Assembly
        | _ -> None

    let getItems(provider: CompletionProvider, document: Document, position: int, allowableExtensions: string[], directiveRegex: Regex) =
        asyncMaybe {
            let! ct = liftAsync Async.CancellationToken
            let! text = document.GetTextAsync ct
            let line = text.Lines.GetLineFromPosition(position)
            let lineText = text.ToString(TextSpan.FromBounds(line.Start, position));
            let m = directiveRegex.Match lineText
            
            do! Option.guard m.Success
            let quotedPathGroup = m.Groups.["literal"]
            let quotedPath = quotedPathGroup.Value;
            let endsWithQuote = PathCompletionUtilities.EndsWithQuote(quotedPath)
            
            do! Option.guard (not (endsWithQuote && (position >= line.Start + m.Length)))
            let snapshot = text.FindCorrespondingEditorTextSnapshot()
            
            do! Option.guard (not (isNull snapshot))
            let fileSystem = CurrentWorkingDirectoryDiscoveryService.GetService(snapshot)
            
            let searchPaths = ImmutableArray.Create (Path.GetDirectoryName document.FilePath)
     
            let helper = 
                FileSystemCompletionHelper(
                    provider,
                    getTextChangeSpan(text, position, quotedPathGroup),
                    fileSystem,
                    Glyph.OpenFolder,
                    allowableExtensions |> Array.tryPick getFileGlyph |> Option.defaultValue Glyph.None,
                    searchPaths = searchPaths,
                    allowableExtensions = allowableExtensions,
                    itemRules = rules)
     
            let pathThroughLastSlash = getPathThroughLastSlash(text, position, quotedPathGroup)
            let documentPath = if document.Project.IsSubmission then null else document.FilePath
            return helper.GetItems(pathThroughLastSlash, documentPath) 
        } |> Async.map (Option.defaultValue ImmutableArray.Empty)

    let isInsertionTrigger(text: SourceText, position) =
        // Bring up completion when the user types a quote (i.e.: #r "), or if they type a slash
        // path separator character, or if they type a comma (#r "foo,version...").
        // Also, if they're starting a word.  i.e. #r "c:\W
        let ch = text.[position]
        ch = '"' || ch = '\\' || ch = ',' || ch = '/' ||
            CommonCompletionUtilities.IsStartingNewWord(text, position, (fun x -> Char.IsLetter x), (fun x -> Char.IsLetterOrDigit x))
 
    let getTextChange(selectedItem: CompletionItem, ch: Nullable<char>) =
        // When we commit "\\" when the user types \ we have to adjust for the fact that the
        // controller will automatically append \ after we commit.  Because of that, we don't
        // want to actually commit "\\" as we'll end up with "\\\".  So instead we just commit
        // "\" and know that controller will append "\" and give us "\\".
        if selectedItem.DisplayText = NetworkPath && ch = Nullable '\\' then
            Some (TextChange(selectedItem.Span, "\\"))
        else
            None

type internal LoadDirectiveCompletionProvider() =
    inherit CommonCompletionProvider()

    let directiveRegex = Regex("""#load\s+(@?"*(?<literal>"[^"]*"?))""", RegexOptions.Compiled ||| RegexOptions.ExplicitCapture)
 
    override this.ProvideCompletionsAsync(context) =
        async {
            let! items = FileSystemCompletion.getItems(this, context.Document, context.Position, [|".fs"; ".fsx"|], directiveRegex)
            context.AddItems(items)
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask context.CancellationToken
 
    override __.IsInsertionTrigger(text, position, _options) = FileSystemCompletion.isInsertionTrigger(text, position)
 
    override __.GetTextChangeAsync(selectedItem, ch, cancellationToken) = 
        match FileSystemCompletion.getTextChange(selectedItem, ch) with
        | Some x -> Task.FromResult(Nullable x)
        | None -> base.GetTextChangeAsync(selectedItem, ch, cancellationToken)

type internal ReferenceDirectiveCompletionProvider() =
    inherit CommonCompletionProvider()

    let directiveRegex = Regex("""#r\s+(@?"*(?<literal>"[^"]*"?))""", RegexOptions.Compiled ||| RegexOptions.ExplicitCapture)
 
    override this.ProvideCompletionsAsync(context) =
        async {
            let! items = FileSystemCompletion.getItems(this, context.Document, context.Position, [|".dll"; ".exe"|], directiveRegex)
            context.AddItems(items)
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask context.CancellationToken
 
    override __.IsInsertionTrigger(text, position, _options) = FileSystemCompletion.isInsertionTrigger(text, position)
 
    override __.GetTextChangeAsync(selectedItem, ch, cancellationToken) = 
        match FileSystemCompletion.getTextChange(selectedItem, ch) with
        | Some x -> Task.FromResult(Nullable x)
        | None -> base.GetTextChangeAsync(selectedItem, ch, cancellationToken)