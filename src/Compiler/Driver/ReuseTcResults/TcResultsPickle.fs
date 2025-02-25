module internal FSharp.Compiler.ReuseTcResults.TcResultsPickle

open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Infos
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreePickle
open Internal.Utilities.Collections
open Internal.Utilities.Library

type TcSharedData = { TopAttribs: TopAttribs }

// pickling

let p_context_info (x: ContextInfo) st =
    match x with
    | ContextInfo.NoContext -> p_byte 0 st
    | ContextInfo.IfExpression range ->
        p_byte 1 st
        p_range range st
    | ContextInfo.OmittedElseBranch range ->
        p_byte 2 st
        p_range range st
    | ContextInfo.ElseBranchResult range ->
        p_byte 3 st
        p_range range st
    | ContextInfo.RecordFields -> p_byte 4 st
    | ContextInfo.TupleInRecordFields -> p_byte 5 st
    | ContextInfo.CollectionElement(bool, range) ->
        p_byte 6 st
        p_bool bool st
        p_range range st
    | ContextInfo.ReturnInComputationExpression -> p_byte 7 st
    | ContextInfo.YieldInComputationExpression -> p_byte 8 st
    | ContextInfo.RuntimeTypeTest bool ->
        p_byte 9 st
        p_bool bool st
    | ContextInfo.DowncastUsedInsteadOfUpcast bool ->
        p_byte 10 st
        p_bool bool st
    | ContextInfo.FollowingPatternMatchClause range ->
        p_byte 11 st
        p_range range st
    | ContextInfo.PatternMatchGuard range ->
        p_byte 12 st
        p_range range st
    | ContextInfo.SequenceExpression ttype ->
        p_byte 13 st
        p_ty ttype st

let p_safe_init_data (x: SafeInitData) st =
    match x with
    | SafeInitField(recdFieldRef, recdField) ->
        p_byte 0 st
        p_rfref recdFieldRef st
        p_recdfield_spec recdField st
    | NoSafeInitInfo -> p_byte 1 st

let p_ctor_info (x: CtorInfo) st =
    p_int x.ctorShapeCounter st
    p_option p_Val x.safeThisValOpt st
    p_safe_init_data x.safeInitInfo st
    p_bool x.ctorIsImplicit st

let p_module_and_namespace (s: string, l: ModuleOrNamespaceRef list) st =
    p_string s st
    p_list (p_tcref "test") l st

let p_union_case_info (UnionCaseInfo(typeInst, ucref)) st =
    p_tys_new typeInst st
    p_ucref ucref st

let p_item (x: Item) st =
    match x with
    | Item.Value vref ->
        p_byte 0 st
        p_vref "test" vref st
        p_non_null_slot p_Val_new vref.binding st
    | Item.UnqualifiedType tcrefs ->
        p_byte 1 st
        p_list (p_tcref "test") tcrefs st
    | Item.UnionCase(unionCaseInfo, hasAttrs) ->
        p_byte 2 st
        p_union_case_info unionCaseInfo st
        p_bool hasAttrs st
    | Item.ExnCase tcref ->
        p_byte 3 st
        p_tcref "test" tcref st
    | _ -> ()

let p_name_resolution_env (env: NameResolutionEnv) st =
    // eDisplayEnv
    p_Map p_string p_item env.eUnqualifiedItems st
    // eUnqualifiedEnclosingTypeInsts
    // ePatItems
    (p_list p_module_and_namespace) (env.eModulesAndNamespaces |> Map.toList) st
// eFullyQualifiedModulesAndNamespaces
// eFieldLabels
// eUnqualifiedRecordOrUnionTypeInsts
// eTyconsByAccessNames
// eFullyQualifiedTyconsByAccessNames
// eTyconsByDemangledNameAndArity
// eFullyQualifiedTyconsByDemangledNameAndArity
// eIndexedExtensionMembers
// eUnindexedExtensionMembers
// eTypars

let p_tc_env (tcEnv: TcEnv) (st: WriterState) =
    p_name_resolution_env tcEnv.eNameResEnv st
    // tcEnv.eUngeneralizableItems
    p_list p_ident tcEnv.ePath st
    p_cpath tcEnv.eCompPath st
    p_cpath tcEnv.eAccessPath st
    // tcEnv.eAccessRights
    p_list p_cpath tcEnv.eInternalsVisibleCompPaths st
    p_modul_typ tcEnv.eModuleOrNamespaceTypeAccumulator.Value st
    p_context_info tcEnv.eContextInfo st
    p_option (p_tcref "test") tcEnv.eFamilyType st
    p_option p_ctor_info tcEnv.eCtorInfo st
    p_option p_string tcEnv.eCallerMemberName st
    p_list (p_list (p_ArgReprInfo)) tcEnv.eLambdaArgInfos st
    p_bool tcEnv.eIsControlFlow st
