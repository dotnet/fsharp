// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckIncrementalClasses

open System

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CheckPatterns
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Xml
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeHierarchy

type cenv = TcFileState

let TcClassRewriteStackGuardDepth = StackGuard.GetDepthOption "TcClassRewrite"

exception ParameterlessStructCtor of range: range

/// Represents a single group of bindings in a class with an implicit constructor
type IncrClassBindingGroup = 
    | IncrClassBindingGroup of bindings: Binding list * isStatic: bool* isRecursive: bool
    | IncrClassDo of expr: Expr * isStatic: bool * range: Range 

/// Typechecked info for implicit constructor and it's arguments 
type IncrClassCtorLhs = 
    {
        /// The TyconRef for the type being defined
        TyconRef: TyconRef

        /// The type parameters allocated for the implicit instance constructor. 
        /// These may be equated with other (WillBeRigid) type parameters through equi-recursive inference, and so 
        /// should always be renormalized/canonicalized when used.
        InstanceCtorDeclaredTypars: Typars     

        /// The value representing the static implicit constructor.
        /// Lazy to ensure the static ctor value is only published if needed.
        StaticCtorValInfo: Lazy<Val list * Val * ValScheme>

        /// The value representing the implicit constructor.
        InstanceCtorVal: Val

        /// The type of the implicit constructor, representing as a ValScheme.
        InstanceCtorValScheme: ValScheme
        
        /// The values representing the arguments to the implicit constructor.
        InstanceCtorArgs: Val list

        /// The reference cell holding the 'this' parameter within the implicit constructor so it can be referenced in the
        /// arguments passed to the base constructor
        InstanceCtorSafeThisValOpt: Val option

        /// Data indicating if safe-initialization checks need to be inserted for this type.
        InstanceCtorSafeInitInfo: SafeInitData

        /// The value representing the 'base' variable within the implicit instance constructor.
        InstanceCtorBaseValOpt: Val option

        /// The value representing the 'this' variable within the implicit instance constructor.
        InstanceCtorThisVal: Val

        /// The name generator used to generate the names of fields etc. within the type.
        NameGenerator: NiceNameGenerator
    }
        
    /// Get the type parameters of the implicit constructor, after taking equi-recursive inference into account.
    member ctorInfo.GetNormalizedInstanceCtorDeclaredTypars (cenv: cenv) denv m = 
        let g = cenv.g
        let ctorDeclaredTypars = ctorInfo.InstanceCtorDeclaredTypars
        let ctorDeclaredTypars = ChooseCanonicalDeclaredTyparsAfterInference g denv ctorDeclaredTypars m
        ctorDeclaredTypars

/// Check and elaborate the "left hand side" of the implicit class construction 
/// syntax.
let TcImplicitCtorLhs_Phase2A(cenv: cenv, env, tpenv, tcref: TyconRef, vis, attrs, spats, thisIdOpt, baseValOpt: Val option, safeInitInfo, m, copyOfTyconTypars, objTy, thisTy, xmlDoc: PreXmlDoc) =

    let g = cenv.g
    let baseValOpt = 
        match GetSuperTypeOfType g cenv.amap m objTy with 
        | Some superTy -> MakeAndPublishBaseVal cenv env (match baseValOpt with None -> None | Some v -> Some v.Id) superTy
        | None -> None

    // Add class typars to env 
    let env = AddDeclaredTypars CheckForDuplicateTypars copyOfTyconTypars env

    // Type check arguments by processing them as 'simple' patterns 
    //     NOTE: if we allow richer patterns here this is where we'd process those patterns 
    let ctorArgNames, patEnv = TcSimplePatsOfUnknownType cenv true CheckCxs env tpenv (SynSimplePats.SimplePats (spats, m))

    let (TcPatLinearEnv(_, names, _)) = patEnv
        
    // Create the values with the given names 
    let _, vspecs = MakeAndPublishSimpleVals cenv env names

    if tcref.IsStructOrEnumTycon && isNil spats then 
        errorR (ParameterlessStructCtor(tcref.Range))
        
    // Put them in order 
    let ctorArgs = List.map (fun v -> NameMap.find v vspecs) ctorArgNames
    let safeThisValOpt = MakeAndPublishSafeThisVal cenv env thisIdOpt thisTy
        
    // NOTE: the type scheme here is not complete!!! The ctorTy is more or less 
    // just a type variable. The type and typars get fixed-up after inference 
    let ctorValScheme, ctorVal = 
        let argTy = mkRefTupledTy g (typesOfVals ctorArgs)

        // Initial type has known information 
        let ctorTy = mkFunTy g argTy objTy    

        // NOTE: no attributes can currently be specified for the implicit constructor 
        let attribs = TcAttributes cenv env (AttributeTargets.Constructor ||| AttributeTargets.Method) attrs
        let memberFlags = CtorMemberFlags SynMemberFlagsTrivia.Zero
                                  
        let synArgInfos = List.map (SynInfo.InferSynArgInfoFromSimplePat []) spats
        let valSynData = SynValInfo([synArgInfos], SynInfo.unnamedRetVal)
        let id = ident ("new", m)

        CheckForNonAbstractInterface ModuleOrMemberBinding tcref memberFlags id.idRange
        let memberInfo = MakeMemberDataAndMangledNameForMemberVal(g, tcref, false, attribs, [], memberFlags, valSynData, id, false)
        let prelimValReprInfo = TranslateSynValInfo m (TcAttributes cenv env) valSynData
        let prelimTyschemeG = GeneralizedType(copyOfTyconTypars, ctorTy)
        let isComplete = ComputeIsComplete copyOfTyconTypars [] ctorTy
        let varReprInfo = InferGenericArityFromTyScheme prelimTyschemeG prelimValReprInfo
        let ctorValScheme = ValScheme(id, prelimTyschemeG, Some varReprInfo, None, Some memberInfo, false, ValInline.Never, NormalVal, vis, false, true, false, false)
        let paramNames = varReprInfo.ArgNames
        let xmlDoc = xmlDoc.ToXmlDoc(true, Some paramNames)
        let ctorVal = MakeAndPublishVal cenv env (Parent tcref, false, ModuleOrMemberBinding, ValInRecScope isComplete, ctorValScheme, attribs, xmlDoc, None, false) 
        ctorValScheme, ctorVal

    // We only generate the cctor on demand, because we don't need it if there are no cctor actions. 
    // The code below has a side-effect (MakeAndPublishVal), so we only want to run it once if at all. 
    // The .cctor is never referenced by any other code.
    let cctorValInfo = 
        lazy 
            let cctorArgs = [ fst(mkCompGenLocal m "unitVar" g.unit_ty) ]

            let cctorTy = mkFunTy g g.unit_ty g.unit_ty
            let valSynData = SynValInfo([[]], SynInfo.unnamedRetVal)
            let id = ident ("cctor", m)
            CheckForNonAbstractInterface ModuleOrMemberBinding tcref (ClassCtorMemberFlags SynMemberFlagsTrivia.Zero) id.idRange
            let memberInfo = MakeMemberDataAndMangledNameForMemberVal(g, tcref, false, [], [], (ClassCtorMemberFlags SynMemberFlagsTrivia.Zero), valSynData, id, false)
            let prelimValReprInfo = TranslateSynValInfo m (TcAttributes cenv env) valSynData
            let prelimTyschemeG = GeneralizedType(copyOfTyconTypars, cctorTy)
            let valReprInfo = InferGenericArityFromTyScheme prelimTyschemeG prelimValReprInfo
            let cctorValScheme = ValScheme(id, prelimTyschemeG, Some valReprInfo, None, Some memberInfo, false, ValInline.Never, NormalVal, Some (SynAccess.Private Range.Zero), false, true, false, false)
                 
            let cctorVal = MakeAndPublishVal cenv env (Parent tcref, false, ModuleOrMemberBinding, ValNotInRecScope, cctorValScheme, [(* no attributes*)], XmlDoc.Empty, None, false) 
            cctorArgs, cctorVal, cctorValScheme

    let thisVal = 
        // --- Create this for use inside constructor 
        let thisId = ident ("this", m)
        let thisValScheme = ValScheme(thisId, NonGenericTypeScheme thisTy, None, None, None, false, ValInline.Never, CtorThisVal, None, true, false, false, false)
        let thisVal = MakeAndPublishVal cenv env (ParentNone, false, ClassLetBinding false, ValNotInRecScope, thisValScheme, [], XmlDoc.Empty, None, false)
        thisVal

    {   TyconRef = tcref
        InstanceCtorDeclaredTypars = copyOfTyconTypars
        StaticCtorValInfo = cctorValInfo
        InstanceCtorArgs = ctorArgs
        InstanceCtorVal = ctorVal
        InstanceCtorValScheme = ctorValScheme
        InstanceCtorBaseValOpt = baseValOpt
        InstanceCtorSafeThisValOpt = safeThisValOpt
        InstanceCtorSafeInitInfo = safeInitInfo
        InstanceCtorThisVal = thisVal
        // For generating names of local fields
        NameGenerator = NiceNameGenerator()
    }


