#indent "off"

open System

  type Bytes = Byte[]

  type Guid = Option<Bytes>

  type Document = 
    { Language: Option<Guid>;
      Vendor: Option<Guid>;
      Type: Option<Guid>;
      File: string; }

  type SourceAnnotation = 
    { Document: Document;
      Line: int;
      Column: int;
      EndLine: int;
      EndColumn: int }

  type AssemblyName = string
  type ModuleName = string
  type Locale = string

  type PublicKeyInfo = 
    | PublicKey of Bytes
    | PublicKeyToken of Bytes

  type VersionInfo = UInt16 * UInt16 * UInt16 * UInt16

  type AssemblyRef = 
    { Name: string;
      Hash: Option<Bytes>;
      PublicKeyInfo: Option<PublicKeyInfo>;
      Version: Option<VersionInfo>; 
      Locale: Option<Locale>; } 

  type ModuleRef = 
    { Name: string;
      NoMetadata: bool;
      Hash: Option<Bytes>; } 

  type ScopeRef = 
    | Local 
    | Module of ModuleRef  
    | Assembly of AssemblyRef 

  type BasicCallconv = 
    | Default
    | Cdecl 
    | Stdcall 
    | Thiscall 
    | Fastcall 
    | Vararg
      
  type HasThis = 
    | Instance           (* accepts an implicit 'this' pointer *)
    | InstanceExplicit  (* any 'this' pointer is made explicit (C++ only) *)
    | Static             (* no 'this' pointer is passed *)

  type Callconv 
    static member InstanceCallconv: Callconv
    static member StaticCallconv: Callconv
    member IsVararg: bool
    member IsStatic: bool
    member HasThis: HasThis
    member BasicCallconv: BasicCallconv


  val is_vararg_callconv: Callconv -> bool
  val is_static_callconv: Callconv -> bool

  type ArrayShape
    member Details: (Int32 option * Int32 option) list 
    member Rank: Int32
    static member SingleDimensional: ArrayShape

  // various constructors here
  val ArrayShape: (Int32 option * Int32 option) list -> ArrayShape

  val rank_of_arrayshape: ArrayShape -> Int32
  val sdshape: ArrayShape (* bounds for a single dimensional, zero based array *)



  type TypeRef 
     member TypeName:  string
     member FullTypeName:  string
     member EnclosingTypeNames: List<string>
     member ScopeRef: ScopeRef

  val tname_of_tref: TypeRef -> string
  val nested_tname_of_tref: TypeRef -> string
  val enclosing_tnames_of_tref: TypeRef -> List<string>
  val scoref_of_tref: TypeRef -> ScopeRef

  type TypeSpec 
    member TypeRef: TypeSpec -> TypeRef
    member TypeName: TypeSpec -> string
    member FullTypeName: TypeSpec -> string
    member ScopeRef: TypeSpec -> ScopeRef
    member EnclosingTypeNames: TypeSpec -> List<string>
    
  and Type 
  // member Details: (?k:TypeKind. TypeDetails<TypeKind>)
  // These properties let you access the contents of a type in a
  // semi-safe way - they may throw exceptions.  They could be 
  // given different names.
    member Shape: ArrayShape
    member ElementType: Type
    member TypeSpec: TypeSpec
    member ParameterIndex: UInt16
    member IsRequired: bool
    member CustomModifier: Type
    member ModifiedType: Type
    member Kind: TypeKind
    member Void: Type
    static member Array: ArrayShape * Type -> Type
    static member Value: TypeSpec -> Type
    static member Boxed: TypeSpec -> Type
    static member Ptr: Type -> Type
    static member Byref: Type -> Type
    static member Fptr: Callsig -> Type
    static member Tyvar: Callsig -> Type
    static member Modified: bool * Type * Type -> Type

  and TypeKind = 
    | Void     
    | Array 
    | Value 
    | Boxed 
    | Ptr 
    | Byref
    | Fptr 
    | Tyvar
    | Modified

  // These let you access the contents of a type in a
  // safe way - you may only access the given contents when the
  // given constraints hold.  You must unpack the existential result
  // returned by Details.
(*
  type TypeDetails<k:TypeKind> =
    member Shape: ArrayShape when k = Array
    member ElementType: Type when k = Array or K = Ptr or k = Byref
    member TypeSpec: TypeSpec when k = Value or k = Boxed
    member Callsig: Callsig when k = Fptr
    member ParameterIndex: UInt16 when k = Tyvar
    member IsRequired: bool when k = Modified
    member CustomModifier: Type when k = Modified
    member ModifiedType: Type when k = Modified
*)

  and Instantiation = List<Type>

  and GenericParams = GenericParam list
  and GenericParam
   member Name: string
   member Constraints: List<Type>

  val tref_of_tspec: TypeSpec -> TypeRef
  val inst_of_tspec: TypeSpec -> Instantiation
  val tname_of_tspec: TypeSpec -> string
  val scoref_of_tspec: TypeSpec -> ScopeRef
  val enclosing_tnames_of_tspec: TypeSpec -> List<string>


  type boxity = AsObject | AsValue

  val is_array_ty: Type -> bool
  val dest_array_ty: Type -> ArrayShape * Type

  val tspec_of_typ: Type -> TypeSpec
  val boxity_of_typ: Type -> boxity
  val tref_of_typ: Type -> TypeRef
  val is_tref_typ: Type -> bool
  val inst_of_typ: Type -> Instantiation
  val is_tyvar_ty: Type -> bool


  type CallSig =  
    { Callconv: Callconv;
      ArgTypes: List<Type>;
      ReturnType: Type }

  val Callconv_of_CallSig: CallSig -> Callconv
  val args_of_CallSig: CallSig -> List<Type>
  val ret_of_CallSig: CallSig -> Type


