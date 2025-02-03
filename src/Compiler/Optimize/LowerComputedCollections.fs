// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerComputedCollectionExpressions

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.InfoReader
open FSharp.Compiler.LowerSequenceExpressions
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypeRelations
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeHierarchy
open Import

/// Build the 'test and dispose' part of a 'use' statement
let BuildDisposableCleanup tcVal (g: TcGlobals) infoReader m (v: Val) =
    let disposeMethod = 
        match GetIntrinsicMethInfosOfType infoReader (Some "Dispose") AccessibleFromSomewhere AllowMultiIntfInstantiations.Yes IgnoreOverrides m g.system_IDisposable_ty with
        | [x] -> x
        | _ -> error(InternalError(FSComp.SR.tcCouldNotFindIDisposable(), m))
    // For struct types the test is simpler
    if isStructTy g v.Type then
        assert (TypeFeasiblySubsumesType 0 g infoReader.amap m g.system_IDisposable_ty CanCoerce v.Type)
        // We can use NeverMutates here because the variable is going out of scope, there is no need to take a defensive
        // copy of it.
        let disposeExpr, _ = BuildMethodCall tcVal g infoReader.amap NeverMutates m false disposeMethod NormalValUse [] [exprForVal v.Range v] [] None
        //callNonOverloadedILMethod g infoReader.amap m "Dispose" g.system_IDisposable_ty [exprForVal v.Range v]
        
        disposeExpr
    else
        let disposeObjVar, disposeObjExpr = mkCompGenLocal m "objectToDispose" g.system_IDisposableNull_ty
        let disposeExpr, _ = BuildMethodCall tcVal g infoReader.amap PossiblyMutates m false disposeMethod NormalValUse [] [disposeObjExpr] [] None
        let inputExpr = mkCoerceExpr(exprForVal v.Range v, g.obj_ty_ambivalent, m, v.Type)
        mkIsInstConditional g m g.system_IDisposable_ty inputExpr disposeObjVar disposeExpr (mkUnit g m)

let mkCallCollectorMethod tcVal (g: TcGlobals) infoReader m name collExpr args =
    let listCollectorTy = tyOfExpr g collExpr
    let addMethod = 
        match GetIntrinsicMethInfosOfType infoReader (Some name) AccessibleFromSomewhere AllowMultiIntfInstantiations.Yes IgnoreOverrides m listCollectorTy with
        | [x] -> x
        | _ -> error(InternalError("no " + name + " method found on Collector", m))
    let expr, _ = BuildMethodCall tcVal g infoReader.amap DefinitelyMutates m false addMethod NormalValUse [] [collExpr] args None
    expr

let mkCallCollectorAdd tcVal (g: TcGlobals) infoReader m collExpr arg =
    mkCallCollectorMethod tcVal g infoReader m "Add" collExpr [arg]

let mkCallCollectorAddMany tcVal (g: TcGlobals) infoReader m collExpr arg =
    mkCallCollectorMethod tcVal g infoReader m "AddMany" collExpr [arg]

let mkCallCollectorAddManyAndClose tcVal (g: TcGlobals) infoReader m collExpr arg =
    mkCallCollectorMethod tcVal g infoReader m "AddManyAndClose" collExpr [arg]

let mkCallCollectorClose tcVal (g: TcGlobals) infoReader m collExpr =
    mkCallCollectorMethod tcVal g infoReader m "Close" collExpr []

