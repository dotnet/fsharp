// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Xml

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.IO
open System.Xml
open System.Xml.Linq
open Internal.Utilities.Library
open Internal.Utilities.Collections
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.IO
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range

/// The tag an XML doc attribute value belongs to
[<RequireQualifiedAccess>]
type XmlDocRefKind =
    | Param
    | ParamRef
    | TypeParam
    | TypeParamRef
    | Cref

/// An attribute value in an XML doc that names something else: a `name` or a `cref`
[<Struct>]
type XmlDocRef =
    {
        Kind: XmlDocRefKind
        Text: string
        Range: range
    }

module private XmlDocRefs =

    let private nameAttribute = XName.Get "name"

    let private crefAttribute = XName.Get "cref"

    /// What an element names and the attribute that holds the name, for the elements that name something
    let private refOf (element: XElement) =
        if element.Name.NamespaceName.Length <> 0 then
            ValueNone
        else
            match element.Name.LocalName with
            | "param" -> ValueSome struct (XmlDocRefKind.Param, nameAttribute)
            | "paramref" -> ValueSome struct (XmlDocRefKind.ParamRef, nameAttribute)
            | "typeparam" -> ValueSome struct (XmlDocRefKind.TypeParam, nameAttribute)
            | "typeparamref" -> ValueSome struct (XmlDocRefKind.TypeParamRef, nameAttribute)
            | "see"
            | "seealso"
            | "exception"
            | "permission" -> ValueSome struct (XmlDocRefKind.Cref, crefAttribute)
            | _ -> ValueNone

    /// The three slashes of `///` are not part of the stored line
    let private lineTextOffset = 3

    let private isBlank (line: string) = line.AsSpan().TrimStart(' ').IsEmpty

    /// The line the XML of a doc starts on, or -1 for a blank doc or one of plain text, which becomes an escaped
    /// `<summary>` and so names nothing: the rule `XmlDoc.GetElaboratedXmlLines` applies
    let private firstXmlLine (lines: string[]) =
        match Array.tryFindIndex (isBlank >> not) lines with
        | Some i when lines[i].AsSpan().TrimStart(' ').StartsWith("<".AsSpan(), StringComparison.Ordinal) -> i
        | _ -> -1

    let private skipWhiteSpace (text: string) i =
        let mutable i = i

        while i < text.Length && Char.IsWhiteSpace text[i] do
            i <- i + 1

        i

    /// Where the quoted value of the attribute whose name starts at `nameStart` begins in `text`, and its length.
    /// The `=` and the opening quote may follow on later lines; a value that itself runs over a line break
    /// has no single-line range.
    let private valueSpan (text: string) (nameStart: int) (attributeName: string) =
        let equals = skipWhiteSpace text (nameStart + attributeName.Length)

        if equals < text.Length && text[equals] = '=' then
            let quote = skipWhiteSpace text (equals + 1)

            if quote < text.Length && (text[quote] = '"' || text[quote] = '\'') then
                let start = quote + 1

                match text.IndexOf(text[quote], start) with
                | -1 -> ValueNone
                | close when text.IndexOf('\n', start, close - start) >= 0 -> ValueNone
                | close -> ValueSome struct (start, close - start)
            else
                ValueNone
        else
            ValueNone

    /// Parses the doc's own lines, not the text with `<include>` expanded, so that every element maps back to a source line
    let collect (lines: string[]) (lineRanges: ImmutableArray<range>) =
        match firstXmlLine lines with
        | -1 -> ImmutableArray.empty
        | first ->
            let text = String.Join("\n", lines, first, lines.Length - first)

            let xml =
                if
                    text.AsSpan().Contains("name".AsSpan(), StringComparison.Ordinal)
                    || text.AsSpan().Contains("cref".AsSpan(), StringComparison.Ordinal)
                then
                    // A badly formed doc names nothing; FS3390 reports it
                    try
                        ValueSome(XDocument.Parse("<doc>\n" + text + "\n</doc>", LoadOptions.SetLineInfo))
                    with :? XmlException ->
                        ValueNone
                else
                    ValueNone

            match xml with
            | ValueNone -> ImmutableArray.empty
            | ValueSome xml ->
                // Where each of the parsed lines starts in `text`
                let lineStarts = Array.zeroCreate (lines.Length - first)

                for i in 1 .. lineStarts.Length - 1 do
                    lineStarts[i] <- lineStarts[i - 1] + lines[first + i - 1].Length + 1

                let refs = ImmutableArray.CreateBuilder()

                for element in xml.Descendants() do
                    match refOf element with
                    | ValueSome struct (kind, attributeName) ->
                        match element.Attribute attributeName with
                        | null -> ()
                        | attribute ->
                            let position = attribute :> IXmlLineInfo
                            // Line 1 of the parsed text is the wrapping `<doc>`
                            let nameLine = position.LineNumber - 2
                            let nameStart = lineStarts[nameLine] + position.LinePosition - 1

                            match valueSpan text nameStart attributeName.LocalName with
                            | ValueSome struct (start, length) ->
                                // A line break before or after the `=` puts the value on a later line than the name
                                let mutable line = nameLine

                                while line + 1 < lineStarts.Length && lineStarts[line + 1] <= start do
                                    line <- line + 1

                                let m = lineRanges[first + line]
                                let column = m.StartColumn + lineTextOffset + start - lineStarts[line]

                                refs.Add
                                    {
                                        Kind = kind
                                        Text = attribute.Value
                                        Range =
                                            mkFileIndexRange m.FileIndex (mkPos m.StartLine column) (mkPos m.StartLine (column + length))
                                    }
                            | ValueNone -> ()
                    | ValueNone -> ()

                refs.ToImmutable()

