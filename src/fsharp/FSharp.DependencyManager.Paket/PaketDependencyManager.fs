namespace Microsoft.FSharp.DependencyManager.Paket

// used as a marker that compiler scans for, although there is no hard dependency, filtered by name
type FSharpDependencyManagerAttribute() =
  inherit System.Attribute()

type [<FSharpDependencyManager>] PaketDependencyManager() =
    member __.Name = "Paket"
    member __.ToolName = "paket.exe"
    member __.Key = "paket"
    member __.ResolveDependencies(targetFramework:string, scriptDir: string, scriptName: string, packageManagerTextLines: string seq) = 
        ReferenceLoading.PaketHandler.ResolveDependencies(
            targetFramework,
            scriptDir,
            scriptName,
            packageManagerTextLines)

    interface System.IDisposable with
        member __.Dispose() = ()
