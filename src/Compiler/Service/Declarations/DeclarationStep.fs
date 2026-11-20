// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CodeAnalysis.Declarations.DeclarationStep

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
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

/// What a declaration adds to the module type it is published into, as the values and entities that are new.
let private contribution (before: ModuleOrNamespaceType) (after: ModuleOrNamespaceType) =
    let oldVals = HashSet<Val>(before.AllValsAndMembers, HashIdentity.Reference)
    let oldEntities = HashSet<Entity>(before.AllEntities, HashIdentity.Reference)

    let vals =
        after.AllValsAndMembers |> Seq.filter (oldVals.Contains >> not) |> List.ofSeq

    let entities =
        after.AllEntities |> Seq.filter (oldEntities.Contains >> not) |> List.ofSeq

    Construct.NewModuleOrNamespaceType after.ModuleOrNamespaceKind entities vals

/// Whether a declaration publishes into the file's contents by combining: a fragment in a namespace does, at its
/// end, and only reads accumulators of its own before that. A fragment with no namespace (namespace global, or the
/// module of a file) publishes into the file's accumulator directly, as declarations in a module do.
let combinesIntoFile (decl: SynModuleDecl) =
    match decl with
    | SynModuleDecl.NamespaceFragment(SynModuleOrNamespace(longId = longId; kind = kind)) ->
        // The enclosing namespace path, as EnterNamespaceFragment computes it
        let path =
            if kind.IsModule then
                List.truncate (max 0 (longId.Length - 1)) longId
            else
                longId

        not path.IsEmpty
    | _ -> false

/// Merges a declaration's delta onto what its module or namespace held before it, as publishing it did: a
/// namespace fragment combines its namespaces with the file's, and anything else adds its values and entities.
let mergeDelta (acc: ModuleOrNamespaceType) (delta: ModuleOrNamespaceType) (combines: bool) =
    if combines then
        CombineCcuContentFragments [ acc; delta ]
    else
        let acc =
            (acc, delta.AllEntities) ||> Seq.fold (fun acc entity -> acc.AddEntity entity)

        (acc, delta.AllValsAndMembers) ||> Seq.fold (fun acc v -> acc.AddVal v)

/// The environment after leaving nested modules until the environment is in only `depth` of them, as the first
/// declaration after a module is checked in. FinishFile finishes the modules again and reports what that finds,
/// so what this reports is dropped. It gives each module its contents so far as its type, as FinishFile does.
let exitModulesInto (exits: TcResultsSinkImpl) (state: CompilerState) (start: FileStart) (envBefore: DeclarationEnvironment) (depth: int) =
    if envBefore.Frames.Length <= depth then
        envBefore
    else
        use _ = new CompilationGlobalsScope(DiscardErrorsLogger, BuildPhase.TypeCheck)

        let cenv, _, _ =
            DeclarationProjectCheck.newCheckerState state start.TcState start.RootSigOpt start.File exits

        let rec exit (env: DeclarationEnvironment) =
            match env.Frames with
            | frame :: outer when env.Frames.Length > depth ->
                let outerEnv =
                    { frame.OuterEnv with
                        eModuleOrNamespaceTypeAccumulator = ref frame.OuterEnv.eModuleOrNamespaceTypeAccumulator.Value
                    }

                let envAfter =
                    match frame.Entity with
                    | Some entity ->
                        let moduleContents = env.Env.eModuleOrNamespaceTypeAccumulator.Value

                        let _, envAfter, _ =
                            ExitNestedModuleDeclarations
                                cenv
                                frame.ScopeRange
                                outerEnv
                                frame.Range
                                entity
                                moduleContents
                                (List.rev frame.Checked)
                                env.Env

                        envAfter
                    | None -> outerEnv

                exit (DeclarationEnvironment(envAfter, frame.OuterParent, frame.OuterTypeNames, outer, env.Definitions))
            | _ -> env

        exit envBefore

let private exitModules (state: CompilerState) (start: FileStart) (envBefore: DeclarationEnvironment) (depth: int) =
    exitModulesInto (TcResultsSinkImpl state.tcGlobals) state start envBefore depth

