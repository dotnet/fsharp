// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.XmlDocInheritance

open System.Xml.Linq
open System.Xml.XPath
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
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
    let inheritDocName = XName.op_Implicit "inheritdoc"

    let crefName = XName.op_Implicit "cref" |> Operators.nonNull
    let pathName = XName.op_Implicit "path" |> Operators.nonNull
    
    doc.Descendants(inheritDocName)
    |> Seq.map (fun elem ->
        let crefAttr = elem.Attribute(crefName)
        let pathAttr = elem.Attribute(pathName)

        {
            Cref = match crefAttr with null -> None | attr -> Some attr.Value
            Path = match pathAttr with null -> None | attr -> Some attr.Value
            Element = elem
        })
    |> List.ofSeq

/// Extracts assembly name from a cref string.
/// For explicit crefs like "M:System.String.Trim", the assembly is inferred from the type path.
/// This is a heuristic - in practice we try known loaded assemblies.
let private extractAssemblyAndSigFromCref (cref: string) : (string * string) option =
    // The cref IS the xmlDocSig (e.g., "M:System.String.Trim")
    // We need to figure out which assembly it belongs to.
    // For now, we try to extract the type name and guess common assemblies.
    if cref.Length > 2 && cref.[1] = ':' then
        let xmlDocSig = cref
        // Extract the type path from the signature
        let entityPart = cref.Substring(2)
        // For methods/properties, the type is everything before the last dot (before any parens)
        let parenIdx = entityPart.IndexOf('(')

        let pathPart =
            if parenIdx > 0 then
                entityPart.Substring(0, parenIdx)
            else
                entityPart

        let lastDot = pathPart.LastIndexOf('.')

        let typePath =
            if lastDot > 0 && cref.[0] <> 'T' then
                pathPart.Substring(0, lastDot)
            else
                pathPart

        // Try to infer assembly from namespace
        let assemblyName =
            if typePath.StartsWith("System.") || typePath = "System" then
                "System.Runtime"
            elif typePath.StartsWith("Microsoft.FSharp.") then
                "FSharp.Core"
            else
                // For user types, we'd need access to the compilation to find the right assembly
                // Return None for now - implicit resolution will handle these
                ""

        if assemblyName = "" then
            None
        else
            Some(assemblyName, xmlDocSig)
    else
        None

/// Parses a cref into a type path (for T: prefix)
/// Handles generic types (T:Foo`1) and nested types (T:Outer+Inner)
/// Returns None if not a type cref or if parsing fails
/// For nested types (contains +), returns both the nested path and an alternative F#-style path
let private parseTypePath (cref: string) : string list option =
    if cref.Length > 2 && cref.[1] = ':' && cref.[0] = 'T' then
        let typePath = cref.Substring(2)
        // Handle nested types: replace + with . for path resolution
        let normalizedPath = typePath.Replace('+', '.')
        // Keep the path as-is, including backticks for generic types
        // Entity names include arity (e.g., Container`1)
        Some(normalizedPath.Split('.') |> Array.toList)
    else
        None

/// For nested type crefs (with +), returns an alternative path where the nested type is at module level
/// E.g., "T:Test.Outer+Inner" -> Some(["Test"; "Inner"]) as F# exposes nested types at module level
let private parseNestedTypeAlternativePath (cref: string) : string list option =
    if cref.Length > 2 && cref.[1] = ':' && cref.[0] = 'T' && cref.Contains("+") then
        let typePath = cref.Substring(2)
        // Find the last + which separates the nested type
        let lastPlus = typePath.LastIndexOf('+')

        if lastPlus > 0 then
            let beforePlus = typePath.Substring(0, lastPlus)
            let nestedTypeName = typePath.Substring(lastPlus + 1)
            // Get the module path (everything before the outer type)
            let lastDotBeforePlus = beforePlus.LastIndexOf('.')

            if lastDotBeforePlus > 0 then
                let modulePath = beforePlus.Substring(0, lastDotBeforePlus)
                Some((modulePath.Split('.') |> Array.toList) @ [ nestedTypeName ])
            else
                // No module, nested type at root
                Some([ nestedTypeName ])
        else
            None
    else
        None

