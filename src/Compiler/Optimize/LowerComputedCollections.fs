// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerComputedCollectionExpressions

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.InfoReader
open FSharp.Compiler.LowerSequenceExpressions
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypeRelations
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeHierarchy

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
        let disposeObjVar, disposeObjExpr = mkCompGenLocal m "objectToDispose" g.system_IDisposable_ty
        let disposeExpr, _ = BuildMethodCall tcVal g infoReader.amap PossiblyMutates m false disposeMethod NormalValUse [] [disposeObjExpr] [] None
        let inputExpr = mkCoerceExpr(exprForVal v.Range v, g.obj_ty, m, v.Type)
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

[<RequireQualifiedAccess>]
module IntegralConst =
    /// Constant 0.
    [<return: Struct>]
    let (|Zero|_|) expr =
        match expr with
        | Const.Zero
        | Const.Int32 0
        | Const.Int64 0L
        | Const.UInt64 0UL
        | Const.UInt32 0u
        | Const.IntPtr 0L
        | Const.UIntPtr 0UL
        | Const.Int16 0s
        | Const.UInt16 0us
        | Const.SByte 0y
        | Const.Byte 0uy
        | Const.Char '\000' -> ValueSome Zero
        | _ -> ValueNone

    /// Constant 1.
    [<return: Struct>]
    let (|One|_|) expr =
        match expr with
        | Const.Int32 1
        | Const.Int64 1L
        | Const.UInt64 1UL
        | Const.UInt32 1u
        | Const.IntPtr 1L
        | Const.UIntPtr 1UL
        | Const.Int16 1s
        | Const.UInt16 1us
        | Const.SByte 1y
        | Const.Byte 1uy
        | Const.Char '\001' -> ValueSome One
        | _ -> ValueNone

    /// Constant -1.
    [<return: Struct>]
    let (|MinusOne|_|) expr =
        match expr with
        | Const.Int32 -1
        | Const.Int64 -1L
        | Const.IntPtr -1L
        | Const.Int16 -1s
        | Const.SByte -1y -> ValueSome MinusOne
        | _ -> ValueNone

/// Note: this assumes that an empty range has already been checked for
/// (otherwise the conversion operations here might overflow).
[<return: Struct>]
let (|ConstCount|_|) (start, step, finish) =
    match start, step, finish with
    | Expr.Const (value = Const.Int32 start), Expr.Const (value = Const.Int32 step), Expr.Const (value = Const.Int32 finish) -> ValueSome (Const.Int32 ((finish - start) / step + 1))
    | Expr.Const (value = Const.Int64 start), Expr.Const (value = Const.Int64 step), Expr.Const (value = Const.Int64 finish) -> ValueSome (Const.Int64 ((finish - start) / step + 1L))
    | Expr.Const (value = Const.UInt64 start), Expr.Const (value = Const.UInt64 step), Expr.Const (value = Const.UInt64 finish) -> ValueSome (Const.UInt64 ((finish - start) / step + 1UL))
    | Expr.Const (value = Const.UInt32 start), Expr.Const (value = Const.UInt32 step), Expr.Const (value = Const.UInt32 finish) -> ValueSome (Const.UInt32 ((finish - start) / step + 1u))
    | Expr.Const (value = Const.IntPtr start), Expr.Const (value = Const.IntPtr step), Expr.Const (value = Const.IntPtr finish) -> ValueSome (Const.IntPtr ((finish - start) / step + 1L))
    | Expr.Const (value = Const.UIntPtr start), Expr.Const (value = Const.UIntPtr step), Expr.Const (value = Const.UIntPtr finish) -> ValueSome (Const.UIntPtr ((finish - start) / step + 1UL))
    | Expr.Const (value = Const.Int16 start), Expr.Const (value = Const.Int16 step), Expr.Const (value = Const.Int16 finish) -> ValueSome (Const.Int16 ((finish - start) / step + 1s))
    | Expr.Const (value = Const.UInt16 start), Expr.Const (value = Const.UInt16 step), Expr.Const (value = Const.UInt16 finish) -> ValueSome (Const.UInt16 ((finish - start) / step + 1us))
    | Expr.Const (value = Const.SByte start), Expr.Const (value = Const.SByte step), Expr.Const (value = Const.SByte finish) -> ValueSome (Const.SByte ((finish - start) / step + 1y))
    | Expr.Const (value = Const.Byte start), Expr.Const (value = Const.Byte step), Expr.Const (value = Const.Byte finish) -> ValueSome (Const.Byte ((finish - start) / step + 1uy))
    | Expr.Const (value = Const.Char start), Expr.Const (value = Const.Char step), Expr.Const (value = Const.Char finish) -> ValueSome (Const.Char (char (uint16 (finish - start) / uint16 step) + '\001'))
    | _ -> ValueNone

