// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.XmlDocInheritance

open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.Xml

/// Expands `<inheritdoc>` elements in XML documentation
/// Takes an optional list of all loaded CCUs for resolving cref targets in external assemblies
/// Takes an optional function to look up XML docs by signature from external XML files
/// Takes an optional CCU for resolving same-compilation types
/// Takes an optional ModuleOrNamespaceType for accessing the current compilation's typed content
/// Takes an optional implicit target cref for resolving <inheritdoc/> without cref attribute
/// Takes a set of visited signatures to prevent cycles
val expandInheritDoc:
    allCcusOpt: CcuThunk list option ->
    tryFindXmlDocBySignature: (string -> string -> XmlDoc option) option ->
    ccuOpt: CcuThunk option ->
    currentModuleTypeOpt: ModuleOrNamespaceType option ->
    implicitTargetCrefOpt: string option ->
    m: range ->
    visited: Set<string> ->
    doc: XmlDoc ->
        XmlDoc