let LowerComputedListOrArraySeqExpr tcVal g amap m collectorTy overallSeqExpr =
    let infoReader = InfoReader(g, amap)
    let collVal, collExpr = mkMutableCompGenLocal m "@collector" collectorTy
    //let collExpr = mkValAddr m false (mkLocalValRef collVal)
    let rec ConvertSeqExprCode isUninteresting isTailcall expr =
        match expr with
        | SeqYield g (e, m) -> 
            let exprR = mkCallCollectorAdd tcVal g infoReader m collExpr e
            Result.Ok (false, exprR)

        | SeqDelay g (delayedExpr, _elemTy) ->
            ConvertSeqExprCode isUninteresting isTailcall delayedExpr

        | SeqAppend g (e1, e2, m) ->
            let res1 = ConvertSeqExprCode false false e1
            let res2 = ConvertSeqExprCode false isTailcall e2
            match res1, res2 with 
            | Result.Ok (_, e1R), Result.Ok (closed2, e2R) -> 
                let exprR = mkSequential m e1R e2R
                Result.Ok (closed2, exprR)
            | Result.Error msg, _ | _, Result.Error msg -> Result.Error msg

        | SeqWhile g (guardExpr, bodyExpr, spWhile, m) ->
            let resBody = ConvertSeqExprCode false false bodyExpr
            match resBody with 
            | Result.Ok (_, bodyExprR) ->
                let exprR = mkWhile g (spWhile, NoSpecialWhileLoopMarker, guardExpr, bodyExprR, m)
                Result.Ok (false, exprR)
            | Result.Error msg -> Result.Error msg

        | SeqUsing g (resource, v, bodyExpr, _elemTy, spBind, m) ->
            let resBody = ConvertSeqExprCode false false bodyExpr
            match resBody with 
            | Result.Ok (_, bodyExprR) ->
                // printfn "found Seq.using"
                let cleanupE = BuildDisposableCleanup tcVal g infoReader m v
                let exprR = 
                    mkLet spBind m v resource
                        (mkTryFinally g (bodyExprR, cleanupE, m, tyOfExpr g bodyExpr, DebugPointAtTry.No, DebugPointAtFinally.No))
                Result.Ok (false, exprR)
            | Result.Error msg -> Result.Error msg

        | SeqForEach g (inp, v, bodyExpr, _genElemTy, mFor, mIn, spIn) ->
            let resBody = ConvertSeqExprCode false false bodyExpr
            match resBody with 
            | Result.Ok (_, bodyExprR) ->
                // printfn "found Seq.for"
                let inpElemTy = v.Type
                let inpEnumTy = mkIEnumeratorTy g inpElemTy
                let enumv, enumve = mkCompGenLocal m "enum" inpEnumTy
                let guardExpr = callNonOverloadedILMethod g amap m "MoveNext" inpEnumTy [enumve]
                let cleanupE = BuildDisposableCleanup tcVal g infoReader m enumv

                // A debug point should get emitted prior to both the evaluation of 'inp' and the call to GetEnumerator
                let addForDebugPoint e = Expr.DebugPoint(DebugPointAtLeafExpr.Yes mFor, e)

                let spInAsWhile = match spIn with DebugPointAtInOrTo.Yes m -> DebugPointAtWhile.Yes m | DebugPointAtInOrTo.No -> DebugPointAtWhile.No

                let exprR =
                    mkInvisibleLet mFor enumv (callNonOverloadedILMethod g amap mFor "GetEnumerator" (mkSeqTy g inpElemTy) [inp])
                        (mkTryFinally g 
                            (mkWhile g (spInAsWhile, NoSpecialWhileLoopMarker, guardExpr, 
                                (mkInvisibleLet mIn v 
                                    (callNonOverloadedILMethod g amap mIn "get_Current" inpEnumTy [enumve]))
                                    bodyExprR, mIn), 
                            cleanupE,
                            mFor, tyOfExpr g bodyExpr, DebugPointAtTry.No, DebugPointAtFinally.No))
                    |> addForDebugPoint
                Result.Ok (false, exprR)
            | Result.Error msg -> Result.Error msg

        | SeqTryFinally g (bodyExpr, compensation, spTry, spFinally, m) ->
            let resBody = ConvertSeqExprCode false false bodyExpr
            match resBody with 
            | Result.Ok (_, bodyExprR) ->
                let exprR =
                    mkTryFinally g (bodyExprR, compensation, m, tyOfExpr g bodyExpr, spTry, spFinally)
                Result.Ok (false, exprR)
            | Result.Error msg -> Result.Error msg

        | SeqEmpty g m ->
            let exprR = mkUnit g m
            Result.Ok(false, exprR)

        | Expr.Sequential (x1, bodyExpr, NormalSeq, m) ->
            let resBody = ConvertSeqExprCode isUninteresting isTailcall bodyExpr
            match resBody with 
            | Result.Ok (closed, bodyExprR) ->
                let exprR = Expr.Sequential (x1, bodyExprR, NormalSeq, m)
                Result.Ok(closed, exprR)
            | Result.Error msg -> Result.Error msg

        | Expr.Let (bind, bodyExpr, m, _) ->
            let resBody = ConvertSeqExprCode isUninteresting isTailcall bodyExpr
            match resBody with 
            | Result.Ok (closed, bodyExprR) ->
                let exprR = mkLetBind m bind bodyExprR
                Result.Ok(closed, exprR)
            | Result.Error msg -> Result.Error msg

        | Expr.LetRec (binds, bodyExpr, m, _) ->
            let resBody = ConvertSeqExprCode isUninteresting isTailcall bodyExpr
            match resBody with 
            | Result.Ok (closed, bodyExprR) ->
                let exprR = mkLetRecBinds m binds bodyExprR
                Result.Ok(closed, exprR)
            | Result.Error msg -> Result.Error msg

        | Expr.Match (spBind, mExpr, pt, targets, m, ty) ->
            // lower all the targets. abandon if any fail to lower
            let resTargets =
                targets |> Array.map (fun (TTarget(vs, targetExpr, flags)) -> 
                    match ConvertSeqExprCode false false targetExpr with 
                    | Result.Ok (_, targetExprR) -> 
                        Result.Ok (TTarget(vs, targetExprR, flags))
                    | Result.Error msg -> Result.Error msg )

            if resTargets |> Array.forall (function Result.Ok _ -> true | _ -> false) then
                let tglArray = Array.map (function Result.Ok v -> v | _ -> failwith "unreachable") resTargets

                let exprR = primMkMatch (spBind, mExpr, pt, tglArray, m, ty)
                Result.Ok(false, exprR)
            else
                resTargets |> Array.pick (function Result.Error msg -> Some (Result.Error msg) | _ -> None)

        | Expr.DebugPoint(dp, innerExpr) ->
            let resInnerExpr = ConvertSeqExprCode isUninteresting isTailcall innerExpr
            match resInnerExpr with 
            | Result.Ok (flag, innerExprR) ->
                let exprR = Expr.DebugPoint(dp, innerExprR)
                Result.Ok (flag, exprR)
            | Result.Error msg -> Result.Error msg

        // yield! e ---> (for x in e -> x)

        | arbitrarySeqExpr ->
            let m = arbitrarySeqExpr.Range
            if isUninteresting then
                // printfn "FAILED - not worth compiling an unrecognized Seq.toList at %s " (stringOfRange m)
                Result.Error ()
            else
                // If we're the final in a sequential chain then we can AddMany, Close and return
                if isTailcall then 
                    let exprR = mkCallCollectorAddManyAndClose tcVal (g: TcGlobals) infoReader m collExpr arbitrarySeqExpr
                    // Return 'true' to indicate the collector was closed and the overall result of the expression is the result
                    Result.Ok(true, exprR)
                else
                    let exprR = mkCallCollectorAddMany tcVal (g: TcGlobals) infoReader m collExpr arbitrarySeqExpr
                    Result.Ok(false, exprR)


    // Perform conversion
    match ConvertSeqExprCode true true overallSeqExpr with 
    | Result.Ok (closed, overallSeqExprR) ->
        mkInvisibleLet m collVal (mkDefault (m, collectorTy)) 
            (if closed then 
                // If we ended with AddManyAndClose then we're done
                overallSeqExprR
             else
                mkSequential m
                    overallSeqExprR
                    (mkCallCollectorClose tcVal g infoReader m collExpr))
        |> Some
    | Result.Error () -> 
        None

