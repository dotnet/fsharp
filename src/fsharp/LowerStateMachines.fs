// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LowerStateMachines

open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.Internal
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Infos
open FSharp.Compiler.Lib
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.PrettyNaming
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps


let mkLabelled m l e = mkCompGenSequential m (Expr.Op (TOp.Label l, [], [], m)) e

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

     /// All this values get represented via the 'this' pointer
     thisVars: ValRef list

     /// The vars captured by the non-synchronous path
     asyncVars: FreeVars
   }

let sm_verbose = try System.Environment.GetEnvironmentVariable("FSharp_StateMachineVerbose") <> null with _ -> false


let (|RefStateMachineExpr|_|) g expr =
    match expr with
    | ValApp g g.cgh__resumableStateMachine_vref (_, [e], _m) -> Some e
    | _ -> None

let (|StructStateMachineExpr|_|) g expr =
    match expr with
    | ValApp g g.cgh__resumableStateMachineStruct_vref ([templateStructTy; _resultTy], [moveNextExpr; setMachineStateExpr; afterMethodExpr], m) ->
        if sm_verbose then 
            printfn "LowerStateMachine: found resumableStruct call at %A" m 
        Some (templateStructTy, moveNextExpr, setMachineStateExpr, afterMethodExpr)
    | _ -> None


let (|ResumeAtExpr|_|) g expr =
    match expr with
    | ValApp g g.cgh__resumeAt_vref (_, [pcExpr], _m) -> Some pcExpr
    | _ -> None

let (|OptionalResumeAtExpr|) g expr =
    match expr with
    | Expr.Sequential(ResumeAtExpr g pcExpr, codeExpr, NormalSeq, _, _m) -> (Some pcExpr, codeExpr)
    | _ -> (None, expr)

/// Implement a decision to represent a 'let' binding as a non-escaping local variable (rather than a state machine variable)
let RepresentBindingAsTopLevelOrLocal (bind: Binding) (res2: StateMachineConversionFirstPhaseResult) m =
    if sm_verbose then 
        printfn "LowerStateMachine: found local variable %s" bind.Var.DisplayName

    { res2 with
        phase1 = mkLetBind m bind res2.phase1 
        phase2 = (fun ctxt -> mkLetBind m bind (res2.phase2 ctxt)) }

/// Implement a decision to represent a 'let' binding as a non-escaping local variable (rather than a state machine variable)
let RepresentBindingAsThis (bind: Binding) (res2: StateMachineConversionFirstPhaseResult) _m =
    if sm_verbose then 
        printfn "LowerStateMachine: found local variable %s" bind.Var.DisplayName

    { res2 with
        thisVars = mkLocalValRef bind.Var :: res2.thisVars
        // Drop the let binding on the floor as it is only rebinding the 'this' variable
        phase1 = res2.phase1 
        phase2 = res2.phase2 }

/// Implement a decision to represent a 'let' binding as a state machine variable
let RepresentBindingAsStateVar (bind: Binding) (res2: StateMachineConversionFirstPhaseResult) m =
    if sm_verbose then 
        printfn "LowerStateMachine: found state variable %s" bind.Var.DisplayName
    
    let (TBind(v, e, sp)) = bind
    let sp, spm =
        match sp with
        | DebugPointAtBinding m -> DebugPointAtSequential.Both, m
        | _ -> DebugPointAtSequential.StmtOnly, e.Range
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
//       (__resumableStateMachine
//           { new TaskStateMachine<'T>() with 
//               member __.Step(pc) = __resumeAt pc code }).Start()
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
//    (__resumableStateMachine code).Start()
//
// IN RELEASE:
//
//    let code = (fun ...)
//    (__resumableStateMachine code).Start()

let isExpandVar (v: Val) = 
    let nm = v.LogicalName
    (nm.StartsWith expansionFunctionPrefix
     || nm.StartsWith "builder@" 
     || (v.BaseOrThisInfo = MemberThisVal)) &&
    not v.IsCompiledAsTopLevel

type env = 
    { 
      Macros: ValMap<Expr>
      TemplateStructTy: TType option
      //MachineAddrExpr: Expr option 
    }

    static member Empty = 
        { Macros = ValMap.Empty
          TemplateStructTy = None
          //MachineAddrExpr = None 
        }

/// Detect prefix of state machine expressions 
let rec IsPossibleStateMachineExpr g expr = 
    match expr with
    | Expr.Let (macroBind, bodyExpr, _, _) when isExpandVar macroBind.Var -> IsPossibleStateMachineExpr g bodyExpr
    | IfUseResumableStateMachinesExpr g _ -> true
    | _ -> false

