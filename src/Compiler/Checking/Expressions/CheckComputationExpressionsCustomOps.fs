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
open FSharp.Compiler.Import
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

/// Information about a `[<CustomOperation>]` keyword usage whose `Item.CustomOperation`
/// sink record needs to be upgraded once normal overload resolution picks an overload.
[<NoComparison; NoEquality>]
type DeferredCustomOpSink =
    {
        KeywordRange: range
        OpName: string
        UsageText: unit -> string option
        SyntheticCallRange: range
        Candidates: MethInfo list
        Fallback: MethInfo
        NameEnv: NameResolutionEnv
        AccessRights: AccessorDomain
        mutable Resolved: (MethInfo * TyparInstantiation) option
    }

/// Sink wrapper that forwards every notification to the supplied `forwardTo` and additionally
/// records the singleton `Item.MethodGroup` resolution that lands at one of the tracked
/// synthetic call ranges. Last-write-wins, validated by `MethInfosEquivByNameAndSig` against
/// the deferred entry's candidates so unrelated notifications at the same range are ignored.
type private CustomOpResolutionCapturingSink
    (
        g: TcGlobals,
        amap: ImportMap,
        forwardTo: ITypecheckResultsSink option,
        deferredSinksBySyntheticRange: Dictionary<range, DeferredCustomOpSink list>
    ) =

    let matchesCandidate (mi: MethInfo) (entry: DeferredCustomOpSink) =
        entry.Candidates
        |> List.exists (fun c -> MethInfosEquivByNameAndSig EraseAll true g amap entry.KeywordRange c mi)

    let tryCapture (m: range) (item: Item) (tpinst: TyparInstantiation) =
        match item with
        | Item.MethodGroup(_, [ mi ], _) ->
            match deferredSinksBySyntheticRange.TryGetValue m with
            | true, entries ->
                for entry in entries do
                    if matchesCandidate mi entry then
                        entry.Resolved <- Some(mi, tpinst)
            | false, _ -> ()
        | _ -> ()

    interface ITypecheckResultsSink with
        member _.NotifyEnvWithScope(m, nenv, ad) =
            forwardTo |> Option.iter (fun s -> s.NotifyEnvWithScope(m, nenv, ad))

        member _.NotifyExprHasType(ty, nenv, ad, m) =
            forwardTo |> Option.iter (fun s -> s.NotifyExprHasType(ty, nenv, ad, m))

        member _.NotifyExprHasTypeSynthetic(ty, nenv, ad, m) =
            forwardTo
            |> Option.iter (fun s -> s.NotifyExprHasTypeSynthetic(ty, nenv, ad, m))

        member _.NotifyNameResolution(endPos, item, tpinst, occurrenceType, nenv, ad, m, replace) =
            tryCapture m item tpinst

            forwardTo
            |> Option.iter (fun s -> s.NotifyNameResolution(endPos, item, tpinst, occurrenceType, nenv, ad, m, replace))

        member _.NotifyMethodGroupNameResolution(endPos, item, itemMethodGroup, tpinst, occurrenceType, nenv, ad, m, replace) =
            tryCapture m item tpinst

            forwardTo
            |> Option.iter (fun s ->
                s.NotifyMethodGroupNameResolution(endPos, item, itemMethodGroup, tpinst, occurrenceType, nenv, ad, m, replace))

        member _.NotifyFormatSpecifierLocation(m, numArgs) =
            forwardTo |> Option.iter (fun s -> s.NotifyFormatSpecifierLocation(m, numArgs))

        member _.NotifyRelatedSymbolUse(m, item, kind) =
            forwardTo |> Option.iter (fun s -> s.NotifyRelatedSymbolUse(m, item, kind))

        member _.NotifyOpenDeclaration openDeclaration =
            forwardTo |> Option.iter (fun s -> s.NotifyOpenDeclaration openDeclaration)

        member _.CurrentSourceText = forwardTo |> Option.bind (fun s -> s.CurrentSourceText)

        member _.FormatStringCheckContext =
            forwardTo |> Option.bind (fun s -> s.FormatStringCheckContext)

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
    candidates
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
            Candidates = candidates
            Fallback = fallback
            NameEnv = nenv
            AccessRights = ad
            Resolved = None
        }

/// Run `action` (typically the `TcExpr` of the desugared CE lambda) with a sink wrapper
/// installed that captures the resolved `MethInfo` for each enqueued custom-operation
/// keyword. After `action` returns, replace each early sink record whose captured overload
/// is signature-different from the fallback with one carrying the resolved overload.
///
/// Short-circuits the wrapping and draining entirely when no custom operations were
/// enqueued (most async/task/seq/option/list CEs) or when no IDE sink is listening
/// (plain `dotnet build`) — in either case the resolved sink record has no consumer.
///
/// Nested CEs: the inner call captures *this* wrapper as its own `forwardTo` via
/// `sink.CurrentSink`, so notifications chain outer→inner correctly. Do not forward
/// through `sink` directly inside the wrapper or it would recurse.
let captureCustomOperationOverloads
    (g: TcGlobals)
    (amap: ImportMap)
    (sink: TcResultsSink)
    (queue: ResizeArray<DeferredCustomOpSink>)
    (action: unit -> 'T)
    : 'T =
    if queue.Count = 0 || sink.CurrentSink.IsNone then
        action ()
    else
        let oldSinkOpt = sink.CurrentSink

        let deferredSinksBySyntheticRange =
            let d = Dictionary<range, DeferredCustomOpSink list>(Range.comparer)

            for entry in queue do
                let existing =
                    match d.TryGetValue entry.SyntheticCallRange with
                    | true, xs -> xs
                    | false, _ -> []

                d[entry.SyntheticCallRange] <- entry :: existing

            d

        let captureSink =
            CustomOpResolutionCapturingSink(g, amap, oldSinkOpt, deferredSinksBySyntheticRange) :> ITypecheckResultsSink

        let result =
            use _holder = WithNewTypecheckResultsSink(captureSink, sink)
            action ()

        let isDifferentOverload (entry: DeferredCustomOpSink) (resolved: MethInfo) =
            not (MethInfosEquivByNameAndSig EraseAll true g amap entry.KeywordRange resolved entry.Fallback)

        for entry in queue do
            match entry.Resolved with
            | Some(resolved, tpinst) when isDifferentOverload entry resolved ->
                let item = Item.CustomOperation(entry.OpName, entry.UsageText, Some resolved)

                CallNameResolutionSinkReplacing
                    sink
                    (entry.KeywordRange, entry.NameEnv, item, tpinst, ItemOccurrence.Use, entry.AccessRights)
            | _ -> ()

        result
