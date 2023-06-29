// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Implements a set of checks on the TAST for a file that can only be performed after type inference
/// is complete.
module internal FSharp.Compiler.TailCallChecks

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeRelations

let PostInferenceChecksStackGuardDepth = GetEnvInteger "FSHARP_PostInferenceChecks" 50

//--------------------------------------------------------------------------
// check environment
//--------------------------------------------------------------------------

type env = 
    { 
      /// Values in module that have been marked [<TailCall>]
      mustTailCall: Zset<Val>
      
      /// Recursive expressions of [<TailCall>] attributed values
      mustTailCallExprs: Map<Stamp, Expr>
    } 

    override _.ToString() = "<env>"

let (|ValUseAtApp|_|) e = 
     match e with 
     | InnerExprPat(
         Expr.App(
             InnerExprPat(Expr.Val(valRef = vref; flags = valUseFlags)),_,_,[],_)
            | Expr.Val(valRef = vref; flags = valUseFlags)) -> Some (vref, valUseFlags)
     | _ -> None

type IsTailCall = 
    | Yes of bool // true indicates "has unit return type and must return void"
    | No

    static member private IsVoidRet (g: TcGlobals) (v: Val) =
        match v.ValReprInfo with 
        | Some info ->
            let _tps, tau = destTopForallTy g info v.Type
            let _curriedArgInfos, returnTy = GetTopTauTypeInFSharpForm g info.ArgInfos tau v.Range
            isUnitTy g returnTy
        | None -> false
        
    static member YesFromVal (g: TcGlobals) (v: Val) = IsTailCall.Yes (IsTailCall.IsVoidRet g v)
    
    static member YesFromExpr (g: TcGlobals) (expr: Expr) =
        match expr with
        | ValUseAtApp(valRef, _) -> IsTailCall.Yes (IsTailCall.IsVoidRet g valRef.Deref)
        | _ -> IsTailCall.Yes false

    member x.AtExprLambda = 
        match x with 
        // Inside a lambda that is considered an expression, we must always return "unit" not "void"
        | IsTailCall.Yes _ -> IsTailCall.Yes false
        | IsTailCall.No -> IsTailCall.No

let IsValRefIsDllImport g (vref:ValRef) = 
    vref.Attribs |> HasFSharpAttributeOpt g g.attrib_DllImportAttribute 

type cenv = 
    { stackGuard: StackGuard

      g: TcGlobals 

      amap: Import.ImportMap 

      reportErrors: bool }

    override x.ToString() = "<cenv>"

//--------------------------------------------------------------------------
// approx walk of type
//--------------------------------------------------------------------------

/// Indicates whether an address-of operation is permitted at a particular location
[<RequireQualifiedAccess>]
type PermitByRefExpr = 
    /// Permit a tuple of arguments where elements can be byrefs
    | YesTupleOfArgs of int 

    /// Context allows for byref typed expr. 
    | Yes

    /// Context allows for byref typed expr, but the byref must be returnable
    | YesReturnable

    /// Context allows for byref typed expr, but the byref must be returnable and a non-local
    | YesReturnableNonLocal

    /// General (address-of expr and byref values not allowed) 
    | No            

    member ctxt.Disallow = 
        match ctxt with 
        | PermitByRefExpr.Yes 
        | PermitByRefExpr.YesReturnable 
        | PermitByRefExpr.YesReturnableNonLocal -> false 
        | _ -> true

    member ctxt.PermitOnlyReturnable = 
        match ctxt with 
        | PermitByRefExpr.YesReturnable 
        | PermitByRefExpr.YesReturnableNonLocal -> true
        | _ -> false

    member ctxt.PermitOnlyReturnableNonLocal =
        match ctxt with
        | PermitByRefExpr.YesReturnableNonLocal -> true
        | _ -> false

let mkArgsPermit n = 
    if n=1 then PermitByRefExpr.Yes
    else PermitByRefExpr.YesTupleOfArgs n

/// Work out what byref-values are allowed at input positions to named F# functions or members
let mkArgsForAppliedVal isBaseCall (vref: ValRef) argsl = 
    match vref.ValReprInfo with
    | Some valReprInfo -> 
        let argArities = valReprInfo.AritiesOfArgs
        let argArities = if isBaseCall && argArities.Length >= 1 then List.tail argArities else argArities
        // Check for partial applications: arguments to partial applications don't get to use byrefs
        if List.length argsl >= argArities.Length then 
            List.map mkArgsPermit argArities
        else
            []
    | None -> []  

/// Work out what byref-values are allowed at input positions to functions
let rec mkArgsForAppliedExpr isBaseCall argsl x =
    match stripDebugPoints (stripExpr x) with 
    // recognise val 
    | Expr.Val (vref, _, _) -> mkArgsForAppliedVal isBaseCall vref argsl
    // step through instantiations 
    | Expr.App (f, _fty, _tyargs, [], _) -> mkArgsForAppliedExpr isBaseCall argsl f        
    // step through subsumption coercions 
    | Expr.Op (TOp.Coerce, _, [f], _) -> mkArgsForAppliedExpr isBaseCall argsl f        
    | _  -> []