/// Bind start, step, and finish exprs to local variables if needed, e.g.,
///
///     [start..finishExpr] → let finish = finishExpr in …
///
///     [startExpr..finish] → let start = startExpr in …
///
///     [startExpr..finishExpr] → let start = startExpr in let finish = finishExpr in …
let mkLetBindingsIfNeeded m ty start step finish mkInitExpr =
    match start, step, finish with
    | (Expr.Const _ | Expr.Val _), (Expr.Const _ | Expr.Val _), (Expr.Const _ | Expr.Val _) ->
        mkInitExpr start step finish

    | (Expr.Const _ | Expr.Val _), (Expr.Const _ | Expr.Val _), _ ->
        mkCompGenLetIn m (nameof finish) ty finish (fun (_, finish) ->
            mkInitExpr start step finish)

    | _, (Expr.Const _ | Expr.Val _), (Expr.Const _ | Expr.Val _) ->
        mkCompGenLetIn m (nameof start) ty start (fun (_, start) ->
            mkInitExpr start step finish)

    | (Expr.Const _ | Expr.Val _), _, (Expr.Const _ | Expr.Val _) ->
        mkCompGenLetIn m (nameof step) ty step (fun (_, step) ->
            mkInitExpr start step finish)

    | _, (Expr.Const _ | Expr.Val _), _ ->
        mkCompGenLetIn m (nameof start) ty start (fun (_, start) ->
            mkCompGenLetIn m (nameof finish) ty finish (fun (_, finish) ->
                mkInitExpr start step finish))

    | (Expr.Const _ | Expr.Val _), _, _ ->
        mkCompGenLetIn m (nameof step) ty step (fun (_, step) ->
            mkCompGenLetIn m (nameof finish) ty finish (fun (_, finish) ->
                mkInitExpr start step finish))

    | _, _, (Expr.Const _ | Expr.Val _) ->
        mkCompGenLetIn m (nameof start) ty start (fun (_, start) ->
            mkCompGenLetIn m (nameof step) ty step (fun (_, step) ->
                mkInitExpr start step finish))

    | _, _, _ ->
        mkCompGenLetIn m (nameof start) ty start (fun (_, start) ->
            mkCompGenLetIn m (nameof step) ty step (fun (_, step) ->
                mkCompGenLetIn m (nameof finish) ty finish (fun (_, finish) ->
                    mkInitExpr start step finish)))

module List =
    /// Makes an expression that will build a list from an integral range.
    /// Expects a constant zero step to have already been checked for.
    let mkFromIntegralRange tcVal g amap m overallElemTy overallSeqExpr start step finish =
        match start, step, finish with
        // [5..1] → []
        // [1..-1..5] → []
        | EmptyRange -> mkNil g m overallElemTy

        // [1..5]
        // [1..2..5]
        // [start..finish]
        // [start..step..finish]
        | _, _, _ ->
            let collectorTy = g.mk_ListCollector_ty overallElemTy

            mkLetBindingsIfNeeded m overallElemTy start step finish (fun start step finish ->
                mkCompGenLetMutableIn m "collector" collectorTy (mkDefault (m, collectorTy)) (fun (_, collector) ->
                    mkCompGenLetMutableIn m "loopVar" overallElemTy start (fun (loopVarVal, loopVar) ->
                        let reader = InfoReader (g, amap)

                        let body = mkCallCollectorAdd tcVal g reader m collector loopVar

                        let loop =
                            mkOptimizedRangeLoop
                                g
                                (m, m, m, DebugPointAtWhile.No)
                                (overallElemTy, overallSeqExpr)
                                (start, step, finish)
                                (loopVarVal, loopVar)
                                body

                        let close = mkCallCollectorClose tcVal g reader m collector

                        mkSequential m loop close
                    )
                )
            )

