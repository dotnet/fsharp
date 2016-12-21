// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.Text
open System.Collections.Generic
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

type internal TextSanitizingCollector(collector, ?lineLimit: int) =
    let mutable isEmpty = true 
    let mutable endsWithLineBreak = false
    let mutable count = 0

    let buf = StringBuilder()

    let addTaggedTextEntry t =
        if lineLimit.IsNone || count < lineLimit.Value then 
            isEmpty <- false
            endsWithLineBreak <- match t with TaggedText.LineBreak _ -> true | _ -> false
            if endsWithLineBreak then count <- count + 1
            collector t
        if lineLimit.IsSome && lineLimit.Value = count then
            // add ... when line limit is reached
            collector (tagText "...")
            count <- count + 1
    
    let isCROrLF c = c = '\r' || c = '\n'

    let reportTextLines (s: string) =
        let mutable pos = 0
        // skip newlines at the beginning
        while pos < s.Length && isCROrLF s.[pos] do
            pos <- pos + 1
        
        // skip newlines whitespaces at the end
        let mutable endPos = s.Length - 1
        while endPos >= pos && (Char.IsWhiteSpace s.[endPos] || isCROrLF s.[endPos])do
            endPos <- endPos - 1

        if pos < endPos then
            buf.Clear() |> ignore
            while (pos < s.Length) do
                match s.[pos] with
                | '\r' -> ()
                | '\n' -> 
                    if buf.Length > 0 then
                        addTaggedTextEntry (tagText (buf.ToString()))
                        addTaggedTextEntry Literals.lineBreak
                        buf.Clear() |> ignore
                | c -> buf.Append(c) |> ignore
                pos <- pos + 1
            // flush the rest
            if buf.Length > 0 then
                addTaggedTextEntry (tagText (buf.ToString()))

        buf.Clear() |> ignore

    interface ITaggedTextCollector with
        member this.Add text = 
            // TODO: bail out early if line limit is already hit
            match text with
            | TaggedText.Text t -> reportTextLines t
            | t -> addTaggedTextEntry t

        member this.IsEmpty = isEmpty
        member this.EndsWithLineBreak = isEmpty || endsWithLineBreak

/// XmlDocumentation builder, using the VS interfaces to build documentation.  An interface is used
/// to allow unit testing to give an alternative implementation which captures the documentation.
type internal IDocumentationBuilder =

    /// Append the given raw XML formatted into the string builder
    abstract AppendDocumentationFromProcessedXML : collector: ITaggedTextCollector * processedXml:string * showExceptions:bool * showParameters:bool * paramName:string option-> unit

    /// Appends text for the given filename and signature into the StringBuilder
    abstract AppendDocumentation : collector: ITaggedTextCollector * filename: string * signature: string * showExceptions: bool * showParameters: bool * paramName: string option-> unit

