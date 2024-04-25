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
