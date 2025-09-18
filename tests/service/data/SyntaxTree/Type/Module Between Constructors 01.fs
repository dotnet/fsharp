// Testing: Module between constructors
module Module

type MyClass(x: int) =
    new() = MyClass(0)
    
    module InvalidModule = 
        let x = 1
    
    new(s: string) = MyClass(1)
