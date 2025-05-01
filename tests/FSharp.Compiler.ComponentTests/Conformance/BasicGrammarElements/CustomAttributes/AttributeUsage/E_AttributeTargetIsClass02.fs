open System

[<AttributeUsage(AttributeTargets.Class)>]
type ClassTargetAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Interface)>]
type InterfaceTargetAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Struct)>]
type StructTargetAttribute() =
    inherit Attribute()

[<InterfaceTarget>]
[<StructTarget>]
[<ClassTarget>]
type Record = { Prop: string }

[<ClassTarget>]
[<InterfaceTarget>]
[<StructTarget>]
[<Struct>]
type StructRecord = { Prop: string }

[<ClassTarget>]
[<InterfaceTarget>]
[<StructTarget>]
[<Struct>]
type UnionCase = 
    | UnionCase of a: int
    | UnionCase2 of string

[<ClassTarget>]
[<InterfaceTarget>]
[<StructTarget>]
[<Struct>]
type UnionCase2 = 
    | UnionCase of a: int * b: int
    | UnionCase2 of c: string * d: string

[<ClassTarget>]
[<InterfaceTarget>]
[<StructTarget>]
type StructUnionId = Id

[<ClassTarget>]
[<InterfaceTarget>]
[<StructTarget>]
[<Struct>]
type StructUnionId2 = Id