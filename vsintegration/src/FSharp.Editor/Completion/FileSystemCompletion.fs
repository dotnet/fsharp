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

    let getItems(provider: CompletionProvider, document: Document, position: int, allowableExtensions: string list, directiveRegex: Regex, searchPaths: string list) =
        asyncMaybe {
            do! Option.guard (Path.GetExtension document.FilePath = ".fsx")
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
            
            let helper = 
                FileSystemCompletionHelper(
                    provider,
                    getTextChangeSpan(text, position, quotedPathGroup),
                    fileSystem,
                    Glyph.OpenFolder,
                    allowableExtensions |> List.tryPick getFileGlyph |> Option.defaultValue Glyph.None,
                    searchPaths = Seq.toImmutableArray searchPaths,
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

    let private includeDirectiveCleanRegex = Regex("""#I\s+(@?"*(?<literal>[^"]*)"?)""", RegexOptions.Compiled ||| RegexOptions.ExplicitCapture)

    let getIncludeDirectives (document: Document, position: int) =
        async {
            let! ct = Async.CancellationToken
            let! text = document.GetTextAsync(ct)
            let lines = text.Lines
            let caretLine = text.Lines.GetLinePosition(position).Line
            return
                lines
                |> Seq.filter (fun x -> x.LineNumber <= caretLine)
                |> Seq.choose (fun line ->
                    let lineStr = line.ToString().Trim()
                    // optimization: fail fast if the line does not start with "(optional spaces) #I"
                    if not (lineStr.StartsWith "#I") then None
                    else
                        match includeDirectiveCleanRegex.Match lineStr with
                        | m when m.Success -> Some (m.Groups.["literal"].Value)
                        | _ -> None
                   )
                |> Seq.toList
        }

[<AbstractClass>]
type internal HashDirectiveCompletionProvider(directiveRegex: string, allowableExtensions: string list, useIncludeDirectives: bool) =
    inherit CommonCompletionProvider()

    let directiveRegex = Regex(directiveRegex, RegexOptions.Compiled ||| RegexOptions.ExplicitCapture)
 
    override this.ProvideCompletionsAsync(context) =
        async {
            let defaultSearchPath = Path.GetDirectoryName context.Document.FilePath 
            let! extraSearchPaths = 
                if useIncludeDirectives then
                    FileSystemCompletion.getIncludeDirectives (context.Document, context.Position)
                else async.Return []
            let searchPaths = defaultSearchPath :: extraSearchPaths
            let! items = FileSystemCompletion.getItems(this, context.Document, context.Position, allowableExtensions, directiveRegex, searchPaths)
            context.AddItems(items)
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask context.CancellationToken
 
    override __.IsInsertionTrigger(text, position, _) = FileSystemCompletion.isInsertionTrigger(text, position)
 
    override __.GetTextChangeAsync(selectedItem, ch, cancellationToken) = 
        match FileSystemCompletion.getTextChange(selectedItem, ch) with
        | Some x -> Task.FromResult(Nullable x)
        | None -> base.GetTextChangeAsync(selectedItem, ch, cancellationToken)


type internal LoadDirectiveCompletionProvider() =
    inherit HashDirectiveCompletionProvider("""\s*#load\s+(@?"*(?<literal>"[^"]*"?))""", [".fs"; ".fsx"], useIncludeDirectives = true)

type internal ReferenceDirectiveCompletionProvider() =
    inherit HashDirectiveCompletionProvider("""\s*#r\s+(@?"*(?<literal>"[^"]*"?))""", [".dll"; ".exe"], useIncludeDirectives = true)

type internal IncludeDirectiveCompletionProvider() =
    // we have to pass an extension that's not met in real life because if we pass empty list, it does not filter at all.
    inherit HashDirectiveCompletionProvider("""\s*#I\s+(@?"*(?<literal>"[^"]*"?))""", [".impossible_extension"], useIncludeDirectives = false)