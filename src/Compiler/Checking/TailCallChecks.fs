// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

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

let PostInferenceChecksStackGuardDepth = GetEnvInteger "FSHARP_TailCallChecks" 50

//--------------------------------------------------------------------------
// check environment
//--------------------------------------------------------------------------

type env =
    {
        /// Values in module that have been marked [<TailCall>]
        mustTailCall: Zset<Val>
    }

    override _.ToString() = "<env>"

let (|ValUseAtApp|_|) e =
    match e with
    | InnerExprPat (Expr.App(funcExpr = InnerExprPat (Expr.Val (valRef = vref; flags = valUseFlags))) | Expr.Val (valRef = vref
                                                                                                                  flags = valUseFlags)) ->
        Some(vref, valUseFlags)
    | _ -> None

type TailCallReturnType =
    | MustReturnVoid // indicates "has unit return type and must return void"
    | NonVoid

type TailCall =
    | Yes of TailCallReturnType
    | No

    static member private IsVoidRet (g: TcGlobals) (v: Val) =
        match v.ValReprInfo with
        | Some info ->
            let _tps, tau = destTopForallTy g info v.Type

            let _curriedArgInfos, returnTy =
                GetTopTauTypeInFSharpForm g info.ArgInfos tau v.Range

            if isUnitTy g returnTy then
                TailCallReturnType.MustReturnVoid
            else
                TailCallReturnType.NonVoid
        | None -> TailCallReturnType.NonVoid

    static member YesFromVal (g: TcGlobals) (v: Val) = TailCall.Yes(TailCall.IsVoidRet g v)

    static member YesFromExpr (g: TcGlobals) (expr: Expr) =
        match expr with
        | ValUseAtApp (valRef, _) -> TailCall.Yes(TailCall.IsVoidRet g valRef.Deref)
        | _ -> TailCall.Yes TailCallReturnType.NonVoid

    member x.AtExprLambda =
        match x with
        // Inside a lambda that is considered an expression, we must always return "unit" not "void"
        | TailCall.Yes _ -> TailCall.Yes TailCallReturnType.NonVoid
        | TailCall.No -> TailCall.No

let IsValRefIsDllImport g (vref: ValRef) =
    vref.Attribs |> HasFSharpAttributeOpt g g.attrib_DllImportAttribute

type cenv =
    {
        stackGuard: StackGuard

        g: TcGlobals

        amap: Import.ImportMap

        reportErrors: bool
    }

    override x.ToString() = "<cenv>"

//--------------------------------------------------------------------------
// approx walk of type
//--------------------------------------------------------------------------

/// Indicates whether an address-of operation is permitted at a particular location
/// Type definition taken from PostInferenceChecks.fs. To be kept in sync.
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

    member ctxt.PermitOnlyReturnable =
        match ctxt with
        | PermitByRefExpr.YesReturnable
        | PermitByRefExpr.YesReturnableNonLocal -> true
        | _ -> false

let mkArgsPermit n =
    if n = 1 then
        PermitByRefExpr.Yes
    else
        PermitByRefExpr.YesTupleOfArgs n

/// Work out what byref-values are allowed at input positions to named F# functions or members
let mkArgsForAppliedVal isBaseCall (vref: ValRef) argsl =
    match vref.ValReprInfo with
    | Some valReprInfo ->
        let argArities = valReprInfo.AritiesOfArgs

        let argArities =
            if isBaseCall && argArities.Length >= 1 then
                List.tail argArities
            else
                argArities
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
    | Expr.Op (TOp.Coerce, _, [ f ], _) -> mkArgsForAppliedExpr isBaseCall argsl f
    | _ -> []

/// Check an expression, where the expression is in a position where byrefs can be generated
let rec CheckExprNoByrefs cenv env (tailCall: TailCall) expr =
    CheckExpr cenv env expr PermitByRefExpr.No tailCall

