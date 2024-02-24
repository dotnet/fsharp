module Test

type MyDu = 
    | A 
    | Other

    static let ofString s = match s with | "A" -> A | _ -> Other
    static member Q = <@ ofString "A" @>

let myQ = MyDu.Q

printfn "%A" myQ