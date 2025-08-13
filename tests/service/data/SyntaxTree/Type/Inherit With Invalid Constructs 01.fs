// Expected: Warning for module after inherit
module Module

type Base() =
    member _.BaseMethod() = 1

type Derived() =
    inherit Base()
    
    module InvalidModule = 
        let x = 2
    
    override _.ToString() = "Derived"