/// Parses a method cref (M: prefix) into (typePath, methodName)
/// E.g., "M:Namespace.Type.Method(System.Int32)" -> (["Namespace"; "Type"], "Method")
let private parseMethodCref (cref: string) : (string list * string) option =
    if cref.Length > 2 && cref.[1] = ':' && cref.[0] = 'M' then
        let entityPart = cref.Substring(2)
        // Remove parameter list for matching
        let parenIdx = entityPart.IndexOf('(')

        let pathPart =
            if parenIdx > 0 then
                entityPart.Substring(0, parenIdx)
            else
                entityPart

        let lastDot = pathPart.LastIndexOf('.')

        if lastDot > 0 then
            let typePath = pathPart.Substring(0, lastDot)
            let methodName = pathPart.Substring(lastDot + 1)
            Some(typePath.Split('.') |> Array.toList, methodName)
        else
            None
    else
        None

/// Parses a property cref (P: prefix) into (typePath, propertyName)
/// E.g., "P:Namespace.Type.PropertyName" -> (["Namespace"; "Type"], "PropertyName")
let private parsePropertyCref (cref: string) : (string list * string) option =
    if cref.Length > 2 && cref.[1] = ':' && cref.[0] = 'P' then
        let entityPart = cref.Substring(2)
        let lastDot = entityPart.LastIndexOf('.')

        if lastDot > 0 then
            let typePath = entityPart.Substring(0, lastDot)
            let propName = entityPart.Substring(lastDot + 1)
            Some(typePath.Split('.') |> Array.toList, propName)
        else
            None
    else
        None

/// Tries to find a member's XmlDoc on an entity by method name
let private tryFindMemberXmlDoc (entity: Entity) (methodName: string) : string option =
    // Search in the type's members
    let members = entity.MembersOfFSharpTyconSorted

    members
    |> List.tryPick (fun vref ->
        if vref.DisplayName = methodName || vref.LogicalName = methodName then
            let doc = vref.XmlDoc
            if doc.IsEmpty then None else Some(doc.GetXmlText())
        else
            None)

/// Tries to find an entity in a module/namespace by path
/// Also handles nested types (e.g., Outer.Inner where Inner is nested in type Outer)
let rec private tryFindEntityByPath (mtyp: ModuleOrNamespaceType) (path: string list) : Entity option =
    match path with
    | [] -> None
    | [ name ] ->
        // Last element - should be the type
        mtyp.AllEntitiesByCompiledAndLogicalMangledNames.TryFind name
    | name :: rest ->
        // Navigate into a module/namespace OR a type with nested types
        match mtyp.AllEntitiesByCompiledAndLogicalMangledNames.TryFind name with
        | Some entity when entity.IsModuleOrNamespace -> tryFindEntityByPath entity.ModuleOrNamespaceType rest
        | Some entity ->
            // Entity is a type - check for nested types inside it
            tryFindEntityByPath entity.ModuleOrNamespaceType rest
        | None -> None

/// Tries to find an entity in the CCU by type path
/// First tries direct path, then searches within nested modules
let private tryFindEntityInCcu (ccu: CcuThunk) (path: string list) : Entity option =
    let rootMtyp = ccu.Contents.ModuleOrNamespaceType

    // Try direct path first
    match tryFindEntityByPath rootMtyp path with
    | Some entity -> Some entity
    | None ->
        // If the first path element matches the CCU name, try the rest of the path directly
        // This handles the case where `module Test` creates a CCU named "Test"
        // and types are at the root level
        match path with
        | ccuName :: rest when ccuName = ccu.AssemblyName || ccuName = ccu.Contents.LogicalName ->
            match rest with
            | [] -> None // Can't resolve to the CCU itself
            | _ -> tryFindEntityByPath rootMtyp rest
        | [] -> None
        | moduleName :: rest ->
            // Check if any root module matches
            let foundInRoots =
                rootMtyp.ModuleAndNamespaceDefinitions
                |> List.tryPick (fun m ->
                    if m.LogicalName = moduleName || m.CompiledName = moduleName then
                        match rest with
                        | [] -> Some m
                        | _ -> tryFindEntityByPath m.ModuleOrNamespaceType rest
                    else
                        None)

            match foundInRoots with
            | Some e -> Some e
            | None ->
                // Last resort: recursively search all nested modules for the first path element
                let rec searchNested (mtyp: ModuleOrNamespaceType) =
                    // First, try to find the first path element directly
                    match tryFindEntityByPath mtyp path with
                    | Some e -> Some e
                    | None ->
                        // Search within all nested modules
                        mtyp.ModuleAndNamespaceDefinitions
                        |> List.tryPick (fun m -> searchNested m.ModuleOrNamespaceType)

                searchNested rootMtyp

