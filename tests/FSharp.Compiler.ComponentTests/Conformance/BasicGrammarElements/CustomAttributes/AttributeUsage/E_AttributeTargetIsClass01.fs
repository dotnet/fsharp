open System

[<AttributeUsage(AttributeTargets.Interface)>]
type CustomInterfaceAttribute() =
    inherit Attribute()

[<CustomInterface>]
type Class(x: int) = class end

[<Class; CustomInterface>]
type Class3 = class end