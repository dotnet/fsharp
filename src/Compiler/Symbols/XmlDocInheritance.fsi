// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Symbols.XmlDocInheritance

open FSharp.Compiler.Text
open FSharp.Compiler.Xml

/// Expands `<inheritdoc>` elements in XML documentation (currently a placeholder)
/// Returns the original documentation unchanged
val expandInheritDoc: m: range -> doc: XmlDoc -> XmlDoc