/// Check an expression, given information about the position of the expression
and CheckForOverAppliedExceptionRaisingPrimitive (cenv: cenv) (env: env) expr (tailCall: TailCall) =
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
                        match tailCall with
                        | TailCall.No -> // an upper level has already decided that this is not in a tailcall position
                            false
                        | TailCall.Yes returnType ->
                            if vref.IsMemberOrModuleBinding && vref.ValReprInfo.IsSome then
                                let topValInfo = vref.ValReprInfo.Value

                                let (nowArgs, laterArgs), returnTy =
                                    let _tps, tau = destTopForallTy g topValInfo _fty

                                    let curriedArgInfos, returnTy =
                                        GetTopTauTypeInFSharpForm cenv.g topValInfo.ArgInfos tau m

                                    if argsl.Length >= curriedArgInfos.Length then
                                        (List.splitAfter curriedArgInfos.Length argsl), returnTy
                                    else
                                        ([], argsl), returnTy

                                let _, _, isNewObj, isSuperInit, isSelfInit, _, _, _ =
                                    GetMemberCallInfo cenv.g (vref, valUseFlags)

                                let isCCall =
                                    match valUseFlags with
                                    | PossibleConstrainedCall _ -> true
                                    | _ -> false

                                let hasByrefArg = nowArgs |> List.exists (tyOfExpr cenv.g >> isByrefTy cenv.g)

                                let mustGenerateUnitAfterCall =
                                    (isUnitTy g returnTy && returnType <> TailCallReturnType.MustReturnVoid)

                                let noTailCallBlockers =
                                    not isNewObj
                                    && not isSuperInit
                                    && not isSelfInit
                                    && not mustGenerateUnitAfterCall
                                    && isNil laterArgs
                                    && not (IsValRefIsDllImport cenv.g vref)
                                    && not isCCall
                                    && not hasByrefArg

                                noTailCallBlockers // blockers that will prevent the IL level from emmiting a tail instruction
                            else
                                true

                    // warn if we call inside of recursive scope in non-tail-call manner/with tail blockers. See
                    // ``Warn successfully in match clause``
                    // ``Warn for byref parameters``
                    if not canTailCall then
                        warning (Error(FSComp.SR.chkNotTailRecursive vref.DisplayName, m))
                | _ -> ()
    | _ -> ()

/// Check call arguments, including the return argument.
and CheckCall cenv env _m _returnTy args ctxts _ctxt =
    CheckExprs cenv env args ctxts TailCall.No

/// Check call arguments, including the return argument. The receiver argument is handled differently.
and CheckCallWithReceiver cenv env _m _returnTy args ctxts _ctxt =
    match args with
    | [] -> failwith "CheckCallWithReceiver: Argument list is empty."
    | receiverArg :: args ->

        let receiverContext, ctxts =
            match ctxts with
            | [] -> PermitByRefExpr.No, []
            | ctxt :: ctxts -> ctxt, ctxts

        CheckExpr cenv env receiverArg receiverContext TailCall.No
        CheckExprs cenv env args ctxts (TailCall.Yes TailCallReturnType.NonVoid)

and CheckExprLinear (cenv: cenv) (env: env) expr (ctxt: PermitByRefExpr) (tailCall: TailCall) : unit =
    match expr with
    | Expr.Sequential (e1, e2, NormalSeq, _) ->
        CheckExprNoByrefs cenv env TailCall.No e1
        // tailcall
        CheckExprLinear cenv env e2 ctxt tailCall

    | Expr.Let (TBind (v, _bindRhs, _) as bind, body, _, _) ->
        let isByRef = isByrefTy cenv.g v.Type

        let bindingContext =
            if isByRef then
                PermitByRefExpr.YesReturnable
            else
                PermitByRefExpr.Yes

        CheckBinding cenv env false bindingContext bind
        // tailcall
        CheckExprLinear cenv env body ctxt tailCall

    | LinearOpExpr (_op, _tyargs, argsHead, argLast, _m) ->
        argsHead |> List.iter (CheckExprNoByrefs cenv env tailCall)
        // tailcall
        CheckExprLinear cenv env argLast PermitByRefExpr.No tailCall

    | LinearMatchExpr (_spMatch, _exprm, dtree, tg1, e2, _m, _ty) ->
        CheckDecisionTree cenv env dtree
        CheckDecisionTreeTarget cenv env tailCall ctxt tg1
        // tailcall
        CheckExprLinear cenv env e2 ctxt tailCall

    | Expr.DebugPoint (_, innerExpr) -> CheckExprLinear cenv env innerExpr ctxt tailCall

    | _ ->
        // not a linear expression
        CheckExpr cenv env expr ctxt (TailCall.YesFromExpr cenv.g expr)

