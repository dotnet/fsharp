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

[<return: Struct>]
let (|ValUseAtApp|_|) e =
    match e with
    | InnerExprPat(Expr.App(funcExpr = InnerExprPat(Expr.Val(valRef = vref; flags = valUseFlags))) | Expr.Val(
        valRef = vref; flags = valUseFlags)) -> ValueSome(vref, valUseFlags)
    | _ -> ValueNone

type TailCallReturnType =
    | MustReturnVoid // indicates "has unit return type and must return void"
    | NonVoid

type TailCall =
    | Yes of TailCallReturnType
    | No

    static member private IsVoidRet (g: TcGlobals) (v: Val) =
        match v.ValReprInfo with
        | Some info ->
            let _, _, returnTy, _ = GetValReprTypeInFSharpForm g info v.Type v.Range

            if isUnitTy g returnTy then
                TailCallReturnType.MustReturnVoid
            else
                TailCallReturnType.NonVoid
        | None -> TailCallReturnType.NonVoid

    static member YesFromVal (g: TcGlobals) (v: Val) = TailCall.Yes(TailCall.IsVoidRet g v)

    static member YesFromExpr (g: TcGlobals) (expr: Expr) =
        let yesFromTType (t: TType) =
            if isUnitTy g t then
                TailCall.Yes TailCallReturnType.MustReturnVoid
            else
                TailCall.Yes TailCallReturnType.NonVoid

        match expr with
        | ValUseAtApp(valRef, _) -> TailCall.Yes(TailCall.IsVoidRet g valRef.Deref)
        | Expr.Const(constType = constType) -> yesFromTType constType
        | Expr.Match(exprType = exprType) -> yesFromTType exprType
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

        /// Values in module that have been marked [<TailCall>]
        mustTailCall: Zset<Val>
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
    | Expr.Val(vref, _, _) -> mkArgsForAppliedVal isBaseCall vref argsl
    // step through instantiations
    | Expr.App(f, _fty, _tyargs, [], _) -> mkArgsForAppliedExpr isBaseCall argsl f
    // step through subsumption coercions
    | Expr.Op(TOp.Coerce, _, [ f ], _) -> mkArgsForAppliedExpr isBaseCall argsl f
    | _ -> []

/// Check an expression, warn if it's attributed with TailCall but our analysis concludes it's not a valid tail call
let CheckForNonTailRecCall (cenv: cenv) expr (tailCall: TailCall) =
    let g = cenv.g
    let expr = stripExpr expr
    let expr = stripDebugPoints expr

    match expr with
    | Expr.App(f, _fty, _tyargs, argsl, m) ->

        match f with
        | ValUseAtApp(vref, valUseFlags) when cenv.mustTailCall.Contains vref.Deref ->

            let canTailCall =
                match tailCall with
                | TailCall.No -> // an upper level has already decided that this is not in a tailcall position
                    false
                | TailCall.Yes returnType ->
                    if vref.IsMemberOrModuleBinding && vref.ValReprInfo.IsSome then
                        let topValInfo = vref.ValReprInfo.Value

                        let nowArgs, laterArgs =
                            let _, curriedArgInfos, _, _ =
                                GetValReprTypeInFSharpForm cenv.g topValInfo vref.Type m

                            if argsl.Length >= curriedArgInfos.Length then
                                (List.splitAfter curriedArgInfos.Length argsl)
                            else
                                ([], argsl)

                        let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal vref.Deref

                        let _, _, _, returnTy, _ =
                            GetValReprTypeInCompiledForm g topValInfo numEnclosingTypars vref.Type m

                        let _, _, isNewObj, isSuperInit, isSelfInit, _, _, _ =
                            GetMemberCallInfo cenv.g (vref, valUseFlags)

                        let isCCall =
                            match valUseFlags with
                            | PossibleConstrainedCall _ -> true
                            | _ -> false

                        let hasByrefArg = nowArgs |> List.exists (tyOfExpr cenv.g >> isByrefTy cenv.g)

                        let mustGenerateUnitAfterCall =
                            (Option.isNone returnTy && returnType <> TailCallReturnType.MustReturnVoid)

                        let noTailCallBlockers =
                            not isNewObj
                            && not isSuperInit
                            && not isSelfInit
                            && not mustGenerateUnitAfterCall
                            && isNil laterArgs
                            && not (IsValRefIsDllImport cenv.g vref)
                            && not isCCall
                            && not hasByrefArg

                        noTailCallBlockers // blockers that will prevent the IL level from emitting a tail instruction
                    else
                        true

            // warn if we call inside of recursive scope in non-tail-call manner/with tail blockers. See
            // ``Warn successfully in match clause``
            // ``Warn for byref parameters``
            if not canTailCall then
                warning (Error(FSComp.SR.chkNotTailRecursive vref.DisplayName, m))
        | _ -> ()
    | _ -> ()

