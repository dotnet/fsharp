// Learn more about F# at https://fsharp.org
// See the 'F# Tutorial' project for more help.


let private x : int = failwith ""

let inline f () = x

let f1 argv =
    //let x = 1 + 1.0
    let y = argx // this gives a multi-line error message
    printfn "%A" argv
    0 // return an integer exit code

[<EntryPoint>]
let main argv =
    //let x = 1 + 1.0
    //let y = argx // this gives
    printfn "%A" argv
    0 // return an integer exit code