let rec allValsAndExprsOfModDef mdef =
    seq { match mdef with 
          | TMDefRec(tycons = _tycons; bindings = mbinds) ->
              for mbind in mbinds do 
                match mbind with 
                | ModuleOrNamespaceBinding.Binding bind ->
                    yield bind.Var, bind.Expr
                | ModuleOrNamespaceBinding.Module(moduleOrNamespaceContents = def) -> yield! allValsAndExprsOfModDef def
          | TMDefLet(binding = bind) ->
              let e = stripExpr bind.Expr
              yield bind.Var, e
          | TMDefDo _ -> ()
          | TMDefOpens _ -> ()
          | TMDefs defs -> 
              for def in defs do 
                  yield! allValsAndExprsOfModDef def
    }

/// Check an expression, where the expression is in a position where byrefs can be generated
let rec CheckExprNoByrefs cenv env (isTailCall: IsTailCall) expr =
    CheckExpr cenv env expr PermitByRefExpr.No isTailCall |> ignore

/// Check a value
and CheckValRef (cenv: cenv) (env: env) (v: ValRef) m (_ctxt: PermitByRefExpr) (isTailCall: IsTailCall) = 
    // To warn for mutually recursive calls like in the following tests:
    // ``Warn for invalid tailcalls in rec module``
    // ``Warn successfully for invalid tailcalls in type methods``
    if cenv.reportErrors then 
        if cenv.g.langVersion.SupportsFeature LanguageFeature.WarningWhenTailRecAttributeButNonTailRecUsage then
            if env.mustTailCall.Contains v.Deref && isTailCall = IsTailCall.No then
                warning(Error(FSComp.SR.chkNotTailRecursive(v.DisplayName), m))

/// Check a use of a value
and CheckValUse (cenv: cenv) (env: env) (vref: ValRef, _vFlags, m) (ctxt: PermitByRefExpr) (isTailCall: IsTailCall) : unit = 
    CheckValRef cenv env vref m ctxt isTailCall
    
/// Check an expression, given information about the position of the expression
and CheckForOverAppliedExceptionRaisingPrimitive (cenv: cenv) (env: env) expr (isTailCall: IsTailCall) =    
    let g = cenv.g
    let expr = stripExpr expr
    let expr = stripDebugPoints expr

    // Some things are more easily checked prior to NormalizeAndAdjustPossibleSubsumptionExprs
    match expr with
    | Expr.App (f, _fty, _tyargs, argsl, m) ->

        if cenv.reportErrors then
            if cenv.g.langVersion.SupportsFeature LanguageFeature.WarningWhenTailRecAttributeButNonTailRecUsage then
                match f with 
                | ValUseAtApp (vref, valUseFlags) when env.mustTailCall.Contains vref.Deref ->

                    let canTailCall = 
                        match isTailCall with 
                        | IsTailCall.No ->  // an upper level has already decided that this is not in a tailcall position
                            false
                        | IsTailCall.Yes isVoidRet -> 
                            if vref.IsMemberOrModuleBinding && vref.ValReprInfo.IsSome then
                                let topValInfo = vref.ValReprInfo.Value
                                let (nowArgs, laterArgs), returnTy = 
                                    let _tps, tau = destTopForallTy g topValInfo _fty
                                    let curriedArgInfos, returnTy = GetTopTauTypeInFSharpForm cenv.g topValInfo.ArgInfos tau m
                                    if argsl.Length >= curriedArgInfos.Length then
                                        (List.splitAfter curriedArgInfos.Length argsl), returnTy
                                    else
                                        ([], argsl), returnTy
                                let _,_,isNewObj,isSuperInit,isSelfInit,_,_,_ = GetMemberCallInfo cenv.g (vref,valUseFlags) 
                                let isCCall = 
                                    match valUseFlags with
                                    | PossibleConstrainedCall _ ->  true
                                    | _ -> false
                                let hasByrefArg = nowArgs |> List.exists (tyOfExpr cenv.g >> isByrefTy cenv.g)
                                let mustGenerateUnitAfterCall = (isUnitTy g returnTy && not isVoidRet)

                                let noTailCallBlockers =
                                    not isNewObj &&
                                    not isSuperInit &&
                                    not isSelfInit  &&
                                    not mustGenerateUnitAfterCall &&
                                    isNil laterArgs && 
                                    not (IsValRefIsDllImport cenv.g vref) &&
                                    not isCCall && 
                                    not hasByrefArg
                                noTailCallBlockers  // blockers that will prevent the IL level from emmiting a tail instruction
                            else 
                                true

                    // warn if we call inside of recursive scope in non-tail-call manner or with tail blockers. See
                    // ``Warn successfully in match clause``
                    // ``Warn for byref parameters``
                    if not canTailCall then
                        warning(Error(FSComp.SR.chkNotTailRecursive(vref.DisplayName), m))
                | _ -> ()
    | _ -> ()

/// Check call arguments, including the return argument.
and CheckCall cenv env _m _returnTy args ctxts _ctxt =
    CheckExprs cenv env args ctxts IsTailCall.No

