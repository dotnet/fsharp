// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.AbstractIL.Morphs 

open System.Collections.Generic
open Internal.Utilities.Library 
open FSharp.Compiler.AbstractIL.IL 

let mutable morphCustomAttributeData = false

let enableMorphCustomAttributeData() = 
    morphCustomAttributeData <- true

let disableMorphCustomAttributeData() =
    morphCustomAttributeData <- false

let code_instr2instr f (code: ILCode) =
    { code with Instrs= Array.map f code.Instrs}

let code_instr2instrs f (code: ILCode) = 
    let instrs = code.Instrs
    let codebuf = ResizeArray()
    let adjust = Dictionary()
    let mutable old = 0
    let mutable nw = 0
    for instr in instrs do 
        adjust[old] <- nw
        let instrs : _ list = f instr
        for instr2 in instrs do
            codebuf.Add instr2
            nw <- nw + 1
        old <- old + 1
    adjust[old] <- nw
    let labels =
        let dict = Dictionary.newWithSize code.Labels.Count
        for kvp in code.Labels do dict.Add(kvp.Key, adjust[kvp.Value])
        dict
    { code with 
         Instrs = codebuf.ToArray()
         Labels = labels }

let code_instr2instr_ty2ty (finstr,fTy) (code: ILCode) = 
    let codeR = code_instr2instr finstr code
    let exnSpecsR =
        [ for exnSpec in codeR.Exceptions do
            let clause =
                match exnSpec.Clause with
                | ILExceptionClause.TypeCatch (ilty, b) -> ILExceptionClause.TypeCatch (fTy ilty, b)
                | cl -> cl
            { exnSpec with Clause = clause } ]
    { codeR with Exceptions = exnSpecsR }

// --------------------------------------------------------------------
// Standard morphisms - mapping types etc.
// -------------------------------------------------------------------- 

let rec morphILTypeRefsInILType f x = 
    match x with 
    | ILType.Ptr t -> ILType.Ptr (morphILTypeRefsInILType f t)
    | ILType.FunctionPointer x -> 
        ILType.FunctionPointer
          { x with 
                ArgTypes=List.map (morphILTypeRefsInILType f) x.ArgTypes
                ReturnType=morphILTypeRefsInILType f x.ReturnType}
    | ILType.Byref t -> ILType.Byref (morphILTypeRefsInILType f t)
    | ILType.Boxed cr -> mkILBoxedType (tspec_tref2tref f cr)
    | ILType.Value ir -> ILType.Value (tspec_tref2tref f ir)
    | ILType.Array (s,ty) -> ILType.Array (s,morphILTypeRefsInILType f ty)
    | ILType.TypeVar v ->  ILType.TypeVar v 
    | ILType.Modified (req,tref,ty) ->  ILType.Modified (req, f tref, morphILTypeRefsInILType f ty) 
    | ILType.Void -> ILType.Void

and tspec_tref2tref f (tspec: ILTypeSpec) = 
    mkILTySpec(f tspec.TypeRef, List.map (morphILTypeRefsInILType f) tspec.GenericArgs)

let rec ty_scoref2scoref_tyvar2ty ((_fscope, fTyvar) as fs) ty = 
    match ty with 
    | ILType.Ptr elemTy -> ILType.Ptr (ty_scoref2scoref_tyvar2ty fs elemTy)
    | ILType.FunctionPointer callsig -> ILType.FunctionPointer (callsig_scoref2scoref_tyvar2ty fs callsig)
    | ILType.Byref elemTy -> ILType.Byref (ty_scoref2scoref_tyvar2ty fs elemTy)
    | ILType.Boxed tspec -> mkILBoxedType (tspec_scoref2scoref_tyvar2ty fs tspec)
    | ILType.Value tspec -> ILType.Value (tspec_scoref2scoref_tyvar2ty fs tspec)
    | ILType.Array (shape, elemTy) -> ILType.Array (shape,ty_scoref2scoref_tyvar2ty fs elemTy)
    | ILType.TypeVar idx ->  fTyvar idx
    | x -> x

