module Test

[<Struct>]
type R =
    {
        F1: int
        F2: int
    }

    static let cachedval = { F1 = 1; F2 = 1 }
    static let factoryFunc x = { F1 = x; F2 = x }
    static let mutable mutableVal = 0
    static let incrementor() = mutableVal <- mutableVal + 1

    static member IncrementAndReturn() = 
        do incrementor()
        let freshVal = factoryFunc mutableVal
        freshVal.F1 + cachedval.F1


let mutable lastVal = 0
for i=0 to 5 do 
    lastVal <- R.IncrementAndReturn()

printfn "%i" lastVal