let (|OptionalCoerce|) expr = 
    match expr with
    | Expr.Op (TOp.Coerce, _, [arg], _) -> arg
    | _ -> expr

// Making 'seq' optional means this kicks in for FSharp.Core, see TcArrayOrListComputedExpression
// which only adds a 'seq' call outside of FSharp.Core
[<return: Struct>]
let (|OptionalSeq|_|) g amap expr =
    match expr with
    // use 'seq { ... }' as an indicator
    | Seq g (e, elemTy) -> 
        ValueSome (e, elemTy)
    | _ -> 
    // search for the relevant element type
    match tyOfExpr g expr with
    | SeqElemTy g amap expr.Range elemTy ->
        ValueSome (expr, elemTy)
    | _ -> ValueNone

[<return: Struct>]
let (|SeqToList|_|) g expr =
    match expr with
    | ValApp g g.seq_to_list_vref (_, [seqExpr], m) -> ValueSome (seqExpr, m)
    | _ -> ValueNone

[<return: Struct>]
let (|SeqToArray|_|) g expr =
    match expr with
    | ValApp g g.seq_to_array_vref (_, [seqExpr], m) -> ValueSome (seqExpr, m)
    | _ -> ValueNone

module List =
    /// Makes the equivalent of an inlined call to List.map.
    let mkMap tcVal (g: TcGlobals) amap m (mBody, spFor, _spIn, mFor, mIn, spInWhile) srcList overallElemTy loopVal body =
        let collectorTy = g.mk_ListCollector_ty overallElemTy
        let srcListTy = tyOfExpr g srcList

        mkCompGenLetMutableIn m "collector" collectorTy (mkDefault (m, collectorTy)) (fun (_, collector) ->
            let reader = InfoReader (g, amap)

            // Adapted from DetectAndOptimizeForEachExpression in TypedTreeOps.fs.

            let IndexHead = 0
            let IndexTail = 1

            let currentVar, currentExpr = mkMutableCompGenLocal mIn "current" srcListTy
            let nextVar, nextExpr = mkMutableCompGenLocal mIn "next" srcListTy
            let srcElemTy = loopVal.val_type

            let guardExpr = mkNonNullTest g mFor nextExpr
            let headOrDefaultExpr = mkUnionCaseFieldGetUnprovenViaExprAddr (currentExpr, g.cons_ucref, [srcElemTy], IndexHead, mIn)
            let tailOrNullExpr = mkUnionCaseFieldGetUnprovenViaExprAddr (currentExpr, g.cons_ucref, [srcElemTy], IndexTail, mIn)

            let body =
                mkInvisibleLet mIn loopVal headOrDefaultExpr
                    (mkSequential mIn
                        (mkCallCollectorAdd tcVal g reader mIn collector body)
                        (mkSequential mIn
                            (mkValSet mIn (mkLocalValRef currentVar) nextExpr)
                            (mkValSet mIn (mkLocalValRef nextVar) tailOrNullExpr)))

            let loop =
                // let mutable current = enumerableExpr
                mkLet spFor m currentVar srcList
                    // let mutable next = current.TailOrNull
                    (mkInvisibleLet mFor nextVar tailOrNullExpr 
                        // while nonNull next do
                       (mkWhile g (spInWhile, WhileLoopForCompiledForEachExprMarker, guardExpr, body, mBody)))

            let close = mkCallCollectorClose tcVal g reader m collector

            mkSequential m loop close
        )

    /// Makes an expression that will build a list from an integral range.
    let mkFromIntegralRange
        tcVal
        (g: TcGlobals)
        amap
        m
        (mBody, _spFor, _spIn, mFor, mIn, spInWhile)
        rangeTy
        overallElemTy
        (rangeExpr: Expr)
        start
        step
        finish
        (body: (Val * Expr) option)
        =
        let collectorTy = g.mk_ListCollector_ty overallElemTy

        /// let collector = ListCollector () in
        /// <initialization loop>
        /// collector.Close ()
        let mkListInit mkLoop =
            mkCompGenLetMutableIn m "collector" collectorTy (mkDefault (m, collectorTy)) (fun (_, collector) ->
                let reader = InfoReader (g, amap)

                let loop =
                    mkLoop (fun _idxVar loopVar ->
                        let body =
                            body
                            |> Option.map (fun (loopVal, body) -> mkInvisibleLet m loopVal loopVar body)
                            |> Option.defaultValue loopVar

                        mkCallCollectorAdd tcVal g reader mBody collector body)

                let close = mkCallCollectorClose tcVal g reader mBody collector
                mkSequential m loop close
            )

        mkOptimizedRangeLoop
            g
            (mBody, mFor, mIn, spInWhile)
            (rangeTy, rangeExpr)
            (start, step, finish)
            (fun count mkLoop ->
                match count with
                | Expr.Const (value = IntegralConst.Zero) ->
                    mkNil g m overallElemTy

                | Expr.Const (value = _nonzeroConstant) ->
                    mkListInit mkLoop

                | _dynamicCount ->
                    mkListInit mkLoop
            )