and tspec_scoref2scoref_tyvar2ty fs (x:ILTypeSpec) = 
    ILTypeSpec.Create(morphILScopeRefsInILTypeRef (fst fs) x.TypeRef,tys_scoref2scoref_tyvar2ty fs x.GenericArgs)

and callsig_scoref2scoref_tyvar2ty f x = 
    { x with 
        ArgTypes=List.map (ty_scoref2scoref_tyvar2ty f) x.ArgTypes
        ReturnType=ty_scoref2scoref_tyvar2ty f x.ReturnType}

and tys_scoref2scoref_tyvar2ty f i = List.map (ty_scoref2scoref_tyvar2ty f) i

and gparams_scoref2scoref_tyvar2ty f i = List.map (gparam_scoref2scoref_tyvar2ty f) i

and gparam_scoref2scoref_tyvar2ty _f i = i

and morphILScopeRefsInILTypeRef fscope (tref: ILTypeRef) = 
    ILTypeRef.Create(scope=fscope tref.Scope, enclosing=tref.Enclosing, name = tref.Name)

let callsig_ty2ty f (callsig: ILCallingSignature) = 
    { CallingConv = callsig.CallingConv
      ArgTypes = List.map f callsig.ArgTypes
      ReturnType = f callsig.ReturnType}

let gparam_ty2ty f gf = {gf with Constraints = List.map f gf.Constraints}
let gparams_ty2ty f gfs = List.map (gparam_ty2ty f) gfs
let tys_ty2ty (f: ILType -> ILType)  x = List.map f x
let mref_ty2ty (f: ILType -> ILType) (x:ILMethodRef) = 
    ILMethodRef.Create(enclosingTypeRef= (f (mkILBoxedType (mkILNonGenericTySpec x.DeclaringTypeRef))).TypeRef,
                       callingConv=x.CallingConv,
                       name=x.Name,
                       genericArity=x.GenericArity,
                       argTypes= List.map f x.ArgTypes,
                       returnType= f x.ReturnType)


type formal_scopeCtxt = Choice<ILMethodSpec, ILFieldSpec>

let mspec_ty2ty ((factualTy : ILType -> ILType, fformalTy: formal_scopeCtxt -> ILType -> ILType)) (x: ILMethodSpec) = 
    mkILMethSpecForMethRefInTy(mref_ty2ty (fformalTy (Choice1Of2 x)) x.MethodRef,
                               factualTy x.DeclaringType, 
                               tys_ty2ty factualTy  x.GenericArgs)

let fref_ty2ty (f: ILType -> ILType) fref = 
    { fref with
        DeclaringTypeRef = (f (mkILBoxedType (mkILNonGenericTySpec fref.DeclaringTypeRef))).TypeRef
        Type= f fref.Type }

let fspec_ty2ty ((factualTy,fformalTy : formal_scopeCtxt -> ILType -> ILType)) fspec = 
    { FieldRef=fref_ty2ty (fformalTy (Choice2Of2 fspec)) fspec.FieldRef
      DeclaringType= factualTy fspec.DeclaringType }

let rec celem_ty2ty f celem =
    match celem with
    | ILAttribElem.Type (Some ty)    -> ILAttribElem.Type (Some (f ty))
    | ILAttribElem.TypeRef (Some tref) -> ILAttribElem.TypeRef (Some (f (mkILBoxedType (mkILNonGenericTySpec tref))).TypeRef)
    | ILAttribElem.Array (elemTy,elems) -> ILAttribElem.Array (f elemTy, List.map (celem_ty2ty f) elems)
    | _ -> celem

let cnamedarg_ty2ty f ((nm, ty, isProp, elem) : ILAttributeNamedArg)  =
    (nm, f ty, isProp, celem_ty2ty f elem) 

let cattr_ty2ty f (c: ILAttribute) =
    let meth = mspec_ty2ty (f, (fun _ -> f)) c.Method
    // dev11 M3 defensive coding: if anything goes wrong with attribute decoding or encoding, then back out.
    if morphCustomAttributeData then
        try 
           let elems,namedArgs = decodeILAttribData c 
           let elems = elems |> List.map (celem_ty2ty f)
           let namedArgs = namedArgs |> List.map (cnamedarg_ty2ty f)
           mkILCustomAttribMethRef (meth, elems, namedArgs)
        with _ ->
           c.WithMethod(meth)
    else
        c.WithMethod(meth)


