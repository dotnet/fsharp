// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module public Microsoft.FSharp.Compiler.Ast

open System.Collections.Generic
open Internal.Utilities.Text.Lexing
open Internal.Utilities.Text.Parsing
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL.Internal
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.UnicodeLexing
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.PrettyNaming
open Microsoft.FSharp.Compiler.Range

/// The prefix of the names used for the fake namespace path added to all dynamic code entries in FSI.EXE
let FsiDynamicModulePrefix = "FSI_"

[<RequireQualifiedAccess>]
module FSharpLib =
    let Root      = "Microsoft.FSharp"
    let RootPath  = IL.splitNamespace Root
    let Core      = Root + ".Core"
    let CorePath  = IL.splitNamespace Core


[<RequireQualifiedAccess>]
module CustomOperations =
    [<Literal>]
    let Into = "into"

//------------------------------------------------------------------------
// XML doc pre-processing
//-----------------------------------------------------------------------


/// Used to collect XML documentation during lexing and parsing.
type XmlDocCollector() =
    let mutable savedLines = new ResizeArray<(string * pos)>()
    let mutable savedGrabPoints = new ResizeArray<pos>()
    let posCompare p1 p2 = if posGeq p1 p2 then 1 else if posEq p1 p2 then 0 else -1
    let savedGrabPointsAsArray =
        lazy (savedGrabPoints.ToArray() |> Array.sortWith posCompare)

    let savedLinesAsArray =
        lazy (savedLines.ToArray() |> Array.sortWith (fun (_,p1) (_,p2) -> posCompare p1 p2))

    let check() =
        assert (not savedLinesAsArray.IsValueCreated && "can't add more XmlDoc elements to XmlDocCollector after extracting first XmlDoc from the overall results" <> "")

    member x.AddGrabPoint(pos) =
        check()
        savedGrabPoints.Add pos

    member x.AddXmlDocLine(line,pos) =
        check()
        savedLines.Add(line,pos)

    member x.LinesBefore(grabPointPos) = 
      try
        let lines = savedLinesAsArray.Force()
        let grabPoints = savedGrabPointsAsArray.Force()
        let firstLineIndexAfterGrabPoint = Array.findFirstIndexWhereTrue lines (fun (_,pos) -> posGeq pos grabPointPos)
        let grabPointIndex = Array.findFirstIndexWhereTrue grabPoints (fun pos -> posGeq pos grabPointPos)
        assert (posEq grabPoints.[grabPointIndex] grabPointPos)
        let firstLineIndexAfterPrevGrabPoint =
            if grabPointIndex = 0 then
                0
            else
                let prevGrabPointPos = grabPoints.[grabPointIndex-1]
                Array.findFirstIndexWhereTrue lines (fun (_,pos) -> posGeq pos prevGrabPointPos)
        //printfn "#lines = %d, firstLineIndexAfterPrevGrabPoint = %d, firstLineIndexAfterGrabPoint = %d" lines.Length firstLineIndexAfterPrevGrabPoint  firstLineIndexAfterGrabPoint

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

type XmlDoc =
    | XmlDoc of string[]
    static member Empty = XmlDocStatics.Empty
    member x.NonEmpty = (let (XmlDoc lines) = x in lines.Length <> 0)
    static member Merge (XmlDoc lines) (XmlDoc lines') = XmlDoc (Array.append lines lines')
    static member Process (XmlDoc lines) =
        // This code runs for .XML generation and thus influences cross-project xmldoc tooltips; for within-project tooltips, see XmlDocumentation.fs in the language service
        let rec processLines (lines:string list) =
            match lines with
            | [] -> []
            | (lineA::rest) as lines ->
                let lineAT = lineA.TrimStart([|' '|])
                if lineAT = "" then processLines rest
                else if lineAT.StartsWith "<" then lines
                else ["<summary>"] @
                     (lines |> List.map (fun line -> Microsoft.FSharp.Core.XmlAdapters.escape(line))) @
                     ["</summary>"]

        let lines = processLines (Array.toList lines)
        if isNil lines then XmlDoc.Empty
        else XmlDoc (Array.ofList lines)

// Discriminated unions can't contain statics, so we use a separate type
and XmlDocStatics() =
    static let empty = XmlDoc[| |]
    static member Empty = empty

type PreXmlDoc =
    | PreXmlMerge of PreXmlDoc * PreXmlDoc
    | PreXmlDoc of pos * XmlDocCollector
    | PreXmlDocEmpty

    member x.ToXmlDoc() =
        match x with
        | PreXmlMerge(a,b) -> XmlDoc.Merge (a.ToXmlDoc()) (b.ToXmlDoc())
        | PreXmlDocEmpty -> XmlDoc.Empty
        | PreXmlDoc (pos,collector) ->
            let lines = collector.LinesBefore pos
            if lines.Length = 0 then XmlDoc.Empty
            else XmlDoc lines

    static member CreateFromGrabPoint(collector:XmlDocCollector,grabPointPos) =
        collector.AddGrabPoint grabPointPos
        PreXmlDoc(grabPointPos,collector)

    static member Empty = PreXmlDocEmpty
    static member Merge a b = PreXmlMerge (a,b)

type ParserDetail =
    | Ok
    | ThereWereSignificantParseErrorsSoDoNotTypecheckThisNode  // would cause spurious/misleading diagnostics

//------------------------------------------------------------------------
//  AST: identifiers and long identifiers
//-----------------------------------------------------------------------


// PERFORMANCE: consider making this a struct.
[<System.Diagnostics.DebuggerDisplay("{idText}")>]
[<Struct>]
[<NoEquality; NoComparison>]
type Ident (text: string, range: range) =
     member x.idText = text
     member x.idRange = range
     override x.ToString() = text

type LongIdent = Ident list
type LongIdentWithDots =
    /// LongIdentWithDots(lid, dotms)
    /// Typically dotms.Length = lid.Length-1, but they may be same if (incomplete) code ends in a dot, e.g. "Foo.Bar."
    /// The dots mostly matter for parsing, and are typically ignored by the typechecker, but
    /// if dotms.Length = lid.Length, then the parser must have reported an error, so the typechecker is allowed
    /// more freedom about typechecking these expressions.
    /// LongIdent can be empty list - it is used to denote that name of some AST element is absent (i.e. empty type name in inherit)
    | LongIdentWithDots of id:LongIdent * dotms:range list
    with member this.Range =
            match this with
            | LongIdentWithDots([],_) -> failwith "rangeOfLidwd"
            | LongIdentWithDots([id],[]) -> id.idRange
            | LongIdentWithDots([id],[m]) -> unionRanges id.idRange m
            | LongIdentWithDots(h::t,[]) -> unionRanges h.idRange (List.last t).idRange
            | LongIdentWithDots(h::t,dotms) -> unionRanges h.idRange (List.last t).idRange |> unionRanges (List.last dotms)
         member this.Lid = match this with LongIdentWithDots(lid,_) -> lid
         member this.ThereIsAnExtraDotAtTheEnd = match this with LongIdentWithDots(lid,dots) -> lid.Length = dots.Length
         member this.RangeSansAnyExtraDot =
            match this with
            | LongIdentWithDots([],_) -> failwith "rangeOfLidwd"
            | LongIdentWithDots([id],_) -> id.idRange
            | LongIdentWithDots(h::t,dotms) ->
                let nonExtraDots = if dotms.Length = t.Length then dotms else List.take t.Length dotms
                unionRanges h.idRange (List.last t).idRange |> unionRanges (List.last nonExtraDots)

//------------------------------------------------------------------------
//  AST: the grammar of implicitly scoped type parameters
//-----------------------------------------------------------------------

type TyparStaticReq =
    | NoStaticReq
    | HeadTypeStaticReq

[<NoEquality; NoComparison>]
type SynTypar =
    | Typar of ident:Ident * staticReq:TyparStaticReq * isCompGen:bool
    with member this.Range =
            match this with
            | Typar(id,_,_) ->
                id.idRange

//------------------------------------------------------------------------
//  AST: the grammar of constants and measures
//-----------------------------------------------------------------------

type
    [<NoEquality; NoComparison; RequireQualifiedAccess>]
    /// The unchecked abstract syntax tree of constants in F# types and expressions.
    SynConst =
    /// F# syntax: ()
    | Unit
    /// F# syntax: true, false
    | Bool of bool
    /// F# syntax: 13y, 0xFFy, 0o077y, 0b0111101y
    | SByte of sbyte
    /// F# syntax: 13uy, 0x40uy, 0oFFuy, 0b0111101uy
    | Byte of byte
    /// F# syntax: 13s, 0x4000s, 0o0777s, 0b0111101s
    | Int16 of int16
    /// F# syntax: 13us, 0x4000us, 0o0777us, 0b0111101us
    | UInt16 of uint16
    /// F# syntax: 13, 0x4000, 0o0777
    | Int32 of int32
    /// F# syntax: 13u, 0x4000u, 0o0777u
    | UInt32 of uint32
    /// F# syntax: 13L
    | Int64 of int64
    /// F# syntax: 13UL
    | UInt64 of uint64
    /// F# syntax: 13n
    | IntPtr of int64
    /// F# syntax: 13un
    | UIntPtr of uint64
    /// F# syntax: 1.30f, 1.40e10f etc.
    | Single of single
    /// F# syntax: 1.30, 1.40e10 etc.
    | Double of double
    /// F# syntax: 'a'
    | Char of char
    /// F# syntax: 23.4M
    | Decimal of System.Decimal
    /// UserNum(value, suffix)
    ///
    /// F# syntax: 1Q, 1Z, 1R, 1N, 1G
    | UserNum of value:string * suffix:string
    /// F# syntax: verbatim or regular string, e.g. "abc"
    | String of text:string * range:range
    /// F# syntax: verbatim or regular byte string, e.g. "abc"B.
    ///
    /// Also used internally in the typechecker once an array of unit16 constants
    /// is detected, to allow more efficient processing of large arrays of uint16 constants.
    | Bytes of bytes:byte[] * range:range
    /// Used internally in the typechecker once an array of unit16 constants
    /// is detected, to allow more efficient processing of large arrays of uint16 constants.
    | UInt16s of uint16[]
    /// Old comment: "we never iterate, so the const here is not another SynConst.Measure"
    | Measure of constant:SynConst * SynMeasure
    member c.Range dflt =
        match c with
        | SynConst.String (_,m0) | SynConst.Bytes (_,m0) -> m0
        | _ -> dflt

and
    [<NoEquality; NoComparison; RequireQualifiedAccess>]
    /// The unchecked abstract syntax tree of F# unit of measure annotations.
    /// This should probably be merged with the representation of SynType.
    SynMeasure =
    | Named of longId:LongIdent * range:range
    | Product of SynMeasure * SynMeasure * range:range
    | Seq of SynMeasure list * range:range
    | Divide of SynMeasure * SynMeasure * range:range
    | Power of SynMeasure * SynRationalConst * range:range
    | One
    | Anon of range:range
    | Var of SynTypar * range:range

and
    [<NoEquality; NoComparison; RequireQualifiedAccess>]
    /// The unchecked abstract syntax tree of F# unit of measure exponents.
    SynRationalConst =
    | Integer of int32
    | Rational of int32 * int32 * range:range
    | Negate of SynRationalConst


//------------------------------------------------------------------------
//  AST: the grammar of types, expressions, declarations etc.
//-----------------------------------------------------------------------

[<RequireQualifiedAccess>]
type SynAccess =
    | Public
    | Internal
    | Private


type SequencePointInfoForTarget =
    | SequencePointAtTarget
    | SuppressSequencePointAtTarget

type SequencePointInfoForSeq =
    | SequencePointsAtSeq
    // This means "suppress a in 'a;b'" and "suppress b in 'a before b'"
    | SuppressSequencePointOnExprOfSequential
    // This means "suppress b in 'a;b'" and "suppress a in 'a before b'"
    | SuppressSequencePointOnStmtOfSequential

type SequencePointInfoForTry =
    | SequencePointAtTry of range:range
    // Used for "use" and "for"
    | SequencePointInBodyOfTry
    | NoSequencePointAtTry

type SequencePointInfoForWith =
    | SequencePointAtWith of range:range
    | NoSequencePointAtWith

type SequencePointInfoForFinally =
    | SequencePointAtFinally of range:range
    | NoSequencePointAtFinally

type SequencePointInfoForForLoop =
    | SequencePointAtForLoop of range:range
    | NoSequencePointAtForLoop

type SequencePointInfoForWhileLoop =
    | SequencePointAtWhileLoop of range:range
    | NoSequencePointAtWhileLoop

type SequencePointInfoForBinding =
    | SequencePointAtBinding of range:range
    // Indicates the omission of a sequence point for a binding for a 'do expr'
    | NoSequencePointAtDoBinding
    // Indicates the omission of a sequence point for a binding for a 'let e = expr' where 'expr' has immediate control flow
    | NoSequencePointAtLetBinding
    // Indicates the omission of a sequence point for a compiler generated binding
    // where we've done a local expansion of some construct into something that involves
    // a 'let'. e.g. we've inlined a function and bound its arguments using 'let'
    // The let bindings are 'sticky' in that the inversion of the inlining would involve
    // replacing the entire expression with the original and not just the let bindings alone.
    | NoSequencePointAtStickyBinding
    // Given 'let v = e1 in e2', where this is a compiler generated binding,
    // we are sometimes forced to generate a sequence point for the expression anyway based on its
    // overall range. If the let binding is given the flag below then it is asserting that
    // the binding has no interesting side effects and can be totally ignored and the range
    // of the inner expression is used instead
    | NoSequencePointAtInvisibleBinding

    // Don't drop sequence points when combining sequence points
    member x.Combine(y:SequencePointInfoForBinding) =
        match x,y with
        | SequencePointAtBinding _ as g, _  -> g
        | _, (SequencePointAtBinding _ as g)  -> g
        | _ -> x

/// Indicates if a for loop is 'for x in e1 -> e2', only valid in sequence expressions
type SeqExprOnly =
    /// Indicates if a for loop is 'for x in e1 -> e2', only valid in sequence expressions
    | SeqExprOnly of bool

/// denotes location of the separator block + optional position of the semicolon (used for tooling support)
type BlockSeparator = range * pos option
/// stores pair: record field name + (true if given record field name is syntactically correct and can be used in name resolution)
type RecordFieldName = LongIdentWithDots * bool

type ExprAtomicFlag =
    /// Says that the expression is an atomic expression, i.e. is of a form that has no whitespace unless
    /// enclosed in parentheses, e.g. 1, "3", ident, ident.[expr] and (expr). If an atomic expression has
    /// type T, then the largest expression ending at the same range as the atomic expression also has type T.
    | Atomic = 0
    | NonAtomic = 1