module Array =
    /// Makes an expression that will build an array from an integral range.
    /// Expects a constant zero step to have already been checked for.
    let mkFromIntegralRange g m overallElemTy overallSeqExpr start step finish =
        let arrayTy = mkArrayType g overallElemTy

        /// Triggers an overflow exception at runtime if count doesn't fit in a native int.
        let convToNativeIntWithOverflow m overallElemTy count =
            if typeEquiv g overallElemTy g.int64_ty then mkAsmExpr ([AI_conv_ovf DT_I], [], [count], [g.nativeint_ty], m)
            elif typeEquiv g overallElemTy g.uint64_ty then mkAsmExpr ([AI_conv_ovf_un DT_I], [], [count], [g.nativeint_ty], m)
            else count

        let mkInitializer count start step =
            let ilTy =
                let ty = stripMeasuresFromTy g overallElemTy
                if typeEquiv g ty g.int32_ty then g.ilg.typ_Int32
                elif typeEquiv g ty g.int64_ty then g.ilg.typ_Int64
                elif typeEquiv g ty g.uint64_ty then g.ilg.typ_UInt64
                elif typeEquiv g ty g.uint32_ty then g.ilg.typ_UInt32
                elif typeEquiv g ty g.nativeint_ty then g.ilg.typ_IntPtr
                elif typeEquiv g ty g.unativeint_ty then g.ilg.typ_UIntPtr
                elif typeEquiv g ty g.int16_ty then g.ilg.typ_Int16
                elif typeEquiv g ty g.uint16_ty then g.ilg.typ_UInt16
                elif typeEquiv g ty g.sbyte_ty then g.ilg.typ_SByte
                elif typeEquiv g ty g.byte_ty then g.ilg.typ_Byte
                elif typeEquiv g ty g.char_ty then g.ilg.typ_Char
                else error(InternalError($"Unrecognized integral type '{ty}'.", m))

            /// (# "newarr !0" type ('T) count : 'T array #)
            let mkNewArray count =
                mkAsmExpr
                    (
                        [I_newarr (ILArrayShape.SingleDimensional, ilTy)],
                        [],
                        [convToNativeIntWithOverflow m overallElemTy count],
                        [arrayTy],
                        m
                    )

            mkCompGenLetIn m "array" arrayTy (mkNewArray count) (fun (_, array) ->
                mkCompGenLetMutableIn m "i" g.int32_ty (mkZero g m) (fun (iVal, i) ->
                    mkCompGenLetMutableIn m "loopVar" overallElemTy start (fun (loopVarVal, loopVar) ->
                        // array[i] <- loopVar
                        let setArrSubI = mkAsmExpr ([I_stelem_any (ILArrayShape.SingleDimensional, ilTy)], [], [array; i; loopVar], [], m)

                        // loopVar <- loopVar + step
                        let incrV = mkValSet m (mkLocalValRef loopVarVal) (mkAsmExpr ([AI_add], [], [loopVar; step], [overallElemTy], m))

                        // i <- i + 1
                        let incrI = mkValSet m (mkLocalValRef iVal) (mkAsmExpr ([AI_add], [], [i; mkOne g m], [g.int32_ty], m))

                        let body = mkSequentials g m [setArrSubI; incrV; incrI]

                        let guard = mkILAsmClt g m i (mkLdlen g m array)

                        let loop =
                            mkWhile
                                g
                                (
                                    DebugPointAtWhile.No,
                                    NoSpecialWhileLoopMarker,
                                    guard,
                                    body,
                                    m
                                )

                        // while i < array.Length do <body> done
                        // array
                        mkSequential m loop array
                    )
                )
            )

        match start, step, finish with
        // [|5..1|] → [||]
        // [|1..-1..5|] → [||]
        | EmptyRange -> mkArray (overallElemTy, [], m)

        // [|1..5|]
        // [|1..2..5|]
        | ConstCount count -> mkInitializer (Expr.Const (count, m, overallElemTy)) start step

        // [|start..finish|]
        // [|start..step..finish|]
        | _, _, _ ->
            /// Make an expression holding the size of the array.
            let mkCount m rangeExpr ty start step finish =
                match step with
                // step = 1:
                //     finish - start + 1
                | Expr.Const (value = IntegralConst.One) ->
                    let diff = mkAsmExpr ([AI_sub], [], [finish; start], [ty], m)
                    mkAsmExpr ([AI_add], [], [diff; mkOne g m], [ty], m)

                // step = -1:
                //     -(finish - start) + 1
                | Expr.Const (value = IntegralConst.MinusOne) ->
                    let diff = mkAsmExpr ([AI_neg], [], [mkAsmExpr ([AI_sub], [], [finish; start], [ty], m)], [ty], m)
                    mkAsmExpr ([AI_add], [], [diff; mkOne g m], [ty], m)

                // step = <const>:
                //     (finish - start) / step + 1
                | Expr.Const (value = _notOneOrZeroOrMinusOne) ->
                    let diff = mkAsmExpr ([AI_sub], [], [finish; start], [ty], m)

                    let quotient =
                        if isSignedIntegerTy g ty then
                            mkAsmExpr ([AI_div], [], [diff; step], [ty], m)
                        else
                            mkAsmExpr ([AI_div_un], [], [diff; step], [ty], m)

                    mkAsmExpr ([AI_add], [], [quotient; mkOne g m], [ty], m)

                // Arbitrary step:
                //     (finish - start) / step + 1
                | _notConst ->
                    // Use the potentially-evaluated-and-bound start, step, and finish.
                    let rangeExpr =
                        match rangeExpr with
                        | Expr.App (funcExpr, formalType, tyargs, _, m) -> Expr.App (funcExpr, formalType, tyargs, [start; step; finish], m)
                        | _ -> rangeExpr

                    /// This will raise an exception at runtime if step is zero.
                    let callAndIgnoreRangeExpr =
                        mkSequential
                            m
                            rangeExpr
                            (mkUnit g m)

                    // Let the range call throw the appropriate localized
                    // exception at runtime if step is zero:
                    //     if step = 0 then (.. ..) start step finish
                    let throwIfStepIsZero =
                        mkCond
                            DebugPointAtBinding.NoneAtInvisible
                            m
                            g.unit_ty
                            (mkILAsmCeq g m step (mkZero g m))
                            callAndIgnoreRangeExpr
                            (mkUnit g m)

                    let count =
                        let diff = mkAsmExpr ([AI_sub], [], [finish; start], [ty], m)

                        let quotient =
                            if isSignedIntegerTy g ty then
                                mkAsmExpr ([AI_div], [], [diff; step], [ty], m)
                            else
                                mkAsmExpr ([AI_div_un], [], [diff; step], [ty], m)

                        mkAsmExpr ([AI_add], [], [quotient; mkOne g m], [ty], m)

                    mkSequential m throwIfStepIsZero count

            mkLetBindingsIfNeeded m overallElemTy start step finish (fun start step finish ->
                mkCompGenLetIn m "count" overallElemTy (mkCount m overallSeqExpr overallElemTy start step finish) (fun (_, count) ->
                    let mkSignednessAppropriateClt g m ty e1 e2 =
                        if isSignedIntegerTy g ty then
                            mkILAsmClt g m e1 e2
                        else
                            mkAsmExpr ([AI_clt_un], [], [e1; e2], [g.bool_ty], m)

                    // count < 1
                    let countLtOne = mkSignednessAppropriateClt g m overallElemTy count (mkOne g m)

                    // [||]
                    let empty = mkArray (overallElemTy, [], m)

                    let initialize = mkInitializer count start step

                    // if count < 1 then [||] else <initialize>
                    mkCond
                        DebugPointAtBinding.NoneAtInvisible
                        m
                        arrayTy
                        countLtOne
                        empty
                        initialize
                )
            )

