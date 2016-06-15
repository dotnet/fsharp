
module Test

[<Struct>]
type StructRecord =
    {
        X: float
        Y: StructRecord
    }

[<Struct>]
type StructUnion = StructUnion of float * StructUnion 

[<Struct>]
type StructUnion2 = A of int | B of string