/// The kind associated with a binding - "let", "do" or a standalone expression
type SynBindingKind =
    /// A standalone expression in a module
    | StandaloneExpression
    /// A normal 'let' binding in a module
    | NormalBinding
    /// A 'do' binding in a module. Must have type 'unit'
    | DoBinding

type
    [<NoEquality; NoComparison>]
    /// Represents the explicit declaration of a type parameter
    SynTyparDecl =
    | TyparDecl of attributes:SynAttributes * SynTypar


and
    [<NoEquality; NoComparison>]
    /// The unchecked abstract syntax tree of F# type constraints
    SynTypeConstraint =
    /// F# syntax : is 'typar : struct
    | WhereTyparIsValueType of genericName:SynTypar * range:range
    /// F# syntax : is 'typar : not struct
    | WhereTyparIsReferenceType of genericName:SynTypar * range:range
    /// F# syntax is 'typar : unmanaged
    | WhereTyparIsUnmanaged of genericName:SynTypar * range:range
    /// F# syntax is 'typar : null
    | WhereTyparSupportsNull of genericName:SynTypar * range:range
    /// F# syntax is 'typar : comparison
    | WhereTyparIsComparable of genericName:SynTypar * range:range
    /// F# syntax is 'typar : equality
    | WhereTyparIsEquatable of genericName:SynTypar * range:range
    /// F# syntax is default ^T : type
    | WhereTyparDefaultsToType of genericName:SynTypar * typeName:SynType * range:range
    /// F# syntax is 'typar :> type
    | WhereTyparSubtypeOfType of genericName:SynTypar *  typeName:SynType * range:range
    /// F# syntax is ^T : (static member MemberName : ^T * int -> ^T)
    | WhereTyparSupportsMember of genericNames:SynType list * memberSig:SynMemberSig * range:range
    /// F# syntax is 'typar : enum<'UnderlyingType>
    | WhereTyparIsEnum of genericName:SynTypar * SynType list * range:range
    /// F# syntax is 'typar : delegate<'Args,unit>
    | WhereTyparIsDelegate of genericName:SynTypar * SynType list * range:range

and
    [<NoEquality; NoComparison;RequireQualifiedAccess>]
    /// The unchecked abstract syntax tree of F# types
    SynType =
    /// F# syntax : A.B.C
    | LongIdent of longDotId:LongIdentWithDots
    /// App(typeName, LESSm, typeArgs, commasm, GREATERm, isPostfix, m)
    ///
    /// F# syntax : type<type, ..., type> or type type or (type,...,type) type
    ///   isPostfix: indicates a postfix type application e.g. "int list" or "(int,string) dict"
    ///   commasm: ranges for interstitial commas, these only matter for parsing/design-time tooling, the typechecker may munge/discard them
    | App of typeName:SynType * LESSrange:range option * typeArgs:SynType list * commaRanges:range list * GREATERrange:range option * isPostfix:bool * range:range
    /// LongIdentApp(typeName, longId, LESSm, tyArgs, commasm, GREATERm, wholem)
    ///
    /// F# syntax : type.A.B.C<type, ..., type>
    ///   commasm: ranges for interstitial commas, these only matter for parsing/design-time tooling, the typechecker may munge/discard them
    | LongIdentApp of typeName:SynType * longDotId:LongIdentWithDots * LESSRange:range option * typeArgs:SynType list * commaRanges:range list * GREATERrange:range option * range:range

    /// F# syntax : type * ... * type
    // the bool is true if / rather than * follows the type
    | Tuple of typeNames:(bool*SynType) list * range:range

    /// F# syntax : struct (type * ... * type)
    // the bool is true if / rather than * follows the type
    | StructTuple of typeNames:(bool*SynType) list * range:range

    /// F# syntax : type[]
    | Array of  int * elementType:SynType * range:range
    /// F# syntax : type -> type
    | Fun of  argType:SynType * returnType:SynType * range:range
    /// F# syntax : 'Var
    | Var of genericName:SynTypar * range:range
    /// F# syntax : _
    | Anon of range:range
    /// F# syntax : typ with constraints
    | WithGlobalConstraints of typeName:SynType * constraints:SynTypeConstraint list * range:range
    /// F# syntax : #type
    | HashConstraint of SynType * range:range
    /// F# syntax : for units of measure e.g. m / s
    | MeasureDivide of dividendType:SynType * divisorType:SynType * range:range
    /// F# syntax : for units of measure e.g. m^3, kg^1/2
    | MeasurePower of measureType:SynType * SynRationalConst * range:range
    /// F# syntax : 1, "abc" etc, used in parameters to type providers
    /// For the dimensionless units i.e. 1 , and static parameters to provided types
    | StaticConstant of constant:SynConst * range:range
    /// F# syntax : const expr, used in static parameters to type providers
    | StaticConstantExpr of expr:SynExpr * range:range
    /// F# syntax : ident=1 etc., used in static parameters to type providers
    | StaticConstantNamed of expr:SynType * SynType * range:range
    /// Get the syntactic range of source code covered by this construct.
    member x.Range =
        match x with
        | SynType.App (range=m)
        | SynType.LongIdentApp (range=m)
        | SynType.Tuple (range=m)
        | SynType.StructTuple (range=m)
        | SynType.Array (range=m)
        | SynType.Fun (range=m)
        | SynType.Var (range=m)
        | SynType.Anon (range=m)
        | SynType.WithGlobalConstraints (range=m)
        | SynType.StaticConstant (range=m)
        | SynType.StaticConstantExpr (range=m)
        | SynType.StaticConstantNamed (range=m)
        | SynType.HashConstraint (range=m)
        | SynType.MeasureDivide (range=m)
        | SynType.MeasurePower (range=m) -> m
        | SynType.LongIdent(lidwd) -> lidwd.Range



