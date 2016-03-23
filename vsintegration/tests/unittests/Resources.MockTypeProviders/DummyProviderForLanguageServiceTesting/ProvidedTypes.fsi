// Copyright (c) Microsoft Corporation 2005-2014 and other contributors.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 
//
// This file contains a set of helper types and methods for providing types in an implementation 
// of ITypeProvider.
//
// This code has been modified and is appropriate for use in conjunction with the F# 3.0-4.0 releases


namespace ProviderImplementation.ProvidedTypes

open System
open System.Reflection
open System.Linq.Expressions
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Core.CompilerServices

/// Represents an erased provided parameter
type ProvidedParameter =
    inherit ParameterInfo
    // [<CompilerMessage("Please create a ProvidedTypesContext and use ctxt.ProvidedParameter to create a provided parameter. This will help allow your type provider to target portable profiles where that makes sense. Some argument names and values may need adjusting.", 8796)>]
    new : parameterName: string * parameterType: Type * ?isOut:bool * ?optionalValue:obj -> ProvidedParameter
    member IsParamArray : bool with get,set

/// Represents a provided static parameter.
type ProvidedStaticParameter =
    inherit ParameterInfo
    // [<CompilerMessage("Please create a ProvidedTypesContext and use ctxt.ProvidedStaticParameter to create a provided static parameter. Some argument names and values may need adjusting.", 8796)>]
    new : parameterName: string * parameterType:Type * ?parameterDefaultValue:obj -> ProvidedStaticParameter

    /// Add XML documentation information to this provided constructor
    member AddXmlDoc            : xmlDoc: string -> unit    

    /// Add XML documentation information to this provided constructor, where the computation of the documentation is delayed until necessary
    member AddXmlDocDelayed   : xmlDocFunction: (unit -> string) -> unit   

/// Represents an erased provided constructor.
type ProvidedConstructor =    
    inherit ConstructorInfo

    /// Create a new provided constructor. It is not initially associated with any specific provided type definition.
    // [<CompilerMessage("Please create a ProvidedTypesContext and use ctxt.ProvidedConstructor. Some argument names and values may need adjusting.", 8796)>]
    new : parameters: ProvidedParameter list -> ProvidedConstructor

    /// Add a 'Obsolete' attribute to this provided constructor
    member AddObsoleteAttribute : message: string * ?isError: bool -> unit    
    
    /// Add XML documentation information to this provided constructor
    member AddXmlDoc          : xmlDoc: string -> unit   
    
    /// Add XML documentation information to this provided constructor, where the computation of the documentation is delayed until necessary
    member AddXmlDocDelayed   : xmlDocFunction: (unit -> string) -> unit   
    
    /// Add XML documentation information to this provided constructor, where the documentation is re-computed  every time it is required.
    member AddXmlDocComputed   : xmlDocFunction: (unit -> string) -> unit   
    
    /// Set the quotation used to compute the implementation of invocations of this constructor.
    member InvokeCode         : (Expr list -> Expr) with set

    /// FSharp.Data addition: this method is used by Debug.fs
    member internal GetInvokeCodeInternal : bool -> (Expr [] -> Expr)

    /// Set the target and arguments of the base constructor call. Only used for generated types.
    member BaseConstructorCall : (Expr list -> ConstructorInfo * Expr list) with set

    /// Set a flag indicating that the constructor acts like an F# implicit constructor, so the
    /// parameters of the constructor become fields and can be accessed using Expr.GlobalVar with the
    /// same name.
    member IsImplicitCtor : bool with get,set

    /// Add definition location information to the provided constructor.
    member AddDefinitionLocation : line:int * column:int * filePath:string -> unit
    
    member IsTypeInitializer : bool with get,set

