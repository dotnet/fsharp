// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Find unsolved, uninstantiated type variables
module internal FSharp.Compiler.FindUnsolved

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypeRelations

type env = | NoEnv

type [<NoEquality; NoComparison>] WorkItem =
    | WExpr of env * Expr
    | WBind of env * Binding
    | WBinds of env * Binding list
    | WVal of env * Val
    | WTy of env * range * TType
    | WTypeInst of env * range * TType list
    | WOp of env * TOp * TType list * Expr list * range
    | WTraitInfo of env * range * TraitConstraintInfo
    | WLambdas of env * ValReprInfo * Expr * TType
    | WExprs of env * Expr list
    | WTargets of env * range * TType * DecisionTreeTarget array
    | WTarget of env * range * TType * DecisionTreeTarget
    | WDTree of env * DecisionTree
    | WSwitch of env * Expr * DecisionTreeCase list * DecisionTree option * range
    | WDiscrim of env * DecisionTreeTest * range
    | WAttrib of env * Attrib
    | WAttribs of env * Attribs
    | WValReprInfo of env * ValReprInfo
    | WArgReprInfo of env * ArgReprInfo
    | WTyconRecdField of env * Tycon * RecdField
    | WTycon of env * Tycon
    | WTycons of env * Tycon list
    | WModuleOrNamespaceDefs of env * ModuleOrNamespaceContents list
    | WModuleOrNamespaceDef of env * ModuleOrNamespaceContents
    | WModuleOrNamespaceBinds of env * ModuleOrNamespaceBinding list
    | WModuleOrNamespaceBind of env * ModuleOrNamespaceBinding
    | WMethods of env * Val option * ObjExprMethod list
    | WMethod of env * Val option * ObjExprMethod
    | WIntfImpls of env * Val option * range * (TType * ObjExprMethod list) list
    | WIntfImpl of env * Val option * range * TType * ObjExprMethod list

/// The environment and collector
type cenv =
    { g: TcGlobals
      amap: Import.ImportMap
      denv: DisplayEnv
      mutable unsolved: Typars
      stack: ResizeArray<WorkItem> }

    override _.ToString() = "<cenv>"

/// Walk types, collecting type variables
let accTy cenv _env (mFallback: range) ty =
    let normalizedTy = tryNormalizeMeasureInType cenv.g ty
    (freeInType CollectTyparsNoCaching normalizedTy).FreeTypars |> Zset.iter (fun tp ->
        if (tp.Rigidity <> TyparRigidity.Rigid) then
            match mFallback with
            | r when tp.Range = Range.range0 -> tp.SetIdent (FSharp.Compiler.Syntax.Ident(tp.typar_id.idText, r))
            | _ -> ()
            cenv.unsolved <- tp :: cenv.unsolved)

/// Push type arguments onto work stack
let accTypeInst cenv env mFallback tyargs =
    for ty in tyargs do
        cenv.stack.Add(WTy(env, mFallback, ty))

/// Push expressions onto work stack
let accExprs cenv env exprs =
    for expr in exprs do
        cenv.stack.Add(WExpr(env, expr))

