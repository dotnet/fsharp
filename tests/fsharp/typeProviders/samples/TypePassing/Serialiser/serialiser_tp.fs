namespace Serialiser

open System
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Reflection
open ProviderImplementation.ProvidedTypes


[<TypeProvider>]
type Serialiser(config: TypeProviderConfig) as this =     
    inherit TypeProviderForNamespaces()

    let ns = "Serialiser"
    let runtimeAssembly = Assembly.LoadFrom(config.RuntimeAssembly)

    let createTypes (ty:Type) typeName = 
        let rootType = ProvidedTypeDefinition(runtimeAssembly,ns,typeName,baseType= (Some typeof<obj>), HideObjectMethods=true)
        let fields = ty.GetFields() //Because IsRecord is false here due to missing attrs in TAST, we cant use FSharp Reflection atm.
        let typName = ty.Name
        let fieldGetters = 
            (fun x -> Expr.NewArray(typeof<obj>, fields |> Array.map(fun f -> Expr.Coerce(Expr.FieldGet(x,f),typeof<obj>)) |> Array.toList))

        

        let headers = 
            fields |> Array.map (fun x -> x.Name)


        rootType.AddMember(ProvidedProperty("Headers", typeof<string[]>, IsStatic = true, GetterCode = (fun x -> <@@ headers @@>)))

        rootType.AddMember(
             ProvidedMethod("Serialise", 
                            [ProvidedParameter("obj", ty)], 
                            typeof<obj[]>, 
                            IsStaticMethod = true, 
                            InvokeCode = (fun x -> fieldGetters (Expr.Coerce(x.[0], ty)) )
                           )
                          )

        rootType

    let paramType = ProvidedTypeDefinition(runtimeAssembly, ns, "SourceType", Some(typeof<obj>), HideObjectMethods = true)
    
    do paramType.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> createTypes (unbox args.[0]) typeName)

    do this.AddNamespace(ns, [paramType])
                            
[<assembly:TypeProviderAssembly>] 
do()

