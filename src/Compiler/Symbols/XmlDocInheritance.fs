// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.XmlDocInheritance

open System.Xml.Linq
open System.Xml.XPath
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Text
open FSharp.Compiler.Xml

/// Represents an inheritdoc directive found in XML documentation
type InheritDocDirective =
    {
        /// Optional cref attribute specifying explicit target
        Cref: string option
        /// Optional path attribute for XPath filtering
        Path: string option
        /// The original XElement for replacement
        Element: XElement
    }

/// Checks if an XML document contains <inheritdoc> elements
let private hasInheritDoc (xmlText: string) = xmlText.IndexOf("<inheritdoc") >= 0

/// Extracts inheritdoc directives from parsed XML
let private extractInheritDocDirectives (doc: XDocument) =
    let inheritDocName = XName.op_Implicit "inheritdoc" |> Operators.nonNull

    let crefName = XName.op_Implicit "cref" |> Operators.nonNull
    let pathName = XName.op_Implicit "path" |> Operators.nonNull

    doc.Descendants(inheritDocName)
    |> Seq.map (fun elem ->
        let crefAttr = elem.Attribute(crefName)
        let pathAttr = elem.Attribute(pathName)

        {
            Cref =
                match crefAttr with
                | null -> None
                | attr -> Some attr.Value
            Path =
                match pathAttr with
                | null -> None
                | attr -> Some attr.Value
            Element = elem
        })
    |> List.ofSeq

/// Applies an XPath filter to XML content
let private applyXPathFilter (m: range) (xpath: string) (sourceXml: string) : string option =
    try
        let doc =
            XDocument.Parse("<doc>" + sourceXml + "</doc>", LoadOptions.PreserveWhitespace)

        // If the xpath starts with /, it's an absolute path that won't work with our wrapper
        // Adjust to search within the doc
        let adjustedXpath =
            if xpath.StartsWith("/") && not (xpath.StartsWith("//")) then
                "/doc" + xpath
            else
                xpath

        let selectedElements = doc.XPathSelectElements(adjustedXpath)

        if Seq.isEmpty selectedElements then
            None
        else
            let result =
                selectedElements
                |> Seq.map (fun elem -> elem.ToString(SaveOptions.DisableFormatting))
                |> String.concat "\n"

            Some result
    with ex ->
        warning (Error(FSComp.SR.xmlDocInheritDocError ($"invalid XPath '{xpath}': {ex.Message}"), m))
        None

/// Recursively expands inheritdoc in the retrieved documentation
let rec private expandInheritedDoc
    (resolveCref: string -> string option)
    (implicitTargetCrefOpt: string option)
    (m: range)
    (visited: Set<string>)
    (cref: string)
    (xmlText: string)
    : string =
    if visited.Contains(cref) then
        xmlText
    else
        let newVisited = visited.Add(cref)
        expandInheritDocText resolveCref implicitTargetCrefOpt m newVisited xmlText

