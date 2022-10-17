// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ParseHelpers

open FSharp.Compiler.AbstractIL
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.UnicodeLexing
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml
open Internal.Utilities.Library
open Internal.Utilities.Text.Lexing
open Internal.Utilities.Text.Parsing

//------------------------------------------------------------------------
// Parsing: Error recovery exception for fsyacc
//------------------------------------------------------------------------

/// The error raised by the parse_error_rich function, which is called by the parser engine
/// when a syntax error occurs. The first object is the ParseErrorContext which contains a dump of
/// information about the grammar at the point where the error occurred, e.g. what tokens
/// are valid to shift next at that point in the grammar. This information is processed in CompileOps.fs.
[<NoEquality; NoComparison>]
exception SyntaxError of obj (* ParseErrorContext<_> *) * range: range

exception IndentationProblem of string * range

let warningStringOfCoords line column = sprintf "(%d:%d)" line (column + 1)

let warningStringOfPos (p: pos) = warningStringOfCoords p.Line p.Column

//------------------------------------------------------------------------
// Parsing: getting positions from the lexer
//------------------------------------------------------------------------

/// Get an F# compiler position from a lexer position
let posOfLexPosition (p: Position) = mkPos p.Line p.Column

/// Get an F# compiler range from a lexer range
let mkSynRange (p1: Position) (p2: Position) =
    mkFileIndexRange p1.FileIndex (posOfLexPosition p1) (posOfLexPosition p2)

type LexBuffer<'Char> with

    member lexbuf.LexemeRange = mkSynRange lexbuf.StartPos lexbuf.EndPos

/// Get the range corresponding to the result of a grammar rule while it is being reduced
let lhs (parseState: IParseState) =
    let p1 = parseState.ResultStartPosition
    let p2 = parseState.ResultEndPosition
    mkSynRange p1 p2

/// Get the range covering two of the r.h.s. symbols of a grammar rule while it is being reduced
let rhs2 (parseState: IParseState) i j =
    let p1 = parseState.InputStartPosition i
    let p2 = parseState.InputEndPosition j
    mkSynRange p1 p2

/// Get the range corresponding to one of the r.h.s. symbols of a grammar rule while it is being reduced
let rhs parseState i = rhs2 parseState i i

type IParseState with

    /// Get the generator used for compiler-generated argument names.
    member x.SynArgNameGenerator =
        let key = "SynArgNameGenerator"
        let bls = x.LexBuffer.BufferLocalStore

        let gen =
            match bls.TryGetValue key with
            | true, gen -> gen
            | _ ->
                let gen = box (SynArgNameGenerator())
                bls[key] <- gen
                gen

        gen :?> SynArgNameGenerator

    /// Reset the generator used for compiler-generated argument names.
    member x.ResetSynArgNameGenerator() = x.SynArgNameGenerator.Reset()

//------------------------------------------------------------------------
// Parsing: grabbing XmlDoc
//------------------------------------------------------------------------

/// XmlDoc F# lexer/parser state, held in the BufferLocalStore for the lexer.
module LexbufLocalXmlDocStore =
    // The key into the BufferLocalStore used to hold the current accumulated XmlDoc lines
    let private xmlDocKey = "XmlDoc"

    let private getCollector (lexbuf: Lexbuf) =
        match lexbuf.BufferLocalStore.TryGetValue xmlDocKey with
        | true, collector -> collector
        | _ ->
            let collector = box (XmlDocCollector())
            lexbuf.BufferLocalStore[ xmlDocKey ] <- collector
            collector

        |> unbox<XmlDocCollector>

    let ClearXmlDoc (lexbuf: Lexbuf) =
        lexbuf.BufferLocalStore[ xmlDocKey ] <- box (XmlDocCollector())

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
        match lexbuf.BufferLocalStore.TryGetValue xmlDocKey with
        | true, collector ->
            let collector = unbox<XmlDocCollector> (collector)
            PreXmlDoc.CreateFromGrabPoint(collector, markerRange.Start)
        | _ -> PreXmlDoc.Empty

    let ReportInvalidXmlDocPositions (lexbuf: Lexbuf) =
        let collector = getCollector lexbuf
        collector.CheckInvalidXmlDocPositions()

//------------------------------------------------------------------------
// Parsing/lexing: status of #if/#endif processing in lexing, used for continutations
// for whitespace tokens in parser specification.
//------------------------------------------------------------------------

type LexerIfdefStackEntry =
    | IfDefIf
    | IfDefElse

/// Represents the active #if/#else blocks
type LexerIfdefStackEntries = (LexerIfdefStackEntry * range) list

type LexerIfdefStack = LexerIfdefStackEntries

/// Specifies how the 'endline' function in the lexer should continue after
/// it reaches end of line or eof. The options are to continue with 'token' function
/// or to continue with 'skip' function.
type LexerEndlineContinuation =
    | Token
    | Skip of int * range: range

type LexerIfdefExpression =
    | IfdefAnd of LexerIfdefExpression * LexerIfdefExpression
    | IfdefOr of LexerIfdefExpression * LexerIfdefExpression
    | IfdefNot of LexerIfdefExpression
    | IfdefId of string