/// Check call arguments, including the return argument. The receiver argument is handled differently.
and CheckCallWithReceiver cenv env _m _returnTy args ctxts _ctxt =
    match args with
    | [] -> failwith "CheckCallWithReceiver: Argument list is empty."
    | receiverArg :: args ->

        let receiverContext, ctxts =
            match ctxts with
            | [] -> PermitByRefExpr.No, []
            | ctxt :: ctxts -> ctxt, ctxts

        CheckExpr cenv env receiverArg receiverContext IsTailCall.No
        CheckExprs cenv env args ctxts (IsTailCall.Yes false)

and CheckExprLinear (cenv: cenv) (env: env) expr (ctxt: PermitByRefExpr) (isTailCall: IsTailCall) : unit =    
    match expr with
    | Expr.Sequential (e1, e2, NormalSeq, _) -> 
        CheckExprNoByrefs cenv env IsTailCall.No e1
        // tailcall
        CheckExprLinear cenv env e2 ctxt isTailCall

    | Expr.Let (TBind(v, _bindRhs, _) as bind, body, _, _) ->
        let isByRef = isByrefTy cenv.g v.Type

        let bindingContext =
            if isByRef then
                PermitByRefExpr.YesReturnable
            else
                PermitByRefExpr.Yes

        CheckBinding cenv env false bindingContext bind  
        // tailcall
        CheckExprLinear cenv env body ctxt isTailCall

    | LinearOpExpr (_op, _tyargs, argsHead, argLast, _m) ->
        argsHead |> List.iter (CheckExprNoByrefs cenv env isTailCall) 
        // tailcall
        CheckExprLinear cenv env argLast PermitByRefExpr.No isTailCall

    | LinearMatchExpr (_spMatch, _exprm, dtree, tg1, e2, _m, _ty) ->
        CheckDecisionTree cenv env dtree
        CheckDecisionTreeTarget cenv env isTailCall ctxt tg1
        // tailcall
        CheckExprLinear cenv env e2 ctxt isTailCall

    | Expr.DebugPoint (_, innerExpr) -> 
        CheckExprLinear cenv env innerExpr ctxt isTailCall

    | _ -> 
        // not a linear expression
        CheckExpr cenv env expr ctxt isTailCall

/// Check an expression, given information about the position of the expression
and CheckExpr (cenv: cenv) (env: env) origExpr (ctxt: PermitByRefExpr) (isTailCall: IsTailCall) : unit =    
    
    // Guard the stack for deeply nested expressions
    cenv.stackGuard.Guard <| fun () ->
    
    let g = cenv.g

    let origExpr = stripExpr origExpr

    // CheckForOverAppliedExceptionRaisingPrimitive is more easily checked prior to NormalizeAndAdjustPossibleSubsumptionExprs
    CheckForOverAppliedExceptionRaisingPrimitive cenv env origExpr isTailCall
    let expr = NormalizeAndAdjustPossibleSubsumptionExprs g origExpr
    let expr = stripExpr expr

    match expr with
    | LinearOpExpr _ 
    | LinearMatchExpr _ 
    | Expr.Let _ 
    | Expr.Sequential (_, _, NormalSeq, _)
    | Expr.DebugPoint _ -> 
        CheckExprLinear cenv env expr ctxt isTailCall

    | Expr.Sequential (e1, e2, ThenDoSeq, _) -> 
        CheckExprNoByrefs cenv env IsTailCall.No e1
        CheckExprNoByrefs cenv env IsTailCall.No e2

    | Expr.Const (_, _m, _ty) -> 
        ()
            
    | Expr.Val (vref, vFlags, m) -> 
        CheckValUse cenv env (vref, vFlags, m) ctxt isTailCall
          
    | Expr.Quote (_ast, _savedConv, _isFromQueryExpression, _m, _ty) -> 
         ()

    | StructStateMachineExpr g info ->
        CheckStructStateMachineExpr cenv env expr info

    | Expr.Obj (_, ty, basev, superInitCall, overrides, iimpls, m) -> 
        CheckObjectExpr cenv env (ty, basev, superInitCall, overrides, iimpls, m)

    // Allow base calls to F# methods
    | Expr.App (InnerExprPat(ExprValWithPossibleTypeInst(v, vFlags, _, _)  as f), _fty, tyargs, Expr.Val (baseVal, _, _) :: rest, m) 
          when ((match vFlags with VSlotDirectCall -> true | _ -> false) && 
                baseVal.IsBaseVal) ->

        CheckFSharpBaseCall cenv env expr (v, f, _fty, tyargs, baseVal, rest, m)

    // Allow base calls to IL methods
    | Expr.Op (TOp.ILCall (isVirtual, _, _, _, _, _, _, ilMethRef, enclTypeInst, methInst, retTypes), tyargs, Expr.Val (baseVal, _, _) :: rest, m) 
          when not isVirtual && baseVal.IsBaseVal ->
        
        CheckILBaseCall cenv env (ilMethRef, enclTypeInst, methInst, retTypes, tyargs, baseVal, rest, m)

    | Expr.Op (op, tyargs, args, m) ->
        CheckExprOp cenv env (op, tyargs, args, m) ctxt expr

    // Allow 'typeof<System.Void>' calls as a special case, the only accepted use of System.Void! 
    | TypeOfExpr g ty when isVoidTy g ty ->
        ()

    // Allow 'typedefof<System.Void>' calls as a special case, the only accepted use of System.Void! 
    | TypeDefOfExpr g ty when isVoidTy g ty ->
        ()

    // Check an application
    | Expr.App (f, _fty, tyargs, argsl, m) ->
        CheckApplication cenv env expr (f, tyargs, argsl, m) ctxt isTailCall

    | Expr.Lambda (_, _, _, argvs, _, m, bodyTy) -> 
        CheckLambda cenv env expr (argvs, m, bodyTy) isTailCall

    | Expr.TyLambda (_, tps, _, m, bodyTy)  -> 
        CheckTyLambda cenv env expr (tps, m, bodyTy) isTailCall

    | Expr.TyChoose (_tps, e1, _)  -> 
        CheckExprNoByrefs cenv env isTailCall e1

    | Expr.Match (_, _, dtree, targets, m, ty) -> 
        CheckMatch cenv env ctxt (dtree, targets, m, ty) isTailCall

    | Expr.LetRec (binds, bodyExpr, _, _) ->  
        CheckLetRec cenv env (binds, bodyExpr) isTailCall

    | Expr.StaticOptimization (constraints, e2, e3, m) -> 
        CheckStaticOptimization cenv env (constraints, e2, e3, m)

    | Expr.WitnessArg _ ->
        ()

    | Expr.Link _ -> 
        failwith "Unexpected reclink"

