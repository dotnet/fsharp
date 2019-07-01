// Learn more about F# at https://fsharp.org
// See the 'F# Tutorial' project for more help.

[<EntryPoint>]
let main argv =
    printfn "%A" argv
    let c1 = Library1.Class.PropertyAlwaysAvailable
    let c1 = Library2.Class.PropertyAlwaysAvailable
    0 // return an integer exit code