let rec LexerIfdefEval (lookup: string -> bool) =
    function
    | IfdefAnd (l, r) -> (LexerIfdefEval lookup l) && (LexerIfdefEval lookup r)
    | IfdefOr (l, r) -> (LexerIfdefEval lookup l) || (LexerIfdefEval lookup r)
    | IfdefNot e -> not (LexerIfdefEval lookup e)
    | IfdefId id -> lookup id

/// Ifdef F# lexer/parser state, held in the BufferLocalStore for the lexer.
/// Used to capture #if, #else and #endif as syntax trivia.
module LexbufIfdefStore =
    // The key into the BufferLocalStore used to hold the compiler directives
    let private ifDefKey = "Ifdef"

    let private getStore (lexbuf: Lexbuf) : ResizeArray<ConditionalDirectiveTrivia> =
        match lexbuf.BufferLocalStore.TryGetValue ifDefKey with
        | true, store -> store
        | _ ->
            let store = box (ResizeArray<ConditionalDirectiveTrivia>())
            lexbuf.BufferLocalStore[ ifDefKey ] <- store
            store
        |> unbox<ResizeArray<ConditionalDirectiveTrivia>>

    let private mkRangeWithoutLeadingWhitespace (lexed: string) (m: range) : range =
        let startColumn = lexed.Length - lexed.TrimStart().Length
        mkFileIndexRange m.FileIndex (mkPos m.StartLine startColumn) m.End

    let SaveIfHash (lexbuf: Lexbuf, lexed: string, expr: LexerIfdefExpression, range: range) =
        let store = getStore lexbuf

        let expr =
            let rec visit (expr: LexerIfdefExpression) : IfDirectiveExpression =
                match expr with
                | LexerIfdefExpression.IfdefAnd (l, r) -> IfDirectiveExpression.And(visit l, visit r)
                | LexerIfdefExpression.IfdefOr (l, r) -> IfDirectiveExpression.Or(visit l, visit r)
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

/// Used to capture the ranges of code comments as syntax trivia
module LexbufCommentStore =
    // The key into the BufferLocalStore used to hold the compiler directives
    let private commentKey = "Comments"

    let private getStore (lexbuf: Lexbuf) : ResizeArray<CommentTrivia> =
        match lexbuf.BufferLocalStore.TryGetValue commentKey with
        | true, store -> store
        | _ ->
            let store = box (ResizeArray<CommentTrivia>())
            lexbuf.BufferLocalStore[ commentKey ] <- store
            store
        |> unbox<ResizeArray<CommentTrivia>>

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

    let ClearComments (lexbuf: Lexbuf) : unit =
        lexbuf.BufferLocalStore.Remove(commentKey) |> ignore

//------------------------------------------------------------------------
// Parsing: continuations for whitespace tokens
//------------------------------------------------------------------------

[<RequireQualifiedAccess>]
type LexerStringStyle =
    | Verbatim
    | TripleQuote
    | SingleQuote

[<RequireQualifiedAccess; Struct>]
type LexerStringKind =
    {
        IsByteString: bool
        IsInterpolated: bool
        IsInterpolatedFirst: bool
    }

    static member String =
        {
            IsByteString = false
            IsInterpolated = false
            IsInterpolatedFirst = false
        }

    static member ByteString =
        {
            IsByteString = true
            IsInterpolated = false
            IsInterpolatedFirst = false
        }

    static member InterpolatedStringFirst =
        {
            IsByteString = false
            IsInterpolated = true
            IsInterpolatedFirst = true
        }

    static member InterpolatedStringPart =
        {
            IsByteString = false
            IsInterpolated = true
            IsInterpolatedFirst = false
        }

/// Represents the degree of nesting of '{..}' and the style of the string to continue afterwards, in an interpolation fill.
/// Nesting counters and styles of outer interpolating strings are pushed on this stack.
type LexerInterpolatedStringNesting = (int * LexerStringStyle * range) list

/// The parser defines a number of tokens for whitespace and
/// comments eliminated by the lexer.  These carry a specification of
/// a continuation for the lexer for continued processing after we've dealt with
/// the whitespace.
[<RequireQualifiedAccess>]
[<NoComparison; NoEquality>]
type LexerContinuation =
    | Token of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting
    | IfDefSkip of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: range
    | String of
        ifdef: LexerIfdefStackEntries *
        nesting: LexerInterpolatedStringNesting *
        style: LexerStringStyle *
        kind: LexerStringKind *
        range: range
    | Comment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: range
    | SingleLineComment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: range
    | StringInComment of
        ifdef: LexerIfdefStackEntries *
        nesting: LexerInterpolatedStringNesting *
        style: LexerStringStyle *
        int *
        range: range
    | MLOnly of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * range: range
    | EndLine of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * LexerEndlineContinuation

    static member Default = LexCont.Token([], [])

    member x.LexerIfdefStack =
        match x with
        | LexCont.Token (ifdef = ifd)
        | LexCont.IfDefSkip (ifdef = ifd)
        | LexCont.String (ifdef = ifd)
        | LexCont.Comment (ifdef = ifd)
        | LexCont.SingleLineComment (ifdef = ifd)
        | LexCont.StringInComment (ifdef = ifd)
        | LexCont.EndLine (ifdef = ifd)
        | LexCont.MLOnly (ifdef = ifd) -> ifd

    member x.LexerInterpStringNesting =
        match x with
        | LexCont.Token (nesting = nesting)
        | LexCont.IfDefSkip (nesting = nesting)
        | LexCont.String (nesting = nesting)
        | LexCont.Comment (nesting = nesting)
        | LexCont.SingleLineComment (nesting = nesting)
        | LexCont.StringInComment (nesting = nesting)
        | LexCont.EndLine (nesting = nesting)
        | LexCont.MLOnly (nesting = nesting) -> nesting

