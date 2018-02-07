namespace Test

open System
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes


[<TypeProvider>]
type TypePassingTp(config: TypeProviderConfig) as this =     
    inherit TypeProviderForNamespaces(config)

    let ns = "TypePassing"
    let runtimeAssembly = Assembly.LoadFrom(config.RuntimeAssembly)

    let createTypes (ty:Type) typeName = 
        let rootType = ProvidedTypeDefinition(runtimeAssembly,ns,typeName,baseType= (Some typeof<obj>), hideObjectMethods=true)
        rootType.AddMember(ProvidedProperty("Name", typeof<string>, getterCode = (fun args -> let v = ty.Name in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("FullName", typeof<string>, getterCode = (fun args -> let v = ty.FullName in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("AssemblyQualifiedName", typeof<string>, getterCode = (fun args -> let v = ty.AssemblyQualifiedName in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsAbstract", typeof<bool>, getterCode = (fun args -> let v = ty.IsAbstract in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsAnsiClass", typeof<bool>, getterCode = (fun args -> let v = ty.IsAnsiClass in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsArray", typeof<bool>, getterCode = (fun args -> let v = ty.IsArray in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsAutoClass", typeof<bool>, getterCode = (fun args -> let v = ty.IsAutoClass in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsAutoLayout", typeof<bool>, getterCode = (fun args -> let v = ty.IsAutoLayout in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsByRef", typeof<bool>, getterCode = (fun args -> let v = ty.IsByRef in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsClass", typeof<bool>, getterCode = (fun args -> let v = ty.IsClass in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsValueType", typeof<bool>, getterCode = (fun args -> let v = ty.IsValueType in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsInterface", typeof<bool>, getterCode = (fun args -> let v = ty.IsInterface in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsGenericParameter", typeof<bool>, getterCode = (fun args -> let v = ty.IsGenericParameter in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsNested", typeof<bool>, getterCode = (fun args -> let v = ty.IsNested in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsNestedPublic", typeof<bool>, getterCode = (fun args -> let v = ty.IsNestedPublic in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsPublic", typeof<bool>, getterCode = (fun args -> let v = ty.IsPublic in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsNotPublic", typeof<bool>, getterCode = (fun args -> let v = ty.IsNotPublic in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsSealed", typeof<bool>, getterCode = (fun args -> let v = ty.IsSealed in <@@ v  @@> ), isStatic=true))     
        rootType.AddMember(ProvidedProperty("IsGenericType", typeof<bool>, getterCode = (fun args -> let v = ty.IsGenericType in <@@ v  @@> ), isStatic=true))  
        rootType.AddMember(ProvidedProperty("IsGenericTypeDefinition", typeof<bool>, getterCode = (fun args -> let v = ty.IsGenericTypeDefinition in <@@ v  @@> ), isStatic=true))         
        rootType.AddMember(ProvidedProperty("IsRecord", typeof<bool>, getterCode = (fun args -> let v = Reflection.FSharpType.IsRecord(ty) in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsFunction", typeof<bool>, getterCode = (fun args -> let v = Reflection.FSharpType.IsFunction(ty) in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsModule", typeof<bool>, getterCode = (fun args -> let v = Reflection.FSharpType.IsModule(ty) in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsExceptionRepresentation", typeof<bool>, getterCode = (fun args -> let v = Reflection.FSharpType.IsExceptionRepresentation(ty) in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsTuple", typeof<bool>, getterCode = (fun args -> let v = Reflection.FSharpType.IsTuple(ty) in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("IsUnion", typeof<bool>, getterCode = (fun args -> let v = Reflection.FSharpType.IsUnion(ty) in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("GetPublicProperties_Length", typeof<int>, getterCode = (fun args -> let v = ty.GetProperties().Length in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("GetPublicConstructors_Length", typeof<int>, getterCode = (fun args -> let v = ty.GetConstructors().Length in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("GetPublicMethods_Length", typeof<int>, getterCode = (fun args -> let v = ty.GetMethods().Length in <@@ v  @@> ), isStatic=true))    
        rootType.AddMember(ProvidedProperty("GetGenericArguments_Length", typeof<int>, getterCode = (fun args -> let v = ty.GetGenericArguments().Length in <@@ v  @@> ), isStatic=true))   

        rootType.AddMember(ProvidedProperty("CustomAttribute_Names", typeof<string[]>, getterCode = (fun args -> let v = (ty.CustomAttributes |> Seq.map (fun x -> x.AttributeType.Name) |> Seq.toArray) in <@@ v  @@> ), isStatic=true))      
        rootType.AddMember(ProvidedProperty("CustomAttributes_Length", typeof<int>, getterCode = (fun args -> let v = Seq.length ty.CustomAttributes in <@@ v  @@> ), isStatic=true))
        // Raises error is used in program
        rootType.AddMember(ProvidedProperty("Assembly_CodeBase", typeof<string>, getterCode = (fun args -> let v = ty.Assembly.CodeBase in <@@ v  @@> ), isStatic=true))        
        
        // Raises error is used in program
        rootType.AddMember(ProvidedProperty("Assembly_CustomAttributes_Count", typeof<int>, getterCode = (fun args -> let v = Seq.length ty.Assembly.CustomAttributes in <@@ v  @@> ), isStatic=true))        
        
        // Always returns 0
        rootType.AddMember(ProvidedProperty("Assembly_DefinedTypes_Count", typeof<int>, getterCode = (fun args -> let v = Seq.length ty.Assembly.DefinedTypes in <@@ v  @@> ), isStatic=true))        

        rootType.AddMember(ProvidedProperty("Assembly_FullName", typeof<string>, getterCode = (fun args -> let v = ty.Assembly.FullName in <@@ v  @@> ), isStatic=true))        
        rootType.AddMember(ProvidedProperty("Assembly_EntryPoint_isNull", typeof<bool>, getterCode = (fun args -> let v = isNull ty.Assembly.EntryPoint in <@@ v  @@> ), isStatic=true))        
        // TODO - more here
        rootType.AddMember(ProvidedProperty("GUID", typeof<string>, getterCode = (fun args -> let v = ty.GUID.ToString() in <@@ v  @@> ), isStatic=true))        
        rootType

    let paramType = ProvidedTypeDefinition(runtimeAssembly, ns, "Summarize", Some(typeof<obj>), hideObjectMethods = true)
    
    do paramType.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> createTypes (unbox args.[0]) typeName)

    do this.AddNamespace(ns, [paramType])
                            
[<assembly:TypeProviderAssembly>] 
do()

