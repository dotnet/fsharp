// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.QuotationTranslator

open Internal.Utilities
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.AbstractIL.Diagnostics
open FSharp.Compiler.Tast
open FSharp.Compiler.Tastops
open FSharp.Compiler.Lib
open FSharp.Compiler.Ast
open FSharp.Compiler.PrettyNaming
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Range
open System.Collections.Generic

module QP = FSharp.Compiler.QuotationPickler

let verboseCReflect = condition "VERBOSE_CREFLECT"

[<RequireQualifiedAccess>]
type IsReflectedDefinition =
    | Yes
    | No

[<RequireQualifiedAccess>]
type QuotationSerializationFormat =
    /// Indicates that type references are emitted as integer indexes into a supplied table
    | FSharp_40_Plus
    | FSharp_20_Plus

type QuotationGenerationScope =
    { g: TcGlobals
      amap: Import.ImportMap
      scope: CcuThunk
      // Accumulate the references to type definitions
      referencedTypeDefs: ResizeArray<ILTypeRef>
      referencedTypeDefsTable: Dictionary<ILTypeRef, int>
      // Accumulate the type splices (i.e. captured type parameters) into here
      typeSplices: ResizeArray<Tast.Typar * range>
      // Accumulate the expression splices into here
      exprSplices: ResizeArray<Expr * range>
      isReflectedDefinition : IsReflectedDefinition
      quotationFormat : QuotationSerializationFormat
      mutable emitDebugInfoInQuotations : bool }

    static member Create (g: TcGlobals, amap, scope, isReflectedDefinition) =
        { g = g
          scope = scope
          amap = amap
          referencedTypeDefs = new ResizeArray<_>()
          referencedTypeDefsTable = new Dictionary<_, _>()
          typeSplices = new ResizeArray<_>()
          exprSplices = new ResizeArray<_>()
          isReflectedDefinition = isReflectedDefinition
          quotationFormat = QuotationGenerationScope.ComputeQuotationFormat g
          emitDebugInfoInQuotations = g.emitDebugInfoInQuotations }

    member cenv.Close() =
        cenv.referencedTypeDefs |> ResizeArray.toList,
        cenv.typeSplices |> ResizeArray.map (fun (ty, m) -> mkTyparTy ty, m) |> ResizeArray.toList,
        cenv.exprSplices |> ResizeArray.toList

    static member ComputeQuotationFormat g =
        let deserializeExValRef = ValRefForIntrinsic g.deserialize_quoted_FSharp_40_plus_info
        if deserializeExValRef.TryDeref.IsSome then
            QuotationSerializationFormat.FSharp_40_Plus
        else
            QuotationSerializationFormat.FSharp_20_Plus

type QuotationTranslationEnv =
    { 
      /// Map from Val to binding index
      vs: ValMap<int>

      nvs: int

      /// Map from typar stamps to binding index
      tyvs: StampMap<int>

      // Map for values bound by the
      //     'let v = isinst e in .... if nonnull v then ...v .... '
      // construct arising out the compilation of pattern matching. We decode these back to the form
      //     'if istype v then ...unbox v .... '
      isinstVals: ValMap<TType * Expr>

      substVals: ValMap<Expr> 
    }

    static member Empty =
        { vs = ValMap<_>.Empty
          nvs = 0
          tyvs = Map.empty
          isinstVals = ValMap<_>.Empty
          substVals = ValMap<_>.Empty }

    member env.BindTypar (v: Typar) =
        let idx = env.tyvs.Count
        { env with tyvs = env.tyvs.Add(v.Stamp, idx ) }

    member env.BindTypars vs =
        (env, vs) ||> List.fold (fun env v -> env.BindTypar v) // fold left-to-right because indexes are left-to-right

let BindFormalTypars (env: QuotationTranslationEnv) vs =
    { env with tyvs = Map.empty }.BindTypars vs

let BindVal env v =
    { env with
       vs = env.vs.Add v env.nvs
       nvs = env.nvs + 1 }

let BindIsInstVal env v (ty, e) =
    { env with isinstVals = env.isinstVals.Add v (ty, e) }

let BindSubstVal env v e =
    { env with substVals = env.substVals.Add v e  }

let BindVals env vs = List.fold BindVal env vs // fold left-to-right because indexes are left-to-right

let BindFlatVals env vs = List.fold BindVal env vs // fold left-to-right because indexes are left-to-right

exception InvalidQuotedTerm of exn

exception IgnoringPartOfQuotedTermWarning of string * Range.range

let wfail e = raise (InvalidQuotedTerm e)

let (|ModuleValueOrMemberUse|_|) g expr =
    let rec loop expr args =
        match stripExpr expr with
        | Expr.App ((InnerExprPat(Expr.Val (vref, vFlags, _) as f)), fty, tyargs, actualArgs, _m) when vref.IsMemberOrModuleBinding ->
            Some(vref, vFlags, f, fty, tyargs, actualArgs @ args)
        | Expr.App (f, _fty, [], actualArgs, _)  ->
            loop f (actualArgs @ args)
        | (Expr.Val (vref, vFlags, _m) as f) when (match vref.DeclaringEntity with ParentNone -> false | _ -> true) ->
            let fty = tyOfExpr g f
            Some(vref, vFlags, f, fty, [], args)
        | _ ->
            None
    loop expr []

let (|SimpleArrayLoopUpperBound|_|) expr =
    match expr with
    | Expr.Op (TOp.ILAsm ([AI_sub], _), _, [Expr.Op (TOp.ILAsm ([I_ldlen; AI_conv ILBasicType.DT_I4], _), _, _, _); Expr.Const (Const.Int32 1, _, _) ], _) -> Some ()
    | _ -> None

let (|SimpleArrayLoopBody|_|) g expr =
    match expr with
    | Expr.Lambda (_, a, b, ([_] as args), Expr.Let (TBind(forVarLoop, Expr.Op (TOp.ILAsm ([I_ldelem_any(ILArrayShape [(Some 0, None)], _)], _), [elemTy], [arr; idx], m1), seqPoint), body, m2, freeVars), m, ty) ->
        let body = Expr.Let (TBind(forVarLoop, mkCallArrayGet g m1 elemTy arr idx, seqPoint), body, m2, freeVars)
        let expr = Expr.Lambda (newUnique(), a, b, args, body, m, ty)
        Some (arr, elemTy, expr)
    | _ -> None

let (|ObjectInitializationCheck|_|) g expr =
    // recognize "if this.init@ < 1 then failinit"
    match expr with
    | Expr.Match
        (
           _, _,
           TDSwitch
            (
                Expr.Op (TOp.ILAsm ([AI_clt], _), _, [Expr.Op (TOp.ValFieldGet ((RFRef(_, name))), _, [Expr.Val (selfRef, NormalValUse, _)], _); Expr.Const (Const.Int32 1, _, _)], _), _, _, _
            ),
           [| TTarget([], Expr.App (Expr.Val (failInitRef, _, _), _, _, _, _), _); _ |], _, resultTy
        ) when
            IsCompilerGeneratedName name &&
            name.StartsWithOrdinal("init") &&
            selfRef.BaseOrThisInfo = MemberThisVal &&
            valRefEq g failInitRef (ValRefForIntrinsic g.fail_init_info) &&
            isUnitTy g resultTy -> Some()
    | _ -> None

let isSplice g vref = valRefEq g vref g.splice_expr_vref || valRefEq g vref g.splice_raw_expr_vref

