open System

[<AttributeUsage(AttributeTargets.Struct)>]
type CustomStructAttribute() =
    inherit Attribute()

[<CustomStruct>]
type Class(x: int) = struct end

[<CustomStruct>]
[<Struct>]
type Class2 = struct end

[<RequireQualifiedAccess>]
type SemanticClassificationItem =
    val Range: int
    val Type: string
    new((range, ty)) = { Range = range; Type = ty }

[<AutoOpen>]
type ILTableName(idx: int) =
    member __.Index = idx
    static member FromIndex n = ILTableName n
