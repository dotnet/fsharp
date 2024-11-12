// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open FSharp.Compiler.UnicodeLexing
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open Internal.Utilities.Library
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
module internal WarnScopes =

    // *************************************
    // Temporary storage (during lexing one file) for warn scope related data
    // *************************************

    [<RequireQualifiedAccess>]
    type private WarnCmd =
        | Nowarn of int * range
        | Warnon of int * range

    type private WarnDirective =
        {
            DirectiveRange: range
            CommentRange: range option
            WarnCmds: WarnCmd list
            IsWarnon: bool
        }

    type private TempData =
        {
            OriginalFileIndex: int
            mutable WarnDirectives: WarnDirective list
            mutable LineMap: Map<int, int * (int * int) list>
        }

    let private initialData (lexbuf: Lexbuf) =
        {
            OriginalFileIndex = lexbuf.StartPos.FileIndex
            WarnDirectives = []
            LineMap = Map.empty
        }

    let private getTempData (lexbuf: Lexbuf) =
        lexbuf.GetLocalData("WarnScopeData", (fun () -> initialData lexbuf))

    // *************************************
    // Collect the line directives to correctly interact with them
    // *************************************

    let RegisterLineDirective (lexbuf, fileIndex, line: int) =
        let data = getTempData lexbuf
        let sectionMap = line, lexbuf.StartPos.OriginalLine + 1

        let changer entry =
            match entry with
            | None -> Some(data.OriginalFileIndex, [ sectionMap ])
            | Some(originalFileIndex, maps) ->
                if originalFileIndex <> data.OriginalFileIndex then
                    let convert (p: Internal.Utilities.Text.Lexing.Position) = mkPos p.OriginalLine p.Column

                    let m =
                        mkFileIndexRange data.OriginalFileIndex (convert lexbuf.StartPos) (convert lexbuf.EndPos)

                    let surrFile = FileIndex.fileOfFileIndex fileIndex
                    let origFile = FileIndex.fileOfFileIndex originalFileIndex
                    warning (Error((0, $"File {surrFile} was used in line directives of {origFile} already."), m)) //TODO: FsComp.txt
                    entry
                else
                    Some(originalFileIndex, sectionMap :: maps)

        data.LineMap <- data.LineMap.Change(fileIndex, changer)

    // *************************************
    // Collect the warn scopes during lexing
    // *************************************

    type LexPosition = Internal.Utilities.Text.Lexing.Position

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
                if langVersion.SupportsFeature argFeature then
                    warning (Error(FSComp.SR.buildInvalidWarningNumber argString, m))

                None

        ns |> removeQuotes |> Option.bind removePrefix |> Option.bind parseInt

    let private regex =
        Regex(""" *#(nowarn|warnon|\S+)(?: +([^ \r\n/;]+))*(?:;;)? *(\/\/.*)?$""", RegexOptions.CultureInvariant)

    let private parseDirective originalFileIndex lexbuf =
        let text = Lexbuf.LexemeString lexbuf
        let startPos = lexbuf.StartPos

        let mGroups = (regex.Match text).Groups
        let dIdent = mGroups[1].Value
        let argCaptures = [ for c in mGroups[2].Captures -> c ]
        let commentGroup = mGroups[3]

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

        let m = mkRange 0 text.Length

        let directiveRange, commentRange =
            if commentGroup.Success then
                mkRange 0 commentGroup.Index, Some(mkRange commentGroup.Index commentGroup.Length)
            else
                m, None

        if argCaptures.IsEmpty then
            errorR (Error(FSComp.SR.lexWarnDirectiveMustHaveArgs (), m))

        let mkDirective ctor (c: Capture) =
            getNumber lexbuf.LanguageVersion (mkRange c.Index c.Length) c.Value
            |> Option.map (fun n -> ctor (n, (mkOriginalRange c.Index c.Length)))

        let isWarnon, warnCmds =
            match dIdent with
            | "warnon" -> true, argCaptures |> List.choose (mkDirective WarnCmd.Warnon)
            | "nowarn" -> false, argCaptures |> List.choose (mkDirective WarnCmd.Nowarn)
            | _ ->
                errorR (Error(FSComp.SR.fsiInvalidDirective ($"#{dIdent}", ""), m))
                false, []

        {
            DirectiveRange = directiveRange
            CommentRange = commentRange
            WarnCmds = warnCmds
            IsWarnon = isWarnon
        }

    let private getScopes idx warnScopes =
        Map.tryFind idx warnScopes |> Option.defaultValue []

    let private mkScope (m1: range) (m2: range) =
        mkFileIndexRange m1.FileIndex m1.Start m2.End

    let ParseAndRegisterWarnDirective (lexbuf: Lexbuf) =
        let data = getTempData lexbuf
        let warnDirective = parseDirective data.OriginalFileIndex lexbuf
        data.WarnDirectives <- warnDirective :: data.WarnDirectives

    // *************************************
    // After lexing, the (processed) warn scope data are kept in diagnosticOptions
    // *************************************

    type private FileIndex = int
    type private WarningNumber = int
    type private LineNumber = int

    /// The range between #nowarn and #warnon, or #warnon and #nowarn, for a warning number.
    /// Or between the directive and eof, for the "Open" cases.
    [<RequireQualifiedAccess>]
    type private WarnScope =
        | Off of range
        | On of range
        | OpenOff of range
        | OpenOn of range

    type private WarnScopeData =
        {
            /// The collected WarnScope objects (collected during lexing)
            warnScopes: Map<FileIndex * WarningNumber, WarnScope list>
            /// Information about the mapping implied by the #line directives.
            /// The Map key is the file index of the surrogate source.
            /// The Map value contains the file index of the original source and
            /// a list of mapped sections (surrogate and original start lines).
            lineMaps: Map<FileIndex, (FileIndex * (LineNumber * LineNumber) list)>
        }

    let private getWarnScopeData (diagnosticOptions: FSharpDiagnosticOptions) =
        match diagnosticOptions.WarnScopeData with
        | None ->
            {
                warnScopes = Map.empty
                lineMaps = Map.empty
            }
        | Some data -> data :?> WarnScopeData

    let private setWarnScopeData (diagnosticOptions: FSharpDiagnosticOptions) data =
        diagnosticOptions.WarnScopeData <- Some data

    // *************************************
    // Move the warnscope data to diagnosticOptions / make ranges available
    // *************************************

    let private processWarnCmd (langVersion: LanguageVersion) warnScopeMap (wd: WarnCmd) =
        match wd with
        | WarnCmd.Nowarn(n, m) ->
            let idx = m.FileIndex, n

            match getScopes idx warnScopeMap with
            | WarnScope.OpenOn m' :: t -> warnScopeMap.Add(idx, WarnScope.On(mkScope m' m) :: t)
            | WarnScope.OpenOff m' :: _
            | WarnScope.On m' :: _ ->
                if langVersion.SupportsFeature LanguageFeature.ScopedNowarn then
                    informationalWarning (Error(FSComp.SR.lexWarnDirectivesMustMatch ("#nowarn", m'.StartLine), m))

                warnScopeMap
            | scopes -> warnScopeMap.Add(idx, WarnScope.OpenOff(mkScope m m) :: scopes)
        | WarnCmd.Warnon(n, m) ->
            let idx = m.FileIndex, n

            match getScopes idx warnScopeMap with
            | WarnScope.OpenOff m' :: t -> warnScopeMap.Add(idx, WarnScope.Off(mkScope m' m) :: t)
            | WarnScope.OpenOn m' :: _
            | WarnScope.Off m' :: _ ->
                warning (Error(FSComp.SR.lexWarnDirectivesMustMatch ("#warnon", m'.EndLine), m))
                warnScopeMap
            | scopes -> warnScopeMap.Add(idx, WarnScope.OpenOn(mkScope m m) :: scopes)

    let MergeInto (diagnosticOptions: FSharpDiagnosticOptions) (subModuleRanges: range list) (lexbuf: Lexbuf) =
        let data = getTempData lexbuf
        let warnDirectives = List.rev data.WarnDirectives

        let warnCmds =
            if lexbuf.LanguageVersion.SupportsFeature LanguageFeature.ScopedNowarn then
                warnDirectives |> List.collect _.WarnCmds
            else
                let isInSubmodule (warnDirective: WarnDirective) =
                    List.exists (fun mRange -> rangeContainsRange mRange warnDirective.DirectiveRange) subModuleRanges

                let subModuleWarnDirectives, topLevelWarnDirectives =
                    List.partition isInSubmodule warnDirectives

                // Warn about and ignore directives in submodules
                subModuleWarnDirectives
                |> List.iter (fun wd -> warning (Error(FSComp.SR.buildDirectivesInModulesAreIgnored (), wd.DirectiveRange)))

                let topLevelWarnons, topLevelNowarns =
                    List.partition (_.IsWarnon) topLevelWarnDirectives

                // "feature not available in this language version" error for top-level #nowarn
                topLevelWarnons
                |> List.iter (fun wd ->
                    checkLanguageFeatureAndRecover lexbuf.LanguageVersion LanguageFeature.ScopedNowarn wd.DirectiveRange)

                topLevelNowarns |> List.collect _.WarnCmds

        let lexbufWarnScopes =
            warnCmds |> List.fold (processWarnCmd lexbuf.LanguageVersion) Map.empty

        let lexbufLineMap =
            data.LineMap
            |> Map.map (fun _ (oidx, sectionMaps) -> oidx, List.rev sectionMaps)

        lock diagnosticOptions (fun () ->
            let data = getWarnScopeData diagnosticOptions

            let warnScopes =
                Map.fold (fun wss idx ws -> Map.add idx ws wss) data.warnScopes lexbufWarnScopes
            // TODO: check here also for duplicate file map
            let lineMaps =
                Map.fold (fun lms idx oidx -> Map.add idx oidx lms) data.lineMaps lexbufLineMap

            let newWarnScopeData =
                {
                    warnScopes = warnScopes
                    lineMaps = lineMaps
                }

            setWarnScopeData diagnosticOptions newWarnScopeData)

    let getDirectiveRanges (lexbuf: Lexbuf) =
        (getTempData lexbuf).WarnDirectives |> List.rev |> List.map _.DirectiveRange

    let getCommentRanges (lexbuf: Lexbuf) =
        (getTempData lexbuf).WarnDirectives |> List.rev |> List.choose _.CommentRange

    // *************************************
    // Apply the warn scopes after lexing
    // *************************************

    let private originalRange lineMaps (m: range) =
        match Map.tryFind m.FileIndex lineMaps with
        | None -> m
        | Some(originalFileIndex, sectionMaps) ->
            let surr, orig =
                if List.isEmpty sectionMaps || m.StartLine < fst sectionMaps.Head then
                    (1, 1)
                else
                    sectionMaps |> List.skipWhile (fun (s, _) -> m.StartLine < s) |> List.head

            let origStart = mkPos (m.StartLine + orig - surr) m.StartColumn
            let origEnd = mkPos (m.EndLine + orig - surr) m.EndColumn
            mkFileIndexRange originalFileIndex origStart origEnd

    /// true if m1 contains the start of m2 (#line directives can appear in the middle of an error range)
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

        match mo, diagnosticOptions.WarnScopesFeatureIsSupported with
        | Some m, true ->
            let mOrig = originalRange data.lineMaps m
            let scopes = getScopes (mOrig.FileIndex, warningNumber) data.warnScopes
            List.exists (isEnclosingWarnonScope mOrig) scopes
        | _ -> false

    let IsNowarn (diagnosticOptions: FSharpDiagnosticOptions) warningNumber (mo: range option) =
        let data = getWarnScopeData diagnosticOptions

        match mo with
        | Some m ->
            let mOrig = originalRange data.lineMaps m
            let scopes = getScopes (mOrig.FileIndex, warningNumber) data.warnScopes
            List.exists (isEnclosingNowarnScope mOrig) scopes
        | None -> data.warnScopes |> Map.exists (fun idx _ -> snd idx = warningNumber)