let rec EmitDebugInfoIfNecessary cenv env m astExpr : QP.ExprData =
    // do not emit debug info if emitDebugInfoInQuotations = false or it was already written for the given expression
    if cenv.emitDebugInfoInQuotations && not (QP.isAttributedExpression astExpr) then
        cenv.emitDebugInfoInQuotations <- false
        try
            let mk_tuple g m es = mkRefTupled g m es (List.map (tyOfExpr g) es)

            let rangeExpr =
                    mk_tuple cenv.g m
                        [ mkString cenv.g m m.FileName
                          mkInt cenv.g m m.StartLine
                          mkInt cenv.g m m.StartColumn
                          mkInt cenv.g m m.EndLine
                          mkInt cenv.g m m.EndColumn ]
            let attrExpr =
                mk_tuple cenv.g m
                    [ mkString cenv.g m "DebugRange"
                      rangeExpr ]

            let attrExprR = ConvExprCore cenv env attrExpr

            QP.mkAttributedExpression(astExpr, attrExprR)
        finally
            cenv.emitDebugInfoInQuotations <- true
    else
        astExpr

and ConvExpr cenv env (expr : Expr) =
    EmitDebugInfoIfNecessary cenv env expr.Range (ConvExprCore cenv env expr)

and private ConvExprCore cenv (env : QuotationTranslationEnv) (expr: Expr) : QP.ExprData =

    let expr = DetectAndOptimizeForExpression cenv.g OptimizeIntRangesOnly expr

    // Eliminate subsumption coercions for functions. This must be done post-typechecking because we need
    // complete inference types.
    let expr = NormalizeAndAdjustPossibleSubsumptionExprs cenv.g expr

    // Remove TExpr_ref nodes
    let expr = stripExpr expr

    // Recognize F# object model calls
    // Recognize applications of module functions.
    match expr with
    // Detect expression tree exprSplices
    | Expr.App (InnerExprPat(Expr.Val (vf, _, _)), _, _, x0 :: rest, m)
           when isSplice cenv.g vf ->
        let idx = cenv.exprSplices.Count
        let ty = tyOfExpr cenv.g expr

        match (freeInExpr CollectTyparsAndLocalsNoCaching x0).FreeLocals |> Seq.tryPick (fun v -> if env.vs.ContainsVal v then Some v else None) with
        | Some v -> errorR(Error(FSComp.SR.crefBoundVarUsedInSplice(v.DisplayName), v.Range))
        | None -> ()

        cenv.exprSplices.Add((x0, m))
        let hole = QP.mkHole(ConvType cenv env m ty, idx)
        (hole, rest) ||> List.fold (fun fR arg -> QP.mkApp (fR, ConvExpr cenv env arg))

    | ModuleValueOrMemberUse cenv.g (vref, vFlags, _f, _fty, tyargs, curriedArgs)
        when not (isSplice cenv.g vref) ->
        let m = expr.Range

        let (numEnclTypeArgs, _, isNewObj, valUseFlags, isSelfInit, takesInstanceArg, isPropGet, isPropSet) =
            GetMemberCallInfo cenv.g (vref, vFlags)

        let isMember, tps, curriedArgInfos, retTy =
            match vref.MemberInfo with
            | Some _ when not vref.IsExtensionMember ->
                // This is an application of a member method
                // We only count one argument block for these.
                let tps, curriedArgInfos, retTy, _ = GetTypeOfIntrinsicMemberInCompiledForm cenv.g vref
                true, tps, curriedArgInfos, retTy
            | _ ->
                // This is an application of a module value or extension member
                let arities = arityOfVal vref.Deref
                let tps, curriedArgInfos, retTy, _ = GetTopValTypeInCompiledForm cenv.g arities vref.Type m
                false, tps, curriedArgInfos, retTy

        // Compute the object arguments as they appear in a compiled call
        // Strip off the object argument, if any. The curriedArgInfos are already adjusted to compiled member form
        let objArgs, curriedArgs =
            match takesInstanceArg, curriedArgs with
            | false, curriedArgs -> [], curriedArgs
            | true, (objArg :: curriedArgs) -> [objArg], curriedArgs
            | true, [] -> wfail(InternalError("warning: unexpected missing object argument when generating quotation for call to F# object member " + vref.LogicalName, m))

        if verboseCReflect then
            dprintfn "vref.DisplayName = %A, #objArgs = %A, #curriedArgs = %A" vref.DisplayName objArgs.Length curriedArgs.Length

        // Check to see if there aren't enough arguments or if there is a tuple-arity mismatch
        // If so, adjust and try again
        let nCurriedArgInfos = curriedArgInfos.Length
        if curriedArgs.Length < nCurriedArgInfos ||
           ((List.truncate nCurriedArgInfos curriedArgs, curriedArgInfos) ||> List.exists2 (fun arg argInfo ->
                       (argInfo.Length > (tryDestRefTupleExpr arg).Length)))
        then
            if verboseCReflect then
                dprintfn "vref.DisplayName = %A was under applied" vref.DisplayName
            // Too few arguments or incorrect tupling? Convert to a lambda and beta-reduce the
            // partially applied arguments to 'let' bindings
            let topValInfo =
               match vref.ValReprInfo with
               | None -> error(InternalError("no arity information found for F# value " + vref.LogicalName, vref.Range))
               | Some a -> a

            let expr, exprty = AdjustValForExpectedArity cenv.g m vref vFlags topValInfo
            ConvExpr cenv env (MakeApplicationAndBetaReduce cenv.g (expr, exprty, [tyargs], curriedArgs, m))
        else
            // Too many arguments? Chop
            let (curriedArgs: Expr list ), laterArgs = List.splitAt nCurriedArgInfos curriedArgs

            let callR =
                // We now have the right number of arguments, w.r.t. currying and tupling.
                // Next work out what kind of object model call and build an object model call node.

                // detuple the args
                let untupledCurriedArgs =
                    (curriedArgs, curriedArgInfos) ||> List.map2 (fun arg curriedArgInfo ->
                        let numUntupledArgs = curriedArgInfo.Length
                        (if numUntupledArgs = 0 then []
                         elif numUntupledArgs = 1 then [arg]
                         else tryDestRefTupleExpr arg))

                if verboseCReflect then
                    dprintfn "vref.DisplayName  = %A, after unit adjust, #untupledCurriedArgs = %A, #curriedArgInfos = %d" vref.DisplayName  (List.map List.length untupledCurriedArgs) curriedArgInfos.Length
                let subCall =
                    if isMember then
                        // This is an application of a member method
                        // We only count one argument block for these.
                        let callArgs = (objArgs :: untupledCurriedArgs) |> List.concat

                        let parentTyconR = ConvTyconRef cenv vref.TopValDeclaringEntity m
                        let isNewObj = isNewObj || valUseFlags || isSelfInit
                        // The signature types are w.r.t. to the formal context
                        let envinner = BindFormalTypars env tps
                        let argTys = curriedArgInfos |> List.concat |> List.map fst
                        let methArgTypesR = ConvTypes cenv envinner m argTys
                        let methRetTypeR = ConvReturnType cenv envinner m retTy
                        let methName = vref.CompiledName cenv.g.CompilerGlobalState
                        let numGenericArgs = tyargs.Length - numEnclTypeArgs
                        ConvObjectModelCall cenv env m (isPropGet, isPropSet, isNewObj, parentTyconR, methArgTypesR, methRetTypeR, methName, tyargs, numGenericArgs, callArgs)
                    else
                        // This is an application of the module value.
                        ConvModuleValueApp cenv env m vref tyargs untupledCurriedArgs
                match curriedArgs, curriedArgInfos with
                // static member and module value unit argument elimination
                | [arg: Expr], [[]] ->
                    // we got here if quotation is represents a call with unit argument
                    // let f () = ()
                    // <@ f @> // => (\arg -> f arg) => arg is Expr.Val - no-effects, first case
                    // <@ f() @> // Expr.Const (Unit) - no-effects - first case
                    // <@ f (someFunctionThatReturnsUnit) @> - potential effects - second case
                    match arg with
                    | Expr.Val _
                    | Expr.Const (Const.Unit, _, _) -> subCall
                    | _ ->
                        let argQ = ConvExpr cenv env arg
                        QP.mkSequential(argQ, subCall)
                | _ -> subCall

            List.fold (fun fR arg -> QP.mkApp (fR, ConvExpr cenv env arg)) callR laterArgs


    // Blast type application nodes and expression application nodes apart so values are left with just their type arguments
    | Expr.App (f, fty, (_ :: _ as tyargs), (_ :: _ as args), m) ->
      let rfty = applyForallTy cenv.g fty tyargs
      ConvExpr cenv env (primMkApp (primMkApp (f, fty) tyargs [] m, rfty) [] args m)

    // Uses of possibly-polymorphic values
    | Expr.App (InnerExprPat(Expr.Val (vref, _vFlags, m)), _fty, tyargs, [], _) ->
      ConvValRef true cenv env m vref tyargs

    // Simple applications
    | Expr.App (f, _fty, tyargs, args, m) ->
        if not (List.isEmpty tyargs) then wfail(Error(FSComp.SR.crefQuotationsCantContainGenericExprs(), m))
        List.fold (fun fR arg -> QP.mkApp (fR, ConvExpr cenv env arg)) (ConvExpr cenv env f) args

    // REVIEW: what is the quotation view of literals accessing enumerations? Currently they show up as integers.
    | Expr.Const (c, m, ty) ->
        ConvConst cenv env m c ty

    | Expr.Val (vref, _vFlags, m) ->
        ConvValRef true cenv env m vref []

    | Expr.Let (bind, body, _, _) ->
        // The binding may be a compiler-generated binding that gets removed in the quotation presentation
        match ConvLetBind cenv env bind with
        | None, env -> ConvExpr cenv env body
        | Some(bindR), env -> QP.mkLet(bindR, ConvExpr cenv env body)

    | Expr.LetRec (binds, body, _, _) ->
         let vs = valsOfBinds binds
         let vsR = vs |> List.map (ConvVal cenv env)
         let env = BindFlatVals env vs
         let bodyR = ConvExpr cenv env body
         let bindsR = List.zip vsR (binds |> List.map (fun b -> ConvExpr cenv env b.Expr))
         QP.mkLetRec(bindsR, bodyR)

    | Expr.Lambda (_, _, _, vs, b, _, _) ->
        let v, b = MultiLambdaToTupledLambda cenv.g vs b
        let vR = ConvVal cenv env v
        let bR = ConvExpr cenv (BindVal env v) b
        QP.mkLambda(vR, bR)

    | Expr.Quote (ast, _, _, _, ety) ->
        // F# 2.0-3.1 had a bug with nested 'raw' quotations. F# 4.0 + FSharp.Core 4.4.0.0+ allows us to do the right thing.
        if cenv.quotationFormat = QuotationSerializationFormat.FSharp_40_Plus &&
           // Look for a 'raw' quotation
           tyconRefEq cenv.g (tcrefOfAppTy cenv.g ety) cenv.g.raw_expr_tcr
        then
            QP.mkQuoteRaw40(ConvExpr cenv env ast)
        else
            QP.mkQuote(ConvExpr cenv env ast)

    | Expr.TyLambda (_, _, _, m, _) ->
        wfail(Error(FSComp.SR.crefQuotationsCantContainGenericFunctions(), m))

    | Expr.Match (_spBind, m, dtree, tgs, _, retTy) ->
        let typR = ConvType cenv env m retTy
        ConvDecisionTree cenv env tgs typR dtree

    // initialization check
    | Expr.Sequential (ObjectInitializationCheck cenv.g, x1, NormalSeq, _, _) -> ConvExpr cenv env x1
    | Expr.Sequential (x0, x1, NormalSeq, _, _)  -> QP.mkSequential(ConvExpr cenv env x0, ConvExpr cenv env x1)
    | Expr.Obj (_, ty, _, _, [TObjExprMethod(TSlotSig(_, ctyp, _, _, _, _), _, tps, [tmvs], e, _) as tmethod], _, _, m) when isDelegateTy cenv.g ty ->
         let f = mkLambdas m tps tmvs (e, GetFSharpViewOfReturnType cenv.g (returnTyOfMethod cenv.g tmethod))
         let fR = ConvExpr cenv env f
         let tyargR = ConvType cenv env m ctyp
         QP.mkDelegate(tyargR, fR)

    | Expr.StaticOptimization (_, _, x, _)               -> ConvExpr cenv env x
    | Expr.TyChoose _  -> ConvExpr cenv env (TypeRelations.ChooseTyparSolutionsForFreeChoiceTypars cenv.g cenv.amap expr)
    | Expr.Sequential  (x0, x1, ThenDoSeq, _, _)                        -> QP.mkSequential(ConvExpr cenv env x0, ConvExpr cenv env x1)
    | Expr.Obj (_lambdaId, _typ, _basev, _basecall, _overrides, _iimpls, _stateVars, m)      -> wfail(Error(FSComp.SR.crefQuotationsCantContainObjExprs(), m))

    | Expr.Op (op, tyargs, args, m) ->
        match op, tyargs, args with
        | TOp.UnionCase ucref, _, _ ->
            let mkR = ConvUnionCaseRef cenv ucref m
            let tyargsR = ConvTypes cenv env m tyargs
            let argsR = ConvExprs cenv env args
            QP.mkUnion(mkR, tyargsR, argsR)


        | TOp.Tuple tupInfo, tyargs, _ ->
            let tyR = ConvType cenv env m (mkAnyTupledTy cenv.g tupInfo tyargs)
            let argsR = ConvExprs cenv env args
            QP.mkTuple(tyR, argsR) // TODO: propagate to quotations

        | TOp.Recd (_, tcref), _, _  ->
            let rgtypR = ConvTyconRef cenv tcref m
            let tyargsR = ConvTypes cenv env m tyargs
            let argsR = ConvExprs cenv env args
            QP.mkRecdMk(rgtypR, tyargsR, argsR)

        | TOp.AnonRecd anonInfo, _, _  ->
            let tref = anonInfo.ILTypeRef
            let rgtypR = ConvILTypeRef cenv tref
            let tyargsR = ConvTypes cenv env m tyargs
            let argsR = ConvExprs cenv env args
            QP.mkRecdMk(rgtypR, tyargsR, argsR)

        | TOp.AnonRecdGet (anonInfo, n), _, _  ->
            let tref = anonInfo.ILTypeRef
            let rgtypR = ConvILTypeRef cenv tref
            let tyargsR = ConvTypes cenv env m tyargs
            let argsR = ConvExprs cenv env args
            QP.mkRecdGet((rgtypR, anonInfo.SortedNames.[n]), tyargsR, argsR)

        | TOp.UnionCaseFieldGet (ucref, n), tyargs, [e] ->
            ConvUnionFieldGet cenv env m ucref n tyargs e

        | TOp.ValFieldGetAddr (_rfref, _readonly), _tyargs, _ ->
            wfail(Error(FSComp.SR.crefQuotationsCantContainAddressOf(), m))

        | TOp.UnionCaseFieldGetAddr _, _tyargs, _ ->
            wfail(Error(FSComp.SR.crefQuotationsCantContainAddressOf(), m))

        | TOp.ValFieldGet (_rfref), _tyargs, [] ->
            wfail(Error(FSComp.SR.crefQuotationsCantContainStaticFieldRef(), m))

        | TOp.ValFieldGet (rfref), tyargs, args ->
            ConvClassOrRecdFieldGet cenv env m rfref tyargs args

        | TOp.TupleFieldGet (tupInfo, n), tyargs, [e] ->
            let eR = ConvLValueExpr cenv env e
            let tyR = ConvType cenv env m (mkAnyTupledTy cenv.g tupInfo tyargs)
            QP.mkTupleGet(tyR, n, eR)

        | TOp.ILAsm (([ I_ldfld (_, _, fspec) ]
                    | [ I_ldfld (_, _, fspec); AI_nop ]
                    | [ I_ldsfld (_, fspec) ]
                    | [ I_ldsfld (_, fspec); AI_nop ]), _), enclTypeArgs, args  ->
            ConvLdfld  cenv env m fspec enclTypeArgs args

        | TOp.ILAsm ([ I_stfld (_, _, fspec) | I_stsfld (_, fspec) ], _), enclTypeArgs, args  ->
            let tyargsR = ConvTypes cenv env m enclTypeArgs
            let parentTyconR = ConvILTypeRefUnadjusted cenv m fspec.DeclaringTypeRef
            let argsR = ConvLValueArgs cenv env args
            QP.mkFieldSet( (parentTyconR, fspec.Name), tyargsR, argsR)

        | TOp.ILAsm ([ AI_ceq ], _), _, [arg1;arg2]  ->
            let ty = tyOfExpr cenv.g arg1
            let eq = mkCallEqualsOperator cenv.g m ty arg1 arg2
            ConvExpr cenv env eq

        | TOp.ILAsm ([ I_throw ], _), _, [arg1]  ->
            let raiseExpr = mkCallRaise cenv.g m (tyOfExpr cenv.g expr) arg1
            ConvExpr cenv env raiseExpr

        | TOp.ILAsm (_il, _), _, _                         ->
            wfail(Error(FSComp.SR.crefQuotationsCantContainInlineIL(), m))

        | TOp.ExnConstr tcref, _, args              ->
            let _rgtypR = ConvTyconRef cenv tcref m
            let _typ = mkAppTy tcref []
            let parentTyconR = ConvTyconRef cenv tcref m
            let argtys = tcref |> recdFieldsOfExnDefRef  |> List.map (fun rfld -> rfld.FormalType)
            let methArgTypesR = ConvTypes cenv env m argtys
            let argsR = ConvExprs cenv env args
            let objR =
                QP.mkCtorCall( { ctorParent   = parentTyconR
                                 ctorArgTypes = methArgTypesR },
                              [], argsR)
            let exnTypeR = ConvType cenv env m cenv.g.exn_ty
            QP.mkCoerce(exnTypeR, objR)

        | TOp.ValFieldSet rfref, _tinst, args     ->
            let argsR = ConvLValueArgs cenv env args
            let tyargsR = ConvTypes cenv env m tyargs
            let ((_parentTyconR, fldOrPropName) as projR) = ConvRecdFieldRef cenv rfref m
            if rfref.TyconRef.IsRecordTycon then
                QP.mkRecdSet(projR, tyargsR, argsR)
            else
                let fspec = rfref.RecdField
                let tcref = rfref.TyconRef
                let parentTyconR = ConvTyconRef cenv tcref m
                if useGenuineField tcref.Deref fspec then
                    QP.mkFieldSet( projR, tyargsR, argsR)
                else
                    let envinner = BindFormalTypars env (tcref.TyparsNoRange)
                    let propRetTypeR = ConvType cenv envinner m fspec.FormalType
                    QP.mkPropSet( (parentTyconR, fldOrPropName, propRetTypeR, []), tyargsR, argsR)

        | TOp.ExnFieldGet (tcref, i), [], [obj] ->
            let exnc = stripExnEqns tcref
            let fspec = exnc.TrueInstanceFieldsAsList.[i]
            let parentTyconR = ConvTyconRef cenv tcref m
            let propRetTypeR = ConvType cenv env m fspec.FormalType
            let callArgR = ConvExpr cenv env obj
            let exnTypeR = ConvType cenv env m (generalizedTyconRef tcref)
            QP.mkPropGet( (parentTyconR, fspec.Name, propRetTypeR, []), [], [QP.mkCoerce (exnTypeR, callArgR)])

        | TOp.Coerce, [tgtTy;srcTy], [x]  ->
            let xR = ConvExpr cenv env x
            if typeEquiv cenv.g tgtTy srcTy then
                xR
            else
                QP.mkCoerce(ConvType cenv env m tgtTy, xR)

        | TOp.Reraise, [toTy], []         ->
            // rebuild reraise<T>() and Convert
            mkReraiseLibCall cenv.g toTy m |> ConvExpr cenv env

        | TOp.LValueOp (LAddrOf _, vref), [], [] ->
            QP.mkAddressOf(ConvValRef false cenv env m vref [])

        | TOp.LValueOp (LByrefSet, vref), [], [e] ->
            QP.mkAddressSet(ConvValRef false cenv env m vref [], ConvExpr cenv env e)

        | TOp.LValueOp (LSet, vref), [], [e] ->
            // Sets of module values become property sets
            match vref.DeclaringEntity with
            | Parent tcref when IsCompiledAsStaticProperty cenv.g vref.Deref  ->
                let parentTyconR = ConvTyconRef cenv tcref m
                let propName = vref.CompiledName cenv.g.CompilerGlobalState
                let propTy = ConvType cenv env m vref.Type
                QP.mkPropSet( (parentTyconR, propName, propTy, []), [], [ConvExpr cenv env e])
            | _ ->
                QP.mkVarSet( ConvValRef false cenv env m vref [], ConvExpr cenv env e)

        | TOp.LValueOp (LByrefGet, vref), [], [] ->
            ConvValRef false cenv env m vref []

        | TOp.Array, [ty], xa ->
             QP.mkNewArray(ConvType cenv env m ty, ConvExprs cenv env xa)

        | TOp.While _, [], [Expr.Lambda (_, _, _, [_], test, _, _);Expr.Lambda (_, _, _, [_], body, _, _)]  ->
              QP.mkWhileLoop(ConvExpr cenv env test, ConvExpr cenv env body)

        | TOp.For (_, FSharpForLoopUp), [], [Expr.Lambda (_, _, _, [_], lim0, _, _); Expr.Lambda (_, _, _, [_], SimpleArrayLoopUpperBound, lm, _); SimpleArrayLoopBody cenv.g (arr, elemTy, body)] ->
            let lim1 =
                let len = mkCallArrayLength cenv.g lm elemTy arr // Array.length arr
                mkCallSubtractionOperator cenv.g lm cenv.g.int32_ty len (Expr.Const (Const.Int32 1, m, cenv.g.int32_ty)) // len - 1
            QP.mkForLoop(ConvExpr cenv env lim0, ConvExpr cenv env lim1, ConvExpr cenv env body)

        | TOp.For (_, dir), [], [Expr.Lambda (_, _, _, [_], lim0, _, _);Expr.Lambda (_, _, _, [_], lim1, _, _);body]  ->
            match dir with
            | FSharpForLoopUp -> QP.mkForLoop(ConvExpr cenv env lim0, ConvExpr cenv env lim1, ConvExpr cenv env body)
            | _ -> wfail(Error(FSComp.SR.crefQuotationsCantContainDescendingForLoops(), m))

        | TOp.ILCall (_, _, _, isNewObj, valUseFlags, isProp, _, ilMethRef, enclTypeArgs, methTypeArgs, _tys), [], callArgs ->
             let parentTyconR = ConvILTypeRefUnadjusted cenv m ilMethRef.DeclaringTypeRef
             let isNewObj = isNewObj || (match valUseFlags with CtorValUsedAsSuperInit | CtorValUsedAsSelfInit -> true | _ -> false)
             let methArgTypesR = List.map (ConvILType cenv env m) ilMethRef.ArgTypes
             let methRetTypeR = ConvILType cenv env m ilMethRef.ReturnType
             let methName = ilMethRef.Name
             let isPropGet = isProp && methName.StartsWithOrdinal("get_")
             let isPropSet = isProp && methName.StartsWithOrdinal("set_")
             let tyargs = (enclTypeArgs@methTypeArgs)
             ConvObjectModelCall cenv env m (isPropGet, isPropSet, isNewObj, parentTyconR, methArgTypesR, methRetTypeR, methName, tyargs, methTypeArgs.Length, callArgs)

        | TOp.TryFinally _, [_resty], [Expr.Lambda (_, _, _, [_], e1, _, _); Expr.Lambda (_, _, _, [_], e2, _, _)] ->
            QP.mkTryFinally(ConvExpr cenv env e1, ConvExpr cenv env e2)

        | TOp.TryCatch _, [_resty], [Expr.Lambda (_, _, _, [_], e1, _, _); Expr.Lambda (_, _, _, [vf], ef, _, _); Expr.Lambda (_, _, _, [vh], eh, _, _)] ->
            let vfR = ConvVal cenv env vf
            let envf = BindVal env vf
            let vhR = ConvVal cenv env vh
            let envh = BindVal env vh
            QP.mkTryWith(ConvExpr cenv env e1, vfR, ConvExpr cenv envf ef, vhR, ConvExpr cenv envh eh)

        | TOp.Bytes bytes, [], [] ->
              ConvExpr cenv env (Expr.Op (TOp.Array, [cenv.g.byte_ty], List.ofArray (Array.map (mkByte cenv.g m) bytes), m))

        | TOp.UInt16s arr, [], [] ->
              ConvExpr cenv env (Expr.Op (TOp.Array, [cenv.g.uint16_ty], List.ofArray (Array.map (mkUInt16 cenv.g m) arr), m))

        | TOp.UnionCaseProof _, _, [e]       -> ConvExpr cenv env e  // Note: we erase the union case proof conversions when converting to quotations
        | TOp.UnionCaseTagGet _tycr, _tinst, [_cx]          -> wfail(Error(FSComp.SR.crefQuotationsCantFetchUnionIndexes(), m))
        | TOp.UnionCaseFieldSet (_c, _i), _tinst, [_cx;_x]     -> wfail(Error(FSComp.SR.crefQuotationsCantSetUnionFields(), m))
        | TOp.ExnFieldSet (_tcref, _i), [], [_ex;_x] -> wfail(Error(FSComp.SR.crefQuotationsCantSetExceptionFields(), m))
        | TOp.RefAddrGet _, _, _                       -> wfail(Error(FSComp.SR.crefQuotationsCantRequireByref(), m))
        | TOp.TraitCall (_ss), _, _                    -> wfail(Error(FSComp.SR.crefQuotationsCantCallTraitMembers(), m))
        | _ ->
            wfail(InternalError( "Unexpected expression shape", m))

    | _ ->
        wfail(InternalError(sprintf "unhandled construct in AST: %A" expr, expr.Range))

