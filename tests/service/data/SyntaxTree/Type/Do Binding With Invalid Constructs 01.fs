// Expected: Multiple warnings for type, module, and open
module Module

type ClassWithDoBinding() =
    let mutable initialized = false
    
    do 
        printfn "Starting initialization"
        initialized <- true
    
    type InternalType = int
    
    do 
        printfn "More initialization"
    
    module InternalModule = 
        let x = 1
    
    open System.Collections
    
    member _.IsInitialized = initialized