let cattrs_ty2ty f (cs: ILAttributes) =
    mkILCustomAttrs (List.map (cattr_ty2ty f) (cs.AsList()))

let fdef_ty2ty fTyInCtxt (fdef: ILFieldDef) = 
    fdef.With(fieldType=fTyInCtxt fdef.FieldType,
            customAttrs=cattrs_ty2ty fTyInCtxt fdef.CustomAttrs)

let morphILLocal f (l: ILLocal) = {l with Type = f l.Type}

let morphILVarArgs f (varargs: ILVarArgs) = Option.map (List.map f) varargs

let morphILTypesInILInstr ((factualTy,fformalTy)) i = 
    let factualTy = factualTy (Some i) 
    let conv_fspec fr = fspec_ty2ty (factualTy,fformalTy (Some i)) fr 
    let conv_mspec mr = mspec_ty2ty (factualTy,fformalTy (Some i)) mr 
    match i with 
    | I_calli (a,mref,varargs) ->  I_calli (a,callsig_ty2ty factualTy mref, morphILVarArgs factualTy varargs)
    | I_call (a,mr,varargs) ->  I_call (a,conv_mspec mr, morphILVarArgs factualTy varargs)
    | I_callvirt (a,mr,varargs) ->   I_callvirt (a,conv_mspec mr, morphILVarArgs factualTy varargs)
    | I_callconstraint (a,ty,mr,varargs) ->   I_callconstraint (a,factualTy ty,conv_mspec mr, morphILVarArgs factualTy varargs)
    | I_newobj (mr,varargs) ->  I_newobj (conv_mspec mr, morphILVarArgs factualTy varargs)
    | I_ldftn mr ->  I_ldftn (conv_mspec mr)
    | I_ldvirtftn mr ->  I_ldvirtftn (conv_mspec mr)
    | I_ldfld (a,b,fr) ->  I_ldfld (a,b,conv_fspec fr)
    | I_ldsfld (a,fr) ->  I_ldsfld (a,conv_fspec fr)
    | I_ldsflda fr ->  I_ldsflda (conv_fspec fr)
    | I_ldflda fr ->  I_ldflda (conv_fspec fr)
    | I_stfld (a,b,fr) -> I_stfld (a,b,conv_fspec fr)
    | I_stsfld (a,fr) -> I_stsfld (a,conv_fspec fr)
    | I_castclass ty -> I_castclass (factualTy ty)
    | I_isinst ty -> I_isinst (factualTy ty)
    | I_initobj ty -> I_initobj (factualTy ty)
    | I_cpobj ty -> I_cpobj (factualTy ty)
    | I_stobj (al,vol,ty) -> I_stobj (al,vol,factualTy ty)
    | I_ldobj (al,vol,ty) -> I_ldobj (al,vol,factualTy ty)
    | I_box ty -> I_box (factualTy ty)
    | I_unbox ty -> I_unbox (factualTy ty)
    | I_unbox_any ty -> I_unbox_any (factualTy ty)
    | I_ldelem_any (shape,ty) ->  I_ldelem_any (shape,factualTy ty)
    | I_stelem_any (shape,ty) ->  I_stelem_any (shape,factualTy ty)
    | I_newarr (shape,ty) ->  I_newarr (shape,factualTy ty)
    | I_ldelema (ro,isNativePtr,shape,ty) ->  I_ldelema (ro,isNativePtr,shape,factualTy ty)
    | I_sizeof ty ->  I_sizeof (factualTy ty)
    | I_ldtoken tok -> 
        match tok with 
        | ILToken.ILType ty ->   I_ldtoken (ILToken.ILType (factualTy ty))
        | ILToken.ILMethod mr -> I_ldtoken (ILToken.ILMethod (conv_mspec mr))
        | ILToken.ILField fr -> I_ldtoken (ILToken.ILField (conv_fspec fr))
    | x -> x

let morphILReturn f (r:ILReturn) =
    {r with
        Type=f r.Type
        CustomAttrsStored= storeILCustomAttrs (cattrs_ty2ty f r.CustomAttrs)}