module Array =
    let private mkIlInstr (g: TcGlobals) specific any ilTy =
        if ilTy = g.ilg.typ_Int32 then specific DT_I4
        elif ilTy = g.ilg.typ_Int64 then specific DT_I8
        elif ilTy = g.ilg.typ_UInt64 then specific DT_U8
        elif ilTy = g.ilg.typ_UInt32 then specific DT_U4
        elif ilTy = g.ilg.typ_IntPtr then specific DT_I
        elif ilTy = g.ilg.typ_UIntPtr then specific DT_U
        elif ilTy = g.ilg.typ_Int16 then specific DT_I2
        elif ilTy = g.ilg.typ_UInt16 then specific DT_U2
        elif ilTy = g.ilg.typ_SByte then specific DT_I1
        elif ilTy = g.ilg.typ_Byte then specific DT_U1
        elif ilTy = g.ilg.typ_Char then specific DT_U2
        elif ilTy = g.ilg.typ_Double then specific DT_R8
        elif ilTy = g.ilg.typ_Single then specific DT_R4
        else any ilTy

    /// Makes the equivalent of an inlined call to Array.map.
    let mkMap g m (mBody, _spFor, _spIn, mFor, mIn, spInWhile) srcArray srcIlTy destIlTy overallElemTy (loopVal: Val) body =
        mkCompGenLetIn m (nameof srcArray) (tyOfExpr g srcArray) srcArray (fun (_, srcArray) ->
            let len = mkLdlen g mIn srcArray
            let arrayTy = mkArrayType g overallElemTy

            /// (# "newarr !0" type ('T) count : 'T array #)
            let array =
                mkAsmExpr
                    (
                        [I_newarr (ILArrayShape.SingleDimensional, destIlTy)],
                        [],
                        [len],
                        [arrayTy],
                        m
                    )

            let ldelem = mkIlInstr g I_ldelem (fun ilTy -> I_ldelem_any (ILArrayShape.SingleDimensional, ilTy)) srcIlTy
            let stelem = mkIlInstr g I_stelem (fun ilTy -> I_stelem_any (ILArrayShape.SingleDimensional, ilTy)) destIlTy

            let mapping =
                mkCompGenLetIn m (nameof array) arrayTy array (fun (_, array) ->
                    mkCompGenLetMutableIn mFor "i" g.int32_ty (mkTypedZero g mIn g.int32_ty) (fun (iVal, i) ->
                        let body =
                            // If the loop val is used in the loop body,
                            // rebind it to pull directly from the source array.
                            // Otherwise, don't bother reading from the source array at all.
                            let body =
                                let freeLocals = (freeInExpr CollectLocals body).FreeLocals

                                if freeLocals.Contains loopVal then
                                    mkInvisibleLet mBody loopVal (mkAsmExpr ([ldelem], [], [srcArray; i], [loopVal.val_type], mBody)) body
                                else
                                    body

                            // destArray[i] <- body srcArray[i]
                            let setArrSubI = mkAsmExpr ([stelem], [], [array; i; body], [], mIn)

                            // i <- i + 1
                            let incrI = mkValSet mIn (mkLocalValRef iVal) (mkAsmExpr ([AI_add], [], [i; mkTypedOne g mIn g.int32_ty], [g.int32_ty], mIn))

                            mkSequential mIn setArrSubI incrI

                        let guard = mkILAsmClt g mFor i (mkLdlen g mFor array)

                        let loop =
                            mkWhile
                                g
                                (
                                    spInWhile,
                                    NoSpecialWhileLoopMarker,
                                    guard,
                                    body,
                                    mIn
                                )

                        // while i < array.Length do <body> done
                        // array
                        mkSequential m loop array
                    )
                )

            // Add a debug point at the `for`, before anything gets evaluated.
            Expr.DebugPoint (DebugPointAtLeafExpr.Yes mFor, mapping)
        )

    /// Whether to check for overflow when converting a value to a native int.
    [<NoEquality; NoComparison>]
    type Ovf =
        /// Check for overflow. We need this when passing the count into newarr.
        | CheckOvf

        /// Don't check for overflow. We don't need to check when indexing into the array,
        /// since we already know count didn't overflow during initialization.
        | NoCheckOvf

    /// Makes an expression that will build an array from an integral range.
    let mkFromIntegralRange g m (mBody, _spFor, _spIn, mFor, mIn, spInWhile) rangeTy ilTy overallElemTy (rangeExpr: Expr) start step finish (body: (Val * Expr) option) =
        let arrayTy = mkArrayType g overallElemTy

        let convToNativeInt ovf expr =
            let ty = tyOfExpr g expr

            let conv =
                match ovf with
                | NoCheckOvf -> AI_conv DT_I
                | CheckOvf when isSignedIntegerTy g ty -> AI_conv_ovf DT_I
                | CheckOvf -> AI_conv_ovf_un DT_I

            if typeEquivAux EraseMeasures g ty g.int64_ty then
                mkAsmExpr ([conv], [], [expr], [g.nativeint_ty], mIn)
            elif typeEquivAux EraseMeasures g ty g.nativeint_ty then
                mkAsmExpr ([conv], [], [mkAsmExpr ([AI_conv DT_I8], [], [expr], [g.int64_ty], mIn)], [g.nativeint_ty], mIn)
            elif typeEquivAux EraseMeasures g ty g.uint64_ty then
                mkAsmExpr ([conv], [], [expr], [g.nativeint_ty], mIn)
            elif typeEquivAux EraseMeasures g ty g.unativeint_ty then
                mkAsmExpr ([conv], [], [mkAsmExpr ([AI_conv DT_U8], [], [expr], [g.uint64_ty], mIn)], [g.nativeint_ty], mIn)
            else
                expr

        let stelem = mkIlInstr g I_stelem (fun ilTy -> I_stelem_any (ILArrayShape.SingleDimensional, ilTy)) ilTy

        /// (# "newarr !0" type ('T) count : 'T array #)
        let mkNewArray count =
            mkAsmExpr
                (
                    [I_newarr (ILArrayShape.SingleDimensional, ilTy)],
                    [],
                    [convToNativeInt CheckOvf count],
                    [arrayTy],
                    m
                )

        /// let array = (# "newarr !0" type ('T) count : 'T array #) in
        /// <initialization loop>
        /// array
        let mkArrayInit count mkLoop =
            mkCompGenLetIn mFor "array" arrayTy (mkNewArray count) (fun (_, array) ->
                let loop =
                    mkLoop (fun idxVar loopVar ->
                        let body =
                            body
                            |> Option.map (fun (loopVal, body) -> mkInvisibleLet mBody loopVal loopVar body)
                            |> Option.defaultValue loopVar

                        mkAsmExpr ([stelem], [], [array; convToNativeInt NoCheckOvf idxVar; body], [], mBody))

                mkSequential m loop array)

        mkOptimizedRangeLoop
            g
            (mBody, mFor, mIn, spInWhile)
            (rangeTy, rangeExpr)
            (start, step, finish)
            (fun count mkLoop ->
                match count with
                | Expr.Const (value = IntegralConst.Zero) ->
                    mkArray (overallElemTy, [], m)

                | Expr.Const (value = _nonzeroConstant) ->
                    mkArrayInit count mkLoop

                | _dynamicCount ->
                    mkCompGenLetIn m (nameof count) (tyOfExpr g count) count (fun (_, count) ->
                        let countTy = tyOfExpr g count

                        // if count = 0 then
                        //     [||]
                        // else
                        //     let array = (# "newarr !0" type ('T) count : 'T array #) in
                        //     <initialization loop>
                        //     array
                        mkCond
                            DebugPointAtBinding.NoneAtInvisible
                            m
                            arrayTy
                            (mkILAsmCeq g m count (mkTypedZero g m countTy))
                            (mkArray (overallElemTy, [], m))
                            (mkArrayInit count mkLoop)
                    )
            )

