﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Runtime.CompilerServices
open System.Text.RegularExpressions
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Text.TaggedText
open System.Collections.Generic

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

    let addTaggedTextEntry (text: TaggedText) =
        match lineLimit with
        | Some lineLimit when lineLimit = count ->
            // add ... when line limit is reached
            collector (tagText "...")
            count <- count + 1
        | _ ->
            isEmpty <- false
            endsWithLineBreak <- text.Tag = TextTag.LineBreak

            if endsWithLineBreak then
                count <- count + 1

            collector text

    static let splitTextRegex =
        Regex(@"\s*\n\s*\n\s*", RegexOptions.Compiled ||| RegexOptions.ExplicitCapture)

    static let normalizeSpacesRegex =
        Regex(@"\s+", RegexOptions.Compiled ||| RegexOptions.ExplicitCapture)

    let reportTextLines (s: string) =
        // treat _double_ newlines as line breaks and remove all \n after that
        let paragraphs =
            splitTextRegex.Split(s.Replace("\r", ""))
            |> Array.filter (not << String.IsNullOrWhiteSpace)

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
                else
                    paragraph

            addTaggedTextEntry (tagText paragraph)

            if i < paragraphs.Length - 1 then
                // insert two line breaks to separate paragraphs
                addTaggedTextEntry TaggedText.lineBreak
                addTaggedTextEntry TaggedText.lineBreak)

    interface ITaggedTextCollector with
        member _.Add taggedText =
            match lineLimit with
            | Some lineLimit when lineLimit < count -> ()
            | _ ->
                match taggedText.Tag with
                | TextTag.Text -> reportTextLines taggedText.Text
                | _ -> addTaggedTextEntry taggedText

        member _.IsEmpty = isEmpty
        member _.EndsWithLineBreak = isEmpty || endsWithLineBreak
        member _.StartXMLDoc() = startXmlDoc <- true

/// XmlDocumentation builder, using the VS interfaces to build documentation.  An interface is used
/// to allow unit testing to give an alternative implementation which captures the documentation.
type internal IDocumentationBuilder =

    /// Append the given raw XML formatted into the string builder
    abstract AppendDocumentationFromProcessedXML:
        xmlCollector: ITaggedTextCollector *
        exnCollector: ITaggedTextCollector *
        processedXml: string *
        showExceptions: bool *
        showParameters: bool *
        showRemarks: bool *
        paramName: string option ->
            unit

    /// Appends text for the given file name and signature into the StringBuilder
    abstract AppendDocumentation:
        xmlCollector: ITaggedTextCollector *
        exnCollector: ITaggedTextCollector *
        fileName: string *
        signature: string *
        showExceptions: bool *
        showParameters: bool *
        showRemarks: bool *
        paramName: string option ->
            unit

