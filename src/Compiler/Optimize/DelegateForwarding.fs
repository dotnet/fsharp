// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Recognition of delegate constructions whose Invoke body is a transparent forwarding call to a known
/// method, shared by the optimizer (which preserves the call from inlining) and the ILX generator (which
/// points the delegate directly at the target). The 'exprHasEffect' parameter is Optimizer.ExprHasEffect;
/// it is passed in because this file compiles before the optimizer.
module internal FSharp.Compiler.DelegateForwarding

open Internal.Utilities.Collections

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

/// A delegate target that can potentially be forwarded to directly, without an intermediate closure
[<RequireQualifiedAccess>]
type DirectDelegateForwardingTargetCandidate =
    /// A known F# value: a module-level function or a member
    | FSharpVal of vref: ValRef * valUseFlags: ValUseFlag * tyargs: TypeInst * leadingArgs: Expr list
    /// A direct IL method call (e.g. a BCL method)
    | ILMethod of
        isVirtual: bool *
        isStruct: bool *
        isCtor: bool *
        valUseFlags: ValUseFlag *
        ilMethRef: ILMethodRef *
        enclTypeInst: TypeInst *
        methInst: TypeInst *
        leadingArgs: Expr list
    | Other

let private isUnitValue e =
    match stripDebugPoints e with
    | Expr.Const(Const.Unit, _, _) -> true
    | _ -> false

// The forwarding call of a *tupled* target carries each tupled argument group as a single tuple node: e.g. for
// `accTupled (x, y)` (a value of arity [2]) the call `accTupled (a, b)` is one `(a, b)` argument, exactly the
// shape BuildFSharpMethodApp produced from the value's arity. The code generator de-tuples these by the value's
// arity when it emits the call, so the compiled target takes the elements as separate IL parameters and is
// perfectly compatible with the delegate (a tupled target is as direct-able as a curried one). Mirror that
// de-tupling here so the forwarding match sees the flattened argument list: expand a tuple-construction argument
// whose size matches its arity group into the group's elements.
//
// The argument-group count must equal the target's arity *exactly*: fewer groups is a partial application of
// a group, and more is an over-application - e.g. 'Action(failwith "x")' eta-expands to '(failwith "x") ()' -
// where the extra trailing arguments are consumed by the target's *result*, not by the target itself, so
// there is no saturated call to point the delegate at. (This also rejects targets with no arity information,
// such as parameters and locals, which have no compiled method form.) A mismatch fails the match and
// conservatively keeps the closure.
let private tryFlattenTupledArgs (vref: ValRef) (args: Expr list) =
    let arities = (arityOfVal vref.Deref).AritiesOfArgs

    if arities.Length <> args.Length then
        None
    else
        (arities, args)
        ||> List.map2 (fun arity arg ->
            match stripDebugPoints arg with
            | Expr.Op(TOp.Tuple _, _, elems, _) when arity >= 2 && elems.Length = arity -> elems
            | _ -> [ arg ])
        |> List.concat
        |> Some

/// Resolve a value reference through the aliases recorded while peeling wrappers off the forwarding
/// call, so the match sees the expression that actually flows into the call.
let rec private resolveAliases (aliases: ValMap<Expr>) e =
    let e = stripDebugPoints e

    match e with
    | Expr.Val(vref, _, _) ->
        match aliases.TryFind vref.Deref with
        | Some e2 -> resolveAliases aliases e2
        | None -> e
    | _ -> e

