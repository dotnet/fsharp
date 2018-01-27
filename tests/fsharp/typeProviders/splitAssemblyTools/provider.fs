namespace Microsoft.FSharp.Core.CompilerServices

type TypeProviderAssemblyAttribute(assemblyName) =
    inherit System.Attribute()

    new() = TypeProviderAssemblyAttribute(null)
    member this.AssemblyName 
        with get () = assemblyName 

[<assembly:TypeProviderAssembly("providerDesigner")>]
do()


namespace My
    type Runtime =
        static member Id x = x

