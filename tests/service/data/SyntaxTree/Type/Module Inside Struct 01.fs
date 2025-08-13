// Expected: Warning for module inside struct
module Module

[<Struct>]
type MyStruct =
    val X: int
    val Y: int
    module InvalidModule = 
        let helper = 10