/// Check an expression, given information about the position of the expression
and CheckExpr (cenv: cenv) (env: env) origExpr (ctxt: PermitByRefExpr) (tailCall: TailCall) : unit =

    // Guard the stack for deeply nested expressions
    cenv.stackGuard.Guard
    <| fun () ->

        let g = cenv.g

        let origExpr = stripExpr origExpr

        // CheckForOverAppliedExceptionRaisingPrimitive is more easily checked prior to NormalizeAndAdjustPossibleSubsumptionExprs
        CheckForOverAppliedExceptionRaisingPrimitive cenv env origExpr tailCall
        let expr = NormalizeAndAdjustPossibleSubsumptionExprs g origExpr
        let expr = stripExpr expr

        match expr with
        | LinearOpExpr _
        | LinearMatchExpr _
        | Expr.Let _
        | Expr.Sequential (_, _, NormalSeq, _)
        | Expr.DebugPoint _ -> CheckExprLinear cenv env expr ctxt tailCall

        | Expr.Sequential (e1, e2, ThenDoSeq, _) ->
            CheckExprNoByrefs cenv env TailCall.No e1
            CheckExprNoByrefs cenv env TailCall.No e2

        | Expr.Const _
        | Expr.Val _
        | Expr.Quote _ -> ()

        | StructStateMachineExpr g info -> CheckStructStateMachineExpr cenv env expr info

        | Expr.Obj (_, ty, basev, superInitCall, overrides, iimpls, m) ->
            CheckObjectExpr cenv env (ty, basev, superInitCall, overrides, iimpls, m)

        // Allow base calls to F# methods
        | Expr.App (InnerExprPat (ExprValWithPossibleTypeInst (v, vFlags, _, _) as f), _fty, tyargs, Expr.Val (baseVal, _, _) :: rest, m) when
            ((match vFlags with
              | VSlotDirectCall -> true
              | _ -> false)
             && baseVal.IsBaseVal)
            ->

            CheckFSharpBaseCall cenv env expr (v, f, _fty, tyargs, baseVal, rest, m)

        // Allow base calls to IL methods
        | Expr.Op (TOp.ILCall (isVirtual, _, _, _, _, _, _, ilMethRef, enclTypeInst, methInst, retTypes),
                   tyargs,
                   Expr.Val (baseVal, _, _) :: rest,
                   m) when not isVirtual && baseVal.IsBaseVal ->

            CheckILBaseCall cenv env (ilMethRef, enclTypeInst, methInst, retTypes, tyargs, baseVal, rest, m)

        | Expr.Op (op, tyargs, args, m) -> CheckExprOp cenv env (op, tyargs, args, m) ctxt expr

        // Allow 'typeof<System.Void>' calls as a special case, the only accepted use of System.Void!
        | TypeOfExpr g ty when isVoidTy g ty -> ()

        // Allow 'typedefof<System.Void>' calls as a special case, the only accepted use of System.Void!
        | TypeDefOfExpr g ty when isVoidTy g ty -> ()

        // Check an application
        | Expr.App (f, _fty, tyargs, argsl, m) -> CheckApplication cenv env expr (f, tyargs, argsl, m) ctxt tailCall

        | Expr.Lambda (_, _, _, argvs, _, m, bodyTy) -> CheckLambda cenv env expr (argvs, m, bodyTy) tailCall

        | Expr.TyLambda (_, tps, _, m, bodyTy) -> CheckTyLambda cenv env expr (tps, m, bodyTy) tailCall

        | Expr.TyChoose (_tps, e1, _) -> CheckExprNoByrefs cenv env tailCall e1

        | Expr.Match (_, _, dtree, targets, m, ty) -> CheckMatch cenv env ctxt (dtree, targets, m, ty) tailCall

        | Expr.LetRec (binds, bodyExpr, _, _) -> CheckLetRec cenv env (binds, bodyExpr) tailCall

        | Expr.StaticOptimization (constraints, e2, e3, m) -> CheckStaticOptimization cenv env (constraints, e2, e3, m)

        | Expr.WitnessArg _ -> ()

        | Expr.Link _ -> failwith "Unexpected reclink"

