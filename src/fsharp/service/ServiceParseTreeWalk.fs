// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast
 

/// A range of utility functions to assist with traversing an AST
module public AstTraversal =
    // treat ranges as though they are half-open: [,)
    let rangeContainsPosLeftEdgeInclusive (m1:range) p =
        if posEq m1.Start m1.End then
            // the parser doesn't produce zero-width ranges, except in one case, for e.g. a block of lets that lacks a body
            // we treat the range [n,n) as containing position n
            posGeq p m1.Start &&
            posGeq m1.End p
        else
            posGeq p m1.Start &&   // [
            posGt m1.End p         // )

    // treat ranges as though they are fully open: (,)
    let rangeContainsPosEdgesExclusive (m1:range) p = posGt p m1.Start && posGt m1.End p

    /// used to track route during traversal AST
    [<RequireQualifiedAccess>]
    type TraverseStep = 
        | Expr of SynExpr
        | Module of SynModuleDecl
        | ModuleOrNamespace of SynModuleOrNamespace
        | TypeDefn of SynTypeDefn
        | MemberDefn of SynMemberDefn
        | MatchClause of SynMatchClause
        | Binding of SynBinding

    type TraversePath = TraverseStep list

    [<AbstractClass>]
    type AstVisitorBase<'T>() =
        /// VisitExpr(path, traverseSynExpr, defaultTraverse, expr)
        /// controls the behavior when a SynExpr is reached; it can just do 
        ///          defaultTraverse(expr)      if you have no special logic for this node, and want the default processing to pick which sub-node to dive deeper into
        /// or can inject non-default behavior, which might incorporate:
        ///          traverseSynExpr(subExpr)   to recurse deeper on some particular sub-expression based on your own logic
        /// path helps to track AST nodes that were passed during traversal
        abstract VisitExpr : TraversePath * (SynExpr -> 'T option) * (SynExpr -> 'T option) * SynExpr -> 'T option
        /// VisitTypeAbbrev(ty,m), defaults to ignoring this leaf of the AST
        abstract VisitTypeAbbrev : SynType * range -> 'T option
        default this.VisitTypeAbbrev(_ty,_m) = None
        /// VisitImplicitInherit(defaultTraverse,ty,expr,m), defaults to just visiting expr
        abstract VisitImplicitInherit : (SynExpr -> 'T option) * SynType * SynExpr * range -> 'T option
        default this.VisitImplicitInherit(defaultTraverse, _ty, expr, _m) = defaultTraverse expr
        /// VisitModuleDecl allows overriding module declaration behavior
        abstract VisitModuleDecl : (SynModuleDecl -> 'T option) * SynModuleDecl -> 'T option
        default this.VisitModuleDecl(defaultTraverse, decl) = defaultTraverse decl
        /// VisitBinding allows overriding binding behavior (note: by default it would defaultTraverse expression)
        abstract VisitBinding : (SynBinding -> 'T option) * SynBinding -> 'T option
        default this.VisitBinding(defaultTraverse, binding) = defaultTraverse binding
        /// VisitMatchClause allows overriding clause behavior (note: by default it would defaultTraverse expression)
        abstract VisitMatchClause : (SynMatchClause -> 'T option) * SynMatchClause -> 'T option
        default this.VisitMatchClause(defaultTraverse, mc) = defaultTraverse mc
        /// VisitInheritSynMemberDefn allows overriding inherit behavior (by default do nothing)
        abstract VisitInheritSynMemberDefn : SynComponentInfo * SynTypeDefnKind * SynType  * SynMemberDefns * range -> 'T option
        default this.VisitInheritSynMemberDefn(_componentInfo, _typeDefnKind, _synType, _members, _range) = None
        /// VisitInterfaceSynMemberDefnType allows overriding behavior for visiting interface member in types (by default - do nothing)
        abstract VisitInterfaceSynMemberDefnType : SynType -> 'T option
        default this.VisitInterfaceSynMemberDefnType(_synType) = None
        /// VisitRecordField allows overriding behavior when visiting l.h.s. of constructed record instances
        abstract VisitRecordField : TraversePath * SynExpr option * LongIdentWithDots option -> 'T option
        default this.VisitRecordField (_path, _copyOpt, _recordField) = None
        /// VisitHashDirective allows overriding behavior when visiting hash directives in FSX scripts, like #r, #load and #I.
        abstract VisitHashDirective : range -> 'T option
        default this.VisitHashDirective (_) = None
        /// VisitModuleOrNamespace allows overriding behavior when visiting module or namespaces
        abstract VisitModuleOrNamespace : SynModuleOrNamespace -> 'T option
        default this.VisitModuleOrNamespace (_) = None
        /// VisitComponentInfo allows overriding behavior when visiting type component infos 
        abstract VisitComponentInfo : SynComponentInfo -> 'T option
        default this.VisitComponentInfo (_) = None
        /// VisitLetOrUse allows overriding behavior when visiting module or local let or use bindings
        abstract VisitLetOrUse : SynBinding list * range -> 'T option
        default this.VisitLetOrUse (_, _) = None
        /// VisitType allows overriding behavior when visiting simple pats
        abstract VisitSimplePats : SynSimplePat list -> 'T option
        default this.VisitSimplePats (_) = None
        /// VisitPat allows overriding behavior when visiting patterns
        abstract VisitPat : (SynPat -> 'T option) * SynPat -> 'T option
        default this.VisitPat (defaultTraverse, pat) = defaultTraverse pat
        /// VisitType allows overriding behavior when visiting type hints (x: ..., etc.)
        abstract VisitType : (SynType -> 'T option) * SynType -> 'T option
        default this.VisitType (defaultTraverse, ty) = defaultTraverse ty

    let dive node range project =
        range,(fun() -> project node)

    let pick pos (outerRange:range) (_debugObj:obj) (diveResults:list<range*_>) =
        match diveResults with
        | [] -> None
        | _ ->
        let isOrdered =
#if DEBUG
            // ranges in a dive-and-pick group should be ordered
            diveResults |> Seq.pairwise |> Seq.forall (fun ((r1,_),(r2,_)) -> posGeq r2.Start r1.End)
#else
            true
#endif
        if not isOrdered then
            let s = sprintf "ServiceParseTreeWalk: not isOrdered: %A" (diveResults |> List.map (fun (r,_) -> r.ToShortString()))
            ignore s
            //System.Diagnostics.Debug.Assert(false, s)
        let outerContainsInner =
#if DEBUG
            // ranges in a dive-and-pick group should be "under" the thing that contains them
            let innerTotalRange = diveResults |> List.map fst  |> List.reduce unionRanges
            rangeContainsRange outerRange innerTotalRange
#else
            ignore(outerRange)
            true
#endif
        if not outerContainsInner then
            let s = sprintf "ServiceParseTreeWalk: not outerContainsInner: %A : %A" (outerRange.ToShortString()) (diveResults |> List.map (fun (r,_) -> r.ToShortString()))
            ignore s
            //System.Diagnostics.Debug.Assert(false, s)
        let isZeroWidth(r:range) =
            posEq r.Start r.End // the parser inserts some zero-width elements to represent the completions of incomplete constructs, but we should never 'dive' into them, since they don't represent actual user code
        match List.choose (fun (r,f) -> if rangeContainsPosLeftEdgeInclusive r pos && not(isZeroWidth r) then Some(f) else None) diveResults with 
        | [] -> 
            // No entity's range contained the desired position.  However the ranges in the parse tree only span actual characters present in the file.  
            // The cursor may be at whitespace between entities or after everything, so find the nearest entity with the range left of the position.
            let mutable e = diveResults.Head
            for r in diveResults do
                if posGt pos (fst r).Start then
                    e <- r
            snd(e)()
        | [x] -> x()
        | _ -> 
#if DEBUG
            assert(false)
            failwithf "multiple disjoint AST node ranges claimed to contain (%A) from %+A" pos _debugObj
#else
            None
#endif

    /// traverse an implementation file walking all the way down to SynExpr or TypeAbbrev at a particular location
    ///
    let Traverse(pos:pos, parseTree, visitor:AstVisitorBase<'T>) =
        let pick x = pick pos x
        let rec traverseSynModuleDecl path (decl:SynModuleDecl) =
            let pick = pick decl.Range
            let defaultTraverse m = 
                let path = TraverseStep.Module m :: path
                match m with
                | SynModuleDecl.ModuleAbbrev(_ident, _longIdent, _range) -> None
                | SynModuleDecl.NestedModule(_synComponentInfo, _isRec, synModuleDecls, _, _range) -> synModuleDecls |> List.map (fun x -> dive x x.Range (traverseSynModuleDecl path)) |> pick decl
                | SynModuleDecl.Let(_, synBindingList, range) ->
                    match visitor.VisitLetOrUse(synBindingList, range) with
                    | Some x -> Some x
                    | None -> synBindingList |> List.map (fun x -> dive x x.RangeOfBindingAndRhs (traverseSynBinding path)) |> pick decl
                | SynModuleDecl.DoExpr(_sequencePointInfoForBinding, synExpr, _range) -> traverseSynExpr path synExpr  
                | SynModuleDecl.Types(synTypeDefnList, _range) -> synTypeDefnList |> List.map (fun x -> dive x x.Range (traverseSynTypeDefn path)) |> pick decl
                | SynModuleDecl.Exception(_synExceptionDefn, _range) -> None
                | SynModuleDecl.Open(_longIdent, _range) -> None
                | SynModuleDecl.Attributes(_synAttributes, _range) -> None
                | SynModuleDecl.HashDirective(_parsedHashDirective, range) -> visitor.VisitHashDirective range
                | SynModuleDecl.NamespaceFragment(synModuleOrNamespace) -> traverseSynModuleOrNamespace path synModuleOrNamespace
            visitor.VisitModuleDecl(defaultTraverse, decl)

        and traverseSynModuleOrNamespace path (SynModuleOrNamespace(_longIdent, _isRec, _isModule, synModuleDecls, _preXmlDoc, _synAttributes, _synAccessOpt, range) as mors) =
            match visitor.VisitModuleOrNamespace(mors) with
            | Some x -> Some x
            | None ->
                let path = TraverseStep.ModuleOrNamespace mors :: path
                synModuleDecls |> List.map (fun x -> dive x x.Range (traverseSynModuleDecl path)) |> pick range mors

        and traverseSynExpr path (expr:SynExpr) =
            let pick = pick expr.Range
            let defaultTraverse e = 
                let origPath = path
                let path = TraverseStep.Expr e :: path
                let traverseSynExpr = traverseSynExpr path
                match e with
                | SynExpr.Paren(synExpr, _, _, _parenRange) -> traverseSynExpr synExpr
                | SynExpr.Quote(_synExpr, _, synExpr2, _, _range) -> 
                    [//dive synExpr synExpr.Range traverseSynExpr // TODO, what is this?
                     dive synExpr2 synExpr2.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.Const(_synConst, _range) -> None
                | SynExpr.Typed(synExpr, synType, _range) -> [ traverseSynExpr synExpr; traverseSynType synType ] |> List.tryPick id
                | SynExpr.Tuple(synExprList, _, _range) 
                | SynExpr.StructTuple(synExprList, _, _range) -> synExprList |> List.map (fun x -> dive x x.Range traverseSynExpr) |> pick expr
                | SynExpr.ArrayOrList(_, synExprList, _range) -> synExprList |> List.map (fun x -> dive x x.Range traverseSynExpr) |> pick expr
                | SynExpr.Record(inheritOpt,copyOpt,fields, _range) -> 
                    [ 
                        let diveIntoSeparator offsideColumn scPosOpt copyOpt  = 
                            match scPosOpt with
                            | Some scPos -> 
                                if posGeq pos scPos then 
                                    visitor.VisitRecordField(path, copyOpt, None) // empty field after the inherits
                                else None
                            | None -> 
                                //semicolon position is not available - use offside rule
                                if pos.Column = offsideColumn then
                                    visitor.VisitRecordField(path, copyOpt, None) // empty field after the inherits
                                else None

                        match inheritOpt with
                        | Some(_ty,expr, _range, sepOpt, inheritRange) -> 
                            // dive into argument
                            yield dive expr expr.Range (fun expr ->
                                // special-case:caret is located in the offside position below inherit 
                                // inherit A()
                                // $
                                if not (rangeContainsPos expr.Range pos) && sepOpt.IsNone && pos.Column = inheritRange.StartColumn then
                                    visitor.VisitRecordField(path, None, None)
                                else
                                    traverseSynExpr expr
                                )
                            match sepOpt with
                            | Some (sep, scPosOpt) ->
                                yield dive () sep (fun () ->
                                    // special case: caret is below 'inherit' + one or more fields are already defined
                                    // inherit A()
                                    // $
                                    // field1 = 5
                                    diveIntoSeparator inheritRange.StartColumn scPosOpt None
                                    )
                            | None -> ()
                        | _ -> ()
                        match copyOpt with
                        | Some(expr, (withRange, _)) -> 
                            yield dive expr expr.Range traverseSynExpr 
                            yield dive () withRange (fun () ->
                                if posGeq pos withRange.End then 
                                    // special case: caret is after WITH
                                    // { x with $ }
                                    visitor.VisitRecordField (path, Some expr, None) 
                                else
                                    None
                            )
                        | _ -> ()
                        let copyOpt = Option.map fst copyOpt
                        for (field, _), e, sepOpt in fields do
                            yield dive (path, copyOpt, Some field) field.Range (fun r -> 
                                if rangeContainsPos field.Range pos then
                                    visitor.VisitRecordField r
                                else 
                                    None
                                )
                            let offsideColumn = 
                                match inheritOpt with
                                | Some(_,_, _, _, inheritRange) -> inheritRange.StartColumn
                                | None -> field.Range.StartColumn

                            match e with
                            | Some e -> yield dive e e.Range (fun expr ->
                                // special case: caret is below field binding
                                // field x = 5
                                // $
                                if not (rangeContainsPos e.Range pos) && sepOpt.IsNone && pos.Column = offsideColumn then
                                    visitor.VisitRecordField(path, copyOpt, None)
                                else
                                    traverseSynExpr expr
                                )
                            | None -> ()

                            match sepOpt with
                            | Some (sep, scPosOpt) -> 
                                yield dive () sep (fun () -> 
                                    // special case: caret is between field bindings
                                    // field1 = 5
                                    // $
                                    // field2 = 5
                                    diveIntoSeparator offsideColumn scPosOpt copyOpt
                                    )
                            | _ -> ()

                    ] |> pick expr
                | SynExpr.New(_, _synType, synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.ObjExpr(ty,baseCallOpt,binds,ifaces,_range1,_range2) -> 
                    let result = 
                        ifaces 
                        |> Seq.map (fun (InterfaceImpl(ty, _, _)) -> ty)
                        |> Seq.tryPick visitor.VisitInterfaceSynMemberDefnType
                    
                    if result.IsSome then 
                        result
                    else
                    [
                        match baseCallOpt with
                        | Some(expr,_) -> 
                            // this is like a call to 'new', so mock up a 'new' so we can recurse and use that existing logic
                            let newCall = SynExpr.New(false, ty, expr, unionRanges ty.Range expr.Range)
                            yield dive newCall newCall.Range traverseSynExpr
                        | _ -> ()
                        for b in binds do
                            yield dive b b.RangeOfBindingAndRhs (traverseSynBinding path)
                        for InterfaceImpl(_ty, binds, _range) in ifaces do
                            for b in binds do
                                yield dive b b.RangeOfBindingAndRhs (traverseSynBinding path)
                    ] |> pick expr
                | SynExpr.While(_sequencePointInfoForWhileLoop, synExpr, synExpr2, _range) -> 
                    [dive synExpr synExpr.Range traverseSynExpr
                     dive synExpr2 synExpr2.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.For(_sequencePointInfoForForLoop, _ident, synExpr, _, synExpr2, synExpr3, _range) -> 
                    [dive synExpr synExpr.Range traverseSynExpr
                     dive synExpr2 synExpr2.Range traverseSynExpr
                     dive synExpr3 synExpr3.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.ForEach(_sequencePointInfoForForLoop, _seqExprOnly, _isFromSource, synPat, synExpr, synExpr2, _range) ->
                    [dive synPat synPat.Range traversePat
                     dive synExpr synExpr.Range traverseSynExpr
                     dive synExpr2 synExpr2.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.ArrayOrListOfSeqExpr(_, synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.CompExpr(_, _, synExpr, _range) -> 
                    // now parser treats this syntactic expression as computation expression
                    // { identifier }
                    // here we detect this situation and treat CompExpr  { Identifier } as attempt to create record
                    // note: sequence expressions use SynExpr.CompExpr too - they need to be filtered out
                    let isPartOfArrayOrList = 
                        match origPath with
                        | TraverseStep.Expr(SynExpr.ArrayOrListOfSeqExpr(_, _, _)) :: _ -> true
                        | _ -> false
                    let ok = 
                        match isPartOfArrayOrList, synExpr with
                        | false, SynExpr.Ident ident -> visitor.VisitRecordField(path, None, Some (LongIdentWithDots([ident], [])))
                        | false, SynExpr.LongIdent(false, lidwd, _, _) -> visitor.VisitRecordField(path, None, Some lidwd)
                        | _ -> None
                    if ok.IsSome then ok
                    else
                    traverseSynExpr synExpr
                | SynExpr.Lambda(_, _, synSimplePats, synExpr, _range) ->
                    match synSimplePats with
                    | SynSimplePats.SimplePats(pats,_) ->
                        match visitor.VisitSimplePats(pats) with
                        | Some x -> Some x
                        | None -> traverseSynExpr synExpr
                    | _ -> traverseSynExpr synExpr
                | SynExpr.MatchLambda(_isExnMatch,_argm,synMatchClauseList,_spBind,_wholem) -> 
                    synMatchClauseList 
                    |> List.map (fun x -> dive x x.Range (traverseSynMatchClause path))
                    |> pick expr
                | SynExpr.Match(_sequencePointInfoForBinding, synExpr, synMatchClauseList, _, _range) -> 
                    [yield dive synExpr synExpr.Range traverseSynExpr
                     yield! synMatchClauseList |> List.map (fun x -> dive x x.RangeOfGuardAndRhs (traverseSynMatchClause path))]
                    |> pick expr
                | SynExpr.Do(synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.Assert(synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.Fixed(synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.App(_exprAtomicFlag, isInfix, synExpr, synExpr2, _range) ->
                    if isInfix then
                        [dive synExpr2 synExpr2.Range traverseSynExpr
                         dive synExpr synExpr.Range traverseSynExpr]   // reverse the args
                        |> pick expr
                    else
                        [dive synExpr synExpr.Range traverseSynExpr
                         dive synExpr2 synExpr2.Range traverseSynExpr]
                        |> pick expr
                | SynExpr.TypeApp(synExpr, _, _synTypeList, _commas, _, _, _range) -> traverseSynExpr synExpr
                | SynExpr.LetOrUse(_, _, synBindingList, synExpr, range) -> 
                    match visitor.VisitLetOrUse(synBindingList, range) with
                    | Some x -> Some x
                    | None ->
                        [yield! synBindingList |> List.map (fun x -> dive x x.RangeOfBindingAndRhs (traverseSynBinding path))
                         yield dive synExpr synExpr.Range traverseSynExpr]
                        |> pick expr
                | SynExpr.TryWith(synExpr, _range, synMatchClauseList, _range2, _range3, _sequencePointInfoForTry, _sequencePointInfoForWith) -> 
                    [yield dive synExpr synExpr.Range traverseSynExpr
                     yield! synMatchClauseList |> List.map (fun x -> dive x x.Range (traverseSynMatchClause path))]
                    |> pick expr
                | SynExpr.TryFinally(synExpr, synExpr2, _range, _sequencePointInfoForTry, _sequencePointInfoForFinally) -> 
                    [dive synExpr synExpr.Range traverseSynExpr
                     dive synExpr2 synExpr2.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.Lazy(synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.Sequential(_sequencePointInfoForSeq, _, synExpr, synExpr2, _range) -> 
                    [dive synExpr synExpr.Range traverseSynExpr
                     dive synExpr2 synExpr2.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.IfThenElse(synExpr, synExpr2, synExprOpt, _sequencePointInfoForBinding, _isRecovery, _range, _range2) -> 
                    [yield dive synExpr synExpr.Range traverseSynExpr
                     yield dive synExpr2 synExpr2.Range traverseSynExpr
                     match synExprOpt with 
                     | None -> ()
                     | Some(x) -> yield dive x x.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.Ident(_ident) -> None
                | SynExpr.LongIdent(_, _longIdent, _altNameRefCell, _range) -> None
                | SynExpr.LongIdentSet(_longIdent, synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.DotGet(synExpr, _dotm, _longIdent, _range) -> traverseSynExpr synExpr
                | SynExpr.DotSet(synExpr, _longIdent, synExpr2, _range) ->
                    [dive synExpr synExpr.Range traverseSynExpr
                     dive synExpr2 synExpr2.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.DotIndexedGet(synExpr, synExprList, _range, _range2) -> 
                    [yield dive synExpr synExpr.Range traverseSynExpr
                     for synExpr in synExprList do 
                         for x in synExpr.Exprs do 
                             yield dive x x.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.DotIndexedSet(synExpr, synExprList, synExpr2, _, _range, _range2) -> 
                    [yield dive synExpr synExpr.Range traverseSynExpr
                     for synExpr in synExprList do 
                         for x in synExpr.Exprs do 
                             yield dive x x.Range traverseSynExpr
                     yield dive synExpr2 synExpr2.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.JoinIn(synExpr1, _range, synExpr2, _range2) -> 
                    [dive synExpr1 synExpr1.Range traverseSynExpr
                     dive synExpr2 synExpr2.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.NamedIndexedPropertySet(_longIdent, synExpr, synExpr2, _range) ->
                    [dive synExpr synExpr.Range traverseSynExpr
                     dive synExpr2 synExpr2.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.DotNamedIndexedPropertySet(synExpr, _longIdent, synExpr2, synExpr3, _range) ->  
                    [dive synExpr synExpr.Range traverseSynExpr
                     dive synExpr2 synExpr2.Range traverseSynExpr
                     dive synExpr3 synExpr3.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.TypeTest(synExpr, _synType, _range) -> traverseSynExpr synExpr
                | SynExpr.Upcast(synExpr, _synType, _range) -> traverseSynExpr synExpr
                | SynExpr.Downcast(synExpr, _synType, _range) -> traverseSynExpr synExpr
                | SynExpr.InferredUpcast(synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.InferredDowncast(synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.Null(_range) -> None
                | SynExpr.AddressOf(_, synExpr, _range, _range2) -> traverseSynExpr synExpr
                | SynExpr.TraitCall(_synTyparList, _synMemberSig, synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.ImplicitZero(_range) -> None
                | SynExpr.YieldOrReturn(_, synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.YieldOrReturnFrom(_, synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.LetOrUseBang(_sequencePointInfoForBinding, _, _, synPat, synExpr, synExpr2, _range) -> 
                    [dive synPat synPat.Range traversePat
                     dive synExpr synExpr.Range traverseSynExpr
                     dive synExpr2 synExpr2.Range traverseSynExpr]
                    |> pick expr
                | SynExpr.MatchBang(_sequencePointInfoForBinding, synExpr, synMatchClauseList, _, _range) -> 
                    [yield dive synExpr synExpr.Range traverseSynExpr
                     yield! synMatchClauseList |> List.map (fun x -> dive x x.RangeOfGuardAndRhs (traverseSynMatchClause path))]
                    |> pick expr
                | SynExpr.DoBang(synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.LibraryOnlyILAssembly _ -> None
                | SynExpr.LibraryOnlyStaticOptimization _ -> None
                | SynExpr.LibraryOnlyUnionCaseFieldGet _ -> None
                | SynExpr.LibraryOnlyUnionCaseFieldSet _ -> None
                | SynExpr.ArbitraryAfterError(_debugStr, _range) -> None
                | SynExpr.FromParseError(synExpr, _range) -> traverseSynExpr synExpr
                | SynExpr.DiscardAfterMissingQualificationAfterDot(synExpr, _range) -> traverseSynExpr synExpr

            visitor.VisitExpr(path, traverseSynExpr path, defaultTraverse, expr)

        and traversePat (pat: SynPat) =
            let defaultTraverse p =
                match p with
                | SynPat.Paren (p, _) -> traversePat p
                | SynPat.Or (p1, p2, _) -> [ p1; p2] |> List.tryPick traversePat
                | SynPat.Ands (ps, _)
                | SynPat.Tuple (ps, _)
                | SynPat.StructTuple (ps, _)
                | SynPat.ArrayOrList (_, ps, _) -> ps |> List.tryPick traversePat
                | SynPat.Attrib (p, _, _) -> traversePat p
                | SynPat.LongIdent(_, _, _, args, _, _) ->
                    match args with
                    | SynConstructorArgs.Pats ps -> ps |> List.tryPick traversePat
                    | SynConstructorArgs.NamePatPairs (ps, _) ->
                        ps |> List.map snd |> List.tryPick traversePat
                | SynPat.Typed (p, ty, _) ->
                    [ traversePat p; traverseSynType ty ] |> List.tryPick id
                | _ -> None
                
            visitor.VisitPat (defaultTraverse, pat)

        and traverseSynType (ty: SynType) =
            let defaultTraverse ty =
                match ty with
                | SynType.App (typeName, _, typeArgs, _, _, _, _)
                | SynType.LongIdentApp (typeName, _, _, typeArgs, _, _, _) ->
                    [ yield typeName
                      yield! typeArgs ]
                    |> List.tryPick traverseSynType
                | SynType.Fun (ty1, ty2, _) -> [ty1; ty2] |> List.tryPick traverseSynType
                | SynType.MeasurePower (ty, _, _) 
                | SynType.HashConstraint (ty, _)
                | SynType.WithGlobalConstraints (ty, _, _)
                | SynType.Array (_, ty, _) -> traverseSynType ty
                | SynType.StaticConstantNamed (ty1, ty2, _)
                | SynType.MeasureDivide (ty1, ty2, _) -> [ty1; ty2] |> List.tryPick traverseSynType
                | SynType.Tuple (tys, _)
                | SynType.StructTuple (tys, _) -> tys |> List.map snd |> List.tryPick traverseSynType
                | SynType.StaticConstantExpr (expr, _) -> traverseSynExpr [] expr
                | SynType.Anon _ -> None
                | _ -> None

            visitor.VisitType (defaultTraverse, ty)

        and normalizeMembersToDealWithPeculiaritiesOfGettersAndSetters path traverseInherit (synMemberDefns:SynMemberDefns) =
            synMemberDefns 
                    // property getters are setters are two members that can have the same range, so do some somersaults to deal with this
                    |> Seq.groupBy (fun x -> x.Range)
                    |> Seq.choose (fun (r, mems) ->
                        match mems |> Seq.toList with
                        | [mem] -> // the typical case, a single member has this range 'r'
                            Some (dive mem r (traverseSynMemberDefn path  traverseInherit))
                        |  [SynMemberDefn.Member(Binding(_,_,_,_,_,_,_,SynPat.LongIdent(lid1,Some(info1),_,_,_,_),_,_,_,_),_) as mem1
                            SynMemberDefn.Member(Binding(_,_,_,_,_,_,_,SynPat.LongIdent(lid2,Some(info2),_,_,_,_),_,_,_,_),_) as mem2] -> // can happen if one is a getter and one is a setter
                            // ensure same long id
                            assert( (lid1.Lid,lid2.Lid) ||> List.forall2 (fun x y -> x.idText = y.idText) )
                            // ensure one is getter, other is setter
                            assert( (info1.idText="set" && info2.idText="get") ||
                                    (info2.idText="set" && info1.idText="get") )
                            Some (
                                    r,(fun() -> 
                                    // both mem1 and mem2 have same range, would violate dive-and-pick assertions, so just try the first one, else try the second one:
                                    match traverseSynMemberDefn path (fun _ -> None) mem1  with
                                    | Some _ as x -> x
                                    | _ -> traverseSynMemberDefn path (fun _ -> None) mem2 )
                                )
                        | [] ->
#if DEBUG
                            assert(false)
                            failwith "impossible, Seq.groupBy never returns empty results"
#else
                            // swallow AST error and recover silently
                            None
#endif
                        | _ ->
#if DEBUG
                            assert(false) // more than 2 members claim to have the same range, this indicates a bug in the AST
                            failwith "bug in AST"
#else
                            // swallow AST error and recover silently
                            None
#endif
                        )

        and traverseSynTypeDefn path (SynTypeDefn.TypeDefn(synComponentInfo, synTypeDefnRepr, synMemberDefns, tRange) as tydef) =
            let path = TraverseStep.TypeDefn tydef :: path
            
            match visitor.VisitComponentInfo synComponentInfo with
            | Some x -> Some x
            | None ->
            [
                match synTypeDefnRepr with
                | SynTypeDefnRepr.Exception _ -> 
                    // This node is generated in TypeChecker.fs, not in the AST.  
                    // But note exception declarations are missing from this tree walk.
                    () 
                | SynTypeDefnRepr.ObjectModel(synTypeDefnKind, synMemberDefns, _oRange) ->
                    // traverse inherit function is used to capture type specific data required for processing Inherit part
                    let traverseInherit (synType : SynType, range : range) = 
                        visitor.VisitInheritSynMemberDefn(synComponentInfo, synTypeDefnKind, synType, synMemberDefns, range)
                    yield! synMemberDefns |> normalizeMembersToDealWithPeculiaritiesOfGettersAndSetters path traverseInherit
                | SynTypeDefnRepr.Simple(synTypeDefnSimpleRepr, _range) -> 
                    match synTypeDefnSimpleRepr with
                    | SynTypeDefnSimpleRepr.TypeAbbrev(_,synType,m) ->
                        yield dive synTypeDefnRepr synTypeDefnRepr.Range (fun _ -> visitor.VisitTypeAbbrev(synType,m))
                    | _ ->
                        () // enums/DUs/record definitions don't have any SynExprs inside them
                yield! synMemberDefns |> normalizeMembersToDealWithPeculiaritiesOfGettersAndSetters path (fun _ -> None)
            ] |> pick tRange tydef

        and traverseSynMemberDefn path traverseInherit (m:SynMemberDefn)  =
            let pick (debugObj:obj) = pick m.Range debugObj
            let path = TraverseStep.MemberDefn m :: path
            match m with
            | SynMemberDefn.Open(_longIdent, _range) -> None
            | SynMemberDefn.Member(synBinding, _range) -> traverseSynBinding path synBinding
            | SynMemberDefn.ImplicitCtor(_synAccessOption, _synAttributes, synSimplePatList, _identOption, _range) ->
                visitor.VisitSimplePats(synSimplePatList)
            | SynMemberDefn.ImplicitInherit(synType, synExpr, _identOption, range) -> 
                [
                    dive () synType.Range (fun () -> 
                        match traverseInherit (synType, range) with
                        | None -> visitor.VisitImplicitInherit(traverseSynExpr path, synType, synExpr, range)
                        | x -> x)
                    dive () synExpr.Range (fun() -> 
                        visitor.VisitImplicitInherit(traverseSynExpr path, synType, synExpr, range)
                        )
                ] |> pick m
            | SynMemberDefn.AutoProperty(_attribs, _isStatic, _id, _tyOpt, _propKind, _, _xmlDoc, _access, synExpr, _, _) -> traverseSynExpr path synExpr
            | SynMemberDefn.LetBindings(synBindingList, _, _, range) -> 
                match visitor.VisitLetOrUse(synBindingList, range) with
                | Some x -> Some x
                | None -> synBindingList |> List.map (fun x -> dive x x.RangeOfBindingAndRhs (traverseSynBinding path)) |> pick m
            | SynMemberDefn.AbstractSlot(_synValSig, _memberFlags, _range) -> None
            | SynMemberDefn.Interface(synType, synMemberDefnsOption, _range) -> 
                match visitor.VisitInterfaceSynMemberDefnType(synType) with
                | None -> 
                    match synMemberDefnsOption with 
                    | None -> None
                    | Some(x) -> [ yield! x |> normalizeMembersToDealWithPeculiaritiesOfGettersAndSetters path (fun _ -> None) ]  |> pick x
                | ok -> ok
            | SynMemberDefn.Inherit(synType, _identOption, range) -> traverseInherit (synType, range)
            | SynMemberDefn.ValField(_synField, _range) -> None
            | SynMemberDefn.NestedType(synTypeDefn, _synAccessOption, _range) -> traverseSynTypeDefn path synTypeDefn

        and traverseSynMatchClause path mc =
            let path = TraverseStep.MatchClause mc :: path
            let defaultTraverse mc =
                match mc with
                | (SynMatchClause.Clause(synPat, synExprOption, synExpr, _range, _sequencePointInfoForTarget) as all) ->
                    [dive synPat synPat.Range traversePat]
                    @
                    ([
                        match synExprOption with
                        | None -> ()
                        | Some guard -> yield guard
                        yield synExpr
                     ] 
                     |> List.map (fun x -> dive x x.Range (traverseSynExpr path))
                    )|> pick all.Range all
            visitor.VisitMatchClause(defaultTraverse,mc)

        and traverseSynBinding path b =
            let defaultTraverse b =
                let path = TraverseStep.Binding b :: path
                match b with
                | (SynBinding.Binding(_synAccessOption, _synBindingKind, _, _, _synAttributes, _preXmlDoc, _synValData, synPat, _synBindingReturnInfoOption, synExpr, _range, _sequencePointInfoForBinding)) ->
                    [ traversePat synPat
                      traverseSynExpr path synExpr ]
                    |> List.tryPick id
            visitor.VisitBinding(defaultTraverse,b)

        match parseTree with
        | ParsedInput.ImplFile(ParsedImplFileInput(_,_,_,_,_,l,_))-> 
            let fileRange =
#if DEBUG
                match l with [] -> range0 | _ -> l |> List.map (fun x -> x.Range) |> List.reduce unionRanges
#else
                range0  // only used for asserting, does not matter in non-debug
#endif
            l |> List.map (fun x -> dive x x.Range (traverseSynModuleOrNamespace [])) |> pick fileRange l
        | ParsedInput.SigFile _sigFile -> None
