// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace HelloWorldTypeProviderNeg1

#r "System.Core.dll"
#load @"extras\extenders\shared\TypeBuilder.fs"

open System
open Microsoft.FSharp.Core.CompilerServices

[<TypeProvider>]
type public HelloWorldProvider() = 
    let invalidation = new Event<EventHandler,_>()

    interface IProvidedNamespace with
        member __.NamespaceName = "HelloWorld"
        member __.GetTypes() = [| |]
        member __.ResolveTypeName name =  failwith (sprintf "GetType %s" name)
        member __.GetNestedNamespaces() = [| |]
    interface IDisposable with
        member __.Dispose() = ()

    interface ITypeProvider with
        member this.GetNamespaces() = [| this |] 
        member __.GetInvokerExpression(syntheticMethodBase, parameterExpressions) = failwith "nyi"
        member __.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) =
            if staticArguments.Length <> 0 then failwith "this provided type does not accept static parameters" 
            typeWithoutArguments
        member __.GetStaticParameters(typeWithoutArguments) = [| |]

        [<CLIEvent>]
        member x.Invalidate = invalidation.Publish
        member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents - only erased types were provided!!"


[<assembly:TypeProviderAssembly("ThisAssemblyDoesNotExist")>]
do()