and LexCont = LexerContinuation

//------------------------------------------------------------------------
// Parse IL assembly code
//------------------------------------------------------------------------

let ParseAssemblyCodeInstructions s reportLibraryOnlyFeatures langVersion m : IL.ILInstr[] =
#if NO_INLINE_IL_PARSER
    ignore s
    ignore isFeatureSupported

    errorR (Error((193, "Inline IL not valid in a hosted environment"), m))
    [||]
#else
    try
        AsciiParser.ilInstrs AsciiLexer.token (StringAsLexbuf(reportLibraryOnlyFeatures, langVersion, s))
    with _ ->
        errorR (Error(FSComp.SR.astParseEmbeddedILError (), m))
        [||]
#endif

let ParseAssemblyCodeType s reportLibraryOnlyFeatures langVersion m =
    ignore s

#if NO_INLINE_IL_PARSER
    errorR (Error((193, "Inline IL not valid in a hosted environment"), m))
    IL.PrimaryAssemblyILGlobals.typ_Object
#else
    try
        AsciiParser.ilType AsciiLexer.token (StringAsLexbuf(reportLibraryOnlyFeatures, langVersion, s))
    with RecoverableParseError ->
        errorR (Error(FSComp.SR.astParseEmbeddedILTypeError (), m))
        IL.PrimaryAssemblyILGlobals.typ_Object
#endif

let grabXmlDocAtRangeStart (parseState: IParseState, optAttributes: SynAttributeList list, range: range) =
    let grabPoint =
        match optAttributes with
        | [] -> range
        | h :: _ -> h.Range

    LexbufLocalXmlDocStore.GrabXmlDocBeforeMarker(parseState.LexBuffer, grabPoint)

let grabXmlDoc (parseState: IParseState, optAttributes: SynAttributeList list, elemIdx) =
    grabXmlDocAtRangeStart (parseState, optAttributes, rhs parseState elemIdx)

let reportParseErrorAt m s = errorR (Error(s, m))

let raiseParseErrorAt m s =
    reportParseErrorAt m s
    // This initiates error recovery
    raise RecoverableParseError

let (|GetIdent|SetIdent|OtherIdent|) (ident: Ident option) =
    match ident with
    | Some ident when ident.idText = "get" -> GetIdent ident.idRange
    | Some ident when ident.idText = "set" -> SetIdent ident.idRange
    | _ -> OtherIdent

