module Test

type AB = 
    | A
    | B of int

    static let (|B0PatPrivate|_|) value = if value = 0 then Some (B 999) else None
    static member ParseUsingActivePattern x = match x with | B0PatPrivate x -> x | _ -> A



let testThis = AB.ParseUsingActivePattern 0

printfn "%A" testThis