and CheckStructStateMachineExpr cenv env _expr info =

    let (_dataTy,  
         (_moveNextThisVar, moveNextExpr), 
         (_setStateMachineThisVar, _setStateMachineStateVar, setStateMachineBody), 
         (_afterCodeThisVar, afterCodeBody)) = info

    CheckExprNoByrefs cenv env IsTailCall.No moveNextExpr
    CheckExprNoByrefs cenv env IsTailCall.No setStateMachineBody
    CheckExprNoByrefs cenv env IsTailCall.No afterCodeBody

and CheckObjectExpr cenv env (ty, basev, superInitCall, overrides, iimpls, _m) =
    CheckExprNoByrefs cenv env IsTailCall.No superInitCall
    CheckMethods cenv env basev (ty, overrides)
    CheckInterfaceImpls cenv env basev iimpls

and CheckFSharpBaseCall cenv env _expr (v, f, _fty, _tyargs, baseVal, rest, m) : unit =
    let memberInfo = Option.get v.MemberInfo
    if memberInfo.MemberFlags.IsDispatchSlot then
        ()
    else         
        CheckValRef cenv env v m PermitByRefExpr.No IsTailCall.No
        CheckValRef cenv env baseVal m PermitByRefExpr.No IsTailCall.No
        CheckExprs cenv env rest (mkArgsForAppliedExpr true rest f) IsTailCall.No

and CheckILBaseCall cenv env (_ilMethRef, _enclTypeInst, _methInst, _retTypes, _tyargs, baseVal, rest, m) : unit = 
    CheckValRef cenv env baseVal m PermitByRefExpr.No IsTailCall.No
    CheckExprsPermitByRefLike cenv env rest

and CheckApplication cenv env expr (f, _tyargs, argsl, m) ctxt (isTailCall: IsTailCall) : unit = 
    let g = cenv.g

    let returnTy = tyOfExpr g expr
    CheckExprNoByrefs cenv env isTailCall f

    let hasReceiver =
        match f with
        | Expr.Val (vref, _, _) when vref.IsInstanceMember && not argsl.IsEmpty -> true
        | _ -> false

    let ctxts = mkArgsForAppliedExpr false argsl f
    if hasReceiver then
        CheckCallWithReceiver cenv env m returnTy argsl ctxts ctxt
    else
        CheckCall cenv env m returnTy argsl ctxts ctxt

and CheckLambda cenv env expr (argvs, m, bodyTy) (isTailCall: IsTailCall) = 
    let valReprInfo = ValReprInfo ([], [argvs |> List.map (fun _ -> ValReprInfo.unnamedTopArg1)], ValReprInfo.unnamedRetVal) 
    let ty = mkMultiLambdaTy cenv.g m argvs bodyTy in 
    CheckLambdas false None cenv env false valReprInfo isTailCall.AtExprLambda false expr m ty PermitByRefExpr.Yes

and CheckTyLambda cenv env expr (tps, m, bodyTy) (isTailCall: IsTailCall) = 
    let valReprInfo = ValReprInfo (ValReprInfo.InferTyparInfo tps, [], ValReprInfo.unnamedRetVal) 
    let ty = mkForallTyIfNeeded tps bodyTy in 
    CheckLambdas false None cenv env false valReprInfo isTailCall.AtExprLambda false expr m ty PermitByRefExpr.Yes

and CheckMatch cenv env ctxt (dtree, targets, _m, _ty) isTailCall = 
    CheckDecisionTree cenv env dtree
    CheckDecisionTreeTargets cenv env targets ctxt isTailCall

and CheckLetRec cenv env (binds, bodyExpr) isTailCall =
    CheckBindings cenv env binds
    CheckExprNoByrefs cenv env isTailCall bodyExpr
    
and CheckStaticOptimization cenv env (_constraints, e2, e3, _m) = 
    CheckExprNoByrefs cenv env IsTailCall.No e2
    CheckExprNoByrefs cenv env IsTailCall.No e3
    
