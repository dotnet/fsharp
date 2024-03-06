// This tests that AttributeTargets.Class is not allowed on a struct, and that AttributeTargets.Struct is not allowed on a class.

open System

[<AttributeUsage(AttributeTargets.Struct)>]
type CustomStructAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Class)>]
type CustomClassAttribute() =
    inherit Attribute()

[<CustomClass>]
type Struct(x: int) = struct end

[<CustomStruct>]
type Struct1(x: int) = struct end

[<CustomClass; CustomStruct>]
type Struct2(x: int) = struct end

[<Struct; CustomClass>]
type Struct4 = struct end

[<CustomClass>]
[<Struct>]
type Struct5 = struct end