// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckDeclarations

open System
open System.Collections.Generic

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Internal.Utilities.Library.ResultOrException
open FSharp.Compiler 
open FSharp.Compiler.AbstractIL.IL 
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.AttributeChecking
open FSharp.Compiler.CheckComputationExpressions
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CheckIncrementalClasses
open FSharp.Compiler.CheckPatterns
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Infos
open FSharp.Compiler.InfoReader
open FSharp.Compiler.MethodOverrides
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeHierarchy
open FSharp.Compiler.TypeRelations

#if !NO_TYPEPROVIDERS
open FSharp.Compiler.TypeProviders
#endif

type cenv = TcFileState

let TcClassRewriteStackGuardDepth = StackGuard.GetDepthOption "TcClassRewrite"

//-------------------------------------------------------------------------
// Mutually recursive shapes
//------------------------------------------------------------------------- 

type MutRecDataForOpen = MutRecDataForOpen of SynOpenDeclTarget * range * appliedScope: range * OpenDeclaration list ref
type MutRecDataForModuleAbbrev = MutRecDataForModuleAbbrev of Ident * LongIdent * range

/// Represents the shape of a mutually recursive group of declarations including nested modules
[<RequireQualifiedAccess>]
type MutRecShape<'TypeData, 'LetsData, 'ModuleData> = 
    | Tycon of 'TypeData
    | Lets of 'LetsData
    | Module of 'ModuleData * MutRecShapes<'TypeData, 'LetsData, 'ModuleData> 
    | ModuleAbbrev of MutRecDataForModuleAbbrev
    | Open of MutRecDataForOpen

and MutRecShapes<'TypeData, 'LetsData, 'ModuleData> = MutRecShape<'TypeData, 'LetsData, 'ModuleData> list

//-------------------------------------------------------------------------
// Mutually recursive shapes
//------------------------------------------------------------------------- 

