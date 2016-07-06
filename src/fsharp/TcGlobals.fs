// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Defines the global environment for all type checking.
///
/// The environment (TcGlobals) are well-known types and values are hard-wired 
/// into the compiler.  This lets the compiler perform particular optimizations
/// for these types and values, for example emitting optimized calls for
/// comparison and hashing functions.  
module internal Microsoft.FSharp.Compiler.TcGlobals

open Internal.Utilities
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library

open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.PrettyNaming

open System.Collections.Generic

let internal DummyFileNameForRangesWithoutASpecificLocation = "startup"
let private envRange = rangeN DummyFileNameForRangesWithoutASpecificLocation 0

type public IntrinsicValRef = IntrinsicValRef of NonLocalEntityRef * string * bool * TType * ValLinkageFullKey

let ValRefForIntrinsic (IntrinsicValRef(mvr,_,_,_,key))  = mkNonLocalValRef mvr key

//-------------------------------------------------------------------------
// Access the initial environment: names
//------------------------------------------------------------------------- 

[<AutoOpen>]
module FSharpLib = 

    let CoreOperatorsName        = FSharpLib.Root + ".Core.Operators"
    let CoreOperatorsCheckedName = FSharpLib.Root + ".Core.Operators.Checked"
    let ControlName              = FSharpLib.Root + ".Control"
    let LinqName                 = FSharpLib.Root + ".Linq"
    let CollectionsName          = FSharpLib.Root + ".Collections"
    let LanguagePrimitivesName   = FSharpLib.Root + ".Core.LanguagePrimitives"
    let CompilerServicesName     = FSharpLib.Root + ".Core.CompilerServices"
    let LinqRuntimeHelpersName   = FSharpLib.Root + ".Linq.RuntimeHelpers"
    let RuntimeHelpersName       = FSharpLib.Root + ".Core.CompilerServices.RuntimeHelpers"
    let ExtraTopLevelOperatorsName = FSharpLib.Root + ".Core.ExtraTopLevelOperators" 
    let HashCompareName            = FSharpLib.Root + ".Core.LanguagePrimitives.HashCompare"

    let QuotationsName             = FSharpLib.Root + ".Quotations"

    let OperatorsPath               = IL.splitNamespace CoreOperatorsName |> Array.ofList
    let OperatorsCheckedPath        = IL.splitNamespace CoreOperatorsCheckedName |> Array.ofList
    let ControlPath                 = IL.splitNamespace ControlName 
    let LinqPath                    = IL.splitNamespace LinqName 
    let CollectionsPath             = IL.splitNamespace CollectionsName 
    let LanguagePrimitivesPath      = IL.splitNamespace LanguagePrimitivesName |> Array.ofList
    let HashComparePath             = IL.splitNamespace HashCompareName |> Array.ofList
    let CompilerServicesPath        = IL.splitNamespace CompilerServicesName |> Array.ofList
    let LinqRuntimeHelpersPath      = IL.splitNamespace LinqRuntimeHelpersName |> Array.ofList
    let RuntimeHelpersPath          = IL.splitNamespace RuntimeHelpersName |> Array.ofList
    let QuotationsPath              = IL.splitNamespace QuotationsName |> Array.ofList
    let ExtraTopLevelOperatorsPath  = IL.splitNamespace ExtraTopLevelOperatorsName |> Array.ofList

    let RootPathArray                    = FSharpLib.RootPath |> Array.ofList
    let CorePathArray                    = FSharpLib.CorePath |> Array.ofList
    let LinqPathArray                    = LinqPath |> Array.ofList
    let ControlPathArray                 = ControlPath |> Array.ofList
    let CollectionsPathArray             = CollectionsPath |> Array.ofList

//-------------------------------------------------------------------------
// Access the initial environment: helpers to build references
//-------------------------------------------------------------------------

let private mkNonGenericTy tcref = TType_app(tcref,[])

let mkNonLocalTyconRef2 ccu path n = mkNonLocalTyconRef (mkNonLocalEntityRef ccu path) n 

let mk_MFCore_tcref             ccu n = mkNonLocalTyconRef2 ccu FSharpLib.CorePathArray n 
let mk_MFQuotations_tcref       ccu n = mkNonLocalTyconRef2 ccu FSharpLib.QuotationsPath n 
let mk_MFLinq_tcref             ccu n = mkNonLocalTyconRef2 ccu LinqPathArray n 
let mk_MFCollections_tcref      ccu n = mkNonLocalTyconRef2 ccu FSharpLib.CollectionsPathArray n 
let mk_MFCompilerServices_tcref ccu n = mkNonLocalTyconRef2 ccu FSharpLib.CompilerServicesPath n 
let mk_MFRuntimeHelpers_tcref   ccu n = mkNonLocalTyconRef2 ccu FSharpLib.RuntimeHelpersPath n 
let mk_MFControl_tcref          ccu n = mkNonLocalTyconRef2 ccu FSharpLib.ControlPathArray n 


type public BuiltinAttribInfo =
    | AttribInfo of ILTypeRef * TyconRef 
    member this.TyconRef = let (AttribInfo(_,tcref)) = this in tcref
    member this.TypeRef  = let (AttribInfo(tref,_)) = this in tref

//-------------------------------------------------------------------------
// Table of all these "globals"
//------------------------------------------------------------------------- 

