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
            WarnCmds: WarnCmd list
            DirectiveRange: range
        }

    let private isWarnonDirective (w: WarnDirective) =
        match w.WarnCmds with
        | [] -> false
        | h :: _ -> h.IsWarnon

    type private LexbufData =
        {
            mutable WarnDirectives: WarnDirective list
        }

    let private getInitialData () = { WarnDirectives = [] }

    let private getLexbufData (lexbuf: Lexbuf) =
        lexbuf.GetLocalData("WarnScopeData", getInitialData)

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

    let private parseDirective lexbuf =
        let text = Lexbuf.LexemeString lexbuf
        let startPos = lexbuf.StartPos

        let mGroups = (regex.Match text).Groups
        let totalLength = mGroups[0].Length
        let dIdent = mGroups[2].Value
        let argCaptures = [ for c in mGroups[3].Captures -> c ]

        let positions line offset length =
            mkPos line (startPos.Column + offset), mkPos line (startPos.Column + offset + length)

        let mkRange offset length =
            positions lexbuf.StartPos.Line offset length
            ||> mkFileIndexRange startPos.FileIndex

        let directiveRange = mkRange 0 totalLength

        if argCaptures.IsEmpty then
            errorR (Error(FSComp.SR.lexWarnDirectiveMustHaveArgs (), directiveRange))

        let mkDirective ctor (c: Capture) =
            let m = mkRange c.Index c.Length
            getNumber lexbuf.LanguageVersion m c.Value |> Option.map (fun n -> ctor (n, m))

        let warnCmds =
            match dIdent with
            | "warnon" -> argCaptures |> List.choose (mkDirective WarnCmd.Warnon)
            | "nowarn" -> argCaptures |> List.choose (mkDirective WarnCmd.Nowarn)
            | _ -> // like "warnonx"
                errorR (Error(FSComp.SR.fsiInvalidDirective ($"#{dIdent}", ""), directiveRange))
                []

        {
            DirectiveRange = directiveRange
            WarnCmds = warnCmds
        }

    let ParseAndRegisterWarnDirective (lexbuf: Lexbuf) =
        let data = getLexbufData lexbuf
        let warnDirective = parseDirective lexbuf
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

    type private WarnScopeData =
        {
            ScopedNowarnFeatureIsSupported: bool
            ScriptNowarns: WarningNumber list // only needed to avoid breaking changes for previous language versions
            WarnScopes: Map<FileIndex, Map<WarningNumber, WarnScope list>>
        }

    let private getWarnScopeData (diagnosticOptions: FSharpDiagnosticOptions) =
        match diagnosticOptions.WarnScopeData with
        | None ->
            {
                ScopedNowarnFeatureIsSupported = true
                ScriptNowarns = []
                WarnScopes = Map.empty
            }
        | Some data -> data :?> WarnScopeData

    let private setWarnScopeData (diagnosticOptions: FSharpDiagnosticOptions) data =
        diagnosticOptions.WarnScopeData <- Some data

    // *************************************
    // Create the warn scopes from the directives and store them in diagnosticOptions.
    // *************************************

    let MergeInto diagnosticOptions isScript subModuleRanges lexbuf =
        let lexbufData = getLexbufData lexbuf
        let fileIndex = lexbuf.StartPos.FileIndex

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
                assert (m1.FileIndex = m2.FileIndex)
                mkFileIndexRange m1.FileIndex m1.Start m2.End // LineDirectives: n/a

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

        let merge () =
            let projectData = getWarnScopeData diagnosticOptions

            // If the same file is parsed again (same fileIndex), we replace the warn scopes.
            let projectWarnScopes = projectData.WarnScopes.Add(fileIndex, fileWarnScopes)

            let newWarnScopeData =
                {
                    ScopedNowarnFeatureIsSupported = scopedNowarnFeatureIsSupported
                    ScriptNowarns = List.distinct (projectData.ScriptNowarns @ fileScriptNowarns)
                    WarnScopes = projectWarnScopes
                }

            setWarnScopeData diagnosticOptions newWarnScopeData

        lock diagnosticOptions merge

    let getDirectiveTrivia (lexbuf: Lexbuf) =
        let mkTrivia d =
            if isWarnonDirective d then
                WarnDirectiveTrivia.Warnon d.DirectiveRange
            else
                WarnDirectiveTrivia.Nowarn d.DirectiveRange

        (getLexbufData lexbuf).WarnDirectives |> List.rev |> List.map mkTrivia

    // *************************************
    // Apply the warn scopes after lexing
    // *************************************

    let private getScopes fileIndex warningNumber warnScopes =
        Map.tryFind fileIndex warnScopes
        |> Option.bind (Map.tryFind warningNumber)
        |> Option.defaultValue []

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
            let scopes = getScopes m.FileIndex warningNumber data.WarnScopes // LineDirectives: n/a
            List.exists (isEnclosingWarnonScope m) scopes
        | _ -> false

    let IsNowarn (diagnosticOptions: FSharpDiagnosticOptions) warningNumber (mo: range option) =
        let data = getWarnScopeData diagnosticOptions

        if List.contains warningNumber data.ScriptNowarns then // this happens only for legacy language versions
            true
        else
            match mo with
            | Some m ->
                let scopes = getScopes m.FileIndex warningNumber data.WarnScopes // LineDirectives: n/a
                List.exists (isEnclosingNowarnScope m) scopes
            | None -> false
