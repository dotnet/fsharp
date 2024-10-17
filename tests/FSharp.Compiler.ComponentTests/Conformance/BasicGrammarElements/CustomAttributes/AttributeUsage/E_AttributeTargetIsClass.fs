// This tests that AttributeTargets.Struct is not allowed on a class, and that AttributeTargets.Class is not allowed on a struct.

open System

[<AttributeUsage(AttributeTargets.Struct)>]
type CustomStructAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Class)>]
type CustomClassAttribute() =
    inherit Attribute()

[<CustomStruct>]
type Class(x: int) = class end

[<CustomClass>]
type Class2(x: int) = class end

[<CustomStruct; CustomClass>]
type Class3(x: int) = class end

[<Class; CustomStruct>]
type Class4 = class end