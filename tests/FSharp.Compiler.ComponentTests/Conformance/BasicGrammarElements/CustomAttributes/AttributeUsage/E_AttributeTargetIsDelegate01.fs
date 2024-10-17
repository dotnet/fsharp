open System

[<AttributeUsage(AttributeTargets.Class)>]
type CustomClassAttribute() =
    inherit Attribute()
    
[<AttributeUsage(AttributeTargets.Struct)>]
type CustomStructAttribute() =
    inherit Attribute()
    
[<AttributeUsage(AttributeTargets.Interface)>]
type CustomInterfaceAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Enum)>]
type CustomEnumAttribute() =
    inherit Attribute()

[<CustomClass>]
[<CustomStruct>]
[<CustomInterface>]
[<CustomEnum>]
type Delegate1 = delegate of int -> int