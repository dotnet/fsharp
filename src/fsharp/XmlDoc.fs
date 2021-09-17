// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Xml

open System
open System.Collections.Generic
open System.IO
open System.Xml
open System.Xml.Linq
open Internal.Utilities.Library
open Internal.Utilities.Collections
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.IO
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.AbstractIL.IL

/// Represents collected XmlDoc lines
[<RequireQualifiedAccess>]
type XmlDoc(unprocessedLines: string[], range: range) =
    let rec processLines (lines: string list) =
        match lines with
        | [] -> []
        | lineA :: rest as lines ->
            let lineAT = lineA.TrimStart([|' '|])
            if lineAT = "" then processLines rest
            elif lineAT.StartsWithOrdinal("<") then lines
            else
                ["<summary>"] @
                (lines |> List.map Internal.Utilities.XmlAdapters.escape) @
                ["</summary>"]

    /// Get the lines before insertion of implicit summary tags and encoding
    member _.UnprocessedLines = unprocessedLines

    /// Get the lines after insertion of implicit summary tags and encoding
    member _.GetElaboratedXmlLines() =
        let processedLines = processLines (Array.toList unprocessedLines)

        let lines = Array.ofList processedLines

        lines

    member _.Range = range

    static member Empty = XmlDocStatics.Empty

    member _.IsEmpty =
        unprocessedLines  |> Array.forall String.IsNullOrWhiteSpace

    member doc.NonEmpty = not doc.IsEmpty

    static member Merge (doc1: XmlDoc) (doc2: XmlDoc) =
        XmlDoc(Array.append doc1.UnprocessedLines doc2.UnprocessedLines,
               unionRanges doc1.Range doc2.Range)

    member doc.GetXmlText() =
        if doc.IsEmpty then ""
        else
            doc.GetElaboratedXmlLines()
            |> String.concat Environment.NewLine

    member doc.Check(paramNamesOpt: string list option) =
        try
            // We must wrap with <doc> in order to have only one root element
            let xml =
                XDocument.Parse("<doc>\n"+doc.GetXmlText()+"\n</doc>",
                    LoadOptions.SetLineInfo ||| LoadOptions.PreserveWhitespace)

            // The parameter names are checked for consistency, so parameter references and
            // parameter documentation must match an actual parameter.  In addition, if any parameters
            // have documentation then all parameters must have documentation
            match paramNamesOpt with
            | None -> ()
            | Some paramNames ->
                for p in xml.Descendants(XName.op_Implicit "param") do
                    match p.Attribute(XName.op_Implicit "name") with
                    | null ->
                        warning (Error (FSComp.SR.xmlDocMissingParameterName(), doc.Range))
                    | attr ->
                        let nm = attr.Value
                        if not (paramNames |> List.contains nm) then
                            warning (Error (FSComp.SR.xmlDocInvalidParameterName(nm), doc.Range))

                let paramsWithDocs =
                    [ for p in xml.Descendants(XName.op_Implicit "param") do
                        match p.Attribute(XName.op_Implicit "name") with
                        | null -> ()
                        | attr -> attr.Value ]

                if paramsWithDocs.Length > 0 then
                    for p in paramNames do
                        if not (paramsWithDocs |> List.contains p) then
                            warning (Error (FSComp.SR.xmlDocMissingParameter(p), doc.Range))

                let duplicates = paramsWithDocs |> List.duplicates

                for d in duplicates do
                    warning (Error (FSComp.SR.xmlDocDuplicateParameter(d), doc.Range))

                for pref in xml.Descendants(XName.op_Implicit "paramref") do
                    match pref.Attribute(XName.op_Implicit "name") with
                    | null -> warning (Error (FSComp.SR.xmlDocMissingParameterName(), doc.Range))
                    | attr ->
                        let nm = attr.Value
                        if not (paramNames |> List.contains nm) then
                            warning (Error (FSComp.SR.xmlDocInvalidParameterName(nm), doc.Range))

        with e ->
            warning (Error (FSComp.SR.xmlDocBadlyFormed(e.Message), doc.Range))

