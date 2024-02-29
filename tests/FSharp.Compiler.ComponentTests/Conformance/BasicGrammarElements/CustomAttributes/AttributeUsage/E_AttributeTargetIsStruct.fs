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

[<RequireQualifiedAccess>] //RequireQualifiedAccess is marked AttributeTargets.Class. so this should be an error
[<Struct>]
type SemanticClassificationItem =
    val Range: int
    val Type: string
    new((range, ty)) = { Range = range; Type = ty }

[<AutoOpen>] //AutoOpen is marked AttributeTargets.Class so this should be an error
[<Struct>]
type ILTableName(idx: int) =
    member __.Index = idx
    static member FromIndex n = ILTableName n