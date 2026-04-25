module PartialFsi.Main

[<EntryPoint>]
let main _ =
    let n = PartialFsi.Lib.multiply (PartialFsi.Util.triple 2) 5
    printfn "%s -> %d" PartialFsi.Lib.message n
    0
