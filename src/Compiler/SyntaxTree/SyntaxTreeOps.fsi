// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.SyntaxTreeOps

open FSharp.Compiler.Text
open FSharp.Compiler.Xml
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia

[<Class>]
type SynArgNameGenerator =
    new: unit -> SynArgNameGenerator
    member New: unit -> string
    member Reset: unit -> unit

val ident: s: string * r: range -> Ident

val textOfId: id: Ident -> string

val pathOfLid: lid: Ident list -> string list

val arrPathOfLid: lid: Ident list -> string[]

val textOfPath: path: seq<string> -> string

val textOfLid: lid: Ident list -> string

val rangeOfLid: lid: Ident list -> range

val mkSynId: m: range -> s: string -> Ident

val pathToSynLid: m: range -> p: string list -> Ident list

val mkSynIdGet: m: range -> n: string -> SynExpr

val mkSynLidGet: m: range -> path: string list -> n: string -> SynExpr

val mkSynIdGetWithAlt: m: range -> id: Ident -> altInfo: SynSimplePatAlternativeIdInfo ref option -> SynExpr

val mkSynSimplePatVar: isOpt: bool -> id: Ident -> SynSimplePat

val mkSynCompGenSimplePatVar: id: Ident -> SynSimplePat

/// Match a long identifier, including the case for single identifiers which gets a more optimized node in the syntax tree.
val (|LongOrSingleIdent|_|):
    inp: SynExpr -> (bool * SynLongIdent * SynSimplePatAlternativeIdInfo ref option * range) option

val (|SingleIdent|_|): inp: SynExpr -> Ident option

/// This affects placement of debug points
val IsControlFlowExpression: e: SynExpr -> bool

// The debug point for a 'let' extends to include the 'let' if we're not defining a function and the r.h.s. is not a control-flow
// expression. Otherwise, there is no debug point at the binding.
val IsDebugPointBinding: synPat: SynPat -> synExpr: SynExpr -> bool

val mkSynAnonField: ty: SynType * xmlDoc: PreXmlDoc -> SynField

val mkSynNamedField: ident: Ident * ty: SynType * xmlDoc: PreXmlDoc * m: range -> SynField

val mkSynPatVar: vis: SynAccess option -> id: Ident -> SynPat

val mkSynThisPatVar: id: Ident -> SynPat

val mkSynPatMaybeVar: lidwd: SynLongIdent -> vis: SynAccess option -> m: range -> SynPat

val (|SynPatForConstructorDecl|_|): x: SynPat -> SynPat option

/// Recognize the '()' in 'new()'
val (|SynPatForNullaryArgs|_|): x: SynPat -> unit option

val (|SynExprErrorSkip|): p: SynExpr -> SynExpr

val (|SynExprParen|_|): e: SynExpr -> (SynExpr * range * range option * range) option

val (|SynPatErrorSkip|): p: SynPat -> SynPat

/// Push non-simple parts of a patten match over onto the r.h.s. of a lambda.
/// Return a simple pattern and a function to build a match on the r.h.s. if the pattern is complex
val SimplePatOfPat: synArgNameGenerator: SynArgNameGenerator -> p: SynPat -> SynSimplePat * (SynExpr -> SynExpr) option

val appFunOpt: funOpt: ('a -> 'a) option -> x: 'a -> 'a

val composeFunOpt: funOpt1: ('a -> 'a) option -> funOpt2: ('a -> 'a) option -> ('a -> 'a) option

val SimplePatsOfPat:
    synArgNameGenerator: SynArgNameGenerator -> p: SynPat -> SynSimplePats * (SynExpr -> SynExpr) option

val PushPatternToExpr:
    synArgNameGenerator: SynArgNameGenerator -> isMember: bool -> pat: SynPat -> rhs: SynExpr -> SynSimplePats * SynExpr

/// "fun (UnionCase x) (UnionCase y) -> body"
///       ==>
///   "fun tmp1 tmp2 ->
///        let (UnionCase x) = tmp1 in
///        let (UnionCase y) = tmp2 in
///        body"
val PushCurriedPatternsToExpr:
    synArgNameGenerator: SynArgNameGenerator ->
    wholem: range ->
    isMember: bool ->
    pats: SynPat list ->
    arrow: Range option ->
    rhs: SynExpr ->
        SynSimplePats list * SynExpr

