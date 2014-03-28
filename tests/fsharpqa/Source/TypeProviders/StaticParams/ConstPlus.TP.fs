// A mock TP that exposes a type that takes a static param

namespace TestTypeProvider

open System.Linq.Expressions
open Microsoft.FSharp.TypeProvider.Emit
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations

type RuntimeAPI = 
    static member Identity(x : int) = x

module TestTypeProviderModule = 

    let namespaceName = "N1"    
    let thisAssembly  = System.Reflection.Assembly.GetExecutingAssembly()

    // A parametric type N1.T
    let typeT = ProvidedTypeDefinition(thisAssembly,namespaceName,"T",Some typeof<System.Object>)

    // A parametric type N1.P
    let typeP = ProvidedTypeDefinition(thisAssembly,namespaceName,"P",Some typeof<System.Object>)

    // Make an instantiation of the parametric type
    // THe instantiated type has a static property that evaluates to the param passed
    let instantiateParametricType (typeName:string) (args:System.Object[]) =
        match args with
        | [| :? int as value |] -> 
            let typeParam = ProvidedTypeDefinition(thisAssembly,namespaceName, typeName, Some typeof<System.Object>)     
            let propParam = ProvidedProperty("Param1", typeof<int>, 
                                             IsStatic = true,
                                             // A complicated was to basically return the constant value... Maybe there's a better/simpler way?
                                             GetterCode = fun _ -> <@@ RuntimeAPI.Identity(value) @@>)
            typeParam.AddMember(propParam :>System.Reflection.MemberInfo)
            typeParam 
        | [| :? string as value |] -> 
            let typeParam = ProvidedTypeDefinition(thisAssembly,namespaceName, typeName, Some typeof<System.Object>)     
            let propParam = ProvidedProperty("Param2", typeof<string>, 
                                             IsStatic = true,
                                             // A complicated was to basically return the constant value... Maybe there's a better/simpler way?
                                             GetterCode = fun _ -> <@@ "Returned " + value @@>)
            typeParam.AddMember(propParam :>System.Reflection.MemberInfo)
            typeParam
        | _ -> failwithf "instantiateParametricType: unexpected params %A" args
    
    // N1.T<int>
    typeT.DefineStaticParameters( [ ProvidedStaticParameter("Param1", typeof<int>) ], instantiateParametricType )

    // N1.P<string>
    typeP.DefineStaticParameters( [ ProvidedStaticParameter("Param2", typeof<string>) ], instantiateParametricType )

    let types = [ typeT; typeP ]  // The non-nested provided types

[<Microsoft.FSharp.Core.CompilerServices.TypeProvider>]
type TestTypeProvider() = 
    inherit TypeProviderForNamespaces(TestTypeProviderModule.namespaceName,TestTypeProviderModule.types)
                            
[<assembly:Microsoft.FSharp.Core.CompilerServices.TypeProviderAssembly>]
do()