and
    [<NoEquality; NoComparison;RequireQualifiedAccess>]
    SynExpr =

    /// F# syntax: (expr)
    ///
    /// Paren(expr, leftParenRange, rightParenRange, wholeRangeIncludingParentheses)
    ///
    /// Parenthesized expressions. Kept in AST to distinguish A.M((x,y))
    /// from A.M(x,y), among other things.
    | Paren of expr:SynExpr * leftParenRange:range * rightParenRange:range option * range:range

    /// F# syntax: <@ expr @>, <@@ expr @@>
    ///
    /// Quote(operator,isRaw,quotedSynExpr,isFromQueryExpression,m)
    | Quote of operator:SynExpr * isRaw:bool * quotedSynExpr:SynExpr * isFromQueryExpression:bool * range:range

    /// F# syntax: 1, 1.3, () etc.
    | Const of constant:SynConst * range:range

    /// F# syntax: expr : type
    | Typed of  expr:SynExpr * typeName:SynType * range:range

    /// F# syntax: e1, ..., eN
    | Tuple of  exprs:SynExpr list * commaRanges:range list * range:range  // "range list" is for interstitial commas, these only matter for parsing/design-time tooling, the typechecker may munge/discard them

    /// F# syntax: struct (e1, ..., eN)
    | StructTuple of  exprs:SynExpr list * commaRanges:range list * range:range  // "range list" is for interstitial commas, these only matter for parsing/design-time tooling, the typechecker may munge/discard them

    /// F# syntax: [ e1; ...; en ], [| e1; ...; en |]
    | ArrayOrList of  isList:bool * exprs:SynExpr list * range:range

    /// F# syntax: { f1=e1; ...; fn=en }
    /// SynExpr.Record((baseType, baseCtorArgs, mBaseCtor, sepAfterBase, mInherits), (copyExpr, sepAfterCopyExpr), (recordFieldName, fieldValue, sepAfterField), mWholeExpr)
    /// inherit includes location of separator (for tooling)
    /// copyOpt contains range of the following WITH part (for tooling)
    /// every field includes range of separator after the field (for tooling)
    | Record of baseInfo:(SynType * SynExpr * range * BlockSeparator option * range) option * copyInfo:(SynExpr * BlockSeparator) option * recordFields:(RecordFieldName * (SynExpr option) * BlockSeparator option) list * range:range

    /// F# syntax: new C(...)
    /// The flag is true if known to be 'family' ('protected') scope
    | New of isProtected:bool * typeName:SynType * expr:SynExpr * range:range

    /// SynExpr.ObjExpr(objTy,argOpt,binds,extraImpls,mNewExpr,mWholeExpr)
    ///
    /// F# syntax: { new ... with ... }
    | ObjExpr of objType:SynType * argOptions:(SynExpr * Ident option) option * bindings:SynBinding list * extraImpls:SynInterfaceImpl list * newExprRange:range * range:range

    /// F# syntax: 'while ... do ...'
    | While of whileSeqPoint:SequencePointInfoForWhileLoop * whileExpr:SynExpr * doExpr:SynExpr * range:range

    /// F# syntax: 'for i = ... to ... do ...'
    | For of forSeqPoint:SequencePointInfoForForLoop * ident:Ident * identBody:SynExpr * bool * toBody:SynExpr * doBody:SynExpr * range:range

    /// SynExpr.ForEach (spBind, seqExprOnly, isFromSource, pat, enumExpr, bodyExpr, mWholeExpr).
    ///
    /// F# syntax: 'for ... in ... do ...'
    | ForEach of forSeqPoint:SequencePointInfoForForLoop * seqExprOnly:SeqExprOnly * isFromSource:bool * pat:SynPat * enumExpr:SynExpr * bodyExpr:SynExpr * range:range

    /// F# syntax: [ expr ], [| expr |]
    | ArrayOrListOfSeqExpr of isArray:bool * expr:SynExpr * range:range

    /// CompExpr(isArrayOrList, isNotNakedRefCell, expr)
    ///
    /// F# syntax: { expr }
    | CompExpr of isArrayOrList:bool * isNotNakedRefCell:bool ref * expr:SynExpr * range:range

    /// First bool indicates if lambda originates from a method. Patterns here are always "simple"
    /// Second bool indicates if this is a "later" part of an iterated sequence of lambdas
    ///
    /// F# syntax: fun pat -> expr
    | Lambda of  fromMethod:bool * inLambdaSeq:bool * args:SynSimplePats * body:SynExpr * range:range

    /// F# syntax: function pat1 -> expr | ... | patN -> exprN
    | MatchLambda of isExnMatch:bool * range * SynMatchClause list * matchSeqPoint:SequencePointInfoForBinding * range:range

    /// F# syntax: match expr with pat1 -> expr | ... | patN -> exprN
    | Match of  matchSeqPoint:SequencePointInfoForBinding * expr:SynExpr * clauses:SynMatchClause list * isExnMatch:bool * range:range (* bool indicates if this is an exception match in a computation expression which throws unmatched exceptions *)

    /// F# syntax: do expr
    | Do of  expr:SynExpr * range:range

    /// F# syntax: assert expr
    | Assert of expr:SynExpr * range:range

    /// App(exprAtomicFlag, isInfix, funcExpr, argExpr, m)
    ///  - exprAtomicFlag: indicates if the application is syntactically atomic, e.g. f.[1] is atomic, but 'f x' is not
    ///  - isInfix is true for the first app of an infix operator, e.g. 1+2 becomes App(App(+,1),2), where the inner node is marked isInfix
    ///      (or more generally, for higher operator fixities, if App(x,y) is such that y comes before x in the source code, then the node is marked isInfix=true)
    ///
    /// F# syntax: f x
    | App of ExprAtomicFlag * isInfix:bool * funcExpr:SynExpr * argExpr:SynExpr * range:range

    /// TypeApp(expr, mLessThan, types, mCommas, mGreaterThan, mTypeArgs, mWholeExpr)
    ///     "mCommas" are the ranges for interstitial commas, these only matter for parsing/design-time tooling, the typechecker may munge/discard them
    ///
    /// F# syntax: expr<type1,...,typeN>
    | TypeApp of expr:SynExpr * LESSrange:range * typeNames:SynType list * commaRanges:range list * GREATERrange:range option * typeArgsRange:range * range:range

    /// LetOrUse(isRecursive, isUse, bindings, body, wholeRange)
    ///
    /// F# syntax: let pat = expr in expr
    /// F# syntax: let f pat1 .. patN = expr in expr
    /// F# syntax: let rec f pat1 .. patN = expr in expr
    /// F# syntax: use pat = expr in expr
    | LetOrUse of isRecursive:bool * isUse:bool * bindings:SynBinding list * body:SynExpr * range:range

    /// F# syntax: try expr with pat -> expr
    | TryWith of tryExpr:SynExpr * tryRange:range * withCases:SynMatchClause list * withRange:range * range:range * trySeqPoint:SequencePointInfoForTry * withSeqPoint:SequencePointInfoForWith

    /// F# syntax: try expr finally expr
    | TryFinally of tryExpr:SynExpr * finallyExpr:SynExpr * range:range * trySeqPoint:SequencePointInfoForTry * finallySeqPoint:SequencePointInfoForFinally

    /// F# syntax: lazy expr
    | Lazy of SynExpr * range:range

    /// Seq(seqPoint, isTrueSeq, e1, e2, m)
    ///  isTrueSeq: false indicates "let v = a in b; v"
    ///
    /// F# syntax: expr; expr
    | Sequential of seqPoint:SequencePointInfoForSeq * isTrueSeq:bool * expr1:SynExpr * expr2:SynExpr * range:range

    ///  IfThenElse(exprGuard,exprThen,optionalExprElse,spIfToThen,isFromErrorRecovery,mIfToThen,mIfToEndOfLastBranch)
    ///
    /// F# syntax: if expr then expr
    /// F# syntax: if expr then expr else expr
    | IfThenElse of ifExpr:SynExpr * thenExpr:SynExpr * elseExpr:SynExpr option * spIfToThen:SequencePointInfoForBinding * isFromErrorRecovery:bool * ifToThenRange:range * range:range

    /// F# syntax: ident
    /// Optimized representation, = SynExpr.LongIdent(false,[id],id.idRange)
    | Ident of Ident

    /// F# syntax: ident.ident...ident
    /// LongIdent(isOptional, longIdent, altNameRefCell, m)
    ///   isOptional: true if preceded by a '?' for an optional named parameter
    ///   altNameRefCell: Normally 'None' except for some compiler-generated variables in desugaring pattern matching. See SynSimplePat.Id
    | LongIdent of isOptional:bool * longDotId:LongIdentWithDots * altNameRefCell:SynSimplePatAlternativeIdInfo ref option * range:range

    /// F# syntax: ident.ident...ident <- expr
    | LongIdentSet of longDotId:LongIdentWithDots * expr:SynExpr * range:range

    /// DotGet(expr, rangeOfDot, lid, wholeRange)
    ///
    /// F# syntax: expr.ident.ident
    | DotGet of expr:SynExpr * rangeOfDot:range * longDotId:LongIdentWithDots * range:range

    /// F# syntax: expr.ident...ident <- expr
    | DotSet of SynExpr * longDotId:LongIdentWithDots * SynExpr * range:range

    /// F# syntax: expr.[expr,...,expr]
    | DotIndexedGet of SynExpr * SynIndexerArg list * range * range:range

    /// DotIndexedSet (objectExpr, indexExprs, valueExpr, rangeOfLeftOfSet, rangeOfDot, rangeOfWholeExpr)
    ///
    /// F# syntax: expr.[expr,...,expr] <- expr
    | DotIndexedSet of objectExpr:SynExpr * indexExprs:SynIndexerArg list * valueExpr:SynExpr * leftOfSetRange:range * dotRange:range * range:range

    /// F# syntax: Type.Items(e1) <- e2 , rarely used named-property-setter notation, e.g. Foo.Bar.Chars(3) <- 'a'
    | NamedIndexedPropertySet of longDotId:LongIdentWithDots * SynExpr * SynExpr * range:range

    /// F# syntax: expr.Items(e1) <- e2 , rarely used named-property-setter notation, e.g. (stringExpr).Chars(3) <- 'a'
    | DotNamedIndexedPropertySet of SynExpr * longDotId:LongIdentWithDots * SynExpr * SynExpr * range:range

    /// F# syntax: expr :? type
    | TypeTest of  expr:SynExpr * typeName:SynType * range:range

    /// F# syntax: expr :> type
    | Upcast of  expr:SynExpr * typeName:SynType * range:range

    /// F# syntax: expr :?> type
    | Downcast of  expr:SynExpr * typeName:SynType * range:range

    /// F# syntax: upcast expr
    | InferredUpcast of  expr:SynExpr * range:range

    /// F# syntax: downcast expr
    | InferredDowncast of  expr:SynExpr * range:range

    /// F# syntax: null
    | Null of range:range

    /// F# syntax: &expr, &&expr
    | AddressOf of  isByref:bool * SynExpr * range * range:range

    /// F# syntax: ((typar1 or ... or typarN): (member-dig) expr)
    | TraitCall of SynTypar list * SynMemberSig * SynExpr * range:range

    /// F# syntax: ... in ...
    /// Computation expressions only, based on JOIN_IN token from lex filter
    | JoinIn of SynExpr * range * SynExpr * range:range

    /// F# syntax: <implicit>
    /// Computation expressions only, implied by final "do" or "do!"
    | ImplicitZero of range:range

    /// F# syntax: yield expr
    /// F# syntax: return expr
    /// Computation expressions only
    | YieldOrReturn   of (bool * bool) * expr:SynExpr * range:range

    /// F# syntax: yield! expr
    /// F# syntax: return! expr
    /// Computation expressions only
    | YieldOrReturnFrom  of (bool * bool) * expr:SynExpr * range:range

    /// SynExpr.LetOrUseBang(spBind, isUse, isFromSource, pat, rhsExpr, bodyExpr, mWholeExpr).
    ///
    /// F# syntax: let! pat = expr in expr
    /// F# syntax: use! pat = expr in expr
    /// Computation expressions only
    | LetOrUseBang    of bindSeqPoint:SequencePointInfoForBinding * isUse:bool * isFromSource:bool * SynPat * SynExpr * SynExpr * range:range

    /// F# syntax: match! expr with pat1 -> expr | ... | patN -> exprN
    | MatchBang of  matchSeqPoint:SequencePointInfoForBinding * expr:SynExpr * clauses:SynMatchClause list * isExnMatch:bool * range:range (* bool indicates if this is an exception match in a computation expression which throws unmatched exceptions *)

    /// F# syntax: do! expr
    /// Computation expressions only
    | DoBang      of expr:SynExpr * range:range

    /// Only used in FSharp.Core
    | LibraryOnlyILAssembly of ILInstr array *  SynType list * SynExpr list * SynType list * range:range (* Embedded IL assembly code *)

    /// Only used in FSharp.Core
    | LibraryOnlyStaticOptimization of SynStaticOptimizationConstraint list * SynExpr * SynExpr * range:range

    /// Only used in FSharp.Core
    | LibraryOnlyUnionCaseFieldGet of expr:SynExpr * longId:LongIdent * int * range:range

    /// Only used in FSharp.Core
    | LibraryOnlyUnionCaseFieldSet of SynExpr * longId:LongIdent * int * SynExpr * range:range

    /// Inserted for error recovery
    | ArbitraryAfterError  of debugStr:string * range:range

    /// Inserted for error recovery
    | FromParseError  of expr:SynExpr * range:range

    /// Inserted for error recovery when there is "expr." and missing tokens or error recovery after the dot
    | DiscardAfterMissingQualificationAfterDot  of SynExpr * range:range

    /// 'use x = fixed expr'
    | Fixed of expr:SynExpr * range:range

    /// Get the syntactic range of source code covered by this construct.
    member e.Range =
        match e with
        | SynExpr.Paren (range=m)
        | SynExpr.Quote (range=m)
        | SynExpr.Const (range=m)
        | SynExpr.Typed (range=m)
        | SynExpr.Tuple (range=m)
        | SynExpr.StructTuple (range=m)
        | SynExpr.ArrayOrList (range=m)
        | SynExpr.Record (range=m)
        | SynExpr.New (range=m)
        | SynExpr.ObjExpr (range=m)
        | SynExpr.While (range=m)
        | SynExpr.For (range=m)
        | SynExpr.ForEach (range=m)
        | SynExpr.CompExpr (range=m)
        | SynExpr.ArrayOrListOfSeqExpr (range=m)
        | SynExpr.Lambda (range=m)
        | SynExpr.Match (range=m)
        | SynExpr.MatchLambda (range=m)
        | SynExpr.Do (range=m)
        | SynExpr.Assert (range=m)
        | SynExpr.App (range=m)
        | SynExpr.TypeApp (range=m)
        | SynExpr.LetOrUse (range=m)
        | SynExpr.TryWith (range=m)
        | SynExpr.TryFinally (range=m)
        | SynExpr.Sequential (range=m)
        | SynExpr.ArbitraryAfterError (range=m)
        | SynExpr.FromParseError (range=m)
        | SynExpr.DiscardAfterMissingQualificationAfterDot (range=m)
        | SynExpr.IfThenElse (range=m)
        | SynExpr.LongIdent (range=m)
        | SynExpr.LongIdentSet (range=m)
        | SynExpr.NamedIndexedPropertySet (range=m)
        | SynExpr.DotIndexedGet (range=m)
        | SynExpr.DotIndexedSet (range=m)
        | SynExpr.DotGet (range=m)
        | SynExpr.DotSet (range=m)
        | SynExpr.DotNamedIndexedPropertySet (range=m)
        | SynExpr.LibraryOnlyUnionCaseFieldGet (range=m)
        | SynExpr.LibraryOnlyUnionCaseFieldSet (range=m)
        | SynExpr.LibraryOnlyILAssembly (range=m)
        | SynExpr.LibraryOnlyStaticOptimization (range=m)
        | SynExpr.TypeTest (range=m)
        | SynExpr.Upcast (range=m)
        | SynExpr.AddressOf (range=m)
        | SynExpr.Downcast (range=m)
        | SynExpr.JoinIn (range=m)
        | SynExpr.InferredUpcast (range=m)
        | SynExpr.InferredDowncast (range=m)
        | SynExpr.Null (range=m)
        | SynExpr.Lazy (range=m)
        | SynExpr.TraitCall (range=m)
        | SynExpr.ImplicitZero (range=m)
        | SynExpr.YieldOrReturn (range=m)
        | SynExpr.YieldOrReturnFrom (range=m)
        | SynExpr.LetOrUseBang (range=m)
        | SynExpr.MatchBang (range=m)
        | SynExpr.DoBang (range=m)
        | SynExpr.Fixed (range=m) -> m
        | SynExpr.Ident id -> id.idRange

    /// range ignoring any (parse error) extra trailing dots
    member e.RangeSansAnyExtraDot =
        match e with
        | SynExpr.Paren (range=m)
        | SynExpr.Quote (range=m)
        | SynExpr.Const (range=m)
        | SynExpr.Typed (range=m)
        | SynExpr.Tuple (range=m)
        | SynExpr.StructTuple (range=m)
        | SynExpr.ArrayOrList (range=m)
        | SynExpr.Record (range=m)
        | SynExpr.New (range=m)
        | SynExpr.ObjExpr (range=m)
        | SynExpr.While (range=m)
        | SynExpr.For (range=m)
        | SynExpr.ForEach (range=m)
        | SynExpr.CompExpr (range=m)
        | SynExpr.ArrayOrListOfSeqExpr (range=m)
        | SynExpr.Lambda (range=m)
        | SynExpr.Match (range=m)
        | SynExpr.MatchLambda (range=m)
        | SynExpr.Do (range=m)
        | SynExpr.Assert (range=m)
        | SynExpr.App (range=m)
        | SynExpr.TypeApp (range=m)
        | SynExpr.LetOrUse (range=m)
        | SynExpr.TryWith (range=m)
        | SynExpr.TryFinally (range=m)
        | SynExpr.Sequential (range=m)
        | SynExpr.ArbitraryAfterError (range=m)
        | SynExpr.FromParseError (range=m)
        | SynExpr.IfThenElse (range=m)
        | SynExpr.LongIdentSet (range=m)
        | SynExpr.NamedIndexedPropertySet (range=m)
        | SynExpr.DotIndexedGet (range=m)
        | SynExpr.DotIndexedSet (range=m)
        | SynExpr.DotSet (range=m)
        | SynExpr.DotNamedIndexedPropertySet (range=m)
        | SynExpr.LibraryOnlyUnionCaseFieldGet (range=m)
        | SynExpr.LibraryOnlyUnionCaseFieldSet (range=m)
        | SynExpr.LibraryOnlyILAssembly (range=m)
        | SynExpr.LibraryOnlyStaticOptimization (range=m)
        | SynExpr.TypeTest (range=m)
        | SynExpr.Upcast (range=m)
        | SynExpr.AddressOf (range=m)
        | SynExpr.Downcast (range=m)
        | SynExpr.JoinIn (range=m)
        | SynExpr.InferredUpcast (range=m)
        | SynExpr.InferredDowncast (range=m)
        | SynExpr.Null (range=m)
        | SynExpr.Lazy (range=m)
        | SynExpr.TraitCall (range=m)
        | SynExpr.ImplicitZero (range=m)
        | SynExpr.YieldOrReturn (range=m)
        | SynExpr.YieldOrReturnFrom (range=m)
        | SynExpr.LetOrUseBang (range=m)
        | SynExpr.MatchBang (range=m)
        | SynExpr.DoBang (range=m) -> m
        | SynExpr.DotGet (expr,_,lidwd,m) -> if lidwd.ThereIsAnExtraDotAtTheEnd then unionRanges expr.Range lidwd.RangeSansAnyExtraDot else m
        | SynExpr.LongIdent (_,lidwd,_,_) -> lidwd.RangeSansAnyExtraDot
        | SynExpr.DiscardAfterMissingQualificationAfterDot (expr,_) -> expr.Range
        | SynExpr.Fixed (_,m) -> m
        | SynExpr.Ident id -> id.idRange
    /// Attempt to get the range of the first token or initial portion only - this is extremely ad-hoc, just a cheap way to improve a certain 'query custom operation' error range
    member e.RangeOfFirstPortion =
        match e with
        // haven't bothered making these cases better than just .Range
        | SynExpr.Quote (range=m)
        | SynExpr.Const (range=m)
        | SynExpr.Typed (range=m)
        | SynExpr.Tuple (range=m)
        | SynExpr.StructTuple (range=m)
        | SynExpr.ArrayOrList (range=m)
        | SynExpr.Record (range=m)
        | SynExpr.New (range=m)
        | SynExpr.ObjExpr (range=m)
        | SynExpr.While (range=m)
        | SynExpr.For (range=m)
        | SynExpr.CompExpr (range=m)
        | SynExpr.ArrayOrListOfSeqExpr (range=m)
        | SynExpr.Lambda (range=m)
        | SynExpr.Match (range=m)
        | SynExpr.MatchLambda (range=m)
        | SynExpr.Do (range=m)
        | SynExpr.Assert (range=m)
        | SynExpr.TypeApp (range=m)
        | SynExpr.LetOrUse (range=m)
        | SynExpr.TryWith (range=m)
        | SynExpr.TryFinally (range=m)
        | SynExpr.ArbitraryAfterError (range=m)
        | SynExpr.FromParseError (range=m)
        | SynExpr.DiscardAfterMissingQualificationAfterDot (range=m)
        | SynExpr.IfThenElse (range=m)
        | SynExpr.LongIdent (range=m)
        | SynExpr.LongIdentSet (range=m)
        | SynExpr.NamedIndexedPropertySet (range=m)
        | SynExpr.DotIndexedGet (range=m)
        | SynExpr.DotIndexedSet (range=m)
        | SynExpr.DotGet (range=m)
        | SynExpr.DotSet (range=m)
        | SynExpr.DotNamedIndexedPropertySet (range=m)
        | SynExpr.LibraryOnlyUnionCaseFieldGet (range=m)
        | SynExpr.LibraryOnlyUnionCaseFieldSet (range=m)
        | SynExpr.LibraryOnlyILAssembly (range=m)
        | SynExpr.LibraryOnlyStaticOptimization (range=m)
        | SynExpr.TypeTest (range=m)
        | SynExpr.Upcast (range=m)
        | SynExpr.AddressOf (range=m)
        | SynExpr.Downcast (range=m)
        | SynExpr.JoinIn (range=m)
        | SynExpr.InferredUpcast (range=m)
        | SynExpr.InferredDowncast (range=m)
        | SynExpr.Null (range=m)
        | SynExpr.Lazy (range=m)
        | SynExpr.TraitCall (range=m)
        | SynExpr.ImplicitZero (range=m)
        | SynExpr.YieldOrReturn (range=m)
        | SynExpr.YieldOrReturnFrom (range=m)
        | SynExpr.LetOrUseBang (range=m)
        | SynExpr.MatchBang (range=m)
        | SynExpr.DoBang (range=m)  -> m
        // these are better than just .Range, and also commonly applicable inside queries
        | SynExpr.Paren(_,m,_,_) -> m
        | SynExpr.Sequential (_,_,e1,_,_)
        | SynExpr.App (_,_,e1,_,_) ->
            e1.RangeOfFirstPortion
        | SynExpr.ForEach (_,_,_,pat,_,_,r) ->
            let start = r.Start
            let e = (pat.Range : range).Start
            mkRange r.FileName start e
        | SynExpr.Ident id -> id.idRange
        | SynExpr.Fixed (_,m) -> m


and
    [<NoEquality; NoComparison; RequireQualifiedAccess>]
    SynIndexerArg =
    | Two of SynExpr * SynExpr
    | One of SynExpr
    member x.Range = match x with Two (e1,e2) -> unionRanges e1.Range e2.Range | One e -> e.Range
    member x.Exprs = match x with Two (e1,e2) -> [e1;e2] | One e -> [e]
