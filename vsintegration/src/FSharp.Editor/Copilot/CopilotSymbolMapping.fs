// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Translates F# navigable items into the shapes the Copilot chat "#" mention picker understands.
module internal Microsoft.VisualStudio.FSharp.Editor.CopilotSymbolMapping

open System
open System.Collections.Generic

open Microsoft.VisualStudio.Copilot
open Microsoft.VisualStudio.Imaging

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax

/// Name of the context member. It becomes the mention prefix the user sees and re-types,
/// as in "#fsharpSymbol:Namespace.Type.Member".
[<Literal>]
let SymbolMember = "fsharpSymbol"

[<Literal>]
let FullyQualifiedNameInput = "fullyQualifiedName"

/// Disambiguates which overload or partial definition a mention was picked for - several of them share
/// one fully qualified name, and only the one at this line is the one the user meant.
[<Literal>]
let DeclarationLineInput = "declarationLine"

/// The parse tree cannot tell an interface, struct or record apart from a plain class, so every
/// type-like declaration is reported as a class.
let symbolContextType kind =
    match kind with
    | NavigableItemKind.Module
    | NavigableItemKind.ModuleAbbreviation
    | NavigableItemKind.Exception
    | NavigableItemKind.Type -> CopilotSymbolContextType.Class
    | NavigableItemKind.ModuleValue -> CopilotSymbolContextType.Function
    | NavigableItemKind.Field
    | NavigableItemKind.Property -> CopilotSymbolContextType.Field
    | NavigableItemKind.Constructor
    | NavigableItemKind.Member -> CopilotSymbolContextType.Method
    | NavigableItemKind.EnumCase -> CopilotSymbolContextType.Constant
    | NavigableItemKind.UnionCase -> CopilotSymbolContextType.Union

let private imageId kind =
    match kind with
    | NavigableItemKind.Module
    | NavigableItemKind.ModuleAbbreviation -> KnownImageIds.ModulePublic
    | NavigableItemKind.Exception -> KnownImageIds.ExceptionPublic
    | NavigableItemKind.Type -> KnownImageIds.ClassPublic
    | NavigableItemKind.ModuleValue
    | NavigableItemKind.Constructor
    | NavigableItemKind.Member -> KnownImageIds.MethodPublic
    | NavigableItemKind.Field -> KnownImageIds.FieldPublic
    | NavigableItemKind.Property -> KnownImageIds.PropertyPublic
    | NavigableItemKind.EnumCase
    | NavigableItemKind.UnionCase -> KnownImageIds.EnumerationItemPublic

let icon kind =
    CopilotImageMoniker(Guid = KnownImageIds.ImageCatalogGuid, Id = imageId kind)

/// A name F# spells in double backticks keeps them: bare, a value ``a.b`` in module M and the value b of
/// M's nested module a would both be "M.a.b". A file's top-level module is left bare - NavigateTo names it
/// by its whole dotted path, joined, so its segments are not known apart.
let private isQuoted (item: NavigableItem) =
    item.NeedsBackticks && not (item.Kind.IsModule && item.Container.Type.IsFile)

/// FCS joins a container's path into one string, so of its segments only the last can be spelled.
let private isContainerQuoted (container: NavigableContainer) =
    container.Name.Length > 0
    && not container.Type.IsFile
    && PrettyNaming.DoesIdentifierNeedBackticks container.Name

let private containerPath (container: NavigableContainer) =
    let path = container.FullName

    if isContainerQuoted container then
        String.Concat(path.Substring(0, path.Length - container.Name.Length), "``", container.Name, "``")
    else
        path

/// Dotted path that identifies a picked mention when it is resolved back to source.
let fullyQualifiedName (item: NavigableItem) =
    let name =
        if isQuoted item then
            String.Concat("``", item.Name, "``")
        else
            item.Name

    match containerPath item.Container with
    | "" -> name
    | container -> String.Concat(container, ".", name)

/// How Copilot's own tooltip names a declaration: a member by the container it is declared in, a type
/// or module by itself.
let tooltipName (item: NavigableItem) =
    match symbolContextType item.Kind, item.Container.Type, item.Container.Name with
    | CopilotSymbolContextType.Class, _, _
    | _, NavigableContainerType.File, _
    | _, _, "" -> item.Name
    | _, _, container -> $"{container}.{item.Name}"

