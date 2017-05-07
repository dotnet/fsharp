// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//--------------------------------------------------------------------------
// The ILX generator. 
//-------------------------------------------------------------------------- 

module internal Microsoft.FSharp.Compiler.IlxGen

open System.IO
open System.Collections.Generic
open Internal.Utilities
open Internal.Utilities.Collections
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.Types
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.BinaryConstants 

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Import
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Tastops.DebugPrint
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.PrettyNaming
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.TypeRelations
open Microsoft.FSharp.Compiler.TypeChecker
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.Types 

  
let IsNonErasedTypar (tp:Typar) = not tp.IsErased
let DropErasedTypars (tps:Typar list) = tps |> List.filter IsNonErasedTypar
let DropErasedTyargs tys = tys |> List.filter (fun ty -> match ty with TType_measure _ -> false | _ -> true) 
let AddSpecialNameFlag (mdef:ILMethodDef) = { mdef with IsSpecialName = true }

let AddNonUserCompilerGeneratedAttribs (g: TcGlobals) (mdef:ILMethodDef) = g.AddMethodGeneratedAttributes  mdef

let debugDisplayMethodName = "__DebugDisplay"

let useHiddenInitCode = true

//--------------------------------------------------------------------------
// misc
//-------------------------------------------------------------------------- 

let iLdcZero = AI_ldc (DT_I4,ILConst.I4 0)
let iLdcInt64 i = AI_ldc (DT_I8,ILConst.I8 i)
let iLdcDouble i = AI_ldc (DT_R8,ILConst.R8 i)
let iLdcSingle i = AI_ldc (DT_R4,ILConst.R4 i)

/// Make a method that simply loads a field
let mkLdfldMethodDef (ilMethName,reprAccess,isStatic,ilTy,ilFieldName,ilPropType) =
   let ilFieldSpec = mkILFieldSpecInTy(ilTy,ilFieldName,ilPropType)
   let ilReturn = mkILReturn ilPropType
   let ilMethodDef = 
       if isStatic then 
           mkILNonGenericStaticMethod (ilMethName,reprAccess,[],ilReturn,mkMethodBody(true,[],2,nonBranchingInstrsToCode [mkNormalLdsfld ilFieldSpec],None))
       else 
           mkILNonGenericInstanceMethod (ilMethName,reprAccess,[],ilReturn,mkMethodBody (true,[],2,nonBranchingInstrsToCode [ mkLdarg0; mkNormalLdfld ilFieldSpec],None))
   ilMethodDef |> AddSpecialNameFlag

let ChooseParamNames fieldNamesAndTypes = 
    let takenFieldNames = fieldNamesAndTypes |> List.map p23 |> Set.ofList

    fieldNamesAndTypes
    |> List.map (fun (ilPropName,ilFieldName,ilPropType) -> 
        let lowerPropName = String.uncapitalize ilPropName 
        let ilParamName = if takenFieldNames.Contains(lowerPropName) then ilPropName else lowerPropName 
        ilParamName,ilFieldName,ilPropType)

let markup s = Seq.indexed s

// Approximation for purposes of optimization and giving a warning when compiling definition-only files as EXEs 
let rec CheckCodeDoesSomething (code: ILCode) = 
    code.Instrs |> Array.exists (function AI_ldnull | AI_nop | AI_pop | I_ret |  I_seqpoint _ -> false | _ -> true) 

let ChooseFreeVarNames takenNames ts =
    let tns = List.map (fun t -> (t,None)) ts
    let rec chooseName names (t,nOpt) = 
        let tn = match nOpt with None -> t | Some n -> t + string n
        if Zset.contains tn names then
          chooseName names (t,Some(match nOpt with None ->  0 | Some n -> (n+1)))
        else
          let names = Zset.add tn names
          tn,names

    let names    = Zset.empty String.order |> Zset.addList takenNames
    let ts,_names = List.mapFold chooseName names tns
    ts

let ilxgenGlobalNng = NiceNameGenerator ()

// We can't tailcall to methods taking byrefs. This helper helps search for them
let IsILTypeByref  = function ILType.Byref _ -> true | _ -> false

let mainMethName = CompilerGeneratedName "main"

type AttributeDecoder(namedArgs) = 
    let nameMap = namedArgs |> List.map (fun (AttribNamedArg(s,_,_,c)) -> s,c) |> NameMap.ofList
    let findConst x = match NameMap.tryFind x nameMap with | Some(AttribExpr(_,Expr.Const(c,_,_))) -> Some c | _ -> None
    let findAppTr x = match NameMap.tryFind x nameMap with | Some(AttribExpr(_,Expr.App(_,_,[TType_app(tr,_)],_,_))) -> Some tr | _ -> None

    member self.FindInt16  x dflt = match findConst x with | Some(Const.Int16 x) -> x | _ -> dflt
    member self.FindInt32  x dflt = match findConst x with | Some(Const.Int32 x) -> x | _ -> dflt
    member self.FindBool   x dflt = match findConst x with | Some(Const.Bool x) -> x | _ -> dflt
    member self.FindString x dflt = match findConst x with | Some(Const.String x) -> x | _ -> dflt
    member self.FindTypeName   x dflt = match findAppTr x with | Some(tr) -> tr.DisplayName | _ -> dflt             
    
//--------------------------------------------------------------------------
// Statistics
//-------------------------------------------------------------------------- 

let mutable reports = (fun _ -> ()) 
let AddReport f = let old = reports in reports <- (fun oc -> old oc; f oc) 
let ReportStatistics (oc:TextWriter) = reports oc

let NewCounter nm = 
    let count = ref 0
    AddReport (fun oc -> if !count <> 0 then oc.WriteLine (string !count + " " + nm))
    (fun () -> incr count)

let CountClosure = NewCounter "closures"
let CountMethodDef = NewCounter "IL method defintitions corresponding to values"
let CountStaticFieldDef = NewCounter "IL field defintitions corresponding to values"
let CountCallFuncInstructions = NewCounter "callfunc instructions (indirect calls)"

/// Non-local information related to internals of code generation within an assembly
type IlxGenIntraAssemblyInfo = 
    { /// A table recording the generated name of the static backing fields for each mutable top level value where 
      /// we may need to take the address of that value, e.g. static mutable module-bound values which are structs. These are 
      /// only accessible intra-assembly. Across assemblies, taking the address of static mutable module-bound values is not permitted.
      /// The key to the table is the method ref for the property getter for the value, which is a stable name for the Val's
      /// that come from both the signature and the implementation.
      StaticFieldInfo : Dictionary<ILMethodRef, ILFieldSpec> }

//-------------------------------------------------------------------------- 

/// Indicates how the generated IL code is ultimately emitted 
type IlxGenBackend =
|   IlWriteBackend
|   IlReflectBackend

[<NoEquality; NoComparison>]
type IlxGenOptions = 
    { fragName: string
      generateFilterBlocks: bool
      workAroundReflectionEmitBugs: bool
      emitConstantArraysUsingStaticDataBlobs: bool
      /// If this is set, then the last module becomes the "main" module and its toplevel bindings are executed at startup 
      mainMethodInfo: Tast.Attribs option
      localOptimizationsAreOn: bool
      generateDebugSymbols: bool
      testFlagEmitFeeFeeAs100001: bool
      ilxBackend: IlxGenBackend

      /// Indicates the code is being generated in FSI.EXE and is executed immediately after code generation
      /// This includes all interactively compiled code, including #load, definitions, and expressions
      isInteractive: bool 

      /// Indicates the code generated is an interactive 'it' expression. We generate a setter to allow clearing of the underlying
      /// storage, even though 'it' is not logically mutable
      isInteractiveItExpr: bool

      /// Whenever possible, use callvirt instead of call
      alwaysCallVirt: bool  }


/// Compilation environment for compiling a fragment of an assembly
[<NoEquality; NoComparison>]
type cenv = 
    { g: TcGlobals
      TcVal : ConstraintSolver.TcValF
      viewCcu: CcuThunk
      opts: IlxGenOptions
      /// Cache the generation of the "unit" type
      mutable ilUnitTy: ILType option
      amap: ImportMap
      intraAssemblyInfo : IlxGenIntraAssemblyInfo
      /// Cache methods with SecurityAttribute applied to them, to prevent unnecessary calls to ExistsInEntireHierarchyOfType
      casApplied : Dictionary<Stamp,bool> 
      /// Used to apply forced inlining optimizations to witnesses generated late during codegen
      mutable optimizeDuringCodeGen : (Expr -> Expr) }



let mkTypeOfExpr cenv m ilty =  
    mkAsmExpr ([  mkNormalCall (mspec_Type_GetTypeFromHandle cenv.g) ], [],
                   [mkAsmExpr ([ I_ldtoken (ILToken.ILType ilty) ], [],[],[cenv.g.system_RuntimeTypeHandle_typ],m)],
                   [cenv.g.system_Type_typ],m)    
                   
let mkGetNameExpr cenv (ilt : ILType) m =
    mkAsmExpr ([I_ldstr ilt.BasicQualifiedName],[],[],[cenv.g.string_ty],m)  

let useCallVirt cenv boxity (mspec : ILMethodSpec) isBaseCall = 
    cenv.opts.alwaysCallVirt && 
    (boxity = AsObject) && 
    not mspec.CallingConv.IsStatic && 
    not isBaseCall    

//--------------------------------------------------------------------------
// CompileLocation
//--------------------------------------------------------------------------
 
/// compilation location = path to a ccu, namespace or class 
/// Referencing other stuff, and descriptions of where items are to be placed
/// within the generated IL namespace/typespace.  This should be cleaned up.
type CompileLocation = 
    { clocScope: IL.ILScopeRef 
      clocTopImplQualifiedName: string
      clocNamespace: string option  
      clocEncl: string list
      clocQualifiedNameOfFile : string }

//--------------------------------------------------------------------------
// Access this and other assemblies
//-------------------------------------------------------------------------- 

let mkTopName ns n = String.concat "." (match ns with Some x -> [x;n] | None -> [n])

let CompLocForFragment fragName (ccu:CcuThunk) = 
   { clocQualifiedNameOfFile =fragName
     clocTopImplQualifiedName= fragName 
     clocScope=ccu.ILScopeRef 
     clocNamespace=None 
     clocEncl=[]} 

let CompLocForCcu (ccu:CcuThunk) =  CompLocForFragment ccu.AssemblyName ccu

let CompLocForSubModuleOrNamespace cloc (submod:ModuleOrNamespace) =
    let n = submod.CompiledName
    match submod.ModuleOrNamespaceType.ModuleOrNamespaceKind with 
    | FSharpModuleWithSuffix | ModuleOrType -> { cloc with clocEncl= cloc.clocEncl @ [n]}
    | Namespace -> {cloc with clocNamespace=Some (mkTopName cloc.clocNamespace n)}

let CompLocForFixedPath fragName qname (CompPath(sref,cpath)) = 
    let ns,t = List.takeUntil (fun (_,mkind) -> mkind <> Namespace) cpath
    let ns = List.map fst ns
    let ns = textOfPath ns
    let encl = t |> List.map (fun (s ,_)-> s)
    let ns = if ns = "" then None else Some ns
    { clocQualifiedNameOfFile =fragName
      clocTopImplQualifiedName=qname
      clocScope=sref
      clocNamespace=ns 
      clocEncl=encl }

let CompLocForFixedModule fragName qname (mspec:ModuleOrNamespace) = 
   let cloc = CompLocForFixedPath fragName qname mspec.CompilationPath
   let cloc = CompLocForSubModuleOrNamespace cloc mspec
   cloc 

let NestedTypeRefForCompLoc cloc n = 
    match cloc.clocEncl with 
    | [] ->
        let tyname = mkTopName cloc.clocNamespace n
        mkILTyRef(cloc.clocScope,tyname)
    | h::t -> mkILNestedTyRef(cloc.clocScope,mkTopName cloc.clocNamespace h :: t,n)
        
let CleanUpGeneratedTypeName (nm:string) = 
    if nm.IndexOfAny IllegalCharactersInTypeAndNamespaceNames = -1 then 
        nm
    else
        (nm,IllegalCharactersInTypeAndNamespaceNames) ||> Array.fold (fun nm c -> nm.Replace(string c, "-"))
  

let TypeNameForInitClass cloc = "<StartupCode$" + (CleanUpGeneratedTypeName cloc.clocQualifiedNameOfFile) + ">.$" + cloc.clocTopImplQualifiedName 
let TypeNameForImplicitMainMethod cloc = TypeNameForInitClass cloc + "$Main"
let TypeNameForPrivateImplementationDetails cloc = "<PrivateImplementationDetails$" + (CleanUpGeneratedTypeName cloc.clocQualifiedNameOfFile) + ">"

let CompLocForInitClass cloc = 
    {cloc with clocEncl=[TypeNameForInitClass cloc]; clocNamespace=None}

let CompLocForImplicitMainMethod cloc = 
    {cloc with clocEncl=[TypeNameForImplicitMainMethod cloc]; clocNamespace=None}

let CompLocForPrivateImplementationDetails cloc = 
    {cloc with 
        clocEncl=[TypeNameForPrivateImplementationDetails cloc]; clocNamespace=None}

let rec TypeRefForCompLoc cloc  =
    match cloc.clocEncl with
    | [] ->  
      mkILTyRef(cloc.clocScope,TypeNameForPrivateImplementationDetails cloc)
    | [h] -> 
      let tyname = mkTopName cloc.clocNamespace h
      mkILTyRef(cloc.clocScope,tyname)
    | _ ->  
      let encl,n = List.frontAndBack cloc.clocEncl
      NestedTypeRefForCompLoc {cloc with clocEncl=encl} n 

let mkILTyForCompLoc cloc = mkILNonGenericBoxedTy (TypeRefForCompLoc cloc)

let ComputeMemberAccess hidden = if hidden then ILMemberAccess.Assembly else ILMemberAccess.Public


// Under --publicasinternal change types from Public to Private (internal for types)
let ComputePublicTypeAccess() = ILTypeDefAccess.Public

let ComputeTypeAccess (tref:ILTypeRef) hidden = 
    match tref.Enclosing with 
    | [] -> if hidden then ILTypeDefAccess.Private else ComputePublicTypeAccess() 
    | _ -> ILTypeDefAccess.Nested (ComputeMemberAccess hidden)
            
//--------------------------------------------------------------------------
// TypeReprEnv
//-------------------------------------------------------------------------- 

/// Indicates how type parameters are mapped to IL type variables 
[<NoEquality; NoComparison>]
type TypeReprEnv(reprs : Map<Stamp, uint16>, count: int) = 

    member tyenv.Item (tp:Typar, m:range) = 
        try reprs.[tp.Stamp] 
        with :? KeyNotFoundException -> 
          errorR(InternalError("Undefined or unsolved type variable: " + showL(typarL tp),m)) 
          // Random value for post-hoc diagnostic analysis on generated tree *
          uint16 666 

    member tyenv.AddOne (tp: Typar) =
        if IsNonErasedTypar tp then 
            TypeReprEnv(reprs.Add (tp.Stamp, uint16 count), count + 1)
        else
            tyenv

    member tyenv.Add tps =
        (tyenv,tps) ||> List.fold (fun tyenv tp -> tyenv.AddOne tp)

    member tyenv.Count = count

    static member Empty = 
        TypeReprEnv(count = 0, reprs = Map.empty)

    static member ForTypars tps = 
        TypeReprEnv.Empty.Add tps
         
    static member ForTycon (tycon:Tycon) = 
        TypeReprEnv.ForTypars (tycon.TyparsNoRange)
        
    static member ForTyconRef (tycon:TyconRef) = 
        TypeReprEnv.ForTycon tycon.Deref
        

//--------------------------------------------------------------------------
// Generate type references
//-------------------------------------------------------------------------- 

let GenTyconRef (tcref:TyconRef) = 
    assert(not tcref.IsTypeAbbrev)
    tcref.CompiledRepresentation

type VoidNotOK = VoidNotOK | VoidOK
#if DEBUG 
let voidCheck m g permits ty = 
   if permits=VoidNotOK && isVoidTy g ty then 
       error(InternalError("System.Void unexpectedly detected in IL code generation. This should not occur.",m))
#endif

// When generating parameter and return types generate precise .NET IL pointer types 
// These can't be generated for generic instantiations, since .NET generics doesn't 
// permit this. But for 'naked' values (locals, parameters, return values etc.) machine 
// integer values and native pointer values are compatible (though the code is unverifiable). 
type PtrsOK = 
    | PtrTypesOK 
    | PtrTypesNotOK

let rec GenTypeArgAux amap m tyenv tyarg =  
    GenTypeAux amap m tyenv VoidNotOK PtrTypesNotOK tyarg

and GenTypeArgsAux amap m tyenv  tyargs = 
    List.map (GenTypeArgAux amap m tyenv) (DropErasedTyargs tyargs)

and GenTyAppAux amap m tyenv repr tinst =
    match repr with  
    | CompiledTypeRepr.ILAsmOpen ty -> 
        let ilTypeInst = GenTypeArgsAux amap m tyenv tinst
        let ty = IL.instILType ilTypeInst ty
        ty
    | CompiledTypeRepr.ILAsmNamed (tref, boxity, ilTypeOpt) -> 
        match ilTypeOpt with 
        | None -> 
            let ilTypeInst = GenTypeArgsAux amap m tyenv tinst
            mkILTy boxity (mkILTySpec (tref,ilTypeInst))
        | Some ilType -> 
            ilType // monomorphic types include a cached ilType to avoid reallocation of an ILType node


and GenNamedTyAppAux (amap:ImportMap) m tyenv ptrsOK tcref tinst = 
    let g = amap.g
    let tinst = DropErasedTyargs tinst 
    // See above note on ptrsOK 
    if ptrsOK = PtrTypesOK && tyconRefEq g tcref g.nativeptr_tcr && (freeInTypes CollectTypars tinst).FreeTypars.IsEmpty then 
        GenNamedTyAppAux amap m tyenv ptrsOK g.ilsigptr_tcr tinst
    else
#if EXTENSIONTYPING
        match tcref.TypeReprInfo with 
        // Generate the base type, because that is always the representation of the erased type, unless the assembly is being injected
        | TProvidedTypeExtensionPoint info when info.IsErased -> 
            GenTypeAux amap m tyenv VoidNotOK ptrsOK (info.BaseTypeForErased (m,g.obj_ty))
        | _ -> 
#endif
            GenTyAppAux amap m tyenv (GenTyconRef tcref) tinst

and GenTypeAux amap m (tyenv: TypeReprEnv) voidOK ptrsOK ty =
    let g = amap.g
#if DEBUG 
    voidCheck m g voidOK ty
#else
    ignore voidOK    
#endif
    match stripTyEqnsAndMeasureEqns g ty with 
    | TType_app (tcref, tinst) -> GenNamedTyAppAux amap m tyenv ptrsOK tcref tinst
    | TType_tuple (tupInfo, args) -> GenTypeAux amap m tyenv VoidNotOK ptrsOK (mkCompiledTupleTy g (evalTupInfoIsStruct tupInfo) args)
    | TType_fun (dty, returnTy) -> EraseClosures.mkILFuncTy g.ilxPubCloEnv  (GenTypeArgAux amap m tyenv dty) (GenTypeArgAux amap m tyenv returnTy)

    | TType_ucase (ucref, args) -> 
        let cuspec,idx = GenUnionCaseSpec amap m tyenv ucref args 
        EraseUnions.GetILTypeForAlternative cuspec idx

    | TType_forall (tps, tau) -> 
        let tps = DropErasedTypars tps 
        if tps.IsEmpty then GenTypeAux amap m tyenv VoidNotOK ptrsOK tau
        else EraseClosures.mkILTyFuncTy g.ilxPubCloEnv 
    | TType_var tp -> mkILTyvarTy tyenv.[tp,m]
    | TType_measure _ -> g.ilg.typ_Int32 

//--------------------------------------------------------------------------
// Generate ILX references to closures, classunions etc. given a tyenv
//-------------------------------------------------------------------------- 

and GenUnionCaseRef (amap: ImportMap) m tyenv i (fspecs:RecdField array) = 
    let g = amap.g
    fspecs |> Array.mapi (fun j fspec -> 
        let ilFieldDef = IL.mkILInstanceField(fspec.Name,GenType amap m tyenv fspec.FormalType, None, ILMemberAccess.Public)
        IlxUnionField
          { ilFieldDef with 
              // These properties on the "field" of an alternative end up going on a property generated by cu_erase.fs
              CustomAttrs = mkILCustomAttrs [(mkCompilationMappingAttrWithVariantNumAndSeqNum g (int SourceConstructFlags.Field) i j )] } )
   

and GenUnionRef (amap: ImportMap) m (tcref: TyconRef) = 
    let g = amap.g
    let tycon = tcref.Deref
    assert(not tycon.IsTypeAbbrev)
    match tycon.UnionTypeInfo with 
    | None -> failwith "GenUnionRef m"
    | Some funion -> 
      cached funion.CompiledRepresentation (fun () -> 
          let tyenvinner = TypeReprEnv.ForTycon tycon
          match tcref.CompiledRepresentation with
          | CompiledTypeRepr.ILAsmOpen _ -> failwith "GenUnionRef m: unexpected ASM tyrep"
          | CompiledTypeRepr.ILAsmNamed (tref,_,_) -> 
              let alternatives = 
                  tycon.UnionCasesArray |> Array.mapi (fun i cspec -> 
                      { altName=cspec.CompiledName
                        altCustomAttrs=emptyILCustomAttrs
                        altFields=GenUnionCaseRef amap m tyenvinner i cspec.RecdFieldsArray })
              let nullPermitted = IsUnionTypeWithNullAsTrueValue g tycon
              let hasHelpers = ComputeUnionHasHelpers g tcref
              let boxity = (if tcref.IsStructOrEnumTycon then ILBoxity.AsValue else ILBoxity.AsObject)
              IlxUnionRef(boxity, tref,alternatives,nullPermitted,hasHelpers))

and ComputeUnionHasHelpers g (tcref : TyconRef) = 
    if tyconRefEq g tcref g.unit_tcr_canon then NoHelpers
    elif tyconRefEq g tcref g.list_tcr_canon then SpecialFSharpListHelpers
    elif tyconRefEq g tcref g.option_tcr_canon then SpecialFSharpOptionHelpers
    else
     match TryFindFSharpAttribute g g.attrib_DefaultAugmentationAttribute tcref.Attribs with
     | Some(Attrib(_,_,[ AttribBoolArg (b) ],_,_,_,_)) -> 
         if b then AllHelpers else NoHelpers
     | Some (Attrib(_,_,_,_,_,_,m))  -> 
         errorR(Error(FSComp.SR.ilDefaultAugmentationAttributeCouldNotBeDecoded(),m))
         AllHelpers
     | _ -> 
         AllHelpers (* not hiddenRepr *)

and GenUnionSpec amap m tyenv tcref tyargs = 
    let curef = GenUnionRef amap m tcref
    let tinst = GenTypeArgs amap m tyenv tyargs
    IlxUnionSpec(curef,tinst) 

and GenUnionCaseSpec amap m tyenv (ucref:UnionCaseRef) tyargs = 
    let cuspec = GenUnionSpec amap m tyenv ucref.TyconRef tyargs
    cuspec, ucref.Index

and GenType amap m tyenv ty = 
    GenTypeAux amap m tyenv VoidNotOK PtrTypesNotOK ty


and GenTypes amap m tyenv tys = List.map (GenType amap m tyenv) tys
and GenTypePermitVoid amap m tyenv ty = (GenTypeAux amap m tyenv VoidOK PtrTypesNotOK ty)
and GenTypesPermitVoid amap m tyenv tys = List.map (GenTypePermitVoid amap m tyenv) tys

and GenTyApp amap m tyenv repr tyargs = GenTyAppAux amap m tyenv repr tyargs
and GenNamedTyApp amap m tyenv tcref tinst = GenNamedTyAppAux amap m tyenv PtrTypesNotOK tcref tinst 

/// IL void types are only generated for return types 
and GenReturnType amap m tyenv returnTyOpt = 
    match returnTyOpt with 
    | None -> ILType.Void
    | Some returnTy -> GenTypeAux amap m tyenv VoidNotOK(*1*) PtrTypesOK returnTy (*1: generate void from unit, but not accept void *)

and GenParamType amap m tyenv ty = 
    ty |> GenTypeAux amap m tyenv VoidNotOK PtrTypesOK 

and GenParamTypes amap m tyenv tys = 
    tys |> List.map (GenTypeAux amap m tyenv VoidNotOK PtrTypesOK) 

and GenTypeArgs amap m tyenv tyargs = GenTypeArgsAux amap m tyenv tyargs

let GenericParamHasConstraint (gp: ILGenericParameterDef) = 
     gp.Constraints.Length <> 0 ||
     gp.Variance <> NonVariant ||
     gp.HasReferenceTypeConstraint ||
     gp.HasNotNullableValueTypeConstraint ||
     gp.HasDefaultConstructorConstraint


// Static fields generally go in a private InitializationCodeAndBackingFields section. This is to ensure all static 
// fields are initialized only in their class constructors (we generate one primary 
// cctor for each file to ensure initialization coherence across the file, regardless 
// of how many modules are in the file). This means F# passes an extra check applied by SQL Server when it
// verifies stored procedures: SQL Server checks that all 'initionly' static fields are only initialized from
// their own class constructor. 
//   
// However, mutable static fields must be accessible across compilation units. This means we place them in their "natural" location
// which may be in a nested module etc. This means mutable static fields can't be used in code to be loaded by SQL Server. 
//   
// Computes the location where the static field for a value lives. 
//     - Literals go in their type/module. 
//     - For interactive code, we always place fields in their type/module with an accurate name
let GenFieldSpecForStaticField (isInteractive, g, ilContainerTy, vspec:Val, nm, m, cloc, ilTy) =
    if isInteractive || HasFSharpAttribute g g.attrib_LiteralAttribute vspec.Attribs then 
        let fieldName = vspec.CompiledName 
        let fieldName = if isInteractive then CompilerGeneratedName fieldName else fieldName
        mkILFieldSpecInTy (ilContainerTy, fieldName, ilTy) 
    else
        let fieldName = ilxgenGlobalNng.FreshCompilerGeneratedName (nm,m)
        let ilFieldContainerTy = mkILTyForCompLoc (CompLocForInitClass cloc)
        mkILFieldSpecInTy (ilFieldContainerTy, fieldName, ilTy) 

let GenRecdFieldRef m cenv tyenv (rfref:RecdFieldRef) tyargs = 
    let tyenvinner = TypeReprEnv.ForTycon rfref.Tycon
    mkILFieldSpecInTy(GenTyApp cenv.amap m tyenv rfref.TyconRef.CompiledRepresentation tyargs,
                      ComputeFieldName rfref.Tycon rfref.RecdField,
                      GenType cenv.amap m tyenvinner rfref.RecdField.FormalType)

let GenExnType amap m tyenv (ecref:TyconRef) = GenTyApp amap m tyenv ecref.CompiledRepresentation []


//--------------------------------------------------------------------------
// Closure summaries
//-------------------------------------------------------------------------- 

type ArityInfo = int list
      

[<NoEquality; NoComparison>]
type IlxClosureInfo = 
    { cloExpr: Expr
      cloName: string
      cloArityInfo: ArityInfo
      cloILFormalRetTy: ILType
      /// An immutable array of free variable descriptions for the closure
      cloILFreeVars: IlxClosureFreeVar[] 
      cloSpec: IlxClosureSpec
      cloAttribs: Attribs
      cloILGenericParams: IL.ILGenericParameterDefs
      cloFreeVars: Val list (* nb. the freevars we actually close over *)
      ilCloLambdas: IlxClosureLambdas

      (* local type func support *)
      /// The free type parameters occuring in the type of the closure (and not just its body)
      /// This is used for local type functions, whose contract class must use these types
      ///    type Contract<'fv> =
      ///        abstract DirectInvoke : ty['fv]
      ///    type Implementation<'fv,'fv2> : Contract<'fv> =
      ///        override DirectInvoke : ty['fv] = expr['fv,'fv2]
      ///
      ///   At the callsite we generate
      ///      unbox ty['fv]
      ///      callvirt clo.DirectInvoke
      localTypeFuncILGenericArgs: ILType list
      localTypeFuncContractFreeTypars: Typar list
      localTypeFuncDirectILGenericParams: IL.ILGenericParameterDefs 
      localTypeFuncInternalFreeTypars: Typar list}


//--------------------------------------------------------------------------
// Representation of term declarations = Environments for compiling expressions.
//-------------------------------------------------------------------------- 

      
[<NoEquality; NoComparison>]
type ValStorage = 
    /// Indicates the value is always null
    | Null 
    /// Indicates the value is stored in a static field. 
    | StaticField of ILFieldSpec * ValRef * (*hasLiteralAttr:*)bool * ILType * string * ILType * ILMethodRef  * ILMethodRef  * OptionalShadowLocal
    /// Indicates the value is "stored" as a property that recomputes it each time it is referenced. Used for simple constants that do not cause initialization triggers
    | StaticProperty of ILMethodSpec  * OptionalShadowLocal
    /// Indicates the value is "stored" as a IL static method (in a "main" class for a F# 
    /// compilation unit, or as a member) according to its inferred or specified arity.  
    | Method of  ValReprInfo * ValRef * ILMethodSpec * Range.range * ArgReprInfo list * ArgReprInfo
    /// Indicates the value is stored at the given position in the closure environment accessed via "ldarg 0"
    | Env of ILType * int * ILFieldSpec * NamedLocalIlxClosureInfo ref option  
    /// Indicates that the value is an argument of a method being generated
    | Arg of int 
    /// Indicates that the value is stored in local of the method being generated. NamedLocalIlxClosureInfo is normally empty.
    /// It is non-empty for 'local type functions', see comments on definition of NamedLocalIlxClosureInfo.
    | Local of int * NamedLocalIlxClosureInfo ref option 

and OptionalShadowLocal = 
    | NoShadowLocal
    | ShadowLocal of ValStorage

/// The representation of a NamedLocalClosure is based on a cloinfo.  However we can't generate a cloinfo until we've 
/// decided the representations of other items in the recursive set. Hence we use two phases to decide representations in 
/// a recursive set. Yuck. 
and NamedLocalIlxClosureInfo = 
    | NamedLocalIlxClosureInfoGenerator of (IlxGenEnv -> IlxClosureInfo)
    | NamedLocalIlxClosureInfoGenerated of IlxClosureInfo
  
and ModuleStorage = 
    { Vals: Lazy<NameMap<ValStorage>> 
      SubModules: Lazy<NameMap<ModuleStorage>> }

/// BranchCallItems are those where a call to the value can be implemented as 
/// a branch. At the moment these are only used for generating branch calls back to 
/// the entry label of the method currently being generated. 
and BranchCallItem = 
    | BranchCallClosure of ArityInfo
    | BranchCallMethod of 
        // Argument counts for compiled form  of F# method or value
        ArityInfo * 
        // Arg infos for compiled form of F# method or value
        (TType * ArgReprInfo) list list * 
        // Typars for F# method or value
        Tast.Typars * 
        // Typars for F# method or value
        int *
        // num obj args 
        int 
      
and Mark = 
    | Mark of ILCodeLabel (* places we can branch to  *)
    member x.CodeLabel = (let (Mark(lab)) = x in lab)

and IlxGenEnv =
    { tyenv: TypeReprEnv 
      someTypeInThisAssembly: ILType
      isFinalFile: bool
      /// Where to place the stuff we're currently generating
      cloc: CompileLocation 
      /// Hiding information down the signature chain, used to compute what's public to the assembly 
      sigToImplRemapInfo: (Remap * SignatureHidingInfo) list 
      /// All values in scope 
      valsInScope: ValMap<Lazy<ValStorage>> 
      /// For optimizing direct tail recursion to a loop - mark says where to branch to.  Length is 0 or 1. 
      /// REVIEW: generalize to arbitrary nested local loops?? 
      innerVals: (ValRef * (BranchCallItem * Mark)) list 
      /// Full list of enclosing bound values.  First non-compiler-generated element is used to help give nice names for closures and other expressions.  
      letBoundVars: ValRef list 
      /// The set of IL local variable indexes currently in use by lexically scoped variables, to allow reuse on different branches. 
      /// Really an integer set. 
      liveLocals: IntMap<unit> 
      /// Are we under the scope of a try, catch or finally? If so we can't tailcall. SEH = structured exception handling
      withinSEH: bool }

let ReplaceTyenv tyenv (eenv: IlxGenEnv) = {eenv with tyenv = tyenv } 
let EnvForTypars tps eenv =  {eenv with tyenv = TypeReprEnv.ForTypars tps } 
let AddTyparsToEnv typars (eenv: IlxGenEnv) = {eenv with tyenv = eenv.tyenv.Add typars}

let AddSignatureRemapInfo _msg (rpi, mhi) eenv = 
    { eenv with sigToImplRemapInfo = (mkRepackageRemapping rpi,mhi) :: eenv.sigToImplRemapInfo }
     
//--------------------------------------------------------------------------
// Print eenv
//-------------------------------------------------------------------------- 

let OutputStorage (pps: TextWriter) s = 
    match s with 
    | StaticField _ ->  pps.Write "(top)" 
    | StaticProperty _ -> pps.Write "(top)" 
    | Method _ -> pps.Write "(top)" 
    | Local _ -> pps.Write "(local)" 
    | Arg _ -> pps.Write "(arg)" 
    | Env _ -> pps.Write "(env)" 
    | Null -> pps.Write "(null)"

//--------------------------------------------------------------------------
// Augment eenv with values
//-------------------------------------------------------------------------- 

let AddStorageForVal (g: TcGlobals) (v,s) eenv = 
    let eenv = { eenv with valsInScope = eenv.valsInScope.Add v s }
    // If we're compiling fslib then also bind the value as a non-local path to 
    // allow us to resolve the compiler-non-local-references that arise from env.fs
    //
    // Do this by generating a fake "looking from the outside in" non-local value reference for
    // v, dereferencing it to find the corresponding signature Val, and adding an entry for the signature val.
    //
    // A similar code path exists in ilxgen.fs for the tables of "optimization data" for values
    if g.compilingFslib then 
        // Passing an empty remap is sufficient for FSharp.Core.dll because it turns out the remapped type signature can
        // still be resolved.
        match tryRescopeVal g.fslibCcu Remap.Empty  v with 
        | None -> eenv
        | Some vref -> 
            match vref.TryDeref with
            | VNone -> 
                //let msg = sprintf "could not dereference external value reference to something in FSharp.Core.dll during code generation, v.MangledName = '%s', v.Range = %s" v.MangledName (stringOfRange v.Range)
                //System.Diagnostics.Debug.Assert(false, msg)
                eenv
            | VSome gv -> 
                { eenv with valsInScope = eenv.valsInScope.Add gv s }
    else 
        eenv

let AddStorageForLocalVals g vals eenv = List.foldBack (fun (v,s) acc -> AddStorageForVal g (v,notlazy s) acc) vals eenv

//--------------------------------------------------------------------------
// Lookup eenv 
//-------------------------------------------------------------------------- 
  
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 

let StorageForVal m v eenv = 
    let v = 
        try eenv.valsInScope.[v]
        with :? KeyNotFoundException ->
          assert false
          errorR(Error(FSComp.SR.ilUndefinedValue(showL(vspecAtBindL v)),m)) 
          notlazy (Arg 668(* random value for post-hoc diagnostic analysis on generated tree *) )
    v.Force()

let StorageForValRef m (v: ValRef) eenv = StorageForVal m v.Deref eenv

//--------------------------------------------------------------------------
// Imported modules and the environment
//
// How a top level value is represented depends on its type.  If it's a 
// function or is polymorphic, then it gets represented as a 
// method (possibly and instance method).  Otherwise it gets represented as a 
// static field.
//-------------------------------------------------------------------------- 

let IsValRefIsDllImport g (vref:ValRef) = 
    vref.Attribs |> HasFSharpAttributeOpt g g.attrib_DllImportAttribute 

let GetMethodSpecForMemberVal amap g (memberInfo:ValMemberInfo) (vref:ValRef) = 
    let m = vref.Range
    let tps,curriedArgInfos,returnTy,retInfo = 
         assert(vref.ValReprInfo.IsSome)
         GetTopValTypeInCompiledForm g (Option.get vref.ValReprInfo) vref.Type m
    let tyenvUnderTypars = TypeReprEnv.ForTypars tps
    let flatArgInfos = List.concat curriedArgInfos
    let isCtor = (memberInfo.MemberFlags.MemberKind = MemberKind.Constructor)
    let cctor = (memberInfo.MemberFlags.MemberKind = MemberKind.ClassConstructor)
    let parentTcref = vref.TopValActualParent
    let parentTypars = parentTcref.TyparsNoRange
    let numParentTypars = parentTypars.Length
    if tps.Length < numParentTypars then error(InternalError("CodeGen check: type checking did not ensure that this method is sufficiently generic", m))
    let ctps,mtps = List.chop numParentTypars tps
    let isCompiledAsInstance = ValRefIsCompiledAsInstanceMember g vref

    let ilActualRetTy = 
        let ilRetTy = GenReturnType amap m tyenvUnderTypars returnTy
        if isCtor || cctor then ILType.Void else ilRetTy
    let ilTy = GenType amap m tyenvUnderTypars (mkAppTy parentTcref (List.map mkTyparTy ctps))
    if isCompiledAsInstance || isCtor then 
        // Find the 'this' argument type if any 
        let thisTy,flatArgInfos = 
            if isCtor then (GetFSharpViewOfReturnType g returnTy),flatArgInfos 
            else 
               match flatArgInfos with 
               | [] -> error(InternalError("This instance method '" + vref.LogicalName + "' has no arguments", m))
               | (h,_):: t -> h,t

        let thisTy = if isByrefTy g thisTy then destByrefTy g thisTy else thisTy
        let thisArgTys = argsOfAppTy g thisTy
        if ctps.Length <> thisArgTys.Length then
           warning(InternalError(sprintf "CodeGen check: type checking did not quantify the correct number of type variables for this method, #parentTypars = %d, #ctps = %d, #mtps = %d, #thisArgTys = %d" numParentTypars ctps.Length mtps.Length thisArgTys.Length,m))
        else 
           List.iter2
              (fun gtp ty2 -> 
                if not (typeEquiv g (mkTyparTy gtp) ty2) then 
                  warning(InternalError("CodeGen check: type checking did not quantify the correct type variables for this method: generalization list contained " + gtp.Name + "#" + string gtp.Stamp + " and list from 'this' pointer contained " +  (showL(typeL ty2)), m)))
              ctps 
              thisArgTys
        let methodArgTys,paramInfos = List.unzip flatArgInfos
        let ilMethodArgTys = GenParamTypes amap m tyenvUnderTypars methodArgTys
        let ilMethodInst = GenTypeArgs amap m tyenvUnderTypars (List.map mkTyparTy mtps)
        let mspec = mkILInstanceMethSpecInTy (ilTy,vref.CompiledName,ilMethodArgTys,ilActualRetTy,ilMethodInst)
        
        mspec,ctps,mtps,paramInfos,retInfo
    else 
        let methodArgTys,paramInfos = List.unzip flatArgInfos
        let ilMethodArgTys = GenParamTypes amap m tyenvUnderTypars methodArgTys
        let ilMethodInst = GenTypeArgs amap m tyenvUnderTypars (List.map mkTyparTy mtps)
        let mspec = mkILStaticMethSpecInTy (ilTy,vref.CompiledName,ilMethodArgTys,ilActualRetTy,ilMethodInst)
        
        mspec,ctps,mtps,paramInfos,retInfo

// Generate the ILFieldSpec for a top-level value

let ComputeFieldSpecForVal(optIntraAssemblyInfo:IlxGenIntraAssemblyInfo option, isInteractive, g, ilTyForProperty, vspec:Val, nm, m, cloc, ilTy, ilGetterMethRef) = 
    assert vspec.IsCompiledAsTopLevel
    let generate() = GenFieldSpecForStaticField (isInteractive, g, ilTyForProperty, vspec, nm, m, cloc, ilTy)
    match optIntraAssemblyInfo with 
    | None -> generate()
    | Some intraAssemblyInfo -> 
        if vspec.IsMutable && vspec.IsCompiledAsTopLevel && isStructTy g vspec.Type then
            let ok, res = intraAssemblyInfo.StaticFieldInfo.TryGetValue ilGetterMethRef
            if ok then 
                res 
            else 
                let res = generate()
                intraAssemblyInfo.StaticFieldInfo.[ilGetterMethRef] <- res
                res
        else 
            generate()


let IsValCompiledAsMethod g (v:Val) =
    match v.ValReprInfo with 
    | None -> false
    | Some topValInfo -> 
        not (isUnitTy g v.Type && not v.IsMemberOrModuleBinding && not v.IsMutable) &&
        not v.IsCompiledAsStaticPropertyWithoutField  &&
        match GetTopValTypeInFSharpForm g topValInfo v.Type v.Range with 
        | [],[],_,_ when not v.IsMember -> false 
        | _ -> true

// This called via 2 routes.
// (a) ComputeAndAddStorageForLocalTopVal
// (b) ComputeStorageForNonLocalTopVal
//
/// This function decides the storage for the val.
/// The decision is based on arityInfo.
let ComputeStorageForTopVal (amap, g, optIntraAssemblyInfo:IlxGenIntraAssemblyInfo option, isInteractive, optShadowLocal, vref:ValRef, cloc) =

  if isUnitTy g vref.Type  && not vref.IsMemberOrModuleBinding && not vref.IsMutable then  
    Null   
  else
    let topValInfo = 
        match vref.ValReprInfo with 
        | None -> error(InternalError("ComputeStorageForTopVal: no arity found for " + showL(valRefL vref),vref.Range))
        | Some a -> a
        
    let m = vref.Range
    let nm = vref.CompiledName 

    if vref.Deref.IsCompiledAsStaticPropertyWithoutField then 
        let nm = "get_"+nm 
        let tyenvUnderTypars = TypeReprEnv.ForTypars []
        let ilRetTy = GenType amap m tyenvUnderTypars vref.Type
        let typ = mkILTyForCompLoc cloc
        let mspec = mkILStaticMethSpecInTy (typ, nm, [], ilRetTy, [])
    
        StaticProperty (mspec, optShadowLocal)
    else 

        // Determine when a static field is required.
        //
        // REVIEW: This call to GetTopValTypeInFSharpForm is only needed to determine if this is a (type) function or a value
        // We should just look at the arity
        match GetTopValTypeInFSharpForm g topValInfo vref.Type vref.Range with 
        | [],[], returnTy,_ when not vref.IsMember ->
            // Mutable and literal static fields must have stable names and live in the "public" location 
            // See notes on GenFieldSpecForStaticField above. 
            let vspec = vref.Deref
            let ilTy = GenType amap m TypeReprEnv.Empty returnTy (* TypeReprEnv.Empty ok: not a field in a generic class *)
            let ilTyForProperty = mkILTyForCompLoc cloc
            let attribs = vspec.Attribs
            let hasLiteralAttr = HasFSharpAttribute g g.attrib_LiteralAttribute attribs

            let ilTypeRefForProperty = ilTyForProperty.TypeRef
            let ilGetterMethRef = mkILMethRef (ilTypeRefForProperty, ILCallingConv.Static, "get_"+nm, 0, [], ilTy)
            let ilSetterMethRef = mkILMethRef (ilTypeRefForProperty, ILCallingConv.Static, "set_"+nm, 0, [ilTy], ILType.Void)

            let fspec = ComputeFieldSpecForVal(optIntraAssemblyInfo, isInteractive, g, ilTyForProperty, vspec, nm, m, cloc, ilTy, ilGetterMethRef) 

            StaticField (fspec, vref, hasLiteralAttr, ilTyForProperty, nm, ilTy, ilGetterMethRef, ilSetterMethRef, optShadowLocal)
              
        | _ -> 
            match vref.MemberInfo with 
            | Some memberInfo when not vref.IsExtensionMember -> 
                let mspec,_,_,paramInfos,retInfo = GetMethodSpecForMemberVal amap g memberInfo vref
                Method (topValInfo, vref, mspec, m, paramInfos, retInfo) 
            | _ -> 
                let (tps, curriedArgInfos, returnTy, retInfo) = GetTopValTypeInCompiledForm g topValInfo vref.Type m 
                let tyenvUnderTypars = TypeReprEnv.ForTypars tps
                let (methodArgTys,paramInfos) = curriedArgInfos |> List.concat |> List.unzip 
                let ilMethodArgTys = GenParamTypes amap m tyenvUnderTypars methodArgTys
                let ilRetTy = GenReturnType amap m tyenvUnderTypars returnTy
                let ilLocTy = mkILTyForCompLoc cloc
                let ilMethodInst = GenTypeArgs amap m tyenvUnderTypars (List.map mkTyparTy tps)
                let mspec = mkILStaticMethSpecInTy (ilLocTy, nm, ilMethodArgTys, ilRetTy, ilMethodInst)
                Method (topValInfo, vref, mspec, m, paramInfos, retInfo)

let ComputeAndAddStorageForLocalTopVal (amap, g, intraAssemblyFieldTable, isInteractive, optShadowLocal)  cloc (v:Val) eenv =
    let storage = ComputeStorageForTopVal (amap, g, Some intraAssemblyFieldTable, isInteractive, optShadowLocal, mkLocalValRef v, cloc)
    AddStorageForVal g (v,notlazy storage) eenv

let ComputeStorageForNonLocalTopVal amap g cloc modref (v:Val) =
    match v.ValReprInfo with 
    | None -> error(InternalError("ComputeStorageForNonLocalTopVal, expected an arity for " + v.LogicalName,v.Range))
    | Some _ -> ComputeStorageForTopVal (amap, g, None, false, NoShadowLocal, mkNestedValRef modref v, cloc)

let rec ComputeStorageForNonLocalModuleOrNamespaceRef amap g cloc acc (modref:ModuleOrNamespaceRef) (modul:ModuleOrNamespace)  = 
    let acc = 
        (acc, modul.ModuleOrNamespaceType.ModuleAndNamespaceDefinitions) ||> List.fold (fun acc smodul -> 
            ComputeStorageForNonLocalModuleOrNamespaceRef amap g (CompLocForSubModuleOrNamespace cloc smodul) acc (modref.NestedTyconRef smodul) smodul) 

    let acc = 
        (acc, modul.ModuleOrNamespaceType.AllValsAndMembers) ||> Seq.fold (fun acc v -> 
            AddStorageForVal g (v, lazy (ComputeStorageForNonLocalTopVal amap g cloc modref v)) acc) 
    acc

let ComputeStorageForExternalCcu amap g  eenv (ccu:CcuThunk) = 
    if not ccu.IsFSharp then eenv else
    let cloc = CompLocForCcu ccu
    let eenv = 
       List.foldBack
           (fun smodul acc -> 
               let cloc = CompLocForSubModuleOrNamespace cloc smodul
               let modref =  mkNonLocalCcuRootEntityRef ccu smodul
               ComputeStorageForNonLocalModuleOrNamespaceRef amap g cloc acc modref smodul)
           ccu.RootModulesAndNamespaces
           eenv
    let eenv = 
        let eref = ERefNonLocalPreResolved ccu.Contents (mkNonLocalEntityRef ccu [| |])
        (eenv, ccu.Contents.ModuleOrNamespaceType.AllValsAndMembers) ||> Seq.fold (fun acc v -> 
            AddStorageForVal g (v, lazy (ComputeStorageForNonLocalTopVal amap g cloc eref v)) acc) 
    eenv
    
let rec AddBindingsForLocalModuleType allocVal cloc eenv (mty:ModuleOrNamespaceType) = 
    let eenv = List.fold (fun eenv submodul -> AddBindingsForLocalModuleType allocVal (CompLocForSubModuleOrNamespace cloc submodul) eenv submodul.ModuleOrNamespaceType) eenv mty.ModuleAndNamespaceDefinitions 
    let eenv = Seq.fold (fun eenv v -> allocVal cloc v eenv) eenv mty.AllValsAndMembers 
    eenv 

let AddExternalCcusToIlxGenEnv amap g eenv ccus = List.fold (ComputeStorageForExternalCcu amap g) eenv ccus

let AddBindingsForTycon allocVal (cloc:CompileLocation) (tycon:Tycon) eenv  =
    let unrealizedSlots = 
        if tycon.IsFSharpObjectModelTycon
        then tycon.FSharpObjectModelTypeInfo.fsobjmodel_vslots 
        else []
    (eenv,unrealizedSlots) ||> List.fold (fun eenv vref -> allocVal cloc vref.Deref eenv) 

let rec AddBindingsForModuleDefs allocVal (cloc:CompileLocation) eenv  mdefs = 
    List.fold (AddBindingsForModuleDef allocVal cloc) eenv mdefs

and AddBindingsForModuleDef allocVal cloc eenv x = 
    match x with 
    | TMDefRec(_isRec,tycons,mbinds,_) -> 
        (* Virtual don't have 'let' bindings and must be added to the environment *)
        let eenv = List.foldBack (AddBindingsForTycon allocVal cloc) tycons eenv
        let eenv = List.foldBack (AddBindingsForModule allocVal cloc) mbinds eenv
        eenv
    | TMDefLet(bind,_) -> 
        allocVal cloc bind.Var eenv
    | TMDefDo _ -> 
        eenv
    | TMAbstract(ModuleOrNamespaceExprWithSig(mtyp,_,_)) -> 
        AddBindingsForLocalModuleType allocVal cloc eenv  mtyp
    | TMDefs(mdefs) -> 
        AddBindingsForModuleDefs allocVal cloc eenv  mdefs 

and AddBindingsForModule allocVal cloc x eenv = 
    match x with 
    | ModuleOrNamespaceBinding.Binding bind -> 
        allocVal cloc bind.Var eenv
    | ModuleOrNamespaceBinding.Module (mspec, mdef) -> 
        let cloc = 
            if mspec.IsNamespace then cloc 
            else CompLocForFixedModule cloc.clocQualifiedNameOfFile cloc.clocTopImplQualifiedName mspec
        
        AddBindingsForModuleDef allocVal cloc eenv mdef

and AddBindingsForModuleTopVals _g allocVal _cloc eenv vs = 
    List.foldBack allocVal vs eenv


// Put the partial results for a generated fragment (i.e. a part of a CCU generated by FSI) 
// into the stored results for the whole CCU.  
// isIncrementalFragment = true -->  "typed input" 
// isIncrementalFragment = false -->  "#load" 
let AddIncrementalLocalAssemblyFragmentToIlxGenEnv (amap:ImportMap, isIncrementalFragment, g, ccu, fragName, intraAssemblyInfo, eenv, typedImplFiles) = 
    let cloc = CompLocForFragment fragName ccu
    let allocVal = ComputeAndAddStorageForLocalTopVal (amap, g, intraAssemblyInfo, true, NoShadowLocal)
    (eenv, typedImplFiles) ||> List.fold (fun eenv (TImplFile(qname,_,mexpr,_,_)) -> 
        let cloc = { cloc with clocTopImplQualifiedName = qname.Text }
        if isIncrementalFragment then 
            match mexpr with
            | ModuleOrNamespaceExprWithSig(_,mdef,_) -> AddBindingsForModuleDef allocVal cloc eenv mdef
            (* | ModuleOrNamespaceExprWithSig(mtyp,_,m) -> error(Error("don't expect inner defs to have a constraint",m)) *)
        else
            AddBindingsForLocalModuleType allocVal cloc eenv mexpr.Type) 

//--------------------------------------------------------------------------
// Generate debugging marks 
//-------------------------------------------------------------------------- 

let GenILSourceMarker (g: TcGlobals) (m:range) = 
  Some (ILSourceMarker.Create(document=g.memoize_file m.FileIndex,
                              line=m.StartLine,
                              /// NOTE: .NET && VS  measure first column as column 1
                              column= m.StartColumn+1, 
                              endLine= m.EndLine,
                              endColumn=m.EndColumn+1)) 

let GenPossibleILSourceMarker cenv m = 
    if cenv.opts.generateDebugSymbols then 
        GenILSourceMarker cenv.g m 
    else 
        None

//--------------------------------------------------------------------------
// Helpers for merging property definitions
//--------------------------------------------------------------------------

let HashRangeSorted (ht: IDictionary<_, (int * _)>) = 
    [ for KeyValue(_k,v) in ht -> v ] |> List.sortBy fst |> List.map snd 

let MergeOptions m o1 o2 = 
    match o1,o2 with
    | Some x, None | None, Some x -> Some x
    | None, None -> None
    | Some x, Some _ -> 
#if DEBUG
       // This warning fires on some code that also triggers this warning:
       //          warning(Error("The implementation of a specified generic interface required a method implementation not fully supported by F# Interactive. In the unlikely event that the resulting class fails to load then compile the interface type into a statically-compiled DLL and reference it using '#r'",m))
       // The code is OK so we don't print this.
       errorR(InternalError("MergeOptions: two values given",m)) 
#else
       ignore m
#endif
       Some x 

let MergePropertyPair m (pd: ILPropertyDef) pdef = 
    {pd with GetMethod=MergeOptions m pd.GetMethod pdef.GetMethod
             SetMethod=MergeOptions m pd.SetMethod pdef.SetMethod} 

type PropKey = PropKey of string * ILTypes * ILThisConvention

let AddPropertyDefToHash (m:range) (ht:Dictionary<PropKey,(int * ILPropertyDef)>) (pdef: ILPropertyDef) =
    let nm = PropKey(pdef.Name, pdef.Args, pdef.CallingConv)
    if ht.ContainsKey nm then
        let idx,pd = ht.[nm]
        ht.[nm] <- (idx, MergePropertyPair m pd pdef)
    else 
        ht.[nm] <- (ht.Count, pdef)
    

/// Merge a whole group of properties all at once 
let MergePropertyDefs m ilPropertyDefs = 
    let ht = new Dictionary<_,_>(3,HashIdentity.Structural)
    ilPropertyDefs |> List.iter (AddPropertyDefToHash m ht)  
    HashRangeSorted ht

//--------------------------------------------------------------------------
// Buffers for compiling modules. The entire assembly gets compiled via an AssemblyBuilder
//-------------------------------------------------------------------------- 

/// Information collected imperatively for each type definition 
type TypeDefBuilder(tdef, tdefDiscards) = 
    let gmethods   = new ResizeArray<ILMethodDef>(0)
    let gfields    = new ResizeArray<ILFieldDef>(0)
    let gproperties : Dictionary<PropKey,(int * ILPropertyDef)> = new Dictionary<_,_>(3,HashIdentity.Structural)
    let gevents    = new ResizeArray<ILEventDef>(0)
    let gnested    = new TypeDefsBuilder()
    
    member b.Close() = 
        { tdef with 
            Methods = mkILMethods      (tdef.Methods.AsList @ ResizeArray.toList gmethods)
            Fields  = mkILFields      (tdef.Fields.AsList  @ ResizeArray.toList gfields)
            Properties = mkILProperties (tdef.Properties.AsList @ HashRangeSorted gproperties )
            Events     = mkILEvents     (tdef.Events.AsList     @ ResizeArray.toList gevents)
            NestedTypes     = mkILTypeDefs      (tdef.NestedTypes.AsList @ gnested.Close()) }


    member b.AddEventDef(edef) = gevents.Add edef
    member b.AddFieldDef(ilFieldDef) = gfields.Add ilFieldDef
    member b.AddMethodDef(ilMethodDef) = 
        let discard = 
            match tdefDiscards with 
            | Some (mdefDiscard, _) -> mdefDiscard ilMethodDef
            | None -> false
        if not discard then 
            gmethods.Add ilMethodDef
    member b.NestedTypeDefs = gnested
    member b.GetCurrentFields() = gfields |> Seq.readonly

    /// Merge Get and Set property nodes, which we generate independently for F# code 
    /// when we come across their corresponding methods. 
    member b.AddOrMergePropertyDef(pdef,m) = 
        let discard = 
            match tdefDiscards with 
            | Some (_, pdefDiscard) -> pdefDiscard pdef
            | None -> false
        if not discard then 
            AddPropertyDefToHash m gproperties pdef

    member b.PrependInstructionsToSpecificMethodDef(cond,instrs,tag) = 
        match ResizeArray.tryFindIndex cond gmethods with
        | Some idx -> gmethods.[idx] <-  prependInstrsToMethod instrs gmethods.[idx]
        | None -> gmethods.Add(mkILClassCtor (mkMethodBody (false,[],1,nonBranchingInstrsToCode instrs,tag)))


and TypeDefsBuilder() = 
    let tdefs : Internal.Utilities.Collections.HashMultiMap<string, (int * (TypeDefBuilder * bool))> = HashMultiMap(0, HashIdentity.Structural)
    let mutable countDown = System.Int32.MaxValue 

    member b.Close() = 
        //The order we emit type definitions is not deterministic since it is using the reverse of a range from a hash table. We should use an approximation of source order. 
        // Ideally it shouldn't matter which order we use. 
        // However, for some tests FSI generated code appears sensitive to the order, especially for nested types.
        
        [ for (b, eliminateIfEmpty) in HashRangeSorted tdefs do 
              let tdef = b.Close() 
              // Skip the <PrivateImplementationDetails$> type if it is empty
              if not eliminateIfEmpty 
                 || not tdef.NestedTypes.AsList.IsEmpty 
                 || not tdef.Fields.AsList.IsEmpty 
                 || not tdef.Events.AsList.IsEmpty 
                 || not tdef.Properties.AsList.IsEmpty 
                 || not tdef.Methods.AsList.IsEmpty then 
                  yield tdef  ]

    member b.FindTypeDefBuilder(nm) = 
        try tdefs.[nm]  |> snd |> fst
        with :? KeyNotFoundException -> failwith ("FindTypeDefBuilder: " + nm + " not found")

    member b.FindNestedTypeDefsBuilder(path) = 
        List.fold (fun (acc:TypeDefsBuilder) x -> acc.FindTypeDefBuilder(x).NestedTypeDefs) b path

    member b.FindNestedTypeDefBuilder(tref:ILTypeRef) = 
        b.FindNestedTypeDefsBuilder(tref.Enclosing).FindTypeDefBuilder(tref.Name)

    member b.AddTypeDef(tdef:ILTypeDef, eliminateIfEmpty, addAtEnd, tdefDiscards) = 
        let idx = if addAtEnd then (countDown <- countDown - 1; countDown) else tdefs.Count
        tdefs.Add (tdef.Name, (idx, (new TypeDefBuilder(tdef, tdefDiscards), eliminateIfEmpty)))

/// Assembly generation buffers 
type AssemblyBuilder(cenv:cenv) as mgbuf = 
    // The Abstract IL table of types 
    let gtdefs= new TypeDefsBuilder() 
    // The definitions of top level values, as quotations. 
    let mutable reflectedDefinitions : System.Collections.Generic.Dictionary<Tast.Val,(string * int * Expr)> = System.Collections.Generic.Dictionary(HashIdentity.Reference)
    // A memoization table for generating value types for big constant arrays  
    let vtgenerator=
         new MemoizationTable<(CompileLocation * int) , ILTypeSpec>
              ((fun (cloc,size) -> 
                 let name   = CompilerGeneratedName ("T" + string(newUnique()) + "_" + string size + "Bytes") // Type names ending ...$T<unique>_37Bytes
                 let vtdef  = mkRawDataValueTypeDef cenv.g.iltyp_ValueType (name,size,0us)
                 let vtref = NestedTypeRefForCompLoc cloc vtdef.Name 
                 let vtspec = mkILTySpec(vtref,[])
                 let vtdef = {vtdef with Access= ComputeTypeAccess vtref true}
                 mgbuf.AddTypeDef(vtref, vtdef, false, true, None)
                 vtspec), 
               keyComparer=HashIdentity.Structural)

    let mutable explicitEntryPointInfo : ILTypeRef option  = None

    /// static init fields on script modules.
    let mutable scriptInitFspecs : (ILFieldSpec * range) list = []    
    
    member mgbuf.AddScriptInitFieldSpec(fieldSpec,range) =
        scriptInitFspecs <- (fieldSpec,range) :: scriptInitFspecs
        
    /// This initializes the script in #load and fsc command-line order causing their
    /// sideeffects to be executed.
    member mgbuf.AddInitializeScriptsInOrderToEntryPoint() = 
        // Get the entry point and initialized any scripts in order.
        match explicitEntryPointInfo with
        | Some tref ->
            let IntializeCompiledScript(fspec,m) =
                mgbuf.AddExplicitInitToSpecificMethodDef((fun md -> md.IsEntryPoint), tref, fspec, GenPossibleILSourceMarker cenv m, [], [])              
            scriptInitFspecs |> List.iter IntializeCompiledScript
        | None -> ()

     

    member mgbuf.GenerateRawDataValueType(cloc,size) = 
        // Byte array literals require a ValueType of size the required number of bytes.
        // With fsi.exe, S.R.Emit TypeBuilder CreateType has restrictions when a ValueType VT is nested inside a type T, and T has a field of type VT.
        // To avoid this situation, these ValueTypes are generated under the private implementation rather than in the current cloc. [was bug 1532].
        let cloc = CompLocForPrivateImplementationDetails cloc
        vtgenerator.Apply((cloc,size))

    member mgbuf.AddTypeDef(tref:ILTypeRef, tdef, eliminateIfEmpty, addAtEnd, tdefDiscards) = 
        gtdefs.FindNestedTypeDefsBuilder(tref.Enclosing).AddTypeDef(tdef, eliminateIfEmpty, addAtEnd, tdefDiscards)

    member mgbuf.GetCurrentFields(tref:ILTypeRef) =
        gtdefs.FindNestedTypeDefBuilder(tref).GetCurrentFields()

    member mgbuf.AddReflectedDefinition(vspec : Tast.Val,expr) = 
        // preserve order by storing index of item
        let n = reflectedDefinitions.Count
        reflectedDefinitions.Add(vspec, (vspec.CompiledName, n, expr))
   
    member mgbuf.ReplaceNameOfReflectedDefinition(vspec, newName) = 
        match reflectedDefinitions.TryGetValue vspec with
        | true, (name, n, expr) when name <> newName -> reflectedDefinitions.[vspec] <- (newName, n, expr)
        | _ -> ()

    member mgbuf.AddMethodDef(tref:ILTypeRef,ilMethodDef) = 
        gtdefs.FindNestedTypeDefBuilder(tref).AddMethodDef(ilMethodDef)
        if ilMethodDef.IsEntryPoint then 
            explicitEntryPointInfo <- Some(tref)

    member mgbuf.AddExplicitInitToSpecificMethodDef(cond,tref,fspec,sourceOpt,feefee,seqpt) = 
    // Authoring a .cctor with effects forces the cctor for the 'initialization' module by doing a dummy store & load of a field 
    // Doing both a store and load keeps FxCop happier because it thinks the field is useful 
        let instrs = 
            [ yield! (if condition "NO_ADD_FEEFEE_TO_CCTORS" then [] elif condition "ADD_SEQPT_TO_CCTORS"  then seqpt else feefee) // mark start of hidden code
              yield mkLdcInt32 0 
              yield mkNormalStsfld fspec 
              yield mkNormalLdsfld fspec 
              yield AI_pop]   
        gtdefs.FindNestedTypeDefBuilder(tref).PrependInstructionsToSpecificMethodDef(cond,instrs,sourceOpt) 

    member mgbuf.AddEventDef(tref,edef) = 
        gtdefs.FindNestedTypeDefBuilder(tref).AddEventDef(edef)

    member mgbuf.AddFieldDef(tref,ilFieldDef) = 
        gtdefs.FindNestedTypeDefBuilder(tref).AddFieldDef(ilFieldDef)

    member mgbuf.AddOrMergePropertyDef(tref,pdef,m) = 
        gtdefs.FindNestedTypeDefBuilder(tref).AddOrMergePropertyDef(pdef,m)

    member mgbuf.Close() = 
        // old implementation adds new element to the head of list so result was accumulated in reversed order
        let orderedReflectedDefinitions = 
            [for (KeyValue(vspec, (name, n, expr))) in reflectedDefinitions -> n, ((name,vspec), expr)]
            |> List.sortBy (fst >> (~-)) // invert the result to get 'order-by-descending' behavior (items in list are 0..* so we don't need to worry about int.MinValue)
            |> List.map snd
        gtdefs.Close(), orderedReflectedDefinitions
    member mgbuf.cenv = cenv
    member mgbuf.GetExplicitEntryPointInfo() = explicitEntryPointInfo

     

/// Record the types of the things on the evaluation stack. 
/// Used for the few times we have to flush the IL evaluation stack and to compute maxStack. 
type Pushes = ILType list
type Pops = int
let pop (i:int) : Pops = i
let Push tys : Pushes = tys
let Push0 = Push []

let FeeFee (cenv:cenv) = (if cenv.opts.testFlagEmitFeeFeeAs100001 then 100001 else 0x00feefee)
let FeeFeeInstr (cenv:cenv) doc = 
      I_seqpoint (ILSourceMarker.Create(document = doc,
                                        line = FeeFee cenv,
                                        column = 0,
                                        endLine = FeeFee cenv,
                                        endColumn = 0))

/// Buffers for IL code generation
type CodeGenBuffer(m:range,
                   mgbuf: AssemblyBuilder,
                   methodName,
                   alreadyUsedArgs:int,
                   alreadyUsedLocals:int,
                   zapFirstSeqPointToStart:bool) = 

    let locals = new ResizeArray<((string * (Mark * Mark)) list * ILType * bool)>(10)
    let codebuf = new ResizeArray<ILInstr>(200)
    let exnSpecs = new ResizeArray<ILExceptionSpec>(10)

    // Keep track of the current stack so we can spill stuff when we hit a "try" when some stuff
    // is on the stack.        
    let mutable stack : ILType list = []
    let mutable nstack=0
    let mutable maxStack=0
    let mutable seqpoint= None
    
    let codeLabelToPC : Dictionary<ILCodeLabel,int> = new Dictionary<_,_>(10)
    let codeLabelToCodeLabel : Dictionary<ILCodeLabel,ILCodeLabel> = new Dictionary<_,_>(10)
    
    let rec lab2pc n lbl = 
        if n = System.Int32.MaxValue then error(InternalError("recursive label graph",m))
        if codeLabelToCodeLabel.ContainsKey lbl then 
            lab2pc (n+1) codeLabelToCodeLabel.[lbl]
        else
           codeLabelToPC.[lbl] 
    
    let mutable lastSeqPoint = None
    // Add a nop to make way for the first sequence point. There is always such a 
    // sequence point even when zapFirstSeqPointToStart=false
    do if mgbuf.cenv.opts.generateDebugSymbols  then codebuf.Add(AI_nop)

    member cgbuf.DoPushes (pushes: Pushes) = 
        for ty in pushes do 
           stack <- ty :: stack 
           nstack <- nstack + 1
           maxStack <- Operators.max maxStack nstack

    member cgbuf.DoPops (n:Pops) = 
        for i = 0 to n - 1 do
           match stack with
           | [] -> 
               let msg = sprintf "pop on empty stack during code generation, methodName = %s, m = %s" methodName (stringOfRange m)
               System.Diagnostics.Debug.Assert(false, msg)
               warning(InternalError(msg,m))
           | _ :: t -> 
               stack <- t 
               nstack <- nstack - 1

    member cgbuf.GetCurrentStack() = stack
    member cgbuf.AssertEmptyStack() = 
        if not (isNil stack) then 
            let msg = sprintf "stack flush didn't work, or extraneous expressions left on stack before stack restore, methodName = %s, stack = %+A, m = %s" methodName stack (stringOfRange m)
            System.Diagnostics.Debug.Assert(false, msg)
            warning(InternalError(msg,m))
        ()

    member cgbuf.EmitInstr(pops,pushes,i) = 
        cgbuf.DoPops pops
        cgbuf.DoPushes pushes
        codebuf.Add i

    member cgbuf.EmitInstrs (pops,pushes,is) = 
        cgbuf.DoPops pops
        cgbuf.DoPushes pushes
        is |> List.iter codebuf.Add 

    member cgbuf.GetLastSequencePoint() = 
        lastSeqPoint
       
    member private cgbuf.EnsureNopBetweenDebugPoints() = 
        // Always add a nop between sequence points to help .NET get the stepping right
        // Don't do this after a FeeFee marker for hidden code
        if (codebuf.Count > 0 && 
             (match codebuf.[codebuf.Count-1] with 
              | I_seqpoint sm when sm.Line <> FeeFee mgbuf.cenv -> true 
              | _ -> false)) then 
        
            codebuf.Add(AI_nop)

    member cgbuf.EmitSeqPoint(src) = 
        if mgbuf.cenv.opts.generateDebugSymbols then 
            cgbuf.EnsureNopBetweenDebugPoints()

            let attr = GenILSourceMarker mgbuf.cenv.g src
            assert(Option.isSome attr)
            let i = I_seqpoint (Option.get attr)
            codebuf.Add i
            // Save the first sequence point away to snap it to the top of the method
            match seqpoint with 
            | Some _ -> ()
            | None -> seqpoint <- Some i
            // Save the last sequence point away so we can make a decision graph look consistent (i.e. reassert the sequence point at each target)
            lastSeqPoint <- Some src
            
    // For debug code, emit FeeFee breakpoints for hidden code, see http://blogs.msdn.com/jmstall/archive/2005/06/19/FeeFee_SequencePoints.aspx
    member cgbuf.EmitStartOfHiddenCode() = 
        if mgbuf.cenv.opts.generateDebugSymbols && not mgbuf.cenv.opts.localOptimizationsAreOn then 
            let doc = mgbuf.cenv.g.memoize_file m.FileIndex
            codebuf.Add(FeeFeeInstr mgbuf.cenv doc)   

    member cgbuf.EmitExceptionClause(clause) = 
         exnSpecs.Add clause

    member cgbuf.GenerateDelayMark(_nm) = 
         let lab = IL.generateCodeLabel()
         Mark lab

    member cgbuf.SetCodeLabelToCodeLabel(lab1,lab2) = 
#if DEBUG
        if codeLabelToCodeLabel.ContainsKey(lab1) then 
            let msg = sprintf "two values given for label %s, methodName = %s, m = %s" (formatCodeLabel lab1) methodName (stringOfRange m)
            System.Diagnostics.Debug.Assert(false, msg)
            warning(InternalError(msg,m))
#endif
        codeLabelToCodeLabel.[lab1] <- lab2

    member cgbuf.SetCodeLabelToPC(lab,pc) = 
#if DEBUG
        if codeLabelToPC.ContainsKey(lab) then 
            let msg = sprintf "two values given for label %s, methodName = %s, m = %s" (formatCodeLabel lab) methodName (stringOfRange m)
            System.Diagnostics.Debug.Assert(false, msg)
            warning(InternalError(msg,m))
#endif
        codeLabelToPC.[lab] <- pc 

    member cgbuf.SetMark (mark1: Mark, mark2: Mark) = 
        cgbuf.SetCodeLabelToCodeLabel(mark1.CodeLabel, mark2.CodeLabel)
        
    member cgbuf.SetMarkToHere (Mark lab) =  
        cgbuf.SetCodeLabelToPC(lab,codebuf.Count)

    member cgbuf.SetStack(s) = 
        stack <- s 
        nstack <- s.Length

    member cgbuf.Mark(s) = 
        let res = cgbuf.GenerateDelayMark(s)
        cgbuf.SetMarkToHere(res)
        res 

    member cgbuf.mgbuf = mgbuf
    member cgbuf.MethodName = methodName
    member cgbuf.PreallocatedArgCount = alreadyUsedArgs

    member cgbuf.AllocLocal(ranges,ty,isFixed) = 
        let j = locals.Count
        locals.Add((ranges,ty,isFixed))
        j 

    member cgbuf.ReallocLocal(cond,ranges,ty,isFixed) = 
        let j = 
            match ResizeArray.tryFindIndexi cond locals with 
            | Some j -> 
                let (prevRanges,_,isFixed) = locals.[j]
                locals.[j] <- ((ranges@prevRanges),ty,isFixed)
                j             
            | None -> 
                cgbuf.AllocLocal(ranges,ty,isFixed)
        let j = j + alreadyUsedLocals
        j

    member cgbuf.Close() = 
        let instrs = codebuf.ToArray() 
        let instrs = 
            // If we omitted ANY sequence points, then promote the first sequence point to be the first instruction in the
            // method. A bit ugly but .NET debuggers only honour "step into" if the sequence point is the first in the method.
            //
            match seqpoint with 
            | Some(I_seqpoint sp as i) ->
                let i = 
                    if zapFirstSeqPointToStart then 
                        i
                    else
                        // This special dummy sequence point seems to be the magic to indicate that the head of the 
                        // method has no sequence point
                        I_seqpoint (ILSourceMarker.Create(document = sp.Document,
                                                          line = FeeFee mgbuf.cenv,
                                                          column = 0,
                                                          endLine = FeeFee mgbuf.cenv,
                                                          endColumn = 0))

                // Note we use physical equality '==' to compare the instruction objects. Nasty.
                instrs |> Array.mapi (fun idx i2 -> if idx = 0 then i else if i === i2 then AI_nop else i2)
            | _ -> 
                instrs

        let codeLabels =
            let dict = Dictionary.newWithSize (codeLabelToPC.Count + codeLabelToCodeLabel.Count)
            for kvp in codeLabelToPC        do dict.Add(kvp.Key, lab2pc 0 kvp.Key)
            for kvp in codeLabelToCodeLabel do dict.Add(kvp.Key, lab2pc 0 kvp.Key)
            dict
        ResizeArray.toList locals ,
        maxStack,
        codeLabels,
        instrs,
        ResizeArray.toList exnSpecs,
        Option.isSome seqpoint

module CG = 
    let EmitInstr (cgbuf:CodeGenBuffer) pops pushes i = cgbuf.EmitInstr(pops,pushes,i)
    let EmitInstrs (cgbuf:CodeGenBuffer) pops pushes is = cgbuf.EmitInstrs(pops,pushes,is)
    let EmitSeqPoint (cgbuf:CodeGenBuffer) src = cgbuf.EmitSeqPoint(src)
    let GenerateDelayMark (cgbuf:CodeGenBuffer) nm = cgbuf.GenerateDelayMark(nm)
    let SetMark (cgbuf:CodeGenBuffer) m1 m2 = cgbuf.SetMark(m1,m2)
    let SetMarkToHere (cgbuf:CodeGenBuffer) m1 =  cgbuf.SetMarkToHere(m1)
    let SetStack (cgbuf:CodeGenBuffer) s = cgbuf.SetStack(s)
    let GenerateMark (cgbuf:CodeGenBuffer) s = cgbuf.Mark(s)

open CG


//--------------------------------------------------------------------------
// Compile constants 
//-------------------------------------------------------------------------- 

let GenString cenv cgbuf s = 
    CG.EmitInstrs cgbuf (pop 0) (Push [cenv.g.ilg.typ_String]) [ I_ldstr s ]

let GenConstArray cenv (cgbuf:CodeGenBuffer) eenv ilElementType (data:'a[]) (write : ByteBuffer -> 'a -> unit) = 
    let buf = ByteBuffer.Create data.Length
    data |> Array.iter (write buf)
    let bytes = buf.Close()
    let ilArrayType = mkILArr1DTy ilElementType
    if data.Length = 0 then 
        CG.EmitInstrs cgbuf (pop 0) (Push [ilArrayType]) [ mkLdcInt32 (0);  I_newarr (ILArrayShape.SingleDimensional,ilElementType); ]
    else        
        let vtspec = cgbuf.mgbuf.GenerateRawDataValueType(eenv.cloc,bytes.Length)
        let ilFieldName = CompilerGeneratedName ("field" + string(newUnique()))
        let fty = ILType.Value vtspec
        let ilFieldDef = mkILStaticField (ilFieldName,fty, None, Some bytes, ILMemberAccess.Assembly)
        let ilFieldDef = { ilFieldDef with CustomAttrs = mkILCustomAttrs [ cenv.g.DebuggerBrowsableNeverAttribute ] }
        let fspec = mkILFieldSpecInTy (mkILTyForCompLoc eenv.cloc,ilFieldName, fty)
        CountStaticFieldDef()
        cgbuf.mgbuf.AddFieldDef(fspec.EnclosingTypeRef,ilFieldDef) 
        CG.EmitInstrs cgbuf 
          (pop 0)
          (Push [ ilArrayType; ilArrayType; cenv.g.iltyp_RuntimeFieldHandle ])
          [ mkLdcInt32 data.Length
            I_newarr (ILArrayShape.SingleDimensional,ilElementType) 
            AI_dup 
            I_ldtoken (ILToken.ILField fspec) ]            
        CG.EmitInstrs cgbuf 
          (pop 2)
          Push0
          [ mkNormalCall (mkInitializeArrayMethSpec cenv.g) ]


//--------------------------------------------------------------------------
// We normally generate in the context of a "what to do next" continuation
//-------------------------------------------------------------------------- 

type sequel = 
  | EndFilter 
  /// Exit a 'handler' block
  /// The integer says which local to save result in 
  | LeaveHandler of (bool (* finally? *) * int * Mark)  
  /// Branch to the given mark
  | Br of Mark
  | CmpThenBrOrContinue of Pops * ILInstr list
  /// Continue and leave the value on the IL computation stack
  | Continue
  /// The value then do something else
  | DiscardThen of sequel
  /// Return from the method
  | Return
  /// End a scope of local variables. Used at end of 'let' and 'let rec' blocks to get tail recursive setting 
  /// of end-of-scope marks 
  | EndLocalScope of sequel * Mark 
  /// Return from a method whose return type is void
  | ReturnVoid

let discard = DiscardThen Continue
let discardAndReturnVoid = DiscardThen ReturnVoid


//-------------------------------------------------------------------------
// This is the main code generation routine.  It is used to generate 
// the bodies of methods in a couple of places
//------------------------------------------------------------------------- 
 
let CodeGenThen cenv mgbuf (zapFirstSeqPointToStart,entryPointInfo,methodName,eenv,alreadyUsedArgs,alreadyUsedLocals,codeGenFunction,m) = 
    let cgbuf = new CodeGenBuffer(m,mgbuf,methodName,alreadyUsedArgs,alreadyUsedLocals,zapFirstSeqPointToStart)
    let start = CG.GenerateMark cgbuf "mstart"
    let innerVals = entryPointInfo |> List.map (fun (v,kind) -> (v,(kind,start))) 

    (* Call the given code generator *)
    codeGenFunction cgbuf {eenv with withinSEH=false
                                     liveLocals=IntMap.empty()  
                                     innerVals = innerVals}

    let locals,maxStack,lab2pc,code,exnSpecs,hasSequencePoints = cgbuf.Close()
    
    let localDebugSpecs : ILLocalDebugInfo list = 
        locals
        |> List.mapi (fun i (nms,_,_isFixed) -> List.map (fun nm -> (i,nm)) nms)
        |> List.concat
        |> List.map (fun (i,(nm,(start,finish))) -> 
            { Range=(start.CodeLabel, finish.CodeLabel)
              DebugMappings= [{ LocalIndex=i; LocalName=nm }] })

    let ilLocals =
        locals
        |> List.map (fun (infos, ty, isFixed) ->
          let loc = 
            // in interactive environment, attach name and range info to locals to improve debug experience
            if cenv.opts.isInteractive && cenv.opts.generateDebugSymbols then
                match infos with
                | [(nm, (start, finish))] -> mkILLocal ty (Some(nm, start.CodeLabel, finish.CodeLabel))
                // REVIEW: what do these cases represent?
                | _ :: _
                | [] -> mkILLocal ty None 
            // if not interactive, don't bother adding this info
            else
                mkILLocal ty None
          if isFixed then { loc with IsPinned=true } else loc)

    (ilLocals, 
     maxStack,
     lab2pc,
     code,
     exnSpecs,
     localDebugSpecs,
     hasSequencePoints)

let CodeGenMethod cenv mgbuf (zapFirstSeqPointToStart,entryPointInfo,methodName,eenv,alreadyUsedArgs,alreadyUsedLocals,codeGenFunction,m) = 

    let locals,maxStack,lab2pc,instrs,exns,localDebugSpecs,hasSequencePoints = 
      CodeGenThen cenv mgbuf (zapFirstSeqPointToStart,entryPointInfo,methodName,eenv,alreadyUsedArgs,alreadyUsedLocals,codeGenFunction,m)

    let code =  IL.buildILCode methodName lab2pc instrs exns localDebugSpecs
    
    // Attach a source range to the method. Only do this is it has some sequence points, because .NET 2.0/3.5 
    // ILDASM has issues if you emit symbols with a source range but without any sequence points
    let sourceRange = if hasSequencePoints then GenPossibleILSourceMarker cenv m else None

    // The old union erasure phase increased maxstack by 2 since the code pushes some items, we do the same here
    let maxStack = maxStack + 2

    // Build an Abstract IL method     
    instrs, mkILMethodBody (true,locals,maxStack,code, sourceRange)

let StartDelayedLocalScope nm cgbuf =
    let startScope = CG.GenerateDelayMark cgbuf ("start_" + nm) 
    let endScope = CG.GenerateDelayMark cgbuf ("end_" + nm)
    startScope,endScope

let StartLocalScope nm cgbuf =
    let startScope = CG.GenerateMark cgbuf ("start_" + nm) 
    let endScope = CG.GenerateDelayMark cgbuf ("end_" + nm)
    startScope,endScope
  
let LocalScope nm cgbuf (f : (Mark * Mark) -> 'a) : 'a =
    let _,endScope as scopeMarks = StartLocalScope nm cgbuf
    let res = f scopeMarks
    CG.SetMarkToHere cgbuf endScope
    res

let compileSequenceExpressions = true // try (System.Environment.GetEnvironmentVariable("COMPILED_SEQ") <> null) with _ -> false

//-------------------------------------------------------------------------
// Sequence Point Logic
//------------------------------------------------------------------------- 

type EmitSequencePointState = 
    /// Indicates that we need a sequence point at first opportunity. Used on entrance to a method
    /// and whenever we drop into an expression within the stepping control structure.
    | SPAlways 
    | SPSuppress

/// Determines what sequence point should be emitted for a binding
let ComputeSequencePointInfoForBinding g (TBind(vspec,e,spBind)) = 
    if IsValCompiledAsMethod g vspec then 
        false, None, SPSuppress
    else
        match spBind, stripExpr e with 
        | NoSequencePointAtInvisibleBinding, _ -> false, None, SPSuppress
        | NoSequencePointAtStickyBinding, _ -> true, None, SPSuppress
        | NoSequencePointAtDoBinding, _ -> false, None, SPAlways
        | NoSequencePointAtLetBinding, _ -> false, None, SPSuppress
        // Don't emit sequence points for lambdas.
        // SEQUENCE POINT REVIEW: don't emit for lazy either, nor any builder expressions
        | _, (Expr.Lambda _ | Expr.TyLambda _) -> false, None, SPSuppress
        | SequencePointAtBinding m,_ -> false, Some m, SPSuppress

 
/// Determines if a sequence will be emitted when we generate the code for a binding
let BindingEmitsSequencePoint g bind = 
    match ComputeSequencePointInfoForBinding g bind with
    | _, None, SPSuppress -> false
    | _ -> true

/// Determines if any code at all will be emitted for a binding
let BindingEmitsNoCode g (TBind(vspec,_,_)) = IsValCompiledAsMethod g vspec 

let bindHasSeqPt = function (TBind(_,_,SequencePointAtBinding _)) -> true | _ -> false
let bindIsInvisible = function (TBind(_,_,NoSequencePointAtInvisibleBinding _)) -> true | _ -> false

    
// This determines if we're about to emit a sequence point as the first emitted code for an expression.
// It determines if expression code generation certainly starts with a sequence point. An approximation used
// to prevent the generation of duplicate sequence points for conditionals and pattern matching
let rec FirstEmittedCodeWillBeSequencePoint g sp expr = 
    match sp with 
    | SPAlways -> 
        match stripExpr expr with 
        | Expr.Let (bind,body,_,_) -> 
            BindingEmitsSequencePoint g bind || 
            FirstEmittedCodeWillBeSequencePoint g sp bind.Expr || 
            (BindingEmitsNoCode g bind && FirstEmittedCodeWillBeSequencePoint g sp body)
        | Expr.LetRec(binds,body,_,_) -> 
            binds |> List.exists (BindingEmitsSequencePoint g) || 
            (binds |> List.forall (BindingEmitsNoCode g) && FirstEmittedCodeWillBeSequencePoint g sp body)
        | Expr.Sequential (_, _, NormalSeq,spSeq,_) -> 
            match spSeq with 
            | SequencePointsAtSeq -> true
            | SuppressSequencePointOnExprOfSequential -> true
            | SuppressSequencePointOnStmtOfSequential -> false
        | Expr.Match (SequencePointAtBinding _,_,_,_,_,_)   -> true
        | Expr.Op((  TOp.TryCatch (SequencePointAtTry _,_) 
                    | TOp.TryFinally (SequencePointAtTry _,_) 
                    | TOp.For (SequencePointAtForLoop _,_) 
                    | TOp.While (SequencePointAtWhileLoop _,_)),_,_,_) -> true
        | _ -> false

     | SPSuppress -> 
        false                  

/// Suppress sequence points in some cases even though "SPAlways" is set.  In most
/// cases this is for code-generated constructs that form part of the compilation of a larger 
/// construct.
///
/// Note this is only used when FirstEmittedCodeWillBeSequencePoint is false.
let AlwaysSuppressSequencePoint g sp expr = 
    assert (not(FirstEmittedCodeWillBeSequencePoint g sp expr))
    match sp with 
    | SPAlways -> 
        match expr with 
        
        // We suppress sequence points at invisible let bindings even if they are requested by SPAlways.
        | Expr.Let (bind,_,_,_) when bindIsInvisible bind -> true
        | Expr.LetRec(binds,_,_,_) when (binds |> List.exists bindHasSeqPt) || (binds |> List.forall bindIsInvisible) -> true

        // We always suppress at sequential where the sequence point is missing. We need to document better why.
        | Expr.Sequential _ -> true 

        // We always suppress at labels and gotos, it makes no sense to emit sequence points at these
        | Expr.Op(TOp.Label _,_,_,_) -> true
        | Expr.Op(TOp.Goto _,_,_,_) -> true

        // We always suppress at 'match'/'try'/... where the sequence point is missing (if the sequence point was
        // present then FirstEmittedCodeWillBeSequencePoint would have returned true).  
        //
        // These cases need more looking into. For example, a typical 'match' gets compiled to 
        //    let tmp = expr   // generates a sequence point, BEFORE tmp is evaluated
        //    match tmp with  // a match marked with NoSequencePointAtInvisibleLetBinding
        // So since the 'let tmp = expr' has a sequence point, then no sequence point is needed for the 'match'. But the processing
        // of the 'let' requests SPAlways for the body.
        | Expr.Match _ -> true
        | Expr.Op(TOp.TryCatch _,_,_,_) -> true
        | Expr.Op(TOp.TryFinally _,_,_,_) -> true
        | Expr.Op(TOp.For _,_,_,_)  -> true
        | Expr.Op(TOp.While _,_,_,_) -> true
        | _ -> false
    | SPSuppress -> 
        true


/// Some expressions must emit some preparation code, then emit the actual code.  
let rec RangeOfEventualEmittedSequencePoint g expr = 
    match stripExpr expr with 
    | Expr.Let (bind,body,_,_) ->
        match ComputeSequencePointInfoForBinding g bind with
        | true, _, _ -> expr.Range  // for sticky bindings, prefer the range of the overall expression
        | _, None, SPSuppress -> RangeOfEventualEmittedSequencePoint g body 
        | _, Some m, _ -> m
        | _, None, SPAlways -> RangeOfEventualEmittedSequencePoint g bind.Expr
    | Expr.LetRec(_,body,_,_)  -> RangeOfEventualEmittedSequencePoint g body 
    | Expr.Sequential (expr1, _, NormalSeq, _, _) -> RangeOfEventualEmittedSequencePoint g expr1
    | _ -> expr.Range

let DoesGenExprStartWithSequencePoint g  sp expr = 
    FirstEmittedCodeWillBeSequencePoint g sp expr || not (AlwaysSuppressSequencePoint g sp expr)


//-------------------------------------------------------------------------
// Generate expressions
//------------------------------------------------------------------------- 

let rec GenExpr (cenv:cenv) (cgbuf:CodeGenBuffer) eenv sp expr sequel =

  let expr =  stripExpr expr

  if not (FirstEmittedCodeWillBeSequencePoint cenv.g sp expr) && not (AlwaysSuppressSequencePoint cenv.g sp expr) then 
      CG.EmitSeqPoint cgbuf (RangeOfEventualEmittedSequencePoint cenv.g expr)

  match (if compileSequenceExpressions then LowerCallsAndSeqs.LowerSeqExpr cenv.g cenv.amap expr else None) with
  | Some info ->
      GenSequenceExpr cenv cgbuf eenv info sequel
  | None ->

  match expr with 
  | Expr.Const(c,m,ty) -> 
      GenConstant cenv cgbuf eenv (c,m,ty) sequel
  | Expr.Match (spBind,exprm,tree,targets,m,ty) -> 
      GenMatch cenv cgbuf eenv (spBind,exprm,tree,targets,m,ty) sequel
  | Expr.Sequential(e1,e2,dir,spSeq,m) ->  
      GenSequential cenv cgbuf eenv sp (e1,e2,dir,spSeq,m) sequel
  | Expr.LetRec (binds,body,m,_)  -> 
      GenLetRec cenv cgbuf eenv (binds,body,m) sequel
  | Expr.Let (bind,body,_,_)  -> 
     // This case implemented here to get a guaranteed tailcall 
     // Make sure we generate the sequence point outside the scope of the variable
     let startScope,endScope as scopeMarks = StartDelayedLocalScope "let" cgbuf
     let eenv = AllocStorageForBind cenv cgbuf scopeMarks eenv bind
     let spBind = GenSequencePointForBind cenv cgbuf bind
     CG.SetMarkToHere cgbuf startScope 
     GenBindAfterSequencePoint cenv cgbuf eenv spBind bind

     // Work out if we need a sequence point for the body. For any "user" binding then the body gets SPAlways.
     // For invisible compiler-generated bindings we just use "sp", unless its body is another invisible binding
     // For sticky bindings arising from inlining we suppress any immediate sequence point in the body
     let spBody = 
        match bind.SequencePointInfo with 
        | SequencePointAtBinding _ 
        | NoSequencePointAtLetBinding 
        | NoSequencePointAtDoBinding -> SPAlways
        | NoSequencePointAtInvisibleBinding -> sp
        | NoSequencePointAtStickyBinding -> SPSuppress
        
     // Generate the body
     GenExpr cenv cgbuf eenv spBody body (EndLocalScope(sequel,endScope)) 

  | Expr.Lambda _  | Expr.TyLambda _  -> 
      GenLambda cenv cgbuf eenv false None expr sequel
  | Expr.App(Expr.Val(vref, _, m) as v, _, tyargs, [], _) when 
        List.forall (isMeasureTy cenv.g) tyargs &&
        (
            // inline only values that are stored in local variables
            match StorageForValRef m vref eenv with 
            | ValStorage.Local _ -> true 
            | _ -> false
        )   ->
      // application of local type functions with type parameters = measure types and body = local value - inine the body
      GenExpr cenv cgbuf eenv sp v sequel
  | Expr.App(f,fty,tyargs,args,m) -> 
      GenApp cenv cgbuf eenv (f,fty,tyargs,args,m) sequel
  | Expr.Val(v,_,m) -> 
      GenGetVal cenv cgbuf eenv (v,m) sequel
  | Expr.Op(op,tyargs,args,m) -> 
      begin match op,args,tyargs with 
      | TOp.ExnConstr(c),_,_      -> 
          GenAllocExn cenv cgbuf eenv (c,args,m) sequel
      | TOp.UnionCase(c),_,_        -> 
          GenAllocUnionCase cenv cgbuf eenv (c,tyargs,args,m) sequel
      | TOp.Recd(isCtor,tycon),_,_ -> 
          GenAllocRecd cenv cgbuf eenv isCtor (tycon,tyargs,args,m) sequel
      | TOp.TupleFieldGet (tupInfo,n),[e],_ -> 
          GenGetTupleField cenv cgbuf eenv (tupInfo,e,tyargs,n,m) sequel
      | TOp.ExnFieldGet(ecref,n),[e],_ -> 
          GenGetExnField cenv cgbuf eenv (e,ecref,n,m) sequel
      | TOp.UnionCaseFieldGet(ucref,n),[e],_ -> 
          GenGetUnionCaseField cenv cgbuf eenv (e,ucref,tyargs,n,m) sequel
      | TOp.UnionCaseFieldGetAddr(ucref,n),[e],_ -> 
          GenGetUnionCaseFieldAddr cenv cgbuf eenv (e,ucref,tyargs,n,m) sequel
      | TOp.UnionCaseTagGet ucref,[e],_ -> 
          GenGetUnionCaseTag cenv cgbuf eenv (e,ucref,tyargs,m) sequel
      | TOp.UnionCaseProof ucref,[e],_ -> 
          GenUnionCaseProof cenv cgbuf eenv (e,ucref,tyargs,m) sequel
      | TOp.ExnFieldSet(ecref,n),[e;e2],_ -> 
          GenSetExnField cenv cgbuf eenv (e,ecref,n,e2,m) sequel 
      | TOp.UnionCaseFieldSet(ucref,n),[e;e2],_ -> 
          GenSetUnionCaseField cenv cgbuf eenv (e,ucref,tyargs,n,e2,m) sequel
      | TOp.ValFieldGet f,[e],_ -> 
         GenGetRecdField cenv cgbuf eenv (e,f,tyargs,m) sequel
      | TOp.ValFieldGet f,[],_ -> 
         GenGetStaticField cenv cgbuf eenv (f,tyargs,m) sequel
      | TOp.ValFieldGetAddr f,[e],_ -> 
         GenGetRecdFieldAddr cenv cgbuf eenv (e,f,tyargs,m) sequel
      | TOp.ValFieldGetAddr f,[],_ -> 
         GenGetStaticFieldAddr cenv cgbuf eenv (f,tyargs,m) sequel
      | TOp.ValFieldSet f,[e1;e2],_ -> 
         GenSetRecdField cenv cgbuf eenv (e1,f,tyargs,e2,m) sequel
      | TOp.ValFieldSet f,[e2],_ -> 
         GenSetStaticField cenv cgbuf eenv (f,tyargs,e2,m) sequel
      | TOp.Tuple tupInfo,_,_ -> 
         GenAllocTuple cenv cgbuf eenv (tupInfo,args,tyargs,m) sequel
      | TOp.ILAsm(code,returnTys),_,_ ->  
         GenAsmCode cenv cgbuf eenv (code,tyargs,args,returnTys,m) sequel 
      | TOp.While (sp,_),[Expr.Lambda(_,_,_,[_],e1,_,_);Expr.Lambda(_,_,_,[_],e2,_,_)],[]  -> 
         GenWhileLoop cenv cgbuf eenv (sp,e1,e2,m) sequel 
      | TOp.For(spStart,dir),[Expr.Lambda(_,_,_,[_],e1,_,_);Expr.Lambda(_,_,_,[_],e2,_,_);Expr.Lambda(_,_,_,[v],e3,_,_)],[]  -> 
         GenForLoop cenv cgbuf eenv (spStart,v,e1,dir,e2,e3,m) sequel
      | TOp.TryFinally(spTry,spFinally),[Expr.Lambda(_,_,_,[_],e1,_,_); Expr.Lambda(_,_,_,[_],e2,_,_)],[resty] -> 
         GenTryFinally cenv cgbuf eenv (e1,e2,m,resty,spTry,spFinally) sequel
      | TOp.TryCatch(spTry,spWith),[Expr.Lambda(_,_,_,[_],e1,_,_); Expr.Lambda(_,_,_,[vf],ef,_,_);Expr.Lambda(_,_,_,[vh],eh,_,_)],[resty] -> 
         GenTryCatch cenv cgbuf eenv (e1,vf,ef,vh,eh,m,resty,spTry,spWith) sequel
      | TOp.ILCall(virt,_,valu,newobj,valUseFlags,_,isDllImport,ilMethRef,enclArgTys,methArgTys,returnTys),args,[] -> 
         GenILCall cenv cgbuf eenv (virt,valu,newobj,valUseFlags,isDllImport,ilMethRef,enclArgTys,methArgTys,args,returnTys,m) sequel
      | TOp.RefAddrGet,[e],[ty]       -> GenGetAddrOfRefCellField cenv cgbuf eenv (e,ty,m) sequel
      | TOp.Coerce,[e],[tgty;srcty]    -> GenCoerce cenv cgbuf eenv (e,tgty,m,srcty) sequel
      | TOp.Reraise,[],[rtnty]         -> GenReraise cenv cgbuf eenv (rtnty,m) sequel
      | TOp.TraitCall(ss),args,[] -> GenTraitCall cenv cgbuf eenv (ss,args, m) expr sequel
      | TOp.LValueOp(LSet,v),[e],[]      -> GenSetVal cenv cgbuf eenv (v,e,m) sequel
      | TOp.LValueOp(LByrefGet,v),[],[]  -> GenGetByref cenv cgbuf eenv (v,m) sequel
      | TOp.LValueOp(LByrefSet,v),[e],[] -> GenSetByref cenv cgbuf eenv (v,e,m) sequel
      | TOp.LValueOp(LGetAddr,v),[],[]   -> GenGetValAddr cenv cgbuf eenv (v,m) sequel
      | TOp.Array,elems,[elemTy] ->  GenNewArray cenv cgbuf eenv (elems,elemTy,m) sequel
      | TOp.Bytes bytes,[],[] -> 
          if cenv.opts.emitConstantArraysUsingStaticDataBlobs then 
              GenConstArray cenv cgbuf eenv cenv.g.ilg.typ_Byte bytes (fun buf b -> buf.EmitByte b)
              GenSequel cenv eenv.cloc cgbuf sequel
          else
              GenNewArraySimple cenv cgbuf eenv (List.ofArray (Array.map (mkByte cenv.g m) bytes),cenv.g.byte_ty,m) sequel
      | TOp.UInt16s arr,[],[] -> 
          if cenv.opts.emitConstantArraysUsingStaticDataBlobs then 
              GenConstArray cenv cgbuf eenv cenv.g.ilg.typ_UInt16 arr (fun buf b -> buf.EmitUInt16 b)
              GenSequel cenv eenv.cloc cgbuf sequel
          else
              GenNewArraySimple cenv cgbuf eenv (List.ofArray (Array.map (mkUInt16 cenv.g m) arr),cenv.g.uint16_ty,m) sequel
      | TOp.Goto(label),_,_ ->  
          if cgbuf.mgbuf.cenv.opts.generateDebugSymbols then 
             cgbuf.EmitStartOfHiddenCode()
             CG.EmitInstr cgbuf (pop 0) Push0 AI_nop
          CG.EmitInstr cgbuf (pop 0) Push0 (I_br label)
          // NOTE: discard sequel
      | TOp.Return,[e],_ ->  
         GenExpr cenv cgbuf eenv SPSuppress e Return
         // NOTE: discard sequel
      | TOp.Return,[],_ ->  
         GenSequel cenv eenv.cloc cgbuf ReturnVoid
         // NOTE: discard sequel
      | TOp.Label(label),_,_ ->  
         cgbuf.SetMarkToHere (Mark label) 
         GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel
      | _ -> error(InternalError("Unexpected operator node expression",expr.Range))
     end 
  | Expr.StaticOptimization(constraints,e2,e3,m) -> 
      GenStaticOptimization cenv cgbuf eenv (constraints,e2,e3,m) sequel
  | Expr.Obj(_,typ,_,_,[meth],[],m) when isDelegateTy cenv.g typ -> 
      GenDelegateExpr cenv cgbuf eenv expr (meth,m) sequel
  | Expr.Obj(_,typ,basev,basecall,overrides,interfaceImpls,m) -> 
      GenObjectExpr cenv cgbuf eenv expr (typ,basev,basecall,overrides,interfaceImpls,m)  sequel

  | Expr.Quote(ast,conv,_,m,ty) -> GenQuotation cenv cgbuf eenv (ast,conv,m,ty) sequel
  | Expr.Link _ -> failwith "Unexpected reclink"
  | Expr.TyChoose (_,_,m) -> error(InternalError("Unexpected Expr.TyChoose",m))

and GenExprs cenv cgbuf eenv es = 
    List.iter (fun e -> GenExpr cenv cgbuf eenv SPSuppress e Continue) es

and CodeGenMethodForExpr cenv mgbuf (spReq,entryPointInfo,methodName,eenv,alreadyUsedArgs,alreadyUsedLocals,expr0,sequel0) = 
    let zapFirstSeqPointToStart = (spReq = SPAlways)
    let _,code = 
        CodeGenMethod cenv mgbuf (zapFirstSeqPointToStart,entryPointInfo,methodName,eenv,alreadyUsedArgs,alreadyUsedLocals,
                                   (fun cgbuf eenv -> GenExpr cenv cgbuf eenv spReq expr0 sequel0),
                                   expr0.Range)
    code                                   



//--------------------------------------------------------------------------
// Generate sequels
//-------------------------------------------------------------------------- 

(* does the sequel discard its result, and if so what does it do next? *)
and sequelAfterDiscard sequel = 
  match sequel with 
   | DiscardThen sequel -> Some(sequel)
   | EndLocalScope(sq,mark) -> sequelAfterDiscard sq |> Option.map (fun sq -> EndLocalScope(sq,mark))
   | _ -> None

and sequelIgnoringEndScopesAndDiscard sequel =
    let sequel = sequelIgnoreEndScopes sequel
    match sequelAfterDiscard sequel with 
    | Some sq -> sq
    | None ->  sequel 

and sequelIgnoreEndScopes  sequel = 
    match sequel with 
    | EndLocalScope(sq,_) -> sequelIgnoreEndScopes sq
    | sq -> sq

(* commit any 'EndLocalScope' nodes in the sequel and return the residue *)
and GenSequelEndScopes cgbuf sequel =
    match sequel with 
    | EndLocalScope(sq,m) -> CG.SetMarkToHere cgbuf m; GenSequelEndScopes cgbuf sq
    | _ -> ()

and StringOfSequel sequel =
    match sequel with
    | Continue -> "continue"
    | DiscardThen sequel -> "discard; " + StringOfSequel sequel
    | ReturnVoid -> "ReturnVoid"
    | CmpThenBrOrContinue _ -> "CmpThenBrOrContinue"
    | Return -> "Return"
    | EndLocalScope (sq,Mark k) -> "EndLocalScope(" + StringOfSequel sq + "," + formatCodeLabel k + ")"
    | Br (Mark x) -> sprintf "Br L%s" (formatCodeLabel x)
    | LeaveHandler _ -> "LeaveHandler"
    | EndFilter -> "EndFilter"

and GenSequel cenv cloc cgbuf sequel =
  let sq = sequelIgnoreEndScopes sequel
  (match sq with 
  | Continue -> ()
  | DiscardThen sq -> 
      CG.EmitInstr cgbuf (pop 1) Push0 AI_pop
      GenSequel cenv cloc cgbuf sq 
  | ReturnVoid ->
      CG.EmitInstr cgbuf (pop 0) Push0 I_ret 
  | CmpThenBrOrContinue(pops,bri) ->
      CG.EmitInstrs cgbuf pops Push0 bri
  | Return -> 
      CG.EmitInstr cgbuf (pop 1) Push0 I_ret 
  | EndLocalScope _ -> failwith "EndLocalScope unexpected"
  | Br x -> 
      // Emit a NOP in debug code in case the branch instruction gets eliminated 
      // because it is a "branch to next instruction". This prevents two unrelated sequence points
      // (the one before the branch and the one after) being coalesced together
      if cgbuf.mgbuf.cenv.opts.generateDebugSymbols then 
         cgbuf.EmitStartOfHiddenCode()
         CG.EmitInstr cgbuf (pop 0) Push0 AI_nop
      CG.EmitInstr cgbuf (pop 0) Push0 (I_br x.CodeLabel)  
  | LeaveHandler (isFinally, whereToSaveResult,x) ->
      if isFinally then 
        CG.EmitInstr cgbuf (pop 1) Push0 AI_pop
      else
        EmitSetLocal cgbuf whereToSaveResult
      CG.EmitInstr cgbuf (pop 0) Push0 (if isFinally then I_endfinally else I_leave(x.CodeLabel))
  | EndFilter ->
      CG.EmitInstr cgbuf (pop 1) Push0 I_endfilter
  )
  GenSequelEndScopes cgbuf sequel


//--------------------------------------------------------------------------
// Generate constants
//-------------------------------------------------------------------------- 

and GenConstant cenv cgbuf eenv (c,m,ty) sequel =
  let ilTy = GenType cenv.amap m eenv.tyenv ty
  // Check if we need to generate the value at all
  match sequelAfterDiscard sequel with 
  | None -> 
      match TryEliminateDesugaredConstants cenv.g m c with 
      | Some e -> 
          GenExpr cenv cgbuf eenv SPSuppress e Continue
      | None ->
          match c with 
          | Const.Bool b -> CG.EmitInstr cgbuf (pop 0) (Push [cenv.g.ilg.typ_Bool]) (mkLdcInt32 (if b then 1 else 0))
          | Const.SByte i -> CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (mkLdcInt32 (int32 i))
          | Const.Int16 i -> CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (mkLdcInt32 (int32 i))
          | Const.Int32 i -> CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (mkLdcInt32 i)
          | Const.Int64 i -> CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (iLdcInt64 i)
          | Const.IntPtr i -> CG.EmitInstrs cgbuf (pop 0) (Push [ilTy]) [iLdcInt64 i; AI_conv DT_I ]
          | Const.Byte i -> CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (mkLdcInt32 (int32 i))
          | Const.UInt16 i -> CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (mkLdcInt32 (int32 i))
          | Const.UInt32 i -> CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (mkLdcInt32 (int32 i))
          | Const.UInt64 i -> CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (iLdcInt64 (int64 i))
          | Const.UIntPtr i -> CG.EmitInstrs cgbuf (pop 0) (Push [ilTy]) [iLdcInt64 (int64 i); AI_conv DT_U ]
          | Const.Double f -> CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (AI_ldc (DT_R8,ILConst.R8 f))
          | Const.Single f -> CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (AI_ldc (DT_R4,ILConst.R4 f))
          | Const.Char(c) -> CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) ( mkLdcInt32 (int c))
          | Const.String(s) -> GenString cenv cgbuf s
          | Const.Unit -> GenUnit cenv eenv m cgbuf
          | Const.Zero -> GenDefaultValue cenv cgbuf eenv (ty,m) 
          | Const.Decimal _ -> failwith "unreachable"
      GenSequel cenv eenv.cloc cgbuf sequel
  | Some sq -> 
      // Even if we didn't need to generate the value then maybe we still have to branch or return 
      GenSequel cenv eenv.cloc cgbuf sq

and GenUnitTy cenv eenv m = 
    match cenv.ilUnitTy with 
    | None -> 
        let res = GenType cenv.amap m eenv.tyenv cenv.g.unit_ty 
        cenv.ilUnitTy <- Some res
        res
    | Some res ->  res

and GenUnit cenv eenv m cgbuf  = 
    CG.EmitInstr cgbuf (pop 0) (Push [GenUnitTy cenv eenv m]) AI_ldnull

and GenUnitThenSequel cenv eenv m cloc cgbuf sequel =
    match sequelAfterDiscard sequel with 
    | Some(sq) -> GenSequel cenv cloc cgbuf sq
    | None -> GenUnit cenv eenv m cgbuf; GenSequel cenv cloc cgbuf sequel


//--------------------------------------------------------------------------
// Generate simple data-related constructs
//-------------------------------------------------------------------------- 

and GenAllocTuple cenv cgbuf eenv (tupInfo, args,argtys,m) sequel =

    let tupInfo = evalTupInfoIsStruct tupInfo
    let tcref, tys, args, newm = mkCompiledTuple cenv.g tupInfo (argtys,args,m)
    let typ = GenNamedTyApp cenv.amap newm eenv.tyenv tcref tys
    let ntyvars = if (tys.Length - 1) < goodTupleFields then (tys.Length - 1) else goodTupleFields
    let formalTyvars = [ for n in 0 .. ntyvars do yield mkILTyvarTy (uint16 n) ]

    GenExprs cenv cgbuf eenv args
    // Generate a reference to the constructor 
    CG.EmitInstr cgbuf (pop args.Length) (Push [typ])
      (mkNormalNewobj 
          (mkILCtorMethSpecForTy (typ,formalTyvars)))
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetTupleField cenv cgbuf eenv (tupInfo,e,tys,n,m) sequel =
    let tupInfo = evalTupInfoIsStruct tupInfo
    let rec getCompiledTupleItem g (e,tys:TTypes,n,m) =
        let ar = tys.Length
        if ar <= 0 then failwith "getCompiledTupleItem"
        elif ar < maxTuple then
            let tcr' = mkCompiledTupleTyconRef g tupInfo ar
            let typ = GenNamedTyApp cenv.amap m eenv.tyenv tcr' tys
            mkGetTupleItemN g m n typ tupInfo e tys.[n]
        else
            let tysA,tysB = List.splitAfter (goodTupleFields) tys
            let tyB = mkCompiledTupleTy g tupInfo tysB
            let tys' = tysA@[tyB]
            let tcr' = mkCompiledTupleTyconRef g tupInfo (List.length tys')
            let typ' = GenNamedTyApp cenv.amap m eenv.tyenv tcr' tys'
            let n' = (min n goodTupleFields)
            let elast = mkGetTupleItemN g m n' typ' tupInfo e tys'.[n']
            if n < goodTupleFields then
                elast
            else
                getCompiledTupleItem g (elast,tysB,n-goodTupleFields,m)
    GenExpr cenv cgbuf eenv SPSuppress (getCompiledTupleItem cenv.g (e,tys,n,m)) sequel

and GenAllocExn cenv cgbuf eenv (c,args,m) sequel =
    GenExprs cenv cgbuf eenv args
    let typ = GenExnType cenv.amap m eenv.tyenv c
    let flds = recdFieldsOfExnDefRef c
    let argtys = flds |> List.map (fun rfld -> GenType cenv.amap m eenv.tyenv rfld.FormalType) 
    let mspec = mkILCtorMethSpecForTy (typ, argtys)
    CG.EmitInstr cgbuf
      (pop args.Length) (Push [typ])
      (mkNormalNewobj mspec) 
    GenSequel cenv eenv.cloc cgbuf sequel

and GenAllocUnionCase cenv cgbuf eenv  (c,tyargs,args,m) sequel =
    GenExprs cenv cgbuf eenv args
    let cuspec,idx = GenUnionCaseSpec cenv.amap m eenv.tyenv c tyargs
    CG.EmitInstrs cgbuf (pop args.Length) (Push [cuspec.EnclosingType]) (EraseUnions.mkNewData cenv.g.ilg (cuspec, idx))
    GenSequel cenv eenv.cloc cgbuf sequel

and GenAllocRecd cenv cgbuf eenv ctorInfo (tcref,argtys,args,m) sequel =
    let typ = GenNamedTyApp cenv.amap m eenv.tyenv tcref argtys

    // Filter out fields with default initialization 
    let relevantFields = 
        tcref.AllInstanceFieldsAsList
        |> List.filter (fun f -> not f.IsZeroInit)
        |> List.filter (fun f -> not f.IsCompilerGenerated)

    match ctorInfo with 
    | RecdExprIsObjInit  -> 
        (args,relevantFields) ||> List.iter2 (fun e f -> 
                CG.EmitInstr cgbuf (pop 0) (Push (if tcref.IsStructOrEnumTycon then [ILType.Byref typ] else [typ])) mkLdarg0 
                GenExpr cenv cgbuf eenv SPSuppress e Continue
                GenFieldStore false cenv cgbuf eenv (tcref.MakeNestedRecdFieldRef f,argtys,m) discard) 
        // Object construction doesn't generate a true value. 
        // Object constructions will always just get thrown away so this is safe 
        GenSequel cenv eenv.cloc cgbuf sequel
    | RecdExpr -> 
        GenExprs cenv cgbuf eenv args
        // generate a reference to the record constructor 
        let tyenvinner = TypeReprEnv.ForTyconRef tcref
        CG.EmitInstr cgbuf (pop args.Length) (Push [typ])
          (mkNormalNewobj 
             (mkILCtorMethSpecForTy (typ,relevantFields |> List.map (fun f -> GenType cenv.amap m tyenvinner f.FormalType) )))
        GenSequel cenv eenv.cloc cgbuf sequel


and GenNewArraySimple cenv cgbuf eenv (elems,elemTy,m) sequel =
    let ilElemTy = GenType cenv.amap m eenv.tyenv elemTy
    let ilArrTy = mkILArr1DTy ilElemTy
    
    CG.EmitInstrs cgbuf (pop 0) (Push [ilArrTy]) [ (AI_ldc (DT_I4,ILConst.I4 (elems.Length))); I_newarr (ILArrayShape.SingleDimensional,ilElemTy) ]
    elems |> List.iteri (fun i e ->             
        CG.EmitInstrs cgbuf (pop 0) (Push [ilArrTy; cenv.g.ilg.typ_Int32]) [ AI_dup; (AI_ldc (DT_I4,ILConst.I4  i)) ]
        GenExpr cenv cgbuf eenv SPSuppress e Continue          
        CG.EmitInstr cgbuf (pop 3) Push0  (I_stelem_any (ILArrayShape.SingleDimensional,ilElemTy))) 
      
    GenSequel cenv eenv.cloc cgbuf sequel

and GenNewArray cenv cgbuf eenv (elems: Expr list,elemTy,m) sequel =
  // REVIEW: The restriction against enum types here has to do with Dev10/Dev11 bug 872799
  // GenConstArray generates a call to RuntimeHelpers.InitializeArray.  On CLR 2.0/x64 and CLR 4.0/x64/x86,
  // InitializeArray is a JIT intrinsic that will result in invalid runtime CodeGen when initializing an array
  // of enum types. Until bug 872799 is fixed, we'll need to generate arrays the "simple" way for enum types
  // Also note - C# never uses InitializeArray for enum types, so this change puts us on equal footing with them.
  if elems.Length <= 5 || not cenv.opts.emitConstantArraysUsingStaticDataBlobs || (isEnumTy cenv.g elemTy) then 
      GenNewArraySimple cenv cgbuf eenv (elems,elemTy,m) sequel 
  else
      // Try to emit a constant byte-blob array 
      let elems' = Array.ofList elems
      let test,write =  
          match elems'.[0] with 
          | Expr.Const(Const.Bool   _,_,_) -> (function Const.Bool   _ -> true | _ -> false), (fun (buf: ByteBuffer) -> function Const.Bool  b -> buf.EmitBoolAsByte b | _ -> failwith "unreachable")
          | Expr.Const(Const.Char   _,_,_) -> (function Const.Char   _ -> true | _ -> false), (fun buf -> function Const.Char  b -> buf.EmitInt32AsUInt16 (int b) | _ -> failwith "unreachable")
          | Expr.Const(Const.Byte   _,_,_) -> (function Const.Byte   _ -> true | _ -> false), (fun buf -> function Const.Byte b -> buf.EmitByte b | _ -> failwith "unreachable")
          | Expr.Const(Const.UInt16 _,_,_) -> (function Const.UInt16 _ -> true | _ -> false), (fun buf -> function Const.UInt16 b -> buf.EmitUInt16 b | _ -> failwith "unreachable")
          | Expr.Const(Const.UInt32 _,_,_) -> (function Const.UInt32 _ -> true | _ -> false), (fun buf -> function Const.UInt32 b -> buf.EmitInt32 (int32 b) | _ -> failwith "unreachable")
          | Expr.Const(Const.UInt64 _,_,_) -> (function Const.UInt64 _ -> true | _ -> false), (fun buf -> function Const.UInt64 b -> buf.EmitInt64 (int64 b) | _ -> failwith "unreachable")
          | Expr.Const(Const.SByte  _,_,_) -> (function Const.SByte  _ -> true | _ -> false), (fun buf -> function Const.SByte b -> buf.EmitByte (byte b) | _ -> failwith "unreachable")
          | Expr.Const(Const.Int16  _,_,_) -> (function Const.Int16  _ -> true | _ -> false), (fun buf -> function Const.Int16 b -> buf.EmitUInt16 (uint16 b) | _ -> failwith "unreachable")
          | Expr.Const(Const.Int32  _,_,_) -> (function Const.Int32  _ -> true | _ -> false), (fun buf -> function Const.Int32 b -> buf.EmitInt32 b | _ -> failwith "unreachable")
          | Expr.Const(Const.Int64  _,_,_) -> (function Const.Int64  _ -> true | _ -> false), (fun buf -> function Const.Int64 b -> buf.EmitInt64 b | _ -> failwith "unreachable")          
          | _ -> (function _ -> false), (fun _ _ -> failwith "unreachable")

      if elems' |> Array.forall (function Expr.Const(c,_,_) -> test c | _ -> false) then
           let ilElemTy = GenType cenv.amap m eenv.tyenv elemTy
           GenConstArray cenv cgbuf eenv ilElemTy elems' (fun buf -> function Expr.Const(c,_,_) -> write buf c | _ -> failwith "unreachable")
           GenSequel cenv eenv.cloc cgbuf sequel

      else
           GenNewArraySimple cenv cgbuf eenv (elems,elemTy,m) sequel 

and GenCoerce cenv cgbuf eenv (e,tgty,m,srcty) sequel = 
  // Is this an upcast? 
  if TypeRelations.TypeDefinitelySubsumesTypeNoCoercion 0 cenv.g cenv.amap m tgty srcty &&
     // Do an extra check - should not be needed 
     TypeRelations.TypeFeasiblySubsumesType 0 cenv.g cenv.amap m tgty TypeRelations.NoCoerce srcty then
     begin 
       if (isInterfaceTy cenv.g tgty) then (
           GenExpr cenv cgbuf eenv SPSuppress e Continue
           let ilToTy = GenType cenv.amap m eenv.tyenv tgty
           // Section "III.1.8.1.3 Merging stack states" of ECMA-335 implies that no unboxing
           // is required, but we still push the coerce'd type on to the code gen buffer.
           CG.EmitInstrs cgbuf (pop 1) (Push [ilToTy]) []
           GenSequel cenv eenv.cloc cgbuf sequel
       ) else (
           GenExpr cenv cgbuf eenv SPSuppress e sequel
       )
     end       
  else  
    GenExpr cenv cgbuf eenv SPSuppress e Continue          
    if not (isObjTy cenv.g srcty) then 
       let ilFromTy = GenType cenv.amap m eenv.tyenv srcty
       CG.EmitInstrs cgbuf (pop 1) (Push [cenv.g.ilg.typ_Object]) [ I_box ilFromTy  ]
    if not (isObjTy cenv.g tgty) then 
        let ilToTy = GenType cenv.amap m eenv.tyenv tgty
        CG.EmitInstrs cgbuf (pop 1) (Push [ilToTy]) [ I_unbox_any ilToTy  ]
    GenSequel cenv eenv.cloc cgbuf sequel

and GenReraise cenv cgbuf eenv (rtnty,m) sequel =     
    let ilReturnTy = GenType cenv.amap m eenv.tyenv rtnty
    CG.EmitInstrs cgbuf (pop 0) Push0 [I_rethrow]
    // [See comment related to I_throw].
    // Rethrow does not return. Required to push dummy value on the stack.
    // This follows prior behaviour by prim-types reraise<_>.
    CG.EmitInstrs cgbuf (pop 0) (Push [ilReturnTy])  [AI_ldnull;  I_unbox_any ilReturnTy ]
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetExnField cenv cgbuf eenv (e,ecref,fieldNum,m) sequel =
    GenExpr cenv cgbuf eenv SPSuppress e Continue
    let exnc = stripExnEqns ecref
    let typ = GenExnType cenv.amap m eenv.tyenv ecref
    CG.EmitInstrs cgbuf (pop 0) Push0 [ I_castclass typ]

    let fld = List.item fieldNum exnc.TrueInstanceFieldsAsList
    let ftyp = GenType cenv.amap m eenv.tyenv fld.FormalType

    let mspec = mkILNonGenericInstanceMethSpecInTy (typ,"get_" + fld.Name, [], ftyp)
    CG.EmitInstr cgbuf (pop 1) (Push [ftyp]) (mkNormalCall mspec)

    GenSequel cenv eenv.cloc cgbuf sequel

and GenSetExnField cenv cgbuf eenv (e,ecref,fieldNum,e2,m) sequel = 
    GenExpr cenv cgbuf eenv SPSuppress e Continue
    let exnc = stripExnEqns ecref
    let typ = GenExnType cenv.amap m eenv.tyenv ecref
    CG.EmitInstrs cgbuf (pop 0) Push0 [ I_castclass typ ]
    let fld = List.item fieldNum exnc.TrueInstanceFieldsAsList
    let ftyp = GenType cenv.amap m eenv.tyenv fld.FormalType
    let ilFieldName = ComputeFieldName exnc fld
    GenExpr cenv cgbuf eenv SPSuppress e2 Continue
    CG.EmitInstr cgbuf (pop 2) Push0 (mkNormalStfld(mkILFieldSpecInTy (typ,ilFieldName,ftyp)))
    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

and UnionCodeGen (cgbuf: CodeGenBuffer) = 
    { new EraseUnions.ICodeGen<Mark> with 
        member __.CodeLabel(m) = m.CodeLabel
        member __.GenerateDelayMark() = CG.GenerateDelayMark cgbuf "unionCodeGenMark"
        member __.GenLocal(ilty) = cgbuf.AllocLocal([],ilty,false) |> uint16
        member __.SetMarkToHere(m) = CG.SetMarkToHere cgbuf m
        member __.MkInvalidCastExnNewobj () = mkInvalidCastExnNewobj cgbuf.mgbuf.cenv.g
        member __.EmitInstr x = CG.EmitInstr cgbuf (pop 0) (Push []) x
        member __.EmitInstrs xs = CG.EmitInstrs cgbuf (pop 0) (Push []) xs }

and GenUnionCaseProof cenv cgbuf eenv (e,ucref,tyargs,m) sequel =
    GenExpr cenv cgbuf eenv SPSuppress e Continue
    let cuspec,idx = GenUnionCaseSpec cenv.amap m eenv.tyenv ucref tyargs
    let fty = EraseUnions.GetILTypeForAlternative cuspec idx 
    let avoidHelpers = entityRefInThisAssembly cenv.g.compilingFslib ucref.TyconRef
    EraseUnions.emitCastData cenv.g.ilg (UnionCodeGen cgbuf) (false,avoidHelpers,cuspec,idx)
    CG.EmitInstrs cgbuf (pop 1) (Push [fty]) [ ]  // push/pop to match the line above
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetUnionCaseField cenv cgbuf eenv (e,ucref,tyargs,n,m) sequel =
    assert (ucref.Tycon.IsStructOrEnumTycon || isProvenUnionCaseTy (tyOfExpr cenv.g e))
    
    GenExpr cenv cgbuf eenv SPSuppress e Continue
    let cuspec,idx = GenUnionCaseSpec cenv.amap m eenv.tyenv ucref tyargs
    let fty = actualTypOfIlxUnionField cuspec idx n
    let avoidHelpers = entityRefInThisAssembly cenv.g.compilingFslib ucref.TyconRef
    CG.EmitInstrs cgbuf (pop 1) (Push [fty]) (EraseUnions.mkLdData (avoidHelpers, cuspec, idx, n))
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetUnionCaseFieldAddr cenv cgbuf eenv (e,ucref,tyargs,n,m) sequel =
    assert (ucref.Tycon.IsStructOrEnumTycon || isProvenUnionCaseTy (tyOfExpr cenv.g e))
    
    GenExpr cenv cgbuf eenv SPSuppress e Continue
    let cuspec,idx = GenUnionCaseSpec cenv.amap m eenv.tyenv ucref tyargs
    let fty = actualTypOfIlxUnionField cuspec idx n
    let avoidHelpers = entityRefInThisAssembly cenv.g.compilingFslib ucref.TyconRef
    CG.EmitInstrs cgbuf (pop 1) (Push [ILType.Byref fty]) (EraseUnions.mkLdDataAddr (avoidHelpers, cuspec, idx, n))
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetUnionCaseTag cenv cgbuf eenv (e,tcref,tyargs,m) sequel =
    GenExpr cenv cgbuf eenv SPSuppress e Continue
    let cuspec = GenUnionSpec cenv.amap m eenv.tyenv tcref tyargs
    let avoidHelpers = entityRefInThisAssembly cenv.g.compilingFslib tcref
    EraseUnions.emitLdDataTag cenv.g.ilg (UnionCodeGen cgbuf) (avoidHelpers, cuspec)
    CG.EmitInstrs cgbuf (pop 1) (Push [cenv.g.ilg.typ_Int32]) [  ] // push/pop to match the line above
    GenSequel cenv eenv.cloc cgbuf sequel

and GenSetUnionCaseField cenv cgbuf eenv (e,ucref,tyargs,n,e2,m) sequel = 
    GenExpr cenv cgbuf eenv SPSuppress e Continue
    let cuspec,idx = GenUnionCaseSpec cenv.amap m eenv.tyenv ucref tyargs
    let avoidHelpers = entityRefInThisAssembly cenv.g.compilingFslib ucref.TyconRef
    EraseUnions.emitCastData cenv.g.ilg (UnionCodeGen cgbuf) (false,avoidHelpers,cuspec,idx)
    CG.EmitInstrs cgbuf (pop 1) (Push [cuspec.EnclosingType]) [ ] // push/pop to match the line above
    GenExpr cenv cgbuf eenv SPSuppress e2 Continue
    CG.EmitInstrs cgbuf (pop 2) Push0 (EraseUnions.mkStData (cuspec, idx, n))
    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

and GenGetRecdFieldAddr cenv cgbuf eenv (e,f,tyargs,m) sequel = 
    GenExpr cenv cgbuf eenv SPSuppress e Continue
    let fref = GenRecdFieldRef m cenv eenv.tyenv f tyargs
    CG.EmitInstrs cgbuf (pop 1) (Push [ILType.Byref fref.ActualType]) [ I_ldflda fref ] 
    GenSequel cenv eenv.cloc cgbuf sequel
         
and GenGetStaticFieldAddr cenv cgbuf eenv (f,tyargs,m) sequel = 
    let fspec = GenRecdFieldRef m cenv eenv.tyenv f tyargs
    CG.EmitInstrs cgbuf (pop 0) (Push [ILType.Byref fspec.ActualType]) [ I_ldsflda fspec ] 
    GenSequel cenv eenv.cloc cgbuf sequel
         
and GenGetRecdField cenv cgbuf eenv (e,f,tyargs,m) sequel =
    GenExpr cenv cgbuf eenv SPSuppress e Continue
    GenFieldGet false cenv cgbuf eenv (f,tyargs,m)
    GenSequel cenv eenv.cloc cgbuf sequel
  
and GenSetRecdField cenv cgbuf eenv (e1,f,tyargs,e2,m) sequel =
    GenExpr cenv cgbuf eenv SPSuppress e1 Continue
    GenExpr cenv cgbuf eenv SPSuppress e2 Continue
    GenFieldStore false cenv cgbuf eenv (f,tyargs,m) sequel
  
and GenGetStaticField cenv cgbuf eenv (f,tyargs,m) sequel =
    GenFieldGet true cenv cgbuf eenv (f,tyargs,m)
    GenSequel cenv eenv.cloc cgbuf sequel
  
and GenSetStaticField cenv cgbuf eenv (f,tyargs,e2,m) sequel =
    GenExpr cenv cgbuf eenv SPSuppress e2 Continue
    GenFieldStore true cenv cgbuf eenv (f,tyargs,m) sequel

and mk_field_pops isStatic n = if isStatic then pop n else pop (n+1)


and GenFieldGet isStatic cenv cgbuf eenv (rfref:RecdFieldRef,tyargs,m) =
    let fspec = GenRecdFieldRef m cenv eenv.tyenv rfref tyargs
    let vol = if rfref.RecdField.IsVolatile then Volatile else Nonvolatile
    if useGenuineField rfref.Tycon rfref.RecdField || entityRefInThisAssembly cenv.g.compilingFslib rfref.TyconRef then 
        let instr = if isStatic then I_ldsfld(vol, fspec) else I_ldfld (ILAlignment.Aligned, vol, fspec)
        CG.EmitInstrs cgbuf (mk_field_pops isStatic 0) (Push [fspec.ActualType]) [ instr ] 
    else
        let cconv = if isStatic then ILCallingConv.Static else ILCallingConv.Instance
        let mspec = mkILMethSpecInTy (fspec.EnclosingType,cconv, "get_" + rfref.RecdField.rfield_id.idText, [], fspec.FormalType, [])
        CG.EmitInstr cgbuf (mk_field_pops isStatic 0) (Push [fspec.ActualType]) (mkNormalCall mspec)

and GenFieldStore isStatic cenv cgbuf eenv (rfref:RecdFieldRef,tyargs,m) sequel =
    let fspec = GenRecdFieldRef m cenv eenv.tyenv rfref tyargs
    let fld = rfref.RecdField
    if fld.IsMutable && not (useGenuineField rfref.Tycon fld) then
        let cconv = if isStatic then ILCallingConv.Static else ILCallingConv.Instance
        let mspec = mkILMethSpecInTy (fspec.EnclosingType, cconv, "set_" + fld.rfield_id.idText, [fspec.FormalType],ILType.Void,[])
        CG.EmitInstr cgbuf (mk_field_pops isStatic 1) Push0 (mkNormalCall mspec)
    else
        let vol = if rfref.RecdField.IsVolatile then Volatile else Nonvolatile
        let instr = if isStatic then I_stsfld (vol, fspec) else I_stfld (ILAlignment.Aligned, vol, fspec)
        CG.EmitInstr cgbuf (mk_field_pops isStatic 1) Push0 instr 
    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

//--------------------------------------------------------------------------
// Generate arguments to calls
//-------------------------------------------------------------------------- 

/// Generate arguments to a call, unless the argument is the single lone "unit" value
/// to a method or value compiled as a method taking no arguments
and GenUntupledArgsDiscardingLoneUnit cenv cgbuf eenv m numObjArgs curriedArgInfos args  =
    match curriedArgInfos ,args with 
    // Type.M()
    // new C()
    | [[]],[arg] when numObjArgs = 0  -> 
        assert isUnitTy cenv.g (tyOfExpr cenv.g arg)
        GenExpr cenv cgbuf eenv SPSuppress arg discard
    // obj.M()
    | [[_];[]],[arg1;arg2] when numObjArgs = 1 -> 
        assert isUnitTy cenv.g (tyOfExpr cenv.g arg2) 
        GenExpr cenv cgbuf eenv SPSuppress arg1 Continue
        GenExpr cenv cgbuf eenv SPSuppress arg2 discard
    | _ -> 
        (curriedArgInfos,args) ||> List.iter2 (fun argInfos x -> 
            GenUntupledArgExpr cenv cgbuf eenv m argInfos x Continue) 

/// Codegen arguments 
and GenUntupledArgExpr cenv cgbuf eenv m argInfos expr sequel =
    let numRequiredExprs = List.length argInfos
    assert (numRequiredExprs >= 1)
    if numRequiredExprs = 1 then
        GenExpr cenv cgbuf eenv SPSuppress expr sequel
    elif isRefTupleExpr expr then
        let es = tryDestRefTupleExpr expr
        if es.Length <> numRequiredExprs then error(InternalError("GenUntupledArgExpr (2)",m));
        es |> List.iter (fun x -> GenExpr cenv cgbuf eenv SPSuppress x Continue);
        GenSequel cenv eenv.cloc cgbuf sequel
    else
        let ty = tyOfExpr cenv.g expr
        let locv,loce = mkCompGenLocal m "arg" ty
        let bind = mkCompGenBind locv expr
        LocalScope "untuple" cgbuf (fun scopeMarks ->
            let eenvinner = AllocStorageForBind cenv cgbuf scopeMarks eenv bind
            GenBind cenv cgbuf eenvinner bind;
            let tys = destRefTupleTy cenv.g ty
            assert (tys.Length = numRequiredExprs)
            // TODO - tupInfoRef
            argInfos |> List.iteri (fun i _ -> GenGetTupleField cenv cgbuf eenvinner (tupInfoRef (* TODO *),loce,tys,i,m) Continue);
            GenSequel cenv eenv.cloc cgbuf sequel
        )


//--------------------------------------------------------------------------
// Generate calls (try to detect direct calls)
//-------------------------------------------------------------------------- 
 
and GenApp cenv cgbuf eenv (f,fty,tyargs,args,m) sequel =
  match (f,tyargs,args) with 
   (* Look for tailcall to turn into branch *)
  | (Expr.Val(v,_,_),_,_) when  
       ((ListAssoc.containsKey cenv.g.valRefEq v eenv.innerVals) && 
        not v.IsConstructor &&
        let (kind,_) = ListAssoc.find cenv.g.valRefEq v eenv.innerVals
        (* when branch-calling methods we must have the right type parameters *)
        begin match kind with
          | BranchCallClosure _ -> true
          | BranchCallMethod (_,_,tps,_,_)  ->  
              (List.lengthsEqAndForall2 (fun ty tp -> typeEquiv cenv.g ty (mkTyparTy tp)) tyargs tps)
        end &&
        (* must be exact #args, ignoring tupling - we untuple if needed below *)
        (let arityInfo = 
           match kind with
           | BranchCallClosure arityInfo
           | BranchCallMethod (arityInfo,_,_,_,_)  ->  arityInfo
         arityInfo.Length = args.Length
        ) &&
        (* no tailcall out of exception handler, etc. *)
        (match sequelIgnoringEndScopesAndDiscard sequel with Return | ReturnVoid -> true | _ -> false))
    -> 
        let (kind,mark) = ListAssoc.find cenv.g.valRefEq v eenv.innerVals
        let ntmargs = 
          match kind with
          | BranchCallClosure arityInfo ->
              let ntmargs = List.foldBack (+) arityInfo 0
              GenExprs cenv cgbuf eenv args
              ntmargs
          | BranchCallMethod (arityInfo,curriedArgInfos,_,ntmargs,numObjArgs)  ->
              assert (curriedArgInfos.Length = arityInfo.Length )
              assert (curriedArgInfos.Length = args.Length)
              //assert (curriedArgInfos.Length = ntmargs )
              GenUntupledArgsDiscardingLoneUnit cenv cgbuf eenv m numObjArgs curriedArgInfos args
              if v.IsExtensionMember then
                match curriedArgInfos, args with
                | [[]],[_] when numObjArgs = 0 -> (ntmargs-1) 
                | [[_];[]],[_;_] when numObjArgs = 1 -> (ntmargs-1) 
                | _ -> ntmargs    
              else ntmargs

        for i = ntmargs - 1 downto 0 do 
            CG.EmitInstrs cgbuf (pop 1) Push0 [ I_starg (uint16 (i+cgbuf.PreallocatedArgCount)) ]

        CG.EmitInstrs cgbuf (pop 0) Push0 [ I_br mark.CodeLabel ]

        GenSequelEndScopes cgbuf sequel
        
  // PhysicalEquality becomes cheap reference equality once
  // a nominal type is known. We can't replace it for variable types since
  // a "ceq" instruction can't be applied to variable type values.
  | (Expr.Val(v,_,_),[ty],[arg1;arg2]) when
    (valRefEq cenv.g v cenv.g.reference_equality_inner_vref)  
    && isAppTy cenv.g ty ->
        
      GenExpr cenv cgbuf eenv SPSuppress arg1 Continue
      GenExpr cenv cgbuf eenv SPSuppress arg2 Continue
      CG.EmitInstr cgbuf (pop 2) (Push [cenv.g.ilg.typ_Bool]) AI_ceq
      GenSequel cenv eenv.cloc cgbuf sequel

  // Emit "methodhandleof" calls as ldtoken instructions
  //
  // The token for the "GenericMethodDefinition" is loaded
  | Expr.Val(v,_,m),_,[arg] when valRefEq cenv.g v cenv.g.methodhandleof_vref ->
        let (|OptionalCoerce|) = function Expr.Op(TOp.Coerce _,_,[arg],_) -> arg | x -> x
        let (|OptionalTyapp|) = function Expr.App(f,_,[_],[],_) -> f | x -> x
        match arg with 
        // Generate ldtoken instruction for "methodhandleof(fun (a,b,c) -> f(a,b,c))"
        // where f is an F# function value or F# method
        | Expr.Lambda(_,_,_,_,Expr.App(OptionalCoerce(OptionalTyapp(Expr.Val(vref,_,_))),_,_,_,_),_,_) ->
            
            let storage = StorageForValRef m vref eenv
            match storage with   
            | Method (_,_,mspec,_,_,_) ->
                CG.EmitInstr cgbuf (pop 0) (Push [cenv.g.iltyp_RuntimeMethodHandle]) (I_ldtoken (ILToken.ILMethod mspec))
            | _ -> 
                errorR(Error(FSComp.SR.ilxgenUnexpectedArgumentToMethodHandleOfDuringCodegen(), m)) 
            
        // Generate ldtoken instruction for "methodhandleof(fun (a,b,c) -> obj.M(a,b,c))"
        // where M is an IL method.
        | Expr.Lambda(_,_,_,_,Expr.Op(TOp.ILCall(_,_,valu,_,_,_,_,ilMethRef,actualTypeInst,actualMethInst,_),_,_,_),_,_) ->
            
            let boxity = (if valu then AsValue else AsObject)
            let mkFormalParams gparams = gparams |> DropErasedTyargs |> List.mapi (fun n _gf -> mkILTyvarTy (uint16 n)) 
            let ilGenericMethodSpec = IL.mkILMethSpec (ilMethRef, boxity, mkFormalParams actualTypeInst, mkFormalParams actualMethInst)
            let i = I_ldtoken (ILToken.ILMethod ilGenericMethodSpec)
            CG.EmitInstr cgbuf (pop 0) (Push [cenv.g.iltyp_RuntimeMethodHandle]) i 

        | _ -> 
            System.Diagnostics.Debug.Assert(false,sprintf "Break for invalid methodhandleof argument expression")
            //System.Diagnostics.Debugger.Break()
            errorR(Error(FSComp.SR.ilxgenUnexpectedArgumentToMethodHandleOfDuringCodegen(), m)) 

        GenSequel cenv eenv.cloc cgbuf sequel

  // Optimize calls to top methods when given "enough" arguments. 
  | Expr.Val(vref,valUseFlags,_),_,_ 
                when
                     (let storage = StorageForValRef m vref eenv
                      match storage with   
                      | Method (topValInfo,vref,_,_,_,_) ->
                          (let tps,argtys,_,_ = GetTopValTypeInFSharpForm cenv.g topValInfo vref.Type m
                           tps.Length = tyargs.Length && 
                           argtys.Length <= args.Length)
                      | _ -> false) ->

      let storage = StorageForValRef m vref eenv
      match storage with   
      | Method (topValInfo,vref,mspec,_,_,_) ->
          let nowArgs,laterArgs = 
              let _,curriedArgInfos,_,_ = GetTopValTypeInFSharpForm cenv.g topValInfo vref.Type m
              List.chop curriedArgInfos.Length args

          let actualRetTy = applyTys cenv.g vref.Type (tyargs,nowArgs)
          let _,curriedArgInfos,returnTy,_ = GetTopValTypeInCompiledForm cenv.g topValInfo vref.Type m

          let ilTyArgs = GenTypeArgs cenv.amap m eenv.tyenv tyargs
          

          // For instance method calls chop off some type arguments, which are already 
          // carried by the class.  Also work out if it's a virtual call. 
          let _,virtualCall,newobj,isSuperInit,isSelfInit,_,_,_ = GetMemberCallInfo cenv.g (vref,valUseFlags) in

          // numEnclILTypeArgs will include unit-of-measure args, unfortunately. For now, just cut-and-paste code from GetMemberCallInfo
          // @REVIEW: refactor this 
          let numEnclILTypeArgs = 
              match vref.MemberInfo with 
              | Some _ when not (vref.IsExtensionMember) -> 
                  List.length(vref.MemberApparentParent.TyparsNoRange |> DropErasedTypars) 
              | _ -> 0

          let (ilEnclArgTys,ilMethArgTys) = 
              if ilTyArgs.Length  < numEnclILTypeArgs then error(InternalError("length mismatch",m))
              List.chop numEnclILTypeArgs ilTyArgs

          let boxity = mspec.EnclosingType.Boxity
          let mspec = mkILMethSpec (mspec.MethodRef, boxity,ilEnclArgTys,ilMethArgTys)
          
          // "Unit" return types on static methods become "void" 
          let mustGenerateUnitAfterCall = Option.isNone returnTy
          
          let ccallInfo = 
              match valUseFlags with
              | PossibleConstrainedCall ty ->  Some ty
              | _ -> None
              
          let isBaseCall = match valUseFlags with VSlotDirectCall -> true | _ -> false

          let isTailCall = 
              if isNil laterArgs && not isSelfInit then 
                  let isDllImport = IsValRefIsDllImport cenv.g vref
                  let hasByrefArg = mspec.FormalArgTypes |> List.exists (function ILType.Byref _ -> true | _ -> false)
                  let makesNoCriticalTailcalls = vref.MakesNoCriticalTailcalls 
                  CanTailcall((boxity=AsValue),ccallInfo,eenv.withinSEH,hasByrefArg,mustGenerateUnitAfterCall,isDllImport,isSelfInit,makesNoCriticalTailcalls,sequel)
              else Normalcall
              
          let useICallVirt = virtualCall || useCallVirt cenv boxity mspec isBaseCall
          
          let callInstr = 
              match valUseFlags with
              | PossibleConstrainedCall ty -> 
                  let ilThisTy = GenType cenv.amap m eenv.tyenv ty
                  I_callconstraint ( isTailCall, ilThisTy,mspec,None)
              | _ -> 
                  if newobj then I_newobj (mspec, None) 
                  elif useICallVirt then I_callvirt (isTailCall, mspec, None) 
                  else I_call (isTailCall, mspec, None)

          // ok, now we're ready to generate 
          if isSuperInit || isSelfInit then 
              CG.EmitInstrs cgbuf (pop 0) (Push [mspec.EnclosingType ]) [ mkLdarg0 ] 

          GenUntupledArgsDiscardingLoneUnit cenv cgbuf eenv m vref.NumObjArgs curriedArgInfos nowArgs

          // Generate laterArgs (for effects) and save
          LocalScope "callstack" cgbuf (fun scopeMarks ->
                let whereSaved,eenv = 
                    (eenv,laterArgs) ||> List.mapFold (fun eenv laterArg -> 
                        // Only save arguments that have effects
                        if Optimizer.ExprHasEffect cenv.g laterArg then 
                            let ilTy = laterArg |> tyOfExpr cenv.g |> GenType cenv.amap m eenv.tyenv
                            let loc,eenv = AllocLocal cenv cgbuf eenv true (ilxgenGlobalNng.FreshCompilerGeneratedName ("arg",m), ilTy, false) scopeMarks
                            GenExpr cenv cgbuf eenv SPSuppress laterArg Continue
                            EmitSetLocal cgbuf loc
                            Choice1Of2 (ilTy,loc),eenv
                        else
                            Choice2Of2 laterArg, eenv) 

                let nargs = mspec.FormalArgTypes.Length
                CG.EmitInstr cgbuf (pop (nargs + (if mspec.CallingConv.IsStatic || newobj then 0 else 1)))
                                     (if mustGenerateUnitAfterCall || isSuperInit || isSelfInit then Push0 else (Push [(GenType cenv.amap m eenv.tyenv actualRetTy)])) callInstr

                // For isSuperInit, load the 'this' pointer as the pretend 'result' of the operation.  It will be popped again in most cases 
                if isSuperInit then CG.EmitInstrs cgbuf (pop 0) (Push [mspec.EnclosingType]) [ mkLdarg0 ] 

                // When generating debug code, generate a 'nop' after a 'call' that returns 'void'
                // This is what C# does, as it allows the call location to be maintained correctly in the stack frame
                if cenv.opts.generateDebugSymbols && mustGenerateUnitAfterCall && (isTailCall = Normalcall) then 
                    CG.EmitInstrs cgbuf (pop 0) Push0  [ AI_nop ]

                if isNil laterArgs then 
                    assert isNil whereSaved 
                    // Generate the "unit" value if necessary 
                    CommitCallSequel cenv eenv m eenv.cloc cgbuf mustGenerateUnitAfterCall sequel 
                else 
                    //printfn "%d EXTRA ARGS IN TOP APP at %s" laterArgs.Length (stringOfRange m)
                    whereSaved |>  List.iter (function 
                        | Choice1Of2 (ilTy,loc) -> EmitGetLocal cgbuf ilTy loc 
                        | Choice2Of2 expr -> GenExpr cenv cgbuf eenv SPSuppress expr Continue)
                    GenIndirectCall cenv cgbuf eenv (actualRetTy,[],laterArgs,m) sequel)
                  
      | _ -> failwith "??"
        
    // This case is for getting/calling a value, when we can't call it directly. 
    // However, we know the type instantiation for the value.  
    // In this case we can often generate a type-specific local expression for the value. 
    // This reduces the number of dynamic type applications. 
  | (Expr.Val(vref,_,_),_,_)  -> 
     GenGetValRefAndSequel cenv cgbuf eenv m vref (Some (tyargs,args,m,sequel))
        
  | _ ->
    (* worst case: generate a first-class function value and call *)
    GenExpr cenv cgbuf eenv SPSuppress f Continue
    GenArgsAndIndirectCall cenv cgbuf eenv (fty,tyargs,args,m) sequel
        
and CanTailcall (hasStructObjArg, ccallInfo, withinSEH, hasByrefArg, mustGenerateUnitAfterCall, isDllImport, isSelfInit, makesNoCriticalTailcalls, sequel) = 
    // Can't tailcall with a struct object arg since it involves a byref
    // Can't tailcall with a .NET 2.0 generic constrained call since it involves a byref
    if not hasStructObjArg && Option.isNone ccallInfo && not withinSEH && not hasByrefArg && not isDllImport && not isSelfInit && not makesNoCriticalTailcalls &&
        // We can tailcall even if we need to generate "unit", as long as we're about to throw the value away anyway as par of the return. 
        // We can tailcall if we don't need to generate "unit", as long as we're about to return. 
        (match sequelIgnoreEndScopes sequel with 
         | ReturnVoid | Return           -> not mustGenerateUnitAfterCall
         | DiscardThen ReturnVoid ->     mustGenerateUnitAfterCall
         | _                -> false) 
    then Tailcall 
    else Normalcall
        
and GenNamedLocalTyFuncCall cenv (cgbuf: CodeGenBuffer) eenv typ cloinfo tyargs m = 
    
    let ilContractClassTyargs = 
        cloinfo.localTypeFuncContractFreeTypars 
            |> List.map mkTyparTy 
            |> GenTypeArgs cenv.amap m eenv.tyenv

    let ilTyArgs = tyargs |> GenTypeArgs cenv.amap m eenv.tyenv

    let _,(ilContractMethTyargs: ILGenericParameterDefs),(ilContractCloTySpec:ILTypeSpec),ilContractFormalRetTy = 
        GenNamedLocalTypeFuncContractInfo cenv eenv m cloinfo

    let ilContractTy = mkILBoxedTy ilContractCloTySpec.TypeRef ilContractClassTyargs
    
    if not (ilContractMethTyargs.Length = ilTyArgs.Length) then errorR(Error(FSComp.SR.ilIncorrectNumberOfTypeArguments(),m))

    // Local TyFunc are represented as a $contract type. they currently get stored in a value of type object
    // Recover result (value or reference types) via unbox_any.
    CG.EmitInstrs cgbuf (pop 1) (Push [ilContractTy])  [I_unbox_any ilContractTy]
    let actualRetTy = applyTys cenv.g typ (tyargs,[])

    let ilDirectInvokeMethSpec = mkILInstanceMethSpecInTy(ilContractTy, "DirectInvoke", [], ilContractFormalRetTy, ilTyArgs)
    let ilActualRetTy = GenType cenv.amap m eenv.tyenv actualRetTy
    CountCallFuncInstructions()
    CG.EmitInstr cgbuf (pop 1) (Push [ilActualRetTy]) (mkNormalCallvirt ilDirectInvokeMethSpec)
    actualRetTy

        
/// Generate an indirect call, converting to an ILX callfunc instruction
and GenArgsAndIndirectCall cenv cgbuf eenv (functy,tyargs,args,m) sequel =

    // Generate the arguments to the indirect call
    GenExprs cenv cgbuf eenv args
    GenIndirectCall cenv cgbuf eenv (functy,tyargs,args,m) sequel 

/// Generate an indirect call, converting to an ILX callfunc instruction
and GenIndirectCall cenv cgbuf eenv (functy,tyargs,args,m) sequel =
    
    // Fold in the new types into the environment as we generate the formal types. 
    let ilxClosureApps = 
        // keep only non-erased type arguments when computing indirect call
        let tyargs = DropErasedTyargs tyargs 

        let typars,formalFuncTyp = tryDestForallTy cenv.g functy

        let feenv = eenv.tyenv.Add typars

        // This does two phases: REVIEW: the code is too complex for what it's achieving and should be rewritten
        let formalRetTy,appBuilder = 
            List.fold 
              (fun (formalFuncTyp,sofar) _ -> 
                let dty,rty = destFunTy cenv.g formalFuncTyp
                (rty,(fun acc -> sofar (Apps_app(GenType cenv.amap m feenv dty,acc)))))
              (formalFuncTyp,id)
              args

        let ilxRetApps = Apps_done (GenType cenv.amap m feenv formalRetTy)

        List.foldBack (fun tyarg acc -> Apps_tyapp(GenType cenv.amap m eenv.tyenv tyarg,acc)) tyargs (appBuilder ilxRetApps)

    let actualRetTy = applyTys cenv.g functy (tyargs, args)
    let ilActualRetTy = GenType cenv.amap m eenv.tyenv actualRetTy

    // Check if any byrefs are involved to make sure we don't tailcall
    let hasByrefArg = 
        let rec check x = 
          match x with 
          | Apps_tyapp(_,apps) -> check apps
          | Apps_app(arg,apps) -> IsILTypeByref arg || check apps
          | _ -> false
        check ilxClosureApps
        
    let isTailCall = CanTailcall(false,None,eenv.withinSEH,hasByrefArg,false,false,false,false,sequel)
    CountCallFuncInstructions()

    // Generate the code code an ILX callfunc operation
    let instrs = EraseClosures.mkCallFunc cenv.g.ilxPubCloEnv (fun ty -> cgbuf.AllocLocal([], ty,false) |> uint16) eenv.tyenv.Count isTailCall ilxClosureApps
    CG.EmitInstrs cgbuf (pop (1+args.Length)) (Push [ilActualRetTy]) instrs

    // Done compiling indirect call...
    GenSequel cenv eenv.cloc cgbuf sequel

//--------------------------------------------------------------------------
// Generate try expressions
//-------------------------------------------------------------------------- 

and GenTry cenv cgbuf eenv scopeMarks (e1,m,resty,spTry) =
    let sp = 
        match spTry with 
        | SequencePointAtTry m -> CG.EmitSeqPoint cgbuf m; SPAlways
        | SequencePointInBodyOfTry -> SPAlways
        | NoSequencePointAtTry -> SPSuppress
    
    let stack,eenvinner = EmitSaveStack cenv cgbuf eenv m scopeMarks
    let startTryMark = CG.GenerateMark cgbuf "startTryMark"
    let endTryMark = CG.GenerateDelayMark cgbuf "endTryMark"
    let afterHandler = CG.GenerateDelayMark cgbuf "afterHandler"
    let eenvinner = {eenvinner with withinSEH = true}
    let ilResultTy = GenType cenv.amap m eenvinner.tyenv resty
    let whereToSave,eenvinner = AllocLocal cenv cgbuf eenvinner true (ilxgenGlobalNng.FreshCompilerGeneratedName ("tryres",m),ilResultTy, false) (startTryMark,endTryMark)

    // Generate the body of the try. In the normal case (SequencePointAtTry) we generate a sequence point
    // both on the 'try' keyword and on the start of the expression in the 'try'. For inlined code and
    // compiler generated 'try' blocks (i.e. NoSequencePointAtTry, used for the try/finally implicit 
    // in a 'use' or 'foreach'), we suppress the sequence point
    GenExpr cenv cgbuf eenvinner sp e1 (LeaveHandler (false, whereToSave,afterHandler))
    CG.SetMarkToHere cgbuf endTryMark
    let tryMarks = (startTryMark.CodeLabel, endTryMark.CodeLabel)
    whereToSave,eenvinner,stack,tryMarks,afterHandler,ilResultTy

and GenTryCatch cenv cgbuf eenv (e1,vf:Val,ef,vh:Val,eh,m,resty,spTry,spWith) sequel =
    // Save the stack - gross because IL flushes the stack at the exn. handler 
    // note: eenvinner notes spill vars are live 
    LocalScope "trystack" cgbuf (fun scopeMarks -> 
       let whereToSave,eenvinner,stack,tryMarks,afterHandler,ilResultTy = GenTry cenv cgbuf eenv scopeMarks (e1,m,resty,spTry) 

       // Now the filter and catch blocks 

       let seh = 
           if cenv.opts.generateFilterBlocks then 
               let startOfFilter = CG.GenerateMark cgbuf "startOfFilter" 
               let afterFilter = CG.GenerateDelayMark cgbuf "afterFilter"
               let (sequelOnBranches,afterJoin,stackAfterJoin,sequelAfterJoin) = GenJoinPoint cenv cgbuf "filter" eenv cenv.g.int_ty m EndFilter
               begin
                   // We emit the sequence point for the 'with' keyword span on the start of the filter
                   // block. However the targets of the filter block pattern matching should not get any
                   // sequence points (they will be 'true'/'false' values indicating if the exception has been
                   // caught or not).
                   //
                   // The targets of the handler block DO get sequence points. Thus the expected behaviour 
                   // for a try/with with a complex pattern is that we hit the "with" before the filter is run
                   // and then jump to the handler for the successful catch (or continue with exception handling
                   // if the filter fails)
                   match spWith with 
                   | SequencePointAtWith m -> CG.EmitSeqPoint cgbuf m
                   | NoSequencePointAtWith -> () 


                   CG.SetStack cgbuf [cenv.g.ilg.typ_Object]
                   let _,eenvinner = AllocLocalVal cenv cgbuf vf eenvinner None (startOfFilter,afterFilter)
                   CG.EmitInstr cgbuf (pop 1) (Push [cenv.g.iltyp_Exception]) (I_castclass cenv.g.iltyp_Exception)

                   GenStoreVal cgbuf eenvinner vf.Range vf

                   // Why SPSuppress? Because we do not emit a sequence point at the start of the List.filter - we've already put one on
                   // the 'with' keyword above
                   GenExpr cenv cgbuf eenvinner  SPSuppress ef sequelOnBranches
                   CG.SetMarkToHere cgbuf afterJoin
                   CG.SetStack cgbuf stackAfterJoin
                   GenSequel cenv eenv.cloc cgbuf sequelAfterJoin
               end
               let endOfFilter = CG.GenerateMark cgbuf "endOfFilter"
               let filterMarks = (startOfFilter.CodeLabel, endOfFilter.CodeLabel)
               CG.SetMarkToHere cgbuf afterFilter

               let startOfHandler = CG.GenerateMark cgbuf "startOfHandler" 
               begin
                   CG.SetStack cgbuf [cenv.g.ilg.typ_Object]
                   let _,eenvinner = AllocLocalVal cenv cgbuf vh eenvinner None (startOfHandler,afterHandler)
                   CG.EmitInstr cgbuf (pop 1) (Push [cenv.g.iltyp_Exception]) (I_castclass cenv.g.iltyp_Exception)
                   GenStoreVal cgbuf eenvinner vh.Range vh

                   GenExpr cenv cgbuf eenvinner SPAlways eh (LeaveHandler (false, whereToSave,afterHandler))
               end
               let endOfHandler = CG.GenerateMark cgbuf "endOfHandler"
               let handlerMarks = (startOfHandler.CodeLabel, endOfHandler.CodeLabel)
               ILExceptionClause.FilterCatch(filterMarks, handlerMarks)
           else 
               let startOfHandler = CG.GenerateMark cgbuf "startOfHandler" 
               begin
                   match spWith with 
                   | SequencePointAtWith m -> CG.EmitSeqPoint cgbuf m
                   | NoSequencePointAtWith -> () 

                   CG.SetStack cgbuf [cenv.g.ilg.typ_Object]
                   let _,eenvinner = AllocLocalVal cenv cgbuf vh eenvinner None (startOfHandler,afterHandler)
                   CG.EmitInstr cgbuf (pop 1) (Push [cenv.g.iltyp_Exception]) (I_castclass cenv.g.iltyp_Exception)

                   GenStoreVal cgbuf eenvinner m vh

                   GenExpr cenv cgbuf eenvinner SPAlways eh (LeaveHandler (false, whereToSave,afterHandler))
               end
               let endOfHandler = CG.GenerateMark cgbuf "endOfHandler"
               let handlerMarks = (startOfHandler.CodeLabel, endOfHandler.CodeLabel)
               ILExceptionClause.TypeCatch(cenv.g.ilg.typ_Object, handlerMarks)

       cgbuf.EmitExceptionClause
         { Clause = seh
           Range= tryMarks } 

       CG.SetMarkToHere cgbuf afterHandler
       CG.SetStack cgbuf []

       cgbuf.EmitStartOfHiddenCode()

       (* Restore the stack and load the result *)
       EmitRestoreStack cgbuf stack (* RESTORE *)

       EmitGetLocal cgbuf ilResultTy whereToSave
       GenSequel cenv eenv.cloc cgbuf sequel
   ) 


and GenTryFinally cenv cgbuf eenv (bodyExpr,handlerExpr,m,resty,spTry,spFinally) sequel =
    // Save the stack - needed because IL flushes the stack at the exn. handler 
    // note: eenvinner notes spill vars are live 
    LocalScope "trystack" cgbuf (fun scopeMarks -> 

       let whereToSave,eenvinner,stack,tryMarks,afterHandler,ilResultTy = GenTry cenv cgbuf eenv scopeMarks (bodyExpr,m,resty,spTry) 

       // Now the catch/finally block 
       let startOfHandler = CG.GenerateMark cgbuf "startOfHandler" 
       CG.SetStack cgbuf []
       
       let sp = 
           match spFinally with 
           | SequencePointAtFinally m -> CG.EmitSeqPoint cgbuf m; SPAlways
           | NoSequencePointAtFinally -> SPSuppress

       GenExpr cenv cgbuf eenvinner sp handlerExpr (LeaveHandler (true, whereToSave,afterHandler))
       let endOfHandler = CG.GenerateMark cgbuf "endOfHandler"
       let handlerMarks = (startOfHandler.CodeLabel, endOfHandler.CodeLabel)
       cgbuf.EmitExceptionClause
         { Clause = ILExceptionClause.Finally(handlerMarks)
           Range   = tryMarks } 

       CG.SetMarkToHere cgbuf afterHandler
       CG.SetStack cgbuf []

       // Restore the stack and load the result 
       cgbuf.EmitStartOfHiddenCode()
       EmitRestoreStack cgbuf stack 
       EmitGetLocal cgbuf ilResultTy whereToSave
       GenSequel cenv eenv.cloc cgbuf sequel
   ) 

//--------------------------------------------------------------------------
// Generate for-loop
//-------------------------------------------------------------------------- 
    
and GenForLoop cenv cgbuf eenv (spFor,v,e1,dir,e2,loopBody,m) sequel =
    // The JIT/NGen eliminate array-bounds checks for C# loops of form:
    //   for(int i=0; i < (#ldlen arr#); i++) { ... arr[i] ... }
    // Here
    //     dir = BI_blt indicates an optimized for loop that fits C# form that evaluates its 'end' argument each time around
    //     dir = BI_ble indicates a normal F# for loop that evaluates its argument only once
    //
    // It is also important that we follow C# IL-layout exactly "prefix, jmp test, body, test, finish" for JIT/NGEN.
    let start = CG.GenerateMark cgbuf "for_start" 
    let finish = CG.GenerateDelayMark cgbuf "for_finish"
    let inner = CG.GenerateDelayMark cgbuf "for_inner"
    let test = CG.GenerateDelayMark cgbuf "for_test"
    let stack,eenvinner = EmitSaveStack cenv cgbuf eenv m (start,finish)

    let isUp = (match dir with | FSharpForLoopUp | CSharpForLoopUp -> true | FSharpForLoopDown -> false)
    let isFSharpStyle = (match dir with FSharpForLoopUp | FSharpForLoopDown -> true | CSharpForLoopUp  -> false)
    
    let finishIdx,eenvinner = 
        if isFSharpStyle then 
            let v,eenvinner = AllocLocal cenv cgbuf eenvinner true (ilxgenGlobalNng.FreshCompilerGeneratedName ("endLoop",m), cenv.g.ilg.typ_Int32, false) (start,finish)
            v, eenvinner
        else
            -1,eenvinner

    let _, eenvinner = AllocLocalVal cenv cgbuf v eenvinner None (start,finish) (* note: eenvStack noted stack spill vars are live *)
    match spFor with 
    | SequencePointAtForLoop(spStart) -> CG.EmitSeqPoint cgbuf  spStart
    | NoSequencePointAtForLoop -> ()

    GenExpr cenv cgbuf eenv SPSuppress e1 Continue
    GenStoreVal cgbuf eenvinner m v
    if isFSharpStyle then 
        GenExpr cenv cgbuf eenvinner SPSuppress e2 Continue
        EmitSetLocal cgbuf finishIdx
        EmitGetLocal cgbuf cenv.g.ilg.typ_Int32 finishIdx
        GenGetLocalVal cenv cgbuf eenvinner e2.Range v None        
        CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp ((if isUp then BI_blt else BI_bgt),finish.CodeLabel))
    
    else
        CG.EmitInstr cgbuf (pop 0) Push0 (I_br test.CodeLabel)

    // .inner 
    CG.SetMarkToHere cgbuf inner
    //    <loop body>
    GenExpr cenv cgbuf eenvinner SPAlways loopBody discard
    //    v++ or v--
    GenGetLocalVal cenv cgbuf eenvinner e2.Range v None

    CG.EmitInstr cgbuf (pop 0) (Push [cenv.g.ilg.typ_Int32]) (mkLdcInt32 1)
    CG.EmitInstr cgbuf (pop 1) Push0 (if isUp then AI_add else AI_sub)
    GenStoreVal cgbuf eenvinner m v

    // .text 
    CG.SetMarkToHere cgbuf test

    // FSharpForLoopUp: if v <> e2 + 1 then goto .inner
    // FSharpForLoopDown: if v <> e2 - 1 then goto .inner
    // CSharpStyle: if v < e2 then goto .inner
    match spFor with 
    | SequencePointAtForLoop(spStart) -> CG.EmitSeqPoint cgbuf  spStart
    | NoSequencePointAtForLoop -> () //CG.EmitSeqPoint cgbuf  e2.Range
    
    GenGetLocalVal cenv cgbuf eenvinner e2.Range v None

    let cmp = match dir with FSharpForLoopUp | FSharpForLoopDown -> BI_bne_un | CSharpForLoopUp -> BI_blt
    let e2Sequel =  (CmpThenBrOrContinue (pop 2, [ I_brcmp(cmp,inner.CodeLabel) ]))

    if isFSharpStyle then 
        EmitGetLocal cgbuf cenv.g.ilg.typ_Int32  finishIdx
        CG.EmitInstr cgbuf (pop 0) (Push [cenv.g.ilg.typ_Int32]) (mkLdcInt32 1)
        CG.EmitInstr cgbuf (pop 1) Push0 (if isUp then AI_add else AI_sub)
        GenSequel cenv eenv.cloc cgbuf e2Sequel
    else
        GenExpr cenv cgbuf eenv SPSuppress e2 e2Sequel

    // .finish - loop-exit here 
    CG.SetMarkToHere cgbuf finish

    // Restore the stack and load the result 
    EmitRestoreStack cgbuf stack
    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

//--------------------------------------------------------------------------
// Generate while-loop 
//-------------------------------------------------------------------------- 
    
and GenWhileLoop cenv cgbuf eenv (spWhile,e1,e2,m) sequel =
    let finish = CG.GenerateDelayMark cgbuf "while_finish" 
    let startTest = CG.GenerateMark cgbuf "startTest"
    
    match spWhile with 
    | SequencePointAtWhileLoop(spStart) -> CG.EmitSeqPoint cgbuf  spStart
    | NoSequencePointAtWhileLoop -> ()

    // SEQUENCE POINTS: Emit a sequence point to cover all of 'while e do' 
    GenExpr cenv cgbuf eenv SPSuppress e1 (CmpThenBrOrContinue (pop 1, [ I_brcmp(BI_brfalse,finish.CodeLabel) ]))
    
    GenExpr cenv cgbuf eenv SPAlways e2 (DiscardThen (Br startTest))
    CG.SetMarkToHere cgbuf finish 

    // SEQUENCE POINTS: Emit a sequence point to cover 'done' if present 
    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

//--------------------------------------------------------------------------
// Generate seq
//-------------------------------------------------------------------------- 

and GenSequential cenv cgbuf eenv spIn (e1,e2,specialSeqFlag,spSeq,_m) sequel =
    
    // Compiler generated sequential executions result in suppressions of sequence points on both 
    // left and right of the sequence
    let spAction,spExpr = 
        (match spSeq with 
         | SequencePointsAtSeq -> SPAlways,SPAlways
         | SuppressSequencePointOnExprOfSequential -> SPSuppress,spIn
         | SuppressSequencePointOnStmtOfSequential -> spIn,SPSuppress)
    match specialSeqFlag with 
    | NormalSeq -> 
        GenExpr cenv cgbuf eenv spAction e1 discard 
        GenExpr cenv cgbuf eenv spExpr e2 sequel
    | ThenDoSeq ->
        GenExpr cenv cgbuf eenv spExpr e1 Continue
        GenExpr cenv cgbuf eenv spAction e2 discard
        GenSequel cenv eenv.cloc cgbuf sequel

//--------------------------------------------------------------------------
// Generate IL assembly code.
// Polymorphic IL/ILX instructions may be instantiated when polymorphic code is inlined.
// We must implement this for the few uses of polymorphic instructions 
// in the standard libarary. 
//-------------------------------------------------------------------------- 

and GenAsmCode cenv cgbuf eenv (il,tyargs,args,returnTys,m) sequel =
    let ilTyArgs = GenTypesPermitVoid cenv.amap m eenv.tyenv tyargs
    let ilReturnTys   = GenTypesPermitVoid cenv.amap m eenv.tyenv returnTys
    let ilAfterInst = 
      il |> List.filter (function AI_nop -> false | _ -> true)
         |> List.map (fun i -> 
          let err s  = 
              errorR(InternalError(sprintf "%s: bad instruction: %A" s i,m))

          let modFieldSpec fspec = 
              if isNil ilTyArgs then 
                fspec 
              else
                {fspec with EnclosingType= 
                                   let ty = fspec.EnclosingType
                                   let tspec = ty.TypeSpec
                                   mkILTy ty.Boxity (mkILTySpec(tspec.TypeRef, ilTyArgs)) }          
          match i,ilTyArgs with   
            | I_unbox_any (ILType.TypeVar _)           ,[tyarg] -> I_unbox_any (tyarg)
            | I_box (ILType.TypeVar _)                 ,[tyarg] -> I_box (tyarg)
            | I_isinst (ILType.TypeVar _)              ,[tyarg] -> I_isinst (tyarg)
            | I_castclass (ILType.TypeVar _)           ,[tyarg] -> I_castclass (tyarg)
            | I_newarr (shape,ILType.TypeVar _)        ,[tyarg] -> I_newarr (shape,tyarg)
            | I_ldelem_any (shape,ILType.TypeVar _)    ,[tyarg] -> I_ldelem_any (shape,tyarg)
            | I_ldelema (ro,_,shape,ILType.TypeVar _)    ,[tyarg] -> I_ldelema (ro,false,shape,tyarg)
            | I_stelem_any (shape,ILType.TypeVar _)    ,[tyarg] -> I_stelem_any (shape,tyarg)
            | I_ldobj (a,b,ILType.TypeVar _)           ,[tyarg] -> I_ldobj (a,b,tyarg)
            | I_stobj (a,b,ILType.TypeVar _)           ,[tyarg] -> I_stobj (a,b,tyarg)
            | I_ldtoken (ILToken.ILType (ILType.TypeVar _)),[tyarg] -> I_ldtoken (ILToken.ILType (tyarg))
            | I_sizeof (ILType.TypeVar _)              ,[tyarg] -> I_sizeof (tyarg)
            | I_cpobj (ILType.TypeVar _)               ,[tyarg] -> I_cpobj (tyarg)      // currently unused, added for forward compat, see https://visualfsharp.codeplex.com/SourceControl/network/forks/jackpappas/fsharpcontrib/contribution/7134
            | I_initobj (ILType.TypeVar _)             ,[tyarg] -> I_initobj (tyarg)       // currently unused, added for forward compat, see https://visualfsharp.codeplex.com/SourceControl/network/forks/jackpappas/fsharpcontrib/contribution/7134
            | I_ldfld (al,vol,fspec)                 ,_       -> I_ldfld (al,vol,modFieldSpec fspec)
            | I_ldflda (fspec)                       ,_       -> I_ldflda (modFieldSpec fspec)
            | I_stfld (al,vol,fspec)                 ,_       -> I_stfld (al,vol,modFieldSpec fspec)
            | I_stsfld (vol,fspec)                   ,_       -> I_stsfld (vol,modFieldSpec fspec)
            | I_ldsfld (vol,fspec)                   ,_       -> I_ldsfld (vol,modFieldSpec fspec)
            | I_ldsflda (fspec)                      ,_       -> I_ldsflda (modFieldSpec fspec)
            | EI_ilzero(ILType.TypeVar _)              ,[tyarg] -> EI_ilzero(tyarg)
            | AI_nop,_ -> i  
                // These are embedded in the IL for a an initonly ldfld, i.e. 
                // here's the relevant comment from tc.fs 
                //     "Add an I_nop if this is an initonly field to make sure we never recognize it as an lvalue. See mkExprAddrOfExpr." 

            | _ -> 
                if not (isNil tyargs) then err "Bad polymorphic IL instruction" 
                i)
    match ilAfterInst,args,sequel,ilReturnTys with 

    | [ EI_ilzero _ ], _, _, _ -> 
          match tyargs with 
          | [typ] -> 
              GenDefaultValue cenv cgbuf eenv (typ,m)
              GenSequel cenv eenv.cloc cgbuf sequel
          | _ -> failwith "Bad polymorphic IL instruction" 

    // Strip off any ("ceq" x false) when the sequel is a comparison branch and change the BI_brfalse to a BI_brtrue
    // This is the instruction sequence for "not" 
    // For these we can just generate the argument and change the test (from a brfalse to a brtrue and vice versa) 
    | ([ AI_ceq ],
       [arg1; Expr.Const((Const.Bool false | Const.SByte 0y| Const.Int16 0s | Const.Int32 0 | Const.Int64 0L | Const.Byte 0uy| Const.UInt16 0us | Const.UInt32 0u | Const.UInt64 0UL),_,_) ], 
       CmpThenBrOrContinue(1, [I_brcmp (((BI_brfalse | BI_brtrue) as bi),label1) ]),
       _) ->

            let bi = match bi with BI_brtrue -> BI_brfalse | _ -> BI_brtrue
            GenExpr cenv cgbuf eenv SPSuppress arg1 (CmpThenBrOrContinue(pop 1, [ I_brcmp (bi,label1) ]))

    // Query; when do we get a 'ret' in IL assembly code?
    | [ I_ret ], [arg1],sequel,[_ilRetTy] -> 
          GenExpr cenv cgbuf eenv SPSuppress arg1 Continue
          CG.EmitInstr cgbuf (pop 1) Push0 I_ret
          GenSequelEndScopes cgbuf sequel

    // Query; when do we get a 'ret' in IL assembly code?
    | [ I_ret ], [],sequel,[_ilRetTy] -> 
          CG.EmitInstr cgbuf (pop 1) Push0 I_ret
          GenSequelEndScopes cgbuf sequel

    // 'throw' instructions are a bit of a problem - e.g. let x = (throw ...) in ... expects a value *)
    // to be left on the stack.  But dead-code checking by some versions of the .NET verifier *)
    // mean that we can't just have fake code after the throw to generate the fake value *)
    // (nb. a fake value can always be generated by a "ldnull unbox.any ty" sequence *)
    // So in the worst case we generate a fake (never-taken) branch to a piece of code to generate *)
    // the fake value *)
    | [ I_throw ], [arg1],sequel,[ilRetTy] -> 
        match sequelIgnoreEndScopes sequel with 
        | s when IsSequelImmediate  s -> 
            (* In most cases we can avoid doing this... *)
            GenExpr cenv cgbuf eenv SPSuppress arg1 Continue
            CG.EmitInstr cgbuf (pop 1) Push0 I_throw
            GenSequelEndScopes cgbuf sequel
        | _ ->  
            let after1 = CG.GenerateDelayMark cgbuf ("fake_join")
            let after2 = CG.GenerateDelayMark cgbuf ("fake_join")
            let after3 = CG.GenerateDelayMark cgbuf ("fake_join")
            CG.EmitInstrs cgbuf (pop 0) Push0 [mkLdcInt32 0; I_brcmp (BI_brfalse,after2.CodeLabel) ]

            CG.SetMarkToHere cgbuf after1
            CG.EmitInstrs cgbuf (pop 0) (Push [ilRetTy]) [AI_ldnull;  I_unbox_any ilRetTy; I_br after3.CodeLabel ]
            
            CG.SetMarkToHere cgbuf after2
            GenExpr cenv cgbuf eenv SPSuppress arg1 Continue
            CG.EmitInstr cgbuf (pop 1) Push0 I_throw
            CG.SetMarkToHere cgbuf after3
            GenSequel cenv eenv.cloc cgbuf sequel
    | _ -> 
      // float or float32 or float<_> or float32<_>
      let g = cenv.g in 
      let anyfpType ty = typeEquivAux EraseMeasures g g.float_ty ty ||  typeEquivAux EraseMeasures g g.float32_ty ty 

      // Otherwise generate the arguments, and see if we can use a I_brcmp rather than a comparison followed by an I_brfalse/I_brtrue 
      GenExprs cenv cgbuf eenv args
      match ilAfterInst,sequel with

      // NOTE: THESE ARE NOT VALID ON FLOATING POINT DUE TO NaN.  Hence INLINE ASM ON FP. MUST BE CAREFULLY WRITTEN  

      | [ AI_clt ], CmpThenBrOrContinue(1,[ I_brcmp (BI_brfalse, label1) ]) when not (anyfpType (tyOfExpr g args.Head)) ->
        CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_bge,label1))
      | [ AI_cgt ], CmpThenBrOrContinue(1,[ I_brcmp (BI_brfalse, label1) ]) when not (anyfpType (tyOfExpr g args.Head)) ->
        CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_ble,label1))
      | [ AI_clt_un ], CmpThenBrOrContinue(1,[ I_brcmp (BI_brfalse, label1) ]) when not (anyfpType (tyOfExpr g args.Head)) ->
        CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_bge_un,label1))
      | [ AI_cgt_un ], CmpThenBrOrContinue(1, [I_brcmp (BI_brfalse, label1) ]) when not (anyfpType (tyOfExpr g args.Head)) ->
        CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_ble_un,label1))
      | [ AI_ceq ], CmpThenBrOrContinue(1,[ I_brcmp (BI_brfalse, label1) ]) when not (anyfpType (tyOfExpr g args.Head)) ->
        CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_bne_un,label1))
        
      // THESE ARE VALID ON FP w.r.t. NaN 
        
      | [ AI_clt ], CmpThenBrOrContinue(1,[ I_brcmp (BI_brtrue, label1) ]) ->
        CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_blt,label1))
      | [ AI_cgt ], CmpThenBrOrContinue(1,[ I_brcmp (BI_brtrue, label1) ]) ->
        CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_bgt,label1))
      | [ AI_clt_un ], CmpThenBrOrContinue(1,[ I_brcmp (BI_brtrue, label1) ]) ->
        CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_blt_un,label1))
      | [ AI_cgt_un ], CmpThenBrOrContinue(1,[ I_brcmp (BI_brtrue, label1) ]) ->
        CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_bgt_un,label1))
      | [ AI_ceq ], CmpThenBrOrContinue(1, [ I_brcmp (BI_brtrue, label1) ]) ->
        CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_beq,label1))
      | _ -> 
        // Failing that, generate the real IL leaving value(s) on the stack 
        CG.EmitInstrs cgbuf (pop args.Length) (Push ilReturnTys) ilAfterInst

        // If no return values were specified generate a "unit" 
        if isNil returnTys then 
          GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel
        else 
          GenSequel cenv eenv.cloc cgbuf sequel

//--------------------------------------------------------------------------
// Generate expression quotations
//-------------------------------------------------------------------------- 

and GenQuotation cenv cgbuf eenv (ast,conv,m,ety) sequel =

    let referencedTypeDefs, spliceTypes, spliceArgExprs, astSpec = 
        match !conv with  
        | Some res -> res
        | None -> 
            try 
                let qscope = QuotationTranslator.QuotationGenerationScope.Create (cenv.g, cenv.amap, cenv.viewCcu, QuotationTranslator.IsReflectedDefinition.No)
                let astSpec = QuotationTranslator.ConvExprPublic qscope QuotationTranslator.QuotationTranslationEnv.Empty ast 
                let referencedTypeDefs, spliceTypes, spliceArgExprs = qscope.Close()
                referencedTypeDefs, List.map fst spliceTypes, List.map fst spliceArgExprs, astSpec
            with 
                QuotationTranslator.InvalidQuotedTerm e -> error(e)

    let astSerializedBytes = QuotationPickler.pickle astSpec

    let someTypeInModuleExpr =  mkTypeOfExpr cenv m eenv.someTypeInThisAssembly
    let rawTy = mkRawQuotedExprTy cenv.g                          
    let spliceTypeExprs = List.map (GenType cenv.amap m eenv.tyenv >> (mkTypeOfExpr cenv m)) spliceTypes 

    let bytesExpr = Expr.Op(TOp.Bytes(astSerializedBytes),[],[],m)

    let deserializeExpr = 
        match QuotationTranslator.QuotationGenerationScope.ComputeQuotationFormat cenv.g with
        | QuotationTranslator.QuotationSerializationFormat.FSharp_40_Plus ->
            let referencedTypeDefExprs =  List.map (mkILNonGenericBoxedTy >> mkTypeOfExpr cenv m) referencedTypeDefs
            let referencedTypeDefsExpr = mkArray (cenv.g.system_Type_typ, referencedTypeDefExprs, m) 
            let spliceTypesExpr = mkArray (cenv.g.system_Type_typ, spliceTypeExprs, m)
            let spliceArgsExpr = mkArray (rawTy, spliceArgExprs, m)
            mkCallDeserializeQuotationFSharp40Plus cenv.g m someTypeInModuleExpr referencedTypeDefsExpr spliceTypesExpr spliceArgsExpr bytesExpr

        | QuotationTranslator.QuotationSerializationFormat.FSharp_20_Plus ->
            let mkList ty els = List.foldBack (mkCons cenv.g ty) els (mkNil cenv.g m ty)
            let spliceTypesExpr = mkList cenv.g.system_Type_typ spliceTypeExprs 
            let spliceArgsExpr = mkList rawTy spliceArgExprs 
            mkCallDeserializeQuotationFSharp20Plus cenv.g m someTypeInModuleExpr spliceTypesExpr spliceArgsExpr bytesExpr

    let afterCastExpr = 
        // Detect a typed quotation and insert the cast if needed. The cast should not fail but does
        // unfortunately involve a "typeOf" computation over a quotation tree.
        if tyconRefEq cenv.g (tcrefOfAppTy cenv.g ety) cenv.g.expr_tcr then 
            mkCallCastQuotation cenv.g m (List.head (argsOfAppTy cenv.g ety)) deserializeExpr
        else
            deserializeExpr
    GenExpr cenv cgbuf eenv SPSuppress afterCastExpr sequel

//--------------------------------------------------------------------------
// Generate calls to IL methods
//-------------------------------------------------------------------------- 

and GenILCall cenv cgbuf eenv (virt,valu,newobj,valUseFlags,isDllImport,ilMethRef:ILMethodRef,enclArgTys,methArgTys,argExprs,returnTys,m) sequel =
    let hasByrefArg  =  ilMethRef.ArgTypes |> List.exists IsILTypeByref
    let isSuperInit = match valUseFlags with CtorValUsedAsSuperInit -> true | _ -> false
    let isBaseCall = match valUseFlags with VSlotDirectCall -> true | _ -> false
    let ccallInfo = match valUseFlags with PossibleConstrainedCall ty -> Some ty | _ -> None
    let boxity = (if valu then AsValue else AsObject)
    let mustGenerateUnitAfterCall = isNil returnTys
    let makesNoCriticalTailcalls = (newobj || not virt) // Don't tailcall for 'newobj', or 'call' to IL code
    let tail = CanTailcall(valu,ccallInfo,eenv.withinSEH,hasByrefArg,mustGenerateUnitAfterCall,isDllImport,false,makesNoCriticalTailcalls,sequel)
    
    let ilEnclArgTys = GenTypeArgs cenv.amap m eenv.tyenv enclArgTys
    let ilMethArgTys = GenTypeArgs cenv.amap m eenv.tyenv methArgTys
    let ilReturnTys = GenTypes cenv.amap m eenv.tyenv returnTys
    let ilMethSpec = mkILMethSpec (ilMethRef,boxity,ilEnclArgTys,ilMethArgTys)
    let useICallVirt = virt || useCallVirt cenv boxity ilMethSpec isBaseCall

    // Load the 'this' pointer to pass to the superclass constructor. This argument is not 
    // in the expression tree since it can't be treated like an ordinary value 
    if isSuperInit then CG.EmitInstrs cgbuf (pop 0) (Push [ilMethSpec.EnclosingType]) [ mkLdarg0 ] 
    GenExprs cenv cgbuf eenv argExprs
    let il = 
        if newobj then [ I_newobj(ilMethSpec,None) ] 
        else 
            match ccallInfo with 
            | Some objArgTy -> 
                let ilObjArgTy = GenType cenv.amap m eenv.tyenv objArgTy
                [ I_callconstraint(tail,ilObjArgTy,ilMethSpec,None) ] 
            | None -> 
                if useICallVirt then [ I_callvirt(tail,ilMethSpec,None) ] 
                else  [ I_call(tail,ilMethSpec,None) ]

    CG.EmitInstrs cgbuf (pop (argExprs.Length + (if isSuperInit then 1 else 0))) (if isSuperInit then Push0 else Push ilReturnTys) il

    // Load the 'this' pointer as the pretend 'result' of the isSuperInit operation.  
    // It will be immediately popped in most cases, but may also be used as the target of some "property set" operations. 
    if isSuperInit then CG.EmitInstrs cgbuf (pop 0) (Push [ilMethSpec.EnclosingType]) [ mkLdarg0 ] 
    CommitCallSequel cenv eenv m eenv.cloc cgbuf mustGenerateUnitAfterCall sequel

and CommitCallSequel cenv eenv m cloc cgbuf mustGenerateUnitAfterCall sequel =
    if mustGenerateUnitAfterCall 
    then GenUnitThenSequel cenv eenv m cloc cgbuf sequel
    else GenSequel cenv cloc cgbuf sequel


and GenTraitCall cenv cgbuf eenv (traitInfo, argExprs, m) expr sequel =
    let minfoOpt = CommitOperationResult (ConstraintSolver.CodegenWitnessThatTypSupportsTraitConstraint cenv.TcVal cenv.g cenv.amap m traitInfo argExprs)
    match minfoOpt with 
    | None -> 
        let replacementExpr = 
            mkThrow m (tyOfExpr cenv.g expr)
               (mkExnExpr(cenv.g.FindSysTyconRef ["System"] "NotSupportedException", 
                             [ mkString cenv.g m (FSComp.SR.ilDynamicInvocationNotSupported(traitInfo.MemberName))],m)) 
        GenExpr cenv cgbuf eenv SPSuppress replacementExpr sequel
    | Some expr -> 
        let expr = cenv.optimizeDuringCodeGen expr
        GenExpr cenv cgbuf eenv SPSuppress expr sequel 

//--------------------------------------------------------------------------
// Generate byref-related operations
//-------------------------------------------------------------------------- 

and GenGetAddrOfRefCellField cenv cgbuf eenv (e,ty,m) sequel =
    GenExpr cenv cgbuf eenv SPSuppress e Continue
    let fref = GenRecdFieldRef m cenv eenv.tyenv (mkRefCellContentsRef cenv.g) [ty]
    CG.EmitInstrs cgbuf (pop 1) (Push [ILType.Byref fref.ActualType]) [ I_ldflda fref ] 
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetValAddr cenv cgbuf eenv (v: ValRef, m) sequel =
    let vspec = v.Deref
    let ilTy = GenTypeOfVal cenv eenv vspec
    match StorageForValRef m v eenv with 
    | Local (idx,None) ->
        CG.EmitInstrs cgbuf (pop 0) (Push [ILType.Byref ilTy]) [ I_ldloca (uint16 idx) ] 
    | Arg idx ->
        CG.EmitInstrs cgbuf (pop 0) (Push [ILType.Byref ilTy]) [ I_ldarga (uint16 idx) ] 
    | StaticField (fspec, _vref, hasLiteralAttr, _ilTyForProperty, _, ilTy, _, _, _) ->  
        if hasLiteralAttr then errorR(Error(FSComp.SR.ilAddressOfLiteralFieldIsInvalid(),m))
        let ilTy = if ilTy.IsNominal && ilTy.Boxity = ILBoxity.AsValue then ILType.Byref ilTy else ilTy
        EmitGetStaticFieldAddr cgbuf ilTy fspec
    | Env (_,_,ilField,_) -> 
        CG.EmitInstrs cgbuf (pop 0) (Push [ILType.Byref ilTy]) [ mkLdarg0; mkNormalLdflda ilField ] 
    | Local (_,Some _) | StaticProperty _ | Method _ | Env _  | Null ->  
        errorR(Error(FSComp.SR.ilAddressOfValueHereIsInvalid(v.DisplayName),m))
        CG.EmitInstrs cgbuf (pop 1) (Push [ILType.Byref ilTy]) [ I_ldarga (uint16 669 (* random value for post-hoc diagnostic analysis on generated tree *) ) ] ;

    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetByref cenv cgbuf eenv (v:ValRef,m) sequel =
    GenGetLocalVRef cenv cgbuf eenv m v None
    let ilty = GenType cenv.amap m eenv.tyenv (destByrefTy cenv.g v.Type)
    CG.EmitInstrs cgbuf (pop 1) (Push [ilty]) [ mkNormalLdobj ilty ]
    GenSequel cenv eenv.cloc cgbuf sequel

and GenSetByref cenv cgbuf eenv (v:ValRef,e,m) sequel =
    GenGetLocalVRef cenv cgbuf eenv m v None
    GenExpr cenv cgbuf eenv SPSuppress e Continue
    let ilty = GenType cenv.amap m eenv.tyenv (destByrefTy cenv.g v.Type)
    CG.EmitInstrs cgbuf (pop 2) Push0 [ mkNormalStobj ilty ]
    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

and GenDefaultValue cenv cgbuf eenv (ty,m) =
    let ilTy = GenType cenv.amap m eenv.tyenv ty
    if isRefTy cenv.g ty then 
        CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) AI_ldnull
    else
        match tryDestAppTy cenv.g ty with 
        | Some tcref when (tyconRefEq cenv.g cenv.g.system_SByte_tcref tcref || 
                           tyconRefEq cenv.g cenv.g.system_Int16_tcref tcref || 
                           tyconRefEq cenv.g cenv.g.system_Int32_tcref tcref || 
                           tyconRefEq cenv.g cenv.g.system_Bool_tcref tcref || 
                           tyconRefEq cenv.g cenv.g.system_Byte_tcref tcref || 
                           tyconRefEq cenv.g cenv.g.system_Char_tcref tcref || 
                           tyconRefEq cenv.g cenv.g.system_UInt16_tcref tcref || 
                           tyconRefEq cenv.g cenv.g.system_UInt32_tcref tcref) ->
            CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) iLdcZero
        | Some tcref when (tyconRefEq cenv.g cenv.g.system_Int64_tcref tcref || 
                           tyconRefEq cenv.g cenv.g.system_UInt64_tcref tcref) ->
            CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (iLdcInt64 0L)
        | Some tcref when (tyconRefEq cenv.g cenv.g.system_Single_tcref tcref) ->
            CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (iLdcSingle 0.0f)
        | Some tcref when (tyconRefEq cenv.g cenv.g.system_Double_tcref tcref) ->
            CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (iLdcDouble 0.0)
        | _ -> 
            let ilTy = GenType cenv.amap m eenv.tyenv ty
            LocalScope "ilzero" cgbuf (fun scopeMarks ->
                let locIdx, _ = AllocLocal cenv cgbuf eenv true (ilxgenGlobalNng.FreshCompilerGeneratedName ("default",m), ilTy, false) scopeMarks
                // "initobj" (Generated by EmitInitLocal) doesn't work on byref types 
                // But ilzero(&ty) only gets generated in the built-in get-address function so 
                // we can just rely on zeroinit of all IL locals. 
                match ilTy with 
                |  ILType.Byref _ -> ()
                | _ -> EmitInitLocal cgbuf ilTy locIdx
                EmitGetLocal cgbuf ilTy locIdx
            )

//--------------------------------------------------------------------------
// Generate generic parameters
//-------------------------------------------------------------------------- 

and GenGenericParam cenv eenv (tp:Typar) = 
    let subTypeConstraints             = tp.Constraints |> List.choose (function | TyparConstraint.CoercesTo(ty,_) -> Some(ty) | _ -> None) |> List.map (GenTypeAux cenv.amap tp.Range eenv.tyenv VoidNotOK PtrTypesNotOK)
    let refTypeConstraint              = tp.Constraints |> List.exists (function TyparConstraint.IsReferenceType _ -> true | TyparConstraint.SupportsNull _ -> true | _ -> false)
    let notNullableValueTypeConstraint = tp.Constraints |> List.exists (function TyparConstraint.IsNonNullableStruct _ -> true | _ -> false)
    let defaultConstructorConstraint   = tp.Constraints |> List.exists (function TyparConstraint.RequiresDefaultConstructor _ -> true | _ -> false)
    { Name= 

          // use the CompiledName if given
          // Inference variables get given an IL name "TA, TB" etc.
          let nm = 
              match tp.typar_il_name with 
              | None -> tp.Name  
              | Some nm -> nm
          // Some special rules apply when compiling Fsharp.Core.dll to avoid a proliferation of [<CompiledName>] attributes on type parameters
          if cenv.g.compilingFslib then 
              match nm with 
              | "U" -> "TResult"
              | "U1" -> "TResult1"
              | "U2" -> "TResult2"
              | _ -> 
                  if nm.TrimEnd([| '0' .. '9' |]).Length = 1 then nm 
                  elif nm.Length >= 1 && nm.[0] = 'T' && (nm.Length = 1 || not (System.Char.IsLower nm.[1]))  then nm
                  else "T" + (String.capitalize nm)
          else 
               nm 

      Constraints = subTypeConstraints
      Variance=NonVariant
      CustomAttrs = mkILCustomAttrs (GenAttrs cenv eenv tp.Attribs)
      HasReferenceTypeConstraint=refTypeConstraint
      HasNotNullableValueTypeConstraint=notNullableValueTypeConstraint
      HasDefaultConstructorConstraint= defaultConstructorConstraint }

//--------------------------------------------------------------------------
// Generate object expressions as ILX "closures"
//-------------------------------------------------------------------------- 

/// Generates the data used for parameters at definitions of abstract method slots such as interface methods or override methods.
and GenSlotParam m cenv eenv (TSlotParam(nm,ty,inFlag,outFlag,optionalFlag,attribs)) : ILParameter = 
    let inFlag2,outFlag2,optionalFlag2,defaultParamValue,paramMarshal2,attribs = GenParamAttribs cenv attribs
    
    { Name=nm
      Type= GenParamType cenv.amap m eenv.tyenv ty
      Default=defaultParamValue  
      Marshal=paramMarshal2 
      IsIn=inFlag || inFlag2
      IsOut=outFlag || outFlag2
      IsOptional=optionalFlag || optionalFlag2
      CustomAttrs= mkILCustomAttrs (GenAttrs cenv eenv attribs) }
    
and GenFormalSlotsig m cenv eenv (TSlotSig(_,typ,ctps,mtps,paraml,returnTy)) = 
    let paraml = List.concat paraml
    let ilTy = GenType cenv.amap m eenv.tyenv typ
    let eenvForSlotSig = EnvForTypars (ctps @ mtps) eenv
    let ilParams = paraml |> List.map (GenSlotParam m cenv eenvForSlotSig) 
    let ilRetTy = GenReturnType cenv.amap m eenvForSlotSig.tyenv returnTy
    let ilReturn = mkILReturn  ilRetTy
    ilTy, ilParams,ilReturn

and instSlotParam inst (TSlotParam(nm,ty,inFlag,fl2,fl3,attrs)) = TSlotParam(nm,instType inst ty,inFlag,fl2,fl3,attrs) 

and GenActualSlotsig m cenv eenv (TSlotSig(_,typ,ctps,mtps,paraml,returnTy)) methTyparsOfOverridingMethod = 
    let paraml = List.concat paraml
    let instForSlotSig = mkTyparInst (ctps@mtps) (argsOfAppTy cenv.g typ @ generalizeTypars methTyparsOfOverridingMethod)
    let ilParams = paraml |> List.map (instSlotParam instForSlotSig >> GenSlotParam m cenv eenv) 
    let ilRetTy = GenReturnType cenv.amap m eenv.tyenv (Option.map (instType instForSlotSig) returnTy)
    let ilReturn = mkILReturn ilRetTy
    ilParams,ilReturn

and GenNameOfOverridingMethod cenv (useMethodImpl,(TSlotSig(nameOfOverridenMethod,enclTypOfOverridenMethod,_,_,_,_))) =
    if useMethodImpl then qualifiedMangledNameOfTyconRef (tcrefOfAppTy cenv.g enclTypOfOverridenMethod) nameOfOverridenMethod else nameOfOverridenMethod

and GenMethodImpl cenv eenv (useMethodImpl,(TSlotSig(nameOfOverridenMethod,_,_,_,_,_) as slotsig)) m =
    let ilOverrideTy,ilOverrideParams,ilOverrideRet = GenFormalSlotsig m cenv eenv slotsig

    let nameOfOverridingMethod = GenNameOfOverridingMethod cenv (useMethodImpl,slotsig)
    nameOfOverridingMethod, 
    (fun (ilTyForOverriding,methTyparsOfOverridingMethod) -> 
        let ilOverrideTyRef = ilOverrideTy.TypeRef
        let ilOverrideMethRef = mkILMethRef(ilOverrideTyRef, ILCallingConv.Instance, nameOfOverridenMethod, List.length (DropErasedTypars methTyparsOfOverridingMethod), (typesOfILParams ilOverrideParams), ilOverrideRet.Type)
        let eenvForOverrideBy = AddTyparsToEnv methTyparsOfOverridingMethod eenv 
        let ilParamsOfOverridingMethod,ilReturnOfOverridingMethod = GenActualSlotsig m cenv eenvForOverrideBy slotsig methTyparsOfOverridingMethod
        let ilOverrideMethGenericParams = GenGenericParams cenv eenvForOverrideBy methTyparsOfOverridingMethod 
        let ilOverrideMethGenericArgs = mkILFormalGenericArgs 0 ilOverrideMethGenericParams
        let ilOverrideBy = mkILInstanceMethSpecInTy(ilTyForOverriding, nameOfOverridingMethod, typesOfILParams ilParamsOfOverridingMethod, ilReturnOfOverridingMethod.Type, ilOverrideMethGenericArgs)
        { Overrides = OverridesSpec(ilOverrideMethRef,ilOverrideTy)
          OverrideBy = ilOverrideBy })

and bindBaseOrThisVarOpt cenv eenv baseValOpt = 
    match baseValOpt with 
    | None -> eenv
    | Some basev -> AddStorageForVal cenv.g (basev,notlazy (Arg 0))  eenv  

and fixupVirtualSlotFlags mdef = 
    {mdef with
        IsHideBySig=true 
        mdKind = (match mdef.mdKind with 
                   | MethodKind.Virtual vinfo -> 
                      MethodKind.Virtual
                         {vinfo with 
                             IsCheckAccessOnOverride=false }
                   | _ -> failwith "fixupVirtualSlotFlags") } 

and renameMethodDef nameOfOverridingMethod (mdef : ILMethodDef) = 
    {mdef with Name=nameOfOverridingMethod }

and fixupMethodImplFlags mdef = 
    {mdef with 
               Access=ILMemberAccess.Private
               IsHideBySig=true 
               mdKind=(match mdef.mdKind with 
                         | MethodKind.Virtual vinfo -> 
                            MethodKind.Virtual
                               {vinfo with 
                                   IsCheckAccessOnOverride=false
                                   IsFinal=true
                                   IsNewSlot=true  }
                         | _ -> failwith "fixupMethodImpl") }

and GenObjectMethod cenv eenvinner (cgbuf:CodeGenBuffer) useMethodImpl tmethod =

    // Check if we're compiling the property as a .NET event
    let (TObjExprMethod(slotsig,attribs,methTyparsOfOverridingMethod,methodParams,methodBodyExpr,m)) = tmethod
    let (TSlotSig(nameOfOverridenMethod,_,_,_,_,_)) = slotsig
    if CompileAsEvent cenv.g attribs  then 
        []
    else
        let eenvUnderTypars = AddTyparsToEnv methTyparsOfOverridingMethod eenvinner
        let ilParamsOfOverridingMethod,ilReturnOfOverridingMethod = GenActualSlotsig m cenv eenvUnderTypars slotsig methTyparsOfOverridingMethod
        let ilAttribs = GenAttrs cenv eenvinner attribs

        // Args are stored starting at #1
        let methodParams = List.concat methodParams
        let eenvForMeth = AddStorageForLocalVals cenv.g (methodParams  |> List.mapi (fun i v -> (v,Arg i)))  eenvUnderTypars
        let ilMethodBody = CodeGenMethodForExpr cenv cgbuf.mgbuf (SPAlways,[],nameOfOverridenMethod,eenvForMeth,0,0,methodBodyExpr,(if slotSigHasVoidReturnTy slotsig then discardAndReturnVoid else Return))

        let nameOfOverridingMethod,methodImplGenerator = GenMethodImpl cenv eenvinner (useMethodImpl,slotsig) methodBodyExpr.Range

        let mdef = 
            mkILGenericVirtualMethod
              (nameOfOverridingMethod,
               ILMemberAccess.Public,
               GenGenericParams cenv eenvUnderTypars methTyparsOfOverridingMethod,
               ilParamsOfOverridingMethod,
               ilReturnOfOverridingMethod,
               MethodBody.IL ilMethodBody)
        // fixup attributes to generate a method impl 
        let mdef = if useMethodImpl then fixupMethodImplFlags mdef else mdef
        let mdef = fixupVirtualSlotFlags mdef
        let mdef = { mdef with CustomAttrs = mkILCustomAttrs ilAttribs }
        [(useMethodImpl,methodImplGenerator,methTyparsOfOverridingMethod),mdef]

and GenObjectExpr cenv cgbuf eenvouter expr (baseType,baseValOpt,basecall,overrides,interfaceImpls,m)  sequel =
    let cloinfo,_,eenvinner  = GetIlxClosureInfo cenv m false None eenvouter expr 

    let cloAttribs = cloinfo.cloAttribs
    let cloFreeVars = cloinfo.cloFreeVars
    let ilCloLambdas = cloinfo.ilCloLambdas
    let cloName = cloinfo.cloName
    
    let ilxCloSpec = cloinfo.cloSpec
    let ilCloFreeVars = cloinfo.cloILFreeVars
    let ilCloGenericFormals = cloinfo.cloILGenericParams
    assert (isNil cloinfo.localTypeFuncDirectILGenericParams)
    let ilCloGenericActuals = cloinfo.cloSpec.GenericArgs
    let ilCloRetTy = cloinfo.cloILFormalRetTy
    let ilCloTypeRef = cloinfo.cloSpec.TypeRef
    let ilTyForOverriding = mkILBoxedTy ilCloTypeRef ilCloGenericActuals

    let eenvinner = bindBaseOrThisVarOpt cenv eenvinner baseValOpt
    let ilCtorBody = CodeGenMethodForExpr cenv cgbuf.mgbuf (SPAlways,[],cloName,eenvinner,1,0,basecall,discardAndReturnVoid)


    let genMethodAndOptionalMethodImpl tmethod useMethodImpl = 
        [ for ((useMethodImpl,methodImplGeneratorFunction,methTyparsOfOverridingMethod),mdef) in GenObjectMethod cenv eenvinner cgbuf useMethodImpl tmethod do
              let mimpl = (if useMethodImpl then Some(methodImplGeneratorFunction (ilTyForOverriding,methTyparsOfOverridingMethod)) else None)
              yield (mimpl,mdef) ]  

    let mimpls,mdefs = 
        [ for ov in overrides do 
              yield! genMethodAndOptionalMethodImpl ov (isInterfaceTy cenv.g baseType)
          for (_,tmethods) in interfaceImpls do 
             for tmethod in tmethods do 
                 yield! genMethodAndOptionalMethodImpl tmethod true ]
        |> List.unzip 

    let mimpls = mimpls |> List.choose id // choose the ones that actually have method impls

    let interfaceTys = interfaceImpls |> List.map (fst >> GenType cenv.amap m eenvinner.tyenv) 

    let attrs = GenAttrs cenv eenvinner cloAttribs
    let super = (if isInterfaceTy cenv.g baseType then cenv.g.ilg.typ_Object else ilCloRetTy)
    let interfaceTys = interfaceTys @ (if isInterfaceTy cenv.g baseType then [ilCloRetTy] else [])
    let cloTypeDefs = GenClosureTypeDefs cenv (ilCloTypeRef,ilCloGenericFormals,attrs,ilCloFreeVars,ilCloLambdas,ilCtorBody,mdefs,mimpls,super,interfaceTys)

    for cloTypeDef in cloTypeDefs do 
        cgbuf.mgbuf.AddTypeDef(ilCloTypeRef, cloTypeDef, false, false, None)
    CountClosure()
    GenGetLocalVals cenv cgbuf eenvouter m cloFreeVars
    CG.EmitInstr cgbuf (pop ilCloFreeVars.Length) (Push [ EraseClosures.mkTyOfLambdas cenv.g.ilxPubCloEnv ilCloLambdas]) (I_newobj (ilxCloSpec.Constructor,None))
    GenSequel cenv eenvouter.cloc cgbuf sequel

and GenSequenceExpr cenv (cgbuf:CodeGenBuffer) eenvouter (nextEnumeratorValRef:ValRef,pcvref:ValRef,currvref:ValRef,stateVars,generateNextExpr,closeExpr,checkCloseExpr:Expr,seqElemTy, m)  sequel =
    let stateVars = [ pcvref; currvref ] @ stateVars
    let stateVarsSet = stateVars |> List.map (fun vref -> vref.Deref) |> Zset.ofList valOrder 

    // pretend that the state variables are bound
    let eenvouter = 
        eenvouter |> AddStorageForLocalVals cenv.g (stateVars |> List.map (fun v -> v.Deref,Local(0,None)))
    
    // Get the free variables. Make a lambda to pretend that the 'nextEnumeratorValRef' is bound (it is an argument to GenerateNext)
    let (cloAttribs,_,_,cloFreeTyvars,cloFreeVars,ilCloTypeRef:ILTypeRef,ilCloFreeVars,eenvinner) = 
         GetIlxClosureFreeVars cenv m None eenvouter [] (mkLambda m nextEnumeratorValRef.Deref (generateNextExpr, cenv.g.int32_ty))

    let ilCloSeqElemTy = GenType cenv.amap m eenvinner.tyenv seqElemTy
    let cloRetTy = mkSeqTy cenv.g seqElemTy
    let ilCloRetTyInner = GenType cenv.amap m eenvinner.tyenv cloRetTy
    let ilCloRetTyOuter = GenType cenv.amap m eenvouter.tyenv cloRetTy
    let ilCloEnumeratorTy = GenType cenv.amap m eenvinner.tyenv (mkIEnumeratorTy cenv.g seqElemTy)
    let ilCloEnumerableTy = GenType cenv.amap m eenvinner.tyenv (mkSeqTy cenv.g seqElemTy)
    let ilCloBaseTy = GenType cenv.amap m eenvinner.tyenv (mkAppTy cenv.g.seq_base_tcr [seqElemTy])  
    let ilCloGenericParams = GenGenericParams cenv eenvinner cloFreeTyvars

    // Create a new closure class with a single "MoveNext" method that implements the iterator. 
    let ilCloTyInner = mkILFormalBoxedTy ilCloTypeRef ilCloGenericParams
    let ilCloLambdas = Lambdas_return ilCloRetTyInner 
    let cloref = IlxClosureRef(ilCloTypeRef, ilCloLambdas, ilCloFreeVars)
    let ilxCloSpec = IlxClosureSpec.Create(cloref, GenGenericArgs m eenvouter.tyenv cloFreeTyvars)
    let formalClospec = IlxClosureSpec.Create(cloref, mkILFormalGenericArgs 0 ilCloGenericParams)

    let getFreshMethod = 
        let _,mbody =
            CodeGenMethod cenv cgbuf.mgbuf (true,[],"GetFreshEnumerator",eenvinner,1,0,
                                            (fun cgbuf eenv -> 
                                                for fv in cloFreeVars do 
(*  TODO: Emit CompareExchange 
                                                        if (System.Threading.Interlocked.CompareExchange(&__state, 1, 0) = 0) then
                                                            (x :> IEnumerator<'T>)
                                                        else
                                                            ...
*)
                                                   /// State variables always get zero-initialized
                                                   if stateVarsSet.Contains fv then 
                                                       GenDefaultValue cenv cgbuf eenv (fv.Type,m) 
                                                   else
                                                       GenGetLocalVal cenv cgbuf eenv m fv None
                                                CG.EmitInstr cgbuf (pop ilCloFreeVars.Length) (Push [ilCloRetTyInner]) (I_newobj (formalClospec.Constructor,None))
                                                GenSequel cenv eenv.cloc cgbuf Return),
                                            m)
        mkILNonGenericVirtualMethod("GetFreshEnumerator",ILMemberAccess.Public, [], mkILReturn ilCloEnumeratorTy, MethodBody.IL mbody)
        |> AddNonUserCompilerGeneratedAttribs cenv.g

    let closeMethod = 
        // Note: We suppress the first sequence point in the body of this method since it is the initial state machine jump
        let spReq = SPSuppress
        mkILNonGenericVirtualMethod("Close",ILMemberAccess.Public, [], mkILReturn ILType.Void, MethodBody.IL (CodeGenMethodForExpr cenv cgbuf.mgbuf (spReq,[],"Close",eenvinner,1,0,closeExpr,discardAndReturnVoid)))

    let checkCloseMethod = 
        // Note: We suppress the first sequence point in the body of this method since it is the initial state machine jump
        let spReq = SPSuppress
        mkILNonGenericVirtualMethod("get_CheckClose",ILMemberAccess.Public, [], mkILReturn cenv.g.ilg.typ_Bool, MethodBody.IL (CodeGenMethodForExpr cenv cgbuf.mgbuf (spReq,[],"get_CheckClose",eenvinner,1,0,checkCloseExpr,Return)))

    let generateNextMethod = 
        // Note: We suppress the first sequence point in the body of this method since it is the initial state machine jump
        let spReq = SPSuppress
        // the 'next enumerator' byref arg is at arg position 1 
        let eenvinner = eenvinner |> AddStorageForLocalVals cenv.g [ (nextEnumeratorValRef.Deref, Arg 1) ]
        mkILNonGenericVirtualMethod("GenerateNext",ILMemberAccess.Public, [mkILParamNamed("next",ILType.Byref ilCloEnumerableTy)], mkILReturn cenv.g.ilg.typ_Int32, MethodBody.IL (CodeGenMethodForExpr cenv cgbuf.mgbuf (spReq,[],"GenerateNext",eenvinner,2,0,generateNextExpr,Return)))

    let lastGeneratedMethod = 
        mkILNonGenericVirtualMethod("get_LastGenerated",ILMemberAccess.Public, [], mkILReturn ilCloSeqElemTy, MethodBody.IL (CodeGenMethodForExpr cenv cgbuf.mgbuf (SPSuppress,[],"get_LastGenerated",eenvinner,1,0,exprForValRef m currvref,Return)))
        |> AddNonUserCompilerGeneratedAttribs cenv.g

    let ilCtorBody = 
        mkILSimpleStorageCtor(None, Some ilCloBaseTy.TypeSpec, ilCloTyInner, [], [], ILMemberAccess.Assembly).MethodBody

    let attrs = GenAttrs cenv eenvinner cloAttribs
    let cloTypeDefs = GenClosureTypeDefs cenv (ilCloTypeRef,ilCloGenericParams,attrs,ilCloFreeVars,ilCloLambdas,ilCtorBody,[generateNextMethod;closeMethod;checkCloseMethod;lastGeneratedMethod;getFreshMethod],[],ilCloBaseTy,[])
    for cloTypeDef in cloTypeDefs do 
        cgbuf.mgbuf.AddTypeDef(ilCloTypeRef, cloTypeDef, false, false, None)
    CountClosure()

    for fv in cloFreeVars do 
       /// State variables always get zero-initialized
       if stateVarsSet.Contains fv then 
           GenDefaultValue cenv cgbuf eenvouter (fv.Type,m) 
       else
           GenGetLocalVal cenv cgbuf eenvouter m fv None
       
    CG.EmitInstr cgbuf (pop ilCloFreeVars.Length) (Push [ilCloRetTyOuter]) (I_newobj (ilxCloSpec.Constructor,None))
    GenSequel cenv eenvouter.cloc cgbuf sequel



/// Generate the class for a closure type definition
and GenClosureTypeDefs cenv (tref:ILTypeRef, ilGenParams, attrs, ilCloFreeVars, ilCloLambdas, ilCtorBody, mdefs, mimpls,ext, ilIntfTys) =

  let cloInfo = 
      { cloFreeVars=ilCloFreeVars
        cloStructure=ilCloLambdas
        cloCode=notlazy ilCtorBody }

  let td = 
    { Name = tref.Name 
      Layout = ILTypeDefLayout.Auto
      Access =  ComputeTypeAccess tref true
      GenericParams = ilGenParams
      CustomAttrs = mkILCustomAttrs(attrs @ [mkCompilationMappingAttr cenv.g (int SourceConstructFlags.Closure) ])
      Fields = emptyILFields
      InitSemantics=ILTypeInit.BeforeField         
      IsSealed=true
      IsAbstract=false
      tdKind=ILTypeDefKind.Class
      Events= emptyILEvents
      Properties = emptyILProperties
      Methods= mkILMethods mdefs 
      MethodImpls= mkILMethodImpls mimpls 
      IsSerializable= cenv.g.attrib_SerializableAttribute.IsSome
      IsComInterop= false    
      IsSpecialName= true
      NestedTypes=emptyILTypeDefs
      Encoding= ILDefaultPInvokeEncoding.Auto
      Implements = ilIntfTys  
      Extends= Some ext
      SecurityDecls= emptyILSecurityDecls
      HasSecurity=false } 

  let tdefs = EraseClosures.convIlxClosureDef cenv.g.ilxPubCloEnv tref.Enclosing td cloInfo
  tdefs
          
and GenGenericParams cenv eenv tps =  tps |> DropErasedTypars |> List.map (GenGenericParam cenv eenv)
and GenGenericArgs m (tyenv:TypeReprEnv) tps = tps |> DropErasedTypars |> List.map (fun c -> (mkILTyvarTy tyenv.[c,m])) 

/// Generate the closure class for a function 
and GenLambdaClosure cenv (cgbuf:CodeGenBuffer) eenv isLocalTypeFunc selfv expr =
    match expr with 
    | Expr.Lambda (_,_,_,_,_,m,_) 
    | Expr.TyLambda(_,_,_,m,_) -> 
          
        let cloinfo,body,eenvinner  = GetIlxClosureInfo cenv  m isLocalTypeFunc selfv eenv expr 
          
        let entryPointInfo = 
          match selfv with 
          | Some v -> [(v, BranchCallClosure (cloinfo.cloArityInfo))]
          | _ -> []
        let ilCloBody = CodeGenMethodForExpr cenv cgbuf.mgbuf (SPAlways,entryPointInfo,cloinfo.cloName,eenvinner,1,0,body,Return)
        let ilCloTypeRef = cloinfo.cloSpec.TypeRef
        let cloTypeDefs = 
            if isLocalTypeFunc then 

                // Work out the contract type and generate a class with an abstract method for this type
                let (ilContractGenericParams,ilContractMethTyargs,ilContractTySpec:ILTypeSpec,ilContractFormalRetTy) = GenNamedLocalTypeFuncContractInfo cenv eenv m cloinfo
                let ilContractTypeRef = ilContractTySpec.TypeRef
                let ilContractTy = mkILFormalBoxedTy ilContractTypeRef ilContractGenericParams
                let ilContractCtor =  mkILNonGenericEmptyCtor None cenv.g.ilg.typ_Object

                let ilContractMeths = [ilContractCtor; mkILGenericVirtualMethod("DirectInvoke",ILMemberAccess.Assembly,ilContractMethTyargs,[],mkILReturn ilContractFormalRetTy, MethodBody.Abstract) ]

                let ilContractTypeDef = 
                    { Name = ilContractTypeRef.Name 
                      Layout = ILTypeDefLayout.Auto
                      Access =  ComputeTypeAccess ilContractTypeRef true
                      GenericParams = ilContractGenericParams
                      CustomAttrs = mkILCustomAttrs [mkCompilationMappingAttr cenv.g (int SourceConstructFlags.Closure) ]
                      Fields = emptyILFields
                      InitSemantics=ILTypeInit.BeforeField         
                      IsSealed=false  // the contract type is an abstract type and not sealed
                      IsAbstract=true // the contract type is an abstract type
                      tdKind=ILTypeDefKind.Class
                      Events= emptyILEvents
                      Properties = emptyILProperties
                      Methods= mkILMethods ilContractMeths 
                      MethodImpls= emptyILMethodImpls 
                      IsSerializable= cenv.g.attrib_SerializableAttribute.IsSome 
                      IsComInterop=false    
                      IsSpecialName= true
                      NestedTypes=emptyILTypeDefs
                      Encoding= ILDefaultPInvokeEncoding.Auto
                      Implements = []  
                      Extends= Some cenv.g.ilg.typ_Object
                      SecurityDecls= emptyILSecurityDecls
                      HasSecurity=false } 
                cgbuf.mgbuf.AddTypeDef(ilContractTypeRef, ilContractTypeDef, false, false, None)
                
                let ilCtorBody =  mkILMethodBody (true,[],8,nonBranchingInstrsToCode (mkCallBaseConstructor(ilContractTy,[])), None )
                let cloMethods = [ mkILGenericVirtualMethod("DirectInvoke",ILMemberAccess.Assembly,cloinfo.localTypeFuncDirectILGenericParams,[],mkILReturn (cloinfo.cloILFormalRetTy), MethodBody.IL ilCloBody) ]
                let cloTypeDefs = GenClosureTypeDefs cenv (ilCloTypeRef,cloinfo.cloILGenericParams,[],cloinfo.cloILFreeVars,cloinfo.ilCloLambdas,ilCtorBody,cloMethods,[],ilContractTy,[])
                cloTypeDefs
                
            else 
                GenClosureTypeDefs cenv (ilCloTypeRef,cloinfo.cloILGenericParams,[],cloinfo.cloILFreeVars,cloinfo.ilCloLambdas,ilCloBody,[],[],cenv.g.ilg.typ_Object,[])
        CountClosure()
        for cloTypeDef in cloTypeDefs do 
            cgbuf.mgbuf.AddTypeDef(ilCloTypeRef, cloTypeDef, false, false, None)
        cloinfo,m
    |     _ -> failwith "GenLambda: not a lambda"
        
and GenLambdaVal cenv (cgbuf:CodeGenBuffer) eenv (cloinfo,m) =
    GenGetLocalVals cenv cgbuf eenv m cloinfo.cloFreeVars
    CG.EmitInstr cgbuf 
        (pop cloinfo.cloILFreeVars.Length) 
        (Push [EraseClosures.mkTyOfLambdas cenv.g.ilxPubCloEnv cloinfo.ilCloLambdas]) 
        (I_newobj (cloinfo.cloSpec.Constructor,None))

and GenLambda cenv cgbuf eenv isLocalTypeFunc selfv expr sequel =
    let cloinfo,m = GenLambdaClosure cenv cgbuf eenv isLocalTypeFunc selfv expr
    GenLambdaVal cenv cgbuf eenv (cloinfo,m)
    GenSequel cenv eenv.cloc cgbuf sequel

and GenTypeOfVal cenv eenv (v:Val) = 
    GenType cenv.amap v.Range eenv.tyenv v.Type

and GenFreevar cenv m eenvouter tyenvinner (fv:Val) = 
    match StorageForVal m fv eenvouter with 
    // Local type functions
    | Local(_,Some _) | Env(_,_,_,Some _) -> cenv.g.ilg.typ_Object
#if DEBUG
    // Check for things that should never make it into the free variable set. Only do this in debug for performance reasons
    | (StaticField _ | StaticProperty _ | Method _ | Null) -> error(InternalError("GenFreevar: compiler error: unexpected unrealized value",fv.Range))
#endif
    | _ -> GenType cenv.amap m tyenvinner fv.Type

and GetIlxClosureFreeVars cenv m selfv eenvouter takenNames expr =

    // Choose a base name for the closure
    let basename = 
        let boundv = eenvouter.letBoundVars |> List.tryFind (fun v -> not v.IsCompilerGenerated) 
        match boundv with
        | Some v -> v.CompiledName
        | None -> "clo"

    // Get a unique stamp for the closure. This must be stable for things that can be part of a let rec.
    let uniq = 
        match expr with 
        | Expr.Obj (uniq,_,_,_,_,_,_) 
        | Expr.Lambda (uniq,_,_,_,_,_,_) 
        | Expr.TyLambda(uniq,_,_,_,_) -> uniq
        | _ -> newUnique()

    // Choose a name for the closure
    let ilCloTypeRef = 
        // FSharp 1.0 bug 3404: System.Reflection doesn't like '.' and '`' in type names
        let basenameSafeForUseAsTypename = CleanUpGeneratedTypeName basename
        let suffixmark = expr.Range
        let cloName = globalStableNameGenerator.GetUniqueCompilerGeneratedName(basenameSafeForUseAsTypename,suffixmark,uniq)
        NestedTypeRefForCompLoc eenvouter.cloc cloName

    // Collect the free variables of the closure
    let cloFreeVarResults =  freeInExpr CollectTyparsAndLocals expr

    // Partition the free variables when some can be accessed from places besides the immediate environment 
    // Also filter out the current value being bound, if any, as it is available from the "this" 
    // pointer which gives the current closure itself. This is in the case e.g. let rec f = ... f ... 
    let cloFreeVars = 
        cloFreeVarResults.FreeLocals
        |> Zset.elements 
        |> List.filter (fun fv -> 
            match StorageForVal m fv eenvouter with 
            | (StaticField _ | StaticProperty _ | Method _ | Null) -> false
            | _ -> 
                match selfv with 
                | Some v -> not (valRefEq cenv.g (mkLocalValRef fv) v) 
                | _ -> true)

    // The general shape is:
    //    {LAM <tyfunc-typars>. expr }[free-typars] : overall-type[contract-typars]
    // Then
    //    internal-typars = free-typars - contract-typars
    //
    // In other words, the free type variables get divided into two sets
    //  -- "contract" ones, which are part of the return type. We separate these to enable use to 
    //     bake our own function base contracts for local type functions
    //
    //  -- "internal" ones, which get used internally in the implementation
    let cloContractFreeTyvarSet = (freeInType CollectTypars (tyOfExpr cenv.g expr)).FreeTypars 
    
    let cloInternalFreeTyvars = Zset.diff  cloFreeVarResults.FreeTyvars.FreeTypars cloContractFreeTyvarSet |> Zset.elements
    let cloContractFreeTyvars = cloContractFreeTyvarSet |> Zset.elements
    
    let cloFreeTyvars = cloContractFreeTyvars @ cloInternalFreeTyvars
    
    let cloAttribs = []

    let eenvinner = eenvouter |> EnvForTypars cloFreeTyvars

    let ilCloTyInner = 
        let ilCloGenericParams = GenGenericParams cenv eenvinner cloFreeTyvars
        mkILFormalBoxedTy ilCloTypeRef ilCloGenericParams

    // If generating a named closure, add the closure itself as a var, available via "arg0" . 
    // The latter doesn't apply for the delegate implementation of closures. 
    // Build the environment that is active inside the closure itself
    let eenvinner = eenvinner |> AddStorageForLocalVals cenv.g (match selfv with | Some v  -> [(v.Deref, Arg 0)] | _ -> [])

    let ilCloFreeVars = 
        let ilCloFreeVarNames = ChooseFreeVarNames takenNames (List.map nameOfVal cloFreeVars)   
        let ilCloFreeVars = (cloFreeVars,ilCloFreeVarNames) ||> List.map2 (fun fv nm -> mkILFreeVar (nm,fv.IsCompilerGenerated, GenFreevar cenv m eenvouter eenvinner.tyenv fv))  
        ilCloFreeVars

    let ilCloFreeVarStorage = 
        (cloFreeVars,ilCloFreeVars) ||> List.mapi2 (fun i v fv -> 
            let localCloInfo = 
                match StorageForVal m v eenvouter with 
                | Local(_,localCloInfo) 
                | Env(_,_,_,localCloInfo) -> localCloInfo
                | _ -> None
            let ilField = mkILFieldSpecInTy (ilCloTyInner,fv.fvName,fv.fvType)

            (v,Env(ilCloTyInner,i,ilField,localCloInfo)))

    let eenvinner = eenvinner |> AddStorageForLocalVals cenv.g ilCloFreeVarStorage

    
    // Return a various results
    (cloAttribs,cloInternalFreeTyvars,cloContractFreeTyvars,cloFreeTyvars,cloFreeVars,ilCloTypeRef,Array.ofList ilCloFreeVars,eenvinner)


and GetIlxClosureInfo cenv m isLocalTypeFunc  selfv eenvouter expr =
    let returnTy = 
      match expr with 
      | Expr.Lambda (_,_,_,_,_,_,returnTy) | Expr.TyLambda(_,_,_,_,returnTy) -> returnTy
      | Expr.Obj(_,typ,_,_,_,_,_) -> typ
      | _ -> failwith "GetIlxClosureInfo: not a lambda expression"

    // Determine the structure of the closure. We do this before analyzing free variables to
    // determine the taken argument names.
    let tvsl, vs, body, returnTy = 
        let rec getCallStructure tvacc vacc (e,ety) = 
            match e with 
            | Expr.TyLambda(_,tvs,body,_m,bty) -> 
                getCallStructure ((DropErasedTypars tvs) :: tvacc) vacc (body,bty)
            | Expr.Lambda (_,_,_,vs,body,_,bty) when not isLocalTypeFunc -> 
                // Transform a lambda taking untupled arguments into one 
                // taking only a single tupled argument if necessary.  REVIEW: do this earlier 
                let tupledv, body =  MultiLambdaToTupledLambda cenv.g vs body 
                getCallStructure tvacc (tupledv :: vacc) (body,bty)
            | _ -> 
                (List.rev tvacc, List.rev vacc, e, ety)
        getCallStructure [] [] (expr,returnTy)

    let takenNames = vs |> List.map (fun v -> v.CompiledName)

    // Get the free variables and the information about the closure, add the free variables to the environment
    let (cloAttribs,cloInternalFreeTyvars,cloContractFreeTyvars,_,cloFreeVars,ilCloTypeRef,ilCloFreeVars,eenvinner) = GetIlxClosureFreeVars cenv m selfv eenvouter takenNames expr

    // Put the type and value arguments into the environment
    let rec getClosureArgs eenv ntmargs tvsl (vs:Val list) = 
        match tvsl, vs with 
        | tvs :: rest, _ -> 
            let eenv = AddTyparsToEnv tvs eenv
            let l,eenv = getClosureArgs eenv ntmargs rest vs
            let lambdas = (tvs, l) ||> List.foldBack (fun tv sofar -> Lambdas_forall(GenGenericParam cenv eenv tv,sofar)) 
            lambdas,eenv
        | [], v :: rest -> 
            let nm = v.CompiledName
            let l,eenv = 
                let eenv = AddStorageForVal cenv.g (v,notlazy (Arg ntmargs)) eenv
                getClosureArgs eenv (ntmargs+1) [] rest
            let lambdas = Lambdas_lambda (mkILParamNamed(nm,GenTypeOfVal cenv eenv v),l)
            lambdas,eenv
        | _ -> 
            let returnTy' = GenType cenv.amap m eenv.tyenv returnTy
            Lambdas_return returnTy', eenv

    // start at arg number 1 as "this" pointer holds the current closure
    let ilCloLambdas,eenvinner = getClosureArgs eenvinner 1 tvsl vs

    // Arity info: one argument at each position
    let narginfo = vs |> List.map (fun _ -> 1)

    // Generate the ILX view of the lambdas
    let ilReturnTy = GenType cenv.amap m eenvinner.tyenv returnTy

    // The general shape is:
    //    {LAM <tyfunc-typars>. expr }[free-typars] : overall-type[contract-typars]
    // Then
    //    internal-typars = free-typars - contract-typars
    //
    // For a local type function closure, this becomes
    //    class Contract<contract-typars> {
    //        abstract DirectInvoke<tyfunc-typars> : overall-type
    //    }
    //
    //    class ContractImplementation<contract-typars, internal-typars> : Contract<contract-typars>  {
    //        override DirectInvoke<tyfunc-typars> : overall-type { expr }
    //    }
    //
    // For a non-local type function closure, this becomes
    //
    //    class FunctionImplementation<contract-typars, internal-typars> : FSharpTypeFunc  {
    //        override Specialize<tyfunc-typars> : overall-type { expr }
    //    }
    //
    // For a normal function closure, <tyfunc-typars> is empty, and this becomes
    //
    //    class FunctionImplementation<contract-typars, internal-typars> : overall-type<contract-typars>  {
    //        override Invoke(..) { expr }
    //    }
    
    // In other words, the free type variables get divided into two sets
    //  -- "contract" ones, which are part of the return type. We separate these to enable use to 
    //     bake our own function base contracts for local type functions
    //
    //  -- "internal" ones, which get used internally in the implementation
    //
    // There are also "direct" and "indirect" type variables, which are part of the lambdas of the type function.
    // Direct type variables are only used for local type functions, and indirect type variables only used for first class
    // function values.

    /// Compute the contract if it is a local type function
    let ilContractGenericParams  = GenGenericParams cenv eenvinner cloContractFreeTyvars
    let ilContractGenericActuals = GenGenericArgs m eenvouter.tyenv cloContractFreeTyvars
    let ilInternalGenericParams  = GenGenericParams cenv eenvinner cloInternalFreeTyvars
    let ilInternalGenericActuals = GenGenericArgs m eenvouter.tyenv cloInternalFreeTyvars

    let ilCloGenericFormals = ilContractGenericParams @ ilInternalGenericParams
    let ilCloGenericActuals = ilContractGenericActuals @ ilInternalGenericActuals

    
    let ilDirectGenericParams,ilReturnTy,ilCloLambdas = 
        if isLocalTypeFunc then 
            let rec strip lambdas acc = 
                match lambdas with 
                | Lambdas_forall(gp,r) -> strip r  (gp::acc)
                | Lambdas_return returnTy -> List.rev acc,returnTy,lambdas
                | _ -> failwith "AdjustNamedLocalTypeFuncIlxClosureInfo: local functions can currently only be type functions"
            strip ilCloLambdas []
        else 
            [],ilReturnTy,ilCloLambdas
        

    let ilxCloSpec = IlxClosureSpec.Create(IlxClosureRef(ilCloTypeRef, ilCloLambdas, ilCloFreeVars), ilCloGenericActuals)
    let cloinfo = 
        { cloExpr=expr
          cloName=ilCloTypeRef.Name
          cloArityInfo =narginfo
          ilCloLambdas=ilCloLambdas
          cloILFreeVars = ilCloFreeVars
          cloILFormalRetTy=ilReturnTy
          cloSpec = ilxCloSpec
          cloILGenericParams = ilCloGenericFormals
          cloFreeVars=cloFreeVars
          cloAttribs=cloAttribs
          localTypeFuncContractFreeTypars = cloContractFreeTyvars
          localTypeFuncInternalFreeTypars = cloInternalFreeTyvars 
          localTypeFuncILGenericArgs = ilContractGenericActuals
          localTypeFuncDirectILGenericParams = ilDirectGenericParams }
    cloinfo,body,eenvinner

//--------------------------------------------------------------------------
// Named local type functions
//-------------------------------------------------------------------------- 

and IsNamedLocalTypeFuncVal g (v:Val) expr =
    not v.IsCompiledAsTopLevel &&
    IsGenericValWithGenericContraints g v && 
    (match stripExpr expr with Expr.TyLambda _ -> true | _ -> false)
 
/// Generate the information relevant to the contract portion of a named local type function
and GenNamedLocalTypeFuncContractInfo cenv eenv m cloinfo = 
    let ilCloTypeRef = cloinfo.cloSpec.TypeRef
    let ilContractTypeRef = ILTypeRef.Create(scope=ilCloTypeRef.Scope,enclosing=ilCloTypeRef.Enclosing,name=ilCloTypeRef.Name + "$contract")
    let eenvForContract  = EnvForTypars cloinfo.localTypeFuncContractFreeTypars eenv
    let ilContractGenericParams = GenGenericParams cenv eenv cloinfo.localTypeFuncContractFreeTypars
    let tvs,contractRetTy  = 
        match cloinfo.cloExpr with 
        | Expr.TyLambda(_,tvs,_,_,bty) -> tvs, bty
        | e -> [], tyOfExpr cenv.g e
    let eenvForContract = AddTyparsToEnv tvs eenvForContract 
    let ilContractMethTyargs = GenGenericParams cenv eenvForContract tvs
    let ilContractFormalRetTy = GenType cenv.amap m eenvForContract.tyenv contractRetTy
    ilContractGenericParams,ilContractMethTyargs,mkILTySpec(ilContractTypeRef,cloinfo.localTypeFuncILGenericArgs),ilContractFormalRetTy

/// Generate a new delegate construction including a closure class if necessary. This is a lot like generating function closures
/// and object expression closures, and most of the code is shared.
and GenDelegateExpr cenv cgbuf eenvouter expr (TObjExprMethod((TSlotSig(_,delegateTy, _,_,_, _) as slotsig),_attribs,methTyparsOfOverridingMethod,tmvs,body,_),m) sequel =
    // Get the instantiation of the delegate type 
    let ilCtxtDelTy = GenType cenv.amap m eenvouter.tyenv delegateTy
    let tmvs = List.concat tmvs

    // Yuck. TLBIMP.EXE generated APIs use UIntPtr for the delegate ctor. 
    let useUIntPtrForDelegateCtor = 
        try 
            if isILAppTy cenv.g delegateTy then 
                let tcref = tcrefOfAppTy cenv.g delegateTy
                let tdef = tcref.ILTyconRawMetadata
                match tdef.Methods.FindByName ".ctor" with 
                | [ctorMDef] -> 
                    match ctorMDef.Parameters with 
                    | [ _;p2 ] -> (p2.Type.TypeSpec.Name = "System.UIntPtr")
                    | _ -> false
                | _ -> false
            else 
                false 
         with _ -> 
            false
        
    // Work out the free type variables for the morphing thunk 
    let takenNames = List.map nameOfVal tmvs
    let (cloAttribs,_,_,cloFreeTyvars,cloFreeVars,ilDelegeeTypeRef,ilCloFreeVars,eenvinner) = GetIlxClosureFreeVars cenv m None eenvouter takenNames expr
    let ilDelegeeGenericParams = GenGenericParams cenv eenvinner cloFreeTyvars
    let ilDelegeeGenericActualsInner = mkILFormalGenericArgs 0 ilDelegeeGenericParams

    // Create a new closure class with a single "delegee" method that implements the delegate. 
    let delegeeMethName = "Invoke"
    let ilDelegeeTyInner = mkILBoxedTy ilDelegeeTypeRef ilDelegeeGenericActualsInner

    let envForDelegeeUnderTypars = AddTyparsToEnv methTyparsOfOverridingMethod eenvinner

    // The slot sig contains a formal instantiation.  When creating delegates we're only 
    // interested in the actual instantiation since we don't have to emit a method impl. 
    let ilDelegeeParams,ilDelegeeRet = GenActualSlotsig m cenv envForDelegeeUnderTypars slotsig methTyparsOfOverridingMethod

    let numthis = 1
    let tmvs, body = BindUnitVars cenv.g (tmvs, List.replicate (List.concat slotsig.FormalParams).Length ValReprInfo.unnamedTopArg1, body)
    let envForDelegeeMeth = AddStorageForLocalVals cenv.g (List.mapi (fun i v -> (v,Arg (i+numthis))) tmvs)  envForDelegeeUnderTypars
    let ilMethodBody = CodeGenMethodForExpr cenv cgbuf.mgbuf (SPAlways,[],delegeeMethName,envForDelegeeMeth,1,0,body,(if slotSigHasVoidReturnTy slotsig then discardAndReturnVoid else Return))
    let delegeeInvokeMeth =
        mkILNonGenericInstanceMethod
            (delegeeMethName,ILMemberAccess.Assembly, 
             ilDelegeeParams, 
             ilDelegeeRet,
             MethodBody.IL ilMethodBody)
    let delegeeCtorMeth = mkILSimpleStorageCtor(None, Some cenv.g.ilg.typ_Object.TypeSpec, ilDelegeeTyInner, [], [], ILMemberAccess.Assembly)
    let ilCtorBody = delegeeCtorMeth.MethodBody

    let ilCloLambdas = Lambdas_return ilCtxtDelTy
    let ilAttribs = GenAttrs cenv eenvinner cloAttribs
    let cloTypeDefs = GenClosureTypeDefs cenv (ilDelegeeTypeRef,ilDelegeeGenericParams,ilAttribs,ilCloFreeVars,ilCloLambdas,ilCtorBody,[delegeeInvokeMeth],[],cenv.g.ilg.typ_Object,[])
    for cloTypeDef in cloTypeDefs do 
        cgbuf.mgbuf.AddTypeDef(ilDelegeeTypeRef, cloTypeDef, false, false, None)
    CountClosure()

    let ctxtGenericArgsForDelegee = GenGenericArgs m eenvouter.tyenv cloFreeTyvars
    let ilxCloSpec = IlxClosureSpec.Create(IlxClosureRef(ilDelegeeTypeRef, ilCloLambdas, ilCloFreeVars), ctxtGenericArgsForDelegee)
    GenGetLocalVals cenv cgbuf eenvouter m cloFreeVars
    CG.EmitInstr cgbuf (pop ilCloFreeVars.Length) (Push [EraseClosures.mkTyOfLambdas cenv.g.ilxPubCloEnv ilCloLambdas]) (I_newobj (ilxCloSpec.Constructor,None))

    let ilDelegeeTyOuter = mkILBoxedTy ilDelegeeTypeRef ctxtGenericArgsForDelegee
    let ilDelegeeInvokeMethOuter = mkILNonGenericInstanceMethSpecInTy (ilDelegeeTyOuter,"Invoke",typesOfILParams ilDelegeeParams, ilDelegeeRet.Type)
    let ilDelegeeCtorMethOuter = mkCtorMethSpecForDelegate cenv.g.ilg (ilCtxtDelTy,useUIntPtrForDelegateCtor)
    CG.EmitInstr cgbuf (pop 0) (Push [cenv.g.ilg.typ_IntPtr]) (I_ldftn ilDelegeeInvokeMethOuter)
    CG.EmitInstr cgbuf (pop 2) (Push [ilCtxtDelTy]) (I_newobj(ilDelegeeCtorMethOuter,None))
    GenSequel cenv eenvouter.cloc cgbuf sequel

//-------------------------------------------------------------------------
// Generate statically-resolved conditionals used for type-directed optimizations.
//------------------------------------------------------------------------- 
    
and GenStaticOptimization cenv cgbuf eenv (constraints,e2,e3,_m) sequel = 
    let e = 
      if DecideStaticOptimizations cenv.g constraints = StaticOptimizationAnswer.Yes then e2 
      else e3
    GenExpr cenv cgbuf eenv SPSuppress e sequel


//-------------------------------------------------------------------------
// Generate discrimination trees
//------------------------------------------------------------------------- 

and IsSequelImmediate  sequel = 
    match sequel with 
    (* All of these can be done at the end of each branch - we don't need a real join point *)
    | Return | ReturnVoid | Br _ | LeaveHandler _  -> true
    | DiscardThen sequel -> IsSequelImmediate  sequel
    | _ -> false

/// Generate a point where several branches of control flow can merge back together, e.g. after a conditional
/// or 'match'.
and GenJoinPoint cenv cgbuf pos eenv ty m sequel = 

    // What the join point does depends on the contents of the sequel. For example, if the sequal is "return" then
    // each branch can just return and no true join point is needed.
    match sequel with 
    // All of these can be done at the end of each branch - we don't need a real join point 
    | _ when IsSequelImmediate sequel -> 
        let stackAfterJoin = cgbuf.GetCurrentStack()
        let afterJoin = CG.GenerateDelayMark cgbuf (pos + "_join") 
        sequel,afterJoin,stackAfterJoin,Continue

    // We end scopes at the join point, if any 
    | EndLocalScope(sq,mark) -> 
        let sequelNow,afterJoin,stackAfterJoin,sequelAfterJoin = GenJoinPoint cenv cgbuf pos eenv ty m sq 
        sequelNow,afterJoin,stackAfterJoin,EndLocalScope(sequelAfterJoin,mark)

    // If something non-trivial happens after a discard then generate a join point, but first discard the value (often this means we won't generate it at all) 
    | DiscardThen sequel -> 
        let stackAfterJoin =  cgbuf.GetCurrentStack()
        let afterJoin = CG.GenerateDelayMark cgbuf (pos + "_join") 
        DiscardThen (Br afterJoin),afterJoin,stackAfterJoin,sequel
 
    // The others (e.g. Continue, LeaveFilter and CmpThenBrOrContinue) can't be done at the end of each branch. We must create a join point. 
    | _ -> 
        let pushed = GenType cenv.amap m eenv.tyenv ty
        let stackAfterJoin = (pushed :: (cgbuf.GetCurrentStack()))
        let afterJoin = CG.GenerateDelayMark cgbuf (pos + "_join") 
        // go to the join point 
        Br afterJoin, afterJoin,stackAfterJoin,sequel
        
and GenMatch cenv cgbuf eenv (spBind,_exprm,tree,targets,m,ty) sequel =

    match spBind with 
    | SequencePointAtBinding m -> CG.EmitSeqPoint cgbuf m
    | NoSequencePointAtDoBinding
    | NoSequencePointAtLetBinding
    | NoSequencePointAtInvisibleBinding 
    | NoSequencePointAtStickyBinding -> ()

    // The target of branch needs a sequence point.
    // If we don't give it one it will get entirely the wrong sequence point depending on earlier codegen
    // Note we're not interested in having pattern matching and decision trees reveal their inner working.
    // Hence at each branch target we 'reassert' the overall sequence point that was active as we came into the match.
    //
    // NOTE: sadly this causes multiple sequence points to appear for the "initial" location of an if/then/else or match.
    let activeSP = cgbuf.GetLastSequencePoint()
    let repeatSP() = 
        match activeSP with 
        | None -> () 
        | Some src -> 
            if activeSP <> cgbuf.GetLastSequencePoint() then 
                CG.EmitSeqPoint cgbuf src

    // First try the common cases where we don't need a join point. 
    match tree with 
    | TDSuccess _ -> 
        failwith "internal error: matches that immediately succeed should have been normalized using mkAndSimplifyMatch"

    | _ -> 
        // Create a join point 
        let stackAtTargets = cgbuf.GetCurrentStack() // the stack at the target of each clause 
        let (sequelOnBranches,afterJoin,stackAfterJoin,sequelAfterJoin) = GenJoinPoint cenv cgbuf "match" eenv ty m sequel

        // Stack: "stackAtTargets" is "stack prior to any match-testing" and also "stack at the start of each branch-RHS".
        //        match-testing (dtrees) should not contribute to the stack.
        //        Each branch-RHS (targets) may contribute to the stack, leaving it in the "stackAfterJoin" state, for the join point.
        //        Since code is branching and joining, the cgbuf stack is maintained manually.
        GenDecisionTreeAndTargets cenv cgbuf stackAtTargets eenv tree targets repeatSP sequelOnBranches 
        CG.SetMarkToHere cgbuf afterJoin

        //assert(cgbuf.GetCurrentStack() = stackAfterJoin)  // REVIEW: Since gen_dtree* now sets stack, stack should be stackAfterJoin at this point...
        CG.SetStack cgbuf stackAfterJoin
        // If any values are left on the stack after the join then we're certainly going to do something with them
        // For example, we may be about to execute a 'stloc' for
        //
        //   let y2 = if System.DateTime.Now.Year < 2000 then 1 else 2
        //
        // or a 'stelem' for
        //
        //   arr.[0] <- if System.DateTime.Now.Year > 2000 then 1 else 2
        //
        // In both cases, any instructions that come after this point will be falsely associated with the last branch of the control
        // prior to the join point. This is base, e.g. see FSharp 1.0 bug 5155
        if not (isNil stackAfterJoin) then 
            cgbuf.EmitStartOfHiddenCode()

        GenSequel cenv eenv.cloc cgbuf sequelAfterJoin

// Accumulate the decision graph as we go
and GenDecisionTreeAndTargets cenv cgbuf stackAtTargets eenv tree targets repeatSP sequel = 
    let targetInfos = GenDecisionTreeAndTargetsInner cenv cgbuf None stackAtTargets eenv tree targets repeatSP (IntMap.empty()) sequel
    GenPostponedDecisionTreeTargets cenv cgbuf stackAtTargets targetInfos sequel
    
and TryFindTargetInfo targetInfos n =  
    match IntMap.tryFind n targetInfos  with 
    | Some (targetInfo,_) -> Some targetInfo
    | None -> None

/// When inplabOpt is None, we are assuming a branch or fallthrough to the current code location
///
/// When inplabOpt is "Some inplab", we are assuming an existing branch to "inplab" and can optionally
/// set inplab to point to another location if no codegen is required. 
and GenDecisionTreeAndTargetsInner cenv cgbuf inplabOpt stackAtTargets eenv tree targets repeatSP targetInfos sequel = 
    CG.SetStack cgbuf stackAtTargets              // Set the expected initial stack.
    match tree with 
    | TDBind(bind,rest) -> 
       match inplabOpt with Some inplab -> CG.SetMarkToHere cgbuf inplab | None -> ()
       let startScope,endScope as scopeMarks = StartDelayedLocalScope "dtreeBind" cgbuf
       let eenv = AllocStorageForBind cenv cgbuf scopeMarks eenv bind
       let sp = GenSequencePointForBind cenv cgbuf bind
       CG.SetMarkToHere cgbuf startScope
       GenBindAfterSequencePoint cenv cgbuf eenv sp bind
       // We don't get the scope marks quite right for dtree-bound variables.  This is because 
       // we effectively lose an EndLocalScope for all dtrees that go to the same target 
       // So we just pretend that the variable goes out of scope here. 
       CG.SetMarkToHere cgbuf endScope
       GenDecisionTreeAndTargetsInner cenv cgbuf None stackAtTargets eenv rest targets repeatSP targetInfos sequel

    | TDSuccess (es,targetIdx) ->  
       GenDecisionTreeSuccess cenv cgbuf inplabOpt stackAtTargets eenv es targetIdx targets repeatSP targetInfos sequel 

    | TDSwitch(e, cases, dflt,m)  -> 
       GenDecisionTreeSwitch cenv cgbuf inplabOpt stackAtTargets eenv e cases dflt m targets repeatSP targetInfos sequel 

and GetTarget (targets:_[]) n =
    if n >= targets.Length then failwith "GetTarget: target not found in decision tree"
    targets.[n]

and GenDecisionTreeSuccess cenv cgbuf inplabOpt stackAtTargets eenv es targetIdx targets repeatSP targetInfos sequel = 
    let (TTarget(vs,successExpr,spTarget)) = GetTarget targets targetIdx
    match TryFindTargetInfo targetInfos targetIdx with
    | Some (_,targetMarkAfterBinds:Mark,eenvAtTarget,_,_,_,_,_,_,_) ->

        // If not binding anything we can go directly to the targetMarkAfterBinds point 
        // This is useful to avoid lots of branches e.g. in match A | B | C -> e 
        // In this case each case will just go straight to "e" 
        if isNil vs then 
            match inplabOpt with 
            | None -> CG.EmitInstr cgbuf (pop 0) Push0 (I_br targetMarkAfterBinds.CodeLabel) 
            | Some inplab -> CG.SetMark cgbuf inplab targetMarkAfterBinds
        else 
            match inplabOpt with None -> () | Some inplab -> CG.SetMarkToHere cgbuf inplab
            repeatSP()
            // It would be better not to emit any expressions here, and instead push these assignments into the postponed target
            // However not all targets are currently postponed (we only postpone in debug code), pending further testing of the performance
            // impact of postponing.
            (vs,es) ||> List.iter2 (GenBindRhs cenv cgbuf eenv SPSuppress) 
            vs |> List.rev |> List.iter (fun v -> GenStoreVal cgbuf eenvAtTarget v.Range v) 
            CG.EmitInstr cgbuf (pop 0) Push0 (I_br targetMarkAfterBinds.CodeLabel) 

        targetInfos

    | None -> 

        match inplabOpt with None -> () | Some inplab -> CG.SetMarkToHere cgbuf inplab
        let targetMarkBeforeBinds = CG.GenerateDelayMark cgbuf "targetBeforeBinds"
        let targetMarkAfterBinds = CG.GenerateDelayMark cgbuf "targetAfterBinds"
        let startScope,endScope as scopeMarks = StartDelayedLocalScope "targetBinds" cgbuf
        let binds = mkInvisibleBinds vs es
        let eenvAtTarget = AllocStorageForBinds cenv cgbuf scopeMarks eenv binds
        let targetInfo = (targetMarkBeforeBinds,targetMarkAfterBinds,eenvAtTarget,successExpr,spTarget,repeatSP,vs,binds,startScope,endScope)
        
        // In debug mode push all decision tree targets to after the switching
        let isTargetPostponed = 
            if cenv.opts.localOptimizationsAreOn then 
                GenDecisionTreeTarget cenv cgbuf stackAtTargets targetIdx targetInfo sequel
                false
            else
                CG.EmitInstr cgbuf (pop 0) Push0 (I_br targetMarkBeforeBinds.CodeLabel)
                true 

        let targetInfos = IntMap.add targetIdx (targetInfo,isTargetPostponed) targetInfos
        targetInfos

and GenPostponedDecisionTreeTargets cenv cgbuf stackAtTargets targetInfos sequel = 
    let targetInfos = targetInfos |> Seq.sortBy (fun (KeyValue(targetIdx,_)) -> targetIdx)
    for (KeyValue(targetIdx,(targetInfo,isTargetPostponed))) in targetInfos do        
        if isTargetPostponed then 
            GenDecisionTreeTarget cenv cgbuf stackAtTargets targetIdx targetInfo sequel

and GenDecisionTreeTarget cenv cgbuf stackAtTargets _targetIdx (targetMarkBeforeBinds,targetMarkAfterBinds,eenvAtTarget,successExpr,spTarget,repeatSP,vs,binds,startScope,endScope) sequel = 
    CG.SetMarkToHere cgbuf targetMarkBeforeBinds
    let spExpr = (match spTarget with SequencePointAtTarget -> SPAlways | SuppressSequencePointAtTarget _ -> SPSuppress)

    // Repeat the sequence point to make sure each target branch has some sequence point (instead of inheriting
    // a random sequence point from the previously generated IL code from the previous block. See comment on 
    // repeatSP() above.
    //
    // Only repeat the sequence point if we really have to, i.e. if the target expression doesn't start with a
    // sequence point anyway
    if isNil vs && DoesGenExprStartWithSequencePoint cenv.g spExpr successExpr then 
       () 
    else 
       match spTarget with 
       | SequencePointAtTarget -> repeatSP()
       | SuppressSequencePointAtTarget -> cgbuf.EmitStartOfHiddenCode()

    CG.SetMarkToHere cgbuf startScope
    GenBindings cenv cgbuf eenvAtTarget binds
    CG.SetMarkToHere cgbuf targetMarkAfterBinds
    CG.SetStack cgbuf stackAtTargets
    GenExpr cenv cgbuf eenvAtTarget spExpr successExpr (EndLocalScope(sequel,endScope))


and GenDecisionTreeSwitch cenv cgbuf inplabOpt stackAtTargets eenv e cases defaultTargetOpt switchm targets repeatSP targetInfos sequel = 
    let m = e.Range
    match inplabOpt with None -> () | Some inplab -> CG.SetMarkToHere cgbuf inplab

    repeatSP()
    match cases with 
      // optimize a test against a boolean value, i.e. the all-important if-then-else 
      | TCase(DecisionTreeTest.Const(Const.Bool b), successTree) :: _  ->  
       let failureTree = (match defaultTargetOpt with None -> cases.Tail.Head.CaseTree | Some d -> d)
       GenDecisionTreeTest cenv eenv.cloc cgbuf stackAtTargets e None eenv (if b then successTree else  failureTree) (if b then failureTree else successTree) targets repeatSP targetInfos sequel 

      // // Remove a single test for a union case . Union case tests are always exa
      //| [ TCase(DecisionTreeTest.UnionCase _, successTree) ] when (defaultTargetOpt.IsNone)  ->  
      //  GenDecisionTreeAndTargetsInner cenv cgbuf inplabOpt stackAtTargets eenv successTree targets repeatSP targetInfos sequel
      //   //GenDecisionTree cenv eenv.cloc cgbuf stackAtTargets e (Some (pop 1, Push [cenv.g.ilg.typ_Bool], Choice1Of2 (avoidHelpers, cuspec, idx))) eenv successTree failureTree targets repeatSP targetInfos sequel

      // Optimize a single test for a union case to an "isdata" test - much 
      // more efficient code, and this case occurs in the generated equality testers where perf is important 
      | TCase(DecisionTreeTest.UnionCase(c,tyargs), successTree) :: rest when rest.Length = (match defaultTargetOpt with None -> 1 | Some _ -> 0)  ->  
        let failureTree = 
            match defaultTargetOpt with 
            | None -> rest.Head.CaseTree
            | Some tg -> tg
        let cuspec = GenUnionSpec cenv.amap m eenv.tyenv c.TyconRef tyargs
        let idx = c.Index
        let avoidHelpers = entityRefInThisAssembly cenv.g.compilingFslib c.TyconRef
        GenDecisionTreeTest cenv eenv.cloc cgbuf stackAtTargets e (Some (pop 1, Push [cenv.g.ilg.typ_Bool], Choice1Of2 (avoidHelpers, cuspec, idx))) eenv successTree failureTree targets repeatSP targetInfos sequel

      | _ ->  
        let caseLabels = List.map (fun _ -> CG.GenerateDelayMark cgbuf "switch_case") cases
        let firstDiscrim =  cases.Head.Discriminator
        match firstDiscrim with 
        // Iterated tests, e.g. exception constructors, nulltests, typetests and active patterns.
        // These should always have one positive and one negative branch 
        | DecisionTreeTest.IsInst _  
        | DecisionTreeTest.ArrayLength _
        | DecisionTreeTest.IsNull 
        | DecisionTreeTest.Const(Const.Zero) -> 
            if not (isSingleton cases) || Option.isNone defaultTargetOpt then failwith "internal error: GenDecisionTreeSwitch: DecisionTreeTest.IsInst/isnull/query"
            let bi = 
              match firstDiscrim with 
              | DecisionTreeTest.Const(Const.Zero) ->
                  GenExpr cenv cgbuf eenv SPSuppress e Continue 
                  BI_brfalse
              | DecisionTreeTest.IsNull -> 
                  GenExpr cenv cgbuf eenv SPSuppress e Continue 
                  let srcTy = tyOfExpr cenv.g e
                  if isTyparTy cenv.g srcTy then 
                      let ilFromTy = GenType cenv.amap m eenv.tyenv srcTy
                      CG.EmitInstr cgbuf (pop 1) (Push [cenv.g.ilg.typ_Object]) (I_box ilFromTy)
                  BI_brfalse
              | DecisionTreeTest.IsInst (_srcty,tgty) -> 
                  let e = mkCallTypeTest cenv.g m tgty e
                  GenExpr cenv cgbuf eenv SPSuppress e Continue
                  BI_brtrue
              | _ -> failwith "internal error: GenDecisionTreeSwitch"
            CG.EmitInstr cgbuf (pop 1) Push0 (I_brcmp (bi,(List.head caseLabels).CodeLabel))
            GenDecisionTreeCases cenv cgbuf stackAtTargets eenv targets repeatSP targetInfos defaultTargetOpt caseLabels cases sequel
              
        | DecisionTreeTest.ActivePatternCase _ -> error(InternalError("internal error in codegen: DecisionTreeTest.ActivePatternCase",switchm))
        | DecisionTreeTest.UnionCase (hdc,tyargs) -> 
            GenExpr cenv cgbuf eenv SPSuppress e Continue
            let cuspec = GenUnionSpec cenv.amap m eenv.tyenv hdc.TyconRef tyargs
            let dests = 
              if cases.Length <> caseLabels.Length then failwith "internal error: DecisionTreeTest.UnionCase"
              (cases , caseLabels) ||> List.map2 (fun case label  ->
                  match case with 
                  | TCase(DecisionTreeTest.UnionCase (c,_),_) -> (c.Index, label.CodeLabel) 
                  | _ -> failwith "error: mixed constructor/const test?") 
            
            let avoidHelpers = entityRefInThisAssembly cenv.g.compilingFslib hdc.TyconRef
            EraseUnions.emitDataSwitch cenv.g.ilg (UnionCodeGen cgbuf) (avoidHelpers,cuspec,dests)
            CG.EmitInstrs cgbuf (pop 1) Push0 [ ] // push/pop to match the line above
            GenDecisionTreeCases cenv cgbuf stackAtTargets eenv  targets repeatSP targetInfos defaultTargetOpt caseLabels cases sequel
              
        | DecisionTreeTest.Const c ->
            GenExpr cenv cgbuf eenv SPSuppress e Continue
            match c with 
            | Const.Bool _ -> failwith "should have been done earlier"
            | Const.SByte _            
            | Const.Int16 _           
            | Const.Int32 _           
            | Const.Byte _          
            | Const.UInt16 _          
            | Const.UInt32 _
            | Const.Char _ ->
                if List.length cases <> List.length caseLabels then failwith "internal error: "
                let dests = 
                  (cases,caseLabels) ||> List.map2 (fun case label  ->
                      let i = 
                        match case.Discriminator with 
                          DecisionTreeTest.Const c' ->
                            match c' with 
                            | Const.SByte i -> int32 i
                            | Const.Int16 i -> int32 i
                            | Const.Int32 i -> i
                            | Const.Byte i -> int32 i
                            | Const.UInt16 i -> int32 i
                            | Const.UInt32 i -> int32 i
                            | Const.Char c -> int32 c  
                            | _ -> failwith "internal error: badly formed const test"  

                        | _ -> failwith "internal error: badly formed const test" 
                      (i,label.CodeLabel))
                let mn = List.foldBack (fst >> Operators.min) dests (fst(List.head dests))
                let mx = List.foldBack (fst >> Operators.max) dests (fst(List.head dests))
                // Check if it's worth using a switch 
                // REVIEW: this is using switches even for single integer matches! 
                if mx - mn = (List.length dests - 1) then
                    let destinationLabels = dests |> List.sortBy fst |> List.map snd 
                    if mn <> 0 then 
                      CG.EmitInstrs cgbuf (pop 0) (Push [cenv.g.ilg.typ_Int32]) [ mkLdcInt32 mn]
                      CG.EmitInstrs cgbuf (pop 1) Push0 [ AI_sub ]
                    CG.EmitInstr cgbuf (pop 1) Push0 (I_switch destinationLabels)
                else
                  error(InternalError("non-dense integer matches not implemented in codegen - these should have been removed by the pattern match compiler",switchm))
                GenDecisionTreeCases cenv cgbuf stackAtTargets eenv  targets repeatSP targetInfos defaultTargetOpt caseLabels cases sequel
            | _ -> error(InternalError("these matches should never be needed",switchm))

and GenDecisionTreeCases cenv cgbuf stackAtTargets eenv targets repeatSP targetInfos defaultTargetOpt caseLabels cases sequel =
    assert(cgbuf.GetCurrentStack() = stackAtTargets) // cgbuf stack should be unchanged over tests. [bug://1750].

    let targetInfos = 
        match defaultTargetOpt with 
        | Some defaultTarget -> GenDecisionTreeAndTargetsInner cenv cgbuf None stackAtTargets eenv defaultTarget targets repeatSP targetInfos sequel
        | None -> targetInfos

    let targetInfos = 
        (targetInfos, caseLabels, cases) |||> List.fold2 (fun targetInfos caseLabel (TCase(_,caseTree)) -> 
            GenDecisionTreeAndTargetsInner cenv cgbuf (Some caseLabel) stackAtTargets eenv caseTree targets repeatSP targetInfos sequel)
    targetInfos 

// Used for the peephole optimization below
and (|BoolExpr|_|) = function Expr.Const(Const.Bool b1,_,_) -> Some(b1) | _ -> None

and GenDecisionTreeTest cenv cloc cgbuf stackAtTargets e tester eenv successTree failureTree targets repeatSP targetInfos sequel =

    match successTree,failureTree with 

    // Peephole: if generating a boolean value or its negation then just leave it on the stack 
    // This comes up in the generated equality functions.  REVIEW: do this as a peephole optimization elsewhere 
    | TDSuccess(es1,n1), 
      TDSuccess(es2,n2) when 
         isNil es1 && isNil es2 &&
         (match GetTarget targets n1, GetTarget targets n2 with 
          | TTarget(_,BoolExpr(b1),_),TTarget(_,BoolExpr(b2),_) -> b1 = not b2
          | _ -> false) ->

             match GetTarget targets n1, GetTarget targets n2 with 

             | TTarget(_,BoolExpr(b1),_),_ -> 
                 GenExpr cenv cgbuf eenv SPSuppress e Continue
                 match tester with 
                 | Some (pops,pushes,i) -> 
                    match i with 
                    | Choice1Of2 (avoidHelpers,cuspec,idx) -> CG.EmitInstrs cgbuf pops pushes (EraseUnions.mkIsData cenv.g.ilg (avoidHelpers, cuspec, idx))
                    | Choice2Of2 i -> CG.EmitInstr cgbuf pops pushes i
                 | _ -> ()
                 if not b1 then 
                   CG.EmitInstrs cgbuf (pop 0) (Push [cenv.g.ilg.typ_Bool]) [mkLdcInt32 (0) ]
                   CG.EmitInstrs cgbuf (pop 1) Push0 [AI_ceq]
                 GenSequel cenv cloc cgbuf sequel
                 targetInfos

             | _ -> failwith "internal error: GenDecisionTreeTest during bool elim"

    | _ ->
        let failure = CG.GenerateDelayMark cgbuf "testFailure"
        match tester with 
        | None -> 
            // generate the expression, then test it for "false" 
            GenExpr cenv cgbuf eenv SPSuppress e (CmpThenBrOrContinue(pop 1, [ I_brcmp (BI_brfalse, failure.CodeLabel) ]))

        // Turn 'isdata' tests that branch into EI_brisdata tests 
        | Some (_,_,Choice1Of2 (avoidHelpers,cuspec,idx)) ->
            GenExpr cenv cgbuf eenv SPSuppress e (CmpThenBrOrContinue(pop 1, EraseUnions.mkBrIsData cenv.g.ilg false (avoidHelpers,cuspec, idx, failure.CodeLabel)))

        | Some (pops,pushes,i) ->
            GenExpr cenv cgbuf eenv SPSuppress e Continue
            match i with 
            | Choice1Of2 (avoidHelpers,cuspec,idx) -> CG.EmitInstrs cgbuf pops pushes (EraseUnions.mkIsData cenv.g.ilg (avoidHelpers, cuspec, idx))
            | Choice2Of2 i -> CG.EmitInstr cgbuf pops pushes i
            CG.EmitInstr cgbuf (pop 1) Push0  (I_brcmp (BI_brfalse, failure.CodeLabel))

        let targetInfos = GenDecisionTreeAndTargetsInner cenv cgbuf None stackAtTargets eenv successTree targets repeatSP targetInfos sequel

        GenDecisionTreeAndTargetsInner cenv cgbuf (Some failure) stackAtTargets eenv failureTree targets repeatSP targetInfos sequel 

//-------------------------------------------------------------------------
// Generate letrec bindings
//------------------------------------------------------------------------- 

and GenLetRecFixup cenv cgbuf eenv (ilxCloSpec:IlxClosureSpec,e,ilField:ILFieldSpec,e2,_m) =
    GenExpr cenv cgbuf eenv SPSuppress  e Continue
    CG.EmitInstrs cgbuf (pop 0) Push0 [ I_castclass ilxCloSpec.ILType ]
    GenExpr cenv cgbuf eenv SPSuppress  e2 Continue
    CG.EmitInstrs cgbuf (pop 2) Push0 [ mkNormalStfld (mkILFieldSpec(ilField.FieldRef,ilxCloSpec.ILType)) ]

and GenLetRecBinds cenv cgbuf eenv (allBinds: Bindings,m) =
    (* Fix up recursion for non-toplevel recursive bindings *)
    let bindsPossiblyRequiringFixup = 
        allBinds |> List.filter (fun b -> 
            match (StorageForVal m b.Var eenv) with  
            | StaticProperty _
            | Method _ 
            // Note: Recursive data stored in static fields may require fixups e.g. let x = C(x) 
            // | StaticField _ 
            | Null -> false 
            | _ -> true)

    let computeFixupsForOneRecursiveVar boundv forwardReferenceSet fixups selfv access set e =
        match e with 
        | Expr.Lambda _ | Expr.TyLambda _ | Expr.Obj _ -> 
            let isLocalTypeFunc = Option.isSome selfv && (IsNamedLocalTypeFuncVal cenv.g (Option.get selfv) e)
            let selfv = (match e with Expr.Obj _ -> None | _ when isLocalTypeFunc -> None | _ -> Option.map mkLocalValRef selfv)
            let clo,_,eenvclo =  GetIlxClosureInfo cenv m isLocalTypeFunc selfv {eenv with  letBoundVars=(mkLocalValRef boundv)::eenv.letBoundVars}  e 
            clo.cloFreeVars |> List.iter (fun fv -> 
                if Zset.contains fv forwardReferenceSet then 
                    match StorageForVal m fv eenvclo with
                    | Env (_,_,ilField,_) -> fixups := (boundv, fv, (fun () -> GenLetRecFixup cenv cgbuf eenv (clo.cloSpec,access,ilField,exprForVal m fv,m))) :: !fixups
                    | _ -> error (InternalError("GenLetRec: " + fv.LogicalName + " was not in the environment",m)) )
              
        | Expr.Val  (vref,_,_) -> 
            let fv = vref.Deref
            let needsFixup = Zset.contains fv forwardReferenceSet
            if needsFixup then fixups := (boundv, fv,(fun () -> GenExpr cenv cgbuf eenv SPSuppress  (set e) discard)) :: !fixups
        | _ -> failwith "compute real fixup vars"


    let fixups = ref []
    let recursiveVars = Zset.addList (bindsPossiblyRequiringFixup |> List.map (fun v -> v.Var)) (Zset.empty valOrder)
    let _ = 
        (recursiveVars, bindsPossiblyRequiringFixup) ||> List.fold (fun forwardReferenceSet (bind:Binding) ->
            // Compute fixups 
            bind.Expr |> IterateRecursiveFixups cenv.g (Some bind.Var)  
                               (computeFixupsForOneRecursiveVar bind.Var forwardReferenceSet fixups) 
                               (exprForVal m bind.Var, 
                                  (fun _ -> failwith ("internal error: should never need to set non-delayed recursive val: " + bind.Var.LogicalName)))
            // Record the variable as defined
            let forwardReferenceSet = Zset.remove bind.Var forwardReferenceSet
            forwardReferenceSet)

    // Generate the actual bindings
    let _ = 
        (recursiveVars, allBinds) ||> List.fold (fun forwardReferenceSet (bind:Binding) ->
            GenBind cenv cgbuf eenv bind
            // Record the variable as defined
            let forwardReferenceSet = Zset.remove bind.Var forwardReferenceSet
            // Execute and discard any fixups that can now be committed 
            fixups := !fixups |> List.filter (fun (boundv, fv, action) -> if (Zset.contains boundv forwardReferenceSet || Zset.contains fv forwardReferenceSet) then  true else (action(); false))
            forwardReferenceSet)
    ()


and GenLetRec cenv cgbuf eenv (binds,body,m) sequel =
    let _,endScope as scopeMarks = StartLocalScope "letrec" cgbuf
    let eenv = AllocStorageForBinds cenv cgbuf scopeMarks eenv binds
    GenLetRecBinds cenv cgbuf eenv (binds,m)
    
    let sp = if List.exists (BindingEmitsSequencePoint cenv.g) binds then SPAlways else SPSuppress 
    GenExpr cenv cgbuf eenv sp body (EndLocalScope(sequel,endScope))

//-------------------------------------------------------------------------
// Generate simple bindings
//------------------------------------------------------------------------- 

and GenSequencePointForBind cenv cgbuf bind =
    let _, pt, sp = ComputeSequencePointInfoForBinding cenv.g bind
    pt |> Option.iter (CG.EmitSeqPoint cgbuf)
    sp

and GenBind cenv cgbuf eenv bind =
    let sp = GenSequencePointForBind cenv cgbuf bind
    GenBindAfterSequencePoint cenv cgbuf eenv sp bind
    
and ComputeMemberAccessRestrictedBySig eenv vspec =
    let isHidden =  
        IsHiddenVal eenv.sigToImplRemapInfo vspec ||  // anything hidden by a signature gets assembly visibility 
        not vspec.IsMemberOrModuleBinding ||          // anything that's not a module or member binding gets assembly visibility
        vspec.IsIncrClassGeneratedMember              // compiler generated members for class function 'let' bindings get assembly visibility
    ComputeMemberAccess isHidden


and GenBindAfterSequencePoint cenv cgbuf eenv sp (TBind(vspec,rhsExpr,_)) =

    // Record the closed reflection definition if publishing 
    // There is no real reason we're doing this so late in the day
    match vspec.PublicPath, vspec.ReflectedDefinition with 
    | Some _, Some e -> cgbuf.mgbuf.AddReflectedDefinition(vspec,e)
    | _  -> ()

    let eenv = {eenv with letBoundVars= (mkLocalValRef vspec) :: eenv.letBoundVars}

    let access = ComputeMemberAccessRestrictedBySig eenv vspec

    // Workaround for .NET and Visual Studio restriction w.r.t debugger type proxys
    // Mark internal constructors in internal classes as public. 
    let access = 
        if access = ILMemberAccess.Assembly && vspec.IsConstructor && IsHiddenTycon eenv.sigToImplRemapInfo vspec.MemberApparentParent.Deref then 
            ILMemberAccess.Public
        else
            access
    
    let m = vspec.Range

    match StorageForVal m vspec eenv with 

    | Null -> 
        GenExpr cenv cgbuf eenv SPSuppress rhsExpr discard

    // The initialization code for static 'let' and 'do' bindings gets compiled into the initialization .cctor for the whole file
    | _ when vspec.IsClassConstructor && isNil vspec.TopValActualParent.TyparsNoRange ->
        let tps,_,_,_,cctorBody,_ = IteratedAdjustArityOfLambda cenv.g cenv.amap vspec.ValReprInfo.Value rhsExpr
        let eenv = EnvForTypars tps eenv
        GenExpr cenv cgbuf eenv SPSuppress cctorBody discard
        
    | Method (topValInfo,_,mspec,_,paramInfos,retInfo)  ->
        let tps,ctorThisValOpt,baseValOpt,vsl,body',bodyty = IteratedAdjustArityOfLambda cenv.g cenv.amap topValInfo rhsExpr
        let methodVars = List.concat vsl
        GenMethodForBinding cenv cgbuf eenv (vspec,mspec,access,paramInfos,retInfo) (topValInfo,ctorThisValOpt,baseValOpt,tps,methodVars, body', bodyty)

    | StaticProperty (ilGetterMethSpec, optShadowLocal) ->  

        let ilAttribs = GenAttrs cenv eenv vspec.Attribs
        let ilTy = ilGetterMethSpec.FormalReturnType
        let ilPropDef = 
            { Name = PrettyNaming.ChopPropertyName ilGetterMethSpec.Name
              IsRTSpecialName = false
              IsSpecialName = false
              SetMethod = None
              GetMethod = Some ilGetterMethSpec.MethodRef
              CallingConv = ILThisConvention.Static
              Type = ilTy          
              Init = None
              Args = []
              CustomAttrs = mkILCustomAttrs ilAttribs }
        cgbuf.mgbuf.AddOrMergePropertyDef(ilGetterMethSpec.MethodRef.EnclosingTypeRef, ilPropDef,m)

        let ilMethodDef = 
            let ilMethodBody = MethodBody.IL(CodeGenMethodForExpr cenv cgbuf.mgbuf (SPSuppress, [], ilGetterMethSpec.Name, eenv, 0, 0, rhsExpr, Return))
            mkILStaticMethod ([], ilGetterMethSpec.Name, access, [], mkILReturn ilTy, ilMethodBody) 
            |> AddSpecialNameFlag
            |> AddNonUserCompilerGeneratedAttribs cenv.g

        CountMethodDef()
        cgbuf.mgbuf.AddMethodDef(ilGetterMethSpec.MethodRef.EnclosingTypeRef, ilMethodDef)

        match optShadowLocal with
        | NoShadowLocal -> ()
        | ShadowLocal storage ->  
            CG.EmitInstr cgbuf (pop 0) (Push [ilTy])  (I_call (Normalcall, ilGetterMethSpec, None))
            GenSetStorage m cgbuf storage

    | StaticField (fspec, vref, hasLiteralAttr, ilTyForProperty, ilPropName, fty, ilGetterMethRef, ilSetterMethRef, optShadowLocal) ->  
        let mut = vspec.IsMutable
        
        let canTarget(targets, goal : System.AttributeTargets) =
            match targets with
            | None -> true
            | Some tgts -> 0 <> int(tgts &&& goal)

        /// Generate a static field definition...
        let ilFieldDefs = 
            let access = ComputeMemberAccess (not hasLiteralAttr || IsHiddenVal eenv.sigToImplRemapInfo vspec)
            let ilFieldDef = mkILStaticField (fspec.Name, fty, None, None, access)
            let ilFieldDef =
                match vref.LiteralValue with 
                | Some konst -> { ilFieldDef with IsLiteral=true; LiteralValue= Some(GenFieldInit m konst) }
                | None  -> ilFieldDef 
              
            let ilFieldDef = 
                let isClassInitializer = (cgbuf.MethodName = ".cctor")
                if mut || cenv.opts.isInteractiveItExpr || not isClassInitializer || hasLiteralAttr then 
                    ilFieldDef 
                else 
                    {ilFieldDef with IsInitOnly=true }

            let ilAttribs = 
                if not hasLiteralAttr then
                    vspec.Attribs 
                    |> List.filter (fun (Attrib(_,_,_,_,_,targets,_)) -> canTarget(targets, System.AttributeTargets.Field))
                    |> GenAttrs cenv eenv // backing field only gets attributes that target fields
                else
                    GenAttrs cenv eenv vspec.Attribs  // literals have no property, so preserve all the attributes on the field itself

            let ilFieldDef = 
                { ilFieldDef with 
                   CustomAttrs = mkILCustomAttrs (ilAttribs @ [ cenv.g.DebuggerBrowsableNeverAttribute ]) }

            [ (fspec.EnclosingTypeRef, ilFieldDef) ]
          
        let ilTypeRefForProperty = ilTyForProperty.TypeRef

        for (tref,ilFieldDef) in ilFieldDefs do
            cgbuf.mgbuf.AddFieldDef(tref,ilFieldDef)
            CountStaticFieldDef()

        // ... and the get/set properties to access it. 
        if not hasLiteralAttr then 
            let ilAttribs = 
                vspec.Attribs 
                |> List.filter (fun (Attrib(_,_,_,_,_,targets,_)) -> canTarget(targets, System.AttributeTargets.Property))
                |> GenAttrs cenv eenv // property only gets attributes that target properties
            let ilPropDef = 
                { Name=ilPropName
                  IsRTSpecialName=false
                  IsSpecialName=false
                  SetMethod=if mut || cenv.opts.isInteractiveItExpr then Some ilSetterMethRef else None
                  GetMethod=Some ilGetterMethRef
                  CallingConv=ILThisConvention.Static
                  Type=fty          
                  Init=None
                  Args = []
                  CustomAttrs=mkILCustomAttrs (ilAttribs @ [mkCompilationMappingAttr cenv.g (int SourceConstructFlags.Value)]) }
            cgbuf.mgbuf.AddOrMergePropertyDef(ilTypeRefForProperty,ilPropDef,m)

            let getterMethod = 
                mkILStaticMethod([],ilGetterMethRef.Name,access,[],mkILReturn fty,
                               mkMethodBody(true,[],2,nonBranchingInstrsToCode [ mkNormalLdsfld fspec ],None)) 
                |> AddSpecialNameFlag
            cgbuf.mgbuf.AddMethodDef(ilTypeRefForProperty,getterMethod) 
            if mut || cenv.opts.isInteractiveItExpr then 
                let setterMethod = 
                    mkILStaticMethod([],ilSetterMethRef.Name,access,[mkILParamNamed("value",fty)],mkILReturn ILType.Void,
                                   mkMethodBody(true,[],2,nonBranchingInstrsToCode [ mkLdarg0;mkNormalStsfld fspec],None))
                    |> AddSpecialNameFlag
                cgbuf.mgbuf.AddMethodDef(ilTypeRefForProperty,setterMethod)

            GenBindRhs cenv cgbuf eenv sp vspec rhsExpr
            match optShadowLocal with
            | NoShadowLocal -> 
                EmitSetStaticField cgbuf fspec
            | ShadowLocal storage->  
                CG.EmitInstr cgbuf (pop 0) (Push [fty])  AI_dup
                EmitSetStaticField cgbuf fspec
                GenSetStorage m cgbuf storage

    | _ ->
        GenSetBindValue cenv cgbuf eenv eenv vspec rhsExpr

//-------------------------------------------------------------------------
// Generate method bindings
//------------------------------------------------------------------------- 

/// Spectacularly gross table encoding P/Invoke and COM marshalling information 
and GenMarshal cenv attribs = 
    let otherAttribs = 
        // For IlReflect backend, we rely on Reflection.Emit API to emit the pseudo-custom attributes
        // correctly, so we do not filter them out. 
        // For IlWriteBackend, we filter MarshalAs attributes
        match cenv.opts.ilxBackend with
        | IlReflectBackend -> attribs
        | IlWriteBackend ->
            attribs |> List.filter (IsMatchingFSharpAttributeOpt cenv.g cenv.g.attrib_MarshalAsAttribute >> not)

    match TryFindFSharpAttributeOpt cenv.g cenv.g.attrib_MarshalAsAttribute attribs with
    | Some (Attrib(_,_,[ AttribInt32Arg unmanagedType ],namedArgs,_,_,m))  -> 
        let decoder = AttributeDecoder namedArgs
        let rec decodeUnmanagedType unmanagedType = 
           (* enumeration values for System.Runtime.InteropServices.UnmanagedType taken from mscorlib.il *)
            match  unmanagedType with 
            | 0x0 -> ILNativeType.Empty
            | 0x01 -> ILNativeType.Void
            | 0x02 -> ILNativeType.Bool
            | 0x03 -> ILNativeType.Int8
            | 0x04 -> ILNativeType.Byte
            | 0x05 -> ILNativeType.Int16
            | 0x06 -> ILNativeType.UInt16
            | 0x07 -> ILNativeType.Int32
            | 0x08 -> ILNativeType.UInt32
            | 0x09 -> ILNativeType.Int64
            | 0x0A -> ILNativeType.UInt64
            | 0x0B -> ILNativeType.Single
            | 0x0C -> ILNativeType.Double
            | 0x0F -> ILNativeType.Currency
            | 0x13 -> ILNativeType.BSTR
            | 0x14 -> ILNativeType.LPSTR
            | 0x15 -> ILNativeType.LPWSTR
            | 0x16 -> ILNativeType.LPTSTR
            | 0x17 -> ILNativeType.FixedSysString (decoder.FindInt32 "SizeConst" 0x0)
            | 0x19 -> ILNativeType.IUnknown
            | 0x1A -> ILNativeType.IDispatch
            | 0x1B -> ILNativeType.Struct
            | 0x1C -> ILNativeType.Interface
            | 0x1D -> 
                let safeArraySubType = 
                    match decoder.FindInt32 "SafeArraySubType" 0x0 with 
                    (* enumeration values for System.Runtime.InteropServices.VarType taken from mscorlib.il *)
                    | 0x0 -> ILNativeVariant.Empty
                    | 0x1 -> ILNativeVariant.Null                  
                    | 0x02 -> ILNativeVariant.Int16                
                    | 0x03 -> ILNativeVariant.Int32                
                    | 0x0C -> ILNativeVariant.Variant              
                    | 0x04 -> ILNativeVariant.Single              
                    | 0x05 -> ILNativeVariant.Double              
                    | 0x06 -> ILNativeVariant.Currency             
                    | 0x07 -> ILNativeVariant.Date                 
                    | 0x08 -> ILNativeVariant.BSTR                 
                    | 0x09 -> ILNativeVariant.IDispatch            
                    | 0x0a -> ILNativeVariant.Error                
                    | 0x0b -> ILNativeVariant.Bool                 
                    | 0x0d -> ILNativeVariant.IUnknown             
                    | 0x0e -> ILNativeVariant.Decimal              
                    | 0x10 -> ILNativeVariant.Int8                 
                    | 0x11 -> ILNativeVariant.UInt8        
                    | 0x12 -> ILNativeVariant.UInt16       
                    | 0x13 -> ILNativeVariant.UInt32       
                    | 0x15 -> ILNativeVariant.UInt64       
                    | 0x16 -> ILNativeVariant.Int                  
                    | 0x17 -> ILNativeVariant.UInt         
                    | 0x18 -> ILNativeVariant.Void                 
                    | 0x19 -> ILNativeVariant.HRESULT              
                    | 0x1a -> ILNativeVariant.PTR                  
                    | 0x1c -> ILNativeVariant.CArray               
                    | 0x1d -> ILNativeVariant.UserDefined          
                    | 0x1e -> ILNativeVariant.LPSTR                
                    | 0x1B -> ILNativeVariant.SafeArray            
                    | 0x1f -> ILNativeVariant.LPWSTR               
                    | 0x24 -> ILNativeVariant.Record               
                    | 0x40 -> ILNativeVariant.FileTime             
                    | 0x41 -> ILNativeVariant.Blob                 
                    | 0x42 -> ILNativeVariant.Stream               
                    | 0x43 -> ILNativeVariant.Storage              
                    | 0x44 -> ILNativeVariant.StreamedObject      
                    | 0x45 -> ILNativeVariant.StoredObject        
                    | 0x46 -> ILNativeVariant.BlobObject          
                    | 0x47 -> ILNativeVariant.CF                   
                    | 0x48 -> ILNativeVariant.CLSID                
                    | 0x14 -> ILNativeVariant.Int64                
                    | _ -> ILNativeVariant.Empty
                let safeArrayUserDefinedSubType =
                    // the argument is a System.Type obj, but it's written to MD as a UTF8 string
                    match decoder.FindTypeName "SafeArrayUserDefinedSubType" "" with 
                    | "" -> None
                    | res -> if (safeArraySubType = ILNativeVariant.IDispatch) || (safeArraySubType = ILNativeVariant.IUnknown) then Some(res) else None
                ILNativeType.SafeArray(safeArraySubType,safeArrayUserDefinedSubType)
            | 0x1E -> ILNativeType.FixedArray  (decoder.FindInt32 "SizeConst" 0x0)
            | 0x1F -> ILNativeType.Int
            | 0x20 -> ILNativeType.UInt
            | 0x22 -> ILNativeType.ByValStr
            | 0x23 -> ILNativeType.ANSIBSTR
            | 0x24 -> ILNativeType.TBSTR
            | 0x25 -> ILNativeType.VariantBool
            | 0x26 -> ILNativeType.Method
            | 0x28 -> ILNativeType.AsAny
            | 0x2A -> 
               let sizeParamIndex = 
                    match decoder.FindInt16 "SizeParamIndex" -1s with 
                    | -1s -> None
                    | res -> Some ((int)res,None)
               let arraySubType = 
                    match decoder.FindInt32 "ArraySubType" -1 with 
                    | -1 -> None
                    | res -> Some (decodeUnmanagedType res)
               ILNativeType.Array(arraySubType,sizeParamIndex) 
            | 0x2B -> ILNativeType.LPSTRUCT
            | 0x2C -> 
               error(Error(FSComp.SR.ilCustomMarshallersCannotBeUsedInFSharp(),m))
               (* ILNativeType.Custom of bytes * string * string * bytes (* GUID,nativeTypeName,custMarshallerName,cookieString *) *)
               //ILNativeType.Error  
            | 0x2D -> ILNativeType.Error  
            | _ -> ILNativeType.Empty
        Some(decodeUnmanagedType unmanagedType), otherAttribs
    | Some (Attrib(_,_,_,_,_,_,m))  -> 
        errorR(Error(FSComp.SR.ilMarshalAsAttributeCannotBeDecoded(),m))
        None, attribs 
    | _ -> 
        // No MarshalAs detected
        None, attribs 

and GenParamAttribs cenv attribs =
    let inFlag = HasFSharpAttributeOpt cenv.g cenv.g.attrib_InAttribute attribs
    let outFlag = HasFSharpAttribute cenv.g cenv.g.attrib_OutAttribute attribs
    let optionalFlag = HasFSharpAttributeOpt cenv.g cenv.g.attrib_OptionalAttribute attribs
    
    let defaultValue = TryFindFSharpAttributeOpt cenv.g cenv.g.attrib_DefaultParameterValueAttribute attribs 
                       |> Option.bind OptionalArgInfo.FieldInitForDefaultParameterValueAttrib
    // Return the filtered attributes. Do not generate In, Out, Optional or DefaultParameterValue attributes 
    // as custom attributes in the code - they are implicit from the IL bits for these
    let attribs = 
        attribs 
        |> List.filter (IsMatchingFSharpAttributeOpt cenv.g cenv.g.attrib_InAttribute >> not)
        |> List.filter (IsMatchingFSharpAttribute cenv.g cenv.g.attrib_OutAttribute >> not)
        |> List.filter (IsMatchingFSharpAttributeOpt cenv.g cenv.g.attrib_OptionalAttribute >> not)
        |> List.filter (IsMatchingFSharpAttributeOpt cenv.g cenv.g.attrib_DefaultParameterValueAttribute >> not)

    let Marshal,attribs =  GenMarshal cenv attribs
    inFlag,outFlag,optionalFlag,defaultValue,Marshal,attribs

and GenParams cenv eenv (mspec:ILMethodSpec) (attribs:ArgReprInfo list) (implValsOpt: Val list option) =
    let ilArgTys = mspec.FormalArgTypes
    let argInfosAndTypes = 
        if List.length attribs = List.length ilArgTys then List.zip ilArgTys attribs
        else ilArgTys |> List.map (fun ilArgTy -> ilArgTy,ValReprInfo.unnamedTopArg1) 

    let argInfosAndTypes = 
        match implValsOpt with 
        | Some(implVals) when (implVals.Length = ilArgTys.Length) ->
            List.map2 (fun x y -> x,Some y) argInfosAndTypes implVals
        | _ -> 
            List.map (fun x -> x,None) argInfosAndTypes

    (Set.empty,argInfosAndTypes)
    ||> List.mapFold (fun takenNames ((ilArgTy,topArgInfo),implValOpt) -> 
        let inFlag,outFlag,optionalFlag,defaultParamValue,Marshal,attribs = GenParamAttribs cenv topArgInfo.Attribs
        
        let idOpt = (match topArgInfo.Name with 
                     | Some v -> Some v 
                     | None -> match implValOpt with 
                               | Some v -> Some v.Id
                               | None -> None)

        let nmOpt,takenNames = 
            match idOpt with 
            | Some id -> 
                let nm = if takenNames.Contains(id.idText) then globalNng.FreshCompilerGeneratedName (id.idText, id.idRange) else id.idText
                Some nm, takenNames.Add(nm)
            | None -> 
                None, takenNames
            
        let param : ILParameter = 
            { Name=nmOpt
              Type= ilArgTy  
              Default=defaultParamValue
              Marshal=Marshal 
              IsIn=inFlag    
              IsOut=outFlag  
              IsOptional=optionalFlag 
              CustomAttrs= mkILCustomAttrs (GenAttrs cenv eenv attribs) }

        param, takenNames)
    |> fst
    
and GenReturnInfo cenv eenv ilRetTy (retInfo : ArgReprInfo) : ILReturn =
    let marshal,attrs = GenMarshal cenv retInfo.Attribs
    { Type=ilRetTy
      Marshal=marshal
      CustomAttrs= mkILCustomAttrs (GenAttrs cenv eenv attrs) }
       
and GenPropertyForMethodDef compileAsInstance tref mdef (v:Val) (memberInfo:ValMemberInfo) ilArgTys ilPropTy ilAttrs compiledName =
    let name = match compiledName with | Some n -> n | _ -> v.PropertyName in  (* chop "get_" *)
    
    { Name=name 
      IsRTSpecialName=false
      IsSpecialName=false
      SetMethod=(if memberInfo.MemberFlags.MemberKind= MemberKind.PropertySet then Some(mkRefToILMethod(tref,mdef)) else None)
      GetMethod=(if memberInfo.MemberFlags.MemberKind= MemberKind.PropertyGet then Some(mkRefToILMethod(tref,mdef)) else None)
      CallingConv=(if compileAsInstance then ILThisConvention.Instance else ILThisConvention.Static)
      Type=ilPropTy          
      Init=None
      Args = ilArgTys
      CustomAttrs=ilAttrs }  

and GenEventForProperty cenv eenvForMeth (mspec:ILMethodSpec) (v:Val) ilAttrsThatGoOnPrimaryItem m returnTy =
    let evname = v.PropertyName
    let delegateTy = Infos.FindDelegateTypeOfPropertyEvent cenv.g cenv.amap evname m returnTy
    let ilDelegateTy = GenType cenv.amap m eenvForMeth.tyenv delegateTy
    let ilThisTy = mspec.EnclosingType
    let addMethRef    = mkILMethRef (ilThisTy.TypeRef,mspec.CallingConv,"add_"    + evname,0,[ilDelegateTy],ILType.Void)
    let removeMethRef = mkILMethRef (ilThisTy.TypeRef,mspec.CallingConv,"remove_" + evname,0,[ilDelegateTy],ILType.Void)
    { Type = Some(ilDelegateTy) 
      Name= evname 
      IsRTSpecialName=false
      IsSpecialName=false
      AddMethod = addMethRef 
      RemoveMethod = removeMethRef
      FireMethod= None
      OtherMethods= []
      CustomAttrs = mkILCustomAttrs ilAttrsThatGoOnPrimaryItem }


and ComputeFlagFixupsForMemberBinding cenv (v:Val,memberInfo:ValMemberInfo) =
     if isNil memberInfo.ImplementedSlotSigs then 
         [fixupVirtualSlotFlags]
     else 
         memberInfo.ImplementedSlotSigs |> List.map (fun slotsig -> 
             let oty = slotsig.ImplementedType
             let otcref,_ = destAppTy cenv.g oty
             let tcref = v.MemberApparentParent
             
             let useMethodImpl = 
                 // REVIEW: it would be good to get rid of this special casing of Compare and GetHashCode during code generation
                 let isCompare = 
                     (Option.isSome tcref.GeneratedCompareToValues && typeEquiv cenv.g oty cenv.g.mk_IComparable_ty) ||
                     (Option.isSome tcref.GeneratedCompareToValues && tyconRefEq cenv.g cenv.g.system_GenericIComparable_tcref otcref)
                     
                 let isGenericEquals =
                     (Option.isSome tcref.GeneratedHashAndEqualsWithComparerValues &&  tyconRefEq cenv.g cenv.g.system_GenericIEquatable_tcref otcref)
                     
                 let isStructural =
                     (Option.isSome tcref.GeneratedCompareToWithComparerValues && typeEquiv cenv.g oty cenv.g.mk_IStructuralComparable_ty) ||
                     (Option.isSome tcref.GeneratedHashAndEqualsWithComparerValues && typeEquiv cenv.g oty cenv.g.mk_IStructuralEquatable_ty)
                 isInterfaceTy cenv.g oty && not isCompare && not isStructural && not isGenericEquals


             let nameOfOverridingMethod = GenNameOfOverridingMethod cenv (useMethodImpl,slotsig)

             (if useMethodImpl then fixupMethodImplFlags >> renameMethodDef nameOfOverridingMethod
              else fixupVirtualSlotFlags >> renameMethodDef nameOfOverridingMethod))
              
and ComputeMethodImplAttribs cenv (_v:Val) attrs =
    let implflags = 
        match TryFindFSharpAttribute cenv.g cenv.g.attrib_MethodImplAttribute attrs with
        | Some (Attrib(_,_,[ AttribInt32Arg flags ],_,_,_,_))  -> flags
        | _ -> 0x0

    let hasPreserveSigAttr = 
        match TryFindFSharpAttributeOpt cenv.g cenv.g.attrib_PreserveSigAttribute attrs with
        | Some _ -> true
        | _ -> false
    
    // strip the MethodImpl pseudo-custom attribute    
    // The following method implementation flags are used here
    // 0x80 - hasPreserveSigImplFlag
    // 0x20 - synchronize
    // (See ECMA 335, Partition II, section 23.1.11 - Flags for methods [MethodImplAttributes]) 
    let attrs = attrs 
                    |> List.filter (IsMatchingFSharpAttribute cenv.g cenv.g.attrib_MethodImplAttribute >> not) 
                        |> List.filter (IsMatchingFSharpAttributeOpt cenv.g cenv.g.attrib_PreserveSigAttribute >> not)
    let hasPreserveSigImplFlag = ((implflags &&& 0x80) <> 0x0) || hasPreserveSigAttr
    let hasSynchronizedImplFlag = (implflags &&& 0x20) <> 0x0
    let hasNoInliningImplFlag = (implflags &&& 0x08) <> 0x0
    hasPreserveSigImplFlag, hasSynchronizedImplFlag, hasNoInliningImplFlag, attrs
    
and GenMethodForBinding 
        cenv cgbuf eenv 
        (v:Val,mspec,access,paramInfos,retInfo) 
        (topValInfo,ctorThisValOpt,baseValOpt,tps,methodVars, body, returnTy) =
  
    let m = v.Range
    let selfMethodVars,nonSelfMethodVars,compileAsInstance =
        match v.MemberInfo with 
        | Some _ when ValSpecIsCompiledAsInstance cenv.g v -> 
            match methodVars with 
            | [] -> error(InternalError("Internal error: empty argument list for instance method",v.Range))
            | h::t -> [h],t,true
        |  _ -> [],methodVars,false

    let nonUnitNonSelfMethodVars,body = BindUnitVars cenv.g (nonSelfMethodVars,paramInfos,body)
    let nonUnitMethodVars = selfMethodVars@nonUnitNonSelfMethodVars
    let cmtps,curriedArgInfos,_,_ = GetTopValTypeInCompiledForm cenv.g topValInfo v.Type v.Range
    let eenv = bindBaseOrThisVarOpt cenv eenv ctorThisValOpt
    let eenv = bindBaseOrThisVarOpt cenv eenv baseValOpt

    // The type parameters of the method's type are different to the type parameters 
    // for the big lambda ("tlambda") of the implementation of the method. 
    let eenvUnderMethLambdaTypars = EnvForTypars tps eenv
    let eenvUnderMethTypeTypars = EnvForTypars cmtps eenv

    // Add the arguments to the environment.  We add an implicit 'this' argument to constructors 
    let isCtor = v.IsConstructor 
    let eenvForMeth = 
        let eenvForMeth = eenvUnderMethLambdaTypars
        let numImplicitArgs = if isCtor then 1 else 0
        let eenvForMeth = AddStorageForLocalVals cenv.g (List.mapi (fun i v -> (v,Arg (numImplicitArgs+i))) nonUnitMethodVars) eenvForMeth
        eenvForMeth

    let tailCallInfo = [(mkLocalValRef v,BranchCallMethod (topValInfo.AritiesOfArgs,curriedArgInfos,tps,nonUnitMethodVars.Length,v.NumObjArgs))]

    // Discard the result on a 'void' return type. For a constructor just return 'void'  
    let sequel = 
        if isUnitTy cenv.g returnTy then discardAndReturnVoid 
        elif isCtor then ReturnVoid 
        else Return

    // Now generate the code.

    let hasPreserveSigNamedArg,ilMethodBody,_hasDllImport = 
        match TryFindFSharpAttributeOpt cenv.g cenv.g.attrib_DllImportAttribute v.Attribs with
        | Some (Attrib(_,_,[ AttribStringArg(dll) ],namedArgs,_,_,m))  -> 
            if not (isNil tps) then error(Error(FSComp.SR.ilSignatureForExternalFunctionContainsTypeParameters(),m)) 
            let hasPreserveSigNamedArg, mbody = GenPInvokeMethod (v.CompiledName,dll,namedArgs)
            hasPreserveSigNamedArg, mbody, true

        | Some (Attrib(_,_,_,_,_,_,m))  -> 
            error(Error(FSComp.SR.ilDllImportAttributeCouldNotBeDecoded(),m))
        | _ -> 
            // Replace the body of ValInline.PseudoVal "must inline" methods with a 'throw'
            // However still generate the code for reflection etc.
            let bodyExpr =
                if HasFSharpAttribute cenv.g cenv.g.attrib_NoDynamicInvocationAttribute v.Attribs then
                    mkThrow m returnTy
                         (mkExnExpr(cenv.g.FindSysTyconRef ["System"] "NotSupportedException", 
                                       [ mkString cenv.g m (FSComp.SR.ilDynamicInvocationNotSupported(v.CompiledName))],m)) 
                else 
                    body 

            // This is the main code generation for most methods 
            false,
            MethodBody.IL(CodeGenMethodForExpr cenv cgbuf.mgbuf (SPAlways,tailCallInfo, mspec.Name, eenvForMeth, 0, 0, bodyExpr, sequel)),
            false

    // Do not generate DllImport attributes into the code - they are implicit from the P/Invoke
    let attrs = 
        v.Attribs 
            |> List.filter (IsMatchingFSharpAttributeOpt cenv.g cenv.g.attrib_DllImportAttribute >> not)
            |> List.filter (IsMatchingFSharpAttribute cenv.g cenv.g.attrib_CompiledNameAttribute >> not)
            
    let attrsAppliedToGetterOrSetter, attrs = 
        List.partition (fun (Attrib(_,_,_,_,isAppliedToGetterOrSetter,_,_)) -> isAppliedToGetterOrSetter) attrs
            
    let sourceNameAttribs,compiledName = 
        match v.Attribs |> List.tryFind (IsMatchingFSharpAttribute cenv.g cenv.g.attrib_CompiledNameAttribute) with 
        | Some (Attrib(_,_,[ AttribStringArg(b) ],_,_,_,_))  -> [ mkCompilationSourceNameAttr cenv.g v.LogicalName ], Some b
        | _ -> [],None
    
    // check if the hasPreserveSigNamedArg and hasSynchronizedImplFlag implementation flags have been specified
    let hasPreserveSigImplFlag, hasSynchronizedImplFlag, hasNoInliningFlag, attrs = ComputeMethodImplAttribs cenv v attrs
        
    let securityAttributes,attrs = attrs |> List.partition (fun a -> IsSecurityAttribute cenv.g cenv.amap cenv.casApplied a m)
    
    let permissionSets = CreatePermissionSets cenv.g cenv.amap eenv securityAttributes
    
    let secDecls = if securityAttributes.Length > 0 then (mkILSecurityDecls permissionSets) else (emptyILSecurityDecls)
    
    // Do not push the attributes to the method for events and properties    
    let ilAttrsCompilerGenerated = if v.IsCompilerGenerated then [  cenv.g.CompilerGeneratedAttribute ] else []

    let ilAttrsThatGoOnPrimaryItem = 
        [ yield! GenAttrs cenv eenv attrs
          yield! GenCompilationArgumentCountsAttr cenv v ]

    let ilTypars = GenGenericParams cenv eenvUnderMethLambdaTypars tps
    let ilParams = GenParams cenv eenv mspec paramInfos (Some(nonUnitNonSelfMethodVars))
    let ilReturn = GenReturnInfo cenv eenv mspec.FormalReturnType retInfo
    let methName = mspec.Name
    let tref = mspec.MethodRef.EnclosingTypeRef

    let EmitTheMethodDef mdef = 
        // Does the function have an explicit [<EntryPoint>] attribute? 
        let isExplicitEntryPoint = HasFSharpAttribute cenv.g cenv.g.attrib_EntryPointAttribute attrs
        
        let mdef = 
            {mdef with 
                IsPreserveSig = hasPreserveSigImplFlag || hasPreserveSigNamedArg
                IsSynchronized = hasSynchronizedImplFlag
                IsEntryPoint = isExplicitEntryPoint
                IsNoInline = hasNoInliningFlag
                HasSecurity = mdef.HasSecurity || (securityAttributes.Length > 0)
                SecurityDecls = secDecls }

        let mdef = 
            if // operator names
               mdef.Name.StartsWith("op_",System.StringComparison.Ordinal) || 
               // active pattern names
               mdef.Name.StartsWith("|",System.StringComparison.Ordinal) ||
               // event add/remove method
               v.val_flags.IsGeneratedEventVal then
                {mdef with IsSpecialName=true} 
            else 
                mdef
        CountMethodDef()
        cgbuf.mgbuf.AddMethodDef(tref,mdef)
                

    match v.MemberInfo with 
    // don't generate unimplemented abstracts 
    | Some(memberInfo) when memberInfo.MemberFlags.IsDispatchSlot && not memberInfo.IsImplemented -> 
         // skipping unimplemented abstract method
         ()    
    | Some(memberInfo) when not v.IsExtensionMember -> 

       let ilMethTypars = ilTypars |> List.drop mspec.EnclosingType.GenericArgs.Length
       if memberInfo.MemberFlags.MemberKind = MemberKind.Constructor then 
           assert (isNil ilMethTypars)
           let mdef = mkILCtor (access,ilParams,ilMethodBody) 
           let mdef = { mdef with CustomAttrs= mkILCustomAttrs (ilAttrsThatGoOnPrimaryItem @ sourceNameAttribs @ ilAttrsCompilerGenerated) } 
           EmitTheMethodDef mdef

       elif memberInfo.MemberFlags.MemberKind = MemberKind.ClassConstructor then 
           assert (isNil ilMethTypars)
           let mdef = mkILClassCtor ilMethodBody 
           let mdef = { mdef with CustomAttrs= mkILCustomAttrs (ilAttrsThatGoOnPrimaryItem @ sourceNameAttribs @ ilAttrsCompilerGenerated) } 
           EmitTheMethodDef mdef

       // Generate virtual/override methods + method-impl information if needed
       else
           let mdef = 
               if not compileAsInstance then 
                   mkILStaticMethod (ilMethTypars,v.CompiledName,access,ilParams,ilReturn,ilMethodBody) 

               elif (memberInfo.MemberFlags.IsDispatchSlot && memberInfo.IsImplemented) || 
                    memberInfo.MemberFlags.IsOverrideOrExplicitImpl then 

                   let flagFixups = ComputeFlagFixupsForMemberBinding cenv (v,memberInfo)
                   let mdef = mkILGenericVirtualMethod (v.CompiledName,ILMemberAccess.Public,ilMethTypars,ilParams,ilReturn,ilMethodBody)
                   let mdef = List.fold (fun mdef f -> f mdef) mdef flagFixups

                   // fixup can potentially change name of reflected definition that was already recorded - patch it if necessary
                   cgbuf.mgbuf.ReplaceNameOfReflectedDefinition(v, mdef.Name)
                   mdef
               else 
                   mkILGenericNonVirtualMethod (v.CompiledName,access,ilMethTypars,ilParams,ilReturn,ilMethodBody) 

           let isAbstract = 
               memberInfo.MemberFlags.IsDispatchSlot && 
               let tcref =  v.MemberApparentParent
               not tcref.Deref.IsFSharpDelegateTycon

           let mdef = 
               {mdef with 
                    mdKind=match mdef.mdKind with 
                           | MethodKind.Virtual vinfo -> 
                               MethodKind.Virtual {vinfo with IsFinal=memberInfo.MemberFlags.IsFinal
                                                              IsAbstract=isAbstract } 
                           | k -> k }

           match memberInfo.MemberFlags.MemberKind with 
               
           | (MemberKind.PropertySet | MemberKind.PropertyGet)  ->
               if not (isNil ilMethTypars) then 
                   error(InternalError("A property may not be more generic than the enclosing type - constrain the polymorphism in the expression",v.Range))
                   
               // Check if we're compiling the property as a .NET event
               if CompileAsEvent cenv.g v.Attribs  then 

                   // Emit the pseudo-property as an event, but not if its a private method impl
                   if mdef.Access <> ILMemberAccess.Private then 
                       let edef = GenEventForProperty cenv eenvForMeth mspec v ilAttrsThatGoOnPrimaryItem m returnTy 
                       cgbuf.mgbuf.AddEventDef(tref,edef)
                   // The method def is dropped on the floor here
                   
               else
                   // Emit the property, but not if its a private method impl
                   if mdef.Access <> ILMemberAccess.Private then 
                       let vtyp = ReturnTypeOfPropertyVal cenv.g v
                       let ilPropTy = GenType cenv.amap m eenvUnderMethTypeTypars.tyenv vtyp
                       let ilArgTys = v |> ArgInfosOfPropertyVal cenv.g |> List.map fst |> GenTypes cenv.amap m eenvUnderMethTypeTypars.tyenv 
                       let ilPropDef = GenPropertyForMethodDef compileAsInstance tref mdef v memberInfo ilArgTys ilPropTy (mkILCustomAttrs ilAttrsThatGoOnPrimaryItem) compiledName
                       cgbuf.mgbuf.AddOrMergePropertyDef(tref,ilPropDef,m)

                   // Add the special name flag for all properties                   
                   let mdef = mdef |> AddSpecialNameFlag
                   let mdef = { mdef with CustomAttrs= mkILCustomAttrs ((GenAttrs cenv eenv attrsAppliedToGetterOrSetter) @ sourceNameAttribs @ ilAttrsCompilerGenerated) } 
                   EmitTheMethodDef mdef
           | _ -> 
               let mdef = { mdef with CustomAttrs= mkILCustomAttrs (ilAttrsThatGoOnPrimaryItem @ sourceNameAttribs @ ilAttrsCompilerGenerated) } 
               EmitTheMethodDef mdef

    | _ -> 
        let mdef = mkILStaticMethod (ilTypars, methName, access,ilParams,ilReturn,ilMethodBody)

        // For extension properties, also emit attrsAppliedToGetterOrSetter on the getter or setter method
        let ilAttrs = 
            match v.MemberInfo with 
            | Some memberInfo when v.IsExtensionMember -> 
                 match memberInfo.MemberFlags.MemberKind with 
                 | (MemberKind.PropertySet | MemberKind.PropertyGet)  -> ilAttrsThatGoOnPrimaryItem @ GenAttrs cenv eenv attrsAppliedToGetterOrSetter
                 | _ -> ilAttrsThatGoOnPrimaryItem 
            | _ -> ilAttrsThatGoOnPrimaryItem 

        let ilCustomAttrs = mkILCustomAttrs (ilAttrs @ sourceNameAttribs @ ilAttrsCompilerGenerated)
        let mdef = { mdef with CustomAttrs= ilCustomAttrs } 
        EmitTheMethodDef mdef
        


and GenPInvokeMethod (nm,dll,namedArgs) =
    let decoder = AttributeDecoder namedArgs
    
    let hasPreserveSigNamedArg = decoder.FindBool "PreserveSig" true
    hasPreserveSigNamedArg,
    MethodBody.PInvoke 
      { Where=mkSimpleModRef dll
        Name=decoder.FindString "EntryPoint" nm
        CallingConv=
            match decoder.FindInt32 "CallingConvention" 0 with 
            | 1 -> PInvokeCallingConvention.WinApi
            | 2 -> PInvokeCallingConvention.Cdecl
            | 3 -> PInvokeCallingConvention.Stdcall
            | 4 -> PInvokeCallingConvention.Thiscall
            | 5 -> PInvokeCallingConvention.Fastcall
            | _ -> PInvokeCallingConvention.WinApi
        CharEncoding=
            match decoder.FindInt32 "CharSet" 0 with 
            | 1 -> PInvokeCharEncoding.None
            | 2 -> PInvokeCharEncoding.Ansi
            | 3 -> PInvokeCharEncoding.Unicode
            | 4 -> PInvokeCharEncoding.Auto
            | _  -> PInvokeCharEncoding.None
        NoMangle= decoder.FindBool "ExactSpelling" false
        LastError= decoder.FindBool "SetLastError" false
        ThrowOnUnmappableChar= if (decoder.FindBool "ThrowOnUnmappableChar" false) then PInvokeThrowOnUnmappableChar.Enabled else PInvokeThrowOnUnmappableChar.UseAssembly
        CharBestFit=if (decoder.FindBool "BestFitMapping" false) then PInvokeCharBestFit.Enabled else PInvokeCharBestFit.UseAssembly }
      

and GenBindings cenv cgbuf eenv binds = List.iter (GenBind cenv cgbuf eenv) binds

//-------------------------------------------------------------------------
// Generate locals and other storage of values
//------------------------------------------------------------------------- 

and GenSetVal cenv cgbuf eenv (vref,e,m) sequel =
    let storage = StorageForValRef m vref eenv
    match storage with
    | Env (ilCloTy,_,_,_) -> 
        CG.EmitInstr cgbuf (pop 0) (Push [ilCloTy]) mkLdarg0 
    | _ -> 
        ()
    GenExpr cenv cgbuf eenv SPSuppress e Continue
    GenSetStorage vref.Range cgbuf storage
    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel
      
and GenGetValRefAndSequel cenv cgbuf eenv m (v:ValRef) fetchSequel =
    let ty = v.Type
    GenGetStorageAndSequel cenv cgbuf eenv m (ty, GenType cenv.amap m eenv.tyenv ty) (StorageForValRef m v eenv)  fetchSequel

and GenGetVal cenv cgbuf eenv (v:ValRef,m) sequel =
    GenGetValRefAndSequel cenv cgbuf eenv m v None
    GenSequel cenv eenv.cloc cgbuf sequel
      
and GenBindRhs cenv cgbuf eenv sp (vspec:Val) e =   
    match e with 
    | Expr.TyLambda _ | Expr.Lambda _ -> 
        let isLocalTypeFunc = IsNamedLocalTypeFuncVal cenv.g vspec e
         
        match e with
        | Expr.TyLambda(_, tyargs, body, _, ttype) when 
            (
                tyargs |> List.forall (fun tp -> tp.IsErased) &&
                (match StorageForVal vspec.Range vspec eenv with Local _ -> true | _ -> false) && 
                (isLocalTypeFunc || 
                    (match ttype with 
                     TType_var(typar) -> match typar.Solution with Some(TType_app(t,_))-> t.IsStructOrEnumTycon | _ -> false
                     | _ -> false))
            ) ->
            // type lambda with erased type arguments that is stored as local variable (not method or property)- inline body
            GenExpr cenv cgbuf eenv sp body Continue
        | _ ->
            let selfv = if isLocalTypeFunc then None else Some (mkLocalValRef vspec)
            GenLambda cenv cgbuf eenv isLocalTypeFunc selfv e Continue 
    | _ -> 
        GenExpr cenv cgbuf eenv sp e Continue

and GenSetBindValue cenv cgbuf eenv eenv2 (vspec:Val) e =   
    GenBindRhs cenv cgbuf eenv2 SPSuppress vspec e
    GenStoreVal cgbuf eenv vspec.Range vspec
        
and EmitInitLocal cgbuf typ idx = CG.EmitInstrs cgbuf (pop 0) Push0  [I_ldloca (uint16 idx);  (I_initobj typ) ]
and EmitSetLocal cgbuf idx = CG.EmitInstr cgbuf (pop 1) Push0 (mkStloc (uint16 idx))
and EmitGetLocal cgbuf typ idx = CG.EmitInstr cgbuf (pop 0) (Push [typ]) (mkLdloc (uint16 idx))
and EmitSetStaticField cgbuf fspec = CG.EmitInstr cgbuf (pop 1) Push0 (mkNormalStsfld fspec)
and EmitGetStaticFieldAddr cgbuf typ fspec = CG.EmitInstr cgbuf (pop 0) (Push [typ]) (I_ldsflda fspec)
and EmitGetStaticField cgbuf typ fspec = CG.EmitInstr cgbuf (pop 0) (Push [typ]) (mkNormalLdsfld fspec)

and GenSetStorage m cgbuf storage = 
    match storage with  
    | Local (idx,_)  ->   EmitSetLocal cgbuf idx
    | StaticField (_, _, hasLiteralAttr, ilContainerTy, _, _, _, ilSetterMethRef, _) ->  
        if hasLiteralAttr then errorR(Error(FSComp.SR.ilLiteralFieldsCannotBeSet(),m))
        CG.EmitInstr cgbuf (pop 1) Push0  (I_call(Normalcall,mkILMethSpecForMethRefInTy(ilSetterMethRef,ilContainerTy,[]),None))
    | StaticProperty (ilGetterMethSpec,_) -> 
        error(Error(FSComp.SR.ilStaticMethodIsNotLambda(ilGetterMethSpec.Name),m))
    | Method (_,_,mspec,m,_,_) -> 
        error(Error(FSComp.SR.ilStaticMethodIsNotLambda(mspec.Name),m))
    | Null ->  CG.EmitInstr cgbuf (pop 1) Push0 AI_pop
    | Arg _ -> error(Error(FSComp.SR.ilMutableVariablesCannotEscapeMethod(),m))

    | Env (_,_,ilField,_) -> 
        // Note: ldarg0 has already been emitted in GenSetVal
        CG.EmitInstr cgbuf (pop 2) Push0  (mkNormalStfld ilField)

and CommitGetStorageSequel cenv cgbuf eenv m typ localCloInfo storeSequel = 
    match localCloInfo,storeSequel with 
    | Some {contents =NamedLocalIlxClosureInfoGenerator _cloinfo},_ -> error(InternalError("Unexpected generator",m))
    | Some {contents =NamedLocalIlxClosureInfoGenerated cloinfo},Some (tyargs,args,m,sequel) when not (isNil tyargs) ->
        let actualRetTy = GenNamedLocalTyFuncCall cenv cgbuf eenv typ cloinfo tyargs m
        CommitGetStorageSequel cenv cgbuf eenv m actualRetTy None (Some ([],args,m,sequel))
    | _, None -> ()
    | _,Some ([],[],_,sequel) ->
        GenSequel cenv eenv.cloc cgbuf sequel 
    | _,Some (tyargs,args,m,sequel) ->
        GenArgsAndIndirectCall cenv cgbuf eenv (typ,tyargs,args,m) sequel 

and GenGetStorageAndSequel cenv cgbuf eenv m (typ,ilTy) storage storeSequel =
    match storage with  
    | Local (idx,localCloInfo) ->
        EmitGetLocal cgbuf ilTy idx
        CommitGetStorageSequel cenv cgbuf eenv m typ localCloInfo storeSequel

    | StaticField (fspec, _, hasLiteralAttr, ilContainerTy, _, _, ilGetterMethRef, _, _) ->  
        // References to literals go directly to the field - no property is used
        if hasLiteralAttr then 
            EmitGetStaticField cgbuf ilTy fspec
        else
            CG.EmitInstr cgbuf (pop 0) (Push [ilTy])  (I_call(Normalcall, mkILMethSpecForMethRefInTy (ilGetterMethRef, ilContainerTy, []), None))
        CommitGetStorageSequel cenv cgbuf eenv m typ None storeSequel

    | StaticProperty (ilGetterMethSpec, _) -> 
        CG.EmitInstr cgbuf (pop 0) (Push [ilTy])  (I_call (Normalcall, ilGetterMethSpec, None))
        CommitGetStorageSequel cenv cgbuf eenv m typ None storeSequel

    | Method (topValInfo,vref,mspec,_,_,_) -> 
        // Get a toplevel value as a first-class value. 
        // We generate a lambda expression and that simply calls 
        // the toplevel method. However we optimize the case where we are 
        // immediately applying the value anyway (to insufficient arguments). 

        // First build a lambda expression for the saturated use of the toplevel value... 
        // REVIEW: we should NOT be doing this in the backend... 
        let expr,exprty = AdjustValForExpectedArity cenv.g m vref NormalValUse topValInfo

        // Then reduce out any arguments (i.e. apply the sequel immediately if we can...) 
        match storeSequel with 
        | None -> 
            GenLambda cenv cgbuf eenv false None expr Continue
        | Some (tyargs',args,m,sequel) -> 
            let specializedExpr = 
                if isNil args && isNil tyargs' then failwith ("non-lambda at use of method " + mspec.Name)
                MakeApplicationAndBetaReduce cenv.g (expr,exprty,[tyargs'],args,m)
            GenExpr cenv cgbuf eenv SPSuppress specializedExpr sequel

    | Null  ->   
        CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (AI_ldnull) 
        CommitGetStorageSequel cenv cgbuf eenv m typ None storeSequel

    | Arg i -> 
        CG.EmitInstr cgbuf (pop 0) (Push [ilTy]) (mkLdarg (uint16 i)) 
        CommitGetStorageSequel cenv cgbuf eenv m typ None storeSequel

    | Env (_,_,ilField,localCloInfo) -> 
        // Note: ldarg 0 is emitted in 'cu_erase' erasure of the ldenv instruction
        CG.EmitInstrs cgbuf (pop 0) (Push [ilTy]) [ mkLdarg0; mkNormalLdfld ilField ]
        CommitGetStorageSequel cenv cgbuf eenv m typ localCloInfo storeSequel

and GenGetLocalVals cenv cgbuf eenvouter m fvs = 
    List.iter (fun v -> GenGetLocalVal cenv cgbuf eenvouter m v None) fvs

and GenGetLocalVal cenv cgbuf eenv m (vspec:Val) fetchSequel =
    GenGetStorageAndSequel cenv cgbuf eenv m (vspec.Type, GenTypeOfVal cenv eenv vspec) (StorageForVal m vspec eenv) fetchSequel

and GenGetLocalVRef cenv cgbuf eenv m (vref:ValRef) fetchSequel =
    GenGetStorageAndSequel cenv cgbuf eenv m (vref.Type, GenTypeOfVal cenv eenv vref.Deref) (StorageForValRef m vref eenv) fetchSequel

and GenStoreVal cgbuf eenv m (vspec:Val) =
    GenSetStorage vspec.Range cgbuf (StorageForVal m vspec eenv)

//--------------------------------------------------------------------------
// Allocate locals for values
//-------------------------------------------------------------------------- 
 
and AllocLocal cenv cgbuf eenv compgen (v,ty,isFixed) (scopeMarks: Mark * Mark) = 
     // The debug range for the local
     let ranges = if compgen then [] else [(v,scopeMarks)]
     // Get an index for the local
     let j = 
        if cenv.opts.localOptimizationsAreOn 
        then cgbuf.ReallocLocal((fun i (_,ty',isFixed') -> not isFixed' && not isFixed && not (IntMap.mem i eenv.liveLocals) && (ty = ty')),ranges,ty,isFixed)
        else cgbuf.AllocLocal(ranges,ty,isFixed)
     j, { eenv with liveLocals =  IntMap.add j () eenv.liveLocals  }

and AllocLocalVal cenv cgbuf v eenv repr scopeMarks = 
    let repr,eenv = 
        let ty = v.Type
        if isUnitTy cenv.g ty && not v.IsMutable then  Null,eenv
        elif Option.isSome repr && IsNamedLocalTypeFuncVal cenv.g v (Option.get repr) then 
            (* known, named, non-escaping type functions *)
            let cloinfoGenerate eenv = 
                let eenvinner = 
                    {eenv with 
                         letBoundVars=(mkLocalValRef v)::eenv.letBoundVars}
                let cloinfo,_,_ = GetIlxClosureInfo cenv v.Range true None eenvinner (Option.get repr)
                cloinfo
            
            let idx,eenv = AllocLocal cenv cgbuf eenv v.IsCompilerGenerated (v.CompiledName, cenv.g.ilg.typ_Object, false) scopeMarks
            Local (idx,Some(ref (NamedLocalIlxClosureInfoGenerator cloinfoGenerate))),eenv
        else
            (* normal local *)
            let idx,eenv = AllocLocal cenv cgbuf eenv v.IsCompilerGenerated (v.CompiledName, GenTypeOfVal cenv eenv v, v.IsFixed) scopeMarks
            Local (idx,None),eenv
    let eenv = AddStorageForVal cenv.g (v,notlazy repr) eenv
    Some repr, eenv

and AllocStorageForBind cenv cgbuf scopeMarks eenv bind = 
    AllocStorageForBinds cenv cgbuf scopeMarks eenv [bind]

and AllocStorageForBinds cenv cgbuf scopeMarks eenv binds = 
    // phase 1 - decide representations - most are very simple. 
    let reps, eenv = List.mapFold (AllocValForBind cenv cgbuf scopeMarks) eenv binds 

    // Phase 2 - run the cloinfo generators for NamedLocalClosure values against the environment recording the 
    // representation choices. 
    reps |> List.iter (fun reprOpt -> 
       match reprOpt with 
       | Some repr -> 
           match repr with 
           | Local(_,Some g) 
           | Env(_,_,_,Some g) -> 
               match !g with 
               | NamedLocalIlxClosureInfoGenerator f -> g := NamedLocalIlxClosureInfoGenerated (f eenv) 
               | NamedLocalIlxClosureInfoGenerated _ -> ()
           | _ -> ()
       | _ -> ())

    eenv
   
and AllocValForBind cenv cgbuf (scopeMarks: Mark * Mark) eenv (TBind(v,repr,_)) =
    match v.ValReprInfo with 
    | None -> 
        AllocLocalVal cenv cgbuf v eenv (Some repr) scopeMarks
    | Some _ -> 
        None,AllocTopValWithinExpr cenv cgbuf eenv.cloc scopeMarks v eenv


and AllocTopValWithinExpr cenv cgbuf cloc scopeMarks v eenv =
    // decide whether to use a shadow local or not
    let useShadowLocal = 
        cenv.opts.generateDebugSymbols && 
        not cenv.opts.localOptimizationsAreOn &&
        not v.IsCompilerGenerated &&
        not v.IsMutable &&
        // Don't use shadow locals for things like functions which are not compiled as static values/properties
        IsCompiledAsStaticProperty cenv.g v

    let optShadowLocal,eenv = 
        if useShadowLocal then 
            let storageOpt, eenv = AllocLocalVal cenv cgbuf v eenv None scopeMarks 
            match storageOpt with 
            | None -> NoShadowLocal,eenv
            | Some storage -> ShadowLocal storage,eenv
            
        else 
            NoShadowLocal,eenv

    ComputeAndAddStorageForLocalTopVal (cenv.amap, cenv.g, cenv.intraAssemblyInfo, cenv.opts.isInteractive, optShadowLocal) cloc v eenv



//--------------------------------------------------------------------------
// Generate stack save/restore and assertions - pulled into letrec by alloc*
//-------------------------------------------------------------------------- 

/// Save the stack
/// - [gross] because IL flushes the stack at the exn. handler
/// - and     because IL requires empty stack following a forward br (jump).
and EmitSaveStack cenv cgbuf eenv m scopeMarks =
    let savedStack = (cgbuf.GetCurrentStack())
    let savedStackLocals,eenvinner = List.mapFold (fun eenv ty -> AllocLocal cenv cgbuf eenv true (ilxgenGlobalNng.FreshCompilerGeneratedName ("spill",m), ty, false) scopeMarks) eenv savedStack
    List.iter (EmitSetLocal cgbuf) savedStackLocals
    cgbuf.AssertEmptyStack()
    (savedStack,savedStackLocals),eenvinner (* need to return, it marks locals "live" *)

/// Restore the stack and load the result 
and EmitRestoreStack cgbuf (savedStack,savedStackLocals) =
    cgbuf.AssertEmptyStack()
    List.iter2 (EmitGetLocal cgbuf) (List.rev savedStack) (List.rev savedStackLocals)

//-------------------------------------------------------------------------
//GenAttr: custom attribute generation
//------------------------------------------------------------------------- 

and GenAttribArg amap g eenv x (ilArgTy:ILType) = 

    match x,ilArgTy with 

    // Detect 'null' used for an array argument
    | Expr.Const(Const.Zero,_,_),ILType.Array  _ -> 
        ILAttribElem.Null

    // Detect standard constants 
    | Expr.Const(c,m,_),_ -> 
        let tynm = ilArgTy.TypeSpec.Name
        let isobj = (tynm = "System.Object")

        match c with 
        | Const.Bool b -> ILAttribElem.Bool b
        | Const.Int32 i when isobj || tynm = "System.Int32" ->  ILAttribElem.Int32 ( i)
        | Const.Int32 i when tynm = "System.SByte" ->  ILAttribElem.SByte (sbyte i)
        | Const.Int32 i when tynm = "System.Int16"  -> ILAttribElem.Int16 (int16 i)
        | Const.Int32 i when tynm = "System.Byte"  -> ILAttribElem.Byte (byte i)
        | Const.Int32 i when tynm = "System.UInt16" ->ILAttribElem.UInt16 (uint16 i)
        | Const.Int32 i when tynm = "System.UInt32" ->ILAttribElem.UInt32 (uint32 i)
        | Const.Int32 i when tynm = "System.UInt64" ->ILAttribElem.UInt64 (uint64 (int64 i)) 
        | Const.SByte  i  -> ILAttribElem.SByte i
        | Const.Int16  i  -> ILAttribElem.Int16 i
        | Const.Int32 i   -> ILAttribElem.Int32 i
        | Const.Int64 i   -> ILAttribElem.Int64 i  
        | Const.Byte i    -> ILAttribElem.Byte i
        | Const.UInt16 i  -> ILAttribElem.UInt16 i
        | Const.UInt32 i  -> ILAttribElem.UInt32 i
        | Const.UInt64 i  -> ILAttribElem.UInt64 i
        | Const.Double i   -> ILAttribElem.Double i
        | Const.Single i -> ILAttribElem.Single i
        | Const.Char i    -> ILAttribElem.Char i
        | Const.Zero when   isobj    -> ILAttribElem.Null
        | Const.Zero when   tynm = "System.String"   -> ILAttribElem.String None
        | Const.Zero when   tynm = "System.Type"   -> ILAttribElem.Type None
        | Const.String i  when isobj || tynm = "System.String" ->  ILAttribElem.String (Some i)
        | _ -> error (InternalError ( "The type '" + tynm + "' may not be used as a custom attribute value",m))

    // Detect '[| ... |]' nodes 
    | Expr.Op(TOp.Array,[elemTy],args,m),_ ->
        let ilElemTy = GenType amap m eenv.tyenv elemTy
        ILAttribElem.Array (ilElemTy, List.map (fun arg -> GenAttribArg amap g eenv arg ilElemTy) args)

    // Detect 'typeof<ty>' calls  
    | TypeOfExpr g ty, _    ->
        ILAttribElem.Type (Some (GenType amap x.Range eenv.tyenv ty))

    // Detect 'typedefof<ty>' calls 
    | TypeDefOfExpr g ty, _    ->
        ILAttribElem.TypeRef (Some (GenType amap x.Range eenv.tyenv ty).TypeRef)    
    
    // Ignore upcasts 
    | Expr.Op(TOp.Coerce,_,[arg2],_),_ ->
        GenAttribArg amap g eenv arg2 ilArgTy

    // Detect explicit enum values 
    | EnumExpr g arg1, _ ->
        GenAttribArg amap g eenv arg1 ilArgTy
    

    // Detect bitwise or of attribute flags: one case of constant folding (a more general treatment is needed)
    
    | AttribBitwiseOrExpr g (arg1,arg2),_ ->
        let v1 = GenAttribArg amap g eenv arg1 ilArgTy 
        let v2 = GenAttribArg amap g eenv arg2 ilArgTy 
        match v1,v2 with 
        | ILAttribElem.SByte i1, ILAttribElem.SByte i2 -> ILAttribElem.SByte (i1 ||| i2) 
        | ILAttribElem.Int16 i1, ILAttribElem.Int16 i2-> ILAttribElem.Int16 (i1 ||| i2)
        | ILAttribElem.Int32 i1, ILAttribElem.Int32 i2-> ILAttribElem.Int32 (i1 ||| i2)
        | ILAttribElem.Int64 i1, ILAttribElem.Int64 i2-> ILAttribElem.Int64 (i1 ||| i2)
        | ILAttribElem.Byte i1, ILAttribElem.Byte i2-> ILAttribElem.Byte (i1 ||| i2)
        | ILAttribElem.UInt16 i1, ILAttribElem.UInt16 i2-> ILAttribElem.UInt16 (i1 ||| i2) 
        | ILAttribElem.UInt32 i1, ILAttribElem.UInt32 i2-> ILAttribElem.UInt32 (i1 ||| i2) 
        | ILAttribElem.UInt64 i1, ILAttribElem.UInt64 i2-> ILAttribElem.UInt64 (i1 ||| i2)
        |  _ -> error (InternalError ("invalid custom attribute value (not a valid constant): " + showL (exprL x),x.Range))

    // Other expressions are not valid custom attribute values
    | _ -> 
        error (InternalError ("invalid custom attribute value (not a constant): " + showL (exprL x),x.Range))


and GenAttr amap g eenv (Attrib(_,k,args,props,_,_,_)) = 
    let props = 
        props |> List.map (fun (AttribNamedArg(s,ty,fld,AttribExpr(_,expr))) ->
            let m = expr.Range
            let ilTy = GenType amap m eenv.tyenv ty
            let cval = GenAttribArg amap g eenv expr ilTy
            (s,ilTy,fld,cval))
    let mspec = 
        match k with 
        | ILAttrib(mref) -> mkILMethSpec(mref,AsObject,[],[]) 
        | FSAttrib(vref) -> 
             assert(vref.IsMember) 
             let mspec,_,_,_,_ = GetMethodSpecForMemberVal amap g (Option.get vref.MemberInfo) vref
             mspec
    let ilArgs = List.map2 (fun (AttribExpr(_,vexpr)) ty -> GenAttribArg amap g eenv vexpr ty) args mspec.FormalArgTypes
    mkILCustomAttribMethRef g.ilg (mspec,ilArgs, props)
    
and GenAttrs cenv eenv attrs = List.map (GenAttr cenv.amap cenv.g eenv) attrs

and GenCompilationArgumentCountsAttr cenv (v:Val) =
    [ match v.ValReprInfo with 
      | Some(tvi) when v.IsMemberOrModuleBinding -> 
          let arities = if ValSpecIsCompiledAsInstance cenv.g v then List.tail tvi.AritiesOfArgs else tvi.AritiesOfArgs 
          if arities.Length > 1 then 
              yield mkCompilationArgumentCountsAttr cenv.g arities
      |  _ -> 
          () ]          

// Create a permission set for a list of security attributes   
and CreatePermissionSets g amap eenv (securityAttributes : Attrib list) = 
    [for ((Attrib(tcref,_,actions,_,_,_,_)) as attr) in securityAttributes do
        let action = match actions with | [AttribInt32Arg act] -> act | _ -> failwith "internal error: unrecognized security action"
        let secaction = (List.assoc action (Lazy.force ILSecurityActionRevMap))
        let tref = tcref.CompiledRepresentationForNamedType
        let ilattr = GenAttr amap g eenv attr
        let _, ilNamedArgs = 
            match TryDecodeILAttribute g tref (mkILCustomAttrs [ilattr]) with
            | Some(ae,na) -> ae, na
            | _ -> [],[]
        let setArgs = ilNamedArgs |> List.map (fun (n,ilt,_,ilae) -> (n,ilt,ilae))
        yield IL.mkPermissionSet g.ilg (secaction, [(tref, setArgs)])]

//--------------------------------------------------------------------------
// Generate the set of modules for an assembly, and the declarations in each module
//-------------------------------------------------------------------------- 

/// Generate a static class at the given cloc
and GenTypeDefForCompLoc (cenv, eenv, mgbuf: AssemblyBuilder, cloc, hidden, attribs, initTrigger, eliminateIfEmpty, addAtEnd)  = 
    let tref = TypeRefForCompLoc cloc
    let tdef = 
      mkILSimpleClass cenv.g.ilg
        (tref.Name, 
         ComputeTypeAccess tref hidden,
         emptyILMethods, 
         emptyILFields,
         emptyILTypeDefs,
         emptyILProperties,
         emptyILEvents,
         mkILCustomAttrs 
           (GenAttrs cenv eenv attribs @
            (if List.contains tref.Name [TypeNameForImplicitMainMethod cloc; TypeNameForInitClass cloc; TypeNameForPrivateImplementationDetails cloc]  
             then [ ] 
             else [mkCompilationMappingAttr cenv.g (int SourceConstructFlags.Module)])),
         initTrigger)
    let tdef = { tdef with IsSealed=true; IsAbstract=true }
    mgbuf.AddTypeDef(tref, tdef, eliminateIfEmpty, addAtEnd, None)


and GenModuleExpr cenv cgbuf qname lazyInitInfo eenv x   = 
    let (ModuleOrNamespaceExprWithSig(mty,def,_)) = x 
    // REVIEW: the scopeMarks are used for any shadow locals we create for the module bindings 
    // We use one scope for all the bindings in the module, which makes them all appear with their "default" values
    // rather than incrementally as we step through the  initializations in the module. This is a little unfortunate 
    // but stems from the way we add module values all at once before we generate the module itself.
    LocalScope "module" cgbuf (fun scopeMarks ->
        let sigToImplRemapInfo = ComputeRemappingFromImplementationToSignature cenv.g def mty
        let eenv = AddSignatureRemapInfo "defs" sigToImplRemapInfo eenv
        let eenv = 
            // Allocate all the values, including any shadow locals for static fields
            let allocVal cloc v = AllocTopValWithinExpr cenv cgbuf cloc scopeMarks v
            AddBindingsForModuleDef allocVal eenv.cloc eenv def
        GenModuleDef cenv cgbuf qname lazyInitInfo eenv def)

and GenModuleDefs cenv cgbuf qname lazyInitInfo eenv  mdefs = 
    mdefs |> List.iter (GenModuleDef cenv cgbuf qname lazyInitInfo eenv) 
    
and GenModuleDef cenv (cgbuf:CodeGenBuffer) qname lazyInitInfo eenv  x = 
    match x with 
    | TMDefRec(_isRec,tycons,mbinds,m) -> 
        tycons |> List.iter (fun tc -> 
            if tc.IsExceptionDecl 
            then GenExnDef cenv cgbuf.mgbuf eenv m tc 
            else GenTypeDef cenv cgbuf.mgbuf lazyInitInfo eenv m tc)
        mbinds |> List.iter (GenModuleBinding cenv cgbuf qname lazyInitInfo eenv m) 

    | TMDefLet(bind,_) -> 
        GenBindings cenv cgbuf eenv [bind]

    | TMDefDo(e,_) -> 
        GenExpr cenv cgbuf eenv SPAlways e discard

    | TMAbstract(mexpr) -> 
        GenModuleExpr cenv cgbuf qname lazyInitInfo eenv mexpr

    | TMDefs(mdefs) -> 
        GenModuleDefs cenv cgbuf qname lazyInitInfo eenv  mdefs


// Generate a module binding
and GenModuleBinding cenv (cgbuf:CodeGenBuffer) (qname:QualifiedNameOfFile) lazyInitInfo eenv m x = 
  match x with 
  | ModuleOrNamespaceBinding.Binding bind -> 
    GenLetRecBinds cenv cgbuf eenv ([bind],m)
  | ModuleOrNamespaceBinding.Module (mspec, mdef) ->
    let hidden = IsHiddenTycon eenv.sigToImplRemapInfo mspec

    let eenvinner = 
        if mspec.IsNamespace then eenv else 
        {eenv with cloc = CompLocForFixedModule cenv.opts.fragName qname.Text mspec }

    // Create the class to hold the contents of this module.  No class needed if 
    // we're compiling it as a namespace. 
    //
    // Most module static fields go into the "InitClass" static class. 
    // However mutable static fields go into the class for the module itself. 
    // So this static class ends up with a .cctor if it has mutable fields. 
    //
    if not mspec.IsNamespace then 
        // The use of ILTypeInit.OnAny prevents the execution of the cctor before the 
        // "main" method in the case where the "main" method is implicit.
        let staticClassTrigger = (* if eenv.isFinalFile then *) ILTypeInit.OnAny (* else ILTypeInit.BeforeField *)

        GenTypeDefForCompLoc (cenv, eenvinner, cgbuf.mgbuf, eenvinner.cloc, hidden, mspec.Attribs, staticClassTrigger, false, (* atEnd= *) true)

    // Generate the declarations in the module and its initialization code 
    GenModuleDef cenv cgbuf qname lazyInitInfo eenvinner mdef
    
    // If the module has a .cctor for some mutable fields, we need to ensure that when 
    // those fields are "touched" the InitClass .cctor is forced. The InitClass .cctor will 
    // then fill in the value of the mutable fields.
    if not mspec.IsNamespace && (cgbuf.mgbuf.GetCurrentFields(TypeRefForCompLoc eenvinner.cloc) |> Seq.isEmpty |> not) then 
        GenForceWholeFileInitializationAsPartOfCCtor cenv cgbuf.mgbuf lazyInitInfo (TypeRefForCompLoc eenvinner.cloc) mspec.Range


/// Generate the namespace fragments in a single file
and GenTopImpl cenv mgbuf mainInfoOpt eenv (TImplFile(qname, _, mexpr, hasExplicitEntryPoint, isScript), optimizeDuringCodeGen)  =
    let eenv = {eenv with cloc = { eenv.cloc with clocTopImplQualifiedName = qname.Text } }

    cenv.optimizeDuringCodeGen <- optimizeDuringCodeGen

    // This is used to point the inner classes back to the startup module for initialization purposes 
    let isFinalFile = Option.isSome mainInfoOpt

    let initClassCompLoc = CompLocForInitClass eenv.cloc 
    let initClassTy = mkILTyForCompLoc initClassCompLoc 

    let initClassTrigger = (* if isFinalFile then *) ILTypeInit.OnAny (* else ILTypeInit.BeforeField *)
    
    let eenv = {eenv with cloc = initClassCompLoc
                          isFinalFile = isFinalFile
                          someTypeInThisAssembly = initClassTy } 
  
    // Create the class to hold the initialization code and static fields for this file.  
    //     internal static class $<StartupCode...> {}
    // Put it at the end since that gives an approximation of dependency order (to aid FSI.EXE's code generator - see FSharp 1.0 5548)
    GenTypeDefForCompLoc (cenv, eenv, mgbuf, initClassCompLoc, useHiddenInitCode, [], initClassTrigger, false, (*atEnd=*)true) 
    
    // lazyInitInfo is an accumulator of functions which add the forced initialization of the storage module to
    //    - mutable fields in public modules
    //    - static "let" bindings in types 
    // These functions only get executed/committed if we actually end up producing some code for the .cctor for the storage module.
    // The existence of .cctors adds costs to execution, so this is a half-sensible attempt to avoid adding them when possible. 
    let lazyInitInfo = new ResizeArray<ILFieldSpec -> ILInstr list -> ILInstr list -> unit>()

    // codegen .cctor/main for outer module
    let m = qname.Range
    let clocCcu = CompLocForCcu cenv.viewCcu
    
    // This method name is only used internally in ilxgen.fs to aid debugging
    let methodName = 
        match mainInfoOpt with 
        //   Library file 
        | None -> ".cctor" 
        //   Final file, explicit entry point 
        | Some _ when hasExplicitEntryPoint -> ".cctor"
        //   Final file, implicit entry point 
        | Some _ -> mainMethName 
        
    // topInstrs is ILInstr[] and contains the abstract IL for this file's top-level actions. topCode is the ILMethodBody for that same code.
    let topInstrs,topCode = 
        CodeGenMethod cenv mgbuf 
            (true,[],methodName,eenv,0,0,
             (fun cgbuf eenv -> 
                  GenModuleExpr cenv cgbuf qname lazyInitInfo eenv mexpr
                  CG.EmitInstr cgbuf (pop 0) Push0 I_ret),m)

    // The code generation for the initialization is now complete and the IL code is in topCode.
    // Make a .cctor and/or main method to contain the code.  This initializes all modules. 
    //   Library file (mainInfoOpt = None) : optional .cctor if topCode has initialization effect
    //   Final file, explicit entry point (mainInfoOpt = Some _, GetExplicitEntryPointInfo() = Some) : main + optional .cctor if topCode has initialization effect
    //   Final file, implicit entry point (mainInfoOpt = Some _, GetExplicitEntryPointInfo() = None) : main + initialize + optional .cctor calling initialize
    

    let doesSomething = CheckCodeDoesSomething topCode.Code

    // Make a FEEFEE instruction to mark hidden code regions
    // We expect the first instruction to be a sequence point when generating debug symbols
    let feefee, seqpt = 
        if topInstrs.Length > 1 then 
            match topInstrs.[0] with 
            | I_seqpoint sp as i -> [ FeeFeeInstr cenv sp.Document ], [ i ]
            | _ -> [], [] 
        else 
            [], []

    begin

        match mainInfoOpt with 

        // Final file in .EXE
        | Some mainInfo -> 

            // Generate an explicit main method. If necessary, make a class constructor as 
            // well for the bindings earlier in the file containing the entrypoint.  
            match mgbuf.GetExplicitEntryPointInfo() with

            // Final file, explicit entry point : place the code in a .cctor, and add code to main that forces the .cctor (if topCode has initialization effect).
            | Some tref ->           
                if doesSomething then
                    lazyInitInfo.Add (fun fspec feefee seqpt -> 
                        // This adds the explicit init of the .cctor to the explicit entrypoint main method
                        mgbuf.AddExplicitInitToSpecificMethodDef((fun md -> md.IsEntryPoint), tref, fspec, GenPossibleILSourceMarker cenv m, feefee, seqpt))

                    let cctorMethDef = mkILClassCtor (MethodBody.IL topCode) 
                    mgbuf.AddMethodDef(initClassTy.TypeRef,cctorMethDef)

            // Final file, implicit entry point. We generate no .cctor.
            //       void main@() { 
            //             <topCode> 
            //    }
            | None ->

                let ilAttrs = mkILCustomAttrs (GenAttrs cenv eenv mainInfo)
                if not cenv.opts.isInteractive && not doesSomething then 
                    let errorM = m.EndRange
                    warning (Error(FSComp.SR.ilMainModuleEmpty(), errorM))

                // generate main@ 
                let ilMainMethodDef = 
                    let mdef = mkILNonGenericStaticMethod(mainMethName,ILMemberAccess.Public,[],mkILReturn ILType.Void, MethodBody.IL topCode) 
                    {mdef with IsEntryPoint= true; CustomAttrs = ilAttrs } 

                mgbuf.AddMethodDef(initClassTy.TypeRef,ilMainMethodDef)


        //   Library file : generate an optional .cctor if topCode has initialization effect
        | None -> 
            if doesSomething then 

                // Add the cctor 
                let cctorMethDef =  mkILClassCtor (MethodBody.IL topCode) 
                mgbuf.AddMethodDef(initClassTy.TypeRef,cctorMethDef)

    
    end
    
    // Commit the directed initializations
    if doesSomething then 
        // Create the field to act as the target for the forced initialization. 
        // Why do this for the final file?
        // There is no need to do this for a final file with an implicit entry point. For an explicit entry point in lazyInitInfo.
        let initFieldName = CompilerGeneratedName "init"
        let ilFieldDef = 
            mkILStaticField (initFieldName,cenv.g.ilg.typ_Int32, None, None, ComputeMemberAccess true)
            |> cenv.g.AddFieldNeverAttrs 
            |> cenv.g.AddFieldGeneratedAttrs 

        let fspec = mkILFieldSpecInTy (initClassTy, initFieldName, cenv. g.ilg.typ_Int32)
        CountStaticFieldDef()
        mgbuf.AddFieldDef(initClassTy.TypeRef,ilFieldDef) 

        // Run the imperative (yuck!) actions that force the generation 
        // of references to the cctor for nested modules etc. 
        lazyInitInfo |> Seq.iter (fun f -> f fspec feefee seqpt)

        if isScript && not(isFinalFile) then 
            mgbuf.AddScriptInitFieldSpec(fspec,m)    

    // Compute the ilxgenEnv after the generation of the module, i.e. the residue need to generate anything that
    // uses the constructs exported from this module.
    // We add the module type all over again. Note no shadow locals for static fields needed here since they are only relevant to the main/.cctor
    let eenvafter = 
        let allocVal = ComputeAndAddStorageForLocalTopVal (cenv.amap, cenv.g, cenv.intraAssemblyInfo, cenv.opts.isInteractive, NoShadowLocal) 
        AddBindingsForLocalModuleType allocVal  clocCcu eenv mexpr.Type

    eenvafter

and GenForceWholeFileInitializationAsPartOfCCtor cenv (mgbuf:AssemblyBuilder) (lazyInitInfo: ResizeArray<_>) tref m =
    // Authoring a .cctor with effects forces the cctor for the 'initialization' module by doing a dummy store & load of a field 
    // Doing both a store and load keeps FxCop happier because it thinks the field is useful 
    lazyInitInfo.Add (fun fspec feefee seqpt -> mgbuf.AddExplicitInitToSpecificMethodDef((fun md -> md.Name = ".cctor"), tref, fspec, GenPossibleILSourceMarker cenv m, feefee, seqpt)) 


/// Generate an Equals method.  
and GenEqualsOverrideCallingIComparable cenv (tcref:TyconRef, ilThisTy, _ilThatTy) =
    let mspec = mkILNonGenericInstanceMethSpecInTy (cenv.g.iltyp_IComparable, "CompareTo", [cenv.g.ilg.typ_Object], cenv.g.ilg.typ_Int32)
    
    mkILNonGenericVirtualMethod
        ("Equals",ILMemberAccess.Public,
         [mkILParamNamed ("obj",cenv.g.ilg.typ_Object)], 
         mkILReturn cenv.g.ilg.typ_Bool,
         mkMethodBody(true,[],2,
                         nonBranchingInstrsToCode
                            [ yield mkLdarg0
                              yield mkLdarg 1us 
                              if tcref.IsStructOrEnumTycon then 
                                  yield I_callconstraint ( Normalcall, ilThisTy,mspec,None)
                              else 
                                  yield I_callvirt ( Normalcall, mspec,None) 
                              yield mkLdcInt32 (0)
                              yield AI_ceq ], 
                         None))
    |> AddNonUserCompilerGeneratedAttribs cenv.g

and GenFieldInit m c =
    match c with 
    | ConstToILFieldInit fieldInit -> fieldInit
    | _ -> error(Error(FSComp.SR.ilTypeCannotBeUsedForLiteralField(),m))

and GenAbstractBinding cenv eenv tref (vref:ValRef) =
    assert(vref.IsMember)
    let m = vref.Range
    let memberInfo = Option.get vref.MemberInfo
    let attribs = vref.Attribs
    let hasPreserveSigImplFlag,hasSynchronizedImplFlag,hasNoInliningFlag,attribs = ComputeMethodImplAttribs cenv vref.Deref attribs
    if memberInfo.MemberFlags.IsDispatchSlot && not memberInfo.IsImplemented then 
        let ilAttrs = 
            [ yield! GenAttrs cenv eenv attribs 
              yield! GenCompilationArgumentCountsAttr cenv vref.Deref ]
        
        let mspec,ctps,mtps,argInfos,retInfo = GetMethodSpecForMemberVal cenv.amap cenv.g memberInfo vref 
        let eenvForMeth = EnvForTypars (ctps@mtps) eenv
        let ilMethTypars = GenGenericParams cenv eenvForMeth mtps
        let ilReturn = GenReturnInfo cenv eenvForMeth mspec.FormalReturnType retInfo
        let ilParams = GenParams cenv eenvForMeth mspec argInfos None
        
        let compileAsInstance = ValRefIsCompiledAsInstanceMember cenv.g vref
        let mdef = mkILGenericVirtualMethod (vref.CompiledName,ILMemberAccess.Public,ilMethTypars,ilParams,ilReturn,MethodBody.Abstract)

        let mdef = fixupVirtualSlotFlags mdef
        let mdef = 
          {mdef with 
            IsPreserveSig=hasPreserveSigImplFlag
            IsSynchronized=hasSynchronizedImplFlag
            IsNoInline=hasNoInliningFlag
            mdKind=match mdef.mdKind with 
                    | MethodKind.Virtual vinfo -> 
                        MethodKind.Virtual {vinfo with IsFinal=memberInfo.MemberFlags.IsFinal
                                                       IsAbstract=memberInfo.MemberFlags.IsDispatchSlot } 
                    | k -> k }
        
        match memberInfo.MemberFlags.MemberKind with 
        | MemberKind.ClassConstructor 
        | MemberKind.Constructor 
        | MemberKind.Member -> 
             let mdef = {mdef with CustomAttrs= mkILCustomAttrs ilAttrs }
             [mdef], [], []
        | MemberKind.PropertyGetSet -> error(Error(FSComp.SR.ilUnexpectedGetSetAnnotation(),m))
        | MemberKind.PropertySet | MemberKind.PropertyGet ->
             let v = vref.Deref
             let vtyp = ReturnTypeOfPropertyVal cenv.g v
             if CompileAsEvent cenv.g attribs then 
                   
                 let edef = GenEventForProperty cenv eenvForMeth mspec v ilAttrs m vtyp 
                 [],[],[edef]
             else
                 let ilPropDef = 
                     let ilPropTy = GenType cenv.amap m eenvForMeth.tyenv vtyp
                     let ilArgTys = v |> ArgInfosOfPropertyVal cenv.g |> List.map fst |> GenTypes cenv.amap m eenvForMeth.tyenv
                     GenPropertyForMethodDef compileAsInstance tref mdef v memberInfo ilArgTys ilPropTy (mkILCustomAttrs ilAttrs) None
                 let mdef = mdef |> AddSpecialNameFlag
                 [mdef], [ilPropDef],[]

    else 
        [],[],[]

and GenTypeDef cenv mgbuf lazyInitInfo eenv m (tycon:Tycon) =
    let genToString ilThisTy = 
        [
        match (eenv.valsInScope.TryFind cenv.g.sprintf_vref.Deref,
               eenv.valsInScope.TryFind cenv.g.new_format_vref.Deref) with
        | Some(Lazy(Method(_,_,sprintfMethSpec,_,_,_))), Some(Lazy(Method(_,_,newFormatMethSpec,_,_,_))) ->
               // The type returned by the 'sprintf' call
               let funcTy = EraseClosures.mkILFuncTy cenv.g.ilxPubCloEnv ilThisTy cenv.g.ilg.typ_String
               // Give the instantiation of the printf format object, i.e. a Format`5 object compatible with StringFormat<ilThisTy>
               let newFormatMethSpec = mkILMethSpec(newFormatMethSpec.MethodRef,AsObject,
                                               [// 'T -> string'
                                               funcTy 
                                               // rest follow from 'StringFormat<T>'
                                               GenUnitTy cenv eenv m  
                                               cenv.g.ilg.typ_String 
                                               cenv.g.ilg.typ_String 
                                               ilThisTy],[])
               // Instantiate with our own type
               let sprintfMethSpec = mkILMethSpec(sprintfMethSpec.MethodRef,AsObject,[],[funcTy])
               // Here's the body of the method. Call printf, then invoke the function it returns
               let callInstrs = EraseClosures.mkCallFunc cenv.g.ilxPubCloEnv (fun _ -> 0us) eenv.tyenv.Count Normalcall (Apps_app(ilThisTy, Apps_done cenv.g.ilg.typ_String))
               let ilMethodDef = mkILNonGenericVirtualMethod ("ToString",ILMemberAccess.Public,[],
                                           mkILReturn cenv.g.ilg.typ_String,
                                           mkMethodBody (true,[],2,nonBranchingInstrsToCode 
                                                   ([ // load the hardwired format string
                                                       yield I_ldstr "%+A"  
                                                       // make the printf format object
                                                       yield mkNormalNewobj newFormatMethSpec
                                                       // call sprintf
                                                       yield mkNormalCall sprintfMethSpec 
                                                       // call the function returned by sprintf
                                                       yield mkLdarg0 
                                                       if ilThisTy.Boxity = ILBoxity.AsValue then
                                                           yield mkNormalLdobj ilThisTy  ] @
                                                       callInstrs),
                                                   None))
               let mdef = { ilMethodDef with CustomAttrs = mkILCustomAttrs [ cenv.g.CompilerGeneratedAttribute ] }
               yield mdef
        | None,_ -> ()
        | _,None -> ()
        | _ -> ()]
    let tcref = mkLocalTyconRef tycon
    if tycon.IsTypeAbbrev then () else
    match tycon.TypeReprInfo with 
#if EXTENSIONTYPING
    | TProvidedNamespaceExtensionPoint _ -> ()
    | TProvidedTypeExtensionPoint _ -> ()
#endif
    | TNoRepr -> ()
    | TAsmRepr _ | TILObjectRepr _ | TMeasureableRepr _ -> () 
    | TFSharpObjectRepr _ | TRecdRepr _ | TUnionRepr _ -> 
        let eenvinner = ReplaceTyenv (TypeReprEnv.ForTycon tycon) eenv
        let thisTy = generalizedTyconRef tcref

        let ilThisTy      = GenType cenv.amap m eenvinner.tyenv thisTy
        let tref = ilThisTy.TypeRef
        let ilGenParams   = GenGenericParams cenv eenvinner tycon.TyparsNoRange
        let ilIntfTys     = tycon.ImmediateInterfaceTypesOfFSharpTycon |> List.map (GenType cenv.amap m eenvinner.tyenv) 
        let ilTypeName    = tref.Name

        let hidden     = IsHiddenTycon eenv.sigToImplRemapInfo tycon
        let hiddenRepr = hidden || IsHiddenTyconRepr eenv.sigToImplRemapInfo tycon
        let access     = ComputeTypeAccess tref hidden

        // The implicit augmentation doesn't actually create CompareTo(object) or Object.Equals 
        // So we do it here. 
        //
        // Note you only have to implement 'System.IComparable' to customize structural comparison AND equality on F# types 
        // See also FinalTypeDefinitionChecksAtEndOfInferenceScope in tc.fs
        //      
        // Generate an Equals method implemented via IComparable if the type EXPLICITLY implements IComparable.
        // HOWEVER, if the type doesn't override Object.Equals already.  
        let augmentOverrideMethodDefs = 

              (if Option.isNone tycon.GeneratedCompareToValues &&
                  Option.isNone tycon.GeneratedHashAndEqualsValues &&
                  tycon.HasInterface cenv.g cenv.g.mk_IComparable_ty && 
                  not (tycon.HasOverride cenv.g "Equals" [cenv.g.obj_ty]) &&
                  not tycon.IsFSharpInterfaceTycon
               then 
                  [ GenEqualsOverrideCallingIComparable cenv (tcref,ilThisTy,ilThisTy) ] 
               else [])

        // Generate the interface slots and abstract slots.  
        let abstractMethodDefs,abstractPropDefs, abstractEventDefs = 
            if tycon.IsFSharpDelegateTycon then 
                [],[],[]
            else
                // sort by order of declaration
                // REVIEW: this should be based off tcaug_adhoc_list, which is in declaration order
                tycon.MembersOfFSharpTyconSorted
                |> List.sortWith (fun v1 v2 -> rangeOrder.Compare(v1.DefinitionRange,v2.DefinitionRange))
                |> List.map (GenAbstractBinding cenv eenv tref)
                |> List.unzip3 
                |> mapTriple (List.concat, List.concat, List.concat)


        let abstractPropDefs = abstractPropDefs |> MergePropertyDefs m
        let isAbstract =  isAbstractTycon tycon

        // Generate all the method impls showing how various abstract slots and interface slots get implemented
        // REVIEW: no method impl generated for IStructuralHash or ICompare 
        let methodImpls = 
            [ for vref in tycon.MembersOfFSharpTyconByName |> NameMultiMap.range  do
                 assert(vref.IsMember)
                 let memberInfo = vref.MemberInfo.Value
                 if memberInfo.MemberFlags.IsOverrideOrExplicitImpl && not (CompileAsEvent cenv.g vref.Attribs) then 

                     for slotsig in memberInfo.ImplementedSlotSigs do

                         if isInterfaceTy cenv.g slotsig.ImplementedType then

                             match vref.ValReprInfo with 
                             | Some _ -> 

                                 let memberParentTypars,memberMethodTypars = 
                                     match PartitionValRefTypars cenv.g vref with
                                     | Some(_,memberParentTypars,memberMethodTypars,_,_) -> memberParentTypars,memberMethodTypars
                                     | None -> [],[]

                                 let useMethodImpl = true
                                 let eenvUnderTypars = EnvForTypars memberParentTypars eenv
                                 let _,methodImplGenerator = GenMethodImpl cenv eenvUnderTypars (useMethodImpl,slotsig) m
                                 if useMethodImpl then
                                     yield methodImplGenerator (ilThisTy,memberMethodTypars)

                             | _ -> () ]
        
        // Try to add a DefaultMemberAttribute for the 'Item' property
        let defaultMemberAttrs = 
            // REVIEW: this should be based off tcaug_adhoc_list, which is in declaration order
            tycon.MembersOfFSharpTyconSorted
            |> List.tryPick (fun vref -> 
                let name = vref.DisplayName
                match vref.MemberInfo with 
                | None -> None
                | Some memberInfo -> 
                    match name, memberInfo.MemberFlags.MemberKind with 
                    | ("Item" | "op_IndexedLookup"), (MemberKind.PropertyGet  | MemberKind.PropertySet) when not (isNil (ArgInfosOfPropertyVal cenv.g vref.Deref)) ->
                        Some( mkILCustomAttribute cenv.g.ilg (cenv.g.FindSysILTypeRef "System.Reflection.DefaultMemberAttribute",[cenv.g.ilg.typ_String],[ILAttribElem.String(Some(name))],[]) ) 
                    | _ -> None)
            |> Option.toList

        let tyconRepr = tycon.TypeReprInfo

        // DebugDisplayAttribute gets copied to the subtypes generated as part of DU compilation
        let debugDisplayAttrs,normalAttrs = tycon.Attribs |> List.partition (IsMatchingFSharpAttribute cenv.g cenv.g.attrib_DebuggerDisplayAttribute)
        let securityAttrs,normalAttrs = normalAttrs |> List.partition (fun a -> IsSecurityAttribute cenv.g cenv.amap cenv.casApplied a m)
        let generateDebugDisplayAttribute = not cenv.g.compilingFslib && tycon.IsUnionTycon && isNil debugDisplayAttrs
        let generateDebugProxies = (not (tyconRefEq cenv.g tcref cenv.g.unit_tcr_canon) &&
                                    not (HasFSharpAttribute cenv.g cenv.g.attrib_DebuggerTypeProxyAttribute tycon.Attribs))

        let permissionSets = CreatePermissionSets cenv.g cenv.amap eenv securityAttrs
        let secDecls = if securityAttrs.Length > 0 then (mkILSecurityDecls permissionSets) else (emptyILSecurityDecls)
        
        let ilDebugDisplayAttributes = 
            [ yield! GenAttrs cenv eenv debugDisplayAttrs
              if generateDebugDisplayAttribute then 
                  yield cenv.g.mkDebuggerDisplayAttribute ("{" + debugDisplayMethodName + "(),nq}")  ]


        let ilCustomAttrs = 
          [ yield! defaultMemberAttrs 
            yield! normalAttrs 
                      |> List.filter (IsMatchingFSharpAttribute cenv.g cenv.g.attrib_StructLayoutAttribute >> not) 
                      |> GenAttrs cenv eenv
            yield! ilDebugDisplayAttributes  ]

        let reprAccess = ComputeMemberAccess hiddenRepr


        let ilTypeDefKind = 
           match  tyconRepr with 
           | TFSharpObjectRepr o -> 
               match o.fsobjmodel_kind with 
               | TTyconClass      -> ILTypeDefKind.Class
               | TTyconStruct     -> ILTypeDefKind.ValueType
               | TTyconInterface  -> ILTypeDefKind.Interface
               | TTyconEnum       -> ILTypeDefKind.Enum 
               | TTyconDelegate _ -> ILTypeDefKind.Delegate 
           | TRecdRepr _ | TUnionRepr _ when tycon.IsStructOrEnumTycon -> ILTypeDefKind.ValueType
           | _ -> ILTypeDefKind.Class

        let requiresExtraField = 
            let isEmptyStruct = 
                (match ilTypeDefKind with ILTypeDefKind.ValueType -> true | _ -> false) &&
                // All structs are sequential by default 
                // Structs with no instance fields get size 1, pack 0
                tycon.AllFieldsAsList |> List.forall (fun f -> f.IsStatic)

            isEmptyStruct && cenv.opts.workAroundReflectionEmitBugs && not tycon.TyparsNoRange.IsEmpty
        
        // Compute a bunch of useful things for each field 
        let isCLIMutable = (TryFindFSharpBoolAttribute  cenv.g cenv.g.attrib_CLIMutableAttribute tycon.Attribs = Some true) 
        let fieldSummaries = 

             [ for fspec in tycon.AllFieldsAsList do

                   let useGenuineField = useGenuineField tycon fspec
                   
                   // The property (or genuine IL field) is hidden in these circumstances:  
                   //     - secret fields apart from "__value" fields for enums 
                   //     - the representation of the type is hidden 
                   //     - the F# field is hidden by a signature or private declaration 
                   let isPropHidden = 
                       ((fspec.IsCompilerGenerated && not tycon.IsEnumTycon) ||
                        hiddenRepr ||
                        IsHiddenRecdField eenv.sigToImplRemapInfo (tcref.MakeNestedRecdFieldRef fspec))
                   let ilType = GenType cenv.amap m eenvinner.tyenv fspec.FormalType
                   let ilFieldName = ComputeFieldName tycon fspec
                        
                   yield (useGenuineField, ilFieldName, fspec.IsMutable, fspec.IsStatic, fspec.PropertyAttribs, ilType, isPropHidden, fspec) ]
                    
        // Generate the IL fields 
        let ilFieldDefs = 
             [ for (useGenuineField,ilFieldName,isFSharpMutable,isStatic,_,ilPropType,isPropHidden,fspec) in fieldSummaries do

                  let ilFieldOffset = 
                     match TryFindFSharpAttribute cenv.g cenv.g.attrib_FieldOffsetAttribute fspec.FieldAttribs with
                     | Some (Attrib(_,_,[ AttribInt32Arg(fieldOffset) ],_,_,_,_))  -> 
                         Some fieldOffset
                     | Some (Attrib(_,_,_,_,_,_,m))  -> 
                         errorR(Error(FSComp.SR.ilFieldOffsetAttributeCouldNotBeDecoded(),m))
                         None
                     | _ -> 
                         None

                  let attribs = 
                      [ // If using a field then all the attributes go on the field
                        // See also FSharp 1.0 Bug 4727: once we start compiling them as real mutable fields, you should not be able to target both "property" for "val mutable" fields in classes

                        if useGenuineField then yield! fspec.PropertyAttribs 
                        yield! fspec.FieldAttribs ]

                            
                  let ilNotSerialized = HasFSharpAttributeOpt cenv.g cenv.g.attrib_NonSerializedAttribute attribs
                  
                  let fattribs = 
                      attribs
                      // Do not generate FieldOffset as a true CLI custom attribute, since it is implied by other corresponding CLI metadata 
                      |> List.filter (IsMatchingFSharpAttribute cenv.g cenv.g.attrib_FieldOffsetAttribute >> not) 
                      // Do not generate NonSerialized as a true CLI custom attribute, since it is implied by other corresponding CLI metadata 
                      |> List.filter (IsMatchingFSharpAttributeOpt cenv.g cenv.g.attrib_NonSerializedAttribute >> not) 

                  let ilFieldMarshal, fattribs = GenMarshal cenv fattribs

                  // The IL field is hidden if the property/field is hidden OR we're using a property AND the field is not mutable (because we can take the address of a mutable field). 
                  // Otherwise fields are always accessed via their property getters/setters 
                  let isFieldHidden = isPropHidden || (not useGenuineField && not isFSharpMutable)
                  
                  let extraAttribs = 
                     match tyconRepr with 
                     | TRecdRepr _ when not useGenuineField -> [ cenv.g.DebuggerBrowsableNeverAttribute ] // hide fields in records in debug display
                     | _ -> [] // don't hide fields in classes in debug display

                  yield
                      { Name          = ilFieldName
                        Type          = ilPropType
                        IsStatic      = isStatic
                        Access        = ComputeMemberAccess isFieldHidden
                        Data          = None 
                        LiteralValue  = Option.map (GenFieldInit m) fspec.LiteralValue
                        Offset        = ilFieldOffset
                        IsSpecialName = (ilFieldName="value__" && tycon.IsEnumTycon)
                        Marshal       = ilFieldMarshal
                        NotSerialized = ilNotSerialized 
                        IsInitOnly    = false  
                        IsLiteral     = fspec.LiteralValue.IsSome 
                        CustomAttrs   = mkILCustomAttrs (GenAttrs cenv eenv fattribs @ extraAttribs) } 

               if requiresExtraField then 
                   yield mkILInstanceField("__dummy",cenv.g.ilg.typ_Int32,None,ILMemberAccess.Assembly) ]
         
        // Generate property definitions for the fields compiled as properties 
        let ilPropertyDefsForFields = 
             [ for (i, (useGenuineField,_,isFSharpMutable,isStatic,propAttribs,ilPropType,_,fspec)) in markup fieldSummaries do
                 if not useGenuineField then 
                     let ilCallingConv = if isStatic then ILCallingConv.Static else ILCallingConv.Instance
                     let ilPropName = fspec.Name
                     let ilHasSetter = isCLIMutable || isFSharpMutable
                     let ilFieldAttrs = GenAttrs cenv eenv propAttribs @ [mkCompilationMappingAttrWithSeqNum cenv.g (int SourceConstructFlags.Field) i]
                     yield
                       { Name            = ilPropName
                         IsRTSpecialName = false
                         IsSpecialName   = false
                         SetMethod       = (if ilHasSetter then Some(mkILMethRef(tref,ilCallingConv,"set_" + ilPropName,0,[ilPropType],ILType.Void)) else None)
                         GetMethod       = Some(mkILMethRef(tref,ilCallingConv,"get_" + ilPropName,0,[],ilPropType))
                         CallingConv     = ilCallingConv.ThisConv
                         Type            = ilPropType          
                         Init            = None
                         Args            = []
                         CustomAttrs     = mkILCustomAttrs ilFieldAttrs } ] 
         
        let methodDefs = 
            [ // Generate property getter methods for those fields that have properties 
              for (useGenuineField,ilFieldName,_,isStatic,_,ilPropType,isPropHidden,fspec) in fieldSummaries do
                if not useGenuineField then 
                    let ilPropName = fspec.Name
                    let ilMethName = "get_" + ilPropName
                    let access = ComputeMemberAccess isPropHidden
                    yield mkLdfldMethodDef (ilMethName,access,isStatic,ilThisTy,ilFieldName,ilPropType) 

              // Generate property setter methods for the mutable fields 
              for (useGenuineField,ilFieldName,isFSharpMutable,isStatic,_,ilPropType,isPropHidden,fspec) in fieldSummaries do
                let ilHasSetter = (isCLIMutable || isFSharpMutable) && not useGenuineField 
                if ilHasSetter then 
                    let ilPropName = fspec.Name
                    let ilFieldSpec = mkILFieldSpecInTy(ilThisTy,ilFieldName,ilPropType)
                    let ilMethName = "set_" + ilPropName
                    let ilParams = [mkILParamNamed("value",ilPropType)]
                    let ilReturn = mkILReturn ILType.Void
                    let iLAccess = ComputeMemberAccess isPropHidden
                    let ilMethodDef = 
                         if isStatic then 
                             mkILNonGenericStaticMethod
                               (ilMethName,iLAccess,ilParams,ilReturn,
                                  mkMethodBody(true,[],2,nonBranchingInstrsToCode ([ mkLdarg0;mkNormalStsfld ilFieldSpec]),None))
                         else 
                             mkILNonGenericInstanceMethod
                               (ilMethName,iLAccess,ilParams,ilReturn,
                                  mkMethodBody(true,[],2,nonBranchingInstrsToCode ([ mkLdarg0;mkLdarg 1us;mkNormalStfld ilFieldSpec]),None))
                    yield ilMethodDef |> AddSpecialNameFlag 

              if generateDebugDisplayAttribute then 
                  let (|Lazy|) (x:Lazy<_>) = x.Force()
                  match (eenv.valsInScope.TryFind cenv.g.sprintf_vref.Deref,
                         eenv.valsInScope.TryFind cenv.g.new_format_vref.Deref) with
                  | Some(Lazy(Method(_,_,sprintfMethSpec,_,_,_))), Some(Lazy(Method(_,_,newFormatMethSpec,_,_,_))) ->
                      // The type returned by the 'sprintf' call
                      let funcTy = EraseClosures.mkILFuncTy cenv.g.ilxPubCloEnv ilThisTy cenv.g.ilg.typ_String
                      // Give the instantiation of the printf format object, i.e. a Format`5 object compatible with StringFormat<ilThisTy>
                      let newFormatMethSpec = mkILMethSpec(newFormatMethSpec.MethodRef,AsObject,
                                                      [// 'T -> string'
                                                       funcTy 
                                                       // rest follow from 'StringFormat<T>'
                                                       GenUnitTy cenv eenv m  
                                                       cenv.g.ilg.typ_String 
                                                       cenv.g.ilg.typ_String 
                                                       cenv.g.ilg.typ_String],[])
                      // Instantiate with our own type
                      let sprintfMethSpec = mkILMethSpec(sprintfMethSpec.MethodRef,AsObject,[],[funcTy])
                      // Here's the body of the method. Call printf, then invoke the function it returns
                      let callInstrs = EraseClosures.mkCallFunc cenv.g.ilxPubCloEnv (fun _ -> 0us) eenv.tyenv.Count Normalcall (Apps_app(ilThisTy, Apps_done cenv.g.ilg.typ_String))
                      let ilMethodDef = mkILNonGenericInstanceMethod (debugDisplayMethodName,ILMemberAccess.Assembly,[],
                                                   mkILReturn cenv.g.ilg.typ_Object,
                                                   mkMethodBody 
                                                         (true,[],2,
                                                          nonBranchingInstrsToCode 
                                                            ([ // load the hardwired format string
                                                               yield I_ldstr "%+0.8A"  
                                                               // make the printf format object
                                                               yield mkNormalNewobj newFormatMethSpec
                                                               // call sprintf
                                                               yield mkNormalCall sprintfMethSpec 
                                                               // call the function returned by sprintf
                                                               yield mkLdarg0 
                                                               if ilThisTy.Boxity = ILBoxity.AsValue then
                                                                  yield mkNormalLdobj ilThisTy  ] @
                                                             callInstrs),
                                                          None))
                      yield ilMethodDef |> AddSpecialNameFlag |> AddNonUserCompilerGeneratedAttribs cenv.g
                  | None,_ ->
                      //printfn "sprintf not found"
                      ()
                  | _,None ->
                      //printfn "new formatnot found"
                      ()
                  | _ ->
                      //printfn "neither found, or non-method"
                      ()

              // Build record constructors and the funky methods that go with records and delegate types. 
              // Constructors and delegate methods have the same access as the representation 
              match tyconRepr with 
              | TRecdRepr _ when not (tycon.IsEnumTycon) ->
                 // No constructor for enum types 
                 // Otherwise find all the non-static, non zero-init fields and build a constructor 
                 let relevantFields = 
                     fieldSummaries 
                     |> List.filter (fun (_,_,_,isStatic,_,_,_,fspec) -> not isStatic && not fspec.IsZeroInit)

                 let fieldNamesAndTypes = 
                     relevantFields
                     |> List.map (fun (_,ilFieldName,_,_,_,ilPropType,_,fspec) -> (fspec.Name,ilFieldName,ilPropType))

                 let isStructRecord = tycon.IsStructRecordOrUnionTycon

                 // No type spec if the record is a value type
                 let spec = if isStructRecord then None else Some(cenv.g.ilg.typ_Object.TypeSpec)
                 let ilMethodDef = mkILSimpleStorageCtorWithParamNames(None, spec, ilThisTy, [], ChooseParamNames fieldNamesAndTypes, reprAccess)

                 yield ilMethodDef 
                 // FSharp 1.0 bug 1988: Explicitly setting the ComVisible(true)  attribute on an F# type causes an F# record to be emitted in a way that enables mutation for COM interop scenarios
                 // FSharp 3.0 feature: adding CLIMutable to a record type causes emit of default constructor, and all fields get property setters
                 // Records that are value types do not create a default constructor with CLIMutable or ComVisible
                 if not isStructRecord && (isCLIMutable || (TryFindFSharpBoolAttribute cenv.g cenv.g.attrib_ComVisibleAttribute tycon.Attribs = Some true)) then
                     yield mkILSimpleStorageCtor(None, Some cenv.g.ilg.typ_Object.TypeSpec, ilThisTy, [], [], reprAccess) 
                 
                 if not (tycon.HasMember cenv.g "ToString" []) then
                    yield! genToString ilThisTy
              | TFSharpObjectRepr r when tycon.IsFSharpDelegateTycon ->

                 // Build all the methods that go with a delegate type 
                 match r.fsobjmodel_kind with 
                 | TTyconDelegate ss ->
                     let p,r = 
                         // When "type delegateTy = delegate of unit -> returnTy",
                         // suppress the unit arg from delegate .Invoke vslot. 
                         let (TSlotSig(nm,typ,ctps,mtps,paraml,returnTy)) = ss
                         let paraml = 
                             match paraml with
                             | [[tsp]] when isUnitTy cenv.g tsp.Type -> [] (* suppress unit arg *)
                             | paraml -> paraml
                         GenActualSlotsig m cenv eenvinner (TSlotSig(nm,typ,ctps,mtps,paraml,returnTy)) []
                     for ilMethodDef in mkILDelegateMethods cenv.g.ilg (cenv.g.iltyp_AsyncCallback, cenv.g.iltyp_IAsyncResult) (p,r) do
                        yield { ilMethodDef with Access=reprAccess }
                 | _ -> 
                     ()
              | TUnionRepr _ when not (tycon.HasMember cenv.g "ToString" []) -> 
                  yield! genToString ilThisTy
              | _ -> () ]
              
        let ilMethods = methodDefs @ augmentOverrideMethodDefs @ abstractMethodDefs
        let ilProperties = mkILProperties (ilPropertyDefsForFields @ abstractPropDefs)
        let ilEvents = mkILEvents abstractEventDefs
        let ilFields = mkILFields ilFieldDefs
        
        let tdef, tdefDiscards = 
           let isSerializable = (TryFindFSharpBoolAttribute cenv.g cenv.g.attrib_AutoSerializableAttribute tycon.Attribs <> Some(false)) 
                             && cenv.g.attrib_SerializableAttribute.IsSome
                                       
           match tycon.TypeReprInfo with 
           | TILObjectRepr _ ->
               let td = tycon.ILTyconRawMetadata
               {td with Access = access
                        CustomAttrs = mkILCustomAttrs ilCustomAttrs
                        GenericParams = ilGenParams }, None

           | TRecdRepr _ | TFSharpObjectRepr _ as tyconRepr  ->
               let super = superOfTycon cenv.g tycon
               let ilBaseTy = GenType cenv.amap m eenvinner.tyenv super
               
               // Build a basic type definition 
               let isObjectType = (match tyconRepr with TFSharpObjectRepr _ -> true | _ -> false)
               let ilAttrs = 
                   ilCustomAttrs @ 
                   [mkCompilationMappingAttr cenv.g
                       (int (if isObjectType
                             then SourceConstructFlags.ObjectType
                             elif hiddenRepr then SourceConstructFlags.RecordType ||| SourceConstructFlags.NonPublicRepresentation
                             else SourceConstructFlags.RecordType)) ]
                                
               // For now, generic types always use ILTypeInit.BeforeField. This is because 
               // there appear to be some cases where ILTypeInit.OnAny causes problems for 
               // the .NET CLR when used in conjunction with generic classes in cross-DLL
               // and NGEN scenarios. 
               //
               // We don't apply this rule to the final file. This is because ALL classes with .cctors in
               // the final file (which may in turn trigger the .cctor for the .EXE itself, which 
               // in turn calls the main() method) must have deterministic initialization 
               // that is not triggered prior to execution of the main() method.
               // If this property doesn't hold then the .cctor can end up running 
               // before the main method even starts.
               let typeDefTrigger = 
                   if eenv.isFinalFile || tycon.TyparsNoRange.IsEmpty then 
                       ILTypeInit.OnAny 
                   else 
                       ILTypeInit.BeforeField

               let tdef = mkILGenericClass (ilTypeName, access, ilGenParams, ilBaseTy, ilIntfTys, 
                                            mkILMethods ilMethods, ilFields, emptyILTypeDefs, ilProperties, ilEvents, mkILCustomAttrs ilAttrs, 
                                            typeDefTrigger)

               // Set some the extra entries in the definition 
               let isTheSealedAttribute = tyconRefEq cenv.g tcref cenv.g.attrib_SealedAttribute.TyconRef

               let tdef = { tdef with  IsSealed = isSealedTy cenv.g thisTy || isTheSealedAttribute
                                       IsSerializable = isSerializable
                                       MethodImpls=mkILMethodImpls methodImpls 
                                       IsAbstract=isAbstract
                                       IsComInterop=isComInteropTy cenv.g thisTy }

               let tdLayout,tdEncoding = 
                    match TryFindFSharpAttribute cenv.g cenv.g.attrib_StructLayoutAttribute tycon.Attribs with
                    | Some (Attrib(_,_,[ AttribInt32Arg(layoutKind) ],namedArgs,_,_,_))  -> 
                        let decoder = AttributeDecoder namedArgs
                        let ilPack = decoder.FindInt32 "Pack" 0x0
                        let ilSize = decoder.FindInt32 "Size" 0x0
                        let tdEncoding = 
                            match (decoder.FindInt32 "CharSet" 0x0) with
                            (* enumeration values for System.Runtime.InteropServices.CharSet taken from mscorlib.il *)
                            | 0x03 -> ILDefaultPInvokeEncoding.Unicode
                            | 0x04 -> ILDefaultPInvokeEncoding.Auto
                            | _ -> ILDefaultPInvokeEncoding.Ansi
                        let layoutInfo = 
                            if ilPack = 0x0 && ilSize = 0x0 
                            then { Size=None; Pack=None } 
                            else { Size = Some ilSize; Pack = Some (uint16 ilPack) }
                        let tdLayout = 
                          match layoutKind with
                          (* enumeration values for System.Runtime.InteropServices.LayoutKind taken from mscorlib.il *)
                          | 0x0 -> ILTypeDefLayout.Sequential layoutInfo
                          | 0x2 -> ILTypeDefLayout.Explicit layoutInfo
                          | _ -> ILTypeDefLayout.Auto
                        tdLayout,tdEncoding
                    | Some (Attrib(_,_,_,_,_,_,m))  -> 
                        errorR(Error(FSComp.SR.ilStructLayoutAttributeCouldNotBeDecoded(),m))
                        ILTypeDefLayout.Auto, ILDefaultPInvokeEncoding.Ansi

                    | _ when (match ilTypeDefKind with ILTypeDefKind.ValueType -> true | _ -> false) ->
                        
                        // All structs are sequential by default 
                        // Structs with no instance fields get size 1, pack 0
                        if tycon.AllFieldsAsList |> List.exists (fun f -> not f.IsStatic) ||
                            // Reflection emit doesn't let us emit 'pack' and 'size' for generic structs.
                            // In that case we generate a dummy field instead
                           (cenv.opts.workAroundReflectionEmitBugs && not tycon.TyparsNoRange.IsEmpty) 
                           then 
                            ILTypeDefLayout.Sequential { Size=None; Pack=None }, ILDefaultPInvokeEncoding.Ansi 
                        else
                            ILTypeDefLayout.Sequential { Size=Some 1; Pack=Some 0us }, ILDefaultPInvokeEncoding.Ansi 
                        
                    | _ -> 
                        ILTypeDefLayout.Auto, ILDefaultPInvokeEncoding.Ansi
               
               // if the type's layout is Explicit, ensure that each field has a valid offset
               let validateExplicit fdef =
                    match fdef.Offset with
                    // Remove field suffix "@" for pretty printing
                    | None -> errorR(Error(FSComp.SR.ilFieldDoesNotHaveValidOffsetForStructureLayout(tdef.Name, fdef.Name.Replace("@","")), (trimRangeToLine m)))
                    | _ -> ()
               
               // if the type's layout is Sequential, no offsets should be applied
               let validateSequential fdef =
                    match fdef.Offset with
                    | Some _ -> errorR(Error(FSComp.SR.ilFieldHasOffsetForSequentialLayout(), (trimRangeToLine m)))
                    | _ -> ()
                    
               match tdLayout with
               | ILTypeDefLayout.Explicit(_) -> List.iter validateExplicit ilFieldDefs
               | ILTypeDefLayout.Sequential(_) -> List.iter validateSequential ilFieldDefs                     
               | _ -> ()
               
               let tdef = { tdef with tdKind =  ilTypeDefKind; Layout=tdLayout; Encoding=tdEncoding }
               let tdef = match ilTypeDefKind with ILTypeDefKind.Interface -> { tdef with Extends = None; IsAbstract=true } | _ -> tdef
               tdef, None

           | TUnionRepr _ -> 
               let alternatives = 
                   tycon.UnionCasesArray |> Array.mapi (fun i ucspec -> 
                       { altName=ucspec.CompiledName
                         altFields=GenUnionCaseRef cenv.amap m eenvinner.tyenv i ucspec.RecdFieldsArray
                         altCustomAttrs= mkILCustomAttrs (GenAttrs cenv eenv ucspec.Attribs @ [mkCompilationMappingAttrWithSeqNum cenv.g (int SourceConstructFlags.UnionCase) i]) })
               let cuinfo =
                  { cudReprAccess=reprAccess
                    cudNullPermitted=IsUnionTypeWithNullAsTrueValue cenv.g tycon
                    cudHelpersAccess=reprAccess
                    cudHasHelpers=ComputeUnionHasHelpers cenv.g tcref
                    cudDebugProxies= generateDebugProxies
                    cudDebugDisplayAttributes= ilDebugDisplayAttributes
                    cudAlternatives= alternatives
                    cudWhere = None}
               let layout = 
                   if isStructTy cenv.g thisTy then 
                       ILTypeDefLayout.Sequential { Size=None; Pack=None } 
                   else 
                       ILTypeDefLayout.Auto
               let tdef = 
                   { Name = ilTypeName
                     Layout =  layout
                     Access = access
                     GenericParams = ilGenParams
                     CustomAttrs = 
                         mkILCustomAttrs (ilCustomAttrs @ 
                                          [mkCompilationMappingAttr cenv.g
                                              (int (if hiddenRepr
                                                    then SourceConstructFlags.SumType ||| SourceConstructFlags.NonPublicRepresentation 
                                                    else SourceConstructFlags.SumType)) ])
                     InitSemantics=ILTypeInit.BeforeField      
                     IsSealed=true
                     IsAbstract=false
                     tdKind= (if tycon.IsStructOrEnumTycon then ILTypeDefKind.ValueType else ILTypeDefKind.Class)
                     Fields = ilFields
                     Events= ilEvents
                     Properties = ilProperties
                     Methods= mkILMethods ilMethods 
                     MethodImpls= mkILMethodImpls methodImpls 
                     IsComInterop=false    
                     IsSerializable= isSerializable 
                     IsSpecialName= false
                     NestedTypes=emptyILTypeDefs
                     Encoding= ILDefaultPInvokeEncoding.Auto
                     Implements = ilIntfTys
                     Extends= Some (if tycon.IsStructOrEnumTycon then cenv.g.iltyp_ValueType else cenv.g.ilg.typ_Object)
                     SecurityDecls= emptyILSecurityDecls
                     HasSecurity=false }
               let tdef2 = cenv.g.eraseClassUnionDef tref tdef cuinfo
   
               // Discard the user-supplied (i.e. prim-type.fs) implementations of the get_Empty, get_IsEmpty, get_Value and get_None and Some methods. 
               // This is because we will replace their implementations by ones that load the unique 
               // private static field for lists etc.
               // 
               // Also discard the F#-compiler supplied implementation of the Empty, IsEmpty, Value and None properties.
               let tdefDiscards = 
                  Some ((fun (md: ILMethodDef) ->
                            (cuinfo.cudHasHelpers = SpecialFSharpListHelpers && (md.Name = "get_Empty" || md.Name = "Cons" || md.Name = "get_IsEmpty")) ||
                            (cuinfo.cudHasHelpers = SpecialFSharpOptionHelpers && (md.Name = "get_Value" || md.Name = "get_None" || md.Name = "Some"))),
    
                        (fun (pd: ILPropertyDef) ->
                            (cuinfo.cudHasHelpers = SpecialFSharpListHelpers && (pd.Name = "Empty"  || pd.Name = "IsEmpty"  )) ||
                            (cuinfo.cudHasHelpers = SpecialFSharpOptionHelpers && (pd.Name = "Value" || pd.Name = "None"))))

               tdef2, tdefDiscards

           | _ -> failwith "??"

        let tdef = {tdef with SecurityDecls= secDecls; HasSecurity=securityAttrs.Length > 0}
        mgbuf.AddTypeDef(tref, tdef, false, false, tdefDiscards)

        // If a non-generic type is written with "static let" and "static do" (i.e. it has a ".cctor")
        // then the code for the .cctor is placed into .cctor for the backing static class for the file.
        // It is not placed in its own .cctor as there is no feasible way for this to be given a coherent 
        // order in the sequential initialization of the file.
        //
        // In this case, the .cctor for this type must force the .cctor of the backing static class for the file.
        if tycon.TyparsNoRange.IsEmpty && tycon.MembersOfFSharpTyconSorted |> List.exists (fun vref -> vref.Deref.IsClassConstructor) then
          GenForceWholeFileInitializationAsPartOfCCtor cenv mgbuf lazyInitInfo tref m


        
/// Generate the type for an F# exception declaration. 
and GenExnDef cenv mgbuf eenv m (exnc:Tycon) =
    let exncref  = mkLocalEntityRef exnc
    match exnc.ExceptionInfo with 
    | TExnAbbrevRepr _ | TExnAsmRepr _ | TExnNone -> ()
    | TExnFresh _ ->
        let ilThisTy = GenExnType cenv.amap m eenv.tyenv exncref
        let tref = ilThisTy.TypeRef
        let isHidden = IsHiddenTycon eenv.sigToImplRemapInfo exnc
        let access = ComputeTypeAccess tref isHidden
        let reprAccess = ComputeMemberAccess isHidden
        let fspecs = exnc.TrueInstanceFieldsAsList 

        let ilMethodDefsForProperties,ilFieldDefs,ilPropertyDefs,fieldNamesAndTypes = 
            [ for i,fld in markup fspecs do 
               let ilPropName = fld.Name
               let ilPropType = GenType cenv.amap m eenv.tyenv fld.FormalType
               let ilMethName = "get_" + fld.Name
               let ilFieldName = ComputeFieldName exnc fld 
               let ilMethodDef = mkLdfldMethodDef (ilMethName,reprAccess,false,ilThisTy,ilFieldName,ilPropType)
               let ilFieldDef = IL.mkILInstanceField(ilFieldName,ilPropType, None, ILMemberAccess.Assembly)
               let ilPropDef = 
                     { Name=ilPropName
                       IsRTSpecialName=false
                       IsSpecialName=false
                       SetMethod=None
                       GetMethod=Some(mkILMethRef(tref,ILCallingConv.Instance,ilMethName,0,[],ilPropType))
                       CallingConv=ILThisConvention.Instance
                       Type=ilPropType          
                       Init=None
                       Args = []
                       CustomAttrs=mkILCustomAttrs (GenAttrs cenv eenv fld.PropertyAttribs @ [mkCompilationMappingAttrWithSeqNum cenv.g (int SourceConstructFlags.Field) i]) }
               yield (ilMethodDef,ilFieldDef,ilPropDef,(ilPropName,ilFieldName,ilPropType)) ] 
             |> List.unzip4

        let ilCtorDef = 
            mkILSimpleStorageCtorWithParamNames(None, Some cenv.g.iltyp_Exception.TypeSpec, ilThisTy, [], ChooseParamNames fieldNamesAndTypes, reprAccess) 

        // In compiled code, all exception types get a parameterless constructor for use with XML serialization
        // This does default-initialization of all fields
        let ilCtorDefNoArgs = 
            if not (isNil fieldNamesAndTypes) then 
                [ mkILSimpleStorageCtor(None, Some cenv.g.iltyp_Exception.TypeSpec, ilThisTy, [], [], reprAccess) ]
            else
                []

        
        let serializationRelatedMembers =
            // do not emit serialization related members if target framework lacks SerializableAttribute or SerializationInfo
          match cenv.g.attrib_SerializableAttribute, cenv.g.iltyp_SerializationInfo, cenv.g.iltyp_StreamingContext with 
          | Some _,  Some serializationInfoType, Some streamingContextType -> 
            let ilCtorDefForSerialziation = 
                mkILCtor(ILMemberAccess.Family,
                        [mkILParamNamed("info", serializationInfoType);mkILParamNamed("context",streamingContextType)],
                        mkMethodBody
                          (false,[],8,
                           nonBranchingInstrsToCode
                              [ mkLdarg0 
                                mkLdarg 1us
                                mkLdarg 2us
                                mkNormalCall (mkILCtorMethSpecForTy (cenv.g.iltyp_Exception,[serializationInfoType; streamingContextType])) ]
                           ,None))
                
//#if BE_SECURITY_TRANSPARENT
            [ilCtorDefForSerialziation]
//#else
(*
            let getObjectDataMethodForSerialization = 
            
                let ilMethodDef = 
                    mkILNonGenericVirtualMethod
                        ("GetObjectData",ILMemberAccess.Public,
                         [mkILParamNamed ("info", serializationInfoType);mkILParamNamed("context",cenv.g.iltyp_StreamingContext)], 
                         mkILReturn ILType.Void,
                         (let code = 
                            nonBranchingInstrsToCode
                              [ mkLdarg0 
                                mkLdarg 1us
                                mkLdarg 2us
                                mkNormalCall (mkILNonGenericInstanceMethSpecInTy (cenv.g.iltyp_Exception, "GetObjectData", [serializationInfoType; cenv.g.iltyp_StreamingContext], ILType.Void))
                              ]
                          mkMethodBody(true,[],8,code,None)))
                // Here we must encode: [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
                // In ILDASM this is: .permissionset demand = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SerializationFormatter' = bool(true)}}
                match cenv.g.tref_SecurityPermissionAttribute with
                | None -> ilMethodDef
                | Some securityPermissionAttributeType ->
                    { ilMethodDef with 
                           SecurityDecls=mkILSecurityDecls [ IL.mkPermissionSet cenv.g.ilg (ILSecurityAction.Demand,[(securityPermissionAttributeType, [("SerializationFormatter",cenv.g.ilg.typ_Bool, ILAttribElem.Bool(true))])])]
                           HasSecurity=true }
            [ilCtorDefForSerialziation; getObjectDataMethodForSerialization]
*)
//#endif                
          | _ -> []

        let ilTypeName = tref.Name
        
        let interfaces =  exnc.ImmediateInterfaceTypesOfFSharpTycon |> List.map (GenType cenv.amap m eenv.tyenv) 
        let tdef = 
          mkILGenericClass
            (ilTypeName,access,[],cenv.g.iltyp_Exception, 
             interfaces,  
             mkILMethods ([ilCtorDef] @ ilCtorDefNoArgs @ serializationRelatedMembers @ ilMethodDefsForProperties),
             mkILFields ilFieldDefs,
             emptyILTypeDefs, 
             mkILProperties ilPropertyDefs,
             emptyILEvents,
             mkILCustomAttrs [mkCompilationMappingAttr cenv.g (int SourceConstructFlags.Exception)],
             ILTypeInit.BeforeField)
        let tdef = { tdef with IsSerializable = cenv.g.attrib_SerializableAttribute.IsSome }
        mgbuf.AddTypeDef(tref, tdef, false, false, None)


let CodegenAssembly cenv eenv mgbuf fileImpls = 
    if not (isNil fileImpls) then 
      let a,b = List.frontAndBack fileImpls
      let eenv = List.fold (GenTopImpl cenv mgbuf None) eenv a
      let _eenv = GenTopImpl cenv mgbuf cenv.opts.mainMethodInfo eenv b
      mgbuf.AddInitializeScriptsInOrderToEntryPoint()

//-------------------------------------------------------------------------
// When generating a module we just write into mutable 
// structures representing the contents of the module. 
//------------------------------------------------------------------------- 

let GetEmptyIlxGenEnv (ilg : ILGlobals) ccu = 
    let thisCompLoc = CompLocForCcu ccu
    { tyenv=TypeReprEnv.Empty
      cloc = thisCompLoc
      valsInScope=ValMap<_>.Empty 
      someTypeInThisAssembly=ilg.typ_Object (* dummy value *)
      isFinalFile = false
      letBoundVars=[]
      liveLocals=IntMap.empty()
      innerVals = []
      sigToImplRemapInfo = [] (* "module remap info" *)
      withinSEH = false }

type IlxGenResults = 
    { ilTypeDefs: ILTypeDef list
      ilAssemAttrs : ILAttribute list
      ilNetModuleAttrs: ILAttribute list
      quotationResourceInfo: (ILTypeRef list * byte[]) list }


let GenerateCode (cenv, eenv, TypedAssemblyAfterOptimization fileImpls, assemAttribs, moduleAttribs) =

    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.IlxGen

    // Generate the implementations into the mgbuf 
    let mgbuf= new AssemblyBuilder(cenv)
    let eenv = { eenv with cloc = CompLocForFragment cenv.opts.fragName cenv.viewCcu }
    
    // Generate the PrivateImplementationDetails type
    GenTypeDefForCompLoc (cenv, eenv, mgbuf, CompLocForPrivateImplementationDetails eenv.cloc, useHiddenInitCode, [], ILTypeInit.BeforeField, true, (* atEnd= *) true)

    // Generate the whole assembly
    CodegenAssembly cenv eenv mgbuf fileImpls

    let ilAssemAttrs = GenAttrs cenv eenv assemAttribs
    
    let tdefs,reflectedDefinitions = mgbuf.Close()

    // Generate the quotations
    let quotationResourceInfo = 
        match reflectedDefinitions with 
        | [] -> []
        | _ -> 
            let qscope = QuotationTranslator.QuotationGenerationScope.Create (cenv.g, cenv.amap, cenv.viewCcu, QuotationTranslator.IsReflectedDefinition.Yes)
            let defns = 
              reflectedDefinitions |> List.choose (fun ((methName, v),e) -> 
                    try 
                      let ety = tyOfExpr cenv.g e
                      let tps,taue,_ = 
                        match e with 
                        | Expr.TyLambda (_,tps,b,_,_) -> tps,b,applyForallTy cenv.g ety (List.map mkTyparTy tps)
                        | _ -> [],e,ety
                      let env = QuotationTranslator.QuotationTranslationEnv.Empty.BindTypars tps
                      let astExpr = QuotationTranslator.ConvExprPublic qscope env taue
                      let mbaseR = QuotationTranslator.ConvMethodBase qscope env (methName, v)
                      
                      Some(mbaseR,astExpr) 
                    with 
                    | QuotationTranslator.InvalidQuotedTerm e -> warning(e); None)

            let referencedTypeDefs, freeTypes, spliceArgExprs = qscope.Close()

            for (_freeType, m) in freeTypes do 
                error(InternalError("A free type variable was detected in a reflected definition",m))

            for (_spliceArgExpr, m) in spliceArgExprs do 
                error(Error(FSComp.SR.ilReflectedDefinitionsCannotUseSliceOperator(),m))

            let defnsResourceBytes = defns |> QuotationPickler.PickleDefns

            [ (referencedTypeDefs, defnsResourceBytes) ]

    let ilNetModuleAttrs = GenAttrs cenv eenv moduleAttribs

    { ilTypeDefs= tdefs
      ilAssemAttrs = ilAssemAttrs
      ilNetModuleAttrs = ilNetModuleAttrs
      quotationResourceInfo = quotationResourceInfo }
    

//-------------------------------------------------------------------------
// For printing values in fsi we want to lookup the value of given vrefs.
// The storage in the eenv says if the vref is stored in a static field.
// If we know how/where the field was generated, then we can lookup via reflection.
//------------------------------------------------------------------------- 

open System
open System.Reflection

/// The lookup* functions are the conversions available from ilreflect.
type ExecutionContext =
    { LookupFieldRef : (ILFieldRef -> FieldInfo)
      LookupMethodRef : (ILMethodRef -> MethodInfo)
      LookupTypeRef : (ILTypeRef -> Type)
      LookupType : (ILType -> Type) } 

// A helper to generate a default value for any System.Type. I couldn't find a System.Reflection
// method to do this.
let defaultOf = 
    let gminfo = 
       lazy 
          (match <@@ Unchecked.defaultof<int> @@> with 
           | Quotations.Patterns.Call(_,minfo,_) -> minfo.GetGenericMethodDefinition()
           | _ -> failwith "unexpected failure decoding quotation at ilxgen startup")
    fun ty -> gminfo.Value.MakeGenericMethod([| ty |]).Invoke(null,[| |])
    
/// Top-level val bindings are stored (for example) in static fields.
/// In the FSI case, these fields are be created and initialised, so we can recover the object.
/// IlxGen knows how v was stored, and then ilreflect knows how this storage was generated.
/// IlxGen converts (v:Tast.Val) to AbsIL datastructures.
/// Ilreflect converts from AbsIL datastructures to emitted Type, FieldInfo, MethodInfo etc.
let LookupGeneratedValue (amap:ImportMap) (ctxt: ExecutionContext) eenv (v:Val) =
  try
    // Convert the v.Type into a System.Type according to ilxgen and ilreflect.
    let objTyp =
        let ilTy = GenType amap v.Range TypeReprEnv.Empty v.Type (* TypeReprEnv.Empty ok, not expecting typars *)
        ctxt.LookupType ilTy
    // Lookup the compiled v value (as an object).
    match StorageForVal v.Range v eenv with
      | StaticField (fspec, _, hasLiteralAttr, ilContainerTy, _, _, ilGetterMethRef, _, _) ->
          let obj =
              if hasLiteralAttr then
                  let staticTyp = ctxt.LookupTypeRef fspec.EnclosingTypeRef
                  // Checked: This FieldInfo (FieldBuilder) supports GetValue().
                  staticTyp.GetField(fspec.Name).GetValue(null:obj)
              else
                  let staticTyp = ctxt.LookupTypeRef ilContainerTy.TypeRef
                  // We can't call .Invoke on the ILMethodRef's MethodInfo,
                  // because it is the MethodBuilder and that does not support Invoke.
                  // Rather, we look for the getter MethodInfo from the built type and .Invoke on that.
                  let methInfo = staticTyp.GetMethod(ilGetterMethRef.Name, BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
                  methInfo.Invoke((null:obj),(null:obj[]))
          Some (obj,objTyp)

      | StaticProperty (ilGetterMethSpec, _) ->
          let obj =
              let staticTyp = ctxt.LookupTypeRef ilGetterMethSpec.MethodRef.EnclosingTypeRef
              // We can't call .Invoke on the ILMethodRef's MethodInfo,
              // because it is the MethodBuilder and that does not support Invoke.
              // Rather, we look for the getter MethodInfo from the built type and .Invoke on that.
              let methInfo = staticTyp.GetMethod(ilGetterMethSpec.Name, BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
              methInfo.Invoke((null:obj),(null:obj[]))
          Some (obj,objTyp)

      | Null ->
          Some (null,objTyp)
      | Local _ -> None     
      | Method _ -> None
      | Arg _ -> None
      | Env _ -> None
  with
    e ->
#if DEBUG      
      printf "ilxGen.LookupGeneratedValue for v=%s caught exception:\n%A\n\n" v.LogicalName e
#endif  
      None
    
// Invoke the set_Foo method for a declaration with a default/null value. Used to release storage in fsi.exe
let ClearGeneratedValue (ctxt: ExecutionContext) (_g:TcGlobals) eenv (v:Val) =
  try
    match StorageForVal v.Range v eenv with
      | StaticField (fspec, _, hasLiteralAttr, _, _, _, _ilGetterMethRef, ilSetterMethRef, _) ->
          if not hasLiteralAttr && v.IsMutable then 
              let staticTyp = ctxt.LookupTypeRef ilSetterMethRef.EnclosingTypeRef
              let typ = ctxt.LookupType fspec.ActualType

              let methInfo = staticTyp.GetMethod(ilSetterMethRef.Name, BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
              methInfo.Invoke (null, [| defaultOf typ |]) |> ignore
      | _ -> ()
  with
    e ->
#if DEBUG      
      printf "ilxGen.ClearGeneratedValue for v=%s caught exception:\n%A\n\n" v.LogicalName e
#endif  
      ()

(*
let LookupGeneratedInfo (ctxt: ExecutionContext) (g:TcGlobals) eenv (v:Val) =
  try
    match StorageForVal v.Range v eenv with
      | StaticField (fspec, _, hasLiteralAttr, ilContainerTy, _, _, ilGetterMethRef, _, _) ->
          let staticTyp = ctxt.LookupTypeRef ilContainerTy.TypeRef
          if hasLiteralAttr then
              Some (staticTyp.GetField(fspec.Name) :> MemberInfo)
          else
              Some (staticTyp.GetMethod(ilGetterMethRef.Name,[||]) :> MemberInfo)
      | Null -> None
      | Local _ -> None     
      | Method _ -> None
      | Arg _ -> None
      | Env _ -> None
  with
    e ->
#if DEBUG      
      printf "ilxGen.lookupGenertedInfo for v=%s caught exception:\n%A\n\n" v.LogicalName e
#endif  
      None
    
    
*)
    

/// The published API from the ILX code generator
type IlxAssemblyGenerator(amap: ImportMap, tcGlobals: TcGlobals, tcVal : ConstraintSolver.TcValF, ccu: Tast.CcuThunk) = 
    
    // The incremental state held by the ILX code generator
    let mutable ilxGenEnv = GetEmptyIlxGenEnv tcGlobals.ilg ccu
    let intraAssemblyInfo = { StaticFieldInfo = new Dictionary<_,_>(HashIdentity.Structural) }
    let casApplied = new Dictionary<Stamp,bool>()

    /// Register a set of referenced assemblies with the ILX code generator
    member __.AddExternalCcus ccus = 
        ilxGenEnv <- AddExternalCcusToIlxGenEnv amap tcGlobals ilxGenEnv ccus

    /// Register a fragment of the current assembly with the ILX code generator. If 'isIncrementalFragment' is true then the input
    /// is assumed to be a fragment 'typed' into FSI.EXE, otherwise the input is assumed to be the result of a '#load'
    member __.AddIncrementalLocalAssemblyFragment  (isIncrementalFragment, fragName, typedImplFiles) = 
        ilxGenEnv <- AddIncrementalLocalAssemblyFragmentToIlxGenEnv (amap, isIncrementalFragment, tcGlobals, ccu, fragName, intraAssemblyInfo, ilxGenEnv, typedImplFiles)

    /// Generate ILX code for an assembly fragment
    member __.GenerateCode (codeGenOpts, typedAssembly, assemAttribs, moduleAttribs) = 
        let cenv : cenv = 
            { g=tcGlobals
              TcVal = tcVal
              viewCcu = ccu
              ilUnitTy = None
              amap = amap
              casApplied = casApplied
              intraAssemblyInfo = intraAssemblyInfo
              opts = codeGenOpts 
              optimizeDuringCodeGen = (fun x -> x) }
        GenerateCode (cenv, ilxGenEnv, typedAssembly, assemAttribs, moduleAttribs)

    /// Invert the compilation of the given value and clear the storage of the value
    member __.ClearGeneratedValue (ctxt, v) = ClearGeneratedValue ctxt tcGlobals ilxGenEnv v

    /// Invert the compilation of the given value and return its current dynamic value and its compiled System.Type
    member __.LookupGeneratedValue (ctxt, v) = LookupGeneratedValue amap ctxt ilxGenEnv v

    /// Create the CAS permission sets for an assembly fragment
    member __.CreatePermissionSets attribs = CreatePermissionSets tcGlobals amap ilxGenEnv attribs