/// Matches Seq.singleton and returns the body expression.
[<return: Struct>]
let (|SeqSingleton|_|) g expr : Expr voption =
    match expr with
    | ValApp g g.seq_singleton_vref (_, [body], _) -> ValueSome body
    | _ -> ValueNone

/// Matches the compiled representation of the mapping in
///
///     for … in … do f (); …; yield …
///     for … in … do let … = … in yield …
///     for … in … do f (); …; …
///     for … in … do let … = … in …
///
/// i.e.,
///
///     f (); …; Seq.singleton …
///     let … = … in Seq.singleton …
[<return: Struct>]
let (|SingleYield|_|) g expr : Expr voption =
    let rec loop expr cont =
        match expr with
        | Expr.Let (binding, DebugPoints (SeqSingleton g body, debug), m, frees) ->
            ValueSome (cont (Expr.Let (binding, debug body, m, frees)))

        | Expr.Let (binding, DebugPoints (body, debug), m, frees) ->
            loop body (cont << fun body -> Expr.Let (binding, debug body, m, frees))

        | Expr.Sequential (expr1, DebugPoints (SeqSingleton g body, debug), kind, m) ->
            ValueSome (cont (Expr.Sequential (expr1, debug body, kind, m)))

        | Expr.Sequential (expr1, DebugPoints (body, debug), kind, m) ->
            loop body (cont << fun body -> Expr.Sequential (expr1, debug body, kind, m))

        | Expr.Match (debugPoint, mInput, decision, [|TTarget (boundVals, DebugPoints (SeqSingleton g body, debug), isStateVarFlags)|], mFull, exprType) ->
            ValueSome (cont (Expr.Match (debugPoint, mInput, decision, [|TTarget (boundVals, debug body, isStateVarFlags)|], mFull, exprType)))

        | SeqSingleton g body ->
            ValueSome (cont body)

        | _ -> ValueNone

    loop expr id

