// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CodeAnalysis.Declarations.DeclarationFinish

open System
open System.Collections.Generic
open System.Threading

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras

open FSharp.Compiler
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.EditorServices
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

/// A module or namespace being built from results at finish.
[<NoEquality; NoComparison>]
type private Level =
    {
        /// None for the file's scope
        Frame: ModuleFrame option
        mutable Contents: ModuleOrNamespaceType
        Checked: ResizeArray<CheckedDeclaration>
        mutable EnvAtEnd: TcEnv
    }

/// Builds the scope's contents, checked declarations and environment at its end from the results in order,
/// finishing each nested module after its declarations, as checking the file whole does.
let private assemble
    (cenv: TcFileState)
    (scopeContents: ModuleOrNamespaceType)
    (scopeEnv: TcEnv)
    (results: DeclarationResult[])
    (exits: TcResultsSinkImpl[])
    =
    let root =
        {
            Frame = None
            Contents = scopeContents
            Checked = ResizeArray()
            EnvAtEnd = scopeEnv
        }

    let levels = Stack<Level>()
    levels.Push root

    // What leaving a module reports goes to `exits`, by the row it comes before, as a full check reports it there
    let close (exit: TcResultsSinkImpl) =
        use _ = WithNewTypecheckResultsSink(exit, cenv.tcSink)
        let level = levels.Pop()
        let parent = levels.Peek()

        match level.Frame with
        | Some({ Entity = Some entity } as frame) ->
            let outerEnv =
                { frame.OuterEnv with
                    eModuleOrNamespaceTypeAccumulator = ref parent.Contents
                }

            let checkedDecl, _, envAtEnd =
                ExitNestedModuleDeclarations
                    cenv
                    frame.ScopeRange
                    outerEnv
                    frame.Range
                    entity
                    level.Contents
                    (List.ofSeq level.Checked)
                    level.EnvAtEnd

            parent.Contents <- outerEnv.eModuleOrNamespaceTypeAccumulator.Value
            parent.Checked.Add checkedDecl
            parent.EnvAtEnd <- envAtEnd
        | Some frame ->
            parent.Checked.Add(([], [], []))
            parent.EnvAtEnd <- frame.OuterEnv
        | None -> ()

    for i in 0 .. results.Length - 1 do
        let result = results[i]

        while levels.Count - 1 > result.Nesting.Depth do
            close exits[i]

        match result.Nesting with
        | ModuleHeader(_, frame) ->
            let kind =
                match frame.Entity with
                | Some entity -> entity.ModuleOrNamespaceType.ModuleOrNamespaceKind
                | None -> ModuleOrNamespaceKind.ModuleOrType

            levels.Push
                {
                    Frame = Some frame
                    Contents = Construct.NewEmptyModuleOrNamespaceType kind
                    Checked = ResizeArray()
                    EnvAtEnd = frame.OuterEnv
                }
        | InModules _ ->
            let level = levels.Peek()
            level.Contents <- DeclarationStep.mergeDelta level.Contents result.Delta result.DeltaCombines
            level.Checked.Add result.Checked
            level.EnvAtEnd <- result.EnvAtEnd

    while levels.Count > 1 do
        close exits[results.Length]

    root.Contents, List.ofSeq root.Checked, root.EnvAtEnd

/// What a kept result's scope ranges are for the current text. A declaration's scope reaches the end of its module
/// or file, and a declaration added since is outside the scope its check captured.
let private scopesNow
    (setup: TcResultsSinkImpl)
    (setupEnd: pos)
    (rows: (TcResultsSinkImpl * range * range)[])
    (parse: FSharpParseFileResults)
    (current: unit -> (range * range)[])
    =
    ScopesNow(setup, setupEnd, rows, DeclarationSyntax.fileEndOf parse, current)

