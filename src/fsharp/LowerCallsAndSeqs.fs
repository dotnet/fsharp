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
   {
     /// The second phase of the transformation.  This rebuilds the 'generate', 'dispose' and 'checkDispose' expressions for the
     /// state machine.  It is run after all code labels and their mapping to program counters have been determined
     /// after the first phase. 
     ///
     /// The arguments to phase2 are as follows:
     ///     'pc' is the state machine variable allocated to hold the "program counter" for the state machine
     ///     'current' is the state machine variable allocated to hold the "current" value being yielded from the enumeration
     ///     'nextVar' is the argument variable for the GenerateNext method that represents the byref argument
     ///               that holds the "goto" destination for a tailcalling sequence expression
     ///     'pcMap' is the mapping from code labels to values for 'pc'
     ///
     /// The phase2 function returns the core of the generate, dipsose and checkDispose implementations. 
     phase2 : ((* pc: *) ValRef * (* current: *) ValRef * (* nextVar: *) ValRef * Map<ILCodeLabel, int> -> Expr * Expr * Expr)

     /// The labels allocated for one portion of the sequence expression
     entryPoints : int list

     /// Indicates if any actual work is done in dispose, i.e. is there a 'try-finally' (or 'use') in the computation.
     significantClose : bool

     /// The state variables allocated for one portion of the sequence expression (i.e. the local let-bound variables which become state variables)
     stateVars: ValRef list

     /// The vars captured by the non-synchronous path
     asyncVars: FreeVars
   }

let isVarFreeInExpr v e = Zset.contains v (freeInExpr CollectTyparsAndLocals e).FreeLocals

