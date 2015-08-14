// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Extension typing, validation of extension types, etc.

namespace Microsoft.FSharp.Compiler

module internal ExtensionTyping =

    open System
    open System.IO
    open System.Collections.Generic
    open Microsoft.FSharp.Core.CompilerServices
    open Microsoft.FSharp.Compiler.AbstractIL.IL
    open Microsoft.FSharp.Compiler.Range

#if TYPE_PROVIDER_SECURITY
    // These global variables are used by the VS language service for the type provider security dialog
    module internal GlobalsTheLanguageServiceCanPoke =
        //+++ GLOBAL STATE
        val mutable displayLSTypeProviderSecurityDialogBlockingUI : (string->unit) option
        val mutable theMostRecentFileNameWeChecked : string option

    module internal ApprovalIO =

        val partiallyCanonicalizeFileName : string -> string

        /// location of approvals data file, e.g. C:\Users\username\AppData\Local\Microsoft\VisualStudio\14.0\type-providers.txt
        val ApprovalsAbsoluteFileName  : string

        [<RequireQualifiedAccess>]
        type TypeProviderApprovalStatus =
            /// NotTrusted(absoluteFileName)
            | NotTrusted of string
            /// Trusted(absoluteFileName)
            | Trusted of string

        /// Try to do an operation on the type-provider approvals file
        val DoWithApprovalsFile : FileStream option -> (FileStream -> 'a) -> 'a

        /// Read all TP approval data.  does not throw, will swallow exceptions and return empty list if there's trouble.
        val ReadApprovalsFile : FileStream option -> TypeProviderApprovalStatus list

        /// Replace one piece of TP approval info (or append it, if this is a new filename).  may throw if trouble with file IO.
        val ReplaceApprovalStatus : FileStream option -> TypeProviderApprovalStatus -> unit
#endif

    type TypeProviderDesignation = TypeProviderDesignation of string

    /// Raised when a type provider has thrown an exception.    
    exception ProvidedTypeResolution of range * exn

    /// Raised when an type provider has thrown an exception.    
    exception ProvidedTypeResolutionNoRange of exn

    /// Carries information about the type provider resolution environment.
    type ResolutionEnvironment =
      {
        /// The folder from which an extension provider is resolving from. This is typically the project folder.
        resolutionFolder            : string
        /// Output file name
        outputFile                  : string option
        /// Whether or not the --showextensionresolution flag was supplied to the compiler.
        showResolutionMessages      : bool
        
        /// All referenced assemblies, including the type provider itself, and possibly other type providers.
        referencedAssemblies        : string[]

        /// The folder for temporary files
        temporaryFolder             : string
      }

    /// Find and instantiate the set of ITypeProvider components for the given assembly reference
    val GetTypeProvidersOfAssembly : 
          displayPSTypeProviderSecurityDialogBlockingUI : (string->unit) option 
          * validateTypeProviders: bool 
#if TYPE_PROVIDER_SECURITY
          * ApprovalIO.TypeProviderApprovalStatus list 
#endif
          * runtimeAssemblyFilename: string 
          * ilScopeRefOfRuntimeAssembly:ILScopeRef
          * designerAssemblyName: string 
          * ResolutionEnvironment 
          * bool
          * isInteractive: bool
          * systemRuntimeContainsType : (string -> bool)
          * systemRuntimeAssemblyVersion : System.Version
          * range -> bool * Tainted<ITypeProvider> list

    /// Given an extension type resolver, supply a human-readable name suitable for error messages.
    val DisplayNameOfTypeProvider : Tainted<Microsoft.FSharp.Core.CompilerServices.ITypeProvider> * range -> string

     /// The context used to interpret information in the closure of System.Type, System.MethodInfo and other 
     /// info objects coming from the type provider.
     ///
     /// At the moment this is the "Type --> ILTypeRef" and "Type --> Tycon" remapping 
     /// context for generated types (it is empty for erased types). This is computed from
     /// while processing the [<Generate>] declaration related to the type.
     ///
     /// Immutable (after type generation for a [<Generate>] declaration populates the dictionaries).
     ///
     /// The 'obj' values are all TyconRef, but obj is used due to a forward reference being required. Not particularly
     /// pleasant, but better than intertwining the whole "ProvidedType" with the TAST structure.
    [<Sealed>]
    type ProvidedTypeContext =

        member TryGetILTypeRef : System.Type -> ILTypeRef option

        member TryGetTyconRef : System.Type -> obj option

        static member Empty : ProvidedTypeContext 

        static member Create : Dictionary<System.Type,ILTypeRef> * Dictionary<System.Type,obj (* TyconRef *) > -> ProvidedTypeContext 

        member GetDictionaries : unit -> Dictionary<System.Type,ILTypeRef> * Dictionary<System.Type,obj (* TyconRef *) > 

        /// Map the TyconRef objects, if any
        member RemapTyconRefs : (obj -> obj) -> ProvidedTypeContext 
           
#if FX_NO_CUSTOMATTRIBUTEDATA
    type CustomAttributeData = Microsoft.FSharp.Core.CompilerServices.IProvidedCustomAttributeData
    type CustomAttributeNamedArgument = Microsoft.FSharp.Core.CompilerServices.IProvidedCustomAttributeNamedArgument
    type CustomAttributeTypedArgument = Microsoft.FSharp.Core.CompilerServices.IProvidedCustomAttributeTypedArgument
#endif

    type [<AllowNullLiteral; Sealed; Class>] 
        ProvidedType =
        inherit ProvidedMemberInfo
        member IsSuppressRelocate : bool
        member IsErased : bool
        member IsGenericType : bool
        member Namespace : string
        member FullName : string
        member IsArray : bool
        member GetInterfaces : unit -> ProvidedType[]
        member Assembly : ProvidedAssembly
        member BaseType : ProvidedType
        member GetNestedType : string -> ProvidedType
        member GetNestedTypes : unit -> ProvidedType[]
        member GetAllNestedTypes : unit -> ProvidedType[]
        member GetMethods : unit -> ProvidedMethodInfo[]
        member GetFields : unit -> ProvidedFieldInfo[]
        member GetField : string -> ProvidedFieldInfo
        member GetProperties : unit -> ProvidedPropertyInfo[]
        member GetProperty : string -> ProvidedPropertyInfo
        member GetEvents : unit -> ProvidedEventInfo[]
        member GetEvent : string -> ProvidedEventInfo
        member GetConstructors : unit -> ProvidedConstructorInfo[]
        member GetStaticParameters : ITypeProvider -> ProvidedParameterInfo[]
        member GetGenericTypeDefinition : unit -> ProvidedType
        member IsVoid : bool
        member IsGenericParameter : bool
        member IsValueType : bool
        member IsByRef : bool
        member IsPointer : bool
        member IsEnum : bool
        member IsInterface : bool
        member IsClass : bool
        member IsSealed : bool
        member IsPublic : bool
        member IsNestedPublic : bool
        member GenericParameterPosition : int
        member GetElementType : unit -> ProvidedType
        member GetGenericArguments : unit -> ProvidedType[]
        member GetArrayRank : unit -> int
        member RawSystemType : System.Type
        member GetEnumUnderlyingType : unit -> ProvidedType
        static member Void : ProvidedType
        static member CreateNoContext : Type -> ProvidedType
        member TryGetILTypeRef : unit -> ILTypeRef option
        member TryGetTyconRef : unit -> obj option
        static member ApplyContext : ProvidedType * ProvidedTypeContext -> ProvidedType
        member Context : ProvidedTypeContext 
        interface IProvidedCustomAttributeProvider
        static member TaintedEquals : Tainted<ProvidedType> * Tainted<ProvidedType> -> bool 

    and [<AllowNullLiteral>] 
        IProvidedCustomAttributeProvider =
        abstract GetHasTypeProviderEditorHideMethodsAttribute : provider:ITypeProvider -> bool
        abstract GetDefinitionLocationAttribute : provider:ITypeProvider -> (string * int * int) option 
        abstract GetXmlDocAttributes : provider:ITypeProvider -> string[]
        abstract GetAttributeConstructorArgs: provider:ITypeProvider * attribName:string -> (obj option list * (string * obj option) list) option
        
    and [<AllowNullLiteral; Sealed; Class>] 
        ProvidedAssembly = 
        member GetName : unit -> System.Reflection.AssemblyName
        member FullName : string
        member GetManifestModuleContents : ITypeProvider -> byte[]
        member Handle : System.Reflection.Assembly

    and [<AllowNullLiteral;AbstractClass>] 
        ProvidedMemberInfo = 
        member Name :string
        member DeclaringType : ProvidedType
        interface IProvidedCustomAttributeProvider 

    and [<AllowNullLiteral;AbstractClass>] 
        ProvidedMethodBase = 
        inherit ProvidedMemberInfo
        member IsGenericMethod : bool
        member IsStatic : bool
        member IsFamily : bool
        member IsFamilyAndAssembly : bool
        member IsFamilyOrAssembly : bool
        member IsVirtual : bool
        member IsFinal : bool
        member IsPublic : bool
        member IsAbstract : bool
        member IsHideBySig : bool
        member IsConstructor : bool
        member GetParameters : unit -> ProvidedParameterInfo[]
        member GetGenericArguments : unit -> ProvidedType[]
        member GetStaticParametersForMethod : ITypeProvider -> ProvidedParameterInfo[]
        static member TaintedGetHashCode : Tainted<ProvidedMethodBase> -> int
        static member TaintedEquals : Tainted<ProvidedMethodBase> * Tainted<ProvidedMethodBase> -> bool 

    and [<AllowNullLiteral; Sealed; Class>] 
        ProvidedMethodInfo = 
        inherit ProvidedMethodBase
        member ReturnType : ProvidedType
        member MetadataToken : int

    and [<AllowNullLiteral; Sealed; Class>] 
        ProvidedParameterInfo = 
        member Name :string
        member ParameterType : ProvidedType
        member IsIn : bool
        member IsOut : bool
        member IsOptional : bool
        member RawDefaultValue : obj
        member HasDefaultValue : bool
        interface IProvidedCustomAttributeProvider 

    and [<AllowNullLiteral; Class; Sealed>] 
        ProvidedFieldInfo = 
        inherit ProvidedMemberInfo
        member IsInitOnly : bool
        member IsStatic : bool
        member IsSpecialName : bool
        member IsLiteral : bool
        member GetRawConstantValue : unit -> obj
        member FieldType : ProvidedType
        member IsPublic : bool
        member IsFamily : bool
        member IsFamilyAndAssembly : bool
        member IsFamilyOrAssembly : bool
        member IsPrivate : bool

    and [<AllowNullLiteral; Class; Sealed>] 
        ProvidedPropertyInfo = 
        inherit ProvidedMemberInfo
        member GetGetMethod : unit -> ProvidedMethodInfo
        member GetSetMethod : unit -> ProvidedMethodInfo
        member GetIndexParameters : unit -> ProvidedParameterInfo[]
        member CanRead : bool
        member CanWrite : bool
        member PropertyType : ProvidedType
        static member TaintedGetHashCode : Tainted<ProvidedPropertyInfo> -> int
        static member TaintedEquals : Tainted<ProvidedPropertyInfo> * Tainted<ProvidedPropertyInfo> -> bool 

    and [<AllowNullLiteral; Class; Sealed>] 
        ProvidedEventInfo = 
        inherit ProvidedMemberInfo
        member GetAddMethod : unit -> ProvidedMethodInfo
        member GetRemoveMethod : unit -> ProvidedMethodInfo
        member EventHandlerType : ProvidedType
        static member TaintedGetHashCode : Tainted<ProvidedEventInfo> -> int
        static member TaintedEquals : Tainted<ProvidedEventInfo> * Tainted<ProvidedEventInfo> -> bool 

    and [<AllowNullLiteral; Class; Sealed>] 
        ProvidedConstructorInfo = 
        inherit ProvidedMethodBase
        
    [<RequireQualifiedAccess; Class; Sealed; AllowNullLiteral>]
    type ProvidedExpr =
        member Type : ProvidedType
        /// Convert the expression to a string for diagnostics
        member UnderlyingExpressionString : string

    [<RequireQualifiedAccess; Class; Sealed; AllowNullLiteral>]
    type ProvidedVar =
        member Type : ProvidedType
        member Name : string
        member IsMutable : bool
        static member Fresh : string * ProvidedType -> ProvidedVar
        override Equals : obj -> bool
        override GetHashCode : unit -> int

    /// Detect a provided new-array expression 
    val (|ProvidedNewArrayExpr|_|)   : ProvidedExpr -> (ProvidedType * ProvidedExpr[]) option

#if PROVIDED_ADDRESS_OF
    val (|ProvidedAddressOfExpr|_|)  : ProvidedExpr -> ProvidedExpr option
#endif

    /// Detect a provided new-object expression 
    val (|ProvidedNewObjectExpr|_|)     : ProvidedExpr -> (ProvidedConstructorInfo * ProvidedExpr[]) option

    /// Detect a provided while-loop expression 
    val (|ProvidedWhileLoopExpr|_|) : ProvidedExpr -> (ProvidedExpr * ProvidedExpr) option

    /// Detect a provided new-delegate expression 
    val (|ProvidedNewDelegateExpr|_|) : ProvidedExpr -> (ProvidedType * ProvidedVar[] * ProvidedExpr) option

    /// Detect a provided expression which is a for-loop over integers
    val (|ProvidedForIntegerRangeLoopExpr|_|) : ProvidedExpr -> (ProvidedVar * ProvidedExpr * ProvidedExpr * ProvidedExpr) option

    /// Detect a provided sequential expression 
    val (|ProvidedSequentialExpr|_|)    : ProvidedExpr -> (ProvidedExpr * ProvidedExpr) option

    /// Detect a provided try/with expression 
    val (|ProvidedTryWithExpr|_|)       : ProvidedExpr -> (ProvidedExpr * ProvidedVar * ProvidedExpr * ProvidedVar * ProvidedExpr) option

    /// Detect a provided try/finally expression 
    val (|ProvidedTryFinallyExpr|_|)    : ProvidedExpr -> (ProvidedExpr * ProvidedExpr) option

    /// Detect a provided lambda expression 
    val (|ProvidedLambdaExpr|_|)     : ProvidedExpr -> (ProvidedVar * ProvidedExpr) option

    /// Detect a provided call expression 
    val (|ProvidedCallExpr|_|) : ProvidedExpr -> (ProvidedExpr option * ProvidedMethodInfo * ProvidedExpr[]) option

    /// Detect a provided constant expression 
    val (|ProvidedConstantExpr|_|)   : ProvidedExpr -> (obj * ProvidedType) option

    /// Detect a provided default-value expression 
    val (|ProvidedDefaultExpr|_|)    : ProvidedExpr -> ProvidedType option

    /// Detect a provided new-tuple expression 
    val (|ProvidedNewTupleExpr|_|)   : ProvidedExpr -> ProvidedExpr[] option

    /// Detect a provided tuple-get expression 
    val (|ProvidedTupleGetExpr|_|)   : ProvidedExpr -> (ProvidedExpr * int) option

    /// Detect a provided type-as expression 
    val (|ProvidedTypeAsExpr|_|)      : ProvidedExpr -> (ProvidedExpr * ProvidedType) option

    /// Detect a provided type-test expression 
    val (|ProvidedTypeTestExpr|_|)      : ProvidedExpr -> (ProvidedExpr * ProvidedType) option

    /// Detect a provided 'let' expression 
    val (|ProvidedLetExpr|_|)      : ProvidedExpr -> (ProvidedVar * ProvidedExpr * ProvidedExpr) option

    /// Detect a provided 'set variable' expression 
    val (|ProvidedVarSetExpr|_|)      : ProvidedExpr -> (ProvidedVar * ProvidedExpr) option

    /// Detect a provided 'IfThenElse' expression 
    val (|ProvidedIfThenElseExpr|_|) : ProvidedExpr -> (ProvidedExpr * ProvidedExpr * ProvidedExpr) option

    /// Detect a provided 'Var' expression 
    val (|ProvidedVarExpr|_|)  : ProvidedExpr -> ProvidedVar option

    /// Get the provided expression for a particular use of a method.
    val GetInvokerExpression : ITypeProvider * ProvidedMethodBase * ProvidedVar[] ->  ProvidedExpr

    /// Validate that the given provided type meets some of the rules for F# provided types
    val ValidateProvidedTypeAfterStaticInstantiation : range * Tainted<ProvidedType> * expectedPath : string[] * expectedName : string-> unit

    /// Try to apply a provided type to the given static arguments. If successful also return a function 
    /// to check the type name is as expected (this function is called by the caller of TryApplyProvidedType
    /// after other checks are made).
    val TryApplyProvidedType : typeBeforeArguments:Tainted<ProvidedType> * optGeneratedTypePath: string list option * staticArgs:obj[]  * range -> (Tainted<ProvidedType> * (unit -> unit)) option

    /// Try to apply a provided method to the given static arguments. 
    val TryApplyProvidedMethod : methBeforeArguments:Tainted<ProvidedMethodBase> * staticArgs:obj[]  * range -> Tainted<ProvidedMethodBase> option

    /// Try to resolve a type in the given extension type resolver
    val TryResolveProvidedType : ResolutionEnvironment * Tainted<ITypeProvider> * range * string[] * typeName: string -> Tainted<ProvidedType> option

    /// Try to resolve a type in the given extension type resolver
    val TryLinkProvidedType : ResolutionEnvironment * Tainted<ITypeProvider> * string[] * typeLogicalName: string * range: range -> Tainted<ProvidedType> option

    /// Get the parts of a .NET namespace. Special rules: null means global, empty is not allowed.
    val GetProvidedNamespaceAsPath : range * Tainted<ITypeProvider> * string -> string list

    /// Decompose the enclosing name of a type (including any class nestings) into a list of parts.
    /// e.g. System.Object -> ["System"; "Object"]
    val GetFSharpPathToProvidedType : Tainted<ProvidedType> * range:range-> string list
    
    /// Get the ILTypeRef for the provided type (including for nested types). Take into account
    /// any type relocations or static linking for generated types.
    val GetILTypeRefOfProvidedType : Tainted<ProvidedType> * range:range -> Microsoft.FSharp.Compiler.AbstractIL.IL.ILTypeRef

    /// Get the ILTypeRef for the provided type (including for nested types). Do not take into account
    /// any type relocations or static linking for generated types.
    val GetOriginalILTypeRefOfProvidedType : Tainted<ProvidedType> * range:range -> Microsoft.FSharp.Compiler.AbstractIL.IL.ILTypeRef


    /// Represents the remapping information for a generated provided type and its nested types.
    ///
    /// There is one overall tree for each root 'type X = ... type generation expr...' specification.
    type ProviderGeneratedType = ProviderGeneratedType of (*ilOrigTyRef*)ILTypeRef * (*ilRenamedTyRef*)ILTypeRef * ProviderGeneratedType list

    /// The table of information recording remappings from type names in the provided assembly to type
    /// names in the statically linked, embedded assembly, plus what types are nested in side what types.
    type ProvidedAssemblyStaticLinkingMap = 
        {  /// The table of remappings from type names in the provided assembly to type
           /// names in the statically linked, embedded assembly.
           ILTypeMap: System.Collections.Generic.Dictionary<ILTypeRef, ILTypeRef> }
        
        /// Create a new static linking map, ready to populate with data.
        static member CreateNew : unit -> ProvidedAssemblyStaticLinkingMap

    /// Check if this is a direct reference to a non-embedded generated type. This is not permitted at any name resolution.
    /// We check by seeing if the type is absent from the remapping context.
    val IsGeneratedTypeDirectReference         : Tainted<ProvidedType> * range -> bool