// Partial class defns - local val mapping to fields
      
/// Create the field for a "let" binding in a type definition.
///
/// The "v" is the local typed w.r.t. tyvars of the implicit ctor.
/// The formalTyparInst does the formal-typars/implicit-ctor-typars subst.
/// Field specifications added to a tcref must be in terms of the tcrefs formal typars.
let private MakeIncrClassField(g, cpath, formalTyparInst: TyparInstantiation, v: Val, isStatic, rfref: RecdFieldRef) =
    let name = rfref.FieldName
    let id = ident (name, v.Range)
    let ty = v.Type |> instType formalTyparInst
    let taccess = TAccess [cpath]
    let isVolatile = HasFSharpAttribute g g.attrib_VolatileFieldAttribute v.Attribs

    Construct.NewRecdField isStatic None id false ty v.IsMutable isVolatile [] v.Attribs v.XmlDoc taccess true

/// Indicates how is a 'let' bound value in a class with implicit construction is represented in
/// the TAST ultimately produced by type checking.    
type IncrClassValRepr = 

    // e.g representation for 'let v = 3' if it is not used in anything given a method representation
    | InVar of isArg: bool 

    // e.g representation for 'let v = 3'
    | InField of isStatic: bool * staticCountForSafeInit: int * fieldRef: RecdFieldRef

    // e.g representation for 'let f x = 3'
    | InMethod of isStatic:bool * value: Val * valReprInfo: ValReprInfo

