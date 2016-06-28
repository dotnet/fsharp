
module Test

[<Struct;CLIMutable>]
type StructRecord =
    {
        X: float
        Y: float
    }

let x = StructRecord ()
