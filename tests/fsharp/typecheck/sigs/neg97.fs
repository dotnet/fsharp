
module Test

[<Struct;CLIMutable>]
type StructRecord =
    {
        X: float
        mutable Y: float
    }

let x = { X = 1.; Y = 1. }

x.Y <- 5.

