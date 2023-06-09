module Test

type AB = 
    | A
    | B of int
    static let (|B0Pat|_|) value = if value = 0 then Some (B 999) else None
    static let b1 = B 1
    static member ParseUsingActivePattern x = match x with | B0Pat x -> x | _ -> A
    static member B1 = b1


let testThis = AB.ParseUsingActivePattern 0

printfn "%A" testThis