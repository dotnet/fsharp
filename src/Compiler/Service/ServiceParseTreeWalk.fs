// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace FSharp.Compiler.Syntax

open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range

[<RequireQualifiedAccess>]
type SyntaxNode =
    | SynPat of SynPat
    | SynType of SynType
    | SynExpr of SynExpr
    | SynModule of SynModuleDecl
    | SynModuleOrNamespace of SynModuleOrNamespace
    | SynTypeDefn of SynTypeDefn
    | SynMemberDefn of SynMemberDefn
    | SynMatchClause of SynMatchClause
    | SynBinding of SynBinding
    | SynModuleOrNamespaceSig of SynModuleOrNamespaceSig
    | SynModuleSigDecl of SynModuleSigDecl
    | SynValSig of SynValSig
    | SynTypeDefnSig of SynTypeDefnSig
    | SynMemberSig of SynMemberSig

    member this.Range =
        match this with
        | SynPat pat -> pat.Range
        | SynType ty -> ty.Range
        | SynExpr expr -> expr.Range
        | SynModule modul -> modul.Range
        | SynModuleOrNamespace moduleOrNamespace -> moduleOrNamespace.Range
        | SynTypeDefn tyDef -> tyDef.Range
        | SynMemberDefn memberDef -> memberDef.Range
        | SynMatchClause matchClause -> matchClause.Range
        | SynBinding binding -> binding.RangeOfBindingWithRhs
        | SynModuleOrNamespaceSig moduleOrNamespaceSig -> moduleOrNamespaceSig.Range
        | SynModuleSigDecl moduleSigDecl -> moduleSigDecl.Range
        | SynValSig(SynValSig.SynValSig(range = range)) -> range
        | SynTypeDefnSig tyDefSig -> tyDefSig.Range
        | SynMemberSig memberSig -> memberSig.Range

type SyntaxVisitorPath = SyntaxNode list

