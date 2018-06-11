#r "../../../../../../Debug/net40/bin/type_passing_tp.dll"

open System
open FSharp.Reflection
open System.Reflection
open System.Collections.Generic
open Test

[<AutoOpen>]
module TypeProperties =

    type PropertyInfo<'a> = (string * ('a -> string)) seq
    type Properties = Properties of (string * string) seq * (string * Properties) seq


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
        props |> Seq.map (fun (a, g) -> try a, g value with | e -> a, sprintf "NOT SUPPORTED: %A" e)

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

let mutable failures = []

let betterCheck prefix testName expected actualGetter =
    let check nm expected actual =
        if expected = actual then ()
        else
            failures <- failures @ [nm]
            printfn "%s, expected %A, got %A\n" nm expected actual

    let name = prefix + ": " + testName
    try
        let actual = actualGetter()
        check name actual (string expected)
    with
    | e -> ()

let compare (expected : Type) (actual : Map<string, string>) =
    let prefix = expected.Name
    printfn ""
    printfn "=============================================="
    printfn "STARTING TESTING OF TYPE: %s" prefix
    let check = betterCheck prefix
    let expectedMap = flattenTree (typeTree expected) |> Map.ofSeq

    Map.iter (fun k v -> check k v (fun () -> actual.[k])) expectedMap

    let missingItems = Map.fold (fun s k _ -> Map.remove k s) expectedMap actual
    missingItems |> Map.iter (printfn "Missing item: %s, should have value: %s")

    printfn "TESTING COMPLETE OF TYPE: %s" prefix
    printfn "=============================================="
    printfn ""

[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Field)>]
type MyTpAttribute(name:string) =
     inherit Attribute()

     member x.Name with get() = name

module MyModule =

    type TypeInModule =
        { x : int }

type [<MyTpAttribute("Foo")>] AttributedRecord =
    { Id : string }
    member x.TestInstanceProperty = 1
    member x.TestInstanceMethod(y:string) = y
    static member TestStaticMethod(y:string) = y
    static member TestStaticProperty = 2

type MyRecord =
    { Id: string }
    member x.TestInstanceProperty = 1
    member x.TestInstanceMethod(y:string) = y
    static member TestStaticMethod(y:string) = y
    static member TestStaticProperty = 2

type internal MyInternalRecord =
    { Id: string }
    member x.TestInstanceProperty = 1
    member x.TestInstanceMethod(y:string) = y
    static member TestStaticMethod(y:string) = y
    static member TestStaticProperty = 2

type MyUnion =
    | A of int
    | B of string
    member x.TestInstanceProperty = 1
    member x.TestInstanceMethod(y:string) = y
    static member TestStaticMethod(y:string) = y
    static member TestStaticProperty = 2


type MyGenericRecord<'a, 'b> =
    { Id: 'a; X : 'b }
    member x.TestInstanceProperty = 1
    member x.TestInstanceMethod(y:string) = y
    static member TestStaticMethod(y:string) = y
    static member TestStaticProperty = 2

module AttributedRecordTest =
    let actual = TypePassing.Metadata.Create<AttributedRecord>()
    let expected = typeof<AttributedRecord>
    compare expected actual

module MyRecordTest =
    let actual = TypePassing.Metadata.Create<MyRecord>()
    let expected = typeof<MyRecord>
    compare expected actual

module MyInternalRecordTest =
    let actual = TypePassing.Metadata.Create<MyInternalRecord>()
    let expected = typeof<MyInternalRecord>
    compare expected actual

module TypeInModuleTest =
    let actual = TypePassing.Metadata.Create<MyModule.TypeInModule>()
    let expected = typeof<MyModule.TypeInModule>
    compare expected actual

module MyUnionTest =
    let actual = TypePassing.Metadata.Create<MyUnion>()
    let expected = typeof<MyUnion>
    compare expected actual

module MyGenericRecordTest =
    let actual = TypePassing.Metadata.Create<MyGenericRecord<String, int>>()
    let expected = typeof<MyGenericRecord<String, int>>
    compare expected actual

module IntTest =
    let actual = TypePassing.Metadata.Create<int>()
    let expected = typeof<int>
    compare expected actual

module IComparableTest =
    let actual = TypePassing.Metadata.Create<System.IComparable>()
    let expected = typeof<System.IComparable>
    compare expected actual

module OptionalIntTest =
    let actual = TypePassing.Metadata.Create<int option>()
    let expected = typeof<int option>
    compare expected actual
