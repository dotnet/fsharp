namespace Test

open System
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open System.Collections
open System.Collections.Generic
open Microsoft.FSharp.Quotations

type GD = Dictionary<string, string>

[<TypeProvider>]
type TypePassingTp(config: TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces(config)

    let ns = "TypePassing"
    let runtimeAssembly = Assembly.LoadFrom(config.RuntimeAssembly)

    let properties =
        let box x = (box x).ToString()
        Map.ofList
            [
                "Name",                             fun (ty : Type) -> box <| ty.Name
                "FullName",                         fun (ty : Type) -> box <| ty.FullName
                "AssemblyQualifiedName",            fun (ty : Type) -> box <| ty.AssemblyQualifiedName
                "IsAbstract",                       fun (ty : Type) -> box <| ty.IsAbstract
                "IsAnsiClass",                      fun (ty : Type) -> box <| ty.IsAnsiClass
                "IsArray",                          fun (ty : Type) -> box <| ty.IsArray
                "IsAutoClass",                      fun (ty : Type) -> box <| ty.IsAutoClass
                "IsAutoLayout",                     fun (ty : Type) -> box <| ty.IsAutoLayout
                "IsByRef",                          fun (ty : Type) -> box <| ty.IsByRef
                "IsClass",                          fun (ty : Type) -> box <| ty.IsClass
                "IsValueType",                      fun (ty : Type) -> box <| ty.IsValueType
                "IsInterface",                      fun (ty : Type) -> box <| ty.IsInterface
                "IsGenericParameter",               fun (ty : Type) -> box <| ty.IsGenericParameter
                "IsNested",                         fun (ty : Type) -> box <| ty.IsNested
                "IsNestedPublic",                   fun (ty : Type) -> box <| ty.IsNestedPublic
                "IsPublic",                         fun (ty : Type) -> box <| ty.IsPublic
                "IsNotPublic",                      fun (ty : Type) -> box <| ty.IsNotPublic
                "IsSealed",                         fun (ty : Type) -> box <| ty.IsSealed
                "IsGenericType",                    fun (ty : Type) -> box <| ty.IsGenericType
                "IsGenericTypeDefinition",          fun (ty : Type) -> box <| ty.IsGenericTypeDefinition
                "IsRecord",                         fun (ty : Type) -> box <| Reflection.FSharpType.IsRecord(ty)
                "IsFunction",                       fun (ty : Type) -> box <| Reflection.FSharpType.IsFunction(ty)
                "IsModule",                         fun (ty : Type) -> box <| Reflection.FSharpType.IsModule(ty)
                "IsExceptionRepresentation",        fun (ty : Type) -> box <| Reflection.FSharpType.IsExceptionRepresentation(ty)
                "IsTuple",                          fun (ty : Type) -> box <| Reflection.FSharpType.IsTuple(ty)
                "IsUnion",                          fun (ty : Type) -> box <| Reflection.FSharpType.IsUnion(ty)
                "GetPublicProperties_Length",       fun (ty : Type) -> box <| ty.GetProperties().Length
                "GetPublicConstructors_Length",     fun (ty : Type) -> box <| ty.GetConstructors().Length
                "GetPublicMethods_Length",          fun (ty : Type) -> box <| ty.GetMethods().Length
                "GetGenericArguments_Length",       fun (ty : Type) -> box <| ty.GetGenericArguments().Length
                "CustomAttribute_Names",            fun (ty : Type) -> box <| (ty.CustomAttributes |> Seq.map (fun x -> x.AttributeType.Name) |> Seq.toArray)
                "CustomAttributes_Length",          fun (ty : Type) -> box <| Seq.length ty.CustomAttributes
                "Assembly_CodeBase",                fun (ty : Type) -> box <| ty.Assembly.CodeBase
                "Assembly_CustomAttributes_Count",  fun (ty : Type) -> box <| Seq.length ty.Assembly.CustomAttributes
                "Assembly_DefinedTypes_Count",      fun (ty : Type) -> box <| Seq.length ty.Assembly.DefinedTypes
                "Assembly_FullName",                fun (ty : Type) -> box <| ty.Assembly.FullName
                "Assembly_EntryPoint_isNull",       fun (ty : Type) -> box <| isNull ty.Assembly.EntryPoint
//                "GUID",                             fun (ty : Type) -> box <| ty.GUID.ToString()
           ]

    let metadataType =
        let rootType = ProvidedTypeDefinition(runtimeAssembly, ns ,"Metadata", baseType = Some typeof<GD>, hideObjectMethods=true)
        for (name, _) in Map.toSeq properties do
            let prop = ProvidedProperty(name, typeof<string>, getterCode = (fun ms -> let m = ms.[0] in <@@ (%%m : GD).[name] @@>), isStatic=false)
            rootType.AddMember(prop)
        rootType

    let createMethod (t : Type) (name : string) : ProvidedMethod =
        let invoke (_ : Quotations.Expr list) =
            let object = properties |> Map.map (fun a g -> try g t with | _ -> "NOT SUPPORTED")
            let dictionaryVar = Var("dictionary", typeof<GD>)
            let dictionary : Expr<GD> = dictionaryVar |> Expr.Var |> Expr.Cast
            let setValues =
              object
              |> Map.toSeq
              |> Seq.fold (fun state (name, arg) ->
                <@ (%dictionary).[name] <- arg
                   %state @>) <@ %dictionary @>
            Expr.Let(dictionaryVar, <@ GD() @>, setValues)
        let method = ProvidedMethod(name, [ProvidedParameter("unit", typeof<unit>)], metadataType, invoke, isStatic = true)
        metadataType.AddMember(method)
        method

    let builder = ProvidedMethod("Create", [], metadataType, Unchecked.defaultof<_>, isStatic = true)
    do builder.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> createMethod (unbox args.[0]) typeName)

    do metadataType.AddMember(builder)

    do this.AddNamespace(ns, [metadataType])

[<assembly:TypeProviderAssembly>]
do()

