#nowarn "25" // incomplete match

namespace TPT

open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.TypeProvider.Emit

module DesignTimeAPI =
    let asm = System.Reflection.Assembly.GetExecutingAssembly()
    let t = ProvidedTypeDefinition(asm, "N", "T", None)
    t.DefineStaticParameters([ProvidedStaticParameter("ty", typeof<string>); ProvidedStaticParameter("n", typeof<int>)], 
                                fun nm [|:? string as ty; :? int as n|] ->
                                    let ty = System.Type.GetType(ty)
                                    let arrTy = ty.MakeArrayType n
                                    let defn = ProvidedTypeDefinition(asm, "N", nm, Some arrTy)
                                    defn)

[<TypeProvider>]
type Provider() =
    inherit TypeProviderForNamespaces("N", [DesignTimeAPI.t])
 

[<assembly:Microsoft.FSharp.Core.CompilerServices.TypeProviderAssembly>] 
do()

 