#if CREF_ELABORATION
    member doc.Elaborate (crefResolver) =
                for see in seq { yield! xml.Descendants(XName.op_Implicit "see")
                                 yield! xml.Descendants(XName.op_Implicit "seealso")
                                 yield! xml.Descendants(XName.op_Implicit "exception") } do
                    match see.Attribute(XName.op_Implicit "cref") with
                    | null -> warning (Error (FSComp.SR.xmlDocMissingCrossReference(), doc.Range))
                    | attr ->
                        let cref = attr.Value
                        if cref.StartsWith("T:") || cref.StartsWith("P:") || cref.StartsWith("M:") ||
                           cref.StartsWith("E:") || cref.StartsWith("F:") then
                            ()
                        else
                            match crefResolver cref with
                            | None ->
                                warning (Error (FSComp.SR.xmlDocUnresolvedCrossReference(nm), doc.Range))
                            | Some text ->
                                attr.Value <- text
                                modified <- true
                if modified then
                    let m = doc.Range
                    let newLines =
                        [| for e in xml.Elements() do
                             yield! e.ToString().Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)  |]
                    lines <-  newLines
#endif

// Discriminated unions can't contain statics, so we use a separate type
and XmlDocStatics() =

    static let empty = XmlDoc ([| |], range0)

    static member Empty = empty

/// Used to collect XML documentation during lexing and parsing.
type XmlDocCollector() =
    let mutable savedLines = ResizeArray<string * range>()
    let mutable savedGrabPoints = Dictionary<pos, struct(int * int * bool)>()
    let mutable currentGrabPointCommentsCount = 0
    let mutable delayedGrabPoint = ValueNone
    let savedLinesAsArray = lazy (savedLines.ToArray())

    let check() =
        // can't add more XmlDoc elements to XmlDocCollector after extracting first XmlDoc from the overall results
        assert (not savedLinesAsArray.IsValueCreated)

    member x.AddGrabPoint(pos: pos) =
        check()
        if currentGrabPointCommentsCount = 0 then () else
        let xmlDocBlock = struct(savedLines.Count - currentGrabPointCommentsCount, savedLines.Count - 1, false)
        savedGrabPoints.Add(pos, xmlDocBlock)
        currentGrabPointCommentsCount <- 0
        delayedGrabPoint <- ValueNone

    member x.AddGrabPointDelayed(pos: pos) =
        check()
        if currentGrabPointCommentsCount = 0 then () else
        match delayedGrabPoint with
        | ValueNone -> delayedGrabPoint <- ValueSome(pos)
        | _ -> ()

    member x.AddXmlDocLine(line, range) =
        check()
        match delayedGrabPoint with
        | ValueNone -> ()
        | ValueSome pos -> x.AddGrabPoint(pos) // Commit delayed grab point

        savedLines.Add(line, range)
        currentGrabPointCommentsCount <- currentGrabPointCommentsCount + 1

    member x.LinesBefore grabPointPos =
        let lines = savedLinesAsArray.Force()
        match savedGrabPoints.TryGetValue grabPointPos with
        | true, struct(startIndex, endIndex, _) -> lines.[startIndex .. endIndex]
        | false, _ -> [||]

    member x.SetXmlDocValidity(grabPointPos, isValid) =
        match savedGrabPoints.TryGetValue grabPointPos with
        | true, struct(startIndex, endIndex, _) ->
            savedGrabPoints.[grabPointPos] <- struct(startIndex, endIndex, isValid)
        | _ -> ()

    member x.HasComments grabPointPos =
        savedGrabPoints.TryGetValue grabPointPos |> fst

    member x.CheckInvalidXmlDocPositions() =
        let lines = savedLinesAsArray.Force()
        for startIndex, endIndex, isValid in savedGrabPoints.Values do
            if isValid then () else
            let _, startRange = lines.[startIndex]
            let _, endRange = lines.[endIndex]
            let range = unionRanges startRange endRange
            informationalWarning (Error(FSComp.SR.invalidXmlDocPosition(), range))

