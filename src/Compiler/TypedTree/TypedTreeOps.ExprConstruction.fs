// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace FSharp.Compiler.TypedTreeOps

open System
open System.CodeDom.Compiler
open System.Collections.Generic
open System.Collections.Immutable
open Internal.Utilities
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Internal.Utilities.Rational
open FSharp.Compiler.IO
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Text.Layout
open FSharp.Compiler.Text.LayoutRender
open FSharp.Compiler.Text.TaggedText
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
#if !NO_TYPEPROVIDERS
open FSharp.Compiler.TypeProviders
#endif

[<AutoOpen>]
module internal ExprConstruction =

    //---------------------------------------------------------------------------
    // Standard orderings, e.g. for order set/map keys
    //---------------------------------------------------------------------------

    let valOrder =
        { new IComparer<Val> with
            member _.Compare(v1, v2) = compareBy v1 v2 _.Stamp
        }

    let tyconOrder =
        { new IComparer<Tycon> with
            member _.Compare(tycon1, tycon2) = compareBy tycon1 tycon2 _.Stamp
        }

    let recdFieldRefOrder =
        { new IComparer<RecdFieldRef> with
            member _.Compare(RecdFieldRef(tcref1, nm1), RecdFieldRef(tcref2, nm2)) =
                let c = tyconOrder.Compare(tcref1.Deref, tcref2.Deref)
                if c <> 0 then c else compare nm1 nm2
        }

    let unionCaseRefOrder =
        { new IComparer<UnionCaseRef> with
            member _.Compare(UnionCaseRef(tcref1, nm1), UnionCaseRef(tcref2, nm2)) =
                let c = tyconOrder.Compare(tcref1.Deref, tcref2.Deref)
                if c <> 0 then c else compare nm1 nm2
        }

    let mkLambdaTy g tps tys bodyTy =
        mkForallTyIfNeeded tps (mkIteratedFunTy g tys bodyTy)

    let mkLambdaArgTy m tys =
        match tys with
        | [] -> error (InternalError("mkLambdaArgTy", m))
        | [ h ] -> h
        | _ -> mkRawRefTupleTy tys

    let typeOfLambdaArg m vs = mkLambdaArgTy m (typesOfVals vs)

    let mkMultiLambdaTy g m vs bodyTy = mkFunTy g (typeOfLambdaArg m vs) bodyTy

    /// When compiling FSharp.Core.dll we have to deal with the non-local references into
    /// the library arising from env.fs. Part of this means that we have to be able to resolve these
    /// references. This function artificially forces the existence of a module or namespace at a
    /// particular point in order to do this.
    let ensureCcuHasModuleOrNamespaceAtPath (ccu: CcuThunk) path (CompPath(_, sa, cpath)) xml =
        let scoref = ccu.ILScopeRef

        let rec loop prior_cpath (path: Ident list) cpath (modul: ModuleOrNamespace) =
            let mtype = modul.ModuleOrNamespaceType

            match path, cpath with
            | hpath :: tpath, (_, mkind) :: tcpath ->
                let modName = hpath.idText

                if not (Map.containsKey modName mtype.AllEntitiesByCompiledAndLogicalMangledNames) then
                    let mty = Construct.NewEmptyModuleOrNamespaceType mkind
                    let cpath = CompPath(scoref, sa, prior_cpath)

                    let smodul =
                        Construct.NewModuleOrNamespace (Some cpath) taccessPublic hpath xml [] (MaybeLazy.Strict mty)

                    mtype.AddModuleOrNamespaceByMutation smodul

                let modul = Map.find modName mtype.AllEntitiesByCompiledAndLogicalMangledNames
                loop (prior_cpath @ [ (modName, Namespace true) ]) tpath tcpath modul

            | _ -> ()

        loop [] path cpath ccu.Contents

    //---------------------------------------------------------------------------
    // Primitive destructors
    //---------------------------------------------------------------------------

    /// Look through the Expr.Link nodes arising from type inference
    let rec stripExpr e =
        match e with
        | Expr.Link eref -> stripExpr eref.Value
        | _ -> e

    let rec stripDebugPoints expr =
        match stripExpr expr with
        | Expr.DebugPoint(_, innerExpr) -> stripDebugPoints innerExpr
        | expr -> expr

    // Strip debug points and remember how to recreate them
    let (|DebugPoints|) expr =
        let rec loop expr debug =
            match stripExpr expr with
            | Expr.DebugPoint(dp, innerExpr) -> loop innerExpr (debug << fun e -> Expr.DebugPoint(dp, e))
            | expr -> expr, debug

        loop expr id

    let mkCase (a, b) = TCase(a, b)

    let isRefTupleExpr e =
        match e with
        | Expr.Op(TOp.Tuple tupInfo, _, _, _) -> not (evalTupInfoIsStruct tupInfo)
        | _ -> false

    let tryDestRefTupleExpr e =
        match e with
        | Expr.Op(TOp.Tuple tupInfo, _, es, _) when not (evalTupInfoIsStruct tupInfo) -> es
        | _ -> [ e ]

    //---------------------------------------------------------------------------
    // Build nodes in decision graphs
    //---------------------------------------------------------------------------

    let primMkMatch (spBind, mExpr, tree, targets, mMatch, ty) =
        Expr.Match(spBind, mExpr, tree, targets, mMatch, ty)

    type MatchBuilder(spBind, inpRange: range) =

        let targets = ResizeArray<_>(10)

        member x.AddTarget tg =
            let n = targets.Count
            targets.Add tg
            n

        member x.AddResultTarget(e) =
            TDSuccess([], x.AddTarget(TTarget([], e, None)))

        member _.CloseTargets() = targets |> ResizeArray.toList

        member _.Close(dtree, m, ty) =
            primMkMatch (spBind, inpRange, dtree, targets.ToArray(), m, ty)

    let mkBoolSwitch m g t e =
        TDSwitch(g, [ TCase(DecisionTreeTest.Const(Const.Bool true), t) ], Some e, m)

    let primMkCond spBind m ty e1 e2 e3 =
        let mbuilder = MatchBuilder(spBind, m)

        let dtree =
            mkBoolSwitch m e1 (mbuilder.AddResultTarget(e2)) (mbuilder.AddResultTarget(e3))

        mbuilder.Close(dtree, m, ty)

    let mkCond spBind m ty e1 e2 e3 = primMkCond spBind m ty e1 e2 e3

    //---------------------------------------------------------------------------
    // Primitive constructors
    //---------------------------------------------------------------------------

    let exprForValRef m vref = Expr.Val(vref, NormalValUse, m)
    let exprForVal m v = exprForValRef m (mkLocalValRef v)

    let mkLocalAux m s ty mut compgen =
        let thisv =
            Construct.NewVal(
                s,
                m,
                None,
                ty,
                mut,
                compgen,
                None,
                taccessPublic,
                ValNotInRecScope,
                None,
                NormalVal,
                [],
                ValInline.Optional,
                XmlDoc.Empty,
                false,
                false,
                false,
                false,
                false,
                false,
                None,
                ParentNone
            )

        thisv, exprForVal m thisv

    let mkLocal m s ty = mkLocalAux m s ty Immutable false
    let mkCompGenLocal m s ty = mkLocalAux m s ty Immutable true
    let mkMutableCompGenLocal m s ty = mkLocalAux m s ty Mutable true

    // Type gives return type. For type-lambdas this is the formal return type.
    let mkMultiLambda m vs (body, bodyTy) =
        Expr.Lambda(newUnique (), None, None, vs, body, m, bodyTy)

    let rebuildLambda m ctorThisValOpt baseValOpt vs (body, bodyTy) =
        Expr.Lambda(newUnique (), ctorThisValOpt, baseValOpt, vs, body, m, bodyTy)

    let mkLambda m v (body, bodyTy) = mkMultiLambda m [ v ] (body, bodyTy)

    let mkTypeLambda m vs (body, bodyTy) =
        match vs with
        | [] -> body
        | _ -> Expr.TyLambda(newUnique (), vs, body, m, bodyTy)

    let mkTypeChoose m vs body =
        match vs with
        | [] -> body
        | _ -> Expr.TyChoose(vs, body, m)

    let mkObjExpr (ty, basev, basecall, overrides, iimpls, m) =
        Expr.Obj(newUnique (), ty, basev, basecall, overrides, iimpls, m)

    let mkLambdas g m tps (vs: Val list) (body, bodyTy) =
        mkTypeLambda m tps (List.foldBack (fun v (e, ty) -> mkLambda m v (e, ty), mkFunTy g v.Type ty) vs (body, bodyTy))

    let mkMultiLambdasCore g m vsl (body, bodyTy) =
        List.foldBack (fun v (e, ty) -> mkMultiLambda m v (e, ty), mkFunTy g (typeOfLambdaArg m v) ty) vsl (body, bodyTy)

    let mkMultiLambdas g m tps vsl (body, bodyTy) =
        mkTypeLambda m tps (mkMultiLambdasCore g m vsl (body, bodyTy))

    let mkMemberLambdas g m tps ctorThisValOpt baseValOpt vsl (body, bodyTy) =
        let expr =
            match ctorThisValOpt, baseValOpt with
            | None, None -> mkMultiLambdasCore g m vsl (body, bodyTy)
            | _ ->
                match vsl with
                | [] -> error (InternalError("mk_basev_multi_lambdas_core: can't attach a basev to a non-lambda expression", m))
                | h :: t ->
                    let body, bodyTy = mkMultiLambdasCore g m t (body, bodyTy)
                    (rebuildLambda m ctorThisValOpt baseValOpt h (body, bodyTy), (mkFunTy g (typeOfLambdaArg m h) bodyTy))

        mkTypeLambda m tps expr

    let mkMultiLambdaBind g v letSeqPtOpt m tps vsl (body, bodyTy) =
        TBind(v, mkMultiLambdas g m tps vsl (body, bodyTy), letSeqPtOpt)

    let mkBind seqPtOpt v e = TBind(v, e, seqPtOpt)

    let mkLetBind m bind body =
        Expr.Let(bind, body, m, Construct.NewFreeVarsCache())

    let mkLetsBind m binds body = List.foldBack (mkLetBind m) binds body

    let mkLetsFromBindings m binds body = List.foldBack (mkLetBind m) binds body

    let mkLet seqPtOpt m v x body = mkLetBind m (mkBind seqPtOpt v x) body

    /// Make sticky bindings that are compiler generated (though the variables may not be - e.g. they may be lambda arguments in a beta reduction)
    let mkCompGenBind v e =
        TBind(v, e, DebugPointAtBinding.NoneAtSticky)

    let mkCompGenBinds (vs: Val list) (es: Expr list) = List.map2 mkCompGenBind vs es

    let mkCompGenLet m v x body = mkLetBind m (mkCompGenBind v x) body

    let mkInvisibleBind v e =
        TBind(v, e, DebugPointAtBinding.NoneAtInvisible)

    let mkInvisibleBinds (vs: Val list) (es: Expr list) = List.map2 mkInvisibleBind vs es

    let mkInvisibleLet m v x body = mkLetBind m (mkInvisibleBind v x) body

    let mkInvisibleLets m vs xs body =
        mkLetsBind m (mkInvisibleBinds vs xs) body

    let mkInvisibleLetsFromBindings m vs xs body =
        mkLetsFromBindings m (mkInvisibleBinds vs xs) body

    let mkLetRecBinds m binds body =
        if isNil binds then
            body
        else
            Expr.LetRec(binds, body, m, Construct.NewFreeVarsCache())

    //-------------------------------------------------------------------------
    // Type schemes...
    //-------------------------------------------------------------------------

    // Type parameters may be have been equated to other tps in equi-recursive type inference
    // and unit type inference. Normalize them here
    let NormalizeDeclaredTyparsForEquiRecursiveInference g tps =
        match tps with
        | [] -> []
        | tps ->
            tps
            |> List.map (fun tp ->
                let ty = mkTyparTy tp

                match tryAnyParTy g ty with
                | ValueSome anyParTy -> anyParTy
                | ValueNone -> tp)

    type GeneralizedType = GeneralizedType of Typars * TType

    let mkGenericBindRhs g m generalizedTyparsForRecursiveBlock typeScheme bodyExpr =
        let (GeneralizedType(generalizedTypars, tauTy)) = typeScheme

        // Normalize the generalized typars
        let generalizedTypars =
            NormalizeDeclaredTyparsForEquiRecursiveInference g generalizedTypars

        // Some recursive bindings result in free type variables, e.g.
        //    let rec f (x:'a) = ()
        //    and g() = f y |> ignore
        // What is the type of y? Type inference equates it to 'a.
        // But "g" is not polymorphic in 'a. Hence we get a free choice of "'a"
        // in the scope of "g". Thus at each individual recursive binding we record all
        // type variables for which we have a free choice, which is precisely the difference
        // between the union of all sets of generalized type variables and the set generalized
        // at each particular binding.
        //
        // We record an expression node that indicates that a free choice can be made
        // for these. This expression node effectively binds the type variables.
        let freeChoiceTypars =
            ListSet.subtract typarEq generalizedTyparsForRecursiveBlock generalizedTypars

        mkTypeLambda m generalizedTypars (mkTypeChoose m freeChoiceTypars bodyExpr, tauTy)

    let isBeingGeneralized tp typeScheme =
        let (GeneralizedType(generalizedTypars, _)) = typeScheme
        ListSet.contains typarRefEq tp generalizedTypars

    //-------------------------------------------------------------------------
    // Build conditional expressions...
    //-------------------------------------------------------------------------

    let mkBool (g: TcGlobals) m b = Expr.Const(Const.Bool b, m, g.bool_ty)

    let mkTrue g m = mkBool g m true

    let mkFalse g m = mkBool g m false

    let mkLazyOr (g: TcGlobals) m e1 e2 =
        mkCond DebugPointAtBinding.NoneAtSticky m g.bool_ty e1 (mkTrue g m) e2

    let mkLazyAnd (g: TcGlobals) m e1 e2 =
        mkCond DebugPointAtBinding.NoneAtSticky m g.bool_ty e1 e2 (mkFalse g m)

    let mkCoerceExpr (e, toTy, m, fromTy) =
        Expr.Op(TOp.Coerce, [ toTy; fromTy ], [ e ], m)

    let mkAsmExpr (code, tinst, args, rettys, m) =
        Expr.Op(TOp.ILAsm(code, rettys), tinst, args, m)

    let mkUnionCaseExpr (uc, tinst, args, m) =
        Expr.Op(TOp.UnionCase uc, tinst, args, m)

    let mkExnExpr (uc, args, m) = Expr.Op(TOp.ExnConstr uc, [], args, m)

    let mkTupleFieldGetViaExprAddr (tupInfo, e, tinst, i, m) =
        Expr.Op(TOp.TupleFieldGet(tupInfo, i), tinst, [ e ], m)

    let mkAnonRecdFieldGetViaExprAddr (anonInfo, e, tinst, i, m) =
        Expr.Op(TOp.AnonRecdGet(anonInfo, i), tinst, [ e ], m)

    let mkRecdFieldGetViaExprAddr (e, fref, tinst, m) =
        Expr.Op(TOp.ValFieldGet fref, tinst, [ e ], m)

    let mkRecdFieldGetAddrViaExprAddr (readonly, e, fref, tinst, m) =
        Expr.Op(TOp.ValFieldGetAddr(fref, readonly), tinst, [ e ], m)

    let mkStaticRecdFieldGetAddr (readonly, fref, tinst, m) =
        Expr.Op(TOp.ValFieldGetAddr(fref, readonly), tinst, [], m)

    let mkStaticRecdFieldGet (fref, tinst, m) =
        Expr.Op(TOp.ValFieldGet fref, tinst, [], m)

    let mkStaticRecdFieldSet (fref, tinst, e, m) =
        Expr.Op(TOp.ValFieldSet fref, tinst, [ e ], m)

    let mkArrayElemAddress g (readonly, ilInstrReadOnlyAnnotation, isNativePtr, shape, elemTy, exprs, m) =
        Expr.Op(
            TOp.ILAsm(
                [ I_ldelema(ilInstrReadOnlyAnnotation, isNativePtr, shape, mkILTyvarTy 0us) ],
                [ mkByrefTyWithFlag g readonly elemTy ]
            ),
            [ elemTy ],
            exprs,
            m
        )

    let mkRecdFieldSetViaExprAddr (e1, fref, tinst, e2, m) =
        Expr.Op(TOp.ValFieldSet fref, tinst, [ e1; e2 ], m)

    let mkUnionCaseTagGetViaExprAddr (e1, cref, tinst, m) =
        Expr.Op(TOp.UnionCaseTagGet cref, tinst, [ e1 ], m)

    /// Make a 'TOp.UnionCaseProof' expression, which proves a union value is over a particular case (used only for ref-unions, not struct-unions)
    let mkUnionCaseProof (e1, cref: UnionCaseRef, tinst, m) =
        if cref.Tycon.IsStructOrEnumTycon then
            e1
        else
            Expr.Op(TOp.UnionCaseProof cref, tinst, [ e1 ], m)

    /// Build a 'TOp.UnionCaseFieldGet' expression for something we've already determined to be a particular union case. For ref-unions,
    /// the input expression has 'TType_ucase', which is an F# compiler internal "type" corresponding to the union case. For struct-unions,
    /// the input should be the address of the expression.
    let mkUnionCaseFieldGetProvenViaExprAddr (e1, cref, tinst, j, m) =
        Expr.Op(TOp.UnionCaseFieldGet(cref, j), tinst, [ e1 ], m)

    /// Build a 'TOp.UnionCaseFieldGetAddr' expression for a field of a union when we've already determined the value to be a particular union case. For ref-unions,
    /// the input expression has 'TType_ucase', which is an F# compiler internal "type" corresponding to the union case. For struct-unions,
    /// the input should be the address of the expression.
    let mkUnionCaseFieldGetAddrProvenViaExprAddr (readonly, e1, cref, tinst, j, m) =
        Expr.Op(TOp.UnionCaseFieldGetAddr(cref, j, readonly), tinst, [ e1 ], m)

    /// Build a 'get' expression for something we've already determined to be a particular union case, but where
    /// the static type of the input is not yet proven to be that particular union case. This requires a type
    /// cast to 'prove' the condition.
    let mkUnionCaseFieldGetUnprovenViaExprAddr (e1, cref, tinst, j, m) =
        mkUnionCaseFieldGetProvenViaExprAddr (mkUnionCaseProof (e1, cref, tinst, m), cref, tinst, j, m)

    let mkUnionCaseFieldSet (e1, cref, tinst, j, e2, m) =
        Expr.Op(TOp.UnionCaseFieldSet(cref, j), tinst, [ e1; e2 ], m)

    let mkExnCaseFieldGet (e1, ecref, j, m) =
        Expr.Op(TOp.ExnFieldGet(ecref, j), [], [ e1 ], m)

    let mkExnCaseFieldSet (e1, ecref, j, e2, m) =
        Expr.Op(TOp.ExnFieldSet(ecref, j), [], [ e1; e2 ], m)

    let mkDummyLambda (g: TcGlobals) (bodyExpr: Expr, bodyExprTy) =
        let m = bodyExpr.Range
        mkLambda m (fst (mkCompGenLocal m "unitVar" g.unit_ty)) (bodyExpr, bodyExprTy)

    let mkWhile (g: TcGlobals) (spWhile, marker, guardExpr, bodyExpr, m) =
        Expr.Op(
            TOp.While(spWhile, marker),
            [],
            [
                mkDummyLambda g (guardExpr, g.bool_ty)
                mkDummyLambda g (bodyExpr, g.unit_ty)
            ],
            m
        )

    let mkIntegerForLoop (g: TcGlobals) (spFor, spIn, v, startExpr, dir, finishExpr, bodyExpr: Expr, m) =
        Expr.Op(
            TOp.IntegerForLoop(spFor, spIn, dir),
            [],
            [
                mkDummyLambda g (startExpr, g.int_ty)
                mkDummyLambda g (finishExpr, g.int_ty)
                mkLambda bodyExpr.Range v (bodyExpr, g.unit_ty)
            ],
            m
        )

    let mkTryWith g (bodyExpr, filterVal, filterExpr: Expr, handlerVal, handlerExpr: Expr, m, ty, spTry, spWith) =
        Expr.Op(
            TOp.TryWith(spTry, spWith),
            [ ty ],
            [
                mkDummyLambda g (bodyExpr, ty)
                mkLambda filterExpr.Range filterVal (filterExpr, ty)
                mkLambda handlerExpr.Range handlerVal (handlerExpr, ty)
            ],
            m
        )

    let mkTryFinally (g: TcGlobals) (bodyExpr, finallyExpr, m, ty, spTry, spFinally) =
        Expr.Op(TOp.TryFinally(spTry, spFinally), [ ty ], [ mkDummyLambda g (bodyExpr, ty); mkDummyLambda g (finallyExpr, g.unit_ty) ], m)

    let mkDefault (m, ty) = Expr.Const(Const.Zero, m, ty)

    let mkValSet m vref e =
        Expr.Op(TOp.LValueOp(LSet, vref), [], [ e ], m)

    let mkAddrSet m vref e =
        Expr.Op(TOp.LValueOp(LByrefSet, vref), [], [ e ], m)

    let mkAddrGet m vref =
        Expr.Op(TOp.LValueOp(LByrefGet, vref), [], [], m)

    let mkValAddr m readonly vref =
        Expr.Op(TOp.LValueOp(LAddrOf readonly, vref), [], [], m)

    let valOfBind (b: Binding) = b.Var

    let valsOfBinds (binds: Bindings) = binds |> List.map (fun b -> b.Var)

    let mkDebugPoint m expr =
        Expr.DebugPoint(DebugPointAtLeafExpr.Yes m, expr)

    // Used to remove Expr.Link for inner expressions in pattern matches
    let (|InnerExprPat|) expr = stripExpr expr

