// A mock TP that tries to expose a namespace which contains illegal chars
// 

namespace TestTypeProvider

open System.Linq.Expressions
open Microsoft.FSharp.TypeProvider.Emit
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations

type RuntimeAPI = 
    static member AddInt(x:int,y:int) = x+y

module InvokeAPI = 
    let addIntX (args : Quotations.Expr list) =
        match Seq.length args with
        | 2 -> <@@ RuntimeAPI.AddInt (%%args.[0], %%args.[1]) @@>
        | _ -> failwithf "addIntX: expect arity 2 (got %d)" (Seq.length args)

module TestTypeProviderModule = 
    open Microsoft.FSharp.TypeProvider.Emit
        
    let namespaceName = "A+B"  // <-- invalid namespace: + is not allowed!  
    //let container =
    let hereAssembly  = System.Reflection.Assembly.GetExecutingAssembly()
        //let hereModule    = hereAssembly.GetModules().[0]        
        //TypeContainer.Namespace(hereModule,namespaceName)

    let typeA = ProvidedTypeDefinition(hereAssembly, namespaceName, "T", Some typeof<System.Object>)
    let methA = ProvidedMethod("M", [ProvidedParameter("x1", typeof<int>); ProvidedParameter("x2", typeof<int>)], typeof<int>, IsStaticMethod=true, InvokeCode=InvokeAPI.addIntX)
    
    typeA.AddMember methA

    let types = [ typeA ]  // The non-nested provided types

[<Microsoft.FSharp.Core.CompilerServices.TypeProvider>]
type TestTypeProvider() = 
    inherit TypeProviderForNamespaces(TestTypeProviderModule.namespaceName,TestTypeProviderModule.types)
                            
[<assembly:Microsoft.FSharp.Core.CompilerServices.TypeProviderAssembly>]
do()
