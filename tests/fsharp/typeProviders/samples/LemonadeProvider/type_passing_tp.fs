namespace Test

open System
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open System.Collections
open System.Collections.Generic
open Microsoft.FSharp.Quotations

type GD = Map<string, string>

type PropertyInfo<'a> = (string * ('a -> string)) seq
type Properties = Properties of (string * string) seq * (string * Properties) seq

[<TypeProvider>]
type TypePassingTp(config: TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces(config)

    let ns = "TypePassing"
    let runtimeAssembly = Assembly.LoadFrom(config.RuntimeAssembly)

    let box x = (box x).ToString()

    let attributeProperties : PropertyInfo<CustomAttributeData> =
        seq [
            "AttributeType",  fun (attr : CustomAttributeData) -> box <| attr.AttributeType
            "NamedArguments", fun (attr : CustomAttributeData) -> box <| attr.NamedArguments
        ]

    let propertyProperties : PropertyInfo<PropertyInfo> =
        seq [
            "Name",                fun (prop : PropertyInfo) -> box <| prop.Name
            "CanRead",             fun (prop : PropertyInfo) -> box <| prop.CanRead
            "CanWrite",            fun (prop : PropertyInfo) -> box <| prop.CanWrite
            "IsSpecialName",       fun (prop : PropertyInfo) -> box <| prop.IsSpecialName
            "DeclaringType",       fun (prop : PropertyInfo) -> box <| prop.DeclaringType
            "MemberType",          fun (prop : PropertyInfo) -> box <| prop.MemberType
            "PropertyType",        fun (prop : PropertyInfo) -> box <| prop.PropertyType
            "Attributes",          fun (prop : PropertyInfo) -> box <| prop.Attributes
        ]

    let typeProperties : PropertyInfo<Type> =
        seq [
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
        ]

    let makeMetadata props value =
        props |> Seq.map (fun (a, g) -> try a, g value with | _ -> a, sprintf "NOT SUPPORTED")

    let attributeTree (a : CustomAttributeData) =
        Properties(makeMetadata attributeProperties a, [])

    let propertyTree (p : PropertyInfo) =
        let nested =
            seq {
                yield! p.CustomAttributes |> Seq.map (fun attr -> attr.AttributeType.Name, attributeTree attr)
            }
        Properties(makeMetadata propertyProperties p, nested)

    let typeTree (t : Type) =
        let nested =
            seq {
                yield! t.CustomAttributes |> Seq.map (fun attr -> attr.AttributeType.Name, attributeTree attr)
                yield! t.GetProperties() |> Seq.map (fun prop -> prop.Name, propertyTree prop)
            }
        Properties(makeMetadata typeProperties t, nested)

    let flattenTree tree =
        let rec go (Properties(p, children)) =
            let children =
                children
                |> Seq.map (fun (k,v) -> k, go v)
                |> Seq.map (fun (k,v) -> Seq.map (fun (k', v') -> k + "." + k', v') v)
                |> Seq.concat
            Seq.append p children
        go tree

    let metadataType = ProvidedTypeDefinition(runtimeAssembly, ns ,"Metadata", baseType = Some typeof<GD>, hideObjectMethods=true)

    let createMethod t : string -> ProvidedMethod =
        let invoke (_ : Quotations.Expr list) : Expr =
            let object = flattenTree (typeTree t)
            let dictionaryVar = Var("dictionary", typeof<GD>)
            let dictionary : Expr<GD> = dictionaryVar |> Expr.Var |> Expr.Cast
            Seq.fold
                (fun state (name, arg) -> <@ Map.add name arg %state @>)
                <@ Map.empty @>
                object
            |> fun x -> x :> _

        fun name -> 
            let method = ProvidedMethod(name, [ProvidedParameter("unit", typeof<unit>)], typeof<GD>, invoke, isStatic = true)
            metadataType.AddMember(method)
            method

    let builder = ProvidedMethod("Create", [], typeof<GD>, Unchecked.defaultof<_>, isStatic = true)
    do builder.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> createMethod (unbox args.[0]) typeName)

    do metadataType.AddMember(builder)

    do this.AddNamespace(ns, [metadataType])

[<assembly:TypeProviderAssembly>]
do()

