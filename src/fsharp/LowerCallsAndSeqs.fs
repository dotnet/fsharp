// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerCallsAndSeqs

open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.Internal
open FSharp.Compiler.AbstractIL.Internal.Library

open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.Ast
open FSharp.Compiler.Infos
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Tast
open FSharp.Compiler.Tastops
open FSharp.Compiler.Lib
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.InfoReader
open FSharp.Compiler.MethodCalls

//----------------------------------------------------------------------------
// Eta-expansion of calls to top-level-methods

let InterceptExpr g cont expr =

    match expr with
    | Expr.Val (vref, flags, m) ->
        match vref.ValReprInfo with
        | Some arity -> Some (fst (AdjustValForExpectedArity g m vref flags arity))
        | None -> None

    // App (Val v, tys, args)
    | Expr.App ((Expr.Val (vref, flags, _) as f0), f0ty, tyargsl, argsl, m) ->
        // Only transform if necessary, i.e. there are not enough arguments
        match vref.ValReprInfo with
        | Some(topValInfo) ->
            let argsl = List.map cont argsl
            let f0 =
                if topValInfo.AritiesOfArgs.Length > argsl.Length
                then fst(AdjustValForExpectedArity g m vref flags topValInfo)
                else f0

            Some (MakeApplicationAndBetaReduce g (f0, f0ty, [tyargsl], argsl, m))
        | None -> None

    | Expr.App (f0, f0ty, tyargsl, argsl, m) ->
        Some (MakeApplicationAndBetaReduce g (f0, f0ty, [tyargsl], argsl, m) )

    | _ -> None

/// An "expr -> expr" pass that eta-expands under-applied values of
/// known arity to lambda expressions and beta-var-reduces to bind
/// any known arguments.  The results are later optimized by the peephole
/// optimizer in opt.fs
let LowerImplFile g assembly =
    RewriteImplFile { PreIntercept = Some(InterceptExpr g)
                      PreInterceptBinding=None
                      PostTransform= (fun _ -> None)
                      IsUnderQuotations=false } assembly


//----------------------------------------------------------------------------
// State machine compilation for sequence expressions

let mkLambdaNoType g m uv e =
    mkLambda m uv (e, tyOfExpr g e)

let mkUnitDelayLambda (g: TcGlobals) m e =
    let uv, _ue = mkCompGenLocal m "unitVar" g.unit_ty
    mkLambdaNoType g m uv e

let callNonOverloadedMethod g amap m methName ty args =
    match TryFindIntrinsicMethInfo (InfoReader(g, amap)) m AccessibleFromSomeFSharpCode methName ty  with
    | [] -> error(InternalError("No method called '"+methName+"' was found", m))
    | ILMeth(g, ilMethInfo, _) :: _  ->
        // REVIEW: consider if this should ever be a constrained call. At the moment typecheck limitations in the F# typechecker
        // ensure the enumerator type used within computation expressions is not a struct type
        BuildILMethInfoCall g amap m false ilMethInfo NormalValUse  [] false args |> fst
    | _  ->
        error(InternalError("The method called '"+methName+"' resolved to a non-IL type", m))


type LoweredSeqFirstPhaseResult =
   { /// The code to run in the second phase, to rebuild the expressions, once all code labels and their mapping to program counters have been determined
     /// 'nextVar' is the argument variable for the GenerateNext method that represents the byref argument that holds the "goto" destination for a tailcalling sequence expression
     phase2 : ((* pc: *) ValRef * (* current: *) ValRef * (* nextVar: *) ValRef * Map<ILCodeLabel, int> -> Expr * Expr * Expr)

     /// The labels allocated for one portion of the sequence expression
     labels : int list

     /// any actual work done in Close
     significantClose : bool

     /// The state variables allocated for one portion of the sequence expression (i.e. the local let-bound variables which become state variables)
     stateVars: ValRef list

     /// The vars captured by the non-synchronous path
     capturedVars: FreeVars }

let isVarFreeInExpr v e = Zset.contains v (freeInExpr CollectTyparsAndLocals e).FreeLocals