// F# library signature:

  //-------------------------------------------------------------------------
  // This is the primitive interface for implementing events corresponding to 
  // any delegate type.
  //
  // Note 'a and 'args must correspond - if they do not runtime exceptions will arise.
  // We could add a constraint of the form 
  //    when 'a :> delegate(object * 'arg -> void)
  type EventForDelegateType<'a> =
    { Add: 'a -> unit;
      Remove: 'a -> unit }

  type ListenerListForDelegateType<'a,'arg> =
    { Fire: 'arg -> unit;
      AsEvent: EventForDelegateType<'a> }
 
  val ListenerListForDelegateType: unit -> ListenerListForDelegateType<'a,'arg>  when 'a :> System.Delegate

  // This is for the common case for F# where the event type is EventHandler<T>
  type Event<'a> = EventForDelegateType< EventHandler<'a> >

  type ListenerList<'a> =
    { Fire: 'a -> unit;
      AsEvent: Event<'a> }

  val NewListenerList: unit -> ListenerList<'a>  
  
// A component signature:

  type NoiseLevel = Double
  
  type MyComponent 
    member OnNoise: Event<NoiseLevel>
    member OnPaint: EventForDelegateType<PaintEventHandler>
    
  val NewMyComponent: unit -> MyComponent
  
    

  type MethodRef 
    member Name: string
    member Callconv: Callconv
    member Return: Type
    member ArgTypes: List<Type>
    member Parent: TypeRef 
    member GenericArity: int
    member CallSig: CallSig
    member Rename: MethodRef -> MethodRef
    member Relocate: Type -> MethodRef

(* Functional View *)
  val name_of_mref: MethodRef -> string
  val callconv_of_mref: MethodRef -> Callconv
  val ret_of_mref: MethodRef -> Type
  val args_of_mref: MethodRef -> List<Type>
  val tref_of_mref: MethodRef -> TypeRef 
  val parent_of_mref: MethodRef -> TypeRef (* same as tref_of_mref *)
  val genarity_of_mref: MethodRef -> int
  val callsig_of_mref: MethodRef -> CallSig
  val rename_mref: string -> MethodRef -> MethodRef
  val relocate_mref: Type -> MethodRef -> MethodRef

  type FieldRef 
    member Type: FieldRef
    member Name: string
    member TypeRef: TypeRef

  val typ_of_fref: FieldRef -> Type
  val name_of_fref: FieldRef -> string
  val tref_of_fref: FieldRef -> TypeRef


  type MethodSpec
    member EnclosingType: Type
    member MethodInstantiation: Instantiation
    member Callconv: Callconv
    member Name: string
    member FormalMethodRef: MethodRef
    member FormalReturnType: Type
    member FormalArgTypes: List<Type>
    member FormalParent: TypeRef
    member FormalCallsig: CallSig
    member ActualReturnType: Type
    member ActualArgTypes: List<Type>
    member DeclaringEntity: TypeRef
    member ActualCallsig: CallSig
    member GenericArity: int

  val enclosing_typ_of_mspec: MethodSpec -> Type
  val minst_of_mspec: MethodSpec -> Instantiation
  val callconv_of_mspec: MethodSpec -> Callconv
  val name_of_mspec: MethodSpec -> string

  val formal_mref_of_mspec: MethodSpec -> MethodRef
  val formal_ret_of_mspec: MethodSpec -> Type
  val formal_args_of_mspec: MethodSpec -> List<Type>
  val formal_parent_of_mspec: MethodSpec -> TypeRef
  val formal_CallSig_of_mspec: MethodSpec -> CallSig

  val active_inst_of_mspec: MethodSpec -> Instantiation
  val actual_ret_of_mspec: MethodSpec -> Type
  val actual_args_of_mspec: MethodSpec -> List<Type>
  val actual_CallSig_of_mspec: MethodSpec -> CallSig

  val genarity_of_mspec: MethodSpec -> int


  type FieldSpec
    member FieldRef: FieldRef
    member EnclosingType: Type
    member FormalType: Type
    member TypeInstantiation: Instantiation
    member ActualType: Type
    member Name: string
    member FormalParent: TypeRef


  val fref_of_fspec: FieldSpec -> FieldRef
  val enclosing_typ_of_fspec: FieldSpec -> Type
  val formal_typ_of_fspec: FieldSpec -> Type
  val active_inst_of_fspec: FieldSpec -> Instantiation
  val actual_typ_of_fspec: FieldSpec -> Type
  val name_of_fspec: FieldSpec -> string
  val tref_of_fspec: FieldSpec -> TypeRef

  type CodeLabel = string


  type BasicType = 
  | DT_R
  | DT_I1
  | DT_U1
  | DT_I2
  | DT_U2
  | DT_I4
  | DT_U4
  | DT_I8
  | DT_U8
  | DT_R4
  | DT_R8
  | DT_I
  | DT_U
  | DT_REF

  type LdtokenInfo = 
  | Token_type of Type 
  | Token_method of MethodSpec 
  | Token_field of FieldSpec

  type LdcInfo = 
  | NUM_I4 of Int32
  | NUM_I8 of Int64
  | NUM_R4 of Single
  | NUM_R8 of Double

  type Tailness = 
  | Tailcall
  | Normalcall

  type Alignment = 
  | Aligned
  | Unaligned_1
  | Unaligned_2
  | Unaligned_4

  type Volatility = 
  | Volatile
  | Nonvolatile

  type Readonly = 
  | ReadonlyAddress
  | NormalAddress

  type VarargTypes = Option< List<Type> >

  type CompareOp =
  | BI_beq        
  | BI_bge        
  | BI_bge_un     
  | BI_bgt        
  | BI_bgt_un        
  | BI_ble        
  | BI_ble_un        
  | BI_blt        
  | BI_blt_un 
  | BI_bne_un 
  | BI_brfalse 
  | BI_brtrue 

  type ArithmeticOp =
  | AI_add    
  | AI_add_ovf
  | AI_add_ovf_un
  | AI_and    
  | AI_div   
  | AI_div_un
  | AI_ceq      
  | AI_cgt      
  | AI_cgt_un   
  | AI_clt     
  | AI_clt_un  
  | AI_conv      of BasicType
  | AI_conv_ovf  of BasicType
  | AI_conv_ovf_un  of BasicType
  | AI_mul       
  | AI_mul_ovf   
  | AI_mul_ovf_un
  | AI_rem       
  | AI_rem_un       
  | AI_shl       
  | AI_shr       
  | AI_shr_un
  | AI_sub       
  | AI_sub_ovf   
  | AI_sub_ovf_un   
  | AI_xor       
  | AI_or        
  | AI_neg       
  | AI_not       
  | AI_ldnull    
  | AI_dup       
  | AI_pop
  | AI_ckfinite 
  | AI_nop
  | AI_ldc       of BasicType * LdcInfo

  // A discriminated union automatically defines a kind and a "details"?

  type Instr = 
  (* Basic *)
  | I_arith of ArithmeticOp
  | I_ldarg     of UInt16
  | I_ldarga    of UInt16
  | I_ldind     of Alignment * Volatility * BasicType
  | I_ldloc     of UInt16
  | I_ldloca    of UInt16
  | I_starg     of UInt16
  | I_stind     of  Alignment * Volatility * BasicType
  | I_stloc     of UInt16

  (* Control transfer *)
  | I_br    of  CodeLabel
  | I_jmp   of MethodSpec
  | I_brcmp of CompareOp * CodeLabel * CodeLabel (* second label is fall-through *)
  | I_switch    of (CodeLabel list * CodeLabel) (* last label is fallthrough *)
  | I_ret 

   (* Method call *)
  | I_call     of Tailness * MethodSpec * VarargTypes
  | I_callvirt of Tailness * MethodSpec * VarargTypes
  | I_callconstraint of Tailness * Type * MethodSpec * VarargTypes
  | I_calli    of Tailness * CallSig * VarargTypes
  | I_ldftn    of MethodSpec
  | I_newobj  of MethodSpec  * VarargTypes
  
  (* Exceptions *)
  | I_throw
  | I_endfinally
  | I_endfilter
  | I_leave     of  CodeLabel

  (* Object instructions *)
  | I_ldsfld      of Volatility * FieldSpec
  | I_ldfld       of Alignment * Volatility * FieldSpec
  | I_ldsflda     of FieldSpec
  | I_ldflda      of FieldSpec 
  | I_stsfld      of Volatility  *  FieldSpec
  | I_stfld       of Alignment * Volatility * FieldSpec
  | I_ldstr       of Bytes  (* Beware!  This is a unicode encoding of the string! *)
  | I_isinst      of Type
  | I_castclass   of Type
  | I_ldtoken     of LdtokenInfo
  | I_ldvirtftn   of MethodSpec

  (* Value type instructions *)
  | I_cpobj       of Type
  | I_initobj     of Type
  | I_ldobj       of Alignment * Volatility * Type
  | I_stobj       of Alignment * Volatility * Type
  | I_box         of Type
  | I_unbox       of Type
  | I_unbox_any   of Type
  | I_sizeof      of Type

  | I_ldelem      of BasicType
  | I_stelem      of BasicType
  | I_ldelema     of Readonly * ArrayShape * Type (* ArrayShape = sdshape for single dimensional arrays *)
  | I_ldelem_any  of ArrayShape * Type (* ArrayShape = sdshape for single dimensional arrays *)
  | I_stelem_any  of ArrayShape * Type (* ArrayShape = sdshape for single dimensional arrays *)
  | I_newarr      of ArrayShape * Type (* ArrayShape = sdshape for single dimensional arrays *)
  | I_ldlen

  | I_mkrefany    of Type
  | I_refanytype  
  | I_refanyval   of Type
  | I_rethrow

  | I_break 
  | I_seqpoint of SourceAnnotation 

  | I_arglist  

  | I_localloc
  | I_cpblk of Alignment * Volatility
  | I_initblk of Alignment  * Volatility


type basic_block = 
    { bblockLabel: CodeLabel;
      bblockInstrs: Instr array }

val label_of_bblock: basic_block -> CodeLabel
val instrs_of_bblock: basic_block -> Instr array
val destinations_of_bblock: basic_block -> List<CodeLabel>
val fallthrough_of_bblock: basic_block -> CodeLabel option
val last_of_bblock: basic_block -> Instr

val instr_is_bblock_end: Instr -> bool
val instr_is_tailcall: Instr -> bool



type local_debug_info = 
    { localNum: int;
      localName: string; }

type Code = 
    | BasicBlock of basic_block
    | GroupBlock of local_debug_info list * Code list
    | RestrictBlock of List<CodeLabel> * Code
    | TryBlock of Code * seh

  member Name: string
  member X : int
  
and seh =
  | FaultBlock of Code 
  | FinallyBlock of Code
  | FilterCatchBlock of (filter * Code) list
and filter = 
  | TypeFilter of Type
  | CodeFilter of Code

val entries_of_code: Code -> List<CodeLabel>
val exits_of_code: Code -> List<CodeLabel>
val unique_entry_of_code: Code -> CodeLabel
val unique_exit_of_code: Code -> CodeLabel

(* -------------------------------------------------------------------- 
 * Field Init
 * -------------------------------------------------------------------- *)

type FieldInit = 
  | FieldInit_Bytes of Bytes
  | FieldInit_bool of bool
  | FieldInit_char of UInt16
  | FieldInit_Int8 of sbyte
  | FieldInit_Int16 of Int16
  | FieldInit_Int32 of Int32
  | FieldInit_Int64 of Int64
  | FieldInit_UInt8 of byte
  | FieldInit_UInt16 of UInt16
  | FieldInit_UInt32 of UInt32
  | FieldInit_UInt64 of UInt64
  | FieldInit_Single of Single
  | FieldInit_Double of Double
  | FieldInit_ref

(* -------------------------------------------------------------------- 
 * Native Types, for marshalling to the native C interface.
 * These are taken directly from the ILASM syntax, and don't really
 * correspond yet to the ECMA Spec (Partition II, 7.4).  
 * -------------------------------------------------------------------- *)

type native_type =
  | NativeType_empty
  | NativeType_custom of Bytes * string * string * Bytes (* guid,nativeTypeName,custMarshallerName,cookieString *)
  | NativeType_fixed_sysstring of Int32
  | NativeType_fixed_array of Int32
  | NativeType_currency
  | NativeType_lpstr
  | NativeType_lpwstr
  | NativeType_lptstr
  | NativeType_byvalstr
  | NativeType_tbstr
  | NativeType_lpstruct
  | NativeType_struct
  | NativeType_void
  | NativeType_bool
  | NativeType_Int8
  | NativeType_Int16
  | NativeType_Int32
  | NativeType_Int64
  | NativeType_Single
  | NativeType_Double
  | NativeType_unsigned_Int8
  | NativeType_unsigned_Int16
  | NativeType_unsigned_Int32
  | NativeType_unsigned_Int64
  | NativeType_array of native_type option * (Int32 * Int32 option) option (* optional idx of parameter giving size plus optional additive i.e. num elems *)
  | NativeType_int
  | NativeType_unsigned_int
  | NativeType_method
  | NativeType_as_any
  | (* COM interop *) NativeType_bstr
  | (* COM interop *) NativeType_iunknown
  | (* COM interop *) NativeType_idispatch
  | (* COM interop *) NativeType_interface
  | (* COM interop *) NativeType_error               
  | (* COM interop *) NativeType_safe_array of variant_type * string option 
  | (* COM interop *) NativeType_ansi_bstr
  | (* COM interop *) NativeType_variant_bool

and variant_type = 
  | VariantType_empty
  | VariantType_null
  | VariantType_variant
  | VariantType_currency
  | VariantType_decimal               
  | VariantType_date               
  | VariantType_bstr               
  | VariantType_lpstr               
  | VariantType_lpwstr               
  | VariantType_iunknown               
  | VariantType_idispatch               
  | VariantType_safearray               
  | VariantType_error               
  | VariantType_hresult               
  | VariantType_carray               
  | VariantType_userdefined               
  | VariantType_record               
  | VariantType_filetime
  | VariantType_blob               
  | VariantType_stream               
  | VariantType_storage               
  | VariantType_streamed_object               
  | VariantType_stored_object               
  | VariantType_blob_object               
  | VariantType_cf                
  | VariantType_clsid
  | VariantType_void 
  | VariantType_bool
  | VariantType_Int8
  | VariantType_Int16                
  | VariantType_Int32                
  | VariantType_Int64                
  | VariantType_Single                
  | VariantType_Double                
  | VariantType_unsigned_Int8                
  | VariantType_unsigned_Int16                
  | VariantType_unsigned_Int32                
  | VariantType_unsigned_Int64                
  | VariantType_ptr                
  | VariantType_array of variant_type                
  | VariantType_vector of variant_type                
  | VariantType_byref of variant_type                
  | VariantType_int                
  | VariantType_unsigned_int                


(* -------------------------------------------------------------------- 
 * Local variables
 * -------------------------------------------------------------------- *)

type local = 
    { localType: Type;
      localPinned: bool  }
    :> Object 
    :> IComparable


val typ_of_local: local -> Type

(* -------------------------------------------------------------------- 
 * IL method bodies
 * -------------------------------------------------------------------- *)

type CILMethodBody = 
    { ilZeroInit: bool;
      ilMaxStack: Int32; (* strictly speaking should be a UInt16 *)
      ilNoInlining: bool;
      ilLocals: local list;
      ilCode: Code;
      ilSource: Option<SourceAnnotation> }

val locals_of_ilmbody: CILMethodBody -> local list
val code_of_ilmbody: CILMethodBody -> Code

(* -------------------------------------------------------------------- 
 * Member Access
 * -------------------------------------------------------------------- *)

type MemberAccess = 
  | MemAccess_assembly
  | MemAccess_compilercontrolled
  | MemAccess_famandassem
  | MemAccess_famorassem
  | MemAccess_family
  | MemAccess_private 
  | MemAccess_public 

(* --------------------------------------------------------------------
 * Custom attributes: @todo: provide a helper to parse the Bytes 
 * to CustomAttribute_elem's as best as possible.  
 * -------------------------------------------------------------------- *)

type CustomAttributeElement = 
  | String of String  
  | Bool of Boolean
  | Char of Char
  | Int8 of SByte
  | Int16 of Int16
  | Int32 of Int32
  | Int64 of Int64
  | UInt8 of byte
  | UInt16 of UInt16
  | UInt32 of UInt32
  | UInt64 of UInt64
  | Single of Single
  | Double of Double
  | Type of TypeRef  
  | Enum of Type * CustomAttributeElement

type CustomAttribute =
    { customMethod: MethodSpec;  
      customData: Bytes }

type CustomAttributes (* Equivalent to CustomAttribute list - use helpers below to construct/destruct these *)

val dest_CustomAttributes: CustomAttributes -> CustomAttribute list

(* -------------------------------------------------------------------- 
 * Method parameters and return values
 * -------------------------------------------------------------------- *)

type param = 
    { Name: string option;
      Type: Type;
      Default: Option<FieldInit>;  
      Marshal: native_type option; (* Marshalling map for parameters. COM Interop only. *)
      In: bool;
      Out: bool;
      Optional: bool;
      CustomAttrs: CustomAttributes }

val name_of_param: param -> string option
val typ_of_param: param -> Type

(* -------------------------------------------------------------------- 
 * Method return values
 * -------------------------------------------------------------------- *)

type ReturnSpec = 
    { returnMarshal: native_type option;
      returnType: Type; }

val typ_of_return: ReturnSpec -> Type

(* -------------------------------------------------------------------- 
 * Security Permissions
 * 
 * Attached to various structures...
 * -------------------------------------------------------------------- *)

type SecurityAction = 
  | Request 
  | Demand
  | Assert
  | Deny
  | Permitonly
  | Linkcheck 
  | Inheritcheck
  | Reqmin
  | Reqopt
  | Reqrefuse
  | Prejitgrant
  | Prejitdeny
  | Noncasdemand
  | Noncaslinkdemand
  | Noncasinheritance

type PermissionValue = 
    Bool of bool
  | Int32 of Int32
  | String of string
  | EnumInt8 of TypeRef * sbyte
  | EnumInt16 of TypeRef * Int16
  | EnumInt32 of TypeRef * Int32

type permission = 
  | Permission of SecurityAction * Type * (string * PermissionValue) list
  | PermissionSet of SecurityAction * Bytes

type SecurityDecls (* Opaque type equivalent to permission list - use helpers below to construct/destruct these *)

val dest_SecurityDecls: SecurityDecls -> permission list

(* -------------------------------------------------------------------- 
 * PInvoke attributes.
 * -------------------------------------------------------------------- *)

type pinvoke_Callconv = 
  | PInvoke_CC_none
  | PInvoke_CC_cdecl
  | PInvoke_CC_stdcall
  | PInvoke_CC_thiscall
  | PInvoke_CC_fastcall
  | PInvoke_CC_winapi

type pinvoke_encoding = 
  | PInvoke_Encoding_none
  | PInvoke_Encoding_ansi
  | PInvoke_Encoding_unicode
  | PInvoke_Encoding_autochar

type pinvoke_attr =  
    { pinvokeWhere: ModuleRef;
      pinvokeName: string;
      pinvokeCallconv: pinvoke_Callconv;
      pinvokeEncoding: pinvoke_encoding;
      pinvokeNoMangle: bool;
      pinvokeLastErr: bool }


type OverridesSpec = OverridesSpec of MethodRef * Type

val enclosing_typ_of_ospec: OverridesSpec -> Type
val formal_mref_of_ospec: OverridesSpec -> MethodRef
val Callconv_of_ospec: OverridesSpec -> Callconv
val formal_ret_of_ospec: OverridesSpec -> Type
val formal_args_of_ospec: OverridesSpec -> List<Type>
val tref_of_ospec: OverridesSpec -> TypeRef
val name_of_ospec: OverridesSpec -> string
val formal_CallSig_of_ospec: OverridesSpec -> CallSig
val actual_CallSig_of_ospec: OverridesSpec -> CallSig


type virtual_info = 
    { Final: bool; 
      Newslot: bool; 
      Abstract: bool;
      Overrides: OverridesSpec option; }

type method_kind = 
  | Static 
  | Cctor 
  | Ctor 
  | Nonvirtual 
  | Virtual of virtual_info

type MethodBody_details = 
  | MethodBody_il of CILMethodBody
  | MethodBody_pinvoke of pinvoke_attr       (* platform invoke to native  *)
  | MethodBody_abstract
  | MethodBody_native

type method_code_kind =
  | MethodCodeKind_il
  | MethodCodeKind_native
  | MethodCodeKind_runtime

type MethodBody (* isomorphic to MethodBody_details *)

val dest_mbody : MethodBody -> MethodBody_details 

type MethodDef = 
    { mdName: string;
      mdKind: method_kind;
      mdCallconv: Callconv;
      mdParams: List<Param>;
      mdReturn: ReturnSpec;
      mdAccess: MemberAccess;
      mdBody: MethodBody;   
      mdCodeKind: method_code_kind;   
      mdInternalCall: bool;
      mdManaged: bool;
      mdForwardRef: bool;
      mdSecurityDecls: SecurityDecls;
      mdEntrypoint:bool;
      mdReqSecObj: bool;
      mdHideBySig: bool;
      mdSpecialName: bool;
      mdUnmanagedExport: bool; (* -- The method is exported to unmanaged code using COM interop. *)
      mdSynchronized: bool;
      mdPreserveSig: bool;
      mdMustRun: bool; (* Whidbey feature: SafeHandle finalizer must be run *)
      mdExport: (Int32 * string option) option; 
      mdVtableEntry: (Int32 * Int32) option;
     
      (* MS-GENERICS *) mdGenericParams: GenericParams;
      mdCustomAttrs: CustomAttributes; }

val name_of_mdef: MethodDef -> string
val num_args: MethodDef -> int
val mdef_is_virt: MethodDef -> bool
val mdef_is_il: MethodDef -> bool
val params_of_mdef: MethodDef -> List<Param>
val Callconv_of_mdef: MethodDef -> Callconv
val ilmbody_of_mdef: MethodDef -> CILMethodBody
val code_of_mdef: MethodDef -> Code
val entry_of_mdef: MethodDef -> CodeLabel
val CallSig_of_mdef: MethodDef -> CallSig

(* -------------------------------------------------------------------- 
 * Delegates.  Derived functions for telling if a method/class definition 
 * is really a delegate.  Also for telling if method signatures refer to 
 * delegate methods.
 * -------------------------------------------------------------------- *)

(* these are approximations: you have to check that the type def is actually a delegate as well *)
val is_delegate_ctor: MethodDef -> bool
val is_delegate_invoke: MethodDef -> bool
val dest_delegate_invoke: MethodDef -> List<Type> * Type
val is_delegate_begin_invoke: MethodDef -> bool
val is_delegate_end_invoke: MethodDef -> bool
val dest_delegate_begin_end_invoke: MethodDef -> MethodDef -> List<Type> * Type

(* -------------------------------------------------------------------- 
 * Tables of methods.  Logically equivalent to a list of methods but
 * the table is kept in a form optimized for looking up methods by 
 * name and arity.
 * -------------------------------------------------------------------- *)

type MethodDefs (* abstract type equivalent to [MethodDef list] *)

val dest_mdefs: MethodDefs -> MethodDef list
val filter_mdefs: (MethodDef -> bool) -> MethodDefs -> MethodDefs
val find_mdefs_by_arity: string * int -> MethodDefs -> MethodDef list

(* -------------------------------------------------------------------- 
 * Field definitions
 * -------------------------------------------------------------------- *)

type FieldDef = 
    { Name: string;
      Type: Type;
      Static: bool;
      Access: MemberAccess;
      Data:  Option<Bytes>;
      Init: Option<FieldInit>;  
      Offset:  Int32 option; (* The explicit offset in Bytes when explicit layout is used. *)
      SpecialName: bool;
      Marshal: native_type option; 
      NotSerialized: bool;
      Literal: bool ;
      InitOnly: bool;
      CustomAttrs: CustomAttributes; }

val typ_of_fdef : FieldDef -> Type
val name_of_fdef: FieldDef -> string

(* -------------------------------------------------------------------- 
 * Tables of FieldDefs.  Logically equivalent to a list of FieldDefs but
 * the table is kept in a form optimized for looking up FieldDefs by 
 * name.
 * -------------------------------------------------------------------- *)

type FieldDefs (* Abstract type equivalent to a list of FieldDefs *)

val dest_fdefs: FieldDefs -> FieldDef list
val filter_fdefs: (FieldDef -> bool) -> FieldDefs -> FieldDefs
val find_fdef: string -> FieldDefs -> FieldDef list

(* -------------------------------------------------------------------- 
 * Event definitions
 * -------------------------------------------------------------------- *)

type EventDef = 
    { eventType: Type option; 
      eventName: string;
      eventRTSpecialName: bool;
      eventSpecialName: bool;
      eventAddOn: MethodRef; 
      eventRemoveOn: MethodRef;
      eventFire: Option<MethodRef>;
      eventOther: MethodRef list;
      eventCustomAttrs: CustomAttributes; }

(* -------------------------------------------------------------------- 
 * Table of those events in a type definition.
 * -------------------------------------------------------------------- *)

type EventDefs

val dest_events: EventDefs -> EventDef list
val filter_events: (EventDef -> bool) -> EventDefs -> EventDefs

(* -------------------------------------------------------------------- 
 * Property definitions
 * -------------------------------------------------------------------- *)

type PropertyDef = 
    { propName: string;
      propRTSpecialName: bool;
      propSpecialName: bool;
      propSet: Option<MethodRef>;
      propGet: Option<MethodRef>;
      propCallconv: HasThis;
      propType: Type;          
      propInit: Option<FieldInit>;
      propArgs: List<Type>;
      propCustomAttrs: CustomAttributes; }

(* -------------------------------------------------------------------- 
 * Table of those properties in a type definition.
 * -------------------------------------------------------------------- *)

type ILPropertyDefs

val dest_properties: properties -> PropertyDef  list
val filter_properties: (PropertyDef -> bool) -> ILPropertyDefs -> ILPropertyDefs

(* ------------------------------------------------------------------ 
 * Method Impls
 * 
 * If there is an entry (pms --> ms) in this table, then method [ms] 
 * is used to implement method [pms] for the purposes of this class 
 * and its subclasses. 
 * ------------------------------------------------------------------ *)

type MethodImpl = 
   { mimplOverrides: OverridesSpec;
     mimplOverrideBy: MethodSpec }

type MethodImpls

val dest_mimpls: MethodImpls -> MethodImpl list

(* ------------------------------------------------------------------ 
 * Type Access, Layout etc.
 * ------------------------------------------------------------------ *)

type TypeLayout =
  | TypeLayout_auto
  | TypeLayout_sequential of TypeLayout_info
  | TypeLayout_explicit of TypeLayout_info 

and TypeLayout_info =
    { typeSize: Int32 option;
      typePack: UInt16 option } 

type TypeInit =
  | TypeInit_beforefield
  | TypeInit_onany

type TypeEncoding =
  | TypeEncoding_ansi
  | TypeEncoding_autochar
  | TypeEncoding_unicode

type TypeDefAccess = 
  | TypeAccess_public 
  | TypeAccess_private
  | TypeAccess_nested of MemberAccess 

type delegate_kind = Multicast | Singlecast
type delegate_info =  
    { delKind: delegate_kind;  
      delAsync: bool;
      delArgs: List<Type>;
      delRet: Type }

type enum_info =  
    { enumValues: (string * FieldInit) list;  
      enumType: Type }

val values_of_enum_info: enum_info -> (string * FieldInit) list
val typ_of_enum_info: enum_info -> Type

type TypeDefKind = 
  | TypeDef_class
  | TypeDef_valuetype
  | TypeDef_interface
  | TypeDef_enum of enum_info
  | TypeDef_delegate of delegate_info


val split_type_name: string -> List<string> * string


type TypeDef =  
    { tdKind: TypeDefKind;
      tdName: string;  
      tdGenericParams: GenericParams;
      tdAccess: TypeDefAccess;  
      tdAbstract: bool;
      tdSealed: bool; 
      tdSerializable: bool; 
      tdComInterop: bool; (* Class or interface generated for COM interop *) 
      tdLayout: TypeLayout;
      tdSpecialName: bool;
      tdEncoding: TypeEncoding;
      tdNested: TypeDefs;
      tdImplements: List<Type>;  
      tdExtends: Type option; 
      tdMethodDefs: MethodDefs;
      tdSecurityDecls: SecurityDecls;
      tdFieldDefs: FieldDefs;
      tdMethodImpls: MethodImpls;
      tdInitSemantics: TypeInit;
      tdEvents: EventDefs;
      tdProperties: ILPropertyDefs;
      tdCustomAttrs: CustomAttributes; }

and TypeDefs (* abstract type equivalent to "TypeDef list" *)

val kind_of_tdef: TypeDef -> TypeDefKind
val name_of_tdef: TypeDef -> string
val gparams_of_tdef: TypeDef -> GenericParams
val FieldDefs_of_tdef: TypeDef -> FieldDefs
val methods_of_tdef: TypeDef -> MethodDefs
val mimpls_of_tdef: TypeDef -> MethodImpls
val properties_of_tdef: TypeDef -> ILPropertyDefs
val events_of_tdef: TypeDef -> EventDefs
val nested_of_tdef: TypeDef -> TypeDefs
val extends_of_tdef: TypeDef -> Type option
val implements_of_tdef: TypeDef -> List<Type>
val access_of_tdef: TypeDef -> TypeAccess
val layout_of_tdef: TypeDef -> TypeLayout
val sealed_of_tdef: TypeDef -> bool
val abstract_of_tdef: TypeDef -> bool
val serializable_of_tdef: TypeDef -> bool
val CustomAttributes_of_tdef: TypeDef -> CustomAttributes
val SecurityDecls_of_tdef: TypeDef -> SecurityDecls
val encoding_of_tdef: TypeDef -> TypeEncoding
val initsemantics_of_tdef: TypeDef -> TypeInit

val is_value_tdef: TypeDef -> bool

val name_of_nested_tdef: TypeDef list * TypeDef -> string
val gparams_of_nested_tdef: TypeDef list * TypeDef -> GenericParams

val iter_tdefs: (TypeDef -> unit) -> TypeDefs -> unit
val dest_tdefs: TypeDefs -> TypeDef  list
val find_tdef: string -> TypeDefs -> TypeDef

val dest_lazy_tdefs: TypeDefs -> (string * TypeDef Lazy.t) list
val iter_lazy_tdefs: (string -> TypeDef Lazy.t -> unit) -> TypeDefs -> unit

val tname_for_toplevel: string
val is_toplevel_tname: string -> bool
val dest_tdefs_with_toplevel_first: TypeDefs -> TypeDef list

(* -------------------------------------------------------------------- 
 * "Classes Elsewhere" - classes in auxillary modules.
 *
 * Manifests include declarations for all the classes in an 
 * assembly, regardless of which module they are in.
 *
 * The ".class extern" construct describes so-called exported Types -- 
 * these are public classes defined in the auxillary modules of this assembly,
 * i.e. modules other than the manifest-carrying module. 
 * 
 * For example, if you have a two-module 
 * assembly (A.DLL and B.DLL), and the manifest resides in the A.DLL, 
 * then in the manifest all the public classes declared in B.DLL should
 * be defined as exported Types, i.e., as ".class extern". The public classes 
 * defined in A.DLL should not be defined as ".class extern" -- they are 
 * already available in the manifest-carrying module. The union of all 
 * public classes defined in the manifest-carrying module and all 
 * exported Types defined there is the set of all classes exposed by 
 * this assembly. Thus, by analysing the metadata of the manifest-carrying 
 * module of an assembly, you can identify all the classes exposed by 
 * this assembly, and where to find them.
 *
 * Nested classes found in external modules should also be located in 
 * this table, suitably nested inside another "class_elsewhere"
 * definition.
 * -------------------------------------------------------------------- *)

(* these are only found in the "Nested" field of class_elsewhere objects *)
type nested_class_elsewhere = 
    { Name: string;
      Access: MemberAccess;
      Nested: nested_classes_elsewhere;
      CustomAttrs: CustomAttributes } 

and nested_classes_elsewhere

(* these are only found in the classes_elsewhere table in the manifest *)
type class_elsewhere = 
    { Module: ModuleRef;
      Name: string;
      Access: TypeDefAccess;
      Nested: nested_classes_elsewhere;
      CustomAttrs: CustomAttributes } 

type classes_elsewhere

val dest_nested_classes_elsewhere: nested_classes_elsewhere -> nested_class_elsewhere  list
val dest_classes_elsewhere: classes_elsewhere -> class_elsewhere list
val find_class_elsewhere: string -> classes_elsewhere -> class_elsewhere

(* -------------------------------------------------------------------- 
 * "Manifest Resources" are chunks of resource data, being one of:
 *   - the data section of the current module (Bytes of resource given directly) 
 *   - in an external file in this assembly (offset given in the resource_where field) 
 *   - as a resources in another assembly of the same name.  
 * -------------------------------------------------------------------- *)

type resource_access = 
  | Public 
  | Private 
type resource_where = 
  | Local of Bytes 
  | File of ModuleRef * Int32
  | Assembly of AssemblyRef

type Resource = 
    { Name: string;
      Where: resource_where;
      Access: resource_access;
      CustomAttrs: CustomAttributes }

(* -------------------------------------------------------------------- 
 * Table of resources in a module
 * -------------------------------------------------------------------- *)

type Resources

val dest_resources: Resources -> Resource  list

(* -------------------------------------------------------------------- 
 * Fixups are pretty obscure stuff for C++ code.  These are not
 * yet correctly represented in the AbstractIL syntax.
 * -------------------------------------------------------------------- *)

(* type fixup = Fixup of (Int32 * List<string> * data_label) *)
(* type fixups = fixup list *)

(* -------------------------------------------------------------------- 
 * Manifests, The "main" module of an assembly, and Assemblies. 
 * 
 * The main module of an assembly is a module plus some manifest information.
 *
 * An assembly is built by joining together a "main" module plus 
 * several auxiliary modules. 
 * -------------------------------------------------------------------- *)

type manifest = 
    { Name: string;
      AuxModuleHashAlgorithm: Int32; 
      SecurityDecls: SecurityDecls;
      PublicKey: Option<Bytes>;  
      Version: VersionInfo option;
      Locale: Locale option;
      CustomAttrs: CustomAttributes;
      EnableJITOptimizations: bool;
      DLL: bool;
      NoAppDomain: bool;
      NoProcess: bool;
      NoMachine: bool;
      ClassesElsewhere: classes_elsewhere;
      EntrypointElsewhere: ModuleRef option;
    } 

(* -------------------------------------------------------------------- 
 * One module in the "current" assembly, either a main-module or
 * an auxiliary module.  The main module will have a manifest.
 *
 * The abbreviation "modul" is used frequently throughout the OCaml source
 * code because "module" is a reserved word in OCaml.
 * -------------------------------------------------------------------- *)

type modul = 
    { Manifest: manifest option;
      CustomAttrs: CustomAttributes;
      Name: string;
      TypeDefs: TypeDefs;
      SubSystem: Int32;
      ILonly: bool;
      32bit: bool;
      VirtAlignment: Int32;
      PhysAlignment: Int32;
      ImageBase: Int32;
      Resources: Resources; 
      NativeResources: Bytes Lazy.t option; (* e.g. win86 resources, as the exact contents of a .res or .obj file *)
    }

val manifest_of_mainmod: modul -> manifest
val module_is_mainmod: modul -> bool
val assemblyName_of_mainmod: modul -> AssemblyName

(* ====================================================================
 * PART 2
 * 
 * Making metadata.  Where no explicit static member 
 * is given, you should create the concrete datatype directly, 
 * e.g. by filling in all appropriate record FieldDefs.
 * ==================================================================== *)

(* -------------------------------------------------------------------- 
 * Generate references to existing assemblies and modules
 * -------------------------------------------------------------------- *)

val mk_simple_assemblyRef: AssemblyName -> AssemblyRef
val mk_simple_modref: ModuleName -> ModuleRef

val mk_simple_scoref_from_assemblyName: AssemblyName -> ScopeRef 
val mk_simple_scoref_from_assemblyRef: AssemblyRef -> ScopeRef 

val assemblyRef_for_manifest: manifest -> AssemblyRef
val assemblyRef_for_mainmod: modul -> AssemblyRef

(* -------------------------------------------------------------------- 
 * Take apart MethodSpecs
 * -------------------------------------------------------------------- *)

val rename_mspec: string -> MethodSpec -> MethodSpec
val relocate_mspec: Type -> MethodSpec -> MethodSpec (* deprecated *)

(* -------------------------------------------------------------------- 
 * Make type refs
 * -------------------------------------------------------------------- *)

val mk_empty_gparams: GenericParams
val mk_empty_gactuals: Instantiation
val mk_tyvar_ty: UInt16 -> Type
val mk_Type: Type -> Type

val mk_nested_tref: ScopeRef * List<string> * string -> TypeRef
val mk_tref: ScopeRef * string -> TypeRef
val mk_tref_in_tref: TypeRef * string -> TypeRef

(* -------------------------------------------------------------------- 
 * Make type specs
 * -------------------------------------------------------------------- *)

val mk_nongeneric_tspec: TypeRef -> TypeSpec
val mk_tspec: TypeRef * Instantiation -> TypeSpec
val mk_Callconv: HasThis -> Callconv

(* -------------------------------------------------------------------- 
 * Make Types
 * -------------------------------------------------------------------- *)

val mk_typ: boxity -> TypeSpec -> Type
val mk_named_typ: boxity -> TypeRef -> Instantiation -> Type
val mk_boxed_typ: TypeRef -> Instantiation -> Type
val mk_value_typ: TypeRef -> Instantiation -> Type
val mk_nongeneric_boxed_typ: TypeRef -> Type
val mk_nongeneric_value_typ: TypeRef -> Type

val mk_array_ty: Type * ArrayShape -> Type
val mk_sdarray_ty: Type -> Type

(* -------------------------------------------------------------------- 
 * Make method references and specs
 * -------------------------------------------------------------------- *)

(* Construct references to any kind of method *)
val mk_mref: TypeRef * Callconv * string * int * List<Type> * Type -> MethodRef
val mk_mspec: MethodRef * boxity * Instantiation * Instantiation -> MethodSpec
val mk_mspec_in_typ: Type * Callconv * string * List<Type> * Type * Instantiation -> MethodSpec

(* Construct references to methods on a given type *)
val mk_nongeneric_mspec_in_typ: Type * Callconv * string * List<Type> * Type -> MethodSpec

(* Construct references to methods given a TypeSpec *)
val mk_mspec_in_tspec: TypeSpec * boxity * Callconv * string * List<Type> * Type * Instantiation -> MethodSpec
val mk_nongeneric_mspec_in_tspec: TypeSpec * boxity * Callconv * string * List<Type> * Type -> MethodSpec

(* Construct references to instance methods *)
val mk_instance_mspec_in_tref: TypeRef * boxity * string * List<Type> * Type * Instantiation * Instantiation -> MethodSpec
val mk_instance_mspec_in_tspec: TypeSpec * boxity * string * List<Type> * Type * Instantiation -> MethodSpec
val mk_instance_mspec_in_boxed_tspec: TypeSpec * string * List<Type> * Type * Instantiation -> MethodSpec

(* Construct references to non-generic methods *)
val mk_nongeneric_mspec_in_tref: TypeRef * boxity * Callconv * string * List<Type> * Type * Instantiation -> MethodSpec
val mk_nongeneric_mspec_in_nongeneric_tref: TypeRef * boxity * Callconv * string * List<Type> * Type -> MethodSpec

(* Construct references to non-generic instance methods *)
val mk_instance_nongeneric_mspec_in_tref: TypeRef * boxity * string * List<Type> * Type * Instantiation -> MethodSpec
val mk_instance_nongeneric_mspec_in_tspec: TypeSpec * boxity * string * List<Type> * Type -> MethodSpec
val mk_instance_nongeneric_mspec_in_boxed_tspec: TypeSpec * string * List<Type> * Type -> MethodSpec
val mk_instance_nongeneric_mspec_in_nongeneric_boxed_tref: TypeRef * string * List<Type> * Type -> MethodSpec

(* Construct references to static methods *)
val mk_static_mspec_in_nongeneric_boxed_tref: TypeRef * string * List<Type> * Type * Instantiation -> MethodSpec
val mk_static_mspec_in_boxed_tspec: TypeSpec * string * List<Type> * Type * Instantiation -> MethodSpec

(* Construct references to static, non-generic methods *)
val mk_static_nongeneric_mspec_in_nongeneric_boxed_tref: TypeRef * string * List<Type> * Type -> MethodSpec
val mk_static_nongeneric_mspec_in_boxed_tspec: TypeSpec * string * List<Type> * Type -> MethodSpec

(* Construct references to toplevel methods in modules.  Usually compiler generated. *)
val mk_toplevel_static_mspec: ScopeRef -> string * List<Type> * Type * Instantiation -> MethodSpec
val mk_toplevel_static_nongeneric_mspec: ScopeRef -> string * List<Type> * Type -> MethodSpec

(* Construct references to constructors *)
val mk_ctor_mspec: TypeRef * boxity * List<Type> * Instantiation -> MethodSpec
val mk_nongeneric_ctor_mspec: TypeRef * boxity * List<Type> -> MethodSpec
val mk_ctor_mspec_for_boxed_tspec: TypeSpec * List<Type> -> MethodSpec
val mk_ctor_mspec_for_typ: Type * List<Type> -> MethodSpec
val mk_ctor_mspec_for_nongeneric_boxed_tref: TypeRef * List<Type> -> MethodSpec

(* Construct references to FieldDefs *)
val mk_fref_in_tref: TypeRef * string * Type -> FieldRef
val mk_fspec: FieldRef * Type -> FieldSpec
val mk_fspec_in_typ: Type * string * Type -> FieldSpec
val mk_fspec_in_tspec: TypeSpec * boxity * string * Type -> FieldSpec
val mk_fspec_in_boxed_tspec: TypeSpec * string * Type -> FieldSpec
val mk_fspec_in_nongeneric_boxed_tref: TypeRef * string * Type -> FieldSpec

val ref_to_field_in_tdef: ScopeRef -> TypeDef -> string * Type -> FieldSpec
val mk_mspec_to_mdef: Type * MethodDef * Instantiation -> MethodSpec

val mk_CallSig: Callconv * List<Type> * Type -> CallSig

(* -------------------------------------------------------------------- 
 * Make generalized versions of possibly-generic Types,
 * e.g. Given the TypeDef for List, return the type "List<T>".
 * -------------------------------------------------------------------- *)

val generalize_tref: TypeRef -> GenericParam list -> TypeSpec
val generalize_tdef: ScopeRef -> TypeDef -> TypeSpec
val generalize_nested_tdef: ScopeRef -> TypeDef list * TypeDef -> TypeSpec

val gparams_add: GenericParams -> GenericParams -> GenericParams
val gparam_of_gactual: Type -> GenericParam
val gparams_of_inst: Instantiation -> GenericParam list
val generalize_gparams: GenericParam list -> List<Type>

(* -------------------------------------------------------------------- 
 * Custom attributes 
 * -------------------------------------------------------------------- *)

val mk_CustomAttributeibute: 
    TypeRef * 
    List<CustomAttributeElement> (* fixed args: values and implicit Types *) * 
    List<(FieldSpec * bool * CustomAttributeElement)> (* named args: vluaes and flags indicating if they are FieldDefs or properties *) 
      -> CustomAttribute

(* -------------------------------------------------------------------- 
 * Making code.
 * -------------------------------------------------------------------- *)

val check_code:  Code -> Code
val new_generator: unit -> (string -> string)
val generate_CodeLabel: string -> CodeLabel

(* Make some code that is a straight line sequence of instructions. *)
(* The function will add a "return" if the last instruction is not an exiting instruction *)
val nonbranching_instrs_to_code:  Instr list -> Code

(* For "join_code c1 c2", the unique exit of c1 should be the entry of c2, and the *)
(* label is then hidden in the result. *)
val join_code: Code -> Code -> Code

(* Some more primitive helpers *)
val mk_bblock: basic_block -> Code
val mk_scope_block: local_debug_info list * Code -> Code
val mk_group_block: List<CodeLabel> * Code list -> Code
val mk_try_finally_block: Code * CodeLabel * Code -> Code
val mk_try_fault_block: Code * CodeLabel * Code -> Code

type ('a, 'b) choice = Choice1of2 of 'a | Choice2of2 of 'b

val mk_try_multi_filter_catch_block:
  Code * (((CodeLabel * Code), typ) choice * (CodeLabel * Code)) list -> Code

(* -------------------------------------------------------------------- 
 * Injecting code into existing code blocks.  A branch will
 * be added from the given instructions to the (unique) entry of
 * the code, and the first instruction will be the new entry
 * of the method.  The instructions should be non-branching.
 *
 * If you need to inject more complicated code sequences at the
 * start of a method, you can use "join_code" on the underlying "code"
 * for the method.
 * -------------------------------------------------------------------- *)

val prepend_instrs_to_code: Instr list -> Code -> Code
val prepend_instrs_to_ilmbody: Instr list -> CILMethodBody -> CILMethodBody
val prepend_instrs_to_mdef: Instr list -> MethodDef -> MethodDef

(* -------------------------------------------------------------------- 
 * Default values for some of the strange flags in a module.
 * -------------------------------------------------------------------- *)

val default_modulSubSystem: Int32
val default_modulVirtAlignment: Int32
val default_modulPhysAlignment: Int32
val default_modulImageBase: Int32

(* -------------------------------------------------------------------- 
 * Helper to check a class is really an enum
 * -------------------------------------------------------------------- *)
val check_enum_FieldDefs:
   TypeRef -> FieldDef list -> Type * FieldDef list

(* -------------------------------------------------------------------- 
 * Derived functions for making some instructions
 * -------------------------------------------------------------------- *)

val mk_normal_call: MethodSpec -> Instr
val mk_normal_callvirt: MethodSpec -> Instr
val mk_normal_newobj: MethodSpec -> Instr
val mk_mscorlib_exn_newobj: string -> Instr
val mk_nongeneric_call_superclass_constructor: boxity -> TypeRef -> Instr list
val mk_call_superclass_constructor: TypeSpec -> Instr list
val mk_normal_stfld: FieldSpec -> Instr
val mk_normal_stsfld: FieldSpec -> Instr
val mk_normal_ldsfld: FieldSpec -> Instr
val mk_normal_ldfld: FieldSpec -> Instr
val mk_normal_ldflda: FieldSpec -> Instr
val mk_normal_stind: BasicType -> Instr
val mk_normal_ldind: BasicType -> Instr
val mk_normal_cpind: BasicType -> Instr list
val mk_writeline_call: Instr
val mk_ldc_Int32: Int32 -> Instr

val mk_param: string option * Type -> param
val mk_unnamed_param: Type -> param
val mk_named_param: string * Type -> param
val mk_return: Type -> ReturnSpec
val mk_void_return: ReturnSpec
val mk_local: Type -> local

val mk_ilmbody: bool (* initlocals? *) * local list * int * Code * Option<SourceAnnotation> -> CILMethodBody
val mk_impl: bool * local list * int * Code * Option<SourceAnnotation> -> MethodBody_details

val mk_ctor: MemberAccess * List<Param> * MethodBody_details -> MethodDef
val mk_nongeneric_nothing_ctor: Option<SourceAnnotation> -> TypeRef -> boxity -> List<Param> -> MethodDef
val mk_nothing_ctor: Option<SourceAnnotation> -> TypeSpec -> List<Param> -> MethodDef
val mk_static_mdef: GenericParams * string * MemberAccess * List<Param> * Type * MethodBody_details -> MethodDef
val mk_static_nongeneric_mdef: string * MemberAccess * List<Param> * Type * MethodBody_details -> MethodDef
val mk_cctor: MethodBody_details -> MethodDef
val mk_generic_virtual_mdef: string * MemberAccess * GenericParams * (Type * List<Type> * typ) option * List<Param> * Type * MethodBody_details -> MethodDef
val mk_nongeneric_virtual_mdef: string * MemberAccess * (Type * List<Type> * typ) option * List<Param> * Type * MethodBody_details -> MethodDef
val mk_instance_nongeneric_mdef: string * MemberAccess * List<Param> * Type * MethodBody_details -> MethodDef
val mk_normal_virtual_mdef: string * MemberAccess * List<Param> * Type * MethodBody_details -> MethodDef

val gen_mk_fdef:
  bool (* static? *) * string * Type * Option<FieldInit> *  Option<Bytes> * MemberAccess -> FieldDef
val mk_fdef: string * Type * Option<FieldInit> * MemberAccess -> FieldDef
val mk_static_fdef:
  string * Type * Option<FieldInit> * Option<Bytes> * MemberAccess -> FieldDef

val add_mdef_to_tdef: MethodDef -> TypeDef -> TypeDef
val mk_generic_class: string * TypeDefAccess * GenericParams * Type * List<Type> * MethodDefs * FieldDefs -> TypeDef
val mk_simple_tdef: string * TypeDefAccess * MethodDefs * FieldDefs -> TypeDef
val mk_toplevel_tdef: MethodDefs * FieldDefs -> TypeDef
val add_fdef_to_tdef: FieldDef -> TypeDef -> TypeDef

val mk_rawdata_vtdef:  string * Int32 (* size *) * UInt16 (* pack *) -> TypeDef

val tref_for_tdef: ScopeRef -> TypeDef -> TypeRef
val tref_for_nested_tdef: ScopeRef -> TypeDef list * TypeDef -> TypeRef
val tspec_for_nested_tdef: ScopeRef -> TypeDef list * TypeDef -> TypeSpec

val prepend_instrs_to_cctor: Instr list -> Option<SourceAnnotation> -> TypeDef -> TypeDef

type tmps = { num_old_locals: int; mutable newlocals: local list; } 
val alloc_tmp: tmps -> local -> UInt16

val mk_storage_ctor: Option<SourceAnnotation> -> Instr list -> TypeSpec -> (string * typ) list -> MethodDef
val mk_simple_storage_ctor: Option<SourceAnnotation> -> TypeSpec option -> TypeSpec -> (string * typ) list -> MethodDef

val and_Tailness: Tailness -> bool -> Tailness

val mk_ctor_mspec_for_delegate_tdef: ScopeRef * TypeDef * Instantiation -> MethodSpec 

val tref_for_toplevel: ScopeRef -> TypeRef
val tspec_for_toplevel: ScopeRef -> TypeSpec
val typ_for_toplevel: ScopeRef -> Type

val mk_CustomAttributes: CustomAttribute list -> CustomAttributes
val mk_lazy_CustomAttributes: (CustomAttribute list) Lazy.t -> CustomAttributes
val add_CustomAttribute: CustomAttribute -> CustomAttributes -> CustomAttributes

val mk_SecurityDecls: permission list -> SecurityDecls
val mk_lazy_SecurityDecls: (permission list) Lazy.t -> SecurityDecls
val add_security_decl: permission -> SecurityDecls -> SecurityDecls

val mk_mbody : MethodBody_details -> MethodBody
val mk_lazy_mbody : MethodBody_details Lazy.t -> MethodBody

val mk_events: EventDef list -> EventDefs
val mk_lazy_events: (EventDef list) Lazy.t -> EventDefs
val add_event: EventDef -> EventDefs -> EventDefs

val mk_properties: PropertyDef list -> ILPropertyDefs
val mk_lazy_properties: (PropertyDef list) Lazy.t -> ILPropertyDefs
val add_property: PropertyDef -> ILPropertyDefs -> ILPropertyDefs

val mk_mdefs: MethodDef list -> MethodDefs
val mk_lazy_mdefs: (MethodDef list) Lazy.t -> MethodDefs
val add_mdef:  MethodDef -> MethodDefs -> MethodDefs

val mk_fdefs: FieldDef list -> FieldDefs
val mk_lazy_fdefs: (FieldDef list) Lazy.t -> FieldDefs
val add_fdef: FieldDef -> FieldDefs -> FieldDefs

val mk_mimpls: MethodImpl list -> MethodImpls
val mk_lazy_mimpls: (MethodImpl list) Lazy.t -> MethodImpls
val add_mimpl: MethodImpl -> MethodImpls -> MethodImpls

val mk_tdefs: TypeDef  list -> TypeDefs
val mk_lazy_tdefs: ((string * TypeDef Lazy.t) list) Lazy.t -> TypeDefs
val add_tdef: TypeDef -> TypeDefs -> TypeDefs

val add_nested_class_elsewhere: nested_class_elsewhere -> nested_classes_elsewhere -> nested_classes_elsewhere
val mk_nested_classes_elsewhere: nested_class_elsewhere list -> nested_classes_elsewhere
val mk_lazy_nested_classes_elsewhere: (nested_class_elsewhere list) Lazy.t -> nested_classes_elsewhere

val mk_classes_elsewhere: class_elsewhere list -> classes_elsewhere
val mk_lazy_classes_elsewhere: (class_elsewhere list) Lazy.t -> classes_elsewhere
val add_class_elsewhere: class_elsewhere -> classes_elsewhere -> classes_elsewhere

val mk_resources: Resource list -> Resources
val add_resource: Resource -> Resources -> Resources
val mk_lazy_resources: (Resource list) Lazy.t -> Resources

exception Not_unique_field of FieldDef
exception Not_unique_method of MethodDef
exception Not_unique_type of string

val mk_simple_mainmod: AssemblyName -> ModuleName -> bool (* dll? *) -> TypeDefs -> modul
val mk_modul_fragment: ModuleName -> TypeDefs -> modul
val add_toplevel_mdef: MethodDef -> modul -> modul


(* ====================================================================
 * PART 3: Utilities
 * ==================================================================== *)

val rescope_scoref: ScopeRef -> ScopeRef -> ScopeRef
val rescope_tspec: ScopeRef -> TypeSpec -> TypeSpec
val rescope_typ: ScopeRef -> Type -> Type
val rescope_mspec: ScopeRef -> MethodSpec -> MethodSpec
val rescope_ospec: ScopeRef -> OverridesSpec -> OverridesSpec
val rescope_mref: ScopeRef -> MethodRef -> MethodRef 
val rescope_fref: ScopeRef -> FieldRef -> FieldRef
val rescope_fspec: ScopeRef -> FieldSpec -> FieldSpec


type seh_clause = 
  | SEH_finally of (CodeLabel * CodeLabel)
  | SEH_fault  of (CodeLabel * CodeLabel)
  | SEH_filter_catch of (CodeLabel * CodeLabel) * (CodeLabel * CodeLabel)
  | SEH_type_catch of Type * (CodeLabel * CodeLabel)

type exception_spec = 
    { exnRange: (CodeLabel * CodeLabel);
      exnClauses: seh_clause list }

type local_spec = 
    { locRange: (CodeLabel * CodeLabel);
      locInfos: local_debug_info list }

val build_code:
    string ->
    (CodeLabel -> int) -> 
    Instr array -> 
    exception_spec list -> 
    local_spec list -> 
    Code

val inst_tspec_aux: int -> Instantiation -> TypeSpec -> TypeSpec
val inst_typ_aux: int -> Instantiation -> Type -> Type
val inst_Type_aux: int -> Instantiation -> Type -> Type
val inst_inst_aux: int -> Instantiation -> Instantiation -> Instantiation
val inst_CallSig_aux: int -> Instantiation -> CallSig -> CallSig
val inst_typ: Instantiation -> Type -> Type
val inst_inst: Instantiation -> Instantiation -> Instantiation
val inst_tspec: Instantiation -> TypeSpec -> TypeSpec
val inst_CallSig: Instantiation -> CallSig -> CallSig

val inst_read: Instantiation -> UInt16 -> Type
val inst_add: Instantiation -> Instantiation -> Instantiation


val ecma_public_token: PublicKeyInfo

val mscorlib_assembly_name: string
val mscorlib_module_name: string
val mscorlib_aref: AssemblyRef
val mscorlib_scoref: ScopeRef

val tname_Object: string
val tref_Object: TypeRef
val tspec_Object: TypeSpec
val typ_Object: typ
val tname_String: string
val tref_String: TypeRef
val tspec_String: TypeSpec
val typ_String: typ
val tname_AsyncCallback: string
val tref_AsyncCallback: TypeRef
val tspec_AsyncCallback: TypeSpec
val typ_AsyncCallback: typ
val tname_IAsyncResult: string
val tref_IAsyncResult: TypeRef
val tspec_IAsyncResult: TypeSpec
val typ_IAsyncResult: typ
val tname_IComparable: string
val tref_IComparable: TypeRef
val tspec_IComparable: TypeSpec
val typ_IComparable: typ
val tname_Type: string
val tref_Type: TypeRef
val tspec_Type: TypeSpec
val typ_Type: typ
val tname_Delegate: string
val tref_Delegate: TypeRef
val tspec_Delegate: TypeSpec
val typ_Delegate: typ
val tname_ValueType: string
val tref_ValueType: TypeRef
val tspec_ValueType: TypeSpec
val typ_ValueType: typ
val tname_Enum: string
val tref_Enum: TypeRef
val tspec_Enum: TypeSpec
val typ_Enum: typ
val tname_TypedReference: string
val tref_TypedReference: TypeRef
val tspec_TypedReference: TypeSpec
val typ_TypedReference: typ

val tname_MulticastDelegate: string
val tref_MulticastDelegate: TypeRef
val tspec_MulticastDelegate: TypeSpec
val typ_MulticastDelegate: typ
val tname_Array: string
val tref_Array: TypeRef
val tspec_Array: TypeSpec
val typ_Array: typ
val tname_Int64: string
val tref_Int64: TypeRef
val tname_UInt64: string
val tref_UInt64: TypeRef
val tname_Int32: string
val tref_Int32: TypeRef
val tname_UInt32: string
val tref_UInt32: TypeRef
val tname_Int16: string
val tref_Int16: TypeRef
val tname_UInt16: string
val tref_UInt16: TypeRef
val tname_SByte: string
val tref_SByte: TypeRef
val tname_Byte: string
val tref_Byte: TypeRef
val tname_Single: string
val tref_Single: TypeRef
val tname_Double: string
val tref_Double: TypeRef
val tname_NativeFloat: string
val tref_NativeFloat: TypeRef
val tname_Bool: string
val tref_Bool: TypeRef
val tname_Char: string
val tref_Char: TypeRef
val tname_IntPtr: string
val tref_IntPtr: TypeRef
val tname_UIntPtr: string
val tref_UIntPtr: TypeRef
val tspec_Int64: TypeSpec
val tspec_UInt64: TypeSpec
val tspec_Int32: TypeSpec
val tspec_UInt32: TypeSpec
val tspec_Int16: TypeSpec
val tspec_UInt16: TypeSpec
val tspec_SByte: TypeSpec
val tspec_Byte: TypeSpec
val tspec_Single: TypeSpec
val tspec_Double: TypeSpec
val tspec_NativeFloat: TypeSpec
val tspec_IntPtr: TypeSpec
val tspec_UIntPtr: TypeSpec
val tspec_Char: TypeSpec
val tspec_Bool: TypeSpec
val typ_Int8: Type
val typ_Int16: Type
val typ_Int32: Type
val typ_Int64: Type
val typ_UInt8: Type
val typ_UInt16: Type
val typ_UInt32: Type
val typ_UInt64: Type
val typ_Single: Type
val typ_Double: Type
val typ_bool: Type
val typ_char: Type
val typ_int: Type
val typ_uint: Type
val tname_RuntimeArgumentHandle: string
val tref_RuntimeArgumentHandle: TypeRef
val tspec_RuntimeArgumentHandle: TypeSpec
val typ_RuntimeArgumentHandle: Type
val tname_RuntimeTypeHandle: string
val tref_RuntimeTypeHandle: TypeRef
val tspec_RuntimeTypeHandle: TypeSpec
val typ_RuntimeTypeHandle: Type
val tname_RuntimeMethodHandle: string
val tref_RuntimeMethodHandle: TypeRef
val tspec_RuntimeMethodHandle: TypeSpec
val typ_RuntimeMethodHandle: Type
val tname_RuntimeFieldHandle: string
val tref_RuntimeFieldHandle: TypeRef
val tspec_RuntimeFieldHandle: TypeSpec
val typ_RuntimeFieldHandle: Type

val typ_Byte: Type
val typ_Int16: Type
val typ_Int32: Type
val typ_Int64: Type
val typ_SByte: Type
val typ_UInt16: Type
val typ_UInt32: Type
val typ_UInt64: Type
val typ_Single: Type
val typ_Double: Type
val typ_NativeFloat: Type
val typ_Bool: Type
val typ_Char: Type
val typ_IntPtr: Type
val typ_UIntPtr: Type

val tname_Exception: string
val tref_Exception: TypeRef
val tspec_Exception: TypeSpec
val typ_Exception: Type

(* Some commonly used methods *)
val mspec_Object_Equals: MethodSpec
val mspec_Object_GetHashCode: MethodSpec
val mspec_IComparable_CompareTo: MethodSpec
val mspec_Console_WriteLine: MethodSpec
val mspec_RuntimeHelpers_InitializeArray: MethodSpec 

val mk_DebuggableAttribute: bool (* debug tracking *) * bool (* disable JIT optimizations *) -> CustomAttribute

val mk_DebuggerHiddenAttribute: CustomAttribute
val mk_DebuggerStepThroughAttribute: CustomAttribute

val mk_RunClassConstructor: TypeSpec -> Instr list

val typ_is_SystemMulticastDelegate: Type -> bool
val typ_is_SystemDelegate: Type -> bool
val typ_is_SystemValueType: Type -> bool
val typ_is_SystemEnum: Type -> bool

val token_from_public_key: Bytes -> Bytes

val parse_version: string -> VersionInfo

val info_for_delegate: string * MethodDefs -> delegate_info
val info_for_enum: string * FieldDefs -> enum_info

val intern_Bytes: Bytes -> Bytes
val intern_string: string -> string
val intern_tref: TypeRef -> TypeRef
val intern_tspec: TypeSpec -> TypeSpec
val intern_typ: Type -> Type
val intern_instr: Instr -> Instr

type refs = 
    { refsAssembly: AssemblyRef list; 
      refsModul: ModuleRef list; }

val refs_of_module: modul -> refs
val combine_refs: refs -> refs -> refs
val empty_refs: refs

