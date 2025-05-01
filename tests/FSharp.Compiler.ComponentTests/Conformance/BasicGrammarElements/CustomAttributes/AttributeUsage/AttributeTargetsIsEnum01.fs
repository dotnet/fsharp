open System

[<AttributeUsage(AttributeTargets.Enum)>]
type CustomEnumAttribute() =
    inherit Attribute()

[<CustomEnum>]
type Color =
    | Red = 0
    | Green = 1
    | Blue = 2