/// Process work items with explicit stack
let processWorkItem cenv workItem =
    match workItem with
    | WExpr (env, expr) ->
        let expr = stripExpr expr
        match expr with
        | Expr.Sequential (e1, e2, _, _) ->
            cenv.stack.Add(WExpr(env, e2))
            cenv.stack.Add(WExpr(env, e1))

        | Expr.Let (bind, body, _, _) ->
            cenv.stack.Add(WExpr(env, body))
            cenv.stack.Add(WBind(env, bind))

        | Expr.Const (_, r, ty) ->
            accTy cenv env r ty

        | Expr.Val (_v, _vFlags, _m) -> ()

        | Expr.Quote (ast, _, _, m, ty) ->
            accTy cenv env m ty
            cenv.stack.Add(WExpr(env, ast))

        | Expr.Obj (_, ty, basev, basecall, overrides, iimpls, m) ->
            cenv.stack.Add(WIntfImpls(env, basev, m, iimpls))
            cenv.stack.Add(WMethods(env, basev, overrides))
            cenv.stack.Add(WExpr(env, basecall))
            accTy cenv env m ty

        | LinearOpExpr (_op, tyargs, argsHead, argLast, m) ->
            cenv.stack.Add(WExpr(env, argLast))
            accExprs cenv env argsHead
            accTypeInst cenv env m tyargs

        | Expr.Op (c, tyargs, args, m) ->
            cenv.stack.Add(WOp(env, c, tyargs, args, m))

        | Expr.App (f, fty, tyargs, argsl, m) ->
            accExprs cenv env argsl
            cenv.stack.Add(WExpr(env, f))
            accTypeInst cenv env m tyargs
            accTy cenv env m fty

        | Expr.Lambda (_, _ctorThisValOpt, _baseValOpt, argvs, _body, m, bodyTy) ->
            let valReprInfo = ValReprInfo ([], [argvs |> List.map (fun _ -> ValReprInfo.unnamedTopArg1)], ValReprInfo.unnamedRetVal)
            let ty = mkMultiLambdaTy cenv.g m argvs bodyTy
            cenv.stack.Add(WLambdas(env, valReprInfo, expr, ty))

        | Expr.TyLambda (_, tps, _body, m, bodyTy)  ->
            let valReprInfo = ValReprInfo (ValReprInfo.InferTyparInfo tps, [], ValReprInfo.unnamedRetVal)
            let ty = mkForallTyIfNeeded tps bodyTy
            cenv.stack.Add(WLambdas(env, valReprInfo, expr, ty))
            accTy cenv env m bodyTy

        | Expr.TyChoose (_tps, e1, _m)  ->
            cenv.stack.Add(WExpr(env, e1))

        | Expr.Match (_, _exprm, dtree, targets, m, ty) ->
            cenv.stack.Add(WTargets(env, m, ty, targets))
            cenv.stack.Add(WDTree(env, dtree))
            accTy cenv env m ty

        | Expr.LetRec (binds, e, _m, _) ->
            cenv.stack.Add(WExpr(env, e))
            cenv.stack.Add(WBinds(env, binds))

        | Expr.StaticOptimization (constraints, e2, e3, m) ->
            for constr in constraints do
                match constr with
                | TTyconEqualsTycon(ty1, ty2) ->
                    accTy cenv env m ty1
                    accTy cenv env m ty2
                | TTyconIsStruct(ty1) ->
                    accTy cenv env m ty1
            cenv.stack.Add(WExpr(env, e3))
            cenv.stack.Add(WExpr(env, e2))

        | Expr.WitnessArg (traitInfo, m) ->
            cenv.stack.Add(WTraitInfo(env, m, traitInfo))

        | Expr.Link eref ->
            cenv.stack.Add(WExpr(env, eref.Value))

        | Expr.DebugPoint (_, innerExpr) ->
            cenv.stack.Add(WExpr(env, innerExpr))

    | WBind (env, bind) ->
        let valReprInfo =
            match bind.Var.ValReprInfo with
            | Some info ->
                info
            | _ ->
                ValReprInfo.emptyValData
        cenv.stack.Add(WLambdas(env, valReprInfo, bind.Expr, bind.Var.Type))
        cenv.stack.Add(WVal(env, bind.Var))

    | WBinds (env, binds) ->
        for bind in binds do
            cenv.stack.Add(WBind(env, bind))

    | WVal (env, v) ->
        accTy cenv env v.Range v.Type

        if Option.isSome v.ValReprInfo then
            cenv.stack.Add(WValReprInfo(env, v.ValReprInfo.Value))

        cenv.stack.Add(WAttribs(env, v.Attribs))

    | WTy (env, mFallback, ty) ->
        accTy cenv env mFallback ty

    | WTypeInst (env, mFallback, tyargs) ->
        accTypeInst cenv env mFallback tyargs

    | WOp (env, op, tyargs, args, m) ->
        match op with
        | TOp.ILCall (_, _, _, _, _, _, _, _, enclTypeInst, methInst, retTys) ->
            accTypeInst cenv env m retTys
            accTypeInst cenv env m methInst
            accTypeInst cenv env m enclTypeInst
        | TOp.TraitCall traitInfo ->
            cenv.stack.Add(WTraitInfo(env, m, traitInfo))
        | TOp.ILAsm (_, retTys) ->
            accTypeInst cenv env m retTys
        | _ -> ()
        accExprs cenv env args
        accTypeInst cenv env m tyargs

    | WTraitInfo (env, mFallback, TTrait(tys=tys; objAndArgTys=argTys; returnTyOpt=retTy)) ->
        for ty in tys do
            accTy cenv env mFallback ty

        if Option.isSome retTy then
            accTy cenv env mFallback retTy.Value

        accTypeInst cenv env mFallback argTys

    | WLambdas (env, valReprInfo, expr, exprTy) ->
        match stripDebugPoints expr with
        | Expr.TyChoose (_tps, bodyExpr, _m) ->
            cenv.stack.Add(WLambdas(env, valReprInfo, bodyExpr, exprTy))
        | Expr.Lambda (range = range)
        | Expr.TyLambda (range = range) ->
            let _tps, ctorThisValOpt, baseValOpt, vsl, body, bodyTy = destLambdaWithValReprInfo cenv.g cenv.amap valReprInfo (expr, exprTy)

            cenv.stack.Add(WExpr(env, body))

            if Option.isSome ctorThisValOpt then
                cenv.stack.Add(WVal(env, ctorThisValOpt.Value))

            if Option.isSome baseValOpt then
                cenv.stack.Add(WVal(env, baseValOpt.Value))

            for vs in vsl do
                for v in vs do
                    cenv.stack.Add(WVal(env, v))

            accTy cenv env range bodyTy
        | _ ->
            cenv.stack.Add(WExpr(env, expr))

    | WExprs (env, exprs) ->
        accExprs cenv env exprs

    | WTargets (env, m, ty, targets) ->
        for target in targets do
            cenv.stack.Add(WTarget(env, m, ty, target))

    | WTarget (env, _m, _ty, TTarget(_vs, e, _)) ->
        cenv.stack.Add(WExpr(env, e))

    | WDTree (env, dtree) ->
        match dtree with
        | TDSuccess (es, _n) ->
            accExprs cenv env es
        | TDBind(bind, rest) ->
            cenv.stack.Add(WDTree(env, rest))
            cenv.stack.Add(WBind(env, bind))
        | TDSwitch (e, cases, dflt, m) ->
            cenv.stack.Add(WSwitch(env, e, cases, dflt, m))

    | WSwitch (env, e, cases, dflt, m) ->

        if Option.isSome dflt then
            cenv.stack.Add(WDTree(env, dflt.Value))

        for (TCase(discrim, e)) in cases do
            cenv.stack.Add(WDTree(env, e))
            cenv.stack.Add(WDiscrim(env, discrim, m))

        cenv.stack.Add(WExpr(env, e))

    | WDiscrim (env, d, mFallback) ->
        match d with
        | DecisionTreeTest.UnionCase(_ucref, tinst) ->
            accTypeInst cenv env mFallback tinst
        | DecisionTreeTest.ArrayLength(_, ty) ->
            accTy cenv env mFallback ty
        | DecisionTreeTest.Const _
        | DecisionTreeTest.IsNull -> ()
        | DecisionTreeTest.IsInst (srcTy, tgtTy) ->
            accTy cenv env mFallback tgtTy
            accTy cenv env mFallback srcTy
        | DecisionTreeTest.ActivePatternCase (exp, tys, _, _, _, _) ->
            accTypeInst cenv env mFallback tys
            cenv.stack.Add(WExpr(env, exp))
        | DecisionTreeTest.Error _ -> ()

    | WAttrib (env, Attrib(_, _k, args, props, _, _, m)) ->
        for (AttribNamedArg(_nm, ty, _flg, AttribExpr(expr, expr2))) in props do
            accTy cenv env m ty
            cenv.stack.Add(WExpr(env, expr2))
            cenv.stack.Add(WExpr(env, expr))
        for (AttribExpr(expr1, expr2)) in args do
            cenv.stack.Add(WExpr(env, expr2))
            cenv.stack.Add(WExpr(env, expr1))

    | WAttribs (env, attribs) ->
        for attrib in attribs do
            cenv.stack.Add(WAttrib(env, attrib))

    | WValReprInfo (env, ValReprInfo(_, args, ret)) ->
        cenv.stack.Add(WArgReprInfo(env, ret))

        for argInfos in args do
            for argInfo in argInfos do
                cenv.stack.Add(WArgReprInfo(env, argInfo))

    | WArgReprInfo (env, argInfo) ->
        cenv.stack.Add(WAttribs(env, argInfo.Attribs))

    | WTyconRecdField (env, _tycon, rfield) ->
        cenv.stack.Add(WAttribs(env, rfield.FieldAttribs))
        cenv.stack.Add(WAttribs(env, rfield.PropertyAttribs))

    | WTycon (env, tycon) ->
        if tycon.IsUnionTycon then
            for uc in tycon.UnionCasesArray do
                for rf in uc.RecdFieldsArray do
                    cenv.stack.Add(WTyconRecdField(env, tycon, rf))
                cenv.stack.Add(WAttribs(env, uc.Attribs))

        for rf in tycon.AllFieldsArray do
            cenv.stack.Add(WTyconRecdField(env, tycon, rf))

        for v in abstractSlotValsOfTycons [tycon] do
            cenv.stack.Add(WVal(env, v))

        cenv.stack.Add(WAttribs(env, tycon.Attribs))

    | WTycons (env, tycons) ->
        for tycon in tycons do
            cenv.stack.Add(WTycon(env, tycon))

    | WModuleOrNamespaceDefs (env, defs) ->
        for def in defs do
            cenv.stack.Add(WModuleOrNamespaceDef(env, def))

    | WModuleOrNamespaceDef (env, def) ->
        match def with
        | TMDefRec(_, _opens, tycons, mbinds, _m) ->
            cenv.stack.Add(WModuleOrNamespaceBinds(env, mbinds))
            cenv.stack.Add(WTycons(env, tycons))
        | TMDefLet(bind, _m) ->
            cenv.stack.Add(WBind(env, bind))
        | TMDefDo(e, _m) ->
            cenv.stack.Add(WExpr(env, e))
        | TMDefOpens _ -> ()
        | TMDefs defs ->
            cenv.stack.Add(WModuleOrNamespaceDefs(env, defs))

    | WModuleOrNamespaceBinds (env, xs) ->
        for x in xs do
            cenv.stack.Add(WModuleOrNamespaceBind(env, x))

    | WModuleOrNamespaceBind (env, x) ->
        match x with
        | ModuleOrNamespaceBinding.Binding bind ->
            cenv.stack.Add(WBind(env, bind))
        | ModuleOrNamespaceBinding.Module(mspec, rhs) ->
            cenv.stack.Add(WModuleOrNamespaceDef(env, rhs))
            cenv.stack.Add(WTycon(env, mspec))

    | WMethods (env, baseValOpt, l) ->
        for m in l do
            cenv.stack.Add(WMethod(env, baseValOpt, m))

    | WMethod (env, _baseValOpt, TObjExprMethod(_slotsig, _attribs, _tps, vs, bodyExpr, _m)) ->
        cenv.stack.Add(WExpr(env, bodyExpr))

        for vsList in vs do
            for v in vsList do
                cenv.stack.Add(WVal(env, v))

    | WIntfImpls (env, baseValOpt, mFallback, l) ->
        for impl in l do
            let (ty, overrides) = impl
            cenv.stack.Add(WIntfImpl(env, baseValOpt, mFallback, ty, overrides))

    | WIntfImpl (env, baseValOpt, mFallback, ty, overrides) ->
        cenv.stack.Add(WMethods(env, baseValOpt, overrides))
        accTy cenv env mFallback ty

/// Find all unsolved inference variables after type inference for an entire file
let UnsolvedTyparsOfModuleDef g amap denv mdef extraAttribs =
    let stack = ResizeArray<WorkItem>()
    let cenv =
        { g = g
          amap = amap
          denv = denv
          unsolved = []
          stack = stack }

    stack.Add(WAttribs(NoEnv, extraAttribs))
    stack.Add(WModuleOrNamespaceDef(NoEnv, mdef))

    while stack.Count > 0 do
        let lastIndex = stack.Count - 1
        let workItem = stack.[lastIndex]
        stack.RemoveAt(lastIndex)
        processWorkItem cenv workItem

    List.rev cenv.unsolved