// tcEnv.eCachedImplicitYieldExpressions

let p_tcs_root_sig (qualifiedNameOfFile, moduleOrNamespaceType) st =
    p_tup2 p_qualified_name_of_file p_modul_typ_new (qualifiedNameOfFile, moduleOrNamespaceType) st

// pickling top

let pickleSharedData sharedData st =
    p_tup3
        p_attribs
        p_attribs
        p_attribs
        (sharedData.TopAttribs.mainMethodAttrs, sharedData.TopAttribs.netModuleAttrs, sharedData.TopAttribs.assemblyAttrs)
        st

let pickleCheckedImplFile checkedImplFile st = p_checked_impl_file checkedImplFile st

let pickleTcState (tcState: TcState) (st: WriterState) =
    p_ccuref_new tcState.tcsCcu st
    p_tc_env tcState.tcsTcSigEnv st
    p_tc_env tcState.tcsTcImplEnv st
    p_bool tcState.tcsCreatesGeneratedProvidedTypes st
    (p_list p_tcs_root_sig) (tcState.tcsRootSigs.ToList()) st
    p_list p_qualified_name_of_file (tcState.tcsRootImpls.ToList()) st
    p_modul_typ_new tcState.tcsCcuSig st
    p_list p_open_decl tcState.tcsImplicitOpenDeclarations st

// unpickling

let u_context_info st : ContextInfo =
    let tag = u_byte st

    match tag with
    | 0 -> ContextInfo.NoContext
    | 1 ->
        let range = u_range st
        ContextInfo.IfExpression range
    | 2 ->
        let range = u_range st
        ContextInfo.OmittedElseBranch range
    | 3 ->
        let range = u_range st
        ContextInfo.ElseBranchResult range
    | 4 -> ContextInfo.RecordFields
    | 5 -> ContextInfo.TupleInRecordFields
    | 6 ->
        let bool = u_bool st
        let range = u_range st
        ContextInfo.CollectionElement(bool, range)
    | 7 -> ContextInfo.ReturnInComputationExpression
    | 8 -> ContextInfo.YieldInComputationExpression
    | 9 ->
        let bool = u_bool st
        ContextInfo.RuntimeTypeTest bool
    | 10 ->
        let bool = u_bool st
        ContextInfo.DowncastUsedInsteadOfUpcast bool
    | 11 ->
        let range = u_range st
        ContextInfo.FollowingPatternMatchClause range
    | 12 ->
        let range = u_range st
        ContextInfo.PatternMatchGuard range
    | 13 ->
        let ttype = u_ty st
        ContextInfo.SequenceExpression ttype
    | _ -> ufailwith st "u_context_info"

let u_safe_init_data st : SafeInitData =
    let tag = u_byte st

    match tag with
    | 0 ->
        let recdFieldRef = u_rfref st
        let recdField = u_recdfield_spec st
        SafeInitField(recdFieldRef, recdField)
    | 1 -> NoSafeInitInfo
    | _ -> ufailwith st "u_safe_init_data"

let u_ctor_info st : CtorInfo =
    let ctorShapeCounter = u_int st
    let safeThisValOpt = u_option u_Val st
    let safeInitInfo = u_safe_init_data st
    let ctorIsImplicit = u_bool st

    {
        ctorShapeCounter = ctorShapeCounter
        safeThisValOpt = safeThisValOpt
        safeInitInfo = safeInitInfo
        ctorIsImplicit = ctorIsImplicit
    }

let u_module_and_namespace st : string * ModuleOrNamespaceRef list =
    let s = u_string st
    let l = u_list u_tcref st
    s, l

let u_union_case_info st =
    let typeInst = u_tys_new st
    let ucref = u_ucref st
    UnionCaseInfo(typeInst, ucref)