let mkSynMemberDefnGetSet
    (parseState: IParseState)
    (opt_inline: bool)
    (mWith: range)
    (classDefnMemberGetSetElements: (bool * SynAttributeList list * (SynPat * range) * (range option * SynReturnInfo) option * range option * SynExpr * range) list)
    (mAnd: range option)
    (mWhole: range)
    (propertyNameBindingPat: SynPat)
    (optPropertyType: (range option * SynReturnInfo) option)
    (visNoLongerUsed: SynAccess option)
    flagsBuilderAndLeadingKeyword
    (attrs: SynAttributeList list)
    (rangeStart: range)
    : SynMemberDefn list =
    let isMutable = false
    let mutable hasGet = false
    let mutable hasSet = false

    let memFlagsBuilder, leadingKeyword = flagsBuilderAndLeadingKeyword
    let xmlDoc = grabXmlDocAtRangeStart (parseState, attrs, rangeStart)

    let tryMkSynMemberDefnMember
        (
            optInline,
            optAttrs: SynAttributeList list,
            (bindingPat, mBindLhs),
            optReturnType,
            mEquals,
            expr,
            mExpr
        ) : (SynMemberDefn * Ident option) option =
        let optInline = opt_inline || optInline
        // optional attributes are only applied to getters and setters
        // the "top level" attrs will be applied to both
        let optAttrs =
            optAttrs
            |> List.map (fun attrList ->
                { attrList with
                    Attributes =
                        attrList.Attributes
                        |> List.map (fun a ->
                            { a with
                                AppliesToGetterAndSetter = true
                            })
                })

        let attrs = attrs @ optAttrs

        let trivia: SynBindingTrivia =
            {
                LeadingKeyword = leadingKeyword
                EqualsRange = mEquals
            }

        let binding =
            mkSynBinding
                (xmlDoc, bindingPat)
                (visNoLongerUsed,
                 optInline,
                 isMutable,
                 mBindLhs,
                 DebugPointAtBinding.NoneAtInvisible,
                 optReturnType,
                 expr,
                 mExpr,
                 [],
                 attrs,
                 Some(memFlagsBuilder SynMemberKind.Member),
                 trivia)

        let (SynBinding (accessibility = vis; isInline = isInline; attributes = attrs; headPat = pv; range = mBindLhs)) =
            binding

        let memberKind =
            let getset =
                let rec go p =
                    match p with
                    | SynPat.LongIdent(longDotId = SynLongIdent ([ id ], _, _)) -> id.idText
                    | SynPat.Named (SynIdent (nm, _), _, _, _)
                    | SynPat.As (_, SynPat.Named (SynIdent (nm, _), _, _, _), _) -> nm.idText
                    | SynPat.Typed (p, _, _) -> go p
                    | SynPat.Attrib (p, _, _) -> go p
                    | _ -> raiseParseErrorAt mBindLhs (FSComp.SR.parsInvalidDeclarationSyntax ())

                go pv

            if getset = "get" then
                if hasGet then
                    reportParseErrorAt mBindLhs (FSComp.SR.parsGetAndOrSetRequired ())
                    None
                else
                    hasGet <- true
                    Some SynMemberKind.PropertyGet
            else if getset = "set" then
                if hasSet then
                    reportParseErrorAt mBindLhs (FSComp.SR.parsGetAndOrSetRequired ())
                    None
                else
                    hasSet <- true
                    Some SynMemberKind.PropertySet
            else
                raiseParseErrorAt mBindLhs (FSComp.SR.parsGetAndOrSetRequired ())

        match memberKind with
        | None -> None
        | Some memberKind ->

            // REVIEW: It's hard not to ignore the optPropertyType type annotation for 'set' properties. To apply it,
            // we should apply it to the last argument, but at this point we've already pushed the patterns that
            // make up the arguments onto the RHS. So we just always give a warning.

            (match optPropertyType with
             | Some _ -> errorR (Error(FSComp.SR.parsTypeAnnotationsOnGetSet (), mBindLhs))
             | None -> ())

            let optReturnType =
                match (memberKind, optReturnType) with
                | SynMemberKind.PropertySet, _ -> optReturnType
                | _, None -> optPropertyType
                | _ -> optReturnType

            // REDO with the correct member kind
            let binding =
                mkSynBinding
                    (PreXmlDoc.Empty, bindingPat)
                    (vis,
                     isInline,
                     isMutable,
                     mBindLhs,
                     DebugPointAtBinding.NoneAtInvisible,
                     optReturnType,
                     expr,
                     mExpr,
                     [],
                     attrs,
                     Some(memFlagsBuilder memberKind),
                     trivia)

            let (SynBinding (vis, _, isInline, _, attrs, doc, valSynData, pv, rhsRetInfo, rhsExpr, mBindLhs, spBind, trivia)) =
                binding

            let mWholeBindLhs =
                (mBindLhs, attrs)
                ||> unionRangeWithListBy (fun (a: SynAttributeList) -> a.Range)

            let (SynValData (_, valSynInfo, _)) = valSynData

            // Setters have all arguments tupled in their internal TAST form, though they don't appear to be
            // tupled from the syntax
            let memFlags: SynMemberFlags = memFlagsBuilder memberKind

            let valSynInfo =
                let adjustValueArg valueArg =
                    match valueArg with
                    | [ _ ] -> valueArg
                    | _ -> SynInfo.unnamedTopArg

                match memberKind, valSynInfo, memFlags.IsInstance with
                | SynMemberKind.PropertyGet, SynValInfo ([], _ret), false
                | SynMemberKind.PropertyGet, SynValInfo ([ _ ], _ret), true ->
                    raiseParseErrorAt mWholeBindLhs (FSComp.SR.parsGetterMustHaveAtLeastOneArgument ())

                | SynMemberKind.PropertyGet, SynValInfo (thisArg :: indexOrUnitArgs :: rest, ret), true ->
                    if not rest.IsEmpty then
                        reportParseErrorAt mWholeBindLhs (FSComp.SR.parsGetterAtMostOneArgument ())

                    SynValInfo([ thisArg; indexOrUnitArgs ], ret)

                | SynMemberKind.PropertyGet, SynValInfo (indexOrUnitArgs :: rest, ret), false ->
                    if not rest.IsEmpty then
                        reportParseErrorAt mWholeBindLhs (FSComp.SR.parsGetterAtMostOneArgument ())

                    SynValInfo([ indexOrUnitArgs ], ret)

                | SynMemberKind.PropertySet, SynValInfo ([ thisArg; valueArg ], ret), true ->
                    SynValInfo([ thisArg; adjustValueArg valueArg ], ret)

                | SynMemberKind.PropertySet, SynValInfo (thisArg :: indexArgs :: valueArg :: rest, ret), true ->
                    if not rest.IsEmpty then
                        reportParseErrorAt mWholeBindLhs (FSComp.SR.parsSetterAtMostTwoArguments ())

                    SynValInfo([ thisArg; indexArgs @ adjustValueArg valueArg ], ret)

                | SynMemberKind.PropertySet, SynValInfo ([ valueArg ], ret), false -> SynValInfo([ adjustValueArg valueArg ], ret)

                | SynMemberKind.PropertySet, SynValInfo (indexArgs :: valueArg :: rest, ret), _ ->
                    if not rest.IsEmpty then
                        reportParseErrorAt mWholeBindLhs (FSComp.SR.parsSetterAtMostTwoArguments ())

                    SynValInfo([ indexArgs @ adjustValueArg valueArg ], ret)

                | _ ->
                    // should be unreachable, cover just in case
                    raiseParseErrorAt mWholeBindLhs (FSComp.SR.parsInvalidProperty ())

            let valSynData = SynValData(Some(memFlags), valSynInfo, None)

            // Fold together the information from the first lambda pattern and the get/set binding
            // This uses the 'this' variable from the first and the patterns for the get/set binding,
            // replacing the get/set identifier. A little gross.

            let (bindingPatAdjusted, getOrSetIdentOpt), xmlDocAdjusted =
                let bindingOuter =
                    mkSynBinding
                        (xmlDoc, propertyNameBindingPat)
                        (vis,
                         optInline,
                         isMutable,
                         mWholeBindLhs,
                         spBind,
                         optReturnType,
                         expr,
                         mExpr,
                         [],
                         attrs,
                         Some(memFlagsBuilder SynMemberKind.Member),
                         trivia)

                let (SynBinding (_, _, _, _, _, doc2, _, bindingPatOuter, _, _, _, _, _)) =
                    bindingOuter

                let lidOuter, lidVisOuter =
                    match bindingPatOuter with
                    | SynPat.LongIdent (lid, _, None, SynArgPats.Pats [], lidVisOuter, _m) -> lid, lidVisOuter
                    | SynPat.Named (SynIdent (id, _), _, visOuter, _m)
                    | SynPat.As (_, SynPat.Named (SynIdent (id, _), _, visOuter, _m), _) -> SynLongIdent([ id ], [], [ None ]), visOuter
                    | _ -> raiseParseErrorAt mWholeBindLhs (FSComp.SR.parsInvalidDeclarationSyntax ())

                // Merge the visibility from the outer point with the inner point, e.g.
                //    member <VIS1>  this.Size with <VIS2> get ()      = m_size

                let mergeLidVisOuter lidVisInner =
                    match lidVisInner, lidVisOuter with
                    | None, None -> None
                    | Some lidVisInner, None
                    | None, Some lidVisInner -> Some lidVisInner
                    | Some _, Some _ ->
                        errorR (Error(FSComp.SR.parsMultipleAccessibilitiesForGetSet (), mWholeBindLhs))
                        lidVisInner

                // Replace the "get" or the "set" with the right name
                let rec go p =
                    match p with
                    | SynPat.LongIdent (longDotId = SynLongIdent ([ id ], _, _)
                                        typarDecls = tyargs
                                        argPats = SynArgPats.Pats args
                                        accessibility = lidVisInner
                                        range = m) ->
                        // Setters have all arguments tupled in their internal form, though they don't
                        // appear to be tupled from the syntax. Somewhat unfortunate
                        let args =
                            if id.idText = "set" then
                                match args with
                                | [ SynPat.Paren (SynPat.Tuple (false, indexPats, _), indexPatRange); valuePat ] when id.idText = "set" ->
                                    [
                                        SynPat.Tuple(false, indexPats @ [ valuePat ], unionRanges indexPatRange valuePat.Range)
                                    ]
                                | [ indexPat; valuePat ] -> [ SynPat.Tuple(false, args, unionRanges indexPat.Range valuePat.Range) ]
                                | [ valuePat ] -> [ valuePat ]
                                | _ -> raiseParseErrorAt m (FSComp.SR.parsSetSyntax ())
                            else
                                args

                        SynPat.LongIdent(lidOuter, Some id, tyargs, SynArgPats.Pats args, mergeLidVisOuter lidVisInner, m), Some id
                    | SynPat.Named (_, _, lidVisInner, m)
                    | SynPat.As (_, SynPat.Named (_, _, lidVisInner, m), _) ->
                        SynPat.LongIdent(lidOuter, None, None, SynArgPats.Pats [], mergeLidVisOuter lidVisInner, m), None
                    | SynPat.Typed (p, ty, m) ->
                        let p, id = go p
                        SynPat.Typed(p, ty, m), id
                    | SynPat.Attrib (p, attribs, m) ->
                        let p, id = go p
                        SynPat.Attrib(p, attribs, m), id
                    | SynPat.Wild m -> SynPat.Wild(m), None
                    | _ -> raiseParseErrorAt mWholeBindLhs (FSComp.SR.parsInvalidDeclarationSyntax ())

                go pv, PreXmlDoc.Merge doc2 doc

            let binding =
                SynBinding(
                    vis,
                    SynBindingKind.Normal,
                    isInline,
                    isMutable,
                    attrs,
                    xmlDocAdjusted,
                    valSynData,
                    bindingPatAdjusted,
                    rhsRetInfo,
                    rhsExpr,
                    mWholeBindLhs,
                    spBind,
                    trivia
                )

            let memberRange =
                unionRanges rangeStart mWhole |> unionRangeWithXmlDoc xmlDocAdjusted

            Some(SynMemberDefn.Member(binding, memberRange), getOrSetIdentOpt)

    // Iterate over 1 or 2 'get'/'set' entries
    match classDefnMemberGetSetElements with
    | [ h ] ->
        match tryMkSynMemberDefnMember h with
        | Some (memberDefn, getSetIdentOpt) ->
            match memberDefn, getSetIdentOpt with
            | SynMemberDefn.Member _, None -> [ memberDefn ]
            | SynMemberDefn.Member (binding, m), Some getOrSet ->
                if getOrSet.idText = "get" then
                    let trivia =
                        {
                            WithKeyword = mWith
                            GetKeyword = Some getOrSet.idRange
                            AndKeyword = None
                            SetKeyword = None
                        }

                    [ SynMemberDefn.GetSetMember(Some binding, None, m, trivia) ]
                else
                    let trivia =
                        {
                            WithKeyword = mWith
                            GetKeyword = None
                            AndKeyword = None
                            SetKeyword = Some getOrSet.idRange
                        }

                    [ SynMemberDefn.GetSetMember(None, Some binding, m, trivia) ]
            | _ -> []
        | None -> []
    | [ g; s ] ->
        let getter = tryMkSynMemberDefnMember g
        let setter = tryMkSynMemberDefnMember s

        match getter, setter with
        | Some (SynMemberDefn.Member (getBinding, m1), GetIdent mGet), Some (SynMemberDefn.Member (setBinding, m2), SetIdent mSet)
        | Some (SynMemberDefn.Member (setBinding, m1), SetIdent mSet), Some (SynMemberDefn.Member (getBinding, m2), GetIdent mGet) ->
            let range = unionRanges m1 m2

            let trivia =
                {
                    WithKeyword = mWith
                    GetKeyword = Some mGet
                    AndKeyword = mAnd
                    SetKeyword = Some mSet
                }

            [ SynMemberDefn.GetSetMember(Some getBinding, Some setBinding, range, trivia) ]
        | Some (SynMemberDefn.Member (binding, m), getOrSet), None
        | None, Some (SynMemberDefn.Member (binding, m), getOrSet) ->
            let trivia =
                match getOrSet with
                | GetIdent mGet ->
                    {
                        WithKeyword = mWith
                        GetKeyword = Some mGet
                        AndKeyword = mAnd
                        SetKeyword = None
                    }
                | SetIdent mSet ->
                    {
                        WithKeyword = mWith
                        GetKeyword = None
                        AndKeyword = mAnd
                        SetKeyword = Some mSet
                    }
                | OtherIdent ->
                    {
                        WithKeyword = mWith
                        AndKeyword = mAnd
                        GetKeyword = None
                        SetKeyword = None
                    }

            if trivia.GetKeyword.IsSome then
                [ SynMemberDefn.GetSetMember(Some binding, None, m, trivia) ]
            elif trivia.SetKeyword.IsSome then
                [ SynMemberDefn.GetSetMember(None, Some binding, m, trivia) ]
            else
                []
        | _ -> []
    | _ -> []