/// Extracts any let-bindings or sequential
/// expressions that directly precede the specified mapping application, e.g.,
///
///     [let y = f () in for … in … -> …]
///
///     [f (); g (); for … in … -> …]
///
/// Returns a function that will re-prefix the prelude to the
/// lowered mapping, as well as the mapping to lower, i.e.,
/// to transform the above into something like:
///
///     let y = f () in [for … in … -> …]
///
///     f (); g (); [for … in … -> …]
let gatherPrelude ((|App|_|) : _ -> _ voption) expr =
    let rec loop expr cont =
        match expr with
        | Expr.Let (binding, DebugPoints (body, debug), m, frees) ->
            loop body (cont << fun body -> Expr.Let (binding, debug body, m, frees))

        | Expr.Sequential (expr1, DebugPoints (body, debug), kind, m) ->
            loop body (cont << fun body -> Expr.Sequential (expr1, debug body, kind, m))

        | App contents ->
            ValueSome (cont, contents)

        | _ -> ValueNone

    loop expr id

/// The representation used for
///
///     for … in … -> …
///     for … in … do yield …
///     for … in … do …
[<return: Struct>]
let (|SeqMap|_|) g =
    gatherPrelude (function
        | ValApp g g.seq_map_vref ([ty1; ty2], [Expr.Lambda (valParams = [loopVal]; bodyExpr = body; range = mIn) as mapping; input], mFor) ->
            let spIn = match mIn.NotedSourceConstruct with NotedSourceConstruct.InOrTo -> DebugPointAtInOrTo.Yes mIn | _ -> DebugPointAtInOrTo.No
            let spFor = DebugPointAtBinding.Yes mFor
            let spInWhile = match spIn with DebugPointAtInOrTo.Yes m -> DebugPointAtWhile.Yes m | DebugPointAtInOrTo.No -> DebugPointAtWhile.No
            let ranges = body.Range, spFor, spIn, mFor, mIn, spInWhile
            ValueSome (ty1, ty2, input, mapping, loopVal, body, ranges)

        | _ -> ValueNone)

