// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module public FSharp.Compiler.XmlDoc

open System
open System.Xml.Linq
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Lib
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Range

/// Represents collected XmlDoc lines
type XmlDoc =
    | XmlDoc of (string * range)[]
    
    static member Empty = XmlDocStatics.Empty
    
    member x.IsEmpty =
        let (XmlDoc lines) = x
        lines |> Array.forall (fst >> String.IsNullOrWhiteSpace)

    member x.NonEmpty = not x.IsEmpty
    
    static member Merge (XmlDoc lines) (XmlDoc lines') = 
        XmlDoc (Array.append lines lines')
    
    member x.Range = 
        let (XmlDoc lines) = x
        match lines with 
        | [| |] -> Range.range0
        | _ -> Array.reduce Range.unionRanges (Array.map snd lines)

    /// This code runs for .XML generation and thus influences cross-project xmldoc tooltips; for within-project tooltips,
    /// see XmlDocumentation.fs in the language service
    static member Process (XmlDoc lines) =
        let rec processLines (lines: (string * range) list) =
            match lines with
            | [] -> []
            | ((lineA, m) :: rest) as lines ->
                let lineAT = lineA.TrimStart([|' '|])
                if lineAT = "" then processLines rest
                else if lineAT.StartsWithOrdinal("<") then lines
                else [("<summary>", m)] @
                     (lines |> List.map (map1Of2 Microsoft.FSharp.Core.XmlAdapters.escape)) @
                     [("</summary>", m)]

        let lines = processLines (Array.toList lines)
        if isNil lines then XmlDoc.Empty
        else XmlDoc (Array.ofList lines)

    member x.GetXmlText() =
        match XmlDoc.Process x with
        | XmlDoc [| |] -> ""
        | XmlDoc strs ->
            strs 
            |> Array.toList
            |> List.map fst
            |> String.concat Environment.NewLine

// Discriminated unions can't contain statics, so we use a separate type
and XmlDocStatics() =

    static let empty = XmlDoc[| |]

    static member Empty = empty

/// Used to collect XML documentation during lexing and parsing.
type XmlDocCollector() =
    let mutable savedLines = new ResizeArray<(string * range)>()
    let mutable savedGrabPoints = new ResizeArray<pos>()
    let posCompare p1 p2 = if posGeq p1 p2 then 1 else if posEq p1 p2 then 0 else -1
    let savedGrabPointsAsArray =
        lazy (savedGrabPoints.ToArray() |> Array.sortWith posCompare)

    let savedLinesAsArray =
        lazy (savedLines.ToArray() |> Array.sortWith (fun (_, p1) (_, p2) -> posCompare p1.End p2.End))

    let check() =
        // can't add more XmlDoc elements to XmlDocCollector after extracting first XmlDoc from the overall results
        assert (not savedLinesAsArray.IsValueCreated)

    member x.AddGrabPoint pos =
        check()
        savedGrabPoints.Add pos

    member x.AddXmlDocLine(line, range) =
        check()
        savedLines.Add(line, range)

    member x.LinesBefore grabPointPos =
      try
        let lines = savedLinesAsArray.Force()
        let grabPoints = savedGrabPointsAsArray.Force()
        let firstLineIndexAfterGrabPoint = Array.findFirstIndexWhereTrue lines (fun (_, m) -> posGeq m.End grabPointPos)
        let grabPointIndex = Array.findFirstIndexWhereTrue grabPoints (fun pos -> posGeq pos grabPointPos)
        assert (posEq grabPoints.[grabPointIndex] grabPointPos)
        let firstLineIndexAfterPrevGrabPoint =
            if grabPointIndex = 0 then
                0
            else
                let prevGrabPointPos = grabPoints.[grabPointIndex-1]
                Array.findFirstIndexWhereTrue lines (fun (_, m) -> posGeq m.End prevGrabPointPos)

        let lines = lines.[firstLineIndexAfterPrevGrabPoint..firstLineIndexAfterGrabPoint-1]
        lines
      with e ->
        [| |]

/// Represents the XmlDoc fragments as collected from the lexer during parsing
type PreXmlDoc =
    | PreXmlMerge of PreXmlDoc * PreXmlDoc
    | PreXmlDoc of pos * XmlDocCollector
    | PreXmlDocEmpty

    member x.ToXmlDoc() =
        let doc =
            match x with
            | PreXmlMerge(a, b) -> XmlDoc.Merge (a.ToXmlDoc()) (b.ToXmlDoc())
            | PreXmlDocEmpty -> XmlDoc.Empty
            | PreXmlDoc (pos, collector) ->
                let lines = collector.LinesBefore pos
                if lines.Length = 0 then XmlDoc.Empty
                else XmlDoc lines
        if doc.NonEmpty then
            try
                // We must wrap with <doc> in order to have only one root element
                let xml = XDocument.Parse("<doc>\n"+doc.GetXmlText()+"\n</doc>", LoadOptions.SetLineInfo)
                
                // Note, the parameter names are curently only checked for internal
                // consistency, so parameter references must match an XML doc parameter name.
                let paramNames =
                    [ for p in xml.Elements(XName.op_Implicit "param") do
                        match p.Attribute(XName.op_Implicit "name") with 
                        | null -> 
                            warning (Error (FSComp.SR.xmlDocMissingParameterName(), doc.Range))
                        | nm -> 
                            nm.Value ]

                for pref in xml.Descendants(XName.op_Implicit "paramref") do
                    match pref.Attribute(XName.op_Implicit "name") with 
                    | null -> warning (Error (FSComp.SR.xmlDocMissingParameterName(), doc.Range))
                    | attr -> 
                       let nm = attr.Value
                       if not (paramNames |> List.contains nm) then
                          warning (Error (FSComp.SR.xmlDocInvalidParameterName(nm), doc.Range))
                xml |> ignore

            with e -> 
                warning (Error (FSComp.SR.xmlDocBadlyFormed(e.Message), doc.Range))
        doc


    static member CreateFromGrabPoint(collector: XmlDocCollector, grabPointPos) =
        collector.AddGrabPoint grabPointPos
        PreXmlDoc(grabPointPos, collector)

    static member Empty = PreXmlDocEmpty

    static member Merge a b = PreXmlMerge (a, b)

