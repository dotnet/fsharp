// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Find unsolved, uninstantiated type variables
module internal FSharp.Compiler.FindUnsolved

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypeRelations

type env = | NoEnv

let FindUnsolvedStackGuardDepth = StackGuard.GetDepthOption "FindUnsolved"

/// The environment and collector
type cenv = 
    { g: TcGlobals 
      amap: Import.ImportMap 
      denv: DisplayEnv 
      mutable unsolved: Typars 
      stackGuard: StackGuard }

    override _.ToString() = "<cenv>"

/// Walk types, collecting type variables.
/// The backupRange is attached best-effort to unsolved type parameters, for better reporting.
let accTy cenv _env (backupRange: Text.range) ty =
    let normalizedTy = tryNormalizeMeasureInType cenv.g ty
    (freeInType CollectTyparsNoCaching normalizedTy).FreeTypars |> Zset.iter (fun tp -> 
        if (tp.Rigidity <> TyparRigidity.Rigid) then
            let tp =
                if tp.Range = Text.Range.range0 then
                    { tp with typar_id = Syntax.Ident(tp.typar_id.idText, backupRange) }
                else tp
            cenv.unsolved <- tp :: cenv.unsolved)

let accTypeInst cenv env (backupRange: Text.range) tyargs =
    tyargs |> List.iter (accTy cenv env backupRange)

/// Walk expressions, collecting type variables
let rec accExpr (cenv: cenv) (env: env) expr =
    cenv.stackGuard.Guard <| fun () ->

    let expr = stripExpr expr 
    match expr with
    | Expr.Sequential (e1, e2, _, _) -> 
        accExpr cenv env e1 
        accExpr cenv env e2

    | Expr.Let (bind, body, _, _) ->  
        accBind cenv env bind  
        accExpr cenv env body

    | Expr.Const (_, m, ty) ->
        accTy cenv env m ty
    
    | Expr.Val (_v, _vFlags, _m) -> ()

    | Expr.Quote (ast, _, _, m, ty) ->
        accExpr cenv env ast
        accTy cenv env m ty

    | Expr.Obj (_, ty, basev, basecall, overrides, iimpls, m) ->
        accTy cenv env m ty
        accExpr cenv env basecall
        accMethods cenv env basev overrides 
        accIntfImpls cenv env basev m iimpls

    | LinearOpExpr (_op, tyargs, argsHead, argLast, m) ->
        // Note, LinearOpExpr doesn't include any of the "special" cases for accOp
        accTypeInst cenv env m tyargs
        accExprs cenv env argsHead
        // tailcall
        accExpr cenv env argLast

    | Expr.Op (c, tyargs, args, m) ->
        accOp cenv env (c, tyargs, args, m) 

    | Expr.App (f, fty, tyargs, argsl, m) ->
        accTy cenv env m fty
        accTypeInst cenv env m tyargs
        accExpr cenv env f
        accExprs cenv env argsl

    | Expr.Lambda (_, _ctorThisValOpt, _baseValOpt, argvs, _body, m, bodyTy) -> 
        let valReprInfo = ValReprInfo ([], [argvs |> List.map (fun _ -> ValReprInfo.unnamedTopArg1)], ValReprInfo.unnamedRetVal) 
        let ty = mkMultiLambdaTy cenv.g m argvs bodyTy 
        accLambdas cenv env valReprInfo expr ty

    | Expr.TyLambda (_, tps, _body, m, bodyTy)  ->
        let valReprInfo = ValReprInfo (ValReprInfo.InferTyparInfo tps, [], ValReprInfo.unnamedRetVal) 
        accTy cenv env m bodyTy
        let ty = mkForallTyIfNeeded tps bodyTy 
        accLambdas cenv env valReprInfo expr ty

    | Expr.TyChoose (_tps, e1, _m)  -> 
        accExpr cenv env e1 

    | Expr.Match (_, _exprm, dtree, targets, m, ty) -> 
        accTy cenv env m ty
        accDTree cenv env dtree
        accTargets cenv env m ty targets

    | Expr.LetRec (binds, e, _m, _) ->  
        accBinds cenv env binds
        accExpr cenv env e

    | Expr.StaticOptimization (constraints, e2, e3, m) ->
        accExpr cenv env e2
        accExpr cenv env e3
        constraints |> List.iter (function 
            | TTyconEqualsTycon(ty1, ty2) -> 
                accTy cenv env m ty1
                accTy cenv env m ty2
            | TTyconIsStruct(ty1) -> 
                accTy cenv env m ty1)

    | Expr.WitnessArg (traitInfo, m) ->
        accTraitInfo cenv env m traitInfo

    | Expr.Link eref ->
        accExpr cenv env eref.Value

    | Expr.DebugPoint (_, innerExpr) ->
        accExpr cenv env innerExpr

