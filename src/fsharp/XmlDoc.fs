// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module public FSharp.Compiler.XmlDoc

open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Range

/// Represents the final form of collected XmlDoc lines
type XmlDoc =
    | XmlDoc of string[]
    
    static member Empty = XmlDocStatics.Empty
    
    member x.NonEmpty = (let (XmlDoc lines) = x in lines.Length <> 0)
    
    static member Merge (XmlDoc lines) (XmlDoc lines') = XmlDoc (Array.append lines lines')
    
    /// This code runs for .XML generation and thus influences cross-project xmldoc tooltips; for within-project tooltips,
    /// see XmlDocumentation.fs in the language service
    static member Process (XmlDoc lines) =
        let rec processLines (lines: string list) =
            match lines with
            | [] -> []
            | (lineA :: rest) as lines ->
                let lineAT = lineA.TrimStart([|' '|])
                if lineAT = "" then processLines rest
                else if lineAT.StartsWithOrdinal("<") then lines
                else ["<summary>"] @
                     (lines |> List.map (fun line -> Microsoft.FSharp.Core.XmlAdapters.escape line)) @
                     ["</summary>"]

        let lines = processLines (Array.toList lines)
        if isNil lines then XmlDoc.Empty
        else XmlDoc (Array.ofList lines)

// Discriminated unions can't contain statics, so we use a separate type
and XmlDocStatics() =

    static let empty = XmlDoc[| |]

    static member Empty = empty

/// Used to collect XML documentation during lexing and parsing.
type XmlDocCollector() =
    let mutable savedLines = new ResizeArray<(string * pos)>()
    let mutable savedGrabPoints = new ResizeArray<pos>()
    let posCompare p1 p2 = if posGeq p1 p2 then 1 else if posEq p1 p2 then 0 else -1
    let savedGrabPointsAsArray =
        lazy (savedGrabPoints.ToArray() |> Array.sortWith posCompare)

    let savedLinesAsArray =
        lazy (savedLines.ToArray() |> Array.sortWith (fun (_, p1) (_, p2) -> posCompare p1 p2))

    let check() =
        // can't add more XmlDoc elements to XmlDocCollector after extracting first XmlDoc from the overall results
        assert (not savedLinesAsArray.IsValueCreated)

    member x.AddGrabPoint pos =
        check()
        savedGrabPoints.Add pos

    member x.AddXmlDocLine(line, pos) =
        check()
        savedLines.Add(line, pos)

    member x.LinesBefore grabPointPos =
      try
        let lines = savedLinesAsArray.Force()
        let grabPoints = savedGrabPointsAsArray.Force()
        let firstLineIndexAfterGrabPoint = Array.findFirstIndexWhereTrue lines (fun (_, pos) -> posGeq pos grabPointPos)
        let grabPointIndex = Array.findFirstIndexWhereTrue grabPoints (fun pos -> posGeq pos grabPointPos)
        assert (posEq grabPoints.[grabPointIndex] grabPointPos)
        let firstLineIndexAfterPrevGrabPoint =
            if grabPointIndex = 0 then
                0
            else
                let prevGrabPointPos = grabPoints.[grabPointIndex-1]
                Array.findFirstIndexWhereTrue lines (fun (_, pos) -> posGeq pos prevGrabPointPos)

        let lines = lines.[firstLineIndexAfterPrevGrabPoint..firstLineIndexAfterGrabPoint-1] |> Array.rev
        if lines.Length = 0 then
            [| |]
        else
            let firstLineNumber = (snd lines.[0]).Line
            lines
            |> Array.mapi (fun i x -> firstLineNumber - i, x)
            |> Array.takeWhile (fun (sequencedLineNumber, (_, pos)) -> sequencedLineNumber = pos.Line)
            |> Array.map (fun (_, (lineStr, _)) -> lineStr)
            |> Array.rev
      with e ->
        //printfn "unexpected error in LinesBefore:\n%s" (e.ToString())
        [| |]

/// Represents the XmlDoc fragments as collected from the lexer during parsing
type PreXmlDoc =
    | PreXmlMerge of PreXmlDoc * PreXmlDoc
    | PreXmlDoc of pos * XmlDocCollector
    | PreXmlDocEmpty

    member x.ToXmlDoc() =
        match x with
        | PreXmlMerge(a, b) -> XmlDoc.Merge (a.ToXmlDoc()) (b.ToXmlDoc())
        | PreXmlDocEmpty -> XmlDoc.Empty
        | PreXmlDoc (pos, collector) ->
            let lines = collector.LinesBefore pos
            if lines.Length = 0 then XmlDoc.Empty
            else XmlDoc lines

    static member CreateFromGrabPoint(collector: XmlDocCollector, grabPointPos) =
        collector.AddGrabPoint grabPointPos
        PreXmlDoc(grabPointPos, collector)

    static member Empty = PreXmlDocEmpty

    static member Merge a b = PreXmlMerge (a, b)