and ConvLdfld cenv env m (fspec: ILFieldSpec) enclTypeArgs args =
    let tyargsR = ConvTypes cenv env m enclTypeArgs
    let parentTyconR = ConvILTypeRefUnadjusted cenv m fspec.DeclaringTypeRef
    let argsR = ConvLValueArgs cenv env args
    QP.mkFieldGet( (parentTyconR, fspec.Name), tyargsR, argsR)

and ConvUnionFieldGet cenv env m ucref n tyargs e =
    let tyargsR = ConvTypes cenv env m tyargs
    let tcR, s = ConvUnionCaseRef cenv ucref m
    let projR = (tcR, s, n)
    let eR = ConvLValueExpr cenv env e
    QP.mkUnionFieldGet(projR, tyargsR, eR)

and ConvClassOrRecdFieldGet cenv env m rfref tyargs args =
    EmitDebugInfoIfNecessary cenv env m (ConvClassOrRecdFieldGetCore cenv env m rfref tyargs args)

and private ConvClassOrRecdFieldGetCore cenv env m rfref tyargs args =
    let tyargsR = ConvTypes cenv env m tyargs
    let argsR = ConvLValueArgs cenv env args
    let ((parentTyconR, fldOrPropName) as projR) = ConvRecdFieldRef cenv rfref m
    if rfref.TyconRef.IsRecordTycon then
        QP.mkRecdGet(projR, tyargsR, argsR)
    else
        let fspec = rfref.RecdField
        let tcref = rfref.TyconRef
        if useGenuineField tcref.Deref fspec then
            QP.mkFieldGet(projR, tyargsR, argsR)
        else
            let envinner = BindFormalTypars env tcref.TyparsNoRange
            let propRetTypeR = ConvType cenv envinner m fspec.FormalType
            QP.mkPropGet( (parentTyconR, fldOrPropName, propRetTypeR, []), tyargsR, argsR)

