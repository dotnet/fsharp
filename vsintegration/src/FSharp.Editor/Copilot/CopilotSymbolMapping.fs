// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.CopilotSymbolMapping

open System
open System.Collections.Generic

open Microsoft.VisualStudio.Copilot
open Microsoft.VisualStudio.Imaging

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax

/// Also the mention prefix the user types and sees: "#fsharpSymbol:Namespace.Type.Member".
[<Literal>]
let SymbolMember = "fsharpSymbol"

[<Literal>]
let FullyQualifiedNameInput = "fullyQualifiedName"

[<Literal>]
let DeclarationLineInput = "declarationLine"

[<Literal>]
let DeclarationFileInput = "declarationFile"

[<Literal>]
let DeclarationProjectInput = "declarationProject"

let inputOf<'T> name (inputs: IReadOnlyDictionary<string, CopilotValue> | null) =
    match inputs with
    | null -> ValueNone
    | inputs ->
        match inputs.TryGetValue name with
        | true, value ->
            match value.TryGetValue<'T>() with
            | true, input -> ValueSome input
            | _ -> ValueNone
        | _ -> ValueNone

let textOf name inputs =
    inputOf<string> name inputs
    |> ValueOption.filter (fun text -> not (String.IsNullOrWhiteSpace text))

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

/// FCS joins dotted paths: only a container's last segment can be quoted, and a file's top-level module not at all.
let private isQuoted (item: NavigableItem) =
    item.NeedsBackticks && not (item.Kind.IsModule && item.Container.Type.IsFile)

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

let fullyQualifiedName (item: NavigableItem) =
    let name =
        if isQuoted item then
            String.Concat("``", item.Name, "``")
        else
            item.Name

    match containerPath item.Container with
    | "" -> name
    | container -> String.Concat(container, ".", name)

let tooltipName (item: NavigableItem) =
    match symbolContextType item.Kind, item.Container.Type, item.Container.Name with
    | CopilotSymbolContextType.Class, _, _
    | _, NavigableContainerType.File, _
    | _, _, "" -> item.Name
    | _, _, container -> $"{container}.{item.Name}"

/// The suffix test spares a solution-wide scan from building the name of every declaration it passes.
let hasFullyQualifiedName (candidate: string) (item: NavigableItem) =
    (candidate.EndsWith(item.Name, StringComparison.Ordinal)
     || candidate.EndsWith("``", StringComparison.Ordinal))
    && String.Equals(candidate, fullyQualifiedName item, StringComparison.Ordinal)

/// The typed text split on ':' ("#fsharpSymbol:Ns.Type:15" arrives as three inputs), asked as Unknown before the picker resolves it.
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

let private lineOf (content: string) offset =
    let mutable line = 1

    for i in 0 .. offset - 1 do
        if content[i] = '\n' then
            line <- line + 1

    line

/// Copilot widens an empty selection to the whole file for F#, so a whole-file range means no line is known.
let editorFocusOf (document: DocumentContext) =
    match document.FilePath with
    | null -> ValueNone
    | filePath ->
        let focusOn firstLine lastLine =
            ValueSome
                {
                    FilePath = filePath
                    FirstLine = firstLine
                    LastLine = lastLine
                }

        match document.Selections |> Seq.tryHeadV with
        | ValueSome selection when selection.Caret.HasValue ->
            let line = lineOf document.Content selection.Caret.Value
            focusOn line line
        | ValueSome selection when selection.LineRange.HasValue ->
            let range = selection.LineRange.Value

            let totalLines =
                document.TotalLinesInFile
                |> ValueOption.ofNullable
                |> ValueOption.defaultWith (fun () -> lineOf document.Content document.Content.Length)

            if range.StartLine <= 1 && range.EndLine >= totalLines then
                focusOn 0 0
            else
                focusOn range.StartLine range.EndLine
        | _ -> focusOn 0 0