/// Check an expression, where the expression is in a position where byrefs can be generated
let rec CheckExprNoByrefs cenv (tailCall: TailCall) expr =
    CheckExpr cenv expr PermitByRefExpr.No tailCall

/// Check call arguments, including the return argument.
and CheckCall cenv args ctxts (tailCall: TailCall) =
    // detect CPS-like expressions
    let rec (|IsAppInLambdaBody|_|) e =
        match stripDebugPoints e with
        | Expr.TyLambda(bodyExpr = bodyExpr)
        | Expr.Lambda(bodyExpr = bodyExpr) ->
            match (stripDebugPoints bodyExpr) with
            | Expr.App _ -> Some(TailCall.YesFromExpr cenv.g e)
            | IsAppInLambdaBody t -> Some t
            | _ -> None
        | Expr.App(args = args) ->
            args
            |> List.tryPick (fun a ->
                match a with
                | IsAppInLambdaBody t -> Some t
                | _ -> None)

        | _ -> None

    // if we haven't already decided this is no tail call, try to detect CPS-like expressions
    let tailCall =
        if tailCall = TailCall.No then
            tailCall
        else
            args
            |> List.tryPick (fun a ->
                match a with
                | IsAppInLambdaBody t -> Some t
                | _ -> None)
            |> Option.defaultValue TailCall.No

    CheckExprs cenv args ctxts tailCall

/// Check call arguments, including the return argument. The receiver argument is handled differently.
and CheckCallWithReceiver cenv args ctxts =
    match args with
    | [] -> failwith "CheckCallWithReceiver: Argument list is empty."
    | receiverArg :: args ->

        let receiverContext, ctxts =
            match ctxts with
            | [] -> PermitByRefExpr.No, []
            | ctxt :: ctxts -> ctxt, ctxts

        CheckExpr cenv receiverArg receiverContext TailCall.No
        CheckExprs cenv args ctxts (TailCall.Yes TailCallReturnType.NonVoid)

and CheckExprLinear (cenv: cenv) expr (ctxt: PermitByRefExpr) (tailCall: TailCall) : unit =
    match expr with
    | Expr.Sequential(e1, e2, NormalSeq, _) ->
        CheckExprNoByrefs cenv TailCall.No e1
        // tailcall
        CheckExprLinear cenv e2 ctxt tailCall

    | Expr.Let(TBind(v, _bindRhs, _) as bind, body, _, _) ->
        let isByRef = isByrefTy cenv.g v.Type

        let bindingContext =
            if isByRef then
                PermitByRefExpr.YesReturnable
            else
                PermitByRefExpr.Yes

        CheckBinding cenv false bindingContext bind
        // tailcall
        CheckExprLinear cenv body ctxt tailCall

    | LinearOpExpr(_op, _tyargs, argsHead, argLast, _m) ->
        argsHead |> List.iter (CheckExprNoByrefs cenv tailCall)
        // tailcall
        CheckExprLinear cenv argLast PermitByRefExpr.No tailCall

    | LinearMatchExpr(_spMatch, _exprm, dtree, tg1, e2, _m, _ty) ->
        CheckDecisionTree cenv dtree
        CheckDecisionTreeTarget cenv tailCall ctxt tg1
        // tailcall
        CheckExprLinear cenv e2 ctxt tailCall

    | Expr.DebugPoint(_, innerExpr) -> CheckExprLinear cenv innerExpr ctxt tailCall

    | _ ->
        // not a linear expression
        CheckExpr cenv expr ctxt (TailCall.YesFromExpr cenv.g expr)