// The trailing arguments of the call must be exactly the delegate's Invoke parameters, forwarded
// verbatim and in order. The remaining leading arguments (e.g. an instance receiver) are resolved
// and returned for the consumer to check and emit.
let private matchForwarding g (aliases: ValMap<Expr>) (invokeParams: Val list) (args: Expr list) =
    let args = args |> List.map (resolveAliases aliases)

    // Drop the elided unit argument when the Invoke takes no parameters.
    let args =
        match List.tryLast args with
        | Some last when List.isEmpty invokeParams && isUnitValue last -> List.truncate (args.Length - 1) args
        | _ -> args

    let numLeading = args.Length - invokeParams.Length

    if numLeading >= 0 then
        let leadingArgs, forwardedArgs = List.splitAt numLeading args

        if
            List.forall2
                (fun (a: Expr) (tv: Val) ->
                    match a with
                    | Expr.Val(avref, _, _) -> valRefEq g avref (mkLocalValRef tv)
                    | _ -> false)
                forwardedArgs
                invokeParams
        then
            // A value-type receiver is passed to its instance method by address ('&recv'); recover the
            // underlying value so it can be boxed and stored as the delegate Target (the unboxing stub
            // supplies the 'this' pointer on invocation), then resolve it in case the value was itself
            // bound by a peeled binding.
            let leadingArgs =
                leadingArgs
                |> List.map (fun a ->
                    match a with
                    | Expr.Op(TOp.LValueOp(LAddrOf _, vref), _, _, m) -> resolveAliases aliases (exprForValRef m vref)
                    | _ -> a)

            Some leadingArgs
        else
            None
    else
        None

// The forwarding call may be wrapped in shapes the elaborator and BuildNewDelegateExpr produce before any
// optimizer normalization has run, in any combination:
//  - effect-free let-bindings over the call: 'let unitVar = () in …' for the elided unit parameter of a
//    zero-parameter Invoke (e.g. System.Action), and 'let v = recv in v.M …' evaluating the receiver of a
//    non-eta delegate once;
//  - an application of a let-wrapped or immediate lambda: the method-group coercion of a non-eta delegate
//    over an instance method ('(let v = recv in fun x y -> v.M x y) p0 p1') and the partial-application
//    shells of a curried member call. The optimizer beta-reduces these only while it is already making
//    inlining decisions, which is too late for the recognizer run that must precede those decisions;
//  - curried applications ('(f p0) p1'), which flatten to a single application.
// Peel them by *aliasing*: each effect-free binding (from a let or a beta-reduced lambda parameter) maps
// the bound value to the expression flowing into it, to be resolved when the arguments are matched.
// Anything not covered (an effectful binding or argument, an exotic function position) is left in place
// and fails the match, conservatively keeping the closure. The downstream bindability checks run on the
// resolved (leading) arguments, so a receiver reached through an alias is still checked before being
// hoisted.
let rec private stripToForwardingCall exprHasEffect g (aliases: ValMap<Expr>) expr =
    match stripDebugPoints expr with
    | Expr.Let(TBind(v, rhs, _), inner, _, _) when not (exprHasEffect g rhs) ->
        stripToForwardingCall exprHasEffect g (aliases.Add v rhs) inner
    | Expr.App(f, fty, tyargs, args, m) as app ->
        match stripDebugPoints f with
        // '(let v = e in f2) args': lift the binding off the function position
        | Expr.Let(TBind(v, rhs, _), f2, _, _) when not (exprHasEffect g rhs) ->
            stripToForwardingCall exprHasEffect g (aliases.Add v rhs) (Expr.App(f2, fty, tyargs, args, m))
        // '(fun v -> body) arg …': beta-reduce by aliasing, one parameter at a time
        | Expr.Lambda(_, None, None, [ v ], body, _, _) when List.isEmpty tyargs ->
            match args with
            | a :: rest when not (exprHasEffect g a) ->
                let aliases = aliases.Add v a

                match rest with
                | [] -> stripToForwardingCall exprHasEffect g aliases body
                | _ -> stripToForwardingCall exprHasEffect g aliases (Expr.App(body, tyOfExpr g body, [], rest, m))
            | _ -> app, aliases
        // '(f2 args2) args': flatten the curried application
        | Expr.App(f2, f2ty, tyargs2, args2, _) when List.isEmpty tyargs ->
            stripToForwardingCall exprHasEffect g aliases (Expr.App(f2, f2ty, tyargs2, args2 @ args, m))
        | _ -> app, aliases
    | e -> e, aliases

