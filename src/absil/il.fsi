// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// The "unlinked" view of .NET metadata and code.  Central to 
///  to Abstract IL library
module internal Microsoft.FSharp.Compiler.AbstractIL.IL 

open Internal.Utilities
open System.Collections.Generic

/// The type used to store relatively small lists in the Abstract IL data structures, i.e. for ILTypes, ILGenericArgs, ILParameters and ILLocals.
/// See comments in il.fs for why we've isolated this representation and the possible future choices we might use here.
#if ABSIL_USES_ARRAY_FOR_ILLIST
type ILList<'T> = 'T []
#endif

#if ABSIL_USES_THREELIST_FOR_ILLIST
type ILList<'T> = ThreeList<'T>
#endif

//#if ABSIL_USES_LIST_FOR_ILLIST
type ILList<'T> = 'T list
//#endif

type PrimaryAssembly = 
    | Mscorlib
    | DotNetCore

    member Name: string

// ====================================================================
// .NET binaries can be converted to the data structures below by using 
// the functions in the "Ilread" module. 
//
// Constituent types are listed in ascending order of complexity, 
// all the way up to the type ILModuleDef, representing the read of an IL 
// assembly (.dll or .exe), or part of a multi-module assembly.  Types are 
// often specified via a concrete representation for the type (e.g. a record), 
// though some types are abstract. 
//
// The second part of the file (after the definition of all the types) 
// specifies a large set of utilities for building objects belonging to 
// the types.  You will only need to become familiar with these if you 
// are transforming code or writing a code-generating compiler.
// 
// Several other utilities are also defined in this file:
//   1. A code builder for turning linear sequences of instructions 
//      augmented with exception tables into the more structured 
//      format used for code.  
//
//   2. The "typ_XYZ", "tspec_XYZ" and "mspec_XYZ" values which 
//      can be used to reference types in the "primary assembly (either System.Runtime or mscorlib)" assembly.
//
//   3. The "rescopeXYZ" functions which can be used to lift a piece of
//      metadata from one assembly and transform it to a piece of metadata
//      suitable for use from another assembly.  The transformation adjusts
//      references in the metadata to take into account the assembly
//      where the metadata will now be located.
//
//   4. The "instantiateXYZ" utilities to replace type variables
//      by types.  These are associated with generics.
//
//   5. The "intern_XYZ" tables for reducing the memory used by 
//      generated constructs.
//
//   6. The "refs_of_XYZ" utilities for finding all the assemblies 
//      referenced by a module.
//
//   7. A somewhat obscure facility to allow new instructions and types
//      to be added to the   This is only used by ILX.
// ==================================================================== 

// Guids (Note: consider adjusting these to the System.Guid type)
type Guid = byte[]

[<StructuralEquality; StructuralComparison>]
type ILPlatform = 
    | X86
    | AMD64
    | IA64

/// Debug info.  Values of type "source" can be attached at sequence 
/// points and some other locations. 
[<Sealed>]
type ILSourceDocument =
    static member Create : language: Guid option * vendor: Guid option * documentType: Guid option * file: string -> ILSourceDocument
    member Language: Guid option
    member Vendor: Guid option
    member DocumentType: Guid option
    member File: string


[<Sealed>]
type ILSourceMarker =
    static member Create : document: ILSourceDocument * line: int * column: int * endLine:int * endColumn: int-> ILSourceMarker
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
    static member Create : name: string * hash: byte[] option * publicKey: PublicKey option * retargetable: bool * version: ILVersionInfo option * locale: string option -> ILAssemblyRef
    static member FromAssemblyName : System.Reflection.AssemblyName -> ILAssemblyRef
    member Name: string;
    /// The fully qualified name of the assembly reference, e.g. mscorlib, Version=1.0.3705 etc.
    member QualifiedName: string; 
    member Hash: byte[] option;
    member PublicKey: PublicKey option;
    /// CLI says this indicates if the assembly can be retargeted (at runtime) to be from a different publisher. 
    member Retargetable: bool;
    member Version: ILVersionInfo option;
    member Locale: string option
    interface System.IComparable

[<Sealed>]
type ILModuleRef =
    static member Create : name: string * hasMetadata: bool * hash: byte[] option -> ILModuleRef
    member Name: string
    member HasMetadata: bool
    member Hash: byte[] option
    interface System.IComparable

// Scope references
//
// Scope references are the bits of metadata attached to type names
// that indicate where a type can be found. CIL has three 
// kinds: local, module and assembly references:
//   o Local: the type must reside in the same module as the scope reference
//   o Module: the type must reside in the indicated module in the same
//     assembly as the scope reference
//   o Assembly: The type must reside in the indicated assembly.
//     These have no implicit context. Assembly references can end up 
//     binding to the assembly containing the reference, i.e. 
//     may be self or mutually referential.
//
//     Assembly reference may also resolve to type in an 
//     auxiliary module of an assembly when the assembly 
//     has an "exported types" (here called "classes elsewhere") table.
//
// We represent these references by values embedded within type
// references.  These values are usually "shared" across the data
// structures for a module, i.e. one such value is created for each
// assembly or module reference, and this value is reused within each
// type object.
//
// Note that as with method references the term structure is not 
// _linked_, i.e. a "ILScopeRef" is still a _reference_ to a scope, 
// not the scope itself.  Because the structure is not linked, 
// the Abstract IL toolset does not require 
// strongly connected inputs: you can manipulate an assembly
// without loading all its dependent assemblies.  This is the primary
// difference between Abstract IL and Reflection, and it can be both
// a blessing and a curse depending on the kind of manipulation you
// wish to perform.
//
// Similarly, you can manipulate individual modules within
// an assembly without having the whole assembly loaded.  (But note that
// most assemblies are single-module in any case).
//
// [ILScopeRef]'s _cannot_ be compared for equality in the way that
// might be expected, in these sense that two ILScopeRef's may 
// resolve to the same assembly/module even though they are not equal.  
//
//   Aside: People have suggested normalizing all scope references
//          so that this would be possible, and early versions of this
//          toolkit did this.  However, this meant that in order to load
//          each module you had to tell the toolkit which assembly it belonged to.
//          Furthermore, you had to know the exact resolved details of 
//          each assembly the module refers to.  This is
//          effectively like having a "fully-linked" view of the graph
//          of assemblies, like that provided in the Ilbind module.  This is really problematic for compile-time tools,
//          as, for example, the policy for linking at the runtime-machine
//          may actually alter the results of linking.  If such compile-time
//          assumptions are to be made then the tool built on top
//          of the toolkit rather than the toolkit itself should
//          make them.
//
// Scope references, type references, field references and method references
// can be "bound" to particular assemblies using the functions in "Ilbind".  
// This simulates the resolution/binding process performed by a Common Language
// Runtime during execution.  Various tests and derived operations
// can then be performed on the results of binding.  
[<StructuralEquality; StructuralComparison>]
[<RequireQualifiedAccess>]
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
    member IsInstance : bool
    member IsInstanceExplicit : bool
    member IsStatic : bool
    member ThisConv : ILThisConvention
    member BasicConv : ILArgConvention
    static member Instance : ILCallingConv
    static member Static   : ILCallingConv

/// Array shapes. For most purposes, including verification, the
/// rank is the only thing that matters.
 
