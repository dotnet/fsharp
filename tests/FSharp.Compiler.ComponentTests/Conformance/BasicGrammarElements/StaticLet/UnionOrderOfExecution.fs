module Test

type U =
    | Case1
    | Case2 of int

    static do printfn "init type U"
    static let case2cachedVal = 
        do printfn "side effect in let binding case2cachedVal"
        Case2 42
    static member GetSingleton = 
        do printfn "side effect in member Singleton"
        case2cachedVal


module InnerModule = 
    let print() = printfn "calling print %A" (U.GetSingleton)


printfn "Before accessing type"
InnerModule.print()
InnerModule.print()