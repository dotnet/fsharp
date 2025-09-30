// Testing: Module after inherit
module Module

type Base() = class end

type Derived() =
    inherit Base()
    
    module InvalidModule = 
        let x = 2