let morphILParameter f (p: ILParameter) =
    { p with
        Type=f p.Type
        CustomAttrsStored= storeILCustomAttrs (cattrs_ty2ty f p.CustomAttrs)}

let morphILMethodDefs f (m:ILMethodDefs) =
    mkILMethods (List.map f (m.AsList()))

let morphILFieldDefs f (fdefs: ILFieldDefs) =
    mkILFields (List.map f (fdefs.AsList()))

let morphILTypeDefs f (tdefs: ILTypeDefs) =
    mkILTypeDefsFromArray (Array.map f (tdefs.AsArray()))

let morphILLocals f locals =
    List.map (morphILLocal f) locals

let ilmbody_instr2instr_ty2ty fs (ilmbody: ILMethodBody) = 
    let finstr, fTyInCtxt = fs 
    {ilmbody with
        Code=code_instr2instr_ty2ty (finstr, fTyInCtxt) ilmbody.Code
        Locals = morphILLocals fTyInCtxt ilmbody.Locals }

let morphILMethodBody fMethBody (x: MethodBody) = 
    match x with
    | MethodBody.IL il -> 
        let ilCode = fMethBody il.Value // Eager
        MethodBody.IL (lazy ilCode)
    | x -> x

let ospec_ty2ty f (OverridesSpec(mref,ty)) = OverridesSpec(mref_ty2ty f mref, f ty)

let mdef_ty2ty_ilmbody2ilmbody fs (md: ILMethodDef)  = 
    let fTyInCtxt,fMethBody = fs 
    let fTyInCtxtR = fTyInCtxt (Some md) 
    let bodyR = morphILMethodBody (fMethBody (Some md)) md.Body 
    md.With(
        genericParams=gparams_ty2ty fTyInCtxtR md.GenericParams,
        body= notlazy bodyR,
        parameters = List.map (morphILParameter fTyInCtxtR) md.Parameters,
        ret = morphILReturn fTyInCtxtR md.Return,
        customAttrs=cattrs_ty2ty fTyInCtxtR md.CustomAttrs
    )

let fdefs_ty2ty f fdefs =
    morphILFieldDefs (fdef_ty2ty f) fdefs

let mdefs_ty2ty_ilmbody2ilmbody fs mdefs =
    morphILMethodDefs (mdef_ty2ty_ilmbody2ilmbody fs) mdefs

let mimpl_ty2ty f mimpl =
    { Overrides = ospec_ty2ty f mimpl.Overrides
      OverrideBy = mspec_ty2ty (f,(fun _ -> f)) mimpl.OverrideBy; }

let edef_ty2ty f (edef: ILEventDef) =
    edef.With(
        eventType = Option.map f edef.EventType,
        addMethod = mref_ty2ty f edef.AddMethod,
        removeMethod = mref_ty2ty f edef.RemoveMethod,
        fireMethod = Option.map (mref_ty2ty f) edef.FireMethod,
        otherMethods = List.map (mref_ty2ty f) edef.OtherMethods,
        customAttrs = cattrs_ty2ty f edef.CustomAttrs
    )

let pdef_ty2ty f (pdef: ILPropertyDef) =
    pdef.With(
        setMethod = Option.map (mref_ty2ty f) pdef.SetMethod,
        getMethod = Option.map (mref_ty2ty f) pdef.GetMethod,
        propertyType = f pdef.PropertyType,
        args = List.map f pdef.Args,
        customAttrs = cattrs_ty2ty f pdef.CustomAttrs
    )

let pdefs_ty2ty f (pdefs: ILPropertyDefs) =
    mkILProperties (pdefs.AsList() |> List.map (pdef_ty2ty f))

let edefs_ty2ty f (edefs: ILEventDefs) =
    mkILEvents (edefs.AsList() |> List.map (edef_ty2ty f))

let mimpls_ty2ty f (mimpls : ILMethodImplDefs) =
    mkILMethodImpls (mimpls.AsList() |> List.map (mimpl_ty2ty f))

