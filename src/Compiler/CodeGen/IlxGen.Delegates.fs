// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Helpers for delegate construction in the ILX generator
[<AutoOpen>]
module internal FSharp.Compiler.IlxGenDelegates

open Internal.Utilities.Collections

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
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
// whose size matches its arity group into the group's elements; leave anything else (including a target with no
// arity info) untouched, which conservatively falls back to a closure.
let private flattenTupledArgs (vref: ValRef) (args: Expr list) =
    let arities = (arityOfVal vref.Deref).AritiesOfArgs

    if arities.Length <> args.Length then
        args
    else
        (arities, args)
        ||> List.map2 (fun arity arg ->
            match stripDebugPoints arg with
            | Expr.Op(TOp.Tuple _, _, elems, _) when arity >= 2 && elems.Length = arity -> elems
            | _ -> [ arg ])
        |> List.concat

// The forwarding call may be wrapped in let-bindings the recognizer must see through, in any combination:
//  - a 'let unitVar = () in …' that BindUnitVars (or the elaborator) inserts for the elided unit parameter
//    of a zero-parameter Invoke (e.g. System.Action), and
//  - a side-effect-free receiver binding 'let v = recv in v.M …' that a non-eta delegate over an instance
//    method value (e.g. Action(c.M)) produces to evaluate the receiver once.
// Peel them repeatedly: unit bindings are dropped; a receiver binding is dropped while recording 'v -> recv'
// so the receiver, which appears in the leading args as 'v', is mapped back to 'recv'. The downstream
// bindability check confirms 'recv' may be hoisted to the construction site.
let rec private peel g expr mapReceiver =
    match stripDebugPoints expr with
    | Expr.Let(TBind(v, rhs, _), inner, _, _) when isUnitTy g v.Type && isUnitValue rhs -> peel g inner mapReceiver
    | Expr.Let(TBind(v, recv, _), inner, _, _) when not (Optimizer.ExprHasEffect g recv) ->
        let mapReceiver leadingArgs =
            mapReceiver (
                leadingArgs
                |> List.map (fun a ->
                    match stripDebugPoints a with
                    | Expr.Val(vref, _, _) when valRefEq g vref (mkLocalValRef v) -> recv
                    | _ -> a)
            )

        peel g inner mapReceiver
    | e -> e, mapReceiver

let classifyForwardingTarget g (invokeParams: Val list) expr =
    let body, mapReceiver = peel g expr id

    // The trailing arguments of the call must be exactly the delegate's Invoke parameters, forwarded
    // verbatim and in order. The remaining leading arguments (e.g. an instance receiver) are returned
    // for the consumer to handle.
    let matchForwarding (args: Expr list) =
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
                        match stripDebugPoints a with
                        | Expr.Val(avref, _, _) -> valRefEq g avref (mkLocalValRef tv)
                        | _ -> false)
                    forwardedArgs
                    invokeParams
            then
                ValueSome(mapReceiver leadingArgs)
            else
                ValueNone
        else
            ValueNone

    match body with
    | Expr.App(Expr.Val(vref, valUseFlags, _), _, tyargs, args, _) ->
        match matchForwarding (flattenTupledArgs vref args) with
        | ValueSome leadingArgs -> DirectDelegateForwardingTargetCandidate.FSharpVal(vref, valUseFlags, tyargs, leadingArgs)
        | ValueNone -> DirectDelegateForwardingTargetCandidate.Other
    | Expr.Op(TOp.ILCall(isVirtual, _, isStruct, isCtor, valUseFlag, _, _, ilMethRef, enclTypeInst, methInst, _), _, args, _) ->
        match matchForwarding args with
        | ValueSome leadingArgs ->
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
        | ValueNone -> DirectDelegateForwardingTargetCandidate.Other
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

/// A leading argument of a *static* target is stored as the delegate's Target, which is typed `object`. A
/// value-type leading argument would have to be boxed (changing identity/copy semantics), which is not yet
/// supported, so it disqualifies the direct form (the same boxing gap that excludes value-type instance
/// receivers). Targets with no leading argument, or an instance receiver, are unaffected here.
let private staticLeadingArgIsRefType g takesInstanceArg (leadingArgs: Expr list) =
    match leadingArgs with
    | [ recv ] when not takesInstanceArg -> not (isStructTy g (tyOfExpr g recv))
    | _ -> true