and accMethods cenv env baseValOpt l = 
    List.iter (accMethod cenv env baseValOpt) l

and accMethod cenv env _baseValOpt (TObjExprMethod(_slotsig, _attribs, _tps, vs, bodyExpr, m)) =
    vs |> List.iterSquared (accVal cenv env m)
    accExpr cenv env bodyExpr

and accIntfImpls cenv env baseValOpt (backupRange: Text.range) l =
    List.iter (accIntfImpl cenv env baseValOpt backupRange) l

and accIntfImpl cenv env baseValOpt (backupRange: Text.range) (ty, overrides) =
    accTy cenv env backupRange ty
    accMethods cenv env baseValOpt overrides 

and accOp cenv env (op, tyargs, args, m) =
    // Special cases 
    accTypeInst cenv env m tyargs
    accExprs cenv env args
    match op with 
    // Handle these as special cases since mutables are allowed inside their bodies 
    | TOp.ILCall (_, _, _, _, _, _, _, _, enclTypeInst, methInst, retTys) ->
        accTypeInst cenv env m enclTypeInst
        accTypeInst cenv env m methInst
        accTypeInst cenv env m retTys
    | TOp.TraitCall traitInfo -> 
        accTraitInfo cenv env m traitInfo
        
    | TOp.ILAsm (_, retTys) ->
        accTypeInst cenv env m retTys
    | _ ->    ()

and accTraitInfo cenv env (backupRange : Text.range) (TTrait(tys, _nm, _, argTys, retTy, _sln)) =
    argTys |> accTypeInst cenv env backupRange
    retTy |> Option.iter (accTy cenv env backupRange)
    tys |> List.iter (accTy cenv env backupRange)

and accLambdas cenv env valReprInfo expr exprTy =
    match stripDebugPoints expr with
    | Expr.TyChoose (_tps, bodyExpr, _m)  -> accLambdas cenv env valReprInfo bodyExpr exprTy      
    | Expr.Lambda (_, _, _, _, _, m, _)
    | Expr.TyLambda (_, _, _, m, _) ->
        let _tps, ctorThisValOpt, baseValOpt, vsl, body, bodyTy = destTopLambda cenv.g cenv.amap valReprInfo (expr, exprTy) 
        accTy cenv env expr.Range bodyTy
        vsl |> List.iterSquared (accVal cenv env m)
        baseValOpt |> Option.iter (accVal cenv env m)
        ctorThisValOpt |> Option.iter (accVal cenv env m)
        accExpr cenv env body
    | _ -> 
        accExpr cenv env expr

and accExprs cenv env exprs = 
    exprs |> List.iter (accExpr cenv env) 

and accTargets cenv env m ty targets = 
    Array.iter (accTarget cenv env m ty) targets

and accTarget cenv env _m _ty (TTarget(_vs, e, _)) = 
    accExpr cenv env e

and accDTree cenv env dtree =
    match dtree with 
    | TDSuccess (es, _n) -> accExprs cenv env es
    | TDBind(bind, rest) -> accBind cenv env bind; accDTree cenv env rest 
    | TDSwitch (e, cases, dflt, m) -> accSwitch cenv env (e, cases, dflt, m)

and accSwitch cenv env (e, cases, dflt, m) =
    accExpr cenv env e
    cases |> List.iter (fun (TCase(discrim, e)) -> accDiscrim cenv env m discrim; accDTree cenv env e)
    dflt |> Option.iter (accDTree cenv env) 