type ProvidedMethod = 
    inherit MethodInfo

    /// Create a new provided method. It is not initially associated with any specific provided type definition.
    // [<CompilerMessage("Please create a ProvidedTypesContext and use ctxt.ProvidedMethod to create a provided method. Some argument names and values may need adjusting.", 8796)>]
    new : methodName:string * parameters: ProvidedParameter list * returnType: Type -> ProvidedMethod

    /// Add XML documentation information to this provided method
    member AddObsoleteAttribute : message: string * ?isError: bool -> unit    

    /// Add XML documentation information to this provided constructor
    member AddXmlDoc            : xmlDoc: string -> unit    

    /// Add XML documentation information to this provided constructor, where the computation of the documentation is delayed until necessary
    member AddXmlDocDelayed   : xmlDocFunction: (unit -> string) -> unit   
    
    /// Add XML documentation information to this provided constructor, where the computation of the documentation is delayed until necessary
    /// The documentation is re-computed  every time it is required.
    member AddXmlDocComputed   : xmlDocFunction: (unit -> string) -> unit   
    
    member AddMethodAttrs       : attributes:MethodAttributes -> unit

    /// Set the method attributes of the method. By default these are simple 'MethodAttributes.Public'
    member SetMethodAttrs       : attributes:MethodAttributes -> unit

    /// Get or set a flag indicating if the property is static.
    member IsStaticMethod       : bool with get, set

    /// Set the quotation used to compute the implementation of invocations of this method.
    member InvokeCode         : (Expr list -> Expr) with set

    /// FSharp.Data addition: this method is used by Debug.fs
    member internal GetInvokeCodeInternal : bool -> (Expr [] -> Expr)

    /// Add definition location information to the provided type definition.
    member AddDefinitionLocation : line:int * column:int * filePath:string -> unit

    /// Add a custom attribute to the provided method definition.
    member AddCustomAttribute : CustomAttributeData -> unit

    /// Define the static parameters available on a statically parameterized method
    member DefineStaticParameters     : parameters: ProvidedStaticParameter list * instantiationFunction: (string -> obj[] -> ProvidedMethod) -> unit

/// Represents an erased provided property.
type ProvidedProperty =
    inherit PropertyInfo

    /// Create a new provided property. It is not initially associated with any specific provided type definition.
    // [<CompilerMessage("Please create a ProvidedTypesContext and use ctxt.ProvidedProperty to create a provided property. This will help allow your type provider to target portable profiles where that makes sense. Some argument names and values may need adjusting.", 8796)>]
    new  : propertyName: string * propertyType: Type * ?parameters:ProvidedParameter list -> ProvidedProperty

    /// Add a 'Obsolete' attribute to this provided property
    member AddObsoleteAttribute : message: string * ?isError: bool -> unit    

    /// Add XML documentation information to this provided constructor
    member AddXmlDoc            : xmlDoc: string -> unit    

    /// Add XML documentation information to this provided constructor, where the computation of the documentation is delayed until necessary
    member AddXmlDocDelayed   : xmlDocFunction: (unit -> string) -> unit   
    
    /// Add XML documentation information to this provided constructor, where the computation of the documentation is delayed until necessary
    /// The documentation is re-computed  every time it is required.
    member AddXmlDocComputed   : xmlDocFunction: (unit -> string) -> unit   
    
    /// Get or set a flag indicating if the property is static.
    /// FSharp.Data addition: the getter is used by Debug.fs
    member IsStatic             : bool with get,set

    /// Set the quotation used to compute the implementation of gets of this property.
    member GetterCode           : (Expr list -> Expr) with set

    /// Set the function used to compute the implementation of sets of this property.
    member SetterCode           : (Expr list -> Expr) with set

    /// Add definition location information to the provided type definition.
    member AddDefinitionLocation : line:int * column:int * filePath:string -> unit

    /// Add a custom attribute to the provided property definition.
    member AddCustomAttribute : CustomAttributeData -> unit