/// IncrClassReprInfo represents the decisions we make about the representation of 'let' and 'do' bindings in a
/// type defined with implicit class construction.
type IncrClassReprInfo = 
    { 
        /// Indicates the set of field names taken within one incremental class
        TakenFieldNames: Set<string>
          
        RepInfoTcGlobals: TcGlobals
          
        /// vals mapped to representations
        ValReprs: Zmap<Val, IncrClassValRepr> 
          
        /// vals represented as fields or members from this point on 
        ValsWithRepresentation: Zset<Val> 
    }

    static member Empty(g, names) = 
        {   TakenFieldNames=Set.ofList names
            RepInfoTcGlobals=g
            ValReprs = Zmap.empty valOrder 
            ValsWithRepresentation = Zset.empty valOrder }

    /// Find the representation of a value
    member localRep.LookupRepr (v: Val) = 
        match Zmap.tryFind v localRep.ValReprs with 
        | None -> error(InternalError("LookupRepr: failed to find representation for value", v.Range))
        | Some res -> res

    static member IsMethodRepr (cenv: cenv) (bind: Binding) = 
        let g = cenv.g
        let v = bind.Var
        // unit fields are not stored, just run rhs for effects
        if isUnitTy g v.Type then 
            false
        else 
            let arity = InferArityOfExprBinding g AllowTypeDirectedDetupling.Yes v bind.Expr 
            not arity.HasNoArgs && not v.IsMutable


    /// <summary>
    /// Choose how a binding is represented
    /// </summary>
    /// <param name='cenv'></param>
    /// <param name='env'></param>
    /// <param name='isStatic'></param>
    /// <param name='isCtorArg'></param>
    /// <param name='ctorInfo'></param>
    /// <param name='staticForcedFieldVars'>The vars forced to be fields due to static member bindings, instance initialization expressions or instance member bindings</param>
    /// <param name='instanceForcedFieldVars'>The vars forced to be fields due to instance member bindings</param>
    /// <param name='takenFieldNames'></param>
    /// <param name='bind'></param>
    member localRep.ChooseRepresentation (cenv: cenv, env: TcEnv, isStatic, isCtorArg,
                                            ctorInfo: IncrClassCtorLhs,
                                            staticForcedFieldVars: FreeLocals,
                                            instanceForcedFieldVars: FreeLocals, 
                                            takenFieldNames: Set<string>, 
                                            bind: Binding) = 
        let g = cenv.g 
        let v = bind.Var
        let relevantForcedFieldVars = (if isStatic then staticForcedFieldVars else instanceForcedFieldVars)
            
        let tcref = ctorInfo.TyconRef
        let name, takenFieldNames = 

            let isNameTaken = 
                // Check if a implicit field already exists with this name
                takenFieldNames.Contains(v.LogicalName) ||
                // Check if a user-defined field already exists with this name. Struct fields have already been created - see bug FSharp 1.0 5304
                (tcref.GetFieldByName(v.LogicalName).IsSome && (isStatic || not tcref.IsFSharpStructOrEnumTycon)) 

            let nm = 
                if isNameTaken then 
                    ctorInfo.NameGenerator.FreshCompilerGeneratedName (v.LogicalName, v.Range)
                else 
                    v.LogicalName
            nm, takenFieldNames.Add nm
                 
        let reportIfUnused() = 
            if not v.HasBeenReferenced && not v.IsCompiledAsTopLevel && not (v.DisplayName.StartsWithOrdinal("_")) && not v.IsCompilerGenerated then 
                warning (Error(FSComp.SR.chkUnusedValue(v.DisplayName), v.Range))

        let repr = 
            match InferArityOfExprBinding g AllowTypeDirectedDetupling.Yes v bind.Expr with 
            | arity when arity.HasNoArgs || v.IsMutable -> 
                // all mutable variables are forced into fields, since they may escape into closures within the implicit constructor
                // e.g. 
                //     type C() =  
                //        let mutable m = 1
                //        let n = ... (fun () -> m) ....
                //
                // All struct variables are forced into fields. Structs may not contain "let" bindings, so no new variables can be 
                // introduced.
                    
                if v.IsMutable || relevantForcedFieldVars.Contains v || tcref.IsStructOrEnumTycon then 
                    //dprintfn "Representing %s as a field %s" v.LogicalName name
                    let rfref = RecdFieldRef(tcref, name)
                    reportIfUnused()
                    InField (isStatic, localRep.ValReprs.Count, rfref)
                else
                    //if not v.Attribs.IsEmpty then 
                    //    warning(Error(FSComp.SR.tcAttributesIgnoredOnLetBinding(), v.Range))
                    //dprintfn 
                    //    "Representing %s as a local variable %s, staticForcedFieldVars = %s, instanceForcedFieldVars = %s" 
                    //    v.LogicalName name 
                    //    (staticForcedFieldVars |> Seq.map (fun v -> v.LogicalName) |> String.concat ",")
                    //    (instanceForcedFieldVars |> Seq.map (fun v -> v.LogicalName) |> String.concat ",")
                    InVar isCtorArg
            | valReprInfo -> 
                //dprintfn "Representing %s as a method %s" v.LogicalName name
                let tps, _, argInfos, _, _ = GetValReprTypeInCompiledForm g valReprInfo 0 v.Type v.Range

                let valSynInfo = SynValInfo(argInfos |> List.mapSquared (fun (_, argInfo) -> SynArgInfo([], false, argInfo.Name)), SynInfo.unnamedRetVal)
                let memberFlags = (if isStatic then StaticMemberFlags else NonVirtualMemberFlags) SynMemberFlagsTrivia.Zero SynMemberKind.Member
                let id = mkSynId v.Range name
                let memberInfo = MakeMemberDataAndMangledNameForMemberVal(g, tcref, false, [], [], memberFlags, valSynInfo, mkSynId v.Range name, true)

                let copyOfTyconTypars = ctorInfo.GetNormalizedInstanceCtorDeclaredTypars cenv env.DisplayEnv ctorInfo.TyconRef.Range
                
                AdjustValToValRepr v (Parent tcref) valReprInfo

                // Add the 'this' pointer on to the function
                let memberTauTy, valReprInfo = 
                    let tauTy = v.TauType
                    if isStatic then 
                        tauTy, valReprInfo 
                    else 
                        let tauTy = mkFunTy g ctorInfo.InstanceCtorThisVal.Type v.TauType
                        let (ValReprInfo(tpNames, args, ret)) = valReprInfo
                        let valReprInfo = ValReprInfo(tpNames, ValReprInfo.selfMetadata :: args, ret)
                        tauTy, valReprInfo

                // Add the enclosing type parameters on to the function
                let valReprInfo = 
                    let (ValReprInfo(tpNames, args, ret)) = valReprInfo
                    ValReprInfo(tpNames@ValReprInfo.InferTyparInfo copyOfTyconTypars, args, ret)
                                          
                let prelimTyschemeG = GeneralizedType(copyOfTyconTypars@tps, memberTauTy)

                // NOTE: putting isCompilerGenerated=true here is strange.  The method is not public, nor is
                // it a "member" in the F# sense, but the F# spec says it is generated and it is reasonable to reflect on it.
                let memberValScheme = ValScheme(id, prelimTyschemeG, Some valReprInfo, None, Some memberInfo, false, ValInline.Never, NormalVal, None, true (* isCompilerGenerated *), true (* isIncrClass *), false, false)

                let methodVal = MakeAndPublishVal cenv env (Parent tcref, false, ModuleOrMemberBinding, ValNotInRecScope, memberValScheme, v.Attribs, XmlDoc.Empty, None, false) 

                reportIfUnused()
                InMethod(isStatic, methodVal, valReprInfo)

        repr, takenFieldNames

    /// Extend the known local representations by choosing a representation for a binding
    member localRep.ChooseAndAddRepresentation(cenv: cenv, env: TcEnv, isStatic, isCtorArg, ctorInfo: IncrClassCtorLhs, staticForcedFieldVars: FreeLocals, instanceForcedFieldVars: FreeLocals, bind: Binding) = 
        let v = bind.Var
        let repr, takenFieldNames = localRep.ChooseRepresentation (cenv, env, isStatic, isCtorArg, ctorInfo, staticForcedFieldVars, instanceForcedFieldVars, localRep.TakenFieldNames, bind )
        // OK, representation chosen, now add it 
        {localRep with 
            TakenFieldNames=takenFieldNames 
            ValReprs = Zmap.add v repr localRep.ValReprs}  

    member localRep.ValNowWithRepresentation (v: Val) = 
        {localRep with ValsWithRepresentation = Zset.add v localRep.ValsWithRepresentation}

    member localRep.IsValWithRepresentation (v: Val) = 
            localRep.ValsWithRepresentation.Contains v 

    member localRep.IsValRepresentedAsLocalVar (v: Val) =
        match localRep.LookupRepr v with 
        | InVar false -> true
        | _ -> false

    member localRep.IsValRepresentedAsMethod (v: Val) =
        localRep.IsValWithRepresentation v &&
        match localRep.LookupRepr v with 
        | InMethod _ -> true 
        | _ -> false

    /// Make the elaborated expression that represents a use of a 
    /// a "let v = ..." class binding
    member localRep.MakeValueLookup thisValOpt tinst safeStaticInitInfo v tyargs m =
        let g = localRep.RepInfoTcGlobals 
        match localRep.LookupRepr v, thisValOpt with 
        | InVar _, _ -> 
            exprForVal m v
        | InField(false, _idx, rfref), Some thisVal -> 
            let thise = exprForVal m thisVal
            mkRecdFieldGetViaExprAddr (thise, rfref, tinst, m)
        | InField(false, _idx, _rfref), None -> 
            error(InternalError("Unexpected missing 'this' variable in MakeValueLookup", m))
        | InField(true, idx, rfref), _ -> 
            let expr = mkStaticRecdFieldGet (rfref, tinst, m)
            MakeCheckSafeInit g tinst safeStaticInitInfo (mkInt g m idx) expr
                
        | InMethod(isStatic, methodVal, valReprInfo), _ -> 
            //dprintfn "Rewriting application of %s to be call to method %s" v.LogicalName methodVal.LogicalName
            let expr, exprTy = AdjustValForExpectedArity g m (mkLocalValRef methodVal) NormalValUse valReprInfo 
            // Prepend the the type arguments for the class
            let tyargs = tinst @ tyargs 
            let thisArgs =
                if isStatic then []
                else Option.toList (Option.map (exprForVal m) thisValOpt)
                    
            MakeApplicationAndBetaReduce g (expr, exprTy, [tyargs], thisArgs, m) 

    /// Make the elaborated expression that represents an assignment 
    /// to a "let mutable v = ..." class binding
    member localRep.MakeValueAssign thisValOpt tinst safeStaticInitInfo v expr m =
        let g = localRep.RepInfoTcGlobals 
        match localRep.LookupRepr v, thisValOpt with 
        | InField(false, _, rfref), Some thisVal -> 
            let thise = exprForVal m thisVal
            mkRecdFieldSetViaExprAddr(thise, rfref, tinst, expr, m)
        | InField(false, _, _rfref), None -> 
            error(InternalError("Unexpected missing 'this' variable in MakeValueAssign", m))
        | InVar _, _ -> 
            mkValSet m (mkLocalValRef v) expr
        | InField (true, idx, rfref), _ -> 
            let expr = mkStaticRecdFieldSet(rfref, tinst, expr, m)
            MakeCheckSafeInit g tinst safeStaticInitInfo (mkInt g m idx) expr
        | InMethod _, _ -> 
            error(InternalError("Local was given method storage, yet later it's been assigned to", m))
          
    member localRep.MakeValueGetAddress readonly thisValOpt tinst safeStaticInitInfo v m =
        let g = localRep.RepInfoTcGlobals 
        match localRep.LookupRepr v, thisValOpt with 
        | InField(false, _, rfref), Some thisVal -> 
            let thise = exprForVal m thisVal
            mkRecdFieldGetAddrViaExprAddr(readonly, thise, rfref, tinst, m)
        | InField(false, _, _rfref), None -> 
            error(InternalError("Unexpected missing 'this' variable in MakeValueGetAddress", m))
        | InField(true, idx, rfref), _ -> 
            let expr = mkStaticRecdFieldGetAddr(readonly, rfref, tinst, m)
            MakeCheckSafeInit g tinst safeStaticInitInfo (mkInt g m idx) expr
        | InVar _, _ -> 
            mkValAddr m readonly (mkLocalValRef v)
        | InMethod _, _ -> 
            error(InternalError("Local was given method storage, yet later it's address was required", m))

    /// Mutate a type definition by adding fields 
    /// Used as part of processing "let" bindings in a type definition. 
    member localRep.PublishIncrClassFields (cenv, denv, cpath, ctorInfo: IncrClassCtorLhs, safeStaticInitInfo) =    
        let tcref = ctorInfo.TyconRef
        let rfspecs = 
            [ for KeyValue(v, repr) in localRep.ValReprs do
                    match repr with 
                    | InField(isStatic, _, rfref) -> 
                        // Instance fields for structs are published earlier because the full set of fields is determined syntactically from the implicit
                        // constructor arguments. This is important for the "default value" and "does it have an implicit default constructor" 
                        // semantic conditions for structs - see bug FSharp 1.0 5304.
                        if isStatic || not tcref.IsFSharpStructOrEnumTycon then 
                            let ctorDeclaredTypars = ctorInfo.GetNormalizedInstanceCtorDeclaredTypars cenv denv ctorInfo.TyconRef.Range

                            // Note: tcrefObjTy contains the original "formal" typars, thisTy is the "fresh" one... f<>fresh. 
                            let revTypeInst = List.zip ctorDeclaredTypars (tcref.TyparsNoRange |> List.map mkTyparTy)

                            yield MakeIncrClassField(localRep.RepInfoTcGlobals, cpath, revTypeInst, v, isStatic, rfref)
                    | _ -> 
                        () 

              match safeStaticInitInfo with 
              | SafeInitField (_, fld) -> yield fld
              | NoSafeInitInfo -> () ]

        let recdFields = Construct.MakeRecdFieldsTable (rfspecs @ tcref.AllFieldsAsList)

        // Mutate the entity_tycon_repr to publish the fields
        tcref.Deref.entity_tycon_repr <- TFSharpObjectRepr { tcref.FSharpObjectModelTypeInfo with fsobjmodel_rfields = recdFields}  


    /// Given localRep saying how locals have been represented, e.g. as fields.
    /// Given an expr under a given thisVal context.
    //
    // Fix up the references to the locals, e.g.
    //     v -> this.fieldv
    //     f x -> this.method x
    member localRep.FixupIncrClassExprPhase2C cenv thisValOpt safeStaticInitInfo (thisTyInst: TypeInst) expr = 
        // fixup: intercept and expr rewrite
        let FixupExprNode rw e =
            //dprintfn "Fixup %s" (showL (exprL e))
            let g = localRep.RepInfoTcGlobals
            let e = NormalizeAndAdjustPossibleSubsumptionExprs g e
            match e with
            // Rewrite references to applied let-bound-functions-compiled-as-methods
            // Rewrite references to applied recursive let-bound-functions-compiled-as-methods
            // Rewrite references to applied recursive generic let-bound-functions-compiled-as-methods
            | Expr.App (Expr.Val (ValDeref v, _, _), _, tyargs, args, m) 
            | Expr.App (Expr.Link {contents = Expr.Val (ValDeref v, _, _) }, _, tyargs, args, m)  
            | Expr.App (Expr.Link {contents = Expr.App (Expr.Val (ValDeref v, _, _), _, tyargs, [], _) }, _, [], args, m)  
                    when localRep.IsValRepresentedAsMethod v && not (cenv.recUses.ContainsKey v) -> 

                    let expr = localRep.MakeValueLookup thisValOpt thisTyInst safeStaticInitInfo v tyargs m
                    let args = args |> List.map rw
                    Some (MakeApplicationAndBetaReduce g (expr, (tyOfExpr g expr), [], args, m)) 

            // Rewrite references to values stored as fields and first class uses of method values
            | Expr.Val (ValDeref v, _, m)                         
                when localRep.IsValWithRepresentation v -> 

                    //dprintfn "Found use of %s" v.LogicalName
                    Some (localRep.MakeValueLookup thisValOpt thisTyInst safeStaticInitInfo v [] m)

            // Rewrite assignments to mutable values stored as fields 
            | Expr.Op (TOp.LValueOp (LSet, ValDeref v), [], [arg], m) 
                when localRep.IsValWithRepresentation v ->
                    let arg = rw arg 
                    Some (localRep.MakeValueAssign thisValOpt thisTyInst safeStaticInitInfo v arg m)

            // Rewrite taking the address of mutable values stored as fields 
            | Expr.Op (TOp.LValueOp (LAddrOf readonly, ValDeref v), [], [], m) 
                when localRep.IsValWithRepresentation v ->
                    Some (localRep.MakeValueGetAddress readonly thisValOpt thisTyInst safeStaticInitInfo v m)

            | _ -> None

        RewriteExpr {   PreIntercept = Some FixupExprNode 
                        PostTransform = (fun _ -> None)
                        PreInterceptBinding = None
                        RewriteQuotations = true
                        StackGuard = StackGuard(TcClassRewriteStackGuardDepth) } expr 