[<AutoOpen>]
module internal TypedTreeCollections =

    //--------------------------------------------------------------------------
    // Maps tracking extra information for values
    //--------------------------------------------------------------------------

    [<NoEquality; NoComparison>]
    type ValHash<'T> =
        | ValHash of Dictionary<Stamp, 'T>

        member ht.Values =
            let (ValHash t) = ht
            t.Values :> seq<'T>

        member ht.TryFind(v: Val) =
            let (ValHash t) = ht

            match t.TryGetValue v.Stamp with
            | true, v -> Some v
            | _ -> None

        member ht.Add(v: Val, x) =
            let (ValHash t) = ht
            t[v.Stamp] <- x

        static member Create() = ValHash(new Dictionary<_, 'T>(11))

    [<Struct; NoEquality; NoComparison>]
    type ValMultiMap<'T>(contents: StampMap<'T list>) =

        member _.ContainsKey(v: Val) = contents.ContainsKey v.Stamp

        member _.Find(v: Val) =
            match contents |> Map.tryFind v.Stamp with
            | Some vals -> vals
            | _ -> []

        member m.Add(v: Val, x) =
            ValMultiMap<'T>(contents.Add(v.Stamp, x :: m.Find v))

        member _.Remove(v: Val) =
            ValMultiMap<'T>(contents.Remove v.Stamp)

        member _.Contents = contents

        static member Empty = ValMultiMap<'T>(Map.empty)

    [<Struct; NoEquality; NoComparison>]
    type TyconRefMultiMap<'T>(contents: TyconRefMap<'T list>) =

        member _.Find v =
            match contents.TryFind v with
            | Some vals -> vals
            | _ -> []

        member m.Add(v, x) =
            TyconRefMultiMap<'T>(contents.Add v (x :: m.Find v))

        static member Empty = TyconRefMultiMap<'T>(TyconRefMap<_>.Empty)

        static member OfList vs =
            (vs, TyconRefMultiMap<'T>.Empty)
            ||> List.foldBack (fun (x, y) acc -> acc.Add(x, y))

[<AutoOpen>]
module internal TypeTesters =

    //--------------------------------------------------------------------------
    // From Ref_private to Ref_nonlocal when exporting data.
    //--------------------------------------------------------------------------

    /// Try to create a EntityRef suitable for accessing the given Entity from another assembly
    let tryRescopeEntity viewedCcu (entity: Entity) : EntityRef voption =
        match entity.PublicPath with
        | Some pubpath -> ValueSome(ERefNonLocal(rescopePubPath viewedCcu pubpath))
        | None -> ValueNone

    /// Try to create a ValRef suitable for accessing the given Val from another assembly
    let tryRescopeVal viewedCcu (entityRemap: Remap) (vspec: Val) : ValRef voption =
        match vspec.PublicPath with
        | Some(ValPubPath(p, fullLinkageKey)) ->
            // The type information in the val linkage doesn't need to keep any information to trait solutions.
            let entityRemap =
                { entityRemap with
                    removeTraitSolutions = true
                }

            let fullLinkageKey = remapValLinkage entityRemap fullLinkageKey

            let vref =
                // This compensates for the somewhat poor design decision in the F# compiler and metadata where
                // members are stored as values under the enclosing namespace/module rather than under the type.
                // This stems from the days when types and namespace/modules were separated constructs in the
                // compiler implementation.
                if vspec.IsIntrinsicMember then
                    mkNonLocalValRef (rescopePubPathToParent viewedCcu p) fullLinkageKey
                else
                    mkNonLocalValRef (rescopePubPath viewedCcu p) fullLinkageKey

            ValueSome vref
        | _ -> ValueNone

    //---------------------------------------------------------------------------
    // Type information about records, constructors etc.
    //---------------------------------------------------------------------------

    let actualTyOfRecdField inst (fspec: RecdField) = instType inst fspec.FormalType

    let actualTysOfRecdFields inst rfields =
        List.map (actualTyOfRecdField inst) rfields

    let actualTysOfInstanceRecdFields inst (tcref: TyconRef) =
        tcref.AllInstanceFieldsAsList |> actualTysOfRecdFields inst

    let actualTysOfUnionCaseFields inst (x: UnionCaseRef) =
        actualTysOfRecdFields inst x.AllFieldsAsList

    let actualResultTyOfUnionCase tinst (x: UnionCaseRef) =
        instType (mkTyconRefInst x.TyconRef tinst) x.ReturnType

    let recdFieldsOfExnDefRef x =
        (stripExnEqns x).TrueInstanceFieldsAsList

    let recdFieldOfExnDefRefByIdx x n = (stripExnEqns x).GetFieldByIndex n

    let recdFieldTysOfExnDefRef x =
        actualTysOfRecdFields [] (recdFieldsOfExnDefRef x)

    let recdFieldTyOfExnDefRefByIdx x j =
        actualTyOfRecdField [] (recdFieldOfExnDefRefByIdx x j)

    let actualTyOfRecdFieldForTycon tycon tinst (fspec: RecdField) =
        instType (mkTyconInst tycon tinst) fspec.FormalType

    let actualTyOfRecdFieldRef (fref: RecdFieldRef) tinst =
        actualTyOfRecdFieldForTycon fref.Tycon tinst fref.RecdField

    let actualTyOfUnionFieldRef (fref: UnionCaseRef) n tinst =
        actualTyOfRecdFieldForTycon fref.Tycon tinst (fref.FieldByIndex n)

    //---------------------------------------------------------------------------
    // Apply type functions to types
    //---------------------------------------------------------------------------

    let destForallTy g ty =
        let tps, tau = primDestForallTy g ty
        // tps may be have been equated to other tps in equi-recursive type inference
        // and unit type inference. Normalize them here
        let tps = NormalizeDeclaredTyparsForEquiRecursiveInference g tps
        tps, tau

    let tryDestForallTy g ty =
        if isForallTy g ty then destForallTy g ty else [], ty

    let rec stripFunTy g ty =
        if isFunTy g ty then
            let domainTy, rangeTy = destFunTy g ty
            let more, retTy = stripFunTy g rangeTy
            domainTy :: more, retTy
        else
            [], ty

    let applyForallTy g ty tyargs =
        let tps, tau = destForallTy g ty
        instType (mkTyparInst tps tyargs) tau

    let reduceIteratedFunTy g ty args =
        List.fold
            (fun ty _ ->
                if not (isFunTy g ty) then
                    failwith "reduceIteratedFunTy"

                snd (destFunTy g ty))
            ty
            args

    let applyTyArgs g ty tyargs =
        if isForallTy g ty then applyForallTy g ty tyargs else ty

    let applyTys g funcTy (tyargs, argTys) =
        let afterTyappTy = applyTyArgs g funcTy tyargs
        reduceIteratedFunTy g afterTyappTy argTys

    let formalApplyTys g funcTy (tyargs, args) =
        reduceIteratedFunTy g (if isNil tyargs then funcTy else snd (destForallTy g funcTy)) args

    let rec stripFunTyN g n ty =
        assert (n >= 0)

        if n > 0 && isFunTy g ty then
            let d, r = destFunTy g ty
            let more, retTy = stripFunTyN g (n - 1) r
            d :: more, retTy
        else
            [], ty

    let tryDestAnyTupleTy g ty =
        if isAnyTupleTy g ty then
            destAnyTupleTy g ty
        else
            tupInfoRef, [ ty ]

    let tryDestRefTupleTy g ty =
        if isRefTupleTy g ty then destRefTupleTy g ty else [ ty ]

    type UncurriedArgInfos = (TType * ArgReprInfo) list

    type CurriedArgInfos = (TType * ArgReprInfo) list list

    type TraitWitnessInfos = TraitWitnessInfo list

    // A 'tau' type is one with its type parameters stripped off
    let GetTopTauTypeInFSharpForm g (curriedArgInfos: ArgReprInfo list list) tau m =
        let nArgInfos = curriedArgInfos.Length
        let argTys, retTy = stripFunTyN g nArgInfos tau

        if nArgInfos <> argTys.Length then
            error (Error(FSComp.SR.tastInvalidMemberSignature (), m))

        let argTysl =
            (curriedArgInfos, argTys)
            ||> List.map2 (fun argInfos argTy ->
                match argInfos with
                | [] -> [ (g.unit_ty, ValReprInfo.unnamedTopArg1) ]
                | [ argInfo ] -> [ (argTy, argInfo) ]
                | _ -> List.zip (destRefTupleTy g argTy) argInfos)

        argTysl, retTy

    let destTopForallTy g (ValReprInfo(ntps, _, _)) ty =
        let tps, tau = (if isNil ntps then [], ty else tryDestForallTy g ty)
        // tps may be have been equated to other tps in equi-recursive type inference. Normalize them here
        let tps = NormalizeDeclaredTyparsForEquiRecursiveInference g tps
        tps, tau

    let GetValReprTypeInFSharpForm g (ValReprInfo(_, argInfos, retInfo) as valReprInfo) ty m =
        let tps, tau = destTopForallTy g valReprInfo ty
        let curriedArgTys, returnTy = GetTopTauTypeInFSharpForm g argInfos tau m
        tps, curriedArgTys, returnTy, retInfo

    let IsCompiledAsStaticProperty g (v: Val) =
        match v.ValReprInfo with
        | Some valReprInfoValue ->
            match GetValReprTypeInFSharpForm g valReprInfoValue v.Type v.Range with
            | [], [], _, _ when not v.IsMember -> true
            | _ -> false
        | _ -> false

    let IsCompiledAsStaticPropertyWithField g (v: Val) =
        not v.IsCompiledAsStaticPropertyWithoutField && IsCompiledAsStaticProperty g v

    //-------------------------------------------------------------------------
    // Multi-dimensional array types...
    //-------------------------------------------------------------------------

    let isArrayTyconRef (g: TcGlobals) tcref =
        g.il_arr_tcr_map |> Array.exists (tyconRefEq g tcref)

    let rankOfArrayTyconRef (g: TcGlobals) tcref =
        match g.il_arr_tcr_map |> Array.tryFindIndex (tyconRefEq g tcref) with
        | Some idx -> idx + 1
        | None -> failwith "rankOfArrayTyconRef: unsupported array rank"

    //-------------------------------------------------------------------------
    // Misc functions on F# types
    //-------------------------------------------------------------------------

    let destArrayTy (g: TcGlobals) ty =
        match tryAppTy g ty with
        | ValueSome(tcref, [ ty ]) when isArrayTyconRef g tcref -> ty
        | _ -> failwith "destArrayTy"

    let destListTy (g: TcGlobals) ty =
        match tryAppTy g ty with
        | ValueSome(tcref, [ ty ]) when tyconRefEq g tcref g.list_tcr_canon -> ty
        | _ -> failwith "destListTy"

    let tyconRefEqOpt g tcrefOpt tcref =
        match tcrefOpt with
        | None -> false
        | Some tcref2 -> tyconRefEq g tcref2 tcref

    let isStringTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tyconRefEq g tcref g.system_String_tcref
        | _ -> false)

    let isListTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tyconRefEq g tcref g.list_tcr_canon
        | _ -> false)

    let isArrayTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> isArrayTyconRef g tcref
        | _ -> false)

    let isArray1DTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tyconRefEq g tcref g.il_arr_tcr_map[0]
        | _ -> false)

    let isUnitTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tyconRefEq g g.unit_tcr_canon tcref
        | _ -> false)

    let isObjTyAnyNullness g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tyconRefEq g g.system_Object_tcref tcref
        | _ -> false)

    let isObjNullTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, n) when
            (not g.checkNullness)
            || (n.TryEvaluate() <> ValueSome(NullnessInfo.WithoutNull))
            ->
            tyconRefEq g g.system_Object_tcref tcref
        | _ -> false)

    let isObjTyWithoutNull (g: TcGlobals) ty =
        g.checkNullness
        && ty
           |> stripTyEqns g
           |> (function
           | TType_app(tcref, _, n) when (n.TryEvaluate() = ValueSome(NullnessInfo.WithoutNull)) -> tyconRefEq g g.system_Object_tcref tcref
           | _ -> false)

    let isValueTypeTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tyconRefEq g g.system_Value_tcref tcref
        | _ -> false)

    let isVoidTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tyconRefEq g g.system_Void_tcref tcref
        | _ -> false)

    let isILAppTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tcref.IsILTycon
        | _ -> false)

    let isNativePtrTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tyconRefEq g g.nativeptr_tcr tcref
        | _ -> false)

    let isByrefTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) when g.byref2_tcr.CanDeref -> tyconRefEq g g.byref2_tcr tcref
        | TType_app(tcref, _, _) -> tyconRefEq g g.byref_tcr tcref
        | _ -> false)

    let isInByrefTag g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, [], _) -> tyconRefEq g g.byrefkind_In_tcr tcref
        | _ -> false)

    let isInByrefTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, [ _; tagTy ], _) when g.byref2_tcr.CanDeref -> tyconRefEq g g.byref2_tcr tcref && isInByrefTag g tagTy
        | _ -> false)

    let isOutByrefTag g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, [], _) -> tyconRefEq g g.byrefkind_Out_tcr tcref
        | _ -> false)

    let isOutByrefTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, [ _; tagTy ], _) when g.byref2_tcr.CanDeref -> tyconRefEq g g.byref2_tcr tcref && isOutByrefTag g tagTy
        | _ -> false)

