module Test

type R =
    {
        F1: int
        F2: int
    }

    static do printfn "init R 1"
    static let r1 = 
        do printfn "side effect in let binding R1"
        { F1 = 1; F2 = 1 }
    static member R1 = 
        do printfn "side effect in member R1"
        r1


module InnerModule = 
    let print() = printfn "calling print %i" R.R1.F2


printfn "Before accessing type"
InnerModule.print()
InnerModule.print()