/// Represents an erased provided property.
type ProvidedEvent =
    inherit EventInfo

    /// Create a new provided type. It is not initially associated with any specific provided type definition.
    new  : propertyName: string * eventHandlerType: Type -> ProvidedEvent

    /// Add XML documentation information to this provided constructor
    member AddXmlDoc            : xmlDoc: string -> unit    

    /// Add XML documentation information to this provided constructor, where the computation of the documentation is delayed until necessary
    member AddXmlDocDelayed   : xmlDocFunction: (unit -> string) -> unit   
    
    /// Add XML documentation information to this provided constructor, where the computation of the documentation is delayed until necessary
    /// The documentation is re-computed  every time it is required.
    member AddXmlDocComputed   : xmlDocFunction: (unit -> string) -> unit   
    
    /// Get or set a flag indicating if the property is static.
    member IsStatic             : bool with set

    /// Set the quotation used to compute the implementation of gets of this property.
    member AdderCode           : (Expr list -> Expr) with set

    /// Set the function used to compute the implementation of sets of this property.
    member RemoverCode           : (Expr list -> Expr) with set

    /// Add definition location information to the provided type definition.
    member AddDefinitionLocation : line:int * column:int * filePath:string -> unit

/// Represents an erased provided field.
type ProvidedLiteralField =
    inherit FieldInfo

    /// Create a new provided field. It is not initially associated with any specific provided type definition.
    // [<CompilerMessage("Please create a ProvidedTypesContext and use ctxt.ProvidedLiteralField. This will help allow your type provider to target portable profiles where that makes sense. Some argument names and values may need adjusting.", 8796)>]
    new : fieldName: string * fieldType: Type * literalValue: obj -> ProvidedLiteralField

    /// Add a 'Obsolete' attribute to this provided field
    member AddObsoleteAttribute : message: string * ?isError: bool -> unit    

    /// Add XML documentation information to this provided field
    member AddXmlDoc            : xmlDoc: string -> unit    

    /// Add XML documentation information to this provided field, where the computation of the documentation is delayed until necessary
    member AddXmlDocDelayed   : xmlDocFunction: (unit -> string) -> unit   
    
    /// Add XML documentation information to this provided field, where the computation of the documentation is delayed until necessary
    /// The documentation is re-computed  every time it is required.
    member AddXmlDocComputed   : xmlDocFunction: (unit -> string) -> unit   

    /// Add definition location information to the provided field.
    member AddDefinitionLocation : line:int * column:int * filePath:string -> unit

/// Represents an erased provided field.
type ProvidedField =
    inherit FieldInfo

    /// Create a new provided field. It is not initially associated with any specific provided type definition.
    // [<CompilerMessage("Please create a ProvidedTypesContext and use ctxt.ProvidedField. This will help allow your type provider to target portable profiles where that makes sense. Some argument names and values may need adjusting.", 8796)>]
    new  : fieldName: string * fieldType: Type -> ProvidedField

    /// Add a 'Obsolete' attribute to this provided field
    member AddObsoleteAttribute : message: string * ?isError: bool -> unit    

    /// Add XML documentation information to this provided field
    member AddXmlDoc            : xmlDoc: string -> unit    

    /// Add XML documentation information to this provided field, where the computation of the documentation is delayed until necessary
    member AddXmlDocDelayed   : xmlDocFunction: (unit -> string) -> unit   
    
    /// Add XML documentation information to this provided field, where the computation of the documentation is delayed until necessary
    /// The documentation is re-computed  every time it is required.
    member AddXmlDocComputed   : xmlDocFunction: (unit -> string) -> unit   

    /// Add definition location information to the provided field definition.
    member AddDefinitionLocation : line:int * column:int * filePath:string -> unit

    member SetFieldAttributes : attributes : FieldAttributes -> unit

/// Represents the type constructor in a provided symbol type.
[<NoComparison>]
type ProvidedSymbolKind = 
    /// Indicates that the type constructor is for a single-dimensional array
    | SDArray 
    /// Indicates that the type constructor is for a multi-dimensional array
    | Array of int 
    /// Indicates that the type constructor is for pointer types
    | Pointer 
    /// Indicates that the type constructor is for byref types
    | ByRef 
    /// Indicates that the type constructor is for named generic types
    | Generic of Type 
    /// Indicates that the type constructor is for abbreviated types
    | FSharpTypeAbbreviation of (Assembly * string * string[])

