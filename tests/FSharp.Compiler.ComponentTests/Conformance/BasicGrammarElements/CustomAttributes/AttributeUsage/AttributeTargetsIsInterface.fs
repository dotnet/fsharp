open System

[<AttributeUsage(AttributeTargets.Interface)>]
type CustomInterfaceAttribute() =
    inherit Attribute()

[<CustomInterface>]
type IFoo = interface end

[<CustomInterface>]
type IFoo2 =
    abstract A :int
 
[<CustomInterface>]  
[<Interface>]
type IFoo3 =
    abstract A :int