#if !NO_TYPEPROVIDERS
    let extensionInfoOfTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tcref.TypeReprInfo
        | _ -> TNoRepr)
#endif

    type TypeDefMetadata =
        | ILTypeMetadata of TILObjectReprData
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata of TProvidedTypeInfo
#endif

    let metadataOfTycon (tycon: Tycon) =
#if !NO_TYPEPROVIDERS
        match tycon.TypeReprInfo with
        | TProvidedTypeRepr info -> ProvidedTypeMetadata info
        | _ ->
#endif
        if tycon.IsILTycon then
            ILTypeMetadata tycon.ILTyconInfo
        else
            FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata

    let metadataOfTy g ty =
#if !NO_TYPEPROVIDERS
        match extensionInfoOfTy g ty with
        | TProvidedTypeRepr info -> ProvidedTypeMetadata info
        | _ ->
#endif
        if isILAppTy g ty then
            let tcref = tcrefOfAppTy g ty
            ILTypeMetadata tcref.ILTyconInfo
        else
            FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata

    let isILReferenceTy g ty =
        match metadataOfTy g ty with
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata info -> not info.IsStructOrEnum
#endif
        | ILTypeMetadata(TILObjectReprData(_, _, td)) -> not td.IsStructOrEnum
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata -> isArrayTy g ty

    let isILInterfaceTycon (tycon: Tycon) =
        match metadataOfTycon tycon with
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata info -> info.IsInterface
#endif
        | ILTypeMetadata(TILObjectReprData(_, _, td)) -> td.IsInterface
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata -> false

    let rankOfArrayTy g ty =
        rankOfArrayTyconRef g (tcrefOfAppTy g ty)

    let isFSharpObjModelRefTy g ty =
        isFSharpObjModelTy g ty
        && let tcref = tcrefOfAppTy g ty in

           match tcref.FSharpTyconRepresentationData.fsobjmodel_kind with
           | TFSharpClass
           | TFSharpInterface
           | TFSharpDelegate _ -> true
           | TFSharpUnion
           | TFSharpRecord
           | TFSharpStruct
           | TFSharpEnum -> false

    let isFSharpClassTy g ty =
        match tryTcrefOfAppTy g ty with
        | ValueSome tcref -> tcref.Deref.IsFSharpClassTycon
        | _ -> false

    let isFSharpStructTy g ty =
        match tryTcrefOfAppTy g ty with
        | ValueSome tcref -> tcref.Deref.IsFSharpStructOrEnumTycon
        | _ -> false

    let isFSharpInterfaceTy g ty =
        match tryTcrefOfAppTy g ty with
        | ValueSome tcref -> tcref.Deref.IsFSharpInterfaceTycon
        | _ -> false

    let isDelegateTy g ty =
        match metadataOfTy g ty with
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata info -> info.IsDelegate()
#endif
        | ILTypeMetadata(TILObjectReprData(_, _, td)) -> td.IsDelegate
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata ->
            match tryTcrefOfAppTy g ty with
            | ValueSome tcref -> tcref.Deref.IsFSharpDelegateTycon
            | _ -> false

    let isInterfaceTy g ty =
        match metadataOfTy g ty with
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata info -> info.IsInterface
#endif
        | ILTypeMetadata(TILObjectReprData(_, _, td)) -> td.IsInterface
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata -> isFSharpInterfaceTy g ty

    let isFSharpDelegateTy g ty =
        isDelegateTy g ty && isFSharpObjModelTy g ty

    let isClassTy g ty =
        match metadataOfTy g ty with
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata info -> info.IsClass
#endif
        | ILTypeMetadata(TILObjectReprData(_, _, td)) -> td.IsClass
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata -> isFSharpClassTy g ty

    let isStructOrEnumTyconTy g ty =
        match tryTcrefOfAppTy g ty with
        | ValueSome tcref -> tcref.Deref.IsStructOrEnumTycon
        | _ -> false

    let isStructRecordOrUnionTyconTy g ty =
        match tryTcrefOfAppTy g ty with
        | ValueSome tcref -> tcref.Deref.IsStructRecordOrUnionTycon
        | _ -> false

    let isStructTyconRef (tcref: TyconRef) =
        let tycon = tcref.Deref
        tycon.IsStructRecordOrUnionTycon || tycon.IsStructOrEnumTycon

    let isStructTy g ty =
        match tryTcrefOfAppTy g ty with
        | ValueSome tcref -> isStructTyconRef tcref
        | _ -> isStructAnonRecdTy g ty || isStructTupleTy g ty

    let isMeasureableValueType g ty =
        match stripTyEqns g ty with
        | TType_app(tcref, _, _) when tcref.IsMeasureableReprTycon ->
            let erasedTy = stripTyEqnsAndMeasureEqns g ty
            isStructTy g erasedTy
        | _ -> false

    let isRefTy g ty =
        not (isStructOrEnumTyconTy g ty)
        && (isUnionTy g ty
            || isRefTupleTy g ty
            || isRecdTy g ty
            || isILReferenceTy g ty
            || isFunTy g ty
            || isReprHiddenTy g ty
            || isFSharpObjModelRefTy g ty
            || isUnitTy g ty
            || (isAnonRecdTy g ty && not (isStructAnonRecdTy g ty)))

    let isForallFunctionTy g ty =
        let _, tau = tryDestForallTy g ty
        isFunTy g tau

    // An unmanaged-type is any type that isn't a reference-type, a type-parameter, or a generic struct-type and
    // contains no fields whose type is not an unmanaged-type. In other words, an unmanaged-type is one of the
    // following:
    // - sbyte, byte, short, ushort, int, uint, long, ulong, char, float, double, decimal, or bool.
    // - Any enum-type.
    // - Any pointer-type.
    // - Any generic user-defined struct-type that can be statically determined to be 'unmanaged' at construction.
    let rec isUnmanagedTy g ty =
        let isUnmanagedRecordField tinst rf =
            isUnmanagedTy g (actualTyOfRecdField tinst rf)

        let ty = stripTyEqnsAndMeasureEqns g ty

        match tryTcrefOfAppTy g ty with
        | ValueSome tcref ->
            let isEq tcref2 = tyconRefEq g tcref tcref2

            if
                isEq g.nativeptr_tcr
                || isEq g.nativeint_tcr
                || isEq g.sbyte_tcr
                || isEq g.byte_tcr
                || isEq g.int16_tcr
                || isEq g.uint16_tcr
                || isEq g.int32_tcr
                || isEq g.uint32_tcr
                || isEq g.int64_tcr
                || isEq g.uint64_tcr
                || isEq g.char_tcr
                || isEq g.voidptr_tcr
                || isEq g.float32_tcr
                || isEq g.float_tcr
                || isEq g.decimal_tcr
                || isEq g.bool_tcr
            then
                true
            else
                let tycon = tcref.Deref

                if tycon.IsEnumTycon then
                    true
                elif isStructUnionTy g ty then
                    let tinst = mkInstForAppTy g ty

                    tcref.UnionCasesAsRefList
                    |> List.forall (fun c -> c |> actualTysOfUnionCaseFields tinst |> List.forall (isUnmanagedTy g))
                elif tycon.IsStructOrEnumTycon then
                    let tinst = mkInstForAppTy g ty
                    tycon.AllInstanceFieldsAsList |> List.forall (isUnmanagedRecordField tinst)
                else
                    false
        | ValueNone ->
            if isStructTupleTy g ty then
                (destStructTupleTy g ty) |> List.forall (isUnmanagedTy g)
            else if isStructAnonRecdTy g ty then
                (destStructAnonRecdTy g ty) |> List.forall (isUnmanagedTy g)
            else
                false

    let isInterfaceTycon x =
        isILInterfaceTycon x || x.IsFSharpInterfaceTycon

    let isInterfaceTyconRef (tcref: TyconRef) = isInterfaceTycon tcref.Deref

    let isEnumTy g ty =
        match tryTcrefOfAppTy g ty with
        | ValueNone -> false
        | ValueSome tcref -> tcref.IsEnumTycon

    let isSignedIntegerTy g ty =
        typeEquivAux EraseMeasures g g.sbyte_ty ty
        || typeEquivAux EraseMeasures g g.int16_ty ty
        || typeEquivAux EraseMeasures g g.int32_ty ty
        || typeEquivAux EraseMeasures g g.nativeint_ty ty
        || typeEquivAux EraseMeasures g g.int64_ty ty

    let isUnsignedIntegerTy g ty =
        typeEquivAux EraseMeasures g g.byte_ty ty
        || typeEquivAux EraseMeasures g g.uint16_ty ty
        || typeEquivAux EraseMeasures g g.uint32_ty ty
        || typeEquivAux EraseMeasures g g.unativeint_ty ty
        || typeEquivAux EraseMeasures g g.uint64_ty ty

    let isIntegerTy g ty =
        isSignedIntegerTy g ty || isUnsignedIntegerTy g ty

    /// float or float32 or float<_> or float32<_>
    let isFpTy g ty =
        typeEquivAux EraseMeasures g g.float_ty ty
        || typeEquivAux EraseMeasures g g.float32_ty ty

    /// decimal or decimal<_>
    let isDecimalTy g ty =
        typeEquivAux EraseMeasures g g.decimal_ty ty

    let isNonDecimalNumericType g ty = isIntegerTy g ty || isFpTy g ty

    let isNumericType g ty =
        isNonDecimalNumericType g ty || isDecimalTy g ty

    let actualReturnTyOfSlotSig parentTyInst methTyInst (TSlotSig(_, _, parentFormalTypars, methFormalTypars, _, formalRetTy)) =
        let methTyInst = mkTyparInst methFormalTypars methTyInst
        let parentTyInst = mkTyparInst parentFormalTypars parentTyInst
        Option.map (instType (parentTyInst @ methTyInst)) formalRetTy

    let slotSigHasVoidReturnTy (TSlotSig(_, _, _, _, _, formalRetTy)) = Option.isNone formalRetTy

    let returnTyOfMethod g (TObjExprMethod(TSlotSig(_, parentTy, _, _, _, _) as ss, _, methFormalTypars, _, _, _)) =
        let tinst = argsOfAppTy g parentTy
        let methTyInst = generalizeTypars methFormalTypars
        actualReturnTyOfSlotSig tinst methTyInst ss

    /// Is the type 'abstract' in C#-speak
    let isAbstractTycon (tycon: Tycon) =
        if tycon.IsFSharpObjectModelTycon then
            not tycon.IsFSharpDelegateTycon && tycon.TypeContents.tcaug_abstract
        else
            tycon.IsILTycon && tycon.ILTyconRawMetadata.IsAbstract

    //---------------------------------------------------------------------------
    // Determine if a member/Val/ValRef is an explicit impl
    //---------------------------------------------------------------------------

    let MemberIsExplicitImpl g (membInfo: ValMemberInfo) =
        membInfo.MemberFlags.IsOverrideOrExplicitImpl
        && match membInfo.ImplementedSlotSigs with
           | [] -> false
           | slotsigs -> slotsigs |> List.forall (fun slotsig -> isInterfaceTy g slotsig.DeclaringType)

    let ValIsExplicitImpl g (v: Val) =
        match v.MemberInfo with
        | Some membInfo -> MemberIsExplicitImpl g membInfo
        | _ -> false

    let ValRefIsExplicitImpl g (vref: ValRef) = ValIsExplicitImpl g vref.Deref

    // Get measure of type, float<_> or float32<_> or decimal<_> but not float=float<1> or float32=float32<1> or decimal=decimal<1>
    let getMeasureOfType g ty =
        match ty with
        | AppTy g (tcref, [ tyarg ]) ->
            match stripTyEqns g tyarg with
            | TType_measure ms when not (measureEquiv g ms (Measure.One(tcref.Range))) -> Some(tcref, ms)
            | _ -> None
        | _ -> None

    let isErasedType g ty =
        match stripTyEqns g ty with