/// Check an expression, given information about the position of the expression
and CheckExpr (cenv: cenv) origExpr (ctxt: PermitByRefExpr) (tailCall: TailCall) : unit =

    // Guard the stack for deeply nested expressions
    cenv.stackGuard.Guard
    <| fun () ->

        let g = cenv.g

        let origExpr = stripExpr origExpr

        // CheckForOverAppliedExceptionRaisingPrimitive is more easily checked prior to NormalizeAndAdjustPossibleSubsumptionExprs
        CheckForNonTailRecCall cenv origExpr tailCall
        let expr = NormalizeAndAdjustPossibleSubsumptionExprs g origExpr
        let expr = stripExpr expr

        match expr with
        | LinearOpExpr _
        | LinearMatchExpr _
        | Expr.Let _
        | Expr.Sequential(_, _, NormalSeq, _)
        | Expr.DebugPoint _ -> CheckExprLinear cenv expr ctxt tailCall

        | Expr.Sequential(e1, e2, ThenDoSeq, _) ->
            CheckExprNoByrefs cenv TailCall.No e1
            CheckExprNoByrefs cenv TailCall.No e2

        | Expr.Const _
        | Expr.Val _
        | Expr.Quote _ -> ()

        | StructStateMachineExpr g info -> CheckStructStateMachineExpr cenv info

        | Expr.Obj(_, ty, _basev, superInitCall, overrides, iimpls, _) -> CheckObjectExpr cenv (ty, superInitCall, overrides, iimpls)

        // Allow base calls to F# methods
        | Expr.App(InnerExprPat(ExprValWithPossibleTypeInst(v, vFlags, _, _) as f), _fty, _tyargs, Expr.Val(baseVal, _, _) :: rest, _m) when
            ((match vFlags with
              | VSlotDirectCall -> true
              | _ -> false)
             && baseVal.IsBaseVal)
            ->
            CheckFSharpBaseCall cenv (v, f, rest)

        // Allow base calls to IL methods
        | Expr.Op(TOp.ILCall(isVirtual, _, _, _, _, _, _, _ilMethRef, _enclTypeInst, _methInst, _retTypes),
                  _tyargs,
                  Expr.Val(baseVal, _, _) :: rest,
                  _m) when not isVirtual && baseVal.IsBaseVal ->

            CheckILBaseCall cenv rest

        | Expr.Op(op, tyargs, args, m) -> CheckExprOp cenv (op, tyargs, args, m) ctxt

        // Allow 'typeof<System.Void>' calls as a special case, the only accepted use of System.Void!
        | TypeOfExpr g ty when isVoidTy g ty -> ()

        // Allow 'typedefof<System.Void>' calls as a special case, the only accepted use of System.Void!
        | TypeDefOfExpr g ty when isVoidTy g ty -> ()

        // Check an application
        | Expr.App(f, _fty, _tyargs, argsl, _m) ->
            // detect expressions like List.collect
            let checkArgForLambdaWithAppOfMustTailCall e =
                match stripDebugPoints e with
                | Expr.TyLambda(bodyExpr = bodyExpr)
                | Expr.Lambda(bodyExpr = bodyExpr) ->
                    match bodyExpr with
                    | Expr.App(ValUseAtApp(vref, _valUseFlags), _formalType, _typeArgs, _exprs, _range) ->
                        cenv.mustTailCall.Contains vref.Deref
                    | _ -> false
                | _ -> false

            let tailCall =
                if argsl |> List.exists checkArgForLambdaWithAppOfMustTailCall then
                    TailCall.No
                else
                    tailCall

            CheckApplication cenv (f, argsl) tailCall

        | Expr.Lambda(_, _, _, argvs, _, m, bodyTy) -> CheckLambda cenv expr (argvs, m, bodyTy) tailCall

        | Expr.TyLambda(_, tps, _, m, bodyTy) -> CheckTyLambda cenv expr (tps, m, bodyTy) tailCall

        | Expr.TyChoose(_tps, e1, _) -> CheckExprNoByrefs cenv tailCall e1

        | Expr.Match(_, _, dtree, targets, _m, _ty) -> CheckMatch cenv ctxt (dtree, targets) tailCall

        | Expr.LetRec(binds, bodyExpr, _, _) -> CheckLetRec cenv (binds, bodyExpr) tailCall

        | Expr.StaticOptimization(_constraints, e2, e3, _m) -> CheckStaticOptimization cenv (e2, e3)

        | Expr.WitnessArg _ -> ()

        | Expr.Link _ -> failwith "Unexpected reclink"

