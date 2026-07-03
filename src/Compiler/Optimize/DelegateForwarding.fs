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

// Mirror the code generator's arity-based de-tupling (a tupled argument group is one tuple node in the
// call but separate IL parameters in the compiled target) so the match sees the flattened argument list.
// The group count must equal the target's arity exactly: fewer is a partial application, more an
// over-application whose trailing arguments are consumed by the target's *result*, and a target without
// arity information has no compiled method to point at.
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

let rec private resolveAliases (aliases: ValMap<Expr>) e =
    let e = stripDebugPoints e

    match e with
    | Expr.Val(vref, _, _) ->
        match aliases.TryFind vref.Deref with
        | Some e2 -> resolveAliases aliases e2
        | None -> e
    | _ -> e

// Trailing arguments must be the delegate's Invoke parameters, verbatim and in order; the leading rest
// (e.g. an instance receiver) is resolved and returned for the caller to check and emit.
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
            // A struct receiver arrives by address; recover the value so the emit can box it as the
            // Target (invocation reaches 'this' through the runtime's unboxing stub).
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

// Peel the wrappers the elaborator and BuildNewDelegateExpr leave around the forwarding call: effect-free
// let-bindings, applications of let-wrapped or immediate lambdas (method-group coercions, the shells of
// curried member calls), and curried application nesting. The optimizer reduces these only while already
// making inlining decisions - too late for a recognizer that must precede them - so peel by aliasing:
// each bound value maps to the expression flowing into it, resolved when the arguments are matched.
// Anything else is left in place and fails the match, conservatively keeping the closure.
let rec private stripToForwardingCall exprHasEffect g (aliases: ValMap<Expr>) expr =
    match stripDebugPoints expr with
    | Expr.Let(TBind(v, rhs, _), inner, _, _) when not (exprHasEffect g rhs) ->
        stripToForwardingCall exprHasEffect g (aliases.Add v rhs) inner
    | Expr.App(f, fty, tyargs, args, m) as app ->
        match stripDebugPoints f with
        | Expr.Let(TBind(v, rhs, _), f2, _, _) when not (exprHasEffect g rhs) ->
            stripToForwardingCall exprHasEffect g (aliases.Add v rhs) (Expr.App(f2, fty, tyargs, args, m))
        | Expr.Lambda(_, None, None, [ v ], body, _, _) when List.isEmpty tyargs ->
            match args with
            | a :: rest when not (exprHasEffect g a) ->
                let aliases = aliases.Add v a

                match rest with
                | [] -> stripToForwardingCall exprHasEffect g aliases body
                | _ -> stripToForwardingCall exprHasEffect g aliases (Expr.App(body, tyOfExpr g body, [], rest, m))
            | _ -> app, aliases
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

/// At most one leading argument can become the delegate's Target: the receiver of an instance target, or
/// the first parameter of a static one via the CLR's "closed over the first argument" delegate form
/// (extension-member receivers, one-argument partial applications). More has no closed form.
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

/// The closed-delegate thunk passes the Target into a static target's first parameter with no unboxing,
/// so a value-type leading argument has no closed form (unlike an instance receiver's unboxing stub).
let private staticLeadingArgIsRefType g takesInstanceArg (leadingArgs: Expr list) =
    match leadingArgs with
    | [ recv ] when not takesInstanceArg -> not (isStructTy g (tyOfExpr g recv))
    | _ -> true

/// Boxing the receiver needs it as a value: '&localVar' is recovered by the recognizer, but any other
/// address form leaves a byref the box cannot consume.
let private receiverNotByref g (leadingArgs: Expr list) =
    match leadingArgs with
    | [ recv ] -> not (isByrefTy g (tyOfExpr g recv))
    | _ -> true

/// The receiver is evaluated once at the construction site rather than on every Invoke, which is only
/// unobservable when it is effect-free; and it must not reference the Invoke parameters, which exist
/// only inside the delegee.
let private receiverBindable exprHasEffect g (invokeParams: Val list) (leadingArgs: Expr list) =
    match leadingArgs with
    | [ recv ] ->
        let recvFreeLocals = (freeInExpr CollectLocals recv).FreeLocals

        not (exprHasEffect g recv)
        && (not (invokeParams |> List.exists (fun tv -> Zset.contains tv recvFreeLocals)))
    | _ -> true

/// Returns the virtual-call and instance-receiver facts derived from the member call info when the
/// target is directly bindable. Witnesses are passed in: computing them needs the IlxGen environment.
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

/// Residual IL compatibility check; the type checker verified the call and the forwarding match pinned
/// the shape. Parameter types are deliberately not compared - value types are exact by construction and
/// reference types may use the CLR's contravariant delegate relaxation - only their count, minus any
/// leading formals consumed by a bound Target. The return type must match exactly for a non-generic
/// target (the CLR does not relax e.g. 'void' against 'Unit'); a generic target's return is written in
/// type variables, where no exact comparison is meaningful.
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

let receiverInfo (leadingArgs: Expr list) virtualCall =
    match leadingArgs with
    | [ recv ] -> Some(recv, virtualCall)
    | _ -> None