[<AbstractClass>]
type SyntaxVisitorBase<'T>() =
    abstract VisitExpr:
        path: SyntaxVisitorPath * traverseSynExpr: (SynExpr -> 'T option) * defaultTraverse: (SynExpr -> 'T option) * synExpr: SynExpr ->
            'T option

    default _.VisitExpr
        (
            path: SyntaxVisitorPath,
            traverseSynExpr: SynExpr -> 'T option,
            defaultTraverse: SynExpr -> 'T option,
            synExpr: SynExpr
        ) =
        ignore (path, traverseSynExpr, defaultTraverse, synExpr)
        None

    /// VisitTypeAbbrev(ty,m), defaults to ignoring this leaf of the AST
    abstract VisitTypeAbbrev: path: SyntaxVisitorPath * synType: SynType * range: range -> 'T option

    default _.VisitTypeAbbrev(path, synType, range) =
        ignore (path, synType, range)
        None

    /// VisitImplicitInherit(defaultTraverse,ty,expr,m), defaults to just visiting expr
    abstract VisitImplicitInherit:
        path: SyntaxVisitorPath * defaultTraverse: (SynExpr -> 'T option) * inheritedType: SynType * synArgs: SynExpr * range: range ->
            'T option

    default _.VisitImplicitInherit(path, defaultTraverse, inheritedType, synArgs, range) =
        ignore (path, inheritedType, range)
        defaultTraverse synArgs

    /// VisitModuleDecl allows overriding module declaration behavior
    abstract VisitModuleDecl:
        path: SyntaxVisitorPath * defaultTraverse: (SynModuleDecl -> 'T option) * synModuleDecl: SynModuleDecl -> 'T option

    default _.VisitModuleDecl(path, defaultTraverse, synModuleDecl) =
        ignore path
        defaultTraverse synModuleDecl

    /// VisitBinding allows overriding binding behavior (note: by default it would defaultTraverse expression)
    abstract VisitBinding: path: SyntaxVisitorPath * defaultTraverse: (SynBinding -> 'T option) * synBinding: SynBinding -> 'T option

    default _.VisitBinding(path, defaultTraverse, synBinding) =
        ignore path
        defaultTraverse synBinding

    /// VisitMatchClause allows overriding clause behavior (note: by default it would defaultTraverse expression)
    abstract VisitMatchClause:
        path: SyntaxVisitorPath * defaultTraverse: (SynMatchClause -> 'T option) * matchClause: SynMatchClause -> 'T option

    default _.VisitMatchClause(path, defaultTraverse, matchClause) =
        ignore path
        defaultTraverse matchClause

    /// VisitInheritSynMemberDefn allows overriding inherit behavior (by default do nothing)
    abstract VisitInheritSynMemberDefn:
        path: SyntaxVisitorPath * componentInfo: SynComponentInfo * typeDefnKind: SynTypeDefnKind * SynType * SynMemberDefns * range ->
            'T option

    default _.VisitInheritSynMemberDefn(path, componentInfo, typeDefnKind, synType, members, range) =
        ignore (path, componentInfo, typeDefnKind, synType, members, range)
        None

    /// VisitRecordDefn allows overriding behavior when visiting record definitions (by default do nothing)
    abstract VisitRecordDefn: path: SyntaxVisitorPath * fields: SynField list * range -> 'T option

    default _.VisitRecordDefn(path, fields, range) =
        ignore (path, fields, range)
        None

    /// VisitUnionDefn allows overriding behavior when visiting union definitions (by default do nothing)
    abstract VisitUnionDefn: path: SyntaxVisitorPath * cases: SynUnionCase list * range -> 'T option

    default _.VisitUnionDefn(path, cases, range) =
        ignore (path, cases, range)
        None

    /// VisitEnumDefn allows overriding behavior when visiting enum definitions (by default do nothing)
    abstract VisitEnumDefn: path: SyntaxVisitorPath * cases: SynEnumCase list * range -> 'T option

    default _.VisitEnumDefn(path, cases, range) =
        ignore (path, cases, range)
        None

    /// VisitInterfaceSynMemberDefnType allows overriding behavior for visiting interface member in types (by default - do nothing)
    abstract VisitInterfaceSynMemberDefnType: path: SyntaxVisitorPath * synType: SynType -> 'T option

    default _.VisitInterfaceSynMemberDefnType(path, synType) =
        ignore (path, synType)
        None

    /// VisitRecordField allows overriding behavior when visiting l.h.s. of constructed record instances
    abstract VisitRecordField: path: SyntaxVisitorPath * copyOpt: SynExpr option * recordField: SynLongIdent option -> 'T option

    default _.VisitRecordField(path, copyOpt, recordField) =
        ignore (path, copyOpt, recordField)
        None

    /// VisitHashDirective allows overriding behavior when visiting hash directives in FSX scripts, like #r, #load and #I.
    abstract VisitHashDirective: path: SyntaxVisitorPath * hashDirective: ParsedHashDirective * range: range -> 'T option

    default _.VisitHashDirective(path, hashDirective, range) =
        ignore (path, hashDirective, range)
        None

    /// VisitModuleOrNamespace allows overriding behavior when visiting module or namespaces
    abstract VisitModuleOrNamespace: path: SyntaxVisitorPath * synModuleOrNamespace: SynModuleOrNamespace -> 'T option

    default _.VisitModuleOrNamespace(path, synModuleOrNamespace) =
        ignore (path, synModuleOrNamespace)
        None

    /// VisitComponentInfo allows overriding behavior when visiting type component infos
    abstract VisitComponentInfo: path: SyntaxVisitorPath * synComponentInfo: SynComponentInfo -> 'T option

    default _.VisitComponentInfo(path, synComponentInfo) =
        ignore (path, synComponentInfo)
        None

    /// VisitLetOrUse allows overriding behavior when visiting module or local let or use bindings
    abstract VisitLetOrUse:
        path: SyntaxVisitorPath * isRecursive: bool * defaultTraverse: (SynBinding -> 'T option) * bindings: SynBinding list * range: range ->
            'T option

    default _.VisitLetOrUse(path, isRecursive, defaultTraverse, bindings, range) =
        ignore (path, isRecursive, defaultTraverse, bindings, range)
        None

    /// VisitSimplePats allows overriding behavior when visiting simple pats
    abstract VisitSimplePats: path: SyntaxVisitorPath * pat: SynPat -> 'T option

    default _.VisitSimplePats(path, pat) =
        ignore (path, pat)
        None

    /// VisitPat allows overriding behavior when visiting patterns
    abstract VisitPat: path: SyntaxVisitorPath * defaultTraverse: (SynPat -> 'T option) * synPat: SynPat -> 'T option

    default _.VisitPat(path, defaultTraverse, synPat) =
        ignore path
        defaultTraverse synPat

    /// VisitType allows overriding behavior when visiting type hints (x: ..., etc.)
    abstract VisitType: path: SyntaxVisitorPath * defaultTraverse: (SynType -> 'T option) * synType: SynType -> 'T option

    default _.VisitType(path, defaultTraverse, synType) =
        ignore path
        defaultTraverse synType

    abstract VisitAttributeApplication: path: SyntaxVisitorPath * attributes: SynAttributeList -> 'T option

    default _.VisitAttributeApplication(path, attributes) =
        ignore (path, attributes)
        None

    /// VisitModuleOrNamespaceSig allows overriding behavior when visiting module or namespaces
    abstract VisitModuleOrNamespaceSig: path: SyntaxVisitorPath * synModuleOrNamespaceSig: SynModuleOrNamespaceSig -> 'T option

    default _.VisitModuleOrNamespaceSig(path, synModuleOrNamespaceSig) =
        ignore (path, synModuleOrNamespaceSig)
        None

    /// VisitModuleDecl allows overriding signature module declaration behavior
    abstract VisitModuleSigDecl:
        path: SyntaxVisitorPath * defaultTraverse: (SynModuleSigDecl -> 'T option) * synModuleSigDecl: SynModuleSigDecl -> 'T option

    default _.VisitModuleSigDecl(path, defaultTraverse, synModuleSigDecl) =
        ignore path
        defaultTraverse synModuleSigDecl

    /// VisitValSig allows overriding SynValSig behavior
    abstract VisitValSig: path: SyntaxVisitorPath * defaultTraverse: (SynValSig -> 'T option) * valSig: SynValSig -> 'T option

    default _.VisitValSig(path, defaultTraverse, valSig) =
        ignore path
        defaultTraverse valSig

[<AutoOpen>]
module private ParsedInputExtensions =
    type ParsedInput with

        member parsedInput.Contents =
            match parsedInput with
            | ParsedInput.ImplFile file -> file.Contents |> List.map SyntaxNode.SynModuleOrNamespace
            | ParsedInput.SigFile file -> file.Contents |> List.map SyntaxNode.SynModuleOrNamespaceSig

/// A range of utility functions to assist with traversing an AST
module SyntaxTraversal =
    // treat ranges as though they are half-open: [,)
    let rangeContainsPosLeftEdgeInclusive (m1: range) p =
        if posEq m1.Start m1.End then
            // the parser doesn't produce zero-width ranges, except in one case, for e.g. a block of lets that lacks a body
            // we treat the range [n,n) as containing position n
            posGeq p m1.Start && posGeq m1.End p
        else
            posGeq p m1.Start
            && // [
            posGt m1.End p // )

    // treat ranges as though they are fully open: (,)
    let rangeContainsPosEdgesExclusive (m1: range) p = posGt p m1.Start && posGt m1.End p

    let rangeContainsPosLeftEdgeExclusiveAndRightEdgeInclusive (m1: range) p = posGt p m1.Start && posGeq m1.End p

    let dive node range project = range, (fun () -> project node)

    let pick pos (outerRange: range) (debugObj: obj) (diveResults: (range * _) list) =
        match diveResults with
        | [] -> None
        | _ ->
            let isOrdered =
#if DEBUG
                // ranges in a dive-and-pick group should be ordered
                diveResults
                |> Seq.pairwise
                |> Seq.forall (fun ((r1, _), (r2, _)) -> posGeq r2.Start r1.End)
#else
                true
#endif
            if not isOrdered then
                let s =
                    sprintf "ServiceParseTreeWalk: not isOrdered: %A" (diveResults |> List.map (fun (r, _) -> r.ToString()))

                ignore s
            //System.Diagnostics.Debug.Assert(false, s)
            let outerContainsInner =
#if DEBUG
                // ranges in a dive-and-pick group should be "under" the thing that contains them
                let innerTotalRange = diveResults |> List.map fst |> List.reduce unionRanges
                rangeContainsRange outerRange innerTotalRange
#else
                ignore (outerRange)
                true
#endif
            if not outerContainsInner then
                let s =
                    sprintf
                        "ServiceParseTreeWalk: not outerContainsInner: %A : %A"
                        (outerRange.ToString())
                        (diveResults |> List.map (fun (r, _) -> r.ToString()))

                ignore s
            //System.Diagnostics.Debug.Assert(false, s)
            let isZeroWidth (r: range) = posEq r.Start r.End // the parser inserts some zero-width elements to represent the completions of incomplete constructs, but we should never 'dive' into them, since they don't represent actual user code

            match
                List.choose
                    (fun (r, f) ->
                        if rangeContainsPosLeftEdgeInclusive r pos && not (isZeroWidth r) then
                            Some(f)
                        else
                            None)
                    diveResults
            with
            | [] ->
                // No entity's range contained the desired position.  However the ranges in the parse tree only span actual characters present in the file.
                // The cursor may be at whitespace between entities or after everything, so find the nearest entity with the range left of the position.
                let mutable e = diveResults.Head

                for r in diveResults do
                    if posGt pos (fst r).Start then
                        e <- r

                snd (e) ()
            | [ x ] -> x ()
            | _ ->
#if DEBUG
                printf "multiple disjoint AST node ranges claimed to contain (%A) from %+A" pos debugObj
#endif
                ignore debugObj
                None

    /// <summary>
    /// Traverse an implementation file until <paramref name="pick"/> returns <c>Some value</c>.
    /// </summary>
    let traverseUntil
        (pick: pos -> range -> obj -> (range * (unit -> 'T option)) list -> 'T option)
        (pos: pos)
        (visitor: SyntaxVisitorBase<'T>)
        (ast: SyntaxNode list)
        : 'T option =
        let pick x = pick pos x

        let rec traverseSynModuleDecl origPath (decl: SynModuleDecl) =
            let pick = pick decl.Range

            let defaultTraverse m =
                let path = SyntaxNode.SynModule m :: origPath

                match m with
                | SynModuleDecl.ModuleAbbrev(_ident, _longIdent, _range) -> None
                | SynModuleDecl.NestedModule(decls = synModuleDecls; moduleInfo = SynComponentInfo(attributes = attributes)) ->
                    synModuleDecls
                    |> List.map (fun x -> dive x x.Range (traverseSynModuleDecl path))
                    |> List.append (attributeApplicationDives path attributes)
                    |> pick decl
                | SynModuleDecl.Let(isRecursive, synBindingList, range) ->
                    match visitor.VisitLetOrUse(path, isRecursive, traverseSynBinding path, synBindingList, range) with
                    | Some x -> Some x
                    | None ->
                        synBindingList
                        |> List.map (fun x -> dive x x.RangeOfBindingWithRhs (traverseSynBinding path))
                        |> pick decl
                | SynModuleDecl.Expr(synExpr, _range) -> traverseSynExpr path synExpr
                | SynModuleDecl.Types(synTypeDefnList, _range) ->
                    synTypeDefnList
                    |> List.map (fun x -> dive x x.Range (traverseSynTypeDefn path))
                    |> pick decl
                | SynModuleDecl.Exception(_synExceptionDefn, _range) -> None
                | SynModuleDecl.Open(_target, _range) -> None
                | SynModuleDecl.Attributes(attributes, _) -> attributeApplicationDives path attributes |> pick decl
                | SynModuleDecl.HashDirective(parsedHashDirective, range) -> visitor.VisitHashDirective(path, parsedHashDirective, range)
                | SynModuleDecl.NamespaceFragment(synModuleOrNamespace) -> traverseSynModuleOrNamespace path synModuleOrNamespace

            visitor.VisitModuleDecl(origPath, defaultTraverse, decl)

        and traverseSynModuleOrNamespace origPath (SynModuleOrNamespace(decls = synModuleDecls; range = range) as mors) =
            match visitor.VisitModuleOrNamespace(origPath, mors) with
            | Some x -> Some x
            | None ->
                let path = SyntaxNode.SynModuleOrNamespace mors :: origPath

                synModuleDecls
                |> List.map (fun x -> dive x x.Range (traverseSynModuleDecl path))
                |> pick range mors

        and traverseSynExpr origPath (expr: SynExpr) =
            let pick = pick expr.Range

            /// Sequential expressions are more likely than
            /// most other expression kinds to be deeply nested,
            /// e.g., in very large list or array expressions.
            /// We treat them specially to avoid blowing the stack,
            /// since traverseSynExpr itself is not tail-recursive.
            let rec traverseSequentials path expr =
                seq {
                    match expr with
                    | SynExpr.Sequential(expr1 = expr1; expr2 = SynExpr.Sequential _ as expr2) ->
                        let path = SyntaxNode.SynExpr expr :: path
                        yield dive expr1 expr1.Range (traverseSynExpr path)
                        yield! traverseSequentials path expr2

                    | _ ->
                        // It's not a nested sequential expression.
                        // Traverse it normally.
                        yield dive expr expr.Range (traverseSynExpr path)
                }

            let defaultTraverse e =
                let path = SyntaxNode.SynExpr e :: origPath
                let traverseSynExpr = traverseSynExpr path
                let traverseSynType = traverseSynType path
                let traversePat = traversePat path

                match e with
                | SynExpr.LongIdentSet(expr = synExpr)
                | SynExpr.DotGet(expr = synExpr)
                | SynExpr.Do(expr = synExpr)
                | SynExpr.DoBang(expr = synExpr)
                | SynExpr.Assert(expr = synExpr)
                | SynExpr.Fixed(expr = synExpr)
                | SynExpr.DebugPoint(innerExpr = synExpr)
                | SynExpr.AddressOf(expr = synExpr)
                | SynExpr.TraitCall(argExpr = synExpr)
                | SynExpr.Lazy(expr = synExpr)
                | SynExpr.InferredUpcast(expr = synExpr)
                | SynExpr.InferredDowncast(expr = synExpr)
                | SynExpr.YieldOrReturn(expr = synExpr)
                | SynExpr.YieldOrReturnFrom(expr = synExpr)
                | SynExpr.FromParseError(expr = synExpr)
                | SynExpr.DiscardAfterMissingQualificationAfterDot(expr = synExpr)
                | SynExpr.IndexFromEnd(expr = synExpr)
                | SynExpr.New(expr = synExpr)
                | SynExpr.ArrayOrListComputed(expr = synExpr)
                | SynExpr.TypeApp(expr = synExpr)
                | SynExpr.DotLambda(expr = synExpr)
                | SynExpr.Quote(quotedExpr = synExpr)
                | SynExpr.Paren(expr = synExpr) -> traverseSynExpr synExpr

                | SynExpr.InterpolatedString(contents = parts) ->
                    [
                        for part in parts do
                            match part with
                            | SynInterpolatedStringPart.String _ -> ()
                            | SynInterpolatedStringPart.FillExpr(fillExpr, _) -> yield dive fillExpr fillExpr.Range traverseSynExpr
                    ]
                    |> pick expr

                | SynExpr.Typed(expr = synExpr; targetType = synType) ->
                    match traverseSynExpr synExpr with
                    | None -> traverseSynType synType
                    | x -> x

                | SynExpr.Tuple(exprs = synExprList)
                | SynExpr.ArrayOrList(exprs = synExprList) -> synExprList |> List.map (fun x -> dive x x.Range traverseSynExpr) |> pick expr

                | SynExpr.AnonRecd(copyInfo = copyOpt; recordFields = fields) ->
                    [
                        match copyOpt with
                        | Some(expr, (withRange, _)) ->
                            yield dive expr expr.Range traverseSynExpr

                            yield
                                dive () withRange (fun () ->
                                    if posGeq pos withRange.End then
                                        // special case: caret is after WITH
                                        // { x with $ }
                                        visitor.VisitRecordField(path, Some expr, None)
                                    else
                                        None)
                        | _ -> ()

                        for field, _, x in fields do
                            yield dive () field.Range (fun () -> visitor.VisitRecordField(path, copyOpt |> Option.map fst, Some field))
                            yield dive x x.Range traverseSynExpr
                    ]
                    |> pick expr

                | SynExpr.Record(baseInfo = inheritOpt; copyInfo = copyOpt; recordFields = fields) ->
                    [
                        let diveIntoSeparator offsideColumn scPosOpt copyOpt =
                            match scPosOpt with
                            | Some scPos ->
                                if posGeq pos scPos then
                                    visitor.VisitRecordField(path, copyOpt, None) // empty field after the inherits
                                else
                                    None
                            | None ->
                                //semicolon position is not available - use offside rule
                                if pos.Column = offsideColumn then
                                    visitor.VisitRecordField(path, copyOpt, None) // empty field after the inherits
                                else
                                    None

                        match inheritOpt with
                        | Some(_ty, expr, _range, sepOpt, inheritRange) ->
                            // dive into argument
                            yield
                                dive expr expr.Range (fun expr ->
                                    // special-case:caret is located in the offside position below inherit
                                    // inherit A()
                                    // $
                                    if
                                        not (rangeContainsPos expr.Range pos)
                                        && sepOpt.IsNone
                                        && pos.Column = inheritRange.StartColumn
                                    then
                                        visitor.VisitRecordField(path, None, None)
                                    else
                                        traverseSynExpr expr)

                            match sepOpt with
                            | Some(sep, scPosOpt) ->
                                yield
                                    dive () sep (fun () ->
                                        // special case: caret is below 'inherit' + one or more fields are already defined
                                        // inherit A()
                                        // $
                                        // field1 = 5
                                        diveIntoSeparator inheritRange.StartColumn scPosOpt None)
                            | None -> ()
                        | _ -> ()

                        match copyOpt with
                        | Some(expr, (withRange, _)) ->
                            yield dive expr expr.Range traverseSynExpr

                            yield
                                dive () withRange (fun () ->
                                    if posGeq pos withRange.End then
                                        // special case: caret is after WITH
                                        // { x with $ }
                                        visitor.VisitRecordField(path, Some expr, None)
                                    else
                                        None)
                        | _ -> ()

                        let copyOpt = Option.map fst copyOpt

                        for SynExprRecordField(fieldName = (field, _); expr = e; blockSeparator = sepOpt) in fields do
                            yield
                                dive (path, copyOpt, Some field) field.Range (fun r ->
                                    if rangeContainsPos field.Range pos then
                                        visitor.VisitRecordField r
                                    else
                                        None)

                            let offsideColumn =
                                match inheritOpt with
                                | Some(_, _, _, _, inheritRange) -> inheritRange.StartColumn
                                | None -> field.Range.StartColumn

                            match e with
                            | Some e ->
                                yield
                                    dive e e.Range (fun expr ->
                                        // special case: caret is below field binding
                                        // field x = 5
                                        // $
                                        if
                                            not (rangeContainsPos e.Range pos)
                                            && sepOpt.IsNone
                                            && pos.Column = offsideColumn
                                        then
                                            visitor.VisitRecordField(path, copyOpt, None)
                                        else
                                            traverseSynExpr expr)
                            | None -> ()

                            match sepOpt with
                            | Some(sep, scPosOpt) ->
                                yield
                                    dive () sep (fun () ->
                                        // special case: caret is between field bindings
                                        // field1 = 5
                                        // $
                                        // field2 = 5
                                        diveIntoSeparator offsideColumn scPosOpt copyOpt)
                            | _ -> ()

                    ]
                    |> pick expr

                | SynExpr.ObjExpr(objType = ty; argOptions = baseCallOpt; bindings = binds; members = ms; extraImpls = ifaces) ->
                    let binds = unionBindingAndMembers binds ms

                    let result =
                        ifaces
                        |> Seq.map (fun (SynInterfaceImpl(interfaceTy = ty)) -> ty)
                        |> Seq.tryPick (fun ty -> visitor.VisitInterfaceSynMemberDefnType(path, ty))

                    if result.IsSome then
                        result
                    else
                        [
                            match baseCallOpt with
                            | Some(expr, _) ->
                                // this is like a call to 'new', so mock up a 'new' so we can recurse and use that existing logic
                                let newCall = SynExpr.New(false, ty, expr, unionRanges ty.Range expr.Range)
                                yield dive newCall newCall.Range traverseSynExpr
                            | _ -> ()
                            for b in binds do
                                yield dive b b.RangeOfBindingWithRhs (traverseSynBinding path)
                            for SynInterfaceImpl(ty, withKeyword, binds, members, range) in ifaces do
                                let path =
                                    SyntaxNode.SynMemberDefn(SynMemberDefn.Interface(ty, withKeyword, Some members, range))
                                    :: path

                                for b in binds do
                                    yield dive b b.RangeOfBindingWithRhs (traverseSynBinding path)

                                for m in members do
                                    yield dive m m.Range (traverseSynMemberDefn path (fun _ -> None))
                        ]
                        |> pick expr

                | SynExpr.ForEach(pat = synPat; enumExpr = synExpr; bodyExpr = synExpr2) ->
                    [
                        dive synPat synPat.Range traversePat
                        dive synExpr synExpr.Range traverseSynExpr
                        dive synExpr2 synExpr2.Range traverseSynExpr
                    ]
                    |> pick expr

                | SynExpr.ComputationExpr(expr = synExpr) ->
                    // now parser treats this syntactic expression as computation expression
                    // { identifier }
                    // here we detect this situation and treat ComputationExpr  { Identifier } as attempt to create record
                    // note: sequence expressions use SynExpr.ComputationExpr too - they need to be filtered out
                    let isPartOfArrayOrList =
                        match origPath with
                        | SyntaxNode.SynExpr(SynExpr.ArrayOrListComputed _) :: _ -> true
                        | _ -> false

                    let ok =
                        match isPartOfArrayOrList, synExpr with
                        | false, LongOrSingleIdent(_, lid, _, _) -> visitor.VisitRecordField(path, None, Some lid)
                        | _ -> None

                    if ok.IsSome then ok else traverseSynExpr synExpr

                | SynExpr.Lambda(parsedData = parsedData) ->
                    [
                        match parsedData with
                        | Some(pats, body) ->
                            for pat in pats do
                                yield dive pat pat.Range traversePat

                            yield dive body body.Range traverseSynExpr
                        | None -> ()
                    ]
                    |> pick expr

                | SynExpr.MatchLambda(matchClauses = synMatchClauseList) ->
                    synMatchClauseList
                    |> List.map (fun x -> dive x x.Range (traverseSynMatchClause path))
                    |> pick expr

                | SynExpr.TryWith(tryExpr = synExpr; withCases = synMatchClauseList)
                | SynExpr.Match(expr = synExpr; clauses = synMatchClauseList)
                | SynExpr.MatchBang(expr = synExpr; clauses = synMatchClauseList) ->
                    [
                        yield dive synExpr synExpr.Range traverseSynExpr
                        yield!
                            synMatchClauseList
                            |> List.map (fun x -> dive x x.Range (traverseSynMatchClause path))
                    ]
                    |> pick expr

                | SynExpr.App(isInfix = isInfix; funcExpr = synExpr; argExpr = synExpr2) ->
                    if isInfix then
                        [
                            dive synExpr2 synExpr2.Range traverseSynExpr
                            dive synExpr synExpr.Range traverseSynExpr
                        ] // reverse the args
                        |> pick expr
                    else
                        [
                            dive synExpr synExpr.Range traverseSynExpr
                            dive synExpr2 synExpr2.Range traverseSynExpr
                        ]
                        |> pick expr

                | SynExpr.LetOrUse(isRecursive = isRecursive; bindings = synBindingList; body = synExpr; range = range) ->
                    match visitor.VisitLetOrUse(path, isRecursive, traverseSynBinding path, synBindingList, range) with
                    | None ->
                        [
                            yield!
                                synBindingList
                                |> List.map (fun x -> dive x x.RangeOfBindingWithRhs (traverseSynBinding path))
                            yield dive synExpr synExpr.Range traverseSynExpr
                        ]
                        |> pick expr
                    | x -> x

                | SynExpr.IfThenElse(ifExpr = synExpr; thenExpr = synExpr2; elseExpr = synExprOpt) ->
                    [
                        yield dive synExpr synExpr.Range traverseSynExpr
                        yield dive synExpr2 synExpr2.Range traverseSynExpr
                        match synExprOpt with
                        | None -> ()
                        | Some x -> yield dive x x.Range traverseSynExpr
                    ]
                    |> pick expr

                | SynExpr.IndexRange(expr1 = expr1; expr2 = expr2) ->
                    [
                        match expr1 with
                        | Some e -> dive e e.Range traverseSynExpr
                        | None -> ()
                        match expr2 with
                        | Some e -> dive e e.Range traverseSynExpr
                        | None -> ()
                    ]
                    |> pick expr

                // Nested sequentials.
                | SynExpr.Sequential(expr1 = synExpr1; expr2 = synExpr2 & SynExpr.Sequential _) ->
                    [
                        dive synExpr1 synExpr1.Range traverseSynExpr
                        yield! traverseSequentials path synExpr2
                    ]
                    |> pick expr

                | SynExpr.Sequential(expr1 = synExpr1; expr2 = synExpr2)
                | SynExpr.Set(targetExpr = synExpr1; rhsExpr = synExpr2)
                | SynExpr.DotSet(targetExpr = synExpr1; rhsExpr = synExpr2)
                | SynExpr.TryFinally(tryExpr = synExpr1; finallyExpr = synExpr2)
                | SynExpr.SequentialOrImplicitYield(expr1 = synExpr1; expr2 = synExpr2)
                | SynExpr.While(whileExpr = synExpr1; doExpr = synExpr2)
                | SynExpr.WhileBang(whileExpr = synExpr1; doExpr = synExpr2)
                | SynExpr.DotIndexedGet(objectExpr = synExpr1; indexArgs = synExpr2)
                | SynExpr.JoinIn(lhsExpr = synExpr1; rhsExpr = synExpr2)
                | SynExpr.NamedIndexedPropertySet(expr1 = synExpr1; expr2 = synExpr2) ->
                    [
                        dive synExpr1 synExpr1.Range traverseSynExpr
                        dive synExpr2 synExpr2.Range traverseSynExpr
                    ]
                    |> pick expr

                | SynExpr.For(identBody = synExpr1; toBody = synExpr2; doBody = synExpr3)
                | SynExpr.DotIndexedSet(objectExpr = synExpr1; indexArgs = synExpr2; valueExpr = synExpr3)
                | SynExpr.DotNamedIndexedPropertySet(targetExpr = synExpr1; argExpr = synExpr2; rhsExpr = synExpr3) ->
                    [
                        dive synExpr1 synExpr1.Range traverseSynExpr
                        dive synExpr2 synExpr2.Range traverseSynExpr
                        dive synExpr3 synExpr3.Range traverseSynExpr
                    ]
                    |> pick expr

                | SynExpr.TypeTest(expr = synExpr; targetType = synType)
                | SynExpr.Upcast(expr = synExpr; targetType = synType)
                | SynExpr.Downcast(expr = synExpr; targetType = synType) ->
                    [
                        dive synExpr synExpr.Range traverseSynExpr
                        dive synType synType.Range traverseSynType
                    ]
                    |> pick expr

                | SynExpr.LetOrUseBang(pat = synPat; rhs = synExpr; andBangs = andBangSynExprs; body = synExpr2) ->
                    [
                        yield dive synPat synPat.Range traversePat
                        yield dive synExpr synExpr.Range traverseSynExpr
                        yield!
                            [
                                for SynExprAndBang(pat = andBangSynPat; body = andBangSynExpr) in andBangSynExprs do
                                    yield (dive andBangSynPat andBangSynPat.Range traversePat)
                                    yield (dive andBangSynExpr andBangSynExpr.Range traverseSynExpr)
                            ]
                        yield dive synExpr2 synExpr2.Range traverseSynExpr
                    ]
                    |> pick expr

                | SynExpr.Dynamic _
                | SynExpr.Ident _
                | SynExpr.LongIdent _
                | SynExpr.Typar _
                | SynExpr.Const _
                | SynExpr.Null _
                | SynExpr.ImplicitZero _
                | SynExpr.LibraryOnlyILAssembly _
                | SynExpr.LibraryOnlyStaticOptimization _
                | SynExpr.LibraryOnlyUnionCaseFieldGet _
                | SynExpr.LibraryOnlyUnionCaseFieldSet _
                | SynExpr.ArbitraryAfterError _ -> None

            visitor.VisitExpr(origPath, traverseSynExpr origPath, defaultTraverse, expr)

        and traversePat origPath (pat: SynPat) =
            let defaultTraverse p =
                let path = SyntaxNode.SynPat p :: origPath

                match p with
                | SynPat.Paren(p, _) -> traversePat path p
                | SynPat.As(p1, p2, _)
                | SynPat.Or(p1, p2, _, _)
                | SynPat.ListCons(p1, p2, _, _) -> [ p1; p2 ] |> List.tryPick (traversePat path)
                | SynPat.Ands(ps, _)
                | SynPat.Tuple(elementPats = ps)
                | SynPat.ArrayOrList(_, ps, _) -> ps |> List.tryPick (traversePat path)
                | SynPat.Record(fieldPats = fieldPats) -> fieldPats |> List.tryPick (fun (_, _, p) -> traversePat path p)
                | SynPat.Attrib(p, attributes, m) ->
                    match traversePat path p with
                    | None -> attributeApplicationDives path attributes |> pick m attributes
                    | x -> x
                | SynPat.LongIdent(argPats = args) ->
                    match args with
                    | SynArgPats.Pats ps -> ps |> List.tryPick (traversePat path)
                    | SynArgPats.NamePatPairs(pats = ps) -> ps |> List.map (fun (_, _, pat) -> pat) |> List.tryPick (traversePat path)
                | SynPat.Typed(p, ty, _) ->
                    match traversePat path p with
                    | None -> traverseSynType path ty
                    | x -> x
                | SynPat.QuoteExpr(expr, _) -> traverseSynExpr path expr
                | _ -> None

            visitor.VisitPat(origPath, defaultTraverse, pat)

        and traverseSynSimplePats origPath (pat: SynPat) =
            match visitor.VisitSimplePats(origPath, pat) with
            | None ->
                let rec loop (pat: SynPat) =
                    match pat with
                    | SynPat.Paren(pat = pat)
                    | SynPat.Typed(pat = pat) -> loop pat
                    | SynPat.Tuple(elementPats = pats) -> List.tryPick loop pats
                    | SynPat.Attrib(_, attributes, m) -> attributeApplicationDives origPath attributes |> pick m attributes
                    | _ -> None

                loop pat
            | x -> x

        and traverseSynType origPath (StripParenTypes ty) =
            let defaultTraverse ty =
                let path = SyntaxNode.SynType ty :: origPath

                match ty with
                | SynType.App(typeName, _, typeArgs, _, _, _, _)
                | SynType.LongIdentApp(typeName, _, _, typeArgs, _, _, _) -> typeName :: typeArgs |> List.tryPick (traverseSynType path)
                | SynType.Fun(argType = ty1; returnType = ty2) -> [ ty1; ty2 ] |> List.tryPick (traverseSynType path)
                | SynType.MeasurePower(ty, _, _)
                | SynType.HashConstraint(ty, _)
                | SynType.WithNull(innerType = ty)
                | SynType.WithGlobalConstraints(ty, _, _)
                | SynType.Array(_, ty, _) -> traverseSynType path ty
                | SynType.StaticConstantNamed(ty1, ty2, _)
                | SynType.Or(ty1, ty2, _, _) -> [ ty1; ty2 ] |> List.tryPick (traverseSynType path)
                | SynType.Tuple(path = segments) -> getTypeFromTuplePath segments |> List.tryPick (traverseSynType path)
                | SynType.StaticConstantExpr(expr, _) -> traverseSynExpr [] expr
                | SynType.Paren(innerType = t)
                | SynType.SignatureParameter(usedType = t) -> traverseSynType path t
                | SynType.Intersection(types = types) -> List.tryPick (traverseSynType path) types
                | SynType.StaticConstantNull _
                | SynType.Anon _
                | SynType.AnonRecd _
                | SynType.LongIdent _
                | SynType.Var _
                | SynType.StaticConstant _
                | SynType.FromParseError _ -> None

            visitor.VisitType(origPath, defaultTraverse, ty)

        and normalizeMembersToDealWithPeculiaritiesOfGettersAndSetters path traverseInherit (synMemberDefns: SynMemberDefns) =
            synMemberDefns
            // property getters are setters are two members that can have the same range, so do some somersaults to deal with this
            |> Seq.map (fun mb ->
                match mb with
                | SynMemberDefn.GetSetMember(Some binding, None, m, _)
                | SynMemberDefn.GetSetMember(None, Some binding, m, _) ->
                    dive (SynMemberDefn.Member(binding, m)) m (traverseSynMemberDefn path traverseInherit)
                | SynMemberDefn.GetSetMember(Some getBinding, Some setBinding, m, _) ->
                    let traverse () =
                        match traverseSynMemberDefn path (fun _ -> None) (SynMemberDefn.Member(getBinding, m)) with
                        | Some _ as x -> x
                        | None -> traverseSynMemberDefn path (fun _ -> None) (SynMemberDefn.Member(setBinding, m))

                    m, traverse
                | mem -> dive mem mem.Range (traverseSynMemberDefn path traverseInherit))

        and traverseSynTypeDefn origPath (SynTypeDefn(synComponentInfo, synTypeDefnRepr, synMemberDefns, _, tRange, _) as tydef) =
            let path = SyntaxNode.SynTypeDefn tydef :: origPath

            match visitor.VisitComponentInfo(origPath, synComponentInfo) with
            | Some x -> Some x
            | None ->
                match synComponentInfo with
                | SynComponentInfo(attributes = attributes) ->
                    [
                        yield! attributeApplicationDives path attributes

                        match synTypeDefnRepr with
                        | SynTypeDefnRepr.Exception _ ->
                            // This node is generated in CheckExpressions.fs, not in the AST.
                            // But note exception declarations are missing from this tree walk.
                            ()
                        | SynTypeDefnRepr.ObjectModel(synTypeDefnKind, synMemberDefns, _oRange) ->
                            // traverse inherit function is used to capture type specific data required for processing Inherit part
                            let traverseInherit (synType: SynType, range: range) =
                                visitor.VisitInheritSynMemberDefn(path, synComponentInfo, synTypeDefnKind, synType, synMemberDefns, range)

                            yield!
                                synMemberDefns
                                |> normalizeMembersToDealWithPeculiaritiesOfGettersAndSetters path traverseInherit
                        | SynTypeDefnRepr.Simple(synTypeDefnSimpleRepr, _range) ->
                            match synTypeDefnSimpleRepr with
                            | SynTypeDefnSimpleRepr.Record(_synAccessOption, fields, m) ->
                                yield dive () synTypeDefnRepr.Range (fun () -> traverseRecordDefn path fields m)
                            | SynTypeDefnSimpleRepr.Union(_synAccessOption, cases, m) ->
                                yield dive () synTypeDefnRepr.Range (fun () -> traverseUnionDefn path cases m)
                            | SynTypeDefnSimpleRepr.Enum(cases, m) ->
                                yield dive () synTypeDefnRepr.Range (fun () -> traverseEnumDefn path cases m)
                            | SynTypeDefnSimpleRepr.TypeAbbrev(_, synType, m) ->
                                yield dive synTypeDefnRepr synTypeDefnRepr.Range (fun _ -> visitor.VisitTypeAbbrev(path, synType, m))
                            | _ -> ()
                        yield!
                            synMemberDefns
                            |> normalizeMembersToDealWithPeculiaritiesOfGettersAndSetters path (fun _ -> None)
                    ]
                    |> pick tRange tydef

        and traverseRecordDefn path fields m =
            fields
            |> List.tryPick (fun (SynField(attributes = attributes)) -> attributeApplicationDives path attributes |> pick m attributes)
            |> Option.orElseWith (fun () -> visitor.VisitRecordDefn(path, fields, m))

        and traverseEnumDefn path cases m =
            cases
            |> List.tryPick (fun (SynEnumCase(attributes = attributes)) -> attributeApplicationDives path attributes |> pick m attributes)
            |> Option.orElseWith (fun () -> visitor.VisitEnumDefn(path, cases, m))

        and traverseUnionDefn path cases m =
            cases
            |> List.tryPick (fun (SynUnionCase(attributes = attributes; caseType = caseType)) ->
                match attributeApplicationDives path attributes |> pick m attributes with
                | None ->
                    match caseType with
                    | SynUnionCaseKind.Fields fields ->
                        fields
                        |> List.tryPick (fun (SynField(attributes = attributes)) ->
                            attributeApplicationDives path attributes |> pick m attributes)
                    | _ -> None
                | x -> x)
            |> Option.orElseWith (fun () -> visitor.VisitUnionDefn(path, cases, m))

        and traverseSynMemberDefn path traverseInherit (m: SynMemberDefn) =
            let pick (debugObj: obj) = pick m.Range debugObj
            let path = SyntaxNode.SynMemberDefn m :: path

            match m with
            | SynMemberDefn.Open(_longIdent, _range) -> None
            | SynMemberDefn.Member(synBinding, _range) -> traverseSynBinding path synBinding
            | SynMemberDefn.GetSetMember(getBinding, setBinding, _, _) ->
                match getBinding, setBinding with
                | None, None -> None
                | Some binding, None
                | None, Some binding -> traverseSynBinding path binding
                | Some getBinding, Some setBinding ->
                    traverseSynBinding path getBinding
                    |> Option.orElseWith (fun () -> traverseSynBinding path setBinding)

            | SynMemberDefn.ImplicitCtor(ctorArgs = pat) -> traverseSynSimplePats path pat

            | SynMemberDefn.ImplicitInherit(synType, synExpr, _identOption, range, _) ->
                [
                    dive () synType.Range (fun () ->
                        match traverseInherit (synType, range) with
                        | None -> visitor.VisitImplicitInherit(path, traverseSynExpr path, synType, synExpr, range)
                        | x -> x)
                    dive () synExpr.Range (fun () -> visitor.VisitImplicitInherit(path, traverseSynExpr path, synType, synExpr, range))
                ]
                |> pick m
            | SynMemberDefn.AutoProperty(synExpr = synExpr; attributes = attributes) ->
                match traverseSynExpr path synExpr with
                | None -> attributeApplicationDives path attributes |> pick attributes
                | x -> x
            | SynMemberDefn.LetBindings(synBindingList, isRecursive, _, range) ->
                match visitor.VisitLetOrUse(path, isRecursive, traverseSynBinding path, synBindingList, range) with
                | None ->
                    synBindingList
                    |> List.map (fun x -> dive x x.RangeOfBindingWithRhs (traverseSynBinding path))
                    |> pick m
                | x -> x
            | SynMemberDefn.AbstractSlot(slotSig = SynValSig(synType = synType; attributes = attributes)) ->
                match traverseSynType path synType with
                | None -> attributeApplicationDives path attributes |> pick attributes
                | x -> x
            | SynMemberDefn.Interface(interfaceType = synType; members = synMemberDefnsOption) ->
                match visitor.VisitInterfaceSynMemberDefnType(path, synType) with
                | None ->
                    match synMemberDefnsOption with
                    | None -> None
                    | Some(x) ->
                        [
                            yield!
                                x
                                |> normalizeMembersToDealWithPeculiaritiesOfGettersAndSetters path (fun _ -> None)
                        ]

                        |> pick x
                | ok -> ok
            | SynMemberDefn.Inherit(Some synType, _identOption, range, _) -> traverseInherit (synType, range)
            | SynMemberDefn.Inherit(None, _, _, _) -> None
            | SynMemberDefn.ValField _ -> None
            | SynMemberDefn.NestedType(synTypeDefn, _synAccessOption, _range) -> traverseSynTypeDefn path synTypeDefn

        and traverseSynMatchClause origPath mc =
            let defaultTraverse mc =
                let path = SyntaxNode.SynMatchClause mc :: origPath

                match mc with
                | SynMatchClause(pat = synPat; whenExpr = synExprOption; resultExpr = synExpr) as all ->
                    [ dive synPat synPat.Range (traversePat path) ]
                    @ ([
                        match synExprOption with
                        | None -> ()
                        | Some guard -> yield guard
                        yield synExpr
                       ]
                       |> List.map (fun x -> dive x x.Range (traverseSynExpr path)))
                    |> pick all.Range all

            visitor.VisitMatchClause(origPath, defaultTraverse, mc)

        and traverseSynBinding origPath b =
            let defaultTraverse b =
                let path = SyntaxNode.SynBinding b :: origPath

                match b with
                | SynBinding(headPat = synPat; expr = synExpr; attributes = attributes; range = m) ->
                    [
                        yield! attributeApplicationDives path attributes
                        dive synPat synPat.Range (traversePat path)
                        dive synExpr synExpr.Range (traverseSynExpr path)
                    ]
                    |> pick m b

            visitor.VisitBinding(origPath, defaultTraverse, b)

        and attributeApplicationDives origPath attributes =
            attributes
            |> List.map (fun attributes -> dive () attributes.Range (fun () -> visitor.VisitAttributeApplication(origPath, attributes)))

        and traverseSynModuleOrNamespaceSig origPath (SynModuleOrNamespaceSig(decls = synModuleSigDecls; range = range) as mors) =
            match visitor.VisitModuleOrNamespaceSig(origPath, mors) with
            | Some x -> Some x
            | None ->
                let path = SyntaxNode.SynModuleOrNamespaceSig mors :: origPath

                synModuleSigDecls
                |> List.map (fun x -> dive x x.Range (traverseSynModuleSigDecl path))
                |> pick range mors

        and traverseSynModuleSigDecl origPath (decl: SynModuleSigDecl) =
            let pick = pick decl.Range

            let defaultTraverse m =
                let path = SyntaxNode.SynModuleSigDecl m :: origPath

                match m with
                | SynModuleSigDecl.ModuleAbbrev(_ident, _longIdent, _range) -> None
                | SynModuleSigDecl.NestedModule(moduleDecls = synModuleDecls; moduleInfo = SynComponentInfo(attributes = attributes)) ->
                    synModuleDecls
                    |> List.map (fun x -> dive x x.Range (traverseSynModuleSigDecl path))
                    |> List.append (attributeApplicationDives path attributes)
                    |> pick decl
                | SynModuleSigDecl.Val(synValSig, range) -> [ dive synValSig range (traverseSynValSig path) ] |> pick decl
                | SynModuleSigDecl.Types(types = types; range = range) ->
                    types
                    |> List.map (fun t -> dive t range (traverseSynTypeDefnSig path))
                    |> pick decl
                | SynModuleSigDecl.Exception(_synExceptionDefn, _range) -> None
                | SynModuleSigDecl.Open(_target, _range) -> None
                | SynModuleSigDecl.HashDirective(parsedHashDirective, range) -> visitor.VisitHashDirective(path, parsedHashDirective, range)
                | SynModuleSigDecl.NamespaceFragment synModuleOrNamespaceSig -> traverseSynModuleOrNamespaceSig path synModuleOrNamespaceSig

            visitor.VisitModuleSigDecl(origPath, defaultTraverse, decl)

        and traverseSynValSig origPath (valSig: SynValSig) =
            let defaultTraverse (SynValSig(synType = t; attributes = attributes; synExpr = expr; range = m)) =
                let path = SyntaxNode.SynValSig valSig :: origPath

                [
                    yield! attributeApplicationDives path attributes
                    yield dive t t.Range (traverseSynType path)
                    match expr with
                    | Some expr -> yield dive expr expr.Range (traverseSynExpr path)
                    | None -> ()
                ]
                |> pick m valSig

            visitor.VisitValSig(origPath, defaultTraverse, valSig)

        and traverseSynTypeDefnSig origPath (SynTypeDefnSig(synComponentInfo, typeRepr, members, tRange, _) as tydef) =
            let path = SyntaxNode.SynTypeDefnSig tydef :: origPath

            match visitor.VisitComponentInfo(origPath, synComponentInfo) with
            | Some x -> Some x
            | None ->
                match synComponentInfo with
                | SynComponentInfo(attributes = attributes) ->
                    [
                        yield! attributeApplicationDives path attributes

                        match typeRepr with
                        | SynTypeDefnSigRepr.Exception _ ->
                            // This node is generated in CheckExpressions.fs, not in the AST.
                            // But note exception declarations are missing from this tree walk.
                            ()
                        | SynTypeDefnSigRepr.ObjectModel(memberSigs = memberSigs) ->
                            yield! memberSigs |> List.map (fun ms -> dive ms ms.Range (traverseSynMemberSig path))
                        | SynTypeDefnSigRepr.Simple(synTypeDefnSimpleRepr, _range) ->
                            match synTypeDefnSimpleRepr with
                            | SynTypeDefnSimpleRepr.Record(_synAccessOption, fields, m) ->
                                yield dive () typeRepr.Range (fun () -> traverseRecordDefn path fields m)
                            | SynTypeDefnSimpleRepr.Union(_synAccessOption, cases, m) ->
                                yield dive () typeRepr.Range (fun () -> traverseUnionDefn path cases m)
                            | SynTypeDefnSimpleRepr.Enum(cases, m) -> yield dive () typeRepr.Range (fun () -> traverseEnumDefn path cases m)
                            | SynTypeDefnSimpleRepr.TypeAbbrev(_, synType, m) ->
                                yield dive typeRepr typeRepr.Range (fun _ -> visitor.VisitTypeAbbrev(path, synType, m))
                            | _ -> ()
                        yield! members |> List.map (fun ms -> dive ms ms.Range (traverseSynMemberSig path))
                    ]
                    |> pick tRange tydef

        and traverseSynMemberSig path (m: SynMemberSig) =
            let path = SyntaxNode.SynMemberSig m :: path

            match m with
            | SynMemberSig.Member(memberSig = memberSig) -> traverseSynValSig path memberSig
            | SynMemberSig.Interface(interfaceType = synType) -> traverseSynType path synType
            | SynMemberSig.Inherit(inheritedType = synType) -> traverseSynType path synType
            | SynMemberSig.ValField(field = SynField(attributes = attributes)) ->
                attributeApplicationDives path attributes |> pick m.Range attributes
            | SynMemberSig.NestedType(nestedType = nestedType) -> traverseSynTypeDefnSig path nestedType

        let fileRange =
            (range0, ast) ||> List.fold (fun acc node -> unionRanges acc node.Range)

        ast
        |> List.map (fun node ->
            match node with
            | SyntaxNode.SynModuleOrNamespace moduleOrNamespace ->
                dive moduleOrNamespace moduleOrNamespace.Range (traverseSynModuleOrNamespace [])
            | SyntaxNode.SynModuleOrNamespaceSig moduleOrNamespaceSig ->
                dive moduleOrNamespaceSig moduleOrNamespaceSig.Range (traverseSynModuleOrNamespaceSig [])
            | SyntaxNode.SynPat pat -> dive pat pat.Range (traversePat [])
            | SyntaxNode.SynType ty -> dive ty ty.Range (traverseSynType [])
            | SyntaxNode.SynExpr expr -> dive expr expr.Range (traverseSynExpr [])
            | SyntaxNode.SynModule modul -> dive modul modul.Range (traverseSynModuleDecl [])
            | SyntaxNode.SynTypeDefn tyDef -> dive tyDef tyDef.Range (traverseSynTypeDefn [])
            | SyntaxNode.SynMemberDefn memberDef -> dive memberDef memberDef.Range (traverseSynMemberDefn [] (fun _ -> None))
            | SyntaxNode.SynMatchClause matchClause -> dive matchClause matchClause.Range (traverseSynMatchClause [])
            | SyntaxNode.SynBinding binding -> dive binding binding.RangeOfBindingWithRhs (traverseSynBinding [])
            | SyntaxNode.SynModuleSigDecl moduleSigDecl -> dive moduleSigDecl moduleSigDecl.Range (traverseSynModuleSigDecl [])
            | SyntaxNode.SynValSig(SynValSig.SynValSig(range = range) as valSig) -> dive valSig range (traverseSynValSig [])
            | SyntaxNode.SynTypeDefnSig tyDefSig -> dive tyDefSig tyDefSig.Range (traverseSynTypeDefnSig [])
            | SyntaxNode.SynMemberSig memberSig -> dive memberSig memberSig.Range (traverseSynMemberSig []))
        |> pick fileRange ast

    /// traverse an implementation file walking all the way down to SynExpr or TypeAbbrev at a particular location
    ///
    let Traverse (pos: pos, parseTree: ParsedInput, visitor: SyntaxVisitorBase<'T>) =
        traverseUntil pick pos visitor parseTree.Contents

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SyntaxNode =
    let (|Attributes|) node =
        let (|All|) = List.collect
        let field (SynField(attributes = attributes)) = attributes
        let unionCase (SynUnionCase(attributes = attributes)) = attributes
        let enumCase (SynEnumCase(attributes = attributes)) = attributes
        let typar (SynTyparDecl(attributes = attributes)) = attributes

        let (|SynComponentInfo|) componentInfo =
            match componentInfo with
            | SynComponentInfo(attributes = attributes; typeParams = Some(SynTyparDecls.PrefixList(decls = All typar attributes')))
            | SynComponentInfo(attributes = attributes; typeParams = Some(SynTyparDecls.PostfixList(decls = All typar attributes')))
            | SynComponentInfo(
                attributes = attributes; typeParams = Some(SynTyparDecls.SinglePrefix(decl = SynTyparDecl(attributes = attributes')))) ->
                attributes @ attributes'
            | SynComponentInfo(attributes = attributes) -> attributes

        let (|SynBinding|) binding =
            match binding with
            | SynBinding(attributes = attributes; returnInfo = Some(SynBindingReturnInfo(attributes = attributes'))) ->
                attributes @ attributes'
            | SynBinding(attributes = attributes) -> attributes

        match node with
        | SyntaxNode.SynModuleOrNamespace(SynModuleOrNamespace(attribs = attributes))
        | SyntaxNode.SynModuleOrNamespaceSig(SynModuleOrNamespaceSig(attribs = attributes))
        | SyntaxNode.SynModule(SynModuleDecl.Attributes(attributes = attributes))
        | SyntaxNode.SynTypeDefn(SynTypeDefn(typeInfo = SynComponentInfo attributes))
        | SyntaxNode.SynTypeDefn(SynTypeDefn(
            typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Record(recordFields = All field attributes), _)))
        | SyntaxNode.SynTypeDefn(SynTypeDefn(
            typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Union(unionCases = All unionCase attributes), _)))
        | SyntaxNode.SynTypeDefn(SynTypeDefn(
            typeRepr = SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.Enum(cases = All enumCase attributes), _)))
        | SyntaxNode.SynMemberDefn(SynMemberDefn.AutoProperty(attributes = attributes))
        | SyntaxNode.SynMemberDefn(SynMemberDefn.AbstractSlot(slotSig = SynValSig(attributes = attributes)))
        | SyntaxNode.SynMemberDefn(SynMemberDefn.ImplicitCtor(attributes = attributes))
        | SyntaxNode.SynBinding(SynBinding attributes)
        | SyntaxNode.SynPat(SynPat.Attrib(attributes = attributes))
        | SyntaxNode.SynType(SynType.SignatureParameter(attributes = attributes))
        | SyntaxNode.SynValSig(SynValSig(attributes = attributes)) -> attributes
        | _ -> []

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal SyntaxNodes =
    let fold folder state (ast: SyntaxNode list) =
        let mutable state = state

        let visitor =
            { new SyntaxVisitorBase<unit>() with
                member _.VisitExpr(path, _, defaultTraverse, expr) =
                    match path with
                    | SyntaxNode.SynMemberDefn _ as parent :: path -> state <- folder state path parent
                    | _ -> ()

                    state <- folder state path (SyntaxNode.SynExpr expr)
                    defaultTraverse expr

                member _.VisitPat(path, defaultTraverse, pat) =
                    state <- folder state path (SyntaxNode.SynPat pat)
                    defaultTraverse pat

                member _.VisitType(path, defaultTraverse, synType) =
                    match path with
                    | SyntaxNode.SynMemberDefn _ | SyntaxNode.SynMemberSig _ as parent :: path -> state <- folder state path parent
                    | _ -> ()

                    state <- folder state path (SyntaxNode.SynType synType)
                    defaultTraverse synType

                member _.VisitModuleDecl(path, defaultTraverse, synModuleDecl) =
                    state <- folder state path (SyntaxNode.SynModule synModuleDecl)

                    match synModuleDecl with
                    | SynModuleDecl.Types(types, _) ->
                        let path = SyntaxNode.SynModule synModuleDecl :: path

                        for ty in types do
                            state <- folder state path (SyntaxNode.SynTypeDefn ty)

                    | _ -> ()

                    defaultTraverse synModuleDecl

                member _.VisitModuleOrNamespace(path, synModuleOrNamespace) =
                    state <- folder state path (SyntaxNode.SynModuleOrNamespace synModuleOrNamespace)
                    None

                member _.VisitMatchClause(path, defaultTraverse, matchClause) =
                    state <- folder state path (SyntaxNode.SynMatchClause matchClause)
                    defaultTraverse matchClause

                member _.VisitBinding(path, defaultTraverse, synBinding) =
                    match path with
                    | SyntaxNode.SynMemberDefn _ as parent :: path -> state <- folder state path parent
                    | _ -> ()

                    state <- folder state path (SyntaxNode.SynBinding synBinding)
                    defaultTraverse synBinding

                member _.VisitModuleOrNamespaceSig(path, synModuleOrNamespaceSig) =
                    state <- folder state path (SyntaxNode.SynModuleOrNamespaceSig synModuleOrNamespaceSig)
                    None

                member _.VisitModuleSigDecl(path, defaultTraverse, synModuleSigDecl) =
                    state <- folder state path (SyntaxNode.SynModuleSigDecl synModuleSigDecl)

                    match synModuleSigDecl with
                    | SynModuleSigDecl.Types(types, _) ->
                        let path = SyntaxNode.SynModuleSigDecl synModuleSigDecl :: path

                        for ty in types do
                            state <- folder state path (SyntaxNode.SynTypeDefnSig ty)

                    | _ -> ()

                    defaultTraverse synModuleSigDecl

                member _.VisitValSig(path, defaultTraverse, valSig) =
                    match path with
                    | SyntaxNode.SynMemberSig _ as parent :: path -> state <- folder state path parent
                    | _ -> ()

                    state <- folder state path (SyntaxNode.SynValSig valSig)
                    defaultTraverse valSig

                member _.VisitSimplePats(path, _pat) =
                    match path with
                    | SyntaxNode.SynMemberDefn _ as node :: path -> state <- folder state path node
                    | _ -> ()

                    None

                member _.VisitInterfaceSynMemberDefnType(path, _synType) =
                    match path with
                    | SyntaxNode.SynMemberDefn _ as node :: path -> state <- folder state path node
                    | _ -> ()

                    None
            }

        let pickAll _ _ _ diveResults =
            let rec loop diveResults =
                match diveResults with
                | [] -> None
                | (_, project) :: rest ->
                    ignore (project ())
                    loop rest

            loop diveResults

        let m = (range0, ast) ||> List.fold (fun acc node -> unionRanges acc node.Range)
        ignore<unit option> (SyntaxTraversal.traverseUntil pickAll m.End visitor ast)
        state

    let private foldWhileImpl pick pos folder state (ast: SyntaxNode list) =
        let mutable state = state

        let visitor =
            { new SyntaxVisitorBase<unit>() with
                member _.VisitExpr(path, _, defaultTraverse, expr) =
                    match path with
                    | SyntaxNode.SynMemberDefn _ as parent :: path ->
                        match folder state path parent with
                        | Some state' ->
                            match folder state' path (SyntaxNode.SynExpr expr) with
                            | Some state' ->
                                state <- state'
                                defaultTraverse expr
                            | None -> Some()
                        | None -> Some()
                    | _ ->
                        match folder state path (SyntaxNode.SynExpr expr) with
                        | Some state' ->
                            state <- state'
                            defaultTraverse expr
                        | None -> Some()

                member _.VisitPat(path, defaultTraverse, pat) =
                    match folder state path (SyntaxNode.SynPat pat) with
                    | Some state' ->
                        state <- state'
                        defaultTraverse pat
                    | None -> Some()

                member _.VisitType(path, defaultTraverse, synType) =
                    match path with
                    | SyntaxNode.SynMemberDefn _ | SyntaxNode.SynMemberSig _ as parent :: path ->
                        match folder state path parent with
                        | Some state' ->
                            match folder state' path (SyntaxNode.SynType synType) with
                            | Some state' ->
                                state <- state'
                                defaultTraverse synType
                            | None -> Some()
                        | None -> Some()
                    | _ ->
                        match folder state path (SyntaxNode.SynType synType) with
                        | Some state' ->
                            state <- state'
                            defaultTraverse synType
                        | None -> Some()

                member _.VisitModuleDecl(path, defaultTraverse, synModuleDecl) =
                    match folder state path (SyntaxNode.SynModule synModuleDecl) with
                    | Some state' ->
                        state <- state'

                        match synModuleDecl with
                        | SynModuleDecl.Types(types, _) ->
                            let path = SyntaxNode.SynModule synModuleDecl :: path

                            let rec loop types =
                                match types with
                                | [] -> defaultTraverse synModuleDecl
                                | ty :: types ->
                                    match folder state path (SyntaxNode.SynTypeDefn ty) with
                                    | Some state' ->
                                        state <- state'
                                        loop types
                                    | None -> Some()

                            loop types

                        | _ -> defaultTraverse synModuleDecl

                    | None -> Some()

                member _.VisitModuleOrNamespace(path, synModuleOrNamespace) =
                    match folder state path (SyntaxNode.SynModuleOrNamespace synModuleOrNamespace) with
                    | Some state' ->
                        state <- state'
                        None
                    | None -> Some()

                member _.VisitMatchClause(path, defaultTraverse, matchClause) =
                    match folder state path (SyntaxNode.SynMatchClause matchClause) with
                    | Some state' ->
                        state <- state'
                        defaultTraverse matchClause
                    | None -> Some()

                member _.VisitBinding(path, defaultTraverse, synBinding) =
                    match path with
                    | SyntaxNode.SynMemberDefn _ as parent :: path ->
                        match folder state path parent with
                        | Some state' ->
                            match folder state' path (SyntaxNode.SynBinding synBinding) with
                            | Some state' ->
                                state <- state'
                                defaultTraverse synBinding
                            | None -> Some()
                        | None -> Some()
                    | _ ->
                        match folder state path (SyntaxNode.SynBinding synBinding) with
                        | Some state' ->
                            state <- state'
                            defaultTraverse synBinding
                        | None -> Some()

                member _.VisitModuleOrNamespaceSig(path, synModuleOrNamespaceSig) =
                    match folder state path (SyntaxNode.SynModuleOrNamespaceSig synModuleOrNamespaceSig) with
                    | Some state' ->
                        state <- state'
                        None
                    | None -> Some()

                member _.VisitModuleSigDecl(path, defaultTraverse, synModuleSigDecl) =
                    match folder state path (SyntaxNode.SynModuleSigDecl synModuleSigDecl) with
                    | Some state' ->
                        state <- state'

                        match synModuleSigDecl with
                        | SynModuleSigDecl.Types(types, _) ->
                            let path = SyntaxNode.SynModuleSigDecl synModuleSigDecl :: path

                            let rec loop types =
                                match types with
                                | [] -> defaultTraverse synModuleSigDecl
                                | ty :: types ->
                                    match folder state path (SyntaxNode.SynTypeDefnSig ty) with
                                    | Some state' ->
                                        state <- state'
                                        loop types
                                    | None -> Some()

                            loop types

                        | _ -> defaultTraverse synModuleSigDecl

                    | None -> Some()

                member _.VisitValSig(path, defaultTraverse, valSig) =
                    match path with
                    | SyntaxNode.SynMemberSig _ as parent :: path ->
                        match folder state path parent with
                        | Some state' ->
                            match folder state' path (SyntaxNode.SynValSig valSig) with
                            | Some state' ->
                                state <- state'
                                defaultTraverse valSig
                            | None -> Some()
                        | None -> Some()
                    | _ ->
                        match folder state path (SyntaxNode.SynValSig valSig) with
                        | Some state' ->
                            state <- state'
                            defaultTraverse valSig
                        | None -> Some()

                member _.VisitSimplePats(path, _pat) =
                    match path with
                    | SyntaxNode.SynMemberDefn _ as node :: path ->
                        match folder state path node with
                        | Some state' ->
                            state <- state'
                            None
                        | None -> Some()
                    | _ -> None

                member _.VisitInterfaceSynMemberDefnType(path, _synType) =
                    match path with
                    | SyntaxNode.SynMemberDefn _ as node :: path ->
                        match folder state path node with
                        | Some state' ->
                            state <- state'
                            None
                        | None -> Some()
                    | _ -> None
            }

        ignore<unit option> (SyntaxTraversal.traverseUntil pick pos visitor ast)
        state

    let foldWhile folder state (ast: SyntaxNode list) =
        let pickAll _ _ _ diveResults =
            let rec loop diveResults =
                match diveResults with
                | [] -> None
                | (_, project) :: rest ->
                    ignore (project ())
                    loop rest

            loop diveResults

        let m = (range0, ast) ||> List.fold (fun acc node -> unionRanges acc node.Range)
        foldWhileImpl pickAll m.End folder state ast

    let tryPick chooser position (ast: SyntaxNode list) =
        let visitor =
            { new SyntaxVisitorBase<'T>() with
                member _.VisitExpr(path, _, defaultTraverse, expr) =
                    (match path with
                     | SyntaxNode.SynMemberDefn _ as parent :: parentPath -> chooser parentPath parent
                     | _ -> None)
                    |> Option.orElseWith (fun () -> chooser path (SyntaxNode.SynExpr expr))
                    |> Option.orElseWith (fun () -> defaultTraverse expr)

                member _.VisitPat(path, defaultTraverse, pat) =
                    chooser path (SyntaxNode.SynPat pat)
                    |> Option.orElseWith (fun () -> defaultTraverse pat)

                member _.VisitType(path, defaultTraverse, synType) =
                    (match path with
                     | SyntaxNode.SynMemberDefn _ | SyntaxNode.SynMemberSig _ as parent :: parentPath -> chooser parentPath parent
                     | _ -> None)
                    |> Option.orElseWith (fun () -> chooser path (SyntaxNode.SynType synType))
                    |> Option.orElseWith (fun () -> defaultTraverse synType)

                member _.VisitModuleDecl(path, defaultTraverse, synModuleDecl) =
                    chooser path (SyntaxNode.SynModule synModuleDecl)
                    |> Option.orElseWith (fun () ->
                        match synModuleDecl with
                        | SynModuleDecl.Types(types, _) ->
                            let path = SyntaxNode.SynModule synModuleDecl :: path
                            types |> List.tryPick (SyntaxNode.SynTypeDefn >> chooser path)
                        | _ -> None)
                    |> Option.orElseWith (fun () -> defaultTraverse synModuleDecl)

                member _.VisitModuleOrNamespace(path, synModuleOrNamespace) =
                    chooser path (SyntaxNode.SynModuleOrNamespace synModuleOrNamespace)

                member _.VisitMatchClause(path, defaultTraverse, matchClause) =
                    chooser path (SyntaxNode.SynMatchClause matchClause)
                    |> Option.orElseWith (fun () -> defaultTraverse matchClause)

                member _.VisitBinding(path, defaultTraverse, synBinding) =
                    (match path with
                     | SyntaxNode.SynMemberDefn _ as parent :: parentPath -> chooser parentPath parent
                     | _ -> None)
                    |> Option.orElseWith (fun () -> chooser path (SyntaxNode.SynBinding synBinding))
                    |> Option.orElseWith (fun () -> defaultTraverse synBinding)

                member _.VisitModuleOrNamespaceSig(path, synModuleOrNamespaceSig) =
                    chooser path (SyntaxNode.SynModuleOrNamespaceSig synModuleOrNamespaceSig)

                member _.VisitModuleSigDecl(path, defaultTraverse, synModuleSigDecl) =
                    chooser path (SyntaxNode.SynModuleSigDecl synModuleSigDecl)
                    |> Option.orElseWith (fun () ->
                        match synModuleSigDecl with
                        | SynModuleSigDecl.Types(types, _) ->
                            let path = SyntaxNode.SynModuleSigDecl synModuleSigDecl :: path
                            types |> List.tryPick (SyntaxNode.SynTypeDefnSig >> chooser path)
                        | _ -> None)
                    |> Option.orElseWith (fun () -> defaultTraverse synModuleSigDecl)

                member _.VisitValSig(path, defaultTraverse, valSig) =
                    (match path with
                     | SyntaxNode.SynMemberSig _ as parent :: parentPath -> chooser parentPath parent
                     | _ -> None)
                    |> Option.orElseWith (fun () -> chooser path (SyntaxNode.SynValSig valSig))
                    |> Option.orElseWith (fun () -> defaultTraverse valSig)

                member _.VisitSimplePats(path, _pat) =
                    match path with
                    | SyntaxNode.SynMemberDefn _ as node :: path -> chooser path node
                    | _ -> None

                member _.VisitInterfaceSynMemberDefnType(path, _synType) =
                    match path with
                    | SyntaxNode.SynMemberDefn _ as node :: path -> chooser path node
                    | _ -> None
            }

        SyntaxTraversal.traverseUntil SyntaxTraversal.pick position visitor ast

    let tryPickLast chooser position (ast: SyntaxNode list) =
        (None, ast)
        ||> foldWhileImpl SyntaxTraversal.pick position (fun prev path node ->
            match chooser path node with
            | Some _ as next -> Some next
            | None -> Some prev)

    let tryNode position (ast: SyntaxNode list) =
        let Matching = Some

        (None, ast)
        ||> foldWhileImpl SyntaxTraversal.pick position (fun _prev path node ->
            if rangeContainsPos node.Range position then
                Some(Matching(node, path))
            else
                None)

    let exists predicate position ast =
        tryPick (fun path node -> if predicate path node then Some() else None) position ast
        |> Option.isSome

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ParsedInput =
    let fold folder state (parsedInput: ParsedInput) =
        SyntaxNodes.fold folder state parsedInput.Contents

    let foldWhile folder state (parsedInput: ParsedInput) =
        SyntaxNodes.foldWhile folder state parsedInput.Contents

    let tryPick chooser position (parsedInput: ParsedInput) =
        SyntaxNodes.tryPick chooser position parsedInput.Contents

    let tryPickLast chooser position (parsedInput: ParsedInput) =
        SyntaxNodes.tryPickLast chooser position parsedInput.Contents

    let tryNode position (parsedInput: ParsedInput) =
        SyntaxNodes.tryNode position parsedInput.Contents

    let exists predicate position (parsedInput: ParsedInput) =
        SyntaxNodes.exists predicate position parsedInput.Contents