/// Represents an array or other symbolic type involving a provided type as the argument.
/// See the type provider spec for the methods that must be implemented.
/// Note that the type provider specification does not require us to implement pointer-equality for provided types.
[<Class>]
type ProvidedSymbolType =
    inherit Type

    /// Returns the kind of this symbolic type
    member Kind : ProvidedSymbolKind

    /// Return the provided types used as arguments of this symbolic type
    member Args : list<Type>

    /// For example, kg
    member IsFSharpTypeAbbreviation: bool

    /// For example, int<kg> or int<kilogram>
    member IsFSharpUnitAnnotated : bool

/// Helpers to build symbolic provided types
[<Class>]
type ProvidedTypeBuilder =

    /// Like typ.MakeGenericType, but will also work with unit-annotated types
    static member MakeGenericType: genericTypeDefinition: Type * genericArguments: Type list -> Type

    /// Like methodInfo.MakeGenericMethod, but will also work with unit-annotated types and provided types
    static member MakeGenericMethod: genericMethodDefinition: MethodInfo * genericArguments: Type list -> MethodInfo

/// Helps create erased provided unit-of-measure annotations.
[<Class>]
type ProvidedMeasureBuilder =
    
    /// The ProvidedMeasureBuilder for building measures.
    static member Default : ProvidedMeasureBuilder

    /// Gets the measure indicating the "1" unit of measure, that is the unitless measure. 
    member One : Type

    /// Returns the measure indicating the product of two units of measure, e.g. kg * m
    member Product : measure1: Type * measure1: Type  -> Type

    /// Returns the measure indicating the inverse of two units of measure, e.g. 1 / s
    member Inverse : denominator: Type -> Type

    /// Returns the measure indicating the ratio of two units of measure, e.g. kg / m
    member Ratio : numerator: Type * denominator: Type -> Type
    
    /// Returns the measure indicating the square of a unit of measure, e.g. m * m
    member Square : ``measure``: Type -> Type
    
    /// Returns the measure for an SI unit from the F# core library, where the string is in capitals and US spelling, e.g. Meter
    member SI : unitName:string -> Type
    
    /// Returns a type where the type has been annotated with the given types and/or units-of-measure.
    /// e.g. float<kg>, Vector<int, kg>
    member AnnotateType : basic: Type * argument: Type list -> Type