/// Documentation helpers.
module internal XmlDocumentation =
    open System.Security

    /// If the XML comment starts with '<' not counting whitespace then treat it as a literal XML comment.
    /// Otherwise, escape it and surround it with <summary></summary>
    let ProcessXml (xml: string) =
        if String.IsNullOrEmpty(xml) then
            xml
        else
            let trimmedXml = xml.TrimStart([| ' '; '\r'; '\n' |])

            if trimmedXml.Length > 0 then
                if trimmedXml.[0] <> '<' then
                    // This code runs for local/within-project xmldoc tooltips, but not for cross-project or .XML - for that see ast.fs in the compiler
                    let escapedXml = SecurityElement.Escape(xml)
                    "<summary>" + escapedXml + "</summary>"
                else
                    "<root>" + xml + "</root>"
            else
                xml

    let AppendHardLine (collector: ITaggedTextCollector) = collector.Add TaggedText.lineBreak

    let EnsureHardLine (collector: ITaggedTextCollector) =
        if not collector.EndsWithLineBreak then
            AppendHardLine collector

    let AppendOnNewLine (collector: ITaggedTextCollector) (line: string) =
        if line.Length > 0 then
            EnsureHardLine collector
            collector.Add(TaggedText.tagText line)

    open System.Xml
    open System.Xml.Linq

    let rec private WriteElement (collector: ITaggedTextCollector) (n: XNode) =
        match n.NodeType with
        | XmlNodeType.Text -> WriteText collector (n :?> XText)
        | XmlNodeType.Element ->
            let el = n :?> XElement

            match el.Name.LocalName with
            | "see"
            | "seealso" ->
                for attr in el.Attributes() do
                    WriteAttribute collector attr "cref" (WriteTypeName collector)
            | "paramref"
            | "typeref" ->
                for attr in el.Attributes() do
                    WriteAttribute collector attr "name" (tagParameter >> collector.Add)
            | "br" -> AppendHardLine collector
            | _ -> WriteNodes collector (el.Nodes())
        | _ -> ()

    and WriteNodes (collector: ITaggedTextCollector) (nodes: seq<XNode>) =
        for n in nodes do
            WriteElement collector n

    and WriteText (collector: ITaggedTextCollector) (n: XText) = collector.Add(tagText n.Value)

    and WriteAttribute (collector: ITaggedTextCollector) (attr: XAttribute) (taggedName: string) tagger =
        if attr.Name.LocalName = taggedName then
            tagger attr.Value
        else
            collector.Add(tagText attr.Value)

    and WriteTypeName (collector: ITaggedTextCollector) (typeName: string) =
        let typeName =
            if typeName.StartsWith("T:") then
                typeName.Substring(2)
            else
                typeName

        let parts = typeName.Split([| '.' |])

        for i = 0 to parts.Length - 2 do
            collector.Add(tagNamespace parts.[i])
            collector.Add(TaggedText.dot)

        collector.Add(tagClass parts.[parts.Length - 1])

    type XmlDocReader private (doc: XElement) =

        let tryFindParameter name =
            doc.Descendants(XName.op_Implicit "param")
            |> Seq.tryFind (fun el ->
                match el.Attribute(XName.op_Implicit "name") with
                | null -> false
                | attr -> attr.Value = name)

        static member TryCreate(xml: string) =
            try
                Some(XmlDocReader(XElement.Parse(ProcessXml xml)))
            with _ ->
                None

        member _.CollectSummary(collector: ITaggedTextCollector, showRemarks) =
            match Seq.tryHead (doc.Descendants(XName.op_Implicit "summary")) with
            | None -> ()
            | Some el ->
                EnsureHardLine collector
                WriteElement collector el

            match Seq.tryHead (doc.Descendants(XName.op_Implicit "returns")) with
            | None -> ()
            | Some el ->
                AppendHardLine collector
                AppendHardLine collector
                AppendOnNewLine collector (SR.ReturnsHeader())
                AppendHardLine collector
                collector.Add(tagSpace "    ")
                WriteElement collector el

            if showRemarks then
                match Seq.tryHead (doc.Descendants(XName.op_Implicit "remarks")) with
                | None -> ()
                | Some el ->
                    AppendHardLine collector
                    AppendHardLine collector
                    AppendOnNewLine collector (SR.RemarksHeader())
                    AppendHardLine collector
                    collector.Add(tagSpace "    ")
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
                    collector.Add(TaggedText.colon)
                    collector.Add(TaggedText.space)
                    WriteNodes collector (p.Nodes())

        member this.CollectExceptions(collector: ITaggedTextCollector) =
            let mutable started = false

            for p in doc.Descendants(XName.op_Implicit "exception") do
                match p.Attribute(XName.op_Implicit "cref") with
                | null -> ()
                | exnType ->
                    if not started then
                        started <- true
                        AppendHardLine collector
                        AppendOnNewLine collector (SR.ExceptionsHeader())

                    EnsureHardLine collector
                    collector.Add(tagSpace "    ")
                    WriteTypeName collector exnType.Value

                    if not (Seq.isEmpty (p.Nodes())) then
                        collector.Add TaggedText.space
                        collector.Add TaggedText.minus
                        collector.Add TaggedText.space
                        WriteNodes collector (p.Nodes())

    type VsThreadToken() =
        class
        end

    let vsToken = VsThreadToken()

    /// Provide Xml Documentation
    type Provider(xmlIndexService: IVsXMLMemberIndexService) =
        /// Index of assembly name to xml member index.
        let cache = Dictionary<string, IVsXMLMemberIndex>()

        do Events.SolutionEvents.OnAfterCloseSolution.Add(fun _ -> cache.Clear())

        /// Retrieve the preexisting xml index or None
        let GetMemberIndexOfAssembly (assemblyName) =
            match cache.TryGetValue(assemblyName) with
            | true, memberIndex -> Some(memberIndex)
            | false, _ ->
                let ok, memberIndex = xmlIndexService.CreateXMLMemberIndex(assemblyName)

                if Com.Succeeded(ok) then
                    let ok = memberIndex.BuildMemberIndex()

                    if Com.Succeeded(ok) then
                        cache.Add(assemblyName, memberIndex)
                        Some(memberIndex)
                    else
                        None
                else
                    None

        let AppendMemberData
            (
                xmlCollector: ITaggedTextCollector,
                exnCollector: ITaggedTextCollector,
                xmlDocReader: XmlDocReader,
                showExceptions,
                showParameters,
                showRemarks
            ) =
            AppendHardLine xmlCollector
            xmlCollector.StartXMLDoc()
            xmlDocReader.CollectSummary(xmlCollector, showRemarks)

            if (showParameters) then
                xmlDocReader.CollectParameters xmlCollector

            if (showExceptions) then
                xmlDocReader.CollectExceptions exnCollector

        interface IDocumentationBuilder with
            /// Append the given processed XML formatted into the string builder
            override _.AppendDocumentationFromProcessedXML
                (
                    xmlCollector,
                    exnCollector,
                    processedXml,
                    showExceptions,
                    showParameters,
                    showRemarks,
                    paramName
                ) =
                match XmlDocReader.TryCreate processedXml with
                | Some xmlDocReader ->
                    match paramName with
                    | Some paramName -> xmlDocReader.CollectParameter(xmlCollector, paramName)
                    | None -> AppendMemberData(xmlCollector, exnCollector, xmlDocReader, showExceptions, showParameters, showRemarks)
                | None -> ()

            /// Append Xml documentation contents into the StringBuilder
            override this.AppendDocumentation
                ( // ITaggedTextCollector to add to
                    xmlCollector: ITaggedTextCollector,
                    exnCollector: ITaggedTextCollector,
                    fileName: string,
                    signature: string,
                    showExceptions: bool,
                    showParameters: bool,
                    showRemarks,
                    paramName: string option
                ) =
                try
                    match GetMemberIndexOfAssembly(fileName) with
                    | Some(index) ->
                        let ok, idx = index.ParseMemberSignature(signature)

                        if Com.Succeeded(ok) then
                            let ok, xml = index.GetMemberXML(idx)

                            if Com.Succeeded(ok) then
                                (this :> IDocumentationBuilder)
                                    .AppendDocumentationFromProcessedXML(
                                        xmlCollector,
                                        exnCollector,
                                        xml,
                                        showExceptions,
                                        showParameters,
                                        showRemarks,
                                        paramName
                                    )
                    | None -> ()
                with e ->
                    Assert.Exception(e)
                    reraise ()

    /// Append an XmlComment to the segment.
    let AppendXmlComment
        (
            documentationProvider: IDocumentationBuilder,
            xmlCollector: ITaggedTextCollector,
            exnCollector: ITaggedTextCollector,
            xml,
            showExceptions,
            showParameters,
            showRemarks,
            paramName
        ) =
        match xml with
        | FSharpXmlDoc.None -> ()
        | FSharpXmlDoc.FromXmlFile(fileName, signature) ->
            documentationProvider.AppendDocumentation(
                xmlCollector,
                exnCollector,
                fileName,
                signature,
                showExceptions,
                showParameters,
                showRemarks,
                paramName
            )
        | FSharpXmlDoc.FromXmlText(xmlDoc) ->
            let elaboratedXml = xmlDoc.GetElaboratedXmlLines()
            let processedXml = ProcessXml("\n\n" + String.concat "\n" elaboratedXml)

            documentationProvider.AppendDocumentationFromProcessedXML(
                xmlCollector,
                exnCollector,
                processedXml,
                showExceptions,
                showParameters,
                showRemarks,
                paramName
            )

    [<Literal>]
    let separatorText = "-------------"

    let private AddSeparator (collector: ITaggedTextCollector) =
        if not collector.IsEmpty then
            EnsureHardLine collector
            collector.Add(tagText separatorText)
            AppendHardLine collector

    type LineLimits =
        {
            LineLimit: int
            TypeParameterLimit: int
            OverLoadsLimit: int
        }

    let DefaultLineLimits =
        {
            LineLimit = 45
            TypeParameterLimit = 6
            OverLoadsLimit = 5
        }

    let BuildSingleTipText
        (
            documentationProvider: IDocumentationBuilder,
            dataTipElement: ToolTipElement,
            limits: LineLimits,
            showRemarks: bool
        ) =

        let {
                LineLimit = lineLimit
                TypeParameterLimit = typeParameterLineLimit
                OverLoadsLimit = overLoadsLimit
            } =
            limits

        let mainDescription, documentation, typeParameterMap, exceptions, usage =
            ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray(), ResizeArray()

        let textCollector: ITaggedTextCollector =
            TextSanitizingCollector(mainDescription.Add, lineLimit = lineLimit)

        let xmlCollector: ITaggedTextCollector =
            TextSanitizingCollector(documentation.Add, lineLimit = lineLimit)

        let typeParameterMapCollector: ITaggedTextCollector =
            TextSanitizingCollector(typeParameterMap.Add, lineLimit = typeParameterLineLimit)

        let exnCollector: ITaggedTextCollector =
            TextSanitizingCollector(exceptions.Add, lineLimit = lineLimit)

        let usageCollector: ITaggedTextCollector =
            TextSanitizingCollector(usage.Add, lineLimit = lineLimit)

        let ProcessGenericParameters (tps: TaggedText[] list) =
            if not tps.IsEmpty then
                AppendHardLine typeParameterMapCollector
                AppendOnNewLine typeParameterMapCollector (SR.GenericParametersHeader())

                for tp in tps do
                    AppendHardLine typeParameterMapCollector
                    typeParameterMapCollector.Add(tagSpace "    ")
                    tp |> Array.iter typeParameterMapCollector.Add

        let collectDocumentation () =
            [ documentation; typeParameterMap; exceptions; usage ]
            |> List.filter (Seq.isEmpty >> not)
            |> List.map List.ofSeq
            |> List.intersperse [ lineBreak ]
            |> Seq.concat
            |> List.ofSeq

        match dataTipElement with
        | ToolTipElement.Group overloads when not overloads.IsEmpty ->
            overloads[.. overLoadsLimit - 1]
            |> List.map (fun item -> item.MainDescription)
            |> List.intersperse [| lineBreak |]
            |> Seq.concat
            |> Seq.iter textCollector.Add

            if not overloads[overLoadsLimit..].IsEmpty then
                AppendOnNewLine textCollector $"({(PrettyNaming.FormatAndOtherOverloadsString overloads[overLoadsLimit..].Length)})"

            let item0 = overloads.Head

            item0.Remarks
            |> Option.iter (fun r ->
                if TaggedText.toString r <> "" then
                    AppendHardLine usageCollector
                    r |> Seq.iter usageCollector.Add)

            AppendXmlComment(documentationProvider, xmlCollector, exnCollector, item0.XmlDoc, true, false, showRemarks, item0.ParamName)

            ProcessGenericParameters item0.TypeMapping

            item0.Symbol, mainDescription |> List.ofSeq, collectDocumentation ()

        | ToolTipElement.CompositionError(errText) ->
            textCollector.Add(tagText errText)
            None, mainDescription |> List.ofSeq, collectDocumentation ()

        | _ -> None, [], []

    /// Build a data tip text string with xml comments injected.
    let BuildTipText
        (
            documentationProvider: IDocumentationBuilder,
            dataTipText: ToolTipElement list,
            textCollector,
            xmlCollector,
            typeParameterMapCollector,
            usageCollector,
            exnCollector,
            showText,
            showExceptions,
            showParameters,
            showRemarks
        ) =
        let textCollector: ITaggedTextCollector =
            TextSanitizingCollector(textCollector, lineLimit = 45) :> _

        let xmlCollector: ITaggedTextCollector =
            TextSanitizingCollector(xmlCollector, lineLimit = 45) :> _

        let typeParameterMapCollector: ITaggedTextCollector =
            TextSanitizingCollector(typeParameterMapCollector, lineLimit = 6) :> _

        let exnCollector: ITaggedTextCollector =
            TextSanitizingCollector(exnCollector, lineLimit = 45) :> _

        let usageCollector: ITaggedTextCollector =
            TextSanitizingCollector(usageCollector, lineLimit = 45) :> _

        let addSeparatorIfNecessary add =
            if add then
                AddSeparator textCollector
                AddSeparator xmlCollector

        let ProcessGenericParameters (tps: TaggedText[] list) =
            if not tps.IsEmpty then
                AppendHardLine typeParameterMapCollector
                AppendOnNewLine typeParameterMapCollector (SR.GenericParametersHeader())

                for tp in tps do
                    AppendHardLine typeParameterMapCollector
                    typeParameterMapCollector.Add(tagSpace "    ")
                    tp |> Array.iter typeParameterMapCollector.Add

        let Process add (dataTipElement: ToolTipElement) =

            match dataTipElement with
            | ToolTipElement.None -> false

            | ToolTipElement.Group(overloads) ->
                let overloads = Array.ofList overloads
                let len = overloads.Length

                if len >= 1 then
                    addSeparatorIfNecessary add

                    if showText then
                        let AppendOverload (item: ToolTipElementData) =
                            if TaggedText.toString item.MainDescription <> "" then
                                if not textCollector.IsEmpty then
                                    AppendHardLine textCollector

                                item.MainDescription |> Seq.iter textCollector.Add

                        AppendOverload(overloads.[0])

                        if len >= 2 then
                            AppendOverload(overloads.[1])

                        if len >= 3 then
                            AppendOverload(overloads.[2])

                        if len >= 4 then
                            AppendOverload(overloads.[3])

                        if len >= 5 then
                            AppendOverload(overloads.[4])

                        if len >= 6 then
                            AppendHardLine textCollector
                            textCollector.Add(tagText (PrettyNaming.FormatAndOtherOverloadsString(len - 5)))

                    let item0 = overloads.[0]

                    item0.Remarks
                    |> Option.iter (fun r ->
                        if TaggedText.toString r <> "" then
                            AppendHardLine usageCollector
                            r |> Seq.iter usageCollector.Add)

                    AppendXmlComment(
                        documentationProvider,
                        xmlCollector,
                        exnCollector,
                        item0.XmlDoc,
                        showExceptions,
                        showParameters,
                        showRemarks,
                        item0.ParamName
                    )

                    if showText then
                        ProcessGenericParameters item0.TypeMapping

                    true
                else
                    false

            | ToolTipElement.CompositionError(errText) ->
                textCollector.Add(tagText errText)
                true

        List.fold Process false dataTipText |> ignore

    let BuildDataTipText
        (
            documentationProvider,
            textCollector,
            xmlCollector,
            typeParameterMapCollector,
            usageCollector,
            exnCollector,
            ToolTipText(dataTipText),
            showRemarks
        ) =
        BuildTipText(
            documentationProvider,
            dataTipText,
            textCollector,
            xmlCollector,
            typeParameterMapCollector,
            usageCollector,
            exnCollector,
            true,
            true,
            false,
            showRemarks
        )

    let BuildMethodOverloadTipText (documentationProvider, textCollector, xmlCollector, ToolTipText(dataTipText), showParams, showRemarks) =
        BuildTipText(
            documentationProvider,
            dataTipText,
            textCollector,
            xmlCollector,
            xmlCollector,
            ignore,
            ignore,
            false,
            false,
            showParams,
            showRemarks
        )

    let BuildMethodParamText (documentationProvider, xmlCollector, xml, paramName, showRemarks) =
        AppendXmlComment(
            documentationProvider,
            TextSanitizingCollector(xmlCollector),
            TextSanitizingCollector(xmlCollector),
            xml,
            false,
            true,
            showRemarks,
            Some paramName
        )

    let documentationBuilderCache =
        ConditionalWeakTable<IVsXMLMemberIndexService, IDocumentationBuilder>()

    let CreateDocumentationBuilder (xmlIndexService: IVsXMLMemberIndexService) =
        documentationBuilderCache.GetValue(xmlIndexService, (fun _ -> Provider(xmlIndexService) :> IDocumentationBuilder))