let ConvertStateMachineExprToObject g overallExpr =

    let mutable pcCount = 0
    let genPC() =
        pcCount <- pcCount + 1
        pcCount

    // Record definitions for __expand_ABC bindings at compile-time. 
    let rec BindMacros (env: env) expr = 

        match expr with
        // Bind 'let __expand_ABC = bindExpr in bodyExpr'
        | Expr.Let (macroBind, bodyExpr, _, _) when isExpandVar macroBind.Var -> 
            if sm_verbose then printfn "binding %A --> %A..." macroBind.Var macroBind.Expr
            let envR = { env with Macros = env.Macros.Add macroBind.Var macroBind.Expr }
            BindMacros envR bodyExpr

        // Eliminate 'if useResumableCode ...'
        | IfUseResumableStateMachinesExpr g (thenExpr, _) -> 
            if sm_verbose then printfn "eliminating 'if useResumableCode...'" 
            BindMacros env thenExpr

        | _ ->
            (env, expr)

    // Detect sequencing constructs in state machine code
    let (|SequentialStateMachineCode|_|) (g: TcGlobals) expr = 
        match expr with

        // e1; e2
        | Expr.Sequential(e1, e2, NormalSeq, _, m) ->
            Some (e1, e2, m, (fun e1 e2 -> mkCompGenSequential m e1 e2))

        // let __stack_step = e1 in e2
        | Expr.Let(bind, e2, m, _) when bind.Var.CompiledName(g.CompilerGlobalState).StartsWith(stackStepName) ->
            Some (bind.Expr, e2, m, (fun e1 e2 -> mkLet bind.DebugPoint m bind.Var e1 e2))

        | _ -> None

    let rec TryApplyMacroDef (env: env) expr (args: Expr list) = 
        if isNil args then Some expr else
        match expr with 
        | Expr.TyLambda _ 
        | Expr.Lambda _ -> 
            let macroTypars, macroParamsCurried, macroBody, _rty = stripTopLambda (expr, tyOfExpr g expr)
            let m = macroBody.Range
            if not (isNil macroTypars) then 
                error(Error(FSComp.SR.stateMachineMacroTypars(), m))
            let macroParams = List.concat macroParamsCurried
            let macroVal2 = mkLambdas m macroTypars macroParams (macroBody, tyOfExpr g macroBody)
            if args.Length < macroParams.Length then 
                error(Error(FSComp.SR.stateMachineMacroUnderapplied(), m))
            let nowArgs, laterArgs = List.splitAt macroParams.Length args
            let expandedExpr = MakeApplicationAndBetaReduce g (macroVal2, (tyOfExpr g macroVal2), [], nowArgs, m)
            TryApplyMacroDef env expandedExpr laterArgs

        | NewDelegateExpr g (macroParamsCurried, macroBody, _) -> 
            let m = expr.Range
            let macroParams = List.concat macroParamsCurried
            let macroVal2 = mkLambdas m [] macroParams (macroBody, tyOfExpr g macroBody)
            if args.Length < macroParams.Length then 
                error(Error(FSComp.SR.stateMachineMacroUnderapplied(), m))
            let nowArgs, laterArgs = List.splitAt macroParams.Length args
            let expandedExpr = MakeApplicationAndBetaReduce g (macroVal2, (tyOfExpr g macroVal2), [], nowArgs, m)
            TryApplyMacroDef env expandedExpr laterArgs

        | Expr.Let (bind, bodyExpr, m, _) -> 
            match TryApplyMacroDef env bodyExpr args with 
            | Some bodyExpr2 -> Some (mkLetBind m bind bodyExpr2)
            | None -> None

        | Expr.Sequential (x1, bodyExpr, sp, ty, m) -> 
            match TryApplyMacroDef env bodyExpr args with 
            | Some bodyExpr2 -> Some (Expr.Sequential (x1, bodyExpr2, sp, ty, m))
            | None -> None

        | Expr.Match (spBind, exprm, dtree, targets, m, ty) ->
            let targets2 = 
                targets |> Array.choose (fun (TTarget(vs, targetExpr, spTarget, flags)) -> 
                    // Incomplete excption matching expressions give rise to targets with I_throw. Keep these in the residue with
                    // the type of the expression adjusted
                    match targetExpr with 
                    | Expr.Op (TOp.ILAsm ([ AbstractIL.IL.I_throw ], [_]), tyargs, args, m) -> 
                        Some (TTarget(vs, Expr.Op (TOp.ILAsm ([ AbstractIL.IL.I_throw ], [tyOfExpr g expr]), tyargs, args, m), spTarget, flags))
                    | _ ->

                    match TryApplyMacroDef env targetExpr args with 
                    | Some targetExpr2 -> Some (TTarget(vs, targetExpr2, spTarget, flags))
                    | None -> None)
            if targets2.Length = targets.Length then 
                Some (Expr.Match (spBind, exprm, dtree, targets2, m, ty))
            else
                None

        | TryFinallyExpr (sp1, sp2, ty, bodyExpr, compensation, m) ->
            match TryApplyMacroDef env bodyExpr args with
            | Some bodyExpr2 -> Some (mkTryFinally g (bodyExpr2, compensation, m, ty, sp1, sp2))
            | None -> None

        | WhileExpr (sp1, sp2, guardExpr, bodyExpr, m) ->
            match TryApplyMacroDef env bodyExpr args with
            | Some bodyExpr2 -> Some (mkWhile g (sp1, sp2, guardExpr, bodyExpr2, m))
            | None -> None

        | TryCatchExpr (spTry, spWith, resTy, bodyExpr, filterVar, filterExpr, handlerVar, handlerExpr, m) ->
            match TryApplyMacroDef env bodyExpr args with
            | Some bodyExpr2 -> Some (mkTryWith g (bodyExpr2, filterVar, filterExpr, handlerVar, handlerExpr, m, resTy, spTry, spWith))
            | None -> None

        | _ -> 
            None

    // Apply a single expansion of __expand_ABC at the outermost position in an arbitrary expression
    let rec TryExpandMacro (env: env) expr args remake = 
        if sm_verbose then printfn "expanding macros for %A..." expr
        //let (env, expr) = BindMacros env expr
        //if sm_verbose then printfn "checking %A for possible macro application..." expr
        match expr with
        // __expand_code --> [expand_code]
        | Expr.Val (macroRef, _, _) when env.Macros.ContainsVal macroRef.Deref ->
            let macroDef = env.Macros.[macroRef.Deref]
            if sm_verbose then printfn "found macro %A --> %A" macroRef macroDef 
            // Expand the macro definition
            match TryApplyMacroDef env macroDef args with 
            | Some expandedExpr -> 
                if sm_verbose then printfn "expanded macro %A --> %A..." macroRef expandedExpr
                Some expandedExpr
            | None -> 
                // If the arity wasn't right and the macro is simply 'a = b' then substitute the r.h.s.
                // e.g. passing __expand_code to __expand_code parameter
                match macroDef with 
                | Expr.Val _ -> Some (remake macroDef)
                | _ -> 
                    error(Error(FSComp.SR.stateMachineMacroInvalidExpansion(), expr.Range))
                    //error(InternalError(sprintf "invalid macro expansion %A = %A" expr macroDef, expr.Range))

        // __expand_code.Invoke x --> let arg = x in [__expand_code][arg/x]
        | Expr.App ((Expr.Val (invokeRef, _, _) as iref), a, b, (f :: args2), m) 
                when invokeRef.LogicalName = "Invoke" && isDelegateTy g (tyOfExpr g f) -> 
            if sm_verbose then printfn "found delegate invoke in possible macro application..." 
            TryExpandMacro env f (args2 @ args) (fun f2 -> remake (Expr.App ((iref, a, b, (f2 :: args2), m))))

        // __expand_code x --> let arg = x in [__expand_code][arg/x] 
        | Expr.App (f, _fty, _tyargs, args2, _m) ->
            if sm_verbose then printfn "found app in possible macro application..." 
            TryExpandMacro env f (args2 @ args) (fun f2 -> remake (Expr.App (f2, _fty, _tyargs, args2, _m)))

        | _ -> None

    // Repeated top-down rewrite
    let makeRewriteEnv (env: env) = 
        { PreIntercept = Some (fun cont e -> match TryExpandMacro env e [] id with Some e2 -> Some (cont e2) | None -> None)
          PostTransform = (fun _ -> None)
          PreInterceptBinding = None
          IsUnderQuotations=true } 

    let ConvertStateMachineLeafExpression (env: env) expr = 
        if sm_verbose then printfn "ConvertStateMachineLeafExpression for %A..." expr
        expr |> RewriteExpr (makeRewriteEnv env)

    let ConvertStateMachineLeafDecisionTree (env: env) expr = 
        if sm_verbose then printfn "ConvertStateMachineLeafDecisionTree for %A..." expr
        expr |> RewriteDecisionTree (makeRewriteEnv env)

    /// Repeatedly find outermost expansion definitions and apply outermost expansions 
    let rec RepeatBindAndApplyOuterMacros (env: env) expr = 
        if sm_verbose then printfn "RepeatBindAndApplyOuterMacros for %A..." expr
        let env2, expr2 = BindMacros env expr
        match TryExpandMacro env2 expr2 [] id with 
        | Some res -> RepeatBindAndApplyOuterMacros env2 res
        | None -> env2, expr2

    // Detect a reference-type state machine (or an application of a reference type state machine to a method)
    let (|RefStateMachineInContext|_|) (env: env) expr = 
        match expr with
        | Expr.App (f0, f0ty, tyargsl, (RefStateMachineExpr g objExpr :: args), mApp) ->
            Some (env, objExpr, (fun objExprR -> Expr.App (f0, f0ty, tyargsl, (objExprR :: args), mApp)))
        | RefStateMachineExpr g objExpr ->
            Some (env, objExpr, id)
        | _ -> None

    // Detect a struct-type state machine (or an application of a struct type state machine to a method)
    let (|StructStateMachineInContext|_|) (env: env) expr = 
        match expr with
        | StructStateMachineExpr g (templateStructTy, moveNextExpr, setMachineStateExpr, afterMethodExpr) ->
            Some (env, templateStructTy, moveNextExpr, setMachineStateExpr, afterMethodExpr)
        | _ -> None

    // Detect a state machine with a single method override
    let (|StateMachineInContext|_|) inputExpr = 
        // All true instantiated 'task { .. }' begin with a bind of @builder or '__expand_code' (as opposed to the library definitions of 'Run' etc. which do not)
        //match inputExpr with 
        //| Expr.Let (bind, _, _, _) when isExpandVar bind.Var -> 
        if IsPossibleStateMachineExpr g inputExpr then 
            let env, expr = BindMacros env.Empty inputExpr 
            match expr with
            | RefStateMachineInContext env (env, objExpr, remake) ->
                if sm_verbose then printfn "Found ref state machine..."
                match objExpr with 
                | Expr.Obj (objExprStamp, ty, basev, basecall, overrides, iimpls, stateVars, objExprRange) ->
                    if sm_verbose then printfn "Found ref state machine object expression..."
                    match overrides with 
                    | [ (TObjExprMethod(slotsig, attribs, methTyparsOfOverridingMethod, methodParams, 
                             (OptionalResumeAtExpr g (pcExprOpt, codeExpr)), m)) ] ->
                        if sm_verbose then printfn "Found ref state machine override and jump table call..."
                        let env, codeExprR = RepeatBindAndApplyOuterMacros env codeExpr
                        if sm_verbose then printfn "Found ref state machine jump table code lambda..."

                        let remake2 (moveNextExprWithJumpTable, furtherStateVars, _thisVars) = 
                            let overrideR = TObjExprMethod(slotsig, attribs, methTyparsOfOverridingMethod, methodParams, moveNextExprWithJumpTable, m) 
                            let objExprR = Expr.Obj (objExprStamp, ty, basev, basecall, [overrideR], iimpls, stateVars @ furtherStateVars, objExprRange)
                            let overallExprR = remake objExprR
                            if sm_verbose then 
                                printfn "----------- AFTER REWRITE ----------------------"
                                printfn "%s" (DebugPrint.showExpr g overallExprR)

                            Choice1Of2 overallExprR

                        Some (env, remake2, pcExprOpt, codeExprR, m)
                    | _ -> 
                        if sm_verbose then printfn "CONVERSION FAILURE: Didn't find ref state machine override and jump table call..."
                        None
                | _ -> 
                    if sm_verbose then printfn "CONVERSION FAILURE: Didn't find ref state machine object expression..."
                    None

            | StructStateMachineInContext env (env, templateStructTy, moveNextExpr, setMachineStateExpr, afterMethodExpr) ->
                let env = { env with TemplateStructTy = Some templateStructTy }
                if sm_verbose then printfn "Found struct machine call..."
                match moveNextExpr, setMachineStateExpr, afterMethodExpr with 
                | NewDelegateExpr g ([[moveNextMethodThisVar]], moveNextMethodBodyExpr, m), setMachineStateExpr, NewDelegateExpr g ([[afterMethodThisVar]], afterMethodBodyExpr, _) ->
                    if sm_verbose then printfn "Found struct machine lambdas..."
                    match moveNextMethodBodyExpr with 
                    | OptionalResumeAtExpr g (pcExprOpt, codeExpr) ->
                        if sm_verbose then printfn "Found struct machine jump table call..."
                        let env, codeExprR = RepeatBindAndApplyOuterMacros env codeExpr
                        let setMachineStateExprR = ConvertStateMachineLeafExpression env setMachineStateExpr
                        let afterMethodBodyExprR = ConvertStateMachineLeafExpression env afterMethodBodyExpr
                        //let afterMethodBodyExprR = ConvertStateMachineLeafExpression { env with MachineAddrExpr = Some machineAddrExpr } startExpr
                        let remake2 (moveNextExprWithJumpTable, stateVars, thisVars) = 
                            if sm_verbose then 
                                printfn "----------- AFTER REWRITE moveNextExprWithJumpTable ----------------------"
                                printfn "%s" (DebugPrint.showExpr g moveNextExprWithJumpTable)
                                printfn "----------- AFTER REWRITE setMachineStateExprR ----------------------"
                                printfn "%s" (DebugPrint.showExpr g setMachineStateExprR)
                                printfn "----------- AFTER REWRITE afterMethodBodyExprR ----------------------"
                                printfn "%s" (DebugPrint.showExpr g afterMethodBodyExprR)
                            Choice2Of2 (templateStructTy, stateVars, thisVars, moveNextMethodThisVar, moveNextExprWithJumpTable, setMachineStateExprR, afterMethodThisVar, afterMethodBodyExprR)
                        Some (env, remake2, pcExprOpt, codeExprR, m)
                | _ -> 
                    if sm_verbose then printfn "CONVERSION FAILURE: Didn't find struct machine lambdas ...moveNextExpr = %A, afterMethodExpr = %A" moveNextExpr.DebugText afterMethodExpr.DebugText
                    None
            | _ -> 
                //if sm_verbose then printfn "ACCEPTABLE CONVERSION FAILURE: recorded expansions but not a state machine expression at %A" expr.Range
                None
        else
            //if sm_verbose then printfn "ACCEPTABLE CONVERSION FAILURE: no prefix for state machine expression at %A" expr.Range
            None

    // A utility to add a jump table an expression
    let addPcJumpTable m (pcs: int list)  (pc2lab: Map<int, ILCodeLabel>) pcExpr expr =
        if pcs.IsEmpty then 
            expr
        else
            let initLabel = IL.generateCodeLabel()
            let mbuilder = new MatchBuilder(NoDebugPointAtInvisibleBinding, m )
            let mkGotoLabelTarget lab = mbuilder.AddResultTarget(Expr.Op (TOp.Goto lab, [], [], m), DebugPointForTarget.No)
            let dtree =
                TDSwitch(pcExpr,
                        [   // Yield one target for each PC, where the action of the target is to goto the appropriate label
                            for pc in pcs do
                                yield mkCase(DecisionTreeTest.Const(Const.Int32 pc), mkGotoLabelTarget pc2lab.[pc]) ],
                        // The default is to go to pcInit
                        Some(mkGotoLabelTarget initLabel),
                        m)

            let table = mbuilder.Close(dtree, m, g.int_ty)
            mkCompGenSequential m table (mkLabelled m initLabel expr)

    /// Detect constructs allowed in state machines
    let rec ConvertStateMachineCode env (pcExprOpt: Expr option) expr = 
        if sm_verbose then 
            printfn "---------ConvertStateMachineCode-------------------"
            printfn "%s" (DebugPrint.showExpr g expr)
            printfn "---------"
        
        let env, expr = RepeatBindAndApplyOuterMacros env expr
        
        if sm_verbose then 
            printfn "After RepeatBindAndApplyOuterMacros:\n%s" (DebugPrint.showExpr g expr)
            printfn "---------"

        // Detect the different permitted constructs in the expanded state machine
        let res = 
            match expr with 
            | ResumableEntryMatchExpr g (noneBranchExpr, someVar, someBranchExpr, _rebuild) ->
                if sm_verbose then printfn "ResumableEntryMatchExpr" 
                // printfn "found sequential"
                let reenterPC = genPC()
                let envSome = { env with Macros = env.Macros.Add someVar (mkInt g someVar.Range reenterPC) }
                let resNone = ConvertStateMachineCode env pcExprOpt noneBranchExpr
                let resSome = ConvertStateMachineCode envSome pcExprOpt someBranchExpr
                let asyncVars = unionFreeVars (freeInExpr CollectLocals resNone.phase1) resSome.asyncVars 
                let m = someBranchExpr.Range
                let recreate reenterLabOpt e1 e2 = 
                    let lab = (match reenterLabOpt with Some l -> l | _ -> IL.generateCodeLabel())
                    mkCond NoDebugPointAtStickyBinding DebugPointForTarget.No  m (tyOfExpr g noneBranchExpr) (mkFalse g m) (mkLabelled m lab e1) e2

                { phase1 = recreate None resNone.phase1 resSome.phase1
                  phase2 = (fun ctxt ->
                    let generate2 = resSome.phase2 ctxt
                    let generate1 = resNone.phase2 ctxt
                    let generate = recreate (Some ctxt.[reenterPC]) generate1 generate2
                    generate)
                  entryPoints= resSome.entryPoints @ [reenterPC] @ resNone.entryPoints
                  stateVars = resSome.stateVars @ resNone.stateVars 
                  thisVars = resSome.thisVars @ resNone.thisVars
                  asyncVars = asyncVars }

            | ResumeAtExpr g pcExpr ->
                if sm_verbose then printfn "ResumeAtExpr" 
                let m = expr.Range
                // Macro-evaluate the pcExpr
                let pcExprVal = ConvertStateMachineLeafExpression env pcExpr
                match pcExprVal with
                | Int32Expr contIdPC ->
                    let recreate contLabOpt =
                        Expr.Op (TOp.Goto (match contLabOpt with Some l -> l | _ -> IL.generateCodeLabel()), [], [], m)

                    { phase1 = recreate None 
                      phase2 = (fun ctxt ->
                        let generate = recreate (Some ctxt.[contIdPC]) 
                        generate)
                      entryPoints = []
                      stateVars = []
                      thisVars = []
                      asyncVars = emptyFreeVars }
                | _ -> 
                    error(Error(FSComp.SR.stateMachineResumeAtTargetNotStatic(), pcExpr.Range))

            // The expanded code for state machines may use sequential binding and sequential execution.
            //
            // let __stack_step = e1 in e2
            // e1; e2
            //
            // A binding 'let .. = ... in ... ' is considered part of the state machine logic 
            // if it uses a binding variable name of precisely '__stack_step'.
            // If this case 'e1' becomes part of the state machine too.
            | SequentialStateMachineCode g (e1, e2, _m, recreate) ->
                if sm_verbose then printfn "SequentialStateMachineCode" 
                // printfn "found sequential"
                let res1 = ConvertStateMachineCode env pcExprOpt e1
                let res2 = ConvertStateMachineCode env pcExprOpt e2
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
                  thisVars = res1.thisVars @ res2.thisVars
                  asyncVars = asyncVars }

            // The expanded code for state machines may use while loops...
            | WhileExpr (sp1, sp2, guardExpr, bodyExpr, m) ->
                if sm_verbose then printfn "WhileExpr" 

                let resg = ConvertStateMachineCode env pcExprOpt guardExpr
                let resb = ConvertStateMachineCode env pcExprOpt bodyExpr
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
                  thisVars = resg.thisVars @ resb.thisVars
                  asyncVars = asyncVars }

            // The expanded code for state machines should not normally contain try/finally as any resumptions will repeatedly execute the finally.
            // Hoever we include the synchronous version of the construct here for completeness.
            | TryFinallyExpr (sp1, sp2, ty, e1, e2, m) ->
                if sm_verbose then printfn "TryFinallyExpr" 
                let res1 = ConvertStateMachineCode env pcExprOpt e1
                let res2 = ConvertStateMachineCode env pcExprOpt e2
                let eps = res1.entryPoints @ res2.entryPoints
                if eps.Length > 0 then 
                    error(Error(FSComp.SR.stateMachineResumptionInTryFinally(), expr.Range))
                { phase1 = mkTryFinally g (res1.phase1, res2.phase1, m, ty, sp1, sp2)
                  phase2 = (fun ctxt -> 
                        let egR = res1.phase2 ctxt
                        let ebR = res2.phase2 ctxt
                        mkTryFinally g (egR, ebR, m, ty, sp1, sp2))
                  entryPoints= eps
                  stateVars = res1.stateVars @ res2.stateVars 
                  thisVars = res1.thisVars @ res2.thisVars
                  asyncVars = emptyFreeVars (* eps is empty, hence synchronous, no capture *)  }

            // The expanded code for state machines may use for loops....
            | ForLoopExpr (sp1, sp2, e1, e2, v, e3, m) ->
                if sm_verbose then printfn "ForLoopExpr" 
                let res1 = ConvertStateMachineCode env pcExprOpt e1
                let res2 = ConvertStateMachineCode env pcExprOpt e2
                let res3 = ConvertStateMachineCode env pcExprOpt e3
                let eps = res1.entryPoints @ res2.entryPoints @ res3.entryPoints
                if eps.Length > 0 then 
                    error(Error(FSComp.SR.stateMachineResumptionForLoop(), expr.Range))
                { phase1 = mkFor g (sp1, v, res1.phase1, sp2, res2.phase1, res3.phase1, m)
                  phase2 = (fun ctxt -> 
                        let e1R = res1.phase2 ctxt
                        let e2R = res2.phase2 ctxt
                        let e3R = res3.phase2 ctxt
                        mkFor g (sp1, v, e1R, sp2, e2R, e3R, m))
                  entryPoints= eps
                  stateVars = res1.stateVars @ res2.stateVars @ res3.stateVars
                  thisVars = res1.thisVars @ res2.thisVars @ res3.thisVars
                  asyncVars = emptyFreeVars (* eps is empty, hence synchronous, no capture *) }

            // The expanded code for state machines may use try/with....
            | TryCatchExpr (spTry, spWith, resTy, bodyExpr, filterVar, filterExpr, handlerVar, handlerExpr, m) ->
                if sm_verbose then printfn "TryCatchExpr" 
                let resBody = ConvertStateMachineCode env pcExprOpt bodyExpr
                let resFilter = ConvertStateMachineCode env pcExprOpt filterExpr
                let resHandler = ConvertStateMachineCode env pcExprOpt handlerExpr
                { phase1 = mkTryWith g (resBody.phase1, filterVar, resFilter.phase1, handlerVar, resHandler.phase1, m, resTy, spTry, spWith)
                  phase2 = (fun ctxt -> 
                    // We can't jump into a try/catch block.  So we jump to the start of the try/catch and add a new jump table
                    let pcsAndLabs = ctxt |> Map.toList  
                    let innerPcs = resBody.entryPoints 
                    if innerPcs.IsEmpty then 
                        let vBodyR = resBody.phase2 ctxt
                        let filterExprR = resFilter.phase2 ctxt
                        let handlerExprR = resHandler.phase2 ctxt
                        mkTryWith g (vBodyR, filterVar, filterExprR, handlerVar, handlerExprR, m, resTy, spTry, spWith)
                    else
                        let innerPcSet = innerPcs |> Set.ofList
                        let outerLabsForInnerPcs = pcsAndLabs |> List.filter (fun (pc, _outerLab) -> innerPcSet.Contains pc) |> List.map snd
                        // generate the inner labels
                        let pcsAndInnerLabs = pcsAndLabs |> List.map (fun (pc, l) -> (pc, if innerPcSet.Contains pc then IL.generateCodeLabel() else l))
                        let innerPc2Lab = Map.ofList pcsAndInnerLabs

                        let vBodyR = resBody.phase2 innerPc2Lab
                        let filterExprR = resFilter.phase2 ctxt
                        let handlerExprR = resHandler.phase2 ctxt

                        // Add a jump table at the entry to the try
                        let vBodyRWithJumpTable = 
                            match pcExprOpt with 
                            | None -> vBodyR
                            | Some pcExpr -> addPcJumpTable m innerPcs innerPc2Lab pcExpr vBodyR
                        let coreExpr = mkTryWith g (vBodyRWithJumpTable, filterVar, filterExprR, handlerVar, handlerExprR, m, resTy, spTry, spWith)
                        // Place all the outer labels just before the try
                        let labelledExpr = (coreExpr, outerLabsForInnerPcs) ||> List.fold (fun e l -> mkLabelled m l e)                

                        labelledExpr)
                  entryPoints= resBody.entryPoints @ resFilter.entryPoints @ resHandler.entryPoints 
                  stateVars = resBody.stateVars @ resFilter.stateVars @ resHandler.stateVars
                  thisVars = resBody.thisVars @ resFilter.thisVars @ resHandler.thisVars
                  asyncVars = unionFreeVars resBody.asyncVars (unionFreeVars(freeInExpr CollectLocals resFilter.phase1) (freeInExpr CollectLocals resHandler.phase1)) }

            // control-flow match
            | Expr.Match (spBind, exprm, dtree, targets, m, ty) ->
                if sm_verbose then printfn "MatchExpr" 
                // lower all the targets. 
                let dtreeR = ConvertStateMachineLeafDecisionTree env dtree 
                let tglArray = 
                    targets |> Array.map (fun (TTarget(_vs, targetExpr, _spTarget, _)) -> 
                        ConvertStateMachineCode env pcExprOpt targetExpr)
                let tgl = Array.toList tglArray
                let entryPoints = tgl |> List.collect (fun res -> res.entryPoints)
                let asyncVars =
                    (emptyFreeVars, Array.zip targets tglArray)
                    ||> Array.fold (fun fvs ((TTarget(_vs, _, _spTarget, _)), res) ->
                        if res.entryPoints.IsEmpty then fvs else unionFreeVars fvs res.asyncVars)
                let stateVars = 
                    (targets, tglArray) ||> Array.zip |> Array.toList |> List.collect (fun (TTarget(vs, _, _, _), res) -> 
                        let stateVars = vs |> List.filter (fun v -> res.asyncVars.FreeLocals.Contains(v)) |> List.map mkLocalValRef 
                        stateVars @ res.stateVars)
                let thisVars = tglArray |> Array.toList |> List.collect (fun res -> res.thisVars) 
                { phase1 = 
                    let gtgs =
                        (targets, tglArray) ||> Array.map2 (fun (TTarget(vs, _, spTarget, _)) res -> 
                            let flags = vs |> List.map (fun v -> res.asyncVars.FreeLocals.Contains(v)) 
                            TTarget(vs, res.phase1, spTarget, Some flags))
                    primMkMatch (spBind, exprm, dtreeR, gtgs, m, ty)

                  phase2 = (fun ctxt ->
                                let gtgs =
                                    (targets, tglArray) ||> Array.map2 (fun (TTarget(vs, _, spTarget, _)) res ->
                                        let flags = vs |> List.map (fun v -> res.asyncVars.FreeLocals.Contains(v)) 
                                        TTarget(vs, res.phase2 ctxt, spTarget, Some flags))
                                let generate = primMkMatch (spBind, exprm, dtreeR, gtgs, m, ty)
                                generate)

                  entryPoints = entryPoints
                  stateVars = stateVars
                  asyncVars = asyncVars 
                  thisVars = thisVars }

            // Non-control-flow let binding can appear as part of state machine. The body is considered state-machine code,
            // the expression being bound is not.
            | Expr.Let (bind, bodyExpr, m, _)
                  // Restriction: compilation of sequence expressions containing non-toplevel constrained generic functions is not supported
                  when  bind.Var.IsCompiledAsTopLevel || not (IsGenericValWithGenericConstraints g bind.Var) ->
                if sm_verbose then printfn "LetExpr (non-control-flow, rewrite rhs)" 

                // Rewrite the expression on the r.h.s. of the binding
                let bindExpr = ConvertStateMachineLeafExpression env bind.Expr
                let bind = mkBind bind.DebugPoint bind.Var bindExpr
                if sm_verbose then printfn "LetExpr (non-control-flow, body)" 

                let resBody = ConvertStateMachineCode env pcExprOpt bodyExpr

                // The isByrefTy check is an adhoc check to avoid capturing the 'this' parameter of a struct state machine 
                // You might think we could do this:
                //
                //    let sm = &this
                //    ... await point ...
                //    ... sm ....
                // However the 'sm' won't be set on that path.
                if bind.Var.IsCompiledAsTopLevel || not (resBody.asyncVars.FreeLocals.Contains(bind.Var)) || bind.Var.LogicalName.StartsWith prefixForVariablesThatMayNotBeEliminated then
                    if sm_verbose then printfn "LetExpr (non-control-flow, rewrite rhs, RepresentBindingAsTopLevelOrLocal)" 
                    RepresentBindingAsTopLevelOrLocal bind resBody m
                elif isByrefTy g bind.Var.Type then
                    match env.TemplateStructTy with 
                    | None -> error(Error(FSComp.SR.stateMachineResumptionByrefInStructMachine(), m))
                    | Some ty -> 
                        if typeEquiv g ty (destByrefTy g bind.Var.Type) then
                            RepresentBindingAsThis bind resBody m
                        else
                            error(Error(FSComp.SR.stateMachineResumptionByrefInStructMachine(), m))
                else
                    if sm_verbose then printfn "LetExpr (non-control-flow, rewrite rhs, RepresentBindingAsStateVar)" 
                    // printfn "found state variable %s" bind.Var.DisplayName
                    RepresentBindingAsStateVar bind resBody m

            // LetRec bindings may not appear as part of state machine.
            | Expr.LetRec _ -> 
                  error(Error(FSComp.SR.stateMachineResumptionByrefInStructMachine(), expr.Range))

            // Arbitrary expression
            | _ -> 
                let exprR = ConvertStateMachineLeafExpression env expr
                { phase1 = exprR
                  phase2 = (fun _ctxt -> exprR)
                  entryPoints = []
                  stateVars = []
                  thisVars = []
                  asyncVars = emptyFreeVars }

        if sm_verbose then 
            printfn "-------------------"
            printfn "Phase 1 Done for %s" (DebugPrint.showExpr g res.phase1)
            printfn "Phase 1 Done, asyncVars = %A" (res.asyncVars.FreeLocals |> Zset.elements |> List.map (fun v -> v.CompiledName(g.CompilerGlobalState)) |> String.concat ",")
            printfn "Phase 1 Done, stateVars = %A" (res.stateVars |> List.map (fun v -> v.CompiledName(g.CompilerGlobalState)) |> String.concat ",")
            printfn "Phase 1 Done, thisVars = %A" (res.thisVars |> List.map (fun v -> v.CompiledName(g.CompilerGlobalState)) |> String.concat ",")
            printfn "-------------------"
        res

    // Detect a state machine and convert it
    match overallExpr with
    | StateMachineInContext (env, remake, pcExprOpt, codeExpr, m) ->
        if (freeInExpr CollectLocals overallExpr).FreeLocals |> Zset.exists isExpandVar then 
            printfn "Abandoning: Not all macro variables expanded..."
            None
        else
            let pcExprROpt = pcExprOpt |> Option.map (ConvertStateMachineLeafExpression env)
        
            if sm_verbose then 
                printfn "Found state machine override method and code expression..."
                printfn "----------- OVERALL EXPRESSION FOR STATE MACHINE CONVERSION ----------------------"
                printfn "%s" (DebugPrint.showExpr g overallExpr)
                printfn "----------- INPUT TO STATE MACHINE CONVERSION ----------------------"
                printfn "%s" (DebugPrint.showExpr g codeExpr)
                printfn "----------- START STATE MACHINE CONVERSION ----------------------"
    
            // Perform phase1 of the conversion
            let phase1 = ConvertStateMachineCode env pcExprROpt codeExpr 

            // Work out the initial mapping of pcs to labels
            let pcs = [ 1 .. pcCount ]
            let labs = pcs |> List.map (fun _ -> IL.generateCodeLabel())
            let pc2lab  = Map.ofList (List.zip pcs labs)

            // Execute phase2, building the core of the method
            if sm_verbose then printfn "----------- PHASE2 ----------------------"

            // Perform phase2 to build the final expression
            let moveNextExprR = phase1.phase2 pc2lab

            if sm_verbose then printfn "----------- ADDING JUMP TABLE ----------------------"

            // Add the jump table
            let moveNextExprWithJumpTable = 
                match pcExprROpt with 
                | None -> moveNextExprR
                | Some pcExprR -> addPcJumpTable m pcs pc2lab pcExprR moveNextExprR
        
            if sm_verbose then printfn "----------- REMAKE ----------------------"

            // Build the result
            Some (remake (moveNextExprWithJumpTable, phase1.stateVars, phase1.thisVars))

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
        //if sm_verbose then printfn "----------- DONE ----------------------"
        
        //Some overallExprR

    | _ -> None

