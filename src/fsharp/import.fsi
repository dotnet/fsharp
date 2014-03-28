// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Import

open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.AbstractIL.IL
#if EXTENSIONTYPING
open Microsoft.FSharp.Compiler.ExtensionTyping
#endif



type AssemblyLoader = 
    abstract LoadAssembly : range * ILAssemblyRef -> CcuResolutionResult
#if EXTENSIONTYPING
    /// Get a flag indicating if an assembly is a provided assembly, plus the
    /// table of information recording remappings from type names in the provided assembly to type
    /// names in the statically linked, embedded assembly.
    abstract GetProvidedAssemblyInfo : range * Tainted<ProvidedAssembly> -> bool * ProvidedAssemblyStaticLinkingMap option
    /// Record a root for a [<Generate>] type to help guide static linking & type relocation
    abstract RecordGeneratedTypeRoot : ProviderGeneratedType -> unit
#endif


[<SealedAttribute ()>]
/// This is the context used for converting AbstractIL .NET and provided types to F# internal compiler data structures.
/// We currently cache the conversion of AbstractIL ILTypeRef nodes, based on hashes of these.
///
/// There is normally only one ImportMap for any assembly compilation, though additional instances can be created
/// using tcImports.GetImportMap() if needed, and it is not harmful if multiple instances are used. The object 
/// serves as an interface through to the tables stored in the primary TcImports structures defined in build.fs. 
type ImportMap =
    new : g:Env.TcGlobals * assemblyLoader:AssemblyLoader -> ImportMap
    member assemblyLoader : AssemblyLoader
    member g : Env.TcGlobals

val internal ImportILTypeRef : ImportMap -> range -> ILTypeRef -> TyconRef
val internal ImportILType : ImportMap -> range -> TType list -> ILType -> TType
#if EXTENSIONTYPING
val internal ImportProvidedType : ImportMap -> range -> (* TType list -> *) Tainted<ProvidedType> -> TType
val internal ImportProvidedNamedType : ImportMap -> range -> (* TType list -> *) Tainted<ProvidedType> -> TyconRef
val internal ImportProvidedTypeAsILType : ImportMap -> range -> Tainted<ProvidedType> -> ILType
val internal ImportProvidedMethodBaseAsILMethodRef : ImportMap -> range -> Tainted<ProvidedMethodBase> -> ILMethodRef
#endif
val internal ImportILGenericParameters : (unit -> ImportMap) -> range -> ILScopeRef -> TType list -> ILGenericParameterDef list -> Typar list
val internal ImportILAssembly : (unit -> ImportMap) * range * (ILScopeRef -> ILModuleDef) * ILScopeRef * sourceDir:string * filename: string option * ILModuleDef * IEvent<string> -> CcuThunk
val internal ImportILAssemblyTypeForwarders : (unit -> ImportMap) * range * ILExportedTypesAndForwarders -> Map<(string array * string), Lazy<EntityRef>>
