#r "../../../../../../Debug/net40/bin/type_passing_tp.dll"

open System
open FSharp.Reflection
open Test

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

let mutable failures = []

let check nm v1 v2 =
    if v1 = v2 then ()
    else
        failures <- failures @ [nm]
        printfn "%s, expected %A, got %A\n" nm v2 v1

let inaccurate nm v1 v2 v3 =
    if v1 = v2 then printfn "%s: PASSED (was failing, now passing)" nm
    elif v1 = v3 then printfn "%s: PASSED (inaccurate), expected %A, allowing %A" nm v2 v1
    else
        failures <- failures @ [nm]
        printfn "\n*** %s: FAILED, expected %A, got %A, would have accepted %A\n" nm v2 v1 v3

let betterCheck prefix testName expected actualGetter =
    let name = prefix + ": " + testName
    try
        let actual = actualGetter()
        check name actual (string expected)
    with
    | e -> printfn "\n*** %s: THREW EXCEPTION, Message = %s\n" name e.Message

type TypeMetadata = TypePassing.Metadata

let compare (expected : Type) (actual : TypeMetadata) =
    let prefix = expected.Name
    let check x = betterCheck prefix x //'
    check "Name"
        (string <| expected.Name)
        (fun () -> actual.Name)
    check "FullName"
        (string <| expected.FullName)
        (fun () -> actual.FullName)
    check "AssemblyQualifiedName"
        (string <| expected.AssemblyQualifiedName)
        (fun () -> actual.AssemblyQualifiedName)
    check "IsAbstract"
        (string <| expected.IsAbstract)
        (fun () -> actual.IsAbstract)
    check "IsAnsiClass"
        (string <| expected.IsAnsiClass)
        (fun () -> actual.IsAnsiClass)
    check "IsArray"
        (string <| expected.IsArray)
        (fun () -> actual.IsArray)
    check "IsAutoClass"
        (string <| expected.IsAutoClass)
        (fun () -> actual.IsAutoClass)
    check "IsAutoLayout"
        (string <| expected.IsAutoLayout)
        (fun () -> actual.IsAutoLayout)
    check "IsByRef"
        (string <| expected.IsByRef)
        (fun () -> actual.IsByRef)
    check "IsClass"
        (string <| expected.IsClass)
        (fun () -> actual.IsClass)
    check "IsValueType"
        (string <| expected.IsValueType)
        (fun () -> actual.IsValueType)
    check "IsInterface"
        (string <| expected.IsInterface)
        (fun () -> actual.IsInterface)
    check "IsGenericParameter"
        (string <| expected.IsGenericParameter)
        (fun () -> actual.IsGenericParameter)
    check "IsNested"
        (string <| expected.IsNested)
        (fun () -> actual.IsNested)
    check "IsNestedPublic"
        (string <| expected.IsNestedPublic)
        (fun () -> actual.IsNestedPublic)
    check "IsPublic"
        (string <| expected.IsPublic)
        (fun () -> actual.IsPublic)
    check "IsNotPublic"
        (string <| expected.IsNotPublic)
        (fun () -> actual.IsNotPublic)
    check "IsSealed"
        (string <| expected.IsSealed)
        (fun () -> actual.IsSealed)
    check "IsGenericType"
        (string <| expected.IsGenericType)
        (fun () -> actual.IsGenericType)
    check "IsGenericTypeDefinition"
        (string <| expected.IsGenericTypeDefinition)
        (fun () -> actual.IsGenericTypeDefinition)
    check "IsRecord"
        (string <| Reflection.FSharpType.IsRecord(expected))
        (fun () -> actual.IsRecord)
    check "IsFunction"
        (string <| Reflection.FSharpType.IsFunction(expected))
        (fun () -> actual.IsFunction)
    check "IsModule"
        (string <| Reflection.FSharpType.IsModule(expected))
        (fun () -> actual.IsModule)
    check "IsExceptionRepresentation"
        (string <| Reflection.FSharpType.IsExceptionRepresentation(expected))
        (fun () -> actual.IsExceptionRepresentation)
    check "IsTuple"
        (string <| Reflection.FSharpType.IsTuple(expected))
        (fun () -> actual.IsTuple)
    check "IsUnion"
        (string <| Reflection.FSharpType.IsUnion(expected))
        (fun () -> actual.IsUnion)
    check "GetPublicProperties_Length"
        (string <| expected.GetProperties().Length)
        (fun () -> actual.GetPublicProperties_Length)
    check "GetPublicConstructors_Length"
        (string <| expected.GetConstructors().Length)
        (fun () -> actual.GetPublicConstructors_Length)
    check "GetPublicMethods_Length"
        (string <| expected.GetMethods().Length)
        (fun () -> actual.GetPublicMethods_Length)
    check "GetGenericArguments_Length"
        (string <| expected.GetGenericArguments().Length)
        (fun () -> actual.GetGenericArguments_Length)
    check "CustomAttribute_Names"
        (string <| (expected.CustomAttributes |> Seq.map (fun x -> x.AttributeType.Name) |> Seq.toArray))
        (fun () -> actual.CustomAttribute_Names)
    check "CustomAttributes_Length"
        (string <| Seq.length expected.CustomAttributes)
        (fun () -> actual.CustomAttributes_Length)
//    check "Assembly_CodeBase"
//        (string <| expected.Assembly.CodeBase)
//        (fun () -> actual.Assembly_CodeBase)
//    check "Assembly_CustomAttributes_Count"
//        (string <| Seq.length expected.Assembly.CustomAttributes)
//        (fun () -> actual.Assembly_CustomAttributes_Count)
//    check "Assembly_DefinedTypes_Count"
//        (string <| Seq.length expected.Assembly.DefinedTypes)
//        (fun () -> actual.Assembly_DefinedTypes_Count)
//    check "Assembly_FullName"
//        (string <| expected.Assembly.FullName)
//        (fun () -> actual.Assembly_FullName)
//    check "Assembly_EntryPoint_isNull"
//        (string <| isNull expected.Assembly.EntryPoint)
//        (fun () -> actual.Assembly_EntryPoint_isNull)

module AttributedRecordTest =
    let actual = TypePassing.Metadata.Create<AttributedRecord>()
    let expected = typeof<AttributedRecord>
    compare expected actual

module MyRecordTest =
    let actual = TypePassing.Metadata.Create<MyRecord>()
    let expected = typeof<MyRecord>
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