and
    [<NoEquality; NoComparison; RequireQualifiedAccess>]
    SynSimplePat =

    /// Id (ident, altNameRefCell, isCompilerGenerated, isThisVar, isOptArg, range)
    ///
    /// Indicates a simple pattern variable.
    ///
    ///   altNameRefCell
    ///     Normally 'None' except for some compiler-generated variables in desugaring pattern matching.
    ///     Pattern processing sets this reference for hidden variable introduced by desugaring pattern matching in arguments.
    ///     The info indicates an alternative (compiler generated) identifier to be used because the name of the identifier is already bound.
    ///     See Product Studio FSharp 1.0, bug 6389.
    ///
    ///   isCompilerGenerated : true if a compiler generated name
    ///   isThisVar: true if 'this' variable in member
    ///   isOptArg: true if a '?' is in front of the name
    | Id of  ident:Ident * altNameRefCell:SynSimplePatAlternativeIdInfo ref option * isCompilerGenerated:bool * isThisVar:bool *  isOptArg:bool * range:range

    | Typed of  SynSimplePat * SynType * range:range
    | Attrib of  SynSimplePat * SynAttributes * range:range


and SynSimplePatAlternativeIdInfo =
    /// We have not decided to use an alternative name in tha pattern and related expression
    | Undecided of Ident
    /// We have decided to use an alternative name in tha pattern and related expression
    | Decided of Ident

and
    [<NoEquality; NoComparison>]
    SynStaticOptimizationConstraint =
    | WhenTyparTyconEqualsTycon of SynTypar *  SynType * range:range
    | WhenTyparIsStruct of SynTypar * range:range

and
    [<NoEquality; NoComparison;RequireQualifiedAccess>]
    /// Represents a simple set of variable bindings a, (a,b) or (a:Type,b:Type) at a lambda,
    /// function definition or other binding point, after the elimination of pattern matching
    /// from the construct, e.g. after changing a "function pat1 -> rule1 | ..." to a
    /// "fun v -> match v with ..."
    SynSimplePats =
    | SimplePats of SynSimplePat list * range:range
    | Typed of  SynSimplePats * SynType * range:range

and SynConstructorArgs =
    | Pats of SynPat list
    | NamePatPairs of (Ident * SynPat) list * range:range
and
    [<NoEquality; NoComparison;RequireQualifiedAccess>]
    SynPat =
    | Const of SynConst * range:range
    | Wild of range:range
    | Named of SynPat * Ident *  isSelfIdentifier:bool (* true if 'this' variable *)  * accessibility:SynAccess option * range:range
    | Typed of SynPat * SynType * range:range
    | Attrib of SynPat * SynAttributes * range:range
    | Or of SynPat * SynPat * range:range
    | Ands of SynPat list * range:range
    | LongIdent of longDotId:LongIdentWithDots * (* holds additional ident for tooling *) Ident option * SynValTyparDecls option (* usually None: temporary used to parse "f<'a> x = x"*) * SynConstructorArgs  * accessibility:SynAccess option * range:range
    | Tuple of SynPat list * range:range
    | StructTuple of SynPat list * range:range
    | Paren of SynPat * range:range
    | ArrayOrList of bool * SynPat list * range:range
    | Record of ((LongIdent * Ident) * SynPat) list * range:range
    /// 'null'
    | Null of range:range
    /// '?id' -- for optional argument names
    | OptionalVal of Ident * range:range
    /// ':? type '
    | IsInst of SynType * range:range
    /// &lt;@ expr @&gt;, used for active pattern arguments
    | QuoteExpr of SynExpr * range:range

    /// Deprecated character range:ranges
    | DeprecatedCharRange of char * char * range:range
    /// Used internally in the type checker
    | InstanceMember of  Ident * Ident * (* holds additional ident for tooling *) Ident option * accessibility:SynAccess option * range:range (* adhoc overloaded method/property *)

    /// A pattern arising from a parse error
    | FromParseError of SynPat * range:range

    member p.Range =
      match p with
      | SynPat.Const (range=m)
      | SynPat.Wild (range=m)
      | SynPat.Named (range=m)
      | SynPat.Or (range=m)
      | SynPat.Ands (range=m)
      | SynPat.LongIdent (range=m)
      | SynPat.ArrayOrList (range=m)
      | SynPat.Tuple (range=m)
      | SynPat.StructTuple (range=m)
      | SynPat.Typed (range=m)
      | SynPat.Attrib (range=m)
      | SynPat.Record (range=m)
      | SynPat.DeprecatedCharRange (range=m)
      | SynPat.Null (range=m)
      | SynPat.IsInst (range=m)
      | SynPat.QuoteExpr (range=m)
      | SynPat.InstanceMember (range=m)
      | SynPat.OptionalVal (range=m)
      | SynPat.Paren (range=m)
      | SynPat.FromParseError (range=m) -> m

and
    [<NoEquality; NoComparison>]
    SynInterfaceImpl =
    | InterfaceImpl of SynType * SynBinding list * range:range

and
    [<NoEquality; NoComparison>]
    SynMatchClause =
    | Clause of SynPat * SynExpr option *  SynExpr * range:range * SequencePointInfoForTarget
    member this.RangeOfGuardAndRhs =
        match this with
        | Clause(_,eo,e,_,_) ->
            match eo with
            | None -> e.Range
            | Some x -> unionRanges e.Range x.Range
    member this.Range =
        match this with
        | Clause(_,eo,e,m,_) ->
            match eo with
            | None -> unionRanges e.Range m
            | Some x -> unionRanges (unionRanges e.Range m) x.Range

and SynAttributes = SynAttribute list

and
    [<NoEquality; NoComparison; RequireQualifiedAccess>]
    SynAttribute =
    { TypeName: LongIdentWithDots
      ArgExpr: SynExpr
      /// Target specifier, e.g. "assembly","module",etc.
      Target: Ident option
      /// Is this attribute being applied to a property getter or setter?
      AppliesToGetterAndSetter: bool
      Range: range }

and
    [<NoEquality; NoComparison>]
    SynValData =
    | SynValData of MemberFlags option * SynValInfo * Ident option

and
    [<NoEquality; NoComparison>]
    SynBinding =
    | Binding of
        accessibility:SynAccess option *
        kind:SynBindingKind *
        mustInline:bool *
        isMutable:bool *
        attrs:SynAttributes *
        xmlDoc:PreXmlDoc *
        valData:SynValData *
        headPat:SynPat *
        returnInfo:SynBindingReturnInfo option *
        expr:SynExpr  *
        range:range *
        seqPoint:SequencePointInfoForBinding
    // no member just named "Range", as that would be confusing:
    //  - for everything else, the 'range' member that appears last/second-to-last is the 'full range' of the whole tree construct
    //  - but for Binding, the 'range' is only the range of the left-hand-side, the right-hand-side range is in the SynExpr
    //  - so we use explicit names to avoid confusion
    member x.RangeOfBindingSansRhs = let (Binding(range=m)) = x in m
    member x.RangeOfBindingAndRhs = let (Binding(expr=e; range=m)) = x in unionRanges e.Range m
    member x.RangeOfHeadPat = let (Binding(headPat=headPat)) = x in headPat.Range

and
    [<NoEquality; NoComparison>]
    SynBindingReturnInfo =
    | SynBindingReturnInfo of typeName:SynType * range:range * attributes:SynAttributes


and
    [<NoComparison>]
    MemberFlags =
    { IsInstance: bool
      IsDispatchSlot: bool
      IsOverrideOrExplicitImpl: bool
      IsFinal: bool
      MemberKind: MemberKind }

/// Note the member kind is actually computed partially by a syntax tree transformation in tc.fs
and
    [<StructuralEquality; NoComparison; RequireQualifiedAccess>]
    MemberKind =
    | ClassConstructor
    | Constructor
    | Member
    | PropertyGet
    | PropertySet
    /// An artificial member kind used prior to the point where a get/set property is split into two distinct members.
    | PropertyGetSet

and
    [<NoEquality; NoComparison; RequireQualifiedAccess>]
    /// The untyped, unchecked syntax tree for a member signature, used in signature files, abstract member declarations
    /// and member constraints.
    SynMemberSig =
    | Member of SynValSig * MemberFlags * range:range
    | Interface of typeName:SynType * range:range
    | Inherit of typeName:SynType * range:range
    | ValField of SynField * range:range
    | NestedType  of SynTypeDefnSig * range:range

and SynMemberSigs = SynMemberSig list

and
    [<NoEquality; NoComparison>]
    SynTypeDefnKind =
    | TyconUnspecified
    | TyconClass
    | TyconInterface
    | TyconStruct
    | TyconRecord
    | TyconUnion
    | TyconAbbrev
    | TyconHiddenRepr
    | TyconAugmentation
    | TyconILAssemblyCode
    | TyconDelegate of SynType * SynValInfo


and
    [<NoEquality; NoComparison; RequireQualifiedAccess>]
    /// The untyped, unchecked syntax tree for the core of a simple type definition, in either signature
    /// or implementation.
    SynTypeDefnSimpleRepr =

    /// A union type definition, type X = A | B
    | Union      of accessibility:SynAccess option * unionCases:SynUnionCases * range:range
    /// An enum type definition, type X = A = 1 | B = 2
    | Enum       of SynEnumCases * range:range
    /// A record type definition, type X = { A : int; B : int }
    | Record     of accessibility:SynAccess option * recordFields:SynFields * range:range
    /// An object oriented type definition. This is not a parse-tree form, but represents the core
    /// type representation which the type checker splits out from the "ObjectModel" cases of type definitions.
    | General    of SynTypeDefnKind * (SynType * range * Ident option) list * (SynValSig * MemberFlags) list * SynField list  * bool * bool * SynSimplePat list option * range:range
    /// A type defined by using an IL assembly representation. Only used in FSharp.Core.
    ///
    /// F# syntax: "type X = (# "..."#)
    | LibraryOnlyILAssembly of ILType * range:range
    /// A type abbreviation, "type X = A.B.C"
    | TypeAbbrev of ParserDetail * SynType * range:range
    /// An abstract definition , "type X"
    | None       of range:range
    /// An exception definition , "exception E = ..."
    | Exception of SynExceptionDefnRepr

    member this.Range =
        match this with
        | Union (range=m)
        | Enum (range=m)
        | Record (range=m)
        | General (range=m)
        | LibraryOnlyILAssembly (range=m)
        | TypeAbbrev (range=m)
        | None (range=m) -> m
        | Exception t -> t.Range

and SynEnumCases = SynEnumCase list

and
    [<NoEquality; NoComparison>]
    SynEnumCase =
    /// The untyped, unchecked syntax tree for one case in an enum definition.
    | EnumCase of attrs:SynAttributes * ident:Ident * SynConst * PreXmlDoc * range:range
    member this.Range =
        match this with
        | EnumCase (range=m) -> m

and SynUnionCases = SynUnionCase list

and
    [<NoEquality; NoComparison>]
    SynUnionCase =
    /// The untyped, unchecked syntax tree for one case in a union definition.
    | UnionCase of SynAttributes * ident:Ident * SynUnionCaseType * PreXmlDoc * accessibility:SynAccess option * range:range
    member this.Range =
        match this with
        | UnionCase (range=m) -> m

and
    [<NoEquality; NoComparison>]
    /// The untyped, unchecked syntax tree for the right-hand-side of union definition, excluding members,
    /// in either a signature or implementation.
    SynUnionCaseType =
    /// Normal style declaration
    | UnionCaseFields of cases:SynField list
    /// Full type spec given by 'UnionCase : ty1 * tyN -> rty'. Only used in FSharp.Core, otherwise a warning.
    | UnionCaseFullType of (SynType * SynValInfo)

and
    [<NoEquality; NoComparison; RequireQualifiedAccess>]
    /// The untyped, unchecked syntax tree for the right-hand-side of a type definition in a signature.
    /// Note: in practice, using a discriminated union to make a distinction between
    /// "simple" types and "object oriented" types is not particularly useful.
    SynTypeDefnSigRepr =
    /// Indicates the right right-hand-side is a class, struct, interface or other object-model type
    | ObjectModel of SynTypeDefnKind * memberSigs:SynMemberSigs * range:range
    /// Indicates the right right-hand-side is a record, union or other simple type.
    | Simple of SynTypeDefnSimpleRepr * range:range
    | Exception of SynExceptionDefnRepr
    member this.Range =
        match this with
        | ObjectModel (range=m)
        | Simple (range=m) -> m
        | Exception e -> e.Range

and
    [<NoEquality; NoComparison>]
    /// The untyped, unchecked syntax tree for a type definition in a signature
    SynTypeDefnSig =
    /// The information for a type definition in a signature
    | TypeDefnSig of SynComponentInfo * SynTypeDefnSigRepr * SynMemberSigs * range:range

and SynFields = SynField list

and
    [<NoEquality; NoComparison>]
    /// The untyped, unchecked syntax tree for a field declaration in a record or class
    SynField =
    | Field of attrs:SynAttributes * isStatic:bool * Ident option * SynType * bool * xmlDoc:PreXmlDoc * accessibility:SynAccess option * range:range


and
    [<NoEquality; NoComparison>]
    /// The untyped, unchecked syntax tree associated with the name of a type definition or module
    /// in signature or implementation.
    ///
    /// This includes the name, attributes, type parameters, constraints, documentation and accessibility
    /// for a type definition or module. For modules, entries such as the type parameters are
    /// always empty.
    SynComponentInfo =
    | ComponentInfo of attribs:SynAttributes * typeParams:SynTyparDecl list * constraints:SynTypeConstraint list * longId:LongIdent * xmlDoc:PreXmlDoc * preferPostfix:bool * accessibility:SynAccess option * range:range
    member this.Range =
        match this with
        | ComponentInfo (range=m) -> m

and
    [<NoEquality; NoComparison>]
    SynValSig =
    | ValSpfn of
        synAttributes:SynAttributes *
        ident:Ident *
        explicitValDecls:SynValTyparDecls *
        synType:SynType *
        arity:SynValInfo *
        isInline:bool *
        isMutable:bool *
        xmlDoc:PreXmlDoc *
        accessibility:SynAccess option *
        synExpr:SynExpr option *
        range:range

    member x.RangeOfId  = let (ValSpfn(ident=id)) = x in id.idRange
    member x.SynInfo = let (ValSpfn(arity=v)) = x in v
    member x.SynType = let (ValSpfn(synType=ty)) = x in ty

/// The argument names and other metadata for a member or function
and
    [<NoEquality; NoComparison>]
    SynValInfo =
    /// SynValInfo(curriedArgInfos, returnInfo)
    | SynValInfo of SynArgInfo list list * SynArgInfo
    member x.ArgInfos = (let (SynValInfo(args,_)) = x in args)