and ConvLetBind cenv env (bind : Binding) =
    match bind.Expr with
    // Map for values bound by the
    //     'let v = isinst e in .... if nonnull v then ...v .... '
    // construct arising out the compilation of pattern matching. We decode these back to the form
    //     'if istype e then ...unbox e .... '
    // It's bit annoying that pattern matching does this transformation. Like all premature optimization we pay a
    // cost here to undo it.
    | Expr.Op (TOp.ILAsm ([ I_isinst _ ], _), [ty], [e], _) ->
        None, BindIsInstVal env bind.Var (ty, e)

    // Remove let <compilerGeneratedVar> = <var> from quotation tree
    | Expr.Val _ when bind.Var.IsCompilerGenerated ->
        None, BindSubstVal env bind.Var bind.Expr

    // Remove let unionCase = ... from quotation tree
    | Expr.Op (TOp.UnionCaseProof _, _, [e], _) ->
        None, BindSubstVal env bind.Var e

    | _ ->
        let v = bind.Var
        let vR = ConvVal cenv env v
        let rhsR = ConvExpr cenv env bind.Expr
        let envinner = BindVal env v
        Some(vR, rhsR), envinner

and ConvLValueArgs cenv env args =
    match args with
    | obj :: rest -> ConvLValueExpr cenv env obj :: ConvExprs cenv env rest
    | [] -> []