/// How much of `candidate` is left in front of `segment` - spelled in double backticks when `quoted` -
/// or -1 when the candidate does not end with it.
let private lengthBefore (candidate: ReadOnlySpan<char>) (segment: string) quoted =
    let quotes = if quoted then 2 else 0
    let length = segment.Length + quotes * 2

    if candidate.Length < length then
        -1
    else
        let tail = candidate.Slice(candidate.Length - length)

        if
            tail.Slice(quotes, segment.Length).EqualsOrdinal segment
            && (not quoted || tail.StartsWithOrdinal "``" && tail.EndsWithOrdinal "``")
        then
            candidate.Length - length
        else
            -1

/// Answers what comparing against `fullyQualifiedName` would, without building the dotted path -
/// a solution-wide scan asks this of every declaration it walks past.
let hasFullyQualifiedName (candidate: string) (item: NavigableItem) =
    let candidate = candidate.AsSpan()
    let container = item.Container
    let path = container.FullName
    let beforeName = lengthBefore candidate item.Name (isQuoted item)

    if beforeName < 0 then
        false
    elif path.Length = 0 then
        beforeName = 0
    elif beforeName = 0 || candidate[beforeName - 1] <> '.' then
        false
    else
        let spelledPath = candidate.Slice(0, beforeName - 1)
        let enclosing = path.Length - container.Name.Length

        lengthBefore spelledPath container.Name (isContainerQuoted container) = enclosing
        && spelledPath.Slice(0, enclosing).EqualsOrdinal(path.AsSpan(0, enclosing))

/// The text a picker query searches for. The inputs are positional: the member name first once the
/// mention has been committed, as in "#fsharpSymbol:Namespace.Type", then the search text, then
/// qualifiers the search ignores. The picker asks before it has resolved what kind of mention is typed.
let searchTextOf (query: CopilotMentionQuery) =
    let searchTextAt index (inputs: IReadOnlyList<string>) =
        if index < inputs.Count then
            ValueSome(
                match inputs[index] with
                | null -> ""
                | text -> text.Trim()
            )
        else
            ValueNone

    match query.Type, query.Inputs with
    | (CopilotMentionType.Context | CopilotMentionType.Unknown), inputs when inputs.Count > 0 ->
        if String.Equals(inputs[0], SymbolMember, StringComparison.Ordinal) then
            searchTextAt 1 inputs
        else
            searchTextAt 0 inputs
    | _ -> ValueNone

/// The number of lines before `offset` in `content`, 1-based - `DocumentSelection.Caret` is always the
/// offset of the start of a line, so no column arithmetic is needed.
let private lineOf (content: string) offset =
    let mutable line = 1

    for i in 0 .. offset - 1 do
        if content[i] = '\n' then
            line <- line + 1

    line

let private totalLinesIn (content: string) = lineOf content content.Length

/// Where Copilot's document context says the user is. Copilot widens an empty selection to the
/// enclosing block, and to the whole file when a language has no block expander, as F# has not: the
/// caret line then comes from the selection's caret, which Copilot leaves out when the caret is on the
/// range's first or last line - so a caret on the file's first or last line reports no line.
let editorFocusOf (document: DocumentContext) =
    match document.FilePath with
    | null -> ValueNone
    | filePath ->
        match document.Selections |> Seq.tryHeadV with
        | ValueSome selection when selection.Caret.HasValue ->
            let line = lineOf document.Content selection.Caret.Value

            ValueSome
                {
                    FilePath = filePath
                    FirstLine = line
                    LastLine = line
                }
        | ValueSome selection when selection.LineRange.HasValue ->
            let range = selection.LineRange.Value

            let totalLines =
                document.TotalLinesInFile
                |> ValueOption.ofNullable
                |> ValueOption.defaultWith (fun () -> totalLinesIn document.Content)

            if range.StartLine <= 1 && range.EndLine >= totalLines then
                ValueSome
                    {
                        FilePath = filePath
                        FirstLine = 0
                        LastLine = 0
                    }
            else
                ValueSome
                    {
                        FilePath = filePath
                        FirstLine = range.StartLine
                        LastLine = range.EndLine
                    }
        | _ ->
            ValueSome
                {
                    FilePath = filePath
                    FirstLine = 0
                    LastLine = 0
                }