val opNameParenGet: string

val opNameQMark: string

val mkSynOperator: opm: range -> oper: string -> SynExpr

val mkSynInfix: opm: range -> l: SynExpr -> oper: string -> r: SynExpr -> SynExpr

val mkSynBifix: m: range -> oper: string -> x1: SynExpr -> x2: SynExpr -> SynExpr

val mkSynTrifix: m: range -> oper: string -> x1: SynExpr -> x2: SynExpr -> x3: SynExpr -> SynExpr

val mkSynPrefixPrim: opm: range -> m: range -> oper: string -> x: SynExpr -> SynExpr

val mkSynPrefix: opm: range -> m: range -> oper: string -> x: SynExpr -> SynExpr

val mkSynCaseName: m: range -> n: string -> Ident list

val mkSynApp1: f: SynExpr -> x1: SynExpr -> m: range -> SynExpr

val mkSynApp2: f: SynExpr -> x1: SynExpr -> x2: SynExpr -> m: range -> SynExpr

val mkSynApp3: f: SynExpr -> x1: SynExpr -> x2: SynExpr -> x3: SynExpr -> m: range -> SynExpr

val mkSynApp4: f: SynExpr -> x1: SynExpr -> x2: SynExpr -> x3: SynExpr -> x4: SynExpr -> m: range -> SynExpr

val mkSynApp5:
    f: SynExpr -> x1: SynExpr -> x2: SynExpr -> x3: SynExpr -> x4: SynExpr -> x5: SynExpr -> m: range -> SynExpr

val mkSynDotParenSet: m: range -> a: SynExpr -> b: SynExpr -> c: SynExpr -> SynExpr

val mkSynDotBrackGet: m: range -> mDot: range -> a: SynExpr -> b: SynExpr -> SynExpr

val mkSynQMarkSet: m: range -> a: SynExpr -> b: SynExpr -> c: SynExpr -> SynExpr

//val mkSynDotBrackSliceGet: m:range -> mDot:range -> arr:SynExpr -> sliceArg:SynIndexerArg -> SynExpr

//val mkSynDotBrackSeqSliceGet: m:range -> mDot:range -> arr:SynExpr -> argsList:SynIndexerArg list -> SynExpr

val mkSynDotParenGet: mLhs: range -> mDot: range -> a: SynExpr -> b: SynExpr -> SynExpr

val mkSynUnit: m: range -> SynExpr

val mkSynUnitPat: m: range -> SynPat

val mkSynDelay: m: range -> e: SynExpr -> SynExpr

val mkSynAssign: l: SynExpr -> r: SynExpr -> SynExpr

val mkSynDot: mDot: range -> m: range -> l: SynExpr -> r: SynIdent -> SynExpr

val mkSynDotMissing: mDot: range -> m: range -> l: SynExpr -> SynExpr

val mkSynFunMatchLambdas:
    synArgNameGenerator: SynArgNameGenerator ->
    isMember: bool ->
    wholem: range ->
    ps: SynPat list ->
    arrow: Range option ->
    e: SynExpr ->
        SynExpr

val arbExpr: debugStr: string * range: range -> SynExpr

