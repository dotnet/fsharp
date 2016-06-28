// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.Text
open Internal.Utilities.Collections
open EnvDTE
open EnvDTE80
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.FSharp.Compiler.SourceCodeServices

/// XmlDocumentation builder, using the VS interfaces to build documentation.  An interface is used
/// to allow unit testing to give an alternative implementation which captures the documentation.
type internal IDocumentationBuilder =

    /// Append the given raw XML formatted into the string builder
    abstract AppendDocumentationFromProcessedXML : appendTo:StringBuilder * processedXml:string * showExceptions:bool * showParameters:bool * paramName:string option-> unit

    /// Appends text for the given filename and signature into the StringBuilder
    abstract AppendDocumentation : appendTo: StringBuilder * filename: string * signature: string * showExceptions: bool * showParameters: bool * paramName: string option-> unit

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

        let HasTrailingEndOfLine(sb:StringBuilder) = 
            if sb.Length = 0 then true
            else
                let c = sb.[sb.Length-1]
                c = '\r' || c = '\n'
                
        let AppendHardLine(sb:StringBuilder) =
            sb.AppendLine() |> ignore            
       
        let AppendOnNewLine (sb:StringBuilder) (line:string) =
            if line.Length>0 then 
                if not(HasTrailingEndOfLine(sb)) then 
                    sb.AppendLine("")|>ignore
                sb.Append(line.TrimEnd([|' '|]))|>ignore                      
                    
        let AppendSummary (sb:StringBuilder) (memberData:IVsXMLMemberData3) = 
            let ok,summary = memberData.GetSummaryText()
            if Com.Succeeded(ok) then 
                AppendOnNewLine sb summary
            else 
                // Failed, but still show the summary because it may contain an error message.
                if summary<>null then AppendOnNewLine sb summary
            
    #if DEBUG // Keep under DEBUG so that it can keep building.

        let _AppendTypeParameters (sb:StringBuilder) (memberData:IVsXMLMemberData3) = 
            let ok,count = memberData.GetTypeParamCount()
            if Com.Succeeded(ok) && count>0 then 
                if not(HasTrailingEndOfLine(sb)) then 
                    AppendHardLine(sb)
                for param in 0..count do
                    let ok,name,text = memberData.GetTypeParamTextAt(param)
                    if Com.Succeeded(ok) then 
                        AppendOnNewLine sb (sprintf "%s - %s" name text)
                
        let _AppendRemarks (sb:StringBuilder) (memberData:IVsXMLMemberData3) = 
            let ok,remarksText = memberData.GetRemarksText()
            if Com.Succeeded(ok) then 
                AppendOnNewLine sb remarksText            
    #endif

        let AppendParameters (sb:StringBuilder) (memberData:IVsXMLMemberData3) = 
            let ok,count = memberData.GetParamCount()
            if Com.Succeeded(ok) && count > 0 then 
                if not(HasTrailingEndOfLine(sb)) then 
                    AppendHardLine(sb)
                    AppendHardLine(sb)
                for param in 0..(count-1) do
                    let ok,name,text = memberData.GetParamTextAt(param)
                    if Com.Succeeded(ok) then 
                        AppendOnNewLine sb (sprintf "%s: %s" name text)

        let AppendParameter (sb:StringBuilder, memberData:IVsXMLMemberData3, paramName:string) =
            let ok,count = memberData.GetParamCount()
            if Com.Succeeded(ok) && count > 0 then 
                if not(HasTrailingEndOfLine(sb)) then 
                    AppendHardLine(sb)
                for param in 0..(count-1) do
                    let ok,name,text = memberData.GetParamTextAt(param)
                    if Com.Succeeded(ok) && name = paramName then 
                        AppendOnNewLine sb text

        let _AppendReturns (sb:StringBuilder) (memberData:IVsXMLMemberData3) = 
            let ok,returnsText = memberData.GetReturnsText()
            if Com.Succeeded(ok) then 
                if not(HasTrailingEndOfLine(sb)) then 
                    AppendHardLine(sb)
                    AppendHardLine(sb)
                AppendOnNewLine sb returnsText
            
        let AppendExceptions (sb:StringBuilder) (memberData:IVsXMLMemberData3) = 
            let ok,count = memberData.GetExceptionCount()
            if Com.Succeeded(ok) && count > 0 then 
                if count > 0 then 
                    AppendHardLine sb
                    AppendHardLine sb
                    AppendOnNewLine sb Strings.ExceptionsHeader
                    for exc in 0..count do
                        let ok,typ,_text = memberData.GetExceptionTextAt(exc)
                        if Com.Succeeded(ok) then 
                            AppendOnNewLine sb (sprintf "    %s" typ )
                
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

        let AppendMemberData(appendTo:StringBuilder,memberData:IVsXMLMemberData3,showExceptions:bool,showParameters:bool) =
            AppendHardLine appendTo
            AppendSummary appendTo memberData
//          AppendParameters appendTo memberData
//          AppendTypeParameters appendTo memberData
            if (showParameters) then 
                AppendParameters appendTo memberData
                // Not showing returns because there's no resource localization in language service to place the "returns:" text
                // AppendReturns appendTo memberData
            if (showExceptions) then AppendExceptions appendTo memberData
