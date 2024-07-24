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

[<AttributeUsage(AttributeTargets.Interface)>]
type InterfaceTargetAttribute() =
    inherit Attribute()

[<CustomClass>]
[<InterfaceTarget>]
[<CustomStruct>]
type UnionCase = 
    | UnionCase of int
    | UnionCase2 of string

[<CustomClass>]
[<InterfaceTarget>]
[<CustomStruct>]
[<Struct>]
type UnionCase2 = 
    | UnionCase of a: int * b: int
    | UnionCase2 of c: string * d: string

[<CustomClass>]
[<InterfaceTarget>]
[<CustomStruct>]
type StructUnionId = Id

[<CustomClass>]
[<InterfaceTarget>]
[<CustomStruct>]
[<Struct>]
type StructUnionId2 = Id