module MutRecShapes = 
   let rec map f1 f2 f3 x = 
       x |> List.map (function 
           | MutRecShape.Open a -> MutRecShape.Open a
           | MutRecShape.ModuleAbbrev b -> MutRecShape.ModuleAbbrev b
           | MutRecShape.Tycon a -> MutRecShape.Tycon (f1 a)
           | MutRecShape.Lets b -> MutRecShape.Lets (f2 b)
           | MutRecShape.Module (c, d) -> MutRecShape.Module (f3 c, map f1 f2 f3 d))

   let mapTycons f1 xs = map f1 id id xs
   let mapTyconsAndLets f1 f2 xs = map f1 f2 id xs
   let mapLets f2 xs = map id f2 id xs
   let mapModules f1 xs = map id id f1 xs

   let rec mapWithEnv fTycon fLets (env: 'Env) x = 
       x |> List.map (function 
           | MutRecShape.Open a -> MutRecShape.Open a
           | MutRecShape.ModuleAbbrev a -> MutRecShape.ModuleAbbrev a
           | MutRecShape.Tycon a -> MutRecShape.Tycon (fTycon env a)
           | MutRecShape.Lets b -> MutRecShape.Lets (fLets env b)
           | MutRecShape.Module ((c, env2), d) -> MutRecShape.Module ((c, env2), mapWithEnv fTycon fLets env2 d))

   let mapTyconsWithEnv f1 env xs = mapWithEnv f1 (fun _env x -> x) env xs

   let rec mapWithParent parent f1 f2 f3 xs = 
       xs |> List.map (function 
           | MutRecShape.Open a -> MutRecShape.Open a
           | MutRecShape.ModuleAbbrev a -> MutRecShape.ModuleAbbrev a
           | MutRecShape.Tycon a -> MutRecShape.Tycon (f2 parent a)
           | MutRecShape.Lets b -> MutRecShape.Lets (f3 parent b)
           | MutRecShape.Module (c, d) -> 
               let c2, parent2 = f1 parent c d
               MutRecShape.Module (c2, mapWithParent parent2 f1 f2 f3 d))

   let rec computeEnvs f1 f2 (env: 'Env) xs = 
       let env = f2 env xs
       env, 
       xs |> List.map (function 
           | MutRecShape.Open a -> MutRecShape.Open a
           | MutRecShape.ModuleAbbrev a -> MutRecShape.ModuleAbbrev a
           | MutRecShape.Tycon a -> MutRecShape.Tycon a
           | MutRecShape.Lets b -> MutRecShape.Lets b
           | MutRecShape.Module (c, ds) -> 
               let env2 = f1 env c
               let env3, ds2 = computeEnvs f1 f2 env2 ds
               MutRecShape.Module ((c, env3), ds2))

   let rec extendEnvs f1 (env: 'Env) xs = 
       let env = f1 env xs
       env, 
       xs |> List.map (function 
           | MutRecShape.Module ((c, env), ds) -> 
               let env2, ds2 = extendEnvs f1 env ds
               MutRecShape.Module ((c, env2), ds2)
           | x -> x)

   let dropEnvs xs = xs |> mapModules fst

   let rec expandTyconsWithEnv f1 env xs = 
       let preBinds, postBinds = 
           xs |> List.map (fun elem -> 
               match elem with 
               | MutRecShape.Tycon a -> f1 env a
               | _ -> [], [])
            |> List.unzip
       [MutRecShape.Lets (List.concat preBinds)] @ 
       (xs |> List.map (fun elem -> 
           match elem with 
           | MutRecShape.Module ((c, env2), d) -> MutRecShape.Module ((c, env2), expandTyconsWithEnv f1 env2 d)
           | _ -> elem)) @
       [MutRecShape.Lets (List.concat postBinds)] 

   let rec mapFoldWithEnv f1 z env xs = 
       (z, xs) ||> List.mapFold (fun z x ->
           match x with
           | MutRecShape.Module ((c, env2), ds) -> let ds2, z = mapFoldWithEnv f1 z env2 ds in MutRecShape.Module ((c, env2), ds2), z
           | _ -> let x2, z = f1 z env x in x2, z)


   let rec collectTycons x = 
       x |> List.collect (function 
           | MutRecShape.Tycon a -> [a]
           | MutRecShape.Module (_, d) -> collectTycons d
           | _ -> [])

   let topTycons x = 
       x |> List.choose (function MutRecShape.Tycon a -> Some a | _ -> None)

   let rec iter f1 f2 f3 f4 f5 x = 
       x |> List.iter (function 
           | MutRecShape.Tycon a -> f1 a
           | MutRecShape.Lets b -> f2 b
           | MutRecShape.Module (c, d) -> f3 c; iter f1 f2 f3 f4 f5 d
           | MutRecShape.Open a -> f4 a
           | MutRecShape.ModuleAbbrev a -> f5 a)

   let iterTycons f1 x = iter f1 ignore ignore ignore ignore x
   let iterTyconsAndLets f1 f2 x = iter f1 f2 ignore ignore ignore x
   let iterModules f1 x = iter ignore ignore f1 ignore ignore x

   let rec iterWithEnv f1 f2 f3 f4 env x = 
       x |> List.iter (function 
           | MutRecShape.Tycon a -> f1 env a
           | MutRecShape.Lets b -> f2 env b
           | MutRecShape.Module ((_, env), d) -> iterWithEnv f1 f2 f3 f4 env d
           | MutRecShape.Open a -> f3 env a
           | MutRecShape.ModuleAbbrev a -> f4 env a)

   let iterTyconsWithEnv f1 env xs = iterWithEnv f1 (fun _env _x -> ()) (fun _env _x -> ()) (fun _env _x -> ()) env xs


/// Indicates a declaration is contained in the given module 
let ModuleOrNamespaceContainerInfo modref = ContainerInfo(Parent modref, Some(MemberOrValContainerInfo(modref, None, None, NoSafeInitInfo, [])))

/// Indicates a declaration is contained in the given type definition in the given module 
let TyconContainerInfo (parent, tcref, declaredTyconTypars, safeInitInfo) = ContainerInfo(parent, Some(MemberOrValContainerInfo(tcref, None, None, safeInitInfo, declaredTyconTypars)))

type TyconBindingDefn = TyconBindingDefn of ContainerInfo * NewSlotsOK * DeclKind * SynMemberDefn * range

type MutRecSigsInitialData = MutRecShape<SynTypeDefnSig, SynValSig, SynComponentInfo> list
type MutRecDefnsInitialData = MutRecShape<SynTypeDefn, SynBinding list, SynComponentInfo> list

type MutRecDefnsPhase1DataForTycon = MutRecDefnsPhase1DataForTycon of SynComponentInfo * SynTypeDefnSimpleRepr * (SynType * range) list * preEstablishedHasDefaultCtor: bool * hasSelfReferentialCtor: bool * isAtOriginalTyconDefn: bool
type MutRecDefnsPhase1Data = MutRecShape<MutRecDefnsPhase1DataForTycon * SynMemberDefn list, RecDefnBindingInfo list, SynComponentInfo> list

type MutRecDefnsPhase2DataForTycon = MutRecDefnsPhase2DataForTycon of Tycon option * ParentRef * DeclKind * TyconRef * Val option * SafeInitData * Typars * SynMemberDefn list * range * NewSlotsOK * fixupFinalAttribs: (unit -> unit)
type MutRecDefnsPhase2DataForModule = MutRecDefnsPhase2DataForModule of ModuleOrNamespaceType ref * ModuleOrNamespace
type MutRecDefnsPhase2Data = MutRecShape<MutRecDefnsPhase2DataForTycon, RecDefnBindingInfo list, MutRecDefnsPhase2DataForModule * TcEnv> list

type MutRecDefnsPhase2InfoForTycon = MutRecDefnsPhase2InfoForTycon of Tycon option * TyconRef * Typars * DeclKind * TyconBindingDefn list * fixupFinalAttrs: (unit -> unit)
type MutRecDefnsPhase2Info = MutRecShape<MutRecDefnsPhase2InfoForTycon, RecDefnBindingInfo list, MutRecDefnsPhase2DataForModule * TcEnv> list

//-------------------------------------------------------------------------
// Helpers for TcEnv
//------------------------------------------------------------------------- 

/// Add an exception definition to TcEnv and report it to the sink
let AddLocalExnDefnAndReport tcSink scopem env (exnc: Tycon) =
    let env = { env with eNameResEnv = AddExceptionDeclsToNameEnv BulkAdd.No env.eNameResEnv (mkLocalEntityRef exnc) }
    // Also make VisualStudio think there is an identifier in scope at the range of the identifier text of its binding location
    CallEnvSink tcSink (exnc.Range, env.NameEnv, env.AccessRights)
    CallEnvSink tcSink (scopem, env.NameEnv, env.AccessRights)
    env
 
/// Add a list of type definitions to TcEnv
let AddLocalTyconRefs ownDefinition g amap m tcrefs env =
    if isNil tcrefs then env else
    { env with eNameResEnv = AddTyconRefsToNameEnv BulkAdd.No ownDefinition g amap env.eAccessRights m false env.eNameResEnv tcrefs }

/// Add a list of type definitions to TcEnv
let AddLocalTycons g amap m (tycons: Tycon list) env =
    if isNil tycons then env else
    env |> AddLocalTyconRefs false g amap m (List.map mkLocalTyconRef tycons) 

/// Add a list of type definitions to TcEnv and report them to the sink
let AddLocalTyconsAndReport tcSink scopem g amap m tycons env = 
    let env = AddLocalTycons g amap m tycons env
    CallEnvSink tcSink (scopem, env.NameEnv, env.eAccessRights)
    env

/// Add a "module X = ..." definition to the TcEnv 
let AddLocalSubModule g amap m env (moduleEntity: ModuleOrNamespace) =
    let env = { env with
                    eNameResEnv = AddModuleOrNamespaceRefToNameEnv g amap m false env.eAccessRights env.eNameResEnv (mkLocalModuleRef moduleEntity)
                    eUngeneralizableItems = addFreeItemOfModuleTy moduleEntity.ModuleOrNamespaceType env.eUngeneralizableItems }
    env

/// Add a "module X = ..." definition to the TcEnv and report it to the sink
let AddLocalSubModuleAndReport tcSink scopem g amap m env (moduleEntity: ModuleOrNamespace) =
    let env = AddLocalSubModule g amap m env moduleEntity
    if not (equals scopem m) then
        // Don't report another environment for top-level module at its own range,
        // so it doesn't overwrite inner environment used by features like code completion. 
        CallEnvSink tcSink (scopem, env.NameEnv, env.eAccessRights)
    env

/// Given an inferred module type, place that inside a namespace path implied by a "namespace X.Y.Z" definition
let BuildRootModuleType enclosingNamespacePath (cpath: CompilationPath) moduleTy = 
    (enclosingNamespacePath, (cpath, (moduleTy, []))) 
        ||> List.foldBack (fun id (cpath, (moduleTy, moduls)) ->
            let a, b = wrapModuleOrNamespaceTypeInNamespace id cpath.ParentCompPath moduleTy 
            cpath.ParentCompPath, (a, b :: moduls))
        |> fun (_, (moduleTy, moduls)) -> moduleTy, List.rev moduls
        
/// Given a resulting module expression, place that inside a namespace path implied by a "namespace X.Y.Z" definition
let BuildRootModuleContents (isModule: bool) enclosingNamespacePath (cpath: CompilationPath) moduleContents = 
    (enclosingNamespacePath, (cpath, moduleContents)) 
        ||> List.foldBack (fun id (cpath, moduleContents) -> (cpath.ParentCompPath, wrapModuleOrNamespaceContentsInNamespace isModule id cpath.ParentCompPath moduleContents))
        |> snd

/// Try to take the "FSINNN" prefix off a namespace path
let TryStripPrefixPath (g: TcGlobals) (enclosingNamespacePath: Ident list) = 
    match enclosingNamespacePath with 
    | p :: rest when
        g.isInteractive &&
        not (isNil rest) &&
        p.idText.StartsWithOrdinal FsiDynamicModulePrefix && 
        p.idText[FsiDynamicModulePrefix.Length..] |> String.forall Char.IsDigit 
        -> Some(p, rest)
    | _ -> None

/// Add a "module X = Y" local module abbreviation to the TcEnv 
let AddModuleAbbreviationAndReport tcSink scopem id modrefs env =
    let env = 
        if isNil modrefs then env else
        { env with eNameResEnv = AddModuleAbbrevToNameEnv id env.eNameResEnv modrefs }

    CallEnvSink tcSink (scopem, env.NameEnv, env.eAccessRights)
    let item = Item.ModuleOrNamespaces modrefs
    CallNameResolutionSink tcSink (id.idRange, env.NameEnv, item, emptyTyparInst, ItemOccurence.Use, env.AccessRights)
    env

/// Adjust the TcEnv to account for opening the set of modules or namespaces implied by an `open` declaration
let OpenModuleOrNamespaceRefs tcSink g amap scopem root env mvvs openDeclaration =
    let env =
        if isNil mvvs then env else
        { env with eNameResEnv = AddModuleOrNamespaceRefsContentsToNameEnv g amap env.eAccessRights scopem root env.eNameResEnv mvvs }
    CallEnvSink tcSink (scopem, env.NameEnv, env.eAccessRights)
    CallOpenDeclarationSink tcSink openDeclaration
    env

/// Adjust the TcEnv to account for opening a type implied by an `open type` declaration
let OpenTypeContent tcSink g amap scopem env (ty: TType) openDeclaration =
    let env =
        { env with eNameResEnv = AddTypeContentsToNameEnv g amap env.eAccessRights scopem env.eNameResEnv ty }
    CallEnvSink tcSink (scopem, env.NameEnv, env.eAccessRights)
    CallOpenDeclarationSink tcSink openDeclaration
    env

/// Adjust the TcEnv to account for a new root Ccu being available, e.g. a referenced assembly
let AddRootModuleOrNamespaceRefs g amap m env modrefs =
    if isNil modrefs then env else
    { env with eNameResEnv = AddModuleOrNamespaceRefsToNameEnv g amap m true env.eAccessRights env.eNameResEnv modrefs }

/// Adjust the TcEnv to make more things 'InternalsVisibleTo'
let addInternalsAccessibility env (ccu: CcuThunk) =
    let compPath = CompPath (ccu.ILScopeRef, [])    
    let eInternalsVisibleCompPaths = compPath :: env.eInternalsVisibleCompPaths
    { env with 
        eAccessRights = ComputeAccessRights env.eAccessPath eInternalsVisibleCompPaths env.eFamilyType // update this computed field
        eInternalsVisibleCompPaths = compPath :: env.eInternalsVisibleCompPaths }

/// Adjust the TcEnv to account for a new referenced assembly
let AddNonLocalCcu g amap scopem env assemblyName (ccu: CcuThunk, internalsVisibleToAttributes) = 

    let internalsVisible = 
        internalsVisibleToAttributes 
        |> List.exists (fun visibleTo ->             
            try
                System.Reflection.AssemblyName(visibleTo).Name = assemblyName                
            with _ ->
                warning(InvalidInternalsVisibleToAssemblyName(visibleTo, ccu.FileName))
                false)

    let env = if internalsVisible then addInternalsAccessibility env ccu else env

    // Compute the top-rooted module or namespace references
    let modrefs = ccu.RootModulesAndNamespaces |> List.map (mkNonLocalCcuRootEntityRef ccu) 

    // Compute the top-rooted type definitions
    let tcrefs = ccu.RootTypeAndExceptionDefinitions |> List.map (mkNonLocalCcuRootEntityRef ccu) 
    let env = AddRootModuleOrNamespaceRefs g amap scopem env modrefs
    let env =
        if isNil tcrefs then env else
        { env with eNameResEnv = AddTyconRefsToNameEnv BulkAdd.Yes false g amap env.eAccessRights scopem true env.eNameResEnv tcrefs }
    env

/// Adjust the TcEnv to account for a fully processed "namespace" declaration in this file
let AddLocalRootModuleOrNamespace tcSink g amap scopem env (moduleTy: ModuleOrNamespaceType) = 
    // Compute the top-rooted module or namespace references
    let modrefs = moduleTy.ModuleAndNamespaceDefinitions |> List.map mkLocalModuleRef
    // Compute the top-rooted type definitions
    let tcrefs = moduleTy.TypeAndExceptionDefinitions |> List.map mkLocalTyconRef
    let env = AddRootModuleOrNamespaceRefs g amap scopem env modrefs
    let env = { env with
                    eNameResEnv = if isNil tcrefs then env.eNameResEnv else AddTyconRefsToNameEnv BulkAdd.No false g amap env.eAccessRights scopem true env.eNameResEnv tcrefs
                    eUngeneralizableItems = addFreeItemOfModuleTy moduleTy env.eUngeneralizableItems }
    CallEnvSink tcSink (scopem, env.NameEnv, env.eAccessRights)
    env

/// Inside "namespace X.Y.Z" there is an implicit open of "X.Y.Z"
let ImplicitlyOpenOwnNamespace tcSink g amap scopem enclosingNamespacePath (env: TcEnv) = 
    if isNil enclosingNamespacePath then 
        env
    else
        // For F# interactive, skip "FSI_0002" prefixes when determining the path to open implicitly
        let enclosingNamespacePathToOpen = 
            match TryStripPrefixPath g enclosingNamespacePath with 
            | Some(_, rest) -> rest
            | None -> enclosingNamespacePath

        match enclosingNamespacePathToOpen with
        | id :: rest ->
            let ad = env.AccessRights
            match ResolveLongIdentAsModuleOrNamespace tcSink ResultCollectionSettings.AllResults amap scopem true OpenQualified env.eNameResEnv ad id rest true with 
            | Result modrefs -> 
                let modrefs = List.map p23 modrefs
                let openTarget = SynOpenDeclTarget.ModuleOrNamespace(enclosingNamespacePathToOpen, scopem)
                let openDecl = OpenDeclaration.Create (openTarget, modrefs, [], scopem, true)
                OpenModuleOrNamespaceRefs tcSink g amap scopem false env modrefs openDecl
            | Exception _ -> env
        | _ -> env

//-------------------------------------------------------------------------
// Bind elements of data definitions for exceptions and types (fields, etc.)
//------------------------------------------------------------------------- 

exception NotUpperCaseConstructor of range: range

let CheckNamespaceModuleOrTypeName (g: TcGlobals) (id: Ident) = 
    // type names '[]' etc. are used in fslib
    if not g.compilingFSharpCore && id.idText.IndexOfAny IllegalCharactersInTypeAndNamespaceNames <> -1 then 
        errorR(Error(FSComp.SR.tcInvalidNamespaceModuleTypeUnionName(), id.idRange))

let CheckDuplicates (idf: _ -> Ident) k elems = 
    elems |> List.iteri (fun i uc1 -> 
        elems |> List.iteri (fun j uc2 -> 
            let id1 = (idf uc1)
            let id2 = (idf uc2)
            if j > i && id1.idText = id2.idText then 
                errorR (Duplicate(k, id1.idText, id1.idRange))))
    elems


module TcRecdUnionAndEnumDeclarations =

    let CombineReprAccess parent vis = 
        match parent with 
        | ParentNone -> vis 
        | Parent tcref -> combineAccess vis tcref.TypeReprAccessibility

    let MakeRecdFieldSpec _cenv env parent (isStatic, konst, tyR, attrsForProperty, attrsForField, id, nameGenerated, isMutable, vol, xmldoc, vis, m) =
        let vis, _ = ComputeAccessAndCompPath env None m vis None parent
        let vis = CombineReprAccess parent vis
        Construct.NewRecdField isStatic konst id nameGenerated tyR isMutable vol attrsForProperty attrsForField xmldoc vis false

    let TcFieldDecl (cenv: cenv) env parent isIncrClass tpenv (isStatic, synAttrs, id, nameGenerated, ty, isMutable, xmldoc, vis, m) =
        let g = cenv.g
        let attrs, _ = TcAttributesWithPossibleTargets false cenv env AttributeTargets.FieldDecl synAttrs
        let attrsForProperty, attrsForField = attrs |> List.partition (fun (attrTargets, _) -> (attrTargets &&& AttributeTargets.Property) <> enum 0) 
        let attrsForProperty = (List.map snd attrsForProperty) 
        let attrsForField = (List.map snd attrsForField)
        let tyR, _ = TcTypeAndRecover cenv NoNewTypars CheckCxs ItemOccurence.UseInType env tpenv ty
        let zeroInit = HasFSharpAttribute g g.attrib_DefaultValueAttribute attrsForField
        let isVolatile = HasFSharpAttribute g g.attrib_VolatileFieldAttribute attrsForField
        
        let isThreadStatic = isThreadOrContextStatic g attrsForField
        if isThreadStatic && (not zeroInit || not isStatic) then 
            error(Error(FSComp.SR.tcThreadStaticAndContextStaticMustBeStatic(), m))

        if isVolatile then 
            error(Error(FSComp.SR.tcVolatileOnlyOnClassLetBindings(), m))

        if isIncrClass && (not zeroInit || not isMutable) then errorR(Error(FSComp.SR.tcUninitializedValFieldsMustBeMutable(), m))
        let isPrivate = match vis with | Some (SynAccess.Private _) -> true | _ -> false
        if isStatic && (not zeroInit || not isMutable || not isPrivate) then errorR(Error(FSComp.SR.tcStaticValFieldsMustBeMutableAndPrivate(), m))
        let konst = if zeroInit then Some Const.Zero else None
        let rfspec = MakeRecdFieldSpec cenv env parent (isStatic, konst, tyR, attrsForProperty, attrsForField, id, nameGenerated, isMutable, isVolatile, xmldoc, vis, m)
        match parent with
        | Parent tcref when useGenuineField tcref.Deref rfspec ->
            // Recheck the attributes for errors if the definition only generates a field
            TcAttributesWithPossibleTargets false cenv env AttributeTargets.FieldDeclRestricted synAttrs |> ignore
        | _ -> ()
        rfspec

    let TcAnonFieldDecl cenv env parent tpenv nm (SynField(Attributes attribs, isStatic, idOpt, ty, isMutable, xmldoc, vis, m)) =
        let id = (match idOpt with None -> mkSynId m nm | Some id -> id)
        let xmlDoc = xmldoc.ToXmlDoc(true, Some [])
        TcFieldDecl cenv env parent false tpenv (isStatic, attribs, id, idOpt.IsNone, ty, isMutable, xmlDoc, vis, m)

    let TcNamedFieldDecl cenv env parent isIncrClass tpenv (SynField(Attributes attribs, isStatic, id, ty, isMutable, xmldoc, vis, m)) =
        match id with 
        | None -> error (Error(FSComp.SR.tcFieldRequiresName(), m))
        | Some id ->
            let xmlDoc = xmldoc.ToXmlDoc(true, Some [])
            TcFieldDecl cenv env parent isIncrClass tpenv (isStatic, attribs, id, false, ty, isMutable, xmlDoc, vis, m) 

    let TcNamedFieldDecls cenv env parent isIncrClass tpenv fields =
        fields |> List.map (TcNamedFieldDecl cenv env parent isIncrClass tpenv) 

    //-------------------------------------------------------------------------
    // Bind other elements of type definitions (constructors etc.)
    //------------------------------------------------------------------------- 

    let CheckUnionCaseName (cenv: cenv) (id: Ident) =
        let g = cenv.g
        let name = id.idText
        if name = "Tags" then
            errorR(Error(FSComp.SR.tcUnionCaseNameConflictsWithGeneratedType(name, "Tags"), id.idRange))

        CheckNamespaceModuleOrTypeName g id

        if not (String.isLeadingIdentifierCharacterUpperCase name) && name <> opNameCons && name <> opNameNil then
            errorR(NotUpperCaseConstructor(id.idRange))

    let ValidateFieldNames (synFields: SynField list, tastFields: RecdField list) = 
        let seen = Dictionary()
        (synFields, tastFields) ||> List.iter2 (fun sf f ->
            match seen.TryGetValue f.LogicalName with
            | true, synField ->
                match sf, synField with
                | SynField(_, _, Some id, _, _, _, _, _), SynField(_, _, Some _, _, _, _, _, _) ->
                    error(Error(FSComp.SR.tcFieldNameIsUsedModeThanOnce(id.idText), id.idRange))
                | SynField(_, _, Some id, _, _, _, _, _), SynField(_, _, None, _, _, _, _, _)
                | SynField(_, _, None, _, _, _, _, _), SynField(_, _, Some id, _, _, _, _, _) ->
                    error(Error(FSComp.SR.tcFieldNameConflictsWithGeneratedNameForAnonymousField(id.idText), id.idRange))
                | _ -> assert false
            | _ ->
                seen.Add(f.LogicalName, sf))
                
    let TcUnionCaseDecl (cenv: cenv) env parent thisTy thisTyInst tpenv (SynUnionCase(Attributes synAttrs, SynIdent(id, _), args, xmldoc, vis, m, _)) =
        let g = cenv.g
        let attrs = TcAttributes cenv env AttributeTargets.UnionCaseDecl synAttrs // the attributes of a union case decl get attached to the generated "static factory" method
        let vis, _ = ComputeAccessAndCompPath env None m vis None parent
        let vis = CombineReprAccess parent vis

        CheckUnionCaseName cenv id

        let rfields, recordTy = 
            match args with
            | SynUnionCaseKind.Fields flds -> 
                let nFields = flds.Length
                let rfields = flds |> List.mapi (fun i (SynField (idOpt = idOpt) as fld) ->
                    match idOpt, parent with
                    | Some fieldId, Parent tcref ->
                        let item = Item.UnionCaseField (UnionCaseInfo (thisTyInst, UnionCaseRef (tcref, id.idText)), i)
                        CallNameResolutionSink cenv.tcSink (fieldId.idRange, env.NameEnv, item, emptyTyparInst, ItemOccurence.Binding, env.AccessRights)
                        TcNamedFieldDecl cenv env parent false tpenv fld
                    | _ ->
                        TcAnonFieldDecl cenv env parent tpenv (mkUnionCaseFieldName nFields i) fld)
                ValidateFieldNames(flds, rfields)
                
                rfields, thisTy

            | SynUnionCaseKind.FullType (ty, arity) -> 
                let tyR, _ = TcTypeAndRecover cenv NoNewTypars CheckCxs ItemOccurence.UseInType env tpenv ty
                let curriedArgTys, recordTy = GetTopTauTypeInFSharpForm g (arity |> TranslateSynValInfo m (TcAttributes cenv env) |> TranslatePartialValReprInfo []).ArgInfos tyR m

                if curriedArgTys.Length > 1 then 
                    errorR(Error(FSComp.SR.tcIllegalFormForExplicitTypeDeclaration(), m))   

                let argTys = curriedArgTys |> List.concat
                let nFields = argTys.Length

                let rfields = 
                    argTys |> List.mapi (fun i (argTy, argInfo) ->
                        let id = (match argInfo.Name with Some id -> id | None -> mkSynId m (mkUnionCaseFieldName nFields i))
                        MakeRecdFieldSpec cenv env parent (false, None, argTy, [], [], id, argInfo.Name.IsNone, false, false, XmlDoc.Empty, None, m))

                if not (typeEquiv g recordTy thisTy) then 
                    error(Error(FSComp.SR.tcReturnTypesForUnionMustBeSameAsType(), m))
                rfields, recordTy

        let names = rfields |> List.map (fun f -> f.DisplayNameCore)
        let xmlDoc = xmldoc.ToXmlDoc(true, Some names)
        Construct.NewUnionCase id rfields recordTy attrs xmlDoc vis

    let TcUnionCaseDecls cenv env parent (thisTy: TType) thisTyInst tpenv unionCases =
        let unionCasesR = unionCases |> List.map (TcUnionCaseDecl cenv env parent thisTy thisTyInst tpenv) 
        unionCasesR |> CheckDuplicates (fun uc -> uc.Id) "union case" 

    let TcEnumDecl cenv env parent thisTy fieldTy (SynEnumCase(attributes=Attributes synAttrs; ident= SynIdent(id,_); value=v; xmlDoc=xmldoc; range=m)) =
        let attrs = TcAttributes cenv env AttributeTargets.Field synAttrs
        match v with 
        | SynConst.Bytes _
        | SynConst.UInt16s _
        | SynConst.UserNum _ -> error(Error(FSComp.SR.tcInvalidEnumerationLiteral(), m))
        | _ -> 
            let v = TcConst cenv fieldTy m env v
            let vis, _ = ComputeAccessAndCompPath env None m None None parent
            let vis = CombineReprAccess parent vis
            if id.idText = "value__" then errorR(Error(FSComp.SR.tcNotValidEnumCaseName(), id.idRange))
            let xmlDoc = xmldoc.ToXmlDoc(true, Some [])
            Construct.NewRecdField true (Some v) id false thisTy false false [] attrs xmlDoc vis false
      
    let TcEnumDecls (cenv: cenv) env parent thisTy enumCases =
        let g = cenv.g
        let fieldTy = NewInferenceType g
        let enumCases' = enumCases |> List.map (TcEnumDecl cenv env parent thisTy fieldTy) |> CheckDuplicates (fun f -> f.Id) "enum element"
        fieldTy, enumCases'

//-------------------------------------------------------------------------
// Bind elements of classes
//------------------------------------------------------------------------- 

let PublishInterface (cenv: cenv) denv (tcref: TyconRef) m isCompGen interfaceTy = 
    let g = cenv.g

    if not (isInterfaceTy g interfaceTy) then
        errorR(Error(FSComp.SR.tcTypeIsNotInterfaceType1(NicePrint.minimalStringOfType denv interfaceTy), m))

    if tcref.HasInterface g interfaceTy then 
        errorR(Error(FSComp.SR.tcDuplicateSpecOfInterface(), m))

    let tcaug = tcref.TypeContents

    tcaug.tcaug_interfaces <- (interfaceTy, isCompGen, m) :: tcaug.tcaug_interfaces

let TcAndPublishMemberSpec cenv env containerInfo declKind tpenv memb = 
    match memb with 
    | SynMemberSig.ValField(_, m) -> error(Error(FSComp.SR.tcFieldValIllegalHere(), m))
    | SynMemberSig.Inherit(_, m) -> error(Error(FSComp.SR.tcInheritIllegalHere(), m))
    | SynMemberSig.NestedType(_, m) -> error(Error(FSComp.SR.tcTypesCannotContainNestedTypes(), m))
    | SynMemberSig.Member(synValSig, memberFlags, _) -> 
        TcAndPublishValSpec (cenv, env, containerInfo, declKind, Some memberFlags, tpenv, synValSig)
    | SynMemberSig.Interface _ -> 
        // These are done in TcMutRecDefns_Phase1
        [], tpenv

let TcTyconMemberSpecs cenv env containerInfo declKind tpenv augSpfn =
    let members, tpenv = List.mapFold (TcAndPublishMemberSpec cenv env containerInfo declKind) tpenv augSpfn
    List.concat members, tpenv

//-------------------------------------------------------------------------
// Bind 'open' declarations
//------------------------------------------------------------------------- 

let TcOpenLidAndPermitAutoResolve tcSink (env: TcEnv) amap (longId : Ident list) =
    let ad = env.AccessRights
    match longId with
    | [] -> []
    | id :: rest ->
        let m = longId |> List.map (fun id -> id.idRange) |> List.reduce unionRanges
        match ResolveLongIdentAsModuleOrNamespace tcSink ResultCollectionSettings.AllResults amap m true OpenQualified env.NameEnv ad id rest true with 
        | Result res -> res
        | Exception err ->
            errorR(err); []

let TcOpenModuleOrNamespaceDecl tcSink g amap scopem env (longId, m) = 
    match TcOpenLidAndPermitAutoResolve tcSink env amap longId with
    | [] -> env, []
    | modrefs ->

    // validate opened namespace names
    for id in longId do
        if id.idText <> MangledGlobalName then
            CheckNamespaceModuleOrTypeName g id

    let IsPartiallyQualifiedNamespace (modref: ModuleOrNamespaceRef) = 
        let (CompPath(_, p)) = modref.CompilationPath 
        // Bug FSharp 1.0 3274: FSI paths don't count when determining this warning
        let p = 
            match p with 
            | [] -> []
            | (h, _) :: t -> if h.StartsWithOrdinal FsiDynamicModulePrefix then t else p

        // See https://fslang.uservoice.com/forums/245727-f-language/suggestions/6107641-make-microsoft-prefix-optional-when-using-core-f
        let isFSharpCoreSpecialCase =
            match ccuOfTyconRef modref with 
            | None -> false
            | Some ccu -> 
                ccuEq ccu g.fslibCcu &&
                // Check if we're using a reference one string shorter than what we expect.
                //
                // "p" is the fully qualified path _containing_ the thing we're opening, e.g. "Microsoft.FSharp" when opening "Microsoft.FSharp.Data"
                // "longId" is the text being used, e.g. "FSharp.Data"
                //    Length of thing being opened = p.Length + 1
                //    Length of reference = longId.Length
                // So the reference is a "shortened" reference if (p.Length + 1) - 1 = longId.Length
                (p.Length + 1) - 1 = longId.Length && 
                fst p[0] = "Microsoft" 

        modref.IsNamespace && 
        p.Length >= longId.Length &&
        not isFSharpCoreSpecialCase
        // Allow "open Foo" for "Microsoft.Foo" from FSharp.Core

    modrefs |> List.iter (fun (_, modref, _) ->
       if modref.IsModule && HasFSharpAttribute g g.attrib_RequireQualifiedAccessAttribute modref.Attribs then 
           errorR(Error(FSComp.SR.tcModuleRequiresQualifiedAccess(fullDisplayTextOfModRef modref), m)))

    // Bug FSharp 1.0 3133: 'open Lexing'. Skip this warning if we successfully resolved to at least a module name
    if not (modrefs |> List.exists (fun (_, modref, _) -> modref.IsModule && not (HasFSharpAttribute g g.attrib_RequireQualifiedAccessAttribute modref.Attribs))) then
        modrefs |> List.iter (fun (_, modref, _) ->
            if IsPartiallyQualifiedNamespace modref then 
                 errorR(Error(FSComp.SR.tcOpenUsedWithPartiallyQualifiedPath(fullDisplayTextOfModRef modref), m)))
        
    let modrefs = List.map p23 modrefs
    modrefs |> List.iter (fun modref -> CheckEntityAttributes g modref m |> CommitOperationResult)        

    let openDecl = OpenDeclaration.Create (SynOpenDeclTarget.ModuleOrNamespace (longId, m), modrefs, [], scopem, false)
    let env = OpenModuleOrNamespaceRefs tcSink g amap scopem false env modrefs openDecl
    env, [openDecl]

let TcOpenTypeDecl (cenv: cenv) mOpenDecl scopem env (synType: SynType, m) =
    let g = cenv.g

    checkLanguageFeatureError g.langVersion LanguageFeature.OpenTypeDeclaration mOpenDecl

    let ty, _tpenv = TcType cenv NoNewTypars CheckCxs ItemOccurence.Open env emptyUnscopedTyparEnv synType

    if not (isAppTy g ty) then
        error(Error(FSComp.SR.tcNamedTypeRequired("open type"), m))

    if isByrefTy g ty then
        error(Error(FSComp.SR.tcIllegalByrefsInOpenTypeDeclaration(), m))

    let openDecl = OpenDeclaration.Create (SynOpenDeclTarget.Type (synType, m), [], [ty], scopem, false)
    let env = OpenTypeContent cenv.tcSink g cenv.amap scopem env ty openDecl
    env, [openDecl]

let TcOpenDecl (cenv: cenv) mOpenDecl scopem env target = 
    let g = cenv.g
    match target with
    | SynOpenDeclTarget.ModuleOrNamespace (longId, m) ->
        TcOpenModuleOrNamespaceDecl cenv.tcSink g cenv.amap scopem env (longId, m)

    | SynOpenDeclTarget.Type (synType, m) ->
        TcOpenTypeDecl cenv mOpenDecl scopem env (synType, m)
        
let MakeSafeInitField (g: TcGlobals) env m isStatic = 
    let id =
        // Ensure that we have an g.CompilerGlobalState
        assert(g.CompilerGlobalState |> Option.isSome)
        ident(g.CompilerGlobalState.Value.NiceNameGenerator.FreshCompilerGeneratedName("init", m), m)
    let taccess = TAccess [env.eAccessPath]
    Construct.NewRecdField isStatic None id false g.int_ty true true [] [] XmlDoc.Empty taccess true

// Checking of mutually recursive types, members and 'let' bindings in classes
//
// Technique: multiple passes.
//   Phase1: create and establish type definitions and core representation information
//   Phase2A: create Vals for recursive items given names and args
//   Phase2B-D: type check AST to TAST collecting (sufficient) type constraints, 
//              generalize definitions, fix up recursive instances, build ctor binding
module MutRecBindingChecking = 

    /// Represents one element in a type definition, after the first phase    
    type TyconBindingPhase2A =
      /// An entry corresponding to the definition of the implicit constructor for a class
      | Phase2AIncrClassCtor of IncrClassCtorLhs
      /// An 'inherit' declaration in an incremental class
      ///
      /// Phase2AInherit (ty, arg, baseValOpt, m)
      | Phase2AInherit of SynType * SynExpr * Val option * range
      /// A set of value or function definitions in an incremental class
      ///
      /// Phase2AIncrClassBindings (tcref, letBinds, isStatic, isRec, m)
      | Phase2AIncrClassBindings of TyconRef * SynBinding list * bool * bool * range
      /// A 'member' definition in a class
      | Phase2AMember of PreCheckingRecursiveBinding
#if OPEN_IN_TYPE_DECLARATIONS
      /// A dummy declaration, should we ever support 'open' in type definitions
      | Phase2AOpen of SynOpenDeclTarget * range
#endif
      /// Indicates the super init has just been called, 'this' may now be published
      | Phase2AIncrClassCtorJustAfterSuperInit 
      /// Indicates the last 'field' has been initialized, only 'do' comes after 
      | Phase2AIncrClassCtorJustAfterLastLet

    /// The collected syntactic input definitions for a single type or type-extension definition
    type TyconBindingsPhase2A = 
      | TyconBindingsPhase2A of Tycon option * DeclKind * Val list * TyconRef * Typar list * TType * TyconBindingPhase2A list

    /// The collected syntactic input definitions for a recursive group of type or type-extension definitions
    type MutRecDefnsPhase2AData = MutRecShape<TyconBindingsPhase2A, PreCheckingRecursiveBinding list, MutRecDefnsPhase2DataForModule * TcEnv> list

    /// Represents one element in a type definition, after the second phase
    type TyconBindingPhase2B =
      | Phase2BIncrClassCtor of IncrClassCtorLhs * Binding option 
      | Phase2BInherit of Expr * Val option
      /// A set of value of function definitions in a class definition with an implicit constructor.
      | Phase2BIncrClassBindings of IncrClassBindingGroup list
      | Phase2BMember of int
      /// An intermediate definition that represent the point in an implicit class definition where
      /// the super type has been initialized.
      | Phase2BIncrClassCtorJustAfterSuperInit
      /// An intermediate definition that represent the point in an implicit class definition where
      /// the last 'field' has been initialized, i.e. only 'do' and 'member' definitions come after 
      /// this point.
      | Phase2BIncrClassCtorJustAfterLastLet

    type TyconBindingsPhase2B = TyconBindingsPhase2B of Tycon option * TyconRef * TyconBindingPhase2B list

    type MutRecDefnsPhase2BData = MutRecShape<TyconBindingsPhase2B, int list, MutRecDefnsPhase2DataForModule * TcEnv> list

    /// Represents one element in a type definition, after the third phase
    type TyconBindingPhase2C =
      | Phase2CIncrClassCtor of IncrClassCtorLhs * Binding option 
      | Phase2CInherit of Expr * Val option
      | Phase2CIncrClassBindings of IncrClassBindingGroup list
      | Phase2CMember of PreInitializationGraphEliminationBinding
      // Indicates the last 'field' has been initialized, only 'do' comes after 
      | Phase2CIncrClassCtorJustAfterSuperInit     
      | Phase2CIncrClassCtorJustAfterLastLet     

    type TyconBindingsPhase2C = TyconBindingsPhase2C of Tycon option * TyconRef * TyconBindingPhase2C list

    type MutRecDefnsPhase2CData = MutRecShape<TyconBindingsPhase2C, PreInitializationGraphEliminationBinding list, MutRecDefnsPhase2DataForModule * TcEnv> list

    // Phase2A: create member prelimRecValues for "recursive" items, i.e. ctor val and member vals 
    // Phase2A: also processes their arg patterns - collecting type assertions 
    let TcMutRecBindings_Phase2A_CreateRecursiveValuesAndCheckArgumentPatterns (cenv: cenv) tpenv (envMutRec, mutRecDefns: MutRecDefnsPhase2Info) =
        let g = cenv.g

        // The basic iteration over the declarations in a single type definition
        // State:
        //    tpenv: floating type parameter environment
        //    recBindIdx: index of the recursive binding
        //    prelimRecValuesRev: accumulation of prelim value entries
        //    uncheckedBindsRev: accumulation of unchecked bindings
        let (defnsAs: MutRecDefnsPhase2AData), (tpenv, _, uncheckedBindsRev) =
            let initialOuterState = (tpenv, 0, ([]: PreCheckingRecursiveBinding list))
            (initialOuterState, envMutRec, mutRecDefns) |||> MutRecShapes.mapFoldWithEnv (fun outerState envForDecls defn -> 
              let tpenv, recBindIdx, uncheckedBindsRev = outerState
              match defn with 
              | MutRecShape.Module _ -> failwith "unreachable"
              | MutRecShape.Open x -> MutRecShape.Open x, outerState 
              | MutRecShape.ModuleAbbrev x -> MutRecShape.ModuleAbbrev x, outerState 
              | MutRecShape.Lets recBinds -> 
                let normRecDefns = 
                   [ for RecDefnBindingInfo(a, b, c, bind) in recBinds do 
                       yield NormalizedRecBindingDefn(a, b, c, BindingNormalization.NormalizeBinding ValOrMemberBinding cenv envForDecls bind) ]
                let bindsAndValues, (tpenv, recBindIdx) = ((tpenv, recBindIdx), normRecDefns) ||> List.mapFold (AnalyzeAndMakeAndPublishRecursiveValue ErrorOnOverrides false cenv envForDecls) 
                let binds = bindsAndValues |> List.collect fst

                let defnAs = MutRecShape.Lets binds
                defnAs, (tpenv, recBindIdx, List.rev binds @ uncheckedBindsRev)

              | MutRecShape.Tycon (MutRecDefnsPhase2InfoForTycon(tyconOpt, tcref, declaredTyconTypars, declKind, binds, _)) ->

                // Class members can access protected members of the implemented type 
                // Class members can access private members in the ty
                let isExtrinsic = (declKind = ExtrinsicExtensionBinding)
                let initialEnvForTycon = MakeInnerEnvForTyconRef envForDecls tcref isExtrinsic 

                // Re-add the type constructor to make it take precedence for record label field resolutions
                // This does not apply to extension members: in those cases the relationship between the record labels
                // and the type is too extruded
                let envForTycon = 
                    if isExtrinsic then 
                        initialEnvForTycon
                    else
                        AddLocalTyconRefs true g cenv.amap tcref.Range [tcref] initialEnvForTycon

                // Make fresh version of the class type for type checking the members and lets *
                let _, copyOfTyconTypars, _, objTy, thisTy = FreshenObjectArgType cenv tcref.Range TyparRigidity.WillBeRigid tcref isExtrinsic declaredTyconTypars


                // The basic iteration over the declarations in a single type definition
                let initialInnerState = (None, envForTycon, tpenv, recBindIdx, uncheckedBindsRev)
                let defnAs, (_, _envForTycon, tpenv, recBindIdx, uncheckedBindsRev) = 

                    (initialInnerState, binds) ||> List.collectFold (fun innerState defn ->

                        let (TyconBindingDefn(containerInfo, newslotsOK, declKind, classMemberDef, m)) = defn
                        let incrClassCtorLhsOpt, envForTycon, tpenv, recBindIdx, uncheckedBindsRev = innerState

                        if tcref.IsTypeAbbrev then
                            // ideally we'd have the 'm' of the type declaration stored here, to avoid needing to trim to line to approx
                            error(Error(FSComp.SR.tcTypeAbbreviationsMayNotHaveMembers(), (trimRangeToLine m)))

                        if tcref.IsEnumTycon && (declKind <> ExtrinsicExtensionBinding) then 
                            // ideally we'd have the 'm' of the type declaration stored here, to avoid needing to trim to line to approx
                            error(Error(FSComp.SR.tcEnumerationsMayNotHaveMembers(), (trimRangeToLine m))) 

                        match classMemberDef, containerInfo with
                        | SynMemberDefn.ImplicitCtor (vis, Attributes attrs, SynSimplePats.SimplePats(spats, _), thisIdOpt, xmlDoc, m), ContainerInfo(_, Some(MemberOrValContainerInfo(tcref, _, baseValOpt, safeInitInfo, _))) ->
                            if tcref.TypeOrMeasureKind = TyparKind.Measure then
                                error(Error(FSComp.SR.tcMeasureDeclarationsRequireStaticMembers(), m))

                            // Phase2A: make incrClassCtorLhs - ctorv, thisVal etc, type depends on argty(s) 
                            let incrClassCtorLhs = TcImplicitCtorLhs_Phase2A(cenv, envForTycon, tpenv, tcref, vis, attrs, spats, thisIdOpt, baseValOpt, safeInitInfo, m, copyOfTyconTypars, objTy, thisTy, xmlDoc)

                            // Phase2A: Add copyOfTyconTypars from incrClassCtorLhs - or from tcref 
                            let envForTycon = AddDeclaredTypars CheckForDuplicateTypars incrClassCtorLhs.InstanceCtorDeclaredTypars envForTycon
                            let innerState = (Some incrClassCtorLhs, envForTycon, tpenv, recBindIdx, uncheckedBindsRev)

                            [Phase2AIncrClassCtor incrClassCtorLhs], innerState
                              
                        | SynMemberDefn.ImplicitInherit (ty, arg, _baseIdOpt, m), _ ->
                            if tcref.TypeOrMeasureKind = TyparKind.Measure then
                                error(Error(FSComp.SR.tcMeasureDeclarationsRequireStaticMembers(), m))

                            // Phase2A: inherit ty(arg) as base - pass through 
                            // Phase2A: pick up baseValOpt! 
                            let baseValOpt = incrClassCtorLhsOpt |> Option.bind (fun x -> x.InstanceCtorBaseValOpt)
                            let innerState = (incrClassCtorLhsOpt, envForTycon, tpenv, recBindIdx, uncheckedBindsRev)
                            [Phase2AInherit (ty, arg, baseValOpt, m); Phase2AIncrClassCtorJustAfterSuperInit], innerState

                        | SynMemberDefn.LetBindings (letBinds, isStatic, isRec, m), _ ->
                            match tcref.TypeOrMeasureKind, isStatic with 
                            | TyparKind.Measure, false -> error(Error(FSComp.SR.tcMeasureDeclarationsRequireStaticMembers(), m)) 
                            | _ -> ()

                            if not isStatic && tcref.IsStructOrEnumTycon then 
                                let allDo = letBinds |> List.forall (function SynBinding(kind=SynBindingKind.Do) -> true | _ -> false)
                                // Code for potential future design change to allow functions-compiled-as-members in structs
                                if allDo then 
                                    errorR(Deprecated(FSComp.SR.tcStructsMayNotContainDoBindings(), (trimRangeToLine m)))
                                else
                                // Code for potential future design change to allow functions-compiled-as-members in structs
                                    errorR(Error(FSComp.SR.tcStructsMayNotContainLetBindings(), (trimRangeToLine m)))

                            if isStatic && Option.isNone incrClassCtorLhsOpt then 
                                errorR(Error(FSComp.SR.tcStaticLetBindingsRequireClassesWithImplicitConstructors(), m))
                              
                            // Phase2A: let-bindings - pass through 
                            let innerState = (incrClassCtorLhsOpt, envForTycon, tpenv, recBindIdx, uncheckedBindsRev)     
                            [Phase2AIncrClassBindings (tcref, letBinds, isStatic, isRec, m)], innerState
                              
                        | SynMemberDefn.Member (bind, m), _ ->
                            // Phase2A: member binding - create prelim valspec (for recursive reference) and RecursiveBindingInfo 
                            let NormalizedBinding(_, _, _, _, _, _, _, valSynData, _, _, _, _) as bind = BindingNormalization.NormalizeBinding ValOrMemberBinding cenv envForTycon bind
                            let (SynValData(memberFlagsOpt, _, _)) = valSynData 

                            match tcref.TypeOrMeasureKind with
                            | TyparKind.Type -> ()
                            | TyparKind.Measure ->
                                match memberFlagsOpt with 
                                | None -> () 
                                | Some memberFlags -> 
                                    if memberFlags.IsInstance then error(Error(FSComp.SR.tcMeasureDeclarationsRequireStaticMembers(), m))
                                    match memberFlags.MemberKind with 
                                    | SynMemberKind.Constructor -> error(Error(FSComp.SR.tcMeasureDeclarationsRequireStaticMembersNotConstructors(), m))
                                    | _ -> ()

                            let envForMember = 
                                match incrClassCtorLhsOpt with
                                | None -> AddDeclaredTypars CheckForDuplicateTypars copyOfTyconTypars envForTycon
                                | Some _ -> envForTycon

                            let rbind = NormalizedRecBindingDefn(containerInfo, newslotsOK, declKind, bind)
                            let overridesOK = DeclKind.CanOverrideOrImplement declKind
                            let (binds, _values), (tpenv, recBindIdx) = AnalyzeAndMakeAndPublishRecursiveValue overridesOK false cenv envForMember (tpenv, recBindIdx) rbind
                            let cbinds = [ for rbind in binds -> Phase2AMember rbind ]

                            let innerState = (incrClassCtorLhsOpt, envForTycon, tpenv, recBindIdx, List.rev binds @ uncheckedBindsRev)
                            cbinds, innerState
                        
#if OPEN_IN_TYPE_DECLARATIONS
                        | SynMemberDefn.Open (target, m), _ ->
                            let innerState = (incrClassCtorLhsOpt, env, tpenv, recBindIdx, prelimRecValuesRev, uncheckedBindsRev)
                            [ Phase2AOpen (target, m) ], innerState
#endif
                        
                        | definition -> 
                            error(InternalError(sprintf "Unexpected definition %A" definition, m)))

                // If no constructor call, insert Phase2AIncrClassCtorJustAfterSuperInit at start
                let defnAs = 
                    match defnAs with 
                    | Phase2AIncrClassCtor _ as b1 :: rest -> 
                        let rest = 
                            if rest |> List.exists (function Phase2AIncrClassCtorJustAfterSuperInit -> true | _ -> false) then 
                                rest
                            else
                                Phase2AIncrClassCtorJustAfterSuperInit :: rest
                        // Insert Phase2AIncrClassCtorJustAfterLastLet at the point where local construction is known to have been finished 
                        let rest = 
                            let isAfter b = 
                                match b with 
#if OPEN_IN_TYPE_DECLARATIONS
                                | Phase2AOpen _ 
#endif
                                | Phase2AIncrClassCtor _ | Phase2AInherit _ | Phase2AIncrClassCtorJustAfterSuperInit -> false
                                | Phase2AIncrClassBindings (_, binds, _, _, _) -> binds |> List.exists (function SynBinding (kind=SynBindingKind.Do) -> true | _ -> false)
                                | Phase2AIncrClassCtorJustAfterLastLet
                                | Phase2AMember _ -> true
                            let restRev = List.rev rest
                            let afterRev = restRev |> List.takeWhile isAfter
                            let beforeRev = restRev |> List.skipWhile isAfter
                            
                            [ yield! List.rev beforeRev
                              yield Phase2AIncrClassCtorJustAfterLastLet
                              yield! List.rev afterRev ]
                        b1 :: rest

                    // Cover the case where this is not a type with an implicit constructor.
                    | rest -> rest

                let prelimRecValues = [ for x in defnAs do match x with Phase2AMember bind -> yield bind.RecBindingInfo.Val | _ -> () ]
                let defnAs = MutRecShape.Tycon(TyconBindingsPhase2A(tyconOpt, declKind, prelimRecValues, tcref, copyOfTyconTypars, thisTy, defnAs))
                defnAs, (tpenv, recBindIdx, uncheckedBindsRev))

        let uncheckedRecBinds = List.rev uncheckedBindsRev

        (defnsAs, uncheckedRecBinds, tpenv)

    /// Phase2B: check each of the bindings, convert from ast to tast and collects type assertions.
    /// Also generalize incrementally.
    let TcMutRecBindings_Phase2B_TypeCheckAndIncrementalGeneralization (cenv: cenv) tpenv envInitial (envMutRec, defnsAs: MutRecDefnsPhase2AData, uncheckedRecBinds: PreCheckingRecursiveBinding list, scopem) : MutRecDefnsPhase2BData * _ * _ =
        let g = cenv.g

        let (defnsBs: MutRecDefnsPhase2BData), (tpenv, generalizedRecBinds, preGeneralizationRecBinds, _, _) = 

            let uncheckedRecBindsTable = uncheckedRecBinds |> List.map (fun rbind -> rbind.RecBindingInfo.Val.Stamp, rbind) |> Map.ofList 

            // Loop through the types being defined...
            //
            // The envNonRec is the environment used to limit generalization to prevent leakage of type
            // variables into the types of 'let' bindings. It gets accumulated across type definitions, e.g.
            // consider
            //
            //   type A<'T>() =  
            //       let someFuncValue: 'A = A<'T>.Meth2()
            //       static member Meth2() = A<'T>.Meth2() 
            //   and B<'T>() =
            //       static member Meth1() = A<'T>.Meth2()
            //
            // Here 'A can't be generalized, even at 'Meth1'.
            //
            // The envForTycon is the environment used for name resolution within the let and member bindings
            // of the type definition. This becomes 'envStatic' and 'envInstance' for the two 
             
            let initialOuterState = (tpenv, ([]: PostGeneralizationRecursiveBinding list), ([]: PreGeneralizationRecursiveBinding list), uncheckedRecBindsTable, envInitial)

            (initialOuterState, envMutRec, defnsAs) |||> MutRecShapes.mapFoldWithEnv (fun outerState envForDecls defnsA -> 

              let tpenv, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable, envNonRec = outerState

              match defnsA with 
              | MutRecShape.Module _ -> failwith "unreachable"
              | MutRecShape.Open x -> MutRecShape.Open x, outerState 
              | MutRecShape.ModuleAbbrev x -> MutRecShape.ModuleAbbrev x, outerState 
              | MutRecShape.Lets binds ->
                
                let defnBs, (tpenv, _, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable) = 

                    let initialInnerState = (tpenv, envForDecls, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable)
                    (initialInnerState, binds) ||> List.mapFold (fun innerState rbind -> 

                        let tpenv, envStatic, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable = innerState

                        let envNonRec, generalizedRecBinds, preGeneralizationRecBinds, _, uncheckedRecBindsTable = 
                            TcLetrecBinding (cenv, envStatic, scopem, [], None) (envNonRec, generalizedRecBinds, preGeneralizationRecBinds, tpenv, uncheckedRecBindsTable) rbind
                             
                        let innerState = (tpenv, envStatic, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable)
                        rbind.RecBindingInfo.Index, innerState)
                
                let outerState = (tpenv, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable, envNonRec)
                MutRecShape.Lets defnBs, outerState

              | MutRecShape.Tycon (TyconBindingsPhase2A(tyconOpt, declKind, _, tcref, copyOfTyconTypars, thisTy, defnAs)) ->
                
                let isExtrinsic = (declKind = ExtrinsicExtensionBinding)
                let envForTycon = MakeInnerEnvForTyconRef envForDecls tcref isExtrinsic 
                let envForTycon = if isExtrinsic then envForTycon else AddLocalTyconRefs true g cenv.amap tcref.Range [tcref] envForTycon
                // Set up the environment so use-before-definition warnings are given, at least 
                // until we reach a Phase2AIncrClassCtorJustAfterSuperInit. 
                let envForTycon = { envForTycon with eCtorInfo = Some (InitialImplicitCtorInfo()) }

                let reqdThisValTyOpt = Some thisTy
                
                // Loop through the definition elements in a type...
                // State: 
                //      envInstance: the environment in scope in instance members
                //      envStatic: the environment in scope in static members
                //      envNonRec: the environment relevant to generalization
                //      generalizedRecBinds: part of the incremental generalization state
                //      preGeneralizationRecBinds: part of the incremental generalization state
                //      uncheckedRecBindsTable: part of the incremental generalization state
                let defnBs, (tpenv, _, _, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable) = 

                    let initialInnerState = (tpenv, envForTycon, envForTycon, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable)
                    (initialInnerState, defnAs) ||> List.mapFold (fun innerState defnA -> 

                        let tpenv, envInstance, envStatic, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable = innerState

                        match defnA with
                        // Phase2B for the definition of an implicit constructor. Enrich the instance environments
                        // with the implicit ctor args.
                        | Phase2AIncrClassCtor incrClassCtorLhs ->

                            let envInstance = AddDeclaredTypars CheckForDuplicateTypars incrClassCtorLhs.InstanceCtorDeclaredTypars envInstance
                            let envStatic = AddDeclaredTypars CheckForDuplicateTypars incrClassCtorLhs.InstanceCtorDeclaredTypars envStatic
                            let envInstance = match incrClassCtorLhs.InstanceCtorSafeThisValOpt with Some v -> AddLocalVal g cenv.tcSink scopem v envInstance | None -> envInstance
                            let envInstance = List.foldBack (AddLocalValPrimitive g) incrClassCtorLhs.InstanceCtorArgs envInstance 
                            let envNonRec = match incrClassCtorLhs.InstanceCtorSafeThisValOpt with Some v -> AddLocalVal g cenv.tcSink scopem v envNonRec | None -> envNonRec
                            let envNonRec = List.foldBack (AddLocalValPrimitive g) incrClassCtorLhs.InstanceCtorArgs envNonRec
                            let safeThisValBindOpt = TcLetrecComputeCtorSafeThisValBind cenv incrClassCtorLhs.InstanceCtorSafeThisValOpt

                            let innerState = (tpenv, envInstance, envStatic, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable)
                            Phase2BIncrClassCtor (incrClassCtorLhs, safeThisValBindOpt), innerState
                            
                        // Phase2B: typecheck the argument to an 'inherits' call and build the new object expr for the inherit-call 
                        | Phase2AInherit (synBaseTy, arg, baseValOpt, m) ->
                            let baseTy, tpenv = TcType cenv NoNewTypars CheckCxs ItemOccurence.Use envInstance tpenv synBaseTy
                            let baseTy = baseTy |> convertToTypeWithMetadataIfPossible g
                            let inheritsExpr, tpenv =
                                try 
                                   TcNewExpr cenv envInstance tpenv baseTy (Some synBaseTy.Range) true arg m
                                with e ->
                                    errorRecovery e m
                                    mkUnit g m, tpenv
                            let envInstance = match baseValOpt with Some baseVal -> AddLocalVal g cenv.tcSink scopem baseVal envInstance | None -> envInstance
                            let envNonRec = match baseValOpt with Some baseVal -> AddLocalVal g cenv.tcSink scopem baseVal envNonRec | None -> envNonRec
                            let innerState = (tpenv, envInstance, envStatic, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable)
                            Phase2BInherit (inheritsExpr, baseValOpt), innerState
                            
                        // Phase2B: let and let rec value and function definitions
                        | Phase2AIncrClassBindings (tcref, binds, isStatic, isRec, mBinds) ->
                            let envForBinding = if isStatic then envStatic else envInstance
                            let binds, bindRs, env, tpenv = 
                                if isRec then
                                
                                    // Type check local recursive binding 
                                    let binds = binds |> List.map (fun bind -> RecDefnBindingInfo(ExprContainerInfo, NoNewSlots, ClassLetBinding isStatic, bind))
                                    let binds, env, tpenv = TcLetrecBindings ErrorOnOverrides cenv envForBinding tpenv (binds, scopem, scopem)
                                    let bindRs = [IncrClassBindingGroup(binds, isStatic, true)]
                                    binds, bindRs, env, tpenv 
                                else

                                    // Type check local binding 
                                    let binds, env, tpenv = TcLetBindings cenv envForBinding ExprContainerInfo (ClassLetBinding isStatic) tpenv (binds, mBinds, scopem)
                                    let binds, bindRs = 
                                        binds 
                                        |> List.map (function
                                            | TMDefLet(bind, _) -> [bind], IncrClassBindingGroup([bind], isStatic, false)
                                            | TMDefDo(e, _) -> [], IncrClassDo(e, isStatic, mBinds)
                                            | _ -> error(InternalError("unexpected definition kind", tcref.Range)))
                                        |> List.unzip
                                    List.concat binds, bindRs, env, tpenv

                            let envNonRec = (envNonRec, binds) ||> List.fold (fun acc bind -> AddLocalValPrimitive g bind.Var acc)

                            // Check to see that local bindings and members don't have the same name and check some other adhoc conditions
                            for bind in binds do
                                if not isStatic && HasFSharpAttributeOpt g g.attrib_DllImportAttribute bind.Var.Attribs then 
                                    errorR(Error(FSComp.SR.tcDllImportNotAllowed(), bind.Var.Range))
                                    
                                let nm = bind.Var.DisplayName
                                let ty = generalizedTyconRef g tcref
                                let ad = envNonRec.AccessRights
                                match TryFindIntrinsicMethInfo cenv.infoReader bind.Var.Range ad nm ty, 
                                      TryFindIntrinsicPropInfo cenv.infoReader bind.Var.Range ad nm ty with 
                                | [], [] -> ()
                                | _ -> errorR (Error(FSComp.SR.tcMemberAndLocalClassBindingHaveSameName nm, bind.Var.Range))

                            // Also add static entries to the envInstance if necessary 
                            let envInstance = (if isStatic then (binds, envInstance) ||> List.foldBack (fun b e -> AddLocalVal g cenv.tcSink scopem b.Var e) else env)
                            let envStatic = (if isStatic then env else envStatic)
                            let innerState = (tpenv, envInstance, envStatic, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable)
                            Phase2BIncrClassBindings bindRs, innerState
                              
                        | Phase2AIncrClassCtorJustAfterSuperInit -> 
                            let innerState = (tpenv, envInstance, envStatic, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable)
                            Phase2BIncrClassCtorJustAfterSuperInit, innerState
                            
                        | Phase2AIncrClassCtorJustAfterLastLet -> 
                            let innerState = (tpenv, envInstance, envStatic, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable)
                            Phase2BIncrClassCtorJustAfterLastLet, innerState
                            
                            
#if OPEN_IN_TYPE_DECLARATIONS
                        | Phase2AOpen(target, m) -> 
                            let envInstance = TcOpenDecl cenv m scopem envInstance target
                            let envStatic = TcOpenDecl cenv m scopem envStatic target
                            let innerState = (tpenv, envInstance, envStatic, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable)
                            Phase2BOpen, innerState
#endif


                        // Note: this path doesn't add anything the environment, because the member is already available off via its type 
                        
                        | Phase2AMember rbind ->

                            // Phase2B: Typecheck member binding, generalize them later, when all type constraints are known 
                            // static members are checked under envStatic.
                            // envStatic contains class typars and the (ungeneralized) members on the class(es).
                            // envStatic has no instance-variables (local let-bindings or ctor args). 

                            let v = rbind.RecBindingInfo .Val
                            let envForBinding = if v.IsInstanceMember then envInstance else envStatic

                            // Type variables derived from the type definition (or implicit constructor) are always generalizable (we check their generalizability later).
                            // Note they may be solved to be equi-recursive.
                            let extraGeneralizableTypars = copyOfTyconTypars

                            // Inside the incremental class syntax we assert the type of the 'this' variable to be precisely the same type as the 
                            // this variable for the implicit class constructor. For static members, we assert the type variables associated
                            // for the class to be identical to those used for the implicit class constructor and the static class constructor.
                            //
                            // See TcLetrecBinding where this information is consumed.

                            // Type check the member and apply early generalization.
                            // We ignore the tpenv returned by checking each member. Each member gets checked in a fresh, clean tpenv
                            let envNonRec, generalizedRecBinds, preGeneralizationRecBinds, _, uncheckedRecBindsTable = 
                                TcLetrecBinding (cenv, envForBinding, scopem, extraGeneralizableTypars, reqdThisValTyOpt) (envNonRec, generalizedRecBinds, preGeneralizationRecBinds, tpenv, uncheckedRecBindsTable) rbind
                             
                            let innerState = (tpenv, envInstance, envStatic, envNonRec, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable)
                            Phase2BMember rbind.RecBindingInfo.Index, innerState)
                
                let defnBs = MutRecShape.Tycon (TyconBindingsPhase2B(tyconOpt, tcref, defnBs))
                let outerState = (tpenv, generalizedRecBinds, preGeneralizationRecBinds, uncheckedRecBindsTable, envNonRec)
                defnBs, outerState)

        // There should be no bindings that have not been generalized since checking the vary last binding always
        // results in the generalization of all remaining ungeneralized bindings, since there are no remaining unchecked bindings
        // to prevent the generalization 
        assert preGeneralizationRecBinds.IsEmpty

        defnsBs, generalizedRecBinds, tpenv


    // Choose type scheme implicit constructors and adjust their recursive types.
    // Fixup recursive references to members.
    let TcMutRecBindings_Phase2C_FixupRecursiveReferences (cenv: cenv) (denv, defnsBs: MutRecDefnsPhase2BData, generalizedTyparsForRecursiveBlock: Typar list, generalizedRecBinds: PostGeneralizationRecursiveBinding list, scopem) =
        let g = cenv.g

        // Build an index ---> binding map
        let generalizedBindingsMap = generalizedRecBinds |> List.map (fun pgrbind -> (pgrbind.RecBindingInfo.Index, pgrbind)) |> Map.ofList

        defnsBs |> MutRecShapes.mapTyconsAndLets 

            // Phase2C: Fixup member bindings 
            (fun (TyconBindingsPhase2B(tyconOpt, tcref, defnBs)) -> 

                let defnCs = 
                    defnBs |> List.map (fun defnB -> 

                        // Phase2C: Generalise implicit ctor val 
                        match defnB with
                        | Phase2BIncrClassCtor (incrClassCtorLhs, safeThisValBindOpt) ->
                            let valscheme = incrClassCtorLhs.InstanceCtorValScheme
                            let valscheme = ChooseCanonicalValSchemeAfterInference g denv valscheme scopem
                            AdjustRecType incrClassCtorLhs.InstanceCtorVal valscheme
                            Phase2CIncrClassCtor (incrClassCtorLhs, safeThisValBindOpt)

                        | Phase2BInherit (inheritsExpr, basevOpt) -> 
                            Phase2CInherit (inheritsExpr, basevOpt)

                        | Phase2BIncrClassBindings bindRs -> 
                            Phase2CIncrClassBindings bindRs

                        | Phase2BIncrClassCtorJustAfterSuperInit -> 
                            Phase2CIncrClassCtorJustAfterSuperInit

                        | Phase2BIncrClassCtorJustAfterLastLet -> 
                            Phase2CIncrClassCtorJustAfterLastLet

                        | Phase2BMember idx ->
                            // Phase2C: Fixup member bindings 
                            let generalizedBinding = generalizedBindingsMap[idx] 
                            let vxbind = TcLetrecAdjustMemberForSpecialVals cenv generalizedBinding
                            let pgbrind = FixupLetrecBind cenv denv generalizedTyparsForRecursiveBlock vxbind
                            Phase2CMember pgbrind)
                TyconBindingsPhase2C(tyconOpt, tcref, defnCs))

            // Phase2C: Fixup let bindings 
            (fun bindIdxs -> 
                    [ for idx in bindIdxs do 
                        let generalizedBinding = generalizedBindingsMap[idx] 
                        let vxbind = TcLetrecAdjustMemberForSpecialVals cenv generalizedBinding
                        yield FixupLetrecBind cenv denv generalizedTyparsForRecursiveBlock vxbind ])


    // --- Extract field bindings from let-bindings 
    // --- Extract method bindings from let-bindings 
    // --- Extract bindings for implicit constructors
    let TcMutRecBindings_Phase2D_ExtractImplicitFieldAndMethodBindings (cenv: cenv) envMutRec tpenv (denv, generalizedTyparsForRecursiveBlock, defnsCs: MutRecDefnsPhase2CData) =
            let g = cenv.g

      //  let (fixupValueExprBinds, methodBinds) = 
            (envMutRec, defnsCs) ||> MutRecShapes.mapTyconsWithEnv (fun envForDecls (TyconBindingsPhase2C(tyconOpt, tcref, defnCs)) -> 
                match defnCs with 
                | Phase2CIncrClassCtor (incrClassCtorLhs, safeThisValBindOpt) :: defnCs -> 

                    // Determine is static fields in this type need to be "protected" against invalid recursive initialization
                    let safeStaticInitInfo = 
                        // Safe static init checks are not added to FSharp.Core. The FailInit helper is not defined in some places, and 
                        // there are some minor concerns about performance w.r.t. these static bindings:
                        //
                        // set.fs (also map.fs)
                        //       static let empty: Set<'T> = 
                        //           let comparer = LanguagePrimitives.FastGenericComparer<'T> 
                        //           new Set<'T>(comparer, SetEmpty)
                        //
                        // prim-types.fs:
                        //       type TypeInfo<'T>() = 
                        //          static let info = 
                        //              let ty = typeof<'T>
                        //              ...
                        // and some others in prim-types.fs
                        //
                        // REVIEW: consider allowing an optimization switch to turn off these checks

                        let needsSafeStaticInit = not g.compilingFSharpCore
                        
                        // We only need safe static init checks if there are some static field bindings (actually, we look for non-method bindings)
                        let hasStaticBindings = 
                            defnCs |> List.exists (function 
                                | Phase2CIncrClassBindings groups -> 
                                    groups |> List.exists (function 
                                        | IncrClassBindingGroup(binds, isStatic, _) ->
                                            isStatic && (binds |> List.exists (IncrClassReprInfo.IsMethodRepr cenv >> not)) 
                                        | _ -> false) 
                                | _ -> false)

                        if needsSafeStaticInit && hasStaticBindings then
                            let rfield = MakeSafeInitField g envForDecls tcref.Range true
                            SafeInitField(mkRecdFieldRef tcref rfield.LogicalName, rfield)
                        else
                            NoSafeInitInfo


                    // This is the type definition we're processing  
                    let tcref = incrClassCtorLhs.TyconRef

                    // Assumes inherit call immediately follows implicit ctor. Checked by CheckMembersForm 
                    let inheritsExpr, inheritsIsVisible, _, defnCs = 
                        match defnCs |> List.partition (function Phase2CInherit _ -> true | _ -> false) with
                        | [Phase2CInherit (inheritsExpr, baseValOpt)], defnCs -> 
                            inheritsExpr, true, baseValOpt, defnCs

                        | _ ->
                            if tcref.IsStructOrEnumTycon then 
                                mkUnit g tcref.Range, false, None, defnCs
                            else
                                let inheritsExpr, _ = TcNewExpr cenv envForDecls tpenv g.obj_ty None true (SynExpr.Const (SynConst.Unit, tcref.Range)) tcref.Range

                                // If there is no 'inherits' and no simple non-static 'let' of a non-method then add a debug point at the entry to the constructor over the type name itself.
                                let addDebugPointAtImplicitCtorArguments =
                                    defnCs |> List.forall (fun defnC ->
                                        match defnC with
                                        | Phase2CIncrClassBindings binds -> 
                                            binds |> List.forall (fun bind ->
                                                match bind with
                                                | IncrClassBindingGroup(binds, isStatic, _) -> 
                                                    isStatic || 
                                                    binds |> List.forall (IncrClassReprInfo.IsMethodRepr cenv)
                                                | IncrClassDo(_, isStatic, _) ->
                                                    isStatic)
                                        | _ -> true) 

                                let inheritsExpr =
                                    if addDebugPointAtImplicitCtorArguments then
                                        mkDebugPoint tcref.Range inheritsExpr
                                    else
                                        inheritsExpr
                                inheritsExpr, false, None, defnCs
                       
                    let envForTycon = MakeInnerEnvForTyconRef envForDecls tcref false 

                    // Compute the cpath used when creating the hidden fields 
                    let cpath = envForTycon.eAccessPath

                    let localDecs = 
                        defnCs |> List.filter (function 
                            | Phase2CIncrClassBindings _ 
                            | Phase2CIncrClassCtorJustAfterSuperInit 
                            | Phase2CIncrClassCtorJustAfterLastLet -> true 
                            | _ -> false)
                    let memberBindsWithFixups = defnCs |> List.choose (function Phase2CMember pgrbind -> Some pgrbind | _ -> None) 

                    // Extend localDecs with "let safeThisVal = ref null" if there is a safeThisVal
                    let localDecs = 
                        match safeThisValBindOpt with 
                        | None -> localDecs 
                        | Some bind -> Phase2CIncrClassBindings [IncrClassBindingGroup([bind], false, false)] :: localDecs
                        
                    // Carve out the initialization sequence and decide on the localRep 
                    let ctorBodyLambdaExpr, cctorBodyLambdaExprOpt, methodBinds, localReps = 
                        
                        let localDecs = 
                            [ for localDec in localDecs do 
                                  match localDec with 
                                  | Phase2CIncrClassBindings binds -> yield Phase2CBindings binds
                                  | Phase2CIncrClassCtorJustAfterSuperInit -> yield Phase2CCtorJustAfterSuperInit
                                  | Phase2CIncrClassCtorJustAfterLastLet -> yield Phase2CCtorJustAfterLastLet
                                  | _ -> () ]
                        let memberBinds = memberBindsWithFixups |> List.map (fun x -> x.Binding) 
                        MakeCtorForIncrClassConstructionPhase2C(cenv, envForTycon, incrClassCtorLhs, inheritsExpr, inheritsIsVisible, localDecs, memberBinds, generalizedTyparsForRecursiveBlock, safeStaticInitInfo)

                    // Generate the (value, expr) pairs for the implicit 
                    // object constructor and implicit static initializer 
                    let ctorValueExprBindings = 
                        [ (let ctorValueExprBinding = TBind(incrClassCtorLhs.InstanceCtorVal, ctorBodyLambdaExpr, DebugPointAtBinding.NoneAtSticky)
                           let rbind = { ValScheme = incrClassCtorLhs.InstanceCtorValScheme ; Binding = ctorValueExprBinding }
                           FixupLetrecBind cenv envForDecls.DisplayEnv generalizedTyparsForRecursiveBlock rbind) ]
                        @ 
                        ( match cctorBodyLambdaExprOpt with 
                          | None -> []
                          | Some cctorBodyLambdaExpr -> 
                              [ (let _, cctorVal, cctorValScheme = incrClassCtorLhs.StaticCtorValInfo.Force()
                                 let cctorValueExprBinding = TBind(cctorVal, cctorBodyLambdaExpr, DebugPointAtBinding.NoneAtSticky)
                                 let rbind = { ValScheme = cctorValScheme; Binding = cctorValueExprBinding }
                                 FixupLetrecBind cenv envForDecls.DisplayEnv generalizedTyparsForRecursiveBlock rbind) ] ) 

                    // Publish the fields of the representation to the type 
                    localReps.PublishIncrClassFields (cenv, denv, cpath, incrClassCtorLhs, safeStaticInitInfo) (* mutation *)    
                    
                    // Fixup members
                    let memberBindsWithFixups = 
                        memberBindsWithFixups |> List.map (fun pgrbind -> 
                            let (TBind(v, x, spBind)) = pgrbind.Binding

                            // Work out the 'this' variable and type instantiation for field fixups. 
                            // We use the instantiation from the instance member if any. Note: It is likely this is not strictly needed 
                            // since we unify the types of the 'this' variables with those of the ctor declared typars. 
                            let thisValOpt = GetInstanceMemberThisVariable (v, x)

                            // Members have at least as many type parameters as the enclosing class. Just grab the type variables for the type.
                            let thisTyInst = List.map mkTyparTy (List.truncate (tcref.Typars(v.Range).Length) v.Typars)
                                    
                            let x = localReps.FixupIncrClassExprPhase2C cenv thisValOpt safeStaticInitInfo thisTyInst x 

                            { pgrbind with Binding = TBind(v, x, spBind) } )
                        
                    tyconOpt, ctorValueExprBindings @ memberBindsWithFixups, methodBinds  
                
                // Cover the case where this is not a class with an implicit constructor
                | defnCs -> 
                    let memberBindsWithFixups = defnCs |> List.choose (function Phase2CMember pgrbind -> Some pgrbind | _ -> None) 
                    tyconOpt, memberBindsWithFixups, [])

    /// Check a "module X = A.B.C" module abbreviation declaration
    let TcModuleAbbrevDecl (cenv: cenv) scopem (env: TcEnv) (id, p, m) = 
        let g = cenv.g
        let ad = env.AccessRights
        let resolved =
            match p with
            | [] -> Result []
            | id :: rest -> ResolveLongIdentAsModuleOrNamespace cenv.tcSink ResultCollectionSettings.AllResults cenv.amap m true OpenQualified env.NameEnv ad id rest false

        let mvvs = ForceRaise resolved

        if isNil mvvs then env else
        let modrefs = mvvs |> List.map p23

        if not (isNil modrefs) && modrefs |> List.forall (fun modref -> modref.IsNamespace) then 
            errorR(Error(FSComp.SR.tcModuleAbbreviationForNamespace(fullDisplayTextOfModRef (List.head modrefs)), m))

        let modrefs = modrefs |> List.filter (fun mvv -> not mvv.IsNamespace)

        if isNil modrefs then env else 
        modrefs |> List.iter (fun modref -> CheckEntityAttributes g modref m |> CommitOperationResult)        
        let env = AddModuleAbbreviationAndReport cenv.tcSink scopem id modrefs env
        env

    /// Update the contents accessible via the recursive namespace declaration, if any
    let TcMutRecDefns_UpdateNSContents mutRecNSInfo =
        match mutRecNSInfo with 
        | Some (Some (modulNS: ModuleOrNamespace), moduleTyAcc: _ ref) -> 
            modulNS.entity_modul_type <- MaybeLazy.Strict moduleTyAcc.Value
        | _ -> ()  

    /// Updates the types of the modules to contain the contents so far
    let TcMutRecDefns_UpdateModuleContents mutRecNSInfo defns =
        defns |> MutRecShapes.iterModules (fun (MutRecDefnsPhase2DataForModule (moduleTyAcc, moduleEntity), _) -> 
              moduleEntity.entity_modul_type <- MaybeLazy.Strict moduleTyAcc.Value)  

        TcMutRecDefns_UpdateNSContents mutRecNSInfo
    
    /// Compute the active environments within each nested module.
    let TcMutRecDefns_ComputeEnvs getTyconOpt getVals (cenv: cenv) report scopem m envInitial mutRecShape =
        let g = cenv.g
        (envInitial, mutRecShape) ||> MutRecShapes.computeEnvs 
            (fun envAbove (MutRecDefnsPhase2DataForModule (moduleTyAcc, moduleEntity)) ->
                MakeInnerEnvWithAcc true envAbove moduleEntity.Id moduleTyAcc moduleEntity.ModuleOrNamespaceType.ModuleOrNamespaceKind)

            (fun envAbove decls -> 

                // Collect the type definitions, exception definitions, modules and "open" declarations
                let tycons = decls |> List.choose (function MutRecShape.Tycon d -> getTyconOpt d | _ -> None) 
                let moduls = decls |> List.choose (function MutRecShape.Module (MutRecDefnsPhase2DataForModule (_, moduleEntity), _) -> Some moduleEntity | _ -> None)
                let moduleAbbrevs = decls |> List.choose (function MutRecShape.ModuleAbbrev (MutRecDataForModuleAbbrev (id, mp, m)) -> Some (id, mp, m) | _ -> None)
                let opens = decls |> List.choose (function MutRecShape.Open (MutRecDataForOpen (target, m, moduleRange, openDeclsRef)) -> Some (target, m, moduleRange, openDeclsRef) | _ -> None)
                let lets = decls |> List.collect (function MutRecShape.Lets binds -> getVals binds | _ -> [])
                let exns = tycons |> List.filter (fun (tycon: Tycon) -> tycon.IsFSharpException)

                // Add the type definitions, exceptions, modules and "open" declarations.
                // The order here is sensitive. The things added first will be resolved in an environment
                // where not everything has been added. The things added last will be preferred in name 
                // resolution.
                //
                // 'open' declarations ('open M') may refer to modules being defined ('M') and so must be
                // processed in an environment where 'M' is present. However, in later processing the names of 
                // modules being defined ('M') take precedence over those coming from 'open' declarations.  
                // So add the names of the modules being defined to the environment twice - once to allow 
                // the processing of 'open M', and once to allow the correct name resolution of 'M'.
                //
                // Module abbreviations being defined ('module M = A.B.C') are not available for use in 'open'
                // declarations. So
                //    namespace rec N = 
                //       open M
                //       module M = FSharp.Core.Operators
                // is not allowed.

                let envForDecls = envAbove

                // Add the modules being defined
                let envForDecls = (envForDecls, moduls) ||> List.fold ((if report then AddLocalSubModuleAndReport cenv.tcSink scopem else AddLocalSubModule) g cenv.amap m)

                // Process the 'open' declarations                
                let envForDecls =
                    (envForDecls, opens) ||> List.fold (fun env (target, m, moduleRange, openDeclsRef) ->
                        let env, openDecls = TcOpenDecl cenv m moduleRange env target
                        openDeclsRef.Value <- openDecls
                        env)

                // Add the type definitions being defined
                let envForDecls = (if report then AddLocalTyconsAndReport cenv.tcSink scopem else AddLocalTycons) g cenv.amap m tycons envForDecls 
                // Add the exception definitions being defined
                let envForDecls = (envForDecls, exns) ||> List.fold (AddLocalExnDefnAndReport cenv.tcSink scopem)
                // Add the modules again (but don't report them a second time)
                let envForDecls = (envForDecls, moduls) ||> List.fold (AddLocalSubModule g cenv.amap m)
                // Add the module abbreviations
                let envForDecls = (envForDecls, moduleAbbrevs) ||> List.fold (TcModuleAbbrevDecl cenv scopem)
                // Add the values and members
                let envForDecls = AddLocalVals g cenv.tcSink scopem lets envForDecls
                envForDecls)

    /// Phase 2: Check the members and 'let' definitions in a mutually recursive group of definitions.
    let TcMutRecDefns_Phase2_Bindings (cenv: cenv) envInitial tpenv mBinds scopem mutRecNSInfo (envMutRecPrelimWithReprs: TcEnv) (mutRecDefns: MutRecDefnsPhase2Info) =
        let g = cenv.g
        let denv = envMutRecPrelimWithReprs.DisplayEnv
        
        // Phase2A: create member prelimRecValues for "recursive" items, i.e. ctor val and member vals 
        // Phase2A: also processes their arg patterns - collecting type assertions 
        let defnsAs, uncheckedRecBinds, tpenv = TcMutRecBindings_Phase2A_CreateRecursiveValuesAndCheckArgumentPatterns cenv tpenv (envMutRecPrelimWithReprs, mutRecDefns)

        // Now basic member values are created we can compute the final attributes (i.e. in the case where attributes refer to constructors being defined)
        mutRecDefns |> MutRecShapes.iterTycons (fun (MutRecDefnsPhase2InfoForTycon(_, _, _, _, _, fixupFinalAttrs)) -> 
                fixupFinalAttrs())  

        // Updates the types of the modules to contain the contents so far, which now includes values and members
        TcMutRecDefns_UpdateModuleContents mutRecNSInfo defnsAs

        // Updates the environments to include the values
        // We must open all modules from scratch again because there may be extension methods and/or AutoOpen
        let envMutRec, defnsAs =  
            (envInitial, MutRecShapes.dropEnvs defnsAs) 
            ||> TcMutRecDefns_ComputeEnvs 
                   (fun (TyconBindingsPhase2A(tyconOpt, _, _, _, _, _, _)) -> tyconOpt) 
                   (fun binds -> [ for bind in binds -> bind.RecBindingInfo.Val ]) 
                   cenv false scopem scopem 
            ||> MutRecShapes.extendEnvs (fun envForDecls decls -> 

                let prelimRecValues =  
                    decls |> List.collect (function 
                        | MutRecShape.Tycon (TyconBindingsPhase2A(_, _, prelimRecValues, _, _, _, _)) -> prelimRecValues 
                        | MutRecShape.Lets binds -> [ for bind in binds -> bind.RecBindingInfo.Val ] 
                        | _ -> [])

                let ctorVals = 
                    decls |> MutRecShapes.topTycons |> List.collect (fun (TyconBindingsPhase2A(_, _, _, _, _, _, defnAs)) ->
                    [ for defnB in defnAs do
                        match defnB with
                        | Phase2AIncrClassCtor incrClassCtorLhs -> yield incrClassCtorLhs.InstanceCtorVal
                        | _ -> () ])

                let envForDeclsUpdated = 
                    envForDecls
                    |> AddLocalVals g cenv.tcSink scopem prelimRecValues 
                    |> AddLocalVals g cenv.tcSink scopem ctorVals 

                envForDeclsUpdated)

        // Phase2B: type check pass, convert from ast to tast and collects type assertions, and generalize
        let defnsBs, generalizedRecBinds, tpenv = TcMutRecBindings_Phase2B_TypeCheckAndIncrementalGeneralization cenv tpenv envInitial (envMutRec, defnsAs, uncheckedRecBinds, scopem)

        let generalizedTyparsForRecursiveBlock = 
             generalizedRecBinds 
                |> List.map (fun pgrbind -> pgrbind.GeneralizedTypars)
                |> unionGeneralizedTypars

        // Check the escape condition for all extraGeneralizableTypars.
        // First collect up all the extraGeneralizableTypars.
        let allExtraGeneralizableTypars = 
            defnsAs |> MutRecShapes.collectTycons |> List.collect (fun (TyconBindingsPhase2A(_, _, _, _, copyOfTyconTypars, _, defnAs)) ->
                [ yield! copyOfTyconTypars
                  for defnA in defnAs do 
                      match defnA with
                      | Phase2AMember rbind -> yield! rbind.RecBindingInfo.EnclosingDeclaredTypars
                      | _ -> () ])

        // Now check they don't escape the overall scope of the recursive set of types
        if not (isNil allExtraGeneralizableTypars) then         
            let freeInInitialEnv = GeneralizationHelpers.ComputeUngeneralizableTypars envInitial
            for extraTypar in allExtraGeneralizableTypars do 
                if Zset.memberOf freeInInitialEnv extraTypar then
                    let ty = mkTyparTy extraTypar
                    error(Error(FSComp.SR.tcNotSufficientlyGenericBecauseOfScope(NicePrint.prettyStringOfTy denv ty), extraTypar.Range))                                

        // Solve any type variables in any part of the overall type signature of the class whose
        // constraints involve generalized type variables.
        //
        // This includes property, member and constructor argument types that couldn't be fully generalized because they
        // involve generalized copies of class type variables.
        let unsolvedTyparsForRecursiveBlockInvolvingGeneralizedVariables = 
             let genSet = (freeInTypes CollectAllNoCaching [ for tp in generalizedTyparsForRecursiveBlock -> mkTyparTy tp ]).FreeTypars
             //printfn "genSet.Count = %d" genSet.Count
             let allTypes = 
                 [ for pgrbind in generalizedRecBinds do 
                      yield pgrbind.RecBindingInfo.Val.Type 
                   for TyconBindingsPhase2B(_tyconOpt, _tcref, defnBs) in MutRecShapes.collectTycons defnsBs do
                      for defnB in defnBs do
                        match defnB with
                        | Phase2BIncrClassCtor (incrClassCtorLhs, _) ->
                            yield incrClassCtorLhs.InstanceCtorVal.Type
                        | _ -> 
                            ()
                  ]
             //printfn "allTypes.Length = %d" allTypes.Length
             let unsolvedTypars = freeInTypesLeftToRight g true allTypes
             //printfn "unsolvedTypars.Length = %d" unsolvedTypars.Length
             //for x in unsolvedTypars do 
             //    printfn "unsolvedTypar: %s #%d" x.DisplayName x.Stamp
             let unsolvedTyparsInvolvingGeneralizedVariables =
                 unsolvedTypars |> List.filter (fun tp -> 
                     let freeInTypar = (freeInType CollectAllNoCaching (mkTyparTy tp)).FreeTypars
                     // Check it is not one of the generalized variables...
                     not (genSet.Contains tp) && 
                     // Check it involves a generalized variable in one of its constraints...
                     freeInTypar.Exists(fun otherTypar -> genSet.Contains otherTypar))
             //printfn "unsolvedTyparsInvolvingGeneralizedVariables.Length = %d" unsolvedTyparsInvolvingGeneralizedVariables.Length
             //for x in unsolvedTypars do 
             //    printfn "unsolvedTyparsInvolvingGeneralizedVariable: %s #%d" x.DisplayName x.Stamp
             unsolvedTyparsInvolvingGeneralizedVariables

        for tp in unsolvedTyparsForRecursiveBlockInvolvingGeneralizedVariables do
            //printfn "solving unsolvedTyparsInvolvingGeneralizedVariable: %s #%d" tp.DisplayName tp.Stamp
            if (tp.Rigidity <> TyparRigidity.Rigid) && not tp.IsSolved then 
                ChooseTyparSolutionAndSolve cenv.css denv tp
          
        // Now that we know what we've generalized we can adjust the recursive references 
        let defnsCs = TcMutRecBindings_Phase2C_FixupRecursiveReferences cenv (denv, defnsBs, generalizedTyparsForRecursiveBlock, generalizedRecBinds, scopem)

        // --- Extract field bindings from let-bindings 
        // --- Extract method bindings from let-bindings 
        // --- Extract bindings for implicit constructors
        let defnsDs = TcMutRecBindings_Phase2D_ExtractImplicitFieldAndMethodBindings cenv envMutRec tpenv (denv, generalizedTyparsForRecursiveBlock, defnsCs)
        
        // Phase2E - rewrite values to initialization graphs
        let defnsEs = 
           EliminateInitializationGraphs 
             //(fun morpher (tyconOpt, fixupValueExprBinds, methodBinds) -> (tyconOpt, morpher fixupValueExprBinds @ methodBinds))
             g true denv defnsDs
             (fun morpher shape -> shape |> MutRecShapes.iterTyconsAndLets (p23 >> morpher) morpher)
             MutRecShape.Lets
             (fun morpher shape -> shape |> MutRecShapes.mapTyconsAndLets (fun (tyconOpt, fixupValueExprBinds, methodBinds) -> tyconOpt, (morpher fixupValueExprBinds @ methodBinds)) morpher)
             mBinds 
        
        defnsEs, envMutRec

/// Check and generalize the interface implementations, members, 'let' definitions in a mutually recursive group of definitions.
let TcMutRecDefns_Phase2 (cenv: cenv) envInitial mBinds scopem mutRecNSInfo (envMutRec: TcEnv) (mutRecDefns: MutRecDefnsPhase2Data) = 
    let g = cenv.g
    let interfacesFromTypeDefn envForTycon tyconMembersData = 
        let (MutRecDefnsPhase2DataForTycon(_, _, declKind, tcref, _, _, declaredTyconTypars, members, _, _, _)) = tyconMembersData
        let overridesOK = DeclKind.CanOverrideOrImplement declKind
        members |> List.collect (function
            | SynMemberDefn.Interface(interfaceType=intfTy; members=defnOpt) -> 
                  let ty = if tcref.Deref.IsFSharpException then g.exn_ty else generalizedTyconRef g tcref
                  let m = intfTy.Range
                  if tcref.IsTypeAbbrev then error(Error(FSComp.SR.tcTypeAbbreviationsCannotHaveInterfaceDeclaration(), m))
                  if tcref.IsEnumTycon then error(Error(FSComp.SR.tcEnumerationsCannotHaveInterfaceDeclaration(), m))

                  let intfTyR = 
                      let envinner = AddDeclaredTypars CheckForDuplicateTypars declaredTyconTypars envForTycon
                      TcTypeAndRecover cenv NoNewTypars CheckCxs ItemOccurence.UseInType envinner emptyUnscopedTyparEnv intfTy |> fst

                  if not (tcref.HasInterface g intfTyR) then 
                      error(Error(FSComp.SR.tcAllImplementedInterfacesShouldBeDeclared(), intfTy.Range))
                   
                  let generatedCompareToValues = tcref.GeneratedCompareToValues.IsSome
                  let generatedHashAndEqualsWithComparerValues = tcref.GeneratedHashAndEqualsWithComparerValues.IsSome
                  let generatedCompareToWithComparerValues = tcref.GeneratedCompareToWithComparerValues.IsSome
                  
                  if (generatedCompareToValues && typeEquiv g intfTyR g.mk_IComparable_ty) || 
                      (generatedCompareToWithComparerValues && typeEquiv g intfTyR g.mk_IStructuralComparable_ty) ||
                      (generatedCompareToValues && typeEquiv g intfTyR (mkAppTy g.system_GenericIComparable_tcref [ty])) ||
                      (generatedHashAndEqualsWithComparerValues && typeEquiv g intfTyR (mkAppTy g.system_GenericIEquatable_tcref [ty])) ||
                      (generatedHashAndEqualsWithComparerValues && typeEquiv g intfTyR g.mk_IStructuralEquatable_ty) then
                      errorR(Error(FSComp.SR.tcDefaultImplementationForInterfaceHasAlreadyBeenAdded(), intfTy.Range))

                  if overridesOK = WarnOnOverrides then  
                      warning(IntfImplInIntrinsicAugmentation(intfTy.Range))
                  if overridesOK = ErrorOnOverrides then  
                      errorR(IntfImplInExtrinsicAugmentation(intfTy.Range))
                  match defnOpt with 
                  | Some defn -> [ (intfTyR, defn, m) ]
                  | _-> []
                  
            | _ -> []) 

    let interfaceMembersFromTypeDefn tyconMembersData (intfTyR, defn, _) implTySet = 
        let (MutRecDefnsPhase2DataForTycon(_, parent, declKind, tcref, baseValOpt, safeInitInfo, declaredTyconTypars, _, _, newslotsOK, _)) = tyconMembersData
        let containerInfo = ContainerInfo(parent, Some(MemberOrValContainerInfo(tcref, Some(intfTyR, implTySet), baseValOpt, safeInitInfo, declaredTyconTypars)))
        defn |> List.choose (fun mem ->
            match mem with
            | SynMemberDefn.Member(_, m) -> Some(TyconBindingDefn(containerInfo, newslotsOK, declKind, mem, m))
            | SynMemberDefn.AutoProperty(range=m) -> Some(TyconBindingDefn(containerInfo, newslotsOK, declKind, mem, m))
            | _ -> errorR(Error(FSComp.SR.tcMemberNotPermittedInInterfaceImplementation(), mem.Range)); None)

    let tyconBindingsOfTypeDefn (MutRecDefnsPhase2DataForTycon(_, parent, declKind, tcref, baseValOpt, safeInitInfo, declaredTyconTypars, members, _, newslotsOK, _)) = 
        let containerInfo = ContainerInfo(parent, Some(MemberOrValContainerInfo(tcref, None, baseValOpt, safeInitInfo, declaredTyconTypars)))
        members 
        |> List.choose (fun memb ->
            match memb with 
            | SynMemberDefn.ImplicitCtor _
            | SynMemberDefn.ImplicitInherit _
            | SynMemberDefn.LetBindings _
            | SynMemberDefn.AutoProperty _
            | SynMemberDefn.Member _
            | SynMemberDefn.GetSetMember _
            | SynMemberDefn.Open _
                -> Some(TyconBindingDefn(containerInfo, newslotsOK, declKind, memb, memb.Range))

            // Interfaces exist in the member list - handled above in interfaceMembersFromTypeDefn 
            | SynMemberDefn.Interface _ -> None

            // The following should have been List.unzip out already in SplitTyconDefn 
            | SynMemberDefn.AbstractSlot _
            | SynMemberDefn.ValField _             
            | SynMemberDefn.Inherit _ -> error(InternalError("Unexpected declaration element", memb.Range))
            | SynMemberDefn.NestedType _ -> error(Error(FSComp.SR.tcTypesCannotContainNestedTypes(), memb.Range)))
          
    let tpenv = emptyUnscopedTyparEnv

    try
      // Some preliminary checks 
      mutRecDefns |> MutRecShapes.iterTycons (fun tyconData ->
             let (MutRecDefnsPhase2DataForTycon(_, _, declKind, tcref, _, _, _, members, m, newslotsOK, _)) = tyconData
             let tcaug = tcref.TypeContents
             if tcaug.tcaug_closed && declKind <> ExtrinsicExtensionBinding then 
               error(InternalError("Intrinsic augmentations of types are only permitted in the same file as the definition of the type", m))
             members |> List.iter (fun mem ->
                    match mem with
                    | SynMemberDefn.Member _
                    | SynMemberDefn.GetSetMember _
                    | SynMemberDefn.Interface _ -> () 
                    | SynMemberDefn.Open _ 
                    | SynMemberDefn.AutoProperty _
                    | SynMemberDefn.LetBindings _  // accept local definitions 
                    | SynMemberDefn.ImplicitCtor _ // accept implicit ctor pattern, should be first! 
                    | SynMemberDefn.ImplicitInherit _ when newslotsOK = NewSlotsOK -> () // accept implicit ctor pattern, should be first! 
                    // The rest should have been removed by splitting, they belong to "core" (they are "shape" of type, not implementation) 
                    | _ -> error(Error(FSComp.SR.tcDeclarationElementNotPermittedInAugmentation(), mem.Range))))


      let binds: MutRecDefnsPhase2Info = 
          (envMutRec, mutRecDefns) ||> MutRecShapes.mapTyconsWithEnv (fun envForDecls tyconData -> 
              let (MutRecDefnsPhase2DataForTycon(tyconOpt, _, declKind, tcref, _, _, declaredTyconTypars, _, _, _, fixupFinalAttrs)) = tyconData
              let envForDecls = 
                // This allows to implement protected interface methods if it's a DIM.
                // Does not need to be hidden behind a lang version as it needs to be possible to
                //     implement protected interface methods in lower F# versions regardless if it's a DIM or not.
                match tyconOpt with
                | Some _ when declKind = DeclKind.ModuleOrMemberBinding ->
                    MakeInnerEnvForTyconRef envForDecls tcref false
                | _ -> 
                    envForDecls
              let obinds = tyconBindingsOfTypeDefn tyconData
              let ibinds = 
                      let intfTypes = interfacesFromTypeDefn envForDecls tyconData
                      let slotImplSets = DispatchSlotChecking.GetSlotImplSets cenv.infoReader envForDecls.DisplayEnv envForDecls.AccessRights false (List.map (fun (intfTy, _, m) -> (intfTy, m)) intfTypes)
                      (intfTypes, slotImplSets) ||> List.map2 (interfaceMembersFromTypeDefn tyconData) |> List.concat
              MutRecDefnsPhase2InfoForTycon(tyconOpt, tcref, declaredTyconTypars, declKind, obinds @ ibinds, fixupFinalAttrs))
      
      MutRecBindingChecking.TcMutRecDefns_Phase2_Bindings cenv envInitial tpenv mBinds scopem mutRecNSInfo envMutRec binds

    with exn -> errorRecovery exn scopem; [], envMutRec

//-------------------------------------------------------------------------
// Build augmentation declarations
//------------------------------------------------------------------------- 

module AddAugmentationDeclarations = 
    let tcaugHasNominalInterface g (tcaug: TyconAugmentation) tcref =
        tcaug.tcaug_interfaces |> List.exists (fun (x, _, _) -> 
            match tryTcrefOfAppTy g x with
            | ValueSome tcref2 when tyconRefEq g tcref2 tcref -> true
            | _ -> false)

    let AddGenericCompareDeclarations (cenv: cenv) (env: TcEnv) (scSet: Set<Stamp>) (tycon: Tycon) =
        let g = cenv.g
        if AugmentWithHashCompare.TyconIsCandidateForAugmentationWithCompare g tycon && scSet.Contains tycon.Stamp then 
            let tcref = mkLocalTyconRef tycon
            let tcaug = tycon.TypeContents
            let ty = if tcref.Deref.IsFSharpException then g.exn_ty else generalizedTyconRef g tcref
            let m = tycon.Range
            let genericIComparableTy = mkAppTy g.system_GenericIComparable_tcref [ty]


            let hasExplicitIComparable = tycon.HasInterface g g.mk_IComparable_ty 
            let hasExplicitGenericIComparable = tcaugHasNominalInterface g tcaug g.system_GenericIComparable_tcref    
            let hasExplicitIStructuralComparable = tycon.HasInterface g g.mk_IStructuralComparable_ty

            if hasExplicitIComparable then 
                errorR(Error(FSComp.SR.tcImplementsIComparableExplicitly(tycon.DisplayName), m)) 
      
            elif hasExplicitGenericIComparable then 
                errorR(Error(FSComp.SR.tcImplementsGenericIComparableExplicitly(tycon.DisplayName), m)) 
            elif hasExplicitIStructuralComparable then
                errorR(Error(FSComp.SR.tcImplementsIStructuralComparableExplicitly(tycon.DisplayName), m)) 
            else
                let hasExplicitGenericIComparable = tycon.HasInterface g genericIComparableTy
                let cvspec1, cvspec2 = AugmentWithHashCompare.MakeValsForCompareAugmentation g tcref
                let cvspec3 = AugmentWithHashCompare.MakeValsForCompareWithComparerAugmentation g tcref

                PublishInterface cenv env.DisplayEnv tcref m true g.mk_IStructuralComparable_ty
                PublishInterface cenv env.DisplayEnv tcref m true g.mk_IComparable_ty
                if not tycon.IsFSharpException && not hasExplicitGenericIComparable then 
                    PublishInterface cenv env.DisplayEnv tcref m true genericIComparableTy
                tcaug.SetCompare (mkLocalValRef cvspec1, mkLocalValRef cvspec2)
                tcaug.SetCompareWith (mkLocalValRef cvspec3)
                PublishValueDefn cenv env ModuleOrMemberBinding cvspec1
                PublishValueDefn cenv env ModuleOrMemberBinding cvspec2
                PublishValueDefn cenv env ModuleOrMemberBinding cvspec3

    let AddGenericEqualityWithComparerDeclarations (cenv: cenv) (env: TcEnv) (seSet: Set<Stamp>) (tycon: Tycon) =
        let g = cenv.g
        if AugmentWithHashCompare.TyconIsCandidateForAugmentationWithEquals g tycon && seSet.Contains tycon.Stamp then 
            let tcref = mkLocalTyconRef tycon
            let tcaug = tycon.TypeContents
            let m = tycon.Range

            let hasExplicitIStructuralEquatable = tycon.HasInterface g g.mk_IStructuralEquatable_ty

            if hasExplicitIStructuralEquatable then
                errorR(Error(FSComp.SR.tcImplementsIStructuralEquatableExplicitly(tycon.DisplayName), m)) 
            else
                let evspec1, evspec2, evspec3 = AugmentWithHashCompare.MakeValsForEqualityWithComparerAugmentation g tcref
                PublishInterface cenv env.DisplayEnv tcref m true g.mk_IStructuralEquatable_ty                
                tcaug.SetHashAndEqualsWith (mkLocalValRef evspec1, mkLocalValRef evspec2, mkLocalValRef evspec3)
                PublishValueDefn cenv env ModuleOrMemberBinding evspec1
                PublishValueDefn cenv env ModuleOrMemberBinding evspec2
                PublishValueDefn cenv env ModuleOrMemberBinding evspec3

    let AddGenericCompareBindings (cenv: cenv) (tycon: Tycon) =
        if (* AugmentWithHashCompare.TyconIsCandidateForAugmentationWithCompare g tycon && *) Option.isSome tycon.GeneratedCompareToValues then 
            AugmentWithHashCompare.MakeBindingsForCompareAugmentation cenv.g tycon
        else
            []
            
    let AddGenericCompareWithComparerBindings (cenv: cenv) (tycon: Tycon) =
        let g = cenv.g
        if Option.isSome tycon.GeneratedCompareToWithComparerValues then
            AugmentWithHashCompare.MakeBindingsForCompareWithComparerAugmentation g tycon
        else
            []
             
    let AddGenericEqualityWithComparerBindings (cenv: cenv) (tycon: Tycon) =
        let g = cenv.g
        if AugmentWithHashCompare.TyconIsCandidateForAugmentationWithEquals g tycon && Option.isSome tycon.GeneratedHashAndEqualsWithComparerValues then
            (AugmentWithHashCompare.MakeBindingsForEqualityWithComparerAugmentation g tycon)
        else
            []

    let AddGenericHashAndComparisonDeclarations (cenv: cenv) (env: TcEnv) scSet seSet tycon =
        AddGenericCompareDeclarations cenv env scSet tycon
        AddGenericEqualityWithComparerDeclarations cenv env seSet tycon

    let AddGenericHashAndComparisonBindings cenv tycon =
        AddGenericCompareBindings cenv tycon @ AddGenericCompareWithComparerBindings cenv tycon @ AddGenericEqualityWithComparerBindings cenv tycon

    // We can only add the Equals override after we've done the augmentation because we have to wait until 
    // tycon.HasOverride can give correct results 
    let AddGenericEqualityBindings (cenv: cenv) (env: TcEnv) tycon =
        let g = cenv.g
        if AugmentWithHashCompare.TyconIsCandidateForAugmentationWithEquals g tycon then 
            let tcref = mkLocalTyconRef tycon
            let tcaug = tycon.TypeContents
            let ty = if tcref.Deref.IsFSharpException then g.exn_ty else generalizedTyconRef g tcref
            let m = tycon.Range
            
            // Note: tycon.HasOverride only gives correct results after we've done the type augmentation 
            let hasExplicitObjectEqualsOverride = tycon.HasOverride g "Equals" [g.obj_ty]
            let hasExplicitGenericIEquatable = tcaugHasNominalInterface g tcaug g.system_GenericIEquatable_tcref
            
            if hasExplicitGenericIEquatable then 
                errorR(Error(FSComp.SR.tcImplementsIEquatableExplicitly(tycon.DisplayName), m)) 

            // Note: only provide the equals method if Equals is not implemented explicitly, and
            // we're actually generating Hash/Equals for this type
            if not hasExplicitObjectEqualsOverride &&
                Option.isSome tycon.GeneratedHashAndEqualsWithComparerValues then

                 let vspec1, vspec2 = AugmentWithHashCompare.MakeValsForEqualsAugmentation g tcref
                 tcaug.SetEquals (mkLocalValRef vspec1, mkLocalValRef vspec2)
                 if not tycon.IsFSharpException then 
                    PublishInterface cenv env.DisplayEnv tcref m true (mkAppTy g.system_GenericIEquatable_tcref [ty])
                 PublishValueDefn cenv env ModuleOrMemberBinding vspec1
                 PublishValueDefn cenv env ModuleOrMemberBinding vspec2
                 AugmentWithHashCompare.MakeBindingsForEqualsAugmentation g tycon
            else []
        else []



/// Infer 'comparison' and 'equality' constraints from type definitions
module TyconConstraintInference = 

    /// Infer 'comparison' constraints from type definitions
    let InferSetOfTyconsSupportingComparable (cenv: cenv) (denv: DisplayEnv) tyconsWithStructuralTypes =

        let g = cenv.g 
        let tab = tyconsWithStructuralTypes |> List.map (fun (tycon: Tycon, structuralTypes) -> tycon.Stamp, (tycon, structuralTypes)) |> Map.ofList 

        // Initially, assume the equality relation is available for all structural type definitions 
        let initialAssumedTycons = 
            set [ for tycon, _ in tyconsWithStructuralTypes do 
                       if AugmentWithHashCompare.TyconIsCandidateForAugmentationWithCompare g tycon then 
                           yield tycon.Stamp ]

        // Initially, don't assume that the equality relation is dependent on any type variables
        let initialAssumedTypars = Set.empty

        // Repeatedly eliminate structural type definitions whose structural component types no longer support 
        // comparison. On the way record type variables which are support the comparison relation.
        let rec loop (assumedTycons: Set<Stamp>) (assumedTypars: Set<Stamp>) =
            let mutable assumedTyparsAcc = assumedTypars

            // Checks if a field type supports the 'comparison' constraint based on the assumptions about the type constructors
            // and type parameters.
            let rec checkIfFieldTypeSupportsComparison (tycon: Tycon) (ty: TType) =
                
                // Is the field type a type parameter?
                match tryDestTyparTy g ty with
                | ValueSome tp ->
                    // Look for an explicit 'comparison' constraint
                    if tp.Constraints |> List.exists (function TyparConstraint.SupportsComparison _ -> true | _ -> false) then 
                        true
                    
                    // Within structural types, type parameters can be optimistically assumed to have comparison
                    // We record the ones for which we have made this assumption.
                    elif tycon.TyparsNoRange |> List.exists (fun tp2 -> typarRefEq tp tp2) then 
                        assumedTyparsAcc <- assumedTyparsAcc.Add(tp.Stamp)
                        true
                    
                    else
                        false
                | _ ->
                    match ty with 
                    // Look for array, UIntPtr and IntPtr types
                    | SpecialComparableHeadType g tinst -> 
                        tinst |> List.forall (checkIfFieldTypeSupportsComparison tycon)

                    // Otherwise it's a nominal type
                    | _ -> 

                        match ty with
                        | AppTy g (tcref, tinst) ->
                            // Check the basic requirement - IComparable/IStructuralComparable or assumed-comparable
                            (if initialAssumedTycons.Contains tcref.Stamp then 
                                assumedTycons.Contains tcref.Stamp
                             else
                                ExistsSameHeadTypeInHierarchy g cenv.amap range0 ty g.mk_IComparable_ty || 
                                ExistsSameHeadTypeInHierarchy g cenv.amap range0 ty g.mk_IStructuralComparable_ty)
                            &&
                            // Check it isn't ruled out by the user
                            not (HasFSharpAttribute g g.attrib_NoComparisonAttribute tcref.Attribs)
                            &&
                            // Check the structural dependencies
                            (tinst, tcref.TyparsNoRange) ||> List.lengthsEqAndForall2 (fun ty tp -> 
                                if tp.ComparisonConditionalOn || assumedTypars.Contains tp.Stamp then 
                                    checkIfFieldTypeSupportsComparison tycon ty 
                                else 
                                    true) 
                        | _ ->
                            false

            let newSet = 
                assumedTycons |> Set.filter (fun tyconStamp -> 
                   let tycon, structuralTypes = tab[tyconStamp] 

                   if g.compilingFSharpCore && 
                      AugmentWithHashCompare.TyconIsCandidateForAugmentationWithCompare g tycon && 
                      not (HasFSharpAttribute g g.attrib_StructuralComparisonAttribute tycon.Attribs) && 
                      not (HasFSharpAttribute g g.attrib_NoComparisonAttribute tycon.Attribs) then 
                       errorR(Error(FSComp.SR.tcFSharpCoreRequiresExplicit(), tycon.Range)) 

                   let res = (structuralTypes |> List.forall (fst >> checkIfFieldTypeSupportsComparison tycon))

                   // If the type was excluded, say why
                   if not res then 
                       match TryFindFSharpBoolAttribute g g.attrib_StructuralComparisonAttribute tycon.Attribs with
                       | Some true -> 
                           match structuralTypes |> List.tryFind (fst >> checkIfFieldTypeSupportsComparison tycon >> not) with
                           | None -> 
                               assert false
                               failwith "unreachable"
                           | Some (ty, _) -> 
                               if isTyparTy g ty then 
                                   errorR(Error(FSComp.SR.tcStructuralComparisonNotSatisfied1(tycon.DisplayName, NicePrint.prettyStringOfTy denv ty), tycon.Range)) 
                               else 
                                   errorR(Error(FSComp.SR.tcStructuralComparisonNotSatisfied2(tycon.DisplayName, NicePrint.prettyStringOfTy denv ty), tycon.Range)) 
                       | Some false -> 
                           ()
                       
                       | None -> 
                           match structuralTypes |> List.tryFind (fst >> checkIfFieldTypeSupportsComparison tycon >> not) with
                           | None -> 
                               assert false
                               failwith "unreachable"
                           | Some (ty, _) -> 
                               // NOTE: these warnings are off by default - they are level 4 informational warnings
                               // PERF: this call to prettyStringOfTy is always being executed, even when the warning
                               // is not being reported (the normal case).
                               if isTyparTy g ty then 
                                   warning(Error(FSComp.SR.tcNoComparisonNeeded1(tycon.DisplayName, NicePrint.prettyStringOfTy denv ty, tycon.DisplayName), tycon.Range)) 
                               else 
                                   warning(Error(FSComp.SR.tcNoComparisonNeeded2(tycon.DisplayName, NicePrint.prettyStringOfTy denv ty, tycon.DisplayName), tycon.Range)) 

                                                      
                   res)

            if newSet = assumedTycons && assumedTypars = assumedTyparsAcc then 
                newSet, assumedTyparsAcc
            else 
                loop newSet assumedTyparsAcc

        let uneliminatedTycons, assumedTyparsActual = loop initialAssumedTycons initialAssumedTypars

        // OK, we're done, Record the results for the type variable which provide the support
        for tyconStamp in uneliminatedTycons do
            let tycon, _ = tab[tyconStamp] 
            for tp in tycon.Typars(tycon.Range) do
                if assumedTyparsActual.Contains(tp.Stamp) then 
                    tp.SetComparisonDependsOn true

        // Return the set of structural type definitions which support the relation
        uneliminatedTycons

    /// Infer 'equality' constraints from type definitions
    let InferSetOfTyconsSupportingEquatable (cenv: cenv) (denv: DisplayEnv) (tyconsWithStructuralTypes:(Tycon * _) list) =

        let g = cenv.g 
        let tab = tyconsWithStructuralTypes |> List.map (fun (tycon, c) -> tycon.Stamp, (tycon, c)) |> Map.ofList 

        // Initially, assume the equality relation is available for all structural type definitions 
        let initialAssumedTycons = 
            set [ for tycon, _ in tyconsWithStructuralTypes do 
                       if AugmentWithHashCompare.TyconIsCandidateForAugmentationWithEquals g tycon then 
                           yield tycon.Stamp ]
                           
        // Initially, don't assume that the equality relation is dependent on any type variables
        let initialAssumedTypars = Set.empty

        // Repeatedly eliminate structural type definitions whose structural component types no longer support 
        // equality. On the way add type variables which are support the equality relation
        let rec loop (assumedTycons: Set<Stamp>) (assumedTypars: Set<Stamp>) =
            let mutable assumedTyparsAcc = assumedTypars
            
            // Checks if a field type supports the 'equality' constraint based on the assumptions about the type constructors
            // and type parameters.
            let rec checkIfFieldTypeSupportsEquality (tycon: Tycon) (ty: TType) =
                match tryDestTyparTy g ty with
                | ValueSome tp ->
                    // Look for an explicit 'equality' constraint
                    if tp.Constraints |> List.exists (function TyparConstraint.SupportsEquality _ -> true | _ -> false) then 
                        true

                    // Within structural types, type parameters can be optimistically assumed to have equality
                    // We record the ones for which we have made this assumption.
                    elif tycon.Typars(tycon.Range) |> List.exists (fun tp2 -> typarRefEq tp tp2) then                     
                        assumedTyparsAcc <- assumedTyparsAcc.Add(tp.Stamp)
                        true
                    else
                        false
                | _ ->
                    match ty with 
                    | SpecialEquatableHeadType g tinst -> 
                        tinst |> List.forall (checkIfFieldTypeSupportsEquality tycon)
                    | SpecialNotEquatableHeadType g -> 
                        false
                    | _ -> 
                        // Check the basic requirement - any types except those eliminated
                        match ty with
                        | AppTy g (tcref, tinst) ->
                            (if initialAssumedTycons.Contains tcref.Stamp then 
                                assumedTycons.Contains tcref.Stamp
                             elif AugmentWithHashCompare.TyconIsCandidateForAugmentationWithEquals g tcref.Deref then
                                Option.isSome tcref.GeneratedHashAndEqualsWithComparerValues
                             else
                                true) 
                             &&
                             // Check it isn't ruled out by the user
                             not (HasFSharpAttribute g g.attrib_NoEqualityAttribute tcref.Attribs)
                             &&
                             // Check the structural dependencies
                             (tinst, tcref.TyparsNoRange) ||> List.lengthsEqAndForall2 (fun ty tp -> 
                                 if tp.EqualityConditionalOn || assumedTypars.Contains tp.Stamp then 
                                     checkIfFieldTypeSupportsEquality tycon ty 
                                 else 
                                     true) 
                        | _ ->
                            false

            let newSet = 
                assumedTycons |> Set.filter (fun tyconStamp -> 

                   let tycon, structuralTypes = tab[tyconStamp] 

                   if g.compilingFSharpCore && 
                      AugmentWithHashCompare.TyconIsCandidateForAugmentationWithEquals g tycon && 
                      not (HasFSharpAttribute g g.attrib_StructuralEqualityAttribute tycon.Attribs) && 
                      not (HasFSharpAttribute g g.attrib_NoEqualityAttribute tycon.Attribs) then 
                       errorR(Error(FSComp.SR.tcFSharpCoreRequiresExplicit(), tycon.Range)) 

                   // Remove structural types with incomparable elements from the assumedTycons
                   let res = (structuralTypes |> List.forall (fst >> checkIfFieldTypeSupportsEquality tycon))

                   // If the type was excluded, say why
                   if not res then 
                       match TryFindFSharpBoolAttribute g g.attrib_StructuralEqualityAttribute tycon.Attribs with
                       | Some true -> 
                           if AugmentWithHashCompare.TyconIsCandidateForAugmentationWithEquals g tycon then 
                               match structuralTypes |> List.tryFind (fst >> checkIfFieldTypeSupportsEquality tycon >> not) with
                               | None -> 
                                   assert false
                                   failwith "unreachable"
                               | Some (ty, _) -> 
                                   if isTyparTy g ty then 
                                       errorR(Error(FSComp.SR.tcStructuralEqualityNotSatisfied1(tycon.DisplayName, NicePrint.prettyStringOfTy denv ty), tycon.Range)) 
                                   else 
                                       errorR(Error(FSComp.SR.tcStructuralEqualityNotSatisfied2(tycon.DisplayName, NicePrint.prettyStringOfTy denv ty), tycon.Range)) 
                           else
                               ()
                       | Some false -> 
                           ()
                       | None -> 
                           if AugmentWithHashCompare.TyconIsCandidateForAugmentationWithEquals g tycon then 
                               match structuralTypes |> List.tryFind (fst >> checkIfFieldTypeSupportsEquality tycon >> not) with
                               | None -> 
                                   assert false
                                   failwith "unreachable"
                               | Some (ty, _) -> 
                                   if isTyparTy g ty then 
                                       warning(Error(FSComp.SR.tcNoEqualityNeeded1(tycon.DisplayName, NicePrint.prettyStringOfTy denv ty, tycon.DisplayName), tycon.Range)) 
                                   else 
                                       warning(Error(FSComp.SR.tcNoEqualityNeeded2(tycon.DisplayName, NicePrint.prettyStringOfTy denv ty, tycon.DisplayName), tycon.Range)) 

                                                      
                   res)

            if newSet = assumedTycons && assumedTypars = assumedTyparsAcc then 
                newSet, assumedTyparsAcc
            else 
                loop newSet assumedTyparsAcc

        let uneliminatedTycons, assumedTyparsActual = loop initialAssumedTycons initialAssumedTypars

        // OK, we're done, Record the results for the type variable which provide the support
        for tyconStamp in uneliminatedTycons do
            let tycon, _ = tab[tyconStamp] 
            for tp in tycon.Typars(tycon.Range) do
                if assumedTyparsActual.Contains(tp.Stamp) then 
                    tp.SetEqualityDependsOn true

        // Return the set of structural type definitions which support the relation
        uneliminatedTycons


//-------------------------------------------------------------------------
// Helpers for modules, types and exception declarations
//------------------------------------------------------------------------- 

let ComputeModuleName (longPath: Ident list) = 
    if longPath.Length <> 1 then error(Error(FSComp.SR.tcInvalidModuleName(), (List.head longPath).idRange))
    longPath.Head 

let CheckForDuplicateConcreteType env nm m = 
    let curr = GetCurrAccumulatedModuleOrNamespaceType env
    if Map.containsKey nm curr.AllEntitiesByCompiledAndLogicalMangledNames then 
        // Use 'error' instead of 'errorR' here to avoid cascading errors - see bug 1177 in FSharp 1.0 
        error (Duplicate(FSComp.SR.tcTypeExceptionOrModule(), nm, m))

let CheckForDuplicateModule env nm m = 
    let curr = GetCurrAccumulatedModuleOrNamespaceType env
    if curr.ModulesAndNamespacesByDemangledName.ContainsKey nm then 
        errorR (Duplicate(FSComp.SR.tcTypeOrModule(), nm, m))


//-------------------------------------------------------------------------
// Bind exception definitions
//------------------------------------------------------------------------- 

/// Check 'exception' declarations in implementations and signatures
module TcExceptionDeclarations = 

    let TcExnDefnCore_Phase1A cenv env parent (SynExceptionDefnRepr(Attributes synAttrs, SynUnionCase(ident= SynIdent(id,_)), _, xmlDoc, vis, m)) =
        let attrs = TcAttributes cenv env AttributeTargets.ExnDecl synAttrs
        if not (String.isLeadingIdentifierCharacterUpperCase id.idText) then errorR(NotUpperCaseConstructor m)
        let vis, cpath = ComputeAccessAndCompPath env None m vis None parent
        let vis = TcRecdUnionAndEnumDeclarations.CombineReprAccess parent vis
        CheckForDuplicateConcreteType env (id.idText + "Exception") id.idRange
        CheckForDuplicateConcreteType env id.idText id.idRange
        let repr = TExnFresh (Construct.MakeRecdFieldsTable [])
        let xmlDoc = xmlDoc.ToXmlDoc(true, Some [])
        Construct.NewExn cpath id vis repr attrs xmlDoc

    let TcExnDefnCore_Phase1G_EstablishRepresentation (cenv: cenv) (env: TcEnv) parent (exnc: Entity) (SynExceptionDefnRepr(_, SynUnionCase(caseType=args), reprIdOpt, _, _, m)) =
        let g = cenv.g 
        let args = match args with SynUnionCaseKind.Fields args -> args | _ -> error(Error(FSComp.SR.tcExplicitTypeSpecificationCannotBeUsedForExceptionConstructors(), m))
        let ad = env.AccessRights
        let id = exnc.Id
        
        let args' =
            args |> List.mapi (fun i (SynField (idOpt = idOpt) as fdef) ->
                match idOpt with
                | Some fieldId ->
                    let tcref = mkLocalTyconRef exnc
                    let thisTypInst, _ = generalizeTyconRef g tcref
                    let item = Item.RecdField (RecdFieldInfo (thisTypInst, RecdFieldRef (tcref, fieldId.idText)))
                    CallNameResolutionSink cenv.tcSink (fieldId.idRange, env.NameEnv, item, emptyTyparInst, ItemOccurence.Binding, env.AccessRights)
                | _ -> ()

                TcRecdUnionAndEnumDeclarations.TcAnonFieldDecl cenv env parent emptyUnscopedTyparEnv (mkExceptionFieldName i) fdef)
        TcRecdUnionAndEnumDeclarations.ValidateFieldNames(args, args')
        let repr = 
          match reprIdOpt with 
          | Some longId ->
              let resolution =
                  ResolveExprLongIdent cenv.tcSink cenv.nameResolver m ad env.NameEnv TypeNameResolutionInfo.Default longId 
                  |> ForceRaise
              match resolution with
              | _, Item.ExnCase exnc, [] -> 
                  CheckTyconAccessible cenv.amap m env.AccessRights exnc |> ignore
                  if not (isNil args') then 
                      errorR (Error(FSComp.SR.tcExceptionAbbreviationsShouldNotHaveArgumentList(), m))
                  TExnAbbrevRepr exnc
              | _, Item.CtorGroup(_, meths), [] -> 
                  // REVIEW: check this really is an exception type 
                  match args' with 
                  | [] -> ()
                  | _ -> error (Error(FSComp.SR.tcAbbreviationsFordotNetExceptionsCannotTakeArguments(), m))
                  let candidates = 
                      meths |> List.filter (fun minfo -> 
                          minfo.NumArgs = [args'.Length] &&
                          minfo.GenericArity = 0) 
                  match candidates with 
                  | [minfo] -> 
                      match minfo.ApparentEnclosingType with 
                      | AppTy g (tcref, _) as enclTy when TypeDefinitelySubsumesTypeNoCoercion 0 g cenv.amap m g.exn_ty enclTy ->
                          TExnAsmRepr tcref.CompiledRepresentationForNamedType
                      | _ -> 
                          error(Error(FSComp.SR.tcExceptionAbbreviationsMustReferToValidExceptions(), m))
                  | _ -> 
                      error (Error(FSComp.SR.tcAbbreviationsFordotNetExceptionsMustHaveMatchingObjectConstructor(), m))
              | _ ->
                  error (Error(FSComp.SR.tcNotAnException(), m))
          | None -> 
             TExnFresh (Construct.MakeRecdFieldsTable args')
        
        exnc.SetExceptionInfo repr 

        let item = Item.ExnCase(mkLocalTyconRef exnc)
        CallNameResolutionSink cenv.tcSink (id.idRange, env.NameEnv, item, emptyTyparInst, ItemOccurence.Binding, env.AccessRights)
        args'

    let private TcExnDefnCore (cenv: cenv) env parent synExnDefnRepr =
        let g = cenv.g
        let exnc = TcExnDefnCore_Phase1A cenv env parent synExnDefnRepr
        let args' = TcExnDefnCore_Phase1G_EstablishRepresentation cenv env parent exnc synExnDefnRepr
        exnc.TypeContents.tcaug_super <- Some g.exn_ty

        PublishTypeDefn cenv env exnc

        let structuralTypes = args' |> List.map (fun rf -> (rf.FormalType, rf.Range))
        let scSet = TyconConstraintInference.InferSetOfTyconsSupportingComparable cenv env.DisplayEnv [(exnc, structuralTypes)]
        let seSet = TyconConstraintInference.InferSetOfTyconsSupportingEquatable cenv env.DisplayEnv [(exnc, structuralTypes)] 

        // Augment the exception constructor with comparison and hash methods if needed 
        let binds = 
          match exnc.ExceptionInfo with 
          | TExnAbbrevRepr _ | TExnNone | TExnAsmRepr _ -> []
          | TExnFresh _ -> 
              AddAugmentationDeclarations.AddGenericHashAndComparisonDeclarations cenv env scSet seSet exnc
              AddAugmentationDeclarations.AddGenericHashAndComparisonBindings cenv exnc

        binds, exnc

    let TcExnDefn (cenv: cenv) envInitial parent (SynExceptionDefn(core, _, aug, m), scopem) = 
        let g = cenv.g
        let binds1, exnc = TcExnDefnCore cenv envInitial parent core
        let envMutRec = AddLocalExnDefnAndReport cenv.tcSink scopem (AddLocalTycons g cenv.amap scopem [exnc] envInitial) exnc 

        let defns = [MutRecShape.Tycon(MutRecDefnsPhase2DataForTycon(Some exnc, parent, ModuleOrMemberBinding, mkLocalEntityRef exnc, None, NoSafeInitInfo, [], aug, m, NoNewSlots, (fun () -> ())))]
        let binds2, envFinal = TcMutRecDefns_Phase2 cenv envInitial m scopem None envMutRec defns
        let binds2flat = binds2 |> MutRecShapes.collectTycons |> List.collect snd
        // Augment types with references to values that implement the pre-baked semantics of the type
        let binds3 = AddAugmentationDeclarations.AddGenericEqualityBindings cenv envFinal exnc
        binds1 @ binds2flat @ binds3, exnc, envFinal

    let TcExnSignature (cenv: cenv) envInitial parent tpenv (SynExceptionSig(exnRepr=core; members=aug), scopem) = 
        let g = cenv.g
        let binds, exnc = TcExnDefnCore cenv envInitial parent core
        let envMutRec = AddLocalExnDefnAndReport cenv.tcSink scopem (AddLocalTycons g cenv.amap scopem [exnc] envInitial) exnc 
        let ecref = mkLocalEntityRef exnc
        let vals, _ = TcTyconMemberSpecs cenv envMutRec (ContainerInfo(parent, Some(MemberOrValContainerInfo(ecref, None, None, NoSafeInitInfo, [])))) ModuleOrMemberBinding tpenv aug
        binds, vals, ecref, envMutRec



/// Bind type definitions
///
/// We first establish the cores of a set of type definitions (i.e. everything
/// about the type definitions that doesn't involve values or expressions)
///
/// This is a non-trivial multi-phase algorithm. The technique used
/// is to gradually "fill in" the fields of the type constructors. 
///
/// This use of mutation is very problematic. This has many dangers, 
/// since the process of filling in the fields
/// involves creating, traversing and analyzing types that may recursively
/// refer to the types being defined. However a functional version of this
/// would need to re-implement certain type relations to work over a 
/// partial representation of types.
module EstablishTypeDefinitionCores = 
 
    type TypeRealizationPass = 
        | FirstPass 
        | SecondPass 

    /// Compute the mangled name of a type definition. 'doErase' is true for all type definitions except type abbreviations.
    let private ComputeTyconName (longPath: Ident list, doErase: bool, typars: Typars) = 
        if longPath.Length <> 1 then error(Error(FSComp.SR.tcInvalidTypeExtension(), longPath.Head.idRange))
        let id = longPath.Head
        let erasedArity = 
            if doErase then typars |> Seq.sumBy (fun tp -> if tp.IsErased then 0 else 1) 
            else typars.Length
        mkSynId id.idRange (if erasedArity = 0 then id.idText else id.idText + "`" + string erasedArity)
 
    let private GetTyconAttribs g attrs = 
        let hasClassAttr = HasFSharpAttribute g g.attrib_ClassAttribute attrs
        let hasAbstractClassAttr = HasFSharpAttribute g g.attrib_AbstractClassAttribute attrs
        let hasInterfaceAttr = HasFSharpAttribute g g.attrib_InterfaceAttribute attrs
        let hasStructAttr = HasFSharpAttribute g g.attrib_StructAttribute attrs
        let hasMeasureAttr = HasFSharpAttribute g g.attrib_MeasureAttribute attrs
        (hasClassAttr, hasAbstractClassAttr, hasInterfaceAttr, hasStructAttr, hasMeasureAttr)

    //-------------------------------------------------------------------------
    // Type kind inference 
    //------------------------------------------------------------------------- 
       
    let private InferTyconKind g (kind, attrs, slotsigs, fields, inSig, isConcrete, m) =
        let hasClassAttr, hasAbstractClassAttr, hasInterfaceAttr, hasStructAttr, hasMeasureAttr = GetTyconAttribs g attrs
        let bi b = (if b then 1 else 0)
        if (bi hasClassAttr + bi hasInterfaceAttr + bi hasStructAttr + bi hasMeasureAttr) > 1 ||
           (bi hasAbstractClassAttr + bi hasInterfaceAttr + bi hasStructAttr + bi hasMeasureAttr) > 1 then
           error(Error(FSComp.SR.tcAttributesOfTypeSpecifyMultipleKindsForType(), m))
        
        match kind with 
        | SynTypeDefnKind.Unspecified ->
            if hasClassAttr || hasAbstractClassAttr || hasMeasureAttr then SynTypeDefnKind.Class        
            elif hasInterfaceAttr then SynTypeDefnKind.Interface
            elif hasStructAttr then SynTypeDefnKind.Struct
            elif isConcrete || not (isNil fields) then SynTypeDefnKind.Class
            elif isNil slotsigs && inSig then SynTypeDefnKind.Opaque
            else SynTypeDefnKind.Interface
        | k -> 
            if hasClassAttr && not (match k with SynTypeDefnKind.Class -> true | _ -> false) || 
               hasMeasureAttr && not (match k with SynTypeDefnKind.Class | SynTypeDefnKind.Abbrev | SynTypeDefnKind.Opaque -> true | _ -> false) || 
               hasInterfaceAttr && not (match k with SynTypeDefnKind.Interface -> true | _ -> false) || 
               hasStructAttr && not (match k with SynTypeDefnKind.Struct | SynTypeDefnKind.Record | SynTypeDefnKind.Union -> true | _ -> false) then 
                error(Error(FSComp.SR.tcKindOfTypeSpecifiedDoesNotMatchDefinition(), m))
            k

    let private (|TyconCoreAbbrevThatIsReallyAUnion|_|) (hasMeasureAttr, envinner: TcEnv, id: Ident) synTyconRepr =
        match synTyconRepr with 
        | SynTypeDefnSimpleRepr.TypeAbbrev(_, StripParenTypes (SynType.LongIdent(SynLongIdent([unionCaseName], _, _))), m) 
                              when 
                                (not hasMeasureAttr && 
                                 (isNil (LookupTypeNameInEnvNoArity OpenQualified unionCaseName.idText envinner.NameEnv) || 
                                  id.idText = unionCaseName.idText)) -> 
            Some(unionCaseName, m)
        | _ -> 
            None

    /// Get the component types that make a record, union or struct type.
    ///
    /// Used when determining if a structural type supports structural comparison.
    let private GetStructuralElementsOfTyconDefn (cenv: cenv) env tpenv (MutRecDefnsPhase1DataForTycon(_, synTyconRepr, _, _, _, _)) tycon = 
        let thisTyconRef = mkLocalTyconRef tycon
        let g = cenv.g
        let m = tycon.Range
        let env = AddDeclaredTypars CheckForDuplicateTypars (tycon.Typars m) env
        let env = MakeInnerEnvForTyconRef env thisTyconRef false 
        [ match synTyconRepr with 
          | SynTypeDefnSimpleRepr.None _ -> ()
          | SynTypeDefnSimpleRepr.Union (_, unionCases, _) -> 

              for SynUnionCase (caseType=args; range=m) in unionCases do 
                match args with
                | SynUnionCaseKind.Fields flds -> 
                    for SynField(_, _, _, ty, _, _, _, m) in flds do 
                        let tyR, _ = TcTypeAndRecover cenv NoNewTypars NoCheckCxs ItemOccurence.UseInType env tpenv ty
                        yield (tyR, m)

                | SynUnionCaseKind.FullType (ty, arity) -> 
                    let tyR, _ = TcTypeAndRecover cenv NoNewTypars NoCheckCxs ItemOccurence.UseInType env tpenv ty
                    let curriedArgTys, _ = GetTopTauTypeInFSharpForm g (arity |> TranslateSynValInfo m (TcAttributes cenv env) |> TranslatePartialValReprInfo []).ArgInfos tyR m

                    if curriedArgTys.Length > 1 then 
                        errorR(Error(FSComp.SR.tcIllegalFormForExplicitTypeDeclaration(), m))   

                    for argTys in curriedArgTys do
                        for argTy, _ in argTys do
                            yield (argTy, m)

          | SynTypeDefnSimpleRepr.General (_, _, _, fields, _, _, implicitCtorSynPats, _) when tycon.IsFSharpStructOrEnumTycon -> // for structs
              for SynField(_, isStatic, _, ty, _, _, _, m) in fields do 
                  if not isStatic then 
                      let tyR, _ = TcTypeAndRecover cenv NoNewTypars NoCheckCxs ItemOccurence.UseInType env tpenv ty
                      yield (tyR, m)

              match implicitCtorSynPats with
              | None -> ()
              | Some spats -> 
                  let ctorArgNames, patEnv = TcSimplePatsOfUnknownType cenv true NoCheckCxs env tpenv spats

                  let (TcPatLinearEnv(_, names, _)) = patEnv

                  for arg in ctorArgNames do
                      let ty = names[arg].Type
                      let m = names[arg].Ident.idRange
                      if not (isNil (ListSet.subtract typarEq (freeInTypeLeftToRight g false ty) tycon.TyparsNoRange)) then
                          errorR(Error(FSComp.SR.tcStructsMustDeclareTypesOfImplicitCtorArgsExplicitly(), m))   
                      yield (ty, m)

          | SynTypeDefnSimpleRepr.Record (_, fields, _) -> 
              for SynField(_, _, _, ty, _, _, _, m) in fields do 
                  let tyR, _ = TcTypeAndRecover cenv NoNewTypars NoCheckCxs ItemOccurence.UseInType env tpenv ty
                  yield (tyR, m)

          | _ ->
              () ]

    let ComputeModuleOrNamespaceKind g isModule typeNames attribs nm = 
        if not isModule then (Namespace true)
        elif ModuleNameIsMangled g attribs || Set.contains nm typeNames then FSharpModuleWithSuffix 
        else ModuleOrType

    let AdjustModuleName moduleKind nm =
        match moduleKind with
        | FSharpModuleWithSuffix -> nm+FSharpModuleSuffix
        | _ -> nm

    let InstanceMembersNeedSafeInitCheck (cenv: cenv) m thisTy = 
        let g = cenv.g
        ExistsInEntireHierarchyOfType 
            (fun ty -> not (isStructTy g ty) && (match tryTcrefOfAppTy g ty with ValueSome tcref when tcref.HasSelfReferentialConstructor -> true | _ -> false))
            g 
            cenv.amap
            m 
            AllowMultiIntfInstantiations.Yes
            thisTy
        
    // Make the "delayed reference" boolean value recording the safe initialization of a type in a hierarchy where there is a HasSelfReferentialConstructor
    let ComputeInstanceSafeInitInfo (cenv: cenv) env m thisTy = 
        let g = cenv.g
        if InstanceMembersNeedSafeInitCheck cenv m thisTy then 
            let rfield = MakeSafeInitField g env m false
            let tcref = tcrefOfAppTy g thisTy
            SafeInitField (mkRecdFieldRef tcref rfield.LogicalName, rfield)
        else
            NoSafeInitInfo

    // We suppress all error reporting for this pre-analysis of the attributes,
    // in case some attributes involve constructs not already in scope - we are
    // only looking for the 'Measure' attributes, then we discard the generated type parameters
    let TyparsAllHaveMeasureDeclEarlyCheck cenv env (TyparDecls synTypars) =
        suppressErrorReporting (fun () -> 
            synTypars|> List.forall (fun synTypar ->
                try
                    let (SynTyparDecl(Attributes synAttrs, _)) = synTypar
                    let attrs = TcAttributes cenv env AttributeTargets.GenericParameter synAttrs
                    HasFSharpAttribute cenv.g cenv.g.attrib_MeasureAttribute attrs
                with _ -> false))

    let TypeNamesInMutRecDecls cenv env (compDecls: MutRecShapes<MutRecDefnsPhase1DataForTycon * 'MemberInfo, 'LetInfo, SynComponentInfo>) =
        [ for d in compDecls do 
                match d with 
                | MutRecShape.Tycon (MutRecDefnsPhase1DataForTycon(SynComponentInfo(typeParams=synTypars; longId=ids), _, _, _, _, isAtOriginalTyconDefn), _) -> 
                    if isAtOriginalTyconDefn && TyparsAllHaveMeasureDeclEarlyCheck cenv env synTypars then 
                        yield (List.last ids).idText
                | _ -> () ]
         |> set

    let TypeNamesInNonMutRecDecls cenv env defs =
            [ for def in defs do 
                match def with 
                | SynModuleDecl.Types (typeSpecs, _) -> 
                    for SynTypeDefn(typeInfo=SynComponentInfo(typeParams=synTypars; longId=ids); typeRepr=trepr) in typeSpecs do 
                        if TyparsAllHaveMeasureDeclEarlyCheck cenv env synTypars then
                            match trepr with 
                            | SynTypeDefnRepr.ObjectModel(kind=SynTypeDefnKind.Augmentation _) -> ()
                            | _ -> yield (List.last ids).idText
                | _ -> () ]
            |> set

        // Collect the type names so we can implicitly add the compilation suffix to module names
    let TypeNamesInNonMutRecSigDecls defs =
            [ for def in defs do 
               match def with 
               | SynModuleSigDecl.Types (typeSpecs, _) -> 
                  for SynTypeDefnSig(typeInfo=SynComponentInfo(typeParams=TyparDecls typars; longId=ids); typeRepr=trepr; members=extraMembers) in typeSpecs do 
                      if isNil typars then
                          match trepr with 
                          | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.None _, _) when not (isNil extraMembers) -> ()
                          | _ -> yield (List.last ids).idText
               | _ -> () ]
            |> set

    let TcTyconDefnCore_Phase1A_BuildInitialModule (cenv: cenv) envInitial parent typeNames compInfo decls =
        let g = cenv.g
        let (SynComponentInfo(Attributes attribs, _, _, longPath, xml, _, vis, im)) = compInfo 
        let id = ComputeModuleName longPath
        let modAttrs = TcAttributes cenv envInitial AttributeTargets.ModuleDecl attribs 
        let moduleKind = ComputeModuleOrNamespaceKind g true typeNames modAttrs id.idText
        let modName = AdjustModuleName moduleKind id.idText

        let vis, _ = ComputeAccessAndCompPath envInitial None id.idRange vis None parent
             
        CheckForDuplicateModule envInitial id.idText id.idRange
        let id = ident (modName, id.idRange)
        CheckForDuplicateConcreteType envInitial id.idText im
        CheckNamespaceModuleOrTypeName g id

        let envForDecls, moduleTyAcc = MakeInnerEnv true envInitial id moduleKind    
        let moduleTy = Construct.NewEmptyModuleOrNamespaceType moduleKind
        let xmlDoc = xml.ToXmlDoc(true, Some [])
        let moduleEntity = Construct.NewModuleOrNamespace (Some envInitial.eCompPath) vis id xmlDoc modAttrs (MaybeLazy.Strict moduleTy)
        let innerParent = Parent (mkLocalModuleRef moduleEntity)
        let innerTypeNames = TypeNamesInMutRecDecls cenv envForDecls decls
        MutRecDefnsPhase2DataForModule (moduleTyAcc, moduleEntity), (innerParent, innerTypeNames, envForDecls)

    /// Establish 'type <vis1> C < T1... TN > = <vis2> ...' including 
    ///    - computing the mangled name for C
    /// but 
    ///    - we don't yet 'properly' establish constraints on type parameters
    let private TcTyconDefnCore_Phase1A_BuildInitialTycon (cenv: cenv) env parent (MutRecDefnsPhase1DataForTycon(synTyconInfo, synTyconRepr, _, preEstablishedHasDefaultCtor, hasSelfReferentialCtor, _)) = 
        let g = cenv.g
        let (SynComponentInfo (_, TyparDecls synTypars, _, id, xmlDoc, preferPostfix, synVis, _)) = synTyconInfo
        let checkedTypars = TcTyparDecls cenv env synTypars
        id |> List.iter (CheckNamespaceModuleOrTypeName g)

        match synTyconRepr with 
        | SynTypeDefnSimpleRepr.Exception synExnDefnRepr -> 
          TcExceptionDeclarations.TcExnDefnCore_Phase1A cenv env parent synExnDefnRepr
        | _ ->
        let id = ComputeTyconName (id, (match synTyconRepr with SynTypeDefnSimpleRepr.TypeAbbrev _ -> false | _ -> true), checkedTypars)

        // Augmentations of type definitions are allowed within the same file as long as no new type representation or abbreviation is given 
        CheckForDuplicateConcreteType env id.idText id.idRange
        let vis, cpath = ComputeAccessAndCompPath env None id.idRange synVis None parent

        // Establish the visibility of the representation, e.g.
        //   type R = 
        //      private { f: int }
        //      member x.P = x.f + x.f
        let synVisOfRepr = 
            match synTyconRepr with 
            | SynTypeDefnSimpleRepr.None _ -> None
            | SynTypeDefnSimpleRepr.TypeAbbrev _ -> None
            | SynTypeDefnSimpleRepr.Union (vis, _, _) -> vis
            | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _ -> None
            | SynTypeDefnSimpleRepr.Record (vis, _, _) -> vis
            | SynTypeDefnSimpleRepr.General _ -> None
            | SynTypeDefnSimpleRepr.Enum _ -> None
            | SynTypeDefnSimpleRepr.Exception _ -> None
         
        let visOfRepr, _ = ComputeAccessAndCompPath env None id.idRange synVisOfRepr None parent
        let visOfRepr = combineAccess vis visOfRepr 
        // If we supported nested types and modules then additions would be needed here
        let lmodTy = MaybeLazy.Strict (Construct.NewEmptyModuleOrNamespaceType ModuleOrType)

        // '<param>' documentation is allowed for delegates
        let paramNames =
            match synTyconRepr with 
            | SynTypeDefnSimpleRepr.General (SynTypeDefnKind.Delegate (_ty, arity), _, _, _, _, _, _, _) -> arity.ArgNames
            | SynTypeDefnSimpleRepr.General (SynTypeDefnKind.Unspecified, _, _, _, _, _, Some synPats, _) ->
                let rec patName (p: SynSimplePat) =
                    match p with
                    | SynSimplePat.Id (id, _, _, _, _, _) -> id.idText
                    | SynSimplePat.Typed(pat, _, _) -> patName pat
                    | SynSimplePat.Attrib(pat, _, _) -> patName pat

                let rec pats (p: SynSimplePats) =
                    match p with
                    | SynSimplePats.SimplePats (ps, _) -> ps
                    | SynSimplePats.Typed (ps, _, _) -> pats ps

                let patNames =
                    pats synPats
                    |> List.map patName

                patNames
            | _ -> []
        let xmlDoc = xmlDoc.ToXmlDoc(true, Some paramNames )
        Construct.NewTycon
            (cpath, id.idText, id.idRange, vis, visOfRepr, TyparKind.Type, LazyWithContext.NotLazy checkedTypars,
             xmlDoc, preferPostfix, preEstablishedHasDefaultCtor, hasSelfReferentialCtor, lmodTy)

    //-------------------------------------------------------------------------
    /// Establishing type definitions: early phase: work out the basic kind of the type definition
    ///
    ///    On entry: the Tycon for the type definition has been created but many of its fields are not
    ///              yet filled in.
    ///    On exit: the entity_tycon_repr field of the tycon has been filled in with a dummy value that
    ///             indicates the kind of the type constructor
    /// Also, some adhoc checks are made.
    ///
    ///  synTyconInfo: Syntactic AST for the name, attributes etc. of the type constructor
    ///  synTyconRepr: Syntactic AST for the RHS of the type definition
    let private TcTyconDefnCore_Phase1B_EstablishBasicKind (cenv: cenv) inSig envinner (MutRecDefnsPhase1DataForTycon(synTyconInfo, synTyconRepr, _, _, _, _)) (tycon: Tycon) = 
        let g = cenv.g
        let (SynComponentInfo(Attributes synAttrs, TyparDecls typars, _, _, _, _, _, _)) = synTyconInfo
        let m = tycon.Range
        let id = tycon.Id

        // 'Check' the attributes. We return the results to avoid having to re-check them in all other phases. 
        // Allow failure of constructor resolution because Vals for members in the same recursive group are not yet available
        let attrs, getFinalAttrs = TcAttributesCanFail cenv envinner AttributeTargets.TyconDecl synAttrs
        let hasMeasureAttr = HasFSharpAttribute g g.attrib_MeasureAttribute attrs

        let isStructRecordOrUnionType = 
            match synTyconRepr with
            | SynTypeDefnSimpleRepr.Record _ 
            | TyconCoreAbbrevThatIsReallyAUnion (hasMeasureAttr, envinner, id) _
            | SynTypeDefnSimpleRepr.Union _ -> 
                HasFSharpAttribute g g.attrib_StructAttribute attrs
            | _ -> 
                false

        tycon.SetIsStructRecordOrUnion isStructRecordOrUnionType

        // Set the compiled name, if any
        tycon.SetCompiledName (TryFindFSharpStringAttribute g g.attrib_CompiledNameAttribute attrs)

        if hasMeasureAttr then 
            tycon.SetTypeOrMeasureKind TyparKind.Measure
            if not (isNil typars) then error(Error(FSComp.SR.tcMeasureDefinitionsCannotHaveTypeParameters(), m))

        let repr = 
            match synTyconRepr with 
            | SynTypeDefnSimpleRepr.Exception _ -> TNoRepr
            | SynTypeDefnSimpleRepr.None m -> 
                // Run InferTyconKind to raise errors on inconsistent attribute sets
                InferTyconKind g (SynTypeDefnKind.Opaque, attrs, [], [], inSig, true, m) |> ignore
                if not inSig && not hasMeasureAttr then 
                    errorR(Error(FSComp.SR.tcTypeRequiresDefinition(), m))
                if hasMeasureAttr then 
                    TFSharpObjectRepr { fsobjmodel_kind = TFSharpClass 
                                        fsobjmodel_vslots = []
                                        fsobjmodel_rfields = Construct.MakeRecdFieldsTable [] }
                else 
                    TNoRepr

            | TyconCoreAbbrevThatIsReallyAUnion (hasMeasureAttr, envinner, id) (_, m)
            | SynTypeDefnSimpleRepr.Union (_, _, m) -> 

                // Run InferTyconKind to raise errors on inconsistent attribute sets
                InferTyconKind g (SynTypeDefnKind.Union, attrs, [], [], inSig, true, m) |> ignore

                // Note: the table of union cases is initially empty
                Construct.MakeUnionRepr []

            | SynTypeDefnSimpleRepr.TypeAbbrev _ -> 
                // Run InferTyconKind to raise errors on inconsistent attribute sets
                InferTyconKind g (SynTypeDefnKind.Abbrev, attrs, [], [], inSig, true, m) |> ignore
                TNoRepr

            | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly (s, m) -> 
                let s = (s :?> ILType)
                // Run InferTyconKind to raise errors on inconsistent attribute sets
                InferTyconKind g (SynTypeDefnKind.IL, attrs, [], [], inSig, true, m) |> ignore
                TAsmRepr s

            | SynTypeDefnSimpleRepr.Record (_, _, m) -> 

                // Run InferTyconKind to raise errors on inconsistent attribute sets
                InferTyconKind g (SynTypeDefnKind.Record, attrs, [], [], inSig, true, m) |> ignore

                // Note: the table of record fields is initially empty
                TFSharpRecdRepr (Construct.MakeRecdFieldsTable [])

            | SynTypeDefnSimpleRepr.General (kind, _, slotsigs, fields, isConcrete, _, _, _) ->
                let kind = InferTyconKind g (kind, attrs, slotsigs, fields, inSig, isConcrete, m)
                match kind with 
                | SynTypeDefnKind.Opaque -> 
                    TNoRepr
                | _ -> 
                    let kind = 
                        match kind with
                        | SynTypeDefnKind.Class -> TFSharpClass
                        | SynTypeDefnKind.Interface -> TFSharpInterface
                        | SynTypeDefnKind.Delegate _ -> TFSharpDelegate (MakeSlotSig("Invoke", g.unit_ty, [], [], [], None))
                        | SynTypeDefnKind.Struct -> TFSharpStruct 
                        | _ -> error(InternalError("should have inferred tycon kind", m))

                    let repr =
                        { fsobjmodel_kind = kind 
                          fsobjmodel_vslots = []
                          fsobjmodel_rfields = Construct.MakeRecdFieldsTable [] }

                    TFSharpObjectRepr repr

            | SynTypeDefnSimpleRepr.Enum _ -> 
                let kind = TFSharpEnum
                let repr =
                    { fsobjmodel_kind = kind 
                      fsobjmodel_vslots = []
                      fsobjmodel_rfields = Construct.MakeRecdFieldsTable [] }

                TFSharpObjectRepr repr

        // OK, now fill in the (partially computed) type representation
        tycon.entity_tycon_repr <- repr
        attrs, getFinalAttrs

#if !NO_TYPEPROVIDERS
    /// Get the items on the r.h.s. of a 'type X = ABC<...>' definition
    let private TcTyconDefnCore_GetGenerateDeclaration_Rhs (StripParenTypes rhsType) =
        match rhsType with 
        | SynType.App (StripParenTypes (SynType.LongIdent(SynLongIdent(tc, _, _))), _, args, _commas, _, _postfix, m) -> Some(tc, args, m)
        | SynType.LongIdent (SynLongIdent(tc, _, _) as lidwd) -> Some(tc, [], lidwd.Range)
        | SynType.LongIdentApp (StripParenTypes (SynType.LongIdent (SynLongIdent(tc, _, _))), SynLongIdent(longId, _, _), _, args, _commas, _, m) -> Some(tc@longId, args, m)
        | _ -> None

    /// Check whether 'type X = ABC<...>' is a generative provided type definition
    let private TcTyconDefnCore_TryAsGenerateDeclaration (cenv: cenv) (envinner: TcEnv) tpenv (tycon: Tycon, rhsType) =

        let tcref = mkLocalTyconRef tycon
        match TcTyconDefnCore_GetGenerateDeclaration_Rhs rhsType with 
        | None -> None
        | Some (tc, args, m) -> 
            let ad = envinner.AccessRights
            match ResolveTypeLongIdent cenv.tcSink cenv.nameResolver ItemOccurence.UseInType OpenQualified envinner.NameEnv ad tc TypeNameResolutionStaticArgsInfo.DefiniteEmpty PermitDirectReferenceToGeneratedType.Yes with
            | Result (_, tcrefBeforeStaticArguments) when 
                  tcrefBeforeStaticArguments.IsProvided && 
                  not tcrefBeforeStaticArguments.IsErased -> 

                    let typeBeforeArguments = 
                        match tcrefBeforeStaticArguments.TypeReprInfo with 
                        | TProvidedTypeRepr info -> info.ProvidedType
                        | _ -> failwith "unreachable"

                    if IsGeneratedTypeDirectReference (typeBeforeArguments, m) then 
                        let generatedTypePath = tcref.CompilationPath.MangledPath @ [ tcref.LogicalName ]
                        let _hasNoArgs, providedTypeAfterStaticArguments, checkTypeName = TcProvidedTypeAppToStaticConstantArgs cenv envinner (Some generatedTypePath) tpenv tcrefBeforeStaticArguments args m
                        let isGenerated = providedTypeAfterStaticArguments.PUntaint((fun st -> not st.IsErased), m)
                        if isGenerated then 
                           Some (tcrefBeforeStaticArguments, providedTypeAfterStaticArguments, checkTypeName, args, m)
                        else
                           None  // The provided type (after ApplyStaticArguments) must also be marked 'IsErased=false' 
                    else 
                        // This must be a direct reference to a generated type, otherwise it is a type abbreviation
                        None
            | _ -> 
                None


    /// Check and establish a 'type X = ABC<...>' provided type definition
    let private TcTyconDefnCore_Phase1C_EstablishDeclarationForGeneratedSetOfTypes (cenv: cenv) inSig (tycon: Tycon, rhsType: SynType, tcrefForContainer: TyconRef, theRootType: Tainted<ProvidedType>, checkTypeName, args, m) =
        let g = cenv.g

        // Explanation: We are definitely on the compilation thread here, we just have not propagated the token this far.
        let ctok = AssumeCompilationThreadWithoutEvidence()

        let tcref = mkLocalTyconRef tycon
        try 
            let resolutionEnvironment =
                if not (isNil args) then 
                   checkTypeName()
                let resolutionEnvironment = 
                    match tcrefForContainer.TypeReprInfo with 
                    | TProvidedTypeRepr info -> info.ResolutionEnvironment
                    | _ -> failwith "unreachable"
                resolutionEnvironment

            // Build up a mapping from System.Type --> TyconRef/ILTypeRef, to allow reverse-mapping
            // of types

            let previousContext = (theRootType.PApply ((fun x -> x.Context), m)).PUntaint (id, m)
            let lookupILTypeRef, lookupTyconRef = previousContext.GetDictionaries()
                    
            let ctxt = ProvidedTypeContext.Create(lookupILTypeRef, lookupTyconRef)

            // Create a new provided type which captures the reverse-remapping tables.
            let theRootTypeWithRemapping = theRootType.PApply ((fun x -> ProvidedType.ApplyContext(x, ctxt)), m)

            let isRootGenerated, rootProvAssemStaticLinkInfoOpt = 
                let stRootAssembly = theRootTypeWithRemapping.PApply((fun st -> st.Assembly), m)

                cenv.amap.assemblyLoader.GetProvidedAssemblyInfo (ctok, m, stRootAssembly)

            let isRootGenerated = isRootGenerated || theRootTypeWithRemapping.PUntaint((fun st -> not st.IsErased), m)

            if not isRootGenerated then 
                let desig = theRootTypeWithRemapping.TypeProviderDesignation
                let nm = theRootTypeWithRemapping.PUntaint((fun st -> st.FullName), m)
                error(Error(FSComp.SR.etErasedTypeUsedInGeneration(desig, nm), m))

            cenv.createsGeneratedProvidedTypes <- true

            // In compiled code, all types in the set of generated types end up being both generated and relocated, unless relocation is suppressed
            let isForcedSuppressRelocate = theRootTypeWithRemapping.PUntaint((fun st -> st.IsSuppressRelocate), m) 
            if isForcedSuppressRelocate && canAccessFromEverywhere tycon.Accessibility && not cenv.isScript then 
                errorR(Error(FSComp.SR.tcGeneratedTypesShouldBeInternalOrPrivate(), tcref.Range))

            let isSuppressRelocate = g.isInteractive || isForcedSuppressRelocate
    
            // Adjust the representation of the container type
            let repr =
                Construct.NewProvidedTyconRepr(resolutionEnvironment, theRootTypeWithRemapping, 
                                               Import.ImportProvidedType cenv.amap m, 
                                               isSuppressRelocate, m)
            tycon.entity_tycon_repr <- repr
            // Record the details so we can map System.Type --> TyconRef
            let ilOrigRootTypeRef = GetOriginalILTypeRefOfProvidedType (theRootTypeWithRemapping, m)
            theRootTypeWithRemapping.PUntaint ((fun st -> ignore(lookupTyconRef.TryRemove(st)) ; ignore(lookupTyconRef.TryAdd(st, tcref))), m)

            // Record the details so we can map System.Type --> ILTypeRef, including the relocation if any
            if not isSuppressRelocate then 
                let ilTgtRootTyRef = tycon.CompiledRepresentationForNamedType
                theRootTypeWithRemapping.PUntaint ((fun st -> ignore(lookupILTypeRef.TryRemove(st)) ; ignore(lookupILTypeRef.TryAdd(st, ilTgtRootTyRef))), m)

            // Iterate all nested types and force their embedding, to populate the mapping from System.Type --> TyconRef/ILTypeRef.
            // This is only needed for generated types, because for other types the System.Type objects self-describe
            // their corresponding F# type.
            let rec doNestedType (eref: EntityRef) (st: Tainted<ProvidedType>) = 

                // Check the type is a generated type
                let isGenerated, provAssemStaticLinkInfoOpt = 
                    let stAssembly = st.PApply((fun st -> st.Assembly), m)
                    cenv.amap.assemblyLoader.GetProvidedAssemblyInfo (ctok, m, stAssembly)

                let isGenerated = isGenerated || st.PUntaint((fun st -> not st.IsErased), m)

                if not isGenerated then 
                    let desig = st.TypeProviderDesignation
                    let nm = st.PUntaint((fun st -> st.FullName), m)
                    error(Error(FSComp.SR.etErasedTypeUsedInGeneration(desig, nm), m))

                // Embed the type into the module we're compiling
                let cpath = eref.CompilationPath.NestedCompPath eref.LogicalName ModuleOrNamespaceKind.ModuleOrType
                let access = combineAccess tycon.Accessibility (if st.PUntaint((fun st -> st.IsPublic || st.IsNestedPublic), m) then taccessPublic else taccessPrivate cpath)

                let nestedTycon = Construct.NewProvidedTycon(resolutionEnvironment, st, 
                                                             Import.ImportProvidedType cenv.amap m, 
                                                             isSuppressRelocate, 
                                                             m=m, cpath=cpath, access=access)
                eref.ModuleOrNamespaceType.AddProvidedTypeEntity nestedTycon

                let nestedTyRef = eref.NestedTyconRef nestedTycon
                let ilOrigTypeRef = GetOriginalILTypeRefOfProvidedType (st, m)
                                
                // Record the details so we can map System.Type --> TyconRef
                st.PUntaint ((fun st -> ignore(lookupTyconRef.TryRemove(st)) ; ignore(lookupTyconRef.TryAdd(st, nestedTyRef))), m)

                if isGenerated then 
                    let ilTgtTyRef = nestedTycon.CompiledRepresentationForNamedType
                    // Record the details so we can map System.Type --> ILTypeRef
                    st.PUntaint ((fun st -> ignore(lookupILTypeRef.TryRemove(st)) ; ignore(lookupILTypeRef.TryAdd(st, ilTgtTyRef))), m)

                    // Record the details so we can build correct ILTypeDefs during static linking rewriting
                    if not isSuppressRelocate then 
                        match provAssemStaticLinkInfoOpt with 
                        | Some provAssemStaticLinkInfo -> provAssemStaticLinkInfo.ILTypeMap[ilOrigTypeRef] <- ilTgtTyRef
                        | None -> ()
                       
                    ProviderGeneratedType(ilOrigTypeRef, ilTgtTyRef, doNestedTypes nestedTyRef st)
                else
                    ProviderGeneratedType(ilOrigTypeRef, ilOrigTypeRef, doNestedTypes nestedTyRef st)


                //System.Diagnostics.Debug.Assert eref.TryDeref.IsSome

            and doNestedTypes (eref: EntityRef) (st: Tainted<ProvidedType>) =
                st.PApplyArray((fun st -> st.GetAllNestedTypes()), "GetAllNestedTypes", m)
                |> Array.map (doNestedType eref)
                |> Array.toList

            let nested = doNestedTypes tcref theRootTypeWithRemapping 
            if not isSuppressRelocate then 

                let ilTgtRootTyRef = tycon.CompiledRepresentationForNamedType
                match rootProvAssemStaticLinkInfoOpt with 
                | Some provAssemStaticLinkInfo -> provAssemStaticLinkInfo.ILTypeMap[ilOrigRootTypeRef] <- ilTgtRootTyRef
                | None -> ()

                if not inSig then 
                    cenv.amap.assemblyLoader.RecordGeneratedTypeRoot (ProviderGeneratedType(ilOrigRootTypeRef, ilTgtRootTyRef, nested))

        with exn -> 
            errorRecovery exn rhsType.Range 
#endif

    /// Establish any type abbreviations
    ///
    /// e.g. for  
    ///    type B<'a when 'a: C> = DDD of C
    ///    and C = B<int>
    ///
    /// we establish
    ///
    ///   Entity('B) 
    ///       TypeAbbrev = TType_app(Entity('int'), [])
    ///
    /// and for
    ///
    ///    type C = B
    ///
    /// we establish
    ///       TypeAbbrev = TType_app(Entity('B'), [])
    ///
    /// Note that for 
    ///              type PairOfInts = int * int
    /// then after running this phase and checking for cycles, operations 
    /// such as 'isRefTupleTy' will return reliable results, e.g. isRefTupleTy on the
    /// TAST type for 'PairOfInts' will report 'true' 
    //
    let private TcTyconDefnCore_Phase1C_Phase1E_EstablishAbbreviations (cenv: cenv) envinner inSig tpenv pass (MutRecDefnsPhase1DataForTycon(_, synTyconRepr, _, _, _, _)) (tycon: Tycon) (attrs: Attribs) =
        let g = cenv.g
        let m = tycon.Range
        let checkConstraints = if (pass = SecondPass) then CheckCxs else NoCheckCxs
        let firstPass = (pass = FirstPass)
        try 
            let id = tycon.Id
            let thisTyconRef = mkLocalTyconRef tycon

            let hasMeasureAttr = HasFSharpAttribute g g.attrib_MeasureAttribute attrs
            let hasMeasureableAttr = HasFSharpAttribute g g.attrib_MeasureableAttribute attrs
            let envinner = AddDeclaredTypars CheckForDuplicateTypars (tycon.Typars m) envinner
            let envinner = MakeInnerEnvForTyconRef envinner thisTyconRef false 

            match synTyconRepr with 

            // This unfortunate case deals with "type x = A" 
            // In F# this only defines a new type if A is not in scope 
            // as a type constructor, or if the form type A = A is used. 
            // "type x = | A" can always be used instead. 
            | TyconCoreAbbrevThatIsReallyAUnion (hasMeasureAttr, envinner, id) _ -> ()
            
            | SynTypeDefnSimpleRepr.TypeAbbrev(ParserDetail.Ok, rhsType, m) ->

#if !NO_TYPEPROVIDERS
              // Check we have not already decided that this is a generative provided type definition. If we have already done this (i.e. this is the second pass
              // for a generative provided type definition, then there is no more work to do).
              if (match tycon.entity_tycon_repr with TNoRepr -> true | _ -> false) then 

                // Determine if this is a generative type definition.
                match TcTyconDefnCore_TryAsGenerateDeclaration cenv envinner tpenv (tycon, rhsType) with 
                | Some (tcrefForContainer, providedTypeAfterStaticArguments, checkTypeName, args, m) ->
                   // If this is a generative provided type definition then establish the provided type and all its nested types. Only do this on the first pass.
                   if firstPass then 
                       TcTyconDefnCore_Phase1C_EstablishDeclarationForGeneratedSetOfTypes cenv inSig (tycon, rhsType, tcrefForContainer, providedTypeAfterStaticArguments, checkTypeName, args, m)
                | None -> 
#else
                  ignore inSig 
#endif

                  // This case deals with ordinary type and measure abbreviations 
                  if not hasMeasureableAttr then 
                    let kind = if hasMeasureAttr then TyparKind.Measure else TyparKind.Type
                    let ty, _ = TcTypeOrMeasureAndRecover (Some kind) cenv NoNewTypars checkConstraints ItemOccurence.UseInType envinner tpenv rhsType

                    if not firstPass then 
                        let ftyvs = freeInTypeLeftToRight g false ty 
                        let typars = tycon.Typars m
                        if ftyvs.Length <> typars.Length then 
                            errorR(Deprecated(FSComp.SR.tcTypeAbbreviationHasTypeParametersMissingOnType(), tycon.Range))

                    if firstPass then
                        tycon.SetTypeAbbrev (Some ty)

            | _ -> ()
        
        with exn -> 
            errorRecovery exn m

    // Third phase: check and publish the super types. Run twice, once before constraints are established
    // and once after
    let private TcTyconDefnCore_Phase1D_Phase1F_EstablishSuperTypesAndInterfaceTypes (cenv: cenv) tpenv inSig pass (envMutRec, mutRecDefns: MutRecShape<_ * (Tycon * (Attribs * _)) option, _, _> list) = 
        let g = cenv.g
        let checkConstraints = if (pass = SecondPass) then CheckCxs else NoCheckCxs
        let firstPass = (pass = FirstPass)

        // Publish the immediately declared interfaces. 
        let tyconWithImplementsL = 
            (envMutRec, mutRecDefns) ||> MutRecShapes.mapTyconsWithEnv (fun envinner (origInfo, tyconAndAttrsOpt) -> 
               match origInfo, tyconAndAttrsOpt with 
               | (typeDefCore, _, _), Some (tycon, (attrs, _)) ->
                let (MutRecDefnsPhase1DataForTycon(_, synTyconRepr, explicitImplements, _, _, _)) = typeDefCore
                let m = tycon.Range
                let tcref = mkLocalTyconRef tycon
                let envinner = AddDeclaredTypars CheckForDuplicateTypars (tycon.Typars m) envinner
                let envinner = MakeInnerEnvForTyconRef envinner tcref false 
                
                let implementedTys, _ = List.mapFold (mapFoldFst (TcTypeAndRecover cenv NoNewTypars checkConstraints ItemOccurence.UseInType envinner)) tpenv explicitImplements

                if firstPass then 
                    tycon.entity_attribs <- attrs

                let implementedTys, inheritedTys = 
                    match synTyconRepr with 
                    | SynTypeDefnSimpleRepr.Exception _ -> [], []
                    | SynTypeDefnSimpleRepr.General (kind, inherits, slotsigs, fields, isConcrete, _, _, m) ->
                        let kind = InferTyconKind g (kind, attrs, slotsigs, fields, inSig, isConcrete, m)

                        let inherits = inherits |> List.map (fun (ty, m, _) -> (ty, m)) 
                        let inheritedTys = fst (List.mapFold (mapFoldFst (TcTypeAndRecover cenv NoNewTypars checkConstraints ItemOccurence.UseInType envinner)) tpenv inherits)
                        let implementedTys, inheritedTys =   
                            match kind with 
                            | SynTypeDefnKind.Interface -> 
                                explicitImplements |> List.iter (fun (_, m) -> errorR(Error(FSComp.SR.tcInterfacesShouldUseInheritNotInterface(), m)))
                                (implementedTys @ inheritedTys), [] 
                            | _ -> implementedTys, inheritedTys
                        implementedTys, inheritedTys 
                    | SynTypeDefnSimpleRepr.Enum _ | SynTypeDefnSimpleRepr.None _ | SynTypeDefnSimpleRepr.TypeAbbrev _
                    
                    | SynTypeDefnSimpleRepr.Union _ | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _ | SynTypeDefnSimpleRepr.Record _ -> 
                        // REVIEW: we could do the IComparable/IStructuralHash interface analysis here. 
                        // This would let the type satisfy more recursive IComparable/IStructuralHash constraints 
                        implementedTys, []

                for implementedTy, m in implementedTys do
                    if firstPass && isErasedType g implementedTy then 
                        errorR(Error(FSComp.SR.tcCannotInheritFromErasedType(), m)) 

                // Publish interfaces, but only on the first pass, to avoid a duplicate interface check 
                if firstPass then 
                    implementedTys |> List.iter (fun (ty, m) -> PublishInterface cenv envinner.DisplayEnv tcref m false ty) 

                Some (attrs, inheritedTys, synTyconRepr, tycon)
               | _ -> None)

        // Publish the attributes and supertype  
        tyconWithImplementsL |> MutRecShapes.iterTycons (Option.iter (fun (attrs, inheritedTys, synTyconRepr, tycon) -> 
          let m = tycon.Range
          try 
              let super = 
                  match synTyconRepr with 
                  | SynTypeDefnSimpleRepr.Exception _ -> Some g.exn_ty
                  | SynTypeDefnSimpleRepr.None _ -> None
                  | SynTypeDefnSimpleRepr.TypeAbbrev _ -> None
                  | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _ -> None
                  | SynTypeDefnSimpleRepr.Union _ 
                  | SynTypeDefnSimpleRepr.Record _ ->
                      if tycon.IsStructRecordOrUnionTycon then Some(g.system_Value_ty)
                      else None
                  | SynTypeDefnSimpleRepr.General (kind, _, slotsigs, fields, isConcrete, _, _, _) ->
                      let kind = InferTyconKind g (kind, attrs, slotsigs, fields, inSig, isConcrete, m)
                                           
                      match inheritedTys with 
                      | [] -> 
                          match kind with 
                          | SynTypeDefnKind.Struct -> Some(g.system_Value_ty)
                          | SynTypeDefnKind.Delegate _ -> Some(g.system_MulticastDelegate_ty )
                          | SynTypeDefnKind.Opaque | SynTypeDefnKind.Class | SynTypeDefnKind.Interface -> None
                          | _ -> error(InternalError("should have inferred tycon kind", m)) 

                      | [(ty, m)] -> 
                          if not firstPass && not (match kind with SynTypeDefnKind.Class -> true | _ -> false) then 
                              errorR (Error(FSComp.SR.tcStructsInterfacesEnumsDelegatesMayNotInheritFromOtherTypes(), m)) 
                          CheckSuperType cenv ty m 
                          if isTyparTy g ty then 
                              if firstPass then 
                                  errorR(Error(FSComp.SR.tcCannotInheritFromVariableType(), m)) 
                              Some g.obj_ty // a "super" that is a variable type causes grief later
                          else                          
                              Some ty 
                      | _ -> 
                          error(Error(FSComp.SR.tcTypesCannotInheritFromMultipleConcreteTypes(), m))

                  | SynTypeDefnSimpleRepr.Enum _ -> 
                      Some(g.system_Enum_ty) 

              // Allow super type to be a function type but convert back to FSharpFunc<A,B> to make sure it has metadata
              // (We don't apply the same rule to tuple types, i.e. no F#-declared inheritors of those are permitted)
              let super = 
                  super |> Option.map (fun ty -> 
                     if isFunTy g ty then  
                         let a,b = destFunTy g ty
                         mkAppTy g.fastFunc_tcr [a; b] 
                     else ty)

              // Publish the super type
              tycon.TypeContents.tcaug_super <- super
              
           with exn -> errorRecovery exn m))

    /// Establish the fields, dispatch slots and union cases of a type
    let private TcTyconDefnCore_Phase1G_EstablishRepresentation (cenv: cenv) envinner tpenv inSig (MutRecDefnsPhase1DataForTycon(_, synTyconRepr, _, _, _, _)) (tycon: Tycon) (attrs: Attribs) =
        let g = cenv.g
        let m = tycon.Range
        try 
            let id = tycon.Id
            let thisTyconRef = mkLocalTyconRef tycon
            let innerParent = Parent thisTyconRef
            let thisTyInst, thisTy = generalizeTyconRef g thisTyconRef

            let hasAbstractAttr = HasFSharpAttribute g g.attrib_AbstractClassAttribute attrs
            let hasSealedAttr = 
                // The special case is needed for 'unit' because the 'Sealed' attribute is not yet available when this type is defined.
                if g.compilingFSharpCore && id.idText = "Unit" then 
                    Some true
                else
                    TryFindFSharpBoolAttribute g g.attrib_SealedAttribute attrs
            let hasMeasureAttr = HasFSharpAttribute g g.attrib_MeasureAttribute attrs
            
            // REVIEW: for hasMeasureableAttr we need to be stricter about checking these
            // are only used on exactly the right kinds of type definitions and not in conjunction with other attributes.
            let hasMeasureableAttr = HasFSharpAttribute g g.attrib_MeasureableAttribute attrs
            let hasCLIMutable = HasFSharpAttribute g g.attrib_CLIMutableAttribute attrs
            
            let structLayoutAttr = TryFindFSharpInt32Attribute g g.attrib_StructLayoutAttribute attrs
            let hasAllowNullLiteralAttr = TryFindFSharpBoolAttribute g g.attrib_AllowNullLiteralAttribute attrs = Some true

            if hasAbstractAttr then 
                tycon.TypeContents.tcaug_abstract <- true

            tycon.entity_attribs <- attrs
            let noAbstractClassAttributeCheck() = 
                if hasAbstractAttr then errorR (Error(FSComp.SR.tcOnlyClassesCanHaveAbstract(), m))
                
            let noAllowNullLiteralAttributeCheck() = 
                if hasAllowNullLiteralAttr then errorR (Error(FSComp.SR.tcRecordsUnionsAbbreviationsStructsMayNotHaveAllowNullLiteralAttribute(), m))
                
                
            let allowNullLiteralAttributeCheck() = 
                if hasAllowNullLiteralAttr then 
                    tycon.TypeContents.tcaug_super |> Option.iter (fun ty -> if not (TypeNullIsExtraValue g m ty) then errorR (Error(FSComp.SR.tcAllowNullTypesMayOnlyInheritFromAllowNullTypes(), m)))
                    tycon.ImmediateInterfaceTypesOfFSharpTycon |> List.iter (fun ty -> if not (TypeNullIsExtraValue g m ty) then errorR (Error(FSComp.SR.tcAllowNullTypesMayOnlyInheritFromAllowNullTypes(), m)))
                
                
            let structLayoutAttributeCheck allowed = 
                let explicitKind = int32 System.Runtime.InteropServices.LayoutKind.Explicit
                match structLayoutAttr with
                | Some kind ->
                    if allowed then 
                        if kind = explicitKind then
                            warning(PossibleUnverifiableCode m)
                    elif List.isEmpty (thisTyconRef.Typars m) then
                        errorR (Error(FSComp.SR.tcOnlyStructsCanHaveStructLayout(), m))
                    else
                        errorR (Error(FSComp.SR.tcGenericTypesCannotHaveStructLayout(), m))
                | None -> ()
                
            let hiddenReprChecks hasRepr =
                 structLayoutAttributeCheck false
                 if hasSealedAttr = Some false || (hasRepr && hasSealedAttr <> Some true && not (id.idText = "Unit" && g.compilingFSharpCore) ) then 
                    errorR(Error(FSComp.SR.tcRepresentationOfTypeHiddenBySignature(), m))
                 if hasAbstractAttr then 
                     errorR (Error(FSComp.SR.tcOnlyClassesCanHaveAbstract(), m))

            let noMeasureAttributeCheck() = 
                if hasMeasureAttr then errorR (Error(FSComp.SR.tcOnlyTypesRepresentingUnitsOfMeasureCanHaveMeasure(), m))

            let noCLIMutableAttributeCheck() = 
                if hasCLIMutable then errorR (Error(FSComp.SR.tcThisTypeMayNotHaveACLIMutableAttribute(), m))

            let noSealedAttributeCheck k = 
                if hasSealedAttr = Some true then errorR (Error(k(), m))

            let noFieldsCheck(fields': RecdField list) = 
                match fields' with 
                | rf :: _ -> errorR (Error(FSComp.SR.tcInterfaceTypesAndDelegatesCannotContainFields(), rf.Range))
                | _ -> ()
            
            let primaryConstructorInDelegateCheck(implicitCtorSynPats : SynSimplePats option) =
                match implicitCtorSynPats with 
                | None -> ()
                | Some spats ->
                    let ctorArgNames, _ = TcSimplePatsOfUnknownType cenv true CheckCxs envinner tpenv spats
                    if not ctorArgNames.IsEmpty then errorR (Error(FSComp.SR.parsOnlyClassCanTakeValueArguments(), m))
                
            let envinner = AddDeclaredTypars CheckForDuplicateTypars (tycon.Typars m) envinner
            let envinner = MakeInnerEnvForTyconRef envinner thisTyconRef false 


            // Notify the Language Service about field names in record/class declaration
            let ad = envinner.AccessRights
            let writeFakeRecordFieldsToSink (fields: RecdField list) =
                let nenv = envinner.NameEnv
                // Record fields should be visible from IntelliSense, so add fake names for them (similarly to "let a = ..")
                for fspec in fields do
                    if not fspec.IsCompilerGenerated then
                        let info = RecdFieldInfo(thisTyInst, thisTyconRef.MakeNestedRecdFieldRef fspec)
                        let nenv' = AddFakeNameToNameEnv fspec.LogicalName nenv (Item.RecdField info) 
                        // Name resolution gives better info for tooltips
                        let item = Item.RecdField(FreshenRecdFieldRef cenv.nameResolver m (thisTyconRef.MakeNestedRecdFieldRef fspec))
                        CallNameResolutionSink cenv.tcSink (fspec.Range, nenv, item, emptyTyparInst, ItemOccurence.Binding, ad)
                        // Environment is needed for completions
                        CallEnvSink cenv.tcSink (fspec.Range, nenv', ad)

            // Notify the Language Service about constructors in discriminated union declaration
            let writeFakeUnionCtorsToSink (unionCases: UnionCase list) = 
                let nenv = envinner.NameEnv
                // Constructors should be visible from IntelliSense, so add fake names for them 
                for unionCase in unionCases do
                    let info = UnionCaseInfo(thisTyInst, mkUnionCaseRef thisTyconRef unionCase.Id.idText)
                    let nenv' = AddFakeNameToNameEnv unionCase.Id.idText nenv (Item.UnionCase(info, false)) 
                    // Report to both - as in previous function
                    let item = Item.UnionCase(info, false)
                    CallNameResolutionSink cenv.tcSink (unionCase.Range, nenv, item, emptyTyparInst, ItemOccurence.Binding, ad)
                    CallEnvSink cenv.tcSink (unionCase.Id.idRange, nenv', ad)
            
            let typeRepr, baseValOpt, safeInitInfo = 
                match synTyconRepr with 

                | SynTypeDefnSimpleRepr.Exception synExnDefnRepr -> 
                    let parent = Parent (mkLocalTyconRef tycon)
                    TcExceptionDeclarations.TcExnDefnCore_Phase1G_EstablishRepresentation cenv envinner parent tycon synExnDefnRepr |> ignore
                    TNoRepr, None, NoSafeInitInfo

                | SynTypeDefnSimpleRepr.None _ -> 
                    hiddenReprChecks false
                    noAllowNullLiteralAttributeCheck()
                    if hasMeasureAttr then 
                        let repr = TFSharpObjectRepr { fsobjmodel_kind=TFSharpClass 
                                                       fsobjmodel_vslots=[]
                                                       fsobjmodel_rfields= Construct.MakeRecdFieldsTable [] }
                        repr, None, NoSafeInitInfo
                    else 
                        TNoRepr, None, NoSafeInitInfo

                // This unfortunate case deals with "type x = A" 
                // In F# this only defines a new type if A is not in scope 
                // as a type constructor, or if the form type A = A is used. 
                // "type x = | A" can always be used instead. 
                | TyconCoreAbbrevThatIsReallyAUnion (hasMeasureAttr, envinner, id) (unionCaseName, _) ->
                          
                    structLayoutAttributeCheck false
                    noAllowNullLiteralAttributeCheck()
                    TcRecdUnionAndEnumDeclarations.CheckUnionCaseName cenv unionCaseName
                    let unionCase = Construct.NewUnionCase unionCaseName [] thisTy [] XmlDoc.Empty tycon.Accessibility
                    writeFakeUnionCtorsToSink [ unionCase ]
                    Construct.MakeUnionRepr [ unionCase ], None, NoSafeInitInfo

                | SynTypeDefnSimpleRepr.TypeAbbrev(ParserDetail.ErrorRecovery, _rhsType, _) ->
                    TNoRepr, None, NoSafeInitInfo

                | SynTypeDefnSimpleRepr.TypeAbbrev(ParserDetail.Ok, rhsType, _) ->
                    if hasSealedAttr = Some true then 
                        errorR (Error(FSComp.SR.tcAbbreviatedTypesCannotBeSealed(), m))
                    noAbstractClassAttributeCheck()
                    noAllowNullLiteralAttributeCheck()
                    if hasMeasureableAttr then 
                        let kind = if hasMeasureAttr then TyparKind.Measure else TyparKind.Type
                        let theTypeAbbrev, _ = TcTypeOrMeasureAndRecover (Some kind) cenv NoNewTypars CheckCxs ItemOccurence.UseInType envinner tpenv rhsType

                        TMeasureableRepr theTypeAbbrev, None, NoSafeInitInfo
                    // If we already computed a representation, e.g. for a generative type definition, then don't change it here.
                    elif (match tycon.TypeReprInfo with TNoRepr -> false | _ -> true) then 
                        tycon.TypeReprInfo, None, NoSafeInitInfo
                    else 
                        TNoRepr, None, NoSafeInitInfo

                | SynTypeDefnSimpleRepr.Union (_, unionCases, _) -> 
                    noCLIMutableAttributeCheck()
                    noMeasureAttributeCheck()
                    noSealedAttributeCheck FSComp.SR.tcTypesAreAlwaysSealedDU
                    noAbstractClassAttributeCheck()
                    noAllowNullLiteralAttributeCheck()
                    structLayoutAttributeCheck false
                    let unionCases = TcRecdUnionAndEnumDeclarations.TcUnionCaseDecls cenv envinner innerParent thisTy thisTyInst tpenv unionCases
                        
                    if tycon.IsStructRecordOrUnionTycon && unionCases.Length > 1 then 
                      let fieldNames = [ for uc in unionCases do for ft in uc.FieldTable.TrueInstanceFieldsAsList do yield ft.LogicalName ]
                      if fieldNames |> List.distinct |> List.length <> fieldNames.Length then 
                          errorR(Error(FSComp.SR.tcStructUnionMultiCaseDistinctFields(), m))

                    writeFakeUnionCtorsToSink unionCases
                    let repr = Construct.MakeUnionRepr unionCases
                    repr, None, NoSafeInitInfo

                | SynTypeDefnSimpleRepr.Record (_, fields, _) -> 
                    noMeasureAttributeCheck()
                    noSealedAttributeCheck FSComp.SR.tcTypesAreAlwaysSealedRecord
                    noAbstractClassAttributeCheck()
                    noAllowNullLiteralAttributeCheck()
                    structLayoutAttributeCheck true  // these are allowed for records
                    let recdFields = TcRecdUnionAndEnumDeclarations.TcNamedFieldDecls cenv envinner innerParent false tpenv fields
                    recdFields |> CheckDuplicates (fun f -> f.Id) "field" |> ignore
                    writeFakeRecordFieldsToSink recdFields
                    let repr = TFSharpRecdRepr (Construct.MakeRecdFieldsTable recdFields)
                    repr, None, NoSafeInitInfo

                | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly (s, _) -> 
                    let s = (s :?> ILType)
                    noCLIMutableAttributeCheck()
                    noMeasureAttributeCheck()
                    noSealedAttributeCheck FSComp.SR.tcTypesAreAlwaysSealedAssemblyCode
                    noAllowNullLiteralAttributeCheck()
                    structLayoutAttributeCheck false
                    noAbstractClassAttributeCheck()
                    TAsmRepr s, None, NoSafeInitInfo

                | SynTypeDefnSimpleRepr.General (kind, inherits, slotsigs, fields, isConcrete, isIncrClass, implicitCtorSynPats, _) ->
                    let userFields = TcRecdUnionAndEnumDeclarations.TcNamedFieldDecls cenv envinner innerParent isIncrClass tpenv fields
                    let implicitStructFields = 
                        [ // For structs with an implicit ctor, determine the fields immediately based on the arguments
                          match implicitCtorSynPats with 
                          | None -> 
                              ()
                          | Some spats -> 
                              if tycon.IsFSharpStructOrEnumTycon then 
                                  let ctorArgNames, patEnv = TcSimplePatsOfUnknownType cenv true CheckCxs envinner tpenv spats

                                  let (TcPatLinearEnv(_, names, _)) = patEnv

                                  for arg in ctorArgNames do
                                      let ty = names[arg].Type
                                      let id = names[arg].Ident
                                      let taccess = TAccess [envinner.eAccessPath]
                                      yield Construct.NewRecdField false None id false ty false false [] [] XmlDoc.Empty taccess true ]
                    
                    (userFields @ implicitStructFields) |> CheckDuplicates (fun f -> f.Id) "field" |> ignore
                    writeFakeRecordFieldsToSink userFields
                    
                    let superTy = tycon.TypeContents.tcaug_super
                    let containerInfo = TyconContainerInfo(innerParent, thisTyconRef, thisTyconRef.Typars m, NoSafeInitInfo)
                    let kind = InferTyconKind g (kind, attrs, slotsigs, fields, inSig, isConcrete, m)
                    match kind with 
                    | SynTypeDefnKind.Opaque -> 
                        hiddenReprChecks true
                        noAllowNullLiteralAttributeCheck()
                        TNoRepr, None, NoSafeInitInfo
                    | _ ->

                        // Note: for a mutually recursive set we can't check this condition 
                        // until "isSealedTy" and "isClassTy" give reliable results. 
                        superTy |> Option.iter (fun ty -> 
                            let m = match inherits with | [] -> m | (_, m, _) :: _ -> m
                            if isSealedTy g ty then 
                                errorR(Error(FSComp.SR.tcCannotInheritFromSealedType(), m))
                            elif not (isClassTy g ty) then 
                                errorR(Error(FSComp.SR.tcCannotInheritFromInterfaceType(), m)))

                        let kind = 
                            match kind with 
                              | SynTypeDefnKind.Struct -> 
                                  noCLIMutableAttributeCheck()
                                  noSealedAttributeCheck FSComp.SR.tcTypesAreAlwaysSealedStruct
                                  noAbstractClassAttributeCheck()
                                  noAllowNullLiteralAttributeCheck()
                                  if not (isNil slotsigs) then 
                                    errorR (Error(FSComp.SR.tcStructTypesCannotContainAbstractMembers(), m)) 
                                  structLayoutAttributeCheck true

                                  TFSharpStruct
                              | SynTypeDefnKind.Interface -> 
                                  if hasSealedAttr = Some true then errorR (Error(FSComp.SR.tcInterfaceTypesCannotBeSealed(), m))
                                  noCLIMutableAttributeCheck()
                                  structLayoutAttributeCheck false
                                  noAbstractClassAttributeCheck()
                                  allowNullLiteralAttributeCheck()
                                  noFieldsCheck userFields
                                  TFSharpInterface
                              | SynTypeDefnKind.Class -> 
                                  noCLIMutableAttributeCheck()
                                  structLayoutAttributeCheck(not isIncrClass)
                                  allowNullLiteralAttributeCheck()
                                  TFSharpClass
                              | SynTypeDefnKind.Delegate (ty, arity) -> 
                                  noCLIMutableAttributeCheck()
                                  noSealedAttributeCheck FSComp.SR.tcTypesAreAlwaysSealedDelegate
                                  structLayoutAttributeCheck false
                                  noAllowNullLiteralAttributeCheck()
                                  noAbstractClassAttributeCheck()
                                  noFieldsCheck userFields
                                  primaryConstructorInDelegateCheck(implicitCtorSynPats)
                                  let tyR, _ = TcTypeAndRecover cenv NoNewTypars CheckCxs ItemOccurence.UseInType envinner tpenv ty
                                  let _, _, curriedArgInfos, returnTy, _ = GetTopValTypeInCompiledForm g (arity |> TranslateSynValInfo m (TcAttributes cenv envinner)  |> TranslatePartialValReprInfo []) 0 tyR m
                                  if curriedArgInfos.Length < 1 then error(Error(FSComp.SR.tcInvalidDelegateSpecification(), m))
                                  if curriedArgInfos.Length > 1 then error(Error(FSComp.SR.tcDelegatesCannotBeCurried(), m))
                                  let ttps = thisTyconRef.Typars m
                                  let fparams = curriedArgInfos.Head |> List.map MakeSlotParam 
                                  TFSharpDelegate (MakeSlotSig("Invoke", thisTy, ttps, [], [fparams], returnTy))
                              | _ -> 
                                  error(InternalError("should have inferred tycon kind", m))

                        let baseIdOpt = 
                            match synTyconRepr with 
                            | SynTypeDefnSimpleRepr.None _ -> None
                            | SynTypeDefnSimpleRepr.Exception _ -> None
                            | SynTypeDefnSimpleRepr.TypeAbbrev _ -> None
                            | SynTypeDefnSimpleRepr.Union _ -> None
                            | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _ -> None
                            | SynTypeDefnSimpleRepr.Record _ -> None
                            | SynTypeDefnSimpleRepr.Enum _ -> None
                            | SynTypeDefnSimpleRepr.General (_, inherits, _, _, _, _, _, _) ->
                                match inherits with 
                                | [] -> None
                                | (_, m, baseIdOpt) :: _ -> 
                                    match baseIdOpt with 
                                    | None -> Some(ident("base", m)) 
                                    | Some id -> Some id
                            
                        let abstractSlots = 
                            [ for synValSig, memberFlags in slotsigs do 

                                  let (SynValSig(range=m)) = synValSig

                                  CheckMemberFlags None NewSlotsOK OverridesOK memberFlags m
                                  
                                  let slots = fst (TcAndPublishValSpec (cenv, envinner, containerInfo, ModuleOrMemberBinding, Some memberFlags, tpenv, synValSig))
                                  // Multiple slots may be returned, e.g. for 
                                  //    abstract P: int with get, set
                                  
                                  for slot in slots do 
                                      yield mkLocalValRef slot ]

                        let baseValOpt = MakeAndPublishBaseVal cenv envinner baseIdOpt (superOfTycon g tycon)
                        let safeInitInfo = ComputeInstanceSafeInitInfo cenv envinner thisTyconRef.Range thisTy
                        let safeInitFields = match safeInitInfo with SafeInitField (_, fld) -> [fld] | NoSafeInitInfo -> []
                        
                        let repr = 
                            TFSharpObjectRepr 
                                { fsobjmodel_kind = kind 
                                  fsobjmodel_vslots = abstractSlots
                                  fsobjmodel_rfields = Construct.MakeRecdFieldsTable (userFields @ implicitStructFields @ safeInitFields) } 
                        repr, baseValOpt, safeInitInfo

                | SynTypeDefnSimpleRepr.Enum (decls, m) -> 
                    let fieldTy, fields' = TcRecdUnionAndEnumDeclarations.TcEnumDecls cenv envinner innerParent thisTy decls
                    let kind = TFSharpEnum
                    structLayoutAttributeCheck false
                    noCLIMutableAttributeCheck()
                    noSealedAttributeCheck FSComp.SR.tcTypesAreAlwaysSealedEnum
                    noAllowNullLiteralAttributeCheck()
                    let vid = ident("value__", m)
                    let vfld = Construct.NewRecdField false None vid false fieldTy false false [] [] XmlDoc.Empty taccessPublic true
                    
                    let legitEnumTypes = [ g.int32_ty; g.int16_ty; g.sbyte_ty; g.int64_ty; g.char_ty; g.bool_ty; g.uint32_ty; g.uint16_ty; g.byte_ty; g.uint64_ty ]
                    if not (ListSet.contains (typeEquiv g) fieldTy legitEnumTypes) then 
                        errorR(Error(FSComp.SR.tcInvalidTypeForLiteralEnumeration(), m))

                    writeFakeRecordFieldsToSink fields' 
                    let repr = 
                        TFSharpObjectRepr 
                            { fsobjmodel_kind=kind 
                              fsobjmodel_vslots=[]
                              fsobjmodel_rfields= Construct.MakeRecdFieldsTable (vfld :: fields') }
                    repr, None, NoSafeInitInfo
            
            tycon.entity_tycon_repr <- typeRepr
            // We check this just after establishing the representation
            if TyconHasUseNullAsTrueValueAttribute g tycon && not (CanHaveUseNullAsTrueValueAttribute g tycon) then 
                errorR(Error(FSComp.SR.tcInvalidUseNullAsTrueValue(), m))
                
            // validate ConditionalAttribute, should it be applied (it's only valid on a type if the type is an attribute type)
            match attrs |> List.tryFind (IsMatchingFSharpAttribute g g.attrib_ConditionalAttribute) with
            | Some _ ->
                if not(ExistsInEntireHierarchyOfType (fun t -> typeEquiv g t (mkAppTy g.tcref_System_Attribute [])) g cenv.amap m AllowMultiIntfInstantiations.Yes thisTy) then
                    errorR(Error(FSComp.SR.tcConditionalAttributeUsage(), m))
            | _ -> ()         
                   
            (baseValOpt, safeInitInfo)
        with exn -> 
            errorRecovery exn m 
            None, NoSafeInitInfo

    /// Check that a set of type definitions is free of cycles in abbreviations
    let private TcTyconDefnCore_CheckForCyclicAbbreviations tycons = 

        let edgesFrom (tycon: Tycon) =

            let rec accInAbbrevType ty acc = 
                match stripTyparEqns ty with 
                | TType_anon (_,l) 
                | TType_tuple (_, l) -> accInAbbrevTypes l acc
                | TType_ucase (UnionCaseRef(tcref2, _), tinst) 
                | TType_app (tcref2, tinst, _) -> 
                    let tycon2 = tcref2.Deref
                    let acc = accInAbbrevTypes tinst acc
                    // Record immediate recursive references 
                    if ListSet.contains (===) tycon2 tycons then 
                        (tycon, tycon2) :: acc 
                    // Expand the representation of abbreviations 
                    elif tcref2.IsTypeAbbrev then
                        accInAbbrevType (reduceTyconRefAbbrev tcref2 tinst) acc
                    // Otherwise H<inst> - explore the instantiation. 
                    else 
                        acc

                | TType_fun (domainTy, rangeTy, _) -> 
                    accInAbbrevType domainTy (accInAbbrevType rangeTy acc)
                
                | TType_var _ -> acc
                
                | TType_forall (_, bodyTy) -> accInAbbrevType bodyTy acc
                
                | TType_measure measureTy -> accInMeasure measureTy acc

            and accInMeasure measureTy acc =
                match stripUnitEqns measureTy with
                | Measure.Const tcref when ListSet.contains (===) tcref.Deref tycons ->  
                    (tycon, tcref.Deref) :: acc
                | Measure.Const tcref when tcref.IsTypeAbbrev ->              
                    accInMeasure (reduceTyconRefAbbrevMeasureable tcref) acc
                | Measure.Prod (ms1, ms2) -> accInMeasure ms1 (accInMeasure ms2 acc)
                | Measure.Inv invTy -> accInMeasure invTy acc
                | _ -> acc

            and accInAbbrevTypes tys acc = 
                List.foldBack accInAbbrevType tys acc
            
            match tycon.TypeAbbrev with 
            | None -> []
            | Some ty -> accInAbbrevType ty []

        let edges = List.collect edgesFrom tycons
        let graph = Graph<Tycon, Stamp> ((fun tycon -> tycon.Stamp), tycons, edges)
        graph.IterateCycles (fun path -> 
            let tycon = path.Head 
            // The thing is cyclic. Set the abbreviation and representation to be "None" to stop later VS crashes
            tycon.SetTypeAbbrev None
            tycon.entity_tycon_repr <- TNoRepr
            errorR(Error(FSComp.SR.tcTypeDefinitionIsCyclic(), tycon.Range)))


    /// Check that a set of type definitions is free of inheritance cycles
    let TcTyconDefnCore_CheckForCyclicStructsAndInheritance (cenv: cenv) tycons =
        let g = cenv.g
        // Overview:
        // Given several tycons now being defined (the "initial" tycons).
        // Look for cycles in inheritance and struct-field-containment.
        //
        // The graph is on the (initial) type constructors (not types (e.g. tycon instantiations)).
        // Closing under edges:
        // 1. (tycon, superTycon)     -- tycon (initial) to the tycon of its super type.
        // 2. (tycon, interfaceTycon) -- tycon (initial) to the tycon of an interface it implements.
        // 3. (tycon, T)              -- tycon (initial) is a struct with a field (static or instance) that would store a T<_>
        //                              where storing T<_> means is T<_>
        //                                                    or is a struct<instantiation> with an instance field that stores T<_>.
        // The implementation only stores edges between (initial) tycons.
        //
        // The special case "S<'a> static field on S<'a>" is allowed, so no #3 edge is collected for this.
        // Only static fields for current tycons need to be followed. Previous tycons are assumed (previously checked) OK.
        //
        // BEGIN: EARLIER COMMENT
        //        Of course structs are not allowed to contain instance fields of their own type:
        //         type S = struct { field x: S } 
        //
        //        In addition, see bug 3429. In the .NET IL structs are allowed to contain 
        //        static fields of their exact generic type, e.g.
        //         type S    = struct { static field x: S    } 
        //         type S<T> = struct { static field x: S<T> } 
        //        but not
        //         type S<T> = struct { static field x: S<int> } 
        //         type S<T> = struct { static field x: S<T[]> } 
        //        etc.
        //
        //        Ideally structs would allow static fields of any type. However
        //        this is a restriction and exemption that originally stems from 
        //        the way the Microsoft desktop CLR class loader works.
        // END: EARLIER COMMENT

        // edgesFrom tycon collects (tycon, tycon2) edges, for edges as described above.
        let edgesFrom (tycon: Tycon) =
            // Record edge (tycon, tycon2), only when tycon2 is an "initial" tycon.
            let insertEdgeToTycon tycon2 acc = 
                if ListSet.contains (===) tycon2 tycons && // note: only add if tycon2 is initial
                    not (List.exists (fun (tc, tc2) -> tc === tycon && tc2 === tycon2) acc)  // note: only add if (tycon, tycon2) not already an edge
                then
                    (tycon, tycon2) :: acc
                else acc // note: all edges added are (tycon, _)

            let insertEdgeToType ty acc = 
                match tryTcrefOfAppTy g ty with
                | ValueSome tcref ->
                    insertEdgeToTycon tcref.Deref acc
                | _ ->
                    acc

            // collect edges from an a struct field (which is struct-contained in tycon)
            let rec accStructField (structTycon: Tycon) structTyInst (fspec: RecdField) (doneTypes, acc) =
                let fieldTy = actualTyOfRecdFieldForTycon structTycon structTyInst fspec
                accStructFieldType structTycon structTyInst fspec fieldTy (doneTypes, acc)

            // collect edges from an a struct field (given the field type, which may be expanded if it is a type abbreviation)
            and accStructFieldType structTycon structTyInst fspec fieldTy (doneTypes, acc) =
                let fieldTy = stripTyparEqns fieldTy
                match fieldTy with
                | TType_tuple (_isStruct , tinst2) when isStructTupleTy g fieldTy ->
                    // The field is a struct tuple. Check each element of the tuple.
                    // This case was added to resolve issues/3916
                    ((doneTypes, acc), tinst2)
                    ||> List.fold (fun acc' x -> accStructFieldType structTycon structTyInst fspec x acc')

                | TType_app (tcref2 , tinst2, _) when tcref2.IsStructOrEnumTycon ->
                    // The field is a struct.
                    // An edge (tycon, tycon2) should be recorded, unless it is the "static self-typed field" case.
                    let tycon2 = tcref2.Deref
                    let specialCaseStaticField =
                        // The special case of "static field S<'a> in struct S<'a>" is permitted. (so no (S, S) edge to be collected).
                        fspec.IsStatic &&
                        (structTycon === tycon2) && 
                        (structTyInst, tinst2) ||> List.lengthsEqAndForall2 (fun ty1 ty2 -> 
                            match tryDestTyparTy g ty1 with
                            | ValueSome destTypar1 ->
                                match tryDestTyparTy g ty2 with
                                | ValueSome destTypar2 -> typarEq destTypar1 destTypar2
                                | _ -> false
                            | _ -> false)
                    if specialCaseStaticField then
                        doneTypes, acc // no edge collected, no recursion.
                    else
                        let acc = insertEdgeToTycon tycon2 acc // collect edge (tycon, tycon2), if tycon2 is initial.
                        accStructInstanceFields fieldTy tycon2 tinst2 (doneTypes, acc) // recurse through struct field looking for more edges

                | TType_app (tcref2, tinst2, _) when tcref2.IsTypeAbbrev ->
                    // The field is a type abbreviation. Expand and repeat.
                    accStructFieldType structTycon structTyInst fspec (reduceTyconRefAbbrev tcref2 tinst2) (doneTypes, acc)

                | _ ->
                    doneTypes, acc

            // collect edges from the fields of a given struct type.
            and accStructFields includeStaticFields ty (structTycon: Tycon) tinst (doneTypes, acc) =
                if List.exists (typeEquiv g ty) doneTypes then
                    // This type (type instance) has been seen before, so no need to collect the same edges again (and avoid loops!)
                    doneTypes, acc 
                else
                    // Only collect once from each type instance.
                    let doneTypes = ty :: doneTypes 
                    let fspecs = 
                        if structTycon.IsUnionTycon then 
                            [ for uc in structTycon.UnionCasesArray do 
                                 for c in uc.FieldTable.FieldsByIndex do
                                    yield c]
                        else
                            structTycon.AllFieldsAsList 
                    let fspecs = fspecs |> List.filter (fun fspec -> includeStaticFields || not fspec.IsStatic)
                    let doneTypes, acc = List.foldBack (accStructField structTycon tinst) fspecs (doneTypes, acc)
                    doneTypes, acc
            and accStructInstanceFields ty structTycon tinst (doneTypes, acc) = accStructFields false ty structTycon tinst (doneTypes, acc)
            and accStructAllFields ty (structTycon: Tycon) tinst (doneTypes, acc) = accStructFields true ty structTycon tinst (doneTypes, acc)

            let acc = []
            let acc = 
                if tycon.IsStructOrEnumTycon then
                    let tinst, ty = generalizeTyconRef g (mkLocalTyconRef tycon)
                    let _, acc = accStructAllFields ty tycon tinst ([], acc)
                    acc
                else
                    acc

            let acc =
                // Note: only the nominal type counts 
                let super = superOfTycon g tycon
                insertEdgeToType super acc
            let acc =
                // Note: only the nominal type counts 
                List.foldBack insertEdgeToType tycon.ImmediateInterfaceTypesOfFSharpTycon acc
            acc
        let edges = (List.collect edgesFrom tycons)
        let graph = Graph<Tycon, Stamp> ((fun tc -> tc.Stamp), tycons, edges)
        graph.IterateCycles (fun path -> 
            let tycon = path.Head 
            // The thing is cyclic. Set the abbreviation and representation to be "None" to stop later VS crashes
            tycon.SetTypeAbbrev None
            tycon.entity_tycon_repr <- TNoRepr
            errorR(Error(FSComp.SR.tcTypeDefinitionIsCyclicThroughInheritance(), tycon.Range)))
        

    // Interlude between Phase1D and Phase1E - Check and publish the explicit constraints. 
    let TcMutRecDefns_CheckExplicitConstraints cenv tpenv m checkConstraints envMutRecPrelim withEnvs = 
        (envMutRecPrelim, withEnvs) ||> MutRecShapes.iterTyconsWithEnv (fun envForDecls (origInfo, tyconOpt) -> 
            match origInfo, tyconOpt with 
            | (typeDefCore, _, _), Some (tycon: Tycon) -> 
                let (MutRecDefnsPhase1DataForTycon(synTyconInfo, _, _, _, _, _)) = typeDefCore
                let (SynComponentInfo(_, TyparsAndConstraints (_, cs1), cs2, _, _, _, _, _)) = synTyconInfo
                let synTyconConstraints = cs1 @ cs2
                let envForTycon = AddDeclaredTypars CheckForDuplicateTypars (tycon.Typars m) envForDecls
                let thisTyconRef = mkLocalTyconRef tycon
                let envForTycon = MakeInnerEnvForTyconRef envForTycon thisTyconRef false 
                try
                    TcTyparConstraints cenv NoNewTypars checkConstraints ItemOccurence.UseInType envForTycon tpenv synTyconConstraints |> ignore
                with exn ->
                    errorRecovery exn m
            | _ -> ())


    let TcMutRecDefns_Phase1 mkLetInfo (cenv: cenv) envInitial parent typeNames inSig tpenv m scopem mutRecNSInfo (mutRecDefns: MutRecShapes<MutRecDefnsPhase1DataForTycon * 'MemberInfo, 'LetInfo, SynComponentInfo>) =
        // Phase1A - build Entity for type definitions, exception definitions and module definitions.
        // Also for abbreviations of any of these. Augmentations are skipped in this phase.
        let withEntities = 
            mutRecDefns 
            |> MutRecShapes.mapWithParent 
                 (parent, typeNames, envInitial)
                 // Build the initial entity for each module definition
                 (fun (innerParent, typeNames, envForDecls) compInfo decls -> 
                     TcTyconDefnCore_Phase1A_BuildInitialModule cenv envForDecls innerParent typeNames compInfo decls) 

                 // Build the initial Tycon for each type definition
                 (fun (innerParent, _, envForDecls) (typeDefCore, tyconMemberInfo) -> 
                     let (MutRecDefnsPhase1DataForTycon(_, _, _, _, _, isAtOriginalTyconDefn)) = typeDefCore
                     let tyconOpt = 
                         if isAtOriginalTyconDefn then 
                             Some (TcTyconDefnCore_Phase1A_BuildInitialTycon cenv envForDecls innerParent typeDefCore)
                         else 
                             None 
                     (typeDefCore, tyconMemberInfo, innerParent), tyconOpt) 

                 // Bundle up the data for each 'val', 'member' or 'let' definition (just package up the data, no processing yet)
                 (fun (innerParent, _, _) synBinds ->               
                    let containerInfo = ModuleOrNamespaceContainerInfo(match innerParent with Parent p -> p | _ -> failwith "unreachable")
                    mkLetInfo containerInfo synBinds)

        // Phase1AB - Publish modules
        let envTmp, withEnvs =  
            (envInitial, withEntities) ||> MutRecShapes.computeEnvs 
              (fun envAbove (MutRecDefnsPhase2DataForModule (moduleTyAcc, moduleEntity)) ->  
                  PublishModuleDefn cenv envAbove moduleEntity 
                  MakeInnerEnvWithAcc true envAbove moduleEntity.Id moduleTyAcc moduleEntity.ModuleOrNamespaceType.ModuleOrNamespaceKind)
              (fun envAbove _ -> envAbove)

        // Updates the types of the modules to contain the contents so far, which now includes the nested modules and types
        MutRecBindingChecking.TcMutRecDefns_UpdateModuleContents mutRecNSInfo withEnvs 

        // Publish tycons
        (envTmp, withEnvs) ||> MutRecShapes.iterTyconsWithEnv
                (fun envAbove (_, tyconOpt) -> 
                    tyconOpt |> Option.iter (fun tycon -> 
                        // recheck these in case type is a duplicate in a mutually recursive set
                        CheckForDuplicateConcreteType envAbove tycon.LogicalName tycon.Range
                        PublishTypeDefn cenv envAbove tycon))

        // Updates the types of the modules to contain the contents so far
        MutRecBindingChecking.TcMutRecDefns_UpdateModuleContents mutRecNSInfo withEnvs 

        // Phase1AB - Compute the active environments within each nested module.
        //
        // Add the types to the environment. This does not add the fields and union cases (because we haven't established them yet). 
        // We re-add them to the original environment later on. We don't report them to the Language Service yet as we don't know if 
        // they are well-formed (e.g. free of abbreviation cycles) 
        let envMutRecPrelim, withEnvs = (envInitial, withEntities) ||> MutRecBindingChecking.TcMutRecDefns_ComputeEnvs snd (fun _ -> []) cenv false scopem m 

        // Phase 1B. Establish the kind of each type constructor 
        // Here we run InferTyconKind and record partial information about the kind of the type constructor. 
        // This means TyconFSharpObjModelKind is set, which means isSealedTy, isInterfaceTy etc. give accurate results. 
        let withAttrs = 
            (envMutRecPrelim, withEnvs) ||> MutRecShapes.mapTyconsWithEnv (fun envForDecls (origInfo, tyconOpt) -> 
                let res = 
                    match origInfo, tyconOpt with 
                    | (typeDefCore, _, _), Some tycon -> Some (tycon, TcTyconDefnCore_Phase1B_EstablishBasicKind cenv inSig envForDecls typeDefCore tycon)
                    | _ -> None
                origInfo, res)
            
        // Phase 1C. Establish the abbreviations (no constraint checking, because constraints not yet established)
        (envMutRecPrelim, withAttrs) ||> MutRecShapes.iterTyconsWithEnv (fun envForDecls (origInfo, tyconAndAttrsOpt) -> 
            match origInfo, tyconAndAttrsOpt with 
            | (typeDefCore, _, _), Some (tycon, (attrs, _)) -> TcTyconDefnCore_Phase1C_Phase1E_EstablishAbbreviations cenv envForDecls inSig tpenv FirstPass typeDefCore tycon attrs
            | _ -> ()) 

        // Check for cyclic abbreviations. If this succeeds we can start reducing abbreviations safely.
        let tycons = withEntities |> MutRecShapes.collectTycons |> List.choose snd

        TcTyconDefnCore_CheckForCyclicAbbreviations tycons

        // Phase 1D. Establish the super type and interfaces (no constraint checking, because constraints not yet established)     
        (envMutRecPrelim, withAttrs) |> TcTyconDefnCore_Phase1D_Phase1F_EstablishSuperTypesAndInterfaceTypes cenv tpenv inSig FirstPass 

        // Interlude between Phase1D and Phase1E - Add the interface and member declarations for 
        // hash/compare. Because this adds interfaces, this may let constraints 
        // be satisfied, so we have to do this prior to checking any constraints.
        //
        // First find all the field types in all the structural types
        let tyconsWithStructuralTypes = 
            (envMutRecPrelim, withEnvs) 
            ||> MutRecShapes.mapTyconsWithEnv (fun envForDecls (origInfo, tyconOpt) -> 
                   match origInfo, tyconOpt with 
                   | (typeDefCore, _, _), Some tycon -> Some (tycon, GetStructuralElementsOfTyconDefn cenv envForDecls tpenv typeDefCore tycon)
                   | _ -> None) 
            |> MutRecShapes.collectTycons 
            |> List.choose id
        
        let scSet = TyconConstraintInference.InferSetOfTyconsSupportingComparable cenv envMutRecPrelim.DisplayEnv tyconsWithStructuralTypes
        let seSet = TyconConstraintInference.InferSetOfTyconsSupportingEquatable cenv envMutRecPrelim.DisplayEnv tyconsWithStructuralTypes

        (envMutRecPrelim, withEnvs) 
        ||> MutRecShapes.iterTyconsWithEnv (fun envForDecls (_, tyconOpt) -> 
               tyconOpt |> Option.iter (AddAugmentationDeclarations.AddGenericHashAndComparisonDeclarations cenv envForDecls scSet seSet))

        TcMutRecDefns_CheckExplicitConstraints cenv tpenv m NoCheckCxs envMutRecPrelim withEnvs 

        // No inferred constraints allowed on declared typars 
        (envMutRecPrelim, withEnvs) ||> MutRecShapes.iterTyconsWithEnv (fun envForDecls (_, tyconOpt) -> 
            tyconOpt |> Option.iter (fun tycon -> tycon.Typars m |> List.iter (SetTyparRigid envForDecls.DisplayEnv m)))
        
        // Phase1E. OK, now recheck the abbreviations, super/interface and explicit constraints types (this time checking constraints)
        (envMutRecPrelim, withAttrs) ||> MutRecShapes.iterTyconsWithEnv (fun envForDecls (origInfo, tyconAndAttrsOpt) -> 
            match origInfo, tyconAndAttrsOpt with 
            | (typeDefCore, _, _), Some (tycon, (attrs, _)) -> TcTyconDefnCore_Phase1C_Phase1E_EstablishAbbreviations cenv envForDecls inSig tpenv SecondPass typeDefCore tycon attrs
            | _ -> ()) 

        // Phase1F. Establish inheritance hierarchy
        (envMutRecPrelim, withAttrs) |> TcTyconDefnCore_Phase1D_Phase1F_EstablishSuperTypesAndInterfaceTypes cenv tpenv inSig SecondPass 

        TcMutRecDefns_CheckExplicitConstraints cenv tpenv m CheckCxs envMutRecPrelim withEnvs 

        // Add exception definitions to the environments, which are used for checking exception abbreviations in representations
        let envMutRecPrelim, withAttrs =  
            (envMutRecPrelim, withAttrs) 
            ||> MutRecShapes.extendEnvs (fun envForDecls decls -> 
                    let tycons = decls |> List.choose (function MutRecShape.Tycon (_, Some (tycon, _)) -> Some tycon | _ -> None) 
                    let exns = tycons |> List.filter (fun tycon -> tycon.IsFSharpException) 
                    let envForDecls = (envForDecls, exns) ||> List.fold (AddLocalExnDefnAndReport cenv.tcSink scopem)
                    envForDecls)

        // Phase1G. Establish inheritance hierarchy
        // Now all the type parameters, abbreviations, constraints and kind information is established.
        // Now do the representations. Each baseValOpt is a residue from the representation which is potentially available when
        // checking the members.
        let withBaseValsAndSafeInitInfos = 
            (envMutRecPrelim, withAttrs) ||> MutRecShapes.mapTyconsWithEnv (fun envForDecls (origInfo, tyconAndAttrsOpt) -> 
                let info = 
                    match origInfo, tyconAndAttrsOpt with 
                    | (typeDefCore, _, _), Some (tycon, (attrs, _)) -> TcTyconDefnCore_Phase1G_EstablishRepresentation cenv envForDecls tpenv inSig typeDefCore tycon attrs
                    | _ -> None, NoSafeInitInfo 
                let tyconOpt, fixupFinalAttrs = 
                    match tyconAndAttrsOpt with
                    | None -> None, (fun () -> ())
                    | Some (tycon, (_prelimAttrs, getFinalAttrs)) -> Some tycon, (fun () -> tycon.entity_attribs <- getFinalAttrs())

                (origInfo, tyconOpt, fixupFinalAttrs, info))
                
        // Now check for cyclic structs and inheritance. It's possible these should be checked as separate conditions. 
        // REVIEW: checking for cyclic inheritance is happening too late. See note above.
        TcTyconDefnCore_CheckForCyclicStructsAndInheritance cenv tycons


        (tycons, envMutRecPrelim, withBaseValsAndSafeInitInfos)


/// Bind declarations in implementation and signature files
module TcDeclarations = 

    /// Given a type definition, compute whether its members form an extension of an existing type, and if so if it is an 
    /// intrinsic or extrinsic extension
    let private ComputeTyconDeclKind (cenv: cenv) (envForDecls: TcEnv) tyconOpt isAtOriginalTyconDefn inSig m (synTypars: SynTyparDecl list) synTyparCxs longPath = 
        let g = cenv.g
        let ad = envForDecls.AccessRights
        
        let tcref = 
          match tyconOpt with
          | Some tycon when isAtOriginalTyconDefn -> 

            // This records a name resolution of the type at the location
            let resInfo = TypeNameResolutionStaticArgsInfo.FromTyArgs synTypars.Length
            ResolveTypeLongIdent cenv.tcSink cenv.nameResolver ItemOccurence.Binding OpenQualified envForDecls.NameEnv ad longPath resInfo PermitDirectReferenceToGeneratedType.No 
               |> ignore

            mkLocalTyconRef tycon

          | _ ->
            let resInfo = TypeNameResolutionStaticArgsInfo.FromTyArgs synTypars.Length
            let _, tcref =
                match ResolveTypeLongIdent cenv.tcSink cenv.nameResolver ItemOccurence.Binding OpenQualified envForDecls.NameEnv ad longPath resInfo PermitDirectReferenceToGeneratedType.No with
                | Result res -> res
                | res when inSig && List.isSingleton longPath ->
                    errorR(Deprecated(FSComp.SR.tcReservedSyntaxForAugmentation(), m))
                    ForceRaise res
                | res -> ForceRaise res
            tcref

        let isInterfaceOrDelegateOrEnum = 
            tcref.Deref.IsFSharpInterfaceTycon || 
            tcref.Deref.IsFSharpDelegateTycon ||
            tcref.Deref.IsFSharpEnumTycon

        let reqTypars = tcref.Typars m

        // Member definitions are intrinsic (added directly to the type) if:
        // a) For interfaces, only if it is in the original defn.
        //    Augmentations to interfaces via partial type defns will always be extensions, e.g. extension members on interfaces.
        // b) For other types, if the type is isInSameModuleOrNamespace
        let declKind, typars = 
          if isAtOriginalTyconDefn then 
              ModuleOrMemberBinding, reqTypars

          else
            let isInSameModuleOrNamespace = 
                 match envForDecls.eModuleOrNamespaceTypeAccumulator.Value.TypesByMangledName.TryGetValue tcref.LogicalName with 
                 | true, tycon -> tyconOrder.Compare(tcref.Deref, tycon) = 0
                 | _ -> 
                        //false
                        // There is a special case we allow when compiling FSharp.Core.dll which permits interface implementations across namespace fragments
                        g.compilingFSharpCore && tcref.LogicalName.StartsWithOrdinal("Tuple`")
        
            let nReqTypars = reqTypars.Length

            let declaredTypars = TcTyparDecls cenv envForDecls synTypars
            let envForTycon = AddDeclaredTypars CheckForDuplicateTypars declaredTypars envForDecls
            let _tpenv = TcTyparConstraints cenv NoNewTypars CheckCxs ItemOccurence.UseInType envForTycon emptyUnscopedTyparEnv synTyparCxs
            declaredTypars |> List.iter (SetTyparRigid envForDecls.DisplayEnv m)

            if isInSameModuleOrNamespace && not isInterfaceOrDelegateOrEnum then 
                // For historical reasons we only give a warning for incorrect type parameters on intrinsic extensions
                if nReqTypars <> synTypars.Length then 
                    errorR(Error(FSComp.SR.tcDeclaredTypeParametersForExtensionDoNotMatchOriginal(tcref.DisplayNameWithStaticParametersAndUnderscoreTypars), m))
                if not (typarsAEquiv g TypeEquivEnv.Empty reqTypars declaredTypars) then 
                    warning(Error(FSComp.SR.tcDeclaredTypeParametersForExtensionDoNotMatchOriginal(tcref.DisplayNameWithStaticParametersAndUnderscoreTypars), m))
                // Note we return 'reqTypars' for intrinsic extensions since we may only have given warnings
                IntrinsicExtensionBinding, reqTypars
            else 
                if isInSameModuleOrNamespace && isInterfaceOrDelegateOrEnum then 
                    errorR(Error(FSComp.SR.tcMembersThatExtendInterfaceMustBePlacedInSeparateModule(), tcref.Range))
                if nReqTypars <> synTypars.Length then 
                    error(Error(FSComp.SR.tcDeclaredTypeParametersForExtensionDoNotMatchOriginal(tcref.DisplayNameWithStaticParametersAndUnderscoreTypars), m))
                if not (typarsAEquiv g TypeEquivEnv.Empty reqTypars declaredTypars) then 
                    errorR(Error(FSComp.SR.tcDeclaredTypeParametersForExtensionDoNotMatchOriginal(tcref.DisplayNameWithStaticParametersAndUnderscoreTypars), m))
                ExtrinsicExtensionBinding, declaredTypars


        declKind, tcref, typars


    let private isAugmentationTyconDefnRepr = function SynTypeDefnSimpleRepr.General(kind=SynTypeDefnKind.Augmentation _) -> true | _ -> false
    let private isAutoProperty = function SynMemberDefn.AutoProperty _ -> true | _ -> false
    let private isMember = function SynMemberDefn.Member _ -> true | _ -> false
    let private isImplicitCtor = function SynMemberDefn.ImplicitCtor _ -> true | _ -> false
    let private isImplicitInherit = function SynMemberDefn.ImplicitInherit _ -> true | _ -> false
    let private isAbstractSlot = function SynMemberDefn.AbstractSlot _ -> true | _ -> false
    let private isInterface = function SynMemberDefn.Interface _ -> true | _ -> false
    let private isInherit = function SynMemberDefn.Inherit _ -> true | _ -> false
    let private isField = function SynMemberDefn.ValField _ -> true | _ -> false
    let private isTycon = function SynMemberDefn.NestedType _ -> true | _ -> false

    let private allFalse ps x = List.forall (fun p -> not (p x)) ps

    /// Check the ordering on the bindings and members in a class construction
    // Accepted forms:
    //
    // Implicit Construction:
    //   implicit_ctor
    //   optional implicit_inherit
    //   multiple bindings
    //   multiple member-binding(includes-overrides) or abstract-slot-declaration or interface-bindings
    //
    // Classic construction:
    //   multiple (binding or slotsig or field or interface or inherit).
    //   i.e. not local-bindings, implicit ctor or implicit inherit (or tycon?).
    //   atMostOne inherit.
    let private CheckMembersForm ds = 
        match ds with
        | d :: ds when isImplicitCtor d ->
            // Implicit construction 
            let ds = 
                match ds with
                | d :: ds when isImplicitInherit d -> ds  // skip inherit call if it comes next 
                | _ -> ds

            // Skip over 'let' and 'do' bindings
            let _, ds = ds |> List.takeUntil (function SynMemberDefn.LetBindings _ -> false | _ -> true) 

            // Skip over 'let' and 'do' bindings
            let _, ds = ds |> List.takeUntil (allFalse [isMember;isAbstractSlot;isInterface;isAutoProperty]) 

            match ds with
             | SynMemberDefn.Member (range=m) :: _ -> errorR(InternalError("List.takeUntil is wrong, have binding", m))
             | SynMemberDefn.AbstractSlot (range=m) :: _ -> errorR(InternalError("List.takeUntil is wrong, have slotsig", m))
             | SynMemberDefn.Interface (range=m) :: _ -> errorR(InternalError("List.takeUntil is wrong, have interface", m))
             | SynMemberDefn.ImplicitCtor (range=m) :: _ -> errorR(InternalError("implicit class construction with two implicit constructions", m))
             | SynMemberDefn.AutoProperty (range=m) :: _ -> errorR(InternalError("List.takeUntil is wrong, have auto property", m))
             | SynMemberDefn.ImplicitInherit (range=m) :: _ -> errorR(Error(FSComp.SR.tcTypeDefinitionsWithImplicitConstructionMustHaveOneInherit(), m))
             | SynMemberDefn.LetBindings (range=m) :: _ -> errorR(Error(FSComp.SR.tcTypeDefinitionsWithImplicitConstructionMustHaveLocalBindingsBeforeMembers(), m))
             | SynMemberDefn.Inherit (range=m) :: _ -> errorR(Error(FSComp.SR.tcInheritDeclarationMissingArguments(), m))
             | SynMemberDefn.NestedType (range=m) :: _ -> errorR(Error(FSComp.SR.tcTypesCannotContainNestedTypes(), m))
             | _ -> ()
        | ds ->
            // Classic class construction 
            let _, ds = List.takeUntil (allFalse [isMember;isAbstractSlot;isInterface;isInherit;isField;isTycon]) ds
            match ds with
             | SynMemberDefn.Member (range=m) :: _ -> errorR(InternalError("CheckMembersForm: List.takeUntil is wrong", m))
             | SynMemberDefn.ImplicitCtor (range=m) :: _ -> errorR(InternalError("CheckMembersForm: implicit ctor line should be first", m))
             | SynMemberDefn.ImplicitInherit (range=m) :: _ -> errorR(Error(FSComp.SR.tcInheritConstructionCallNotPartOfImplicitSequence(), m))
             | SynMemberDefn.AutoProperty(range=m) :: _ -> errorR(Error(FSComp.SR.tcAutoPropertyRequiresImplicitConstructionSequence(), m))
             | SynMemberDefn.LetBindings (isStatic=false; range=m) :: _ -> errorR(Error(FSComp.SR.tcLetAndDoRequiresImplicitConstructionSequence(), m))
             | SynMemberDefn.AbstractSlot (range=m) :: _ 
             | SynMemberDefn.Interface (range=m) :: _ 
             | SynMemberDefn.Inherit (range=m) :: _ 
             | SynMemberDefn.ValField (range=m) :: _ 
             | SynMemberDefn.NestedType (range=m) :: _ -> errorR(InternalError("CheckMembersForm: List.takeUntil is wrong", m))
             | _ -> ()
                     

    /// Separates the definition into core (shape) and body.
    ///
    /// core = synTyconInfo, simpleRepr, interfaceTypes
    ///        where simpleRepr can contain inherit type, declared fields and virtual slots.
    /// body = members
    ///        where members contain methods/overrides, also implicit ctor, inheritCall and local definitions.
    let rec private SplitTyconDefn (SynTypeDefn(typeInfo=synTyconInfo;typeRepr=trepr; members=extraMembers)) =
        let extraMembers = desugarGetSetMembers extraMembers
        let implements1 = List.choose (function SynMemberDefn.Interface (interfaceType=ty) -> Some(ty, ty.Range) | _ -> None) extraMembers
        match trepr with
        | SynTypeDefnRepr.ObjectModel(kind, cspec, m) ->
            let cspec = desugarGetSetMembers cspec
            CheckMembersForm cspec
            let fields = cspec |> List.choose (function SynMemberDefn.ValField (f, _) -> Some f | _ -> None)
            let implements2 = cspec |> List.choose (function SynMemberDefn.Interface (interfaceType=ty) -> Some(ty, ty.Range) | _ -> None)
            let inherits =
                cspec |> List.choose (function 
                    | SynMemberDefn.Inherit (ty, idOpt, m) -> Some(ty, m, idOpt)
                    | SynMemberDefn.ImplicitInherit (ty, _, idOpt, m) -> Some(ty, m, idOpt)
                    | _ -> None)
            //let nestedTycons = cspec |> List.choose (function SynMemberDefn.NestedType (x, _, _) -> Some x | _ -> None)
            let slotsigs = cspec |> List.choose (function SynMemberDefn.AbstractSlot (x, y, _) -> Some(x, y) | _ -> None)
           
            let members = 
                let membersIncludingAutoProps = 
                    cspec |> List.filter (fun memb -> 
                      match memb with 
                      | SynMemberDefn.Interface _
                      | SynMemberDefn.Member _
                      | SynMemberDefn.GetSetMember _
                      | SynMemberDefn.LetBindings _
                      | SynMemberDefn.ImplicitCtor _
                      | SynMemberDefn.AutoProperty _ 
                      | SynMemberDefn.Open _
                      | SynMemberDefn.ImplicitInherit _ -> true
                      | SynMemberDefn.NestedType (_, _, m) -> error(Error(FSComp.SR.tcTypesCannotContainNestedTypes(), m)); false
                      // covered above 
                      | SynMemberDefn.ValField _   
                      | SynMemberDefn.Inherit _ 
                      | SynMemberDefn.AbstractSlot _ -> false)

                // Convert auto properties to let bindings in the pre-list
                let rec preAutoProps memb =
                    match memb with 
                    | SynMemberDefn.AutoProperty(attributes=Attributes attribs; isStatic=isStatic; ident=id; typeOpt=tyOpt; propKind=propKind; xmlDoc=xmlDoc; synExpr=synExpr; range=mWholeAutoProp) -> 
                        // Only the keep the field-targeted attributes
                        let attribs = attribs |> List.filter (fun a -> match a.Target with Some t when t.idText = "field" -> true | _ -> false)
                        let mLetPortion = synExpr.Range
                        let fldId = ident (CompilerGeneratedName id.idText, mLetPortion)
                        let headPat = SynPat.LongIdent (SynLongIdent([fldId], [], [None]), None, Some noInferredTypars, SynArgPats.Pats [], None, mLetPortion)
                        let retInfo = match tyOpt with None -> None | Some ty -> Some (SynReturnInfo((ty, SynInfo.unnamedRetVal), ty.Range))
                        let isMutable = 
                            match propKind with 
                            | SynMemberKind.PropertySet 
                            | SynMemberKind.PropertyGetSet -> true 
                            | _ -> false
                        let attribs = mkAttributeList attribs mWholeAutoProp
                        let binding = mkSynBinding (xmlDoc, headPat) (None, false, isMutable, mLetPortion, DebugPointAtBinding.NoneAtInvisible, retInfo, synExpr, synExpr.Range, [], attribs, None, SynBindingTrivia.Zero)

                        [(SynMemberDefn.LetBindings ([binding], isStatic, false, mWholeAutoProp))]

                    | SynMemberDefn.Interface (members=Some membs) -> membs |> List.collect preAutoProps
                    | SynMemberDefn.LetBindings _
                    | SynMemberDefn.ImplicitCtor _ 
                    | SynMemberDefn.Open _
                    | SynMemberDefn.ImplicitInherit _ -> [memb]
                    | _ -> []

                // Convert auto properties to member bindings in the post-list
                let rec postAutoProps memb =
                    match memb with 
                    | SynMemberDefn.AutoProperty(attributes=Attributes attribs; isStatic=isStatic; ident=id; typeOpt=tyOpt; propKind=propKind; memberFlags=memberFlags; xmlDoc=xmlDoc; accessibility=access; getSetRange=mGetSetOpt) ->
                        let mMemberPortion = id.idRange
                        // Only the keep the non-field-targeted attributes
                        let attribs = attribs |> List.filter (fun a -> match a.Target with Some t when t.idText = "field" -> false | _ -> true)
                        let fldId = ident (CompilerGeneratedName id.idText, mMemberPortion)
                        let headPatIds = if isStatic then [id] else [ident ("__", mMemberPortion);id]
                        let headPat = SynPat.LongIdent (SynLongIdent(headPatIds, [], List.replicate headPatIds.Length None), None, Some noInferredTypars, SynArgPats.Pats [], None, mMemberPortion)

                        match propKind, mGetSetOpt with 
                        | SynMemberKind.PropertySet, Some m -> errorR(Error(FSComp.SR.parsMutableOnAutoPropertyShouldBeGetSetNotJustSet(), m))
                        | _ -> ()
       
                        [ 
                            match propKind with 
                            | SynMemberKind.Member
                            | SynMemberKind.PropertyGet 
                            | SynMemberKind.PropertyGetSet -> 
                                let getter = 
                                    let rhsExpr = SynExpr.Ident fldId
                                    let retInfo = match tyOpt with None -> None | Some ty -> Some (SynReturnInfo((ty, SynInfo.unnamedRetVal), ty.Range))
                                    let attribs = mkAttributeList attribs mMemberPortion
                                    let binding = mkSynBinding (xmlDoc, headPat) (access, false, false, mMemberPortion, DebugPointAtBinding.NoneAtInvisible, retInfo, rhsExpr, rhsExpr.Range, [], attribs, Some (memberFlags SynMemberKind.Member), SynBindingTrivia.Zero)
                                    SynMemberDefn.Member (binding, mMemberPortion) 
                                yield getter
                            | _ -> ()

                            match propKind with 
                            | SynMemberKind.PropertySet 
                            | SynMemberKind.PropertyGetSet -> 
                                let setter = 
                                    let vId = ident("v", mMemberPortion)
                                    let headPat = SynPat.LongIdent (SynLongIdent(headPatIds, [], List.replicate headPatIds.Length None), None, Some noInferredTypars, SynArgPats.Pats [mkSynPatVar None vId], None, mMemberPortion)
                                    let rhsExpr = mkSynAssign (SynExpr.Ident fldId) (SynExpr.Ident vId)
                                    //let retInfo = match tyOpt with None -> None | Some ty -> Some (SynReturnInfo((ty, SynInfo.unnamedRetVal), ty.Range))
                                    let binding = mkSynBinding (xmlDoc, headPat) (access, false, false, mMemberPortion, DebugPointAtBinding.NoneAtInvisible, None, rhsExpr, rhsExpr.Range, [], [], Some (memberFlags SynMemberKind.PropertySet), SynBindingTrivia.Zero)
                                    SynMemberDefn.Member (binding, mMemberPortion) 
                                yield setter 
                            | _ -> ()]
                    | SynMemberDefn.Interface (ty, mWith, Some membs, m) -> 
                        let membs' = membs |> List.collect postAutoProps
                        [SynMemberDefn.Interface (ty, mWith, Some membs', m)]
                    | SynMemberDefn.LetBindings _
                    | SynMemberDefn.ImplicitCtor _ 
                    | SynMemberDefn.Open _
                    | SynMemberDefn.ImplicitInherit _ -> []
                    | _ -> [memb]

                let preMembers = membersIncludingAutoProps |> List.collect preAutoProps
                let postMembers = membersIncludingAutoProps |> List.collect postAutoProps

                preMembers @ postMembers

            let isConcrete = 
                members |> List.exists (function 
                    | SynMemberDefn.Member(SynBinding(valData = SynValData(Some memberFlags, _, _)), _) -> not memberFlags.IsDispatchSlot 
                    | SynMemberDefn.Interface (members=defOpt) -> Option.isSome defOpt
                    | SynMemberDefn.LetBindings _ -> true
                    | SynMemberDefn.ImplicitCtor _ -> true
                    | SynMemberDefn.ImplicitInherit _ -> true
                    | _ -> false)

            let isIncrClass = 
                members |> List.exists (function 
                    | SynMemberDefn.ImplicitCtor _ -> true
                    | _ -> false)

            let hasSelfReferentialCtor = 
                members |> List.exists (function 
                    | SynMemberDefn.ImplicitCtor (_, _, _, thisIdOpt, _, _) 
                    | SynMemberDefn.Member(memberDefn=SynBinding(valData=SynValData(thisIdOpt=thisIdOpt))) -> thisIdOpt.IsSome
                    | _ -> false)

            let implicitCtorSynPats = 
                members |> List.tryPick (function 
                    | SynMemberDefn.ImplicitCtor (_, _, (SynSimplePats.SimplePats _ as spats), _, _, _) -> Some spats
                    | _ -> None)

            // An ugly bit of code to pre-determine if a type has a nullary constructor, prior to establishing the 
            // members of the type
            let preEstablishedHasDefaultCtor = 
                members |> List.exists (function 
                    | SynMemberDefn.Member(memberDefn=SynBinding(valData=SynValData(memberFlags=Some memberFlags); headPat = SynPatForConstructorDecl SynPatForNullaryArgs)) -> 
                        memberFlags.MemberKind=SynMemberKind.Constructor 
                    | SynMemberDefn.ImplicitCtor (_, _, SynSimplePats.SimplePats(spats, _), _, _, _) -> isNil spats
                    | _ -> false)
            let repr = SynTypeDefnSimpleRepr.General(kind, inherits, slotsigs, fields, isConcrete, isIncrClass, implicitCtorSynPats, m)
            let isAtOriginalTyconDefn = not (isAugmentationTyconDefnRepr repr)
            let core = MutRecDefnsPhase1DataForTycon(synTyconInfo, repr, implements2@implements1, preEstablishedHasDefaultCtor, hasSelfReferentialCtor, isAtOriginalTyconDefn)

            core, members @ extraMembers

        | SynTypeDefnRepr.Simple(repr, _) -> 
            let members = []
            let isAtOriginalTyconDefn = true
            let core = MutRecDefnsPhase1DataForTycon(synTyconInfo, repr, implements1, false, false, isAtOriginalTyconDefn)
            core, members @ extraMembers

        | SynTypeDefnRepr.Exception r -> 
            let isAtOriginalTyconDefn = true
            let core = MutRecDefnsPhase1DataForTycon(synTyconInfo, SynTypeDefnSimpleRepr.Exception r, implements1, false, false, isAtOriginalTyconDefn)
            core, extraMembers

    //-------------------------------------------------------------------------

    /// Bind a collection of mutually recursive definitions in an implementation file
    let TcMutRecDefinitions (cenv: cenv) envInitial parent typeNames tpenv m scopem mutRecNSInfo (mutRecDefns: MutRecDefnsInitialData) =

        let g = cenv.g

        // Split the definitions into "core representations" and "members". The code to process core representations
        // is shared between processing of signature files and implementation files.
        let mutRecDefnsAfterSplit = mutRecDefns |> MutRecShapes.mapTycons SplitTyconDefn

        // Create the entities for each module and type definition, and process the core representation of each type definition.
        let tycons, envMutRecPrelim, mutRecDefnsAfterCore = 
            EstablishTypeDefinitionCores.TcMutRecDefns_Phase1 
               (fun containerInfo synBinds -> [ for synBind in synBinds -> RecDefnBindingInfo(containerInfo, NoNewSlots, ModuleOrMemberBinding, synBind) ])
               cenv envInitial parent typeNames false tpenv m scopem mutRecNSInfo mutRecDefnsAfterSplit

        // Package up the phase two information for processing members.
        let mutRecDefnsAfterPrep = 
            (envMutRecPrelim, mutRecDefnsAfterCore)
            ||> MutRecShapes.mapTyconsWithEnv (fun envForDecls ((typeDefnCore, members, innerParent), tyconOpt, fixupFinalAttrs, (baseValOpt, safeInitInfo)) -> 
                let (MutRecDefnsPhase1DataForTycon(synTyconInfo, _, _, _, _, isAtOriginalTyconDefn)) = typeDefnCore
                let tyDeclRange = synTyconInfo.Range
                let (SynComponentInfo(_, TyparsAndConstraints (typars, cs1), cs2, longPath, _, _, _, _)) = synTyconInfo
                let cs = cs1 @ cs2
                let declKind, tcref, declaredTyconTypars = ComputeTyconDeclKind cenv envForDecls tyconOpt isAtOriginalTyconDefn false tyDeclRange typars cs longPath
                let newslotsOK = (if isAtOriginalTyconDefn && tcref.IsFSharpObjectModelTycon then NewSlotsOK else NoNewSlots)

                if (declKind = ExtrinsicExtensionBinding) && isByrefTyconRef g tcref then 
                    error(Error(FSComp.SR.tcByrefsMayNotHaveTypeExtensions(), tyDeclRange))

                if not (isNil members) && tcref.IsTypeAbbrev then 
                    errorR(Error(FSComp.SR.tcTypeAbbreviationsCannotHaveAugmentations(), tyDeclRange))

                let (SynComponentInfo (attributes, _, _, _, _, _, _, _)) = synTyconInfo
                if not (List.isEmpty attributes) && (declKind = ExtrinsicExtensionBinding || declKind = IntrinsicExtensionBinding) then
                    let attributeRange = (List.head attributes).Range
                    error(Error(FSComp.SR.tcAugmentationsCannotHaveAttributes(), attributeRange))

                MutRecDefnsPhase2DataForTycon(tyconOpt, innerParent, declKind, tcref, baseValOpt, safeInitInfo, declaredTyconTypars, members, tyDeclRange, newslotsOK, fixupFinalAttrs))

        // By now we've established the full contents of type definitions apart from their
        // members and any fields determined by implicit construction. We know the kinds and 
        // representations of types and have established them as valid.
        //
        // We now reconstruct the active environments all over again - this will add the union cases and fields. 
        //
        // Note: This environment reconstruction doesn't seem necessary. We're about to create Val's for all members, 
        // which does require type checking, but no more information than is already available.
        let envMutRecPrelimWithReprs, withEnvs =  
            (envInitial, MutRecShapes.dropEnvs mutRecDefnsAfterPrep) 
                ||> MutRecBindingChecking.TcMutRecDefns_ComputeEnvs 
                       (fun (MutRecDefnsPhase2DataForTycon(tyconOpt, _, _, _, _, _, _, _, _, _, _)) -> tyconOpt)  
                       (fun _binds -> [ (* no values are available yet *) ]) 
                       cenv true scopem m 

        // Check the members and decide on representations for types with implicit constructors.
        let withBindings, envFinal = TcMutRecDefns_Phase2 cenv envInitial m scopem mutRecNSInfo envMutRecPrelimWithReprs withEnvs

        // Generate the hash/compare/equality bindings for all tycons.
        //
        // Note: generating these bindings must come after generating the members, since some in the case of structs some fields
        // may be added by generating the implicit construction syntax 
        let withExtraBindings = 
            (envFinal, withBindings) ||> MutRecShapes.expandTyconsWithEnv (fun envForDecls (tyconOpt, _) -> 
                match tyconOpt with 
                | None -> [], [] 
                | Some tycon -> 
                    // We put the hash/compare bindings before the type definitions and the
                    // equality bindings after because tha is the order they've always been generated
                    // in, and there are code generation tests to check that.
                    let binds = AddAugmentationDeclarations.AddGenericHashAndComparisonBindings cenv tycon 
                    let binds3 = AddAugmentationDeclarations.AddGenericEqualityBindings cenv envForDecls tycon
                    binds, binds3)

        // Check for cyclic structs and inheritance all over again, since we may have added some fields to the struct when generating the implicit construction syntax 
        EstablishTypeDefinitionCores.TcTyconDefnCore_CheckForCyclicStructsAndInheritance cenv tycons

        withExtraBindings, envFinal  


    //-------------------------------------------------------------------------

    /// Separates the signature declaration into core (shape) and body.
    let rec private SplitTyconSignature (SynTypeDefnSig(typeInfo=synTyconInfo; typeRepr=trepr; members=extraMembers)) = 

        let implements1 = 
            extraMembers |> List.choose (function SynMemberSig.Interface (f, m) -> Some(f, m) | _ -> None) 

        match trepr with
        | SynTypeDefnSigRepr.ObjectModel(kind, cspec, m) -> 
            let fields = cspec |> List.choose (function SynMemberSig.ValField (f, _) -> Some f | _ -> None)
            let implements2 = cspec |> List.choose (function SynMemberSig.Interface (ty, m) -> Some(ty, m) | _ -> None)
            let inherits = cspec |> List.choose (function SynMemberSig.Inherit (ty, _) -> Some(ty, m, None) | _ -> None)
            //let nestedTycons = cspec |> List.choose (function SynMemberSig.NestedType (x, _) -> Some x | _ -> None)
            let slotsigs = cspec |> List.choose (function SynMemberSig.Member (v, fl, _) when fl.IsDispatchSlot -> Some(v, fl) | _ -> None)
            let members = cspec |> List.filter (function   
                                                          | SynMemberSig.Interface _ -> true
                                                          | SynMemberSig.Member (_, memberFlags, _) when not memberFlags.IsDispatchSlot -> true
                                                          | SynMemberSig.NestedType (_, m) -> error(Error(FSComp.SR.tcTypesCannotContainNestedTypes(), m)); false
                                                          | _ -> false)
            let isConcrete = 
                members |> List.exists (function 
                    | SynMemberSig.Member (_, memberFlags, _) -> memberFlags.MemberKind=SynMemberKind.Constructor 
                    | _ -> false)

            // An ugly bit of code to pre-determine if a type has a nullary constructor, prior to establishing the 
            // members of the type
            let preEstablishedHasDefaultCtor = 
                members |> List.exists (function 
                    | SynMemberSig.Member (synValSig, memberFlags, _) -> 
                        memberFlags.MemberKind=SynMemberKind.Constructor && 
                        // REVIEW: This is a syntactic approximation
                        (match synValSig.SynType, synValSig.SynInfo.CurriedArgInfos with 
                         | StripParenTypes (SynType.Fun (argType = StripParenTypes (SynType.LongIdent (SynLongIdent([id], _, _))))), [[_]] when id.idText = "unit" -> true
                         | _ -> false) 
                    | _ -> false) 

            let hasSelfReferentialCtor = false
            
            let repr = SynTypeDefnSimpleRepr.General(kind, inherits, slotsigs, fields, isConcrete, false, None, m)
            let isAtOriginalTyconDefn = true
            let tyconCore = MutRecDefnsPhase1DataForTycon (synTyconInfo, repr, implements2@implements1, preEstablishedHasDefaultCtor, hasSelfReferentialCtor, isAtOriginalTyconDefn)

            tyconCore, (synTyconInfo, members@extraMembers)

        // 'type X with ...' in a signature is always interpreted as an extrinsic extension.
        // Representation-hidden types with members and interfaces are written 'type X = ...' 
        | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.None _ as r, _) when not (isNil extraMembers) -> 
            let isAtOriginalTyconDefn = false
            let tyconCore = MutRecDefnsPhase1DataForTycon (synTyconInfo, r, implements1, false, false, isAtOriginalTyconDefn)
            tyconCore, (synTyconInfo, extraMembers)

        | SynTypeDefnSigRepr.Exception r -> 
            let isAtOriginalTyconDefn = true
            let core = MutRecDefnsPhase1DataForTycon(synTyconInfo, SynTypeDefnSimpleRepr.Exception r, implements1, false, false, isAtOriginalTyconDefn)
            core, (synTyconInfo, extraMembers)

        | SynTypeDefnSigRepr.Simple(r, _) -> 
            let isAtOriginalTyconDefn = true
            let tyconCore = MutRecDefnsPhase1DataForTycon (synTyconInfo, r, implements1, false, false, isAtOriginalTyconDefn)
            tyconCore, (synTyconInfo, extraMembers) 


    let private TcMutRecSignatureDecls_Phase2 (cenv: cenv) scopem envMutRec mutRecDefns =
        let g = cenv.g
        (envMutRec, mutRecDefns) ||> MutRecShapes.mapWithEnv 
            // Do this for the members in each 'type' declaration 
            (fun envForDecls ((tyconCore, (synTyconInfo, members), innerParent), tyconOpt, _fixupFinalAttrs, _) -> 
                let tpenv = emptyUnscopedTyparEnv
                let (MutRecDefnsPhase1DataForTycon (_, _, _, _, _, isAtOriginalTyconDefn)) = tyconCore
                let (SynComponentInfo(_, TyparsAndConstraints (typars, cs1), cs2, longPath, _, _, _, m)) = synTyconInfo
                let cs = cs1 @ cs2
                let declKind, tcref, declaredTyconTypars = ComputeTyconDeclKind cenv envForDecls tyconOpt isAtOriginalTyconDefn true m typars cs longPath

                let envForTycon = AddDeclaredTypars CheckForDuplicateTypars declaredTyconTypars envForDecls
                let envForTycon = MakeInnerEnvForTyconRef envForTycon tcref (declKind = ExtrinsicExtensionBinding) 

                TcTyconMemberSpecs cenv envForTycon (TyconContainerInfo(innerParent, tcref, declaredTyconTypars, NoSafeInitInfo)) declKind tpenv members)
            
            // Do this for each 'val' declaration in a module
            (fun envForDecls (containerInfo, valSpec) -> 
                let tpenv = emptyUnscopedTyparEnv
                let idvs, _ = TcAndPublishValSpec (cenv, envForDecls, containerInfo, ModuleOrMemberBinding, None, tpenv, valSpec)
                let env = List.foldBack (AddLocalVal g cenv.tcSink scopem) idvs envForDecls
                env)


    /// Bind a collection of mutually recursive declarations in a signature file
    let TcMutRecSignatureDecls (cenv: cenv) envInitial parent typeNames tpenv m scopem mutRecNSInfo (mutRecSigs: MutRecSigsInitialData) =
        let mutRecSigsAfterSplit = mutRecSigs |> MutRecShapes.mapTycons SplitTyconSignature
        let _tycons, envMutRec, mutRecDefnsAfterCore =
            EstablishTypeDefinitionCores.TcMutRecDefns_Phase1
                (fun containerInfo valDecl -> (containerInfo, valDecl))
                cenv envInitial parent typeNames true tpenv m scopem mutRecNSInfo mutRecSigsAfterSplit

        // Updates the types of the modules to contain the contents so far, which now includes values and members
        MutRecBindingChecking.TcMutRecDefns_UpdateModuleContents mutRecNSInfo mutRecDefnsAfterCore

        // By now we've established the full contents of type definitions apart from their
        // members and any fields determined by implicit construction. We know the kinds and 
        // representations of types and have established them as valid.
        //
        // We now reconstruct the active environments all over again - this will add the union cases and fields. 
        //
        // Note: This environment reconstruction doesn't seem necessary. We're about to create Val's for all members, 
        // which does require type checking, but no more information than is already available.
        let envMutRecPrelimWithReprs, withEnvs =  
            (envInitial, MutRecShapes.dropEnvs mutRecDefnsAfterCore) 
                ||> MutRecBindingChecking.TcMutRecDefns_ComputeEnvs 
                       (fun (_, tyconOpt, _, _) -> tyconOpt)  
                       (fun _binds -> [ (* no values are available yet *) ]) 
                       cenv true scopem m 

        let mutRecDefnsAfterVals = TcMutRecSignatureDecls_Phase2 cenv scopem envMutRecPrelimWithReprs withEnvs

        // Updates the types of the modules to contain the contents so far, which now includes values and members
        MutRecBindingChecking.TcMutRecDefns_UpdateModuleContents mutRecNSInfo mutRecDefnsAfterVals

        envMutRec

//-------------------------------------------------------------------------
// Bind module types
//------------------------------------------------------------------------- 

let rec TcSignatureElementNonMutRec (cenv: cenv) parent typeNames endm (env: TcEnv) synSigDecl: Cancellable<TcEnv> =
  cancellable {
    let g = cenv.g
    try 
        match synSigDecl with 
        | SynModuleSigDecl.Exception (edef, m) ->
            let scopem = unionRanges m.EndRange endm
            let _, _, _, env = TcExceptionDeclarations.TcExnSignature cenv env parent emptyUnscopedTyparEnv (edef, scopem)
            return env

        | SynModuleSigDecl.Types (typeSpecs, m) -> 
            let scopem = unionRanges m endm
            let mutRecDefns = typeSpecs |> List.map MutRecShape.Tycon
            let env = TcDeclarations.TcMutRecSignatureDecls cenv env parent typeNames emptyUnscopedTyparEnv m scopem None mutRecDefns
            return env 

        | SynModuleSigDecl.Open (target, m) -> 
            let scopem = unionRanges m.EndRange endm
            let env, _openDecl = TcOpenDecl cenv m scopem env target
            return env

        | SynModuleSigDecl.Val (vspec, m) -> 
            let parentModule = 
                match parent with 
                | ParentNone -> error(Error(FSComp.SR.tcNamespaceCannotContainValues(), vspec.RangeOfId)) 
                | Parent p -> p
            let containerInfo = ModuleOrNamespaceContainerInfo parentModule
            let idvs, _ = TcAndPublishValSpec (cenv, env, containerInfo, ModuleOrMemberBinding, None, emptyUnscopedTyparEnv, vspec)
            let scopem = unionRanges m endm
            let env = List.foldBack (AddLocalVal g cenv.tcSink scopem) idvs env
            return env

        | SynModuleSigDecl.NestedModule(SynComponentInfo(attributes=Attributes attribs; longId=longPath; xmlDoc=xml; accessibility=vis; range=im) as compInfo, isRec, moduleDefs, m, trivia) ->
            if isRec then 
                // Treat 'module rec M = ...' as a single mutually recursive definition group 'module M = ...'
                let modDecl = SynModuleSigDecl.NestedModule(compInfo, false, moduleDefs, m, trivia)

                return! TcSignatureElementsMutRec cenv parent typeNames endm None env [modDecl]
            else
                let id = ComputeModuleName longPath
                let vis, _ = ComputeAccessAndCompPath env None im vis None parent
                let attribs = TcAttributes cenv env AttributeTargets.ModuleDecl attribs
                CheckNamespaceModuleOrTypeName g id
                let moduleKind = EstablishTypeDefinitionCores.ComputeModuleOrNamespaceKind g true typeNames attribs id.idText
                let modName = EstablishTypeDefinitionCores.AdjustModuleName moduleKind id.idText
                CheckForDuplicateConcreteType env modName id.idRange

                // Now typecheck the signature, accumulating and then recording the submodule description. 
                let id = ident (modName, id.idRange)

                let moduleTy = Construct.NewEmptyModuleOrNamespaceType moduleKind
                let xmlDoc = xml.ToXmlDoc(true, Some [])
                let moduleEntity = Construct.NewModuleOrNamespace (Some env.eCompPath) vis id xmlDoc attribs (MaybeLazy.Strict moduleTy) 

                let! moduleTy, _ = TcModuleOrNamespaceSignatureElementsNonMutRec cenv (Parent (mkLocalModuleRef moduleEntity)) env (id, moduleKind, moduleDefs, m, xml)

                moduleEntity.entity_modul_type <- MaybeLazy.Strict moduleTy 
                let scopem = unionRanges m endm
                PublishModuleDefn cenv env moduleEntity
                let env = AddLocalSubModuleAndReport cenv.tcSink scopem g cenv.amap m env moduleEntity
                return env
            
        | SynModuleSigDecl.ModuleAbbrev (id, p, m) -> 
            let ad = env.AccessRights
            let resolved =
                match p with
                | [] -> Result []
                | id :: rest -> ResolveLongIdentAsModuleOrNamespace cenv.tcSink ResultCollectionSettings.AllResults cenv.amap m true OpenQualified env.NameEnv ad id rest false
            let mvvs = ForceRaise resolved
            let scopem = unionRanges m endm
            let unfilteredModrefs = mvvs |> List.map p23
            
            let modrefs = unfilteredModrefs |> List.filter (fun modref -> not modref.IsNamespace)

            if not (List.isEmpty unfilteredModrefs) && List.isEmpty modrefs then 
                errorR(Error(FSComp.SR.tcModuleAbbreviationForNamespace(fullDisplayTextOfModRef (List.head unfilteredModrefs)), m))
            
            if List.isEmpty modrefs then return env else
            modrefs |> List.iter (fun modref -> CheckEntityAttributes g modref m |> CommitOperationResult)        
            
            let env = AddModuleAbbreviationAndReport cenv.tcSink scopem id modrefs env 
            return env

        | SynModuleSigDecl.HashDirective _ -> 
            return env


        | SynModuleSigDecl.NamespaceFragment (SynModuleOrNamespaceSig(longId, isRec, kind, defs, xml, attribs, vis, m, _)) -> 

            do for id in longId do 
                 CheckNamespaceModuleOrTypeName g id

            // Logically speaking, this changes 
            //    module [rec] A.B.M
            //    ...
            // to 
            //    namespace [rec] A.B
            //      module M = ...
            let enclosingNamespacePath, defs = 
                if kind.IsModule then 
                    let nsp, modName = List.frontAndBack longId
                    let modDecl = [SynModuleSigDecl.NestedModule(SynComponentInfo(attribs, None, [], [modName], xml, false, vis, m), false, defs, m, SynModuleSigDeclNestedModuleTrivia.Zero)] 
                    nsp, modDecl
                else 
                    longId, defs

            let envNS = LocateEnv kind.IsModule cenv.thisCcu env enclosingNamespacePath
            let envNS = ImplicitlyOpenOwnNamespace cenv.tcSink g cenv.amap m enclosingNamespacePath envNS

            // For 'namespace rec' and 'module rec' we add the thing being defined 
            let modTyNS = envNS.eModuleOrNamespaceTypeAccumulator.Value
            let modTyRoot, modulNSs = BuildRootModuleType enclosingNamespacePath envNS.eCompPath modTyNS
            let modulNSOpt = List.tryHead modulNSs

            modulNSs |> List.iter (fun moduleEntity ->
                let modref = mkLocalModuleRef moduleEntity
                let item = Item.ModuleOrNamespaces [modref]
                CallNameResolutionSink cenv.tcSink (moduleEntity.Range, env.NameEnv, item, emptyTyparInst, ItemOccurence.Binding, env.AccessRights))

            // For 'namespace rec' and 'module rec' we add the thing being defined 
            let envNS = if isRec then AddLocalRootModuleOrNamespace cenv.tcSink g cenv.amap m envNS modTyRoot else envNS
            let nsInfo = Some (modulNSOpt, envNS.eModuleOrNamespaceTypeAccumulator) 
            let mutRecNSInfo = if isRec then nsInfo else None

            let! envAtEnd = TcSignatureElements cenv ParentNone m.EndRange envNS xml mutRecNSInfo defs

            MutRecBindingChecking.TcMutRecDefns_UpdateNSContents nsInfo

            let env = 
                if isNil enclosingNamespacePath then 
                    envAtEnd
                else
                    let env = AddLocalRootModuleOrNamespace cenv.tcSink g cenv.amap m env modTyRoot

                    // If the namespace is an interactive fragment e.g. FSI_0002, then open FSI_0002 in the subsequent environment.
                    let env, _openDecls = 
                        match TryStripPrefixPath g enclosingNamespacePath with 
                        | Some(p, _) -> TcOpenModuleOrNamespaceDecl cenv.tcSink g cenv.amap m.EndRange env ([p], m.EndRange)
                        | None -> env, []

                    // Publish the combined module type
                    env.eModuleOrNamespaceTypeAccumulator.Value <- 
                        CombineCcuContentFragments m [env.eModuleOrNamespaceTypeAccumulator.Value; modTyRoot]
                    env

            return env
            
    with exn -> 
        errorRecovery exn endm 
        return env
  }


and TcSignatureElements cenv parent endm env xml mutRecNSInfo defs = 
    cancellable {
        // Ensure the .Deref call in UpdateAccModuleOrNamespaceType succeeds 
        if cenv.compilingCanonicalFslibModuleType then 
            let xmlDoc = xml.ToXmlDoc(true, Some [])
            ensureCcuHasModuleOrNamespaceAtPath cenv.thisCcu env.ePath env.eCompPath xmlDoc

        let typeNames = EstablishTypeDefinitionCores.TypeNamesInNonMutRecSigDecls defs
        match mutRecNSInfo with 
        | Some _ ->
            return! TcSignatureElementsMutRec cenv parent typeNames endm mutRecNSInfo env defs
        | None ->
            return! TcSignatureElementsNonMutRec cenv parent typeNames endm env defs
    }

and TcSignatureElementsNonMutRec cenv parent typeNames endm env defs = 
    Cancellable.fold (TcSignatureElementNonMutRec cenv parent typeNames endm) env defs

and TcSignatureElementsMutRec cenv parent typeNames m mutRecNSInfo envInitial (defs: SynModuleSigDecl list) =
    cancellable {
        let m = match defs with [] -> m | _ -> defs |> List.map (fun d -> d.Range) |> List.reduce unionRanges
        let scopem = (defs, m) ||> List.foldBack (fun h m -> unionRanges h.Range m) 

        let mutRecDefns = 
          let rec loop isNamespace moduleRange defs: MutRecSigsInitialData = 
            ((true, true), defs) ||> List.collectFold (fun (openOk, moduleAbbrevOk) def -> 
                match def with 
                | SynModuleSigDecl.Types (typeSpecs, _) -> 
                    let decls = typeSpecs |> List.map MutRecShape.Tycon
                    decls, (false, false)

                | SynModuleSigDecl.Open (target, m) -> 
                      if not openOk then errorR(Error(FSComp.SR.tcOpenFirstInMutRec(), m))
                      let decls = [ MutRecShape.Open (MutRecDataForOpen(target, m, moduleRange, ref [])) ]
                      decls, (openOk, moduleAbbrevOk)

                | SynModuleSigDecl.Exception (exnSig=SynExceptionSig(exnRepr=exnRepr; withKeyword=withKeyword; members=members)) ->
                      let ( SynExceptionDefnRepr(synAttrs, SynUnionCase(ident=SynIdent(id,_)), _, xmlDoc, vis, m)) = exnRepr
                      let compInfo = SynComponentInfo(synAttrs, None, [], [id], xmlDoc, false, vis, id.idRange)
                      let decls = [ MutRecShape.Tycon(SynTypeDefnSig.SynTypeDefnSig(compInfo, None, SynTypeDefnSigRepr.Exception exnRepr, withKeyword, members, m)) ]
                      decls, (false, false)

                | SynModuleSigDecl.Val (vspec, _) -> 
                    if isNamespace then error(Error(FSComp.SR.tcNamespaceCannotContainValues(), vspec.RangeOfId)) 
                    let decls = [ MutRecShape.Lets vspec ]
                    decls, (false, false)

                | SynModuleSigDecl.NestedModule(moduleInfo=compInfo; isRecursive=isRec; moduleDecls=synDefs; range=moduleRange) ->
                      if isRec then warning(Error(FSComp.SR.tcRecImplied(), compInfo.Range))
                      let mutRecDefs = loop false moduleRange synDefs
                      let decls = [MutRecShape.Module (compInfo, mutRecDefs)]
                      decls, (false, false)

                | SynModuleSigDecl.HashDirective _ -> 
                      [], (openOk, moduleAbbrevOk)

                | SynModuleSigDecl.ModuleAbbrev (id, p, m) ->
                      if not moduleAbbrevOk then errorR(Error(FSComp.SR.tcModuleAbbrevFirstInMutRec(), m))
                      let decls = [ MutRecShape.ModuleAbbrev (MutRecDataForModuleAbbrev(id, p, m)) ]
                      decls, (false, moduleAbbrevOk)

                | SynModuleSigDecl.NamespaceFragment _ ->
                    error(Error(FSComp.SR.tcUnsupportedMutRecDecl(), def.Range)))

              |> fst
          loop (match parent with ParentNone -> true | Parent _ -> false) m defs
        return TcDeclarations.TcMutRecSignatureDecls cenv envInitial parent typeNames emptyUnscopedTyparEnv m scopem mutRecNSInfo mutRecDefns
    }



and TcModuleOrNamespaceSignatureElementsNonMutRec cenv parent env (id, moduleKind, defs, m: range, xml) =

  cancellable {
    let endm = m.EndRange // use end of range for errors 

    // Create the module type that will hold the results of type checking.... 
    let envForModule, moduleTyAcc = MakeInnerEnv true env id moduleKind

    // Now typecheck the signature, using mutation to fill in the submodule description. 
    let! envAtEnd = TcSignatureElements cenv parent endm envForModule xml None defs
    
    // moduleTyAcc has now accumulated the module type 
    return moduleTyAcc.Value, envAtEnd
  }
    
//-------------------------------------------------------------------------
// Bind definitions within modules
//------------------------------------------------------------------------- 

/// Removes SynModuleDecl.Expr in favour of a SynModuleDecl.Let with a SynBindingKind.StandaloneExpression
let ElimSynModuleDeclExpr bind =
    match bind with 
    | SynModuleDecl.Expr (expr, m) -> 
        let bind2 = SynBinding (None, SynBindingKind.StandaloneExpression, false, false, [], PreXmlDoc.Empty, SynInfo.emptySynValData, SynPat.Wild m, None, expr, m, DebugPointAtBinding.NoneAtDo, SynBindingTrivia.Zero)
        SynModuleDecl.Let(false, [bind2], m)
    | _ -> bind

let TcMutRecDefnsEscapeCheck (binds: MutRecShapes<_, _, _>) env = 
    let freeInEnv = GeneralizationHelpers.ComputeUnabstractableTycons env
    let checkTycon (tycon: Tycon) = 
        if not tycon.IsTypeAbbrev && Zset.contains tycon freeInEnv then 
            let nm = tycon.DisplayName
            errorR(Error(FSComp.SR.tcTypeUsedInInvalidWay(nm, nm, nm), tycon.Range))

    binds |> MutRecShapes.iterTycons (fst >> Option.iter checkTycon) 

    let freeInEnv = GeneralizationHelpers.ComputeUnabstractableTraitSolutions env
    let checkBinds (binds: Binding list) = 
        for bind in binds do 
            if Zset.contains bind.Var freeInEnv then 
                let nm = bind.Var.DisplayName 
                errorR(Error(FSComp.SR.tcMemberUsedInInvalidWay(nm, nm, nm), bind.Var.Range))

    binds |> MutRecShapes.iterTyconsAndLets (snd >> checkBinds) checkBinds 

// ignore solitary '()' expressions and 'do ()' bindings, since these are allowed in namespaces
// for the purposes of attaching attributes to an assembly, e.g. 
//   namespace A.B.C
//     [<assembly: Foo >]
//     do()

let CheckLetOrDoInNamespace binds m =
    match binds with 
    | [ SynBinding (accessibility=None; kind=(SynBindingKind.StandaloneExpression | SynBindingKind.Do); isInline=false; isMutable=false; attributes=[]; returnInfo=None; expr=(SynExpr.Do (expr=SynExpr.Const (constant=SynConst.Unit)) | SynExpr.Const (constant=SynConst.Unit))) ] ->
        ()
    | [] -> 
        error(Error(FSComp.SR.tcNamespaceCannotContainValues(), m)) 
    | _ -> 
        error(Error(FSComp.SR.tcNamespaceCannotContainValues(), binds.Head.RangeOfHeadPattern)) 

/// The non-mutually recursive case for a declaration
let rec TcModuleOrNamespaceElementNonMutRec (cenv: cenv) parent typeNames scopem env synDecl = 
  cancellable {
    let g = cenv.g
    cenv.synArgNameGenerator.Reset()
    let tpenv = emptyUnscopedTyparEnv

    //printfn "----------\nCHECKING, e = %+A\n------------------\n" e
    try 
      match ElimSynModuleDeclExpr synDecl with 

      | SynModuleDecl.ModuleAbbrev (id, p, m) -> 
          let env = MutRecBindingChecking.TcModuleAbbrevDecl cenv scopem env (id, p, m)
          return ([], [], []), env, env

      | SynModuleDecl.Exception (edef, m) -> 
          let binds, decl, env = TcExceptionDeclarations.TcExnDefn cenv env parent (edef, scopem)
          let defn = TMDefRec(true, [], [decl], binds |> List.map ModuleOrNamespaceBinding.Binding, m)
          return ([defn], [], []), env, env

      | SynModuleDecl.Types (typeDefs, m) -> 
          let scopem = unionRanges m scopem
          let mutRecDefns = typeDefs |> List.map MutRecShape.Tycon
          let mutRecDefnsChecked, envAfter = TcDeclarations.TcMutRecDefinitions cenv env parent typeNames tpenv m scopem None mutRecDefns
          // Check the non-escaping condition as we build the expression on the way back up 
          let defn = TcMutRecDefsFinish cenv mutRecDefnsChecked m
          let escapeCheck () = 
              TcMutRecDefnsEscapeCheck mutRecDefnsChecked env

          return ([defn], [escapeCheck], []), envAfter, envAfter

      | SynModuleDecl.Open (target, m) -> 
          let scopem = unionRanges m.EndRange scopem
          let env, openDecls = TcOpenDecl cenv m scopem env target
          let defns =
              match openDecls with
              | [] -> []
              | _ -> [ TMDefOpens openDecls ]
          return (defns, [], []), env, env

      | SynModuleDecl.Let (letrec, binds, m) -> 

          match parent with
          | ParentNone ->
                CheckLetOrDoInNamespace binds m
                return ([], [], []), env, env

          | Parent parentModule -> 
              let containerInfo = ModuleOrNamespaceContainerInfo parentModule
              if letrec then 
                let scopem = unionRanges m scopem
                let binds = binds |> List.map (fun bind -> RecDefnBindingInfo(containerInfo, NoNewSlots, ModuleOrMemberBinding, bind))
                let binds, env, _ = TcLetrecBindings WarnOnOverrides cenv env tpenv (binds, m, scopem)
                let defn = TMDefRec(true, [], [], binds |> List.map ModuleOrNamespaceBinding.Binding, m)
                return ([defn], [], []), env, env
              else 
                let defns, env, _ = TcLetBindings cenv env containerInfo ModuleOrMemberBinding tpenv (binds, m, scopem)
                return (defns, [], []), env, env 

      | SynModuleDecl.Expr _ ->
          return! failwith "unreachable"

      | SynModuleDecl.Attributes (Attributes synAttrs, _) -> 
          let attrs, _ = TcAttributesWithPossibleTargets false cenv env AttributeTargets.Top synAttrs
          return ([], [], attrs), env, env

      | SynModuleDecl.HashDirective _ -> 
          return ([], [], []), env, env

      | SynModuleDecl.NestedModule(compInfo, isRec, moduleDefs, isContinuingModule, m, trivia) ->

          // Treat 'module rec M = ...' as a single mutually recursive definition group 'module M = ...'
          if isRec then 
              assert (not isContinuingModule)
              let modDecl = SynModuleDecl.NestedModule(compInfo, false, moduleDefs, isContinuingModule, m, trivia)
              return! TcModuleOrNamespaceElementsMutRec cenv parent typeNames m env None [modDecl]
          else
              let (SynComponentInfo(Attributes attribs, _, _, longPath, xml, _, vis, im)) = compInfo
              let id = ComputeModuleName longPath

              let modAttrs = TcAttributes cenv env AttributeTargets.ModuleDecl attribs
              let moduleKind = EstablishTypeDefinitionCores.ComputeModuleOrNamespaceKind g true typeNames modAttrs id.idText
              let modName = EstablishTypeDefinitionCores.AdjustModuleName moduleKind id.idText
              CheckForDuplicateConcreteType env modName im
              CheckForDuplicateModule env id.idText id.idRange
              let vis, _ = ComputeAccessAndCompPath env None id.idRange vis None parent
             
              let endm = m.EndRange
              let id = ident (modName, id.idRange)

              CheckNamespaceModuleOrTypeName g id

              let envForModule, moduleTyAcc = MakeInnerEnv true env id moduleKind
    
              // Create the new module specification to hold the accumulated results of the type of the module 
              // Also record this in the environment as the accumulator 
              let moduleTy = Construct.NewEmptyModuleOrNamespaceType moduleKind
              let xmlDoc = xml.ToXmlDoc(true, Some [])
              let moduleEntity = Construct.NewModuleOrNamespace (Some env.eCompPath) vis id xmlDoc modAttrs (MaybeLazy.Strict moduleTy)

              // Now typecheck. 
              let! moduleContents, topAttrsNew, envAtEnd = TcModuleOrNamespaceElements cenv (Parent (mkLocalModuleRef moduleEntity)) endm envForModule xml None [] moduleDefs 

              // Get the inferred type of the decls and record it in the modul. 
              moduleEntity.entity_modul_type <- MaybeLazy.Strict moduleTyAcc.Value
              let moduleDef = TMDefRec(false, [], [], [ModuleOrNamespaceBinding.Module(moduleEntity, moduleContents)], m)

              PublishModuleDefn cenv env moduleEntity 

              let env = AddLocalSubModuleAndReport cenv.tcSink scopem g cenv.amap m env moduleEntity
          
              // isContinuingModule is true for all of the following
              //   - the implicit module of a script 
              //   - the major 'module' declaration for a file stating with 'module X.Y' 
              //   - an interactive entry for F# Interactive 
              //
              // In this case the envAtEnd is the environment at the end of this module, which doesn't contain the module definition itself
              // but does contain the results of all the 'open' declarations and so on.
              let envAtEnd = (if isContinuingModule then envAtEnd else env)
          
              return ([ moduleDef ], [], topAttrsNew), env, envAtEnd
      

      | SynModuleDecl.NamespaceFragment(SynModuleOrNamespace(longId, isRec, kind, defs, xml, attribs, vis, m, _)) ->

          let endm = m.EndRange

          do for id in longId do 
               CheckNamespaceModuleOrTypeName g id

          // Logically speaking, this changes 
          //    module [rec] A.B.M
          //    ...
          // to 
          //    namespace [rec] A.B
          //      module M = ...
          let enclosingNamespacePath, defs = 
              if kind.IsModule then 
                  let nsp, modName = List.frontAndBack longId
                  let modDecl = [SynModuleDecl.NestedModule(SynComponentInfo(attribs, None, [], [modName], xml, false, vis, m), false, defs, true, m, SynModuleDeclNestedModuleTrivia.Zero)] 
                  nsp, modDecl
              else 
                  longId, defs

          let envNS = LocateEnv kind.IsModule cenv.thisCcu env enclosingNamespacePath
          let envNS = ImplicitlyOpenOwnNamespace cenv.tcSink g cenv.amap m enclosingNamespacePath envNS

          let modTyNS = envNS.eModuleOrNamespaceTypeAccumulator.Value
          let modTyRoot, modulNSs = BuildRootModuleType enclosingNamespacePath envNS.eCompPath modTyNS
          let modulNSOpt = List.tryHead modulNSs

          modulNSs |> List.iter (fun moduleEntity ->
            let modref = mkLocalModuleRef moduleEntity
            let item = Item.ModuleOrNamespaces [modref]
            CallNameResolutionSink cenv.tcSink (moduleEntity.Range, env.NameEnv, item, emptyTyparInst, ItemOccurence.Binding, env.AccessRights))

          // For 'namespace rec' and 'module rec' we add the thing being defined 
          let envNS = if isRec then AddLocalRootModuleOrNamespace cenv.tcSink g cenv.amap m envNS modTyRoot else envNS
          let nsInfo = Some (modulNSOpt, envNS.eModuleOrNamespaceTypeAccumulator)
          let mutRecNSInfo = if isRec then nsInfo else None

          let! moduleContents, topAttrs, envAtEnd = TcModuleOrNamespaceElements cenv parent endm envNS xml mutRecNSInfo [] defs

          MutRecBindingChecking.TcMutRecDefns_UpdateNSContents nsInfo
          
          let env, openDecls = 
              if isNil enclosingNamespacePath then 
                  envAtEnd, []
              else
                  let env = AddLocalRootModuleOrNamespace cenv.tcSink g cenv.amap m env modTyRoot

                  // If the namespace is an interactive fragment e.g. FSI_0002, then open FSI_0002 in the subsequent environment
                  let env, openDecls = 
                      match TryStripPrefixPath g enclosingNamespacePath with 
                      | Some(p, _) -> TcOpenModuleOrNamespaceDecl cenv.tcSink g cenv.amap m.EndRange env ([p], m.EndRange)
                      | None -> env, []

                  // Publish the combined module type
                  env.eModuleOrNamespaceTypeAccumulator.Value <-
                      CombineCcuContentFragments m [env.eModuleOrNamespaceTypeAccumulator.Value; modTyRoot]
                  env, openDecls
          
          let moduleContentsRoot = BuildRootModuleContents kind.IsModule enclosingNamespacePath envNS.eCompPath moduleContents

          let defns =
              match openDecls with 
              | [] -> [ moduleContentsRoot ]
              | _ -> [ TMDefOpens openDecls; moduleContentsRoot ]

          return 
              (defns, [], topAttrs), env, envAtEnd

    with exn -> 
        errorRecovery exn synDecl.Range 
        return ([], [], []), env, env
 }
 
/// The non-mutually recursive case for a sequence of declarations
and TcModuleOrNamespaceElementsNonMutRec cenv parent typeNames endm (defsSoFar, env, envAtEnd) (moreDefs: SynModuleDecl list) =
 cancellable {
    match moreDefs with 
    | firstDef :: otherDefs ->
        // Lookahead one to find out the scope of the next declaration.
        let scopem = 
            if isNil otherDefs then unionRanges firstDef.Range endm
            else unionRanges (List.head otherDefs).Range endm

        let! firstDef, env, envAtEnd = TcModuleOrNamespaceElementNonMutRec cenv parent typeNames scopem env firstDef

        // tail recursive 
        return! TcModuleOrNamespaceElementsNonMutRec cenv parent typeNames endm ( (firstDef :: defsSoFar), env, envAtEnd) otherDefs
    | [] -> 
        return List.rev defsSoFar, envAtEnd
 }

/// The mutually recursive case for a sequence of declarations (and nested modules)
and TcModuleOrNamespaceElementsMutRec (cenv: cenv) parent typeNames m envInitial mutRecNSInfo (defs: SynModuleDecl list) =
 cancellable {

    let m = match defs with [] -> m | _ -> defs |> List.map (fun d -> d.Range) |> List.reduce unionRanges
    let scopem = (defs, m) ||> List.foldBack (fun h m -> unionRanges h.Range m) 

    let mutRecDefns, (_, _, Attributes synAttrs) = 
      let rec loop isNamespace moduleRange attrs defs: MutRecDefnsInitialData * _ = 
        ((true, true, attrs), defs) ||> List.collectFold (fun (openOk, moduleAbbrevOk, attrs) def -> 
            match ElimSynModuleDeclExpr def with

              | SynModuleDecl.Types (typeDefs, _) -> 
                  let decls = typeDefs |> List.map MutRecShape.Tycon
                  decls, (false, false, attrs)

              | SynModuleDecl.Let (letrec, binds, m) -> 
                  let binds = 
                      if isNamespace then 
                          CheckLetOrDoInNamespace binds m; []
                      else
                          if letrec then [MutRecShape.Lets binds]
                          else List.map (List.singleton >> MutRecShape.Lets) binds
                  binds, (false, false, attrs)

              | SynModuleDecl.NestedModule(moduleInfo=compInfo; isRecursive=isRec; decls=synDefs; range=moduleRange) -> 
                  if isRec then warning(Error(FSComp.SR.tcRecImplied(), compInfo.Range))
                  let mutRecDefs, (_, _, attrs) = loop false moduleRange attrs synDefs 
                  let decls = [MutRecShape.Module (compInfo, mutRecDefs)]
                  decls, (false, false, attrs)

              | SynModuleDecl.Open (target, m) ->  
                  if not openOk then errorR(Error(FSComp.SR.tcOpenFirstInMutRec(), m))
                  let decls = [ MutRecShape.Open (MutRecDataForOpen(target, m, moduleRange, ref [])) ]
                  decls, (openOk, moduleAbbrevOk, attrs)

              | SynModuleDecl.Exception (SynExceptionDefn(repr, _, members, _), _m) ->
                  let members = desugarGetSetMembers members
                  let (SynExceptionDefnRepr(synAttrs, SynUnionCase(ident=SynIdent(id,_)), _repr, xmlDoc, vis, m)) = repr
                  let compInfo = SynComponentInfo(synAttrs, None, [], [id], xmlDoc, false, vis, id.idRange)
                  let decls = [ MutRecShape.Tycon(SynTypeDefn(compInfo, SynTypeDefnRepr.Exception repr, members, None, m, SynTypeDefnTrivia.Zero)) ]
                  decls, (false, false, attrs)

              | SynModuleDecl.HashDirective _ -> 
                  [ ], (openOk, moduleAbbrevOk, attrs)

              | SynModuleDecl.Attributes (synAttrs, _) -> 
                  [ ], (false, false, synAttrs)

              | SynModuleDecl.ModuleAbbrev (id, p, m) ->
                  if not moduleAbbrevOk then errorR(Error(FSComp.SR.tcModuleAbbrevFirstInMutRec(), m))
                  let decls = [ MutRecShape.ModuleAbbrev (MutRecDataForModuleAbbrev(id, p, m)) ]
                  decls, (false, moduleAbbrevOk, attrs)

              | SynModuleDecl.Expr _ -> failwith "unreachable: SynModuleDecl.Expr - ElimSynModuleDeclExpr"

              | SynModuleDecl.NamespaceFragment _ as d -> error(Error(FSComp.SR.tcUnsupportedMutRecDecl(), d.Range)))

      loop (match parent with ParentNone -> true | Parent _ -> false) m [] defs

    let tpenv = emptyUnscopedTyparEnv 
    let mutRecDefnsChecked, envAfter = TcDeclarations.TcMutRecDefinitions cenv envInitial parent typeNames tpenv m scopem mutRecNSInfo mutRecDefns 

    // Check the assembly attributes
    let attrs, _ = TcAttributesWithPossibleTargets false cenv envAfter AttributeTargets.Top synAttrs

    // Check the non-escaping condition as we build the list of module expressions on the way back up 
    let moduleContents = TcMutRecDefsFinish cenv mutRecDefnsChecked m 
    let escapeCheck () = 
        TcMutRecDefnsEscapeCheck mutRecDefnsChecked envInitial

    return ([ moduleContents ], [ escapeCheck ], attrs), envAfter, envAfter

 }

and TcMutRecDefsFinish cenv defs m =
    let opens =
        [ for def in defs do
            match def with 
            | MutRecShape.Open (MutRecDataForOpen (_target, _m, _moduleRange, openDeclsRef)) ->
                yield! openDeclsRef.Value
            | _ -> () ]

    let tycons = defs |> List.choose (function MutRecShape.Tycon (Some tycon, _) -> Some tycon | _ -> None)

    let binds = 
        defs |> List.collect (function 
            | MutRecShape.Open _ -> []
            | MutRecShape.ModuleAbbrev _ -> []
            | MutRecShape.Tycon (_, binds) 
            | MutRecShape.Lets binds -> 
                binds |> List.map ModuleOrNamespaceBinding.Binding 
            | MutRecShape.Module ((MutRecDefnsPhase2DataForModule(moduleTyAcc, moduleEntity), _), moduleDefs) -> 
                let moduleContents = TcMutRecDefsFinish cenv moduleDefs m
                moduleEntity.entity_modul_type <- MaybeLazy.Strict moduleTyAcc.Value
                [ ModuleOrNamespaceBinding.Module(moduleEntity, moduleContents) ])

    TMDefRec(true, opens, tycons, binds, m)

and TcModuleOrNamespaceElements cenv parent endm env xml mutRecNSInfo openDecls0 synModuleDecls =
  cancellable {
    // Ensure the deref_nlpath call in UpdateAccModuleOrNamespaceType succeeds 
    if cenv.compilingCanonicalFslibModuleType then 
        let xmlDoc = xml.ToXmlDoc(true, Some [])
        ensureCcuHasModuleOrNamespaceAtPath cenv.thisCcu env.ePath env.eCompPath xmlDoc

    // Collect the type names so we can implicitly add the compilation suffix to module names
    let typeNames = EstablishTypeDefinitionCores.TypeNamesInNonMutRecDecls cenv env synModuleDecls

    match mutRecNSInfo with 
    | Some _ -> 
        let! (moduleDefs, escapeChecks, topAttrsNew), _, envAtEnd = TcModuleOrNamespaceElementsMutRec cenv parent typeNames endm env mutRecNSInfo synModuleDecls
        let moduleContents = TMDefs(moduleDefs) 
        // Run the escape checks (for compat run in reverse order)
        do 
          for escapeCheck in List.rev escapeChecks do
            escapeCheck()
        return (moduleContents, topAttrsNew, envAtEnd)

    | None -> 

        let! compiledDefs, envAtEnd = TcModuleOrNamespaceElementsNonMutRec cenv parent typeNames endm ([], env, env) synModuleDecls 

        // Apply the functions for each declaration to build the overall expression-builder 
        let moduleDefs = List.collect p13 compiledDefs
        let moduleDefs = match openDecls0 with [] -> moduleDefs | _ -> TMDefOpens openDecls0 :: moduleDefs
        let moduleContents = TMDefs moduleDefs 

        // Collect up the attributes that are global to the file 
        let topAttrsNew = compiledDefs |> List.map p33 |> List.concat
        return (moduleContents, topAttrsNew, envAtEnd)
  }  
    

//--------------------------------------------------------------------------
// CheckOneImplFile - Typecheck all the namespace fragments in a file.
//-------------------------------------------------------------------------- 


let ApplyAssemblyLevelAutoOpenAttributeToTcEnv g amap (ccu: CcuThunk) scopem env (p, root) = 
    let warn() = 
        warning(Error(FSComp.SR.tcAttributeAutoOpenWasIgnored(p, ccu.AssemblyName), scopem))
        [], env
    let p = splitNamespace p 
    match List.tryFrontAndBack p with
    | None -> warn()
    | Some (h, t) ->
        let modref = mkNonLocalTyconRef (mkNonLocalEntityRef ccu (Array.ofList h)) t
        match modref.TryDeref with 
        | ValueNone -> warn()
        | ValueSome _ -> 
            let openTarget = SynOpenDeclTarget.ModuleOrNamespace([], scopem)
            let openDecl = OpenDeclaration.Create (openTarget, [modref], [], scopem, false)
            let envinner = OpenModuleOrNamespaceRefs TcResultsSink.NoSink g amap scopem root env [modref] openDecl
            [openDecl], envinner

// Add the CCU and apply the "AutoOpen" attributes
let AddCcuToTcEnv (g, amap, scopem, env, assemblyName, ccu, autoOpens, internalsVisibleToAttributes) = 
    let env = AddNonLocalCcu g amap scopem env assemblyName (ccu, internalsVisibleToAttributes)

    // See https://fslang.uservoice.com/forums/245727-f-language/suggestions/6107641-make-microsoft-prefix-optional-when-using-core-f
    // "Microsoft" is opened by default in FSharp.Core
    let autoOpens = 
        let autoOpens = autoOpens |> List.map (fun p -> (p, false))
        if ccuEq ccu g.fslibCcu then 
            // Auto open 'Microsoft' in FSharp.Core.dll. Even when using old versions of FSharp.Core.dll that do
            // not have this attribute. The 'true' means 'treat all namespaces so revealed as "roots" accessible via
            // global, e.g. global.FSharp.Collections'
            ("Microsoft", true) :: autoOpens
        else 
            autoOpens

    (env, autoOpens) ||> List.collectFold (ApplyAssemblyLevelAutoOpenAttributeToTcEnv g amap ccu scopem)

let emptyTcEnv g =
    let cpath = compPathInternal // allow internal access initially
    { eNameResEnv = NameResolutionEnv.Empty g
      eUngeneralizableItems = []
      ePath = []
      eCompPath = cpath // dummy 
      eAccessPath = cpath // dummy 
      eAccessRights = ComputeAccessRights cpath [] None // compute this field 
      eInternalsVisibleCompPaths = []
      eContextInfo = ContextInfo.NoContext
      eModuleOrNamespaceTypeAccumulator = ref (Construct.NewEmptyModuleOrNamespaceType (Namespace true))
      eFamilyType = None
      eCtorInfo = None
      eCallerMemberName = None 
      eLambdaArgInfos = []
      eIsControlFlow = false }

let CreateInitialTcEnv(g, amap, scopem, assemblyName, ccus) =
    (emptyTcEnv g, ccus) ||> List.collectFold (fun env (ccu, autoOpens, internalsVisible) -> 
        try 
            AddCcuToTcEnv(g, amap, scopem, env, assemblyName, ccu, autoOpens, internalsVisible)
        with exn -> 
            errorRecovery exn scopem 
            [], env) 

type ConditionalDefines = 
    string list


/// The attributes that don't get attached to any declaration
type TopAttribs =
    { mainMethodAttrs: Attribs
      netModuleAttrs: Attribs
      assemblyAttrs: Attribs }

let EmptyTopAttrs =
    { mainMethodAttrs=[]
      netModuleAttrs=[]
      assemblyAttrs =[] }

let CombineTopAttrs topAttrs1 topAttrs2 =
    { mainMethodAttrs = topAttrs1.mainMethodAttrs @ topAttrs2.mainMethodAttrs
      netModuleAttrs = topAttrs1.netModuleAttrs @ topAttrs2.netModuleAttrs
      assemblyAttrs = topAttrs1.assemblyAttrs @ topAttrs2.assemblyAttrs } 

let rec IterTyconsOfModuleOrNamespaceType f (mty: ModuleOrNamespaceType) = 
    mty.AllEntities |> QueueList.iter (fun tycon -> f tycon)
    mty.ModuleAndNamespaceDefinitions |> List.iter (fun v -> 
        IterTyconsOfModuleOrNamespaceType f v.ModuleOrNamespaceType)


// Defaults get applied before the module signature is checked and before the implementation conditions on virtuals/overrides. 
// Defaults get applied in priority order. Defaults listed last get priority 0 (lowest), 2nd last priority 1 etc. 
let ApplyDefaults (cenv: cenv) g denvAtEnd m moduleContents extraAttribs = 
    try
        let unsolved = FindUnsolved.UnsolvedTyparsOfModuleDef g cenv.amap denvAtEnd moduleContents extraAttribs

        CanonicalizePartialInferenceProblem cenv.css denvAtEnd m unsolved

        // The priority order comes from the order of declaration of the defaults in FSharp.Core.
        for priority = 10 downto 0 do
            unsolved |> List.iter (fun tp -> 
                if not tp.IsSolved then 
                    // Apply the first default. If we're defaulting one type variable to another then 
                    // the defaults will be propagated to the new type variable. 
                    ApplyTyparDefaultAtPriority denvAtEnd cenv.css priority tp)

        // OK, now apply defaults for any unsolved TyparStaticReq.HeadType 
        unsolved |> List.iter (fun tp ->     
            if not tp.IsSolved then 
                if (tp.StaticReq <> TyparStaticReq.None) then
                    ChooseTyparSolutionAndSolve cenv.css denvAtEnd tp)
    with exn ->
        errorRecovery exn m

let CheckValueRestriction denvAtEnd infoReader rootSigOpt implFileTypePriorToSig m = 
    if Option.isNone rootSigOpt then
      let rec check (mty: ModuleOrNamespaceType) =
          for v in mty.AllValsAndMembers do
              let ftyvs = (freeInVal CollectTyparsNoCaching v).FreeTypars |> Zset.elements
              if (not v.IsCompilerGenerated && 
                  not (ftyvs |> List.exists (fun tp -> tp.IsFromError)) && 
                  // Do not apply the value restriction to methods and functions
                  // Note, normally these completely generalize their argument types anyway. However, 
                  // some methods (property getters/setters, constructors) can't be as generic
                  // as they might naturally be, and these can leave type variables unsolved. See
                  // for example FSharp 1.0 3661.
                  (match v.ValReprInfo with None -> true | Some tvi -> tvi.HasNoArgs)) then 
                match ftyvs with 
                | tp :: _ -> errorR (ValueRestriction(denvAtEnd, infoReader, false, v, tp, v.Range))
                | _ -> ()
          mty.ModuleAndNamespaceDefinitions |> List.iter (fun v -> check v.ModuleOrNamespaceType) 
      try check implFileTypePriorToSig with e -> errorRecovery e m


let SolveInternalUnknowns g (cenv: cenv) denvAtEnd moduleContents extraAttribs =
    let unsolved = FindUnsolved.UnsolvedTyparsOfModuleDef g cenv.amap denvAtEnd moduleContents extraAttribs

    for tp in unsolved do
        if (tp.Rigidity <> TyparRigidity.Rigid) && not tp.IsSolved then 
            ChooseTyparSolutionAndSolve cenv.css denvAtEnd tp

let CheckModuleSignature g (cenv: cenv) m denvAtEnd rootSigOpt implFileTypePriorToSig implFileSpecPriorToSig moduleContents =
    match rootSigOpt with 
    | None -> 
        // Deep copy the inferred type of the module 
        let implFileTypePriorToSigCopied = copyModuleOrNamespaceType g CloneAll implFileTypePriorToSig

        (implFileTypePriorToSigCopied, moduleContents)
            
    | Some sigFileType -> 

        // We want to show imperative type variables in any types in error messages at this late point 
        let denv = { denvAtEnd with showInferenceTyparAnnotations=true }
        try 
                
            // As typechecked the signature and implementation use different tycons etc. 
            // Here we (a) check there are enough names, (b) match them up to build a renaming and   
            // (c) check signature conformance up to this renaming. 
            if not (SignatureConformance.CheckNamesOfModuleOrNamespace denv cenv.infoReader (mkLocalTyconRef implFileSpecPriorToSig) sigFileType) then 
                raise (ReportedError None)

            // Compute the remapping from implementation to signature
            let remapInfo, _ = ComputeRemappingFromInferredSignatureToExplicitSignature g implFileTypePriorToSig sigFileType
                     
            let aenv = { TypeEquivEnv.Empty with EquivTycons = TyconRefMap.OfList remapInfo.RepackagedEntities }
                    
            if not (SignatureConformance.Checker(g, cenv.amap, denv, remapInfo, true).CheckSignature aenv cenv.infoReader (mkLocalModuleRef implFileSpecPriorToSig) sigFileType) then
                // We can just raise 'ReportedError' since CheckModuleOrNamespace raises its own error 
                raise (ReportedError None)
        with exn ->
            errorRecovery exn m
            
        (sigFileType, moduleContents)


/// Make the initial type checking environment for a single file with an empty accumulator for the overall contents for the file
let MakeInitialEnv env = 
    // Note: here we allocate a new module type accumulator 
    let moduleTyAcc = ref (Construct.NewEmptyModuleOrNamespaceType (Namespace false))
    { env with eModuleOrNamespaceTypeAccumulator = moduleTyAcc }, moduleTyAcc

/// Check an entire implementation file
/// Typecheck, then close the inference scope and then check the file meets its signature (if any)
let CheckOneImplFile 
       // checkForErrors: A function to help us stop reporting cascading errors 
       (g, niceNameGen, amap,
        thisCcu,
        openDecls0,
        checkForErrors,
        conditionalDefines,
        tcSink,
        isInternalTestSpanStackReferring,
        env,
        rootSigOpt: ModuleOrNamespaceType option,
        synImplFile) =

    let (ParsedImplFileInput (_, isScript, qualNameOfFile, scopedPragmas, _, implFileFrags, isLastCompiland, _)) = synImplFile
    let infoReader = InfoReader(g, amap)

    cancellable {
        let cenv = 
            cenv.Create (g, isScript, niceNameGen, amap, thisCcu, false, Option.isSome rootSigOpt,
                conditionalDefines, tcSink, (LightweightTcValForUsingInBuildMethodCall g), isInternalTestSpanStackReferring,
                tcPat=TcPat,
                tcSimplePats=TcSimplePats,
                tcSequenceExpressionEntry=TcSequenceExpressionEntry,
                tcArrayOrListSequenceExpression=TcArrayOrListComputedExpression,
                tcComputationExpression=TcComputationExpression)    

        let envinner, moduleTyAcc = MakeInitialEnv env 

        let defs = [ for x in implFileFrags -> SynModuleDecl.NamespaceFragment x ]
        let! moduleContents, topAttrs, envAtEnd = TcModuleOrNamespaceElements cenv ParentNone qualNameOfFile.Range envinner PreXmlDoc.Empty None openDecls0 defs

        let implFileTypePriorToSig = moduleTyAcc.Value

        let topAttrs = 
            let mainMethodAttrs, others = topAttrs |> List.partition (fun (possTargets, _) -> possTargets &&& AttributeTargets.Method <> enum 0) 
            let assemblyAttrs, others = others |> List.partition (fun (possTargets, _) -> possTargets &&& AttributeTargets.Assembly <> enum 0) 
            // REVIEW: consider checking if '_others' is empty
            let netModuleAttrs, _others = others |> List.partition (fun (possTargets, _) -> possTargets &&& AttributeTargets.Module <> enum 0)
            { mainMethodAttrs = List.map snd mainMethodAttrs
              netModuleAttrs = List.map snd netModuleAttrs
              assemblyAttrs = List.map snd assemblyAttrs}

        let denvAtEnd = envAtEnd.DisplayEnv

        let m = qualNameOfFile.Range
    
        // This is a fake module spec
        let implFileSpecPriorToSig = wrapModuleOrNamespaceType qualNameOfFile.Id (compPathOfCcu thisCcu) implFileTypePriorToSig

        let extraAttribs = topAttrs.mainMethodAttrs@topAttrs.netModuleAttrs@topAttrs.assemblyAttrs
    
        // Run any additional checks registered to be run before applying defaults
        do 
          for check in cenv.css.GetPostInferenceChecksPreDefaults() do
            try  
                check()
            with exn -> 
                errorRecovery exn m

        conditionallySuppressErrorReporting (checkForErrors()) (fun () ->
            ApplyDefaults cenv g denvAtEnd m moduleContents extraAttribs)

        // Check completion of all classes defined across this file. 
        // NOTE: this is not a great technique if inner signatures are permitted to hide 
        // virtual dispatch slots. 
        conditionallySuppressErrorReporting (checkForErrors()) (fun () ->
            try
                implFileTypePriorToSig |> IterTyconsOfModuleOrNamespaceType (fun tycon ->
                    FinalTypeDefinitionChecksAtEndOfInferenceScope (cenv.infoReader, envAtEnd.NameEnv, cenv.tcSink, true, denvAtEnd, tycon))

            with exn ->
                errorRecovery exn m)

        // Check the value restriction. Only checked if there is no signature.
        conditionallySuppressErrorReporting (checkForErrors()) (fun () ->
          CheckValueRestriction denvAtEnd infoReader rootSigOpt implFileTypePriorToSig m)

        // Solve unsolved internal type variables 
        conditionallySuppressErrorReporting (checkForErrors()) (fun () ->
            SolveInternalUnknowns g cenv denvAtEnd moduleContents extraAttribs)

        // Check the module matches the signature 
        let implFileTy, implFileContents =
          conditionallySuppressErrorReporting (checkForErrors()) (fun () ->
            CheckModuleSignature g cenv m denvAtEnd rootSigOpt implFileTypePriorToSig implFileSpecPriorToSig moduleContents)

        do 
          conditionallySuppressErrorReporting (checkForErrors()) (fun () ->
             for check in cenv.css.GetPostInferenceChecksFinal() do
                try  
                    check()
                with exn -> 
                    errorRecovery exn m)

        // We ALWAYS run the PostTypeCheckSemanticChecks phase, though we if we have already encountered some
        // errors we turn off error reporting. This is because it performs various fixups over the TAST, e.g. 
        // assigning nice names for inference variables.
        let hasExplicitEntryPoint, anonRecdTypes = 

            conditionallySuppressErrorReporting (checkForErrors()) (fun () ->

                try  
                    let reportErrors = not (checkForErrors())
                    let tcVal = LightweightTcValForUsingInBuildMethodCall g
                    PostTypeCheckSemanticChecks.CheckImplFile 
                       (g, cenv.amap, reportErrors, cenv.infoReader, 
                        env.eInternalsVisibleCompPaths, cenv.thisCcu, tcVal, envAtEnd.DisplayEnv, 
                        implFileTy, implFileContents, extraAttribs, isLastCompiland, 
                        isInternalTestSpanStackReferring)

                with exn -> 
                    errorRecovery exn m
                    false, StampMap.Empty)

        // Warn on version attributes.
        topAttrs.assemblyAttrs |> List.iter (function
           | Attrib(tref, _, [ AttribExpr(Expr.Const (Const.String version, range, _), _) ], _, _, _, _) ->
                let attrName = tref.CompiledRepresentationForNamedType.FullName
                let isValid() =
                    try parseILVersion version |> ignore; true
                    with _ -> false
                match attrName with
                | "System.Reflection.AssemblyFileVersionAttribute" //TODO compile error like c# compiler?
                | "System.Reflection.AssemblyVersionAttribute" when not (isValid()) ->
                    warning(Error(FSComp.SR.fscBadAssemblyVersion(attrName, version), range))
                | _ -> ()
            | _ -> ())

        let namedDebugPointsForInlinedCode =
           cenv.namedDebugPointsForInlinedCode
           |> Seq.toArray
           |> Array.map (fun (KeyValue(k,v)) -> (k,v))
           |> Map

        let implFile = CheckedImplFile (qualNameOfFile, scopedPragmas, implFileTy, implFileContents, hasExplicitEntryPoint, isScript, anonRecdTypes, namedDebugPointsForInlinedCode)

        return (topAttrs, implFile, implFileTypePriorToSig, envAtEnd, cenv.createsGeneratedProvidedTypes)
     } 
   


/// Check an entire signature file
let CheckOneSigFile (g, niceNameGen, amap, thisCcu, checkForErrors, conditionalDefines, tcSink, isInternalTestSpanStackReferring) tcEnv (ParsedSigFileInput (qualifiedNameOfFile = qualNameOfFile; modules = sigFileFrags)) = 
 cancellable {     
    let cenv = 
        cenv.Create 
            (g, false, niceNameGen, amap, thisCcu, true, false, conditionalDefines, tcSink,
             (LightweightTcValForUsingInBuildMethodCall g), isInternalTestSpanStackReferring,
             tcPat=TcPat,
             tcSimplePats=TcSimplePats,
             tcSequenceExpressionEntry=TcSequenceExpressionEntry,
             tcArrayOrListSequenceExpression=TcArrayOrListComputedExpression,
             tcComputationExpression=TcComputationExpression)

    let envinner, moduleTyAcc = MakeInitialEnv tcEnv 

    let specs = [ for x in sigFileFrags -> SynModuleSigDecl.NamespaceFragment x ]
    let! tcEnv = TcSignatureElements cenv ParentNone qualNameOfFile.Range envinner PreXmlDoc.Empty None specs
    
    let sigFileType = moduleTyAcc.Value
    
    if not (checkForErrors()) then  
        try
            sigFileType |> IterTyconsOfModuleOrNamespaceType (fun tycon ->
                FinalTypeDefinitionChecksAtEndOfInferenceScope(cenv.infoReader, tcEnv.NameEnv, cenv.tcSink, false, tcEnv.DisplayEnv, tycon))
        with exn -> errorRecovery exn qualNameOfFile.Range

    return (tcEnv, sigFileType, cenv.createsGeneratedProvidedTypes)
 }
