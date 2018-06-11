namespace Test

open System
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open System.Collections
open System.Collections.Generic
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Reflection


[<TypeProvider>]
type TypePassingTp(config: TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces(config)

    let ns = "Generic"
    let runtimeAssembly = Assembly.LoadFrom(config.RuntimeAssembly)

    let overloaded = ProvidedTypeDefinition(runtimeAssembly, ns, "Overloaded", baseType = Some typeof<obj>, hideObjectMethods=true)
    let overload1 = ProvidedMethod("X", [ProvidedParameter("x", typeof<int>)], typeof<int>, (fun xs -> xs.[0]), isStatic = true)
    let overload2 = ProvidedMethod("X", [ProvidedParameter("x", typeof<int>)], typeof<string>, (fun xs -> <@@ string (%%(xs.[0]) : string) @@>), isStatic = true)
    do overloaded.AddMembers([overload1; overload2])

    // ====== Generic Method ========

    let genericMethodType = ProvidedTypeDefinition(runtimeAssembly, ns, "IdentityMethod", baseType = Some typeof<obj>, hideObjectMethods=true)

    let createMethod (t : Type) (name : string) : ProvidedMethod =
        let invoke (xs : Quotations.Expr list) =
            xs.[0]
        let m = ProvidedMethod(name, [ProvidedParameter("x", t)], t, invoke, isStatic = true)
        genericMethodType.AddMember(m)
        m

    let builder = ProvidedMethod("Create", [], genericMethodType, Unchecked.defaultof<_>, isStatic = true)
    do builder.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> createMethod (unbox args.[0]) typeName)

    do genericMethodType.AddMember(builder)

    // ====== Returning types ========

    let idType = ProvidedTypeDefinition(runtimeAssembly, ns, "IdentityType", baseType = Some typeof<obj>, hideObjectMethods=true)

    do idType.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> unbox args.[0])

    let constType = ProvidedTypeDefinition(runtimeAssembly, ns, "ConstType", baseType = Some typeof<obj>, hideObjectMethods=true)
    do constType.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type1",typeof<Type>, null)
            ProvidedStaticParameter("Type2",typeof<Type>, null)
        ], fun typeName args -> unbox args.[0])

    let ifThenElseType = ProvidedTypeDefinition(runtimeAssembly, ns, "IfThenElse", baseType = Some typeof<obj>, hideObjectMethods=true)
    do ifThenElseType.DefineStaticParameters(
        [
            ProvidedStaticParameter("Cond",typeof<bool>, null)
            ProvidedStaticParameter("A",typeof<Type>, null)
            ProvidedStaticParameter("B",typeof<Type>, null)
        ], fun typeName args -> if unbox args.[0] then unbox args.[1] else unbox args.[2])

    // ====== Generic Types ========

    let idFunc = ProvidedTypeDefinition(runtimeAssembly, ns, "IdentityFunction", baseType = Some typeof<obj>, hideObjectMethods=true)

    let createIdType (t : Type) (name : string) : ProvidedTypeDefinition =
        let invoke (xs : Quotations.Expr list) =
            xs.[0]
        let newType = ProvidedTypeDefinition(runtimeAssembly, ns, name, baseType = Some typeof<obj>, hideObjectMethods = true)
        let m = ProvidedMethod("Invoke", [ProvidedParameter("x", t)], t, invoke, isStatic = true)
        newType.AddMember(m)
        newType

    do idFunc.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> createIdType (unbox args.[0]) typeName :> Type)

    let constFunc = ProvidedTypeDefinition(runtimeAssembly, ns, "ConstFunction", baseType = Some typeof<obj>, hideObjectMethods=true)

    let createConstType (t1 : Type) (t2 : Type) (name : string) : ProvidedTypeDefinition =
        let invoke (xs : Quotations.Expr list) =
            xs.[0]
        let newType = ProvidedTypeDefinition(runtimeAssembly, ns, name, baseType = Some typeof<obj>, hideObjectMethods = true)
        let m = ProvidedMethod("Invoke", [ProvidedParameter("x", t1); ProvidedParameter("y", t2)], t1, invoke, isStatic = true)
        newType.AddMember(m)
        newType

    do constFunc.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type1",typeof<Type>, null)
            ProvidedStaticParameter("Type2",typeof<Type>, null)
        ], fun typeName args -> createConstType (unbox args.[0]) (unbox args.[1]) typeName :> Type)

    // ===========================

    do this.AddNamespace(ns, [genericMethodType; idFunc; constFunc; idType; constType; ifThenElseType; overloaded])

[<assembly:TypeProviderAssembly>]
do()