//  Input Text               Precedence by Parser        Adjustment
//
//  ^T.Ident                 ^(T.Ident)                  (^T).Ident
//  ^T.Ident[idx]            ^(T.Ident[idx])             (^T).Ident[idx]
//  ^T.Ident.[idx]           ^(T.Ident.[idx])            (^T).Ident.[idx]
//  ^T.Ident.Ident2          ^(T.Ident.Ident2)           (^T).Ident.Ident2
//  ^T.Ident(args).Ident3    ^(T.Ident(args).Ident3)     (^T).Ident(args).Ident3
//  ^T.(+)(args)             ^(T.(+)(args))              (^T).(+)(args).Ident3
let adjustHatPrefixToTyparLookup mFull rightExpr =
    let rec take inp =
        match inp with
        | SynExpr.Ident (typarIdent)
        | SynExpr.LongIdent (false, SynLongIdent ([ typarIdent ], _, _), None, _) ->
            let typar = SynTypar(typarIdent, TyparStaticReq.HeadType, false)
            SynExpr.Typar(typar, mFull)
        | SynExpr.LongIdent (false, SynLongIdent ((typarIdent :: items), (dotm :: dots), (_ :: itemTrivias)), None, _) ->
            let typar = SynTypar(typarIdent, TyparStaticReq.HeadType, false)
            let lookup = SynLongIdent(items, dots, itemTrivias)
            SynExpr.DotGet(SynExpr.Typar(typar, mFull), dotm, lookup, mFull)
        | SynExpr.App (isAtomic, false, funcExpr, argExpr, m) ->
            let funcExpr2 = take funcExpr
            SynExpr.App(isAtomic, false, funcExpr2, argExpr, unionRanges funcExpr2.Range m)
        | SynExpr.DotGet (leftExpr, dotm, lookup, m) ->
            let leftExpr2 = take leftExpr
            SynExpr.DotGet(leftExpr2, dotm, lookup, m)
        | SynExpr.DotIndexedGet (leftExpr, indexArg, dotm, m) ->
            let leftExpr2 = take leftExpr
            SynExpr.DotIndexedGet(leftExpr2, indexArg, dotm, m)
        | _ ->
            reportParseErrorAt mFull (FSComp.SR.parsIncompleteTyparExpr2 ())
            arbExpr ("hatExpr1", mFull)

    take rightExpr