/// Expands `<inheritdoc>` elements in XML documentation text.
/// The caller provides a `resolveCref` function that maps a cref string to its resolved XML doc text.
/// This keeps the module free of CCU/TypedTree dependencies — resolution is done by the tooling layer.
and private expandInheritDocText
    (resolveCref: string -> string option)
    (implicitTargetCrefOpt: string option)
    (m: range)
    (visited: Set<string>)
    (xmlText: string)
    : string =
    if not (hasInheritDoc xmlText) then
        xmlText
    else
        try
            let wrappedXml = "<doc>\n" + xmlText + "\n</doc>"
            let xdoc = XDocument.Parse(wrappedXml, LoadOptions.PreserveWhitespace)

            let directives = extractInheritDocDirectives xdoc

            if directives.IsEmpty then
                xmlText
            else
                for directive in directives do
                    match directive.Cref with
                    | Some cref ->
                        if visited.Contains(cref) then
                            warning (Error(FSComp.SR.xmlDocInheritDocError ($"Circular reference detected for '{cref}'"), m))
                        else
                            match resolveCref cref with
                            | Some inheritedXml ->
                                let expandedInheritedXml =
                                    expandInheritedDoc resolveCref implicitTargetCrefOpt m visited cref inheritedXml

                                let contentToInherit =
                                    match directive.Path with
                                    | Some xpath ->
                                        applyXPathFilter m xpath expandedInheritedXml
                                        |> Option.defaultValue expandedInheritedXml
                                    | None -> expandedInheritedXml

                                try
                                    let newContent = XElement.Parse("<temp>" + contentToInherit + "</temp>")
                                    directive.Element.ReplaceWith(newContent.Nodes())
                                with ex ->
                                    warning (Error(FSComp.SR.xmlDocInheritDocError ($"Failed to process inheritdoc: {ex.Message}"), m))
                            | None -> warning (Error(FSComp.SR.xmlDocInheritDocError ($"Cannot resolve cref '{cref}'"), m))
                    | None ->
                        match implicitTargetCrefOpt with
                        | Some implicitCref ->
                            if visited.Contains(implicitCref) then
                                warning (
                                    Error(
                                        FSComp.SR.xmlDocInheritDocError (
                                            $"Circular reference detected for implicit target '{implicitCref}'"
                                        ),
                                        m
                                    )
                                )
                            else
                                match resolveCref implicitCref with
                                | Some inheritedXml ->
                                    let expandedInheritedXml =
                                        expandInheritedDoc resolveCref None m visited implicitCref inheritedXml

                                    let contentToInherit =
                                        match directive.Path with
                                        | Some xpath ->
                                            applyXPathFilter m xpath expandedInheritedXml
                                            |> Option.defaultValue expandedInheritedXml
                                        | None -> expandedInheritedXml

                                    try
                                        let newContent = XElement.Parse("<temp>" + contentToInherit + "</temp>")
                                        directive.Element.ReplaceWith(newContent.Nodes())
                                    with ex ->
                                        warning (Error(FSComp.SR.xmlDocInheritDocError ($"Failed to process inheritdoc: {ex.Message}"), m))
                                | None ->
                                    warning (Error(FSComp.SR.xmlDocInheritDocError ($"Cannot resolve implicit target '{implicitCref}'"), m))
                        | None ->
                            warning (
                                Error(
                                    FSComp.SR.xmlDocInheritDocError ("Implicit inheritdoc (without cref) requires a base type or interface"),
                                    m
                                )
                            )

                match xdoc.Root with
                | null -> xmlText
                | root ->
                    root.Nodes()
                    |> Seq.map (fun node -> node.ToString(SaveOptions.DisableFormatting))
                    |> String.concat "\n"
        with :? System.Xml.XmlException ->
            xmlText

/// Expands `<inheritdoc>` elements in XML documentation.
/// The caller provides a `resolveCref` function to look up documentation by cref string.
/// Takes an optional implicit target cref for resolving <inheritdoc/> without cref attribute.
/// Tracks visited signatures to prevent infinite recursion.
let expandInheritDoc
    (resolveCref: string -> string option)
    (implicitTargetCrefOpt: string option)
    (m: range)
    (visited: Set<string>)
    (doc: XmlDoc)
    : XmlDoc =
    if doc.IsEmpty then
        doc
    else
        let xmlText = doc.GetXmlText()

        let expandedText =
            expandInheritDocText resolveCref implicitTargetCrefOpt m visited xmlText

        if obj.ReferenceEquals(xmlText, expandedText) || xmlText = expandedText then
            doc
        else
            XmlDoc([| expandedText |], m)

/// Like expandInheritDoc but takes a pre-computed xmlText string, avoiding an extra GetXmlText() call.
/// Use when the caller has already obtained the XML text (e.g. to check for <inheritdoc> presence).
let expandInheritDocFromXmlText
    (resolveCref: string -> string option)
    (implicitTargetCrefOpt: string option)
    (m: range)
    (visited: Set<string>)
    (xmlText: string)
    : string =
    expandInheritDocText resolveCref implicitTargetCrefOpt m visited xmlText