let u_item st : Item =
    let tag = u_byte st

    match tag with
    | 0 ->
        let vref = u_vref st
        let binding = u_non_null_slot u_Val_new st
        vref.binding <- binding
        Item.Value vref
    | 1 ->
        let tcrefs = u_list u_tcref st
        Item.UnqualifiedType tcrefs

    | 2 ->
        let unionCaseInfo = u_union_case_info st
        let hasAttrs = u_bool st
        Item.UnionCase(unionCaseInfo, hasAttrs)
    | 3 ->
        let tcref = u_tcref st
        Item.ExnCase tcref
    | _ -> ufailwith st "u_item"

let u_name_resolution_env st : NameResolutionEnv =
    let eUnqualifiedItems = u_Map u_string u_item st

    let eModulesAndNamespaces: NameMultiMap<ModuleOrNamespaceRef> =
        u_list u_module_and_namespace st |> Map.ofList

    let g: TcGlobals = Unchecked.defaultof<_>

    { NameResolutionEnv.Empty g with
        eUnqualifiedItems = eUnqualifiedItems
        eModulesAndNamespaces = eModulesAndNamespaces
    }

let u_tc_env (st: ReaderState) : TcEnv =
    let eNameResEnv = u_name_resolution_env st
    //let eUngeneralizableItems
    let ePath = u_list u_ident st
    let eCompPath = u_cpath st
    let eAccessPath = u_cpath st
    // eAccessRights
    let eInternalsVisibleCompPaths = u_list u_cpath st
    let eModuleOrNamespaceTypeAccumulator = u_modul_typ st
    let eContextInfo = u_context_info st
    let eFamilyType = u_option u_tcref st
    let eCtorInfo = u_option u_ctor_info st
    let eCallerMemberName = u_option u_string st
    let eLambdaArgInfos = u_list (u_list u_ArgReprInfo) st
    let eIsControlFlow = u_bool st
    // eCachedImplicitYieldExpressions

    {
        eNameResEnv = eNameResEnv
        eUngeneralizableItems = List.empty
        ePath = ePath
        eCompPath = eCompPath
        eAccessPath = eAccessPath
        eAccessRights = AccessibleFromEverywhere
        eInternalsVisibleCompPaths = eInternalsVisibleCompPaths
        eModuleOrNamespaceTypeAccumulator = ref eModuleOrNamespaceTypeAccumulator
        eContextInfo = eContextInfo
        eFamilyType = eFamilyType
        eCtorInfo = eCtorInfo
        eCallerMemberName = eCallerMemberName
        eLambdaArgInfos = eLambdaArgInfos
        eIsControlFlow = eIsControlFlow
        eCachedImplicitYieldExpressions = HashMultiMap(HashIdentity.Structural)
    }

let u_tcs_root_sig st =
    let qualifiedNameOfFile, moduleOrNamespaceType =
        u_tup2 u_qualified_name_of_file u_modul_typ_new st

    qualifiedNameOfFile, moduleOrNamespaceType

// unpickling top

let unpickleSharedData st =
    let mainMethodAttrs, netModuleAttrs, assemblyAttrs =
        u_tup3 u_attribs u_attribs u_attribs st

    let attribs =
        {
            mainMethodAttrs = mainMethodAttrs
            netModuleAttrs = netModuleAttrs
            assemblyAttrs = assemblyAttrs
        }

    { TopAttribs = attribs }

let unpickleCheckedImplFile st = u_checked_impl_file st

let unpickleTcState (st: ReaderState) : TcState =
    let tcsCcu = u_ccuref_new st
    let tcsTcSigEnv = u_tc_env st
    let tcsTcImplEnv = u_tc_env st
    let tcsCreatesGeneratedProvidedTypes = u_bool st
    let tcsRootSigs = u_list u_tcs_root_sig st
    let tcsRootImpls = u_list u_qualified_name_of_file st
    let tcsCcuSig = u_modul_typ_new st
    let tcsImplicitOpenDeclarations = u_list u_open_decl st

    {
        tcsCcu = tcsCcu
        tcsCreatesGeneratedProvidedTypes = tcsCreatesGeneratedProvidedTypes
        tcsTcSigEnv = tcsTcSigEnv
        tcsTcImplEnv = tcsTcImplEnv
        tcsRootSigs = RootSigs.FromList(qnameOrder, tcsRootSigs)
        tcsRootImpls = RootImpls.Create(qnameOrder, tcsRootImpls)
        tcsCcuSig = tcsCcuSig
        tcsImplicitOpenDeclarations = tcsImplicitOpenDeclarations
    }