/// Attempts to retrieve XML documentation from a ModuleOrNamespaceType by cref
/// This is used for same-compilation resolution where we have direct access to the typed module content
let private tryGetXmlDocFromModuleType (ccuName: string) (mtyp: ModuleOrNamespaceType) (cref: string) : string option =
    // Helper to find entity doc by path with various fallbacks
    let tryFindWithFallbacks (path: string list) =
        // Try direct path first
        match tryFindEntityByPath mtyp path with
        | Some entity ->
            let doc = entity.XmlDoc
            if doc.IsEmpty then None else Some(doc.GetXmlText())
        | None ->
            // If the first path element matches the CCU name, try the rest directly
            match path with
            | firstPart :: rest when firstPart = ccuName && not rest.IsEmpty ->
                match tryFindEntityByPath mtyp rest with
                | Some entity ->
                    let doc = entity.XmlDoc
                    if doc.IsEmpty then None else Some(doc.GetXmlText())
                | None -> None
            | moduleName :: rest ->
                // Check if any root module matches (handles `module Test` at top level)
                mtyp.ModuleAndNamespaceDefinitions
                |> List.tryPick (fun m ->
                    if m.LogicalName = moduleName || m.CompiledName = moduleName then
                        match rest with
                        | [] ->
                            let doc = m.XmlDoc
                            if doc.IsEmpty then None else Some(doc.GetXmlText())
                        | _ ->
                            match tryFindEntityByPath m.ModuleOrNamespaceType rest with
                            | Some entity ->
                                let doc = entity.XmlDoc
                                if doc.IsEmpty then None else Some(doc.GetXmlText())
                            | None -> None
                    else
                        None)
            | _ -> None

    match parseTypePath cref with
    | Some path ->
        match tryFindWithFallbacks path with
        | Some doc -> Some doc
        | None ->
            // For nested types (Outer+Inner), try F#-style path (just Inner at module level)
            match parseNestedTypeAlternativePath cref with
            | Some altPath -> tryFindWithFallbacks altPath
            | None -> None
    | None ->
        // Try method cref
        match parseMethodCref cref with
        | Some(typePath, methodName) ->
            match tryFindEntityByPath mtyp typePath with
            | Some entity -> tryFindMemberXmlDoc entity methodName
            | None ->
                // Try with CCU name prefix stripped
                match typePath with
                | firstPart :: rest when firstPart = ccuName && not rest.IsEmpty ->
                    match tryFindEntityByPath mtyp rest with
                    | Some entity -> tryFindMemberXmlDoc entity methodName
                    | None -> None
                | _ -> None
        | None ->
            // Try property cref
            match parsePropertyCref cref with
            | Some(typePath, propName) ->
                match tryFindEntityByPath mtyp typePath with
                | Some entity -> tryFindMemberXmlDoc entity propName
                | None ->
                    match typePath with
                    | firstPart :: rest when firstPart = ccuName && not rest.IsEmpty ->
                        match tryFindEntityByPath mtyp rest with
                        | Some entity -> tryFindMemberXmlDoc entity propName
                        | None -> None
                    | _ -> None
            | None -> None

/// Attempts to retrieve XML documentation from a CCU by cref
let private tryGetXmlDocFromCcu (ccu: CcuThunk) (cref: string) : string option =
    match parseTypePath cref with
    | Some path ->
        match tryFindEntityInCcu ccu path with
        | Some entity ->
            let doc = entity.XmlDoc
            if doc.IsEmpty then None else Some(doc.GetXmlText())
        | None -> None
    | None ->
        // Try method cref
        match parseMethodCref cref with
        | Some(typePath, methodName) ->
            match tryFindEntityInCcu ccu typePath with
            | Some entity -> tryFindMemberXmlDoc entity methodName
            | None -> None
        | None ->
            // Try property cref
            match parsePropertyCref cref with
            | Some(typePath, propName) ->
                match tryFindEntityInCcu ccu typePath with
                | Some entity -> tryFindMemberXmlDoc entity propName
                | None -> None
            | None -> None

