// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The "unlinked" view of .NET metadata and code.  Central to the Abstract IL library
module public Microsoft.FSharp.Compiler.AbstractIL.IL 

open System.Collections.Generic
open System.Reflection

[<RequireQualifiedAccess>]
type PrimaryAssembly = 
    | Mscorlib
    | System_Runtime
    | NetStandard

    member Name: string

/// Represents guids 
type ILGuid = byte[]

[<StructuralEquality; StructuralComparison>]
type ILPlatform = 
    | X86
    | AMD64
    | IA64

/// Debug info.  Values of type "source" can be attached at sequence 
/// points and some other locations. 
[<Sealed>]
type ILSourceDocument =
    static member Create: language: ILGuid option * vendor: ILGuid option * documentType: ILGuid option * file: string -> ILSourceDocument
    member Language: ILGuid option
    member Vendor: ILGuid option
    member DocumentType: ILGuid option
    member File: string


[<Sealed>]
type ILSourceMarker =
    static member Create: document: ILSourceDocument * line: int * column: int * endLine:int * endColumn: int-> ILSourceMarker
    member Document: ILSourceDocument
    member Line: int
    member Column: int
    member EndLine: int
    member EndColumn: int

[<StructuralEquality; StructuralComparison>]
type PublicKey = 
    | PublicKey of byte[]
    | PublicKeyToken of byte[]
    member IsKey: bool
    member IsKeyToken: bool
    member Key: byte[]
    member KeyToken: byte[]
    static member KeyAsToken: byte[] -> PublicKey 

type ILVersionInfo = uint16 * uint16 * uint16 * uint16

[<Sealed>]
type ILAssemblyRef =
    static member Create: name: string * hash: byte[] option * publicKey: PublicKey option * retargetable: bool * version: ILVersionInfo option * locale: string option -> ILAssemblyRef
    static member FromAssemblyName: System.Reflection.AssemblyName -> ILAssemblyRef
    member Name: string

    /// The fully qualified name of the assembly reference, e.g. mscorlib, Version=1.0.3705 etc.
    member QualifiedName: string 
    member Hash: byte[] option
    member PublicKey: PublicKey option

    /// CLI says this indicates if the assembly can be retargeted (at runtime) to be from a different publisher. 
    member Retargetable: bool
    member Version: ILVersionInfo option
    member Locale: string option
    interface System.IComparable

[<Sealed>]
type ILModuleRef =
    static member Create: name: string * hasMetadata: bool * hash: byte[] option -> ILModuleRef
    member Name: string
    member HasMetadata: bool
    member Hash: byte[] option
    interface System.IComparable

// Scope references
[<StructuralEquality; StructuralComparison; RequireQualifiedAccess>]
type ILScopeRef = 
    /// A reference to the type in the current module
    | Local 
    /// A reference to a type in a module in the same assembly
    | Module of ILModuleRef   
    /// A reference to a type in another assembly
    | Assembly of ILAssemblyRef  
    member IsLocalRef: bool
    member IsModuleRef: bool
    member IsAssemblyRef: bool
    member ModuleRef: ILModuleRef
    member AssemblyRef: ILAssemblyRef
    member QualifiedName: string

// Calling conventions.  
//
// For nearly all purposes you simply want to use ILArgConvention.Default combined
// with ILThisConvention.Instance or ILThisConvention.Static, i.e.
//   ILCallingConv.Instance == Callconv(ILThisConvention.Instance, ILArgConvention.Default): for an instance method
//   ILCallingConv.Static   == Callconv(ILThisConvention.Static, ILArgConvention.Default): for a static method
//
// ILThisConvention.InstanceExplicit is only used by Managed C++, and indicates 
// that the 'this' pointer is actually explicit in the signature. 
[<StructuralEquality; StructuralComparison; RequireQualifiedAccess>]
type ILArgConvention = 
    | Default
    | CDecl 
    | StdCall 
    | ThisCall 
    | FastCall 
    | VarArg
      
[<StructuralEquality; StructuralComparison; RequireQualifiedAccess>]
type ILThisConvention =
    /// accepts an implicit 'this' pointer 
    | Instance           
    /// accepts an explicit 'this' pointer 
    | InstanceExplicit  
    /// no 'this' pointer is passed
    | Static             

[<StructuralEquality; StructuralComparison>]
type ILCallingConv =
    | Callconv of ILThisConvention * ILArgConvention

    member IsInstance: bool
    member IsInstanceExplicit: bool
    member IsStatic: bool
    member ThisConv: ILThisConvention
    member BasicConv: ILArgConvention

    static member Instance: ILCallingConv
    static member Static  : ILCallingConv

/// Array shapes. For most purposes the rank is the only thing that matters. 
type ILArrayBound = int32 option 

/// Lower-bound/size pairs 
type ILArrayBounds = ILArrayBound * ILArrayBound

type ILArrayShape =
    | ILArrayShape of ILArrayBounds list 

    member Rank: int

    /// Bounds for a single dimensional, zero based array 
    static member SingleDimensional: ILArrayShape
    static member FromRank: int -> ILArrayShape

type ILBoxity = 
    | AsObject
    | AsValue

type ILGenericVariance = 
    | NonVariant            
    | CoVariant             
    | ContraVariant         

/// Type refs, i.e. references to types in some .NET assembly
[<Sealed>]
type ILTypeRef =

    /// Create a ILTypeRef.
    static member Create: scope: ILScopeRef * enclosing: string list * name: string -> ILTypeRef

    /// Where is the type, i.e. is it in this module, in another module in this assembly or in another assembly? 
    member Scope: ILScopeRef

    /// The list of enclosing type names for a nested type. If non-nil then the first of these also contains the namespace.
    member Enclosing: string list

    /// The name of the type. This also contains the namespace if Enclosing is empty.
    member Name: string

    /// The name of the type in the assembly using the '.' notation for nested types.
    member FullName: string

    /// The name of the type in the assembly using the '+' notation for nested types.
    member BasicQualifiedName: string

    member QualifiedName: string

    interface System.IComparable
    
/// Type specs and types.  
[<Sealed>]
type ILTypeSpec =
    /// Create an ILTypeSpec.
    static member Create: typeRef:ILTypeRef * instantiation:ILGenericArgs -> ILTypeSpec

    /// Which type is being referred to?
    member TypeRef: ILTypeRef

    /// The type instantiation if the type is generic, otherwise empty
    member GenericArgs: ILGenericArgs
    
    /// Where is the type, i.e. is it in this module, in another module in this assembly or in another assembly? 
    member Scope: ILScopeRef
    
    /// The list of enclosing type names for a nested type. If non-nil then the first of these also contains the namespace.
    member Enclosing: string list
    
    /// The name of the type. This also contains the namespace if Enclosing is empty.
    member Name: string
    
    /// The name of the type in the assembly using the '.' notation for nested types.
    member FullName: string
    
    interface System.IComparable

and 
    [<RequireQualifiedAccess; StructuralEquality; StructuralComparison>]
    ILType =

    /// Used only in return and pointer types.
    | Void                   

    /// Array types 
    | Array of ILArrayShape * ILType 

    /// Unboxed types, including builtin types.
    | Value of ILTypeSpec     

    /// Reference types.  Also may be used for parents of members even if for members in value types. 
    | Boxed of ILTypeSpec     

    /// Unmanaged pointers.  Nb. the type is used by tools and for binding only, not by the verifier.
    | Ptr of ILType             

    /// Managed pointers.
    | Byref of ILType           

    /// ILCode pointers. 
    | FunctionPointer of ILCallingSignature        

    /// Reference a generic arg. 
    | TypeVar of uint16           

    /// Custom modifiers. 
    | Modified of            
          /// True if modifier is "required". 
          bool *                  
          /// The class of the custom modifier. 
          ILTypeRef *                   
          /// The type being modified. 
          ILType                     

    member TypeSpec: ILTypeSpec

    member Boxity: ILBoxity

    member TypeRef: ILTypeRef

    member IsNominal: bool

    member GenericArgs: ILGenericArgs

    member IsTyvar: bool

    member BasicQualifiedName: string

    member QualifiedName: string

and [<StructuralEquality; StructuralComparison>]
    ILCallingSignature =  
    { CallingConv: ILCallingConv
      ArgTypes: ILTypes
      ReturnType: ILType }

/// Actual generic parameters are  always types.  
and ILGenericArgs = ILType list

and ILTypes = ILType list

/// Formal identities of methods.  
[<Sealed>]
type ILMethodRef =

     /// Functional creation
     static member Create: enclosingTypeRef: ILTypeRef * callingConv: ILCallingConv * name: string * genericArity: int * argTypes: ILTypes * returnType: ILType -> ILMethodRef

     member DeclaringTypeRef: ILTypeRef

     member CallingConv: ILCallingConv

     member Name: string

     member GenericArity: int

     member ArgCount: int

     member ArgTypes: ILTypes

     member ReturnType: ILType

     member CallingSignature: ILCallingSignature

     interface System.IComparable
     
/// Formal identities of fields. 
[<StructuralEquality; StructuralComparison>]
type ILFieldRef = 
    { DeclaringTypeRef: ILTypeRef
      Name: string
      Type: ILType }

/// The information at the callsite of a method
[<Sealed>]
type ILMethodSpec =

     /// Functional creation
     static member Create: ILType * ILMethodRef * ILGenericArgs -> ILMethodSpec

     member MethodRef: ILMethodRef

     member DeclaringType: ILType 

     member GenericArgs: ILGenericArgs

     member CallingConv: ILCallingConv

     member GenericArity: int

     member Name: string

     member FormalArgTypes: ILTypes

     member FormalReturnType: ILType

     interface System.IComparable
      
/// Field specs.  The data given for a ldfld, stfld etc. instruction.
[<StructuralEquality; StructuralComparison>]    
type ILFieldSpec =
    { FieldRef: ILFieldRef
      DeclaringType: ILType }    

    member DeclaringTypeRef: ILTypeRef

    member Name: string

    member FormalType: ILType

    member ActualType: ILType

