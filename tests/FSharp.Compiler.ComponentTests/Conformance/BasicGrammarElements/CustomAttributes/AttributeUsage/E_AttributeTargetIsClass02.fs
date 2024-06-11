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