and CheckStructStateMachineExpr cenv env _expr info =

    let (_dataTy,
         (_moveNextThisVar, moveNextExpr),
         (_setStateMachineThisVar, _setStateMachineStateVar, setStateMachineBody),
         (_afterCodeThisVar, afterCodeBody)) =
        info

    CheckExprNoByrefs cenv env TailCall.No moveNextExpr
    CheckExprNoByrefs cenv env TailCall.No setStateMachineBody
    CheckExprNoByrefs cenv env TailCall.No afterCodeBody

and CheckObjectExpr cenv env (ty, basev, superInitCall, overrides, iimpls, _m) =
    CheckExprNoByrefs cenv env TailCall.No superInitCall
    CheckMethods cenv env basev (ty, overrides)
    CheckInterfaceImpls cenv env basev iimpls

and CheckFSharpBaseCall cenv env _expr (v, f, _fty, _tyargs, _baseVal, rest, _m) : unit =
    let memberInfo = Option.get v.MemberInfo

    if memberInfo.MemberFlags.IsDispatchSlot then
        ()
    else
        CheckExprs cenv env rest (mkArgsForAppliedExpr true rest f) TailCall.No

and CheckILBaseCall cenv env (_ilMethRef, _enclTypeInst, _methInst, _retTypes, _tyargs, _baseVal, rest, _m) : unit =
    CheckExprsPermitByRefLike cenv env rest

and CheckApplication cenv env expr (f, _tyargs, argsl, m) ctxt (tailCall: TailCall) : unit =
    let g = cenv.g

    let returnTy = tyOfExpr g expr
    CheckExprNoByrefs cenv env tailCall f

    let hasReceiver =
        match f with
        | Expr.Val (vref, _, _) when vref.IsInstanceMember && not argsl.IsEmpty -> true
        | _ -> false

    let ctxts = mkArgsForAppliedExpr false argsl f

    if hasReceiver then
        CheckCallWithReceiver cenv env m returnTy argsl ctxts ctxt
    else
        CheckCall cenv env m returnTy argsl ctxts ctxt

and CheckLambda cenv env expr (argvs, m, bodyTy) (tailCall: TailCall) =
    let valReprInfo =
        ValReprInfo([], [ argvs |> List.map (fun _ -> ValReprInfo.unnamedTopArg1) ], ValReprInfo.unnamedRetVal)

    let ty = mkMultiLambdaTy cenv.g m argvs bodyTy in
    CheckLambdas false None cenv env false valReprInfo tailCall.AtExprLambda false expr m ty PermitByRefExpr.Yes

and CheckTyLambda cenv env expr (tps, m, bodyTy) (tailCall: TailCall) =
    let valReprInfo =
        ValReprInfo(ValReprInfo.InferTyparInfo tps, [], ValReprInfo.unnamedRetVal)

    let ty = mkForallTyIfNeeded tps bodyTy in
    CheckLambdas false None cenv env false valReprInfo tailCall.AtExprLambda false expr m ty PermitByRefExpr.Yes

and CheckMatch cenv env ctxt (dtree, targets, _m, _ty) tailCall =
    CheckDecisionTree cenv env dtree
    CheckDecisionTreeTargets cenv env targets ctxt tailCall

and CheckLetRec cenv env (binds, bodyExpr) tailCall =
    CheckBindings cenv env binds
    CheckExprNoByrefs cenv env tailCall bodyExpr

and CheckStaticOptimization cenv env (_constraints, e2, e3, _m) =
    CheckExprNoByrefs cenv env TailCall.No e2
    CheckExprNoByrefs cenv env TailCall.No e3

and CheckMethods cenv env baseValOpt (ty, methods) =
    methods |> List.iter (CheckMethod cenv env baseValOpt ty)

