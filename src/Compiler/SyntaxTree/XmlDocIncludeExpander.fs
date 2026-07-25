// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Xml.XmlDocIncludeExpander

open System
open System.Collections.Generic
open System.IO
open System.Xml
open System.Xml.Linq
open System.Xml.XPath
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.IO
open FSharp.Compiler.Text
open Internal.Utilities.Library

// Matches Roslyn's finite include-recursion guard; deep real-world docs stay far below this.
[<Literal>]
let private maxIncludeDepth = 64

// Per-document include budget: high enough for generated docs, finite to stop runaway expansion.
[<Literal>]
let private maxIncludeExpansions = 10000

let private pathComparer = StringComparer.Ordinal

/// Per-pass shared state: file cache only.
type ExpansionEnv =
    {
        FileCache: Dictionary<string, Result<XDocument, string>>
    }

let mkExpansionEnv () : ExpansionEnv =
    {
        FileCache = Dictionary<string, Result<XDocument, string>>(pathComparer)
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

/// Expansion context threaded through recursive calls
type private ExpansionContext =
    {
        Env: ExpansionEnv
        InProgressIncludes: HashSet<struct (string * string)>
        Depth: int
        Budget: int ref
        BudgetExhaustedWarned: bool ref
        Range: range
        Emit: bool
    }

let private warnIncludeError (ctx: ExpansionContext) (msg: string) =
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
    /// The per-document expansion budget is exhausted: keep the tag, but warn only once per document.
    | IncludeBudgetExceeded of string

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
        elif ctx.Budget.Value <= 0 then
            IncludeBudgetExceeded $"include expansion limit exceeded (maximum of {maxIncludeExpansions} includes) at: {resolvedPath}"
        else
            match
                loadXmlFile ctx.Env.FileCache resolvedPath
                |> Result.bind (fun includeDoc -> evaluateXPath includeDoc includeInfo.XPath)
            with
            | Result.Error msg -> IncludeError msg
            | Result.Ok [] -> IncludeNoMatch
            | Result.Ok matchedElements ->
                ctx.Budget.Value <- ctx.Budget.Value - 1
                let childInProgress = HashSet<struct (string * string)>(ctx.InProgressIncludes)
                childInProgress.Add(key) |> ignore

                let childCtx =
                    { ctx with
                        InProgressIncludes = childInProgress
                        Depth = ctx.Depth + 1
                    }

                IncludeResolved(expandAllIncludeNodes resolvedPath (matchedElements |> Seq.cast<XNode>) childCtx)

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
                    Seq.singleton node
                | IncludeBudgetExceeded msg ->
                    // Report the document-wide budget limit at most once to avoid warning spam.
                    if not ctx.BudgetExhaustedWarned.Value then
                        ctx.BudgetExhaustedWarned.Value <- true
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
                    InProgressIncludes = HashSet<struct (string * string)>()
                    Depth = 0
                    Budget = ref maxIncludeExpansions
                    BudgetExhaustedWarned = ref false
                    Range = range
                    Emit = emit
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