val unionRangeWithListBy: projectRangeFromThing: ('a -> range) -> m: range -> listOfThing: 'a list -> range

val inline unionRangeWithXmlDoc: xmlDoc: PreXmlDoc -> range: range -> range

val mkAttributeList: attrs: SynAttribute list -> range: range -> SynAttributeList list

val ConcatAttributesLists: attrsLists: SynAttributeList list -> SynAttribute list

val (|Attributes|): synAttributes: SynAttributeList list -> SynAttribute list

val (|TyparDecls|): typarDecls: SynTyparDecls option -> SynTyparDecl list
val (|TyparsAndConstraints|): typarDecls: SynTyparDecls option -> SynTyparDecl list * SynTypeConstraint list
val (|ValTyparDecls|): valTyparDecls: SynValTyparDecls -> SynTyparDecl list * SynTypeConstraint list * bool

val rangeOfNonNilAttrs: attrs: SynAttributes -> range

val stripParenTypes: synType: SynType -> SynType

val (|StripParenTypes|): synType: SynType -> SynType

val mkSynBindingRhs:
    staticOptimizations: (SynStaticOptimizationConstraint list * SynExpr) list ->
    rhsExpr: SynExpr ->
    mRhs: range ->
    retInfo: SynType option ->
        SynExpr * SynType option

val mkSynBinding:
    xmlDoc: PreXmlDoc * headPat: SynPat ->
        vis: SynAccess option *
        isInline: bool *
        isMutable: bool *
        mBind: range *
        spBind: DebugPointAtBinding *
        retInfo: SynType option *
        origRhsExpr: SynExpr *
        mRhs: range *
        staticOptimizations: (SynStaticOptimizationConstraint list * SynExpr) list *
        attrs: SynAttributes *
        memberFlagsOpt: SynMemberFlags option *
        trivia: SynBindingTrivia ->
            SynBinding

val NonVirtualMemberFlags: trivia: SynMemberFlagsTrivia -> k: SynMemberKind -> SynMemberFlags

val CtorMemberFlags: trivia: SynMemberFlagsTrivia -> SynMemberFlags

val ClassCtorMemberFlags: trivia: SynMemberFlagsTrivia -> SynMemberFlags

val OverrideMemberFlags: trivia: SynMemberFlagsTrivia -> k: SynMemberKind -> SynMemberFlags

val AbstractMemberFlags: isInstance: bool -> trivia: SynMemberFlagsTrivia -> k: SynMemberKind -> SynMemberFlags

val StaticMemberFlags: trivia: SynMemberFlagsTrivia -> k: SynMemberKind -> SynMemberFlags

val ImplementStaticMemberFlags: SynMemberFlagsTrivia -> k: SynMemberKind -> SynMemberFlags

val MemberSynMemberFlagsTrivia: mMember: range -> SynMemberFlagsTrivia

val OverrideSynMemberFlagsTrivia: mOverride: range -> SynMemberFlagsTrivia

val StaticMemberSynMemberFlagsTrivia: mStatic: range -> mMember: range -> SynMemberFlagsTrivia

val DefaultSynMemberFlagsTrivia: mDefault: range -> SynMemberFlagsTrivia

val AbstractSynMemberFlagsTrivia: mAbstract: range -> SynMemberFlagsTrivia

val AbstractMemberSynMemberFlagsTrivia: mAbstract: range -> mMember: range -> SynMemberFlagsTrivia

val StaticAbstractSynMemberFlagsTrivia: mStatic: range -> mAbstract: range -> SynMemberFlagsTrivia

val StaticAbstractMemberSynMemberFlagsTrivia:
    mStatic: range -> mAbstract: range -> mMember: range -> SynMemberFlagsTrivia

val inferredTyparDecls: SynValTyparDecls

val noInferredTypars: SynValTyparDecls

val unionBindingAndMembers: bindings: SynBinding list -> members: SynMemberDefn list -> SynBinding list

val synExprContainsError: inpExpr: SynExpr -> bool

val (|ParsedHashDirectiveArguments|): ParsedHashDirectiveArgument list -> string list

/// 'e1 && e2'
val (|SynAndAlso|_|): SynExpr -> (SynExpr * SynExpr) option

/// 'e1 || e2'
val (|SynOrElse|_|): SynExpr -> (SynExpr * SynExpr) option

/// 'e1 |> e2'
val (|SynPipeRight|_|): SynExpr -> (SynExpr * SynExpr) option

/// 'e1 ||> e2'
val (|SynPipeRight2|_|): SynExpr -> (SynExpr * SynExpr * SynExpr) option

/// 'e1 |||> e2'
val (|SynPipeRight3|_|): SynExpr -> (SynExpr * SynExpr * SynExpr * SynExpr) option

val prependIdentInLongIdentWithTrivia: ident: SynIdent -> mDot: range -> lid: SynLongIdent -> SynLongIdent

val mkDynamicArgExpr: expr: SynExpr -> SynExpr

val normalizeTupleExpr: exprs: SynExpr list -> commas: range list -> SynExpr list * range List

val desugarGetSetMembers: memberDefns: SynMemberDefns -> SynMemberDefns

val getTypeFromTuplePath: path: SynTupleTypeSegment list -> SynType list