and CheckStructStateMachineExpr cenv info =

    let (_dataTy,
         (_moveNextThisVar, moveNextExpr),
         (_setStateMachineThisVar, _setStateMachineStateVar, setStateMachineBody),
         (_afterCodeThisVar, afterCodeBody)) =
        info

    CheckExprNoByrefs cenv TailCall.No moveNextExpr
    CheckExprNoByrefs cenv TailCall.No setStateMachineBody
    CheckExprNoByrefs cenv TailCall.No afterCodeBody

and CheckObjectExpr cenv (ty, superInitCall, overrides, iimpls) =
    CheckExprNoByrefs cenv TailCall.No superInitCall
    CheckMethods cenv (ty, overrides)
    CheckInterfaceImpls cenv iimpls

and CheckFSharpBaseCall cenv (v, f, rest) : unit =
    let memberInfo = Option.get v.MemberInfo

    if memberInfo.MemberFlags.IsDispatchSlot then
        ()
    else
        CheckExprs cenv rest (mkArgsForAppliedExpr true rest f) TailCall.No

and CheckILBaseCall cenv rest : unit = CheckExprsPermitByRefLike cenv rest

and CheckApplication cenv (f, argsl) (tailCall: TailCall) : unit =
    CheckExprNoByrefs cenv tailCall f

    let hasReceiver =
        match f with
        | Expr.Val(vref, _, _) when vref.IsInstanceMember && not argsl.IsEmpty -> true
        | _ -> false

    let ctxts = mkArgsForAppliedExpr false argsl f

    if hasReceiver then
        CheckCallWithReceiver cenv argsl ctxts
    else
        CheckCall cenv argsl ctxts tailCall

and CheckLambda cenv expr (argvs, m, bodyTy) (tailCall: TailCall) =
    let valReprInfo =
        ValReprInfo([], [ argvs |> List.map (fun _ -> ValReprInfo.unnamedTopArg1) ], ValReprInfo.unnamedRetVal)

    let ty = mkMultiLambdaTy cenv.g m argvs bodyTy in
    CheckLambdas false None cenv false valReprInfo tailCall.AtExprLambda false expr m ty PermitByRefExpr.Yes

and CheckTyLambda cenv expr (tps, m, bodyTy) (tailCall: TailCall) =
    let valReprInfo =
        ValReprInfo(ValReprInfo.InferTyparInfo tps, [], ValReprInfo.unnamedRetVal)

    let ty = mkForallTyIfNeeded tps bodyTy in
    CheckLambdas false None cenv false valReprInfo tailCall.AtExprLambda false expr m ty PermitByRefExpr.Yes

and CheckMatch cenv ctxt (dtree, targets) tailCall =
    CheckDecisionTree cenv dtree
    CheckDecisionTreeTargets cenv targets ctxt tailCall

and CheckLetRec cenv (binds, bodyExpr) tailCall =
    CheckBindings cenv binds
    CheckExprNoByrefs cenv tailCall bodyExpr

and CheckStaticOptimization cenv (e2, e3) =
    CheckExprNoByrefs cenv TailCall.No e2
    CheckExprNoByrefs cenv TailCall.No e3

and CheckMethods cenv (ty, methods) =
    methods |> List.iter (CheckMethod cenv ty)

and CheckMethod cenv _ty (TObjExprMethod(_, _, _tps, _vs, body, _m)) =
    let tailCall =
        match stripDebugPoints body with
        | Expr.App _ as a -> TailCall.YesFromExpr cenv.g a
        | _ -> TailCall.No

    CheckExpr cenv body PermitByRefExpr.YesReturnableNonLocal tailCall

and CheckInterfaceImpls cenv l =
    l |> List.iter (CheckInterfaceImpl cenv)

and CheckInterfaceImpl cenv overrides = CheckMethods cenv overrides

