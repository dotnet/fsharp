// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.TcGlobals

/// Signals how checker/compiler was invoked - from FSC task/process (a one-off compilation), from tooling or from interactive session.
/// This is used to determine if we want to use certain features in the pipeline, for example, type subsumption cache is only used in one-off compilation now.
[<RequireQualifiedAccess>]
type CompilationMode =
    | Unset // Default: not set
    | OneOff // Running the FSC task/process
    | Service // Running from service
    | Interactive // Running from interactive session

val internal DummyFileNameForRangesWithoutASpecificLocation: string

/// Represents an intrinsic value from FSharp.Core known to the compiler
[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type internal IntrinsicValRef =
    | IntrinsicValRef of TypedTree.NonLocalEntityRef * string * bool * TypedTree.TType * TypedTree.ValLinkageFullKey

    /// For debugging
    override ToString: unit -> string

    /// For debugging
    [<System.Diagnostics.DebuggerBrowsable(enum<System.Diagnostics.DebuggerBrowsableState> 0)>]
    member DebugText: string

    member Name: string

val internal ValRefForIntrinsic: IntrinsicValRef -> TypedTree.ValRef

[<AutoOpen>]
module internal FSharpLib =

    val Root: string

    val RootPath: string list

    val Core: string

    val CorePath: string list

    val CoreOperatorsCheckedName: string

    val ControlName: string

    val LinqName: string

    val CollectionsName: string

    val LanguagePrimitivesName: string

    val CompilerServicesName: string

    val LinqRuntimeHelpersName: string

    val ExtraTopLevelOperatorsName: string

    val NativeInteropName: string

    val QuotationsName: string

    val ControlPath: string list

    val LinqPath: string list

    val CollectionsPath: string list

    val NativeInteropPath: string array

    val CompilerServicesPath: string array

    val LinqRuntimeHelpersPath: string array

    val QuotationsPath: string array

    val RootPathArray: string array

    val CorePathArray: string array

    val LinqPathArray: string array

    val ControlPathArray: string array

    val CollectionsPathArray: string array

[<NoEquality; NoComparison; StructuredFormatDisplay("{DebugText}")>]
type internal BuiltinAttribInfo =
    | AttribInfo of FSharp.Compiler.AbstractIL.IL.ILTypeRef * TypedTree.TyconRef

    /// For debugging
    override ToString: unit -> string

    /// For debugging
    [<System.Diagnostics.DebuggerBrowsable(enum<System.Diagnostics.DebuggerBrowsableState> 0)>]
    member DebugText: string

    member TyconRef: TypedTree.TyconRef

    member TypeRef: FSharp.Compiler.AbstractIL.IL.ILTypeRef

[<Literal>]
val internal tname_InternalsVisibleToAttribute: string = "System.Runtime.CompilerServices.InternalsVisibleToAttribute"

[<Literal>]
val internal tname_DebuggerHiddenAttribute: string = "System.Diagnostics.DebuggerHiddenAttribute"

[<Literal>]
val internal tname_DebuggerStepThroughAttribute: string = "System.Diagnostics.DebuggerStepThroughAttribute"

[<Literal>]
val internal tname_StringBuilder: string = "System.Text.StringBuilder"

[<Literal>]
val internal tname_FormattableString: string = "System.FormattableString"

[<Literal>]
val internal tname_SecurityPermissionAttribute: string = "System.Security.Permissions.SecurityPermissionAttribute"

[<Literal>]
val internal tname_Delegate: string = "System.Delegate"

[<Literal>]
val internal tname_Enum: string = "System.Enum"

[<Literal>]
val internal tname_FlagsAttribute: string = "System.FlagsAttribute"

[<Literal>]
val internal tname_Array: string = "System.Array"

[<Literal>]
val internal tname_RuntimeArgumentHandle: string = "System.RuntimeArgumentHandle"

[<Literal>]
val internal tname_IsByRefLikeAttribute: string = "System.Runtime.CompilerServices.IsByRefLikeAttribute"

type internal TcGlobals =

    new:
        compilingFSharpCore: bool *
        ilg: FSharp.Compiler.AbstractIL.IL.ILGlobals *
        fslibCcu: TypedTree.CcuThunk *
        directoryToResolveRelativePaths: string *
        mlCompatibility: bool *
        isInteractive: bool *
        checkNullness: bool *
        useReflectionFreeCodeGen: bool *
        tryFindSysTypeCcuHelper: (string list -> string -> bool -> TypedTree.CcuThunk option) *
        emitDebugInfoInQuotations: bool *
        noDebugAttributes: bool *
        pathMap: Internal.Utilities.PathMap *
        langVersion: Features.LanguageVersion *
        realsig: bool *
        compilationMode: CompilationMode ->
            TcGlobals

    static member IsInEmbeddableKnownSet: name: string -> bool

    member directoryToResolveRelativePaths: string

    member noDebugAttributes: bool

    member tryFindSysTypeCcuHelper: (string list -> string -> bool -> TypedTree.CcuThunk option) with get

    member AddFieldGeneratedAttributes:
        mdef: FSharp.Compiler.AbstractIL.IL.ILFieldDef -> FSharp.Compiler.AbstractIL.IL.ILFieldDef

    member AddFieldNeverAttributes:
        mdef: FSharp.Compiler.AbstractIL.IL.ILFieldDef -> FSharp.Compiler.AbstractIL.IL.ILFieldDef

    member AddGeneratedAttributes:
        attrs: FSharp.Compiler.AbstractIL.IL.ILAttributes -> FSharp.Compiler.AbstractIL.IL.ILAttributes

    member AddMethodGeneratedAttributes:
        mdef: FSharp.Compiler.AbstractIL.IL.ILMethodDef -> FSharp.Compiler.AbstractIL.IL.ILMethodDef

    member AddPropertyGeneratedAttributes:
        mdef: FSharp.Compiler.AbstractIL.IL.ILPropertyDef -> FSharp.Compiler.AbstractIL.IL.ILPropertyDef

    member AddPropertyNeverAttributes:
        mdef: FSharp.Compiler.AbstractIL.IL.ILPropertyDef -> FSharp.Compiler.AbstractIL.IL.ILPropertyDef

    member AddValGeneratedAttributes: v: TypedTree.Val -> (FSharp.Compiler.Text.range -> unit)

    member FindSysAttrib: nm: string -> BuiltinAttribInfo

    member FindSysILTypeRef: nm: string -> FSharp.Compiler.AbstractIL.IL.ILTypeRef

    member isSpliceOperator: TypedTree.ValRef -> bool

    member FindSysTyconRef: path: string list -> nm: string -> TypedTree.EntityRef

    member HasTailCallAttrib: attribs: TypedTree.Attribs -> bool

    /// Find an FSharp.Core LanguagePrimitives dynamic function that corresponds to a trait witness, e.g.
    /// AdditionDynamic for op_Addition.  Also work out the type instantiation of the dynamic function.
    member MakeBuiltInWitnessInfo: t: TypedTree.TraitConstraintInfo -> IntrinsicValRef * TypedTree.TType list

    member MakeInternalsVisibleToAttribute: simpleAssemName: string -> FSharp.Compiler.AbstractIL.IL.ILAttribute

    member MkDebuggerTypeProxyAttribute:
        ty: FSharp.Compiler.AbstractIL.IL.ILType -> FSharp.Compiler.AbstractIL.IL.ILAttribute

    member TryEmbedILType:
        tref: FSharp.Compiler.AbstractIL.IL.ILTypeRef *
        mkEmbeddableType: (unit -> FSharp.Compiler.AbstractIL.IL.ILTypeDef) ->
            unit

    member TryFindSysAttrib: nm: string -> BuiltinAttribInfo option

    member TryFindSysILTypeRef: nm: string -> FSharp.Compiler.AbstractIL.IL.ILTypeRef option

    member TryFindSysTyconRef: path: string list -> nm: string -> TypedTree.EntityRef option

    /// Find an FSharp.Core operator that corresponds to a trait witness
    member TryMakeOperatorAsBuiltInWitnessInfo:
        isStringTy: (TcGlobals -> TypedTree.TType -> bool) ->
        isArrayTy: (TcGlobals -> TypedTree.TType -> bool) ->
        t: TypedTree.TraitConstraintInfo ->
        argExprs: 'a list ->
            (IntrinsicValRef * TypedTree.TType list * 'a list) option

    member decompileType:
        tcref: TypedTree.EntityRef -> tinst: TypedTree.TypeInst -> (TypedTree.Nullness -> TypedTree.TType)

    member improveType:
        tcref: TypedTree.EntityRef -> tinst: TypedTree.TType list -> (TypedTree.Nullness -> TypedTree.TType)

    /// Memoization table to help minimize the number of ILSourceDocument objects we create
    member memoize_file: x: int -> FSharp.Compiler.AbstractIL.IL.ILSourceDocument

    member mkDebuggableAttributeV2:
        jitTracking: bool * jitOptimizerDisabled: bool -> FSharp.Compiler.AbstractIL.IL.ILAttribute

    member mkDebuggerDisplayAttribute: s: string -> FSharp.Compiler.AbstractIL.IL.ILAttribute

    member mk_ArrayCollector_ty: seqElemTy: TypedTree.TType -> TypedTree.TType

    member mk_GeneratedSequenceBase_ty: seqElemTy: TypedTree.TType -> TypedTree.TType

    member mk_IResumableStateMachine_ty: dataTy: TypedTree.TType -> TypedTree.TType

    member mk_ListCollector_ty: seqElemTy: TypedTree.TType -> TypedTree.TType

    member mk_ResumableStateMachine_ty: dataTy: TypedTree.TType -> TypedTree.TType

    member tryRemoveEmbeddedILTypeDefs: unit -> FSharp.Compiler.AbstractIL.IL.ILTypeDef list

    member unionCaseRefEq: x: TypedTree.UnionCaseRef -> y: TypedTree.UnionCaseRef -> bool

    member valRefEq: x: TypedTree.ValRef -> y: TypedTree.ValRef -> bool

    member CompilerGeneratedAttribute: FSharp.Compiler.AbstractIL.IL.ILAttribute

    member CompilerGlobalState: CompilerGlobalState.CompilerGlobalState option

    member DebuggerBrowsableNeverAttribute: FSharp.Compiler.AbstractIL.IL.ILAttribute

    member DebuggerNonUserCodeAttribute: FSharp.Compiler.AbstractIL.IL.ILAttribute

    member IComparer_ty: TypedTree.TType

    member IEqualityComparer_ty: TypedTree.TType

    member ListCollector_tcr: TypedTree.EntityRef

    member SupportsWhenTEnum_tcr: TypedTree.EntityRef

    member MatchFailureException_tcr: TypedTree.EntityRef

    member ResumableCode_tcr: TypedTree.EntityRef

    member System_Runtime_CompilerServices_RuntimeFeature_ty: TypedTree.TType option

    member addrof2_vref: TypedTree.ValRef

    member addrof_vref: TypedTree.ValRef

    member and2_vref: TypedTree.ValRef

    member and_vref: TypedTree.ValRef

    member array2D_get_info: IntrinsicValRef

    member array2D_get_vref: TypedTree.ValRef

    member array2D_set_info: IntrinsicValRef

    member array3D_get_info: IntrinsicValRef

    member array3D_get_vref: TypedTree.ValRef

    member array3D_set_info: IntrinsicValRef

    member array4D_get_info: IntrinsicValRef

    member array4D_get_vref: TypedTree.ValRef

    member array4D_set_info: IntrinsicValRef

    member array_get_info: IntrinsicValRef

    member array_get_vref: TypedTree.ValRef

    member array_length_info: IntrinsicValRef

    member array_set_info: IntrinsicValRef

    member array_tcr_nice: TypedTree.EntityRef

    member attrib_AbstractClassAttribute: BuiltinAttribInfo

    member attrib_AllowNullLiteralAttribute: BuiltinAttribInfo

    member attrib_AttributeUsageAttribute: BuiltinAttribInfo

    member attrib_AutoOpenAttribute: BuiltinAttribInfo

    member attrib_AutoSerializableAttribute: BuiltinAttribInfo

    member attrib_CLIEventAttribute: BuiltinAttribInfo

    member attrib_CLIMutableAttribute: BuiltinAttribInfo

    member attrib_CallerFilePathAttribute: BuiltinAttribInfo

    member attrib_CallerLineNumberAttribute: BuiltinAttribInfo

    member attrib_CallerMemberNameAttribute: BuiltinAttribInfo

    member attrib_ClassAttribute: BuiltinAttribInfo

    member attrib_ComImportAttribute: BuiltinAttribInfo option

    member attrib_ComVisibleAttribute: BuiltinAttribInfo

    member attrib_ComparisonConditionalOnAttribute: BuiltinAttribInfo

    member attrib_CompilationArgumentCountsAttribute: BuiltinAttribInfo

    member attrib_CompilationMappingAttribute: BuiltinAttribInfo

    member attrib_CompilationRepresentationAttribute: BuiltinAttribInfo

    member attrib_CompiledNameAttribute: BuiltinAttribInfo

    member attrib_CompilerFeatureRequiredAttribute: BuiltinAttribInfo

    member attrib_CompilerMessageAttribute: BuiltinAttribInfo

    member attrib_ComponentModelEditorBrowsableAttribute: BuiltinAttribInfo

    member attrib_ConditionalAttribute: BuiltinAttribInfo

    member attrib_ContextStaticAttribute: BuiltinAttribInfo option

    member attrib_CustomComparisonAttribute: BuiltinAttribInfo

    member attrib_CustomEqualityAttribute: BuiltinAttribInfo

    member attrib_CustomOperationAttribute: BuiltinAttribInfo

    member attrib_DebuggerDisplayAttribute: BuiltinAttribInfo

    member attrib_DebuggerTypeProxyAttribute: BuiltinAttribInfo

    member attrib_DefaultAugmentationAttribute: BuiltinAttribInfo

    member attrib_DefaultMemberAttribute: BuiltinAttribInfo

    member attrib_DefaultParameterValueAttribute: BuiltinAttribInfo option

    member attrib_DefaultValueAttribute: BuiltinAttribInfo

    member attrib_DllImportAttribute: BuiltinAttribInfo option

    member attrib_DynamicDependencyAttribute: BuiltinAttribInfo

    member attrib_EntryPointAttribute: BuiltinAttribInfo

    member attrib_EqualityConditionalOnAttribute: BuiltinAttribInfo

    member attrib_ExperimentalAttribute: BuiltinAttribInfo

    member attrib_ExtensionAttribute: BuiltinAttribInfo

    member attrib_FieldOffsetAttribute: BuiltinAttribInfo

    member attrib_FlagsAttribute: BuiltinAttribInfo

    member attrib_GeneralizableValueAttribute: BuiltinAttribInfo

    member attrib_IDispatchConstantAttribute: BuiltinAttribInfo option

    member attrib_IUnknownConstantAttribute: BuiltinAttribInfo option

    member attrib_InAttribute: BuiltinAttribInfo

    member attrib_InlineIfLambdaAttribute: BuiltinAttribInfo

    member attrib_InterfaceAttribute: BuiltinAttribInfo

    member attrib_InternalsVisibleToAttribute: BuiltinAttribInfo

    member attrib_IsReadOnlyAttribute: BuiltinAttribInfo

    member attrib_IsUnmanagedAttribute: BuiltinAttribInfo

    member attrib_LiteralAttribute: BuiltinAttribInfo

    member attrib_MarshalAsAttribute: BuiltinAttribInfo option

    member attrib_MeasureAttribute: BuiltinAttribInfo

    member attrib_MeasureableAttribute: BuiltinAttribInfo

    member attrib_MemberNotNullWhenAttribute: BuiltinAttribInfo

    member attrib_MethodImplAttribute: BuiltinAttribInfo

    member attrib_NoComparisonAttribute: BuiltinAttribInfo

    member attrib_NoCompilerInliningAttribute: BuiltinAttribInfo

    member attrib_NoDynamicInvocationAttribute: BuiltinAttribInfo

    member attrib_NoEagerConstraintApplicationAttribute: BuiltinAttribInfo

    member attrib_NoEqualityAttribute: BuiltinAttribInfo

    member attrib_NonSerializedAttribute: BuiltinAttribInfo option

    member attrib_NullableAttribute: BuiltinAttribInfo

    member attrib_NullableAttribute_opt: BuiltinAttribInfo option

    member attrib_NullableContextAttribute: BuiltinAttribInfo

    member attrib_NullableContextAttribute_opt: BuiltinAttribInfo option

    member attrib_OptionalArgumentAttribute: BuiltinAttribInfo

    member attrib_OptionalAttribute: BuiltinAttribInfo option

    member attrib_OutAttribute: BuiltinAttribInfo

    member attrib_ParamArrayAttribute: BuiltinAttribInfo

    member attrib_PreserveSigAttribute: BuiltinAttribInfo option

    member attrib_ProjectionParameterAttribute: BuiltinAttribInfo

    member attrib_ReferenceEqualityAttribute: BuiltinAttribInfo

    member attrib_ReflectedDefinitionAttribute: BuiltinAttribInfo

    member attrib_RequireQualifiedAccessAttribute: BuiltinAttribInfo

    member attrib_RequiredMemberAttribute: BuiltinAttribInfo

    member attrib_RequiresExplicitTypeArgumentsAttribute: BuiltinAttribInfo

    member attrib_RequiresLocationAttribute: BuiltinAttribInfo

    member attrib_SealedAttribute: BuiltinAttribInfo

    member attrib_SecurityAttribute: BuiltinAttribInfo option

    member attrib_SecurityCriticalAttribute: BuiltinAttribInfo

    member attrib_SecuritySafeCriticalAttribute: BuiltinAttribInfo

    member attrib_SetsRequiredMembersAttribute: BuiltinAttribInfo

    member attrib_SkipLocalsInitAttribute: BuiltinAttribInfo

    member attrib_DecimalConstantAttribute: BuiltinAttribInfo

    member attrib_StructAttribute: BuiltinAttribInfo

    member attrib_StructLayoutAttribute: BuiltinAttribInfo

    member attrib_StructuralComparisonAttribute: BuiltinAttribInfo

    member attrib_StructuralEqualityAttribute: BuiltinAttribInfo

    member attrib_SystemObsolete: BuiltinAttribInfo

    member attrib_IsByRefLikeAttribute_opt: BuiltinAttribInfo option

    member attrib_ThreadStaticAttribute: BuiltinAttribInfo option

    member attrib_TypeForwardedToAttribute: BuiltinAttribInfo

    member attrib_UnverifiableAttribute: BuiltinAttribInfo

    member attrib_VolatileFieldAttribute: BuiltinAttribInfo

    member attrib_WarnOnWithoutNullArgumentAttribute: BuiltinAttribInfo

    member attrib_IlExperimentalAttribute: BuiltinAttribInfo

    member attribs_Unsupported: TypedTree.TyconRef list

    member bitwise_and_info: IntrinsicValRef

    member bitwise_and_vref: TypedTree.ValRef

    member bitwise_or_info: IntrinsicValRef

    member bitwise_or_vref: TypedTree.ValRef

    member bitwise_shift_left_info: IntrinsicValRef

    member bitwise_shift_left_vref: TypedTree.ValRef

    member bitwise_shift_right_info: IntrinsicValRef

    member bitwise_shift_right_vref: TypedTree.ValRef

    member bitwise_unary_not_info: IntrinsicValRef

    member bitwise_unary_not_vref: TypedTree.ValRef

    member bitwise_xor_info: IntrinsicValRef

    member bitwise_xor_vref: TypedTree.ValRef

    member bool_tcr: TypedTree.EntityRef

    member bool_ty: TypedTree.TType

    member box_info: IntrinsicValRef

    member byref2_tcr: TypedTree.EntityRef

    member byref_tcr: TypedTree.EntityRef

    member byrefkind_InOut_tcr: TypedTree.EntityRef

    member byrefkind_In_tcr: TypedTree.EntityRef

    member byrefkind_Out_tcr: TypedTree.EntityRef

    member byte_checked_info: IntrinsicValRef

    member byte_operator_info: IntrinsicValRef

    member byte_tcr: TypedTree.EntityRef

    member byte_ty: TypedTree.TType

    member call_with_witnesses_info: IntrinsicValRef

    member cast_quotation_info: IntrinsicValRef

    member cgh__debugPoint_vref: TypedTree.ValRef

    member cgh__resumableEntry_vref: TypedTree.ValRef

    member cgh__resumeAt_vref: TypedTree.ValRef

    member cgh__stateMachine_vref: TypedTree.ValRef

    member cgh__useResumableCode_vref: TypedTree.ValRef

    member char_operator_info: IntrinsicValRef

    member char_tcr: TypedTree.EntityRef

    member char_ty: TypedTree.TType

    member checkNullness: bool

    member check_this_info: IntrinsicValRef

    member checked_addition_info: IntrinsicValRef

    member checked_multiply_info: IntrinsicValRef

    member checked_subtraction_info: IntrinsicValRef

    member checked_unary_minus_info: IntrinsicValRef

    member choice2_tcr: TypedTree.EntityRef

    member choice3_tcr: TypedTree.EntityRef

    member choice4_tcr: TypedTree.EntityRef

    member choice5_tcr: TypedTree.EntityRef

    member choice6_tcr: TypedTree.EntityRef

    member choice7_tcr: TypedTree.EntityRef

    member compare_operator_vref: TypedTree.ValRef

    member compilingFSharpCore: bool

    member cons_ucref: TypedTree.UnionCaseRef

    member create_event_info: IntrinsicValRef

    member create_instance_info: IntrinsicValRef

    member date_tcr: TypedTree.EntityRef

    member decimal_tcr: TypedTree.EntityRef

    member decimal_ty: TypedTree.TType

    member deserialize_quoted_FSharp_20_plus_info: IntrinsicValRef

    member deserialize_quoted_FSharp_40_plus_info: IntrinsicValRef

    member dispose_info: IntrinsicValRef

    member emitDebugInfoInQuotations: bool

    member enumOfValue_vref: TypedTree.ValRef

    member enum_DynamicallyAccessedMemberTypes: BuiltinAttribInfo

    member enum_operator_info: IntrinsicValRef

    member enum_vref: TypedTree.ValRef

    member equals_nullable_operator_vref: TypedTree.ValRef

    member equals_operator_info: IntrinsicValRef

    member equals_operator_vref: TypedTree.ValRef

    member exn_tcr: TypedTree.EntityRef

    member exn_ty: TypedTree.TType

    member exponentiation_vref: TypedTree.ValRef

    member expr_tcr: TypedTree.EntityRef

    member fail_init_info: IntrinsicValRef

    member fail_static_init_info: IntrinsicValRef

    member failwith_vref: TypedTree.ValRef

    member failwithf_vref: TypedTree.ValRef

    member fastFunc_tcr: TypedTree.EntityRef

    member float32_operator_info: IntrinsicValRef

    member float32_tcr: TypedTree.EntityRef

    member float32_ty: TypedTree.TType

    member float_operator_info: IntrinsicValRef

    member float_tcr: TypedTree.EntityRef

    member float_ty: TypedTree.TType

    member format4_tcr: TypedTree.EntityRef

    member format_tcr: TypedTree.EntityRef

    member fslibCcu: TypedTree.CcuThunk

    member fslib_IDelegateEvent_tcr: TypedTree.EntityRef

    member fslib_IEvent2_tcr: TypedTree.EntityRef

    /// Indicates if we are generating witness arguments for SRTP constraints. Only done if the FSharp.Core
    /// supports witness arguments.
    member generateWitnesses: bool

    member generic_compare_withc_tuple2_vref: TypedTree.ValRef

    member generic_compare_withc_tuple3_vref: TypedTree.ValRef

    member generic_compare_withc_tuple4_vref: TypedTree.ValRef

    member generic_compare_withc_tuple5_vref: TypedTree.ValRef

    member generic_comparison_inner_vref: TypedTree.ValRef

    member generic_comparison_withc_inner_vref: TypedTree.ValRef

    member generic_comparison_withc_outer_info: IntrinsicValRef

    member generic_equality_er_inner_vref: TypedTree.ValRef

    member generic_equality_er_outer_info: IntrinsicValRef

    member generic_equality_per_inner_vref: TypedTree.ValRef

    member generic_equality_withc_inner_vref: TypedTree.ValRef

    member generic_equality_withc_outer_info: IntrinsicValRef

    member generic_equality_withc_outer_vref: TypedTree.ValRef

    member generic_equals_withc_tuple2_vref: TypedTree.ValRef

    member generic_equals_withc_tuple3_vref: TypedTree.ValRef

    member generic_equals_withc_tuple4_vref: TypedTree.ValRef

    member generic_equals_withc_tuple5_vref: TypedTree.ValRef

    member generic_hash_inner_vref: TypedTree.ValRef

    member generic_hash_withc_inner_vref: TypedTree.ValRef

    member generic_hash_withc_outer_info: IntrinsicValRef

    member generic_hash_withc_tuple2_vref: TypedTree.ValRef

    member generic_hash_withc_tuple3_vref: TypedTree.ValRef

    member generic_hash_withc_tuple4_vref: TypedTree.ValRef

    member generic_hash_withc_tuple5_vref: TypedTree.ValRef

    member get_generic_comparer_info: IntrinsicValRef

    member get_generic_er_equality_comparer_info: IntrinsicValRef

    member get_generic_per_equality_comparer_info: IntrinsicValRef

    member getstring_info: IntrinsicValRef

    member greater_than_operator: IntrinsicValRef

    member greater_than_operator_vref: TypedTree.ValRef

    member greater_than_or_equals_operator: IntrinsicValRef

    member greater_than_or_equals_operator_vref: TypedTree.ValRef

    member hash_info: IntrinsicValRef

    member il_arr_tcr_map: TypedTree.EntityRef array

    member ilg: FSharp.Compiler.AbstractIL.IL.ILGlobals

    member ilsigptr_tcr: TypedTree.EntityRef

    member iltyp_AsyncCallback: FSharp.Compiler.AbstractIL.IL.ILType

    member iltyp_Exception: FSharp.Compiler.AbstractIL.IL.ILType

    member iltyp_IAsyncResult: FSharp.Compiler.AbstractIL.IL.ILType

    member iltyp_IComparable: FSharp.Compiler.AbstractIL.IL.ILType

    member iltyp_Missing: FSharp.Compiler.AbstractIL.IL.ILType

    member iltyp_ReferenceAssemblyAttributeOpt: FSharp.Compiler.AbstractIL.IL.ILType option

    member iltyp_RuntimeFieldHandle: FSharp.Compiler.AbstractIL.IL.ILType

    member iltyp_RuntimeMethodHandle: FSharp.Compiler.AbstractIL.IL.ILType

    member iltyp_RuntimeTypeHandle: FSharp.Compiler.AbstractIL.IL.ILType

    member iltyp_SerializationInfo: FSharp.Compiler.AbstractIL.IL.ILType option

    member iltyp_StreamingContext: FSharp.Compiler.AbstractIL.IL.ILType option

    member iltyp_UnmanagedType: FSharp.Compiler.AbstractIL.IL.ILType

    member iltyp_ValueType: FSharp.Compiler.AbstractIL.IL.ILType

    member inref_tcr: TypedTree.EntityRef

    member int16_checked_info: IntrinsicValRef

    member int16_operator_info: IntrinsicValRef

    member int16_tcr: TypedTree.EntityRef

    member int16_ty: TypedTree.TType

    member int32_checked_info: IntrinsicValRef

    member int32_operator_info: IntrinsicValRef

    member int32_tcr: TypedTree.EntityRef

    member int32_ty: TypedTree.TType

    member int64_checked_info: IntrinsicValRef

    member int64_operator_info: IntrinsicValRef

    member int64_tcr: TypedTree.EntityRef

    member int64_ty: TypedTree.TType

    member int_checked_info: IntrinsicValRef

    member int_ty: TypedTree.TType

    member invalid_arg_vref: TypedTree.ValRef

    member invalid_op_vref: TypedTree.ValRef

    /// Indicates if we can use System.Array.Empty when emitting IL for empty array literals
    member isArrayEmptyAvailable: bool

    /// Are we assuming all code gen is for F# interactive, with no static linking
    member isInteractive: bool

    member compilationMode: CompilationMode

    member isnull_info: IntrinsicValRef

    member istype_fast_vref: TypedTree.ValRef

    member istype_info: IntrinsicValRef

    member istype_vref: TypedTree.ValRef

    member knownFSharpCoreModules: System.Collections.Generic.IDictionary<string, TypedTree.EntityRef>

    member knownIntrinsics:
        System.Collections.Concurrent.ConcurrentDictionary<string * string option * string * int, TypedTree.ValRef>

    member knownWithNull: TypedTree.Nullness

    member knownWithoutNull: TypedTree.Nullness

    member langFeatureNullness: bool

    member langVersion: Features.LanguageVersion

    member lazy_create_info: IntrinsicValRef

    member lazy_force_info: IntrinsicValRef

    member lazy_tcr_canon: TypedTree.EntityRef

    member lazy_tcr_nice: TypedTree.EntityRef

    member less_than_operator: IntrinsicValRef

    member less_than_operator_vref: TypedTree.ValRef

    member less_than_or_equals_operator: IntrinsicValRef

    member less_than_or_equals_operator_vref: TypedTree.ValRef

    member lift_value_info: IntrinsicValRef

    member lift_value_with_defn_info: IntrinsicValRef

    member lift_value_with_name_info: IntrinsicValRef

    member list_tcr_canon: TypedTree.EntityRef

    member list_tcr_nice: TypedTree.EntityRef

    member measureinverse_tcr: TypedTree.EntityRef

    member measureone_tcr: TypedTree.EntityRef

    member measureproduct_tcr: TypedTree.EntityRef

    member methodhandleof_vref: TypedTree.ValRef

    member mk_Attribute_ty: TypedTree.TType

    member mk_IAsyncStateMachine_ty: TypedTree.TType

    member mk_IComparable_ty: TypedTree.TType

    member mk_IStructuralComparable_ty: TypedTree.TType

    member mk_IStructuralEquatable_ty: TypedTree.TType

    member mlCompatibility: bool

    member nameof_vref: TypedTree.ValRef

    member nativeint_checked_info: IntrinsicValRef

    member nativeint_operator_info: IntrinsicValRef

    member nativeint_tcr: TypedTree.EntityRef

    member nativeint_ty: TypedTree.TType

    member nativeptr_tcr: TypedTree.EntityRef

    member nativeptr_tobyref_vref: TypedTree.ValRef

    member new_decimal_info: IntrinsicValRef

    member new_format_info: IntrinsicValRef

    member new_format_vref: TypedTree.ValRef

    member new_query_source_info: IntrinsicValRef

    member nil_ucref: TypedTree.UnionCaseRef

    member not_equals_operator: IntrinsicValRef

    member not_equals_operator_vref: TypedTree.ValRef

    member null_arg_vref: TypedTree.ValRef

    member nullable_equals_nullable_operator_vref: TypedTree.ValRef

    member nullable_equals_operator_vref: TypedTree.ValRef

    member obj_ty_ambivalent: TypedTree.TType

    member obj_ty_noNulls: TypedTree.TType

    member obj_ty_withNulls: TypedTree.TType

    member option_defaultValue_info: IntrinsicValRef

    member option_tcr_canon: TypedTree.EntityRef

    member option_tcr_nice: TypedTree.EntityRef

    member option_toNullable_info: IntrinsicValRef

    member or2_vref: TypedTree.ValRef

    member or_vref: TypedTree.ValRef

    member outref_tcr: TypedTree.EntityRef

    member pathMap: Internal.Utilities.PathMap

    member pdecimal_tcr: TypedTree.EntityRef

    member pfloat32_tcr: TypedTree.EntityRef

    member pfloat_tcr: TypedTree.EntityRef

    member pint16_tcr: TypedTree.EntityRef

    member pint64_tcr: TypedTree.EntityRef

    member pint8_tcr: TypedTree.EntityRef

    member pint_tcr: TypedTree.EntityRef

    member piperight2_vref: TypedTree.ValRef

    member piperight3_vref: TypedTree.ValRef

    member piperight_vref: TypedTree.ValRef

    member pnativeint_tcr: TypedTree.EntityRef

    member puint16_tcr: TypedTree.EntityRef

    member puint64_tcr: TypedTree.EntityRef

    member puint8_tcr: TypedTree.EntityRef

    member puint_tcr: TypedTree.EntityRef

    member punativeint_tcr: TypedTree.EntityRef

    member query_builder_tcref: TypedTree.EntityRef

    member query_for_vref: TypedTree.ValRef

    member query_run_enumerable_vref: TypedTree.ValRef

    member query_run_value_vref: TypedTree.ValRef

    member query_select_vref: TypedTree.ValRef

    member query_source_as_enum_info: IntrinsicValRef

    member query_source_vref: TypedTree.ValRef

    member query_value_vref: TypedTree.ValRef

    member query_yield_from_vref: TypedTree.ValRef

    member query_yield_vref: TypedTree.ValRef

    member query_zero_vref: TypedTree.ValRef

    member quote_to_linq_lambda_info: IntrinsicValRef

    member raise_info: IntrinsicValRef

    member raise_vref: TypedTree.ValRef

    member range_byte_op_vref: TypedTree.ValRef

    member range_char_op_vref: TypedTree.ValRef

    member range_generic_op_vref: TypedTree.ValRef

    member range_int16_op_vref: TypedTree.ValRef

    member range_int32_op_vref: TypedTree.ValRef

    member range_int64_op_vref: TypedTree.ValRef

    member range_nativeint_op_vref: TypedTree.ValRef

    member range_op_vref: TypedTree.ValRef

    member range_sbyte_op_vref: TypedTree.ValRef

    member range_step_generic_op_vref: TypedTree.ValRef

    member range_step_op_vref: TypedTree.ValRef

    member range_uint16_op_vref: TypedTree.ValRef

    member range_uint32_op_vref: TypedTree.ValRef

    member range_uint64_op_vref: TypedTree.ValRef

    member range_unativeint_op_vref: TypedTree.ValRef

    member raw_expr_tcr: TypedTree.EntityRef

    member realsig: bool

    member ref_tuple1_tcr: TypedTree.EntityRef

    member ref_tuple2_tcr: TypedTree.EntityRef

    member ref_tuple3_tcr: TypedTree.EntityRef

    member ref_tuple4_tcr: TypedTree.EntityRef

    member ref_tuple5_tcr: TypedTree.EntityRef

    member ref_tuple6_tcr: TypedTree.EntityRef

    member ref_tuple7_tcr: TypedTree.EntityRef

    member ref_tuple8_tcr: TypedTree.EntityRef

    member refcell_assign_vref: TypedTree.ValRef

    member refcell_decr_vref: TypedTree.ValRef

    member refcell_deref_vref: TypedTree.ValRef

    member refcell_incr_vref: TypedTree.ValRef

    member refcell_tcr_canon: TypedTree.EntityRef

    member refcell_tcr_nice: TypedTree.EntityRef

    member reference_equality_inner_vref: TypedTree.ValRef

    member reraise_info: IntrinsicValRef

    member reraise_vref: TypedTree.ValRef

    member sbyte_checked_info: IntrinsicValRef

    member sbyte_operator_info: IntrinsicValRef

    member sbyte_tcr: TypedTree.EntityRef

    member sbyte_ty: TypedTree.TType

    member seq_append_info: IntrinsicValRef

    member seq_append_vref: TypedTree.ValRef

    member seq_collect_info: IntrinsicValRef

    member seq_collect_vref: TypedTree.ValRef

    member seq_delay_info: IntrinsicValRef

    member seq_delay_vref: TypedTree.ValRef

    member seq_empty_info: IntrinsicValRef

    member seq_empty_vref: TypedTree.ValRef

    member seq_finally_info: IntrinsicValRef

    member seq_finally_vref: TypedTree.ValRef

    member seq_generated_info: IntrinsicValRef

    member seq_generated_vref: TypedTree.ValRef

    member seq_info: IntrinsicValRef

    member seq_map_info: IntrinsicValRef

    member seq_map_vref: TypedTree.ValRef

    member seq_of_functions_info: IntrinsicValRef

    member seq_singleton_info: IntrinsicValRef

    member seq_singleton_vref: TypedTree.ValRef

    member seq_tcr: TypedTree.EntityRef

    member seq_to_array_info: IntrinsicValRef

    member seq_to_array_vref: TypedTree.ValRef

    member seq_to_list_info: IntrinsicValRef

    member seq_to_list_vref: TypedTree.ValRef

    member seq_trywith_info: IntrinsicValRef

    member seq_using_info: IntrinsicValRef

    member seq_using_vref: TypedTree.ValRef

    member seq_vref: TypedTree.ValRef

    member sizeof_vref: TypedTree.ValRef

    member splice_expr_vref: TypedTree.ValRef

    member splice_raw_expr_vref: TypedTree.ValRef

    member sprintf_info: IntrinsicValRef

    member sprintf_vref: TypedTree.ValRef

    member string_ty: TypedTree.TType

    member string_ty_ambivalent: TypedTree.TType

    member struct_tuple1_tcr: TypedTree.EntityRef

    member struct_tuple2_tcr: TypedTree.EntityRef

    member struct_tuple3_tcr: TypedTree.EntityRef

    member struct_tuple4_tcr: TypedTree.EntityRef

    member struct_tuple5_tcr: TypedTree.EntityRef

    member struct_tuple6_tcr: TypedTree.EntityRef

    member struct_tuple7_tcr: TypedTree.EntityRef

    member struct_tuple8_tcr: TypedTree.EntityRef

    member suppressed_types: TypedTree.EntityRef list

    member system_ArgIterator_tcref: TypedTree.EntityRef option

    member system_Array_ty: TypedTree.TType

    member system_Bool_tcref: TypedTree.EntityRef

    member system_Byte_tcref: TypedTree.EntityRef

    member system_Char_tcref: TypedTree.EntityRef

    member system_Decimal_tcref: TypedTree.EntityRef

    member system_Delegate_ty: TypedTree.TType

    member system_Double_tcref: TypedTree.EntityRef

    member system_Enum_ty: TypedTree.TType

    member system_ExceptionDispatchInfo_ty: TypedTree.TType option

    member system_FormattableStringFactory_ty: TypedTree.TType

    member system_FormattableString_tcref: TypedTree.EntityRef

    member system_FormattableString_ty: TypedTree.TType

    member system_GenericIComparable_tcref: TypedTree.EntityRef

    member system_GenericIEquatable_tcref: TypedTree.EntityRef

    member system_IDisposable_ty: TypedTree.TType

    member system_IDisposableNull_ty: TypedTree.TType

    member system_IFormattable_tcref: TypedTree.EntityRef

    member system_IFormattable_ty: TypedTree.TType

    member system_Int16_tcref: TypedTree.EntityRef

    member system_Int32_tcref: TypedTree.EntityRef

    member system_Int64_tcref: TypedTree.EntityRef

    member system_IntPtr_tcref: TypedTree.EntityRef

    member system_LinqExpression_tcref: TypedTree.EntityRef

    member system_MarshalByRefObject_tcref: TypedTree.EntityRef option

    member system_MarshalByRefObject_ty: TypedTree.TType option

    member system_MulticastDelegate_ty: TypedTree.TType

    member system_Nullable_tcref: TypedTree.EntityRef

    member system_Object_tcref: TypedTree.EntityRef

    member system_Object_ty: TypedTree.TType

    member system_RuntimeArgumentHandle_tcref: TypedTree.EntityRef option

    member system_RuntimeHelpers_ty: TypedTree.TType

    member system_RuntimeTypeHandle_ty: TypedTree.TType

    member system_SByte_tcref: TypedTree.EntityRef

    member system_Single_tcref: TypedTree.EntityRef

    member system_String_tcref: TypedTree.EntityRef

    member system_Type_ty: TypedTree.TType

    member system_TypedReference_tcref: TypedTree.EntityRef option

    member system_UInt16_tcref: TypedTree.EntityRef

    member system_UInt32_tcref: TypedTree.EntityRef

    member system_UInt64_tcref: TypedTree.EntityRef

    member system_UIntPtr_tcref: TypedTree.EntityRef

    member system_Value_tcref: TypedTree.EntityRef

    member system_Value_ty: TypedTree.TType

    member system_Void_tcref: TypedTree.EntityRef

    member tcref_IObservable: TypedTree.EntityRef

    member tcref_IObserver: TypedTree.EntityRef

    member tcref_LanguagePrimitives: TypedTree.EntityRef

    member tcref_System_Attribute: TypedTree.EntityRef

    member tcref_System_Collections_Generic_Dictionary: TypedTree.EntityRef

    member tcref_System_Collections_Generic_ICollection: TypedTree.EntityRef

    member tcref_System_Collections_Generic_IEnumerable: TypedTree.EntityRef

    member tcref_System_Collections_Generic_IEnumerator: TypedTree.EntityRef

    member tcref_System_Collections_Generic_IEqualityComparer: TypedTree.EntityRef

    member tcref_System_Collections_Generic_IList: TypedTree.EntityRef

    member tcref_System_Collections_Generic_IReadOnlyCollection: TypedTree.EntityRef

    member tcref_System_Collections_Generic_IReadOnlyList: TypedTree.EntityRef

    member tcref_System_Collections_IComparer: TypedTree.EntityRef

    member tcref_System_Collections_IEnumerable: TypedTree.EntityRef

    member tcref_System_Collections_IEqualityComparer: TypedTree.EntityRef

    member tcref_System_IComparable: TypedTree.EntityRef

    member tcref_System_IDisposable: TypedTree.EntityRef

    member tcref_System_IStructuralComparable: TypedTree.EntityRef

    member tcref_System_IStructuralEquatable: TypedTree.EntityRef

    member typedefof_info: IntrinsicValRef

    member typedefof_vref: TypedTree.ValRef

    member typeof_info: IntrinsicValRef

    member typeof_vref: TypedTree.ValRef

    member uint16_checked_info: IntrinsicValRef

    member uint16_operator_info: IntrinsicValRef

    member uint16_tcr: TypedTree.EntityRef

    member uint16_ty: TypedTree.TType

    member uint32_checked_info: IntrinsicValRef

    member uint32_operator_info: IntrinsicValRef

    member uint32_tcr: TypedTree.EntityRef

    member uint32_ty: TypedTree.TType

    member uint64_checked_info: IntrinsicValRef

    member uint64_operator_info: IntrinsicValRef

    member uint64_tcr: TypedTree.EntityRef

    member uint64_ty: TypedTree.TType

    member unativeint_checked_info: IntrinsicValRef

    member unativeint_operator_info: IntrinsicValRef

    member unativeint_ty: TypedTree.TType

    member unbox_fast_info: IntrinsicValRef

    member unbox_fast_vref: TypedTree.ValRef

    member unbox_info: IntrinsicValRef

    member unbox_vref: TypedTree.ValRef

    member unchecked_addition_info: IntrinsicValRef

    member unchecked_addition_vref: TypedTree.ValRef

    member unchecked_defaultof_info: IntrinsicValRef

    member unchecked_defaultof_vref: TypedTree.ValRef

    member unchecked_division_info: IntrinsicValRef

    member unchecked_division_vref: TypedTree.ValRef

    member unchecked_modulus_info: IntrinsicValRef

    member unchecked_modulus_vref: TypedTree.ValRef

    member unchecked_multiply_info: IntrinsicValRef

    member unchecked_multiply_vref: TypedTree.ValRef

    member unchecked_subtraction_info: IntrinsicValRef

    member unchecked_subtraction_vref: TypedTree.ValRef

    member unchecked_unary_minus_info: IntrinsicValRef

    member unchecked_unary_minus_vref: TypedTree.ValRef

    member unchecked_unary_not_vref: TypedTree.ValRef

    member unchecked_unary_plus_vref: TypedTree.ValRef

    member unit_tcr_canon: TypedTree.EntityRef

    member unit_ty: TypedTree.TType

    member useReflectionFreeCodeGen: bool

    member valueoption_tcr_canon: TypedTree.EntityRef

    member valueoption_tcr_nice: TypedTree.EntityRef

    member voidptr_tcr: TypedTree.EntityRef

#if DEBUG
// This global is only used during debug output
val mutable internal global_g: TcGlobals option
#endif
