// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Text.RegularExpressions
open Internal.Utilities.Collections
open EnvDTE
open EnvDTE80
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.Layout.TaggedTextOps

type internal ITaggedTextCollector =
    abstract Add: text: TaggedText -> unit
    abstract EndsWithLineBreak: bool
    abstract IsEmpty: bool
    abstract StartXMLDoc: unit -> unit

type internal TextSanitizingCollector(collector, ?lineLimit: int) =
    let mutable isEmpty = true 
    let mutable endsWithLineBreak = false
    let mutable count = 0
    let mutable startXmlDoc = false

    let addTaggedTextEntry (text:TaggedText) =
        match lineLimit with
        | Some lineLimit when lineLimit = count ->
            // add ... when line limit is reached
            collector (tagText "...")
            count <- count + 1
        | _ ->
            isEmpty <- false
            endsWithLineBreak <- text.Tag = LayoutTag.LineBreak
            if endsWithLineBreak then count <- count + 1
            collector text
    
    static let splitTextRegex = Regex(@"\s*\n\s*\n\s*", RegexOptions.Compiled ||| RegexOptions.ExplicitCapture)
    static let normalizeSpacesRegex = Regex(@"\s+", RegexOptions.Compiled ||| RegexOptions.ExplicitCapture)

    let reportTextLines (s: string) =
        // treat _double_ newlines as line breaks and remove all \n after that
        let paragraphs = splitTextRegex.Split(s.Replace("\r", "")) |> Array.filter (not << String.IsNullOrWhiteSpace)
        paragraphs
        |> Array.iteri (fun i paragraph ->
            let paragraph = normalizeSpacesRegex.Replace(paragraph, " ")
            let paragraph = 
                // it's the first line of XML Doc. It often has heading '\n' and spaces, we should remove it.
                // We should not remove them from subsequent lines, because spaces may be proper delimiters 
                // between plane text and formatted code.
                if startXmlDoc then 
                    startXmlDoc <- false
                    paragraph.TrimStart() 
                else paragraph
                
            addTaggedTextEntry (tagText paragraph)
            if i < paragraphs.Length - 1 then
                // insert two line breaks to separate paragraphs
                addTaggedTextEntry Literals.lineBreak
                addTaggedTextEntry Literals.lineBreak)

    interface ITaggedTextCollector with
        member this.Add taggedText = 
            // TODO: bail out early if line limit is already hit
            match taggedText.Tag with
            | LayoutTag.Text -> reportTextLines taggedText.Text
            | _ -> addTaggedTextEntry taggedText

        member this.IsEmpty = isEmpty
        member this.EndsWithLineBreak = isEmpty || endsWithLineBreak
        member this.StartXMLDoc() = startXmlDoc <- true

/// XmlDocumentation builder, using the VS interfaces to build documentation.  An interface is used
/// to allow unit testing to give an alternative implementation which captures the documentation.
type internal IDocumentationBuilder =

    /// Append the given raw XML formatted into the string builder
    abstract AppendDocumentationFromProcessedXML : xmlCollector: ITaggedTextCollector * exnCollector: ITaggedTextCollector * processedXml:string * showExceptions:bool * showParameters:bool * paramName:string option-> unit

    /// Appends text for the given filename and signature into the StringBuilder
    abstract AppendDocumentation : xmlCollector: ITaggedTextCollector * exnCollector: ITaggedTextCollector * filename: string * signature: string * showExceptions: bool * showParameters: bool * paramName: string option-> unit

