// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Defines derived expression manipulation and construction functions.
namespace FSharp.Compiler.TypedTreeOps

open System
open System.CodeDom.Compiler
open System.Collections.Generic
open System.Collections.Immutable
open Internal.Utilities
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Internal.Utilities.Rational

open FSharp.Compiler.IO
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Text.Layout
open FSharp.Compiler.Text.LayoutRender
open FSharp.Compiler.Text.TaggedText
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
#if !NO_TYPEPROVIDERS
open FSharp.Compiler.TypeProviders
#endif

[<AutoOpen>]
module internal TypeRemapping =

    let inline compareBy (x: 'T | null) (y: 'T | null) ([<InlineIfLambda>] func: 'T -> 'K) =
        match x, y with
        | null, null -> 0
        | null, _ -> -1
        | _, null -> 1
        | x, y -> compare (func !!x) (func !!y)

    //---------------------------------------------------------------------------
    // Basic data structures
    //---------------------------------------------------------------------------

    [<NoEquality; NoComparison>]
    type TyparMap<'T> =
        | TPMap of StampMap<'T>

        member tm.Item
            with get (tp: Typar) =
                let (TPMap m) = tm
                m[tp.Stamp]

        member tm.ContainsKey(tp: Typar) =
            let (TPMap m) = tm
            m.ContainsKey(tp.Stamp)

        member tm.TryGetValue(tp: Typar) =
            let (TPMap m) = tm
            m.TryGetValue(tp.Stamp)

        member tm.TryFind(tp: Typar) =
            let (TPMap m) = tm
            m.TryFind(tp.Stamp)

        member tm.Add(tp: Typar, x) =
            let (TPMap m) = tm
            TPMap(m.Add(tp.Stamp, x))

        static member Empty: TyparMap<'T> = TPMap Map.empty

    [<NoEquality; NoComparison; Sealed>]
    type TyconRefMap<'T>(imap: StampMap<'T>) =
        member _.Item
            with get (tcref: TyconRef) = imap[tcref.Stamp]

        member _.TryFind(tcref: TyconRef) = imap.TryFind tcref.Stamp
        member _.ContainsKey(tcref: TyconRef) = imap.ContainsKey tcref.Stamp
        member _.Add (tcref: TyconRef) x = TyconRefMap(imap.Add(tcref.Stamp, x))
        member _.Remove(tcref: TyconRef) = TyconRefMap(imap.Remove tcref.Stamp)
        member _.IsEmpty = imap.IsEmpty
        member _.TryGetValue(tcref: TyconRef) = imap.TryGetValue tcref.Stamp

        static member Empty: TyconRefMap<'T> = TyconRefMap Map.empty

        static member OfList vs =
            (vs, TyconRefMap<'T>.Empty) ||> List.foldBack (fun (x, y) acc -> acc.Add x y)

    [<Struct>]
    [<NoEquality; NoComparison>]
    type ValMap<'T>(imap: StampMap<'T>) =

        member _.Contents = imap

        member _.Item
            with get (v: Val) = imap[v.Stamp]

        member _.TryFind(v: Val) = imap.TryFind v.Stamp
        member _.ContainsVal(v: Val) = imap.ContainsKey v.Stamp
        member _.Add (v: Val) x = ValMap(imap.Add(v.Stamp, x))
        member _.Remove(v: Val) = ValMap(imap.Remove(v.Stamp))
        static member Empty = ValMap<'T> Map.empty
        member _.IsEmpty = imap.IsEmpty

        static member OfList vs =
            (vs, ValMap<'T>.Empty) ||> List.foldBack (fun (x, y) acc -> acc.Add x y)

    //--------------------------------------------------------------------------
    // renamings
    //--------------------------------------------------------------------------

    type TyparInstantiation = (Typar * TType) list

    type TyconRefRemap = TyconRefMap<TyconRef>
    type ValRemap = ValMap<ValRef>

    let emptyTyconRefRemap: TyconRefRemap = TyconRefMap<_>.Empty
    let emptyTyparInst = ([]: TyparInstantiation)

    [<NoEquality; NoComparison>]
    type Remap =
        {
            tpinst: TyparInstantiation

            /// Values to remap
            valRemap: ValRemap

            /// TyconRefs to remap
            tyconRefRemap: TyconRefRemap

            /// Remove existing trait solutions?
            removeTraitSolutions: bool
        }

    let emptyRemap =
        {
            tpinst = emptyTyparInst
            tyconRefRemap = emptyTyconRefRemap
            valRemap = ValMap.Empty
            removeTraitSolutions = false
        }

    type Remap with
        static member Empty = emptyRemap

    //--------------------------------------------------------------------------
    // Substitute for type variables and remap type constructors
    //--------------------------------------------------------------------------

    let addTyconRefRemap tcref1 tcref2 tmenv =
        { tmenv with
            tyconRefRemap = tmenv.tyconRefRemap.Add tcref1 tcref2
        }

    let isRemapEmpty remap =
        isNil remap.tpinst && remap.tyconRefRemap.IsEmpty && remap.valRemap.IsEmpty

    let rec instTyparRef tpinst ty tp =
        match tpinst with
        | [] -> ty
        | (tpR, tyR) :: t -> if typarEq tp tpR then tyR else instTyparRef t ty tp

    let remapTyconRef (tcmap: TyconRefMap<_>) tcref =
        match tcmap.TryFind tcref with
        | Some tcref -> tcref
        | None -> tcref

    let remapUnionCaseRef tcmap (UnionCaseRef(tcref, nm)) =
        UnionCaseRef(remapTyconRef tcmap tcref, nm)

    let remapRecdFieldRef tcmap (RecdFieldRef(tcref, nm)) =
        RecdFieldRef(remapTyconRef tcmap tcref, nm)

    let mkTyparInst (typars: Typars) tyargs =
        (List.zip typars tyargs: TyparInstantiation)

    let generalizeTypar tp = mkTyparTy tp
    let generalizeTypars tps = List.map generalizeTypar tps

    let rec remapTypeAux (tyenv: Remap) (ty: TType) =
        let ty = stripTyparEqns ty

        match ty with
        | TType_var(tp, nullness) as ty ->
            let res = instTyparRef tyenv.tpinst ty tp
            addNullnessToTy nullness res

        | TType_app(tcref, tinst, flags) as ty ->
            match tyenv.tyconRefRemap.TryFind tcref with
            | Some tcrefR -> TType_app(tcrefR, remapTypesAux tyenv tinst, flags)
            | None ->
                match tinst with
                | [] -> ty // optimization to avoid re-allocation of TType_app node in the common case
                | _ ->
                    // avoid reallocation on idempotent
                    let tinstR = remapTypesAux tyenv tinst

                    if tinst === tinstR then
                        ty
                    else
                        TType_app(tcref, tinstR, flags)

        | TType_ucase(UnionCaseRef(tcref, n), tinst) ->
            match tyenv.tyconRefRemap.TryFind tcref with
            | Some tcrefR -> TType_ucase(UnionCaseRef(tcrefR, n), remapTypesAux tyenv tinst)
            | None -> TType_ucase(UnionCaseRef(tcref, n), remapTypesAux tyenv tinst)

        | TType_anon(anonInfo, l) as ty ->
            let tupInfoR = remapTupInfoAux tyenv anonInfo.TupInfo
            let lR = remapTypesAux tyenv l

            if anonInfo.TupInfo === tupInfoR && l === lR then
                ty
            else
                TType_anon(AnonRecdTypeInfo.Create(anonInfo.Assembly, tupInfoR, anonInfo.SortedIds), lR)

        | TType_tuple(tupInfo, l) as ty ->
            let tupInfoR = remapTupInfoAux tyenv tupInfo
            let lR = remapTypesAux tyenv l

            if tupInfo === tupInfoR && l === lR then
                ty
            else
                TType_tuple(tupInfoR, lR)

        | TType_fun(domainTy, rangeTy, flags) as ty ->
            let domainTyR = remapTypeAux tyenv domainTy
            let retTyR = remapTypeAux tyenv rangeTy

            if domainTy === domainTyR && rangeTy === retTyR then
                ty
            else
                TType_fun(domainTyR, retTyR, flags)

        | TType_forall(tps, ty) ->
            let tpsR, tyenv = copyAndRemapAndBindTypars tyenv tps
            TType_forall(tpsR, remapTypeAux tyenv ty)

        | TType_measure unt -> TType_measure(remapMeasureAux tyenv unt)

    and remapMeasureAux tyenv unt =
        match unt with
        | Measure.One _ -> unt
        | Measure.Const(entityRef, m) ->
            match tyenv.tyconRefRemap.TryFind entityRef with
            | Some tcref -> Measure.Const(tcref, m)
            | None -> unt
        | Measure.Prod(u1, u2, m) -> Measure.Prod(remapMeasureAux tyenv u1, remapMeasureAux tyenv u2, m)
        | Measure.RationalPower(u, q) -> Measure.RationalPower(remapMeasureAux tyenv u, q)
        | Measure.Inv u -> Measure.Inv(remapMeasureAux tyenv u)
        | Measure.Var tp as unt ->
            match tp.Solution with
            | None ->
                match ListAssoc.tryFind typarEq tp tyenv.tpinst with
                | Some tpTy ->
                    match tpTy with
                    | TType_measure unt -> unt
                    | TType_var(typar = typar) when tp.Kind = TyparKind.Measure ->
                        // This is a measure typar that is not yet solved, so we can't remap it
                        error (Error(FSComp.SR.tcExpectedTypeParamMarkedWithUnitOfMeasureAttribute (), typar.Range))
                    | _ -> failwith "remapMeasureAux: incorrect kinds"
                | None -> unt
            | Some(TType_measure unt) -> remapMeasureAux tyenv unt
            | Some ty -> failwithf "incorrect kinds: %A" ty

    and remapTupInfoAux _tyenv unt =
        match unt with
        | TupInfo.Const _ -> unt

    and remapTypesAux tyenv types = List.mapq (remapTypeAux tyenv) types

    and remapTyparConstraintsAux tyenv cs =
        cs
        |> List.choose (fun x ->
            match x with
            | TyparConstraint.CoercesTo(ty, m) -> Some(TyparConstraint.CoercesTo(remapTypeAux tyenv ty, m))
            | TyparConstraint.MayResolveMember(traitInfo, m) -> Some(TyparConstraint.MayResolveMember(remapTraitInfo tyenv traitInfo, m))
            | TyparConstraint.DefaultsTo(priority, ty, m) -> Some(TyparConstraint.DefaultsTo(priority, remapTypeAux tyenv ty, m))
            | TyparConstraint.IsEnum(underlyingTy, m) -> Some(TyparConstraint.IsEnum(remapTypeAux tyenv underlyingTy, m))
            | TyparConstraint.IsDelegate(argTys, retTy, m) ->
                Some(TyparConstraint.IsDelegate(remapTypeAux tyenv argTys, remapTypeAux tyenv retTy, m))
            | TyparConstraint.SimpleChoice(tys, m) -> Some(TyparConstraint.SimpleChoice(remapTypesAux tyenv tys, m))
            | TyparConstraint.SupportsComparison _
            | TyparConstraint.SupportsEquality _
            | TyparConstraint.SupportsNull _
            | TyparConstraint.NotSupportsNull _
            | TyparConstraint.IsUnmanaged _
            | TyparConstraint.AllowsRefStruct _
            | TyparConstraint.IsNonNullableStruct _
            | TyparConstraint.IsReferenceType _
            | TyparConstraint.RequiresDefaultConstructor _ -> Some x)

    and remapTraitInfo tyenv (TTrait(tys, nm, flags, argTys, retTy, source, slnCell)) =
        let slnCell =
            match slnCell.Value with
            | None -> None
            | _ when tyenv.removeTraitSolutions -> None
            | Some sln ->
                let sln =
                    match sln with
                    | ILMethSln(ty, extOpt, ilMethRef, minst, staticTyOpt) ->
                        ILMethSln(
                            remapTypeAux tyenv ty,
                            extOpt,
                            ilMethRef,
                            remapTypesAux tyenv minst,
                            Option.map (remapTypeAux tyenv) staticTyOpt
                        )
                    | FSMethSln(ty, vref, minst, staticTyOpt) ->
                        FSMethSln(
                            remapTypeAux tyenv ty,
                            remapValRef tyenv vref,
                            remapTypesAux tyenv minst,
                            Option.map (remapTypeAux tyenv) staticTyOpt
                        )
                    | FSRecdFieldSln(tinst, rfref, isSet) ->
                        FSRecdFieldSln(remapTypesAux tyenv tinst, remapRecdFieldRef tyenv.tyconRefRemap rfref, isSet)
                    | FSAnonRecdFieldSln(anonInfo, tinst, n) -> FSAnonRecdFieldSln(anonInfo, remapTypesAux tyenv tinst, n)
                    | BuiltInSln -> BuiltInSln
                    | ClosedExprSln e -> ClosedExprSln e // no need to remap because it is a closed expression, referring only to external types

                Some sln

        let tysR = remapTypesAux tyenv tys
        let argTysR = remapTypesAux tyenv argTys
        let retTyR = Option.map (remapTypeAux tyenv) retTy

        // Note: we reallocate a new solution cell on every traversal of a trait constraint
        // This feels incorrect for trait constraints that are quantified: it seems we should have
        // formal binders for trait constraints when they are quantified, just as
        // we have formal binders for type variables.
        //
        // The danger here is that a solution for one syntactic occurrence of a trait constraint won't
        // be propagated to other, "linked" solutions. However trait constraints don't appear in any algebra
        // in the same way as types
        let newSlnCell = ref slnCell

        TTrait(tysR, nm, flags, argTysR, retTyR, source, newSlnCell)

    and bindTypars tps tyargs tpinst =
        match tps with
        | [] -> tpinst
        | _ -> List.map2 (fun tp tyarg -> (tp, tyarg)) tps tyargs @ tpinst

    // This version is used to remap most type parameters, e.g. ones bound at tycons, vals, records
    // See notes below on remapTypeFull for why we have a function that accepts remapAttribs as an argument
    and copyAndRemapAndBindTyparsFull remapAttrib tyenv tps =
        match tps with
        | [] -> tps, tyenv
        | _ ->
            let tpsR = copyTypars false tps

            let tyenv =
                { tyenv with
                    tpinst = bindTypars tps (generalizeTypars tpsR) tyenv.tpinst
                }

            (tps, tpsR)
            ||> List.iter2 (fun tporig tp ->
                tp.SetConstraints(remapTyparConstraintsAux tyenv tporig.Constraints)
                tp.SetAttribs(tporig.Attribs |> remapAttrib))

            tpsR, tyenv

    // copies bound typars, extends tpinst
    and copyAndRemapAndBindTypars tyenv tps =
        copyAndRemapAndBindTyparsFull (fun _ -> []) tyenv tps

    and remapValLinkage tyenv (vlink: ValLinkageFullKey) =
        let tyOpt = vlink.TypeForLinkage

        let tyOptR =
            match tyOpt with
            | None -> tyOpt
            | Some ty ->
                let tyR = remapTypeAux tyenv ty
                if ty === tyR then tyOpt else Some tyR

        if tyOpt === tyOptR then
            vlink
        else
            ValLinkageFullKey(vlink.PartialKey, tyOptR)

    and remapNonLocalValRef tyenv (nlvref: NonLocalValOrMemberRef) =
        let eref = nlvref.EnclosingEntity
        let erefR = remapTyconRef tyenv.tyconRefRemap eref
        let vlink = nlvref.ItemKey
        let vlinkR = remapValLinkage tyenv vlink

        if eref === erefR && vlink === vlinkR then
            nlvref
        else
            {
                EnclosingEntity = erefR
                ItemKey = vlinkR
            }

    and remapValRef tmenv (vref: ValRef) =
        match tmenv.valRemap.TryFind vref.Deref with
        | None ->
            if vref.IsLocalRef then
                vref
            else
                let nlvref = vref.nlr
                let nlvrefR = remapNonLocalValRef tmenv nlvref
                if nlvref === nlvrefR then vref else VRefNonLocal nlvrefR
        | Some res -> res

    let remapType tyenv x =
        if isRemapEmpty tyenv then x else remapTypeAux tyenv x

    let remapTypes tyenv x =
        if isRemapEmpty tyenv then x else remapTypesAux tyenv x

    /// Use this one for any type that may be a forall type where the type variables may contain attributes
    /// Logically speaking this is mutually recursive with remapAttribImpl defined much later in this file,
    /// because types may contain forall types that contain attributes, which need to be remapped.
    /// We currently break the recursion by passing in remapAttribImpl as a function parameter.
    /// Use this one for any type that may be a forall type where the type variables may contain attributes
    let remapTypeFull remapAttrib tyenv ty =
        if isRemapEmpty tyenv then
            ty
        else
            match stripTyparEqns ty with
            | TType_forall(tps, tau) ->
                let tpsR, tyenvinner = copyAndRemapAndBindTyparsFull remapAttrib tyenv tps
                TType_forall(tpsR, remapType tyenvinner tau)
            | _ -> remapType tyenv ty

    let remapParam tyenv (TSlotParam(nm, ty, fl1, fl2, fl3, attribs) as x) =
        if isRemapEmpty tyenv then
            x
        else
            TSlotParam(nm, remapTypeAux tyenv ty, fl1, fl2, fl3, attribs)

    let remapSlotSig remapAttrib tyenv (TSlotSig(nm, ty, ctps, methTypars, paraml, retTy) as x) =
        if isRemapEmpty tyenv then
            x
        else
            let tyR = remapTypeAux tyenv ty
            let ctpsR, tyenvinner = copyAndRemapAndBindTyparsFull remapAttrib tyenv ctps

            let methTyparsR, tyenvinner =
                copyAndRemapAndBindTyparsFull remapAttrib tyenvinner methTypars

            TSlotSig(
                nm,
                tyR,
                ctpsR,
                methTyparsR,
                List.mapSquared (remapParam tyenvinner) paraml,
                Option.map (remapTypeAux tyenvinner) retTy
            )

    let mkInstRemap tpinst =
        {
            tyconRefRemap = emptyTyconRefRemap
            tpinst = tpinst
            valRemap = ValMap.Empty
            removeTraitSolutions = false
        }

    // entry points for "typar -> TType" instantiation
    let instType tpinst x =
        if isNil tpinst then
            x
        else
            remapTypeAux (mkInstRemap tpinst) x

    let instTypes tpinst x =
        if isNil tpinst then
            x
        else
            remapTypesAux (mkInstRemap tpinst) x

    let instTrait tpinst x =
        if isNil tpinst then
            x
        else
            remapTraitInfo (mkInstRemap tpinst) x

    let instTyparConstraints tpinst x =
        if isNil tpinst then
            x
        else
            remapTyparConstraintsAux (mkInstRemap tpinst) x

    let instSlotSig tpinst ss =
        remapSlotSig (fun _ -> []) (mkInstRemap tpinst) ss

    let copySlotSig ss =
        remapSlotSig (fun _ -> []) Remap.Empty ss

    /// Decouple SRTP constraint solution ref cells on typars from any shared expression-tree nodes.
    /// In FSI, codegen mutates shared TTrait solution cells after typechecking; decoupling at
    /// generalization prevents stale solutions from bleeding into subsequent submissions. See #12386.
    let decoupleTraitSolutions (typars: Typars) =
        for tp in typars do
            tp.SetConstraints(
                tp.Constraints
                |> List.map (fun cx ->
                    match cx with
                    | TyparConstraint.MayResolveMember(traitInfo, m) ->
                        TyparConstraint.MayResolveMember(traitInfo.CloneWithFreshSolution(), m)
                    | c -> c)
            )

    let mkTyparToTyparRenaming tpsorig tps =
        let tinst = generalizeTypars tps
        mkTyparInst tpsorig tinst, tinst

    let mkTyconInst (tycon: Tycon) tinst = mkTyparInst tycon.TyparsNoRange tinst
    let mkTyconRefInst (tcref: TyconRef) tinst = mkTyconInst tcref.Deref tinst

[<AutoOpen>]
module internal MeasureOps =

    //---------------------------------------------------------------------------
    // Basic equalities
    //---------------------------------------------------------------------------

    let tyconRefEq (g: TcGlobals) tcref1 tcref2 =
        primEntityRefEq g.compilingFSharpCore g.fslibCcu tcref1 tcref2

    let valRefEq (g: TcGlobals) vref1 vref2 =
        primValRefEq g.compilingFSharpCore g.fslibCcu vref1 vref2

    //---------------------------------------------------------------------------
    // Remove inference equations and abbreviations from units
    //---------------------------------------------------------------------------

    let reduceTyconRefAbbrevMeasureable (tcref: TyconRef) =
        let abbrev = tcref.TypeAbbrev

        match abbrev with
        | Some(TType_measure ms) -> ms
        | _ -> invalidArg "tcref" "not a measure abbreviation, or incorrect kind"

    let rec stripUnitEqnsFromMeasureAux canShortcut unt =
        match stripUnitEqnsAux canShortcut unt with
        | Measure.Const(tyconRef = tcref) when tcref.IsTypeAbbrev ->
            stripUnitEqnsFromMeasureAux canShortcut (reduceTyconRefAbbrevMeasureable tcref)
        | m -> m

    let stripUnitEqnsFromMeasure m = stripUnitEqnsFromMeasureAux false m

    //---------------------------------------------------------------------------
    // Basic unit stuff
    //---------------------------------------------------------------------------

    /// What is the contribution of unit-of-measure constant ucref to unit-of-measure expression measure?
    let rec MeasureExprConExponent g abbrev ucref unt =
        match
            (if abbrev then
                 stripUnitEqnsFromMeasure unt
             else
                 stripUnitEqns unt)
        with
        | Measure.Const(tyconRef = ucrefR) ->
            if tyconRefEq g ucrefR ucref then
                OneRational
            else
                ZeroRational
        | Measure.Inv untR -> NegRational(MeasureExprConExponent g abbrev ucref untR)
        | Measure.Prod(measure1 = unt1; measure2 = unt2) ->
            AddRational (MeasureExprConExponent g abbrev ucref unt1) (MeasureExprConExponent g abbrev ucref unt2)
        | Measure.RationalPower(measure = untR; power = q) -> MulRational (MeasureExprConExponent g abbrev ucref untR) q
        | _ -> ZeroRational

    /// What is the contribution of unit-of-measure constant ucref to unit-of-measure expression measure
    /// after remapping tycons?
    let rec MeasureConExponentAfterRemapping g r ucref unt =
        match stripUnitEqnsFromMeasure unt with
        | Measure.Const(tyconRef = ucrefR) ->
            if tyconRefEq g (r ucrefR) ucref then
                OneRational
            else
                ZeroRational
        | Measure.Inv untR -> NegRational(MeasureConExponentAfterRemapping g r ucref untR)
        | Measure.Prod(measure1 = unt1; measure2 = unt2) ->
            AddRational (MeasureConExponentAfterRemapping g r ucref unt1) (MeasureConExponentAfterRemapping g r ucref unt2)
        | Measure.RationalPower(measure = untR; power = q) -> MulRational (MeasureConExponentAfterRemapping g r ucref untR) q
        | _ -> ZeroRational

    /// What is the contribution of unit-of-measure variable tp to unit-of-measure expression unt?
    let rec MeasureVarExponent tp unt =
        match stripUnitEqnsFromMeasure unt with
        | Measure.Var tpR -> if typarEq tp tpR then OneRational else ZeroRational
        | Measure.Inv untR -> NegRational(MeasureVarExponent tp untR)
        | Measure.Prod(measure1 = unt1; measure2 = unt2) -> AddRational (MeasureVarExponent tp unt1) (MeasureVarExponent tp unt2)
        | Measure.RationalPower(measure = untR; power = q) -> MulRational (MeasureVarExponent tp untR) q
        | _ -> ZeroRational

    /// List the *literal* occurrences of unit variables in a unit expression, without repeats
    let ListMeasureVarOccs unt =
        let rec gather acc unt =
            match stripUnitEqnsFromMeasure unt with
            | Measure.Var tp -> if List.exists (typarEq tp) acc then acc else tp :: acc
            | Measure.Prod(measure1 = unt1; measure2 = unt2) -> gather (gather acc unt1) unt2
            | Measure.RationalPower(measure = untR) -> gather acc untR
            | Measure.Inv untR -> gather acc untR
            | _ -> acc

        gather [] unt

    /// List the *observable* occurrences of unit variables in a unit expression, without repeats, paired with their non-zero exponents
    let ListMeasureVarOccsWithNonZeroExponents untexpr =
        let rec gather acc unt =
            match stripUnitEqnsFromMeasure unt with
            | Measure.Var tp ->
                if List.exists (fun (tpR, _) -> typarEq tp tpR) acc then
                    acc
                else
                    let e = MeasureVarExponent tp untexpr
                    if e = ZeroRational then acc else (tp, e) :: acc
            | Measure.Prod(measure1 = unt1; measure2 = unt2) -> gather (gather acc unt1) unt2
            | Measure.Inv untR -> gather acc untR
            | Measure.RationalPower(measure = untR) -> gather acc untR
            | _ -> acc

        gather [] untexpr

    /// List the *observable* occurrences of unit constants in a unit expression, without repeats, paired with their non-zero exponents
    let ListMeasureConOccsWithNonZeroExponents g eraseAbbrevs untexpr =
        let rec gather acc unt =
            match
                (if eraseAbbrevs then
                     stripUnitEqnsFromMeasure unt
                 else
                     stripUnitEqns unt)
            with
            | Measure.Const(tyconRef = c) ->
                if List.exists (fun (cR, _) -> tyconRefEq g c cR) acc then
                    acc
                else
                    let e = MeasureExprConExponent g eraseAbbrevs c untexpr
                    if e = ZeroRational then acc else (c, e) :: acc
            | Measure.Prod(measure1 = unt1; measure2 = unt2) -> gather (gather acc unt1) unt2
            | Measure.Inv untR -> gather acc untR
            | Measure.RationalPower(measure = untR) -> gather acc untR
            | _ -> acc

        gather [] untexpr

    /// List the *literal* occurrences of unit constants in a unit expression, without repeats,
    /// and after applying a remapping function r to tycons
    let ListMeasureConOccsAfterRemapping g r unt =
        let rec gather acc unt =
            match stripUnitEqnsFromMeasure unt with
            | Measure.Const(tyconRef = c) ->
                if List.exists (tyconRefEq g (r c)) acc then
                    acc
                else
                    r c :: acc
            | Measure.Prod(measure1 = unt1; measure2 = unt2) -> gather (gather acc unt1) unt2
            | Measure.RationalPower(measure = untR) -> gather acc untR
            | Measure.Inv untR -> gather acc untR
            | _ -> acc

        gather [] unt

    /// Construct a measure expression representing the n'th power of a measure
    let MeasurePower u n =
        if n = 1 then u
        elif n = 0 then Measure.One(range0)
        else Measure.RationalPower(u, intToRational n)

    let MeasureProdOpt m1 m2 =
        match m1, m2 with
        | Measure.One _, _ -> m2
        | _, Measure.One _ -> m1
        | _, _ -> Measure.Prod(m1, m2, unionRanges m1.Range m2.Range)

    /// Construct a measure expression representing the product of a list of measures
    let ProdMeasures ms =
        match ms with
        | [] -> Measure.One(range0)
        | m :: ms -> List.foldBack MeasureProdOpt ms m

    let isDimensionless g ty =
        match stripTyparEqns ty with
        | TType_measure unt ->
            isNil (ListMeasureVarOccsWithNonZeroExponents unt)
            && isNil (ListMeasureConOccsWithNonZeroExponents g true unt)
        | _ -> false

    let destUnitParMeasure g unt =
        let vs = ListMeasureVarOccsWithNonZeroExponents unt
        let cs = ListMeasureConOccsWithNonZeroExponents g true unt

        match vs, cs with
        | [ (v, e) ], [] when e = OneRational -> v
        | _, _ -> failwith "destUnitParMeasure: not a unit-of-measure parameter"

    let isUnitParMeasure g unt =
        let vs = ListMeasureVarOccsWithNonZeroExponents unt
        let cs = ListMeasureConOccsWithNonZeroExponents g true unt

        match vs, cs with
        | [ (_, e) ], [] when e = OneRational -> true
        | _, _ -> false

    let normalizeMeasure g ms =
        let vs = ListMeasureVarOccsWithNonZeroExponents ms
        let cs = ListMeasureConOccsWithNonZeroExponents g false ms

        match vs, cs with
        | [], [] -> Measure.One(ms.Range)
        | [ (v, e) ], [] when e = OneRational -> Measure.Var v
        | vs, cs ->
            List.foldBack
                (fun (v, e) ->
                    fun unt ->
                        let measureVar = Measure.Var(v)
                        let measureRational = Measure.RationalPower(measureVar, e)
                        Measure.Prod(measureRational, unt, unionRanges measureRational.Range unt.Range))
                vs
                (List.foldBack
                    (fun (c, e) ->
                        fun unt ->
                            let measureConst = Measure.Const(c, c.Range)
                            let measureRational = Measure.RationalPower(measureConst, e)
                            let prodM = unionRanges measureConst.Range unt.Range
                            Measure.Prod(measureRational, unt, prodM))
                    cs
                    (Measure.One(ms.Range)))

    let tryNormalizeMeasureInType g ty =
        match ty with
        | TType_measure(Measure.Var v) ->
            match v.Solution with
            | Some(TType_measure ms) ->
                v.typar_solution <- Some(TType_measure(normalizeMeasure g ms))
                ty
            | _ -> ty
        | _ -> ty

[<AutoOpen>]
module internal TypeBuilders =

    //---------------------------------------------------------------------------
    // Some basic type builders
    //---------------------------------------------------------------------------

    let mkForallTy d r = TType_forall(d, r)

    let mkForallTyIfNeeded d r = if isNil d then r else mkForallTy d r

    let (+->) d r = mkForallTyIfNeeded d r

    //---------------------------------------------------------------------------
    // Make some common types
    //---------------------------------------------------------------------------

    let mkFunTy (g: TcGlobals) domainTy rangeTy =
        TType_fun(domainTy, rangeTy, g.knownWithoutNull)

    let mkIteratedFunTy g dl r = List.foldBack (mkFunTy g) dl r

    let mkNativePtrTy (g: TcGlobals) ty =
        assert g.nativeptr_tcr.CanDeref // this should always be available, but check anyway
        TType_app(g.nativeptr_tcr, [ ty ], g.knownWithoutNull)

    let mkByrefTy (g: TcGlobals) ty =
        assert g.byref_tcr.CanDeref // this should always be available, but check anyway
        TType_app(g.byref_tcr, [ ty ], g.knownWithoutNull)

    let mkInByrefTy (g: TcGlobals) ty =
        if g.inref_tcr.CanDeref then // If not using sufficient FSharp.Core, then inref<T> = byref<T>, see RFC FS-1053.md
            TType_app(g.inref_tcr, [ ty ], g.knownWithoutNull)
        else
            mkByrefTy g ty

    let mkOutByrefTy (g: TcGlobals) ty =
        if g.outref_tcr.CanDeref then // If not using sufficient FSharp.Core, then outref<T> = byref<T>, see RFC FS-1053.md
            TType_app(g.outref_tcr, [ ty ], g.knownWithoutNull)
        else
            mkByrefTy g ty

    let mkByrefTyWithFlag g readonly ty =
        if readonly then mkInByrefTy g ty else mkByrefTy g ty

    let mkByref2Ty (g: TcGlobals) ty1 ty2 =
        assert g.byref2_tcr.CanDeref // check we are using sufficient FSharp.Core, caller should check this
        TType_app(g.byref2_tcr, [ ty1; ty2 ], g.knownWithoutNull)

    let mkVoidPtrTy (g: TcGlobals) =
        assert g.voidptr_tcr.CanDeref // check we are using sufficient FSharp.Core, caller should check this
        TType_app(g.voidptr_tcr, [], g.knownWithoutNull)

    let mkByrefTyWithInference (g: TcGlobals) ty1 ty2 =
        if g.byref2_tcr.CanDeref then // If not using sufficient FSharp.Core, then inref<T> = byref<T>, see RFC FS-1053.md
            TType_app(g.byref2_tcr, [ ty1; ty2 ], g.knownWithoutNull)
        else
            TType_app(g.byref_tcr, [ ty1 ], g.knownWithoutNull)

    let mkArrayTy (g: TcGlobals) rank nullness ty m =
        if rank < 1 || rank > 32 then
            errorR (Error(FSComp.SR.tastopsMaxArrayThirtyTwo rank, m))
            TType_app(g.il_arr_tcr_map[3], [ ty ], nullness)
        else
            TType_app(g.il_arr_tcr_map[rank - 1], [ ty ], nullness)

    //--------------------------------------------------------------------------
    // Tuple compilation (types)
    //------------------------------------------------------------------------

    let maxTuple = 8
    let goodTupleFields = maxTuple - 1

    let isCompiledTupleTyconRef g tcref =
        tyconRefEq g g.ref_tuple1_tcr tcref
        || tyconRefEq g g.ref_tuple2_tcr tcref
        || tyconRefEq g g.ref_tuple3_tcr tcref
        || tyconRefEq g g.ref_tuple4_tcr tcref
        || tyconRefEq g g.ref_tuple5_tcr tcref
        || tyconRefEq g g.ref_tuple6_tcr tcref
        || tyconRefEq g g.ref_tuple7_tcr tcref
        || tyconRefEq g g.ref_tuple8_tcr tcref
        || tyconRefEq g g.struct_tuple1_tcr tcref
        || tyconRefEq g g.struct_tuple2_tcr tcref
        || tyconRefEq g g.struct_tuple3_tcr tcref
        || tyconRefEq g g.struct_tuple4_tcr tcref
        || tyconRefEq g g.struct_tuple5_tcr tcref
        || tyconRefEq g g.struct_tuple6_tcr tcref
        || tyconRefEq g g.struct_tuple7_tcr tcref
        || tyconRefEq g g.struct_tuple8_tcr tcref

    let mkCompiledTupleTyconRef (g: TcGlobals) isStruct n =
        if n = 1 then
            (if isStruct then g.struct_tuple1_tcr else g.ref_tuple1_tcr)
        elif n = 2 then
            (if isStruct then g.struct_tuple2_tcr else g.ref_tuple2_tcr)
        elif n = 3 then
            (if isStruct then g.struct_tuple3_tcr else g.ref_tuple3_tcr)
        elif n = 4 then
            (if isStruct then g.struct_tuple4_tcr else g.ref_tuple4_tcr)
        elif n = 5 then
            (if isStruct then g.struct_tuple5_tcr else g.ref_tuple5_tcr)
        elif n = 6 then
            (if isStruct then g.struct_tuple6_tcr else g.ref_tuple6_tcr)
        elif n = 7 then
            (if isStruct then g.struct_tuple7_tcr else g.ref_tuple7_tcr)
        elif n = 8 then
            (if isStruct then g.struct_tuple8_tcr else g.ref_tuple8_tcr)
        else
            failwithf "mkCompiledTupleTyconRef, n = %d" n

    /// Convert from F# tuple types to .NET tuple types
    let rec mkCompiledTupleTy g isStruct tupElemTys =
        let n = List.length tupElemTys

        if n < maxTuple then
            TType_app(mkCompiledTupleTyconRef g isStruct n, tupElemTys, g.knownWithoutNull)
        else
            let tysA, tysB = List.splitAfter goodTupleFields tupElemTys

            TType_app(
                (if isStruct then g.struct_tuple8_tcr else g.ref_tuple8_tcr),
                tysA @ [ mkCompiledTupleTy g isStruct tysB ],
                g.knownWithoutNull
            )

    /// Convert from F# tuple types to .NET tuple types, but only the outermost level
    let mkOuterCompiledTupleTy g isStruct tupElemTys =
        let n = List.length tupElemTys

        if n < maxTuple then
            TType_app(mkCompiledTupleTyconRef g isStruct n, tupElemTys, g.knownWithoutNull)
        else
            let tysA, tysB = List.splitAfter goodTupleFields tupElemTys
            let tcref = (if isStruct then g.struct_tuple8_tcr else g.ref_tuple8_tcr)
            // In the case of an 8-tuple we add the Tuple<_> marker. For other sizes we keep the type
            // as a regular F# tuple type.
            match tysB with
            | [ tyB ] ->
                let marker =
                    TType_app(mkCompiledTupleTyconRef g isStruct 1, [ tyB ], g.knownWithoutNull)

                TType_app(tcref, tysA @ [ marker ], g.knownWithoutNull)
            | _ -> TType_app(tcref, tysA @ [ TType_tuple(mkTupInfo isStruct, tysB) ], g.knownWithoutNull)

[<AutoOpen>]
module internal TypeAbbreviations =

    //---------------------------------------------------------------------------
    // Remove inference equations and abbreviations from types
    //---------------------------------------------------------------------------

    let applyTyconAbbrev abbrevTy tycon tyargs =
        if isNil tyargs then
            abbrevTy
        else
            instType (mkTyconInst tycon tyargs) abbrevTy

    let reduceTyconAbbrev (tycon: Tycon) tyargs =
        let abbrev = tycon.TypeAbbrev

        match abbrev with
        | None -> invalidArg "tycon" "this type definition is not an abbreviation"
        | Some abbrevTy -> applyTyconAbbrev abbrevTy tycon tyargs

    let reduceTyconRefAbbrev (tcref: TyconRef) tyargs = reduceTyconAbbrev tcref.Deref tyargs

    let reduceTyconMeasureableOrProvided (g: TcGlobals) (tycon: Tycon) tyargs =
#if NO_TYPEPROVIDERS
        ignore g // otherwise g would be unused
#endif
        let repr = tycon.TypeReprInfo

        match repr with
        | TMeasureableRepr ty ->
            if isNil tyargs then
                ty
            else
                instType (mkTyconInst tycon tyargs) ty
#if !NO_TYPEPROVIDERS
        | TProvidedTypeRepr info when info.IsErased -> info.BaseTypeForErased(range0, g.obj_ty_withNulls)
#endif
        | _ -> invalidArg "tc" "this type definition is not a refinement"

    let reduceTyconRefMeasureableOrProvided (g: TcGlobals) (tcref: TyconRef) tyargs =
        reduceTyconMeasureableOrProvided g tcref.Deref tyargs

[<AutoOpen>]
module internal TypeDecomposition =

    let rec stripTyEqnsA g canShortcut ty =
        let ty = stripTyparEqnsAux KnownWithoutNull canShortcut ty

        match ty with
        | TType_app(tcref, tinst, nullness) ->
            let tycon = tcref.Deref

            match tycon.TypeAbbrev with
            | Some abbrevTy ->
                let reducedTy = applyTyconAbbrev abbrevTy tycon tinst
                let reducedTy2 = addNullnessToTy nullness reducedTy
                stripTyEqnsA g canShortcut reducedTy2
            | None ->
                // This is the point where we get to add additional conditional normalizing equations
                // into the type system. Such power!
                //
                // Add the equation byref<'T> = byref<'T, ByRefKinds.InOut> for when using sufficient FSharp.Core
                // See RFC FS-1053.md
                if
                    tyconRefEq g tcref g.byref_tcr
                    && g.byref2_tcr.CanDeref
                    && g.byrefkind_InOut_tcr.CanDeref
                then
                    mkByref2Ty g tinst[0] (TType_app(g.byrefkind_InOut_tcr, [], g.knownWithoutNull))

                // Add the equation double<1> = double for units of measure.
                elif tycon.IsMeasureableReprTycon && List.forall (isDimensionless g) tinst then
                    let reducedTy = reduceTyconMeasureableOrProvided g tycon tinst
                    let reducedTy2 = addNullnessToTy nullness reducedTy
                    stripTyEqnsA g canShortcut reducedTy2
                else
                    ty
        | ty -> ty

    let stripTyEqns g ty = stripTyEqnsA g false ty

    let evalTupInfoIsStruct aexpr =
        match aexpr with
        | TupInfo.Const b -> b

    let evalAnonInfoIsStruct (anonInfo: AnonRecdTypeInfo) = evalTupInfoIsStruct anonInfo.TupInfo

    /// This erases outermost occurrences of inference equations, type abbreviations, non-generated provided types
    /// and measurable types (float<_>).
    /// It also optionally erases all "compilation representations", i.e. function and
    /// tuple types, and also "nativeptr<'T> --> System.IntPtr"
    let rec stripTyEqnsAndErase eraseFuncAndTuple (g: TcGlobals) ty =
        let ty = stripTyEqns g ty

        match ty with
        | TType_app(tcref, args, nullness) ->
            let tycon = tcref.Deref

            if tycon.IsErased then
                let reducedTy = reduceTyconMeasureableOrProvided g tycon args
                let reducedTy2 = addNullnessToTy nullness reducedTy
                stripTyEqnsAndErase eraseFuncAndTuple g reducedTy2
            elif tyconRefEq g tcref g.nativeptr_tcr && eraseFuncAndTuple then
                // Regression fix (issue #7428): nativeptr<'T> erases to ilsigptr<'T>, not nativeint
                stripTyEqnsAndErase eraseFuncAndTuple g (TType_app(g.ilsigptr_tcr, args, nullness))
            else
                ty

        | TType_fun(domainTy, rangeTy, nullness) when eraseFuncAndTuple -> TType_app(g.fastFunc_tcr, [ domainTy; rangeTy ], nullness)

        | TType_tuple(tupInfo, l) when eraseFuncAndTuple -> mkCompiledTupleTy g (evalTupInfoIsStruct tupInfo) l

        | ty -> ty

    let stripTyEqnsAndMeasureEqns g ty = stripTyEqnsAndErase false g ty

    type Erasure =
        | EraseAll
        | EraseMeasures
        | EraseNone

    let stripTyEqnsWrtErasure erasureFlag g ty =
        match erasureFlag with
        | EraseAll -> stripTyEqnsAndErase true g ty
        | EraseMeasures -> stripTyEqnsAndErase false g ty
        | _ -> stripTyEqns g ty

    let rec stripExnEqns (eref: TyconRef) =
        let exnc = eref.Deref

        match exnc.ExceptionInfo with
        | TExnAbbrevRepr eref -> stripExnEqns eref
        | _ -> exnc

    let primDestForallTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_forall(tyvs, tau) -> (tyvs, tau)
        | _ -> failwith "primDestForallTy: not a forall type")

    let destFunTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_fun(domainTy, rangeTy, _) -> (domainTy, rangeTy)
        | _ -> failwith "destFunTy: not a function type")

    let destAnyTupleTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_tuple(tupInfo, l) -> tupInfo, l
        | _ -> failwith "destAnyTupleTy: not a tuple type")

    let destRefTupleTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_tuple(tupInfo, l) when not (evalTupInfoIsStruct tupInfo) -> l
        | _ -> failwith "destRefTupleTy: not a reference tuple type")

    let destStructTupleTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_tuple(tupInfo, l) when evalTupInfoIsStruct tupInfo -> l
        | _ -> failwith "destStructTupleTy: not a struct tuple type")

    let destTyparTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_var(v, _) -> v
        | _ -> failwith "destTyparTy: not a typar type")

    let destAnyParTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_var(v, _) -> v
        | TType_measure unt -> destUnitParMeasure g unt
        | _ -> failwith "destAnyParTy: not a typar or unpar type")

    let destMeasureTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_measure m -> m
        | _ -> failwith "destMeasureTy: not a unit-of-measure type")

    let destAnonRecdTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_anon(anonInfo, tys) -> anonInfo, tys
        | _ -> failwith "destAnonRecdTy: not an anonymous record type")

    let destStructAnonRecdTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_anon(anonInfo, tys) when evalAnonInfoIsStruct anonInfo -> tys
        | _ -> failwith "destAnonRecdTy: not a struct anonymous record type")

    let isFunTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_fun _ -> true
        | _ -> false)

    let isForallTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_forall _ -> true
        | _ -> false)

    let isAnyTupleTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_tuple _ -> true
        | _ -> false)

    let isRefTupleTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_tuple(tupInfo, _) -> not (evalTupInfoIsStruct tupInfo)
        | _ -> false)

    let isStructTupleTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_tuple(tupInfo, _) -> evalTupInfoIsStruct tupInfo
        | _ -> false)

    let isAnonRecdTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_anon _ -> true
        | _ -> false)

    let isStructAnonRecdTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_anon(anonInfo, _) -> evalAnonInfoIsStruct anonInfo
        | _ -> false)

    let isUnionTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tcref.IsUnionTycon
        | _ -> false)

    let isStructUnionTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tcref.IsUnionTycon && tcref.Deref.entity_flags.IsStructRecordOrUnionType
        | _ -> false)

    let isReprHiddenTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tcref.IsHiddenReprTycon
        | _ -> false)

    let isFSharpObjModelTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tcref.IsFSharpObjectModelTycon
        | _ -> false)

    let isRecdTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tcref.IsRecordTycon
        | _ -> false)

    let isFSharpStructOrEnumTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tcref.IsFSharpStructOrEnumTycon
        | _ -> false)

    let isFSharpEnumTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tcref.IsFSharpEnumTycon
        | _ -> false)

    let isTyparTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_var _ -> true
        | _ -> false)

    let isAnyParTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_var _ -> true
        | TType_measure unt -> isUnitParMeasure g unt
        | _ -> false)

    let isMeasureTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_measure _ -> true
        | _ -> false)

    let isProvenUnionCaseTy ty =
        match ty with
        | TType_ucase _ -> true
        | _ -> false

    let mkWoNullAppTy tcref tyargs =
        TType_app(tcref, tyargs, KnownWithoutNull)

    let mkProvenUnionCaseTy ucref tyargs = TType_ucase(ucref, tyargs)

    let isAppTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app _ -> true
        | _ -> false)

    let tryAppTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, tinst, _) -> ValueSome(tcref, tinst)
        | _ -> ValueNone)

    let destAppTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, tinst, _) -> tcref, tinst
        | _ -> failwith "destAppTy")

    let tcrefOfAppTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> tcref
        | _ -> failwith "tcrefOfAppTy")

    let argsOfAppTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(_, tinst, _) -> tinst
        | _ -> [])

    let tryDestTyparTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_var(v, _) -> ValueSome v
        | _ -> ValueNone)

    let tryDestFunTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_fun(domainTy, rangeTy, _) -> ValueSome(domainTy, rangeTy)
        | _ -> ValueNone)

    let tryTcrefOfAppTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, _, _) -> ValueSome tcref
        | _ -> ValueNone)

    let tryDestAnonRecdTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_anon(anonInfo, tys) -> ValueSome(anonInfo, tys)
        | _ -> ValueNone)

    let tryAnyParTy g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_var(v, _) -> ValueSome v
        | TType_measure unt when isUnitParMeasure g unt -> ValueSome(destUnitParMeasure g unt)
        | _ -> ValueNone)

    let tryAnyParTyOption g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_var(v, _) -> Some v
        | TType_measure unt when isUnitParMeasure g unt -> Some(destUnitParMeasure g unt)
        | _ -> None)

    [<return: Struct>]
    let (|AppTy|_|) g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_app(tcref, tinst, _) -> ValueSome(tcref, tinst)
        | _ -> ValueNone)

    [<return: Struct>]
    let (|RefTupleTy|_|) g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_tuple(tupInfo, tys) when not (evalTupInfoIsStruct tupInfo) -> ValueSome tys
        | _ -> ValueNone)

    [<return: Struct>]
    let (|FunTy|_|) g ty =
        ty
        |> stripTyEqns g
        |> (function
        | TType_fun(domainTy, rangeTy, _) -> ValueSome(domainTy, rangeTy)
        | _ -> ValueNone)

    let tryNiceEntityRefOfTy ty =
        let ty = stripTyparEqnsAux KnownWithoutNull false ty

        match ty with
        | TType_app(tcref, _, _) -> ValueSome tcref
        | TType_measure(Measure.Const(tyconRef = tcref)) -> ValueSome tcref
        | _ -> ValueNone

    let tryNiceEntityRefOfTyOption ty =
        let ty = stripTyparEqnsAux KnownWithoutNull false ty

        match ty with
        | TType_app(tcref, _, _) -> Some tcref
        | TType_measure(Measure.Const(tyconRef = tcref)) -> Some tcref
        | _ -> None

    let mkInstForAppTy g ty =
        match tryAppTy g ty with
        | ValueSome(tcref, tinst) -> mkTyconRefInst tcref tinst
        | _ -> []

    let domainOfFunTy g ty = fst (destFunTy g ty)
    let rangeOfFunTy g ty = snd (destFunTy g ty)

    let convertToTypeWithMetadataIfPossible g ty =
        if isAnyTupleTy g ty then
            let tupInfo, tupElemTys = destAnyTupleTy g ty
            mkOuterCompiledTupleTy g (evalTupInfoIsStruct tupInfo) tupElemTys
        elif isFunTy g ty then
            let a, b = destFunTy g ty
            mkWoNullAppTy g.fastFunc_tcr [ a; b ]
        else
            ty

    //---------------------------------------------------------------------------
    // TType modifications
    //---------------------------------------------------------------------------

    let stripMeasuresFromTy g ty =
        match ty with
        | TType_app(tcref, tinst, nullness) ->
            let tinstR = tinst |> List.filter (isMeasureTy g >> not)
            TType_app(tcref, tinstR, nullness)
        | _ -> ty

    let mkAnyTupledTy (g: TcGlobals) tupInfo tys =
        match tys with
        | [] -> g.unit_ty
        | [ h ] -> h
        | _ -> TType_tuple(tupInfo, tys)

    let mkAnyAnonRecdTy (_g: TcGlobals) anonInfo tys = TType_anon(anonInfo, tys)

    let mkRefTupledTy g tys = mkAnyTupledTy g tupInfoRef tys

    let mkRefTupledVarsTy g vs = mkRefTupledTy g (typesOfVals vs)

    let mkMethodTy g argTys retTy =
        mkIteratedFunTy g (List.map (mkRefTupledTy g) argTys) retTy

    let mkArrayType (g: TcGlobals) ty =
        TType_app(g.array_tcr_nice, [ ty ], g.knownWithoutNull)

    let mkByteArrayTy (g: TcGlobals) = mkArrayType g g.byte_ty

    let isQuotedExprTy g ty =
        match tryAppTy g ty with
        | ValueSome(tcref, _) -> tyconRefEq g tcref g.expr_tcr
        | _ -> false

    let destQuotedExprTy g ty =
        match tryAppTy g ty with
        | ValueSome(_, [ ty ]) -> ty
        | _ -> failwith "destQuotedExprTy"

    let mkQuotedExprTy (g: TcGlobals) ty =
        TType_app(g.expr_tcr, [ ty ], g.knownWithoutNull)

    let mkRawQuotedExprTy (g: TcGlobals) =
        TType_app(g.raw_expr_tcr, [], g.knownWithoutNull)

    let mkIEventType (g: TcGlobals) ty1 ty2 =
        TType_app(g.fslib_IEvent2_tcr, [ ty1; ty2 ], g.knownWithoutNull)

    let mkIObservableType (g: TcGlobals) ty1 =
        TType_app(g.tcref_IObservable, [ ty1 ], g.knownWithoutNull)

    let mkIObserverType (g: TcGlobals) ty1 =
        TType_app(g.tcref_IObserver, [ ty1 ], g.knownWithoutNull)

    let mkSeqTy (g: TcGlobals) ty = mkWoNullAppTy g.seq_tcr [ ty ]

    let mkIEnumeratorTy (g: TcGlobals) ty =
        mkWoNullAppTy g.tcref_System_Collections_Generic_IEnumerator [ ty ]

