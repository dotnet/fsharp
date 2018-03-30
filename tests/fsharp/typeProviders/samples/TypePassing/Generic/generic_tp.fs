namespace Test

open System
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open System.Collections
open System.Collections.Generic
open Microsoft.FSharp.Quotations

[<TypeProvider>]
type TypePassingTp(config: TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces(config)

    let ns = "Generic"
    let runtimeAssembly = Assembly.LoadFrom(config.RuntimeAssembly)

    let baseType =
        let rootType = ProvidedTypeDefinition(runtimeAssembly, ns, "Identity", baseType = Some typeof<obj>, hideObjectMethods=true)
        rootType

    let createMethod (t : Type) (name : string) : ProvidedMethod =
        let invoke (xs : Quotations.Expr list) =
            xs.[0]
        let m = ProvidedMethod(name, [ProvidedParameter("x", t)], t, invoke, isStatic = true)
        baseType.AddMember(m)
        m

    let builder = ProvidedMethod("Create", [], baseType, Unchecked.defaultof<_>, isStatic = true)
    do builder.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> createMethod (unbox args.[0]) typeName)

    do baseType.AddMember(builder)

    do this.AddNamespace(ns, [baseType])

[<assembly:TypeProviderAssembly>]
do()