[<NoEquality; NoComparison>]
type public TcGlobals = 
    { ilg : ILGlobals
#if NO_COMPILER_BACKEND
#else
      ilxPubCloEnv : EraseClosures.cenv
#endif
      emitDebugInfoInQuotations : bool
      compilingFslib: bool
      mlCompatibility : bool
      directoryToResolveRelativePaths : string
      fslibCcu: CcuThunk 
      sysCcu: CcuThunk 
      using40environment: bool
      better_tcref_map: TyconRef -> TypeInst -> TType option
      refcell_tcr_canon: TyconRef
      option_tcr_canon : TyconRef
      choice2_tcr : TyconRef
      choice3_tcr : TyconRef
      choice4_tcr : TyconRef
      choice5_tcr : TyconRef
      choice6_tcr : TyconRef
      choice7_tcr : TyconRef
      list_tcr_canon   : TyconRef
      set_tcr_canon   : TyconRef
      map_tcr_canon   : TyconRef
      lazy_tcr_canon   : TyconRef 
      
      // These have a slightly different behaviour when compiling GetFSharpCoreLibraryName 
      // hence they are 'methods' on the TcGlobals structure. 

      unionCaseRefEq : UnionCaseRef -> UnionCaseRef -> bool
      valRefEq  : ValRef         -> ValRef         -> bool

      refcell_tcr_nice: TyconRef
      option_tcr_nice : TyconRef
      list_tcr_nice   : TyconRef 
      lazy_tcr_nice   : TyconRef 

      format_tcr      : TyconRef
      expr_tcr        : TyconRef
      raw_expr_tcr    : TyconRef
      nativeint_tcr   : TyconRef 
      int32_tcr       : TyconRef
      int16_tcr       : TyconRef
      int64_tcr       : TyconRef
      uint16_tcr      : TyconRef
      uint32_tcr      : TyconRef
      uint64_tcr      : TyconRef
      sbyte_tcr       : TyconRef
      decimal_tcr     : TyconRef
      date_tcr        : TyconRef
      pdecimal_tcr    : TyconRef
      byte_tcr        : TyconRef
      bool_tcr        : TyconRef
      unit_tcr_canon  : TyconRef
      unit_tcr_nice   : TyconRef
      exn_tcr         : TyconRef
      char_tcr        : TyconRef
      float_tcr       : TyconRef
      float32_tcr     : TyconRef
      pfloat_tcr      : TyconRef
      pfloat32_tcr    : TyconRef
      pint_tcr        : TyconRef
      pint8_tcr       : TyconRef
      pint16_tcr      : TyconRef
      pint64_tcr      : TyconRef
      byref_tcr       : TyconRef
      nativeptr_tcr   : TyconRef
      ilsigptr_tcr    : TyconRef
      fastFunc_tcr    : TyconRef
      array_tcr_nice  : TyconRef
      seq_tcr         : TyconRef
      seq_base_tcr    : TyconRef
      measureproduct_tcr : TyconRef
      measureinverse_tcr : TyconRef
      measureone_tcr : TyconRef
      il_arr_tcr_map : TyconRef[]
      ref_tuple1_tcr      : TyconRef
      ref_tuple2_tcr      : TyconRef
      ref_tuple3_tcr      : TyconRef
      ref_tuple4_tcr      : TyconRef
      ref_tuple5_tcr      : TyconRef
      ref_tuple6_tcr      : TyconRef
      ref_tuple7_tcr      : TyconRef
      ref_tuple8_tcr      : TyconRef
      struct_tuple1_tcr      : TyconRef
      struct_tuple2_tcr      : TyconRef
      struct_tuple3_tcr      : TyconRef
      struct_tuple4_tcr      : TyconRef
      struct_tuple5_tcr      : TyconRef
      struct_tuple6_tcr      : TyconRef
      struct_tuple7_tcr      : TyconRef
      struct_tuple8_tcr      : TyconRef

      tcref_IQueryable        : TyconRef
      tcref_IObservable       : TyconRef
      tcref_IObserver         : TyconRef
      fslib_IEvent2_tcr       : TyconRef
      fslib_IDelegateEvent_tcr: TyconRef
      system_Nullable_tcref                 : TyconRef 
      system_GenericIComparable_tcref       : TyconRef 
      system_GenericIEquatable_tcref        : TyconRef 
      system_IndexOutOfRangeException_tcref : TyconRef
      int_ty         : TType
      nativeint_ty   : TType 
      unativeint_ty  : TType 
      int32_ty       : TType 
      int16_ty       : TType 
      int64_ty       : TType 
      uint16_ty      : TType 
      uint32_ty      : TType 
      uint64_ty      : TType 
      sbyte_ty       : TType 
      byte_ty        : TType 
      bool_ty        : TType 
      string_ty      : TType 
      obj_ty         : TType 
      unit_ty        : TType 
      exn_ty         : TType 
      char_ty        : TType 
      decimal_ty                   : TType 
      float_ty                     : TType 
      float32_ty                   : TType 
      system_Array_typ             : TType 
      system_Object_typ            : TType 
      system_IDisposable_typ       : TType 
      system_RuntimeHelpers_typ       : TType 
      system_Value_typ             : TType 
      system_Delegate_typ          : TType
      system_MulticastDelegate_typ : TType
      system_Enum_typ              : TType 
      system_Exception_typ         : TType 
      system_Int32_typ             : TType 
      system_String_typ            : TType 
      system_String_tcref          : TyconRef
      system_Type_typ              : TType 
      system_TypedReference_tcref  : TyconRef option
      system_ArgIterator_tcref     : TyconRef option 
      system_Decimal_tcref : TyconRef 
      system_SByte_tcref : TyconRef 
      system_Int16_tcref : TyconRef 
      system_Int32_tcref : TyconRef 
      system_Int64_tcref : TyconRef 
      system_IntPtr_tcref : TyconRef 
      system_Bool_tcref : TyconRef 
      system_Char_tcref : TyconRef 
      system_Byte_tcref : TyconRef 
      system_UInt16_tcref : TyconRef 
      system_UInt32_tcref : TyconRef 
      system_UInt64_tcref : TyconRef 
      system_UIntPtr_tcref : TyconRef 
      system_Single_tcref : TyconRef 
      system_Double_tcref : TyconRef 
      system_RuntimeArgumentHandle_tcref : TyconRef option 
      system_RuntimeTypeHandle_typ       : TType
      system_RuntimeMethodHandle_typ     : TType
      system_MarshalByRefObject_tcref    : TyconRef option
      system_MarshalByRefObject_typ      : TType option
      system_Reflection_MethodInfo_typ   : TType
      system_Array_tcref           : TyconRef
      system_Object_tcref          : TyconRef
      system_Void_tcref            : TyconRef
      system_LinqExpression_tcref  : TyconRef
      mk_IComparable_ty            : TType
      mk_IStructuralComparable_ty  : TType
      mk_IStructuralEquatable_ty   : TType
      mk_IComparer_ty              : TType
      mk_IEqualityComparer_ty      : TType
      tcref_System_Collections_IComparer                 : TyconRef
      tcref_System_Collections_IEqualityComparer         : TyconRef
      tcref_System_Collections_Generic_IEqualityComparer : TyconRef
      tcref_System_Collections_Generic_Dictionary        : TyconRef
      tcref_System_IComparable                           : TyconRef
      tcref_System_IStructuralComparable                 : TyconRef
      tcref_System_IStructuralEquatable                  : TyconRef
      tcref_LanguagePrimitives                           : TyconRef
      attrib_CustomOperationAttribute    : BuiltinAttribInfo
      attrib_ProjectionParameterAttribute : BuiltinAttribInfo
      attrib_AttributeUsageAttribute     : BuiltinAttribInfo
      attrib_ParamArrayAttribute         : BuiltinAttribInfo
      attrib_IDispatchConstantAttribute  : BuiltinAttribInfo option
      attrib_IUnknownConstantAttribute   : BuiltinAttribInfo option
      attrib_SystemObsolete              : BuiltinAttribInfo
      attrib_DllImportAttribute          : BuiltinAttribInfo option
      attrib_CompiledNameAttribute       : BuiltinAttribInfo
      attrib_NonSerializedAttribute      : BuiltinAttribInfo option
      attrib_AutoSerializableAttribute   : BuiltinAttribInfo
      attrib_StructLayoutAttribute       : BuiltinAttribInfo
      attrib_TypeForwardedToAttribute    : BuiltinAttribInfo
      attrib_ComVisibleAttribute         : BuiltinAttribInfo
      attrib_ComImportAttribute          : BuiltinAttribInfo option
      attrib_FieldOffsetAttribute        : BuiltinAttribInfo
      attrib_MarshalAsAttribute          : BuiltinAttribInfo option
      attrib_InAttribute                 : BuiltinAttribInfo option
      attrib_OutAttribute                : BuiltinAttribInfo
      attrib_OptionalAttribute           : BuiltinAttribInfo option
      attrib_ThreadStaticAttribute       : BuiltinAttribInfo option
      attrib_SpecialNameAttribute        : BuiltinAttribInfo option
      attrib_VolatileFieldAttribute      : BuiltinAttribInfo
      attrib_ContextStaticAttribute      : BuiltinAttribInfo option
      attrib_FlagsAttribute              : BuiltinAttribInfo
      attrib_DefaultMemberAttribute      : BuiltinAttribInfo
      attrib_DebuggerDisplayAttribute    : BuiltinAttribInfo
      attrib_DebuggerTypeProxyAttribute  : BuiltinAttribInfo
      attrib_PreserveSigAttribute        : BuiltinAttribInfo option
      attrib_MethodImplAttribute         : BuiltinAttribInfo
      attrib_ExtensionAttribute          : BuiltinAttribInfo
      attrib_CallerLineNumberAttribute   : BuiltinAttribInfo
      attrib_CallerFilePathAttribute     : BuiltinAttribInfo
      attrib_CallerMemberNameAttribute   : BuiltinAttribInfo

      tcref_System_Collections_Generic_IList               : TyconRef
      tcref_System_Collections_Generic_IReadOnlyList       : TyconRef
      tcref_System_Collections_Generic_ICollection         : TyconRef
      tcref_System_Collections_Generic_IReadOnlyCollection : TyconRef
      tcref_System_Collections_Generic_IEnumerable         : TyconRef
      tcref_System_Collections_IEnumerable                 : TyconRef
      tcref_System_Collections_Generic_IEnumerator         : TyconRef
      tcref_System_Attribute                               : TyconRef

      attrib_RequireQualifiedAccessAttribute        : BuiltinAttribInfo 
      attrib_EntryPointAttribute                    : BuiltinAttribInfo 
      attrib_DefaultAugmentationAttribute           : BuiltinAttribInfo 
      attrib_CompilerMessageAttribute               : BuiltinAttribInfo 
      attrib_ExperimentalAttribute                  : BuiltinAttribInfo 
      attrib_UnverifiableAttribute                  : BuiltinAttribInfo 
      attrib_LiteralAttribute                       : BuiltinAttribInfo 
      attrib_ConditionalAttribute                   : BuiltinAttribInfo 
      attrib_OptionalArgumentAttribute              : BuiltinAttribInfo 
      attrib_RequiresExplicitTypeArgumentsAttribute : BuiltinAttribInfo 
      attrib_DefaultValueAttribute                  : BuiltinAttribInfo 
      attrib_ClassAttribute                         : BuiltinAttribInfo 
      attrib_InterfaceAttribute                     : BuiltinAttribInfo 
      attrib_StructAttribute                        : BuiltinAttribInfo 
      attrib_ReflectedDefinitionAttribute           : BuiltinAttribInfo 
      attrib_AutoOpenAttribute                      : BuiltinAttribInfo 
      attrib_InternalsVisibleToAttribute            : BuiltinAttribInfo 
      attrib_CompilationRepresentationAttribute     : BuiltinAttribInfo 
      attrib_CompilationArgumentCountsAttribute     : BuiltinAttribInfo 
      attrib_CompilationMappingAttribute            : BuiltinAttribInfo 

      attrib_CLIEventAttribute                      : BuiltinAttribInfo 
      attrib_AllowNullLiteralAttribute              : BuiltinAttribInfo 
      attrib_CLIMutableAttribute                    : BuiltinAttribInfo 
      attrib_NoComparisonAttribute                  : BuiltinAttribInfo 
      attrib_NoEqualityAttribute                    : BuiltinAttribInfo 
      attrib_CustomComparisonAttribute              : BuiltinAttribInfo 
      attrib_CustomEqualityAttribute                : BuiltinAttribInfo 
      attrib_EqualityConditionalOnAttribute         : BuiltinAttribInfo 
      attrib_ComparisonConditionalOnAttribute       : BuiltinAttribInfo 
      attrib_ReferenceEqualityAttribute             : BuiltinAttribInfo 
      attrib_StructuralEqualityAttribute            : BuiltinAttribInfo 
      attrib_StructuralComparisonAttribute          : BuiltinAttribInfo 
      attrib_SealedAttribute                        : BuiltinAttribInfo 
      attrib_AbstractClassAttribute                 : BuiltinAttribInfo 
      attrib_GeneralizableValueAttribute            : BuiltinAttribInfo
      attrib_MeasureAttribute                       : BuiltinAttribInfo
      attrib_MeasureableAttribute                   : BuiltinAttribInfo
      attrib_NoDynamicInvocationAttribute           : BuiltinAttribInfo
      
      attrib_SecurityAttribute                      : BuiltinAttribInfo option
      attrib_SecurityCriticalAttribute              : BuiltinAttribInfo
      attrib_SecuritySafeCriticalAttribute          : BuiltinAttribInfo

      
      cons_ucref : UnionCaseRef
      nil_ucref : UnionCaseRef
      (* These are the library values the compiler needs to know about *)
      seq_vref                  : ValRef
      and_vref                  : ValRef
      and2_vref                 : ValRef
      addrof_vref               : ValRef
      addrof2_vref              : ValRef
      or_vref                   : ValRef
      or2_vref                  : ValRef
      
      // 'inner' refers to "after optimization boils away inlined functions"
      generic_equality_er_inner_vref         : ValRef
      generic_equality_per_inner_vref        : ValRef
      generic_equality_withc_inner_vref      : ValRef
      generic_comparison_inner_vref          : ValRef
      generic_comparison_withc_inner_vref    : ValRef
      generic_hash_inner_vref                : ValRef
      generic_hash_withc_inner_vref          : ValRef
      reference_equality_inner_vref          : ValRef

      compare_operator_vref                  : ValRef
      equals_operator_vref                   : ValRef
      equals_nullable_operator_vref          : ValRef
      nullable_equals_nullable_operator_vref : ValRef
      nullable_equals_operator_vref          : ValRef
      not_equals_operator_vref               : ValRef
      less_than_operator_vref                : ValRef
      less_than_or_equals_operator_vref      : ValRef
      greater_than_operator_vref             : ValRef
      greater_than_or_equals_operator_vref   : ValRef
 
      bitwise_or_vref            : ValRef
      bitwise_and_vref           : ValRef
      bitwise_xor_vref           : ValRef
      bitwise_unary_not_vref     : ValRef
      bitwise_shift_left_vref    : ValRef
      bitwise_shift_right_vref   : ValRef
      unchecked_addition_vref    : ValRef
      unchecked_unary_plus_vref  : ValRef
      unchecked_unary_minus_vref : ValRef
      unchecked_unary_not_vref   : ValRef
      unchecked_subtraction_vref : ValRef
      unchecked_multiply_vref    : ValRef
      unchecked_defaultof_vref   : ValRef
      unchecked_subtraction_info : IntrinsicValRef
      seq_info                  : IntrinsicValRef
      reraise_info              : IntrinsicValRef
      reraise_vref              : ValRef      
      typeof_info               : IntrinsicValRef
      typeof_vref               : ValRef
      methodhandleof_info       : IntrinsicValRef
      methodhandleof_vref       : ValRef
      sizeof_vref               : ValRef
      typedefof_info            : IntrinsicValRef
      typedefof_vref            : ValRef
      enum_vref                 : ValRef
      enumOfValue_vref          : ValRef
      new_decimal_info          : IntrinsicValRef
      
      // 'outer' refers to 'before optimization has boiled away inlined functions'
      // Augmentation generation generates calls to these functions
      // Optimization generates calls to these functions
      generic_comparison_withc_outer_info : IntrinsicValRef
      generic_equality_er_outer_info      : IntrinsicValRef
      generic_equality_withc_outer_info   : IntrinsicValRef
      generic_hash_withc_outer_info       : IntrinsicValRef

      // Augmentation generation and pattern match compilation generates calls to this function
      equals_operator_info    : IntrinsicValRef
      
      query_source_vref     : ValRef
      query_value_vref      : ValRef
      query_run_value_vref  : ValRef
      query_run_enumerable_vref : ValRef
      query_for_vref        : ValRef
      query_yield_vref      : ValRef
      query_yield_from_vref : ValRef
      query_select_vref     : ValRef
      query_where_vref      : ValRef
      query_zero_vref       : ValRef
      query_builder_tcref   : TyconRef
      generic_hash_withc_tuple2_vref : ValRef
      generic_hash_withc_tuple3_vref : ValRef
      generic_hash_withc_tuple4_vref : ValRef
      generic_hash_withc_tuple5_vref : ValRef
      generic_equals_withc_tuple2_vref : ValRef
      generic_equals_withc_tuple3_vref : ValRef
      generic_equals_withc_tuple4_vref : ValRef
      generic_equals_withc_tuple5_vref : ValRef
      generic_compare_withc_tuple2_vref : ValRef
      generic_compare_withc_tuple3_vref : ValRef
      generic_compare_withc_tuple4_vref : ValRef
      generic_compare_withc_tuple5_vref : ValRef
      generic_equality_withc_outer_vref : ValRef

      create_instance_info      : IntrinsicValRef
      create_event_info         : IntrinsicValRef
      unbox_vref                : ValRef
      unbox_fast_vref           : ValRef
      istype_vref               : ValRef
      istype_fast_vref          : ValRef
      get_generic_comparer_info                : IntrinsicValRef
      get_generic_er_equality_comparer_info                : IntrinsicValRef
      get_generic_per_equality_comparer_info            : IntrinsicValRef
      unbox_info                : IntrinsicValRef
      unbox_fast_info           : IntrinsicValRef
      istype_info               : IntrinsicValRef
      istype_fast_info          : IntrinsicValRef

      dispose_info              : IntrinsicValRef
      
      getstring_info            : IntrinsicValRef

      range_op_vref             : ValRef
      range_step_op_vref        : ValRef
      range_int32_op_vref       : ValRef
      array_get_vref            : ValRef
      array2D_get_vref          : ValRef
      array3D_get_vref          : ValRef
      array4D_get_vref          : ValRef
      seq_collect_vref          : ValRef
      seq_collect_info          : IntrinsicValRef
      seq_using_info            : IntrinsicValRef
      seq_using_vref            : ValRef
      seq_delay_info            : IntrinsicValRef
      seq_delay_vref            : ValRef
      seq_append_info           : IntrinsicValRef
      seq_append_vref           : ValRef
      seq_generated_info        : IntrinsicValRef
      seq_generated_vref        : ValRef
      seq_finally_info          : IntrinsicValRef
      seq_finally_vref          : ValRef
      seq_of_functions_info     : IntrinsicValRef
      seq_of_functions_vref     : ValRef
      seq_to_array_info         : IntrinsicValRef
      seq_to_list_info          : IntrinsicValRef
      seq_map_info              : IntrinsicValRef
      seq_map_vref              : ValRef
      seq_singleton_info        : IntrinsicValRef
      seq_singleton_vref        : ValRef
      seq_empty_info            : IntrinsicValRef
      seq_empty_vref            : ValRef
      new_format_info           : IntrinsicValRef
      raise_info                : IntrinsicValRef
      raise_vref                : ValRef
      failwith_info             : IntrinsicValRef
      failwith_vref             : ValRef
      invalid_arg_info          : IntrinsicValRef
      invalid_arg_vref          : ValRef
      null_arg_info             : IntrinsicValRef
      null_arg_vref             : ValRef
      invalid_op_info           : IntrinsicValRef
      invalid_op_vref           : ValRef
      failwithf_info            : IntrinsicValRef
      failwithf_vref            : ValRef

      lazy_force_info           : IntrinsicValRef
      lazy_create_info          : IntrinsicValRef

      array_get_info             : IntrinsicValRef
      array_length_info          : IntrinsicValRef
      array2D_get_info           : IntrinsicValRef
      array3D_get_info           : IntrinsicValRef
      array4D_get_info           : IntrinsicValRef
      deserialize_quoted_FSharp_20_plus_info       : IntrinsicValRef
      deserialize_quoted_FSharp_40_plus_info    : IntrinsicValRef
      cast_quotation_info        : IntrinsicValRef
      lift_value_info            : IntrinsicValRef
      lift_value_with_name_info  : IntrinsicValRef
      lift_value_with_defn_info  : IntrinsicValRef
      query_source_as_enum_info  : IntrinsicValRef
      new_query_source_info      : IntrinsicValRef
      fail_init_info             : IntrinsicValRef
      fail_static_init_info      : IntrinsicValRef
      check_this_info            : IntrinsicValRef
      quote_to_linq_lambda_info  : IntrinsicValRef
      sprintf_vref               : ValRef
      splice_expr_vref           : ValRef
      splice_raw_expr_vref       : ValRef
      new_format_vref            : ValRef
      mkSysTyconRef : string list -> string -> TyconRef

      // A list of types that are explicitly suppressed from the F# intellisense 
      // Note that the suppression checks for the precise name of the type
      // so the lowercase versions are visible
      suppressed_types           : TyconRef list
      
      /// Memoization table to help minimize the number of ILSourceDocument objects we create
      memoize_file : int -> IL.ILSourceDocument
      // Are we assuming all code gen is for F# interactive, with no static linking 
      isInteractive : bool
      // A table of all intrinsics that the compiler cares about
      knownIntrinsics : IDictionary<(string * string), ValRef>
      // A table of known modules in FSharp.Core. Not all modules are necessarily listed, but the more we list the
      // better the job we do of mapping from provided expressions back to FSharp.Core F# functions and values.
      knownFSharpCoreModules : IDictionary<string, ModuleOrNamespaceRef>
      
    } 
    override x.ToString() = "<TcGlobals>"

#if DEBUG
// This global is only used during debug output 
let global_g = ref (None : TcGlobals option)
#endif

