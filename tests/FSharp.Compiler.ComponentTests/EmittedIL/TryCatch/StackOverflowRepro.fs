module StackOverflowRepro
open System

[<return: Struct>]
let (|RecoverableException|_|) (exn: Exception) =
    match exn with
    | :? OperationCanceledException -> ValueNone
    | _ ->
        ValueSome exn

let rec viaActivePattern (a:int) =
    try
        if a = 0
        then
            raise (OperationCanceledException())
        else
            viaActivePattern (a - 1)
    with
    | RecoverableException e -> 
        let x = struct(a,a,a,a)
        let y = struct(x,a,x)
        y.GetHashCode() + e.GetHashCode()



[<EntryPoint>]
let main (args:string[]) =
    let iterations = 4096
    try 
        viaActivePattern iterations
    with
    | ex -> 
        printf "%s" (ex.GetType().ToString())
        0