// The last element of elementTypes does not have a star or slash
let mkSynTypeTuple (elementTypes: SynTupleTypeSegment list) : SynType =
    let range =
        match elementTypes with
        | [] -> Range.Zero
        | head :: tail ->

            (head.Range, tail)
            ||> List.fold (fun acc segment -> unionRanges acc segment.Range)

    SynType.Tuple(false, elementTypes, range)

#if DEBUG
let debugPrint s =
    if Internal.Utilities.Text.Parsing.Flags.debug then
        printfn "\n%s" s
#else
let debugPrint s = ignore s
#endif

let exprFromParseError (e: SynExpr) = SynExpr.FromParseError(e, e.Range)

let patFromParseError (e: SynPat) = SynPat.FromParseError(e, e.Range)

// record bindings returned by the recdExprBindings rule has shape:
// (binding, separator-before-this-binding)
// this function converts arguments from form
// binding1 (binding2*sep1, binding3*sep2...) sepN
// to form
// binding1*sep1, binding2*sep2
let rebindRanges first fields lastSep =
    let rec run (name, mEquals, value) l acc =
        match l with
        | [] -> List.rev (SynExprRecordField(name, mEquals, value, lastSep) :: acc)
        | (f, m) :: xs -> run f xs (SynExprRecordField(name, mEquals, value, m) :: acc)

    run first fields []