/// The argument names and other metadata for a parameter for a member or function
and
    [<NoEquality; NoComparison>]
    SynArgInfo =
    | SynArgInfo of SynAttributes * optional:bool *  Ident option

/// The names and other metadata for the type parameters for a member or function
and
    [<NoEquality; NoComparison>]
    SynValTyparDecls =
    | SynValTyparDecls of SynTyparDecl list * bool * constraints:SynTypeConstraint list

/// 'exception E = ... '
and [<NoEquality; NoComparison>]
    SynExceptionDefnRepr =
    | SynExceptionDefnRepr of SynAttributes * SynUnionCase * longId:LongIdent option * xmlDoc:PreXmlDoc * accessiblity:SynAccess option * range:range
    member this.Range = match this with SynExceptionDefnRepr (range=m) -> m

/// 'exception E = ... with ...'
and
    [<NoEquality; NoComparison>]
    SynExceptionDefn =
    | SynExceptionDefn of SynExceptionDefnRepr * SynMemberDefns * range:range
    member this.Range =
        match this with
        | SynExceptionDefn (range=m) -> m

and
    [<NoEquality; NoComparison; RequireQualifiedAccess>]
    SynTypeDefnRepr =
    | ObjectModel  of SynTypeDefnKind * SynMemberDefns * range:range
    | Simple of SynTypeDefnSimpleRepr * range:range
    | Exception of SynExceptionDefnRepr
    member this.Range =
        match this with
        | ObjectModel (range=m)
        | Simple (range=m) -> m
        | Exception t -> t.Range

and
    [<NoEquality; NoComparison>]
    SynTypeDefn =
    | TypeDefn of SynComponentInfo * SynTypeDefnRepr * members:SynMemberDefns * range:range
    member this.Range =
        match this with
        | TypeDefn (range=m) -> m

and
    [<NoEquality; NoComparison; RequireQualifiedAccess>]
    SynMemberDefn =
    | Open of longId:LongIdent * range:range
    | Member of memberDefn:SynBinding * range:range
    /// implicit ctor args as a defn line, 'as' specification
    | ImplicitCtor of accessiblity:SynAccess option * attributes:SynAttributes * ctorArgs:SynSimplePat list * selfIdentifier:Ident option * range:range
    /// inherit <typ>(args...) as base
    | ImplicitInherit of inheritType:SynType * inheritArgs:SynExpr * inheritAlias:Ident option * range:range
    /// LetBindings(bindingList, isStatic, isRecursive, wholeRange)
    ///
    /// localDefns
    | LetBindings of SynBinding list * isStatic:bool * isRecursive:bool * range:range
    | AbstractSlot of SynValSig * MemberFlags * range:range
    | Interface of SynType * SynMemberDefns option  * range:range
    | Inherit of SynType  * Ident option * range:range
    | ValField of SynField  * range:range
    /// A feature that is not implemented
    | NestedType of typeDefn:SynTypeDefn * accessibility:SynAccess option * range:range
    /// SynMemberDefn.AutoProperty (attribs,isStatic,id,tyOpt,propKind,memberFlags,xmlDoc,access,synExpr,mGetSet,mWholeAutoProp).
    ///
    /// F# syntax: 'member val X = expr'
    | AutoProperty of attribs:SynAttributes * isStatic:bool * ident:Ident * typeOpt:SynType option * propKind:MemberKind * memberFlags:(MemberKind -> MemberFlags) * xmlDoc:PreXmlDoc * accessiblity:SynAccess option * synExpr:SynExpr * getSetRange:range option * range:range
    member d.Range =
        match d with
        | SynMemberDefn.Member (range=m)
        | SynMemberDefn.Interface (range=m)
        | SynMemberDefn.Open (range=m)
        | SynMemberDefn.LetBindings (range=m)
        | SynMemberDefn.ImplicitCtor (range=m)
        | SynMemberDefn.ImplicitInherit (range=m)
        | SynMemberDefn.AbstractSlot (range=m)
        | SynMemberDefn.Inherit (range=m)
        | SynMemberDefn.ValField (range=m)
        | SynMemberDefn.AutoProperty (range=m)
        | SynMemberDefn.NestedType (range=m) -> m

and SynMemberDefns = SynMemberDefn list

and
    [<NoEquality; NoComparison;RequireQualifiedAccess>]
    SynModuleDecl =
    | ModuleAbbrev of ident:Ident * longId:LongIdent * range:range
    | NestedModule of SynComponentInfo * isRecursive:bool * SynModuleDecls * bool * range:range
    | Let of bool * SynBinding list * range:range
    | DoExpr of SequencePointInfoForBinding * SynExpr * range:range
    | Types of SynTypeDefn list * range:range
    | Exception of SynExceptionDefn * range:range
    | Open of longDotId:LongIdentWithDots * range:range
    | Attributes of SynAttributes * range:range
    | HashDirective of ParsedHashDirective * range:range
    | NamespaceFragment of SynModuleOrNamespace
    member d.Range =
        match d with
        | SynModuleDecl.ModuleAbbrev (range=m)
        | SynModuleDecl.NestedModule (range=m)
        | SynModuleDecl.Let (range=m)
        | SynModuleDecl.DoExpr (range=m)
        | SynModuleDecl.Types (range=m)
        | SynModuleDecl.Exception (range=m)
        | SynModuleDecl.Open (range=m)
        | SynModuleDecl.HashDirective (range=m)
        | SynModuleDecl.NamespaceFragment (SynModuleOrNamespace (range=m))
        | SynModuleDecl.Attributes (range=m) -> m

and SynModuleDecls = SynModuleDecl list

and
    [<NoEquality; NoComparison>]
    SynExceptionSig =
    | SynExceptionSig of SynExceptionDefnRepr * SynMemberSigs * range:range

and
    [<NoEquality; NoComparison; RequireQualifiedAccess>]
    SynModuleSigDecl =
    | ModuleAbbrev      of ident:Ident * longId:LongIdent * range:range
    | NestedModule      of SynComponentInfo * isRecursive:bool * SynModuleSigDecls * range:range
    | Val               of SynValSig * range:range
    | Types             of SynTypeDefnSig list * range:range
    | Exception         of SynExceptionSig * range:range
    | Open              of longId:LongIdent * range:range
    | HashDirective     of ParsedHashDirective * range:range
    | NamespaceFragment of SynModuleOrNamespaceSig

    member d.Range =
        match d with
        | SynModuleSigDecl.ModuleAbbrev (range=m)
        | SynModuleSigDecl.NestedModule (range=m)
        | SynModuleSigDecl.Val (range=m)
        | SynModuleSigDecl.Types (range=m)
        | SynModuleSigDecl.Exception (range=m)
        | SynModuleSigDecl.Open (range=m)
        | SynModuleSigDecl.NamespaceFragment (SynModuleOrNamespaceSig(range=m))
        | SynModuleSigDecl.HashDirective (range=m) -> m

and SynModuleSigDecls = SynModuleSigDecl list

/// SynModuleOrNamespace(lid,isRec,isModule,decls,xmlDoc,attribs,SynAccess,m)
and
    [<NoEquality; NoComparison>]
    SynModuleOrNamespace =
    | SynModuleOrNamespace of longId:LongIdent * isRecursive:bool * isModule:bool * decls:SynModuleDecls * xmlDoc:PreXmlDoc * attribs:SynAttributes * accessibility:SynAccess option * range:range
    member this.Range =
        match this with
        | SynModuleOrNamespace (range=m) -> m

and
    [<NoEquality; NoComparison>]
    SynModuleOrNamespaceSig =
    | SynModuleOrNamespaceSig of longId:LongIdent * isRecursive:bool * isModule:bool * SynModuleSigDecls * xmlDoc:PreXmlDoc * attribs:SynAttributes * accessibility:SynAccess option * range:range

and [<NoEquality; NoComparison>]
    ParsedHashDirective =
    | ParsedHashDirective of string * string list * range:range

[<NoEquality; NoComparison; RequireQualifiedAccess>]
type ParsedImplFileFragment =
    | AnonModule of SynModuleDecls * range:range
    | NamedModule of SynModuleOrNamespace
    | NamespaceFragment of longId:LongIdent * bool * bool * SynModuleDecls * xmlDoc:PreXmlDoc * SynAttributes * range:range

[<NoEquality; NoComparison; RequireQualifiedAccess>]
type ParsedSigFileFragment =
    | AnonModule of SynModuleSigDecls * range:range
    | NamedModule of SynModuleOrNamespaceSig
    | NamespaceFragment of longId:LongIdent * bool * bool * SynModuleSigDecls * xmlDoc:PreXmlDoc * SynAttributes * range:range

[<NoEquality; NoComparison>]
type ParsedFsiInteraction =
    | IDefns of SynModuleDecl list * range:range
    | IHash  of ParsedHashDirective * range:range

[<NoEquality; NoComparison>]
type ParsedImplFile =
    | ParsedImplFile of hashDirectives:ParsedHashDirective list * ParsedImplFileFragment list

[<NoEquality; NoComparison>]
type ParsedSigFile =
    | ParsedSigFile of hashDirectives:ParsedHashDirective list * ParsedSigFileFragment list

//----------------------------------------------------------------------
// AST and parsing utilities.
//----------------------------------------------------------------------

let ident (s,r) = new Ident(s,r)
let textOfId (id:Ident) = id.idText
let pathOfLid lid = List.map textOfId lid
let arrPathOfLid lid = Array.ofList (pathOfLid lid)
let textOfPath path = String.concat "." path
let textOfLid lid = textOfPath (pathOfLid lid)

let rangeOfLid (lid: Ident list) =
    match lid with
    | [] -> failwith "rangeOfLid"
    | [id] -> id.idRange
    | h::t -> unionRanges h.idRange (List.last t).idRange

[<RequireQualifiedAccess>]
type ScopedPragma =
   | WarningOff of range:range * int
   // Note: this type may be extended in the future with optimization on/off switches etc.

// These are the results of parsing + folding in the implicit file name
/// ImplFile(modname,isScript,qualName,hashDirectives,modules,isLastCompiland)

/// QualifiedNameOfFile acts to fully-qualify module specifications and implementations,
/// most importantly the ones that simply contribute fragments to a namespace (i.e. the ParsedSigFileFragment.NamespaceFragment case)
/// There may be multiple such fragments in a single assembly.  There may thus also
/// be multiple matching pairs of these in an assembly, all contributing types to the same
/// namespace.
[<NoEquality; NoComparison>]
type QualifiedNameOfFile =
    | QualifiedNameOfFile of Ident
    member x.Text = (let (QualifiedNameOfFile t) = x in t.idText)
    member x.Id = (let (QualifiedNameOfFile t) = x in t)
    member x.Range = (let (QualifiedNameOfFile t) = x in t.idRange)

[<NoEquality; NoComparison>]
type ParsedImplFileInput =
    | ParsedImplFileInput of 
        fileName : string * 
        isScript : bool * 
        qualifiedNameOfFile : QualifiedNameOfFile * 
        scopedPragmas : ScopedPragma list * 
        hashDirectives : ParsedHashDirective list * 
        modules : SynModuleOrNamespace list * 
        ((* isLastCompiland *) bool * (* isExe *) bool)

[<NoEquality; NoComparison>]
type ParsedSigFileInput =
    | ParsedSigFileInput of 
        fileName : string * 
        qualifiedNameOfFile : QualifiedNameOfFile * 
        scopedPragmas : ScopedPragma list * 
        hashDirectives : ParsedHashDirective list * 
        modules : SynModuleOrNamespaceSig list

[<NoEquality; NoComparison; RequireQualifiedAccess>]
type ParsedInput =
    | ImplFile of ParsedImplFileInput
    | SigFile of ParsedSigFileInput

    member inp.Range =
        match inp with
        | ParsedInput.ImplFile (ParsedImplFileInput (modules=SynModuleOrNamespace(range=m) :: _))
        | ParsedInput.SigFile (ParsedSigFileInput (modules=SynModuleOrNamespaceSig(range=m) :: _)) -> m
        | ParsedInput.ImplFile (ParsedImplFileInput (fileName=filename))
        | ParsedInput.SigFile (ParsedSigFileInput (fileName=filename)) ->
#if DEBUG
            assert("" = "compiler expects ParsedInput.ImplFile and ParsedInput.SigFile to have at least one fragment, 4488")
#endif
            rangeN filename 0 (* There are no implementations, e.g. due to errors, so return a default range for the file *)


//----------------------------------------------------------------------
// Construct syntactic AST nodes
//-----------------------------------------------------------------------

// REVIEW: get rid of this global state
type SynArgNameGenerator() =
    let mutable count = 0
    let generatedArgNamePrefix = "_arg"

    member __.New() : string = count <- count + 1; generatedArgNamePrefix + string count
    member __.Reset() = count <- 0

//----------------------------------------------------------------------
// Construct syntactic AST nodes
//-----------------------------------------------------------------------


let mkSynId m s = Ident(s,m)
let pathToSynLid m p = List.map (mkSynId m) p
let mkSynIdGet m n = SynExpr.Ident(mkSynId m n)
let mkSynLidGet m path n =
    let lid = pathToSynLid m path @ [mkSynId m n]
    let dots = List.replicate (lid.Length - 1) m
    SynExpr.LongIdent(false,LongIdentWithDots(lid,dots),None,m)
let mkSynIdGetWithAlt m id altInfo =
    match altInfo with
    | None -> SynExpr.Ident id
    | _ -> SynExpr.LongIdent(false,LongIdentWithDots([id],[]),altInfo,m)

let mkSynSimplePatVar isOpt id = SynSimplePat.Id (id,None,false,false,isOpt,id.idRange)
let mkSynCompGenSimplePatVar id = SynSimplePat.Id (id,None,true,false,false,id.idRange)

/// Match a long identifier, including the case for single identifiers which gets a more optimized node in the syntax tree.
let (|LongOrSingleIdent|_|) inp =
    match inp with
    | SynExpr.LongIdent(isOpt,lidwd,altId,_m) -> Some (isOpt,lidwd,altId,lidwd.RangeSansAnyExtraDot)
    | SynExpr.Ident id -> Some (false,LongIdentWithDots([id],[]),None,id.idRange)
    | _ -> None

let (|SingleIdent|_|) inp =
    match inp with
    | SynExpr.LongIdent(false,LongIdentWithDots([id],_),None,_) -> Some id
    | SynExpr.Ident id -> Some id
    | _ -> None

/// This affects placement of sequence points
let rec IsControlFlowExpression e =
    match e with
    | SynExpr.ObjExpr _
    | SynExpr.Lambda _
    | SynExpr.LetOrUse _
    | SynExpr.Sequential _
    // Treat "ident { ... }" as a control flow expression
    | SynExpr.App (_, _, SynExpr.Ident _, SynExpr.CompExpr _,_)
    | SynExpr.IfThenElse _
    | SynExpr.LetOrUseBang _
    | SynExpr.Match _
    | SynExpr.TryWith _
    | SynExpr.TryFinally _
    | SynExpr.For _
    | SynExpr.ForEach _
    | SynExpr.While _ -> true
    | SynExpr.Typed(e,_,_) -> IsControlFlowExpression e
    | _ -> false

