// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.XmlDocInheritance

open System.Xml.Linq
open System.Xml.XPath

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
let private hasInheritDoc (xmlText: string) =
    xmlText.IndexOf("<inheritdoc", System.StringComparison.Ordinal) >= 0

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

/// Serializes a sequence of XML nodes back to text, one node per line.
let private nodesToString (nodes: seq<#XNode>) : string =
    nodes
    |> Seq.map (fun node -> node.ToString(SaveOptions.DisableFormatting))
    |> String.concat "\n"

/// Applies an XPath filter to XML content
let private applyXPathFilter (xpath: string) (sourceXml: string) : string =
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
            ""
        else
            nodesToString selectedElements
    with
    | :? XPathException
    | :? System.Xml.XmlException
    // XPathSelectElements raises InvalidOperationException when the expression selects non-element
    // nodes (e.g. a text()/node() XPath). Such selections are not supported for inheritance; degrade
    // to no inherited content rather than letting the exception crash the tooltip/completion caller.
    | :? System.InvalidOperationException -> ""

/// Selects the default inherited content, excluding top-level <overloads> nodes.
let private selectDefaultInheritedContent (sourceXml: string) : string =
    try
        let doc =
            XElement.Parse("<temp>" + sourceXml + "</temp>", LoadOptions.PreserveWhitespace)

        doc.Nodes()
        |> Seq.filter (fun node ->
            match node with
            | :? XElement as element -> element.Name.LocalName <> "overloads"
            | _ -> true)
        |> nodesToString
    with :? System.Xml.XmlException ->
        ""

/// Recursively expands inheritdoc in the retrieved documentation
let rec private expandInheritedDoc
    (resolveCref: string -> string option)
    (implicitTargetCrefOpt: string option)
    (visited: Set<string>)
    (cref: string)
    (xmlText: string)
    : string =
    if visited.Contains(cref) then
        xmlText
    else
        let newVisited = visited.Add(cref)
        expandInheritDocFromXmlText resolveCref implicitTargetCrefOpt newVisited xmlText

/// Expands `<inheritdoc>` elements in XML documentation text.
/// The caller provides a `resolveCref` function that maps a cref string to its resolved XML doc text.
/// Takes an optional implicit target cref for resolving <inheritdoc/> without cref attribute.
/// Tracks visited signatures to prevent infinite recursion.
/// Takes a pre-computed xmlText string, avoiding an extra GetXmlText() call.
and expandInheritDocFromXmlText
    (resolveCref: string -> string option)
    (implicitTargetCrefOpt: string option)
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
                let resolveAndReplace (directive: InheritDocDirective) (cref: string) =
                    if visited.Contains(cref) then
                        directive.Element.Remove()
                    else
                        match resolveCref cref with
                        | Some inheritedXml ->
                            // Recurse with no implicit target: a bare <inheritdoc/> nested inside a
                            // resolved doc must inherit from THAT doc's own base (not knowable here,
                            // and not the caller's), so it is dropped rather than resolved against the
                            // wrong target. Only explicit-cref chains propagate through recursion.
                            let expandedInheritedXml =
                                expandInheritedDoc resolveCref None visited cref inheritedXml

                            let contentToInherit =
                                match directive.Path with
                                | Some xpath -> applyXPathFilter xpath expandedInheritedXml
                                | None -> selectDefaultInheritedContent expandedInheritedXml

                            try
                                let newContent = XElement.Parse("<temp>" + contentToInherit + "</temp>")
                                directive.Element.ReplaceWith(newContent.Nodes())
                            with :? System.Xml.XmlException ->
                                directive.Element.Remove()
                        | None -> directive.Element.Remove()

                for directive in directives do
                    match directive.Cref with
                    | Some cref -> resolveAndReplace directive cref
                    | None ->
                        match implicitTargetCrefOpt with
                        | Some implicitCref -> resolveAndReplace directive implicitCref
                        | None -> directive.Element.Remove()

                match xdoc.Root with
                | null -> xmlText
                | root ->
                    root.Nodes()
                    |> Seq.map (fun node -> node.ToString(SaveOptions.DisableFormatting))
                    |> String.concat "\n"
        with _ ->
            // Doc-comment inheritance is best-effort: it must never crash a tooltip or the public
            // FSharpSymbol.XmlDoc. Besides XML parse errors, the caller-supplied resolveCref can throw
            // while walking CCUs (e.g. invalidOp on an unresolved assembly). On any failure, fall back
            // to the original text (which still contains the verbatim <inheritdoc>, harmless downstream).
            xmlText
