// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Symbols.XmlDocInheritance

open FSharp.Compiler.InfoReader
open FSharp.Compiler.Text
open FSharp.Compiler.Xml

/// Expands `<inheritdoc>` elements in XML documentation
/// Takes an optional InfoReader for resolving cref targets to their documentation
/// Takes a set of visited signatures to prevent cycles
val expandInheritDoc: infoReaderOpt: InfoReader option -> m: range -> visited: Set<string> -> doc: XmlDoc -> XmlDoc