/// The representation used for
///
///     for … in … do f (); …; yield …
///     for … in … do let … = … in yield …
///     for … in … do f (); …; …
///     for … in … do let … = … in …
[<return: Struct>]
let (|SeqCollectSingle|_|) g =
    gatherPrelude (function
        | ValApp g g.seq_collect_vref ([ty1; _; ty2], [Expr.Lambda (valParams = [loopVal]; bodyExpr = DebugPoints (SingleYield g body, debug); range = mIn) as mapping; input], mFor) ->
            let spIn = match mIn.NotedSourceConstruct with NotedSourceConstruct.InOrTo -> DebugPointAtInOrTo.Yes mIn | _ -> DebugPointAtInOrTo.No
            let spFor = DebugPointAtBinding.Yes mFor
            let spInWhile = match spIn with DebugPointAtInOrTo.Yes m -> DebugPointAtWhile.Yes m | DebugPointAtInOrTo.No -> DebugPointAtWhile.No
            let ranges = body.Range, spFor, spIn, mFor, mIn, spInWhile
            ValueSome (ty1, ty2, input, mapping, loopVal, debug body, ranges)

        | _ -> ValueNone)

/// for … in … -> …
/// for … in … do yield …
/// for … in … do …
/// for … in … do f (); …; yield …
/// for … in … do let … = … in yield …
/// for … in … do f (); …; …
/// for … in … do let … = … in …
[<return: Struct>]
let (|SimpleMapping|_|) g expr =
    match expr with
    // for … in … -> …
    // for … in … do yield …
    // for … in … do …
    | ValApp g g.seq_delay_vref (_, [Expr.Lambda (bodyExpr = DebugPoints (SeqMap g (cont, (ty1, ty2, input, mapping, loopVal, body, ranges)), debug))], _)

    // for … in … do f (); …; yield …
    // for … in … do let … = … in yield …
    // for … in … do f (); …; …
    // for … in … do let … = … in …
    | ValApp g g.seq_delay_vref (_, [Expr.Lambda (bodyExpr = DebugPoints (SeqCollectSingle g (cont, (ty1, ty2, input, mapping, loopVal, body, ranges)), debug))], _) ->
        ValueSome (debug >> cont, (ty1, ty2, input, mapping, loopVal, body, ranges))

    | _ -> ValueNone