let classifyForwardingTarget exprHasEffect g (invokeParams: Val list) expr =
    let call, aliases = stripToForwardingCall exprHasEffect g ValMap<Expr>.Empty expr

    match call with
    | Expr.App(f, _, tyargs, args, _) ->
        match stripDebugPoints f with
        | Expr.Val(vref, valUseFlags, _) ->
            match
                tryFlattenTupledArgs vref args
                |> Option.bind (matchForwarding g aliases invokeParams)
            with
            | Some leadingArgs -> DirectDelegateForwardingTargetCandidate.FSharpVal(vref, valUseFlags, tyargs, leadingArgs)
            | None -> DirectDelegateForwardingTargetCandidate.Other
        | _ -> DirectDelegateForwardingTargetCandidate.Other
    | Expr.Op(TOp.ILCall(isVirtual, _, isStruct, isCtor, valUseFlag, _, _, ilMethRef, enclTypeInst, methInst, _), _, args, _) ->
        match matchForwarding g aliases invokeParams args with
        | Some leadingArgs ->
            DirectDelegateForwardingTargetCandidate.ILMethod(
                isVirtual,
                isStruct,
                isCtor,
                valUseFlag,
                ilMethRef,
                enclTypeInst,
                methInst,
                leadingArgs
            )
        | None -> DirectDelegateForwardingTargetCandidate.Other
    | _ -> DirectDelegateForwardingTargetCandidate.Other

/// At most one leading argument can be hoisted to the construction site and stored as the delegate's Target:
///  - for an instance method it is the receiver (not part of the IL signature), and
///  - for a static method it is the first IL parameter, bound via the CLR's "closed over the first argument"
///    delegate mechanism (this is how an extension member's receiver, or a one-argument partial application of
///    a static method / module function, becomes direct).
/// Two or more leading arguments (e.g. a partial application that also fixes a receiver) have no closed
/// direct-delegate form, so they stay a closure.
let private receiverShapeOk (leadingArgs: Expr list) takesInstanceArg =
    if takesInstanceArg then
        match leadingArgs with
        | [ _ ] -> true
        | _ -> false
    else
        match leadingArgs with
        | []
        | [ _ ] -> true
        | _ -> false

/// A leading argument of a *static* target is stored as the delegate's Target (typed `object`) and the
/// delegate thunk passes it straight into the method's first by-value parameter with no unboxing. A value-type
/// leading argument therefore has no closed form at all, so it disqualifies the direct form. (This is unlike a
/// value-type *instance* receiver, which is boxed and reached through the method's unboxing stub - see the
/// emit - and so is supported.) Targets with no leading argument, or an instance receiver, are unaffected here.
let private staticLeadingArgIsRefType g takesInstanceArg (leadingArgs: Expr list) =
    match leadingArgs with
    | [ recv ] when not takesInstanceArg -> not (isStructTy g (tyOfExpr g recv))
    | _ -> true

/// A value-type receiver is boxed at the construction site, which needs the receiver as a *value*. A struct
/// receiver is normally taken by address ('&recv'); the recognizer recovers the value from '&localVar', but any
/// other address form (e.g. '&someField') would leave a byref the box cannot consume, so disqualify it and keep
/// the closure.
let private receiverNotByref g (leadingArgs: Expr list) =
    match leadingArgs with
    | [ recv ] -> not (isByrefTy g (tyOfExpr g recv))
    | _ -> true

/// The receiver is hoisted to the delegate-construction site, so a direct delegate is only sound when it can
/// be evaluated there exactly once and as the Target:
///  - A closure built from an explicit eta-lambda (e.g. `fun a -> recv.M a`) re-evaluates the receiver on
///    every Invoke; evaluating it once instead is observable unless it is side-effect free (a non-mutable
///    value, a constant, a pure field read).
///  - It must not reference the delegate's own Invoke parameters, which only exist inside the delegee method.
let private receiverBindable exprHasEffect g (invokeParams: Val list) (leadingArgs: Expr list) =
    match leadingArgs with
    | [ recv ] ->
        let recvFreeLocals = (freeInExpr CollectLocals recv).FreeLocals

        not (exprHasEffect g recv)
        && (not (invokeParams |> List.exists (fun tv -> Zset.contains tv recvFreeLocals)))
    | _ -> true

