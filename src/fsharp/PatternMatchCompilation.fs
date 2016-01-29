// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.PatternMatchCompilation

open System.Collections.Generic
open Internal.Utilities
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.Tastops.DebugPrint
open Microsoft.FSharp.Compiler.PrettyNaming
open Microsoft.FSharp.Compiler.TypeRelations
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.Lib

exception MatchIncomplete of bool * (string * bool) option * range
exception RuleNeverMatched of range

type ActionOnFailure = 
  | ThrowIncompleteMatchException 
  | IgnoreWithWarning 
  | Throw 
  | Rethrow 
  | FailFilter

[<NoEquality; NoComparison>]
/// Represents type-checked patterns
type Pattern =
  | TPat_const of Const * range
  | TPat_wild of range  (* note = TPat_disjs([],m), but we haven't yet removed that duplication *)
  | TPat_as of  Pattern * PatternValBinding * range (* note: can be replaced by TPat_var, i.e. equals TPat_conjs([TPat_var; pat]) *)
  | TPat_disjs of  Pattern list * range
  | TPat_conjs of  Pattern list * range
  | TPat_query of (Expr * TType list * (ValRef * TypeInst) option * int * ActivePatternInfo) * Pattern * range
  | TPat_unioncase of UnionCaseRef * TypeInst * Pattern list * range
  | TPat_exnconstr of TyconRef * Pattern list * range
  | TPat_tuple of  Pattern list * TType list * range
  | TPat_array of  Pattern list * TType * range
  | TPat_recd of TyconRef * TypeInst * Pattern list * range
  | TPat_range of char * char * range
  | TPat_null of range
  | TPat_isinst of TType * TType * PatternValBinding option * range
  member this.Range =
    match this with
    |   TPat_const(_,m) -> m
    |   TPat_wild m -> m
    |   TPat_as(_,_,m) -> m
    |   TPat_disjs(_,m) -> m
    |   TPat_conjs(_,m) -> m
    |   TPat_query(_,_,m) -> m
    |   TPat_unioncase(_,_,_,m) -> m
    |   TPat_exnconstr(_,_,m) -> m
    |   TPat_tuple(_,_,m) -> m
    |   TPat_array(_,_,m) -> m
    |   TPat_recd(_,_,_,m) -> m
    |   TPat_range(_,_,m) -> m
    |   TPat_null(m) -> m
    |   TPat_isinst(_,_,_,m) -> m

and PatternValBinding = PBind of Val * TypeScheme

and TypedMatchClause =  
    | TClause of Pattern * Expr option * DecisionTreeTarget * range
    member c.GuardExpr = let (TClause(_,whenOpt,_,_)) = c in whenOpt
    member c.Pattern = let (TClause(p,_,_,_)) = c in p
    member c.Range = let (TClause(_,_,_,m)) = c in m
    member c.Target = let (TClause(_,_,tg,_)) = c in tg
    member c.BoundVals = let (TClause(_p,_whenOpt,TTarget(vs,_,_),_m)) = c in vs

let debug = false

//---------------------------------------------------------------------------
// Nasty stuff to permit obscure generic bindings such as 
//     let x,y = [],[]
//
// BindSubExprOfInput actually produces the binding
// e.g. let v2 = \Gamma ['a,'b]. ([] : 'a ,[] : 'b)
//      let (x,y) = p.  
// When v = x, gtvs = 'a,'b.  We must bind:
//     x --> \Gamma A. fst (v2[A,<dummy>]) 
//     y --> \Gamma A. snd (v2[<dummy>,A]).  
// 
// GetSubExprOfInput is just used to get a concrete value from a type
// function in the middle of the "test" part of pattern matching.
// For example, e.g.  let [x; y] = [ (\x.x); (\x.x) ] 
// Here the constructor test needs a real list, even though the
// r.h.s. is actually a polymorphic type function.  To do the
// test, we apply the r.h.s. to a dummy type - it doesn't matter
// which (unless the r.h.s. actually looks at it's type argument...)
//---------------------------------------------------------------------------

type SubExprOfInput = 
    | SubExpr of (TyparInst -> Expr -> Expr) * (Expr * Val)

let BindSubExprOfInput g amap gtps (PBind(v,tyscheme)) m (SubExpr(accessf,(ve2,v2))) =
    let e' = 
        if isNil gtps then 
            accessf [] ve2 
        else 
            let tyargs = 
                let someSolved = ref false
                let freezeVar gtp = 
                     if isBeingGeneralized gtp tyscheme then 
                         mkTyparTy gtp 
                     else 
                         someSolved := true
                         TypeRelations.ChooseTyparSolution g amap gtp

                let solutions = List.map freezeVar gtps
                if !someSolved then 
                    TypeRelations.IterativelySubstituteTyparSolutions g gtps solutions
                else
                    solutions

            let tinst = mkTyparInst gtps tyargs
            accessf tinst (mkApps g ((ve2,v2.Type),[tyargs],[],v2.Range))

    v,mkGenericBindRhs g m [] tyscheme e'

let GetSubExprOfInput g (gtps,tyargs,tinst) (SubExpr(accessf,(ve2,v2))) =
    if isNil gtps then accessf [] ve2 else
    accessf tinst (mkApps g ((ve2,v2.Type),[tyargs],[],v2.Range))

//---------------------------------------------------------------------------
// path, frontier
//---------------------------------------------------------------------------

// A path reaches into a pattern.
// The ints record which choices taken, e.g. tuple/record fields.
type Path = 
    | PathQuery of Path * Unique
    | PathConj of Path * int
    | PathTuple of Path * TypeInst * int
    | PathRecd of Path * TyconRef * TypeInst * int
    | PathUnionConstr of Path * UnionCaseRef * TypeInst * int
    | PathArray of Path * TType * int * int
    | PathExnConstr of Path * TyconRef * int
    | PathEmpty of TType 

let rec pathEq p1 p2 = 
    match p1,p2 with
    | PathQuery(p1,n1), PathQuery(p2,n2) -> (n1 = n2) && pathEq p1 p2
    | PathConj(p1,n1), PathConj(p2,n2) -> (n1 = n2) && pathEq p1 p2
    | PathTuple(p1,_,n1), PathTuple(p2,_,n2) -> (n1 = n2) && pathEq p1 p2
    | PathRecd(p1,_,_,n1), PathRecd(p2,_,_,n2) -> (n1 = n2) && pathEq p1 p2
    | PathUnionConstr(p1,_,_,n1), PathUnionConstr(p2,_,_,n2) -> (n1 = n2) && pathEq p1 p2
    | PathArray(p1,_,_,n1), PathArray(p2,_,_,n2) -> (n1 = n2) && pathEq p1 p2
    | PathExnConstr(p1,_,n1), PathExnConstr(p2,_,n2) -> (n1 = n2) && pathEq p1 p2
    | PathEmpty(_), PathEmpty(_) -> true
    | _ -> false


//---------------------------------------------------------------------------
// Counter example generation 
//---------------------------------------------------------------------------

type RefutedSet = 
    /// A value RefutedInvestigation(path,discrim) indicates that the value at the given path is known 
    /// to NOT be matched by the given discriminator
    | RefutedInvestigation of Path * Test list
    /// A value RefutedWhenClause indicates that a 'when' clause failed
    | RefutedWhenClause

let notNullText = "some-non-null-value"
let otherSubtypeText = "some-other-subtype"

exception CannotRefute
let RefuteDiscrimSet g m path discrims = 
    let mkUnknown ty = snd(mkCompGenLocal m "_" ty)
    let rec go path tm = 
        match path with 
        | PathQuery _ -> raise CannotRefute
        | PathConj (p,_j) -> 
             go p tm
        | PathTuple (p,tys,j) -> 
             go p (fun _ -> mkTupled g m (mkOneKnown tm j tys) tys)
        | PathRecd (p,tcref,tinst,j) -> 
             let flds = tcref |> actualTysOfInstanceRecdFields (mkTyconRefInst tcref tinst) |> mkOneKnown tm j
             go p (fun _ -> Expr.Op(TOp.Recd(RecdExpr, tcref),tinst, flds,m))

        | PathUnionConstr (p,ucref,tinst,j) -> 
             let flds = ucref |> actualTysOfUnionCaseFields (mkTyconRefInst ucref.TyconRef tinst)|> mkOneKnown tm j
             go p (fun _ -> Expr.Op(TOp.UnionCase(ucref),tinst, flds,m))

        | PathArray (p,ty,len,n) -> 
             go p (fun _ -> Expr.Op(TOp.Array,[ty], mkOneKnown tm n (List.replicate len ty) ,m))

        | PathExnConstr (p,ecref,n) -> 
             let flds = ecref |> recdFieldTysOfExnDefRef |> mkOneKnown tm n
             go p (fun _ -> Expr.Op(TOp.ExnConstr(ecref),[], flds,m))

        | PathEmpty(ty) -> tm ty
        
    and mkOneKnown tm n tys = List.mapi (fun i ty -> if i = n then tm ty else mkUnknown ty) tys 
    and mkUnknowns tys = List.map mkUnknown tys

    let tm ty = 
        match discrims with 
        | [Test.IsNull] -> 
            snd(mkCompGenLocal m notNullText ty)
        | [Test.IsInst (_,_)] -> 
            snd(mkCompGenLocal m otherSubtypeText ty)
        | (Test.Const c :: rest) -> 
            let consts = Set.ofList (c :: List.choose (function Test.Const(c) -> Some c | _ -> None) rest)
            let c' = 
                Seq.tryFind (fun c -> not (consts.Contains(c)))
                     (match c with 
                      | Const.Bool _ -> [ true; false ] |> List.toSeq |> Seq.map (fun v -> Const.Bool(v))
                      | Const.SByte _ ->  Seq.append (seq { 0y .. System.SByte.MaxValue }) (seq { System.SByte.MinValue .. 0y })|> Seq.map (fun v -> Const.SByte(v))
                      | Const.Int16 _ -> Seq.append (seq { 0s .. System.Int16.MaxValue }) (seq { System.Int16.MinValue .. 0s }) |> Seq.map (fun v -> Const.Int16(v))
                      | Const.Int32 _ ->  Seq.append (seq { 0 .. System.Int32.MaxValue }) (seq { System.Int32.MinValue .. 0 })|> Seq.map (fun v -> Const.Int32(v))
                      | Const.Int64 _ ->  Seq.append (seq { 0L .. System.Int64.MaxValue }) (seq { System.Int64.MinValue .. 0L })|> Seq.map (fun v -> Const.Int64(v))
                      | Const.IntPtr _ ->  Seq.append (seq { 0L .. System.Int64.MaxValue }) (seq { System.Int64.MinValue .. 0L })|> Seq.map (fun v -> Const.IntPtr(v))
                      | Const.Byte _ -> seq { 0uy .. System.Byte.MaxValue } |> Seq.map (fun v -> Const.Byte(v))
                      | Const.UInt16 _ -> seq { 0us .. System.UInt16.MaxValue } |> Seq.map (fun v -> Const.UInt16(v))
                      | Const.UInt32 _ -> seq { 0u .. System.UInt32.MaxValue } |> Seq.map (fun v -> Const.UInt32(v))
                      | Const.UInt64 _ -> seq { 0UL .. System.UInt64.MaxValue } |> Seq.map (fun v -> Const.UInt64(v))
                      | Const.UIntPtr _ -> seq { 0UL .. System.UInt64.MaxValue } |> Seq.map (fun v -> Const.UIntPtr(v))
                      | Const.Double _ -> seq { 0 .. System.Int32.MaxValue } |> Seq.map (fun v -> Const.Double(float v))
                      | Const.Single _ -> seq { 0 .. System.Int32.MaxValue } |> Seq.map (fun v -> Const.Single(float32 v))
                      | Const.Char _ -> seq { 32us .. System.UInt16.MaxValue } |> Seq.map (fun v -> Const.Char(char v))
                      | Const.String _ -> seq { 1 .. System.Int32.MaxValue } |> Seq.map (fun v -> Const.String(new System.String('a',v)))
                      | Const.Decimal _ -> seq { 1 .. System.Int32.MaxValue } |> Seq.map (fun v -> Const.Decimal(decimal v))
                      | _ -> 
                          raise CannotRefute) 

            (* REVIEW: we could return a better enumeration literal field here if a field matches one of the enumeration cases *)

            match c' with 
            | None -> raise CannotRefute
            | Some c -> Expr.Const(c,m,ty)
            
        | (Test.UnionCase (ucref1,tinst) :: rest) -> 
             let ucrefs = ucref1 :: List.choose (function Test.UnionCase(ucref,_) -> Some ucref | _ -> None) rest
             let tcref = ucref1.TyconRef
             (* Choose the first ucref based on ordering of names *)
             let others = 
                 tcref.UnionCasesAsRefList 
                 |> List.filter (fun ucref -> not (List.exists (g.unionCaseRefEq ucref) ucrefs)) 
                 |> List.sortBy (fun ucref -> ucref.CaseName)
             match others with 
             | [] -> raise CannotRefute
             | ucref2 :: _ -> 
               let flds = ucref2 |> actualTysOfUnionCaseFields (mkTyconRefInst tcref tinst) |> mkUnknowns
               Expr.Op(TOp.UnionCase(ucref2),tinst, flds,m)
               
        | [Test.ArrayLength (n,ty)] -> 
             Expr.Op(TOp.Array,[ty], mkUnknowns (List.replicate (n+1) ty) ,m)
             
        | _ -> 
            raise CannotRefute
    go path tm

let rec CombineRefutations g r1 r2 =
   match r1,r2 with
   | Expr.Val(vref,_,_), other | other, Expr.Val(vref,_,_) when vref.LogicalName = "_" -> other 
   | Expr.Val(vref,_,_), other | other, Expr.Val(vref,_,_) when vref.LogicalName = notNullText -> other 
   | Expr.Val(vref,_,_), other | other, Expr.Val(vref,_,_) when vref.LogicalName = otherSubtypeText -> other 

   | Expr.Op((TOp.ExnConstr(ecref1) as op1), tinst1,flds1,m1), Expr.Op(TOp.ExnConstr(ecref2), _,flds2,_) when tyconRefEq g ecref1 ecref2 -> 
        Expr.Op(op1, tinst1,List.map2 (CombineRefutations g) flds1 flds2,m1)

   | Expr.Op((TOp.UnionCase(ucref1) as op1), tinst1,flds1,m1), 
     Expr.Op(TOp.UnionCase(ucref2), _,flds2,_)  -> 
       if g.unionCaseRefEq ucref1 ucref2 then 
           Expr.Op(op1, tinst1,List.map2 (CombineRefutations g) flds1 flds2,m1)
       (* Choose the greater of the two ucrefs based on name ordering *)
       elif ucref1.CaseName < ucref2.CaseName then 
           r2
       else 
           r1
        
   | Expr.Op(op1, tinst1,flds1,m1), Expr.Op(_, _,flds2,_) -> 
        Expr.Op(op1, tinst1,List.map2 (CombineRefutations g) flds1 flds2,m1)
        
   | Expr.Const(c1, m1, ty1), Expr.Const(c2,_,_) -> 
       let c12 = 

           // Make sure longer strings are greater, not the case in the default ordinal comparison
           // This is needed because the individual counter examples make longer strings
           let MaxStrings s1 s2 = 
               let c = compare (String.length s1) (String.length s2)
               if c < 0 then s2 
               elif c > 0 then s1 
               elif s1 < s2 then s2 
               else s1
               
           match c1,c2 with 
           | Const.String(s1), Const.String(s2) -> Const.String(MaxStrings s1 s2)
           | Const.Decimal(s1), Const.Decimal(s2) -> Const.Decimal(max s1 s2)
           | _ -> max c1 c2 
           
       (* REVIEW: we could return a better enumeration literal field here if a field matches one of the enumeration cases *)
       Expr.Const(c12, m1, ty1)

   | _ -> r1 

let ShowCounterExample g denv m refuted = 
   try
      let refutations = refuted |> List.collect (function RefutedWhenClause -> [] | (RefutedInvestigation(path,discrim)) -> [RefuteDiscrimSet g m path discrim])
      let counterExample = 
          match refutations with 
          | [] -> raise CannotRefute
          | h :: t -> 
              if verbose then dprintf "h = %s\n" (Layout.showL (exprL h));
              List.fold (CombineRefutations g) h t
      let text = Layout.showL (NicePrint.dataExprL denv counterExample)
      let failingWhenClause = refuted |> List.exists (function RefutedWhenClause -> true | _ -> false)
      Some(text,failingWhenClause)
      
    with 
        | CannotRefute ->    
          None 
        | e -> 
          warning(InternalError(sprintf "<failure during counter example generation: %s>" (e.ToString()),m));
          None
       
//---------------------------------------------------------------------------
// Basic problem specification
//---------------------------------------------------------------------------
    
type RuleNumber = int

type Active = Active of Path * SubExprOfInput * Pattern

type Actives = Active list

type Frontier = Frontier of RuleNumber * Actives * ValMap<Expr>

type InvestigationPoint = Investigation of RuleNumber * Test * Path

// Note: actives must be a SortedDictionary 
// REVIEW: improve these data structures, though surprisingly these functions don't tend to show up 
// on profiling runs 
let rec isMemOfActives p1 actives = 
    match actives with 
    | [] -> false 
    | (Active(p2,_,_)) :: rest -> pathEq p1 p2 || isMemOfActives p1 rest

let rec lookupActive x l = 
    match l with 
    | [] -> raise (KeyNotFoundException())
    | (Active(h,r1,r2)::t) -> if pathEq x h then (r1,r2) else lookupActive x t

let rec removeActive x l = 
    match l with 
    | [] -> []
    | ((Active(h,_,_) as p) ::t) -> if pathEq x h then t else p:: removeActive x t

//---------------------------------------------------------------------------
// Utilities
//---------------------------------------------------------------------------

// tpinst is required because the pattern is specified w.r.t. generalized type variables. 
let getDiscrimOfPattern g tpinst t = 
    match t with 
    | TPat_null _m -> 
        Some(Test.IsNull)
    | TPat_isinst (srcty,tgty,_,_m) -> 
        Some(Test.IsInst (instType tpinst srcty,instType tpinst tgty))
    | TPat_exnconstr(tcref,_,_m) -> 
        Some(Test.IsInst (g.exn_ty,mkAppTy tcref []))
    | TPat_const (c,_m) -> 
        Some(Test.Const c)
    | TPat_unioncase (c,tyargs',_,_m) -> 
        Some(Test.UnionCase (c,instTypes tpinst tyargs'))
    | TPat_array (args,ty,_m) -> 
        Some(Test.ArrayLength (args.Length,ty))
    | TPat_query ((pexp,resTys,apatVrefOpt,idx,apinfo),_,_m) -> 
        Some(Test.ActivePatternCase (pexp, instTypes tpinst resTys, apatVrefOpt,idx,apinfo))
    | _ -> None

let constOfDiscrim discrim =
    match discrim with 
    | Test.Const x -> x
    | _ -> failwith "not a const case"

let constOfCase (c: DecisionTreeCase) = constOfDiscrim c.Discriminator

/// Compute pattern identity
let discrimsEq g d1 d2 =
  match d1,d2 with 
  | Test.UnionCase (c1,_),    Test.UnionCase(c2,_) -> g.unionCaseRefEq c1 c2
  | Test.ArrayLength (n1,_),   Test.ArrayLength(n2,_) -> (n1=n2)
  | Test.Const c1,              Test.Const c2 -> (c1=c2)
  | Test.IsNull ,               Test.IsNull -> true
  | Test.IsInst (srcty1,tgty1), Test.IsInst (srcty2,tgty2) -> typeEquiv g srcty1 srcty2 && typeEquiv g tgty1 tgty2
  | Test.ActivePatternCase (_,_,vrefOpt1,n1,_),        Test.ActivePatternCase (_,_,vrefOpt2,n2,_) -> 
      match vrefOpt1, vrefOpt2 with 
      | Some (vref1, tinst1), Some (vref2, tinst2) -> valRefEq g vref1 vref2 && n1 = n2  && not (doesActivePatternHaveFreeTypars g vref1) && List.lengthsEqAndForall2 (typeEquiv g) tinst1 tinst2
      | _ -> false (* for equality purposes these are considered unequal! This is because adhoc computed patterns have no identity. *)

  | _ -> false
    
/// Redundancy of 'isinst' patterns 
let isDiscrimSubsumedBy g amap m d1 d2 =
    (discrimsEq g d1 d2) 
    ||
    (match d1,d2 with 
     | Test.IsInst (_,tgty1), Test.IsInst (_,tgty2) -> 
        TypeDefinitelySubsumesTypeNoCoercion 0 g amap m tgty2 tgty1
     | _ -> false)
    
/// Choose a set of investigations that can be performed simultaneously 
let rec chooseSimultaneousEdgeSet prevOpt f l =
    match l with 
    | [] -> [],[]
    | h::t -> 
        match f prevOpt h with 
        | Some x,_ ->         
             let l,r = chooseSimultaneousEdgeSet (Some x) f t
             x :: l, r
        | None,_cont -> 
             let l,r = chooseSimultaneousEdgeSet prevOpt f t
             l, h :: r

/// Can we represent a integer discrimination as a 'switch'
let canCompactConstantClass c = 
    match c with 
    | Const.SByte _ | Const.Int16 _ | Const.Int32 _ 
    | Const.Byte _ | Const.UInt16 _ | Const.UInt32 _ 
    | Const.Char _ -> true
    | _ -> false
                         
/// Can two discriminators in a 'column' be decided simultaneously?
let discrimsHaveSameSimultaneousClass g d1 d2 =
    match d1,d2 with 
    | Test.Const _,              Test.Const _ 
    | Test.IsNull ,               Test.IsNull 
    | Test.ArrayLength _,   Test.ArrayLength _
    | Test.UnionCase _,    Test.UnionCase _  -> true

    | Test.IsInst _, Test.IsInst _ -> false
    | Test.ActivePatternCase (_,_,apatVrefOpt1,_,_),        Test.ActivePatternCase (_,_,apatVrefOpt2,_,_) -> 
        match apatVrefOpt1, apatVrefOpt2 with 
        | Some (vref1, tinst1), Some (vref2, tinst2) -> valRefEq g vref1 vref2  && not (doesActivePatternHaveFreeTypars g vref1) && List.lengthsEqAndForall2 (typeEquiv g) tinst1 tinst2
        | _ -> false (* for equality purposes these are considered different classes of discriminators! This is because adhoc computed patterns have no identity! *)

    | _ -> false


/// Decide the next pattern to investigate
let ChooseInvestigationPointLeftToRight frontiers =
    match frontiers with 
    | Frontier (_i,actives,_) ::_t -> 
        let rec choose l = 
            match l with 
            | [] -> failwith "ChooseInvestigationPointLeftToRight: no non-immediate patterns in first rule"
            | (Active(_,_,(TPat_null _ | TPat_isinst _ | TPat_exnconstr _ | TPat_unioncase _ | TPat_array _ | TPat_const _ | TPat_query _ | TPat_range _)) as active)
                :: _ -> active
            | _ :: t -> choose t
        choose actives
    | [] -> failwith "ChooseInvestigationPointLeftToRight: no frontiers!"



#if OPTIMIZE_LIST_MATCHING
// This is an initial attempt to remove extra typetests/castclass for simple list pattern matching "match x with h::t -> ... | [] -> ..."
// The problem with this technique is that it creates extra locals which inhibit the process of converting pattern matches into linear let bindings.

let (|ListConsDiscrim|_|) g = function
     | (Test.UnionCase (ucref,tinst))
                (* check we can use a simple 'isinst' instruction *)
                when tyconRefEq g ucref.TyconRef g.list_tcr_canon & ucref.CaseName = "op_ColonColon" -> Some tinst
     | _ -> None

let (|ListEmptyDiscrim|_|) g = function
     | (Test.UnionCase (ucref,tinst))
                (* check we can use a simple 'isinst' instruction *)
                when tyconRefEq g ucref.TyconRef g.list_tcr_canon & ucref.CaseName = "op_Nil" -> Some tinst 
     | _ -> None
#endif

/// Build a dtree, equivalent to: TDSwitch("expr",edges,default,m) 
///
/// Once we've chosen a particular active to investigate, we compile the
/// set of edges affected by this investigation into a switch.  
///
///   - For Test.ActivePatternCase(...,None,...) there is only one edge
///
///   - For Test.IsInst there are multiple edges, which we can't deal with
///     one switch, so we make an iterated if-then-else to cover the cases. We
///     should probably adjust the code to only choose one edge in this case.
///
///   - Compact integer switches become a single switch.  Non-compact integer
///     switches, string switches and floating point switches are treated in the
///     same way as Test.IsInst.
let rec BuildSwitch resPreBindOpt g expr edges dflt m =
    if verbose then dprintf "--> BuildSwitch@%a, #edges = %A, dflt.IsSome = %A\n" outputRange m (List.length edges) (Option.isSome dflt); 
    match edges,dflt with 
    | [], None      -> failwith "internal error: no edges and no default"
    | [], Some dflt -> dflt      (* NOTE: first time around, edges<>[] *)

    // Optimize the case where the match always succeeds 
    | [TCase(_,tree)], None -> tree

    // 'isinst' tests where we have stored the result of the 'isinst' in a variable 
    // In this case the 'expr' already holds the result of the 'isinst' test. 

    | (TCase(Test.IsInst _,success)):: edges, dflt  when isSome resPreBindOpt -> 
        TDSwitch(expr,[TCase(Test.IsNull,BuildSwitch None g expr edges dflt m)],Some success,m)    
        
    // isnull and isinst tests
    | (TCase((Test.IsNull | Test.IsInst _),_) as edge):: edges, dflt  -> 
        TDSwitch(expr,[edge],Some (BuildSwitch resPreBindOpt g expr edges dflt m),m)    

#if OPTIMIZE_LIST_MATCHING
    // 'cons/nil' tests where we have stored the result of the cons test in an 'isinst' in a variable 
    // In this case the 'expr' already holds the result of the 'isinst' test. 
    | [TCase(ListConsDiscrim g tinst, consCase)], Some emptyCase 
    | [TCase(ListEmptyDiscrim g tinst, emptyCase)], Some consCase 
    | [TCase(ListEmptyDiscrim g _, emptyCase); TCase(ListConsDiscrim g tinst, consCase)], None
    | [TCase(ListConsDiscrim g tinst, consCase); TCase(ListEmptyDiscrim g _, emptyCase)], None
                     when isSome resPreBindOpt -> 
        TDSwitch(expr, [TCase(Test.IsNull, emptyCase)], Some consCase, m)    
#endif
                
    // All these should also always have default cases 
    | TCase(Test.Const (Const.Decimal _ | Const.String _ | Const.Single _ |  Const.Double _ | Const.SByte _ | Const.Byte _| Const.Int16 _ | Const.UInt16 _ | Const.Int32 _ | Const.UInt32 _ | Const.Int64 _ | Const.UInt64 _ | Const.IntPtr _ | Const.UIntPtr _ | Const.Char _ ),_) :: _, None -> 
        error(InternalError("inexhaustive match - need a default cases!",m))

    // Split string, float, uint64, int64, unativeint, nativeint matches into serial equality tests 
    | TCase((Test.ArrayLength _ | Test.Const (Const.Single _ | Const.Double _ | Const.String _ | Const.Decimal _ | Const.Int64 _ | Const.UInt64 _ | Const.IntPtr _ | Const.UIntPtr _)),_) :: _, Some dflt -> 
        List.foldBack 
            (fun (TCase(discrim,tree)) sofar -> 
                let testexpr = expr
                let testexpr = 
                    match discrim with 
                    | Test.ArrayLength(n,_)       -> 
                        let _v,vexp,bind = mkCompGenLocalAndInvisbleBind g "testExpr" m testexpr
                        mkLetBind m bind (mkLazyAnd g m (mkNonNullTest g m vexp) (mkILAsmCeq g m (mkLdlen g m vexp) (mkInt g m n)))
                    | Test.Const (Const.String _ as c)  -> 
                        mkCallEqualsOperator g m g.string_ty testexpr (Expr.Const(c,m,g.string_ty))
                    | Test.Const (Const.Decimal _ as c)  -> 
                        mkCallEqualsOperator g m g.decimal_ty testexpr (Expr.Const(c,m,g.decimal_ty))
                    | Test.Const ((Const.Double _ | Const.Single _ | Const.Int64 _ | Const.UInt64 _ | Const.IntPtr _ | Const.UIntPtr _) as c)   -> 
                        mkILAsmCeq g m testexpr (Expr.Const(c,m,tyOfExpr g testexpr))
                    | _ -> error(InternalError("strange switch",m))
                mkBoolSwitch m testexpr tree sofar)
          edges
          dflt

    // Split integer and char matches into compact fragments which will themselves become switch statements. 
    | TCase(Test.Const c,_) :: _, Some dflt when canCompactConstantClass c -> 
        let edgeCompare c1 c2 = 
            match constOfCase c1,constOfCase c2 with 
            | (Const.SByte i1),(Const.SByte i2) -> compare i1 i2
            | (Const.Int16 i1),(Const.Int16 i2) -> compare i1 i2
            | (Const.Int32 i1),(Const.Int32 i2) -> compare i1 i2
            | (Const.Byte i1),(Const.Byte i2) -> compare i1 i2
            | (Const.UInt16 i1),(Const.UInt16 i2) -> compare i1 i2
            | (Const.UInt32 i1),(Const.UInt32 i2) -> compare i1 i2
            | (Const.Char c1),(Const.Char c2) -> compare c1 c2
            | _ -> failwith "illtyped term during pattern compilation" 
        let edges' = List.sortWith edgeCompare edges
        let rec compactify curr edges = 
            if debug then  dprintf "--> compactify@%a\n" outputRange m; 
            match curr,edges with 
            | None,[] -> []
            | Some last,[] -> [List.rev last]
            | None,h::t -> compactify (Some [h]) t
            | Some (prev::moreprev),h::t -> 
                match constOfCase prev,constOfCase h with 
                | Const.SByte iprev,Const.SByte inext when int32(iprev) + 1 = int32 inext -> 
                    compactify (Some (h::prev::moreprev)) t
                | Const.Int16 iprev,Const.Int16 inext when int32(iprev) + 1 = int32 inext -> 
                    compactify (Some (h::prev::moreprev)) t
                | Const.Int32 iprev,Const.Int32 inext when iprev+1 = inext -> 
                    compactify (Some (h::prev::moreprev)) t
                | Const.Byte iprev,Const.Byte inext when int32(iprev) + 1 = int32 inext -> 
                    compactify (Some (h::prev::moreprev)) t
                | Const.UInt16 iprev,Const.UInt16 inext when int32(iprev)+1 = int32 inext -> 
                    compactify (Some (h::prev::moreprev)) t
                | Const.UInt32 iprev,Const.UInt32 inext when int32(iprev)+1 = int32 inext -> 
                    compactify (Some (h::prev::moreprev)) t
                | Const.Char cprev,Const.Char cnext when (int32 cprev + 1 = int32 cnext) -> 
                    compactify (Some (h::prev::moreprev)) t
                |       _ ->  (List.rev (prev::moreprev)) :: compactify None edges

            | _ -> failwith "internal error: compactify"
        let edgeGroups = compactify None edges'
        (edgeGroups, dflt) ||> List.foldBack (fun edgeGroup sofar ->  TDSwitch(expr,edgeGroup,Some sofar,m))

    // For a total pattern match, run the active pattern, bind the result and 
    // recursively build a switch in the choice type 
    | (TCase(Test.ActivePatternCase _,_)::_), _ -> 
       error(InternalError("Test.ActivePatternCase should have been eliminated",m));

    // For a complete match, optimize one test to be the default 
    | (TCase(_,tree)::rest), None -> TDSwitch (expr,rest,Some tree,m)

    // Otherwise let codegen make the choices 
    | _ -> TDSwitch (expr,edges,dflt,m)

#if DEBUG
let rec layoutPat pat = 
    if debug then  dprintf "--> layoutPat\n"; 
    match pat with
    | TPat_query (_,pat,_) -> Layout.(--) (Layout.wordL "query") (layoutPat pat)
    | TPat_wild _ -> Layout.wordL "wild"
    | TPat_as _ -> Layout.wordL "var"
    | TPat_tuple (pats, _, _) 
    | TPat_array (pats, _, _) -> Layout.bracketL (Layout.tupleL (List.map layoutPat pats))
    | _ -> Layout.wordL "?" 
  
let layoutPath _p = Layout.wordL "<path>"
     
let layoutActive (Active (path, _subexpr, pat)) =
    Layout.(--) (Layout.wordL "Active") (Layout.tupleL [layoutPath path; layoutPat pat]) 
     
let layoutFrontier (Frontier (i,actives,_)) =
    Layout.(--) (Layout.wordL "Frontier") (Layout.tupleL [intL i; Layout.listL layoutActive actives]) 
#endif

let mkFrontiers investigations i = 
    List.map (fun (actives,valMap) -> Frontier(i,actives,valMap)) investigations

let getRuleIndex (Frontier (i,_active,_valMap)) = i

/// Is a pattern a partial pattern?
let rec isPatternPartial p = 
    match p with 
    | TPat_query ((_,_,_,_,apinfo),p,_m) -> not apinfo.IsTotal || isPatternPartial p
    | TPat_const _ -> false
    | TPat_wild _ -> false
    | TPat_as (p,_,_) -> isPatternPartial p
    | TPat_disjs (ps,_) | TPat_conjs(ps,_) 
    | TPat_tuple (ps,_,_) | TPat_exnconstr(_,ps,_) 
    | TPat_array (ps,_,_) | TPat_unioncase (_,_,ps,_)
    | TPat_recd (_,_,ps,_) -> List.exists isPatternPartial ps
    | TPat_range _ -> false
    | TPat_null _ -> false
    | TPat_isinst _ -> false

let rec erasePartialPatterns inpp = 
    match inpp with 
    | TPat_query ((expr,resTys,apatVrefOpt,idx,apinfo),p,m) -> 
         if apinfo.IsTotal then TPat_query ((expr,resTys,apatVrefOpt,idx,apinfo),erasePartialPatterns p,m)
         else TPat_disjs ([],m) (* always fail *)
    | TPat_as (p,x,m) -> TPat_as (erasePartialPatterns p,x,m)
    | TPat_disjs (ps,m) -> TPat_disjs(erasePartials ps, m)
    | TPat_conjs(ps,m) -> TPat_conjs(erasePartials ps, m)
    | TPat_tuple (ps,x,m) -> TPat_tuple(erasePartials ps, x, m)
    | TPat_exnconstr(x,ps,m) -> TPat_exnconstr(x,erasePartials ps,m) 
    | TPat_array (ps,x,m) -> TPat_array (erasePartials ps,x,m)
    | TPat_unioncase (x,y,ps,m) -> TPat_unioncase (x,y,erasePartials ps,m)
    | TPat_recd (x,y,ps,m) -> TPat_recd (x,y,List.map erasePartialPatterns ps,m)
    | TPat_const _ 
    | TPat_wild _ 
    | TPat_range _ 
    | TPat_null _ 
    | TPat_isinst _ -> inpp
and erasePartials inps = List.map erasePartialPatterns inps


//---------------------------------------------------------------------------
// The algorithm
//---------------------------------------------------------------------------

type EdgeDiscrim = EdgeDiscrim of int * Test * range
let getDiscrim (EdgeDiscrim(_,discrim,_)) = discrim


let CompilePatternBasic 
        g denv amap exprm matchm 
        warnOnUnused 
        warnOnIncomplete 
        actionOnFailure 
        (topv,topgtvs) 
        (clausesL: TypedMatchClause list)
        inputTy
        resultTy = 
    // Add the targets to a match builder 
    // Note the input expression has already been evaluated and saved into a variable.
    // Hence no need for a new sequence point.
    let mbuilder = new MatchBuilder(NoSequencePointAtInvisibleBinding,exprm)
    clausesL |> List.iteri (fun _i c -> mbuilder.AddTarget c.Target |> ignore) 
    
    // Add the incomplete or rethrow match clause on demand, printing a 
    // warning if necessary (only if it is ever exercised) 
    let incompleteMatchClauseOnce = ref None
    let getIncompleteMatchClause (refuted) = 
        // This is lazy because emit a 
        // warning when the lazy thunk gets evaluated 
        match !incompleteMatchClauseOnce with 
        | None -> 
                (* Emit the incomplete match warning *)               
                if warnOnIncomplete then 
                   match actionOnFailure with 
                   | ThrowIncompleteMatchException ->
                        warning (MatchIncomplete (false,ShowCounterExample g denv matchm refuted, matchm));
                   | IgnoreWithWarning ->
                        warning (MatchIncomplete (true,ShowCounterExample g denv matchm refuted, matchm));
                   | _ -> 
                        ()
                        
                let throwExpr =
                    match actionOnFailure with
                      | FailFilter  -> 
                          // Return 0 from the .NET exception filter
                          mkInt g matchm 0

                      | Rethrow     -> 
                          // Rethrow unmatched try-catch exn. No sequence point at the target since its not
                          // real code.
                          mkReraise matchm resultTy 
                      
                      | Throw       -> 
                          // We throw instead of rethrow on unmatched try-catch in a computation expression. But why?
                          // Because this isn't a real .NET exception filter/handler but just a function we're passing
                          // to a computation expression builder to simulate one.
                          mkThrow   matchm resultTy (exprForVal matchm topv) 
                          
                      | ThrowIncompleteMatchException  -> 
                          mkThrow   matchm resultTy 
                              (mkExnExpr(mk_MFCore_tcref g.fslibCcu "MatchFailureException", 
                                            [ mkString g matchm matchm.FileName; 
                                              mkInt g matchm matchm.StartLine; 
                                              mkInt g matchm matchm.StartColumn],matchm))

                      | IgnoreWithWarning  -> 
                          mkUnit g matchm

                // We don't emit a sequence point at any of the above cases because they don't correspond to 
                // user code. 
                //
                // Note we don't emit sequence points at either the succeeding or failing
                // targets of filters since if the exception is filtered successfully then we 
                // will run the handler and hit the sequence point there.
                // That sequence point will have the pattern variables bound, which is exactly what we want.
                let tg = TTarget(FlatList.empty,throwExpr,SuppressSequencePointAtTarget  )
                mbuilder.AddTarget tg |> ignore;
                let clause = TClause(TPat_wild matchm,None,tg,matchm)
                incompleteMatchClauseOnce := Some(clause);
                clause
                
        | Some c -> c

    // Helpers to get the variables bound at a target. We conceptually add a dummy clause that will always succeed with a "throw" 
    let clausesA = Array.ofList clausesL
    let nclauses = clausesA.Length
    let GetClause i refuted = 
        if i < nclauses then 
            clausesA.[i]  
        elif i = nclauses then getIncompleteMatchClause(refuted)
        else failwith "GetClause"
    let GetValsBoundByClause i refuted = (GetClause i refuted).BoundVals
    let GetWhenGuardOfClause i refuted = (GetClause i refuted).GuardExpr
    
    // Different uses of parameterized active patterns have different identities as far as paths 
    // are concerned. Here we generate unique numbers that are completely different to any stamp
    // by usig negative numbers.
    let genUniquePathId() = - (newUnique())

    // Build versions of these functions which apply a dummy instantiation to the overall type arguments 
    let GetSubExprOfInput,getDiscrimOfPattern = 
        let tyargs = List.map (fun _ -> g.unit_ty) topgtvs
        let unit_tpinst = mkTyparInst topgtvs tyargs
        GetSubExprOfInput g (topgtvs,tyargs,unit_tpinst),
        getDiscrimOfPattern g unit_tpinst

    // The main recursive loop of the pattern match compiler 
    let rec InvestigateFrontiers refuted frontiers = 
        if debug then dprintf "frontiers = %s\n" (String.concat ";" (List.map (getRuleIndex >> string) frontiers));
        match frontiers with
        | [] -> failwith "CompilePattern:compile - empty clauses: at least the final clause should always succeed"
        | (Frontier (i,active,valMap)) :: rest ->

            // Check to see if we've got a succeeding clause.  There may still be a 'when' condition for the clause 
            match active with
            | [] -> CompileSuccessPointAndGuard i refuted valMap rest 

            | _ -> 
                if debug then dprintf "Investigating based on rule %d, #active = %d\n" i (List.length active);
                (* Otherwise choose a point (i.e. a path) to investigate. *)
                let (Active(path,subexpr,pat))  = ChooseInvestigationPointLeftToRight frontiers
                match pat with
                // All these constructs should have been eliminated in BindProjectionPattern 
                | TPat_as _   | TPat_tuple _  | TPat_wild _      | TPat_disjs _  | TPat_conjs _  | TPat_recd _ -> failwith "Unexpected pattern"

                // Leaving the ones where we have real work to do 
                | _ -> 

                    if debug then dprintf "chooseSimultaneousEdgeSet\n";
                    let simulSetOfEdgeDiscrims,fallthroughPathFrontiers = ChooseSimultaneousEdges frontiers path

                    let resPreBindOpt, bindOpt =     ChoosePreBinder simulSetOfEdgeDiscrims subexpr    
                            
                    // For each case, recursively compile the residue decision trees that result if that case successfully matches 
                    let simulSetOfCases, _ = CompileSimultaneousSet frontiers path refuted subexpr simulSetOfEdgeDiscrims resPreBindOpt 
                          
                    assert (nonNil(simulSetOfCases));

                    if debug then 
                        dprintf "#fallthroughPathFrontiers = %d, #simulSetOfEdgeDiscrims = %d\n"  (List.length fallthroughPathFrontiers) (List.length simulSetOfEdgeDiscrims);
                        dprintf "Making cases for each discriminator...\n";
                        dprintf "#edges = %d\n" (List.length simulSetOfCases);
                        dprintf "Checking for completeness of edge set from earlier investigation of rule %d, #active = %d\n" i (List.length active);

                    // Work out what the default/fall-through tree looks like, is any 
                    // Check if match is complete, if so optimize the default case away. 
                
                    let defaultTreeOpt  : DecisionTree option = CompileFallThroughTree fallthroughPathFrontiers path refuted  simulSetOfCases

                    // OK, build the whole tree and whack on the binding if any 
                    let finalDecisionTree = 
                        let inpExprToSwitch = (match resPreBindOpt with Some vexp -> vexp | None -> GetSubExprOfInput subexpr)
                        let tree = BuildSwitch resPreBindOpt g inpExprToSwitch simulSetOfCases defaultTreeOpt matchm
                        match bindOpt with 
                        | None -> tree
                        | Some bind -> TDBind (bind,tree)
                        
                    finalDecisionTree

    and CompileSuccessPointAndGuard i refuted valMap rest =

        if debug then dprintf "generating success node for rule %d\n" i;
        let vs2 = GetValsBoundByClause i refuted
        let es2 = 
            vs2 |> FlatList.map (fun v -> 
                match valMap.TryFind v with 
                | None -> error(Error(FSComp.SR.patcMissingVariable(v.DisplayName),v.Range)) 
                | Some res -> res)
        let rhs' = TDSuccess(es2, i)
        match GetWhenGuardOfClause i refuted with 
        | Some whenExpr -> 
            if debug then dprintf "generating success node for rule %d, with 'when' clause\n" i;

            let m = whenExpr.Range

            // SEQUENCE POINTS: REVIEW: Build a sequence point at 'when' 
            let whenExpr = mkLetsFromBindings m (mkInvisibleFlatBindings vs2 es2) whenExpr

            // We must duplicate both the bindings and the guard expression to ensure uniqueness of bound variables.
            // This is because guards and bindings can end up being compiled multiple times when "or" patterns are used.
            //
            // let whenExpr = copyExpr g CloneAll whenExpr
            //
            // However, we are not allowed to copy expressions until type checking is complete, because this 
            // would lose recursive fixup points within the expressions (see FSharp 1.0 bug 4821).

            mkBoolSwitch m whenExpr rhs' (InvestigateFrontiers (RefutedWhenClause::refuted) rest)

        | None -> rhs' 

    /// Select the set of discriminators which we can handle in one test, or as a series of 
    /// iterated tests, e.g. in the case of TPat_isinst.  Ensure we only take at most one class of TPat_query(_) at a time. 
    /// Record the rule numbers so we know which rule the TPat_query cam from, so that when we project through 
    /// the frontier we only project the right rule. 
    and ChooseSimultaneousEdges frontiers path =
        if debug then dprintf "chooseSimultaneousEdgeSet\n";
        frontiers |> chooseSimultaneousEdgeSet None (fun prevOpt (Frontier (i',active',_)) -> 
              if isMemOfActives path active' then 
                  let p = lookupActive path active' |> snd
                  match getDiscrimOfPattern p with
                  | Some discrim -> 
                      if (match prevOpt with None -> true | Some (EdgeDiscrim(_,discrimPrev,_)) -> discrimsHaveSameSimultaneousClass g discrim discrimPrev) then (
                          if debug then dprintf "taking rule %d\n" i';
                          Some (EdgeDiscrim(i',discrim,p.Range)),true
                      ) else 
                          None,false
                                                        
                  | None -> 
                      None,true
              else 
                  None,true)

    and ChoosePreBinder simulSetOfEdgeDiscrims subexpr =
         match simulSetOfEdgeDiscrims with 
          // Very simple 'isinst' tests: put the result of 'isinst' in a local variable 
          //
          // That is, transform 
          //     'if istype e then ...unbox e .... ' 
          // into
          //     'let v = isinst e in .... if nonnull v then ...v .... ' 
          //
          // This is really an optimization that could be done more effectively in opt.fs
          // if we flowed a bit of information through 

          
         | EdgeDiscrim(_i',(Test.IsInst (_srcty,tgty)),m) :: _rest 
                    (* check we can use a simple 'isinst' instruction *)
                    when canUseTypeTestFast g tgty && isNil topgtvs ->

             let v,vexp = mkCompGenLocal m "typeTestResult" tgty
             if topv.IsMemberOrModuleBinding then 
                 AdjustValToTopVal v topv.ActualParent ValReprInfo.emptyValData;
             let argexp = GetSubExprOfInput subexpr
             let appexp = mkIsInst tgty argexp matchm
             Some(vexp),Some(mkInvisibleBind v appexp)

#if OPTIMIZE_LIST_MATCHING
         | [EdgeDiscrim(_, ListConsDiscrim g tinst,m); EdgeDiscrim(_, ListEmptyDiscrim g _, _)]
         | [EdgeDiscrim(_, ListEmptyDiscrim g _, _); EdgeDiscrim(_, ListConsDiscrim g tinst, m)]
         | [EdgeDiscrim(_, ListConsDiscrim g tinst, m)]
         | [EdgeDiscrim(_, ListEmptyDiscrim g tinst, m)]
                    (* check we can use a simple 'isinst' instruction *)
                    when isNil topgtvs ->

             let ucaseTy = (mkProvenUnionCaseTy g.cons_ucref tinst)
             let v,vexp = mkCompGenLocal m "unionTestResult" ucaseTy
             if topv.IsMemberOrModuleBinding then 
                 AdjustValToTopVal v topv.ActualParent ValReprInfo.emptyValData;
             let argexp = GetSubExprOfInput subexpr
             let appexp = mkIsInst ucaseTy argexp matchm
             Some vexp,Some (mkInvisibleBind v appexp)
#endif

         // Active pattern matches: create a variable to hold the results of executing the active pattern. 
         | (EdgeDiscrim(_,(Test.ActivePatternCase(pexp,resTys,_resPreBindOpt,_,apinfo)),m) :: _) ->
             if debug then dprintf "Building result var for active pattern...\n";
             
             if nonNil topgtvs then error(InternalError("Unexpected generalized type variables when compiling an active pattern",m));
             let rty = apinfo.ResultType g m resTys
             let v,vexp = mkCompGenLocal m "activePatternResult" rty
             if topv.IsMemberOrModuleBinding then 
                 AdjustValToTopVal v topv.ActualParent ValReprInfo.emptyValData;
             let argexp = GetSubExprOfInput subexpr
             let appexp = mkApps g ((pexp,tyOfExpr g pexp), [], [argexp],m)
             
             Some(vexp),Some(mkInvisibleBind v appexp)
          | _ -> None,None
                            

    and CompileSimultaneousSet frontiers path refuted subexpr simulSetOfEdgeDiscrims (resPreBindOpt: Expr option) =

        ([],simulSetOfEdgeDiscrims) ||> List.collectFold (fun taken (EdgeDiscrim(i',discrim,m)) -> 
             // Check to see if we've already collected the edge for this case, in which case skip it. 
             if List.exists (isDiscrimSubsumedBy g amap m discrim) taken  then 
                 // Skip this edge: it is refuted 
                 ([],taken) 
             else 
                 // Make a resVar to hold the results of the successful "proof" that a union value is
                 // a successful union case. That is, transform 
                 //     'match v with 
                 //        | A _ -> ... 
                 //        | B _ -> ...' 
                 // into
                 //     'match v with 
                 //        | A _ -> let vA = (v ~~> A)  in .... 
                 //        | B _ -> let vB = (v ~~> B)  in .... ' 
                 //
                 // Only do this for union cases that actually have some fields and with more than one case
                 let resPostBindOpt,ucaseBindOpt =
                     match discrim with 
                     | Test.UnionCase (ucref, tinst) when 
#if OPTIMIZE_LIST_MATCHING
                                                           isNone resPreBindOpt &&
#endif
                                                          (isNil topgtvs && 
                                                           not topv.IsMemberOrModuleBinding && 
                                                           ucref.UnionCase.RecdFields.Length >= 1 && 
                                                           ucref.Tycon.UnionCasesArray.Length > 1) ->

                       let v,vexp = mkCompGenLocal m "unionCase" (mkProvenUnionCaseTy ucref tinst)
                       let argexp = GetSubExprOfInput subexpr
                       let appexp = mkUnionCaseProof(argexp, ucref,tinst,m)
                       Some(vexp),Some(mkInvisibleBind v appexp)
                     | _ -> 
                       None,None
                 
                 // Convert active pattern edges to tests on results data 
                 let discrim' = 
                     match discrim with 
                     | Test.ActivePatternCase(_pexp,resTys,_apatVrefOpt,idx,apinfo) -> 
                         let aparity = apinfo.Names.Length
                         let total = apinfo.IsTotal
                         if not total && aparity > 1 then 
                             error(Error(FSComp.SR.patcPartialActivePatternsGenerateOneResult(),m));
                         
                         if not total then Test.UnionCase(mkSomeCase g,resTys)
                         elif aparity <= 1 then Test.Const(Const.Unit) 
                         else Test.UnionCase(mkChoiceCaseRef g m aparity idx,resTys) 
                     | _ -> discrim
                     
                 // Project a successful edge through the frontiers. 
                 let investigation = Investigation(i',discrim,path)

                 let frontiers = frontiers |> List.collect (GenerateNewFrontiersAfterSucccessfulInvestigation resPreBindOpt resPostBindOpt investigation) 
                 let tree = InvestigateFrontiers refuted frontiers
                 // Bind the resVar for the union case, if we have one
                 let tree = 
                     match ucaseBindOpt with 
                     | None -> tree
                     | Some bind -> TDBind (bind,tree)
                 // Return the edge 
                 let edge = TCase(discrim',tree)
                 [edge], (discrim :: taken) )

    and CompileFallThroughTree fallthroughPathFrontiers path refuted (simulSetOfCases: DecisionTreeCase list) =

        let simulSetOfDiscrims = simulSetOfCases |> List.map (fun c -> c.Discriminator)

        let isRefuted (Frontier (_i',active',_)) = 
            isMemOfActives path active' &&
            let p = lookupActive path active' |> snd
            match getDiscrimOfPattern p with 
            | Some(discrim) -> List.exists (isDiscrimSubsumedBy g amap exprm discrim) simulSetOfDiscrims 
            | None -> false

        match simulSetOfDiscrims with 
        | Test.Const (Const.Bool _b) :: _ when simulSetOfCases.Length = 2 ->  None
        | Test.Const (Const.Unit) :: _  ->  None
        | Test.UnionCase (ucref,_) :: _ when  simulSetOfCases.Length = ucref.TyconRef.UnionCasesArray.Length -> None                      
        | Test.ActivePatternCase _ :: _ -> error(InternalError("Test.ActivePatternCase should have been eliminated",matchm))
        | _ -> 
            let fallthroughPathFrontiers = List.filter (isRefuted >> not) fallthroughPathFrontiers
            
            (* Add to the refuted set *)
            let refuted = (RefutedInvestigation(path,simulSetOfDiscrims)) :: refuted
        
            if debug then dprintf "Edge set was incomplete. Compiling remaining cases\n";
            match fallthroughPathFrontiers with
            | [] -> 
                None
            | _ -> 
                Some(InvestigateFrontiers refuted fallthroughPathFrontiers)
          
    // Build a new frontier that represents the result of a successful investigation 
    // at rule point (i',discrim,path) 
    and GenerateNewFrontiersAfterSucccessfulInvestigation resPreBindOpt resPostBindOpt (Investigation(i',discrim,path)) (Frontier (i, active,valMap) as frontier) =
        if debug then dprintf "projecting success of investigation encompassing rule %d through rule %d \n" i' i;

        if (isMemOfActives path active) then
            let (SubExpr(accessf,ve)),pat = lookupActive path active
            if debug then dprintf "active...\n";

            let mkSubFrontiers path accessf' active' argpats pathBuilder = 
                let mkSubActive j p = 
                    let newSubExpr = SubExpr(accessf' j, ve)
                    let newPath = pathBuilder path j
                    Active(newPath, newSubExpr, p)
                let newActives = List.mapi mkSubActive argpats
                let investigations = BindProjectionPatterns newActives (active', valMap)
                mkFrontiers investigations i

            let active' = removeActive path active
            match pat with 
            | TPat_wild _ | TPat_as _ | TPat_tuple _ | TPat_disjs _ | TPat_conjs _ | TPat_recd _ -> failwith "Unexpected projection pattern"
            | TPat_query ((_,resTys,apatVrefOpt,idx,apinfo),p,m) -> 
            
                if apinfo.IsTotal then
                    let hasParam = (match apatVrefOpt with None -> true | Some (vref,_) -> doesActivePatternHaveFreeTypars g vref)
                    if (hasParam && i = i') || (discrimsEq g discrim (Option.get (getDiscrimOfPattern pat))) then
                        let aparity = apinfo.Names.Length
                        let accessf' j tpinst _e' = 
                            if aparity <= 1 then 
                                Option.get resPreBindOpt 
                            else
                                let ucref = mkChoiceCaseRef g m aparity idx
                                mkUnionCaseFieldGetUnproven(Option.get resPreBindOpt,ucref,instTypes tpinst resTys,j,exprm)
                        mkSubFrontiers path accessf' active' [p] (fun path j -> PathQuery(path,int64 j))

                    elif hasParam then

                        // Successful active patterns  don't refute other patterns
                        [frontier] 
                    else
                        []
                else 
                    if i = i' then
                            let accessf' _j tpinst _ =  
                                mkUnionCaseFieldGetUnproven(Option.get resPreBindOpt, mkSomeCase g, instTypes tpinst resTys, 0, exprm)
                            mkSubFrontiers path accessf' active' [p] (fun path j -> PathQuery(path,int64 j))
                    else 
                        // Successful active patterns  don't refute other patterns
                        [frontier]  

            | TPat_unioncase (ucref1, tyargs, argpats,_) -> 
                match discrim with 
                | Test.UnionCase (ucref2, tinst) when g.unionCaseRefEq ucref1 ucref2 ->
                    let accessf' j tpinst e' = 
#if OPTIMIZE_LIST_MATCHING
                        match resPreBindOpt with 
                        | Some e -> mkUnionCaseFieldGetProven(e,ucref1,tinst,j,exprm)
                        | None -> 
#endif
                        match resPostBindOpt with 
                        | Some e -> mkUnionCaseFieldGetProven(e,ucref1,tinst,j,exprm)
                        | None -> mkUnionCaseFieldGetUnproven(accessf tpinst e',ucref1,instTypes tpinst tyargs,j,exprm)
                        
                    mkSubFrontiers path accessf' active' argpats (fun path j -> PathUnionConstr(path,ucref1,tyargs,j))
                | Test.UnionCase _ ->
                    // Successful union case tests DO refute all other union case tests (no overlapping union cases)
                    []
                | _ -> 
                    // Successful union case tests don't refute any other patterns
                    [frontier]

            | TPat_array (argpats,ty,_) -> 
                match discrim with
                | Test.ArrayLength (n,_) when List.length argpats = n ->
                    let accessf' j tpinst e' = mkCallArrayGet g exprm ty (accessf tpinst e') (mkInt g exprm j)
                    mkSubFrontiers path accessf' active' argpats (fun path j -> PathArray(path,ty,List.length argpats,j))
                // Successful length tests refute all other lengths
                | Test.ArrayLength _ -> 
                    []
                | _ -> 
                    [frontier]

            | TPat_exnconstr (ecref, argpats,_) -> 
                match discrim with 
                | Test.IsInst (_srcTy,tgtTy) when typeEquiv g (mkAppTy ecref []) tgtTy ->
                    let accessf' j tpinst e' = mkExnCaseFieldGet(accessf tpinst e',ecref,j,exprm)
                    mkSubFrontiers path accessf' active' argpats (fun path j -> PathExnConstr(path,ecref,j))
                | _ -> 
                    // Successful type tests against one sealed type refute all other sealed types
                    // REVIEW: Successful type tests against one sealed type should refute all other sealed types
                    [frontier]

            | TPat_isinst (_srcty,tgtTy1,pbindOpt,_) -> 
                match discrim with 
                | Test.IsInst (_srcTy,tgtTy2) when typeEquiv g tgtTy1 tgtTy2  ->
                    match pbindOpt with 
                    | Some pbind -> 
                        let accessf' tpinst e' = 
                            // Fetch the result from the place where we saved it, if possible
                            match resPreBindOpt with 
                            | Some e -> e 
                            | _ -> 
                                // Otherwise call the helper
                               mkCallUnboxFast g exprm (instType tpinst tgtTy1) (accessf tpinst e')

                        let (v,e') =  BindSubExprOfInput g amap topgtvs pbind exprm (SubExpr(accessf',ve))
                        [Frontier (i, active', valMap.Add v e' )]
                    | None -> 
                        [Frontier (i, active', valMap)]
                    
                | _ ->
                    // Successful type tests against other types don't refute anything
                    // REVIEW: Successful type tests against one sealed type should refute all other sealed types
                    [frontier]

            | TPat_null _ -> 
                match discrim with 
                | Test.IsNull -> 
                    [Frontier (i, active',valMap)]
                | _ ->
                    // Successful null tests don't refute any other patterns 
                    [frontier]

            | TPat_const (c1,_) -> 
                match discrim with 
                | Test.Const c2 when (c1=c2) -> 
                    [Frontier (i, active',valMap)]
                | Test.Const _ -> 
                    // All constants refute all other constants (no overlapping between constants!)
                    []
                | _ ->
                    [frontier]

            | _ -> failwith "pattern compilation: GenerateNewFrontiersAfterSucccessfulInvestigation"
        else [frontier] 
        
    and BindProjectionPattern (Active(path,subExpr,p) as inp) ((accActive,accValMap) as s) = 
        let (SubExpr(accessf,ve)) = subExpr 
        let mkSubActive pathBuilder accessf'  j p'  = 
            Active(pathBuilder path j,SubExpr(accessf' j,ve),p')
            
        match p with 
        | TPat_wild _ -> 
            BindProjectionPatterns [] s 
        | TPat_as(p',pbind,m) -> 
            let (v,e') =  BindSubExprOfInput g amap topgtvs pbind m subExpr
            BindProjectionPattern (Active(path,subExpr,p')) (accActive,accValMap.Add v e' )
        | TPat_tuple(ps,tyargs,_m) ->
            let accessf' j tpinst e' = mkTupleFieldGet(accessf tpinst e',instTypes tpinst tyargs,j,exprm)
            let pathBuilder path j = PathTuple(path,tyargs,j)
            let newActives = List.mapi (mkSubActive pathBuilder accessf') ps
            BindProjectionPatterns newActives s 
        | TPat_recd(tcref,tinst,ps,_m) -> 
            let newActives = 
                (ps,tcref.TrueInstanceFieldsAsRefList) ||> List.mapi2 (fun j p fref -> 
                    let accessf' fref _j tpinst e' = mkRecdFieldGet g (accessf tpinst e',fref,instTypes tpinst tinst,exprm)
                    let pathBuilder path j = PathRecd(path,tcref,tinst,j)
                    mkSubActive pathBuilder (accessf' fref) j p) 
            BindProjectionPatterns newActives s 
        | TPat_disjs(ps,_m) -> 
            List.collect (fun p -> BindProjectionPattern (Active(path,subExpr,p)) s)  ps
        | TPat_conjs(ps,_m) -> 
            let newActives = List.mapi (mkSubActive (fun path j -> PathConj(path,j)) (fun _j -> accessf)) ps
            BindProjectionPatterns newActives s 
        
        | TPat_range (c1,c2,m) ->
            let res = ref []
            for i = int c1 to int c2 do
                res :=  BindProjectionPattern (Active(path,subExpr,TPat_const(Const.Char(char i),m))) s @ !res
            !res
        // Assign an identifier to each TPat_query based on our knowledge of the 'identity' of the active pattern, if any 
        | TPat_query ((_,_,apatVrefOpt,_,_),_,_) -> 
            let uniqId = 
                match apatVrefOpt with 
                | Some (vref,_) when not (doesActivePatternHaveFreeTypars g vref) -> vref.Stamp 
                | _ -> genUniquePathId() 
            let inp = Active(PathQuery(path,uniqId),subExpr,p) 
            [(inp::accActive, accValMap)] 
        | _ -> 
            [(inp::accActive, accValMap)] 

    and BindProjectionPatterns ps s =
        List.foldBack (fun p sofar -> List.collect (BindProjectionPattern p) sofar) ps [s] 

    (* The setup routine of the match compiler *)
    let frontiers = 
        ((clausesL 
          |> List.mapi (fun i c -> 
                let initialSubExpr = SubExpr((fun _tpinst x -> x),(exprForVal topv.Range topv,topv))
                let investigations = BindProjectionPattern (Active(PathEmpty(inputTy),initialSubExpr,c.Pattern)) ([],ValMap<_>.Empty)
                mkFrontiers investigations i) 
          |> List.concat)
          @ 
          mkFrontiers [([],ValMap<_>.Empty)] nclauses)
    let dtree = 
      InvestigateFrontiers
        []
        frontiers

    let targets = mbuilder.CloseTargets()

    
    // Report unused targets 
    if warnOnUnused then 
        let used = accTargetsOfDecisionTree dtree [] |> Hashset.ofList

        clausesL |> List.iteri (fun i c ->  
            if not (used.ContainsKey i) then warning (RuleNeverMatched c.Range)) 

    dtree,targets
  
let isPartialOrWhenClause (c:TypedMatchClause) = isPatternPartial c.Pattern || c.GuardExpr.IsSome


let rec CompilePattern  g denv amap exprm matchm warnOnUnused actionOnFailure (topv,topgtvs) (clausesL: TypedMatchClause list) inputTy resultTy =
  match clausesL with 
  | _ when List.exists isPartialOrWhenClause clausesL ->
        // Partial clauses cause major code explosion if treated naively 
        // Hence treat any pattern matches with any partial clauses clause-by-clause 
        
        // First make sure we generate at least some of the obvious incomplete match warnings. 
        let warnOnUnused = false in (* we can't turn this on since we're pretending all partial's fail in order to control the complexity of this. *)
        let warnOnIncomplete = true
        let clausesPretendAllPartialFail = List.collect (fun (TClause(p,whenOpt,tg,m)) -> [TClause(erasePartialPatterns p,whenOpt,tg,m)]) clausesL
        let _ = CompilePatternBasic g denv amap exprm matchm warnOnUnused warnOnIncomplete actionOnFailure (topv,topgtvs) clausesPretendAllPartialFail inputTy resultTy
        let warnOnIncomplete = false
        
        let rec atMostOnePartialAtATime clauses = 
            if debug then dprintf "atMostOnePartialAtATime: #clauses = %A\n" clauses;
            match List.takeUntil isPartialOrWhenClause clauses with 
            | l,[]       -> 
                CompilePatternBasic g denv amap exprm matchm warnOnUnused warnOnIncomplete actionOnFailure (topv,topgtvs) l inputTy resultTy
            | l,(h :: t) -> 
                // Add the partial clause 
                doGroupWithAtMostOnePartial (l @ [h]) t

        and doGroupWithAtMostOnePartial group rest = 
            if debug then dprintf "doGroupWithAtMostOnePartial: #group = %A\n" group;

            // Compile the remaining clauses
            let dtree,targets = atMostOnePartialAtATime rest

            // Make the expression that represents the remaining cases of the pattern match
            let expr = mkAndSimplifyMatch NoSequencePointAtInvisibleBinding exprm matchm resultTy dtree targets
            
            // If the remainder of the match boiled away to nothing interesting.
            // We measure this simply by seeing if the range of the resulting expression is identical to matchm.
            let spTarget = 
                if expr.Range = matchm then SuppressSequencePointAtTarget 
                else SequencePointAtTarget

            // Make the clause that represents the remaining cases of the pattern match
            let clauseForRestOfMatch = TClause(TPat_wild matchm,None,TTarget(FlatList.empty,expr,spTarget),matchm)
            
            CompilePatternBasic 
                 g denv amap exprm matchm warnOnUnused warnOnIncomplete actionOnFailure (topv,topgtvs) 
                 (group @ [clauseForRestOfMatch]) inputTy resultTy
        

        atMostOnePartialAtATime clausesL
      
  | _ -> 
      CompilePatternBasic g denv amap exprm matchm warnOnUnused true actionOnFailure (topv,topgtvs) (clausesL: TypedMatchClause list) inputTy resultTy
