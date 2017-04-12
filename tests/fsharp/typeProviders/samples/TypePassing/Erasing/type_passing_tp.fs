namespace Test

open System
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open ProviderImplementation.ProvidedTypes


[<TypeProvider>]
type TypePassingTp(config: TypeProviderConfig) as this =     
    inherit TypeProviderForNamespaces()

    let ns = "TypePassing"
    let runtimeAssembly = Assembly.LoadFrom(config.RuntimeAssembly)

    let createTypes (ty:Type) typeName = 
        let rootType = ProvidedTypeDefinition(runtimeAssembly,ns,typeName,baseType= (Some typeof<obj>), HideObjectMethods=true)
        rootType.AddMember(ProvidedProperty(ty.Name, typeof<obj>, GetterCode = fun args -> <@@ obj()  @@> ))        
        rootType

    let paramType = ProvidedTypeDefinition(runtimeAssembly, ns, "TypePassingTP", Some(typeof<obj>), HideObjectMethods = true)
    
    do paramType.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> createTypes (unbox args.[0]) typeName)

    do this.AddNamespace(ns, [paramType])
                            
[<assembly:TypeProviderAssembly>] 
do()

