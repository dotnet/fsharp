// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Translates F# navigable items into the shapes the Copilot chat "#" mention picker understands.
module internal Microsoft.VisualStudio.FSharp.Editor.CopilotSymbolMapping

open Microsoft.VisualStudio.Copilot
open Microsoft.VisualStudio.Imaging

open FSharp.Compiler.EditorServices

/// Name of the context member. It becomes the mention prefix the user sees and re-types,
/// as in "#fsharpSymbol:Namespace.Type.Member".
[<Literal>]
let SymbolMember = "fsharpSymbol"

[<Literal>]
let FullyQualifiedNameInput = "fullyQualifiedName"

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

/// Dotted path that both drives the picker's pattern matching and identifies a picked mention
/// when it is resolved back to source.
let fullyQualifiedName (item: NavigableItem) =
    match item.Container.FullName with
    | "" -> item.Name
    | container -> $"{container}.{item.Name}"