and CheckMethods cenv env baseValOpt (ty, methods) = 
    methods |> List.iter (CheckMethod cenv env baseValOpt ty) 

and CheckMethod cenv env _baseValOpt _ty (TObjExprMethod(_, _, _tps, _vs, body, _m)) = 
    CheckExpr cenv env body PermitByRefExpr.YesReturnableNonLocal IsTailCall.No |> ignore

and CheckInterfaceImpls cenv env baseValOpt l = 
    l |> List.iter (CheckInterfaceImpl cenv env baseValOpt)
    
and CheckInterfaceImpl cenv env baseValOpt overrides = 
    CheckMethods cenv env baseValOpt overrides 

and CheckExprOp cenv env (op, tyargs, args, m) ctxt expr : unit =
    let g = cenv.g

    // Special cases
    match op, tyargs, args with 
    // Handle these as special cases since mutables are allowed inside their bodies 
    | TOp.While _, _, [Expr.Lambda (_, _, _, [_], e1, _, _);Expr.Lambda (_, _, _, [_], e2, _, _)]  ->
        CheckExprsNoByRefLike cenv env [e1;e2]

    | TOp.TryFinally _, [_], [Expr.Lambda (_, _, _, [_], e1, _, _); Expr.Lambda (_, _, _, [_], e2, _, _)] ->
        CheckExpr cenv env e1 ctxt IsTailCall.No   // result of a try/finally can be a byref if in a position where the overall expression is can be a byref
        CheckExprNoByrefs cenv env IsTailCall.No e2

    | TOp.IntegerForLoop _, _, [Expr.Lambda (_, _, _, [_], e1, _, _);Expr.Lambda (_, _, _, [_], e2, _, _);Expr.Lambda (_, _, _, [_], e3, _, _)]  ->
        CheckExprsNoByRefLike cenv env [e1;e2;e3]

    | TOp.TryWith _, [_], [Expr.Lambda (_, _, _, [_], e1, _, _); Expr.Lambda (_, _, _, [_], _e2, _, _); Expr.Lambda (_, _, _, [_], e3, _, _)] ->
        CheckExpr cenv env e1 ctxt IsTailCall.No // result of a try/catch can be a byref if in a position where the overall expression is can be a byref
        // [(* e2; -- don't check filter body - duplicates logic in 'catch' body *) e3]
        CheckExpr cenv env e3 ctxt IsTailCall.No // result of a try/catch can be a byref if in a position where the overall expression is can be a byref
        
    | TOp.ILCall (_, _, _, _, _, _, _, ilMethRef, _enclTypeInst, _methInst, retTypes), _, _ ->

        let hasReceiver = 
            (ilMethRef.CallingConv.IsInstance || ilMethRef.CallingConv.IsInstanceExplicit) &&
            not args.IsEmpty

        let returnTy = tyOfExpr g expr

        let argContexts = List.init args.Length (fun _ -> PermitByRefExpr.Yes)

        match retTypes with
        | [ty] when ctxt.PermitOnlyReturnable && isByrefLikeTy g m ty -> 
            if hasReceiver then
                CheckCallWithReceiver cenv env m returnTy args argContexts ctxt
            else
                CheckCall cenv env m returnTy args argContexts ctxt
        | _ -> 
            if hasReceiver then
                CheckCallWithReceiver cenv env m returnTy args argContexts PermitByRefExpr.Yes
            else
                CheckCall cenv env m returnTy args argContexts PermitByRefExpr.Yes

    | TOp.Tuple tupInfo, _, _ when not (evalTupInfoIsStruct tupInfo) ->           
        match ctxt with 
        | PermitByRefExpr.YesTupleOfArgs _nArity -> 
            // This tuple should not be generated. The known function arity 
            // means it just bundles arguments. 
            CheckExprsPermitByRefLike cenv env args
        | _ -> 
            CheckExprsNoByRefLike cenv env args 

    | TOp.LValueOp (LAddrOf _, _vref), _, _ -> 
        CheckExprsNoByRefLike cenv env args

    | TOp.LValueOp (LByrefSet, _vref), _, [_arg] -> 
        ()

    | TOp.LValueOp (LByrefGet, _vref), _, [] -> 
        ()

    | TOp.LValueOp (LSet, _vref), _, [_arg] -> 
        ()

    | TOp.AnonRecdGet _, _, [arg1]
    | TOp.TupleFieldGet _, _, [arg1] -> 
        CheckExprsPermitByRefLike cenv env [arg1]

    | TOp.ValFieldGet _rf, _, [arg1] -> 
        CheckExprsPermitByRefLike cenv env [arg1]          

    | TOp.ValFieldSet _rf, _, [_arg1;_arg2] -> 
        ()

    | TOp.Coerce, [tgtTy;srcTy], [x] ->
        let isTailCall = IsTailCall.YesFromExpr cenv.g x
        if TypeDefinitelySubsumesTypeNoCoercion 0 g cenv.amap m tgtTy srcTy then
            CheckExpr cenv env x ctxt isTailCall
        else
            CheckExprNoByrefs cenv env isTailCall x

    | TOp.Reraise, [_ty1], [] ->
        ()

    // Check get of static field
    | TOp.ValFieldGetAddr (_rfref, _readonly), _tyargs, [] ->
        ()

    // Check get of instance field
    | TOp.ValFieldGetAddr (_rfref, _readonly), _tyargs, [obj] ->
        // Recursively check in same ctxt, e.g. if at PermitOnlyReturnable the obj arg must also be returnable
        CheckExpr cenv env obj ctxt IsTailCall.No

    | TOp.UnionCaseFieldGet _, _, [arg1] -> 
        CheckExprPermitByRefLike cenv env arg1

    | TOp.UnionCaseTagGet _, _, [arg1] -> 
        CheckExprPermitByRefLike cenv env arg1  // allow byref - it may be address-of-struct

    | TOp.UnionCaseFieldGetAddr (_uref, _idx, _readonly), _tyargs, [obj] ->
        // Recursively check in same ctxt, e.g. if at PermitOnlyReturnable the obj arg must also be returnable
        CheckExpr cenv env obj ctxt IsTailCall.No

    | TOp.ILAsm (instrs, _retTypes), _, _  ->
        match instrs, args with
        // Write a .NET instance field
        | [ I_stfld (_alignment, _vol, _fspec) ], _ ->
            match args with
            | [ _; rhs ] -> CheckExprNoByrefs cenv env IsTailCall.No rhs
            | _ -> ()
            
            // permit byref for lhs lvalue 
            // permit byref for rhs lvalue (field would have to have ByRefLike type, i.e. be a field in another ByRefLike type)
            CheckExprsPermitByRefLike cenv env args

        // Read a .NET instance field
        | [ I_ldfld (_alignment, _vol, _fspec) ], _ ->
            // permit byref for lhs lvalue 
            CheckExprsPermitByRefLike cenv env args

        // Read a .NET instance field
        | [ I_ldfld (_alignment, _vol, _fspec); AI_nop ], _ ->
            // permit byref for lhs lvalue of readonly value 
            CheckExprsPermitByRefLike cenv env args

        | [ I_ldsflda _fspec ], [] ->
            ()

        | [ I_ldflda _fspec ], [obj] ->

            // Recursively check in same ctxt, e.g. if at PermitOnlyReturnable the obj arg must also be returnable
            CheckExpr cenv env obj ctxt IsTailCall.No

        | [ I_ldelema (_, _isNativePtr, _, _) ], lhsArray :: indices ->
            // permit byref for lhs lvalue 
            CheckExprPermitByRefLike cenv env lhsArray
            CheckExprsNoByRefLike cenv env indices |> ignore

        | [ AI_conv _ ], _ ->
            // permit byref for args to conv 
            CheckExprsPermitByRefLike cenv env args 

        | _ ->
            CheckExprsNoByRefLike cenv env args  

    | TOp.TraitCall _, _, _ ->
        // CheckTypeInstNoByrefs cenv env m tyargs
        // allow args to be byref here 
        CheckExprsPermitByRefLike cenv env args
        
    | TOp.Recd _, _, _ ->
        CheckExprsPermitByRefLike cenv env args

    | _ -> 
        CheckExprsNoByRefLike cenv env args 