let mkTcGlobals (compilingFslib,sysCcu,ilg,fslibCcu,directoryToResolveRelativePaths,mlCompatibility,
                 using40environment,isInteractive,getTypeCcu, emitDebugInfoInQuotations) = 

  let vara = NewRigidTypar "a" envRange
  let varb = NewRigidTypar "b" envRange
  let varc = NewRigidTypar "c" envRange
  let vard = NewRigidTypar "d" envRange
  let vare = NewRigidTypar "e" envRange

  let varaTy = mkTyparTy vara 
  let varbTy = mkTyparTy varb 
  let varcTy = mkTyparTy varc
  let vardTy = mkTyparTy vard
  let vareTy = mkTyparTy vare

  let int_tcr        = mk_MFCore_tcref fslibCcu "int"
  let nativeint_tcr  = mk_MFCore_tcref fslibCcu "nativeint"
  let unativeint_tcr = mk_MFCore_tcref fslibCcu "unativeint"
  let int32_tcr      = mk_MFCore_tcref fslibCcu "int32"
  let int16_tcr      = mk_MFCore_tcref fslibCcu "int16"
  let int64_tcr      = mk_MFCore_tcref fslibCcu "int64"
  let uint16_tcr     = mk_MFCore_tcref fslibCcu "uint16"
  let uint32_tcr     = mk_MFCore_tcref fslibCcu "uint32"
  let uint64_tcr     = mk_MFCore_tcref fslibCcu "uint64"
  let sbyte_tcr      = mk_MFCore_tcref fslibCcu "sbyte"
  let decimal_tcr    = mk_MFCore_tcref fslibCcu "decimal"
  let pdecimal_tcr   = mk_MFCore_tcref fslibCcu "decimal`1"
  let byte_tcr       = mk_MFCore_tcref fslibCcu "byte"
  let bool_tcr       = mk_MFCore_tcref fslibCcu "bool"
  let string_tcr     = mk_MFCore_tcref fslibCcu "string"
  let obj_tcr        = mk_MFCore_tcref fslibCcu "obj"
  let unit_tcr_canon = mk_MFCore_tcref fslibCcu "Unit"
  let unit_tcr_nice  = mk_MFCore_tcref fslibCcu "unit"
  let exn_tcr        = mk_MFCore_tcref fslibCcu "exn"
  let char_tcr       = mk_MFCore_tcref fslibCcu "char"
  let float_tcr      = mk_MFCore_tcref fslibCcu "float"  
  let float32_tcr    = mk_MFCore_tcref fslibCcu "float32"
  let pfloat_tcr     = mk_MFCore_tcref fslibCcu "float`1"  
  let pfloat32_tcr   = mk_MFCore_tcref fslibCcu "float32`1"  
  let pint_tcr       = mk_MFCore_tcref fslibCcu "int`1"  
  let pint8_tcr      = mk_MFCore_tcref fslibCcu "sbyte`1"  
  let pint16_tcr     = mk_MFCore_tcref fslibCcu "int16`1"  
  let pint64_tcr     = mk_MFCore_tcref fslibCcu "int64`1"  
  let byref_tcr      = mk_MFCore_tcref fslibCcu "byref`1"
  let nativeptr_tcr  = mk_MFCore_tcref fslibCcu "nativeptr`1"
  let ilsigptr_tcr   = mk_MFCore_tcref fslibCcu "ilsigptr`1"
  let fastFunc_tcr   = mk_MFCore_tcref fslibCcu "FSharpFunc`2"

  let mkSysTyconRef path nm = 
        let ccu = getTypeCcu path nm
        mkNonLocalTyconRef2 ccu (Array.ofList path) nm

  let mkSysNonGenericTy path n = mkNonGenericTy(mkSysTyconRef path n)

  let sys = ["System"]
  let sysLinq = ["System";"Linq"]
  let sysCollections = ["System";"Collections"]
  let sysGenerics = ["System";"Collections";"Generic"]
  let sysCompilerServices = ["System";"Runtime";"CompilerServices"]

  let lazy_tcr = mkSysTyconRef sys "Lazy`1"
  let fslib_IEvent2_tcr        = mk_MFControl_tcref fslibCcu "IEvent`2"
  let tcref_IQueryable      =  mkSysTyconRef sysLinq "IQueryable`1"
  let tcref_IObservable      =  mkSysTyconRef sys "IObservable`1"
  let tcref_IObserver        =  mkSysTyconRef sys "IObserver`1"
  let fslib_IDelegateEvent_tcr = mk_MFControl_tcref fslibCcu "IDelegateEvent`1"

  let option_tcr_nice     = mk_MFCore_tcref fslibCcu "option`1"
  let list_tcr_canon        = mk_MFCollections_tcref fslibCcu "List`1"
  let list_tcr_nice            = mk_MFCollections_tcref fslibCcu "list`1"
  let lazy_tcr_nice            = mk_MFControl_tcref fslibCcu "Lazy`1"
  let seq_tcr                  = mk_MFCollections_tcref fslibCcu "seq`1"
  let format_tcr               = mk_MFCore_tcref     fslibCcu "PrintfFormat`5" 
  let format4_tcr              = mk_MFCore_tcref     fslibCcu "PrintfFormat`4" 
  let date_tcr                 = mkSysTyconRef sys "DateTime"
  let IEnumerable_tcr          = mkSysTyconRef sysGenerics "IEnumerable`1"
  let IEnumerator_tcr          = mkSysTyconRef sysGenerics "IEnumerator`1"
  let System_Attribute_tcr     = mkSysTyconRef sys "Attribute"
  let expr_tcr                 = mk_MFQuotations_tcref fslibCcu "Expr`1" 
  let raw_expr_tcr             = mk_MFQuotations_tcref fslibCcu "Expr" 
  let query_builder_tcref         = mk_MFLinq_tcref fslibCcu "QueryBuilder" 
  let querySource_tcr         = mk_MFLinq_tcref fslibCcu "QuerySource`2" 
  let linqExpression_tcr     = mkSysTyconRef ["System";"Linq";"Expressions"] "Expression`1"

  let il_arr_tcr_map =
      Array.init 32 (fun idx ->
          let type_sig =
              let rank = idx + 1
              if rank = 1 then "[]`1"
              else "[" + (String.replicate (rank - 1) ",") + "]`1"
          mk_MFCore_tcref fslibCcu type_sig)
  
  let bool_ty         = mkNonGenericTy bool_tcr   
  let int_ty          = mkNonGenericTy int_tcr    
  let char_ty         = mkNonGenericTy char_tcr
  let obj_ty          = mkNonGenericTy obj_tcr    
  let string_ty       = mkNonGenericTy string_tcr
  let byte_ty         = mkNonGenericTy byte_tcr
  let decimal_ty      = mkSysNonGenericTy sys "Decimal"
  let unit_ty         = mkNonGenericTy unit_tcr_nice 
  let system_Type_typ = mkSysNonGenericTy sys "Type" 

  
  let system_Reflection_MethodInfo_typ = mkSysNonGenericTy ["System";"Reflection"] "MethodInfo"
  let nullable_tcr = mkSysTyconRef sys "Nullable`1"

  (* local helpers to build value infos *)
  let mkNullableTy ty = TType_app(nullable_tcr, [ty]) 
  let mkByrefTy ty = TType_app(byref_tcr, [ty]) 
  let mkNativePtrTy ty = TType_app(nativeptr_tcr, [ty]) 
  let mkFunTy d r = TType_fun (d,r) 
  let (-->) d r = mkFunTy d r
  let mkIteratedFunTy dl r = List.foldBack (-->) dl r
  let mkSmallRefTupledTy l = match l with [] -> unit_ty | [h] -> h | tys -> mkRawRefTupleTy tys
  let tryMkForallTy d r = match d with [] -> r | tps -> TType_forall(tps,r)

  let knownIntrinsics = Dictionary<(string*string), ValRef>(HashIdentity.Structural)

  let makeIntrinsicValRef (enclosingEntity, logicalName, memberParentName, compiledNameOpt, typars, (argtys,rty))  =
      let ty = tryMkForallTy typars (mkIteratedFunTy (List.map mkSmallRefTupledTy argtys) rty)
      let isMember = isSome memberParentName
      let argCount = if isMember then List.sum (List.map List.length argtys) else 0
      let linkageType = if isMember then Some ty else None
      let key = ValLinkageFullKey({ MemberParentMangledName=memberParentName; MemberIsOverride=false; LogicalName=logicalName; TotalArgCount= argCount },linkageType)
      let vref = IntrinsicValRef(enclosingEntity,logicalName,isMember,ty,key)
      let compiledName = defaultArg compiledNameOpt logicalName
      knownIntrinsics.Add((enclosingEntity.LastItemMangledName, compiledName), ValRefForIntrinsic vref)
      vref


  let mk_IComparer_ty = mkSysNonGenericTy sysCollections "IComparer"
  let mk_IEqualityComparer_ty = mkSysNonGenericTy sysCollections "IEqualityComparer"

  let system_RuntimeMethodHandle_typ = mkSysNonGenericTy sys "RuntimeMethodHandle"

  let mk_unop_ty ty             = [[ty]], ty
  let mk_binop_ty ty            = [[ty]; [ty]], ty
  let mk_shiftop_ty ty          = [[ty]; [int_ty]], ty
  let mk_binop_ty3 ty1 ty2 ty3  = [[ty1]; [ty2]], ty3
  let mk_rel_sig ty             = [[ty];[ty]],bool_ty
  let mk_compare_sig ty         = [[ty];[ty]],int_ty
  let mk_hash_sig ty            = [[ty]], int_ty
  let mk_compare_withc_sig  ty = [[mk_IComparer_ty];[ty]; [ty]], int_ty
  let mk_equality_withc_sig ty = [[mk_IEqualityComparer_ty];[ty];[ty]], bool_ty
  let mk_hash_withc_sig     ty = [[mk_IEqualityComparer_ty]; [ty]], int_ty
  let mkListTy ty         = TType_app(list_tcr_nice,[ty])
  let mkSeqTy ty1         = TType_app(seq_tcr,[ty1])
  let mkQuerySourceTy ty1 ty2         = TType_app(querySource_tcr,[ty1; ty2])
  let tcref_System_Collections_IEnumerable         = mkSysTyconRef sysCollections "IEnumerable";
  let mkArrayType rank (ty : TType) : TType =
      assert (rank >= 1 && rank <= 32)
      TType_app(il_arr_tcr_map.[rank - 1], [ty])
  let mkLazyTy ty         = TType_app(lazy_tcr, [ty])
  
  let mkPrintfFormatTy aty bty cty dty ety = TType_app(format_tcr, [aty;bty;cty;dty; ety]) 
  let mk_format4_ty aty bty cty dty = TType_app(format4_tcr, [aty;bty;cty;dty]) 
  let mkQuotedExprTy aty = TType_app(expr_tcr, [aty]) 
  let mkRawQuotedExprTy = TType_app(raw_expr_tcr, []) 
  let mkQueryBuilderTy = TType_app(query_builder_tcref, []) 
  let mkLinqExpressionTy aty = TType_app(linqExpression_tcr, [aty]) 
  let cons_ucref = mkUnionCaseRef list_tcr_canon "op_ColonColon" 
  let nil_ucref  = mkUnionCaseRef list_tcr_canon "op_Nil" 

  
  let fslib_MF_nleref                   = mkNonLocalEntityRef fslibCcu FSharpLib.RootPathArray
  let fslib_MFCore_nleref               = mkNonLocalEntityRef fslibCcu FSharpLib.CorePathArray 
  let fslib_MFLinq_nleref               = mkNonLocalEntityRef fslibCcu FSharpLib.LinqPathArray 
  let fslib_MFCollections_nleref        = mkNonLocalEntityRef fslibCcu FSharpLib.CollectionsPathArray 
  let fslib_MFCompilerServices_nleref   = mkNonLocalEntityRef fslibCcu FSharpLib.CompilerServicesPath
  let fslib_MFLinqRuntimeHelpers_nleref = mkNonLocalEntityRef fslibCcu FSharpLib.LinqRuntimeHelpersPath
  let fslib_MFControl_nleref            = mkNonLocalEntityRef fslibCcu FSharpLib.ControlPathArray

  let fslib_MFLanguagePrimitives_nleref        = mkNestedNonLocalEntityRef fslib_MFCore_nleref "LanguagePrimitives"
  let fslib_MFIntrinsicOperators_nleref        = mkNestedNonLocalEntityRef fslib_MFLanguagePrimitives_nleref "IntrinsicOperators" 
  let fslib_MFIntrinsicFunctions_nleref        = mkNestedNonLocalEntityRef fslib_MFLanguagePrimitives_nleref "IntrinsicFunctions" 
  let fslib_MFHashCompare_nleref               = mkNestedNonLocalEntityRef fslib_MFLanguagePrimitives_nleref "HashCompare"
  let fslib_MFOperators_nleref                 = mkNestedNonLocalEntityRef fslib_MFCore_nleref "Operators"
  let fslib_MFOperatorIntrinsics_nleref        = mkNestedNonLocalEntityRef fslib_MFOperators_nleref "OperatorIntrinsics"
  let fslib_MFOperatorsUnchecked_nleref        = mkNestedNonLocalEntityRef fslib_MFOperators_nleref "Unchecked"
  let fslib_MFOperatorsChecked_nleref        = mkNestedNonLocalEntityRef fslib_MFOperators_nleref "Checked"
  let fslib_MFExtraTopLevelOperators_nleref    = mkNestedNonLocalEntityRef fslib_MFCore_nleref "ExtraTopLevelOperators"
  let fslib_MFNullableOperators_nleref         = mkNestedNonLocalEntityRef fslib_MFLinq_nleref "NullableOperators"
  let fslib_MFQueryRunExtensions_nleref              = mkNestedNonLocalEntityRef fslib_MFLinq_nleref "QueryRunExtensions"
  let fslib_MFQueryRunExtensionsLowPriority_nleref   = mkNestedNonLocalEntityRef fslib_MFQueryRunExtensions_nleref "LowPriority"
  let fslib_MFQueryRunExtensionsHighPriority_nleref  = mkNestedNonLocalEntityRef fslib_MFQueryRunExtensions_nleref "HighPriority"
  
  let fslib_MFSeqModule_nleref                 = mkNestedNonLocalEntityRef fslib_MFCollections_nleref "SeqModule"
  let fslib_MFListModule_nleref                = mkNestedNonLocalEntityRef fslib_MFCollections_nleref "ListModule"
  let fslib_MFArrayModule_nleref               = mkNestedNonLocalEntityRef fslib_MFCollections_nleref "ArrayModule"
  let fslib_MFArray2DModule_nleref               = mkNestedNonLocalEntityRef fslib_MFCollections_nleref "Array2DModule"
  let fslib_MFArray3DModule_nleref               = mkNestedNonLocalEntityRef fslib_MFCollections_nleref "Array3DModule"
  let fslib_MFArray4DModule_nleref               = mkNestedNonLocalEntityRef fslib_MFCollections_nleref "Array4DModule"
  let fslib_MFSetModule_nleref               = mkNestedNonLocalEntityRef fslib_MFCollections_nleref "SetModule"
  let fslib_MFMapModule_nleref               = mkNestedNonLocalEntityRef fslib_MFCollections_nleref "MapModule"
  let fslib_MFStringModule_nleref               = mkNestedNonLocalEntityRef fslib_MFCollections_nleref "StringModule"
  let fslib_MFOptionModule_nleref              = mkNestedNonLocalEntityRef fslib_MFCore_nleref "OptionModule"
  let fslib_MFRuntimeHelpers_nleref            = mkNestedNonLocalEntityRef fslib_MFCompilerServices_nleref "RuntimeHelpers"
  let fslib_MFQuotations_nleref                = mkNestedNonLocalEntityRef fslib_MF_nleref "Quotations"
  
  let fslib_MFLinqRuntimeHelpersQuotationConverter_nleref        = mkNestedNonLocalEntityRef fslib_MFLinqRuntimeHelpers_nleref "LeafExpressionConverter"
  let fslib_MFLazyExtensions_nleref            = mkNestedNonLocalEntityRef fslib_MFControl_nleref "LazyExtensions" 

  let ref_tuple1_tcr      = mkSysTyconRef sys "Tuple`1" 
  let ref_tuple2_tcr      = mkSysTyconRef sys "Tuple`2" 
  let ref_tuple3_tcr      = mkSysTyconRef sys "Tuple`3" 
  let ref_tuple4_tcr      = mkSysTyconRef sys "Tuple`4" 
  let ref_tuple5_tcr      = mkSysTyconRef sys "Tuple`5" 
  let ref_tuple6_tcr      = mkSysTyconRef sys "Tuple`6" 
  let ref_tuple7_tcr      = mkSysTyconRef sys "Tuple`7" 
  let ref_tuple8_tcr      = mkSysTyconRef sys "Tuple`8" 
  let struct_tuple1_tcr      = mkSysTyconRef sys "ValueTuple`1" 
  let struct_tuple2_tcr      = mkSysTyconRef sys "ValueTuple`2" 
  let struct_tuple3_tcr      = mkSysTyconRef sys "ValueTuple`3" 
  let struct_tuple4_tcr      = mkSysTyconRef sys "ValueTuple`4" 
  let struct_tuple5_tcr      = mkSysTyconRef sys "ValueTuple`5" 
  let struct_tuple6_tcr      = mkSysTyconRef sys "ValueTuple`6" 
  let struct_tuple7_tcr      = mkSysTyconRef sys "ValueTuple`7" 
  let struct_tuple8_tcr      = mkSysTyconRef sys "ValueTuple`8" 
  
  let choice2_tcr     = mk_MFCore_tcref fslibCcu "Choice`2" 
  let choice3_tcr     = mk_MFCore_tcref fslibCcu "Choice`3" 
  let choice4_tcr     = mk_MFCore_tcref fslibCcu "Choice`4" 
  let choice5_tcr     = mk_MFCore_tcref fslibCcu "Choice`5" 
  let choice6_tcr     = mk_MFCore_tcref fslibCcu "Choice`6" 
  let choice7_tcr     = mk_MFCore_tcref fslibCcu "Choice`7" 
  let tyconRefEq x y = primEntityRefEq compilingFslib fslibCcu  x y
  let valRefEq  x y = primValRefEq compilingFslib fslibCcu x y
  let unionCaseRefEq x y = primUnionCaseRefEq compilingFslib fslibCcu x y

  let suppressed_types = 
    [ mk_MFCore_tcref fslibCcu "Option`1";
      mk_MFCore_tcref fslibCcu "Ref`1"; 
      mk_MFCore_tcref fslibCcu "FSharpTypeFunc";
      mk_MFCore_tcref fslibCcu "FSharpFunc`2"; 
      mk_MFCore_tcref fslibCcu "Unit" ] 

  let knownFSharpCoreModules = 
     dict [ for nleref in [ fslib_MFLanguagePrimitives_nleref 
                            fslib_MFIntrinsicOperators_nleref
                            fslib_MFIntrinsicFunctions_nleref
                            fslib_MFHashCompare_nleref
                            fslib_MFOperators_nleref 
                            fslib_MFOperatorIntrinsics_nleref
                            fslib_MFOperatorsUnchecked_nleref
                            fslib_MFOperatorsChecked_nleref
                            fslib_MFExtraTopLevelOperators_nleref
                            fslib_MFNullableOperators_nleref
                            fslib_MFQueryRunExtensions_nleref         
                            fslib_MFQueryRunExtensionsLowPriority_nleref  
                            fslib_MFQueryRunExtensionsHighPriority_nleref 

                            fslib_MFSeqModule_nleref    
                            fslib_MFListModule_nleref
                            fslib_MFArrayModule_nleref   
                            fslib_MFArray2DModule_nleref   
                            fslib_MFArray3DModule_nleref   
                            fslib_MFArray4DModule_nleref   
                            fslib_MFSetModule_nleref   
                            fslib_MFMapModule_nleref   
                            fslib_MFStringModule_nleref   
                            fslib_MFOptionModule_nleref   
                            fslib_MFRuntimeHelpers_nleref ] do

                    yield nleref.LastItemMangledName, ERefNonLocal nleref  ]
                                               
  let decodeTupleTy tupInfo l = 
      match l with 
      | [t1;t2;t3;t4;t5;t6;t7;marker] -> 
          match marker with 
          | TType_app(tcref,[t8]) when tyconRefEq tcref ref_tuple1_tcr -> mkRawRefTupleTy [t1;t2;t3;t4;t5;t6;t7;t8]
          | TType_app(tcref,[t8]) when tyconRefEq tcref struct_tuple1_tcr -> mkRawStructTupleTy [t1;t2;t3;t4;t5;t6;t7;t8]
          | TType_tuple (_structness2, t8plus) -> TType_tuple (tupInfo, [t1;t2;t3;t4;t5;t6;t7] @ t8plus)
          | _ -> TType_tuple (tupInfo, l)
      | _ -> TType_tuple (tupInfo, l) 
      

  let mk_MFCore_attrib nm : BuiltinAttribInfo = 
      AttribInfo(mkILTyRef(IlxSettings.ilxFsharpCoreLibScopeRef (), FSharpLib.Core + "." + nm),mk_MFCore_tcref fslibCcu nm) 
    
  let mkAttrib (nm:string) scopeRef : BuiltinAttribInfo = 
      let path, typeName = splitILTypeName nm
      AttribInfo(mkILTyRef (scopeRef, nm), mkSysTyconRef path typeName)

   
  let mkSystemRuntimeAttrib (nm:string) : BuiltinAttribInfo = mkAttrib nm ilg.traits.ScopeRef    
  let mkSystemRuntimeInteropServicesAttribute nm = 
      match ilg.traits.SystemRuntimeInteropServicesScopeRef.Value with 
      | Some assemblyRef -> Some (mkAttrib nm assemblyRef)
      | None -> None
  let mkSystemDiagnosticsDebugAttribute nm = mkAttrib nm (ilg.traits.SystemDiagnosticsDebugScopeRef.Value)

  let mk_doc filename = ILSourceDocument.Create(language=None, vendor=None, documentType=None, file=filename)
  // Build the memoization table for files
  let memoize_file = new MemoizationTable<int,ILSourceDocument> ((fileOfFileIndex >> Filename.fullpath directoryToResolveRelativePaths >> mk_doc), keyComparer=HashIdentity.Structural)

  let and_info =                   makeIntrinsicValRef(fslib_MFIntrinsicOperators_nleref,                    CompileOpName "&"                      ,None                 ,None          ,[],         mk_rel_sig bool_ty) 
  let addrof_info =                makeIntrinsicValRef(fslib_MFIntrinsicOperators_nleref,                    CompileOpName "~&"                     ,None                 ,None          ,[vara],     ([[varaTy]], mkByrefTy varaTy))   
  let addrof2_info =               makeIntrinsicValRef(fslib_MFIntrinsicOperators_nleref,                    CompileOpName "~&&"                    ,None                 ,None          ,[vara],     ([[varaTy]], mkNativePtrTy varaTy))
  let and2_info =                  makeIntrinsicValRef(fslib_MFIntrinsicOperators_nleref,                    CompileOpName "&&"                     ,None                 ,None          ,[],         mk_rel_sig bool_ty) 
  let or_info =                    makeIntrinsicValRef(fslib_MFIntrinsicOperators_nleref,                    "or"                                   ,None                 ,Some "Or"     ,[],         mk_rel_sig bool_ty) 
  let or2_info =                   makeIntrinsicValRef(fslib_MFIntrinsicOperators_nleref,                    CompileOpName "||"                     ,None                 ,None          ,[],         mk_rel_sig bool_ty) 
  let compare_operator_info                = makeIntrinsicValRef(fslib_MFOperators_nleref,                   "compare"                              ,None                 ,Some "Compare",[vara],     mk_compare_sig varaTy) 
  let equals_operator_info                 = makeIntrinsicValRef(fslib_MFOperators_nleref,                   CompileOpName "="                      ,None                 ,None          ,[vara],     mk_rel_sig varaTy) 
  let equals_nullable_operator_info        = makeIntrinsicValRef(fslib_MFNullableOperators_nleref,           CompileOpName "=?"                     ,None                 ,None          ,[vara],     ([[varaTy];[mkNullableTy varaTy]],bool_ty)) 
  let nullable_equals_operator_info        = makeIntrinsicValRef(fslib_MFNullableOperators_nleref,           CompileOpName "?="                     ,None                 ,None          ,[vara],     ([[mkNullableTy varaTy];[varaTy]],bool_ty)) 
  let nullable_equals_nullable_operator_info  = makeIntrinsicValRef(fslib_MFNullableOperators_nleref,        CompileOpName "?=?"                    ,None                 ,None          ,[vara],     ([[mkNullableTy varaTy];[mkNullableTy varaTy]],bool_ty)) 
  let not_equals_operator_info             = makeIntrinsicValRef(fslib_MFOperators_nleref,                   CompileOpName "<>"                     ,None                 ,None          ,[vara],     mk_rel_sig varaTy) 
  let less_than_operator_info              = makeIntrinsicValRef(fslib_MFOperators_nleref,                   CompileOpName "<"                      ,None                 ,None          ,[vara],     mk_rel_sig varaTy) 
  let less_than_or_equals_operator_info    = makeIntrinsicValRef(fslib_MFOperators_nleref,                   CompileOpName "<="                     ,None                 ,None          ,[vara],     mk_rel_sig varaTy) 
  let greater_than_operator_info           = makeIntrinsicValRef(fslib_MFOperators_nleref,                   CompileOpName ">"                      ,None                 ,None          ,[vara],     mk_rel_sig varaTy) 
  let greater_than_or_equals_operator_info = makeIntrinsicValRef(fslib_MFOperators_nleref,                   CompileOpName ">="                     ,None                 ,None          ,[vara],     mk_rel_sig varaTy) 
  
  let enumOfValue_info                     = makeIntrinsicValRef(fslib_MFLanguagePrimitives_nleref,          "EnumOfValue"        ,None                 ,None          ,[vara; varb],     ([[varaTy]], varbTy)) 
  
  let generic_comparison_withc_outer_info = makeIntrinsicValRef(fslib_MFLanguagePrimitives_nleref,           "GenericComparisonWithComparer"        ,None                 ,None          ,[vara],     mk_compare_withc_sig  varaTy) 
  let generic_hash_withc_tuple2_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,           "FastHashTuple2"                                   ,None                 ,None          ,[vara;varb],               mk_hash_withc_sig (decodeTupleTy tupInfoRef [varaTy; varbTy]))   
  let generic_hash_withc_tuple3_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,           "FastHashTuple3"                                   ,None                 ,None          ,[vara;varb;varc],          mk_hash_withc_sig (decodeTupleTy tupInfoRef [varaTy; varbTy; varcTy]))   
  let generic_hash_withc_tuple4_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,           "FastHashTuple4"                                   ,None                 ,None          ,[vara;varb;varc;vard],     mk_hash_withc_sig (decodeTupleTy tupInfoRef [varaTy; varbTy; varcTy; vardTy]))   
  let generic_hash_withc_tuple5_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,           "FastHashTuple5"                                   ,None                 ,None          ,[vara;varb;varc;vard;vare],mk_hash_withc_sig (decodeTupleTy tupInfoRef [varaTy; varbTy; varcTy; vardTy; vareTy]))   
  let generic_equals_withc_tuple2_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,           "FastEqualsTuple2"                               ,None                 ,None          ,[vara;varb],               mk_equality_withc_sig (decodeTupleTy tupInfoRef [varaTy; varbTy]))   
  let generic_equals_withc_tuple3_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,           "FastEqualsTuple3"                               ,None                 ,None          ,[vara;varb;varc],          mk_equality_withc_sig (decodeTupleTy tupInfoRef [varaTy; varbTy; varcTy]))   
  let generic_equals_withc_tuple4_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,           "FastEqualsTuple4"                               ,None                 ,None          ,[vara;varb;varc;vard],     mk_equality_withc_sig (decodeTupleTy tupInfoRef [varaTy; varbTy; varcTy; vardTy]))   
  let generic_equals_withc_tuple5_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,           "FastEqualsTuple5"                               ,None                 ,None          ,[vara;varb;varc;vard;vare],mk_equality_withc_sig (decodeTupleTy tupInfoRef [varaTy; varbTy; varcTy; vardTy; vareTy]))   

  let generic_compare_withc_tuple2_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,           "FastCompareTuple2"                             ,None                 ,None          ,[vara;varb],               mk_compare_withc_sig (decodeTupleTy tupInfoRef [varaTy; varbTy]))   
  let generic_compare_withc_tuple3_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,           "FastCompareTuple3"                             ,None                 ,None          ,[vara;varb;varc],          mk_compare_withc_sig (decodeTupleTy tupInfoRef [varaTy; varbTy; varcTy]))   
  let generic_compare_withc_tuple4_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,           "FastCompareTuple4"                             ,None                 ,None          ,[vara;varb;varc;vard],     mk_compare_withc_sig (decodeTupleTy tupInfoRef [varaTy; varbTy; varcTy; vardTy]))   
  let generic_compare_withc_tuple5_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,           "FastCompareTuple5"                             ,None                 ,None          ,[vara;varb;varc;vard;vare],mk_compare_withc_sig (decodeTupleTy tupInfoRef [varaTy; varbTy; varcTy; vardTy; vareTy]))   


  let generic_equality_er_outer_info             = makeIntrinsicValRef(fslib_MFLanguagePrimitives_nleref,    "GenericEqualityER"                    ,None                 ,None          ,[vara],     mk_rel_sig varaTy) 
  let get_generic_comparer_info               = makeIntrinsicValRef(fslib_MFLanguagePrimitives_nleref,       "GenericComparer"                      ,None                 ,None          ,[],         ([], mk_IComparer_ty)) 
  let get_generic_er_equality_comparer_info      = makeIntrinsicValRef(fslib_MFLanguagePrimitives_nleref,    "GenericEqualityERComparer"            ,None                 ,None          ,[],         ([], mk_IEqualityComparer_ty)) 
  let get_generic_per_equality_comparer_info  = makeIntrinsicValRef(fslib_MFLanguagePrimitives_nleref,       "GenericEqualityComparer"              ,None                 ,None          ,[],         ([], mk_IEqualityComparer_ty)) 
  let generic_equality_withc_outer_info       = makeIntrinsicValRef(fslib_MFLanguagePrimitives_nleref,       "GenericEqualityWithComparer"          ,None                 ,None          ,[vara],     mk_equality_withc_sig varaTy)
  let generic_hash_withc_outer_info           = makeIntrinsicValRef(fslib_MFLanguagePrimitives_nleref,       "GenericHashWithComparer"              ,None                 ,None          ,[vara],     mk_hash_withc_sig varaTy)

  let generic_equality_er_inner_info         = makeIntrinsicValRef(fslib_MFHashCompare_nleref,               "GenericEqualityERIntrinsic"           ,None                 ,None          ,[vara],     mk_rel_sig varaTy)
  let generic_equality_per_inner_info     = makeIntrinsicValRef(fslib_MFHashCompare_nleref,                  "GenericEqualityIntrinsic"             ,None                 ,None          ,[vara],     mk_rel_sig varaTy)
  let generic_equality_withc_inner_info   = makeIntrinsicValRef(fslib_MFHashCompare_nleref,                  "GenericEqualityWithComparerIntrinsic" ,None                 ,None          ,[vara],     mk_equality_withc_sig varaTy)
  let generic_comparison_inner_info       = makeIntrinsicValRef(fslib_MFHashCompare_nleref,                  "GenericComparisonIntrinsic"           ,None                 ,None          ,[vara],     mk_compare_sig varaTy)
  let generic_comparison_withc_inner_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,                  "GenericComparisonWithComparerIntrinsic",None                ,None          ,[vara],     mk_compare_withc_sig varaTy)

  let generic_hash_inner_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,                              "GenericHashIntrinsic"                 ,None                 ,None          ,[vara],     mk_hash_sig varaTy)
  let generic_hash_withc_inner_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,                        "GenericHashWithComparerIntrinsic"     ,None                 ,None          ,[vara],     mk_hash_withc_sig  varaTy)
  
  let create_instance_info       = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "CreateInstance"                       ,None                 ,None          ,[vara],     ([[unit_ty]], varaTy))
  let unbox_info                 = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "UnboxGeneric"                         ,None                 ,None          ,[vara],     ([[obj_ty]], varaTy))

  let unbox_fast_info            = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "UnboxFast"                            ,None                 ,None          ,[vara],     ([[obj_ty]], varaTy))
  let istype_info                = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "TypeTestGeneric"                      ,None                 ,None          ,[vara],     ([[obj_ty]], bool_ty)) 
  let istype_fast_info           = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "TypeTestFast"                         ,None                 ,None          ,[vara],     ([[obj_ty]], bool_ty)) 

  let dispose_info               = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "Dispose"                              ,None                 ,None          ,[vara],     ([[varaTy]],unit_ty))

  let getstring_info             = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "GetString"                            ,None                 ,None          ,[],         ([[string_ty];[int_ty]],char_ty))

  let reference_equality_inner_info = makeIntrinsicValRef(fslib_MFHashCompare_nleref,                        "PhysicalEqualityIntrinsic"            ,None                 ,None          ,[vara],     mk_rel_sig varaTy)  

  let bitwise_or_info            = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_BitwiseOr"                         ,None                 ,None          ,[vara],     mk_binop_ty varaTy)  
  let bitwise_and_info           = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_BitwiseAnd"                        ,None                 ,None          ,[vara],     mk_binop_ty varaTy)  
  let bitwise_xor_info           = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_ExclusiveOr"                       ,None                 ,None          ,[vara],     mk_binop_ty varaTy)  
  let bitwise_unary_not_info     = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_LogicalNot"                        ,None                 ,None          ,[vara],     mk_unop_ty varaTy)  
  let bitwise_shift_left_info    = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_LeftShift"                         ,None                 ,None          ,[vara],     mk_shiftop_ty varaTy)  
  let bitwise_shift_right_info   = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_RightShift"                        ,None                 ,None          ,[vara],     mk_shiftop_ty varaTy)  
  let unchecked_addition_info    = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_Addition"                          ,None                 ,None          ,[vara;varb;varc],     mk_binop_ty3 varaTy varbTy  varcTy)  
  let unchecked_subtraction_info = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_Subtraction"                       ,None                 ,None          ,[vara;varb;varc],     mk_binop_ty3 varaTy varbTy  varcTy)  
  let unchecked_multiply_info    = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_Multiply"                          ,None                 ,None          ,[vara;varb;varc],     mk_binop_ty3 varaTy varbTy  varcTy)  
  let unchecked_unary_plus_info  = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_UnaryPlus"                         ,None                 ,None          ,[vara],     mk_unop_ty varaTy)  
  let unchecked_unary_minus_info = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_UnaryNegation"                     ,None                 ,None          ,[vara],     mk_unop_ty varaTy)  
  let unchecked_unary_not_info   = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "not"                                  ,None                 ,Some "Not"    ,[],     mk_unop_ty bool_ty)  

  let raise_info                 = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "raise"                                ,None                 ,Some "Raise"  ,[vara],     ([[mkSysNonGenericTy sys "Exception"]],varaTy))  
  let failwith_info              = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "failwith"                             ,None               ,Some "FailWith" ,[vara],     ([[string_ty]],varaTy))  
  let invalid_arg_info           = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "invalidArg"                           ,None             ,Some "InvalidArg" ,[vara],     ([[string_ty]; [string_ty]],varaTy))  
  let null_arg_info              = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "nullArg"                              ,None                ,Some "NullArg" ,[vara],     ([[string_ty]],varaTy))  
  let invalid_op_info            = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "invalidOp"                            ,None              ,Some "InvalidOp" ,[vara],     ([[string_ty]],varaTy))  
  let failwithf_info             = makeIntrinsicValRef(fslib_MFExtraTopLevelOperators_nleref,                "failwithf"                       ,None, Some "PrintFormatToStringThenFail" ,[vara;varb],([[mk_format4_ty varaTy unit_ty string_ty string_ty]], varaTy))  
  
  let reraise_info               = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "reraise"                              ,None                 ,Some "Reraise",[vara],     ([[unit_ty]],varaTy))
  let typeof_info                = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "typeof"                               ,None                 ,Some "TypeOf" ,[vara],     ([],system_Type_typ))  
  let methodhandleof_info        = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "methodhandleof"                       ,None                 ,Some "MethodHandleOf",[vara;varb],([[varaTy --> varbTy]],system_RuntimeMethodHandle_typ))
  let sizeof_info                = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "sizeof"                               ,None                 ,Some "SizeOf" ,[vara],     ([],int_ty))  
  let unchecked_defaultof_info   = makeIntrinsicValRef(fslib_MFOperatorsUnchecked_nleref,                    "defaultof"                            ,None                 ,Some "DefaultOf",[vara],     ([],varaTy))  
  let typedefof_info             = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "typedefof"                            ,None                 ,Some "TypeDefOf",[vara],     ([],system_Type_typ))  
  let enum_info                  = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "enum"                                 ,None                 ,Some "ToEnum" ,[vara],     ([[int_ty]],varaTy))  
  let range_op_info              = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_Range"                             ,None                 ,None          ,[vara],     ([[varaTy];[varaTy]],mkSeqTy varaTy))
  let range_step_op_info         = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "op_RangeStep"                         ,None                 ,None          ,[vara;varb],([[varaTy];[varbTy];[varaTy]],mkSeqTy varaTy))
  let range_int32_op_info        = makeIntrinsicValRef(fslib_MFOperatorIntrinsics_nleref,                    "RangeInt32"                           ,None                 ,None          ,[],     ([[int_ty];[int_ty];[int_ty]],mkSeqTy int_ty))
  let array2D_get_info           = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "GetArray2D"                           ,None                 ,None          ,[vara],     ([[mkArrayType 2 varaTy];[int_ty]; [int_ty]],varaTy))  
  let array3D_get_info           = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "GetArray3D"                           ,None                 ,None          ,[vara],     ([[mkArrayType 3 varaTy];[int_ty]; [int_ty]; [int_ty]],varaTy))
  let array4D_get_info           = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "GetArray4D"                           ,None                 ,None          ,[vara],     ([[mkArrayType 4 varaTy];[int_ty]; [int_ty]; [int_ty]; [int_ty]],varaTy))

  let seq_collect_info           = makeIntrinsicValRef(fslib_MFSeqModule_nleref,                             "collect"                              ,None                 ,Some "Collect",[vara;varb;varc],([[varaTy --> varbTy]; [mkSeqTy varaTy]], mkSeqTy varcTy))  
  let seq_delay_info             = makeIntrinsicValRef(fslib_MFSeqModule_nleref,                             "delay"                                ,None                 ,Some "Delay"  ,[varb],     ([[unit_ty --> mkSeqTy varbTy]], mkSeqTy varbTy)) 
  let seq_append_info            = makeIntrinsicValRef(fslib_MFSeqModule_nleref,                             "append"                               ,None                 ,Some "Append" ,[varb],     ([[mkSeqTy varbTy]; [mkSeqTy varbTy]], mkSeqTy varbTy))  
  let seq_using_info             = makeIntrinsicValRef(fslib_MFRuntimeHelpers_nleref,                        "EnumerateUsing"                       ,None                 ,None          ,[vara;varb;varc], ([[varaTy];[(varaTy --> varbTy)]],mkSeqTy varcTy))
  let seq_generated_info         = makeIntrinsicValRef(fslib_MFRuntimeHelpers_nleref,                        "EnumerateWhile"                       ,None                 ,None          ,[varb],     ([[unit_ty --> bool_ty]; [mkSeqTy varbTy]], mkSeqTy varbTy))
  let seq_finally_info           = makeIntrinsicValRef(fslib_MFRuntimeHelpers_nleref,                        "EnumerateThenFinally"                 ,None                 ,None          ,[varb],     ([[mkSeqTy varbTy]; [unit_ty --> unit_ty]], mkSeqTy varbTy))
  let seq_of_functions_info      = makeIntrinsicValRef(fslib_MFRuntimeHelpers_nleref,                        "EnumerateFromFunctions"               ,None                 ,None          ,[vara;varb],([[unit_ty --> varaTy]; [varaTy --> bool_ty]; [varaTy --> varbTy]], mkSeqTy varbTy))  
  let create_event_info          = makeIntrinsicValRef(fslib_MFRuntimeHelpers_nleref,                        "CreateEvent"                          ,None                 ,None          ,[vara;varb],([[varaTy --> unit_ty]; [varaTy --> unit_ty]; [(obj_ty --> (varbTy --> unit_ty)) --> varaTy]], TType_app (fslib_IEvent2_tcr, [varaTy;varbTy])))
  let seq_to_array_info          = makeIntrinsicValRef(fslib_MFSeqModule_nleref,                             "toArray"                              ,None                 ,Some "ToArray",[varb],     ([[mkSeqTy varbTy]], mkArrayType 1 varbTy))  
  let seq_to_list_info           = makeIntrinsicValRef(fslib_MFSeqModule_nleref,                             "toList"                               ,None                 ,Some "ToList" ,[varb],     ([[mkSeqTy varbTy]], mkListTy varbTy))
  let seq_map_info               = makeIntrinsicValRef(fslib_MFSeqModule_nleref,                             "map"                                  ,None                 ,Some "Map"    ,[vara;varb],([[varaTy --> varbTy]; [mkSeqTy varaTy]], mkSeqTy varbTy))
  let seq_singleton_info         = makeIntrinsicValRef(fslib_MFSeqModule_nleref,                             "singleton"                            ,None                 ,Some "Singleton"              ,[vara],     ([[varaTy]], mkSeqTy varaTy))
  let seq_empty_info             = makeIntrinsicValRef(fslib_MFSeqModule_nleref,                             "empty"                                ,None                 ,Some "Empty"                  ,[vara],     ([], mkSeqTy varaTy))
  let new_format_info            = makeIntrinsicValRef(fslib_MFCore_nleref,                                  ".ctor"                                ,Some "PrintfFormat`5",None                          ,[vara;varb;varc;vard;vare], ([[string_ty]], mkPrintfFormatTy varaTy varbTy varcTy vardTy vareTy))  
  let sprintf_info               = makeIntrinsicValRef(fslib_MFExtraTopLevelOperators_nleref,                "sprintf"                              ,None                 ,Some "PrintFormatToStringThen",[vara],     ([[mk_format4_ty varaTy unit_ty string_ty string_ty]], varaTy))  
  let lazy_force_info            = 
    // Lazy\Value for > 4.0
                                   makeIntrinsicValRef(fslib_MFLazyExtensions_nleref,                        "Force"                                ,Some "Lazy`1"        ,None                          ,[vara],     ([[mkLazyTy varaTy]; []], varaTy))
  let lazy_create_info           = makeIntrinsicValRef(fslib_MFLazyExtensions_nleref,                        "Create"                               ,Some "Lazy`1"        ,None                          ,[vara],     ([[unit_ty --> varaTy]], mkLazyTy varaTy))

  let seq_info                   = makeIntrinsicValRef(fslib_MFOperators_nleref,                             "seq"                                  ,None                 ,Some "CreateSequence"         ,[vara],     ([[mkSeqTy varaTy]], mkSeqTy varaTy))
  let splice_expr_info           = makeIntrinsicValRef(fslib_MFExtraTopLevelOperators_nleref,                "op_Splice"                            ,None                 ,None                          ,[vara],     ([[mkQuotedExprTy varaTy]], varaTy))
  let splice_raw_expr_info       = makeIntrinsicValRef(fslib_MFExtraTopLevelOperators_nleref,                "op_SpliceUntyped"                     ,None                 ,None                          ,[vara],     ([[mkRawQuotedExprTy]], varaTy))
  let new_decimal_info           = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "MakeDecimal"                          ,None                 ,None                          ,[],         ([[int_ty]; [int_ty]; [int_ty]; [bool_ty]; [byte_ty]], decimal_ty))
  let array_get_info             = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "GetArray"                             ,None                 ,None                          ,[vara],     ([[mkArrayType 1 varaTy]; [int_ty]], varaTy))
  let array_length_info          = makeIntrinsicValRef(fslib_MFArrayModule_nleref,                           "length"                               ,None                 ,Some "Length"                 ,[vara],     ([[mkArrayType 1 varaTy]], int_ty))
  let deserialize_quoted_FSharp_20_plus_info    = makeIntrinsicValRef(fslib_MFQuotations_nleref,             "Deserialize"                          ,Some "Expr"          ,None                          ,[],          ([[system_Type_typ ;mkListTy system_Type_typ ;mkListTy mkRawQuotedExprTy ; mkArrayType 1 byte_ty]], mkRawQuotedExprTy ))
  let deserialize_quoted_FSharp_40_plus_info    = makeIntrinsicValRef(fslib_MFQuotations_nleref,             "Deserialize40"                        ,Some "Expr"          ,None                          ,[],          ([[system_Type_typ ;mkArrayType 1 system_Type_typ; mkArrayType 1 system_Type_typ; mkArrayType 1 mkRawQuotedExprTy; mkArrayType 1 byte_ty]], mkRawQuotedExprTy ))
  let cast_quotation_info        = makeIntrinsicValRef(fslib_MFQuotations_nleref,                            "Cast"                                 ,Some "Expr"          ,None                          ,[vara],      ([[mkRawQuotedExprTy]], mkQuotedExprTy varaTy))
  let lift_value_info            = makeIntrinsicValRef(fslib_MFQuotations_nleref,                            "Value"                                ,Some "Expr"          ,None                          ,[vara],      ([[varaTy]], mkRawQuotedExprTy))
  let lift_value_with_name_info  = makeIntrinsicValRef(fslib_MFQuotations_nleref,                            "ValueWithName"                        ,Some "Expr"          ,None                          ,[vara],      ([[varaTy; string_ty]], mkRawQuotedExprTy))
  let lift_value_with_defn_info  = makeIntrinsicValRef(fslib_MFQuotations_nleref,                            "WithValue"                  ,Some "Expr"          ,None                          ,[vara],      ([[varaTy; mkQuotedExprTy varaTy]], mkQuotedExprTy varaTy))
  let query_value_info           = makeIntrinsicValRef(fslib_MFExtraTopLevelOperators_nleref,                "query"                                ,None                 ,None                          ,[],      ([], mkQueryBuilderTy) )
  let query_run_value_info       = makeIntrinsicValRef(fslib_MFQueryRunExtensionsLowPriority_nleref,         "Run"                                  ,Some "QueryBuilder"  ,None                          ,[vara],      ([[mkQueryBuilderTy];[mkQuotedExprTy varaTy]], varaTy) )
  let query_run_enumerable_info  = makeIntrinsicValRef(fslib_MFQueryRunExtensionsHighPriority_nleref,        "Run"                                  ,Some "QueryBuilder"  ,None                          ,[vara],      ([[mkQueryBuilderTy];[mkQuotedExprTy (mkQuerySourceTy varaTy (mkNonGenericTy tcref_System_Collections_IEnumerable)) ]], mkSeqTy varaTy) )
  let query_for_value_info       = makeIntrinsicValRef(fslib_MFLinq_nleref,                                  "For"                                  ,Some "QueryBuilder"  ,None                          ,[vara; vard; varb; vare], ([[mkQueryBuilderTy];[mkQuerySourceTy varaTy vardTy;varaTy --> mkQuerySourceTy varbTy vareTy]], mkQuerySourceTy varbTy vardTy) )
  let query_select_value_info    = makeIntrinsicValRef(fslib_MFLinq_nleref,                                  "Select"                               ,Some "QueryBuilder"  ,None                          ,[vara; vare; varb], ([[mkQueryBuilderTy];[mkQuerySourceTy varaTy vareTy;varaTy --> varbTy]], mkQuerySourceTy varbTy vareTy) )
  let query_yield_value_info     = makeIntrinsicValRef(fslib_MFLinq_nleref,                                  "Yield"                                ,Some "QueryBuilder"  ,None                          ,[vara; vare],      ([[mkQueryBuilderTy];[varaTy]], mkQuerySourceTy varaTy vareTy) )
  let query_yield_from_value_info = makeIntrinsicValRef(fslib_MFLinq_nleref,                                 "YieldFrom"                            ,Some "QueryBuilder"  ,None                          ,[vara; vare],      ([[mkQueryBuilderTy];[mkQuerySourceTy varaTy vareTy]], mkQuerySourceTy varaTy vareTy) )
  let query_source_info          = makeIntrinsicValRef(fslib_MFLinq_nleref,                                  "Source"                               ,Some "QueryBuilder"  ,None                          ,[vara],      ([[mkQueryBuilderTy];[mkSeqTy varaTy ]], mkQuerySourceTy varaTy (mkNonGenericTy tcref_System_Collections_IEnumerable)) )
  let query_source_as_enum_info  = makeIntrinsicValRef(fslib_MFLinq_nleref,                                  "get_Source"                           ,Some "QuerySource`2" ,None                          ,[vara; vare],      ([[mkQuerySourceTy varaTy vareTy];[]], mkSeqTy varaTy) )
  let new_query_source_info     = makeIntrinsicValRef(fslib_MFLinq_nleref,                                  ".ctor"                                 ,Some "QuerySource`2" ,None                          ,[vara; vare],      ([[mkSeqTy varaTy]], mkQuerySourceTy varaTy vareTy) )
  let query_where_value_info     = makeIntrinsicValRef(fslib_MFLinq_nleref,                                  "Where"                                ,Some "QueryBuilder"  ,None                          ,[vara; vare],      ([[mkQueryBuilderTy];[mkQuerySourceTy varaTy vareTy;varaTy --> bool_ty]], mkQuerySourceTy varaTy vareTy) )
  let query_zero_value_info      = makeIntrinsicValRef(fslib_MFLinq_nleref,                                  "Zero"                                 ,Some "QueryBuilder"  ,None                          ,[vara; vare],      ([[mkQueryBuilderTy];[]], mkQuerySourceTy varaTy vareTy) )
  let fail_init_info             = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "FailInit"                             ,None                 ,None                          ,[],      ([[unit_ty]], unit_ty))
  let fail_static_init_info      = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "FailStaticInit"                       ,None                 ,None                          ,[],      ([[unit_ty]], unit_ty))
  let check_this_info            = makeIntrinsicValRef(fslib_MFIntrinsicFunctions_nleref,                    "CheckThis"                            ,None                 ,None                          ,[vara],      ([[varaTy]], varaTy))
  let quote_to_linq_lambda_info  = makeIntrinsicValRef(fslib_MFLinqRuntimeHelpersQuotationConverter_nleref,  "QuotationToLambdaExpression"          ,None                 ,None                          ,[vara],      ([[mkQuotedExprTy varaTy]], mkLinqExpressionTy varaTy))
    
  { ilg=ilg
#if NO_COMPILER_BACKEND
#else
    ilxPubCloEnv=EraseClosures.newIlxPubCloEnv(ilg)
#endif
    knownIntrinsics                = knownIntrinsics
    knownFSharpCoreModules         = knownFSharpCoreModules
    compilingFslib                 = compilingFslib
    mlCompatibility                = mlCompatibility
    emitDebugInfoInQuotations      = emitDebugInfoInQuotations
    directoryToResolveRelativePaths= directoryToResolveRelativePaths
    unionCaseRefEq                 = unionCaseRefEq
    valRefEq                 = valRefEq
    fslibCcu                 = fslibCcu
    using40environment       = using40environment
    sysCcu                   = sysCcu
    refcell_tcr_canon    = mk_MFCore_tcref     fslibCcu "Ref`1"
    option_tcr_canon     = mk_MFCore_tcref     fslibCcu "Option`1"
    list_tcr_canon       = mk_MFCollections_tcref   fslibCcu "List`1"
    set_tcr_canon        = mk_MFCollections_tcref   fslibCcu "Set`1"
    map_tcr_canon        = mk_MFCollections_tcref   fslibCcu "Map`2"
    lazy_tcr_canon       = lazy_tcr
    refcell_tcr_nice     = mk_MFCore_tcref     fslibCcu "ref`1"
    array_tcr_nice       = il_arr_tcr_map.[0]
    option_tcr_nice   = option_tcr_nice
    list_tcr_nice     = list_tcr_nice
    lazy_tcr_nice     = lazy_tcr_nice
    format_tcr       = format_tcr
    expr_tcr       = expr_tcr
    raw_expr_tcr       = raw_expr_tcr
    nativeint_tcr  = nativeint_tcr
    int32_tcr      = int32_tcr
    int16_tcr      = int16_tcr
    int64_tcr      = int64_tcr
    uint16_tcr     = uint16_tcr
    uint32_tcr     = uint32_tcr
    uint64_tcr     = uint64_tcr
    sbyte_tcr      = sbyte_tcr
    decimal_tcr    = decimal_tcr
    date_tcr    = date_tcr
    pdecimal_tcr   = pdecimal_tcr
    byte_tcr       = byte_tcr
    bool_tcr       = bool_tcr
    unit_tcr_canon = unit_tcr_canon
    unit_tcr_nice  = unit_tcr_nice
    exn_tcr        = exn_tcr
    char_tcr       = char_tcr
    float_tcr      = float_tcr
    float32_tcr    = float32_tcr
    pfloat_tcr     = pfloat_tcr
    pfloat32_tcr   = pfloat32_tcr
    pint_tcr       = pint_tcr
    pint8_tcr      = pint8_tcr
    pint16_tcr     = pint16_tcr
    pint64_tcr     = pint64_tcr
    byref_tcr      = byref_tcr
    nativeptr_tcr  = nativeptr_tcr
    ilsigptr_tcr   = ilsigptr_tcr
    fastFunc_tcr = fastFunc_tcr
    tcref_IQueryable = tcref_IQueryable
    tcref_IObservable      = tcref_IObservable
    tcref_IObserver      = tcref_IObserver
    fslib_IEvent2_tcr      = fslib_IEvent2_tcr
    fslib_IDelegateEvent_tcr      = fslib_IDelegateEvent_tcr
    seq_tcr        = seq_tcr
    seq_base_tcr = mk_MFCompilerServices_tcref fslibCcu "GeneratedSequenceBase`1"
    measureproduct_tcr = mk_MFCompilerServices_tcref fslibCcu "MeasureProduct`2"
    measureinverse_tcr = mk_MFCompilerServices_tcref fslibCcu "MeasureInverse`1"
    measureone_tcr = mk_MFCompilerServices_tcref fslibCcu "MeasureOne"
    il_arr_tcr_map = il_arr_tcr_map
    ref_tuple1_tcr     = ref_tuple1_tcr
    ref_tuple2_tcr     = ref_tuple2_tcr
    ref_tuple3_tcr     = ref_tuple3_tcr
    ref_tuple4_tcr     = ref_tuple4_tcr
    ref_tuple5_tcr     = ref_tuple5_tcr
    ref_tuple6_tcr     = ref_tuple6_tcr
    ref_tuple7_tcr     = ref_tuple7_tcr
    ref_tuple8_tcr     = ref_tuple8_tcr
    struct_tuple1_tcr     = struct_tuple1_tcr
    struct_tuple2_tcr     = struct_tuple2_tcr
    struct_tuple3_tcr     = struct_tuple3_tcr
    struct_tuple4_tcr     = struct_tuple4_tcr
    struct_tuple5_tcr     = struct_tuple5_tcr
    struct_tuple6_tcr     = struct_tuple6_tcr
    struct_tuple7_tcr     = struct_tuple7_tcr
    struct_tuple8_tcr     = struct_tuple8_tcr
    choice2_tcr    = choice2_tcr
    choice3_tcr    = choice3_tcr
    choice4_tcr    = choice4_tcr
    choice5_tcr    = choice5_tcr
    choice6_tcr    = choice6_tcr
    choice7_tcr    = choice7_tcr
    nativeint_ty  = mkNonGenericTy nativeint_tcr
    unativeint_ty = mkNonGenericTy unativeint_tcr
    int32_ty      = mkNonGenericTy int32_tcr
    int16_ty      = mkNonGenericTy int16_tcr
    int64_ty      = mkNonGenericTy int64_tcr
    uint16_ty     = mkNonGenericTy uint16_tcr
    uint32_ty     = mkNonGenericTy uint32_tcr
    uint64_ty     = mkNonGenericTy uint64_tcr
    sbyte_ty      = mkNonGenericTy sbyte_tcr
    byte_ty       = byte_ty
    bool_ty       = bool_ty
    int_ty       = int_ty
    string_ty     = string_ty
    obj_ty        = mkNonGenericTy obj_tcr
    unit_ty       = unit_ty
    exn_ty        = mkNonGenericTy exn_tcr
    char_ty       = mkNonGenericTy char_tcr
    decimal_ty    = mkNonGenericTy decimal_tcr
    float_ty      = mkNonGenericTy float_tcr 
    float32_ty    = mkNonGenericTy float32_tcr
    memoize_file  = memoize_file.Apply

    system_Array_typ     = mkSysNonGenericTy sys "Array"
    system_Object_typ    = mkSysNonGenericTy sys "Object"
    system_IDisposable_typ    = mkSysNonGenericTy sys "IDisposable"
    system_RuntimeHelpers_typ    = mkSysNonGenericTy sysCompilerServices "RuntimeHelpers"
    system_Value_typ     = mkSysNonGenericTy sys "ValueType"
    system_Delegate_typ     = mkSysNonGenericTy sys "Delegate"
    system_MulticastDelegate_typ     = mkSysNonGenericTy sys "MulticastDelegate"
    system_Enum_typ      = mkSysNonGenericTy sys "Enum"
    system_Exception_typ = mkSysNonGenericTy sys "Exception"
    system_String_typ    = mkSysNonGenericTy sys "String"
    system_String_tcref  = mkSysTyconRef sys "String"
    system_Int32_typ     = mkSysNonGenericTy sys "Int32"
    system_Type_typ                  = system_Type_typ
    system_TypedReference_tcref        = if ilg.traits.TypedReferenceTypeScopeRef.IsSome then Some(mkSysTyconRef sys "TypedReference") else None
    system_ArgIterator_tcref           = if ilg.traits.ArgIteratorTypeScopeRef.IsSome then Some(mkSysTyconRef sys "ArgIterator") else None
    system_RuntimeArgumentHandle_tcref =  if ilg.traits.RuntimeArgumentHandleTypeScopeRef.IsSome then Some (mkSysTyconRef sys "RuntimeArgumentHandle") else None
    system_SByte_tcref =  mkSysTyconRef sys "SByte"
    system_Decimal_tcref =  mkSysTyconRef sys "Decimal"
    system_Int16_tcref =  mkSysTyconRef sys "Int16"
    system_Int32_tcref =  mkSysTyconRef sys "Int32"
    system_Int64_tcref =  mkSysTyconRef sys "Int64"
    system_IntPtr_tcref =  mkSysTyconRef sys "IntPtr"
    system_Bool_tcref =  mkSysTyconRef sys "Boolean" 
    system_Byte_tcref =  mkSysTyconRef sys "Byte"
    system_UInt16_tcref =  mkSysTyconRef sys "UInt16"
    system_Char_tcref            =  mkSysTyconRef sys "Char"
    system_UInt32_tcref          =  mkSysTyconRef sys "UInt32"
    system_UInt64_tcref          =  mkSysTyconRef sys "UInt64"
    system_UIntPtr_tcref         =  mkSysTyconRef sys "UIntPtr"
    system_Single_tcref          =  mkSysTyconRef sys "Single"
    system_Double_tcref          =  mkSysTyconRef sys "Double"
    system_RuntimeTypeHandle_typ = mkSysNonGenericTy sys "RuntimeTypeHandle"
    system_RuntimeMethodHandle_typ = system_RuntimeMethodHandle_typ
    
    system_MarshalByRefObject_tcref =  if ilg.traits.MarshalByRefObjectScopeRef.IsSome then Some(mkSysTyconRef sys "MarshalByRefObject") else None
    system_MarshalByRefObject_typ = if ilg.traits.MarshalByRefObjectScopeRef.IsSome then Some(mkSysNonGenericTy sys "MarshalByRefObject") else None

    system_Reflection_MethodInfo_typ = system_Reflection_MethodInfo_typ
    
    system_Array_tcref  = mkSysTyconRef sys "Array"
    system_Object_tcref  = mkSysTyconRef sys "Object"
    system_Void_tcref    = mkSysTyconRef sys "Void"
    system_IndexOutOfRangeException_tcref    = mkSysTyconRef sys "IndexOutOfRangeException"
    system_Nullable_tcref = nullable_tcr
    system_GenericIComparable_tcref = mkSysTyconRef sys "IComparable`1"
    system_GenericIEquatable_tcref = mkSysTyconRef sys "IEquatable`1"
    mk_IComparable_ty    = mkSysNonGenericTy sys "IComparable"
    system_LinqExpression_tcref = linqExpression_tcr

    mk_IStructuralComparable_ty = mkSysNonGenericTy sysCollections "IStructuralComparable"
        
    mk_IStructuralEquatable_ty = mkSysNonGenericTy sysCollections "IStructuralEquatable"

    mk_IComparer_ty = mk_IComparer_ty
    mk_IEqualityComparer_ty = mk_IEqualityComparer_ty
    tcref_System_Collections_IComparer = mkSysTyconRef sysCollections "IComparer"
    tcref_System_Collections_IEqualityComparer = mkSysTyconRef sysCollections "IEqualityComparer"
    tcref_System_Collections_Generic_IEqualityComparer = mkSysTyconRef sysGenerics "IEqualityComparer`1"
    tcref_System_Collections_Generic_Dictionary = mkSysTyconRef sysGenerics "Dictionary`2"
    
    tcref_System_IComparable = mkSysTyconRef sys "IComparable"
    tcref_System_IStructuralComparable = mkSysTyconRef sysCollections "IStructuralComparable"
    tcref_System_IStructuralEquatable  = mkSysTyconRef sysCollections "IStructuralEquatable"
            
    tcref_LanguagePrimitives = mk_MFCore_tcref fslibCcu "LanguagePrimitives"


    tcref_System_Collections_Generic_IList       = mkSysTyconRef sysGenerics "IList`1"
    tcref_System_Collections_Generic_IReadOnlyList       = mkSysTyconRef sysGenerics "IReadOnlyList`1"
    tcref_System_Collections_Generic_ICollection = mkSysTyconRef sysGenerics "ICollection`1"
    tcref_System_Collections_Generic_IReadOnlyCollection = mkSysTyconRef sysGenerics "IReadOnlyCollection`1"
    tcref_System_Collections_IEnumerable         = tcref_System_Collections_IEnumerable

    tcref_System_Collections_Generic_IEnumerable = IEnumerable_tcr
    tcref_System_Collections_Generic_IEnumerator = IEnumerator_tcr
    
    tcref_System_Attribute = System_Attribute_tcr

    attrib_AttributeUsageAttribute = mkSystemRuntimeAttrib "System.AttributeUsageAttribute"
    attrib_ParamArrayAttribute     = mkSystemRuntimeAttrib "System.ParamArrayAttribute"
    attrib_IDispatchConstantAttribute  = if ilg.traits.IDispatchConstantAttributeScopeRef.IsSome then Some(mkSystemRuntimeAttrib "System.Runtime.CompilerServices.IDispatchConstantAttribute") else None
    attrib_IUnknownConstantAttribute  = if ilg.traits.IUnknownConstantAttributeScopeRef.IsSome then Some (mkSystemRuntimeAttrib "System.Runtime.CompilerServices.IUnknownConstantAttribute") else None
    
    attrib_SystemObsolete          = mkSystemRuntimeAttrib "System.ObsoleteAttribute"
    attrib_DllImportAttribute      = mkSystemRuntimeInteropServicesAttribute "System.Runtime.InteropServices.DllImportAttribute"
    attrib_StructLayoutAttribute   = mkSystemRuntimeAttrib "System.Runtime.InteropServices.StructLayoutAttribute"
    attrib_TypeForwardedToAttribute   = mkSystemRuntimeAttrib "System.Runtime.CompilerServices.TypeForwardedToAttribute"
    attrib_ComVisibleAttribute     = mkSystemRuntimeAttrib "System.Runtime.InteropServices.ComVisibleAttribute"
    attrib_ComImportAttribute      = mkSystemRuntimeInteropServicesAttribute "System.Runtime.InteropServices.ComImportAttribute"
    attrib_FieldOffsetAttribute    = mkSystemRuntimeAttrib "System.Runtime.InteropServices.FieldOffsetAttribute" 
    attrib_MarshalAsAttribute      = mkSystemRuntimeInteropServicesAttribute "System.Runtime.InteropServices.MarshalAsAttribute"
    attrib_InAttribute             = mkSystemRuntimeInteropServicesAttribute "System.Runtime.InteropServices.InAttribute" 
    attrib_OutAttribute            = mkSystemRuntimeAttrib "System.Runtime.InteropServices.OutAttribute" 
    attrib_OptionalAttribute       = mkSystemRuntimeInteropServicesAttribute "System.Runtime.InteropServices.OptionalAttribute" 
    attrib_ThreadStaticAttribute   = if ilg.traits.ThreadStaticAttributeScopeRef.IsSome then Some(mkSystemRuntimeAttrib "System.ThreadStaticAttribute") else None
    attrib_SpecialNameAttribute   = if ilg.traits.SpecialNameAttributeScopeRef.IsSome then Some(mkSystemRuntimeAttrib "System.Runtime.CompilerServices.SpecialNameAttribute") else None
    attrib_VolatileFieldAttribute   = mk_MFCore_attrib "VolatileFieldAttribute"
    attrib_ContextStaticAttribute  = if ilg.traits.ContextStaticAttributeScopeRef.IsSome then Some (mkSystemRuntimeAttrib "System.ContextStaticAttribute") else None
    attrib_FlagsAttribute          = mkSystemRuntimeAttrib "System.FlagsAttribute"
    attrib_DefaultMemberAttribute  = mkSystemRuntimeAttrib "System.Reflection.DefaultMemberAttribute"
    attrib_DebuggerDisplayAttribute  = mkSystemDiagnosticsDebugAttribute "System.Diagnostics.DebuggerDisplayAttribute"
    attrib_DebuggerTypeProxyAttribute  = mkSystemDiagnosticsDebugAttribute "System.Diagnostics.DebuggerTypeProxyAttribute"
    attrib_PreserveSigAttribute    = mkSystemRuntimeInteropServicesAttribute "System.Runtime.InteropServices.PreserveSigAttribute"
    attrib_MethodImplAttribute     = mkSystemRuntimeAttrib "System.Runtime.CompilerServices.MethodImplAttribute"
    attrib_ExtensionAttribute     = mkSystemRuntimeAttrib "System.Runtime.CompilerServices.ExtensionAttribute"
    attrib_CallerLineNumberAttribute = mkSystemRuntimeAttrib "System.Runtime.CompilerServices.CallerLineNumberAttribute"
    attrib_CallerFilePathAttribute = mkSystemRuntimeAttrib "System.Runtime.CompilerServices.CallerFilePathAttribute"
    attrib_CallerMemberNameAttribute = mkSystemRuntimeAttrib "System.Runtime.CompilerServices.CallerMemberNameAttribute"

    attrib_ProjectionParameterAttribute           = mk_MFCore_attrib "ProjectionParameterAttribute"
    attrib_CustomOperationAttribute               = mk_MFCore_attrib "CustomOperationAttribute"
    attrib_NonSerializedAttribute                 = if ilg.traits.NonSerializedAttributeScopeRef.IsSome then Some(mkSystemRuntimeAttrib "System.NonSerializedAttribute") else None
    attrib_AutoSerializableAttribute              = mk_MFCore_attrib "AutoSerializableAttribute"
    attrib_RequireQualifiedAccessAttribute        = mk_MFCore_attrib "RequireQualifiedAccessAttribute"
    attrib_EntryPointAttribute                    = mk_MFCore_attrib "EntryPointAttribute"
    attrib_DefaultAugmentationAttribute           = mk_MFCore_attrib "DefaultAugmentationAttribute"
    attrib_CompilerMessageAttribute               = mk_MFCore_attrib "CompilerMessageAttribute"
    attrib_ExperimentalAttribute                  = mk_MFCore_attrib "ExperimentalAttribute"
    attrib_UnverifiableAttribute                  = mk_MFCore_attrib "UnverifiableAttribute"
    attrib_LiteralAttribute                       = mk_MFCore_attrib "LiteralAttribute"
    attrib_ConditionalAttribute                   = mkSystemRuntimeAttrib "System.Diagnostics.ConditionalAttribute"
    attrib_OptionalArgumentAttribute              = mk_MFCore_attrib "OptionalArgumentAttribute"
    attrib_RequiresExplicitTypeArgumentsAttribute = mk_MFCore_attrib "RequiresExplicitTypeArgumentsAttribute"
    attrib_DefaultValueAttribute                  = mk_MFCore_attrib "DefaultValueAttribute"
    attrib_ClassAttribute                         = mk_MFCore_attrib "ClassAttribute"
    attrib_InterfaceAttribute                     = mk_MFCore_attrib "InterfaceAttribute"
    attrib_StructAttribute                        = mk_MFCore_attrib "StructAttribute"
    attrib_ReflectedDefinitionAttribute           = mk_MFCore_attrib "ReflectedDefinitionAttribute"
    attrib_CompiledNameAttribute                  = mk_MFCore_attrib "CompiledNameAttribute"
    attrib_AutoOpenAttribute                      = mk_MFCore_attrib "AutoOpenAttribute"
    attrib_InternalsVisibleToAttribute            = mkSystemRuntimeAttrib "System.Runtime.CompilerServices.InternalsVisibleToAttribute"
    attrib_CompilationRepresentationAttribute     = mk_MFCore_attrib "CompilationRepresentationAttribute"
    attrib_CompilationArgumentCountsAttribute     = mk_MFCore_attrib "CompilationArgumentCountsAttribute"
    attrib_CompilationMappingAttribute            = mk_MFCore_attrib "CompilationMappingAttribute"
    attrib_CLIEventAttribute                      = mk_MFCore_attrib "CLIEventAttribute"
    attrib_CLIMutableAttribute                    = mk_MFCore_attrib "CLIMutableAttribute"
    attrib_AllowNullLiteralAttribute              = mk_MFCore_attrib "AllowNullLiteralAttribute"
    attrib_NoEqualityAttribute                    = mk_MFCore_attrib "NoEqualityAttribute"
    attrib_NoComparisonAttribute                  = mk_MFCore_attrib "NoComparisonAttribute"
    attrib_CustomEqualityAttribute                = mk_MFCore_attrib "CustomEqualityAttribute"
    attrib_CustomComparisonAttribute              = mk_MFCore_attrib "CustomComparisonAttribute"
    attrib_EqualityConditionalOnAttribute         = mk_MFCore_attrib "EqualityConditionalOnAttribute"
    attrib_ComparisonConditionalOnAttribute       = mk_MFCore_attrib "ComparisonConditionalOnAttribute"
    attrib_ReferenceEqualityAttribute             = mk_MFCore_attrib "ReferenceEqualityAttribute"
    attrib_StructuralEqualityAttribute            = mk_MFCore_attrib "StructuralEqualityAttribute"
    attrib_StructuralComparisonAttribute          = mk_MFCore_attrib "StructuralComparisonAttribute"
    attrib_SealedAttribute                        = mk_MFCore_attrib "SealedAttribute"
    attrib_AbstractClassAttribute                 = mk_MFCore_attrib "AbstractClassAttribute"
    attrib_GeneralizableValueAttribute            = mk_MFCore_attrib "GeneralizableValueAttribute"
    attrib_MeasureAttribute                       = mk_MFCore_attrib "MeasureAttribute"
    attrib_MeasureableAttribute                   = mk_MFCore_attrib "MeasureAnnotatedAbbreviationAttribute"
    attrib_NoDynamicInvocationAttribute           = mk_MFCore_attrib "NoDynamicInvocationAttribute"
    attrib_SecurityAttribute                      = if ilg.traits.SecurityPermissionAttributeTypeScopeRef.IsSome then Some(mkSystemRuntimeAttrib"System.Security.Permissions.SecurityAttribute") else None
    attrib_SecurityCriticalAttribute              = mkSystemRuntimeAttrib "System.Security.SecurityCriticalAttribute"
    attrib_SecuritySafeCriticalAttribute          = mkSystemRuntimeAttrib "System.Security.SecuritySafeCriticalAttribute"

    // Build a map that uses the "canonical" F# type names and TyconRef's for these
    // in preference to the .NET type names. Doing this normalization is a fairly performance critical
    // piece of code as it is frequently invoked in the process of converting .NET metadata to F# internal
    // compiler data structures (see import.fs).
    better_tcref_map = 
       begin 
        let entries1 = 
         [ "Int32", int_tcr 
           "IntPtr", nativeint_tcr 
           "UIntPtr", unativeint_tcr
           "Int16",int16_tcr 
           "Int64",int64_tcr 
           "UInt16",uint16_tcr
           "UInt32",uint32_tcr
           "UInt64",uint64_tcr
           "SByte",sbyte_tcr
           "Decimal",decimal_tcr
           "Byte",byte_tcr
           "Boolean",bool_tcr
           "String",string_tcr
           "Object",obj_tcr
           "Exception",exn_tcr
           "Char",char_tcr
           "Double",float_tcr
           "Single",float32_tcr] 
             |> List.map (fun (nm,tcr) -> 
                   let ty = mkNonGenericTy tcr 
                   nm, mkSysTyconRef sys nm, (fun _ -> ty)) 

        let entries2 =
            [ "FSharpFunc`2",    fastFunc_tcr, (fun tinst -> mkFunTy (List.item 0 tinst) (List.item 1 tinst))
              "Tuple`2",       ref_tuple2_tcr, decodeTupleTy tupInfoRef
              "Tuple`3",       ref_tuple3_tcr, decodeTupleTy tupInfoRef
              "Tuple`4",       ref_tuple4_tcr, decodeTupleTy tupInfoRef
              "Tuple`5",       ref_tuple5_tcr, decodeTupleTy tupInfoRef
              "Tuple`6",       ref_tuple6_tcr, decodeTupleTy tupInfoRef
              "Tuple`7",       ref_tuple7_tcr, decodeTupleTy tupInfoRef
              "Tuple`8",       ref_tuple8_tcr, decodeTupleTy tupInfoRef
              "ValueTuple`2",       struct_tuple2_tcr, decodeTupleTy tupInfoStruct
              "ValueTuple`3",       struct_tuple3_tcr, decodeTupleTy tupInfoStruct
              "ValueTuple`4",       struct_tuple4_tcr, decodeTupleTy tupInfoStruct
              "ValueTuple`5",       struct_tuple5_tcr, decodeTupleTy tupInfoStruct
              "ValueTuple`6",       struct_tuple6_tcr, decodeTupleTy tupInfoStruct
              "ValueTuple`7",       struct_tuple7_tcr, decodeTupleTy tupInfoStruct
              "ValueTuple`8",       struct_tuple8_tcr, decodeTupleTy tupInfoStruct] 

        let entries = (entries1 @ entries2)
        
        if compilingFslib then 
            // This map is for use when building FSharp.Core.dll. The backing Tycon's may not yet exist for
            // the TyconRef's we have in our hands, hence we can't dereference them to find their stamps.

            // So this dictionary is indexed by names.
            let dict = 
                entries 
                |> List.map (fun (nm,tcref,builder) -> nm, (fun tcref2 tinst -> if tyconRefEq tcref tcref2 then Some(builder tinst) else None)) 
                |> Dictionary.ofList  
            (fun tcref tinst -> 
                 if dict.ContainsKey tcref.LogicalName then dict.[tcref.LogicalName] tcref tinst
                 else None )  
        else
            // This map is for use in normal times (not building FSharp.Core.dll). It is indexed by tcref stamp which is 
            // faster than the indexing technique used in the case above.
            //
            // So this dictionary is indexed by integers.
            let dict = 
                entries  
                |> List.filter (fun (_,tcref,_) -> tcref.CanDeref) 
                |> List.map (fun (_,tcref,builder) -> tcref.Stamp, builder) 
                |> Dictionary.ofList 
            (fun tcref2 tinst -> 
                 if dict.ContainsKey tcref2.Stamp then Some(dict.[tcref2.Stamp] tinst)
                 else None)  
       end
           
    new_decimal_info = new_decimal_info
    seq_info    = seq_info
    seq_vref    = (ValRefForIntrinsic seq_info) 
    and_vref    = (ValRefForIntrinsic and_info) 
    and2_vref   = (ValRefForIntrinsic and2_info)
    addrof_vref = (ValRefForIntrinsic addrof_info)
    addrof2_vref = (ValRefForIntrinsic addrof2_info)
    or_vref     = (ValRefForIntrinsic or_info)
    //splice_vref     = (ValRefForIntrinsic splice_info)
    splice_expr_vref     = (ValRefForIntrinsic splice_expr_info)
    splice_raw_expr_vref     = (ValRefForIntrinsic splice_raw_expr_info)
    or2_vref    = (ValRefForIntrinsic or2_info) 
    generic_equality_er_inner_vref     = ValRefForIntrinsic generic_equality_er_inner_info
    generic_equality_per_inner_vref = ValRefForIntrinsic generic_equality_per_inner_info
    generic_equality_withc_inner_vref  = ValRefForIntrinsic generic_equality_withc_inner_info
    generic_comparison_inner_vref    = ValRefForIntrinsic generic_comparison_inner_info
    generic_comparison_withc_inner_vref    = ValRefForIntrinsic generic_comparison_withc_inner_info
    generic_comparison_withc_outer_info    = generic_comparison_withc_outer_info
    generic_equality_er_outer_info     = generic_equality_er_outer_info
    generic_equality_withc_outer_info  = generic_equality_withc_outer_info
    generic_hash_withc_outer_info = generic_hash_withc_outer_info
    generic_hash_inner_vref = ValRefForIntrinsic generic_hash_inner_info
    generic_hash_withc_inner_vref = ValRefForIntrinsic generic_hash_withc_inner_info

    reference_equality_inner_vref         = ValRefForIntrinsic reference_equality_inner_info

    bitwise_or_vref            = ValRefForIntrinsic bitwise_or_info
    bitwise_and_vref           = ValRefForIntrinsic bitwise_and_info
    bitwise_xor_vref           = ValRefForIntrinsic bitwise_xor_info
    bitwise_unary_not_vref     = ValRefForIntrinsic bitwise_unary_not_info
    bitwise_shift_left_vref    = ValRefForIntrinsic bitwise_shift_left_info
    bitwise_shift_right_vref   = ValRefForIntrinsic bitwise_shift_right_info
    unchecked_addition_vref    = ValRefForIntrinsic unchecked_addition_info
    unchecked_unary_plus_vref  = ValRefForIntrinsic unchecked_unary_plus_info
    unchecked_unary_minus_vref = ValRefForIntrinsic unchecked_unary_minus_info
    unchecked_unary_not_vref = ValRefForIntrinsic unchecked_unary_not_info
    unchecked_subtraction_vref = ValRefForIntrinsic unchecked_subtraction_info
    unchecked_multiply_vref    = ValRefForIntrinsic unchecked_multiply_info
    unchecked_defaultof_vref    = ValRefForIntrinsic unchecked_defaultof_info
    unchecked_subtraction_info = unchecked_subtraction_info
    compare_operator_vref    = ValRefForIntrinsic compare_operator_info
    equals_operator_vref    = ValRefForIntrinsic equals_operator_info
    equals_nullable_operator_vref    = ValRefForIntrinsic equals_nullable_operator_info
    nullable_equals_nullable_operator_vref    = ValRefForIntrinsic nullable_equals_nullable_operator_info
    nullable_equals_operator_vref    = ValRefForIntrinsic nullable_equals_operator_info
    not_equals_operator_vref    = ValRefForIntrinsic not_equals_operator_info
    less_than_operator_vref    = ValRefForIntrinsic less_than_operator_info
    less_than_or_equals_operator_vref    = ValRefForIntrinsic less_than_or_equals_operator_info
    greater_than_operator_vref    = ValRefForIntrinsic greater_than_operator_info
    greater_than_or_equals_operator_vref    = ValRefForIntrinsic greater_than_or_equals_operator_info

    equals_operator_info     = equals_operator_info

    raise_info                 = raise_info
    raise_vref                 = ValRefForIntrinsic raise_info
    failwith_info              = failwith_info
    failwith_vref              = ValRefForIntrinsic failwith_info
    invalid_arg_info           = invalid_arg_info
    invalid_arg_vref           = ValRefForIntrinsic invalid_arg_info
    null_arg_info              = null_arg_info
    null_arg_vref              = ValRefForIntrinsic null_arg_info
    invalid_op_info            = invalid_op_info
    invalid_op_vref            = ValRefForIntrinsic invalid_op_info
    failwithf_info             = failwithf_info
    failwithf_vref             = ValRefForIntrinsic failwithf_info

    reraise_info               = reraise_info
    reraise_vref               = ValRefForIntrinsic reraise_info
    methodhandleof_info        = methodhandleof_info
    methodhandleof_vref        = ValRefForIntrinsic methodhandleof_info
    typeof_info                = typeof_info
    typeof_vref                = ValRefForIntrinsic typeof_info
    sizeof_vref                = ValRefForIntrinsic sizeof_info
    typedefof_info             = typedefof_info
    typedefof_vref             = ValRefForIntrinsic typedefof_info
    enum_vref                  = ValRefForIntrinsic enum_info
    enumOfValue_vref           = ValRefForIntrinsic enumOfValue_info
    range_op_vref              = ValRefForIntrinsic range_op_info
    range_step_op_vref         = ValRefForIntrinsic range_step_op_info
    range_int32_op_vref        = ValRefForIntrinsic range_int32_op_info
    array_length_info          = array_length_info
    array_get_vref             = ValRefForIntrinsic array_get_info
    array2D_get_vref           = ValRefForIntrinsic array2D_get_info
    array3D_get_vref           = ValRefForIntrinsic array3D_get_info
    array4D_get_vref           = ValRefForIntrinsic array4D_get_info
    seq_singleton_vref         = ValRefForIntrinsic seq_singleton_info
    seq_collect_vref           = ValRefForIntrinsic seq_collect_info
    seq_collect_info           = seq_collect_info
    seq_using_info             = seq_using_info
    seq_using_vref             = ValRefForIntrinsic seq_using_info
    seq_delay_info             = seq_delay_info
    seq_delay_vref             = ValRefForIntrinsic  seq_delay_info
    seq_append_info            = seq_append_info
    seq_append_vref            = ValRefForIntrinsic  seq_append_info
    seq_generated_info         = seq_generated_info
    seq_generated_vref         = ValRefForIntrinsic  seq_generated_info
    seq_finally_info           = seq_finally_info
    seq_finally_vref           = ValRefForIntrinsic  seq_finally_info
    seq_of_functions_info      = seq_of_functions_info
    seq_of_functions_vref      = ValRefForIntrinsic  seq_of_functions_info
    seq_map_info               = seq_map_info
    seq_map_vref               = ValRefForIntrinsic  seq_map_info
    seq_singleton_info         = seq_singleton_info
    seq_empty_info             = seq_empty_info
    seq_empty_vref             = ValRefForIntrinsic  seq_empty_info
    new_format_info            = new_format_info
    new_format_vref            = ValRefForIntrinsic new_format_info
    sprintf_vref               = ValRefForIntrinsic sprintf_info
    unbox_vref                 = ValRefForIntrinsic unbox_info
    unbox_fast_vref            = ValRefForIntrinsic unbox_fast_info
    istype_vref                = ValRefForIntrinsic istype_info
    istype_fast_vref           = ValRefForIntrinsic istype_fast_info
    unbox_info                 = unbox_info
    get_generic_comparer_info                 = get_generic_comparer_info
    get_generic_er_equality_comparer_info        = get_generic_er_equality_comparer_info
    get_generic_per_equality_comparer_info    = get_generic_per_equality_comparer_info
    dispose_info               = dispose_info
    getstring_info             = getstring_info
    unbox_fast_info            = unbox_fast_info
    istype_info                = istype_info
    istype_fast_info           = istype_fast_info
    lazy_force_info            = lazy_force_info
    lazy_create_info           = lazy_create_info
    create_instance_info       = create_instance_info
    create_event_info          = create_event_info
    seq_to_list_info           = seq_to_list_info
    seq_to_array_info          = seq_to_array_info
    array_get_info             = array_get_info
    array2D_get_info             = array2D_get_info
    array3D_get_info             = array3D_get_info
    array4D_get_info             = array4D_get_info
    deserialize_quoted_FSharp_20_plus_info       = deserialize_quoted_FSharp_20_plus_info
    deserialize_quoted_FSharp_40_plus_info    = deserialize_quoted_FSharp_40_plus_info
    cast_quotation_info        = cast_quotation_info
    lift_value_info            = lift_value_info
    lift_value_with_name_info            = lift_value_with_name_info
    lift_value_with_defn_info            = lift_value_with_defn_info
    query_source_as_enum_info            = query_source_as_enum_info
    new_query_source_info            = new_query_source_info
    query_source_vref            = ValRefForIntrinsic query_source_info
    query_value_vref            = ValRefForIntrinsic query_value_info
    query_run_value_vref            = ValRefForIntrinsic query_run_value_info
    query_run_enumerable_vref            = ValRefForIntrinsic query_run_enumerable_info
    query_for_vref            = ValRefForIntrinsic query_for_value_info
    query_yield_vref            = ValRefForIntrinsic query_yield_value_info
    query_yield_from_vref        = ValRefForIntrinsic query_yield_from_value_info
    query_select_vref            = ValRefForIntrinsic query_select_value_info
    query_where_vref            = ValRefForIntrinsic query_where_value_info
    query_zero_vref            = ValRefForIntrinsic query_zero_value_info
    query_builder_tcref            = query_builder_tcref
    fail_init_info             = fail_init_info
    fail_static_init_info           = fail_static_init_info
    check_this_info            = check_this_info
    quote_to_linq_lambda_info        = quote_to_linq_lambda_info


    generic_hash_withc_tuple2_vref = ValRefForIntrinsic generic_hash_withc_tuple2_info
    generic_hash_withc_tuple3_vref = ValRefForIntrinsic generic_hash_withc_tuple3_info
    generic_hash_withc_tuple4_vref = ValRefForIntrinsic generic_hash_withc_tuple4_info
    generic_hash_withc_tuple5_vref = ValRefForIntrinsic generic_hash_withc_tuple5_info
    generic_equals_withc_tuple2_vref = ValRefForIntrinsic generic_equals_withc_tuple2_info
    generic_equals_withc_tuple3_vref = ValRefForIntrinsic generic_equals_withc_tuple3_info
    generic_equals_withc_tuple4_vref = ValRefForIntrinsic generic_equals_withc_tuple4_info
    generic_equals_withc_tuple5_vref = ValRefForIntrinsic generic_equals_withc_tuple5_info
    generic_compare_withc_tuple2_vref = ValRefForIntrinsic generic_compare_withc_tuple2_info
    generic_compare_withc_tuple3_vref = ValRefForIntrinsic generic_compare_withc_tuple3_info
    generic_compare_withc_tuple4_vref = ValRefForIntrinsic generic_compare_withc_tuple4_info
    generic_compare_withc_tuple5_vref = ValRefForIntrinsic generic_compare_withc_tuple5_info
    generic_equality_withc_outer_vref = ValRefForIntrinsic generic_equality_withc_outer_info


    cons_ucref = cons_ucref
    nil_ucref = nil_ucref
    
    suppressed_types = suppressed_types
    isInteractive=isInteractive
    mkSysTyconRef=mkSysTyconRef
   }
     
let public mkMscorlibAttrib g nm = 
      let path, typeName = splitILTypeName nm
      AttribInfo(mkILTyRef (g.ilg.traits.ScopeRef,nm), g.mkSysTyconRef path typeName)


