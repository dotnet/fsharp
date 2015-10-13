// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.AbstractIL.Morphs 

open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.Types 
open Microsoft.FSharp.Compiler.AbstractIL.IL 

type 'T morph = 'T -> 'T

type EnclosingTypeDefs = ILTypeDef list * ILTypeDef

let checking = false 
let notlazy v = Lazy.CreateFromValue v

// REVIEW: Consider removing Post-Dev11 M3
let mutable morphCustomAttributeData = false

let enablemorphCustomAttributeData() = 
    morphCustomAttributeData <- true

let disablemorphCustomAttributeData() =
    morphCustomAttributeData <- false

let mdef_code2code f md  =
    let code = 
        match md.mdBody.Contents with 
        | MethodBody.IL il-> il 
        | _ -> failwith "mdef_code2code - method not IL"  
    let code' = MethodBody.IL {code with Code = f code.Code} 
    {md with mdBody=  mkMethBodyAux code'}  

let code_block2block f (c:ILCode) = checkILCode (f c)

let bblock_instr2instr f bb = 
    let instrs = bb.Instructions 
    let len = Array.length instrs 
    let res = Array.zeroCreate len 
    for i = 0 to len - 1 do 
        res.[i] <- f instrs.[i]
    {bb with Instructions=res}

// This is quite performance critical 
let nonNil x = match x with [] -> false | _ -> true
let bblock_instr2instrs f bb = 
  let instrs = bb.Instructions 
  let codebuf = ref (Array.zeroCreate (Array.length instrs)) 
  let codebuf_size = ref 0 
  for i = 0 to Array.length instrs - 1 do 
    let instr = instrs.[i] 
    let instrs = f instr 
    let curr = ref instrs 
    while nonNil !curr do
        match !curr with 
        | instr2::t ->  
            let sz = !codebuf_size 
            let old_buf_size = Array.length !codebuf 
            let new_size = sz + 1 
            if new_size > old_buf_size then begin
              let old = !codebuf 
              let new' = Array.zeroCreate (max new_size (old_buf_size * 4)) 
              Array.blit old 0 new' 0 sz;
              codebuf := new';
            end;
            (!codebuf).[sz] <- instr2;
            incr codebuf_size;
            curr := t;
        | [] -> ()
  {bb with Instructions = Array.sub !codebuf 0 !codebuf_size}

// Map each instruction in a basic block to a more complicated block that 
// may involve internal branching, but which will still have one entry 
// label and one exit label. This is used, for example, when macro-expanding 
// complicated high-level ILX instructions. 
// The morphing function is told the name of the input and output labels 
// that must be used for the generated block. 
// Optimize the case where an instruction gets mapped to a 
// straightline sequence of instructions by allowing the morphing 
// function to return a special result for this case. 
// 
// Let [i] be the instruction being morphed.  If [i] is a control-flow 
// then instruction then [f] must return either a control-flow terminated 
// sequence of instructions or a block both of which must targets the same labels 
// (or a subset of the labels) targeted in [i].  If [i] 
// is not a if not a control-flow instruction then [f] 
// must return a block targeting the given output label. 

let rec countAccInstrs (xss:ILInstr list list) acc = 
    match xss with 
    | [] -> acc
    | xs :: rest -> countAccInstrs rest (acc + List.length xs)

let rec commitAccInstrsAux (xs:ILInstr list) (arr:ILInstr[]) i = 
    match xs with 
    | [] -> ()
    | x :: rest -> arr.[i] <- x; commitAccInstrsAux rest arr (i+1)
    
// Fill in the array chunk by chunk from the end and work backwards
let rec commitAccInstrs xss arr i = 
    match xss with 
    | [] -> assert (i = 0)
    | xs :: rest -> 
        let n = List.length xs
        commitAccInstrsAux xs arr (i - n)
        commitAccInstrs rest arr (i - n)
    
// Write the accumulated instructions into an array. The fragments come in in reverse order.
let commitAccBasicBlock (sofar: ILInstr list list) = 
    let n = countAccInstrs sofar 0
    let arr = Array.zeroCreate n 
    commitAccInstrs sofar arr n
    arr

[<Struct; NoComparison; NoEquality>]
type InstrMorph(isInstrs:bool, instrs:ILInstr list, code: ILCode) = 
    new (instrs:ILInstr list) = InstrMorph(true,instrs,Unchecked.defaultof<_>)
    new (code:ILCode) = InstrMorph(false,Unchecked.defaultof<_>,code)
    member x.IsInstrs = isInstrs
    member x.Instrs = instrs
    member x.Code = code
    
let rec bblockLoop f bb currBBlockInpLabel currInpLabel currOutLabel sofar instrs = 
    match instrs with 
    | (i::rest) -> 
        let res : InstrMorph = f currInpLabel currOutLabel i 
        if res.IsInstrs then
          // First possibility: return a list of instructions.  No addresses get consumed. 
            bblockLoop f bb currBBlockInpLabel currInpLabel currOutLabel (res.Instrs :: sofar) rest
        else
          let middle_bblock = res.Code
          let before_bblock = 
            let instrs = commitAccBasicBlock ([I_br currInpLabel] :: sofar) 
            mkBasicBlock {Label=currBBlockInpLabel;Instructions=instrs} 
          if checking && uniqueEntryOfCode middle_bblock <> currInpLabel then 
            dprintn ("*** warning when transforming bblock "^formatCodeLabel bb.Label^": bblock2code_instr2code: input label of returned block does not match the expected label while converting an instruction to a block.");
          let afterBlocks = 
              match rest with 
              | [] -> [] // the bblock has already been transformed 
              | _ -> 
                  let newInLab = generateCodeLabel () 
                  let newOutLab = generateCodeLabel () 
                  [ bblockLoop f bb currOutLabel newInLab newOutLab [] rest ]
           
          checkILCode 
              (mkGroupBlock 
                 ( currInpLabel :: (match rest with [] -> [] | _ -> [ currOutLabel ]),
                  before_bblock ::  middle_bblock :: afterBlocks))
    | [] -> 
       let instrs = commitAccBasicBlock sofar 
       mkBasicBlock {Label=currBBlockInpLabel;Instructions=instrs} 

let bblock2code_instr2code (f:ILCodeLabel -> ILCodeLabel -> ILInstr -> InstrMorph) bb = 
    bblockLoop f bb bb.Label (generateCodeLabel ()) (generateCodeLabel ()) [] (Array.toList bb.Instructions)

let rec block_bblock2code_typ2typ ((fbb,fty) as f) x =
    match x with
    | ILBasicBlock bblock -> fbb bblock
    | GroupBlock (locs,l) -> GroupBlock(locs,List.map (code_bblock2code_typ2typ f) l)
    | TryBlock (tryb,seh) ->
        let seh = 
            match seh with 
            | FaultBlock b -> FaultBlock (code_bblock2code_typ2typ f  b)
            | FinallyBlock b -> FinallyBlock (code_bblock2code_typ2typ f  b)
            | FilterCatchBlock clsl -> 
                FilterCatchBlock 
                  (List.map (fun (flt,ctch) -> 
                    (match flt with 
                      CodeFilter fltcode -> CodeFilter (code_bblock2code_typ2typ f fltcode)
                    | TypeFilter ty -> TypeFilter (fty ty)), 
                    code_bblock2code_typ2typ f ctch) clsl)
        TryBlock (code_bblock2code_typ2typ f tryb,seh)
    | RestrictBlock (ls,c) -> RestrictBlock (ls,code_bblock2code_typ2typ f c)

and code_bblock2code_typ2typ f (c:ILCode) = checkILCode (block_bblock2code_typ2typ f c)
let topcode_bblock2code_typ2typ f (c:ILCode) = code_bblock2code_typ2typ f c

let rec block_bblock2code f x =
    match x with
    | ILBasicBlock bblock -> f bblock
    | GroupBlock (locs,l) -> GroupBlock(locs,List.map (code_bblock2code f) l)
    | TryBlock (tryb,seh) ->
        TryBlock (code_bblock2code f tryb,
                  begin match seh with 
                  | FaultBlock b -> FaultBlock (code_bblock2code f  b)
                  | FinallyBlock b -> FinallyBlock (code_bblock2code f  b)
                  | FilterCatchBlock clsl -> 
                      FilterCatchBlock 
                        (List.map (fun (flt,ctch) -> 
                          (match flt with 
                           |CodeFilter fltcode -> CodeFilter (code_bblock2code f fltcode)
                           | TypeFilter _ty -> flt), 
                          code_bblock2code f ctch) clsl)
                  end)
    | RestrictBlock (ls,c) -> RestrictBlock (ls,code_bblock2code f c)

and code_bblock2code f (c:ILCode) = checkILCode (block_bblock2code f c)
let topcode_bblock2code f (c:ILCode) = code_bblock2code f c

// --------------------------------------------------------------------
// Standard morphisms - mapping types etc.
// -------------------------------------------------------------------- 

let rec typ_tref2tref f x  = 
    match x with 
    | ILType.Ptr t -> ILType.Ptr (typ_tref2tref f t)
    | ILType.FunctionPointer x -> 
        ILType.FunctionPointer
          { x with 
                ArgTypes=ILList.map (typ_tref2tref f) x.ArgTypes;
                ReturnType=typ_tref2tref f x.ReturnType}
    | ILType.Byref t -> ILType.Byref (typ_tref2tref f t)
    | ILType.Boxed cr -> mkILBoxedType (tspec_tref2tref f cr)
    | ILType.Value ir -> ILType.Value (tspec_tref2tref f ir)
    | ILType.Array (s,ty) -> ILType.Array (s,typ_tref2tref f ty)
    | ILType.TypeVar v ->  ILType.TypeVar v 
    | ILType.Modified (req,tref,ty) ->  ILType.Modified (req, f tref, typ_tref2tref f ty) 
    | ILType.Void -> ILType.Void
and tspec_tref2tref f (x:ILTypeSpec) = 
    mkILTySpecRaw(f x.TypeRef, ILList.map (typ_tref2tref f) x.GenericArgs)

let rec typ_scoref2scoref_tyvar2typ ((_fscope,ftyvar) as fs)x  = 
    match x with 
    | ILType.Ptr t -> ILType.Ptr (typ_scoref2scoref_tyvar2typ fs t)
    | ILType.FunctionPointer t -> ILType.FunctionPointer (callsig_scoref2scoref_tyvar2typ fs t)
    | ILType.Byref t -> ILType.Byref (typ_scoref2scoref_tyvar2typ fs t)
    | ILType.Boxed cr -> mkILBoxedType (tspec_scoref2scoref_tyvar2typ fs cr)
    | ILType.Value ir -> ILType.Value (tspec_scoref2scoref_tyvar2typ fs ir)
    | ILType.Array (s,ty) -> ILType.Array (s,typ_scoref2scoref_tyvar2typ fs ty)
    | ILType.TypeVar v ->  ftyvar v
    | x -> x
and tspec_scoref2scoref_tyvar2typ fs (x:ILTypeSpec) = 
    ILTypeSpec.Create(morphILScopeRefsInILTypeRef (fst fs) x.TypeRef,typs_scoref2scoref_tyvar2typ fs x.GenericArgs)
and callsig_scoref2scoref_tyvar2typ f x = 
    { x with 
          ArgTypes=ILList.map (typ_scoref2scoref_tyvar2typ f) x.ArgTypes;
          ReturnType=typ_scoref2scoref_tyvar2typ f x.ReturnType}
and typs_scoref2scoref_tyvar2typ f i = ILList.map (typ_scoref2scoref_tyvar2typ f) i
and gparams_scoref2scoref_tyvar2typ f i = List.map (gparam_scoref2scoref_tyvar2typ f) i
and gparam_scoref2scoref_tyvar2typ _f i = i
and morphILScopeRefsInILTypeRef fscope (x:ILTypeRef) = 
    ILTypeRef.Create(scope=fscope x.Scope, enclosing=x.Enclosing, name = x.Name)


let callsig_typ2typ f (x: ILCallingSignature) = 
    { CallingConv=x.CallingConv;
      ArgTypes=ILList.map f x.ArgTypes;
      ReturnType=f x.ReturnType}

let gparam_typ2typ f gf = {gf with Constraints = ILList.map f gf.Constraints}
let gparams_typ2typ f gfs = List.map (gparam_typ2typ f) gfs
let typs_typ2typ (f: ILType -> ILType)  x = ILList.map f x
let mref_typ2typ (f: ILType -> ILType) (x:ILMethodRef) = 
    ILMethodRef.Create(enclosingTypeRef= (f (mkILBoxedType (mkILNonGenericTySpec x.EnclosingTypeRef))).TypeRef,
                       callingConv=x.CallingConv,
                       name=x.Name,
                       genericArity=x.GenericArity,
                       argTypes= ILList.map f x.ArgTypes,
                       returnType= f x.ReturnType)


type formal_scopeCtxt =  Choice<ILMethodSpec, ILFieldSpec, IlxUnionSpec>

let mspec_typ2typ (((factualty : ILType -> ILType) , (fformalty: formal_scopeCtxt -> ILType -> ILType))) (x: ILMethodSpec) = 
    mkILMethSpecForMethRefInTyRaw(mref_typ2typ (fformalty (Choice1Of3 x)) x.MethodRef,
                                  factualty x.EnclosingType, 
                                  typs_typ2typ factualty  x.GenericArgs)

let fref_typ2typ (f: ILType -> ILType) x = 
    { x with EnclosingTypeRef = (f (mkILBoxedType (mkILNonGenericTySpec x.EnclosingTypeRef))).TypeRef;
             Type= f x.Type }

let fspec_typ2typ ((factualty,(fformalty : formal_scopeCtxt -> ILType -> ILType))) x = 
    { FieldRef=fref_typ2typ (fformalty (Choice2Of3 x)) x.FieldRef;
      EnclosingType= factualty x.EnclosingType }

let rec celem_typ2typ f celem =
    match celem with
    | ILAttribElem.Type (Some ty)    -> ILAttribElem.Type (Some (f ty))
    | ILAttribElem.TypeRef (Some tref) -> ILAttribElem.TypeRef (Some (f (mkILBoxedType (mkILNonGenericTySpec tref))).TypeRef)
    | ILAttribElem.Array (elemTy,elems) -> ILAttribElem.Array (f elemTy, List.map (celem_typ2typ f) elems)
    | _ -> celem

let cnamedarg_typ2typ f ((nm, ty, isProp, elem) : ILAttributeNamedArg)  =
    (nm, f ty, isProp, celem_typ2typ f elem) 

let cattr_typ2typ ilg f c =
    let meth = mspec_typ2typ (f, (fun _ -> f)) c.Method
    // dev11 M3 defensive coding: if anything goes wrong with attribute decoding or encoding, then back out.
    if morphCustomAttributeData then
        try 
           let elems,namedArgs = IL.decodeILAttribData ilg c 
           let elems = elems |> List.map (celem_typ2typ f)
           let namedArgs = namedArgs |> List.map (cnamedarg_typ2typ f)
           IL.mkILCustomAttribMethRef ilg (meth, elems, namedArgs)       
        with _ -> 
           { c with Method = meth }
    else
        { c with Method = meth }


let cattrs_typ2typ ilg f (cs: ILAttributes) =
    mkILCustomAttrs (List.map (cattr_typ2typ ilg f) cs.AsList)

let fdef_typ2typ ilg ftype (fd: ILFieldDef) = 
    {fd with Type=ftype fd.Type; 
             CustomAttrs=cattrs_typ2typ ilg ftype fd.CustomAttrs}
let altfdef_typ2typ ilg ftype (fd: IlxUnionField) = 
    IlxUnionField( fdef_typ2typ ilg ftype fd.ILField)

let alts_typ2typ ilg f alts = 
  Array.map (fun alt -> { alt with altFields = Array.map (altfdef_typ2typ ilg f)  alt.altFields;
                                   altCustomAttrs = cattrs_typ2typ ilg f alt.altCustomAttrs }) alts

let curef_typ2typ ilg f (IlxUnionRef(s,alts,nullPermitted,helpers)) =
  IlxUnionRef(s,alts_typ2typ ilg f alts,nullPermitted,helpers)

let local_typ2typ f (l: ILLocal) = {l with Type = f l.Type}
let freevar_typ2typ f l = {l with fvType = f l.fvType}
let varargs_typ2typ f (varargs: ILVarArgs) = Option.map (ILList.map f) varargs
(* REVIEW: convert varargs *)
let morphILTypesInILInstr ((factualty,fformalty)) i = 
    let factualty = factualty (Some i) 
    let conv_fspec fr = fspec_typ2typ (factualty,fformalty (Some i)) fr 
    let conv_mspec mr = mspec_typ2typ (factualty,fformalty (Some i)) mr 
    match i with 
    | I_calli (a,mref,varargs) ->  I_calli (a,callsig_typ2typ (factualty) mref,varargs_typ2typ factualty varargs)
    | I_call (a,mr,varargs) ->  I_call (a,conv_mspec mr,varargs_typ2typ factualty varargs)
    | I_callvirt (a,mr,varargs) ->   I_callvirt (a,conv_mspec mr,varargs_typ2typ factualty varargs)
    | I_callconstraint (a,ty,mr,varargs) ->   I_callconstraint (a,factualty ty,conv_mspec mr,varargs_typ2typ factualty varargs)
    | I_newobj (mr,varargs) ->  I_newobj (conv_mspec mr,varargs_typ2typ factualty varargs)
    | I_ldftn mr ->  I_ldftn (conv_mspec mr)
    | I_ldvirtftn mr ->  I_ldvirtftn (conv_mspec mr)
    | I_ldfld (a,b,fr) ->  I_ldfld (a,b,conv_fspec fr)
    | I_ldsfld (a,fr) ->  I_ldsfld (a,conv_fspec fr)
    | I_ldsflda (fr) ->  I_ldsflda (conv_fspec fr)
    | I_ldflda fr ->  I_ldflda (conv_fspec fr)
    | I_stfld (a,b,fr) -> I_stfld (a,b,conv_fspec fr)
    | I_stsfld (a,fr) -> I_stsfld (a,conv_fspec fr)
    | I_castclass typ -> I_castclass (factualty typ)
    | I_isinst typ -> I_isinst (factualty typ)
    | I_initobj typ -> I_initobj (factualty typ)
    | I_cpobj typ -> I_cpobj (factualty typ)
    | I_stobj (al,vol,typ) -> I_stobj (al,vol,factualty typ)
    | I_ldobj (al,vol,typ) -> I_ldobj (al,vol,factualty typ)
    | I_box typ -> I_box (factualty typ)
    | I_unbox typ -> I_unbox (factualty typ)
    | I_unbox_any typ -> I_unbox_any (factualty typ)
    | I_ldelem_any (shape,typ) ->  I_ldelem_any (shape,factualty typ)
    | I_stelem_any (shape,typ) ->  I_stelem_any (shape,factualty typ)
    | I_newarr (shape,typ) ->  I_newarr (shape,factualty typ)
    | I_ldelema (ro,isNativePtr,shape,typ) ->  I_ldelema (ro,isNativePtr,shape,factualty typ)
    | I_sizeof typ ->  I_sizeof (factualty typ)
    | I_ldtoken tok -> 
        match tok with 
        | ILToken.ILType typ ->   I_ldtoken (ILToken.ILType (factualty typ))
        | ILToken.ILMethod mr -> I_ldtoken (ILToken.ILMethod (conv_mspec mr))
        | ILToken.ILField fr -> I_ldtoken (ILToken.ILField (conv_fspec fr))
    | x -> x

let return_typ2typ ilg f (r:ILReturn) = {r with Type=f r.Type; CustomAttrs=cattrs_typ2typ ilg f r.CustomAttrs}
let param_typ2typ ilg f (p: ILParameter) = {p with Type=f p.Type; CustomAttrs=cattrs_typ2typ ilg f p.CustomAttrs}

let morphILMethodDefs f (m:ILMethodDefs) = mkILMethods (List.map f m.AsList)
let fdefs_fdef2fdef f (m:ILFieldDefs) = mkILFields (List.map f m.AsList)

(* use this when the conversion produces just one type... *)
let morphILTypeDefs f (m: ILTypeDefs) = mkILTypeDefs (List.map f m.AsList)

let morphExpandILTypeDefs f (m:ILTypeDefs) = 
  mkILTypeDefs (List.foldBack (fun x y -> f x @ y) m.AsList [])

let morphILTypeDefsInILModule typesf m = 
    {m with TypeDefs=typesf m.TypeDefs}

let locals_typ2typ f ls = ILList.map (local_typ2typ f) ls
let freevars_typ2typ f ls = Array.map (freevar_typ2typ f) ls

let ilmbody_bblock2code_typ2typ_maxstack2maxstack fs il = 
    let (finstr,ftype,fmaxstack) = fs 
    {il with Code=topcode_bblock2code_typ2typ (finstr,ftype) il.Code;
             Locals = locals_typ2typ ftype il.Locals;
             MaxStack = fmaxstack il.MaxStack }

let morphILMethodBody (filmbody) (x: ILLazyMethodBody) = 
    let c = 
        match x.Contents with
        | MethodBody.IL il -> MethodBody.IL (filmbody il)
        | x -> x
    mkMethBodyAux c

let ospec_typ2typ f (OverridesSpec(mref,ty)) = OverridesSpec(mref_typ2typ f mref, f ty)

let mdef_typ2typ_ilmbody2ilmbody ilg fs md  = 
    let (ftype,filmbody) = fs 
    let ftype' = ftype (Some md) 
    let body' = morphILMethodBody (filmbody (Some md))  md.mdBody 
    {md with 
      GenericParams=gparams_typ2typ ftype' md.GenericParams;
      mdBody= body';
      Parameters = ILList.map (param_typ2typ ilg ftype') md.Parameters;
      Return = return_typ2typ ilg ftype' md.Return;
      CustomAttrs=cattrs_typ2typ ilg ftype' md.CustomAttrs }

let fdefs_typ2typ ilg f x = fdefs_fdef2fdef (fdef_typ2typ ilg f) x

let mdefs_typ2typ_ilmbody2ilmbody ilg fs x = morphILMethodDefs (mdef_typ2typ_ilmbody2ilmbody ilg fs) x

let cuinfo_typ2typ ilg ftype cud = 
    { cud with cudAlternatives = alts_typ2typ ilg ftype cud.cudAlternatives; } 


let cloinfo_typ2typ_ilmbody2ilmbody fs clo = 
    let (ftype,filmbody) = fs 
    let c' = filmbody None (Lazy.force clo.cloCode) 
    { clo with cloFreeVars = freevars_typ2typ ftype clo.cloFreeVars;
               cloCode=notlazy c' }

let morphIlxClosureInfo f clo = 
    let c' = f (Lazy.force clo.cloCode) 
    { clo with cloCode=notlazy c' }

let mimpl_typ2typ f e =
    { Overrides = ospec_typ2typ f e.Overrides;
      OverrideBy = mspec_typ2typ (f,(fun _ -> f)) e.OverrideBy; }

let edef_typ2typ ilg f e =
    { e with
        Type = Option.map f e.Type;
        AddMethod = mref_typ2typ f e.AddMethod;
        RemoveMethod = mref_typ2typ f e.RemoveMethod;
        FireMethod = Option.map (mref_typ2typ f) e.FireMethod;
        OtherMethods = List.map (mref_typ2typ f) e.OtherMethods;
        CustomAttrs = cattrs_typ2typ ilg f e.CustomAttrs }

let pdef_typ2typ ilg f p =
    { p with
        SetMethod = Option.map (mref_typ2typ f) p.SetMethod;
        GetMethod = Option.map (mref_typ2typ f) p.GetMethod;
        Type = f p.Type;
        Args = ILList.map f p.Args;
        CustomAttrs = cattrs_typ2typ ilg f p.CustomAttrs }

let pdefs_typ2typ ilg f (pdefs: ILPropertyDefs) = mkILProperties (List.map (pdef_typ2typ ilg f) pdefs.AsList)
let edefs_typ2typ ilg f (edefs: ILEventDefs) = mkILEvents (List.map (edef_typ2typ ilg f) edefs.AsList)

let mimpls_typ2typ f (mimpls : ILMethodImplDefs) = mkILMethodImpls (List.map (mimpl_typ2typ f) mimpls.AsList)

let rec tdef_typ2typ_ilmbody2ilmbody_mdefs2mdefs ilg enc fs td = 
   let (ftype,filmbody,fmdefs) = fs 
   let ftype' = ftype (Some (enc,td)) None 
   let mdefs' = fmdefs (enc,td) td.Methods 
   let fdefs' = fdefs_typ2typ ilg ftype' td.Fields 
   {td with Implements= ILList.map ftype' td.Implements;
            GenericParams= gparams_typ2typ ftype' td.GenericParams; 
            Extends = Option.map ftype' td.Extends;
            Methods=mdefs';
            NestedTypes=tdefs_typ2typ_ilmbody2ilmbody_mdefs2mdefs ilg (enc@[td]) fs td.NestedTypes;
            Fields=fdefs';
            MethodImpls = mimpls_typ2typ ftype' td.MethodImpls;
            Events = edefs_typ2typ ilg ftype' td.Events; 
            Properties = pdefs_typ2typ ilg ftype' td.Properties;
            CustomAttrs = cattrs_typ2typ ilg ftype' td.CustomAttrs;
            tdKind =
               match td.tdKind with
               | ILTypeDefKind.Other e when isIlxExtTypeDefKind e -> 
                   match destIlxExtTypeDefKind e with 
                   | IlxTypeDefKind.Closure i -> mkIlxTypeDefKind (IlxTypeDefKind.Closure (cloinfo_typ2typ_ilmbody2ilmbody (ftype',filmbody (enc,td)) i))
                   | IlxTypeDefKind.Union i -> mkIlxTypeDefKind (IlxTypeDefKind.Union (cuinfo_typ2typ ilg ftype' i))
               | _ -> td.tdKind
  }

and tdefs_typ2typ_ilmbody2ilmbody_mdefs2mdefs ilg enc fs tdefs = 
  morphILTypeDefs (tdef_typ2typ_ilmbody2ilmbody_mdefs2mdefs ilg enc fs) tdefs

// --------------------------------------------------------------------
// Derived versions of the above, e.g. with defaults added
// -------------------------------------------------------------------- 

let manifest_typ2typ ilg f (m : ILAssemblyManifest) =
    { m with CustomAttrs = cattrs_typ2typ ilg f m.CustomAttrs }

let morphILTypeInILModule_ilmbody2ilmbody_mdefs2mdefs ilg
    ((ftype: ILModuleDef -> (ILTypeDef list * ILTypeDef) option -> ILMethodDef option -> ILType -> ILType),
     (filmbody: ILModuleDef -> ILTypeDef list * ILTypeDef -> ILMethodDef option -> ILMethodBody -> ILMethodBody),
     fmdefs) m = 

    let ftdefs = 
      tdefs_typ2typ_ilmbody2ilmbody_mdefs2mdefs ilg []
        (ftype m,
         filmbody m,
         fmdefs m) 

    { m with TypeDefs=ftdefs m.TypeDefs;
             CustomAttrs=cattrs_typ2typ ilg (ftype m None None) m.CustomAttrs;
             Manifest=Option.map (manifest_typ2typ ilg (ftype m None None)) m.Manifest  }
    
let module_bblock2code_typ2typ_maxstack2maxstack ilg fs x = 
    let (fbblock,ftype,fmaxstack) = fs 
    let filmbody modCtxt tdefCtxt mdefCtxt =
      ilmbody_bblock2code_typ2typ_maxstack2maxstack 
        (fbblock modCtxt tdefCtxt mdefCtxt, 
         ftype modCtxt (Some tdefCtxt) mdefCtxt,
         fmaxstack modCtxt tdefCtxt mdefCtxt) 
    let fmdefs modCtxt tdefCtxt = mdefs_typ2typ_ilmbody2ilmbody ilg (ftype modCtxt (Some tdefCtxt), filmbody modCtxt tdefCtxt) 
    morphILTypeInILModule_ilmbody2ilmbody_mdefs2mdefs ilg (ftype, filmbody, fmdefs) x 

let module_bblock2code_typ2typ ilg (f1,f2) x = 
  module_bblock2code_typ2typ_maxstack2maxstack ilg (f1, f2, (fun _modCtxt _tdefCtxt _mdefCtxt x -> x)) x
let morphILInstrsAndILTypesInILModule ilg (f1,f2) x = 
  module_bblock2code_typ2typ ilg ((fun modCtxt tdefCtxt mdefCtxt  i -> mkBasicBlock (bblock_instr2instr (f1 modCtxt tdefCtxt mdefCtxt) i)), f2) x

let morphILInstrsInILCode f x = topcode_bblock2code (fun i -> mkBasicBlock (bblock_instr2instrs f i)) x
let morphExpandILInstrsInILCode f x = topcode_bblock2code (bblock2code_instr2code f) x

let morphILTypeInILModule ilg ftype y = 
    let finstr modCtxt tdefCtxt mdefCtxt =
        let fty = ftype modCtxt (Some tdefCtxt) mdefCtxt 
        morphILTypesInILInstr ((fun _instrCtxt -> fty), (fun _instrCtxt _formalCtxt -> fty)) 
    morphILInstrsAndILTypesInILModule ilg (finstr,ftype) y

let morphILTypeRefsInILModuleMemoized ilg f modul = 
    let fty = Tables.memoize (typ_tref2tref f)
    morphILTypeInILModule ilg (fun _ _ _ ty -> fty ty) modul

let morphILScopeRefsInILModuleMemoized ilg f modul = 
    morphILTypeRefsInILModuleMemoized ilg (morphILScopeRefsInILTypeRef f) modul
