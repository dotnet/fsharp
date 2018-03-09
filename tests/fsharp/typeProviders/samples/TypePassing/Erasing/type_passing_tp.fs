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
                "Name",                             (typeof<string>,     fun (ty : Type) -> box <| ty.Name)
                "FullName",                         (typeof<string>,     fun (ty : Type) -> box <| ty.FullName)
                "AssemblyQualifiedName",            (typeof<string>,     fun (ty : Type) -> box <| ty.AssemblyQualifiedName)
                "IsAbstract",                       (typeof<bool>,       fun (ty : Type) -> box <| ty.IsAbstract)
                "IsAnsiClass",                      (typeof<bool>,       fun (ty : Type) -> box <| ty.IsAnsiClass)
                "IsArray",                          (typeof<bool>,       fun (ty : Type) -> box <| ty.IsArray)
                "IsAutoClass",                      (typeof<bool>,       fun (ty : Type) -> box <| ty.IsAutoClass)
                "IsAutoLayout",                     (typeof<bool>,       fun (ty : Type) -> box <| ty.IsAutoLayout)
                "IsByRef",                          (typeof<bool>,       fun (ty : Type) -> box <| ty.IsByRef)
                "IsClass",                          (typeof<bool>,       fun (ty : Type) -> box <| ty.IsClass)
                "IsValueType",                      (typeof<bool>,       fun (ty : Type) -> box <| ty.IsValueType)
                "IsInterface",                      (typeof<bool>,       fun (ty : Type) -> box <| ty.IsInterface)
                "IsGenericParameter",               (typeof<bool>,       fun (ty : Type) -> box <| ty.IsGenericParameter)
                "IsNested",                         (typeof<bool>,       fun (ty : Type) -> box <| ty.IsNested)
                "IsNestedPublic",                   (typeof<bool>,       fun (ty : Type) -> box <| ty.IsNestedPublic)
                "IsPublic",                         (typeof<bool>,       fun (ty : Type) -> box <| ty.IsPublic)
                "IsNotPublic",                      (typeof<bool>,       fun (ty : Type) -> box <| ty.IsNotPublic)
                "IsSealed",                         (typeof<bool>,       fun (ty : Type) -> box <| ty.IsSealed)
                "IsGenericType",                    (typeof<bool>,       fun (ty : Type) -> box <| ty.IsGenericType)
                "IsGenericTypeDefinition",          (typeof<bool>,       fun (ty : Type) -> box <| ty.IsGenericTypeDefinition)
                "IsRecord",                         (typeof<bool>,       fun (ty : Type) -> box <| Reflection.FSharpType.IsRecord(ty))
                "IsFunction",                       (typeof<bool>,       fun (ty : Type) -> box <| Reflection.FSharpType.IsFunction(ty))
                "IsModule",                         (typeof<bool>,       fun (ty : Type) -> box <| Reflection.FSharpType.IsModule(ty))
                "IsExceptionRepresentation",        (typeof<bool>,       fun (ty : Type) -> box <| Reflection.FSharpType.IsExceptionRepresentation(ty))
                "IsTuple",                          (typeof<bool>,       fun (ty : Type) -> box <| Reflection.FSharpType.IsTuple(ty))
                "IsUnion",                          (typeof<bool>,       fun (ty : Type) -> box <| Reflection.FSharpType.IsUnion(ty))
                "GetPublicProperties_Length",       (typeof<int>,        fun (ty : Type) -> box <| ty.GetProperties().Length)
                "GetPublicConstructors_Length",     (typeof<int>,        fun (ty : Type) -> box <| ty.GetConstructors().Length)
                "GetPublicMethods_Length",          (typeof<int>,        fun (ty : Type) -> box <| ty.GetMethods().Length)
                "GetGenericArguments_Length",       (typeof<int>,        fun (ty : Type) -> box <| ty.GetGenericArguments().Length)
                "CustomAttribute_Names",            (typeof<string[]>,   fun (ty : Type) -> box <| (ty.CustomAttributes |> Seq.map (fun x -> x.AttributeType.Name) |> Seq.toArray))
                "CustomAttributes_Length",          (typeof<int>,        fun (ty : Type) -> box <| Seq.length ty.CustomAttributes)
                "Assembly_CodeBase",                (typeof<string>,     fun (ty : Type) -> box <| ty.Assembly.CodeBase)
                "Assembly_CustomAttributes_Count",  (typeof<int>,        fun (ty : Type) -> box <| Seq.length ty.Assembly.CustomAttributes)
                "Assembly_DefinedTypes_Count",      (typeof<int>,        fun (ty : Type) -> box <| Seq.length ty.Assembly.DefinedTypes)
                "Assembly_FullName",                (typeof<string>,     fun (ty : Type) -> box <| ty.Assembly.FullName)
                "Assembly_EntryPoint_isNull",       (typeof<bool>,       fun (ty : Type) -> box <| isNull ty.Assembly.EntryPoint)
//                "GUID",                             (typeof<string>,     fun (ty : Type) -> box <| ty.GUID.ToString())
           ]

    let (|UnKey|) (kvp : Generic.KeyValuePair<'a, 'b>) =
        kvp.Key, kvp.Value

    let metadataType =
        let rootType = ProvidedTypeDefinition(runtimeAssembly, ns ,"Metadata", baseType = Some typeof<GD>, hideObjectMethods=true)
        for UnKey (name, (propTy, _)) in properties do
            let prop = ProvidedProperty(name, typeof<string>, getterCode = (fun ms -> let m = ms.[0] in <@@ (%%m : GD).[name] @@>), isStatic=false)
            rootType.AddMember(prop)
        rootType

    let createMethod (t : Type) (name : string) : ProvidedMethod =
        let invoke (_ : Quotations.Expr list) =
            let object = properties |> Map.map (fun a (t', g) -> try g t with | _ -> "NOT SUPPORTED")
            let dictionaryVar = Var("dictionary", typeof<GD>)
            let dictionary : Expr<GD> = dictionaryVar |> Expr.Var |> Expr.Cast
            let setValues =
              object
              |> Seq.fold (fun state (UnKey(name, arg)) ->
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