/// Represents collected XmlDoc lines
type XmlDoc(unprocessedLines: string[], lineRanges: ImmutableArray<range>, range: range) =
    // A `default` array from a caller has no lines, like an empty one
    let lineRanges =
        if lineRanges.IsDefault then
            ImmutableArray.empty
        else
            lineRanges

    do
        if lineRanges.Length <> 0 && lineRanges.Length <> unprocessedLines.Length then
            invalidArg (nameof lineRanges) "one range per line, or none"

    let rec processLines (lines: string list) =
        match lines with
        | [] -> []
        | lineA :: rest as lines ->
            let lineAT = lineA.TrimStart([| ' ' |])

            if String.IsNullOrEmpty(lineAT) then
                processLines rest
            elif lineAT.StartsWithOrdinal("<") then
                lines
            else
                [ "<summary>" ]
                @ (lines |> List.map Internal.Utilities.XmlAdapters.escape)
                @ [ "</summary>" ]

    new(unprocessedLines: string[], range: range) = XmlDoc(unprocessedLines, ImmutableArray.empty, range)

    /// Get the lines before insertion of implicit summary tags and encoding
    member _.UnprocessedLines = unprocessedLines

    /// Get the lines after insertion of implicit summary tags and encoding
    member _.GetElaboratedXmlLines() =
        let processedLines = processLines (Array.toList unprocessedLines)

        let lines = Array.ofList processedLines

        lines

    member _.Range = range

    /// The source range of each unprocessed line; empty when the doc did not come from source
    member _.LineRanges = lineRanges

    /// The `name` and `cref` attribute values in the doc, with their source ranges; empty without line ranges
    member _.GetRefs() =
        if lineRanges.IsEmpty then
            ImmutableArray.empty
        else
            XmlDocRefs.collect unprocessedLines lineRanges

    static member Empty = XmlDocStatics.Empty

    member _.IsEmpty = unprocessedLines |> Array.forall String.IsNullOrWhiteSpace

    member doc.NonEmpty = not doc.IsEmpty

    static member Merge (doc1: XmlDoc) (doc2: XmlDoc) =
        let range =
            if doc1.IsEmpty then doc2.Range
            elif doc2.IsEmpty then doc1.Range
            else unionRanges doc1.Range doc2.Range

        let lineRanges =
            if
                doc1.LineRanges.Length = doc1.UnprocessedLines.Length
                && doc2.LineRanges.Length = doc2.UnprocessedLines.Length
            then
                doc1.LineRanges.AddRange doc2.LineRanges
            else
                ImmutableArray.empty

        XmlDoc(Array.append doc1.UnprocessedLines doc2.UnprocessedLines, lineRanges, range)

    member doc.GetXmlText() =
        if doc.IsEmpty then
            ""
        else
            doc.GetElaboratedXmlLines() |> String.concat Environment.NewLine

    member doc.GetExpandedXmlText(emit) =
        doc.GetExpandedXmlText(emit, XmlDocIncludeExpander.mkExpansionEnv ())

    member doc.GetExpandedXmlText(emit, env: XmlDocIncludeExpander.ExpansionEnv) =
        if doc.IsEmpty then
            ""
        else
            XmlDocIncludeExpander.expandIncludeLines env emit doc.Range.FileName doc.Range (doc.GetElaboratedXmlLines())
            |> String.concat Environment.NewLine

    member doc.Check(paramNamesOpt: string list option) =
        try
            // emit=false: quiet expansion so included <param>/<paramref> reach validation; the writer emits FS3908.
            let expandedText = doc.GetExpandedXmlText false

            // We must wrap with <doc> in order to have only one root element
            let xml =
                XDocument.Parse("<doc>\n" + expandedText + "\n</doc>", LoadOptions.SetLineInfo ||| LoadOptions.PreserveWhitespace)

            // The parameter names are checked for consistency, so parameter references and
            // parameter documentation must match an actual parameter.  In addition, if any parameters
            // have documentation then all parameters must have documentation
            match paramNamesOpt with
            | None -> ()
            | Some paramNames ->

                for p in xml.Descendants(XName.op_Implicit "param") do
                    match p.Attribute(!!(XName.op_Implicit "name")) with
                    | null -> warning (Error(FSComp.SR.xmlDocMissingParameterName (), doc.Range))
                    | attr ->
                        let nm = attr.Value

                        if not (paramNames |> List.contains nm) then
                            warning (Error(FSComp.SR.xmlDocInvalidParameterName (RichText.mkParameter nm), doc.Range))

                let paramsWithDocs =
                    [
                        for p in xml.Descendants(XName.op_Implicit "param") do
                            match p.Attribute(!!(XName.op_Implicit "name")) with
                            | Null -> ()
                            | NonNull attr -> attr.Value
                    ]

                if paramsWithDocs.Length > 0 then

                    for p in paramNames do
                        if not (paramsWithDocs |> List.contains p) then
                            warning (Error(FSComp.SR.xmlDocMissingParameter (RichText.mkParameter p), doc.Range))

                let duplicates = paramsWithDocs |> List.duplicates

                for d in duplicates do
                    warning (Error(FSComp.SR.xmlDocDuplicateParameter (RichText.mkParameter d), doc.Range))

                for pref in xml.Descendants(XName.op_Implicit "paramref") do
                    match pref.Attribute(!!(XName.op_Implicit "name")) with
                    | null -> warning (Error(FSComp.SR.xmlDocMissingParameterName (), doc.Range))
                    | attr ->
                        let nm = attr.Value

                        if not (paramNames |> List.contains nm) then
                            warning (Error(FSComp.SR.xmlDocInvalidParameterName (RichText.mkParameter nm), doc.Range))

        with e ->
            warning (Error(FSComp.SR.xmlDocBadlyFormed e.Message, doc.Range))

