open System

[<AttributeUsage(AttributeTargets.Struct)>]
type CustomStructAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Class)>]
type CustomClassAttribute() =
    inherit Attribute()

[<CustomClass>]
type IFoo = interface end

[<CustomStruct>]
type IFoo2 =
    abstract A :int
 
[<CustomClass>] 
[<CustomStruct>]
[<Interface>]
type IFoo3 =
    abstract A :int