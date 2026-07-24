// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Xml.XmlDocIncludeExpander

open System
open System.Collections.Generic
open System.IO
open System.Runtime.InteropServices
open System.Xml
open System.Xml.Linq
open System.Xml.XPath
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.IO
open FSharp.Compiler.Text
open Internal.Utilities.Library

[<Literal>]
let private maxIncludeDepth = 64

[<Literal>]
let private maxIncludeExpansions = 10000

/// Path comparison must match the host filesystem: case-insensitive on Windows/macOS, case-sensitive elsewhere.
let private pathComparer =
    if
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
    then
        StringComparer.OrdinalIgnoreCase
    else
        StringComparer.Ordinal

/// Cycle-detection key comparer: path per the OS-aware pathComparer, xpath ordinal.
/// The same file included via a DIFFERENT xpath is NOT a cycle, so both parts matter.
let private includeKeyComparer =
    { new IEqualityComparer<struct (string * string)> with
        member _.Equals(struct (p1, x1), struct (p2, x2)) =
            pathComparer.Equals(p1, p2) && String.Equals(x1, x2, StringComparison.Ordinal)

        member _.GetHashCode(struct (p, x)) =
            // Combine hashes consistently with Equals (path via pathComparer, xpath ordinal).
            (pathComparer.GetHashCode(p) <<< 1)
            + StringComparer.Ordinal.GetHashCode(x)
            + 631
    }

/// Per-pass shared state: file cache, remaining expansion budget, and fully expanded fragment memo.
type ExpansionEnv =
    {
        FileCache: Dictionary<string, Result<XDocument, string>>
        mutable Budget: int
        Memo: Dictionary<struct (string * string), struct (XNode list * int)>
    }

let mkExpansionEnv () : ExpansionEnv =
    {
        FileCache = Dictionary<string, Result<XDocument, string>>(pathComparer)
        Budget = maxIncludeExpansions
        Memo = Dictionary<struct (string * string), struct (XNode list * int)>(includeKeyComparer)
    }

/// Roslyn-parity comment inserted when an <include> XPath is valid but matches no elements.
/// No warning is emitted in this case; the original tag is kept after the comment.
let private noMatchCommentText =
    " No matching elements were found for the following include tag "

/// Load an XML file from disk, using a per-pass shared cache.
/// The cache avoids re-reading the same file within a single doc generation pass
/// while avoiding stale data across compilations (unlike a global static cache).
let private loadXmlFile (cache: Dictionary<string, Result<XDocument, string>>) (filePath: string) : Result<XDocument, string> =
    match cache.TryGetValue(filePath) with
    | true, result -> result
    | false, _ ->
        let result =
            try
                if not (FileSystem.FileExistsShim(filePath)) then
                    Result.Error $"File not found: {filePath}"
                else
                    use stream = FileSystem.OpenFileForReadShim(filePath)

                    let settings =
                        XmlReaderSettings(DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null)

                    use reader = XmlReader.Create(stream, settings)

                    let doc =
                        XDocument.Load(reader, LoadOptions.PreserveWhitespace ||| LoadOptions.SetLineInfo)

                    Result.Ok doc
            with ex ->
                Result.Error $"Error loading file '{filePath}': {ex.Message}"

        cache[filePath] <- result
        result

/// Resolve a file path (absolute or relative to source file).
/// Always normalizes via GetFullPath so that cycle detection uses canonical paths.
let private resolveFilePath (baseFileName: string) (includePath: string) : string =
    if FileSystem.IsPathRootedShim(includePath) then
        FileSystem.GetFullPathShim(includePath)
    else
        let baseDir =
            if String.IsNullOrEmpty(baseFileName) || baseFileName = "unknown" then
                Directory.GetCurrentDirectory()
            else
                let dir = FileSystem.GetDirectoryNameShim(baseFileName)

                if String.IsNullOrEmpty(dir) then
                    Directory.GetCurrentDirectory()
                else
                    dir

        FileSystem.GetFullFilePathInDirectoryShim baseDir includePath

/// Evaluate XPath and return matching elements
let private evaluateXPath (doc: XDocument) (xpath: string) : Result<XElement list, string> =
    try
        if String.IsNullOrWhiteSpace(xpath) then
            Result.Error "XPath expression is empty"
        else
            // Materialize inside the try: XPathSelectElements is lazily enumerated and throws
            // InvalidOperationException during enumeration when the result is not a set of elements
            // (for example a text or attribute node-set). Enumerating here keeps that a warning.
            Result.Ok(doc.XPathSelectElements(xpath) |> List.ofSeq)
    with ex ->
        Result.Error $"Invalid XPath expression '{xpath}': {ex.Message}"