/// Represents a provided type definition.
type ProvidedTypeDefinition =
    inherit Type

    /// Create a new provided type definition in a namespace. 
    // [<CompilerMessage("Please create a ProvidedTypesContext and use ctxt.ProvidedTypeDefinition to create a provided type definition. This will help allow your type provider to target portable profiles where that makes sense. Some argument names and values may need adjusting.", 8796)>]
    new : assembly: Assembly * namespaceName: string * className: string * baseType: Type option -> ProvidedTypeDefinition

    /// Create a new provided type definition, to be located as a nested type in some type definition.
    // [<CompilerMessage("Please create a ProvidedTypesContext and use ctxt.ProvidedTypeDefinition to create a provided type definition. This will help allow your type provider to target portable profiles where that makes sense. Some argument names and values may need adjusting.", 8796)>]
    new : className : string * baseType: Type option -> ProvidedTypeDefinition

    /// Add the given type as an implemented interface.
    member AddInterfaceImplementation : interfaceType: Type -> unit    

    /// Add the given function as a set of on-demand computed interfaces.
    member AddInterfaceImplementationsDelayed : interfacesFunction:(unit -> Type list)-> unit    

    /// Specifies that the given method body implements the given method declaration.
    member DefineMethodOverride : methodInfoBody: ProvidedMethod * methodInfoDeclaration: MethodInfo -> unit

    /// Add a 'Obsolete' attribute to this provided type definition
    member AddObsoleteAttribute : message: string * ?isError: bool -> unit    

    /// Add XML documentation information to this provided constructor
    member AddXmlDoc             : xmlDoc: string -> unit    

    /// Set the base type
    member SetBaseType             : Type -> unit    

    /// Set the base type to a lazily evaluated value. Use this to delay realization of the base type as late as possible.
    member SetBaseTypeDelayed      : baseTypeFunction:(unit -> Type) -> unit    

    /// Set underlying type for generated enums
    member SetEnumUnderlyingType : Type -> unit

    /// Add XML documentation information to this provided constructor, where the computation of the documentation is delayed until necessary.
    /// The documentation is only computed once.
    member AddXmlDocDelayed   : xmlDocFunction: (unit -> string) -> unit   
    
    /// Add XML documentation information to this provided constructor, where the computation of the documentation is delayed until necessary
    /// The documentation is re-computed  every time it is required.
    member AddXmlDocComputed   : xmlDocFunction: (unit -> string) -> unit   
    
    /// Set the attributes on the provided type. This fully replaces the default TypeAttributes.
    member SetAttributes        : TypeAttributes -> unit
    
    /// Reset the enclosing type (for generated nested types)
    member ResetEnclosingType: enclosingType:Type -> unit
    
    /// Add a method, property, nested type or other member to a ProvidedTypeDefinition
    member AddMember         : memberInfo:MemberInfo      -> unit  

    /// Add a set of members to a ProvidedTypeDefinition
    member AddMembers        : memberInfos:list<#MemberInfo> -> unit

    /// Add a member to a ProvidedTypeDefinition, delaying computation of the members until required by the compilation context.
    member AddMemberDelayed  : memberFunction:(unit -> #MemberInfo)      -> unit

    /// Add a set of members to a ProvidedTypeDefinition, delaying computation of the members until required by the compilation context.
    member AddMembersDelayed : membersFunction:(unit -> list<#MemberInfo>) -> unit    
    
    /// Add the types of the generated assembly as generative types, where types in namespaces get hierarchically positioned as nested types.
    member AddAssemblyTypesAsNestedTypesDelayed : assemblyFunction:(unit -> Assembly) -> unit

    /// Define the static parameters available on a statically parameterized type
    member DefineStaticParameters     : parameters: ProvidedStaticParameter list * instantiationFunction: (string -> obj[] -> ProvidedTypeDefinition) -> unit

    /// Add definition location information to the provided type definition.
    member AddDefinitionLocation : line:int * column:int * filePath:string -> unit

    /// Suppress Object entries in intellisense menus in instances of this provided type 
    member HideObjectMethods  : bool with set

    /// Disallows the use of the null literal. 
    member NonNullable : bool with set

    /// Get or set a flag indicating if the ProvidedTypeDefinition is erased
    member IsErased : bool  with get,set

    /// Get or set a flag indicating if the ProvidedTypeDefinition has type-relocation suppressed
    [<Experimental("SuppressRelocation is a workaround and likely to be removed")>]
    member SuppressRelocation : bool  with get,set

    /// FSharp.Data addition: this method is used by Debug.fs
    member MakeParametricType : name:string * args:obj[] -> ProvidedTypeDefinition

    /// Add a custom attribute to the provided type definition.
    member AddCustomAttribute : CustomAttributeData -> unit

    /// Emulate the F# type provider type erasure mechanism to get the 
    /// actual (erased) type. We erase ProvidedTypes to their base type
    /// and we erase array of provided type to array of base type. In the
    /// case of generics all the generic type arguments are also recursively
    /// replaced with the erased-to types
    static member EraseType : typ:Type -> Type

    /// Get or set a utility function to log the creation of root Provided Type. Used to debug caching/invalidation.
    static member Logger : (string -> unit) option ref

/// A provided generated assembly
type ProvidedAssembly =
    /// Create a provided generated assembly
    new : assemblyFileName:string -> ProvidedAssembly

    /// Emit the given provided type definitions as part of the assembly 
    /// and adjust the 'Assembly' property of all provided type definitions to return that
    /// assembly.
    ///
    /// The assembly is only emitted when the Assembly property on the root type is accessed for the first time.
    /// The host F# compiler does this when processing a generative type declaration for the type.
    member AddTypes : types : ProvidedTypeDefinition list -> unit

    /// <summary>
    /// Emit the given nested provided type definitions as part of the assembly.
    /// and adjust the 'Assembly' property of all provided type definitions to return that
    /// assembly.
    /// </summary>
    /// <param name="enclosingTypeNames">A path of type names to wrap the generated types. The generated types are then generated as nested types.</param>
    member AddNestedTypes : types : ProvidedTypeDefinition list * enclosingGeneratedTypeNames: string list -> unit

#if FX_NO_LOCAL_FILESYSTEM
#else
    /// Register that a given file is a provided generated assembly
    static member RegisterGenerated : fileName:string -> Assembly
#endif


/// A base type providing default implementations of type provider functionality when all provided 
/// types are of type ProvidedTypeDefinition.
type TypeProviderForNamespaces =

    /// Initializes a type provider to provide the types in the given namespace.
    new : namespaceName:string * types: ProvidedTypeDefinition list -> TypeProviderForNamespaces

    /// Initializes a type provider 
    new : unit -> TypeProviderForNamespaces

    /// Invoked by the type provider to add a namespace of provided types in the specification of the type provider.
    member AddNamespace : namespaceName:string * types: ProvidedTypeDefinition list -> unit

    /// Invoked by the type provider to get all provided namespaces with their provided types.
    member Namespaces : seq<string * ProvidedTypeDefinition list> 

    /// Invoked by the type provider to invalidate the information provided by the provider
    member Invalidate : unit -> unit

    /// Invoked by the host of the type provider to get the static parameters for a method.
    member GetStaticParametersForMethod : MethodBase -> ParameterInfo[]
    
    /// Invoked by the host of the type provider to apply the static argumetns for a method.
    member ApplyStaticArgumentsForMethod : MethodBase * string * obj[] -> MethodBase

#if FX_NO_LOCAL_FILESYSTEM
#else
    /// AssemblyResolve handler. Default implementation searches <assemblyname>.dll file in registered folders 
    abstract ResolveAssembly : ResolveEventArgs -> Assembly
    default ResolveAssembly : ResolveEventArgs -> Assembly

    /// Registers custom probing path that can be used for probing assemblies
    member RegisterProbingFolder : folder: string -> unit

    /// Registers location of RuntimeAssembly (from TypeProviderConfig) as probing folder
    member RegisterRuntimeAssemblyLocationAsProbingFolder : config: TypeProviderConfig -> unit

#endif

    [<CLIEvent>]
    member Disposing : IEvent<EventHandler,EventArgs>

    interface ITypeProvider


module internal UncheckedQuotations = 

  type Expr with
    static member NewDelegateUnchecked : ty:Type * vs:Var list * body:Expr -> Expr
    static member NewObjectUnchecked : cinfo:ConstructorInfo * args:Expr list -> Expr
    static member NewArrayUnchecked : elementType:Type * elements:Expr list -> Expr
    static member CallUnchecked : minfo:MethodInfo * args:Expr list -> Expr
    static member CallUnchecked : obj:Expr * minfo:MethodInfo * args:Expr list -> Expr
    static member ApplicationUnchecked : f:Expr * x:Expr -> Expr
    static member PropertyGetUnchecked : pinfo:PropertyInfo * args:Expr list -> Expr
    static member PropertyGetUnchecked : obj:Expr * pinfo:PropertyInfo * ?args:Expr list -> Expr
    static member PropertySetUnchecked : pinfo:PropertyInfo * value:Expr * ?args:Expr list -> Expr
    static member PropertySetUnchecked : obj:Expr * pinfo:PropertyInfo * value:Expr * args:Expr list -> Expr
    static member FieldGetUnchecked : pinfo:FieldInfo -> Expr
    static member FieldGetUnchecked : obj:Expr * pinfo:FieldInfo -> Expr
    static member FieldSetUnchecked : pinfo:FieldInfo * value:Expr -> Expr
    static member FieldSetUnchecked : obj:Expr * pinfo:FieldInfo * value:Expr -> Expr
    static member TupleGetUnchecked : e:Expr * n:int -> Expr
    static member LetUnchecked : v:Var * e:Expr * body:Expr -> Expr

  type Shape
  val ( |ShapeCombinationUnchecked|ShapeVarUnchecked|ShapeLambdaUnchecked| ) : e:Expr -> Choice<(Shape * Expr list),Var, (Var * Expr)>
  val RebuildShapeCombinationUnchecked : Shape * args:Expr list -> Expr