/// Documentation helpers.
module internal XmlDocumentation =

    /// If the XML comment starts with '<' not counting whitespace then treat it as a literal XML comment.
    /// Otherwise, escape it and surround it with <summary></summary>
    let ProcessXml(xml:string) =
        if String.IsNullOrEmpty(xml) then xml
        else
            let trimmedXml = xml.TrimStart([|' ';'\r';'\n'|])
            if trimmedXml.Length>0 then
                if trimmedXml.[0] <> '<' then 
                    // This code runs for local/within-project xmldoc tooltips, but not for cross-project or .XML - for that see ast.fs in the compiler
                    let escapedXml = System.Security.SecurityElement.Escape(xml)
                    "<summary>" + escapedXml + "</summary>"
                else 
                    "<root>" + xml + "</root>"
            else xml
    
    /// Provide Xml Documentation             
    type Provider(xmlIndexService:IVsXMLMemberIndexService, dte: DTE) = 
        /// Index of assembly name to xml member index.
        let mutable xmlCache = new AgedLookup<string,IVsXMLMemberIndex>(10,areSame=(fun (x,y) -> x = y))
        
        let events = dte.Events :?> Events2
        let solutionEvents = events.SolutionEvents
        do solutionEvents.add_AfterClosing(fun () -> 
            xmlCache.Clear())
            
        let AppendHardLine(collector: ITaggedTextCollector) =
            collector.Add Literals.lineBreak
       
        let EnsureHardLine(collector: ITaggedTextCollector) =
            if not collector.EndsWithLineBreak then AppendHardLine collector
        
        let AppendOnNewLine (collector: ITaggedTextCollector) (line:string) =
            if line.Length > 0 then 
                EnsureHardLine collector
                collector.Add(TaggedTextOps.tagText line)
                    
        let AppendSummary (collector: ITaggedTextCollector) (memberData:IVsXMLMemberData3) = 
            let ok,summary = memberData.GetSummaryText()
            if Com.Succeeded(ok) || not (String.IsNullOrEmpty summary)then 
                // if failed, still show the summary because it may contain an error message.
                AppendOnNewLine collector summary
            
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
                        //AppendOnNewLine sb (sprintf "%s - %s" name text)
                
        let _AppendRemarks (collector: ITaggedTextCollector) (memberData:IVsXMLMemberData3) = 
            let ok,remarksText = memberData.GetRemarksText()
            if Com.Succeeded(ok) then 
                AppendOnNewLine collector remarksText            
    #endif

        let AppendParameters (collector: ITaggedTextCollector) (memberData:IVsXMLMemberData3) = 
            let ok,count = memberData.GetParamCount()
            if Com.Succeeded(ok) && count > 0 then 
                for param in 0..(count-1) do
                    let ok,name,text = memberData.GetParamTextAt(param)
                    if Com.Succeeded(ok) then
                        EnsureHardLine collector
                        collector.Add(tagParameter name)
                        collector.Add(Literals.colon)
                        collector.Add(Literals.space)
                        collector.Add(tagText text)
                        //AppendOnNewLine sb (sprintf "%s: %s" name text)

        let AppendParameter (collector: ITaggedTextCollector) ( memberData:IVsXMLMemberData3) (paramName:string) =
            let ok,count = memberData.GetParamCount()
            if Com.Succeeded(ok) && count > 0 then 
                if not collector.EndsWithLineBreak then 
                    AppendHardLine(collector)
                for param in 0..(count-1) do
                    let ok,name,text = memberData.GetParamTextAt(param)
                    if Com.Succeeded(ok) && name = paramName then 
                        AppendOnNewLine collector text

        let _AppendReturns (collector: ITaggedTextCollector) (memberData:IVsXMLMemberData3) = 
            let ok,returnsText = memberData.GetReturnsText()
            if Com.Succeeded(ok) then 
                if not collector.EndsWithLineBreak then 
                    AppendHardLine(collector)
                    AppendHardLine(collector)
                AppendOnNewLine collector returnsText
            
        let AppendExceptions (collector: ITaggedTextCollector) (memberData:IVsXMLMemberData3) = 
            let ok,count = memberData.GetExceptionCount()
            if Com.Succeeded(ok) && count > 0 then 
                if count > 0 then 
                    AppendHardLine collector
                    AppendHardLine collector
                    AppendOnNewLine collector Strings.ExceptionsHeader
                    for exc in 0..count do
                        let ok,typ,_text = memberData.GetExceptionTextAt(exc)
                        if Com.Succeeded(ok) then 
                            EnsureHardLine collector
                            collector.Add(tagSpace "    ")
                            collector.Add(tagClass typ)
                            //AppendOnNewLine sb (sprintf "    %s" typ )
                
        /// Retrieve the pre-existing xml index or None
        let GetMemberIndexOfAssembly(assemblyName) =
            match xmlCache.TryGet(assemblyName) with 
            | Some(memberIndex) -> Some(memberIndex)
            | None -> 
                let ok,memberIndex = xmlIndexService.CreateXMLMemberIndex(assemblyName)
                if Com.Succeeded(ok) then 
                    let ok = memberIndex.BuildMemberIndex()
                    if Com.Succeeded(ok) then 
                        xmlCache.Put(assemblyName,memberIndex)
                        Some(memberIndex)
                    else None
                else None

        let AppendMemberData(collector: ITaggedTextCollector,memberData:IVsXMLMemberData3,showExceptions:bool,showParameters:bool) =
            AppendHardLine collector
            AppendSummary collector memberData