/// Include directive information
type private IncludeInfo = { FilePath: string; XPath: string }

/// Quick check if a string might contain an include tag (no allocations)
let private mayContainInclude (text: string) : bool =
    not (String.IsNullOrEmpty(text)) && text.Contains("<include")

/// Classify an XElement as an include directive.
/// Returns Some(Ok info) for valid includes, Some(Error msg) for malformed includes, None for non-includes.
let private classifyInclude (elem: XElement) : Result<IncludeInfo, string> option =
    if elem.Name.LocalName <> "include" then
        None
    else
        let fileAttr = elem.Attribute(!!(XName.op_Implicit "file"))
        let pathAttr = elem.Attribute(!!(XName.op_Implicit "path"))

        match fileAttr, pathAttr with
        | NonNull file, NonNull path ->
            Some(
                Result.Ok
                    {
                        FilePath = file.Value
                        XPath = path.Value
                    }
            )
        | NonNull _, Null -> Some(Result.Error "<include> element is missing required 'path' attribute")
        | Null, NonNull _ -> Some(Result.Error "<include> element is missing required 'file' attribute")
        | Null, Null -> Some(Result.Error "<include> element is missing required 'file' and 'path' attributes")

let private cloneNode (node: XNode) : XNode =
    match node with
    | :? XElement as element -> XElement(element) :> XNode
    | :? XText as text -> XText(text) :> XNode
    | :? XComment as comment -> XComment(comment) :> XNode
    | other -> XNode.ReadFrom(other.CreateReader())

let private cloneNodes (nodes: XNode list) : XNode seq = nodes |> Seq.map cloneNode

/// Expansion context threaded through recursive calls
type private ExpansionContext =
    {
        Env: ExpansionEnv
        InProgressIncludes: HashSet<struct (string * string)>
        Depth: int
        Range: range
        Emit: bool
        HadError: bool ref
        MaxDepth: int ref
    }

let private noteResolvedDepth (ctx: ExpansionContext) depth =
    if depth > ctx.MaxDepth.Value then
        ctx.MaxDepth.Value <- depth

let private warnIncludeError (ctx: ExpansionContext) (msg: string) =
    ctx.HadError.Value <- true

    if ctx.Emit then
        warning (Error(FSComp.SR.xmlDocIncludeError msg, ctx.Range))

/// Outcome of resolving a single <include> directive.
type private IncludeOutcome =
    /// Expanded to these nodes.
    | IncludeResolved of XNode seq
    /// Valid XPath but zero matches: Roslyn parity is a comment + the kept tag, with no warning.
    | IncludeNoMatch
    /// Genuine failure (missing file, invalid/empty XPath, cycle): warn and keep the tag.
    | IncludeError of string