let LowerComputedListOrArrayExpr tcVal (g: TcGlobals) amap overallExpr =
    // If ListCollector is in FSharp.Core then this optimization kicks in
    if g.ListCollector_tcr.CanDeref then
        match overallExpr with
        // […]
        | SeqToList g (OptionalCoerce (OptionalSeq g amap (overallSeqExpr, overallElemTy)), m) ->
            match overallSeqExpr with
            // [start..0..finish] → let the default implementation raise an exception.
            | IntegralRange g (_, (_, Expr.Const (value = IntegralConst.Zero), _)) ->
                let collectorTy = g.mk_ListCollector_ty overallElemTy
                LowerComputedListOrArraySeqExpr tcVal g amap m collectorTy overallSeqExpr
                
            // [start..finish]
            // [start..step..finish]
            | IntegralRange g (_, (start, step, finish)) ->
                Some (List.mkFromIntegralRange tcVal g amap m overallElemTy overallSeqExpr start step finish)

            // [(* Anything more complex. *)]
            | _ ->
                let collectorTy = g.mk_ListCollector_ty overallElemTy
                LowerComputedListOrArraySeqExpr tcVal g amap m collectorTy overallSeqExpr

        // [|…|]
        | SeqToArray g (OptionalCoerce (OptionalSeq g amap (overallSeqExpr, overallElemTy)), m) ->
            match overallSeqExpr with
            // [|start..0..finish|] → let the default implementation raise an exception.
            | IntegralRange g (_, (_, Expr.Const (value = IntegralConst.Zero), _)) ->
                let collectorTy = g.mk_ArrayCollector_ty overallElemTy
                LowerComputedListOrArraySeqExpr tcVal g amap m collectorTy overallSeqExpr

            // [|start..finish|]
            // [|start..step..finish|]
            | IntegralRange g (_, (start, step, finish)) ->
                Some (Array.mkFromIntegralRange g m overallElemTy overallSeqExpr start step finish)

            // [|(* Anything more complex. *)|]
            | _ ->
                let collectorTy = g.mk_ArrayCollector_ty overallElemTy
                LowerComputedListOrArraySeqExpr tcVal g amap m collectorTy overallSeqExpr

        | _ -> None
    else
        None