and CheckMethod cenv env _baseValOpt _ty (TObjExprMethod (_, _, _tps, _vs, body, _m)) =
    CheckExpr cenv env body PermitByRefExpr.YesReturnableNonLocal TailCall.No

and CheckInterfaceImpls cenv env baseValOpt l =
    l |> List.iter (CheckInterfaceImpl cenv env baseValOpt)

and CheckInterfaceImpl cenv env baseValOpt overrides =
    CheckMethods cenv env baseValOpt overrides

and CheckExprOp cenv env (op, tyargs, args, m) ctxt expr : unit =
    let g = cenv.g

    // Special cases
    match op, tyargs, args with
    // Handle these as special cases since mutables are allowed inside their bodies
    | TOp.While _, _, [ Expr.Lambda (_, _, _, [ _ ], e1, _, _); Expr.Lambda (_, _, _, [ _ ], e2, _, _) ] ->
        CheckExprsNoByRefLike cenv env [ e1; e2 ]

    | TOp.TryFinally _, [ _ ], [ Expr.Lambda (_, _, _, [ _ ], e1, _, _); Expr.Lambda (_, _, _, [ _ ], e2, _, _) ] ->
        CheckExpr cenv env e1 ctxt TailCall.No // result of a try/finally can be a byref if in a position where the overall expression is can be a byref
        CheckExprNoByrefs cenv env TailCall.No e2

    | TOp.IntegerForLoop _,
      _,
      [ Expr.Lambda (_, _, _, [ _ ], e1, _, _); Expr.Lambda (_, _, _, [ _ ], e2, _, _); Expr.Lambda (_, _, _, [ _ ], e3, _, _) ] ->
        CheckExprsNoByRefLike cenv env [ e1; e2; e3 ]

    | TOp.TryWith _,
      [ _ ],
      [ Expr.Lambda (_, _, _, [ _ ], e1, _, _); Expr.Lambda (_, _, _, [ _ ], _e2, _, _); Expr.Lambda (_, _, _, [ _ ], e3, _, _) ] ->
        CheckExpr cenv env e1 ctxt TailCall.No // result of a try/catch can be a byref if in a position where the overall expression is can be a byref
        // [(* e2; -- don't check filter body - duplicates logic in 'catch' body *) e3]
        CheckExpr cenv env e3 ctxt TailCall.No // result of a try/catch can be a byref if in a position where the overall expression is can be a byref

    | TOp.ILCall (_, _, _, _, _, _, _, ilMethRef, _enclTypeInst, _methInst, retTypes), _, _ ->

        let hasReceiver =
            (ilMethRef.CallingConv.IsInstance || ilMethRef.CallingConv.IsInstanceExplicit)
            && not args.IsEmpty

        let returnTy = tyOfExpr g expr

        let argContexts = List.init args.Length (fun _ -> PermitByRefExpr.Yes)

        match retTypes with
        | [ ty ] when ctxt.PermitOnlyReturnable && isByrefLikeTy g m ty ->
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
        | _ -> CheckExprsNoByRefLike cenv env args

    | TOp.LValueOp (LAddrOf _, _vref), _, _ -> CheckExprsNoByRefLike cenv env args

    | TOp.LValueOp (LByrefSet, _vref), _, [ _arg ] -> ()

    | TOp.LValueOp (LByrefGet, _vref), _, [] -> ()

    | TOp.LValueOp (LSet, _vref), _, [ _arg ] -> ()

    | TOp.AnonRecdGet _, _, [ arg1 ]
    | TOp.TupleFieldGet _, _, [ arg1 ] -> CheckExprsPermitByRefLike cenv env [ arg1 ]

    | TOp.ValFieldGet _rf, _, [ arg1 ] -> CheckExprsPermitByRefLike cenv env [ arg1 ]

    | TOp.ValFieldSet _rf, _, [ _arg1; _arg2 ] -> ()

    | TOp.Coerce, [ tgtTy; srcTy ], [ x ] ->
        let tailCall = TailCall.YesFromExpr cenv.g x

        if TypeDefinitelySubsumesTypeNoCoercion 0 g cenv.amap m tgtTy srcTy then
            CheckExpr cenv env x ctxt tailCall
        else
            CheckExprNoByrefs cenv env tailCall x

    | TOp.Reraise, [ _ty1 ], [] -> ()

    // Check get of static field
    | TOp.ValFieldGetAddr (_rfref, _readonly), _tyargs, [] -> ()

    // Check get of instance field
    | TOp.ValFieldGetAddr (_rfref, _readonly), _tyargs, [ obj ] ->
        // Recursively check in same ctxt, e.g. if at PermitOnlyReturnable the obj arg must also be returnable
        CheckExpr cenv env obj ctxt TailCall.No

    | TOp.UnionCaseFieldGet _, _, [ arg1 ] -> CheckExprPermitByRefLike cenv env arg1

    | TOp.UnionCaseTagGet _, _, [ arg1 ] -> CheckExprPermitByRefLike cenv env arg1 // allow byref - it may be address-of-struct

    | TOp.UnionCaseFieldGetAddr (_uref, _idx, _readonly), _tyargs, [ obj ] ->
        // Recursively check in same ctxt, e.g. if at PermitOnlyReturnable the obj arg must also be returnable
        CheckExpr cenv env obj ctxt TailCall.No

    | TOp.ILAsm (instrs, _retTypes), _, _ ->
        match instrs, args with
        // Write a .NET instance field
        | [ I_stfld (_alignment, _vol, _fspec) ], _ ->
            match args with
            | [ _; rhs ] -> CheckExprNoByrefs cenv env TailCall.No rhs
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

        | [ I_ldsflda _fspec ], [] -> ()

        | [ I_ldflda _fspec ], [ obj ] ->

            // Recursively check in same ctxt, e.g. if at PermitOnlyReturnable the obj arg must also be returnable
            CheckExpr cenv env obj ctxt TailCall.No

        | [ I_ldelema (_, _isNativePtr, _, _) ], lhsArray :: indices ->
            // permit byref for lhs lvalue
            CheckExprPermitByRefLike cenv env lhsArray
            CheckExprsNoByRefLike cenv env indices

        | [ AI_conv _ ], _ ->
            // permit byref for args to conv
            CheckExprsPermitByRefLike cenv env args

        | _ -> CheckExprsNoByRefLike cenv env args

    | TOp.TraitCall _, _, _ ->
        // CheckTypeInstNoByrefs cenv env m tyargs
        // allow args to be byref here
        CheckExprsPermitByRefLike cenv env args

    | TOp.Recd _, _, _ -> CheckExprsPermitByRefLike cenv env args

    | _ -> CheckExprsNoByRefLike cenv env args

