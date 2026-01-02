// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Symbols

open System
open System.Xml.Linq
open System.Xml.XPath
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Infos
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.Xml

/// Represents a target for XML documentation expansion
[<RequireQualifiedAccess>]
type XmlDocTarget =
    /// Value or member reference
    | Val of ValRef
    /// Type reference
    | Type of TyconRef
    /// Union case reference
    | UnionCase of UnionCaseRef
    /// Record field reference
    | RecdField of RecdFieldRef

module XmlDocInheritance =

    /// Try to find the XmlDoc for the base/interface that this target overrides or implements
    let tryFindInheritedXmlDoc (infoReader: InfoReader) (_m: range) (target: XmlDocTarget) : XmlDoc option =
        match target with
        | XmlDocTarget.Val vref ->
            // Check if this is an override or interface implementation
            match vref.MemberInfo with
            | Some memberInfo ->
                // Check for interface implementations or overrides
                match memberInfo.ImplementedSlotSigs with
                | slotSig :: _ ->
                    // Get the declaring type of the slot
                    match slotSig.DeclaringType with
                    | TType_app (tcref, _, _) ->
                        // Find the member in the interface/base type
                        let slotMemberName = slotSig.Name
                        tcref.MembersOfFSharpTyconSorted
                        |> Seq.tryFind (fun m -> m.LogicalName = slotMemberName)
                        |> Option.bind (fun mref ->
                            let mv = mref.Deref
                            if mv.XmlDoc.NonEmpty then Some mv.XmlDoc else None)
                    | _ -> None
                | [] ->
                    // Check if it's an override without explicit slot sigs
                    if vref.IsDefiniteFSharpOverrideMember then
                        // Try to find base class member
                        // This is a simplified approach - full implementation would traverse base classes
                        None
                    else
                        None
            | None -> None

        | XmlDocTarget.Type tcref ->
            // For types, inherit from base class
            match tcref.TypeContents.tcaug_super with
            | Some superTy ->
                match superTy with
                | TType_app (baseTcref, _, _) ->
                    let btc = baseTcref.Deref
                    if btc.XmlDoc.NonEmpty then Some btc.XmlDoc else None
                | _ -> None
            | None -> None

        | _ -> None

    /// Process a single inheritdoc element
    let processInheritDocElement (infoReader: InfoReader) (m: range) (target: XmlDocTarget) (elem: XElement) (visited: Set<string>) : XElement list * Set<string> =
        try
            // Check for cref attribute (handle nullability)
            let crefAttrValue =
                match elem.Attribute(XName.Get "cref") with
                | null -> None
                | attr -> Some attr.Value
            
            let pathAttrValue =
                match elem.Attribute(XName.Get "path") with
                | null -> None
                | attr -> Some attr.Value
            
            // Try to find inherited documentation
            let inheritedDocOpt =
                match crefAttrValue with
                | Some _ ->
                    // Explicit cref - try to resolve it
                    // For now, we'll skip explicit cref support in this minimal implementation
                    None
                | None ->
                    // Implicit - find from override/interface
                    tryFindInheritedXmlDoc infoReader m target
            
            match inheritedDocOpt with
            | Some inheritedDoc when inheritedDoc.NonEmpty ->
                let inheritedText = inheritedDoc.GetXmlText()
                
                // Parse the inherited XML
                let wrappedXml = "<root>" + inheritedText + "</root>"
                let doc = XDocument.Parse(wrappedXml, LoadOptions.PreserveWhitespace)
                
                // Apply path filter if specified
                let elements =
                    match pathAttrValue with
                    | Some xpath when not (String.IsNullOrWhiteSpace xpath) ->
                        // Adjust xpath to account for root wrapper
                        let adjustedXPath = if xpath.StartsWith("/") then "/*" + xpath else xpath
                        try
                            doc.Root.XPathSelectElements(adjustedXPath) |> List.ofSeq
                        with
                        | _ ->
                            warning (Error(FSComp.SR.xmlDocInheritDocError ("Invalid XPath: " + xpath), m))
                            []
                    | _ ->
                        // Return all child elements
                        doc.Root.Elements() |> List.ofSeq
                
                (elements, visited)
            | _ ->
                // No inherited doc found
                ([], visited)
        with
        | ex ->
            warning (Error(FSComp.SR.xmlDocInheritDocError ex.Message, m))
            ([], visited)

    /// Expands `<inheritdoc>` elements in XML documentation
    let expandInheritDoc (infoReader: InfoReader) (m: range) (target: XmlDocTarget) (doc: XmlDoc) : XmlDoc =
        if doc.IsEmpty then
            doc
        else
            try
                let xmlText = doc.GetXmlText()

                // Check if there are any <inheritdoc> elements
                if not (xmlText.Contains "<inheritdoc") then
                    doc
                else
                    // Parse the XML
                    let wrappedXml = "<root>" + xmlText + "</root>"
                    let xdoc = XDocument.Parse(wrappedXml, LoadOptions.PreserveWhitespace)
                    
                    // Find all inheritdoc elements
                    let inheritdocElements = xdoc.Descendants(XName.Get "inheritdoc") |> List.ofSeq
                    
                    if inheritdocElements.IsEmpty then
                        doc
                    else
                        // Process each inheritdoc element
                        let mutable visited = Set.empty<string>
                        
                        for elem in inheritdocElements do
                            let (replacements, newVisited) = processInheritDocElement infoReader m target elem visited
                            visited <- newVisited
                            
                            // Replace the inheritdoc element with the inherited content
                            if not replacements.IsEmpty then
                                elem.ReplaceWith(replacements |> Array.ofList)
                            else
                                // Remove the inheritdoc element if no replacement found
                                elem.Remove()
                        
                        // Convert back to XmlDoc
                        let newLines =
                            xdoc.Root.Elements()
                            |> Seq.map (fun e -> e.ToString(SaveOptions.DisableFormatting))
                            |> Array.ofSeq
                        
                        XmlDoc(newLines, doc.Range)
            with
            | ex ->
                warning (Error(FSComp.SR.xmlDocInheritDocError ex.Message, m))
                doc
