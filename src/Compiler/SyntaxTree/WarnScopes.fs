// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open FSharp.Compiler.UnicodeLexing
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Diagnostics
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
module internal WarnScopes =

    // *************************************
    // Collect the warn scopes during lexing
    // *************************************

    // The key into the BufferLocalStore used to hold the warn scopes
    let private warnScopeKey = "WarnScopes"

    let private fromLexbuf (lexbuf: Lexbuf) : WarnScopeMap =
        if not <| lexbuf.BufferLocalStore.ContainsKey warnScopeKey then
            lexbuf.BufferLocalStore.Add(warnScopeKey, WarnScopeMap Map.empty)

        lexbuf.BufferLocalStore[warnScopeKey] :?> WarnScopeMap

    [<RequireQualifiedAccess>]
    type private WarnDirective =
        | Nowarn of int * range
        | Warnon of int * range

    let private getWarningNumber (s: string) =
        let s =
            if s.StartsWith "\"" && s.EndsWith "\"" then
                s.Substring(1, s.Length - 2)
            else
                s

        let s = if s.StartsWith "FS" then s[2..] else s

        match System.Int32.TryParse s with
        | true, i -> Some i
        | false, _ -> None

    let private regex =
        Regex(" *#(nowarn|warnon)(?: +([^ \r\n]+))*\r?\n", RegexOptions.Compiled ||| RegexOptions.CultureInvariant)

    let private getDirectives text m =
        let mkDirective (directiveId: string) (m: range) (c: Capture) =
            let argRange () =
                Range.withEnd (mkPos m.StartLine (c.Index + c.Length - 1)) (Range.shiftStart 0 c.Index m)

            match directiveId, getWarningNumber c.Value with
            | "nowarn", Some n -> Some(WarnDirective.Nowarn(n, argRange ()))
            | "warnon", Some n -> Some(WarnDirective.Warnon(n, argRange ()))
            | _ -> None

        let mGroups = (regex.Match text).Groups
        let dIdent = mGroups[1].Value
        [ for c in mGroups[2].Captures -> c ] |> List.choose (mkDirective dIdent m)

    let private index (fileIndex, warningNumber) =
        (int64 fileIndex <<< 32) + int64 warningNumber

    let private warnNumFromIndex (idx: int64) = idx &&& 0xFFFFFFFFL

    let private getScopes idx warnScopes =
        Map.tryFind idx warnScopes |> Option.defaultValue []

    let private mkScope (m1: range) (m2: range) =
        mkFileIndexRange m1.FileIndex m1.Start m2.End

    let private processWarnDirective (WarnScopeMap warnScopes) (wd: WarnDirective) =
        match wd with
        | WarnDirective.Nowarn(n, m) ->
            let idx = index (m.FileIndex, n)

            match getScopes idx warnScopes with
            | WarnScope.OpenOn m' :: t -> warnScopes.Add(idx, WarnScope.On(mkScope m' m) :: t)
            | WarnScope.OpenOff _ :: _ -> warnScopes
            | scopes -> warnScopes.Add(idx, WarnScope.OpenOff(mkScope m m) :: scopes)
        | WarnDirective.Warnon(n, m) ->
            let idx = index (m.FileIndex, n)

            match getScopes idx warnScopes with
            | WarnScope.OpenOff m' :: t -> warnScopes.Add(idx, WarnScope.Off(mkScope m' m) :: t)
            | WarnScope.OpenOn _ :: _ -> warnScopes
            | scopes -> warnScopes.Add(idx, WarnScope.OpenOn(mkScope m m) :: scopes)
        |> WarnScopeMap

    let ParseAndSaveWarnDirectiveLine (lexbuf: Lexbuf) =
        let convert (p: Internal.Utilities.Text.Lexing.Position) = mkPos p.Line p.Column
        let idx = lexbuf.StartPos.FileIndex
        let idx = FileIndex.tryGetLineMappingOrigin idx |> Option.defaultValue idx

        let m = mkFileIndexRange idx (convert lexbuf.StartPos) (convert lexbuf.EndPos)

        let text = Lexbuf.LexemeString lexbuf
        let directives = getDirectives text m
        let warnScopes = (fromLexbuf lexbuf, directives) ||> List.fold processWarnDirective
        lexbuf.BufferLocalStore[warnScopeKey] <- warnScopes

    let ClearLexbufStore (lexbuf: Lexbuf) =
        lexbuf.BufferLocalStore.Remove warnScopeKey |> ignore

    let MergeInto (diagnosticOptions: FSharpDiagnosticOptions) (lexbuf: Lexbuf) =
        let (WarnScopeMap warnScopes) = fromLexbuf lexbuf

        lock diagnosticOptions (fun () ->
            let (WarnScopeMap current) = diagnosticOptions.WarnScopes
            let warnScopes' = Map.fold (fun wss idx ws -> Map.add idx ws wss) current warnScopes
            diagnosticOptions.WarnScopes <- WarnScopeMap warnScopes')

    // *************************************
    // Apply the warn scopes after lexing
    // *************************************

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

    let private isOffScope scope =
        match scope with
        | WarnScope.Off _
        | WarnScope.OpenOff _ -> true
        | _ -> false

    let IsWarnon (WarnScopeMap warnScopes) warningNumber (mo: range option) =
        match mo with
        | None -> false
        | Some m ->
            if FileIndex.hasLineMapping m.FileIndex then
                false
            else
                let scopes = getScopes (index (m.FileIndex, warningNumber)) warnScopes
                List.exists (isEnclosingWarnonScope m) scopes

    let IsNowarn (WarnScopeMap warnScopes) warningNumber (mo: range option) compatible =
        match mo, compatible with
        | Some m, false ->
            match FileIndex.tryGetLineMappingOrigin m.FileIndex with
            | None ->
                let scopes = getScopes (index (m.FileIndex, warningNumber)) warnScopes
                List.exists (isEnclosingNowarnScope m) scopes
            | Some fileIndex ->  // file has #line directives
                let scopes = getScopes (index (fileIndex, warningNumber)) warnScopes
                List.exists isOffScope scopes
        | _ -> warnScopes |> Map.exists (fun idx _ -> warnNumFromIndex idx = warningNumber)