//          AppendRemarks appendTo memberData

        interface IDocumentationBuilder with 
            /// Append the given processed XML formatted into the string builder
            override this.AppendDocumentationFromProcessedXML
                            ( /// StringBuilder to append to
                              appendTo:StringBuilder,
                              /// The processed XML text.
                              processedXml:string,
                              /// Whether to show exceptions
                              showExceptions:bool,
                              /// Whether to show parameters and return
                              showParameters:bool,
                              /// Name of parameter
                              paramName:string option
                             ) = 
                let ok,xml = xmlIndexService.GetMemberDataFromXML(processedXml)
                if Com.Succeeded(ok) then 
                    if paramName.IsSome then
                        AppendParameter(appendTo, xml:?>IVsXMLMemberData3, paramName.Value)
                    else
                        AppendMemberData(appendTo,xml:?>IVsXMLMemberData3,showExceptions,showParameters)

            /// Append Xml documentation contents into the StringBuilder
            override this.AppendDocumentation
                            ( /// StringBuilder to append to
                              appendTo:StringBuilder,
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
                                (this:>IDocumentationBuilder).AppendDocumentationFromProcessedXML(appendTo,processedXml,showExceptions,showParameters, paramName)
                    | None -> ()
                with e-> 
                    Assert.Exception(e)
                    reraise()    
 
    /// Append an XmlCommnet to the segment.
    let AppendXmlComment(documentationProvider:IDocumentationBuilder, segment:StringBuilder, xml, showExceptions, showParameters, paramName) =
        match xml with
        | FSharpXmlDoc.None -> ()
        | FSharpXmlDoc.XmlDocFileSignature(filename,signature) -> 
            segment.Append("\n") |> ignore
            documentationProvider.AppendDocumentation(segment,filename,signature,showExceptions,showParameters, paramName)
        | FSharpXmlDoc.Text(rawXml) ->
            let processedXml = ProcessXml(rawXml)
            segment.Append("\n") |> ignore
            documentationProvider.AppendDocumentationFromProcessedXML(segment,processedXml,showExceptions,showParameters, paramName)

    /// Common sanitation for data tip segment
    let CleanDataTipSegment(segment:StringBuilder) =     
        segment.Replace("\r", "")
          .Replace("\n\n\n","\n\n")
          .Replace("\n\n\n","\n\n") 
          .ToString()
          .Trim([|'\n'|])

    /// Build a data tip text string with xml comments injected.
    let BuildTipText(documentationProvider:IDocumentationBuilder, dataTipText:FSharpToolTipElement list, showText, showExceptions, showParameters, showOverloadText) = 
        let maxLinesInText = 45
        let Format(dataTipElement:FSharpToolTipElement) =
            let segment = 
                match dataTipElement with 
                | FSharpToolTipElement.None ->StringBuilder()
                | FSharpToolTipElement.Single (text,xml) -> 
                    let segment = StringBuilder()
                    if showText then 
                        segment.Append(text) |> ignore

                    AppendXmlComment(documentationProvider, segment, xml, showExceptions, showParameters, None)
                    segment
                | FSharpToolTipElement.SingleParameter(text, xml, paramName) ->
                    let segment = StringBuilder()
                    if showText then 
                        segment.Append(text) |> ignore

                    AppendXmlComment(documentationProvider, segment, xml, showExceptions, showParameters, Some paramName)
                    segment
                | FSharpToolTipElement.Group (overloads) -> 
                    let segment = StringBuilder()
                    let overloads = Array.ofList overloads
                    let len = Array.length overloads
                    if len >= 1 then 
                        if showOverloadText then 
                            let AppendOverload(text,_) = 
                                if not(String.IsNullOrEmpty(text)) then
                                    segment.Append("\n").Append(text) |> ignore

                            AppendOverload(overloads.[0])
                            if len >= 2 then AppendOverload(overloads.[1])
                            if len >= 3 then AppendOverload(overloads.[2])
                            if len >= 4 then AppendOverload(overloads.[3])
                            if len >= 5 then AppendOverload(overloads.[4])
                            if len >= 6 then segment.Append("\n").Append(PrettyNaming.FormatAndOtherOverloadsString(len-5)) |> ignore

                        let _,xml = overloads.[0]
                        AppendXmlComment(documentationProvider, segment, xml, showExceptions, showParameters, None)
                    segment
                | FSharpToolTipElement.CompositionError(errText) -> StringBuilder(errText)
            CleanDataTipSegment(segment) 

        let segments = dataTipText |> List.map Format |> List.filter (fun d->d<>null) |> Array.ofList
        let text =  System.String.Join("\n-------------\n", segments)

        let lines = text.Split([|'\n'|],maxLinesInText+1) // Need one more than max to determine whether there is truncation.
        let truncate = lines.Length>maxLinesInText            
        let lines = lines |> Seq.truncate maxLinesInText 
        let lines = if truncate then Seq.append lines ["..."] else lines
        let lines = lines |> Seq.toArray
        let join = String.Join("\n",lines)

        join

    let BuildDataTipText(documentationProvider, FSharpToolTipText(dataTipText)) = 
        BuildTipText(documentationProvider,dataTipText,true, true, false, true) 

    let BuildMethodOverloadTipText(documentationProvider, FSharpToolTipText(dataTipText)) = 
        BuildTipText(documentationProvider,dataTipText,false, false, true, false) 

    let CreateDocumentationBuilder(xmlIndexService, dte) = Provider(xmlIndexService, dte) :> IDocumentationBuilder