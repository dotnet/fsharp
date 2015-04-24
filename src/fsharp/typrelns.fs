// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Primary relations on types and signatures, with the exception of
/// constraint solving and method overload resolution.
module internal Microsoft.FSharp.Compiler.Typrelns

open Internal.Utilities
open System.Text

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Tastops.DebugPrint
open Microsoft.FSharp.Compiler.Env
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.PrettyNaming
open Microsoft.FSharp.Compiler.Infos.AccessibilityLogic
open Microsoft.FSharp.Compiler.Nameres

#if EXTENSIONTYPING
open Microsoft.FSharp.Compiler.ExtensionTyping
#endif

//-------------------------------------------------------------------------
// a :> b without coercion based on finalized (no type variable) types
//------------------------------------------------------------------------- 


// QUERY: This relation is approximate and not part of the language specification. 
//
//  Some appropriate uses: 
//     patcompile.fs: IsDiscrimSubsumedBy (approximate warning for redundancy of 'isinst' patterns)
//     tc.fs: TcRuntimeTypeTest (approximate warning for redundant runtime type tests)
//     tc.fs: TcExnDefnCore (error for bad exception abbreviation)
//     ilxgen.fs: GenCoerce (omit unecessary castclass or isinst instruction)
//
let rec TypeDefinitelySubsumesTypeNoCoercion ndeep g amap m ty1 ty2 = 
  if ndeep > 100 then error(InternalError("recursive class hierarchy (detected in TypeDefinitelySubsumesTypeNoCoercion), ty1 = "^(DebugPrint.showType ty1),m))
  if ty1 === ty2 then true 
  // QUERY : quadratic
  elif typeEquiv g ty1 ty2 then true
  else
    let ty1 = stripTyEqns g ty1
    let ty2 = stripTyEqns g ty2
    match ty1,ty2 with 
    | TType_app (tc1,l1)  ,TType_app (tc2,l2) when tyconRefEq g tc1 tc2  ->  
        List.lengthsEqAndForall2 (typeEquiv g) l1 l2
    | TType_ucase (tc1,l1)  ,TType_ucase (tc2,l2) when g.unionCaseRefEq tc1 tc2  ->  
        List.lengthsEqAndForall2 (typeEquiv g) l1 l2
    | TType_tuple l1    ,TType_tuple l2     -> 
        List.lengthsEqAndForall2 (typeEquiv g) l1 l2 
    | TType_fun (d1,r1)  ,TType_fun (d2,r2)   -> 
        typeEquiv g d1 d2 && typeEquiv g r1 r2
    | TType_measure measure1, TType_measure measure2 ->
        measureEquiv g measure1 measure2
    | _ ->  
        (typeEquiv g ty1 g.obj_ty && isRefTy g ty2) || (* F# reference types are subtypes of type 'obj' *)
        (isAppTy g ty2 &&
         isRefTy g ty2 && 

         ((match GetSuperTypeOfType g amap m ty2 with 
           | None -> false
           | Some ty -> TypeDefinitelySubsumesTypeNoCoercion (ndeep+1) g amap m ty1 ty) ||

           (isInterfaceTy g ty1 &&
            ty2 |> GetImmediateInterfacesOfType SkipUnrefInterfaces.Yes g amap m 
                |> List.exists (TypeDefinitelySubsumesTypeNoCoercion (ndeep+1) g amap m ty1))))



type CanCoerce = CanCoerce | NoCoerce

/// The feasible equivalence relation. Part of the language spec.
let rec TypesFeasiblyEquiv ndeep g amap m ty1 ty2 = 

    if ndeep > 100 then error(InternalError("recursive class hierarchy (detected in TypeFeasiblySubsumesType), ty1 = "^(DebugPrint.showType ty1),m));
    let ty1 = stripTyEqns g ty1
    let ty2 = stripTyEqns g ty2
    match ty1,ty2 with 
    // QUERY: should these be false for non-equal rigid typars? warn-if-not-rigid typars?
    | TType_var _ , _  
    | _, TType_var _ -> true
    | TType_app (tc1,l1)  ,TType_app (tc2,l2) when tyconRefEq g tc1 tc2  ->  
        List.lengthsEqAndForall2 (TypesFeasiblyEquiv ndeep g amap m) l1 l2
    | TType_tuple l1    ,TType_tuple l2     -> 
        List.lengthsEqAndForall2 (TypesFeasiblyEquiv ndeep g amap m) l1 l2 
    | TType_fun (d1,r1)  ,TType_fun (d2,r2)   -> 
        (TypesFeasiblyEquiv ndeep g amap m) d1 d2 && (TypesFeasiblyEquiv ndeep g amap m) r1 r2
    | TType_measure _, TType_measure _ ->
        true
    | _ -> 
        false

/// The feasible coercion relation. Part of the language spec.

let rec TypeFeasiblySubsumesType ndeep g amap m ty1 canCoerce ty2 = 
    if ndeep > 100 then error(InternalError("recursive class hierarchy (detected in TypeFeasiblySubsumesType), ty1 = "^(DebugPrint.showType ty1),m))
    let ty1 = stripTyEqns g ty1
    let ty2 = stripTyEqns g ty2
    match ty1,ty2 with 
    // QUERY: should these be false for non-equal rigid typars? warn-if-not-rigid typars?
    | TType_var _ , _  | _, TType_var _ -> true

    | TType_app (tc1,l1)  ,TType_app (tc2,l2) when tyconRefEq g tc1 tc2  ->  
        List.lengthsEqAndForall2 (TypesFeasiblyEquiv ndeep g amap m) l1 l2
    | TType_tuple l1    ,TType_tuple l2     -> 
        List.lengthsEqAndForall2 (TypesFeasiblyEquiv ndeep g amap m) l1 l2 
    | TType_fun (d1,r1)  ,TType_fun (d2,r2)   -> 
        (TypesFeasiblyEquiv ndeep g amap m) d1 d2 && (TypesFeasiblyEquiv ndeep g amap m) r1 r2
    | TType_measure _, TType_measure _ ->
        true
    | _ -> 
        // F# reference types are subtypes of type 'obj' 
        (isObjTy g ty1 && (canCoerce = CanCoerce || isRefTy g ty2)) 
        ||
        (isAppTy g ty2 &&
         (canCoerce = CanCoerce || isRefTy g ty2) && 
         begin match GetSuperTypeOfType g amap m ty2 with 
         | None -> false
         | Some ty -> TypeFeasiblySubsumesType (ndeep+1) g amap m ty1 NoCoerce ty
         end ||
         ty2 |> GetImmediateInterfacesOfType SkipUnrefInterfaces.Yes g amap m 
             |> List.exists (TypeFeasiblySubsumesType (ndeep+1) g amap m ty1 NoCoerce))
                   

/// Choose solutions for Expr.TyChoose type "hidden" variables introduced
/// by letrec nodes. Also used by the pattern match compiler to choose type
/// variables when compiling patterns at generalized bindings.
///     e.g. let ([],x) = ([],[])
/// Here x gets a generalized type "list<'T>".
let ChooseTyparSolutionAndRange g amap (tp:Typar) =
    let m = tp.Range
    let max,m = 
         let initial = 
             match tp.Kind with 
             | TyparKind.Type -> g.obj_ty 
             | TyparKind.Measure -> TType_measure MeasureOne
         // Loop through the constraints computing the lub
         ((initial,m), tp.Constraints) ||> List.fold (fun (maxSoFar,_) tpc -> 
             let join m x = 
                 if TypeFeasiblySubsumesType 0 g amap m x CanCoerce maxSoFar then maxSoFar
                 elif TypeFeasiblySubsumesType 0 g amap m maxSoFar CanCoerce x then x
                 else errorR(Error(FSComp.SR.typrelCannotResolveImplicitGenericInstantiation((DebugPrint.showType x), (DebugPrint.showType maxSoFar)),m)); maxSoFar
             // Don't continue if an error occurred and we set the value eagerly 
             if tp.IsSolved then maxSoFar,m else
             match tpc with 
             | TyparConstraint.CoercesTo(x,m) -> 
                 join m x,m
             | TyparConstraint.MayResolveMember(TTrait(_,nm,_,_,_,_),m) -> 
                 errorR(Error(FSComp.SR.typrelCannotResolveAmbiguityInOverloadedOperator(DemangleOperatorName nm),m))
                 maxSoFar,m
             | TyparConstraint.SimpleChoice(_,m) -> 
                 errorR(Error(FSComp.SR.typrelCannotResolveAmbiguityInPrintf(),m))
                 maxSoFar,m
             | TyparConstraint.SupportsNull m -> 
                 maxSoFar,m
             | TyparConstraint.SupportsComparison m -> 
                 join m g.mk_IComparable_ty,m
             | TyparConstraint.SupportsEquality m -> 
                 maxSoFar,m
             | TyparConstraint.IsEnum(_,m) -> 
                 errorR(Error(FSComp.SR.typrelCannotResolveAmbiguityInEnum(),m))
                 maxSoFar,m
             | TyparConstraint.IsDelegate(_,_,m) -> 
                 errorR(Error(FSComp.SR.typrelCannotResolveAmbiguityInDelegate(),m))
                 maxSoFar,m
             | TyparConstraint.IsNonNullableStruct m -> 
                 join m g.int_ty,m
             | TyparConstraint.IsUnmanaged m ->
                 errorR(Error(FSComp.SR.typrelCannotResolveAmbiguityInUnmanaged(),m))
                 maxSoFar,m
             | TyparConstraint.RequiresDefaultConstructor m -> 
                 maxSoFar,m
             | TyparConstraint.IsReferenceType m -> 
                 maxSoFar,m
             | TyparConstraint.DefaultsTo(_priority,_ty,m) -> 
                 maxSoFar,m)
    max,m

let ChooseTyparSolution g amap tp = 
    let ty,_m = ChooseTyparSolutionAndRange g amap tp
    if tp.Rigidity = TyparRigidity.Anon && typeEquiv g ty (TType_measure MeasureOne) then
        warning(Error(FSComp.SR.csCodeLessGeneric(),tp.Range))
    ty

// Solutions can, in theory, refer to each other
// For example
//   'a = Expr<'b>
//   'b = int
// In this case the solutions are 
//   'a = Expr<int>
//   'b = int
// We ground out the solutions by repeatedly instantiating
let IterativelySubstituteTyparSolutions g tps solutions = 
    let tpenv = mkTyparInst tps solutions
    let rec loop n curr = 
        let curr' = curr |> instTypes tpenv 
        // We cut out at n > 40 just in case this loops. It shouldn't, since there should be no cycles in the
        // solution equations, and we've only ever seen one example where even n = 2 was required.
        // Perhaps it's possible in error recovery some strange situations could occur where cycles
        // arise, so it's better to be on the safe side.
        //
        // We don't give an error if we hit the limit since it's feasible that the solutions of unknowns
        // is not actually relevant to the rest of type checking or compilation.
        if n > 40 || List.forall2 (typeEquiv g) curr curr' then 
            curr 
        else 
            loop (n+1) curr'

    loop 0 solutions

let ChooseTyparSolutionsForFreeChoiceTypars g amap e = 
    match e with 
    | Expr.TyChoose(tps,e1,_m)  -> 
    
        /// Only make choices for variables that are actually used in the expression 
        let ftvs = (freeInExpr CollectTyparsNoCaching e1).FreeTyvars.FreeTypars
        let tps = tps |> List.filter (Zset.memberOf ftvs)
        
        let solutions =  tps |> List.map (ChooseTyparSolution g amap) |> IterativelySubstituteTyparSolutions g tps
        
        let tpenv = mkTyparInst tps solutions
        
        instExpr g tpenv e1

    | _ -> e
                 

/// Break apart lambdas. Needs ChooseTyparSolutionsForFreeChoiceTypars because it's used in
/// PostTypecheckSemanticChecks before we've eliminated these nodes.
let tryDestTopLambda g amap (ValReprInfo (tpNames,_,_) as tvd) (e,ty) =
    let rec stripLambdaUpto n (e,ty) = 
        match e with 
        | Expr.Lambda (_,None,None,v,b,_,retTy) when n > 0 -> 
            let (vs',b',retTy') = stripLambdaUpto (n-1) (b,retTy)
            (v :: vs', b', retTy') 
        | _ -> 
            ([],e,ty)

    let rec startStripLambdaUpto n (e,ty) = 
        match e with 
        | Expr.Lambda (_,ctorThisValOpt,baseValOpt,v,b,_,retTy) when n > 0 -> 
            let (vs',b',retTy') = stripLambdaUpto (n-1) (b,retTy)
            (ctorThisValOpt,baseValOpt, (v :: vs'), b', retTy') 
        | Expr.TyChoose (_tps,_b,_) -> 
            startStripLambdaUpto n (ChooseTyparSolutionsForFreeChoiceTypars g amap e, ty)
        | _ -> 
            (None,None,[],e,ty)

    let n = tvd.NumCurriedArgs
    let tps,taue,tauty = 
        match e with 
        | Expr.TyLambda (_,tps,b,_,retTy) when nonNil tpNames -> tps,b,retTy 
        | _ -> [],e,ty
    let ctorThisValOpt,baseValOpt,vsl,body,retTy = startStripLambdaUpto n (taue,tauty)
    if vsl.Length <> n then 
        None 
    else
        Some (tps,ctorThisValOpt,baseValOpt,vsl,body,retTy)

let destTopLambda g amap topValInfo (e,ty) = 
    match tryDestTopLambda g amap topValInfo (e,ty) with 
    | None -> error(Error(FSComp.SR.typrelInvalidValue(), e.Range))
    | Some res -> res
    
let IteratedAdjustArityOfLambdaBody g arities vsl body  =
      (arities, vsl, ([],body)) |||> List.foldBack2 (fun arities vs (allvs,body) -> 
          let vs,body = AdjustArityOfLambdaBody g arities vs body
          vs :: allvs, body)

/// Do AdjustArityOfLambdaBody for a series of  
/// iterated lambdas, producing one method.  
/// The required iterated function arity (List.length topValInfo) must be identical 
/// to the iterated function arity of the input lambda (List.length vsl) 
let IteratedAdjustArityOfLambda g amap topValInfo e =
    let tps,ctorThisValOpt,baseValOpt,vsl,body,bodyty = destTopLambda g amap topValInfo (e, tyOfExpr g e)
    let arities = topValInfo.AritiesOfArgs
    if arities.Length <> vsl.Length then 
        errorR(InternalError(sprintf "IteratedAdjustArityOfLambda, List.length arities = %d, List.length vsl = %d" (List.length arities) (List.length vsl), body.Range))
    let vsl,body = IteratedAdjustArityOfLambdaBody g arities vsl body
    tps,ctorThisValOpt,baseValOpt,vsl,body,bodyty


exception RequiredButNotSpecified of DisplayEnv * Tast.ModuleOrNamespaceRef * string * (StringBuilder -> unit) * range
exception ValueNotContained       of DisplayEnv * Tast.ModuleOrNamespaceRef * Val * Val * (string * string * string -> string)
exception ConstrNotContained      of DisplayEnv * UnionCase * UnionCase * (string * string -> string)
exception ExnconstrNotContained   of DisplayEnv * Tycon * Tycon * (string * string -> string)
exception FieldNotContained       of DisplayEnv * RecdField * RecdField * (string * string -> string)
exception InterfaceNotRevealed    of DisplayEnv * TType * range


/// Containment relation for module types
module SignatureConformance = begin

    // Use a type to capture the constant, common parameters 
    type Checker(g, amap, denv, remapInfo: SignatureRepackageInfo, checkingSig) = 

        // Build a remap that maps tcrefs in the signature to tcrefs in the implementation
        // Used when checking attributes.
        let sigToImplRemap = 
            let remap = Remap.Empty 
            let remap = (remapInfo.mrpiEntities,remap) ||> List.foldBack (fun (implTcref ,signTcref) acc -> addTyconRefRemap signTcref implTcref acc) 
            let remap = (remapInfo.mrpiVals    ,remap) ||> List.foldBack (fun (implValRef,signValRef) acc -> addValRemap signValRef.Deref implValRef.Deref acc) 
            remap
            
        // For all attributable elements (types, modules, exceptions, record fields, unions, parameters, generic type parameters)
        //
        // (a)	Start with lists AImpl and ASig containing the attributes in the implementation and signature, in declaration order
        // (b)	Each attribute in AImpl is checked against the available attributes in ASig. 
        //     a.	If an attribute is found in ASig which is an exact match (after evaluating attribute arguments), then the attribute in the implementation is ignored, the attribute is removed from ASig, and checking continues
        //     b.	If an attribute is found in ASig that has the same attribute type, then a warning is given and the attribute in the implementation is ignored 
        //     c.	Otherwise, the attribute in the implementation is kept
        // (c)	The attributes appearing in the compiled element are the compiled forms of the attributes from the signature and the kept attributes from the implementation
        let checkAttribs _aenv (implAttribs:Attribs) (sigAttribs:Attribs) fixup =
            
            // Remap the signature attributes to make them look as if they were declared in 
            // the implementation. This allows us to compare them and propagate them to the implementation
            // if needed.
            let sigAttribs = sigAttribs |> List.map (remapAttrib g sigToImplRemap)

            // Helper to check for equality of evaluated attribute expressions
            let attribExprEq (AttribExpr(_,e1)) (AttribExpr(_,e2)) = EvaledAttribExprEquality g e1 e2

            // Helper to check for equality of evaluated named attribute arguments
            let attribNamedArgEq (AttribNamedArg(nm1,ty1,isProp1,e1)) (AttribNamedArg(nm2,ty2,isProp2,e2)) = 
                (nm1 = nm2) && 
                typeEquiv g ty1 ty2 && 
                (isProp1 = isProp2) && 
                attribExprEq e1 e2

            let attribsEq  attrib1 attrib2 = 
                let (Attrib(implTcref,_,implArgs,implNamedArgs,_,_,_implRange)) = attrib1
                let (Attrib(signTcref,_,signArgs,signNamedArgs,_,_,_signRange)) = attrib2
                tyconRefEq g signTcref implTcref &&
                (implArgs,signArgs) ||> List.lengthsEqAndForall2 attribExprEq &&
                (implNamedArgs, signNamedArgs) ||> List.lengthsEqAndForall2 attribNamedArgEq

            let attribsHaveSameTycon attrib1 attrib2 = 
                let (Attrib(implTcref,_,_,_,_,_,_)) = attrib1
                let (Attrib(signTcref,_,_,_,_,_,_)) = attrib2
                tyconRefEq g signTcref implTcref 

            // For each implementation attribute, only keep if it it is not mentioned in the signature.
            // Emit a warning if it is mentioned in the signature and the arguments to the attributes are 
            // not identical.
            let rec check keptImplAttribsRev implAttribs sigAttribs = 
                match implAttribs with 
                | [] -> keptImplAttribsRev |> List.rev
                | implAttrib :: remainingImplAttribs -> 

                    // Look for an attribute in the signature that matches precisely. If so, remove it 
                    let lookForMatchingAttrib =  sigAttribs |> List.tryRemove (attribsEq implAttrib)
                    match lookForMatchingAttrib with 
                    | Some (_, remainingSigAttribs) -> check keptImplAttribsRev remainingImplAttribs remainingSigAttribs    
                    | None ->

                    // Look for an attribute in the signature that has the same type. If so, give a warning
                    let existsSimilarAttrib = sigAttribs |> List.exists (attribsHaveSameTycon implAttrib)

                    if existsSimilarAttrib then 
                        let (Attrib(implTcref,_,_,_,_,_,implRange)) = implAttrib
                        warning(Error(FSComp.SR.tcAttribArgsDiffer(implTcref.DisplayName), implRange))
                        check keptImplAttribsRev remainingImplAttribs sigAttribs    
                    else
                        check (implAttrib :: keptImplAttribsRev) remainingImplAttribs sigAttribs    
                
            let keptImplAttribs = check [] implAttribs sigAttribs 

            fixup (sigAttribs @ keptImplAttribs)
            true

        let rec checkTypars m (aenv: TypeEquivEnv) (implTypars:Typars) (sigTypars:Typars) = 
            if implTypars.Length <> sigTypars.Length then 
                errorR (Error(FSComp.SR.typrelSigImplNotCompatibleParamCountsDiffer(),m)) 
                false
            else 
              let aenv = aenv.BindEquivTypars implTypars sigTypars 
              (implTypars,sigTypars) ||> List.forall2 (fun implTypar sigTypar -> 
                  let m = sigTypar.Range
                  if implTypar.StaticReq <> sigTypar.StaticReq then 
                      errorR (Error(FSComp.SR.typrelSigImplNotCompatibleCompileTimeRequirementsDiffer(), m))          
                  
                  // Adjust the actual type parameter name to look look like the signature
                  implTypar.SetIdent (mkSynId implTypar.Range sigTypar.Id.idText)     

                  // Mark it as "not compiler generated", now that we've got a good name for it
                  implTypar.SetCompilerGenerated false 

                  // Check the constraints in the implementation are present in the signature
                  implTypar.Constraints |> List.forall (fun implTyparCx -> 
                      match implTyparCx with 
                      // defaults can be dropped in the signature 
                      | TyparConstraint.DefaultsTo(_,_acty,_) -> true
                      | _ -> 
                          if not (List.exists  (typarConstraintsAEquiv g aenv implTyparCx) sigTypar.Constraints)
                          then (errorR(Error(FSComp.SR.typrelSigImplNotCompatibleConstraintsDiffer(sigTypar.Name, Layout.showL(NicePrint.layoutTyparConstraint denv (implTypar,implTyparCx))),m)); false)
                          else  true) &&

                  // Check the constraints in the signature are present in the implementation
                  sigTypar.Constraints |> List.forall (fun sigTyparCx -> 
                      match sigTyparCx with 
                      // defaults can be present in the signature and not in the implementation  because they are erased
                      | TyparConstraint.DefaultsTo(_,_acty,_) -> true
                      // 'comparison' and 'equality' constraints can be present in the signature and not in the implementation  because they are erased
                      | TyparConstraint.SupportsComparison _ -> true
                      | TyparConstraint.SupportsEquality _ -> true
                      | _ -> 
                          if not (List.exists  (fun implTyparCx -> typarConstraintsAEquiv g aenv implTyparCx sigTyparCx) implTypar.Constraints) then
                              (errorR(Error(FSComp.SR.typrelSigImplNotCompatibleConstraintsDifferRemove(sigTypar.Name, Layout.showL(NicePrint.layoutTyparConstraint denv (sigTypar,sigTyparCx))),m)); false)
                          else  
                              true) &&
                  (not checkingSig || checkAttribs aenv implTypar.Attribs sigTypar.Attribs (fun attribs -> implTypar.Data.typar_attribs <- attribs)))

        and checkTypeDef (aenv: TypeEquivEnv) (implTycon:Tycon) (sigTycon:Tycon) =
            let m = implTycon.Range
            let err f =  Error(f(implTycon.TypeOrMeasureKind.ToString()), m)
            if implTycon.LogicalName <> sigTycon.LogicalName then  (errorR (err (FSComp.SR.DefinitionsInSigAndImplNotCompatibleNamesDiffer)); false) else
            if implTycon.CompiledName <> sigTycon.CompiledName then  (errorR (err (FSComp.SR.DefinitionsInSigAndImplNotCompatibleNamesDiffer)); false) else
            checkExnInfo  (fun f -> ExnconstrNotContained(denv,implTycon,sigTycon,f)) aenv implTycon.ExceptionInfo sigTycon.ExceptionInfo &&
            let implTypars = implTycon.Typars m
            let sigTypars = sigTycon.Typars m
            if implTypars.Length <> sigTypars.Length then  
                errorR (err(FSComp.SR.DefinitionsInSigAndImplNotCompatibleParameterCountsDiffer)) 
                false
            elif isLessAccessible implTycon.Accessibility sigTycon.Accessibility then 
                errorR(err(FSComp.SR.DefinitionsInSigAndImplNotCompatibleAccessibilityDiffer)) 
                false
            else 
                let aenv = aenv.BindEquivTypars implTypars sigTypars 

                let aintfs = implTycon.ImmediateInterfaceTypesOfFSharpTycon 
                let fintfs = sigTycon.ImmediateInterfaceTypesOfFSharpTycon 
                let aintfsUser = implTycon.TypeContents.tcaug_interfaces |> List.filter (fun (_,compgen,_) -> not compgen) |> List.map p13 
                let flatten tys = 
                   tys 
                   |> List.collect (AllSuperTypesOfType g amap m AllowMultiIntfInstantiations.Yes) 
                   |> ListSet.setify (typeEquiv g) 
                   |> List.filter (isInterfaceTy g)
                let aintfs     = flatten aintfs 
                let aintfsUser = flatten aintfsUser 
                let fintfs     = flatten fintfs 
              
                let unimpl = ListSet.subtract (fun fity aity -> typeAEquiv g aenv aity fity) fintfs aintfs
                (unimpl |> List.forall (fun ity -> errorR (err (fun x -> FSComp.SR.DefinitionsInSigAndImplNotCompatibleMissingInterface(x,  NicePrint.minimalStringOfType denv ity))); false)) &&
                let hidden = ListSet.subtract (typeAEquiv g aenv) aintfsUser fintfs
                hidden |> List.iter (fun ity -> (if implTycon.IsFSharpInterfaceTycon then error else warning) (InterfaceNotRevealed(denv,ity,implTycon.Range)))

                let aNull = IsUnionTypeWithNullAsTrueValue g implTycon
                let fNull = IsUnionTypeWithNullAsTrueValue g sigTycon
                if aNull && not fNull then 
                  errorR(err(FSComp.SR.DefinitionsInSigAndImplNotCompatibleImplementationSaysNull))
                elif fNull && not aNull then 
                  errorR(err(FSComp.SR.DefinitionsInSigAndImplNotCompatibleSignatureSaysNull))

                let aNull2 = TypeNullIsExtraValue g m (generalizedTyconRef (mkLocalTyconRef implTycon))
                let fNull2 = TypeNullIsExtraValue g m (generalizedTyconRef (mkLocalTyconRef implTycon))
                if aNull2 && not fNull2 then 
                    errorR(err(FSComp.SR.DefinitionsInSigAndImplNotCompatibleImplementationSaysNull2))
                elif fNull2 && not aNull2 then 
                    errorR(err(FSComp.SR.DefinitionsInSigAndImplNotCompatibleSignatureSaysNull2))

                let aSealed = isSealedTy g (generalizedTyconRef (mkLocalTyconRef implTycon))
                let fSealed = isSealedTy g (generalizedTyconRef (mkLocalTyconRef sigTycon))
                if  aSealed && not fSealed  then 
                    errorR(err(FSComp.SR.DefinitionsInSigAndImplNotCompatibleImplementationSealed))
                if  not aSealed && fSealed  then 
                    errorR(err(FSComp.SR.DefinitionsInSigAndImplNotCompatibleImplementationIsNotSealed))

                let aPartial = isAbstractTycon implTycon
                let fPartial = isAbstractTycon sigTycon
                if aPartial && not fPartial then 
                    errorR(err(FSComp.SR.DefinitionsInSigAndImplNotCompatibleImplementationIsAbstract))

                if not aPartial && fPartial then 
                    errorR(err(FSComp.SR.DefinitionsInSigAndImplNotCompatibleSignatureIsAbstract))

                if not (typeAEquiv g aenv (superOfTycon g implTycon) (superOfTycon g sigTycon)) then 
                    errorR (err(FSComp.SR.DefinitionsInSigAndImplNotCompatibleTypesHaveDifferentBaseTypes))

                checkTypars m aenv implTypars sigTypars &&
                checkTypeRepr err aenv implTycon.TypeReprInfo sigTycon.TypeReprInfo &&
                checkTypeAbbrev err aenv implTycon.TypeOrMeasureKind sigTycon.TypeOrMeasureKind implTycon.TypeAbbrev sigTycon.TypeAbbrev  &&
                checkAttribs aenv implTycon.Attribs sigTycon.Attribs (fun attribs -> implTycon.Data.entity_attribs <- attribs) &&
                checkModuleOrNamespaceContents implTycon.Range aenv (mkLocalEntityRef implTycon) sigTycon.ModuleOrNamespaceType 
            
        and checkValInfo aenv err (implVal : Val) (sigVal : Val) = 
            let id = implVal.Id
            match implVal.ValReprInfo, sigVal.ValReprInfo with 
            | _,None -> true
            | None, Some _ -> err(FSComp.SR.ValueNotContainedMutabilityArityNotInferred)
            | Some (ValReprInfo (implTyparNames,implArgInfos,implRetInfo) as implValInfo), Some (ValReprInfo (sigTyparNames,sigArgInfos,sigRetInfo) as sigValInfo) ->
                let ntps = implTyparNames.Length
                let mtps = sigTyparNames.Length
                if ntps <> mtps then
                  err(fun(x, y, z) -> FSComp.SR.ValueNotContainedMutabilityGenericParametersDiffer(x, y, z, string mtps, string ntps))
                elif implValInfo.KindsOfTypars <> sigValInfo.KindsOfTypars then
                  err(FSComp.SR.ValueNotContainedMutabilityGenericParametersAreDifferentKinds)
                elif not (sigArgInfos.Length <= implArgInfos.Length && List.forall2 (fun x y -> List.length x <= List.length y) sigArgInfos (fst (List.chop sigArgInfos.Length implArgInfos))) then 
                  err(fun(x, y, z) -> FSComp.SR.ValueNotContainedMutabilityAritiesDiffer(x, y, z, id.idText, string sigArgInfos.Length, id.idText, id.idText))
                else 
                  let implArgInfos = implArgInfos |> List.take sigArgInfos.Length  
                  let implArgInfos = (implArgInfos, sigArgInfos) ||> List.map2 (fun l1 l2 -> l1 |> List.take l2.Length)
                  // Propagate some information signature to implementation. 

                  // Check the attributes on each argument, and update the ValReprInfo for
                  // the value to reflect the information in the the signature.
                  // This ensures that the compiled form of the value matches the signature rather than 
                  // the implementation. This also propagates argument names from signature to implementation
                  let res = 
                      (implArgInfos,sigArgInfos) ||> List.forall2 (List.forall2 (fun implArgInfo sigArgInfo -> 
                          checkAttribs aenv implArgInfo.Attribs sigArgInfo.Attribs (fun attribs -> 
                              implArgInfo.Name <- sigArgInfo.Name
                              implArgInfo.Attribs <- attribs))) && 

                      checkAttribs aenv implRetInfo.Attribs sigRetInfo.Attribs (fun attribs -> 
                          implRetInfo.Name <- sigRetInfo.Name
                          implRetInfo.Attribs <- attribs)
                  
                  implVal.SetValReprInfo (Some (ValReprInfo (sigTyparNames,implArgInfos,implRetInfo)))
                  res

        and checkVal implModRef (aenv:TypeEquivEnv) (implVal:Val) (sigVal:Val) =

            // Propagate defn location information from implementation to signature . 
            sigVal.SetDefnRange implVal.DefinitionRange

            let mk_err denv f = ValueNotContained(denv,implModRef,implVal,sigVal,f)
            let err denv f = errorR(mk_err denv f); false
            let m = implVal.Range
            if implVal.IsMutable <> sigVal.IsMutable then (err denv FSComp.SR.ValueNotContainedMutabilityAttributesDiffer)
            elif implVal.LogicalName <> sigVal.LogicalName then (err denv FSComp.SR.ValueNotContainedMutabilityNamesDiffer)
            elif implVal.CompiledName <> sigVal.CompiledName then (err denv FSComp.SR.ValueNotContainedMutabilityCompiledNamesDiffer)
            elif implVal.DisplayName <> sigVal.DisplayName then (err denv FSComp.SR.ValueNotContainedMutabilityDisplayNamesDiffer)
            elif isLessAccessible implVal.Accessibility sigVal.Accessibility then (err denv FSComp.SR.ValueNotContainedMutabilityAccessibilityMore)
            elif implVal.MustInline <> sigVal.MustInline then (err denv FSComp.SR.ValueNotContainedMutabilityInlineFlagsDiffer)
            elif implVal.LiteralValue <> sigVal.LiteralValue then (err denv FSComp.SR.ValueNotContainedMutabilityLiteralConstantValuesDiffer)
            elif implVal.IsTypeFunction <> sigVal.IsTypeFunction then (err denv FSComp.SR.ValueNotContainedMutabilityOneIsTypeFunction)
            else 
                let implTypars,atau = implVal.TypeScheme
                let sigTypars,ftau = sigVal.TypeScheme
                if implTypars.Length <> sigTypars.Length then (err {denv with showTyparBinding=true} FSComp.SR.ValueNotContainedMutabilityParameterCountsDiffer) else
                let aenv = aenv.BindEquivTypars implTypars sigTypars 
                checkTypars m aenv implTypars sigTypars &&
                if not (typeAEquiv g aenv atau ftau) then err denv (FSComp.SR.ValueNotContainedMutabilityTypesDiffer)
                elif not (checkValInfo aenv (err denv) implVal sigVal) then false
                elif not (implVal.IsExtensionMember = sigVal.IsExtensionMember) then err denv (FSComp.SR.ValueNotContainedMutabilityExtensionsDiffer)
                elif not (checkMemberDatasConform (err denv) (implVal.Attribs, implVal,implVal.MemberInfo) (sigVal.Attribs,sigVal,sigVal.MemberInfo)) then false
                else checkAttribs aenv implVal.Attribs sigVal.Attribs (fun attribs -> implVal.Data.val_attribs <- attribs)              


        and checkExnInfo err aenv implTypeRepr sigTypeRepr =
            match implTypeRepr,sigTypeRepr with 
            | TExnAsmRepr _, TExnFresh _ -> 
                (errorR (err FSComp.SR.ExceptionDefsNotCompatibleHiddenBySignature); false)
            | TExnAsmRepr tcr1, TExnAsmRepr tcr2  -> 
                if tcr1 <> tcr2 then  (errorR (err FSComp.SR.ExceptionDefsNotCompatibleDotNetRepresentationsDiffer); false) else true
            | TExnAbbrevRepr _, TExnFresh _ -> 
                (errorR (err FSComp.SR.ExceptionDefsNotCompatibleAbbreviationHiddenBySignature); false)
            | TExnAbbrevRepr ecr1, TExnAbbrevRepr ecr2 -> 
                if not (tcrefAEquiv g aenv ecr1 ecr2) then 
                  (errorR (err FSComp.SR.ExceptionDefsNotCompatibleSignaturesDiffer); false)
                else true
            | TExnFresh r1, TExnFresh  r2-> checkRecordFieldsForExn g denv err aenv r1 r2
            | TExnNone,TExnNone -> true
            | _ -> 
                (errorR (err FSComp.SR.ExceptionDefsNotCompatibleExceptionDeclarationsDiffer); false)

        and checkUnionCase aenv implUnionCase sigUnionCase =
            let err f = errorR(ConstrNotContained(denv,implUnionCase,sigUnionCase,f));false
            if implUnionCase.Id.idText <> sigUnionCase.Id.idText then  err FSComp.SR.ModuleContainsConstructorButNamesDiffer
            elif implUnionCase.RecdFields.Length <> sigUnionCase.RecdFields.Length then err FSComp.SR.ModuleContainsConstructorButDataFieldsDiffer
            elif not (List.forall2 (checkField aenv) implUnionCase.RecdFields sigUnionCase.RecdFields) then err FSComp.SR.ModuleContainsConstructorButTypesOfFieldsDiffer
            elif isLessAccessible implUnionCase.Accessibility sigUnionCase.Accessibility then err FSComp.SR.ModuleContainsConstructorButAccessibilityDiffers
            else checkAttribs aenv implUnionCase.Attribs sigUnionCase.Attribs (fun attribs -> implUnionCase.Attribs <- attribs)

        and checkField aenv implField sigField =
            let err f = errorR(FieldNotContained(denv,implField,sigField,f)); false
            if implField.rfield_id.idText <> sigField.rfield_id.idText then err FSComp.SR.FieldNotContainedNamesDiffer
            elif isLessAccessible implField.Accessibility sigField.Accessibility then err FSComp.SR.FieldNotContainedAccessibilitiesDiffer
            elif implField.IsStatic <> sigField.IsStatic then err FSComp.SR.FieldNotContainedStaticsDiffer
            elif implField.IsMutable <> sigField.IsMutable then err FSComp.SR.FieldNotContainedMutablesDiffer
            elif implField.LiteralValue <> sigField.LiteralValue then err FSComp.SR.FieldNotContainedLiteralsDiffer
            elif not (typeAEquiv g aenv implField.FormalType sigField.FormalType) then err FSComp.SR.FieldNotContainedTypesDiffer
            else 
                checkAttribs aenv implField.FieldAttribs sigField.FieldAttribs (fun attribs -> implField.rfield_fattribs <- attribs) &&
                checkAttribs aenv implField.PropertyAttribs sigField.PropertyAttribs (fun attribs -> implField.rfield_pattribs <- attribs)
            
        and checkMemberDatasConform err  (_implAttrs,implVal,implMemberInfo) (_sigAttrs, sigVal,sigMemberInfo)  =
            match implMemberInfo,sigMemberInfo with 
            | None,None -> true
            | Some implMembInfo, Some sigMembInfo -> 
                if not (implVal.CompiledName = sigVal.CompiledName) then 
                  err(FSComp.SR.ValueNotContainedMutabilityDotNetNamesDiffer)
                elif not (implMembInfo.MemberFlags.IsInstance = sigMembInfo.MemberFlags.IsInstance) then 
                  err(FSComp.SR.ValueNotContainedMutabilityStaticsDiffer)
                elif false then 
                  err(FSComp.SR.ValueNotContainedMutabilityVirtualsDiffer)
                elif not (implMembInfo.MemberFlags.IsDispatchSlot = sigMembInfo.MemberFlags.IsDispatchSlot) then 
                  err(FSComp.SR.ValueNotContainedMutabilityAbstractsDiffer)
               // The final check is an implication:
               //     classes have non-final CompareTo/Hash methods 
               //     abstract have non-final CompareTo/Hash methods 
               //     records  have final CompareTo/Hash methods 
               //     unions  have final CompareTo/Hash methods 
               // This is an example where it is OK for the signaure to say 'non-final' when the implementation says 'final' 
                elif not implMembInfo.MemberFlags.IsFinal && sigMembInfo.MemberFlags.IsFinal then 
                  err(FSComp.SR.ValueNotContainedMutabilityFinalsDiffer)
                elif not (implMembInfo.MemberFlags.IsOverrideOrExplicitImpl = sigMembInfo.MemberFlags.IsOverrideOrExplicitImpl) then 
                  err(FSComp.SR.ValueNotContainedMutabilityOverridesDiffer)
                elif not (implMembInfo.MemberFlags.MemberKind = sigMembInfo.MemberFlags.MemberKind) then 
                  err(FSComp.SR.ValueNotContainedMutabilityOneIsConstructor)
                else  
                   let finstance = ValSpecIsCompiledAsInstance g sigVal
                   let ainstance = ValSpecIsCompiledAsInstance g implVal
                   if  finstance && not ainstance then 
                      err(FSComp.SR.ValueNotContainedMutabilityStaticButInstance)
                   elif not finstance && ainstance then 
                      err(FSComp.SR.ValueNotContainedMutabilityInstanceButStatic)
                   else true

            | _ -> false

        // -------------------------------------------------------------------------------
        // WARNING!!!!
        // checkRecordFields and checkRecordFieldsForExn are the EXACT SAME FUNCTION.
        // The only difference is the signature for err - this is because err is a function
        // that reports errors, and checkRecordFields is called with a different
        // sig for err then checkRecordFieldsForExn.
        // -------------------------------------------------------------------------------

        and checkRecordFields _g _amap _denv err aenv (implFields:TyconRecdFields) (sigFields:TyconRecdFields) =
            let implFields = implFields.TrueFieldsAsList
            let sigFields = sigFields.TrueFieldsAsList
            let m1 = implFields |> NameMap.ofKeyedList (fun rfld -> rfld.Name)
            let m2 = sigFields |> NameMap.ofKeyedList (fun rfld -> rfld.Name)
            NameMap.suball2 (fun s _ -> errorR(err (fun x -> FSComp.SR.DefinitionsInSigAndImplNotCompatibleFieldRequiredButNotSpecified(x, s))); false) (checkField aenv)  m1 m2 &&
            NameMap.suball2 (fun s _ -> errorR(err (fun x -> FSComp.SR.DefinitionsInSigAndImplNotCompatibleFieldWasPresent(x, s))); false) (fun x y -> checkField aenv y x)  m2 m1 &&
            // This check is required because constructors etc. are externally visible 
            // and thus compiled representations do pick up dependencies on the field order  
            (if List.forall2 (checkField aenv)  implFields sigFields
             then true
             else (errorR(err (FSComp.SR.DefinitionsInSigAndImplNotCompatibleFieldOrderDiffer)); false))

        and checkRecordFieldsForExn _g _denv err aenv (implFields:TyconRecdFields) (sigFields:TyconRecdFields) =
            let implFields = implFields.TrueFieldsAsList
            let sigFields = sigFields.TrueFieldsAsList
            let m1 = implFields |> NameMap.ofKeyedList (fun rfld -> rfld.Name)
            let m2 = sigFields |> NameMap.ofKeyedList (fun rfld -> rfld.Name)
            NameMap.suball2 (fun s _ -> errorR(err (fun (x, y) -> FSComp.SR.ExceptionDefsNotCompatibleFieldInSigButNotImpl(s, x, y))); false) (checkField aenv)  m1 m2 &&
            NameMap.suball2 (fun s _ -> errorR(err (fun (x, y) -> FSComp.SR.ExceptionDefsNotCompatibleFieldInImplButNotSig(s, x, y))); false) (fun x y -> checkField aenv y x)  m2 m1 &&
            // This check is required because constructors etc. are externally visible 
            // and thus compiled representations do pick up dependencies on the field order  
            (if List.forall2 (checkField aenv)  implFields sigFields
             then true
             else (errorR(err (FSComp.SR.ExceptionDefsNotCompatibleFieldOrderDiffers)); false))

        and checkVirtualSlots _g denv err _aenv implAbstractSlots sigAbstractSlots =
            let m1 = NameMap.ofKeyedList (fun (v:ValRef) -> v.DisplayName) implAbstractSlots
            let m2 = NameMap.ofKeyedList (fun (v:ValRef) -> v.DisplayName) sigAbstractSlots
            (m1,m2) ||> NameMap.suball2 (fun _s vref -> errorR(err (fun x -> FSComp.SR.DefinitionsInSigAndImplNotCompatibleAbstractMemberMissingInImpl(x, NicePrint.stringValOrMember denv vref.Deref))); false) (fun _x _y -> true)  &&
            (m2,m1) ||> NameMap.suball2 (fun _s vref -> errorR(err (fun x -> FSComp.SR.DefinitionsInSigAndImplNotCompatibleAbstractMemberMissingInSig(x, NicePrint.stringValOrMember denv vref.Deref))); false) (fun _x _y -> true)  

        and checkClassFields isStruct _g _amap _denv err aenv (implFields:TyconRecdFields) (sigFields:TyconRecdFields) =
            let implFields = implFields.TrueFieldsAsList
            let sigFields = sigFields.TrueFieldsAsList
            let m1 = implFields |> NameMap.ofKeyedList (fun rfld -> rfld.Name) 
            let m2 = sigFields |> NameMap.ofKeyedList (fun rfld -> rfld.Name) 
            NameMap.suball2 (fun s _ -> errorR(err (fun x -> FSComp.SR.DefinitionsInSigAndImplNotCompatibleFieldRequiredButNotSpecified(x, s))); false) (checkField aenv)  m1 m2 &&
            (if isStruct then 
                NameMap.suball2 (fun s _ -> warning(err (fun x -> FSComp.SR.DefinitionsInSigAndImplNotCompatibleFieldIsInImplButNotSig(x, s))); true) (fun x y -> checkField aenv y x)  m2 m1 
             else
                true)
            

        and checkTypeRepr err aenv implTypeRepr sigTypeRepr =
            let reportNiceError k s1 s2 = 
              let aset = NameSet.ofList s1
              let fset = NameSet.ofList s2
              match Zset.elements (Zset.diff aset fset) with 
              | [] -> 
                  match Zset.elements (Zset.diff fset aset) with             
                  | [] -> (errorR (err (fun x -> FSComp.SR.DefinitionsInSigAndImplNotCompatibleNumbersDiffer(x, k))); false)
                  | l -> (errorR (err (fun x -> FSComp.SR.DefinitionsInSigAndImplNotCompatibleSignatureDefinesButImplDoesNot(x, k, String.concat ";" l))); false)
              | l -> (errorR (err (fun x -> FSComp.SR.DefinitionsInSigAndImplNotCompatibleImplDefinesButSignatureDoesNot(x, k, String.concat ";" l))); false)

            match implTypeRepr,sigTypeRepr with 
            | (TRecdRepr _ 
              | TFiniteUnionRepr _ 
              | TILObjModelRepr _ 
#if EXTENSIONTYPING
              | TProvidedTypeExtensionPoint _ 
              | TProvidedNamespaceExtensionPoint _
#endif
              ), TNoRepr  -> true
            | (TFsObjModelRepr r), TNoRepr  -> 
                match r.fsobjmodel_kind with 
                | TTyconStruct | TTyconEnum -> 
                   (errorR (err FSComp.SR.DefinitionsInSigAndImplNotCompatibleImplDefinesStruct); false)
                | _ -> 
                   true
            | (TAsmRepr _), TNoRepr -> 
                (errorR (err FSComp.SR.DefinitionsInSigAndImplNotCompatibleDotNetTypeRepresentationIsHidden); false)
            | (TMeasureableRepr _), TNoRepr -> 
                (errorR (err FSComp.SR.DefinitionsInSigAndImplNotCompatibleTypeIsHidden); false)
            | (TFiniteUnionRepr r1), (TFiniteUnionRepr r2) -> 
                let ucases1 = r1.UnionCasesAsList
                let ucases2 = r2.UnionCasesAsList
                if ucases1.Length <> ucases2.Length then
                  let names l = List.map (fun c -> c.Id.idText) l
                  reportNiceError "union case" (names ucases1) (names ucases2) 
                else List.forall2 (checkUnionCase aenv) ucases1 ucases2
            | (TRecdRepr implFields), (TRecdRepr sigFields) -> 
                checkRecordFields g amap denv err aenv implFields sigFields
            | (TFsObjModelRepr r1), (TFsObjModelRepr r2) -> 
                if not (match r1.fsobjmodel_kind,r2.fsobjmodel_kind with 
                         | TTyconClass,TTyconClass -> true
                         | TTyconInterface,TTyconInterface -> true
                         | TTyconStruct,TTyconStruct -> true
                         | TTyconEnum, TTyconEnum -> true
                         | TTyconDelegate (TSlotSig(_,typ1,ctps1,mtps1,ps1, rty1)), 
                           TTyconDelegate (TSlotSig(_,typ2,ctps2,mtps2,ps2, rty2)) -> 
                             (typeAEquiv g aenv typ1 typ2) &&
                             (ctps1.Length = ctps2.Length) &&
                             (let aenv = aenv.BindEquivTypars ctps1 ctps2 
                              (typarsAEquiv g aenv ctps1 ctps2) &&
                              (mtps1.Length = mtps2.Length) &&
                              (let aenv = aenv.BindEquivTypars mtps1 mtps2 
                               (typarsAEquiv g aenv mtps1 mtps2) &&
                               ((ps1,ps2) ||> List.lengthsEqAndForall2 (List.lengthsEqAndForall2 (fun p1 p2 -> typeAEquiv g aenv p1.Type p2.Type))) &&
                               (returnTypesAEquiv g aenv rty1 rty2)))
                         | _,_ -> false) then 
                  (errorR (err FSComp.SR.DefinitionsInSigAndImplNotCompatibleTypeIsDifferentKind); false)
                else 
                  let isStruct = (match r1.fsobjmodel_kind with TTyconStruct -> true | _ -> false)
                  checkClassFields isStruct g amap denv err aenv r1.fsobjmodel_rfields r2.fsobjmodel_rfields &&
                  checkVirtualSlots g denv err aenv r1.fsobjmodel_vslots r2.fsobjmodel_vslots
            | (TAsmRepr tcr1),  (TAsmRepr tcr2) -> 
                if tcr1 <> tcr2 then  (errorR (err FSComp.SR.DefinitionsInSigAndImplNotCompatibleILDiffer); false) else true
            | (TMeasureableRepr ty1),  (TMeasureableRepr ty2) -> 
                if typeAEquiv g aenv ty1 ty2 then true else (errorR (err FSComp.SR.DefinitionsInSigAndImplNotCompatibleRepresentationsDiffer); false)
            | TNoRepr, TNoRepr -> true
#if EXTENSIONTYPING
            | TProvidedTypeExtensionPoint info1 , TProvidedTypeExtensionPoint info2 ->  
                Tainted.EqTainted info1.ProvidedType.TypeProvider info2.ProvidedType.TypeProvider && ProvidedType.TaintedEquals(info1.ProvidedType,info2.ProvidedType)
            | TProvidedNamespaceExtensionPoint _, TProvidedNamespaceExtensionPoint _ -> 
                System.Diagnostics.Debug.Assert(false, "unreachable: TProvidedNamespaceExtensionPoint only on namespaces, not types" )
                true
#endif
            | TNoRepr, _ -> (errorR (err FSComp.SR.DefinitionsInSigAndImplNotCompatibleRepresentationsDiffer); false)
            | _, _ -> (errorR (err FSComp.SR.DefinitionsInSigAndImplNotCompatibleRepresentationsDiffer); false)

        and checkTypeAbbrev err aenv kind1 kind2 implTypeAbbrev sigTypeAbbrev =
            if kind1 <> kind2 then (errorR (err (fun x -> FSComp.SR.DefinitionsInSigAndImplNotCompatibleSignatureDeclaresDiffer(x, kind2.ToString(), kind1.ToString()))); false)
            else
              match implTypeAbbrev,sigTypeAbbrev with 
              | Some ty1, Some ty2 -> 
                  if not (typeAEquiv g aenv ty1 ty2) then 
                      let s1, s2, _  = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
                      errorR (err (fun x -> FSComp.SR.DefinitionsInSigAndImplNotCompatibleAbbreviationsDiffer(x, s1, s2))) 
                      false 
                  else 
                      true
              | None,None -> true
              | Some _, None -> (errorR (err (FSComp.SR.DefinitionsInSigAndImplNotCompatibleAbbreviationHiddenBySig)); false)
              | None, Some _ -> (errorR (err FSComp.SR.DefinitionsInSigAndImplNotCompatibleSigHasAbbreviation); false)

        and checkModuleOrNamespaceContents m aenv (implModRef:ModuleOrNamespaceRef) (signModType:ModuleOrNamespaceType) = 
            let implModType = implModRef.ModuleOrNamespaceType
            (if implModType.ModuleOrNamespaceKind <> signModType.ModuleOrNamespaceKind then errorR(Error(FSComp.SR.typrelModuleNamespaceAttributesDifferInSigAndImpl(),m)))


            (implModType.TypesByMangledName , signModType.TypesByMangledName)
             ||> NameMap.suball2 
                (fun s _fx -> errorR(RequiredButNotSpecified(denv,implModRef,"type",(fun os -> Printf.bprintf os "%s" s),m)); false) 
                (checkTypeDef aenv)  &&


            (implModType.ModulesAndNamespacesByDemangledName, signModType.ModulesAndNamespacesByDemangledName ) 
              ||> NameMap.suball2 
                   (fun s fx -> errorR(RequiredButNotSpecified(denv,implModRef,(if fx.IsModule then "module" else "namespace"),(fun os -> Printf.bprintf os "%s" s),m)); false) 
                   (fun x1 x2 -> checkModuleOrNamespace aenv (mkLocalModRef x1) x2)  &&

            let sigValHadNoMatchingImplementation (fx:Val) (_closeActualVal: Val option) = 
                errorR(RequiredButNotSpecified(denv,implModRef,"value",(fun os -> 
                   (* In the case of missing members show the full required enclosing type and signature *)
                   if fx.IsMember then 
                       NicePrint.outputQualifiedValOrMember denv os fx
                   else
                       Printf.bprintf os "%s" fx.DisplayName),m))
            
            let valuesPartiallyMatch (av:Val) (fv:Val) =
                (av.LinkagePartialKey.MemberParentMangledName = fv.LinkagePartialKey.MemberParentMangledName) &&
                (av.LinkagePartialKey.LogicalName = fv.LinkagePartialKey.LogicalName) &&
                (av.LinkagePartialKey.TotalArgCount = fv.LinkagePartialKey.TotalArgCount)    
                                       
            (implModType.AllValsAndMembersByLogicalNameUncached, signModType.AllValsAndMembersByLogicalNameUncached)
              ||> NameMap.suball2 
                    (fun _s (fxs:Val list) -> sigValHadNoMatchingImplementation fxs.Head None; false)
                    (fun avs fvs -> 
                        match avs,fvs with 
                        | [],_ | _,[] -> failwith "unreachable"
                        | [av],[fv] -> 
                            if valuesPartiallyMatch av fv then
                                checkVal implModRef aenv av fv
                            else
                                sigValHadNoMatchingImplementation fv None
                                false    
                        | _ -> 
                             // for each formal requirement, try to find a precisely matching actual requirement
                             let matchingPairs = 
                                 fvs |> List.choose (fun fv -> 
                                     match avs |> List.tryFind (fun av -> 
                                                         let res = valLinkageAEquiv g aenv av fv
                                                         //if res then printfn "%s" (bufs (fun buf -> Printf.bprintf buf "YES MATCH: fv '%a', av '%a'" (NicePrint.outputQualifiedValOrMember denv) fv (NicePrint.outputQualifiedValOrMember denv) av))
                                                         //else printfn "%s" (bufs (fun buf -> Printf.bprintf buf "NO MATCH: fv '%a', av '%a'" (NicePrint.outputQualifiedValOrMember denv) fv (NicePrint.outputQualifiedValOrMember denv) av))  
                                                         res) with 
                                      | None -> None
                                      | Some av -> Some(fv,av))
                             
                             // Check the ones with matching linkage
                             let allPairsOk = matchingPairs |> List.map (fun (fv,av) -> checkVal implModRef aenv av fv) |> List.forall id
                             let someNotOk = matchingPairs.Length < fvs.Length
                             // Report an error for those that don't. Try pairing up by enclosing-type/name
                             if someNotOk then 
                                 let noMatches,partialMatchingPairs = 
                                     fvs |> List.splitChoose (fun fv -> 
                                         match avs |> List.tryFind (fun av -> valuesPartiallyMatch av fv) with 
                                          | None -> Choice1Of2 fv
                                          | Some av -> Choice2Of2(fv,av))
                                 for (fv,av) in partialMatchingPairs do
                                     checkVal implModRef aenv av fv |> ignore
                                 for fv in noMatches do 
                                     sigValHadNoMatchingImplementation fv None
                             allPairsOk && not someNotOk)


        and checkModuleOrNamespace aenv implModRef sigModRef = 
            checkModuleOrNamespaceContents implModRef.Range aenv implModRef sigModRef.ModuleOrNamespaceType &&
            checkAttribs aenv implModRef.Attribs sigModRef.Attribs implModRef.Deref.SetAttribs

        member __.CheckSignature aenv (implModRef:ModuleOrNamespaceRef) (signModType:ModuleOrNamespaceType) = 
            checkModuleOrNamespaceContents implModRef.Range aenv implModRef signModType

        member __.CheckTypars m aenv (implTypars: Typars) (signTypars: Typars) = 
            checkTypars m aenv implTypars signTypars


    /// Check the names add up between a signature and its implementation. We check this first.
    let rec CheckNamesOfModuleOrNamespaceContents denv (implModRef:ModuleOrNamespaceRef) (signModType:ModuleOrNamespaceType) = 
        let m = implModRef.Range 
        let implModType = implModRef.ModuleOrNamespaceType
        NameMap.suball2 
            (fun s _fx -> errorR(RequiredButNotSpecified(denv,implModRef,"type",(fun os -> Printf.bprintf os "%s" s),m)); false) 
            (fun _ _ -> true)  
            implModType.TypesByMangledName 
            signModType.TypesByMangledName &&

        (implModType.ModulesAndNamespacesByDemangledName, signModType.ModulesAndNamespacesByDemangledName ) 
          ||> NameMap.suball2 
                (fun s fx -> errorR(RequiredButNotSpecified(denv,implModRef,(if fx.IsModule then "module" else "namespace"),(fun os -> Printf.bprintf os "%s" s),m)); false) 
                (fun x1 (x2:ModuleOrNamespace) -> CheckNamesOfModuleOrNamespace denv (mkLocalModRef x1) x2.ModuleOrNamespaceType)  &&

        (implModType.AllValsAndMembersByLogicalNameUncached , signModType.AllValsAndMembersByLogicalNameUncached) 
          ||> NameMap.suball2 
                (fun _s (fxs:Val list) -> 
                    let fx = fxs.Head
                    errorR(RequiredButNotSpecified(denv,implModRef,"value",(fun os -> 
                       // In the case of missing members show the full required enclosing type and signature 
                       if isSome fx.MemberInfo then 
                           NicePrint.outputQualifiedValOrMember denv os fx
                       else
                           Printf.bprintf os "%s" fx.DisplayName),m)); false)
                (fun _ _ -> true) 


    and CheckNamesOfModuleOrNamespace denv (implModRef:ModuleOrNamespaceRef) signModType = 
        CheckNamesOfModuleOrNamespaceContents denv implModRef signModType

end

//-------------------------------------------------------------------------
// Completeness of classes
//------------------------------------------------------------------------- 

type OverrideCanImplement = 
    | CanImplementAnyInterfaceSlot
    | CanImplementAnyClassHierarchySlot
    | CanImplementAnySlot
    | CanImplementNoSlots
    
/// The overall information about a method implementation in a class or object expression 
type OverrideInfo = 
    | Override of OverrideCanImplement * TyconRef * Ident * (Typars * TyparInst) * TType list list * TType option * bool
    member x.CanImplement = let (Override(a,_,_,_,_,_,_)) = x in a
    member x.BoundingTyconRef = let (Override(_,ty,_,_,_,_,_)) = x in ty
    member x.LogicalName = let (Override(_,_,id,_,_,_,_)) = x in id.idText
    member x.Range = let (Override(_,_,id,_,_,_,_)) = x in id.idRange
    member x.IsFakeEventProperty = let (Override(_,_,_,_,_,_,b)) = x in b
    member x.ArgTypes = let (Override(_,_,_,_,b,_,_)) = x in b
    member x.ReturnType = let (Override(_,_,_,_,_,b,_)) = x in b

// If the bool is true then the slot is optional, i.e. is an interface slot
// which does not _have_ to be implemented, because an inherited implementation 
// is available.
type RequiredSlot = RequiredSlot of MethInfo * (* isOptional: *) bool 

type SlotImplSet = SlotImplSet of RequiredSlot list * NameMultiMap<RequiredSlot> * OverrideInfo list * PropInfo list

exception TypeIsImplicitlyAbstract of range
exception OverrideDoesntOverride of DisplayEnv * OverrideInfo * MethInfo option * TcGlobals * Import.ImportMap * range

module DispatchSlotChecking =

    /// Print the signature of an override to a buffer as part of an error message
    let PrintOverrideToBuffer denv os (Override(_,_,id,(mtps,memberToParentInst),argTys,retTy,_)) = 
       let denv = { denv with showTyparBinding = true }
       let retTy = (retTy  |> GetFSharpViewOfReturnType denv.g)
       let argInfos = 
           match argTys with 
           | [] -> [[(denv.g.unit_ty,ValReprInfo.unnamedTopArg1)]]
           | _ -> argTys |> List.mapSquared (fun ty -> (ty, ValReprInfo.unnamedTopArg1)) 
       Layout.bufferL os (NicePrint.layoutMemberSig denv (memberToParentInst,id.idText,mtps, argInfos, retTy))

    /// Print the signature of a MethInfo to a buffer as part of an error message
    let PrintMethInfoSigToBuffer g amap m denv os minfo =
        let denv = { denv with showTyparBinding = true }
        let (CompiledSig(argTys,retTy,fmtps,ttpinst)) = CompiledSigOfMeth g amap m minfo
        let retTy = (retTy  |> GetFSharpViewOfReturnType g)
        let argInfos = argTys |> List.mapSquared (fun ty -> (ty, ValReprInfo.unnamedTopArg1))
        let nm = minfo.LogicalName
        Layout.bufferL os (NicePrint.layoutMemberSig denv (ttpinst,nm,fmtps, argInfos, retTy))

    /// Format the signature of an override as a string as part of an error message
    let FormatOverride denv d = bufs (fun buf -> PrintOverrideToBuffer denv buf d)

    /// Format the signature of a MethInfo as a string as part of an error message
    let FormatMethInfoSig g amap m denv d = bufs (fun buf -> PrintMethInfoSigToBuffer g amap m denv buf d)

    /// Get the override info for an existing (inherited) method being used to implement a dispatch slot.
    let GetInheritedMemberOverrideInfo g amap m parentType (minfo:MethInfo) = 
        let nm = minfo.LogicalName
        let (CompiledSig (argTys,retTy,fmtps,ttpinst)) = CompiledSigOfMeth g amap m minfo

        let isFakeEventProperty = minfo.IsFSharpEventPropertyMethod
        Override(parentType,tcrefOfAppTy g minfo.EnclosingType,mkSynId m nm, (fmtps,ttpinst),argTys,retTy,isFakeEventProperty)

    /// Get the override info for a value being used to implement a dispatch slot.
    let GetTypeMemberOverrideInfo g reqdTy (overrideBy:ValRef) = 
        let _,argInfos,retTy,_ = GetTypeOfMemberInMemberForm g overrideBy
        let nm = overrideBy.LogicalName

        let argTys = argInfos |> List.mapSquared fst
        
        let memberMethodTypars,memberToParentInst,argTys,retTy = 
            match PartitionValRefTypars g overrideBy with
            | Some(_,_,memberMethodTypars,memberToParentInst,_tinst) -> 
                let argTys = argTys |> List.mapSquared (instType memberToParentInst) 
                let retTy = retTy |> Option.map (instType memberToParentInst) 
                memberMethodTypars, memberToParentInst,argTys, retTy
            | None -> 
                error(Error(FSComp.SR.typrelMethodIsOverconstrained(),overrideBy.Range))
        let implKind = 
            if ValRefIsExplicitImpl g overrideBy then 
                
                let belongsToReqdTy = 
                    match overrideBy.MemberInfo.Value.ImplementedSlotSigs with
                    | [] -> false
                    | ss :: _ -> isInterfaceTy g ss.ImplementedType && typeEquiv g reqdTy ss.ImplementedType
                if belongsToReqdTy then 
                    CanImplementAnyInterfaceSlot
                else
                    CanImplementNoSlots
            else if overrideBy.IsDispatchSlotMember then 
                CanImplementNoSlots
                // abstract slots can only implement interface slots
                //CanImplementAnyInterfaceSlot  <<----- Change to this to enable implicit interface implementation
            
            else 
                CanImplementAnyClassHierarchySlot
                //CanImplementAnySlot  <<----- Change to this to enable implicit interface implementation

        let isFakeEventProperty = overrideBy.IsFSharpEventProperty(g)
        Override(implKind,overrideBy.MemberApparentParent, mkSynId overrideBy.Range nm, (memberMethodTypars,memberToParentInst),argTys,retTy,isFakeEventProperty)

    /// Get the override information for an object expression method being used to implement dispatch slots
    let GetObjectExprOverrideInfo g amap (implty, id:Ident, memberFlags, ty, arityInfo, bindingAttribs, rhsExpr) = 
        // Dissect the type
        let tps, argInfos, retTy, _ = GetMemberTypeInMemberForm g memberFlags arityInfo ty id.idRange
        let argTys = argInfos |> List.mapSquared fst
        // Dissect the implementation
        let _, ctorThisValOpt, baseValOpt, vsl, rhsExpr,_ = destTopLambda g amap arityInfo (rhsExpr,ty)
        assert ctorThisValOpt.IsNone

        // Drop 'this'
        match vsl with 
        | [thisv]::vs -> 
            // Check for empty variable list from a () arg
            let vs = if vs.Length = 1 && argInfos.IsEmpty then [] else vs
            let implKind = 
                if isInterfaceTy g implty then 
                    CanImplementAnyInterfaceSlot 
                else 
                    CanImplementAnyClassHierarchySlot
                    //CanImplementAnySlot  <<----- Change to this to enable implicit interface implementation
            let isFakeEventProperty = CompileAsEvent g bindingAttribs
            let overrideByInfo = Override(implKind, tcrefOfAppTy g implty, id, (tps,[]), argTys, retTy, isFakeEventProperty)
            overrideByInfo, (baseValOpt, thisv, vs, bindingAttribs, rhsExpr)
        | _ -> 
            error(InternalError("Unexpected shape for object expression override",id.idRange))
          
    /// Check if an override matches a dispatch slot by name
    let IsNameMatch (dispatchSlot:MethInfo) (overrideBy: OverrideInfo) = 
        (overrideBy.LogicalName = dispatchSlot.LogicalName)
          
    /// Check if an override matches a dispatch slot by name
    let IsImplMatch g (dispatchSlot:MethInfo) (overrideBy: OverrideInfo) = 
        // If the override is listed as only relevant to one type, and we're matching it against an abstract slot of an interface type,
        // then check that interface type is the right type.
        (match overrideBy.CanImplement with 
         | CanImplementNoSlots -> false
         | CanImplementAnySlot -> true 
         | CanImplementAnyClassHierarchySlot -> not (isInterfaceTy g dispatchSlot.EnclosingType)
         //| CanImplementSpecificInterfaceSlot parentTy -> isInterfaceTy g dispatchSlot.EnclosingType && typeEquiv g parentTy dispatchSlot.EnclosingType 
         | CanImplementAnyInterfaceSlot -> isInterfaceTy g dispatchSlot.EnclosingType)

    /// Check if the kinds of type parameters match between a dispatch slot and an override.
    let IsTyparKindMatch g amap m (dispatchSlot:MethInfo) (Override(_,_,_,(mtps,_),_,_,_)) = 
        let (CompiledSig(_,_,fvmtps,_)) = CompiledSigOfMeth g amap m dispatchSlot 
        List.lengthsEqAndForall2 (fun (tp1:Typar) (tp2:Typar) -> tp1.Kind = tp2.Kind) mtps fvmtps
        
    /// Check if an override is a partial match for the requirements for a dispatch slot 
    let IsPartialMatch g amap m (dispatchSlot:MethInfo) (Override(_,_,_,(mtps,_),argTys,_retTy,_) as overrideBy) = 
        IsNameMatch dispatchSlot overrideBy &&
        let (CompiledSig (vargtys,_,fvmtps,_)) = CompiledSigOfMeth g amap m dispatchSlot 
        mtps.Length = fvmtps.Length &&
        IsTyparKindMatch g amap m dispatchSlot overrideBy && 
        argTys.Length = vargtys.Length &&
        IsImplMatch g dispatchSlot overrideBy  
          
    /// Compute the reverse of a type parameter renaming.
    let ReverseTyparRenaming g tinst = 
        tinst |> List.map (fun (tp,ty) -> (destTyparTy g ty, mkTyparTy tp))

    /// Compose two instantiations of type parameters.
    let ComposeTyparInsts inst1 inst2 = 
        inst1 |> List.map (map2Of2 (instType inst2)) 
     
    /// Check if an override exactly matches the requirements for a dispatch slot 
    let IsExactMatch g amap m dispatchSlot (Override(_,_,_,(mtps,mtpinst),argTys,retTy,_) as overrideBy) =
        IsPartialMatch g amap m dispatchSlot overrideBy &&
        let (CompiledSig (vargtys,vrty,fvmtps,ttpinst)) = CompiledSigOfMeth g amap m dispatchSlot

        // Compare the types. CompiledSigOfMeth, GetObjectExprOverrideInfo and GetTypeMemberOverrideInfo have already 
        // applied all relevant substitutions except the renamings from fvtmps <-> mtps 

        let aenv = TypeEquivEnv.FromEquivTypars fvmtps mtps 

        List.forall2 (List.lengthsEqAndForall2 (typeAEquiv g aenv)) vargtys argTys &&
        returnTypesAEquiv g aenv vrty retTy &&
        
        // Comparing the method typars and their constraints is much trickier since the substitutions have not been applied 
        // to the constraints of these babies. This is partly because constraints are directly attached to typars so it's 
        // difficult to apply substitutions to them unless we separate them off at some point, which we don't as yet.        
        //
        // Given   C<ctps>
        //         D<dtps>
        //         dispatchSlot :   C<ctys[dtps]>.M<fvmtps[ctps]>(...)
        //         overrideBy:  parent: D<dtys[dtps]>  value: !<ttps> <mtps[ttps]>(...) 
        //         
        //     where X[dtps] indicates that X may involve free type variables dtps
        //     
        //     we have 
        //         ttpinst maps  ctps --> ctys[dtps] 
        //         mtpinst maps  ttps --> dtps
        //       
        //     compare fvtmps[ctps] and mtps[ttps] by 
        //        fvtmps[ctps]  @ ttpinst     -- gives fvtmps[dtps]
        //        fvtmps[dtps] @ rev(mtpinst) -- gives fvtmps[ttps]
        //        
        //     Now fvtmps[ttps] and mtpinst[ttps] are comparable, i.e. have contraints w.r.t. the same set of type variables 
        //         
        // i.e.  Compose the substitutions ttpinst and rev(mtpinst) 
        
        let ttpinst = 
            // check we can reverse - in some error recovery situations we can't 
            if mtpinst |> List.exists (snd >> isTyparTy g >> not) then ttpinst 
            else ComposeTyparInsts ttpinst (ReverseTyparRenaming g mtpinst)

        // Compare under the composed substitutions 
        let aenv = TypeEquivEnv.FromTyparInst ttpinst 
        
        typarsAEquiv g aenv fvmtps mtps

    /// Check if an override implements a dispatch slot 
    let OverrideImplementsDispatchSlot g amap m dispatchSlot availPriorOverride =
        IsExactMatch g amap m dispatchSlot availPriorOverride &&
        // The override has to actually be in some subtype of the dispatch slot
        ExistsHeadTypeInEntireHierarchy g amap m (generalizedTyconRef availPriorOverride.BoundingTyconRef) (tcrefOfAppTy g dispatchSlot.EnclosingType)

    /// Check if a dispatch slot is already implemented
    let DispatchSlotIsAlreadyImplemented g amap m availPriorOverridesKeyed (dispatchSlot: MethInfo) =
        availPriorOverridesKeyed 
            |> NameMultiMap.find  dispatchSlot.LogicalName  
            |> List.exists (OverrideImplementsDispatchSlot g amap m dispatchSlot)


    /// Check all dispatch slots are implemented by some override.
    let CheckDispatchSlotsAreImplemented (denv,g,amap,m,
                                          isOverallTyAbstract,
                                          reqdTy,
                                          dispatchSlots:RequiredSlot list,
                                          availPriorOverrides:OverrideInfo list,
                                          overrides:OverrideInfo list) = 

        let isReqdTyInterface = isInterfaceTy g reqdTy 
        let showMissingMethodsAndRaiseErrors = (isReqdTyInterface || not isOverallTyAbstract)
        let res = ref true
        let fail exn = (res := false ; if showMissingMethodsAndRaiseErrors then errorR exn)
        
        // Index the availPriorOverrides and overrides by name
        let availPriorOverridesKeyed = availPriorOverrides |> NameMultiMap.initBy (fun ov -> ov.LogicalName)
        let overridesKeyed = overrides |> NameMultiMap.initBy (fun ov -> ov.LogicalName)
        
        dispatchSlots |> List.iter (fun (RequiredSlot(dispatchSlot,isOptional)) -> 
          
            match NameMultiMap.find dispatchSlot.LogicalName overridesKeyed 
                    |> List.filter (OverrideImplementsDispatchSlot g amap m dispatchSlot)  with
            | [_] -> 
                ()
            | [] -> 
                if not isOptional && 
                   // Check that no available prior override implements this dispatch slot
                   not (DispatchSlotIsAlreadyImplemented g amap m availPriorOverridesKeyed dispatchSlot) then 
                    // error reporting path 
                    let (CompiledSig (vargtys,_vrty,fvmtps,_)) = CompiledSigOfMeth g amap m dispatchSlot
                    let noimpl() = if isReqdTyInterface then fail(Error(FSComp.SR.typrelNoImplementationGivenWithSuggestion(NicePrint.stringOfMethInfo amap m denv dispatchSlot), m))
                                   else fail(Error(FSComp.SR.typrelNoImplementationGiven(NicePrint.stringOfMethInfo amap m denv dispatchSlot), m))
                    match  overrides |> List.filter (IsPartialMatch g amap m dispatchSlot)  with 
                    | [] -> 
                        match overrides |> List.filter (fun overrideBy -> IsNameMatch dispatchSlot overrideBy && 
                                                                            IsImplMatch g dispatchSlot overrideBy)  with 
                        | [] -> 
                            noimpl()
                        | [ Override(_,_,_,(mtps,_),argTys,_,_) as overrideBy ] -> 
                            let error_msg = 
                                if argTys.Length <> vargtys.Length then FSComp.SR.typrelMemberDoesNotHaveCorrectNumberOfArguments(FormatOverride denv overrideBy, FormatMethInfoSig g amap m denv dispatchSlot)
                                elif mtps.Length <> fvmtps.Length then FSComp.SR.typrelMemberDoesNotHaveCorrectNumberOfTypeParameters(FormatOverride denv overrideBy, FormatMethInfoSig g amap m denv dispatchSlot)
                                elif not (IsTyparKindMatch g amap m dispatchSlot overrideBy) then  FSComp.SR.typrelMemberDoesNotHaveCorrectKindsOfGenericParameters(FormatOverride denv overrideBy, FormatMethInfoSig g amap m denv dispatchSlot)
                                else FSComp.SR.typrelMemberCannotImplement(FormatOverride denv overrideBy, NicePrint.stringOfMethInfo amap m denv dispatchSlot, FormatMethInfoSig g amap m denv dispatchSlot)
                            fail(Error(error_msg, overrideBy.Range))
                        | overrideBy :: _ -> 
                            errorR(Error(FSComp.SR.typrelOverloadNotFound(FormatMethInfoSig g amap m denv dispatchSlot, FormatMethInfoSig g amap m denv dispatchSlot),overrideBy.Range))

                    | [ overrideBy ] -> 

                        match dispatchSlots  |> List.filter (fun (RequiredSlot(dispatchSlot,_)) -> OverrideImplementsDispatchSlot g amap m dispatchSlot overrideBy) with 
                        | [] -> 
                            // Error will be reported below in CheckOverridesAreAllUsedOnce 
                            ()
                        | _ -> 
                            noimpl()
                        
                    | _ -> 
                        fail(Error(FSComp.SR.typrelOverrideWasAmbiguous(FormatMethInfoSig g amap m denv dispatchSlot),m))
            | _ -> fail(Error(FSComp.SR.typrelMoreThenOneOverride(FormatMethInfoSig g amap m denv dispatchSlot),m)))
        !res

    /// Check all implementations implement some dispatch slot.
    let CheckOverridesAreAllUsedOnce(denv, g, amap, isObjExpr, reqdTy,
                                     dispatchSlotsKeyed: NameMultiMap<RequiredSlot>,
                                     availPriorOverrides: OverrideInfo list,
                                     overrides: OverrideInfo list) = 
        let availPriorOverridesKeyed = availPriorOverrides |> NameMultiMap.initBy (fun ov -> ov.LogicalName)
        for overrideBy in overrides do 
          if not overrideBy.IsFakeEventProperty then
            let m = overrideBy.Range
            let relevantVirts = NameMultiMap.find overrideBy.LogicalName dispatchSlotsKeyed
            let relevantVirts = relevantVirts |> List.map (fun (RequiredSlot(dispatchSlot,_)) -> dispatchSlot)

            match relevantVirts |> List.filter (fun dispatchSlot -> OverrideImplementsDispatchSlot g amap m dispatchSlot overrideBy) with
            | [] -> 
                // This is all error reporting
                match relevantVirts |> List.filter (fun dispatchSlot -> IsPartialMatch g amap m dispatchSlot overrideBy) with 
                | [dispatchSlot] -> 
                    errorR(OverrideDoesntOverride(denv,overrideBy,Some dispatchSlot,g,amap,m))
                | _ -> 
                    match relevantVirts |> List.filter (fun dispatchSlot -> IsNameMatch dispatchSlot overrideBy) with 
                    | [dispatchSlot] -> 
                        errorR(OverrideDoesntOverride(denv, overrideBy, Some dispatchSlot, g, amap, m))
                    | _ -> 
                        errorR(OverrideDoesntOverride(denv,overrideBy,None,g,amap,m))


            | [dispatchSlot] -> 
                if dispatchSlot.IsFinal && (isObjExpr || not (typeEquiv g reqdTy dispatchSlot.EnclosingType)) then 
                    errorR(Error(FSComp.SR.typrelMethodIsSealed(NicePrint.stringOfMethInfo amap m denv dispatchSlot),m))
            | dispatchSlots -> 
                match dispatchSlots |> List.filter (fun dispatchSlot -> 
                              isInterfaceTy g dispatchSlot.EnclosingType || 
                              not (DispatchSlotIsAlreadyImplemented g amap m availPriorOverridesKeyed dispatchSlot)) with
                | h1 :: h2 :: _ -> 
                    errorR(Error(FSComp.SR.typrelOverrideImplementsMoreThenOneSlot((FormatOverride denv overrideBy), (NicePrint.stringOfMethInfo amap m denv h1), (NicePrint.stringOfMethInfo amap m denv h2)),m))
                | _ -> 
                    // dispatch slots are ordered from the derived classes to base
                    // so we can check the topmost dispatch slot if it is final
                    match dispatchSlots with
                    | meth::_ when meth.IsFinal -> errorR(Error(FSComp.SR.tcCannotOverrideSealedMethod((sprintf "%s::%s" (meth.EnclosingType.ToString()) (meth.LogicalName))), m))
                    | _ -> ()



    /// Get the slots of a type that can or must be implemented. This depends
    /// partly on the full set of interface types that are being implemented
    /// simultaneously, e.g.
    ///    { new C with  interface I2 = ... interface I3 = ... }
    /// allReqdTys = {C;I2;I3}
    ///
    /// allReqdTys can include one class/record/union type. 
    let GetSlotImplSets (infoReader:InfoReader) denv isObjExpr allReqdTys = 

        let g = infoReader.g
        let amap = infoReader.amap
        
        let availImpliedInterfaces : TType list = 
            [ for (reqdTy,m) in allReqdTys do
                if not (isInterfaceTy g reqdTy) then 
                    let baseTyOpt = if isObjExpr then Some reqdTy else GetSuperTypeOfType g amap m reqdTy 
                    match baseTyOpt with 
                    | None -> ()
                    | Some baseTy -> yield! AllInterfacesOfType g amap m AllowMultiIntfInstantiations.Yes baseTy  ]
                    
        // For each implemented type, get a list containing the transitive closure of
        // interface types implied by the type. This includes the implemented type itself if the implemented type
        // is an interface type.
        let intfSets = 
            allReqdTys |> List.mapi (fun i (reqdTy,m) -> 
                let interfaces = AllInterfacesOfType g amap m AllowMultiIntfInstantiations.Yes reqdTy 
                let impliedTys = (if isInterfaceTy g reqdTy then interfaces else reqdTy :: interfaces)
                (i, reqdTy, impliedTys,m))

        // For each implemented type, reduce its list of implied interfaces by subtracting out those implied 
        // by another implemented interface type.
        //
        // REVIEW: Note complexity O(ity*jty)
        let reqdTyInfos = 
            intfSets |> List.map (fun (i,reqdTy,impliedTys,m) -> 
                let reduced = 
                    (impliedTys,intfSets) ||> List.fold (fun acc (j,jty,impliedTys2,m) -> 
                         if i <> j && TypeFeasiblySubsumesType 0 g amap m jty CanCoerce reqdTy 
                         then ListSet.subtract (TypesFeasiblyEquiv 0 g amap m) acc impliedTys2
                         else acc ) 
                (i, reqdTy, m, reduced))

        // Check that, for each implemented type, at least one implemented type is implied. This is enough to capture
        // duplicates.
        for (_i, reqdTy, m, impliedTys) in reqdTyInfos do
            if isInterfaceTy g reqdTy && isNil impliedTys then 
                errorR(Error(FSComp.SR.typrelDuplicateInterface(),m))

        // Check that no interface type is implied twice
        //
        // Note complexity O(reqdTy*reqdTy)
        for (i, _reqdTy, reqdTyRange, impliedTys) in reqdTyInfos do
            for (j,_,_,impliedTys2) in reqdTyInfos do
                if i > j then  
                    let overlap = ListSet.intersect (TypesFeasiblyEquiv 0 g amap reqdTyRange) impliedTys impliedTys2
                    overlap |> List.iter (fun overlappingTy -> 
                        if nonNil(GetImmediateIntrinsicMethInfosOfType (None,AccessibleFromSomewhere) g amap reqdTyRange overlappingTy |> List.filter (fun minfo -> minfo.IsVirtual)) then                                
                            errorR(Error(FSComp.SR.typrelNeedExplicitImplementation(NicePrint.minimalStringOfType denv (List.head overlap)),reqdTyRange)))

        // Get the SlotImplSet for each implemented type
        // This contains the list of required members and the list of available members
        [ for (_,reqdTy,reqdTyRange,impliedTys) in reqdTyInfos do

            // Build a set of the implied interface types, for quicker lookup, by nominal type
            let isImpliedInterfaceTable = 
                impliedTys 
                |> List.filter (isInterfaceTy g) 
                |> List.map (fun ty -> tcrefOfAppTy g ty, ()) 
                |> TyconRefMap.OfList 
            
            // Is a member an abstract slot of one of the implied interface types?
            let isImpliedInterfaceType ty =
                isImpliedInterfaceTable.ContainsKey (tcrefOfAppTy g ty) &&
                impliedTys |> List.exists (TypesFeasiblyEquiv 0 g amap reqdTyRange ty)

            //let isSlotImpl (minfo:MethInfo) = 
            //    not minfo.IsAbstract && minfo.IsVirtual 

            // Compute the abstract slots that require implementations
            let dispatchSlots = 
                [ if isInterfaceTy g reqdTy then 
                      for impliedTy in impliedTys  do
                          // Check if the interface has an inherited implementation
                          // If so, you do not have to implement all the methods - each
                          // specific method is "optionally" implemented.
                          let isOptional = 
                              ListSet.contains (typeEquiv g) impliedTy availImpliedInterfaces
                          for reqdSlot in GetImmediateIntrinsicMethInfosOfType (None,AccessibleFromSomewhere) g amap reqdTyRange impliedTy do
                              yield RequiredSlot(reqdSlot, isOptional)
                  else
                      
                      // In the normal case, the requirements for a class are precisely all the abstract slots up the whole hierarchy.
                      // So here we get and yield all of those.
                      for minfo in reqdTy |> GetIntrinsicMethInfosOfType infoReader (None,AccessibleFromSomewhere,AllowMultiIntfInstantiations.Yes) IgnoreOverrides reqdTyRange do
                         if minfo.IsDispatchSlot then
                             yield RequiredSlot(minfo,(*isOptional=*)false) ]
                
                
            // Compute the methods that are available to implement abstract slots from the base class
            //
            // This is used in CheckDispatchSlotsAreImplemented when we think a dispatch slot may not
            // have been implemented. 
            let availPriorOverrides : OverrideInfo list = 
                if isInterfaceTy g reqdTy then 
                    []
                else 
                    let reqdTy = 
                        let baseTyOpt = if isObjExpr then Some reqdTy else GetSuperTypeOfType g amap reqdTyRange reqdTy 
                        match baseTyOpt with 
                        | None -> reqdTy
                        | Some baseTy -> baseTy 
                    [ // Get any class hierarchy methods on this type 
                      //
                      // NOTE: What we have below is an over-approximation that will get too many methods 
                      // and not always correctly relate them to the slots they implement. For example, 
                      // we may get an override from a base class and believe it implements a fresh, new abstract
                      // slot in a subclass. 
                      for minfos in infoReader.GetRawIntrinsicMethodSetsOfType(None,AccessibleFromSomewhere,AllowMultiIntfInstantiations.Yes,reqdTyRange,reqdTy) do
                        for minfo in minfos do
                          if not minfo.IsAbstract then 
                              yield GetInheritedMemberOverrideInfo g amap reqdTyRange CanImplementAnyClassHierarchySlot minfo   ]
                     
            // We also collect up the properties. This is used for abstract slot inference when overriding properties
            let isRelevantRequiredProperty (x:PropInfo) = 
                (x.IsVirtualProperty && not (isInterfaceTy g reqdTy)) ||
                isImpliedInterfaceType x.EnclosingType
                
            let reqdProperties = 
                GetIntrinsicPropInfosOfType infoReader (None,AccessibleFromSomewhere,AllowMultiIntfInstantiations.Yes) IgnoreOverrides reqdTyRange reqdTy 
                |> List.filter isRelevantRequiredProperty
                
            let dispatchSlotsKeyed = dispatchSlots |> NameMultiMap.initBy (fun (RequiredSlot(v,_)) -> v.LogicalName) 
            yield SlotImplSet(dispatchSlots, dispatchSlotsKeyed, availPriorOverrides, reqdProperties) ]


    /// Check that a type definition implements all its required interfaces after processing all declarations 
    /// within a file.
    let CheckImplementationRelationAtEndOfInferenceScope (infoReader:InfoReader,denv,tycon:Tycon,isImplementation) =

        let g = infoReader.g
        let amap = infoReader.amap

        let tcaug = tycon.TypeContents        
        let interfaces = tycon.ImmediateInterfacesOfFSharpTycon |> List.map (fun (ity,_compgen,m) -> (ity,m))

        let overallTy = generalizedTyconRef (mkLocalTyconRef tycon)

        let allReqdTys = (overallTy,tycon.Range) :: interfaces 

        // Get all the members that are immediately part of this type
        // Include the auto-generated members
        let allImmediateMembers = tycon.MembersOfFSharpTyconSorted @ tycon.AllGeneratedValues

        // Get all the members we have to implement, organized by each type we explicitly implement
        let slotImplSets = GetSlotImplSets infoReader denv false allReqdTys

        let allImpls = List.zip allReqdTys slotImplSets

        // Find the methods relevant to implementing the abstract slots listed under the reqdType being checked.
        let allImmediateMembersThatMightImplementDispatchSlots = 
            allImmediateMembers |> List.filter (fun overrideBy -> 
                overrideBy.IsInstanceMember   &&  // exclude static
                overrideBy.IsVirtualMember &&  // exclude non virtual (e.g. keep override/default). [4469]
                not overrideBy.IsDispatchSlotMember)

        let mustOverrideSomething reqdTy (overrideBy:ValRef) =
           let memberInfo = overrideBy.MemberInfo.Value
           not (overrideBy.IsFSharpEventProperty(g)) &&
           memberInfo.MemberFlags.IsOverrideOrExplicitImpl && 
    
           match memberInfo.ImplementedSlotSigs with 
           | [] -> 
                // Are we looking at the implementation of the class hierarchy? If so include all the override members
                not (isInterfaceTy g reqdTy)
           | ss -> 
                 ss |> List.forall (fun ss -> 
                     let ty = ss.ImplementedType
                     if isInterfaceTy g ty then 
                         // Is this a method impl listed under the reqdTy?
                         typeEquiv g ty reqdTy
                     else
                         not (isInterfaceTy g reqdTy) )
        

        // We check all the abstracts related to the class hierarchy and then check each interface implementation
        for ((reqdTy,m),slotImplSet) in allImpls do
            let (SlotImplSet(dispatchSlots, dispatchSlotsKeyed, availPriorOverrides,_)) = slotImplSet
            try 

                // Now extract the information about each overriding method relevant to this SlotImplSet
                let allImmediateMembersThatMightImplementDispatchSlots = 
                    allImmediateMembersThatMightImplementDispatchSlots
                    |> List.map (fun overrideBy -> overrideBy,GetTypeMemberOverrideInfo g reqdTy overrideBy)
                
                // Now check the implementation
                // We don't give missing method errors for abstract classes 
                
                if isImplementation && not (isInterfaceTy g overallTy) then 
                    let overrides = allImmediateMembersThatMightImplementDispatchSlots |> List.map snd 
                    let allCorrect = CheckDispatchSlotsAreImplemented (denv,g,amap,m,tcaug.tcaug_abstract,reqdTy,dispatchSlots,availPriorOverrides,overrides)
                    
                    // Tell the user to mark the thing abstract if it was missing implementations
                    if not allCorrect && not tcaug.tcaug_abstract && not (isInterfaceTy g reqdTy) then 
                        errorR(TypeIsImplicitlyAbstract(m))
                    
                    let overridesToCheck = 
                        allImmediateMembersThatMightImplementDispatchSlots 
                           |> List.filter (fst >> mustOverrideSomething reqdTy)
                           |> List.map snd

                    CheckOverridesAreAllUsedOnce (denv, g, amap, false, reqdTy, dispatchSlotsKeyed, availPriorOverrides, overridesToCheck)

            with e -> errorRecovery e m

        // Now record the full slotsigs of the abstract members implemented by each override.
        // This is used to generate IL MethodImpls in the code generator.
        allImmediateMembersThatMightImplementDispatchSlots |> List.iter (fun overrideBy -> 

            let isFakeEventProperty = overrideBy.IsFSharpEventProperty(g)
            if not isFakeEventProperty then 
                
                let overriden = 
                    [ for ((reqdTy,m),(SlotImplSet(_dispatchSlots,dispatchSlotsKeyed,_,_))) in allImpls do
                          let overrideByInfo = GetTypeMemberOverrideInfo g reqdTy overrideBy
                          let overridenForThisSlotImplSet = 
                              [ for (RequiredSlot(dispatchSlot,_)) in NameMultiMap.find overrideByInfo.LogicalName dispatchSlotsKeyed do 
                                    if OverrideImplementsDispatchSlot g amap m dispatchSlot overrideByInfo then 
                                        if tyconRefEq g overrideByInfo.BoundingTyconRef (tcrefOfAppTy g dispatchSlot.EnclosingType) then 
                                             match dispatchSlot.ArbitraryValRef with 
                                             | Some virtMember -> 
                                                  if virtMember.MemberInfo.Value.IsImplemented then errorR(Error(FSComp.SR.tcDefaultImplementationAlreadyExists(),overrideByInfo.Range))
                                                  virtMember.MemberInfo.Value.IsImplemented <- true
                                             | None -> () // not an F# slot

                                        // Get the slotsig of the overriden method 
                                        let slotsig = dispatchSlot.GetSlotSig(amap, m)

                                        // The slotsig from the overriden method is in terms of the type parameters on the parent type of the overriding method,
                                        // Modify map the slotsig so it is in terms of the type parameters for the overriding method 
                                        let slotsig = ReparentSlotSigToUseMethodTypars g m overrideBy slotsig
                     
                                        // Record the slotsig via mutation 
                                        yield slotsig ] 
                          //if mustOverrideSomething reqdTy overrideBy then 
                          //    assert nonNil overridenForThisSlotImplSet
                          yield! overridenForThisSlotImplSet ]
                
                overrideBy.MemberInfo.Value.ImplementedSlotSigs <- overriden)


//-------------------------------------------------------------------------
// Sets of methods involved in overload resolution and trait constraint
// satisfaction.
//------------------------------------------------------------------------- 

/// In the following, 'T gets instantiated to: 
///   1. the expression being supplied for an argument 
///   2. "unit", when simply checking for the existence of an overload that satisfies 
///      a signature, or when finding the corresponding witness. 
/// Note the parametricity helps ensure that overload resolution doesn't depend on the 
/// expression on the callside (though it is in some circumstances allowed 
/// to depend on some type information inferred syntactically from that 
/// expression, e.g. a lambda expression may be converted to a delegate as 
/// an adhoc conversion. 
///
/// The bool indicates if named using a '?' 
type CallerArg<'T> = 
    /// CallerArg(ty, range, isOpt, exprInfo)
    | CallerArg of TType * range * bool * 'T  
    member x.Type = (let (CallerArg(ty,_,_,_)) = x in ty)
    member x.Range = (let (CallerArg(_,m,_,_)) = x in m)
    member x.IsOptional = (let (CallerArg(_,_,isOpt,_)) = x in isOpt)
    member x.Expr = (let (CallerArg(_,_,_,expr)) = x in expr)
    
/// Represents the information about an argument in the method being called
type CalledArg = 
    { Position: (int * int)
      IsParamArray : bool
      OptArgInfo : OptionalArgInfo
      IsOutArg: bool
      ReflArgInfo: ReflectedArgInfo
      NameOpt: string option
      CalledArgumentType : TType }

let CalledArg(pos,isParamArray,optArgInfo,isOutArg,nameOpt,reflArgInfo,calledArgTy) =
    { Position=pos
      IsParamArray=isParamArray
      OptArgInfo =optArgInfo
      IsOutArg=isOutArg
      ReflArgInfo=reflArgInfo
      NameOpt=nameOpt
      CalledArgumentType = calledArgTy }

/// Represents a match between a caller argument and a called argument, arising from either
/// a named argument or an unnamed argument.
type AssignedCalledArg<'T> = 
    { /// The identifier for a named argument, if any
      NamedArgIdOpt : Ident option
      /// The called argument in the method
      CalledArg: CalledArg 
      /// The argument on the caller side
      CallerArg: CallerArg<'T> }
    member x.Position = x.CalledArg.Position

/// Represents the possibilities for a named-setter argument (a property, field , or a record field setter)
type AssignedItemSetterTarget = 
    | AssignedPropSetter of PropInfo * MethInfo * TypeInst   (* the MethInfo is a non-indexer setter property *)
    | AssignedILFieldSetter of ILFieldInfo 
    | AssignedRecdFieldSetter of RecdFieldInfo 

/// Represents the resolution of a caller argument as a named-setter argument
type AssignedItemSetter<'T> = AssignedItemSetter of Ident * AssignedItemSetterTarget * CallerArg<'T> 

type CallerNamedArg<'T> = 
    | CallerNamedArg of Ident * CallerArg<'T>  
    member x.Ident = (let (CallerNamedArg(id,_)) = x in id)
    member x.Name = x.Ident.idText
    member x.CallerArg = (let (CallerNamedArg(_,a)) = x in a)

//-------------------------------------------------------------------------
// Callsite conversions
//------------------------------------------------------------------------- 

// F# supports three adhoc conversions at method callsites (note C# supports more, though ones 
// such as implicit conversions interact badly with type inference). 
//
// 1. The use of "(fun x y -> ...)" when  a delegate it expected. This is not part of 
// the ":>" coercion relationship or inference constraint problem as 
// such, but is a special rule applied only to method arguments. 
// 
// The function AdjustCalledArgType detects this case based on types and needs to know that the type being applied 
// is a function type. 
// 
// 2. The use of "(fun x y -> ...)" when Expression<delegate> it expected. This is similar to above.
// 
// 3. Two ways to pass a value where a byref is expected. The first (default) 
// is to use a reference cell, and the interior address is taken automatically 
// The second is an explicit use of the "address-of" operator "&e". Here we detect the second case, 
// and record the presence of the sytnax "&e" in the pre-inferred actual type for the method argument. 
// The function AdjustCalledArgType detects this and refuses to apply the default byref-to-ref transformation. 
//
// The function AdjustCalledArgType also adjusts for optional arguments. 
let AdjustCalledArgType (infoReader:InfoReader) isConstraint (calledArg: CalledArg) (callerArg: CallerArg<_>)  =
    let g = infoReader.g
    // #424218 - when overload resolution is part of constraint solving - do not perform type-directed conversions
    let calledArgTy = calledArg.CalledArgumentType
    let callerArgTy = callerArg.Type
    let m = callerArg.Range
    if isConstraint then calledArgTy else
    // If the called method argument is a byref type, then the caller may provide a byref or ref 
    if isByrefTy g calledArgTy then
        if isByrefTy g callerArgTy then 
            calledArgTy
        else 
            mkRefCellTy g (destByrefTy g calledArgTy)  
    else 
        // If the called method argument is a delegate type, then the caller may provide a function 
        let calledArgTy = 
            let adjustDelegateTy calledTy =
                let (SigOfFunctionForDelegate(_,delArgTys,_,fty)) = GetSigOfFunctionForDelegate infoReader calledTy m  AccessibleFromSomeFSharpCode
                let delArgTys = (if isNil delArgTys then [g.unit_ty] else delArgTys)
                if (fst (stripFunTy g callerArgTy)).Length = delArgTys.Length
                then fty 
                else calledArgTy 

            if isDelegateTy g calledArgTy && isFunTy g callerArgTy then 
                adjustDelegateTy calledArgTy

            elif isLinqExpressionTy g calledArgTy && isFunTy g callerArgTy then 
                let origArgTy = calledArgTy
                let calledArgTy = destLinqExpressionTy g calledArgTy
                if isDelegateTy g calledArgTy then 
                    adjustDelegateTy calledArgTy
                else
                    // BUG 435170: called arg is Expr<'t> where 't is not delegate - such conversion is not legal -> return original type
                    origArgTy

            elif calledArg.ReflArgInfo.AutoQuote && isQuotedExprTy g calledArgTy && not (isQuotedExprTy g callerArgTy) then 
                destQuotedExprTy g calledArgTy

            else calledArgTy

        // Adjust the called argument type to take into account whether the caller's argument is M(?arg=Some(3)) or M(arg=1) 
        // If the called method argument is optional with type Option<T>, then the caller may provide a T, unless their argument is propogating-optional (i.e. isOptCallerArg) 
        let calledArgTy = 
            match calledArg.OptArgInfo with 
            | NotOptional                    -> calledArgTy
            | CalleeSide when not callerArg.IsOptional && isOptionTy g calledArgTy  -> destOptionTy g calledArgTy
            | CalleeSide | CallerSide _ -> calledArgTy
        calledArgTy
        

//-------------------------------------------------------------------------
// CalledMeth
//------------------------------------------------------------------------- 

type CalledMethArgSet<'T> = 
    { /// The called arguments corresponding to "unnamed" arguments
      UnnamedCalledArgs : CalledArg list
      /// Any unnamed caller arguments not otherwise assigned 
      UnnamedCallerArgs :  CallerArg<'T> list
      /// The called "ParamArray" argument, if any
      ParamArrayCalledArgOpt : CalledArg option 
      /// Any unnamed caller arguments assigned to a "param array" argument
      ParamArrayCallerArgs : CallerArg<'T> list
      /// Named args
      AssignedNamedArgs: AssignedCalledArg<'T> list  }
    member x.NumUnnamedCallerArgs = x.UnnamedCallerArgs.Length
    member x.NumAssignedNamedArgs = x.AssignedNamedArgs.Length
    member x.NumUnnamedCalledArgs = x.UnnamedCalledArgs.Length


let MakeCalledArgs amap m (minfo:MethInfo) minst =
    // Mark up the arguments with their position, so we can sort them back into order later 
    let paramDatas = minfo.GetParamDatas(amap, m, minst)
    paramDatas |> List.mapiSquared (fun i j (ParamData(isParamArrayArg,isOutArg,optArgInfo,nmOpt,reflArgInfo,typeOfCalledArg))  -> 
      { Position=(i,j)
        IsParamArray=isParamArrayArg
        OptArgInfo=optArgInfo
        IsOutArg=isOutArg
        ReflArgInfo=reflArgInfo
        NameOpt=nmOpt
        CalledArgumentType=typeOfCalledArg })

/// Represents the syntactic matching between a caller of a method and the called method.
///
/// The constructor takes all the information about the caller and called side of a method, match up named arguments, property setters etc., 
/// and returns a CalledMeth object for further analysis.
type CalledMeth<'T>
      (infoReader:InfoReader,
       nameEnv: Microsoft.FSharp.Compiler.Nameres.NameResolutionEnv option,
       isCheckingAttributeCall, 
       freshenMethInfo,// a function to help generate fresh type variables the property setters methods in generic classes 
       m, 
       ad,                // the access domain of the place where the call is taking place
       minfo:MethInfo,    // the method we're attempting to call 
       calledTyArgs,      // the 'called type arguments', i.e. the fresh generic instantiation of the method we're attempting to call 
       callerTyArgs: TType list, // the 'caller type arguments', i.e. user-given generic instantiation of the method we're attempting to call 
       pinfoOpt: PropInfo option,   // the property related to the method we're attempting to call, if any  
       callerObjArgTys: TType list,   // the types of the actual object argument, if any 
       curriedCallerArgs: (CallerArg<'T> list * CallerNamedArg<'T> list) list,     // the data about any arguments supplied by the caller 
       allowParamArgs:bool,       // do we allow the use of a param args method in its "expanded" form?
       allowOutAndOptArgs: bool,  // do we allow the use of the transformation that converts out arguments as tuple returns?
       tyargsOpt : TType option) // method parameters
    =
    let g = infoReader.g
    let methodRetTy = minfo.GetFSharpReturnTy(infoReader.amap, m, calledTyArgs)

    let fullCurriedCalledArgs = MakeCalledArgs infoReader.amap m minfo calledTyArgs
    do assert (fullCurriedCalledArgs.Length = fullCurriedCalledArgs.Length)
 
    let argSetInfos = 
        (curriedCallerArgs, fullCurriedCalledArgs) ||> List.map2 (fun (unnamedCallerArgs,namedCallerArgs) fullCalledArgs -> 
            // Find the arguments not given by name 
            let unnamedCalledArgs = 
                fullCalledArgs |> List.filter (fun calledArg -> 
                    match calledArg.NameOpt with 
                    | Some nm -> namedCallerArgs |> List.forall (fun (CallerNamedArg(nm2,_e)) -> nm <> nm2.idText)   
                    | None -> true)

            // See if any of them are 'out' arguments being returned as part of a return tuple 
            let unnamedCalledArgs, unnamedCalledOptArgs, unnamedCalledOutArgs = 
                let nUnnamedCallerArgs = unnamedCallerArgs.Length
                if allowOutAndOptArgs && nUnnamedCallerArgs < unnamedCalledArgs.Length then
                    let unnamedCalledArgsTrimmed,unnamedCalledOptOrOutArgs = List.chop nUnnamedCallerArgs unnamedCalledArgs
                    
                    // Check if all optional/out arguments are byref-out args
                    if unnamedCalledOptOrOutArgs |> List.forall (fun x -> x.IsOutArg && isByrefTy g x.CalledArgumentType) then 
                        unnamedCalledArgsTrimmed,[],unnamedCalledOptOrOutArgs 
                    // Check if all optional/out arguments are optional args
                    elif unnamedCalledOptOrOutArgs |> List.forall (fun x -> x.OptArgInfo.IsOptional) then 
                        unnamedCalledArgsTrimmed,unnamedCalledOptOrOutArgs,[]
                    // Otherwise drop them on the floor
                    else
                        unnamedCalledArgs,[],[]
                else 
                    unnamedCalledArgs,[],[]

            let (unnamedCallerArgs,paramArrayCallerArgs),unnamedCalledArgs,paramArrayCalledArgOpt = 
                let minArgs = unnamedCalledArgs.Length - 1
                let supportsParamArgs = 
                    allowParamArgs && 
                    minArgs >= 0 && 
                    unnamedCalledArgs |> List.last |> (fun calledArg -> calledArg.IsParamArray && isArray1DTy g calledArg.CalledArgumentType)

                if supportsParamArgs  && unnamedCallerArgs.Length >= minArgs then
                    let a,b = List.frontAndBack unnamedCalledArgs
                    List.chop minArgs unnamedCallerArgs, a, Some(b)
                else
                    (unnamedCallerArgs, []),unnamedCalledArgs, None

            let assignedNamedArgs = 
                fullCalledArgs |> List.choose (fun calledArg ->
                    match calledArg.NameOpt with 
                    | Some nm -> 
                        namedCallerArgs |> List.tryPick (fun (CallerNamedArg(nm2,callerArg)) -> 
                            if nm = nm2.idText then Some { NamedArgIdOpt = Some nm2; CallerArg=callerArg; CalledArg=calledArg } 
                            else None) 
                    | _ -> None)

            let unassignedNamedItem = 
                namedCallerArgs |> List.filter (fun (CallerNamedArg(nm,_e)) -> 
                    fullCalledArgs |> List.forall (fun calledArg -> 
                        match calledArg.NameOpt with 
                        | Some nm2 -> nm.idText <> nm2
                        | None -> true))

            let attributeAssignedNamedItems,unassignedNamedItem = 
                if isCheckingAttributeCall then 
                    // the assignment of names to properties is substantially for attribute specifications 
                    // permits bindings of names to non-mutable fields and properties, so we do that using the old 
                    // reliable code for this later on. 
                    unassignedNamedItem,[]
                 else 
                    [],unassignedNamedItem

            let assignedNamedProps,unassignedNamedItem = 
                let returnedObjTy = if minfo.IsConstructor then minfo.EnclosingType else methodRetTy
                unassignedNamedItem |> List.splitChoose (fun (CallerNamedArg(id,e) as arg) -> 
                    let nm = id.idText
                    let pinfos = GetIntrinsicPropInfoSetsOfType infoReader (Some(nm),ad,AllowMultiIntfInstantiations.Yes) IgnoreOverrides id.idRange returnedObjTy
                    let pinfos = pinfos |> ExcludeHiddenOfPropInfos g infoReader.amap m 
                    match pinfos with 
                    | [pinfo] when pinfo.HasSetter && not pinfo.IsIndexer -> 
                        let pminfo = pinfo.SetterMethod
                        let pminst = freshenMethInfo m pminfo
                        Choice1Of2(AssignedItemSetter(id,AssignedPropSetter(pinfo,pminfo, pminst), e))
                    | _ ->
                        let epinfos = 
                            match nameEnv with  
                            | Some(ne) ->  ExtensionPropInfosOfTypeInScope infoReader ne (Some(nm), ad) m returnedObjTy
                            | _ -> []
                        match epinfos with 
                        | [pinfo] when pinfo.HasSetter && not pinfo.IsIndexer -> 
                            let pminfo = pinfo.SetterMethod
                            let pminst = match minfo with
                                         | MethInfo.FSMeth(_,TType.TType_app(_,types),_,_) -> types
                                         | _ -> freshenMethInfo m pminfo

                            let pminst = match tyargsOpt with
                                          | Some(TType.TType_app(_, types)) -> types
                                          | _ -> pminst
                            Choice1Of2(AssignedItemSetter(id,AssignedPropSetter(pinfo,pminfo, pminst), e))
                        |  _ ->    
                            match infoReader.GetILFieldInfosOfType(Some(nm),ad,m,returnedObjTy) with
                            | finfo :: _ -> 
                                Choice1Of2(AssignedItemSetter(id,AssignedILFieldSetter(finfo), e))
                            | _ ->              
                              match infoReader.TryFindRecdOrClassFieldInfoOfType(nm,m,returnedObjTy) with
                              | Some rfinfo -> 
                                  Choice1Of2(AssignedItemSetter(id,AssignedRecdFieldSetter(rfinfo), e))
                              | None -> 
                                  Choice2Of2(arg))

            let names = namedCallerArgs |> List.map (fun (CallerNamedArg(nm,_)) -> nm.idText) 

            if (List.noRepeats String.order names).Length <> namedCallerArgs.Length then
                errorR(Error(FSComp.SR.typrelNamedArgumentHasBeenAssignedMoreThenOnce(),m))
                
            let argSet = { UnnamedCalledArgs=unnamedCalledArgs; UnnamedCallerArgs=unnamedCallerArgs; ParamArrayCalledArgOpt=paramArrayCalledArgOpt; ParamArrayCallerArgs=paramArrayCallerArgs; AssignedNamedArgs=assignedNamedArgs }

            (argSet,assignedNamedProps,unassignedNamedItem,attributeAssignedNamedItems,unnamedCalledOptArgs,unnamedCalledOutArgs))

    let argSets                     = argSetInfos |> List.map     (fun (x,_,_,_,_,_) -> x)
    let assignedNamedProps          = argSetInfos |> List.collect (fun (_,x,_,_,_,_) -> x)
    let unassignedNamedItems        = argSetInfos |> List.collect (fun (_,_,x,_,_,_) -> x)
    let attributeAssignedNamedItems = argSetInfos |> List.collect (fun (_,_,_,x,_,_) -> x)
    let unnamedCalledOptArgs        = argSetInfos |> List.collect (fun (_,_,_,_,x,_) -> x)
    let unnamedCalledOutArgs        = argSetInfos |> List.collect (fun (_,_,_,_,_,x) -> x)

    member x.infoReader = infoReader
    member x.amap = infoReader.amap

      /// the method we're attempting to call 
    member x.Method=minfo
      /// the instantiation of the method we're attempting to call 
    member x.CalledTyArgs=calledTyArgs
      /// the formal instantiation of the method we're attempting to call 
    member x.CallerTyArgs=callerTyArgs
      /// The types of the actual object arguments, if any
    member x.CallerObjArgTys=callerObjArgTys
      /// The argument analysis for each set of curried arguments
    member x.ArgSets=argSets
      /// return type
    member x.ReturnType=methodRetTy
      /// named setters
    member x.AssignedItemSetters=assignedNamedProps
      /// the property related to the method we're attempting to call, if any  
    member x.AssociatedPropertyInfo=pinfoOpt
      /// unassigned args
    member x.UnassignedNamedArgs=unassignedNamedItems
      /// args assigned to specifiy values for attribute fields and properties (these are not necessarily "property sets")
    member x.AttributeAssignedNamedArgs=attributeAssignedNamedItems
      /// unnamed called optional args: pass defaults for these
    member x.UnnamedCalledOptArgs=unnamedCalledOptArgs
      /// unnamed called out args: return these as part of the return tuple
    member x.UnnamedCalledOutArgs=unnamedCalledOutArgs

    static member GetMethod (x:CalledMeth<'T>) = x.Method

    member x.NumArgSets             = x.ArgSets.Length

    member x.HasOptArgs             = nonNil x.UnnamedCalledOptArgs
    member x.HasOutArgs             = nonNil x.UnnamedCalledOutArgs
    member x.UsesParamArrayConversion = x.ArgSets |> List.exists (fun argSet -> argSet.ParamArrayCalledArgOpt.IsSome)
    member x.ParamArrayCalledArgOpt = x.ArgSets |> List.tryPick (fun argSet -> argSet.ParamArrayCalledArgOpt)
    member x.ParamArrayCallerArgs = x.ArgSets |> List.tryPick (fun argSet -> if isSome argSet.ParamArrayCalledArgOpt then Some argSet.ParamArrayCallerArgs else None )
    member x.ParamArrayElementType = 
        assert (x.UsesParamArrayConversion)
        x.ParamArrayCalledArgOpt.Value.CalledArgumentType |> destArrayTy x.amap.g 
    member x.NumAssignedProps = x.AssignedItemSetters.Length
    member x.CalledObjArgTys(m) = x.Method.GetObjArgTypes(x.amap, m, x.CalledTyArgs)
    member x.NumCalledTyArgs = x.CalledTyArgs.Length
    member x.NumCallerTyArgs = x.CallerTyArgs.Length 

    member x.AssignsAllNamedArgs = isNil x.UnassignedNamedArgs

    member x.HasCorrectArity =
      (x.NumCalledTyArgs = x.NumCallerTyArgs)  &&
      x.ArgSets |> List.forall (fun argSet -> argSet.NumUnnamedCalledArgs = argSet.NumUnnamedCallerArgs) 

    member x.HasCorrectGenericArity =
      (x.NumCalledTyArgs = x.NumCallerTyArgs)  

    member x.IsAccessible(m,ad) = 
        IsMethInfoAccessible x.amap m ad x.Method 

    member x.HasCorrectObjArgs(m) = 
        x.CalledObjArgTys(m).Length = x.CallerObjArgTys.Length 

    member x.IsCandidate(m,ad) =
        x.IsAccessible(m,ad) &&
        x.HasCorrectArity && 
        x.HasCorrectObjArgs(m) &&
        x.AssignsAllNamedArgs

    member x.AssignedUnnamedArgs = 
       // We use Seq.map2 to tolerate there being mismatched caller/called args
       x.ArgSets |> List.map (fun argSet -> 
           (argSet.UnnamedCalledArgs, argSet.UnnamedCallerArgs) ||> Seq.map2 (fun calledArg callerArg -> 
               { NamedArgIdOpt=None; CalledArg=calledArg; CallerArg=callerArg }) |> Seq.toList)

    member x.AssignedNamedArgs = 
       x.ArgSets |> List.map (fun argSet -> argSet.AssignedNamedArgs)

    member x.AllUnnamedCalledArgs = x.ArgSets |> List.collect (fun x -> x.UnnamedCalledArgs)
    member x.TotalNumUnnamedCalledArgs = x.ArgSets |> List.sumBy (fun x -> x.NumUnnamedCalledArgs)
    member x.TotalNumUnnamedCallerArgs = x.ArgSets |> List.sumBy (fun x -> x.NumUnnamedCallerArgs)
    member x.TotalNumAssignedNamedArgs = x.ArgSets |> List.sumBy (fun x -> x.NumAssignedNamedArgs)

let NamesOfCalledArgs (calledArgs: CalledArg list) = 
    calledArgs |> List.choose (fun x -> x.NameOpt) 

//-------------------------------------------------------------------------
// Helpers dealing with propagating type information in method overload resolution
//------------------------------------------------------------------------- 

type ArgumentAnalysis = 
    | NoInfo
    | ArgDoesNotMatch 
    | CallerLambdaHasArgTypes of TType list
    | CalledArgMatchesType of TType

let InferLambdaArgsForLambdaPropagation origRhsExpr = 
    let rec loop e = 
        match e with 
        | SynExpr.Lambda(_,_,_,rest,_) -> 1 + loop rest
        | SynExpr.MatchLambda _ -> 1
        | _ -> 0
    loop origRhsExpr

let ExamineArgumentForLambdaPropagation (infoReader:InfoReader) (arg: AssignedCalledArg<SynExpr>) =
    let g = infoReader.g
    // Find the explicit lambda arguments of the caller. Ignore parentheses.
    let argExpr = match arg.CallerArg.Expr with SynExpr.Paren(x,_,_,_) -> x  | x -> x
    let countOfCallerLambdaArg = InferLambdaArgsForLambdaPropagation argExpr
    // Adjust for Expression<_>, Func<_,_>, ...
    let adjustedCalledArgTy = AdjustCalledArgType infoReader false arg.CalledArg arg.CallerArg
    if countOfCallerLambdaArg > 0 then 
        // Decompose the explicit function type of the target
        let calledLambdaArgTys,_calledLambdaRetTy = Tastops.stripFunTy g adjustedCalledArgTy
        if calledLambdaArgTys.Length >= countOfCallerLambdaArg then 
            // success 
            CallerLambdaHasArgTypes calledLambdaArgTys
        elif isDelegateTy g (if isLinqExpressionTy g adjustedCalledArgTy then destLinqExpressionTy g adjustedCalledArgTy else adjustedCalledArgTy) then
            ArgDoesNotMatch  // delegate arity mismatch
        else
            NoInfo   // not a function type on the called side - no information
    else CalledArgMatchesType(adjustedCalledArgTy)  // not a lambda on the caller side - push information from caller to called

let ExamineMethodForLambdaPropagation(x:CalledMeth<SynExpr>) =
    let unnamedInfo = x.AssignedUnnamedArgs |> List.mapSquared (ExamineArgumentForLambdaPropagation x.infoReader)
    let namedInfo = x.AssignedNamedArgs |> List.mapSquared (fun arg -> (arg.NamedArgIdOpt.Value, ExamineArgumentForLambdaPropagation x.infoReader arg))
    if unnamedInfo |> List.existsSquared (function CallerLambdaHasArgTypes _ -> true | _ -> false) || 
       namedInfo |> List.existsSquared (function (_,CallerLambdaHasArgTypes _) -> true | _ -> false) then 
        Some (unnamedInfo, namedInfo)
    else
        None



//-------------------------------------------------------------------------
// "Type Completion" inference and a few other checks at the end of the inference scope
//------------------------------------------------------------------------- 


/// "Type Completion" inference and a few other checks at the end of the inference scope
let FinalTypeDefinitionChecksAtEndOfInferenceScope (infoReader:InfoReader) isImplementation denv (tycon:Tycon) =

    let g = infoReader.g
    let amap = infoReader.amap

    let tcaug = tycon.TypeContents
    tcaug.tcaug_closed <- true
  
    // Note you only have to explicitly implement 'System.IComparable' to customize structural comparison AND equality on F# types 
    if isImplementation &&
#if EXTENSIONTYPING
       not tycon.IsProvidedGeneratedTycon &&
#endif
       isNone tycon.GeneratedCompareToValues &&
       tycon.HasInterface g g.mk_IComparable_ty && 
       not (tycon.HasOverride g "Equals" [g.obj_ty]) && 
       not tycon.IsFSharpInterfaceTycon
     then
        (* Warn when we're doing this for class types *)
        if Augment.TyconIsCandidateForAugmentationWithEquals g tycon then
            warning(Error(FSComp.SR.typrelTypeImplementsIComparableShouldOverrideObjectEquals(tycon.DisplayName),tycon.Range))
        else
            warning(Error(FSComp.SR.typrelTypeImplementsIComparableDefaultObjectEqualsProvided(tycon.DisplayName),tycon.Range))

    Augment.CheckAugmentationAttribs isImplementation g amap tycon
    // Check some conditions about generic comparison and hashing. We can only check this condition after we've done the augmentation 
    if isImplementation 
#if EXTENSIONTYPING
       && not tycon.IsProvidedGeneratedTycon  
#endif
       then
        let tcaug = tycon.TypeContents
        let m = tycon.Range
        let hasExplicitObjectGetHashCode = tycon.HasOverride g "GetHashCode" []
        let hasExplicitObjectEqualsOverride = tycon.HasOverride g "Equals" [g.obj_ty]

        if (isSome tycon.GeneratedHashAndEqualsWithComparerValues) && 
           (hasExplicitObjectGetHashCode || hasExplicitObjectEqualsOverride) then 
            errorR(Error(FSComp.SR.typrelExplicitImplementationOfGetHashCodeOrEquals(tycon.DisplayName),m)) 

        if not hasExplicitObjectEqualsOverride && hasExplicitObjectGetHashCode then 
            warning(Error(FSComp.SR.typrelExplicitImplementationOfGetHashCode(tycon.DisplayName),m)) 

        if hasExplicitObjectEqualsOverride && not hasExplicitObjectGetHashCode then 
            warning(Error(FSComp.SR.typrelExplicitImplementationOfEquals(tycon.DisplayName),m)) 


        // remember these values to ensure we don't generate these methods during codegen 
        tcaug.SetHasObjectGetHashCode hasExplicitObjectGetHashCode

        if not tycon.IsHiddenReprTycon
           && not tycon.IsTypeAbbrev
           && not tycon.IsMeasureableReprTycon
           && not tycon.IsAsmReprTycon
           && not tycon.IsFSharpInterfaceTycon
           && not tycon.IsFSharpDelegateTycon then 

            DispatchSlotChecking.CheckImplementationRelationAtEndOfInferenceScope (infoReader,denv,tycon,isImplementation) 
    
//-------------------------------------------------------------------------
// Additional helpers for type checking and constraint solving
//------------------------------------------------------------------------- 

/// "Single Feasible Type" inference
/// Look for the unique supertype of ty2 for which ty2 :> ty1 might feasibly hold
let FindUniqueFeasibleSupertype g amap m ty1 ty2 =  
    if not (isAppTy g ty2) then None else
    let supertypes = Option.toList (GetSuperTypeOfType g amap m ty2) @ (GetImmediateInterfacesOfType SkipUnrefInterfaces.Yes g amap m ty2)
    supertypes |> List.tryFind (TypeFeasiblySubsumesType 0 g amap m ty1 NoCoerce) 
    


/// Get the methods relevant to deterimining if a uniquely-identified-override exists based on the syntactic information 
/// at the member signature prior to type inference. This is used to pre-assign type information if it does 
let GetAbstractMethInfosForSynMethodDecl(infoReader:InfoReader,ad,memberName:Ident,bindm,typToSearchForAbstractMembers,valSynData) =
    let minfos = 
        match typToSearchForAbstractMembers with 
        | _,Some(SlotImplSet(_, dispatchSlotsKeyed,_,_)) -> 
            NameMultiMap.find  memberName.idText dispatchSlotsKeyed |> List.map (fun (RequiredSlot(dispatchSlot,_)) -> dispatchSlot)
        | ty, None -> 
            GetIntrinsicMethInfosOfType infoReader (Some(memberName.idText), ad, AllowMultiIntfInstantiations.Yes) IgnoreOverrides bindm ty
    let dispatchSlots = minfos |> List.filter (fun minfo -> minfo.IsDispatchSlot)
    let topValSynArities = SynInfo.AritiesOfArgs valSynData
    let topValSynArities = if topValSynArities.Length > 0 then topValSynArities.Tail else topValSynArities
    let dispatchSlotsArityMatch = dispatchSlots |> List.filter (fun minfo -> minfo.NumArgs = topValSynArities) 
    dispatchSlots,dispatchSlotsArityMatch 

/// Get the proeprties relevant to deterimining if a uniquely-identified-override exists based on the syntactic information 
/// at the member signature prior to type inference. This is used to pre-assign type information if it does 
let GetAbstractPropInfosForSynPropertyDecl(infoReader:InfoReader,ad,memberName:Ident,bindm,typToSearchForAbstractMembers,_k,_valSynData) = 
    let pinfos = 
        match typToSearchForAbstractMembers with 
        | _,Some(SlotImplSet(_,_,_,reqdProps)) -> 
            reqdProps |> List.filter (fun pinfo -> pinfo.PropertyName = memberName.idText) 
        | ty, None -> 
            GetIntrinsicPropInfosOfType infoReader (Some(memberName.idText), ad, AllowMultiIntfInstantiations.Yes) IgnoreOverrides bindm ty
        
    let dispatchSlots = pinfos |> List.filter (fun pinfo -> pinfo.IsVirtualProperty)
    dispatchSlots

//-------------------------------------------------------------------------
// Additional helpers for building method calls and doing TAST generation
//------------------------------------------------------------------------- 

/// Is this a 'base' call (in the sense of C#) 
let IsBaseCall objArgs = 
    match objArgs with 
    | [Expr.Val(v,_,_)] when v.BaseOrThisInfo  = BaseVal -> true
    | _ -> false
    
/// Compute whether we insert a 'coerce' on the 'this' pointer for an object model call 
/// For example, when calling an interface method on a struct, or a method on a constrained 
/// variable type. 
let ComputeConstrainedCallInfo g amap m (objArgs,minfo:MethInfo) =
    match objArgs with 
    | [objArgExpr] when not minfo.IsExtensionMember -> 
        let methObjTy = minfo.EnclosingType
        let objArgTy = tyOfExpr g objArgExpr
        if TypeDefinitelySubsumesTypeNoCoercion 0 g amap m methObjTy objArgTy 
           // Constrained calls to class types can only ever be needed for the three class types that 
           // are base types of value types
           || (isClassTy g methObjTy && 
                 (not (typeEquiv g methObjTy g.system_Object_typ || 
                       typeEquiv g methObjTy g.system_Value_typ ||
                       typeEquiv g methObjTy g.system_Enum_typ))) then 
            None
        else
            // The object argument is a value type or variable type and the target method is an interface or System.Object
            // type. A .NET 2.0 generic constrained call is required
            Some objArgTy
    | _ -> 
        None


/// Adjust the 'this' pointer before making a call 
/// Take the address of a struct, and coerce to an interface/base/constraint type if necessary 
let TakeObjAddrForMethodCall g amap (minfo:MethInfo) isMutable m objArgs f =
    let ccallInfo = ComputeConstrainedCallInfo g amap m (objArgs,minfo) 
    let mustTakeAddress = 
        (minfo.IsStruct && not minfo.IsExtensionMember)  // don't take the address of a struct when passing to an extension member
        ||
        (match ccallInfo with 
         | Some _ -> true 
         | None -> false) 
    let wrap,objArgs = 
        match objArgs with
        | [objArgExpr] -> 
            let objArgTy = tyOfExpr g objArgExpr
            let wrap,objArgExpr' = mkExprAddrOfExpr g mustTakeAddress (isSome ccallInfo) isMutable objArgExpr None m
            
            // Extension members and calls to class constraints may need a coercion for their object argument
            let objArgExpr' = 
              if isNone ccallInfo && // minfo.IsExtensionMember && minfo.IsStruct && 
                 not (TypeDefinitelySubsumesTypeNoCoercion 0 g amap m minfo.EnclosingType objArgTy) then 
                  mkCoerceExpr(objArgExpr',minfo.EnclosingType,m,objArgTy)
              else
                  objArgExpr'

            wrap,[objArgExpr'] 

        | _ -> 
            (fun x -> x), objArgs
    let e,ety = f ccallInfo objArgs
    wrap e,ety

//-------------------------------------------------------------------------
// Build method calls.
//------------------------------------------------------------------------- 

#if EXTENSIONTYPING
// This imports a provided method, and checks if it is a known compiler intrinsic like "1 + 2"
let TryImportProvidedMethodBaseAsLibraryIntrinsic (amap:Import.ImportMap, m:range, mbase: Tainted<ProvidedMethodBase>) = 
    let methodName = mbase.PUntaint((fun x -> x.Name),m)
    let declaringType = Import.ImportProvidedType amap m (mbase.PApply((fun x -> x.DeclaringType),m))
    if isAppTy amap.g declaringType then 
        let declaringEntity = tcrefOfAppTy amap.g declaringType
        if not declaringEntity.IsLocalRef && ccuEq declaringEntity.nlr.Ccu amap.g.fslibCcu then
            match amap.g.knownIntrinsics.TryGetValue ((declaringEntity.LogicalName, methodName)) with 
            | true,vref -> Some vref
            | _ -> 
            match amap.g.knownFSharpCoreModules.TryGetValue(declaringEntity.LogicalName) with
            | true,modRef -> 
                match modRef.ModuleOrNamespaceType.AllValsByLogicalName |> Seq.tryPick (fun (KeyValue(_,v)) -> if v.CompiledName = methodName then Some v else None) with 
                | Some v -> Some (mkNestedValRef modRef v)
                | None -> None
            | _ -> None
        else
            None
    else
        None
#endif
        

/// Build an expression that calls a given method info. 
/// This is called after overload resolution, and also to call other 
/// methods such as 'setters' for properties. 
//   tcVal: used to convert an F# value into an expression. See tc.fs. 
//   isProp: is it a property get? 
//   minst: the instantiation to apply for a generic method 
//   objArgs: the 'this' argument, if any 
//   args: the arguments, if any 
let BuildMethodCall tcVal g amap isMutable m isProp minfo valUseFlags minst objArgs args =

    let direct = IsBaseCall objArgs

    TakeObjAddrForMethodCall g amap minfo isMutable m objArgs (fun ccallInfo objArgs -> 
        let allArgs = (objArgs @ args)
        let valUseFlags = 
            if (direct && (match valUseFlags with NormalValUse -> true | _ -> false)) then 
                VSlotDirectCall 
            else 
                match ccallInfo with
                | Some ty -> 
                    // printfn "possible constrained call to '%s' at %A" minfo.LogicalName m
                    PossibleConstrainedCall ty
                | None -> 
                    valUseFlags

        match minfo with 
#if EXTENSIONTYPING
        // By this time this is an erased method info, e.g. one returned from an expression
        // REVIEW: copied from tastops, which doesn't allow protected methods
        | ProvidedMeth (amap,providedMeth,_,_) -> 
            // TODO: there  is a fair bit of duplication here with mk_il_minfo_call. We should be able to merge these
                
            /// Build an expression node that is a call to a extension method in a generated assembly
            let enclTy = minfo.EnclosingType
            // prohibit calls to methods that are declared in specific array types (Get,Set,Address)
            // these calls are provided by the runtime and should not be called from the user code
            if isArrayTy g enclTy then
                let tpe = TypeProviderError(FSComp.SR.tcRuntimeSuppliedMethodCannotBeUsedInUserCode(minfo.DisplayName), providedMeth.TypeProviderDesignation, m)
                error (tpe)
            let valu = isStructTy g enclTy
            let isCtor = minfo.IsConstructor
            if minfo.IsClassConstructor then 
                error (InternalError (minfo.LogicalName ^": cannot call a class constructor",m))
            let useCallvirt = not valu && not direct && minfo.IsVirtual
            let isProtected = minfo.IsProtectedAccessiblity
            let exprTy = if isCtor then enclTy else minfo.GetFSharpReturnTy(amap, m, minst)
            match TryImportProvidedMethodBaseAsLibraryIntrinsic (amap, m, providedMeth) with 
            | Some fsValRef -> 
                //reraise() calls are converted to TOp.Reraise in the type checker. So if a provided expression includes a reraise call
                // we must put it in that form here.
                if valRefEq amap.g fsValRef amap.g.reraise_vref then
                    mkReraise m exprTy, exprTy
                else
                    let vexp, vexpty = tcVal fsValRef valUseFlags (minfo.DeclaringTypeInst @ minst) m
                    BuildFSharpMethodApp g m fsValRef vexp vexpty allArgs
            | None -> 
                let ilMethRef = Import.ImportProvidedMethodBaseAsILMethodRef amap m providedMeth
                let isNewObj = isCtor && (match valUseFlags with NormalValUse -> true | _ -> false)
                let actualTypeInst = 
                    if isTupleTy g enclTy then argsOfAppTy g (mkCompiledTupleTy g (destTupleTy g enclTy))  // provided expressions can include method calls that get properties of tuple types
                    elif isFunTy g enclTy then [ domainOfFunTy g enclTy; rangeOfFunTy g enclTy ]  // provided expressions can call Invoke
                    else minfo.DeclaringTypeInst
                let actualMethInst = minst
                let retTy = (if not isCtor && (ilMethRef.ReturnType = ILType.Void) then [] else [exprTy])
                let noTailCall = false
                let expr = Expr.Op(TOp.ILCall(useCallvirt,isProtected,valu,isNewObj,valUseFlags,isProp,noTailCall,ilMethRef,actualTypeInst,actualMethInst, retTy),[],allArgs,m)
                expr,exprTy

#endif
            
        // Build a call to a .NET method 
        | ILMeth(_,ilMethInfo,_) -> 
            BuildILMethInfoCall g amap m isProp ilMethInfo valUseFlags minst direct allArgs

        // Build a call to an F# method 
        | FSMeth(_, _, vref, _) -> 

            // Go see if this is a use of a recursive definition... Note we know the value instantiation 
            // we want to use so we pass that in in order not to create a new one. 
            let vexp, vexpty = tcVal vref valUseFlags (minfo.DeclaringTypeInst @ minst) m
            BuildFSharpMethodApp g m vref vexp vexpty allArgs

        // Build a 'call' to a struct default constructor 
        | DefaultStructCtor (g,typ) -> 
            if not (TypeHasDefaultValue g m typ) then 
                errorR(Error(FSComp.SR.tcDefaultStructConstructorCall(),m))
            mkDefault (m,typ), typ)

//-------------------------------------------------------------------------
// Build delegate constructions (lambdas/functions to delegates)
//------------------------------------------------------------------------- 

/// Implements the elaborated form of adhoc conversions from functions to delegates at member callsites
let BuildNewDelegateExpr (eventInfoOpt:EventInfo option, g, amap, delegateTy, invokeMethInfo:MethInfo, delArgTys, f, fty, m) =
    let slotsig = invokeMethInfo.GetSlotSig(amap, m)
    let delArgVals,expr = 
        let topValInfo = ValReprInfo([],List.replicate (List.length delArgTys) ValReprInfo.unnamedTopArg, ValReprInfo.unnamedRetVal)

        // Try to pull apart an explicit lambda and use it directly 
        // Don't do this in the case where we're adjusting the arguments of a function used to build a .NET-compatible event handler 
        let lambdaContents = 
            if isSome eventInfoOpt then 
                None 
            else 
                tryDestTopLambda g amap topValInfo (f, fty)        
        match lambdaContents with 
        | None -> 
        
            if List.exists (isByrefTy g) delArgTys then
                    error(Error(FSComp.SR.tcFunctionRequiresExplicitLambda(List.length delArgTys),m)) 

            let delArgVals = delArgTys |> List.map (fun argty -> fst (mkCompGenLocal m "delegateArg" argty)) 
            let expr = 
                let args = 
                    match eventInfoOpt with 
                    | Some einfo -> 
                        match delArgVals with 
                        | [] -> error(nonStandardEventError einfo.EventName m)
                        | h :: _ when not (isObjTy g h.Type) -> error(nonStandardEventError einfo.EventName m)
                        | h :: t -> [exprForVal m h; mkTupledVars g m t] 
                    | None -> 
                        if isNil delArgTys then [mkUnit g m] else List.map (exprForVal m) delArgVals
                mkApps g ((f,fty),[],args,m)
            delArgVals,expr
            
        | Some _ -> 
           if isNil delArgTys then [], mkApps g ((f,fty),[],[mkUnit g m],m) 
           else
               let _,_,_,vsl,body,_ = IteratedAdjustArityOfLambda g amap topValInfo f
               List.concat vsl, body
            
    let meth = TObjExprMethod(slotsig, [], [], [delArgVals], expr, m)
    mkObjExpr(delegateTy,None,BuildObjCtorCall g m,[meth],[],m)

let CoerceFromFSharpFuncToDelegate g amap infoReader ad callerArgTy m callerArgExpr delegateTy =    
    let (SigOfFunctionForDelegate(invokeMethInfo,delArgTys,_,_)) = GetSigOfFunctionForDelegate infoReader delegateTy m ad
    BuildNewDelegateExpr (None, g, amap, delegateTy, invokeMethInfo, delArgTys, callerArgExpr, callerArgTy, m)


//-------------------------------------------------------------------------
// Import provided expressions
//------------------------------------------------------------------------- 


#if EXTENSIONTYPING
// This file is not a great place for this functionality to sit, it's here because of BuildMethodCall
module ProvidedMethodCalls =

    let private convertConstExpr g amap m (constant : Tainted<obj * ProvidedType>) =
        let (obj,objTy) = constant.PApply2(id,m)
        let ty = Import.ImportProvidedType amap m objTy
        let normTy = normalizeEnumTy g ty
        obj.PUntaint((fun v ->
            let fail() = raise <| TypeProviderError(FSComp.SR.etUnsupportedConstantType(v.GetType().ToString()), constant.TypeProviderDesignation, m)
            try 
                match v with
                | null -> mkNull m ty
                | _ when typeEquiv g normTy g.bool_ty -> Expr.Const(Const.Bool(v :?> bool), m, ty)
                | _ when typeEquiv g normTy g.sbyte_ty -> Expr.Const(Const.SByte(v :?> sbyte), m, ty)
                | _ when typeEquiv g normTy g.byte_ty -> Expr.Const(Const.Byte(v :?> byte), m, ty)
                | _ when typeEquiv g normTy g.int16_ty -> Expr.Const(Const.Int16(v :?> int16), m, ty)
                | _ when typeEquiv g normTy g.uint16_ty -> Expr.Const(Const.UInt16(v :?> uint16), m, ty)
                | _ when typeEquiv g normTy g.int32_ty -> Expr.Const(Const.Int32(v :?> int32), m, ty)
                | _ when typeEquiv g normTy g.uint32_ty -> Expr.Const(Const.UInt32(v :?> uint32), m, ty)
                | _ when typeEquiv g normTy g.int64_ty -> Expr.Const(Const.Int64(v :?> int64), m, ty)
                | _ when typeEquiv g normTy g.uint64_ty -> Expr.Const(Const.UInt64(v :?> uint64), m, ty)
                | _ when typeEquiv g normTy g.nativeint_ty -> Expr.Const(Const.IntPtr(v :?> int64), m, ty) 
                | _ when typeEquiv g normTy g.unativeint_ty -> Expr.Const(Const.UIntPtr(v :?> uint64), m, ty) 
                | _ when typeEquiv g normTy g.float32_ty -> Expr.Const(Const.Single(v :?> float32), m, ty)
                | _ when typeEquiv g normTy g.float_ty -> Expr.Const(Const.Double(v :?> float), m, ty)
                | _ when typeEquiv g normTy g.char_ty -> Expr.Const(Const.Char(v :?> char), m, ty)
                | _ when typeEquiv g normTy g.string_ty -> Expr.Const(Const.String(v :?> string), m, ty)
                | _ when typeEquiv g normTy g.decimal_ty -> Expr.Const(Const.Decimal(v :?> decimal), m, ty)
                | _ when typeEquiv g normTy g.unit_ty -> Expr.Const(Const.Unit, m, ty)
                | _ -> fail()
             with _ -> 
                 fail()
            ), range=m)

    /// Erasure over System.Type.
    ///
    /// This is a reimplementation of the logic of provided-type erasure, working entirely over (tainted, provided) System.Type
    /// values. This is used when preparing ParameterInfo objects to give to the provider in GetInvokerExpression. 
    /// These ParameterInfo have erased ParameterType - giving the provider an erased type makes it considerably easier 
    /// to implement a correct GetInvokerExpression.
    ///
    /// Ideally we would implement this operation by converting to an F# TType using ImportSystemType, and then erasing, and then converting
    /// back to System.Type. However, there is currently no way to get from an arbitrary F# TType (even the TType for 
    /// System.Object) to a System.Type to give to the type provider.
    let eraseSystemType (amap,m,inputType) = 
        let rec loop (st:Tainted<ProvidedType>) = 
            if st.PUntaint((fun st -> st.IsGenericParameter),m) then st
            elif st.PUntaint((fun st -> st.IsArray),m) then 
                let et = st.PApply((fun st -> st.GetElementType()),m)
                let rank = st.PUntaint((fun st -> st.GetArrayRank()),m)
                (loop et).PApply((fun st -> ProvidedType.CreateNoContext(if rank = 1 then st.RawSystemType.MakeArrayType() else st.RawSystemType.MakeArrayType(rank))),m)
            elif st.PUntaint((fun st -> st.IsByRef),m) then 
                let et = st.PApply((fun st -> st.GetElementType()),m)
                (loop et).PApply((fun st -> ProvidedType.CreateNoContext(st.RawSystemType.MakeByRefType())),m)
            elif st.PUntaint((fun st -> st.IsPointer),m) then 
                let et = st.PApply((fun st -> st.GetElementType()),m)
                (loop et).PApply((fun st -> ProvidedType.CreateNoContext(st.RawSystemType.MakePointerType())),m)
            else
                let isGeneric = st.PUntaint((fun st -> st.IsGenericType),m)
                let headType = if isGeneric then st.PApply((fun st -> st.GetGenericTypeDefinition()),m) else st
                // We import in order to use IsProvidedErasedTycon, to make sure we at least don't reinvent that 
                let headTypeAsFSharpType = Import.ImportProvidedNamedType amap m headType
                if headTypeAsFSharpType.IsProvidedErasedTycon then 
                    let baseType = 
                        st.PApply((fun st -> 
                            match st.BaseType with 
                            | null -> ProvidedType.CreateNoContext(typeof<obj>)  // it might be an interface
                            | st -> st),m)
                    loop baseType
                else
                    if isGeneric then 
                        let genericArgs = st.PApplyArray((fun st -> st.GetGenericArguments()),"GetGenericArguments",m) 
                        let typars = headTypeAsFSharpType.Typars(m)
                        // Drop the generic arguments that don't correspond to type arguments, i.e. are units-of-measure
                        let genericArgs = 
                            [| for (genericArg,tp) in Seq.zip genericArgs typars do
                                   if tp.Kind = TyparKind.Type then 
                                       yield genericArg |]

                        if genericArgs.Length = 0 then 
                            headType
                        else
                            let erasedArgTys = genericArgs |> Array.map loop
                            headType.PApply((fun st -> 
                                let erasedArgTys = erasedArgTys |> Array.map (fun a -> a.PUntaintNoFailure (fun x -> x.RawSystemType))
                                ProvidedType.CreateNoContext(st.RawSystemType.MakeGenericType erasedArgTys)),m)
                    else   
                        st
        loop inputType

    let convertProvidedExpressionToExprAndWitness tcVal (thisArg:Expr option,
                                                         allArgs:Exprs,
                                                         paramVars:Tainted<ProvidedVar>[],
                                                         g,amap,mut,isProp,isSuperInit,m,
                                                         expr:Tainted<ProvidedExpr>) = 
        let varConv = 
            [ for (v,e) in Seq.zip (paramVars |> Seq.map (fun x -> x.PUntaint(id,m))) (Option.toList thisArg @ allArgs) do
                 yield (v,(None,e)) ]
            |> Dictionary.ofList 

        let rec exprToExprAndWitness top (ea:Tainted<ProvidedExpr>) =
            let fail() = error(Error(FSComp.SR.etUnsupportedProvidedExpression(ea.PUntaint((fun etree -> etree.UnderlyingExpressionString), m)),m))
            match ea with
            | Tainted.Null -> error(Error(FSComp.SR.etNullProvidedExpression(ea.TypeProviderDesignation),m))
            |  _ ->
            match ea.PApplyOption((function ProvidedTypeAsExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let (expr,targetTy) = info.PApply2(id,m)
                let srcExpr = exprToExpr expr
                let targetTy = Import.ImportProvidedType amap m (targetTy.PApply(id,m)) 
                let sourceTy = Import.ImportProvidedType amap m (expr.PApply((fun e -> e.Type),m)) 
                let te = mkCoerceIfNeeded g targetTy sourceTy srcExpr
                None, (te, tyOfExpr g te)
            | None -> 
            match ea.PApplyOption((function ProvidedTypeTestExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let (expr,targetTy) = info.PApply2(id,m)
                let srcExpr = exprToExpr expr
                let targetTy = Import.ImportProvidedType amap m (targetTy.PApply(id,m)) 
                let te = mkCallTypeTest g m targetTy srcExpr
                None, (te, tyOfExpr g te)
            | None -> 
            match ea.PApplyOption((function ProvidedIfThenElseExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let test,thenBranch,elseBranch = info.PApply3(id,m)
                let testExpr = exprToExpr test
                let ifTrueExpr = exprToExpr thenBranch
                let ifFalseExpr = exprToExpr elseBranch
                let te = mkCond NoSequencePointAtStickyBinding SuppressSequencePointAtTarget m (tyOfExpr g ifTrueExpr) testExpr ifTrueExpr ifFalseExpr
                None, (te, tyOfExpr g te)
            | None -> 
            match ea.PApplyOption((function ProvidedVarExpr x -> Some x | _ -> None), m) with
            | Some info ->  
                let _,vTe = varToExpr info
                None, (vTe, tyOfExpr g vTe)
            | None -> 
            match ea.PApplyOption((function ProvidedConstantExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let ce = convertConstExpr g amap m info
                None, (ce, tyOfExpr g ce)
            | None -> 
            match ea.PApplyOption((function ProvidedNewTupleExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let elems = info.PApplyArray(id, "GetInvokerExpresson",m)
                let elemsT = elems |> Array.map exprToExpr |> Array.toList
                let exprT = mkTupledNoTypes g m elemsT
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedNewArrayExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let ty,elems = info.PApply2(id,m)
                let tyT = Import.ImportProvidedType amap m ty
                let elems = elems.PApplyArray(id, "GetInvokerExpresson",m)
                let elemsT = elems |> Array.map exprToExpr |> Array.toList
                let exprT = Expr.Op(TOp.Array, [tyT],elemsT,m)
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedTupleGetExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let inp,n = info.PApply2(id, m)
                let inpT = inp |> exprToExpr 
                // if type of expression is erased type then we need convert it to the underlying base type
                let typeOfExpr = 
                    let t = tyOfExpr g inpT
                    stripTyEqnsWrtErasure EraseMeasures g t
                let tysT = tryDestTupleTy g typeOfExpr
                let exprT = mkTupleFieldGet (inpT, tysT, n.PUntaint(id,m), m)
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedLambdaExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let v,b = info.PApply2(id, m)
                let vT = addVar v
                let bT = exprToExpr b
                removeVar v
                let exprT = mkLambda m vT (bT, tyOfExpr g bT)
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedLetExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let v,e,b = info.PApply3(id, m)
                let eT = exprToExpr  e
                let vT = addVar v
                let bT = exprToExpr  b
                removeVar v
                let exprT = mkCompGenLet m vT eT bT
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedVarSetExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let v,e = info.PApply2(id, m)
                let eT = exprToExpr  e
                let vTopt,_ = varToExpr v
                match vTopt with 
                | None -> 
                    fail()
                | Some vT -> 
                    let exprT = mkValSet m (mkLocalValRef vT) eT 
                    None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedWhileLoopExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let guardExpr,bodyExpr = info.PApply2(id, m)
                let guardExprT = exprToExpr guardExpr
                let bodyExprT = exprToExpr bodyExpr
                let exprT = mkWhile g (SequencePointInfoForWhileLoop.NoSequencePointAtWhileLoop,SpecialWhileLoopMarker.NoSpecialWhileLoopMarker, guardExprT, bodyExprT, m)
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedForIntegerRangeLoopExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let v,e1,e2,e3 = info.PApply4(id, m)
                let e1T = exprToExpr  e1
                let e2T = exprToExpr  e2
                let vT = addVar v
                let e3T = exprToExpr  e3
                removeVar v
                let exprT = mkFastForLoop g (SequencePointInfoForForLoop.NoSequencePointAtForLoop,m,vT,e1T,true,e2T,e3T)
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedNewDelegateExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let delegateTy,boundVars,delegateBodyExpr = info.PApply3(id, m)
                let delegateTyT = Import.ImportProvidedType amap m delegateTy
                let vs = boundVars.PApplyArray(id, "GetInvokerExpresson",m) |> Array.toList 
                let vsT = List.map addVar vs
                let delegateBodyExprT = exprToExpr delegateBodyExpr
                List.iter removeVar vs
                let lambdaExpr = mkLambdas m [] vsT (delegateBodyExprT, tyOfExpr g delegateBodyExprT)
                let lambdaExprTy = tyOfExpr g lambdaExpr
                let infoReader = InfoReader(g, amap)
                let exprT = CoerceFromFSharpFuncToDelegate g amap infoReader AccessorDomain.AccessibleFromSomewhere lambdaExprTy m lambdaExpr delegateTyT
                None, (exprT, tyOfExpr g exprT)
            | None -> 
#if PROVIDED_ADDRESS_OF
            match ea.PApplyOption((function ProvidedAddressOfExpr x -> Some x | _ -> None), m) with
            | Some e -> 
                let eT =  exprToExpr e
                let wrap,ce = mkExprAddrOfExpr g true false DefinitelyMutates eT None m
                let ce = wrap ce
                None, (ce, tyOfExpr g ce)
            | None -> 
#endif
            match ea.PApplyOption((function ProvidedDefaultExpr x -> Some x | _ -> None), m) with
            | Some pty -> 
                let ty = Import.ImportProvidedType amap m pty
                let ce = mkDefault (m, ty)
                None, (ce, tyOfExpr g ce)
            | None -> 
            match ea.PApplyOption((function ProvidedCallExpr c -> Some c | _ -> None), m) with 
            | Some info ->
                methodCallToExpr top ea info
            | None -> 
            match ea.PApplyOption((function ProvidedSequentialExpr c -> Some c | _ -> None), m) with 
            | Some info ->
                let e1,e2 = info.PApply2(id, m)
                let e1T = exprToExpr e1
                let e2T = exprToExpr e2
                let ce = mkCompGenSequential m e1T e2T
                None, (ce, tyOfExpr g ce)
            | None -> 
            match ea.PApplyOption((function ProvidedTryFinallyExpr c -> Some c | _ -> None), m) with 
            | Some info ->
                let e1,e2 = info.PApply2(id, m)
                let e1T = exprToExpr e1
                let e2T = exprToExpr e2
                let ce = mkTryFinally g (e1T,e2T,m,tyOfExpr g e1T,SequencePointInfoForTry.NoSequencePointAtTry,SequencePointInfoForFinally.NoSequencePointAtFinally)
                None, (ce, tyOfExpr g ce)
            | None -> 
            match ea.PApplyOption((function ProvidedTryWithExpr c -> Some c | _ -> None), m) with 
            | Some info ->
                let bT = exprToExpr (info.PApply((fun (x,_,_,_,_) -> x), m))
                let v1 = info.PApply((fun (_,x,_,_,_) -> x), m)
                let v1T = addVar v1
                let e1T = exprToExpr (info.PApply((fun (_,_,x,_,_) -> x), m))
                removeVar v1
                let v2 = info.PApply((fun (_,_,_,x,_) -> x), m)
                let v2T = addVar v2
                let e2T = exprToExpr (info.PApply((fun (_,_,_,_,x) -> x), m))
                removeVar v2
                let ce = mkTryWith g (bT,v1T,e1T,v2T,e2T,m,tyOfExpr g bT,SequencePointInfoForTry.NoSequencePointAtTry,SequencePointInfoForWith.NoSequencePointAtWith)
                None, (ce, tyOfExpr g ce)
            | None -> 
            match ea.PApplyOption((function ProvidedNewObjectExpr c -> Some c | _ -> None), m) with 
            | Some info -> 
                None, ctorCallToExpr info
            | None -> 
                fail()


        and ctorCallToExpr (ne:Tainted<_>) =    
            let (ctor,args) = ne.PApply2(id,m)
            let targetMethInfo = ProvidedMeth(amap,ctor.PApply((fun ne -> upcast ne),m),None,m)
            let objArgs = [] 
            let arguments = [ for ea in args.PApplyArray(id, "GetInvokerExpresson", m) -> exprToExpr ea ]
            let callExpr = BuildMethodCall tcVal g amap Mutates.PossiblyMutates m false targetMethInfo isSuperInit [] objArgs arguments
            callExpr

        and addVar (v:Tainted<ProvidedVar>) =    
            let nm = v.PUntaint ((fun v -> v.Name),m)
            let mut = v.PUntaint ((fun v -> v.IsMutable),m)
            let vRaw = v.PUntaint (id,m)
            let tyT = Import.ImportProvidedType amap m (v.PApply ((fun v -> v.Type),m))
            let vT,vTe = if mut then mkMutableCompGenLocal m nm tyT else mkCompGenLocal m nm tyT
            varConv.[vRaw] <- (Some vT,vTe)
            vT

        and removeVar (v:Tainted<ProvidedVar>) =    
            let vRaw = v.PUntaint (id,m)
            varConv.Remove vRaw |> ignore

        and methodCallToExpr top _origExpr (mce:Tainted<_>) =    
            let (objOpt,meth,args) = mce.PApply3(id,m)
            let targetMethInfo = ProvidedMeth(amap,meth.PApply((fun mce -> upcast mce), m),None,m)
            let objArgs = 
                match objOpt.PApplyOption(id, m) with
                | None -> []
                | Some objExpr -> [exprToExpr objExpr]

            let arguments = [ for ea in args.PApplyArray(id, "GetInvokerExpresson", m) -> exprToExpr ea ]
            let genericArguments = 
                if meth.PUntaint((fun m -> m.IsGenericMethod), m) then 
                    meth.PApplyArray((fun m -> m.GetGenericArguments()), "GetGenericArguments", m)  
                else 
                    [| |]
            let replacementGenericArguments = genericArguments |> Array.map (fun t->Import.ImportProvidedType amap m t) |> List.ofArray

            let mut         = if top then mut else PossiblyMutates
            let isSuperInit = if top then isSuperInit else ValUseFlag.NormalValUse
            let isProp      = if top then isProp else false
            let callExpr = BuildMethodCall tcVal g amap mut m isProp targetMethInfo isSuperInit replacementGenericArguments objArgs arguments
            Some meth, callExpr

        and varToExpr (pe:Tainted<ProvidedVar>) =    
            // sub in the appropriate argument
            // REVIEW: "thisArg" pointer should be first, if present
            let vRaw = pe.PUntaint(id,m)
            if not (varConv.ContainsKey vRaw) then
                        let typeProviderDesignation = ExtensionTyping.DisplayNameOfTypeProvider (pe.TypeProvider, m)
                        error(NumberedError(FSComp.SR.etIncorrectParameterExpression(typeProviderDesignation,vRaw.Name), m))
            varConv.[vRaw]
                
        and exprToExpr expr =
            let _, (resExpr, _) = exprToExprAndWitness false expr
            resExpr

        exprToExprAndWitness true expr

        
    // fill in parameter holes in the expression   
    let TranslateInvokerExpressionForProvidedMethodCall tcVal (g, amap, mut, isProp, isSuperInit, mi:Tainted<ProvidedMethodBase>, objArgs, allArgs, m) =        
        let parameters = 
            mi.PApplyArray((fun mi -> mi.GetParameters()), "GetParameters", m)
        let paramTys = 
            parameters
            |> Array.map (fun p -> p.PApply((fun st -> st.ParameterType),m))
        let erasedParamTys = 
            paramTys
            |> Array.map (fun pty -> eraseSystemType (amap,m,pty))
        let paramVars = 
            erasedParamTys
            |> Array.mapi (fun i erasedParamTy -> erasedParamTy.PApply((fun ty ->  ProvidedVar.Fresh("arg" + i.ToString(),ty)),m))


        // encode "this" as the first ParameterExpression, if applicable
        let thisArg, paramVars = 
            match objArgs with
            | [objArg] -> 
                let erasedThisTy = eraseSystemType (amap,m,mi.PApply((fun mi -> mi.DeclaringType),m))
                let thisVar = erasedThisTy.PApply((fun ty -> ProvidedVar.Fresh("this", ty)), m)
                Some objArg , Array.append [| thisVar |] paramVars
            | [] -> None , paramVars
            | _ -> failwith "multiple objArgs?"
            
        let ea = mi.PApplyWithProvider((fun (methodInfo,provider) -> ExtensionTyping.GetInvokerExpression(provider, methodInfo, [| for p in paramVars -> p.PUntaintNoFailure id |])), m)

        convertProvidedExpressionToExprAndWitness tcVal (thisArg,allArgs,paramVars,g,amap,mut,isProp,isSuperInit,m,ea)

            
    let BuildInvokerExpressionForProvidedMethodCall tcVal (g, amap, mi:Tainted<ProvidedMethodBase>, objArgs, mut, isProp, isSuperInit, allArgs, m) =
        try                   
            let methInfoOpt, (expr, retTy) = TranslateInvokerExpressionForProvidedMethodCall tcVal (g, amap, mut, isProp, isSuperInit, mi, objArgs, allArgs, m)

            let exprty = GetCompiledReturnTyOfProvidedMethodInfo amap m mi |> GetFSharpViewOfReturnType g
            let expr = mkCoerceIfNeeded g exprty retTy expr
            methInfoOpt, expr, exprty
        with
            | :? TypeProviderError as tpe ->
                let typeName = mi.PUntaint((fun mb -> mb.DeclaringType.FullName), m)
                let methName = mi.PUntaint((fun mb -> mb.Name), m)
                raise( tpe.WithContext(typeName, methName) )  // loses original stack trace
#endif