// Discriminated unions can't contain statics, so we use a separate type
and XmlDocStatics() =

    static let empty = XmlDoc([||], range0)

    static member Empty = empty

/// Used to collect XML documentation during lexing and parsing.
type XmlDocCollector() =
    let mutable savedLines = ResizeArray<string * range>()
    let mutable savedGrabPoints = Dictionary<pos, struct (int * int * bool)>()
    let mutable currentGrabPointCommentsCount = 0
    let mutable delayedGrabPoint = ValueNone
    let mutable lastNonCommentTokenLine = 0

    member _.AddGrabPoint(pos: pos) =
        if currentGrabPointCommentsCount = 0 then
            ()
        else
            let xmlDocBlock =
                struct (savedLines.Count - currentGrabPointCommentsCount, savedLines.Count - 1, false)

            // silently override duplicate grab points (which happen only when preceded by nonsensical line directives)
            savedGrabPoints[pos] <- xmlDocBlock
            currentGrabPointCommentsCount <- 0
            delayedGrabPoint <- ValueNone

    member _.AddGrabPointDelayed(pos: pos) =
        if currentGrabPointCommentsCount = 0 then
            ()
        else
            match delayedGrabPoint with
            | ValueNone -> delayedGrabPoint <- ValueSome(pos)
            | _ -> ()

    member x.AddXmlDocLine(line, range) =
        match delayedGrabPoint with
        | ValueNone -> ()
        | ValueSome pos -> x.AddGrabPoint(pos) // Commit delayed grab point

        savedLines.Add(line, range)
        currentGrabPointCommentsCount <- currentGrabPointCommentsCount + 1

    member _.LinesBefore grabPointPos =
        match savedGrabPoints.TryGetValue grabPointPos with
        | true, struct (startIndex, endIndex, _) ->
            let linesBefore = Array.create (endIndex - startIndex + 1) ("", range0)

            for i in startIndex..endIndex do
                linesBefore[i - startIndex] <- savedLines[i]

            linesBefore
        | false, _ -> [||]

    member _.LinesRange grabPointPos =
        match savedGrabPoints.TryGetValue grabPointPos with
        | true, struct (startIndex, endIndex, _) ->
            let startRange = savedLines[startIndex] |> snd
            let endRange = savedLines[endIndex] |> snd
            unionRanges startRange endRange
        | false, _ -> range0

    member _.SetXmlDocValidity(grabPointPos, isValid) =
        match savedGrabPoints.TryGetValue grabPointPos with
        | true, struct (startIndex, endIndex, _) -> savedGrabPoints[grabPointPos] <- struct (startIndex, endIndex, isValid)
        | _ -> ()

    member _.HasComments grabPointPos =
        savedGrabPoints.TryGetValue grabPointPos |> fst

    member _.SetLastNonCommentTokenLine(line: int) = lastNonCommentTokenLine <- line

    member _.LastNonCommentTokenLine = lastNonCommentTokenLine

    member _.CheckInvalidXmlDocPositions() =
        let comments = ResizeArray<range>(savedLines.Count)

        for startIndex, endIndex, isValid in savedGrabPoints.Values do
            if isValid then
                ()
            else
                let _, startRange = savedLines[startIndex]
                let _, endRange = savedLines[endIndex]
                let range = unionRanges startRange endRange
                informationalWarning (Error(FSComp.SR.invalidXmlDocPosition (), range))
                // Collect invalid triple slash comment ranges, to later transform these to trivia
                [ startIndex..endIndex ]
                |> List.iter (fun idx -> savedLines[idx] |> snd |> comments.Add)

        List.ofSeq comments

