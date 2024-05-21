open System

[<AttributeUsage(AttributeTargets.Interface)>]
type InterfaceTargetAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Struct)>]
type StructTargetAttribute() =
    inherit Attribute()

[<InterfaceTarget>] // Should this be allowed?
[<StructTarget>] // Should this be allowed?
type Record = { Prop: string }


[<ClassTarget>] // Should this be allowed?
[<InterfaceTarget>]// Should this be allowed?
[<StructTarget>]
[<Struct>]
type StructRecord = { Prop: string }