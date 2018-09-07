module Test.Test

open System

let ss (x: int) =
    printfn "%A" x
    "_" + string x + "_"

let test () =
    let result = ss 5 + ss 6 + ss 7 + String.Concat(ss 8, ss 9) + ss 10 + "_50_" + "_60_" + String.Concat(ss 100, let x = String.Concat(ss 101, ss 102) in printfn "%s" x;x, ss 103) + String.Concat([|"_104_";"_105_"|]) + ss 106
    printfn "%A" result

[<EntryPoint>]
let main argv =
    let result = ss 5 + ss 6 + ss 7 + String.Concat(ss 8, ss 9) + ss 10 + "_50_" + "_60_" + String.Concat(ss 100, String.Concat(ss 101, ss 102), ss 103) + String.Concat([|"_104_";"_105_"|]) + ss 106
    printfn "%A" result
    0