let mkAnonField (ty: SynType) = Field([],false,None,ty,false,PreXmlDoc.Empty,None,ty.Range)
let mkNamedField (ident, ty: SynType) = Field([],false,Some ident,ty,false,PreXmlDoc.Empty,None,ty.Range)

let mkSynPatVar vis (id:Ident) = SynPat.Named (SynPat.Wild id.idRange,id,false,vis,id.idRange)
let mkSynThisPatVar (id:Ident) = SynPat.Named (SynPat.Wild id.idRange,id,true,None,id.idRange)
let mkSynPatMaybeVar lidwd vis m =  SynPat.LongIdent (lidwd,None,None,SynConstructorArgs.Pats [],vis,m)

/// Extract the argument for patterns corresponding to the declaration of 'new ... = ...'
let (|SynPatForConstructorDecl|_|) x =
    match x with
    | SynPat.LongIdent (LongIdentWithDots([_],_),_,_, SynConstructorArgs.Pats [arg],_,_) -> Some arg
    | _ -> None

/// Recognize the '()' in 'new()'
let (|SynPatForNullaryArgs|_|) x =
    match x with
    | SynPat.Paren(SynPat.Const(SynConst.Unit,_),_) -> Some()
    | _ -> None

let (|SynExprErrorSkip|) (p:SynExpr) =
    match p with
    | SynExpr.FromParseError(p,_) -> p
    | _ -> p

let (|SynExprParen|_|) (e:SynExpr) =
    match e with
    | SynExpr.Paren(SynExprErrorSkip e,a,b,c) -> Some (e,a,b,c)
    | _ -> None

let (|SynPatErrorSkip|) (p:SynPat) =
    match p with
    | SynPat.FromParseError(p,_) -> p
    | _ -> p

