module Test

type R =
    {
        F1: int
        F2: int
    }

    static do printfn "init R 1"
    static let r1 = { F1 = 1; F2 = 1 }
    static member R1 = r1

    static do printfn "init R 2"
    static let r2 = { F1 = 1; F2 = 2 }
    static member val R2 = r2



printfn "%i" R.R1.F2
printfn "%i" R.R2.F2