/// Represents the XmlDoc fragments as collected from the lexer during parsing
type PreXmlDoc =
    | PreXmlDirect of unprocessedLines: string[] * range: range
    | PreXmlMerge of PreXmlDoc * PreXmlDoc
    | PreXmlDoc of pos * XmlDocCollector
    | PreXmlDocEmpty

    member x.ToXmlDoc(check: bool, paramNamesOpt: string list option) =
        match x with
        | PreXmlDirect (lines, m) -> XmlDoc(lines, m)
        | PreXmlMerge(a, b) -> XmlDoc.Merge (a.ToXmlDoc(check, paramNamesOpt)) (b.ToXmlDoc(check, paramNamesOpt))
        | PreXmlDocEmpty -> XmlDoc.Empty
        | PreXmlDoc (pos, collector) ->
            let preLines = collector.LinesBefore pos
            if preLines.Length = 0 then
                XmlDoc.Empty
            else
                let lines = Array.map fst preLines
                let m = Array.reduce unionRanges (Array.map snd preLines)
                let doc = XmlDoc (lines, m)
                if check then
                   doc.Check(paramNamesOpt)
                doc

    member x.IsEmpty =
        match x with
        | PreXmlDirect (lines, _) -> lines |> Array.forall String.IsNullOrWhiteSpace
        | PreXmlMerge(a, b) -> a.IsEmpty && b.IsEmpty
        | PreXmlDocEmpty -> true
        | PreXmlDoc (pos, collector) -> not (collector.HasComments pos)

    member x.MarkAsInvalid() =
        match x with
        | PreXmlDoc (pos, collector) -> collector.SetXmlDocValidity(pos, false)
        | _ -> ()

    static member CreateFromGrabPoint(collector: XmlDocCollector, grabPointPos) =
        collector.SetXmlDocValidity(grabPointPos, true)
        PreXmlDoc(grabPointPos, collector)

    static member Empty = PreXmlDocEmpty

    static member Create(unprocessedLines, range) = PreXmlDirect(unprocessedLines, range)

    static member Merge a b = PreXmlMerge (a, b)

[<Sealed>]
type XmlDocumentationInfo private (tryGetXmlDocument: unit -> XmlDocument option) =

    // 2 and 4 are arbitrary but should be reasonable enough
    [<Literal>]
    static let cacheStrongSize = 2
    [<Literal>]
    static let cacheMaxSize = 4
    static let cacheAreSimilar =
        fun ((str1: string, dt1: DateTime), (str2: string, dt2: DateTime)) ->
            str1.Equals(str2, StringComparison.OrdinalIgnoreCase) &&
            dt1 = dt2
    static let cache = AgedLookup<unit, string * DateTime, XmlDocument>(keepStrongly=cacheStrongSize, areSimilar=cacheAreSimilar, keepMax=cacheMaxSize)

    let tryGetSummaryNode xmlDocSig =
        tryGetXmlDocument()
        |> Option.bind (fun doc ->
            match doc.SelectSingleNode(sprintf "doc/members/member[@name='%s']" xmlDocSig) with
            | null -> None
            | node when node.HasChildNodes -> Some node
            | _ -> None)

    member _.TryGetXmlDocBySig(xmlDocSig: string) =
        tryGetSummaryNode xmlDocSig
        |> Option.map (fun node ->
            let childNodes = node.ChildNodes
            let lines = Array.zeroCreate childNodes.Count
            for i = 0 to childNodes.Count - 1 do
                let childNode = childNodes.[i]
                lines.[i] <- childNode.OuterXml
            XmlDoc(lines, range0)
        )

    static member TryCreateFromFile(xmlFileName: string) =
        if not (FileSystem.FileExistsShim(xmlFileName)) || not (String.Equals(Path.GetExtension(xmlFileName), ".xml", StringComparison.OrdinalIgnoreCase)) then
            None
        else
            let tryGetXmlDocument =
                fun () ->
                    try
                        let lastWriteTime = FileSystem.GetLastWriteTimeShim(xmlFileName)
                        let cacheKey = (xmlFileName, lastWriteTime)
                        match cache.TryGet((), cacheKey) with
                        | Some doc -> Some doc
                        | _ ->
                            let doc = XmlDocument()
                            use xmlStream = FileSystem.OpenFileForReadShim(xmlFileName)
                            doc.Load(xmlStream)
                            cache.Put((), cacheKey, doc)
                            Some doc
                    with
                    | _ ->
                        None
            Some(XmlDocumentationInfo(tryGetXmlDocument))

type IXmlDocumentationInfoLoader =

    abstract TryLoad : assemblyFileName: string * ILModuleDef -> XmlDocumentationInfo option