let rec tdef_ty2ty_ilmbody2ilmbody_mdefs2mdefs enc fs (tdef: ILTypeDef) = 
   let fTyInCtxt,fMethodDefs = fs 
   let fTyInCtxtR = fTyInCtxt (Some (enc,tdef)) None 
   let mdefsR = fMethodDefs (enc, tdef) tdef.Methods 
   let fdefsR = fdefs_ty2ty fTyInCtxtR tdef.Fields 
   tdef.With(
       implements= List.map fTyInCtxtR tdef.Implements,
       genericParams= gparams_ty2ty fTyInCtxtR tdef.GenericParams,
       extends = Option.map fTyInCtxtR tdef.Extends,
       methods=mdefsR,
       nestedTypes=tdefs_ty2ty_ilmbody2ilmbody_mdefs2mdefs (enc@[tdef]) fs tdef.NestedTypes,
       fields=fdefsR,
       methodImpls = mimpls_ty2ty fTyInCtxtR tdef.MethodImpls,
       events = edefs_ty2ty fTyInCtxtR tdef.Events,
       properties = pdefs_ty2ty fTyInCtxtR tdef.Properties,
       customAttrs = cattrs_ty2ty fTyInCtxtR tdef.CustomAttrs
   )

and tdefs_ty2ty_ilmbody2ilmbody_mdefs2mdefs enc fs tdefs = 
  morphILTypeDefs (tdef_ty2ty_ilmbody2ilmbody_mdefs2mdefs enc fs) tdefs

// --------------------------------------------------------------------
// Derived versions of the above, e.g. with defaults added
// -------------------------------------------------------------------- 

let manifest_ty2ty f (m : ILAssemblyManifest) =
    { m with CustomAttrsStored = storeILCustomAttrs (cattrs_ty2ty f m.CustomAttrs) }

let morphILTypeInILModule_ilmbody2ilmbody_mdefs2mdefs (fTyInCtxt: ILModuleDef -> (ILTypeDef list * ILTypeDef) option -> ILMethodDef option -> ILType -> ILType, fMethodDefs) modul = 

    let ftdefs = tdefs_ty2ty_ilmbody2ilmbody_mdefs2mdefs [] (fTyInCtxt modul, fMethodDefs modul) 

    { modul with
        TypeDefs=ftdefs modul.TypeDefs
        CustomAttrsStored= storeILCustomAttrs (cattrs_ty2ty (fTyInCtxt modul None None) modul.CustomAttrs)
        Manifest=Option.map (manifest_ty2ty (fTyInCtxt modul None None)) modul.Manifest  }
    
let morphILInstrsAndILTypesInILModule fs modul = 
    let fCode, fTyInCtxt = fs 
    let fMethBody modCtxt tdefCtxt mdefCtxt = ilmbody_instr2instr_ty2ty (fCode modCtxt tdefCtxt mdefCtxt, fTyInCtxt modCtxt (Some tdefCtxt) mdefCtxt) 
    let fMethodDefs modCtxt tdefCtxt = mdefs_ty2ty_ilmbody2ilmbody (fTyInCtxt modCtxt (Some tdefCtxt), fMethBody modCtxt tdefCtxt) 
    morphILTypeInILModule_ilmbody2ilmbody_mdefs2mdefs (fTyInCtxt, fMethodDefs) modul 

let morphILInstrsInILCode f ilcode =
    code_instr2instrs f ilcode

let morphILTypeInILModule fTyInCtxt modul = 
    let finstr modCtxt tdefCtxt mdefCtxt =
        let fTy = fTyInCtxt modCtxt (Some tdefCtxt) mdefCtxt 
        morphILTypesInILInstr ((fun _instrCtxt -> fTy), (fun _instrCtxt _formalCtxt -> fTy)) 
    morphILInstrsAndILTypesInILModule (finstr, fTyInCtxt) modul

let morphILTypeRefsInILModuleMemoized f modul = 
    let fTy = Tables.memoize (morphILTypeRefsInILType f)
    morphILTypeInILModule (fun _ _ _ ty -> fTy ty) modul

let morphILScopeRefsInILModuleMemoized f modul = 
    morphILTypeRefsInILModuleMemoized (morphILScopeRefsInILTypeRef f) modul
