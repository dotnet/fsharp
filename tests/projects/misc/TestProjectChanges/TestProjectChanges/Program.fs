// Learn more about F# at https://fsharp.org
// See the 'F# Tutorial' project for more help.

[<EntryPoint>]
let main argv =
    printfn "%A" argv
    let c1 = Library1AlwaysInMatchingConfiguration.Class.PropertyAlwaysAvailable
    let c1 = Library2AlwaysInDebugConfiguration.Class.PropertyAlwaysAvailable
#if DEBUG
    let c1 = Library1AlwaysInMatchingConfiguration.Class.PropertyAvailableInProject1DebugConfiguration
    let c2 = Library2AlwaysInDebugConfiguration.Class.PropertyAvailableInProject2DebugConfiguration
#else
    let c1 = Library1AlwaysInMatchingConfiguration.Class.PropertyAvailableInProject1ReleaseConfiguration
    let c2 = Library2AlwaysInDebugConfiguration.Class.PropertyAvailableInProject2DebugConfiguration
#endif


    0 // return an integer exit code
