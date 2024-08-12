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

[<RequireQualifiedAccess>]
[<Struct>]
type StructRecord = { Prop: string }

[<RequireQualifiedAccess>]
[<Struct>]
type StructUnion =
    | StructUnionCase of a: int
    | StructUnionCase2 of string

[<RequireQualifiedAccess>]
[<Struct>]
type StructUnionId = Id

[<CustomStruct>]
[<Struct>]
type StructUnionId2 = Id

[<CustomStruct>]
[<Struct>]
type Union1 = 
    | UnionCase of a: int
    | UnionCase2 of string

[<CustomStruct>]
[<Struct>]
type Union2 = 
    | UnionCase of a: int * b: int
    | UnionCase2 of c: string * d: string