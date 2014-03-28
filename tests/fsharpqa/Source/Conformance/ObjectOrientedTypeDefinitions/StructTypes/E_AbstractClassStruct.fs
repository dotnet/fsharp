// #Regression #Conformance #ObjectOrientedTypes #Structs 
#light

// Verify error when both AbstractClassAttribute and StructAttribute are on the same type.
//<Expects id="FS0926" span="(8,6)" status="error">The attributes of this type specify multiple kinds for the type</Expects>

[<Struct; AbstractClass>]
type StructPoint =
    val mutable m_x : float
    val mutable m_y : float
    
    new (x : float, y : float) = { m_x = x; m_y = y }
    
    member this.X with get() = this.m_x
                  and  set x = this.m_x <- x
    member this.Y with get() = this.m_y 
                  and  set x = this.m_y <- x

exit 1
