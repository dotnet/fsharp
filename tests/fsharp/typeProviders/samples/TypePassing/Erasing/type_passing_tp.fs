namespace Test

open System
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes


[<TypeProvider>]
type TypePassingTp(config: TypeProviderConfig) as this =     
    inherit TypeProviderForNamespaces()

    let ns = "TypePassing"
    let runtimeAssembly = Assembly.LoadFrom(config.RuntimeAssembly)

    let createTypes (ty:Type) typeName = 
        let rootType = ProvidedTypeDefinition(runtimeAssembly,ns,typeName,baseType= (Some typeof<obj>), HideObjectMethods=true)
        rootType.AddMember(ProvidedProperty("Name", typeof<string>, GetterCode = (fun args -> let v = ty.Name in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("FullName", typeof<string>, GetterCode = (fun args -> let v = ty.FullName in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("AssemblyQualifiedName", typeof<string>, GetterCode = (fun args -> let v = ty.AssemblyQualifiedName in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsAbstract", typeof<bool>, GetterCode = (fun args -> let v = ty.IsAbstract in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsAnsiClass", typeof<bool>, GetterCode = (fun args -> let v = ty.IsAnsiClass in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsArray", typeof<bool>, GetterCode = (fun args -> let v = ty.IsArray in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsAutoClass", typeof<bool>, GetterCode = (fun args -> let v = ty.IsAutoClass in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsAutoLayout", typeof<bool>, GetterCode = (fun args -> let v = ty.IsAutoLayout in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsByRef", typeof<bool>, GetterCode = (fun args -> let v = ty.IsByRef in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsClass", typeof<bool>, GetterCode = (fun args -> let v = ty.IsClass in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsValueType", typeof<bool>, GetterCode = (fun args -> let v = ty.IsValueType in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsInterface", typeof<bool>, GetterCode = (fun args -> let v = ty.IsInterface in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsGenericParameter", typeof<bool>, GetterCode = (fun args -> let v = ty.IsGenericParameter in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsNested", typeof<bool>, GetterCode = (fun args -> let v = ty.IsNested in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsNestedPublic", typeof<bool>, GetterCode = (fun args -> let v = ty.IsNestedPublic in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsPublic", typeof<bool>, GetterCode = (fun args -> let v = ty.IsPublic in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsNotPublic", typeof<bool>, GetterCode = (fun args -> let v = ty.IsNotPublic in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsSealed", typeof<bool>, GetterCode = (fun args -> let v = ty.IsSealed in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsRecord", typeof<bool>, GetterCode = (fun args -> let v = Reflection.FSharpType.IsRecord(ty) in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsFunction", typeof<bool>, GetterCode = (fun args -> let v = Reflection.FSharpType.IsFunction(ty) in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsModule", typeof<bool>, GetterCode = (fun args -> let v = Reflection.FSharpType.IsModule(ty) in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsExceptionRepresentation", typeof<bool>, GetterCode = (fun args -> let v = Reflection.FSharpType.IsExceptionRepresentation(ty) in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsTuple", typeof<bool>, GetterCode = (fun args -> let v = Reflection.FSharpType.IsTuple(ty) in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("IsUnion", typeof<bool>, GetterCode = (fun args -> let v = Reflection.FSharpType.IsUnion(ty) in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("GetPublicProperties_Length", typeof<int>, GetterCode = (fun args -> let v = ty.GetProperties().Length in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("GetPublicConstructors_Length", typeof<int>, GetterCode = (fun args -> let v = ty.GetConstructors().Length in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("GetPublicMethods_Length", typeof<int>, GetterCode = (fun args -> let v = ty.GetMethods().Length in <@@ v  @@> ), IsStatic=true))        

        // Raises error is used in program
        rootType.AddMember(ProvidedProperty("Assembly_CodeBase", typeof<string>, GetterCode = (fun args -> let v = ty.Assembly.CodeBase in <@@ v  @@> ), IsStatic=true))        
        
        // Raises error is used in program
        rootType.AddMember(ProvidedProperty("Assembly_CustomAttributes_Count", typeof<int>, GetterCode = (fun args -> let v = Seq.length ty.Assembly.CustomAttributes in <@@ v  @@> ), IsStatic=true))        
        
        // Always returns 0
        rootType.AddMember(ProvidedProperty("Assembly_DefinedTypes_Count", typeof<int>, GetterCode = (fun args -> let v = Seq.length ty.Assembly.DefinedTypes in <@@ v  @@> ), IsStatic=true))        

        rootType.AddMember(ProvidedProperty("Assembly_FullName", typeof<string>, GetterCode = (fun args -> let v = ty.Assembly.FullName in <@@ v  @@> ), IsStatic=true))        
        rootType.AddMember(ProvidedProperty("Assembly_EntryPoint_isNull", typeof<bool>, GetterCode = (fun args -> let v = isNull ty.Assembly.EntryPoint in <@@ v  @@> ), IsStatic=true))        
        // TODO - more here
        rootType.AddMember(ProvidedProperty("GUID", typeof<string>, GetterCode = (fun args -> let v = ty.GUID.ToString() in <@@ v  @@> ), IsStatic=true))        
        rootType

    let paramType = ProvidedTypeDefinition(runtimeAssembly, ns, "Summarize", Some(typeof<obj>), HideObjectMethods = true)
    
    do paramType.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> createTypes (unbox args.[0]) typeName)

    do this.AddNamespace(ns, [paramType])
                            
[<assembly:TypeProviderAssembly>] 
do()

