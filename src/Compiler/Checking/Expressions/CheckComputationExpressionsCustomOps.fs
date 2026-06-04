// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Capture-and-replay machinery used by `TcComputationExpression` to sink the *resolved*
/// `MethInfo` for overloaded `[<CustomOperation>]` keywords at the keyword's source range
/// (instead of the first-registered overload), so QuickInfo and `GetAllUsesOfAllSymbolsInFile`
/// behave the same way they do for regular overloaded method calls.
///
/// The "emit a fallback `Item.X` early then `CallNameResolutionSinkReplacing` once the
/// final resolution is known" idiom is the cross-range generalisation of what
/// `TcMethodItemThen` (in `CheckExpressions.fs`) already does at a single range for
/// type-providers static arguments. Here the early sink lands at the keyword range while
/// overload resolution fires at a different synthetic call range, hence the sink wrapper.
///
/// See https://github.com/dotnet/fsharp/issues/11612 and https://github.com/dotnet/fsharp/issues/15206.
module internal FSharp.Compiler.CheckComputationExpressionsCustomOps

open System.Collections.Generic
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

/// Information about a `[<CustomOperation>]` keyword usage whose `Item.CustomOperation`
/// sink record needs to be upgraded once normal overload resolution picks an overload.
/// Fully immutable: captured resolutions live in the side dictionary set up by
/// `captureCustomOperationOverloads`, keyed by `SyntheticCallRange`.
[<NoComparison; NoEquality>]
type DeferredCustomOpSink =
    {
        KeywordRange: range
        OpName: string
        UsageText: unit -> string option
        SyntheticCallRange: range
        Fallback: MethInfo
        NameEnv: NameResolutionEnv
        AccessRights: AccessorDomain
    }

/// Sink wrapper that forwards every notification to `forwardTo` and additionally records,
/// for every tracked synthetic call range, the singleton `Item.MethodGroup` resolution that
/// lands at it. The synthetic range can collide with outer-comprehension calls (e.g. `For`)
/// at the same range, so we also check the method name matches the expected fallback's
/// `LogicalName` — that's an O(1) string compare, no `MethInfosEquivByNameAndSig` on the
/// hot path.
///
/// `forwardTo` is non-optional: callers must gate construction on a real outer sink.
let private makeCustomOpResolutionCapturingSink
    (forwardTo: ITypecheckResultsSink)
    (capturedResolutions: Dictionary<range, string * MethInfo * TyparInstantiation>)
    : ITypecheckResultsSink =

    let tryCapture (m: range) (item: Item) (tpinst: TyparInstantiation) =
        match item with
        | Item.MethodGroup(name, [ mi ], _) ->
            match capturedResolutions.TryGetValue m with
            | true, (expectedName, _, _) when name = expectedName -> capturedResolutions[m] <- (expectedName, mi, tpinst)
            | _ -> ()
        | _ -> ()

    { new ITypecheckResultsSink with
        member _.NotifyEnvWithScope(m, nenv, ad) =
            forwardTo.NotifyEnvWithScope(m, nenv, ad)

        member _.NotifyExprHasType(ty, nenv, ad, m) =
            forwardTo.NotifyExprHasType(ty, nenv, ad, m)

        member _.NotifyExprHasTypeSynthetic(ty, nenv, ad, m) =
            forwardTo.NotifyExprHasTypeSynthetic(ty, nenv, ad, m)

        member _.NotifyNameResolution(endPos, item, tpinst, occurrenceType, nenv, ad, m, replace) =
            tryCapture m item tpinst
            forwardTo.NotifyNameResolution(endPos, item, tpinst, occurrenceType, nenv, ad, m, replace)

        member _.NotifyMethodGroupNameResolution(endPos, item, itemMethodGroup, tpinst, occurrenceType, nenv, ad, m, replace) =
            tryCapture m item tpinst
            forwardTo.NotifyMethodGroupNameResolution(endPos, item, itemMethodGroup, tpinst, occurrenceType, nenv, ad, m, replace)

        member _.NotifyFormatSpecifierLocation(m, numArgs) =
            forwardTo.NotifyFormatSpecifierLocation(m, numArgs)

        member _.NotifyRelatedSymbolUse(m, item, kind) =
            forwardTo.NotifyRelatedSymbolUse(m, item, kind)

        member _.NotifyOpenDeclaration openDeclaration =
            forwardTo.NotifyOpenDeclaration openDeclaration

        member _.CurrentSourceText = forwardTo.CurrentSourceText

        member _.FormatStringCheckContext = forwardTo.FormatStringCheckContext
    }

