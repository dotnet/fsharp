module internal FSharp.Compiler.ReuseTcResults.TcResultsPickle

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Infos
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypedTreePickle
open FSharp.Compiler.Xml

open Internal.Utilities
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras

type TcSharedData = { TopAttribs: TopAttribs }

// pickling

let p_stamp = p_int64

let p_stamp_map pv = p_Map p_stamp pv

let p_non_null_slot f (x: 'a | null) st =
    match x with
    | null -> p_byte 0 st
    | h ->
        p_byte 1 st
        f h st

let p_ILTypeDefAdditionalFlags (x: ILTypeDefAdditionalFlags) st = p_int32 (int x) st

let p_ILTypeDef (x: ILTypeDef) st =
    p_string x.Name st
    //p_type_attributes x.Attributes
    //p_il_type_def_layout x.Layout
    //x.Implements
    //x.Extends
    //x.Methods
    //x.NestedTypes
    //x.Fields
    //x.MethodImpls
    //x.Events
    //x.Properties
    p_ILTypeDefAdditionalFlags x.Flags st
//x.SecurityDeclsStored
//x.CustomAttrsStored
//p_il
//p_int32 x.MetadataIndex st

let p_tyar_spec_data_new (x: Typar) st =
    p_tup6
        p_ident
        p_attribs
        p_int64
        p_tyar_constraints
        p_xmldoc
        p_stamp
        (x.typar_id, x.Attribs, int64 x.typar_flags.PickledBits, x.Constraints, x.XmlDoc, x.Stamp)
        st

let p_tyar_spec_new (x: Typar) st =
    //Disabled, workaround for bug 2721: if x.Rigidity <> TyparRigidity.Rigid then warning(Error(sprintf "p_tyar_spec: typar#%d is not rigid" x.Stamp, x.Range))
    if x.IsFromError then
        warning (Error((0, "p_tyar_spec: from error"), x.Range))

    p_osgn_decl st.otypars p_tyar_spec_data_new x st

let p_tyar_specs_new = (p_list p_tyar_spec_new)

let rec p_ty_new (ty: TType) st : unit =
    match ty with
    | TType_tuple(tupInfo, l) ->
        p_byte 0 st
        p_tup2 p_tup_info p_tys_new (tupInfo, l) st

    | TType_app(tyconRef, typeInstantiation, nullness) ->
        p_byte 1 st

        p_tup4
            (p_tcref "app")
            p_tys_new
            p_nullness
            (p_non_null_slot p_entity_spec_new)
            (tyconRef, typeInstantiation, nullness, tyconRef.binding)
            st

    | TType_fun(domainType, rangeType, nullness) ->
        p_byte 2 st
        p_ty_new domainType st
        p_ty_new rangeType st
        p_nullness nullness st

    | TType_var(typar, nullness) ->
        p_byte 3 st
        p_tup4 p_tpref p_nullness (p_option p_ty_new) p_stamp (typar, nullness, typar.Solution, typar.Stamp) st

    | TType_forall(tps, r) ->
        p_byte 4 st
        p_tup2 p_typars p_ty_new (tps, r) st

    | TType_measure unt ->
        p_byte 5 st
        p_measure_expr unt st

    | TType_ucase(uc, tinst) ->
        p_byte 6 st
        p_tup2 p_ucref p_tys_new (uc, tinst) st

    | TType_anon(anonInfo, l) ->
        p_byte 7 st
        p_tup2 p_anonInfo p_tys_new (anonInfo, l) st

and p_tys_new l =
    let _count = l.Length
    p_list p_ty_new l

and p_expr_new (expr: Expr) st =
    match expr with
    | Expr.Link e ->
        p_byte 0 st
        p_expr_new e.Value st
    | Expr.Const(x, m, ty) ->
        p_byte 1 st
        p_tup3 p_const p_dummy_range p_ty_new (x, m, ty) st
    | Expr.Val(a, b, m) ->
        p_byte 2 st
        p_tup4 p_vref_new p_vrefFlags p_dummy_range (p_non_null_slot p_Val_new) (a, b, m, a.binding) st
    | Expr.Op(a, b, c, d) ->
        p_byte 3 st
        p_tup4 p_op_new p_tys_new p_exprs_new p_dummy_range (a, b, c, d) st
    | Expr.Sequential(a, b, c, d) ->
        p_byte 4 st

        p_tup4
            p_expr_new
            p_expr_new
            p_int
            p_dummy_range
            (a,
             b,
             (match c with
              | NormalSeq -> 0
              | ThenDoSeq -> 1),
             d)
            st
    | Expr.Lambda(_, a1, b0, b1, c, d, e) ->
        p_byte 5 st
        p_tup6 (p_option p_Val) (p_option p_Val) p_Vals p_expr_new p_dummy_range p_ty_new (a1, b0, b1, c, d, e) st
    | Expr.TyLambda(_, b, c, d, e) ->
        p_byte 6 st
        p_tup4 p_tyar_specs_new p_expr_new p_dummy_range p_ty_new (b, c, d, e) st
    | Expr.App(funcExpr, formalType, typeArgs, args, range) ->
        p_byte 7 st

        p_expr_new funcExpr st
        p_ty_new formalType st
        p_tys_new typeArgs st
        p_exprs_new args st
        p_dummy_range range st
    | Expr.LetRec(a, b, c, _) ->
        p_byte 8 st
        p_tup3 p_binds p_expr_new p_dummy_range (a, b, c) st
    | Expr.Let(a, b, c, _) ->
        p_byte 9 st
        p_tup3 p_bind p_expr_new p_dummy_range (a, b, c) st
    | Expr.Match(_, a, b, c, d, e) ->
        p_byte 10 st
        p_tup5 p_dummy_range p_dtree p_targets p_dummy_range p_ty_new (a, b, c, d, e) st
    | Expr.Obj(_, b, c, d, e, f, g) ->
        p_byte 11 st
        p_tup6 p_ty_new (p_option p_Val) p_expr_new p_methods p_intfs p_dummy_range (b, c, d, e, f, g) st
    | Expr.StaticOptimization(a, b, c, d) ->
        p_byte 12 st
        p_tup4 p_constraints p_expr_new p_expr_new p_dummy_range (a, b, c, d) st
    | Expr.TyChoose(a, b, c) ->
        p_byte 13 st
        p_tup3 p_tyar_specs_new p_expr_new p_dummy_range (a, b, c) st
    | Expr.Quote(ast, _, _, m, ty) ->
        p_byte 14 st
        p_tup3 p_expr_new p_dummy_range p_ty_new (ast, m, ty) st
    | Expr.WitnessArg(traitInfo, m) ->
        p_byte 15 st
        p_trait traitInfo st
        p_dummy_range m st
    | Expr.DebugPoint(_, innerExpr) ->
        p_byte 16 st
        p_expr_new innerExpr st

and p_exprs_new = p_list p_expr_new

and p_ucref_new (UnionCaseRef(a, b)) st =
    p_tup3 (p_tcref "ucref") p_string (p_non_null_slot p_entity_spec_new) (a, b, a.binding) st

and p_op_new x st =
    match x with
    | TOp.UnionCase c ->
        p_byte 0 st
        p_ucref_new c st
    | TOp.ExnConstr c ->
        p_byte 1 st
        p_tcref "op" c st
    | TOp.Tuple tupInfo ->
        if evalTupInfoIsStruct tupInfo then
            p_byte 29 st
        else
            p_byte 2 st
    | TOp.Recd(a, b) ->
        p_byte 3 st
        p_tup2 p_recdInfo (p_tcref "recd op") (a, b) st
    | TOp.ValFieldSet a ->
        p_byte 4 st
        p_rfref a st
    | TOp.ValFieldGet a ->
        p_byte 5 st
        p_rfref a st
    | TOp.UnionCaseTagGet a ->
        p_byte 6 st
        p_tcref "cnstr op" a st
    | TOp.UnionCaseFieldGet(a, b) ->
        p_byte 7 st
        p_tup2 p_ucref p_int (a, b) st
    | TOp.UnionCaseFieldSet(a, b) ->
        p_byte 8 st
        p_tup2 p_ucref p_int (a, b) st
    | TOp.ExnFieldGet(a, b) ->
        p_byte 9 st
        p_tup2 (p_tcref "exn op") p_int (a, b) st
    | TOp.ExnFieldSet(a, b) ->
        p_byte 10 st
        p_tup2 (p_tcref "exn op") p_int (a, b) st
    | TOp.TupleFieldGet(tupInfo, a) ->
        if evalTupInfoIsStruct tupInfo then
            p_byte 30 st
            p_int a st
        else
            p_byte 11 st
            p_int a st
    | TOp.ILAsm(a, b) ->
        p_byte 12 st
        p_tup2 (p_list p_ILInstr) p_tys (a, b) st
    | TOp.RefAddrGet _ -> p_byte 13 st
    | TOp.UnionCaseProof a ->
        p_byte 14 st
        p_ucref a st
    | TOp.Coerce -> p_byte 15 st
    | TOp.TraitCall b ->
        p_byte 16 st
        p_trait b st
    | TOp.LValueOp(a, b) ->
        p_byte 17 st
        p_tup2 p_lval_op_kind (p_vref "lval") (a, b) st
    | TOp.ILCall(a1, a2, a3, a4, a5, a7, a8, a9, b, c, d) ->
        p_byte 18 st

        p_tup11
            p_bool
            p_bool
            p_bool
            p_bool
            p_vrefFlags
            p_bool
            p_bool
            p_ILMethodRef
            p_tys
            p_tys
            p_tys
            (a1, a2, a3, a4, a5, a7, a8, a9, b, c, d)
            st
    | TOp.Array -> p_byte 19 st
    | TOp.While _ -> p_byte 20 st
    | TOp.IntegerForLoop(_, _, dir) ->
        p_byte 21 st

        p_int
            (match dir with
             | FSharpForLoopUp -> 0
             | CSharpForLoopUp -> 1
             | FSharpForLoopDown -> 2)
            st
    | TOp.Bytes bytes ->
        p_byte 22 st
        p_bytes bytes st
    | TOp.TryWith _ -> p_byte 23 st
    | TOp.TryFinally _ -> p_byte 24 st
    | TOp.ValFieldGetAddr(a, _) ->
        p_byte 25 st
        p_rfref a st
    | TOp.UInt16s arr ->
        p_byte 26 st
        p_array p_uint16 arr st
    | TOp.Reraise -> p_byte 27 st
    | TOp.UnionCaseFieldGetAddr(a, b, _) ->
        p_byte 28 st
        p_tup2 p_ucref p_int (a, b) st
    // Note tag byte 29 is taken for struct tuples, see above
    // Note tag byte 30 is taken for struct tuples, see above
    (* 29: TOp.Tuple when evalTupInfoIsStruct tupInfo = true *)
    (* 30: TOp.TupleFieldGet  when evalTupInfoIsStruct tupInfo = true *)
    | TOp.AnonRecd info ->
        p_byte 31 st
        p_anonInfo info st
    | TOp.AnonRecdGet(info, n) ->
        p_byte 32 st
        p_anonInfo info st
        p_int n st
    | TOp.Goto _
    | TOp.Label _
    | TOp.Return -> failwith "unexpected backend construct in pickled TAST"

and p_entity_spec_data_new (x: Entity) st =
    p_tyar_specs_new (x.entity_typars.Force(x.entity_range)) st
    p_string x.entity_logical_name st
    p_option p_string x.EntityCompiledName st
    p_range x.entity_range st
    p_stamp x.entity_stamp st
    p_option p_pubpath x.entity_pubpath st
    p_access x.Accessibility st
    p_access x.TypeReprAccessibility st
    p_attribs x.entity_attribs st
    let _ = p_tycon_repr_new x.entity_tycon_repr st
    p_option p_ty_new x.TypeAbbrev st
    p_tcaug_new x.entity_tycon_tcaug st
    p_string System.String.Empty st
    p_kind x.TypeOrMeasureKind st
    p_int64 x.entity_flags.Flags st
    p_option p_cpath x.entity_cpath st
    p_maybe_lazy p_modul_typ_new x.entity_modul_type st
    p_exnc_repr x.ExceptionInfo st

    if st.oInMem then
        p_used_space1 (p_xmldoc x.XmlDoc) st
    else
        p_space 1 () st

and p_entity_spec_new x st =
    p_osgn_decl st.oentities p_entity_spec_data_new x st

and p_ValData_new x st =
    p_string x.val_logical_name st
    p_option p_string x.ValCompiledName st
    // only keep range information on published values, not on optimization data
    p_ranges (x.ValReprInfo |> Option.map (fun _ -> x.val_range, x.DefinitionRange)) st

    p_ty_new x.val_type st
    p_stamp x.val_stamp st

    p_int64 x.val_flags.Flags st
    p_option p_member_info x.MemberInfo st
    p_attribs x.Attribs st
    p_option p_ValReprInfo x.ValReprInfo st
    p_string x.XmlDocSig st
    p_access x.Accessibility st
    p_parentref x.TryDeclaringEntity st
    p_option p_const x.LiteralValue st

    if st.oInMem then
        p_used_space1 (p_xmldoc x.XmlDoc) st
    else
        p_space 1 () st

and p_Val_new x st = p_osgn_decl st.ovals p_ValData_new x st

and p_modul_typ_new (x: ModuleOrNamespaceType) st =
    p_tup3 p_istype (p_qlist p_Val_new) (p_qlist p_entity_spec_new) (x.ModuleOrNamespaceKind, x.AllValsAndMembers, x.AllEntities) st

and p_tcaug_new (p: TyconAugmentation) st =
    p_tup9
        (p_option (p_tup2 (p_vref "compare_obj") (p_vref "compare")))
        (p_option (p_vref "compare_withc"))
        (p_option (p_tup3 (p_vref "hash_obj") (p_vref "hash_withc") (p_vref "equals_withc")))
        (p_option (p_tup2 (p_vref "hash") (p_vref "equals")))
        (p_list (p_tup2 p_string (p_vref "adhoc")))
        (p_list (p_tup3 p_ty_new p_bool p_dummy_range))
        (p_option p_ty_new)
        p_bool
        (p_space 1)
        (p.tcaug_compare,
         p.tcaug_compare_withc,
         p.tcaug_hash_and_equals_withc
         |> Option.map (fun (v1, v2, v3, _) -> (v1, v2, v3)),
         p.tcaug_equals,
         (p.tcaug_adhoc_list
          |> ResizeArray.toList
          // Explicit impls of interfaces only get kept in the adhoc list
          // in order to get check the well-formedness of an interface.
          // Keeping them across assembly boundaries is not valid, because relinking their ValRefs
          // does not work correctly (they may get incorrectly relinked to a default member)
          |> List.filter (fun (isExplicitImpl, _) -> not isExplicitImpl)
          |> List.map (fun (_, vref) -> vref.LogicalName, vref)),
         p.tcaug_interfaces,
         p.tcaug_super,
         p.tcaug_abstract,
         space)
        st

and p_ccu_data (x: CcuData) st =
    p_option p_string x.FileName st
    p_ILScopeRef x.ILScopeRef st
    p_stamp x.Stamp st
    p_option p_string x.QualifiedName st
    p_string x.SourceCodeDirectory st
    p_bool x.IsFSharp st
#if !NO_TYPEPROVIDERS
    p_bool x.IsProviderGenerated st
#endif
    p_bool x.UsesFSharp20PlusQuotations st
    p_entity_spec_data_new x.Contents st

and p_ccuref_new (x: CcuThunk) st =
    p_tup2 p_ccu_data p_string (x.target, x.name) st

and p_nleref_new (x: NonLocalEntityRef) st =
    let (NonLocalEntityRef(ccu, strings)) = x
    p_tup2 p_ccuref_new (p_array p_string) (ccu, strings) st

and p_tcref_new (x: EntityRef) st =
    match x with
    | ERefLocal x ->
        p_byte 0 st
        p_local_item_ref "tcref" st.oentities x st
    | ERefNonLocal x ->
        p_byte 1 st
        p_nleref_new x st

and p_nonlocal_val_ref_new (nlv: NonLocalValOrMemberRef) st =
    let a = nlv.EnclosingEntity
    let key = nlv.ItemKey
    let pkey = key.PartialKey
    p_tcref_new a st
    p_option p_string pkey.MemberParentMangledName st
    p_bool pkey.MemberIsOverride st
    p_string pkey.LogicalName st
    p_int pkey.TotalArgCount st

    let isStructThisArgPos =
        match key.TypeForLinkage with
        | None -> false
        | Some ty -> checkForInRefStructThisArg st ty

    p_option p_ty_new key.TypeForLinkage st

and p_vref_new (x: ValRef) st =
    match x with
    | VRefLocal x ->
        p_byte 0 st
        p_local_item_ref "valref" st.ovals x st
    | VRefNonLocal x ->
        p_byte 1 st
        p_nonlocal_val_ref_new x st

and p_bind_new (TBind(a, b, _)) st = p_tup2 p_Val_new p_expr_new (a, b) st

and p_binding (x: ModuleOrNamespaceBinding) st =
    match x with
    | ModuleOrNamespaceBinding.Binding binding ->
        p_byte 0 st
        p_bind binding st
    | ModuleOrNamespaceBinding.Module(moduleOrNamespace, moduleOrNamespaceContents) ->
        p_byte 1 st
        p_tup2 p_entity_spec_new p_module_or_namespace_contents (moduleOrNamespace, moduleOrNamespaceContents) st

and p_tycon_repr_new (x: TyconRepresentation) st =
    // The leading "p_byte 1" and "p_byte 0" come from the F# 2.0 format, which used an option value at this point.

    match x with
    // Records
    | TFSharpTyconRepr {
                           fsobjmodel_rfields = fs
                           fsobjmodel_kind = TFSharpRecord
                       } ->
        p_byte 1 st
        p_byte 0 st
        p_rfield_table fs st
        false

    // Unions without static fields
    | TFSharpTyconRepr {
                           fsobjmodel_cases = x
                           fsobjmodel_kind = TFSharpUnion
                           fsobjmodel_rfields = fs
                       } when fs.FieldsByIndex.Length = 0 ->
        p_byte 1 st
        p_byte 1 st
        p_array p_unioncase_spec x.CasesTable.CasesByIndex st
        false

    // Unions with static fields, added to format
    | TFSharpTyconRepr({
                           fsobjmodel_cases = cases
                           fsobjmodel_kind = TFSharpUnion
                       } as r) ->
        if st.oglobals.compilingFSharpCore then
            let fields = r.fsobjmodel_rfields.FieldsByIndex
            let firstFieldRange = fields[0].DefinitionRange

            let allFieldsText =
                fields
                |> Array.map (fun f -> f.LogicalName)
                |> String.concat System.Environment.NewLine

            raise (Error(FSComp.SR.pickleFsharpCoreBackwardsCompatible ("fields in union", allFieldsText), firstFieldRange))

        p_byte 2 st
        p_array p_unioncase_spec cases.CasesTable.CasesByIndex st
        p_tycon_objmodel_data r st
        false

    | TAsmRepr ilTy ->
        p_byte 1 st
        p_byte 2 st
        p_ILType ilTy st
        false

    | TFSharpTyconRepr r ->
        p_byte 1 st
        p_byte 3 st
        p_tycon_objmodel_data r st
        false

    | TMeasureableRepr ty ->
        p_byte 1 st
        p_byte 4 st
        p_ty ty st
        false

    | TNoRepr ->
        p_byte 0 st
        false

#if !NO_TYPEPROVIDERS
    | TProvidedTypeRepr info ->
        if info.IsErased then
            // Pickle erased type definitions as a NoRepr
            p_byte 0 st
            false
        else
            // Pickle generated type definitions as a TAsmRepr
            p_byte 1 st
            p_byte 2 st
            p_ILType (mkILBoxedType (ILTypeSpec.Create(TypeProviders.GetILTypeRefOfProvidedType(info.ProvidedType, range0), []))) st
            true

    | TProvidedNamespaceRepr _ ->
        p_byte 0 st
        false
#endif

    | TILObjectRepr(TILObjectReprData(scope, nesting, td)) ->
        p_byte 5 st
        p_ILScopeRef scope st
        (p_list p_ILTypeDef) nesting st
        p_ILTypeDef td st
        false

and p_qualified_name_of_file qualifiedNameOfFile st =
    let (QualifiedNameOfFile ident) = qualifiedNameOfFile
    p_ident ident st

and p_pragma pragma st =
    let (ScopedPragma.WarningOff(range, warningNumber)) = pragma
    p_tup2 p_range p_int (range, warningNumber) st

and p_pragmas x st = p_list p_pragma x st

and p_long_ident (x: LongIdent) st = p_list p_ident x st

and p_trivia (x: SyntaxTrivia.IdentTrivia) st = pfailwith st (nameof p_trivia)

and p_syn_long_ident (x: SynLongIdent) st =
    let (SynLongIdent(id, dotRanges, trivia)) = x
    p_tup3 p_long_ident (p_list p_range) (p_list (p_option p_trivia)) (id, dotRanges, trivia) st

and p_syn_type (x: SynType) st = pfailwith st (nameof p_syn_type)

and p_syn_open_decl_target (x: SynOpenDeclTarget) st =
    match x with
    | SynOpenDeclTarget.ModuleOrNamespace(longId, range) ->
        p_byte 0 st
        p_tup2 p_syn_long_ident p_range (longId, range) st
    | SynOpenDeclTarget.Type(typeName, range) ->
        p_byte 1 st
        p_tup2 p_syn_type p_range (typeName, range) st

and p_tup_info (tupInfo: TupInfo) st =
    let (TupInfo.Const c) = tupInfo
    p_bool c st

and p_nullness (nullness: Nullness) st =
    match nullness.Evaluate() with
    | NullnessInfo.WithNull -> p_byte 0 st
    | NullnessInfo.WithoutNull -> p_byte 1 st
    | NullnessInfo.AmbivalentToNull -> p_byte 2 st

and p_typars = p_list p_tpref

and p_module_or_namespace_contents (x: ModuleOrNamespaceContents) st =
    match x with
    | TMDefs defs ->
        p_byte 0 st
        p_list p_module_or_namespace_contents defs st
    | TMDefOpens openDecls ->
        p_byte 1 st
        p_list p_open_decl openDecls st
    | TMDefLet(binding, range) ->
        p_byte 2 st
        p_tup2 p_bind_new p_range (binding, range) st
    | TMDefDo(expr, range) ->
        p_byte 3 st
        p_tup2 p_expr_new p_range (expr, range) st
    | TMDefRec(isRec, opens, tycons, bindings, range) ->
        p_byte 4 st

        p_tup5
            p_bool
            (p_list p_open_decl)
            (p_list p_entity_spec_data_new)
            (p_list p_binding)
            p_range
            (isRec, opens, tycons, bindings, range)
            st

and p_checked_impl_file_contents = p_module_or_namespace_contents

and p_named_debug_point_key (x: NamedDebugPointKey) st =
    p_tup2 p_range p_string (x.Range, x.Name) st

and p_named_debug_points = p_Map p_named_debug_point_key p_range

and p_anon_recd_types = p_stamp_map p_anonInfo

and p_open_decl (x: OpenDeclaration) st =
    p_tup6
        p_syn_open_decl_target
        (p_option p_range)
        (p_list p_tcref_new)
        p_tys
        p_range
        p_bool
        (x.Target, x.Range, x.Modules, x.Types, x.AppliedScope, x.IsOwnNamespace)
        st

and p_checked_impl_file file st =
    let (CheckedImplFile(qualifiedNameOfFile,
                         pragmas,
                         signature,
                         contents,
                         hasExplicitEntryPoint,
                         isScript,
                         anonRecdTypeInfo,
                         namedDebugPointsForInlinedCode)) =
        file

    p_qualified_name_of_file qualifiedNameOfFile st
    p_pragmas pragmas st
    p_modul_typ_new signature st
    p_checked_impl_file_contents contents st
    p_bool hasExplicitEntryPoint st
    p_bool isScript st
    p_anon_recd_types anonRecdTypeInfo st
    p_named_debug_points namedDebugPointsForInlinedCode st

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

let u_stamp = u_int64

let u_stamp_map uv = u_Map u_stamp uv

let u_non_null_slot f st =
    let tag = u_byte st

    match tag with
    | 0 -> Unchecked.defaultof<_>
    | 1 -> f st
    | n -> ufailwith st ("u_option: found number " + string n)

let u_ILTypeDefAdditionalFlags st : ILTypeDefAdditionalFlags =
    let i = u_int32 st
    enum i

let u_ILTypeDef st : ILTypeDef =
    let name = u_string st
    let attributes = System.Reflection.TypeAttributes.Public
    let layout = ILTypeDefLayout.Auto
    let implements = Unchecked.defaultof<_>
    let genericParams = []
    let extends = Unchecked.defaultof<_>
    let methods = ILMethodDefs(fun () -> [||])
    let nestedTypes = Unchecked.defaultof<_>
    let fields = Unchecked.defaultof<_>
    let methodImpls = Unchecked.defaultof<_>
    let events = Unchecked.defaultof<_>
    let properties = Unchecked.defaultof<_>
    let additionalFlags = u_ILTypeDefAdditionalFlags st
    let securityDeclsStored = ILSecurityDecls([||])
    // TODO: fill this in
    let customAttrsStored = ILAttributesStored.Given(ILAttributes.Empty)

    ILTypeDef(
        name,
        attributes,
        layout,
        implements,
        genericParams,
        extends,
        methods,
        nestedTypes,
        fields,
        methodImpls,
        events,
        properties,
        additionalFlags,
        securityDeclsStored,
        customAttrsStored
    )

let u_tyar_spec_data_new st =
    let a, c, d, e, g, stamp =
        u_tup6 u_ident u_attribs u_int64 u_tyar_constraints u_xmldoc u_stamp st

    {
        typar_id = a
        typar_stamp = stamp
        typar_flags = TyparFlags(int32 d)
        typar_solution = None
        typar_astype = Unchecked.defaultof<_>
        typar_opt_data =
            match g, e, c with
            | doc, [], [] when doc.IsEmpty -> None
            | _ ->
                Some
                    {
                        typar_il_name = None
                        typar_xmldoc = g
                        typar_constraints = e
                        typar_attribs = c
                        typar_is_contravariant = false
                    }
    }

let u_tyar_spec_new st =
    u_osgn_decl st.itypars u_tyar_spec_data_new st

let u_tyar_specs_new = u_list u_tyar_spec_new

let rec u_ty_new st : TType =
    let tag = u_byte st

    match tag with
    | 0 ->
        let tupInfo, l = u_tup2 u_tup_info u_tys_new st
        TType_tuple(tupInfo, l)

    | 1 ->
        let tyconRef, typeInstantiation, nullness, binding =
            u_tup4 u_tcref u_tys_new u_nullness (u_non_null_slot u_entity_spec_new) st

        tyconRef.binding <- binding
        TType_app(tyconRef, typeInstantiation, nullness)

    | 2 ->
        let (domainType, rangeType, nullness) = u_tup3 u_ty_new u_ty_new u_nullness st
        TType_fun(domainType, rangeType, nullness)

    | 3 ->
        let (typar, nullness, solution, stamp) =
            u_tup4 u_tpref u_nullness (u_option u_ty_new) u_stamp st

        typar.typar_solution <- solution
        typar.typar_stamp <- stamp
        TType_var(typar, nullness)

    | 4 ->
        let (tps, r) = u_tup2 u_typars u_ty_new st

        TType_forall(tps, r)

    | 5 ->
        let unt = u_measure_expr st
        TType_measure unt

    | 6 ->
        let uc, tinst = u_tup2 u_ucref u_tys_new st
        TType_ucase(uc, tinst)

    | 7 ->
        let anonInfo, l = u_tup2 u_anonInfo u_tys_new st
        TType_anon(anonInfo, l)
    | _ -> ufailwith st (nameof u_ty_new)

and u_tys_new = u_list u_ty_new

and u_expr_new st : Expr =
    let tag = u_byte st

    match tag with
    | 0 ->
        let e = u_expr_new st
        let r = ref e
        Expr.Link r
    | 1 ->
        let a = u_const st
        let b = u_dummy_range st
        let c = u_ty_new st
        Expr.Const(a, b, c)
    | 2 ->
        let valRef = u_vref_new st
        let flags = u_vrefFlags st
        let range = u_dummy_range st
        let binding = (u_non_null_slot u_Val_new) st

        valRef.binding <- binding
        let expr = Expr.Val(valRef, flags, range)
        expr
    | 3 ->
        let a = u_op_new st
        let b = u_tys_new st
        let c = u_exprs_new st
        let d = u_dummy_range st
        Expr.Op(a, b, c, d)
    | 4 ->
        let a = u_expr_new st
        let b = u_expr_new st
        let c = u_int st
        let d = u_dummy_range st

        let dir =
            match c with
            | 0 -> NormalSeq
            | 1 -> ThenDoSeq
            | _ -> ufailwith st "specialSeqFlag"

        Expr.Sequential(a, b, dir, d)
    | 5 ->
        let a0 = u_option u_Val st
        let b0 = u_option u_Val st
        let b1 = u_Vals st
        let c = u_expr_new st
        let d = u_dummy_range st
        let e = u_ty_new st
        Expr.Lambda(newUnique (), a0, b0, b1, c, d, e)
    | 6 ->
        let b = u_tyar_specs_new st
        let c = u_expr_new st
        let d = u_dummy_range st
        let e = u_ty_new st
        Expr.TyLambda(newUnique (), b, c, d, e)
    | 7 ->
        let a1 = u_expr_new st
        let a2 = u_ty_new st
        let b = u_tys_new st
        let c = u_exprs_new st
        let d = u_dummy_range st
        let expr = Expr.App(a1, a2, b, c, d)
        expr
    | 8 ->
        let a = u_binds st
        let b = u_expr_new st
        let c = u_dummy_range st
        Expr.LetRec(a, b, c, Construct.NewFreeVarsCache())
    | 9 ->
        let a = u_bind st
        let b = u_expr_new st
        let c = u_dummy_range st
        Expr.Let(a, b, c, Construct.NewFreeVarsCache())
    | 10 ->
        let a = u_dummy_range st
        let b = u_dtree st
        let c = u_targets st
        let d = u_dummy_range st
        let e = u_ty_new st
        Expr.Match(DebugPointAtBinding.NoneAtSticky, a, b, c, d, e)
    | 11 ->
        let b = u_ty_new st
        let c = (u_option u_Val) st
        let d = u_expr_new st
        let e = u_methods st
        let f = u_intfs st
        let g = u_dummy_range st
        Expr.Obj(newUnique (), b, c, d, e, f, g)
    | 12 ->
        let a = u_constraints st
        let b = u_expr_new st
        let c = u_expr_new st
        let d = u_dummy_range st
        Expr.StaticOptimization(a, b, c, d)
    | 13 ->
        let a = u_tyar_specs_new st
        let b = u_expr_new st
        let c = u_dummy_range st
        Expr.TyChoose(a, b, c)
    | 14 ->
        let b = u_expr_new st
        let c = u_dummy_range st
        let d = u_ty_new st
        Expr.Quote(b, ref None, false, c, d) // isFromQueryExpression=false
    | 15 ->
        let traitInfo = u_trait st
        let m = u_dummy_range st
        Expr.WitnessArg(traitInfo, m)
    | 16 ->
        let m = u_dummy_range st
        let expr = u_expr_new st
        Expr.DebugPoint(DebugPointAtLeafExpr.Yes m, expr)
    | _ -> ufailwith st "u_expr"

and u_exprs_new = u_list u_expr_new

and u_ucref_new st =
    let tcref, caseName, binding =
        u_tup3 u_tcref u_string (u_non_null_slot u_entity_spec_new) st

    tcref.binding <- binding
    UnionCaseRef(tcref, caseName)

and u_op_new st =
    let tag = u_byte st

    match tag with
    | 0 ->
        let a = u_ucref_new st
        TOp.UnionCase a
    | 1 ->
        let a = u_tcref st
        TOp.ExnConstr a
    | 2 -> TOp.Tuple tupInfoRef
    | 3 ->
        let b = u_tcref st
        TOp.Recd(RecdExpr, b)
    | 4 ->
        let a = u_rfref st
        TOp.ValFieldSet a
    | 5 ->
        let a = u_rfref st
        TOp.ValFieldGet a
    | 6 ->
        let a = u_tcref st
        TOp.UnionCaseTagGet a
    | 7 ->
        let a = u_ucref st
        let b = u_int st
        TOp.UnionCaseFieldGet(a, b)
    | 8 ->
        let a = u_ucref st
        let b = u_int st
        TOp.UnionCaseFieldSet(a, b)
    | 9 ->
        let a = u_tcref st
        let b = u_int st
        TOp.ExnFieldGet(a, b)
    | 10 ->
        let a = u_tcref st
        let b = u_int st
        TOp.ExnFieldSet(a, b)
    | 11 ->
        let a = u_int st
        TOp.TupleFieldGet(tupInfoRef, a)
    | 12 ->
        let a = (u_list u_ILInstr) st
        let b = u_tys st
        TOp.ILAsm(a, b)
    | 13 -> TOp.RefAddrGet false // ok to set the 'readonly' flag on these operands to false on re-read since the flag is only used for typechecking purposes
    | 14 ->
        let a = u_ucref st
        TOp.UnionCaseProof a
    | 15 -> TOp.Coerce
    | 16 ->
        let a = u_trait st
        TOp.TraitCall a
    | 17 ->
        let a = u_lval_op_kind st
        let b = u_vref st
        TOp.LValueOp(a, b)
    | 18 ->
        let a1, a2, a3, a4, a5, a7, a8, a9 =
            (u_tup8 u_bool u_bool u_bool u_bool u_vrefFlags u_bool u_bool u_ILMethodRef) st

        let b = u_tys st
        let c = u_tys st
        let d = u_tys st
        TOp.ILCall(a1, a2, a3, a4, a5, a7, a8, a9, b, c, d)
    | 19 -> TOp.Array
    | 20 -> TOp.While(DebugPointAtWhile.No, NoSpecialWhileLoopMarker)
    | 21 ->
        let dir =
            match u_int st with
            | 0 -> FSharpForLoopUp
            | 1 -> CSharpForLoopUp
            | 2 -> FSharpForLoopDown
            | _ -> failwith "unknown for loop"

        TOp.IntegerForLoop(DebugPointAtFor.No, DebugPointAtInOrTo.No, dir)
    | 22 -> TOp.Bytes(u_bytes st)
    | 23 -> TOp.TryWith(DebugPointAtTry.No, DebugPointAtWith.No)
    | 24 -> TOp.TryFinally(DebugPointAtTry.No, DebugPointAtFinally.No)
    | 25 ->
        let a = u_rfref st
        TOp.ValFieldGetAddr(a, false)
    | 26 -> TOp.UInt16s(u_array u_uint16 st)
    | 27 -> TOp.Reraise
    | 28 ->
        let a = u_ucref st
        let b = u_int st
        TOp.UnionCaseFieldGetAddr(a, b, false)
    | 29 -> TOp.Tuple tupInfoStruct
    | 30 ->
        let a = u_int st
        TOp.TupleFieldGet(tupInfoStruct, a)
    | 31 ->
        let info = u_anonInfo st
        TOp.AnonRecd info
    | 32 ->
        let info = u_anonInfo st
        let n = u_int st
        TOp.AnonRecdGet(info, n)
    | _ -> ufailwith st "u_op"

and u_entity_spec_data_new st : Entity =
    let typars = u_tyar_specs_new st
    let logicalName = u_string st
    let compiledName = u_option u_string st
    let range = u_range st
    let stamp = u_stamp st
    let pubPath = u_option u_pubpath st
    let access = u_access st
    let tyconReprAccess = u_access st
    let attribs = u_attribs st
    let tyconRepr = u_tycon_repr_new st
    let typeAbbrev = u_option u_ty_new st
    let tyconTcaug = u_tcaug_new st
    let _x10 = u_string st
    let kind = u_kind st
    let flags = u_int64 st
    let cpath = u_option u_cpath st
    let modulType = u_lazy u_modul_typ_new st
    let exnInfo = u_exnc_repr st
    let xmlDoc = u_used_space1 u_xmldoc st

    // We use a bit that was unused in the F# 2.0 format to indicate two possible representations in the F# 3.0 tycon_repr format
    //let x7 = x7f (x11 &&& EntityFlags.ReservedBitForPickleFormatTyconReprFlag <> 0L)
    //let x11 = x11 &&& ~~~EntityFlags.ReservedBitForPickleFormatTyconReprFlag

    {
        entity_typars = LazyWithContext.NotLazy typars
        entity_stamp = stamp
        entity_logical_name = logicalName
        entity_range = range
        entity_pubpath = pubPath
        entity_attribs = attribs
        entity_tycon_repr = tyconRepr false
        entity_tycon_tcaug = tyconTcaug
        entity_flags = EntityFlags flags
        entity_cpath = cpath
        entity_modul_type = MaybeLazy.Lazy modulType
        entity_il_repr_cache = newCache ()
        entity_opt_data =
            match compiledName, kind, xmlDoc, typeAbbrev, access, tyconReprAccess, exnInfo with
            | None, TyparKind.Type, None, None, TAccess [], TAccess [], TExnNone -> None
            | _ ->
                Some
                    { Entity.NewEmptyEntityOptData() with
                        entity_compiled_name = compiledName
                        entity_kind = kind
                        entity_xmldoc = defaultArg xmlDoc XmlDoc.Empty
                        entity_xmldocsig = System.String.Empty
                        entity_tycon_abbrev = typeAbbrev
                        entity_accessibility = access
                        entity_tycon_repr_accessibility = tyconReprAccess
                        entity_exn_info = exnInfo
                    }
    }

and u_entity_spec_new st =
    u_osgn_decl st.ientities u_entity_spec_data_new st

and u_ValData_new st =
    let logicalName = u_string st
    let compiledName = u_option u_string st
    let ranges = u_ranges st
    let valType = u_ty_new st
    let stamp = u_stamp st
    let flags = u_int64 st
    let memberInfo = u_option u_member_info st
    let attribs = u_attribs st
    let valReprInfo = u_option u_ValReprInfo st
    let xmlDocSig = u_string st
    let valAccess = u_access st
    let declEntity = u_parentref st
    let valConst = u_option u_const st
    let xmlDoc = u_used_space1 u_xmldoc st

    {
        val_logical_name = logicalName
        val_range =
            (match ranges with
             | None -> range0
             | Some(a, _) -> a)
        val_type = valType
        val_stamp = stamp
        val_flags = ValFlags flags
        val_opt_data =
            match compiledName, ranges, valReprInfo, valConst, valAccess, xmlDoc, memberInfo, declEntity, xmlDocSig, attribs with
            | None, None, None, None, TAccess [], None, None, ParentNone, "", [] -> None
            | _ ->
                Some
                    {
                        val_compiled_name = compiledName
                        val_other_range =
                            (match ranges with
                             | None -> None
                             | Some(_, b) -> Some(b, true))
                        val_defn = None
                        val_repr_info = valReprInfo
                        val_repr_info_for_display = None
                        arg_repr_info_for_display = None
                        val_const = valConst
                        val_access = valAccess
                        val_xmldoc = defaultArg xmlDoc XmlDoc.Empty
                        val_other_xmldoc = None
                        val_member_info = memberInfo
                        val_declaring_entity = declEntity
                        val_xmldocsig = xmlDocSig
                        val_attribs = attribs
                    }
    }

and u_Val_new st = u_osgn_decl st.ivals u_ValData_new st

and u_modul_typ_new st =
    let x1, x3, x5 = u_tup3 u_istype (u_qlist u_Val_new) (u_qlist u_entity_spec_new) st
    ModuleOrNamespaceType(x1, x3, x5)

and u_tcaug_new st : TyconAugmentation =
    let a1, a2, a3, b2, c, d, e, g, _space =
        u_tup9
            (u_option (u_tup2 u_vref u_vref))
            (u_option u_vref)
            (u_option (u_tup3 u_vref u_vref u_vref))
            (u_option (u_tup2 u_vref u_vref))
            (u_list (u_tup2 u_string u_vref))
            (u_list (u_tup3 u_ty_new u_bool u_dummy_range))
            (u_option u_ty_new)
            u_bool
            (u_space 1)
            st

    {
        tcaug_compare = a1
        tcaug_compare_withc = a2
        tcaug_hash_and_equals_withc = a3 |> Option.map (fun (v1, v2, v3) -> (v1, v2, v3, None))
        tcaug_equals = b2
        // only used for code generation and checking - hence don't care about the values when reading back in
        tcaug_hasObjectGetHashCode = false
        tcaug_adhoc_list = ResizeArray<_>(c |> List.map (fun (_, vref) -> (false, vref)))
        tcaug_adhoc = NameMultiMap.ofList c
        tcaug_interfaces = d
        tcaug_super = e
        // pickled type definitions are always closed (i.e. no more intrinsic members allowed)
        tcaug_closed = true
        tcaug_abstract = g
    }

and u_ccu_data st : CcuData =
    let fileName = u_option u_string st
    let ilScopeRef = u_ILScopeRef st
    let stamp = u_stamp st
    let qualifiedName = u_option u_string st
    let sourceCodeDirectory = u_string st
    let isFSharp = u_bool st
#if !NO_TYPEPROVIDERS
    let isProviderGenerated = u_bool st
#endif
    let usesFSharp20PlusQuotations = u_bool st
    let contents = u_entity_spec_data_new st

    {
        FileName = fileName
        ILScopeRef = ilScopeRef
        Stamp = stamp
        QualifiedName = qualifiedName
        SourceCodeDirectory = sourceCodeDirectory
        IsFSharp = isFSharp
#if !NO_TYPEPROVIDERS
        IsProviderGenerated = isProviderGenerated
        InvalidateEvent = Unchecked.defaultof<_>
        ImportProvidedType = Unchecked.defaultof<_>
#endif
        UsesFSharp20PlusQuotations = usesFSharp20PlusQuotations
        Contents = contents
        TryGetILModuleDef = Unchecked.defaultof<_>
        MemberSignatureEquality = Unchecked.defaultof<_>
        TypeForwarders = Unchecked.defaultof<_>
        XmlDocumentationInfo = Unchecked.defaultof<_>
    }

and u_ccuref_new st : CcuThunk =
    let target, name = u_tup2 u_ccu_data u_string st

    { target = target; name = name }

and u_nleref_new st =
    let ccu, strings = u_tup2 u_ccuref_new (u_array u_string) st

    NonLocalEntityRef(ccu, strings)

and u_tcref_new st : EntityRef =
    let tag = u_byte st

    match tag with
    | 0 -> u_local_item_ref st.ientities st |> ERefLocal
    | 1 -> u_nleref_new st |> ERefNonLocal
    | _ -> ufailwith st "u_item_ref"

and u_nonlocal_val_ref_new st : NonLocalValOrMemberRef =
    let a = u_tcref_new st
    let b1 = u_option u_string st
    let b2 = u_bool st
    let b3 = u_string st
    let c = u_int st
    let d = u_option u_ty_new st

    {
        EnclosingEntity = a
        ItemKey =
            ValLinkageFullKey(
                {
                    MemberParentMangledName = b1
                    MemberIsOverride = b2
                    LogicalName = b3
                    TotalArgCount = c
                },
                d
            )
    }

and u_vref_new st : ValRef =
    let tag = u_byte st

    match tag with
    | 0 -> u_local_item_ref st.ivals st |> VRefLocal
    | 1 -> u_nonlocal_val_ref_new st |> VRefNonLocal
    | _ -> ufailwith st "u_item_ref"

and u_bind_new st =
    let a = u_Val_new st
    let b = u_expr_new st
    TBind(a, b, DebugPointAtBinding.NoneAtSticky)

and u_binding st : ModuleOrNamespaceBinding =
    let tag = u_byte st

    match tag with
    | 0 ->
        let binding = u_bind st
        ModuleOrNamespaceBinding.Binding binding
    | 1 ->
        let moduleOrNamespace, moduleOrNamespaceContents =
            u_tup2 u_entity_spec_new u_module_or_namespace_contents st

        ModuleOrNamespaceBinding.Module(moduleOrNamespace, moduleOrNamespaceContents)
    | _ -> ufailwith st (nameof u_binding)

and u_tycon_repr_new st =
    let tag1 = u_byte st

    match tag1 with
    | 0 -> (fun _flagBit -> TNoRepr)
    | 1 ->
        let tag2 = u_byte st

        match tag2 with
        // Records historically use a different format to other FSharpTyconRepr
        | 0 ->
            let v = u_rfield_table st

            (fun _flagBit ->
                TFSharpTyconRepr
                    {
                        fsobjmodel_cases = Construct.MakeUnionCases []
                        fsobjmodel_kind = TFSharpRecord
                        fsobjmodel_vslots = []
                        fsobjmodel_rfields = v
                    })

        // Unions  without static fields historically use a different format to other FSharpTyconRepr
        | 1 ->
            let v = u_list u_unioncase_spec st
            (fun _flagBit -> Construct.MakeUnionRepr v)

        | 2 ->
            let v = u_ILType st
            // This is the F# 3.0 extension to the format used for F# provider-generated types, which record an ILTypeRef in the format
            // You can think of an F# 2.0 reader as always taking the path where 'flagBit' is false. Thus the F# 2.0 reader will
            // interpret provider-generated types as TAsmRepr.
            (fun flagBit ->
                if flagBit then
                    let iltref = v.TypeRef

                    match st.iILModule with
                    | None -> TNoRepr
                    | Some iILModule ->
                        try
                            let rec find acc enclosingTypeNames (tdefs: ILTypeDefs) =
                                match enclosingTypeNames with
                                | [] -> List.rev acc, tdefs.FindByName iltref.Name
                                | h :: t ->
                                    let nestedTypeDef = tdefs.FindByName h
                                    find (nestedTypeDef :: acc) t nestedTypeDef.NestedTypes

                            let nestedILTypeDefs, ilTypeDef = find [] iltref.Enclosing iILModule.TypeDefs
                            TILObjectRepr(TILObjectReprData(st.iilscope, nestedILTypeDefs, ilTypeDef))
                        with _ ->
                            System.Diagnostics.Debug.Assert(
                                false,
                                sprintf "failed to find IL backing metadata for cross-assembly generated type %s" iltref.FullName
                            )

                            TNoRepr
                else
                    TAsmRepr v)

        | 3 ->
            let v = u_tycon_objmodel_data st
            (fun _flagBit -> TFSharpTyconRepr v)

        | 4 ->
            let v = u_ty st
            (fun _flagBit -> TMeasureableRepr v)

        | _ -> ufailwith st "u_tycon_repr"

    // Unions with static fields use a different format to other FSharpTyconRepr
    | 2 ->
        let cases = u_array u_unioncase_spec st
        let data = u_tycon_objmodel_data st

        fun _flagBit ->
            TFSharpTyconRepr
                { data with
                    fsobjmodel_cases = Construct.MakeUnionCases(Array.toList cases)
                }

    | 5 ->
        //    | TILObjectRepr (TILObjectReprData (scope, nesting, td)) ->
        let scope = u_ILScopeRef st
        let nesting = u_list u_ILTypeDef st
        let definition = u_ILTypeDef st

        (fun _flagBit -> TILObjectRepr(TILObjectReprData(scope, nesting, definition)))

    | _ -> ufailwith st "u_tycon_repr"

and u_qualified_name_of_file st =
    let ident = u_ident st
    QualifiedNameOfFile(ident)

and u_pragma st =
    let range, warningNumber = u_tup2 u_range u_int st

    ScopedPragma.WarningOff(range, warningNumber)

and u_pragmas st = u_list u_pragma st

and u_long_ident st = u_list u_ident st

and u_trivia st : SyntaxTrivia.IdentTrivia = ufailwith st (nameof p_trivia)

and u_syn_long_ident st =
    let id, dotRanges, trivia =
        u_tup3 u_long_ident (u_list u_range) (u_list (u_option u_trivia)) st

    SynLongIdent(id, dotRanges, trivia)

and u_syn_type st : SynType = ufailwith st (nameof u_syn_type)

and u_syn_open_decl_target st : SynOpenDeclTarget =
    let tag = u_byte st

    match tag with
    | 0 ->
        let longId, range = u_tup2 u_syn_long_ident u_range st

        SynOpenDeclTarget.ModuleOrNamespace(longId, range)
    | 1 ->
        let typeName, range = u_tup2 u_syn_type u_range st
        SynOpenDeclTarget.Type(typeName, range)
    | _ -> ufailwith st (nameof u_syn_open_decl_target)

and u_tup_info st : TupInfo =
    let c = u_bool st
    TupInfo.Const c

and u_nullness st =
    let tag = u_byte st

    let nullnessInfo =
        match tag with
        | 0 -> NullnessInfo.WithNull
        | 1 -> NullnessInfo.WithoutNull
        | 2 -> NullnessInfo.AmbivalentToNull
        | _ -> ufailwith st (nameof u_nullness)

    Nullness.Known nullnessInfo

and u_typars = u_list u_tpref

and u_module_or_namespace_contents st : ModuleOrNamespaceContents =
    let tag = u_byte st

    match tag with
    | 0 ->
        let defs = u_list u_module_or_namespace_contents st
        TMDefs defs
    | 1 ->
        let openDecls = u_list u_open_decl st
        TMDefOpens openDecls
    | 2 ->
        let binding, range = u_tup2 u_bind_new u_range st
        TMDefLet(binding, range)
    | 3 ->
        let expr, range = u_tup2 u_expr_new u_range st
        TMDefDo(expr, range)
    | 4 ->
        let isRec, opens, tycons, bindings, range =
            u_tup5 u_bool (u_list u_open_decl) (u_list u_entity_spec_data_new) (u_list u_binding) u_range st

        TMDefRec(isRec, opens, tycons, bindings, range)
    | _ -> ufailwith st (nameof u_module_or_namespace_contents)

and u_checked_impl_file_contents = u_module_or_namespace_contents

and u_named_debug_point_key st : NamedDebugPointKey =
    let range, name = u_tup2 u_range u_string st

    { Range = range; Name = name }

and u_named_debug_points = u_Map u_named_debug_point_key u_range

and u_anon_recd_types = u_stamp_map u_anonInfo

and u_open_decl st : OpenDeclaration =
    let target, range, modules, types, appliedScope, isOwnNamespace =
        u_tup6 u_syn_open_decl_target (u_option u_range) (u_list u_tcref_new) u_tys u_range u_bool st

    {
        Target = target
        Range = range
        Modules = modules
        Types = types
        AppliedScope = appliedScope
        IsOwnNamespace = isOwnNamespace
    }

and u_checked_impl_file st =
    let qualifiedNameOfFile, pragmas, signature, contents, hasExplicitEntryPoint, isScript, anonRecdTypeInfo, namedDebugPointsForInlinedCode =
        u_tup8
            u_qualified_name_of_file
            u_pragmas
            u_modul_typ_new
            u_checked_impl_file_contents
            u_bool
            u_bool
            u_anon_recd_types
            u_named_debug_points
            st

    CheckedImplFile(
        qualifiedNameOfFile,
        pragmas,
        signature,
        contents,
        hasExplicitEntryPoint,
        isScript,
        anonRecdTypeInfo,
        namedDebugPointsForInlinedCode
    )

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