let mkUnderscoreRecdField m =
    SynLongIdent([ ident ("_", m) ], [], [ None ]), false

let mkRecdField (lidwd: SynLongIdent) = lidwd, true

// Used for 'do expr' in a class.
let mkSynDoBinding (vis: SynAccess option, mDo, expr, m) =
    match vis with
    | Some vis -> errorR (Error(FSComp.SR.parsDoCannotHaveVisibilityDeclarations (vis.ToString()), m))
    | None -> ()

    SynBinding(
        None,
        SynBindingKind.Do,
        false,
        false,
        [],
        PreXmlDoc.Empty,
        SynInfo.emptySynValData,
        SynPat.Const(SynConst.Unit, m),
        None,
        expr,
        m,
        DebugPointAtBinding.NoneAtDo,
        {
            LeadingKeyword = SynLeadingKeyword.Do mDo
            EqualsRange = None
        }
    )

let mkSynExprDecl (e: SynExpr) = SynModuleDecl.Expr(e, e.Range)

let addAttribs attrs p = SynPat.Attrib(p, attrs, p.Range)

let unionRangeWithPos (r: range) p =
    let r2 = mkRange r.FileName p p
    unionRanges r r2

/// Report a good error at the end of file, e.g. for non-terminated strings
let checkEndOfFileError t =
    match t with
    | LexCont.IfDefSkip (_, _, _, m) -> reportParseErrorAt m (FSComp.SR.parsEofInHashIf ())

    | LexCont.String (_, _, LexerStringStyle.SingleQuote, kind, m) ->
        if kind.IsInterpolated then
            reportParseErrorAt m (FSComp.SR.parsEofInInterpolatedString ())
        else
            reportParseErrorAt m (FSComp.SR.parsEofInString ())

    | LexCont.String (_, _, LexerStringStyle.TripleQuote, kind, m) ->
        if kind.IsInterpolated then
            reportParseErrorAt m (FSComp.SR.parsEofInInterpolatedTripleQuoteString ())
        else
            reportParseErrorAt m (FSComp.SR.parsEofInTripleQuoteString ())

    | LexCont.String (_, _, LexerStringStyle.Verbatim, kind, m) ->
        if kind.IsInterpolated then
            reportParseErrorAt m (FSComp.SR.parsEofInInterpolatedVerbatimString ())
        else
            reportParseErrorAt m (FSComp.SR.parsEofInVerbatimString ())

    | LexCont.Comment (_, _, _, m) -> reportParseErrorAt m (FSComp.SR.parsEofInComment ())

    | LexCont.SingleLineComment (_, _, _, m) -> reportParseErrorAt m (FSComp.SR.parsEofInComment ())

    | LexCont.StringInComment (_, _, LexerStringStyle.SingleQuote, _, m) -> reportParseErrorAt m (FSComp.SR.parsEofInStringInComment ())

    | LexCont.StringInComment (_, _, LexerStringStyle.Verbatim, _, m) ->
        reportParseErrorAt m (FSComp.SR.parsEofInVerbatimStringInComment ())

    | LexCont.StringInComment (_, _, LexerStringStyle.TripleQuote, _, m) ->
        reportParseErrorAt m (FSComp.SR.parsEofInTripleQuoteStringInComment ())

    | LexCont.MLOnly (_, _, m) -> reportParseErrorAt m (FSComp.SR.parsEofInIfOcaml ())

    | LexCont.EndLine (_, _, LexerEndlineContinuation.Skip (_, m)) -> reportParseErrorAt m (FSComp.SR.parsEofInDirective ())

    | LexCont.EndLine (endifs, nesting, LexerEndlineContinuation.Token)
    | LexCont.Token (endifs, nesting) ->
        match endifs with
        | [] -> ()
        | (_, m) :: _ -> reportParseErrorAt m (FSComp.SR.parsNoHashEndIfFound ())

        match nesting with
        | [] -> ()
        | (_, _, m) :: _ -> reportParseErrorAt m (FSComp.SR.parsEofInInterpolatedStringFill ())

type BindingSet = BindingSetPreAttrs of range * bool * bool * (SynAttributes -> SynAccess option -> SynAttributes * SynBinding list) * range

