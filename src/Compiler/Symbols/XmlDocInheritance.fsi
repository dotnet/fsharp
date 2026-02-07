// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.XmlDocInheritance

open FSharp.Compiler.Text
open FSharp.Compiler.Xml

/// Expands `<inheritdoc>` elements in XML documentation.
/// The caller provides a `resolveCref` function to look up documentation by cref string.
/// Takes an optional implicit target cref for resolving <inheritdoc/> without cref attribute.
/// Takes a set of visited signatures to prevent cycles.
val expandInheritDoc:
    resolveCref: (string -> string option) ->
    implicitTargetCrefOpt: string option ->
    m: range ->
    visited: Set<string> ->
    doc: XmlDoc ->
        XmlDoc