/// Eagerly sink an `Item.CustomOperation` at the keyword range using the fallback `MethInfo`
/// (`opDatas[0]`) and enqueue the entry for later upgrade once overload resolution finishes.
/// The early sink preserves the byte-for-byte sink ordering that downstream consumers
/// (e.g. `Test Project12 all symbols`) rely on for single-overload custom operations.
let enqueueDeferredCustomOpSink
    (sink: TcResultsSink)
    (nenv: NameResolutionEnv)
    (ad: AccessorDomain)
    (queue: ResizeArray<DeferredCustomOpSink>)
    (nm: Ident)
    opName
    usageText
    syntheticCallRange
    (fallback: MethInfo)
    =
    let fallbackItem = Item.CustomOperation(opName, usageText, Some fallback)

    CallNameResolutionSink sink (nm.idRange, nenv, fallbackItem, emptyTyparInst, ItemOccurrence.Use, ad)

    queue.Add
        {
            KeywordRange = nm.idRange
            OpName = opName
            UsageText = usageText
            SyntheticCallRange = syntheticCallRange
            Fallback = fallback
            NameEnv = nenv
            AccessRights = ad
        }

/// Run `action` (typically the `TcExpr` of the desugared CE lambda) with a sink wrapper
/// installed that captures the resolved `MethInfo` for each enqueued custom-operation
/// keyword. After `action` returns, replace each early sink record whose captured overload
/// is a *different* method definition from the fallback with one carrying the resolved
/// overload. Single-overload CEs leave the eager fallback record untouched (`Replacing`
/// would reorder it and break `Test Project12 all symbols`).
///
/// Short-circuits the wrapping and draining entirely when no custom operations were
/// enqueued (most async/task/seq/option/list CEs) or when no IDE sink is listening
/// (plain `dotnet build`) — in either case the resolved sink record has no consumer.
///
/// Nested CEs: the inner call captures *this* wrapper as its own `forwardTo` via
/// `sink.CurrentSink`, so notifications chain outer→inner correctly. Do not forward
/// through `sink` directly inside the wrapper or it would recurse.
let captureCustomOperationOverloads (sink: TcResultsSink) (queue: ResizeArray<DeferredCustomOpSink>) (action: unit -> 'T) : 'T =
    match sink.CurrentSink with
    | Some oldSink when queue.Count > 0 ->
        // The capture dict serves both as the "tracked ranges" set and the result buffer.
        // Each entry is `(expectedMethodName, capturedOrFallback, tpinst)`:
        //   * expectedMethodName comes from `Fallback.LogicalName` and is fixed for the entry —
        //     used in the wrapper to filter out unrelated MethodGroup notifications that share
        //     the synthetic range (e.g. an enclosing `For` call in a join/zip clause).
        //   * the second/third positions start as the fallback and are overwritten by the
        //     wrapper when the resolved overload's MethodGroup notification arrives.
        // Drain decides whether to call `Replacing` by comparing the captured method against
        // the fallback with `MethInfo.MethInfosUseIdenticalDefinitions` (cheap def-equality,
        // not the deep `MethInfosEquivByNameAndSig`).
        let capturedResolutions =
            Dictionary<range, string * MethInfo * TyparInstantiation>(Range.comparer)

        for entry in queue do
            capturedResolutions[entry.SyntheticCallRange] <- (entry.Fallback.LogicalName, entry.Fallback, emptyTyparInst)

        let captureSink = makeCustomOpResolutionCapturingSink oldSink capturedResolutions

        let result =
            use _holder = WithNewTypecheckResultsSink(captureSink, sink)
            action ()

        for entry in queue do
            let _, resolved, tpinst = capturedResolutions[entry.SyntheticCallRange]

            if not (MethInfo.MethInfosUseIdenticalDefinitions resolved entry.Fallback) then
                let item = Item.CustomOperation(entry.OpName, entry.UsageText, Some resolved)

                CallNameResolutionSinkReplacing
                    sink
                    (entry.KeywordRange, entry.NameEnv, item, tpinst, ItemOccurrence.Use, entry.AccessRights)

        result
    | _ -> action ()
