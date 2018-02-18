namespace Provider
#load @"..\helloWorld\TypeMagic.fs"
open Microsoft.FSharp.Core.CompilerServices
open System.Collections.Generic
open System.IO
open System
open System.Reflection
open System.Linq.Expressions
open FSharp.TypeMagic

[<TypeProvider>]
type public Provider(config : TypeProviderConfig) =
    let runtimeAssembly = Assembly.ReflectionOnlyLoadFrom(config.RuntimeAssembly)
    let modul = runtimeAssembly.GetModules().[0]

    let ``My.Runtime`` = runtimeAssembly.GetType("My.Runtime")
    let rootNamespace = "FSharp.SplitAssembly"
    let invalidation = new Event<System.EventHandler,_>()

    let theType = 
        let rec members =
            lazy
                [|  let p = TypeBuilder.CreateSyntheticProperty(theType,"Foo",typeof<int>,isStatic=true) 
                    yield! TypeBuilder.JoinPropertiesIntoMemberInfos [p]
                |]
        and theType =                
            TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul,rootNamespace),"TheType", members = members)
        theType

    interface IProvidedNamespace with
        member this.NamespaceName = rootNamespace
        member this.GetNestedNamespaces() = [||]
        member this.ResolveTypeName typeName =
            match typeName with
            |   "TheType" -> theType
            |   _ -> null
        member this.GetTypes() = [| theType |] 

    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
        member this.ApplyStaticArguments (st,_,_) = st
        member this.GetInvokerExpression(mb,p) = 
            let mi = ``My.Runtime``.GetMethod("Id").MakeGenericMethod([|typeof<int>|])
            Quotations.Expr.Call(mi, [ Quotations.Expr.Value(42) ])
        member this.GetNamespaces() = [| this |]
        member this.GetStaticParameters st = [||]
        [<CLIEvent>]
        member this.Invalidate = invalidation.Publish
        member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents - only erased types were provided!!"

