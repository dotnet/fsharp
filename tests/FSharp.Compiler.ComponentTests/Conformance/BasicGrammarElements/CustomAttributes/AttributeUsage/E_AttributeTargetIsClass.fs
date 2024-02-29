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

[<RequireQualifiedAccess>] //RequireQualifiedAccess is marked AttributeTargets.Class
type SemanticClassificationItem =
    val Range: int
    val Type: string
    new((range, ty)) = { Range = range; Type = ty }

[<AutoOpen>] //AutoOpen is marked AttributeTargets.Class
type ILTableName(idx: int) =
    member __.Index = idx
    static member FromIndex n = ILTableName n