[<return: Struct>]
let (|Array|_|) g (OptionalCoerce expr) =
    if isArray1DTy g (tyOfExpr g expr) then ValueSome expr
    else ValueNone

[<return: Struct>]
let (|List|_|) g (OptionalCoerce expr) =
    if isListTy g (tyOfExpr g expr) then ValueSome expr
    else ValueNone

let LowerComputedListOrArrayExpr tcVal (g: TcGlobals) amap ilTyForTy overallExpr =
    // If ListCollector is in FSharp.Core then this optimization kicks in
    if g.ListCollector_tcr.CanDeref then
        match overallExpr with
        // […]
        | SeqToList g (OptionalCoerce (OptionalSeq g amap (overallSeqExpr, overallElemTy)), m) ->
            match overallSeqExpr with
            // [for … in xs -> …] (* When xs is a list. *)
            | SimpleMapping g (cont, (_, _, List g list, _, loopVal, body, ranges)) when
                g.langVersion.SupportsFeature LanguageFeature.LowerSimpleMappingsInComprehensionsToFastLoops
                ->
                Some (cont (List.mkMap tcVal g amap m ranges list overallElemTy loopVal body))

            // [start..finish]
            // [start..step..finish]
            | IntegralRange g (rangeTy, (start, step, finish)) when
                g.langVersion.SupportsFeature LanguageFeature.LowerIntegralRangesToFastLoops
                ->
                let ranges = m, DebugPointAtBinding.NoneAtInvisible, DebugPointAtInOrTo.No, m, m, DebugPointAtWhile.No
                Some (List.mkFromIntegralRange tcVal g amap m ranges rangeTy overallElemTy overallSeqExpr start step finish None)

            // [for … in start..finish -> …]
            // [for … in start..step..finish -> …]
            | SimpleMapping g (cont, (_, _, DebugPoints (rangeExpr & IntegralRange g (rangeTy, (start, step, finish)), debug), _, loopVal, body, ranges)) when
                g.langVersion.SupportsFeature LanguageFeature.LowerIntegralRangesToFastLoops
                ->
                Some (cont (debug (List.mkFromIntegralRange tcVal g amap m ranges rangeTy overallElemTy rangeExpr start step finish (Some (loopVal, body)))))

            // [(* Anything more complex. *)]
            | _ ->
                let collectorTy = g.mk_ListCollector_ty overallElemTy
                LowerComputedListOrArraySeqExpr tcVal g amap m collectorTy overallSeqExpr

        // [|…|]
        | SeqToArray g (OptionalCoerce (OptionalSeq g amap (overallSeqExpr, overallElemTy)), m) ->
            match overallSeqExpr with
            // [|for … in xs -> …|] (* When xs is an array. *)
            | SimpleMapping g (cont, (ty1, ty2, Array g array, _, loopVal, body, ranges)) when
                g.langVersion.SupportsFeature LanguageFeature.LowerSimpleMappingsInComprehensionsToFastLoops
                ->
                Some (cont (Array.mkMap g m ranges array (ilTyForTy ty1) (ilTyForTy ty2) overallElemTy loopVal body))

            // [|start..finish|]
            // [|start..step..finish|]
            | IntegralRange g (rangeTy, (start, step, finish)) when
                g.langVersion.SupportsFeature LanguageFeature.LowerIntegralRangesToFastLoops
                ->
                let ranges = m, DebugPointAtBinding.NoneAtInvisible, DebugPointAtInOrTo.No, m, m, DebugPointAtWhile.No
                Some (Array.mkFromIntegralRange g m ranges rangeTy (ilTyForTy overallElemTy) overallElemTy overallSeqExpr start step finish None)

            // [|for … in start..finish -> …|]
            // [|for … in start..step..finish -> …|]
            | SimpleMapping g (cont, (_, _, DebugPoints (rangeExpr & IntegralRange g (rangeTy, (start, step, finish)), debug), _, loopVal, body, ranges)) when
                g.langVersion.SupportsFeature LanguageFeature.LowerIntegralRangesToFastLoops
                ->
                Some (cont (debug (Array.mkFromIntegralRange g m ranges rangeTy (ilTyForTy overallElemTy) overallElemTy rangeExpr start step finish (Some (loopVal, body)))))

            // [|(* Anything more complex. *)|]
            | _ ->
                let collectorTy = g.mk_ArrayCollector_ty overallElemTy
                LowerComputedListOrArraySeqExpr tcVal g amap m collectorTy overallSeqExpr

        | _ -> None
    else
        None
