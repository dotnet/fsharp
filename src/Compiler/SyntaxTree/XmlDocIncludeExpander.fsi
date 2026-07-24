// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Xml.XmlDocIncludeExpander

open FSharp.Compiler.Text

/// Expand all <include file="..." path="..."/> elements in the given elaborated XML doc lines.
/// When `emit` is true, include errors are reported as warnings (FS3887); when false they are
/// suppressed (for quiet validation such as XmlDoc.Check). Returns the input unchanged when there
/// are no includes, parsing fails, or nothing expanded.
val expandIncludeLines: emit: bool -> baseFileName: string -> range: range -> lines: string[] -> string[]