type IncrClassConstructionBindingsPhase2C =
    | Phase2CBindings of IncrClassBindingGroup list
    | Phase2CCtorJustAfterSuperInit     
    | Phase2CCtorJustAfterLastLet    

/// <summary>
/// Given a set of 'let' bindings (static or not, recursive or not) that make up a class,
/// generate their initialization expression(s).
/// </summary>
/// <param name='cenv'></param>
/// <param name='env'></param>
/// <param name='ctorInfo'>The lhs information about the implicit constructor</param>
/// <param name='inheritsExpr'>The call to the super class constructor</param>
/// <param name='inheritsIsVisible'>Should we place a sequence point at the 'inheritedTys call?</param>
/// <param name='decs'>The declarations</param>
/// <param name='memberBinds'></param>
/// <param name='generalizedTyparsForRecursiveBlock'>Record any unconstrained type parameters generalized for the outer members as "free choices" in the let bindings</param>
/// <param name='safeStaticInitInfo'></param>
let MakeCtorForIncrClassConstructionPhase2C(
    cenv: cenv, 
    env: TcEnv,
    ctorInfo: IncrClassCtorLhs,
    inheritsExpr,
    inheritsIsVisible,
    decs: IncrClassConstructionBindingsPhase2C list, 
    memberBinds: Binding list,
    generalizedTyparsForRecursiveBlock, 
    safeStaticInitInfo: SafeInitData
) = 


    let denv = env.DisplayEnv 
    let g = cenv.g
    let thisVal = ctorInfo.InstanceCtorThisVal 

    let m = thisVal.Range
    let ctorDeclaredTypars = ctorInfo.GetNormalizedInstanceCtorDeclaredTypars cenv denv m

    ctorDeclaredTypars |> List.iter (SetTyparRigid env.DisplayEnv m)  

    // Reconstitute the type with the correct quantified type variables.
    ctorInfo.InstanceCtorVal.SetType (mkForallTyIfNeeded ctorDeclaredTypars ctorInfo.InstanceCtorVal.TauType)

    let freeChoiceTypars = ListSet.subtract typarEq generalizedTyparsForRecursiveBlock ctorDeclaredTypars

    let thisTyInst = List.map mkTyparTy ctorDeclaredTypars

    let accFreeInExpr acc expr =
        unionFreeVars acc (freeInExpr CollectLocalsNoCaching expr) 
            
    let accFreeInBinding acc (bind: Binding) = 
        accFreeInExpr acc bind.Expr
            
    let accFreeInBindings acc (binds: Binding list) = 
        (acc, binds) ||> List.fold accFreeInBinding

    // Find all the variables used in any method. These become fields.
    //   staticForcedFieldVars: FreeLocals: the vars forced to be fields due to static member bindings, instance initialization expressions or instance member bindings
    //   instanceForcedFieldVars: FreeLocals: the vars forced to be fields due to instance member bindings
                                            
    let staticForcedFieldVars, instanceForcedFieldVars = 
            let staticForcedFieldVars, instanceForcedFieldVars = 
                ((emptyFreeVars, emptyFreeVars), decs) ||> List.fold (fun (staticForcedFieldVars, instanceForcedFieldVars) dec -> 
                match dec with 
                | Phase2CCtorJustAfterLastLet
                | Phase2CCtorJustAfterSuperInit ->  
                    (staticForcedFieldVars, instanceForcedFieldVars)
                | Phase2CBindings decs ->
                    ((staticForcedFieldVars, instanceForcedFieldVars), decs) ||> List.fold (fun (staticForcedFieldVars, instanceForcedFieldVars) dec -> 
                        match dec with 
                        | IncrClassBindingGroup(binds, isStatic, _) -> 
                            let methodBinds = binds |> List.filter (IncrClassReprInfo.IsMethodRepr cenv) 
                            let staticForcedFieldVars = 
                                if isStatic then 
                                    // Any references to static variables in any static method force the variable to be represented as a field
                                    (staticForcedFieldVars, methodBinds) ||> accFreeInBindings
                                else
                                    // Any references to static variables in any instance bindings force the variable to be represented as a field
                                    (staticForcedFieldVars, binds) ||> accFreeInBindings
                                        
                            let instanceForcedFieldVars = 
                                // Any references to instance variables in any methods force the variable to be represented as a field
                                (instanceForcedFieldVars, methodBinds) ||> accFreeInBindings
                                        
                            (staticForcedFieldVars, instanceForcedFieldVars)
                        | IncrClassDo (e, isStatic, _) -> 
                            let staticForcedFieldVars = 
                                if isStatic then 
                                    staticForcedFieldVars
                                else
                                    unionFreeVars staticForcedFieldVars (freeInExpr CollectLocalsNoCaching e)
                            (staticForcedFieldVars, instanceForcedFieldVars)))
            let staticForcedFieldVars = (staticForcedFieldVars, memberBinds) ||> accFreeInBindings 
            let instanceForcedFieldVars = (instanceForcedFieldVars, memberBinds) ||> accFreeInBindings 
             
            // Any references to static variables in the 'inherits' expression force those static variables to be represented as fields
            let staticForcedFieldVars = (staticForcedFieldVars, inheritsExpr) ||> accFreeInExpr

            (staticForcedFieldVars.FreeLocals, instanceForcedFieldVars.FreeLocals)


    // Compute the implicit construction side effects of single 
    // 'let' or 'let rec' binding in the implicit class construction sequence 
    let TransBind (reps: IncrClassReprInfo) (TBind(v, rhsExpr, spBind)) =
        if v.MustInline then
            error(Error(FSComp.SR.tcLocalClassBindingsCannotBeInline(), v.Range))
        let rhsExpr = reps.FixupIncrClassExprPhase2C cenv (Some thisVal) safeStaticInitInfo thisTyInst rhsExpr
            
        // The initialization of the 'ref cell' variable for 'this' is the only binding which comes prior to the super init
        let isPriorToSuperInit = 
            match ctorInfo.InstanceCtorSafeThisValOpt with 
            | None -> false
            | Some v2 -> valEq v v2
                            
        match reps.LookupRepr v with
        | InMethod(isStatic, methodVal, _) -> 
            let _, chooseTps, tauExpr, tauTy, m = 
                match rhsExpr with 
                | Expr.TyChoose (chooseTps, b, _) -> [], chooseTps, b, (tyOfExpr g b), m 
                | Expr.TyLambda (_, tps, Expr.TyChoose (chooseTps, b, _), m, returnTy) -> tps, chooseTps, b, returnTy, m 
                | Expr.TyLambda (_, tps, b, m, returnTy) -> tps, [], b, returnTy, m 
                | e -> [], [], e, (tyOfExpr g e), e.Range
                    
            let chooseTps = chooseTps @ (ListSet.subtract typarEq freeChoiceTypars methodVal.Typars)

            // Add the 'this' variable as an argument
            let tauExpr, tauTy = 
                if isStatic then 
                    tauExpr, tauTy
                else
                    let e = mkLambda m thisVal (tauExpr, tauTy)
                    e, tyOfExpr g e

            // Replace the type parameters that used to be on the rhs with 
            // the full set of type parameters including the type parameters of the enclosing class
            let rhsExpr = mkTypeLambda m methodVal.Typars (mkTypeChoose m chooseTps tauExpr, tauTy)
            (isPriorToSuperInit, id), [TBind (methodVal, rhsExpr, spBind)]
            
        // If it's represented as a non-escaping local variable then just bind it to its value
        // If it's represented as a non-escaping local arg then no binding necessary (ctor args are already bound)
            
        | InVar isArg ->
            (isPriorToSuperInit, (fun e -> if isArg then e else mkLetBind m (TBind(v, rhsExpr, spBind)) e)), []

        | InField (isStatic, idx, _) ->
                // Use spBind if it available as the span for the assignment into the field
            let m =
                    match spBind, rhsExpr with 
                    // Don't generate big sequence points for functions in classes
                    | _, (Expr.Lambda _ | Expr.TyLambda _) -> v.Range
                    | DebugPointAtBinding.Yes m, _ -> m 
                    | _ -> v.Range

            let assignExpr = reps.MakeValueAssign (Some thisVal) thisTyInst NoSafeInitInfo v rhsExpr m

            let adjustSafeInitFieldExprOpt = 
                if isStatic then 
                    match safeStaticInitInfo with 
                    | SafeInitField (rfref, _) -> 
                        let setExpr = mkStaticRecdFieldSet (rfref, thisTyInst, mkInt g m idx, m)
                        let setExpr = reps.FixupIncrClassExprPhase2C cenv (Some thisVal) NoSafeInitInfo thisTyInst setExpr
                        Some setExpr
                    | NoSafeInitInfo -> 
                        None
                else
                    None

            (isPriorToSuperInit, (fun e -> 
                    let e =
                        match adjustSafeInitFieldExprOpt with
                        | None -> e
                        | Some adjustExpr -> mkCompGenSequential m adjustExpr e

                    let assignExpr =
                        match spBind with
                        | DebugPointAtBinding.Yes _ -> mkDebugPoint m assignExpr
                        | _ -> assignExpr

                    mkSequential m assignExpr e)), []

    /// Work out the implicit construction side effects of a 'let', 'let rec' or 'do' 
    /// binding in the implicit class construction sequence 
    let TransTrueDec isCtorArg (reps: IncrClassReprInfo) dec = 
            match dec with 
            | IncrClassBindingGroup(binds, isStatic, isRec) ->
                let actions, reps, methodBinds = 
                    let reps = (reps, binds) ||> List.fold (fun rep bind -> rep.ChooseAndAddRepresentation(cenv, env, isStatic, isCtorArg, ctorInfo, staticForcedFieldVars, instanceForcedFieldVars, bind)) // extend
                    if isRec then
                        // Note: the recursive calls are made via members on the object
                        // or via access to fields. This means the recursive loop is "broken", 
                        // and we can collapse to sequential bindings 
                        let reps = (reps, binds) ||> List.fold (fun rep bind -> rep.ValNowWithRepresentation bind.Var) // in scope before
                        let actions, methodBinds = binds |> List.map (TransBind reps) |> List.unzip // since can occur in RHS of own defns 
                        actions, reps, methodBinds
                    else 
                        let actions, methodBinds = binds |> List.map (TransBind reps) |> List.unzip
                        let reps = (reps, binds) ||> List.fold (fun rep bind -> rep.ValNowWithRepresentation bind.Var) // in scope after
                        actions, reps, methodBinds
                let methodBinds = List.concat methodBinds
                if isStatic then 
                    (actions, [], methodBinds), reps
                else 
                    ([], actions, methodBinds), reps

            | IncrClassDo (doExpr, isStatic, mFull) -> 
                let doExpr = reps.FixupIncrClassExprPhase2C cenv (Some thisVal) safeStaticInitInfo thisTyInst doExpr
                // Extend the range of any immediate debug point to include the 'do'
                let doExpr =
                    match doExpr with
                    | Expr.DebugPoint(_, innerExpr) -> Expr.DebugPoint(DebugPointAtLeafExpr.Yes mFull, innerExpr)
                    | e -> e
                let binder = (fun e -> mkSequential mFull doExpr e)
                let isPriorToSuperInit = false
                if isStatic then 
                    ([(isPriorToSuperInit, binder)], [], []), reps
                else 
                    ([], [(isPriorToSuperInit, binder)], []), reps


    /// Work out the implicit construction side effects of each declaration 
    /// in the implicit class construction sequence 
    let TransDec (reps: IncrClassReprInfo) dec = 
        match dec with 
        // The call to the base class constructor is done so we can set the ref cell 
        | Phase2CCtorJustAfterSuperInit ->  
            let binders = 
                [ match ctorInfo.InstanceCtorSafeThisValOpt with 
                    | None -> ()
                    | Some v -> 
                    let setExpr = mkRefCellSet g m ctorInfo.InstanceCtorThisVal.Type (exprForVal m v) (exprForVal m ctorInfo.InstanceCtorThisVal)
                    let setExpr = reps.FixupIncrClassExprPhase2C cenv (Some thisVal) safeStaticInitInfo thisTyInst setExpr
                    let binder = (fun e -> mkSequential setExpr.Range setExpr e)
                    let isPriorToSuperInit = false
                    yield (isPriorToSuperInit, binder) ]

            ([], binders, []), reps

        // The last 'let' binding is done so we can set the initialization condition for the collection of object fields
        // which now allows members to be called.
        | Phase2CCtorJustAfterLastLet ->  
            let binders = 
                [ match ctorInfo.InstanceCtorSafeInitInfo with 
                  | SafeInitField (rfref, _) ->  
                    let setExpr = mkRecdFieldSetViaExprAddr (exprForVal m thisVal, rfref, thisTyInst, mkOne g m, m)
                    let setExpr = reps.FixupIncrClassExprPhase2C cenv (Some thisVal) safeStaticInitInfo thisTyInst setExpr
                    let binder = (fun e -> mkSequential setExpr.Range setExpr e)
                    let isPriorToSuperInit = false
                    yield (isPriorToSuperInit, binder)  
                  | NoSafeInitInfo ->  
                    () ]

            ([], binders, []), reps
                
        | Phase2CBindings decs -> 
            let initActions, reps = List.mapFold (TransTrueDec false) reps decs 
            let cctorInitActions, ctorInitActions, methodBinds = List.unzip3 initActions
            (List.concat cctorInitActions, List.concat ctorInitActions, List.concat methodBinds), reps 

    let takenFieldNames = 
        [ for b in memberBinds do 
            b.Var.CompiledName g.CompilerGlobalState
            b.Var.DisplayName 
            b.Var.DisplayNameCoreMangled 
            b.Var.LogicalName ] 

    let reps = IncrClassReprInfo.Empty(g, takenFieldNames)

    // Bind the IsArg(true) representations of the object constructor arguments and assign them to fields
    // if they escape to the members. We do this by running the instance bindings 'let x = x' through TransTrueDec
    // for each constructor argument 'x', but with the special flag 'isCtorArg', which helps TransBind know that 
    // the value is already available as an argument, and that nothing special needs to be done unless the 
    // value is being stored into a field.
    let (cctorInitActions1, ctorInitActions1, methodBinds1), reps = 
        let binds = ctorInfo.InstanceCtorArgs |> List.map (fun v -> mkInvisibleBind v (exprForVal v.Range v))
        TransTrueDec true reps (IncrClassBindingGroup(binds, false, false))

    // We expect that only ctorInitActions1 will be non-empty here, and even then only if some elements are stored in the field
    assert (isNil cctorInitActions1)
    assert (isNil methodBinds1)

    // Now deal with all the 'let' and 'member' declarations
    let initActions, reps = List.mapFold TransDec reps decs
    let cctorInitActions2, ctorInitActions2, methodBinds2 = List.unzip3 initActions
    let cctorInitActions = cctorInitActions1 @ List.concat cctorInitActions2
    let ctorInitActions = ctorInitActions1 @ List.concat ctorInitActions2
    let methodBinds = methodBinds1 @ List.concat methodBinds2

    let ctorBody =
        // Build the elements of the implicit constructor body, starting from the bottom
        //     <optional-this-ref-cell-init>
        //     <super init>
        //     <let/do bindings>
        //     return ()
        let ctorInitActionsPre, ctorInitActionsPost = ctorInitActions |> List.partition fst

        // This is the return result
        let ctorBody = mkUnit g m

        // Add <optional-this-ref-cell-init>.
        // That is, add any <let/do bindings> that come prior to the super init constructor call, 
        // This is only ever at most the init of the InstanceCtorSafeThisValOpt and InstanceCtorSafeInitInfo var/field
        let ctorBody = List.foldBack (fun (_, binder) acc -> binder acc) ctorInitActionsPost ctorBody
            
        // Add the <super init>
        let ctorBody = 
            // The inheritsExpr may refer to the this variable or to incoming arguments, e.g. in closure fields.
            // References to the this variable go via the ref cell that gets created to help ensure coherent initialization.
            // This ref cell itself may be stored in a field of the object and accessed via arg0.
            // Likewise the incoming arguments will eventually be stored in fields and accessed via arg0.
            // 
            // As a result, the most natural way to implement this would be to simply capture arg0 if needed
            // and access all variables via that. This would be done by rewriting the inheritsExpr as follows:
            //    let inheritsExpr = reps.FixupIncrClassExprPhase2C (Some thisVal) thisTyInst inheritsExpr
            // However, the rules of IL mean we are not actually allowed to capture arg0 
            // and store it as a closure field before the base class constructor is called.
            // 
            // As a result we do not rewrite the inheritsExpr and instead 
            //    (a) wrap a let binding for the ref cell around the inheritsExpr if needed
            //    (b) rely on the fact that the input arguments are in scope and can be accessed from as argument variables
            //    (c) rely on the fact that there are no 'let' bindings prior to the inherits expr.
            let inheritsExpr = 
                match ctorInfo.InstanceCtorSafeThisValOpt with 
                | Some v when not (reps.IsValRepresentedAsLocalVar v) -> 
                    // Rewrite the expression to convert it to a load of a field if needed.
                    // We are allowed to load fields from our own object even though we haven't called
                    // the super class constructor yet.
                    let ldexpr = reps.FixupIncrClassExprPhase2C cenv (Some thisVal) safeStaticInitInfo thisTyInst (exprForVal m v) 
                    mkInvisibleLet m v ldexpr inheritsExpr
                | _ -> 
                    inheritsExpr

            // Add the debug point
            let inheritsExpr =
                if inheritsIsVisible then
                    Expr.DebugPoint(DebugPointAtLeafExpr.Yes inheritsExpr.Range, inheritsExpr)
                else
                    inheritsExpr
                
            mkSequential m inheritsExpr ctorBody

        // Add the normal <let/do bindings> 
        let ctorBody = List.foldBack (fun (_, binder) acc -> binder acc) ctorInitActionsPre ctorBody

        // Add the final wrapping to make this into a method
        let ctorBody = mkMemberLambdas g m [] (Some thisVal) ctorInfo.InstanceCtorBaseValOpt [ctorInfo.InstanceCtorArgs] (ctorBody, g.unit_ty)

        ctorBody

    let cctorBodyOpt =
        // Omit the .cctor if it's empty
        match cctorInitActions with
        | [] -> None 
        | _ -> 
            let cctorInitAction = List.foldBack (fun (_, binder) acc -> binder acc) cctorInitActions (mkUnit g m)
            let m = thisVal.Range
            let cctorArgs, cctorVal, _ = ctorInfo.StaticCtorValInfo.Force()
            // Reconstitute the type of the implicit class constructor with the correct quantified type variables.
            cctorVal.SetType (mkForallTyIfNeeded ctorDeclaredTypars cctorVal.TauType)
            let cctorBody = mkMemberLambdas g m [] None None [cctorArgs] (cctorInitAction, g.unit_ty)
            Some cctorBody
        
    ctorBody, cctorBodyOpt, methodBinds, reps