and ConvLValueExpr cenv env expr =
    EmitDebugInfoIfNecessary cenv env expr.Range (ConvLValueExprCore cenv env expr)

// This function has to undo the work of mkExprAddrOfExpr
and ConvLValueExprCore cenv env expr =
    match expr with
    | Expr.Op (op, tyargs, args, m) ->
        match op, args, tyargs  with
        | TOp.LValueOp (LAddrOf _, vref), _, _ -> ConvValRef false cenv env m vref []
        | TOp.ValFieldGetAddr (rfref, _), _, _ -> ConvClassOrRecdFieldGet cenv env m rfref tyargs args
        | TOp.UnionCaseFieldGetAddr (ucref, n, _), [e], _ -> ConvUnionFieldGet cenv env m ucref n tyargs e
        | TOp.ILAsm ([ I_ldflda(fspec) ], _rtys), _, _  -> ConvLdfld  cenv env m fspec tyargs args
        | TOp.ILAsm ([ I_ldsflda(fspec) ], _rtys), _, _  -> ConvLdfld  cenv env m fspec tyargs args
        | TOp.ILAsm (([ I_ldelema(_ro, _isNativePtr, shape, _tyarg) ] ), _), (arr :: idxs), [elemty]  ->
            match shape.Rank, idxs with
            | 1, [idx1] -> ConvExpr cenv env (mkCallArrayGet cenv.g m elemty arr idx1)
            | 2, [idx1; idx2] -> ConvExpr cenv env (mkCallArray2DGet cenv.g m elemty arr idx1 idx2)
            | 3, [idx1; idx2; idx3] -> ConvExpr cenv env (mkCallArray3DGet cenv.g m elemty arr idx1 idx2 idx3)
            | 4, [idx1; idx2; idx3; idx4] -> ConvExpr cenv env (mkCallArray4DGet cenv.g m elemty arr idx1 idx2 idx3 idx4)
            | _ -> ConvExpr cenv env expr
        | _ -> ConvExpr cenv env expr
    | _ -> ConvExpr cenv env expr