and CheckLambdas
    isTop
    (memberVal: Val option)
    cenv
    env
    inlined
    valReprInfo
    (tailCall: TailCall)
    alwaysCheckNoReraise
    expr
    mOrig
    ety
    ctxt
    : unit =
    let g = cenv.g
    let memInfo = memberVal |> Option.bind (fun v -> v.MemberInfo)

    // The valReprInfo here says we are _guaranteeing_ to compile a function value
    // as a .NET method with precisely the corresponding argument counts.
    match stripDebugPoints expr with
    | Expr.TyChoose (_tps, e1, m) -> CheckLambdas isTop memberVal cenv env inlined valReprInfo tailCall alwaysCheckNoReraise e1 m ety ctxt

    | Expr.Lambda (_, _, _, _, _, m, _)
    | Expr.TyLambda (_, _, _, m, _) ->
        let _tps, ctorThisValOpt, baseValOpt, vsl, body, bodyTy =
            destLambdaWithValReprInfo g cenv.amap valReprInfo (expr, ety)

        let thisAndBase = Option.toList ctorThisValOpt @ Option.toList baseValOpt
        let restArgs = List.concat vsl

        match memInfo with
        | None -> ()
        | Some mi ->
            // ctorThis and baseVal values are always considered used
            for v in thisAndBase do
                v.SetHasBeenReferenced()
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
            CheckExprPermitReturnableByRef cenv env body
        else
            CheckExprNoByrefs cenv env tailCall body

    // This path is for expression bindings that are not actually lambdas
    | _ ->
        let m = mOrig

        if not inlined && (isByrefLikeTy g m ety || isNativePtrTy g ety) then
            // allow byref to occur as RHS of byref binding.
            CheckExpr cenv env expr ctxt tailCall
        else
            CheckExprNoByrefs cenv env tailCall expr