and CheckExprOp cenv (op, tyargs, args, m) ctxt : unit =
    let g = cenv.g

    // Special cases
    match op, tyargs, args with
    // Handle these as special cases since mutables are allowed inside their bodies
    | TOp.While _, _, [ Expr.Lambda(_, _, _, [ _ ], e1, _, _); Expr.Lambda(_, _, _, [ _ ], e2, _, _) ] ->
        CheckExprsNoByRefLike cenv [ e1; e2 ]

    | TOp.TryFinally _, [ _ ], [ Expr.Lambda(_, _, _, [ _ ], e1, _, _); Expr.Lambda(_, _, _, [ _ ], e2, _, _) ] ->
        CheckExpr cenv e1 ctxt TailCall.No // result of a try/finally can be a byref if in a position where the overall expression is can be a byref
        CheckExprNoByrefs cenv TailCall.No e2

    | TOp.IntegerForLoop _,
      _,
      [ Expr.Lambda(_, _, _, [ _ ], e1, _, _); Expr.Lambda(_, _, _, [ _ ], e2, _, _); Expr.Lambda(_, _, _, [ _ ], e3, _, _) ] ->
        CheckExprsNoByRefLike cenv [ e1; e2; e3 ]

    | TOp.TryWith _,
      [ _ ],
      [ Expr.Lambda(_, _, _, [ _ ], e1, _, _); Expr.Lambda(_, _, _, [ _ ], _e2, _, _); Expr.Lambda(_, _, _, [ _ ], e3, _, _) ] ->
        CheckExpr cenv e1 ctxt TailCall.No // result of a try/catch can be a byref if in a position where the overall expression is can be a byref
        // [(* e2; -- don't check filter body - duplicates logic in 'catch' body *) e3]
        CheckExpr cenv e3 ctxt TailCall.No // result of a try/catch can be a byref if in a position where the overall expression is can be a byref

    | TOp.ILCall(_, _, _, _, _, _, _, ilMethRef, _enclTypeInst, _methInst, retTypes), _, _ ->

        let hasReceiver =
            (ilMethRef.CallingConv.IsInstance || ilMethRef.CallingConv.IsInstanceExplicit)
            && not args.IsEmpty

        let argContexts = List.init args.Length (fun _ -> PermitByRefExpr.Yes)

        match retTypes with
        | [ ty ] when ctxt.PermitOnlyReturnable && isByrefLikeTy g m ty ->
            if hasReceiver then
                CheckCallWithReceiver cenv args argContexts
            else
                CheckCall cenv args argContexts TailCall.No
        | _ ->
            if hasReceiver then
                CheckCallWithReceiver cenv args argContexts
            else
                CheckCall cenv args argContexts TailCall.No

    | TOp.Tuple tupInfo, _, _ when not (evalTupInfoIsStruct tupInfo) ->
        match ctxt with
        | PermitByRefExpr.YesTupleOfArgs _nArity ->
            // This tuple should not be generated. The known function arity
            // means it just bundles arguments.
            CheckExprsPermitByRefLike cenv args
        | _ -> CheckExprsNoByRefLike cenv args

    | TOp.LValueOp(LAddrOf _, _vref), _, _ -> CheckExprsNoByRefLike cenv args

    | TOp.LValueOp(LByrefSet, _vref), _, [ _arg ] -> ()

    | TOp.LValueOp(LByrefGet, _vref), _, [] -> ()

    | TOp.LValueOp(LSet, _vref), _, [ _arg ] -> ()

    | TOp.AnonRecdGet _, _, [ arg1 ]
    | TOp.TupleFieldGet _, _, [ arg1 ] -> CheckExprsPermitByRefLike cenv [ arg1 ]

    | TOp.ValFieldGet _rf, _, [ arg1 ] -> CheckExprsPermitByRefLike cenv [ arg1 ]

    | TOp.ValFieldSet _rf, _, [ _arg1; _arg2 ] -> ()

    | TOp.Coerce, [ tgtTy; srcTy ], [ x ] ->
        if TypeDefinitelySubsumesTypeNoCoercion 0 g cenv.amap m tgtTy srcTy then
            CheckExpr cenv x ctxt TailCall.No
        else
            CheckExprNoByrefs cenv TailCall.No x

    | TOp.Reraise, [ _ty1 ], [] -> ()

    // Check get of static field
    | TOp.ValFieldGetAddr(_rfref, _readonly), _tyargs, [] -> ()

    // Check get of instance field
    | TOp.ValFieldGetAddr(_rfref, _readonly), _tyargs, [ obj ] ->
        // Recursively check in same ctxt, e.g. if at PermitOnlyReturnable the obj arg must also be returnable
        CheckExpr cenv obj ctxt TailCall.No

    | TOp.UnionCaseFieldGet _, _, [ arg1 ] -> CheckExprPermitByRefLike cenv arg1

    | TOp.UnionCaseTagGet _, _, [ arg1 ] -> CheckExprPermitByRefLike cenv arg1 // allow byref - it may be address-of-struct

    | TOp.UnionCaseFieldGetAddr(_uref, _idx, _readonly), _tyargs, [ obj ] ->
        // Recursively check in same ctxt, e.g. if at PermitOnlyReturnable the obj arg must also be returnable
        CheckExpr cenv obj ctxt TailCall.No

    | TOp.ILAsm(instrs, _retTypes), _, _ ->
        match instrs, args with
        // Write a .NET instance field
        | [ I_stfld(_alignment, _vol, _fspec) ], _ ->
            match args with
            | [ _; rhs ] -> CheckExprNoByrefs cenv TailCall.No rhs
            | _ -> ()

            // permit byref for lhs lvalue
            // permit byref for rhs lvalue (field would have to have ByRefLike type, i.e. be a field in another ByRefLike type)
            CheckExprsPermitByRefLike cenv args

        // Read a .NET instance field
        | [ I_ldfld(_alignment, _vol, _fspec) ], _ ->
            // permit byref for lhs lvalue
            CheckExprsPermitByRefLike cenv args

        // Read a .NET instance field
        | [ I_ldfld(_alignment, _vol, _fspec); AI_nop ], _ ->
            // permit byref for lhs lvalue of readonly value
            CheckExprsPermitByRefLike cenv args

        | [ I_ldsflda _fspec ], [] -> ()

        | [ I_ldflda _fspec ], [ obj ] ->

            // Recursively check in same ctxt, e.g. if at PermitOnlyReturnable the obj arg must also be returnable
            CheckExpr cenv obj ctxt TailCall.No

        | [ I_ldelema(_, _isNativePtr, _, _) ], lhsArray :: indices ->
            // permit byref for lhs lvalue
            CheckExprPermitByRefLike cenv lhsArray
            CheckExprsNoByRefLike cenv indices

        | [ AI_conv _ ], _ ->
            // permit byref for args to conv
            CheckExprsPermitByRefLike cenv args

        | _ -> CheckExprsNoByRefLike cenv args

    | TOp.TraitCall _, _, _ ->
        // allow args to be byref here
        CheckExprsPermitByRefLike cenv args

    | TOp.Recd _, _, _ -> CheckExprsPermitByRefLike cenv args

    | _ -> CheckExprsNoByRefLike cenv args