/// Attempts to retrieve XML documentation for a given cref
/// Tries current module type first (same-compilation), then CCU, then external assemblies
let private tryGetXmlDocByCref
    (infoReaderOpt: InfoReader option)
    (ccuOpt: CcuThunk option)
    (currentModuleTypeOpt: ModuleOrNamespaceType option)
    (cref: string)
    : string option =
    // First try to resolve from same-compilation module type (most precise)
    let fromModuleType =
        match currentModuleTypeOpt, ccuOpt with
        | Some mtyp, Some ccu -> tryGetXmlDocFromModuleType ccu.AssemblyName mtyp cref
        | Some mtyp, None ->
            // Try with empty CCU name
            tryGetXmlDocFromModuleType "" mtyp cref
        | None, _ -> None

    match fromModuleType with
    | Some doc -> Some doc
    | None ->
        // Try CCU resolution
        match ccuOpt with
        | Some ccu ->
            match tryGetXmlDocFromCcu ccu cref with
            | Some doc -> Some doc
            | None ->
                // Fall back to external assembly resolution
                match infoReaderOpt with
                | Some infoReader ->
                    match extractAssemblyAndSigFromCref cref with
                    | Some(assemblyName, xmlDocSig) ->
                        TryFindXmlDocByAssemblyNameAndSig infoReader assemblyName xmlDocSig
                        |> Option.bind (fun xmlDoc -> if xmlDoc.IsEmpty then None else Some(xmlDoc.GetXmlText()))
                    | None -> None
                | None -> None
        | None ->
            // No CCU available, try external assembly resolution only
            match infoReaderOpt with
            | Some infoReader ->
                match extractAssemblyAndSigFromCref cref with
                | Some(assemblyName, xmlDocSig) ->
                    TryFindXmlDocByAssemblyNameAndSig infoReader assemblyName xmlDocSig
                    |> Option.bind (fun xmlDoc -> if xmlDoc.IsEmpty then None else Some(xmlDoc.GetXmlText()))
                | None -> None
            | None -> None

/// Applies an XPath filter to XML content
let private applyXPathFilter (m: range) (xpath: string) (sourceXml: string) : string option =
    try
        let doc =
            XDocument.Parse("<doc>" + sourceXml + "</doc>", LoadOptions.PreserveWhitespace)

        // If the xpath starts with /, it's an absolute path that won't work with our wrapper
        // Adjust to search within the doc
        let adjustedXpath =
            if xpath.StartsWith("/") && not (xpath.StartsWith("//")) then
                // Convert absolute path to search within doc
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
    (infoReaderOpt: InfoReader option)
    (ccuOpt: CcuThunk option)
    (currentModuleTypeOpt: ModuleOrNamespaceType option)
    (implicitTargetCrefOpt: string option)
    (m: range)
    (visited: Set<string>)
    (cref: string)
    (xmlText: string)
    : string =
    // Check for cycles
    if visited.Contains(cref) then
        // Cycle detected - return original doc to prevent infinite recursion
        xmlText
    else
        let newVisited = visited.Add(cref)
        expandInheritDocText infoReaderOpt ccuOpt currentModuleTypeOpt implicitTargetCrefOpt m newVisited xmlText

