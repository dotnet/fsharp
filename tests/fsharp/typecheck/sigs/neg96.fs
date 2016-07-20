
module Test

[<Struct;CLIMutable>]
type StructRecord =
    {
        X: float
        Y: float
    }

let x = StructRecord ()

let invalidUse() = 
    use mutable x = (null : System.IDisposable)
    ()


type T = X<__SOURCE_DIRECTORY__>