/// Analyze a TAST expression to detect the elaborated form of a sequence expression.
/// Then compile it to a state machine represented as a TAST containing goto, return and label nodes.
/// The returned state machine will also contain references to state variables (from internal 'let' bindings),
/// a program counter (pc) that records the current state, and a current generated value (current).
/// All these variables are then represented as fields in a hosting closure object along with any additional
/// free variables of the sequence expression.
///
/// The analysis is done in two phases. The first phase determines the state variables and state labels (as Abstract IL code labels).
/// We then allocate an integer pc for each state label and proceed with the second phase, which builds two related state machine
/// expressions: one for 'MoveNext' and one for 'Dispose'.
let LowerSeqExpr g amap overallExpr =
    /// Detect a 'yield x' within a 'seq { ... }'
    let (|SeqYield|_|) expr =
        match expr with
        | Expr.App (Expr.Val (vref, _, _), _f0ty, _tyargsl, [arg], m) when valRefEq g vref g.seq_singleton_vref ->
            Some (arg, m)
        | _ ->
            None

    /// Detect a 'expr; expr' within a 'seq { ... }'
    let (|SeqAppend|_|) expr =
        match expr with
        | Expr.App (Expr.Val (vref, _, _), _f0ty, _tyargsl, [arg1;arg2], m) when valRefEq g vref g.seq_append_vref ->
            Some (arg1, arg2, m)
        | _ ->
            None

    /// Detect a 'while gd do expr' within a 'seq { ... }'
    let (|SeqWhile|_|) expr =
        match expr with
        | Expr.App (Expr.Val (vref, _, _), _f0ty, _tyargsl, [Expr.Lambda (_, _, _, [dummyv], gd, _, _);arg2], m)
             when valRefEq g vref g.seq_generated_vref &&
                  not (isVarFreeInExpr dummyv gd) ->
            Some (gd, arg2, m)
        | _ ->
            None

    let (|SeqTryFinally|_|) expr =
        match expr with
        | Expr.App (Expr.Val (vref, _, _), _f0ty, _tyargsl, [arg1;Expr.Lambda (_, _, _, [dummyv], compensation, _, _)], m)
            when valRefEq g vref g.seq_finally_vref &&
                 not (isVarFreeInExpr dummyv compensation) ->
            Some (arg1, compensation, m)
        | _ ->
            None

    let (|SeqUsing|_|) expr =
        match expr with
        | Expr.App (Expr.Val (vref, _, _), _f0ty, [_;_;elemTy], [resource;Expr.Lambda (_, _, _, [v], body, _, _)], m)
            when valRefEq g vref g.seq_using_vref ->
            Some (resource, v, body, elemTy, m)
        | _ ->
            None

    let (|SeqFor|_|) expr =
        match expr with
        // Nested for loops are represented by calls to Seq.collect
        | Expr.App (Expr.Val (vref, _, _), _f0ty, [_inpElemTy;_enumty2;genElemTy], [Expr.Lambda (_, _, _, [v], body, _, _); inp], m) when valRefEq g vref g.seq_collect_vref ->
            Some (inp, v, body, genElemTy, m)
        // "for x in e -> e2" is converted to a call to Seq.map by the F# type checker. This could be removed, except it is also visible in F# quotations.
        | Expr.App (Expr.Val (vref, _, _), _f0ty, [_inpElemTy;genElemTy], [Expr.Lambda (_, _, _, [v], body, _, _); inp], m) when valRefEq g vref g.seq_map_vref ->
            Some (inp, v, mkCallSeqSingleton g body.Range genElemTy body, genElemTy, m)
        | _ -> None

    let (|SeqDelay|_|) expr =
        match expr with
        | Expr.App (Expr.Val (vref, _, _), _f0ty, [elemTy], [Expr.Lambda (_, _, _, [v], e, _, _)], _m) when valRefEq g vref g.seq_delay_vref && not (isVarFreeInExpr v e) ->  Some (e, elemTy)
        | _ -> None

    let (|SeqEmpty|_|) expr =
        match expr with
        | Expr.App (Expr.Val (vref, _, _), _f0ty, _tyargsl, [], m) when valRefEq g vref g.seq_empty_vref ->  Some (m)
        | _ -> None

    let (|Seq|_|) expr =
        match expr with
        // use 'seq { ... }' as an indicator
        | Expr.App (Expr.Val (vref, _, _), _f0ty, [elemTy], [e], _m) when valRefEq g vref g.seq_vref ->  Some (e, elemTy)
        | _ -> None

    let RepresentBindingAsLocal (bind: Binding) res2 m =
        // printfn "found letrec state variable %s" bind.Var.DisplayName
        { res2 with
            phase2 = (fun ctxt ->
                let generate2, dispose2, checkDispose2 = res2.phase2 ctxt
                let generate = mkLetBind m bind generate2
                let dispose = dispose2
                let checkDispose = checkDispose2
                generate, dispose, checkDispose)
            stateVars = res2.stateVars }

    let RepresentBindingAsStateMachineLocal (bind: Binding) res2 m =
        // printfn "found letrec state variable %s" bind.Var.DisplayName
        let (TBind(v, e, sp)) = bind
        let sp, spm =
            match sp with
            | SequencePointAtBinding m -> SequencePointsAtSeq, m
            | _ -> SuppressSequencePointOnExprOfSequential, e.Range
        let vref = mkLocalValRef v
        { res2 with
            phase2 = (fun ctxt ->
                let generate2, dispose2, checkDispose2 = res2.phase2 ctxt
                let generate =
                    mkCompGenSequential m
                        (mkSequential sp m
                            (mkValSet spm vref e)
                            generate2)
                        // zero out the current value to free up its memory
                        (mkValSet m vref (mkDefault (m, vref.Type)))
                let dispose = dispose2
                let checkDispose = checkDispose2
                generate, dispose, checkDispose)
            stateVars = vref :: res2.stateVars }

    let RepresentBindingsAsLifted mkBinds res2 =
        // printfn "found top level let  "
        { res2 with
            phase2 = (fun ctxt ->
                let generate2, dispose2, checkDispose2 = res2.phase2 ctxt
                let generate = mkBinds generate2
                let dispose = dispose2
                let checkDispose = checkDispose2
                generate, dispose, checkDispose) }

    let rec Lower
                 isWholeExpr
                 isTailCall // is this sequence in tailcall position?
                 noDisposeContinuationLabel // represents the label for the code where there is effectively nothing to do to dispose the iterator for the current state
                 currentDisposeContinuationLabel // represents the label for the code we have to run to dispose the iterator given the current state
                 expr =

        match expr with
        | SeqYield(e, m) ->
            // printfn "found Seq.singleton"
                 //this.pc <- NEXT
                 //curr <- e
                 //return true
                 //NEXT:
            let label = IL.generateCodeLabel()
            Some { phase2 = (fun (pcv, currv, _nextv, pcMap) ->
                        let generate =
                            mkCompGenSequential m
                                (mkValSet m pcv (mkInt32 g m pcMap.[label]))
                                (mkSequential SequencePointsAtSeq m
                                    (mkValSet m currv e)
                                    (mkCompGenSequential m
                                        (Expr.Op (TOp.Return, [], [mkOne g m], m))
                                        (Expr.Op (TOp.Label label, [], [], m))))
                        let dispose =
                            mkCompGenSequential m
                                (Expr.Op (TOp.Label label, [], [], m))
                                (Expr.Op (TOp.Goto currentDisposeContinuationLabel, [], [], m))
                        let checkDispose =
                            mkCompGenSequential m
                                (Expr.Op (TOp.Label label, [], [], m))
                                (Expr.Op (TOp.Return, [], [mkBool g m (not (noDisposeContinuationLabel = currentDisposeContinuationLabel))], m))
                        generate, dispose, checkDispose)
                   labels=[label]
                   stateVars=[]
                   significantClose = false
                   capturedVars = emptyFreeVars
                  }

        | SeqDelay(delayedExpr, _elemTy) ->
            // printfn "found Seq.delay"
            // note, using 'isWholeExpr' here prevents 'seq { yield! e }' and 'seq { 0 .. 1000 }' from being compiled
            Lower isWholeExpr isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel delayedExpr

        | SeqAppend(e1, e2, m) ->
            // printfn "found Seq.append"
            let res1 = Lower false false noDisposeContinuationLabel currentDisposeContinuationLabel e1
            let res2 = Lower false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel e2
            match res1, res2 with
            | Some res1, Some res2 ->

                let capturedVars =
                    if res1.labels.IsEmpty then
                        res2.capturedVars
                    else
                        // All of 'e2' is needed after resuming at any of the labels
                        unionFreeVars res1.capturedVars (freeInExpr CollectLocals e2)

                Some { phase2 = (fun ctxt ->
                            let generate1, dispose1, checkDispose1 = res1.phase2 ctxt
                            let generate2, dispose2, checkDispose2 = res2.phase2 ctxt
                            let generate = mkCompGenSequential m generate1 generate2
                            // Order shouldn't matter here, since disposals actions are linked together by goto's  (each ends in a goto).
                            // However leaving as is for now.
                            let dispose = mkCompGenSequential m dispose2 dispose1
                            let checkDispose = mkCompGenSequential m checkDispose2 checkDispose1
                            generate, dispose, checkDispose)
                       labels= res1.labels @ res2.labels
                       stateVars = res1.stateVars @ res2.stateVars
                       significantClose = res1.significantClose || res2.significantClose
                       capturedVars = capturedVars }
            | _ ->
                None

        | SeqWhile(guardExpr, bodyExpr, m) ->
            // printfn "found Seq.while"
            let resBody = Lower false false noDisposeContinuationLabel currentDisposeContinuationLabel bodyExpr
            match resBody with
            | Some res2  ->
                let capturedVars =
                    if res2.labels.IsEmpty then
                        res2.capturedVars  // the whole loopis synchronous, no labels
                    else
                        freeInExpr CollectLocals expr // everything is needed on subsequent iterations

                Some { phase2 = (fun ctxt ->
                            let generate2, dispose2, checkDispose2 = res2.phase2 ctxt
                            let generate = mkWhile g (SequencePointAtWhileLoop guardExpr.Range, NoSpecialWhileLoopMarker, guardExpr, generate2, m)
                            let dispose = dispose2
                            let checkDispose = checkDispose2
                            generate, dispose, checkDispose)
                       labels = res2.labels
                       stateVars = res2.stateVars
                       significantClose = res2.significantClose
                       capturedVars = capturedVars }
            | _ ->
                None

        | SeqUsing(resource, v, body, elemTy, m) ->
            // printfn "found Seq.using"
            let reduction =
                mkLet (SequencePointAtBinding body.Range) m v resource
                    (mkCallSeqFinally g m elemTy body
                        (mkUnitDelayLambda g m
                            (mkCallDispose g m v.Type (exprForVal m v))))
            Lower false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel reduction

        | SeqFor(inp, v, body, genElemTy, m) ->
            // printfn "found Seq.for"
            let inpElemTy = v.Type
            let inpEnumTy = mkIEnumeratorTy g inpElemTy
            let enumv, enume = mkCompGenLocal m "enum" inpEnumTy
            // [[ use enum = inp.GetEnumerator()
            //    while enum.MoveNext() do
            //       let v = enum.Current
            //       body ]]
            let reduction =
                mkCallSeqUsing g m inpEnumTy genElemTy (callNonOverloadedMethod g amap m "GetEnumerator" (mkSeqTy g inpElemTy) [inp])
                    (mkLambdaNoType g m enumv
                       (mkCallSeqGenerated g m genElemTy (mkUnitDelayLambda g m (callNonOverloadedMethod g amap m "MoveNext" inpEnumTy [enume]))
                          (mkInvisibleLet m v (callNonOverloadedMethod g amap m "get_Current" inpEnumTy [enume])
                              (mkCoerceIfNeeded g (mkSeqTy g genElemTy) (tyOfExpr g body) body))))
            Lower false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel reduction

        | SeqTryFinally(e1, compensation, m) ->
            // printfn "found Seq.try/finally"
            let innerDisposeContinuationLabel = IL.generateCodeLabel()
            let resBody = Lower false false noDisposeContinuationLabel innerDisposeContinuationLabel e1
            match resBody with
            | Some res1  ->
                let capturedVars = unionFreeVars res1.capturedVars (freeInExpr CollectLocals compensation)
                Some { phase2 = (fun ((pcv, _currv, _, pcMap) as ctxt) ->
                            let generate1, dispose1, checkDispose1 = res1.phase2 ctxt
                            let generate =
                                // copy the compensation expression - one copy for the success continuation and one for the exception
                                let compensation = copyExpr g CloneAllAndMarkExprValsAsCompilerGenerated compensation
                                mkCompGenSequential m
                                    // set the PC to the inner finally, so that if an exception happens we run the right finally
                                    (mkCompGenSequential m
                                        (mkValSet m pcv (mkInt32 g m pcMap.[innerDisposeContinuationLabel]))
                                        generate1 )
                                    // set the PC past the try/finally before trying to run it, to make sure we only run it once
                                    (mkCompGenSequential m
                                        (Expr.Op (TOp.Label innerDisposeContinuationLabel, [], [], m))
                                        (mkCompGenSequential m
                                            (mkValSet m pcv (mkInt32 g m pcMap.[currentDisposeContinuationLabel]))
                                            compensation))
                            let dispose =
                                // generate inner try/finallys, then outer try/finallys
                                mkCompGenSequential m
                                    dispose1
                                    // set the PC past the try/finally before trying to run it, to make sure we only run it once
                                    (mkCompGenSequential m
                                        (Expr.Op (TOp.Label innerDisposeContinuationLabel, [], [], m))
                                        (mkCompGenSequential m
                                            (mkValSet m pcv (mkInt32 g m pcMap.[currentDisposeContinuationLabel]))
                                            (mkCompGenSequential m
                                                compensation
                                                (Expr.Op (TOp.Goto currentDisposeContinuationLabel, [], [], m)))))
                            let checkDispose =
                                mkCompGenSequential m
                                    checkDispose1
                                    (mkCompGenSequential m
                                        (Expr.Op (TOp.Label innerDisposeContinuationLabel, [], [], m))
                                        (Expr.Op (TOp.Return, [], [mkTrue g m (* yes, we must dispose!!! *) ], m)))

                            generate, dispose, checkDispose)
                       labels = innerDisposeContinuationLabel :: res1.labels
                       stateVars = res1.stateVars
                       significantClose = true
                       capturedVars = capturedVars }
            | _ ->
                None

        | SeqEmpty m ->
            // printfn "found Seq.empty"
            Some { phase2 = (fun _ ->
                            let generate = mkUnit g  m
                            let dispose = Expr.Op (TOp.Goto currentDisposeContinuationLabel, [], [], m)
                            let checkDispose = Expr.Op (TOp.Goto currentDisposeContinuationLabel, [], [], m)
                            generate, dispose, checkDispose)
                   labels = []
                   stateVars = []
                   significantClose = false
                   capturedVars = emptyFreeVars }

        | Expr.Sequential (x1, x2, NormalSeq, ty, m) ->
            match Lower false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel x2 with
            | Some res2->
                // printfn "found sequential execution"
                Some { res2 with
                        phase2 = (fun ctxt ->
                            let generate2, dispose2, checkDispose2 = res2.phase2 ctxt
                            let generate = Expr.Sequential (x1, generate2, NormalSeq, ty, m)
                            let dispose = dispose2
                            let checkDispose = checkDispose2
                            generate, dispose, checkDispose) }
            | None -> None

        | Expr.Let (bind, bodyExpr, m, _)
              // Restriction: compilation of sequence expressions containing non-toplevel constrained generic functions is not supported
              when  bind.Var.IsCompiledAsTopLevel || not (IsGenericValWithGenericContraints g bind.Var) ->

            let resBody = Lower false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel bodyExpr
            match resBody with
            | Some res2 ->
                if bind.Var.IsCompiledAsTopLevel then
                    Some (RepresentBindingsAsLifted (mkLetBind m bind) res2)
                elif not (res2.capturedVars.FreeLocals.Contains(bind.Var)) then
                    // printfn "found state variable %s" bind.Var.DisplayName
                    Some (RepresentBindingAsLocal bind res2 m)
                else
                    // printfn "found state variable %s" bind.Var.DisplayName
                    Some (RepresentBindingAsStateMachineLocal bind res2 m)
            | None ->
                None

