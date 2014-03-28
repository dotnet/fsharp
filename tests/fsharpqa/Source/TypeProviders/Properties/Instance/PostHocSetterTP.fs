// A very basic TP that exposed 1 type
// with a ctor and 1 prop with a setter

namespace TPT

open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.TypeProvider.Emit
 
module DesignTimeAPI =
    let typeWithIntProp = ProvidedTypeDefinition(System.Reflection.Assembly.GetExecutingAssembly(), "N", "T", Some(typeof<obj>))
    typeWithIntProp.AddMember(ProvidedConstructor([], InvokeCode = fun _ -> <@@ new obj() @@>))
    typeWithIntProp.AddMember(ProvidedProperty("IntProp", typeof<int>, SetterCode = (fun _ -> <@@ ignore 1 @@>)))
 
[<TypeProvider>]
type BadProvider() =
    inherit TypeProviderForNamespaces("N", [DesignTimeAPI.typeWithIntProp])
 
[<TypeProviderAssembly>]
do ()

