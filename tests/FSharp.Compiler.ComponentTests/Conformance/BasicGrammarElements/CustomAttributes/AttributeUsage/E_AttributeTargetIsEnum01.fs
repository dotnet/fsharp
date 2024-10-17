open System

[<AttributeUsage(AttributeTargets.Struct)>]
type CustomStructAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Class)>]
type CustomClassAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Interface)>]
type CustomInterfaceAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Delegate)>]
type CustomDelegateAttribute() =
    inherit Attribute()

[<CustomStruct>]
[<CustomClass>]
[<CustomInterface>]
[<CustomDelegate>]
type Color =
    | Red = 0
    | Green = 1
    | Blue = 2