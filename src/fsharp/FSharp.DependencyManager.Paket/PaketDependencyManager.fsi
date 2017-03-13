namespace Microsoft.FSharp.DependencyManager.Paket
type [<Class>] FSharpDependencyManagerAttribute =
    inherit System.Attribute
    new : unit -> FSharpDependencyManagerAttribute
type [<Class>] PaketDependencyManager =
    interface System.IDisposable
    new : unit -> PaketDependencyManager
    member Name : string
    member ToolName: string
    member Key: string
    member ResolveDependencies : targetFramework: string * scriptDir: string * scriptName: string * dependencyManagerTextLines: string seq -> string * string list