type ILArrayBound = int32 option 
type ILArrayBounds = ILArrayBound * ILArrayBound

[<StructuralEquality; StructuralComparison>]
type ILArrayShape =
    | ILArrayShape of ILArrayBounds list // lobound/size pairs 
    member Rank : int
    /// Bounds for a single dimensional, zero based array 
    static member SingleDimensional: ILArrayShape
    static member FromRank : int -> ILArrayShape

[<StructuralEquality; StructuralComparison>]
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
    static member Create : scope: ILScopeRef * enclosing: string list * name: string -> ILTypeRef

    /// Where is the type, i.e. is it in this module, in another module in this assembly or in another assembly? 
    member Scope: ILScopeRef
    /// The list of enclosing type names for a nested type. If non-nil then the first of these also contains the namespace.
    member Enclosing: string list
    /// The name of the type. This also contains the namespace if Enclosing is empty.
    member Name: string
    /// The name of the type in the assembly using the '.' notation for nested types.
    member FullName: string
    /// The name of the type in the assembly using the '+' notation for nested types.
    member BasicQualifiedName : string
    member QualifiedName: string
#if EXTENSIONTYPING
    member QualifiedNameWithNoShortPrimaryAssembly: string
#endif
    interface System.IComparable
    
/// Type specs and types.  
///
/// These are the types that appear syntactically in .NET binaries.  
///
/// Generic type definitions must be combined with
/// an instantiation to form a type.  Throughout this file, 
/// a "ref" refers to something that is uninstantiated, and
/// a "spec" to a ref that is combined with the relevant instantiations.
 
[<Sealed>]
type ILTypeSpec =
    static member Create : typeRef:ILTypeRef * instantiation:ILGenericArgs -> ILTypeSpec

    /// Which type is being referred to?
    member TypeRef: ILTypeRef
    /// The type instantiation if the type is generic, otherwise empty
    member GenericArgs: ILGenericArgs
    member Scope: ILScopeRef
    member Enclosing: string list
    member Name: string
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
    member TypeSpec : ILTypeSpec
    member Boxity : ILBoxity
    member TypeRef : ILTypeRef
    member IsNominal : bool
    member GenericArgs : ILGenericArgs
    member IsTyvar : bool
    member BasicQualifiedName : string
    member QualifiedNameWithNoShortPrimaryAssembly : string

and [<StructuralEquality; StructuralComparison>]
    ILCallingSignature =  
    { CallingConv: ILCallingConv;
      ArgTypes: ILTypes;
      ReturnType: ILType }

/// Actual generic parameters are  always types.  