(*
        | Expr.LetRec (binds, e2, m, _)
              when  // Restriction: only limited forms of "let rec" in sequence expressions can be handled by assignment to state local values

                    (let recvars = valsOfBinds binds  |> List.map (fun v -> (v, 0)) |> ValMap.OfList
                     binds |> List.forall (fun bind ->
                         // Rule 1 - IsCompiledAsTopLevel require no state local value
                         bind.Var.IsCompiledAsTopLevel ||
                         // Rule 2 - funky constrained local funcs not allowed
                         not (IsGenericValWithGenericContraints g bind.Var)) &&
                     binds |> List.count (fun bind ->
                          // Rule 3 - Recursive non-lambda and repack values are allowed
                          match stripExpr bind.Expr with
                          | Expr.Lambda _
                          | Expr.TyLambda _ -> false
                          // "let v = otherv" bindings get produced for environment packing by InnerLambdasToTopLevelFuncs.fs, we can accept and compiler these ok
                          | Expr.Val (v, _, _) when not (recvars.ContainsVal v.Deref) -> false
                          | _ -> true) <= 1)  ->

            match Lower false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel e2 with
            | Some res2 ->
                let topLevelBinds, nonTopLevelBinds = binds |> List.partition (fun bind -> bind.Var.IsCompiledAsTopLevel)
                // Represent the closure-capturing values as state machine locals. They may still be recursively-referential
                let res3 = (res2, nonTopLevelBinds) ||> List.fold (fun acc bind -> RepresentBindingAsStateMachineLocal bind acc m)
                // Represent the non-closure-capturing values as ordinary bindings on the expression.
                let res4 = if topLevelBinds.IsEmpty then res3 else RepresentBindingsAsLifted (mkLetRecBinds m topLevelBinds) res3
                Some res4
            | None ->
                None
*)
        | Expr.Match (spBind, exprm, pt, targets, m, ty) when targets |> Array.forall (fun (TTarget(vs, _e, _spTarget)) -> isNil vs) ->
            // lower all the targets. abandon if any fail to lower
            let tglArray = targets |> Array.map (fun (TTarget(_vs, targetExpr, _spTarget)) -> Lower false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel targetExpr)
            // LIMITATION: non-trivial pattern matches involving or-patterns or active patterns where bindings can't be
            // transferred to the r.h.s. are not yet compiled.
            if tglArray |> Array.forall Option.isSome then
                let tglArray = Array.map Option.get tglArray
                let tgl = Array.toList tglArray
                let labs = tgl |> List.collect (fun res -> res.labels)
                let (capturedVars, _) =
                    ((emptyFreeVars, false), Array.zip targets tglArray)
                    ||> Array.fold (fun (fvs, seenLabel) ((TTarget(_vs, e, _spTarget)), res) ->
                        if seenLabel then unionFreeVars fvs (freeInExpr CollectLocals e), true
                        else res.capturedVars, not res.labels.IsEmpty)
                let stateVars = tgl |> List.collect (fun res -> res.stateVars)
                let significantClose = tgl |> List.exists (fun res -> res.significantClose)
                Some { phase2 = (fun ctxt ->
                            let gtgs, disposals, checkDisposes =
                                (Array.toList targets, tgl)
                                  ||> List.map2 (fun (TTarget(vs, _, spTarget)) res ->
                                        let generate, dispose, checkDispose = res.phase2 ctxt
                                        let gtg = TTarget(vs, generate, spTarget)
                                        gtg, dispose, checkDispose)
                                  |> List.unzip3
                            let generate = primMkMatch (spBind, exprm, pt, Array.ofList gtgs, m, ty)
                            let dispose = if isNil disposals then mkUnit g m else List.reduce (mkCompGenSequential m) disposals
                            let checkDispose = if isNil checkDisposes then mkFalse g m else List.reduce (mkCompGenSequential m) checkDisposes
                            generate, dispose, checkDispose)
                       labels=labs
                       stateVars = stateVars
                       significantClose = significantClose
                       capturedVars = capturedVars }
            else
                None

        // yield! e ---> (for x in e -> x)
        //
        // Design choice: we compile 'yield! e' as 'for x in e do yield x'.
        //
        // Note, however, this leads to a loss of tailcalls: the case not
        // handled correctly yet is sequence expressions that use yield! in the last position
        // This can give rise to infinite iterator chains when implemented by the naive expansion to
        // �for x in e yield e�. For example consider this:
        //
        // let rec rwalk x = {  yield x
        //                      yield! rwalk (x + rand()) }
        //
        // This is the moral equivalent of a tailcall optimization. These also don�t compile well
        // in the C# compilation model

        | arbitrarySeqExpr ->
            let m = arbitrarySeqExpr.Range
            if isWholeExpr then
                // printfn "FAILED - not worth compiling an unrecognized immediate yield! %s " (stringOfRange m)
                None
            else
                let tyConfirmsToSeq g ty = isAppTy g ty && tyconRefEq g (tcrefOfAppTy g ty) g.tcref_System_Collections_Generic_IEnumerable
                match SearchEntireHierarchyOfType (tyConfirmsToSeq g) g amap m (tyOfExpr g arbitrarySeqExpr) with
                | None ->
                    // printfn "FAILED - yield! did not yield a sequence! %s" (stringOfRange m)
                    None
                | Some ty ->
                    // printfn "found yield!"
                    let inpElemTy = List.head (argsOfAppTy g ty)
                    if isTailCall then
                             //this.pc <- NEXT
                             //nextEnumerator <- e
                             //return 2
                             //NEXT:
                        let label = IL.generateCodeLabel()
                        Some { phase2 = (fun (pcv, _currv, nextv, pcMap) ->
                                    let generate =
                                        mkCompGenSequential m
                                            (mkValSet m pcv (mkInt32 g m pcMap.[label]))
                                            (mkSequential SequencePointsAtSeq m
                                                (mkAddrSet m nextv arbitrarySeqExpr)
                                                (mkCompGenSequential m
                                                    (Expr.Op (TOp.Return, [], [mkTwo g m], m))
                                                    (Expr.Op (TOp.Label label, [], [], m))))
                                    let dispose =
                                        mkCompGenSequential m
                                            (Expr.Op (TOp.Label label, [], [], m))
                                            (Expr.Op (TOp.Goto currentDisposeContinuationLabel, [], [], m))
                                    let checkDispose =
                                        mkCompGenSequential m
                                            (Expr.Op (TOp.Label label, [], [], m))
                                            (Expr.Op (TOp.Return, [], [mkFalse g m], m))
                                    generate, dispose, checkDispose)
                               labels=[label]
                               stateVars=[]
                               significantClose = false
                               capturedVars = emptyFreeVars }
                    else
                        let v, ve = mkCompGenLocal m "v" inpElemTy
                        Lower false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel (mkCallSeqCollect g m inpElemTy inpElemTy (mkLambdaNoType g m v (mkCallSeqSingleton g m inpElemTy ve)) arbitrarySeqExpr)


    match overallExpr with
    | Seq(e, ty) ->
        // printfn "found seq { ... } or Seq.delay (fun () -> ...) in FSharp.Core.dll"
        let m = e.Range
        let initLabel = IL.generateCodeLabel()
        let noDisposeContinuationLabel = IL.generateCodeLabel()
        match Lower true true noDisposeContinuationLabel noDisposeContinuationLabel e with
        | Some res ->
            let labs = res.labels
            let stateVars = res.stateVars
            // printfn "successfully lowered, found %d state variables and %d labels!" stateVars.Length labs.Length
            let pcv, pce = mkMutableCompGenLocal m "pc" g.int32_ty
            let currv, _curre = mkMutableCompGenLocal m "current" ty
            let nextv, _nexte = mkMutableCompGenLocal m "next" (mkByrefTy g (mkSeqTy g ty))
            let nextvref = mkLocalValRef nextv
            let pcvref = mkLocalValRef pcv
            let currvref = mkLocalValRef currv
            let pcs = labs |> List.mapi (fun i _ -> i + 1)
            let pcDone = labs.Length + 1
            let pcInit = 0
            let pc2lab  = Map.ofList ((pcInit, initLabel) :: (pcDone, noDisposeContinuationLabel) :: List.zip pcs labs)
            let lab2pc = Map.ofList ((initLabel, pcInit) :: (noDisposeContinuationLabel, pcDone) :: List.zip labs pcs)
            let stateMachineExpr, disposalExpr, checkDisposeExpr = res.phase2 (pcvref, currvref, nextvref, lab2pc)
            // Add on the final 'return false' to indicate the iteration is complete
            let stateMachineExpr =
                mkCompGenSequential m
                    stateMachineExpr
                    (mkCompGenSequential m
                        // set the pc to "finished"
                        (mkValSet m pcvref (mkInt32 g m pcDone))
                        (mkCompGenSequential m
                            (Expr.Op (TOp.Label noDisposeContinuationLabel, [], [], m))
                            (mkCompGenSequential m
                                // zero out the current value to free up its memory
                                (mkValSet m currvref (mkDefault (m, currvref.Type)))
                                (Expr.Op (TOp.Return, [], [mkZero g m], m)))))
            let checkDisposeExpr =
                mkCompGenSequential m
                    checkDisposeExpr
                    (mkCompGenSequential m
                        (Expr.Op (TOp.Label noDisposeContinuationLabel, [], [], m))
                        (Expr.Op (TOp.Return, [], [mkFalse g m], m)))

            let addJumpTable isDisposal expr =
                let mbuilder = new MatchBuilder(NoSequencePointAtInvisibleBinding, m )
                let mkGotoLabelTarget lab = mbuilder.AddResultTarget(Expr.Op (TOp.Goto lab, [], [], m), SuppressSequencePointAtTarget)
                let dtree =
                  TDSwitch(pce,
                           [
                             // no disposal action for the initial state (pc = 0)
                             if isDisposal then
                                 yield mkCase(DecisionTreeTest.Const(Const.Int32 pcInit), mkGotoLabelTarget noDisposeContinuationLabel)
                             for pc in pcs do
                                 yield mkCase(DecisionTreeTest.Const(Const.Int32 pc), mkGotoLabelTarget pc2lab.[pc])
                             yield mkCase(DecisionTreeTest.Const(Const.Int32 pcDone), mkGotoLabelTarget noDisposeContinuationLabel) ],
                           Some(mkGotoLabelTarget pc2lab.[pcInit]),
                           m)

                let table = mbuilder.Close(dtree, m, g.int_ty)
                mkCompGenSequential m table (mkCompGenSequential m (Expr.Op (TOp.Label initLabel, [], [], m)) expr)

            let handleExeceptionsInDispose disposalExpr =
                // let mutable exn : exn = null
                // while(this.pc <> END_STATE) do
                //    try
                //       ``disposalExpr''
                //    with e -> exn <- e
                // if exn <> null then raise exn
                let exnV, exnE = mkMutableCompGenLocal m "exn" g.exn_ty
                let exnVref = mkLocalValRef exnV
                let startLabel = IL.generateCodeLabel()
                let doneLabel = IL.generateCodeLabel ()
                // try ``disposalExpr'' with e -> exn <- e
                let eV, eE = mkLocal m "e" g.exn_ty
                let efV, _ = mkLocal m "ef" g.exn_ty
                let assignToExn = Expr.Op (TOp.LValueOp (LValueOperation.LSet, exnVref), [], [eE], m)
                let exceptionCatcher =
                    mkTryWith g
                        (disposalExpr,
                            efV, Expr.Const ((Const.Bool true), m, g.bool_ty),
                            eV, assignToExn,
                            m, g.unit_ty,
                            NoSequencePointAtTry, NoSequencePointAtWith)


                // while(this.pc != END_STATE)
                let whileLoop =
                    let mbuilder = new MatchBuilder(NoSequencePointAtInvisibleBinding, m)
                    let addResultTarget e = mbuilder.AddResultTarget(e, SuppressSequencePointAtTarget)
                    let dtree =
                        TDSwitch(pce,
                                    [  mkCase((DecisionTreeTest.Const(Const.Int32 pcDone)), addResultTarget (Expr.Op (TOp.Goto doneLabel, [], [], m)) ) ],
                                    Some (addResultTarget (mkUnit g m)),
                                    m)
                    let pcIsEndStateComparison = mbuilder.Close(dtree, m, g.unit_ty)
                    mkCompGenSequential m
                        (Expr.Op ((TOp.Label startLabel), [], [], m))
                        (mkCompGenSequential m
                            pcIsEndStateComparison
                            (mkCompGenSequential m
                                exceptionCatcher
                                (mkCompGenSequential m
                                    (Expr.Op ((TOp.Goto startLabel), [], [], m))
                                    (Expr.Op ((TOp.Label doneLabel), [], [], m))
                                )
                            )
                        )
                // if exn != null then raise exn
                let doRaise =
                    mkNonNullCond g m g.unit_ty exnE (mkThrow m g.unit_ty exnE) (Expr.Const (Const.Unit, m, g.unit_ty))

                mkLet
                    NoSequencePointAtLetBinding m exnV  (Expr.Const (Const.Zero, m, g.exn_ty))
                        (mkCompGenSequential m whileLoop doRaise)

            let stateMachineExprWithJumpTable = addJumpTable false stateMachineExpr
            let disposalExpr =
                if res.significantClose then
                    let disposalExpr =
                        mkCompGenSequential m
                            disposalExpr
                            (mkCompGenSequential m
                                (Expr.Op (TOp.Label noDisposeContinuationLabel, [], [], m))
                                (mkCompGenSequential m
                                    // set the pc to "finished"
                                    (mkValSet m pcvref (mkInt32 g m pcDone))
                                    // zero out the current value to free up its memory
                                    (mkValSet m currvref (mkDefault (m, currvref.Type)))))
                    disposalExpr
                    |> addJumpTable true
                    |> handleExeceptionsInDispose
                else
                    (mkValSet m pcvref (mkInt32 g m pcDone))

            let checkDisposeExprWithJumpTable = addJumpTable true checkDisposeExpr
            // all done, no return the results
            Some (nextvref, pcvref, currvref, stateVars, stateMachineExprWithJumpTable, disposalExpr, checkDisposeExprWithJumpTable, ty, m)

        | None ->
            // printfn "FAILED: no compilation found! %s" (stringOfRange m)
            None
    | _ -> None


