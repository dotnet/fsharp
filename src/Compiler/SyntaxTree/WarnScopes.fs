// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.UnicodeLexing
open Internal.Utilities.Library
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
module internal WarnScopes =

    // *************************************
    // Temporary storage (during lexing one file) for warn scope related data
    // *************************************

    type private FileIndex = int
    type private WarningNumber = int
    type private LineNumber = int

    [<RequireQualifiedAccess>]
    type private WarnCmd =
        | Nowarn of WarningNumber * range
        | Warnon of WarningNumber * range

        member w.WarningNumber =
            match w with
            | Nowarn(n, _)
            | Warnon(n, _) -> n

    type private WarnDirective =
        {
            DirectiveRange: range
            CommentRange: range option
            WarnCmds: WarnCmd list
        }

    let private isWarnonDirective (w: WarnDirective) =
        match w.WarnCmds with
        | [] -> false
        | h :: _ -> h.IsWarnon

    type private LineDirective =
        {
            SurrogateFileIndex: FileIndex
            SurrogateLine: LineNumber
            OriginalLine: LineNumber
        }

    type private LexbufData =
        {
            OriginalFileIndex: int
            mutable WarnDirectives: WarnDirective list
            mutable LineDirectives: LineDirective list
        }

    let private initialData (lexbuf: Lexbuf) =
        {
            OriginalFileIndex = lexbuf.StartPos.FileIndex
            WarnDirectives = []
            LineDirectives = []
        }

    let private getLexbufData (lexbuf: Lexbuf) =
        lexbuf.GetLocalData("WarnScopeData", (fun () -> initialData lexbuf))

    // *************************************
    // Collect the line directives during lexing
    // *************************************

    let RegisterLineDirective (lexbuf, fileIndex, line: int) = //TODO: send OriginalLine i.o. lexbuf
        let data = getLexbufData lexbuf

        let lineDirective =
            {
                SurrogateFileIndex = fileIndex
                SurrogateLine = line
                OriginalLine = lexbuf.StartPos.OriginalLine + 1
            }

        data.LineDirectives <- lineDirective :: data.LineDirectives

    // *************************************
    // Collect the warn scopes during lexing
    // *************************************

    let private getNumber (langVersion: LanguageVersion) m (ns: string) =
        let argFeature = LanguageFeature.ParsedHashDirectiveArgumentNonQuotes

        let removeQuotes (s: string) =
            if s.StartsWithOrdinal "\"" && s.EndsWithOrdinal "\"" then
                if s.StartsWithOrdinal "\"\"\"" && s.EndsWithOrdinal "\"\"\"" then
                    Some(s.Substring(3, s.Length - 6))
                else
                    Some(s.Substring(1, s.Length - 2))
            elif tryCheckLanguageFeatureAndRecover langVersion argFeature m then
                Some s
            else
                None

        let removePrefix (s: string) =
            match s.StartsWithOrdinal "FS", langVersion.SupportsFeature argFeature with
            | true, true -> Some(s.Substring 2, s)
            | true, false ->
                warning (Error(FSComp.SR.buildInvalidWarningNumber s, m))
                None
            | false, _ -> Some(s, s)

        let parseInt (intString: string, argString) =
            match System.Int32.TryParse intString with
            | true, i -> Some i
            | false, _ ->
                if langVersion.SupportsFeature LanguageFeature.ScopedNowarn then
                    errorR (Error(FSComp.SR.buildInvalidWarningNumber argString, m))
                elif langVersion.SupportsFeature argFeature then
                    warning (Error(FSComp.SR.buildInvalidWarningNumber argString, m))

                None

        ns |> removeQuotes |> Option.bind removePrefix |> Option.bind parseInt

    let private regex =
        // This regex is matching the following lexer pattern that brought us here:
        // anywhite* ("#nowarn" | "#warnon") anystring newline
        // while extracting from "anystring" the directive arguments and the comment.
        // A directive argument is any group of characters that is not a space, a newline, a slash or a semicolon.
        // Both the presence and syntactic correctness of the directive arguments are checked later.
        // For compatibility reasons, the arguments are allowed to be followed by a double semicolon.
        // The comment is optional and starts with "//".
        // The "(?: ...)?" is just a way to make the arguments optional while  not interfering with the capturing.
        // Matching a directive with this regex creates 5 groups (next to the full match):
        // 1. The leading whitespace.
        // 2. The directive identifier ("nowarn" or "warnon", possibly followed by additional characters).
        // 3. The directive arguments (if any), with each argument in a separate capture.
        // 4. The trailing whitespace.
        // 5. The comment (if any).

        Regex("""( *)#(\S+)(?: +([^ \r\n/;]+))*(?:;;)?( *)(\/\/.*)?$""", RegexOptions.CultureInvariant)

    let private parseDirective originalFileIndex lexbuf =
        let text = Lexbuf.LexemeString lexbuf
        let startPos = lexbuf.StartPos

        let mGroups = (regex.Match text).Groups
        let dIdentGroup = mGroups[2]
        let dIdent = dIdentGroup.Value
        let argsGroup = mGroups[3]
        let argCaptures = [ for c in argsGroup.Captures -> c ]
        let commentGroup = mGroups[5]

        let positions line offset length =
            mkPos line (startPos.Column + offset), mkPos line (startPos.Column + offset + length)
        // "normal" ranges (i.e. taking #line directives into account), for errors in the warn directive
        let mkRange offset length =
            positions lexbuf.StartPos.Line offset length
            ||> mkFileIndexRange startPos.FileIndex
        // "original" ranges, for the warn scopes
        let mkOriginalRange offset length =
            positions lexbuf.StartPos.OriginalLine offset length
            ||> mkFileIndexRange originalFileIndex

        let directiveLength =
            if argsGroup.Success then
                argsGroup.Index - (dIdentGroup.Index - 1) + argsGroup.Length
            else
                dIdentGroup.Length + 1

        let directiveRange = mkRange (dIdentGroup.Index - 1) directiveLength

        let commentRange =
            if commentGroup.Success then
                Some(mkRange commentGroup.Index commentGroup.Length)
            else
                None

        if argCaptures.IsEmpty then
            errorR (Error(FSComp.SR.lexWarnDirectiveMustHaveArgs (), directiveRange))

        let mkDirective ctor (c: Capture) =
            getNumber lexbuf.LanguageVersion (mkRange c.Index c.Length) c.Value
            |> Option.map (fun n -> ctor (n, mkOriginalRange c.Index c.Length))

        let warnCmds =
            match dIdent with
            | "warnon" -> argCaptures |> List.choose (mkDirective WarnCmd.Warnon)
            | "nowarn" -> argCaptures |> List.choose (mkDirective WarnCmd.Nowarn)
            | _ ->
                errorR (Error(FSComp.SR.fsiInvalidDirective ($"#{dIdent}", ""), directiveRange))
                []

        {
            DirectiveRange = directiveRange
            CommentRange = commentRange
            WarnCmds = warnCmds
        }

    let ParseAndRegisterWarnDirective (lexbuf: Lexbuf) =
        let data = getLexbufData lexbuf
        let warnDirective = parseDirective data.OriginalFileIndex lexbuf
        data.WarnDirectives <- warnDirective :: data.WarnDirectives

    // *************************************
    // After lexing, the (processed) warn scope data are kept in diagnosticOptions
    // *************************************

    [<RequireQualifiedAccess>]
    type private WarnScope =
        | Off of range
        | On of range
        | OpenOff of range
        | OpenOn of range

    /// Information about the mapping implied by the #line directives.
    /// The Map key is the file index of the surrogate source (the source file pointed to by the line directive).
    /// The Map value contains the file index of the original source (the one just being parsed) and
    /// a list of mapped sections (surrogate and original start lines).
    type private LineMaps = Map<FileIndex, FileIndex * (LineNumber * LineNumber) list>

    type private WarnScopeData =
        {
            ScopedNowarnFeatureIsSupported: bool
            ScriptNowarns: WarningNumber list // only needed to avoid breaking changes for previous language versions
            WarnScopes: Map<FileIndex, Map<WarningNumber, WarnScope list>>
            LineMaps: LineMaps
        }

    let private getWarnScopeData (diagnosticOptions: FSharpDiagnosticOptions) =
        match diagnosticOptions.WarnScopeData with
        | None ->
            {
                ScopedNowarnFeatureIsSupported = true
                ScriptNowarns = []
                WarnScopes = Map.empty
                LineMaps = Map.empty
            }
        | Some data -> data :?> WarnScopeData

    let private setWarnScopeData (diagnosticOptions: FSharpDiagnosticOptions) data =
        diagnosticOptions.WarnScopeData <- Some data

    // *************************************
    // Create the warn scopes from the directives and store them in diagnosticOptions.
    // *************************************

    let MergeInto diagnosticOptions isScript subModuleRanges lexbuf =
        let lexbufData = getLexbufData lexbuf
        let fileIndex = lexbufData.OriginalFileIndex

        let scopedNowarnFeatureIsSupported =
            lexbuf.LanguageVersion.SupportsFeature LanguageFeature.ScopedNowarn

        let fileWarnCmds, fileScriptNowarns =
            let warnDirectives = lexbufData.WarnDirectives |> List.rev

            if scopedNowarnFeatureIsSupported then
                List.collect _.WarnCmds warnDirectives, []
            else
                let isInSubmodule (warnDirective: WarnDirective) =
                    List.exists (fun mRange -> rangeContainsRange mRange warnDirective.DirectiveRange) subModuleRanges

                let subModuleWarnDirectives, topLevelWarnDirectives =
                    List.partition isInSubmodule warnDirectives

                // Warn about and ignore directives in submodules
                subModuleWarnDirectives
                |> List.iter (fun wd -> warning (Error(FSComp.SR.buildDirectivesInModulesAreIgnored (), wd.DirectiveRange)))

                let topLevelWarnonDirectives, topLevelNowarnDirectives =
                    List.partition isWarnonDirective topLevelWarnDirectives

                // "feature not available in this language version" error for top-level #nowarn
                topLevelWarnonDirectives
                |> List.iter (fun wd -> errorR (languageFeatureError lexbuf.LanguageVersion LanguageFeature.ScopedNowarn wd.DirectiveRange))

                let nowarnCmds = List.collect _.WarnCmds topLevelNowarnDirectives

                nowarnCmds,
                if isScript then
                    nowarnCmds |> List.map _.WarningNumber
                else
                    []

        let processWarnCmd warnScopeMap warnCmd =
            let getScopes warningNumber warnScopes =
                Map.tryFind warningNumber warnScopes |> Option.defaultValue []

            let mkScope (m1: range) (m2: range) =
                mkFileIndexRange m1.FileIndex m1.Start m2.End

            match warnCmd with
            | WarnCmd.Nowarn(n, m) ->
                match getScopes n warnScopeMap with
                | WarnScope.OpenOn m' :: t -> warnScopeMap.Add(n, WarnScope.On(mkScope m' m) :: t)
                | WarnScope.OpenOff m' :: _
                | WarnScope.On m' :: _ ->
                    if scopedNowarnFeatureIsSupported then
                        informationalWarning (Error(FSComp.SR.lexWarnDirectivesMustMatch ("#nowarn", m'.StartLine), m))

                    warnScopeMap
                | scopes -> warnScopeMap.Add(n, WarnScope.OpenOff(mkScope m m) :: scopes)
            | WarnCmd.Warnon(n, m) ->
                match getScopes n warnScopeMap with
                | WarnScope.OpenOff m' :: t -> warnScopeMap.Add(n, WarnScope.Off(mkScope m' m) :: t)
                | WarnScope.OpenOn m' :: _
                | WarnScope.Off m' :: _ ->
                    warning (Error(FSComp.SR.lexWarnDirectivesMustMatch ("#warnon", m'.EndLine), m))
                    warnScopeMap
                | scopes -> warnScopeMap.Add(n, WarnScope.OpenOn(mkScope m m) :: scopes)

        let fileWarnScopes = fileWarnCmds |> List.fold processWarnCmd Map.empty

        let fileLineMaps: LineMaps =
            let sortedSectionMaps =
                List.map (fun ld -> ld.SurrogateLine, ld.OriginalLine) >> List.sortBy fst

            lexbufData.LineDirectives
            |> List.groupBy _.SurrogateFileIndex
            |> List.map (fun (surrIdx, lineDirectives) -> surrIdx, (fileIndex, sortedSectionMaps lineDirectives))
            |> Map

        let merge () =
            let projectData = getWarnScopeData diagnosticOptions

            // If the same file is parsed again (same fileIndex), we replace the warn scopes.
            let projectWarnScopes = projectData.WarnScopes.Add(fileIndex, fileWarnScopes)

            // If the same surrogate file has entries already (from another parse), we replace the line maps.
            // However, if it was referred to from a different original file before, we issue a warning.
            // (Because it means the maps are not reliable.)
            let projectLineMaps =
                let checkAndAdd previousLinemaps surrIdx (newOrigIdx, linePairList as newLinemaps) =
                    match Map.tryFind surrIdx previousLinemaps with
                    | Some(origIdx, _) when origIdx <> newOrigIdx ->
                        let _, origLine = List.head linePairList
                        let m = mkFileIndexRange origIdx (mkPos origLine 0) (mkPos origLine 4)

                        let getName idx =
                            FileIndex.fileOfFileIndex idx |> System.IO.Path.GetFileName |> string

                        warning (Error(FSComp.SR.lexLineDirectiveMappingIsNotUnique (getName surrIdx, getName origIdx), m))
                    | _ -> ()

                    Map.add surrIdx newLinemaps previousLinemaps

                Map.fold checkAndAdd projectData.LineMaps fileLineMaps

            let newWarnScopeData =
                {
                    ScopedNowarnFeatureIsSupported = scopedNowarnFeatureIsSupported
                    ScriptNowarns = List.distinct (projectData.ScriptNowarns @ fileScriptNowarns)
                    WarnScopes = projectWarnScopes
                    LineMaps = projectLineMaps
                }

            setWarnScopeData diagnosticOptions newWarnScopeData

        lock diagnosticOptions merge

    let getDirectiveTrivia (lexbuf: Lexbuf) =
        let mkTrivia d =
            if isWarnonDirective d then
                WarnDirectiveTrivia.Warnon(d.WarnCmds |> List.map _.WarningNumber, d.DirectiveRange)
            else
                WarnDirectiveTrivia.Nowarn(d.WarnCmds |> List.map _.WarningNumber, d.DirectiveRange)

        (getLexbufData lexbuf).WarnDirectives |> List.rev |> List.map mkTrivia

    let getCommentTrivia (lexbuf: Lexbuf) =
        (getLexbufData lexbuf).WarnDirectives
        |> List.rev
        |> List.choose _.CommentRange
        |> List.map CommentTrivia.LineComment

    // *************************************
    // Apply the warn scopes after lexing
    // *************************************

    let private getScopes fileIndex warningNumber warnScopes =
        Map.tryFind fileIndex warnScopes
        |> Option.bind (Map.tryFind warningNumber)
        |> Option.defaultValue []

    let private originalRange lineMaps (m: range) =
        match Map.tryFind m.FileIndex lineMaps with
        | None -> m
        | Some(origFileIndex, sectionMaps) ->
            let surrLine, origLine =
                if List.isEmpty sectionMaps || m.StartLine < fst sectionMaps.Head then
                    (1, 1)
                else
                    sectionMaps |> List.skipWhile (fun (s, _) -> m.StartLine < s) |> List.head

            let origStart = mkPos (m.StartLine + origLine - surrLine) m.StartColumn
            let origEnd = mkPos (m.EndLine + origLine - surrLine) m.EndColumn
            mkFileIndexRange origFileIndex origStart origEnd

    // true if m1 contains the *start* of m2
    // i.e. if the error range encloses the closing warn directive, we still say it is in scope
    let private contains (m2: range) (m1: range) =
        m2.StartLine > m1.StartLine && m2.StartLine < m1.EndLine

    let private isEnclosingWarnonScope m scope =
        match scope with
        | WarnScope.On wm when contains m wm -> true
        | WarnScope.OpenOn wm when m.StartLine > wm.StartLine -> true
        | _ -> false

    let private isEnclosingNowarnScope m scope =
        match scope with
        | WarnScope.Off wm when contains m wm -> true
        | WarnScope.OpenOff wm when m.StartLine > wm.StartLine -> true
        | _ -> false

    let IsWarnon (diagnosticOptions: FSharpDiagnosticOptions) warningNumber (mo: range option) =
        let data = getWarnScopeData diagnosticOptions

        match mo, data.ScopedNowarnFeatureIsSupported with
        | Some m, true ->
            let mOrig = originalRange data.LineMaps m
            let scopes = getScopes mOrig.FileIndex warningNumber data.WarnScopes
            List.exists (isEnclosingWarnonScope mOrig) scopes
        | _ -> false

    let IsNowarn (diagnosticOptions: FSharpDiagnosticOptions) warningNumber (mo: range option) =
        let data = getWarnScopeData diagnosticOptions

        if List.contains warningNumber data.ScriptNowarns then // this happens only for legacy language versions
            true
        else
            match mo with
            | Some m ->
                let mOrig = originalRange data.LineMaps m
                let scopes = getScopes mOrig.FileIndex warningNumber data.WarnScopes
                List.exists (isEnclosingNowarnScope mOrig) scopes
            | None -> false
