// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Symbols.XmlDocInheritance

open System
open System.Xml.Linq
open System.Xml.XPath
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Symbols.XmlDocSigParser
open FSharp.Compiler.Text
open FSharp.Compiler.Xml

/// Represents an inheritdoc directive found in XML documentation
type InheritDocDirective = {
    /// Optional cref attribute specifying explicit target
    Cref: string option
    /// Optional path attribute for XPath filtering
    Path: string option
    /// The original XElement for replacement
    Element: XElement
}

/// Checks if an XML document contains <inheritdoc> elements
let private hasInheritDoc (xmlText: string) =
    xmlText.Contains("<inheritdoc", StringComparison.Ordinal)

/// Extracts inheritdoc directives from parsed XML
let private extractInheritDocDirectives (doc: XDocument) =
    let inheritDocName = XName.op_Implicit "inheritdoc"
    
    doc.Descendants(inheritDocName)
    |> Seq.map (fun elem ->
        let crefAttr = elem.Attribute(XName.op_Implicit "cref")
        let pathAttr = elem.Attribute(XName.op_Implicit "path")
        
        {
            Cref = if isNull crefAttr then None else Some crefAttr.Value
            Path = if isNull pathAttr then None else Some pathAttr.Value
            Element = elem
        })
    |> List.ofSeq

/// Attempts to retrieve XML documentation for a given cref from InfoReader
let private tryGetXmlDocByCref (infoReader: InfoReader) (cref: string) : XmlDoc option =
    try
        // Use InfoReader's TryFindXmlDocByAssemblyNameAndSig to look up external docs
        // For now, we'll use a simplified approach
        infoReader.TryFindXmlDocByAssemblyNameAndSig(cref)
        |> Option.map (fun xmlText -> XmlDoc([|xmlText|], range0))
    with
    | _ -> None

/// Recursively expands inheritdoc in the retrieved documentation
let rec private expandInheritedDoc (infoReader: InfoReader option) (m: range) (visited: Set<string>) (cref: string) (doc: XmlDoc) : XmlDoc =
    // Check for cycles
    if visited.Contains(cref) then
        // Cycle detected - return original doc to prevent infinite recursion
        doc
    else
        let newVisited = visited.Add(cref)
        expandInheritDoc infoReader m newVisited doc

/// Applies an XPath filter to XML content
let private applyXPathFilter (m: range) (xpath: string) (sourceXml: string) : string option =
    try
        let doc = XDocument.Parse("<doc>" + sourceXml + "</doc>", LoadOptions.PreserveWhitespace)
        let selectedElements = doc.XPathSelectElements(xpath)
        
        if Seq.isEmpty selectedElements then
            None
        else
            let result = 
                selectedElements
                |> Seq.map (fun elem -> elem.ToString(SaveOptions.DisableFormatting))
                |> String.concat "\n"
            Some result
    with
    | ex ->
        warning (Error(FSComp.SR.xmlDocInheritDocError($"invalid XPath '{xpath}': {ex.Message}"), m))
        None

/// Expands `<inheritdoc>` elements in XML documentation
/// Uses InfoReader to resolve cref targets to their documentation
/// Tracks visited signatures to prevent infinite recursion
and expandInheritDoc (infoReaderOpt: InfoReader option) (m: range) (visited: Set<string>) (doc: XmlDoc) : XmlDoc =
    if doc.IsEmpty then
        doc
    else
        let xmlText = doc.GetXmlText()
        
        // Quick check: if no <inheritdoc> present, return original
        if not (hasInheritDoc xmlText) then
            doc
        else
            try
                // Parse the XML document
                // Wrap in <doc> to ensure single root element
                let wrappedXml = "<doc>\n" + xmlText + "\n</doc>"
                let xdoc = XDocument.Parse(wrappedXml, LoadOptions.PreserveWhitespace)
                
                // Find all <inheritdoc> elements
                let directives = extractInheritDocDirectives xdoc
                
                if directives.IsEmpty then
                    doc
                else
                    // Process each directive
                    for directive in directives do
                        match directive.Cref, infoReaderOpt with
                        | Some cref, Some infoReader ->\n                            // Check for cycles
                            if visited.Contains(cref) then
                                warning (Error(FSComp.SR.xmlDocInheritDocError($"Circular reference detected for '{cref}'"), m))
                            else
                                // Try to resolve the cref and get its documentation
                                match tryGetXmlDocByCref infoReader cref with
                                | Some inheritedDoc ->\n                                    // Recursively expand the inherited doc
                                    let expandedInheritedDoc = expandInheritedDoc infoReaderOpt m visited cref inheritedDoc
                                    let inheritedXml = expandedInheritedDoc.GetXmlText()
                                    
                                    // Apply path filter if specified
                                    let contentToInherit =
                                        match directive.Path with
                                        | Some xpath -> 
                                            applyXPathFilter xpath inheritedXml
                                            |> Option.defaultValue inheritedXml
                                        | None -> inheritedXml
                                    
                                    // Replace the <inheritdoc> element with the inherited content
                                    try
                                        let newContent = XElement.Parse("<temp>" + contentToInherit + "</temp>")
                                        directive.Element.ReplaceWith(newContent.Nodes())
                                    with
                                    | ex -> 
                                        warning (Error(FSComp.SR.xmlDocInheritDocError($"Failed to process inheritdoc: {ex.Message}"), m))
                                | None ->
                                    warning (Error(FSComp.SR.xmlDocInheritDocError($"Cannot resolve cref '{cref}'"), m))
                        | Some cref, None ->
                            warning (Error(FSComp.SR.xmlDocInheritDocError($"Cannot resolve cref '{cref}' without symbol information"), m))
                        | None, _ ->
                            warning (Error(FSComp.SR.xmlDocInheritDocError("Implicit inheritdoc (without cref) is not yet supported"), m))
                    
                    // Return the modified document
                    // Extract content from the wrapper <doc> element
                    let root = xdoc.Root
                    let modifiedXml = 
                        root.Nodes()
                        |> Seq.map (fun node -> node.ToString(SaveOptions.DisableFormatting))
                        |> String.concat "\n"
                    
                    XmlDoc([|modifiedXml|], m)
            with
            | :? System.Xml.XmlException ->
                // If XML parsing fails, return original doc unchanged
                doc
