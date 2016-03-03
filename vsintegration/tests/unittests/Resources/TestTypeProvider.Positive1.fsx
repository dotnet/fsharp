// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace HelloWorldTypeProvider

#r "System.Core.dll"
#load @"extras\extenders\shared\TypeBuilder.fs"

open System
open System.Linq.Expressions
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open FSharp.TypeMagic  // utilities for building types

#nowarn "40" // This just suppresses an informational warning that some "recursive knot tying" is used to defined synthetic types and their members. 

module internal SyntheticTypes =

    // This is a non-generative provider - the types are reported as belonging to the provider's assembly
    let thisAssembly = System.Reflection.Assembly.GetExecutingAssembly()

    // This is the namespace
    let rootNamespace = "HelloWorld" 

    // This defines all the members in the one synthetic type. A lazy computation is used to ensure the
    // member array is only created once.
    let rec allHelloWorldTypeMembers : Lazy<MemberInfo[]> = 
        lazy 
            [|  
                for (propertyName, propertyType) in  [("StaticProperty1", typeof<string>); ("StaticProperty2", typeof<int>) ] do 
                    let prop = TypeBuilder.CreateSyntheticProperty(helloWorldType,propertyName,propertyType,isStatic=true) 
                    yield! TypeBuilder.JoinPropertiesIntoMemberInfos [prop]
            |]  

    // This defines the one synthetic type. Put the type in the namespace, and specify its members.
    // Because this is a synthetic type it will be erased at runtime. It is erased to 'System.Object'
    // because that is its first non-synthetic base type.
    and helloWorldType =
        let container = TypeContainer.Namespace(thisAssembly.GetModules().[0], rootNamespace)
        TypeBuilder.CreateSimpleType(container,"HelloWorldType",members=allHelloWorldTypeMembers)


 
/// The implementation of the compiler extension. The attribute indicates that
/// is is a non-generative compiler extension.
[<TypeProvider>]
type public HelloWorldProvider() = 

    // This event is never triggered in this sample, because the schema never changes
    let invalidation = new Event<EventHandler,_>()

    // This implements both get_StaticProperty1 and get_StaticProperty2
    static member GetPropertyByName(propertyName:string) : 'T = 
        match propertyName with 
        | "StaticProperty1" -> "You got a static property" |> box |> unbox
        | "StaticProperty2" -> 42 |> box |> unbox
        | _ -> failwith "unexpected property"

    interface IProvidedNamespace with
        member this.GetNestedNamespaces() = [| |]
        member __.NamespaceName = SyntheticTypes.rootNamespace
        member __.GetTypes() = [SyntheticTypes.helloWorldType] |> Array.ofList
        member __.ResolveTypeName name : System.Type = 
            match name with
            | "HelloWorldType" -> SyntheticTypes.helloWorldType
            | _ -> failwith (sprintf "GetType %s" name)

    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
        member this.GetNamespaces() = [| this |]

        member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) =
            if staticArguments.Length <> 0 then failwith "this provided type does not accept static parameters" 
            typeWithoutArguments

        member __.GetStaticParameters(typeWithoutArguments) = [| |]
        member __.GetInvokerExpression(syntheticMethodBase:MethodBase, parameterExpressions:System.Linq.Expressions.ParameterExpression []) = 
            // trim off the "get_"
            let propertyName = syntheticMethodBase.Name.Substring(4)
            if syntheticMethodBase.DeclaringType = SyntheticTypes.helloWorldType then
                let syntheticMethodBase = syntheticMethodBase :?> MethodInfo
                let getClassInstancesByName = 
                    typeof<HelloWorldProvider>
                        .GetMethod("GetPropertyByName", BindingFlags.Static ||| BindingFlags.Public)
                        .MakeGenericMethod([|syntheticMethodBase.ReturnType|])
                upcast Expression.Call(getClassInstancesByName, Expression.Constant(propertyName))
            else 
                failwith "unexpected type"

        // This event is never triggered in this sample, because the schema never changes
        [<CLIEvent>]
        member x.Invalidate = invalidation.Publish

        member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents - only erased types were provided!!"

[<assembly:TypeProviderAssembly>]
do()