and CheckLambdas
    isTop
    (memberVal: Val option)
    cenv
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

    // The valReprInfo here says we are _guaranteeing_ to compile a function value
    // as a .NET method with precisely the corresponding argument counts.
    match stripDebugPoints expr with
    | Expr.TyChoose(_tps, e1, m) -> CheckLambdas isTop memberVal cenv inlined valReprInfo tailCall alwaysCheckNoReraise e1 m ety ctxt

    | Expr.Lambda(_, _, _, _, _, m, _)
    | Expr.TyLambda(_, _, _, m, _) ->
        let _tps, _ctorThisValOpt, _baseValOpt, _vsl, body, bodyTy =
            destLambdaWithValReprInfo g cenv.amap valReprInfo (expr, ety)

        // Check the body of the lambda
        if isTop && not g.compilingFSharpCore && isByrefLikeTy g m bodyTy then
            // allow byref to occur as return position for byref-typed top level function or method
            CheckExprPermitReturnableByRef cenv body
        else
            CheckExprNoByrefs cenv tailCall body

    // This path is for expression bindings that are not actually lambdas
    | _ ->
        let m = mOrig

        if not inlined && (isByrefLikeTy g m ety || isNativePtrTy g ety) then
            // allow byref to occur as RHS of byref binding.
            CheckExpr cenv expr ctxt tailCall
        else
            CheckExprNoByrefs cenv tailCall expr

and CheckExprs cenv exprs ctxts (tailCall: TailCall) : unit =
    let ctxts = Array.ofList ctxts

    let argArity i =
        if i < ctxts.Length then ctxts[i] else PermitByRefExpr.No

    exprs
    |> List.mapi (fun i exp -> CheckExpr cenv exp (argArity i) tailCall)
    |> ignore

and CheckExprsNoByRefLike cenv exprs : unit =
    for expr in exprs do
        CheckExprNoByrefs cenv TailCall.No expr

