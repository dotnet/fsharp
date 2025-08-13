// Expected: Warning for module between constructors
module Module

type MyClass(primary: int) =
    let mutable value = primary
    
    new() = MyClass(0)
    
    module InvalidModule = 
        let x = 1
    
    new(s: string) = MyClass(int s)
    
    member _.Value = value