#if !NO_TYPEPROVIDERS
        | TType_app(tcref, _, _) -> tcref.IsProvidedErasedTycon
#endif
        | _ -> false

    // Return all components of this type expression that cannot be tested at runtime
    let rec getErasedTypes g ty checkForNullness =
        let ty = stripTyEqns g ty

        if isErasedType g ty then
            [ ty ]
        else
            match ty with
            | TType_forall(_, bodyTy) -> getErasedTypes g bodyTy checkForNullness

            | TType_var(tp, nullness) ->
                match checkForNullness, nullness.Evaluate() with
                | true, NullnessInfo.WithNull -> [ ty ] // with-null annotations can't be tested at runtime, Nullable<> is not part of Nullness feature as of now.
                | _ -> if tp.IsErased then [ ty ] else []

            | TType_app(_, b, nullness) ->
                match checkForNullness, nullness.Evaluate() with
                | true, NullnessInfo.WithNull -> [ ty ]
                | _ -> List.foldBack (fun ty tys -> getErasedTypes g ty false @ tys) b []

            | TType_ucase(_, b)
            | TType_anon(_, b)
            | TType_tuple(_, b) -> List.foldBack (fun ty tys -> getErasedTypes g ty false @ tys) b []

            | TType_fun(domainTy, rangeTy, nullness) ->
                match checkForNullness, nullness.Evaluate() with
                | true, NullnessInfo.WithNull -> [ ty ]
                | _ -> getErasedTypes g domainTy false @ getErasedTypes g rangeTy false
            | TType_measure _ -> [ ty ]

    let underlyingTypeOfEnumTy (g: TcGlobals) ty =
        assert (isEnumTy g ty)

        match metadataOfTy g ty with
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata info -> info.UnderlyingTypeOfEnum()
#endif
        | ILTypeMetadata(TILObjectReprData(_, _, tdef)) ->

            let info = computeILEnumInfo (tdef.Name, tdef.Fields)
            let ilTy = getTyOfILEnumInfo info

            match ilTy.TypeSpec.Name with
            | "System.Byte" -> g.byte_ty
            | "System.SByte" -> g.sbyte_ty
            | "System.Int16" -> g.int16_ty
            | "System.Int32" -> g.int32_ty
            | "System.Int64" -> g.int64_ty
            | "System.UInt16" -> g.uint16_ty
            | "System.UInt32" -> g.uint32_ty
            | "System.UInt64" -> g.uint64_ty
            | "System.Single" -> g.float32_ty
            | "System.Double" -> g.float_ty
            | "System.Char" -> g.char_ty
            | "System.Boolean" -> g.bool_ty
            | _ -> g.int32_ty
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata ->
            let tycon = (tcrefOfAppTy g ty).Deref

            match tycon.GetFieldByName "value__" with
            | Some rf -> rf.FormalType
            | None -> error (InternalError("no 'value__' field found for enumeration type " + tycon.LogicalName, tycon.Range))

    let normalizeEnumTy g ty =
        (if isEnumTy g ty then underlyingTypeOfEnumTy g ty else ty)

    let isResumableCodeTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tyconRefEq g tcref g.ResumableCode_tcr
        | _ -> false)

    let rec isReturnsResumableCodeTy g ty =
        if isFunTy g ty then
            isReturnsResumableCodeTy g (rangeOfFunTy g ty)
        else
            isResumableCodeTy g ty

    let ComputeUseMethodImpl g (v: Val) =
        v.ImplementedSlotSigs
        |> List.exists (fun slotsig ->
            let oty = slotsig.DeclaringType
            let otcref = tcrefOfAppTy g oty
            let tcref = v.MemberApparentEntity

            // REVIEW: it would be good to get rid of this special casing of Compare and GetHashCode
            isInterfaceTy g oty
            &&

            (let isCompare =
                tcref.GeneratedCompareToValues.IsSome
                && (typeEquiv g oty g.mk_IComparable_ty
                    || tyconRefEq g g.system_GenericIComparable_tcref otcref)

             not isCompare)
            &&

            (let isGenericEquals =
                tcref.GeneratedHashAndEqualsWithComparerValues.IsSome
                && tyconRefEq g g.system_GenericIEquatable_tcref otcref

             not isGenericEquals)
            &&

            (let isStructural =
                (tcref.GeneratedCompareToWithComparerValues.IsSome
                 && typeEquiv g oty g.mk_IStructuralComparable_ty)
                || (tcref.GeneratedHashAndEqualsWithComparerValues.IsSome
                    && typeEquiv g oty g.mk_IStructuralEquatable_ty)

             not isStructural))

    let useGenuineField (tycon: Tycon) (f: RecdField) =
        Option.isSome f.LiteralValue
        || tycon.IsEnumTycon
        || f.rfield_secret
        || (not f.IsStatic && f.rfield_mutable && not tycon.IsRecordTycon)

    let ComputeFieldName tycon f =
        if useGenuineField tycon f then
            f.rfield_id.idText
        else
            CompilerGeneratedName f.rfield_id.idText