and ILGenericArgs = ILList<ILType>
and ILTypes = ILList<ILType>


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ILList = 
    val inline map : ('T -> 'U) -> ILList<'T> -> ILList<'U>
    val inline mapi : (int -> 'T -> 'U) -> ILList<'T> -> ILList<'U>
    val inline isEmpty : ILList<'T> -> bool
    val inline toList : ILList<'T> -> 'T list
    val inline ofList : 'T list -> ILList<'T> 
    val inline lengthsEqAndForall2 : ('T -> 'U -> bool) -> ILList<'T> -> ILList<'U> -> bool
    val inline init : int -> (int -> 'T) -> ILList<'T>
    val inline empty<'T> : ILList<'T>
    val inline toArray : ILList<'T> -> 'T[]
    val inline ofArray : 'T[] -> ILList<'T>
    val inline nth : ILList<'T> -> int -> 'T
    val inline iter : ('T -> unit) -> ILList<'T> -> unit
    val inline iteri : (int -> 'T -> unit) -> ILList<'T> -> unit
    val inline foldBack : ('T -> 'State -> 'State) -> ILList<'T> -> 'State -> 'State
    val inline exists : ('T -> bool) -> ILList<'T> -> bool


/// Formal identities of methods.  Method refs refer to methods on 
/// named types.  In general you should work with ILMethodSpec objects
/// rather than MethodRef objects, because ILMethodSpec objects carry
/// information about how generic methods are instantiated.  MethodRef
/// objects are only used at a few places in the Abstract IL syntax
/// and if analyzing or generating IL you will be unlikely to come across
/// these.

[<Sealed>]
type ILMethodRef =
     static member Create : enclosingTypeRef: ILTypeRef * callingConv: ILCallingConv * name: string * genericArity: int * argTypes: ILTypes * returnType: ILType -> ILMethodRef
     member EnclosingTypeRef: ILTypeRef
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
    { EnclosingTypeRef: ILTypeRef;
      Name: string;
      Type: ILType }

/// The information at the callsite of a method
//
// A ILMethodSpec is everything given at the callsite (apart from whether the call is a tailcall and whether it is passing
// varargs - see the instruction set below).  It is made up of: 
//   1) a (possibly generic) ILMethodRef
//   2) a "usage type" that indicates the how the type containing the declaration is being used (as
//      a value class, a boxed value class, an instantiated generic class or whatever - see below)
//   3) an instantiation in the case where the method is generic.
//
// In this unbound form of the metadata, the enclosing type may be ILType.Boxed even when the member is a member of a value type or
// enumeration.  This is because the binary format of the metadata does not carry enough information in a MemberRefParent to determine
// from the binary alone whether the enclosing type is a value type or not.

[<Sealed>]
type ILMethodSpec =
     static member Create : ILType * ILMethodRef * ILGenericArgs -> ILMethodSpec
     member MethodRef: ILMethodRef
     member EnclosingType: ILType 
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
    { FieldRef: ILFieldRef;
      EnclosingType: ILType }    
    member EnclosingTypeRef: ILTypeRef
    member Name: string
    member FormalType: ILType
    member ActualType : ILType

/// ILCode labels.  In structured code each code label
/// refers to a basic block somewhere in the code of the method.

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
///
/// In general we don't categorize instructions, as different 
/// instruction groups are relevant for different types of operations. 
/// However we do collect the branch and compare instructions together 
/// because they all take an address, and the ILArithInstr ones because 
/// none of them take any direct arguments. 
[<StructuralEquality; NoComparison>]
type ILInstr = 
    // Basic 
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

    // Local aggregates, i.e. stack allocated data (alloca) : C++ only 
    | I_localloc
    | I_cpblk of ILAlignment * ILVolatility
    | I_initblk of ILAlignment  * ILVolatility

    // EXTENSIONS, e.g. MS-ILX 
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
    { Range: (ILCodeLabel * ILCodeLabel);
      Clause: ILExceptionClause }

/// Indicates that a particular local variable has a particular source 
/// language name within a given set of ranges. This does not effect local 
/// variable numbering, which is global over the whole method. 
[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILLocalDebugMapping =
    { LocalIndex: int;
      LocalName: string; }

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
    | Custom of Guid * string * string * byte[] (* guid,nativeTypeName,custMarshallerName,cookieString *)
    | FixedSysString of int32
    | FixedArray of int32
    | Currency
    | LPSTR
    | LPWSTR
    | LPTSTR
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
    | Array of ILNativeType option * (int32 * int32 option) option (* optional idx of parameter giving size plus optional additive i.e. num elems *)
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
    { Type: ILType;
      IsPinned: bool;
      DebugInfo: (string * int * int) option }
     

type ILLocals = ILList<ILLocal>

/// IL method bodies
[<RequireQualifiedAccess; NoComparison; NoEquality>]
type ILMethodBody = 
    { IsZeroInit: bool;
      /// strictly speakin should be a uint16 
      MaxStack: int32; 
      NoInlining: bool;
      Locals: ILLocals;
      Code: ILCode;
      SourceMarker: ILSourceMarker option }

/// Member Access
[<RequireQualifiedAccess>]
type ILMemberAccess = 
    | Assembly
    | CompilerControlled
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
    { Method: ILMethodSpec;
      Data: byte[] }

[<NoEquality; NoComparison; Sealed>]
type ILAttributes =
    member AsList : ILAttribute list

/// Method parameters and return values.

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILParameter = 
    { Name: string option;
      Type: ILType;
      Default: ILFieldInit option;  
      /// Marshalling map for parameters. COM Interop only. 
      Marshal: ILNativeType option; 
      IsIn: bool;
      IsOut: bool;
      IsOptional: bool;
      CustomAttrs: ILAttributes }

type ILParameters = ILList<ILParameter>

val typesOfILParamsRaw : ILParameters -> ILTypes
val typesOfILParamsList : ILParameter list -> ILType list

/// Method return values.
[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILReturn = 
    { Marshal: ILNativeType option;
      Type: ILType; 
      CustomAttrs: ILAttributes }

/// Security ILPermissions
/// Attached to various structures...
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

type ILPermission =
    | PermissionSet of ILSecurityAction * byte[]

/// Abstract type equivalent to ILPermission list - use helpers 
/// below to construct/destruct these.
[<NoComparison; NoEquality; Sealed>]
type ILPermissions =
    member AsList : ILPermission list

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
    { Where: ILModuleRef;
      Name: string;
      CallingConv: PInvokeCallingConvention;
      CharEncoding: PInvokeCharEncoding;
      NoMangle: bool;
      LastError: bool;
      ThrowOnUnmappableChar: PInvokeThrowOnUnmappableChar;
      CharBestFit: PInvokeCharBestFit }


/// [OverridesSpec] - refer to a method declaration in a superclass 
/// or superinterface. Used for overriding/method impls.  Includes
/// a type for the parent for the same reason that a method specs
/// includes the type of the enclosing type, i.e. the type
/// gives the "ILGenericArgs" at which the parent type is being used.

type ILOverridesSpec =
    | OverridesSpec of ILMethodRef * ILType
    member MethodRef: ILMethodRef
    member EnclosingType: ILType 

// REVIEW: fold this into ILMethodDef.
type ILMethodVirtualInfo =
    { IsFinal: bool; 
      IsNewSlot: bool; 
      IsCheckAccessOnOverride: bool;
      IsAbstract: bool; }

[<RequireQualifiedAccess>]
type MethodKind =
    | Static 
    | Cctor 
    | Ctor 
    | NonVirtual 
    | Virtual of ILMethodVirtualInfo

// REVIEW: fold this into ILMethodDef.
[<RequireQualifiedAccess>]
type MethodBody =
    | IL of ILMethodBody
    | PInvoke of PInvokeMethod       (* platform invoke to native  *)
    | Abstract
    | Native

// REVIEW: fold this into ILMethodDef.
[<RequireQualifiedAccess>]
type MethodCodeKind =
    | IL
    | Native
    | Runtime

/// Generic parameters.  Formal generic parameter declarations
/// may include the bounds, if any, on the generic parameter.
type ILGenericParameterDef =
    { Name: string;
    /// At most one is the parent type, the others are interface types.
      Constraints: ILTypes; 
      /// Variance of type parameters, only applicable to generic parameters for generic interfaces and delegates.
      Variance: ILGenericVariance; 
      /// Indicates the type argument must be a reference type.
      HasReferenceTypeConstraint: bool;     
      CustomAttrs : ILAttributes;
      /// Indicates the type argument must be a value type, but not Nullable.
      HasNotNullableValueTypeConstraint: bool;  
      /// Indicates the type argument must have a public nullary constructor.
      HasDefaultConstructorConstraint: bool; }


type ILGenericParameterDefs = ILGenericParameterDef list

[<NoComparison; NoEquality; Sealed>]
type ILLazyMethodBody = 
    member Contents : MethodBody 

/// Method definitions.
///
/// There are several different flavours of methods (constructors,
/// abstract, virtual, static, instance, class constructors).  There
/// is no perfect factorization of these as the combinations are not
/// independent.  

[<NoComparison; NoEquality>]
type ILMethodDef = 
    { Name: string;
      mdKind: MethodKind;
      CallingConv: ILCallingConv;
      Parameters: ILParameters;
      Return: ILReturn;
      Access: ILMemberAccess;
      mdBody: ILLazyMethodBody;   
      mdCodeKind: MethodCodeKind;   
      IsInternalCall: bool;
      IsManaged: bool;
      IsForwardRef: bool;
      SecurityDecls: ILPermissions;
      /// Some methods are marked "HasSecurity" even if there are no permissions attached, e.g. if they use SuppressUnmanagedCodeSecurityAttribute 
      HasSecurity: bool; 
      IsEntryPoint:bool;
      IsReqSecObj: bool;
      IsHideBySig: bool;
      IsSpecialName: bool;
      /// The method is exported to unmanaged code using COM interop.
      IsUnmanagedExport: bool; 
      IsSynchronized: bool;
      IsPreserveSig: bool;
      /// .NET 2.0 feature: SafeHandle finalizer must be run.
      IsMustRun: bool; 
      IsNoInline: bool;
     
      GenericParams: ILGenericParameterDefs;
      CustomAttrs: ILAttributes; }
      
    member ParameterTypes: ILTypes;
    member IsIL : bool
    member Code : ILCode option
    member Locals : ILLocals
    member MaxStack : int32
    member IsZeroInit : bool
    
    /// .cctor methods.  The predicates (IsClassInitializer,IsConstructor,IsStatic,IsNonVirtualInstance,IsVirtual) 
    /// form a complete, non-overlapping classification of this type.
    member IsClassInitializer: bool
    /// .ctor methods.  The predicates (IsClassInitializer,IsConstructor,IsStatic,IsNonVirtualInstance,IsVirtual) 
    /// form a complete, non-overlapping classification of this type.
    member IsConstructor: bool
    /// static methods.  The predicates (IsClassInitializer,IsConstructor,IsStatic,IsNonVirtualInstance,IsVirtual) 
    /// form a complete, non-overlapping classification of this type.
    member IsStatic: bool
    /// instance methods that are not virtual.  The predicates (IsClassInitializer,IsConstructor,IsStatic,IsNonVirtualInstance,IsVirtual) 
    /// form a complete, non-overlapping classification of this type.
    member IsNonVirtualInstance: bool
    /// instance methods that are virtual or abstract or implement an interface slot.  
    /// The predicates (IsClassInitializer,IsConstructor,IsStatic,IsNonVirtualInstance,IsVirtual) 
    /// form a complete, non-overlapping classification of this type.
    member IsVirtual: bool
    
    member IsFinal: bool
    member IsNewSlot: bool
    member IsCheckAccessOnOverride : bool
    member IsAbstract: bool
    member MethodBody : ILMethodBody
    member CallingSignature: ILCallingSignature

/// Tables of methods.  Logically equivalent to a list of methods but
/// the table is kept in a form optimized for looking up methods by 
/// name and arity.

/// abstract type equivalent to [ILMethodDef list] 
[<NoEquality; NoComparison; Sealed>]
type ILMethodDefs =
    interface IEnumerable<ILMethodDef>
    member AsArray : ILMethodDef[]
    member AsList : ILMethodDef list
    member FindByName : string -> ILMethodDef list

/// Field definitions.
[<NoComparison; NoEquality>]
type ILFieldDef = 
    { Name: string;
      Type: ILType;
      IsStatic: bool;
      Access: ILMemberAccess;
      Data:  byte[] option;
      LiteralValue: ILFieldInit option;  
      /// The explicit offset in bytes when explicit layout is used.
      Offset:  int32 option; 
      IsSpecialName: bool;
      Marshal: ILNativeType option; 
      NotSerialized: bool;
      IsLiteral: bool ;
      IsInitOnly: bool;
      CustomAttrs: ILAttributes; }

/// Tables of fields.  Logically equivalent to a list of fields but
/// the table is kept in a form optimized for looking up fields by 
/// name.
[<NoEquality; NoComparison; Sealed>]
type ILFieldDefs =
    member AsList : ILFieldDef list
    member LookupByName : string -> ILFieldDef list

/// Event definitions.
[<NoComparison; NoEquality>]
type ILEventDef =
    { Type: ILType option; 
      Name: string;
      IsRTSpecialName: bool;
      IsSpecialName: bool;
      AddMethod: ILMethodRef; 
      RemoveMethod: ILMethodRef;
      FireMethod: ILMethodRef option;
      OtherMethods: ILMethodRef list;
      CustomAttrs: ILAttributes; }

/// Table of those events in a type definition.
[<NoEquality; NoComparison; Sealed>]
type ILEventDefs =
    member AsList : ILEventDef list
    member LookupByName : string -> ILEventDef list

/// Property definitions.
[<NoComparison; NoEquality>]
type ILPropertyDef =
    { Name: string;
      IsRTSpecialName: bool;
      IsSpecialName: bool;
      SetMethod: ILMethodRef option;
      GetMethod: ILMethodRef option;
      CallingConv: ILThisConvention;
      Type: ILType;          
      Init: ILFieldInit option;
      Args: ILTypes;
      CustomAttrs: ILAttributes; }

/// Table of those properties in a type definition.
[<NoEquality; NoComparison>]
[<Sealed>]
type ILPropertyDefs =
    member AsList : ILPropertyDef list
    member LookupByName : string -> ILPropertyDef list

/// Method Impls
///
/// If there is an entry (pms --&gt; ms) in this table, then method [ms] 
/// is used to implement method [pms] for the purposes of this class 
/// and its subclasses. 
type ILMethodImplDef =
    { Overrides: ILOverridesSpec;
      OverrideBy: ILMethodSpec }

[<NoEquality; NoComparison; Sealed>]
type ILMethodImplDefs =
    member AsList : ILMethodImplDef list

/// Type Layout information.
[<RequireQualifiedAccess>]
type ILTypeDefLayout =
    | Auto
    | Sequential of ILTypeDefLayoutInfo
    | Explicit of ILTypeDefLayoutInfo 

and ILTypeDefLayoutInfo =
    { Size: int32 option;
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

//-------------------------------------------------------------------
// A note for the nit-picky.... In theory, the "kind" of a type 
// definition can only be  partially determined prior to binding.  
// For example, you cannot really, absolutely tell if a type is 
// really, absolutely a value type until you bind the 
// super class and test it for type equality against System.ValueType.  
// However, this is unbearably annoying, as it means you 
// have to load "primary runtime assembly (System.Runtime or mscorlib)" and perform bind operations 
// in order to be able to determine some quite simple 
// things.  So we approximate by simply looking at the name
// of the superclass when loading.
// ------------------------------------------------------------------ 

[<RequireQualifiedAccess>]
type ILTypeDefKind =
    | Class
    | ValueType
    | Interface
    | Enum 
    | Delegate 

/// Tables of named type definitions.  The types and table may contain on-demand
/// (lazy) computations, e.g. the actual reading of some aspects
/// of a type definition may be delayed if the reader being used supports
/// this.
///
/// This is an abstract type equivalent to "ILTypeDef list".
[<NoEquality; NoComparison>]
[<Sealed>]
type ILTypeDefs =
    interface IEnumerable<ILTypeDef>
    member AsArray : ILTypeDef[]
    member AsList : ILTypeDef list

    /// Get some information about the type defs, but do not force the read of the type defs themselves.
    member AsArrayOfLazyTypeDefs : (string list * string * ILAttributes * Lazy<ILTypeDef>) array

    /// Calls to <c>FindByName</c> will result in any laziness in the overall 
    /// set of ILTypeDefs being read in in addition 
    /// to the details for the type found, but the remaining individual 
    /// type definitions will not be read. 
    member FindByName : string -> ILTypeDef

/// Type Definitions 
///
/// As for methods there are several important constraints not encoded 
/// in the type definition below, for example that the super class of
/// an interface type is always None, or that enumerations always
/// have a very specific form.
and [<NoComparison; NoEquality>]
    ILTypeDef =  
    { tdKind: ILTypeDefKind;
      Name: string;  
      GenericParams: ILGenericParameterDefs;  
      Access: ILTypeDefAccess;  
      IsAbstract: bool;
      IsSealed: bool; 
      IsSerializable: bool; 
      /// Class or interface generated for COM interop. 
      IsComInterop: bool; 
      Layout: ILTypeDefLayout;
      IsSpecialName: bool;
      Encoding: ILDefaultPInvokeEncoding;
      NestedTypes: ILTypeDefs;
      Implements: ILTypes;  
      Extends: ILType option; 
      Methods: ILMethodDefs;
      SecurityDecls: ILPermissions;
      /// Some classes are marked "HasSecurity" even if there are no permissions attached, 
      /// e.g. if they use SuppressUnmanagedCodeSecurityAttribute 
      HasSecurity: bool; 
      Fields: ILFieldDefs;
      MethodImpls: ILMethodImplDefs;
      InitSemantics: ILTypeInit;
      Events: ILEventDefs;
      Properties: ILPropertyDefs;
      CustomAttrs: ILAttributes; }
    member IsClass: bool;
    member IsInterface: bool;
    member IsEnum: bool;
    member IsDelegate: bool;
    member IsStructOrEnum : bool

[<NoEquality; NoComparison>]
[<Sealed>]
type ILNestedExportedTypes =
    member AsList : ILNestedExportedType  list

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
    { Name: string;
      Access: ILMemberAccess;
      Nested: ILNestedExportedTypes;
      CustomAttrs: ILAttributes } 

/// these are only found in the ILExportedTypesAndForwarders table in the manifest 
[<NoComparison; NoEquality>]
type ILExportedTypeOrForwarder =
    { ScopeRef: ILScopeRef;
      /// [Namespace.]Name
      Name: string;
      IsForwarder: bool;
      Access: ILTypeDefAccess;
      Nested: ILNestedExportedTypes;
      CustomAttrs: ILAttributes } 

[<NoEquality; NoComparison>]
[<Sealed>]
type ILExportedTypesAndForwarders =
    member AsList : ILExportedTypeOrForwarder  list

[<RequireQualifiedAccess>]
type ILResourceAccess = 
    | Public 
    | Private 

[<RequireQualifiedAccess>]
type ILResourceLocation = 
    | Local of (unit -> byte[])  (* resources may be re-read each time this function is called *)
    | File of ILModuleRef * int32
    | Assembly of ILAssemblyRef

/// "Manifest ILResources" are chunks of resource data, being one of:
///   - the data section of the current module (byte[] of resource given directly).
///   - in an external file in this assembly (offset given in the ILResourceLocation field). 
///   - as a resources in another assembly of the same name.  
type ILResource =
    { Name: string;
      Location: ILResourceLocation;
      Access: ILResourceAccess;
      CustomAttrs: ILAttributes }

/// Table of resources in a module.
[<NoEquality; NoComparison>]
[<Sealed>]
type ILResources =
    member AsList : ILResource  list


[<RequireQualifiedAccess>]
type ILAssemblyLongevity =
    | Unspecified
    | Library
    | PlatformAppDomain
    | PlatformProcess
    | PlatformSystem

/// The main module of an assembly is a module plus some manifest information.
type ILAssemblyManifest = 
    { Name: string;
      /// This is the ID of the algorithm used for the hashes of auxiliary 
      /// files in the assembly.   These hashes are stored in the 
      /// <c>ILModuleRef.Hash</c> fields of this assembly. These are not 
      /// cryptographic hashes: they are simple file hashes. The algorithm 
      /// is normally <c>0x00008004</c> indicating the SHA1 hash algorithm.  
      AuxModuleHashAlgorithm: int32; 
      SecurityDecls: ILPermissions;
      /// This is the public key used to sign this 
      /// assembly (the signature itself is stored elsewhere: see the 
      /// binary format, and may not have been written if delay signing 
      /// is used).  (member Name, member PublicKey) forms the full 
      /// public name of the assembly.  
      PublicKey: byte[] option;  
      Version: ILVersionInfo option;
      Locale: string option;
      CustomAttrs: ILAttributes;
      AssemblyLongevity: ILAssemblyLongevity; 
      DisableJitOptimizations: bool;
      JitTracking: bool;
      Retargetable: bool;
      /// Records the types impemented by this asssembly in auxiliary 
      /// modules. 
      ExportedTypes: ILExportedTypesAndForwarders;
      /// Records whether the entrypoint resides in another module. 
      EntrypointElsewhere: ILModuleRef option;
    } 
    
/// One module in the "current" assembly, either a main-module or
/// an auxiliary module.  The main module will have a manifest.
///
/// An assembly is built by joining together a "main" module plus 
/// several auxiliary modules. 
type ILModuleDef = 
    { Manifest: ILAssemblyManifest option;
      CustomAttrs: ILAttributes;
      Name: string;
      TypeDefs: ILTypeDefs;
      SubsystemVersion : int * int
      UseHighEntropyVA : bool
      SubSystemFlags: int32;
      IsDLL: bool;
      IsILOnly: bool;
      Platform: ILPlatform option;
      StackReserveSize: int32 option;
      Is32Bit: bool;
      Is32BitPreferred: bool;
      Is64Bit: bool;
      VirtualAlignment: int32;
      PhysicalAlignment: int32;
      ImageBase: int32;
      MetadataVersion: string;
      Resources: ILResources; 
      /// e.g. win86 resources, as the exact contents of a .res or .obj file. 
      NativeResources: Lazy<byte[]> list;  }
    member ManifestOfAssembly: ILAssemblyManifest 
    member HasManifest : bool

/// Find the method definition corresponding to the given property or 
/// event operation. These are always in the same class as the property 
/// or event. This is useful especially if your code is not using the Ilbind 
/// API to bind references. 
val resolveILMethodRef: ILTypeDef -> ILMethodRef -> ILMethodDef
val resolveILMethodRefWithRescope: (ILType -> ILType) -> ILTypeDef -> ILMethodRef -> ILMethodDef

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

/// Represents the capabilities of target framework profile.
/// Different profiles may omit some types or contain them in different assemblies.
type IPrimaryAssemblyTraits = 
    
    abstract TypedReferenceTypeScopeRef : ILScopeRef option
    abstract RuntimeArgumentHandleTypeScopeRef : ILScopeRef option
    abstract SerializationInfoTypeScopeRef : ILScopeRef option
    abstract SecurityPermissionAttributeTypeScopeRef : ILScopeRef option    
    abstract IDispatchConstantAttributeScopeRef : ILScopeRef option
    abstract IUnknownConstantAttributeScopeRef : ILScopeRef option
    abstract ArgIteratorTypeScopeRef : ILScopeRef option
    abstract MarshalByRefObjectScopeRef : ILScopeRef option
    abstract ThreadStaticAttributeScopeRef : ILScopeRef option
    abstract SpecialNameAttributeScopeRef : ILScopeRef option
    abstract ContextStaticAttributeScopeRef : ILScopeRef option
    abstract NonSerializedAttributeScopeRef : ILScopeRef option

    abstract SystemRuntimeInteropServicesScopeRef   : Lazy<ILScopeRef option>
    abstract SystemLinqExpressionsScopeRef          : Lazy<ILScopeRef>
    abstract SystemCollectionsScopeRef              : Lazy<ILScopeRef>
    abstract SystemReflectionScopeRef               : Lazy<ILScopeRef>
    abstract SystemDiagnosticsDebugScopeRef         : Lazy<ILScopeRef>
    abstract ScopeRef : ILScopeRef

// ====================================================================
// PART 2
// 
// Making metadata.  Where no explicit constructor
// is given, you should create the concrete datatype directly, 
// e.g. by filling in all appropriate record fields.
// ==================================================================== *)

/// A table of common references to items in primary assebly (System.Runtime or mscorlib).
/// If a particular version of System.Runtime.dll has been loaded then you should 
/// reference items from it via an ILGlobals for that specific version built using mkILGlobals. 
[<NoEquality; NoComparison>]
type ILGlobals = 
    { 
      traits : IPrimaryAssemblyTraits
      primaryAssemblyName: string
      noDebugData: bool
      tref_Object: ILTypeRef
      tspec_Object: ILTypeSpec
      typ_Object: ILType
      tref_String: ILTypeRef
      typ_String: ILType
      typ_StringBuilder: ILType
      typ_AsyncCallback: ILType
      typ_IAsyncResult: ILType
      typ_IComparable: ILType
      tref_Type: ILTypeRef
      typ_Type: ILType
      typ_Missing: Lazy<ILType>
      typ_Activator: ILType
      typ_Delegate: ILType
      typ_ValueType: ILType
      typ_Enum: ILType
      tspec_TypedReference: ILTypeSpec option
      typ_TypedReference: ILType option
      typ_MulticastDelegate: ILType
      typ_Array: ILType
      tspec_Int64: ILTypeSpec
      tspec_UInt64: ILTypeSpec
      tspec_Int32: ILTypeSpec
      tspec_UInt32: ILTypeSpec
      tspec_Int16: ILTypeSpec
      tspec_UInt16: ILTypeSpec
      tspec_SByte: ILTypeSpec
      tspec_Byte: ILTypeSpec
      tspec_Single: ILTypeSpec
      tspec_Double: ILTypeSpec
      tspec_IntPtr: ILTypeSpec
      tspec_UIntPtr: ILTypeSpec
      tspec_Char: ILTypeSpec
      tspec_Bool: ILTypeSpec
      typ_int8: ILType
      typ_int16: ILType
      typ_int32: ILType
      typ_int64: ILType
      typ_uint8: ILType
      typ_uint16: ILType
      typ_uint32: ILType
      typ_uint64: ILType
      typ_float32: ILType
      typ_float64: ILType
      typ_bool: ILType
      typ_char: ILType
      typ_IntPtr: ILType
      typ_UIntPtr: ILType
      typ_RuntimeArgumentHandle: ILType option
      typ_RuntimeTypeHandle: ILType
      typ_RuntimeMethodHandle: ILType
      typ_RuntimeFieldHandle: ILType
      typ_Byte: ILType
      typ_Int16: ILType
      typ_Int32: ILType
      typ_Int64: ILType
      typ_SByte: ILType
      typ_UInt16: ILType
      typ_UInt32: ILType
      typ_UInt64: ILType
      typ_Single: ILType
      typ_Double: ILType
      typ_Bool: ILType
      typ_Char: ILType
      typ_SerializationInfo: ILType option
      typ_StreamingContext: ILType
      tref_SecurityPermissionAttribute : ILTypeRef option
      tspec_Exception: ILTypeSpec
      typ_Exception: ILType 
      mutable generatedAttribsCache: ILAttribute list 
      mutable debuggerBrowsableNeverAttributeCache : ILAttribute option 
      mutable debuggerTypeProxyAttributeCache : ILAttribute option }

      with
      member mkDebuggableAttribute: bool (* disable JIT optimizations *) -> ILAttribute
      /// Some commonly used custom attibutes
      member mkDebuggableAttributeV2               : bool (* ignoreSymbolStoreSequencePoints *) * bool (* disable JIT optimizations *) * bool (* enable EnC *) -> ILAttribute
      member mkCompilerGeneratedAttribute          : unit -> ILAttribute
      member mkDebuggerNonUserCodeAttribute        : unit -> ILAttribute
      member mkDebuggerStepThroughAttribute        : unit -> ILAttribute
      member mkDebuggerHiddenAttribute             : unit -> ILAttribute
      member mkDebuggerDisplayAttribute            : string -> ILAttribute
      member mkDebuggerTypeProxyAttribute          : ILType -> ILAttribute
      member mkDebuggerBrowsableNeverAttribute     : unit -> ILAttribute

/// Build the table of commonly used references given an <c>ILScopeRef</c> for system runtime assembly. 
val mkILGlobals : IPrimaryAssemblyTraits -> string option -> bool -> ILGlobals

val mkMscorlibBasedTraits : ILScopeRef -> IPrimaryAssemblyTraits

val EcmaILGlobals : ILGlobals

/// When writing a binary the fake "toplevel" type definition (called <Module>)
/// must come first. This function puts it first, and creates it in the returned 
/// list as an empty typedef if it doesn't already exist.
val destTypeDefsWithGlobalFunctionsFirst: ILGlobals -> ILTypeDefs -> ILTypeDef list

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

val emptyILGenericArgs: ILGenericArgs
val mkILTyvarTy: uint16 -> ILType

/// Make type refs.
val mkILNestedTyRef: ILScopeRef * string list * string -> ILTypeRef
val mkILTyRef: ILScopeRef * string -> ILTypeRef
val mkILTyRefInTyRef: ILTypeRef * string -> ILTypeRef

type ILGenericArgsList = ILType list
val mkILGenericArgs : ILGenericArgsList -> ILGenericArgs
/// Make type specs.
val mkILNonGenericTySpec: ILTypeRef -> ILTypeSpec
val mkILTySpec: ILTypeRef * ILGenericArgsList -> ILTypeSpec
val mkILTySpecRaw: ILTypeRef * ILGenericArgs -> ILTypeSpec

/// Make types.
val mkILTy: ILBoxity -> ILTypeSpec -> ILType
val mkILNamedTy: ILBoxity -> ILTypeRef -> ILGenericArgsList -> ILType
val mkILNamedTyRaw: ILBoxity -> ILTypeRef -> ILGenericArgs -> ILType
val mkILBoxedTy: ILTypeRef -> ILGenericArgsList -> ILType
val mkILBoxedTyRaw: ILTypeRef -> ILGenericArgs -> ILType
val mkILValueTy: ILTypeRef -> ILGenericArgsList -> ILType
val mkILNonGenericBoxedTy: ILTypeRef -> ILType
val mkILNonGenericValueTy: ILTypeRef -> ILType
val mkILArrTy: ILType * ILArrayShape -> ILType
val mkILArr1DTy: ILType -> ILType
val isILArrTy: ILType -> bool
val destILArrTy: ILType -> ILArrayShape * ILType 
val mkILBoxedType : ILTypeSpec -> ILType

val mkILTypes : ILType list -> ILTypes

/// Make method references and specs.
val mkILMethRefRaw: ILTypeRef * ILCallingConv * string * int * ILTypes * ILType -> ILMethodRef
val mkILMethRef: ILTypeRef * ILCallingConv * string * int * ILType list * ILType -> ILMethodRef
val mkILMethSpec: ILMethodRef * ILBoxity * ILGenericArgsList * ILGenericArgsList -> ILMethodSpec
val mkILMethSpecForMethRefInTyRaw: ILMethodRef * ILType * ILGenericArgs -> ILMethodSpec
val mkILMethSpecForMethRefInTy: ILMethodRef * ILType * ILGenericArgsList -> ILMethodSpec
val mkILMethSpecInTy: ILType * ILCallingConv * string * ILType list * ILType * ILGenericArgsList -> ILMethodSpec
val mkILMethSpecInTyRaw: ILType * ILCallingConv * string * ILTypes * ILType * ILGenericArgs -> ILMethodSpec

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

val mkILCallSigRaw: ILCallingConv * ILTypes * ILType -> ILCallingSignature
val mkILCallSig: ILCallingConv * ILType list * ILType -> ILCallingSignature

/// Make generalized verions of possibly-generic types,
/// e.g. Given the ILTypeDef for List, return the type "List<T>".
val mkILFormalBoxedTy: ILTypeRef -> ILGenericParameterDef list -> ILType
val mkILFormalNamedTy: ILBoxity -> ILTypeRef -> ILGenericParameterDef list -> ILType

val mkILFormalTyparsRaw: ILTypes -> ILGenericParameterDefs
val mkILFormalTypars: ILType list -> ILGenericParameterDefs
val mkILFormalGenericArgsRaw: ILGenericParameterDefs -> ILGenericArgs
val mkILFormalGenericArgs: ILGenericParameterDefs -> ILGenericArgsList
val mkILSimpleTypar : string -> ILGenericParameterDef
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

val mkPermissionSet : ILGlobals -> ILSecurityAction * (ILTypeRef * (string * ILType * ILAttribElem) list) list -> ILPermission

/// Making code.
val generateCodeLabel: unit -> ILCodeLabel
val formatCodeLabel : ILCodeLabel -> string

/// Make some code that is a straight line sequence of instructions. 
/// The function will add a "return" if the last instruction is not an exiting instruction.
val nonBranchingInstrsToCode: ILInstr list -> ILCode 

/// Helpers for codegen: scopes for allocating new temporary variables.
type ILLocalsAllocator =
    new : preAlloc: int -> ILLocalsAllocator
    member AllocLocal : ILLocal -> uint16
    member Close : unit -> ILLocal list

/// Derived functions for making some common patterns of instructions.
val mkNormalCall: ILMethodSpec -> ILInstr
val mkNormalCallvirt: ILMethodSpec -> ILInstr
val mkNormalCallconstraint: ILType * ILMethodSpec -> ILInstr
val mkNormalNewobj: ILMethodSpec -> ILInstr
val mkCallBaseConstructor : ILType * ILType list -> ILInstr list
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
val mkILParam: string option * ILType -> ILParameter
val mkILParamAnon: ILType -> ILParameter
val mkILParamNamed: string * ILType -> ILParameter
val mkILReturn: ILType -> ILReturn
val mkILLocal: ILType -> (string * int * int) option -> ILLocal
val mkILLocals : ILLocal list -> ILLocals
val emptyILLocals : ILLocals

/// Make a formal generic parameters.
val mkILEmptyGenericParams: ILGenericParameterDefs

/// Make method definitions.
val mkILMethodBody: initlocals:bool * ILLocals * int * ILCode * ILSourceMarker option -> ILMethodBody
val mkMethodBody: bool * ILLocals * int * ILCode * ILSourceMarker option -> MethodBody

val mkILCtor: ILMemberAccess * ILParameter list * MethodBody -> ILMethodDef
val mkILClassCtor: MethodBody -> ILMethodDef
val mkILNonGenericEmptyCtor: ILSourceMarker option -> ILType -> ILMethodDef
val mkILStaticMethod: ILGenericParameterDefs * string * ILMemberAccess * ILParameter list * ILReturn * MethodBody -> ILMethodDef
val mkILNonGenericStaticMethod: string * ILMemberAccess * ILParameter list * ILReturn * MethodBody -> ILMethodDef
val mkILGenericVirtualMethod: string * ILMemberAccess * ILGenericParameterDefs * ILParameter list * ILReturn * MethodBody -> ILMethodDef
val mkILGenericNonVirtualMethod: string * ILMemberAccess * ILGenericParameterDefs * ILParameter list * ILReturn * MethodBody -> ILMethodDef
val mkILNonGenericVirtualMethod: string * ILMemberAccess * ILParameter list * ILReturn * MethodBody -> ILMethodDef
val mkILNonGenericInstanceMethod: string * ILMemberAccess * ILParameter list * ILReturn * MethodBody -> ILMethodDef


/// Make field definitions.
val mkILInstanceField: string * ILType * ILFieldInit option * ILMemberAccess -> ILFieldDef
val mkILStaticField: string * ILType * ILFieldInit option * byte[] option * ILMemberAccess -> ILFieldDef
val mkILLiteralField: string * ILType * ILFieldInit * byte[] option * ILMemberAccess -> ILFieldDef

/// Make a type definition.
val mkILGenericClass: string * ILTypeDefAccess * ILGenericParameterDefs * ILType * ILType list * ILMethodDefs * ILFieldDefs * ILTypeDefs * ILPropertyDefs * ILEventDefs * ILAttributes * ILTypeInit -> ILTypeDef
val mkILSimpleClass: ILGlobals -> string * ILTypeDefAccess * ILMethodDefs * ILFieldDefs * ILTypeDefs * ILPropertyDefs * ILEventDefs * ILAttributes * ILTypeInit  -> ILTypeDef
val mkILTypeDefForGlobalFunctions: ILGlobals -> ILMethodDefs * ILFieldDefs -> ILTypeDef

/// Make a type definition for a value type used to point to raw data.
/// These are useful when generating array initialization code 
/// according to the 
///   ldtoken    field valuetype '<PrivateImplementationDetails>'/'$$struct0x6000127-1' '<PrivateImplementationDetails>'::'$$method0x6000127-1'
///   call       void System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class System.Array,valuetype System.RuntimeFieldHandle)
/// idiom.
val mkRawDataValueTypeDef:  ILGlobals -> string * size:int32 * pack:uint16 -> ILTypeDef

/// Injecting code into existing code blocks.  A branch will
/// be added from the given instructions to the (unique) entry of
/// the code, and the first instruction will be the new entry
/// of the method.  The instructions should be non-branching.

val prependInstrsToCode: ILInstr list -> ILCode -> ILCode
val prependInstrsToMethod: ILInstr list -> ILMethodDef -> ILMethodDef

/// Injecting initialization code into a class.
/// Add some code to the end of the .cctor for a type.  Create a .cctor
/// if one doesn't exist already.
val prependInstrsToClassCtor: ILInstr list -> ILSourceMarker option -> ILTypeDef -> ILTypeDef

/// Derived functions for making some simple constructors
val mkILStorageCtor: ILSourceMarker option * ILInstr list * ILType * (string * ILType) list * ILMemberAccess -> ILMethodDef
val mkILSimpleStorageCtor: ILSourceMarker option * ILTypeSpec option * ILType * (string * ILType) list * ILMemberAccess -> ILMethodDef
val mkILSimpleStorageCtorWithParamNames: ILSourceMarker option * ILTypeSpec option * ILType * (string * string * ILType) list * ILMemberAccess -> ILMethodDef

val mkILDelegateMethods: ILGlobals -> ILParameter list * ILReturn -> ILMethodDef list

/// Given a delegate type definition which lies in a particular scope, 
/// make a reference to its constructor.
val mkCtorMethSpecForDelegate: ILGlobals -> ILType * bool -> ILMethodSpec 

/// The toplevel "class" for a module or assembly.
val mkILTypeForGlobalFunctions: ILScopeRef -> ILType

/// Making tables of custom attributes, etc.
val mkILCustomAttrs: ILAttribute list -> ILAttributes
val mkILCustomAttrsFromArray: ILAttribute[] -> ILAttributes
val mkILComputedCustomAttrs: (unit -> ILAttribute[]) -> ILAttributes
val emptyILCustomAttrs: ILAttributes

val mkILSecurityDecls: ILPermission list -> ILPermissions
val mkILLazySecurityDecls: Lazy<ILPermission list> -> ILPermissions
val emptyILSecurityDecls: ILPermissions

val mkMethBodyAux : MethodBody -> ILLazyMethodBody
val mkMethBodyLazyAux : Lazy<MethodBody> -> ILLazyMethodBody

val mkILEvents: ILEventDef list -> ILEventDefs
val mkILEventsLazy: Lazy<ILEventDef list> -> ILEventDefs
val emptyILEvents: ILEventDefs

val mkILProperties: ILPropertyDef list -> ILPropertyDefs
val mkILPropertiesLazy: Lazy<ILPropertyDef list> -> ILPropertyDefs
val emptyILProperties: ILPropertyDefs

val mkILMethods: ILMethodDef list -> ILMethodDefs
val mkILMethodsFromArray: ILMethodDef[] -> ILMethodDefs
val mkILMethodsComputed: (unit -> ILMethodDef[]) -> ILMethodDefs
val emptyILMethods: ILMethodDefs

val mkILFields: ILFieldDef list -> ILFieldDefs
val mkILFieldsLazy: Lazy<ILFieldDef list> -> ILFieldDefs
val emptyILFields: ILFieldDefs

val mkILMethodImpls: ILMethodImplDef list -> ILMethodImplDefs
val mkILMethodImplsLazy: Lazy<ILMethodImplDef list> -> ILMethodImplDefs
val emptyILMethodImpls: ILMethodImplDefs

val mkILTypeDefs: ILTypeDef list -> ILTypeDefs
val mkILTypeDefsFromArray: ILTypeDef[] -> ILTypeDefs
val emptyILTypeDefs: ILTypeDefs

/// Create table of types which is loaded/computed on-demand, and whose individual 
/// elements are also loaded/computed on-demand. Any call to tdefs.AsList will 
/// result in the laziness being forced.  Operations can examine the
/// custom attributes and name of each type in order to decide whether
/// to proceed with examining the other details of the type.
/// 
/// Note that individual type definitions may contain further delays 
/// in their method, field and other tables. 
val mkILTypeDefsComputed: (unit -> (string list * string * ILAttributes * Lazy<ILTypeDef>) array) -> ILTypeDefs
val addILTypeDef: ILTypeDef -> ILTypeDefs -> ILTypeDefs

val mkILNestedExportedTypes: ILNestedExportedType list -> ILNestedExportedTypes
val mkILNestedExportedTypesLazy: Lazy<ILNestedExportedType list> -> ILNestedExportedTypes

val mkILExportedTypes: ILExportedTypeOrForwarder list -> ILExportedTypesAndForwarders
val mkILExportedTypesLazy: Lazy<ILExportedTypeOrForwarder list> ->   ILExportedTypesAndForwarders

val mkILResources: ILResource list -> ILResources
val mkILResourcesLazy: Lazy<ILResource list> -> ILResources

/// Making modules.
val mkILSimpleModule: assemblyName:string -> moduleName:string -> dll:bool -> subsystemVersion : (int * int) -> useHighEntropyVA : bool -> ILTypeDefs -> int32 option -> string option -> int -> ILExportedTypesAndForwarders -> string -> ILModuleDef

/// Generate references to existing type definitions, method definitions
/// etc.  Useful for generating references, e.g. to a  class we're processing
/// Also used to reference type definitions that we've generated.  [ILScopeRef] 
/// is normally ILScopeRef.Local, unless we've generated the ILTypeDef in
/// an auxiliary module or are generating multiple assemblies at 
/// once.

val mkRefForNestedILTypeDef : ILScopeRef -> ILTypeDef list * ILTypeDef -> ILTypeRef
val mkRefForILMethod        : ILScopeRef -> ILTypeDef list * ILTypeDef -> ILMethodDef -> ILMethodRef
val mkRefForILField        : ILScopeRef -> ILTypeDef list * ILTypeDef -> ILFieldDef  -> ILFieldRef

val mkRefToILMethod: ILTypeRef * ILMethodDef -> ILMethodRef
val mkRefToILField: ILTypeRef * ILFieldDef -> ILFieldRef

val mkRefToILAssembly: ILAssemblyManifest -> ILAssemblyRef
val mkRefToILModule: ILModuleDef -> ILModuleRef


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


//-----------------------------------------------------------------------
// The ILCode Builder utility.
//----------------------------------------------------------------------


/// buildILCode: Build code from a sequence of instructions.
/// 
/// e.g. "buildILCode meth resolver instrs exns locals"
/// 
/// This makes the basic block structure of code from more primitive
/// information, i.e. an array of instructions.
///   [meth]: for debugging and should give the name of the method.
///   [resolver]: should return the instruction indexes referred to 
///               by code-label strings in the instruction stream.
///   [instrs]: the instructions themselves, perhaps with attributes giving 
///             debugging information
///   [exns]: the table of exception-handling specifications
///           for the method.  These are again given with respect to labels which will
///           be mapped to pc's by [resolver].  
///   [locals]: the table of specifications of when local variables are live and
///           should appear in the debug info.
/// 
/// If the input code is well-formed, the function will returns the 
/// chop up the instruction sequence into basic blocks as required for
/// the exception handlers and then return the tree-structured code
/// corresponding to the instruction stream.
/// A new set of code labels will be used throughout the resulting code.
/// 
/// The input can be badly formed in many ways: exception handlers might
/// overlap, or scopes of local variables may overlap badly with 
/// exception handlers.
val buildILCode: string -> lab2pc: Dictionary<ILCodeLabel,int> -> instrs:ILInstr[] -> ILExceptionSpec list -> ILLocalDebugInfo list -> ILCode

// -------------------------------------------------------------------- 
// The instantiation utilities.
// -------------------------------------------------------------------- 

/// Instantiate type variables that occur within types and other items. 
val instILTypeAux: int -> ILGenericArgs -> ILType -> ILType

/// Instantiate type variables that occur within types and other items. 
val instILType: ILGenericArgs -> ILType -> ILType

// -------------------------------------------------------------------- 
// ECMA globals
// -------------------------------------------------------------------- 

/// This is a 'vendor neutral' way of referencing mscorlib. 
val ecmaPublicKey: PublicKey

/// Some commonly used methods. 
val mkInitializeArrayMethSpec: ILGlobals -> ILMethodSpec 

val mkPrimaryAssemblyExnNewobj: ILGlobals -> string -> ILInstr

val addMethodGeneratedAttrs : ILGlobals -> ILMethodDef -> ILMethodDef
val addPropertyGeneratedAttrs : ILGlobals -> ILPropertyDef -> ILPropertyDef
val addFieldGeneratedAttrs : ILGlobals -> ILFieldDef -> ILFieldDef

val addPropertyNeverAttrs : ILGlobals -> ILPropertyDef -> ILPropertyDef
val addFieldNeverAttrs : ILGlobals -> ILFieldDef -> ILFieldDef

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
val sha1HashBytes : byte[] -> byte[] (* SHA1 hash *)

/// Get a version number from a CLR version string, e.g. 1.0.3705.0
val parseILVersion: string -> ILVersionInfo
val formatILVersion: ILVersionInfo -> string
val compareILVersions: ILVersionInfo -> ILVersionInfo -> int

/// Decompose a type definition according to its kind.
type ILEnumInfo =
    { enumValues: (string * ILFieldInit) list;  
      enumType: ILType }

val getTyOfILEnumInfo: ILEnumInfo -> ILType

val computeILEnumInfo: string * ILFieldDefs -> ILEnumInfo


// --------------------------------------------------------------------
// For completeness.  These do not occur in metadata but tools that
// care about the existence of properties and events in the metadata
// can benefit from them.
// --------------------------------------------------------------------

[<Sealed>]
type ILEventRef =
    static member Create : ILTypeRef * string -> ILEventRef
    member EnclosingTypeRef: ILTypeRef
    member Name: string

[<Sealed>]
type ILPropertyRef =
     static member Create : ILTypeRef * string -> ILPropertyRef
     member EnclosingTypeRef: ILTypeRef
     member Name: string
     interface System.IComparable

val runningOnMono: bool

type ILReferences = 
    { AssemblyReferences: ILAssemblyRef list; 
      ModuleReferences: ILModuleRef list; }

/// Find the full set of assemblies referenced by a module.
val computeILRefs: ILModuleDef -> ILReferences
val emptyILRefs: ILReferences

