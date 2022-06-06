// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
//
// THIS CODE IS DEPRECATED AND IS ONLY USED FOR UNIT TESTING
//

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.Collections.Immutable
open System.Text.RegularExpressions
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Text.TaggedText

[<AutoOpen>]
module internal Utils2 =
    let taggedTextToString (tts: TaggedText[]) =
        tts |> Array.map (fun tt -> tt.Text) |> String.concat ""

type internal ITaggedTextCollector_DEPRECATED =
    abstract Add: text: TaggedText -> unit
    abstract EndsWithLineBreak: bool
    abstract IsEmpty: bool
    abstract StartXMLDoc: unit -> unit

type internal TextSanitizingCollector_DEPRECATED(collector, ?lineLimit: int) =
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
            endsWithLineBreak <- text.Tag = TextTag.LineBreak
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
                addTaggedTextEntry TaggedText.lineBreak
                addTaggedTextEntry TaggedText.lineBreak)

    interface ITaggedTextCollector_DEPRECATED with
        member this.Add taggedText = 
            // TODO: bail out early if line limit is already hit
            match taggedText.Tag with
            | TextTag.Text -> reportTextLines taggedText.Text
            | _ -> addTaggedTextEntry taggedText

        member this.IsEmpty = isEmpty
        member this.EndsWithLineBreak = isEmpty || endsWithLineBreak
        member this.StartXMLDoc() = startXmlDoc <- true

/// XmlDocumentation builder, using the VS interfaces to build documentation.  An interface is used
/// to allow unit testing to give an alternative implementation which captures the documentation.
type internal IDocumentationBuilder_DEPRECATED =

    /// Append the given raw XML formatted into the string builder
    abstract AppendDocumentationFromProcessedXML : collector: ITaggedTextCollector_DEPRECATED * processedXml:string * showExceptions:bool * showParameters:bool * paramName:string option-> unit

    /// Appends text for the given file name and signature into the StringBuilder
    abstract AppendDocumentation : collector: ITaggedTextCollector_DEPRECATED * fileName: string * signature: string * showExceptions: bool * showParameters: bool * paramName: string option-> unit

/// Documentation helpers.
module internal XmlDocumentation =

    /// If the XML comment starts with '<' not counting whitespace then treat it as a literal XML comment.
    /// Otherwise, escape it and surround it with <summary></summary>
    let ProcessXml(xml:string) =
        if String.IsNullOrEmpty(xml) then xml
        else
            let trimmedXml = xml.TrimStart([|' ';'\r';'\n'|])
            if trimmedXml.Length > 0 then
                if trimmedXml.[0] <> '<' then 
                    // This code runs for local/within-project xmldoc tooltips, but not for cross-project or .XML - for that see ast.fs in the compiler
                    let escapedXml = System.Security.SecurityElement.Escape(xml)
                    "<summary>" + escapedXml + "</summary>"
                else 
                    "<root>" + xml + "</root>"
            else xml

    let AppendHardLine(collector: ITaggedTextCollector_DEPRECATED) =
        collector.Add TaggedText.lineBreak
       
    let EnsureHardLine(collector: ITaggedTextCollector_DEPRECATED) =
        if not collector.EndsWithLineBreak then AppendHardLine collector
        
    let AppendOnNewLine (collector: ITaggedTextCollector_DEPRECATED) (line:string) =
        if line.Length > 0 then 
            EnsureHardLine collector
            collector.Add(TaggedText.tagText line)

 
    /// Append an XmlCommnet to the segment.
    let AppendXmlComment_DEPRECATED(documentationProvider:IDocumentationBuilder_DEPRECATED, sink: ITaggedTextCollector_DEPRECATED, xml, showExceptions, showParameters, paramName) =
        match xml with
        | FSharpXmlDoc.None -> ()
        | FSharpXmlDoc.FromXmlFile(fileName,signature) -> 
            documentationProvider.AppendDocumentation(sink, fileName, signature, showExceptions, showParameters, paramName)
        | FSharpXmlDoc.FromXmlText(xmlDoc) ->
            let processedXml = ProcessXml("\n\n" + String.concat "\n" xmlDoc.UnprocessedLines)
            documentationProvider.AppendDocumentationFromProcessedXML(sink, processedXml, showExceptions, showParameters, paramName)

    let private AddSeparator (collector: ITaggedTextCollector_DEPRECATED) =
        if not collector.IsEmpty then
            EnsureHardLine collector
            collector.Add (tagText "-------------")
            AppendHardLine collector

    /// Build a data tip text string with xml comments injected.
    let BuildTipText_DEPRECATED(documentationProvider:IDocumentationBuilder_DEPRECATED, dataTipText: ToolTipElement list, textCollector, xmlCollector, showText, showExceptions, showParameters) = 
        let textCollector: ITaggedTextCollector_DEPRECATED = TextSanitizingCollector_DEPRECATED(textCollector, lineLimit = 45) :> _
        let xmlCollector: ITaggedTextCollector_DEPRECATED = TextSanitizingCollector_DEPRECATED(xmlCollector, lineLimit = 45) :> _

        let addSeparatorIfNecessary add =
            if add then
                AddSeparator textCollector
                AddSeparator xmlCollector

        let Process add (dataTipElement: ToolTipElement) =

            match dataTipElement with 
            | ToolTipElement.None -> false

            | ToolTipElement.Group (overloads) -> 
                let overloads = Array.ofList overloads
                let len = overloads.Length
                if len >= 1 then
                    addSeparatorIfNecessary add
                    if showText then 
                        let AppendOverload (item :ToolTipElementData) = 
                            if taggedTextToString item.MainDescription <> "" then
                                if not textCollector.IsEmpty then textCollector.Add TaggedText.lineBreak
                                item.MainDescription |> Seq.iter textCollector.Add

                        AppendOverload(overloads.[0])
                        if len >= 2 then AppendOverload(overloads.[1])
                        if len >= 3 then AppendOverload(overloads.[2])
                        if len >= 4 then AppendOverload(overloads.[3])
                        if len >= 5 then AppendOverload(overloads.[4])
                        if len >= 6 then 
                            textCollector.Add TaggedText.lineBreak
                            textCollector.Add (tagText(PrettyNaming.FormatAndOtherOverloadsString(len-5)))

                    let item0 = overloads.[0]

                    item0.Remarks |> Option.iter (fun r -> 
                        textCollector.Add TaggedText.lineBreak
                        r |> Seq.iter textCollector.Add |> ignore)

                    AppendXmlComment_DEPRECATED(documentationProvider, xmlCollector, item0.XmlDoc, showExceptions, showParameters, item0.ParamName)

                    true
                else
                    false

            | ToolTipElement.CompositionError(errText) -> 
                textCollector.Add(tagText errText)
                true

        List.fold Process false dataTipText |> ignore

    let BuildDataTipText_DEPRECATED(documentationProvider, textCollector, xmlCollector, ToolTipText(dataTipText)) = 
        BuildTipText_DEPRECATED(documentationProvider, dataTipText, textCollector, xmlCollector, true, true, false) 

    let BuildMethodOverloadTipText_DEPRECATED(documentationProvider, textCollector, xmlCollector, ToolTipText(dataTipText), showParams) = 
        BuildTipText_DEPRECATED(documentationProvider, dataTipText, textCollector, xmlCollector, false, false, showParams) 