/// Load and expand includes from an external file
let rec private resolveSingleInclude (baseFileName: string) (includeInfo: IncludeInfo) (ctx: ExpansionContext) : IncludeOutcome =

    let resolvedPathResult =
        try
            Result.Ok(resolveFilePath baseFileName includeInfo.FilePath)
        with _ ->
            Result.Error $"Invalid file path: {includeInfo.FilePath}"

    match resolvedPathResult with
    | Result.Error msg -> IncludeError msg
    | Result.Ok resolvedPath ->

        let key = struct (resolvedPath, includeInfo.XPath)

        if ctx.InProgressIncludes.Contains(key) then
            IncludeError $"Circular include detected: {resolvedPath}"
        elif ctx.Depth >= maxIncludeDepth then
            IncludeError $"include expansion limit exceeded (maximum nesting depth {maxIncludeDepth}) at: {resolvedPath}"
        elif ctx.Env.Budget <= 0 then
            IncludeError $"include expansion limit exceeded (maximum of {maxIncludeExpansions} includes) at: {resolvedPath}"
        else
            match ctx.Env.Memo.TryGetValue key with
            | true, struct (nodes, expansionDepth) when ctx.Depth + expansionDepth <= maxIncludeDepth ->
                ctx.Env.Budget <- ctx.Env.Budget - 1
                noteResolvedDepth ctx (ctx.Depth + expansionDepth - 1)
                IncludeResolved(cloneNodes nodes)
            | _ ->
                match
                    loadXmlFile ctx.Env.FileCache resolvedPath
                    |> Result.bind (fun includeDoc -> evaluateXPath includeDoc includeInfo.XPath)
                with
                | Result.Error msg -> IncludeError msg
                | Result.Ok elements ->
                    match elements with
                    | [] -> IncludeNoMatch
                    | matchedElements ->
                        ctx.Env.Budget <- ctx.Env.Budget - 1

                        // Clone the in-progress set and add this (file,xpath) for recursive expansion.
                        let childInProgress =
                            HashSet<struct (string * string)>(ctx.InProgressIncludes, includeKeyComparer)

                        childInProgress.Add(key) |> ignore

                        let fragmentHadError = ref false
                        let fragmentMaxDepth = ref ctx.Depth

                        let childCtx =
                            { ctx with
                                InProgressIncludes = childInProgress
                                Depth = ctx.Depth + 1
                                HadError = fragmentHadError
                                MaxDepth = fragmentMaxDepth
                            }

                        let resolvedNodes =
                            expandAllIncludeNodes resolvedPath (matchedElements |> Seq.cast<XNode>) childCtx
                            |> Seq.toList

                        noteResolvedDepth ctx fragmentMaxDepth.Value

                        if fragmentHadError.Value then
                            ctx.HadError.Value <- true
                        else
                            let expansionDepth = fragmentMaxDepth.Value - ctx.Depth + 1
                            ctx.Env.Memo[key] <- struct (resolvedNodes |> List.map cloneNode, expansionDepth)

                        IncludeResolved resolvedNodes

/// Recursively expand includes in XElement nodes
and private expandAllIncludeNodes (baseFileName: string) (nodes: XNode seq) (ctx: ExpansionContext) : XNode seq =
    nodes
    |> Seq.collect (fun node ->
        if node.NodeType <> System.Xml.XmlNodeType.Element then
            Seq.singleton node
        else
            let elem = node :?> XElement

            match classifyInclude elem with
            | None ->
                let expandedChildren = expandAllIncludeNodes baseFileName (elem.Nodes()) ctx
                let newElem = XElement(elem.Name, elem.Attributes(), expandedChildren)
                Seq.singleton (newElem :> XNode)
            | Some(Result.Error msg) ->
                warnIncludeError ctx msg
                Seq.singleton node
            | Some(Result.Ok includeInfo) ->
                match resolveSingleInclude baseFileName includeInfo ctx with
                | IncludeResolved expandedNodes -> expandedNodes
                | IncludeNoMatch ->
                    // Roslyn parity: valid XPath, zero matches => comment + keep the tag, no warning.
                    seq {
                        XComment(noMatchCommentText) :> XNode
                        node
                    }
                | IncludeError msg ->
                    warnIncludeError ctx msg
                    Seq.singleton node)

/// Expand all <include> elements in the given elaborated XML doc lines.
/// `emit` controls whether include errors are reported as warnings (build path)
/// or suppressed (quiet validation path). Returns the input unchanged when there
/// are no include tags, when parsing fails, or when nothing was expanded.
let expandIncludeLines (env: ExpansionEnv) (emit: bool) (baseFileName: string) (range: range) (lines: string[]) : string[] =
    let hasIncludes = lines |> Array.exists mayContainInclude

    if not hasIncludes then
        lines
    else
        let text = lines |> String.concat "\n"

        let parsedRoot =
            try
                Some(
                    XElement.Parse(
                        "<__include_root__>" + text + "</__include_root__>",
                        LoadOptions.PreserveWhitespace ||| LoadOptions.SetLineInfo
                    )
                )
            with _ ->
                None

        match parsedRoot with
        | None -> lines
        | Some root ->
            let ctx =
                {
                    Env = env
                    InProgressIncludes = HashSet<struct (string * string)>(includeKeyComparer)
                    Depth = 0
                    Range = range
                    Emit = emit
                    HadError = ref false
                    MaxDepth = ref -1
                }

            let expandedText =
                expandAllIncludeNodes baseFileName (root.Nodes()) ctx
                |> Seq.map (fun (n: XNode) -> n.ToString(SaveOptions.DisableFormatting))
                |> String.concat ""

            let expandedLines = String.getLines expandedText

            if Array.lengthsEqAndForall2 (=) expandedLines lines then
                lines
            else
                expandedLines