and CheckExprsPermitByRefLike cenv exprs : unit =
    exprs |> List.map (CheckExprPermitByRefLike cenv) |> ignore

and CheckExprPermitByRefLike cenv expr : unit =
    CheckExpr cenv expr PermitByRefExpr.Yes TailCall.No

and CheckExprPermitReturnableByRef cenv expr : unit =
    CheckExpr cenv expr PermitByRefExpr.YesReturnable TailCall.No

and CheckDecisionTreeTargets cenv targets ctxt (tailCall: TailCall) =
    targets
    |> Array.map (CheckDecisionTreeTarget cenv tailCall ctxt)
    |> List.ofArray
    |> ignore

and CheckDecisionTreeTarget cenv (tailCall: TailCall) ctxt (TTarget(_vs, targetExpr, _)) : unit = CheckExpr cenv targetExpr ctxt tailCall

and CheckDecisionTree cenv dtree =
    match dtree with
    | TDSuccess(resultExprs, _) -> CheckExprsNoByRefLike cenv resultExprs
    | TDBind(bind, rest) ->
        CheckBinding cenv false PermitByRefExpr.Yes bind
        CheckDecisionTree cenv rest
    | TDSwitch(inpExpr, cases, dflt, _m) -> CheckDecisionTreeSwitch cenv (inpExpr, cases, dflt)

and CheckDecisionTreeSwitch cenv (inpExpr, cases, dflt) =
    CheckExprPermitByRefLike cenv inpExpr // can be byref for struct union switch

    for TCase(discrim, dtree) in cases do
        CheckDecisionTreeTest cenv discrim
        CheckDecisionTree cenv dtree

    dflt |> Option.iter (CheckDecisionTree cenv)

and CheckDecisionTreeTest cenv discrim =
    match discrim with
    | DecisionTreeTest.ActivePatternCase(exp, _, _, _, _, _) -> CheckExprNoByrefs cenv TailCall.No exp
    | _ -> ()

and CheckBinding cenv alwaysCheckNoReraise ctxt (TBind(v, bindRhs, _) as bind) : unit =
    let g = cenv.g
    let isTop = Option.isSome bind.Var.ValReprInfo
    let tailCall = TailCall.YesFromVal g bind.Var

    let valReprInfo =
        match bind.Var.ValReprInfo with
        | Some info -> info
        | _ -> ValReprInfo.emptyValData

    CheckLambdas isTop (Some v) cenv v.ShouldInline valReprInfo tailCall alwaysCheckNoReraise bindRhs v.Range v.Type ctxt

and CheckBindings cenv binds =
    for bind in binds do
        CheckBinding cenv false PermitByRefExpr.Yes bind

