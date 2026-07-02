// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Sinks the resolved overload's `MethInfo` at the keyword range of an overloaded
/// `[<CustomOperation>]` usage in a computation expression — fixes #11612 / #15206.
module internal FSharp.Compiler.CheckComputationExpressionsCustomOps

open System.Collections.Generic
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

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

let captureCustomOperationOverloads (sink: TcResultsSink) (queue: ResizeArray<DeferredCustomOpSink>) (action: unit -> 'T) : 'T =
    match sink.CurrentSink with
    | Some oldSink when queue.Count > 0 ->
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
