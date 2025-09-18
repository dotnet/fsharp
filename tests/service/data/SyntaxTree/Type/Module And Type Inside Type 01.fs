// Testing: Module and type declarations inside a type
module Module

type MyType =
    module InvalidModule = 
        let helper = 10
    
    type InvalidType = string