and CheckExprs cenv env exprs ctxts tailCall : unit =
    let ctxts = Array.ofList ctxts

    let argArity i =
        if i < ctxts.Length then ctxts[i] else PermitByRefExpr.No

    exprs
    |> List.mapi (fun i exp -> CheckExpr cenv env exp (argArity i) tailCall)
    |> ignore

and CheckExprsNoByRefLike cenv env exprs : unit =
    for expr in exprs do
        CheckExprNoByrefs cenv env TailCall.No expr

and CheckExprsPermitByRefLike cenv env exprs : unit =
    exprs |> List.map (CheckExprPermitByRefLike cenv env) |> ignore

and CheckExprPermitByRefLike cenv env expr : unit =
    CheckExpr cenv env expr PermitByRefExpr.Yes TailCall.No

and CheckExprPermitReturnableByRef cenv env expr : unit =
    CheckExpr cenv env expr PermitByRefExpr.YesReturnable TailCall.No

and CheckDecisionTreeTargets cenv env targets ctxt (tailCall: TailCall) =
    targets
    |> Array.map (CheckDecisionTreeTarget cenv env tailCall ctxt)
    |> List.ofArray
    |> ignore

and CheckDecisionTreeTarget cenv env (tailCall: TailCall) ctxt (TTarget (_vs, targetExpr, _)) : unit =
    CheckExpr cenv env targetExpr ctxt tailCall

and CheckDecisionTree cenv env dtree =
    match dtree with
    | TDSuccess (resultExprs, _) -> CheckExprsNoByRefLike cenv env resultExprs
    | TDBind (bind, rest) ->
        CheckBinding cenv env false PermitByRefExpr.Yes bind
        CheckDecisionTree cenv env rest
    | TDSwitch (inpExpr, cases, dflt, m) -> CheckDecisionTreeSwitch cenv env (inpExpr, cases, dflt, m)

and CheckDecisionTreeSwitch cenv env (inpExpr, cases, dflt, m) =
    CheckExprPermitByRefLike cenv env inpExpr // can be byref for struct union switch

    for TCase (discrim, dtree) in cases do
        CheckDecisionTreeTest cenv env m discrim
        CheckDecisionTree cenv env dtree

    dflt |> Option.iter (CheckDecisionTree cenv env)

and CheckDecisionTreeTest cenv env _m discrim =
    match discrim with
    | DecisionTreeTest.ActivePatternCase (exp, _, _, _, _, _) -> CheckExprNoByrefs cenv env TailCall.No exp
    | _ -> ()

and CheckBinding cenv env alwaysCheckNoReraise ctxt (TBind (v, bindRhs, _) as bind) : unit =
    let g = cenv.g
    let isTop = Option.isSome bind.Var.ValReprInfo
    let tailCall = TailCall.YesFromVal g bind.Var

    let valReprInfo =
        match bind.Var.ValReprInfo with
        | Some info -> info
        | _ -> ValReprInfo.emptyValData

    CheckLambdas isTop (Some v) cenv env v.MustInline valReprInfo tailCall alwaysCheckNoReraise bindRhs v.Range v.Type ctxt

and CheckBindings cenv env binds =
    for bind in binds do
        CheckBinding cenv env false PermitByRefExpr.Yes bind