and ConvObjectModelCall cenv env m callInfo =
    EmitDebugInfoIfNecessary cenv env m (ConvObjectModelCallCore cenv env m callInfo)

and ConvObjectModelCallCore cenv env m (isPropGet, isPropSet, isNewObj, parentTyconR, methArgTypesR, methRetTypeR, methName, tyargs, numGenericArgs, callArgs) =
    let tyargsR = ConvTypes cenv env m tyargs
    let callArgsR = ConvLValueArgs cenv env callArgs

    if isPropGet || isPropSet then
        let propName = ChopPropertyName methName
        if isPropGet then
            QP.mkPropGet( (parentTyconR, propName, methRetTypeR, methArgTypesR), tyargsR, callArgsR)
        else
            let args, propTy = List.frontAndBack methArgTypesR
            QP.mkPropSet( (parentTyconR, propName, propTy, args), tyargsR, callArgsR)

    elif isNewObj then
        let ctorR : QuotationPickler.CtorData =
            { ctorParent   = parentTyconR
              ctorArgTypes = methArgTypesR }
        QP.mkCtorCall(ctorR, tyargsR, callArgsR)

    else
        let methR : QuotationPickler.MethodData =
            { methParent   = parentTyconR
              methArgTypes = methArgTypesR
              methRetType  = methRetTypeR
              methName     = methName
              numGenericArgs = numGenericArgs }
        QP.mkMethodCall(methR, tyargsR, callArgsR)

and ConvModuleValueApp cenv env m (vref: ValRef) tyargs (args: Expr list list) =
    EmitDebugInfoIfNecessary cenv env m (ConvModuleValueAppCore cenv env m vref tyargs args)

and ConvModuleValueAppCore cenv env m (vref: ValRef) tyargs (args: Expr list list) =
    match vref.DeclaringEntity with
    | ParentNone -> failwith "ConvModuleValueApp"
    | Parent(tcref) ->
        let isProperty = IsCompiledAsStaticProperty cenv.g vref.Deref
        let tcrefR = ConvTyconRef cenv tcref m
        let tyargsR = ConvTypes cenv env m tyargs
        let nm = vref.CompiledName cenv.g.CompilerGlobalState
        let argsR = List.map (ConvExprs cenv env) args
        QP.mkModuleValueApp(tcrefR, nm, isProperty, tyargsR, argsR)

and ConvExprs cenv env args =
    List.map (ConvExpr cenv env) args

and ConvValRef holeOk cenv env m (vref: ValRef) tyargs =
    EmitDebugInfoIfNecessary cenv env m (ConvValRefCore holeOk cenv env m vref tyargs)

and private ConvValRefCore holeOk cenv env m (vref: ValRef) tyargs =
    let v = vref.Deref
    if env.isinstVals.ContainsVal v then
        let (ty, e) = env.isinstVals.[v]
        ConvExpr cenv env (mkCallUnbox cenv.g m ty e)
    elif env.substVals.ContainsVal v then
        let e = env.substVals.[v]
        ConvExpr cenv env e
    elif env.vs.ContainsVal v then
        if not (List.isEmpty tyargs) then wfail(InternalError("ignoring generic application of local quoted variable", m))
        QP.mkVar(env.vs.[v])
    elif v.BaseOrThisInfo = CtorThisVal && cenv.isReflectedDefinition = IsReflectedDefinition.Yes then
        QP.mkThisVar(ConvType cenv env m v.Type)
    else
        let vty = v.Type
        match v.DeclaringEntity with
        | ParentNone ->
              // References to local values are embedded by value
              if not holeOk then wfail(Error(FSComp.SR.crefNoSetOfHole(), m))
              let idx = cenv.exprSplices.Count
              cenv.exprSplices.Add((mkCallLiftValueWithName cenv.g m vty v.LogicalName (exprForValRef m vref), m))
              QP.mkHole(ConvType cenv env m vty, idx)
        | Parent _ ->
              ConvModuleValueApp cenv env m vref tyargs []

and ConvUnionCaseRef cenv (ucref: UnionCaseRef) m =
    let ucgtypR = ConvTyconRef cenv ucref.TyconRef m
    let nm =
        if cenv.g.unionCaseRefEq ucref cenv.g.cons_ucref then "Cons"
        elif cenv.g.unionCaseRefEq ucref cenv.g.nil_ucref then "Empty"
        else ucref.CaseName
    (ucgtypR, nm)

and ConvRecdFieldRef cenv (rfref: RecdFieldRef) m =
    let typR = ConvTyconRef cenv rfref.TyconRef m
    let nm =
        if useGenuineField rfref.TyconRef.Deref rfref.RecdField then
            ComputeFieldName rfref.TyconRef.Deref rfref.RecdField
        else
            rfref.FieldName
    (typR, nm)

and ConvVal cenv env (v: Val) =
    let tyR = ConvType cenv env v.Range v.Type
    QP.freshVar (v.CompiledName cenv.g.CompilerGlobalState, tyR, v.IsMutable)

and ConvTyparRef cenv env m (tp: Typar) =
    match env.tyvs.TryFind tp.Stamp  with
    | Some x -> x
    | None ->
        match ResizeArray.tryFindIndex (fun (tp2, _m) -> typarEq tp tp2) cenv.typeSplices with
        | Some idx -> idx
        | None  ->
            let idx = cenv.typeSplices.Count
            cenv.typeSplices.Add((tp, m))
            idx

and FilterMeasureTyargs tys =
    tys |> List.filter (fun ty -> match ty with TType_measure _ -> false | _ -> true)

