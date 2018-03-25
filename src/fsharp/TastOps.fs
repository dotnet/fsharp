// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Defines derived expression manipulation and construction functions.
module internal Microsoft.FSharp.Compiler.Tastops

open System.Collections.Generic 
open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Rational
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.Layout.TaggedTextOps
open Microsoft.FSharp.Compiler.PrettyNaming
#if !NO_EXTENSIONTYPING
open Microsoft.FSharp.Compiler.ExtensionTyping
#endif

//---------------------------------------------------------------------------
// Basic data structures
//---------------------------------------------------------------------------

[<NoEquality; NoComparison>]
type TyparMap<'T> = 
    | TPMap of StampMap<'T>

    member tm.Item 
        with get (v: Typar) = 
            let (TPMap m) = tm
            m.[v.Stamp]

    member tm.ContainsKey (v: Typar) = 
        let (TPMap m) = tm
        m.ContainsKey(v.Stamp)

    member tm.Add (v: Typar, x) = 
        let (TPMap m) = tm
        TPMap (m.Add(v.Stamp, x))

    static member Empty : TyparMap<'T> = TPMap Map.empty

[<NoEquality; NoComparison; Sealed>]
type TyconRefMap<'T>(imap: StampMap<'T>) =
    member m.Item with get (v: TyconRef) = imap.[v.Stamp]
    member m.TryFind (v: TyconRef) = imap.TryFind v.Stamp 
    member m.ContainsKey (v: TyconRef) =  imap.ContainsKey v.Stamp 
    member m.Add (v: TyconRef) x = TyconRefMap (imap.Add (v.Stamp, x))
    member m.Remove (v: TyconRef) = TyconRefMap (imap.Remove v.Stamp)
    member m.IsEmpty = imap.IsEmpty

    static member Empty : TyconRefMap<'T> = TyconRefMap Map.empty
    static member OfList vs = (vs, TyconRefMap<'T>.Empty) ||> List.foldBack (fun (x, y) acc -> acc.Add x y) 

[<Struct>]
[<NoEquality; NoComparison>]
type ValMap<'T>(imap: StampMap<'T>) = 
     
    member m.Contents = imap
    member m.Item with get (v:Val) = imap.[v.Stamp]
    member m.TryFind (v: Val) = imap.TryFind v.Stamp 
    member m.ContainsVal (v: Val) = imap.ContainsKey v.Stamp 
    member m.Add (v: Val) x = ValMap (imap.Add(v.Stamp, x))
    member m.Remove (v: Val) = ValMap (imap.Remove(v.Stamp))
    static member Empty = ValMap<'T> Map.empty
    member m.IsEmpty = imap.IsEmpty
    static member OfList vs = (vs, ValMap<'T>.Empty) ||> List.foldBack (fun (x, y) acc -> acc.Add x y) 

//--------------------------------------------------------------------------
// renamings
//--------------------------------------------------------------------------

type TyparInst = (Typar * TType) list

type TyconRefRemap = TyconRefMap<TyconRef>
type ValRemap = ValMap<ValRef>

let emptyTyconRefRemap : TyconRefRemap = TyconRefMap<_>.Empty
let emptyTyparInst = ([] : TyparInst)

[<NoEquality; NoComparison>]
type Remap =
    { tpinst : TyparInst
      valRemap: ValRemap
      tyconRefRemap : TyconRefRemap
      removeTraitSolutions: bool }

let emptyRemap = 
    { tpinst        = emptyTyparInst;
      tyconRefRemap = emptyTyconRefRemap
      valRemap      = ValMap.Empty
      removeTraitSolutions = false }

type Remap with 
    static member Empty = emptyRemap

//--------------------------------------------------------------------------
// Substitute for type variables and remap type constructors 
//--------------------------------------------------------------------------

let addTyconRefRemap tcref1 tcref2 tmenv = 
    { tmenv with tyconRefRemap = tmenv.tyconRefRemap.Add tcref1 tcref2 }

let isRemapEmpty remap = 
    isNil remap.tpinst && 
    remap.tyconRefRemap.IsEmpty && 
    remap.valRemap.IsEmpty 

let rec instTyparRef tpinst ty tp  =
    match tpinst with 
    | [] -> ty
    | (tp', ty')::t -> 
        if typarEq tp tp' then ty' 
        else instTyparRef t ty tp

let instMeasureTyparRef tpinst unt (tp:Typar)  =
   match tp.Kind with 
   | TyparKind.Measure ->
        let rec loop tpinst = 
            match tpinst with 
            | [] -> unt
            | (tp', ty')::t -> 
                if typarEq tp tp' then 
                    match ty' with 
                    | TType_measure unt -> unt
                    | _ -> failwith "instMeasureTyparRef incorrect kind"
                else
                    loop t
        loop tpinst
   | _ -> failwith "instMeasureTyparRef: kind=Type"

let remapTyconRef (tcmap: TyconRefMap<_>) tcr  =
    match tcmap.TryFind tcr with 
    | Some tcr ->  tcr
    | None -> tcr

let remapUnionCaseRef tcmap (UCRef(tcref, nm)) = UCRef(remapTyconRef tcmap tcref, nm)
let remapRecdFieldRef tcmap (RFRef(tcref, nm)) = RFRef(remapTyconRef tcmap tcref, nm)

let mkTyparInst (typars: Typars) tyargs =  
#if CHECKED
    if List.length typars <> List.length tyargs then
      failwith ("mkTyparInst: invalid type" + (sprintf " %d <> %d" (List.length typars) (List.length tyargs)));
#endif
    (List.zip typars tyargs : TyparInst)

let generalizeTypar tp = mkTyparTy tp
let generalizeTypars tps = List.map generalizeTypar tps

let rec remapTypeAux (tyenv : Remap) (ty:TType) =
  let ty = stripTyparEqns ty
  match ty with
  | TType_var tp as ty       -> instTyparRef tyenv.tpinst ty tp
  | TType_app (tcr, tinst) as ty -> 
      match tyenv.tyconRefRemap.TryFind tcr with 
      | Some tcr' ->  TType_app (tcr', remapTypesAux tyenv tinst)
      | None -> 
          match tinst with 
          | [] -> ty  // optimization to avoid re-allocation of TType_app node in the common case 
          | _ -> 
              // avoid reallocation on idempotent 
              let tinst' = remapTypesAux tyenv tinst
              if tinst === tinst' then ty else 
              TType_app (tcr, tinst')

  | TType_ucase (UCRef(tcr, n), tinst) -> 
      match tyenv.tyconRefRemap.TryFind tcr with 
      | Some tcr' ->  TType_ucase (UCRef(tcr', n), remapTypesAux tyenv tinst)
      | None -> TType_ucase (UCRef(tcr, n), remapTypesAux tyenv tinst)

  | TType_tuple (tupInfo, l)  as ty -> 
      let tupInfo' = remapTupInfoAux tyenv tupInfo
      let l' = remapTypesAux tyenv l
      if tupInfo === tupInfo' && l === l' then ty else  
      TType_tuple (tupInfo', l')

  | TType_fun (d, r) as ty      -> 
      let d' = remapTypeAux tyenv d
      let r' = remapTypeAux tyenv r
      if d === d' && r === r' then ty else
      TType_fun (d', r')

  | TType_forall (tps, ty) -> 
      let tps', tyenv = copyAndRemapAndBindTypars tyenv tps
      TType_forall (tps', remapTypeAux tyenv ty)

  | TType_measure unt -> 
      TType_measure (remapMeasureAux tyenv unt)


and remapMeasureAux tyenv unt =
    match unt with
    | Measure.One -> unt
    | Measure.Con tcr ->
        match tyenv.tyconRefRemap.TryFind tcr with 
        | Some tcr ->  Measure.Con tcr
        | None -> unt
    | Measure.Prod(u1, u2) -> Measure.Prod(remapMeasureAux tyenv u1, remapMeasureAux tyenv u2)
    | Measure.RationalPower(u, q) -> Measure.RationalPower(remapMeasureAux tyenv u, q)
    | Measure.Inv u -> Measure.Inv(remapMeasureAux tyenv u)
    | Measure.Var tp as unt -> 
      match tp.Solution with
       | None -> 
          if ListAssoc.containsKey typarEq tp tyenv.tpinst then 
              match ListAssoc.find typarEq tp tyenv.tpinst with 
              | TType_measure unt -> unt
              | _ -> failwith "remapMeasureAux: incorrect kinds"
          else unt
       | Some (TType_measure unt) -> remapMeasureAux tyenv unt
       | Some ty -> failwithf "incorrect kinds: %A" ty

and remapTupInfoAux _tyenv unt =
    match unt with
    | TupInfo.Const _ -> unt

and remapTypesAux tyenv types = List.mapq (remapTypeAux tyenv) types
and remapTyparConstraintsAux tyenv cs =
   cs |>  List.choose (fun x -> 
         match x with 
         | TyparConstraint.CoercesTo(ty, m) -> 
             Some(TyparConstraint.CoercesTo (remapTypeAux tyenv ty, m))
         | TyparConstraint.MayResolveMember(traitInfo, m) -> 
             Some(TyparConstraint.MayResolveMember (remapTraitAux tyenv traitInfo, m))
         | TyparConstraint.DefaultsTo(priority, ty, m) -> Some(TyparConstraint.DefaultsTo(priority, remapTypeAux tyenv ty, m))
         | TyparConstraint.IsEnum(uty, m) -> 
             Some(TyparConstraint.IsEnum(remapTypeAux tyenv uty, m))
         | TyparConstraint.IsDelegate(uty1, uty2, m) -> 
             Some(TyparConstraint.IsDelegate(remapTypeAux tyenv uty1, remapTypeAux tyenv uty2, m))
         | TyparConstraint.SimpleChoice(tys, m) -> Some(TyparConstraint.SimpleChoice(remapTypesAux tyenv tys, m))
         | TyparConstraint.SupportsComparison  _ 
         | TyparConstraint.SupportsEquality  _ 
         | TyparConstraint.SupportsNull _ 
         | TyparConstraint.IsUnmanaged _ 
         | TyparConstraint.IsNonNullableStruct _ 
         | TyparConstraint.IsReferenceType _ 
         | TyparConstraint.RequiresDefaultConstructor _ -> Some(x))

and remapTraitAux tyenv (TTrait(typs, nm, mf, argtys, rty, slnCell)) =
    let slnCell = 
        match !slnCell with 
        | None -> None
        | _ when tyenv.removeTraitSolutions -> None
        | Some sln -> 
            let sln = 
                match sln with 
                | ILMethSln(typ, extOpt, ilMethRef, minst) ->
                     ILMethSln(remapTypeAux tyenv typ, extOpt, ilMethRef, remapTypesAux tyenv minst)  
                | FSMethSln(typ, vref, minst) ->
                     FSMethSln(remapTypeAux tyenv typ, remapValRef tyenv vref, remapTypesAux tyenv minst)  
                | FSRecdFieldSln(tinst, rfref, isSet) ->
                     FSRecdFieldSln(remapTypesAux tyenv tinst, remapRecdFieldRef tyenv.tyconRefRemap rfref, isSet)  
                | BuiltInSln -> 
                     BuiltInSln
                | ClosedExprSln e -> 
                     ClosedExprSln e // no need to remap because it is a closed expression, referring only to external types
            Some sln
    // Note: we reallocate a new solution cell on every traversal of a trait constraint
    // This feels incorrect for trait constraints that are quantified: it seems we should have 
    // formal binders for trait constraints when they are quantified, just as
    // we have formal binders for type variables.
    //
    // The danger here is that a solution for one syntactic occurrence of a trait constraint won't
    // be propagated to other, "linked" solutions. However trait constraints don't appear in any algebra
    // in the same way as types
    TTrait(remapTypesAux tyenv typs, nm, mf, remapTypesAux tyenv argtys, Option.map (remapTypeAux tyenv) rty, ref slnCell)

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
      let tps' = copyTypars tps
      let tyenv = { tyenv with tpinst = bindTypars tps (generalizeTypars tps') tyenv.tpinst } 
      (tps, tps') ||> List.iter2 (fun tporig tp -> 
         tp.FixupConstraints (remapTyparConstraintsAux tyenv  tporig.Constraints)
         tp.typar_attribs  <- tporig.typar_attribs |> remapAttrib)
      tps', tyenv

// copies bound typars, extends tpinst 
and copyAndRemapAndBindTypars tyenv tps =
    copyAndRemapAndBindTyparsFull (fun _ -> []) tyenv tps

and remapValLinkage tyenv (vlink: ValLinkageFullKey) = 
    let tyOpt = vlink.TypeForLinkage
    let tyOpt' = 
        match tyOpt with 
        | None -> tyOpt 
        | Some ty -> 
            let ty' = remapTypeAux tyenv ty
            if ty === ty' then tyOpt else
            Some ty'
    if tyOpt === tyOpt' then vlink else
    ValLinkageFullKey(vlink.PartialKey, tyOpt')

and remapNonLocalValRef tyenv (nlvref:NonLocalValOrMemberRef) = 
    let eref = nlvref.EnclosingEntity
    let eref' = remapTyconRef tyenv.tyconRefRemap eref
    let vlink = nlvref.ItemKey
    let vlink' = remapValLinkage tyenv vlink
    if eref === eref' && vlink === vlink' then nlvref else
    { EnclosingEntity = eref'
      ItemKey = vlink'  }

and remapValRef tmenv (vref: ValRef) = 
    match tmenv.valRemap.TryFind vref.Deref  with 
    | None -> 
        if vref.IsLocalRef then vref else 
        let nlvref = vref.nlr
        let nlvref' = remapNonLocalValRef tmenv nlvref
        if nlvref === nlvref' then vref else
        VRefNonLocal nlvref'
    | Some res -> 
        res

let remapType tyenv x =
    if isRemapEmpty tyenv then x else
    remapTypeAux tyenv x

let remapTypes tyenv x = 
    if isRemapEmpty tyenv then x else 
    remapTypesAux tyenv x

/// Use this one for any type that may be a forall type where the type variables may contain attributes 
/// Logically speaking this is mutually recursive with remapAttrib defined much later in this file, 
/// because types may contain forall types that contain attributes, which need to be remapped. 
/// We currently break the recursion by passing in remapAttrib as a function parameter. 
/// Use this one for any type that may be a forall type where the type variables may contain attributes 
let remapTypeFull remapAttrib tyenv ty =
    if isRemapEmpty tyenv then ty else 
    match stripTyparEqns ty with
    | TType_forall(tps, tau) -> 
        let tps', tyenvinner = copyAndRemapAndBindTyparsFull remapAttrib tyenv tps
        TType_forall(tps', remapType tyenvinner tau)
    | _ -> 
        remapType tyenv ty

let remapParam tyenv (TSlotParam(nm, typ, fl1, fl2, fl3, attribs) as x) = 
    if isRemapEmpty tyenv then x else 
    TSlotParam(nm, remapTypeAux tyenv typ, fl1, fl2, fl3, attribs) 

let remapSlotSig remapAttrib tyenv (TSlotSig(nm, typ, ctps, methTypars, paraml, rty) as x) =
    if isRemapEmpty tyenv then x else 
    let typ' = remapTypeAux tyenv typ
    let ctps', tyenvinner = copyAndRemapAndBindTyparsFull remapAttrib tyenv ctps
    let methTypars', tyenvinner = copyAndRemapAndBindTyparsFull remapAttrib tyenvinner methTypars
    TSlotSig(nm, typ', ctps', methTypars', List.mapSquared (remapParam tyenvinner) paraml, Option.map (remapTypeAux tyenvinner) rty) 

let mkInstRemap tpinst = 
    { tyconRefRemap = emptyTyconRefRemap
      tpinst        = tpinst
      valRemap      = ValMap.Empty
      removeTraitSolutions = false }

// entry points for "typar -> TType" instantiation 
let instType              tpinst x = if isNil tpinst then x else remapTypeAux  (mkInstRemap tpinst) x
let instTypes             tpinst x = if isNil tpinst then x else remapTypesAux (mkInstRemap tpinst) x
let instTrait             tpinst x = if isNil tpinst then x else remapTraitAux (mkInstRemap tpinst) x
let instTyparConstraints tpinst x = if isNil tpinst then x else remapTyparConstraintsAux (mkInstRemap tpinst) x
let instSlotSig tpinst ss = remapSlotSig (fun _ -> []) (mkInstRemap tpinst) ss
let copySlotSig ss = remapSlotSig (fun _ -> []) Remap.Empty ss


let mkTyparToTyparRenaming tpsOrig tps = 
    let tinst = generalizeTypars tps
    mkTyparInst tpsOrig tinst, tinst

let mkTyconInst (tycon:Tycon) tinst = mkTyparInst tycon.TyparsNoRange tinst
let mkTyconRefInst (tcref:TyconRef) tinst = mkTyconInst tcref.Deref tinst

//---------------------------------------------------------------------------
// Basic equalites
//---------------------------------------------------------------------------

let tyconRefEq (g:TcGlobals) tcref1 tcref2 = primEntityRefEq g.compilingFslib g.fslibCcu tcref1 tcref2
let valRefEq (g:TcGlobals) vref1 vref2 = primValRefEq g.compilingFslib g.fslibCcu vref1 vref2

//---------------------------------------------------------------------------
// Remove inference equations and abbreviations from units
//---------------------------------------------------------------------------

let reduceTyconRefAbbrevMeasureable (tcref:TyconRef) = 
    let abbrev = tcref.TypeAbbrev
    match abbrev with 
    | Some (TType_measure ms) -> ms
    | _ -> invalidArg "tcref" "not a measure abbreviation, or incorrect kind"

let rec stripUnitEqnsFromMeasureAux canShortcut unt = 
    match stripUnitEqnsAux canShortcut unt with 
    | Measure.Con tcref when tcref.IsTypeAbbrev  ->  
        stripUnitEqnsFromMeasureAux canShortcut (reduceTyconRefAbbrevMeasureable tcref) 
    | m -> m

let stripUnitEqnsFromMeasure m = stripUnitEqnsFromMeasureAux false m

//---------------------------------------------------------------------------
// Basic unit stuff
//---------------------------------------------------------------------------

/// What is the contribution of unit-of-measure constant ucref to unit-of-measure expression measure? 
let rec MeasureExprConExponent g abbrev ucref unt =
    match (if abbrev then stripUnitEqnsFromMeasure unt else stripUnitEqns unt) with
    | Measure.Con ucref' -> if tyconRefEq g ucref' ucref then OneRational else ZeroRational
    | Measure.Inv unt' -> NegRational(MeasureExprConExponent g abbrev ucref unt')
    | Measure.Prod(unt1, unt2) -> AddRational(MeasureExprConExponent g abbrev ucref unt1) (MeasureExprConExponent g abbrev ucref unt2)
    | Measure.RationalPower(unt', q) -> MulRational (MeasureExprConExponent g abbrev ucref unt') q
    | _ -> ZeroRational

/// What is the contribution of unit-of-measure constant ucref to unit-of-measure expression measure
/// after remapping tycons? 
let rec MeasureConExponentAfterRemapping g r ucref unt =
    match stripUnitEqnsFromMeasure unt with
    | Measure.Con ucref' -> if tyconRefEq g (r ucref') ucref then OneRational else ZeroRational
    | Measure.Inv unt' -> NegRational(MeasureConExponentAfterRemapping g r ucref unt')
    | Measure.Prod(unt1, unt2) -> AddRational(MeasureConExponentAfterRemapping g r ucref unt1) (MeasureConExponentAfterRemapping g r ucref unt2)
    | Measure.RationalPower(unt', q) -> MulRational (MeasureConExponentAfterRemapping g r ucref unt') q
    | _ -> ZeroRational

/// What is the contribution of unit-of-measure variable tp to unit-of-measure expression unt? 
let rec MeasureVarExponent tp unt =
    match stripUnitEqnsFromMeasure unt with
    | Measure.Var tp' -> if typarEq tp tp' then OneRational else ZeroRational
    | Measure.Inv unt' -> NegRational(MeasureVarExponent tp unt')
    | Measure.Prod(unt1, unt2) -> AddRational(MeasureVarExponent tp unt1) (MeasureVarExponent tp unt2)
    | Measure.RationalPower(unt', q) -> MulRational (MeasureVarExponent tp unt') q
    | _ -> ZeroRational

/// List the *literal* occurrences of unit variables in a unit expression, without repeats  
let ListMeasureVarOccs unt =
    let rec gather acc unt =  
        match stripUnitEqnsFromMeasure unt with
        | Measure.Var tp -> if List.exists (typarEq tp) acc then acc else tp::acc
        | Measure.Prod(unt1, unt2) -> gather (gather acc unt1) unt2
        | Measure.RationalPower(unt', _) -> gather acc unt'
        | Measure.Inv unt' -> gather acc unt'
        | _ -> acc   
    gather [] unt

/// List the *observable* occurrences of unit variables in a unit expression, without repeats, paired with their non-zero exponents
let ListMeasureVarOccsWithNonZeroExponents untexpr =
    let rec gather acc unt =  
        match stripUnitEqnsFromMeasure unt with
        | Measure.Var tp -> 
            if List.exists (fun (tp', _) -> typarEq tp tp') acc then acc 
            else 
                let e = MeasureVarExponent tp untexpr
                if e = ZeroRational then acc else (tp, e)::acc
        | Measure.Prod(unt1, unt2) -> gather (gather acc unt1) unt2
        | Measure.Inv unt' -> gather acc unt'
        | Measure.RationalPower(unt', _) -> gather acc unt'
        | _ -> acc   
    gather [] untexpr

/// List the *observable* occurrences of unit constants in a unit expression, without repeats, paired with their non-zero exponents
let ListMeasureConOccsWithNonZeroExponents g eraseAbbrevs untexpr =
    let rec gather acc unt =  
        match (if eraseAbbrevs then stripUnitEqnsFromMeasure unt else stripUnitEqns unt) with
        | Measure.Con c -> 
            if List.exists (fun (c', _) -> tyconRefEq g c c') acc then acc else 
            let e = MeasureExprConExponent g eraseAbbrevs c untexpr
            if e = ZeroRational then acc else (c, e)::acc
        | Measure.Prod(unt1, unt2) -> gather (gather acc unt1) unt2
        | Measure.Inv unt' -> gather acc unt'
        | Measure.RationalPower(unt', _) -> gather acc unt'
        | _ -> acc  
    gather [] untexpr

/// List the *literal* occurrences of unit constants in a unit expression, without repeats, 
/// and after applying a remapping function r to tycons
let ListMeasureConOccsAfterRemapping g r unt =
    let rec gather acc unt =  
        match stripUnitEqnsFromMeasure unt with
        | Measure.Con c -> if List.exists (tyconRefEq g (r c)) acc then acc else r c::acc
        | Measure.Prod(unt1, unt2) -> gather (gather acc unt1) unt2
        | Measure.RationalPower(unt', _) -> gather acc unt'
        | Measure.Inv unt' -> gather acc unt'
        | _ -> acc
   
    gather [] unt

/// Construct a measure expression representing the n'th power of a measure
let MeasurePower u n = 
    if n = 1 then u
    elif n = 0 then Measure.One
    else Measure.RationalPower (u, intToRational n)

let MeasureProdOpt m1 m2 =
    match m1, m2 with
    | Measure.One, _ -> m2
    | _, Measure.One -> m1
    | _, _ -> Measure.Prod (m1, m2)

/// Construct a measure expression representing the product of a list of measures
let ProdMeasures ms = 
    match ms with 
    | [] -> Measure.One 
    | m::ms -> List.foldBack MeasureProdOpt ms m

let isDimensionless g tyarg =
    match stripTyparEqns tyarg with
    | TType_measure unt ->
      isNil (ListMeasureVarOccsWithNonZeroExponents unt) && 
      isNil (ListMeasureConOccsWithNonZeroExponents g true unt)
    | _ -> false

let destUnitParMeasure g unt =
    let vs = ListMeasureVarOccsWithNonZeroExponents unt
    let cs = ListMeasureConOccsWithNonZeroExponents g true unt

    match vs, cs with
    | [(v, e)], [] when e = OneRational -> v
    | _, _ -> failwith "destUnitParMeasure: not a unit-of-measure parameter"

let isUnitParMeasure g unt =
    let vs = ListMeasureVarOccsWithNonZeroExponents unt
    let cs = ListMeasureConOccsWithNonZeroExponents g true unt
 
    match vs, cs with
    | [(_, e)], [] when e = OneRational -> true
    | _, _ -> false

let normalizeMeasure g ms =
    let vs = ListMeasureVarOccsWithNonZeroExponents ms
    let cs = ListMeasureConOccsWithNonZeroExponents g false ms
    match vs, cs with
    | [], [] -> Measure.One
    | [(v, e)], [] when e = OneRational -> Measure.Var v
    | vs, cs -> List.foldBack (fun (v, e) -> fun m -> Measure.Prod (Measure.RationalPower (Measure.Var v, e), m)) vs (List.foldBack (fun (c, e) -> fun m -> Measure.Prod (Measure.RationalPower (Measure.Con c, e), m)) cs Measure.One)
 
let tryNormalizeMeasureInType g ty =
    match ty with
    | TType_measure (Measure.Var v) ->
        match v.Solution with
        | Some (TType_measure ms) ->
            v.typar_solution <- Some (TType_measure (normalizeMeasure g ms))
            ty
        | _ -> ty
    | _ -> ty

//---------------------------------------------------------------------------
// Some basic type builders
//---------------------------------------------------------------------------

let mkNativePtrTy (g:TcGlobals) ty = TType_app (g.nativeptr_tcr, [ty])
let mkByrefTy (g:TcGlobals) ty = TType_app (g.byref_tcr, [ty])

let mkArrayTy (g:TcGlobals) rank ty m =
    if rank < 1 || rank > 32 then
        errorR(Error(FSComp.SR.tastopsMaxArrayThirtyTwo(rank), m))
        TType_app (g.il_arr_tcr_map.[3], [ty])
    else
        TType_app (g.il_arr_tcr_map.[rank - 1], [ty])

//--------------------------------------------------------------------------
// Tuple compilation (types)
//------------------------------------------------------------------------ 

let maxTuple = 8
let goodTupleFields = maxTuple-1

let isCompiledTupleTyconRef g tcref =
    tyconRefEq g g.ref_tuple1_tcr tcref || 
    tyconRefEq g g.ref_tuple2_tcr tcref || 
    tyconRefEq g g.ref_tuple3_tcr tcref || 
    tyconRefEq g g.ref_tuple4_tcr tcref || 
    tyconRefEq g g.ref_tuple5_tcr tcref || 
    tyconRefEq g g.ref_tuple6_tcr tcref || 
    tyconRefEq g g.ref_tuple7_tcr tcref || 
    tyconRefEq g g.ref_tuple8_tcr tcref ||
    tyconRefEq g g.struct_tuple1_tcr tcref || 
    tyconRefEq g g.struct_tuple2_tcr tcref || 
    tyconRefEq g g.struct_tuple3_tcr tcref || 
    tyconRefEq g g.struct_tuple4_tcr tcref || 
    tyconRefEq g g.struct_tuple5_tcr tcref || 
    tyconRefEq g g.struct_tuple6_tcr tcref || 
    tyconRefEq g g.struct_tuple7_tcr tcref || 
    tyconRefEq g g.struct_tuple8_tcr tcref

let mkCompiledTupleTyconRef (g:TcGlobals) isStruct n = 
    if   n = 1 then (if isStruct then g.struct_tuple1_tcr else g.ref_tuple1_tcr)
    elif n = 2 then (if isStruct then g.struct_tuple2_tcr else g.ref_tuple2_tcr)
    elif n = 3 then (if isStruct then g.struct_tuple3_tcr else g.ref_tuple3_tcr)
    elif n = 4 then (if isStruct then g.struct_tuple4_tcr else g.ref_tuple4_tcr)
    elif n = 5 then (if isStruct then g.struct_tuple5_tcr else g.ref_tuple5_tcr)
    elif n = 6 then (if isStruct then g.struct_tuple6_tcr else g.ref_tuple6_tcr)
    elif n = 7 then (if isStruct then g.struct_tuple7_tcr else g.ref_tuple7_tcr)
    elif n = 8 then (if isStruct then g.struct_tuple8_tcr else g.ref_tuple8_tcr)
    else failwithf "mkCompiledTupleTyconRef, n = %d" n

/// Convert from F# tuple types to .NET tuple types
let rec mkCompiledTupleTy g isStruct tupElemTys = 
    let n = List.length tupElemTys 
    if n < maxTuple then
        TType_app (mkCompiledTupleTyconRef g isStruct n, tupElemTys)
    else 
        let tysA, tysB = List.splitAfter goodTupleFields tupElemTys
        TType_app ((if isStruct then g.struct_tuple8_tcr else g.ref_tuple8_tcr), tysA@[mkCompiledTupleTy g isStruct tysB])

/// Convert from F# tuple types to .NET tuple types, but only the outermost level
let mkOuterCompiledTupleTy g isStruct tupElemTys = 
    let n = List.length tupElemTys 
    if n < maxTuple then 
        TType_app (mkCompiledTupleTyconRef g isStruct n, tupElemTys)
    else 
        let tysA, tysB = List.splitAfter goodTupleFields tupElemTys
        let tcref = (if isStruct then g.struct_tuple8_tcr else g.ref_tuple8_tcr)
        // In the case of an 8-tuple we add the Tuple<_> marker. For other sizes we keep the type 
        // as a regular F# tuple type.
        match tysB with 
        | [ tyB ] -> 
            let marker = TType_app (mkCompiledTupleTyconRef g isStruct 1, [tyB])
            TType_app (tcref, tysA@[marker])
        | _ ->
            TType_app (tcref, tysA@[TType_tuple (TupInfo.Const isStruct, tysB)])

//---------------------------------------------------------------------------
// Remove inference equations and abbreviations from types 
//---------------------------------------------------------------------------

let applyTyconAbbrev abbrevTy tycon tyargs = 
    if isNil tyargs then abbrevTy 
    else instType (mkTyconInst tycon tyargs) abbrevTy

let reduceTyconAbbrev (tycon:Tycon) tyargs = 
    let abbrev = tycon.TypeAbbrev
    match abbrev with 
    | None -> invalidArg "tycon" "this type definition is not an abbreviation";
    | Some abbrevTy -> 
        applyTyconAbbrev abbrevTy tycon tyargs

let reduceTyconRefAbbrev (tcref:TyconRef) tyargs = 
    reduceTyconAbbrev tcref.Deref tyargs

let reduceTyconMeasureableOrProvided (g:TcGlobals) (tycon:Tycon) tyargs =
#if NO_EXTENSIONTYPING
    ignore g  // otherwise g would be unused
#endif
    let repr = tycon.TypeReprInfo
    match repr with 
    | TMeasureableRepr ty -> 
        if isNil tyargs then ty else instType (mkTyconInst tycon tyargs) ty
#if !NO_EXTENSIONTYPING
    | TProvidedTypeExtensionPoint info when info.IsErased -> info.BaseTypeForErased (range0, g.obj_ty)
#endif
    | _ -> invalidArg "tc" "this type definition is not a refinement" 

let reduceTyconRefMeasureableOrProvided (g:TcGlobals) (tcref:TyconRef) tyargs = 
    reduceTyconMeasureableOrProvided g tcref.Deref tyargs

let rec stripTyEqnsA g canShortcut ty = 
    let ty = stripTyparEqnsAux canShortcut ty 
    match ty with 
    | TType_app (tcref, tinst) -> 
        let tycon = tcref.Deref
        match tycon.TypeAbbrev with 
        | Some abbrevTy -> 
            stripTyEqnsA g canShortcut (applyTyconAbbrev abbrevTy tycon tinst)
        | None -> 
            if tycon.IsMeasureableReprTycon && List.forall (isDimensionless g) tinst then
                stripTyEqnsA g canShortcut (reduceTyconMeasureableOrProvided g tycon tinst)
            else 
                ty
    | ty -> ty

let stripTyEqns g ty = stripTyEqnsA g false ty

let evalTupInfoIsStruct aexpr = 
    match aexpr with 
    | TupInfo.Const b -> b

/// This erases outermost occurrences of inference equations, type abbreviations, non-generated provided types
/// and measureable types (float<_>).
/// It also optionally erases all "compilation representations", i.e. function and
/// tuple types, and also "nativeptr<'T> --> System.IntPtr"
let rec stripTyEqnsAndErase eraseFuncAndTuple (g:TcGlobals) ty =
    let ty = stripTyEqns g ty
    match ty with
    | TType_app (tcref, args) -> 
        let tycon = tcref.Deref
        if tycon.IsErased  then
            stripTyEqnsAndErase eraseFuncAndTuple g (reduceTyconMeasureableOrProvided g tycon args)
        elif tyconRefEq g tcref g.nativeptr_tcr && eraseFuncAndTuple then 
            stripTyEqnsAndErase eraseFuncAndTuple g g.nativeint_ty
        else
            ty
    | TType_fun(a, b) when eraseFuncAndTuple -> TType_app(g.fastFunc_tcr, [ a; b]) 
    | TType_tuple(tupInfo, l) when eraseFuncAndTuple -> mkCompiledTupleTy g (evalTupInfoIsStruct tupInfo) l
    | ty -> ty

let stripTyEqnsAndMeasureEqns g ty =
   stripTyEqnsAndErase false g ty
       
type Erasure = EraseAll | EraseMeasures | EraseNone

let stripTyEqnsWrtErasure erasureFlag g ty = 
    match erasureFlag with 
    | EraseAll -> stripTyEqnsAndErase true g ty
    | EraseMeasures -> stripTyEqnsAndErase false g ty
    | _ -> stripTyEqns g ty
    
let rec stripExnEqns (eref:TyconRef) = 
    let exnc = eref.Deref
    match exnc.ExceptionInfo with
    | TExnAbbrevRepr eref -> stripExnEqns eref
    | _ -> exnc


let primDestForallTy g ty = ty |> stripTyEqns g |> (function TType_forall (tyvs, tau) -> (tyvs, tau) | _ -> failwith "primDestForallTy: not a forall type")
let destFunTy      g ty = ty |> stripTyEqns g |> (function TType_fun (tyv, tau) -> (tyv, tau) | _ -> failwith "destFunTy: not a function type")
let destAnyTupleTy    g ty = ty |> stripTyEqns g |> (function TType_tuple (tupInfo, l) -> tupInfo, l | _ -> failwith "destAnyTupleTy: not a tuple type")
let destRefTupleTy    g ty = ty |> stripTyEqns g |> (function TType_tuple (tupInfo, l) when not (evalTupInfoIsStruct tupInfo) -> l | _ -> failwith "destRefTupleTy: not a reference tuple type")
let destStructTupleTy    g ty = ty |> stripTyEqns g |> (function TType_tuple (tupInfo, l) when evalTupInfoIsStruct tupInfo -> l | _ -> failwith "destStructTupleTy: not a struct tuple type")
let destTyparTy    g ty = ty |> stripTyEqns g |> (function TType_var v -> v | _ -> failwith "destTyparTy: not a typar type")
let destAnyParTy   g ty = ty |> stripTyEqns g |> (function TType_var v -> v | TType_measure unt -> destUnitParMeasure g unt | _ -> failwith "destAnyParTy: not a typar or unpar type")
let destMeasureTy  g ty = ty |> stripTyEqns g |> (function TType_measure m -> m | _ -> failwith "destMeasureTy: not a unit-of-measure type")
let isFunTy        g ty = ty |> stripTyEqns g |> (function TType_fun _ -> true | _ -> false)
let isForallTy     g ty = ty |> stripTyEqns g |> (function TType_forall _ -> true | _ -> false)
let isAnyTupleTy      g ty = ty |> stripTyEqns g |> (function TType_tuple _ -> true | _ -> false)
let isRefTupleTy      g ty = ty |> stripTyEqns g |> (function TType_tuple (tupInfo, _) -> not (evalTupInfoIsStruct tupInfo) | _ -> false)
let isStructTupleTy      g ty = ty |> stripTyEqns g |> (function TType_tuple (tupInfo, _) -> evalTupInfoIsStruct tupInfo | _ -> false)
let isUnionTy      g ty = ty |> stripTyEqns g |> (function TType_app(tcr, _) -> tcr.IsUnionTycon | _ -> false)
let isReprHiddenTy   g ty = ty |> stripTyEqns g |> (function TType_app(tcr, _) -> tcr.IsHiddenReprTycon | _ -> false)
let isFSharpObjModelTy g ty = ty |> stripTyEqns g |> (function TType_app(tcr, _) -> tcr.IsFSharpObjectModelTycon | _ -> false)
let isRecdTy       g ty = ty |> stripTyEqns g |> (function TType_app(tcr, _) -> tcr.IsRecordTycon | _ -> false)
let isFSharpStructOrEnumTy   g ty = ty |> stripTyEqns g |> (function TType_app(tcr, _) -> tcr.IsFSharpStructOrEnumTycon | _ -> false)
let isFSharpEnumTy   g ty = ty |> stripTyEqns g |> (function TType_app(tcr, _) -> tcr.IsFSharpEnumTycon | _ -> false)
let isTyparTy      g ty = ty |> stripTyEqns g |> (function TType_var _ -> true | _ -> false)
let isAnyParTy     g ty = ty |> stripTyEqns g |> (function TType_var _ -> true | TType_measure unt -> isUnitParMeasure g unt | _ -> false)
let isMeasureTy    g ty = ty |> stripTyEqns g |> (function TType_measure _ -> true | _ -> false)


let isProvenUnionCaseTy ty = match ty with TType_ucase _ -> true | _ -> false

let mkAppTy tcref tyargs = TType_app(tcref, tyargs)
let mkProvenUnionCaseTy ucref tyargs = TType_ucase(ucref, tyargs)
let isAppTy   g ty = ty |> stripTyEqns g |> (function TType_app _ -> true | _ -> false) 
let destAppTy g ty = ty |> stripTyEqns g |> (function TType_app(tcref, tinst) -> tcref, tinst | _ -> failwith "destAppTy")
let tcrefOfAppTy  g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> tcref | _ -> failwith "tcrefOfAppTy") 
let argsOfAppTy   g ty = ty |> stripTyEqns g |> (function TType_app(_, tinst) -> tinst | _ -> [])
let tryDestTyparTy g ty = ty |> stripTyEqns g |> (function TType_var v -> Some v | _ -> None)
let tryDestFunTy   g ty = ty |> stripTyEqns g |> (function TType_fun (tyv, tau) -> Some(tyv, tau) | _ -> None)
let tryDestAppTy   g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> Some tcref | _ -> None)
let tryAnyParTy    g ty = ty |> stripTyEqns g |> (function TType_var v -> Some v | TType_measure unt when isUnitParMeasure g unt -> Some(destUnitParMeasure g unt) | _ -> None)
let (|AppTy|_|) g ty = ty |> stripTyEqns g |> (function TType_app(tcref, tinst) -> Some (tcref, tinst) | _ -> None) 
let (|RefTupleTy|_|) g ty = ty |> stripTyEqns g |> (function TType_tuple(tupInfo, tys) when not (evalTupInfoIsStruct tupInfo) -> Some tys | _ -> None)
let (|FunTy|_|) g ty = ty |> stripTyEqns g |> (function TType_fun(dty, rty) -> Some (dty, rty) | _ -> None)

let tryNiceEntityRefOfTy  ty = 
    let ty = stripTyparEqnsAux false ty 
    match ty with 
    | TType_app (tcref, _) -> Some tcref
    | TType_measure (Measure.Con tcref) -> Some tcref
    | _ -> None


let (|NullableTy|_|) g ty = 
    match ty with 
    | AppTy g (tcr, [tyarg]) when tyconRefEq g tcr g.system_Nullable_tcref -> Some tyarg
    | _ -> None

let (|StripNullableTy|) g ty = 
    match ty with 
    | AppTy g (tcr, [tyarg]) when tyconRefEq g tcr g.system_Nullable_tcref -> tyarg
    | _ -> ty
    
let (|ByrefTy|_|) g ty = 
    match ty with 
    | AppTy g (tcr, [tyarg]) when tyconRefEq g tcr g.byref_tcr -> Some tyarg
    | _ -> None

let mkInstForAppTy g typ = 
    match typ with
    | AppTy g (tcref, tinst) -> mkTyconRefInst tcref tinst
    | _ -> []

let domainOfFunTy g ty = fst (destFunTy g ty)
let rangeOfFunTy  g ty = snd (destFunTy g ty)

let helpEnsureTypeHasMetadata g ty = 
    if isAnyTupleTy g ty then 
        let (tupInfo, tupElemTys) = destAnyTupleTy g ty
        mkOuterCompiledTupleTy g (evalTupInfoIsStruct tupInfo) tupElemTys
    else ty


//---------------------------------------------------------------------------
// Equivalence of types up to alpha-equivalence 
//---------------------------------------------------------------------------


[<NoEquality; NoComparison>]
type TypeEquivEnv = 
    { EquivTypars: TyparMap<TType>
      EquivTycons: TyconRefRemap}

// allocate a singleton
let typeEquivEnvEmpty = 
    { EquivTypars = TyparMap.Empty
      EquivTycons = emptyTyconRefRemap }

type TypeEquivEnv with 
    static member Empty = typeEquivEnvEmpty

    member aenv.BindTyparsToTypes tps1 tys2 =
        { aenv with EquivTypars = (tps1, tys2, aenv.EquivTypars) |||> List.foldBack2 (fun tp ty tpmap -> tpmap.Add(tp, ty)) }

    member aenv.BindEquivTypars tps1 tps2 =
        aenv.BindTyparsToTypes tps1 (List.map mkTyparTy tps2) 

    static member FromTyparInst tpinst =
        let tps, tys = List.unzip tpinst
        TypeEquivEnv.Empty.BindTyparsToTypes tps tys 

    static member FromEquivTypars tps1 tps2 = 
        TypeEquivEnv.Empty.BindEquivTypars tps1 tps2 

let rec traitsAEquivAux erasureFlag g aenv (TTrait(typs1, nm, mf1, argtys, rty, _)) (TTrait(typs2, nm2, mf2, argtys2, rty2, _)) =
   mf1 = mf2 &&
   nm = nm2 &&
   ListSet.equals (typeAEquivAux erasureFlag g aenv) typs1 typs2 &&
   returnTypesAEquivAux erasureFlag g aenv rty rty2 &&
   List.lengthsEqAndForall2 (typeAEquivAux erasureFlag g aenv) argtys argtys2

and returnTypesAEquivAux erasureFlag g aenv rty rty2 =
    match rty, rty2 with  
    | None, None -> true
    | Some t1, Some t2 -> typeAEquivAux erasureFlag g aenv t1 t2
    | _ -> false

    
and typarConstraintsAEquivAux erasureFlag g aenv tpc1 tpc2 =
    match tpc1, tpc2 with
    | TyparConstraint.CoercesTo(acty, _), 
      TyparConstraint.CoercesTo(fcty, _) -> 
        typeAEquivAux erasureFlag g aenv acty fcty

    | TyparConstraint.MayResolveMember(trait1, _), 
      TyparConstraint.MayResolveMember(trait2, _) -> 
        traitsAEquivAux erasureFlag g aenv trait1 trait2 

    | TyparConstraint.DefaultsTo(_, acty, _), 
      TyparConstraint.DefaultsTo(_, fcty, _) -> 
        typeAEquivAux erasureFlag g aenv acty fcty

    | TyparConstraint.IsEnum(uty1, _), TyparConstraint.IsEnum(uty2, _) -> 
        typeAEquivAux erasureFlag g aenv uty1 uty2

    | TyparConstraint.IsDelegate(aty1, bty1, _), TyparConstraint.IsDelegate(aty2, bty2, _) -> 
        typeAEquivAux erasureFlag g aenv aty1 aty2 && 
        typeAEquivAux erasureFlag g aenv bty1 bty2 

    | TyparConstraint.SimpleChoice (tys1, _), TyparConstraint.SimpleChoice(tys2, _) -> 
        ListSet.equals (typeAEquivAux erasureFlag g aenv) tys1 tys2

    | TyparConstraint.SupportsComparison _        , TyparConstraint.SupportsComparison _ 
    | TyparConstraint.SupportsEquality _          , TyparConstraint.SupportsEquality _ 
    | TyparConstraint.SupportsNull _              , TyparConstraint.SupportsNull _ 
    | TyparConstraint.IsNonNullableStruct _    , TyparConstraint.IsNonNullableStruct _
    | TyparConstraint.IsReferenceType _           , TyparConstraint.IsReferenceType _ 
    | TyparConstraint.IsUnmanaged _               , TyparConstraint.IsUnmanaged _
    | TyparConstraint.RequiresDefaultConstructor _, TyparConstraint.RequiresDefaultConstructor _ -> true
    | _ -> false

and typarConstraintSetsAEquivAux erasureFlag g aenv (tp1:Typar) (tp2:Typar) = 
    tp1.StaticReq = tp2.StaticReq &&
    ListSet.equals (typarConstraintsAEquivAux erasureFlag g aenv) tp1.Constraints tp2.Constraints

and typarsAEquivAux erasureFlag g (aenv: TypeEquivEnv) tps1 tps2 = 
    List.length tps1 = List.length tps2 &&
    let aenv = aenv.BindEquivTypars tps1 tps2 
    List.forall2 (typarConstraintSetsAEquivAux erasureFlag g aenv) tps1 tps2

and tcrefAEquiv g aenv tc1 tc2 = 
    tyconRefEq g tc1 tc2 || 
    (aenv.EquivTycons.ContainsKey tc1  && tyconRefEq g aenv.EquivTycons.[tc1] tc2)

and typeAEquivAux erasureFlag g aenv ty1 ty2 = 
    let ty1 = stripTyEqnsWrtErasure erasureFlag g ty1 
    let ty2 = stripTyEqnsWrtErasure erasureFlag g ty2
    match ty1, ty2 with
    | TType_forall(tps1, rty1), TType_forall(tps2, rty2) -> 
        typarsAEquivAux erasureFlag g aenv tps1 tps2 && typeAEquivAux erasureFlag g (aenv.BindEquivTypars tps1 tps2) rty1 rty2
    | TType_var tp1, TType_var tp2 when typarEq tp1 tp2 -> 
        true
    | TType_var tp1, _ when aenv.EquivTypars.ContainsKey tp1 -> 
        typeEquivAux erasureFlag g aenv.EquivTypars.[tp1] ty2
    | TType_app (tc1, b1)  , TType_app (tc2, b2) -> 
        tcrefAEquiv g aenv tc1 tc2 &&
        typesAEquivAux erasureFlag g aenv b1 b2
    | TType_ucase (UCRef(tc1, n1), b1)  , TType_ucase (UCRef(tc2, n2), b2) -> 
        n1=n2 &&
        tcrefAEquiv g aenv tc1 tc2 &&
        typesAEquivAux erasureFlag g aenv b1 b2
    | TType_tuple (s1, l1), TType_tuple (s2, l2) -> 
        structnessAEquiv s1 s2 && typesAEquivAux erasureFlag g aenv l1 l2
    | TType_fun (dtys1, rty1), TType_fun (dtys2, rty2) -> 
        typeAEquivAux erasureFlag g aenv dtys1 dtys2 && typeAEquivAux erasureFlag g aenv rty1 rty2
    | TType_measure m1, TType_measure m2 -> 
        match erasureFlag with 
        | EraseNone -> measureAEquiv g aenv m1 m2 
        | _ -> true 
    | _ -> false

and structnessAEquiv un1 un2 =
    match un1, un2 with 
    | TupInfo.Const b1, TupInfo.Const b2 -> (b1 = b2)

and measureAEquiv g aenv un1 un2 =
    let vars1 = ListMeasureVarOccs un1
    let trans tp1 = if aenv.EquivTypars.ContainsKey tp1 then destAnyParTy g aenv.EquivTypars.[tp1] else tp1
    let remapTyconRef tc = if aenv.EquivTycons.ContainsKey tc then aenv.EquivTycons.[tc] else tc
    let vars1' = List.map trans vars1
    let vars2 = ListSet.subtract typarEq (ListMeasureVarOccs un2) vars1'
    let cons1 = ListMeasureConOccsAfterRemapping g remapTyconRef un1
    let cons2 = ListMeasureConOccsAfterRemapping g remapTyconRef un2 
 
    List.forall (fun v -> MeasureVarExponent v un1 = MeasureVarExponent (trans v) un2) vars1 &&
    List.forall (fun v -> MeasureVarExponent v un1 = MeasureVarExponent v un2) vars2 &&
    List.forall (fun c -> MeasureConExponentAfterRemapping g remapTyconRef c un1 = MeasureConExponentAfterRemapping g remapTyconRef c un2) (cons1@cons2)  


and typesAEquivAux erasureFlag g aenv l1 l2 = List.lengthsEqAndForall2 (typeAEquivAux erasureFlag g aenv) l1 l2
and typeEquivAux erasureFlag g ty1 ty2 = typeAEquivAux erasureFlag g TypeEquivEnv.Empty ty1 ty2

let typeAEquiv g aenv ty1 ty2 = typeAEquivAux EraseNone g aenv ty1 ty2
let typeEquiv g ty1 ty2 = typeEquivAux EraseNone g ty1 ty2
let traitsAEquiv g aenv t1 t2 = traitsAEquivAux EraseNone g aenv t1 t2
let typarConstraintsAEquiv g aenv c1 c2 = typarConstraintsAEquivAux EraseNone g aenv c1 c2
let typarsAEquiv g aenv d1 d2 = typarsAEquivAux EraseNone g aenv d1 d2
let returnTypesAEquiv g aenv t1 t2 = returnTypesAEquivAux EraseNone g aenv t1 t2

let measureEquiv g m1 m2 = measureAEquiv g TypeEquivEnv.Empty m1 m2

let isErasedType g ty = 
  match stripTyEqns g ty with
#if !NO_EXTENSIONTYPING
  | TType_app (tcref, _) -> tcref.IsProvidedErasedTycon
#endif
  | _ -> false

// Return all components of this type expression that cannot be tested at runtime
let rec getErasedTypes g ty = 
    let ty = stripTyEqns g ty
    if isErasedType g ty then [ty] else 
    match ty with
    | TType_forall(_, rty) -> 
        getErasedTypes g rty
    | TType_var tp -> 
        if tp.IsErased then [ty] else []
    | TType_app (_, b) | TType_ucase(_, b) | TType_tuple (_, b) ->
        List.foldBack (fun ty tys -> getErasedTypes g ty @ tys) b []
    | TType_fun (dty, rty) -> 
        getErasedTypes g dty @ getErasedTypes g rty
    | TType_measure _ -> 
        [ty]


//---------------------------------------------------------------------------
// Standard orderings, e.g. for order set/map keys
//---------------------------------------------------------------------------

let valOrder = { new IComparer<Val> with member __.Compare(v1, v2) = compare v1.Stamp v2.Stamp }
let tyconOrder = { new IComparer<Tycon> with member __.Compare(tc1, tc2) = compare tc1.Stamp tc2.Stamp }
let recdFieldRefOrder  = 
    { new IComparer<RecdFieldRef> with 
         member __.Compare(RFRef(tcref1, nm1), RFRef(tcref2, nm2)) = 
            let c = tyconOrder.Compare (tcref1.Deref, tcref2.Deref) 
            if c <> 0 then c else 
            compare nm1 nm2 }

let unionCaseRefOrder = 
    { new IComparer<UnionCaseRef> with 
         member __.Compare(UCRef(tcref1, nm1), UCRef(tcref2, nm2)) = 
            let c = tyconOrder.Compare (tcref1.Deref, tcref2.Deref) 
            if c <> 0 then c else 
            compare nm1 nm2 }

//---------------------------------------------------------------------------
// Make some common types
//---------------------------------------------------------------------------

let mkFunTy d r = TType_fun (d, r)
let (-->) d r = mkFunTy d r
let mkForallTy d r = TType_forall (d, r)
let tryMkForallTy d r = if isNil d then r else mkForallTy d r
let (+->) d r = tryMkForallTy d r
let mkIteratedFunTy dl r = List.foldBack (-->) dl r

let mkLambdaArgTy m tys = 
    match tys with 
    | [] -> error(InternalError("mkLambdaArgTy", m))
    | [h] -> h 
    | _ -> mkRawRefTupleTy tys

let typeOfLambdaArg m vs = mkLambdaArgTy m (typesOfVals vs)
let mkMultiLambdaTy m vs rty = mkFunTy (typeOfLambdaArg m vs) rty 
let mkLambdaTy tps tys rty = tryMkForallTy tps (mkIteratedFunTy tys rty)

/// When compiling FSharp.Core.dll we have to deal with the non-local references into
/// the library arising from env.fs. Part of this means that we have to be able to resolve these
/// references. This function artificially forces the existence of a module or namespace at a 
/// particular point in order to do this.
let ensureCcuHasModuleOrNamespaceAtPath (ccu:CcuThunk) path (CompPath(_, cpath)) xml =
    let scoref = ccu.ILScopeRef 
    let rec loop prior_cpath (path:Ident list) cpath (modul:ModuleOrNamespace) =
        let mtype = modul.ModuleOrNamespaceType 
        match path, cpath with 
        | (hpath::tpath), ((_, mkind)::tcpath)  -> 
            let modName = hpath.idText 
            if not (Map.containsKey modName mtype.AllEntitiesByCompiledAndLogicalMangledNames) then 
                let smodul = NewModuleOrNamespace (Some(CompPath(scoref, prior_cpath))) taccessPublic hpath xml [] (MaybeLazy.Strict (NewEmptyModuleOrNamespaceType mkind))
                mtype.AddModuleOrNamespaceByMutation(smodul);
            let modul = Map.find modName mtype.AllEntitiesByCompiledAndLogicalMangledNames 
            loop (prior_cpath@[(modName, Namespace)]) tpath tcpath modul 

        | _ -> () 

    loop [] path cpath ccu.Contents


//---------------------------------------------------------------------------
// Primitive destructors
//---------------------------------------------------------------------------

/// Look through the Expr.Link nodes arising from type inference
let rec stripExpr e = 
    match e with 
    | Expr.Link eref -> stripExpr !eref
    | _ -> e    

let mkCase (a, b) = TCase(a, b)

let isRefTupleExpr e = match e with Expr.Op (TOp.Tuple tupInfo, _, _, _) -> not (evalTupInfoIsStruct tupInfo) | _ -> false
let tryDestRefTupleExpr e = match e with Expr.Op (TOp.Tuple tupInfo, _, es, _) when not (evalTupInfoIsStruct tupInfo) -> es | _ -> [e]

//---------------------------------------------------------------------------
// Range info for expressions
//---------------------------------------------------------------------------

let rec rangeOfExpr x = 
    match x with
    | Expr.Val (_, _, m) | Expr.Op (_, _, _, m)   | Expr.Const (_, m, _) | Expr.Quote (_, _, _, m, _)
    | Expr.Obj (_, _, _, _, _, _, m) | Expr.App(_, _, _, _, m) | Expr.Sequential (_, _, _, _, m) 
    | Expr.StaticOptimization (_, _, _, m) | Expr.Lambda (_, _, _, _, _, m, _) 
    | Expr.TyLambda (_, _, _, m, _)| Expr.TyChoose (_, _, m) | Expr.LetRec (_, _, m, _) | Expr.Let (_, _, m, _) | Expr.Match (_, _, _, _, m, _) -> m
    | Expr.Link(eref) -> rangeOfExpr (!eref)

type Expr with 
    member x.Range = rangeOfExpr x

//---------------------------------------------------------------------------
// Build nodes in decision graphs
//---------------------------------------------------------------------------


let primMkMatch(spBind, exprm, tree, targets, matchm, ty) = Expr.Match (spBind, exprm, tree, targets, matchm, ty)

type MatchBuilder(spBind, inpRange: Range.range) = 

    let targets = new ResizeArray<_>(10) 
    member x.AddTarget(tg) = 
        let n = targets.Count 
        targets.Add tg
        n

    member x.AddResultTarget(e, spTarget) = TDSuccess([], x.AddTarget(TTarget([], e, spTarget)))

    member x.CloseTargets() = targets |> ResizeArray.toList

    member x.Close(dtree, m, ty) = primMkMatch  (spBind, inpRange, dtree, targets.ToArray(), m, ty)

let mkBoolSwitch m g t e = TDSwitch(g, [TCase(DecisionTreeTest.Const(Const.Bool(true)), t)], Some e, m)

let primMkCond spBind spTarget1 spTarget2 m ty e1 e2 e3 = 
    let mbuilder = new MatchBuilder(spBind, m)
    let dtree = mkBoolSwitch m e1 (mbuilder.AddResultTarget(e2, spTarget1)) (mbuilder.AddResultTarget(e3, spTarget2)) 
    mbuilder.Close(dtree, m, ty)

let mkCond spBind spTarget m ty e1 e2 e3 =  primMkCond spBind spTarget spTarget m ty e1 e2 e3


//---------------------------------------------------------------------------
// Primitive constructors
//---------------------------------------------------------------------------

let exprForValRef m vref =  Expr.Val(vref, NormalValUse, m)
let exprForVal m v =  exprForValRef m (mkLocalValRef v)
let mkLocalAux m s ty mut compgen =
    let thisv = NewVal(s, m, None, ty, mut, compgen, None, taccessPublic, ValNotInRecScope, None, NormalVal, [], ValInline.Optional, XmlDoc.Empty, false, false, false, false, false, false, None, ParentNone) 
    thisv, exprForVal m thisv

let mkLocal         m s ty = mkLocalAux m s ty Immutable false
let mkCompGenLocal m s ty = mkLocalAux m s ty Immutable true
let mkMutableCompGenLocal m s ty = mkLocalAux m s ty Mutable true


// Type gives return type.  For type-lambdas this is the formal return type. 
let mkMultiLambda m vs (b, rty) = Expr.Lambda (newUnique(), None, None, vs, b, m, rty)
let rebuildLambda m ctorThisValOpt baseValOpt vs (b, rty) = Expr.Lambda (newUnique(), ctorThisValOpt, baseValOpt, vs, b, m, rty)
let mkLambda m v (b, rty) = mkMultiLambda m [v] (b, rty)
let mkTypeLambda m vs (b, tau_ty) = match vs with [] -> b | _ -> Expr.TyLambda (newUnique(), vs, b, m, tau_ty)
let mkTypeChoose m vs b = match vs with [] -> b | _ -> Expr.TyChoose (vs, b, m)

let mkObjExpr (ty, basev, basecall, overrides, iimpls, m) = 
    Expr.Obj (newUnique(), ty, basev, basecall, overrides, iimpls, m) 

let mkLambdas m tps (vs:Val list) (b, rty) = 
    mkTypeLambda m tps (List.foldBack (fun v (e, ty) -> mkLambda m v (e, ty), v.Type --> ty) vs (b, rty))

let mkMultiLambdasCore m vsl (b, rty) = 
    List.foldBack (fun v (e, ty) -> mkMultiLambda m v (e, ty), typeOfLambdaArg m v --> ty) vsl (b, rty)

let mkMultiLambdas m tps vsl (b, rty) = 
    mkTypeLambda m tps (mkMultiLambdasCore m vsl (b, rty) )

let mkMemberLambdas m tps ctorThisValOpt baseValOpt vsl (b, rty) = 
    let expr = 
        match ctorThisValOpt, baseValOpt with
        | None, None -> mkMultiLambdasCore m vsl (b, rty)
        | _ -> 
            match vsl with 
            | [] -> error(InternalError("mk_basev_multi_lambdas_core: can't attach a basev to a non-lambda expression", m))
            | h::t -> 
                let b, rty = mkMultiLambdasCore m t (b, rty)
                (rebuildLambda m ctorThisValOpt baseValOpt h (b, rty), (typeOfLambdaArg m h --> rty))
    mkTypeLambda m tps expr

let mkMultiLambdaBind v letSeqPtOpt m  tps vsl (b, rty) = 
    TBind(v, mkMultiLambdas m tps vsl (b, rty), letSeqPtOpt)

let mkBind seqPtOpt v e = TBind(v, e, seqPtOpt)

let mkLetBind m bind body = Expr.Let(bind, body, m, NewFreeVarsCache())
let mkLetsBind m binds body = List.foldBack (mkLetBind m) binds body 
let mkLetsFromBindings m binds body = List.foldBack (mkLetBind m) binds body 
let mkLet seqPtOpt m v x body = mkLetBind m (mkBind seqPtOpt v x) body

/// Make sticky bindings that are compiler generated (though the variables may not be - e.g. they may be lambda arguments in a beta reduction)
let mkCompGenBind v e = TBind(v, e, NoSequencePointAtStickyBinding)
let mkCompGenBinds (vs: Val list) (es: Expr list) = List.map2 mkCompGenBind vs es
let mkCompGenLet m v x body = mkLetBind m (mkCompGenBind v x) body
let mkCompGenLets m vs xs body = mkLetsBind m (mkCompGenBinds vs xs) body
let mkCompGenLetsFromBindings m vs xs body = mkLetsFromBindings m (mkCompGenBinds vs xs) body

let mkInvisibleBind v e = TBind(v, e, NoSequencePointAtInvisibleBinding)
let mkInvisibleBinds (vs: Val list) (es: Expr list) = List.map2 mkInvisibleBind vs es
let mkInvisibleLet m v x body = mkLetBind m (mkInvisibleBind v x) body
let mkInvisibleLets m vs xs body = mkLetsBind m (mkInvisibleBinds vs xs) body
let mkInvisibleLetsFromBindings m vs xs body = mkLetsFromBindings m (mkInvisibleBinds vs xs) body

let mkLetRecBinds m binds body = if isNil binds then body else Expr.LetRec(binds, body, m, NewFreeVarsCache())

//-------------------------------------------------------------------------
// Type schemes...
//-------------------------------------------------------------------------

// Type parameters may be have been equated to other tps in equi-recursive type inference 
// and unit type inference. Normalize them here 
let NormalizeDeclaredTyparsForEquiRecursiveInference g tps = 
    match tps with 
    | [] -> []
    | tps -> 
        tps |> List.map (fun tp ->
          let ty = mkTyparTy tp
          match tryAnyParTy g ty with
          | Some anyParTy -> anyParTy 
          | None -> tp)
 
type TypeScheme = TypeScheme of Typars * TType    
  
let mkGenericBindRhs g m generalizedTyparsForRecursiveBlock typeScheme bodyExpr = 
    let (TypeScheme(generalizedTypars, tauType)) = typeScheme

    // Normalize the generalized typars
    let generalizedTypars = NormalizeDeclaredTyparsForEquiRecursiveInference g generalizedTypars

    // Some recursive bindings result in free type variables, e.g. 
    //    let rec f (x:'a) = ()  
    //    and g() = f y |> ignore 
    // What is the type of y? Type inference equates it to 'a. 
    // But "g" is not polymorphic in 'a. Hence we get a free choice of "'a" 
    // in the scope of "g". Thus at each individual recursive binding we record all 
    // type variables for which we have a free choice, which is precisely the difference 
    // between the union of all sets of generalized type variables and the set generalized 
    // at each particular binding. 
    //
    // We record an expression node that indicates that a free choice can be made 
    // for these. This expression node effectively binds the type variables. 
    let freeChoiceTypars = ListSet.subtract typarEq generalizedTyparsForRecursiveBlock generalizedTypars
    mkTypeLambda m generalizedTypars (mkTypeChoose m freeChoiceTypars bodyExpr, tauType)

let isBeingGeneralized tp typeScheme = 
    let (TypeScheme(generalizedTypars, _)) = typeScheme
    ListSet.contains typarRefEq tp generalizedTypars

//-------------------------------------------------------------------------
// Build conditional expressions...
//------------------------------------------------------------------------- 

let mkLazyAnd (g:TcGlobals) m e1 e2 = mkCond NoSequencePointAtStickyBinding SuppressSequencePointAtTarget m g.bool_ty e1 e2 (Expr.Const(Const.Bool false, m, g.bool_ty))
let mkLazyOr (g:TcGlobals) m e1 e2 = mkCond NoSequencePointAtStickyBinding SuppressSequencePointAtTarget m g.bool_ty e1 (Expr.Const(Const.Bool true, m, g.bool_ty)) e2

let mkCoerceExpr(e, to_ty, m, from_ty)                     = Expr.Op (TOp.Coerce, [to_ty;from_ty], [e], m)

let mkAsmExpr(code, tinst, args, rettys, m)                 = Expr.Op (TOp.ILAsm(code, rettys), tinst, args, m)
let mkUnionCaseExpr(uc, tinst, args, m)                        = Expr.Op (TOp.UnionCase uc, tinst, args, m)
let mkExnExpr(uc, args, m)                          = Expr.Op (TOp.ExnConstr uc, [], args, m)
let mkTupleFieldGetViaExprAddr(tupInfo, e, tinst, i, m)                  = Expr.Op (TOp.TupleFieldGet(tupInfo, i), tinst, [e], m)

let mkRecdFieldGetViaExprAddr(e, fref, tinst, m)      = Expr.Op (TOp.ValFieldGet(fref), tinst, [e], m)
let mkRecdFieldGetAddrViaExprAddr(e, fref, tinst, m) = Expr.Op (TOp.ValFieldGetAddr(fref), tinst, [e], m)

let mkStaticRecdFieldGetAddr(fref, tinst, m)          = Expr.Op (TOp.ValFieldGetAddr(fref), tinst, [], m)
let mkStaticRecdFieldGet(fref, tinst, m)               = Expr.Op (TOp.ValFieldGet(fref), tinst, [], m)
let mkStaticRecdFieldSet(fref, tinst, e, m)             = Expr.Op (TOp.ValFieldSet(fref), tinst, [e], m)

let mkArrayElemAddress g (readonly, isNativePtr, shape, elemTy, aexpr, nexpr, m) = Expr.Op (TOp.ILAsm ([IL.I_ldelema(readonly, isNativePtr, shape, mkILTyvarTy 0us)], [mkByrefTy g elemTy]), [elemTy], [aexpr;nexpr], m)

let mkRecdFieldSetViaExprAddr (e1, fref, tinst, e2, m)  = Expr.Op (TOp.ValFieldSet(fref), tinst, [e1;e2], m)

let mkUnionCaseTagGetViaExprAddr (e1, cref, tinst, m)      = Expr.Op (TOp.UnionCaseTagGet(cref), tinst, [e1], m)

/// Make a 'TOp.UnionCaseProof' expression, which proves a union value is over a particular case (used only for ref-unions, not struct-unions)
let mkUnionCaseProof (e1, cref:UnionCaseRef, tinst, m)     = if cref.Tycon.IsStructOrEnumTycon then e1 else Expr.Op (TOp.UnionCaseProof(cref), tinst, [e1], m)

/// Build a 'TOp.UnionCaseFieldGet' expression for something we've already determined to be a particular union case. For ref-unions, 
/// the input expression has 'TType_ucase', which is an F# compiler internal "type" corresponding to the union case.  For struct-unions, 
/// the input should be the address of the expression.
let mkUnionCaseFieldGetProvenViaExprAddr (e1, cref, tinst, j, m)   = Expr.Op (TOp.UnionCaseFieldGet(cref, j), tinst, [e1], m)

/// Build a 'TOp.UnionCaseFieldGetAddr' expression for a field of a union when we've already determined the value to be a particular union case. For ref-unions, 
/// the input expression has 'TType_ucase', which is an F# compiler internal "type" corresponding to the union case. For struct-unions, 
/// the input should be the address of the expression.
let mkUnionCaseFieldGetAddrProvenViaExprAddr (e1, cref, tinst, j, m)   = Expr.Op (TOp.UnionCaseFieldGetAddr(cref, j), tinst, [e1], m)

/// Build a 'get' expression for something we've already determined to be a particular union case, but where 
/// the static type of the input is not yet proven to be that particular union case. This requires a type
/// cast to 'prove' the condition.
let mkUnionCaseFieldGetUnprovenViaExprAddr (e1, cref, tinst, j, m)  = mkUnionCaseFieldGetProvenViaExprAddr(mkUnionCaseProof(e1, cref, tinst, m), cref, tinst, j, m)

let mkUnionCaseFieldSet (e1, cref, tinst, j, e2, m)         = Expr.Op (TOp.UnionCaseFieldSet(cref, j), tinst, [e1;e2], m)

let mkExnCaseFieldGet (e1, ecref, j, m)             = Expr.Op (TOp.ExnFieldGet(ecref, j), [], [e1], m)
let mkExnCaseFieldSet (e1, ecref, j, e2, m)          = Expr.Op (TOp.ExnFieldSet(ecref, j), [], [e1;e2], m)

let mkDummyLambda (g:TcGlobals) (e:Expr, ety) = 
    let m = e.Range
    mkLambda m (fst (mkCompGenLocal m "unitVar" g.unit_ty)) (e, ety)
                           
let mkWhile (g:TcGlobals) (spWhile, marker, e1, e2, m)             = 
    Expr.Op (TOp.While (spWhile, marker), []  , [mkDummyLambda g (e1, g.bool_ty);mkDummyLambda g (e2, g.unit_ty)], m)

let mkFor (g:TcGlobals) (spFor, v, e1, dir, e2, e3:Expr, m)    = 
    Expr.Op (TOp.For (spFor, dir)    , []  , [mkDummyLambda g (e1, g.int_ty) ;mkDummyLambda g (e2, g.int_ty);mkLambda e3.Range v (e3, g.unit_ty)], m)

let mkTryWith g (e1, vf, ef:Expr, vh, eh:Expr, m, ty, spTry, spWith) = 
    Expr.Op (TOp.TryCatch(spTry, spWith), [ty], [mkDummyLambda g (e1, ty);mkLambda ef.Range vf (ef, ty);mkLambda eh.Range vh (eh, ty)], m)

let mkTryFinally (g:TcGlobals) (e1, e2, m, ty, spTry, spFinally)          = 
    Expr.Op (TOp.TryFinally(spTry, spFinally), [ty], [mkDummyLambda g (e1, ty);mkDummyLambda g (e2, g.unit_ty)], m)

let mkDefault (m, ty) = Expr.Const(Const.Zero, m, ty) 

let mkValSet m v e = Expr.Op (TOp.LValueOp (LSet, v), [], [e], m)             
let mkAddrSet m v e = Expr.Op (TOp.LValueOp (LByrefSet, v), [], [e], m)       
let mkAddrGet m v = Expr.Op (TOp.LValueOp (LByrefGet, v), [], [], m)          
let mkValAddr m v = Expr.Op (TOp.LValueOp (LGetAddr, v), [], [], m)           

//--------------------------------------------------------------------------
// Maps tracking extra information for values
//--------------------------------------------------------------------------

[<NoEquality; NoComparison>]
type ValHash<'T> = 
    | ValHash of Dictionary<Stamp, 'T>

    member ht.Values = 
        let (ValHash t) = ht
        t.Values :> seq<'T>

    member ht.TryFind (v:Val) = 
        let (ValHash t) = ht
        match t.TryGetValue v.Stamp with
        | true, v -> Some v
        | _ -> None

    member ht.Add (v:Val, x) = 
        let (ValHash t) = ht
        t.[v.Stamp] <- x

    static member Create() = ValHash (new Dictionary<_, 'T>(11))

[<Struct; NoEquality; NoComparison>]
type ValMultiMap<'T>(contents: StampMap<'T list>) =
    member m.Find (v: Val) =
        match contents |> Map.tryFind v.Stamp with
        | Some vals -> vals
        | _ -> []

    member m.Add (v:Val, x) = ValMultiMap<'T>(contents.Add (v.Stamp, x :: m.Find v))
    member m.Remove (v: Val) = ValMultiMap<'T>(contents.Remove v.Stamp)
    member m.Contents = contents
    static member Empty = ValMultiMap<'T>(Map.empty)

[<Struct; NoEquality; NoComparison>]
type TyconRefMultiMap<'T>(contents: TyconRefMap<'T list>) =
    member m.Find v = 
        match contents.TryFind v with
        | Some vals -> vals
        | _ -> []

    member m.Add (v, x) = TyconRefMultiMap<'T>(contents.Add v (x :: m.Find v))
    static member Empty = TyconRefMultiMap<'T>(TyconRefMap<_>.Empty)
    static member OfList vs = (vs, TyconRefMultiMap<'T>.Empty) ||> List.foldBack (fun (x, y) acc -> acc.Add (x, y)) 


//--------------------------------------------------------------------------
// From Ref_private to Ref_nonlocal when exporting data.
//--------------------------------------------------------------------------

/// Try to create a EntityRef suitable for accessing the given Entity from another assembly 
let tryRescopeEntity viewedCcu (entity:Entity) : EntityRef option = 
    match entity.PublicPath with 
    | Some pubpath -> Some (ERefNonLocal (rescopePubPath viewedCcu pubpath))
    | None -> None


/// Try to create a ValRef suitable for accessing the given Val from another assembly 
let tryRescopeVal viewedCcu (entityRemap:Remap) (vspec:Val) : ValRef option = 
    match vspec.PublicPath with 
    | Some (ValPubPath(p, fullLinkageKey)) -> 
        // The type information in the val linkage doesn't need to keep any information to trait solutions.
        let entityRemap = { entityRemap with removeTraitSolutions = true }
        let fullLinkageKey = remapValLinkage entityRemap fullLinkageKey
        let vref = 
            // This compensates for the somewhat poor design decision in the F# compiler and metadata where
            // members are stored as values under the enclosing namespace/module rather than under the type.
            // This stems from the days when types and namespace/modules were separated constructs in the 
            // compiler implementation.
            if vspec.IsIntrinsicMember then  
                mkNonLocalValRef (rescopePubPathToParent viewedCcu p) fullLinkageKey
            else 
                mkNonLocalValRef (rescopePubPath viewedCcu p) fullLinkageKey
        Some vref
    | None -> None

    
//---------------------------------------------------------------------------
// Type information about records, constructors etc.
//---------------------------------------------------------------------------
 
let actualTyOfRecdField inst (fspec:RecdField)  = instType inst fspec.FormalType

let actualTysOfRecdFields inst rfields = List.map (actualTyOfRecdField inst) rfields

let actualTysOfInstanceRecdFields inst (tcref:TyconRef) = tcref.AllInstanceFieldsAsList |>  actualTysOfRecdFields inst 

let actualTysOfUnionCaseFields inst (x:UnionCaseRef) = actualTysOfRecdFields inst x.AllFieldsAsList

let actualResultTyOfUnionCase tinst (x:UnionCaseRef) = 
    instType (mkTyconRefInst x.TyconRef tinst) x.ReturnType

let recdFieldsOfExnDefRef x = (stripExnEqns x).TrueInstanceFieldsAsList
let recdFieldOfExnDefRefByIdx x n = (stripExnEqns x).GetFieldByIndex n

let recdFieldTysOfExnDefRef x = actualTysOfRecdFields [] (recdFieldsOfExnDefRef x)
let recdFieldTyOfExnDefRefByIdx x j = actualTyOfRecdField [] (recdFieldOfExnDefRefByIdx x j)


let actualTyOfRecdFieldForTycon tycon tinst (fspec:RecdField) = 
    instType (mkTyconInst tycon tinst) fspec.FormalType

let actualTyOfRecdFieldRef (fref:RecdFieldRef) tinst = 
    actualTyOfRecdFieldForTycon fref.Tycon tinst fref.RecdField

let actualTyOfUnionFieldRef (fref:UnionCaseRef) n tinst = 
    actualTyOfRecdFieldForTycon fref.Tycon tinst (fref.FieldByIndex(n))

    
//---------------------------------------------------------------------------
// Apply type functions to types
//---------------------------------------------------------------------------

let destForallTy g ty = 
    let tps, tau = primDestForallTy g ty 
    // tps may be have been equated to other tps in equi-recursive type inference 
    // and unit type inference. Normalize them here 
    let tps = NormalizeDeclaredTyparsForEquiRecursiveInference g tps
    tps, tau

let tryDestForallTy g ty = 
    if isForallTy g ty then destForallTy g ty else [], ty


let rec stripFunTy g ty = 
    if isFunTy g ty then 
        let (d, r) = destFunTy g ty 
        let more, rty = stripFunTy g r 
        d::more, rty
    else [], ty

let applyForallTy g ty tyargs = 
    let tps, tau = destForallTy g ty
    instType (mkTyparInst tps tyargs) tau

let reduceIteratedFunTy g ty args = 
    List.fold (fun ty _ -> 
        if not (isFunTy g ty) then failwith "reduceIteratedFunTy"
        snd (destFunTy g ty)) ty args

let applyTyArgs g functy tyargs = 
    if isForallTy g functy then applyForallTy g functy tyargs else functy

let applyTys g functy (tyargs, argtys) = 
    let afterTyappTy = applyTyArgs g functy tyargs
    reduceIteratedFunTy g afterTyappTy argtys

let formalApplyTys g functy (tyargs, args) = 
    reduceIteratedFunTy g
      (if isNil tyargs then functy else snd (destForallTy g functy))
      args

let rec stripFunTyN g n ty = 
    assert (n >= 0);
    if n > 0 && isFunTy g ty then 
        let (d, r) = destFunTy g ty
        let more, rty = stripFunTyN g (n-1) r in d::more, rty
    else [], ty

        
let tryDestAnyTupleTy g ty = 
    if isAnyTupleTy g ty then destAnyTupleTy g ty else tupInfoRef, [ty]

let tryDestRefTupleTy g ty = 
    if isRefTupleTy g ty then destRefTupleTy g ty else [ty]

type UncurriedArgInfos = (TType * ArgReprInfo) list 
type CurriedArgInfos = (TType * ArgReprInfo) list list

// A 'tau' type is one with its type parameters stripped off 
let GetTopTauTypeInFSharpForm g (curriedArgInfos: ArgReprInfo list list) tau m =
    let nArgInfos = curriedArgInfos.Length
    let argtys, rty = stripFunTyN g nArgInfos tau
    if nArgInfos <> argtys.Length then 
        error(Error(FSComp.SR.tastInvalidMemberSignature(), m))
    let argtysl = 
        (curriedArgInfos, argtys) ||> List.map2 (fun argInfos argty -> 
            match argInfos with 
            | [] -> [ (g.unit_ty, ValReprInfo.unnamedTopArg1) ]
            | [argInfo] -> [ (argty, argInfo) ]
            | _ -> List.zip (destRefTupleTy g argty) argInfos) 
    argtysl, rty

let destTopForallTy g (ValReprInfo (ntps, _, _)) ty =
    let tps, tau = (if isNil ntps then [], ty else tryDestForallTy g ty)
#if CHECKED
    if tps.Length <> kinds.Length then failwith (sprintf "destTopForallTy: internal error, #tps = %d, #ntps = %d" (List.length tps) ntps);
#endif
    // tps may be have been equated to other tps in equi-recursive type inference. Normalize them here 
    let tps = NormalizeDeclaredTyparsForEquiRecursiveInference g tps
    tps, tau

let GetTopValTypeInFSharpForm g (ValReprInfo(_, argInfos, retInfo) as topValInfo) ty m =
    let tps, tau = destTopForallTy g topValInfo ty
    let argtysl, rty = GetTopTauTypeInFSharpForm g argInfos tau m
    tps, argtysl, rty, retInfo


let IsCompiledAsStaticProperty g (v:Val) =
    match v.ValReprInfo with
    | Some valReprInfoValue ->
         match GetTopValTypeInFSharpForm g valReprInfoValue v.Type v.Range with 
         | [], [], _, _ when not v.IsMember -> true
         | _ -> false
    | _ -> false

let IsCompiledAsStaticPropertyWithField g (v:Val) = 
    (not v.IsCompiledAsStaticPropertyWithoutField && IsCompiledAsStaticProperty g v) 

//-------------------------------------------------------------------------
// Multi-dimensional array types...
//-------------------------------------------------------------------------

let isArrayTyconRef (g:TcGlobals) tcr =
    g.il_arr_tcr_map
    |> Array.exists (tyconRefEq g tcr)

let rankOfArrayTyconRef (g:TcGlobals) tcr =
    match g.il_arr_tcr_map |> Array.tryFindIndex (tyconRefEq g tcr) with
    | Some idx ->
        idx + 1
    | None ->
        failwith "rankOfArrayTyconRef: unsupported array rank"

//-------------------------------------------------------------------------
// Misc functions on F# types
//------------------------------------------------------------------------- 

let destArrayTy (g:TcGlobals) ty =
    match ty with
    | AppTy g (tcref, [ty]) when isArrayTyconRef g tcref -> ty
    | _ -> failwith "destArrayTy"

let destListTy (g:TcGlobals) ty =
    match ty with
    | AppTy g (tcref, [ty]) when tyconRefEq g tcref g.list_tcr_canon -> ty
    | _ -> failwith "destListTy"

let isTypeConstructorEqualToOptional g tcOpt tc = 
    match tcOpt with
    | None -> false
    | Some tc2 -> tyconRefEq g tc2 tc

let isByrefLikeTyconRef g tcref = 
    tyconRefEq g g.byref_tcr tcref ||
    isTypeConstructorEqualToOptional g g.system_TypedReference_tcref tcref ||
    isTypeConstructorEqualToOptional g g.system_ArgIterator_tcref tcref ||
    isTypeConstructorEqualToOptional g g.system_RuntimeArgumentHandle_tcref tcref

let isStringTy  g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> tyconRefEq g tcref g.system_String_tcref   | _ -> false)
let isListTy    g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> tyconRefEq g tcref g.list_tcr_canon   | _ -> false)
let isArrayTy   g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> isArrayTyconRef g tcref                | _ -> false) 
let isArray1DTy  g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> tyconRefEq g tcref g.il_arr_tcr_map.[0]  | _ -> false) 
let isUnitTy     g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> tyconRefEq g g.unit_tcr_canon tcref      | _ -> false) 
let isObjTy      g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> tyconRefEq g g.system_Object_tcref tcref | _ -> false) 
let isVoidTy     g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> tyconRefEq g g.system_Void_tcref tcref   | _ -> false) 
let isILAppTy    g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> tcref.IsILTycon                          | _ -> false) 
let isNativePtrTy    g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> tyconRefEq g g.nativeptr_tcr tcref           | _ -> false) 
let isByrefTy    g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> tyconRefEq g g.byref_tcr tcref           | _ -> false) 
let isByrefLikeTy g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> isByrefLikeTyconRef g tcref          | _ -> false) 
#if !NO_EXTENSIONTYPING
let extensionInfoOfTy g ty = ty |> stripTyEqns g |> (function TType_app(tcref, _) -> tcref.TypeReprInfo                | _ -> TNoRepr) 
#endif

type TypeDefMetadata = 
     | ILTypeMetadata of TILObjectReprData
     | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata 
#if !NO_EXTENSIONTYPING
     | ProvidedTypeMetadata of  TProvidedTypeInfo
#endif

let metadataOfTycon (tycon:Tycon) = 
#if !NO_EXTENSIONTYPING
    match tycon.TypeReprInfo with 
    | TProvidedTypeExtensionPoint info -> ProvidedTypeMetadata info
    | _ -> 
#endif
    if tycon.IsILTycon then 
       ILTypeMetadata tycon.ILTyconInfo
    else 
       FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata 


let metadataOfTy g ty = 
#if !NO_EXTENSIONTYPING
    match extensionInfoOfTy g ty with 
    | TProvidedTypeExtensionPoint info -> ProvidedTypeMetadata info
    | _ -> 
#endif
    if isILAppTy g ty then 
        let tcref = tcrefOfAppTy g ty
        ILTypeMetadata tcref.ILTyconInfo
    else 
        FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata 


let isILReferenceTy g ty = 
    match metadataOfTy g ty with 
#if !NO_EXTENSIONTYPING
    | ProvidedTypeMetadata info -> not info.IsStructOrEnum
#endif
    | ILTypeMetadata (TILObjectReprData(_, _, td)) -> not td.IsStructOrEnum
    | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata -> isArrayTy g ty

let isILInterfaceTycon (tycon:Tycon) = 
    match metadataOfTycon tycon with 
#if !NO_EXTENSIONTYPING
    | ProvidedTypeMetadata info -> info.IsInterface
#endif
    | ILTypeMetadata (TILObjectReprData(_, _, td)) -> td.IsInterface
    | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata -> false

let rankOfArrayTy g ty = rankOfArrayTyconRef g (tcrefOfAppTy g ty)

let isFSharpObjModelRefTy g ty = 
    isFSharpObjModelTy g ty && 
    let tcr, _ = destAppTy g ty
    match tcr.FSharpObjectModelTypeInfo.fsobjmodel_kind with 
    | TTyconClass | TTyconInterface   | TTyconDelegate _ -> true
    | TTyconStruct | TTyconEnum -> false

let isFSharpClassTy g ty =
    match tryDestAppTy g ty with
    | Some tcref -> tcref.Deref.IsFSharpClassTycon
    | _ -> false

let isFSharpStructTy g ty =
    match tryDestAppTy g ty with
    | Some tcref -> tcref.Deref.IsFSharpStructOrEnumTycon
    | _ -> false

let isFSharpInterfaceTy g ty = 
    match tryDestAppTy g ty with
    | Some tcref -> tcref.Deref.IsFSharpInterfaceTycon
    | _ -> false

let isDelegateTy g ty = 
    match metadataOfTy g ty with 
#if !NO_EXTENSIONTYPING
    | ProvidedTypeMetadata info -> info.IsDelegate ()
#endif
    | ILTypeMetadata (TILObjectReprData(_, _, td)) -> td.IsDelegate
    | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata ->
        match tryDestAppTy g ty with
        | Some tcref -> tcref.Deref.IsFSharpDelegateTycon
        | _ -> false

let isInterfaceTy g ty = 
    match metadataOfTy g ty with 
#if !NO_EXTENSIONTYPING
    | ProvidedTypeMetadata info -> info.IsInterface
#endif
    | ILTypeMetadata (TILObjectReprData(_, _, td)) -> td.IsInterface
    | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata -> isFSharpInterfaceTy g ty

let isClassTy g ty = 
    match metadataOfTy g ty with 
#if !NO_EXTENSIONTYPING
    | ProvidedTypeMetadata info -> info.IsClass
#endif
    | ILTypeMetadata (TILObjectReprData(_, _, td)) -> td.IsClass
    | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata -> isFSharpClassTy g ty

let isStructOrEnumTyconTy g ty = 
    match tryDestAppTy g ty with
    | Some tcref -> tcref.Deref.IsStructOrEnumTycon
    | _ -> false

let isStructRecordOrUnionTyconTy g ty = 
    match tryDestAppTy g ty with
    | Some tcref -> tcref.Deref.IsStructRecordOrUnionTycon
    | _ -> false

let isStructTy g ty =
    match tryDestAppTy g ty with
    | Some tcref -> 
        let tycon = tcref.Deref
        tycon.IsStructRecordOrUnionTycon || tycon.IsStructOrEnumTycon
    | _ -> false

let isRefTy g ty = 
    not (isStructOrEnumTyconTy g ty) &&
    (
        isUnionTy g ty || 
        isRefTupleTy g ty || 
        isRecdTy g ty || 
        isILReferenceTy g ty ||
        isFunTy g ty || 
        isReprHiddenTy g ty || 
        isFSharpObjModelRefTy g ty || 
        isUnitTy g ty
    )

// ECMA C# LANGUAGE SPECIFICATION, 27.2
// An unmanaged-type is any type that isn't a reference-type, a type-parameter, or a generic struct-type and
// contains no fields whose type is not an unmanaged-type. In other words, an unmanaged-type is one of the
// following:
// - sbyte, byte, short, ushort, int, uint, long, ulong, char, float, double, decimal, or bool.
// - Any enum-type.
// - Any pointer-type.
// - Any non-generic user-defined struct-type that contains fields of unmanaged-types only.
// [Note: Constructed types and type-parameters are never unmanaged-types. end note]
let rec isUnmanagedTy g ty =
    let ty = stripTyEqnsAndMeasureEqns g ty
    match tryDestAppTy g ty with
    | Some tcref ->
        let isEq tcref2  = tyconRefEq g tcref tcref2 
        if          isEq g.nativeptr_tcr || isEq g.nativeint_tcr ||
                    isEq g.sbyte_tcr || isEq g.byte_tcr || 
                    isEq g.int16_tcr || isEq g.uint16_tcr ||
                    isEq g.int32_tcr || isEq g.uint32_tcr ||
                    isEq g.int64_tcr || isEq g.uint64_tcr ||
                    isEq g.char_tcr ||
                    isEq g.float32_tcr ||
                    isEq g.float_tcr ||
                    isEq g.decimal_tcr ||
                    isEq g.bool_tcr then
            true
        else
            let tycon = tcref.Deref
            if tycon.IsEnumTycon then 
                true
            elif tycon.IsStructOrEnumTycon then
                match tycon.TyparsNoRange with
                | [] -> tycon.AllInstanceFieldsAsList |> List.forall (fun r -> isUnmanagedTy g r.rfield_type) 
                | _ -> false // generic structs are never 
            else false
    | None ->
        false

let isInterfaceTycon x = 
    isILInterfaceTycon x || x.IsFSharpInterfaceTycon

let isInterfaceTyconRef (tcref: TyconRef) = isInterfaceTycon tcref.Deref

let isEnumTy g ty = 
    match tryDestAppTy g ty with 
    | None -> false
    | Some tcref -> tcref.IsEnumTycon

let actualReturnTyOfSlotSig parentTyInst methTyInst (TSlotSig(_, _, parentFormalTypars, methFormalTypars, _, formalRetTy)) = 
    let methTyInst = mkTyparInst methFormalTypars methTyInst
    let parentTyInst = mkTyparInst parentFormalTypars parentTyInst
    Option.map (instType (parentTyInst @ methTyInst)) formalRetTy

let slotSigHasVoidReturnTy (TSlotSig(_, _, _, _, _, formalRetTy)) = 
    Option.isNone formalRetTy 

let returnTyOfMethod g (TObjExprMethod((TSlotSig(_, parentTy, _, _, _, _) as ss), _, methFormalTypars, _, _, _)) =
    let tinst = argsOfAppTy g parentTy
    let methTyInst = generalizeTypars methFormalTypars
    actualReturnTyOfSlotSig tinst methTyInst ss

/// Is the type 'abstract' in C#-speak
let isAbstractTycon (tycon:Tycon) = 
    if tycon.IsFSharpObjectModelTycon then 
        not tycon.IsFSharpDelegateTycon && 
        tycon.TypeContents.tcaug_abstract 
    else 
        tycon.IsILTycon && tycon.ILTyconRawMetadata.IsAbstract

//---------------------------------------------------------------------------
// Determine if a member/Val/ValRef is an explicit impl
//---------------------------------------------------------------------------

let MemberIsExplicitImpl g (membInfo:ValMemberInfo) = 
   membInfo.MemberFlags.IsOverrideOrExplicitImpl &&
   match membInfo.ImplementedSlotSigs with 
   | [] -> false
   | slotsigs -> slotsigs |> List.forall (fun slotsig -> isInterfaceTy g slotsig.ImplementedType)

let ValIsExplicitImpl g (v:Val) = 
    match v.MemberInfo with 
    | Some membInfo -> MemberIsExplicitImpl g membInfo
    | _ -> false

let ValRefIsExplicitImpl g (vref:ValRef) = ValIsExplicitImpl g vref.Deref

//---------------------------------------------------------------------------
// Find all type variables in a type, apart from those that have had 
// an equation assigned by type inference.
//---------------------------------------------------------------------------

let emptyFreeLocals = Zset.empty valOrder
let unionFreeLocals s1 s2 = 
    if s1 === emptyFreeLocals then s2
    elif s2 === emptyFreeLocals then s1
    else Zset.union s1 s2

let emptyFreeRecdFields = Zset.empty recdFieldRefOrder
let unionFreeRecdFields s1 s2 = 
    if s1 === emptyFreeRecdFields then s2
    elif s2 === emptyFreeRecdFields then s1
    else Zset.union s1 s2

let emptyFreeUnionCases = Zset.empty unionCaseRefOrder
let unionFreeUnionCases s1 s2 = 
    if s1 === emptyFreeUnionCases then s2
    elif s2 === emptyFreeUnionCases then s1
    else Zset.union s1 s2

let emptyFreeTycons = Zset.empty tyconOrder
let unionFreeTycons s1 s2 = 
    if s1 === emptyFreeTycons then s2
    elif s2 === emptyFreeTycons then s1
    else Zset.union s1 s2

let typarOrder = 
    { new System.Collections.Generic.IComparer<Typar> with 
        member x.Compare (v1:Typar, v2:Typar) = compare v1.Stamp v2.Stamp } 

let emptyFreeTypars = Zset.empty typarOrder
let unionFreeTypars s1 s2 = 
    if s1 === emptyFreeTypars then s2
    elif s2 === emptyFreeTypars then s1
    else Zset.union s1 s2

let emptyFreeTyvars =  
    { FreeTycons = emptyFreeTycons
      /// The summary of values used as trait solutions
      FreeTraitSolutions = emptyFreeLocals
      FreeTypars = emptyFreeTypars}

let unionFreeTyvars fvs1 fvs2 = 
    if fvs1 === emptyFreeTyvars then fvs2 else 
    if fvs2 === emptyFreeTyvars then fvs1 else
    { FreeTycons           = unionFreeTycons fvs1.FreeTycons fvs2.FreeTycons
      FreeTraitSolutions   = unionFreeLocals fvs1.FreeTraitSolutions fvs2.FreeTraitSolutions
      FreeTypars           = unionFreeTypars fvs1.FreeTypars fvs2.FreeTypars }

type FreeVarOptions = 
    { canCache: bool
      collectInTypes: bool
      includeLocalTycons: bool
      includeTypars: bool
      includeLocalTyconReprs: bool
      includeRecdFields : bool
      includeUnionCases : bool
      includeLocals : bool }
      
let CollectAllNoCaching = 
    { canCache = false
      collectInTypes = true
      includeLocalTycons = true
      includeLocalTyconReprs = true
      includeRecdFields = true
      includeUnionCases = true
      includeTypars = true; 
      includeLocals = true }

let CollectTyparsNoCaching = 
    { canCache = false
      collectInTypes = true
      includeLocalTycons = false
      includeTypars = true
      includeLocalTyconReprs = false
      includeRecdFields = false
      includeUnionCases = false
      includeLocals = false }

let CollectLocalsNoCaching = 
    { canCache = false
      collectInTypes = false
      includeLocalTycons = false
      includeTypars = false
      includeLocalTyconReprs = false
      includeRecdFields = false 
      includeUnionCases = false
      includeLocals = true }

let CollectTyparsAndLocalsNoCaching = 
    { canCache = false
      collectInTypes = true
      includeLocalTycons = false
      includeLocalTyconReprs = false
      includeRecdFields = false 
      includeUnionCases = false
      includeTypars = true
      includeLocals = true }

let CollectAll =
    { canCache = false
      collectInTypes = true
      includeLocalTycons = true
      includeLocalTyconReprs = true
      includeRecdFields = true 
      includeUnionCases = true
      includeTypars = true; 
      includeLocals = true }
    
let CollectTyparsAndLocals = // CollectAll
    { canCache = true // only cache for this one
      collectInTypes = true
      includeTypars = true
      includeLocals = true
      includeLocalTycons = false
      includeLocalTyconReprs = false
      includeRecdFields = false
      includeUnionCases = false }

  
let CollectTypars = CollectTyparsAndLocals

let CollectLocals = CollectTyparsAndLocals


let accFreeLocalTycon opts x acc = 
    if not opts.includeLocalTycons then acc else
    if Zset.contains x acc.FreeTycons then acc else 
    { acc with FreeTycons = Zset.add x acc.FreeTycons } 

let accFreeTycon opts (tcr:TyconRef) acc = 
    if not opts.includeLocalTycons then acc
    elif tcr.IsLocalRef then accFreeLocalTycon opts tcr.PrivateTarget acc
    else acc

let rec boundTypars opts tps acc = 
    // Bound type vars form a recursively-referential set due to constraints, e.g.  A : I<B>, B : I<A> 
    // So collect up free vars in all constraints first, then bind all variables 
    let acc = List.foldBack (fun (tp:Typar) acc -> accFreeInTyparConstraints opts tp.Constraints acc) tps acc
    List.foldBack (fun tp acc -> { acc with FreeTypars = Zset.remove tp acc.FreeTypars}) tps acc

and accFreeInTyparConstraints opts cxs acc =
    List.foldBack (accFreeInTyparConstraint opts) cxs acc

and accFreeInTyparConstraint opts tpc acc =
    match tpc with 
    | TyparConstraint.CoercesTo(typ, _) -> accFreeInType opts typ acc
    | TyparConstraint.MayResolveMember (traitInfo, _) -> accFreeInTrait opts traitInfo acc
    | TyparConstraint.DefaultsTo(_, rty, _) -> accFreeInType opts rty acc
    | TyparConstraint.SimpleChoice(tys, _) -> accFreeInTypes opts tys acc
    | TyparConstraint.IsEnum(uty, _) -> accFreeInType opts uty acc
    | TyparConstraint.IsDelegate(aty, bty, _) -> accFreeInType opts aty (accFreeInType opts bty acc)
    | TyparConstraint.SupportsComparison _
    | TyparConstraint.SupportsEquality _
    | TyparConstraint.SupportsNull _ 
    | TyparConstraint.IsNonNullableStruct _ 
    | TyparConstraint.IsReferenceType _ 
    | TyparConstraint.IsUnmanaged _
    | TyparConstraint.RequiresDefaultConstructor _ -> acc

and accFreeInTrait opts (TTrait(typs, _, _, argtys, rty, sln)) acc = 
    Option.foldBack (accFreeInTraitSln opts) sln.Value
       (accFreeInTypes opts typs 
         (accFreeInTypes opts argtys 
           (Option.foldBack (accFreeInType opts) rty acc)))

and accFreeInTraitSln opts sln acc = 
    match sln with 
    | ILMethSln(typ, _, _, minst) ->
         accFreeInType opts typ 
            (accFreeInTypes opts minst acc)
    | FSMethSln(typ, vref, minst) ->
         accFreeInType opts typ 
            (accFreeValRefInTraitSln opts vref  
               (accFreeInTypes opts minst acc))
    | FSRecdFieldSln(tinst, _rfref, _isSet) ->
         accFreeInTypes opts tinst acc
    | BuiltInSln -> acc
    | ClosedExprSln _ -> acc // nothing to accumulate because it's a closed expression referring only to erasure of provided method calls

and accFreeLocalValInTraitSln _opts v fvs =
    if Zset.contains v fvs.FreeTraitSolutions then fvs 
    else { fvs with FreeTraitSolutions = Zset.add v fvs.FreeTraitSolutions}

and accFreeValRefInTraitSln opts (vref:ValRef) fvs = 
    if vref.IsLocalRef then
        accFreeLocalValInTraitSln opts vref.PrivateTarget fvs
    else
        // non-local values do not contain free variables 
        fvs

and accFreeTyparRef opts (tp:Typar) acc = 
    if not opts.includeTypars then acc else
    if Zset.contains tp acc.FreeTypars then acc 
    else 
        accFreeInTyparConstraints opts tp.Constraints
          { acc with FreeTypars = Zset.add tp acc.FreeTypars}

and accFreeInType opts ty acc  = 
    match stripTyparEqns ty with 
    | TType_tuple (tupInfo, l) -> accFreeInTypes opts l (accFreeInTupInfo opts tupInfo acc)
    | TType_app (tc, tinst) -> 
        let acc = accFreeTycon opts tc acc
        match tinst with 
        | [] -> acc  // optimization to avoid unneeded call
        | [h] -> accFreeInType opts h acc // optimization to avoid unneeded call
        | _ -> accFreeInTypes opts tinst acc
    | TType_ucase (UCRef(tc, _), tinst) -> accFreeInTypes opts tinst (accFreeTycon opts tc  acc)
    | TType_fun (d, r) -> accFreeInType opts d (accFreeInType opts r acc)
    | TType_var r -> accFreeTyparRef opts r acc
    | TType_forall (tps, r) -> unionFreeTyvars (boundTypars opts tps (freeInType opts r)) acc
    | TType_measure unt -> accFreeInMeasure opts unt acc

and accFreeInTupInfo _opts unt acc = 
    match unt with 
    | TupInfo.Const _ -> acc
and accFreeInMeasure opts unt acc = List.foldBack (fun (tp, _) acc -> accFreeTyparRef opts tp acc) (ListMeasureVarOccsWithNonZeroExponents unt) acc
and accFreeInTypes opts tys acc = 
    match tys with 
    | [] -> acc
    | h :: t -> accFreeInTypes opts t (accFreeInType opts h acc)
and freeInType opts ty = accFreeInType opts ty emptyFreeTyvars

and accFreeInVal opts (v:Val) acc = accFreeInType opts v.val_type acc

let freeInTypes opts tys = accFreeInTypes opts tys emptyFreeTyvars
let freeInVal opts v = accFreeInVal opts v emptyFreeTyvars
let freeInTyparConstraints opts v = accFreeInTyparConstraints opts v emptyFreeTyvars
let accFreeInTypars opts tps acc = List.foldBack (accFreeTyparRef opts) tps acc
        

//--------------------------------------------------------------------------
// Free in type, left-to-right order preserved. This is used to determine the
// order of type variables for top-level definitions based on their signature, 
// so be careful not to change the order.  We accumulate in reverse
// order.
//--------------------------------------------------------------------------

let emptyFreeTyparsLeftToRight = []
let unionFreeTyparsLeftToRight fvs1 fvs2 = ListSet.unionFavourRight typarEq fvs1 fvs2

let rec boundTyparsLeftToRight g cxFlag thruFlag  acc tps = 
    // Bound type vars form a recursively-referential set due to constraints, e.g.  A : I<B>, B : I<A> 
    // So collect up free vars in all constraints first, then bind all variables 
    List.fold (fun acc (tp:Typar) -> accFreeInTyparConstraintsLeftToRight g cxFlag thruFlag acc tp.Constraints) tps acc

and accFreeInTyparConstraintsLeftToRight g cxFlag thruFlag acc cxs =
    List.fold (accFreeInTyparConstraintLeftToRight g cxFlag thruFlag) acc cxs 

and accFreeInTyparConstraintLeftToRight g cxFlag thruFlag acc tpc =
    match tpc with 
    | TyparConstraint.CoercesTo(typ, _) -> accFreeInTypeLeftToRight g cxFlag thruFlag acc typ 
    | TyparConstraint.MayResolveMember (traitInfo, _) -> accFreeInTraitLeftToRight g cxFlag thruFlag acc traitInfo 
    | TyparConstraint.DefaultsTo(_, rty, _) -> accFreeInTypeLeftToRight g cxFlag thruFlag acc rty 
    | TyparConstraint.SimpleChoice(tys, _) -> accFreeInTypesLeftToRight g cxFlag thruFlag acc tys 
    | TyparConstraint.IsEnum(uty, _) -> accFreeInTypeLeftToRight g cxFlag thruFlag acc uty
    | TyparConstraint.IsDelegate(aty, bty, _) -> accFreeInTypeLeftToRight g cxFlag thruFlag (accFreeInTypeLeftToRight g cxFlag thruFlag acc aty) bty  
    | TyparConstraint.SupportsComparison _ 
    | TyparConstraint.SupportsEquality _ 
    | TyparConstraint.SupportsNull _ 
    | TyparConstraint.IsNonNullableStruct _ 
    | TyparConstraint.IsUnmanaged _
    | TyparConstraint.IsReferenceType _ 
    | TyparConstraint.RequiresDefaultConstructor _ -> acc

and accFreeInTraitLeftToRight g cxFlag thruFlag acc (TTrait(typs, _, _, argtys, rty, _))  = 
    let acc = accFreeInTypesLeftToRight g cxFlag thruFlag acc typs
    let acc = accFreeInTypesLeftToRight g cxFlag thruFlag acc argtys
    let acc = Option.fold (accFreeInTypeLeftToRight g cxFlag thruFlag) acc rty
    acc

and accFreeTyparRefLeftToRight g cxFlag thruFlag acc (tp:Typar) = 
    if ListSet.contains typarEq tp acc then 
        acc
    else 
        let acc = ListSet.insert typarEq tp acc
        if cxFlag then 
            accFreeInTyparConstraintsLeftToRight g cxFlag thruFlag acc tp.Constraints
        else 
            acc

and accFreeInTypeLeftToRight g cxFlag thruFlag acc ty  = 
    if verbose then dprintf "--> accFreeInTypeLeftToRight \n"
    match (if thruFlag then stripTyEqns g ty else stripTyparEqns ty) with 
    | TType_tuple (tupInfo, l) -> 
        let acc = accFreeInTupInfoLeftToRight g cxFlag thruFlag acc tupInfo 
        accFreeInTypesLeftToRight g cxFlag thruFlag acc l 
    | TType_app (_, tinst) -> accFreeInTypesLeftToRight g cxFlag thruFlag acc tinst 
    | TType_ucase (_, tinst) -> accFreeInTypesLeftToRight g cxFlag thruFlag acc tinst 
    | TType_fun (d, r) -> accFreeInTypeLeftToRight g cxFlag thruFlag (accFreeInTypeLeftToRight g cxFlag thruFlag acc d ) r
    | TType_var r -> accFreeTyparRefLeftToRight g cxFlag thruFlag acc r 
    | TType_forall (tps, r) -> unionFreeTyparsLeftToRight (boundTyparsLeftToRight g cxFlag thruFlag tps (accFreeInTypeLeftToRight g cxFlag thruFlag emptyFreeTyparsLeftToRight r)) acc
    | TType_measure unt -> List.foldBack (fun (tp, _) acc -> accFreeTyparRefLeftToRight g cxFlag thruFlag acc tp) (ListMeasureVarOccsWithNonZeroExponents unt) acc

and accFreeInTupInfoLeftToRight _g _cxFlag _thruFlag acc unt = 
    match unt with 
    | TupInfo.Const _ -> acc

and accFreeInTypesLeftToRight g cxFlag thruFlag acc tys = 
    match tys with 
    | [] -> acc
    | h :: t -> accFreeInTypesLeftToRight g cxFlag thruFlag (accFreeInTypeLeftToRight g cxFlag thruFlag acc h) t
    
let freeInTypeLeftToRight g thruFlag ty = accFreeInTypeLeftToRight g true thruFlag emptyFreeTyparsLeftToRight ty |> List.rev
let freeInTypesLeftToRight g thruFlag ty = accFreeInTypesLeftToRight g true thruFlag emptyFreeTyparsLeftToRight ty |> List.rev
let freeInTypesLeftToRightSkippingConstraints g ty = accFreeInTypesLeftToRight g false true emptyFreeTyparsLeftToRight ty |> List.rev

let valOfBind (b:Binding) = b.Var
let valsOfBinds (binds:Bindings) = binds |> List.map (fun b -> b.Var)

//--------------------------------------------------------------------------
// Values representing member functions on F# types
//--------------------------------------------------------------------------

// Pull apart the type for an F# value that represents an object model method. Do not strip off a 'unit' argument.
// Review: Should GetMemberTypeInFSharpForm have any other direct callers? 
let GetMemberTypeInFSharpForm g memberFlags arities ty m = 
    let tps, argInfos, rty, retInfo = GetTopValTypeInFSharpForm g arities ty m

    let argInfos = 
        if memberFlags.IsInstance then 
            match argInfos with
            | [] -> 
                errorR(InternalError("value does not have a valid member type", m))
                argInfos
            | _::t -> t
        else argInfos
    tps, argInfos, rty, retInfo

// Check that an F# value represents an object model method. 
// It will also always have an arity (inferred from syntax). 
let checkMemberVal membInfo arity m =
    match membInfo, arity with 
    | None, _ -> error(InternalError("checkMemberVal - no membInfo" , m))
    | _, None -> error(InternalError("checkMemberVal - no arity", m))
    | Some membInfo, Some arity -> (membInfo, arity)

let checkMemberValRef (vref:ValRef) =
    checkMemberVal vref.MemberInfo vref.ValReprInfo vref.Range
     
let GetTopValTypeInCompiledForm g topValInfo typ m =
    let tps, paramArgInfos, rty, retInfo = GetTopValTypeInFSharpForm g topValInfo typ m
    // Eliminate lone single unit arguments
    let paramArgInfos = 
        match paramArgInfos, topValInfo.ArgInfos with 
        // static member and module value unit argument elimination
        | [[(_argType, _)]] , [[]] -> 
            //assert isUnitTy g argType 
            [[]]
        // instance member unit argument elimination
        | [objInfo;[(_argType, _)]] , [[_objArg];[]] -> 
            //assert isUnitTy g argType 
            [objInfo; []]
        | _ -> 
            paramArgInfos
    let rty = if isUnitTy g rty then None else Some rty
    (tps, paramArgInfos, rty, retInfo)
     
// Pull apart the type for an F# value that represents an object model method
// and see the "member" form for the type, i.e. 
// detect methods with no arguments by (effectively) looking for single argument type of 'unit'. 
// The analysis is driven of the inferred arity information for the value.
//
// This is used not only for the compiled form - it's also used for all type checking and object model
// logic such as determining if abstract methods have been implemented or not, and how
// many arguments the method takes etc.
let GetMemberTypeInMemberForm g memberFlags topValInfo typ m =
    let tps, paramArgInfos, rty, retInfo = GetMemberTypeInFSharpForm g memberFlags topValInfo typ m
    // Eliminate lone single unit arguments
    let paramArgInfos = 
        match paramArgInfos, topValInfo.ArgInfos with 
        // static member and module value unit argument elimination
        | [[(argType, _)]] , [[]] -> 
            assert isUnitTy g argType 
            [[]]
        // instance member unit argument elimination
        | [[(argType, _)]] , [[_objArg];[]] -> 
            assert isUnitTy g argType 
            [[]]
        | _ -> 
            paramArgInfos
    let rty = if isUnitTy g rty then None else Some rty
    (tps, paramArgInfos, rty, retInfo)

let GetTypeOfMemberInMemberForm g (vref:ValRef) =
    //assert (not vref.IsExtensionMember)
    let membInfo, topValInfo = checkMemberValRef vref
    GetMemberTypeInMemberForm g membInfo.MemberFlags topValInfo vref.Type vref.Range

let GetTypeOfMemberInFSharpForm g (vref:ValRef) =
    let membInfo, topValInfo = checkMemberValRef vref
    GetMemberTypeInFSharpForm g membInfo.MemberFlags topValInfo vref.Type vref.Range

let PartitionValTyparsForApparentEnclosingType g (v:Val)  = 
    match v.ValReprInfo with 
    | None -> error(InternalError("PartitionValTypars: not a top value", v.Range))
    | Some arities -> 
        let fullTypars, _ = destTopForallTy g arities v.Type 
        let parent = v.MemberApparentEntity
        let parentTypars = parent.TyparsNoRange
        let nparentTypars = parentTypars.Length
        if nparentTypars <= fullTypars.Length then 
            let memberParentTypars, memberMethodTypars = List.chop nparentTypars fullTypars
            let memberToParentInst, tinst = mkTyparToTyparRenaming memberParentTypars parentTypars
            Some(parentTypars, memberParentTypars, memberMethodTypars, memberToParentInst, tinst)
        else None

/// Match up the type variables on an member value with the type 
/// variables on the apparent enclosing type
let PartitionValTypars g (v:Val)  = 
     match v.ValReprInfo with 
     | None -> error(InternalError("PartitionValTypars: not a top value", v.Range))
     | Some arities -> 
         if v.IsExtensionMember then 
             let fullTypars, _ = destTopForallTy g arities v.Type 
             Some([], [], fullTypars, emptyTyparInst, [])
         else
             PartitionValTyparsForApparentEnclosingType g v

let PartitionValRefTypars g (vref: ValRef) = PartitionValTypars g vref.Deref 

/// Get the arguments for an F# value that represents an object model method 
let ArgInfosOfMemberVal g (v:Val) = 
    let membInfo, topValInfo = checkMemberVal v.MemberInfo v.ValReprInfo v.Range
    let _, arginfos, _, _ = GetMemberTypeInMemberForm g membInfo.MemberFlags topValInfo v.Type v.Range
    arginfos

let ArgInfosOfMember g (vref: ValRef) = 
    ArgInfosOfMemberVal g vref.Deref

let GetFSharpViewOfReturnType (g:TcGlobals) retTy =
    match retTy with 
    | None -> g.unit_ty
    | Some retTy ->  retTy


/// Get the property "type" (getter return type) for an F# value that represents a getter or setter
/// of an object model property.
let ReturnTypeOfPropertyVal g (v:Val) = 
    let membInfo, topValInfo = checkMemberVal v.MemberInfo v.ValReprInfo v.Range
    match membInfo.MemberFlags.MemberKind with 
    | MemberKind.PropertySet ->
        let _, arginfos, _, _ = GetMemberTypeInMemberForm g membInfo.MemberFlags topValInfo v.Type v.Range
        if not arginfos.IsEmpty && not arginfos.Head.IsEmpty then
            arginfos.Head |> List.last |> fst 
        else
            error(Error(FSComp.SR.tastValueDoesNotHaveSetterType(), v.Range));
    | MemberKind.PropertyGet ->
        let _, _, rty, _ = GetMemberTypeInMemberForm g membInfo.MemberFlags topValInfo v.Type v.Range
        GetFSharpViewOfReturnType g rty
    | _ -> error(InternalError("ReturnTypeOfPropertyVal", v.Range))


/// Get the property arguments for an F# value that represents a getter or setter
/// of an object model property.
let ArgInfosOfPropertyVal g (v:Val) = 
    let membInfo, topValInfo = checkMemberVal v.MemberInfo v.ValReprInfo v.Range
    match membInfo.MemberFlags.MemberKind with 
    | MemberKind.PropertyGet ->
        ArgInfosOfMemberVal g v |> List.concat
    | MemberKind.PropertySet ->
        let _, arginfos, _, _ = GetMemberTypeInMemberForm g membInfo.MemberFlags topValInfo v.Type v.Range
        if not arginfos.IsEmpty && not arginfos.Head.IsEmpty then
            arginfos.Head |> List.frontAndBack |> fst 
        else
            error(Error(FSComp.SR.tastValueDoesNotHaveSetterType(), v.Range));
    | _ -> 
        error(InternalError("ArgInfosOfPropertyVal", v.Range))

//---------------------------------------------------------------------------
// Generalize type constructors to types
//---------------------------------------------------------------------------

let generalTyconRefInst (tc:TyconRef) =  generalizeTypars tc.TyparsNoRange

let generalizeTyconRef tc = 
    let tinst = generalTyconRefInst tc
    tinst, TType_app(tc, tinst)

let generalizedTyconRef tc = TType_app(tc, generalTyconRefInst tc)

let isTTyparSupportsStaticMethod = function TyparConstraint.MayResolveMember _ -> true | _ -> false
let isTTyparCoercesToType = function TyparConstraint.CoercesTo _ -> true | _ -> false

//--------------------------------------------------------------------------
// Print Signatures/Types - prelude
//-------------------------------------------------------------------------- 

let prefixOfStaticReq s =
    match s with 
    | NoStaticReq -> "'"
    | HeadTypeStaticReq -> " ^"

let prefixOfRigidTypar (typar:Typar) =  
  if (typar.Rigidity <> TyparRigidity.Rigid) then "_" else ""

//---------------------------------------------------------------------------
// Prettify: PrettyTyparNames/PrettifyTypes - make typar names human friendly
//---------------------------------------------------------------------------

type TyparConstraintsWithTypars = (Typar * TyparConstraint) list

module PrettyTypes =
    let newPrettyTypar (tp:Typar) nm = 
        NewTypar (tp.Kind, tp.Rigidity, Typar(ident(nm, tp.Range), tp.StaticReq, false), false, TyparDynamicReq.Yes, [], false, false)

    let NewPrettyTypars renaming tps names = 
        let niceTypars = List.map2 newPrettyTypar tps names
        let tl, _tt = mkTyparToTyparRenaming tps niceTypars in
        let renaming = renaming @ tl
        (tps, niceTypars) ||> List.iter2 (fun tp tpnice -> tpnice.FixupConstraints (instTyparConstraints renaming tp.Constraints)) ;
        niceTypars, renaming

    // We choose names for type parameters from 'a'..'t'
    // We choose names for unit-of-measure from 'u'..'z'
    // If we run off the end of these ranges, we use 'aX' for positive integer X or 'uX' for positive integer X
    // Finally, we skip any names already in use
    let NeedsPrettyTyparName (tp:Typar) = 
        tp.IsCompilerGenerated && 
        tp.ILName.IsNone && 
        (tp.typar_id.idText = unassignedTyparName) 

    let PrettyTyparNames pred alreadyInUse tps = 
        let rec choose (tps:Typar list) (typeIndex, measureIndex) acc = 
            match tps with
            | [] -> List.rev acc
            | tp::tps ->
            

                // Use a particular name, possibly after incrementing indexes
                let useThisName (nm, typeIndex, measureIndex) = 
                    choose tps (typeIndex, measureIndex) (nm::acc)

                // Give up, try again with incremented indexes
                let tryAgain (typeIndex, measureIndex) = 
                    choose (tp::tps) (typeIndex, measureIndex) acc

                let tryName (nm, typeIndex, measureIndex) f = 
                    if List.contains nm alreadyInUse then 
                        f()
                    else
                        useThisName (nm, typeIndex, measureIndex)

                if pred tp then 
                    if NeedsPrettyTyparName tp then 
                        let (typeIndex, measureIndex, baseName, letters, i) = 
                          match tp.Kind with 
                          | TyparKind.Type -> (typeIndex+1, measureIndex, 'a', 20, typeIndex) 
                          | TyparKind.Measure -> (typeIndex, measureIndex+1, 'u', 6, measureIndex)
                        let nm = 
                           if i < letters then String.make 1 (char(int baseName + i)) 
                           else String.make 1 baseName + string (i-letters+1)
                        tryName (nm, typeIndex, measureIndex)  (fun () -> 
                            tryAgain (typeIndex, measureIndex))

                    else
                        tryName (tp.Name, typeIndex, measureIndex) (fun () -> 
                            // Use the next index and append it to the natural name
                            let (typeIndex, measureIndex, nm) = 
                              match tp.Kind with 
                              | TyparKind.Type -> (typeIndex+1, measureIndex, tp.Name+ string typeIndex) 
                              | TyparKind.Measure -> (typeIndex, measureIndex+1, tp.Name+ string measureIndex)
                            tryName (nm, typeIndex, measureIndex) (fun () -> 
                                tryAgain (typeIndex, measureIndex)))
                else
                    useThisName (tp.Name, typeIndex, measureIndex)

                          
        choose tps (0, 0) []

    let PrettifyThings g foldTys mapTys things = 
        let ftps = foldTys (accFreeInTypeLeftToRight g true false) emptyFreeTyparsLeftToRight things
        let ftps = List.rev ftps
        let rec computeKeep (keep: Typars) change (tps: Typars) = 
            match tps with 
            | [] -> List.rev keep, List.rev change 
            | tp :: rest -> 
                if not (NeedsPrettyTyparName tp) && (not (keep |> List.exists (fun tp2 -> tp.Name = tp2.Name)))  then
                    computeKeep (tp :: keep) change rest
                else 
                    computeKeep keep (tp :: change) rest
        let keep, change = computeKeep [] [] ftps
        
        // change |> List.iter (fun tp -> dprintf "change typar: %s %s %d\n" tp.Name (tp.DisplayName) (stamp_of_typar tp));  
        // keep |> List.iter (fun tp -> dprintf "keep typar: %s %s %d\n" tp.Name (tp.DisplayName) (stamp_of_typar tp));  
        let alreadyInUse = keep |> List.map (fun x -> x.Name)
        let names = PrettyTyparNames (fun x -> List.memq x change) alreadyInUse ftps

        let niceTypars, renaming = NewPrettyTypars [] ftps names 
        
        // strip universal types for printing
        let getTauStayTau t = 
            match t with
            | TType_forall (_, tau) -> tau
            | _ -> t
        let tauThings = mapTys getTauStayTau things
                        
        let prettyThings = mapTys (instType renaming) tauThings
        // niceTypars |> List.iter (fun tp -> dprintf "nice typar: %d\n" (stamp_of_typar tp)); *
        let tpconstraints  = niceTypars |> List.collect (fun tpnice -> List.map (fun tpc -> tpnice, tpc) tpnice.Constraints)

        prettyThings, tpconstraints

    let PrettifyType g x = PrettifyThings g id id x
    let PrettifyTypePair g x = PrettifyThings g (fun f -> foldPair (f, f)) (fun f -> mapPair (f, f)) x
    let PrettifyTypes g x = PrettifyThings g List.fold List.map   x
    let PrettifyCurriedTypes g x = PrettifyThings g (fun f -> List.fold (List.fold f)) List.mapSquared   x
    let PrettifyCurriedSigTypes g x = PrettifyThings g (fun f -> foldPair (List.fold (List.fold f), f)) (fun f -> mapPair (List.mapSquared f, f)) x

    // Badly formed code may instantiate rigid declared typars to types.
    // Hence we double check here that the thing is really a type variable
    let safeDestAnyParTy orig g ty = match tryAnyParTy g ty with None -> orig | Some x -> x
    let tee f x = f x x

    let foldUnurriedArgInfos f z (x: UncurriedArgInfos) = List.fold (fold1Of2 f) z x
    let mapUnurriedArgInfos f (x: UncurriedArgInfos) = List.map (map1Of2 f) x

    let foldTypar f z (x: Typar) = foldOn mkTyparTy f z x
    let mapTypar g f (x: Typar) : Typar = (mkTyparTy >> f >> safeDestAnyParTy x g) x

    let foldTypars f z (x: Typars) =  List.fold (foldTypar f) z x
    let mapTypars g f (x: Typars) : Typars = List.map (mapTypar g f) x

    let foldTyparInst f z (x: TyparInst) =  List.fold (foldPair (foldTypar f, f)) z x
    let mapTyparInst g f (x: TyparInst) : TyparInst = List.map (mapPair (mapTypar g f, f)) x

    let PrettifyInstAndTyparsAndType g x = 
        PrettifyThings g 
            (fun f -> foldTriple (foldTyparInst f, foldTypars f, f)) 
            (fun f-> mapTriple (mapTyparInst g f, mapTypars g f, f)) 
            x

    let PrettifyInstAndUncurriedSig  g (x: TyparInst * UncurriedArgInfos * TType) = 
        PrettifyThings g 
            (fun f -> foldTriple (foldTyparInst f, foldUnurriedArgInfos f, f)) 
            (fun f -> mapTriple (mapTyparInst g f, List.map (map1Of2  f), f))
            x

    let PrettifyInstAndCurriedSig g (x: TyparInst * TTypes * CurriedArgInfos * TType) = 
        PrettifyThings g 
            (fun f -> foldQuadruple (foldTyparInst f, List.fold f, List.fold (List.fold (fold1Of2 f)), f)) 
            (fun f -> mapQuadruple (mapTyparInst g f, List.map f, List.mapSquared (map1Of2 f), f))
            x

    let PrettifyInstAndSig g x = 
        PrettifyThings g 
            (fun f -> foldTriple (foldTyparInst f, List.fold f, f))
            (fun f -> mapTriple (mapTyparInst g f, List.map f, f) )
            x

    let PrettifyInstAndTypes g x = 
        PrettifyThings g 
            (fun f -> foldPair (foldTyparInst f, List.fold f)) 
            (fun f -> mapPair (mapTyparInst g f, List.map f))
            x
 
    let PrettifyInstAndType g x = 
        PrettifyThings g 
            (fun f -> foldPair (foldTyparInst f, f)) 
            (fun f -> mapPair (mapTyparInst g f, f))
            x
 
    let PrettifyInst g x = 
        PrettifyThings g 
            (fun f -> foldTyparInst f) 
            (fun f -> mapTyparInst g f)
            x
 
module SimplifyTypes =

    // CAREFUL! This function does NOT walk constraints 
    let rec foldTypeButNotConstraints f z typ =
        let typ = stripTyparEqns typ 
        let z = f z typ
        match typ with
        | TType_forall (_, body) -> foldTypeButNotConstraints f z body
        | TType_app (_, tinst) -> List.fold (foldTypeButNotConstraints f) z tinst
        | TType_ucase (_, tinst) -> List.fold (foldTypeButNotConstraints f) z tinst
        | TType_tuple (_, typs) -> List.fold (foldTypeButNotConstraints f) z typs
        | TType_fun (s, t)         -> foldTypeButNotConstraints f (foldTypeButNotConstraints f z s) t
        | TType_var _            -> z
        | TType_measure _          -> z

    let incM x m =
        if Zmap.mem x m then Zmap.add x (1 + Zmap.find x m) m
        else Zmap.add x 1 m

    let accTyparCounts z typ =
        // Walk type to determine typars and their counts (for pprinting decisions) 
        foldTypeButNotConstraints (fun z typ -> match typ with | TType_var tp when tp.Rigidity = TyparRigidity.Rigid  -> incM tp z | _ -> z) z typ

    let emptyTyparCounts = Zmap.empty typarOrder

    // print multiple fragments of the same type using consistent naming and formatting 
    let accTyparCountsMulti acc l = List.fold accTyparCounts acc l

    type TypeSimplificationInfo =
        { singletons         : Typar Zset
          inplaceConstraints :  Zmap<Typar, TType>
          postfixConstraints : (Typar * TyparConstraint) list }
          
    let typeSimplificationInfo0 = 
        { singletons         = Zset.empty typarOrder
          inplaceConstraints = Zmap.empty typarOrder
          postfixConstraints = [] }

    let categorizeConstraints simplify m cxs =
        let singletons = if simplify then Zmap.chooseL (fun tp n -> if n = 1 then Some tp else None) m else []
        let singletons = Zset.addList singletons (Zset.empty typarOrder)
        // Here, singletons are typars that occur once in the type.
        // However, they may also occur in a type constraint.
        // If they do, they are really multiple occurrence - so we should remove them.
        let constraintTypars = (freeInTyparConstraints CollectTyparsNoCaching (List.map snd cxs)).FreeTypars
        let usedInTypeConstraint typar = Zset.contains typar constraintTypars
        let singletons = singletons |> Zset.filter (usedInTypeConstraint >> not) 
        // Here, singletons should really be used once 
        let inplace, postfix =
          cxs |> List.partition (fun (tp, tpc) -> 
            simplify &&
            isTTyparCoercesToType tpc && 
            Zset.contains tp singletons && 
            tp.Constraints.Length = 1)
        let inplace = inplace |> List.map (function (tp, TyparConstraint.CoercesTo(ty, _)) -> tp, ty | _ -> failwith "not isTTyparCoercesToType")
        
        { singletons         = singletons
          inplaceConstraints = Zmap.ofList typarOrder inplace
          postfixConstraints = postfix }
    let CollectInfo simplify tys cxs = 
        categorizeConstraints simplify (accTyparCountsMulti emptyTyparCounts tys) cxs 

//--------------------------------------------------------------------------
// Print Signatures/Types
//-------------------------------------------------------------------------- 

[<NoEquality; NoComparison>]
type DisplayEnv = 
    { includeStaticParametersInTypeNames : bool
      openTopPathsSorted : Lazy<string list list>
      openTopPathsRaw : string list list
      shortTypeNames : bool
      suppressNestedTypes : bool
      maxMembers : int option
      showObsoleteMembers : bool
      showHiddenMembers : bool
      showTyparBinding : bool 
      showImperativeTyparAnnotations : bool
      suppressInlineKeyword : bool
      suppressMutableKeyword : bool
      showMemberContainers : bool
      shortConstraints : bool
      useColonForReturnType : bool
      showAttributes : bool
      showOverrides : bool
      showConstraintTyparAnnotations : bool
      abbreviateAdditionalConstraints : bool
      showTyparDefaultConstraints : bool
      g : TcGlobals
      contextAccessibility : Accessibility
      generatedValueLayout : (Val -> layout option) }

    member x.SetOpenPaths(paths) = 
        { x with 
             openTopPathsSorted = (lazy (paths |> List.sortWith (fun p1 p2 -> -(compare p1 p2))))
             openTopPathsRaw = paths 
        }

    static member Empty tcGlobals = 
      { includeStaticParametersInTypeNames = false
        openTopPathsRaw = []
        openTopPathsSorted = notlazy []
        shortTypeNames = false
        suppressNestedTypes = false
        maxMembers = None
        showObsoleteMembers = false
        showHiddenMembers = false
        showTyparBinding = false
        showImperativeTyparAnnotations = false
        suppressInlineKeyword = false
        suppressMutableKeyword = false
        showMemberContainers = false
        showAttributes = false
        showOverrides = true
        showConstraintTyparAnnotations = true
        abbreviateAdditionalConstraints = false
        showTyparDefaultConstraints = false
        shortConstraints = false
        useColonForReturnType = false
        g = tcGlobals
        contextAccessibility = taccessPublic
        generatedValueLayout = (fun _ -> None) }


    member denv.AddOpenPath path = 
        denv.SetOpenPaths (path :: denv.openTopPathsRaw)

    member denv.AddOpenModuleOrNamespace (modref: ModuleOrNamespaceRef) = 
        denv.AddOpenPath (fullCompPathOfModuleOrNamespace modref.Deref).DemangledPath

    member denv.AddAccessibility access =     
        { denv with contextAccessibility = combineAccess denv.contextAccessibility access }

let (+.+) s1 s2 = if s1 = "" then s2 else s1+"."+s2

let layoutOfPath p =     
    sepListL SepL.dot (List.map (tagNamespace >> wordL) p)

let fullNameOfParentOfPubPath pp = 
    match pp with 
    | PubPath([| _ |]) -> None 
    | pp -> Some(textOfPath pp.EnclosingPath)

let fullNameOfParentOfPubPathAsLayout pp = 
    match pp with 
    | PubPath([| _ |]) -> None 
    | pp -> Some(layoutOfPath (Array.toList pp.EnclosingPath))

let fullNameOfPubPath (PubPath(p)) = textOfPath p
let fullNameOfPubPathAsLayout (PubPath(p)) = layoutOfPath (Array.toList p)

let fullNameOfParentOfNonLocalEntityRef (nlr: NonLocalEntityRef) = 
    if nlr.Path.Length < 2 then None
    else Some (textOfPath nlr.EnclosingMangledPath)  // <--- BAD BAD BAD: this is a mangled path. This is wrong for nested modules

let fullNameOfParentOfNonLocalEntityRefAsLayout (nlr: NonLocalEntityRef) = 
    if nlr.Path.Length < 2 then None
    else Some (layoutOfPath (List.ofArray nlr.EnclosingMangledPath))  // <--- BAD BAD BAD: this is a mangled path. This is wrong for nested modules

let fullNameOfParentOfEntityRef eref = 
    match eref with 
    | ERefLocal x ->
         match x.PublicPath with 
         | None -> None
         | Some ppath -> fullNameOfParentOfPubPath ppath
    | ERefNonLocal nlr -> fullNameOfParentOfNonLocalEntityRef nlr

let fullNameOfParentOfEntityRefAsLayout eref = 
    match eref with 
    | ERefLocal x ->
         match x.PublicPath with 
         | None -> None
         | Some ppath -> fullNameOfParentOfPubPathAsLayout ppath
    | ERefNonLocal nlr -> fullNameOfParentOfNonLocalEntityRefAsLayout nlr

let fullNameOfEntityRef nmF xref = 
    match fullNameOfParentOfEntityRef xref  with 
    | None -> nmF xref 
    | Some pathText -> pathText +.+ nmF xref

let tagEntityRefName (xref: EntityRef) name =
    if xref.IsNamespace then tagNamespace name
    elif xref.IsModule then tagModule name
    elif xref.IsTypeAbbrev then tagAlias name
    elif xref.IsFSharpDelegateTycon then tagDelegate name
    elif xref.IsILEnumTycon || xref.IsFSharpEnumTycon then tagEnum name
    elif xref.IsStructOrEnumTycon then tagStruct name
    elif xref.IsFSharpInterfaceTycon then tagInterface name
    elif xref.IsUnionTycon then tagUnion name
    elif xref.IsRecordTycon then tagRecord name
    else tagClass name

let fullDisplayTextOfTyconRef  r = fullNameOfEntityRef (fun (tc:TyconRef) -> tc.DisplayNameWithStaticParametersAndUnderscoreTypars) r

let fullNameOfEntityRefAsLayout nmF (xref: EntityRef) =
    let navigableText = 
        tagEntityRefName xref (nmF xref)
        |> mkNav xref.DefinitionRange
        |> wordL
    match fullNameOfParentOfEntityRefAsLayout xref  with 
    | None -> navigableText
    | Some pathText -> pathText ^^ SepL.dot ^^ navigableText

let fullNameOfParentOfValRef vref = 
    match vref with 
    | VRefLocal x -> 
         match x.PublicPath with 
         | None -> None
         | Some (ValPubPath(pp, _)) -> Some(fullNameOfPubPath pp)
    | VRefNonLocal nlr -> 
        Some (fullNameOfEntityRef (fun (x:EntityRef) -> x.DemangledModuleOrNamespaceName) nlr.EnclosingEntity)

let fullNameOfParentOfValRefAsLayout vref = 
    match vref with 
    | VRefLocal x -> 
         match x.PublicPath with 
         | None -> None
         | Some (ValPubPath(pp, _)) -> Some(fullNameOfPubPathAsLayout pp)
    | VRefNonLocal nlr -> 
        Some (fullNameOfEntityRefAsLayout (fun (x:EntityRef) -> x.DemangledModuleOrNamespaceName) nlr.EnclosingEntity)


let fullDisplayTextOfParentOfModRef r = fullNameOfParentOfEntityRef r 

let fullDisplayTextOfModRef r = fullNameOfEntityRef (fun (x:EntityRef) -> x.DemangledModuleOrNamespaceName)  r
let fullDisplayTextOfTyconRefAsLayout  r = fullNameOfEntityRefAsLayout (fun (tc:TyconRef) -> tc.DisplayNameWithStaticParametersAndUnderscoreTypars) r
let fullDisplayTextOfExnRef  r = fullNameOfEntityRef (fun (tc:TyconRef) -> tc.DisplayNameWithStaticParametersAndUnderscoreTypars) r
let fullDisplayTextOfExnRefAsLayout  r = fullNameOfEntityRefAsLayout (fun (tc:TyconRef) -> tc.DisplayNameWithStaticParametersAndUnderscoreTypars) r

let fullDisplayTextOfUnionCaseRef (ucref:UnionCaseRef) = fullDisplayTextOfTyconRef ucref.TyconRef +.+ ucref.CaseName
let fullDisplayTextOfRecdFieldRef (rfref:RecdFieldRef) = fullDisplayTextOfTyconRef rfref.TyconRef +.+ rfref.FieldName

let fullDisplayTextOfValRef   (vref:ValRef) = 
    match fullNameOfParentOfValRef   vref  with 
    | None -> vref.DisplayName 
    | Some pathText -> pathText +.+ vref.DisplayName

let fullDisplayTextOfValRefAsLayout   (vref:ValRef) = 
    let n =
        match vref.MemberInfo with
        | None -> 
            if vref.IsModuleBinding then tagModuleBinding vref.DisplayName
            else tagUnknownEntity vref.DisplayName
        | Some memberInfo ->
            match memberInfo.MemberFlags.MemberKind with
            | MemberKind.PropertyGet
            | MemberKind.PropertySet
            | MemberKind.PropertyGetSet -> tagProperty vref.DisplayName
            | MemberKind.ClassConstructor
            | MemberKind.Constructor -> tagMethod vref.DisplayName
            | MemberKind.Member -> tagMember vref.DisplayName
    match fullNameOfParentOfValRefAsLayout vref  with 
    | None -> wordL n 
    | Some pathText -> 
        pathText ^^ SepL.dot ^^ wordL n
        //pathText +.+ vref.DisplayName


let fullMangledPathToTyconRef (tcref:TyconRef) = 
    match tcref with 
    | ERefLocal _ -> (match tcref.PublicPath with None -> [| |] | Some pp -> pp.EnclosingPath)
    | ERefNonLocal nlr -> nlr.EnclosingMangledPath
  
let qualifiedMangledNameOfTyconRef tcref nm = 
    String.concat "-" (Array.toList (fullMangledPathToTyconRef tcref) @ [ tcref.LogicalName + "-" + nm ])

let rec firstEq p1 p2 = 
    match p1 with
    | [] -> true 
    | h1::t1 -> 
        match p2 with 
        | h2::t2 -> h1 = h2 && firstEq t1 t2
        | _ -> false 

let rec firstRem p1 p2 = 
   match p1 with [] -> p2 | _::t1 -> firstRem t1 (List.tail p2)

let trimPathByDisplayEnv denv path =
    let findOpenedNamespace openedPath = 
        if firstEq openedPath path then 
            let t2 = firstRem openedPath path
            if t2 <> [] then Some(textOfPath t2 + ".")
            else Some("")
        else None

    match List.tryPick findOpenedNamespace (denv.openTopPathsSorted.Force()) with
    | Some s -> s
    | None ->  if isNil path then "" else textOfPath path + "."


let superOfTycon (g:TcGlobals) (tycon:Tycon) = 
    match tycon.TypeContents.tcaug_super with 
    | None -> g.obj_ty 
    | Some ty -> ty 

//----------------------------------------------------------------------------
// Detect attributes
//----------------------------------------------------------------------------

// AbsIL view of attributes (we read these from .NET binaries) 
let isILAttribByName (tencl:string list, tname: string) (attr: ILAttribute) = 
    (attr.Method.DeclaringType.TypeSpec.Name = tname) &&
    (attr.Method.DeclaringType.TypeSpec.Enclosing = tencl)

// AbsIL view of attributes (we read these from .NET binaries) 
let isILAttrib (tref:ILTypeRef) (attr: ILAttribute) = 
    isILAttribByName (tref.Enclosing, tref.Name) attr

// REVIEW: consider supporting querying on Abstract IL custom attributes.
// These linear iterations cost us a fair bit when there are lots of attributes
// on imported types. However this is fairly rare and can also be solved by caching the
// results of attribute lookups in the TAST
let HasILAttribute tref (attrs: ILAttributes) = Array.exists (isILAttrib tref) attrs.AsArray

let HasILAttributeByName tname (attrs: ILAttributes) = Array.exists (isILAttribByName ([], tname)) attrs.AsArray

let TryDecodeILAttribute (g:TcGlobals) tref (attrs: ILAttributes) = 
    attrs.AsArray |> Array.tryPick (fun x -> if isILAttrib tref x then Some(decodeILAttribData g.ilg x)  else None)

// This one is done by name to ensure the compiler doesn't take a dependency on dereferencing a type that only exists in .NET 3.5
let ILThingHasExtensionAttribute (attrs : ILAttributes) = 
    attrs.AsArray
    |> Array.exists (fun attr -> attr.Method.DeclaringType.TypeSpec.Name = "System.Runtime.CompilerServices.ExtensionAttribute")
    
// F# view of attributes (these get converted to AbsIL attributes in ilxgen) 
let IsMatchingFSharpAttribute g (AttribInfo(_, tcref)) (Attrib(tcref2, _, _, _, _, _, _)) = tyconRefEq g tcref  tcref2
let HasFSharpAttribute g tref attrs = List.exists (IsMatchingFSharpAttribute g tref) attrs
let findAttrib g tref attrs = List.find (IsMatchingFSharpAttribute g tref) attrs
let TryFindFSharpAttribute g tref attrs = List.tryFind (IsMatchingFSharpAttribute g tref) attrs
let TryFindFSharpAttributeOpt g tref attrs = match tref with None -> None | Some tref -> List.tryFind (IsMatchingFSharpAttribute g tref) attrs

let HasFSharpAttributeOpt g trefOpt attrs = match trefOpt with Some tref -> List.exists (IsMatchingFSharpAttribute g tref) attrs | _ -> false
let IsMatchingFSharpAttributeOpt g attrOpt (Attrib(tcref2, _, _, _, _, _, _)) = match attrOpt with Some ((AttribInfo(_, tcref))) -> tyconRefEq g tcref  tcref2 | _ -> false

let (|ExtractAttribNamedArg|_|) nm args = 
    args |> List.tryPick (function (AttribNamedArg(nm2, _, _, v)) when nm = nm2 -> Some v | _ -> None) 

let (|AttribInt32Arg|_|) = function AttribExpr(_, Expr.Const (Const.Int32(n), _, _)) -> Some(n) | _ -> None
let (|AttribInt16Arg|_|) = function AttribExpr(_, Expr.Const (Const.Int16(n), _, _)) -> Some(n) | _ -> None
let (|AttribBoolArg|_|) = function AttribExpr(_, Expr.Const (Const.Bool(n), _, _)) -> Some(n) | _ -> None
let (|AttribStringArg|_|) = function AttribExpr(_, Expr.Const (Const.String(n), _, _)) -> Some(n) | _ -> None

let TryFindFSharpBoolAttributeWithDefault dflt g nm attrs = 
    match TryFindFSharpAttribute g nm attrs with
    | Some(Attrib(_, _, [ ], _, _, _, _)) -> Some(dflt)
    | Some(Attrib(_, _, [ AttribBoolArg(b) ], _, _, _, _)) -> Some(b)
    | _ -> None

let TryFindFSharpBoolAttribute g nm attrs = TryFindFSharpBoolAttributeWithDefault true g nm attrs
let TryFindFSharpBoolAttributeAssumeFalse g nm attrs = TryFindFSharpBoolAttributeWithDefault false g nm attrs

let TryFindFSharpInt32Attribute g nm attrs = 
    match TryFindFSharpAttribute g nm attrs with
    | Some(Attrib(_, _, [ AttribInt32Arg(b) ], _, _, _, _)) -> Some b
    | _ -> None
    
let TryFindFSharpStringAttribute g nm attrs = 
    match TryFindFSharpAttribute g nm attrs with
    | Some(Attrib(_, _, [ AttribStringArg(b) ], _, _, _, _)) -> Some b
    | _ -> None
    
let TryFindILAttribute (AttribInfo (atref, _)) attrs = 
    HasILAttribute atref attrs

let TryFindILAttributeOpt attr attrs = 
    match attr with
    | Some (AttribInfo (atref, _)) -> HasILAttribute atref attrs
    | _ -> false

/// Analyze three cases for attributes declared on type definitions: IL-declared attributes, F#-declared attributes and
/// provided attributes.
//
// This is used for AttributeUsageAttribute, DefaultMemberAttribute and ConditionalAttribute (on attribute types)
let TryBindTyconRefAttribute g (m:range) (AttribInfo (atref, _) as args) (tcref:TyconRef) f1 f2 f3 = 
    ignore m; ignore f3
    match metadataOfTycon tcref.Deref with 
#if !NO_EXTENSIONTYPING
    | ProvidedTypeMetadata info -> 
        let provAttribs = info.ProvidedType.PApply((fun a -> (a :> IProvidedCustomAttributeProvider)), m)
        match provAttribs.PUntaint((fun a -> a.GetAttributeConstructorArgs(provAttribs.TypeProvider.PUntaintNoFailure(id), atref.FullName)), m) with
        | Some args -> f3 args
        | None -> None
#endif
    | ILTypeMetadata (TILObjectReprData(_, _, tdef)) -> 
        match TryDecodeILAttribute g atref tdef.CustomAttrs with 
        | Some attr -> f1 attr
        | _ -> None
    | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata -> 
        match TryFindFSharpAttribute g args tcref.Attribs with 
        | Some attr -> f2 attr
        | _ -> None

let TryFindTyconRefBoolAttribute g m attribSpec tcref  =
    TryBindTyconRefAttribute g m attribSpec tcref 
                (function 
                   | ([ ], _) -> Some true
                   | ([ILAttribElem.Bool (v) ], _) -> Some v 
                   | _ -> None)
                (function 
                   | (Attrib(_, _, [ ], _, _, _, _))  -> Some true
                   | (Attrib(_, _, [ AttribBoolArg v ], _, _, _, _))  -> Some v 
                   | _ -> None)
                (function 
                   | ([ ], _) -> Some true
                   | ([ Some ((:? bool as v) : obj) ], _) -> Some v 
                   | _ -> None)

let TryFindAttributeUsageAttribute g m tcref  =
    TryBindTyconRefAttribute g m g.attrib_AttributeUsageAttribute tcref 
                (fun (_, named)             -> named |> List.tryPick (function ("AllowMultiple", _, _, ILAttribElem.Bool res) -> Some res | _ -> None))
                (fun (Attrib(_, _, _, named, _, _, _)) -> named |> List.tryPick (function AttribNamedArg("AllowMultiple", _, _, AttribBoolArg(res) ) -> Some res | _ -> None))
                (fun (_, named)             -> named |> List.tryPick (function ("AllowMultiple", Some ((:? bool as res) : obj)) -> Some res | _ -> None))


/// Try to find a specific attribute on a type definition, where the attribute accepts a string argument.
///
/// This is used to detect the 'DefaultMemberAttribute' and 'ConditionalAttribute' attributes (on type definitions)
let TryFindTyconRefStringAttribute g m attribSpec tcref  =
    TryBindTyconRefAttribute g m attribSpec tcref 
                (function ([ILAttribElem.String (Some(msg)) ], _) -> Some msg | _ -> None)
                (function (Attrib(_, _, [ AttribStringArg(msg) ], _, _, _, _))  -> Some msg | _ -> None)
                (function ([ Some ((:? string as msg) : obj) ], _) -> Some msg | _ -> None)

/// Check if a type definition has a specific attribute
let TyconRefHasAttribute g m attribSpec tcref  =
    TryBindTyconRefAttribute g m attribSpec tcref 
                    (fun _ -> Some ()) 
                    (fun _ -> Some ())
                    (fun _ -> Some ())
        |> Option.isSome

//-------------------------------------------------------------------------
// List and reference types...
//------------------------------------------------------------------------- 

let destByrefTy g ty = 
    match ty |> stripTyEqns g with
    | TType_app(tcref, [x]) when tyconRefEq g g.byref_tcr tcref -> x
    | _ -> failwith "destByrefTy: not a byref type"

let destNativePtrTy g ty =
    match ty |> stripTyEqns g with
    | TType_app(tcref, [x]) when tyconRefEq g g.nativeptr_tcr tcref -> x
    | _ -> failwith "destNativePtrTy: not a native ptr type"

let isRefCellTy g ty   = 
    match tryDestAppTy g ty with 
    | None -> false
    | Some tcref -> tyconRefEq g g.refcell_tcr_canon tcref

let destRefCellTy g ty = 
    match ty |> stripTyEqns g with
    | TType_app(tcref, [x]) when tyconRefEq g g.refcell_tcr_canon tcref -> x
    | _ -> failwith "destRefCellTy: not a ref type"

let StripSelfRefCell(g:TcGlobals, baseOrThisInfo:ValBaseOrThisInfo, tau: TType) : TType =
    if baseOrThisInfo = CtorThisVal && isRefCellTy g tau 
        then destRefCellTy g tau 
        else tau

let mkRefCellTy (g:TcGlobals) ty = TType_app(g.refcell_tcr_nice, [ty])

let mkLazyTy (g:TcGlobals) ty = TType_app(g.lazy_tcr_nice, [ty])

let mkPrintfFormatTy (g:TcGlobals) aty bty cty dty ety = TType_app(g.format_tcr, [aty;bty;cty;dty; ety])

let mkOptionTy (g:TcGlobals) ty = TType_app (g.option_tcr_nice, [ty])

let mkListTy (g:TcGlobals) ty = TType_app (g.list_tcr_nice, [ty])

let isOptionTy (g:TcGlobals) ty = 
    match tryDestAppTy g ty with 
    | None -> false
    | Some tcref -> tyconRefEq g g.option_tcr_canon tcref

let tryDestOptionTy g ty = 
    match argsOfAppTy g ty with 
    | [ty1]  when isOptionTy g ty  -> Some ty1
    | _ -> None

let destOptionTy g ty = 
    match tryDestOptionTy g ty with 
    | Some ty -> ty
    | None -> failwith "destOptionTy: not an option type"

let isLinqExpressionTy g ty = 
    match tryDestAppTy g ty with 
    | None -> false
    | Some tcref -> tyconRefEq g g.system_LinqExpression_tcref tcref

let tryDestLinqExpressionTy g ty = 
    match argsOfAppTy g ty with 
    | [ty1]  when isLinqExpressionTy g ty  -> Some ty1
    | _ -> None

let destLinqExpressionTy g ty = 
    match tryDestLinqExpressionTy g ty with 
    | Some ty -> ty
    | None -> failwith "destLinqExpressionTy: not an expression type"

let mkNoneCase (g:TcGlobals) = mkUnionCaseRef g.option_tcr_canon "None"
let mkSomeCase (g:TcGlobals) = mkUnionCaseRef g.option_tcr_canon "Some"

type ValRef with 
    member vref.IsDispatchSlot = 
        match vref.MemberInfo with 
        | Some membInfo -> membInfo.MemberFlags.IsDispatchSlot 
        | None -> false

let (|UnopExpr|_|) _g expr = 
    match expr with 
    | Expr.App(Expr.Val(vref, _, _), _, _, [arg1], _) -> Some (vref, arg1)
    | _ -> None

let (|BinopExpr|_|) _g expr = 
    match expr with 
    | Expr.App(Expr.Val(vref, _, _), _, _, [arg1;arg2], _) -> Some (vref, arg1, arg2)
    | _ -> None

let (|SpecificUnopExpr|_|)  g vrefReqd expr = 
    match expr with 
    | UnopExpr g (vref, arg1) when valRefEq g vref vrefReqd  -> Some arg1
    | _ -> None

let (|SpecificBinopExpr|_|) g vrefReqd expr = 
    match expr with 
    | BinopExpr g (vref, arg1, arg2) when valRefEq g vref vrefReqd -> Some (arg1, arg2)
    | _ -> None

let (|EnumExpr|_|) g expr = 
    match (|SpecificUnopExpr|_|) g g.enum_vref expr with
    | None -> (|SpecificUnopExpr|_|) g g.enumOfValue_vref expr
    | x -> x

let (|BitwiseOrExpr|_|) g expr = (|SpecificBinopExpr|_|) g g.bitwise_or_vref expr

let (|AttribBitwiseOrExpr|_|) g expr = 
    match expr with 
    | BitwiseOrExpr g (arg1, arg2) -> Some(arg1, arg2)
    // Special workaround, only used when compiling FSharp.Core.dll. Uses of 'a ||| b' occur before the '|||' bitwise or operator
    // is defined. These get through type checking because enums implicitly support the '|||' operator through
    // the automatic resolution of undefined operators (see tc.fs, Item.ImplicitOp). This then compiles as an 
    // application of a lambda to two arguments. We recognize this pattern here
    | Expr.App(Expr.Lambda _, _, _, [arg1;arg2], _) when g.compilingFslib  -> 
        Some(arg1, arg2)
    | _ -> None

let isUncheckedDefaultOfValRef g vref = 
    valRefEq g vref g.unchecked_defaultof_vref 
    // There is an internal version of typeof defined in prim-types.fs that needs to be detected
    || (g.compilingFslib && vref.LogicalName = "defaultof") 

let isTypeOfValRef g vref = 
    valRefEq g vref g.typeof_vref 
    // There is an internal version of typeof defined in prim-types.fs that needs to be detected
    || (g.compilingFslib && vref.LogicalName = "typeof") 

let isSizeOfValRef g vref = 
    valRefEq g vref g.sizeof_vref 
    // There is an internal version of typeof defined in prim-types.fs that needs to be detected
    || (g.compilingFslib && vref.LogicalName = "sizeof") 

let isTypeDefOfValRef g vref = 
    valRefEq g vref g.typedefof_vref 
    // There is an internal version of typedefof defined in prim-types.fs that needs to be detected
    || (g.compilingFslib && vref.LogicalName = "typedefof") 

let (|UncheckedDefaultOfExpr|_|) g expr = 
    match expr with 
    | Expr.App(Expr.Val(vref, _, _), _, [ty], [], _) when isUncheckedDefaultOfValRef g vref ->  Some ty
    | _ -> None

let (|TypeOfExpr|_|) g expr = 
    match expr with 
    | Expr.App(Expr.Val(vref, _, _), _, [ty], [], _) when isTypeOfValRef g vref ->  Some ty
    | _ -> None

let (|SizeOfExpr|_|) g expr = 
    match expr with 
    | Expr.App(Expr.Val(vref, _, _), _, [ty], [], _) when isSizeOfValRef g vref ->  Some ty
    | _ -> None

let (|TypeDefOfExpr|_|) g expr = 
    match expr with 
    | Expr.App(Expr.Val(vref, _, _), _, [ty], [], _) when isTypeDefOfValRef g vref ->  Some ty
    | _ -> None



//--------------------------------------------------------------------------
// DEBUG layout
//---------------------------------------------------------------------------

module DebugPrint = begin
    open Microsoft.FSharp.Compiler.Layout
    open PrettyTypes
    let layoutRanges = ref false  

    let squareAngleL x = LeftL.leftBracketAngle ^^ x ^^ RightL.rightBracketAngle
    let angleL x = sepL Literals.leftAngle ^^ x ^^ rightL Literals.rightAngle
    let braceL x = leftL Literals.leftBrace  ^^ x ^^ rightL Literals.rightBrace
    let boolL = function true -> WordL.keywordTrue | false -> WordL.keywordFalse

    let intL (n:int)          = wordL (tagNumericLiteral (string n ))
    let int64L (n:int64)          = wordL (tagNumericLiteral (string n ))

    let jlistL xL xmap = QueueList.foldBack (fun x z -> z @@ xL x) xmap emptyL

    let bracketIfL x lyt = if x then bracketL lyt else lyt

    let lvalopL x = 
        match x with 
        | LGetAddr  -> wordL (tagText "LGetAddr")
        | LByrefGet -> wordL (tagText "LByrefGet")
        | LSet      -> wordL (tagText "LSet")
        | LByrefSet -> wordL (tagText "LByrefSet")

    let angleBracketL l = leftL (tagText "<") ^^ l ^^ rightL (tagText ">")
    let angleBracketListL l = angleBracketL (sepListL (sepL (tagText ",")) l)


    let layoutMemberFlags memFlags = 
        let stat = if memFlags.IsInstance || (memFlags.MemberKind = MemberKind.Constructor) then emptyL else wordL (tagText "static")
        let stat = if memFlags.IsDispatchSlot then stat ++ wordL (tagText "abstract")
                   elif memFlags.IsOverrideOrExplicitImpl then stat ++ wordL (tagText "override")
                   else stat
        stat

    let stampL _n w = 
        w

    let layoutTyconRef (tc:TyconRef) = wordL (tagText tc.DisplayNameWithStaticParameters) |> stampL tc.Stamp


    let rec auxTypeL env typ = auxTypeWrapL env false typ

    and auxTypeAtomL env typ = auxTypeWrapL env true  typ

    and auxTyparsL env tcL prefix tinst = 
       match tinst with 
       | [] -> tcL
       | [t] -> 
         let tL = auxTypeAtomL env t
         if prefix then        tcL ^^ angleBracketL tL 
         else            tL ^^ tcL 
       | _ -> 
         let tinstL = List.map (auxTypeL env) tinst
         if prefix then                   
             tcL ^^ angleBracketListL tinstL
         else  
             tupleL tinstL ^^ tcL
            
    and auxTypeWrapL env isAtomic typ = 
        let wrap x = bracketIfL isAtomic x in // wrap iff require atomic expr 
        match stripTyparEqns typ with
        | TType_forall (typars, rty) -> 
           (leftL (tagText "!") ^^ layoutTyparDecls typars --- auxTypeL env rty) |> wrap
        | TType_ucase (UCRef(tcref, _), tinst)  
        | TType_app (tcref, tinst)   -> 
           let prefix = tcref.IsPrefixDisplay
           let tcL = layoutTyconRef tcref
           auxTyparsL env tcL prefix tinst
        | TType_tuple (_tupInfo, typs) -> sepListL (wordL (tagText "*")) (List.map (auxTypeAtomL env) typs) |> wrap
        | TType_fun (f, x)           -> ((auxTypeAtomL env f ^^ wordL (tagText "->")) --- auxTypeL env x) |> wrap
        | TType_var typar           -> auxTyparWrapL env isAtomic typar 
        | TType_measure unt -> 
#if DEBUG
          leftL (tagText "{") ^^
          (match !global_g with
           | None -> wordL (tagText "<no global g>")
           | Some g -> 
             let sortVars (vs:(Typar * Rational) list) = vs |> List.sortBy (fun (v, _) -> v.DisplayName) 
             let sortCons (cs:(TyconRef * Rational) list) = cs |> List.sortBy (fun (c, _) -> c.DisplayName) 
             let negvs, posvs = ListMeasureVarOccsWithNonZeroExponents         unt |> sortVars |> List.partition (fun (_, e) -> SignRational e < 0)
             let negcs, poscs = ListMeasureConOccsWithNonZeroExponents g false unt |> sortCons |> List.partition (fun (_, e) -> SignRational e < 0)
             let unparL (uv:Typar) = wordL (tagText ("'" ^  uv.DisplayName))
             let unconL tc = layoutTyconRef tc
             let rationalL e = wordL (tagText(RationalToString e))
             let measureToPowerL x e = if e = OneRational then x else x -- wordL (tagText "^") -- rationalL e
             let prefix = spaceListL  (List.map (fun (v, e) -> measureToPowerL (unparL v) e) posvs @
                                       List.map (fun (c, e) -> measureToPowerL (unconL c) e) poscs)
             let postfix = spaceListL (List.map (fun (v, e) -> measureToPowerL (unparL v) (NegRational e)) negvs @
                                       List.map (fun (c, e) -> measureToPowerL (unconL c) (NegRational e)) negcs)
             match (negvs, negcs) with 
             | [], [] -> prefix 
             | _ -> prefix ^^ sepL (tagText "/") ^^ postfix) ^^
          rightL (tagText "}")
#else
          unt |> ignore
          wordL(tagText "<measure>")
#endif

    and auxTyparWrapL (env:SimplifyTypes.TypeSimplificationInfo) isAtomic (typar:Typar) =
          let wrap x = bracketIfL isAtomic x in // wrap iff require atomic expr 
          // There are several cases for pprinting of typar.
          // 
          //   'a              - is multiple  occurrence.
          //   #Type           - inplace coercion constraint and singleton
          //   ('a :> Type)    - inplace coercion constraint not singleton
          //   ('a.opM : S->T) - inplace operator constraint
          let tpL =
            wordL (tagText (prefixOfStaticReq typar.StaticReq
                   + prefixOfRigidTypar typar
                   + typar.DisplayName))
          let varL = tpL |> stampL typar.Stamp 

          match Zmap.tryFind typar env.inplaceConstraints with
          | Some (typarConstrTyp) ->
              if Zset.contains typar env.singletons then
                leftL (tagText "#") ^^ auxTyparConstraintTypL env typarConstrTyp
              else
                (varL ^^ sepL (tagText ":>") ^^ auxTyparConstraintTypL env typarConstrTyp) |> wrap
          | _ -> varL

    and auxTypar2L     env typar = auxTyparWrapL env false typar

    and auxTyparAtomL env typar = auxTyparWrapL env true  typar

    and auxTyparConstraintTypL env ty = auxTypeL env ty

    and auxTraitL env (ttrait: TraitConstraintInfo) =
#if DEBUG
        let (TTrait(tys, nm, memFlags, argtys, rty, _)) = ttrait 
        match !global_g with
        | None -> wordL (tagText "<no global g>")
        | Some g -> 
            let rty = GetFSharpViewOfReturnType g rty
            let stat = layoutMemberFlags memFlags
            let argsL = sepListL (wordL (tagText "*")) (List.map (auxTypeAtomL env) argtys)
            let resL  = auxTypeL env rty
            let methodTypeL = (argsL ^^ wordL (tagText "->")) ++ resL
            bracketL (stat ++ bracketL (sepListL (wordL (tagText "or")) (List.map (auxTypeAtomL env) tys)) ++ wordL (tagText "member") --- (wordL (tagText nm) ^^ wordL (tagText ":") -- methodTypeL))
#else
        ignore (env, ttrait)
        wordL(tagText "trait")
#endif

    and auxTyparConstraintL env (tp, tpc) = 
        let constraintPrefix l = auxTypar2L env tp ^^ wordL (tagText ":") ^^ l
        match tpc with
        | TyparConstraint.CoercesTo(typarConstrTyp, _) ->
            auxTypar2L env tp ^^ wordL (tagText ":>") --- auxTyparConstraintTypL env typarConstrTyp
        | TyparConstraint.MayResolveMember(traitInfo, _) ->
            auxTypar2L env tp ^^ wordL (tagText ":")  --- auxTraitL env traitInfo
        | TyparConstraint.DefaultsTo(_, ty, _) ->
            wordL (tagText  "default") ^^ auxTypar2L env tp ^^ wordL (tagText ":") ^^ auxTypeL env ty
        | TyparConstraint.IsEnum(ty, _) ->
            auxTyparsL env (wordL (tagText  "enum")) true [ty] |> constraintPrefix
        | TyparConstraint.IsDelegate(aty, bty, _) ->
            auxTyparsL env (wordL (tagText "delegate")) true [aty; bty] |> constraintPrefix
        | TyparConstraint.SupportsNull _ ->
            wordL (tagText "null") |> constraintPrefix
        | TyparConstraint.SupportsComparison _ ->
            wordL (tagText "comparison") |> constraintPrefix
        | TyparConstraint.SupportsEquality _ ->
            wordL (tagText "equality") |> constraintPrefix
        | TyparConstraint.IsNonNullableStruct _ ->
            wordL (tagText  "struct") |> constraintPrefix
        | TyparConstraint.IsReferenceType _ ->
            wordL (tagText "not struct") |> constraintPrefix
        | TyparConstraint.IsUnmanaged _ ->
            wordL (tagText "unmanaged") |> constraintPrefix
        | TyparConstraint.SimpleChoice(tys, _) ->
            bracketL (sepListL (sepL  (tagText "|")) (List.map (auxTypeL env) tys)) |> constraintPrefix
        | TyparConstraint.RequiresDefaultConstructor _ ->
            bracketL (wordL (tagText  "new : unit -> ") ^^ (auxTypar2L env tp)) |> constraintPrefix

    and auxTyparConstraintsL env x = 
        match x with 
        | []   -> emptyL
        | cxs -> wordL (tagText "when") --- aboveListL (List.map (auxTyparConstraintL env) cxs)    

    and typarL     tp = auxTypar2L     SimplifyTypes.typeSimplificationInfo0 tp 
    and typarAtomL tp = auxTyparAtomL SimplifyTypes.typeSimplificationInfo0 tp

    and typeAtomL tau =
        let tau, cxs = tau, []
        let env = SimplifyTypes.CollectInfo false [tau] cxs
        match env.postfixConstraints with
        | [] -> auxTypeAtomL env tau
        | _ -> bracketL (auxTypeL env tau --- auxTyparConstraintsL env env.postfixConstraints)
          
    and typeL tau =
        let tau, cxs = tau, []
        let env = SimplifyTypes.CollectInfo false [tau] cxs
        match env.postfixConstraints with
        | [] -> auxTypeL env tau 
        | _ -> (auxTypeL env tau --- auxTyparConstraintsL env env.postfixConstraints) 

    and typarDeclL tp =
        let tau, cxs = mkTyparTy tp, (List.map (fun x -> (tp, x)) tp.Constraints)
        let env = SimplifyTypes.CollectInfo false [tau] cxs
        match env.postfixConstraints with
        | [] -> auxTypeL env tau 
        | _ -> (auxTypeL env tau --- auxTyparConstraintsL env env.postfixConstraints) 
    and layoutTyparDecls tps = angleBracketListL (List.map typarDeclL       tps) 

    //--------------------------------------------------------------------------
    // DEBUG layout - types
    //--------------------------------------------------------------------------
      
    let rangeL m = wordL (tagText (stringOfRange m))

    let instL tyL tys =
        match tys with
        | []  -> emptyL
        | tys -> sepL (tagText "@[") ^^ commaListL (List.map tyL tys) ^^ rightL (tagText "]")

    let valRefL  (vr:ValRef)  = 
        wordL (tagText vr.LogicalName) |> stampL vr.Stamp 

    let layoutAttrib (Attrib(_, k, _, _, _, _, _)) = 
        leftL (tagText "[<") ^^ 
        (match k with 
         | ILAttrib (ilmeth) -> wordL (tagText ilmeth.Name)
         | FSAttrib (vref)   -> valRefL vref) ^^
        rightL (tagText ">]")
    
    let layoutAttribs attribs = aboveListL (List.map layoutAttrib attribs)

    let arityInfoL (ValReprInfo (tpNames, _, _) as tvd) = 
        let ns = tvd.AritiesOfArgs in 
        leftL (tagText "arity<") ^^ intL tpNames.Length ^^ sepL (tagText ">[") ^^ commaListL (List.map intL ns) ^^ rightL (tagText "]")


    let valL (vspec:Val) =
        let vsL = wordL (tagText (DecompileOpName vspec.LogicalName)) |> stampL vspec.Stamp
        let vsL = vsL -- layoutAttribs (vspec.Attribs)
        vsL

    let typeOfValL      (v:Val) =
        (valL v
          ^^ (if  v.MustInline then wordL (tagText "inline ") else emptyL) 
          ^^ (if v.IsMutable then wordL(tagText "mutable ") else emptyL)
          ^^ wordL (tagText ":")) -- typeL v.Type


    let tslotparamL(TSlotParam(nmOpt, typ, inFlag, outFlag, _, _)) =
        (optionL (tagText >> wordL) nmOpt) ^^ wordL(tagText ":") ^^ typeL typ ^^ (if inFlag then wordL(tagText "[in]") else emptyL)  ^^ (if outFlag then wordL(tagText "[out]") else emptyL)  ^^ (if inFlag then wordL(tagText "[opt]") else emptyL)
    

    let slotSigL (slotsig:SlotSig) =
#if DEBUG
        let (TSlotSig(nm, typ, tps1, tps2, pms, rty)) = slotsig 
        match !global_g with
        | None -> wordL(tagText "<no global g>")
        | Some g -> 
            let rty = GetFSharpViewOfReturnType g rty
            (wordL(tagText "slot") --- (wordL (tagText nm)) ^^ wordL(tagText "@") ^^ typeL typ) --
              (wordL(tagText "LAM") --- spaceListL (List.map typarL       tps1) ^^ rightL(tagText ".")) ---
              (wordL(tagText "LAM") --- spaceListL (List.map typarL       tps2) ^^ rightL(tagText ".")) ---
              (commaListL (List.map (List.map tslotparamL >> tupleL) pms)) ^^ (wordL(tagText "-> ")) --- (typeL rty) 
#else
        ignore slotsig
        wordL(tagText "slotsig")
#endif

    let rec MemberL (v:Val) (membInfo:ValMemberInfo) = 
        (aboveListL [ wordL(tagText "compiled_name! = ") ^^ wordL (tagText v.CompiledName) ;
                      wordL(tagText "membInfo-slotsig! = ") ^^ listL slotSigL membInfo.ImplementedSlotSigs ]) 
    and vspecAtBindL  v = 
        let vL = valL v  in
        let mutL = (if v.IsMutable then wordL(tagText "mutable") ++ vL else vL)
        mutL  --- (aboveListL (List.concat [[wordL(tagText ":") ^^ typeL v.Type];
                                            (match v.MemberInfo with None -> [] | Some mem_info   -> [wordL(tagText "!") ^^ MemberL v mem_info]);
                                            (match v.ValReprInfo with None -> [] | Some arity_info -> [wordL(tagText "#") ^^ arityInfoL arity_info])]))

    let unionCaseRefL (ucr:UnionCaseRef) = wordL (tagText ucr.CaseName)
    let recdFieldRefL (rfref:RecdFieldRef) = wordL (tagText rfref.FieldName)

    //--------------------------------------------------------------------------
    // DEBUG layout - bind, expr, dtree etc.
    //--------------------------------------------------------------------------

    let identL (id:Ident) = wordL (tagText id.idText)  

    // Note: We need nice printing of constants in order to print literals and attributes 
    let constL c =
        let str = 
            match c with
            | Const.Bool x        -> if x then "true" else "false"
            | Const.SByte x       -> (x |> string)+"y"
            | Const.Byte x        -> (x |> string)+"uy"
            | Const.Int16 x       -> (x |> string)+"s"
            | Const.UInt16 x      -> (x |> string)+"us"
            | Const.Int32 x       -> (x |> string)
            | Const.UInt32 x      -> (x |> string)+"u"
            | Const.Int64 x       -> (x |> string)+"L"
            | Const.UInt64 x      -> (x |> string)+"UL"
            | Const.IntPtr x      -> (x |> string)+"n"
            | Const.UIntPtr x     -> (x |> string)+"un"
            | Const.Single d      -> 
                (let s = d.ToString("g12", System.Globalization.CultureInfo.InvariantCulture)
                 if String.forall (fun c -> System.Char.IsDigit(c) || c = '-')  s 
                 then s + ".0" 
                 else s) + "f"
            | Const.Double d      -> 
                let s = d.ToString("g12", System.Globalization.CultureInfo.InvariantCulture)
                if String.forall (fun c -> System.Char.IsDigit(c) || c = '-')  s 
                then s + ".0" 
                else s
            | Const.Char c        -> "'" + c.ToString() + "'" 
            | Const.String bs     -> "\"" + bs + "\"" 
            | Const.Unit          -> "()" 
            | Const.Decimal bs    -> string bs + "M" 
            | Const.Zero       -> "default"
        wordL (tagText str)

    let rec tyconL (tycon:Tycon) =
        if tycon.IsModuleOrNamespace then entityL tycon else 
        
        let lhsL = wordL (tagText (match tycon.TypeOrMeasureKind with TyparKind.Measure -> "[<Measure>] type" | TyparKind.Type -> "type")) ^^ wordL (tagText tycon.DisplayName) ^^ layoutTyparDecls tycon.TyparsNoRange
        let lhsL = lhsL --- layoutAttribs tycon.Attribs
        let memberLs = 
            let adhoc = 
                tycon.MembersOfFSharpTyconSorted 
                    |> List.filter (fun v -> not v.IsDispatchSlot)
                    |> List.filter (fun v -> not v.Deref.IsClassConstructor) 
                    // Don't print individual methods forming interface implementations - these are currently never exported 
                    |> List.filter (fun v -> isNil (Option.get v.MemberInfo).ImplementedSlotSigs)
            let iimpls = 
                match tycon.TypeReprInfo with 
                | TFSharpObjectRepr r when (match r.fsobjmodel_kind with TTyconInterface -> true | _ -> false) -> []
                | _ -> tycon.ImmediateInterfacesOfFSharpTycon
            let iimpls = iimpls |> List.filter (fun (_, compgen, _) -> not compgen)
            // if TTyconInterface, the iimpls should be printed as inherited interfaces 
            if isNil adhoc && isNil iimpls
            then emptyL 
            else 
                let iimplsLs = iimpls |> List.map (fun (ty, _, _) -> wordL(tagText "interface") --- typeL ty)
                let adhocLs  = adhoc  |> List.map (fun vref -> vspecAtBindL  vref.Deref)
                (wordL(tagText "with") @@-- aboveListL (iimplsLs @ adhocLs)) @@ wordL(tagText "end")

        let layoutUnionCaseArgTypes argtys = sepListL (wordL(tagText "*")) (List.map typeL argtys)

        let ucaseL prefixL (ucase: UnionCase) =
            let nmL = wordL (tagText (DemangleOperatorName ucase.Id.idText))
            match ucase.RecdFields |> List.map (fun rfld -> rfld.FormalType) with
            | []     -> (prefixL ^^ nmL)
            | argtys -> (prefixL ^^ nmL ^^ wordL(tagText "of")) --- layoutUnionCaseArgTypes argtys

        let layoutUnionCases ucases =
            let prefixL = if not (isNilOrSingleton ucases) then wordL(tagText "|") else emptyL
            List.map (ucaseL prefixL) ucases
            
        let layoutRecdField (fld:RecdField) =
            let lhs = wordL (tagText fld.Name)
            let lhs = if fld.IsMutable then wordL(tagText "mutable") --- lhs else lhs
            (lhs ^^ rightL(tagText ":")) --- typeL fld.FormalType

        let tyconReprL (repr, tycon:Tycon) = 
            match repr with 
            | TRecdRepr _ ->
                tycon.TrueFieldsAsList |> List.map (fun fld -> layoutRecdField fld ^^ rightL(tagText ";")) |> aboveListL  
            | TFSharpObjectRepr r -> 
                match r.fsobjmodel_kind with 
                | TTyconDelegate _ ->
                    wordL(tagText "delegate ...")
                | _ ->
                    let start = 
                        match r.fsobjmodel_kind with
                        | TTyconClass -> "class" 
                        | TTyconInterface -> "interface" 
                        | TTyconStruct -> "struct" 
                        | TTyconEnum -> "enum" 
                        | _ -> failwith "???"
                    let inherits = 
                       match r.fsobjmodel_kind, tycon.TypeContents.tcaug_super with
                       | TTyconClass, Some super -> [wordL(tagText "inherit") ^^ (typeL super)] 
                       | TTyconInterface, _ -> 
                         tycon.ImmediateInterfacesOfFSharpTycon
                           |> List.filter (fun (_, compgen, _) -> not compgen)
                           |> List.map (fun (ity, _, _) -> wordL(tagText "inherit") ^^ (typeL ity))
                       | _ -> []
                    let vsprs = 
                        tycon.MembersOfFSharpTyconSorted 
                            |> List.filter (fun v -> v.IsDispatchSlot) 
                            |> List.map (fun vref -> vspecAtBindL vref.Deref)
                    let vals  = tycon.TrueFieldsAsList |> List.map (fun f -> (if f.IsStatic then wordL(tagText "static") else emptyL) ^^ wordL(tagText "val") ^^ layoutRecdField f)
                    let alldecls = inherits @ vsprs @ vals
                    let emptyMeasure = match tycon.TypeOrMeasureKind with TyparKind.Measure -> isNil alldecls | _ -> false
                    if emptyMeasure then emptyL else (wordL (tagText start) @@-- aboveListL alldecls) @@ wordL(tagText "end")
            | TUnionRepr _        -> tycon.UnionCasesAsList |> layoutUnionCases |> aboveListL 
            | TAsmRepr _                      -> wordL(tagText "(# ... #)")
            | TMeasureableRepr ty             -> typeL ty
            | TILObjectRepr (TILObjectReprData(_, _, td)) -> wordL (tagText td.Name)
            | _ -> failwith "unreachable"
        let reprL = 
            match tycon.TypeReprInfo with 
#if !NO_EXTENSIONTYPING
            | TProvidedTypeExtensionPoint _
            | TProvidedNamespaceExtensionPoint _
#endif
            | TNoRepr -> 
                match tycon.TypeAbbrev with
                | None   -> lhsL @@-- memberLs
                | Some a -> (lhsL ^^ wordL(tagText "=")) --- (typeL a @@ memberLs)
            | a -> 
                let rhsL = tyconReprL (a, tycon) @@ memberLs
                (lhsL ^^ wordL(tagText "=")) @@-- rhsL
        reprL

        
    //--------------------------------------------------------------------------
    // layout - bind, expr, dtree etc.
    //--------------------------------------------------------------------------

    and bindingL (TBind(v, repr, _)) =
        vspecAtBindL v --- (wordL(tagText "=") ^^ exprL repr)

    and exprL expr = exprWrapL false expr
    and atomL expr = exprWrapL true  expr // true means bracket if needed to be atomic expr 

    and letRecL binds bodyL = 
        let eqnsL = 
            binds
               |> List.mapHeadTail (fun bind -> wordL(tagText "rec") ^^ bindingL bind ^^ wordL(tagText "in"))
                              (fun bind -> wordL(tagText "and") ^^ bindingL bind ^^ wordL(tagText "in")) 
        (aboveListL eqnsL @@ bodyL) 

    and letL bind bodyL = 
        let eqnL = wordL(tagText "let") ^^ bindingL bind ^^ wordL(tagText "in")
        (eqnL @@ bodyL) 
                                                               
    and exprWrapL isAtomic expr =
        let wrap = bracketIfL isAtomic // wrap iff require atomic expr 
        let lay =
            match expr with
            | Expr.Const (c, _, _)  -> constL c
            | Expr.Val (v, flags, _) -> 
                 let xL = valL v.Deref 
                 let xL =
                     match flags with
                       | PossibleConstrainedCall _    -> xL ^^ rightL(tagText "<constrained>")
                       | CtorValUsedAsSelfInit    -> xL ^^ rightL(tagText "<selfinit>")
                       | CtorValUsedAsSuperInit -> xL ^^ rightL(tagText "<superinit>")
                       | VSlotDirectCall -> xL ^^ rightL(tagText "<vdirect>")
                       | NormalValUse -> xL 
                 xL
            | Expr.Sequential (x0, x1, flag, _, _)                    -> 
                let flag = 
                    match flag with
                    | NormalSeq   -> "; (*Seq*)"
                    | ThenDoSeq   -> "; (*ThenDo*)" 
                ((exprL x0 ^^ rightL (tagText flag)) @@ exprL x1) |> wrap
            | Expr.Lambda(_, _, baseValOpt, argvs, body, _, _)  -> 
                let formalsL = spaceListL (List.map vspecAtBindL argvs) in
                let bindingL = 
                    match baseValOpt with
                    | None       -> wordL(tagText "lam") ^^ formalsL ^^ rightL(tagText ".")
                    | Some basev -> wordL(tagText "lam") ^^ (leftL(tagText "base=") ^^ vspecAtBindL basev) --- formalsL ^^ rightL(tagText ".") in
                (bindingL ++ exprL body) |> wrap
            | Expr.TyLambda(_, argtyvs, body, _, _) -> 
                ((wordL(tagText "LAM")    ^^ spaceListL (List.map typarL       argtyvs) ^^ rightL(tagText ".")) ++ exprL body) |> wrap
            | Expr.TyChoose(argtyvs, body, _) -> 
                ((wordL(tagText "CHOOSE") ^^ spaceListL (List.map typarL       argtyvs) ^^ rightL(tagText ".")) ++ exprL body) |> wrap
            | Expr.App (f, _, tys, argtys, _) -> 
                let flayout = atomL f
                appL flayout tys argtys |> wrap
            | Expr.LetRec (binds, body, _, _) -> 
                letRecL binds (exprL body) |> wrap
            | Expr.Let    (bind, body, _, _) -> 
                letL bind (exprL body) |> wrap
            | Expr.Link rX -> 
                (wordL(tagText "RecLink") --- atomL (!rX)) |> wrap
            | Expr.Match (_, _, dtree, targets, _, _) -> 
                leftL(tagText "[") ^^ (decisionTreeL dtree @@ aboveListL (List.mapi targetL (targets |> Array.toList)) ^^ rightL(tagText "]"))
            | Expr.Op (TOp.UnionCase (c), _, args, _)  -> 
                (unionCaseRefL c ++ spaceListL (List.map atomL args)) |> wrap
            | Expr.Op (TOp.ExnConstr (ecref), _, args, _) -> 
                wordL (tagText ecref.LogicalName) ^^ bracketL (commaListL (List.map atomL args))
            | Expr.Op (TOp.Tuple _, _, xs, _) -> 
                tupleL (List.map exprL xs)
            | Expr.Op (TOp.Recd (ctor, tc), _, xs, _)               -> 
                let fields = tc.TrueInstanceFieldsAsList
                let lay fs x = (wordL (tagText fs.rfield_id.idText) ^^ sepL(tagText "=")) --- (exprL x)
                let ctorL = 
                    match ctor with
                    | RecdExpr             -> emptyL
                    | RecdExprIsObjInit-> wordL(tagText "(new)")
                leftL(tagText "{") ^^ semiListL (List.map2 lay fields xs) ^^ rightL(tagText "}") ^^ ctorL
            | Expr.Op (TOp.ValFieldSet rf, _, [rx;x], _) -> 
                (atomL rx --- wordL(tagText ".")) ^^ (recdFieldRefL rf ^^ wordL(tagText "<-") --- exprL x)
            | Expr.Op (TOp.ValFieldSet rf, _, [x], _) -> 
                (recdFieldRefL rf ^^ wordL(tagText "<-") --- exprL x)
            | Expr.Op (TOp.ValFieldGet rf, _, [rx], _) -> 
                (atomL rx ^^ rightL(tagText ".#") ^^ recdFieldRefL rf)
            | Expr.Op (TOp.ValFieldGet rf, _, [], _) -> 
                recdFieldRefL rf
            | Expr.Op (TOp.ValFieldGetAddr rf, _, [rx], _) -> 
                leftL(tagText "&") ^^ bracketL (atomL rx ^^ rightL(tagText ".!") ^^ recdFieldRefL rf)
            | Expr.Op (TOp.ValFieldGetAddr rf, _, [], _) -> 
                leftL(tagText "&") ^^ (recdFieldRefL rf)
            | Expr.Op (TOp.UnionCaseTagGet tycr, _, [x], _) -> 
                wordL (tagText ("#" ^ tycr.LogicalName ^ ".tag")) ^^ atomL x
            | Expr.Op (TOp.UnionCaseProof c, _, [x], _) -> 
                wordL (tagText ("#" ^ c.CaseName^ ".cast")) ^^ atomL x
            | Expr.Op (TOp.UnionCaseFieldGet (c, i), _, [x], _) -> 
                wordL (tagText ("#" ^ c.CaseName ^ "." ^ string i)) --- atomL x
            | Expr.Op (TOp.UnionCaseFieldSet (c, i), _, [x;y], _) -> 
                ((atomL x --- (rightL (tagText ("#" ^ c.CaseName ^ "." ^ string i)))) ^^ wordL(tagText ":=")) --- exprL y
            | Expr.Op (TOp.TupleFieldGet (_, i), _, [x], _) -> 
                wordL (tagText ("#" ^ string i)) --- atomL x
            | Expr.Op (TOp.Coerce, [typ;_], [x], _) -> 
                atomL x --- (wordL(tagText ":>") ^^ typeL typ) 
            | Expr.Op (TOp.Reraise, [_], [], _) -> 
                wordL(tagText "Rethrow!")
            | Expr.Op (TOp.ILAsm (a, tys), tyargs, args, _)      -> 
                let instrs = a |> List.map (sprintf "%+A" >> tagText >> wordL) |> spaceListL // %+A has + since instrs are from an "internal" type  
                let instrs = leftL(tagText "(#") ^^ instrs ^^ rightL(tagText "#)")
                (appL instrs tyargs args ---
                    wordL(tagText ":") ^^ spaceListL (List.map typeAtomL tys)) |> wrap
            | Expr.Op (TOp.LValueOp (lvop, vr), _, args, _) -> 
                (lvalopL lvop ^^ valRefL vr --- bracketL (commaListL (List.map atomL args))) |> wrap
            | Expr.Op (TOp.ILCall (_isVirtCall, _isProtectedCall, _valu, _isNewObjCall, _valUseFlags, _isProperty, _noTailCall, ilMethRef, tinst, minst, _tys), tyargs, args, _) ->
                let meth = ilMethRef.Name
                wordL(tagText "ILCall") ^^ aboveListL [wordL(tagText "meth  ") --- wordL (tagText ilMethRef.DeclaringTypeRef.FullName) ^^ sepL(tagText ".") ^^ wordL (tagText meth);
                                              wordL(tagText "tinst ") --- listL typeL tinst;
                                              wordL(tagText "minst ") --- listL typeL minst;
                                              wordL(tagText "tyargs") --- listL typeL tyargs;
                                              wordL(tagText "args  ") --- listL exprL args] |> wrap
            | Expr.Op (TOp.Array, [_], xs, _) -> 
                leftL(tagText "[|") ^^ commaListL (List.map exprL xs) ^^ rightL(tagText "|]")
            | Expr.Op (TOp.While _, [], [x1;x2], _) -> 
                wordL(tagText "while") ^^ exprL x1 ^^ wordL(tagText "do") ^^ exprL x2 ^^ rightL(tagText "}")
            | Expr.Op (TOp.For _, [], [x1;x2;x3], _) -> 
                wordL(tagText "for") ^^ aboveListL [(exprL x1 ^^ wordL(tagText "to") ^^ exprL x2 ^^ wordL(tagText "do")); exprL x3 ] ^^ rightL(tagText "done")
            | Expr.Op (TOp.TryCatch _, [_], [x1;x2], _) -> 
                wordL(tagText "try") ^^ exprL x1 ^^ wordL(tagText "with") ^^ exprL x2 ^^ rightL(tagText "}")
            | Expr.Op (TOp.TryFinally _, [_], [x1;x2], _) -> 
                wordL(tagText "try") ^^ exprL x1 ^^ wordL(tagText "finally") ^^ exprL x2 ^^ rightL(tagText "}")
            | Expr.Op (TOp.Bytes _, _ , _ , _) -> 
                wordL(tagText "bytes++")       
            | Expr.Op (TOp.UInt16s _, _ , _ , _)                 -> wordL(tagText "uint16++")       
            | Expr.Op (TOp.RefAddrGet, _tyargs, _args, _)      -> wordL(tagText "GetRefLVal...")
            | Expr.Op (TOp.TraitCall _, _tyargs, _args, _)      -> wordL(tagText "traitcall...")
            | Expr.Op (TOp.ExnFieldGet _, _tyargs, _args, _) -> wordL(tagText "TOp.ExnFieldGet...")
            | Expr.Op (TOp.ExnFieldSet _, _tyargs, _args, _) -> wordL(tagText "TOp.ExnFieldSet...")
            | Expr.Op (TOp.TryFinally _, _tyargs, _args, _) -> wordL(tagText "TOp.TryFinally...")
            | Expr.Op (TOp.TryCatch  _, _tyargs, _args, _) -> wordL(tagText "TOp.TryCatch...")
            | Expr.Op (_, _tys, args, _)                        -> wordL(tagText "Expr.Op ...") ^^ bracketL (commaListL (List.map atomL args)) 
            | Expr.Quote (a, _, _, _, _)                       -> leftL(tagText "<@") ^^ atomL a ^^ rightL(tagText "@>")
            | Expr.Obj (_lambdaId, typ, basev, ccall, overrides, iimpls, _)              -> 
                wordL(tagText "OBJ:") ^^ aboveListL [typeL typ;
                                            exprL ccall;
                                            optionL vspecAtBindL basev;
                                            aboveListL (List.map overrideL overrides);
                                            aboveListL (List.map iimplL iimpls)]

            | Expr.StaticOptimization (_tcs, csx, x, _)       -> 
                (wordL(tagText "opt") @@- (exprL x)) @@--
                   (wordL(tagText "|") ^^ exprL csx --- (wordL(tagText "when...") ))
           
        // For tracking ranges through expr rewrites 
        if !layoutRanges 
        then leftL(tagText "{") ^^ (rangeL expr.Range ^^ rightL(tagText ":")) ++ lay ^^ rightL(tagText "}")
        else lay

    and implFilesL implFiles = 
        aboveListL (List.map implFileL implFiles)

    and appL flayout tys args =
        let z = flayout
        let z = z ^^ instL typeL tys
        let z = z --- sepL(tagText "`") --- (spaceListL (List.map atomL args))
        z
       
    and implFileL (TImplFile(_, _, e, _, _)) =
        aboveListL [(wordL(tagText "top implementation ")) @@-- mexprL e]

    and mexprL x =
        match x with 
        | ModuleOrNamespaceExprWithSig(mtyp, defs, _) -> mdefL defs  @@- (wordL(tagText ":")  @@-  entityTypeL mtyp)
    and mdefsL defs = wordL(tagText "Module Defs") @@-- aboveListL(List.map mdefL defs)
    and mdefL x = 
        match x with 
        | TMDefRec(_, tycons , mbinds, _) ->  aboveListL ((tycons |> List.map tyconL) @ List.map mbindL mbinds)
        | TMDefLet(bind, _) -> letL bind emptyL
        | TMDefDo(e, _) -> exprL e
        | TMDefs defs -> mdefsL defs; 
        | TMAbstract mexpr -> mexprL mexpr
    and mbindL x = 
       match x with 
       | ModuleOrNamespaceBinding.Binding bind -> letL bind emptyL
       | ModuleOrNamespaceBinding.Module(mspec, rhs) ->
        (wordL (tagText (if mspec.IsNamespace then "namespace" else "module")) ^^ (wordL (tagText mspec.DemangledModuleOrNamespaceName) |> stampL mspec.Stamp)) @@-- mdefL rhs 

    and entityTypeL (mtyp:ModuleOrNamespaceType) =
        aboveListL [jlistL typeOfValL mtyp.AllValsAndMembers;
                    jlistL tyconL  mtyp.AllEntities;]    

    and entityL (ms:ModuleOrNamespace) =
        let header = wordL(tagText "module") ^^ (wordL  (tagText ms.DemangledModuleOrNamespaceName) |> stampL ms.Stamp) ^^ wordL(tagText ":")
        let footer = wordL(tagText "end")
        let body = entityTypeL ms.ModuleOrNamespaceType
        (header @@-- body) @@ footer

    and ccuL     (ccu:CcuThunk) = entityL ccu.Contents

    and decisionTreeL x = 
        match x with 
        | TDBind (bind, body) -> 
            let bind = wordL(tagText "let") ^^ bindingL bind ^^ wordL(tagText "in") 
            (bind @@ decisionTreeL body) 
        | TDSuccess (args, n) -> 
            wordL(tagText "Success") ^^ leftL(tagText "T") ^^ intL n ^^ tupleL (args |> List.map exprL)
        | TDSwitch (test, dcases, dflt, _) -> 
            (wordL(tagText "Switch") --- exprL test) @@--
            (aboveListL (List.map dcaseL dcases) @@
             match dflt with
             | None       -> emptyL
             | Some dtree -> wordL(tagText "dflt:") --- decisionTreeL dtree)

    and dcaseL (TCase (test, dtree)) = (dtestL test ^^ wordL(tagText "//")) --- decisionTreeL dtree

    and dtestL x = 
        match x with 
        |  (DecisionTreeTest.UnionCase (c, tinst)) -> wordL(tagText "is") ^^ unionCaseRefL c ^^ instL typeL tinst
        |  (DecisionTreeTest.ArrayLength (n, ty)) -> wordL(tagText "length") ^^ intL n ^^ typeL ty
        |  (DecisionTreeTest.Const       c        ) -> wordL(tagText "is") ^^ constL c
        |  (DecisionTreeTest.IsNull               ) -> wordL(tagText "isnull")
        |  (DecisionTreeTest.IsInst (_, typ)           ) -> wordL(tagText "isinst") ^^ typeL typ
        |  (DecisionTreeTest.ActivePatternCase (exp, _, _, _, _)) -> wordL(tagText "query") ^^ exprL exp
            
    and targetL i (TTarget (argvs, body, _)) = leftL(tagText "T") ^^ intL i ^^ tupleL (flatValsL argvs) ^^ rightL(tagText ":") --- exprL body
    and flatValsL vs = vs |> List.map valL

    and tmethodL (TObjExprMethod(TSlotSig(nm, _, _, _, _, _), _, tps, vs, e, _)) =
        (wordL(tagText "TObjExprMethod") --- (wordL (tagText nm)) ^^ wordL(tagText "=")) --
          (wordL(tagText "METH-LAM") --- angleBracketListL (List.map typarL       tps) ^^ rightL(tagText ".")) ---
          (wordL(tagText "meth-lam") --- tupleL (List.map (List.map vspecAtBindL >> tupleL) vs)  ^^ rightL(tagText ".")) ---
          (atomL e) 
    and overrideL tmeth     = wordL(tagText "with") ^^ tmethodL tmeth 
    and iimplL (typ, tmeths) = wordL(tagText "impl") ^^ aboveListL (typeL typ :: List.map tmethodL tmeths) 

    let showType x = Layout.showL (typeL x)
    let showExpr x = Layout.showL (exprL x)
    let traitL x = auxTraitL SimplifyTypes.typeSimplificationInfo0 x
    let typarsL   x = layoutTyparDecls x

end


//--------------------------------------------------------------------------
// Helpers related to type checking modules & namespaces
//--------------------------------------------------------------------------

let wrapModuleOrNamespaceType id cpath mtyp = 
    NewModuleOrNamespace (Some cpath)  taccessPublic  id  XmlDoc.Empty  [] (MaybeLazy.Strict mtyp)

let wrapModuleOrNamespaceTypeInNamespace id cpath mtyp = 
    let mspec = wrapModuleOrNamespaceType id cpath mtyp
    NewModuleOrNamespaceType Namespace [ mspec ] [], mspec

let wrapModuleOrNamespaceExprInNamespace (id :Ident) cpath mexpr = 
    let mspec = wrapModuleOrNamespaceType id cpath (NewEmptyModuleOrNamespaceType Namespace)
    TMDefRec (false, [], [ModuleOrNamespaceBinding.Module(mspec, mexpr)], id.idRange)

// cleanup: make this a property
let SigTypeOfImplFile (TImplFile(_, _, mexpr, _, _)) = mexpr.Type 

//--------------------------------------------------------------------------
// Data structures representing what gets hidden and what gets remapped (i.e. renamed or alpha-converted)
// when a module signature is applied to a module.
//--------------------------------------------------------------------------

type SignatureRepackageInfo = 
    { mrpiVals  : (ValRef * ValRef) list;
      mrpiEntities: (TyconRef * TyconRef) list  }
    
    member remapInfo.ImplToSigMapping = { TypeEquivEnv.Empty with EquivTycons = TyconRefMap.OfList remapInfo.mrpiEntities }
    static member Empty = { mrpiVals = []; mrpiEntities= [] } 

type SignatureHidingInfo = 
    { mhiTycons     : Zset<Tycon>; 
      mhiTyconReprs : Zset<Tycon>;  
      mhiVals       : Zset<Val>; 
      mhiRecdFields : Zset<RecdFieldRef>; 
      mhiUnionCases : Zset<UnionCaseRef> }

    static member Empty = 
        { mhiTycons      = Zset.empty tyconOrder; 
          mhiTyconReprs  = Zset.empty tyconOrder;  
          mhiVals        = Zset.empty valOrder; 
          mhiRecdFields  = Zset.empty recdFieldRefOrder; 
          mhiUnionCases  = Zset.empty unionCaseRefOrder }

let addValRemap v v' tmenv = 
    { tmenv with valRemap= tmenv.valRemap.Add v (mkLocalValRef v')  }

let mkRepackageRemapping mrpi = 
    { valRemap = ValMap.OfList (mrpi.mrpiVals |> List.map (fun (vref, x) -> vref.Deref, x));
      tpinst = emptyTyparInst; 
      tyconRefRemap = TyconRefMap.OfList mrpi.mrpiEntities
      removeTraitSolutions = false }

//--------------------------------------------------------------------------
// Compute instances of the above for mty -> mty
//--------------------------------------------------------------------------

let accEntityRemap (msigty:ModuleOrNamespaceType) (entity:Entity) (mrpi, mhi) =
    let sigtyconOpt = (NameMap.tryFind entity.LogicalName msigty.AllEntitiesByCompiledAndLogicalMangledNames)
    match sigtyconOpt with 
    | None -> 
        // The type constructor is not present in the signature. Hence it is hidden. 
        let mhi = { mhi with mhiTycons = Zset.add entity mhi.mhiTycons }
        (mrpi, mhi) 
    | Some sigtycon  -> 
        // The type constructor is in the signature. Hence record the repackage entry 
        let sigtcref = mkLocalTyconRef sigtycon
        let tcref = mkLocalTyconRef entity
        let mrpi = { mrpi with mrpiEntities = ((tcref, sigtcref) :: mrpi.mrpiEntities) }
        // OK, now look for hidden things 
        let mhi = 
            if (match entity.TypeReprInfo with TNoRepr -> false | _ -> true) && (match sigtycon.TypeReprInfo with TNoRepr -> true | _ -> false) then 
                // The type representation is absent in the signature, hence it is hidden 
                { mhi with mhiTyconReprs = Zset.add entity mhi.mhiTyconReprs } 
            else 
                // The type representation is present in the signature. 
                // Find the fields that have been hidden or which were non-public anyway. 
                mhi 
                |> Array.foldBack  (fun (rfield:RecdField) mhi ->
                            match sigtycon.GetFieldByName(rfield.Name) with 
                            | Some _  -> 
                                // The field is in the signature. Hence it is not hidden. 
                                mhi
                            | _ -> 
                                // The field is not in the signature. Hence it is regarded as hidden. 
                                let rfref = tcref.MakeNestedRecdFieldRef rfield
                                { mhi with mhiRecdFields =  Zset.add rfref mhi.mhiRecdFields })
                        entity.AllFieldsArray
                |> List.foldBack  (fun (ucase:UnionCase) mhi ->
                            match sigtycon.GetUnionCaseByName ucase.DisplayName with 
                            | Some _  -> 
                                // The constructor is in the signature. Hence it is not hidden. 
                                mhi
                            | _ -> 
                                // The constructor is not in the signature. Hence it is regarded as hidden. 
                                let ucref = tcref.MakeNestedUnionCaseRef ucase
                                { mhi with mhiUnionCases =  Zset.add ucref mhi.mhiUnionCases })
                        (entity.UnionCasesAsList)  
        (mrpi, mhi) 

let accSubEntityRemap (msigty:ModuleOrNamespaceType) (entity:Entity) (mrpi, mhi) =
    let sigtyconOpt = (NameMap.tryFind entity.LogicalName msigty.AllEntitiesByCompiledAndLogicalMangledNames)
    match sigtyconOpt with 
    | None -> 
        // The type constructor is not present in the signature. Hence it is hidden. 
        let mhi = { mhi with mhiTycons = Zset.add entity mhi.mhiTycons }
        (mrpi, mhi) 
    | Some sigtycon  -> 
        // The type constructor is in the signature. Hence record the repackage entry 
        let sigtcref = mkLocalTyconRef sigtycon
        let tcref = mkLocalTyconRef entity
        let mrpi = { mrpi with mrpiEntities = ((tcref, sigtcref) :: mrpi.mrpiEntities) }
        (mrpi, mhi) 

let valLinkageAEquiv g aenv (v1:Val) (v2:Val) = 
    (v1.LinkagePartialKey = v2.LinkagePartialKey) &&
    (if v1.IsMember && v2.IsMember then typeAEquivAux EraseAll g aenv v1.Type v2.Type else true)
    
let accValRemap g aenv (msigty:ModuleOrNamespaceType) (implVal:Val) (mrpi, mhi) =
    let sigValOpt = 
        msigty.AllValsAndMembersByPartialLinkageKey 
          |> MultiMap.find implVal.LinkagePartialKey 
          |> List.tryFind (fun sigVal -> valLinkageAEquiv g aenv implVal sigVal)
          
    let vref = mkLocalValRef implVal
    match sigValOpt with 
    | None -> 
        if verbose then dprintf "accValRemap, hide = %s#%d\n" implVal.LogicalName implVal.Stamp
        let mhi = { mhi with mhiVals = Zset.add implVal mhi.mhiVals }
        (mrpi, mhi) 
    | Some (sigVal:Val)  -> 
        // The value is in the signature. Add the repackage entry. 
        let mrpi = { mrpi with mrpiVals = (vref, mkLocalValRef sigVal) :: mrpi.mrpiVals }
        (mrpi, mhi) 

let getCorrespondingSigTy nm (msigty:ModuleOrNamespaceType) = 
    match NameMap.tryFind nm msigty.AllEntitiesByCompiledAndLogicalMangledNames with 
    | None -> NewEmptyModuleOrNamespaceType ModuleOrType 
    | Some sigsubmodul -> sigsubmodul.ModuleOrNamespaceType

let rec accEntityRemapFromModuleOrNamespaceType (mty:ModuleOrNamespaceType) (msigty:ModuleOrNamespaceType) acc = 
    let acc = (mty.AllEntities, acc) ||> QueueList.foldBack (fun e acc -> accEntityRemapFromModuleOrNamespaceType e.ModuleOrNamespaceType (getCorrespondingSigTy e.LogicalName msigty) acc) 
    let acc = (mty.AllEntities, acc) ||> QueueList.foldBack (accEntityRemap msigty) 
    acc 

let rec accValRemapFromModuleOrNamespaceType g aenv (mty:ModuleOrNamespaceType) msigty acc = 
    let acc = (mty.AllEntities,    acc) ||> QueueList.foldBack (fun e acc -> accValRemapFromModuleOrNamespaceType g aenv e.ModuleOrNamespaceType (getCorrespondingSigTy e.LogicalName msigty) acc) 
    let acc = (mty.AllValsAndMembers, acc) ||> QueueList.foldBack (accValRemap g aenv msigty) 
    acc 

let ComputeRemappingFromInferredSignatureToExplicitSignature g mty msigty = 
    // dprintf "ComputeRemappingFromInferredSignatureToExplicitSignature, \nmty = %s\nmmsigty=%s\n" (showL(entityTypeL mty)) (showL(entityTypeL msigty)); 
    let ((mrpi, _) as entityRemap) = accEntityRemapFromModuleOrNamespaceType mty msigty (SignatureRepackageInfo.Empty, SignatureHidingInfo.Empty)  
    let aenv = mrpi.ImplToSigMapping
    let valAndEntityRemap = accValRemapFromModuleOrNamespaceType g aenv mty msigty entityRemap
    valAndEntityRemap 

//--------------------------------------------------------------------------
// Compute instances of the above for mexpr -> mty
//--------------------------------------------------------------------------

/// At TMDefRec nodes abstract (virtual) vslots are effectively binders, even 
/// though they are tucked away inside the tycon. This helper function extracts the
/// virtual slots to aid with finding this babies.
let abstractSlotValsOfTycons (tycons:Tycon list) =  
    tycons 
    |> List.collect (fun tycon -> if tycon.IsFSharpObjectModelTycon then tycon.FSharpObjectModelTypeInfo.fsobjmodel_vslots else []) 
    |> List.map (fun v -> v.Deref)

let rec accEntityRemapFromModuleOrNamespace msigty x acc = 
    match x with 
    | TMDefRec(_, tycons, mbinds, _) -> 
         let acc = (mbinds, acc) ||> List.foldBack (accEntityRemapFromModuleOrNamespaceBind msigty)
         let acc = (tycons, acc) ||> List.foldBack (accEntityRemap msigty) 
         let acc = (tycons, acc) ||> List.foldBack (fun e acc -> accEntityRemapFromModuleOrNamespaceType e.ModuleOrNamespaceType (getCorrespondingSigTy e.LogicalName msigty) acc) 
         acc
    | TMDefLet _  -> acc
    | TMDefDo _  -> acc
    | TMDefs defs -> accEntityRemapFromModuleOrNamespaceDefs msigty defs acc
    | TMAbstract mexpr -> accEntityRemapFromModuleOrNamespaceType mexpr.Type msigty acc

and accEntityRemapFromModuleOrNamespaceDefs msigty mdefs acc = 
    List.foldBack (accEntityRemapFromModuleOrNamespace msigty) mdefs acc

and accEntityRemapFromModuleOrNamespaceBind msigty x acc = 
    match x with 
    | ModuleOrNamespaceBinding.Binding _ -> acc
    | ModuleOrNamespaceBinding.Module(mspec, def) ->
    accSubEntityRemap msigty mspec (accEntityRemapFromModuleOrNamespace (getCorrespondingSigTy mspec.LogicalName msigty) def acc)


let rec accValRemapFromModuleOrNamespace g aenv msigty x acc = 
    match x with 
    | TMDefRec(_, tycons, mbinds, _) -> 
         let acc = (mbinds, acc) ||> List.foldBack (accValRemapFromModuleOrNamespaceBind g aenv msigty)
         //  Abstract (virtual) vslots in the tycons at TMDefRec nodes are binders. They also need to be added to the remapping. 
         let vslotvs = abstractSlotValsOfTycons tycons
         let acc = (vslotvs, acc) ||> List.foldBack (accValRemap g aenv msigty)  
         acc
    | TMDefLet(bind, _)  -> accValRemap g aenv msigty bind.Var acc
    | TMDefDo _  -> acc
    | TMDefs defs -> accValRemapFromModuleOrNamespaceDefs g aenv msigty defs acc
    | TMAbstract mexpr -> accValRemapFromModuleOrNamespaceType g aenv mexpr.Type msigty acc
and accValRemapFromModuleOrNamespaceBind g aenv msigty x acc = 
    match x with 
    | ModuleOrNamespaceBinding.Binding bind -> accValRemap g aenv msigty bind.Var acc
    | ModuleOrNamespaceBinding.Module(mspec, def) ->
    accSubEntityRemap msigty mspec (accValRemapFromModuleOrNamespace g aenv (getCorrespondingSigTy mspec.LogicalName msigty) def acc)

and accValRemapFromModuleOrNamespaceDefs g aenv msigty mdefs acc = List.foldBack (accValRemapFromModuleOrNamespace g aenv msigty) mdefs acc

let ComputeRemappingFromImplementationToSignature g mdef msigty =  
    //if verbose then dprintf "ComputeRemappingFromImplementationToSignature, \nmdefs = %s\nmsigty=%s\n" (showL(DebugPrint.mdefL mdef)) (showL(DebugPrint.entityTypeL msigty));
    let ((mrpi, _) as entityRemap) = accEntityRemapFromModuleOrNamespace msigty mdef (SignatureRepackageInfo.Empty, SignatureHidingInfo.Empty) 
    let aenv = mrpi.ImplToSigMapping
    
    let valAndEntityRemap = accValRemapFromModuleOrNamespace g aenv msigty mdef entityRemap
    valAndEntityRemap

//--------------------------------------------------------------------------
// Compute instances of the above for the assembly boundary
//--------------------------------------------------------------------------

let accTyconHidingInfoAtAssemblyBoundary (tycon:Tycon) mhi =
    if not (canAccessFromEverywhere tycon.Accessibility) then 
        // The type constructor is not public, hence hidden at the assembly boundary. 
        { mhi with mhiTycons = Zset.add tycon mhi.mhiTycons } 
    elif not (canAccessFromEverywhere tycon.TypeReprAccessibility) then 
        { mhi with mhiTyconReprs = Zset.add tycon mhi.mhiTyconReprs } 
    else 
        mhi 
        |> Array.foldBack  
               (fun (rfield:RecdField) mhi ->
                   if not (canAccessFromEverywhere rfield.Accessibility) then 
                       let tcref = mkLocalTyconRef tycon
                       let rfref = tcref.MakeNestedRecdFieldRef rfield
                       { mhi with mhiRecdFields = Zset.add rfref mhi.mhiRecdFields } 
                   else mhi)
               tycon.AllFieldsArray  
        |> List.foldBack  
               (fun (ucase:UnionCase) mhi ->
                   if not (canAccessFromEverywhere ucase.Accessibility) then 
                       let tcref = mkLocalTyconRef tycon
                       let ucref = tcref.MakeNestedUnionCaseRef ucase
                       { mhi with mhiUnionCases = Zset.add ucref mhi.mhiUnionCases } 
                   else mhi)
               (tycon.UnionCasesAsList)   

// Collect up the values hidden at the assembly boundary. This is used by IsHiddenVal to 
// determine if something is considered hidden. This is used in turn to eliminate optimization
// information at the assembly boundary and to decide to label things as "internal".
let accValHidingInfoAtAssemblyBoundary (vspec:Val) mhi =
    if // anything labelled "internal" or more restrictive is considered to be hidden at the assembly boundary
       not (canAccessFromEverywhere vspec.Accessibility) || 
       // compiler generated members for class function 'let' bindings are considered to be hidden at the assembly boundary
       vspec.IsIncrClassGeneratedMember ||                     
       // anything that's not a module or member binding gets assembly visibility
       not vspec.IsMemberOrModuleBinding then 
        // The value is not public, hence hidden at the assembly boundary. 
        { mhi with mhiVals = Zset.add vspec mhi.mhiVals } 
    else 
        mhi

let rec accModuleOrNamespaceHidingInfoAtAssemblyBoundary mty acc = 
    let acc = QueueList.foldBack (fun (e:Entity) acc -> accModuleOrNamespaceHidingInfoAtAssemblyBoundary e.ModuleOrNamespaceType acc) mty.AllEntities acc
    let acc = QueueList.foldBack accTyconHidingInfoAtAssemblyBoundary mty.AllEntities acc
    let acc = QueueList.foldBack accValHidingInfoAtAssemblyBoundary mty.AllValsAndMembers acc
    acc 

let ComputeHidingInfoAtAssemblyBoundary mty acc = 
//     dprintf "ComputeRemappingFromInferredSignatureToExplicitSignature, \nmty = %s\nmmsigty=%s\n" (showL(entityTypeL mty)) (showL(entityTypeL msigty)); 
    accModuleOrNamespaceHidingInfoAtAssemblyBoundary mty acc

//--------------------------------------------------------------------------
// Compute instances of the above for mexpr -> mty
//--------------------------------------------------------------------------

let IsHidden setF accessF remapF debugF = 
    let rec check mrmi x = 
        if verbose then dprintf "IsHidden %s ??\n" (showL (debugF x));
            // Internal/private? 
        not (canAccessFromEverywhere (accessF x)) || 
        (match mrmi with 
         | [] -> false // Ah! we escaped to freedom! 
         | (rpi, mhi) :: rest -> 
            // Explicitly hidden? 
            Zset.contains x (setF mhi) || 
            // Recurse... 
            check rest (remapF rpi x))
    fun mrmi x -> 
        let res = check mrmi x
        if verbose then dprintf "IsHidden, #mrmi = %d, %s = %b\n" mrmi.Length (showL (debugF x)) res;
        res
        
let IsHiddenTycon     mrmi x = IsHidden (fun mhi -> mhi.mhiTycons)     (fun tc -> tc.Accessibility)        (fun rpi x ->  (remapTyconRef rpi.tyconRefRemap (mkLocalTyconRef x)).Deref) DebugPrint.tyconL mrmi x 
let IsHiddenTyconRepr mrmi x = IsHidden (fun mhi -> mhi.mhiTyconReprs) (fun v -> v.TypeReprAccessibility)  (fun rpi x ->  (remapTyconRef rpi.tyconRefRemap (mkLocalTyconRef x)).Deref) DebugPrint.tyconL mrmi x 
let IsHiddenVal       mrmi x = IsHidden (fun mhi -> mhi.mhiVals)       (fun v -> v.Accessibility)          (fun rpi x ->  (remapValRef rpi (mkLocalValRef x)).Deref) DebugPrint.valL mrmi x 
let IsHiddenRecdField mrmi x = IsHidden (fun mhi -> mhi.mhiRecdFields) (fun rfref -> rfref.RecdField.Accessibility) (fun rpi x ->  remapRecdFieldRef rpi.tyconRefRemap x) DebugPrint.recdFieldRefL mrmi x 


//--------------------------------------------------------------------------
// Generic operations on module types
//--------------------------------------------------------------------------

let foldModuleOrNamespaceTy ft fv mty acc = 
    let rec go mty acc = 
        let acc = QueueList.foldBack (fun (e:Entity) acc -> go e.ModuleOrNamespaceType acc) mty.AllEntities acc
        let acc = QueueList.foldBack ft mty.AllEntities acc
        let acc = QueueList.foldBack fv mty.AllValsAndMembers acc
        acc
    go mty acc

let allValsOfModuleOrNamespaceTy m = foldModuleOrNamespaceTy (fun _ acc -> acc) (fun v acc -> v :: acc) m []
let allEntitiesOfModuleOrNamespaceTy m = foldModuleOrNamespaceTy (fun ft acc -> ft :: acc) (fun _ acc -> acc) m []

//---------------------------------------------------------------------------
// Free variables in terms.  Are all constructs public accessible?
//---------------------------------------------------------------------------
 
let isPublicVal (lv:Val)                 = (lv.Accessibility = taccessPublic)
let isPublicUnionCase (ucr:UnionCaseRef) = (ucr.UnionCase.Accessibility = taccessPublic)
let isPublicRecdField (rfr:RecdFieldRef) = (rfr.RecdField.Accessibility = taccessPublic)
let isPublicTycon (tcr:Tycon)            = (tcr.Accessibility = taccessPublic)

let freeVarsAllPublic fvs = 
    // Are any non-public items used in the expr (which corresponded to the fvs)?
    // Recall, taccess occurs in:
    //      EntityData     has     ReprAccessibility and Accessiblity
    //      UnionCase    has     Accessibility
    //      RecdField      has     Accessibility
    //      ValData       has     Accessibility
    // The freevars and FreeTyvars collect local constructs.
    // Here, we test that all those constructs are public.
    //
    // CODEREVIEW:
    // What about non-local vals. This fix assumes non-local vals must be public. OK?
    Zset.forall isPublicVal fvs.FreeLocals  &&
    Zset.forall isPublicUnionCase fvs.FreeUnionCases &&
    Zset.forall isPublicRecdField fvs.FreeRecdFields  &&
    Zset.forall isPublicTycon fvs.FreeTyvars.FreeTycons

let freeTyvarsAllPublic tyvars = 
    Zset.forall isPublicTycon tyvars.FreeTycons


// Detect the subset of match expressions we treat in a linear way
//   -- if then else
//   -- match e with pat[vs] -> e1[vs] | _ -> e2

let (|LinearMatchExpr|_|) expr = 
    match expr with 
    | Expr.Match (sp, m, dtree, [|tg1;(TTarget([], e2, sp2))|], m2, ty) -> Some(sp, m, dtree, tg1, e2, sp2, m2, ty)
    | _ -> None
    
let rebuildLinearMatchExpr (sp, m, dtree, tg1, e2, sp2, m2, ty) = 
    primMkMatch (sp, m, dtree, [|tg1;(TTarget([], e2, sp2))|], m2, ty)


//---------------------------------------------------------------------------
// Free variables in terms.  All binders are distinct.
//---------------------------------------------------------------------------

let emptyFreeVars =  
  { UsesMethodLocalConstructs=false;
    UsesUnboundRethrow=false;
    FreeLocalTyconReprs=emptyFreeTycons; 
    FreeLocals=emptyFreeLocals; 
    FreeTyvars=emptyFreeTyvars;
    FreeRecdFields = emptyFreeRecdFields;
    FreeUnionCases = emptyFreeUnionCases}

let unionFreeVars fvs1 fvs2 = 
  if fvs1 === emptyFreeVars then fvs2 else 
  if fvs2 === emptyFreeVars then fvs1 else
  { FreeLocals                    = unionFreeLocals fvs1.FreeLocals fvs2.FreeLocals;
    FreeTyvars                    = unionFreeTyvars fvs1.FreeTyvars fvs2.FreeTyvars;    
    UsesMethodLocalConstructs     = fvs1.UsesMethodLocalConstructs || fvs2.UsesMethodLocalConstructs;
    UsesUnboundRethrow            = fvs1.UsesUnboundRethrow || fvs2.UsesUnboundRethrow;
    FreeLocalTyconReprs           = unionFreeTycons fvs1.FreeLocalTyconReprs fvs2.FreeLocalTyconReprs; 
    FreeRecdFields                = unionFreeRecdFields fvs1.FreeRecdFields fvs2.FreeRecdFields; 
    FreeUnionCases                = unionFreeUnionCases fvs1.FreeUnionCases fvs2.FreeUnionCases; }

let inline accFreeTyvars (opts:FreeVarOptions) f v acc =
    if not opts.collectInTypes then acc else
    let ftyvs = acc.FreeTyvars
    let ftyvs' = f opts v ftyvs
    if ftyvs === ftyvs' then acc else 
    { acc with FreeTyvars = ftyvs' }

let accFreeVarsInTy  opts ty    acc = accFreeTyvars opts accFreeInType ty acc
let accFreeVarsInTys opts tys   acc = if isNil tys then acc else accFreeTyvars opts accFreeInTypes tys acc
let accFreevarsInTycon opts tcref acc = accFreeTyvars opts accFreeTycon tcref acc
let accFreevarsInVal   opts v     acc = accFreeTyvars opts accFreeInVal v acc
    
let accFreeVarsInTraitSln opts tys acc = accFreeTyvars opts accFreeInTraitSln tys acc 

let boundLocalVal opts v fvs =
    if not opts.includeLocals then fvs else
    let fvs = accFreevarsInVal opts v fvs
    if not (Zset.contains v fvs.FreeLocals) then fvs
    else {fvs with FreeLocals= Zset.remove v fvs.FreeLocals} 

let boundProtect fvs =
    if fvs.UsesMethodLocalConstructs then {fvs with UsesMethodLocalConstructs = false} else fvs

let accUsesFunctionLocalConstructs flg fvs = 
    if flg && not fvs.UsesMethodLocalConstructs then {fvs with UsesMethodLocalConstructs = true} 
    else fvs 

let bound_rethrow fvs =
    if fvs.UsesUnboundRethrow then {fvs with UsesUnboundRethrow = false} else fvs  

let accUsesRethrow flg fvs = 
    if flg && not fvs.UsesUnboundRethrow then {fvs with UsesUnboundRethrow = true} 
    else fvs 

let boundLocalVals opts vs fvs = List.foldBack (boundLocalVal opts) vs fvs

let bindLhs opts (bind:Binding) fvs = boundLocalVal opts bind.Var fvs

let freeVarsCacheCompute opts cache f = if opts.canCache then cached cache f else f()

let rec accBindRhs opts (TBind(_, repr, _)) acc = accFreeInExpr opts repr acc
          
and accFreeInSwitchCases opts csl dflt (acc:FreeVars) =
    Option.foldBack (accFreeInDecisionTree opts) dflt (List.foldBack (accFreeInSwitchCase opts) csl acc)
 
and accFreeInSwitchCase opts (TCase(discrim, dtree)) acc = 
    accFreeInDecisionTree opts dtree (accFreeInTest opts discrim acc)

and accFreeInTest (opts:FreeVarOptions) discrim acc = 
    match discrim with 
    | DecisionTreeTest.UnionCase(ucref, tinst) -> accFreeUnionCaseRef opts ucref (accFreeVarsInTys opts tinst acc)
    | DecisionTreeTest.ArrayLength(_, ty) -> accFreeVarsInTy opts ty acc
    | DecisionTreeTest.Const _
    | DecisionTreeTest.IsNull -> acc
    | DecisionTreeTest.IsInst (srcty, tgty) -> accFreeVarsInTy opts srcty (accFreeVarsInTy opts tgty acc)
    | DecisionTreeTest.ActivePatternCase (exp, tys, activePatIdentity, _, _) -> 
        accFreeInExpr opts exp 
            (accFreeVarsInTys opts tys 
                (Option.foldBack (fun (vref, tinst) acc -> accFreeValRef opts vref (accFreeVarsInTys opts tinst acc)) activePatIdentity acc))

and accFreeInDecisionTree opts x (acc : FreeVars) =
    match x with 
    | TDSwitch(e1, csl, dflt, _) -> accFreeInExpr opts e1 (accFreeInSwitchCases opts csl dflt acc)
    | TDSuccess (es, _) -> accFreeInFlatExprs opts es acc
    | TDBind (bind, body) -> unionFreeVars (bindLhs opts bind (accBindRhs opts bind (freeInDecisionTree opts body))) acc
  
and accFreeInValFlags opts flag acc =
    let isMethLocal = 
        match flag with 
        | VSlotDirectCall 
        | CtorValUsedAsSelfInit 
        | CtorValUsedAsSuperInit -> true 
        | PossibleConstrainedCall  _
        | NormalValUse -> false
    let acc = accUsesFunctionLocalConstructs isMethLocal acc
    match flag with 
    | PossibleConstrainedCall ty -> accFreeTyvars opts accFreeInType ty acc
    | _ -> acc

and accFreeLocalVal opts v fvs =
    if not opts.includeLocals then fvs else
    if Zset.contains v fvs.FreeLocals then fvs 
    else 
        let fvs = accFreevarsInVal opts v fvs
        {fvs with FreeLocals=Zset.add v fvs.FreeLocals}
  
and accLocalTyconRepr opts b fvs = 
    if not opts.includeLocalTyconReprs then fvs else
    if Zset.contains b fvs.FreeLocalTyconReprs  then fvs
    else { fvs with FreeLocalTyconReprs = Zset.add b fvs.FreeLocalTyconReprs } 

and accUsedRecdOrUnionTyconRepr opts (tc:Tycon) fvs = 
    if match tc.TypeReprInfo with  TFSharpObjectRepr _ | TRecdRepr _ | TUnionRepr _ -> true | _ -> false
    then accLocalTyconRepr opts tc fvs
    else fvs

and accFreeUnionCaseRef opts cr fvs =   
    if not opts.includeUnionCases then fvs else
    if Zset.contains cr fvs.FreeUnionCases then fvs 
    else
        let fvs = fvs |> accUsedRecdOrUnionTyconRepr opts cr.Tycon
        let fvs = fvs |> accFreevarsInTycon opts cr.TyconRef
        { fvs with FreeUnionCases = Zset.add cr fvs.FreeUnionCases } 

and accFreeRecdFieldRef opts rfref fvs = 
    if not opts.includeRecdFields then fvs else
    if Zset.contains rfref fvs.FreeRecdFields then fvs 
    else 
        let fvs = fvs |> accUsedRecdOrUnionTyconRepr opts rfref.Tycon
        let fvs = fvs |> accFreevarsInTycon opts rfref.TyconRef 
        { fvs with FreeRecdFields = Zset.add rfref fvs.FreeRecdFields } 
  
and accFreeExnRef _exnc fvs = fvs // Note: this exnc (TyconRef) should be collected the surround types, e.g. tinst of Expr.Op 
and accFreeValRef opts (vref:ValRef) fvs = 
    match vref.IsLocalRef with 
    | true -> accFreeLocalVal opts vref.PrivateTarget fvs
    // non-local values do not contain free variables 
    | _ -> fvs

and accFreeInMethod opts (TObjExprMethod(slotsig, _attribs, tps, tmvs, e, _)) acc =
    accFreeInSlotSig opts slotsig
     (unionFreeVars (accFreeTyvars opts boundTypars tps (List.foldBack (boundLocalVals opts) tmvs (freeInExpr opts  e))) acc)

and accFreeInMethods opts methods acc = 
    List.foldBack (accFreeInMethod opts) methods acc

and accFreeInInterfaceImpl opts (ty, overrides) acc = 
    accFreeVarsInTy opts ty (accFreeInMethods opts overrides acc)

and accFreeInExpr (opts:FreeVarOptions) x acc = 
    match x with
    | Expr.Let _ -> accFreeInExprLinear opts x acc (fun e -> e)
    | _ -> accFreeInExprNonLinear opts x acc
      
and accFreeInExprLinear (opts:FreeVarOptions) x acc contf =   
    // for nested let-bindings, we need to continue after the whole let-binding is processed 
    match x with
    | Expr.Let (bind, e, _, cache) -> 
        let contf = contf << (fun free ->
          unionFreeVars (freeVarsCacheCompute opts cache (fun () -> bindLhs opts bind (accBindRhs opts bind free))) acc )
        accFreeInExprLinear opts e emptyFreeVars contf
    | _ -> 
      // No longer linear expr
      accFreeInExpr opts x acc |> contf
    
and accFreeInExprNonLinear opts x acc =
    match x with
    // BINDING CONSTRUCTS
    | Expr.Lambda (_, ctorThisValOpt, baseValOpt, vs, b, _, rty)  -> 
        unionFreeVars 
                (Option.foldBack (boundLocalVal opts) ctorThisValOpt 
                   (Option.foldBack (boundLocalVal opts) baseValOpt 
                     (boundLocalVals opts vs 
                         (accFreeVarsInTy opts rty 
                             (freeInExpr opts b)))))
            acc
    | Expr.TyLambda (_, vs, b, _, rty) ->
        unionFreeVars (accFreeTyvars opts boundTypars vs (accFreeVarsInTy opts rty (freeInExpr opts b))) acc
    | Expr.TyChoose (vs, b, _) ->
        unionFreeVars (accFreeTyvars opts boundTypars vs (freeInExpr opts b)) acc
    | Expr.LetRec (binds, e, _, cache) ->
        unionFreeVars (freeVarsCacheCompute opts cache (fun () -> List.foldBack (bindLhs opts) binds (List.foldBack (accBindRhs opts) binds (freeInExpr opts e)))) acc
    | Expr.Let _ -> 
        failwith "unreachable - linear expr"
    | Expr.Obj (_, typ, basev, basecall, overrides, iimpls, _)   ->  
        unionFreeVars 
           (boundProtect
              (Option.foldBack (boundLocalVal opts) basev
                (accFreeVarsInTy opts typ
                   (accFreeInExpr opts basecall
                      (accFreeInMethods opts overrides 
                         (List.foldBack (accFreeInInterfaceImpl opts) iimpls emptyFreeVars))))))
           acc  
    // NON-BINDING CONSTRUCTS 
    | Expr.Const _ -> acc
    | Expr.Val (lvr, flags, _) ->  
        accFreeInValFlags opts flags (accFreeValRef opts lvr acc)
    | Expr.Quote (ast, {contents=Some(_, argTypes, argExprs, _data)}, _, _, ty) ->  
        accFreeInExpr opts ast 
            (accFreeInExprs opts argExprs
               (accFreeVarsInTys opts argTypes
                  (accFreeVarsInTy opts ty acc))) 
    | Expr.Quote (ast, {contents=None}, _, _, ty) ->  
        accFreeInExpr opts ast (accFreeVarsInTy opts ty acc)
    | Expr.App(f0, f0ty, tyargs, args, _) -> 
        accFreeVarsInTy opts f0ty
          (accFreeInExpr opts f0
             (accFreeVarsInTys opts tyargs
                (accFreeInExprs opts args acc)))
    | Expr.Link(eref) -> accFreeInExpr opts !eref acc
    | Expr.Sequential (e1, e2, _, _, _) -> 
        let acc = accFreeInExpr opts e1 acc
        // tail-call - this is required because we should be able to handle (((e1; e2); e3); e4; .... ))
        accFreeInExpr opts e2 acc 

    | Expr.StaticOptimization (_, e2, e3, _) -> accFreeInExpr opts e2 (accFreeInExpr opts e3 acc)
    | Expr.Match (_, _, dtree, targets, _, _) -> 
        match x with 
        // Handle if-then-else
        | LinearMatchExpr(_, _, dtree, tg1, e2, _, _, _) ->
            let acc = accFreeInDecisionTree opts dtree acc
            let acc = accFreeInTarget opts tg1 acc
            accFreeInExpr opts e2 acc  // tailcall

        | _ -> 
            let acc = accFreeInDecisionTree opts dtree acc
            accFreeInTargets opts targets acc
            
    //| Expr.Op (TOp.TryCatch, tinst, [Expr.Lambda(_, _, [_], e1, _, _, _); Expr.Lambda(_, _, [_], e2, _, _, _); Expr.Lambda(_, _, [_], e3, _, _, _)], _) ->
    | Expr.Op (TOp.TryCatch _, tinst, [e1;e2;e3], _) ->
        unionFreeVars 
          (accFreeVarsInTys opts tinst
            (accFreeInExprs opts [e1;e2] acc))
          (bound_rethrow (accFreeInExpr opts e3 emptyFreeVars))

    | Expr.Op (op, tinst, args, _) -> 
         let acc = accFreeInOp opts op acc
         let acc = accFreeVarsInTys opts tinst acc
         accFreeInExprs opts args acc

and accFreeInOp opts op acc =
    match op with

    // Things containing no references
    | TOp.Bytes _ 
    | TOp.UInt16s _ 
    | TOp.TryCatch _ 
    | TOp.TryFinally _ 
    | TOp.For _ 
    | TOp.Coerce 
    | TOp.RefAddrGet 
    | TOp.Array 
    | TOp.While _
    | TOp.Goto _ | TOp.Label _ | TOp.Return 
    | TOp.TupleFieldGet _ -> acc

    | TOp.Tuple tupInfo -> accFreeTyvars opts accFreeInTupInfo tupInfo acc
    
    | TOp.UnionCaseTagGet tr -> accUsedRecdOrUnionTyconRepr opts tr.Deref acc
    
    // Things containing just a union case reference
    | TOp.UnionCaseProof cr 
    | TOp.UnionCase cr 
    | TOp.UnionCaseFieldGetAddr (cr, _) 
    | TOp.UnionCaseFieldGet (cr, _) 
    | TOp.UnionCaseFieldSet (cr, _) -> accFreeUnionCaseRef opts cr acc

    // Things containing just an exception reference
    | TOp.ExnConstr ecr 
    | TOp.ExnFieldGet (ecr, _) 
    | TOp.ExnFieldSet (ecr, _)  -> accFreeExnRef ecr acc

    | TOp.ValFieldGet fr 
    | TOp.ValFieldGetAddr fr 
    | TOp.ValFieldSet fr -> accFreeRecdFieldRef opts fr acc

    | TOp.Recd (kind, tcr) -> 
        let acc = accUsesFunctionLocalConstructs (kind = RecdExprIsObjInit) acc
        (accUsedRecdOrUnionTyconRepr opts tcr.Deref (accFreeTyvars opts accFreeTycon tcr acc)) 

    | TOp.ILAsm (_, tys) ->  accFreeVarsInTys opts tys acc
    | TOp.Reraise -> accUsesRethrow true acc

    | TOp.TraitCall(TTrait(tys, _, _, argtys, rty, sln)) -> 
        Option.foldBack (accFreeVarsInTraitSln opts) sln.Value
           (accFreeVarsInTys opts tys 
             (accFreeVarsInTys opts argtys 
               (Option.foldBack (accFreeVarsInTy opts) rty acc)))

    | TOp.LValueOp (_, lvr) -> 
        accFreeValRef opts lvr acc

    | TOp.ILCall (_, isProtectedCall, _, _, valUseFlags, _, _, _, enclTypeArgs, methTypeArgs, tys) ->
       accFreeVarsInTys opts enclTypeArgs 
         (accFreeVarsInTys opts methTypeArgs  
           (accFreeInValFlags opts valUseFlags
             (accFreeVarsInTys opts tys 
               (accUsesFunctionLocalConstructs isProtectedCall acc))))

and accFreeInTargets opts targets acc = 
    Array.foldBack (accFreeInTarget opts) targets acc

and accFreeInTarget opts (TTarget(vs, e, _)) acc = 
    List.foldBack (boundLocalVal opts) vs (accFreeInExpr opts e acc)

and accFreeInFlatExprs opts (es:Exprs) acc = List.foldBack (accFreeInExpr opts) es acc

and accFreeInExprs opts (es: Exprs) acc = 
    match es with 
    | [] -> acc 
    | h::t -> 
        let acc = accFreeInExpr opts h acc
        // tailcall - e.g. Cons(x, Cons(x2, .......Cons(x1000000, Nil))) and [| x1; .... ; x1000000 |]
        accFreeInExprs opts t acc

and accFreeInSlotSig opts (TSlotSig(_, typ, _, _, _, _)) acc = accFreeVarsInTy opts typ acc
 
and freeInDecisionTree opts e = accFreeInDecisionTree opts e emptyFreeVars
and freeInExpr opts e = accFreeInExpr opts e emptyFreeVars

// Note: these are only an approximation - they are currently used only by the optimizer  
let rec accFreeInModuleOrNamespace opts x acc = 
    match x with 
    | TMDefRec(_, _, mbinds, _) -> List.foldBack (accFreeInModuleOrNamespaceBind opts) mbinds acc
    | TMDefLet(bind, _)  -> accBindRhs opts bind  acc
    | TMDefDo(e, _)  -> accFreeInExpr opts e acc
    | TMDefs defs -> accFreeInModuleOrNamespaces opts defs acc
    | TMAbstract(ModuleOrNamespaceExprWithSig(_, mdef, _)) -> accFreeInModuleOrNamespace opts mdef acc // not really right, but sufficient for how this is used in optimization 
and accFreeInModuleOrNamespaceBind opts x acc = 
    match x with 
    | ModuleOrNamespaceBinding.Binding bind ->  accBindRhs opts bind acc
    | ModuleOrNamespaceBinding.Module (_, def) -> accFreeInModuleOrNamespace opts def acc
and accFreeInModuleOrNamespaces opts x acc = 
    List.foldBack (accFreeInModuleOrNamespace opts) x acc

// NOTE: we don't yet need to ask for free variables in module expressions 

let freeInBindingRhs opts bind = accBindRhs opts bind emptyFreeVars
let freeInModuleOrNamespace opts mdef = accFreeInModuleOrNamespace opts mdef emptyFreeVars

//---------------------------------------------------------------------------
// Destruct - rarely needed
//---------------------------------------------------------------------------

let rec stripLambda (e, ty) = 
    match e with 
    | Expr.Lambda (_, ctorThisValOpt, baseValOpt, v, b, _, rty) -> 
        if Option.isSome ctorThisValOpt then errorR(InternalError("skipping ctorThisValOpt", e.Range));
        if Option.isSome baseValOpt then errorR(InternalError("skipping baseValOpt", e.Range));
        let (vs', b', rty') = stripLambda (b, rty)
        (v :: vs', b', rty') 
    | _ -> ([], e, ty)

let rec stripLambdaN n e = 
    assert (n >= 0)
    match e with 
    | Expr.Lambda (_, ctorThisValOpt, baseValOpt, v, body, _, _) when n > 0 -> 
        if Option.isSome ctorThisValOpt then errorR(InternalError("skipping ctorThisValOpt", e.Range));
        if Option.isSome baseValOpt then errorR(InternalError("skipping baseValOpt", e.Range));
        let (vs, body', remaining) = stripLambdaN (n-1) body
        (v :: vs, body', remaining) 
    | _ -> ([], e, n)

let tryStripLambdaN n e = 
    match e with
    | Expr.Lambda(_, None, None, _, _, _, _) -> 
        let argvsl, body, remaining = stripLambdaN n e
        if remaining = 0 then Some (argvsl, body)
        else None
    | _ -> None

let stripTopLambda (e, ty) =
    let tps, taue, tauty = match e with Expr.TyLambda (_, tps, b, _, rty) -> tps, b, rty | _ -> [], e, ty
    let vs, body, rty = stripLambda (taue, tauty)
    tps, vs, body, rty

[<RequireQualifiedAccess>]
type AllowTypeDirectedDetupling = Yes | No

// This is used to infer arities of expressions 
// i.e. base the chosen arity on the syntactic expression shape and type of arguments 
let InferArityOfExpr g allowTypeDirectedDetupling ty partialArgAttribsL retAttribs e = 
    let rec stripLambda_notypes e = 
        match e with 
        | Expr.Lambda (_, _, _, vs, b, _, _) -> 
            let (vs', b') = stripLambda_notypes b
            (vs :: vs', b') 
        | Expr.TyChoose (_, b, _) -> stripLambda_notypes b 
        | _ -> ([], e)

    let stripTopLambdaNoTypes e =
        let tps, taue = match e with Expr.TyLambda (_, tps, b, _, _) -> tps, b | _ -> [], e
        let vs, body = stripLambda_notypes taue
        tps, vs, body

    let tps, vsl, _ = stripTopLambdaNoTypes e
    let fun_arity = vsl.Length
    let dtys, _ =  stripFunTyN g fun_arity (snd (tryDestForallTy g ty))
    let partialArgAttribsL = Array.ofList partialArgAttribsL
    assert (List.length vsl = List.length dtys)
        
    let curriedArgInfos =
        (List.zip vsl dtys) |> List.mapi (fun i (vs, ty) -> 
            let partialAttribs = if i < partialArgAttribsL.Length then partialArgAttribsL.[i] else []
            let tys = 
                match allowTypeDirectedDetupling with
                | AllowTypeDirectedDetupling.No -> [ty] 
                | AllowTypeDirectedDetupling.Yes -> 
                    if (i = 0 && isUnitTy g ty) then [] 
                    else tryDestRefTupleTy g ty
            let ids = 
                if vs.Length = tys.Length then  vs |> List.map (fun v -> Some v.Id)
                else tys |> List.map (fun _ -> None)
            let attribs = 
                if partialAttribs.Length = tys.Length then  partialAttribs 
                else tys |> List.map (fun _ -> [])
            (ids, attribs) ||> List.map2 (fun id attribs -> { Name = id; Attribs = attribs } : ArgReprInfo ))
    let retInfo : ArgReprInfo = { Attribs = retAttribs; Name = None }
    ValReprInfo (ValReprInfo.InferTyparInfo tps, curriedArgInfos, retInfo)

let InferArityOfExprBinding g allowTypeDirectedDetupling (v:Val) e = 
    match v.ValReprInfo with
    | Some info -> info
    | None -> InferArityOfExpr g allowTypeDirectedDetupling v.Type [] [] e

//-------------------------------------------------------------------------
// Check if constraints are satisfied that allow us to use more optimized
// implementations
//------------------------------------------------------------------------- 

let underlyingTypeOfEnumTy (g: TcGlobals) typ = 
    assert(isEnumTy g typ)
    match metadataOfTy g typ with 
#if !NO_EXTENSIONTYPING
    | ProvidedTypeMetadata info -> info.UnderlyingTypeOfEnum()
#endif
    | ILTypeMetadata (TILObjectReprData(_, _, tdef)) -> 

        let info = computeILEnumInfo (tdef.Name, tdef.Fields)
        let ilTy = getTyOfILEnumInfo info
        match ilTy.TypeSpec.Name with 
        | "System.Byte" -> g.byte_ty
        | "System.SByte" -> g.sbyte_ty
        | "System.Int16" -> g.int16_ty
        | "System.Int32" -> g.int32_ty
        | "System.Int64" -> g.int64_ty
        | "System.UInt16" -> g.uint16_ty
        | "System.UInt32" -> g.uint32_ty
        | "System.UInt64" -> g.uint64_ty
        | "System.Single" -> g.float32_ty
        | "System.Double" -> g.float_ty
        | "System.Char" -> g.char_ty
        | "System.Boolean" -> g.bool_ty
        | _ -> g.int32_ty
    | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata ->
        let tycon = (tcrefOfAppTy g typ).Deref
        match tycon.GetFieldByName "value__" with 
        | Some rf -> rf.FormalType
        | None ->  error(InternalError("no 'value__' field found for enumeration type "^tycon.LogicalName, tycon.Range))


// CLEANUP NOTE: Get rid of this mutation. 
let setValHasNoArity (f:Val) = 
    f.SetValReprInfo None; f


//--------------------------------------------------------------------------
// Resolve static optimization constraints
//--------------------------------------------------------------------------

let normalizeEnumTy g ty = (if isEnumTy g ty then underlyingTypeOfEnumTy g ty else ty) 

type StaticOptimizationAnswer = 
    | Yes = 1y
    | No = -1y
    | Unknown = 0y

let decideStaticOptimizationConstraint g c = 
    match c with 
    | TTyconEqualsTycon (a, b) ->
        // Both types must be nominal for a definite result
       let rec checkTypes a b =
           let a = normalizeEnumTy g (stripTyEqnsAndMeasureEqns g a)
           match a with
           | AppTy g (tcref1, _) ->
               let b = normalizeEnumTy g (stripTyEqnsAndMeasureEqns g b)
               match b with 
               | AppTy g (tcref2, _) -> 
                if tyconRefEq g tcref1 tcref2 then StaticOptimizationAnswer.Yes else StaticOptimizationAnswer.No
               | RefTupleTy g _  | FunTy g _  -> StaticOptimizationAnswer.No
               | _ -> StaticOptimizationAnswer.Unknown

           | FunTy g _ ->
               let b = normalizeEnumTy g (stripTyEqnsAndMeasureEqns g b)
               match b with 
               | FunTy g _   -> StaticOptimizationAnswer.Yes
               | AppTy g _ | RefTupleTy g _ -> StaticOptimizationAnswer.No
               | _ -> StaticOptimizationAnswer.Unknown
           | RefTupleTy g ts1 -> 
               let b = normalizeEnumTy g (stripTyEqnsAndMeasureEqns g b)
               match b with 
               | RefTupleTy g ts2 ->
                if ts1.Length = ts2.Length then StaticOptimizationAnswer.Yes
                else StaticOptimizationAnswer.No
               | AppTy g _ | FunTy g _ -> StaticOptimizationAnswer.No
               | _ -> StaticOptimizationAnswer.Unknown
           | _ -> StaticOptimizationAnswer.Unknown
       checkTypes a b
    | TTyconIsStruct a -> 
       let a = normalizeEnumTy g (stripTyEqnsAndMeasureEqns g a)
       match tryDestAppTy g a with 
       | Some tcref1 -> if tcref1.IsStructOrEnumTycon then StaticOptimizationAnswer.Yes else StaticOptimizationAnswer.No
       | None -> StaticOptimizationAnswer.Unknown
            
let rec DecideStaticOptimizations g cs = 
    match cs with 
    | [] -> StaticOptimizationAnswer.Yes
    | h::t -> 
        let d = decideStaticOptimizationConstraint g h 
        if d = StaticOptimizationAnswer.No then StaticOptimizationAnswer.No 
        elif d = StaticOptimizationAnswer.Yes then DecideStaticOptimizations g t 
        else StaticOptimizationAnswer.Unknown

let mkStaticOptimizationExpr g (cs, e1, e2, m) = 
    let d = DecideStaticOptimizations g cs in 
    if d = StaticOptimizationAnswer.No then e2
    elif d = StaticOptimizationAnswer.Yes then e1
    else Expr.StaticOptimization(cs, e1, e2, m)

//--------------------------------------------------------------------------
// Copy expressions, including new names for locally bound values.
// Used to inline expressions.
//--------------------------------------------------------------------------


type ValCopyFlag = 
    | CloneAll
    | CloneAllAndMarkExprValsAsCompilerGenerated
    | OnlyCloneExprVals

// for quotations we do no want to avoid marking values as compiler generated since this may affect the shape of quotation (compiler generated values can be inlined)
let fixValCopyFlagForQuotations = function CloneAllAndMarkExprValsAsCompilerGenerated -> CloneAll | x -> x
    
let markAsCompGen compgen d = 
    let compgen = 
        match compgen with 
        | CloneAllAndMarkExprValsAsCompilerGenerated -> true
        | _ -> false
    { d with val_flags= d.val_flags.SetIsCompilerGenerated(d.val_flags.IsCompilerGenerated || compgen) }

let bindLocalVal (v:Val) (v':Val) tmenv = 
    { tmenv with valRemap=tmenv.valRemap.Add v (mkLocalValRef v') }

let bindLocalVals vs vs' tmenv = 
    { tmenv with valRemap= (vs, vs', tmenv.valRemap) |||> List.foldBack2 (fun v v' acc -> acc.Add v (mkLocalValRef v') ) }

let bindTycon (tc:Tycon) (tc':Tycon) tyenv = 
    { tyenv with tyconRefRemap=tyenv.tyconRefRemap.Add (mkLocalTyconRef tc) (mkLocalTyconRef tc')  }

let bindTycons tcs tcs' tyenv =  
    { tyenv with tyconRefRemap= (tcs, tcs', tyenv.tyconRefRemap) |||> List.foldBack2 (fun tc tc' acc -> acc.Add (mkLocalTyconRef tc) (mkLocalTyconRef tc')) }

let remapAttribKind  tmenv k =  
    match k with 
    | ILAttrib _ as x -> x
    | FSAttrib vref -> FSAttrib(remapValRef tmenv vref)

let tmenvCopyRemapAndBindTypars remapAttrib tmenv tps = 
    let tps', tyenvinner = copyAndRemapAndBindTyparsFull remapAttrib tmenv tps
    let tmenvinner = tyenvinner 
    tps', tmenvinner

let rec remapAttrib g tmenv (Attrib (tcref, kind, args, props, isGetOrSetAttr, targets, m)) = 
    Attrib(remapTyconRef tmenv.tyconRefRemap tcref, 
           remapAttribKind tmenv kind, 
           args |> List.map (remapAttribExpr g tmenv), 
           props |> List.map (fun (AttribNamedArg(nm, ty, flg, expr)) -> AttribNamedArg(nm, remapType tmenv ty, flg, remapAttribExpr g tmenv expr)), 
           isGetOrSetAttr, 
           targets, 
           m)

and remapAttribExpr g tmenv (AttribExpr(e1, e2)) = 
    AttribExpr(remapExpr g CloneAll tmenv e1, remapExpr g CloneAll tmenv e2)
    
and remapAttribs g tmenv xs =  List.map (remapAttrib g tmenv) xs

and remapPossibleForallTy g tmenv ty = remapTypeFull (remapAttribs g tmenv) tmenv ty

and remapArgData g tmenv (argInfo : ArgReprInfo) : ArgReprInfo =
    { Attribs = remapAttribs g tmenv argInfo.Attribs; Name = argInfo.Name }

and remapValReprInfo g tmenv (ValReprInfo(tpNames, arginfosl, retInfo)) =
    ValReprInfo(tpNames, List.mapSquared (remapArgData g tmenv) arginfosl, remapArgData g tmenv retInfo)

and remapValData g tmenv (d: ValData) =
    let ty = d.val_type
    let topValInfo = d.ValReprInfo
    let tyR = ty |> remapPossibleForallTy g tmenv
    let declaringEntityR = d.DeclaringEntity |> remapParentRef tmenv
    let reprInfoR = d.ValReprInfo |> Option.map (remapValReprInfo g tmenv)
    let memberInfoR = d.MemberInfo |> Option.map (remapMemberInfo g d.val_range topValInfo ty tyR tmenv)
    let attribsR = d.Attribs |> remapAttribs g tmenv
    { d with 
        val_type     = tyR
        val_opt_data =
            match d.val_opt_data with
            | Some dd ->
                Some { dd with 
                         val_declaring_entity = declaringEntityR
                         val_repr_info        = reprInfoR
                         val_member_info      = memberInfoR
                         val_attribs          = attribsR }
            | None -> None }

and remapParentRef tyenv p =
    match p with 
    | ParentNone -> ParentNone
    | Parent x -> Parent (x |> remapTyconRef tyenv.tyconRefRemap)

and mapImmediateValsAndTycons ft fv (x:ModuleOrNamespaceType) = 
    let vals = x.AllValsAndMembers |> QueueList.map fv
    let tycons = x.AllEntities |> QueueList.map ft
    new ModuleOrNamespaceType(x.ModuleOrNamespaceKind, vals, tycons)
    
and copyVal compgen (v:Val) = 
    match compgen with 
    | OnlyCloneExprVals when v.IsMemberOrModuleBinding -> v
    | _ ->  v |> NewModifiedVal id

and fixupValData g compgen tmenv (v2:Val) =
    // only fixup if we copy the value
    match compgen with 
    | OnlyCloneExprVals when v2.IsMemberOrModuleBinding -> ()
    | _ ->  
        let newData = remapValData g tmenv v2 |> markAsCompGen compgen
        // uses the same stamp
        v2.SetData newData
    
and copyAndRemapAndBindVals g compgen tmenv vs = 
    let vs2 = vs |> List.map (copyVal compgen)
    let tmenvinner = bindLocalVals vs vs2 tmenv
    vs2 |> List.iter (fixupValData g compgen tmenvinner)
    vs2, tmenvinner

and copyAndRemapAndBindVal g compgen tmenv v = 
    let v2 = v |> copyVal compgen
    let tmenvinner = bindLocalVal v v2 tmenv
    fixupValData g compgen tmenvinner v2
    v2, tmenvinner
    
and remapExpr (g: TcGlobals) (compgen:ValCopyFlag) (tmenv:Remap) x =
    match x with
    // Binding constructs - see also dtrees below 
    | Expr.Lambda (_, ctorThisValOpt, baseValOpt, vs, b, m, rty)  -> 
        let ctorThisValOpt, tmenv =  Option.mapFold (copyAndRemapAndBindVal g compgen) tmenv ctorThisValOpt
        let baseValOpt, tmenv =  Option.mapFold (copyAndRemapAndBindVal g compgen) tmenv baseValOpt
        let vs, tmenv = copyAndRemapAndBindVals g compgen tmenv vs
        let b = remapExpr g compgen tmenv b
        let rty = remapType tmenv rty
        Expr.Lambda (newUnique(), ctorThisValOpt, baseValOpt, vs, b, m, rty)
    | Expr.TyLambda (_, tps, b, m, rty) ->
        let tps', tmenvinner = tmenvCopyRemapAndBindTypars (remapAttribs g tmenv) tmenv tps
        mkTypeLambda m tps' (remapExpr g compgen tmenvinner b, remapType tmenvinner rty)
    | Expr.TyChoose (tps, b, m) ->
        let tps', tmenvinner = tmenvCopyRemapAndBindTypars (remapAttribs g tmenv) tmenv tps
        Expr.TyChoose(tps', remapExpr g compgen tmenvinner b, m)
    | Expr.LetRec (binds, e, m, _) ->  
        let binds', tmenvinner = copyAndRemapAndBindBindings g compgen tmenv binds 
        Expr.LetRec (binds', remapExpr g compgen tmenvinner e, m, NewFreeVarsCache())
    | Expr.Sequential _  
    | Expr.Let _ -> remapLinearExpr g compgen tmenv x (fun x -> x)
    | Expr.Match (spBind, exprm, pt, targets, m, ty) ->
        primMkMatch (spBind, exprm, remapDecisionTree g compgen tmenv pt, 
                     targets |> Array.map (remapTarget g compgen tmenv), 
                     m, remapType tmenv ty)
    // Other constructs
    | Expr.Val (vr, vf, m) -> 
        let vr' = remapValRef tmenv vr 
        let vf' = remapValFlags tmenv vf
        if vr === vr' && vf === vf' then x 
        else Expr.Val (vr', vf', m)
    | Expr.Quote (a, {contents=Some(typeDefs, argTypes, argExprs, data)}, isFromQueryExpression, m, ty) ->  
        // fix value of compgen for both original expression and pickled AST
        let compgen = fixValCopyFlagForQuotations compgen
        Expr.Quote (remapExpr g compgen tmenv a, {contents=Some(typeDefs, remapTypesAux tmenv argTypes, remapExprs g compgen tmenv  argExprs, data)}, isFromQueryExpression, m, remapType tmenv ty)
    | Expr.Quote (a, {contents=None}, isFromQueryExpression, m, ty) ->  
        Expr.Quote (remapExpr g (fixValCopyFlagForQuotations compgen) tmenv a, {contents=None}, isFromQueryExpression, m, remapType tmenv ty)
    | Expr.Obj (_, typ, basev, basecall, overrides, iimpls, m) -> 
        let basev', tmenvinner = Option.mapFold (copyAndRemapAndBindVal g compgen) tmenv basev 
        mkObjExpr(remapType tmenv typ, basev', 
                    remapExpr g compgen tmenv basecall, 
                    List.map (remapMethod g compgen tmenvinner) overrides, 
                    List.map (remapInterfaceImpl g compgen tmenvinner) iimpls, m) 

    // Addresses of immutable field may "leak" across assembly boundaries - see CanTakeAddressOfRecdFieldRef below.
    // This is "ok", in the sense that it is always valid to fix these up to be uses
    // of a temporary local, e.g.
    //       &(E.RF) --> let mutable v = E.RF in &v
    
    | Expr.Op (TOp.ValFieldGetAddr rfref, tinst, [arg], m) when 
          not rfref.RecdField.IsMutable && 
          not (entityRefInThisAssembly g.compilingFslib rfref.TyconRef) -> 

        let tinst = remapTypes tmenv tinst 
        let arg = remapExpr g compgen tmenv arg 
        let tmp, _ = mkMutableCompGenLocal m "copyOfStruct" (actualTyOfRecdFieldRef rfref tinst)
        mkCompGenLet m tmp (mkRecdFieldGetViaExprAddr(arg, rfref, tinst, m)) (mkValAddr m (mkLocalValRef tmp))

    | Expr.Op (TOp.UnionCaseFieldGetAddr (uref, cidx), tinst, [arg], m) when 
          not (uref.FieldByIndex(cidx).IsMutable) && 
          not (entityRefInThisAssembly g.compilingFslib uref.TyconRef) -> 

        let tinst = remapTypes tmenv tinst 
        let arg = remapExpr g compgen tmenv arg 
        let tmp, _ = mkMutableCompGenLocal m "copyOfStruct" (actualTyOfUnionFieldRef uref cidx tinst)
        mkCompGenLet m tmp (mkUnionCaseFieldGetProvenViaExprAddr(arg, uref, tinst, cidx, m)) (mkValAddr m (mkLocalValRef tmp))

    | Expr.Op (op, tinst, args, m) -> 
        let op' = remapOp tmenv op 
        let tinst' = remapTypes tmenv tinst 
        let args' = remapExprs g compgen tmenv args 
        if op === op' && tinst === tinst' && args === args' then x 
        else Expr.Op (op', tinst', args', m)

    | Expr.App(e1, e1ty, tyargs, args, m) -> 
        let e1' = remapExpr g compgen tmenv e1 
        let e1ty' = remapPossibleForallTy g tmenv e1ty 
        let tyargs' = remapTypes tmenv tyargs 
        let args' = remapExprs g compgen tmenv args 
        if e1 === e1' && e1ty === e1ty' && tyargs === tyargs' && args === args' then x 
        else Expr.App(e1', e1ty', tyargs', args', m)
    | Expr.Link(eref) -> 
        remapExpr g compgen tmenv !eref
    | Expr.StaticOptimization (cs, e2, e3, m) -> 
       // note that type instantiation typically resolve the static constraints here 
       mkStaticOptimizationExpr g (List.map (remapConstraint tmenv) cs, remapExpr g compgen tmenv e2, remapExpr g compgen tmenv e3, m)

    | Expr.Const (c, m, ty) -> 
        let ty' = remapType tmenv ty 
        if ty === ty' then x else Expr.Const (c, m, ty')

and remapTarget g compgen tmenv (TTarget(vs, e, spTarget)) = 
    let vs', tmenvinner = copyAndRemapAndBindVals g compgen tmenv vs 
    TTarget(vs', remapExpr g compgen tmenvinner e, spTarget)

and remapLinearExpr g compgen tmenv e contf =
    match e with 
    | Expr.Let (bind, e, m, _) ->  
      let bind', tmenvinner = copyAndRemapAndBindBinding g compgen tmenv bind
      // tailcall
      remapLinearExpr g compgen tmenvinner e (contf << mkLetBind m bind')

    | Expr.Sequential (e1, e2, dir, spSeq, m)  -> 
        let e1' = remapExpr g compgen tmenv e1 
        // tailcall
        remapLinearExpr g compgen tmenv e2 (contf << (fun e2' -> 
        if e1 === e1' && e2 === e2' then e 
        else Expr.Sequential (e1', e2', dir, spSeq, m)))

    | LinearMatchExpr (spBind, exprm, dtree, tg1, e2, sp2, m2, ty) ->
        let dtree = remapDecisionTree g compgen tmenv dtree
        let tg1 = remapTarget g compgen tmenv tg1
        let ty = remapType tmenv ty
        // tailcall
        remapLinearExpr g compgen tmenv e2 (contf << (fun e2 -> 
            rebuildLinearMatchExpr (spBind, exprm, dtree, tg1, e2, sp2, m2, ty)))

    | _ -> contf (remapExpr g compgen tmenv e) 

and remapConstraint tyenv c = 
    match c with 
    | TTyconEqualsTycon(ty1, ty2) -> TTyconEqualsTycon(remapType tyenv ty1, remapType tyenv ty2)
    | TTyconIsStruct(ty1) -> TTyconIsStruct(remapType tyenv ty1)

and remapOp tmenv op = 
    match op with 
    | TOp.Recd (ctor, tcr)             -> TOp.Recd(ctor, remapTyconRef tmenv.tyconRefRemap tcr)
    | TOp.UnionCaseTagGet tcr         -> TOp.UnionCaseTagGet(remapTyconRef tmenv.tyconRefRemap tcr)
    | TOp.UnionCase(ucref)            -> TOp.UnionCase(remapUnionCaseRef tmenv.tyconRefRemap ucref)
    | TOp.UnionCaseProof(ucref)       -> TOp.UnionCaseProof(remapUnionCaseRef tmenv.tyconRefRemap ucref)
    | TOp.ExnConstr ec                -> TOp.ExnConstr(remapTyconRef tmenv.tyconRefRemap ec)
    | TOp.ExnFieldGet(ec, n)           -> TOp.ExnFieldGet(remapTyconRef tmenv.tyconRefRemap ec, n)
    | TOp.ExnFieldSet(ec, n)           -> TOp.ExnFieldSet(remapTyconRef tmenv.tyconRefRemap ec, n)
    | TOp.ValFieldSet rfref           -> TOp.ValFieldSet(remapRecdFieldRef tmenv.tyconRefRemap rfref)
    | TOp.ValFieldGet rfref           -> TOp.ValFieldGet(remapRecdFieldRef tmenv.tyconRefRemap rfref)
    | TOp.ValFieldGetAddr rfref       -> TOp.ValFieldGetAddr(remapRecdFieldRef tmenv.tyconRefRemap rfref)
    | TOp.UnionCaseFieldGet(ucref, n)  -> TOp.UnionCaseFieldGet(remapUnionCaseRef tmenv.tyconRefRemap ucref, n)
    | TOp.UnionCaseFieldSet(ucref, n)  -> TOp.UnionCaseFieldSet(remapUnionCaseRef tmenv.tyconRefRemap ucref, n)
    | TOp.ILAsm (instrs, tys)          -> 
        let tys2 = remapTypes tmenv tys
        if tys === tys2 then op else
        TOp.ILAsm (instrs, tys2)
    | TOp.TraitCall(traitInfo)        -> TOp.TraitCall(remapTraitAux tmenv traitInfo)
    | TOp.LValueOp (kind, lvr)         -> TOp.LValueOp (kind, remapValRef tmenv lvr)
    | TOp.ILCall (isVirtCall, isProtectedCall, valu, isNewObjCall, valUseFlags, isProperty, noTailCall, ilMethRef, enclTypeArgs, methTypeArgs, tys) -> 
       TOp.ILCall (isVirtCall, isProtectedCall, valu, isNewObjCall, remapValFlags tmenv valUseFlags, 
                   isProperty, noTailCall, ilMethRef, remapTypes tmenv enclTypeArgs, 
                   remapTypes tmenv methTypeArgs, remapTypes tmenv tys)
    | _ ->  op
    

and remapValFlags tmenv x =
    match x with 
    | PossibleConstrainedCall ty -> PossibleConstrainedCall (remapType tmenv ty)
    | _ -> x

and remapExprs g compgen tmenv es = List.mapq (remapExpr g compgen tmenv) es
and remapFlatExprs g compgen tmenv es = List.mapq (remapExpr g compgen tmenv) es

and remapDecisionTree g compgen tmenv x =
    match x with 
    | TDSwitch(e1, csl, dflt, m) -> 
        TDSwitch(remapExpr g compgen tmenv e1, 
                List.map (fun (TCase(test, y)) -> 
                  let test' = 
                    match test with 
                    | DecisionTreeTest.UnionCase (uc, tinst)   -> DecisionTreeTest.UnionCase(remapUnionCaseRef tmenv.tyconRefRemap uc, remapTypes tmenv tinst)
                    | DecisionTreeTest.ArrayLength (n, ty)      -> DecisionTreeTest.ArrayLength(n, remapType tmenv ty)
                    | DecisionTreeTest.Const _                  -> test
                    | DecisionTreeTest.IsInst (srcty, tgty)      -> DecisionTreeTest.IsInst (remapType tmenv srcty, remapType tmenv tgty) 
                    | DecisionTreeTest.IsNull                   -> DecisionTreeTest.IsNull 
                    | DecisionTreeTest.ActivePatternCase _ -> failwith "DecisionTreeTest.ActivePatternCase should only be used during pattern match compilation"
                  TCase(test', remapDecisionTree g compgen tmenv y)) csl, 
                Option.map (remapDecisionTree g compgen tmenv) dflt, 
                m)
    | TDSuccess (es, n) -> 
        TDSuccess (remapFlatExprs g compgen tmenv es, n)
    | TDBind (bind, rest) -> 
        let bind', tmenvinner = copyAndRemapAndBindBinding g compgen tmenv bind
        TDBind (bind', remapDecisionTree g compgen tmenvinner rest)
        
and copyAndRemapAndBindBinding g compgen tmenv (bind:Binding) =
    let v = bind.Var
    let v', tmenv = copyAndRemapAndBindVal g compgen tmenv v
    remapAndRenameBind g compgen tmenv bind v' , tmenv

and copyAndRemapAndBindBindings g compgen tmenv binds = 
    let vs', tmenvinner = copyAndRemapAndBindVals g compgen tmenv (valsOfBinds binds)
    remapAndRenameBinds g compgen tmenvinner binds vs', tmenvinner

and remapAndRenameBinds g compgen tmenvinner binds vs' = List.map2 (remapAndRenameBind g compgen tmenvinner) binds vs'
and remapAndRenameBind g compgen tmenvinner (TBind(_, repr, letSeqPtOpt)) v' = TBind(v', remapExpr g compgen tmenvinner repr, letSeqPtOpt)

and remapMethod g compgen tmenv (TObjExprMethod(slotsig, attribs, tps, vs, e, m))  =
    let attribs2 = attribs |> remapAttribs g tmenv
    let slotsig2 = remapSlotSig (remapAttribs g tmenv) tmenv slotsig
    let tps2, tmenvinner = tmenvCopyRemapAndBindTypars (remapAttribs g tmenv) tmenv tps
    let vs2, tmenvinner2 = List.mapFold (copyAndRemapAndBindVals g compgen) tmenvinner vs
    let e2 = remapExpr g compgen tmenvinner2 e
    TObjExprMethod(slotsig2, attribs2, tps2, vs2, e2, m)

and remapInterfaceImpl g compgen tmenv (ty, overrides)  =
    (remapType tmenv ty, List.map (remapMethod g compgen tmenv) overrides)

and remapRecdField g tmenv x = 
    { x with 
          rfield_type     = x.rfield_type     |> remapPossibleForallTy g tmenv;
          rfield_pattribs = x.rfield_pattribs |> remapAttribs g tmenv;
          rfield_fattribs = x.rfield_fattribs |> remapAttribs g tmenv; } 
and remapRecdFields g tmenv (x:TyconRecdFields) = x.AllFieldsAsList |> List.map (remapRecdField g tmenv) |> MakeRecdFieldsTable 

and remapUnionCase g tmenv (x:UnionCase) = 
    { x with 
          FieldTable = x.FieldTable |> remapRecdFields g tmenv;
          ReturnType     = x.ReturnType     |> remapType tmenv;
          Attribs = x.Attribs |> remapAttribs g tmenv; } 
and remapUnionCases g tmenv (x:TyconUnionData) = x.UnionCasesAsList |> List.map (remapUnionCase g tmenv)|> MakeUnionCases 

and remapFsObjData g tmenv x = 
    { x with 
          fsobjmodel_kind = 
             (match x.fsobjmodel_kind with 
              | TTyconDelegate slotsig -> TTyconDelegate (remapSlotSig (remapAttribs g tmenv) tmenv slotsig)
              | TTyconClass | TTyconInterface | TTyconStruct | TTyconEnum -> x.fsobjmodel_kind);
          fsobjmodel_vslots  = x.fsobjmodel_vslots  |> List.map (remapValRef tmenv);
          fsobjmodel_rfields = x.fsobjmodel_rfields |> remapRecdFields g tmenv } 


and remapTyconRepr g tmenv repr = 
    match repr with 
    | TFSharpObjectRepr    x -> TFSharpObjectRepr (remapFsObjData g tmenv x)
    | TRecdRepr          x -> TRecdRepr (remapRecdFields g tmenv x)
    | TUnionRepr   x -> TUnionRepr (remapUnionCases g tmenv x)
    | TILObjectRepr    _ -> failwith "cannot remap IL type definitions"
#if !NO_EXTENSIONTYPING
    | TProvidedNamespaceExtensionPoint _ -> repr
    | TProvidedTypeExtensionPoint info -> 
       TProvidedTypeExtensionPoint 
            { info with 
                 LazyBaseType =  info.LazyBaseType.Force (range0, g.obj_ty) |> remapType tmenv |>  LazyWithContext.NotLazy
                 // The load context for the provided type contains TyconRef objects. We must remap these.
                 // This is actually done on-demand (see the implementation of ProvidedTypeContext)
                 ProvidedType = 
                     info.ProvidedType.PApplyNoFailure (fun st -> 
                         let ctxt = st.Context.RemapTyconRefs(unbox >> remapTyconRef tmenv.tyconRefRemap >> box) 
                         ProvidedType.ApplyContext (st, ctxt)) }
#endif
    | TNoRepr           _ -> repr
    | TAsmRepr           _ -> repr
    | TMeasureableRepr   x -> TMeasureableRepr (remapType tmenv x)

and remapTyconAug tmenv (x:TyconAugmentation) = 
    { x with 
          tcaug_equals                 = x.tcaug_equals                  |> Option.map (mapPair (remapValRef tmenv, remapValRef tmenv));
          tcaug_compare                = x.tcaug_compare                 |> Option.map (mapPair (remapValRef tmenv, remapValRef tmenv));
          tcaug_compare_withc          = x.tcaug_compare_withc           |> Option.map(remapValRef tmenv);
          tcaug_hash_and_equals_withc  = x.tcaug_hash_and_equals_withc   |> Option.map (mapTriple (remapValRef tmenv, remapValRef tmenv, remapValRef tmenv));
          tcaug_adhoc                  = x.tcaug_adhoc                   |> NameMap.map (List.map (remapValRef tmenv));
          tcaug_adhoc_list             = x.tcaug_adhoc_list              |> ResizeArray.map (fun (flag, vref) -> (flag, remapValRef tmenv vref));
          tcaug_super                  = x.tcaug_super                   |> Option.map (remapType tmenv);
          tcaug_interfaces             = x.tcaug_interfaces              |> List.map (map1Of3 (remapType tmenv)) } 

and remapTyconExnInfo g tmenv inp =
    match inp with 
    | TExnAbbrevRepr x -> TExnAbbrevRepr (remapTyconRef tmenv.tyconRefRemap x)
    | TExnFresh      x -> TExnFresh (remapRecdFields g tmenv x)
    | TExnAsmRepr  _ | TExnNone -> inp 

and remapMemberInfo g m topValInfo ty ty' tmenv x = 
    // The slotsig in the ImplementedSlotSigs is w.r.t. the type variables in the value's type. 
    // REVIEW: this is a bit gross. It would be nice if the slotsig was standalone 
    assert (Option.isSome topValInfo)
    let tpsOrig, _, _, _ = GetMemberTypeInFSharpForm g x.MemberFlags (Option.get topValInfo) ty m
    let tps, _, _, _ = GetMemberTypeInFSharpForm g x.MemberFlags (Option.get topValInfo) ty' m
    let renaming, _ = mkTyparToTyparRenaming tpsOrig tps 
    let tmenv = { tmenv with tpinst = tmenv.tpinst @ renaming } 
    { x with 
        ApparentEnclosingEntity    = x.ApparentEnclosingEntity    |>  remapTyconRef tmenv.tyconRefRemap ;
        ImplementedSlotSigs = x.ImplementedSlotSigs |> List.map (remapSlotSig (remapAttribs g tmenv) tmenv); 
    } 

and copyAndRemapAndBindModTy g compgen tmenv mty = 
    let tycons = allEntitiesOfModuleOrNamespaceTy mty
    let vs = allValsOfModuleOrNamespaceTy mty
    let _, _, tmenvinner = copyAndRemapAndBindTyconsAndVals g compgen tmenv tycons vs
    remapModTy g compgen tmenvinner mty, tmenvinner

and remapModTy _g _compgen tmenv mty = 
    mapImmediateValsAndTycons (renameTycon tmenv) (renameVal tmenv) mty 

and renameTycon tyenv x = 
    let tcref = 
        try 
            let res = tyenv.tyconRefRemap.[mkLocalTyconRef x]
            res
        with :? KeyNotFoundException -> 
            errorR(InternalError("couldn't remap internal tycon "^showL(DebugPrint.tyconL x), x.Range)); 
            mkLocalTyconRef x 
    tcref.Deref

and renameVal tmenv x = 
    match tmenv.valRemap.TryFind x with 
    | Some v -> v.Deref
    | None -> x

and copyTycon compgen (tycon:Tycon) = 
    match compgen with 
    | OnlyCloneExprVals -> tycon
    | _ ->  NewClonedTycon tycon

/// This operates over a whole nested collection of tycons and vals simultaneously *)
and copyAndRemapAndBindTyconsAndVals g compgen tmenv tycons vs = 
    let tycons' = tycons |> List.map (copyTycon compgen)

    let tmenvinner = bindTycons tycons tycons' tmenv
    
    // Values need to be copied and renamed. 
    let vs', tmenvinner = copyAndRemapAndBindVals g compgen tmenvinner vs

    // "if a type constructor is hidden then all its inner values and inner type constructors must also be hidden" 
    // Hence we can just lookup the inner tycon/value mappings in the tables. 

    let lookupVal (v:Val) = 
        let vref = 
            try  
               let res = tmenvinner.valRemap.[v]
               res 
            with :? KeyNotFoundException -> 
                errorR(InternalError(sprintf "couldn't remap internal value '%s'" v.LogicalName, v.Range));
                mkLocalValRef v
        vref.Deref
        
    let lookupTycon tycon = 
        let tcref = 
            try 
                let res = tmenvinner.tyconRefRemap.[mkLocalTyconRef tycon]
                res
            with :? KeyNotFoundException -> 
                errorR(InternalError("couldn't remap internal tycon "^showL(DebugPrint.tyconL tycon), tycon.Range));
                mkLocalTyconRef tycon
        tcref.Deref
             
    (tycons, tycons') ||> List.iter2 (fun tcd tcd' -> 
        let tps', tmenvinner2 = tmenvCopyRemapAndBindTypars (remapAttribs g tmenvinner) tmenvinner (tcd.entity_typars.Force(tcd.entity_range))
        tcd'.entity_typars         <- LazyWithContext.NotLazy tps'
        tcd'.entity_attribs        <- tcd.entity_attribs       |> remapAttribs g tmenvinner2
        tcd'.entity_tycon_repr     <- tcd.entity_tycon_repr    |> remapTyconRepr g tmenvinner2
        let typeAbbrevR             = tcd.TypeAbbrev           |> Option.map (remapType tmenvinner2)
        tcd'.entity_tycon_tcaug    <- tcd.entity_tycon_tcaug   |> remapTyconAug tmenvinner2
        tcd'.entity_modul_contents <- MaybeLazy.Strict (tcd.entity_modul_contents.Value 
                                                        |> mapImmediateValsAndTycons lookupTycon lookupVal)
        let exnInfoR                = tcd.ExceptionInfo        |> remapTyconExnInfo g tmenvinner2
        match tcd'.entity_opt_data with
        | Some optData -> tcd'.entity_opt_data <- Some { optData with entity_tycon_abbrev = typeAbbrevR; entity_exn_info = exnInfoR }
        | _ -> 
            tcd'.SetTypeAbbrev typeAbbrevR
            tcd'.SetExceptionInfo exnInfoR)
    tycons', vs', tmenvinner


and allTyconsOfTycon (tycon:Tycon) =
    seq { yield tycon
          for nestedTycon in tycon.ModuleOrNamespaceType.AllEntities do
              yield! allTyconsOfTycon nestedTycon }

and allEntitiesOfModDef mdef =
    seq { match mdef with 
          | TMDefRec(_, tycons, mbinds, _) -> 
              for tycon in tycons do 
                  yield! allTyconsOfTycon tycon
              for mbind in mbinds do 
                match mbind with 
                | ModuleOrNamespaceBinding.Binding _ -> ()
                | ModuleOrNamespaceBinding.Module(mspec, def) -> 
                  yield mspec
                  yield! allEntitiesOfModDef def
          | TMDefLet _           -> ()
          | TMDefDo _            -> ()
          | TMDefs defs      -> 
              for def in defs do 
                  yield! allEntitiesOfModDef def
          | TMAbstract(ModuleOrNamespaceExprWithSig(mty, _, _)) -> 
              yield! allEntitiesOfModuleOrNamespaceTy mty }

and allValsOfModDef mdef = 
    seq { match mdef with 
          | TMDefRec(_, tycons, mbinds, _) -> 
              yield! abstractSlotValsOfTycons tycons 
              for mbind in mbinds do 
                match mbind with 
                | ModuleOrNamespaceBinding.Binding bind -> yield bind.Var
                | ModuleOrNamespaceBinding.Module(_, def) -> yield! allValsOfModDef def
          | TMDefLet(bind, _)            -> 
              yield bind.Var
          | TMDefDo _            -> ()
          | TMDefs defs      -> 
              for def in defs do 
                  yield! allValsOfModDef def
          | TMAbstract(ModuleOrNamespaceExprWithSig(mty, _, _)) -> 
              yield! allValsOfModuleOrNamespaceTy mty }

and remapAndBindModExpr g compgen tmenv (ModuleOrNamespaceExprWithSig(mty, mdef, m)) =
    let mdef = copyAndRemapModDef g compgen tmenv mdef
    let mty, tmenv = copyAndRemapAndBindModTy g compgen tmenv mty
    ModuleOrNamespaceExprWithSig(mty, mdef, m), tmenv

and remapModExpr g compgen tmenv (ModuleOrNamespaceExprWithSig(mty, mdef, m)) =
    let mdef = copyAndRemapModDef g compgen tmenv mdef 
    let mty = remapModTy g compgen tmenv mty 
    ModuleOrNamespaceExprWithSig(mty, mdef, m)

and copyAndRemapModDef g compgen tmenv mdef =
    let tycons = allEntitiesOfModDef mdef |> List.ofSeq
    let vs = allValsOfModDef mdef |> List.ofSeq
    let _, _, tmenvinner = copyAndRemapAndBindTyconsAndVals g compgen tmenv tycons vs
    remapAndRenameModDef g compgen tmenvinner mdef

and remapAndRenameModDefs g compgen tmenv x = 
    List.map (remapAndRenameModDef g compgen tmenv) x 

and remapAndRenameModDef g compgen tmenv mdef =
    match mdef with 
    | TMDefRec(isRec, tycons, mbinds, m) -> 
        // Abstract (virtual) vslots in the tycons at TMDefRec nodes are binders. They also need to be copied and renamed. 
        let tycons = tycons |> List.map (renameTycon tmenv)
        let mbinds = mbinds |> List.map (remapAndRenameModBind g compgen tmenv)
        TMDefRec(isRec, tycons, mbinds, m)
    | TMDefLet(bind, m)            ->
        let v = bind.Var
        let bind = remapAndRenameBind g compgen tmenv bind (renameVal tmenv v)
        TMDefLet(bind, m)
    | TMDefDo(e, m)            ->
        let e = remapExpr g compgen tmenv e
        TMDefDo(e, m)
    | TMDefs defs      -> 
        let defs = remapAndRenameModDefs g compgen tmenv defs
        TMDefs defs
    | TMAbstract mexpr -> 
        let mexpr = remapModExpr g compgen tmenv mexpr
        TMAbstract mexpr

and remapAndRenameModBind g compgen tmenv x = 
    match x with 
    | ModuleOrNamespaceBinding.Binding bind -> 
        let v2 = bind |> valOfBind |> renameVal tmenv
        let bind2 = remapAndRenameBind g compgen tmenv bind v2
        ModuleOrNamespaceBinding.Binding  bind2
    | ModuleOrNamespaceBinding.Module(mspec, def) ->
        let mspec = renameTycon tmenv mspec
        let def = remapAndRenameModDef g compgen tmenv def
        ModuleOrNamespaceBinding.Module(mspec, def)

and remapImplFile g compgen tmenv mv = 
    mapAccImplFile (remapAndBindModExpr g compgen) tmenv mv

let copyModuleOrNamespaceType     g compgen mtyp = copyAndRemapAndBindModTy g compgen Remap.Empty mtyp |> fst
let copyExpr     g compgen e    = remapExpr g compgen Remap.Empty e    
let copyImplFile g compgen e    = remapImplFile g compgen Remap.Empty e |> fst

let instExpr g tpinst e = remapExpr g CloneAll (mkInstRemap tpinst) e

//--------------------------------------------------------------------------
// Replace Marks - adjust debugging marks when a lambda gets
// eliminated (i.e. an expression gets inlined)
//--------------------------------------------------------------------------

let rec remarkExpr m x =
    match x with
    | Expr.Lambda (uniq, ctorThisValOpt, baseValOpt, vs, b, _, rty)  -> Expr.Lambda (uniq, ctorThisValOpt, baseValOpt, vs, remarkExpr m b, m, rty)  
    | Expr.TyLambda (uniq, tps, b, _, rty) -> Expr.TyLambda (uniq, tps, remarkExpr m b, m, rty)
    | Expr.TyChoose (tps, b, _) -> Expr.TyChoose (tps, remarkExpr m b, m)
    | Expr.LetRec (binds, e, _, fvs) ->  Expr.LetRec (remarkBinds m binds, remarkExpr m e, m, fvs)
    | Expr.Let (bind, e, _, fvs) -> Expr.Let (remarkBind m bind, remarkExpr m e, m, fvs)
    | Expr.Match (_, _, pt, targets, _, ty) -> primMkMatch (NoSequencePointAtInvisibleBinding, m, remarkDecisionTree m pt, Array.map (fun (TTarget(vs, e, _)) ->TTarget(vs, remarkExpr m e, SuppressSequencePointAtTarget)) targets, m, ty)
    | Expr.Val (x, valUseFlags, _) -> Expr.Val (x, valUseFlags, m)
    | Expr.Quote (a, conv, isFromQueryExpression, _, ty) ->  Expr.Quote (remarkExpr m a, conv, isFromQueryExpression, m, ty)
    | Expr.Obj (n, typ, basev, basecall, overrides, iimpls, _) -> 
        Expr.Obj (n, typ, basev, remarkExpr m basecall, 
                     List.map (remarkObjExprMethod m) overrides, 
                     List.map (remarkInterfaceImpl m) iimpls, m)
    | Expr.Op (op, tinst, args, _) -> 
        let op = 
            match op with 
            | TOp.TryFinally(_, _) -> TOp.TryFinally(NoSequencePointAtTry, NoSequencePointAtFinally)
            | TOp.TryCatch(_, _) -> TOp.TryCatch(NoSequencePointAtTry, NoSequencePointAtWith)
            | _ -> op
            
        Expr.Op (op, tinst, remarkExprs m args, m)
    | Expr.Link (eref) -> 
        // Preserve identity of fixup nodes during remarkExpr
        eref := remarkExpr m !eref;
        x
    | Expr.App(e1, e1ty, tyargs, args, _) -> Expr.App(remarkExpr m e1, e1ty, tyargs, remarkExprs m args, m)
    | Expr.Sequential (e1, e2, dir, _, _)  -> Expr.Sequential (remarkExpr m e1, remarkExpr m e2, dir, SuppressSequencePointOnExprOfSequential, m)
    | Expr.StaticOptimization (eqns, e2, e3, _) -> Expr.StaticOptimization (eqns, remarkExpr m e2, remarkExpr m e3, m)
    | Expr.Const (c, _, ty) -> Expr.Const (c, m, ty)
  
and remarkObjExprMethod m (TObjExprMethod(slotsig, attribs, tps, vs, e, _)) = 
    TObjExprMethod(slotsig, attribs, tps, vs, remarkExpr m e, m)

and remarkInterfaceImpl m (ty, overrides) = 
    (ty, List.map (remarkObjExprMethod m) overrides)

and remarkExprs m es = es |> List.map (remarkExpr m) 

and remarkFlatExprs m es = es |> List.map (remarkExpr m) 

and remarkDecisionTree m x =
    match x with 
    | TDSwitch(e1, csl, dflt, _) -> TDSwitch(remarkExpr m e1, List.map (fun (TCase(test, y)) -> TCase(test, remarkDecisionTree m y)) csl, Option.map (remarkDecisionTree m) dflt, m)
    | TDSuccess (es, n) -> TDSuccess (remarkFlatExprs m es, n)
    | TDBind (bind, rest) -> TDBind(remarkBind m bind, remarkDecisionTree m rest)

and remarkBinds m binds = List.map (remarkBind m) binds

// This very deliberately drops the sequence points since this is used when adjusting the marks for inlined expressions 
and remarkBind m (TBind(v, repr, _)) = 
    TBind(v, remarkExpr m repr, NoSequencePointAtStickyBinding)


//--------------------------------------------------------------------------
// Reference semantics?
//--------------------------------------------------------------------------

let isRecdOrStructFieldAllocObservable (f:RecdField) = not f.IsStatic && f.IsMutable
let isUnionCaseAllocObservable (uc:UnionCase) = uc.FieldTable.FieldsByIndex |> Array.exists isRecdOrStructFieldAllocObservable
let isUnionCaseRefAllocObservable (uc:UnionCaseRef) = uc.UnionCase |> isUnionCaseAllocObservable
  
let isRecdOrUnionOrStructTyconAllocObservable (_g:TcGlobals) (tycon:Tycon) =
    if tycon.IsUnionTycon then 
        tycon.UnionCasesArray |> Array.exists isUnionCaseAllocObservable
    elif tycon.IsRecordTycon || tycon.IsStructOrEnumTycon then 
        tycon.AllFieldsArray |> Array.exists isRecdOrStructFieldAllocObservable
    else
        false

let isRecdOrUnionOrStructTyconRefAllocObservable g (tcr : TyconRef) = isRecdOrUnionOrStructTyconAllocObservable g tcr.Deref
  
// Although from the pure F# perspective exception values cannot be changed, the .NET 
// implementation of exception objects attaches a whole bunch of stack information to 
// each raised object.  Hence we treat exception objects as if they have identity 
let isExnAllocObservable (_ecref:TyconRef) = true 

// Some of the implementations of library functions on lists use mutation on the tail 
// of the cons cell. These cells are always private, i.e. not accessible by any other 
// code until the construction of the entire return list has been completed. 
// However, within the implementation code reads of the tail cell must in theory be treated 
// with caution.  Hence we are conservative and within fslib we don't treat list 
// reads as if they were pure. 
let isUnionCaseFieldMutable (g: TcGlobals) (ucref:UnionCaseRef) n = 
    (g.compilingFslib && tyconRefEq g ucref.TyconRef g.list_tcr_canon && n = 1) ||
    (ucref.FieldByIndex n).IsMutable
  
let isExnFieldMutable ecref n = 
    if n < 0 || n >= List.length (recdFieldsOfExnDefRef ecref) then errorR(InternalError(sprintf "isExnFieldMutable, exnc = %s, n = %d" ecref.LogicalName n, ecref.Range));
    (recdFieldOfExnDefRefByIdx ecref n).IsMutable

let useGenuineField (tycon:Tycon) (f:RecdField) = 
    Option.isSome f.LiteralValue || tycon.IsEnumTycon || f.rfield_secret || (not f.IsStatic && f.rfield_mutable && not tycon.IsRecordTycon)

let ComputeFieldName tycon f = 
    if useGenuineField tycon f then f.rfield_id.idText
    else CompilerGeneratedName f.rfield_id.idText 

//-------------------------------------------------------------------------
// Helpers for building code contained in the initial environment
//------------------------------------------------------------------------- 

let isQuotedExprTy g ty =  match ty with AppTy g (tcref, _) -> tyconRefEq g tcref g.expr_tcr | _ -> false
let destQuotedExprTy g ty =  match ty with AppTy g (_, [ty]) -> ty | _ -> failwith "destQuotedExprTy"

let mkQuotedExprTy (g:TcGlobals) ty =  TType_app(g.expr_tcr, [ty])
let mkRawQuotedExprTy (g:TcGlobals) =  TType_app(g.raw_expr_tcr, [])

let mkAnyTupledTy (g:TcGlobals) tupInfo tys = 
    match tys with 
    | [] -> g.unit_ty 
    | [h] -> h
    | _ -> TType_tuple(tupInfo, tys)

let mkRefTupledTy g tys = mkAnyTupledTy g tupInfoRef tys
let mkRefTupledVarsTy g vs = mkRefTupledTy g (typesOfVals vs)

let mkMethodTy g argtys rty = mkIteratedFunTy (List.map (mkRefTupledTy g) argtys) rty 
let mkArrayType (g:TcGlobals) ty = TType_app (g.array_tcr_nice, [ty])
let mkByteArrayTy (g:TcGlobals) = mkArrayType g g.byte_ty


//--------------------------------------------------------------------------
// tyOfExpr
//--------------------------------------------------------------------------
 
let rec tyOfExpr g e = 
    match e with 
    | Expr.App(_, fty, tyargs, args, _) -> applyTys g fty (tyargs, args)
    | Expr.Obj (_, ty, _, _, _, _, _)  
    | Expr.Match (_, _, _, _, _, ty) 
    | Expr.Quote(_, _, _, _, ty) 
    | Expr.Const(_, _, ty)              -> (ty)
    | Expr.Val(vref, _, _)  -> vref.Type
    | Expr.Sequential(a, b, k, _, _) -> tyOfExpr g (match k with NormalSeq  -> b | ThenDoSeq -> a)
    | Expr.Lambda(_, _, _, vs, _, _, rty) -> (mkRefTupledVarsTy g vs --> rty)
    | Expr.TyLambda(_, tyvs, _, _, rty) -> (tyvs +-> rty)
    | Expr.Let(_, e, _, _) 
    | Expr.TyChoose(_, e, _)
    | Expr.Link { contents=e}
    | Expr.StaticOptimization (_, _, e, _) 
    | Expr.LetRec(_, e, _, _) -> tyOfExpr g e
    | Expr.Op (op, tinst, _, _) -> 
        match op with 
        | TOp.Coerce -> (match tinst with [to_ty;_fromTy] -> to_ty | _ -> failwith "bad TOp.Coerce node")
        | (TOp.ILCall (_, _, _, _, _, _, _, _, _, _, rtys) | TOp.ILAsm(_, rtys)) -> (match rtys with [h] -> h | _ -> g.unit_ty)
        | TOp.UnionCase uc -> actualResultTyOfUnionCase tinst uc 
        | TOp.UnionCaseProof uc -> mkProvenUnionCaseTy uc tinst  
        | TOp.Recd (_, tcref) -> mkAppTy tcref tinst
        | TOp.ExnConstr _ -> g.exn_ty
        | TOp.Bytes _ -> mkByteArrayTy g
        | TOp.UInt16s _ -> mkArrayType g g.uint16_ty
        | TOp.TupleFieldGet(_, i) -> List.item i tinst
        | TOp.Tuple tupInfo -> mkAnyTupledTy g tupInfo tinst
        | (TOp.For _ | TOp.While _) -> g.unit_ty
        | TOp.Array -> (match tinst with [ty] -> mkArrayType g ty | _ -> failwith "bad TOp.Array node")
        | (TOp.TryCatch _ | TOp.TryFinally _) -> (match tinst with [ty] ->  ty | _ -> failwith "bad TOp_try node")
        | TOp.ValFieldGetAddr(fref) -> mkByrefTy g (actualTyOfRecdFieldRef fref tinst)
        | TOp.ValFieldGet(fref) -> actualTyOfRecdFieldRef fref tinst
        | (TOp.ValFieldSet _ | TOp.UnionCaseFieldSet _ | TOp.ExnFieldSet _ | TOp.LValueOp ((LSet | LByrefSet), _)) ->g.unit_ty
        | TOp.UnionCaseTagGet _ -> g.int_ty
        | TOp.UnionCaseFieldGetAddr(cref, j) -> mkByrefTy g (actualTyOfRecdField (mkTyconRefInst cref.TyconRef tinst) (cref.FieldByIndex j))
        | TOp.UnionCaseFieldGet(cref, j) -> actualTyOfRecdField (mkTyconRefInst cref.TyconRef tinst) (cref.FieldByIndex j)
        | TOp.ExnFieldGet(ecref, j) -> recdFieldTyOfExnDefRefByIdx ecref j
        | TOp.LValueOp (LByrefGet, v) -> destByrefTy g v.Type
        | TOp.LValueOp (LGetAddr, v) -> mkByrefTy g v.Type
        | TOp.RefAddrGet -> (match tinst with [ty] -> mkByrefTy g ty | _ -> failwith "bad TOp.RefAddrGet node")      
        | TOp.TraitCall (TTrait(_, _, _, _, ty, _)) -> GetFSharpViewOfReturnType g ty
        | TOp.Reraise -> (match tinst with [rtn_ty] -> rtn_ty | _ -> failwith "bad TOp.Reraise node")
        | TOp.Goto _ | TOp.Label _ | TOp.Return -> 
            //assert false; 
            //errorR(InternalError("unexpected goto/label/return in tyOfExpr", m)); 
            // It doesn't matter what type we return here. This is only used in free variable analysis in the code generator
            g.unit_ty

//--------------------------------------------------------------------------
// Make applications
//---------------------------------------------------------------------------

let primMkApp (f, fty) tyargs argsl m = 
  Expr.App(f, fty, tyargs, argsl, m)

// Check for the funky where a generic type instantiation at function type causes a generic function
// to appear to accept more arguments than it really does, e.g. "id id 1", where the first "id" is 
// instantiated with "int -> int".
//
// In this case, apply the arguments one at a time.
let isExpansiveUnderInstantiation g fty0 tyargs pargs argsl =
    isForallTy g fty0 && 
    let fty1 = formalApplyTys g fty0 (tyargs, pargs)
    (not (isFunTy g fty1) ||
     let rec loop fty xs = 
         match xs with 
         | [] -> false
         | _ :: t -> not (isFunTy g fty) || loop (rangeOfFunTy g fty) t
     loop fty1 argsl)
    
let rec mkExprApplAux g f fty argsl m =
  match argsl with 
  | [] -> f
  | _ -> 
      // Always combine the term application with a type application
      //
      // Combine the term application with a term application, but only when f' is an under-applied value of known arity
      match f with 
      | Expr.App(f', fty', tyargs, pargs, m2) 
             when
                 (isNil pargs ||
                  (match stripExpr f' with 
                   | Expr.Val(v, _, _) -> 
                       match v.ValReprInfo with 
                       | Some info -> info.NumCurriedArgs > pargs.Length
                       | None -> false
                   | _ -> false)) &&
                 not (isExpansiveUnderInstantiation g fty' tyargs pargs argsl) ->
            primMkApp (f', fty') tyargs (pargs@argsl) (unionRanges m2 m)

      | _ -> 
          // Don't combine. 'f' is not an application
          if not (isFunTy g fty) then error(InternalError("expected a function type", m));
          primMkApp (f, fty) [] argsl m


let rec mkAppsAux g f fty tyargsl argsl m =
  match tyargsl with 
  | tyargs :: rest -> 
      match tyargs with 
      | [] -> mkAppsAux g f fty rest argsl m
      | _ -> 
        let arfty = applyForallTy g fty tyargs
        mkAppsAux g (primMkApp (f, fty) tyargs [] m) arfty rest argsl m
  | [] -> 
      mkExprApplAux g f fty argsl m
      
let mkApps g ((f, fty), tyargsl, argl, m) = mkAppsAux g f fty tyargsl argl m
let mkTyAppExpr m (f, fty) tyargs = match tyargs with [] -> f | _ -> primMkApp (f, fty) tyargs [] m 


//--------------------------------------------------------------------------
// Decision tree reduction
//--------------------------------------------------------------------------

let rec accTargetsOfDecisionTree tree acc =
  match tree with 
  | TDSwitch (_, edges, dflt, _) -> List.foldBack (fun (c:DecisionTreeCase) -> accTargetsOfDecisionTree c.CaseTree) edges (Option.foldBack accTargetsOfDecisionTree dflt acc)
  | TDSuccess (_, i) -> i::acc
  | TDBind (_, rest) -> accTargetsOfDecisionTree rest acc

let rec mapAccTipsOfDecisionTree f tree =
    match tree with 
    | TDSwitch (e, edges, dflt, m) -> TDSwitch (e, List.map (mapAccTipsOfEdge f) edges, Option.map (mapAccTipsOfDecisionTree f) dflt, m)
    | TDSuccess (es, i) -> f es i  
    | TDBind (bind, rest) -> TDBind(bind, mapAccTipsOfDecisionTree f rest)
and mapAccTipsOfEdge f (TCase(x, t)) = 
    TCase(x, mapAccTipsOfDecisionTree f t)

let mapTargetsOfDecisionTree f tree = mapAccTipsOfDecisionTree (fun es i -> TDSuccess(es, f i)) tree

// Dead target elimination 
let eliminateDeadTargetsFromMatch tree (targets:_[]) =
    let used = accTargetsOfDecisionTree tree [] |> ListSet.setify (=) |> Array.ofList
    if used.Length < targets.Length then
        Array.sortInPlace used;
        let ntargets = targets.Length
        let tree' = 
            let remap = Array.create ntargets (-1)
            Array.iteri (fun i tgn -> remap.[tgn] <- i) used;
            tree |> mapTargetsOfDecisionTree (fun tgn -> 
                 if remap.[tgn] = -1 then failwith "eliminateDeadTargetsFromMatch: failure while eliminating unused targets"; 
                 remap.[tgn]) 
        let targets' = Array.map (Array.get targets) used
        tree', targets'
    else 
        tree, targets
    


let rec targetOfSuccessDecisionTree tree =
    match tree with 
    | TDSwitch _ -> None
    | TDSuccess (_, i) -> Some i
    | TDBind(_, t) -> targetOfSuccessDecisionTree t

/// Check a decision tree only has bindings that immediately cover a 'Success'
let rec decisionTreeHasNonTrivialBindings tree =
    match tree with 
    | TDSwitch (_, edges, dflt, _) -> 
        edges |> List.exists (fun c -> decisionTreeHasNonTrivialBindings c.CaseTree) || 
        dflt |> Option.exists decisionTreeHasNonTrivialBindings 
    | TDSuccess _ -> false
    | TDBind (_, t) -> Option.isNone (targetOfSuccessDecisionTree t)

// If a target has assignments and can only be reached through one 
// branch (i.e. is "linear"), then transfer the assignments to the r.h.s. to be a "let". 
let foldLinearBindingTargetsOfMatch tree (targets: _[]) =

    // Don't do this when there are any bindings in the tree except where those bindings immediately cover a success node
    // since the variables would be extruded from their scope. 
    if decisionTreeHasNonTrivialBindings tree then 
        tree, targets 

    else
        let branchesToTargets = Array.create targets.Length []
        // Build a map showing how each target might be reached
        let rec accumulateTipsOfDecisionTree accBinds tree  =
            match tree with 
            | TDSwitch (_, edges, dflt, _) -> 
                assert (isNil accBinds)  // No switches under bindings
                for edge in edges do accumulateTipsOfDecisionTree accBinds edge.CaseTree
                match dflt with 
                | None -> ()
                | Some tree -> accumulateTipsOfDecisionTree accBinds tree
            | TDSuccess (es, i) -> 
                branchesToTargets.[i] <- (List.rev accBinds, es) :: branchesToTargets.[i]
            | TDBind (bind, rest) -> 
                accumulateTipsOfDecisionTree (bind::accBinds) rest 

        // Compute the targets that can only be reached one way
        accumulateTipsOfDecisionTree [] tree 
        let isLinearTarget bs = match bs with [_] -> true | _ -> false
        let isLinearTgtIdx i = isLinearTarget branchesToTargets.[i] 
        let getLinearTgtIdx i = branchesToTargets.[i].Head
        let hasLinearTgtIdx = branchesToTargets |> Array.exists isLinearTarget

        if not hasLinearTgtIdx then 

            tree, targets

        else
            
            /// rebuild the decision tree, replacing 'bind-then-success' decision trees by TDSuccess nodes that just go to the target
            let rec rebuildDecisionTree tree =
                
                // Check if this is a bind-then-success tree
                match targetOfSuccessDecisionTree tree with
                | Some i when isLinearTgtIdx i -> TDSuccess([], i)
                | _ -> 
                    match tree with 
                    | TDSwitch (e, edges, dflt, m) -> TDSwitch (e, List.map rebuildDecisionTreeEdge edges, Option.map rebuildDecisionTree dflt, m)
                    | TDSuccess _ -> tree
                    | TDBind _ -> tree

            and rebuildDecisionTreeEdge (TCase(x, t)) =  
                TCase(x, rebuildDecisionTree t)

            let tree' =  rebuildDecisionTree tree

            /// rebuild the targets , replacing linear targets by ones that include all the 'let' bindings from the source
            let targets' = 
                targets |> Array.mapi (fun i (TTarget(vs, exprTarget, spTarget) as tg) -> 
                    if isLinearTgtIdx i then
                        let (binds, es) = getLinearTgtIdx i
                        // The value bindings are moved to become part of the target.
                        // Hence the expressions in the value bindings can be remarked with the range of the target.
                        let mTarget = exprTarget.Range
                        let es = es |> List.map (remarkExpr mTarget)
                        // These are non-sticky - any sequence point for 'exprTarget' goes on 'exprTarget' _after_ the bindings have been evaluated
                        TTarget(List.empty, mkLetsBind mTarget binds (mkInvisibleLetsFromBindings mTarget vs es exprTarget), spTarget)
                    else tg )
     
            tree', targets'

// Simplify a little as we go, including dead target elimination 
let rec simplifyTrivialMatch spBind exprm matchm ty tree (targets : _[]) = 
    match tree with 
    | TDSuccess(es, n) -> 
        if n >= targets.Length then failwith "simplifyTrivialMatch: target out of range";
        // REVIEW: should we use _spTarget here?
        let (TTarget(vs, rhs, _spTarget)) = targets.[n]
        if vs.Length <> es.Length then failwith ("simplifyTrivialMatch: invalid argument, n = "^string n^", List.length targets = "^string targets.Length);
        // These are non-sticky - any sequence point for 'rhs' goes on 'rhs' _after_ the bindings have been made
        mkInvisibleLetsFromBindings rhs.Range vs es rhs
    | _ -> 
        primMkMatch (spBind, exprm, tree, targets, matchm, ty)
 
// Simplify a little as we go, including dead target elimination 
let mkAndSimplifyMatch spBind exprm matchm ty tree targets  = 
    let targets = Array.ofList targets
    match tree with 
    | TDSuccess _ -> 
        simplifyTrivialMatch spBind exprm matchm ty tree targets
    | _ -> 
        let tree, targets = eliminateDeadTargetsFromMatch tree targets
        let tree, targets = foldLinearBindingTargetsOfMatch tree targets
        simplifyTrivialMatch spBind exprm matchm ty tree targets


//-------------------------------------------------------------------------
// mkExprAddrOfExprAux
//------------------------------------------------------------------------- 

type Mutates = DefinitelyMutates | PossiblyMutates | NeverMutates
exception DefensiveCopyWarning of string * range 

let isRecdOrStructTyImmutable g ty =
    match tryDestAppTy g ty with 
    | None -> false
    | Some tcref -> 
      not (isRecdOrUnionOrStructTyconRefAllocObservable g tcref) ||
      tyconRefEq g tcref g.decimal_tcr ||
      tyconRefEq g tcref g.date_tcr

// We can take the address of values of struct type even if the value is immutable
// under certain conditions
//   - all instances of the type are  known to be immutable; OR
//   - the operation is known not to mutate
//
// Note this may be taking the address of a closure field, i.e. a copy
// of the original struct, e.g. for
//    let f () = 
//        let g1 = A.G(1)
//        (fun () -> g1.x1)
//
// Note: isRecdOrStructTyImmutable implies PossiblyMutates or NeverMutates
//
// We only do this for true local or closure fields because we can't take addresses of immutable static 
// fields across assemblies.
let CanTakeAddressOfImmutableVal g (v:ValRef) mut =
    // We can take the address of values of struct type if the operation doesn't mutate 
    // and the value is a true local or closure field. 
    not v.IsMutable &&
    not v.IsMemberOrModuleBinding &&
    (match mut with 
     | NeverMutates -> true 
     | PossiblyMutates -> isRecdOrStructTyImmutable g v.Type 
     | DefinitelyMutates -> false)

let MustTakeAddressOfVal (g:TcGlobals) (v:ValRef) = 
    v.IsMutable &&
    // We can only take the address of mutable values in the same assembly
    valRefInThisAssembly g.compilingFslib v

let MustTakeAddressOfRecdField (rf: RecdField) = 
    // Static mutable fields must be private, hence we don't have to take their address
    not rf.IsStatic && 
    rf.IsMutable

let MustTakeAddressOfRecdFieldRef (rfref: RecdFieldRef) =  MustTakeAddressOfRecdField rfref.RecdField

let CanTakeAddressOfRecdFieldRef (g:TcGlobals) (rfref: RecdFieldRef) mut tinst =
    mut <> DefinitelyMutates && 
    // We only do this if the field is defined in this assembly because we can't take addresses across assemblies for immutable fields
    entityRefInThisAssembly g.compilingFslib rfref.TyconRef &&
    isRecdOrStructTyImmutable g (actualTyOfRecdFieldRef rfref tinst)

let CanTakeAddressOfUnionFieldRef (g:TcGlobals) (uref: UnionCaseRef) mut tinst cidx =
    mut <> DefinitelyMutates && 
    // We only do this if the field is defined in this assembly because we can't take addresses across assemblies for immutable fields
    entityRefInThisAssembly g.compilingFslib uref.TyconRef &&
    isRecdOrStructTyImmutable g (actualTyOfUnionFieldRef uref cidx tinst)


let rec mkExprAddrOfExprAux g mustTakeAddress useReadonlyForGenericArrayAddress mut e addrExprVal m =
    if not mustTakeAddress then None, e else
    match e with 
    // LVALUE: "x" where "x" is byref 
    | Expr.Op (TOp.LValueOp (LByrefGet, v), _, [], m) -> 
        None, exprForValRef m v
    // LVALUE: "x" where "x" is mutable local, mutable intra-assembly module/static binding, or operation doesn't mutate 
    // Note: we can always take the address of mutable values
    | Expr.Val(v, _, m) when MustTakeAddressOfVal g v || CanTakeAddressOfImmutableVal g v mut ->
        None, mkValAddr m v
    // LVALUE: "x" where "e.x" is record field. 
    | Expr.Op (TOp.ValFieldGet rfref, tinst, [e], m) when MustTakeAddressOfRecdFieldRef rfref || CanTakeAddressOfRecdFieldRef g rfref mut tinst ->
        let exprty = tyOfExpr g e
        let wrap, expra = mkExprAddrOfExprAux g (isStructTy g exprty) false mut e None m
        wrap, mkRecdFieldGetAddrViaExprAddr(expra, rfref, tinst, m)
    // LVALUE: "x" where "e.x" is union field
    | Expr.Op (TOp.UnionCaseFieldGet (uref, cidx), tinst, [e], m) when MustTakeAddressOfRecdField (uref.FieldByIndex(cidx)) || CanTakeAddressOfUnionFieldRef g uref mut tinst cidx ->
        let exprty = tyOfExpr g e
        let wrap, expra = mkExprAddrOfExprAux g (isStructTy g exprty) false mut e None m
        wrap, mkUnionCaseFieldGetAddrProvenViaExprAddr(expra, uref, tinst, cidx, m)

    // LVALUE: "x" where "e.x" is a .NET static field. 
    | Expr.Op (TOp.ILAsm ([IL.I_ldsfld(_vol, fspec)], [ty2]), tinst, [], m) -> 
        None, Expr.Op (TOp.ILAsm ([IL.I_ldsflda(fspec)], [mkByrefTy g ty2]), tinst, [], m)

    // LVALUE: "x" where "e.x" is a .NET instance field. "e" may be an lvalue 
    | Expr.Op (TOp.ILAsm ([IL.I_ldfld(_align, _vol, fspec)], [ty2]), tinst, [e], m) 
       -> 
        let exprty = tyOfExpr g e
        let wrap, expra = mkExprAddrOfExprAux g (isStructTy g exprty) false mut e None m
        wrap, Expr.Op (TOp.ILAsm ([IL.I_ldflda(fspec)], [mkByrefTy g ty2]), tinst, [expra], m)

    // LVALUE: "x" where "x" is mutable static field. 
    | Expr.Op (TOp.ValFieldGet rfref, tinst, [], m) when MustTakeAddressOfRecdFieldRef rfref || CanTakeAddressOfRecdFieldRef g rfref mut tinst ->
        None, mkStaticRecdFieldGetAddr(rfref, tinst, m)

    // LVALUE:  "e.[n]" where e is an array of structs 
    | Expr.App(Expr.Val(vf, _, _), _, [elemTy], [aexpr;nexpr], _) 
         when (valRefEq g vf g.array_get_vref) -> 
        
        let shape = ILArrayShape.SingleDimensional
        let readonly = if isTyparTy g elemTy &&  useReadonlyForGenericArrayAddress then ReadonlyAddress else NormalAddress
        let isNativePtr = 
            match addrExprVal with
            | Some(vf) -> valRefEq g vf g.addrof2_vref
            | _ -> false
        None, mkArrayElemAddress g (readonly, isNativePtr, shape, elemTy, aexpr, nexpr, m)

    // LVALUE:  "e.[n1, n2]", "e.[n1, n2, n3]", "e.[n1, n2, n3, n4]" where e is an array of structs 
    | Expr.App(Expr.Val(vf, _, _), _, [elemTy], (aexpr::args), _) 
         when (valRefEq g vf g.array2D_get_vref || valRefEq g vf g.array3D_get_vref || valRefEq g vf g.array4D_get_vref) -> 
        
        let shape = ILArrayShape.FromRank args.Length
        let readonly = if isTyparTy g elemTy &&  useReadonlyForGenericArrayAddress then ReadonlyAddress else NormalAddress
        let isNativePtr = 
            match addrExprVal with
            | Some(vf) -> valRefEq g vf g.addrof2_vref
            | _ -> false
            
        None, Expr.Op (TOp.ILAsm ([IL.I_ldelema(readonly, isNativePtr, shape, mkILTyvarTy 0us)], [mkByrefTy g elemTy]), [elemTy], (aexpr::args), m)

    // Give a nice error message for DefinitelyMutates on immutable values, or mutable values in other assemblies
    | Expr.Val(v, _, m) when mut = DefinitelyMutates
       -> 
        if isByrefTy g v.Type then error(Error(FSComp.SR.tastUnexpectedByRef(), m));
        if v.IsMutable then 
            error(Error(FSComp.SR.tastInvalidAddressOfMutableAcrossAssemblyBoundary(), m));
        else 
            error(Error(FSComp.SR.tastValueMustBeLocalAndMutable(), m));
         
    | _ -> 
        let ty = tyOfExpr g e
        if isStructTy g ty then 
            match mut with 
            | NeverMutates -> ()
            | DefinitelyMutates -> 
                errorR(Error(FSComp.SR.tastInvalidMutationOfConstant(), m));
            | PossiblyMutates -> 
                warning(DefensiveCopyWarning(FSComp.SR.tastValueHasBeenCopied(), m));
        let tmp, _ = 
            match mut with 
            | NeverMutates -> mkCompGenLocal m "copyOfStruct" ty 
            | _ -> mkMutableCompGenLocal m "copyOfStruct" ty
        Some (tmp, e), (mkValAddr m (mkLocalValRef tmp))        

let mkExprAddrOfExpr g mustTakeAddress useReadonlyForGenericArrayAddress mut e addrExprVal m =
    let optBind, addre = mkExprAddrOfExprAux g mustTakeAddress useReadonlyForGenericArrayAddress mut e addrExprVal m
    match optBind with 
    | None -> (fun x -> x), addre
    | Some (tmp, rval) -> (fun x -> mkCompGenLet m tmp rval x), addre

let mkTupleFieldGet g (tupInfo, e, tinst, i, m) = 
    let wrap, e' = mkExprAddrOfExpr g (evalTupInfoIsStruct tupInfo) false NeverMutates e None m
    wrap (mkTupleFieldGetViaExprAddr(tupInfo, e', tinst, i, m))

let mkRecdFieldGet g (e, fref:RecdFieldRef, tinst, m) = 
    assert (not (isByrefTy g (tyOfExpr g e)))
    let wrap, e' = mkExprAddrOfExpr g fref.Tycon.IsStructOrEnumTycon false NeverMutates e None m
    wrap (mkRecdFieldGetViaExprAddr(e', fref, tinst, m))

let mkUnionCaseFieldGetUnproven g (e, cref:UnionCaseRef, tinst, j, m) = 
    assert (not (isByrefTy g (tyOfExpr g e)))
    let wrap, e' = mkExprAddrOfExpr g cref.Tycon.IsStructOrEnumTycon false NeverMutates e None m
    wrap (mkUnionCaseFieldGetUnprovenViaExprAddr (e', cref, tinst, j, m))


let mkArray (argty, args, m) = Expr.Op(TOp.Array, [argty], args, m)

//---------------------------------------------------------------------------
// Compute fixups for letrec's.
//
// Generate an assignment expression that will fixup the recursion 
// amongst the vals on the r.h.s. of a letrec.  The returned expressions 
// include disorderly constructs such as expressions/statements 
// to set closure environments and non-mutable fields. These are only ever 
// generated by the backend code-generator when processing a "letrec"
// construct.
//
// [self] is the top level value that is being fixed
// [exprToFix] is the r.h.s. expression
// [rvs] is the set of recursive vals being bound. 
// [acc] accumulates the expression right-to-left. 
//
// Traversal of the r.h.s. term must happen back-to-front to get the
// uniq's for the lambdas correct in the very rare case where the same lambda
// somehow appears twice on the right.
//---------------------------------------------------------------------------

let rec IterateRecursiveFixups g (selfv : Val option) rvs ((access : Expr), set) exprToFix  = 
    let exprToFix =  stripExpr exprToFix
    match exprToFix with 
    | Expr.Const _ -> ()
    | Expr.Op (TOp.Tuple tupInfo, argtys, args, m) when not (evalTupInfoIsStruct tupInfo) ->
      args |> List.iteri (fun n -> 
          IterateRecursiveFixups g None rvs 
            (mkTupleFieldGet g (tupInfo, access, argtys, n, m), 
            (fun e -> 
              // NICE: it would be better to do this check in the type checker 
              errorR(Error(FSComp.SR.tastRecursiveValuesMayNotBeInConstructionOfTuple(), m));
              e)))

    | Expr.Op (TOp.UnionCase (c), tinst, args, m) ->
      args |> List.iteri (fun n -> 
          IterateRecursiveFixups g None rvs 
            (mkUnionCaseFieldGetUnprovenViaExprAddr (access, c, tinst, n, m), 
             (fun e -> 
               // NICE: it would be better to do this check in the type checker 
               let tcref = c.TyconRef
               if not (c.FieldByIndex(n)).IsMutable && not (entityRefInThisAssembly g.compilingFslib tcref) then
                 errorR(Error(FSComp.SR.tastRecursiveValuesMayNotAppearInConstructionOfType(tcref.LogicalName), m));
               mkUnionCaseFieldSet (access, c, tinst, n, e, m))))

    | Expr.Op (TOp.Recd (_, tcref), tinst, args, m) -> 
      (tcref.TrueInstanceFieldsAsRefList, args) ||> List.iter2 (fun fref arg -> 
          let fspec = fref.RecdField
          IterateRecursiveFixups g None rvs 
            (mkRecdFieldGetViaExprAddr(access, fref, tinst, m), 
             (fun e -> 
               // NICE: it would be better to do this check in the type checker 
               if not fspec.IsMutable && not (entityRefInThisAssembly g.compilingFslib tcref) then
                 errorR(Error(FSComp.SR.tastRecursiveValuesMayNotBeAssignedToNonMutableField(fspec.rfield_id.idText, tcref.LogicalName), m));
               mkRecdFieldSetViaExprAddr (access, fref, tinst, e, m))) arg )
    | Expr.Val _
    | Expr.Lambda _
    | Expr.Obj _
    | Expr.TyChoose _
    | Expr.TyLambda _ -> 
        rvs selfv access set exprToFix
    | _ -> ()




//--------------------------------------------------------------------------
// computations on constraints
//-------------------------------------------------------------------------- 

let JoinTyparStaticReq r1 r2 = 
  match r1, r2 with
  | NoStaticReq, r | r, NoStaticReq -> r 
  | HeadTypeStaticReq, r | r, HeadTypeStaticReq -> r
  


//-------------------------------------------------------------------------
// ExprFolder - fold steps
//-------------------------------------------------------------------------

type ExprFolder<'T> = 
    { exprIntercept    : ('T -> Expr -> 'T) -> 'T -> Expr  -> 'T option;   
      // the bool is 'bound in dtree' 
      valBindingSiteIntercept          : 'T -> bool * Val  -> 'T;                     
      // these values are always bound to these expressions. bool indicates 'recursively' 
      nonRecBindingsIntercept         : 'T -> Binding -> 'T;         
      recBindingsIntercept         : 'T -> Bindings -> 'T;         
      dtreeIntercept         : 'T -> DecisionTree -> 'T;                     
      targetIntercept  : ('T -> Expr -> 'T) -> 'T -> DecisionTreeTarget  -> 'T option; 
      tmethodIntercept : ('T -> Expr -> 'T) -> 'T -> ObjExprMethod -> 'T option; 
    }

let ExprFolder0 =
    { exprIntercept    = (fun _exprF _z _x -> None);
      valBindingSiteIntercept          = (fun z _b  -> z);
      nonRecBindingsIntercept         = (fun z _bs -> z);
      recBindingsIntercept         = (fun z _bs -> z);
      dtreeIntercept         = (fun z _dt -> z);
      targetIntercept  = (fun _exprF _z _x -> None);
      tmethodIntercept = (fun _exprF _z _x -> None); }


//-------------------------------------------------------------------------
// FoldExpr
//-------------------------------------------------------------------------

/// Adapted from usage info folding.
/// Collecting from exprs at moment.
/// To collect ids etc some additional folding needed, over formals etc.
type ExprFolders<'State> (folders : _ ExprFolder) =
    let mutable exprFClosure = Unchecked.defaultof<_> // prevent reallocation of closure
    let rec exprsF z xs = List.fold exprFClosure z xs
    and exprF (z: 'State) x =
        match folders.exprIntercept exprFClosure z x with // fold this node, then recurse 
        | Some z -> z // intercepted 
        | None ->     // structurally recurse 
            match x with
            | Expr.Const _  -> z
            | Expr.Val _ -> z
            | Expr.Op (_c, _tyargs, args, _) -> exprsF z args
            | Expr.Sequential (x0, x1, _dir, _, _)  -> exprsF z [x0;x1]
            | Expr.Lambda(_lambdaId , _ctorThisValOpt, _baseValOpt, _argvs, body, _m, _rty) -> exprF  z body
            | Expr.TyLambda(_lambdaId, _argtyvs, body, _m, _rty) -> exprF  z body
            | Expr.TyChoose(_, body, _) -> exprF  z body

            | Expr.App (f, _fty, _tys, argtys, _) -> 
                let z = exprF z f
                let z = exprsF z argtys
                z
            | Expr.LetRec (binds, body, _, _) -> 
                let z = valBindsF false z binds
                let z = exprF z body
                z
            | Expr.Let    (bind, body, _, _)  -> 
                let z = valBindF false z bind
                let z = exprF z body
                z
            | Expr.Link rX -> exprF z (!rX)

            | Expr.Match (_spBind, _exprm, dtree, targets, _m, _ty)                 -> 
                let z = dtreeF z dtree
                let z = Array.fold targetF z targets
                z
            | Expr.Quote(e, {contents=Some(_typeDefs, _argTypes, argExprs, _)}, _, _, _)  -> 
                let z = exprF z e
                exprsF z argExprs

            | Expr.Quote(e, {contents=None}, _, _m, _) -> 
                exprF z e

            | Expr.Obj (_n, _typ, _basev, basecall, overrides, iimpls, _m)    -> 
                let z = exprF z basecall
                let z = List.fold tmethodF z overrides
                let z = List.fold (foldOn snd (List.fold tmethodF)) z iimpls
                z

            | Expr.StaticOptimization (_tcs, csx, x, _) -> exprsF z [csx;x]

    and valBindF dtree z bind =
        let z = folders.nonRecBindingsIntercept z bind
        bindF dtree z bind 

    and valBindsF dtree z binds =
        let z = folders.recBindingsIntercept z binds
        List.fold (bindF dtree) z binds 

    and bindF dtree z (bind:Binding) =
        let z = folders.valBindingSiteIntercept z (dtree, bind.Var)
        exprF z bind.Expr

    and dtreeF z dtree =
        let z = folders.dtreeIntercept z dtree
        match dtree with
        | TDBind (bind, rest)            -> 
            let z = valBindF true z bind
            dtreeF z rest
        | TDSuccess (args, _)            -> exprsF z args
        | TDSwitch (test, dcases, dflt, _) -> 
            let z = exprF z test
            let z = List.fold dcaseF z dcases
            let z = Option.fold dtreeF z dflt
            z

    and dcaseF z = function
        TCase (_, dtree)   -> dtreeF z dtree (* not collecting from test *)

    and targetF z x =
        match folders.targetIntercept exprFClosure z x with 
        | Some z -> z // intercepted 
        | None ->     // structurally recurse 
            let (TTarget (_, body, _)) = x
            exprF z body
              
    and tmethodF z x =
        match folders.tmethodIntercept exprFClosure z x with 
        | Some z -> z // intercepted 
        | None ->     // structurally recurse 
            let (TObjExprMethod(_, _, _, _, e, _)) = x
            exprF z e

    and mexprF z x =
        match x with 
        | ModuleOrNamespaceExprWithSig(_, def, _) -> mdefF z def

    and mdefF z x = 
        match x with
        | TMDefRec(_, _, mbinds, _) -> 
            (* REVIEW: also iterate the abstract slot vspecs hidden in the _vslots field in the tycons *)
            let z = List.fold mbindF z mbinds
            z
        | TMDefLet(bind, _) -> valBindF false z bind
        | TMDefDo(e, _) -> exprF z e
        | TMDefs defs -> List.fold mdefF z defs 
        | TMAbstract x -> mexprF z x

    and mbindF z x = 
        match x with 
        | ModuleOrNamespaceBinding.Binding b -> valBindF false z b
        | ModuleOrNamespaceBinding.Module(_, def) -> mdefF z def

    and implF z x = foldTImplFile mexprF z x

    do exprFClosure <- exprF // allocate one instance of this closure
    member x.FoldExpr = exprF
    member x.FoldImplFile = implF

let FoldExpr     folders state expr = ExprFolders(folders).FoldExpr state expr
let FoldImplFile folders state implFile = ExprFolders(folders).FoldImplFile state implFile 

#if DEBUG
//-------------------------------------------------------------------------
// ExprStats
//-------------------------------------------------------------------------

let ExprStats x =
  let count = ref 0
  let folders = {ExprFolder0 with exprIntercept = (fun _ _ _ -> (count := !count + 1; None))}
  let () = FoldExpr folders () x
  string !count ^ " TExpr nodes"
#endif
    
//-------------------------------------------------------------------------
// 
//------------------------------------------------------------------------- 

let mkString (g:TcGlobals) m n = Expr.Const(Const.String n, m, g.string_ty)
let mkBool (g:TcGlobals) m b = Expr.Const(Const.Bool b, m, g.bool_ty)
let mkByte (g:TcGlobals) m b = Expr.Const(Const.Byte b, m, g.byte_ty)
let mkUInt16 (g:TcGlobals) m b = Expr.Const(Const.UInt16 b, m, g.uint16_ty)
let mkTrue g m = mkBool g m true
let mkFalse g m = mkBool g m false
let mkUnit (g:TcGlobals) m = Expr.Const(Const.Unit, m, g.unit_ty)
let mkInt32 (g:TcGlobals) m n =  Expr.Const(Const.Int32 n, m, g.int32_ty)
let mkInt g m n =  mkInt32 g m (n)
let mkZero g m =  mkInt g m 0
let mkOne g m =  mkInt g m 1
let mkTwo g m =  mkInt g m 2
let mkMinusOne g  m =  mkInt g m (-1)

let destInt32 = function Expr.Const(Const.Int32 n, _, _) -> Some n | _ -> None

let isIDelegateEventType g ty     = isAppTy g ty && tyconRefEq g g.fslib_IDelegateEvent_tcr (tcrefOfAppTy g ty)
let destIDelegateEventType g ty   = 
    if isIDelegateEventType g ty then 
        match argsOfAppTy g ty with 
        | [ty1] -> ty1
        | _ -> failwith "destIDelegateEventType: internal error"
    else failwith "destIDelegateEventType: not an IDelegateEvent type"
let mkIEventType (g:TcGlobals) ty1 ty2 = TType_app (g.fslib_IEvent2_tcr, [ty1;ty2])
let mkIObservableType (g:TcGlobals) ty1 = TType_app (g.tcref_IObservable, [ty1])
let mkIObserverType (g:TcGlobals) ty1 = TType_app (g.tcref_IObserver, [ty1])

let mkRefCellContentsRef (g:TcGlobals) = mkRecdFieldRef g.refcell_tcr_canon "contents"

let mkSequential spSeq m e1 e2 = Expr.Sequential(e1, e2, NormalSeq, spSeq, m)
let mkCompGenSequential m e1 e2 = mkSequential SuppressSequencePointOnExprOfSequential m e1 e2
let rec mkSequentials spSeq g m es = 
    match es with 
    | [e] -> e 
    | e::es -> mkSequential spSeq m e (mkSequentials spSeq g m es) 
    | [] -> mkUnit g m

let mkGetArg0 m ty = mkAsmExpr( [ mkLdarg0 ], [], [], [ty], m) 

//-------------------------------------------------------------------------
// Tuples...
//------------------------------------------------------------------------- 
 
let mkAnyTupled g m tupInfo es tys = 
    match es with 
    | [] -> mkUnit g m 
    | [e] -> e
    | _ -> Expr.Op (TOp.Tuple tupInfo, tys, es, m)


let mkRefTupled g m es tys = mkAnyTupled g m tupInfoRef es tys
let mkRefTupledNoTypes g m args = mkRefTupled g m args (List.map (tyOfExpr g) args)
let mkRefTupledVars g m vs = mkRefTupled g m (List.map (exprForVal m) vs) (typesOfVals vs)

//--------------------------------------------------------------------------
// Permute expressions
//--------------------------------------------------------------------------
    
let inversePerm (sigma:int array) =
    let n = sigma.Length
    let invSigma = Array.create n -1
    for i = 0 to n-1 do
        let sigma_i = sigma.[i]
        // assert( invSigma.[sigma_i] = -1 ); 
        invSigma.[sigma_i] <- i
    invSigma
  
let permute (sigma:int[]) (data:'T[]) = 
    let n = sigma.Length
    let invSigma = inversePerm sigma
    Array.init n (fun i -> data.[invSigma.[i]])
  
let rec existsR a b pred = if a<=b then pred a || existsR (a+1) b pred else false

// Given a permutation for record fields, work out the highest entry that we must lift out
// of a record initialization. Lift out xi if xi goes to position that will be preceded by an expr with an effect 
// that originally followed xi.  If one entry gets lifted then everything before it also gets lifted.
let liftAllBefore sigma = 
    let invSigma = inversePerm sigma

    let lifted = 
        [ for i in 0 .. sigma.Length - 1 do 
            let i' = sigma.[i]
            if existsR 0 (i' - 1) (fun j' -> invSigma.[j'] > i)  then 
                    yield i ]

    if lifted.IsEmpty then 0 else List.max lifted + 1


///  Put record field assignments in order.
//
let permuteExprList (sigma:int[]) (exprs: Expr list) (typ: TType list) (names:string list) =
    let typ, names = (Array.ofList typ, Array.ofList names)

    let liftLim = liftAllBefore sigma 

    let rewrite rbinds (i, expri:Expr) =
        if i < liftLim then
            let tmpvi, tmpei = mkCompGenLocal expri.Range names.[i] typ.[i]
            let bindi = mkCompGenBind tmpvi expri
            tmpei, bindi :: rbinds
        else
            expri, rbinds
 
    let newExprs, reversedBinds = List.mapFold rewrite [] (exprs |> List.indexed)
    let binds = List.rev reversedBinds
    let reorderedExprs  = permute sigma (Array.ofList newExprs)
    binds, Array.toList reorderedExprs
    
//-------------------------------------------------------------------------
// Build record expressions...
//------------------------------------------------------------------------- 


/// Evaluate the expressions in the original order, but build a record with the results in field order 
/// Note some fields may be static. If this were not the case we could just use 
///     let sigma       = Array.map #Index  ()  
/// However the presence of static fields means .Index may index into a non-compact set of instance field indexes. 
/// We still need to sort by index. 
let mkRecordExpr g (lnk, tcref, tinst, rfrefs:RecdFieldRef list, args, m) =  
    // Remove any abbreviations 
    let tcref, tinst = destAppTy g (mkAppTy tcref tinst)
    
    let rfrefsArray = rfrefs |> List.indexed |> Array.ofList
    rfrefsArray |> Array.sortInPlaceBy (fun (_, r) -> r.Index)
    let sigma = Array.create rfrefsArray.Length -1
    Array.iteri (fun j (i, _) -> 
        if sigma.[i] <> -1 then error(InternalError("bad permutation", m))
        sigma.[i] <- j)  rfrefsArray
    
    let argTyps     = List.map (fun rfref  -> actualTyOfRecdFieldRef rfref tinst) rfrefs
    let names       = rfrefs |> List.map (fun rfref -> rfref.FieldName)
    let binds, args  = permuteExprList sigma args argTyps names
    mkLetsBind m binds (Expr.Op (TOp.Recd(lnk, tcref), tinst, args, m))
  

//-------------------------------------------------------------------------
// List builders
//------------------------------------------------------------------------- 
 
let mkRefCell     g m ty e = mkRecordExpr g (RecdExpr, g.refcell_tcr_canon, [ty], [mkRefCellContentsRef g], [e], m)
let mkRefCellGet g m ty e = mkRecdFieldGetViaExprAddr (e, mkRefCellContentsRef g, [ty], m)
let mkRefCellSet g m ty e1 e2 = mkRecdFieldSetViaExprAddr (e1, mkRefCellContentsRef g, [ty], e2, m)

let mkNil (g:TcGlobals) m ty = mkUnionCaseExpr (g.nil_ucref, [ty], [], m)
let mkCons (g:TcGlobals) ty h t = mkUnionCaseExpr (g.cons_ucref, [ty], [h;t], unionRanges h.Range t.Range)

let mkCompGenLocalAndInvisbleBind g nm m e = 
    let locv, loce = mkCompGenLocal m nm (tyOfExpr g e)
    locv, loce, mkInvisibleBind locv e 

//----------------------------------------------------------------------------
// Make some fragments of code
//----------------------------------------------------------------------------

let box = IL.I_box (mkILTyvarTy 0us)
let isinst = IL.I_isinst (mkILTyvarTy 0us)
let unbox = IL.I_unbox_any (mkILTyvarTy 0us)
let mkUnbox ty e m = mkAsmExpr ([ unbox ], [ty], [e], [ ty ], m)
let mkBox ty e m = mkAsmExpr ([box], [], [e], [ty], m)
let mkIsInst ty e m = mkAsmExpr ([ isinst ], [ty], [e], [ ty ], m)

let mspec_Type_GetTypeFromHandle (g: TcGlobals) = IL.mkILNonGenericStaticMethSpecInTy(g.ilg.typ_Type, "GetTypeFromHandle", [g.iltyp_RuntimeTypeHandle], g.ilg.typ_Type)
let mspec_String_Length (g: TcGlobals) = mkILNonGenericInstanceMethSpecInTy (g.ilg.typ_String, "get_Length", [], g.ilg.typ_Int32)

let fspec_Missing_Value (g: TcGlobals) = IL.mkILFieldSpecInTy(g.iltyp_Missing, "Value", g.iltyp_Missing)

let mkInitializeArrayMethSpec (g: TcGlobals) = 
  mkILNonGenericStaticMethSpecInTy(mkILNonGenericBoxedTy(g.FindSysILTypeRef "System.Runtime.CompilerServices.RuntimeHelpers"), "InitializeArray", [g.ilg.typ_Array;g.iltyp_RuntimeFieldHandle], ILType.Void)

let mkInvalidCastExnNewobj (g: TcGlobals)  = 
  mkNormalNewobj (mkILCtorMethSpecForTy (mkILNonGenericBoxedTy (g.FindSysILTypeRef "System.InvalidCastException"), []))


let typedExprForIntrinsic _g m (IntrinsicValRef(_, _, _, ty, _) as i) =
    let vref = ValRefForIntrinsic i
    exprForValRef m vref, ty

let mkCallGetGenericComparer (g:TcGlobals) m = typedExprForIntrinsic g m g.get_generic_comparer_info |> fst
let mkCallGetGenericEREqualityComparer (g:TcGlobals) m = typedExprForIntrinsic g m g.get_generic_er_equality_comparer_info |> fst
let mkCallGetGenericPEREqualityComparer (g:TcGlobals) m = typedExprForIntrinsic g m g.get_generic_per_equality_comparer_info |> fst
let mkCallUnbox                (g:TcGlobals) m ty e1    = mkApps g (typedExprForIntrinsic g m g.unbox_info,    [[ty]], [ e1 ], m)
let mkCallUnboxFast            (g:TcGlobals) m ty e1    = mkApps g (typedExprForIntrinsic g m g.unbox_fast_info, [[ty]], [ e1 ], m)
let mkCallTypeTest             (g:TcGlobals) m ty e1    = mkApps g (typedExprForIntrinsic g m g.istype_info,   [[ty]], [ e1 ], m)
let mkCallTypeOf               (g:TcGlobals) m ty       = mkApps g (typedExprForIntrinsic g m g.typeof_info,   [[ty]], [ ], m)
let mkCallTypeDefOf            (g:TcGlobals) m ty       = mkApps g (typedExprForIntrinsic g m g.typedefof_info, [[ty]], [ ], m)

     
let mkCallDispose              (g:TcGlobals) m ty e1         = mkApps g (typedExprForIntrinsic g m g.dispose_info,               [[ty]], [ e1 ], m)
let mkCallSeq                  (g:TcGlobals) m ty e1         = mkApps g (typedExprForIntrinsic g m g.seq_info,                   [[ty]], [ e1 ], m)
let mkCallCreateInstance       (g:TcGlobals) m ty            = mkApps g (typedExprForIntrinsic g m g.create_instance_info,       [[ty]], [ mkUnit g m ], m)

let mkCallGetQuerySourceAsEnumerable      (g:TcGlobals) m ty1 ty2 e1         = mkApps g (typedExprForIntrinsic g m g.query_source_as_enum_info,                   [[ty1;ty2]], [ e1; mkUnit g m  ], m)
let mkCallNewQuerySource                  (g:TcGlobals) m ty1 ty2 e1         = mkApps g (typedExprForIntrinsic g m g.new_query_source_info,                   [[ty1;ty2]], [ e1 ], m)

let mkCallCreateEvent                        (g:TcGlobals) m ty1 ty2 e1 e2 e3 = mkApps g (typedExprForIntrinsic g m g.create_event_info,       [[ty1;ty2]], [ e1;e2;e3 ], m)
let mkCallGenericComparisonWithComparerOuter (g:TcGlobals) m ty comp e1 e2    = mkApps g (typedExprForIntrinsic g m g.generic_comparison_withc_outer_info, [[ty]], [  comp;e1;e2 ], m)
let mkCallGenericEqualityEROuter             (g:TcGlobals) m ty e1 e2         = mkApps g (typedExprForIntrinsic g m g.generic_equality_er_outer_info,     [[ty]], [  e1;e2 ], m)
let mkCallGenericEqualityWithComparerOuter   (g:TcGlobals) m ty comp e1 e2    = mkApps g (typedExprForIntrinsic g m g.generic_equality_withc_outer_info, [[ty]], [comp;e1;e2], m)
let mkCallGenericHashWithComparerOuter       (g:TcGlobals) m ty comp e1       = mkApps g (typedExprForIntrinsic g m g.generic_hash_withc_outer_info, [[ty]], [comp;e1], m)

let mkCallEqualsOperator                     (g:TcGlobals) m ty e1 e2         = mkApps g (typedExprForIntrinsic g m g.equals_operator_info,     [[ty]], [  e1;e2 ], m)
let mkCallNotEqualsOperator                  (g:TcGlobals) m ty e1 e2         = mkApps g (typedExprForIntrinsic g m g.not_equals_operator,     [[ty]], [  e1;e2 ], m)
let mkCallLessThanOperator                   (g:TcGlobals) m ty e1 e2         = mkApps g (typedExprForIntrinsic g m g.less_than_operator,     [[ty]], [  e1;e2 ], m)
let mkCallLessThanOrEqualsOperator           (g:TcGlobals) m ty e1 e2         = mkApps g (typedExprForIntrinsic g m g.less_than_or_equals_operator,     [[ty]], [  e1;e2 ], m)
let mkCallGreaterThanOperator                (g:TcGlobals) m ty e1 e2         = mkApps g (typedExprForIntrinsic g m g.greater_than_operator,     [[ty]], [  e1;e2 ], m)
let mkCallGreaterThanOrEqualsOperator        (g:TcGlobals) m ty e1 e2         = mkApps g (typedExprForIntrinsic g m g.greater_than_or_equals_operator,     [[ty]], [  e1;e2 ], m)

let mkCallAdditionOperator (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.unchecked_addition_info, [[ty; ty; ty]], [e1;e2], m)
let mkCallSubtractionOperator (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.unchecked_subtraction_info, [[ty; ty; ty]], [e1;e2], m)
let mkCallMultiplyOperator (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.unchecked_multiply_info, [[ty; ty; ty]], [e1;e2], m)
let mkCallDivisionOperator (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.unchecked_division_info, [[ty; ty; ty]], [e1;e2], m)
let mkCallModulusOperator (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.unchecked_modulus_info, [[ty; ty; ty]], [e1;e2], m)
let mkCallBitwiseAndOperator (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.bitwise_and_info, [[ty]], [e1;e2], m)
let mkCallBitwiseOrOperator (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.bitwise_or_info, [[ty]], [e1;e2], m)
let mkCallBitwiseXorOperator (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.bitwise_xor_info, [[ty]], [e1;e2], m)
let mkCallShiftLeftOperator (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.bitwise_shift_left_info, [[ty]], [e1;e2], m)
let mkCallShiftRightOperator (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.bitwise_shift_right_info, [[ty]], [e1;e2], m)

let mkCallUnaryNegOperator  (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.unchecked_unary_minus_info, [[ty]], [e1], m)
let mkCallUnaryNotOperator  (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.bitwise_unary_not_info, [[ty]], [e1], m)

let mkCallAdditionChecked   (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.checked_addition_info, [[ty; ty; ty]], [e1;e2], m)
let mkCallSubtractionChecked (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.checked_subtraction_info, [[ty; ty; ty]], [e1;e2], m)
let mkCallMultiplyChecked   (g:TcGlobals) m ty e1 e2 = mkApps g (typedExprForIntrinsic g m g.checked_multiply_info, [[ty; ty; ty]], [e1;e2], m)
let mkCallUnaryNegChecked   (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.checked_unary_minus_info, [[ty]], [e1], m)

let mkCallToByteChecked     (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.byte_checked_info, [[ty]], [e1], m)
let mkCallToSByteChecked    (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.sbyte_checked_info, [[ty]], [e1], m)
let mkCallToInt16Checked    (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.int16_checked_info, [[ty]], [e1], m)
let mkCallToUInt16Checked   (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.uint16_checked_info, [[ty]], [e1], m)
let mkCallToIntChecked      (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.int_checked_info, [[ty]], [e1], m)
let mkCallToInt32Checked    (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.int32_checked_info, [[ty]], [e1], m)
let mkCallToUInt32Checked   (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.uint32_checked_info, [[ty]], [e1], m)
let mkCallToInt64Checked    (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.int64_checked_info, [[ty]], [e1], m)
let mkCallToUInt64Checked   (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.uint64_checked_info, [[ty]], [e1], m)
let mkCallToIntPtrChecked   (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.nativeint_checked_info, [[ty]], [e1], m)
let mkCallToUIntPtrChecked  (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.unativeint_checked_info, [[ty]], [e1], m)

let mkCallToByteOperator    (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.byte_operator_info, [[ty]], [e1], m)
let mkCallToSByteOperator   (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.sbyte_operator_info, [[ty]], [e1], m)
let mkCallToInt16Operator   (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.int16_operator_info, [[ty]], [e1], m)
let mkCallToUInt16Operator  (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.uint16_operator_info, [[ty]], [e1], m)
let mkCallToIntOperator     (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.int_operator_info, [[ty]], [e1], m)
let mkCallToInt32Operator   (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.int32_operator_info, [[ty]], [e1], m)
let mkCallToUInt32Operator  (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.uint32_operator_info, [[ty]], [e1], m)
let mkCallToInt64Operator   (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.int64_operator_info, [[ty]], [e1], m)
let mkCallToUInt64Operator  (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.uint64_operator_info, [[ty]], [e1], m)
let mkCallToSingleOperator  (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.float32_operator_info, [[ty]], [e1], m)
let mkCallToDoubleOperator  (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.float_operator_info, [[ty]], [e1], m)
let mkCallToIntPtrOperator  (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.nativeint_operator_info, [[ty]], [e1], m)
let mkCallToUIntPtrOperator (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.unativeint_operator_info, [[ty]], [e1], m)

let mkCallToCharOperator    (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.char_operator_info, [[ty]], [e1], m)
let mkCallToEnumOperator    (g:TcGlobals) m ty e1 = mkApps g (typedExprForIntrinsic g m g.enum_operator_info, [[ty]], [e1], m)

let mkCallArrayLength (g:TcGlobals) m ty e1                    = mkApps g (typedExprForIntrinsic g m g.array_length_info, [[ty]], [e1], m)
let mkCallArrayGet   (g:TcGlobals) m ty e1 idx1                = mkApps g (typedExprForIntrinsic g m g.array_get_info, [[ty]], [ e1 ; idx1 ], m)
let mkCallArray2DGet (g:TcGlobals) m ty e1 idx1 idx2           = mkApps g (typedExprForIntrinsic g m g.array2D_get_info, [[ty]], [ e1 ; idx1; idx2 ], m)
let mkCallArray3DGet (g:TcGlobals) m ty e1 idx1 idx2 idx3      = mkApps g (typedExprForIntrinsic g m g.array3D_get_info, [[ty]], [ e1 ; idx1; idx2; idx3 ], m)
let mkCallArray4DGet (g:TcGlobals) m ty e1 idx1 idx2 idx3 idx4 = mkApps g (typedExprForIntrinsic g m g.array4D_get_info, [[ty]], [ e1 ; idx1; idx2; idx3; idx4 ], m)
let mkCallArraySet   (g:TcGlobals) m ty e1 idx1 v              = mkApps g (typedExprForIntrinsic g m g.array_set_info, [[ty]], [ e1 ; idx1; v ], m)
let mkCallArray2DSet (g:TcGlobals) m ty e1 idx1 idx2 v         = mkApps g (typedExprForIntrinsic g m g.array2D_set_info, [[ty]], [ e1 ; idx1; idx2; v ], m)
let mkCallArray3DSet (g:TcGlobals) m ty e1 idx1 idx2 idx3 v    = mkApps g (typedExprForIntrinsic g m g.array3D_set_info, [[ty]], [ e1 ; idx1; idx2; idx3; v ], m)
let mkCallArray4DSet (g:TcGlobals) m ty e1 idx1 idx2 idx3 idx4 v = mkApps g (typedExprForIntrinsic g m g.array4D_set_info, [[ty]], [ e1 ; idx1; idx2; idx3; idx4; v ], m)

let mkCallHash       (g:TcGlobals) m ty e1    = mkApps g (typedExprForIntrinsic g m g.hash_info,      [[ty]], [ e1 ], m)
let mkCallBox        (g:TcGlobals) m ty e1    = mkApps g (typedExprForIntrinsic g m g.box_info,       [[ty]], [ e1 ], m)
let mkCallIsNull     (g:TcGlobals) m ty e1    = mkApps g (typedExprForIntrinsic g m g.isnull_info,    [[ty]], [ e1 ], m)
let mkCallIsNotNull  (g:TcGlobals) m ty e1    = mkApps g (typedExprForIntrinsic g m g.isnotnull_info, [[ty]], [ e1 ], m)
let mkCallRaise      (g:TcGlobals) m ty e1    = mkApps g (typedExprForIntrinsic g m g.raise_info,     [[ty]], [ e1 ], m)

let mkCallNewDecimal (g:TcGlobals) m (e1, e2, e3, e4, e5)          = mkApps g (typedExprForIntrinsic g m g.new_decimal_info, [], [ e1;e2;e3;e4;e5 ], m)

let mkCallNewFormat (g:TcGlobals) m aty bty cty dty ety e1    = mkApps g (typedExprForIntrinsic g m g.new_format_info, [[aty;bty;cty;dty;ety]], [ e1 ], m)

let TryEliminateDesugaredConstants g m c = 
    match c with 
    | Const.Decimal d -> 
        match System.Decimal.GetBits(d) with 
        | [| lo;med;hi; signExp |] -> 
            let scale = (min (((signExp &&& 0xFF0000) >>> 16) &&& 0xFF) 28) |> byte
            let isNegative = (signExp &&& 0x80000000) <> 0
            Some(mkCallNewDecimal g m (mkInt g m lo, mkInt g m med, mkInt g m hi, mkBool g m isNegative, mkByte g m scale) )
        | _ -> failwith "unreachable"
    | _ -> 
        None

let mkSeqTy (g:TcGlobals) ty = mkAppTy g.seq_tcr [ty] 
let mkIEnumeratorTy (g:TcGlobals) ty = mkAppTy g.tcref_System_Collections_Generic_IEnumerator [ty] 

let mkCallSeqCollect g m alphaTy betaTy arg1 arg2 = 
    let enumty2 = try rangeOfFunTy g (tyOfExpr g arg1) with _ -> (* defensive programming *) (mkSeqTy g betaTy)
    mkApps g (typedExprForIntrinsic g m g.seq_collect_info, [[alphaTy;enumty2;betaTy]], [ arg1; arg2 ], m) 
                  
let mkCallSeqUsing g m resourceTy elemTy arg1 arg2 = 
    // We're instantiating val using : 'a -> ('a -> 'sb) -> seq<'b> when 'sb :> seq<'b> and 'a :> IDisposable 
    // We set 'sb -> range(typeof(arg2)) 
    let enumty = try rangeOfFunTy g (tyOfExpr g arg2) with _ -> (* defensive programming *) (mkSeqTy g elemTy)
    mkApps g (typedExprForIntrinsic g m g.seq_using_info, [[resourceTy;enumty;elemTy]], [ arg1; arg2 ], m) 
                  
let mkCallSeqDelay g m elemTy arg1 = 
    mkApps g (typedExprForIntrinsic g m g.seq_delay_info, [[elemTy]], [ arg1 ], m) 
                  
let mkCallSeqAppend g m elemTy arg1 arg2 = 
    mkApps g (typedExprForIntrinsic g m g.seq_append_info, [[elemTy]], [ arg1; arg2 ], m) 

let mkCallSeqGenerated g m elemTy arg1 arg2 = 
    mkApps g (typedExprForIntrinsic g m g.seq_generated_info, [[elemTy]], [ arg1; arg2 ], m) 
                       
let mkCallSeqFinally g m elemTy arg1 arg2 = 
    mkApps g (typedExprForIntrinsic g m g.seq_finally_info, [[elemTy]], [ arg1; arg2 ], m) 
                       
let mkCallSeqOfFunctions g m ty1 ty2 arg1 arg2 arg3 = 
    mkApps g (typedExprForIntrinsic g m g.seq_of_functions_info, [[ty1;ty2]], [ arg1; arg2; arg3  ], m) 
                  
let mkCallSeqToArray g m elemTy arg1 =  
    mkApps g (typedExprForIntrinsic g m g.seq_to_array_info, [[elemTy]], [ arg1 ], m) 
                  
let mkCallSeqToList g m elemTy arg1 = 
    mkApps g (typedExprForIntrinsic g m g.seq_to_list_info, [[elemTy]], [ arg1 ], m) 
                  
let mkCallSeqMap g m inpElemTy genElemTy arg1 arg2 = 
    mkApps g (typedExprForIntrinsic g m g.seq_map_info, [[inpElemTy;genElemTy]], [ arg1; arg2 ], m) 
                  
let mkCallSeqSingleton g m ty1 arg1 = 
    mkApps g (typedExprForIntrinsic g m g.seq_singleton_info, [[ty1]], [ arg1 ], m) 
                  
let mkCallSeqEmpty g m ty1 = 
    mkApps g (typedExprForIntrinsic g m g.seq_empty_info, [[ty1]], [ ], m) 
                 
let mkCallDeserializeQuotationFSharp20Plus g m e1 e2 e3 e4 = 
    let args = [ e1; e2; e3; e4 ]
    mkApps g (typedExprForIntrinsic g m g.deserialize_quoted_FSharp_20_plus_info, [], [ mkRefTupledNoTypes g m args ], m)

let mkCallDeserializeQuotationFSharp40Plus g m e1 e2 e3 e4 e5 = 
    let args = [ e1; e2; e3; e4; e5 ]
    mkApps g (typedExprForIntrinsic g m g.deserialize_quoted_FSharp_40_plus_info, [], [ mkRefTupledNoTypes g m args ], m)

let mkCallCastQuotation g m ty e1 = 
    mkApps g (typedExprForIntrinsic g m g.cast_quotation_info, [[ty]], [ e1 ], m)

let mkCallLiftValueWithName (g:TcGlobals) m ty nm e1 = 
    let vref = ValRefForIntrinsic g.lift_value_with_name_info 
    // Use "Expr.ValueWithName" if it exists in FSharp.Core
    match vref.TryDeref with
    | VSome _ ->
        mkApps g (typedExprForIntrinsic g m g.lift_value_with_name_info , [[ty]], [mkRefTupledNoTypes g m [e1; mkString g m nm]], m)
    | VNone ->
        mkApps g (typedExprForIntrinsic g m g.lift_value_info , [[ty]], [e1], m)

let mkCallLiftValueWithDefn g m qty e1 = 
    assert isQuotedExprTy g qty
    let ty = destQuotedExprTy g qty
    let vref = ValRefForIntrinsic g.lift_value_with_defn_info 
    // Use "Expr.WithValue" if it exists in FSharp.Core
    match vref.TryDeref with
    | VSome _ ->
        let copyOfExpr = copyExpr g ValCopyFlag.CloneAll e1
        let quoteOfCopyOfExpr = Expr.Quote(copyOfExpr, ref None, false, m, qty)
        mkApps g (typedExprForIntrinsic g m g.lift_value_with_defn_info , [[ty]], [mkRefTupledNoTypes g m [e1; quoteOfCopyOfExpr]], m)
    | VNone ->
        Expr.Quote(e1, ref None, false, m, qty)

let mkCallCheckThis g m ty e1 = 
    mkApps g (typedExprForIntrinsic g m g.check_this_info, [[ty]], [e1], m)

let mkCallFailInit g m = 
    mkApps g (typedExprForIntrinsic g m g.fail_init_info , [], [mkUnit g m], m)

let mkCallFailStaticInit g m = 
    mkApps g (typedExprForIntrinsic g m g.fail_static_init_info , [], [mkUnit g m], m)

let mkCallQuoteToLinqLambdaExpression g m ty e1 = 
    mkApps g (typedExprForIntrinsic g m g.quote_to_linq_lambda_info , [[ty]], [e1], m)

let mkLazyDelayed g m ty f = mkApps g (typedExprForIntrinsic g m g.lazy_create_info, [[ty]], [ f ], m) 
let mkLazyForce g m ty e = mkApps g (typedExprForIntrinsic g m g.lazy_force_info, [[ty]], [ e; mkUnit g m ], m) 

let mkGetString g m e1 e2 = mkApps g (typedExprForIntrinsic g m g.getstring_info, [], [e1;e2], m)
let mkGetStringChar = mkGetString
let mkGetStringLength g m e =
    let mspec = mspec_String_Length g
    /// ILCall(useCallvirt, isProtected, valu, newobj, valUseFlags, isProp, noTailCall, mref, actualTypeInst, actualMethInst, retTy)
    Expr.Op(TOp.ILCall(false, false, false, false, ValUseFlag.NormalValUse, true, false, mspec.MethodRef, [], [], [g.int32_ty]), [], [e], m)


// Quotations can't contain any IL.
// As a result, we aim to get rid of all IL generation in the typechecker and pattern match
// compiler, or else train the quotation generator to understand the generated IL. 
// Hence each of the following are marked with places where they are generated.

// Generated by the optimizer and the encoding of 'for' loops     
let mkDecr (g:TcGlobals) m e = mkAsmExpr([ IL.AI_sub  ], [], [e; mkOne g m], [g.int_ty], m)
let mkIncr (g:TcGlobals) m e = mkAsmExpr([ IL.AI_add  ], [], [mkOne g m; e], [g.int_ty], m)

// Generated by the pattern match compiler and the optimizer for
//    1. array patterns
//    2. optimizations associated with getting 'for' loops into the shape expected by the JIT.
// 
// NOTE: The conv.i4 assumes that int_ty is int32. Note: ldlen returns native UNSIGNED int 
let mkLdlen (g:TcGlobals) m arre = mkAsmExpr ([ IL.I_ldlen; (IL.AI_conv IL.DT_I4) ], [], [ arre ], [ g.int_ty ], m)
let mkLdelem (_g:TcGlobals) m ty arre idxe = mkAsmExpr ([ IL.I_ldelem_any (ILArrayShape.SingleDimensional, mkILTyvarTy 0us) ], [ty], [ arre;idxe ], [ ty ], m)

// This is generated in equality/compare/hash augmentations and in the pattern match compiler.
// It is understood by the quotation processor and turned into "Equality" nodes.
//
// Note: this is IL assembly code, don't go inserting this in expressions which will be exposed via quotations
let mkILAsmCeq (g:TcGlobals) m e1 e2 = mkAsmExpr ([ IL.AI_ceq  ], [], [e1; e2], [g.bool_ty], m)
let mkILAsmClt (g:TcGlobals) m e1 e2 = mkAsmExpr ([ IL.AI_clt  ], [], [e1; e2], [g.bool_ty], m)

// This is generated in the initialization of the "ctorv" field in the typechecker's compilation of
// an implicit class construction.
let mkNull m ty = Expr.Const(Const.Zero, m, ty)

//----------------------------------------------------------------------------
// rethrow
//----------------------------------------------------------------------------

(* throw, rethrow *)
let mkThrow m ty e = mkAsmExpr ([ IL.I_throw ], [], [e], [ty], m)
let destThrow = function
    | Expr.Op (TOp.ILAsm([IL.I_throw], [ty2]), [], [e], m) -> Some (m, ty2, e)
    | _ -> None
let isThrow x = Option.isSome (destThrow x)

// rethrow - parsed as library call - internally represented as op form.
let mkReraiseLibCall (g:TcGlobals) ty m = let ve, vt = typedExprForIntrinsic g m g.reraise_info in Expr.App(ve, vt, [ty], [mkUnit g m], m)
let mkReraise m returnTy = Expr.Op (TOp.Reraise, [returnTy], [], m) (* could suppress unitArg *)

//----------------------------------------------------------------------------
// CompilationMappingAttribute, SourceConstructFlags
//----------------------------------------------------------------------------

let tnameCompilationSourceNameAttr     = FSharpLib.Core + ".CompilationSourceNameAttribute"
let tnameCompilationArgumentCountsAttr = FSharpLib.Core + ".CompilationArgumentCountsAttribute"
let tnameCompilationMappingAttr        = FSharpLib.Core + ".CompilationMappingAttribute"
let tnameSourceConstructFlags          = FSharpLib.Core + ".SourceConstructFlags"

let tref_CompilationArgumentCountsAttr (g:TcGlobals) = mkILTyRef (g.fslibCcu.ILScopeRef, tnameCompilationArgumentCountsAttr)
let tref_CompilationMappingAttr (g:TcGlobals)        = mkILTyRef (g.fslibCcu.ILScopeRef, tnameCompilationMappingAttr)
let tref_CompilationSourceNameAttr (g:TcGlobals)     = mkILTyRef (g.fslibCcu.ILScopeRef, tnameCompilationSourceNameAttr)
let tref_SourceConstructFlags (g:TcGlobals)          = mkILTyRef (g.fslibCcu.ILScopeRef, tnameSourceConstructFlags)

let mkCompilationMappingAttrPrim (g:TcGlobals) k nums = 
    mkILCustomAttribute g.ilg (tref_CompilationMappingAttr g, 
                               ((mkILNonGenericValueTy (tref_SourceConstructFlags g)) :: (nums |> List.map (fun _ -> g.ilg.typ_Int32))), 
                               ((k :: nums) |> List.map (fun n -> ILAttribElem.Int32(n))), 
                               [])
let mkCompilationMappingAttr g kind = mkCompilationMappingAttrPrim g kind []
let mkCompilationMappingAttrWithSeqNum g kind seqNum = mkCompilationMappingAttrPrim g kind [seqNum]
let mkCompilationMappingAttrWithVariantNumAndSeqNum g kind varNum seqNum = mkCompilationMappingAttrPrim g kind [varNum;seqNum]

let mkCompilationArgumentCountsAttr (g:TcGlobals) nums = 
    mkILCustomAttribute g.ilg (tref_CompilationArgumentCountsAttr g, [ mkILArr1DTy g.ilg.typ_Int32 ], 
                               [ILAttribElem.Array (g.ilg.typ_Int32, List.map (fun n -> ILAttribElem.Int32(n)) nums)], 
                               [])

let mkCompilationSourceNameAttr (g:TcGlobals) n = 
    mkILCustomAttribute g.ilg (tref_CompilationSourceNameAttr g, [  g.ilg.typ_String ], 
                               [ILAttribElem.String(Some n)], 
                               [])

let mkCompilationMappingAttrForQuotationResource (g:TcGlobals) (nm, tys: ILTypeRef list) = 
    mkILCustomAttribute g.ilg (tref_CompilationMappingAttr g, 
                               [ g.ilg.typ_String; mkILArr1DTy g.ilg.typ_Type ], 
                               [ ILAttribElem.String (Some nm); ILAttribElem.Array (g.ilg.typ_Type, [ for ty in tys -> ILAttribElem.TypeRef (Some ty) ]) ], 
                               [])

#if !NO_EXTENSIONTYPING
//----------------------------------------------------------------------------
// Decode extensible typing attributes
//----------------------------------------------------------------------------

let isTypeProviderAssemblyAttr (cattr:ILAttribute) = 
    cattr.Method.DeclaringType.BasicQualifiedName = typeof<Microsoft.FSharp.Core.CompilerServices.TypeProviderAssemblyAttribute>.FullName

let TryDecodeTypeProviderAssemblyAttr ilg (cattr:ILAttribute) = 
    if isTypeProviderAssemblyAttr cattr then 
        let parms, _args = decodeILAttribData ilg cattr 
        match parms with // The first parameter to the attribute is the name of the assembly with the compiler extensions.
        | (ILAttribElem.String (Some assemblyName))::_ -> Some assemblyName
        | (ILAttribElem.String None)::_ -> Some null
        | [] -> Some null
        | _ -> None
    else
        None

#endif

//----------------------------------------------------------------------------
// FSharpInterfaceDataVersionAttribute
//----------------------------------------------------------------------------

let tname_SignatureDataVersionAttr = FSharpLib.Core + ".FSharpInterfaceDataVersionAttribute"
let tnames_SignatureDataVersionAttr = splitILTypeName tname_SignatureDataVersionAttr
let tref_SignatureDataVersionAttr () = mkILTyRef(IlxSettings.ilxFsharpCoreLibScopeRef (), tname_SignatureDataVersionAttr)

let mkSignatureDataVersionAttr (g:TcGlobals) ((v1, v2, v3, _) : ILVersionInfo)  = 
    mkILCustomAttribute g.ilg
        (tref_SignatureDataVersionAttr(), 
         [g.ilg.typ_Int32;g.ilg.typ_Int32;g.ilg.typ_Int32], 
         [ILAttribElem.Int32 (int32 v1);
          ILAttribElem.Int32 (int32 v2) ; 
          ILAttribElem.Int32 (int32 v3)], [])

let tname_AutoOpenAttr = FSharpLib.Core + ".AutoOpenAttribute"

let IsSignatureDataVersionAttr cattr = isILAttribByName ([], tname_SignatureDataVersionAttr) cattr
let TryFindAutoOpenAttr (ilg : IL.ILGlobals) cattr = 
    if isILAttribByName ([], tname_AutoOpenAttr) cattr then 
        match decodeILAttribData ilg cattr with 
        |  [ILAttribElem.String s], _ -> s
        |  [], _ -> None
        | _ -> 
            warning(Failure(FSComp.SR.tastUnexpectedDecodeOfAutoOpenAttribute())); 
            None
    else
        None
        
let tname_InternalsVisibleToAttr = "System.Runtime.CompilerServices.InternalsVisibleToAttribute"

let TryFindInternalsVisibleToAttr ilg cattr = 
    if isILAttribByName ([], tname_InternalsVisibleToAttr) cattr then 
        match decodeILAttribData ilg cattr with 
        |  [ILAttribElem.String s], _ -> s
        |  [], _ -> None
        | _ -> 
            warning(Failure(FSComp.SR.tastUnexpectedDecodeOfInternalsVisibleToAttribute())); 
            None
    else
        None

let IsMatchingSignatureDataVersionAttr ilg ((v1, v2, v3, _) : ILVersionInfo)  cattr = 
    IsSignatureDataVersionAttr cattr &&
    match decodeILAttribData ilg cattr with 
    |  [ILAttribElem.Int32 u1; ILAttribElem.Int32 u2;ILAttribElem.Int32 u3 ], _ -> 
        (v1 = uint16 u1) && (v2 = uint16 u2) && (v3 = uint16 u3)
    | _ -> 
        warning(Failure(FSComp.SR.tastUnexpectedDecodeOfInterfaceDataVersionAttribute())); 
        false

let mkCompilerGeneratedAttr (g:TcGlobals) n = 
    mkILCustomAttribute g.ilg (tref_CompilationMappingAttr g, [mkILNonGenericValueTy (tref_SourceConstructFlags g)], [ILAttribElem.Int32(n)], [])

//--------------------------------------------------------------------------
// tupled lambda --> method/function with a given topValInfo specification.
//
// AdjustArityOfLambdaBody: "(vs, body)" represents a lambda "fun (vs) ->  body".  The
// aim is to produce a "static method" represented by a pair
// "(mvs, body)" where mvs has the List.length "arity".
//--------------------------------------------------------------------------


let untupledToRefTupled g vs =
    let untupledTys = typesOfVals vs
    let m = (List.head vs).Range
    let tupledv, tuplede = mkCompGenLocal m "tupledArg" (mkRefTupledTy g untupledTys)
    let untupling_es =  List.mapi (fun i _ ->  mkTupleFieldGet g (tupInfoRef, tuplede, untupledTys, i, m)) untupledTys
    // These are non-sticky - at the caller,any sequence point for 'body' goes on 'body' _after_ the binding has been made
    tupledv, mkInvisibleLets m vs untupling_es 
    
// The required tupled-arity (arity) can either be 1 
// or N, and likewise for the tuple-arity of the input lambda, i.e. either 1 or N 
// where the N's will be identical. 
let AdjustArityOfLambdaBody g arity (vs:Val list) body = 
    let nvs = vs.Length
    if not (nvs = arity || nvs = 1 || arity = 1) then failwith ("lengths don't add up");
    if arity = 0 then 
        vs, body
    elif nvs = arity then 
        vs, body
    elif nvs = 1 then
        let v = vs.Head
        let untupledTys = destRefTupleTy g v.Type
        if  (untupledTys.Length <> arity) then failwith "length untupledTys <> arity";
        let dummyvs, dummyes = 
            untupledTys 
            |> List.mapi (fun i ty -> mkCompGenLocal v.Range (v.LogicalName ^"_"^string i) ty) 
            |> List.unzip 
        // These are non-sticky - any sequence point for 'body' goes on 'body' _after_ the binding has been made
        let body = mkInvisibleLet v.Range v (mkRefTupled g v.Range dummyes untupledTys) body
        dummyvs, body
    else 
        let tupledv, untupler =  untupledToRefTupled g vs
        [tupledv], untupler body

let MultiLambdaToTupledLambda g vs body = 
    match vs with 
    | [] -> failwith "MultiLambdaToTupledLambda: expected some argments"
    | [v] -> v, body 
    | vs -> 
        let tupledv, untupler =  untupledToRefTupled g vs
        tupledv, untupler body 
      

let (|RefTuple|_|) expr = 
    match expr with
    | Expr.Op (TOp.Tuple (TupInfo.Const false), _, args, _) -> Some args
    | _ -> None

let MultiLambdaToTupledLambdaIfNeeded g (vs, arg) body = 
    match vs, arg with 
    | [], _ -> failwith "MultiLambdaToTupledLambda: expected some argments"
    | [v], _ -> [(v, arg)], body 
    | vs, RefTuple args when args.Length = vs.Length -> List.zip vs args, body
    | vs, _  -> 
        let tupledv, untupler =  untupledToRefTupled g vs
        [(tupledv, arg)], untupler body 

//--------------------------------------------------------------------------
// Beta reduction via let-bindings. Reduce immediate apps. of lambdas to let bindings. 
// Includes binding the immediate application of generic
// functions. Input type is the type of the function.  Makes use of the invariant
// that any two expressions have distinct local variables (because we explicitly copy
// expressions).
//------------------------------------------------------------------------ 

let rec MakeApplicationAndBetaReduceAux g (f, fty, tyargsl : TType list list, argsl: Expr list, m) =
  (* let verbose = true in *)
  match f with 
  | Expr.Let(bind, body, mlet, _) ->
      // Lift bindings out, i.e. (let x = e in f) y --> let x = e in f y 
      // This increases the scope of 'x', which I don't like as it mucks with debugging 
      // scopes of variables, but this is an important optimization, especially when the '|>' 
      // notation is used a lot. 
      mkLetBind mlet bind (MakeApplicationAndBetaReduceAux g (body, fty, tyargsl, argsl, m))
  | _ -> 
  match tyargsl with 
  | [] :: rest -> 
     MakeApplicationAndBetaReduceAux g (f, fty, rest, argsl, m)

  | tyargs :: rest -> 
      // Bind type parameters by immediate substitution 
      match f with 
      | Expr.TyLambda(_, tyvs, body, _, bodyty) when tyvs.Length = List.length tyargs -> 
          let tpenv = bindTypars tyvs tyargs emptyTyparInst
          let body = remarkExpr m (instExpr g tpenv body)
          let bodyty' = instType tpenv bodyty
          MakeApplicationAndBetaReduceAux g (body, bodyty', rest, argsl, m) 

      | _ -> 
          let f = mkAppsAux g f fty [tyargs] [] m
          let fty = applyTyArgs g fty tyargs 
          MakeApplicationAndBetaReduceAux g (f, fty, rest, argsl, m)
  | [] -> 
      match argsl with
      | _ :: _ ->
          // Bind term parameters by "let" explicit substitutions 
          // 
          // Only do this if there are enough lambdas for the number of arguments supplied. This is because
          // all arguments get evaluated before application.
          //
          // VALID:
          //      (fun a b -> E[a, b]) t1 t2 ---> let a = t1 in let b = t2 in E[t1, t2]
          // INVALID:
          //      (fun a -> E[a]) t1 t2     ---> let a = t1 in E[a] t2       UNLESS: E[a] has no effects OR t2 has no effects
          
          match tryStripLambdaN argsl.Length f with 
          | Some (argvsl, body) -> 
               assert (argvsl.Length = argsl.Length)
               let pairs, body = List.mapFoldBack (MultiLambdaToTupledLambdaIfNeeded g) (List.zip argvsl argsl) body
               let argvs2, args2 = List.unzip (List.concat pairs)
               mkLetsBind m (mkCompGenBinds argvs2 args2) body
          | _ -> 
              mkExprApplAux g f fty argsl m 

      | [] -> 
          f
      
let MakeApplicationAndBetaReduce g (f, fty, tyargsl, argl, m) = 
  MakeApplicationAndBetaReduceAux g (f, fty, tyargsl, argl, m)

//---------------------------------------------------------------------------
// Adjust for expected usage
// Convert a use of a value to saturate to the given arity.
//--------------------------------------------------------------------------- 

let MakeArgsForTopArgs _g m argtysl tpenv =
    argtysl |> List.mapi (fun i argtys -> 
        argtys |> List.mapi (fun j (argty, argInfo : ArgReprInfo) -> 
            let ty = instType tpenv argty
            let nm = 
               match argInfo.Name with 
               | None -> CompilerGeneratedName ("arg"^ string i^ string j)
               | Some id -> id.idText
            fst (mkCompGenLocal m nm ty)))

let AdjustValForExpectedArity g m (vref:ValRef) flags topValInfo =

    let tps, argtysl, rty, _ = GetTopValTypeInFSharpForm g topValInfo vref.Type m
    let tps' = copyTypars tps
    let tyargs' = List.map mkTyparTy tps'
    let tpenv = bindTypars tps tyargs' emptyTyparInst
    let rty' = instType tpenv rty
    let vsl = MakeArgsForTopArgs g m argtysl tpenv
    let call = MakeApplicationAndBetaReduce g (Expr.Val(vref, flags, m), vref.Type, [tyargs'], (List.map (mkRefTupledVars g m) vsl), m)
    let tauexpr, tauty = 
        List.foldBack 
            (fun vs (e, ty) -> mkMultiLambda m vs (e, ty), (mkRefTupledVarsTy g vs --> ty))
            vsl
            (call, rty')
    // Build a type-lambda expression for the toplevel value if needed... 
    mkTypeLambda m tps' (tauexpr, tauty), tps' +-> tauty


//---------------------------------------------------------------------------
// 


let IsSubsumptionExpr g expr =
    match expr with 
    | Expr.Op (TOp.Coerce, [inputTy;actualTy], [_], _) ->
        isFunTy g actualTy && isFunTy g inputTy   
    | _ -> 
        false

let stripTupledFunTy g ty = 
    let argTys, retTy = stripFunTy g ty
    let curriedArgTys = argTys |> List.map (tryDestRefTupleTy g)
    curriedArgTys, retTy

let (|ExprValWithPossibleTypeInst|_|) expr =
    match expr with 
    | Expr.App(Expr.Val(vref, flags, m), _fty, tyargs, [], _)  ->
        Some(vref, flags, tyargs, m)
    | Expr.Val(vref, flags, m) ->
        Some(vref, flags, [], m)
    | _ -> 
        None

let mkCoerceIfNeeded g tgtTy srcTy expr =
    if typeEquiv g tgtTy srcTy then 
        expr
    else 
        mkCoerceExpr(expr, tgtTy, expr.Range, srcTy)

let mkCompGenLetIn m nm ty e f = 
    let v, ve = mkCompGenLocal m nm ty
    mkCompGenLet m v e (f (v, ve))

/// Take a node representing a coercion from one function type to another, e.g.
///    A -> A * A -> int 
/// to 
///    B -> B * A -> int 
/// and return an expression of the correct type that doesn't use a coercion type. For example
/// return   
///    (fun b1 b2 -> E (b1 :> A) (b2 :> A))
///
///    - Use good names for the closure arguments if available
///    - Create lambda variables if needed, or use the supplied arguments if available.
///
/// Return the new expression and any unused suffix of supplied arguments
///
/// If E is a value with TopInfo then use the arity to help create a better closure.
/// In particular we can create a closure like this:
///    (fun b1 b2 -> E (b1 :> A) (b2 :> A))
/// rather than 
///    (fun b1 -> let clo = E (b1 :> A) in (fun b2 -> clo (b2 :> A)))
/// The latter closures are needed to carefully preserve side effect order
///
/// Note that the results of this translation are visible to quotations

let AdjustPossibleSubsumptionExpr g (expr: Expr) (suppliedArgs: Expr list) : (Expr* Expr list) option =

    match expr with 
    | Expr.Op (TOp.Coerce, [inputTy;actualTy], [exprWithActualTy], m) when 
        isFunTy g actualTy && isFunTy g inputTy  ->
        
        if typeEquiv g actualTy inputTy then 
            Some(exprWithActualTy, suppliedArgs)
        else
            
            let curriedActualArgTys, retTy = stripTupledFunTy g actualTy

            let curriedInputTys, _ = stripFunTy g inputTy

            assert (curriedActualArgTys.Length = curriedInputTys.Length)

            let argTys = (curriedInputTys, curriedActualArgTys) ||> List.mapi2 (fun i x y -> (i, x, y))


            // Use the nice names for a function of known arity and name. Note that 'nice' here also 
            // carries a semantic meaning. For a function with top-info, 
            //   let f (x:A) (y:A) (z:A) = ...
            // we know there are no side effects on the application of 'f' to 1, 2 args. This greatly simplifies
            // the closure built for 
            //   f b1 b2 
            // and indeed for 
            //   f b1 b2 b3
            // we don't build any closure at all, and just return
            //   f (b1 :> A) (b2 :> A) (b3 :> A)
            
            let curriedNiceNames = 
                match stripExpr exprWithActualTy with 
                | ExprValWithPossibleTypeInst(vref, _, _, _) when vref.ValReprInfo.IsSome -> 

                    let _, argtysl, _, _ = GetTopValTypeInFSharpForm g vref.ValReprInfo.Value vref.Type expr.Range
                    argtysl |> List.mapi (fun i argtys -> 
                        argtys |> List.mapi (fun j (_, argInfo) -> 
                             match argInfo.Name with 
                             | None -> CompilerGeneratedName ("arg" ^ string i ^string j)
                             | Some id -> id.idText))
                | _ -> 
                    []

            assert (curriedActualArgTys.Length >= curriedNiceNames.Length)

            let argTysWithNiceNames, argTysWithoutNiceNames =
                List.chop curriedNiceNames.Length argTys

            /// Only consume 'suppliedArgs' up to at most the number of nice arguments
            let suppliedArgs, droppedSuppliedArgs = 
                List.chop (min suppliedArgs.Length curriedNiceNames.Length) suppliedArgs

            /// The relevant range for any expressions and applications includes the arguments 
            let appm = (m, suppliedArgs) ||> List.fold (fun m e -> unionRanges m (e.Range)) 

            // See if we have 'enough' suppliedArgs. If not, we have to build some lambdas, and, 
            // we have to 'let' bind all arguments that we consume, e.g.
            //   Seq.take (effect;4) : int list -> int list
            // is a classic case. Here we generate
            //   let tmp = (effect;4) in 
            //   (fun v -> Seq.take tmp (v :> seq<_>))
            let buildingLambdas = suppliedArgs.Length <> curriedNiceNames.Length

            /// Given a tuple of argument variables that has a tuple type that satisfies the input argument types, 
            /// coerce it to a tuple that satisfies the matching coerced argument type(s).
            let CoerceDetupled (argTys: TType list) (detupledArgs: Expr list) (actualTys: TType list) =
                assert (actualTys.Length = argTys.Length)
                assert (actualTys.Length = detupledArgs.Length)
                // Inject the coercions into the user-supplied explicit tuple
                let argm = List.reduce unionRanges (detupledArgs |> List.map (fun e -> e.Range))
                mkRefTupled g argm (List.map3 (mkCoerceIfNeeded g) actualTys argTys detupledArgs) actualTys

            /// Given an argument variable of tuple type that has been evaluated and stored in the 
            /// given variable, where the tuple type that satisfies the input argument types, 
            /// coerce it to a tuple that satisfies the matching coerced argument type(s).
            let CoerceBoundTuple tupleVar argTys (actualTys : TType list) =
                assert (actualTys.Length > 1)
            
                mkRefTupled g appm 
                   ((actualTys, argTys) ||> List.mapi2 (fun i actualTy dummyTy ->  
                       let argExprElement = mkTupleFieldGet g (tupInfoRef, tupleVar, argTys, i, appm)
                       mkCoerceIfNeeded  g actualTy dummyTy argExprElement))
                   actualTys

            /// Given an argument that has a tuple type that satisfies the input argument types, 
            /// coerce it to a tuple that satisfies the matching coerced argument type. Try to detuple the argument if possible.
            let CoerceTupled niceNames (argExpr: Expr) (actualTys: TType list) =
                let argExprTy = (tyOfExpr g argExpr)

                let argTys  = 
                    match actualTys with 
                    | [_] -> 
                        [tyOfExpr g argExpr]
                    | _ -> 
                        tryDestRefTupleTy g argExprTy 
                
                assert (actualTys.Length = argTys.Length)
                let nm = match niceNames with [nm] -> nm | _ -> "arg"
                if buildingLambdas then 
                    // Evaluate the user-supplied tuple-valued argument expression, inject the coercions and build an explicit tuple
                    // Assign the argument to make sure it is only run once
                    //     f ~~> : B -> int
                    //     f ~~> : (B * B) -> int
                    //
                    //  for 
                    //     let f a = 1
                    //     let f (a, a) = 1
                    let v, ve = mkCompGenLocal appm nm argExprTy
                    let binderBuilder = (fun tm -> mkCompGenLet appm v argExpr tm)
                    let expr = 
                        match actualTys, argTys with
                        | [actualTy], [argTy] -> mkCoerceIfNeeded  g actualTy argTy ve 
                        | _ -> CoerceBoundTuple ve argTys actualTys

                    binderBuilder, expr
                else                
                    if typeEquiv g (mkRefTupledTy g actualTys) argExprTy then 
                        (fun tm -> tm), argExpr
                    else
                    
                        let detupledArgs, argTys  = 
                            match actualTys with 
                            | [_actualType] -> 
                                [argExpr], [tyOfExpr g argExpr]
                            | _ -> 
                                tryDestRefTupleExpr argExpr, tryDestRefTupleTy g argExprTy 

                        // OK, the tuples match, or there is no de-tupling, 
                        //     f x
                        //     f (x, y)
                        //
                        //  for 
                        //     let f (x, y) = 1
                        // and we're not building lambdas, just coerce the arguments in place
                        if detupledArgs.Length =  actualTys.Length then 
                            (fun tm -> tm), CoerceDetupled argTys detupledArgs actualTys
                        else 
                            // In this case there is a tuple mismatch.
                            //     f p
                            //
                            //
                            //  for 
                            //     let f (x, y) = 1
                            // Assign the argument to make sure it is only run once
                            let v, ve = mkCompGenLocal appm nm argExprTy
                            let binderBuilder = (fun tm -> mkCompGenLet appm v argExpr tm)
                            let expr = CoerceBoundTuple ve argTys actualTys
                            binderBuilder, expr
                        

            // This variable is really a dummy to make the code below more regular. 
            // In the i = N - 1 cases we skip the introduction of the 'let' for
            // this variable.
            let resVar, resVarAsExpr = mkCompGenLocal appm "result" retTy
            let N = argTys.Length
            let (cloVar, exprForOtherArgs, _) = 
                List.foldBack 
                    (fun (i, inpArgTy, actualArgTys) (cloVar:Val, res, resTy) -> 

                        let inpArgTys = 
                            match actualArgTys with 
                            | [_] -> [inpArgTy]
                            | _ -> destRefTupleTy g inpArgTy

                        assert (inpArgTys.Length = actualArgTys.Length)
                        
                        let inpsAsVars, inpsAsExprs = inpArgTys |> List.mapi (fun j ty -> mkCompGenLocal appm ("arg"^string i^string j) ty)  |> List.unzip
                        let inpsAsActualArg = CoerceDetupled inpArgTys inpsAsExprs actualArgTys
                        let inpCloVarType = (mkFunTy (mkRefTupledTy g actualArgTys) cloVar.Type)
                        let newResTy = mkFunTy inpArgTy resTy
                        let inpCloVar, inpCloVarAsExpr = mkCompGenLocal appm ("clo"^string i) inpCloVarType
                        let newRes = 
                            // For the final arg we can skip introducing the dummy variable
                            if i = N - 1 then 
                                mkMultiLambda appm inpsAsVars 
                                    (mkApps g ((inpCloVarAsExpr, inpCloVarType), [], [inpsAsActualArg], appm), resTy)
                            else
                                mkMultiLambda appm inpsAsVars 
                                    (mkCompGenLet appm cloVar 
                                       (mkApps g ((inpCloVarAsExpr, inpCloVarType), [], [inpsAsActualArg], appm)) 
                                       res, 
                                     resTy)
                            
                        inpCloVar, newRes, newResTy)
                    argTysWithoutNiceNames
                    (resVar, resVarAsExpr, retTy)

            
            // Mark the up as Some/None
            let suppliedArgs = List.map Some suppliedArgs @ List.ofArray (Array.create (curriedNiceNames.Length - suppliedArgs.Length) None)

            assert (suppliedArgs.Length = curriedNiceNames.Length)

            let exprForAllArgs = 

                if isNil argTysWithNiceNames then 
                    mkCompGenLet appm cloVar exprWithActualTy exprForOtherArgs
                else
                    let lambdaBuilders, binderBuilders, inpsAsArgs = 
                    
                        (argTysWithNiceNames, curriedNiceNames, suppliedArgs) |||> List.map3 (fun (_, inpArgTy, actualArgTys) niceNames suppliedArg -> 

                                let inpArgTys = 
                                    match actualArgTys with 
                                    | [_] -> [inpArgTy]
                                    | _ -> destRefTupleTy g inpArgTy


                                /// Note: there might not be enough nice names, and they might not match in arity
                                let niceNames = 
                                    match niceNames with 
                                    | nms when nms.Length = inpArgTys.Length -> nms
                                    | [nm] -> inpArgTys |> List.mapi (fun i _ -> (nm^string i))
                                    | nms -> nms
                                match suppliedArg with 
                                | Some arg -> 
                                    let binderBuilder, inpsAsActualArg = CoerceTupled niceNames arg actualArgTys
                                    let lambdaBuilder = (fun tm -> tm)
                                    lambdaBuilder, binderBuilder, inpsAsActualArg
                                | None -> 
                                    let inpsAsVars, inpsAsExprs = (niceNames, inpArgTys)  ||> List.map2 (fun nm ty -> mkCompGenLocal appm nm ty)  |> List.unzip
                                    let inpsAsActualArg = CoerceDetupled inpArgTys inpsAsExprs actualArgTys
                                    let lambdaBuilder = (fun tm -> mkMultiLambda appm inpsAsVars (tm, tyOfExpr g tm))
                                    let binderBuilder = (fun tm -> tm)
                                    lambdaBuilder, binderBuilder, inpsAsActualArg)
                        |> List.unzip3
                    
                    // If no trailing args then we can skip introducing the dummy variable
                    // This corresponds to 
                    //    let f (x:A) = 1      
                    //
                    //   f ~~> type B -> int
                    //
                    // giving
                    //   (fun b -> f (b :> A))
                    // rather than 
                    //   (fun b -> let clo = f (b :> A) in clo)   
                    let exprApp = 
                        if isNil argTysWithoutNiceNames then 
                            mkApps g ((exprWithActualTy, actualTy), [], inpsAsArgs, appm)
                        else
                            mkCompGenLet appm 
                                    cloVar (mkApps g ((exprWithActualTy, actualTy), [], inpsAsArgs, appm)) 
                                    exprForOtherArgs

                    List.foldBack (fun f acc -> f acc) binderBuilders 
                        (List.foldBack (fun f acc -> f acc) lambdaBuilders exprApp)

            Some(exprForAllArgs, droppedSuppliedArgs)
    | _ -> 
        None
  
/// Find and make all subsumption eliminations 
let NormalizeAndAdjustPossibleSubsumptionExprs g inputExpr = 
    let expr, args = 
        // AdjustPossibleSubsumptionExpr can take into account an application
        match stripExpr inputExpr with 
        | Expr.App(f, _fty, [], args, _)  ->
             f, args

        | _ -> 
            inputExpr, []
    
    match AdjustPossibleSubsumptionExpr g expr args with 
    | None -> 
        inputExpr
    | Some (expr', []) -> 
        expr'
    | Some (expr', args') -> 
        //printfn "adjusted...." 
        Expr.App(expr', tyOfExpr g expr', [], args', inputExpr.Range)  
             
  
//---------------------------------------------------------------------------
// LinearizeTopMatch - when only one non-failing target, make linear.  The full
// complexity of this is only used for spectacularly rare bindings such as 
//    type ('a, 'b) either = This of 'a | That of 'b
//    let this_f1 = This (fun x -> x)
//    let This fA | That fA = this_f1
// 
// Here a polymorphic top level binding "fA" is _computed_ by a pattern match!!!
// The TAST coming out of type checking must, however, define fA as a type function, 
// since it is marked with an arity that indicates it's r.h.s. is a type function]
// without side effects and so can be compiled as a generic method (for example).

// polymorphic things bound in complex matches at top level require eta expansion of the 
// type function to ensure the r.h.s. of the binding is indeed a type function 
let etaExpandTypeLambda g m tps (tm, ty) = 
    if isNil tps then tm else mkTypeLambda m tps (mkApps g ((tm, ty), [(List.map mkTyparTy tps)], [], m), ty)

let AdjustValToTopVal (tmp:Val) parent valData =
    tmp.SetValReprInfo (Some valData)
    tmp.SetDeclaringEntity parent
    tmp.SetIsMemberOrModuleBinding()

/// For match with only one non-failing target T0, the other targets, T1... failing (say, raise exception).
///   tree, T0(v0, .., vN) => rhs ; T1() => fail ; ...
/// Convert it to bind T0's variables, then continue with T0's rhs:
///   let tmp = switch tree, TO(fv0, ..., fvN) => Tup (fv0, ..., fvN) ; T1() => fail; ...
///   let v1  = #1 tmp in ...
///   and vN  = #N tmp
///   rhs
/// Motivation:
/// - For top-level let bindings with possibly failing matches, 
///   this makes clear that subsequent bindings (if reached) are top-level ones.
let LinearizeTopMatchAux g parent  (spBind, m, tree, targets, m2, ty) =
    let targetsL = Array.toList targets
    (* items* package up 0, 1, more items *)
    let itemsProj tys i x = 
        match tys with 
        | []  -> failwith "itemsProj: no items?"
        | [_] -> x (* no projection needed *)
        | tys -> Expr.Op (TOp.TupleFieldGet(tupInfoRef, i), tys, [x], m)
    let isThrowingTarget = function TTarget(_, x, _) -> isThrow x
    if 1 + List.count isThrowingTarget targetsL = targetsL.Length then
        (* Have failing targets and ONE successful one, so linearize *)
        let (TTarget (vs, rhs, spTarget)) = Option.get (List.tryFind (isThrowingTarget >> not) targetsL)
        (* note - old code here used copy value to generate locals - this was not right *)
        let fvs      = vs |> List.map (fun v -> fst(mkLocal v.Range v.LogicalName v.Type)) (* fresh *)
        let vtys     = vs |> List.map (fun v -> v.Type) 
        let tmpTy    = mkRefTupledVarsTy g vs
        let tmp, tmpe = mkCompGenLocal m "matchResultHolder" tmpTy

        AdjustValToTopVal tmp parent ValReprInfo.emptyValData;  

        let newTg    = TTarget (fvs, mkRefTupledVars g m fvs, spTarget)
        let fixup (TTarget (tvs, tx, spTarget)) = 
           match destThrow tx with
           | Some (m, _, e) -> 
               let tx = mkThrow m tmpTy e
               TTarget(tvs, tx, spTarget) (* Throwing targets, recast it's "return type" *)
           | None -> newTg       (* Non-throwing target, replaced [new/old] *)
       
        let targets  = Array.map fixup targets
        let binds    = 
            vs |> List.mapi (fun i v -> 
                let ty = v.Type
                let rhs =  etaExpandTypeLambda g m  v.Typars (itemsProj vtys i tmpe, ty)
                // update the arity of the value 
                v.SetValReprInfo (Some (InferArityOfExpr g AllowTypeDirectedDetupling.Yes ty [] [] rhs))
                // This binding is deliberately non-sticky - any sequence point for 'rhs' goes on 'rhs' _after_ the binding has been evaluated
                mkInvisibleBind v rhs)  in (* vi = proj tmp *)
        mkCompGenLet m
          tmp (primMkMatch (spBind, m, tree, targets, m2, tmpTy)) (* note, probably retyped match, but note, result still has same type *)
          (mkLetsFromBindings m binds rhs)                             
    else
        (* no change *)
        primMkMatch (spBind, m, tree, targets, m2, ty)

let LinearizeTopMatch g parent = function
  | Expr.Match (spBind, m, tree, targets, m2, ty) -> LinearizeTopMatchAux g parent (spBind, m, tree, targets, m2, ty)
  | x -> x


//---------------------------------------------------------------------------
// XmlDoc signatures
//---------------------------------------------------------------------------


let commaEncs strs  = String.concat "," strs
let angleEnc  str   = "{" ^ str ^ "}" 
let ticksAndArgCountTextOfTyconRef (tcref:TyconRef) =
     // Generic type names are (name ^ "`" ^ digits) where name does not contain "`".
     let path = Array.toList (fullMangledPathToTyconRef tcref) @ [tcref.CompiledName]
     textOfPath path
     
let typarEnc _g (gtpsType, gtpsMethod) typar =
    match List.tryFindIndex (typarEq typar) gtpsType with
    | Some idx -> "`"  ^ string idx // single-tick-index for typar from type
    | None     ->
        match List.tryFindIndex (typarEq typar) gtpsMethod with
        | Some idx -> "``" ^ string idx // double-tick-index for typar from method
        | None     -> warning(InternalError("Typar not found during XmlDoc generation", typar.Range))
                      "``0" // REVIEW: this should be ERROR not WARNING?

let rec typeEnc g (gtpsType, gtpsMethod) ty = 
    if verbose then dprintf "--> typeEnc"
    let stripped = stripTyEqnsAndMeasureEqns g ty
    match stripped with 
    | TType_forall _ -> 
        "Microsoft.FSharp.Core.FSharpTypeFunc"
    | _ when isArrayTy g ty   -> 
        let tcref, tinst = destAppTy g ty
        let arraySuffix = 
            match rankOfArrayTyconRef g tcref with
            // The easy case
            | 1 -> "[]"
            // REVIEW
            // In fact IL supports 3 kinds of multidimensional arrays, and each kind of array has its own xmldoc spec.
            // We don't support all these, and instead always pull xmldocs for 0-based-arbitrary-length ("0:") multidimensional arrays.
            // This is probably the 99% case anyway.
            | 2 -> "[0:, 0:]"
            | 3 -> "[0:, 0:, 0:]"
            | 4 -> "[0:, 0:, 0:, 0:]"
            | _ -> failwith "impossible: rankOfArrayTyconRef: unsupported array rank"
        typeEnc g (gtpsType, gtpsMethod) (List.head tinst) ^ arraySuffix
    | TType_ucase (UCRef(tcref, _), tinst)   
    | TType_app (tcref, tinst)   -> 
        if tyconRefEq g g.byref_tcr tcref then
            typeEnc g (gtpsType, gtpsMethod) (List.head tinst) ^ "@"
        elif tyconRefEq g tcref g.nativeptr_tcr then
            typeEnc g (gtpsType, gtpsMethod) (List.head tinst) ^ "*"
        else
            let tyName = 
                let ty = stripTyEqnsAndMeasureEqns g ty
                match ty with
                | TType_app (tcref, _tinst)   -> 
                    // Generic type names are (name ^ "`" ^ digits) where name does not contain "`".
                    // In XML doc, when used in type instances, these do not use the ticks.
                    let path = Array.toList (fullMangledPathToTyconRef tcref) @ [tcref.CompiledName]
                    textOfPath (List.map DemangleGenericTypeName path)
                | _ -> assert(false); failwith "impossible"
            tyName + tyargsEnc g (gtpsType, gtpsMethod) tinst
    | TType_tuple (tupInfo, typs) -> 
        if evalTupInfoIsStruct tupInfo then 
            sprintf "System.ValueTuple%s"(tyargsEnc g (gtpsType, gtpsMethod) typs)
        else 
            sprintf "System.Tuple%s"(tyargsEnc g (gtpsType, gtpsMethod) typs)
    | TType_fun (f, x)           -> 
        "Microsoft.FSharp.Core.FSharpFunc" + tyargsEnc g (gtpsType, gtpsMethod) [f;x]
    | TType_var typar           -> 
        typarEnc g (gtpsType, gtpsMethod) typar
    | TType_measure _ -> "?"

and tyargsEnc g (gtpsType, gtpsMethod) args = 
     match args with     
     | [] -> ""
     | [a] when (match (stripTyEqns g a) with TType_measure _ -> true | _ -> false) -> ""  // float<m> should appear as just "float" in the generated .XML xmldoc file
     | _ -> angleEnc (commaEncs (List.map (typeEnc g (gtpsType, gtpsMethod)) args)) 

let XmlDocArgsEnc g (gtpsType, gtpsMethod) argTs =
  if isNil argTs then "" 
  else "(" + String.concat "," (List.map (typeEnc g (gtpsType, gtpsMethod)) argTs) + ")"

let buildAccessPath (cp : CompilationPath option) =
    match cp with
    | Some(cp) ->
        let ap = cp.AccessPath |> List.map fst |> List.toArray
        System.String.Join(".", ap)      
    | None -> "Extension Type"
let prependPath path name = if path = "" then name else path + "." + name

let XmlDocSigOfVal g path (v:Val) =
  let parentTypars, methTypars, argInfos, prefix, path, name = 

    // CLEANUP: this is one of several code paths that treat module values and members 
    // separately when really it would be cleaner to make sure GetTopValTypeInFSharpForm, GetMemberTypeInFSharpForm etc.
    // were lined up so code paths like this could be uniform
    
    match v.MemberInfo with 
    | Some membInfo when not v.IsExtensionMember -> 
        (* Methods, Properties etc. *)
        let tps, argInfos, _, _ = GetMemberTypeInMemberForm g membInfo.MemberFlags (Option.get v.ValReprInfo) v.Type v.Range
        let prefix, name = 
          match membInfo.MemberFlags.MemberKind with 
          | MemberKind.ClassConstructor 
          | MemberKind.Constructor -> "M:", "#ctor"
          | MemberKind.Member -> "M:", v.CompiledName
          | MemberKind.PropertyGetSet 
          | MemberKind.PropertySet
          | MemberKind.PropertyGet -> "P:", v.PropertyName
        let path = if v.HasDeclaringEntity then prependPath path v.TopValDeclaringEntity.CompiledName else path
        let parentTypars, methTypars = 
          match PartitionValTypars g v with
          | Some(_, memberParentTypars, memberMethodTypars, _, _) -> memberParentTypars, memberMethodTypars
          | None -> [], tps
        parentTypars, methTypars, argInfos, prefix, path, name
    | _ ->
        // Regular F# values and extension members 
        let w = arityOfVal v
        let tps, argInfos, _, _ = GetTopValTypeInCompiledForm g w v.Type v.Range
        let name = v.CompiledName
        let prefix =
          if  w.NumCurriedArgs = 0 && isNil tps then "P:"
          else "M:"
        [], tps, argInfos, prefix, path, name
  let argTs = argInfos |> List.concat |> List.map fst
  let args = XmlDocArgsEnc g (parentTypars, methTypars) argTs
  let arity = List.length methTypars in (* C# XML doc adds ``<arity> to *generic* member names *)
  let genArity = if arity=0 then "" else sprintf "``%d" arity
  prefix + prependPath path name + genArity + args
  
let BuildXmlDocSig prefix paths =  prefix + List.fold prependPath "" paths

let XmlDocSigOfUnionCase = BuildXmlDocSig "T:" // Would like to use "U:", but ParseMemberSignature only accepts C# signatures

let XmlDocSigOfField     = BuildXmlDocSig "F:"

let XmlDocSigOfProperty  = BuildXmlDocSig "P:"

let XmlDocSigOfTycon     = BuildXmlDocSig "T:"

let XmlDocSigOfSubModul  = BuildXmlDocSig "T:"

let XmlDocSigOfEntity (eref:EntityRef) =
    XmlDocSigOfTycon [(buildAccessPath eref.CompilationPathOpt); eref.Deref.CompiledName]

//--------------------------------------------------------------------------
// Some unions have null as representations 
//--------------------------------------------------------------------------


let enum_CompilationRepresentationAttribute_Static             = 0b0000000000000001
let enum_CompilationRepresentationAttribute_Instance           = 0b0000000000000010
let enum_CompilationRepresentationAttribute_StaticInstanceMask = 0b0000000000000011
let enum_CompilationRepresentationAttribute_ModuleSuffix       = 0b0000000000000100
let enum_CompilationRepresentationAttribute_PermitNull         = 0b0000000000001000

let HasUseNullAsTrueValueAttribute g attribs =
     match TryFindFSharpInt32Attribute  g g.attrib_CompilationRepresentationAttribute attribs with
     | Some(flags) -> ((flags &&& enum_CompilationRepresentationAttribute_PermitNull) <> 0)
     | _ -> false 

let TyconHasUseNullAsTrueValueAttribute g (tycon:Tycon) = HasUseNullAsTrueValueAttribute g tycon.Attribs 

// WARNING: this must match optimizeAlternativeToNull in ilx/cu_erase.fs
let CanHaveUseNullAsTrueValueAttribute (_g:TcGlobals) (tycon:Tycon) =
  (tycon.IsUnionTycon && 
   let ucs = tycon.UnionCasesArray
   (ucs.Length = 0 ||
     (ucs |> Array.existsOne (fun uc -> uc.IsNullary) &&
      ucs |> Array.exists (fun uc -> not uc.IsNullary))))

// WARNING: this must match optimizeAlternativeToNull in ilx/cu_erase.fs
let IsUnionTypeWithNullAsTrueValue (g:TcGlobals) (tycon:Tycon) =
  (tycon.IsUnionTycon && 
   let ucs = tycon.UnionCasesArray
   (ucs.Length = 0 ||
     (TyconHasUseNullAsTrueValueAttribute g tycon &&
      ucs |> Array.existsOne (fun uc -> uc.IsNullary) &&
      ucs |> Array.exists (fun uc -> not uc.IsNullary))))

let TyconCompilesInstanceMembersAsStatic g tycon = IsUnionTypeWithNullAsTrueValue g tycon
let TcrefCompilesInstanceMembersAsStatic g (tcref: TyconRef) = TyconCompilesInstanceMembersAsStatic g tcref.Deref

let TypeNullNever g ty = 
    let underlyingTy = stripTyEqnsAndMeasureEqns g ty
    (isStructTy g underlyingTy) ||
    (isByrefTy g underlyingTy)



/// Indicates if the type admits the use of 'null' as a value
let TypeNullIsExtraValue g m ty = 
    if isILReferenceTy g ty || isDelegateTy g ty then
        // Putting AllowNullLiteralAttribute(false) on an IL or provided type means 'null' can't be used with that type
        not (isAppTy g ty && TryFindTyconRefBoolAttribute g m g.attrib_AllowNullLiteralAttribute (tcrefOfAppTy g ty) = Some(false))
    elif TypeNullNever g ty then 
        false
    else 
        // Putting AllowNullLiteralAttribute(true) on an F# type means 'null' can be used with that type
        isAppTy g ty && TryFindTyconRefBoolAttribute g m g.attrib_AllowNullLiteralAttribute (tcrefOfAppTy g ty) = Some(true)

let TypeNullIsTrueValue g ty =
    (match tryDestAppTy g ty with
     | Some tcref -> IsUnionTypeWithNullAsTrueValue g tcref.Deref
     | _ -> false) || (isUnitTy g ty)

let TypeNullNotLiked g m ty = 
       not (TypeNullIsExtraValue g m ty) 
    && not (TypeNullIsTrueValue g ty) 
    && not (TypeNullNever g ty) 

let TypeSatisfiesNullConstraint g m ty = 
    TypeNullIsExtraValue g m ty  

let rec TypeHasDefaultValue g m ty = 
    let ty = stripTyEqnsAndMeasureEqns g ty
    TypeSatisfiesNullConstraint g m ty  
    || (isStructTy g ty &&
        // Is it an F# struct type?
        (if isFSharpStructTy g ty then 
            let tcref, tinst = destAppTy g ty 
            let flds = 
                // Note this includes fields implied by the use of the implicit class construction syntax
                tcref.AllInstanceFieldsAsList
                  // We can ignore fields with the DefaultValue(false) attribute 
                  |> List.filter (fun fld -> not (TryFindFSharpBoolAttribute g g.attrib_DefaultValueAttribute fld.FieldAttribs = Some(false)))

            flds |> List.forall (actualTyOfRecdField (mkTyconRefInst tcref tinst) >> TypeHasDefaultValue g m)
         elif isStructTupleTy g ty then 
            destStructTupleTy g ty |> List.forall (TypeHasDefaultValue g m)
         else
            // All struct types defined in other .NET languages have a DefaultValue regardless of their
            // instantiation
            true))


/// Determines types that are potentially known to satisfy the 'comparable' constraint and returns
/// a set of residual types that must also satisfy the constraint
let (|SpecialComparableHeadType|_|) g ty =           
    if isAnyTupleTy g ty then 
        let _tupInfo, elemTys = destAnyTupleTy g ty
        Some elemTys 
    else
        match ty with
        | AppTy g (tcref, tinst) ->
            if isArrayTyconRef g tcref ||
               tyconRefEq g tcref g.system_UIntPtr_tcref ||
               tyconRefEq g tcref g.system_IntPtr_tcref then
                 Some tinst 
            else 
                None
        | _ ->
            None

let (|SpecialEquatableHeadType|_|) g ty = (|SpecialComparableHeadType|_|) g ty
let (|SpecialNotEquatableHeadType|_|) g ty = 
    if isFunTy g ty then Some() else None



// Can we use the fast helper for the 'LanguagePrimitives.IntrinsicFunctions.TypeTestGeneric'? 
let canUseTypeTestFast g ty = 
     not (isTyparTy g ty) && 
     not (TypeNullIsTrueValue g ty) && 
     not (TypeNullNever g ty)

// Can we use the fast helper for the 'LanguagePrimitives.IntrinsicFunctions.UnboxGeneric'? 
let canUseUnboxFast g m ty = 
     not (isTyparTy g ty) && 
     not (TypeNullNotLiked g m ty)
     
     
//--------------------------------------------------------------------------
// Nullness tests and pokes 
//--------------------------------------------------------------------------

(* match inp with :? ty as v -> e2[v] | _ -> e3 *)
let mkIsInstConditional g m tgty vinpe v e2 e3 = 
    // No sequence point for this compiler generated expression form
    
    if canUseTypeTestFast g tgty then 

        let mbuilder = new MatchBuilder(NoSequencePointAtInvisibleBinding, m)
        let tg2 = mbuilder.AddResultTarget(e2, SuppressSequencePointAtTarget)
        let tg3 = mbuilder.AddResultTarget(e3, SuppressSequencePointAtTarget)
        let dtree = TDSwitch(exprForVal m v, [TCase(DecisionTreeTest.IsNull, tg3)], Some tg2, m)
        let expr = mbuilder.Close(dtree, m, tyOfExpr g e2)
        mkCompGenLet m v (mkIsInst tgty vinpe m)  expr

    else
        let mbuilder = new MatchBuilder(NoSequencePointAtInvisibleBinding, m)
        let tg2 = TDSuccess([mkCallUnbox g m tgty vinpe], mbuilder.AddTarget(TTarget([v], e2, SuppressSequencePointAtTarget)))
        let tg3 = mbuilder.AddResultTarget(e3, SuppressSequencePointAtTarget)
        let dtree = TDSwitch(vinpe, [TCase(DecisionTreeTest.IsInst(tyOfExpr g vinpe, tgty), tg2)], Some tg3, m)
        let expr = mbuilder.Close(dtree, m, tyOfExpr g e2)
        expr



// Null tests are generated by
//    1. The compilation of array patterns in the pattern match compiler
//    2. The compilation of string patterns in the pattern match compiler
let mkNullTest g m e1 e2 e3 =
        let mbuilder = new MatchBuilder(NoSequencePointAtInvisibleBinding, m)
        let tg2 = mbuilder.AddResultTarget(e2, SuppressSequencePointAtTarget)
        let tg3 = mbuilder.AddResultTarget(e3, SuppressSequencePointAtTarget)            
        let dtree = TDSwitch(e1, [TCase(DecisionTreeTest.IsNull, tg3)], Some tg2, m)
        let expr = mbuilder.Close(dtree, m, tyOfExpr g e2)
        expr         
let mkNonNullTest (g:TcGlobals) m e = mkAsmExpr ([ IL.AI_ldnull ; IL.AI_cgt_un  ], [], [e], [g.bool_ty], m)
let mkNonNullCond g m ty e1 e2 e3 = mkCond NoSequencePointAtStickyBinding SuppressSequencePointAtTarget m ty (mkNonNullTest g m e1) e2 e3
let mkIfThen (g:TcGlobals) m e1 e2 = mkCond NoSequencePointAtStickyBinding SuppressSequencePointAtTarget m g.unit_ty e1 e2 (mkUnit g m)


let ModuleNameIsMangled g attrs =
    match TryFindFSharpInt32Attribute g g.attrib_CompilationRepresentationAttribute attrs with
    | Some(flags) -> ((flags &&& enum_CompilationRepresentationAttribute_ModuleSuffix) <> 0)
    | _ -> false 

let CompileAsEvent g attrs = HasFSharpAttribute g g.attrib_CLIEventAttribute attrs 


let MemberIsCompiledAsInstance g parent isExtensionMember (membInfo:ValMemberInfo) attrs =
    // All extension members are compiled as static members
    if isExtensionMember then false
    // Anything implementing a dispatch slot is compiled as an instance member
    elif membInfo.MemberFlags.IsOverrideOrExplicitImpl then true
    elif not (isNil membInfo.ImplementedSlotSigs) then true
    else 
        // Otherwise check attributes to see if there is an explicit instance or explicit static flag
        let explicitInstance, explicitStatic = 
            match TryFindFSharpInt32Attribute g g.attrib_CompilationRepresentationAttribute attrs with
            | Some(flags) -> 
              ((flags &&& enum_CompilationRepresentationAttribute_Instance) <> 0), 
              ((flags &&& enum_CompilationRepresentationAttribute_Static) <> 0)
            | _ -> false, false
        explicitInstance ||
        (membInfo.MemberFlags.IsInstance &&
         not explicitStatic &&
         not (TcrefCompilesInstanceMembersAsStatic g parent))


let isSealedTy g ty =
    let ty = stripTyEqnsAndMeasureEqns g ty
    not (isRefTy g ty) ||
    isUnitTy g ty || 
    isArrayTy g ty || 

    match metadataOfTy g ty with 
#if !NO_EXTENSIONTYPING
    | ProvidedTypeMetadata st -> st.IsSealed
#endif
    | ILTypeMetadata (TILObjectReprData(_, _, td)) -> td.IsSealed
    | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata -> 

       if (isFSharpInterfaceTy g ty || isFSharpClassTy g ty) then 
          let tcref, _ = destAppTy g ty
          (TryFindFSharpBoolAttribute g g.attrib_SealedAttribute tcref.Attribs = Some(true))
       else 
          // All other F# types, array, byref, tuple types are sealed
          true
   
let isComInteropTy g ty =
    let tcr, _ = destAppTy g ty
    match g.attrib_ComImportAttribute with
    | None -> false
    | Some attr -> TryFindFSharpBoolAttribute g attr tcr.Attribs = Some(true)
  
let ValSpecIsCompiledAsInstance g (v:Val) =
    match v.MemberInfo with 
    | Some(membInfo) -> 
        // Note it doesn't matter if we pass 'v.TopValDeclaringEntity' or 'v.MemberApparentEntity' here. 
        // These only differ if the value is an extension member, and in that case MemberIsCompiledAsInstance always returns 
        // false anyway 
        MemberIsCompiledAsInstance g v.MemberApparentEntity v.IsExtensionMember membInfo v.Attribs  
    |  _ -> false

let ValRefIsCompiledAsInstanceMember g (vref: ValRef) = ValSpecIsCompiledAsInstance g vref.Deref


//---------------------------------------------------------------------------
// Crack information about an F# object model call
//---------------------------------------------------------------------------

let GetMemberCallInfo g (vref:ValRef, vFlags) = 
    match vref.MemberInfo with 
    | Some(membInfo) when not vref.IsExtensionMember -> 
      let numEnclTypeArgs = vref.MemberApparentEntity.TyparsNoRange.Length
      let virtualCall = 
          (membInfo.MemberFlags.IsOverrideOrExplicitImpl || 
           membInfo.MemberFlags.IsDispatchSlot) && 
          not membInfo.MemberFlags.IsFinal && 
          (match vFlags with VSlotDirectCall -> false | _ -> true)
      let isNewObj    = (membInfo.MemberFlags.MemberKind = MemberKind.Constructor) && (match vFlags with NormalValUse -> true | _ -> false)
      let isSuperInit = (membInfo.MemberFlags.MemberKind = MemberKind.Constructor) && (match vFlags with CtorValUsedAsSuperInit -> true | _ -> false) 
      let isSelfInit  = (membInfo.MemberFlags.MemberKind = MemberKind.Constructor) && (match vFlags with CtorValUsedAsSelfInit -> true | _ -> false) 
      let isCompiledAsInstance = ValRefIsCompiledAsInstanceMember g vref
      let takesInstanceArg = isCompiledAsInstance && not isNewObj
      let isPropGet = (membInfo.MemberFlags.MemberKind = MemberKind.PropertyGet) && (membInfo.MemberFlags.IsInstance = isCompiledAsInstance)
      let isPropSet = (membInfo.MemberFlags.MemberKind = MemberKind.PropertySet) && (membInfo.MemberFlags.IsInstance = isCompiledAsInstance)
      numEnclTypeArgs, virtualCall, isNewObj, isSuperInit, isSelfInit , takesInstanceArg, isPropGet, isPropSet
    | _ -> 
      0, false, false, false, false, false, false, false

//---------------------------------------------------------------------------
// Active pattern name helpers
//---------------------------------------------------------------------------


let TryGetActivePatternInfo (vref:ValRef) =  
    // First is an optimization to prevent calls to CoreDisplayName, which calls DemangleOperatorName
    let logicalName = vref.LogicalName
    if logicalName.Length = 0 || logicalName.[0] <> '|' then 
       None 
    else 
       ActivePatternInfoOfValName vref.CoreDisplayName vref.Range

type ActivePatternElemRef with 
    member x.Name = 
        let (APElemRef(_, vref, n)) = x
        match TryGetActivePatternInfo vref with
        | None -> error(InternalError("not an active pattern name", vref.Range))
        | Some apinfo -> 
            let nms = apinfo.ActiveTags
            if n < 0 || n >= List.length nms  then error(InternalError("name_of_apref: index out of range for active pattern reference", vref.Range));
            List.item n nms

let mkChoiceTyconRef (g:TcGlobals) m n = 
     match n with 
     | 0 | 1 -> error(InternalError("mkChoiceTyconRef", m))
     | 2 -> g.choice2_tcr
     | 3 -> g.choice3_tcr
     | 4 -> g.choice4_tcr
     | 5 -> g.choice5_tcr
     | 6 -> g.choice6_tcr
     | 7 -> g.choice7_tcr
     | _ -> error(Error(FSComp.SR.tastActivePatternsLimitedToSeven(), m))

let mkChoiceTy (g:TcGlobals) m tinst = 
     match List.length tinst with 
     | 0 -> g.unit_ty
     | 1 -> List.head tinst
     | length -> mkAppTy (mkChoiceTyconRef g m length) tinst

let mkChoiceCaseRef g m n i = 
     mkUnionCaseRef (mkChoiceTyconRef g m n) ("Choice"+string (i+1)+"Of"+string n)

type PrettyNaming.ActivePatternInfo with 
    member x.Names = x.ActiveTags

    member apinfo.ResultType g m rtys = 
        let choicety = mkChoiceTy g m rtys
        if apinfo.IsTotal then choicety else mkOptionTy g choicety
    
    member apinfo.OverallType g m dty rtys = 
        mkFunTy dty (apinfo.ResultType g m rtys)

//---------------------------------------------------------------------------
// Active pattern validation
//---------------------------------------------------------------------------
    
// check if an active pattern takes type parameters only bound by the return types, 
// not by their argument types.
let doesActivePatternHaveFreeTypars g (v:ValRef) =
    let vty  = v.TauType
    let vtps = v.Typars |> Zset.ofList typarOrder
    if not (isFunTy g v.TauType) then
        errorR(Error(FSComp.SR.activePatternIdentIsNotFunctionTyped(v.LogicalName), v.Range))
    let argtys, resty  = stripFunTy g vty
    let argtps, restps= (freeInTypes CollectTypars argtys).FreeTypars, (freeInType CollectTypars resty).FreeTypars        
    // Error if an active pattern is generic in type variables that only occur in the result Choice<_, ...>.
    // Note: The test restricts to v.Typars since typars from the closure are considered fixed.
    not (Zset.isEmpty (Zset.inter (Zset.diff restps argtps) vtps)) 

//---------------------------------------------------------------------------
// RewriteExpr: rewrite bottom up with interceptors 
//---------------------------------------------------------------------------

[<NoEquality; NoComparison>]
type ExprRewritingEnv = 
    { PreIntercept: ((Expr -> Expr) -> Expr -> Expr option) option;
      PostTransform: Expr -> Expr option;
      PreInterceptBinding: ((Expr -> Expr) -> Binding -> Binding option) option;
      IsUnderQuotations: bool }    

let rec rewriteBind env bind = 
     match env.PreInterceptBinding  with 
     | Some f -> 
         match f (RewriteExpr env) bind with 
         | Some res -> res
         | None -> rewriteBindStructure env bind
     | None -> rewriteBindStructure env bind
     
and rewriteBindStructure env (TBind(v, e, letSeqPtOpt)) = 
     TBind(v, RewriteExpr env e, letSeqPtOpt) 

and rewriteBinds env binds = List.map (rewriteBind env) binds

and RewriteExpr env expr =
  match expr with 
  | Expr.Let _ 
  | Expr.Sequential _ ->
      rewriteLinearExpr env expr (fun e -> e)
  | _ -> 
      let expr = 
         match preRewriteExpr env expr with 
         | Some expr -> expr
         | None -> rewriteExprStructure env expr
      postRewriteExpr env expr 

and preRewriteExpr env expr = 
     match env.PreIntercept  with 
     | Some f -> f (RewriteExpr env) expr
     | None -> None 

and postRewriteExpr env expr = 
     match env.PostTransform expr with 
     | None -> expr 
     | Some expr -> expr 

and rewriteExprStructure env expr =  
  match expr with
  | Expr.Const _ 
  | Expr.Val _ -> expr

  | Expr.App(f0, f0ty, tyargs, args, m) -> 
      let f0'   = RewriteExpr env f0
      let args' = rewriteExprs env args
      if f0 === f0' && args === args' then expr
      else Expr.App(f0', f0ty, tyargs, args', m)

  | Expr.Quote(ast, {contents=Some(typeDefs, argTypes, argExprs, data)}, isFromQueryExpression, m, ty) -> 
      Expr.Quote((if env.IsUnderQuotations then RewriteExpr env ast else ast), {contents=Some(typeDefs, argTypes, rewriteExprs env argExprs, data)}, isFromQueryExpression, m, ty)

  | Expr.Quote(ast, {contents=None}, isFromQueryExpression, m, ty) -> 
      Expr.Quote((if env.IsUnderQuotations then RewriteExpr env ast else ast), {contents=None}, isFromQueryExpression, m, ty)

  | Expr.Obj (_, ty, basev, basecall, overrides, iimpls, m) -> 
      mkObjExpr(ty, basev, RewriteExpr env basecall, List.map (rewriteObjExprOverride env) overrides, 
                  List.map (rewriteObjExprInterfaceImpl env) iimpls, m)
  | Expr.Link eref -> 
      RewriteExpr env !eref

  | Expr.Op (c, tyargs, args, m) -> 
      let args' = rewriteExprs env args
      if args === args' then expr 
      else Expr.Op (c, tyargs, args', m)

  | Expr.Lambda(_lambdaId, ctorThisValOpt, baseValOpt, argvs, body, m, rty) -> 
      let body = RewriteExpr env body
      rebuildLambda m ctorThisValOpt baseValOpt argvs (body, rty)

  | Expr.TyLambda(_lambdaId, argtyvs, body, m, rty) -> 
      let body = RewriteExpr env body
      mkTypeLambda m argtyvs (body, rty)

  | Expr.Match(spBind, exprm, dtree, targets, m, ty) -> 
      let dtree' = rewriteDecisionTree env dtree
      let targets' = rewriteTargets env targets
      mkAndSimplifyMatch spBind exprm m ty dtree' targets'

  | Expr.LetRec (binds, e, m, _) ->
      let binds = rewriteBinds env binds
      let e' = RewriteExpr env e
      Expr.LetRec(binds, e', m, NewFreeVarsCache())

  | Expr.Let _ -> failwith "unreachable - linear let"

  | Expr.Sequential _ -> failwith "unreachable - linear seq"

  | Expr.StaticOptimization (constraints, e2, e3, m) ->
      let e2' = RewriteExpr env e2
      let e3' = RewriteExpr env e3
      Expr.StaticOptimization(constraints, e2', e3', m)

  | Expr.TyChoose (a, b, m) -> 
      Expr.TyChoose(a, RewriteExpr env b, m)

and rewriteLinearExpr env expr contf =
    // schedule a rewrite on the way back up by adding to the continuation 
    let contf = contf << postRewriteExpr env
    match preRewriteExpr env expr with 
    | Some expr -> contf expr  (* done - intercepted! *)
    | None -> 
        match expr with 
        | Expr.Let (bind, body, m, _) ->  
            let bind = rewriteBind env bind
            rewriteLinearExpr env body (contf << (fun body' ->
                mkLetBind m bind body'))
        | Expr.Sequential  (e1, e2, dir, spSeq, m) ->
            let e1' = RewriteExpr env e1
            rewriteLinearExpr env e2 (contf << (fun e2' ->
                if e1 === e1' && e2 === e2' then expr 
                else Expr.Sequential(e1', e2', dir, spSeq, m)))
        | LinearMatchExpr (spBind, exprm, dtree, tg1, e2, sp2, m2, ty) ->
            let dtree = rewriteDecisionTree env dtree
            let tg1 = rewriteTarget env tg1
            // tailcall
            rewriteLinearExpr env e2 (contf << (fun e2 ->
                rebuildLinearMatchExpr (spBind, exprm, dtree, tg1, e2, sp2, m2, ty)))
        | _ -> 
            (* no longer linear *)
            contf (RewriteExpr env expr) 

and rewriteExprs env exprs = List.mapq (RewriteExpr env) exprs
and rewriteFlatExprs env exprs = List.mapq (RewriteExpr env) exprs

and rewriteDecisionTree env x =
  match x with 
  | TDSuccess (es, n) -> 
      let es' = rewriteFlatExprs env es
      if LanguagePrimitives.PhysicalEquality es es' then x 
      else TDSuccess(es', n)

  | TDSwitch (e, cases, dflt, m) ->
      let e' = RewriteExpr env e
      let cases' = List.map (fun (TCase(discrim, e)) -> TCase(discrim, rewriteDecisionTree env e)) cases
      let dflt' = Option.map (rewriteDecisionTree env) dflt
      TDSwitch (e', cases', dflt', m)

  | TDBind (bind, body) ->
      let bind' = rewriteBind env bind
      let body = rewriteDecisionTree env body
      TDBind (bind', body)

and rewriteTarget env (TTarget(vs, e, spTarget)) = TTarget(vs, RewriteExpr env e, spTarget)

and rewriteTargets env targets = List.map (rewriteTarget env) (Array.toList targets)

and rewriteObjExprOverride env (TObjExprMethod(slotsig, attribs, tps, vs, e, m)) =
  TObjExprMethod(slotsig, attribs, tps, vs, RewriteExpr env e, m)

and rewriteObjExprInterfaceImpl env (ty, overrides) = 
  (ty, List.map (rewriteObjExprOverride env) overrides)
    
and rewriteModuleOrNamespaceExpr env x = 
    match x with  
    (* | ModuleOrNamespaceExprWithSig(mty, e, m) -> ModuleOrNamespaceExprWithSig(mty, rewriteModuleOrNamespaceExpr env e, m) *)
    | ModuleOrNamespaceExprWithSig(mty, def, m) ->  ModuleOrNamespaceExprWithSig(mty, rewriteModuleOrNamespaceDef env def, m)

and rewriteModuleOrNamespaceDefs env x = List.map (rewriteModuleOrNamespaceDef env) x
    
and rewriteModuleOrNamespaceDef env x = 
    match x with 
    | TMDefRec(isRec, tycons, mbinds, m) -> TMDefRec(isRec, tycons, rewriteModuleOrNamespaceBindings env mbinds, m)
    | TMDefLet(bind, m)         -> TMDefLet(rewriteBind env bind, m)
    | TMDefDo(e, m)             -> TMDefDo(RewriteExpr env e, m)
    | TMDefs defs             -> TMDefs(rewriteModuleOrNamespaceDefs env defs)
    | TMAbstract mexpr        -> TMAbstract(rewriteModuleOrNamespaceExpr env mexpr)

and rewriteModuleOrNamespaceBinding env x = 
   match x with 
   | ModuleOrNamespaceBinding.Binding bind -> ModuleOrNamespaceBinding.Binding (rewriteBind env bind)
   | ModuleOrNamespaceBinding.Module(nm, rhs) -> ModuleOrNamespaceBinding.Module(nm, rewriteModuleOrNamespaceDef env rhs)

and rewriteModuleOrNamespaceBindings env mbinds = List.map (rewriteModuleOrNamespaceBinding env) mbinds

and RewriteImplFile env mv = mapTImplFile (rewriteModuleOrNamespaceExpr env) mv



//--------------------------------------------------------------------------
// Build a Remap that converts all "local" references to "public" things 
// accessed via non local references.
//--------------------------------------------------------------------------

let MakeExportRemapping viewedCcu (mspec:ModuleOrNamespace) = 

    let accEntityRemap (entity:Entity) acc = 
        match tryRescopeEntity viewedCcu entity with 
        | Some eref -> 
            addTyconRefRemap (mkLocalTyconRef entity) eref acc
        | None -> 
            if entity.IsNamespace then 
                acc
            else
                error(InternalError("Unexpected entity without a pubpath when remapping assembly data", entity.Range))

    let accValRemap (vspec:Val) acc = 
        // The acc contains the entity remappings
        match tryRescopeVal viewedCcu acc vspec with 
        | Some vref -> 
            {acc with valRemap=acc.valRemap.Add vspec vref }
        | None -> 
            error(InternalError("Unexpected value without a pubpath when remapping assembly data", vspec.Range))

    let mty = mspec.ModuleOrNamespaceType
    let entities = allEntitiesOfModuleOrNamespaceTy mty
    let vs = allValsOfModuleOrNamespaceTy mty
    // Remap the entities first so we can correctly remap the types in the signatures of the ValLinkageFullKey's in the value references
    let acc = List.foldBack accEntityRemap entities Remap.Empty
    let allRemap = List.foldBack accValRemap vs acc
    allRemap

//--------------------------------------------------------------------------
// Apply a "local to nonlocal" renaming to a module type.  This can't use
// remap_mspec since the remapping we want isn't to newly created nodes
// but rather to remap to the nonlocal references. This is deliberately 
// "breaking" the binding structure implicit in the module type, which is
// the whole point - one things are rewritten to use non local references then
// the elements can be copied at will, e.g. when inlining during optimization.
//------------------------------------------------------------------------ 


let rec remapEntityDataToNonLocal g tmenv (d: Entity) = 
    let tps', tmenvinner = tmenvCopyRemapAndBindTypars (remapAttribs g tmenv) tmenv (d.entity_typars.Force(d.entity_range))
    let typarsR          = LazyWithContext.NotLazy tps'
    let attribsR         = d.entity_attribs        |> remapAttribs g tmenvinner
    let tyconReprR       = d.entity_tycon_repr     |> remapTyconRepr g tmenvinner
    let tyconAbbrevR     = d.TypeAbbrev            |> Option.map (remapType tmenvinner)
    let tyconTcaugR      = d.entity_tycon_tcaug    |> remapTyconAug tmenvinner
    let modulContentsR   = 
        MaybeLazy.Strict (d.entity_modul_contents.Value
                          |> mapImmediateValsAndTycons (remapTyconToNonLocal g tmenv) (remapValToNonLocal g tmenv))
    let exnInfoR         = d.ExceptionInfo         |> remapTyconExnInfo g tmenvinner
    { d with 
          entity_typars         = typarsR
          entity_attribs        = attribsR
          entity_tycon_repr     = tyconReprR
          entity_tycon_tcaug    = tyconTcaugR
          entity_modul_contents = modulContentsR
          entity_opt_data       =
            match d.entity_opt_data with
            | Some dd ->
                Some { dd with  entity_tycon_abbrev = tyconAbbrevR; entity_exn_info = exnInfoR }
            | _ -> None }

and remapTyconToNonLocal g tmenv x = 
    x |> NewModifiedTycon (remapEntityDataToNonLocal g tmenv)  

and remapValToNonLocal g  tmenv inp = 
    // creates a new stamp
    inp |> NewModifiedVal (remapValData g tmenv)

let ApplyExportRemappingToEntity g tmenv x = remapTyconToNonLocal g tmenv x

(* Which constraints actually get compiled to .NET constraints? *)
let isCompiledConstraint cx = 
    match cx with 
      | TyparConstraint.SupportsNull _ // this implies the 'class' constraint
      | TyparConstraint.IsReferenceType _  // this is the 'class' constraint
      | TyparConstraint.IsNonNullableStruct _ 
      | TyparConstraint.IsReferenceType _
      | TyparConstraint.RequiresDefaultConstructor _
      | TyparConstraint.CoercesTo _ -> true
      | _ -> false
    
// Is a value a first-class polymorphic value with .NET constraints? 
// Used to turn off TLR and method splitting
let IsGenericValWithGenericContraints g (v:Val) = 
    isForallTy g v.Type && 
    v.Type |> destForallTy g |> fst |> List.exists (fun tp -> List.exists isCompiledConstraint tp.Constraints)

// Does a type support a given interface? 
type Entity with 
    member tycon.HasInterface g ty = 
        tycon.TypeContents.tcaug_interfaces |> List.exists (fun (x, _, _) -> typeEquiv g ty x)  

    // Does a type have an override matching the given name and argument types? 
    // Used to detect the presence of 'Equals' and 'GetHashCode' in type checking 
    member tycon.HasOverride g nm argtys = 
        tycon.TypeContents.tcaug_adhoc 
        |> NameMultiMap.find nm
        |> List.exists (fun vref -> 
                          match vref.MemberInfo with 
                          | None -> false 
                          | Some membInfo -> 
                                         let argInfos = ArgInfosOfMember g vref 
                                         argInfos.Length = 1 && 
                                         List.lengthsEqAndForall2 (typeEquiv g) (List.map fst (List.head argInfos)) argtys  &&  
                                         membInfo.MemberFlags.IsOverrideOrExplicitImpl) 
    
    member tycon.HasMember g nm argtys = 
        tycon.TypeContents.tcaug_adhoc 
        |> NameMultiMap.find nm
        |> List.exists (fun vref -> 
                          match vref.MemberInfo with 
                          | None -> false 
                          | _ -> let argInfos = ArgInfosOfMember g vref 
                                 argInfos.Length = 1 && 
                                 List.lengthsEqAndForall2 (typeEquiv g) (List.map fst (List.head argInfos)) argtys) 


type EntityRef with 
    member tcref.HasInterface g ty = tcref.Deref.HasInterface g ty
    member tcref.HasOverride g nm argtys = tcref.Deref.HasOverride g nm argtys
    member tcref.HasMember g nm argtys = tcref.Deref.HasMember g nm argtys

let mkFastForLoop g (spLet, m, idv:Val, start, dir, finish, body) =
    let dir = if dir then FSharpForLoopUp else FSharpForLoopDown 
    mkFor g (spLet, idv, start, dir, finish, body, m)


/// Accessing a binding of the form "let x = 1" or "let x = e" for any "e" satisfying the predicate
/// below does not cause an initialization trigger, i.e. does not get compiled as a static field.
let IsSimpleSyntacticConstantExpr g inputExpr = 
    let rec checkExpr (vrefs: Set<Stamp>) x = 
        match stripExpr x with 
        | Expr.Op (TOp.Coerce, _, [arg], _) 
             -> checkExpr vrefs arg
        | UnopExpr g (vref, arg) 
             when (valRefEq g vref g.unchecked_unary_minus_vref ||
                   valRefEq g vref g.unchecked_unary_plus_vref ||
                   valRefEq g vref g.unchecked_unary_not_vref ||
                   valRefEq g vref g.bitwise_unary_not_vref ||
                   valRefEq g vref g.enum_vref)
             -> checkExpr vrefs arg
        // compare, =, <>, +, -, <, >, <=, >=, <<<, >>>, &&&
        | BinopExpr g (vref, arg1, arg2) 
             when (valRefEq g vref g.equals_operator_vref  ||
                   valRefEq g vref g.compare_operator_vref  ||
                   valRefEq g vref g.unchecked_addition_vref  ||
                   valRefEq g vref g.less_than_operator_vref  ||
                   valRefEq g vref g.less_than_or_equals_operator_vref  ||
                   valRefEq g vref g.greater_than_operator_vref  ||
                   valRefEq g vref g.greater_than_or_equals_operator_vref  ||
                   valRefEq g vref g.not_equals_operator_vref  ||
                   valRefEq g vref g.unchecked_addition_vref  ||
                   valRefEq g vref g.unchecked_multiply_vref  ||
                   valRefEq g vref g.unchecked_subtraction_vref  ||
        // Note: division and modulus can raise exceptions, so are not included
                   valRefEq g vref g.bitwise_shift_left_vref  ||
                   valRefEq g vref g.bitwise_shift_right_vref  ||
                   valRefEq g vref g.bitwise_xor_vref  ||
                   valRefEq g vref g.bitwise_and_vref  ||
                   valRefEq g vref g.bitwise_or_vref) &&
                   (not (typeEquiv g (tyOfExpr g arg1) g.string_ty)  && not (typeEquiv g (tyOfExpr g arg1) g.decimal_ty) )
                -> checkExpr vrefs arg1 && checkExpr vrefs arg2 
        | Expr.Val(vref, _, _) -> vref.Deref.IsCompiledAsStaticPropertyWithoutField || vrefs.Contains vref.Stamp
        | Expr.Match(_, _, dtree, targets, _, _) -> checkDecisionTree vrefs dtree && targets |> Array.forall (checkDecisionTreeTarget vrefs)
        | Expr.Let(b, e, _, _) -> checkExpr vrefs b.Expr && checkExpr (vrefs.Add b.Var.Stamp) e
        // Detect standard constants 
        | Expr.TyChoose (_, b, _) -> checkExpr vrefs b
        | Expr.Const _ 
        | Expr.Op (TOp.UnionCase _, _, [], _)         // Nullary union cases
        | UncheckedDefaultOfExpr g _ 
        | SizeOfExpr g _ 
        | TypeOfExpr g _ -> true
        // All others are not simple constant expressions
        | _ -> false

    and checkDecisionTree vrefs x = 
        match x with 
        | TDSuccess (es, _n) -> es |> List.forall (checkExpr vrefs)
        | TDSwitch (e, cases, dflt, _m) -> checkExpr vrefs e && cases |> List.forall (checkDecisionTreeCase vrefs) && dflt |> Option.forall (checkDecisionTree vrefs)
        | TDBind (bind, body) -> checkExpr vrefs bind.Expr && checkDecisionTree (vrefs.Add bind.Var.Stamp) body
    and checkDecisionTreeCase vrefs (TCase(discrim, dtree)) = 
       (match discrim with DecisionTreeTest.Const _c -> true | _ -> false) && checkDecisionTree vrefs dtree
    and checkDecisionTreeTarget vrefs (TTarget(vs, e, _)) = 
       let vrefs = ((vrefs, vs) ||> List.fold (fun s v -> s.Add v.Stamp)) 
       checkExpr vrefs e

    checkExpr Set.empty inputExpr    
    
let EvalArithBinOp (opInt8, opInt16, opInt32, opInt64, opUInt8, opUInt16, opUInt32, opUInt64) (arg1:Expr) (arg2:Expr) = 
    // At compile-time we check arithmetic 
    let m = unionRanges arg1.Range arg2.Range
    try 
        match arg1, arg2 with 
        | Expr.Const(Const.Int32  x1, _, ty), Expr.Const(Const.Int32  x2, _, _) -> Expr.Const(Const.Int32  (opInt32 x1 x2), m, ty)
        | Expr.Const(Const.SByte  x1, _, ty), Expr.Const(Const.SByte  x2, _, _) -> Expr.Const(Const.SByte  (opInt8 x1 x2), m, ty)
        | Expr.Const(Const.Int16  x1, _, ty), Expr.Const(Const.Int16  x2, _, _) -> Expr.Const(Const.Int16  (opInt16 x1 x2), m, ty)
        | Expr.Const(Const.Int64  x1, _, ty), Expr.Const(Const.Int64  x2, _, _) -> Expr.Const(Const.Int64  (opInt64 x1 x2), m, ty)
        | Expr.Const(Const.Byte   x1, _, ty), Expr.Const(Const.Byte   x2, _, _) -> Expr.Const(Const.Byte   (opUInt8 x1 x2), m, ty)
        | Expr.Const(Const.UInt16 x1, _, ty), Expr.Const(Const.UInt16 x2, _, _) -> Expr.Const(Const.UInt16 (opUInt16 x1 x2), m, ty)
        | Expr.Const(Const.UInt32 x1, _, ty), Expr.Const(Const.UInt32 x2, _, _) -> Expr.Const(Const.UInt32 (opUInt32 x1 x2), m, ty)
        | Expr.Const(Const.UInt64 x1, _, ty), Expr.Const(Const.UInt64 x2, _, _) -> Expr.Const(Const.UInt64 (opUInt64 x1 x2), m, ty)
        | _ -> error (Error ( FSComp.SR.tastNotAConstantExpression(), m))
    with :? System.OverflowException  -> error (Error ( FSComp.SR.tastConstantExpressionOverflow(), m))

// See also PostTypeCheckSemanticChecks.CheckAttribArgExpr, which must match this precisely
let rec EvalAttribArgExpr g x = 
    match x with 

    // Detect standard constants 
    | Expr.Const(c, m, _) -> 
        match c with 
        | Const.Bool _ 
        | Const.Int32 _ 
        | Const.SByte  _
        | Const.Int16  _
        | Const.Int32 _
        | Const.Int64 _  
        | Const.Byte  _
        | Const.UInt16  _
        | Const.UInt32  _
        | Const.UInt64  _
        | Const.Double _
        | Const.Single _
        | Const.Char _
        | Const.Zero _
        | Const.String _  -> 
            x
        | Const.Decimal _ | Const.IntPtr _ | Const.UIntPtr _ | Const.Unit _ ->
            errorR (Error ( FSComp.SR.tastNotAConstantExpression(), m))
            x

    | TypeOfExpr g _ -> x
    | TypeDefOfExpr g _ -> x
    | Expr.Op (TOp.Coerce, _, [arg], _) -> 
        EvalAttribArgExpr g arg
    | EnumExpr g arg1 -> 
        EvalAttribArgExpr g arg1
    // Detect bitwise or of attribute flags
    | AttribBitwiseOrExpr g (arg1, arg2) -> 
        EvalArithBinOp ((|||), (|||), (|||), (|||), (|||), (|||), (|||), (|||)) (EvalAttribArgExpr g arg1) (EvalAttribArgExpr g arg2) 
    | SpecificBinopExpr g g.unchecked_addition_vref (arg1, arg2) -> 
       // At compile-time we check arithmetic 
       let v1, v2 = EvalAttribArgExpr g arg1, EvalAttribArgExpr g arg2 
       match v1, v2 with 
       | Expr.Const(Const.String x1, m, ty), Expr.Const(Const.String x2, _, _) -> Expr.Const(Const.String (x1 + x2), m, ty)
       | _ -> 
#if ALLOW_ARITHMETIC_OPS_IN_LITERAL_EXPRESSIONS_AND_ATTRIBUTE_ARGS
           EvalArithBinOp (Checked.(+), Checked.(+), Checked.(+), Checked.(+), Checked.(+), Checked.(+), Checked.(+), Checked.(+)) g v1 v2
#else
           errorR (Error ( FSComp.SR.tastNotAConstantExpression(), x.Range)); 
           x
#endif
#if ALLOW_ARITHMETIC_OPS_IN_LITERAL_EXPRESSIONS_AND_ATTRIBUTE_ARGS
    | SpecificBinopExpr g g.unchecked_subtraction_vref (arg1, arg2) -> 
       EvalArithBinOp (Checked.(-), Checked.(-), Checked.(-), Checked.(-), Checked.(-), Checked.(-), Checked.(-), Checked.(-)) g (EvalAttribArgExpr g arg1) (EvalAttribArgExpr g arg2)
    | SpecificBinopExpr g g.unchecked_multiply_vref (arg1, arg2) -> 
       EvalArithBinOp (Checked.(*), Checked.(*), Checked.(*), Checked.(*), Checked.(*), Checked.(*), Checked.(*), Checked.(*)) g (EvalAttribArgExpr g arg1) (EvalAttribArgExpr g arg2)
#endif
    | _ -> 
        errorR (Error ( FSComp.SR.tastNotAConstantExpression(), x.Range)); 
        x


and EvaledAttribExprEquality g e1 e2 = 
    match e1, e2 with 
    | Expr.Const(c1, _, _), Expr.Const(c2, _, _) -> c1 = c2
    | TypeOfExpr g ty1, TypeOfExpr g ty2  -> typeEquiv g ty1 ty2
    | TypeDefOfExpr g ty1, TypeDefOfExpr g ty2 -> typeEquiv g ty1 ty2
    | _ -> false

let (|ConstToILFieldInit|_|) c =
    match c with 
    | Const.SByte n   -> Some (ILFieldInit.Int8 n)
    | Const.Int16 n   -> Some (ILFieldInit.Int16 n)
    | Const.Int32 n   -> Some (ILFieldInit.Int32 n)
    | Const.Int64 n   -> Some (ILFieldInit.Int64 n)
    | Const.Byte n    -> Some (ILFieldInit.UInt8 n)
    | Const.UInt16 n  -> Some (ILFieldInit.UInt16 n)
    | Const.UInt32 n  -> Some (ILFieldInit.UInt32 n)
    | Const.UInt64 n  -> Some (ILFieldInit.UInt64 n)
    | Const.Bool n    -> Some (ILFieldInit.Bool n)
    | Const.Char n    -> Some (ILFieldInit.Char (uint16 n))
    | Const.Single n  -> Some (ILFieldInit.Single n)
    | Const.Double n  -> Some (ILFieldInit.Double n)
    | Const.String s  -> Some (ILFieldInit.String s)
    | Const.Zero      -> Some (ILFieldInit.Null)
    | _               -> None

let EvalLiteralExprOrAttribArg g x = 
    match x with 
    | Expr.Op (TOp.Coerce, _, [Expr.Op (TOp.Array, [elemTy], args, m)], _)
    | Expr.Op (TOp.Array, [elemTy], args, m) ->
        let args = args |> List.map (EvalAttribArgExpr g) 
        Expr.Op (TOp.Array, [elemTy], args, m) 
    | _ -> 
        EvalAttribArgExpr g x

// Take into account the fact that some "instance" members are compiled as static
// members when using CompilationRepresentation.Static, or any non-virtual instance members
// in a type that supports "null" as a true value. This is all members
// where ValRefIsCompiledAsInstanceMember is false but membInfo.MemberFlags.IsInstance 
// is true.
//
// This is the right abstraction for viewing member types, but the implementation
// below is a little ugly.
let GetTypeOfIntrinsicMemberInCompiledForm g (vref:ValRef) =
    assert (not vref.IsExtensionMember)
    let membInfo, topValInfo = checkMemberValRef vref
    let tps, argInfos, rty, retInfo = GetTypeOfMemberInMemberForm g vref
    let argInfos = 
        // Check if the thing is really an instance member compiled as a static member
        // If so, the object argument counts as a normal argument in the compiled form
        if membInfo.MemberFlags.IsInstance && not (ValRefIsCompiledAsInstanceMember g vref) then 
            let _, origArgInfos, _, _ = GetTopValTypeInFSharpForm g topValInfo vref.Type vref.Range
            match origArgInfos with
            | [] -> 
                errorR(InternalError("value does not have a valid member type", vref.Range)); 
                argInfos
            | h::_ -> h ::argInfos
        else argInfos
    tps, argInfos, rty, retInfo


//--------------------------------------------------------------------------
// Tuple compilation (expressions)
//------------------------------------------------------------------------ 


let rec mkCompiledTuple g isStruct (argtys, args, m) = 
    let n = List.length argtys 
    if n <= 0 then failwith "mkCompiledTuple"
    elif n < maxTuple then (mkCompiledTupleTyconRef g isStruct n, argtys, args, m)
    else
        let argtysA, argtysB = List.splitAfter goodTupleFields argtys
        let argsA, argsB = List.splitAfter goodTupleFields args
        let ty8, v8 = 
            match argtysB, argsB with 
            | [ty8], [arg8] -> 
                match ty8 with
                // if it's already been nested or ended, pass it through
                |  TType_app(tn, _)  when (isCompiledTupleTyconRef g tn) ->
                    ty8, arg8
                | _ ->
                    let ty8enc = TType_app((if isStruct then g.struct_tuple1_tcr else g.ref_tuple1_tcr), [ty8])
                    let v8enc = Expr.Op (TOp.Tuple (TupInfo.Const isStruct), [ty8], [arg8], m) 
                    ty8enc, v8enc
            | _ -> 
                let a, b, c, d = mkCompiledTuple g isStruct (argtysB, argsB, m)
                let ty8plus = TType_app(a, b)
                let v8plus = Expr.Op (TOp.Tuple(TupInfo.Const isStruct), b, c, d)
                ty8plus, v8plus
        let argtysAB = argtysA @ [ty8] 
        (mkCompiledTupleTyconRef g isStruct (List.length argtysAB), argtysAB, argsA @ [v8], m)

let mkILMethodSpecForTupleItem (_g : TcGlobals) (typ:ILType) n = 
    mkILNonGenericInstanceMethSpecInTy(typ, (if n < goodTupleFields then "get_Item"+(n+1).ToString() else "get_Rest"), [], mkILTyvarTy (uint16 n))

let mkILFieldSpecForTupleItem (typ:ILType) n = 
    mkILFieldSpecInTy (typ, (if n < goodTupleFields then "Item"+(n+1).ToString() else "Rest"), mkILTyvarTy (uint16 n))

let mkGetTupleItemN g m n (typ:ILType) isStruct te retty =
    if isStruct then
        mkAsmExpr([mkNormalLdfld  (mkILFieldSpecForTupleItem typ n)   ], [], [te], [retty], m)
    else
        mkAsmExpr([IL.mkNormalCall(mkILMethodSpecForTupleItem g typ n)], [], [te], [retty], m)
/// Match an Int32 constant expression
let (|Int32Expr|_|) expr = 
    match expr with 
    | Expr.Const(Const.Int32 n, _, _) -> Some n
    | _ -> None 

/// Match a try-finally expression
let (|TryFinally|_|) expr = 
    match expr with 
    | Expr.Op (TOp.TryFinally _, [_resty], [Expr.Lambda(_, _, _, [_], e1, _, _); Expr.Lambda(_, _, _, [_], e2, _, _)], _) -> Some(e1, e2)
    | _ -> None
    
// detect ONLY the while loops that result from compiling 'for ... in ... do ...'
let (|WhileLoopForCompiledForEachExpr|_|) expr = 
    match expr with 
    | Expr.Op (TOp.While (_, WhileLoopForCompiledForEachExprMarker), _, [Expr.Lambda(_, _, _, [_], e1, _, _); Expr.Lambda(_, _, _, [_], e2, _, _)], m) -> Some(e1, e2, m)
    | _ -> None
    
let (|Let|_|) expr = 
    match expr with 
    | Expr.Let(TBind(v, e1, sp), e2, _, _) -> Some(v, e1, sp, e2)
    | _ -> None

let (|RangeInt32Step|_|) g expr = 
    match expr with 
    // detect 'n .. m' 
    | Expr.App(Expr.Val(vf, _, _), _, [tyarg], [startExpr;finishExpr], _)
         when valRefEq g vf g.range_op_vref && typeEquiv g tyarg g.int_ty -> Some(startExpr, 1, finishExpr)
    
    // detect (RangeInt32 startExpr N finishExpr), the inlined/compiled form of 'n .. m' and 'n .. N .. m'
    | Expr.App(Expr.Val(vf, _, _), _, [], [startExpr; Int32Expr n; finishExpr], _)
         when valRefEq g vf g.range_int32_op_vref -> Some(startExpr, n, finishExpr)

    | _ -> None

let (|GetEnumeratorCall|_|) expr =   
    match expr with   
    | Expr.Op (TOp.ILCall( _, _, _, _, _, _, _, iLMethodRef, _, _, _), _, [Expr.Val(vref, _, _) | Expr.Op(_, _, [Expr.Val(vref, ValUseFlag.NormalValUse, _)], _) ], _) ->  
        if iLMethodRef.Name = "GetEnumerator" then Some(vref)  
        else None  
    | _ -> None  

let (|CompiledForEachExpr|_|) g expr =   
    match expr with
    | Let (enumerableVar, enumerableExpr, _, 
           Let (enumeratorVar, GetEnumeratorCall enumerableVar2, enumeratorBind, 
              TryFinally (WhileLoopForCompiledForEachExpr (_, Let (elemVar, _, _, bodyExpr), _), _))) 
                 // Apply correctness conditions to ensure this really is a compiled for-each expression.
                 when valRefEq g (mkLocalValRef enumerableVar) enumerableVar2 &&
                      enumerableVar.IsCompilerGenerated &&
                      enumeratorVar.IsCompilerGenerated &&
                      (let fvs = (freeInExpr CollectLocals bodyExpr)
                       not (Zset.contains enumerableVar fvs.FreeLocals) && 
                       not (Zset.contains enumeratorVar fvs.FreeLocals)) ->

        // Extract useful ranges
        let mEnumExpr = enumerableExpr.Range
        let mBody = bodyExpr.Range
        let mWholeExpr = expr.Range

        let spForLoop, mForLoop = match enumeratorBind with SequencePointAtBinding(spStart) -> SequencePointAtForLoop(spStart), spStart  |  _ -> NoSequencePointAtForLoop, mEnumExpr
        let spWhileLoop   = match enumeratorBind with SequencePointAtBinding(spStart) -> SequencePointAtWhileLoop(spStart)|  _ -> NoSequencePointAtWhileLoop
        let enumerableTy = tyOfExpr g enumerableExpr

        Some (enumerableTy, enumerableExpr, elemVar, bodyExpr, (mEnumExpr, mBody, spForLoop, mForLoop, spWhileLoop, mWholeExpr))
    | _ -> None  
             

let (|CompiledInt32RangeForEachExpr|_|) g expr = 
    match expr with
    | CompiledForEachExpr g (_, RangeInt32Step g (startExpr, step, finishExpr), elemVar, bodyExpr, ranges) ->
        Some (startExpr, step, finishExpr, elemVar, bodyExpr, ranges)
        | _ -> None
    | _ -> None


type OptimizeForExpressionOptions = OptimizeIntRangesOnly | OptimizeAllForExpressions

let DetectAndOptimizeForExpression g option expr =
    match option, expr with
    | _, CompiledInt32RangeForEachExpr g (startExpr, (1 | -1 as step), finishExpr, elemVar, bodyExpr, ranges) -> 

           let (_mEnumExpr, _mBody, spForLoop, _mForLoop, _spWhileLoop, mWholeExpr) = ranges
           mkFastForLoop  g (spForLoop, mWholeExpr, elemVar, startExpr, (step = 1), finishExpr, bodyExpr)

    | OptimizeAllForExpressions, CompiledForEachExpr g (enumerableTy, enumerableExpr, elemVar, bodyExpr, ranges) ->

         let (mEnumExpr, mBody, spForLoop, mForLoop, spWhileLoop, mWholeExpr) = ranges

         if isStringTy g enumerableTy then
            // type is string, optimize for expression as:
            //  let $str = enumerable
            //  for $idx in 0..(str.Length - 1) do
            //      let elem = str.[idx]
            //      body elem

            let strVar      , strExpr    = mkCompGenLocal mEnumExpr "str" enumerableTy
            let idxVar      , idxExpr    = mkCompGenLocal elemVar.Range "idx" g.int32_ty

            let lengthExpr              = mkGetStringLength g mForLoop strExpr
            let charExpr                = mkGetStringChar g mForLoop strExpr idxExpr

            let startExpr               = mkZero g mForLoop
            let finishExpr              = mkDecr g mForLoop lengthExpr
            let loopItemExpr            = mkCoerceIfNeeded g elemVar.Type g.char_ty charExpr  // for compat reasons, loop item over string is sometimes object, not char
            let bodyExpr                = mkCompGenLet mForLoop elemVar loopItemExpr bodyExpr
            let forExpr                 = mkFastForLoop g (spForLoop, mWholeExpr, idxVar, startExpr, true, finishExpr, bodyExpr)
            let expr                    = mkCompGenLet mEnumExpr strVar enumerableExpr forExpr

            expr

         elif isListTy g enumerableTy then
            // type is list, optimize for expression as:
            //  let mutable $currentVar = listExpr
            //  let mutable $nextVar    = $tailOrNull
            //  while $guardExpr do
            //    let i = $headExpr
            //    bodyExpr ()
            //    $current   <- $next
            //    $next      <- $tailOrNull

            let IndexHead                   = 0
            let IndexTail                   = 1

            let currentVar  , currentExpr    = mkMutableCompGenLocal mEnumExpr "current" enumerableTy
            let nextVar     , nextExpr       = mkMutableCompGenLocal mEnumExpr "next" enumerableTy
            let elemTy                      = destListTy g enumerableTy

            let guardExpr                   = mkNonNullTest g mForLoop nextExpr
            let headOrDefaultExpr           = mkUnionCaseFieldGetUnprovenViaExprAddr (currentExpr, g.cons_ucref, [elemTy], IndexHead, mForLoop)
            let tailOrNullExpr              = mkUnionCaseFieldGetUnprovenViaExprAddr (currentExpr, g.cons_ucref, [elemTy], IndexTail, mForLoop)
            let bodyExpr                    =
                mkCompGenLet mForLoop elemVar headOrDefaultExpr
                    (mkCompGenSequential mForLoop
                        bodyExpr
                        (mkCompGenSequential mForLoop
                            (mkValSet mForLoop (mkLocalValRef currentVar) nextExpr)
                            (mkValSet mForLoop (mkLocalValRef nextVar) tailOrNullExpr)
                        )
                    )

            let expr =
               // let mutable current = enumerableExpr
                let spBind = (match spForLoop with SequencePointAtForLoop(spStart) -> SequencePointAtBinding(spStart) | NoSequencePointAtForLoop -> NoSequencePointAtStickyBinding)
                mkLet spBind mEnumExpr currentVar enumerableExpr
                    // let mutable next = current.TailOrNull
                    (mkCompGenLet mForLoop nextVar tailOrNullExpr 
                        // while nonNull next dp
                       (mkWhile g (spWhileLoop, WhileLoopForCompiledForEachExprMarker, guardExpr, bodyExpr, mBody)))

            expr

         else
            expr
    | _ -> expr

// Used to remove Expr.Link for inner expressions in pattern matches
let (|InnerExprPat|) expr = stripExpr expr

/// One of the transformations performed by the compiler
/// is to eliminate variables of static type "unit".  These is a
/// utility function related to this.

let BindUnitVars g (mvs:Val list, paramInfos:ArgReprInfo list, body) = 
    match mvs, paramInfos with 
    | [v], [] -> 
        assert isUnitTy g v.Type
        [], mkLet NoSequencePointAtInvisibleBinding v.Range v (mkUnit g v.Range) body 
    | _ -> mvs, body