/// Expands `<inheritdoc>` elements in XML documentation text
/// Uses InfoReader to resolve cref targets to their documentation
/// Tracks visited signatures to prevent infinite recursion
and private expandInheritDocText
    (infoReaderOpt: InfoReader option)
    (ccuOpt: CcuThunk option)
    (currentModuleTypeOpt: ModuleOrNamespaceType option)
    (implicitTargetCrefOpt: string option)
    (m: range)
    (visited: Set<string>)
    (xmlText: string)
    : string =
    // Quick check: if no <inheritdoc> present, return original
    if not (hasInheritDoc xmlText) then
        xmlText
    else
        try
            // Parse the XML document
            // Wrap in <doc> to ensure single root element
            let wrappedXml = "<doc>\n" + xmlText + "\n</doc>"
            let xdoc = XDocument.Parse(wrappedXml, LoadOptions.PreserveWhitespace)

            // Find all <inheritdoc> elements
            let directives = extractInheritDocDirectives xdoc

            if directives.IsEmpty then
                xmlText
            else
                // Process each directive
                for directive in directives do
                    match directive.Cref with
                    | Some cref ->
                        // Check for cycles
                        if visited.Contains(cref) then
                            warning (Error(FSComp.SR.xmlDocInheritDocError ($"Circular reference detected for '{cref}'"), m))
                        else
                            // Try to resolve the cref and get its documentation
                            match tryGetXmlDocByCref infoReaderOpt ccuOpt currentModuleTypeOpt cref with
                            | Some inheritedXml ->
                                // Recursively expand the inherited doc
                                let expandedInheritedXml =
                                    expandInheritedDoc
                                        infoReaderOpt
                                        ccuOpt
                                        currentModuleTypeOpt
                                        implicitTargetCrefOpt
                                        m
                                        visited
                                        cref
                                        inheritedXml

                                // Apply path filter if specified
                                let contentToInherit =
                                    match directive.Path with
                                    | Some xpath ->
                                        applyXPathFilter m xpath expandedInheritedXml
                                        |> Option.defaultValue expandedInheritedXml
                                    | None -> expandedInheritedXml

                                // Replace the <inheritdoc> element with the inherited content
                                try
                                    let newContent = XElement.Parse("<temp>" + contentToInherit + "</temp>")
                                    directive.Element.ReplaceWith(newContent.Nodes())
                                with ex ->
                                    warning (Error(FSComp.SR.xmlDocInheritDocError ($"Failed to process inheritdoc: {ex.Message}"), m))
                            | None ->
                                // Only warn if we have some resolution capability but still failed
                                if infoReaderOpt.IsSome || ccuOpt.IsSome || currentModuleTypeOpt.IsSome then
                                    warning (Error(FSComp.SR.xmlDocInheritDocError ($"Cannot resolve cref '{cref}'"), m))
                    | None ->
                        // Implicit inheritdoc - use the implicit target if provided
                        match implicitTargetCrefOpt with
                        | Some implicitCref ->
                            // Check for cycles
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
                                // Try to resolve the implicit target
                                match tryGetXmlDocByCref infoReaderOpt ccuOpt currentModuleTypeOpt implicitCref with
                                | Some inheritedXml ->
                                    let expandedInheritedXml =
                                        expandInheritedDoc
                                            infoReaderOpt
                                            ccuOpt
                                            currentModuleTypeOpt
                                            None
                                            m
                                            visited
                                            implicitCref
                                            inheritedXml

                                    // Apply path filter if specified
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
                                    if infoReaderOpt.IsSome || ccuOpt.IsSome || currentModuleTypeOpt.IsSome then
                                        warning (
                                            Error(FSComp.SR.xmlDocInheritDocError ($"Cannot resolve implicit target '{implicitCref}'"), m)
                                        )
                        | None ->
                            warning (
                                Error(
                                    FSComp.SR.xmlDocInheritDocError ("Implicit inheritdoc (without cref) requires a base type or interface"),
                                    m
                                )
                            )

                // Return the modified document
                // Extract content from the wrapper <doc> element
                match xdoc.Root with
                | null -> xmlText
                | root ->
                    root.Nodes()
                    |> Seq.map (fun node -> node.ToString(SaveOptions.DisableFormatting))
                    |> String.concat "\n"
        with :? System.Xml.XmlException ->
            // If XML parsing fails, return original doc unchanged
            xmlText

/// Expands `<inheritdoc>` elements in XML documentation
/// Uses InfoReader to resolve cref targets to their documentation
/// Uses CCU for same-compilation type resolution
/// Takes an optional implicit target cref for resolving <inheritdoc/> without cref attribute
/// Tracks visited signatures to prevent infinite recursion
let expandInheritDoc
    (infoReaderOpt: InfoReader option)
    (ccuOpt: CcuThunk option)
    (currentModuleTypeOpt: ModuleOrNamespaceType option)
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
            expandInheritDocText infoReaderOpt ccuOpt currentModuleTypeOpt implicitTargetCrefOpt m visited xmlText

        if obj.ReferenceEquals(xmlText, expandedText) || xmlText = expandedText then
            doc
        else
            XmlDoc([| expandedText |], m)
