// Testing: Module after static members
module Module

type MyType =
    static member StaticMethod() = 42
    
    module InvalidModule = 
        let x = 1
