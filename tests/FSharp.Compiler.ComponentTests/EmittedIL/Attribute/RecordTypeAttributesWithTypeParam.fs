module M
open System

type RecordLevelAttribute<^T>()=
    inherit Attribute()

//[<System.AttributeUsage(System.AttributeTargets.Property)>]
type FieldLevelAttribute<^T>()=
    inherit Attribute()

[<RecordLevel<int>>]
type Test =
    {
        [<FieldLevel<string>>]
        someField : string
    }