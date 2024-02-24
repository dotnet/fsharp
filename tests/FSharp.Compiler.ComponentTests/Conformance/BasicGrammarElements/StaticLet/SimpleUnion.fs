module Test

type MyDu = 
    | A of int 
    | B of string 
    | C

    static let factoryFunc x = A x
    static let mutable mutableVal = factoryFunc 11
    static let circularIncrement() = 
        mutableVal <- 
            match mutableVal with
            | A i -> B (string i)
            | B _ -> C
            | C -> A 42

    static member IncrementAndReturn() = 
        do circularIncrement()
        mutableVal


let mutable lastVal = C
for i=0 to 5 do 
    lastVal <- MyDu.IncrementAndReturn()

printfn "%A" lastVal