and CheckLambdas isTop (memberVal: Val option) cenv env inlined valReprInfo (isTailCall: IsTailCall) alwaysCheckNoReraise expr mOrig ety ctxt : unit =
    let g = cenv.g
    let memInfo = memberVal |> Option.bind (fun v -> v.MemberInfo)

    // The valReprInfo here says we are _guaranteeing_ to compile a function value 
    // as a .NET method with precisely the corresponding argument counts. 
    match stripDebugPoints expr with
    | Expr.TyChoose (_tps, e1, m)  -> 
        CheckLambdas isTop memberVal cenv env inlined valReprInfo isTailCall alwaysCheckNoReraise e1 m ety ctxt

    | Expr.Lambda (_, _, _, _, _, m, _)  
    | Expr.TyLambda (_, _, _, m, _) ->
        let _tps, ctorThisValOpt, baseValOpt, vsl, body, bodyTy = destLambdaWithValReprInfo g cenv.amap valReprInfo (expr, ety)
        let thisAndBase = Option.toList ctorThisValOpt @ Option.toList baseValOpt
        let restArgs = List.concat vsl

        match memInfo with 
        | None -> ()
        | Some mi -> 
            // ctorThis and baseVal values are always considered used
            for v in thisAndBase do v.SetHasBeenReferenced() 
            // instance method 'this' is always considered used
            match mi.MemberFlags.IsInstance, restArgs with
            | true, firstArg :: _ -> firstArg.SetHasBeenReferenced()
            | _ -> ()
            // any byRef arguments are considered used, as they may be 'out's
            for arg in restArgs do
                if isByrefTy g arg.Type then
                    arg.SetHasBeenReferenced()

        // Check the body of the lambda
        if isTop && not g.compilingFSharpCore && isByrefLikeTy g m bodyTy then
            // allow byref to occur as return position for byref-typed top level function or method
            CheckExprPermitReturnableByRef cenv env body |> ignore
        else
            CheckExprNoByrefs cenv env isTailCall body
                
    // This path is for expression bindings that are not actually lambdas
    | _ -> 
        let m = mOrig

        if not inlined && (isByrefLikeTy g m ety || isNativePtrTy g ety) then
            // allow byref to occur as RHS of byref binding. 
            CheckExpr cenv env expr ctxt isTailCall
        else 
            CheckExprNoByrefs cenv env isTailCall expr
                