/// ILCode labels.  In structured code each code label refers to a basic block somewhere in the code of the method.
type ILCodeLabel = int

[<StructuralEquality; StructuralComparison>]
type ILBasicType =
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

[<StructuralEquality; StructuralComparison; RequireQualifiedAccess>]
type ILToken = 
    | ILType of ILType 
    | ILMethod of ILMethodSpec 
    | ILField of ILFieldSpec

[<StructuralEquality; StructuralComparison; RequireQualifiedAccess>]
type ILConst = 
    | I4 of int32
    | I8 of int64
    | R4 of single
    | R8 of double

type ILTailcall = 
    | Tailcall
    | Normalcall

type ILAlignment = 
    | Aligned
    | Unaligned1
    | Unaligned2
    | Unaligned4

type ILVolatility = 
    | Volatile
    | Nonvolatile

type ILReadonly = 
    | ReadonlyAddress
    | NormalAddress

type ILVarArgs = ILTypes option

[<StructuralEquality; StructuralComparison>]
type ILComparisonInstr = 
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

/// The instruction set.                                                     
[<StructuralEquality; NoComparison>]
type ILInstr = 
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
    | AI_conv      of ILBasicType
    | AI_conv_ovf  of ILBasicType
    | AI_conv_ovf_un  of ILBasicType
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
    | AI_ldc       of ILBasicType * ILConst
    | I_ldarg     of uint16
    | I_ldarga    of uint16
    | I_ldind     of ILAlignment * ILVolatility * ILBasicType
    | I_ldloc     of uint16
    | I_ldloca    of uint16
    | I_starg     of uint16
    | I_stind     of  ILAlignment * ILVolatility * ILBasicType
    | I_stloc     of uint16

    // Control transfer 
    | I_br    of  ILCodeLabel
    | I_jmp   of ILMethodSpec
    | I_brcmp of ILComparisonInstr * ILCodeLabel 
    | I_switch    of ILCodeLabel list 
    | I_ret 

     // Method call 
    | I_call     of ILTailcall * ILMethodSpec * ILVarArgs
    | I_callvirt of ILTailcall * ILMethodSpec * ILVarArgs
    | I_callconstraint of ILTailcall * ILType * ILMethodSpec * ILVarArgs
    | I_calli    of ILTailcall * ILCallingSignature * ILVarArgs
    | I_ldftn    of ILMethodSpec
    | I_newobj   of ILMethodSpec  * ILVarArgs
    
    // Exceptions 
    | I_throw
    | I_endfinally
    | I_endfilter
    | I_leave     of  ILCodeLabel
    | I_rethrow

    // Object instructions 
    | I_ldsfld      of ILVolatility * ILFieldSpec
    | I_ldfld       of ILAlignment * ILVolatility * ILFieldSpec
    | I_ldsflda     of ILFieldSpec
    | I_ldflda      of ILFieldSpec 
    | I_stsfld      of ILVolatility  *  ILFieldSpec
    | I_stfld       of ILAlignment * ILVolatility * ILFieldSpec
    | I_ldstr       of string
    | I_isinst      of ILType
    | I_castclass   of ILType
    | I_ldtoken     of ILToken
    | I_ldvirtftn   of ILMethodSpec

    // Value type instructions 
    | I_cpobj       of ILType
    | I_initobj     of ILType
    | I_ldobj       of ILAlignment * ILVolatility * ILType
    | I_stobj       of ILAlignment * ILVolatility * ILType
    | I_box         of ILType
    | I_unbox       of ILType
    | I_unbox_any   of ILType
    | I_sizeof      of ILType

    // Generalized array instructions. In AbsIL these instructions include 
    // both the single-dimensional variants (with ILArrayShape == ILArrayShape.SingleDimensional) 
    // and calls to the "special" multi-dimensional "methods" such as: 
    //   newobj void string[,]::.ctor(int32, int32) 
    //   call string string[,]::Get(int32, int32) 
    //   call string& string[,]::Address(int32, int32) 
    //   call void string[,]::Set(int32, int32,string) 
    //
    // The IL reader transforms calls of this form to the corresponding 
    // generalized instruction with the corresponding ILArrayShape 
    // argument. This is done to simplify the IL and make it more uniform. 
    // The IL writer then reverses this when emitting the binary. 
    | I_ldelem      of ILBasicType
    | I_stelem      of ILBasicType
    | I_ldelema     of ILReadonly * bool * ILArrayShape * ILType (* ILArrayShape = ILArrayShape.SingleDimensional for single dimensional arrays *)
    | I_ldelem_any  of ILArrayShape * ILType (* ILArrayShape = ILArrayShape.SingleDimensional for single dimensional arrays *)
    | I_stelem_any  of ILArrayShape * ILType (* ILArrayShape = ILArrayShape.SingleDimensional for single dimensional arrays *)
    | I_newarr      of ILArrayShape * ILType (* ILArrayShape = ILArrayShape.SingleDimensional for single dimensional arrays *)
    | I_ldlen

    // "System.TypedReference" related instructions: almost 
    // no languages produce these, though they do occur in mscorlib.dll 
    // System.TypedReference represents a pair of a type and a byref-pointer
    // to a value of that type. 
    | I_mkrefany    of ILType
    | I_refanytype  
    | I_refanyval   of ILType
    
    // Debug-specific 
    // I_seqpoint is a fake instruction to represent a sequence point: 
    // the next instruction starts the execution of the 
    // statement covered by the given range - this is a 
    // dummy instruction and is not emitted 
    | I_break 
    | I_seqpoint of ILSourceMarker 

    // Varargs - C++ only 
    | I_arglist  

    // Local aggregates, i.e. stack allocated data (alloca): C++ only 
    | I_localloc
    | I_cpblk of ILAlignment * ILVolatility
    | I_initblk of ILAlignment  * ILVolatility

    // EXTENSIONS
    | EI_ilzero of ILType
    | EI_ldlen_multi      of int32 * int32

[<RequireQualifiedAccess>]
type ILExceptionClause = 
    | Finally of (ILCodeLabel * ILCodeLabel)
    | Fault  of (ILCodeLabel * ILCodeLabel)
    | FilterCatch of (ILCodeLabel * ILCodeLabel) * (ILCodeLabel * ILCodeLabel)
    | TypeCatch of ILType * (ILCodeLabel * ILCodeLabel)

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILExceptionSpec = 
    { Range: (ILCodeLabel * ILCodeLabel)
      Clause: ILExceptionClause }