let mkClassMemberLocalBindings
    (
        isStatic,
        initialRangeOpt,
        attrs,
        vis,
        BindingSetPreAttrs (_, isRec, isUse, declsPreAttrs, bindingSetRange)
    ) =
    let ignoredFreeAttrs, decls = declsPreAttrs attrs vis

    let mWhole =
        match initialRangeOpt with
        | None -> bindingSetRange
        | Some m -> unionRanges m bindingSetRange
        // decls could have a leading attribute
        |> fun m -> (m, decls) ||> unionRangeWithListBy (fun (SynBinding (range = m)) -> m)

    if not (isNil ignoredFreeAttrs) then
        warning (Error(FSComp.SR.parsAttributesIgnored (), mWhole))

    if isUse then
        errorR (Error(FSComp.SR.parsUseBindingsIllegalInImplicitClassConstructors (), mWhole))

    let decls =
        match initialRangeOpt, decls with
        | _, [] -> []
        | Some mStatic, SynBinding (a0, k, il, im, a, x, v, h, ri, e, m, dp, trivia) :: rest ->
            // prepend static keyword to existing leading keyword.
            let trivia =
                match trivia.LeadingKeyword with
                | SynLeadingKeyword.LetRec (mLet, mRec) ->
                    { trivia with
                        LeadingKeyword = SynLeadingKeyword.StaticLetRec(mStatic, mLet, mRec)
                    }
                | SynLeadingKeyword.Let mLet ->
                    { trivia with
                        LeadingKeyword = SynLeadingKeyword.StaticLet(mStatic, mLet)
                    }
                | SynLeadingKeyword.Do mDo ->
                    { trivia with
                        LeadingKeyword = SynLeadingKeyword.StaticDo(mStatic, mDo)
                    }
                | _ -> trivia

            SynBinding(a0, k, il, im, a, x, v, h, ri, e, m, dp, trivia) :: rest
        | None, decls -> decls

    SynMemberDefn.LetBindings(decls, isStatic, isRec, mWhole)

let mkLocalBindings (mWhole, BindingSetPreAttrs (_, isRec, isUse, declsPreAttrs, _), mIn, body: SynExpr) =
    let ignoredFreeAttrs, decls = declsPreAttrs [] None

    let mWhole =
        match decls with
        | SynBinding (xmlDoc = xmlDoc) :: _ -> unionRangeWithXmlDoc xmlDoc mWhole
        | _ -> mWhole

    if not (isNil ignoredFreeAttrs) then
        warning (Error(FSComp.SR.parsAttributesIgnored (), mWhole))

    let mIn =
        mIn
        |> Option.bind (fun (mIn: range) ->
            if Position.posEq mIn.Start body.Range.Start then
                None
            else
                Some mIn)

    SynExpr.LetOrUse(isRec, isUse, decls, body, mWhole, { InKeyword = mIn })

let mkDefnBindings (mWhole, BindingSetPreAttrs (_, isRec, isUse, declsPreAttrs, _bindingSetRange), attrs, vis, attrsm) =
    if isUse then
        warning (Error(FSComp.SR.parsUseBindingsIllegalInModules (), mWhole))

    let freeAttrs, decls = declsPreAttrs attrs vis
    // decls might have an extended range due to leading attributes
    let mWhole =
        (mWhole, decls) ||> unionRangeWithListBy (fun (SynBinding (range = m)) -> m)

    let letDecls = [ SynModuleDecl.Let(isRec, decls, mWhole) ]

    let attrDecls =
        if not (isNil freeAttrs) then
            [ SynModuleDecl.Attributes(freeAttrs, attrsm) ]
        else
            []

    attrDecls @ letDecls

let idOfPat (parseState: IParseState) m p =
    match p with
    | SynPat.Wild r when parseState.LexBuffer.SupportsFeature LanguageFeature.WildCardInForLoop -> mkSynId r "_"
    | SynPat.Named (SynIdent (id, _), false, _, _) -> id
    | SynPat.LongIdent (longDotId = SynLongIdent ([ id ], _, _); typarDecls = None; argPats = SynArgPats.Pats []; accessibility = None) ->
        id
    | _ -> raiseParseErrorAt m (FSComp.SR.parsIntegerForLoopRequiresSimpleIdentifier ())

let checkForMultipleAugmentations m a1 a2 =
    if not (isNil a1) && not (isNil a2) then
        raiseParseErrorAt m (FSComp.SR.parsOnlyOneWithAugmentationAllowed ())

    a1 @ a2

let rangeOfLongIdent (lid: LongIdent) =
    System.Diagnostics.Debug.Assert(not lid.IsEmpty, "the parser should never produce a long-id that is the empty list")
    (lid.Head.idRange, lid) ||> unionRangeWithListBy (fun id -> id.idRange)

let appendValToLeadingKeyword mVal leadingKeyword =
    match leadingKeyword with
    | SynLeadingKeyword.StaticMember (mStatic, mMember) -> SynLeadingKeyword.StaticMemberVal(mStatic, mMember, mVal)
    | SynLeadingKeyword.Member mMember -> SynLeadingKeyword.MemberVal(mMember, mVal)
    | SynLeadingKeyword.Override mOverride -> SynLeadingKeyword.OverrideVal(mOverride, mVal)
    | SynLeadingKeyword.Default (mDefault) -> SynLeadingKeyword.DefaultVal(mDefault, mVal)
    | _ -> leadingKeyword