// Top binds introduce expression, check they are reraise free.
let CheckModuleBinding cenv env (isRec: bool) (TBind (_v, _e, _) as bind) =
    // Check that a let binding to the result of a rec expression is not inside the rec expression
    // see test ``Warn for invalid tailcalls in seq expression because of bind`` for an example
    // see test ``Warn successfully for rec call in binding`` for an example
    if cenv.g.langVersion.SupportsFeature LanguageFeature.WarningWhenTailRecAttributeButNonTailRecUsage then
        match bind.Expr with
        | Expr.TyLambda (bodyExpr = bodyExpr)
        | Expr.Lambda (bodyExpr = bodyExpr) ->
            let rec checkTailCall (insideSubBinding: bool) expr =
                match expr with
                | Expr.Val (valRef = valRef; range = m) ->
                    if isRec && insideSubBinding && env.mustTailCall.Contains valRef.Deref then
                        warning (Error(FSComp.SR.chkNotTailRecursive valRef.DisplayName, m))
                | Expr.App (funcExpr = funcExpr; args = argExprs) ->
                    checkTailCall insideSubBinding funcExpr
                    argExprs |> List.iter (checkTailCall insideSubBinding)
                | Expr.Link exprRef -> checkTailCall insideSubBinding exprRef.Value
                | Expr.Lambda (bodyExpr = bodyExpr) -> checkTailCall insideSubBinding bodyExpr
                | Expr.DebugPoint (_debugPointAtLeafExpr, expr) -> checkTailCall insideSubBinding expr
                | Expr.Let (binding = binding; bodyExpr = bodyExpr) ->
                    checkTailCall true binding.Expr

                    let warnForBodyExpr =
                        match stripDebugPoints bodyExpr with
                        | Expr.Op _ -> true // ToDo: too crude of a check?
                        | _ -> false

                    checkTailCall warnForBodyExpr bodyExpr
                | Expr.Match (targets = decisionTreeTargets) ->
                    decisionTreeTargets
                    |> Array.iter (fun target -> checkTailCall insideSubBinding target.TargetExpression)
                | Expr.Op (args = exprs) -> exprs |> Seq.iter (checkTailCall insideSubBinding)
                | _ -> ()

            checkTailCall false bodyExpr
        | _ -> ()

    CheckBinding cenv env true PermitByRefExpr.Yes bind

//--------------------------------------------------------------------------
// check modules
//--------------------------------------------------------------------------

let rec CheckDefnsInModule cenv env mdefs =
    for mdef in mdefs do
        CheckDefnInModule cenv env mdef

and CheckDefnInModule cenv env mdef =
    match mdef with
    | TMDefRec (isRec, _opens, _tycons, mspecs, _m) ->
        let env =
            if isRec then
                let vals = allValsOfModDef mdef

                let mustTailCall =
                    Seq.fold
                        (fun mustTailCall (v: Val) ->
                            if HasFSharpAttribute cenv.g cenv.g.attrib_TailCallAttribute v.Attribs then
                                let newSet = Zset.add v mustTailCall
                                newSet
                            else
                                mustTailCall)
                        env.mustTailCall
                        vals

                { env with mustTailCall = mustTailCall }
            else
                env

        List.iter (CheckModuleSpec cenv env isRec) mspecs
    | TMDefLet (bind, _m) -> CheckModuleBinding cenv env false bind
    | TMDefOpens _ -> ()
    | TMDefDo (e, _m) ->
        let tailCall =
            match stripDebugPoints e with
            | Expr.App (funcExpr = funcExpr) ->
                match funcExpr with
                | ValUseAtApp (vref, _valUseFlags) -> TailCall.YesFromVal cenv.g vref.Deref
                | _ -> TailCall.No
            | _ -> TailCall.No

        CheckExprNoByrefs cenv env tailCall e
    | TMDefs defs -> CheckDefnsInModule cenv env defs

and CheckModuleSpec cenv env isRec mbind =
    match mbind with
    | ModuleOrNamespaceBinding.Binding bind ->
        let env =
            if env.mustTailCall.Contains bind.Var then
                env
            else
                { env with
                    mustTailCall = Zset.empty valOrder
                }

        CheckModuleBinding cenv env isRec bind
    | ModuleOrNamespaceBinding.Module (_mspec, rhs) -> CheckDefnInModule cenv env rhs

let CheckImplFile (g, amap, reportErrors, implFileContents) =
    let cenv =
        {
            g = g
            reportErrors = reportErrors
            stackGuard = StackGuard(PostInferenceChecksStackGuardDepth, "CheckImplFile")
            amap = amap
        }

    let env = { mustTailCall = Zset.empty valOrder }

    CheckDefnInModule cenv env implFileContents