and CheckExprs cenv env exprs ctxts isTailCall : unit =
    let ctxts = Array.ofList ctxts 
    let argArity i = if i < ctxts.Length then ctxts[i] else PermitByRefExpr.No 
    exprs 
    |> List.mapi (fun i exp -> CheckExpr cenv env exp (argArity i) isTailCall) 
    |> ignore

and CheckExprsNoByRefLike cenv env exprs : unit = 
    for expr in exprs do
        CheckExprNoByrefs cenv env IsTailCall.No expr

and CheckExprsPermitByRefLike cenv env exprs : unit = 
    exprs 
    |> List.map (CheckExprPermitByRefLike cenv env)
    |> ignore

and CheckExprPermitByRefLike cenv env expr : unit = 
    CheckExpr cenv env expr PermitByRefExpr.Yes IsTailCall.No

and CheckExprPermitReturnableByRef cenv env expr : unit = 
    CheckExpr cenv env expr PermitByRefExpr.YesReturnable IsTailCall.No

and CheckDecisionTreeTargets cenv env targets ctxt (isTailCall: IsTailCall) = 
    targets 
    |> Array.map (CheckDecisionTreeTarget cenv env isTailCall ctxt) 
    |> List.ofArray
    |> ignore

and CheckDecisionTreeTarget cenv env (isTailCall: IsTailCall) ctxt (TTarget(_vs, targetExpr, _)) : unit = 
    CheckExpr cenv env targetExpr ctxt isTailCall

and CheckDecisionTree cenv env dtree =
    match dtree with 
    | TDSuccess (resultExprs, _) -> 
        CheckExprsNoByRefLike cenv env resultExprs |> ignore
    | TDBind(bind, rest) -> 
        CheckBinding cenv env false PermitByRefExpr.Yes bind |> ignore
        CheckDecisionTree cenv env rest 
    | TDSwitch (inpExpr, cases, dflt, m) -> 
        CheckDecisionTreeSwitch cenv env (inpExpr, cases, dflt, m)

and CheckDecisionTreeSwitch cenv env (inpExpr, cases, dflt, m) =
    CheckExprPermitByRefLike cenv env inpExpr |> ignore// can be byref for struct union switch
    for (TCase(discrim, dtree)) in cases do
        CheckDecisionTreeTest cenv env m discrim
        CheckDecisionTree cenv env dtree
    dflt |> Option.iter (CheckDecisionTree cenv env) 

and CheckDecisionTreeTest cenv env _m discrim =
    match discrim with
    | DecisionTreeTest.ActivePatternCase (exp, _, _, _, _, _) -> CheckExprNoByrefs cenv env IsTailCall.No exp
    | _ -> ()

and CheckBinding cenv env alwaysCheckNoReraise ctxt (TBind(v, bindRhs, _) as bind) : unit =
    let g = cenv.g
    let isTop = Option.isSome bind.Var.ValReprInfo
    let isTailCall = IsTailCall.YesFromVal g bind.Var
    let valReprInfo  = match bind.Var.ValReprInfo with Some info -> info | _ -> ValReprInfo.emptyValData 
    CheckLambdas isTop (Some v) cenv env v.MustInline valReprInfo isTailCall alwaysCheckNoReraise bindRhs v.Range v.Type ctxt

and CheckBindings cenv env binds = 
    for bind in binds do
        CheckBinding cenv env false PermitByRefExpr.Yes bind |> ignore

// Top binds introduce expression, check they are reraise free.
let CheckModuleBinding cenv env (isRec: bool) (TBind(_v, _e, _) as bind) =
    // Check that a let binding to the result of a rec expression is not inside the rec expression
    // see test ``Warn for invalid tailcalls in seq expression because of bind`` for an example
    // see test ``Warn successfully for rec call in binding`` for an example
    if cenv.g.langVersion.SupportsFeature LanguageFeature.WarningWhenTailRecAttributeButNonTailRecUsage then
        match bind.Expr with
        | Expr.Lambda(_unique, _ctorThisValOpt, _baseValOpt, _valParams, bodyExpr, _range, _overallType) ->
            let rec checkTailCall (insideSubBinding: bool) expr =
                match expr with
                | Expr.Val(valRef, _valUseFlag, m) ->
                    if isRec && insideSubBinding && env.mustTailCall.Contains valRef.Deref then
                        warning(Error(FSComp.SR.chkNotTailRecursive(valRef.DisplayName), m))
                | Expr.App(funcExpr, _formalType, _typeArgs, exprs, _range) ->
                    checkTailCall insideSubBinding funcExpr
                    exprs |> List.iter (checkTailCall insideSubBinding)
                | Expr.Link exprRef -> checkTailCall insideSubBinding exprRef.Value
                | Expr.Lambda(_unique, _ctorThisValOpt, _baseValOpt, _valParams, bodyExpr, _range, _overallType) ->
                    checkTailCall insideSubBinding bodyExpr
                | Expr.DebugPoint(_debugPointAtLeafExpr, expr) -> checkTailCall insideSubBinding expr
                | Expr.Let(binding, bodyExpr, _range, _frees) ->
                    checkTailCall true binding.Expr
                    checkTailCall insideSubBinding bodyExpr
                | Expr.Match(_debugPointAtBinding, _inputRange, _decisionTree, decisionTreeTargets, _fullRange, _exprType) ->
                    decisionTreeTargets |> Array.iter (fun target -> checkTailCall insideSubBinding target.TargetExpression)
                | _ -> ()
            checkTailCall false bodyExpr
        | _ -> ()

    CheckBinding cenv env true PermitByRefExpr.Yes bind |> ignore