/// Documentation helpers.
module internal XmlDocumentation =
    open System.Security

    /// If the XML comment starts with '<' not counting whitespace then treat it as a literal XML comment.
    /// Otherwise, escape it and surround it with <summary></summary>
    let ProcessXml(xml:string) =
        if String.IsNullOrEmpty(xml) then xml
        else
            let trimmedXml = xml.TrimStart([|' ';'\r';'\n'|])
            if trimmedXml.Length > 0 then
                if trimmedXml.[0] <> '<' then 
                    // This code runs for local/within-project xmldoc tooltips, but not for cross-project or .XML - for that see ast.fs in the compiler
                    let escapedXml = SecurityElement.Escape(xml)
                    "<summary>" + escapedXml + "</summary>"
                else 
                    "<root>" + xml + "</root>"
            else xml

    let AppendHardLine(collector: ITaggedTextCollector) =
        collector.Add Literals.lineBreak
       
    let EnsureHardLine(collector: ITaggedTextCollector) =
        if not collector.EndsWithLineBreak then AppendHardLine collector
        
    let AppendOnNewLine (collector: ITaggedTextCollector) (line:string) =
        if line.Length > 0 then 
            EnsureHardLine collector
            collector.Add(TaggedTextOps.tagText line)

    open System.Xml
    open System.Xml.Linq

    let rec private WriteElement (collector: ITaggedTextCollector) (n: XNode) = 
        match n.NodeType with
        | XmlNodeType.Text -> 
            WriteText collector (n :?> XText)
        | XmlNodeType.Element ->
            let el = n :?> XElement
            match el.Name.LocalName with
            | "see" | "seealso" -> 
                for attr in el.Attributes() do
                    WriteAttribute collector attr "cref" (WriteTypeName collector)
            | "paramref" | "typeref" ->
                for attr in el.Attributes() do
                    WriteAttribute collector attr "name" (tagParameter >> collector.Add)
            | _ -> 
                WriteNodes collector (el.Nodes())
        | _ -> ()
                
    and WriteNodes (collector: ITaggedTextCollector) (nodes: seq<XNode>) = 
        for n in nodes do
            WriteElement collector n

    and WriteText (collector: ITaggedTextCollector) (n: XText) = 
        collector.Add(tagText n.Value)

    and WriteAttribute (collector: ITaggedTextCollector) (attr: XAttribute) (taggedName: string) tagger = 
        if attr.Name.LocalName = taggedName then
            tagger attr.Value
        else
            collector.Add(tagText attr.Value)

    and WriteTypeName (collector: ITaggedTextCollector) (typeName: string) =
        let typeName = if typeName.StartsWith("T:") then typeName.Substring(2) else typeName
        let parts = typeName.Split([|'.'|])
        for i = 0 to parts.Length - 2 do
            collector.Add(tagNamespace parts.[i])
            collector.Add(Literals.dot)
        collector.Add(tagClass parts.[parts.Length - 1])

    type XmlDocReader private (doc: XElement) = 

        let tryFindParameter name = 
            doc.Descendants (XName.op_Implicit "param")
            |> Seq.tryFind (fun el -> 
                match el.Attribute(XName.op_Implicit "name") with
                | null -> false
                | attr -> attr.Value = name)

        static member TryCreate (xml: string) =
            try Some (XmlDocReader(XElement.Parse(ProcessXml xml))) with _ -> None

        member __.CollectSummary(collector: ITaggedTextCollector) = 
            match Seq.tryHead (doc.Descendants(XName.op_Implicit "summary")) with
            | None -> ()
            | Some el ->
                EnsureHardLine collector
                WriteElement collector el

        member this.CollectParameter(collector: ITaggedTextCollector, paramName: string) =
            match tryFindParameter paramName with
            | None -> ()
            | Some el ->
                EnsureHardLine collector
                WriteNodes collector (el.Nodes())
           
        member this.CollectParameters(collector: ITaggedTextCollector) =
            for p in doc.Descendants(XName.op_Implicit "param") do
                match p.Attribute(XName.op_Implicit "name") with
                | null -> ()
                | name ->
                    EnsureHardLine collector
                    collector.Add(tagParameter name.Value)
                    collector.Add(Literals.colon)
                    collector.Add(Literals.space)
                    WriteNodes collector (p.Nodes())

        member this.CollectExceptions(collector: ITaggedTextCollector) =
            let mutable started = false;
            for p in doc.Descendants(XName.op_Implicit "exception") do
                match p.Attribute(XName.op_Implicit "cref") with
                | null -> ()
                | exnType ->
                    if not started then
                        started <- true
                        AppendHardLine collector
                        AppendOnNewLine collector SR.ExceptionsLabel.Value
                    EnsureHardLine collector
                    collector.Add(tagSpace "    ")
                    WriteTypeName collector exnType.Value
                    if not (Seq.isEmpty (p.Nodes())) then
                        collector.Add Literals.space
                        collector.Add Literals.minus
                        collector.Add Literals.space
                        WriteNodes collector (p.Nodes())

    type VsThreadToken() = class end
    let vsToken = VsThreadToken()
    
    /// Provide Xml Documentation             
    type Provider(xmlIndexService:IVsXMLMemberIndexService, dte: DTE) = 
        /// Index of assembly name to xml member index.
        let mutable xmlCache = new AgedLookup<VsThreadToken,string,IVsXMLMemberIndex>(10,areSame=(fun (x,y) -> x = y))
        
        let events = dte.Events :?> Events2
        let solutionEvents = events.SolutionEvents
        do solutionEvents.add_AfterClosing(fun () -> 
            xmlCache.Clear(vsToken))

    #if DEBUG // Keep under DEBUG so that it can keep building.

        let _AppendTypeParameters (collector: ITaggedTextCollector) (memberData:IVsXMLMemberData3) = 
            let ok,count = memberData.GetTypeParamCount()
            if Com.Succeeded(ok) && count > 0 then 
                for param in 0..count do
                    let ok,name,text = memberData.GetTypeParamTextAt(param)
                    if Com.Succeeded(ok) then
                        EnsureHardLine collector
                        collector.Add(tagTypeParameter name)
                        collector.Add(Literals.space)
                        collector.Add(tagPunctuation "-")
                        collector.Add(Literals.space)
                        collector.Add(tagText text)

        let _AppendRemarks (collector: ITaggedTextCollector) (memberData:IVsXMLMemberData3) = 
            let ok,remarksText = memberData.GetRemarksText()
            if Com.Succeeded(ok) then 
                AppendOnNewLine collector remarksText            
    #endif

        let _AppendReturns (collector: ITaggedTextCollector) (memberData:IVsXMLMemberData3) = 
            let ok,returnsText = memberData.GetReturnsText()
            if Com.Succeeded(ok) then 
                if not collector.EndsWithLineBreak then 
                    AppendHardLine(collector)
                    AppendHardLine(collector)
                AppendOnNewLine collector returnsText

        /// Retrieve the pre-existing xml index or None
        let GetMemberIndexOfAssembly(assemblyName) =
            match xmlCache.TryGet(vsToken, assemblyName) with 
            | Some(memberIndex) -> Some(memberIndex)
            | None -> 
                let ok,memberIndex = xmlIndexService.CreateXMLMemberIndex(assemblyName)
                if Com.Succeeded(ok) then 
                    let ok = memberIndex.BuildMemberIndex()
                    if Com.Succeeded(ok) then 
                        xmlCache.Put(vsToken, assemblyName,memberIndex)
                        Some(memberIndex)
                    else None
                else None

        let AppendMemberData(xmlCollector: ITaggedTextCollector, exnCollector: ITaggedTextCollector, xmlDocReader: XmlDocReader, showExceptions, showParameters) =
            AppendHardLine xmlCollector
            xmlCollector.StartXMLDoc()
            xmlDocReader.CollectSummary(xmlCollector)

            if (showParameters) then
                xmlDocReader.CollectParameters xmlCollector
            if (showExceptions) then 
                xmlDocReader.CollectExceptions exnCollector

        interface IDocumentationBuilder with 
            /// Append the given processed XML formatted into the string builder
            override this.AppendDocumentationFromProcessedXML(xmlCollector, exnCollector, processedXml, showExceptions, showParameters, paramName) =
                match XmlDocReader.TryCreate processedXml with
                | Some xmlDocReader ->
                    match paramName with
                    | Some paramName -> xmlDocReader.CollectParameter(xmlCollector, paramName)
                    | None -> AppendMemberData(xmlCollector, exnCollector, xmlDocReader, showExceptions,showParameters)
                | None -> ()

            /// Append Xml documentation contents into the StringBuilder
            override this.AppendDocumentation
                            ( /// ITaggedTextCollector to add to
                              xmlCollector: ITaggedTextCollector,
                              /// ITaggedTextCollector to add to
                              exnCollector: ITaggedTextCollector,
                              /// Name of the library file
                              filename:string,
                              /// Signature of the comment
                              signature:string,
                              /// Whether to show exceptions
                              showExceptions:bool,
                              /// Whether to show parameters and return
                              showParameters:bool,
                              /// Name of parameter
                              paramName:string option                            
                             ) = 
                try     
                    match GetMemberIndexOfAssembly(filename) with
                    | Some(index) ->
                        let _,idx = index.ParseMemberSignature(signature)
                        if idx <> 0u then
                            let ok,xml = index.GetMemberXML(idx)
                            if Com.Succeeded(ok) then 
                                (this:>IDocumentationBuilder).AppendDocumentationFromProcessedXML(xmlCollector, exnCollector, xml, showExceptions, showParameters, paramName)
                    | None -> ()
                with e-> 
                    Assert.Exception(e)
                    reraise()    
 
    /// Append an XmlCommnet to the segment.
    let AppendXmlComment(documentationProvider:IDocumentationBuilder, xmlCollector: ITaggedTextCollector, exnCollector: ITaggedTextCollector, xml, showExceptions, showParameters, paramName) =
        match xml with
        | FSharpXmlDoc.None -> ()
        | FSharpXmlDoc.XmlDocFileSignature(filename,signature) -> 
            documentationProvider.AppendDocumentation(xmlCollector, exnCollector, filename, signature, showExceptions, showParameters, paramName)
        | FSharpXmlDoc.Text(rawXml) ->
            let processedXml = ProcessXml(rawXml)
            documentationProvider.AppendDocumentationFromProcessedXML(xmlCollector, exnCollector, processedXml, showExceptions, showParameters, paramName)

    let private AddSeparator (collector: ITaggedTextCollector) =
        if not collector.IsEmpty then
            EnsureHardLine collector
            collector.Add (tagText "-------------")
            AppendHardLine collector

    /// Build a data tip text string with xml comments injected.
    let BuildTipText(documentationProvider:IDocumentationBuilder, 
                     dataTipText: FSharpStructuredToolTipElement list,
                     textCollector, xmlCollector,  typeParameterMapCollector, usageCollector, exnCollector,
                     showText, showExceptions, showParameters) = 
        let textCollector: ITaggedTextCollector = TextSanitizingCollector(textCollector, lineLimit = 45) :> _
        let xmlCollector: ITaggedTextCollector = TextSanitizingCollector(xmlCollector, lineLimit = 45) :> _
        let typeParameterMapCollector: ITaggedTextCollector = TextSanitizingCollector(typeParameterMapCollector, lineLimit = 6) :> _
        let exnCollector: ITaggedTextCollector = TextSanitizingCollector(exnCollector, lineLimit = 45) :> _
        let usageCollector: ITaggedTextCollector = TextSanitizingCollector(usageCollector, lineLimit = 45) :> _

        let addSeparatorIfNecessary add =
            if add then
                AddSeparator textCollector
                AddSeparator xmlCollector

        let ProcessGenericParameters (tps: Layout list) =
            if not tps.IsEmpty then
                AppendHardLine typeParameterMapCollector
                AppendOnNewLine typeParameterMapCollector SR.GenericParametersLabel.Value
                for tp in tps do 
                    AppendHardLine typeParameterMapCollector
                    typeParameterMapCollector.Add(tagSpace "    ")
                    renderL (taggedTextListR typeParameterMapCollector.Add) tp |> ignore

        let Process add (dataTipElement: FSharpStructuredToolTipElement) =

            match dataTipElement with 
            | FSharpStructuredToolTipElement.None -> 
                false

            | FSharpStructuredToolTipElement.Group (overloads) -> 
                let overloads = Array.ofList overloads
                let len = overloads.Length
                if len >= 1 then
                    addSeparatorIfNecessary add
                    if showText then 
                        let AppendOverload (item: FSharpToolTipElementData<_>) = 
                            if not(isEmptyL item.MainDescription) then
                                if not textCollector.IsEmpty then 
                                    AppendHardLine textCollector
                                renderL (taggedTextListR textCollector.Add) item.MainDescription |> ignore

                        AppendOverload(overloads.[0])
                        if len >= 2 then AppendOverload(overloads.[1])
                        if len >= 3 then AppendOverload(overloads.[2])
                        if len >= 4 then AppendOverload(overloads.[3])
                        if len >= 5 then AppendOverload(overloads.[4])
                        if len >= 6 then 
                            AppendHardLine textCollector
                            textCollector.Add (tagText(PrettyNaming.FormatAndOtherOverloadsString(len-5)))

                    let item0 = overloads.[0]

                    item0.Remarks |> Option.iter (fun r -> 
                        if not(isEmptyL r) then
                            AppendHardLine usageCollector
                            renderL (taggedTextListR usageCollector.Add) r |> ignore)

                    AppendXmlComment(documentationProvider, xmlCollector, exnCollector, item0.XmlDoc, showExceptions, showParameters, item0.ParamName)

                    if showText then 
                        ProcessGenericParameters item0.TypeMapping

                    true
                else
                    false

            | FSharpStructuredToolTipElement.CompositionError(errText) -> 
                textCollector.Add(tagText errText)
                true

        List.fold Process false dataTipText |> ignore

    let BuildDataTipText(documentationProvider, textCollector, xmlCollector, typeParameterMapCollector, usageCollector, exnCollector, FSharpToolTipText(dataTipText)) = 
        BuildTipText(documentationProvider, dataTipText, textCollector, xmlCollector, typeParameterMapCollector, usageCollector, exnCollector, true, true, false) 

    let BuildMethodOverloadTipText(documentationProvider, textCollector, xmlCollector, FSharpToolTipText(dataTipText), showParams) = 
        BuildTipText(documentationProvider, dataTipText, textCollector, xmlCollector, xmlCollector, ignore, ignore, false, false, showParams) 

    let BuildMethodParamText(documentationProvider, xmlCollector, xml, paramName) =
        AppendXmlComment(documentationProvider, TextSanitizingCollector(xmlCollector), TextSanitizingCollector(xmlCollector), xml, false, true, Some paramName)

    let documentationBuilderCache = System.Runtime.CompilerServices.ConditionalWeakTable<IVsXMLMemberIndexService, IDocumentationBuilder>()
    let CreateDocumentationBuilder(xmlIndexService: IVsXMLMemberIndexService, dte: DTE) = 
        documentationBuilderCache.GetValue(xmlIndexService,(fun _ -> Provider(xmlIndexService, dte) :> IDocumentationBuilder))