/// Represents the XmlDoc fragments as collected from the lexer during parsing
type PreXmlDoc =
    | PreXmlDirect of unprocessedLines: string[] * range: range
    | PreXmlMerge of PreXmlDoc * PreXmlDoc
    | PreXmlDoc of pos * XmlDocCollector
    | PreXmlDocEmpty
    | PreXmlDocPairedWith of inner: PreXmlDoc * extraParamNames: string list

    member x.ToXmlDoc(check: bool, paramNamesOpt: string list option) =
        match x with
        | PreXmlDirect(lines, m) -> XmlDoc(lines, m)
        | PreXmlMerge(a, b) -> XmlDoc.Merge (a.ToXmlDoc(check, paramNamesOpt)) (b.ToXmlDoc(check, paramNamesOpt))
        | PreXmlDocEmpty -> XmlDoc.Empty
        | PreXmlDoc(pos, collector) ->
            let preLines = collector.LinesBefore pos

            if preLines.Length = 0 then
                XmlDoc.Empty
            else
                let lines = Array.map fst preLines
                let lineRanges = ImmutableArray.init preLines.Length (fun i -> snd preLines[i])
                let m = ImmutableArray.fold unionRanges lineRanges[0] lineRanges
                let doc = XmlDoc(lines, lineRanges, m)

                if check then
                    doc.Check(paramNamesOpt)

                doc
        | PreXmlDocPairedWith(inner, extra) ->
            let paramNamesOpt =
                match paramNamesOpt with
                | Some names -> Some(names @ extra)
                | None -> None

            inner.ToXmlDoc(check, paramNamesOpt)

    member x.Range =
        match x with
        | PreXmlDirect(_, m) -> m
        | PreXmlMerge(part1, part2) ->
            if part1.IsEmpty then part2.Range
            elif part2.IsEmpty then part1.Range
            else unionRanges part1.Range part2.Range
        | PreXmlDocEmpty -> range0
        | PreXmlDoc(pos, collector) -> collector.LinesRange pos
        | PreXmlDocPairedWith(inner, _) -> inner.Range

    member x.IsEmpty =
        match x with
        | PreXmlDirect(lines, _) -> lines |> Array.forall String.IsNullOrWhiteSpace
        | PreXmlMerge(a, b) -> a.IsEmpty && b.IsEmpty
        | PreXmlDocEmpty -> true
        | PreXmlDoc(pos, collector) -> not (collector.HasComments pos)
        | PreXmlDocPairedWith(inner, _) -> inner.IsEmpty

    member x.MarkAsInvalid() =
        match x with
        | PreXmlDoc(pos, collector) -> collector.SetXmlDocValidity(pos, false)
        | PreXmlDocPairedWith(inner, _) -> inner.MarkAsInvalid()
        | _ -> ()

    static member CreateFromGrabPoint(collector: XmlDocCollector, grabPointPos) =
        collector.SetXmlDocValidity(grabPointPos, true)
        PreXmlDoc(grabPointPos, collector)

    static member Empty = PreXmlDocEmpty

    static member Create(unprocessedLines, range) = PreXmlDirect(unprocessedLines, range)

    static member Merge a b = PreXmlMerge(a, b)

    static member WithExtraParamsForCheck(doc: PreXmlDoc, extraParamNames: string list) =
        match extraParamNames with
        | [] -> doc
        | _ -> PreXmlDocPairedWith(doc, extraParamNames)