let (|ValApp|_|) g vref expr =
    match expr with
    // use 'seq { ... }' as an indicator
    | Expr.App (Expr.Val (vref2, _, _), _f0ty, tyargs, args, m) when valRefEq g vref vref2 ->  Some (tyargs, args, m)
    | _ -> None

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
let ConvertSequenceExprToObject g amap overallExpr =
    /// Detect a 'yield x' within a 'seq { ... }'
    let (|SeqYield|_|) expr =
        match expr with
        | ValApp g g.seq_singleton_vref (_, [arg], m) -> Some (arg, m)
        | _ -> None

    /// Detect a 'expr; expr' within a 'seq { ... }'
    let (|SeqAppend|_|) expr =
        match expr with
        | ValApp g g.seq_append_vref (_, [arg1; arg2], m) -> Some (arg1, arg2, m)
        | _ -> None

    /// Detect a 'while gd do expr' within a 'seq { ... }'
    let (|SeqWhile|_|) expr =
        match expr with
        | ValApp g g.seq_generated_vref (_, [Expr.Lambda (_, _, _, [dummyv], gd, _, _);arg2], m) 
             when not (isVarFreeInExpr dummyv gd) ->
            Some (gd, arg2, m)
        | _ ->
            None

    let (|SeqTryFinally|_|) expr =
        match expr with
        | ValApp g g.seq_finally_vref (_, [arg1;Expr.Lambda (_, _, _, [dummyv], compensation, _, _)], m) 
            when not (isVarFreeInExpr dummyv compensation) ->
            Some (arg1, compensation, m)
        | _ ->
            None

    let (|SeqUsing|_|) expr =
        match expr with
        | ValApp g g.seq_using_vref ([_;_;elemTy], [resource;Expr.Lambda (_, _, _, [v], body, _, _)], m) ->
            Some (resource, v, body, elemTy, m)
        | _ ->
            None

    let (|SeqFor|_|) expr =
        match expr with
        // Nested for loops are represented by calls to Seq.collect
        | ValApp g g.seq_collect_vref ([_inpElemTy;_enumty2;genElemTy], [Expr.Lambda (_, _, _, [v], body, _, _); inp], m) ->
            Some (inp, v, body, genElemTy, m)
        // "for x in e -> e2" is converted to a call to Seq.map by the F# type checker. This could be removed, except it is also visible in F# quotations.
        | ValApp g g.seq_map_vref ([_inpElemTy;genElemTy], [Expr.Lambda (_, _, _, [v], body, _, _); inp], m) ->
            Some (inp, v, mkCallSeqSingleton g body.Range genElemTy body, genElemTy, m)
        | _ -> None

    let (|SeqDelay|_|) expr =
        match expr with
        | ValApp g g.seq_delay_vref ([elemTy], [Expr.Lambda (_, _, _, [v], e, _, _)], _m) 
            when not (isVarFreeInExpr v e) -> 
            Some (e, elemTy)
        | _ -> None

    let (|SeqEmpty|_|) expr =
        match expr with
        | ValApp g g.seq_empty_vref (_, [], m) -> Some (m)
        | _ -> None

    let (|Seq|_|) expr =
        match expr with
        // use 'seq { ... }' as an indicator
        | ValApp g g.seq_vref ([elemTy], [e], _m) -> Some (e, elemTy)
        | _ -> None

    /// Implement a decision to represent a 'let' binding as a non-escaping local variable (rather than a state machine variable)
    let RepresentBindingAsLocal (bind: Binding) res2 m =
        if verbose then 
            printfn "LowerSeq: found local variable %s" bind.Var.DisplayName

        { res2 with
            phase2 = (fun ctxt ->
                let generate2, dispose2, checkDispose2 = res2.phase2 ctxt
                let generate = mkLetBind m bind generate2
                let dispose = dispose2
                let checkDispose = checkDispose2
                generate, dispose, checkDispose)
            stateVars = res2.stateVars }

    /// Implement a decision to represent a 'let' binding as a state machine variable
    let RepresentBindingAsStateMachineLocal (bind: Binding) res2 m =
        if verbose then 
            printfn "LowerSeq: found state variable %s" bind.Var.DisplayName
        
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
        if verbose then 
            printfn "found top level let  "
        
        { res2 with
            phase2 = (fun ctxt ->
                let generate2, dispose2, checkDispose2 = res2.phase2 ctxt
                let generate = mkBinds generate2
                let dispose = dispose2
                let checkDispose = checkDispose2
                generate, dispose, checkDispose) }

    let rec ConvertSeqExprCode
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
            Some { phase2 = (fun (pcVar, currVar, _nextv, pcMap) ->
                        let generate =
                            mkCompGenSequential m
                                (mkValSet m pcVar (mkInt32 g m pcMap.[label]))
                                (mkSequential SequencePointsAtSeq m
                                    (mkValSet m currVar e)
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
                   entryPoints=[label]
                   stateVars=[]
                   significantClose = false
                   asyncVars = emptyFreeVars
                  }

        | SeqDelay(delayedExpr, _elemTy) ->
            // printfn "found Seq.delay"
            // note, using 'isWholeExpr' here prevents 'seq { yield! e }' and 'seq { 0 .. 1000 }' from being compiled
            ConvertSeqExprCode isWholeExpr isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel delayedExpr

        | SeqAppend(e1, e2, m) ->
            // printfn "found Seq.append"
            let res1 = ConvertSeqExprCode false false noDisposeContinuationLabel currentDisposeContinuationLabel e1
            let res2 = ConvertSeqExprCode false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel e2
            match res1, res2 with
            | Some res1, Some res2 ->

                let asyncVars =
                    if res1.entryPoints.IsEmpty then
                        res2.asyncVars
                    else
                        // All of 'e2' is needed after resuming at any of the labels
                        unionFreeVars res1.asyncVars (freeInExpr CollectLocals e2)

                Some { phase2 = (fun ctxt ->
                            let generate1, dispose1, checkDispose1 = res1.phase2 ctxt
                            let generate2, dispose2, checkDispose2 = res2.phase2 ctxt
                            let generate = mkCompGenSequential m generate1 generate2
                            // Order shouldn't matter here, since disposals actions are linked together by goto's  (each ends in a goto).
                            // However leaving as is for now.
                            let dispose = mkCompGenSequential m dispose2 dispose1
                            let checkDispose = mkCompGenSequential m checkDispose2 checkDispose1
                            generate, dispose, checkDispose)
                       entryPoints= res1.entryPoints @ res2.entryPoints
                       stateVars = res1.stateVars @ res2.stateVars
                       significantClose = res1.significantClose || res2.significantClose
                       asyncVars = asyncVars }
            | _ ->
                None

        | SeqWhile(guardExpr, bodyExpr, m) ->
            // printfn "found Seq.while"
            let resBody = ConvertSeqExprCode false false noDisposeContinuationLabel currentDisposeContinuationLabel bodyExpr
            match resBody with
            | Some res2  ->
                let asyncVars =
                    if res2.entryPoints.IsEmpty then
                        res2.asyncVars  // the whole loop is synchronous, no labels
                    else
                        freeInExpr CollectLocals expr // everything is needed on subsequent iterations

                Some { phase2 = (fun ctxt ->
                            let generate2, dispose2, checkDispose2 = res2.phase2 ctxt
                            let generate = mkWhile g (SequencePointAtWhileLoop guardExpr.Range, NoSpecialWhileLoopMarker, guardExpr, generate2, m)
                            let dispose = dispose2
                            let checkDispose = checkDispose2
                            generate, dispose, checkDispose)
                       entryPoints = res2.entryPoints
                       stateVars = res2.stateVars
                       significantClose = res2.significantClose
                       asyncVars = asyncVars }
            | _ ->
                None

        | SeqUsing(resource, v, body, elemTy, m) ->
            // printfn "found Seq.using"
            let reduction =
                mkLet (SequencePointAtBinding body.Range) m v resource
                    (mkCallSeqFinally g m elemTy body
                        (mkUnitDelayLambda g m
                            (mkCallDispose g m v.Type (exprForVal m v))))
            ConvertSeqExprCode false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel reduction

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
            ConvertSeqExprCode false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel reduction

        | SeqTryFinally(e1, compensation, m) ->
            // printfn "found Seq.try/finally"
            let innerDisposeContinuationLabel = IL.generateCodeLabel()
            let resBody = ConvertSeqExprCode false false noDisposeContinuationLabel innerDisposeContinuationLabel e1
            match resBody with
            | Some res1  ->
                let asyncVars = unionFreeVars res1.asyncVars (freeInExpr CollectLocals compensation)
                Some { phase2 = (fun ((pcVar, _currv, _, pcMap) as ctxt) ->
                            let generate1, dispose1, checkDispose1 = res1.phase2 ctxt
                            let generate =
                                // copy the compensation expression - one copy for the success continuation and one for the exception
                                let compensation = copyExpr g CloneAllAndMarkExprValsAsCompilerGenerated compensation
                                mkCompGenSequential m
                                    // set the PC to the inner finally, so that if an exception happens we run the right finally
                                    (mkCompGenSequential m
                                        (mkValSet m pcVar (mkInt32 g m pcMap.[innerDisposeContinuationLabel]))
                                        generate1 )
                                    // set the PC past the try/finally before trying to run it, to make sure we only run it once
                                    (mkCompGenSequential m
                                        (Expr.Op (TOp.Label innerDisposeContinuationLabel, [], [], m))
                                        (mkCompGenSequential m
                                            (mkValSet m pcVar (mkInt32 g m pcMap.[currentDisposeContinuationLabel]))
                                            compensation))
                            let dispose =
                                // generate inner try/finallys, then outer try/finallys
                                mkCompGenSequential m
                                    dispose1
                                    // set the PC past the try/finally before trying to run it, to make sure we only run it once
                                    (mkCompGenSequential m
                                        (Expr.Op (TOp.Label innerDisposeContinuationLabel, [], [], m))
                                        (mkCompGenSequential m
                                            (mkValSet m pcVar (mkInt32 g m pcMap.[currentDisposeContinuationLabel]))
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
                       entryPoints = innerDisposeContinuationLabel :: res1.entryPoints
                       stateVars = res1.stateVars
                       significantClose = true
                       asyncVars = asyncVars }
            | _ ->
                None

        | SeqEmpty m ->
            // printfn "found Seq.empty"
            Some { phase2 = (fun _ ->
                            let generate = mkUnit g  m
                            let dispose = Expr.Op (TOp.Goto currentDisposeContinuationLabel, [], [], m)
                            let checkDispose = Expr.Op (TOp.Goto currentDisposeContinuationLabel, [], [], m)
                            generate, dispose, checkDispose)
                   entryPoints = []
                   stateVars = []
                   significantClose = false
                   asyncVars = emptyFreeVars }

        | Expr.Sequential (x1, x2, NormalSeq, ty, m) ->
            match ConvertSeqExprCode false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel x2 with
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

            let resBody = ConvertSeqExprCode false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel bodyExpr
            match resBody with
            | Some res2 ->
                if bind.Var.IsCompiledAsTopLevel then
                    Some (RepresentBindingsAsLifted (mkLetBind m bind) res2)
                elif not (res2.asyncVars.FreeLocals.Contains(bind.Var)) then
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

            match ConvertSeqExprCode false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel e2 with
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
        // LIMITATION: non-trivial pattern matches involving or-patterns or active patterns where bindings can't be
        // transferred to the r.h.s. are not yet compiled.
        //
        // TODO: remove this limitation
        | Expr.Match (spBind, exprm, pt, targets, m, ty) when targets |> Array.forall (fun (TTarget(vs, _e, _spTarget)) -> isNil vs) ->
            // lower all the targets. abandon if any fail to lower
            let tglArray = targets |> Array.map (fun (TTarget(_vs, targetExpr, _spTarget)) -> ConvertSeqExprCode false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel targetExpr)
            if tglArray |> Array.forall Option.isSome then
                let tglArray = Array.map Option.get tglArray
                let tgl = Array.toList tglArray
                let labs = tgl |> List.collect (fun res -> res.entryPoints)
                let asyncVars =
                    (emptyFreeVars, Array.zip targets tglArray)
                    ||> Array.fold (fun fvs ((TTarget(_vs, _, _spTarget)), res) ->
                        if res.entryPoints.IsEmpty then fvs else unionFreeVars fvs res.asyncVars)
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
                       entryPoints=labs
                       stateVars = stateVars
                       significantClose = significantClose
                       asyncVars = asyncVars }
            else
                None

        // yield! e ---> (for x in e -> x)
        //
        // Design choice: we compile 'yield! e' as 'for x in e do yield x'.
        //
        // Note, however, this leads to a loss of tailcalls: the case not
        // handled correctly yet is sequence expressions that use yield! in the last position
        // This can give rise to infinite iterator chains when implemented by the naive expansion to
        // 'for x in e yield e'. For example consider this:
        //
        // let rec rwalk x = {  yield x
        //                      yield! rwalk (x + rand()) }
        //
        // This is the moral equivalent of a tailcall optimization. These also don't compile well
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
                        Some { phase2 = (fun (pcVar, _currv, nextVar, pcMap) ->
                                    let generate =
                                        mkCompGenSequential m
                                            (mkValSet m pcVar (mkInt32 g m pcMap.[label]))
                                            (mkSequential SequencePointsAtSeq m
                                                (mkAddrSet m nextVar arbitrarySeqExpr)
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
                               entryPoints=[label]
                               stateVars=[]
                               significantClose = false
                               asyncVars = emptyFreeVars }
                    else
                        let v, ve = mkCompGenLocal m "v" inpElemTy
                        ConvertSeqExprCode false isTailCall noDisposeContinuationLabel currentDisposeContinuationLabel (mkCallSeqCollect g m inpElemTy inpElemTy (mkLambdaNoType g m v (mkCallSeqSingleton g m inpElemTy ve)) arbitrarySeqExpr)


    match overallExpr with
    | Seq(e, ty) ->
        // printfn "found seq { ... } or Seq.delay (fun () -> ...) in FSharp.Core.dll"
        let m = e.Range
        let initLabel = IL.generateCodeLabel()
        let noDisposeContinuationLabel = IL.generateCodeLabel()

        // Perform phase1
        match ConvertSeqExprCode true true noDisposeContinuationLabel noDisposeContinuationLabel e with
        | Some res ->
            
            // After phase1, create the variables for the state machine and work out a program counter for each label.
            let labs = res.entryPoints
            let stateVars = res.stateVars
            // printfn "successfully lowered, found %d state variables and %d labels!" stateVars.Length labs.Length
            let pcVar, pcExpr = mkMutableCompGenLocal m "pc" g.int32_ty
            let currVar, _currExpr = mkMutableCompGenLocal m "current" ty
            let nextVar, _nextExpr = mkMutableCompGenLocal m "next" (mkByrefTy g (mkSeqTy g ty))
            let nextVarRef = mkLocalValRef nextVar
            let pcVarRef = mkLocalValRef pcVar
            let currVarRef = mkLocalValRef currVar
            let pcs = labs |> List.mapi (fun i _ -> i + 1)
            let pcDone = labs.Length + 1
            let pcInit = 0
            let pc2lab  = Map.ofList ((pcInit, initLabel) :: (pcDone, noDisposeContinuationLabel) :: List.zip pcs labs)
            let lab2pc = Map.ofList ((initLabel, pcInit) :: (noDisposeContinuationLabel, pcDone) :: List.zip labs pcs)

            // Execute phase2, building the core of the the GenerateNext, Dispose and CheckDispose methods
            let generateExprCore, disposalExprCore, checkDisposeExprCore =
                res.phase2 (pcVarRef, currVarRef, nextVarRef, lab2pc)
            
            // Add on the final label and cleanup to the GenerateNext method 
            //    generateExpr;
            //    pc <- PC_DONE
            //    noDispose: 
            //       current <- null
            //       return 0
            let generateExprWithCleanup =
                mkCompGenSequential m
                    generateExprCore
                    (mkCompGenSequential m
                        // set the pc to "finished"
                        (mkValSet m pcVarRef (mkInt32 g m pcDone))
                        (mkCompGenSequential m
                            (Expr.Op (TOp.Label noDisposeContinuationLabel, [], [], m))
                            (mkCompGenSequential m
                                // zero out the current value to free up its memory
                                (mkValSet m currVarRef (mkDefault (m, currVarRef.Type)))
                                (Expr.Op (TOp.Return, [], [mkZero g m], m)))))

            // Add on the final label to the 'CheckDispose' method
            //    checkDisposeExprCore
            //    noDispose: 
            //        return false
            let checkDisposeExprWithCleanup =
                mkCompGenSequential m
                    checkDisposeExprCore
                    (mkCompGenSequential m
                        (Expr.Op (TOp.Label noDisposeContinuationLabel, [], [], m))
                        (Expr.Op (TOp.Return, [], [mkFalse g m], m)))

            // A utility to add a jump table to the three generated methods
            let addJumpTable isDisposal expr =
                let mbuilder = new MatchBuilder(NoSequencePointAtInvisibleBinding, m )
                let mkGotoLabelTarget lab = mbuilder.AddResultTarget(Expr.Op (TOp.Goto lab, [], [], m), SuppressSequencePointAtTarget)
                let dtree =
                  TDSwitch(pcExpr,
                           [
                             // Add an empty disposal action for the initial state (pc = 0)
                             if isDisposal then
                                 yield mkCase(DecisionTreeTest.Const(Const.Int32 pcInit), mkGotoLabelTarget noDisposeContinuationLabel)

                             // Yield one target for each PC, where the action of the target is to goto the appropriate label
                             for pc in pcs do
                                 yield mkCase(DecisionTreeTest.Const(Const.Int32 pc), mkGotoLabelTarget pc2lab.[pc])

                             // Yield one target for the 'done' program counter, where the action of the target is to continuation label
                             yield mkCase(DecisionTreeTest.Const(Const.Int32 pcDone), mkGotoLabelTarget noDisposeContinuationLabel) ],
                           Some(mkGotoLabelTarget pc2lab.[pcInit]),
                           m)

                let table = mbuilder.Close(dtree, m, g.int_ty)
                mkCompGenSequential m table (mkCompGenSequential m (Expr.Op (TOp.Label initLabel, [], [], m)) expr)

            // A utility to handle the cases where exceptions are raised by the disposal logic.  
            // We wrap the disposal state machine in a loop that repeatedly drives the disposal logic of the
            // state machine through each disposal state, then re-raise the last exception raised.
            // 
            // let mutable exn : exn = null
            // while(this.pc <> END_STATE) do
            //    try
            //       ``disposalExpr``
            //    with e -> exn <- e
            // if exn <> null then raise exn
            let handleExeceptionsInDispose disposalExpr =
                let exnV, exnE = mkMutableCompGenLocal m "exn" g.exn_ty
                let exnVref = mkLocalValRef exnV
                let startLabel = IL.generateCodeLabel()
                let doneDisposeLabel = IL.generateCodeLabel ()
                // try ``disposalExpr'' with e -> exn <- e
                let eV, eE = mkLocal m "e" g.exn_ty
                let efV, _ = mkLocal m "ef" g.exn_ty

                // exn <- e
                let assignToExn = Expr.Op (TOp.LValueOp (LValueOperation.LSet, exnVref), [], [eE], m)

                //    try
                //       ``disposalExpr``
                //    with e -> exn <- e
                let exceptionCatcher =
                    mkTryWith g
                        (disposalExpr,
                            efV, Expr.Const ((Const.Bool true), m, g.bool_ty),
                            eV, assignToExn,
                            m, g.unit_ty,
                            NoSequencePointAtTry, NoSequencePointAtWith)

                // Make the loop
                //
                // startLabel:
                //  match this.pc with
                //    | PC_DONE -> goto DONE_DISPOSE
                //    | _ -> ()
                //  try
                //      ``disposalExpr``
                //  with e ->
                //      exn <- e
                //  goto startLabel
                // DONE_DISPOSE:
                let whileLoop =
                    let mbuilder = new MatchBuilder(NoSequencePointAtInvisibleBinding, m)
                    let addResultTarget e = mbuilder.AddResultTarget(e, SuppressSequencePointAtTarget)
                    let dtree =
                        TDSwitch(pcExpr,
                                    [  mkCase((DecisionTreeTest.Const(Const.Int32 pcDone)), addResultTarget (Expr.Op (TOp.Goto doneDisposeLabel, [], [], m)) ) ],
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
                                    (Expr.Op ((TOp.Label doneDisposeLabel), [], [], m))
                                )
                            )
                        )
                // if exn != null then raise exn
                let doRaise =
                    mkNonNullCond g m g.unit_ty exnE (mkThrow m g.unit_ty exnE) (Expr.Const (Const.Unit, m, g.unit_ty))

                // let mutable exn = null
                // --loop--
                // if exn != null then raise exn
                mkLet
                    NoSequencePointAtLetBinding m exnV  (Expr.Const (Const.Zero, m, g.exn_ty))
                        (mkCompGenSequential m whileLoop doRaise)

            // Add the jumptable to the GenerateNext method
            let generateExprWithJumpTable =
                addJumpTable false generateExprWithCleanup

            // Add the jumptable to the Dispose method
            let disposalExprWithJumpTable =
                if res.significantClose then
                    let disposalExpr =
                        mkCompGenSequential m
                            disposalExprCore
                            (mkCompGenSequential m
                                (Expr.Op (TOp.Label noDisposeContinuationLabel, [], [], m))
                                (mkCompGenSequential m
                                    // set the pc to "finished"
                                    (mkValSet m pcVarRef (mkInt32 g m pcDone))
                                    // zero out the current value to free up its memory
                                    (mkValSet m currVarRef (mkDefault (m, currVarRef.Type)))))
                    disposalExpr
                    |> addJumpTable true
                    |> handleExeceptionsInDispose
                else
                    mkValSet m pcVarRef (mkInt32 g m pcDone)

            // Add the jumptable to the CheckDispose method
            let checkDisposeExprWithJumpTable =
                addJumpTable true checkDisposeExprWithCleanup

            // all done, now return the results
            Some (nextVarRef, pcVarRef, currVarRef, stateVars, generateExprWithJumpTable, disposalExprWithJumpTable, checkDisposeExprWithJumpTable, ty, m)

        | None ->
            // printfn "FAILED: no compilation found! %s" (stringOfRange m)
            None
    | _ -> None



//---------------------------------------------------------------------------------------------

type StateMachineConversionFirstPhaseResult =
   {
     // Represents the macro-expanded expression prior to decisions about labels
     phase1: Expr

     /// The second phase of the transformation.  It is run after all code labels and their mapping to program counters have been determined
     /// after the first phase. 
     ///
     phase2 : (Map<int, ILCodeLabel> -> Expr)

     /// The labels allocated for this portion of the computation
     entryPoints : int list

     /// The state variables allocated for one portion of the sequence expression (i.e. the local let-bound variables which become state variables)
     stateVars: ValRef list

     /// The vars captured by the non-synchronous path
     asyncVars: FreeVars
   }




let (|StateMachineExpr|_|) g expr =
    match expr with
    | ValApp g g.cgh_stateMachine_vref (_, [e], _m) -> Some e
    | _ -> None

let (|NewEntryPointExpr|_|) g expr =
    match expr with
    | ValApp g g.cgh_newEntryPoint_vref (_, [_], _m) -> Some ()
    | _ -> None

let (|ReturnExpr|_|) g expr =
    match expr with
    | ValApp g g.cgh_return_vref (_, [e], m) -> Some (e, m)
    | _ -> None

let (|MachineExpr|_|) g expr =
    match expr with
    | ValApp g g.cgh_machine_vref ([ty], _, m) -> Some (ty, m)
    | _ -> None

let (|EntryPointExpr|_|) g expr =
    match expr with
    | ValApp g g.cgh_entryPoint_vref (_, [e], m) -> Some (e, m)
    | _ -> None

let (|JumpTableExpr|_|) g expr =
    match expr with
    | ValApp g g.cgh_jumptable_vref (_, [pcExpr; codeExpr], _m) -> Some (pcExpr, codeExpr)
    | _ -> None

let sm_verbose = false

/// Implement a decision to represent a 'let' binding as a non-escaping local variable (rather than a state machine variable)
let RepresentBindingAsLiftedOrLocal (bind: Binding) (res2: StateMachineConversionFirstPhaseResult) m =
    if sm_verbose then 
        printfn "LowerStateMachine: found local variable %s" bind.Var.DisplayName

    { res2 with
        phase1 = mkLetBind m bind res2.phase1 
        phase2 = (fun ctxt -> mkLetBind m bind (res2.phase2 ctxt)) }

/// Implement a decision to represent a 'let' binding as a state machine variable
let RepresentBindingAsStateVar (bind: Binding) (res2: StateMachineConversionFirstPhaseResult) m =
    if sm_verbose then 
        printfn "LowerStateMachine: found state variable %s" bind.Var.DisplayName
    
    let (TBind(v, e, sp)) = bind
    let sp, spm =
        match sp with
        | SequencePointAtBinding m -> SequencePointsAtSeq, m
        | _ -> SuppressSequencePointOnExprOfSequential, e.Range
    let vref = mkLocalValRef v
    { res2 with
        phase1 = mkSequential sp m (mkValSet spm vref e) res2.phase1
        phase2 = (fun ctxt ->
            let generate2 = res2.phase2 ctxt
            let generate =
                //mkCompGenSequential m
                    (mkSequential sp m
                        (mkValSet spm vref e)
                        generate2)
                    // TODO: zero out the current value to free up its memory - but return type is not unit...
                  //  (mkValSet m vref (mkDefault (m, vref.Type)))
            generate )
        stateVars = vref :: res2.stateVars }

// We look ahead one binding to find the binding of the code
//
// GIVEN:
//   member inline __.Run(code : unit -> TaskStep<'T>) = 
//       (__stateMachine
//           { new TaskStateMachine<'T>() with 
//               member __.Step(pc) = __jumptable pc code }).Start()
//
// THEN
//    task { ... }
//
// IN DEBUG:
//
//    let builder@ = task
//    let code = 
//        let builder@ = task
//        (fun ....)
//    (__stateMachine code).Start()
//
// IN RELEASE:
//
//    let code = (fun ...)
//    (__stateMachine code).Start()

// TODO: this is too adhoc
let isMustExpandVar (v: Val) = 
    let nm = v.LogicalName
    nm.StartsWith "__expand_" 
    || nm.StartsWith "builder@" 
    || (v.BaseOrThisInfo = MemberThisVal)

let isExpandVar (v: Val) = 
    isMustExpandVar v
    || (v.BaseOrThisInfo = MemberThisVal)


let ConvertStateMachineExprToObject g overallExpr =

    let mutable pcCount = 0
    let genPC() =
        pcCount <- pcCount + 1
        pcCount

    // Evaluate __expand_ABC and __newEntryPoint bindings at compile-time. 
    // Here we record definitions for later use in TryApplyExpansions
    let rec BindExpansions g (env: ValMap<_>) expr = 

        match expr with
        // Bind 'let __expand_ABC = bindExpr in bodyExpr'
        | Expr.Let (bind, bodyExpr, _, _) when isExpandVar bind.Var -> 
            let envR = env.Add bind.Var bind.Expr
            BindExpansions g envR bodyExpr

        // Bind 'let CODE = __newEntryPoint() in bodyExpr'
        | Expr.Let (TBind(v, NewEntryPointExpr g (), _sp), bodyExpr, m, _) ->
            if sm_verbose then printfn "found __newEntryPoint()"
            let envR = env.Add v (mkInt g m (genPC()))
            BindExpansions g envR bodyExpr

        | _ ->
            (env, expr)

    // Detect sequencing constructs in state machine code
    let (|SequentialStateMachineCode|_|) expr = 
        match expr with

        // e1; e2
        | Expr.Sequential(e1, e2, NormalSeq, _, m) ->
            Some (e1, e2, m, (fun e1 e2 -> mkCompGenSequential m e1 e2))

        // let __machine_step$cont = e1 in e2
        // The $cont is used to prevent elimination in Optimizer.fs
        | Expr.Let(bind, e2, m, _) when bind.Var.CompiledName = "__machine_step$cont" ->
            Some (bind.Expr, e2, m, (fun e1 e2 -> mkLet bind.SequencePointInfo m bind.Var e1 e2))

        | _ -> None

    // Apply a single expansion of __expand_ABC and __newEntryPoint in an arbitrary expression
    let TryApplyExpansions g (env: ValMap<_>) expr = 
        match expr with
        // __machine --> ldarg.0
        | MachineExpr g (ty, m) -> 
            Some (mkGetArg0 m ty)

        // __expand_code --> [expand_code]
        | Expr.Val (vref, _, _) when env.ContainsVal vref.Deref ->
            let expandedExpr = env.[vref.Deref]
            if sm_verbose then printfn "expanded %A --> %A..." expr expandedExpr
            Some expandedExpr

        // __expand_code x --> let arg = x in expand_code[arg/x]
        | Expr.App (Expr.Val (vref, _, _), fty, [], args, m) when env.ContainsVal vref.Deref ->
            let f0 = env.[vref.Deref]
            let expandedExpr = MakeApplicationAndBetaReduce g (f0, fty, [], args, m)
            if sm_verbose then printfn "expanded %A --> %A..." expr expandedExpr
            Some expandedExpr

        | _ -> None

    // Repeatedly apply expansions 
    let rec ApplyExpansions g env expr = 
        match TryApplyExpansions g env expr with 
        | Some res -> ApplyExpansions g env res
        | None -> expr

    // Repeatedly find bindings and apply expansions 
    let rec RepeatBindAndApplyExpansions g env expr = 
        let env, expr = BindExpansions g env expr
        match TryApplyExpansions g env expr with 
        | Some res -> RepeatBindAndApplyExpansions g env res
        | None -> env, expr

    let (|ExpandsTo|) g env e = ApplyExpansions g env e

    // Detect a state machine or an application of a state machine to an method
    let rec (|StateMachineInContext|_|) g (env: ValMap<_>) overallExpr = 
        let env, expr = BindExpansions g env overallExpr 
        match expr with
        | Expr.App (f0, f0ty, tyargsl, (StateMachineExpr g objExpr :: args), mApp) ->
            Some (env, objExpr, (fun objExprR -> Expr.App (f0, f0ty, tyargsl, (objExprR :: args), mApp)))
        | StateMachineExpr g objExpr ->
            Some (env, objExpr, id)
        | _ -> None

    // Detect a state machine with a single method override
    let (|SingleMethodStateMachineInContext|_|) g overallExpr = 
        match overallExpr with
        | StateMachineInContext g ValMap.Empty (env, objExpr, remake) ->
            if sm_verbose then printfn "Found state machine..."
            match objExpr with 
            | Expr.Obj (objExprStamp, ty, basev, basecall, overrides, iimpls, stateVars, objExprRange) ->
                if sm_verbose then printfn "Found state machine object..."
                match overrides with 
                | [ (TObjExprMethod(slotsig, attribs, methTyparsOfOverridingMethod, methodParams, 
                         (JumpTableExpr g (pcExpr, codeLambdaExpr)), m)) ] ->
                    if sm_verbose then printfn "Found override and jump table call..."
                    let env, codeLambdaExpr = RepeatBindAndApplyExpansions g env codeLambdaExpr
                    match codeLambdaExpr with
                    | Expr.Lambda (_, _, _, [_dummyv], codeExpr, _, _) ->
                        if sm_verbose then printfn "Found code lambda..."
                        Some (env, objExprStamp, objExprRange, remake, ty, basev, basecall, slotsig, attribs, methTyparsOfOverridingMethod, methodParams, pcExpr, codeExpr, iimpls, stateVars, m)
                    | _ -> None
                | _ -> None
            | _ -> None
        | _ -> None

    // A utility to add a jump table an expression
    let addPcJumpTable (g: TcGlobals) m (pcs: int list)  (pc2lab: Map<int, ILCodeLabel>) pcExpr expr =
        if pcs.IsEmpty then 
            expr
        else
            let initLabel = IL.generateCodeLabel()
            let mbuilder = new MatchBuilder(NoSequencePointAtInvisibleBinding, m )
            let mkGotoLabelTarget lab = mbuilder.AddResultTarget(Expr.Op (TOp.Goto lab, [], [], m), SuppressSequencePointAtTarget)
            let dtree =
                TDSwitch(pcExpr,
                        [   // Yield one target for each PC, where the action of the target is to goto the appropriate label
                            for pc in pcs do
                                yield mkCase(DecisionTreeTest.Const(Const.Int32 pc), mkGotoLabelTarget pc2lab.[pc]) ],
                        // The default is to go to pcInit
                        Some(mkGotoLabelTarget initLabel),
                        m)

            let table = mbuilder.Close(dtree, m, g.int_ty)
            mkCompGenSequential m table (mkCompGenSequential m (Expr.Op (TOp.Label initLabel, [], [], m)) expr)

    /// Detect constructs allowed in state machines
    let rec ConvertStateMachineCode env pcExpr  expr = 
        if sm_verbose then 
            printfn "---------"
            printfn "ConvertStateMachineCodeing %s" (DebugPrint.showExpr expr)
            printfn "---------"
        
        let env, expr = RepeatBindAndApplyExpansions g env expr
        
        if sm_verbose then 
            printfn "Expanded to %s" (DebugPrint.showExpr expr)
            printfn "---------"

        // Detect the different permitted constructs in the expanded state machine
        let res = 
            match expr with 
        
            // The expanded code for state machines may use __entryPoint.  This indicates a resumption point.
            | EntryPointExpr g (ExpandsTo g env (Int32Expr pc), m) ->
                { phase1 = expr
                  phase2 = (fun pc2lab -> Expr.Op (TOp.Label pc2lab.[pc], [], [], m))
                  entryPoints=[pc]
                  stateVars = []
                  asyncVars = emptyFreeVars }

            // The expanded code for state machines may use __return.  This construct returns from the
            // overall method of the state machine.
            //
            // If the return occurs inside a try/with the actual effect is to branch out of the try/with
            // with the given result for that expression. Thus a 'return' is not guaranteed to be an early
            // exit.
            //
            // __return v --> return
            | ReturnExpr g (v, m) ->
                let expr = Expr.Op (TOp.Return, [], [v], m)
                { phase1 = expr
                  phase2 = (fun _ctxt -> expr)
                  entryPoints = []
                  stateVars = []
                  asyncVars = emptyFreeVars }

            // The expanded code for state machines may use sequential binding and sequential execution.
            //
            // let __machine_step$cont = e1 in e2
            // e1; e2
            //
            // A binding 'let .. = ... in ... ' is considered part of the state machine logic 
            // if it uses a binding variable name of precisely '__machine_step$cont'.
            // If this case 'e1' becomes part of the state machine too.
            | SequentialStateMachineCode(e1, e2, _m, recreate) ->
                // printfn "found sequential"
                let res1 = ConvertStateMachineCode env pcExpr e1
                let res2 = ConvertStateMachineCode env pcExpr e2
                let asyncVars =
                    if res1.entryPoints.IsEmpty then
                        // res1 is synchronous
                        res2.asyncVars
                    else
                        // res1 is not synchronous. All of 'e2' is needed after resuming at any of the labels
                        unionFreeVars res1.asyncVars (freeInExpr CollectLocals res2.phase1)

                { phase1 = recreate res1.phase1 res2.phase1
                  phase2 = (fun ctxt ->
                    let generate1 = res1.phase2 ctxt
                    let generate2 = res2.phase2 ctxt
                    let generate = recreate generate1 generate2
                    generate)
                  entryPoints= res1.entryPoints @ res2.entryPoints
                  stateVars = res1.stateVars @ res2.stateVars
                  asyncVars = asyncVars }

            // The expanded code for state machines may use while loops...
            | Expr.Op (TOp.While (sp1, sp2), _, [Expr.Lambda (_, _, _, [_gv], guardExpr, _, _);Expr.Lambda (_, _, _, [_bv], bodyExpr, _, _)], m) ->
                let resg = ConvertStateMachineCode env pcExpr guardExpr
                let resb = ConvertStateMachineCode env pcExpr bodyExpr
                let eps = resg.entryPoints @ resb.entryPoints
                // All free variables get captured if there are any entrypoints at all
                let asyncVars = if eps.IsEmpty then emptyFreeVars else unionFreeVars (freeInExpr CollectLocals resg.phase1) (freeInExpr CollectLocals resb.phase1)
                { phase1 = mkWhile g (sp1, sp2, resg.phase1, resb.phase1, m)
                  phase2 = (fun ctxt -> 
                        let egR = resg.phase2 ctxt
                        let ebR = resb.phase2 ctxt
                        mkWhile g (sp1, sp2, egR, ebR, m))
                  entryPoints= eps
                  stateVars = resg.stateVars @ resb.stateVars 
                  asyncVars = asyncVars }

            // The expanded code for state machines shoud not normally contain try/finally as any resumptions will repeatedly execute the finally.
            // Hoever we include the synchronous version of the construct here for completeness.
            | Expr.Op (TOp.TryFinally (sp1, sp2), [ty], [Expr.Lambda (_, _, _, [_], e1, _, _); Expr.Lambda (_, _, _, [_], e2, _, _)], m) ->
                let res1 = ConvertStateMachineCode env pcExpr e1
                let res2 = ConvertStateMachineCode env pcExpr e2
                let eps = res1.entryPoints @ res2.entryPoints
                if eps.Length > 0 then 
                    failwith "invalid state machine - try/finally may not contain resumption points"
                { phase1 = mkTryFinally g (res1.phase1, res2.phase1, m, ty, sp1, sp2)
                  phase2 = (fun ctxt -> 
                        let egR = res1.phase2 ctxt
                        let ebR = res2.phase2 ctxt
                        mkTryFinally g (egR, ebR, m, ty, sp1, sp2))
                  entryPoints= eps
                  stateVars = res1.stateVars @ res2.stateVars 
                  asyncVars = emptyFreeVars (* eps is empty, hence synchronous, no capture *)  }

            | Expr.Op (TOp.For (sp1, sp2), _, [Expr.Lambda (_, _, _, [_], e1, _, _);Expr.Lambda (_, _, _, [_], e2, _, _);Expr.Lambda (_, _, _, [v], e3, _, _)], m) ->
                let res1 = ConvertStateMachineCode env pcExpr e1
                let res2 = ConvertStateMachineCode env pcExpr e2
                let res3 = ConvertStateMachineCode env pcExpr e3
                let eps = res1.entryPoints @ res2.entryPoints @ res3.entryPoints
                if eps.Length > 0 then 
                    failwith "invalid state machine - try/finally may not contain asynchronous fast integer for loops"
                { phase1 = mkFor g (sp1, v, res1.phase1, sp2, res2.phase1, res3.phase1, m)
                  phase2 = (fun ctxt -> 
                        let e1R = res1.phase2 ctxt
                        let e2R = res2.phase2 ctxt
                        let e3R = res3.phase2 ctxt
                        mkFor g (sp1, v, e1R, sp2, e2R, e3R, m))
                  entryPoints= eps
                  stateVars = res1.stateVars @ res2.stateVars @ res3.stateVars
                  asyncVars = emptyFreeVars (* eps is empty, hence synchronous, no capture *) }

            // The expanded code for state machines may use try/with....
            | Expr.Op (TOp.TryCatch (spTry, spWith), [resTy], [Expr.Lambda (_, _, _, [_], bodyExpr, _, _); Expr.Lambda (_, _, _, [filterVar], filterExpr, _, _); Expr.Lambda (_, _, _, [handlerVar], handlerExpr, _, _)], m) -> 
                let resBody = ConvertStateMachineCode env pcExpr bodyExpr
                let resFilter = ConvertStateMachineCode env pcExpr filterExpr
                let resHandler = ConvertStateMachineCode env pcExpr handlerExpr
                { phase1 = mkTryWith g (resBody.phase1, filterVar, resFilter.phase1, handlerVar, resHandler.phase1, m, resTy, spTry, spWith)
                  phase2 = (fun ctxt -> 
                    // We can't jump into a try/catch block.  So we jump to the start of the try/catch and add a new jump table
                    let pcsAndLabs = ctxt |> Map.toList  
                    let innerPcs = resBody.entryPoints 
                    if innerPcs.IsEmpty then 
                        let bodyExprR = resBody.phase2 ctxt
                        let filterExprR = resFilter.phase2 ctxt
                        let handlerExprR = resHandler.phase2 ctxt
                        mkTryWith g (bodyExprR, filterVar, filterExprR, handlerVar, handlerExprR, m, resTy, spTry, spWith)
                    else
                        let innerPcSet = innerPcs |> Set.ofList
                        let outerLabsForInnerPcs = pcsAndLabs |> List.filter (fun (pc, _outerLab) -> innerPcSet.Contains pc) |> List.map snd
                        // generate the inner labels
                        let pcsAndInnerLabs = pcsAndLabs |> List.map (fun (pc, l) -> (pc, if innerPcSet.Contains pc then IL.generateCodeLabel() else l))
                        let innerPc2Lab = Map.ofList pcsAndInnerLabs

                        let bodyExprR = resBody.phase2 innerPc2Lab
                        let filterExprR = resFilter.phase2 ctxt
                        let handlerExprR = resHandler.phase2 ctxt

                        // Add a jump table at the entry to the try
                        let bodyExprRWithJumpTable = addPcJumpTable g m innerPcs innerPc2Lab pcExpr bodyExprR
                        let coreExpr = mkTryWith g (bodyExprRWithJumpTable, filterVar, filterExprR, handlerVar, handlerExprR, m, resTy, spTry, spWith)
                        // Place all the outer labels just before the try
                        let labelledExpr = (coreExpr, outerLabsForInnerPcs) ||> List.fold (fun e l -> mkCompGenSequential m (Expr.Op (TOp.Label l, [], [], m)) e)
                

                        //((pcInit, initLabel) :: List.zip pcs labs)

                        labelledExpr)
                  entryPoints= resBody.entryPoints @ resFilter.entryPoints @ resHandler.entryPoints 
                  stateVars = resBody.stateVars @ resFilter.stateVars @ resHandler.stateVars
                  asyncVars = unionFreeVars resBody.asyncVars (unionFreeVars(freeInExpr CollectLocals resFilter.phase1) (freeInExpr CollectLocals resHandler.phase1)) }

            // control-flow match
            | Expr.Match (spBind, exprm, pt, targets, m, ty) ->
                // lower all the targets. 
                let tglArray = targets |> Array.map (fun (TTarget(_vs, targetExpr, _spTarget)) -> ConvertStateMachineCode env pcExpr targetExpr)
                let tgl = Array.toList tglArray
                let entryPoints = tgl |> List.collect (fun res -> res.entryPoints)
                let asyncVars =
                    (emptyFreeVars, Array.zip targets tglArray)
                    ||> Array.fold (fun fvs ((TTarget(_vs, _, _spTarget)), res) ->
                        if res.entryPoints.IsEmpty then fvs else unionFreeVars fvs res.asyncVars)
                let stateVars = tgl |> List.collect (fun res -> res.stateVars)
                { phase1 = 
                      let gtgs = (targets, tglArray) ||> Array.map2 (fun (TTarget(vs, _, spTarget)) res -> TTarget(vs, res.phase1, spTarget))
                      primMkMatch (spBind, exprm, pt, gtgs, m, ty)
                  phase2 = (fun ctxt ->
                                let gtgs = (targets, tglArray) ||> Array.map2 (fun (TTarget(vs, _, spTarget)) res -> TTarget(vs, res.phase2 ctxt, spTarget))
                                let generate = primMkMatch (spBind, exprm, pt, gtgs, m, ty)
                                generate)
                  entryPoints = entryPoints
                  stateVars = stateVars
                  asyncVars = asyncVars }

            // Non-control-flow let binding can appear as part of state machine. The body is considered state-machine code,
            // the expression being bound is not.
            | Expr.Let (bind, bodyExpr, m, _)
                  // Restriction: compilation of sequence expressions containing non-toplevel constrained generic functions is not supported
                  when  bind.Var.IsCompiledAsTopLevel || not (IsGenericValWithGenericContraints g bind.Var) ->

                // Rewrite the expression on the r.h.s. of the binding
                // TODO: this outer use of ApplyExpansions looks wrong.
                let bind = mkBind bind.SequencePointInfo bind.Var (ApplyExpansions g env bind.Expr)

                let resBody = ConvertStateMachineCode env pcExpr bodyExpr
                if bind.Var.IsCompiledAsTopLevel || not (resBody.asyncVars.FreeLocals.Contains(bind.Var)) then
                    (RepresentBindingAsLiftedOrLocal bind resBody m)
                else
                    // printfn "found state variable %s" bind.Var.DisplayName
                    (RepresentBindingAsStateVar bind resBody m)

            // LetRec bindings may not appear as part of state machine.
            | Expr.LetRec _ -> 
                  failwith "recursive bindings not allowed in state machine, please lift it out"

            // Arbitrary expression
            | _ -> 
                // Reqrite all macro expansions
                let expr =  
                    expr |> RewriteExpr { PreIntercept = Some (fun cont e -> match TryApplyExpansions g env e with Some e2 -> Some (cont e2) | None -> None)
                                          PostTransform = (fun _ -> None)
                                          PreInterceptBinding = None
                                          IsUnderQuotations=true } 
                { phase1 = expr
                  phase2 = (fun _ctxt -> expr)
                  entryPoints=[]
                  stateVars = []
                  asyncVars = emptyFreeVars }
        if sm_verbose then 
            printfn "-------------------"
            printfn "Phase 1 Done for %s" (DebugPrint.showExpr res.phase1)
            printfn "Phase 1 Done, asyncVars = %A" (res.asyncVars.FreeLocals |> Zset.elements |> List.map (fun v -> v.CompiledName) |> String.concat ",")
            printfn "-------------------"
        res

    // Detect a state machine and convert it
    match overallExpr with
    | SingleMethodStateMachineInContext g (env, objExprStamp, objExprRange, remake, ty, basev, basecall, slotsig, attribs, methTyparsOfOverridingMethod, methodParams, pcExpr, codeExpr, iimpls, origStateVars, m) ->
        if sm_verbose then 
            printfn "Found state machine override method and code expression..."
            printfn "----------- BEFORE LOWER ----------------------"
            printfn "%s" (DebugPrint.showExpr codeExpr)
            printfn "----------- LOWER ----------------------"
    
        // Perform phase1 of the conversion
        let res = ConvertStateMachineCode env pcExpr codeExpr 

        // Work out the initial mapping of pcs to labels
        let pcs = [ 1 .. pcCount ]
        let stateVars = res.stateVars
        let labs = pcs |> List.map (fun _ -> IL.generateCodeLabel())
        let pc2lab  = Map.ofList (List.zip pcs labs)

        // Execute phase2, building the core of the method
        if sm_verbose then printfn "----------- PHASE2 ----------------------"

        // Perform phase2 to build the final expression
        let methodBodyExprR = res.phase2 pc2lab

        if sm_verbose then printfn "----------- ADDING JUMP TABLE ----------------------"

        // Add the jump table
        let methodBodyExprWithJumpTable = addPcJumpTable g m pcs pc2lab pcExpr methodBodyExprR
        
        if sm_verbose then printfn "----------- REMAKE ----------------------"

        // Rebuild the object expression
        let overrideR = TObjExprMethod(slotsig, attribs, methTyparsOfOverridingMethod, methodParams, methodBodyExprWithJumpTable, m) 
        let objExprR = Expr.Obj (objExprStamp, ty, basev, basecall, [overrideR], iimpls, origStateVars @ stateVars, objExprRange)
        let overallExprR = remake objExprR

        if sm_verbose then 
            printfn "----------- AFTER REWRITE ----------------------"
            printfn "%s" (DebugPrint.showExpr overallExprR)
        
        //printfn "----------- CHECKING ----------------------"
        //let mutable failed = false
        //let _expr =  
        //    overallExprR |> RewriteExpr 
        //        { PreIntercept = None
        //          PostTransform = (fun e ->
        //                match e with
        //                | Expr.Val(vref, _, _) when isMustExpandVar vref.Deref -> 
        //                    System.Diagnostics.Debug.Assert(false, "FAILED: unexpected expand var")
        //                    failed <- true
        //                    None
        //                | _ -> None) 
        //          PreInterceptBinding = None
        //          IsUnderQuotations=true } 
        if sm_verbose then printfn "----------- DONE ----------------------"
        
        Some overallExprR

    | _ -> None