/// Runs `replay`, which may leave modules, and puts back the types it gave them: leaving a module gives it its contents
/// so far, which only a finish may publish.
let keepingModuleTypes (entities: Entity list) (replay: unit -> 'T) : 'T =
    let saved = entities |> List.map (fun e -> e, e.entity_modul_type)

    try
        replay ()
    finally
        for e, t in saved do
            e.entity_modul_type <- t

let private framesOf (env: DeclarationEnvironment) = env.Frames |> List.choose _.Entity

/// The bindings of inline values in a declaration's typed tree, in order.
let rec private inlineBodies (contents: ModuleOrNamespaceContents) =
    let ofBinding (TBind(v, body, _)) =
        if v.ShouldInline then [ v, body ] else []

    match contents with
    | TMDefs defs -> List.collect inlineBodies defs
    | TMDefLet(binding, _) -> ofBinding binding
    | TMDefRec(bindings = bindings) ->
        bindings
        |> List.collect (function
            | ModuleOrNamespaceBinding.Binding binding -> ofBinding binding
            | ModuleOrNamespaceBinding.Module(_, def) -> inlineBodies def)
    | TMDefOpens _
    | TMDefDo _ -> []

/// The slots each member of a contribution implements, and whether it is implemented, as its check left them.
///
/// Closing the inference scope completes them (FinalTypeDefinitionChecksAtEndOfInferenceScope): a slot the check
/// recorded by name and declaring type only gets its parameters and return type, and those of a generated
/// comparison member are taken from the interface's own metadata; a slot no member was matched to at the check is
/// recorded; an abstract member with a default is marked implemented. The declarations after this one are checked
/// before that, so they see these. A fingerprint taken at the finish describes these too: the finish computes the
/// completed slots from the member (its name, type, flags, which the fingerprint describes), the interfaces and base
/// type of its type (described with it) and the slots those declare, which come from referenced assemblies, earlier
/// files (whose signatures are compared whole) or declarations whose own fingerprints describe them. An augmentation
/// of the type later in the file can add members and interfaces, and it leads a segment with the type.
let private slotsAtCheck (contribution: ModuleOrNamespaceType) : IReadOnlyDictionary<Val, SlotSig list * bool> =
    let slots = Dictionary<Val, SlotSig list * bool>(HashIdentity.Reference)

    let rec visit (mty: ModuleOrNamespaceType) =
        for v in mty.AllValsAndMembers do
            match v.MemberInfo with
            | Some memberInfo -> slots[v] <- (memberInfo.ImplementedSlotSigs, memberInfo.IsImplemented)
            | None -> ()

        for entity in mty.AllEntities do
            if entity.IsModuleOrNamespace then
                visit entity.ModuleOrNamespaceType

    visit contribution
    slots :> IReadOnlyDictionary<_, _>

let fingerprintOf
    (contribution: ModuleOrNamespaceType)
    (checkedDecl: CheckedDeclaration)
    (slots: IReadOnlyDictionary<Val, SlotSig list * bool>)
    =
    let contents, _, _ = checkedDecl
    // Every member the fingerprint describes was captured; reading one that was not would describe the finish's slots
    let slotsOf (v: Val) (_: ValMemberInfo) =
        match slots.TryGetValue v with
        | true, atCheck -> Some atCheck
        | _ -> None

    match SignatureFingerprint.computeDeclaration contribution (List.collect inlineBodies contents) slotsOf with
    | Some fingerprint -> fingerprint
    // Not describable: never equal to another fingerprint, so it always counts as changed.
    | None -> "?" + Guid.NewGuid().ToString("N")

/// The fingerprint of a declaration, from the fingerprint of what the declarations after it see and that of what
/// FinishFile leaves (see whileFinished): it changes when either does. One that is not describable stands for both.
let finishedFingerprint (atCheck: string voption) (finished: string) =
    match atCheck with
    | ValueNone -> finished
    | ValueSome seen when seen.StartsWith "?" -> seen
    | ValueSome _ when finished.StartsWith "?" -> finished
    | ValueSome seen -> seen + "|finished|" + finished

/// What the finish does to a declaration's unsolved variables. What it reports, and an error it raises, are dropped.
let applyDefaults (g: TcGlobals) (scratch: unit -> TcFileState) (defaults: ModuleOrNamespaceContents * Attribs) (env: TcEnv) =
    let contents, attribs = defaults

    try
        use _ = new CompilationGlobalsScope(DiscardErrorsLogger, BuildPhase.TypeCheck)
        // A checker state of its own: what solving defers is not this check's to run
        let scratch = scratch ()
        ApplyDefaults scratch g env.DisplayEnv range0 contents attribs
        SolveInternalUnknowns g scratch env.DisplayEnv contents attribs
    with RecoverableException _ ->
        ()

/// Runs `f` while the declaration holds what FinishFile's defaults change in it, then undoes that, so the
/// declarations after it are checked as checking the whole file checks them: `defaults` (its contents and
/// attributes, for a declaration that left inference variables unsolved but depends on no later declaration) with
/// the file-end defaults solving them. No other declaration reaches them, as in the arguments of an attribute
/// (`typedefof&lt;Proxy&lt;_&gt;&gt;`), and FinishFile solves them the same way when its earlier defaults succeed. A fingerprint
/// taken in `f` describes what FinishFile leaves; the declarations after it see the variables unsolved, so the
/// cutoff for them also has a fingerprint taken outside `f` (finishedFingerprint keeps both). The diagnostics are
/// discarded: FinishFile reports them.
let private whileFinished
    (g: TcGlobals)
    (scratch: unit -> TcFileState)
    (defaults: ModuleOrNamespaceContents * Attribs)
    (env: TcEnv)
    (f: unit -> 'T)
    : 'T =
    let log = ResizeArray()
    let outerLog = MutationLog.Current
    MutationLog.Current <- log

    try
        applyDefaults g scratch defaults env
        f ()
    finally
        MutationLog.Undo log
        MutationLog.Current <- outerLog

/// Whether any of the type parameters can still be solved. A rigid one, such as the declared 'T of
/// `type Box&lt;'T&gt;(v: 'T)` that the environment after the type keeps, cannot: unifying it with another type, or
/// constraining it further, is an error, not a solution.
let private anySolvable (typars: Zset<Typar>) =
    typars |> Zset.exists (fun tp -> tp.Rigidity <> TyparRigidity.Rigid)

/// Whether what a declaration publishes, or the body of one of its inline values, holds an inference variable
/// nothing has solved. Such a variable is not always in the environment after it, as a value's is: a member's
/// (`member _.M() = 1L + 1`) is solved in place by a later `let t: int64 = T().M()`, and one only in an inline body
/// is solved by the finish's defaults, which the fingerprint describes.
let private leavesOpenTypes (contribution: ModuleOrNamespaceType) (checkedDecl: CheckedDeclaration) =
    let contents, _, _ = checkedDecl

    anySolvable (freeInModuleTy contribution).FreeTypars
    || contents
       |> List.collect inlineBodies
       |> List.exists (fun (_, body) -> anySolvable (freeInExpr CollectTyparsNoCaching body).FreeTyvars.FreeTypars)

/// The inference variables `leavesOpenTypes` and `ComputeUngeneralizableTypars` find, as they stand.
let private openTypars (env: TcEnv) (contribution: ModuleOrNamespaceType) (checkedDecl: CheckedDeclaration) =
    let contents, _, _ = checkedDecl
    let found = ResizeArray<Typar>()

    let add (typars: Zset<Typar>) =
        typars
        |> Zset.iter (fun tp ->
            if tp.Rigidity <> TyparRigidity.Rigid then
                found.Add tp)

    add (GeneralizationHelpers.ComputeUngeneralizableTypars env)
    add (freeInModuleTy contribution).FreeTypars

    for _, body in contents |> List.collect inlineBodies do
        add (freeInExpr CollectTyparsNoCaching body).FreeTyvars.FreeTypars

    found.ToArray() |> Array.distinctBy _.Stamp

/// Whether an inference variable a check left is open now: its solution, followed through, or a variable a
/// constraint links it to, is still to be solved.
let private isOpen (tp: Typar) =
    anySolvable (freeInType CollectTyparsNoCaching (mkTyparTy tp)).FreeTypars

/// Whether what a declaration publishes, or anything it checked (every body, not only inline ones), holds an
/// inference variable nothing has solved.
///
/// The variables are taken as the finish's defaults and SolveInternalUnknowns take theirs, with
/// FindUnsolved.UnsolvedTyparsOfModuleDef over the declaration's contents and attributes: every lambda parameter
/// and local value included. So whatever those passes could solve in this declaration makes it LeavesUnsolved.
let private leavesUnsolved
    (state: CompilerState)
    (denv: DisplayEnv)
    (contribution: ModuleOrNamespaceType)
    (checkedDecl: CheckedDeclaration)
    =
    let contents, _, attribs = checkedDecl

    anySolvable (freeInModuleTy contribution).FreeTypars
    || FindUnsolved.UnsolvedTyparsOfModuleDef state.tcGlobals (state.tcImports.GetImportMap()) denv (TMDefs contents) (List.map snd attribs)
       |> List.exists (fun tp -> tp.Rigidity <> TyparRigidity.Rigid && not tp.IsSolved)

/// The module values in a module type, including those inside its modules.
let rec moduleValuesOf (mty: ModuleOrNamespaceType) =
    seq {
        for v in mty.AllValsAndMembers do
            if v.IsModuleBinding then
                yield v

        for entity in mty.AllEntities do
            if entity.IsModuleOrNamespace then
                yield! moduleValuesOf entity.ModuleOrNamespaceType
    }

/// The compilation path and name of the module a module value is in: a declaration holding modules whole can hold
/// values of the same name in several of them. The names are the same in every check of the declaration.
let modulePathOf (v: Val) =
    match v.TryDeclaringEntity with
    | Parent entity ->
        let outer =
            match entity.CompilationPathOpt with
            | Some cpath -> cpath.MangledPath
            | None -> []

        outer @ [ entity.LogicalName ]
    | ParentNone -> []

/// Records the members `delta` gave types it does not define, which an augmentation adds to the type in place.
let private recordAugmentations (start: FileStart) (delta: ModuleOrNamespaceType) =
    let definesHere = HashSet<Stamp>(DeclarationProjectCheck.stampsOf delta)

    let rec visit (mty: ModuleOrNamespaceType) =
        for v in mty.AllValsAndMembers do
            if v.IsIntrinsicMember && v.MemberApparentEntity.IsLocalRef then
                let target = v.MemberApparentEntity.Deref

                if not (definesHere.Contains target.Stamp) then
                    start.Augmented.Add(target, mkLocalValRef v)

        for entity in mty.AllEntities do
            if entity.IsModuleOrNamespace then
                visit entity.ModuleOrNamespaceType

    visit delta

/// Runs `replay` with each augmented type holding only the members of the declarations in `definitions`, so a
/// replay of a declaration reads the type as that declaration's check did: the check of an earlier declaration
/// never saw what a later one added, and what it read decided what entered its environment (C#-style extension
/// members, `open type`). The members go back before the FileStart's lock is released.
let private asOfDeclaration (start: FileStart) (definitions: Map<Stamp, DeclarationRef>) (replay: unit -> 'T) : 'T =
    let hidden = ResizeArray()

    try
        start.Augmented.Iter(fun tycon added ->
            let later =
                HashSet<Stamp>(added |> Seq.map _.Stamp |> Seq.filter (definitions.ContainsKey >> not))

            if later.Count > 0 then
                let tcaug = tycon.TypeContents
                hidden.Add((tcaug, tcaug.tcaug_adhoc, tcaug.tcaug_adhoc_list))

                tcaug.tcaug_adhoc <-
                    tcaug.tcaug_adhoc
                    |> Map.map (fun _ members -> members |> List.filter (fun (m: ValRef) -> not (later.Contains m.Stamp)))
                    |> Map.filter (fun _ members -> not members.IsEmpty)

                tcaug.tcaug_adhoc_list <-
                    match tcaug.tcaug_adhoc_list with
                    | null -> null
                    | members -> ResizeArray(members |> Seq.filter (fun (_, m) -> not (later.Contains m.Stamp))))

        replay ()
    finally
        for tcaug, adhoc, adhocList in hidden do
            tcaug.tcaug_adhoc <- adhoc
            tcaug.tcaug_adhoc_list <- adhocList

/// The types a type is made of, which its equality, comparison and other inferred properties come from.
let private structureOf (tycon: Tycon) =
    [
        for field in tycon.AllFieldsArray -> field.FormalType
        if tycon.IsUnionTycon then
            for case in tycon.UnionCasesArray do
                for field in case.RecdFieldsArray -> field.FormalType
        yield! Option.toList tycon.TypeContents.tcaug_super
        yield! tycon.ImmediateInterfaceTypesOfFSharpTycon
        yield! Option.toList tycon.TypeAbbrev
    ]

/// The declarations other than `self` that define what the declaration's checked contents, the types of what it
/// publishes and what its sink resolved refer to, closed over the types each type of the project is made of.
let typedUsesOf
    (definitions: Map<Stamp, DeclarationRef>)
    (self: DeclarationRef)
    (delta: ModuleOrNamespaceType)
    (contents: ModuleOrNamespaceContents list)
    (sink: TcResultsSinkImpl option)
    =
    let refs = HashSet<DeclarationRef>(HashIdentity.Structural)
    let held = Dictionary<Stamp, DeclarationRef>()
    let seenTycons = HashSet<Stamp>()
    let madeOf = Stack<Tycon>()

    let addStamp stamp =
        match definitions.TryFind stamp with
        | Some declaration ->
            refs.Add declaration |> ignore
            held[stamp] <- declaration
            true
        | None -> false

    let addVal (v: Val) = addStamp v.Stamp |> ignore

    // Only a type of the project can change, so only those are opened up
    let addTycon (tycon: Tycon) =
        if seenTycons.Add tycon.Stamp && addStamp tycon.Stamp then
            madeOf.Push tycon

    let addTyvars (ftyvs: FreeTyvars) =
        Zset.iter addTycon ftyvs.FreeTycons
        Zset.iter addVal ftyvs.FreeTraitSolutions

    let addTcref (tcref: TyconRef) =
        if tcref.IsLocalRef then
            addTycon tcref.Deref

    // A type's free entities leave out the units of measure in it, which entity references compare by object as they
    // do any other's
    let seenTypars = HashSet<Stamp>()

    let rec addUnitsOfMeasure (unt: Measure) =
        match stripUnitEqns unt with
        | Measure.Const(tcref, _) -> addTcref tcref
        | Measure.Prod(a, b, _) ->
            addUnitsOfMeasure a
            addUnitsOfMeasure b
        | Measure.Inv a
        | Measure.RationalPower(a, _) -> addUnitsOfMeasure a
        | Measure.Var _
        | Measure.One _ -> ()

    let rec addUnits (ty: TType) =
        match stripTyparEqns ty with
        | TType_measure unt -> addUnitsOfMeasure unt
        | TType_app(_, tinst, _)
        | TType_ucase(_, tinst)
        | TType_tuple(_, tinst)
        | TType_anon(_, tinst) -> List.iter addUnits tinst
        | TType_fun(domainTy, rangeTy, _) ->
            addUnits domainTy
            addUnits rangeTy
        | TType_forall(tps, bodyTy) ->
            List.iter addUnitsOfTypar tps
            addUnits bodyTy
        | TType_var(tp, _) -> addUnitsOfTypar tp

    and addUnitsOfTypar (tp: Typar) =
        if seenTypars.Add tp.Stamp then
            for constr in tp.Constraints do
                match constr with
                | TyparConstraint.CoercesTo(ty, _)
                | TyparConstraint.DefaultsTo(_, ty, _)
                | TyparConstraint.IsEnum(ty, _) -> addUnits ty
                | TyparConstraint.IsDelegate(a, b, _) ->
                    addUnits a
                    addUnits b
                | TyparConstraint.SimpleChoice(tys, _) -> List.iter addUnits tys
                | TyparConstraint.MayResolveMember(TTrait(tys = tys; objAndArgTys = argTys; returnTyOpt = returnTy), _) ->
                    List.iter addUnits tys
                    List.iter addUnits argTys
                    Option.iter addUnits returnTy
                | _ -> ()

    let addType ty =
        addTyvars (freeInType CollectAll ty)
        addUnits ty

    let addVref (vref: ValRef) =
        if vref.IsLocalRef then
            addVal vref.Deref

    let addFreeVars (fvs: FreeVars) =
        Zset.iter addVal fvs.FreeLocals
        Zset.iter (fun (rfref: RecdFieldRef) -> addTcref rfref.TyconRef) fvs.FreeRecdFields
        Zset.iter (fun (ucref: UnionCaseRef) -> addTcref ucref.TyconRef) fvs.FreeUnionCases
        Zset.iter addTycon fvs.FreeLocalTyconReprs
        addTyvars fvs.FreeTyvars

    let addItem (item: Item) =
        match item with
        | Item.Value vref
        | Item.CustomBuilder(_, vref) ->
            addVref vref
            addType vref.Type
        | Item.UnionCase(info, _)
        | Item.UnionCaseField(info, _) -> addTcref info.TyconRef
        | Item.ActivePatternResult(apOverallTy = ty)
        | Item.DelegateCtor ty -> addType ty
        | Item.ActivePatternCase apref -> addVref apref.ActivePatternVal
        | Item.ExnCase tcref -> addTcref tcref
        | Item.RecdField info -> addTcref info.TyconRef
        | Item.Trait info -> addTyvars (accFreeInTrait CollectAll info emptyFreeTyvars)
        | Item.Types(_, tys) -> List.iter addType tys
        | Item.ModuleOrNamespaces modrefs -> List.iter addTcref modrefs
        | Item.Property(info = pinfos) ->
            for pinfo in pinfos do
                addType pinfo.ApparentEnclosingType
                Option.iter addVref pinfo.ArbitraryValRef
        | Item.MethodGroup(methods = minfos)
        | Item.CtorGroup(_, minfos) ->
            for minfo in minfos do
                addType minfo.ApparentEnclosingType
                Option.iter addVref minfo.ArbitraryValRef
        | Item.CustomOperation(_, _, Some minfo) ->
            addType minfo.ApparentEnclosingType
            Option.iter addVref minfo.ArbitraryValRef
        | Item.Event einfo ->
            addType einfo.ApparentEnclosingType
            Option.iter addVref einfo.ArbitraryValRef
        | Item.ILField finfo -> addType finfo.ApparentEnclosingType
        | Item.ImplicitOp(_, sln) ->
            sln.Value
            |> Option.iter (fun sln -> addTyvars (accFreeInTraitSln CollectAll sln emptyFreeTyvars))
        | _ -> ()

    for c in contents do
        addFreeVars (freeInModuleOrNamespace CollectAll c)

    // The types of what it publishes, and what its own types are made of
    let rec published (mty: ModuleOrNamespaceType) =
        for v in mty.AllValsAndMembers do
            addTyvars (freeInVal CollectAll v)
            addUnits v.Type

        for entity in mty.AllEntities do
            if entity.IsModuleOrNamespace then
                published entity.ModuleOrNamespaceType
            elif seenTycons.Add entity.Stamp then
                madeOf.Push entity

    published delta

    match sink with
    | Some sink ->
        let resolutions = sink.GetResolutions()

        for resolution in resolutions.CapturedNameResolutions do
            addItem resolution.Item

        for resolution in resolutions.CapturedMethodGroupResolutions do
            addItem resolution.Item

        for ty, _, _, _ in resolutions.CapturedExpressionTypings do
            addType ty
    | None -> ()

    while madeOf.Count > 0 do
        let tycon = madeOf.Pop()

        for ty in structureOf tycon do
            addType ty

        addTyvars (accFreeInTypars CollectAll tycon.Typars emptyFreeTyvars)
        List.iter addUnitsOfTypar tycon.Typars

    refs.Remove self |> ignore
    refs |> Array.ofSeq |> Array.sort, [| for kv in held -> kv.Key, kv.Value |]

/// Runs with the file's name when CheckDeclaration has undone its FileStart's finish, before it takes the file's
/// check lock. For tests.
let mutable afterUndoHook: string -> unit = ignore

let private checkDeclarationLocked
    (start: FileStart)
    (envBefore: DeclarationEnvironment)
    (decl: ParsedDeclaration)
    (ct: CancellationToken)
    : DeclarationResult =
    let state = start.Context.State
    let capturing, logger = DeclarationProjectCheck.newLoggers state
    use _ = new CompilationGlobalsScope(logger, BuildPhase.TypeCheck)
    // Cancellable.run hands ct to the cancellable computation; code outside it calls CheckAndThrow, which reads
    // the ambient token, and nothing else sets that one.
    use _ = FSharp.Compiler.Cancellable.UsingToken ct
    let sink = TcResultsSinkImpl(state.tcGlobals, ?sourceText = Some decl.Source)

    let cenv, _, _ =
        DeclarationProjectCheck.newCheckerState state start.TcState start.RootSigOpt start.File sink

    let referenced = ResizeArray<Val>()

    cenv.recordValueReference <-
        fun v ->
            if v.IsModuleBinding then
                referenced.Add v

    let envBefore = exitModules state start envBefore decl.Depth
    let contentsBefore = envBefore.Env.eModuleOrNamespaceTypeAccumulator.Value

    // Combining builds new namespace entities, so only from an empty start is what such a fragment adds the
    // whole of what it leaves; it reads nothing else from the file's accumulator. Anything else starts from the
    // earlier declarations' contents, which its duplicate checks read.
    let deltaCombines = combinesIntoFile decl.Syntax

    let contentsAtStart =
        if deltaCombines then
            Construct.NewEmptyModuleOrNamespaceType contentsBefore.ModuleOrNamespaceKind
        else
            contentsBefore

    let accumulator = ref contentsAtStart

    let env =
        { envBefore.Env with
            eModuleOrNamespaceTypeAccumulator = accumulator
        }

    // As checking the whole file: nothing in a scope or module whose setup failed is checked
    let checks =
        start.Scope.ChecksDeclarations
        && (match envBefore.Frames with
            | frame :: _ -> frame.Entity.IsSome
            | [] -> true)

    // Only a module reads them, for its suffix, and the whole check collects them once, in the environment where
    // the enclosing module's declarations start. Inside a nested module, that is what its header collected. At the
    // file's scope it is redone from the siblings' syntax (which HeaderInputs covers) in the scope's environment,
    // so a FileStart made before a sibling type was added still gives the suffix. A cenv of its own keeps the
    // attribute resolutions it reports out of this declaration's sink.
    let typeNames =
        match decl.Syntax with
        | SynModuleDecl.NestedModule _ when decl.Depth = 0 ->
            let scratch, _, _ =
                DeclarationProjectCheck.newCheckerState state start.TcState start.RootSigOpt start.File (TcResultsSinkImpl state.tcGlobals)

            TypeNamesOfDeclarations scratch start.Scope.Env decl.Siblings
        | _ -> envBefore.TypeNames

    let checkedDecl, envAfter, envAtEnd, delta, nesting =
        if decl.IsModuleHeader then
            let entered =
                if checks then
                    EnterNestedModuleDeclarations cenv envBefore.Parent typeNames env decl.Syntax
                else
                    None

            let frame =
                {
                    Entity = entered |> Option.map (fun (entity, _, _, _) -> entity)
                    ScopeRange = decl.ScopeRange
                    Range = decl.Syntax.Range
                    OuterEnv = env
                    OuterParent = envBefore.Parent
                    OuterTypeNames = envBefore.TypeNames
                    Checked = []
                }

            let envAfter =
                match entered with
                | Some(_, envForModule, parent, typeNames) ->
                    DeclarationEnvironment(envForModule, parent, typeNames, frame :: envBefore.Frames, envBefore.Definitions)
                | None ->
                    DeclarationEnvironment(env, envBefore.Parent, envBefore.TypeNames, frame :: envBefore.Frames, envBefore.Definitions)

            // What the header decides about the module (its name, kind, attributes, access), without the contents
            // the module's declarations give it, which FinishFile replaces
            let delta =
                match frame.Entity with
                | Some entity ->
                    let header =
                        { entity with
                            entity_modul_type =
                                MaybeLazy.Strict(Construct.NewEmptyModuleOrNamespaceType entity.ModuleOrNamespaceType.ModuleOrNamespaceKind)
                        }

                    Construct.NewModuleOrNamespaceType contentsBefore.ModuleOrNamespaceKind [ header ] []
                | None -> Construct.NewEmptyModuleOrNamespaceType contentsBefore.ModuleOrNamespaceKind

            ([], [], []), envAfter, envAfter.Env, delta, ModuleHeader(decl.Depth, frame)
        else
            let checkedDecl, envAfter, envAtEnd =
                if not checks then
                    ([], [], []), env, env
                else
                    TcModuleOrNamespaceElementNonMutRec cenv envBefore.Parent typeNames decl.ScopeRange env decl.Syntax
                    |> cenv.stackGuard.GuardCancellable
                    |> Cancellable.run ct
                    |> function
                        | ValueOrCancelled.Value result -> result
                        | ValueOrCancelled.Cancelled exn -> raise exn

            let frames =
                match envBefore.Frames with
                | frame :: outer ->
                    { frame with
                        Checked = checkedDecl :: frame.Checked
                    }
                    :: outer
                | [] -> []

            let delta = contribution contentsAtStart accumulator.Value

            checkedDecl,
            DeclarationEnvironment(envAfter, envBefore.Parent, envBefore.TypeNames, frames, envBefore.Definitions),
            envAtEnd,
            delta,
            InModules decl.Depth

    let leavesFreeTypars =
        anySolvable (GeneralizationHelpers.ComputeUngeneralizableTypars envAfter.Env)

    let self =
        {
            File = start.Parse.FileName
            Key = Some decl.Key
        }

    let definitions =
        DeclarationProjectCheck.addDefinitions envBefore.Definitions self delta

    recordAugmentations start delta

    let envAfter =
        DeclarationEnvironment(envAfter.Env, envAfter.Parent, envAfter.TypeNames, envAfter.Frames, definitions)

    let contents, _, _ = checkedDecl

    // By the declaration that defines each and the value's name, never the value: FinishFile finds the value in
    // whatever result of that declaration is current
    let references =
        referenced
        |> Seq.choose (fun v ->
            definitions.TryFind v.Stamp
            |> Option.map (fun declaration -> declaration, modulePathOf v, v.LogicalName))
        |> Seq.distinct
        |> Array.ofSeq

    let slots = slotsAtCheck delta
    let dependsOnLater = leavesFreeTypars || leavesOpenTypes delta checkedDecl
    // Before the fingerprint: collecting the unsolved variables normalizes the solutions of unit-of-measure
    // variables in place (FindUnsolved), as the finish's defaults then find them
    let leavesUnsolved =
        leavesFreeTypars
        || leavesUnsolved state envAfter.Env.DisplayEnv delta checkedDecl

    let fingerprint, atCheck =
        let _, _, attribs = checkedDecl

        let defaults =
            if leavesUnsolved && not dependsOnLater then
                Some(TMDefs contents, List.map snd attribs)
            else
                None

        match defaults with
        | Some defaults when not decl.IsModuleHeader && checks ->
            // What the declarations after it see: the cutoff for them
            let seen = fingerprintOf delta checkedDecl slots

            let scratch () =
                let scratch, _, _ =
                    DeclarationProjectCheck.newCheckerState
                        state
                        start.TcState
                        start.RootSigOpt
                        start.File
                        (TcResultsSinkImpl state.tcGlobals)

                scratch

            let finished =
                whileFinished state.tcGlobals scratch defaults env (fun () -> fingerprintOf delta checkedDecl slots)

            if String.Equals(seen, finished, StringComparison.Ordinal) then
                finished, ValueNone
            else
                finishedFingerprint (ValueSome seen) finished, ValueSome seen
        | _ -> fingerprintOf delta checkedDecl slots, ValueNone

    let typedUses, heldDefinitions =
        typedUsesOf definitions self delta contents (Some sink)

    DeclarationResult(
        start,
        decl,
        fingerprint,
        atCheck,
        DeclarationProjectCheck.diagnosticsOf state start.Parse.FileName capturing,
        logger.Diagnostics,
        leavesFreeTypars,
        dependsOnLater,
        openTypars envAfter.Env delta checkedDecl,
        leavesUnsolved,
        slots,
        envAfter,
        envAtEnd,
        checkedDecl,
        delta,
        deltaCombines,
        sink,
        Leftovers.Of cenv,
        nesting,
        typedUses,
        heldDefinitions,
        references,
        obj (),
        decl.ScopeRange
    )

let checkDeclaration
    (start: FileStart)
    (envBefore: DeclarationEnvironment)
    (decl: ParsedDeclaration)
    (ct: CancellationToken)
    : DeclarationResult =
    if decl.Depth > envBefore.Frames.Length then
        invalidArg
            (nameof envBefore)
            "The environment is outside the declaration's module: it is not the environment after the declaration before it."

    // The undo and the check under the FileStart's lock, then the file's check lock, as FinishFile and the lazy
    // check results take them: nothing that applies a finish's changes again (building the results applies the
    // conformance check) runs between the undo and the check
    lock start.Sync (fun () ->
        start.UndoLastFinish()
        afterUndoHook start.Parse.FileName

        // The check gives the file's compiler-generated names, from the counters BeginFile reset, and so does its
        // fingerprint (compiled names are given on first read): no other check of the file may run meanwhile. A
        // check cancelled while it waited here does not run.
        lock (start.Context.Checks.FileLock start.Parse.FileName) (fun () ->
            ct.ThrowIfCancellationRequested()
            checkDeclarationLocked start envBefore decl ct))

/// Whether an inference variable that any of `results` left is still open, with the latest finish undone: the
/// finish's defaults never count as solving one. Results checked in order with nothing checked after the last are
/// what later declarations would see.
let windowOpen (start: FileStart) (results: DeclarationResult[]) (ct: CancellationToken) : bool =
    lock start.Sync (fun () ->
        start.UndoLastFinish()

        lock (start.Context.Checks.FileLock start.Parse.FileName) (fun () ->
            ct.ThrowIfCancellationRequested()
            results |> Array.exists (fun result -> result.OpenTypars |> Array.exists isOpen)))

/// The fingerprints of `results` as they stand, with the latest finish undone: for a result that depended on later
/// declarations, what the declarations after them see once they have been solved.
let printsNow (start: FileStart) (results: DeclarationResult[]) : string[] =
    lock start.Sync (fun () ->
        start.UndoLastFinish()

        lock (start.Context.Checks.FileLock start.Parse.FileName) (fun () ->
            let state = start.Context.State
            let _, logger = DeclarationProjectCheck.newLoggers state
            use _ = new CompilationGlobalsScope(logger, BuildPhase.TypeCheck)

            results
            |> Array.map (fun result -> fingerprintOf result.Delta result.Checked result.SlotsAtCheck)))

let private shiftWith (s: DeclarationShift.Shifter) (result: DeclarationResult) (decl: ParsedDeclaration) : DeclarationResult voption =
    let start = result.Start
    let state = start.Context.State
    let rawDiagnostics = result.RawDiagnostics |> Array.map s.Diagnostic
    let leftovers = result.Leftovers.Shifted s
    let sink = s.Sink(state.tcGlobals, result.Sink)
    let contents, actions, attribs = result.Checked
    let contents = List.map s.Contents contents

    let attribs =
        attribs |> List.map (fun (targets, attrib) -> targets, s.Attrib attrib)

    let checkedDecl = contents, actions, attribs
    s.ModuleType result.Delta

    let nesting, envAfter =
        let env = result.EnvAfter

        match result.Nesting with
        | ModuleHeader(depth, frame) ->
            frame.Entity |> Option.iter (fun entity -> s.EntityWith(entity, false))

            let frame =
                { frame with
                    ScopeRange = s.Scope frame.ScopeRange
                    Range = s.Own frame.Range
                }

            let frames =
                match env.Frames with
                | _ :: outer -> frame :: outer
                | [] -> []

            ModuleHeader(depth, frame), DeclarationEnvironment(env.Env, env.Parent, env.TypeNames, frames, env.Definitions)
        | InModules _ as nesting ->
            let frames =
                match env.Frames with
                | frame :: outer when not frame.Checked.IsEmpty ->
                    { frame with
                        Checked = checkedDecl :: frame.Checked.Tail
                    }
                    :: outer
                | frames -> frames

            nesting, DeclarationEnvironment(env.Env, env.Parent, env.TypeNames, frames, env.Definitions)

    if s.Refused.IsSome then
        ValueNone
    else
        use _ = new CompilationGlobalsScope(DiscardErrorsLogger, BuildPhase.TypeCheck)

        let shifted =
            DeclarationResult(
                start,
                decl,
                result.Fingerprint,
                result.AtCheck,
                DeclarationProjectCheck.diagnosticsOf
                    state
                    start.Parse.FileName
                    (DeclarationProjectCheck.refiltered state.tcConfig.diagnosticsOptions rawDiagnostics),
                rawDiagnostics,
                result.LeavesFreeTypars,
                result.DependsOnLaterDeclarations,
                result.OpenTypars,
                result.LeavesUnsolved,
                result.SlotsAtCheck,
                envAfter,
                result.EnvAtEnd,
                checkedDecl,
                result.Delta,
                result.DeltaCombines,
                sink,
                leftovers,
                nesting,
                result.CheckTypedUses,
                result.HeldDefinitions,
                result.References,
                result.ShareKey,
                s.Move result.CapturedScope
            )

        shifted.SetFinishTypedUses result.FinishTypedUses
        ValueSome shifted

let private shiftLocked (result: DeclarationResult) (decl: ParsedDeclaration) (delta: int) : DeclarationResult voption =
    let old = result.Declaration

    let s =
        DeclarationShift.Shifter(old.Range.FileIndex, old.Range, result.CapturedScope.End, delta, result.ShareKey)

    // A refused or failed shift restores what it moved in place, so `result` is left as it was
    try
        match shiftWith s result decl with
        | ValueSome _ as shifted ->
            s.Release()
            shifted
        | ValueNone ->
            s.Undo()
            result.Start.Context.ShiftRefusedHook(defaultArg s.Refused "")
            ValueNone
    with _ ->
        s.Undo()
        reraise ()

/// Under the FileStart's lock.
let private shiftFrom (result: DeclarationResult) (decl: ParsedDeclaration) : DeclarationResult voption =
    let old = result.Declaration
    let delta = decl.Range.StartLine - old.Range.StartLine

    if
        delta = 0
        || not (decl.IsUnchangedFrom old)
        || decl.Range.StartColumn <> old.Range.StartColumn
        || decl.Within <> old.Within
        || decl.Key <> old.Key
        || decl.Depth <> old.Depth
        || decl.IsModuleHeader <> old.IsModuleHeader
        || not (String.Equals(decl.Range.FileName, old.Range.FileName, StringComparison.Ordinal))
        || result.Start.Context.RefuseShifts
    then
        ValueNone
    elif result.Leftovers.HasDeferredChecks then
        result.Start.Context.ShiftRefusedHook $"%A{old.Key.Name}: it holds deferred union nullness checks"
        ValueNone
    else
        let start = result.Start
        start.UndoLastFinish()

        lock (start.Context.Checks.FileLock start.Parse.FileName) (fun () -> shiftLocked result decl delta)

let shift (result: DeclarationResult) (decl: ParsedDeclaration) : DeclarationResult voption =
    if
        not (decl.IsUnchangedFrom result.Declaration)
        || decl.Key <> result.Declaration.Key
    then
        ValueNone
    else
        let start = result.Start

        lock start.Sync (fun () ->
            let chain = ResizeArray [ result ]

            while not (isNull chain[chain.Count - 1].ShiftedTo) do
                chain.Add(!!chain[chain.Count - 1].ShiftedTo)

            let latest = chain[chain.Count - 1]

            if
                latest.Declaration.Range.Start = decl.Range.Start
                && not (obj.ReferenceEquals(latest, result))
            then
                ValueSome latest
            else
                match shiftFrom latest decl with
                | ValueSome shifted as found ->
                    // Each earlier result points at the latest, so the results in between can be collected
                    for r in chain do
                        r.ShiftedTo <- shifted

                    found
                | ValueNone -> ValueNone)

let extendEnvLocked (envBefore: DeclarationEnvironment) (result: DeclarationResult) : DeclarationEnvironment =
    let decl = result.Declaration
    let start = result.Start
    let state = start.Context.State
    use _ = new CompilationGlobalsScope(DiscardErrorsLogger, BuildPhase.TypeCheck)
    use _ = FSharp.Compiler.Cancellable.UsingToken CancellationToken.None
    let envBefore = exitModules state start envBefore decl.Depth
    // Its own checker state and sink: resolving an open or a module abbreviation again reports nothing
    let cenv, _, _ =
        DeclarationProjectCheck.newCheckerState state start.TcState start.RootSigOpt start.File (TcResultsSinkImpl state.tcGlobals)

    let definitions =
        (DeclarationProjectCheck.addDefinitions envBefore.Definitions result.Ref result.Delta, result.HeldDefinitions)
        ||> Array.fold (fun acc (stamp, declaration) ->
            if acc.ContainsKey stamp then
                acc
            else
                Map.add stamp declaration acc)

    let contentsBefore = envBefore.Env.eModuleOrNamespaceTypeAccumulator.Value

    match result.Nesting with
    | ModuleHeader(_, frame) ->
        // As CheckDeclaration for a header: the module is published when it is left, not here
        let env =
            { envBefore.Env with
                eModuleOrNamespaceTypeAccumulator = ref contentsBefore
            }

        let frame =
            { frame with
                OuterEnv = env
                OuterParent = envBefore.Parent
                OuterTypeNames = envBefore.TypeNames
                Checked = []
            }

        match frame.Entity with
        | Some entity ->
            // As TcNestedModuleHeader and EnterNestedModuleDeclarations, with the module the header's check made
            let envForModule, _ =
                MakeInnerEnv true env entity.Id entity.ModuleOrNamespaceType.ModuleOrNamespaceKind

            let moduleDecls =
                match decl.Syntax with
                | SynModuleDecl.NestedModule(decls = decls) -> decls
                | _ -> []

            let typeNames = TypeNamesOfDeclarations cenv envForModule moduleDecls
            DeclarationEnvironment(envForModule, Parent(mkLocalModuleRef entity), typeNames, frame :: envBefore.Frames, definitions)
        | None -> DeclarationEnvironment(env, envBefore.Parent, envBefore.TypeNames, frame :: envBefore.Frames, definitions)
    | InModules _ ->
        let checks =
            start.Scope.ChecksDeclarations
            && (match envBefore.Frames with
                | frame :: _ -> frame.Entity.IsSome
                | [] -> true)

        // Replayed against what the module or namespace held before it, as its check saw it (a recursive
        // fragment reads that), then given what it publishes
        let env =
            { envBefore.Env with
                eModuleOrNamespaceTypeAccumulator = ref contentsBefore
            }

        let env =
            if checks then
                asOfDeclaration start definitions (fun () -> ExtendEnvWithDeclaration cenv decl.ScopeRange env decl.Syntax result.Delta)
            else
                env

        let accumulator = ref (mergeDelta contentsBefore result.Delta result.DeltaCombines)

        let env =
            { env with
                eModuleOrNamespaceTypeAccumulator = accumulator
            }

        let frames =
            match envBefore.Frames with
            | frame :: outer ->
                { frame with
                    Checked = result.Checked :: frame.Checked
                }
                :: outer
            | [] -> []

        DeclarationEnvironment(env, envBefore.Parent, envBefore.TypeNames, frames, definitions)

let extendEnv (envBefore: DeclarationEnvironment) (result: DeclarationResult) : DeclarationEnvironment =
    let decl = result.Declaration

    if decl.Depth > envBefore.Frames.Length then
        invalidArg
            (nameof envBefore)
            "The environment is outside the declaration's module: it is not the environment after the declaration before it."

    // As CheckDeclaration: nothing applies a finish's changes again while it reads the declaration's items
    lock result.Start.Sync (fun () ->
        result.Start.UndoLastFinish()
        extendEnvLocked envBefore result)

let extendEnvAdHoc (envBefore: DeclarationEnvironment) (result: DeclarationResult) : DeclarationEnvironment =
    if result.Declaration.Depth > envBefore.Frames.Length then
        invalidArg
            (nameof envBefore)
            "The environment is outside the declaration's module: it is not the environment after the declaration before it."

    lock result.Start.Sync (fun () -> keepingModuleTypes (framesOf envBefore) (fun () -> extendEnvLocked envBefore result))

let checkDeclarationAdHoc
    (start: FileStart)
    (envBefore: DeclarationEnvironment)
    (decl: ParsedDeclaration)
    (ct: CancellationToken)
    : DeclarationResult =
    if decl.Depth > envBefore.Frames.Length then
        invalidArg
            (nameof envBefore)
            "The environment is outside the declaration's module: it is not the environment after the declaration before it."

    lock start.Sync (fun () ->
        lock (start.Context.Checks.FileLock start.Parse.FileName) (fun () ->
            ct.ThrowIfCancellationRequested()

            keepingModuleTypes (framesOf envBefore) (fun () ->
                let result = checkDeclarationLocked start envBefore decl ct
                result.IsAdHoc <- true
                result)))
