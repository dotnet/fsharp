// Expected: Multiple warnings for module and type
module Module

[<AbstractClass>]
type AbstractBase() =
    abstract member Method : int -> int
    
    module InvalidModule = 
        let helper = 10
    
    type InvalidType = string
    
    default _.Method(x) = x * 2