and accDiscrim cenv env backupRange d =
    match d with 
    | DecisionTreeTest.UnionCase(_ucref, tinst) -> accTypeInst cenv env backupRange tinst
    | DecisionTreeTest.ArrayLength(_, ty) -> accTy cenv env backupRange ty
    | DecisionTreeTest.Const _
    | DecisionTreeTest.IsNull -> ()
    | DecisionTreeTest.IsInst (srcTy, tgtTy) -> accTy cenv env backupRange srcTy; accTy cenv env backupRange tgtTy
    | DecisionTreeTest.ActivePatternCase (exp, tys, _, _, _, _) -> 
        accExpr cenv env exp
        accTypeInst cenv env exp.Range tys
    | DecisionTreeTest.Error _ -> ()

and accAttrib cenv env (Attrib(_, _k, args, props, _, _, m)) =
    args |> List.iter (fun (AttribExpr(expr1, expr2)) -> 
        accExpr cenv env expr1
        accExpr cenv env expr2)
    props |> List.iter (fun (AttribNamedArg(_nm, ty, _flg, AttribExpr(expr, expr2))) -> 
        accExpr cenv env expr
        accExpr cenv env expr2
        accTy cenv env m ty)
  
and accAttribs cenv env attribs = 
    List.iter (accAttrib cenv env) attribs

and accValReprInfo cenv env (ValReprInfo(_, args, ret)) =
    args |> List.iterSquared (accArgReprInfo cenv env)
    ret |> accArgReprInfo cenv env

and accArgReprInfo cenv env (argInfo: ArgReprInfo) = 
    accAttribs cenv env argInfo.Attribs

and accVal cenv env (backupRange: Text.range) v =
    v.Attribs |> accAttribs cenv env
    v.ValReprInfo |> Option.iter (accValReprInfo cenv env)
    v.Type |> accTy cenv env backupRange

and accBind cenv env (bind: Binding) =
    accVal cenv env bind.Expr.Range bind.Var
    let valReprInfo  = match bind.Var.ValReprInfo with Some info -> info | _ -> ValReprInfo.emptyValData
    accLambdas cenv env valReprInfo bind.Expr bind.Var.Type

and accBinds cenv env binds = 
    binds |> List.iter (accBind cenv env) 

let accTyconRecdField cenv env _tycon (rfield:RecdField) = 
    accAttribs cenv env rfield.PropertyAttribs
    accAttribs cenv env rfield.FieldAttribs

let accTycon cenv env (tycon:Tycon) =
    accAttribs cenv env tycon.Attribs
    abstractSlotValsOfTycons [tycon] |> List.iter (accVal cenv env tycon.Range)
    tycon.AllFieldsArray |> Array.iter (accTyconRecdField cenv env tycon)
    if tycon.IsUnionTycon then                             (* This covers finite unions. *)
      tycon.UnionCasesArray |> Array.iter (fun uc ->
          accAttribs cenv env uc.Attribs
          uc.RecdFieldsArray |> Array.iter (accTyconRecdField cenv env tycon))

let accTycons cenv env tycons = 
    List.iter (accTycon cenv env) tycons

let rec accModuleOrNamespaceDefs cenv env defs = 
    List.iter (accModuleOrNamespaceDef cenv env) defs

and accModuleOrNamespaceDef cenv env def = 
    match def with 
    | TMDefRec(_, _opens, tycons, mbinds, _m) -> 
        accTycons cenv env tycons
        accModuleOrNamespaceBinds cenv env mbinds 
    | TMDefLet(bind, _m)  -> accBind cenv env bind 
    | TMDefDo(e, _m)  -> accExpr cenv env e
    | TMDefOpens _ -> ()
    | TMDefs defs -> accModuleOrNamespaceDefs cenv env defs 

and accModuleOrNamespaceBinds cenv env xs = 
    List.iter (accModuleOrNamespaceBind cenv env) xs

and accModuleOrNamespaceBind cenv env x = 
    match x with 
    | ModuleOrNamespaceBinding.Binding bind -> 
        accBind cenv env bind
    | ModuleOrNamespaceBinding.Module(mspec, rhs) -> 
        accTycon cenv env mspec
        accModuleOrNamespaceDef cenv env rhs 

let UnsolvedTyparsOfModuleDef g amap denv mdef extraAttribs =
    let cenv = 
        { g =g  
          amap=amap 
          denv=denv 
          unsolved = [] 
          stackGuard = StackGuard(FindUnsolvedStackGuardDepth) }
    accModuleOrNamespaceDef cenv NoEnv mdef
    accAttribs cenv NoEnv extraAttribs
    List.rev cenv.unsolved


