// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LexerStore

open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.UnicodeLexing
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml

//------------------------------------------------------------------------
// Lexbuf.BufferLocalStore is used during lexing/parsing of a file for different purposes.
// All access happens through the functions and modules below.
//------------------------------------------------------------------------

let private getStoreData<'T when 'T: not null> (lexbuf: Lexbuf) key (getInitialData: unit -> 'T) =
    let store = lexbuf.BufferLocalStore

    match store.TryGetValue key with
    | true, data -> data :?> 'T
    | _ ->
        let data = getInitialData ()
        store[key] <- data
        data

let private tryGetStoreData<'T when 'T: not null> (lexbuf: Lexbuf) key =
    let store = lexbuf.BufferLocalStore

    match store.TryGetValue key with
    | true, data -> Some(data :?> 'T)
    | _ -> None

let private setStoreData (lexbuf: Lexbuf) key data = lexbuf.BufferLocalStore[key] <- data

//------------------------------------------------------------------------
// A SynArgNameGenerator for the current file, used by the parser
//------------------------------------------------------------------------

let getSynArgNameGenerator (lexbuf: Lexbuf) =
    getStoreData lexbuf "SynArgNameGenerator" SynArgNameGenerator

//------------------------------------------------------------------------
// A XmlDocCollector, used to hold the current accumulated Xml doc lines, and related access functions
//------------------------------------------------------------------------

[<RequireQualifiedAccess>]
module XmlDocStore =
    let private xmlDocKey = "XmlDoc"

    let private getCollector (lexbuf: Lexbuf) =
        getStoreData lexbuf xmlDocKey XmlDocCollector

    /// Called from the lexer to save a single line of XML doc comment.
    let SaveXmlDocLine (lexbuf: Lexbuf, lineText, range: range) =
        let collector = getCollector lexbuf
        collector.AddXmlDocLine(lineText, range)

    let AddGrabPoint (lexbuf: Lexbuf) =
        let collector = getCollector lexbuf
        let startPos = lexbuf.StartPos
        collector.AddGrabPoint(mkPos startPos.Line startPos.Column)

    /// Allowed cases when there are comments after XmlDoc
    ///
    ///    /// X xmlDoc
    ///    // comment
    ///    //// comment
    ///    (* multiline comment *)
    ///    let x = ...        // X xmlDoc
    ///
    /// Remember the first position when a comment (//, (* *), ////) is encountered after the XmlDoc block
    /// then add a grab point if a new XmlDoc block follows the comments
    let AddGrabPointDelayed (lexbuf: Lexbuf) =
        let collector = getCollector lexbuf
        let startPos = lexbuf.StartPos
        collector.AddGrabPointDelayed(mkPos startPos.Line startPos.Column)

    /// Called from the parser each time we parse a construct that marks the end of an XML doc comment range,
    /// e.g. a 'type' declaration. The markerRange is the range of the keyword that delimits the construct.
    let GrabXmlDocBeforeMarker (lexbuf: Lexbuf, markerRange: range) =
        match tryGetStoreData lexbuf xmlDocKey with
        | Some collector -> PreXmlDoc.CreateFromGrabPoint(collector, markerRange.Start)
        | _ -> PreXmlDoc.Empty

    let ReportInvalidXmlDocPositions (lexbuf: Lexbuf) =
        let collector = getCollector lexbuf
        collector.CheckInvalidXmlDocPositions()

//------------------------------------------------------------------------
// Storage to hold the current accumulated ConditionalDirectiveTrivia, and related types and access functions
//------------------------------------------------------------------------

type LexerIfdefExpression =
    | IfdefAnd of LexerIfdefExpression * LexerIfdefExpression
    | IfdefOr of LexerIfdefExpression * LexerIfdefExpression
    | IfdefNot of LexerIfdefExpression
    | IfdefId of string

let rec LexerIfdefEval (lookup: string -> bool) =
    function
    | IfdefAnd(l, r) -> (LexerIfdefEval lookup l) && (LexerIfdefEval lookup r)
    | IfdefOr(l, r) -> (LexerIfdefEval lookup l) || (LexerIfdefEval lookup r)
    | IfdefNot e -> not (LexerIfdefEval lookup e)
    | IfdefId id -> lookup id

[<RequireQualifiedAccess>]
module IfdefStore =
    let private getStore (lexbuf: Lexbuf) =
        getStoreData lexbuf "Ifdef" ResizeArray<ConditionalDirectiveTrivia>

    let private mkRangeWithoutLeadingWhitespace (lexed: string) (m: range) : range =
        let startColumn = lexed.Length - lexed.TrimStart().Length
        mkFileIndexRange m.FileIndex (mkPos m.StartLine startColumn) m.End

    let SaveIfHash (lexbuf: Lexbuf, lexed: string, expr: LexerIfdefExpression, range: range) =
        let store = getStore lexbuf

        let expr =
            let rec visit (expr: LexerIfdefExpression) : IfDirectiveExpression =
                match expr with
                | LexerIfdefExpression.IfdefAnd(l, r) -> IfDirectiveExpression.And(visit l, visit r)
                | LexerIfdefExpression.IfdefOr(l, r) -> IfDirectiveExpression.Or(visit l, visit r)
                | LexerIfdefExpression.IfdefNot e -> IfDirectiveExpression.Not(visit e)
                | LexerIfdefExpression.IfdefId id -> IfDirectiveExpression.Ident id

            visit expr

        let m = mkRangeWithoutLeadingWhitespace lexed range

        store.Add(ConditionalDirectiveTrivia.If(expr, m))

    let SaveElseHash (lexbuf: Lexbuf, lexed: string, range: range) =
        let store = getStore lexbuf
        let m = mkRangeWithoutLeadingWhitespace lexed range
        store.Add(ConditionalDirectiveTrivia.Else(m))

    let SaveEndIfHash (lexbuf: Lexbuf, lexed: string, range: range) =
        let store = getStore lexbuf
        let m = mkRangeWithoutLeadingWhitespace lexed range
        store.Add(ConditionalDirectiveTrivia.EndIf(m))

    let GetTrivia (lexbuf: Lexbuf) : ConditionalDirectiveTrivia list =
        let store = getStore lexbuf
        Seq.toList store

//------------------------------------------------------------------------
// Storage to hold the current accumulated CommentTrivia, and related access functions
//------------------------------------------------------------------------

[<RequireQualifiedAccess>]
module CommentStore =
    let private getStore (lexbuf: Lexbuf) =
        getStoreData lexbuf "Comments" ResizeArray<CommentTrivia>

    let SaveSingleLineComment (lexbuf: Lexbuf, startRange: range, endRange: range) =
        let store = getStore lexbuf
        let m = unionRanges startRange endRange
        store.Add(CommentTrivia.LineComment(m))

    let SaveBlockComment (lexbuf: Lexbuf, startRange: range, endRange: range) =
        let store = getStore lexbuf
        let m = unionRanges startRange endRange
        store.Add(CommentTrivia.BlockComment(m))

    let GetComments (lexbuf: Lexbuf) : CommentTrivia list =
        let store = getStore lexbuf
        Seq.toList store