[<Sealed>]
type XmlDocumentationInfo private (tryGetXmlDocument: unit -> XmlDocument option) =

    // 2 and 4 are arbitrary but should be reasonable enough
    [<Literal>]
    static let cacheStrongSize = 2

    [<Literal>]
    static let cacheMaxSize = 4

    static let cacheAreSimilar ((str1: string, dt1: DateTime), (str2: string, dt2: DateTime)) =
        str1.Equals(str2, StringComparison.OrdinalIgnoreCase) && dt1 = dt2

    static let cache =
        AgedLookup<unit, string * DateTime, XmlDocument>(
            keepStrongly = cacheStrongSize,
            areSimilar = cacheAreSimilar,
            keepMax = cacheMaxSize
        )

    let tryGetSummaryNode (xmlDocSig: string) =
        if xmlDocSig.Contains "'" && xmlDocSig.Contains "\"" then
            // No easy way to find this signature with XPath
            None
        else
            tryGetXmlDocument ()
            |> Option.bind (fun doc ->
                let name =
                    if xmlDocSig.Contains "'" then
                        $"\"{xmlDocSig}\""
                    else
                        $"'{xmlDocSig}'"

                match doc.SelectSingleNode $"doc/members/member[@name={name}]" with
                | null -> None
                | node when node.HasChildNodes -> Some node
                | _ -> None)

    member _.TryGetXmlDocBySig(xmlDocSig: string) =
        tryGetSummaryNode xmlDocSig
        |> Option.map (fun node ->
            let childNodes = node.ChildNodes
            let lines = Array.zeroCreate childNodes.Count

            for i = 0 to childNodes.Count - 1 do
                let childNode = !!childNodes[i]
                lines[i] <- childNode.OuterXml

            XmlDoc(lines, range0))

    static member TryCreateFromFile(xmlFileName: string) =
        if
            not (FileSystem.FileExistsShim(xmlFileName))
            || not (String.Equals(Path.GetExtension(xmlFileName), ".xml", StringComparison.OrdinalIgnoreCase))
        then
            None
        else
            let tryGetXmlDocument () =
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
                with _ ->
                    None

            Some(XmlDocumentationInfo(tryGetXmlDocument))

type IXmlDocumentationInfoLoader =

    abstract TryLoad: assemblyFileName: string -> XmlDocumentationInfo option