/// Indicates that a particular local variable has a particular source 
/// language name within a given set of ranges. This does not effect local 
/// variable numbering, which is global over the whole method. 
[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILLocalDebugMapping =
    { LocalIndex: int
      LocalName: string }

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILLocalDebugInfo = 
    { Range: (ILCodeLabel * ILCodeLabel);
      DebugMappings: ILLocalDebugMapping list }

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILCode = 
    { Labels: Dictionary<ILCodeLabel,int> 
      Instrs:ILInstr[] 
      Exceptions: ILExceptionSpec list 
      Locals: ILLocalDebugInfo list }

/// Field Init
[<RequireQualifiedAccess; StructuralEquality; StructuralComparison>]
type ILFieldInit = 
    | String of string
    | Bool of bool
    | Char of uint16
    | Int8 of sbyte
    | Int16 of int16
    | Int32 of int32
    | Int64 of int64
    | UInt8 of byte
    | UInt16 of uint16
    | UInt32 of uint32
    | UInt64 of uint64
    | Single of single
    | Double of double
    | Null

[<RequireQualifiedAccess; StructuralEquality; StructuralComparison>]
type ILNativeVariant = 
    | Empty
    | Null
    | Variant
    | Currency
    | Decimal               
    | Date               
    | BSTR               
    | LPSTR               
    | LPWSTR               
    | IUnknown               
    | IDispatch               
    | SafeArray               
    | Error               
    | HRESULT               
    | CArray               
    | UserDefined               
    | Record               
    | FileTime
    | Blob               
    | Stream               
    | Storage               
    | StreamedObject               
    | StoredObject               
    | BlobObject               
    | CF                
    | CLSID
    | Void 
    | Bool
    | Int8
    | Int16                
    | Int32                
    | Int64                
    | Single                
    | Double                
    | UInt8                
    | UInt16                
    | UInt32                
    | UInt64                
    | PTR                
    | Array of ILNativeVariant                
    | Vector of ILNativeVariant                
    | Byref of ILNativeVariant                
    | Int                
    | UInt                

/// Native Types, for marshalling to the native C interface.
/// These are taken directly from the ILASM syntax, see ECMA Spec (Partition II, 7.4).  
[<RequireQualifiedAccess; StructuralEquality; StructuralComparison>]
type ILNativeType = 
    | Empty
    | Custom of ILGuid * string * string * byte[] (* guid,nativeTypeName,custMarshallerName,cookieString *)
    | FixedSysString of int32
    | FixedArray of int32
    | Currency
    | LPSTR
    | LPWSTR
    | LPTSTR
    | LPUTF8STR
    | ByValStr
    | TBSTR
    | LPSTRUCT
    | Struct
    | Void
    | Bool
    | Int8
    | Int16
    | Int32
    | Int64
    | Single
    | Double
    | Byte
    | UInt16
    | UInt32
    | UInt64
    ///  optional idx of parameter giving size plus optional additive i.e. num elems 
    | Array of ILNativeType option * (int32 * int32 option) option 
    | Int
    | UInt
    | Method
    | AsAny
    | BSTR
    | IUnknown
    | IDispatch
    | Interface
    | Error               
    | SafeArray of ILNativeVariant * string option 
    | ANSIBSTR
    | VariantBool

/// Local variables
[<RequireQualifiedAccess; NoComparison; NoEquality>]
type ILLocal = 
    { Type: ILType
      IsPinned: bool
      DebugInfo: (string * int * int) option }
     
type ILLocals = list<ILLocal>

/// IL method bodies
[<RequireQualifiedAccess; NoComparison; NoEquality>]
type ILMethodBody = 
    { IsZeroInit: bool
      MaxStack: int32 
      NoInlining: bool
      AggressiveInlining: bool
      Locals: ILLocals
      Code: ILCode
      SourceMarker: ILSourceMarker option }

/// Member Access
[<RequireQualifiedAccess>]
type ILMemberAccess = 
    | Assembly
    | FamilyAndAssembly
    | FamilyOrAssembly
    | Family
    | Private 
    | Public 

[<RequireQualifiedAccess>]
type ILAttribElem = 
    /// Represents a custom attribute parameter of type 'string'. These may be null, in which case they are encoded in a special
    /// way as indicated by Ecma-335 Partition II.
    | String of string  option 
    | Bool of bool
    | Char of char
    | SByte of sbyte
    | Int16 of int16
    | Int32 of int32
    | Int64 of int64
    | Byte of byte
    | UInt16 of uint16
    | UInt32 of uint32
    | UInt64 of uint64
    | Single of single
    | Double of double
    | Null 
    | Type of ILType option
    | TypeRef of ILTypeRef option
    | Array of ILType * ILAttribElem list

/// Named args: values and flags indicating if they are fields or properties.
type ILAttributeNamedArg = string * ILType * bool * ILAttribElem

/// Custom attributes.  See 'decodeILAttribData' for a helper to parse the byte[] 
/// to ILAttribElem's as best as possible.  
type ILAttribute =
    { Method: ILMethodSpec  
      Data: byte[] 
      Elements: ILAttribElem list}

[<NoEquality; NoComparison; Struct>]
type ILAttributes =
    member AsArray: ILAttribute []
    member AsList: ILAttribute list

/// Represents the efficiency-oriented storage of ILAttributes in another item.
[<NoEquality; NoComparison>]
type ILAttributesStored

/// Method parameters and return values.
type IParameter =
    abstract Name: string option
    abstract Type: ILType
    abstract Default: ILFieldInit option  

    /// Marshalling map for parameters. COM Interop only. 
    abstract Marshal: ILNativeType option 

    abstract IsIn: bool
    abstract IsOut: bool
    abstract IsOptional: bool
    abstract CustomAttrs: ILAttributes

    abstract With:
         ?newName: string option * ?newTy: ILType * ?newDefaultValue: ILFieldInit option *
         ?newMarshal: ILNativeType option * ?newIsIn: bool * ?newIsOut: bool * ?newIsOptional: bool *
         ?newCustomAttrsStored: ILAttributesStored * ?newMetadataIndex: int
            -> IParameter

[<NoEquality; NoComparison; Class>]
type ILParameter =

    new:
        name: string option *
        ty: ILType *
        defaultValue: ILFieldInit option *
        marshal: ILNativeType option *
        isIn: bool *
        isOut: bool *
        isOptional: bool *
        customAttrsStored: ILAttributesStored *
        metadataIndex: int
            -> ILParameter

    interface IParameter 


type ILParameters = list<IParameter>

val typesOfILParams: ILParameters -> ILType list

/// Method return values.
[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILReturn = 
    { Marshal: ILNativeType option
      Type: ILType 
      CustomAttrsStored: ILAttributesStored
      MetadataIndex: int32  }

    member CustomAttrs: ILAttributes

[<RequireQualifiedAccess>]
type ILSecurityAction = 
    | Request 
    | Demand
    | Assert
    | Deny
    | PermitOnly
    | LinkCheck 
    | InheritCheck
    | ReqMin
    | ReqOpt
    | ReqRefuse
    | PreJitGrant
    | PreJitDeny
    | NonCasDemand
    | NonCasLinkDemand
    | NonCasInheritance
    | LinkDemandChoice
    | InheritanceDemandChoice
    | DemandChoice

type ILSecurityDecl =
    | ILSecurityDecl of ILSecurityAction * byte[]

/// Abstract type equivalent to ILSecurityDecl list - use helpers 
/// below to construct/destruct these.
[<NoComparison; NoEquality; Struct>]
type ILSecurityDecls =
    member AsList: ILSecurityDecl list

/// Represents the efficiency-oriented storage of ILSecurityDecls in another item.
[<NoEquality; NoComparison>]
type ILSecurityDeclsStored

/// PInvoke attributes.
[<RequireQualifiedAccess>]
type PInvokeCallingConvention =
    | None
    | Cdecl
    | Stdcall
    | Thiscall
    | Fastcall
    | WinApi

[<RequireQualifiedAccess>]
type PInvokeCharEncoding =
    | None
    | Ansi
    | Unicode
    | Auto

[<RequireQualifiedAccess>]
type PInvokeCharBestFit =
    | UseAssembly
    | Enabled
    | Disabled

[<RequireQualifiedAccess>]
type PInvokeThrowOnUnmappableChar =
    | UseAssembly
    | Enabled
    | Disabled

[<RequireQualifiedAccess; NoComparison; NoEquality>]
type PInvokeMethod =
    { Where: ILModuleRef
      Name: string
      CallingConv: PInvokeCallingConvention
      CharEncoding: PInvokeCharEncoding
      NoMangle: bool
      LastError: bool
      ThrowOnUnmappableChar: PInvokeThrowOnUnmappableChar
      CharBestFit: PInvokeCharBestFit }


/// [OverridesSpec] - refer to a method declaration in a superclass or interface. 
type ILOverridesSpec =
    | OverridesSpec of ILMethodRef * ILType
    member MethodRef: ILMethodRef
    member DeclaringType: ILType 

type ILMethodVirtualInfo =
    { IsFinal: bool 
      IsNewSlot: bool 
      IsCheckAccessOnOverride: bool
      IsAbstract: bool }

[<RequireQualifiedAccess>]
type MethodKind =
    | Static 
    | Cctor 
    | Ctor 
    | NonVirtual 
    | Virtual of ILMethodVirtualInfo

[<RequireQualifiedAccess>]
type MethodBody =
    | IL of ILMethodBody
    | PInvoke of PInvokeMethod 
    | Abstract
    | Native
    | NotAvailable

[<RequireQualifiedAccess>]
type MethodCodeKind =
    | IL
    | Native
    | Runtime


type IGenericParameterDef =
    abstract Name: string
    
     /// At most one is the parent type, the others are interface types.
    abstract Constraints: ILTypes 

     /// Variance of type parameters, only applicable to generic parameters for generic interfaces and delegates.
    abstract Variance: ILGenericVariance 

     /// Indicates the type argument must be a reference type.
    abstract HasReferenceTypeConstraint: bool    

     /// Indicates the type argument must be a value type, but not Nullable.
    abstract HasNotNullableValueTypeConstraint: bool  

     /// Indicates the type argument must have a public nullary constructor.
    abstract HasDefaultConstructorConstraint: bool 

    abstract CustomAttrs: ILAttributes

    abstract With:
        ?newName: string *
        ?newConstraints: ILTypes *
        ?newVariance: ILGenericVariance *
        ?newHasReferenceTypeConstraint: bool *
        ?newHasNotNullableValueTypeConstraint: bool *
        ?newHasDefaultConstructorConstraint: bool *
        ?newCustomAttrs: ILAttributes
            -> IGenericParameterDef

/// Generic parameters.  Formal generic parameter declarations may include the bounds, if any, on the generic parameter.
[<Sealed>]
type ILGenericParameterDef =
    interface IGenericParameterDef

    new:
        name: string *
        constraints: ILTypes *
        variance: ILGenericVariance *
        hasReferenceTypeConstraint: bool *
        hasNotNullableValueTypeConstraint: bool *
        hasDefaultConstructorConstraint: bool *
        customAttrsStored: ILAttributesStored *
        metadataIndex: int
            -> ILGenericParameterDef

type ILGenericParameterDefs = IGenericParameterDef list

[<NoComparison; NoEquality; Sealed>]
type ILLazyMethodBody = 
    member Contents: MethodBody 


type IMethodDef =
    abstract Name: string
    abstract Attributes: MethodAttributes
    abstract ImplAttributes: MethodImplAttributes
    abstract CallingConv: ILCallingConv
    abstract Parameters: ILParameters
    abstract Return: ILReturn
    abstract Body: ILLazyMethodBody
    abstract SecurityDecls: ILSecurityDecls
    abstract IsEntryPoint:bool
    abstract GenericParams: ILGenericParameterDefs
    abstract CustomAttrs: ILAttributes 
    abstract ParameterTypes: ILTypes
    abstract IsIL: bool
    abstract Code: ILCode option
    abstract Locals: ILLocals
    
    /// Indicates a .cctor method.
    abstract IsClassInitializer: bool

    /// Indicates a .ctor method.
    abstract IsConstructor: bool

    /// Indicates a static method.
    abstract IsStatic: bool

    /// Indicates this is an instance methods that is not virtual.
    abstract IsNonVirtualInstance: bool

    /// Indicates an instance methods that is virtual or abstract or implements an interface slot.  
    abstract IsVirtual: bool
    
    abstract IsFinal: bool
    abstract IsNewSlot: bool
    abstract IsCheckAccessOnOverride: bool
    abstract IsAbstract: bool
    abstract MethodBody: ILMethodBody
    abstract CallingSignature: ILCallingSignature
    abstract Access: ILMemberAccess
    abstract IsHideBySig: bool
    abstract IsSpecialName: bool

    /// The method is exported to unmanaged code using COM interop.
    abstract IsUnmanagedExport: bool
    abstract IsReqSecObj: bool

    /// Some methods are marked "HasSecurity" even if there are no permissions attached, e.g. if they use SuppressUnmanagedCodeSecurityAttribute 
    abstract HasSecurity: bool
    abstract IsManaged: bool
    abstract IsForwardRef: bool
    abstract IsInternalCall: bool
    abstract IsPreserveSig: bool
    abstract IsSynchronized: bool
    abstract IsNoInline: bool
    abstract IsAggressiveInline: bool

    /// SafeHandle finalizer must be run.
    abstract IsMustRun: bool
    
    /// Functional update of the value
    abstract With: ?name: string * ?attributes: MethodAttributes * ?implAttributes: MethodImplAttributes * ?callingConv: ILCallingConv * 
                 ?parameters: ILParameters * ?ret: ILReturn * ?body: ILLazyMethodBody * ?securityDecls: ILSecurityDecls * ?isEntryPoint:bool * 
                 ?genericParams: ILGenericParameterDefs * ?customAttrs: ILAttributes -> IMethodDef

    abstract WithSpecialName: IMethodDef
    abstract WithHideBySig: unit -> IMethodDef
    abstract WithHideBySig: bool -> IMethodDef
    abstract WithFinal: bool -> IMethodDef
    abstract WithAbstract: bool -> IMethodDef
    abstract WithAccess: ILMemberAccess -> IMethodDef
    abstract WithNewSlot: IMethodDef
    abstract WithSecurity: bool -> IMethodDef
    abstract WithPInvoke: bool -> IMethodDef
    abstract WithPreserveSig: bool -> IMethodDef
    abstract WithSynchronized: bool -> IMethodDef
    abstract WithNoInlining: bool -> IMethodDef
    abstract WithAggressiveInlining: bool -> IMethodDef
    abstract WithRuntime: bool -> IMethodDef


/// IL Method definitions. 
[<NoComparison; NoEquality>]
type ILMethodDef = 

    /// Functional creation of a value, with delayed reading of some elements via a metadata index
    new: name: string * attributes: MethodAttributes * implAttributes: MethodImplAttributes * callingConv: ILCallingConv * 
         parameters: ILParameters * ret: ILReturn * body: ILLazyMethodBody * isEntryPoint:bool * genericParams: ILGenericParameterDefs * 
         securityDeclsStored: ILSecurityDeclsStored * customAttrsStored: ILAttributesStored * metadataIndex: int32 -> ILMethodDef

    /// Functional creation of a value, immediate
    new: name: string * attributes: MethodAttributes * implAttributes: MethodImplAttributes * callingConv: ILCallingConv * 
         parameters: ILParameters * ret: ILReturn * body: ILLazyMethodBody * isEntryPoint:bool * genericParams: ILGenericParameterDefs * 
         securityDecls: ILSecurityDecls * customAttrs: ILAttributes -> ILMethodDef

    interface IMethodDef


/// Tables of methods.  Logically equivalent to a list of methods but
/// the table is kept in a form optimized for looking up methods by 
/// name and arity.
type IMethodDefs =
    inherit IEnumerable<IMethodDef>
    abstract AsArray: IMethodDef[]
    abstract AsList: IMethodDef list
    abstract FindByName: string -> IMethodDef list
    abstract FindByNameAndArity: string * int -> IMethodDef list

[<NoEquality; NoComparison; Sealed>]
type ILMethodDefs =
    interface IMethodDefs


type IFieldDef =
    abstract Name: string
    abstract FieldType: ILType
    abstract Attributes: FieldAttributes
    abstract Data:  byte[] option
    abstract LiteralValue: ILFieldInit option  

    /// The explicit offset in bytes when explicit layout is used.
    abstract Offset:  int32 option 
    abstract Marshal: ILNativeType option 
    abstract CustomAttrs: ILAttributes
    abstract IsStatic: bool
    abstract IsSpecialName: bool
    abstract IsLiteral: bool
    abstract NotSerialized: bool
    abstract IsInitOnly: bool
    abstract Access: ILMemberAccess

    /// Functional update of the value
    abstract With:
        ?newName: string * ?newFieldType: ILType * ?newAttributes: FieldAttributes * ?newData: byte[] option *
        ?newLiteralValue: ILFieldInit option * ?newOffset:  int32 option * ?newMarshal: ILNativeType option *
        ?newCustomAttrs: ILAttributes
            -> IFieldDef

    abstract WithAccess: ILMemberAccess -> IFieldDef
    abstract WithInitOnly: bool -> IFieldDef
    abstract WithStatic: bool -> IFieldDef
    abstract WithSpecialName: bool -> IFieldDef
    abstract WithNotSerialized: bool -> IFieldDef
    abstract WithLiteralDefaultValue: ILFieldInit option -> IFieldDef
    abstract WithFieldMarshal: ILNativeType option -> IFieldDef


/// Field definitions.
[<NoComparison; NoEquality>]
type ILFieldDef = 

    /// Functional creation of a value using delayed reading via a metadata index
    new: name: string * fieldType: ILType * attributes: FieldAttributes * data: byte[] option * 
         literalValue: ILFieldInit option * offset:  int32 option * marshal: ILNativeType option * 
         customAttrsStored: ILAttributesStored * metadataIndex: int32 -> ILFieldDef

    /// Functional creation of a value, immediate
    new: name: string * fieldType: ILType * attributes: FieldAttributes * data: byte[] option * 
         literalValue: ILFieldInit option * offset:  int32 option * marshal: ILNativeType option * 
         customAttrs: ILAttributes -> ILFieldDef

    interface IFieldDef


/// Tables of fields. Logically equivalent to a list of fields but the table is kept in 
/// a form to allow efficient looking up fields by name.
type IFieldDefs =
    abstract AsList: IFieldDef list
    abstract LookupByName: string -> IFieldDef list


[<NoEquality; NoComparison; Sealed>]
type ILFieldDefs =
    interface IFieldDefs


/// Event definitions.
type IEventDef =
    abstract EventType: ILType option
    abstract Name: string
    abstract Attributes: EventAttributes
    abstract AddMethod: ILMethodRef 
    abstract RemoveMethod: ILMethodRef
    abstract FireMethod: ILMethodRef option
    abstract OtherMethods: ILMethodRef list
    abstract CustomAttrs: ILAttributes
    abstract IsSpecialName: bool
    abstract IsRTSpecialName: bool

    /// Functional update of the value
    abstract With:
        ?eventType: ILType option * ?name: string * ?attributes: EventAttributes * ?addMethod: ILMethodRef * 
        ?removeMethod: ILMethodRef * ?fireMethod: ILMethodRef option * ?otherMethods: ILMethodRef list * 
        ?customAttrs: ILAttributes
            -> IEventDef


/// Table of those events in a type definition.
type IEventDefs =
    abstract AsList: IEventDef list
    abstract LookupByName: string -> IEventDef list


[<NoComparison; NoEquality>]
type ILEventDef =

    /// Functional creation of a value, using delayed reading via a metadata index, for ilread.fs
    new: eventType: ILType option * name: string * attributes: EventAttributes * addMethod: ILMethodRef * 
         removeMethod: ILMethodRef * fireMethod: ILMethodRef option * otherMethods: ILMethodRef list * 
         customAttrsStored: ILAttributesStored * metadataIndex: int32 -> ILEventDef

    /// Functional creation of a value, immediate
    new: eventType: ILType option * name: string * attributes: EventAttributes * addMethod: ILMethodRef * 
         removeMethod: ILMethodRef * fireMethod: ILMethodRef option * otherMethods: ILMethodRef list * 
         customAttrs: ILAttributes -> ILEventDef

    interface IEventDef


[<NoEquality; NoComparison; Sealed>]
type ILEventDefs =
    interface IEventDefs


type IPropertyDef =
    abstract Name: string
    abstract Attributes: PropertyAttributes
    abstract SetMethod: ILMethodRef option
    abstract GetMethod: ILMethodRef option
    abstract CallingConv: ILThisConvention
    abstract PropertyType: ILType          
    abstract Init: ILFieldInit option
    abstract Args: ILTypes
    abstract CustomAttrs: ILAttributes
    abstract IsSpecialName: bool
    abstract IsRTSpecialName: bool

    /// Functional update of the value
    abstract With: ?name: string * ?attributes: PropertyAttributes * ?setMethod: ILMethodRef option * ?getMethod: ILMethodRef option * 
                 ?callingConv: ILThisConvention * ?propertyType: ILType * ?init: ILFieldInit option * ?args: ILTypes * 
                 ?customAttrs: ILAttributes -> IPropertyDef


/// Table of properties in an IL type definition.
type IPropertyDefs =
    abstract AsList: IPropertyDef list
    abstract LookupByName: string -> IPropertyDef list


/// Property definitions
[<NoComparison; NoEquality>]
type ILPropertyDef =

    /// Functional creation of a value, using delayed reading via a metadata index, for ilread.fs
    new: name: string * attributes: PropertyAttributes * setMethod: ILMethodRef option * getMethod: ILMethodRef option * 
         callingConv: ILThisConvention * propertyType: ILType * init: ILFieldInit option * args: ILTypes * 
         customAttrsStored: ILAttributesStored * metadataIndex: int32 -> ILPropertyDef

    /// Functional creation of a value, immediate
    new: name: string * attributes: PropertyAttributes * setMethod: ILMethodRef option * getMethod: ILMethodRef option * 
         callingConv: ILThisConvention * propertyType: ILType * init: ILFieldInit option * args: ILTypes * 
         customAttrs: ILAttributes -> ILPropertyDef

    interface IPropertyDef

[<NoEquality; NoComparison>]
[<Sealed>]
type ILPropertyDefs =
    interface IPropertyDefs


/// Method Impls
type ILMethodImplDef =
    { Overrides: ILOverridesSpec
      OverrideBy: ILMethodSpec }

[<NoEquality; NoComparison; Sealed>]
type ILMethodImplDefs =
    member AsList: ILMethodImplDef list

/// Type Layout information.
[<RequireQualifiedAccess>]
type ILTypeDefLayout =
    | Auto
    | Sequential of ILTypeDefLayoutInfo
    | Explicit of ILTypeDefLayoutInfo 

and ILTypeDefLayoutInfo =
    { Size: int32 option
      Pack: uint16 option } 

/// Indicate the initialization semantics of a type.
[<RequireQualifiedAccess>]
type ILTypeInit =
    | BeforeField
    | OnAny

/// Default Unicode encoding for P/Invoke  within a type.
[<RequireQualifiedAccess>]
type ILDefaultPInvokeEncoding =
    | Ansi
    | Auto
    | Unicode

/// Type Access.
[<RequireQualifiedAccess>]
type ILTypeDefAccess =
    | Public 
    | Private
    | Nested of ILMemberAccess 

/// A categorization of type definitions into "kinds"
[<RequireQualifiedAccess>]
type ILTypeDefKind =
    | Class
    | ValueType
    | Interface
    | Enum 
    | Delegate 


/// Tables of named type definitions.  
type ITypeDefs =
    inherit  IEnumerable<ITypeDef>

    abstract AsArray: ITypeDef[]
    abstract AsList: ITypeDef list

    /// Get some information about the type defs, but do not force the read of the type defs themselves.
    abstract AsArrayOfPreTypeDefs: IPreTypeDef[]

    /// Calls to <c>FindByName</c> will result in any laziness in the overall 
    /// set of ILTypeDefs being read in in addition 
    /// to the details for the type found, but the remaining individual 
    /// type definitions will not be read. 
    abstract FindByName: string -> ITypeDef


and ITypeDef =
    abstract Name: string  
    abstract Attributes: TypeAttributes
    abstract GenericParams: ILGenericParameterDefs
    abstract Layout: ILTypeDefLayout
    abstract NestedTypes: ITypeDefs
    abstract Implements: ILTypes
    abstract Extends: ILType option
    abstract Methods: IMethodDefs
    abstract SecurityDecls: ILSecurityDecls
    abstract Fields: IFieldDefs
    abstract MethodImpls: ILMethodImplDefs
    abstract Events: IEventDefs
    abstract Properties: IPropertyDefs
    abstract CustomAttrs: ILAttributes
    abstract IsClass: bool
    abstract IsStruct: bool
    abstract IsInterface: bool
    abstract IsEnum: bool
    abstract IsDelegate: bool
    abstract IsStructOrEnum: bool
    abstract Access: ILTypeDefAccess
    abstract IsAbstract: bool
    abstract IsSealed: bool
    abstract IsSerializable: bool
    /// Class or interface generated for COM interop. 
    abstract IsComInterop: bool
    abstract IsSpecialName: bool
    /// Some classes are marked "HasSecurity" even if there are no permissions attached, 
    /// e.g. if they use SuppressUnmanagedCodeSecurityAttribute 
    abstract HasSecurity: bool
    abstract Encoding: ILDefaultPInvokeEncoding

    abstract WithAccess: ILTypeDefAccess -> ITypeDef
    abstract WithNestedAccess: ILMemberAccess -> ITypeDef
    abstract WithSealed: bool -> ITypeDef
    abstract WithSerializable: bool -> ITypeDef
    abstract WithAbstract: bool -> ITypeDef
    abstract WithImport: bool -> ITypeDef
    abstract WithHasSecurity: bool -> ITypeDef
    abstract WithLayout: ILTypeDefLayout -> ITypeDef
    abstract WithKind: ILTypeDefKind -> ITypeDef
    abstract WithEncoding: ILDefaultPInvokeEncoding -> ITypeDef
    abstract WithSpecialName: bool -> ITypeDef
    abstract WithInitSemantics: ILTypeInit -> ITypeDef

    /// Functional update
    abstract With:
        ?name: string * ?attributes: TypeAttributes * ?layout: ILTypeDefLayout *  ?implements: ILTypes * 
        ?genericParams:ILGenericParameterDefs * ?extends:ILType option * ?methods:IMethodDefs * 
        ?nestedTypes:ITypeDefs * ?fields: IFieldDefs * ?methodImpls:ILMethodImplDefs * ?events:IEventDefs * 
        ?properties:IPropertyDefs * ?customAttrs:ILAttributes * ?securityDecls: ILSecurityDecls
            -> ITypeDef

and IPreTypeDef = 
        abstract Namespace: string list
        abstract Name: string
        abstract MetadataIndex: int32
        /// Realise the actual full typedef
        abstract GetTypeDef : unit -> ITypeDef


/// Tables of named type definitions.
[<NoEquality; NoComparison; Sealed>]
type ILTypeDefs =
    interface IEnumerable<ITypeDef>
    interface ITypeDefs

/// Represents IL Type Definitions. 
and [<NoComparison; NoEquality>] ILTypeDef =  

    /// Functional creation of a value, using delayed reading via a metadata index, for ilread.fs
    new: name: string * attributes: TypeAttributes * layout: ILTypeDefLayout * implements: ILTypes * genericParams: ILGenericParameterDefs * 
          extends: ILType option * methods: IMethodDefs * nestedTypes: ITypeDefs * fields: IFieldDefs * methodImpls: ILMethodImplDefs * 
          events: IEventDefs * properties: IPropertyDefs * securityDeclsStored: ILSecurityDeclsStored * customAttrsStored: ILAttributesStored * metadataIndex: int32 -> ILTypeDef

    /// Functional creation of a value, immediate
    new: name: string * attributes: TypeAttributes * layout: ILTypeDefLayout * implements: ILTypes * genericParams: ILGenericParameterDefs * 
          extends: ILType option * methods: IMethodDefs * nestedTypes: ITypeDefs * fields: IFieldDefs * methodImpls: ILMethodImplDefs * 
          events: IEventDefs * properties: IPropertyDefs * securityDecls: ILSecurityDecls * customAttrs: ILAttributes -> ILTypeDef

    interface ITypeDef

/// Represents a prefix of information for ILTypeDef.
///
/// The information is enough to perform name resolution for the F# compiler, probe attributes
/// for ExtensionAttribute  etc.  This is key to the on-demand exploration of .NET metadata.
/// This information has to be "Goldilocks" - not too much, not too little, just right.
and [<NoEquality; NoComparison; Sealed>] ILPreTypeDef = 
    interface IPreTypeDef

and [<Sealed>] ILTypeDefStored 

val mkILPreTypeDef : ITypeDef -> IPreTypeDef
val mkILPreTypeDefComputed : string list * string * (unit -> ITypeDef) -> IPreTypeDef
val mkILPreTypeDefRead : string list * string * int32 * ILTypeDefStored -> IPreTypeDef
val mkILTypeDefReader: (int32 -> ITypeDef) -> ILTypeDefStored

[<NoEquality; NoComparison; Sealed>]
type ILNestedExportedTypes =
    member AsList: ILNestedExportedType  list

/// "Classes Elsewhere" - classes in auxiliary modules.
///
/// Manifests include declarations for all the classes in an 
/// assembly, regardless of which module they are in.
///
/// The ".class extern" construct describes so-called exported types -- 
/// these are public classes defined in the auxiliary modules of this assembly,
/// i.e. modules other than the manifest-carrying module. 
/// 
/// For example, if you have a two-module 
/// assembly (A.DLL and B.DLL), and the manifest resides in the A.DLL, 
/// then in the manifest all the public classes declared in B.DLL should
/// be defined as exported types, i.e., as ".class extern". The public classes 
/// defined in A.DLL should not be defined as ".class extern" -- they are 
/// already available in the manifest-carrying module. The union of all 
/// public classes defined in the manifest-carrying module and all 
/// exported types defined there is the set of all classes exposed by 
/// this assembly. Thus, by analysing the metadata of the manifest-carrying 
/// module of an assembly, you can identify all the classes exposed by 
/// this assembly, and where to find them.
///
/// Nested classes found in external modules should also be located in 
/// this table, suitably nested inside another "ILExportedTypeOrForwarder"
/// definition.

/// these are only found in the "Nested" field of ILExportedTypeOrForwarder objects 
// REVIEW: fold this into ILExportedTypeOrForwarder. There's not much value in keeping these distinct
and ILNestedExportedType =
    { Name: string
      Access: ILMemberAccess
      Nested: ILNestedExportedTypes
      CustomAttrsStored: ILAttributesStored
      MetadataIndex: int32 } 
    member CustomAttrs: ILAttributes

/// these are only found in the ILExportedTypesAndForwarders table in the manifest 
[<NoComparison; NoEquality>]
type ILExportedTypeOrForwarder =
    { ScopeRef: ILScopeRef
      /// [Namespace.]Name
      Name: string
      Attributes: TypeAttributes
      Nested: ILNestedExportedTypes
      CustomAttrsStored: ILAttributesStored
      MetadataIndex: int32 }
    member Access: ILTypeDefAccess
    member IsForwarder: bool
    member CustomAttrs: ILAttributes

[<NoEquality; NoComparison>]
[<Sealed>]
type ILExportedTypesAndForwarders =
    member AsList: ILExportedTypeOrForwarder  list

[<RequireQualifiedAccess>]
type ILResourceAccess = 
    | Public 
    | Private 

[<RequireQualifiedAccess>]
type ILResourceLocation = 
    /// Represents a manifest resource that can be read from within the PE file
    | LocalIn of string * int * int

    /// Represents a manifest resource that is due to be written to the output PE file
    | LocalOut of byte[]

    /// Represents a manifest resource in an associated file
    | File of ILModuleRef * int32

    /// Represents a manifest resource in a different assembly
    | Assembly of ILAssemblyRef

/// "Manifest ILResources" are chunks of resource data, being one of:
///   - the data section of the current module (byte[] of resource given directly).
///   - in an external file in this assembly (offset given in the ILResourceLocation field). 
///   - as a resources in another assembly of the same name.  
type ILResource =
    { Name: string
      Location: ILResourceLocation
      Access: ILResourceAccess
      CustomAttrsStored: ILAttributesStored
      MetadataIndex: int32 }

    /// Read the bytes from a resource local to an assembly. Will fail for non-local resources.
    member GetBytes : unit -> byte[]

    member CustomAttrs: ILAttributes

/// Table of resources in a module.
[<NoEquality; NoComparison>]
[<Sealed>]
type ILResources =
    member AsList: ILResource  list


[<RequireQualifiedAccess>]
type ILAssemblyLongevity =
    | Unspecified
    | Library
    | PlatformAppDomain
    | PlatformProcess
    | PlatformSystem


type IAssemblyManifest =
    abstract Name: string

    /// This is the ID of the algorithm used for the hashes of auxiliary files in the assembly.
    /// These hashes are stored in the <c>ILModuleRef.Hash</c> fields of this assembly.
    /// These are not cryptographic hashes: they are simple file hashes.
    /// The algorithm is normally <c>0x00008004</c> indicating the SHA1 hash algorithm.  
    abstract AuxModuleHashAlgorithm: int

    /// This is the public key used to sign this assembly (the signature itself is stored elsewhere: see the 
    /// binary format, and may not have been written if delay signing is used).
    /// (member Name, member PublicKey) forms the full public name of the assembly.  
    abstract PublicKey: byte[] option  

    abstract Version: ILVersionInfo option
    abstract Locale: string option
    abstract AssemblyLongevity: ILAssemblyLongevity 
    abstract DisableJitOptimizations: bool
    abstract JitTracking: bool
    abstract IgnoreSymbolStoreSequencePoints: bool
    abstract Retargetable: bool

    /// Records the types implemented by this assembly in auxiliary modules. 
    abstract ExportedTypes: ILExportedTypesAndForwarders

    /// Records whether the entrypoint resides in another module. 
    abstract EntrypointElsewhere: ILModuleRef option

    abstract CustomAttrs: ILAttributes
    abstract SecurityDecls: ILSecurityDecls

    abstract With:
        ?newPublicKey: byte[] option * ?newVersion: ILVersionInfo option * ?newDisableJitOptimizations: bool *
        ?newJitTracking: bool * ?newIgnoreSymbolStoreSequencePoints: bool *
        ?newCustomAttrsStored: ILAttributesStored * ?newSecurityDeclsStored: ILSecurityDeclsStored
            -> IAssemblyManifest


/// The main module of an assembly is a module plus some manifest information.
[<Sealed>]
type ILAssemblyManifest =

    new:
        name: string *
        auxModuleHashAlgorithm: int *
        securityDeclsStored: ILSecurityDeclsStored *
        publicKey: byte[] option *
        version: ILVersionInfo option *
        locale: string option *
        customAttrsStored: ILAttributesStored *
        assemblyLongevity: ILAssemblyLongevity *
        disableJitOptimizations: bool *
        jitTracking: bool *
        ignoreSymbolStoreSequencePoints: bool *
        retargetable: bool *
        exportedTypes: ILExportedTypesAndForwarders *
        entrypointElsewhere: ILModuleRef option *
        metadataIndex: int
            -> ILAssemblyManifest 

    interface IAssemblyManifest


[<RequireQualifiedAccess>]
type ILNativeResource = 
    /// Represents a native resource to be read from the PE file
    | In of fileName: string * linkedResourceBase: int * linkedResourceStart: int * linkedResourceLength: int

    /// Represents a native resource to be written in an output file
    | Out of unlinkedResource: byte[]


/// One module in the "current" assembly, either a main-module or an auxiliary module.
/// The main module will have a manifest.
///
/// An assembly is built by joining together a "main" module plus several auxiliary modules. 
type IModuleDef =
    abstract Manifest: IAssemblyManifest option
    abstract Name: string
    abstract TypeDefs: ITypeDefs
    abstract SubsystemVersion: int * int
    abstract UseHighEntropyVA: bool
    abstract SubSystemFlags: int32
    abstract IsDLL: bool
    abstract IsILOnly: bool
    abstract Platform: ILPlatform option
    abstract StackReserveSize: int32 option
    abstract Is32Bit: bool
    abstract Is32BitPreferred: bool
    abstract Is64Bit: bool
    abstract VirtualAlignment: int32
    abstract PhysicalAlignment: int32
    abstract ImageBase: int32
    abstract MetadataVersion: string
    abstract Resources: ILResources 

    /// e.g. win86 resources, as the exact contents of a .res or .obj file. Must be unlinked manually.
    abstract NativeResources: ILNativeResource list
    abstract CustomAttrsStored: ILAttributesStored
    abstract MetadataIndex: int32 
    abstract ManifestOfAssembly: IAssemblyManifest 
    abstract HasManifest: bool
    abstract CustomAttrs: ILAttributes

    abstract With:
        ?name: string * ?manifest: IAssemblyManifest option * ?typeDefs: ITypeDefs *
        ?subsystemVersion: (int * int) * ?useHighEntropyVA: bool * ?subSystemFlags: int32 * ?isDLL: bool *
        ?isILOnly: bool * ?platform: ILPlatform option * ?stackReserveSize: int32 option * ?is32Bit: bool *
        ?is32BitPreferred: bool * ?is64Bit: bool * ?virtualAlignment: int32 * ?physicalAlignment: int32 *
        ?imageBase: int32 * ?metadataVersion: string * ?resources: ILResources *
        ?nativeResources: ILNativeResource list * ?customAttrsStored: ILAttributesStored * ?metadataIndex: int32
            -> IModuleDef


[<Sealed>]
type ILModuleDef =
    internal new:
        name: string *
        manifest: IAssemblyManifest option *
        typeDefs: ITypeDefs *
        subsystemVersion: (int * int) *
        useHighEntropyVA: bool *
        subSystemFlags: int32 *
        isDLL: bool *
        isILOnly: bool *
        platform: ILPlatform option *
        stackReserveSize: int32 option *
        is32Bit: bool *
        is32BitPreferred: bool *
        is64Bit: bool *
        virtualAlignment: int32 *
        physicalAlignment: int32 *
        imageBase: int32 *
        metadataVersion: string *
        resources: ILResources *

        /// e.g. win86 resources, as the exact contents of a .res or .obj file. Must be unlinked manually.
        nativeResources: ILNativeResource list *
        customAttrsStored: ILAttributesStored *
        metadataIndex: int32
            -> ILModuleDef

    interface IModuleDef

/// Find the method definition corresponding to the given property or 
/// event operation. These are always in the same class as the property 
/// or event. This is useful especially if your code is not using the Ilbind 
/// API to bind references. 
val resolveILMethodRef: ITypeDef -> ILMethodRef -> IMethodDef
val resolveILMethodRefWithRescope: (ILType -> ILType) -> ITypeDef -> ILMethodRef -> IMethodDef

// ------------------------------------------------------------------ 
// Type Names
//
// The name of a type stored in the Name field is as follows:
//   - For outer types it is, for example, System.String, i.e.
//     the namespace followed by the type name.
//   - For nested types, it is simply the type name.  The namespace
//     must be gleaned from the context in which the nested type
//     lies.
// ------------------------------------------------------------------ 

val splitNamespace: string -> string list

val splitNamespaceToArray: string -> string[]

/// The <c>splitILTypeName</c> utility helps you split a string representing
/// a type name into the leading namespace elements (if any), the
/// names of any nested types and the type name itself.  This function
/// memoizes and interns the splitting of the namespace portion of
/// the type name. 
val splitILTypeName: string -> string list * string

val splitILTypeNameWithPossibleStaticArguments: string -> string[] * string

/// <c>splitTypeNameRight</c> is like <c>splitILTypeName</c> except the 
/// namespace is kept as a whole string, rather than split at dots.
val splitTypeNameRight: string -> string option * string

val typeNameForGlobalFunctions: string

val isTypeNameForGlobalFunctions: string -> bool

val ungenericizeTypeName: string -> string (* e.g. List`1 --> List *)


// ====================================================================
// PART 2
// 
// Making metadata.  Where no explicit constructor
// is given, you should create the concrete datatype directly, 
// e.g. by filling in all appropriate record fields.
// ==================================================================== *)

/// A table of common references to items in primary assembly (System.Runtime or mscorlib).
/// If a particular version of System.Runtime.dll has been loaded then you should 
/// reference items from it via an ILGlobals for that specific version built using mkILGlobals. 
[<NoEquality; NoComparison; Class>]
type ILGlobals = 
    member primaryAssemblyScopeRef: ILScopeRef
    member primaryAssemblyName: string
    member typ_Object: ILType
    member typ_String: ILType
    member typ_Type: ILType
    member typ_Array: ILType
    member typ_IntPtr: ILType
    member typ_UIntPtr: ILType
    member typ_Byte: ILType
    member typ_Int16: ILType
    member typ_Int32: ILType
    member typ_Int64: ILType
    member typ_SByte: ILType
    member typ_UInt16: ILType
    member typ_UInt32: ILType
    member typ_UInt64: ILType
    member typ_Single: ILType
    member typ_Double: ILType
    member typ_Bool: ILType
    member typ_Char: ILType


/// Build the table of commonly used references given functions to find types in system assemblies
val mkILGlobals: ILScopeRef -> ILGlobals

val EcmaMscorlibILGlobals: ILGlobals

/// When writing a binary the fake "toplevel" type definition (called <Module>)
/// must come first. This function puts it first, and creates it in the returned 
/// list as an empty typedef if it doesn't already exist.
val destTypeDefsWithGlobalFunctionsFirst: ILGlobals -> ITypeDefs -> ITypeDef list

/// Not all custom attribute data can be decoded without binding types.  In particular 
/// enums must be bound in order to discover the size of the underlying integer. 
/// The following assumes enums have size int32. 
val decodeILAttribData: 
    ILGlobals -> 
    ILAttribute -> 
      ILAttribElem list *  (* fixed args *)
      ILAttributeNamedArg list (* named args: values and flags indicating if they are fields or properties *) 

/// Generate simple references to assemblies and modules.
val mkSimpleAssRef: string -> ILAssemblyRef

val mkSimpleModRef: string -> ILModuleRef

val mkILTyvarTy: uint16 -> ILType

/// Make type refs.
val mkILNestedTyRef: ILScopeRef * string list * string -> ILTypeRef
val mkILTyRef: ILScopeRef * string -> ILTypeRef
val mkILTyRefInTyRef: ILTypeRef * string -> ILTypeRef

type ILGenericArgsList = ILType list

/// Make type specs.
val mkILNonGenericTySpec: ILTypeRef -> ILTypeSpec
val mkILTySpec: ILTypeRef * ILGenericArgsList -> ILTypeSpec

/// Make types.
val mkILTy: ILBoxity -> ILTypeSpec -> ILType
val mkILNamedTy: ILBoxity -> ILTypeRef -> ILGenericArgsList -> ILType
val mkILBoxedTy: ILTypeRef -> ILGenericArgsList -> ILType
val mkILValueTy: ILTypeRef -> ILGenericArgsList -> ILType
val mkILNonGenericBoxedTy: ILTypeRef -> ILType
val mkILNonGenericValueTy: ILTypeRef -> ILType
val mkILArrTy: ILType * ILArrayShape -> ILType
val mkILArr1DTy: ILType -> ILType
val isILArrTy: ILType -> bool
val destILArrTy: ILType -> ILArrayShape * ILType 
val mkILBoxedType: ILTypeSpec -> ILType

/// Make method references and specs.
val mkILMethRef: ILTypeRef * ILCallingConv * string * int * ILType list * ILType -> ILMethodRef
val mkILMethSpec: ILMethodRef * ILBoxity * ILGenericArgsList * ILGenericArgsList -> ILMethodSpec
val mkILMethSpecForMethRefInTy: ILMethodRef * ILType * ILGenericArgsList -> ILMethodSpec
val mkILMethSpecInTy: ILType * ILCallingConv * string * ILType list * ILType * ILGenericArgsList -> ILMethodSpec

/// Construct references to methods on a given type .
val mkILNonGenericMethSpecInTy: ILType * ILCallingConv * string * ILType list * ILType -> ILMethodSpec

/// Construct references to instance methods.
val mkILInstanceMethSpecInTy: ILType * string * ILType list * ILType * ILGenericArgsList -> ILMethodSpec

/// Construct references to instance methods.
val mkILNonGenericInstanceMethSpecInTy: ILType * string * ILType list * ILType -> ILMethodSpec

/// Construct references to static methods.
val mkILStaticMethSpecInTy: ILType * string * ILType list * ILType * ILGenericArgsList -> ILMethodSpec

/// Construct references to static, non-generic methods.
val mkILNonGenericStaticMethSpecInTy: ILType * string * ILType list * ILType -> ILMethodSpec

/// Construct references to constructors.
val mkILCtorMethSpecForTy: ILType * ILType list -> ILMethodSpec

/// Construct references to fields.
val mkILFieldRef: ILTypeRef * string * ILType -> ILFieldRef
val mkILFieldSpec: ILFieldRef * ILType -> ILFieldSpec
val mkILFieldSpecInTy: ILType * string * ILType -> ILFieldSpec

val mkILCallSig: ILCallingConv * ILType list * ILType -> ILCallingSignature

/// Make generalized versions of possibly-generic types, e.g. Given the ILTypeDef for List, return the type "List<T>".
val mkILFormalBoxedTy: ILTypeRef -> IGenericParameterDef list -> ILType
val mkILFormalNamedTy: ILBoxity -> ILTypeRef -> IGenericParameterDef list -> ILType

val mkILFormalTypars: ILType list -> ILGenericParameterDefs
val mkILFormalGenericArgs: int -> ILGenericParameterDefs -> ILGenericArgsList
val mkILSimpleTypar: string -> IGenericParameterDef

/// Make custom attributes.
val mkILCustomAttribMethRef: 
    ILGlobals 
    -> ILMethodSpec 
       * ILAttribElem list (* fixed args: values and implicit types *) 
       * ILAttributeNamedArg list (* named args: values and flags indicating if they are fields or properties *) 
      -> ILAttribute

val mkILCustomAttribute: 
    ILGlobals 
    -> ILTypeRef * ILType list * 
       ILAttribElem list (* fixed args: values and implicit types *) * 
       ILAttributeNamedArg list (* named args: values and flags indicating if they are fields or properties *) 
         -> ILAttribute

val mkPermissionSet: ILGlobals -> ILSecurityAction * (ILTypeRef * (string * ILType * ILAttribElem) list) list -> ILSecurityDecl

/// Making code.
val generateCodeLabel: unit -> ILCodeLabel
val formatCodeLabel: ILCodeLabel -> string

/// Make some code that is a straight line sequence of instructions. 
/// The function will add a "return" if the last instruction is not an exiting instruction.
val nonBranchingInstrsToCode: ILInstr list -> ILCode 

/// Helpers for codegen: scopes for allocating new temporary variables.
type ILLocalsAllocator =
    new: preAlloc: int -> ILLocalsAllocator
    member AllocLocal: ILLocal -> uint16
    member Close: unit -> ILLocal list

/// Derived functions for making some common patterns of instructions.
val mkNormalCall: ILMethodSpec -> ILInstr
val mkNormalCallvirt: ILMethodSpec -> ILInstr
val mkNormalCallconstraint: ILType * ILMethodSpec -> ILInstr
val mkNormalNewobj: ILMethodSpec -> ILInstr
val mkCallBaseConstructor: ILType * ILType list -> ILInstr list
val mkNormalStfld: ILFieldSpec -> ILInstr
val mkNormalStsfld: ILFieldSpec -> ILInstr
val mkNormalLdsfld: ILFieldSpec -> ILInstr
val mkNormalLdfld: ILFieldSpec -> ILInstr
val mkNormalLdflda: ILFieldSpec -> ILInstr
val mkNormalLdobj: ILType -> ILInstr
val mkNormalStobj: ILType -> ILInstr 
val mkLdcInt32: int32 -> ILInstr
val mkLdarg0: ILInstr
val mkLdloc: uint16 -> ILInstr
val mkStloc: uint16 -> ILInstr
val mkLdarg: uint16 -> ILInstr

val andTailness: ILTailcall -> bool -> ILTailcall

/// Derived functions for making return, parameter and local variable
/// objects for use in method definitions.
val mkILParam: string option * ILType -> IParameter
val mkILParamAnon: ILType -> IParameter
val mkILParamNamed: string * ILType -> IParameter
val mkILReturn: ILType -> ILReturn
val mkILLocal: ILType -> (string * int * int) option -> ILLocal

/// Make a formal generic parameters.
val mkILEmptyGenericParams: ILGenericParameterDefs

/// Make method definitions.
val mkILMethodBody: initlocals:bool * ILLocals * int * ILCode * ILSourceMarker option -> ILMethodBody
val mkMethodBody: bool * ILLocals * int * ILCode * ILSourceMarker option -> MethodBody
val methBodyNotAvailable: ILLazyMethodBody 
val methBodyAbstract: ILLazyMethodBody 
val methBodyNative: ILLazyMethodBody 

val mkILCtor: ILMemberAccess * IParameter list * MethodBody -> IMethodDef
val mkILClassCtor: MethodBody -> IMethodDef
val mkILNonGenericEmptyCtor: ILSourceMarker option -> ILType -> IMethodDef
val mkILStaticMethod: ILGenericParameterDefs * string * ILMemberAccess * IParameter list * ILReturn * MethodBody -> IMethodDef
val mkILNonGenericStaticMethod: string * ILMemberAccess * IParameter list * ILReturn * MethodBody -> IMethodDef
val mkILGenericVirtualMethod: string * ILMemberAccess  * ILGenericParameterDefs * IParameter list * ILReturn * MethodBody -> IMethodDef
val mkILGenericNonVirtualMethod: string * ILMemberAccess  * ILGenericParameterDefs * IParameter list * ILReturn * MethodBody -> IMethodDef
val mkILNonGenericVirtualMethod: string * ILMemberAccess * IParameter list * ILReturn * MethodBody -> IMethodDef
val mkILNonGenericInstanceMethod: string * ILMemberAccess  * IParameter list * ILReturn * MethodBody -> IMethodDef


/// Make field definitions.
val mkILInstanceField: string * ILType * ILFieldInit option * ILMemberAccess -> IFieldDef
val mkILStaticField: string * ILType * ILFieldInit option * byte[] option * ILMemberAccess -> IFieldDef
val mkILLiteralField: string * ILType * ILFieldInit * byte[] option * ILMemberAccess -> IFieldDef

/// Make a type definition.
val mkILGenericClass: string * ILTypeDefAccess * ILGenericParameterDefs * ILType * ILType list * IMethodDefs * IFieldDefs * ITypeDefs * IPropertyDefs * IEventDefs * ILAttributes * ILTypeInit -> ITypeDef
val mkILSimpleClass: ILGlobals -> string * ILTypeDefAccess * IMethodDefs * IFieldDefs * ITypeDefs * IPropertyDefs * IEventDefs * ILAttributes * ILTypeInit  -> ITypeDef
val mkILTypeDefForGlobalFunctions: ILGlobals -> IMethodDefs * IFieldDefs -> ITypeDef

/// Make a type definition for a value type used to point to raw data.
/// These are useful when generating array initialization code 
/// according to the 
///   ldtoken    field valuetype '<PrivateImplementationDetails>'/'$$struct0x6000127-1' '<PrivateImplementationDetails>'::'$$method0x6000127-1'
///   call       void System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class System.Array,valuetype System.RuntimeFieldHandle)
/// idiom.
val mkRawDataValueTypeDef:  ILType -> string * size:int32 * pack:uint16 -> ITypeDef

/// Injecting code into existing code blocks.  A branch will
/// be added from the given instructions to the (unique) entry of
/// the code, and the first instruction will be the new entry
/// of the method.  The instructions should be non-branching.

val prependInstrsToCode: ILInstr list -> ILCode -> ILCode
val prependInstrsToMethod: ILInstr list -> IMethodDef -> IMethodDef

/// Injecting initialization code into a class.
/// Add some code to the end of the .cctor for a type.  Create a .cctor
/// if one doesn't exist already.
val prependInstrsToClassCtor: ILInstr list -> ILSourceMarker option -> ITypeDef -> ITypeDef

/// Derived functions for making some simple constructors
val mkILStorageCtor: ILSourceMarker option * ILInstr list * ILType * (string * ILType) list * ILMemberAccess -> IMethodDef
val mkILSimpleStorageCtor: ILSourceMarker option * ILTypeSpec option * ILType * IParameter list * (string * ILType) list * ILMemberAccess -> IMethodDef
val mkILSimpleStorageCtorWithParamNames: ILSourceMarker option * ILTypeSpec option * ILType * IParameter list * (string * string * ILType) list * ILMemberAccess -> IMethodDef

val mkILDelegateMethods: ILMemberAccess -> ILGlobals -> ILType * ILType -> IParameter list * ILReturn -> IMethodDef list

/// Given a delegate type definition which lies in a particular scope, 
/// make a reference to its constructor.
val mkCtorMethSpecForDelegate: ILGlobals -> ILType * bool -> ILMethodSpec 

/// The toplevel "class" for a module or assembly.
val mkILTypeForGlobalFunctions: ILScopeRef -> ILType

/// Making tables of custom attributes, etc.
val mkILCustomAttrs: ILAttribute list -> ILAttributes
val mkILCustomAttrsFromArray: ILAttribute[] -> ILAttributes
val storeILCustomAttrs: ILAttributes -> ILAttributesStored
val mkILCustomAttrsReader: (int32 -> ILAttribute[]) -> ILAttributesStored
val emptyILCustomAttrs: ILAttributes

val mkILSecurityDecls: ILSecurityDecl list -> ILSecurityDecls
val emptyILSecurityDecls: ILSecurityDecls
val emptyILSecurityDeclsStored: ILSecurityDeclsStored
val storeILSecurityDecls: ILSecurityDecls -> ILSecurityDeclsStored
val mkILSecurityDeclsReader: (int32 -> ILSecurityDecl[]) -> ILSecurityDeclsStored

val mkMethBodyAux: MethodBody -> ILLazyMethodBody
val mkMethBodyLazyAux: Lazy<MethodBody> -> ILLazyMethodBody

val mkILEvents: IEventDef list -> IEventDefs
val mkILEventsLazy: Lazy<IEventDef list> -> IEventDefs
val emptyILEvents: IEventDefs

val mkILProperties: IPropertyDef list -> IPropertyDefs
val mkILPropertiesLazy: Lazy<IPropertyDef list> -> IPropertyDefs
val emptyILProperties: IPropertyDefs

val mkILMethods: IMethodDef list -> IMethodDefs
val mkILMethodsFromArray: IMethodDef[] -> IMethodDefs
val mkILMethodsComputed: (unit -> IMethodDef[]) -> IMethodDefs
val emptyILMethods: IMethodDefs

val mkILFields: IFieldDef list -> IFieldDefs
val mkILFieldsLazy: Lazy<IFieldDef list> -> IFieldDefs
val emptyILFields: IFieldDefs

val mkILMethodImpls: ILMethodImplDef list -> ILMethodImplDefs
val mkILMethodImplsLazy: Lazy<ILMethodImplDef list> -> ILMethodImplDefs
val emptyILMethodImpls: ILMethodImplDefs

val mkILTypeDefs: ITypeDef list -> ITypeDefs
val mkILTypeDefsFromArray: ITypeDef[] -> ITypeDefs
val emptyILTypeDefs: ITypeDefs

/// Create table of types which is loaded/computed on-demand, and whose individual 
/// elements are also loaded/computed on-demand. Any call to tdefs.AsList will 
/// result in the laziness being forced.  Operations can examine the
/// custom attributes and name of each type in order to decide whether
/// to proceed with examining the other details of the type.
/// 
/// Note that individual type definitions may contain further delays 
/// in their method, field and other tables. 
val mkILTypeDefsComputed: (unit -> IPreTypeDef[]) -> ITypeDefs
val addILTypeDef: ITypeDef -> ITypeDefs -> ITypeDefs

val mkTypeForwarder: ILScopeRef -> string -> ILNestedExportedTypes -> ILAttributes -> ILTypeDefAccess -> ILExportedTypeOrForwarder
val mkILNestedExportedTypes: ILNestedExportedType list -> ILNestedExportedTypes
val mkILNestedExportedTypesLazy: Lazy<ILNestedExportedType list> -> ILNestedExportedTypes

val mkILExportedTypes: ILExportedTypeOrForwarder list -> ILExportedTypesAndForwarders
val mkILExportedTypesLazy: Lazy<ILExportedTypeOrForwarder list> ->   ILExportedTypesAndForwarders

val mkILResources: ILResource list -> ILResources

/// Making modules.
val mkILSimpleModule: assemblyName:string -> moduleName:string -> dll:bool -> subsystemVersion: (int * int) -> useHighEntropyVA: bool -> ITypeDefs -> int32 option -> string option -> int -> ILExportedTypesAndForwarders -> string -> IModuleDef

/// Generate references to existing type definitions, method definitions
/// etc.  Useful for generating references, e.g. to a  class we're processing
/// Also used to reference type definitions that we've generated.  [ILScopeRef] 
/// is normally ILScopeRef.Local, unless we've generated the ILTypeDef in
/// an auxiliary module or are generating multiple assemblies at 
/// once.

val mkRefForNestedILTypeDef: ILScopeRef -> ITypeDef list * ITypeDef -> ILTypeRef
val mkRefForILMethod       : ILScopeRef -> ITypeDef list * ITypeDef -> IMethodDef -> ILMethodRef
val mkRefForILField       : ILScopeRef -> ITypeDef list * ITypeDef -> IFieldDef  -> ILFieldRef

val mkRefToILMethod: ILTypeRef * IMethodDef -> ILMethodRef
val mkRefToILField: ILTypeRef * IFieldDef -> ILFieldRef

val mkRefToILAssembly: IAssemblyManifest -> ILAssemblyRef
val mkRefToILModule: IModuleDef -> ILModuleRef

val NoMetadataIdx: int32

// -------------------------------------------------------------------- 
// Rescoping.
//
// Given an object O1 referenced from where1 (e.g. O1 binds to some  
// result R when referenced from where1), and given that SR2 resolves to where1 from where2, 
// produce a new O2 for use from where2 (e.g. O2 binds to R from where2)
//
// So, ILScopeRef tells you how to reference the original scope from 
// the new scope. e.g. if ILScopeRef is:
//    [ILScopeRef.Local] then the object is returned unchanged
//    [ILScopeRef.Module m] then an object is returned 
//                        where all ILScopeRef.Local references 
//                        become ILScopeRef.Module m
//    [ILScopeRef.Assembly m] then an object is returned 
//                         where all ILScopeRef.Local and ILScopeRef.Module references 
//                        become ILScopeRef.Assembly m
// -------------------------------------------------------------------- 

/// Rescoping. The first argument tells the function how to reference the original scope from 
/// the new scope. 
val rescopeILScopeRef: ILScopeRef -> ILScopeRef -> ILScopeRef

/// Rescoping. The first argument tells the function how to reference the original scope from 
/// the new scope. 
val rescopeILTypeSpec: ILScopeRef -> ILTypeSpec -> ILTypeSpec

/// Rescoping. The first argument tells the function how to reference the original scope from 
/// the new scope. 
val rescopeILType: ILScopeRef -> ILType -> ILType

/// Rescoping. The first argument tells the function how to reference the original scope from 
/// the new scope. 
val rescopeILMethodRef: ILScopeRef -> ILMethodRef -> ILMethodRef 

/// Rescoping. The first argument tells the function how to reference the original scope from 
/// the new scope. 
val rescopeILFieldRef: ILScopeRef -> ILFieldRef -> ILFieldRef

/// Unscoping. Clears every scope information, use for looking up IL method references only.
val unscopeILType: ILType -> ILType

val buildILCode: string -> lab2pc: Dictionary<ILCodeLabel,int> -> instrs:ILInstr[] -> ILExceptionSpec list -> ILLocalDebugInfo list -> ILCode

/// Instantiate type variables that occur within types and other items. 
val instILTypeAux: int -> ILGenericArgs -> ILType -> ILType

/// Instantiate type variables that occur within types and other items. 
val instILType: ILGenericArgs -> ILType -> ILType

/// This is a 'vendor neutral' way of referencing mscorlib. 
val ecmaPublicKey: PublicKey

/// Discriminating different important built-in types.
val isILObjectTy: ILType -> bool
val isILStringTy: ILType -> bool
val isILSByteTy: ILType -> bool
val isILByteTy: ILType -> bool
val isILInt16Ty: ILType -> bool
val isILUInt16Ty: ILType -> bool
val isILInt32Ty: ILType -> bool
val isILUInt32Ty: ILType -> bool
val isILInt64Ty: ILType -> bool
val isILUInt64Ty: ILType -> bool
val isILIntPtrTy: ILType -> bool
val isILUIntPtrTy: ILType -> bool
val isILBoolTy: ILType -> bool
val isILCharTy: ILType -> bool
val isILTypedReferenceTy: ILType -> bool
val isILDoubleTy: ILType -> bool
val isILSingleTy: ILType -> bool

/// Get a public key token from a public key.
val sha1HashBytes: byte[] -> byte[] (* SHA1 hash *)

/// Get a version number from a CLR version string, e.g. 1.0.3705.0
val parseILVersion: string -> ILVersionInfo
val formatILVersion: ILVersionInfo -> string
val compareILVersions: ILVersionInfo -> ILVersionInfo -> int

/// Decompose a type definition according to its kind.
type ILEnumInfo =
    { enumValues: (string * ILFieldInit) list  
      enumType: ILType }

val getTyOfILEnumInfo: ILEnumInfo -> ILType

val computeILEnumInfo: string * IFieldDefs -> ILEnumInfo

/// A utility type provided for completeness
[<Sealed>]
type ILEventRef =
    static member Create: ILTypeRef * string -> ILEventRef
    member DeclaringTypeRef: ILTypeRef
    member Name: string

/// A utility type provided for completeness
[<Sealed>]
type ILPropertyRef =
     static member Create: ILTypeRef * string -> ILPropertyRef
     member DeclaringTypeRef: ILTypeRef
     member Name: string
     interface System.IComparable

val runningOnMono: bool

type ILReferences = 
    { AssemblyReferences: ILAssemblyRef list 
      ModuleReferences: ILModuleRef list }

/// Find the full set of assemblies referenced by a module.
val computeILRefs: IModuleDef -> ILReferences
val emptyILRefs: ILReferences