/// The results of several sinks as one sink's, in order.
let private mergeSinks (g: TcGlobals) (scopes: ScopesNow) (sinks: TcResultsSinkImpl list) =
    let all (pick: TcResolutions -> ResizeArray<'T>) =
        ResizeArray(sinks |> Seq.collect (fun sink -> pick (sink.GetResolutions())))

    let resolutions =
        TcResolutions(
            ResizeArray(
                sinks
                |> Seq.collect (fun sink ->
                    sink.GetResolutions().CapturedEnvs
                    |> Seq.map (fun (m, nenv, ad) -> scopes.Of(sink, m), nenv, ad))
            ),
            all (fun r -> r.CapturedExpressionTypings),
            // What a shifted declaration copied from others, as it is now (DeclarationShift.currentItems)
            ResizeArray(
                all (fun r -> r.CapturedNameResolutions)
                |> Seq.map DeclarationShift.currentItems
            ),
            all (fun r -> r.CapturedMethodGroupResolutions),
            all (fun r -> r.CapturedRelatedSymbolUses)
        )

    let formatSpecifierLocations =
        sinks
        |> Seq.collect (fun sink -> sink.GetFormatSpecifierLocations())
        |> Array.ofSeq

    let symbolUses =
        TcSymbolUses(g, resolutions.CapturedNameResolutions, resolutions.CapturedRelatedSymbolUses, formatSpecifierLocations)

    let openDeclarations =
        sinks
        |> Seq.collect (fun sink -> sink.GetOpenDeclarations())
        |> Seq.distinctBy (fun x -> x.Range, x.AppliedScope, x.IsOwnNamespace)
        |> Array.ofSeq

    resolutions, symbolUses, openDeclarations

/// Only what GetSemanticClassification reads of several sinks, in order: shifted name resolutions, related symbol
/// uses, and format specifier locations.
let private mergeForClassification (sinks: TcResultsSinkImpl list) =
    let all (pick: TcResolutions -> ResizeArray<'T>) =
        ResizeArray(sinks |> Seq.collect (fun sink -> pick (sink.GetResolutions())))

    let resolutions =
        TcResolutions(
            ResizeArray 0,
            ResizeArray 0,
            ResizeArray(
                all (fun r -> r.CapturedNameResolutions)
                |> Seq.map DeclarationShift.currentItems
            ),
            ResizeArray 0,
            all (fun r -> r.CapturedRelatedSymbolUses)
        )

    let formatSpecifierLocations =
        sinks
        |> Seq.collect (fun sink -> sink.GetFormatSpecifierLocations())
        |> Array.ofSeq

    resolutions, formatSpecifierLocations

/// Checks the file's implementation against its signature file as FinishFile's conformance check does, recording
/// how to undo what it writes into the implementation's items in a new log, which it returns. The diagnostics are
/// discarded: FinishFile reported them. For the readers of a finish that need the implementation as conformed:
/// FinishFile undoes its own conformance check before it takes the final fingerprints.
let private conformFile
    (state: CompilerState)
    (ccu: CcuThunk)
    (qualNameOfFile: QualifiedNameOfFile)
    (rootSig: ModuleOrNamespaceType)
    (implTy: ModuleOrNamespaceType)
    =
    let g = state.tcGlobals
    let amap = state.tcImports.GetImportMap()
    let infoReader = FSharp.Compiler.InfoReader.InfoReader(g, amap)
    let denv = DisplayEnv.Empty g
    let log = ResizeArray()
    let outerLog = MutationLog.Current
    let outerRecording = SignatureConformance.UndoableConformance.Recording
    let outerKeep = SignatureConformance.SignatureRanges.Keep
    MutationLog.Current <- log
    SignatureConformance.UndoableConformance.Recording <- true
    SignatureConformance.SignatureRanges.Keep <- true

    try
        try
            use _ = new CompilationGlobalsScope(DiscardErrorsLogger, BuildPhase.TypeCheck)

            let implSpec =
                wrapModuleOrNamespaceType qualNameOfFile.Id (compPathOfCcu ccu) implTy

            if SignatureConformance.CheckNamesOfModuleOrNamespace denv infoReader (mkLocalTyconRef implSpec) rootSig then
                let remapInfo, _ =
                    ComputeRemappingFromInferredSignatureToExplicitSignature g implTy rootSig

                let aenv =
                    { TypeEquivEnv.EmptyWithNullChecks g with
                        EquivTycons = TyconRefMap.OfList remapInfo.RepackagedEntities
                    }

                SignatureConformance.Checker(g, amap, denv, remapInfo, true).CheckSignature
                    aenv
                    infoReader
                    (mkLocalModuleRef implSpec)
                    rootSig
                |> ignore
        with RecoverableException _ ->
            ()

        log
    finally
        MutationLog.Current <- outerLog
        SignatureConformance.UndoableConformance.Recording <- outerRecording
        SignatureConformance.SignatureRanges.Keep <- outerKeep

/// Marks referenced exactly the module values of the results that the results' checks referenced.
/// A check marks the values it uses, and a later check can mark an earlier result's value, or a value of an earlier
/// version of a declaration that the current result no longer holds: so every flag is cleared, then each recorded
/// reference is found by its declaration's key, its module's path and its name in the current results. Both are
/// recorded in the active mutation log, which the next finish or check undoes.
///
/// The flag only means something for a module value a signature hides, which the unused-value warning (FS1182)
/// reads: references from later files are not replayed, so after a finish a public value used only by later files
/// reads as unreferenced, as FSharpMemberOrFunctionOrValue shows it. No diagnostic depends on that.
let private replayReferences (fileName: string) (results: DeclarationResult[]) =
    let byKey = Dictionary<DeclarationKey, DeclarationResult>(HashIdentity.Structural)

    for result in results do
        byKey[result.Key] <- result

    for result in results do
        for v in DeclarationStep.moduleValuesOf result.Delta do
            v.SetHasBeenReferencedTo false

    for result in results do
        for declaration, path, name in result.References do
            match declaration.Key with
            | Some key when String.Equals(declaration.File, fileName, StringComparison.OrdinalIgnoreCase) ->
                match byKey.TryGetValue key with
                | true, target ->
                    for v in DeclarationStep.moduleValuesOf target.Delta do
                        if v.LogicalName = name && DeclarationStep.modulePathOf v = path then
                            v.SetHasBeenReferencedTo true
                | _ -> ()
            | _ -> ()

/// A sink for what leaving modules reports before each of `results`, and last for after them all. One shared, empty,
/// where no module ends: a finish makes these for every declaration.
let private exitSinks (g: TcGlobals) (results: DeclarationResult[]) =
    let none = TcResultsSinkImpl g

    let frames (result: DeclarationResult) =
        match result.Nesting with
        | ModuleHeader _ -> result.Nesting.Depth + 1
        | InModules _ -> result.Nesting.Depth

    Array.init (results.Length + 1) (fun i ->
        let left = if i = 0 then 0 else frames results[i - 1]
        let entering = if i = results.Length then 0 else results[i].Nesting.Depth
        if entering < left then TcResultsSinkImpl g else none)

/// The sinks of a file's check as a full check reports to them: leaving a module, then the next declaration.
let private inOrder (setup: TcResultsSinkImpl) (results: DeclarationResult[]) (exits: TcResultsSinkImpl[]) =
    [
        yield setup

        for i in 0 .. results.Length - 1 do
            yield exits[i]
            yield results[i].Sink

        yield exits[results.Length]
    ]

/// The environment after `results`, folded from BeginFile's with ExtendEnv, and the environment after each. The
/// results at the start of `known` keep theirs: a shifted result has its check's ShareKey and holds what it holds. A
/// file's end is inside its last module, as a full check's is; a declaration checked whole has no replay for what it
/// leaves inside it, so the environment its check left stands. Under Sync, with no finish in effect to undo.
let private assembleEnvAtEnd
    (start: FileStart)
    (results: DeclarationResult[])
    (known: (obj * DeclarationEnvironment)[])
    (exits: TcResultsSinkImpl[] voption)
    =
    let envs = Array.zeroCreate results.Length
    let mutable env = start.EnvAtStart
    let mutable reusing = true

    for i in 0 .. results.Length - 1 do
        let result = results[i]

        if
            reusing
            && i < known.Length
            && obj.ReferenceEquals(fst known[i], result.ShareKey)
        then
            env <- snd known[i]
        else
            reusing <- false

            match exits with
            | ValueSome exits -> env <- DeclarationStep.exitModulesInto exits[i] start.Context.State start env result.Declaration.Depth
            | ValueNone -> ()

            env <- DeclarationStep.extendEnvLocked env result

        envs[i] <- result.ShareKey, env

    match exits with
    | ValueSome exits ->
        DeclarationStep.exitModulesInto exits[results.Length] start.Context.State start env 0
        |> ignore
    | ValueNone -> ()

    let atEnd =
        match Array.tryLast results with
        | Some last ->
            match last.Declaration.Syntax with
            | SynModuleDecl.NamespaceFragment _ -> last.EnvAtEnd
            | SynModuleDecl.NestedModule _ when not last.Declaration.IsModuleHeader -> last.EnvAtEnd
            | _ -> env.Env
        | None -> env.Env

    atEnd, envs

let private scopesNowOf (start: FileStart) (results: DeclarationResult[]) (parse: FSharpParseFileResults) : ScopesNow =
    scopesNow
        start.Sink
        (DeclarationSyntax.fileEndOf start.Parse)
        (results |> Array.map (fun r -> r.Sink, r.Declaration.Range, r.CapturedScope))
        parse
        (fun () -> DeclarationSyntax.split parse |> Array.map (fun d -> d.Range, d.ScopeRange))

let answeringDeclarations
    (start: FileStart)
    (results: DeclarationResult[])
    (parse: FSharpParseFileResults)
    (line: int)
    (column: int)
    : struct (int[] * int * bool) =
    let at = FSharp.Compiler.Text.Position.mkPos line column
    let setup = -1
    let exitBase = results.Length + 1
    let scopes = scopesNowOf start results parse

    let exits =
        match start.LastShares with
        | :? FinishShares as last when last.Exits.Length = results.Length + 1 -> last.Exits
        | _ -> exitSinks start.Context.State.tcGlobals results

    let captured =
        [|
            let of' (i: int) (sink: TcResultsSinkImpl) =
                [|
                    for (m, nenv, _) in sink.GetResolutions().CapturedEnvs -> i, scopes.Of(sink, m), nenv
                |]

            yield! of' setup start.Sink

            for i in 0 .. results.Length - 1 do
                yield! of' (exitBase + i) exits[i]
                yield! of' i results[i].Sink

            yield! of' (exitBase + results.Length) exits[results.Length]
        |]

    let mutable around = ValueNone

    for i, m, _ in captured do
        if rangeContainsPos m at then
            match around with
            | ValueSome(_, best) when not (rangeContainsRange best m) -> ()
            | _ -> around <- ValueSome(i, m)

    let mutable before = ValueNone

    for i, m, _ in captured do
        if rangeBeforePos m at && not (FSharp.Compiler.Text.Position.posEq m.End at) then
            let contained =
                match around with
                | ValueSome(_, best) -> rangeContainsRange best m
                | ValueNone -> true

            if contained then
                match before with
                | ValueSome(_, right: range) when
                    not (
                        FSharp.Compiler.Text.Position.posGt m.End right.End
                        || (FSharp.Compiler.Text.Position.posEq m.End right.End
                            && FSharp.Compiler.Text.Position.posGt m.Start right.Start)
                    )
                    ->
                    ()
                | _ -> before <- ValueSome(i, m)

    let found =
        [
            match around with
            | ValueSome(i, _) -> yield i
            | ValueNone -> ()

            match before with
            | ValueSome(i, _) -> yield i
            | ValueNone -> ()
        ]

    let rows =
        found
        |> List.filter (fun i -> i >= 0 && i < exitBase)
        |> List.distinct
        |> Array.ofList

    let exit =
        found
        |> List.filter (fun i -> i >= exitBase)
        |> List.fold (fun acc i -> max acc (i - exitBase)) -1

    struct (rows, exit, found.IsEmpty)

let viewResults (start: FileStart) (results: DeclarationResult[]) (parse: FSharpParseFileResults) : FSharpCheckFileResults =
    if not (String.Equals(parse.FileName, start.Parse.FileName, StringComparison.OrdinalIgnoreCase)) then
        invalidArg (nameof parse) "The parse is of another file than the FileStart's."

    let state = start.Context.State
    let tcState = start.TcState

    lock start.Sync (fun () ->
        lock (start.Context.Checks.FileLock start.Parse.FileName) (fun () ->
            // As a full check's queries see them, after the file's defaults: these results are this view's alone
            let scratch () =
                let scratch, _, _ =
                    DeclarationProjectCheck.newCheckerState
                        state
                        start.TcState
                        start.RootSigOpt
                        start.File
                        (TcResultsSinkImpl state.tcGlobals)

                scratch

            for result in results do
                if result.IsAdHoc then
                    let contents, _, attribs = result.Checked
                    DeclarationStep.applyDefaults state.tcGlobals scratch (TMDefs contents, List.map snd attribs) result.EnvAfter.Env

            let modules =
                [
                    for result in results do
                        match result.Nesting with
                        | ModuleHeader(_, { Entity = Some entity }) -> entity
                        | _ -> ()
                ]

            // The whole fold: what the finish's leaving a module reported holds names as they were at the module's header
            let exits = exitSinks state.tcGlobals results

            let atEnd, _ =
                DeclarationStep.keepingModuleTypes modules (fun () -> assembleEnvAtEnd start results [||] (ValueSome exits))

            let resolutions, symbolUses, openDeclarations =
                mergeSinks state.tcGlobals (scopesNowOf start results parse) (inOrder start.Sink results exits)

            let scope =
                TypeCheckInfo(
                    state.tcConfig,
                    state.tcGlobals,
                    tcState.CcuSig,
                    Construct.NewEmptyModuleOrNamespaceType(ModuleOrNamespaceKind.Namespace true),
                    tcState.Ccu,
                    state.tcImports,
                    atEnd.AccessRights,
                    state.projectOptions.ProjectFileName,
                    parse.FileName,
                    Some state.projectOptions,
                    resolutions,
                    symbolUses,
                    atEnd.NameEnv,
                    None,
                    None,
                    openDeclarations
                )

            FSharpCheckFileResults(parse.FileName, [||], Some scope, parse.DependencyFiles, None, true)))

let private finish
    (start: FileStart)
    (results: DeclarationResult[])
    (parse: FSharpParseFileResults)
    (entered: int64)
    (allocatedAtEntry: int64)
    : FileResult =
    let state = start.Context.State
    let file = start.File
    let tcState = start.TcState
    let phaseTicks = Dictionary<string, int64>()
    let phaseAllocated = Dictionary<string, int64>()
    let mutable phaseStart = entered
    let mutable allocatedAtStart = allocatedAtEntry

    let endPhase name =
        let now = System.Diagnostics.Stopwatch.GetTimestamp()
        let allocated = DeclarationProjectCheck.allocatedBytesForCurrentThread ()
        phaseTicks[name] <- now - phaseStart
        phaseAllocated[name] <- allocated - allocatedAtStart
        phaseStart <- now
        allocatedAtStart <- allocated

    endPhase "wait"
    let options = DeclarationSyntax.warnOptionsOf parse
    let directives = DeclarationSyntax.lineDirectivesOf parse

    // The previous finish's, taken out until this one succeeds
    let last =
        match start.LastShares with
        | :? FinishShares as last -> Some last
        | _ -> None

    start.LastShares <- null

    let known =
        match last with
        | Some last when
            DeclarationSyntax.sameWarnOptions last.Options options
            && last.Directives = directives
            ->
            Some last.Shares
        | _ -> None

    let shares =
        results
        |> Array.map (fun result ->
            match known with
            | Some known when not result.DependsOnLaterDeclarations && not result.LeavesUnsolved ->
                match known.TryGetValue result.ShareKey with
                // A shifted result renders its diagnostics at its own lines; its fingerprint and errors are the same
                | true, share when share.Line = result.Declaration.Range.StartLine || share.Diagnostics.Length = 0 -> ValueSome share
                | _ -> ValueNone
            | _ -> ValueNone)

    // With the parse's #nowarn scopes, which an added or removed #nowarn changes without changing any declaration.
    // Rendered after the tail, as a full check renders them, so their types show what the defaults solved.
    let startDiagnostics =
        DeclarationProjectCheck.refiltered options start.RawDiagnostics

    let refilteredResults =
        Array.map2
            (fun (result: DeclarationResult) share ->
                match share with
                | ValueSome _ -> ValueNone
                | ValueNone -> ValueSome(DeclarationProjectCheck.refiltered options result.RawDiagnostics))
            results
            shares

    let errorCountOf i =
        match shares[i], refilteredResults[i] with
        | ValueSome share, _ -> share.ErrorCount
        | _, ValueSome capturing -> capturing.ErrorCount
        | ValueNone, ValueNone -> 0

    let earlierErrors =
        startDiagnostics.ErrorCount + Array.sum (Array.init results.Length errorCountOf)

    endPhase "refilter"

    let capturing, logger = DeclarationProjectCheck.newLoggersWith options
    use _ = new CompilationGlobalsScope(logger, BuildPhase.TypeCheck)
    use _ = FSharp.Compiler.Cancellable.UsingToken CancellationToken.None

    let sink =
        TcResultsSinkImpl(state.tcGlobals, ?sourceText = DeclarationSyntax.sourceOf parse)

    let cenv, envFile, moduleTyAcc =
        DeclarationProjectCheck.newCheckerState state tcState start.RootSigOpt file sink

    start.Leftovers.Apply cenv

    for result in results do
        result.Leftovers.Apply cenv

    endPhase "leftovers"

    let exits = exitSinks state.tcGlobals results

    // Before the assembly below, which gives the modules their types: replaying leaves modules too
    let envAtEndNow, envs =
        assembleEnvAtEnd
            start
            results
            (match last with
             | Some last -> last.Envs
             | None -> [||])
            ValueNone

    endPhase "envAtEnd"

    let scopeContents, compiledDefs, envAtEnd =
        assemble cenv start.ScopeContents start.Scope.Env results exits

    endPhase "assemble"

    let checkForErrors () =
        parse.ParseHadErrors || earlierErrors > 0 || logger.ErrorCount > 0

    replayReferences start.Parse.FileName results
    endPhase "replay"

    // The conformance check records what it writes into the implementation's items in a log of its own, undone
    // once the diagnostics are rendered: no declaration of the file is checked against the conformed items, as
    // none is in a full check, so the final fingerprints and every later check of the file see them unconformed
    let conformance = ResizeArray()
    let outerConformance = SignatureConformance.UndoableConformance.Log
    SignatureConformance.UndoableConformance.Log <- conformance

    // Recovered from as CheckOneInput recovers in a full check, such as a default that fails where the tail does
    // not catch it (SolveInternalUnknowns): the file then adds nothing to the files after it.
    let tail =
        try
            DeclarationProjectCheck.leavingSignatureRanges start.Context.Checks parse start.RootSigOpt (fun () ->
                try
                    let fileContents =
                        ExitImplFileDeclarations
                            cenv
                            envFile
                            tcState.ImplicitOpenDeclarations
                            start.Scope
                            scopeContents
                            compiledDefs
                            envAtEnd

                    Some(CheckImplFileTail cenv checkForErrors tcState.TcEnvFromImpls start.RootSigOpt file moduleTyAcc fileContents)
                with RecoverableException exn ->
                    errorRecovery exn range0
                    None)
        finally
            SignatureConformance.UndoableConformance.Log <- outerConformance

    endPhase "tail"
    let qualNameOfFile = file.QualifiedName

    // The finish can solve more of each declaration's types and traits; this is the last finish's view
    for result, share in Array.zip results shares do
        if share.IsNone then
            let contents, _, _ = result.Checked
            result.SetFinishTypedUses(fst (DeclarationStep.typedUsesOf result.EnvAfter.Definitions result.Ref result.Delta contents None))

    endPhase "typedUses"

    let rendered =
        Array.init results.Length (fun i ->
            match shares[i], refilteredResults[i] with
            | ValueSome share, _ -> share.Diagnostics
            | _, ValueSome capturing -> DeclarationProjectCheck.diagnosticsWith state options parse.FileName capturing
            | ValueNone, ValueNone -> [||])

    let diagnostics =
        Array.concat
            [
                yield parse.Diagnostics
                yield DeclarationProjectCheck.diagnosticsWith state options parse.FileName startDiagnostics
                yield! rendered
                yield DeclarationProjectCheck.diagnosticsWith state options parse.FileName capturing
            ]
        |> DeclarationProjectCheck.projectDiagnostics start.Context

    endPhase "render"

    // What the render needed of the conformed items is taken
    MutationLog.Undo conformance

    // While the finish's solutions are in effect: the next finish or declaration check undoes them
    let finalPrints =
        Array.init results.Length (fun i ->
            match shares[i] with
            | ValueSome share -> share.Fingerprint
            | ValueNone ->
                let result = results[i]

                DeclarationStep.finishedFingerprint
                    result.AtCheck
                    (DeclarationStep.fingerprintOf result.Delta result.Checked result.SlotsAtCheck))

    let fingerprints = Dictionary<DeclarationKey, string>(HashIdentity.Structural)

    for result, print in Array.zip results finalPrints do
        fingerprints[result.Key] <- print

    endPhase "fingerprints"
    let keys = results |> Array.map _.Key

    let signature, ownSigForFile, tcEnvAtEnd, implFileOpt =
        match tail with
        | Some(topAttrs, implFile, _, createsGeneratedProvidedTypes, ownSigForFile) ->
            let fingerprint =
                DeclarationProjectCheck.implFingerprint start.RootSigOpt implFile.Signature topAttrs

            let describable (print: string) = not (print.StartsWith "?")

            let unchanged =
                match
                    last
                    |> Option.bind (fun last -> last.Signature |> Option.map (fun s -> last, s))
                with
                | Some(last, previous) when
                    not createsGeneratedProvidedTypes
                    && describable fingerprint
                    && String.Equals(previous.Fingerprint, fingerprint, StringComparison.Ordinal)
                    && last.Keys = keys
                    && Array.forall2
                        (fun (a: string) b -> describable a && String.Equals(a, b, StringComparison.Ordinal))
                        last.Fingerprints
                        finalPrints
                    ->
                    Some previous
                | _ -> None

            let signature =
                match unchanged with
                | Some previous -> previous
                | None ->
                    // Later files see a copy of the file's contents, or the signature file's declarations, which it maps already
                    let definitions =
                        match start.RootSigOpt with
                        | Some _ -> Map.empty
                        | None ->
                            let own =
                                (Map.empty, results)
                                ||> Array.fold (fun acc result -> DeclarationProjectCheck.addDefinitions acc result.Ref result.Delta)

                            DeclarationProjectCheck.copiedDefinitions own moduleTyAcc.Value implFile.Signature Map.empty

                    DeclarationProjectCheck.implFileSignature
                        state
                        parse.FileName
                        qualNameOfFile
                        implFile.Signature
                        createsGeneratedProvidedTypes
                        start.ModuleNamesDict
                        definitions
                        (Lazy<string>.CreateFromValue fingerprint)

            signature, ownSigForFile, envAtEndNow, Some implFile
        | None ->
            DeclarationProjectCheck.failedFileSignature parse.FileName start.ModuleNamesDict,
            Construct.NewEmptyModuleOrNamespaceType(ModuleOrNamespaceKind.Namespace true),
            tcState.TcEnvFromSignatures,
            None

    endPhase "signature"

    // The checker state the file's check results show it in, taken while the finish's solutions are in effect: the
    // signature it adds shares the declarations' inference variables
    let ccuSigForFile =
        match implFileOpt with
        | Some implFile ->
            AddCheckResultsToTcState
                (state.tcGlobals,
                 state.tcImports.GetImportMap(),
                 start.RootSigOpt.IsSome,
                 None,
                 TcResultsSink.NoSink,
                 tcState.TcEnvFromImpls,
                 qualNameOfFile,
                 implFile.Signature)
                tcState
            |> fst
        | None -> tcState.CcuSig

    // The rest is built on first read from what the finish leaves: the sinks, the environment at the file's end and
    // its typed tree, none of which a later finish or check replaces. Under the FileStart's lock, then the file's
    // check lock, as the finish itself, so it never overlaps a later finish's undoing of this one.
    // Not the FileStart itself: a demoted file keeps only its FileResult
    let sync = start.Sync
    let checks = start.Context.Checks
    let finishUndo = start.FinishUndo

    // The results show the implementation as conformed to its signature file (argument names, attributes, xml
    // docs from the signature), which the finish undid: it is applied again under the locks when they are built,
    // and the next check of the file undoes it with the rest of the latest finish.
    let reconform =
        match start.RootSigOpt, tail with
        | Some rootSig, Some(_, _, _, _, implTy) ->
            let ccu = tcState.Ccu
            Some(fun () -> conformFile state ccu qualNameOfFile rootSig implTy)
        | _ -> None

    let fileName = parse.FileName
    let dependencyFiles = parse.DependencyFiles

    let sinks = inOrder start.Sink results exits @ [ sink ]
    let scopes = scopesNowOf start results parse

    let checkResults =
        Lazy<FSharpCheckFileResults>(
            (fun () ->
                lock sync (fun () ->
                    lock (checks.FileLock fileName) (fun () ->
                        match reconform with
                        | Some conform ->
                            let log = conform ()

                            match finishUndo.Log with
                            | null -> finishUndo.Log <- log
                            | later -> later.AddRange log
                        | _ -> ()

                        let resolutions, symbolUses, openDeclarations =
                            mergeSinks state.tcGlobals scopes sinks

                        DeclarationProjectCheck.compiledNamesOf state resolutions implFileOpt

                        let scope =
                            TypeCheckInfo(
                                state.tcConfig,
                                state.tcGlobals,
                                ccuSigForFile,
                                ownSigForFile,
                                tcState.Ccu,
                                state.tcImports,
                                tcEnvAtEnd.AccessRights,
                                state.projectOptions.ProjectFileName,
                                fileName,
                                Some state.projectOptions,
                                resolutions,
                                symbolUses,
                                tcEnvAtEnd.NameEnv,
                                None,
                                implFileOpt,
                                openDeclarations
                            )

                        FSharpCheckFileResults(fileName, diagnostics, Some scope, dependencyFiles, None, true)))),
            LazyThreadSafetyMode.ExecutionAndPublication
        )

    let kept = Dictionary<obj, FinishShare>(HashIdentity.Reference)

    // A fingerprint that is not describable counts as changed every time, so it is taken again
    for i in 0 .. results.Length - 1 do
        if
            not results[i].DependsOnLaterDeclarations
            && not results[i].LeavesUnsolved
            && not (finalPrints[i].StartsWith "?")
        then
            kept[results[i].ShareKey] <-
                {
                    Fingerprint = finalPrints[i]
                    Diagnostics = rendered[i]
                    ErrorCount = errorCountOf i
                    Line = results[i].Declaration.Range.StartLine
                }

    start.LastShares <-
        FinishShares(options, directives, kept, keys, finalPrints, (if tail.IsSome then Some signature else None), envs, exits)

    let reused = shares |> Array.sumBy (fun share -> if share.IsSome then 1 else 0)
    endPhase "sinks"

    let topAttrs =
        match tail with
        | Some(topAttrs, _, _, _, _) -> topAttrs
        | None -> EmptyTopAttrs

    // m2: building the results takes the FileStart's lock and then the file's check lock, never the other way round
    let beforeRead () =
        if Monitor.IsEntered(checks.FileLock fileName) && not (Monitor.IsEntered sync) then
            invalidOp $"%s{fileName}: read while holding the file's check lock"

    let classify (range: range option) =
        lock sync (fun () ->
            lock (checks.FileLock fileName) (fun () ->
                let resolutions, formatSpecifiers = mergeForClassification sinks
                resolutions.GetSemanticClassification(state.tcGlobals, state.tcImports.GetImportMap(), formatSpecifiers, range)))

    FileResult(
        diagnostics,
        fingerprints,
        signature,
        checkResults,
        phaseTicks,
        phaseAllocated,
        reused,
        topAttrs,
        beforeRead,
        ValueNone,
        ValueSome classify
    )

let finishFile (start: FileStart) (results: DeclarationResult[]) (parse: FSharpParseFileResults) : FileResult =
    if not (String.Equals(parse.FileName, start.Parse.FileName, StringComparison.OrdinalIgnoreCase)) then
        invalidArg (nameof parse) "The parse is of another file than the FileStart's."

    let entered = System.Diagnostics.Stopwatch.GetTimestamp()
    let allocatedAtEntry = DeclarationProjectCheck.allocatedBytesForCurrentThread ()

    lock start.Sync (fun () ->
        start.UndoLastFinish()
        let log = ResizeArray()
        let outer = MutationLog.Current
        MutationLog.Current <- log

        try
            // As CheckDeclaration: the finish and its fingerprints give the file's compiler-generated names. Taken
            // inside the FileStart's lock, never the other way round (CheckDeclaration takes it after undoing).
            lock (start.Context.Checks.FileLock start.Parse.FileName) (fun () -> finish start results parse entered allocatedAtEntry)
        finally
            MutationLog.Current <- outer
            start.SetLastFinish log)