and ConvType cenv env m ty =
    match stripTyEqnsAndMeasureEqns cenv.g ty with
    | TType_app(tcref, [tyarg]) when isArrayTyconRef cenv.g tcref ->
        QP.mkArrayTy(rankOfArrayTyconRef cenv.g tcref, ConvType cenv env m tyarg)

    | TType_ucase(UCRef(tcref, _), tyargs) // Note: we erase union case 'types' when converting to quotations
    | TType_app(tcref, tyargs) ->
#if !NO_EXTENSIONTYPING
        match TryElimErasableTyconRef cenv m tcref with
        | Some baseTy -> ConvType cenv env m baseTy
        | _ ->
#endif
        QP.mkILNamedTy(ConvTyconRef cenv tcref m, ConvTypes cenv env m tyargs)

    | TType_fun(a, b)          -> QP.mkFunTy(ConvType cenv env m a, ConvType cenv env m b)
    | TType_tuple(tupInfo, l)  -> ConvType cenv env m (mkCompiledTupleTy cenv.g (evalTupInfoIsStruct tupInfo) l)
    | TType_anon(anonInfo, tinst) ->
        let tref = anonInfo.ILTypeRef
        let tinstR = ConvTypes cenv env m tinst
        QP.mkILNamedTy(ConvILTypeRefUnadjusted cenv m tref, tinstR)
    | TType_var(tp)           -> QP.mkVarTy(ConvTyparRef cenv env m tp)
    | TType_forall(_spec, _ty)   -> wfail(Error(FSComp.SR.crefNoInnerGenericsInQuotations(), m))
    | _ -> wfail(Error (FSComp.SR.crefQuotationsCantContainThisType(), m))

and ConvTypes cenv env m tys =
    List.map (ConvType cenv env m) (FilterMeasureTyargs tys)

and ConvConst cenv env m c ty =
    match TryEliminateDesugaredConstants cenv.g m c with
    | Some e -> ConvExpr cenv env e
    | None ->
        let tyR = ConvType cenv env m ty
        match c with
        | Const.Bool    i ->  QP.mkBool (i, tyR)
        | Const.SByte   i ->  QP.mkSByte (i, tyR)
        | Const.Byte    i ->  QP.mkByte (i, tyR)
        | Const.Int16   i ->  QP.mkInt16 (i, tyR)
        | Const.UInt16  i ->  QP.mkUInt16 (i, tyR)
        | Const.Int32   i ->  QP.mkInt32 (i, tyR)
        | Const.UInt32  i ->  QP.mkUInt32 (i, tyR)
        | Const.Int64   i ->  QP.mkInt64 (i, tyR)
        | Const.UInt64  i ->  QP.mkUInt64 (i, tyR)
        | Const.Double   i ->  QP.mkDouble (i, tyR)
        | Const.Single i ->  QP.mkSingle (i, tyR)
        | Const.String  s ->  QP.mkString (s, tyR)
        | Const.Char    c ->  QP.mkChar (c, tyR)
        | Const.Unit      ->  QP.mkUnit()
        | Const.Zero      ->
            if isRefTy cenv.g ty then
                QP.mkNull tyR
            else
                QP.mkDefaultValue tyR
        | _ ->
            wfail(Error (FSComp.SR.crefQuotationsCantContainThisConstant(), m))

and ConvDecisionTree cenv env tgs typR x =
    match x with
    | TDSwitch(e1, csl, dfltOpt, m) ->
        let acc =
            match dfltOpt with
            | Some d -> ConvDecisionTree cenv env tgs typR d
            | None -> wfail(Error(FSComp.SR.crefQuotationsCantContainThisPatternMatch(), m))

        let converted =
            (csl, acc) ||> List.foldBack (fun (TCase(discrim, dtree)) acc ->

                  match discrim with
                  | DecisionTreeTest.UnionCase (ucref, tyargs) ->
                      let e1R = ConvLValueExpr cenv env e1
                      let ucR = ConvUnionCaseRef cenv ucref m
                      let tyargsR = ConvTypes cenv env m tyargs
                      QP.mkCond (QP.mkUnionCaseTagTest (ucR, tyargsR, e1R), ConvDecisionTree cenv env tgs typR dtree, acc)

                  | DecisionTreeTest.Const (Const.Bool true) ->
                      let e1R = ConvExpr cenv env e1
                      QP.mkCond (e1R, ConvDecisionTree cenv env tgs typR dtree, acc)

                  | DecisionTreeTest.Const (Const.Bool false) ->
                      let e1R = ConvExpr cenv env e1
                      // Note, reverse the branches
                      QP.mkCond (e1R, acc, ConvDecisionTree cenv env tgs typR dtree)

                  | DecisionTreeTest.Const c ->
                      let ty = tyOfExpr cenv.g e1
                      let eq = mkCallEqualsOperator cenv.g m ty e1 (Expr.Const (c, m, ty))
                      let eqR = ConvExpr cenv env eq
                      QP.mkCond (eqR, ConvDecisionTree cenv env tgs typR dtree, acc)

                  | DecisionTreeTest.IsNull ->
                      // Decompile cached isinst tests
                      match e1 with
                      | Expr.Val (vref, _, _) when env.isinstVals.ContainsVal vref.Deref  ->
                          let (ty, e) =  env.isinstVals.[vref.Deref]
                          let tyR = ConvType cenv env m ty
                          let eR = ConvExpr cenv env e
                          // note: reverse the branches - a null test is a failure of an isinst test
                          QP.mkCond (QP.mkTypeTest (tyR, eR), acc, ConvDecisionTree cenv env tgs typR dtree)
                      | _ ->
                          let ty = tyOfExpr cenv.g e1
                          let eq = mkCallEqualsOperator cenv.g m ty e1 (Expr.Const (Const.Zero, m, ty))
                          let eqR = ConvExpr cenv env eq
                          QP.mkCond (eqR, ConvDecisionTree cenv env tgs typR dtree, acc)

                  | DecisionTreeTest.IsInst (_srcty, tgty) ->
                      let e1R = ConvExpr cenv env e1
                      QP.mkCond (QP.mkTypeTest (ConvType cenv env m tgty, e1R), ConvDecisionTree cenv env tgs typR dtree, acc)

                  | DecisionTreeTest.ActivePatternCase _ -> wfail(InternalError( "DecisionTreeTest.ActivePatternCase test in quoted expression", m))

                  | DecisionTreeTest.ArrayLength _ -> wfail(Error(FSComp.SR.crefQuotationsCantContainArrayPatternMatching(), m))
                 )
        EmitDebugInfoIfNecessary cenv env m converted

      | TDSuccess (args, n) ->
          let (TTarget(vars, rhs, _)) = tgs.[n]
          // TAST stores pattern bindings in reverse order for some reason
          // Reverse them here to give a good presentation to the user
          let args = List.rev args
          let vars = List.rev vars

          let varsR = vars |> List.map (ConvVal cenv env)
          let targetR = ConvExpr cenv (BindVals env vars) rhs
          (varsR, args, targetR) |||> List.foldBack2 (fun vR arg acc -> QP.mkLet((vR, ConvExpr cenv env arg), acc) )

      | TDBind(bind, rest) ->
          // The binding may be a compiler-generated binding that gets removed in the quotation presentation
          match ConvLetBind cenv env bind with
          | None, env -> ConvDecisionTree cenv env tgs typR rest
          | Some(bindR), env -> QP.mkLet(bindR, ConvDecisionTree cenv env tgs typR rest)


