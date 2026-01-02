// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Xml.XmlDocIncludeExpander

open FSharp.Compiler.Xml
open FSharp.Compiler.Text

/// Expand all <include file="..." path="..."/> elements in XML documentation text.
/// Warnings are emitted via the diagnostics logger for any errors.
val expandIncludesInText: baseFileName: string -> xmlText: string -> range: range -> string

/// Expand all <include file="..." path="..."/> elements in an XmlDoc.
/// Warnings are emitted via the diagnostics logger for any errors.
val expandIncludes: doc: XmlDoc -> XmlDoc