/// The receiver is hoisted to the delegate-construction site, so a direct delegate is only sound when it can
/// be evaluated there exactly once and as the Target:
///  - A closure built from an explicit eta-lambda (e.g. `fun a -> recv.M a`) re-evaluates the receiver on
///    every Invoke; evaluating it once instead is observable unless it is side-effect free (a non-mutable
///    value, a constant, a pure field read).
///  - It must not reference the delegate's own Invoke parameters, which only exist inside the delegee method.
let private receiverBindable g (invokeParams: Val list) (leadingArgs: Expr list) =
    match leadingArgs with
    | [ recv ] ->
        let recvFreeLocals = (freeInExpr CollectLocals recv).FreeLocals

        not (Optimizer.ExprHasEffect g recv)
        && (not (invokeParams |> List.exists (fun tv -> Zset.contains tv recvFreeLocals)))
    | _ -> true

/// An F# value target can be pointed at directly only when the call needs no witnesses and is not a
/// constructor / base / self-init call, when the receiver shape is right, and when the receiver can be safely
/// hoisted to the construction site. Only the witness fact is passed in (it needs the IlxGen environment); the
/// newobj / super-init / self-init / instance-receiver facts are derived here from the member's call info, and
/// the base-call flag from valUseFlags.
let fsharpValDirectlyBindable g (invokeParams: Val list) (leadingArgs: Expr list) (vrefM: ValRef) (valUseFlags: ValUseFlag) hasWitnesses =
    let _, _, newobj, isSuperInit, isSelfInit, takesInstanceArg, _, _ =
        GetMemberCallInfo g (vrefM, valUseFlags)

    not hasWitnesses
    && not newobj
    && not isSuperInit
    && not isSelfInit
    && not valUseFlags.IsVSlotDirectCall
    && receiverShapeOk leadingArgs takesInstanceArg
    && receiverBindable g invokeParams leadingArgs
    && staticLeadingArgIsRefType g takesInstanceArg leadingArgs

/// An IL method target can be pointed at directly only when it is not a constructor / base / constrained call,
/// when the receiver shape is right, when the receiver is not a value type (boxing is not yet implemented), and
/// when the receiver can be safely hoisted to the construction site. The instance-receiver flag is derived here
/// from ilMethRef and the base/constrained-call flags from valUseFlag.
let ilMethodDirectlyBindable
    g
    (invokeParams: Val list)
    (leadingArgs: Expr list)
    (ilMethRef: ILMethodRef)
    isStruct
    (valUseFlag: ValUseFlag)
    isCtor
    =
    let takesInstanceArg = ilMethRef.CallingConv.IsInstance

    not isCtor
    && not valUseFlag.IsVSlotDirectCall
    && not valUseFlag.IsPossibleConstrainedCall
    && not (takesInstanceArg && isStruct)
    && receiverShapeOk leadingArgs takesInstanceArg
    && receiverBindable g invokeParams leadingArgs
    && staticLeadingArgIsRefType g takesInstanceArg leadingArgs

/// Confirm the target's IL signature is compatible with the delegate's Invoke. The type checker has already
/// verified the delegate is built from a compatible function shape and the forwarding match pins the arity,
/// so this is the residual check:
///  - parameter *count* matches (essentially a sanity check). Parameter IL *types* are deliberately not
///    compared: a value-type parameter is exact by construction (a widening/boxing argument would not be a
///    verbatim Val and so would not have matched), and for reference-type parameters the CLR's delegate
///    relaxation permits contravariance, so an exact comparison would only produce false negatives.
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
    (ilDelegeeParams: ILParameter list)
    (ilDelegeeRet: ILReturn)
    (ilEnclArgTys: ILType list)
    (ilMethArgTys: ILType list)
    (targetMspec: ILMethodSpec)
    =
    let arityMatches =
        targetMspec.FormalArgTypes.Length - numBoundLeadingFormals = ilDelegeeParams.Length

    let returnMatches =
        if List.isEmpty ilEnclArgTys && List.isEmpty ilMethArgTys then
            ilDelegeeRet.Type = targetMspec.FormalReturnType
        else
            true

    arityMatches && returnMatches

/// Package the receiver (if any) with the virtual-call flag for the consumer to emit.
let receiverInfo (leadingArgs: Expr list) virtualCall =
    match leadingArgs with
    | [ recv ] -> Some(recv, virtualCall)
    | _ -> None
