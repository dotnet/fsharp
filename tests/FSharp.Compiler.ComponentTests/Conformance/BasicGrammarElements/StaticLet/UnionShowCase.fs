module Test

type U =
    | Case1
    | Case2 of int

    static do printfn "init U 1"
    static let u1 = Case2 1
    static member U1 = u1

    static do printfn "init U 2"
    static let u2 = Case2 2
    static member val U2 = u2
    
    static do printfn "init end"

printfn "%A" U.U1
printfn "%A" U.U2