//--------------------------------------------------------------------------
// check modules
//--------------------------------------------------------------------------

let rec CheckDefnsInModule cenv env mdefs = 
    for mdef in mdefs do
        CheckDefnInModule cenv env mdef

and CheckDefnInModule cenv env mdef = 
    match mdef with 
    | TMDefRec(isRec, _opens, _tycons, mspecs, _m) -> 
        List.iter (CheckModuleSpec cenv env isRec) mspecs
    | TMDefLet(bind, _m)  -> 
        CheckModuleBinding cenv env false bind 
    | TMDefOpens _ ->
        ()
    | TMDefDo(e, _m)  -> 
        let isTailCall =
            match stripDebugPoints e with
            | Expr.App(funcExpr = funcExpr) ->
                match funcExpr with 
                | ValUseAtApp (vref, _valUseFlags) -> IsTailCall.YesFromVal cenv.g vref.Deref
                | _ -> IsTailCall.No
            | _ -> IsTailCall.No
        CheckExprNoByrefs cenv env isTailCall e
    | TMDefs defs -> CheckDefnsInModule cenv env defs 

and CheckModuleSpec cenv env isRec mbind =
    match mbind with 
    | ModuleOrNamespaceBinding.Binding bind ->
        CheckModuleBinding cenv env isRec bind
    | ModuleOrNamespaceBinding.Module (_mspec, rhs) ->
        CheckDefnInModule cenv env rhs 

let rec CollectCheckDefnsInModule cenv mdefs mustTailCall mustTailCallExpr =
    List.fold (fun (mustTailCall, mustTailCallExpr) mdef ->
        CollectCheckDefnInModule cenv mdef mustTailCall mustTailCallExpr
    ) (mustTailCall, mustTailCallExpr) mdefs
    
and CollectCheckDefnInModule cenv mdef (mustTailCall: Zset<Val>) (mustTailCallExpr: Map<Stamp, Expr>) = 
    match mdef with 
    | TMDefRec(isRec, _opens, _tycons, mspecs, _m) ->
        let mustTailCall'', mustTailCallExprs'' =
            if isRec then
                let vallsAndExprs = allValsAndExprsOfModDef mdef
                
                let mustTailCall', mustTailCallExpr' =
                    Seq.fold (fun (mustTailCall, mustTailCallExpr) (v: Val, e) ->
                        if HasFSharpAttribute cenv.g cenv.g.attrib_TailCallAttribute v.Attribs then
                            let newSet = Zset.add v mustTailCall
                            let newMap = Map.add v.Stamp e mustTailCallExpr
                            (newSet, newMap)
                        else
                            (mustTailCall, mustTailCallExpr)
                    ) (mustTailCall, mustTailCallExpr) vallsAndExprs
                    
                mustTailCall', mustTailCallExpr'
            else
                mustTailCall, mustTailCallExpr
        
        List.fold (fun (mustTailCall, mustTailCallExpr) mspec ->
            CollectCheckModuleSpec cenv mspec mustTailCall mustTailCallExpr
        ) (mustTailCall'', mustTailCallExprs'') mspecs
    | TMDefLet(_bind, _m)  ->
        mustTailCall, mustTailCallExpr
    | TMDefOpens _ ->
        mustTailCall, mustTailCallExpr
    | TMDefDo(_e, _m)  ->
        mustTailCall, mustTailCallExpr
    | TMDefs defs -> CollectCheckDefnsInModule cenv defs mustTailCall mustTailCallExpr

and CollectCheckModuleSpec cenv mbind mustTailCall mustTailCallExpr =
    match mbind with 
    | ModuleOrNamespaceBinding.Binding _bind ->
        mustTailCall, mustTailCallExpr
    | ModuleOrNamespaceBinding.Module (_mspec, rhs) ->
        CollectCheckDefnInModule cenv rhs mustTailCall mustTailCallExpr

let CheckImplFile (g, amap, reportErrors, implFileContents) =
    let cenv = 
        { g = g  
          reportErrors = reportErrors 
          stackGuard = StackGuard(PostInferenceChecksStackGuardDepth, "CheckImplFile")
          amap = amap }
    
    let mustTailCall, mustTailCallExprs = CollectCheckDefnInModule cenv implFileContents (Zset.empty valOrder) Map.Empty
    
    let env = 
        { mustTailCall = mustTailCall
          mustTailCallExprs = mustTailCallExprs }
    
    for v in env.mustTailCall do
        let expr = env.mustTailCallExprs[v.Stamp]
        let binding = Binding.TBind(v, expr, DebugPointAtBinding.NoneAtLet)
        CheckModuleBinding cenv env true binding
