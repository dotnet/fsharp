// compile as: fsc --target:library ProvidedTypes.fsi ProvidedTypes.fs Mirror.fs
namespace Repro

open System.IO
open System.Reflection
open ProviderImplementation.ProvidedTypes
open Microsoft.FSharp.Core.CompilerServices

[<assembly : TypeProviderAssembly>]
do()

[<TypeProvider>]
type Mirror() as this =
    inherit TypeProviderForNamespaces()
    let thisAssembly = typeof<Mirror>.Assembly
    let topType = ProvidedTypeDefinition(thisAssembly, "Top", "Repro", Some typeof<obj>, IsErased = true)
    let csAssembly = 
        let location = Path.GetDirectoryName(thisAssembly.Location)
        System.Reflection.Assembly.LoadFrom(Path.Combine(location, "library.dll"))
    do topType.AddAssemblyTypesAsNestedTypesDelayed(fun() -> csAssembly)
    do this.AddNamespace("Top", [topType])