//          AppendParameters appendTo memberData
//          AppendTypeParameters appendTo memberData
            if (showParameters) then 
                AppendParameters collector memberData
                // Not showing returns because there's no resource localization in language service to place the "returns:" text
                // AppendReturns appendTo memberData
            if (showExceptions) then AppendExceptions collector memberData
//          AppendRemarks appendTo memberData

        interface IDocumentationBuilder with 
            /// Append the given processed XML formatted into the string builder
            override this.AppendDocumentationFromProcessedXML(appendTo, processedXml, showExceptions, showParameters, paramName) = 
                let ok,xml = xmlIndexService.GetMemberDataFromXML(processedXml)
                if Com.Succeeded(ok) then 
                    if paramName.IsSome then
                        AppendParameter appendTo (xml:?>IVsXMLMemberData3) paramName.Value
                    else
                        AppendMemberData(appendTo,xml:?>IVsXMLMemberData3,showExceptions,showParameters)

            /// Append Xml documentation contents into the StringBuilder
            override this.AppendDocumentation
                            ( /// ITaggedTextCollector to add to
                              sink: ITaggedTextCollector,
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
                            let processedXml = ProcessXml(xml)
                            if Com.Succeeded(ok) then 
                                (this:>IDocumentationBuilder).AppendDocumentationFromProcessedXML(sink, processedXml, showExceptions, showParameters, paramName)
                    | None -> ()
                with e-> 
                    Assert.Exception(e)
                    reraise()    
 
    /// Append an XmlCommnet to the segment.
    let AppendXmlComment(documentationProvider:IDocumentationBuilder, sink: ITaggedTextCollector, xml, showExceptions, showParameters, paramName) =
        match xml with
        | FSharpXmlDoc.None -> ()
        | FSharpXmlDoc.XmlDocFileSignature(filename,signature) -> 
            documentationProvider.AppendDocumentation(sink, filename, signature, showExceptions, showParameters, paramName)
        | FSharpXmlDoc.Text(rawXml) ->
            let processedXml = ProcessXml(rawXml)
            documentationProvider.AppendDocumentationFromProcessedXML(sink, processedXml, showExceptions, showParameters, paramName)

    /// Common sanitation for data tip segment
    let _CleanDataTipSegment(segment:StringBuilder) =     
        segment.Replace("\r", "")
          .Replace("\n\n\n","\n\n")
          .Replace("\n\n\n","\n\n") 
          .ToString()
          .Trim([|'\n'|])

    let private AddSeparator (collector: ITaggedTextCollector) =
        collector.Add Literals.lineBreak
        collector.Add (tagText "-------------")
        collector.Add Literals.lineBreak
        

    /// Build a data tip text string with xml comments injected.
    let BuildTipText(documentationProvider:IDocumentationBuilder, dataTipText:FSharpToolTipElement<Layout> list, textCollector, xmlCollector, showText, showExceptions, showParameters, showOverloadText) = 
        let textCollector: ITaggedTextCollector = TextSanitizingCollector(textCollector, lineLimit = 45) :> _
        let xmlCollector: ITaggedTextCollector = TextSanitizingCollector(xmlCollector, lineLimit = 45) :> _
        let Process(dataTipElement:FSharpToolTipElement<_>) =
            match dataTipElement with 
            | FSharpToolTipElement.None -> false
            | FSharpToolTipElement.Single (text, xml) -> 
                if showText then 
                    renderL (taggedTextListR textCollector.Add) text |> ignore
                AppendXmlComment(documentationProvider, xmlCollector, xml, showExceptions, showParameters, None)
                true
            | FSharpToolTipElement.SingleParameter(text, xml, paramName) ->
                if showText then
                    renderL (taggedTextListR textCollector.Add) text |> ignore
                AppendXmlComment(documentationProvider, xmlCollector, xml, showExceptions, showParameters, Some paramName)
                true
            | FSharpToolTipElement.Group (overloads) -> 
                let overloads = Array.ofList overloads
                let len = Array.length overloads
                if len >= 1 then 
                    if showOverloadText then 
                        let AppendOverload(text,_) = 
                            if not(Microsoft.FSharp.Compiler.Layout.isEmptyL text) then
                                if not textCollector.IsEmpty then textCollector.Add Literals.lineBreak
                                renderL (taggedTextListR textCollector.Add) text |> ignore

                        AppendOverload(overloads.[0])
                        if len >= 2 then AppendOverload(overloads.[1])
                        if len >= 3 then AppendOverload(overloads.[2])
                        if len >= 4 then AppendOverload(overloads.[3])
                        if len >= 5 then AppendOverload(overloads.[4])
                        if len >= 6 then 
                            textCollector.Add Literals.lineBreak
                            textCollector.Add (tagText(PrettyNaming.FormatAndOtherOverloadsString(len-5)))
                            //segment.Append("\n").Append(PrettyNaming.FormatAndOtherOverloadsString(len-5)) |> ignore

                    let _,xml = overloads.[0]
                    AppendXmlComment(documentationProvider, textCollector, xml, showExceptions, showParameters, None)
                    true
                else
                    false

            | FSharpToolTipElement.CompositionError(errText) -> 
                textCollector.Add(tagText errText)
                true

//            CleanDataTipSegment(segment) 

        let mutable addSeparator = false
        for dataTip in dataTipText do
            if addSeparator then
                AddSeparator textCollector
                AddSeparator xmlCollector           
            addSeparator <- Process dataTip

        //let segments = dataTipText |> List.map Format |> List.filter (fun d->d<>null) |> Array.ofList
        //let text =  System.String.Join("\n-------------\n", segments)

        //let lines = text.Split([|'\n'|],maxLinesInText+1) // Need one more than max to determine whether there is truncation.
        //let truncate = lines.Length>maxLinesInText            
        //let lines = lines |> Seq.truncate maxLinesInText 
        //let lines = if truncate then Seq.append lines ["..."] else lines
        //let lines = lines |> Seq.toArray
        //let join = String.Join("\n",lines)

        //join

    let BuildDataTipText(documentationProvider, textCollector, xmlCollector, FSharpToolTipText(dataTipText)) = 
        BuildTipText(documentationProvider, dataTipText, textCollector, xmlCollector, true, true, false, true) 

    let BuildMethodOverloadTipText(documentationProvider, textCollector, xmlCollector, FSharpToolTipText(dataTipText), showParams) = 
        BuildTipText(documentationProvider, dataTipText, textCollector, xmlCollector, false, false, showParams, false) 

    let BuildMethodParamText(documentationProvider, xmlCollector, xml, paramName) =
        AppendXmlComment(documentationProvider, TextSanitizingCollector(xmlCollector), xml, false, true, Some paramName)

    let documentationBuilderCache = System.Runtime.CompilerServices.ConditionalWeakTable<IVsXMLMemberIndexService, IDocumentationBuilder>()
    let CreateDocumentationBuilder(xmlIndexService: IVsXMLMemberIndexService, dte: DTE) = 
        documentationBuilderCache.GetValue(xmlIndexService,(fun _ -> Provider(xmlIndexService, dte) :> IDocumentationBuilder))