/// An F# value target can be pointed at directly only when the call needs no witnesses and is not a
/// constructor / base / self-init call, when the receiver shape is right, and when the receiver can be safely
/// hoisted to the construction site. Only the witness fact is passed in (it needs the IlxGen environment); the
/// newobj / super-init / self-init / instance-receiver facts are derived here from the member's call info, and
/// the base-call flag from valUseFlags. When directly bindable, returns the virtual-call and instance-receiver
/// facts already derived from the call info, so the caller does not re-derive them.
let fsharpValDirectlyBindable
    exprHasEffect
    g
    (invokeParams: Val list)
    (leadingArgs: Expr list)
    (vrefM: ValRef)
    (valUseFlags: ValUseFlag)
    hasWitnesses
    =
    let _, virtualCall, newobj, isSuperInit, isSelfInit, takesInstanceArg, _, _ =
        GetMemberCallInfo g (vrefM, valUseFlags)

    if
        not hasWitnesses
        && not newobj
        && not isSuperInit
        && not isSelfInit
        && not valUseFlags.IsVSlotDirectCall
        && receiverShapeOk leadingArgs takesInstanceArg
        && receiverBindable exprHasEffect g invokeParams leadingArgs
        && staticLeadingArgIsRefType g takesInstanceArg leadingArgs
        && receiverNotByref g leadingArgs
    then
        ValueSome(virtualCall, takesInstanceArg)
    else
        ValueNone

/// An IL method target can be pointed at directly only when it is not a constructor / base / constrained call,
/// when the receiver shape is right, and when the receiver can be safely hoisted to the construction site. A
/// value-type instance receiver is boxed at the construction site (see the emit), matching the closure's
/// by-value capture. The instance-receiver flag is derived here from ilMethRef and the base/constrained-call
/// flags from valUseFlag.
let ilMethodDirectlyBindable
    exprHasEffect
    g
    (invokeParams: Val list)
    (leadingArgs: Expr list)
    (ilMethRef: ILMethodRef)
    (valUseFlag: ValUseFlag)
    isCtor
    =
    let takesInstanceArg = ilMethRef.CallingConv.IsInstance

    not isCtor
    && not valUseFlag.IsVSlotDirectCall
    && not valUseFlag.IsPossibleConstrainedCall
    && receiverShapeOk leadingArgs takesInstanceArg
    && receiverBindable exprHasEffect g invokeParams leadingArgs
    && staticLeadingArgIsRefType g takesInstanceArg leadingArgs
    && receiverNotByref g leadingArgs

/// Confirm the target's IL signature is compatible with the delegate's Invoke. The type checker has already
/// verified the delegate is built from a compatible function shape and the forwarding match pins the arity,
/// so this is the residual check:
///  - parameter *count* matches (essentially a sanity check). Parameter IL *types* are deliberately not
///    compared (hence only the count is taken): a value-type parameter is exact by construction (a
///    widening/boxing argument would not be a verbatim Val and so would not have matched), and for
///    reference-type parameters the CLR's delegate relaxation permits contravariance, so an exact
///    comparison would only produce false negatives.
///  - for a non-generic target, the return type matches exactly. This is the one IL mismatch the recognizer
///    cannot see and the CLR will not relax: notably an F# 'unit' return compiled to 'void' versus a delegate
///    whose Invoke returns 'Unit'. For a generic target the return is written in terms of type variables, so
///    no exact comparison is meaningful.
///
/// 'numBoundLeadingFormals' is the number of leading IL parameters consumed by a hoisted Target (1 for a
/// static method whose first argument is bound, e.g. an extension receiver; 0 otherwise - an instance
/// receiver is not part of the IL signature), so it is subtracted from the target's formal-argument count.
let signatureMatches
    numBoundLeadingFormals
    (numDelegeeParams: int)
    (ilDelegeeRetTy: ILType)
    (ilEnclArgTys: ILType list)
    (ilMethArgTys: ILType list)
    (targetMspec: ILMethodSpec)
    =
    let arityMatches =
        targetMspec.FormalArgTypes.Length - numBoundLeadingFormals = numDelegeeParams

    let returnMatches =
        if List.isEmpty ilEnclArgTys && List.isEmpty ilMethArgTys then
            ilDelegeeRetTy = targetMspec.FormalReturnType
        else
            true

    arityMatches && returnMatches

/// Package the receiver (if any) with the virtual-call flag for the consumer to emit.
let receiverInfo (leadingArgs: Expr list) virtualCall =
    match leadingArgs with
    | [ recv ] -> Some(recv, virtualCall)
    | _ -> None