let CheckModuleBinding cenv (isRec: bool) (TBind _ as bind) =

    // warn for non-rec functions which have the attribute
    if cenv.g.langVersion.SupportsFeature LanguageFeature.WarningWhenTailCallAttrOnNonRec then
        if not isRec && cenv.g.HasTailCallAttrib bind.Var.Attribs then
            warning (Error(FSComp.SR.chkTailCallAttrOnNonRec (), bind.Var.Range))

    // Check if a let binding to the result of a rec expression is not inside the rec expression
    // Check if a call of a rec expression is not inside a TryWith/TryFinally operation
    // see test ``Warn for invalid tailcalls in seq expression because of bind`` for an example
    // see test ``Warn successfully for rec call in binding`` for an example
    // see test ``Warn for simple rec call in try-with`` for an example
    if cenv.g.langVersion.SupportsFeature LanguageFeature.WarningWhenTailRecAttributeButNonTailRecUsage then
        match bind.Expr with
        | Expr.TyLambda(bodyExpr = bodyExpr)
        | Expr.Lambda(bodyExpr = bodyExpr) ->
            let rec checkTailCall (insideSubBindingOrTry: bool) expr =
                match expr with
                | Expr.Val(valRef = valRef; range = m) ->
                    if isRec && insideSubBindingOrTry && cenv.mustTailCall.Contains valRef.Deref then
                        warning (Error(FSComp.SR.chkNotTailRecursive valRef.DisplayName, m))
                | Expr.App(funcExpr = funcExpr; args = argExprs) ->
                    checkTailCall insideSubBindingOrTry funcExpr
                    argExprs |> List.iter (checkTailCall insideSubBindingOrTry)
                | Expr.Link exprRef -> checkTailCall insideSubBindingOrTry exprRef.Value
                | Expr.Lambda(bodyExpr = bodyExpr) -> checkTailCall insideSubBindingOrTry bodyExpr
                | Expr.DebugPoint(_debugPointAtLeafExpr, expr) -> checkTailCall insideSubBindingOrTry expr
                | Expr.Let(binding = binding; bodyExpr = bodyExpr) ->
                    // detect continuation shapes like MakeAsync
                    let isContinuation =
                        match bodyExpr with
                        | Expr.App(funcExpr = Expr.Val(valRef = valRef)) ->
                            match valRef.GeneralizedType with
                            | [ _ ],
                              TType_fun(domainType = TType_fun(domainType = TType_app _; rangeType = TType_app _); rangeType = TType_app _) ->
                                true
                            | _ -> false
                        | _ -> false

                    checkTailCall (not isContinuation) binding.Expr

                    let warnForBodyExpr =
                        insideSubBindingOrTry
                        || match stripDebugPoints bodyExpr with
                           | Expr.Op _ -> true
                           | _ -> false

                    checkTailCall warnForBodyExpr bodyExpr
                | Expr.Match(targets = decisionTreeTargets) ->
                    decisionTreeTargets
                    |> Array.iter (fun target -> checkTailCall insideSubBindingOrTry target.TargetExpression)
                | Expr.Op(args = exprs; op = TOp.TryWith _)
                | Expr.Op(args = exprs; op = TOp.TryFinally _) ->
                    // warn for recursive calls in TryWith/TryFinally operations
                    exprs |> Seq.iter (checkTailCall true)
                | Expr.Op(args = exprs) -> exprs |> Seq.iter (checkTailCall insideSubBindingOrTry)
                | Expr.Sequential(expr2 = expr2) -> checkTailCall insideSubBindingOrTry expr2
                | _ -> ()

            checkTailCall false bodyExpr
        | _ -> ()

    CheckBinding cenv true PermitByRefExpr.Yes bind

//--------------------------------------------------------------------------
// check modules
//--------------------------------------------------------------------------

let rec CheckDefnsInModule cenv mdefs =
    for mdef in mdefs do
        CheckDefnInModule cenv mdef

and CheckDefnInModule cenv mdef =
    match mdef with
    | TMDefRec(isRec, _opens, _tycons, mspecs, _m) ->
        let cenv =
            if isRec then
                let vals = allValsOfModDef mdef

                let mustTailCall =
                    Seq.fold
                        (fun mustTailCall (v: Val) ->
                            if cenv.g.HasTailCallAttrib v.Attribs then
                                let newSet = Zset.add v mustTailCall
                                newSet
                            else
                                mustTailCall)
                        cenv.mustTailCall
                        vals

                { cenv with
                    mustTailCall = mustTailCall
                }
            else
                cenv

        List.iter (CheckModuleSpec cenv isRec) mspecs
    | TMDefLet(bind, _m) -> CheckModuleBinding cenv false bind
    | TMDefOpens _ -> ()
    | TMDefDo(e, _m) ->
        let tailCall =
            match stripDebugPoints e with
            | Expr.App(funcExpr = funcExpr) ->
                match funcExpr with
                | ValUseAtApp(vref, _valUseFlags) -> TailCall.YesFromVal cenv.g vref.Deref
                | _ -> TailCall.No
            | _ -> TailCall.No

        CheckExprNoByrefs cenv tailCall e
    | TMDefs defs -> CheckDefnsInModule cenv defs

and CheckModuleSpec cenv isRec mbind =
    match mbind with
    | ModuleOrNamespaceBinding.Binding bind ->
        if cenv.mustTailCall.Contains bind.Var then
            CheckModuleBinding cenv isRec bind

    | ModuleOrNamespaceBinding.Module(_mspec, rhs) -> CheckDefnInModule cenv rhs

let CheckImplFile (g: TcGlobals, amap, reportErrors, implFileContents) =
    if
        reportErrors
        && g.langVersion.SupportsFeature LanguageFeature.WarningWhenTailRecAttributeButNonTailRecUsage
    then
        let cenv =
            {
                g = g
                stackGuard = StackGuard(PostInferenceChecksStackGuardDepth, "CheckImplFile")
                amap = amap
                mustTailCall = Zset.empty valOrder
            }

        CheckDefnInModule cenv implFileContents
