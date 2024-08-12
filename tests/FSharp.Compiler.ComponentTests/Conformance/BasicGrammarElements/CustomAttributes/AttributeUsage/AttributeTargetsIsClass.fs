open System

[<AttributeUsage(AttributeTargets.Class)>]
type CustomClassAttribute() =
    inherit Attribute()

[<CustomClass>]
type Class(x: int) = class end

[<CustomClass>]
[<Class>]
type Class2 = class end

[<CustomClass>]
type Class3() = class end

[<RequireQualifiedAccess>]
type SemanticClassificationItem =
    val Range: int
    val Type: string
    new((range, ty)) = { Range = range; Type = ty }

[<AutoOpen>]
type ILTableName(idx: int) =
    member __.Index = idx
    static member FromIndex n = ILTableName n

[<RequireQualifiedAccess>]
[<CustomClass>]
type Record = { Prop: string }

[<RequireQualifiedAccess>]
[<CustomClass>]
type Record2 = { Prop: string }

[<RequireQualifiedAccess>]
type ClassUnion =
    | StructUnionCase of int
    | StructUnionCase2 of string

[<RequireQualifiedAccess>]
type ClassUnionId = Id

[<CustomClass>]
type ClassUnionId2 = Id

[<CustomClass>]
type UnionCase = 
    | UnionCase of int
    | UnionCase2 of string