// Check if this is an provider-generated assembly that will be statically linked
and IsILTypeRefStaticLinkLocal cenv m (tr: ILTypeRef) =
        ignore cenv; ignore m
        match tr.Scope with
#if !NO_EXTENSIONTYPING
        | ILScopeRef.Assembly aref
            when not cenv.g.isInteractive &&
                 aref.Name <> cenv.g.ilg.primaryAssemblyName && // optimization to avoid this check in the common case

                 // Explanation: This represents an unchecked invariant in the hosted compiler: that any operations
                 // which import types (and resolve assemblies from the tcImports tables) happen on the compilation thread.
                 let ctok = AssumeCompilationThreadWithoutEvidence()

                 (match cenv.amap.assemblyLoader.FindCcuFromAssemblyRef (ctok, m, aref) with
                  | ResolvedCcu ccu -> ccu.IsProviderGenerated
                  | UnresolvedCcu _ -> false)
            -> true
#endif
        | _ -> false

// Adjust for static linking information, then convert
and ConvILTypeRefUnadjusted cenv m (tr: ILTypeRef) =
    let trefAdjusted =
        if IsILTypeRefStaticLinkLocal cenv m tr then
            ILTypeRef.Create(ILScopeRef.Local, tr.Enclosing, tr.Name)
        else tr
    ConvILTypeRef cenv trefAdjusted

and ConvILTypeRef cenv (tr: ILTypeRef) =
    match cenv.quotationFormat with
    | QuotationSerializationFormat.FSharp_40_Plus ->
        let idx =
            match cenv.referencedTypeDefsTable.TryGetValue tr with
            | true, idx -> idx
            | _ ->
                let idx = cenv.referencedTypeDefs.Count
                cenv.referencedTypeDefs.Add tr
                cenv.referencedTypeDefsTable.[tr] <- idx
                idx
        QP.Idx idx

    | QuotationSerializationFormat.FSharp_20_Plus ->
        let assemblyRef =
            match tr.Scope with
            | ILScopeRef.Local -> "."
            | _ -> tr.Scope.QualifiedName

        QP.Named(tr.BasicQualifiedName, assemblyRef)

and ConvVoidType cenv m = QP.mkILNamedTy(ConvTyconRef cenv cenv.g.system_Void_tcref m, [])

and ConvILType cenv env m ty =
    match ty with
    | ILType.Boxed tspec | ILType.Value tspec -> QP.mkILNamedTy(ConvILTypeRefUnadjusted cenv m tspec.TypeRef, List.map (ConvILType cenv env m) tspec.GenericArgs)
    | ILType.Array (shape, ty) -> QP.mkArrayTy(shape.Rank, ConvILType cenv env m ty)
    | ILType.TypeVar idx -> QP.mkVarTy(int idx)
    | ILType.Void -> ConvVoidType cenv m
    | ILType.Ptr _
    | ILType.Byref _
    | ILType.Modified _
    | ILType.FunctionPointer _ -> wfail(Error(FSComp.SR.crefQuotationsCantContainThisType(), m))


#if !NO_EXTENSIONTYPING
and TryElimErasableTyconRef cenv m (tcref: TyconRef) =
    match tcref.TypeReprInfo with
    // Get the base type
    | TProvidedTypeExtensionPoint info when info.IsErased -> Some (info.BaseTypeForErased (m, cenv.g.obj_ty))
    | _ -> None
#endif

and ConvTyconRef cenv (tcref: TyconRef) m =
#if !NO_EXTENSIONTYPING
    match TryElimErasableTyconRef cenv m tcref with
    | Some baseTy -> ConvTyconRef cenv (tcrefOfAppTy cenv.g baseTy) m
    | None ->
    match tcref.TypeReprInfo with
    | TProvidedTypeExtensionPoint info when not cenv.g.isInteractive && not info.IsErased ->
        // Note, generated types are (currently) non-generic
        let tref = ExtensionTyping.GetILTypeRefOfProvidedType (info.ProvidedType, m)
        ConvILTypeRefUnadjusted cenv m tref
    | _ ->
#endif
    let repr = tcref.CompiledRepresentation
    match repr with
    | CompiledTypeRepr.ILAsmOpen asm ->
        match asm with
        | ILType.Boxed tspec | ILType.Value tspec ->
            ConvILTypeRef cenv tspec.TypeRef
        | _ ->
            wfail(Error(FSComp.SR.crefQuotationsCantContainThisType(), m))
    | CompiledTypeRepr.ILAsmNamed (tref, _boxity, _) ->
        ConvILTypeRefUnadjusted cenv m tref

and ConvReturnType cenv envinner m retTy =
    match retTy with
    | None -> ConvVoidType cenv m
    | Some ty -> ConvType cenv envinner m ty

let ConvExprPublic cenv env e =
    let astExpr =
        let astExpr = ConvExpr cenv env e
        // always emit debug info for the top level expression
        cenv.emitDebugInfoInQuotations <- true
        // EmitDebugInfoIfNecessary will check if astExpr is already augmented with debug info and won't wrap it twice
        EmitDebugInfoIfNecessary cenv env e.Range astExpr

    astExpr

let ConvMethodBase cenv env (methName, v: Val) =
    let m = v.Range
    let parentTyconR = ConvTyconRef cenv v.TopValDeclaringEntity m

    match v.MemberInfo with
    | Some vspr when not v.IsExtensionMember ->

        let vref = mkLocalValRef v
        let tps, argInfos, retTy, _ = GetTypeOfMemberInMemberForm cenv.g vref
        let numEnclTypeArgs = vref.MemberApparentEntity.TyparsNoRange.Length
        let argTys = argInfos |> List.concat |> List.map fst

        let isNewObj = (vspr.MemberFlags.MemberKind = MemberKind.Constructor)

        // The signature types are w.r.t. to the formal context
        let envinner = BindFormalTypars env tps
        let methArgTypesR = ConvTypes cenv envinner m argTys
        let methRetTypeR = ConvReturnType cenv envinner m retTy

        let numGenericArgs = tps.Length-numEnclTypeArgs

        if isNewObj then
             QP.MethodBaseData.Ctor
                 { ctorParent   = parentTyconR
                   ctorArgTypes = methArgTypesR }
        else
             QP.MethodBaseData.Method
                { methParent   = parentTyconR
                  methArgTypes = methArgTypesR
                  methRetType  = methRetTypeR
                  methName     = methName
                  numGenericArgs=numGenericArgs }

    | _ when v.IsExtensionMember ->

        let tps, argInfos, retTy, _ = GetTopValTypeInCompiledForm cenv.g v.ValReprInfo.Value v.Type v.Range
        let argTys = argInfos |> List.concat |> List.map fst
        let envinner = BindFormalTypars env tps
        let methArgTypesR = ConvTypes cenv envinner m argTys
        let methRetTypeR = ConvReturnType cenv envinner m retTy
        let numGenericArgs = tps.Length

        QP.MethodBaseData.Method
          { methParent   = parentTyconR
            methArgTypes = methArgTypesR
            methRetType  = methRetTypeR
            methName     = methName
            numGenericArgs=numGenericArgs }
    | _ ->

        QP.MethodBaseData.ModuleDefn
            { Name = methName
              Module = parentTyconR
              IsProperty = IsCompiledAsStaticProperty cenv.g v }


// FSComp.SR.crefQuotationsCantContainLiteralByteArrays