[<AutoOpen>]
module internal CommonContainers =

    let isByrefTyconRef (g: TcGlobals) (tcref: TyconRef) =
        (g.byref_tcr.CanDeref && tyconRefEq g g.byref_tcr tcref)
        || (g.byref2_tcr.CanDeref && tyconRefEq g g.byref2_tcr tcref)
        || (g.inref_tcr.CanDeref && tyconRefEq g g.inref_tcr tcref)
        || (g.outref_tcr.CanDeref && tyconRefEq g g.outref_tcr tcref)
        || tyconRefEqOpt g g.system_TypedReference_tcref tcref
        || tyconRefEqOpt g g.system_ArgIterator_tcref tcref
        || tyconRefEqOpt g g.system_RuntimeArgumentHandle_tcref tcref

    //-------------------------------------------------------------------------
    // List and reference types...
    //-------------------------------------------------------------------------

    let destByrefTy g ty =
        match ty |> stripTyEqns g with
        | TType_app(tcref, [ x; _ ], _) when g.byref2_tcr.CanDeref && tyconRefEq g g.byref2_tcr tcref -> x // Check sufficient FSharp.Core
        | TType_app(tcref, [ x ], _) when tyconRefEq g g.byref_tcr tcref -> x // all others
        | _ -> failwith "destByrefTy: not a byref type"

    [<return: Struct>]
    let (|ByrefTy|_|) g ty =
        // Because of byref = byref2<ty,tags> it is better to write this using is/dest
        if isByrefTy g ty then
            ValueSome(destByrefTy g ty)
        else
            ValueNone

    let destNativePtrTy g ty =
        match ty |> stripTyEqns g with
        | TType_app(tcref, [ x ], _) when tyconRefEq g g.nativeptr_tcr tcref -> x
        | _ -> failwith "destNativePtrTy: not a native ptr type"

    let isRefCellTy g ty =
        match tryTcrefOfAppTy g ty with
        | ValueNone -> false
        | ValueSome tcref -> tyconRefEq g g.refcell_tcr_canon tcref

    let destRefCellTy g ty =
        match ty |> stripTyEqns g with
        | TType_app(tcref, [ x ], _) when tyconRefEq g g.refcell_tcr_canon tcref -> x
        | _ -> failwith "destRefCellTy: not a ref type"

    let StripSelfRefCell (g: TcGlobals, baseOrThisInfo: ValBaseOrThisInfo, tau: TType) : TType =
        if baseOrThisInfo = CtorThisVal && isRefCellTy g tau then
            destRefCellTy g tau
        else
            tau

    let mkRefCellTy (g: TcGlobals) ty =
        TType_app(g.refcell_tcr_nice, [ ty ], g.knownWithoutNull)

    let mkLazyTy (g: TcGlobals) ty =
        TType_app(g.lazy_tcr_nice, [ ty ], g.knownWithoutNull)

    let mkPrintfFormatTy (g: TcGlobals) aty bty cty dty ety =
        TType_app(g.format_tcr, [ aty; bty; cty; dty; ety ], g.knownWithoutNull)

    let mkOptionTy (g: TcGlobals) ty =
        TType_app(g.option_tcr_nice, [ ty ], g.knownWithoutNull)

    let mkValueOptionTy (g: TcGlobals) ty =
        TType_app(g.valueoption_tcr_nice, [ ty ], g.knownWithoutNull)

    let mkNullableTy (g: TcGlobals) ty =
        TType_app(g.system_Nullable_tcref, [ ty ], g.knownWithoutNull)

    let mkListTy (g: TcGlobals) ty =
        TType_app(g.list_tcr_nice, [ ty ], g.knownWithoutNull)

    let isBoolTy (g: TcGlobals) ty =
        match tryTcrefOfAppTy g ty with
        | ValueNone -> false
        | ValueSome tcref -> tyconRefEq g g.system_Bool_tcref tcref || tyconRefEq g g.bool_tcr tcref

    let isValueOptionTy (g: TcGlobals) ty =
        match tryTcrefOfAppTy g ty with
        | ValueNone -> false
        | ValueSome tcref -> tyconRefEq g g.valueoption_tcr_canon tcref

    let isOptionTy (g: TcGlobals) ty =
        match tryTcrefOfAppTy g ty with
        | ValueNone -> false
        | ValueSome tcref -> tyconRefEq g g.option_tcr_canon tcref

    let isChoiceTy (g: TcGlobals) ty =
        match tryTcrefOfAppTy g ty with
        | ValueNone -> false
        | ValueSome tcref ->
            tyconRefEq g g.choice2_tcr tcref
            || tyconRefEq g g.choice3_tcr tcref
            || tyconRefEq g g.choice4_tcr tcref
            || tyconRefEq g g.choice5_tcr tcref
            || tyconRefEq g g.choice6_tcr tcref
            || tyconRefEq g g.choice7_tcr tcref

    let tryDestOptionTy g ty =
        match argsOfAppTy g ty with
        | [ ty1 ] when isOptionTy g ty -> ValueSome ty1
        | _ -> ValueNone

    let tryDestValueOptionTy g ty =
        match argsOfAppTy g ty with
        | [ ty1 ] when isValueOptionTy g ty -> ValueSome ty1
        | _ -> ValueNone

    let tryDestChoiceTy g ty idx =
        match argsOfAppTy g ty with
        | ls when isChoiceTy g ty && ls.Length > idx -> ValueSome ls[idx]
        | _ -> ValueNone

    let destOptionTy g ty =
        match tryDestOptionTy g ty with
        | ValueSome ty -> ty
        | ValueNone -> failwith "destOptionTy: not an option type"

    let destValueOptionTy g ty =
        match tryDestValueOptionTy g ty with
        | ValueSome ty -> ty
        | ValueNone -> failwith "destValueOptionTy: not a value option type"

    let destChoiceTy g ty idx =
        match tryDestChoiceTy g ty idx with
        | ValueSome ty -> ty
        | ValueNone -> failwith "destChoiceTy: not a Choice type"

    let isNullableTy (g: TcGlobals) ty =
        match tryTcrefOfAppTy g ty with
        | ValueNone -> false
        | ValueSome tcref -> tyconRefEq g g.system_Nullable_tcref tcref

    let tryDestNullableTy g ty =
        match argsOfAppTy g ty with
        | [ ty1 ] when isNullableTy g ty -> ValueSome ty1
        | _ -> ValueNone

    let destNullableTy g ty =
        match tryDestNullableTy g ty with
        | ValueSome ty -> ty
        | ValueNone -> failwith "destNullableTy: not a Nullable type"

    [<return: Struct>]
    let (|NullableTy|_|) g ty =
        match tryAppTy g ty with
        | ValueSome(tcref, [ tyarg ]) when tyconRefEq g tcref g.system_Nullable_tcref -> ValueSome tyarg
        | _ -> ValueNone

    let (|StripNullableTy|) g ty =
        match tryDestNullableTy g ty with
        | ValueSome tyarg -> tyarg
        | _ -> ty

    let isLinqExpressionTy g ty =
        match tryTcrefOfAppTy g ty with
        | ValueNone -> false
        | ValueSome tcref -> tyconRefEq g g.system_LinqExpression_tcref tcref

    let tryDestLinqExpressionTy g ty =
        match argsOfAppTy g ty with
        | [ ty1 ] when isLinqExpressionTy g ty -> Some ty1
        | _ -> None

    let destLinqExpressionTy g ty =
        match tryDestLinqExpressionTy g ty with
        | Some ty -> ty
        | None -> failwith "destLinqExpressionTy: not an expression type"

    let mkNoneCase (g: TcGlobals) =
        mkUnionCaseRef g.option_tcr_canon "None"

    let mkSomeCase (g: TcGlobals) =
        mkUnionCaseRef g.option_tcr_canon "Some"

    let mkSome g ty arg m =
        mkUnionCaseExpr (mkSomeCase g, [ ty ], [ arg ], m)

    let mkNone g ty m =
        mkUnionCaseExpr (mkNoneCase g, [ ty ], [], m)

    let mkValueNoneCase (g: TcGlobals) =
        mkUnionCaseRef g.valueoption_tcr_canon "ValueNone"

    let mkValueSomeCase (g: TcGlobals) =
        mkUnionCaseRef g.valueoption_tcr_canon "ValueSome"

    let mkAnySomeCase g isStruct =
        (if isStruct then mkValueSomeCase g else mkSomeCase g)

    let mkValueSome g ty arg m =
        mkUnionCaseExpr (mkValueSomeCase g, [ ty ], [ arg ], m)

    let mkValueNone g ty m =
        mkUnionCaseExpr (mkValueNoneCase g, [ ty ], [], m)

    let isFSharpExceptionTy g ty =
        match tryTcrefOfAppTy g ty with
        | ValueSome tcref -> tcref.IsFSharpException
        | _ -> false
