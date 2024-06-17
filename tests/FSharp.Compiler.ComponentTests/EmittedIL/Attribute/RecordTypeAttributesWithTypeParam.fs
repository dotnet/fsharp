module M
open System

type RecordLevelAttribute<^T>()=
    inherit Attribute()

type FieldLevelAttribute<^T>()=
    inherit Attribute()

[<RecordLevel<int>>]
type Test =
    {
        [<FieldLevel<string>>]
        someField : string
    }