// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Helpers for delegate construction in the ILX generator
[<AutoOpen>]
module internal FSharp.Compiler.IlxGenDelegates

open Internal.Utilities.Collections
open Internal.Utilities.Library.Extras

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

/// The target of a delegate forwarding call
[<RequireQualifiedAccess>]
type DelegateForwardingTarget =
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

let classifyForwardingTarget g (invokeParams: Val list) expr =
    // The trailing arguments of the call must be exactly the delegate's Invoke parameters, forwarded
    // verbatim and in order. The remaining leading arguments (e.g. an instance receiver) are returned
    // for the consumer to handle.
    let matchForwarding (args: Expr list) =
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
                ValueSome leadingArgs
            else
                ValueNone
        else
            ValueNone

    match stripDebugPoints expr with
    | Expr.App(Expr.Val(vref, valUseFlags, _), _, tyargs, args, _) ->
        match matchForwarding args with
        | ValueSome leadingArgs -> DelegateForwardingTarget.FSharpVal(vref, valUseFlags, tyargs, leadingArgs)
        | ValueNone -> DelegateForwardingTarget.Other
    | Expr.Op(TOp.ILCall(isVirtual, _, isStruct, isCtor, valUseFlag, _, _, ilMethRef, enclTypeInst, methInst, _), _, args, _) ->
        match matchForwarding args with
        | ValueSome leadingArgs ->
            DelegateForwardingTarget.ILMethod(isVirtual, isStruct, isCtor, valUseFlag, ilMethRef, enclTypeInst, methInst, leadingArgs)
        | ValueNone -> DelegateForwardingTarget.Other
    | _ -> DelegateForwardingTarget.Other

/// For an instance method the single leading argument is the receiver; a static method or module function
/// must have no leading arguments (otherwise it is a partial application).
let private receiverShapeOk (leadingArgs: Expr list) takesInstanceArg =
    if takesInstanceArg then
        match leadingArgs with
        | [ _ ] -> true
        | _ -> false
    else
        List.isEmpty leadingArgs

/// The receiver is hoisted to the delegate-construction site, so a direct delegate is only sound when it can
/// be evaluated there exactly once and as the Target:
///  - A closure built from an explicit eta-lambda (e.g. `fun a -> recv.M a`) re-evaluates the receiver on
///    every Invoke; evaluating it once instead is observable unless it is side-effect free (a non-mutable
///    value, a constant, a pure field read).
///  - It must not reference the delegate's own Invoke parameters, which only exist inside the delegee method.
/// ExprHasEffect short-circuits before the freeInExpr allocation, so disqualified delegates skip the traversal.
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
    && receiverShapeOk leadingArgs takesInstanceArg
    && not (takesInstanceArg && isStruct)
    && receiverBindable g invokeParams leadingArgs

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
let signatureMatches
    (ilDelegeeParams: ILParameter list)
    (ilDelegeeRet: ILReturn)
    (ilEnclArgTys: ILType list)
    (ilMethArgTys: ILType list)
    (targetMspec: ILMethodSpec)
    =
    let arityMatches = targetMspec.FormalArgTypes.Length = ilDelegeeParams.Length

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