[<AutoOpen>]
module internal TypeEquivalence =

    //---------------------------------------------------------------------------
    // Equivalence of types up to alpha-equivalence
    //---------------------------------------------------------------------------

    [<NoEquality; NoComparison>]
    type TypeEquivEnv =
        {
            EquivTypars: TyparMap<TType>
            EquivTycons: TyconRefRemap
            NullnessMustEqual: bool
        }

    let private nullnessEqual anev (n1: Nullness) (n2: Nullness) =
        if anev.NullnessMustEqual then
            (n1.Evaluate() = NullnessInfo.WithNull) = (n2.Evaluate() = NullnessInfo.WithNull)
        else
            true

    // allocate a singleton
    let private typeEquivEnvEmpty =
        {
            EquivTypars = TyparMap.Empty
            EquivTycons = emptyTyconRefRemap
            NullnessMustEqual = false
        }

    let private typeEquivCheckNullness =
        { typeEquivEnvEmpty with
            NullnessMustEqual = true
        }

    type TypeEquivEnv with
        static member EmptyIgnoreNulls = typeEquivEnvEmpty

        static member EmptyWithNullChecks(g: TcGlobals) =
            if g.checkNullness then
                typeEquivCheckNullness
            else
                typeEquivEnvEmpty

        member aenv.BindTyparsToTypes tps1 tys2 =
            { aenv with
                EquivTypars =
                    (tps1, tys2, aenv.EquivTypars)
                    |||> List.foldBack2 (fun tp ty tpmap -> tpmap.Add(tp, ty))
            }

        member aenv.BindEquivTypars tps1 tps2 =
            aenv.BindTyparsToTypes tps1 (List.map mkTyparTy tps2)

        member aenv.FromTyparInst tpinst =
            let tps, tys = List.unzip tpinst
            aenv.BindTyparsToTypes tps tys

        member aenv.FromEquivTypars tps1 tps2 = aenv.BindEquivTypars tps1 tps2

        member anev.ResetEquiv =
            if anev.NullnessMustEqual then
                typeEquivCheckNullness
            else
                typeEquivEnvEmpty

    let rec traitsAEquivAux erasureFlag g aenv traitInfo1 traitInfo2 =
        let (TTrait(tys1, nm, mf1, argTys, retTy, _, _)) = traitInfo1
        let (TTrait(tys2, nm2, mf2, argTys2, retTy2, _, _)) = traitInfo2

        mf1.IsInstance = mf2.IsInstance
        && nm = nm2
        && ListSet.equals (typeAEquivAux erasureFlag g aenv) tys1 tys2
        && returnTypesAEquivAux erasureFlag g aenv retTy retTy2
        && List.lengthsEqAndForall2 (typeAEquivAux erasureFlag g aenv) argTys argTys2

    and traitKeysAEquivAux erasureFlag g aenv witnessInfo1 witnessInfo2 =
        let (TraitWitnessInfo(tys1, nm, mf1, argTys, retTy)) = witnessInfo1
        let (TraitWitnessInfo(tys2, nm2, mf2, argTys2, retTy2)) = witnessInfo2

        mf1.IsInstance = mf2.IsInstance
        && nm = nm2
        && ListSet.equals (typeAEquivAux erasureFlag g aenv) tys1 tys2
        && returnTypesAEquivAux erasureFlag g aenv retTy retTy2
        && List.lengthsEqAndForall2 (typeAEquivAux erasureFlag g aenv) argTys argTys2

    and returnTypesAEquivAux erasureFlag g aenv retTy retTy2 =
        match retTy, retTy2 with
        | None, None -> true
        | Some ty1, Some ty2 -> typeAEquivAux erasureFlag g aenv ty1 ty2
        | _ -> false

    and typarConstraintsAEquivAux erasureFlag g aenv tpc1 tpc2 =
        match tpc1, tpc2 with
        | TyparConstraint.CoercesTo(tgtTy1, _), TyparConstraint.CoercesTo(tgtTy2, _) -> typeAEquivAux erasureFlag g aenv tgtTy1 tgtTy2

        | TyparConstraint.MayResolveMember(trait1, _), TyparConstraint.MayResolveMember(trait2, _) ->
            traitsAEquivAux erasureFlag g aenv trait1 trait2

        | TyparConstraint.DefaultsTo(_, dfltTy1, _), TyparConstraint.DefaultsTo(_, dfltTy2, _) ->
            typeAEquivAux erasureFlag g aenv dfltTy1 dfltTy2

        | TyparConstraint.IsEnum(underlyingTy1, _), TyparConstraint.IsEnum(underlyingTy2, _) ->
            typeAEquivAux erasureFlag g aenv underlyingTy1 underlyingTy2

        | TyparConstraint.IsDelegate(argTys1, retTy1, _), TyparConstraint.IsDelegate(argTys2, retTy2, _) ->
            typeAEquivAux erasureFlag g aenv argTys1 argTys2
            && typeAEquivAux erasureFlag g aenv retTy1 retTy2

        | TyparConstraint.SimpleChoice(tys1, _), TyparConstraint.SimpleChoice(tys2, _) ->
            ListSet.equals (typeAEquivAux erasureFlag g aenv) tys1 tys2

        | TyparConstraint.SupportsComparison _, TyparConstraint.SupportsComparison _
        | TyparConstraint.SupportsEquality _, TyparConstraint.SupportsEquality _
        | TyparConstraint.SupportsNull _, TyparConstraint.SupportsNull _
        | TyparConstraint.NotSupportsNull _, TyparConstraint.NotSupportsNull _
        | TyparConstraint.IsNonNullableStruct _, TyparConstraint.IsNonNullableStruct _
        | TyparConstraint.IsReferenceType _, TyparConstraint.IsReferenceType _
        | TyparConstraint.IsUnmanaged _, TyparConstraint.IsUnmanaged _
        | TyparConstraint.AllowsRefStruct _, TyparConstraint.AllowsRefStruct _
        | TyparConstraint.RequiresDefaultConstructor _, TyparConstraint.RequiresDefaultConstructor _ -> true
        | _ -> false

    and typarConstraintSetsAEquivAux erasureFlag g aenv (tp1: Typar) (tp2: Typar) =
        tp1.StaticReq = tp2.StaticReq
        && ListSet.equals (typarConstraintsAEquivAux erasureFlag g aenv) tp1.Constraints tp2.Constraints

    and typarsAEquivAux erasureFlag g (aenv: TypeEquivEnv) tps1 tps2 =
        List.length tps1 = List.length tps2
        && let aenv = aenv.BindEquivTypars tps1 tps2 in
           List.forall2 (typarConstraintSetsAEquivAux erasureFlag g aenv) tps1 tps2

    and tcrefAEquiv g aenv tcref1 tcref2 =
        tyconRefEq g tcref1 tcref2
        || (match aenv.EquivTycons.TryFind tcref1 with
            | Some v -> tyconRefEq g v tcref2
            | None -> false)

    and typeAEquivAux erasureFlag g aenv ty1 ty2 =
        let ty1 = stripTyEqnsWrtErasure erasureFlag g ty1
        let ty2 = stripTyEqnsWrtErasure erasureFlag g ty2

        match ty1, ty2 with
        | TType_forall(tps1, rty1), TType_forall(tps2, retTy2) ->
            typarsAEquivAux erasureFlag g aenv tps1 tps2
            && typeAEquivAux erasureFlag g (aenv.BindEquivTypars tps1 tps2) rty1 retTy2

        | TType_var(tp1, n1), TType_var(tp2, n2) when typarEq tp1 tp2 -> nullnessEqual aenv n1 n2

        | TType_var(tp1, n1), _ ->
            match aenv.EquivTypars.TryFind tp1 with
            | Some tpTy1 ->
                let tpTy1 =
                    if (nullnessEqual aenv n1 g.knownWithoutNull) then
                        tpTy1
                    else
                        addNullnessToTy n1 tpTy1

                typeAEquivAux erasureFlag g aenv.ResetEquiv tpTy1 ty2
            | None -> false

        | TType_app(tcref1, tinst1, n1), TType_app(tcref2, tinst2, n2) ->
            nullnessEqual aenv n1 n2
            && tcrefAEquiv g aenv tcref1 tcref2
            && typesAEquivAux erasureFlag g aenv tinst1 tinst2

        | TType_ucase(UnionCaseRef(tcref1, ucase1), tinst1), TType_ucase(UnionCaseRef(tcref2, ucase2), tinst2) ->
            ucase1 = ucase2
            && tcrefAEquiv g aenv tcref1 tcref2
            && typesAEquivAux erasureFlag g aenv tinst1 tinst2

        | TType_tuple(tupInfo1, l1), TType_tuple(tupInfo2, l2) ->
            structnessAEquiv tupInfo1 tupInfo2 && typesAEquivAux erasureFlag g aenv l1 l2

        | TType_fun(domainTy1, rangeTy1, n1), TType_fun(domainTy2, rangeTy2, n2) ->
            nullnessEqual aenv n1 n2
            && typeAEquivAux erasureFlag g aenv domainTy1 domainTy2
            && typeAEquivAux erasureFlag g aenv rangeTy1 rangeTy2

        | TType_anon(anonInfo1, l1), TType_anon(anonInfo2, l2) ->
            anonInfoEquiv anonInfo1 anonInfo2 && typesAEquivAux erasureFlag g aenv l1 l2

        | TType_measure m1, TType_measure m2 ->
            match erasureFlag with
            | EraseNone -> measureAEquiv g aenv m1 m2
            | _ -> true

        | _ -> false

    and anonInfoEquiv (anonInfo1: AnonRecdTypeInfo) (anonInfo2: AnonRecdTypeInfo) =
        ccuEq anonInfo1.Assembly anonInfo2.Assembly
        && structnessAEquiv anonInfo1.TupInfo anonInfo2.TupInfo
        && anonInfo1.SortedNames = anonInfo2.SortedNames

    and structnessAEquiv un1 un2 =
        match un1, un2 with
        | TupInfo.Const b1, TupInfo.Const b2 -> (b1 = b2)

    and measureAEquiv g aenv un1 un2 =
        let vars1 = ListMeasureVarOccs un1

        let trans tp1 =
            match aenv.EquivTypars.TryGetValue tp1 with
            | true, etv -> destAnyParTy g etv
            | false, _ -> tp1

        let remapTyconRef tcref =
            match aenv.EquivTycons.TryGetValue tcref with
            | true, tval -> tval
            | false, _ -> tcref

        let vars1R = List.map trans vars1
        let vars2 = ListSet.subtract typarEq (ListMeasureVarOccs un2) vars1R
        let cons1 = ListMeasureConOccsAfterRemapping g remapTyconRef un1
        let cons2 = ListMeasureConOccsAfterRemapping g remapTyconRef un2

        vars1
        |> List.forall (fun v -> MeasureVarExponent v un1 = MeasureVarExponent (trans v) un2)
        && vars2
           |> List.forall (fun v -> MeasureVarExponent v un1 = MeasureVarExponent v un2)
        && (cons1 @ cons2)
           |> List.forall (fun c ->
               MeasureConExponentAfterRemapping g remapTyconRef c un1 = MeasureConExponentAfterRemapping g remapTyconRef c un2)

    and typesAEquivAux erasureFlag g aenv l1 l2 =
        List.lengthsEqAndForall2 (typeAEquivAux erasureFlag g aenv) l1 l2

    and typeEquivAux erasureFlag g ty1 ty2 =
        typeAEquivAux erasureFlag g TypeEquivEnv.EmptyIgnoreNulls ty1 ty2

    let typeAEquiv g aenv ty1 ty2 = typeAEquivAux EraseNone g aenv ty1 ty2

    let typeEquiv g ty1 ty2 = typeEquivAux EraseNone g ty1 ty2

    let traitsAEquiv g aenv t1 t2 = traitsAEquivAux EraseNone g aenv t1 t2

    let traitKeysAEquiv g aenv t1 t2 =
        traitKeysAEquivAux EraseNone g aenv t1 t2

    let typarConstraintsAEquiv g aenv c1 c2 =
        typarConstraintsAEquivAux EraseNone g aenv c1 c2

    let typarsAEquiv g aenv d1 d2 = typarsAEquivAux EraseNone g aenv d1 d2

    let isConstraintAllowedAsExtra cx =
        match cx with
        | TyparConstraint.NotSupportsNull _ -> true
        | _ -> false

    let typarsAEquivWithFilter g (aenv: TypeEquivEnv) (reqTypars: Typars) (declaredTypars: Typars) allowExtraInDecl =
        List.length reqTypars = List.length declaredTypars
        && let aenv = aenv.BindEquivTypars reqTypars declaredTypars in
           let cxEquiv = typarConstraintsAEquivAux EraseNone g aenv in

           (reqTypars, declaredTypars)
           ||> List.forall2 (fun reqTp declTp ->
               reqTp.StaticReq = declTp.StaticReq
               && ListSet.isSubsetOf cxEquiv reqTp.Constraints declTp.Constraints
               && declTp.Constraints
                  |> List.forall (fun declCx ->
                      allowExtraInDecl declCx
                      || reqTp.Constraints |> List.exists (fun reqCx -> cxEquiv reqCx declCx)))

    let typarsAEquivWithAddedNotNullConstraintsAllowed g aenv reqTypars declaredTypars =
        typarsAEquivWithFilter g aenv reqTypars declaredTypars isConstraintAllowedAsExtra

    let returnTypesAEquiv g aenv t1 t2 =
        returnTypesAEquivAux EraseNone g aenv t1 t2

    let measureEquiv g m1 m2 =
        measureAEquiv g TypeEquivEnv.EmptyIgnoreNulls m1 m2

    /// An immutable mapping from witnesses to some data.
    ///
    /// Note: this uses an immutable HashMap/Dictionary with an IEqualityComparer that captures TcGlobals, see EmptyTraitWitnessInfoHashMap
    type TraitWitnessInfoHashMap<'T> = ImmutableDictionary<TraitWitnessInfo, 'T>

    /// Create an empty immutable mapping from witnesses to some data
    let EmptyTraitWitnessInfoHashMap g : TraitWitnessInfoHashMap<'T> =
        ImmutableDictionary.Create(
            { new IEqualityComparer<_> with
                member _.Equals(a, b) =
                    nullSafeEquality a b (fun a b -> traitKeysAEquiv g TypeEquivEnv.EmptyIgnoreNulls a b)

                member _.GetHashCode(a) = hash a.MemberName
            }
        )
