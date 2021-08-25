#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module FSharp.Compiler.Service.Tests.ExtensionTypingProvider
#endif

open System
open System.Collections.Generic
open System.IO
open FsUnit
open NUnit.Framework
open FSharp.Compiler.ExtensionTyping
open FSharp.Compiler.Service.Tests.Common
open Microsoft.FSharp.Core.CompilerServices

let fileName, options =
    mkTestFileAndOptions ""
     [| @"-r:" + Path.Combine(__SOURCE_DIRECTORY__, Path.Combine("data", "FSharp.Data.dll"))
        @"-r:" + sysLib "System.Xml.Linq" |]

type ProxyProvidedNamespace(pn: IProvidedNamespace, typesCache: Dictionary<_, _>) =
    interface IProvidedNamespace with
        member this.GetNestedNamespaces() = [| |]
        member this.GetTypes() =
            let types = pn.GetTypes()
            for t in types do typesCache.Add(t.Name, t)
            types      
        member this.ResolveTypeName(name) =
            if typesCache.ContainsKey(name) then typesCache.[name] else null
        member this.NamespaceName = pn.NamespaceName

type ProxyTypeProvider(tp: ITypeProvider, typesCache: Dictionary<_,_>) = 
    interface ITypeProvider with
        member this.GetStaticParameters t = tp.GetStaticParameters t
        member this.ApplyStaticArguments(a,b,c) = tp.ApplyStaticArguments(a, b, c)
        member this.GetNamespaces() = tp.GetNamespaces() |> Array.map(fun pn -> ProxyProvidedNamespace(pn, typesCache) :> _)
        member this.GetInvokerExpression(methodBase, args) = tp.GetInvokerExpression(methodBase, args)
        
        [<CLIEvent>]
        member this.Invalidate = tp.Invalidate
        member this.GetGeneratedAssemblyContents(a) = tp.GetGeneratedAssemblyContents(a)
        
    interface ITypeProvider2 with
        member this.GetStaticParametersForMethod t = (tp :?> ITypeProvider2).GetStaticParametersForMethod t
        member this.ApplyStaticArgumentsForMethod(a,b,c) = (tp :?> ITypeProvider2).ApplyStaticArgumentsForMethod(a,b,c)         

    interface IDisposable with
        member this.Dispose() = tp.Dispose()

[<Test>]
#if NETCOREAPP
[<Ignore "SKIPPED: Disabled until FSharp.Data.dll is build for dotnet core.">]
#endif
let ``Extension typing proxy shim gets requests`` () =
    let mutable gotInstantiateTypeProvidersOfAssemblyRequest = false
    let mutable gotGetProvidedTypesRequest = false
    let mutable gotResolveTypeNameRequest = false
    let mutable gotGetInvokerExpressionRequest = false
    let mutable gotDisplayNameOfTypeProviderRequest = false
    
    let defaultExtensionTypingShim = ExtensionTypingProvider
    
    let extensionTypingProvider =
        { new IExtensionTypingProvider with
            member this.InstantiateTypeProvidersOfAssembly(context) =
                gotInstantiateTypeProvidersOfAssemblyRequest <- true
                defaultExtensionTypingShim.InstantiateTypeProvidersOfAssembly(context)
                |> List.map(fun tp -> new ProxyTypeProvider(tp, Dictionary<_,_>()) :> _)
                
            member this.GetProvidedTypes(pn: IProvidedNamespace) =
                gotGetProvidedTypesRequest <- true
                pn :? ProxyProvidedNamespace |> should be True
                defaultExtensionTypingShim.GetProvidedTypes(pn)
        
            member this.ResolveTypeName(pn: IProvidedNamespace, typeName: string) =
                gotResolveTypeNameRequest <- true       
                pn :? ProxyProvidedNamespace |> should be True
                defaultExtensionTypingShim.ResolveTypeName(pn, typeName)
     
            member this.GetInvokerExpression(provider: ITypeProvider, methodBase: ProvidedMethodBase, paramExprs: ProvidedVar[]) =
                gotGetInvokerExpressionRequest <- true
                provider :? ProxyTypeProvider |> should be True
                defaultExtensionTypingShim.GetInvokerExpression(provider, methodBase, paramExprs)
                
            member this.DisplayNameOfTypeProvider(tp: ITypeProvider, fullName: bool) =
                gotDisplayNameOfTypeProviderRequest <- true
                tp :? ProxyTypeProvider |> should be True
                defaultExtensionTypingShim.DisplayNameOfTypeProvider(tp, fullName)
        }
        
    ExtensionTypingProvider <- extensionTypingProvider

    let result =
        parseAndCheckFile fileName """
module TypeProviderTests
open FSharp.Data

type Detailed = XmlProvider<"<developer>Alex</developer>">
let info = Detailed.Parse("<developer>Eugene</developer>")
"""      options
        |> snd

    gotInstantiateTypeProvidersOfAssemblyRequest |> should be True
    gotGetProvidedTypesRequest |> should be True
    gotResolveTypeNameRequest |> should be True
    gotGetInvokerExpressionRequest |> should be True
    //TODO: example with gotDisplayNameOfTypeProviderRequest |> should be True
    
    result.Diagnostics.Length = 0 |> should be True
    
    ExtensionTypingProvider <- defaultExtensionTypingShim
