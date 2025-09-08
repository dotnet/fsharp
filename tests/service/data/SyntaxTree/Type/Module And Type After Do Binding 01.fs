// Testing: Invalid constructs after do binding
module Module

type ClassWithDo() =
    do 
        printfn "init"
    
    type InternalType = int
    
    module InternalModule = 
        let x = 1
    
    open System.Collections
