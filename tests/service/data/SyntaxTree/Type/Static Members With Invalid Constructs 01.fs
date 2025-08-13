// Expected: Warning for module after static members
module Module

type MyType() =
    static member StaticMethod() = 42
    static member StaticProperty = "hello"
    
    module InvalidModule = 
        let x = 1
    
    static member AnotherStatic() = true