/// Push non-simple parts of a patten match over onto the r.h.s. of a lambda.
/// Return a simple pattern and a function to build a match on the r.h.s. if the pattern is complex
let rec SimplePatOfPat (synArgNameGenerator: SynArgNameGenerator) p =
    match p with
    | SynPat.Typed(p',ty,m) ->
        let p2,laterf = SimplePatOfPat synArgNameGenerator p'
        SynSimplePat.Typed(p2,ty,m),
        laterf
    | SynPat.Attrib(p',attribs,m) ->
        let p2,laterf = SimplePatOfPat synArgNameGenerator p'
        SynSimplePat.Attrib(p2,attribs,m),
        laterf
    | SynPat.Named (SynPat.Wild _, v,thisv,_,m) ->
        SynSimplePat.Id (v,None,false,thisv,false,m),
        None
    | SynPat.OptionalVal (v,m) ->
        SynSimplePat.Id (v,None,false,false,true,m),
        None
    | SynPat.Paren (p,_) -> SimplePatOfPat synArgNameGenerator p
    | SynPat.FromParseError (p,_) -> SimplePatOfPat synArgNameGenerator p
    | _ ->
        let m = p.Range
        let isCompGen,altNameRefCell,id,item =
            match p with
            | SynPat.LongIdent(LongIdentWithDots([id],_),_,None, SynConstructorArgs.Pats [],None,_) ->
                // The pattern is 'V' or some other capitalized identifier.
                // It may be a real variable, in which case we want to maintain its name.
                // But it may also be a nullary union case or some other identifier.
                // In this case, we want to use an alternate compiler generated name for the hidden variable.
                let altNameRefCell = Some (ref (Undecided (mkSynId m (synArgNameGenerator.New()))))
                let item = mkSynIdGetWithAlt m id altNameRefCell
                false,altNameRefCell,id,item
            | _ ->
                let nm = synArgNameGenerator.New()
                let id = mkSynId m nm
                let item = mkSynIdGet m nm
                true,None,id,item
        SynSimplePat.Id (id,altNameRefCell,isCompGen,false,false,id.idRange),
        Some (fun e ->
                let clause = Clause(p,None,e,m,SuppressSequencePointAtTarget)
                SynExpr.Match(NoSequencePointAtInvisibleBinding,item,[clause],false,clause.Range))

let appFunOpt funOpt x = match funOpt with None -> x | Some f -> f x
let composeFunOpt funOpt1 funOpt2 = match funOpt2 with None -> funOpt1 | Some f -> Some (fun x -> appFunOpt funOpt1 (f x))
let rec SimplePatsOfPat synArgNameGenerator p =
    match p with
    | SynPat.FromParseError (p,_) -> SimplePatsOfPat synArgNameGenerator p
    | SynPat.Typed(p',ty,m) ->
        let p2,laterf = SimplePatsOfPat synArgNameGenerator p'
        SynSimplePats.Typed(p2,ty,m),
        laterf
//    | SynPat.Paren (p,m) -> SimplePatsOfPat synArgNameGenerator p
    | SynPat.Tuple (ps,m)
    | SynPat.Paren(SynPat.Tuple (ps,m),_) ->
        let ps2,laterf =
          List.foldBack
            (fun (p',rhsf) (ps',rhsf') ->
              p'::ps',
              (composeFunOpt rhsf rhsf'))
            (List.map (SimplePatOfPat synArgNameGenerator) ps)
            ([], None)
        SynSimplePats.SimplePats (ps2,m),
        laterf
    | SynPat.Paren(SynPat.Const (SynConst.Unit,m),_)
    | SynPat.Const (SynConst.Unit,m) ->
        SynSimplePats.SimplePats ([],m),
        None
    | _ ->
        let m = p.Range
        let sp,laterf = SimplePatOfPat synArgNameGenerator p
        SynSimplePats.SimplePats ([sp],m),laterf

let PushPatternToExpr synArgNameGenerator isMember pat (rhs: SynExpr) =
    let nowpats,laterf = SimplePatsOfPat synArgNameGenerator pat
    nowpats, SynExpr.Lambda (isMember,false,nowpats, appFunOpt laterf rhs,rhs.Range)

let private isSimplePattern pat =
    let _nowpats,laterf = SimplePatsOfPat (SynArgNameGenerator()) pat
    Option.isNone laterf

/// "fun (UnionCase x) (UnionCase y) -> body"
///       ==>
///   "fun tmp1 tmp2 ->
///        let (UnionCase x) = tmp1 in
///        let (UnionCase y) = tmp2 in
///        body"
let PushCurriedPatternsToExpr synArgNameGenerator wholem isMember pats rhs =
    // Two phases
    // First phase: Fold back, from right to left, pushing patterns into r.h.s. expr
    let spatsl,rhs =
        (pats, ([],rhs))
           ||> List.foldBack (fun arg (spatsl,body) ->
              let spats,bodyf = SimplePatsOfPat synArgNameGenerator arg
              // accumulate the body. This builds "let (UnionCase y) = tmp2 in body"
              let body = appFunOpt bodyf body
              // accumulate the patterns
              let spatsl = spats::spatsl
              (spatsl,body))
    // Second phase: build lambdas. Mark subsequent ones with "true" indicating they are part of an iterated sequence of lambdas
    let expr =
        match spatsl with
        | [] -> rhs
        | h::t ->
            let expr = List.foldBack (fun spats e -> SynExpr.Lambda (isMember,true,spats, e,wholem)) t rhs
            let expr = SynExpr.Lambda (isMember,false,h, expr,wholem)
            expr
    spatsl,expr

/// Helper for parsing the inline IL fragments.
#if NO_INLINE_IL_PARSER
let ParseAssemblyCodeInstructions _s m =
    errorR(Error((193,"Inline IL not valid in a hosted environment"),m))
    [| |]
#else
let ParseAssemblyCodeInstructions s m =
    try Microsoft.FSharp.Compiler.AbstractIL.Internal.AsciiParser.ilInstrs
           Microsoft.FSharp.Compiler.AbstractIL.Internal.AsciiLexer.token
           (UnicodeLexing.StringAsLexbuf s)
    with RecoverableParseError ->
      errorR(Error(FSComp.SR.astParseEmbeddedILError(), m)); [| |]
#endif


/// Helper for parsing the inline IL fragments.
#if NO_INLINE_IL_PARSER
let ParseAssemblyCodeType _s m =
    errorR(Error((193,"Inline IL not valid in a hosted environment"),m))
    IL.EcmaMscorlibILGlobals.typ_Object
#else
let ParseAssemblyCodeType s m =
    try Microsoft.FSharp.Compiler.AbstractIL.Internal.AsciiParser.ilType
           Microsoft.FSharp.Compiler.AbstractIL.Internal.AsciiLexer.token
           (UnicodeLexing.StringAsLexbuf s)
    with RecoverableParseError ->
      errorR(Error(FSComp.SR.astParseEmbeddedILTypeError(),m));
      IL.EcmaMscorlibILGlobals.typ_Object
#endif

//------------------------------------------------------------------------
// AST constructors
//------------------------------------------------------------------------

let opNameParenGet  = CompileOpName parenGet
let opNameQMark = CompileOpName qmark
let mkSynOperator opm oper = mkSynIdGet opm (CompileOpName oper)

let mkSynInfix opm (l:SynExpr) oper (r:SynExpr) =
    let firstTwoRange = unionRanges l.Range opm
    let wholeRange = unionRanges l.Range r.Range
    SynExpr.App (ExprAtomicFlag.NonAtomic, false, SynExpr.App (ExprAtomicFlag.NonAtomic, true, mkSynOperator opm oper, l, firstTwoRange), r, wholeRange)
let mkSynBifix   m oper x1 x2 = SynExpr.App (ExprAtomicFlag.NonAtomic, false, SynExpr.App (ExprAtomicFlag.NonAtomic, true, mkSynOperator m oper,x1,m), x2,m)
let mkSynTrifix  m oper x1 x2 x3 = SynExpr.App (ExprAtomicFlag.NonAtomic, false, SynExpr.App (ExprAtomicFlag.NonAtomic, false, SynExpr.App (ExprAtomicFlag.NonAtomic, true, mkSynOperator m oper,x1,m), x2,m), x3,m)
let mkSynQuadfix m oper x1 x2 x3 x4 = SynExpr.App (ExprAtomicFlag.NonAtomic, false, SynExpr.App (ExprAtomicFlag.NonAtomic, false, SynExpr.App (ExprAtomicFlag.NonAtomic, false, SynExpr.App (ExprAtomicFlag.NonAtomic, true, mkSynOperator m oper,x1,m), x2,m), x3,m),x4,m)
let mkSynQuinfix m oper x1 x2 x3 x4 x5 = SynExpr.App (ExprAtomicFlag.NonAtomic, false, SynExpr.App (ExprAtomicFlag.NonAtomic, false, SynExpr.App (ExprAtomicFlag.NonAtomic, false, SynExpr.App (ExprAtomicFlag.NonAtomic, false, SynExpr.App (ExprAtomicFlag.NonAtomic, true, mkSynOperator m oper,x1,m), x2,m), x3,m),x4,m),x5,m)
let mkSynPrefix opm m oper x = SynExpr.App (ExprAtomicFlag.NonAtomic, false, mkSynOperator opm oper, x,m)
let mkSynCaseName m n = [mkSynId m (CompileOpName n)]

let mkSynApp1 f x1 m = SynExpr.App(ExprAtomicFlag.NonAtomic,false,f,x1,m)
let mkSynApp2 f x1 x2 m = mkSynApp1 (mkSynApp1 f x1 m) x2 m
let mkSynApp3 f x1 x2 x3 m = mkSynApp1 (mkSynApp2 f x1 x2 m) x3 m
let mkSynApp4 f x1 x2 x3 x4 m = mkSynApp1 (mkSynApp3 f x1 x2 x3 m) x4 m
let mkSynApp5 f x1 x2 x3 x4 x5 m = mkSynApp1 (mkSynApp4 f x1 x2 x3 x4 m) x5 m
let mkSynDotParenSet  m a b c = mkSynTrifix m parenSet a b c
let mkSynDotBrackGet  m mDot a b   = SynExpr.DotIndexedGet(a,[SynIndexerArg.One b],mDot,m)
let mkSynQMarkSet m a b c = mkSynTrifix m qmarkSet a b c
let mkSynDotBrackSliceGet  m mDot arr sliceArg = SynExpr.DotIndexedGet(arr,[sliceArg],mDot,m)

let mkSynDotBrackSeqSliceGet  m mDot arr (argslist:list<SynIndexerArg>) =
    let notsliced=[ for arg in argslist do
                       match arg with
                       | SynIndexerArg.One x -> yield x
                       | _ -> () ]
    if notsliced.Length = argslist.Length then
        SynExpr.DotIndexedGet(arr,[SynIndexerArg.One (SynExpr.Tuple(notsliced,[],unionRanges (List.head notsliced).Range (List.last notsliced).Range))],mDot,m)
    else
        SynExpr.DotIndexedGet(arr,argslist,mDot,m)

let mkSynDotParenGet lhsm dotm a b   =
    match b with
    | SynExpr.Tuple ([_;_],_,_)   -> errorR(Deprecated(FSComp.SR.astDeprecatedIndexerNotation(),lhsm)) ; SynExpr.Const(SynConst.Unit,lhsm)
    | SynExpr.Tuple ([_;_;_],_,_) -> errorR(Deprecated(FSComp.SR.astDeprecatedIndexerNotation(),lhsm)) ; SynExpr.Const(SynConst.Unit,lhsm)
    | _ -> mkSynInfix dotm a parenGet b

let mkSynUnit m = SynExpr.Const(SynConst.Unit,m)
let mkSynUnitPat m = SynPat.Const(SynConst.Unit,m)
let mkSynDelay m e = SynExpr.Lambda (false,false,SynSimplePats.SimplePats ([mkSynCompGenSimplePatVar (mkSynId m "unitVar")],m), e, m)

let mkSynAssign (l: SynExpr) (r: SynExpr) =
    let m = unionRanges l.Range r.Range
    match l with
    //| SynExpr.Paren(l2,m2)  -> mkSynAssign m l2 r
    | LongOrSingleIdent(false,v,None,_)  -> SynExpr.LongIdentSet (v,r,m)
    | SynExpr.DotGet(e,_,v,_)  -> SynExpr.DotSet (e,v,r,m)
    | SynExpr.DotIndexedGet(e1,e2,mDot,mLeft)  -> SynExpr.DotIndexedSet (e1,e2,r,mLeft,mDot,m)
    | SynExpr.LibraryOnlyUnionCaseFieldGet (x,y,z,_) -> SynExpr.LibraryOnlyUnionCaseFieldSet (x,y,z,r,m)
    | SynExpr.App (_, _, SynExpr.App(_, _, SingleIdent(nm), a, _),b,_) when nm.idText = opNameQMark ->
        mkSynQMarkSet m a b r
    | SynExpr.App (_, _, SynExpr.App(_, _, SingleIdent(nm), a, _),b,_) when nm.idText = opNameParenGet ->
        mkSynDotParenSet m a b r
    | SynExpr.App (_, _, SynExpr.LongIdent(false,v,None,_),x,_)  -> SynExpr.NamedIndexedPropertySet (v,x,r,m)
    | SynExpr.App (_, _, SynExpr.DotGet(e,_,v,_),x,_)  -> SynExpr.DotNamedIndexedPropertySet (e,v,x,r,m)
    |   _ -> errorR(Error(FSComp.SR.astInvalidExprLeftHandOfAssignment(), m));  l  // return just the LHS, so the typechecker can see it and capture expression typings that may be useful for dot lookups

let rec mkSynDot dotm m l r =
    match l with
    | SynExpr.LongIdent(isOpt,LongIdentWithDots(lid,dots),None,_) ->
        SynExpr.LongIdent(isOpt,LongIdentWithDots(lid@[r],dots@[dotm]),None,m) // REVIEW: MEMORY PERFORMANCE: This list operation is memory intensive (we create a lot of these list nodes) - an ImmutableArray would be better here
    | SynExpr.Ident id ->
        SynExpr.LongIdent(false,LongIdentWithDots([id;r],[dotm]),None,m)
    | SynExpr.DotGet(e,dm,LongIdentWithDots(lid,dots),_) ->
        SynExpr.DotGet(e,dm,LongIdentWithDots(lid@[r],dots@[dotm]),m)// REVIEW: MEMORY PERFORMANCE: This is memory intensive (we create a lot of these list nodes) - an ImmutableArray would be better here
    | expr ->
        SynExpr.DotGet(expr,dotm,LongIdentWithDots([r],[]),m)

let rec mkSynDotMissing dotm m l =
    match l with
    | SynExpr.LongIdent(isOpt,LongIdentWithDots(lid,dots),None,_) ->
        SynExpr.LongIdent(isOpt,LongIdentWithDots(lid,dots@[dotm]),None,m) // REVIEW: MEMORY PERFORMANCE: This list operation is memory intensive (we create a lot of these list nodes) - an ImmutableArray would be better here
    | SynExpr.Ident id ->
        SynExpr.LongIdent(false,LongIdentWithDots([id],[dotm]),None,m)
    | SynExpr.DotGet(e,dm,LongIdentWithDots(lid,dots),_) ->
        SynExpr.DotGet(e,dm,LongIdentWithDots(lid,dots@[dotm]),m)// REVIEW: MEMORY PERFORMANCE: This is memory intensive (we create a lot of these list nodes) - an ImmutableArray would be better here
    | expr ->
        SynExpr.DiscardAfterMissingQualificationAfterDot(expr,m)

let mkSynFunMatchLambdas synArgNameGenerator isMember wholem ps e =
    let _,e =  PushCurriedPatternsToExpr synArgNameGenerator wholem isMember ps e
    e


// error recovery - the contract is that these expressions can only be produced if an error has already been reported
// (as a result, future checking may choose not to report errors involving these, to prevent noisy cascade errors)
let arbExpr(debugStr,range:range) = SynExpr.ArbitraryAfterError(debugStr,range.MakeSynthetic())
type SynExpr with
    member this.IsArbExprAndThusAlreadyReportedError =
        match this with
        | SynExpr.ArbitraryAfterError _ -> true
        | _ -> false

/// The syntactic elements associated with the "return" of a function or method. Some of this is
/// mostly dummy information to make the return element look like an argument,
/// the important thing is that (a) you can give a return type for the function or method, and
/// (b) you can associate .NET attributes to return of a function or method and these get stored in .NET metadata.
type SynReturnInfo = SynReturnInfo of (SynType * SynArgInfo) * range:range


/// Operations related to the syntactic analysis of arguments of value, function and member definitions and signatures.
///
/// Function and member definitions have strongly syntactically constrained arities.  We infer
/// the arity from the syntax.
///
/// For example, we record the arity for:
/// StaticProperty --> [1]               -- for unit arg
/// this.InstanceProperty --> [1;1]        -- for unit arg
/// StaticMethod(args) --> map InferSynArgInfoFromSimplePat args
/// this.InstanceMethod() --> 1 :: map InferSynArgInfoFromSimplePat args
/// this.InstanceProperty with get(argpat) --> 1 :: [InferSynArgInfoFromSimplePat argpat]
/// StaticProperty with get(argpat) --> [InferSynArgInfoFromSimplePat argpat]
/// this.InstanceProperty with get() --> 1 :: [InferSynArgInfoFromSimplePat argpat]
/// StaticProperty with get() --> [InferSynArgInfoFromSimplePat argpat]
///
/// this.InstanceProperty with set(argpat)(v) --> 1 :: [InferSynArgInfoFromSimplePat argpat; 1]
/// StaticProperty with set(argpat)(v) --> [InferSynArgInfoFromSimplePat argpat; 1]
/// this.InstanceProperty with set(v) --> 1 :: [1]
/// StaticProperty with set(v) --> [1]
module SynInfo =
    /// The argument information for an argument without a name
    let unnamedTopArg1 = SynArgInfo([],false,None)

    /// The argument information for a curried argument without a name
    let unnamedTopArg = [unnamedTopArg1]

    /// The argument information for a '()' argument
    let unitArgData = unnamedTopArg

    /// The 'argument' information for a return value where no attributes are given for the return value (the normal case)
    let unnamedRetVal = SynArgInfo([],false,None)

    /// The 'argument' information for the 'this'/'self' parameter in the cases where it is not given explicitly
    let selfMetadata = unnamedTopArg

    /// Determine if a syntactic information represents a member without arguments (which is implicitly a property getter)
    let HasNoArgs (SynValInfo(args,_)) = isNil args

    /// Check if one particular argument is an optional argument. Used when adjusting the
    /// types of optional arguments for function and member signatures.
    let IsOptionalArg (SynArgInfo(_,isOpt,_)) = isOpt

    /// Check if there are any optional arguments in the syntactic argument information. Used when adjusting the
    /// types of optional arguments for function and member signatures.
    let HasOptionalArgs (SynValInfo(args,_)) = List.exists (List.exists IsOptionalArg) args

    /// Add a parameter entry to the syntactic value information to represent the '()' argument to a property getter. This is
    /// used for the implicit '()' argument in property getter signature specifications.
    let IncorporateEmptyTupledArgForPropertyGetter (SynValInfo(args,retInfo)) = SynValInfo([]::args,retInfo)

    /// Add a parameter entry to the syntactic value information to represent the 'this' argument. This is
    /// used for the implicit 'this' argument in member signature specifications.
    let IncorporateSelfArg (SynValInfo(args,retInfo)) = SynValInfo(selfMetadata::args,retInfo)

    /// Add a parameter entry to the syntactic value information to represent the value argument for a property setter. This is
    /// used for the implicit value argument in property setter signature specifications.
    let IncorporateSetterArg (SynValInfo(args,retInfo)) =
         let args =
             match args with
             | [] -> [unnamedTopArg]
             | [arg] -> [arg@[unnamedTopArg1]]
             | _ -> failwith "invalid setter type"
         SynValInfo(args,retInfo)

    /// Get the argument counts for each curried argument group. Used in some adhoc places in tc.fs.
    let AritiesOfArgs (SynValInfo(args,_)) = List.map List.length args

    /// Get the argument attributes from the syntactic information for an argument.
    let AttribsOfArgData (SynArgInfo(attribs,_,_)) = attribs

    /// Infer the syntactic argument info for a single argument from a simple pattern.
    let rec InferSynArgInfoFromSimplePat attribs p =
        match p with
        | SynSimplePat.Id(nm,_,isCompGen,_,isOpt,_) ->
           SynArgInfo(attribs, isOpt, (if isCompGen then None else Some nm))
        | SynSimplePat.Typed(a,_,_) -> InferSynArgInfoFromSimplePat attribs a
        | SynSimplePat.Attrib(a,attribs2,_) -> InferSynArgInfoFromSimplePat (attribs @ attribs2) a

    /// Infer the syntactic argument info for one or more arguments one or more simple patterns.
    let rec InferSynArgInfoFromSimplePats x =
        match x with
        | SynSimplePats.SimplePats(ps,_) -> List.map (InferSynArgInfoFromSimplePat []) ps
        | SynSimplePats.Typed(ps,_,_) -> InferSynArgInfoFromSimplePats ps

    /// Infer the syntactic argument info for one or more arguments a pattern.
    let InferSynArgInfoFromPat p =
        // It is ok to use a fresh SynArgNameGenerator here, because compiler generated names are filtered from SynArgInfo, see InferSynArgInfoFromSimplePat above
        let sp,_ = SimplePatsOfPat (SynArgNameGenerator()) p
        InferSynArgInfoFromSimplePats sp

    /// Make sure only a solitary unit argument has unit elimination
    let AdjustArgsForUnitElimination infosForArgs =
        match infosForArgs with
        | [[]] -> infosForArgs
        | _ -> infosForArgs |> List.map (function [] -> unitArgData | x -> x)

    /// Transform a property declared using '[static] member P = expr' to a method taking a "unit" argument.
    /// This is similar to IncorporateEmptyTupledArgForPropertyGetter, but applies to member definitions
    /// rather than member signatures.
    let AdjustMemberArgs memFlags infosForArgs =
        match infosForArgs with
        | [] when memFlags=MemberKind.Member -> [] :: infosForArgs
        | _ -> infosForArgs

    /// For 'let' definitions, we infer syntactic argument information from the r.h.s. of a definition, if it
    /// is an immediate 'fun ... -> ...' or 'function ...' expression. This is noted in the F# language specification.
    /// This does not apply to member definitions.
    let InferLambdaArgs origRhsExpr =
        let rec loop e =
            match e with
            | SynExpr.Lambda(false,_,spats,rest,_) ->
                InferSynArgInfoFromSimplePats spats :: loop rest
            | _ -> []
        loop origRhsExpr

    let InferSynReturnData (retInfo: SynReturnInfo option) =
        match retInfo with
        | None -> unnamedRetVal
        | Some(SynReturnInfo((_,retInfo),_)) -> retInfo

    let private emptySynValInfo = SynValInfo([],unnamedRetVal)

    let emptySynValData = SynValData(None,emptySynValInfo,None)

    /// Infer the syntactic information for a 'let' or 'member' definition, based on the argument pattern,
    /// any declared return information (e.g. .NET attributes on the return element), and the r.h.s. expression
    /// in the case of 'let' definitions.
    let InferSynValData (memberFlagsOpt, pat, retInfo, origRhsExpr) =

        let infosForExplicitArgs =
            match pat with
            | Some(SynPat.LongIdent(_,_,_, SynConstructorArgs.Pats curriedArgs,_,_)) -> List.map InferSynArgInfoFromPat curriedArgs
            | _ -> []

        let explicitArgsAreSimple =
            match pat with
            | Some(SynPat.LongIdent(_,_,_, SynConstructorArgs.Pats curriedArgs,_,_)) -> List.forall isSimplePattern curriedArgs
            | _ -> true

        let retInfo = InferSynReturnData retInfo

        match memberFlagsOpt with
        | None ->
            let infosForLambdaArgs = InferLambdaArgs origRhsExpr
            let infosForArgs = infosForExplicitArgs @ (if explicitArgsAreSimple then infosForLambdaArgs else [])
            let infosForArgs = AdjustArgsForUnitElimination infosForArgs
            SynValData(None,SynValInfo(infosForArgs,retInfo),None)

        | Some memFlags  ->
            let infosForObjArgs =
                if memFlags.IsInstance then [ selfMetadata ] else []

            let infosForArgs = AdjustMemberArgs memFlags.MemberKind infosForExplicitArgs
            let infosForArgs = AdjustArgsForUnitElimination infosForArgs

            let argInfos = infosForObjArgs @ infosForArgs
            SynValData(Some(memFlags),SynValInfo(argInfos,retInfo),None)



let mkSynBindingRhs staticOptimizations rhsExpr mRhs retInfo =
    let rhsExpr = List.foldBack (fun (c,e1) e2 -> SynExpr.LibraryOnlyStaticOptimization (c,e1,e2,mRhs)) staticOptimizations rhsExpr
    let rhsExpr,retTyOpt =
        match retInfo with
        | Some (SynReturnInfo((ty,SynArgInfo(rattribs,_,_)),tym)) -> SynExpr.Typed(rhsExpr,ty,rhsExpr.Range), Some(SynBindingReturnInfo(ty,tym,rattribs) )
        | None -> rhsExpr,None
    rhsExpr,retTyOpt

let mkSynBinding (xmlDoc,headPat) (vis,isInline,isMutable,mBind,spBind,retInfo,origRhsExpr,mRhs,staticOptimizations,attrs,memberFlagsOpt) =
    let info = SynInfo.InferSynValData (memberFlagsOpt, Some headPat, retInfo, origRhsExpr)
    let rhsExpr,retTyOpt = mkSynBindingRhs staticOptimizations origRhsExpr mRhs retInfo
    Binding (vis,NormalBinding,isInline,isMutable,attrs,xmlDoc,info,headPat,retTyOpt,rhsExpr,mBind,spBind)

let NonVirtualMemberFlags k = { MemberKind=k;                           IsInstance=true;  IsDispatchSlot=false; IsOverrideOrExplicitImpl=false; IsFinal=false }
let CtorMemberFlags =         { MemberKind=MemberKind.Constructor;      IsInstance=false; IsDispatchSlot=false; IsOverrideOrExplicitImpl=false; IsFinal=false }
let ClassCtorMemberFlags =    { MemberKind=MemberKind.ClassConstructor; IsInstance=false; IsDispatchSlot=false; IsOverrideOrExplicitImpl=false; IsFinal=false }
let OverrideMemberFlags k =   { MemberKind=k;                           IsInstance=true;  IsDispatchSlot=false; IsOverrideOrExplicitImpl=true;  IsFinal=false }
let AbstractMemberFlags k =   { MemberKind=k;                           IsInstance=true;  IsDispatchSlot=true;  IsOverrideOrExplicitImpl=false; IsFinal=false }
let StaticMemberFlags k =     { MemberKind=k;                           IsInstance=false; IsDispatchSlot=false; IsOverrideOrExplicitImpl=false; IsFinal=false }

let inferredTyparDecls = SynValTyparDecls([],true,[])
let noInferredTypars = SynValTyparDecls([],false,[])

//------------------------------------------------------------------------
// Lexer args: status of #if/#endif processing.
//------------------------------------------------------------------------

type LexerIfdefStackEntry = IfDefIf | IfDefElse
type LexerIfdefStackEntries = (LexerIfdefStackEntry * range) list
type LexerIfdefStack = LexerIfdefStackEntries ref

/// Specifies how the 'endline' function in the lexer should continue after
/// it reaches end of line or eof. The options are to continue with 'token' function
/// or to continue with 'skip' function.
type LexerEndlineContinuation =
    | Token of LexerIfdefStackEntries
    | Skip of LexerIfdefStackEntries * int * range:range
    member x.LexerIfdefStack =
      match x with
      | LexerEndlineContinuation.Token(ifd)
      | LexerEndlineContinuation.Skip(ifd, _, _) -> ifd

type LexerIfdefExpression =
    | IfdefAnd          of LexerIfdefExpression*LexerIfdefExpression
    | IfdefOr           of LexerIfdefExpression*LexerIfdefExpression
    | IfdefNot          of LexerIfdefExpression
    | IfdefId           of string

let rec LexerIfdefEval (lookup : string -> bool) = function
    | IfdefAnd (l,r)    -> (LexerIfdefEval lookup l) && (LexerIfdefEval lookup r)
    | IfdefOr (l,r)     -> (LexerIfdefEval lookup l) || (LexerIfdefEval lookup r)
    | IfdefNot e        -> not (LexerIfdefEval lookup e)
    | IfdefId id        -> lookup id

/// The parser defines a number of tokens for whitespace and
/// comments eliminated by the lexer.  These carry a specification of
/// a continuation for the lexer for continued processing after we've dealt with
/// the whitespace.
[<RequireQualifiedAccess>]
[<NoComparison; NoEquality>]
type LexerWhitespaceContinuation =
    | Token            of ifdef:LexerIfdefStackEntries
    | IfDefSkip        of ifdef:LexerIfdefStackEntries * int * range:range
    | String           of ifdef:LexerIfdefStackEntries * range:range
    | VerbatimString   of ifdef:LexerIfdefStackEntries * range:range
    | TripleQuoteString of ifdef:LexerIfdefStackEntries * range:range
    | Comment          of ifdef:LexerIfdefStackEntries * int * range:range
    | SingleLineComment of ifdef:LexerIfdefStackEntries * int * range:range
    | StringInComment    of ifdef:LexerIfdefStackEntries * int * range:range
    | VerbatimStringInComment   of ifdef:LexerIfdefStackEntries * int * range:range
    | TripleQuoteStringInComment   of ifdef:LexerIfdefStackEntries * int * range:range
    | MLOnly            of ifdef:LexerIfdefStackEntries * range:range
    | EndLine           of LexerEndlineContinuation

    member x.LexerIfdefStack =
        match x with
        | LexCont.Token (ifdef=ifd)
        | LexCont.IfDefSkip (ifdef=ifd)
        | LexCont.String (ifdef=ifd)
        | LexCont.VerbatimString (ifdef=ifd)
        | LexCont.Comment (ifdef=ifd)
        | LexCont.SingleLineComment (ifdef=ifd)
        | LexCont.TripleQuoteString (ifdef=ifd)
        | LexCont.StringInComment (ifdef=ifd)
        | LexCont.VerbatimStringInComment (ifdef=ifd)
        | LexCont.TripleQuoteStringInComment (ifdef=ifd)
        | LexCont.MLOnly (ifdef=ifd) -> ifd
        | LexCont.EndLine endl -> endl.LexerIfdefStack

and LexCont = LexerWhitespaceContinuation

//------------------------------------------------------------------------
// Parser/Lexer state
//------------------------------------------------------------------------

/// The error raised by the parse_error_rich function, which is called by the parser engine
/// when a syntax error occurs. The first object is the ParseErrorContext which contains a dump of
/// information about the grammar at the point where the error occurred, e.g. what tokens
/// are valid to shift next at that point in the grammar. This information is processed in CompileOps.fs.
[<NoEquality; NoComparison>]
exception SyntaxError of obj (* ParseErrorContext<_> *) * range:range

/// Get an F# compiler position from a lexer position
let internal posOfLexPosition (p:Position) =
    mkPos p.Line p.Column

/// Get an F# compiler range from a lexer range
let internal mkSynRange (p1:Position) (p2: Position) =
    mkFileIndexRange p1.FileIndex (posOfLexPosition p1) (posOfLexPosition p2)

type LexBuffer<'Char> with
    member internal lexbuf.LexemeRange  = mkSynRange lexbuf.StartPos lexbuf.EndPos

/// Get the range corresponding to the result of a grammar rule while it is being reduced
let internal lhs (parseState: IParseState) =
    let p1 = parseState.ResultStartPosition
    let p2 = parseState.ResultEndPosition
    mkSynRange p1 p2

/// Get the range covering two of the r.h.s. symbols of a grammar rule while it is being reduced
let internal rhs2 (parseState: IParseState) i j = 
    let p1 = parseState.InputStartPosition i
    let p2 = parseState.InputEndPosition j
    mkSynRange p1 p2

/// Get the range corresponding to one of the r.h.s. symbols of a grammar rule while it is being reduced
let internal rhs parseState i = rhs2 parseState i i 

type IParseState with

    /// Get the generator used for compiler-generated argument names.
    member internal x.SynArgNameGenerator = 
        let key = "SynArgNameGenerator"
        let bls = x.LexBuffer.BufferLocalStore
        let gen =
            match bls.TryGetValue(key) with
            | true, gen -> gen
            | _ ->
                let gen = box (SynArgNameGenerator())
                bls.[key] <- gen
                gen
        gen :?> SynArgNameGenerator

    /// Reset the generator used for compiler-generated argument names.
    member internal x.ResetSynArgNameGenerator() = x.SynArgNameGenerator.Reset()


/// XmlDoc F# lexer/parser state, held in the BufferLocalStore for the lexer.
/// This is the only use of the lexer BufferLocalStore in the codebase.
module LexbufLocalXmlDocStore =
    // The key into the BufferLocalStore used to hold the current accumulated XmlDoc lines
    let private xmlDocKey = "XmlDoc"

    let internal ClearXmlDoc (lexbuf:Lexbuf) = 
        lexbuf.BufferLocalStore.[xmlDocKey] <- box (XmlDocCollector())

    /// Called from the lexer to save a single line of XML doc comment.
    let internal SaveXmlDocLine (lexbuf:Lexbuf, lineText, pos) =
        let collector =
            match lexbuf.BufferLocalStore.TryGetValue(xmlDocKey) with
            | true, collector -> collector
            | _ ->
                let collector = box (XmlDocCollector())
                lexbuf.BufferLocalStore.[xmlDocKey] <- collector
                collector
        let collector = unbox<XmlDocCollector>(collector)
        collector.AddXmlDocLine(lineText, pos)

    /// Called from the parser each time we parse a construct that marks the end of an XML doc comment range,
    /// e.g. a 'type' declaration. The markerRange is the range of the keyword that delimits the construct.
    let internal GrabXmlDocBeforeMarker (lexbuf:Lexbuf, markerRange:range)  =
        match lexbuf.BufferLocalStore.TryGetValue(xmlDocKey) with
        | true, collector ->
            let collector = unbox<XmlDocCollector>(collector)
            PreXmlDoc.CreateFromGrabPoint(collector, markerRange.End)
        | _ ->
            PreXmlDoc.Empty



/// Generates compiler-generated names. Each name generated also includes the StartLine number of the range passed in
/// at the point of first generation.
///
/// This type may be accessed concurrently, though in practice it is only used from the compilation thread.
/// It is made concurrency-safe since a global instance of the type is allocated in tast.fs, and it is good
/// policy to make all globally-allocated objects concurrency safe in case future versions of the compiler
/// are used to host multiple concurrent instances of compilation.
type NiceNameGenerator() =

    let lockObj = obj()
    let basicNameCounts = new Dictionary<string,int>(100)

    member x.FreshCompilerGeneratedName (name,m:range) =
      lock lockObj (fun () -> 
        let basicName = GetBasicNameOfPossibleCompilerGeneratedName name
        let n =
            match basicNameCounts.TryGetValue(basicName) with
            | true, count -> count
            | _ -> 0
        let nm = CompilerGeneratedNameSuffix basicName (string m.StartLine + (match n with 0 -> "" | n -> "-" + string n))
        basicNameCounts.[basicName] <- n + 1
        nm)

    member x.Reset () = 
      lock lockObj (fun () -> 
        basicNameCounts.Clear()
      )



/// Generates compiler-generated names marked up with a source code location, but if given the same unique value then
/// return precisely the same name. Each name generated also includes the StartLine number of the range passed in
/// at the point of first generation.
///
/// This type may be accessed concurrently, though in practice it is only used from the compilation thread.
/// It is made concurrency-safe since a global instance of the type is allocated in tast.fs.
type StableNiceNameGenerator() =

    let lockObj = obj()

    let names = new Dictionary<(string * int64),string>(100)
    let basicNameCounts = new Dictionary<string,int>(100)

    member x.GetUniqueCompilerGeneratedName (name,m:range,uniq) =
        lock lockObj (fun () -> 
            let basicName = GetBasicNameOfPossibleCompilerGeneratedName name
            let key = basicName, uniq
            match names.TryGetValue(key) with
            | true, nm -> nm
            | _ ->
                let n =
                    match basicNameCounts.TryGetValue(basicName) with
                    | true, c -> c
                    | _ -> 0
                let nm = CompilerGeneratedNameSuffix basicName (string m.StartLine + (match n with 0 -> "" | n -> "-" + string n))
                names.[key] <- nm
                basicNameCounts.[basicName] <- n + 1
                nm
        )

    member x.Reset () =
      lock lockObj (fun () -> 
        basicNameCounts.Clear()
        names.Clear()
      )

let rec synExprContainsError inpExpr =
    let rec walkBind (Binding(_, _, _, _, _, _, _, _, _, synExpr, _, _)) = walkExpr synExpr
    and walkExprs es = es |> List.exists walkExpr
    and walkBinds es = es |> List.exists walkBind
    and walkMatchClauses cl = cl |> List.exists (fun (Clause(_,whenExpr,e,_,_)) -> walkExprOpt whenExpr || walkExpr e)
    and walkExprOpt eOpt = eOpt |> Option.exists walkExpr
    and walkExpr e =
          match e with
          | SynExpr.FromParseError _
          | SynExpr.DiscardAfterMissingQualificationAfterDot _
          | SynExpr.ArbitraryAfterError _ -> true
          | SynExpr.LongIdent _
          | SynExpr.Quote _
          | SynExpr.LibraryOnlyILAssembly _
          | SynExpr.LibraryOnlyStaticOptimization _
          | SynExpr.Null _
          | SynExpr.Ident _
          | SynExpr.ImplicitZero _
          | SynExpr.Const _ -> false

          | SynExpr.TypeTest (e,_,_)
          | SynExpr.Upcast (e,_,_)
          | SynExpr.AddressOf (_,e,_,_)
          | SynExpr.CompExpr (_,_,e,_)
          | SynExpr.ArrayOrListOfSeqExpr (_,e,_)
          | SynExpr.Typed (e,_,_)
          | SynExpr.FromParseError (e,_)
          | SynExpr.Do (e,_)
          | SynExpr.Assert (e,_)
          | SynExpr.DotGet (e,_,_,_)
          | SynExpr.LongIdentSet (_,e,_)
          | SynExpr.New (_,_,e,_)
          | SynExpr.TypeApp (e,_,_,_,_,_,_)
          | SynExpr.LibraryOnlyUnionCaseFieldGet (e,_,_,_)
          | SynExpr.Downcast (e,_,_)
          | SynExpr.InferredUpcast (e,_)
          | SynExpr.InferredDowncast (e,_)
          | SynExpr.Lazy (e, _)
          | SynExpr.TraitCall(_,_,e,_)
          | SynExpr.YieldOrReturn (_,e,_)
          | SynExpr.YieldOrReturnFrom (_,e,_)
          | SynExpr.DoBang (e,_)
          | SynExpr.Fixed (e,_)
          | SynExpr.Paren (e,_,_,_) ->
              walkExpr e

          | SynExpr.NamedIndexedPropertySet (_,e1,e2,_)
          | SynExpr.DotSet (e1,_,e2,_)
          | SynExpr.LibraryOnlyUnionCaseFieldSet (e1,_,_,e2,_)
          | SynExpr.JoinIn (e1,_,e2,_)
          | SynExpr.App (_,_,e1,e2,_) ->
              walkExpr e1 || walkExpr e2

          | SynExpr.ArrayOrList (_,es,_)
          | SynExpr.Tuple (es,_,_)
          | SynExpr.StructTuple (es,_,_) ->
              walkExprs es

          | SynExpr.Record (_,_,fs,_) ->
              let flds = fs |> List.choose (fun (_, v, _) -> v)
              walkExprs (flds)

          | SynExpr.ObjExpr (_,_,bs,is,_,_) ->
              walkBinds bs || walkBinds [ for (InterfaceImpl(_,bs,_)) in is do yield! bs  ]
          | SynExpr.ForEach (_,_,_,_,e1,e2,_)
          | SynExpr.While (_,e1,e2,_) ->
              walkExpr e1 || walkExpr e2
          | SynExpr.For (_,_,e1,_,e2,e3,_) ->
              walkExpr e1 || walkExpr e2 || walkExpr e3
          | SynExpr.MatchLambda(_,_,cl,_,_) ->
              walkMatchClauses cl
          | SynExpr.Lambda (_,_,_,e,_) ->
              walkExpr e
          | SynExpr.Match (_,e,cl,_,_) ->
              walkExpr e || walkMatchClauses cl
          | SynExpr.LetOrUse (_,_,bs,e,_) ->
              walkBinds bs || walkExpr e

          | SynExpr.TryWith (e,_,cl,_,_,_,_) ->
              walkExpr e  || walkMatchClauses cl

          | SynExpr.TryFinally (e1,e2,_,_,_) ->
              walkExpr e1 || walkExpr e2
          | SynExpr.Sequential (_,_,e1,e2,_) ->
              walkExpr e1 || walkExpr e2
          | SynExpr.IfThenElse (e1,e2,e3opt,_,_,_,_) ->
              walkExpr e1 || walkExpr e2 || walkExprOpt e3opt
          | SynExpr.DotIndexedGet (e1,es,_,_) ->
              walkExpr e1 || walkExprs [ for e in es do yield! e.Exprs ]

          | SynExpr.DotIndexedSet (e1,es,e2,_,_,_) ->
              walkExpr e1 || walkExprs [ for e in es do yield! e.Exprs ] || walkExpr e2
          | SynExpr.DotNamedIndexedPropertySet (e1,_,e2,e3,_) ->
              walkExpr e1 || walkExpr e2 || walkExpr e3

          | SynExpr.MatchBang (_,e,cl,_,_) ->
              walkExpr e || walkMatchClauses cl
          | SynExpr.LetOrUseBang  (_,_,_,_,e1,e2,_) ->
              walkExpr e1 || walkExpr e2
    walkExpr inpExpr
